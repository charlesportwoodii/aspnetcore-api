using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;

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
                        refresh_token = form.GetToken().refresh_token
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

        //[HttpPost, Authorize]
        public IActionResult refresh()
        {
            var ikm = Convert.FromBase64String("jdD1MhZVrymbmrgsFjjZZ1K9UCMbwYV7iqSReUap84g=");
            var salt = Convert.FromBase64String("s70zjOMenuCxIBeba5ya82yryWsiYMuQUtpEZOz5he4=");
            var info = Convert.FromBase64String("SE1BQ3xBdXRoZW50aWNhdGlvbktleQ==");
            var expected = Convert.FromBase64String("i4CxlCDCojxZHILXS6HIaL3wlTAiclTxo5W/0kKCFPY=");
            int l = 32;

            var result = new App.HKDF(HashAlgorithmName.SHA256, ikm, info, l, salt);

            return new JsonResult(new {
                expected = expected,
                actual = result
            });
            return new JsonResult(new {
                success = true
            });
        }
    }
}