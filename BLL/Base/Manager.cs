using DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Base
{
    public class Manager<T> where T:class
    {
        private readonly ApplicationDbContext db;
        public Manager(ApplicationDbContext db)
        {
            this.db = db;
        }
    }
}
