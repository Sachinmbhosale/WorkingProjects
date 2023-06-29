using System;
using System.Data.SqlClient;
using System.Data;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class DashboardDAL : BaseDAL
    {
        private SqlConnection con;
        string strQuery = "";

        private void fnConnection()
        {
            //string constr = ConfigurationManager.ConnectionStrings["DemoConnection"].ToString();

            con = new SqlConnection(DbConnectionString);
        }

        // Get Main chart data in Dashboard..

        

              public DataSet BindMainChartData()
        {
            SqlCommand cmd = null;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                fnConnection();
                cmd = new SqlCommand("GetMainChartData_Dashboard");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                con.Close();

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (da != null)
                {
                    da.Dispose();
                }
            }
        }
        public DataSet GetMainChartData(string strProj, string strDept)
        {
            SqlCommand cmd = null;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                fnConnection();
                cmd = new SqlCommand("GetDepwiseData_Dashboard");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Projtype", strProj);
                cmd.Parameters.AddWithValue("@Dept", strDept);
                cmd.Connection = con;
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                con.Close();

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (da != null)
                {
                    da.Dispose();
                }
            }
        }

        // Get Document wise chart data in Dashboard..
        public DataSet GetSubChartData(string strDocName)
        {
            SqlCommand cmd = null;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                fnConnection();
                cmd = new SqlCommand("GetDocChartData_Dashboard");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocName", strDocName);
                cmd.Connection = con;
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                con.Close();

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (da != null)
                {
                    da.Dispose();
                }
            }
        }


        public DataSet GetDashboardTotalCount(int iOrgid)
        {
            SqlCommand cmd = null;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                fnConnection();
                cmd = new SqlCommand("Dashboard_TotalCount");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocName", iOrgid);
                cmd.Connection = con;
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                con.Close();

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (da != null)
                {
                    da.Dispose();
                }
            }
        }
        //Get Project List
        public DataSet GetDashboardTotalCount_WithDateFilter(int iorgid, DateTime fromdate, DateTime todate)
        {
            SqlCommand cmd = null;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                fnConnection();
                cmd = new SqlCommand("Dashboard_TotalCount_datefilter");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fromdate", fromdate);
                cmd.Parameters.AddWithValue("@todate", todate);
                cmd.Connection = con;
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                con.Close();

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (da != null)
                {
                    da.Dispose();
                }
            }
        }
        public DataSet GetProjList(string strUserName)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = null;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                fnConnection();
                strQuery = "Select distinct O.Orgs_vName as ProjName from ORGS O inner join USERS U on O.Orgs_iId = U.USERS_iOrgId " +
                           "inner join UPLOAD UP on O.Orgs_iId = UP.UPLOAD_iOrgID " +
                           "where U.Users_vUserName = '" + strUserName + "' order by O.Orgs_vName ";
                
                cmd = new SqlCommand("GetProjList_Dashboard");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", strUserName);
                cmd.Connection = con;
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch
            {
                return ds;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (da != null)
                {
                    da.Dispose();
                }
            }
        }

        //Get Department List based on project Type.
        public DataSet GetDeptList(string strProjName)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = null;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                fnConnection();
                strQuery = "Select distinct D.Department_vName as Dept from Department D inner join ORGS O on O.Orgs_iId = D.Department_OrgId " +
                            "inner join UPLOAD UP on D.Department_iID = UP.UPLOAD_iDepartment " +
                            "where O.Orgs_vName = '" + strProjName + "' order by D.Department_vName ";
                
                cmd = new SqlCommand("GetDeptList_Dashboard");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProjName", strProjName);
                cmd.Connection = con;
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch
            {
                return ds;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (da != null)
                {
                    da.Dispose();
                }
            }
        }

        //Get Department List based on project Type.
        public DataSet GetDocList(string strDeptName)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = null;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                fnConnection();
                strQuery = " Select Distinct U.Upload_vOriginFileName as DocName from UPLOAD U inner join Department D on D.Department_iID = U.UPLOAD_iDepartment " +
                           " where D.Department_vName ='" + strDeptName + "' order by U.Upload_vOriginFileName";
                
                cmd = new SqlCommand("GetDoctList_Dashboard");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DeptName", strDeptName);
                cmd.Connection = con;
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch
            {
                return ds;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (da != null)
                {
                    da.Dispose();
                }
            }
        }
       
    }
}
