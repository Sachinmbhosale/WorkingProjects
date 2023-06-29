using System;
using System.Data;
using Lotex.EnterpriseSolutions.CoreDAL;


namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class DashboardBAL
    {
        DashboardDAL objDal = new DashboardDAL();

        public DataSet FillProjectList(string strLoginUser)
        {
            DataSet dsData = new DataSet();
            try
            {
                dsData = objDal.GetProjList(strLoginUser);                              
            }
            catch (Exception ex) 
            {
               
            }
            return dsData;
        }
        
        public DataSet FillDeptList(string ProjName)
        {
            DataSet dsData = new DataSet();
            try
            {
                dsData = objDal.GetDeptList(ProjName);
            }
            catch (Exception ex)
            {

            }
            return dsData;
        }

        public DataSet FillDocList(string DeptName)
        {
            DataSet dsData = new DataSet();
            try
            {
                dsData = objDal.GetDocList(DeptName);
            }
            catch (Exception ex)
            {

            }
            return dsData;
        }
        

             public DataSet BindMainChartData()
        {
            DataSet dsData = new DataSet();
            try
            {
                dsData = objDal.BindMainChartData();
            }
            catch (Exception ex)
            {

            }
            return dsData;
        }
        public DataSet MainChartData(string ProjName, string Dept)
        {
            DataSet dsData = new DataSet();
            try
            {
                dsData = objDal.GetMainChartData(ProjName, Dept);
            }
            catch (Exception ex)
            {

            }
            return dsData;
        }

        public DataSet SubChartData(string DocName)
        {
            DataSet dsData = new DataSet();
            try
            {
                dsData = objDal.GetSubChartData(DocName);
            }
            catch (Exception ex)
            {

            }
            return dsData;
        }

        public DataSet GetDashboardTotalCount(int iorgid)
        {
            DataSet dsData = new DataSet();
            try
            {
                dsData = objDal.GetDashboardTotalCount(iorgid);
            }
            catch (Exception ex)
            {

            }
            return dsData;
        }
        public DataSet GetDashboardTotalCount_WithDateFilter(int iorgid, DateTime fromdate, DateTime todate)
        {
            DataSet dsData = new DataSet();
            try
            {
                dsData = objDal.GetDashboardTotalCount_WithDateFilter(iorgid, fromdate, todate);
            }
            catch (Exception ex)
            {

            }
            return dsData;
        }
    }
}
