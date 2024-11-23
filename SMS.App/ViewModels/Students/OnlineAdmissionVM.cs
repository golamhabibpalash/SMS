using System.Collections.Generic;

namespace SMS.App.ViewModels.Students
{
    public class OnlineAdmissionVM
    {
        public string InstituteName { get; set; }
        public string SearchText { get; set; }
        public List<AppliedStudentVM> SearchApplications { get; set; } = null;
    }
}
