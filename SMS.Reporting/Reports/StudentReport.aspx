<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StudentReport.aspx.cs" Inherits="SMS.Reporting.Reports.StudentReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<section class="panel">
                        <header class="panel-heading">
                            <h3>Report Result</h3>
                        </header>
                        <div id="ReportView" style="height: 400px; width: 980px">

                            <rsweb:ReportViewer ID="ReportViewer1" runat="server" SizeToReportContent="true" Width="100%" Height="100%">
                            </rsweb:ReportViewer>
                            </>

                        </div>
                    </section>

</asp:Content>


