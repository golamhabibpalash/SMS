﻿using Microsoft.Extensions.DependencyInjection;
using Repositories;
using SMS.BLL.Contracts.Reports;
using SMS.BLL.Contracts;
using SMS.BLL.Managers.Reports;
using SMS.BLL.Managers;
using SMS.DAL.Contracts.Reports;
using SMS.DAL.Contracts;
using SMS.DAL.Repositories.Reports;
using SMS.DAL.Repositories;

namespace SMS.App.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection Addservices(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeManager, EmployeeManager>();

            services.AddScoped<IAcademicClassRepository, AcademicClassRepository>();
            services.AddScoped<IAcademicClassManager, AcademicClassManager>();

            services.AddScoped<IAcademicSessionRepository, AcademicSessionRepository>();
            services.AddScoped<IAcademicSessionManager, AcademicSessionManager>();

            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentManager, StudentManager>();

            services.AddScoped<IInstituteRepository, InstituteRepository>();
            services.AddScoped<IInstituteManager, InstituteManager>();

            services.AddScoped<IAcademicSectionRepositoy, AcademicSectionRepository>();
            services.AddScoped<IAcademicSectionManager, AcademicSectionManager>();

            services.AddScoped<IAcademicSubjectRepository, AcademicSubjectRepository>();
            services.AddScoped<IAcademicSubjectManager, AcademicSubjectManager>();

            services.AddScoped<IAcademicSubjectTypeRepository, AcademicSubjectTypeRepository>();
            services.AddScoped<IAcademicSubjectTypeManager, AcademicSubjectTypeManager>();

            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<IDesignationManager, DesignationManager>();

            services.AddScoped<IDesignationTypeRepository, DesignationTypeRepository>();
            services.AddScoped<IDesignationTypeManager, DesignationTypeManager>();

            services.AddScoped<IEmpTypeRepository, EmpTypeRepository>();
            services.AddScoped<IEmpTypeManager, EmpTypeManager>();

            services.AddScoped<IGenderRepository, GenderRepository>();
            services.AddScoped<IGenderManager, GenderManager>();

            services.AddScoped<INationalityRepository, NationalityRepository>();
            services.AddScoped<INationalityManager, NationalityManager>();

            services.AddScoped<IStudentPaymentRepository, StudentPaymentRepository>();
            services.AddScoped<IStudentPaymentManager, StudentPaymentManager>();

            services.AddScoped<IStudentPaymentDetailsRepository, StudentPaymentDetailsRepository>();
            services.AddScoped<IStudentPaymentDetailsManager, StudentPaymentDetailsManager>();

            services.AddScoped<IStudentFeeHeadRepository, StudentFeeHeadRepository>();
            services.AddScoped<IStudentFeeHeadManager, StudentFeeHeadManager>();

            services.AddScoped<IReligionRepository, ReligionRepository>();
            services.AddScoped<IReligionManager, ReligionManager>();

            services.AddScoped<IDivisionRepository, DivisionRepository>();
            services.AddScoped<IDivisionManager, DivisionManager>();

            services.AddScoped<IDistrictRepository, DistrictRepository>();
            services.AddScoped<IDistrictManager, DistrictManager>();

            services.AddScoped<IUpazilaRepository, UpazilaRepository>();
            services.AddScoped<IUpazilaManager, UpazilaManager>();

            services.AddScoped<IBloodGroupRepository, BloodGroupRepository>();
            services.AddScoped<IBloodGroupManager, BloodGroupManager>();

            services.AddScoped<IClassFeeListRepository, ClassFeeListRepository>();
            services.AddScoped<IClassFeeListManager, ClassFeeListManager>();

            services.AddScoped<IPhoneSMSRepository, PhoneSMSRepository>();
            services.AddScoped<IPhoneSMSManager, PhoneSMSManager>();

            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IAttendanceManager, AttendanceManager>();

            services.AddScoped<IAttendanceMachineRepository, AttendanceMachineRepository>();
            services.AddScoped<IAttendanceMachineManager, AttendanceMachineManager>();

            services.AddScoped<IChapterRepository, ChapterRepository>();
            services.AddScoped<IChapterManager, ChapterManager>();

            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IQuestionManager, QuestionManager>();

            services.AddScoped<IQuestionFormationRepository, QuestionFormationRepository>();
            services.AddScoped<IQuestionFormationManager, QuestionFormationManager>();

            services.AddScoped<IStudentActivateHistRepository, StudentActivateHistRepository>();
            services.AddScoped<IStudentActivateHistManager, StudentActivateHistManager>();

            services.AddScoped<IEmployeeActivateHistRepository, EmployeeActivateHistRepository>();
            services.AddScoped<IEmployeeActivateHistManager, EmployeeActivateHistManager>();

            services.AddScoped<ISetupMobileSMSRepository, SetupMobileSMSRepository>();
            services.AddScoped<ISetupMobileSMSManager, SetupMobileSMSManager>();

            services.AddScoped<IOffDayRepository, OffDayRepository>();
            services.AddScoped<IOffDayManager, OffDayManager>();

            services.AddScoped<IOffDayTypeRepository, OffDayTypeRepository>();
            services.AddScoped<IOffDayTypeManager, OffDayTypeManager>();

            services.AddScoped<IAcademicExamTypeRepository, AcademicExamTypeRepository>();
            services.AddScoped<IAcademicExamTypeManager, AcademicExamTypeManager>();

            services.AddScoped<IAcademicExamGroupRepository, AcademicExamGroupRepository>();
            services.AddScoped<IAcademicExamGroupManager, AcademicExamGroupManager>();

            services.AddScoped<IAcademicExamRepository, AcademicExamRepository>();
            services.AddScoped<IAcademicExamManager, AcademicExamManager>();

            services.AddScoped<IAcademicExamDetailsRepository, AcademicExamDetailsRepository>();
            services.AddScoped<IAcademicExamDetailsManager, AcademicExamDetailsManager>();

            services.AddScoped<IExamResultManager, ExamResultManager>();
            services.AddScoped<IExamResultRepository, ExamResultRepository>();

            services.AddScoped<IGradingTableManager, GradingTableManager>();
            services.AddScoped<IGradingTableRepository, GradingTableRepository>();

            services.AddScoped<IStudentFeeAllocationManager, StudentFeeAllocationManager>();
            services.AddScoped<IStudentFeeAllocationRepository, StudentFeeAllocationRepository>();

            services.AddScoped<IAcademicClassSubjectManager, AcademicClassSubjectManager>();
            services.AddScoped<IAcademicClassSubjectRepository, AcademicClassSubjectRepository>();

            services.AddScoped<ISubjectEnrollmentManager, SubjectEnrollmentManager>();
            services.AddScoped<ISubjectEnrollmentRepository, SubjectEnrollmentRepository>();

            services.AddScoped<ISubjectEnrollmentDetailManager, SubjectEnrollmentDetailManager>();
            services.AddScoped<ISubjectEnrollmentDetailRepository, SubjectEnrollmentDetailRepository>();

            services.AddScoped<IClaimStoreManager, ClaimStoreManager>();
            services.AddScoped<IClaimStoreRepository, ClaimStoreRepository>();

            services.AddScoped<IProjectModuleManager, ProjectModuleManager>();
            services.AddScoped<IProjectModuleRepository, ProjectModuleRepository>();

            services.AddScoped<IProjectSubModuleManager, ProjectSubModuleManager>();
            services.AddScoped<IProjectSubModuleRepository, ProjectSubModuleRepository>();

            services.AddScoped<IApplicationSettingsManager, ApplicationSettingsManager>();
            services.AddScoped<IApplicationSettingsRepository, ApplicationSettingsRepository>();

            services.AddScoped<IParamBusConfigManager, ParamBusConfigManager>();
            services.AddScoped<IParamBusConfigRepository, ParamBusConfigRepository>();

            //Reporing part start here===================================
            services.AddScoped<IReportManager, ReportManager>();
            services.AddScoped<IReportRepository, ReportRepository>();
            return services;
        }
    }
}
