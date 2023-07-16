using SMS.Entities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.App.ViewModels.AcademicVM;

public class AcademicExamVM : AcademicExam
{
    public List<AcademicExam> AcademicExams { get; set; }
    public List<SelectListItem> AcademicExamGroupList { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> AcademicClassList { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> AcademicSectionList { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> AcademicSubjectList { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> TeacherList { get; set; } = new List<SelectListItem>();
}

