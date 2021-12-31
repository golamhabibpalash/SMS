
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Student : CommonProps
    {
        [Display(Name ="Student Name")]
        public string Name { get; set; }

        [Display(Name = "Class Roll")]
        public int ClassRoll { get; set; }

        [Display(Name = "Academic Class")]
        public int AcademicClassId { get; set; }

        [Display(Name = "Section")]
        public int? AcademicSectionId { get; set; }

        [Display(Name = "Father's Name")]
        public string FatherName { get; set; }

        [Display(Name = "Mother's Name")]
        public string MotherName { get; set; }

        [Display(Name = "Admission Date")]
        public DateTime AdmissionDate { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }

        [Display(Name = "Guardian Phone")]        
        public string GuardianPhone { get; set; }

        public string Photo { get; set; }

        public DateTime DOB { get; set; }

        [Display(Name ="Birth Certificate No"), StringLength(17, MinimumLength =17)]
        public string BirthCertificateNo { get; set; }

        [Display(Name ="Birth Certificate Image")]
        public string BirthCertificateImage { get; set; }

        [Display(Name = "Religion")]
        public int ReligionId { get; set; }

        [Display(Name = "Gender")]
        public int GenderId { get; set; }

        [Display(Name = "Blood Group")]
        public int BloodGroupId { get; set; }

        [Display(Name = "Nationality")]
        public int NationalityId { get; set; }

        [Display(Name = "Vill/Area")]
        public string PresentAddressArea { get; set; }

        [Display(Name = "Post Office")]
        public string PresentAddressPO { get; set; }

        [Display(Name = "Upazila")]
        public int PresentUpazilaId { get; set; }

        [Display(Name = "District")]
        public int PresentDistrictId { get; set; }

        [Display(Name = "Division")]
        public int PresentDivisiontId { get; set; }

        [Display(Name = "Vill/Area")]
        public string PermanentAddressArea { get; set; }

        [Display(Name = "Post Office")]
        public string PermanentAddressPO { get; set; }

        [Display(Name = "Upazila")]
        public int PermanentUpazilaId { get; set; }

        [Display(Name = "District")]
        public int PermanentDistrictId { get; set; }

        [Display(Name = "Division")]
        public int PermanentDivisiontId { get; set; }

        [Display(Name = "Session")]
        public int AcademicSessionId { get; set; }

        [Display(Name = "Previous School")]
        public string PreviousSchool { get; set; }

        public ICollection<AttachDoc> Documents { get; set; }

        public bool Status { get; set; }

        public Division PresentDivision { get; set; }

        public District PresentDistrict { get; set; }

        public Upazila PresentUpazila { get; set; }

        public Division PermanentDivision { get; set; }

        public District PermanentDistrict { get; set; }

        public Upazila PermanentUpazila { get; set; }

        public AcademicSession AcademicSession { get; set; }
        public Nationality  Nationality{ get; set; }
        public AcademicSection AcademicSection { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public AcademicClass AcademicClass { get; set; }
        public Gender Gender { get; set; }
        public Religion Religion { get; set; }

        public List<Attendance> Attendances { get; set; }

    }
}
