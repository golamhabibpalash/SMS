using AutoMapper;
using SMS.App.ViewModels.ClaimContext;
using SMS.App.ViewModels.Employees;
using SMS.App.ViewModels.ExamVM;
using SMS.App.ViewModels.ModuleSubModuleVM;
using SMS.App.ViewModels.SetupVM;
using SMS.App.ViewModels.Students;
using SMS.Entities;

namespace SMS.App.Utilities.AutoMapperConfiguration
{
    public class AutoMapperProfile : Profile
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

            CreateMap<AcademicExam, AcademicExamVM>().ReverseMap();

            CreateMap<AcademicExamGroup, AcademicExamGroupIndexVM>().ReverseMap();

            CreateMap<SetupMobileSMS, AttendanceSetupVM>().ReverseMap();

            CreateMap<ProjectSubModule, ProjectSubModuleVM>().ReverseMap();

            CreateMap<ClaimStores, ClaimStoreVM>().ReverseMap();

            CreateMap<AppliedStudent, AppliedStudentVM>().ReverseMap();
            CreateMap<AcademicExam, AcademicExamDetailVM>().ReverseMap();

        }
    }
}
