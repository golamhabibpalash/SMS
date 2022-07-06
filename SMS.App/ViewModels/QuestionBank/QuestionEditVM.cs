using SMS.Entities;
using System.Collections.Generic;

namespace SMS.App.ViewModels.QuestionBank
{
    public class QuestionEditVM
    {
        public Question Question { get; set; }
        //public List<QuestionDetails> QuestionDetails { get; set; }
        public int AcademicClassId { get; set; }
        public int AcademicSubjectId { get; set; }
    }
}
