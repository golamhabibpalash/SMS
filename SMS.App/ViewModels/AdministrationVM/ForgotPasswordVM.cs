using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.ViewModels.AdministrationVM
{
    public class ForgotPasswordVM
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
        public string verificationBy { get; set; }
    }
}
