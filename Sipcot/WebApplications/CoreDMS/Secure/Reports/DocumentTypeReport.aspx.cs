using System;
using System.Web.UI.WebControls;
using System.Data;
using Microsoft.Reporting.WebForms;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;


namespace Lotex.EnterpriseSolutions.WebUI.Secure.Reports
{
    public partial class DocumentTypeReport : PageBase
    {
        DataSet temdet = new DataSet();
        protected void Page_Load(object sender, EventArgs e)
        {
            txtCreatedDateFrom.Attributes.Add("readonly", "readonly");
            txtCreatedDateTo.Attributes.Add("readonly", "readonly");
            btnGenerate.Attributes.Add("onclick", "javascript:return validate()");
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
            drpOrg.Items.Add(new ListItem("<Select>", "0"));
            SecurityBL bl = new SecurityBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                Results res = bl.SearchOrgs(filter, "SearchOrgs", hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value);
                if (res.ActionStatus == "SUCCESS" && res.Orgs != null)
                {
                    foreach (Org org in res.Orgs)
                    {
                        drpOrg.Items.Add(new ListItem(org.OrgName, org.OrgId.ToString()));
                    }

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void LoadDocType()
        {
            drpDocType.Items.Clear();
            drpDocType.Items.Add(new ListItem("<Select>", "-1"));
            DocumentTypeBL b1 = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
            filter.CurrOrgId = Convert.ToInt32(drpOrg.SelectedValue);
            try
            {
                string action = "AllDocumentType";
                Results res = b1.GetDocumnetTypeForAOrg(filter, action, hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value);

                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        drpDocType.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {

            //SqlConnection Connect = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
          
            //SqlDataAdapter Gettemdet = new SqlDataAdapter();
            //SqlCommand cmdtempdet = new SqlCommand("USP_CustTypeDocTypeReport", Connect);
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

            ReportBE BE = new ReportBE();
            BE.OrgId = Convert.ToInt32(drpOrg.SelectedValue);
            BE.DocumentTypeReportID = Convert.ToInt32(drpDocType.SelectedValue);
            BE.DocumentTypeReportActionId = Convert.ToInt32(drpAction.SelectedValue);
            BE.CreatedDateFrom = txtCreatedDateFrom.Text.Trim();
            BE.EndDate = ToDateUpdated.Trim();

            ReportBL BL = new ReportBL();
            temdet = BL.DocumentTypeGenerateReport(BE, Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);

            //cmdtempdet.Parameters.AddWithValue("@in_iOrgID", Convert.ToInt32(drpOrg.SelectedValue));
            //cmdtempdet.Parameters.AddWithValue("@in_iDocTypeId", Convert.ToInt32(drpDocType.SelectedValue));
            //cmdtempdet.Parameters.AddWithValue("@in_iActionID", Convert.ToInt32(drpAction.SelectedValue));
            //cmdtempdet.Parameters.AddWithValue("@in_dStartDate", txtCreatedDateFrom.Text);
            //cmdtempdet.Parameters.AddWithValue("@in_dEndDate", ToDateUpdated);
            //cmdtempdet.Parameters.AddWithValue("@in_vLoginToken", hdnLoginToken.Value);
            //cmdtempdet.Parameters.AddWithValue("@in_iLoginOrgId", Convert.ToInt32(hdnLoginOrgId.Value));
            //Gettemdet.SelectCommand = cmdtempdet;
            //Gettemdet.Fill(temdet);
            if (temdet.Tables.Count > 0  &&  temdet.Tables[0].Rows.Count > 0)
            {
                divMsg.InnerText = "";
                ReportViewer1.Visible = true;
                ReportViewer1.LocalReport.ReportEmbeddedResource = "CustTypeDocTypeReport.rdlc";
                ReportDataSource datasource = new ReportDataSource("CustTypeDocTypeDataSet", temdet.Tables[0]);
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(datasource);
                ReportViewer1.LocalReport.Refresh();

                ReportParameter rpt1 = new ReportParameter("ReportParameter1", txtCreatedDateFrom.Text);
                ReportParameter rpt2 = new ReportParameter("ReportParameter2", txtCreatedDateTo.Text);
                ReportParameter rpt3 = new ReportParameter("ReportParameter3", drpOrg.SelectedItem.ToString());



                this.ReportViewer1.LocalReport.SetParameters(new ReportParameter[] { rpt1, rpt2, rpt3 });

               
            }
            else
            {
                divMsg.InnerText = "No Data Available For Particular Period.";
                ReportViewer1.Visible = false;
            }
        }

        protected void drpOrg_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(drpOrg.SelectedValue) > 0)
            {
                LoadDocType();
            }
            else
            {
                drpDocType.Items.Clear();
                drpDocType.Items.Add(new ListItem("<Select>", "-1"));
            }

        }
    }
}