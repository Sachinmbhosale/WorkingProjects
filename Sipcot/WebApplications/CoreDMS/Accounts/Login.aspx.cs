/* ============================================================================  
Author     : Joby
Create date: 
Description:  
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                       Description:  
** ----------   -------------       ----------                ----------------------------
16 Nov 2013     Pratheesh A         Enhancement 2 (DUEN)       Adding Domain User Details
18 Nov 2013     Pratheesh A         Wrtier DMS ENHSMT 2-1695   Domain Id check implemeted, Now domain ID sending to DB
16 Apr 2015     Sabina              DMS5-3935                   Login management:redirecting to either workflow home page or DMS based on application access
=============================================================================== */
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Data;
using System.DirectoryServices;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class Login : PageBase
    {
        UserBL BL = new UserBL();
        string st = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {


                Btnlogin.Attributes.Add("onClick", "return EncryptPassword1('" + txtPassword.Text + "');");
                FillCapctha();
                if (Request.QueryString["msg"] != null)
                {
                    divMsg.InnerHtml = Request.QueryString["msg"].ToString();
                    try
                    {
                        FormsAuthentication.SignOut();
                    }
                    catch
                    {

                    }
                    InitialiseSessionVariables();
                    if (Request.QueryString["Org"] == null) //OrgCode
                    {
                        divMsg.InnerHtml = "Please verify the link.";
                    }
                    else
                    {
                        string loginOrgCode = Request.QueryString["Org"].ToString().Trim();
                        hdnLoginOrgCode.Value = loginOrgCode.ToLower();
                        Session["LoginOrgCode"] = hdnLoginOrgCode.Value;
                        LoadDomain();

                    }

                }
                if (Session["LoggedUser"] == null)
                {
                    InitialiseSessionVariables();
                    txtUsername.Focus();
                    if (Request.QueryString["Org"] == null) //OrgCode
                    {
                        divMsg.InnerHtml = "Please verify the link.";
                    }
                    else
                    {
                        string loginOrgCode = Request.QueryString["Org"].ToString().Trim();
                        hdnLoginOrgCode.Value = loginOrgCode.ToLower();
                        Session["LoginOrgCode"] = hdnLoginOrgCode.Value;
                        LoadDomain();
                    }
                }
                //else
                //{
                //    Response.Redirect("~/secure/Home.aspx", false);
                //}
            }

            //Response.Write("Session.SessionID=" + Session.SessionID + "<br/>");
            //Response.Write("Cookie ASP.NET_SessionId=" + Request.Cookies["ASP.NET_SessionId"].Value + "<br/>");
        }
        /// <summary>
        /// To validate login & Get user information for a valid user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (hdnCapta.Value.ToString() == txtCaptcha.Text)
            {
                divMsg.InnerHtml = string.Empty;
                SecurityBL bl = new SecurityBL();
                User loginUser = new User();
                Results rs = new Results();
                try
                {
                    loginUser.UserName = txtUsername.Text.ToUpper();
                    loginUser.LoginOrgId = hdnLoginOrgId.Value.Length > 0 ? Convert.ToInt32(hdnLoginOrgId.Value) : 0;
                    loginUser.LoginToken = hdnLoginToken.Value;
                    if (loginUser.LoginOrgId < 1 || loginUser.LoginToken.Trim() == string.Empty)
                    {
                        divMsg.InnerHtml = CoreMessages.GetMessages("InvalidToken", string.Empty);
                    }

                    if (chkDomainUser.Checked == true && checkDomainuser() == false) // DUEN - Add
                    {
                        chkDomainUser.Checked = true;
                        divMsg.InnerHtml = "Invalid Domain User Name or Password";
                        return;
                    }

                    if (loginUser.LoginOrgId > 0)
                        rs = bl.LoggedUserManagement(ref loginUser, "UserLogin");

                    if (rs.UserData != null && rs.UserData.UserId > 0)
                    {
                        rs.UserData.LoginOrgCode = hdnLoginOrgCode.Value;
                        //newly added for session issue Resolved
                        HttpCookie Orgcode = new HttpCookie("Orgcode");
                        Orgcode.Value = hdnLoginOrgCode.Value;
                        Response.Cookies.Add(Orgcode);
                        //newly added for session issue Resolved
                        Session["LoggedUser"] = rs.UserData;
                        Session["LoggedUserId"] = rs.UserData.UserId;
                        Session["OrgID"] = loginUser.LoginOrgId.ToString();
                        Session["LoggedUserName"] = rs.UserData.UserName;
                        Session["LanguageID"] = rs.UserData.LanguageID.ToString();
                        Session["LanguageCode"] = rs.UserData.LanguageCode.ToString();
                        Session["OutofOffice"] = rs.UserData.OutofOffice.ToString();
                        Session["LoginOrgCode"] = hdnLoginOrgCode.Value;

                        FormsAuthentication.RedirectFromLoginPage(loginUser.UserName, false);

                        if (rs.ActionStatus == "EXPAIRED")
                        {
                            //... check if user logged in 1st time then go for Changed-Pwd else Dashboard.
                            Response.Redirect("~/secure/Core/ChanagePassword.aspx", false);
                            // Response.Redirect("~/secure/Core/Dashboard.aspx", false);
                        }
                        else
                        {
                            // In master page menus will be validated and redirected to DMS / Workflow 
                            Response.Redirect("~/secure/Home.aspx", false);
                        }

                        //if (loginUser.ConfidentialityAgreement)
                        //{
                        //Response.Redirect("~/secure/Home.aspx");
                        //}
                        //else
                        //{
                        //    Response.Redirect("~/secure/User/ConfidentialityAgreement.aspx");
                        //}
                    }
                    else
                    {
                        divMsg.InnerHtml = rs.Message;
                        //added by sabina to focus the txtusername when no value in textbox
                        if (string.IsNullOrEmpty(divMsg.InnerHtml))
                        {
                            txtUsername.Focus();
                        }
                    }

                }
                catch (Exception ex)
                {
                    divMsg.InnerHtml = "An error occurred, please try again. If problem exists please contact Administrator";
                    Logger.TraceErrorLog("Login.aspx - btnSubmit_Click () :" + ex.Message.ToString());
                }
            }
            else
            {
                divMsg.InnerHtml = "Invalid CaptaCode";
                FillCapctha();
            }
        }

        void FillCapctha()
        {
            try
            {
                Random random = new Random();
                string combination = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                StringBuilder captcha = new StringBuilder();
                for (int i = 0; i < 6; i++)
                    captcha.Append(combination[random.Next(combination.Length)]);
                hdnCapta.Value = captcha.ToString();
                Session["captcha"] = captcha.ToString();
                imgCaptcha.ImageUrl = "GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString();
                txtCaptcha.Text = "";
            }
            catch
            {
                throw;
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Close Window", "window.opener = 'x'; window.close();", true);
        }
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            FillCapctha();
        }
        protected void LoadDomain() // DUEN - Add
        {

            try
            {
                Logger.Trace("Started Loading Domain Name Drop Down", "0");

                DataTable dsDomain = BL.GetDomains(0, "GetDomainNamewithOrgCode", "0", "0", Request.QueryString["Org"].ToString().Trim());

                if (dsDomain.Rows.Count > 0)
                {
                    drpDomain.Items.Clear();
                    drpDomain.DataSource = dsDomain;
                    drpDomain.DataTextField = "TextField";
                    drpDomain.DataValueField = "ValueField";
                    drpDomain.DataBind();
                }
                else
                {
                    drpDomain.Items.Clear();
                    drpDomain.Items.Add(new ListItem("--Select--", "0"));
                }

                Logger.Trace("Finished Loading Domain Name Drop Down", "0");

            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.ToString());
            }

        }

        public bool checkDomainuser()// DUEN - Add
        {
            try
            {
                string strEntry = @"LDAP://" + drpDomain.SelectedItem.Text.Trim();
                DirectoryEntry objDirectoryEntry = new DirectoryEntry(strEntry);
                DirectorySearcher objDirectorySearcher = null;
                objDirectoryEntry.Username = txtUsername.Text.Trim();
                objDirectoryEntry.Password = txtPassword.Text.Trim();
                objDirectorySearcher = new DirectorySearcher(objDirectoryEntry);
                objDirectorySearcher.Filter = "sAMAccountName=" + txtUsername.Text.Trim();
                objDirectorySearcher.SearchScope = SearchScope.Subtree;
                objDirectorySearcher.PropertiesToLoad.Add("cn");

                try
                {
                    SearchResult myresult = objDirectorySearcher.FindOne();
                    if (myresult == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex) //Incase of User Name or Password Wrong --Logon failure: unknown user name or bad password.
                {
                    Logger.TraceErrorLog(ex.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.ToString());
                return false;
            }
        }

        protected void btnrefreshcapta_Click(object sender, EventArgs e)
        {

            divMsg.InnerHtml = hdnmessage.Value.ToString();
            FillCapctha();
        }


        protected void checkConcurrentUser(string username)
        {

            string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
            SqlConnection con = new SqlConnection(connectionstring);
            con.Open();
            SqlCommand cmd = new SqlCommand("[dbo].[checkConcurrentUser]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@username", username);


            //cmd.Parameters["@count"].Direction = ParameterDirection.Output;  
            cmd.Parameters.Add("@count", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
            //st = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.ExecuteNonQuery();
            st = Convert.ToString(cmd.Parameters["@count"].Value);
            //if (st != "0")
            //{
            //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "Script1", "ShowConfirm();", true);
            //}
            con.Close();

        }
        //ends here


        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);
            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = (string)settingsReader.GetValue("SecurityKey",
                                                         typeof(String));

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }


        protected void Btnlogin_Click(object sender, EventArgs e)
        {
            if (hdnCapta.Value.ToString() == txtCaptcha.Text)
            {

                if (txtPassword.Text != string.Empty)
                {
                    checkConcurrentUser(txtUsername.Text.Trim());

                    if (st == "0")
                    {
                        divMsg.InnerHtml = string.Empty;
                        SecurityBL bl = new SecurityBL();
                        User loginUser = new User();
                        Results rs = new Results();



                        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

                        byte[] hashedBytes = null;
                        UTF8Encoding encoder = new UTF8Encoding();

                        hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(txtPassword.Text));



                        try
                        {

                            string connstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
                            SqlConnection conn = new SqlConnection(connstring);
                            conn.Open();
                            SqlCommand cmd = new SqlCommand("[dbo].[USP_ValidateUser]", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@in_vUserName", txtUsername.Text);
                            cmd.Parameters.AddWithValue("@in_bDomainUser", 0);
                            cmd.Parameters.AddWithValue("@in_iDomainID", 0);
                            cmd.Parameters.AddWithValue("@in_vPassword", txtPassword.Text);
                            cmd.Parameters.AddWithValue("@in_vLoginOrgCode", hdnLoginOrgCode.Value);
                            cmd.Parameters.AddWithValue("@in_vLoginToken", System.Guid.NewGuid().ToString());
                            cmd.Parameters.AddWithValue("@in_vAction", "ValidateUser");
                            SqlDataReader rdr1 = cmd.ExecuteReader();
                            while (rdr1.Read())
                            {
                                string ActionStatus = rdr1["UserStatus"].ToString();

                                if (ActionStatus == "SUCCESS" || ActionStatus == "EXPAIRED")
                                {
                                    UserBase UserData = new UserBase();

                                    UserData.UserId = Convert.ToInt32(rdr1["UserId"].ToString());
                                    UserData.LoginOrgId = Convert.ToInt32(rdr1["LoginOrgId"].ToString());
                                    hdnLoginOrgId.Value = Convert.ToString(rdr1["LoginOrgId"]);
                                    UserData.LoginToken = rdr1["LoginToken"].ToString();
                                    hdnLoginToken.Value = rdr1["LoginToken"].ToString();

                                    try
                                    {
                                        loginUser.UserName = txtUsername.Text.ToUpper();
                                        loginUser.LoginOrgId = hdnLoginOrgId.Value.Length > 0 ? Convert.ToInt32(hdnLoginOrgId.Value) : 0;
                                        loginUser.LoginToken = hdnLoginToken.Value;
                                        if (loginUser.LoginOrgId < 1 || loginUser.LoginToken.Trim() == string.Empty)
                                        {
                                            divMsg.InnerHtml = CoreMessages.GetMessages("InvalidToken", string.Empty);
                                        }

                                        if (chkDomainUser.Checked == true && checkDomainuser() == false) // DUEN - Add
                                        {
                                            chkDomainUser.Checked = true;
                                            divMsg.InnerHtml = "Invalid Domain User Name or Password";
                                            return;
                                        }

                                        if (loginUser.LoginOrgId > 0)
                                            rs = bl.LoggedUserManagement(ref loginUser, "UserLogin");

                                        if (rs.UserData != null && rs.UserData.UserId > 0)
                                        {
                                            rs.UserData.LoginOrgCode = hdnLoginOrgCode.Value;
                                            //newly added for session issue Resolved
                                            HttpCookie Orgcode = new HttpCookie("Orgcode");
                                            Orgcode.Value = hdnLoginOrgCode.Value;
                                            Response.Cookies.Add(Orgcode);
                                            //newly added for session issue Resolved
                                            Session["LoggedUser"] = rs.UserData;
                                            Session["LoggedUserId"] = rs.UserData.UserId;
                                            Session["OrgID"] = loginUser.LoginOrgId.ToString();
                                            Session["LoggedUserName"] = rs.UserData.UserName;
                                            Session["LanguageID"] = rs.UserData.LanguageID.ToString();
                                            Session["LanguageCode"] = rs.UserData.LanguageCode.ToString();
                                            Session["OutofOffice"] = rs.UserData.OutofOffice.ToString();
                                            Session["LoginOrgCode"] = hdnLoginOrgCode.Value;
                                            Session["GroupName"] = rs.UserData.GroupName;
                                            Session["RoleID"] = rs.UserData.GroupId;

                                            FormsAuthentication.RedirectFromLoginPage(loginUser.UserName, false);
                                            if (rs.ActionStatus == "EXPAIRED")
                                            {
                                                Response.Redirect("~/secure/Core/ViewDashboard.aspx?Login=" + "Load", false);
                                                //Response.Redirect("~/secure/Core/DocumentDownloadSearch.aspx?Login=" + "Load", false);
                                            }
                                            else
                                            {
                                               Response.Redirect("~/secure/Core/ViewDashboard.aspx?Login=" + "Load", false);
                                                //Response.Redirect("~/secure/Core/DocumentDownloadSearch.aspx?Login=" + "Load", false);
                                            }

                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(divMsg.InnerHtml))
                                            {
                                                txtUsername.Focus();
                                            }
                                            divMsg.InnerHtml = rs.Message;                                          
                                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error", "alert('Wrong userid or Password...!' );", true);
                                            return;
                                          
                                        }

                                    }
                                    catch (Exception ex)
                                    {

                                        divMsg.InnerHtml = "An error occurred, please try again. If problem exists please contact Administrator";
                                        Logger.TraceErrorLog("Login.aspx - btnSubmit_Click () :" + ex.Message.ToString());
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error", "alert('An error occurred, please try again. If problem exists please contact Administrator ' );", true);
                                        return;

                                    }

                                }
                                else
                                {
                                    divMsg.InnerHtml = ActionStatus;
                                    if (ActionStatus == "LOCKED")
                                    {
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error", "alert('User is LOCKED' );", true);
                                        return;
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error", "alert('An error occurred, please try again. If problem exists please contact Administrator ' );", true);
                                        return;

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            divMsg.InnerHtml = "An error occurred, please try again. If problem exists please contact Administrator";
                            Logger.TraceErrorLog("Login.aspx - btnSubmit_Click () :" + ex.Message.ToString());
                        }
                        finally
                        {
                        }
                    }
                    else
                    {
                        divMsg.InnerHtml = "User already logged on.try logging in after closing the current !";
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Error", "alert('User already logged on.try logging in after closing the current !');", true);
                        return;
                    }
                 }
                else
                {
                    divMsg.InnerHtml = "Please enter password..!";
                }
            }
            else
            {
                divMsg.InnerHtml = "Invalid CaptaCode";
                FillCapctha();

            }
        }

    }


}
