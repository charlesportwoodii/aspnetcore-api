using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ASPNetCoreAPI.Controllers
{
    using ASPNetCoreAPI.DataContext;
    using ASPNetCoreAPI.Forms;
    using ASPNetCoreAPI.Models;
    using BCrypt.Net;

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
    }
}