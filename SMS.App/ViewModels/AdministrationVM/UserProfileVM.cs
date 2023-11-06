using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Entities;
using System.Collections.Generic;

namespace SMS.App.ViewModels.AdministrationVM
{
    public class UserProfileVM
    {
        public Employee Employee { get; set; } 
        public UserRoleClaimsVM UserRoleClaimsVM { get; set; }
        public SelectList UserList { get; set; }
    }
}
