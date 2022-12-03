using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.App.ViewModels.InstituteVM
{
    public class InstituteTimeVM
    {
        [Display(Name=("Starting Time"))]
        [Column(TypeName ="time")]
        public TimeOnly StartingTime { get; set; }

        [Display(Name = ("Closing Time"))]
        [Column(TypeName = "time")]
        public TimeOnly ClosingTime { get; set; }

        [Display(Name = ("Late Start Time"))]
        [Column(TypeName = "time")]
        public TimeOnly LateTimeStart { get ; set; }

        public IEquatable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (LateTimeStart>StartingTime)
            {
                results.Add(new ValidationResult("Late Start time must be higher than Starting Time"));
            }
            return (IEquatable<ValidationResult>)results;
        }
    }
}
