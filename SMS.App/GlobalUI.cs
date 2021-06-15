using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem
{
    public class GlobalUI
    {
        public int Id { get; set; }
        public static string PageTitle { get; set; }
        public static string InstituteName { get; set; }
        public static string SiteTitle { get; set; }
        public static byte[] Favicon { get; set; }
        public static byte[] Logo { get; set; }
    }
}
