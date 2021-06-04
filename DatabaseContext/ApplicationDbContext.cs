using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        

        public DbSet<Models.Student> Student { get; set; }
        public DbSet<Models.AcademicSession> AcademicSession { get; set; }
        public DbSet<Models.BloodGroup> BloodGroup { get; set; }
        public DbSet<Models.District> District { get; set; }
        public DbSet<Models.Division> Division { get; set; }
        public DbSet<Models.Gender> Gender { get; set; }
        public DbSet<Models.Nationality> Nationality { get; set; }
        public DbSet<Models.Religion> Religion { get; set; }
        public DbSet<Models.Upazila> Upazila { get; set; }
        public DbSet<Models.Institute> Institute { get; set; }
        public DbSet<Models.AcademicClass> AcademicClass { get; set; }
        public DbSet<Models.AcademicSection> AcademicSection { get; set; }
        public DbSet<Models.ClassFeeList> ClassFeeList { get; set; }
        public DbSet<Models.StudentFeeHead> StudentFeeHead { get; set; }
        public DbSet<Models.StudentPayment> StudentPayment { get; set; }
        public DbSet<Models.StudentPaymentDetails> StudentPaymentDetails { get; set; }
        public DbSet<Models.AcademicSubject> AcademicSubject { get; set; }
        public DbSet<Models.AcademicSubjectType> AcademicSubjectType { get; set; }
        public DbSet<Models.AttachDoc> AttachDocs { get; set; }
        public DbSet<Models.DesignationType> DesignationType { get; set; }
        public DbSet<Models.Designation> Designation { get; set; }
        public DbSet<Models.EmpType> EmpType { get; set; }
        public DbSet<Models.Employee> Employee { get; set; }
        
    }
}
