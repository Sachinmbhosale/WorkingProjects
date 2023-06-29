using System;
using DataAccessLayer;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;


namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class ReportDAL : BaseDAL
    {
        public ReportDAL() { }
        DataSet ds = new DataSet();
        public DataSet BillingSubReports(ReportBE report, int loginOrgId, string loginToken)
        {

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_iOrgID", report.OrgId);
                dbManager.AddParameters(1, "@in_dStartDate", report.CreatedDateFrom);
                dbManager.AddParameters(2, "@in_dEndDate", report.EndDate);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_SubMainReport");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return ds;
        }

        public DataSet BillingMainReports(ReportBE report, int loginOrgId, string loginToken)
        {

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_iOrgID", report.OrgId);
                dbManager.AddParameters(1, "@in_dStartDate", report.CreatedDateFrom);
                dbManager.AddParameters(2, "@in_dEndDate", report.EndDate);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_MainReport");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return ds;


        }

        public DataSet DocumentTypeGenerateReports(ReportBE report, int loginOrgId, string loginToken)
        {

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                dbManager.CreateParameters(7);
                dbManager.AddParameters(0, "@in_iOrgID", report.OrgId);
                dbManager.AddParameters(1, "@in_iDocTypeId", report.DocumentTypeReportID);
                dbManager.AddParameters(2, "@in_iActionID", report.DocumentTypeReportActionId);
                dbManager.AddParameters(3, "@in_dStartDate", report.CreatedDateFrom);
                dbManager.AddParameters(4, "@in_dEndDate", report.EndDate);
                dbManager.AddParameters(5, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(6, "@in_iLoginOrgId", loginOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_CustTypeDocTypeReport");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return ds;


        }
        public DataSet ExpiryReports(ReportBE report, int loginOrgId, string loginToken)
        {

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                dbManager.CreateParameters(6);
                dbManager.AddParameters(0, "@in_iOrgID", report.OrgId);
                dbManager.AddParameters(1, "@in_iUserId", 0);
                dbManager.AddParameters(2, "@in_dStartDate", report.CreatedDateFrom);
                dbManager.AddParameters(3, "@in_dEndDate", report.EndDate);
                dbManager.AddParameters(4, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(5, "@in_iLoginOrgId", loginOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_ExpiryReportFilter");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return ds;


        }
        public DataSet LogFormReports(ReportBE report, int loginOrgId, string loginToken)
        {

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                dbManager.CreateParameters(7);
                dbManager.AddParameters(0, "@in_iOrgID", 0);
                dbManager.AddParameters(1, "@in_iUserId", 0);
                dbManager.AddParameters(2, "@in_iActionID", 0);
                dbManager.AddParameters(3, "@in_dStartDate", report.CreatedDateFrom);
                dbManager.AddParameters(4, "@in_dEndDate", report.EndDate);
                dbManager.AddParameters(5, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(6, "@in_iLoginOrgId", loginOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Logreportfilter");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return ds;


        }
        public DataSet TagChangeReports(ReportBE report, int loginOrgId, string loginToken)
        {

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_iUserId", 0);
                dbManager.AddParameters(1, "@in_dStartDate", report.CreatedDateFrom);
                dbManager.AddParameters(2, "@in_dEndDate", report.EndDate);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_TagchangeLogReport");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return ds;


        }

    }
}
