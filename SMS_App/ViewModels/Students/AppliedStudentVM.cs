using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Entities;
using SMS.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS_App.ViewModels.Students
{
    public class AppliedStudentVM
    {
        public AppliedStudentVM()
        {
            AcademicSessionList = new List<SelectListItem>();
            InterestedAcademicClassList = new List<SelectListItem>();
            PreviousAcademicClassList = new List<SelectListItem>();
            BloodGroupList = new List<SelectListItem>();
            GenderList = new List<SelectListItem>();
            NationalityList = new List<SelectListItem>();
            ReligionList = new List<SelectListItem>();

            PresentDistrictList = new List<SelectListItem>();
            PermanentDistrictList = new List<SelectListItem>();
            PresentUpazilaList = new List<SelectListItem>();
            PermanentUpazilaList = new List<SelectListItem>();

            //FOccupationList = new List<SelectListItem>();
            //MOccupationList = new List<SelectListItem>();
        }
        public int Id { get; set; }
        [Display(Name = "Student Name *")]
        [Required(ErrorMessage = "This field is required.")]
        public string Name { get; set; }

        [Display(Name = "Student Name (Bn) *")]
        [Required(ErrorMessage = "This field is required.")]
        public string NameBangla { get; set; }

        [Display(Name = "Date of Birth"), DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [Display(Name = "Father's Name *")]
        [Required(ErrorMessage = "This field is required.")]
        public string FatherName { get; set; }

        [Display(Name = "Father's Name (Bn)")]
        public string FatherNameBangla { get; set; }

        [Display(Name = "Father's NID")]
        public string FatherNID { get; set; }

        [Display(Name = "Occupation")]
        public string FatherOccupation { get; set; }

        [Display(Name = "Monthly Income")]
        public string FatherMonthlyIncome { get; set; }

        [Display(Name = "Phone No *"), StringLength(11)]
        [Required(ErrorMessage = "This field is required.")]
        public string FatherPhoneNo { get; set; }

        [Display(Name = "Mother's Name *")]
        [Required(ErrorMessage = "This field is required.")]
        public string MotherName { get; set; }

        [Display(Name = "Mother's Name (Bn)")]
        public string MotherNameBangla { get; set; }

        [Display(Name = "Mother's NID")]
        public string MotherNID { get; set; }

        [Display(Name = "Occupation")]
        public string MotherOccupation { get; set; }

        [Display(Name = "Monthly Income")]
        public string MotherMonthlyIncome { get; set; }

        [Display(Name = "Phone No *"), StringLength(11)]
        [Required(ErrorMessage = "This field is required.")]
        public string MotherPhoneNo { get; set; }

        public string Email { get; set; }

        public string Photo { get; set; }

        [Display(Name = "Birth Certificate No"), StringLength(17, MinimumLength = 17)]
        public string BirthCertificateNo { get; set; }

        [Display(Name = "Religion")]
        public int ReligionId { get; set; }

        [Display(Name = "Gender")]
        public int GenderId { get; set; }

        [Display(Name = "Blood Group")]
        public int? BloodGroupId { get; set; }

        [Display(Name = "Nationality")]
        public int NationalityId { get; set; }

        [Display(Name = "Vill/Area")]
        public string PresentAddressArea { get; set; }

        [Display(Name = "Address Info")]
        public string AddressInfo { get; set; } = String.Empty;

        [Display(Name = "Post Office")]
        public string PresentAddressPO { get; set; }

        [Display(Name = "Upazila")]
        public int? PresentUpazilaId { get; set; }

        [Display(Name = "District")]
        public int? PresentDistrictId { get; set; }

        [Display(Name = "Division")]
        public int? PresentDivisionId { get; set; }

        [Display(Name = "Vill/Area")]
        public string PermanentAddressArea { get; set; }

        [Display(Name = "Post Office")]
        public string PermanentAddressPO { get; set; }

        [Display(Name = "Upazila")]
        public int? PermanentUpazilaId { get; set; }

        [Display(Name = "District")]
        public int? PermanentDistrictId { get; set; }

        [Display(Name = "Division")]
        public int? PermanentDivisionId { get; set; }

        [Display(Name = "Session")]
        public int AcademicSessionId { get; set; }

        [Display(Name = "Previous School")]
        public string PreviousSchool { get; set; }
        [Display(Name = "Previous Class")]
        public int PreviousSchoolClassId { get; set; }
        public AcademicClass PreviousClass { get; set; }
        [Required]
        [Display(Name = "Applied For")]
        public int InterestedAppliedClassId { get; set; }
        public AcademicClass InterestedClass { get; set; }
        public string AimInLife { get; set; }
        public bool Status { get; set; } = true;

        public Division PresentDivision { get; set; }

        public District PresentDistrict { get; set; }

        public Upazila PresentUpazila { get; set; }

        public Division PermanentDivision { get; set; }

        public District PermanentDistrict { get; set; }

        public Upazila PermanentUpazila { get; set; }

        public AcademicSession AcademicSession { get; set; }
        public Nationality Nationality { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public AcademicClass AcademicClass { get; set; }
        public Gender Gender { get; set; }
        public Religion Religion { get; set; }
        public AppliedStudentStatus AppliedStudentStatus { get; set; }

        public List<SelectListItem> FOccupationList { get; set; }
        public List<SelectListItem> MOccupationList { get; set; }
        public List<SelectListItem> BloodGroupList { get; set; }
        public List<SelectListItem> PresentUpazilaList { get; set; }
        public List<SelectListItem> PermanentUpazilaList { get; set; }
        public List<SelectListItem> PresentDistrictList { get; set; }
        public List<SelectListItem> PermanentDistrictList { get; set; }
        public List<SelectListItem> NationalityList { get; set; }
        public List<SelectListItem> ReligionList { get; set; }
        public List<SelectListItem> InterestedAcademicClassList { get; set; }
        public List<SelectListItem> PreviousAcademicClassList { get; set; }
        public List<SelectListItem> AcademicSessionList { get; set; }
        public List<SelectListItem> GenderList { get; set; }
    }
}
