/**============================================================================  
Author     : Gokuldas.palapatta 
Create date: 28/02/2012
===============================================================================  

 Date           Programmer          Issue                                   Description
15/07/2013      gokuldas            Bug Wrtier DMS ENHSMT 1-1012               Batch Upload->Upload status error message correction
10/06/2013      gokuldas            DMS5-4339                                  Soft data upload checking for duplicates
 
 
 
 */



using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Configuration;
using System.Data.OleDb;
using System.Data;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class ManageSoftData : PageBase
    {
        OleDbConnection oledbConn;
        public string Messages = string.Empty; /* DMS5-4339 A */

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["action"] != null && Request.QueryString["action"] == "batchedit")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('No more data available to process.');", true);
                }
                CheckAuthentication();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;
                hdnPageId.Value = "0";

                // cmbDocumentType.Attributes.Add("onChange", "return getBatchUploadData();");
                cmbDepartment.Attributes.Add("onChange", "return getBatchUploadData();");
                cmbFilter.Attributes.Add("onChange", "return getBatchUploadData();");

                GetDocumentType(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);



                GetFilters(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
            }
        }

        public void GetDocumentType(string loginOrgId, string loginToken)
        {
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                string action = "DocumentTypeForUpload";
                Results res = bl.SearchDocumentTypes(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    cmbDocumentType.Items.Clear();
                    cmbDocumentType.Items.Add(new ListItem("<Select>", "0"));
                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        cmbDocumentType.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        private string InsertIntoTempTable(DataSet dsSource, DataSet dsDestination, UserBase loginUser)
        {
            DataSet dsFMT = dsDestination;

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


                for (int recCount = 0; recCount <= dsFMT.Tables[0].Rows.Count - 1; recCount++)
                {
                    strFields = strFields + dsFMT.Tables[0].Rows[recCount]["TemplateFields_vDBFld"].ToString() + ",";
                }
                strFields += " UPLOAD_bIsUploaded,UPLOAD_vUploadMode,UPLOAD_iBatchId,UPLOAD_iOrgID,UPLOAD_iUploadedBy,UPLOAD_dUploadedOn,UPLOAD_iDepartment,UPLOAD_iDocType, UPLOAD_vRefID";

                // Loop through excel datasorce 
                for (int i = 0; i <= dsSource.Tables[0].Rows.Count - 1; i++)
                {
                    if (i > 0)
                        strValues += " union all ";

                    strValues += " select ";

                    // Loop throgh FMT data source
                    for (int recCount = 0; recCount <= dsFMT.Tables[0].Rows.Count - 1; recCount++)
                    {
                        strValues = strValues + "&apos;" + dsSource.Tables[0].Rows[i][dsFMT.Tables[0].Rows[recCount]["TemplateFields_vName"].ToString()].ToString() + "&apos;" + ",";
                    }

                    strValues += " 0, &apos;BATCH&apos;, 1, " + loginUser.LoginOrgId + ", " + loginUser.UserId + ", getdate(), " +
                        cmbDepartment.SelectedValue + ", " + cmbDocumentType.SelectedValue +
                        ", dbo.FN_GetReferenceId(" + cmbDocumentType.SelectedValue + "," + cmbDepartment.SelectedValue + "," + loginUser.UserId + "," + loginUser.LoginOrgId + ")";
                }
                strQuery += "insert into BulkUploadSoftdataTemp (" + strFields + " ) " + strValues; /* DMS5-4339 M */


                strXML = "<table><row><query>" + strQuery + "</query></row></table>";

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }

            return strXML;
        }

        private String convDt(string inDate, string format)
        {
            try
            {
                string Indate = inDate;  //convert dd/mm/yyyy string to valid Datetime using C#. 
                System.Globalization.DateTimeFormatInfo dateInfo = new System.Globalization.DateTimeFormatInfo();
                dateInfo.ShortDatePattern = format;
                DateTime Outdate = Convert.ToDateTime(Indate, dateInfo);
                return Outdate.ToString("MM/dd/yyyy");
            }
            catch
            {
                throw new System.ArgumentException("Invalid date present in your schedule. Date format should be " + format, "");
            }
        }

        static bool IsNumber(string value)
        {
            // Return true if this is a number.
            decimal number1;
            return decimal.TryParse(value, out number1);
        }

        public void GetFilters(string loginOrgId, string loginToken)
        {
            BatchUploadBL bl = new BatchUploadBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                string action = "GetFilterOptions";
                Results res = bl.ManageFilterOption(action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.Items != null)
                {
                    cmbFilter.Items.Clear();
                    //cmbFilter.Items.Add(new ListItem("<Select>", "0"));
                    foreach (Item dp in res.Items)
                    {
                        cmbFilter.Items.Add(new ListItem(dp.Value.ToString(), dp.Key));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void GetDepartments(string loginOrgId, string loginToken)
        {

            DepartmentBL bl = new DepartmentBL();
            SearchFilter filter = new SearchFilter();
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
            try
            {
                string action = "DepartmentsForUpload";
                Results res = bl.SearchDepartments(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.Departments != null)
                {
                    cmbDepartment.Items.Clear();
                    cmbDepartment.Items.Add(new ListItem("<Select>", "0"));
                    foreach (Department dp in res.Departments)
                    {
                        cmbDepartment.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        #region Batch upload
        protected void AsyncFileUpload1_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            try
            {
                Logger.Trace("Started Saving Uploaded Document in AsyncFileUpload1_UploadedComplete ", Session["LoggedUserId"].ToString());

                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];

                if (!Directory.Exists(TempFolder))
                {
                    Directory.CreateDirectory(TempFolder);
                }
                if (AsyncFileUpload1.HasFile)
                {
                    AsyncFileUpload1.SaveAs(TempFolder + "\\" + AsyncFileUpload1.FileName);

                    switch (Path.GetExtension(TempFolder + "\\" + AsyncFileUpload1.FileName))
                    {
                        case ".xls":
                        case ".xlsx":
                            ReadExcel(TempFolder + "\\" + AsyncFileUpload1.FileName);
                            break;
                        case ".csv":
                            ReadCSV(TempFolder + "\\" + AsyncFileUpload1.FileName);
                            break;
                        case ".txt":
                            ReadText(TempFolder + "\\" + AsyncFileUpload1.FileName);
                            break;
                    }
                    try
                    {
                        File.Delete(TempFolder + "\\" + AsyncFileUpload1.FileName);
                    }
                    catch
                    {

                    }
                }
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Success", "ReloadDataOnUpload(); alert('Upload status: " + Messages + ".');", true); /* DMS5-4339 M */

                Logger.Trace("Finished Saving Uploaded Document in AsyncFileUpload1_UploadedComplete ", Session["LoggedUserId"].ToString());
            }
            catch (DirectoryNotFoundException ex)
            {
                string alert = "alert('Upload status: The file could not be uploaded. " + ex.Message.ToString() + "');";
                alert = alert.Replace("\r\n", "");
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", alert, true);
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
            catch (IOException ex)
            {
                string alert = "alert('Upload status: The file could not be uploaded. " + ex.Message.ToString() + "');";
                alert = alert.Replace("\r\n", "");
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", alert, true);
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                string alertScript = ex.Message.ToString().Replace("\r", "");
                alertScript = "alert('Upload status: The file could not be uploaded. " + alertScript +
                    "\\n\\nError occured may be because of following reasons:" +
                    "\\n  1.Data does not exist in first sheet of Excel/CSV." +
                    "\\n  2.Excel/CSV/Text file headers changed by the user." +
                    "\\n  3.Excel file is readonly."
                    + "');";
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", alertScript, true);
            }
        }

        private void ReadExcel(string filePath)
        {
            OleDbCommand cmd = new OleDbCommand(); ;
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            SearchFilter filter = new SearchFilter();
            string action = "ExportDocumentType";
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
            filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
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
                string SelectCommand = getIndexFieldsList();
                SelectCommand = "SELECT " + SelectCommand + " FROM [" + excelSheets[0] + "]";
                cmd.CommandText = SelectCommand;
                oleda = new OleDbDataAdapter(cmd);
                try
                {
                    oleda.Fill(ds);
                }
                catch
                {
                    throw new Exception("Selected file is not matching to " + cmbDocumentType.SelectedItem.Text + ".");//Bug Wrtier DMS ENHSMT 1-1012 M
                }



                string strXml = string.Empty;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    strXml = InsertIntoTempTable(ds, new BatchUploadBL().GetHeadersForBatchUpload(filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken), loginUser);
                    uploadBatchData(strXml);
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

        private void ReadText(string filePath)
        {
            string contents;
            int tabSize = 4;
            string[] arInfo, arColList;
            string line;
            //Create new DataTable.
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            SearchFilter filter = new SearchFilter();
            string action = "ExportDocumentType";
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
            filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);

            DataRow row;
            try
            {
                Logger.Trace("Going inside the function ReadText", Session["LoggedUserId"].ToString());
                //OpenPath = Server.MapPath(".") + @"\My File.log";
                //string FILENAME = OpenPath;
                //Get a StreamReader class that can be used to read the file
                StreamReader objStreamReader;
                objStreamReader = File.OpenText(filePath);
                arColList = getIndexFieldsList().Split(','); //.Replace("[","").Replace("]","")
                bool Firstline = true;
                char[] textdelimiter = { '|' };
                string strXml = string.Empty;
                DataTable table = null;

                while ((line = objStreamReader.ReadLine()) != null)
                {
                    //skip first line
                    if (Firstline)
                    {
                        contents = line.Replace(("").PadRight(tabSize, ' '), "\t");
                        // define which character is seperating fields
                        arInfo = contents.Split(textdelimiter);
                        table = getTempTable(arInfo);

                        line = objStreamReader.ReadLine();

                        Firstline = false;
                    }
                    contents = line.Replace(("").PadRight(tabSize, ' '), "\t");
                    // define which character is seperating fields
                    arInfo = contents.Split(textdelimiter);

                    row = table.NewRow();
                    for (int i = 0; i < arInfo.Length; i++)
                    {
                        row[i] = arInfo[i].ToString();


                    }
                    table.Rows.Add(row);
                }
                objStreamReader.Close();
                // Save to database
                DataSet ds = new DataSet();
                ds.Tables.Add(table);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    strXml = InsertIntoTempTable(ds, new BatchUploadBL().GetHeadersForBatchUpload(filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken), loginUser);
                    uploadBatchData(strXml);
                }
                else
                    throw new Exception("Upload status: The file could not be uploaded. No data available in uploaded file!");

                Logger.Trace("Successfully completed the function ReadText", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                throw new Exception(ex.Message);
            }
        }

        protected string getIndexFieldsList()
        {
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();

            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";

            string IndexFieldsList = string.Empty;

            try
            {
                string action = "ExportDocumentType";
                filter.CurrDocumentTypeId = Convert.ToInt32(cmbDocumentType.SelectedValue);
                filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                Results res = bl.SearchDocumentTypes(filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);

                if (res.ActionStatus == "SUCCESS")
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    if (res != null && res.IndexFields.Count > 0)
                    {
                        for (int i = 0; i < res.IndexFields.Count; i++)
                        {
                            if (res.IndexFields[i].IndexName.Trim().Length > 0)
                                IndexFieldsList = IndexFieldsList + "[" + res.IndexFields[i].IndexName + "],";
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Trace("Exception:"+ex.Message, Session["LoggedUserId"].ToString());
                throw new Exception(CoreMessages.GetMessages(hdnAction.Value, "ERROR"));
            }
            return IndexFieldsList.Substring(0, IndexFieldsList.Length - 1);
        }

        private DataTable getTempTable(string[] arColList)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < arColList.Length; i++)
            {
                dt.Columns.Add(arColList[i].ToString().Trim(), typeof(string));
            }
            return dt;
        }

        private void uploadBatchData(string BatchData)
        {
            try
            {
                Results Result = new Results(); /* DMS5-4339 A */

                CheckAuthentication();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;
                BatchUploadBL BatchUploadBL = new BatchUploadBL();
                BatchUploadBE BatchUpload = new BatchUploadBE();
                BatchUpload.batchData = BatchData;
                BatchUpload.DepartmentId = Convert.ToInt32(cmbDepartment.SelectedValue);
                BatchUpload.DocTypeId = Convert.ToInt32(cmbDocumentType.SelectedValue);
                string action = "UpdateBatchData";
                Result = BatchUploadBL.UploadBatchData(BatchUpload, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken); /* DMS5-4339 M */


                Messages = Result.Message; /* DMS5-4339 A*/

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:"+ex.Message, Session["LoggedUserId"].ToString());
                throw new Exception(CoreMessages.GetMessages(hdnAction.Value, "ERROR"));
                //divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
            }
        }

        public DataTable ReadCSV(string path, bool IsFirstRowHeader = true)//here Path is root of file and IsFirstRowHeader is header is there or not
        {
            string header = "No";
            string sql = string.Empty;
            DataSet ds = new DataSet();
            DataTable dataTable = null;
            string pathOnly = string.Empty;
            string fileName = string.Empty;
            DataTable Temp = new DataTable();
            DataSet ds1 = new DataSet();
            string strXml = string.Empty;

            UserBase loginUser = (UserBase)Session["LoggedUser"];
            SearchFilter filter = new SearchFilter();
            string action = "ExportDocumentType";
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
            filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
            try
            {
                pathOnly = Path.GetDirectoryName(path);
                fileName = Path.GetFileName(path);

                string SelectColumnList = getIndexFieldsList();

                sql = @"SELECT " + SelectColumnList + " FROM [" + fileName + "]";

                if (IsFirstRowHeader)
                {
                    header = "Yes";
                }

                using (OleDbConnection connection = new OleDbConnection(
                        @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                        ";Extended Properties=\"Text;HDR=" + header + "\""))
                {
                    using (OleDbCommand command = new OleDbCommand(sql, connection))
                    {
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                        {
                            dataTable = new DataTable();
                            dataTable.Locale = System.Globalization.CultureInfo.CurrentCulture;
                            adapter.Fill(dataTable);
                            ds.Tables.Add(dataTable);
                        }
                    }
                }

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    strXml = InsertIntoTempTable(ds, new BatchUploadBL().GetHeadersForBatchUpload(filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken), loginUser);
                    uploadBatchData(strXml);
                }
                else
                    throw new Exception("Upload status: The file could not be uploaded. No data available in uploaded file!");

            }
            finally
            {
                // Clean up.
                if (oledbConn != null)
                {
                    oledbConn.Close();
                    oledbConn.Dispose();
                }
                if (Temp != null)
                {
                    Temp.Dispose();
                }
                if (ds != null)
                {
                    ds.Dispose();
                }
                if (ds1 != null)
                {
                    ds1.Dispose();
                }
            }
            return dataTable;
        }

        #endregion

        protected void cmbDocumentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetDepartments(hdnLoginOrgId.Value, hdnLoginToken.Value);
        }
    }
}