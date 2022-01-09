using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.App.ViewModels.InstituteVM
{
    public class InstituteTimeVM
    {
        [Display(Name=("Starting Time"))]
        public DateTime StartingTime { get; set; }

        [Display(Name = ("Starting Time"))]
        public DateTime ClosingTime { get; set; }

        [Display(Name = ("Late Start Time"))]
        public DateTime LateTimeStart { get ; set; }

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
