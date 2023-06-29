/* ============================================================================  
Author     : Joby
Create date: 
Description: New Organization Creation
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                  Description:  
** ----------   -------------       ----------           ----------------------------
** 17 APR 13    Pratheesh A         Writer UTC Bug  01   On validation contactPerson.value.length was present, only contactPerson.length required
 * 16 Apr 15    Sabina              DMS5-3929 	         Adding a new checkbox list control inside manage org page
 * 16 Apr 15    Sabina              DMS5-3928	         Creating a new table for mapping Organization and Application
 * 19 Apr 2015  Yogeesha            DMS5-3952            Customer can be created without selecting the application he has access to.
=============================================================================== */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.IO;
using WorkflowBAL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class ManageOrg : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            //DMS5-3929A to bind the Checkbox list for Application 
            BindApplication();
            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;

                if (Request.QueryString["action"] == null)
                {
                    LogOutAndRedirectionWithErrorMessge(string.Empty);
                }
                else
                {
                    
                    string action = Request.QueryString["action"].ToString();
                    txtCustomerName.Attributes.Add("onkeyup", "enable();");
                    txtOrgEmail.Attributes.Add("onkeyup", "enable();");
                    if (action.ToLower() == "add")
                    {
                        
                        hdnAction.Value = "AddOrg";
                        lblHeading.Text = "Add New Customer";
                        hdnCurrentOrgId.Value = "0";
                        btnsearchagain.Visible = false;
                        lblChangeLogo.Visible = false;
                    }
                    else if (action.ToLower() == "edit")
                    {

                        lblLogo.Visible = false;
                        
                        if (Request.QueryString["id"] == null)
                        {
                            LogOutAndRedirectionWithErrorMessge(string.Empty);
                        }
                        else
                        {
                            hdnCurrentOrgId.Value = Request.QueryString["id"].ToString();
                            hdnAction.Value = "EditOrg";
                            lblHeading.Text = "Edit Customer";
                            //DMS5-3929M add new code inside the function for editing the application details
                            GetCustomerWithID(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                        }

                    }
                    string pageRights = GetPageRights();
                    if (pageRights == string.Empty && loginUser.UserName == "administrator")
                    {
                        pageRights = "Edit";
                    }
                    hdnPageRights.Value = pageRights;
                    ApplyPageRights(pageRights, this.Form.Controls);
                }
            }

        }

        public void GetCustomerWithID(string loginOrgId, string loginToken)
        {
            SecurityBL bl = new SecurityBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                filter.CurrOrgId = Convert.ToInt32(hdnCurrentOrgId.Value);

                Results res = bl.GetOrgById(filter, hdnAction.Value, loginOrgId, loginToken);
                if (res.ActionStatus == "SUCCESS" && res.Orgs != null)
                {
                    Org org = res.Orgs[0];
                    txtCustomerName.Text = org.OrgName;
                    txtAddress.Text = org.OrgAddress;
                    txtOrgEmail.Text = org.OrgEmailId;
                    txtPhoneNo.Text = org.PhoneNo;
                    txtFaxNo.Text = org.FaxNo;
                    txtContactPerson.Text = org.ContactPerson;
                    txtContactMobile.Text = org.ContactMobile;
                    imgCustLogo.Src = org.LogoPath + "#" + DateTime.Now.ToString("HH:mm:ss");
                    txtOrgGreeting.Text = org.OrgGreeting;
                    txtOrgDetails.Text = org.OrgDetails;
                }
                //DMS5-3929BS added to edit the application details
                Results rs = bl.GetOrgById(filter, "GetOrgApplication", loginOrgId, loginToken);
                if (rs != null && rs.ResultDS != null && rs.ResultDS.Tables.Count > 0 && rs.ResultDS.Tables[0].Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in rs.ResultDS.Tables[0].Rows)
                        cblApplication.Items.FindByValue(dr["OrganizationApplication_iApplicationId"].ToString()).Selected= true;
                }
                //DMS5-3929BE
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR", ex.Message.ToString());
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string strFieldValue = string.Empty;
            try
            {
                Logger.Trace("btnSubmit_Click Started", "TraceStatus");
                string orgCredentials = hdnCredentials.Value;
                string orglink = hdnOrglink.Value;
                divMsg.InnerHtml = string.Empty;
                Results res = new Results();
                string action = "AddOrgSentMail";

                string[] credentialParts = orgCredentials.Split('|');

                if (credentialParts.Length == 2)
                {
                    User user = new User();
                    user.UserName = credentialParts[0];
                    user.Password = credentialParts[1];

                    user.FirstName = "Administrator";
                    user.LastName = string.Empty;
                    user.OrgEmailId = System.Configuration.ConfigurationManager.AppSettings["SuperAdminEmail"].ToString();
                    user.EmailId = txtOrgEmail.Text;
                    user.LoginWebsiteUrl = orglink;
                    Logger.Trace("Cheching file filelength " + hdnLogoFileName.Value.Trim().Length.ToString(), user.UserName);
                    if (hdnLogoFileName.Value.Trim().Length > 0)
                    {
                        string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"] + "\\" + "Logo" + "\\";
                        Logger.Trace("Cheching file exists in app root " + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), user.UserName);
                        if (File.Exists(Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value))))
                        {
                            Logger.Trace("Deleting " + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), user.UserName);
                            File.Delete(Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)));
                            Logger.Trace("Deleted from root" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), user.UserName);
                        }
                        Logger.Trace("Move Started from root" + TempFolder + "\\" + Session["LoggedUserId"].ToString() + "\\" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value) + Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)), user.UserName);
                        File.Move(TempFolder + "\\" + Session["LoggedUserId"].ToString() + "\\" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)));

                        Logger.Trace("checking tempfolder if the file exist not " + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), user.UserName);
                        if (File.Exists(TempFolder + Session["LoggedUserId"].ToString() + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)))
                        {
                            Logger.Trace("exists file going to be deleted from temp folder" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), user.UserName);
                            File.Delete(TempFolder + Session["LoggedUserId"].ToString() + txtCustomerName.Text + afterdot(hdnLogoFileName.Value));
                            Logger.Trace("exists file deleted from temp folder" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), user.UserName);
                        }

                    }

                    if (SendMessage(user, action))
                    {
                        res.ActionStatus = "SUCCESS";
                    }
                    else
                    {
                        res.ActionStatus = "MAILFAILED";
                    }
                    if (res.ActionStatus == "SUCCESS")
                    {
                        txtCustomerName.Text = "";
                        txtAddress.Text = "";
                        txtOrgEmail.Text = "";
                        txtPhoneNo.Text = "";
                        txtFaxNo.Text = "";
                        txtContactPerson.Text = "";
                        txtContactMobile.Text = "";
                        txtOrgGreeting.Text = "";
                        txtOrgDetails.Text = "";
                        divMsg.Style.Add("color", "green");
                        divMsg.InnerHtml = CoreMessages.GetMessages(action, res.ActionStatus) + user.EmailId;
                    }
                    else
                    {
                        divMsg.InnerHtml = CoreMessages.GetMessages(action, res.ActionStatus);
                    }

                }
                else
                {
                    Logger.Trace("Cheching file filelength in else " + hdnLogoFileName.Value.Trim().Length.ToString(), Session["LoggedUserId"].ToString());
                    if (hdnLogoFileName.Value.Trim().Length > 0)
                    {
                        string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"] + "\\" + "Logo" + "\\";
                        Logger.Trace("Cheching file exists in app root " + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), Session["LoggedUserId"].ToString());
                        if (File.Exists(Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value))))
                        {
                            Logger.Trace("Deleting " + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), Session["LoggedUserId"].ToString());
                            File.Delete(Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)));
                            Logger.Trace("Deleted from root" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), Session["LoggedUserId"].ToString());
                        }
                        Logger.Trace("Move Started from root" + TempFolder + "\\" + Session["LoggedUserId"].ToString() + "\\" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value) + Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)), Session["LoggedUserId"].ToString());
                        File.Move(TempFolder + Session["LoggedUserId"].ToString() + "\\" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)));

                        Logger.Trace("checking tempfolder if the file exist not " + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), Session["LoggedUserId"].ToString());
                        if (File.Exists(TempFolder + Session["LoggedUserId"].ToString() + "\\" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)))
                        {
                            Logger.Trace("exists file going to be deleted from temp folder" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), Session["LoggedUserId"].ToString());
                            File.Delete(TempFolder + Session["LoggedUserId"].ToString() + "\\" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value));
                            Logger.Trace("exists file deleted from temp folder" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), Session["LoggedUserId"].ToString());
                        }

                    }
                    divMsg.Style.Add("color", "green");
                    divMsg.InnerHtml = "Customer details updated successfully.";
                }
                Logger.Trace("btnSubmit_Click Finished", "TraceStatus");
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog("function btnSubmit_Click caught ex;" + ex.Message.ToString());
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            if (loginUser.LoginParentOrgId == 0)
            {
                Response.Redirect("~/Secure/Core/SearchOrg.aspx");
            }
            else
            {
                Response.Redirect("~/Secure/Home.aspx");
            }
        }

        protected void btnsearchagain_Click(object sender, EventArgs e)
        {

            string SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
            Response.Redirect("SearchOrg.aspx?Search=" + SearchCriteria);

        }

        protected void AsyncFULogo_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            try
            {
                Logger.Trace("AsyncFULogo_UploadedComplete Started", "TraceStatus");

                if (AsyncFULogo.HasFile)
                {
                    if (Convert.ToInt32(e.FileSize.ToString()) > 1048567)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "TestAlert", "alert('" + "Image has to be less than 1MB in size" + "');", true);
                    }
                    System.Drawing.Image postedImage = System.Drawing.Image.FromStream(this.AsyncFULogo.PostedFile.InputStream);
                    if (postedImage.Width != 255 || postedImage.Height != 86)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "TestAlert", "alert('" + "Image pixel error: Height should be 86px and width should be 255 px" + "');", true);
                    }
                    else
                    {
                        string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"] + "\\" + "Logo" + "\\";

                        if (!Directory.Exists(TempFolder))
                        {
                            Directory.CreateDirectory(TempFolder);
                        }
                        if (!Directory.Exists(TempFolder + Session["LoggedUserId"].ToString()))
                        {
                            Directory.CreateDirectory(TempFolder + Session["LoggedUserId"].ToString());
                        }
                        AsyncFULogo.SaveAs(TempFolder + Session["LoggedUserId"].ToString() + "\\" + txtCustomerName.Text + afterdot(AsyncFULogo.FileName));
                    }
                }

                Logger.Trace("Finished Saving Logo in AsyncFULogo_Uploaded Complete ", "TraceStatus");
            }
            catch (Exception ex)
            {

                Logger.TraceErrorLog("function AsyncFULogo_Uploaded caught ex;" + ex.Message.ToString());
            }
        }


        protected void btnMoveLogo_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnMoveLogo_Click Started", "TraceStatus");
                if (hdnLogoFileName.Value.Trim().Length > 0)
                {
                    string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"] + "\\" + "Logo" + "\\";
                    if (File.Exists(Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value))))
                    {
                        File.Delete(Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)));
                    }

                    File.Move(TempFolder + Session["LoggedUserId"].ToString() + "\\" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value), Server.MapPath("~/Assets/Logo/" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)));

                    if (File.Exists(TempFolder + Session["LoggedUserId"].ToString() + "\\" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value)))
                    {
                        File.Delete(TempFolder + Session["LoggedUserId"].ToString() + "\\" + txtCustomerName.Text + afterdot(hdnLogoFileName.Value));
                    }

                    if (Request.QueryString["action"].ToString().ToLower() == "add")
                    {
                        divMsg.InnerHtml = "Customer added successfully. Please wait.. ";
                    }
                    else
                    {
                        divMsg.InnerHtml = "Customer details updated successfully.";
                    }
                }
                Logger.Trace("btnMoveLogo_Click Finished", "TraceStatus");
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog("function btnMoveLogo_Click caught ex;" + ex.Message.ToString());
            }
        }

        //DMS5-3928BS,DMS5-3929BS
        public void BindApplication()
        {
            Results Objresult = new Results();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            SecurityBL securityBL = new SecurityBL();
            Objresult = securityBL.GetApplication("GetAllApplication", Convert.ToString(loginUser.LoginOrgId), loginUser.LoginToken);

            if (Objresult.ResultDS != null) // DMS5-3952 A
            {
                cblApplication.DataSource = Objresult.ResultDS;
                cblApplication.DataTextField = "Application_vName";
                cblApplication.DataValueField = "Application_iId";
                cblApplication.DataBind();

                //IMPORTANT! Adding attributes to items in check box list as there is not default way to access id from java script
                foreach (ListItem li in cblApplication.Items)
                {
                    li.Attributes.Add("dvalue", li.Value);
                }
            }
        }
        //DMS5-3928BE

    }

}
