using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SchoolManagementSystem.Models;
using SchoolManagementSystem;

namespace SchoolManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        

        public DbSet<SchoolManagementSystem.Models.Student> Student { get; set; }
        public DbSet<SchoolManagementSystem.Models.AcademicSession> AcademicSession { get; set; }
        public DbSet<SchoolManagementSystem.Models.BloodGroup> BloodGroup { get; set; }
        public DbSet<SchoolManagementSystem.Models.District> District { get; set; }
        public DbSet<SchoolManagementSystem.Models.Division> Division { get; set; }
        public DbSet<SchoolManagementSystem.Models.Gender> Gender { get; set; }
        public DbSet<SchoolManagementSystem.Models.Nationality> Nationality { get; set; }
        public DbSet<SchoolManagementSystem.Models.Religion> Religion { get; set; }
        public DbSet<SchoolManagementSystem.Models.Upazila> Upazila { get; set; }
        public DbSet<SchoolManagementSystem.Models.Institute> Institute { get; set; }
        public DbSet<SchoolManagementSystem.Models.AcademicClass> AcademicClass { get; set; }
        public DbSet<SchoolManagementSystem.Models.AcademicSection> AcademicSection { get; set; }
        public DbSet<SchoolManagementSystem.Models.ClassFeeList> ClassFeeList { get; set; }
        public DbSet<SchoolManagementSystem.Models.StudentFeeHead> StudentFeeHead { get; set; }
        public DbSet<SchoolManagementSystem.Models.StudentPayment> StudentPayment { get; set; }
        public DbSet<SchoolManagementSystem.Models.StudentPaymentDetails> StudentPaymentDetails { get; set; }
        public DbSet<SchoolManagementSystem.Models.AcademicSubject> AcademicSubject { get; set; }
        public DbSet<SchoolManagementSystem.Models.AcademicSubjectType> AcademicSubjectType { get; set; }
        public DbSet<SchoolManagementSystem.Models.AttachDoc> AttachDocs { get; set; }
        public DbSet<SchoolManagementSystem.Models.DesignationType> DesignationType { get; set; }
        public DbSet<SchoolManagementSystem.Models.Designation> Designation { get; set; }
        public DbSet<SchoolManagementSystem.Models.EmpType> EmpType { get; set; }
        public DbSet<SchoolManagementSystem.Models.Employee> Employee { get; set; }
    }
}
