using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.App.ViewModels.ExamVM;
using SMS.Entities;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace SMS.App.ViewModels.AcademicVM
{
    public class AcademicExamGroupVM : AcademicExamGroup
    {
        public List<AcademicExamGroupIndexVM> AcademicExamGroupIndexVMList { get; set; } = new List<AcademicExamGroupIndexVM> { };
        public List<SelectListItem> ExamTypeList { get; set; }
        public List<SelectListItem> AcademicSessionList { get; set; }

    }
}
