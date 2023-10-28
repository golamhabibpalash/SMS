using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class ExpenseType:CommonProps
    {
        public string ExpenseTypeName { get; set; }
        public List<Expense> Expenses { get; set; }
        public string Description { get; set; }
    }
}
