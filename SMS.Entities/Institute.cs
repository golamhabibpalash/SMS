using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Institute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ISO { get; set; }
        public string EIIN { get; set; }
        public string Slogan { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public string Address { get; set; }
        public string BranchName { get; set; }
    }
}
