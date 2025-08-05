using BLL.Managers.Base;
using SMS.BLL.Contracts;
using SMS.DAL.Contracts;
using SMS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.BLL.Managers
{
    public class AppliedStudentManager : Manager<AppliedStudent>, IAppliedStudentManager
    {
        private readonly IAppliedStudentRepository appliedStudentRepository;
        public AppliedStudentManager(IAppliedStudentRepository _appliedStudentRepository, IAppliedStudentRepository appliedStudentRepository) : base(_appliedStudentRepository)
        {
            this.appliedStudentRepository = appliedStudentRepository;
        }

        public async Task<List<AppliedStudent>> SearchBySearchText(string searchText)
        {
            List<AppliedStudent> filteredStudents = null;
            var students = await appliedStudentRepository.GetAllAsync();
            if (students != null)
            {
                filteredStudents = students.Where(s => s.Name.ToLower().Contains(searchText.Trim().ToLower()) || s.NameBangla.ToLower().Contains(searchText.Trim().ToLower()) || s.FatherPhoneNo.Contains(searchText.Trim()) || s.MotherPhoneNo.Contains(searchText.Trim())).ToList();
            }
            return filteredStudents;
        }
    }
}
