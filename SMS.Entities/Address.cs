using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Entities
{
    public class Address: CommonProps
    {
        [Display(Name="Vill/Area")]
        public string Area { get; set; }

        [Display(Name ="Address Type")]
        public int AddressTypeId { get; set; }
        public AddressType AddressType { get; set; }

        [Display(Name ="Post Office")]
        public string PostOffice { get; set; }

        [Display(Name ="Post Code")]
        public int? PostCode { get; set; }

        [Display(Name ="Upadila")]
        public int UpazilaId { get; set; }
        public Upazila Upazila { get; set; }

        [Display(Name ="District")]
        public int  DistrictId { get; set; }
        public District District { get; set; }

        [Display(Name ="Division")]
        public int DivisionId { get; set; }
        public Division Division { get; set; }
    }
}
