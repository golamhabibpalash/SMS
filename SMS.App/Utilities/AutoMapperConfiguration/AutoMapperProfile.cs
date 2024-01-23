using AutoMapper;
using SMS.App.ViewModels.ClaimContext;
using SMS.App.ViewModels.ConfigureVM;
using SMS.App.ViewModels.Employees;
using SMS.App.ViewModels.ExamVM;
using SMS.App.ViewModels.ModuleSubModuleVM;
using SMS.App.ViewModels.SetupVM;
using SMS.App.ViewModels.Students;
using SMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.App.Utilities.AutoMapperConfiguration
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<StudentCreateVM, Student>();
            CreateMap<Student, StudentCreateVM>();

            CreateMap<StudentEditVM, Student>();
            CreateMap<Student, StudentEditVM>();

            CreateMap<StudentListVM, Student>();
            CreateMap<Student, StudentListVM>();

            CreateMap<EmployeeCreateVM, Employee>();
            CreateMap<Employee, EmployeeCreateVM>();

            CreateMap<EmployeeEditVM, Employee>();
            CreateMap<Employee, EmployeeEditVM>();

            CreateMap<EmployeeDetailsVM, Employee>();
            CreateMap<Employee, EmployeeDetailsVM>();

            CreateMap<AcademicExam, AcademicExamVM>();
            CreateMap<AcademicExamVM, AcademicExam>();
            
            CreateMap<AcademicExamGroup, AcademicExamGroupIndexVM>();
            CreateMap<AcademicExamGroupIndexVM, AcademicExamGroup>();

            CreateMap<SetupMobileSMS, AttendanceSetupVM>();
            CreateMap<AttendanceSetupVM,SetupMobileSMS>();
            
            CreateMap<ProjectSubModule, ProjectSubModuleVM>();
            CreateMap<ProjectSubModuleVM, ProjectSubModule>();

            CreateMap<ClaimStores, ClaimStoreVM>();
            CreateMap<ClaimStoreVM, ClaimStores>();

        }
    }
}
