/*************************************************************    
//Class Name:  clsUtility
//Description:  This class does a lot
'//-----------------------------------------------------------------------------
// REVISION HISTORY
//-----------------------------------------------------------------------------
//
// Date                   Programmer           Issue           Description
 *23/07/2015                Sharath         DMSENH6-4713       On clicking the Cancel button in the OCR text page, the control should be taken back to the OCR text search page.
	'***************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using MySql.Data.MySqlClient;



namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class ReadOCRText : PageBase
    {
        private int PageSize = 1;
        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (!IsPostBack)
            {
                Session["DepartmentId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["DepartmentId"]));
                Session["ProjectId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProjectId"]));
                ViewState["Documentid"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["DocumentID"]));               
                this.GetOCRtextPageWise(1);
            }

        }

        private void GetOCRtextPageWise(int pageIndex)
        {
            string DatabaseSystem = ConfigurationManager.AppSettings["DatabaseSystem"];

            if (DatabaseSystem.Equals("SqlServer"))
            {
                string constring = ConfigurationManager.ConnectionStrings["OCRConnString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("USP_GetOCRTextByDocumentID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@in_iDocumentId", Convert.ToInt32(ViewState["Documentid"]));
                        cmd.Parameters.AddWithValue("@in_iPageIndex", pageIndex);
                        cmd.Parameters.AddWithValue("@in_iPageSize", PageSize);
                        cmd.Parameters.Add("@out_iRecordCount", SqlDbType.Int, 4);
                        cmd.Parameters["@out_iRecordCount"].Direction = ParameterDirection.Output;
                        con.Open();
                        IDataReader idr = cmd.ExecuteReader();
                        dataListOCRText.DataSource = idr;
                        dataListOCRText.DataBind();
                        idr.Close();
                        con.Close();
                        int recordCount = Convert.ToInt32(cmd.Parameters["@out_iRecordCount"].Value);
                        this.PopulatePager(recordCount, pageIndex);
                    }
                }

            }

            if (DatabaseSystem.Equals("MySql"))
         {string constr = ConfigurationManager.ConnectionStrings["OCRConnString"].ConnectionString;
            using (MySqlConnection con = new MySqlConnection(constr))
            {
                using (MySqlCommand cmd = new MySqlCommand("USP_GetOCRTextByDocumentID"))
                {
                    using (MySqlDataAdapter sda = new MySqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@in_iDocumentId", Convert.ToInt32(ViewState["Documentid"]));
                        cmd.Parameters.AddWithValue("@in_iPageIndex", pageIndex);
                        cmd.Parameters.AddWithValue("@in_iPageSize", PageSize);
                        cmd.Parameters.Add("@out_iRecordCount",MySqlDbType.Int32, 4);
                        cmd.Parameters["@out_iRecordCount"].Direction = ParameterDirection.Output;
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            dataListOCRText.DataSource = dt;
                            dataListOCRText.DataBind();
                        }
                        //cmd.Connection = con;
                        //con.Open();
                        //IDataReader idr = cmd.ExecuteReader();
                        //dataListOCRText.DataSource = idr;
                        //dataListOCRText.DataBind();
                        //idr.Close();
                        con.Close();
                        int recordCount = Convert.ToInt32(cmd.Parameters["@out_iRecordCount"].Value);
                        this.PopulatePager(recordCount, pageIndex);
                    }
                }
            }
         }
        }

      

        private void PopulatePager(int recordCount, int currentPage)
        {
            List<ListItem> pages = new List<ListItem>();
            int startIndex, endIndex;
            int pagerSpan = 5;

            //Calculate the Start and End Index of pages to be displayed.
            double dblPageCount = (double)((decimal)recordCount / Convert.ToDecimal(PageSize));
            int pageCount = (int)Math.Ceiling(dblPageCount);
            startIndex = currentPage > 1 && currentPage + pagerSpan - 1 < pagerSpan ? currentPage : 1;
            endIndex = pageCount > pagerSpan ? pagerSpan : pageCount;
            if (currentPage > pagerSpan % 2)
            {
                if (currentPage == 2)
                {
                    endIndex = 5;
                }
                else
                {
                    endIndex = currentPage + 2;
                }
            }
            else
            {
                endIndex = (pagerSpan - currentPage) + 1;
            }

            if (endIndex - (pagerSpan - 1) > startIndex)
            {
                startIndex = endIndex - (pagerSpan - 1);
            }

            if (endIndex > pageCount)
            {
                endIndex = pageCount;
                startIndex = ((endIndex - pagerSpan) + 1) > 0 ? (endIndex - pagerSpan) + 1 : 1;
            }

            //Add the First Page Button.
            if (currentPage > 1)
            {
                pages.Add(new ListItem("First", "1"));
            }

            //Add the Previous Button.
            if (currentPage > 1)
            {
                pages.Add(new ListItem("<<", (currentPage - 1).ToString()));
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                pages.Add(new ListItem(i.ToString(), i.ToString(), i != currentPage));
            }

            //Add the Next Button.
            if (currentPage < pageCount)
            {
                pages.Add(new ListItem(">>", (currentPage + 1).ToString()));
            }

            //Add the Last Button.
            if (currentPage != pageCount)
            {
                pages.Add(new ListItem("Last", pageCount.ToString()));
            }
            rptPager.DataSource = pages;
            rptPager.DataBind();
        }

        protected void Page_Changed(object sender, EventArgs e)
        {
            int pageIndex = int.Parse((sender as LinkButton).CommandArgument);
            this.GetOCRtextPageWise(pageIndex);
        }
      
        protected void btnCancel_Click(object sender, System.EventArgs e)
        {
            string projectId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(Session["ProjectId"])));
            string departmentId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(Session["DepartmentId"])));
            Response.Redirect("~/Secure/Core/DocumentDownloadSearch.aspx" + "?ReadOcr=ReadOCR" + "&ProjectId=" + projectId + "&departmentId=" + departmentId);//DMSENH6-4713
        }
    }
}