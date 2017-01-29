using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ASPNetCoreAPI.Forms
{
    using ASPNetCoreAPI.DataContext;
    using ASPNetCoreAPI.Models;

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

        public bool login()
        {
            if (this.GetUser() == null) {
                return false;
            }
            
            return true;
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