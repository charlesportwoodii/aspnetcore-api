using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace ASPNetCoreAPI.Controllers
{
    using ASPNetCoreAPI.DataContext;
    using ASPNetCoreAPI.Forms;

    public class UserController : Controller
    {
        private readonly ILogger _logger;

        public UserController(ILogger<UserController> logger)
        {
            this._logger = logger;
        }

        [HttpPost]
        public IActionResult authenticate([FromBody] LoginForm form)
        {
            if (ModelState.IsValid)
            {
                if (form.login()) {
                    return new JsonResult(new {
                        ikm = 1,
                        salt = 2,
                        access_token = 3,
                        refresh_token = 4
                    });
                }
                
                return new UnauthorizedResult();
            }

            return new BadRequestResult();
        }
    }
}