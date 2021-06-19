﻿
using SMS.Entities.AdditionalModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ClassRoll { get; set; }

        public int AcademicClassId { get; set; }

        public int? AcademicSectionId { get; set; }

        public string FatherName { get; set; }

        public string MotherName { get; set; }

        public DateTime AdmissionDate { get; set; }

        public string Email { get; set; }

        public long PhoneNo { get; set; }

        public long GuardianPhone { get; set; }

        public string Photo { get; set; }

        public DateTime DOB { get; set; }

        public int ReligionId { get; set; }

        public int GenderId { get; set; }

        public int BloodGroupId { get; set; }

        public int NationalityId { get; set; }

        public string PresentAddressArea { get; set; }

        public string PresentAddressPO { get; set; }

        public int PresentUpazilaId { get; set; }

        public int PresentDistrictId { get; set; }

        public int PresentDivisiontId { get; set; }

        public string PermanentAddressArea { get; set; }

        public string PermanentAddressPO { get; set; }

        public int PermanentUpazilaId { get; set; }

        public int PermanentDistrictId { get; set; }

        public int PermanentDivisiontId { get; set; }

        public int AcademicSessionId { get; set; }

        public string PreviousSchool { get; set; }

        public ICollection<AttachDoc> Documents { get; set; }

        public bool Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string EditedBy { get; set; }

        public DateTime EditedAt { get; set; }

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

    }
}
