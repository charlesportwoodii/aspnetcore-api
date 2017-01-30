using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    using BCrypt.Net;
    public class User
    {
        [Key]
        public int id { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime created_at { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime updated_at { get; set; }

        public bool ValidatePassword(string password)
        {
            return BCrypt.Verify(password, this.password);
        }
    }
}