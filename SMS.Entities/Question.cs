using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Question : CommonProps
    {
        public string Uddipok { get; set; }
        public byte[] Image { get; set; }
        public string ImagePosition { get; set; }
        public int AcademicClassId { get; set; }
        public AcademicClass AcademicClass { get; set; } = new AcademicClass();
        public int AcademicSubjectId { get; set; }
        public AcademicSubject AcademicSubject { get; set; } =new AcademicSubject();
        public int ChapterId { get; set; }
        public virtual ICollection<QuestionDetails> QuestionDetails { get; set; }
    }
}
