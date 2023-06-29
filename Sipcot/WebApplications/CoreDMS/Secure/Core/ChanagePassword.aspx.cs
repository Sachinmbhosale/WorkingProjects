/* ============================================================================  
Author     : Joby
Create date: 
Description:  
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                       Description:  
** ----------   -------------       ----------                ----------------------------

 * 17 Apr 2015     Gokuldas.p              DMS5-3935          Login management:redirecting to either workflow home page or DMS based on application access
 * 28 Apr 2015     Yogeesha Naik           DMS5-3935          Re written dynamic master page swapping code
=============================================================================== */

using System;
using System.Configuration;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class ChangePassword : PageBase
    {
        /* DMS5-3935 BS */
        protected void Page_PreInit(object sender, EventArgs e)
        {
            /* setting master page */
            ChangeMasterPage(MasterPage);
        }

        protected void ChangeMasterPage(string masterPage)
        {
            if (masterPage.Length > 0)
                if (!masterPage.Substring(masterPage.LastIndexOf("/")).Equals(this.Page.MasterPageFile.Substring(this.Page.MasterPageFile.LastIndexOf("/"))))
                    MasterPageFile = masterPage;
        }
        // DMS5-3935 BE

        protected void Page_Load(object sender, EventArgs e)
        {

            string pass = txtNewPassword.Text;

           // hdnpass.Value = pass;


            
                btnSubmit.Attributes.Add("onClick", "return EncryptPassword1('" + txtNewPassword.Text + "');");


            //btnSubmit.Attributes.Add("onClick", "return EncryptPassword1('" + txtNewPassword.Text + "');");
            // txtOldPassword.Attributes.Add("type", "password");
            txtOldPassword.Attributes.Add("autocomplete", "off");
            // txtNewPassword.Attributes.Add("type", "password");
            //  txtRetryNewPassord.Attributes.Add("type", "password");

            CheckAuthentication();

            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                if (loginUser.IsDomainUser == true)
                {
                    Response.Redirect("~/Secure/Home.aspx");
                }
                hdnUserName.Value = loginUser.UserName;
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;

                //txtOldPassword.Text = string.Empty;
                //txtOldPassword.Focus();
            }
        }



        public void Btnlogin_Click(object sender, EventArgs e)
        {
            //var remark = "";
            
            string psw = txtRetryNewPassord.Text;

            
            //btnSubmit.Attributes.Add("onClick", "return EncryptPassword1('" + txtOldPassword.Text + "');");

            //var regularExpression = @"^(?=.*[0 - 9])(?=.*[!@#$%^&_)(*.,><~?])[a-zA-Z0-9!@#$%^&_)(*.,><~?]{8,10}$";
            var regularExpression = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$";
            //[a-zA - Z]{ 1,})([@$!% *#?&]{1,})([0-9]{1,}


            var matchVendorcode = Regex.IsMatch(psw, regularExpression);



            bool validpasslenth = true;
            if (psw.Length < 8 || psw.Length > 10)
            {

                validpasslenth = false;

            }




            //if (txtNewPassword.Text != txtRetryNewPassord.Text)
            //{
            //    remark = "Password does not match";

            //}





            if (validpasslenth == true)

            {
                if (matchVendorcode == true)
                {



                    //New Password should contain atleast one number and one special character


                    if (txtOldPassword.Text != string.Empty && txtNewPassword.Text != string.Empty && txtRetryNewPassord.Text != string.Empty)
                    {



                        divMsg.InnerHtml = string.Empty;
                        divmsgsuccess.InnerHtml = string.Empty;
                        SecurityBL bl = new SecurityBL();
                        User loginUser = new User();
                        Results rs = new Results();


                        try
                        {



                            string connstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
                            SqlConnection conn = new SqlConnection(connstring);
                            conn.Open();


                            // string hashedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(txtNewPassword.Text, "MD5");


                            var txtnewpass = psw;

                            SqlCommand cmd = new SqlCommand("[dbo].[USP_LoginManagement]", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@in_vUserName", hdnUserName.Value);
                            cmd.Parameters.AddWithValue("@in_vPassword", txtOldPassword.Text);
                            cmd.Parameters.AddWithValue("@in_vNewPassword", txtNewPassword.Text);
                            cmd.Parameters.AddWithValue("@in_vLoginToken", hdnLoginToken.Value);
                            cmd.Parameters.AddWithValue("@in_iLoginOrgId", hdnLoginOrgId.Value);
                            //cmd.Parameters.AddWithValue("@in_bAcceptConfidentailtyAgreement", loginUser.ConfidentialityAgreement);
                            cmd.Parameters.AddWithValue("@in_vAction", "ChangePassword");
                            SqlDataReader rdr1 = cmd.ExecuteReader();
                            while (rdr1.Read())
                            {
                                string ActionStatus = rdr1["UserStatus"].ToString();

                                if (ActionStatus == "SUCCESS" || ActionStatus == "EXPAIRED")
                                {




                                    divmsgsuccess.InnerHtml = "Password Updated Successfully";



                                }
                                else
                                {
                                    divMsg.InnerHtml = "Password not Updated pls check your current password";


                                }

                            }


                        }
                        catch (Exception ex)
                        {
                            divMsg.InnerHtml = "Error please contact your administrator";
                            //Logger.TraceErrorLog("Login.aspx - btnSubmit_Click () :" + ex.Message.ToString());
                        }
                        finally
                        {

                        }



                    }


                    else
                    {
                        divMsg.InnerHtml = "Field cannot be blank..!";
                    }


                }




                else
                {

                    divMsg.InnerHtml = "New Password should contain atleast one number and one special character..!";

                }
            }


            else
            {

                divMsg.InnerHtml = "New Password should be 8 - 10 characters long.Please re - enter the password..!";

            }

        }




        //protected void Btnlogin_Click(object sender, EventArgs e)
        //{

        //    string psw = TextBox1.Text;
        //    if (txtOldPassword.Text != string.Empty && txtNewPassword.Text != string.Empty && txtRetryNewPassord.Text != string.Empty)
        //    {



        //        divMsg.InnerHtml = string.Empty;
        //        divmsgsuccess.InnerHtml = string.Empty;
        //        SecurityBL bl = new SecurityBL();
        //        User loginUser = new User();
        //        Results rs = new Results();


        //        try
        //        {

        //            string connstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
        //            SqlConnection conn = new SqlConnection(connstring);
        //            conn.Open();
        //            SqlCommand cmd = new SqlCommand("[dbo].[USP_LoginManagement]", conn);

        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@in_vUserName", hdnUserName.Value);
        //            cmd.Parameters.AddWithValue("@in_vPassword", txtOldPassword.Text);
        //            cmd.Parameters.AddWithValue("@in_vNewPassword", txtNewPassword.Text);
        //            cmd.Parameters.AddWithValue("@in_vLoginToken", hdnLoginToken.Value);
        //            cmd.Parameters.AddWithValue("@in_iLoginOrgId", hdnLoginOrgId.Value);
        //            //cmd.Parameters.AddWithValue("@in_bAcceptConfidentailtyAgreement", loginUser.ConfidentialityAgreement);
        //            cmd.Parameters.AddWithValue("@in_vAction", "ChangePassword");
        //            SqlDataReader rdr1 = cmd.ExecuteReader();
        //            while (rdr1.Read())
        //            {
        //                string ActionStatus = rdr1["UserStatus"].ToString();

        //                if (ActionStatus == "SUCCESS" || ActionStatus == "EXPAIRED")
        //                {




        //                    divmsgsuccess.InnerHtml = "Password Updated Successfully";



        //                }
        //                else
        //                {
        //                    divMsg.InnerHtml = "Password not Updated pls check your current password";


        //                }

        //            }


        //        }
        //        catch (Exception ex)
        //        {
        //            divMsg.InnerHtml = "Error please contact your administrator";
        //            //Logger.TraceErrorLog("Login.aspx - btnSubmit_Click () :" + ex.Message.ToString());
        //        }
        //        finally
        //        {

        //        }



        //    }
        //    else
        //    {
        //        divMsg.InnerHtml = "Please enter password..!";
        //    }
        //}





        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // In master page menus will be validated and redirected to DMS / Workflow 
            Response.Redirect("~/secure/Home.aspx", false);
        }
    }
}