using System;
using System.Linq;
using System.Web.UI;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Data;
//using Lotex.EnterpriseSolutions.WebUI.Base;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class DocumentTypes1 : PageBase
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;

                hdnPageId.Value = "0";
                string pageRights = GetPageRights();
                hdnPageRights.Value = pageRights;
                ApplyPageRights(pageRights, this.Form.Controls);

                txtDocumentTypeName.Focus();
            }
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            //Response.Redirect("~/Secure/
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            Logger.Trace("btnExport_Click Started", "TraceStatus");
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();

            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";

            try
            {
                Logger.Trace("btnExport_Click Started", Session["LoggedUserId"].ToString());
                string action = "ExportDocumentType";
                filter.CurrDocumentTypeId = Convert.ToInt32(hdnCurrentDocTypeID.Value);
                filter.CurrTemplateId = Convert.ToInt32(hdnTemplateId.Value);

                string ExportType = hdnExportType.Value.ToString();

                Results res = bl.SearchDocumentTypes(filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);

                if (res.ActionStatus == "SUCCESS")
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    if (res != null && res.IndexFields.Count > 0)
                    {
                        for (int i = 0; i < res.IndexFields.Count; i++)
                        {
                            dt.Columns.Add(res.IndexFields[i].IndexName, typeof(string));
                        }
                    }
                    dt.Rows.Add(dt.NewRow());

                    switch (ExportType)
                    {
                        case "Excel":
                            GridView2.DataSource = dt;
                            GridView2.DataBind();
                            ExportToExcel(dt, hdnDocName.Value.ToString());
                            break;
                        case "CSV":
                            DataTableToCsv(dt, hdnDocName.Value.ToString());
                            break;
                        case "Text":
                            DataTableToText(dt, hdnDocName.Value.ToString());
                            break;
                        default:
                            divMsg.InnerHtml = "Invalid export option.";
                            break;
                    }
                }
                Logger.Trace("btnExport_Click Finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception:"+ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        /// <summary>
        /// Export dt to Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="ExcelName"></param>
        void ExportToExcel(System.Data.DataTable dt, string ExcelName)
        {
            Logger.Trace("ExportToExcel started", Session["LoggedUserId"].ToString());
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + ExcelName + ".xls");
            Response.Charset = "";
            Response.ContentType = "application/ms-excel";
            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            GridView2.RenderControl(htmlWrite);
            //Response.Write("<table border='1' >  ");
            Response.Write(stringWrite.ToString());
            //Response.Write("</table>");
            Response.End();

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //var columnNames = dt.Columns.Cast<System.Data.DataColumn>().Select(column => column.ColumnName.Replace("\"", "")).ToArray();
            //sb.AppendLine(string.Join("\t", columnNames));

            //Response.Clear();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment;filename=" + ExcelName + ".xls");
            //Response.Charset = "";
            //Response.ContentType = "application/ms-excel";
            //System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            //System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            //GridView2.RenderControl(htmlWrite);
            //Response.Write(sb.ToString());
            //Response.End();
        }

        public void DataTableToCsv(System.Data.DataTable dt, string csvFile)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            var columnNames = dt.Columns.Cast<System.Data.DataColumn>().Select(column => "\"" + column.ColumnName.Replace("\"", "\"\"") + "\"").ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            // Below line is commented because there is no row to export
            //foreach (System.Data.DataRow row in dt.Rows)
            //{
            //    var fields = row.ItemArray.Select(field => "\"" + field.ToString().Replace("\"", "\"\"") + "\"").ToArray();
            //    sb.AppendLine(string.Join(",", fields));
            //}
            //System.IO.File.WriteAllText(csvFile, sb.ToString(), System.Text.Encoding.Default);

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + csvFile + ".csv");
            Response.Charset = "";
            Response.ContentType = "application/ms-excel";
            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            GridView2.RenderControl(htmlWrite);
            Response.Write(sb.ToString());
            Response.End();
        }

        public void DataTableToText(System.Data.DataTable dt, string txtFile)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            var columnNames = dt.Columns.Cast<System.Data.DataColumn>().Select(column => column.ColumnName.Replace("\"", "\"\"")).ToArray();
            sb.AppendLine(string.Join(" | ", columnNames));
            //System.IO.File.WriteAllText(txtFile, sb.ToString(), System.Text.Encoding.Default);

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=" + txtFile + ".txt");
            Response.Charset = "";
            Response.ContentType = "application/ms-excel";
            System.IO.StringWriter stringWrite = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            GridView2.RenderControl(htmlWrite);
            Response.Write(sb.ToString());
            Response.End();
        }

        public override void VerifyRenderingInServerForm(Control control)
        {

        }
    }
}
