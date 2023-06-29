/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * ----------       -------------       ----------          ------------------------------------------------------------------
 * 16/4/2015      sabina                DMS5-3935           Login management:redirecting to either workflow home page or DMS based on application access
 * 27/06/2015     GokulDas palapatta    DMS5-4381           Dashboard & Reporting
====================================================================================================================================== */

using System;
using System.Web;
using System.Web.UI.WebControls;
using WorkflowBAL;
using WorkflowBLL.Classes;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class WorkFlowHome : PageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            //DMS5-3935A
            MasterPage = "~/Workflow/WorkflowMaster/WorkflowAdmin.Master";
            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Home";
            //End: ------------------SitePath Details ------------------------
            if (!IsPostBack)
            {
                IntitialLoadGrid();
            }

        }

        #region Initial Loads
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iDt"></param>
        /// <param name="action"></param>
        /// <returns>To Filter Data According To Process workflow and Satges</returns>
        /// 
        private DataTable Getgriddata(DataTable iDt, string action)
        {

            DataTable dt = new DataTable();

            DataView dv = new DataView(iDt);
            dv.RowFilter = ("[UserType] = '" + action + "'");

            dt = dv.ToTable();


            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iDt"></param>
        /// <param name="ID"></param>
        /// <param name="Action"></param>
        /// <returns>To get the stage as well as grid data in seperate grid</returns>
        private DataTable GetStageGriddata(DataTable iDt, string ID, string Action)
        {

            DataTable dt = new DataTable();

            DataView dv = new DataView(iDt);
            if (Action == "Stage")
            {
                dv.RowFilter = ("[WorkflowId] = '" + ID + "'");
            }
            else if (Action == "WorkFlow")
            {
                dv.RowFilter = ("[ProcessId] = '" + ID + "'");
            }

            dt = dv.ToTable();


            return dt;
        }



        /// <summary>
        /// to get workflow and process name for binding to workflow linkbutton
        /// </summary>
        /// <param name="iDt"></param>
        /// <param name="ID"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        /// 
        protected void GetWorkFlowOrProcessName(DataTable iDt, string ID, string Action)
        {
            if (iDt.Rows.Count > 0)
            {
                string Name = string.Empty;
                DataTable dt = new DataTable();

                DataView dv = new DataView(iDt);

                if (Action == "LoadWorkFlow")
                {
                    dv.RowFilter = ("[ProcessId] = '" + ID + "'");
                    dt = dv.ToTable();
                    hdnProcessName.Value = dt.Rows[0]["Process Name"].ToString();
                    //hdnProcessId.Value = dt.Rows[0]["ProcessId"].ToString();
                    hdnWorkFlowName.Value = dt.Rows[0]["Workflow Name"].ToString();
                    //hdnWorkflowId.Value = dt.Rows[0]["WorkflowId"].ToString();
                }
                else if (Action == "LoadStage")
                {
                    dv.RowFilter = ("[WorkflowId] = '" + ID + "'");
                    dt = dv.ToTable();
                    hdnProcessName.Value = dt.Rows[0]["Process Name"].ToString();

                    hdnWorkFlowName.Value = dt.Rows[0]["Workflow Name"].ToString();
                    //hdnProcessId.Value = dt.Rows[0]["ProcessId"].ToString();
                    //hdnWorkflowId.Value = dt.Rows[0]["WorkflowId"].ToString();
                }

                else if (Action == "LoadStatus")
                {
                    dv.RowFilter = ("[StageId] = '" + ID + "'");
                    dt = dv.ToTable();
                    hdnProcessName.Value = dt.Rows[0]["Process Name"].ToString();
                    //  hdnProcessId.Value = dt.Rows[0]["ProcessId"].ToString();
                    // hdnWorkflowId.Value = dt.Rows[0]["WorkflowId"].ToString();
                    hdnWorkFlowName.Value = dt.Rows[0]["Workflow Name"].ToString();
                    hdnStageName.Value = dt.Rows[0]["Stage Name"].ToString();
                    //hdnStageId.Value = dt.Rows[0]["StageId"].ToString();

                }



            }



        }

        /// <summary>
        /// For Loading Controls Initially
        /// </summary>
        /// <param name="DT"></param>
        /// <param name="Action"></param>
        protected void LoadControls(DataTable DT, string Action)
        {
          
            if (DT.Rows.Count > 0)
            {
                if (Action == "PO")
                {
                    Panel Main = new Panel();
                    Main.CssClass = "GVDiv";
                    Panel Link = CreateLinKs("");
                    Panel Data = new Panel();
                    GridView grd = new GridView();
                    grd.RowDataBound += new GridViewRowEventHandler(Gv_ProcessRowDataBound);
                    grd.DataSource = DT;
                    grd.CssClass = "mGrid";

                    grd.DataBind();
                    Data.Controls.Add(grd);
                    Main.Controls.Add(Link);
                    Main.Controls.Add(Data);
                    MainPanel.Controls.Add(Main);

                }
                else if (Action == "WO")
                {
                    DataView WOview = new DataView(DT);
                    DataTable WOdistinctValues = WOview.ToTable(true, "ProcessId");
                    for (int i = 0; i < WOdistinctValues.Rows.Count; i++)
                    {
                        Panel Main = new Panel();
                        Main.CssClass = "GVDiv";
                        //  SetID(DT, Convert.ToString(WOdistinctValues.Rows[i][0]), "WorkFlowLoad");
                        GetWorkFlowOrProcessName(DT, Convert.ToString(WOdistinctValues.Rows[i][0]), "LoadWorkFlow");
                        Panel Link = CreateLinKs("GoBackFromWorkFlow");

                        Panel Data = new Panel();
                        GridView grd = new GridView();
                        grd.RowDataBound += new GridViewRowEventHandler(Gv_WorkFlowRowDataBound);
                        grd.DataSource = GetStageGriddata(DT, Convert.ToString(WOdistinctValues.Rows[i][0]), "WorkFlow");

                        grd.CssClass = "mGrid";

                        grd.DataBind();
                        Data.Controls.Add(grd);
                        Main.Controls.Add(Link);
                        Main.Controls.Add(Data);
                        MainPanel.Controls.Add(Main);
                        //CreateLinkButton("GoToProcess");
                    }
                }
                else if (Action == "SO")
                {
                    DataView SOview = new DataView(DT);
                    DataTable SOdistinctValues = SOview.ToTable(true, "WorkflowId");


                    for (int i = 0; i < SOdistinctValues.Rows.Count; i++)
                    {
                        // SetID(DT, Convert.ToString(SOdistinctValues.Rows[i][0]), "StageLoad");
                        GetWorkFlowOrProcessName(DT, Convert.ToString(SOdistinctValues.Rows[i][0]), "LoadStage");

                        Panel Main = new Panel();
                        Main.CssClass = "GVDiv";
                        Panel Link = CreateLinKs("GoBackFromStage");
                        Panel Data = new Panel();
                        GridView grd = new GridView();

                        grd.RowDataBound += new GridViewRowEventHandler(Gv_StagesDataBound);
                        grd.DataSource = GetStageGriddata(DT, Convert.ToString(SOdistinctValues.Rows[i][0]), "Stage");
                        grd.CssClass = "mGrid";

                        grd.DataBind();
                        //StatusLinkDiv.Controls.Add(grd);
                        //grd.DataBind();
                        Data.Controls.Add(grd);
                        Main.Controls.Add(Link);
                        Main.Controls.Add(Data);
                        MainPanel.Controls.Add(Main);
                    }
                }
            }
        }
        /// <summary>
        /// for clearing  the hidden variables
        /// </summary>
        protected void ClearAllHiddenVatiables()
        {
            hdnProcessId.Value = "";

            hdnWorkflowId.Value = "";
            hdnProcessName.Value = "";
            hdnWorkFlowName.Value = "";
            hdnStageName.Value = "";
            hdnStageId.Value = "";

        }
        /// <summary>
        /// used for initially loading all data
        /// </summary>
        protected void IntitialLoadGrid()
        {
            try
            {
                Logger.Trace("IntitialLoadGrid Started:-", Session["LoggedUserId"].ToString());
                MainPanel.InnerHtml = "";


                WorkFlowHomePageBLL objStatus = new WorkFlowHomePageBLL();

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStatus.LoginToken = loginUser.LoginToken;
                objStatus.LoginOrgId = loginUser.LoginOrgId;
                objResult = objStatus.ManageWorkflowDashBoard(objStatus, "ALL");
                int count = objResult.dsResult.Tables.Count;

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            DataTable PDT = Getgriddata(objResult.dsResult.Tables[i], "PO");
                            LoadControls(PDT, "PO");
                            DataTable WDT = Getgriddata(objResult.dsResult.Tables[i], "WO");
                            LoadControls(WDT, "WO");
                            DataTable SDT = Getgriddata(objResult.dsResult.Tables[i], "SO");
                            LoadControls(SDT, "SO");
                        }
                        catch
                        {

                        }
                    }

                    Logger.Trace("IntitialLoadGrid Completed:-", Session["LoggedUserId"].ToString());
                }
                ClearAllHiddenVatiables();
            }
            catch (Exception ex)
            {
                Logger.Trace("IntitialLoadGrid Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }


            

        }

        /// <summary>
        /// to clear all controls
        /// </summary>
        protected void ClearControls()
        {
            MainPanel.InnerHtml = "";
        }

        /// <summary>
        /// for creating links
        /// </summary>
        /// <param name="Action"></param>
        /// <returns></returns>
        protected Panel CreateLinKs(string Action)
        {
            Panel LinkPanel = new Panel();
            try
            {
                Logger.Trace("CreateLinKs started", Session["LoggedUserId"].ToString());
                Label lblSeperator = new Label() { Text = " > " };
                Label lblSeperator1 = new Label() { Text = " > " };
                Label lblSeperator2 = new Label() { Text = " > " };
                Label lblSeperator3 = new Label() { Text = " > " };
                LinkButton lnkStages = new LinkButton();
                LinkButton lnkprocess = new LinkButton();
                LinkButton lnkworkFlow = new LinkButton();
                LinkButton lnkStatus = new LinkButton();
                lnkStages.CssClass = "Linkbutton";
                lnkprocess.CssClass = "Linkbutton";
                lnkworkFlow.CssClass = "Linkbutton";
                lnkStatus.CssClass = "Linkbutton";
                string processID = hdnProcessId.Value = hdnProcessId.Value.Length > 0 ? hdnProcessId.Value : hdnProcessId.Value = "0";
                string WorkFlowId = hdnWorkflowId.Value = hdnWorkflowId.Value.Length > 0 ? hdnWorkflowId.Value : hdnWorkflowId.Value = "0";
                string stageId = hdnStageId.Value = hdnStageId.Value.Length > 0 ? hdnStageId.Value : hdnStageId.Value = "0";
                string ProcessName = hdnProcessName.Value = hdnProcessName.Value.Length > 0 ? hdnProcessName.Value : hdnProcessName.Value = "0";
                string workFlowName = hdnWorkFlowName.Value = hdnWorkFlowName.Value.Length > 0 ? hdnWorkFlowName.Value : hdnWorkFlowName.Value = "0";
                string StageName = hdnStageName.Value = hdnStageName.Value.Length > 0 ? hdnStageName.Value : hdnStageName.Value = "0";




                if (Action == "GoBackFromWorkFlow")
                {

                    lnkprocess.Text = hdnProcessName.Value;
                    lnkworkFlow.Text = "WorkFlow(S)";
                    if (hdnProcessId.Value.Length > 0 && hdnProcessName.Value.Length > 0)
                    {
                        lnkprocess.Attributes.Add("onClick", string.Format("javascript:GetProcess(" + hdnProcessId.Value + ",'" + hdnProcessName.Value + "');") + "  return false;");

                    }
                    else
                    {
                        hdnProcessId.Value = hdnProcessId.Value.Length > 0 ? hdnProcessId.Value : hdnProcessId.Value = "";
                        hdnProcessName.Value = hdnProcessName.Value.Length > 0 ? hdnProcessName.Value : hdnProcessName.Value = "";

                        lnkprocess.Attributes.Add("onClick", string.Format("javascript:GetProcess(" + hdnProcessId.Value + ",'" + hdnProcessName.Value + "');") + "  return false;");
                    }
                    lnkworkFlow.Attributes.Add("onClick", "return false;");
                    //LinkPanel.Controls.Add(lblSeperator);
                    LinkPanel.Controls.Add(lnkprocess);
                    LinkPanel.Controls.Add(lblSeperator1);
                    LinkPanel.Controls.Add(lnkworkFlow);
                }

                else if (Action == "GoBackFromStage")
                {


                    if (hdnProcessId.Value.Length > 0 && hdnProcessName.Value.Length > 0 && hdnWorkFlowName.Value.Length > 0 && hdnWorkflowId.Value.Length > 0)
                    {


                        lnkprocess.Attributes.Add("onClick", string.Format("javascript:GetProcess(" + processID + ",'" + ProcessName + "');") + "  return false;");
                        lnkworkFlow.Attributes.Add("onClick", string.Format("javascript:GetWorkFlow(" + processID + ",'" + ProcessName + "');") + "  return false;");
                        lnkprocess.Text = hdnProcessName.Value;
                        lnkworkFlow.Text = hdnWorkFlowName.Value;
                        lnkStages.Text = "Stage(S)";
                        lnkStages.Attributes.Add("onClick", "return false;");
                    }
                    else
                    {
                        lnkStages.Text = "Stage(S)";
                        lnkprocess.Text = hdnProcessName.Value;
                        lnkworkFlow.Text = hdnWorkFlowName.Value;

                        lnkworkFlow.Attributes.Add("onClick", string.Format("javascript:GetProcess(" + processID + ",'" + ProcessName + "');") + "  return false;");
                        lnkprocess.Attributes.Add("onClick", string.Format("javascript:GetProcess(" + processID + ",'" + ProcessName + "');") + "  return false;");
                        lnkStages.Attributes.Add("onClick", "return false;");
                    }

                    //LinkPanel.Controls.Add(lblSeperator);
                    LinkPanel.Controls.Add(lnkprocess);
                    LinkPanel.Controls.Add(lblSeperator1);
                    LinkPanel.Controls.Add(lnkworkFlow);
                    LinkPanel.Controls.Add(lblSeperator2);
                    LinkPanel.Controls.Add(lnkStages);
                }
                else if (Action == "GoBackFromStatus")
                {


                    if (hdnWorkflowId.Value.Length > 0 && hdnWorkFlowName.Value.Length > 0 && hdnProcessId.Value.Length > 0 && hdnProcessName.Value.Length > 0 && hdnStageName.Value.Length > 0 && hdnStageId.Value.Length > 0)
                    {

                        //lnkStages.Text = hdnStageName.Value + " >>" + " Status(s)";
                        lnkprocess.Text = hdnProcessName.Value;
                        lnkworkFlow.Text = hdnWorkFlowName.Value;
                        lnkStages.Text = hdnStageName.Value;
                        lnkStatus.Text = " Status(S)";
                        lnkprocess.Attributes.Add("onClick", string.Format("javascript:GetProcess(" + processID + ",'" + ProcessName + "');") + "  return false;");
                        lnkworkFlow.Attributes.Add("onClick", string.Format("javascript:GetWorkFlow(" + processID + ",'" + ProcessName + "');") + "  return false;");
                        lnkStages.Attributes.Add("onClick", string.Format("javascript:GetStages(" + WorkFlowId + ",'" + workFlowName + "');") + "  return false;");
                        lnkStatus.Attributes.Add("onClick", "return false;");
                    }
                    else
                    {

                        lnkprocess.Text = hdnProcessName.Value;
                        lnkStages.Text = hdnStageName.Value;
                        lnkStatus.Text = " Status(S)";
                        lnkworkFlow.Text = hdnWorkFlowName.Value;


                        if (hdnProcessId.Value.Length > 0 && hdnProcessName.Value.Length > 0)
                        {
                            lnkprocess.Attributes.Add("onClick", string.Format("javascript:GetProcess(" + processID + ",'" + ProcessName + "');") + "  return false;");
                            lnkworkFlow.Attributes.Add("onClick", string.Format("javascript:GetWorkFlow(" + processID + ",'" + ProcessName + "');") + "  return false;");
                        }
                        else
                        {
                            lnkprocess.Attributes.Add("onClick", string.Format("javascript:GetProcess(" + processID + ",'" + ProcessName + "');") + "  return false;");
                            lnkworkFlow.Attributes.Add("onClick", string.Format("javascript:GetProcess(" + processID + ",'" + ProcessName + "');") + "  return false;");
                        }
                        if (hdnWorkflowId.Value.Length > 0 && hdnWorkFlowName.Value.Length > 0)
                        {
                            lnkStages.Attributes.Add("onClick", string.Format("javascript:GetStages(" + WorkFlowId + ",'" + workFlowName + "');") + "  return false;");
                        }
                        else
                        {
                            lnkStages.Attributes.Add("onClick", string.Format("javascript:GetProcess(" + WorkFlowId + ",'" + workFlowName + "');") + "  return false;");
                        }
                        lnkStatus.Attributes.Add("onClick", "return false;");
                        //   lnkStages.Attributes.Add("onClick", "return false;");
                    }
                    //LinkPanel.Controls.Add(lblSeperator);
                    LinkPanel.Controls.Add(lnkprocess);
                    LinkPanel.Controls.Add(lblSeperator1);
                    LinkPanel.Controls.Add(lnkworkFlow);
                    LinkPanel.Controls.Add(lblSeperator2);
                    LinkPanel.Controls.Add(lnkStages);
                    LinkPanel.Controls.Add(lblSeperator3);
                    LinkPanel.Controls.Add(lnkStatus);
                }
                else
                {

                    lnkprocess.Text = "Process(s)";
                    lnkprocess.Attributes.Add("onClick", "return false;");
                    //LinkPanel.Controls.Add(lblSeperator);
                    LinkPanel.Controls.Add(lnkprocess);
                }

            }
            catch (Exception ex)
            {
                Logger.Trace("CreateLinKs Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return LinkPanel;
        }


        #endregion  Initial Loads

        #region Loading Grids For Second Time

        protected void LoadWorkFlowGrid()
        {
            Panel Main = new Panel();
            Main.CssClass = "GVDiv";

            Panel Data = new Panel();

            WorkFlowHomePageBLL objStatus = new WorkFlowHomePageBLL();

            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStatus.LoginToken = loginUser.LoginToken;
            objStatus.LoginOrgId = loginUser.LoginOrgId;
            objStatus.ProcessId = Convert.ToInt32(hdnProcessId.Value);

            objResult = objStatus.ManageWorkflowDashBoard(objStatus, "GetWorkFlow");
            if (objResult.dsResult.Tables[0].Rows.Count > 0)
            {
                GetWorkFlowOrProcessName(objResult.dsResult.Tables[0], hdnProcessId.Value, "LoadWorkFlow");
                Panel Link = CreateLinKs("GoBackFromWorkFlow");
                GridView grd = new GridView();
                grd.RowDataBound += new GridViewRowEventHandler(Gv_WorkFlowRowDataBound);
                grd.DataSource = objResult.dsResult.Tables[0];
                grd.CssClass = "mGrid";

                grd.DataBind();

                Data.Controls.Add(grd);
                Main.Controls.Add(Link);
                Main.Controls.Add(Data);
                MainPanel.Controls.Add(Main);
            }
            else
            {
                ClearAllHiddenVatiables();
                IntitialLoadGrid();
            }

        }

        /// <summary>
        /// loading all grids only from second time
        /// </summary>
        protected void LoadStageGrid()
        {
            Panel Main = new Panel();
            Main.CssClass = "GVDiv";

            Panel Data = new Panel();
            WorkFlowHomePageBLL objStatus = new WorkFlowHomePageBLL();


            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStatus.LoginToken = loginUser.LoginToken;
            objStatus.LoginOrgId = loginUser.LoginOrgId;
            objStatus.WorkFlowId = Convert.ToInt32(hdnWorkflowId.Value);
            objResult = objStatus.ManageWorkflowDashBoard(objStatus, "GetStages");

            if (objResult.dsResult.Tables[0].Rows.Count > 0)
            {
                GetWorkFlowOrProcessName(objResult.dsResult.Tables[0], hdnWorkflowId.Value, "LoadStage");
                Panel Link = CreateLinKs("GoBackFromStage");
                GridView grd = new GridView();
                grd.RowDataBound += new GridViewRowEventHandler(Gv_StagesDataBound);
                grd.DataSource = objResult.dsResult.Tables[0];
                grd.CssClass = "mGrid";

                grd.DataBind();
                Data.Controls.Add(grd);
                Main.Controls.Add(Link);
                Main.Controls.Add(Data);
                MainPanel.Controls.Add(Main);
            }
            else
            {
                ClearAllHiddenVatiables();
                IntitialLoadGrid();
            }

        }

        protected void LoadStatusGrid()
        {
            Panel Main = new Panel();
            //Panel Link = CreateLinKs("GoBackFromSatus");
            Panel Data = new Panel();
            Main.CssClass = "GVDiv";
            WorkFlowHomePageBLL objStatus = new WorkFlowHomePageBLL();

            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStatus.LoginToken = loginUser.LoginToken;
            objStatus.LoginOrgId = loginUser.LoginOrgId;
            objStatus.StageId = Convert.ToInt32(hdnStageId.Value);
            objResult = objStatus.ManageWorkflowDashBoard(objStatus, hdnAction.Value);
            if (objResult.dsResult.Tables[0].Rows.Count > 0)
            {
                GetWorkFlowOrProcessName(objResult.dsResult.Tables[0], hdnStageId.Value, "LoadStatus");
                Panel Link = CreateLinKs("GoBackFromStatus");
                GridView grd = new GridView();
                grd.RowDataBound += new GridViewRowEventHandler(Gv_StatusDataBound);
                grd.DataSource = objResult.dsResult.Tables[0];
                grd.CssClass = "mGrid";

                grd.DataBind();
                Data.Controls.Add(grd);
                Main.Controls.Add(Link);
                Main.Controls.Add(Data);
                MainPanel.Controls.Add(Main);

                if (objResult.dsResult.Tables.Count > 1)
                {
                    if (objResult.dsResult.Tables[1].Rows.Count > 0)
                    {

                        Panel DetailsData = new Panel();
                        Panel DMain = new Panel();
                        DetailsData.CssClass = "GVDiv";

                        GridView DetailsGrid = new GridView();
                        // DMain.RowDataBound += new GridViewRowEventHandler(Gv_StatusDataBound);
                        DetailsGrid.DataSource = objResult.dsResult.Tables[1];
                        DetailsGrid.CssClass = "mGrid";
                        DetailsGrid.DataBind();
                        DetailsData.Controls.Add(DetailsGrid);


                        DMain.Controls.Add(DetailsData);
                        MainPanel.Controls.Add(DMain);

                    }
                }


            }
            else
            {
                ClearAllHiddenVatiables();
                IntitialLoadGrid();
            }
        }

        protected void Gv_ProcessRowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Do whatever you want in here.
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {

                e.Row.Cells[0].Visible = false;
                e.Row.Cells[3].Visible = false;
                if (i == 1)
                {
                    string id = e.Row.Cells[0].Text;
                    string text = e.Row.Cells[1].Text;
                    e.Row.Cells[1].Attributes["onclick"] = string.Format("javascript:GetWorkFlow(" + id + ",'" + text + "');", e.Row.RowIndex);
                    // e.Row.Attributes["onclick"] = string.Format("javascript:GetWorkFlow(" + id +");", e.Row.RowIndex);
                }

            }

        }

        protected void Gv_WorkFlowRowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Do whatever you want in here.
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {

                e.Row.Cells[0].Visible = false;
                e.Row.Cells[1].Visible = false;
                e.Row.Cells[4].Visible = false;
                e.Row.Cells[5].Visible = false;
                //string encoded = e.Row.Cells[i].Text;
                if (i == 1)
                {
                    string id = e.Row.Cells[1].Text;
                    string text = e.Row.Cells[2].Text;
                    e.Row.Cells[2].Attributes["onclick"] = string.Format("javascript:GetStages(" + id + ",'" + text + "');", e.Row.RowIndex);
                    // e.Row.Attributes["onclick"] = string.Format("javascript:GetWorkFlow(" + id +");", e.Row.RowIndex);
                }
                //e.Row.Cells[i].Text = Context.Server.HtmlDecode(encoded);
            }

        }

        protected void Gv_StagesDataBound(object sender, GridViewRowEventArgs e)
        {
            //Do whatever you want in here.
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {

                e.Row.Cells[0].Visible = false;
                e.Row.Cells[1].Visible = false;
                e.Row.Cells[2].Visible = false;
                e.Row.Cells[5].Visible = false;
                e.Row.Cells[6].Visible = false;
                e.Row.Cells[7].Visible = false;
                //string encoded = e.Row.Cells[i].Text;
                if (i == 1)
                {
                    string id = e.Row.Cells[2].Text;
                    string text = e.Row.Cells[3].Text;
                    e.Row.Cells[3].Attributes["onclick"] = string.Format("javascript:GetStatus(" + id + ",'" + text + "');", e.Row.RowIndex);
                    // e.Row.Attributes["onclick"] = string.Format("javascript:GetWorkFlow(" + id +");", e.Row.RowIndex);
                }
                //e.Row.Cells[i].Text = Context.Server.HtmlDecode(encoded);
            }

        }

        protected void Gv_StatusDataBound(object sender, GridViewRowEventArgs e)
        {
            //Do whatever you want in here.
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {

                e.Row.Cells[0].Visible = false;
                e.Row.Cells[1].Visible = false;
                e.Row.Cells[2].Visible = false;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[6].Visible = false;
                e.Row.Cells[7].Visible = false;
                e.Row.Cells[8].Visible = false;
                e.Row.Cells[9].Visible = false;
                e.Row.Cells[10].Visible = false;
                string DataEntryMode = e.Row.Cells[6].Text;
                string DataId = e.Row.Cells[7].Text;
                string ProcessId = e.Row.Cells[0].Text;
                string WorkFlowID = e.Row.Cells[1].Text;
                string StageId = e.Row.Cells[2].Text;
                e.Row.Cells[4].Attributes["onclick"] = string.Format("javascript:GetDataEntry(" + DataId + ",'" + DataEntryMode + "' " + "," + ProcessId + "," + WorkFlowID + "," + StageId + ");", e.Row.RowIndex);
            }

        }


        #endregion Loading Grids For Second Time

        #region Handling Reloading

        /// <summary>
        /// a java script function will be called and handled the post back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            try
            {
                ClearControls();
                if (hdnAction.Value == "GetWorkFlow")
                {
                    LoadWorkFlowGrid();
                }
                else if (hdnAction.Value == "GetStages")
                {
                    LoadStageGrid();
                }
                else if (hdnAction.Value == "GetStatus")
                {
                    LoadStatusGrid();
                }
                else if (hdnAction.Value == "GetProcess")
                {
                    ClearAllHiddenVatiables();
                    IntitialLoadGrid();
                }
                else if (hdnAction.Value == "RedirectToDataEntry")
                {
                    RedirectToDataEntry();
                }
            }
            catch(Exception ex)
            {
            }
        }

        #endregion Handling Reloading

        #region Handling Redirection

        protected void RedirectToDataEntry()
        {
            string processId = HttpUtility.UrlEncode(Encrypt(hdnProcessId.Value.Length > 0 ? hdnProcessId.Value : ""));
            string workflowId = HttpUtility.UrlEncode(Encrypt(hdnWorkflowId.Value.Length > 0 ? hdnWorkflowId.Value : ""));
            string stageId = HttpUtility.UrlEncode(Encrypt(hdnStageId.Value.Length > 0 ? hdnStageId.Value : ""));

            string feildDataId = HttpUtility.UrlEncode(Encrypt(hdnDataId.Value));
            string EdataEntryType = HttpUtility.UrlEncode(Encrypt(hdnDataEntryType.Value));
            string dataEntryType = hdnDataEntryType.Value;
            //DMS5-3457A check the type of dataentry and divert based on it                    
            string URL = "?ProcessId=" + processId + "&WorkflowId=" + workflowId + "&StageId=" + stageId + "&FieldDataId=" + feildDataId + "&DataEntryType=" + EdataEntryType;


            if (dataEntryType == "Normal")
            {
                Response.Redirect("~/Workflow/WorkflowAdmin/WorkflowDataEntry.aspx" + URL, false);
            }

            // If Fields or co-ordinates not available do not redirect

            else if (dataEntryType == "Guided")
            {
                Response.Redirect("~/Workflow/WorkflowAdmin/ManageGuidedDataEntry.aspx" + URL, false);
            }
            else if (dataEntryType == "Form")
            {
                Response.Redirect("~/Workflow/WorkflowAdmin/ManageFormDataEntry.aspx" + URL, false);
            }

        }

        #endregion Handling Redirection
    }
}