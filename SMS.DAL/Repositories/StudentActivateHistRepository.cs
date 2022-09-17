using Microsoft.Data.SqlClient;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Base;
using SMS.DB;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SMS.DAL.Repositories
{
    public class StudentActivateHistRepository : Repository<StudentActivateHist>, IStudentActivateHistRepository
    {
        private readonly IStudentRepository _studentRepository;
        public StudentActivateHistRepository(ApplicationDbContext context, IStudentRepository studentRepository) : base(context)
        {
            _studentRepository = studentRepository;
        }

        public async Task<bool> IsStudentActive(int id, string date)
        {
            Student existingStudenet = await _studentRepository.GetByIdAsync(id);
            DateTime qDate = Convert.ToDateTime(date);
            if (existingStudenet.AdmissionDate.Date < qDate.Date)
            {
                var listOfActivationHistory = await GetExistingHistory(id, date);
                StudentActivateHist objStudentActivateHist = (from t in listOfActivationHistory
                                                             where t.ActionDateTime.Date < qDate.Date
                                                             select t).Last();
                return objStudentActivateHist.IsActive;
            }
            else
            {
                return false;
            }

        }

        private async Task<List<StudentActivateHist>> GetExistingHistory(int studentId, string date)
        {
            DateTime qDate = Convert.ToDateTime(date);
            var param = new SqlParameter("@studentId", studentId);
            var param2 = new SqlParameter("@targetDate", qDate.Date);
            //List<SqlParameter> parameters = new List<SqlParameter>();
            //parameters.Add(param);
            //parameters.Add(param2);
            List<StudentActivateHist> studentActivateHists = await _context.StudentActivateHists.FromSqlRaw("GetStudentActivateHistById @studentId, @targetDate", parameters:new[] {param,param2}).ToListAsync();

            return studentActivateHists.OrderBy(m => m.ActionDateTime).ToList();
        }
    }
}
