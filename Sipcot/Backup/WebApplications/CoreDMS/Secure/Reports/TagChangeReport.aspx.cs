﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Reports
{
    public partial class TagChangeReport : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";

            txtCreatedDateFrom.Attributes.Add("readonly", "readonly");
            txtCreatedDateTo.Attributes.Add("readonly", "readonly");
            btnsearchSub.Attributes.Add("onclick", "javascript:return validate()");

            if (!IsPostBack)
            {
                Date();
            }

        }

        protected void btnsearchSub_Click(object sender, EventArgs e)
        {
            if (pnlSub.Visible == true)
            {
                pnlSub.Visible = false;
            }

            //SqlConnection Connect = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);
            DataSet temdet = new DataSet();
            //SqlDataAdapter Gettemdet = new SqlDataAdapter();
            //SqlCommand cmdtempdet = new SqlCommand("USP_TagchangeLogReport", Connect);// have to pass date from there
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
            temdet = BL.TagChangeReport(BE, Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);

            //cmdtempdet.Parameters.AddWithValue("@in_dStartDate", txtCreatedDateFrom.Text);
            //cmdtempdet.Parameters.AddWithValue("@in_dEndDate", ToDateUpdated);
            //cmdtempdet.Parameters.AddWithValue("@in_vLoginToken", hdnLoginToken.Value);
            //cmdtempdet.Parameters.AddWithValue("@in_iLoginOrgId", Convert.ToInt32(hdnLoginOrgId.Value));
            //Gettemdet.SelectCommand = cmdtempdet;
            //Gettemdet.Fill(temdet);

            if (temdet != null && temdet.Tables.Count > 0 && temdet.Tables[0].Rows.Count > 0)
            {
                divMsg.InnerText = "";
                pnlSub.Visible = true;
                ReportViewerMain.Visible = true;
                ReportViewerMain.LocalReport.ReportEmbeddedResource = "TagChangeReport.rdlc";
                ReportDataSource datasource = new ReportDataSource("TagChangeReportDataSet", temdet.Tables[0]);
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
    }
}