using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMS.Reporting.Reports
{
    public partial class StudentReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void BindReportViewer()
        {
            ReportViewer1.Visible = true;
        }

    }
}