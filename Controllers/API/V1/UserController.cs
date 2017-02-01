using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.IO;
using Newtonsoft.Json;

namespace App.Controllers
{
    using App.Forms;

    public class UserController : Controller
    {
        private readonly ILogger _logger;
        private readonly IDistributedCache _cache;

        public UserController(ILogger<UserController> logger, IDistributedCache cache)
        {
            this._logger = logger;
            this._cache = cache;
        }

        [HttpPost]
        public IActionResult authenticate([FromBody] LoginForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.Login(_cache)) {     
                    return new JsonResult(new {
                        ikm = form.GetToken().ikm,
                        salt = form.GetToken().salt,
                        access_token = form.GetToken().access_token,
                        refresh_token = form.GetToken().refresh_token,
                        expires_at = form.GetToken().expires_at
                    });
                }
                
                return new UnauthorizedResult();
            }

            return new BadRequestResult();
        }

        [HttpPost]
        public IActionResult register([FromBody] RegisterForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.register()) {
                    return new JsonResult(new {
                        result = true
                    });
                }

                return new JsonResult(new {
                    result = false
                });
            }

            return new BadRequestResult();
        }

        [HttpPost, Authorize]
        public IActionResult refresh()
        {
            // Pull the claims information
            var access_token = User.FindFirst("access_token").Value;
            var user_id = User.FindFirst("user_id").Value;

            // Fetch the token data
            var token = new App.Models.Token(this._cache, access_token);

            // Get the raw body
            string body = new System.IO.StreamReader(HttpContext.Request.Body).ReadToEnd();
            byte[] requestData = System.Text.Encoding.UTF8.GetBytes(body);
            HttpContext.Request.Body = new System.IO.MemoryStream(requestData);
            var requestBody = JsonConvert.DeserializeObject<IDictionary<string, string>>(body);

            // Make sure the data matches
            if (requestBody["refresh_token"] == token.refresh_token) {
                // Create a new token
                var newT = new App.Models.Token(this._cache, int.Parse(user_id));

                // Delete the only token
                token.Delete();

                // Return the refreshed data
                return new JsonResult(new {
                    ikm = token.ikm,
                    salt = token.salt,
                    access_token = token.access_token,
                    refresh_token = token.refresh_token,
                    expires_at = token.expires_at
                });
            }

            return new UnauthorizedResult();
        }
    }
}