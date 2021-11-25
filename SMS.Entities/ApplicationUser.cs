using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SMS.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public char UserType { get; set; } //e =employee, s =student
        public int ReferenceId { get; set; }
    }
}
