/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * -----------------------------------------------------------------------------------------------------------------------------------
 * 23 Mar 2015      Yogeesha Naik       DMS04-3459	        Created property to sotre previous page name and handled page reload(postback) .
 * 24 Mar 2015      Yogeesha Naik       DMS04-3777	        Service call failed.
 * 16 Apr 2015      Sabina              DMS5-3935           Login management:redirecting to either workflow home page or DMS based on application access
 * 27 Apr 2015      Yogeesha Naik       DMS5-4052	        Menu - Re factor menu implementation
====================================================================================================================================== */
using System;
using System.Web;
using System.IO;
using System.Text;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Globalization;
using System.Security.Cryptography;


namespace Lotex.EnterpriseSolutions.WebUI
{
    public class PageBase : System.Web.UI.Page
    {
        public int LoggedUserId
        {
            get
            {
                if (Session["LoggedUserId"] != null)
                {
                    return Convert.ToInt32(Session["LoggedUserId"]);
                }
                else
                    return 0;
            }
            set { Session["LoggedUserId"] = value; }
        }

        //DMS5-3935BS  added property to get the master page of the page inorder to swap the configuration master pages.
        public string MasterPage
        {
            get
            {
                return Session["MasterPage"] == null ? string.Empty : Convert.ToString(Session["MasterPage"]);
            }
            set { Session["MasterPage"] = value; }
        }
        // DMS5-3935BE

        //DMS04-3459BS -- Property to store previous page name
        public string PreviousPage
        {
            get
            {
                //check if the webpage is loaded for the first time.
                if (!IsPostBack && Request.UrlReferrer != null)
                    Session["PreviousPage"] = Request.UrlReferrer.AbsoluteUri.Substring(Request.UrlReferrer.AbsoluteUri.LastIndexOf('/') + 1).ToLower().Replace(".aspx", "").Replace("?" + Request.UrlReferrer.AbsoluteUri.Substring(Request.UrlReferrer.AbsoluteUri.LastIndexOf('?') + 1).ToLower().Replace(".aspx", ""), "");

                return Session["PreviousPage"] != null ? Convert.ToString(Session["PreviousPage"]) : string.Empty;
            }
            set { Session["PreviousPage"] = value; }
        }
        //DMS04-3459BE --

        protected override void OnInit(EventArgs e)
        {
            //Current page
            string CurrentPage = Request.Url.AbsoluteUri.Substring(Request.Url.AbsoluteUri.LastIndexOf('/') + 1).ToLower().Replace(".aspx", "").Replace("?" + Request.Url.AbsoluteUri.Substring(Request.Url.AbsoluteUri.LastIndexOf('?') + 1).ToLower().Replace(".aspx", ""), "");

            // Previous Page
            if (Request.UrlReferrer != null)
            {

                //DMS04-3459D - used property to store previous page name
                // string PreviousPage = Request.UrlReferrer.AbsoluteUri.Substring(Request.UrlReferrer.AbsoluteUri.LastIndexOf('/') + 1).ToLower().Replace(".aspx", "").Replace("?" + Request.UrlReferrer.AbsoluteUri.Substring(Request.UrlReferrer.AbsoluteUri.LastIndexOf('?') + 1).ToLower().Replace(".aspx", ""), "");

                switch (PreviousPage.ToLower())
                {
                    case "documentdownloaddetails":
                    case "documentqualitycheck":

                        if (PreviousPage != CurrentPage)
                        {
                            ReleaseDocment();

                        }
                        break;

                    default:
                        //Response.Cookies["PreviousPage"].Value = Request.Url.AbsoluteUri;
                        break;
                }
            }
        }

        // Release document which is lockd by user for editing purpose
        public void ReleaseDocment()
        {
            UploadDocBL objUploadDocBL = new UploadDocBL();
            DataSet dsData = new DataSet();
            int userId = 0;
            int processId = 0;
            int orgId = 0;
            string token = string.Empty;
            string message = string.Empty;
            int ErrorState = 0;
            int ErrorSeverity = 0;
            Results rs = new Results();
            try
            {
                userId = Convert.ToInt32(Session["LoggedUserId"]);
                rs.UserData = (CoreBE.UserBase)Session["LoggedUser"];

                token = rs.UserData.LoginToken;
                orgId = Convert.ToInt32(Session["OrgID"]);
                if (Session["ProcessId"] != null)
                {
                    processId = Convert.ToInt32(Session["ProcessId"]);
                    if (processId != 0)
                    {
                        dsData = objUploadDocBL.UpdateDocumentStatusForLock("UnLock Document", processId, userId, token, orgId, out message, out ErrorState, out ErrorSeverity);
                    }

                }

            }
            catch
            {


            }
        }

