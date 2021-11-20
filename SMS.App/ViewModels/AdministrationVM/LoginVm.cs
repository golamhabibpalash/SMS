using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.ViewModels.AdministrationVM
{
    public class LoginVm
    {
        [EmailAddress, Required]
        public string Email { get; set; }

        [DataType(DataType.Password), Required]
        public string Password { get; set; }

        [Display(Name ="Remember me")]
        public bool RememberMe { get; set; }
    }
}
