/* ============================================================================  
Author     : Pratheesh A
Create date: 19 Nov 2013
Description: Enhancement 2 - Sprint 1 - Bulk Upload of users
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
=============================================================================== */
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.IO;
using System.Data;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.OleDb;
using System.Runtime.InteropServices;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class BulkUploadUsers : PageBase
    {
        UserBL BL = new UserBL();
        OleDbConnection oledbConn;
        protected int currentPageNumber;
        protected int PAGE_SIZE;

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            divMsg.InnerText = "";

            if (!IsPostBack)
            {
                divMsg.InnerText = "";
                if (Request.QueryString["act"] != null && Request.QueryString["act"] == "editsuccess")
                {
                    divMsg.Style.Add("color", "green");
                    divMsg.InnerText = "User Details Edited Successfully.";
                }

                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;
                hdnPageId.Value = "0";

                Session["BulkUploadStatus"] = null;

                string pageRights = GetPageRights();
                hdnPageRights.Value = pageRights;
                ApplyPageRights(pageRights, this.Form.Controls);

                BatchUploadBL bal = new BatchUploadBL();
                DataSet ds = bal.ComboFillerBySP("USP_COMBOFILLER_PAGE_SIZE");
                ddlRows.DataTextField = "_Name";
                ddlRows.DataValueField = "_ID";
                ddlRows.DataSource = ds;
                ddlRows.DataBind();

                ddlRows.SelectedValue = "10";

                lblMessage.Text = "";
                currentPageNumber = 1;
                PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                ViewState["SortOrder"] = "ASC";
                pBindData(null, false);
            }
        }

        /// <summary>
        /// Download template function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDownloadTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                string Time = System.DateTime.Now.ToString("hh") + System.DateTime.Now.ToString("mm") + System.DateTime.Now.ToString("ss");
                Logger.Trace("Copying Template Excel to Temp Folder Started", Session["LoggedUserId"].ToString());
               if (!Directory.Exists(TempFolder))
                {
                    Directory.CreateDirectory(TempFolder);
                }

                File.Copy(Server.MapPath("~/Assets/Templates/UserUploadTemplate.xlsx"), TempFolder + "UserUploadTemplate_" + Time + "_" + loginUser.UserId.ToString() + ".xlsx", true);

                Logger.Trace("Copying Template Excel to Temp Folder Finished", Session["LoggedUserId"].ToString());

                Logger.Trace("Calling DB to get details and fill the excel Started", Session["LoggedUserId"].ToString());

                DataSet dsTemplateDetails = new DataSet();

                dsTemplateDetails = BL.GetValuesForTemplate(0, "GetAllDetails", hdnLoginOrgId.Value, hdnLoginToken.Value);


                FillExcelValues(Path.Combine(TempFolder, "UserUploadTemplate_" + Time + "_" + loginUser.UserId.ToString() + ".xlsx"), dsTemplateDetails);

                Logger.Trace("Calling DB to get details and fill the excel Finished", Session["LoggedUserId"].ToString());

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + "UsersBulkUploadTemplate.xlsx");
                Response.TransmitFile(TempFolder + "UserUploadTemplate_" + Time + "_" + loginUser.UserId.ToString() + ".xlsx");
                Response.End();

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception"+ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        /// <summary>
        /// Function to read data from database and fill the excel
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dsValues"></param>
        public void FillExcelValues(string path, DataSet dsValues)
        {
            Excel.Range currentFind = null;
            var app = new Excel.Application();
            app.Visible = false;
            Excel.Workbook workbook = app.Workbooks.Open(path, 0, false, 5, "lotex@123", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);

            //setting sheet 2
            var worksheet = workbook.Sheets[2];

            //setting colum range
            Excel.Range foundNames = worksheet.Range["A2", "A51"];

            if (dsValues.Tables[0] != null && dsValues.Tables[0].Rows.Count > 0)
            {
                int i = 0;
                for (i = 0; i < dsValues.Tables[0].Rows.Count; i++)
                {
                    currentFind = foundNames.Find("UserGroupValues" + (i + 1), LookIn: XlFindLookIn.xlValues, LookAt: XlLookAt.xlWhole, MatchCase: true);
                    if (currentFind != null)
                    {
                        currentFind.Replace(What: "UserGroupValues" + (i + 1), Replacement: dsValues.Tables[0].Rows[i][0].ToString());
                    }
                }
                for (int j = i; j < 51; j++)
                {
                    currentFind = foundNames.Find("UserGroupValues" + (j + 1), LookIn: XlFindLookIn.xlValues, LookAt: XlLookAt.xlWhole, MatchCase: true);
                    if (currentFind != null)
                    {
                        currentFind.Delete();
                    }
                }
            }

            //setting colum range
            foundNames = worksheet.Range["B2", "B51"];

            if (dsValues.Tables[1] != null && dsValues.Tables[1].Rows.Count > 0)
            {
                int i = 0;
                for (i = 0; i < dsValues.Tables[1].Rows.Count; i++)
                {
                    currentFind = foundNames.Find("DepartmentValues" + (i + 1), LookIn: XlFindLookIn.xlValues, LookAt: XlLookAt.xlWhole, MatchCase: true);
                    if (currentFind != null)
                    {
                        currentFind.Replace(What: "DepartmentValues" + (i + 1), Replacement: dsValues.Tables[1].Rows[i][0].ToString());
                    }
                }
                for (int j = i; j < 51; j++)
                {
                    currentFind = foundNames.Find("DepartmentValues" + (j + 1), LookIn: XlFindLookIn.xlValues, LookAt: XlLookAt.xlWhole, MatchCase: true);
                    if (currentFind != null)
                    {
                        currentFind.Delete();
                    }
                }
            }

            //setting colum range
            foundNames = worksheet.Range["C2", "C4"];

            if (dsValues.Tables[2] != null && dsValues.Tables[2].Rows.Count > 0)
            {
                int i = 0;
                for (i = 0; i < 2; i++)
                {
                    currentFind = foundNames.Find("DomainUserValues" + (i + 1), LookIn: XlFindLookIn.xlValues, LookAt: XlLookAt.xlWhole, MatchCase: true);
                    if (currentFind != null)
                    {
                        currentFind.Replace(What: "DomainUserValues" + (i + 1), Replacement: dsValues.Tables[2].Rows[i][0].ToString());
                    }
                }
            }

            //setting colum range
            foundNames = worksheet.Range["D2", "D51"];

            if (dsValues.Tables[3] != null && dsValues.Tables[3].Rows.Count > 0)
            {
                int i = 0;
                for (i = 0; i < dsValues.Tables[3].Rows.Count; i++)
                {
                    currentFind = foundNames.Find("DomainValues" + (i + 1), LookIn: XlFindLookIn.xlValues, LookAt: XlLookAt.xlWhole, MatchCase: true);
                    if (currentFind != null)
                    {
                        currentFind.Replace(What: "DomainValues" + (i + 1), Replacement: dsValues.Tables[3].Rows[i][0].ToString());
                    }
                }
                for (int j = i; j < 51; j++)
                {
                    currentFind = foundNames.Find("DomainValues" + (j + 1), LookIn: XlFindLookIn.xlValues, LookAt: XlLookAt.xlWhole, MatchCase: true);
                    if (currentFind != null)
                    {
                        currentFind.Delete();
                    }
                }
            }
            //setting colum range
            foundNames = worksheet.Range["E2", "E4"];

            if (dsValues.Tables[4] != null && dsValues.Tables[4].Rows.Count > 0)
            {
                int i = 0;
                for (i = 0; i < 2; i++)
                {
                    currentFind = foundNames.Find("ActiveValues" + (i + 1), LookIn: XlFindLookIn.xlValues, LookAt: XlLookAt.xlWhole, MatchCase: true);
                    if (currentFind != null)
                    {
                        currentFind.Replace(What: "ActiveValues" + (i + 1), Replacement: dsValues.Tables[4].Rows[i][0].ToString());
                    }
                }
            }

            workbook.Save();
            workbook.Close();
            app.Workbooks.Close();
            app.Quit();
            Marshal.ReleaseComObject(workbook);
            Marshal.ReleaseComObject(app);

        }

        /// <summary>
        /// Saving the uploaded file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UsersAsyncFileUpload_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                Logger.Trace("Started Processing Document in UsersAsyncFileUpload_UploadedComplete ", Session["LoggedUserId"].ToString());

                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                string Time = System.DateTime.Now.ToString("hh") + System.DateTime.Now.ToString("mm") + System.DateTime.Now.ToString("ss");


                if (!Directory.Exists(TempFolder))
                {
                    Directory.CreateDirectory(TempFolder);
                }

                if (UsersAsyncFileUpload.HasFile)
                {
                    UsersAsyncFileUpload.SaveAs(TempFolder + "\\" + "UserUploadTemplate_" + Time + "_" + loginUser.UserId.ToString() + ".xlsx");
                    Session["BulkUploadStatus"] = Time;
                }
                else
                {
                    Session["BulkUploadStatus"] = null;
                }

                Logger.Trace("Finished Processing Document in UsersAsyncFileUpload_UploadedComplete ", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception "+ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        /// <summary>
        /// After file upload btnSubmit_Click will be called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                if (Session["BulkUploadStatus"] != null && hdnBtnAction.Value == "ReadExcelData")
                {
                    string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                    ReadExcel(TempFolder + "\\" + "UserUploadTemplate_" + Session["BulkUploadStatus"].ToString() + "_" + loginUser.UserId.ToString() + ".xlsx");
                }
                else if (hdnBtnAction.Value == "DeleteUser" && hdnUserID.Value != null)
                {
                    Logger.Trace("Started User Delete Functionality", Session["LoggedUserId"].ToString());

                    bool result = BL.DeleteTempUser(0, "DeleteUser", hdnLoginOrgId.Value, hdnLoginToken.Value, Convert.ToInt32(hdnUserID.Value));

                    if (result == true)
                    {
                        divMsg.Style.Add("color", "green");
                        divMsg.InnerText = "User Details Removed Successfully.";
                    }
                    else
                    {
                        divMsg.Style.Add("color", "red");
                        divMsg.InnerText = "Error occured while deleting user details!";
                    }

                    lblMessage.Text = "";
                    currentPageNumber = 1;
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                    ViewState["SortOrder"] = "ASC";
                    pBindData(null, false);


                    Logger.Trace("Finished User Delete Functionality", Session["LoggedUserId"].ToString());
                }
                else if (hdnBtnAction.Value == "DiscardUsers")
                {
                    Logger.Trace("Started User Delete Functionality", Session["LoggedUserId"].ToString());

                    bool result = BL.DiscardTempUser(0, "DiscardUsers", hdnLoginOrgId.Value, hdnLoginToken.Value);

                    if (result == true)
                    {
                        divMsg.Style.Add("color", "green");
                        divMsg.InnerText = "User(s) details discarded Successfully.";
                    }
                    else
                    {
                        divMsg.Style.Add("color", "red");
                        divMsg.InnerText = "Error occured while discarding user(s) details!";
                    }

                    lblMessage.Text = "";
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                    ViewState["SortOrder"] = "ASC";
                    pBindData(null, false);


                    Logger.Trace("Finished User Delete Functionality", Session["LoggedUserId"].ToString());
                }
                else if (hdnBtnAction.Value == "CommitUsers")
                {
                    Logger.Trace("Started User Commit Functionality", Session["LoggedUserId"].ToString());

                    bool result = BL.CommitTempUser(0, "CommitUsers", hdnLoginOrgId.Value, hdnLoginToken.Value);

                    if (result == true)
                    {
                        divMsg.Style.Add("color", "green");
                        divMsg.InnerText = "User(s) details commited Successfully.";
                    }
                    else
                    {
                        divMsg.Style.Add("color", "red");
                        divMsg.InnerText = "Error occured while commiting user(s) details!";
                    }

                    lblMessage.Text = "";
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                    ViewState["SortOrder"] = "ASC";
                    pBindData(null, false);


                    Logger.Trace("Finished User Commit Functionality", Session["LoggedUserId"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:"+ex.Message, Session["LoggedUserId"].ToString());
                //ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Upload status: The file could not be uploaded. Excel file doesnt match with the Template.');", true);

                string alertScript = "alert('Upload status: The file could not be uploaded." +
                    "\\n\\nError occured may be because of following reasons:" +
                    "\\n  1."+ ex.Message.ToString() +
                    "\\n  2.Excel file may be corrupted." +
                    "\\n  3.Server may be busy. " 
                    + "');";
                ScriptManager.RegisterStartupScript(this, typeof(string), "Error", alertScript, true);
            }
        }

        /// <summary>
        /// Function to read data from excel and save to database
        /// </summary>
        /// <param name="filePath"></param>
        private void ReadExcel(string filePath)
        {
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            OleDbCommand cmd = new OleDbCommand(); ;
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            DataSet ds = new DataSet();
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                Logger.Trace("Started Extracting Soft Data",Session["LoggedUserId"].ToString());
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

                cmd.Connection = oledbConn;
                cmd.CommandType = CommandType.Text;
                // Get column names of selected document type
                string SelectCommand;
                try
                {
                    SelectCommand = "SELECT [Key] FROM [Masters$]";
                    cmd.CommandText = SelectCommand;
                    oleda = new OleDbDataAdapter(cmd);
                    oleda.Fill(ds);

                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[0][0].ToString().Trim() != "UBUTVER1")
                    {
                        throw new Exception("Please upload the exact template file");
                    }

                    ds = new DataSet();
                    SelectCommand = "SELECT [UserName*],[Description],[First Name*],[LastName*],[Email*],[Mobile*],[UserGroup*],[Department*],[DomainUser*],[Domain*],[Active*] FROM [User_Details$]";
                    cmd.CommandText = SelectCommand;
                    oleda = new OleDbDataAdapter(cmd);
                    try
                    {
                        oleda.Fill(ds);
                    }
                    catch
                    {
                        throw new Exception("Selected file missing required columns");
                    }

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        System.Data.DataTable ExcelDT = new System.Data.DataTable();
                        ExcelDT.Columns.Add("UserName");
                        ExcelDT.Columns.Add("Description");
                        ExcelDT.Columns.Add("FirstName");
                        ExcelDT.Columns.Add("LastName");
                        ExcelDT.Columns.Add("Email");
                        ExcelDT.Columns.Add("Mobile");
                        ExcelDT.Columns.Add("UserGroup");
                        ExcelDT.Columns.Add("Department");
                        ExcelDT.Columns.Add("DomainUser");
                        ExcelDT.Columns.Add("Domain");
                        ExcelDT.Columns.Add("Active");

                        int RowCounter = 0;
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            ExcelDT.ImportRow(dr);
                            int ColCounter = 0;
                            foreach (DataColumn col in ds.Tables[0].Columns)
                            {
                                ExcelDT.Rows[RowCounter][ColCounter] = dr[col].ToString();
                                ColCounter++;
                            }
                            RowCounter++;
                        }

                        DataSet ds1 = new DataSet();
                        ds1.Tables.Add(ExcelDT);
                        string BatchData = ds1.GetXml().Replace("_x005B_", "").Replace("_x005D_", "");
                        bool result = BL.UploadSoftData(0, "UploadSoftData", hdnLoginOrgId.Value, hdnLoginToken.Value, BatchData);
                        if (result != true)
                        {
                            throw new Exception("Error occured while saving data to database!");
                        }
                        else
                        {
                            lblMessage.Text = "";
                            currentPageNumber = 1;
                            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                            ViewState["SortOrder"] = "ASC";
                            pBindData(null, false);
                        }
                    }
                }
                catch
                {
                    throw new Exception("Please upload the exact template file");
                }
               
                    Logger.Trace("Finished Extracting Soft Data", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception"+ex.Message, Session["LoggedUserId"].ToString());

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
            //DataSet ds = new DataSet();
            //Excel.Range currentFind = null;
            //var app = new Excel.Application();
            //app.Visible = false;
            //try
            //{
            //    Logger.Trace("Started Extracting Soft Data", loginUser.UserId.ToString());


            //    Excel.Workbook workbook = app.Workbooks.Open(filePath, 0, true, 5, "lotex@123", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            //    //setting sheet 2
            //    var worksheet = workbook.Sheets[2];

            //    //setting colum range
            //    Excel.Range foundNames = worksheet.Range["F2", "F3"];

            //    currentFind = foundNames.Find("UBUTVER1", LookIn: XlFindLookIn.xlValues, LookAt: XlLookAt.xlWhole, MatchCase: true);
            //    if (currentFind != null)
            //    {

            //        worksheet = workbook.Sheets[1];
            //        int index = 0;
            //        object rowIndex = 2;

            //        System.Data.DataTable ExcelDT = new System.Data.DataTable();
            //        ExcelDT.Columns.Add("UserName");
            //        ExcelDT.Columns.Add("Description");
            //        ExcelDT.Columns.Add("FirstName");
            //        ExcelDT.Columns.Add("LastName");
            //        ExcelDT.Columns.Add("Email");
            //        ExcelDT.Columns.Add("Mobile");
            //        ExcelDT.Columns.Add("UserGroup");
            //        ExcelDT.Columns.Add("Department");
            //        ExcelDT.Columns.Add("DomainUser");
            //        ExcelDT.Columns.Add("Domain");
            //        ExcelDT.Columns.Add("Active");

            //        DataRow row;

            //        while (((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 1]).Value2 != null)
            //        {
            //            rowIndex = 2 + index;
            //            row = ExcelDT.NewRow();
            //            row[0] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 1]).Value2);
            //            row[1] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 2]).Value2);
            //            row[2] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 3]).Value2);
            //            row[3] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 4]).Value2);
            //            row[4] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 5]).Value2);
            //            row[5] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 6]).Value2);
            //            row[6] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 7]).Value2);
            //            row[7] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 8]).Value2);
            //            row[8] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 9]).Value2);
            //            row[9] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 10]).Value2);
            //            row[10] = Convert.ToString(((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[rowIndex, 11]).Value2);
            //            index++;
            //            ExcelDT.Rows.Add(row);
            //        }
            //        workbook.Close();
            //        app.Workbooks.Close();
            //        app.Quit();
            //        Marshal.ReleaseComObject(workbook);
            //        Marshal.ReleaseComObject(app);
            //        ds.Tables.Add(ExcelDT);
            //        string BatchData = ds.GetXml().Replace("_x005B_", "").Replace("_x005D_", "").Replace("<Table1 />", "");
            //        bool result = BL.UploadSoftData(0, "UploadSoftData", hdnLoginOrgId.Value, hdnLoginToken.Value, BatchData);
            //        if (result != true)
            //        {
            //            throw new Exception("Error occured while saving data to database!");
            //        }
            //        else
            //        {
            //            lblMessage.Text = "";
            //            currentPageNumber = 1;
            //            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
            //            ViewState["SortOrder"] = "ASC";
            //            pBindData(null, false);
            //        }
            //    }
            //    else
            //    {
            //        app.Workbooks.Close();
            //        app.Quit();
            //        throw new Exception("File doesn't match");
            //    }

            //}
            //catch (Exception ex)
            //{
            //    app.Workbooks.Close();
            //    app.Quit();
            //    throw new Exception(ex.Message.ToString());


            //}

        }

        #region "Page Events"

        protected void GetPageIndex(object sender, CommandEventArgs e)
        {

            switch (e.CommandName)
            {
                case "First":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = 1;
                    break;

                case "Previous":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = Int32.Parse(ddlPage.SelectedValue) - 1;
                    break;

                case "Next":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = Int32.Parse(ddlPage.SelectedValue) + 1;
                    break;

                case "Last":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = Int32.Parse(lblTotalPages.Text);
                    break;
            }

            pBindData(null, false);
        }

        protected void ddlRows_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPageNumber = 1;
            grdView.PageSize = Int32.Parse(ddlRows.SelectedValue);
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
            pBindData(null, false);
        }

        protected void ddlPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
            currentPageNumber = Int32.Parse(ddlPage.SelectedValue);
            pBindData(null, false);
        }

        protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string encoded = e.Row.Cells[i].Text;
                e.Row.Cells[i].Text = Context.Server.HtmlDecode(encoded);
            }


            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].CssClass = "GridItem1";
                if (e.Row.Cells[i].Text.Trim() != "&nbsp;")
                {
                    e.Row.Cells[i].Attributes.Add("title", e.Row.Cells[i].Text);
                }
            }
            e.Row.Attributes.Add("onmouseover", "javascript:this.className = 'GridRowHover'");
            e.Row.Attributes.Add("onmouseout", "javascript:this.className = ''");
            e.Row.TabIndex = -1;
            e.Row.Attributes["onclick"] = string.Format("javascript:SelectRow(this, {0});", e.Row.RowIndex);
            e.Row.Attributes["onkeydown"] = "javascript:return SelectSibling(event);";
            e.Row.Attributes["onselectstart"] = "javascript:return false;";

        }

        protected void grdView_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (string.Compare(Convert.ToString(ViewState["SortOrder"]), " ASC", true) == 0)
            {
                ViewState["SortOrder"] = " DESC";
            }
            else
            {
                ViewState["SortOrder"] = " ASC";
            }
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
            currentPageNumber = Int32.Parse(ddlPage.SelectedValue);  // Added later
            pBindData("[" + e.SortExpression + "]" + ViewState["SortOrder"], false);
        }


        #endregion

        #region Private Method

        private int GetTotalPages(double totalRows)
        {
            int totalPages = (int)Math.Ceiling(totalRows / PAGE_SIZE);

            return totalPages;
        }

        private void pBindData(string aSortExp, bool aIsCompleteData)
        {
            DataSet ds = null;
            try
            {
                // Clear grid data
                grdView.DataSource = null;
                grdView.DataBind();

                ds = BL.GetDataForGrid(((currentPageNumber - 1) * PAGE_SIZE) + 1, (aIsCompleteData == true ? -1 : (currentPageNumber * PAGE_SIZE)), "GetDataForGrid", hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    grdView.DataSource = ds.Tables[0];
                    grdView.DataBind();
                    PageDiv.Visible = true;
                    ///get the total rows 
                    double totalRows = 0;

                    totalRows = Convert.ToDouble(ds.Tables[1].Rows[0]["Rowcount"].ToString());
                    btnDiscard.Attributes.Add("class", "btnclose");
                    btnDiscard.Width = Unit.Pixel(120);
                    if (ds.Tables[1].Rows[0]["Commit"].ToString().ToUpper() == "YES")
                    {
                        btnCommit.Attributes.Add("class", "btncommit");
                    }
                    else
                    {
                        btnCommit.Attributes.Add("class", "HiddenButton");
                    }

                    lblTotalPages.Text = GetTotalPages(totalRows).ToString();

                    ddlPage.Items.Clear();
                    for (int i = 1; i < Convert.ToInt32(lblTotalPages.Text) + 1; i++)
                    {
                        ddlPage.Items.Add(new ListItem(i.ToString()));
                    }

                    ddlPage.SelectedValue = currentPageNumber.ToString();

                    if (currentPageNumber == 1)
                    {
                        lnkbtnPre.Enabled = false;
                        lnkbtnPre.CssClass = "GridPagePreviousInactive";
                        lnkbtnFirst.Enabled = false;
                        lnkbtnFirst.CssClass = "GridPageFirstInactive";

                        if (Int32.Parse(lblTotalPages.Text) > 1)
                        {
                            lnkbtnNext.Enabled = true;
                            lnkbtnNext.CssClass = "GridPageNextActive";
                            lnkbtnLast.Enabled = true;
                            lnkbtnLast.CssClass = "GridPageLastActive";
                        }
                        else
                        {
                            lnkbtnNext.Enabled = false;
                            lnkbtnNext.CssClass = "GridPageNextInactive";
                            lnkbtnLast.Enabled = false;
                            lnkbtnLast.CssClass = "GridPageLastInactive";
                        }
                    }

                    else
                    {
                        lnkbtnPre.Enabled = true;
                        lnkbtnPre.CssClass = "GridPagePreviousActive";
                        lnkbtnFirst.Enabled = true;
                        lnkbtnFirst.CssClass = "GridPageFirstActive";

                        if (currentPageNumber == Int32.Parse(lblTotalPages.Text))
                        {
                            lnkbtnNext.Enabled = false;
                            lnkbtnNext.CssClass = "GridPageNextInactive";
                            lnkbtnLast.Enabled = false;
                            lnkbtnLast.CssClass = "GridPageLastInactive";
                        }
                        else
                        {
                            lnkbtnNext.Enabled = true;
                            lnkbtnNext.CssClass = "GridPageNextActive";
                            lnkbtnLast.Enabled = true;
                            lnkbtnLast.CssClass = "GridPageLastActive";
                        }
                    }
                }
                else
                {
                    PageDiv.Visible = false;
                    btnCommit.Attributes.Add("class", "HiddenButton");
                    btnDiscard.Attributes.Add("class", "HiddenButton");
                }

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                Logger.Trace("Exception:"+ex.Message, Session["LoggedUserId"].ToString());
            }
        }


        #endregion


    }
}