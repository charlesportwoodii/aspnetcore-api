using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace App.Forms
{
    using App.DataContext;
    using App.Models;

    public class LoginForm
    {
        private User user = null;

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength=8)]
        public string password { get; set; }

        [StringLength(6)]
        public string otp { get; set; }

        private Token token { get; set; }

        public bool Login(IDistributedCache cache)
        {
            if (this.GetUser() == null) {
                return false;
            }

            if (this.user.ValidatePassword(this.password)) {
                this.token = new Token(cache);
                return true;
            }

            return false;
        }

        public Token GetToken()
        {
            return this.token;
        }
        
        private User GetUser()
        {
            if (this.user == null) {
                using (var context = new DatabaseContext()) {
                    this.user = context.User
                        .Where(b => b.email == this.email)
                        .FirstOrDefault();
                }
            }            

            return this.user;
        }
    }
}