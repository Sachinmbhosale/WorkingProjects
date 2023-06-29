using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Configuration;
using System.Web.Security;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Data.SqlClient;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class ViewDashboard : System.Web.UI.Page
    {

        DataSet dsData = null;
        string strTemp = "";

        private SqlConnection con;
        private SqlCommand com;
        private string constr, query;
        private void connection()
        {
            constr = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ToString();
            con = new SqlConnection(constr);
            con.Open();

        }

        //protected void Page_Load(object sender, EventArgs e)
        //{

        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();

            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                int iOrgId = loginUser.LoginOrgId;
                if (loginUser.IsDomainUser == true)
                {
                    Response.Redirect("~/Secure/Home.aspx");
                }
                //lblDate.Text = DateTime.Now.ToString("dd-MM-yyyy h:mmtt");




                //Description : Display all department barchart together on page load event 
                //Created on 05/12/2022
                //Created By Sachin Bhosale
                
                //dsChartData = objTChartData.GetDashboardTotalCount(iOrgId);
                DashboardBAL objTChartData = new DashboardBAL();
                DataSet dsChartData = new DataSet();
                //dsChartData = objTChartData.GetDashboardTotalCount(iOrgId);
                DataSet dsChar = new DataSet();

                
                    dsChar = objTChartData.GetDashboardTotalCount(iOrgId);
                


                string deptName = dsChar.Tables[0].Rows[0]["Dept_Name"].ToString();
                deptName = deptName.Replace(",", "','");
                deptName = deptName.Replace("[", "['");
                deptName = deptName.Replace("]", "']");
                string No_of_doc = dsChar.Tables[0].Rows[0]["NoofDocuments"].ToString();
                No_of_doc = No_of_doc.Replace(",", "','");
                No_of_doc = No_of_doc.Replace("[", "['");
                No_of_doc = No_of_doc.Replace("]", "']");



                string Page_Count = dsChar.Tables[0].Rows[0]["Page_Count"].ToString();
                Page_Count = Page_Count.Replace(",", "','");
                Page_Count = Page_Count.Replace("[", "['");
                Page_Count = Page_Count.Replace("]", "']");


                string DeptName = dsChar.Tables[1].Rows[0]["Deptname"].ToString();
                DeptName = DeptName.Replace(",", "','");
                DeptName = DeptName.Replace("[", "['");
                DeptName = DeptName.Replace("]", "']");

                string DeptCount = dsChar.Tables[1].Rows[0]["DeptCount"].ToString();
                DeptCount = DeptCount.Replace(",", "','");
                DeptCount = DeptCount.Replace("[", "['");
                DeptCount = DeptCount.Replace("]", "']");


                string userStatus = dsChar.Tables[2].Rows[0]["userStatus"].ToString();
                userStatus = userStatus.Replace(",", "','");
                userStatus = userStatus.Replace("[", "['");
                userStatus = userStatus.Replace("]", "']");


                string user_count = dsChar.Tables[2].Rows[0]["user_count"].ToString();
                user_count = user_count.Replace(",", "','");
                user_count = user_count.Replace("[", "['");
                user_count = user_count.Replace("]", "']");



                string Dept0 = dsChar.Tables[3].Rows[0]["Department_vName"].ToString();
                string Doc0 = dsChar.Tables[3].Rows[0]["NoofDocuments"].ToString();
                string pgCnt0 = dsChar.Tables[3].Rows[0]["NoofPages"].ToString();

                lblDept0.Text = Dept0;
                LblDept0_Doc_count.Text = Doc0;
                lblPagecount0.Text = pgCnt0;


                string Dept1 = dsChar.Tables[3].Rows[1]["Department_vName"].ToString();
                string Doc1 = dsChar.Tables[3].Rows[1]["NoofDocuments"].ToString();
                string pgCnt1 = dsChar.Tables[3].Rows[1]["NoofPages"].ToString();

                lblDept1.Text = Dept1;
                lblDept1_Doc_count.Text = Doc1;
                lblPagecount1.Text = pgCnt1;


                string Dept2 = dsChar.Tables[3].Rows[2]["Department_vName"].ToString();
                string Doc2 = dsChar.Tables[3].Rows[2]["NoofDocuments"].ToString();
                string pgCnt2 = dsChar.Tables[3].Rows[2]["NoofPages"].ToString();

                lblDept2.Text = Dept2;
                lblDept2_Doc_count.Text = Doc2;
                lblPageCount2.Text = pgCnt2;


                string Dept3 = dsChar.Tables[3].Rows[3]["Department_vName"].ToString();
                string Doc3 = dsChar.Tables[3].Rows[3]["NoofDocuments"].ToString();
                string pgCnt3 = dsChar.Tables[3].Rows[3]["NoofPages"].ToString();

                lblDept3.Text = Dept3;
                lblDoc3.Text = Doc3;
                lblpagecount3.Text = pgCnt3;


                string Dept4 = dsChar.Tables[3].Rows[4]["Department_vName"].ToString();
                string Doc4 = dsChar.Tables[3].Rows[4]["NoofDocuments"].ToString();
                string pgCnt4 = dsChar.Tables[3].Rows[4]["NoofPages"].ToString();

                lblDept4.Text = Dept4;
                lblDoc4.Text = Doc4;
                lblDoccount4.Text = pgCnt4;


                string Dept5 = dsChar.Tables[3].Rows[5]["Department_vName"].ToString();
                string Doc5 = dsChar.Tables[3].Rows[5]["NoofDocuments"].ToString();
                string pgCnt5 = dsChar.Tables[3].Rows[5]["NoofPages"].ToString();

                Lbldept5.Text = Dept5;
                LblDoc5.Text = Doc5;
                lblpagecount5.Text = pgCnt5;

                //ltChartData.Text = "<script>chartDept = " + deptName + ";" + "chart_No_Of_Doc=" + No_of_doc + ";" +"chart_page_Counts=" + Page_Count + ";" +"DeptName=" + DeptName + ";" +"DeptCount=" + DeptCount + ";" +"</script>";

                string Dept6 = dsChar.Tables[3].Rows[6]["Department_vName"].ToString();
                string Doc6 = dsChar.Tables[3].Rows[6]["NoofDocuments"].ToString();
                string pgCnt6 = dsChar.Tables[3].Rows[6]["NoofPages"].ToString();

                lbldept6.Text = Dept6;
                lbldoc6.Text = Doc6;
                lbldoccount6.Text = pgCnt6;


                string Dept7 = dsChar.Tables[3].Rows[7]["Department_vName"].ToString();
                string Doc7 = dsChar.Tables[3].Rows[7]["NoofDocuments"].ToString();
                string pgCnt7 = dsChar.Tables[3].Rows[7]["NoofPages"].ToString();

                lblDept7.Text = Dept7;
                lblDoc7.Text = Doc7;
                lblpagecount7.Text = pgCnt7;


                string Dept8 = dsChar.Tables[3].Rows[8]["Department_vName"].ToString();
                string Doc8 = dsChar.Tables[3].Rows[8]["NoofDocuments"].ToString();
                string pgCnt8 = dsChar.Tables[3].Rows[8]["NoofPages"].ToString();

                lblDept8.Text = Dept8;
                lbldoc8.Text = Doc8;
                lblpagecount8.Text = pgCnt8;


                string Dept9 = dsChar.Tables[3].Rows[9]["Department_vName"].ToString();
                string Doc9 = dsChar.Tables[3].Rows[9]["NoofDocuments"].ToString();
                string pgCnt9 = dsChar.Tables[3].Rows[9]["NoofPages"].ToString();

                lblDept9.Text = Dept9;
                lbldoc9.Text = Doc9;
                lblpagecount9.Text = pgCnt9;


                string Dept10 = dsChar.Tables[3].Rows[10]["Department_vName"].ToString();
                string Doc10 = dsChar.Tables[3].Rows[10]["NoofDocuments"].ToString();
                string pgCnt10 = dsChar.Tables[3].Rows[10]["NoofPages"].ToString();

                lblDept10.Text = Dept10;
                lblDoc10.Text = Doc10;
                lblpagecount10.Text = pgCnt10;


                string Dept11 = dsChar.Tables[3].Rows[11]["Department_vName"].ToString();
                string Doc11 = dsChar.Tables[3].Rows[11]["NoofDocuments"].ToString();
                string pgCnt11 = dsChar.Tables[3].Rows[11]["NoofPages"].ToString();

                lbldept11.Text = Dept11;
                lbldoc11.Text = Doc11;
                lblpagecount11.Text = pgCnt11;

                //GridView1.DataSource = dsChartData;
                //GridView1.DataBind();




                //string Dept12 = dsChar.Tables[3].Rows[12]["Department_vName"].ToString();
                //string Doc12 = dsChar.Tables[3].Rows[12]["NoofDocuments"].ToString();
                //string pgCnt12 = dsChar.Tables[3].Rows[12]["NoofPages"].ToString();

                //lblDept.Text = Dept12;
                //lbldoc12.Text = Doc12;
                //lblpagecount12.Text = pgCnt12;

                //ltChartData.Text = "<script>chartDept = " + deptName + ";" + "chart_No_Of_Doc=" + No_of_doc + ";" +"chart_page_Counts=" + Page_Count + ";" +"DeptName=" + DeptName + ";" +"DeptCount=" + DeptCount + ";" +"</script>";

                string Dept13 = dsChar.Tables[3].Rows[12]["Department_vName"].ToString();
                string Doc13 = dsChar.Tables[3].Rows[12]["NoofDocuments"].ToString();
                string pgCnt13 = dsChar.Tables[3].Rows[12]["NoofPages"].ToString();

                lbldept13.Text = Dept13;
                lbldoc13.Text = Doc13;
                lblpagecount13.Text = pgCnt13;


                string Dept14 = dsChar.Tables[3].Rows[13]["Department_vName"].ToString();
                string Doc14 = dsChar.Tables[3].Rows[13]["NoofDocuments"].ToString();
                string pgCnt14 = dsChar.Tables[3].Rows[13]["NoofPages"].ToString();

                lblDept14.Text = Dept14;
                lbldoc14.Text = Doc14;
                lblPagecount14.Text = pgCnt14;


                string Dept15 = dsChar.Tables[3].Rows[14]["Department_vName"].ToString();
                string Doc15 = dsChar.Tables[3].Rows[14]["NoofDocuments"].ToString();
                string pgCnt15 = dsChar.Tables[3].Rows[14]["NoofPages"].ToString();

                lblDept15.Text = Dept15;
                lbldoc15.Text = Doc15;
                lblpagecount15.Text = pgCnt15;


                string Dept16 = dsChar.Tables[3].Rows[15]["Department_vName"].ToString();
                string Doc16 = dsChar.Tables[3].Rows[15]["NoofDocuments"].ToString();
                string pgCnt16 = dsChar.Tables[3].Rows[15]["NoofPages"].ToString();

                lblDept16.Text = Dept16;
                lblDoc16.Text = Doc16;
                lblpagecount16.Text = pgCnt16;


                string Dept17 = dsChar.Tables[3].Rows[16]["Department_vName"].ToString();
                string Doc17 = dsChar.Tables[3].Rows[16]["NoofDocuments"].ToString();
                string pgCnt17 = dsChar.Tables[3].Rows[16]["NoofPages"].ToString();

                lbldept17.Text = Dept17;
                lbldoc17.Text = Doc17;
                lblpagecount17.Text = pgCnt17;



                //added by parthamesh for chart data
                //ltChartData.Text = "<script>chartDept = " + deptName + ";" + "chart_No_Of_Doc=" + No_of_doc + ";chart_page_Counts=" + Page_Count + ";</script>";

                //ltChartData.Text = "<script>chartDept = " + deptName + ";" + "chart_No_Of_Doc=" + No_of_doc + ";" +"chart_page_Counts=" + Page_Count + ";" +"DeptName=" + DeptName + ";" +"DeptCount=" + DeptCount + ";" +"</script>";
                ltChartData.Text = "<script>userStatus = " + userStatus + ";" + "user_count=" + user_count + ";chartDept = " + deptName + ";" + "chart_No_Of_Doc=" + No_of_doc + ";" + "chart_page_Counts=" + Page_Count + ";" + "DeptName=" + DeptName + ";" + "DeptCount=" + DeptCount + ";" + "</script>";


                GridView1.DataSource = dsChar.Tables[3];
                GridView1.DataBind();

                if (dsChar.Tables.Count > 0 && dsChar.Tables[4].Rows.Count > 0)
                {
                    string Totalnodocs = dsChar.Tables[4].Rows[0][0].ToString();
                    string Totalnopages = dsChar.Tables[4].Rows[0][1].ToString();

                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        Label lblfnofodocs = (Label)GridView1.FooterRow.FindControl("lblfnofodocs");
                        Label lblfnoofpages = (Label)GridView1.FooterRow.FindControl("lblfnoofpages");
                        lblfnofodocs.Text = Totalnodocs;
                        lblfnoofpages.Text = Totalnopages;
                        lblTotalDocs.Text = Totalnodocs;
                        Lbltotalpages.Text = Totalnopages;
                    }


                }

                //end here

            }
        }
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


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            //if (e.Row.RowType == DataControlRowType.Footer)
            //{
            //    e.Row.Cells.RemoveAt(0);
            //    e.Row.Cells[0].ColumnSpan = 1;

            //}

        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {


            //LinkButton lb = sender as LinkButton;


            //GridViewRow Grow = (GridViewRow)lb.NamingContainer;
            //Label lblUPLOADiDocType = (Label)Grow.FindControl("lblUPLOADiDocType");
            //Label lbliDepartment = (Label)Grow.FindControl("lbliDepartment");
            //LinkButton LinkButton1 = (LinkButton)Grow.FindControl("LinkButton1");


            //Bindchart_Grid(lblUPLOADiDocType.Text.Trim(), lbliDepartment.Text.Trim(), LinkButton1.Text);


        }
    }
}