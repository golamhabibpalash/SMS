﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.ViewModels.AdministrationVM
{
    public class UserRoleVM
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public char UserType { get; set; }
        public bool IsSelected { get; set; }
    }
}
