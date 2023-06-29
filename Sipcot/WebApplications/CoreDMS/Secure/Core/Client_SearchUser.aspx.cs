/* ============================================================================  
Author     : 
Create date: 
Description: New Organization Creation
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                  Description:  
** ----------   -------------       ----------           ----------------------------
 * 19 Apr 15    Yogeesha Naik       DMS5-3935           Change Master page dynamically
=============================================================================== */
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class Client_SearchUser : PageBase
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

        /* DMS5-3935 BE*/
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";


            string pageRights = GetPageRights();
            hdnPageRights.Value = pageRights;
            ApplyPageRights(pageRights, this.Form.Controls);
            txtUserName.Focus();
            if (!IsPostBack)
            {
                BindGrid();
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {

            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
            SqlConnection con = new SqlConnection(connectionstring);
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_SearchUsers", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.Add(new SqlParameter("@in_vUserName", txtUserName.Text.Trim()));
            cmd.Parameters.Add(new SqlParameter("@in_vUserEmailId", txtUserEmailId.Text.Trim()));
            cmd.Parameters.Add(new SqlParameter("@in_vFirstName", txtFirstName.Text.Trim()));
            cmd.Parameters.Add(new SqlParameter("@in_vLastName", txtLastName.Text.Trim()));
            cmd.Parameters.Add(new SqlParameter("@in_iLoginOrgId", Convert.ToInt32(hdnLoginOrgId.Value)));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Session["dtSearcData"] = ds.Tables[0];
                        GridView1.DataSource = ds;
                        GridView1.DataBind();
                    }
                }
            }
            con.Close();

        }
        protected void BindGrid()
        {

            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
            SqlConnection con = new SqlConnection(connectionstring);
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_SearchUsers", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.Add(new SqlParameter("@in_vUserName", txtUserName.Text.Trim()));
            cmd.Parameters.Add(new SqlParameter("@in_vUserEmailId", txtUserEmailId.Text.Trim()));
            cmd.Parameters.Add(new SqlParameter("@in_vFirstName", txtFirstName.Text.Trim()));
            cmd.Parameters.Add(new SqlParameter("@in_vLastName", txtLastName.Text.Trim()));
            cmd.Parameters.Add(new SqlParameter("@in_iLoginOrgId", Convert.ToInt32(hdnLoginOrgId.Value)));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Session["dtSearcData"] = ds.Tables[0];
                        GridView1.DataSource = ds;
                        GridView1.DataBind();
                    }
                }
            }
            con.Close();

        }



        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridView1.PageIndex = e.NewPageIndex;
                //To retrieve DataTable from Session:
                DataTable dt = (DataTable)Session["dtSearcData"];

                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            catch
            {
            }
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            //if (e.CommandName == "ActiveUser")
            //{

            //int id = int.Parse(e.CommandArgument.ToString());
            ////GridViewRow row = grdValidations.Rows[];

            //HiddenField hdnCurrentUserId = (HiddenField)GridView1.Rows[id].FindControl("hidUserid");
            //CheckBox chkActives = (CheckBox)GridView1.Rows[id].FindControl("chkActives");
            //int strcheck;
            //if (chkActives.Checked == true)
            //{
            //    strcheck = 1;
            //}
            //else
            //{
            //    strcheck = 0;
            //}
            //UserBase loginUser = (UserBase)Session["LoggedUser"];

            //Results results = new Results();
            //results.ActionStatus = "SUCCESS";
            //string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
            //SqlConnection con = new SqlConnection(connectionstring);
            //con.Open();
            //SqlCommand cmd = new SqlCommand("USP_ActiveInactiveUser", con);
            //cmd.CommandType = CommandType.StoredProcedure;
            //SqlDataAdapter sda = new SqlDataAdapter(cmd);
            //cmd.Parameters.Add(new SqlParameter("@in_iCurrUserId", Convert.ToInt32(hdnCurrentUserId.Value)));
            //cmd.Parameters.Add(new SqlParameter("@iActive", strcheck));
            //cmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;

            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataSet ds = new DataSet();
            //da.Fill(ds);
            //results.Message = cmd.Parameters["@SP_MESSAGE"].Value.ToString();
            //if (results.Message == "Data added successfully")
            //{
            //    BindGrid();
            //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ValidationMessage", "alert('Action completed successfully.');", true);
            //    return;
            //}


            //}

            if (e.CommandName == "Select")
            {

                int id = int.Parse(e.CommandArgument.ToString());
                HiddenField hidUserid = (HiddenField)GridView1.Rows[id].FindControl("hidUserid");
                HiddenField HidOrgId = (HiddenField)GridView1.Rows[id].FindControl("HidOrgId");

                //Fetch value of Country
                string UserId = hidUserid.Value;
                string OrgId = HidOrgId.Value;

                string URL = "?id=" + UserId + "&action=edit";
                Response.Redirect("~/secure/Core/Client_ManageUser.aspx" + URL, false);
                //Response.Redirect("~/secure/Core/UpdateUserdetails.aspx" + URL, false);
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void chkActives_CheckedChanged(object sender, EventArgs e)
        {


            foreach (GridViewRow row in GridView1.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkRow = (row.Cells[0].FindControl("chkActives") as CheckBox);
                    HiddenField hdnCurrentUser = (HiddenField)row.FindControl("hidUserid");

                    if (chkRow.Checked)
                    {
                        int strcheck = 1;
                        UserBase loginUser = (UserBase)Session["LoggedUser"];

                        Results results = new Results();
                        results.ActionStatus = "SUCCESS";
                        string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
                        SqlConnection con = new SqlConnection(connectionstring);
                        con.Open();
                        SqlCommand cmd = new SqlCommand("USP_ActiveInactiveUser", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        cmd.Parameters.Add(new SqlParameter("@in_iCurrUserId", Convert.ToInt32(hdnCurrentUser.Value)));
                        cmd.Parameters.Add(new SqlParameter("@iActive", strcheck));
                        cmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        results.Message = cmd.Parameters["@SP_MESSAGE"].Value.ToString();
                        //if (results.Message == "Data added successfully")
                        //{
                        //  ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ValidationMessage", "alert('Action completed successfully.');", true);

                        //}

                    }
                    else
                    {
                        int strchk = 0;
                        UserBase loginUser = (UserBase)Session["LoggedUser"];

                        Results results = new Results();
                        results.ActionStatus = "SUCCESS";
                        string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
                        SqlConnection con = new SqlConnection(connectionstring);
                        con.Open();
                        SqlCommand cmd = new SqlCommand("USP_ActiveInactiveUser", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        cmd.Parameters.Add(new SqlParameter("@in_iCurrUserId", Convert.ToInt32(hdnCurrentUser.Value)));
                        cmd.Parameters.Add(new SqlParameter("@iActive", strchk));
                        cmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        results.Message = cmd.Parameters["@SP_MESSAGE"].Value.ToString();
                        //if (results.Message == "Data added successfully")
                        //{
                        //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ValidationMessage", "alert('Action completed successfully.');", true);

                        //}
                    }
                }
            }

            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ValidationMessage", "alert('Action completed successfully.');", true);


        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ReBindgrid();
            txtUserName.Text = string.Empty;
            txtUserEmailId.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
        }

        protected void ReBindgrid()
        {


            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
            SqlConnection con = new SqlConnection(connectionstring);
            con.Open();
            SqlCommand cmd = new SqlCommand("USP_SearchUsers", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.Add(new SqlParameter("@in_vUserName", ""));
            cmd.Parameters.Add(new SqlParameter("@in_vUserEmailId", ""));
            cmd.Parameters.Add(new SqlParameter("@in_vFirstName", ""));
            cmd.Parameters.Add(new SqlParameter("@in_vLastName", ""));
            cmd.Parameters.Add(new SqlParameter("@in_iLoginOrgId", Convert.ToInt32(hdnLoginOrgId.Value)));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Session["dtSearcData"] = ds.Tables[0];
                        GridView1.DataSource = ds;
                        GridView1.DataBind();
                    }
                }
            }
            con.Close();
        }
        //    int strcheck;
        //    foreach (GridViewRow row in GridView1.Rows)
        //    {


        //        if (row.RowType == DataControlRowType.DataRow)
        //        {
        //            CheckBox chkActives = (CheckBox)row.FindControl("chkActives");
        //            HiddenField hdnCurrentUser = (HiddenField)row.FindControl("hidUserid");


        //            if (chkActives.Checked == true)
        //            {

        //                strcheck = 1;
        //                UserBase loginUser = (UserBase)Session["LoggedUser"];

        //                Results results = new Results();
        //                results.ActionStatus = "SUCCESS";
        //                string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
        //                SqlConnection con = new SqlConnection(connectionstring);
        //                con.Open();
        //                SqlCommand cmd = new SqlCommand("USP_ActiveInactiveUser", con);
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                SqlDataAdapter sda = new SqlDataAdapter(cmd);
        //                cmd.Parameters.Add(new SqlParameter("@in_iCurrUserId", Convert.ToInt32(hdnCurrentUser.Value)));
        //                cmd.Parameters.Add(new SqlParameter("@iActive", strcheck));
        //                cmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;

        //                SqlDataAdapter da = new SqlDataAdapter(cmd);
        //                DataSet ds = new DataSet();
        //                da.Fill(ds);
        //                results.Message = cmd.Parameters["@SP_MESSAGE"].Value.ToString();
        //                if (results.Message == "Data added successfully")
        //                {
        //                    //   BindGrid();
        //                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ValidationMessage", "alert('Action completed successfully.');", true);
        //                    return;
        //                }

        //            }
        //            else
        //            {
        //                strcheck = 0;
        //                UserBase loginUser = (UserBase)Session["LoggedUser"];

        //                Results results = new Results();
        //                results.ActionStatus = "SUCCESS";
        //                string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
        //                SqlConnection con = new SqlConnection(connectionstring);
        //                con.Open();
        //                SqlCommand cmd = new SqlCommand("USP_ActiveInactiveUser", con);
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                SqlDataAdapter sda = new SqlDataAdapter(cmd);
        //                cmd.Parameters.Add(new SqlParameter("@in_iCurrUserId", Convert.ToInt32(hdnCurrentUser.Value)));
        //                cmd.Parameters.Add(new SqlParameter("@iActive", strcheck));
        //                cmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;

        //                SqlDataAdapter da = new SqlDataAdapter(cmd);
        //                DataSet ds = new DataSet();
        //                da.Fill(ds);
        //                results.Message = cmd.Parameters["@SP_MESSAGE"].Value.ToString();
        //                if (results.Message == "Data added successfully")
        //                {
        //                    BindGrid();
        //                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ValidationMessage", "alert('Action completed successfully.');", true);
        //                    return;
        //                }


        //            }
        //        }
        //    }
        //}
        //--------------------


        //string data = "";
        ////int strcheck;
        //foreach (GridViewRow row in GridView1.Rows)
        //{
        //if (row.RowType == DataControlRowType.DataRow)
        //{
        //    CheckBox chkRow = (row.Cells[0].FindControl("chkActives") as CheckBox);
        //    if (chkRow.Checked)
        //    {


        //        HiddenField hdnCurrentUser = (row.Cells[0].FindControl("hidUserid") as HiddenField);
        //        strcheck = 1;
        //        UserBase loginUser = (UserBase)Session["LoggedUser"];

        //        Results results = new Results();
        //        results.ActionStatus = "SUCCESS";
        //        string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
        //        SqlConnection con = new SqlConnection(connectionstring);
        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("USP_ActiveInactiveUser", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        SqlDataAdapter sda = new SqlDataAdapter(cmd);
        //        cmd.Parameters.Add(new SqlParameter("@in_iCurrUserId", Convert.ToInt32(hdnCurrentUser.Value)));
        //        cmd.Parameters.Add(new SqlParameter("@iActive", strcheck));
        //        cmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;

        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        DataSet ds = new DataSet();
        //        da.Fill(ds);
        //        results.Message = cmd.Parameters["@SP_MESSAGE"].Value.ToString();
        //        if (results.Message == "Data added successfully")
        //        {
        //            BindGrid();
        //            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ValidationMessage", "alert('User Id activated successfully.');", true);
        //            return;
        //        }
        //    }
        //else
        //{
        //    HiddenField hdnCurrentUserId = (row.Cells[0].FindControl("hidUserid") as HiddenField);
        //    strcheck = 0;
        //    UserBase loginUser = (UserBase)Session["LoggedUser"];

        //    Results results = new Results();
        //    results.ActionStatus = "SUCCESS";
        //    string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
        //    SqlConnection con = new SqlConnection(connectionstring);
        //    con.Open();
        //    SqlCommand cmd = new SqlCommand("USP_ActiveInactiveUser", con);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    SqlDataAdapter sda = new SqlDataAdapter(cmd);
        //    cmd.Parameters.Add(new SqlParameter("@in_iCurrUserId", Convert.ToInt32(hdnCurrentUserId.Value)));
        //    cmd.Parameters.Add(new SqlParameter("@iActive", strcheck));
        //    cmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;

        //    SqlDataAdapter da = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    da.Fill(ds);
        //    results.Message = cmd.Parameters["@SP_MESSAGE"].Value.ToString();
        //    if (results.Message == "Data added successfully")
        //    {
        //        BindGrid();
        //        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ValidationMessage", "alert('User Id deactivated successfully.');", true);
        //        return;
        //    }
        //}
        //    }
        //}
        //lblmsg.Text = data;
        // }

        //protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        //{



        //    foreach (GridViewRow row in GridView1.Rows)
        //    {

        //        CheckBox chkRow = (CheckBox)row.FindControl("chkActives");
        //        int strcheck;
        //        if (chkRow.Checked == true)
        //        {
        //            strcheck = 1;
        //        }
        //        else
        //        {
        //            strcheck = 0;
        //        }

        //        UserBase loginUser = (UserBase)Session["LoggedUser"];

        //        //Results results = new Results();
        //        //results.ActionStatus = "SUCCESS";
        //        //string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
        //        //SqlConnection con = new SqlConnection(connectionstring);
        //        //con.Open();
        //        //SqlCommand cmd = new SqlCommand("USP_ManagePassword", con);
        //        //cmd.CommandType = CommandType.StoredProcedure;
        //        //SqlDataAdapter sda = new SqlDataAdapter(cmd);
        //        //cmd.Parameters.Add(new SqlParameter("@in_iCurrUserId", Convert.ToInt32(hdnCurrentUserId.Value)));
        //        //cmd.Parameters.Add(new SqlParameter("@in_iOrgId", Convert.ToInt32(hdnLoginOrgId.Value)));
        //        //cmd.Parameters.Add(new SqlParameter("@vNewPassword_Var", txtNewpassword.Text.Trim()));
        //        //cmd.Parameters.Add(new SqlParameter("@iUserId_Var", Convert.ToInt32(loginUser.UserId)));
        //        //cmd.Parameters.Add(new SqlParameter("@iActive", strcheck));

        //        //cmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;

        //        //SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        //DataSet ds = new DataSet();
        //        //da.Fill(ds);
        //        //results.Message = cmd.Parameters["@SP_MESSAGE"].Value.ToString();
        //        //if (results.Message == "Data added successfully")
        //        //{
        //        //    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ValidationMessage", "alert('Action completed successfully.');", true);
        //        //    return;
        //        //}
        //    }

        //}




    }
}
