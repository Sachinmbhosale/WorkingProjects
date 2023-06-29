using System;
using System.Web;
using System.Web.UI.WebControls;
using WorkflowBLL.Classes;
using WorkflowBAL;
using Lotex.EnterpriseSolutions.CoreBE;
using System.IO;
using System.Data.OleDb;
using System.Data;
using AjaxControlToolkit;
namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageWorkflowBulkUpload : PageBase
    {
        //global variable
        WorkflowDmsFieldsMapping objUpload = new WorkflowDmsFieldsMapping();
        Workflows objWorkflow = new Workflows();
        OleDbConnection oledbConn;
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Text = string.Empty;
            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;
                hdnUserId.Value = loginUser.UserId.ToString();
                BindAllDropdown();
                BindOrgs();
                

            }
        }

        protected void BindAllDropdown()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objUpload.LoginToken = loginUser.LoginToken;
            objUpload.LoginOrgId = loginUser.LoginOrgId;

            objResult = objUpload.ManageUpload(objUpload, "BindAllDropdown");
            ddlOrg.DataSource = objResult.dsResult;
            ddlOrg.DataTextField = "Value";
            ddlOrg.DataValueField = "Id";
            ddlOrg.DataBind();

            ddlProcess.DataSource = objResult.dsResult;
            ddlProcess.DataTextField = "Value";
            ddlProcess.DataValueField = "Id";
            ddlProcess.DataBind();

            ddlWorkflow.DataSource = objResult.dsResult;
            ddlWorkflow.DataTextField = "Value";
            ddlWorkflow.DataValueField = "Id";
            ddlWorkflow.DataBind();

            ddlStage.DataSource = objResult.dsResult;
            ddlStage.DataTextField = "Value";
            ddlStage.DataValueField = "Id";
            ddlStage.DataBind();



        }

        protected void BindOrgs()
        {
            try
            {

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUpload.LoginToken = loginUser.LoginToken;
                objUpload.LoginOrgId = loginUser.LoginOrgId;

                objResult = objUpload.ManageUpload(objUpload, "BindOrgsDropDown");
                ddlOrg.DataSource = objResult.dsResult;
                ddlOrg.DataTextField = "ORGS_vName";
                ddlOrg.DataValueField = "ORGS_iId";
                ddlOrg.DataBind();

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
            }
        }
        //DMSENH6-4796  BS
        protected void BindProcessDropDown()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUpload.LoginToken = loginUser.LoginToken;
                objUpload.LoginOrgId = loginUser.LoginOrgId;
                objUpload.WorkFlowOrgId = Convert.ToInt32(ddlOrg.SelectedValue);
                objResult = objUpload.ManageUpload(objUpload, "BindProcessDropDown");
                ddlProcess.DataSource = objResult.dsResult;
                ddlProcess.DataTextField = "WorkflowProcess_vName";
                ddlProcess.DataValueField = "WorkflowProcess_iId";
                ddlProcess.DataBind();


            }

            catch (Exception ex)
            {

            }
            //Make workflow and stage dropdown as defalt value
            BindListitem(ddlWorkflow);
            BindListitem(ddlStage);


        }

        protected void ddlProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindWorkflowDropDown();
            if (ddlProcess.SelectedIndex.Equals(0))
            {
                ClearGridview();
                BindListitem(ddlWorkflow);
                BindListitem(ddlStage);


            }
        }

        protected void BindWorkflowDropDown()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUpload.LoginToken = loginUser.LoginToken;
                objUpload.LoginOrgId = loginUser.LoginOrgId;
                objUpload.WorkFlowOrgId = Convert.ToInt32(ddlOrg.SelectedValue);
                objUpload.ProcessId = Convert.ToInt32(ddlProcess.SelectedValue);
                objResult = objUpload.ManageUpload(objUpload, "BindWorkflowDropDown");
                ddlWorkflow.DataSource = objResult.dsResult;
                ddlWorkflow.DataTextField = "Workflow_vName";
                ddlWorkflow.DataValueField = "Workflow_iId";
                ddlWorkflow.DataBind();
            }

            catch (Exception ex)
            {

            }

        }
        protected void ddlWorkflow_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearGridview();
            BindStageDropDown();

        }

        protected void BindStageDropDown()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUpload.LoginToken = loginUser.LoginToken;
                objUpload.LoginOrgId = loginUser.LoginOrgId;
                objUpload.ProcessId = Convert.ToInt32(ddlProcess.SelectedValue);
                objUpload.WorkflowId = Convert.ToInt32(ddlWorkflow.SelectedValue);
                objUpload.WorkFlowOrgId = Convert.ToInt32(ddlOrg.SelectedValue);
                objResult = objUpload.ManageUpload(objUpload, "BindStageDropDown");
                ddlStage.DataSource = objResult.dsResult;
                ddlStage.DataTextField = "WorkflowStage_vDisplayName";
                ddlStage.DataValueField = "WorkflowStage_iId";
                ddlStage.DataBind();


            }

            catch (Exception ex)
            {

            }

        }

        protected void BindListitem(DropDownList selectedDropDownId)
        {
            ListItem li = new ListItem("---Select---", "0");
            selectedDropDownId.Items.Clear();
            selectedDropDownId.Items.Add(li);
        }



        protected void btnRedirect_Click(object sender, EventArgs e)
        {
          

                string ProcessId = HttpUtility.UrlEncode(Encrypt(ddlProcess.SelectedValue));
                string workflowId = HttpUtility.UrlEncode(Encrypt(ddlWorkflow.SelectedValue));
                string stageId = HttpUtility.UrlEncode(Encrypt(ddlStage.SelectedValue));

                string Querystring = "?ProcessId=" + ProcessId + "&workflowId=" + workflowId + "&stageId=" + stageId;
                Response.Redirect("~/Workflow/WorkflowAdmin/ManageWorkflowDmsFieldsMapping.aspx" + Querystring);
            
        }




        protected DataTable GetFieldNamesmapping()
        {
            DataTable dt = new DataTable();
            try
            {

                DBResult objResult = new DBResult();
                objUpload.LoginToken = hdnLoginToken.Value;
                objUpload.LoginOrgId = Convert.ToInt32(hdnLoginOrgId.Value);
                objUpload.ProcessId = Convert.ToInt32(ddlProcess.SelectedValue);
                objUpload.WorkflowId = Convert.ToInt32(ddlWorkflow.SelectedValue);
                objUpload.StageID = Convert.ToInt32(ddlStage.SelectedValue);

                objResult = objUpload.ManageUpload(objUpload, "GetFieldNamesmapping");
                if (objResult.dsResult.Tables.Count > 0)
                {
                    dt = objResult.dsResult.Tables[0];
                }
            }
            catch (Exception ex)
            { }
            return dt;
        }



        protected void AsyncFileUpload1_OnUploadedComplete(object sender, AsyncFileUploadEventArgs e)
        {
            if (AsyncFileUpload1.HasFile)
            {

                string key = Guid.NewGuid().ToString();
                string FolderPath = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"] + key + "\\";
                string FileName = AsyncFileUpload1.FileName;
                string Extension = Path.GetExtension(AsyncFileUpload1.FileName);

                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }

                string FilePath = FolderPath + FileName;
                AsyncFileUpload1.SaveAs(FilePath);
                Session["WrokFlowFilePath"] = FilePath;
                //  ReadExcel(FilePath);
            }
        }

        protected string getIndexFieldsList(DataTable dt)
        {

            string IndexFieldsList = string.Empty;

            try
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    if (dt.Rows[i]["FiledName"].ToString().Trim().Length > 0)
                        IndexFieldsList = IndexFieldsList + "[" + dt.Rows[i]["FiledName"].ToString().Trim() + "],";

                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());

            }
            return IndexFieldsList.Substring(0, IndexFieldsList.Length - 1);
        }

        //protected string SQLInsert(DataTable dt,string XML)
        //{

        //    string Insert = string.Empty;
        //    string select = string.Empty;

        //    try
        //    {

        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {

        //            if (dt.Rows[i]["FiledName"].ToString().Trim().Length > 0)
        //                Insert = Insert + dt.Rows[i]["FieldId"].ToString().Trim() + ",";

        //              select=   select + "T.C.value('(" + dt.Rows[i]["FiledName"].ToString().Trim() +")[1]', 'varchar(1000)')";
                
        //        }
        //        select = select +  ddlProcess.SelectedValue + "," + ddlWorkflow.SelectedValue + "," + ddlStage.SelectedValue + "," + hdnUserId.Value + "," + hdnLoginOrgId.Value;
        //        Insert = Insert + " WorkflowStageFieldData_iProcessID,WorkflowStageFieldData_iWorkFlowID,WorkflowStageFieldData_iStageID,WorkflowStageFieldData_iUploadedBy,WorkflowStageFieldData_iOrgID";
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());

        //    }
         
        //    select = "select " + select + " from + @in_xXmlData.nodes('NewDataSet/Table1') AS T(C)";

       
        //  Insert = "insert into WorkflowStageFieldDataStaging  (" + Insert + ") ";
        //  Insert = Insert + select;
        //    return Insert;
        //}

        private void ReadExcel(string filePath)
        {
            DataTable sdt = GetFieldNamesmapping();
            OleDbCommand cmd = new OleDbCommand(); ;
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            SearchFilter filter = new SearchFilter();
            string action = "ExportDocumentType";
            string duplicateValues = string.Empty;
        
            try
            {
                Logger.Trace("Started Extracting Soft Data", Session["LoggedUserId"].ToString());
                // need to pass relative path after deploying on server
                oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                filePath + ";Extended Properties='Excel 12.0;';");
                try
                {
                    oledbConn.Open();
                }
                catch
                {
                    string con = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + "Extended Properties=Excel 8.0;HDR=Yes;IMEX=1";
                    oledbConn = new OleDbConnection(con);
                    oledbConn.Open();
                }

                // Get the data table containg the schema guid.
                dt = oledbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                {
                    throw new Exception(" No sheets available!");
                }

                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }

                cmd.Connection = oledbConn;
                cmd.CommandType = CommandType.Text;
                // Get column names of selected document type
                string SelectCommand = getIndexFieldsList(sdt);
                SelectCommand = "SELECT " + SelectCommand + " FROM [" + excelSheets[0] + "]";
                cmd.CommandText = SelectCommand;
                oleda = new OleDbDataAdapter(cmd);
                try
                {
                    oleda.Fill(ds);
                }
                catch
                {
                    lblMessage.Text = "Selected file is not matching with stage ";
                    throw new Exception("Selected file is not matching with stage " + ddlStage.SelectedItem.ToString() + ".");//Bug Wrtier DMS ENHSMT 1-1012 M
                   
                }

                //string dataxml = ds.GetXml();
                string strXml = string.Empty;
             //    strXml = SQLInsert(sdt, dataxml);

              
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    strXml = InsertIntoTempTable(ds, sdt);
                    if (strXml.Length > 0)
                    {

                        DBResult objResult = new DBResult();
                        objUpload.LoginToken = hdnLoginToken.Value;
                        objUpload.LoginOrgId = Convert.ToInt32(hdnLoginOrgId.Value);
                        objUpload.ProcessId = Convert.ToInt32(ddlProcess.SelectedValue);
                        objUpload.WorkflowId = Convert.ToInt32(ddlWorkflow.SelectedValue);
                        objUpload.StageID = Convert.ToInt32(ddlStage.SelectedValue);
                        
                        objUpload.XmlData = strXml;
                        objResult = objUpload.ManageUpload(objUpload, "UploadToStagingTable");
                        DataTable dataTable = new DataTable();
                        //bind excel data to gridview
                        if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                        {
                            dataTable = objResult.dsResult.Tables[0];
                            foreach (DataRow row in dataTable.Rows)
                            {
                                duplicateValues = row["UploadStatus"].ToString();

                                if (!duplicateValues.Contains("Not Duplicate"))
                                {
                                   
                                    btncommit.Enabled = false;
                                   break;
                                }
                               

                            }
                        }
                        else
                        {
                            if (objResult.dsResult.Tables.Count > 1 && objResult.dsResult.Tables[1].Rows.Count > 0)
                            {
                                dataTable = objResult.dsResult.Tables[1];
                            }
                            lblMessage.Text = Convert.ToString(objResult.Message);
                        }

                        GridView1.DataSource = dataTable;
                        GridView1.DataBind();

                      

                    }
                }

                else
                {
                    throw new Exception(" No data available in uploaded file!");//Bug Wrtier DMS ENHSMT 1-1012 M
                }

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                throw new Exception(ex.Message.ToString());


            }
            finally
            {
                // Clean up.
                if (oledbConn != null)
                {
                    oledbConn.Close();
                    oledbConn.Dispose();
                }
                if (dt != null)
                {
                    dt.Dispose();
                }
                if (ds != null)
                {
                    ds.Dispose();
                }
            }
        }
        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Getting Workflow Controls", Session["LoggedUserId"].ToString());
                ClearGridview();
                btncommit.Enabled = true;
                ReadExcel(Session["WrokFlowFilePath"].ToString());

            }
            catch (Exception ex)
            {


            }


        }

        protected void ClearGridview()
        {
            GridView1.DataSource = null;
            GridView1.DataBind();
        }

        private string InsertIntoTempTable(DataSet dsSource, DataTable dsDestination)
        {


            string strFields;
            string strValues;
            string strQuery = string.Empty;
            string strXML = string.Empty;
            strFields = "";
            strValues = "";
            DataSet dsupload = new DataSet();

            try
            {
                // Constructing query to insert data 

                strQuery = " ";


                for (int recCount = 0; recCount <= dsDestination.Rows.Count - 1; recCount++)
                {
                    strFields = strFields + dsDestination.Rows[recCount]["FieldId"].ToString() + ",";
                }
                strFields += " WorkflowStageFieldData_iProcessID,WorkflowStageFieldData_iWorkFlowID,WorkflowStageFieldData_iStageID,WorkflowStageFieldData_iUploadedBy,WorkflowStageFieldData_iOrgID";

                // Loop through excel datasorce 
                for (int i = 0; i <= dsSource.Tables[0].Rows.Count - 1; i++)
                {
                    if (i > 0)
                        strValues += " union all ";

                    strValues += " select ";

                    // Loop throgh FMT data source
                    for (int recCount = 0; recCount <= dsDestination.Rows.Count - 1; recCount++)
                    {
                        strValues = strValues + "&apos;" + dsSource.Tables[0].Rows[i][dsDestination.Rows[recCount]["FiledName"].ToString()].ToString() + "&apos;" + ",";
                    }


                    strValues += ddlProcess.SelectedValue + "," + ddlWorkflow.SelectedValue + "," + ddlStage.SelectedValue + "," + hdnUserId.Value + "," + hdnLoginOrgId.Value;
                }
                strQuery += "insert into WorkflowStageFieldDataStaging  (" + strFields + " ) " + strValues; /* DMS5-4339 M */


                strXML = "<table><row><query>" + strQuery + "</query></row></table>";

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }

            return strXML;
        }




        protected void dddlorg_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearGridview();
            BindProcessDropDown();
        }

        protected void GridView1_OnPageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                CheckAuthentication();
                GridView1.PageIndex = e.NewPageIndex;
                //BindProcessGridview(GetProcess());
            }
            catch (Exception ex)
            {
                //lblMessageGrid.Text = ex.Message.ToString();
            }
        }

        protected void btncommit_Click(object sender, EventArgs e)
        {
            try
            {


                DBResult objResult = new DBResult();
                objUpload.LoginToken = hdnLoginToken.Value;
                objUpload.LoginOrgId = Convert.ToInt32(hdnLoginOrgId.Value);
                objUpload.ProcessId = Convert.ToInt32(ddlProcess.SelectedValue);
                objUpload.WorkflowId = Convert.ToInt32(ddlWorkflow.SelectedValue);
                objUpload.StageID = Convert.ToInt32(ddlStage.SelectedValue);
                objResult = objUpload.ManageUpload(objUpload, "ConfirmFromStagingTable");
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {

                    GridView1.DataSource = objResult.dsResult.Tables[0];
                    GridView1.DataBind();
                    lblMessage.Text = "Records commited sucessfully .";
                    btncommit.Visible = false;
                }
                        
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
            }
        }

        protected void ddlStage_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearGridview();
        }

        //protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        string matchingResult = (e.Row.Cells[1].Text);//Matching result Column

        //        foreach (TableCell cell in e.Row.Cells)
        //        {
        //            if (!matchingResult.Equals("Matching"))
        //            {
        //                cell.BackColor = Color.Red;
        //            }

        //        }
        //    }
        //}
        //DMSENH6-4796  BE
    }
}