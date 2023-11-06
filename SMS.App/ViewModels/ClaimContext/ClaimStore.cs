using SMS.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace SMS.App.ViewModels.ClaimContext
{
    public static class ClaimStore
    {
        public static List<Claim> All = new List<Claim>()
        {
            new Claim("CreateRole", "Create Role".ToString()),
            new Claim("EditRole", "Edit Role".ToString()),
            new Claim("DeleteRole", "Delete Role".ToString())
        };
    }
}
