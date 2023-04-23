
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.Entities;
using SMS.Entities.AdditionalModels;
using SMS.Entities.RptModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace SMS.DB
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AcademicClass> AcademicClass { get; set; }
        public DbSet<AcademicSection> AcademicSection { get; set; }
        public DbSet<AcademicSession> AcademicSession { get; set; }
        public DbSet<AcademicSubject> AcademicSubject { get; set; }
        public DbSet<AcademicSubjectType> AcademicSubjectType { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AttachDoc> AttachDocs { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<BloodGroup> BloodGroup { get; set; }
        public DbSet<ClassFeeList> ClassFeeList { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<DesignationType> DesignationType { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<Division> Division { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmpType> EmpType { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<Institute> Institute { get; set; }
        public DbSet<Nationality> Nationality { get; set; }
        public DbSet<Religion> Religion { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<StudentFeeHead> StudentFeeHead { get; set; }
        public DbSet<StudentPayment> StudentPayment { get; set; }
        public DbSet<StudentPaymentDetails> StudentPaymentDetails { get; set; }
        public DbSet<Upazila> Upazila { get; set; }
        public DbSet<PhoneSMS> PhoneSMS { get; set; }
        public DbSet<Tran_MachineRawPunch> Tran_MachineRawPunch { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionDetails> QuestionDetails { get; set; }
        public DbSet<QuestionFormat> QuestionFormats { get; set; }
        public DbSet<StudentActivateHist> StudentActivateHists { get; set; }
        public DbSet<EmployeeActivateHist> EmployeeActivateHists { get; set; }
        public DbSet<SetupMobileSMS> SetupMobileSMSs { get; set; }

        public DbSet<AcademicExamType> AcademicExamTypes { get; set; }
        public DbSet<AcademicExam> AcademicExams { get; set; }
        public DbSet<AcademicExamDetail> AcademicExamDetails { get; set; }
        public DbSet<OffDay> OffDays { get; set; }
        public DbSet<OffDayType> OffDayTypes { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<ExamResultDetail> ExamResultDetails { get; set; }
        //public DbSet<ExamResult> ExamResults { get; set; }
        //public DbSet<ExamResultDetail> ExamResultDetails { get; set; }
        //public DbSet<ExamGrade> ExamGrades { get; set; }

        [NotMapped]
        public DbSet<RptStudentVM> RptStudentVMs { get; set; }

        [NotMapped]
        public DbSet<RptAdmitCardVM> RptAdmitCardVMs { get;set; }

        [NotMapped]
        public DbSet<AttendanceVM> AttendanceVMs { get; set; }

        [NotMapped]
        public DbSet<StudentPaymentSummeryVM> StudentPaymentSummeryVMs { get; set; }

        [NotMapped]
        public DbSet<rptStudentPaymentsVM> RptStudentPaymetnsVMs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AttendanceVM>(entity => entity.HasNoKey());
            builder.Entity<StudentPaymentSummeryVM>(entity => entity.HasNoKey());
            builder.Entity<RptStudentVM>().ToView(nameof(RptStudentVMs)).HasNoKey();
            builder.Entity<rptStudentPaymentsVM>().ToView(nameof(RptStudentPaymetnsVMs)).HasNoKey();

            //builder.Entity<StudentPayment>()
            //    .HasMany(p => p.StudentPaymentDetails)
            //    .WithOne(p => p.StudentPayment)
            //    .HasForeignKey(p => p.StudentPayment.Id)
            //    .OnDelete(DeleteBehavior.Cascade);

        }
        
    }
}