        #region Constructor
        public PageBase() { }
        #endregion

        #region General Methods
        /// <summary>
        /// Common Function for LogOut from any page
        /// </summary>
        public void LogOutAndRedirection(string orgCode, string msg)
        {
            if (orgCode != string.Empty)
            {
                Response.Redirect("~/Accounts/Login.aspx?org=" + orgCode + "&msg=" + msg, false);
            }
            else
            {
                InitialiseSessionVariables();
                Response.Redirect("~/Accounts/LogOut.aspx?msg=sessionout", true);
            }
        }
        /// <summary>
        /// Common Function for LogOut from any page
        /// </summary>
        public void LogOutAndRedirectionWithErrorMessge(string message)
        {
            string orName = Session["LoginOrgCode"] != null ? Session["LoginOrgCode"].ToString() : string.Empty;
            Response.Redirect("~/Accounts/Login.aspx?org=" + orName + "&msg=" + message, false);
        }
        /// <summary>
        /// Check Forms Authentication & configure the page caching
        /// </summary>
        public void CheckAuthentication()
        {
            string orgname = string.Empty;
            if ((!User.Identity.IsAuthenticated) || Session["LoggedUser"] == null)
            {
                //Response.Redirect("~/Accounts/LogOut.aspx?msg=sessionout", true);
                if (Request.Cookies["Orgcode"].Value != null)
                {
                    orgname = Request.Cookies["Orgcode"].Value;
                }

                string message = "You have logged out due to inactivity";
                if (orgname == string.Empty || orgname == null)
                {
                    Response.Redirect("~/Accounts/Login.aspx?org=" + "global" + "&msg=" + message, true);
                }
                else
                {
                    Response.Redirect("~/Accounts/Login.aspx?org=" + orgname + "&msg=" + message, true);
                }

            }
        }
        /// <summary>
        /// To open a file from a link in browser
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="name"></param>
        /// <param name="ext"></param>
        public void FileLinkOpen(string filePath, string name, string ext)
        {
            try
            {
                string type = string.Empty;
                if (ext != null)
                {
                    switch (ext.ToLower())
                    {
                        case ".htm":
                        case ".html":
                            type = "text/HTML";
                            break;

                        case ".txt":
                        case ".x12":
                            type = "text/plain";
                            break;

                        case ".doc":
                        case ".ex":
                        case ".rtf":
                            type = "Application/msword";
                            break;
                        case ".xls":
                            // type = "application/vnd.ms-excel";
                            type = "text/csv";
                            break;
                    }
                }
                Response.ContentType = type;
                Response.AppendHeader("content-disposition", "attachment; filename=" + name);
                Response.TransmitFile(@filePath.ToString());
                Response.End();
            }
            catch
            {
                //ShowMessage(ex.Message);
            }
        }
        /// <summary>
        /// Get culture based date format
        /// </summary>
        /// <returns></returns>
        public string GetDateFormat()
        {
            string dateFormat = string.Empty;
            switch (System.Globalization.CultureInfo.CurrentCulture.ToString())
            {
                case "en-US":
                    dateFormat = "MM/dd/yyyy";
                    break;
                case "en-UK":
                case "en-IN":
                    dateFormat = "dd/MM/yyyy";
                    break;
            }
            return dateFormat;
        }

