using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class GradingTableHist:CommonProps
    {
        [Range(0, 100, ErrorMessage = "The value must be Greater than 0 and Smaller than 100")]
        [Display(Name = "Number Range(Min)")]
        public int NumberRangeMin { get; set; }
        [Range(0, 100, ErrorMessage = "The value must be Greater than 0 and Smaller than 100")]
        [Display(Name = "Number Range(Max)")]
        public int NumberRangeMax { get; set; }
        [StringLength(2, ErrorMessage = "Write a suitable Grade by letter")]
        [Display(Name = "Letter Grade"), Required]
        public string LetterGrade { get; set; }
        [Display(Name = "Grade Point"), Required]
        public decimal GradePoint { get; set; }
        public string gradeComments { get; set; }
        public int AcademicExamGroupId { get; set; }
        public AcademicExamGroup AcademicExamGroup { get; set; }
    }
}
