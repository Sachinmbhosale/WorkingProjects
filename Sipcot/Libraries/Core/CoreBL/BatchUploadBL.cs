using System;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;
using System.Data;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class BatchUploadBL
    {
        public Results ManageFilterOption(string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            BatchUploadDAL dal = new BatchUploadDAL();
            try
            {
                results = dal.ManageFilterOptions(action, loginOrgId, loginToken);
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "FAILED";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }

        public Results UploadBatchData(BatchUploadBE BatchUpload, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            BatchUploadDAL dal = new BatchUploadDAL();
            try
            {
                results = dal.UploadBatchData(BatchUpload, action, loginOrgId, loginToken);
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "FAILED";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }

        public Results GetBatchUploadData(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            BatchUploadDAL dal = new BatchUploadDAL();
            try
            {
                results = dal.GetBatchUploadedData(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }


        public DataSet ReportDocumentWise(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            DataSet ds = new DataSet();
            Results results = null;
            BatchUploadDAL dal = new BatchUploadDAL();
            try
            {
                ds = dal.ReportDocumentWise(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return ds;
        }

        public DataSet ReportDocumentTagWise(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            DataSet ds = new DataSet();
            Results results = null;
            BatchUploadDAL dal = new BatchUploadDAL();
            try
            {
                ds = dal.ReportDocumentTagWise(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            return ds;
        }

        public DataSet GetHeadersForBatchUpload(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            DataSet ds = new DataSet();
            Results results = null;
            BatchUploadDAL dal = new BatchUploadDAL();
            try
            {

                ds = dal.GetHeadersForBatchUpload(filter, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return ds;
        }
        public DataSet AuditReport(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            DataSet ds = new DataSet();
            Results results = null;
            BatchUploadDAL dal = new BatchUploadDAL();
            try
            {
                ds = dal.AuditReport(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return ds;
        }
        public DataSet ComboFillerBySP(string StoredProcedure)
        {
            DataSet ds = new DataSet();
            BatchUploadDAL dal = new BatchUploadDAL();
            Results results = null;
            try
            {
                ds = dal.ComboFillerBySP(StoredProcedure);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages("", results.ActionStatus, ex.ToString());
            }
            return ds;
        }

        public Results ManageUploadBatches(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            DataSet ds = new DataSet();
            Results results = null;
            BatchUploadDAL dal = new BatchUploadDAL();
            try
            {
                results = dal.ManageUploadBatches(filter, action, loginOrgId, loginToken);
            }
            catch 
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus);
            }
            return results;
        }
    }
}
