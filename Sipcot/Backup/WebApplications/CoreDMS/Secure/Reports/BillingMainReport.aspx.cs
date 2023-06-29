using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;


namespace Lotex.EnterpriseSolutions.WebUI.Secure.Reports
{
    public partial class BillingMainReport : PageBase
    {
        DataSet temdet = new DataSet();

        protected void Page_Load(object sender, EventArgs e)
        {
            txtCreatedDateFrom.Attributes.Add("readonly", "readonly");
            txtCreatedDateTo.Attributes.Add("readonly", "readonly");
            btnGenerateMain.Attributes.Add("onclick", "javascript:return validate()");
            btnGenerateSub.Attributes.Add("onclick", "javascript:return validate()");

            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";
            if (!IsPostBack)
            {
                LoadOrgNames();
            }
        }
        public void LoadOrgNames()
        {
            txtCreatedDateTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtCreatedDateFrom.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("d2") + "-" + "01";
            drpOrg.Items.Clear();
            SecurityBL bl = new SecurityBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                Results res = bl.SearchOrgs(filter, "DropdownOrgs", hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value);
                if (res.ActionStatus == "SUCCESS" && res.Orgs != null)
                {

                    drpOrg.Items.Add(new ListItem("<Select>", "-1"));
                    foreach (Org org in res.Orgs)
                    {
                        drpOrg.Items.Add(new ListItem(org.OrgName, org.OrgId.ToString()));
                    }

                }


            }
            catch 
            {

            }
        }

        protected void btnGenerateSub_Click(object sender, EventArgs e)
        {
            if (pnlMain.Visible == true)
            {
                pnlMain.Visible = false;
            }

          
           // SqlConnection Connect = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
         
            //SqlDataAdapter Gettemdet = new SqlDataAdapter();
            //SqlCommand cmdtempdet = new SqlCommand("USP_SubMainReport", Connect);
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
            BE.OrgId = Convert.ToInt32(drpOrg.SelectedValue);
            BE.CreatedDateFrom=txtCreatedDateFrom.Text.Trim();
            BE.EndDate = ToDateUpdated.Trim();

            ReportBL BL = new ReportBL();
            temdet = BL.BillingSubReport(BE, Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);

            //cmdtempdet.Parameters.AddWithValue("@in_iOrgID", Convert.ToInt32(drpOrg.SelectedValue));
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
                ReportViewerSub.Visible = true;
                ReportViewerSub.LocalReport.ReportEmbeddedResource = "SubMainReport.rdlc";
                ReportDataSource datasource = new ReportDataSource("SubMainReportDataSet", temdet.Tables[0]);
                ReportViewerSub.LocalReport.DataSources.Clear();
                ReportViewerSub.LocalReport.DataSources.Add(datasource);
                ReportViewerSub.LocalReport.Refresh();
                ReportParameter rpt1 = new ReportParameter("ReportParameter1", txtCreatedDateFrom.Text);
                ReportParameter rpt2 = new ReportParameter("ReportParameter2", txtCreatedDateTo.Text);
                ReportParameter rpt3 = new ReportParameter("ReportParameter3", drpOrg.SelectedItem.ToString());



                this.ReportViewerSub.LocalReport.SetParameters(new ReportParameter[] { rpt1, rpt2, rpt3 });
            }
            else
            {
                divMsg.InnerText = "No Data Available For Particular Period.";
                pnlSub.Visible = false;
            }
        }

        protected void btnGenerateMain_Click(object sender, EventArgs e)
        {
            if (pnlSub.Visible == true)
            {
                pnlSub.Visible = false;
            }

            //SqlConnection Connect = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
            //DataSet temdet = new DataSet();
            //SqlDataAdapter Gettemdet = new SqlDataAdapter();
            //SqlCommand cmdtempdet = new SqlCommand("USP_MainReport", Connect);
           // cmdtempdet.CommandType = CommandType.StoredProcedure;
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
            //cmdtempdet.Parameters.AddWithValue("@in_iOrgID", Convert.ToInt32(drpOrg.SelectedValue));
            //cmdtempdet.Parameters.AddWithValue("@in_dStartDate", txtCreatedDateFrom.Text);
            //cmdtempdet.Parameters.AddWithValue("@in_dEndDate", ToDateUpdated);
            //cmdtempdet.Parameters.AddWithValue("@in_vLoginToken", hdnLoginToken.Value);
            //cmdtempdet.Parameters.AddWithValue("@in_iLoginOrgId", Convert.ToInt32(hdnLoginOrgId.Value));
            //Gettemdet.SelectCommand = cmdtempdet;
            //Gettemdet.Fill(temdet);
            ReportBE BE = new ReportBE();
            BE.OrgId = Convert.ToInt32(drpOrg.SelectedValue);
            BE.CreatedDateFrom = txtCreatedDateFrom.Text.Trim();
            BE.EndDate = ToDateUpdated.Trim();

            ReportBL BL = new ReportBL();
            temdet = BL.BillingMainReport(BE, Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);


            if (temdet.Tables.Count > 0 && temdet.Tables[0].Rows.Count > 0)
            {
                divMsg.InnerText = "";
                pnlMain.Visible = true;
                ReportViewerMain.LocalReport.ReportEmbeddedResource = "MainReport.rdlc";
                ReportDataSource datasource = new ReportDataSource("MainReportDataSet", temdet.Tables[0]);
                ReportViewerMain.LocalReport.DataSources.Clear();
                ReportViewerMain.LocalReport.DataSources.Add(datasource);
                ReportViewerMain.LocalReport.Refresh();
                ReportParameter rpt1 = new ReportParameter("ReportParameter1", txtCreatedDateFrom.Text);
                ReportParameter rpt2 = new ReportParameter("ReportParameter2", txtCreatedDateTo.Text);
                ReportParameter rpt3 = new ReportParameter("ReportParameter3", drpOrg.SelectedItem.ToString());



                this.ReportViewerMain.LocalReport.SetParameters(new ReportParameter[] { rpt1, rpt2, rpt3 });
            }
            else
            {
                divMsg.InnerText = "No Data Available For Particular Period.";
                pnlMain.Visible = false;
            }
        }
    }
}