using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identica.Models
{
    public class UserModel
    {
        [MaxLength(30)]
        [MinLength(6)]
        public string Username { get; set; }

        [Required(ErrorMessage = "The email field is mandatory")]
        [MaxLength(30)]
        [MinLength(6)]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password field is mandatory")]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
