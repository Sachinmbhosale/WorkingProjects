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

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class Dashboard : PageBase
    {
        DataSet dsData = null;
        string strTemp = "";

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

                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;

                //GetDocumentType_BS(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);


                DashboardBAL objTChartData = new DashboardBAL();
                DataSet dsChartData = new DataSet();
                dsChartData = objTChartData.GetDashboardTotalCount(iOrgId);
                GridView1.DataSource = dsChartData;
                GridView1.DataBind();

                BindMainChartData();

                if (dsChartData.Tables.Count > 0 && dsChartData.Tables[1].Rows.Count > 0)
                {
                    string Totalnodocs = dsChartData.Tables[1].Rows[0][0].ToString();
                    string Totalnopages = dsChartData.Tables[1].Rows[0][1].ToString();

                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        Label lblfnofodocs = (Label)GridView1.FooterRow.FindControl("lblfnofodocs");
                        Label lblfnoofpages = (Label)GridView1.FooterRow.FindControl("lblfnoofpages");
                        lblfnofodocs.Text = Totalnodocs;
                        lblfnoofpages.Text = Totalnopages;

                    }


                }


                //DashboardBAL objDash = new DashboardBAL();
                //dsData = new DataSet();
                ////dsData = objDash.FillProjectList(loginUser.UserName);
                //dsData = objDash.FillProjectList("administrator");

                //if (dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                //{
                //    FillDataDropDown(ddlProjType, dsData.Tables[0], dsData.Tables[0].Columns[0].ColumnName.ToString(), dsData.Tables[0].Columns[0].ColumnName.ToString());
                //    ddlProjType.SelectedIndex = 1;

                //    bindDept(ddlProjType.SelectedItem.Text);
                //    if (ddlDept.Items.Count > 1) { ddlDept.SelectedIndex = 1; }
                //    ddlDept_SelectedIndexChanged(sender, e);

                //    if (ddlDoc.Items.Count > 1) { ddlDoc.SelectedIndex = 1; }
                //    ddlDoc_SelectedIndexChanged(sender, e);
                //}
                //else
                //{
                //    ddlProjType.Items.Add(new ListItem("--Select--", "0"));
                //    ddlDept.Items.Add(new ListItem("--Select--", "0"));
                //    ddlDoc.Items.Add(new ListItem("--Select--", "0"));
                //}
            }
        }

        //Bind Main chart.
        //private string Bindchart()
        //{
        //    try
        //    {
        //        DashboardBAL objDash = new DashboardBAL();

        //        DataSet dsData = new DataSet();
        //        int i;

        //        dsData = objDash.MainChartData(ddlProjType.SelectedValue, ddlDept.SelectedValue);
        //        DataTable ChartData = dsData.Tables[0];

        //        if (ChartData.Rows.Count > 0)
        //        {
        //            //storing total rows count to loop on each Record  
        //            string[] XPoint = new string[ChartData.Rows.Count];
        //            int[] YPoint = new int[ChartData.Rows.Count];

        //            for (i = 1; i < ChartData.Columns.Count; i++)
        //            {
        //                Legend Lgd = new Legend();
        //                Lgd.Name = "CountLegend";
        //                chrtOverall.Legends.Add(Lgd);

        //                Series series = new Series();
        //                foreach (DataRow dr in ChartData.Rows)
        //                {
        //                    LegendItem legItem = new LegendItem();

        //                    int y = Convert.ToInt32(dr["Count"]);
        //                    series.Points.AddXY(dr["Data"].ToString(), y);
        //                    series.SmartLabelStyle.Enabled = false;
        //                    series.LabelAngle = -90;

        //                    //Below some properties... of Legends
        //                    legItem.Name = dr["Data"].ToString();
        //                    legItem.BorderWidth = 2;
        //                    legItem.ShadowOffset = 1;

        //                    if (legItem.Name.Trim().ToLower().Contains("doc")) { legItem.Color = Color.DarkMagenta; }
        //                    else if (legItem.Name.Trim().ToLower().Contains("page")) { legItem.Color = Color.YellowGreen; }
        //                    else if (legItem.Name.Trim().ToLower().Contains("untagged")) { legItem.Color = Color.Red; }
        //                    else if (legItem.Name.Trim().ToLower().Contains("tagged")) { legItem.Color = Color.RoyalBlue; }
        //                    else if (legItem.Name.Trim().ToLower().Contains("upload")) { legItem.Color = Color.Brown; }
        //                    else if (legItem.Name.Trim().ToLower().Contains("pending")) { legItem.Color = Color.Gray; }

        //                    //legItem.Color = Color.FromName(chrtOverall.Series[dr["Data"].ToString()].Color.Name.ToString());
        //                    //legItem = new LegendItem();                            
        //                    chrtOverall.Legends["CountLegend"].CustomItems.Add(legItem);
        //                }
        //                //chrtOverall.Legends["CountLegend"].CustomItems.RemoveAt(0);

        //                series.Name = ChartData.Columns[i].ToString();
        //                chrtOverall.Series.Add(series);
        //                chrtOverall.Series[i - 1].ChartType = SeriesChartType.Column;
        //                chrtOverall.Series[i - 1].IsValueShownAsLabel = true;
        //                chrtOverall.Series[i - 1].IsVisibleInLegend = false;
        //                chrtOverall.Series[i - 1].BorderWidth = 10;
        //            }


        //            //--------------------------------------------------------------------------------------------
        //            //Series series = new Series();
        //            //for (int count = 0; count < ChartData.Rows.Count; count++)
        //            //{
        //            //    XPoint[count] = ChartData.Rows[count]["Data"].ToString();    //storing Values for X axis 
        //            //    YPoint[count] = Convert.ToInt32(ChartData.Rows[count]["Count"]);   //storing values for Y Axis  
        //            //}
        //            //chrtOverall.Series[0].Points.DataBindXY(XPoint, YPoint); //binding chart control  
        //            //chrtOverall.Series[0].BorderWidth = 10;  //Setting width of line 
        //            //chrtOverall.Series[0].ChartType = SeriesChartType.Column;   //setting Chart type    
        //            //-------------------------------------------------------------------------------------------------
        //            chrtOverall.ChartAreas[0].AxisY.Title = "Total count";
        //            chrtOverall.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);

        //            chrtOverall.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
        //            chrtOverall.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
        //            chrtOverall.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;

        //            chrtOverall.ChartAreas["ChartArea1"].AxisX.LabelStyle.Angle = 45;
        //            chrtOverall.ChartAreas["ChartArea1"].AxisX.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
        //            chrtOverall.ChartAreas["ChartArea1"].AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
        //            chrtOverall.Legends[0].Enabled = true;

        //            foreach (Series charts in chrtOverall.Series)
        //            {
        //                i = 1;
        //                foreach (DataPoint point in charts.Points)
        //                {
        //                    switch (i)
        //                    {
        //                        //case 1: point.Color = Color.Brown; break;
        //                        //case 2: point.Color = Color.Turquoise; break;
        //                        //case 3: point.Color = Color.YellowGreen; break;
        //                        //case 4: point.Color = Color.LightCoral; break;     //OrangeRed

        //                        case 1:
        //                            point.Color = Color.DarkMagenta;
        //                            break;
        //                        case 2:
        //                            point.Color = Color.YellowGreen;
        //                            //(new System.Collections.Generic.Mscorlib_CollectionDebugView<System.Web.UI.DataVisualization.Charting.LegendItem>(chrtOverall.Legends["CountLegend"].CustomItems)).Items[1].Color = Color.YellowGreen;
        //                            break;
        //                        case 3:
        //                            point.Color = Color.RoyalBlue;
        //                            break;
        //                        case 4:
        //                            point.Color = Color.Red;
        //                            break;     //OrangeRed
        //                        case 5:
        //                            point.Color = Color.Brown;
        //                            break;
        //                        case 6:
        //                            point.Color = Color.Gray;
        //                            break;
        //                    }
        //                    //  point.Label = string.Format("{0:0} - {1}", point.YValues[0], point.AxisLabel);

        //                    point.Name = XPoint[0];
        //                    point.Label = string.Format("{0:0}", point.YValues[0]);
        //                    point.LegendText = point.AxisLabel;

        //                    //point.Label = string.Format("{0:0}", point.YValues[0]);
        //                    //point.Name = XPointMember[0];
        //                    ////point.ToolTip = string.Format("{1} - {0:0}", point.AxisLabel, BudgetAmt[i]);
        //                    //point.LegendText = point.AxisLabel;
        //                    i = i + 1;
        //                }
        //            }
        //            ChartData.Dispose();

        //            // chrtOverall.Legends[0].Position = new ElementPosition(30, 60, 100, 20);
        //            //  chrtOverall.Legends[0].Position = new ElementPosition(90, 15, 50, 120);
        //            for (int j = 0; j < chrtOverall.Legends.Count; j++)
        //            {
        //                chrtOverall.Legends[j].BorderColor = Color.Black;
        //                //chrtOverall.Legends[j].BorderWidth = 1;
        //                chrtOverall.Legends[j].BorderDashStyle = ChartDashStyle.NotSet;
        //                chrtOverall.Legends[j].ShadowOffset = 1;
        //                chrtOverall.Legends[j].LegendStyle = LegendStyle.Table;
        //                chrtOverall.Legends[j].TableStyle = LegendTableStyle.Auto;
        //                chrtOverall.Legends[j].Docking = Docking.Bottom;
        //                chrtOverall.Legends[j].Alignment = StringAlignment.Near;
        //                chrtOverall.Legends[j].Enabled = true;
        //                chrtOverall.Legends[j].Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Regular);
        //                chrtOverall.Legends[j].AutoFitMinFontSize = 5;
        //                chrtOverall.Legends[j].ItemColumnSpacing = 1;
        //                //chrtOverall.Legends[j].DockedToChartArea =                     
        //            }
        //            return "";
        //        }
        //        else
        //        {
        //            //lblErrMsg.Visible = true;
        //            //lblErrMsg.Text = "Data not found.";
        //            return "No Data";
        //        }
        //    }
        //    catch
        //    {
        //        return "Error";
        //    }
        //}


        private string BindMainChartData()
        {
            try
            {
                DashboardBAL objDash = new DashboardBAL();

                DataSet dsData = new DataSet();
                int i;

                dsData = objDash.BindMainChartData();
                DataTable ChartData = dsData.Tables[0];

                if (ChartData.Rows.Count > 0)
                {
                    //storing total rows count to loop on each Record  
                    string[] XPoint = new string[ChartData.Rows.Count];
                    int[] YPoint = new int[ChartData.Rows.Count];

                    for (i = 1; i < ChartData.Columns.Count; i++)
                    {
                        Legend Lgd = new Legend();
                        Lgd.Name = "CountLegend";
                        chrtOverall.Legends.Add(Lgd);

                        Series series = new Series();
                        foreach (DataRow dr in ChartData.Rows)
                        {
                            LegendItem legItem = new LegendItem();


                            int y = Convert.ToInt32(dr["Count"]);
                            series.Points.AddXY(dr["Data"].ToString(), y);
                            series.SmartLabelStyle.Enabled = false;
                            series.LabelAngle = 0;

                            //int y = Convert.ToInt32(dr["NoofDocuments"]);
                            //series.Points.AddXY(dr["NoofPages"].ToString(), y);
                            //series.SmartLabelStyle.Enabled = false;
                            //series.LabelAngle = -90;

                            //Below some properties... of Legends
                            //legItem.Name = "File Count";//dr["Data"].ToString();                          
                            //legItem.BorderWidth = 2;
                            //legItem.ShadowOffset = 1;

                            //legItem.Name = "Page Count";
                            //legItem.BorderWidth = 2;
                            //legItem.ShadowOffset = 1;



                            //if (legItem.Name.Trim().ToLower().Contains("File Count")) { legItem.Color = Color.DarkMagenta; }
                            //else if (legItem.Name.Trim().ToLower().Contains("Page Count")) { legItem.Color = Color.YellowGreen; }
                            //else if (legItem.Name.Trim().ToLower().Contains(" ")) { legItem.Color = Color.Red; }
                            //else if (legItem.Name.Trim().ToLower().Contains("Finance")) { legItem.Color = Color.RoyalBlue; }
                            //else if (legItem.Name.Trim().ToLower().Contains(" ")) { legItem.Color = Color.Brown; }
                            //else if (legItem.Name.Trim().ToLower().Contains("HRD")) { legItem.Color = Color.Gray; }

                            //legItem.Color = Color.FromName(chrtOverall.Series[dr["Data"].ToString()].Color.Name.ToString());
                            //legItem = new LegendItem();                            
                            // chrtOverall.Legends["CountLegend"].CustomItems.Add(legItem);
                        }
                        //chrtOverall.Legends["CountLegend"].CustomItems.RemoveAt(0);

                        series.Name = ChartData.Columns[i].ToString();
                        chrtOverall.Series.Add(series);
                        chrtOverall.Series[i - 1].ChartType = SeriesChartType.Column;
                        chrtOverall.Series[i - 1].IsValueShownAsLabel = true;
                        chrtOverall.Series[i - 1].IsVisibleInLegend = true;
                        chrtOverall.Series[i - 1].BorderWidth = 10;
                    }


                    //--------------------------------------------------------------------------------------------
                    //Series series = new Series();
                    //for (int count = 0; count < ChartData.Rows.Count; count++)
                    //{
                    //    XPoint[count] = ChartData.Rows[count]["Data"].ToString();    //storing Values for X axis 
                    //    YPoint[count] = Convert.ToInt32(ChartData.Rows[count]["Count"]);   //storing values for Y Axis  
                    //}
                    //chrtOverall.Series[0].Points.DataBindXY(XPoint, YPoint); //binding chart control  
                    //chrtOverall.Series[0].BorderWidth = 10;  //Setting width of line 
                    //chrtOverall.Series[0].ChartType = SeriesChartType.Column;   //setting Chart type    
                    //-------------------------------------------------------------------------------------------------

                    chrtOverall.ChartAreas[0].AxisY.Title = "Total count";
                    chrtOverall.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);

                    chrtOverall.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
                    chrtOverall.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                    chrtOverall.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;

                    chrtOverall.ChartAreas["ChartArea1"].AxisX.LabelStyle.Angle = 45;
                    chrtOverall.ChartAreas["ChartArea1"].AxisX.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
                    chrtOverall.ChartAreas["ChartArea1"].AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
                    chrtOverall.Legends[0].Enabled = true;

                    foreach (Series charts in chrtOverall.Series)
                    {
                        i = 1;
                        foreach (DataPoint point in charts.Points)
                        {
                            switch (i)
                            {
                                //case 1: point.Color = Color.Brown; break;
                                //case 2: point.Color = Color.Turquoise; break;
                                //case 3: point.Color = Color.YellowGreen; break;
                                //case 4: point.Color = Color.LightCoral; break;     //OrangeRed

                                case 1:
                                    point.Color = Color.DarkMagenta;
                                    break;
                                case 2:
                                    point.Color = Color.YellowGreen;
                                    //(new System.Collections.Generic.Mscorlib_CollectionDebugView<System.Web.UI.DataVisualization.Charting.LegendItem>(chrtOverall.Legends["CountLegend"].CustomItems)).Items[1].Color = Color.YellowGreen;
                                    break;
                                case 3:
                                    point.Color = Color.DarkMagenta;
                                    break;
                                case 4:
                                    point.Color = Color.YellowGreen;
                                    break;     //OrangeRed
                                case 5:
                                    point.Color = Color.DarkMagenta;
                                    break;
                                case 6:
                                    point.Color = Color.YellowGreen;
                                    break;
                                    //case 3:
                                    //    point.Color = Color.RoyalBlue;
                                    //    break;
                                    //case 4:
                                    //    point.Color = Color.Red;
                                    //    break;     //OrangeRed
                                    //case 5:
                                    //    point.Color = Color.Brown;
                                    //    break;
                                    //case 6:
                                    //    point.Color = Color.Gray;
                                    //    break;
                            }
                            //  point.Label = string.Format("{0:0} - {1}", point.YValues[0], point.AxisLabel);

                            point.Name = XPoint[0];
                            point.Label = string.Format("{0:0}", point.YValues[0]);
                            point.LegendText = point.AxisLabel;

                            //point.Label = string.Format("{0:0}", point.YValues[0]);
                            //point.Name = XPointMember[0];
                            ////point.ToolTip = string.Format("{1} - {0:0}", point.AxisLabel, BudgetAmt[i]);
                            //point.LegendText = point.AxisLabel;
                            i = i + 1;
                        }
                    }
                    ChartData.Dispose();

                    // chrtOverall.Legends[0].Position = new ElementPosition(30, 60, 100, 20);
                    //  chrtOverall.Legends[0].Position = new ElementPosition(90, 15, 50, 120);
                    for (int j = 0; j < chrtOverall.Legends.Count; j++)
                    {
                        chrtOverall.Legends[j].BorderColor = Color.Black;
                        //chrtOverall.Legends[j].BorderWidth = 1;
                        chrtOverall.Legends[j].BorderDashStyle = ChartDashStyle.NotSet;
                        chrtOverall.Legends[j].ShadowOffset = 1;
                        chrtOverall.Legends[j].LegendStyle = LegendStyle.Table;
                        chrtOverall.Legends[j].TableStyle = LegendTableStyle.Auto;
                        chrtOverall.Legends[j].Docking = Docking.Bottom;
                        chrtOverall.Legends[j].Alignment = StringAlignment.Near;
                        chrtOverall.Legends[j].Enabled = true;
                        chrtOverall.Legends[j].Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Regular);
                        chrtOverall.Legends[j].AutoFitMinFontSize = 5;
                        chrtOverall.Legends[j].ItemColumnSpacing = 1;
                        //chrtOverall.Legends[j].DockedToChartArea =                     
                    }
                    return "";
                }
                else
                {
                    //lblErrMsg.Visible = true;
                    //lblErrMsg.Text = "Data not found.";
                    return "No Data";
                }
            }
            catch
            {
                return "Error";
            }
        }

        private string Bindchart_Grid(string sProjType, string sDepttype)
        {
            try
            {
                DashboardBAL objDash = new DashboardBAL();

                DataSet dsData = new DataSet();
                int i;

                dsData = objDash.MainChartData(sProjType, sDepttype);
                DataTable ChartData = dsData.Tables[0];

                if (ChartData.Rows.Count > 0)
                {
                    //storing total rows count to loop on each Record  
                    string[] XPoint = new string[ChartData.Rows.Count];
                    int[] YPoint = new int[ChartData.Rows.Count];

                    for (i = 1; i < ChartData.Columns.Count; i++)
                    {
                        Legend Lgd = new Legend();
                        Lgd.Name = "CountLegend";
                        chrtOverall.Legends.Add(Lgd);

                        Series series = new Series();
                        foreach (DataRow dr in ChartData.Rows)
                        {
                            LegendItem legItem = new LegendItem();

                            int y = Convert.ToInt32(dr["Count"]);
                            series.Points.AddXY(dr["Data"].ToString(), y);
                            series.SmartLabelStyle.Enabled = false;
                            series.LabelAngle = -90;

                            //int y = Convert.ToInt32(dr["NoofDocuments"]);
                            //series.Points.AddXY(dr["NoofPages"].ToString(), y);
                            //series.SmartLabelStyle.Enabled = false;
                            //series.LabelAngle = -90;

                            //Below some properties... of Legends
                            legItem.Name = dr["Data"].ToString();
                            legItem.BorderWidth = 2;
                            legItem.ShadowOffset = 1;

                            if (legItem.Name.Trim().ToLower().Contains("Legal")) { legItem.Color = Color.DarkMagenta; }
                            else if (legItem.Name.Trim().ToLower().Contains("Finance")) { legItem.Color = Color.YellowGreen; }
                            else if (legItem.Name.Trim().ToLower().Contains("HRD")) { legItem.Color = Color.Red; }
                            //else if (legItem.Name.Trim().ToLower().Contains("Finance")) { legItem.Color = Color.RoyalBlue; }
                            //else if (legItem.Name.Trim().ToLower().Contains("Docs")) { legItem.Color = Color.Brown; }
                            //else if (legItem.Name.Trim().ToLower().Contains("HRD")) { legItem.Color = Color.Gray; }

                            //legItem.Color = Color.FromName(chrtOverall.Series[dr["Data"].ToString()].Color.Name.ToString());
                            //legItem = new LegendItem();                            
                            chrtOverall.Legends["CountLegend"].CustomItems.Add(legItem);
                        }
                        //chrtOverall.Legends["CountLegend"].CustomItems.RemoveAt(0);

                        series.Name = ChartData.Columns[i].ToString();
                        chrtOverall.Series.Add(series);
                        chrtOverall.Series[i - 1].ChartType = SeriesChartType.Column;
                        chrtOverall.Series[i - 1].IsValueShownAsLabel = true;
                        chrtOverall.Series[i - 1].IsVisibleInLegend = false;
                        chrtOverall.Series[i - 1].BorderWidth = 10;
                    }


                    //--------------------------------------------------------------------------------------------
                    //Series series = new Series();
                    //for (int count = 0; count < ChartData.Rows.Count; count++)
                    //{
                    //    XPoint[count] = ChartData.Rows[count]["Data"].ToString();    //storing Values for X axis 
                    //    YPoint[count] = Convert.ToInt32(ChartData.Rows[count]["Count"]);   //storing values for Y Axis  
                    //}
                    //chrtOverall.Series[0].Points.DataBindXY(XPoint, YPoint); //binding chart control  
                    //chrtOverall.Series[0].BorderWidth = 10;  //Setting width of line 
                    //chrtOverall.Series[0].ChartType = SeriesChartType.Column;   //setting Chart type    
                    //-------------------------------------------------------------------------------------------------

                    chrtOverall.ChartAreas[0].AxisY.Title = "Total count";
                    chrtOverall.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);

                    chrtOverall.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
                    chrtOverall.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
                    chrtOverall.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;

                    chrtOverall.ChartAreas["ChartArea1"].AxisX.LabelStyle.Angle = 45;
                    chrtOverall.ChartAreas["ChartArea1"].AxisX.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
                    chrtOverall.ChartAreas["ChartArea1"].AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
                    chrtOverall.Legends[0].Enabled = true;

                    foreach (Series charts in chrtOverall.Series)
                    {
                        i = 1;
                        foreach (DataPoint point in charts.Points)
                        {
                            switch (i)
                            {
                                //case 1: point.Color = Color.Brown; break;
                                //case 2: point.Color = Color.Turquoise; break;
                                //case 3: point.Color = Color.YellowGreen; break;
                                //case 4: point.Color = Color.LightCoral; break;     //OrangeRed

                                case 1:
                                    point.Color = Color.DarkMagenta;
                                    break;
                                case 2:
                                    point.Color = Color.YellowGreen;
                                    //(new System.Collections.Generic.Mscorlib_CollectionDebugView<System.Web.UI.DataVisualization.Charting.LegendItem>(chrtOverall.Legends["CountLegend"].CustomItems)).Items[1].Color = Color.YellowGreen;
                                    break;
                                case 3:
                                    point.Color = Color.RoyalBlue;
                                    break;
                                case 4:
                                    point.Color = Color.Red;
                                    break;     //OrangeRed
                                case 5:
                                    point.Color = Color.Brown;
                                    break;
                                case 6:
                                    point.Color = Color.Gray;
                                    break;
                            }
                            //  point.Label = string.Format("{0:0} - {1}", point.YValues[0], point.AxisLabel);

                            point.Name = XPoint[0];
                            point.Label = string.Format("{0:0}", point.YValues[0]);
                            point.LegendText = point.AxisLabel;

                            //point.Label = string.Format("{0:0}", point.YValues[0]);
                            //point.Name = XPointMember[0];
                            ////point.ToolTip = string.Format("{1} - {0:0}", point.AxisLabel, BudgetAmt[i]);
                            //point.LegendText = point.AxisLabel;
                            i = i + 1;
                        }
                    }
                    ChartData.Dispose();

                    //chrtOverall.Legends[0].Position = new ElementPosition(30, 60, 100, 20);
                    //chrtOverall.Legends[0].Position = new ElementPosition(90, 15, 50, 120);
                    for (int j = 0; j < chrtOverall.Legends.Count; j++)
                    {




                        chrtOverall.Legends[j].BorderColor = Color.Black;
                        //chrtOverall.Legends[j].BorderWidth = 1;
                        chrtOverall.Legends[j].BorderDashStyle = ChartDashStyle.NotSet;
                        chrtOverall.Legends[j].ShadowOffset = 1;
                        chrtOverall.Legends[j].LegendStyle = LegendStyle.Table;
                        chrtOverall.Legends[j].TableStyle = LegendTableStyle.Auto;
                        chrtOverall.Legends[j].Docking = Docking.Right;
                        chrtOverall.Legends[j].Alignment = StringAlignment.Center;
                        chrtOverall.Legends[j].Enabled = true;
                        chrtOverall.Legends[j].Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Regular);
                        chrtOverall.Legends[j].AutoFitMinFontSize = 5;
                        chrtOverall.Legends[j].ItemColumnSpacing = 1;
                        //chrtOverall.Legends[j].DockedToChartArea =                     
                    }
                    return "";
                }
                else
                {
                    //lblErrMsg.Visible = true;
                    //lblErrMsg.Text = "Data not found.";
                    return "No Data";
                }
            }
            catch
            {
                return "Error";
            }
        }

        //Bind Sub chart.
        //private string BindSubChart_doc()
        //{
        //    try
        //    {
        //        DashboardBAL objDash = new DashboardBAL();
        //        DataSet dsData = new DataSet();
        //        int i;

        //        //dsData = objDash.SubChartData(ddlDoc.SelectedItem.Text);
        //        dsData = objDash.SubChartData(txtDoc.Text.Trim());
        //        DataTable dtData = dsData.Tables[0];

        //        if (dtData.Rows.Count > 0)
        //        {
        //            //storing total rows count to loop on each Record  
        //            string[] XPoint = new string[dtData.Rows.Count];
        //            int[] YPoint = new int[dtData.Rows.Count];

        //            for (i = 1; i < dtData.Columns.Count; i++)
        //            {
        //                Series series = new Series();

        //                Legend Lgd = new Legend();
        //                Lgd.Name = "TotalCount";
        //                chrtDoc.Legends.Add(Lgd);

        //                foreach (DataRow dr in dtData.Rows)
        //                {
        //                    LegendItem legItem = new LegendItem();

        //                    int y = Convert.ToInt32(dr["Count"]);
        //                    series.Points.AddXY(dr["Data"].ToString(), y);
        //                    series.SmartLabelStyle.Enabled = false;
        //                    series.LabelAngle = -90;

        //                    //Below some properties... of Legends
        //                    legItem.Name = dr["Data"].ToString();
        //                    legItem.BorderWidth = 2;
        //                    legItem.ShadowOffset = 1;

        //                    if (legItem.Name.Trim().ToLower().Contains("page")) { legItem.Color = Color.YellowGreen; }
        //                    else if (legItem.Name.Trim().ToLower().Contains("untagged")) { legItem.Color = Color.Red; }
        //                    else if (legItem.Name.Trim().ToLower().Contains("tagged")) { legItem.Color = Color.RoyalBlue; }
        //                    chrtDoc.Legends["TotalCount"].CustomItems.Add(legItem);
        //                }

        //                series.Name = dtData.Columns[i].ToString();
        //                chrtDoc.Series.Add(series);
        //                chrtDoc.Series[i - 1].ChartType = SeriesChartType.Column;
        //                chrtDoc.Series[i - 1].IsValueShownAsLabel = true;
        //                chrtDoc.Series[i - 1].IsVisibleInLegend = false;
        //                chrtDoc.Series[i - 1].BorderWidth = 10;
        //            }

        //            chrtDoc.ChartAreas[0].AxisY.Title = "Total count";
        //            chrtDoc.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Bold);

        //            chrtDoc.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold);
        //            chrtDoc.ChartAreas["ChartArea2"].AxisX.MajorGrid.Enabled = false;
        //            chrtDoc.ChartAreas["ChartArea2"].AxisY.MajorGrid.Enabled = false;

        //            chrtDoc.ChartAreas["ChartArea2"].AxisX.LabelStyle.Angle = 45;
        //            chrtDoc.ChartAreas["ChartArea2"].AxisX.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
        //            chrtDoc.ChartAreas["ChartArea2"].AxisY.LabelStyle.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);

        //            foreach (Series charts in chrtDoc.Series)
        //            {
        //                i = 1;
        //                foreach (DataPoint point in charts.Points)
        //                {
        //                    switch (i)      //point.AxisLabel)
        //                    {
        //                        //case 1: point.Color = Color.Yellow; break;
        //                        //case 2: point.Color = Color.SpringGreen; break;
        //                        //case 3: point.Color = Color.LightPink; break;
        //                        //case 1: point.Color = Color.Turquoise; break;
        //                        //case 2: point.Color = Color.YellowGreen; break;
        //                        //case 3: point.Color = Color.LightCoral; break;

        //                        //case 1: point.Color = Color.DarkMagenta; break;
        //                        case 1: point.Color = Color.YellowGreen; break;
        //                        case 2: point.Color = Color.RoyalBlue; break;
        //                        case 3: point.Color = Color.Red; break;     //OrangeRed

        //                    }
        //                    point.Label = string.Format("{0:0}", point.YValues[0]);
        //                    point.LegendText = point.AxisLabel;

        //                    i = i + 1;
        //                }
        //            }

        //            //chrtDoc.Legends[0].Position = new ElementPosition(80, 20,30,100);
        //            // chrtOverall.Legends[0].Position = new ElementPosition(30, 60, 100, 20);
        //            //  chrtOverall.Legends[0].Position = new ElementPosition(90, 15, 50, 120);

        //            for (int j = 0; j < chrtDoc.Legends.Count; j++)
        //            {
        //                chrtDoc.Legends[j].BorderColor = Color.Black;
        //                //chrtDoc.Legends[j].BorderWidth = 1;
        //                chrtDoc.Legends[j].BorderDashStyle = ChartDashStyle.NotSet;
        //                chrtDoc.Legends[j].ShadowOffset = 1;
        //                chrtDoc.Legends[j].LegendStyle = LegendStyle.Table;
        //                chrtDoc.Legends[j].TableStyle = LegendTableStyle.Auto;
        //                chrtDoc.Legends[j].Docking = Docking.Bottom;
        //                chrtDoc.Legends[j].Alignment = StringAlignment.Near;
        //                chrtDoc.Legends[j].Enabled = true;
        //                chrtDoc.Legends[j].Font = new System.Drawing.Font("Verdana", 8, System.Drawing.FontStyle.Regular);
        //                chrtDoc.Legends[j].AutoFitMinFontSize = 5;
        //                chrtDoc.Legends[j].ItemColumnSpacing = 1;
        //                chrtDoc.Legends[j].IsDockedInsideChartArea = true;
        //            }
        //            return "";
        //        }
        //        else
        //        {
        //            //lblErrMsg.Visible = true;
        //            //lblErrMsg.Text = "Data not found.";
        //            return "No Data";
        //        }
        //    }
        //    catch
        //    {
        //        return "Error";
        //    }
        //}

        //protected void ddlProjType_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        lblChart1ErrMsg.Text = "";
        //        lblChart1ErrMsg.Visible = false;

        //        lblChrt2ErrMsg.Text = "";
        //        lblChrt2ErrMsg.Visible = false;

        //        //bindDept(ddlProjType.SelectedItem.Text);
        //        //ddlDoc.Items.Clear();
        //        //ddlDoc.Items.Add(new ListItem("--Select--", "0"));

        //        GetDepartments_BS(hdnLoginOrgId.Value, hdnLoginToken.Value);
        //    }

        //    catch (Exception ex)
        //    {
        //        Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
        //    }
        //}

        //protected void ddlDept_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        lblChart1ErrMsg.Text = "";
        //        lblChart1ErrMsg.Visible = false;
        //        lblChrt2ErrMsg.Text = "";
        //        lblChrt2ErrMsg.Visible = false;

        //        if (ddlDept.SelectedItem.Text.ToLower().Contains("select") == false)
        //        {
        //            strTemp = Bindchart();
        //            if (strTemp.Trim().ToLower() == "no data")
        //            {
        //                lblChart1ErrMsg.Visible = true;
        //                lblChart1ErrMsg.Text = "No data found.";

        //                ddlDoc.Items.Clear();
        //                ddlDoc.Items.Add(new ListItem("--Select--", "0"));
        //            }
        //            else
        //            {
        //                bindDocument(ddlDept.SelectedItem.Text);
        //            }
        //        }
        //        else
        //        {
        //            ddlDoc.Items.Clear();
        //            ddlDoc.Items.Add(new ListItem("--Select--", "0"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
        //    }
        //}

        //protected void ddlDoc_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        lblChrt2ErrMsg.Text = "";
        //        lblChrt2ErrMsg.Visible = false;

        //        //Bindchart();      
        //        if (ddlDoc.SelectedItem.Text.ToLower().Contains("select") == false)
        //        {
        //            strTemp = BindSubChart_doc();
        //            if (strTemp.Trim().ToLower() == "no data")
        //            {
        //                lblChrt2ErrMsg.Visible = true;
        //                lblChrt2ErrMsg.Text = "No data found.";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
        //    }
        //}

        //private void bindDept(string strProj)
        //{
        //    try
        //    {
        //        DashboardBAL objDash = new DashboardBAL();
        //        DataSet ds = new DataSet();

        //        ds = objDash.FillDeptList(strProj);
        //        FillDataDropDown(ddlDept, ds.Tables[0], ds.Tables[0].Columns[0].ColumnName.ToString(), ds.Tables[0].Columns[0].ColumnName.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
        //    }

        //}

        ////private void bindDocument(string strDept)
        ////{
        ////    try
        ////    {
        ////        DashboardBAL objDash = new DashboardBAL();
        ////        DataSet ds = new DataSet();
        ////        ds = objDash.FillDocList(strDept);
        ////        FillDataDropDown(ddlDoc, ds.Tables[0], ds.Tables[0].Columns[0].ColumnName.ToString(), ds.Tables[0].Columns[0].ColumnName.ToString());
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
        ////    }

        ////}

        public void FillDataDropDown(DropDownList ddlData, DataTable dtTemp, string v_Member, string T_Member)
        {
            try
            {
                ddlData.Items.Clear();
                if (dtTemp.Rows.Count > 0)
                {
                    ddlData.Items.Add(new ListItem("--Select--", "0"));
                    foreach (DataRow row in dtTemp.Rows)
                    {
                        ddlData.Items.Add(new ListItem(row[T_Member].ToString(), row[v_Member].ToString()));
                    }
                }
                else
                {
                    ddlData.DataSource = null;
                    ddlData.Items.Add(new ListItem("--Select--", "0"));
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }


        //public void GetDepartments_BS(string loginOrgId, string loginToken)
        //{
        //    DepartmentBL bl = new DepartmentBL();
        //    SearchFilter filter = new SearchFilter();
        //    filter.DocumentTypeID = Convert.ToInt32(ddlProjType.SelectedValue) != 0 ? Convert.ToInt32(ddlProjType.SelectedValue) : 0;
        //    try
        //    {
        //        string action = "DepartmentsForUpload";
        //        Results res = bl.SearchDepartments(filter, action, loginOrgId, loginToken);

        //        if (res.ActionStatus == "SUCCESS" && res.Departments != null)
        //        {
        //            ddlDept.Items.Clear();
        //            ddlDept.Items.Add(new ListItem("<Select>", "0"));


        //            // DMSENH6-4657 BE
        //            foreach (Department dp in res.Departments)
        //            {
        //                ddlDept.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
        //        Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
        //    }
        //}

        //public void GetDocumentType_BS(string loginOrgId, string loginToken)
        //{
        //    DocumentTypeBL bl = new DocumentTypeBL();
        //    SearchFilter filter = new SearchFilter();
        //    try
        //    {
        //        string action = "DocumentTypeForUpload";
        //        Results res = bl.SearchDocumentTypes(filter, action, loginOrgId, loginToken);

        //        if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
        //        {
        //            ddlProjType.Items.Clear();
        //            ddlProjType.Items.Add(new ListItem("<Select>", "0"));
        //            foreach (DocumentType dp in res.DocumentTypes)
        //            {
        //                ddlProjType.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
        //        Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
        //    }
        //}

        //protected void txtDoc_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        lblChrt2ErrMsg.Text = "";
        //        lblChrt2ErrMsg.Visible = false;

        //        //Bindchart();      
        //        if (txtDoc.Text.Trim() != "")
        //        {
        //            strTemp = BindSubChart_doc();
        //            if (strTemp.Trim().ToLower() == "no data")
        //            {
        //                lblChrt2ErrMsg.Visible = true;
        //                lblChrt2ErrMsg.Text = "No data found.";
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
        //    }
        //}

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            //if (e.Row.RowType == DataControlRowType.Footer)
            //{
            //    e.Row.Cells.RemoveAt(0);
            //    e.Row.Cells[0].ColumnSpan = 1;

            //}

        }

        //protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    GridViewRow row = GridView1.SelectedRow;
        //    Label lblUPLOADiDocType = (Label)row.FindControl("lblUPLOADiDocType");
        //    Label lblunique_barcode = (Label)row.FindControl("lbliDepartment");


        //    Bindchart_Grid(lblUPLOADiDocType.Text.Trim(), lblunique_barcode.Text.Trim());

        //}

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            //GridViewRow row = GridView1.SelectedRow;
            //Label lblUPLOADiDocType = (Label)row.FindControl("lblUPLOADiDocType");
            //Label lblunique_barcode = (Label)row.FindControl("lbliDepartment");


            //Bindchart_Grid(lblUPLOADiDocType.Text.Trim(), lblunique_barcode.Text.Trim());

            LinkButton lb = sender as LinkButton;


            GridViewRow Grow = (GridViewRow)lb.NamingContainer;
            Label lblUPLOADiDocType = (Label)Grow.FindControl("lblUPLOADiDocType");
            Label lbliDepartment = (Label)Grow.FindControl("lbliDepartment");

            Bindchart_Grid(lblUPLOADiDocType.Text.Trim(), lbliDepartment.Text.Trim());

            // TextBox txtnarration = (TextBox)Grow.FindControl("txtnarration");

            //foreach (GridViewRow row in GridView1.Rows)
            //{
            //    // Selects the text from the TextBox
            //    // which is inside the GridView control
            //    string textBoxText = ((TextBox)row.FindControl("TextBox1")).Text;

            //}
        }
    }
}