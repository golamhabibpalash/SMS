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
        public string Image { get; set; }
        public string ImagePosition { get; set; }
        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }
        public virtual ICollection<QuestionDetails> QuestionDetails { get; set; }
    }
}
