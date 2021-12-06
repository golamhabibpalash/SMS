using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.ViewModels.AdministrationVM
{
    public class ResetPasswordVM
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        
        [DataType(DataType.Password), Display(Name ="Confirm Password"), Compare("Password", ErrorMessage ="Password should be same")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
