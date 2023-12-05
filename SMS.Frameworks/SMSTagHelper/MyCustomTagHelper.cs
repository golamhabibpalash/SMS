using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Frameworks.SMSTagHelper
{
    public class MyCustomTagHelper:TagHelper
    {
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Designation { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "EmployeeSectionTagHelper";
            output.TagMode = TagMode.StartTagAndEndTag; 
            var sb = new StringBuilder();
            sb.AppendFormat($"<span>Employee Id:</span><strong>{EmployeeId}</strong> <br/>");
            sb.AppendFormat($"<span>Employee Name:</span><strong>{EmployeeName}</strong> <br/>");
            sb.AppendFormat($"<span>Employee Designation:</span><strong>{Designation}</strong> <br/>");
            output.Content.SetHtmlContent( sb.ToString() );
        }
    }
}
