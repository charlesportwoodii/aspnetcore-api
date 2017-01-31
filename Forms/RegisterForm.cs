using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace App.Forms
{
    using App.DataContext;
    using App.Models;
    using BCrypt.Net;

    public class RegisterForm
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength=8)]
        public string password { get; set; }

        public bool register()
        {
            var result = false;
            User user = new User();
            user.email = this.email;
            user.password = BCrypt.HashPassword(this.password);

            using (var context = new DatabaseContext()) {
                context.User.Add(user);
                if (context.SaveChanges() == 1) {
                    result = true;
                }
            }

            return result;
        }
    }
}