using System;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBL;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.DataVisualization.Charting;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class NewDashboard : PageBase
    {
        public const string PageName = "Dashboard";
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            string BSvalue = Convert.ToString(loginUser.LoginOrgId);
            hdnLoginOrgId_BS.Value = Convert.ToString(loginUser.LoginOrgId);
            hdnLoginToken_BS.Value = loginUser.LoginToken;

            cmbDocumentType_BS.Attributes.Add("onChange", "javascript:SetHiddenVal_BS('Static');");
            cmbDepartment_BS.Attributes.Add("onChange", "javascript:SetHiddenVal_BS('Static');");

            if (!IsPostBack)
            {
                /* DMS04-3447	  BS*/
                Session["FullTextClause"] = null;
                /* DMS04-3447	  BE*/
                if (ViewState["dt"] == null)
                {
                    DataTable dt = new DataTable();//Creating a Datatable for maintaining id and values of dynamic controls
                    ViewState["dt"] = dt;//Creating a viewstate for maintaining id and values of dynamic controls
                    dt.Columns.Add("DRPID", typeof(string)); //adding columns
                    dt.Columns.Add("SID", typeof(string));
                }

                GetDocumentType_BS(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);

            }

        }

        public void GetDocumentType_BS(string loginOrgId, string loginToken)
        {
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                string action = "DocumentTypeForUpload";
                Results res = bl.SearchDocumentTypes(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    cmbDocumentType_BS.Items.Clear();
                    cmbDocumentType_BS.Items.Add(new ListItem("<Select>", "0"));
                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        cmbDocumentType_BS.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg_BS.InnerHtml = CoreMessages.GetMessages(hdnAction_BS.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
            //TextBox1.Visible = false;
        }

        protected void cmbDocumentType_SelectedIndexChanged_BS(object sender, EventArgs e)
        {
            //cmbDocumentType_BS.Items.Clear();
            cmbDepartment_BS.Items.Clear();
            cmdIndex_BS.Items.Clear();
            TextBox1.Text = "";

            //bDynamicControlIndexChange = false;
            GetDepartments_BS(hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value);
            //hdnCountControls_BS.Value = Convert.ToString(0);
            //this.NumberOfControls = 0;
            //GetTemplateDetails_BS(hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value);
            //pnlIndexpro_BS.Controls.Clear();
            //generatecontrols_BS();
            //hdnDBCOLMapping_BS.Value = getIndexFieldsAndColumnsList_BS();


        }
        protected void cmbDepartmentType_SelectedIndexChanged_BS(object sender, EventArgs e)
        {
            //cmbDepartment_BS.Items.Clear();
            cmdIndex_BS.Items.Clear();
            TextBox1.Text = "";

            //bDynamicControlIndexChange = false;
            GetIndex_BS(hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value);
            //hdnCountControls_BS.Value = Convert.ToString(0);
            //this.NumberOfControls = 0;
            //GetTemplateDetails_BS(hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value);
            //pnlIndexpro_BS.Controls.Clear();
            //generatecontrols_BS();
            //hdnDBCOLMapping_BS.Value = getIndexFieldsAndColumnsList_BS();


        }

        public void GetDepartments_BS(string loginOrgId, string loginToken)
        {
            DepartmentBL bl = new DepartmentBL();
            SearchFilter filter = new SearchFilter();
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType_BS.SelectedValue) == 0 ? Convert.ToInt32(cmbDocumentType_BS.SelectedValue) : Convert.ToInt32(cmbDocumentType_BS.SelectedValue);
            try
            {
                string action = "DepartmentsForUpload";
                Results res = bl.SearchDepartments(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.Departments != null)
                {
                    cmbDepartment_BS.Items.Clear();
                    cmbDepartment_BS.Items.Add(new ListItem("<Select>", "0"));

                    //DMSENH6-4657  BS
                    //cmbDepartment_OCR.Items.Clear();
                    //cmbDepartment_OCR.Items.Add(new ListItem("<Select>", "0"));
                    // DMSENH6-4657 BE
                    foreach (Department dp in res.Departments)
                    {
                        cmbDepartment_BS.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));
                        //cmbDepartment_OCR.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString())); //DMSENH6-4657
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg_BS.InnerHtml = CoreMessages.GetMessages(hdnAction_BS.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
            //TextBox1.Visible = false;
        }

        protected void cmdIndex_BS_SelectedIndexChanged_BS(object sender, EventArgs e)
        {
            //cmdIndex_BS.Items.Clear();
            TextBox1.Text = "";

        }
        public void GetIndex_BS(string loginOrgId, string loginToken)
        {
            string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
            SqlConnection con = new SqlConnection(connectionstring);

            UserBase loginUser = (UserBase)Session["LoggedUser"];
            string BSvalue = Convert.ToString(loginUser.LoginOrgId);

            var doctype = cmbDocumentType_BS.SelectedValue;
            var depttype = cmbDepartment_BS.SelectedValue;

            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            SqlCommand cmd = new SqlCommand("[dbo].[USP_GetTemplateDetails]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            cmd.Parameters.Add(new SqlParameter("@in_iCurrUserId", BSvalue));
            cmd.Parameters.Add(new SqlParameter("@in_iDocuTypeID", doctype));
            cmd.Parameters.Add(new SqlParameter("@in_iDepartmentID", depttype));
            cmd.Parameters.Add(new SqlParameter("@in_vAction", "GetTemplateDetailsForUpload"));
            cmd.Parameters.Add(new SqlParameter("@in_vLoginToken", hdnLoginToken_BS.Value));
            cmd.Parameters.Add(new SqlParameter("@in_iLoginOrgId", hdnLoginOrgId_BS.Value));

            sda.Fill(dt);

            cmdIndex_BS.DataSource = dt;

            cmdIndex_BS.DataTextField = "LabelName";
            cmdIndex_BS.DataValueField = "DBFld";
            cmdIndex_BS.DataBind();
            cmdIndex_BS.Items.Insert(0, "Select");

            //TextBox1.Visible = true;
            //var numberofrows = dt.Rows.Count;

            //for (int i = 0; i < numberofrows; i++)
            //{
            //    //for each row, get the 3rd column
            //   var cell = dt.Rows[i][1];
            //   cmdIndex_BS.DataTextField = cell.ToString();
            //}


        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbDocumentType_BS.SelectedValue != "" && cmbDepartment_BS.SelectedValue != "" && cmdIndex_BS.SelectedValue != "" && TextBox1.Text.Trim() != "")
                {
                    err.Text = "";

                    string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
                    SqlConnection con = new SqlConnection(connectionstring);

                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    string BSvalue = Convert.ToString(loginUser.LoginOrgId);

                    var doctype = cmbDocumentType_BS.SelectedValue;
                    var depttype = cmbDepartment_BS.SelectedValue;
                    var indexvalue = cmdIndex_BS.SelectedValue;
                    var columnvalue = TextBox1.Text;

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    //string query = "Select COUNT(UPLOAD_iDepartment) from UPLOAD where UPLOAD_iDepartment=" + depttype + " and UPLOAD_iDocType=" + doctype + " and " + indexvalue + " like '%" + columnvalue + "%' ";
                    string query = "Select * from UPLOAD where UPLOAD_iDepartment=" + depttype + " and UPLOAD_iDocType=" + doctype + " and " + indexvalue + " like '%" + columnvalue + "%' ";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

                    cmd.Parameters.Add(new SqlParameter("@UPLOAD_iDocType", doctype));
                    cmd.Parameters.Add(new SqlParameter("@UPLOAD_iDepartment", depttype));
                    cmd.Parameters.Add(new SqlParameter("@column", indexvalue));
                    cmd.Parameters.Add(new SqlParameter("@columnval", columnvalue));

                    sda.Fill(dt);

                    lblResult.Text = "The nos. of files of Index Field '" + cmdIndex_BS.SelectedItem.Text + "' is : " + (dt.Rows.Count).ToString();

                    //string[] x = new string[dt.Rows.Count];
                    //int[] y = new int[dt.Rows.Count];

                    string[] x = new string[dt.Rows.Count];
                    int[] y = new int[dt.Rows.Count];

                    if (dt.Rows.Count > 0)
                    {
                        //for (int i = 0; i < dt.Rows.Count; i++)
                        for (int i = 0; i < 1; i++)
                        {
                            //x[i] = dt.Rows[i][0].ToString();
                            //y[i] = Convert.ToInt32(dt.Rows[i][1]);
                            x[i] = cmdIndex_BS.SelectedItem.Text;
                            y[i] = Convert.ToInt32(dt.Rows.Count);
                        }
                        Chart1.Series[0].Points.DataBindXY(x, y);
                        Chart1.Series[0].ChartType = SeriesChartType.Column;
                        //Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                        Chart1.Legends[0].Enabled = true;
                    }
                }

                else
                {
                    err.Text = "* Please fill all the field";
                }
            }
            catch
            {

            }
        }

    }
}