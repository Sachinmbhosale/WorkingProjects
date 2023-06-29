using System;
using System.Data;
using Microsoft.Reporting.WebForms;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;


namespace Lotex.EnterpriseSolutions.WebUI.Secure.Reports
{
    public partial class LogformReport : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtCreatedDateFrom.Attributes.Add("readonly", "readonly");
            txtCreatedDateTo.Attributes.Add("readonly", "readonly");
            btnsearchSub.Attributes.Add("onclick", "javascript:return validate()");
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";

            if (!IsPostBack)
            {
                Date();
            }
        }



        protected void btnsearchSub_Click1(object sender, EventArgs e)
        {
            if (pnlSub.Visible == true)
            {
                pnlSub.Visible = false;
            }

            //SqlConnection Connect = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
            DataSet temdet = new DataSet();
            //SqlDataAdapter Gettemdet = new SqlDataAdapter();
            //SqlCommand cmdtempdet = new SqlCommand("USP_Logreportfilter", Connect);// have to pass date from there
            //cmdtempdet.CommandType = CommandType.StoredProcedure;
            if (txtCreatedDateTo.Text == "" || txtCreatedDateFrom.Text == "")
            {
                txtCreatedDateTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtCreatedDateFrom.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("d2") + "-" + "01";
            }
            string[] FromDate = txtCreatedDateFrom.Text.Split('-');
            string[] ToDate = txtCreatedDateTo.Text.Split('-');
            string update = Convert.ToString(Convert.ToInt32(ToDate[2]));
            if (update.Length == 1)
            {
                update = "0" + update;
            }
            string ToDateUpdated = Convert.ToString(ToDate[0]) + "-" + Convert.ToString(ToDate[1]) + "-" + update;

            ReportBE BE = new ReportBE();
            BE.CreatedDateFrom = txtCreatedDateFrom.Text.Trim();
            BE.EndDate = ToDateUpdated.Trim();

            ReportBL BL = new ReportBL();
            temdet = BL.LogFormReport(BE, Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);

            //cmdtempdet.Parameters.AddWithValue("@in_dStartDate", txtCreatedDateFrom.Text);
            //cmdtempdet.Parameters.AddWithValue("@in_dEndDate", ToDateUpdated);
            //cmdtempdet.Parameters.AddWithValue("@in_vLoginToken", hdnLoginToken.Value);
            //cmdtempdet.Parameters.AddWithValue("@in_iLoginOrgId", Convert.ToInt32(hdnLoginOrgId.Value));
            //Gettemdet.SelectCommand = cmdtempdet;
            //Gettemdet.Fill(temdet);


            if (temdet.Tables.Count > 0 && temdet.Tables[0].Rows.Count > 0)
            {
                divMsg.InnerText = "";
                pnlSub.Visible = true;
                ReportViewerMain.Visible = true;
                ReportViewerMain.LocalReport.ReportEmbeddedResource = "Logreport.rdlc";
                ReportDataSource datasource = new ReportDataSource("DataSet1", temdet.Tables[0]);
                ReportViewerMain.LocalReport.DataSources.Clear();
                ReportViewerMain.LocalReport.DataSources.Add(datasource);
                ReportViewerMain.LocalReport.Refresh();
                ReportParameter rpt1 = new ReportParameter("ReportParameter1", txtCreatedDateFrom.Text);
                ReportParameter rpt2 = new ReportParameter("ReportParameter2", txtCreatedDateTo.Text);
                this.ReportViewerMain.LocalReport.SetParameters(new ReportParameter[] { rpt1, rpt2 });
            }
            else
            {
                divMsg.InnerText = "No Data Available For Particular Period.";
                pnlSub.Visible = false;
            }
        }

        protected void btnclear_Click(object sender, EventArgs e)
        {
            divMsg.InnerHtml = "";
            ReportViewerMain.Visible = false;
            Date();
        }

        public void Date()
        {
            txtCreatedDateTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtCreatedDateFrom.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("d2") + "-" + "01";
        }

        protected void RdoPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (RdoPeriod.SelectedValue == "Last 1 week")
            {
                //-------- Week----------------------
                // DayOfWeek weekStart1 = DayOfWeek.Monday; // or Sunday, or whenever
                DateTime startingDate1 = DateTime.Today;

                //while (startingDate1.DayOfWeek != weekStart1)
                //    startingDate1 = startingDate1.AddDays(-1);
                DateTime previousWeekStart1 = startingDate1.AddDays(-7);
                txtCreatedDateFrom.Text = Convert.ToString(previousWeekStart1.ToString("yyyy-MM-dd"));
            }
            if (RdoPeriod.SelectedValue == "Last 1 month")
            {
                // DayOfWeek weekStart2 = DayOfWeek.Monday; // or Sunday, or whenever
                DateTime startingDate2 = DateTime.Today;

                //while (startingDate2.DayOfWeek != weekStart2)
                //    startingDate2 = startingDate2.AddDays(-1);
                DateTime previousWeekEnd2 = startingDate2.AddMonths(-1);
                txtCreatedDateFrom.Text = Convert.ToString(previousWeekEnd2.ToString("yyyy-MM-dd"));

            }
            if (RdoPeriod.SelectedValue == "Last 6 months")
            {
                // DayOfWeek weekStart = DayOfWeek.Monday; // or Sunday, or whenever
                DateTime startingDate = DateTime.Today;

                //while (startingDate.DayOfWeek != weekStart)
                //    startingDate = startingDate.AddDays(-1);
                DateTime Lastsixmonths = startingDate.AddMonths(-6);
                txtCreatedDateFrom.Text = Convert.ToString(Lastsixmonths.ToString("yyyy-MM-dd"));
            }
        }
    }

}