        public bool SendMessage(User loginUser, string action)
        {
            bool status = false;
            StreamReader reader = null;
            try
            {
                if (action == "GetUserDataToSendPassword") //forgot password
                {
                    reader = new StreamReader(Server.MapPath("~/Accounts/MailFormat.htm"));
                    string readFile = reader.ReadToEnd();
                    string messageBody = "";
                    messageBody = readFile;
                    messageBody = messageBody.Replace("$$UserName$$", loginUser.UserName);
                    messageBody = messageBody.Replace("$$NewPassword$$", loginUser.Password);
                    messageBody = messageBody.Replace("$$OrgEmail$$", loginUser.OrgEmailId);
                    messageBody = messageBody.Replace("$$Website$$", loginUser.LoginWebsiteUrl);
                    messageBody = messageBody.Replace("$$Link$$", loginUser.LoginWebsiteUrl);
                    messageBody = messageBody.Replace("$$OrgName$$", loginUser.LoginOrgName);
                    messageBody = messageBody.ToString();
                    MailHelper.SendMailMessage(loginUser.OrgEmailId, loginUser.EmailId, loginUser.OrgEmailId, string.Empty, "Forgot Password Request", messageBody);
                    status = true;
                }
                else if (action == "AddOrgSentMail") //new customer creation
                {
                    reader = new StreamReader(Server.MapPath("~/Accounts/CustomerMailFormat1.htm"));
                    string readFile = reader.ReadToEnd();
                    string messageBody = "";
                    messageBody = readFile;
                    messageBody = messageBody.Replace("$$UserName$$", loginUser.UserName);
                    messageBody = messageBody.Replace("$$NewPassword$$", loginUser.Password);
                    messageBody = messageBody.Replace("$$OrgEmail$$", loginUser.OrgEmailId);
                    messageBody = messageBody.Replace("$$Website$$", loginUser.LoginWebsiteUrl);
                    messageBody = messageBody.Replace("$$OrgName$$", loginUser.LoginOrgName);
                    messageBody = messageBody.ToString();
                    MailHelper.SendMailMessage(loginUser.OrgEmailId, loginUser.EmailId, loginUser.OrgEmailId, string.Empty, "Customer Registration Confirmation", messageBody);
                    status = true;
                }
                else if (action == "AddUserSentMail") //new user creation 
                {
                    reader = new StreamReader(Server.MapPath("~/Accounts/UserRegistrationMailFormat.htm"));
                    string readFile = reader.ReadToEnd();
                    string messageBody = "";
                    messageBody = readFile;
                    messageBody = messageBody.Replace("$$UserName$$", loginUser.UserName);
                    messageBody = messageBody.Replace("$$NewPassword$$", loginUser.Password);
                    messageBody = messageBody.Replace("$$OrgEmail$$", loginUser.OrgEmailId);
                    messageBody = messageBody.Replace("$$Website$$", loginUser.LoginWebsiteUrl);
                    messageBody = messageBody.Replace("$$OrgName$$", loginUser.LoginOrgName);
                    messageBody = messageBody.Replace("$$FirstName$$", loginUser.FirstName);
                    messageBody = messageBody.Replace("$$LastName$$", loginUser.LastName);
                    messageBody = messageBody.ToString();
                    MailHelper.SendMailMessage(loginUser.OrgEmailId, loginUser.EmailId, loginUser.OrgEmailId, string.Empty, "User Registration Confirmation", messageBody);
                    status = true;
                }
            }
            catch
            {
                status = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return status;
        }

        /// <summary>
        /// Encrypt the query string
        /// </summary>
        public string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        /// <summary>
        /// Decrypt Querystring
        /// </summary>
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public void InitialiseSessionVariables()
        {


            Session["LoggedUser"] = null;
            Session["LoginOrgCode"] = null;
            Session["LoginOrgLogoPath"] = null;
            Session["User_MenuData"] = null;
            Session["LoginOrgName"] = null;
            Session["LoggedUserId"] = null;
            Session["OrgID"] = null;
            Session["LoggedUserName"] = null;
            Session["FullTextClause"] = null;

        }

        // DMS5-4052 BS
        /// <summary>
        /// Returns page name without extension
        /// </summary>
        /// <returns></returns>
        public string GetPageName()
        {
            string path = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            System.IO.FileInfo info = new System.IO.FileInfo(path);
            return info.Name.Replace(".aspx", string.Empty);
        }
        // DMS5-4052 BE

        /// <summary>
        /// Returns pages rights for the current page or provided page.
        /// </summary>
        /// <param name="pagename"> Default is current page</param>
        /// <returns></returns>
        public string GetPageRights(string pagename = "") // DMS5-4052 M -- made pagename as default param
        {
            // DMS5-4052 BS
            if (pagename.Equals(string.Empty))
                pagename = GetPageName();
            // DMS5-4052 BE

            string rights = string.Empty;
            if (Session["User_MenuData"] != null)
            {
                DataSet dsTemp = (DataSet)Session["User_MenuData"];
                if (dsTemp.Tables[0].Rows.Count > 0)
                {
                    // DMS5-4052 M
                    System.Data.DataView dv = new System.Data.DataView(dsTemp.Tables[0], "page_name = '" + pagename + "'", "", System.Data.DataViewRowState.CurrentRows);
                    System.Data.DataTable DataTempTable = dv.ToTable();
                    if (DataTempTable.Rows.Count > 0)
                    {
                        rights = DataTempTable.Rows[0]["PageRights"].ToString();
                    }
                }
                if (dsTemp != null)
                {
                    dsTemp.Dispose();
                }
            }
            return rights;
        }
        public void ApplyPageRights(string pageRights, ControlCollection formControls)
        {
            //if (!pageRights.Contains("Read"))
            //{
            //    if (pageRights != string.Empty)
            //    {
            //        pageRights += pageRights == string.Empty ? "Read" : ",Read";

            //    }
            //}
            foreach (Control c in formControls)
            {
                if (c.ID != null)
                {
                    if (c is Button)
                    {
                        Button btn = ((Button)c);
                        if (btn.Text != "Logout")
                        {
                            string tagName = string.Empty;
                            if (btn.Attributes["TagName"] != null)
                            {
                                tagName = btn.Attributes["TagName"].ToString();
                                ViewState["TagName"] = tagName;
                            }
                            if (tagName == string.Empty || !pageRights.Contains(tagName))
                            {
                                btn.Enabled = false;
                            }
                        }
                    }
                    //Newly added for Enchancmnt4 to make document visibile to authorised users only based on rights.
                    if (c is Panel)
                    {
                        Panel pnl = ((Panel)c);
                        if (pageRights.ToLower().Contains("read") || pageRights.ToLower().Contains("add") || pageRights.ToLower().Contains("upload") || pageRights.ToLower().Contains("downloadoriginal") || pageRights.ToLower().Contains("delete") || pageRights.ToLower().Contains("documentshare") || pageRights.ToLower().Contains("download annotated"))
                        {
                            if (pnl.ID == "DocumentDetails")//Panel containing the Document details.
                            {
                                //  pnl.Attributes.Add("style", "display:none;");

                                pnl.Enabled = false;
                                if (pageRights.ToLower().Contains("edit"))
                                {
                                    //pnl.Attributes.Add("style", "display:block;");   
                                    pnl.Enabled = false;
                                }

                            }

                            if (pnl.ID == "pnlmail")
                            {
                                pnl.Attributes.Add("style", "display:block;");
                                pnl.Enabled = true;
                            }
                        }
                        if (pageRights.ToLower().Contains("edit"))
                        {
                            if (pnl.ID == "DocumentDetails")
                            {
                                //pnl.Attributes.Add("style", "display:block;");
                                pnl.Enabled = false;
                            }

                            if (pnl.ID == "pnlmail")
                            {
                                pnl.Attributes.Add("style", "display:none;");
                            }
                        }
                        if (pageRights.ToLower().Contains("documentshare"))
                        {
                            if (pnl.ID == "pnlEmail")
                            {
                                pnl.Attributes.Add("style", "display:block;");
                            }
                        }
                        if (pageRights.ToLower().Contains("documentshare") && pageRights.ToLower().Contains("edit"))
                        {
                            if (pnl.ID == "pnlEmail")
                            {
                                pnl.Attributes.Add("style", "display:block;");
                            }
                            if (pnl.ID == "DocumentDetails")
                            {
                                //  pnl.Attributes.Add("style", "display:block;");
                                pnl.Enabled = false;
                            }

                        }
                        if (!pageRights.ToLower().Contains("documentshare"))
                        {
                            if (pnl.ID == "pnlEmail")
                            {
                                pnl.Attributes.Add("style", "display:none");
                            }
                        }

                    }
                    else
                    {
                        if (c.HasControls())
                        {
                            ApplyPageRights(pageRights, c.Controls);
                        }
                    }

                }
            }
        }
        public string Encryptdata(string password)
        {
            string strmsg = string.Empty;
            byte[] encode = new
            byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }
        public string beforedot(string s)
        {
            int l = s.LastIndexOf(".");
            return s.Substring(0, l);
        }

        public string afterdot(string s)
        {
            string[] array = s.Split('.');
            string Ext = "." + array[array.Length - 1];
            return Ext;
        }

        public string GetSrc(string link)
        {
            string src = string.Empty;
            src = getRootWebSitePath();

            if (link == "Handler")
            {
                src = src + "/GenericHandler.ashx?f=";
            }
            else
            {
                src = src + "/Images/preview-not-available.pdf";
            }

            return src;

        }

        public string getRootWebSitePath()
        {
            string _location = HttpContext.Current.Request.Url.ToString();
            int applicationNameIndex = _location.IndexOf("/", _location.IndexOf("://") + 3);
            string applicationName = _location.Substring(0, applicationNameIndex) + '/';
            int webFolderIndex = _location.IndexOf('/', _location.IndexOf(applicationName) + applicationName.Length);
            string webFolderFullPath = _location.Substring(0, webFolderIndex);

            //If develoment environement WEB folder won't be there
            webFolderFullPath = webFolderFullPath.Replace("/Accounts", "");
            //DMS04-3777M - used replace for "Secure" too (first letter upper case)
            webFolderFullPath = webFolderFullPath.Replace("/secure", "").Replace("/Secure", "");
            webFolderFullPath = webFolderFullPath.Replace("/Workflow", "");

            return webFolderFullPath;
        }


        #endregion

        protected override void InitializeCulture()
        {
            string strCurrentLanguage = string.Empty;
            if (Session["LanguageCode"] == null)
            {
                strCurrentLanguage = "en-US";
            }
            else
            {
                strCurrentLanguage = Session["LanguageCode"].ToString();
            }
            if (strCurrentLanguage.Trim() == "")
            {
                strCurrentLanguage = "en-US";
            }

            CultureInfo Cul = CultureInfo.CreateSpecificCulture(strCurrentLanguage);
            System.Threading.Thread.CurrentThread.CurrentUICulture = Cul;
            System.Threading.Thread.CurrentThread.CurrentCulture = Cul;

            base.InitializeCulture();

        }

        protected bool VersionManagement(int DocumentId, string ArchiveRemarks)
        {
            bool isval = true;
            try
            {
                Logger.Trace("Versionmanagement started ", Session["LoggedUserId"].ToString());

                UserBase loginUser = (UserBase)Session["LoggedUser"];
                SearchFilter filter = new SearchFilter();
                DataSet data = new DataSet();

                string Versions = string.Empty;
                string TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
                string action = "GetUploadDocumentDetailsForDownload";
                string filePath = string.Empty;
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                string VersionFolder = System.Configuration.ConfigurationManager.AppSettings["VersionFolder"] + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\";
                if (!Directory.Exists(VersionFolder))
                {
                    Directory.CreateDirectory(VersionFolder);
                }
                //To get the version of document
                filter.UploadDocID = DocumentId;
                data = new UploadDocBL().GetUploadDocumentDetails(filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                Versions = data.Tables[0].Rows[0]["DocVersion"].ToString();
                string FilePathToArchive = data.Tables[0].Rows[0]["phyFilepath"].ToString();
                string extension = afterdot(FilePathToArchive);
                if (File.Exists(beforedot(FilePathToArchive) + ".zip"))
                {
                    File.Copy(beforedot(FilePathToArchive) + ".zip", VersionFolder + beforedot(data.Tables[0].Rows[0]["EncrDocName"].ToString()) + "_" + "Version " + Versions + ".zip", true);
                    filePath = VersionFolder + beforedot(data.Tables[0].Rows[0]["EncrDocName"].ToString()) + "_" + "Version " + Versions + ".zip";

                }

                new UploadDocBL().UpdateDocumentDetails(filter.UploadDocID, filePath, loginUser.LoginOrgId.ToString(), loginUser.LoginToken, ArchiveRemarks);
                Logger.Trace("Versionmanagement finished ", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                isval = false;
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return isval;
        }
    }

}
