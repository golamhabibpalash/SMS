using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Expense:CommonProps
    {
        public string ExpenseName { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int ExpenseTypeId { get; set; }
        public ExpenseType ExpenseType { get; set; }
    }
}
