using System;
using System.Web.UI.WebControls;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class GridTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {


                DashboardBAL objTChartData = new DashboardBAL();
                DataSet dsChartData = new DataSet();
                dsChartData = objTChartData.GetDashboardTotalCount(304);

                GridView1.DataSource = dsChartData;
                GridView1.DataBind();

                DataTable taskTable = dsChartData.Tables[0];

                Session["TaskTable"] = taskTable;
            }
        }

        
        //public class trafficSourceData
        //{
        //    public string label { get; set; }
        //    public string value { get; set; }
        //    public string color { get; set; }
        //    public string hightlight { get; set; }
        //}


       // [WebMethod]
        //public List<object> getLineChartData(string mobileId_one, string mobileId_two, string year)
        //{
        //    List<object> iData = new List<object>();
        //    List<string> labels = new List<string>();
        //    //First get distinct Month Name for select Year.
        //    string query1 = "Select distinct( DateName( month , DateAdd( month , DATEPART(MONTH,orders_date) , -1 ) )) as month_name, ";
        //    query1 += " DATEPART(MONTH,orders_date) as month_number from mobile_sales  where DATEPART(YEAR,orders_date)='" + year + "'  ";
        //    query1 += " order by month_number;";

        //    DataTable dtLabels = commonFuntionGetData(query1);
        //    foreach (DataRow drow in dtLabels.Rows)
        //    {
        //        labels.Add(drow["month_name"].ToString());
        //    }
        //    iData.Add(labels);

        //    string query_DataSet_1 = " select DATENAME(MONTH,DATEADD(MONTH,month(orders_date),-1 )) as month_name, month(orders_date) as month_number ,sum ";
        //    query_DataSet_1 += " (orders_quantity) as total_quantity  from mobile_sales  ";
        //    query_DataSet_1 += " where YEAR(orders_date)='" + year + "' and  mobile_id='" + mobileId_one + "'  ";
        //    query_DataSet_1 += " group by   month(orders_date) ";
        //    query_DataSet_1 += " order by  month_number  ";

        //    DataTable dtDataItemsSets_1 = commonFuntionGetData(query_DataSet_1);
        //    List<int> lst_dataItem_1 = new List<int>();
        //    foreach (DataRow dr in dtDataItemsSets_1.Rows)
        //    {
        //        lst_dataItem_1.Add(Convert.ToInt32(dr["total_quantity"].ToString()));
        //    }
        //    iData.Add(lst_dataItem_1);

        //    string query_DataSet_2 = " select DATENAME(MONTH,DATEADD(MONTH,month(orders_date),-1 )) as month_name, month(orders_date) as month_number ,sum ";
        //    query_DataSet_2 += " (orders_quantity) as total_quantity  from mobile_sales  ";
        //    query_DataSet_2 += " where YEAR(orders_date)='" + year + "' and  mobile_id='" + mobileId_two + "'  ";
        //    query_DataSet_2 += " group by   month(orders_date) ";
        //    query_DataSet_2 += " order by  month_number  ";

        //    DataTable dtDataItemsSets_2 = commonFuntionGetData(query_DataSet_2);
        //    List<int> lst_dataItem_2 = new List<int>();
        //    foreach (DataRow dr in dtDataItemsSets_2.Rows)
        //    {
        //        lst_dataItem_2.Add(Convert.ToInt32(dr["total_quantity"].ToString()));
        //    }
        //    iData.Add(lst_dataItem_2);
        //    return iData;
        //}

       
        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            //Retrieve the table from the session object.
            DataTable dt = Session["TaskTable"] as DataTable;

          
            if (dt != null)
            {

                //Sort the data.
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                GridView1.DataSource = Session["TaskTable"];
                GridView1.DataBind();
            }
        }

        private string GetSortDirection(string column)
        {

            // By default, set the sort direction to ascending.
            string sortDirection = "ASC";

            // Retrieve the last column that was sorted.
            string sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                // Check if the same column is being sorted.
                // Otherwise, the default value can be returned.
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            // Save new values in ViewState.
            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
           
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }
    }
}