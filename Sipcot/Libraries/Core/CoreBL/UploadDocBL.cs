
//  Date        Programmer        Issue           Description
//  16/07/2013	gokuldas           997           Viewed count is not reflecting in the log report


using System;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;
using System.Data;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class UploadDocBL
    {
        public UploadDocBL() { }


        #region GetUploadDocumentDetailswithProcessID
        public DataSet GetUploadDocumentDetails(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            return new UploadDocDAL().GetUploadDocumentDetails(filter, action, loginOrgId, loginToken);
        }
        #endregion
        #region DiscardUploadDocument
        public Results DiscardUploadDocument(SearchFilter filter, string action, string loginOrgId, string loginToken, string archiveAction)
        {
           return new UploadDocDAL().DiscardUploadDocument(filter, action, loginOrgId, loginToken, archiveAction);
        }
        #endregion
        #region UpdateTrackTableOnDownload
        public Results UpdateTrackTableOnDownload(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            return new UploadDocDAL().UpdateTrackTableOnDownload(filter, action, loginOrgId, loginToken);
        }
        #endregion

        //997BS
        #region UpdateTrackTableOnviewd
        public Results UpdateTrackTableOnviewd(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            return new UploadDocDAL().UpdateTrackTableOnviewd(filter, action, loginOrgId, loginToken);
        }
        #endregion
        //997BE

        #region UpdateDocumentDetails
        public void UpdateDocumentDetails(int ProcessId, string ArchivePath, string loginOrgId, string loginToken, string archiveAction)
        {

            UploadDocDAL dal = new UploadDocDAL();
            try
            {
                dal.UpdateDocumentDetails(ProcessId, ArchivePath, loginOrgId, loginToken, archiveAction);
            }
            catch (Exception)
            {
                // results = new Results();
                //results.ActionStatus = "ERROR";
                //results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            //  return results;
        }
        #endregion

        //Newly added for Enchment4
        /// <summary>
        /// To lock the Document on editing
        /// </summary>
        /// <param name="action"></param>
        /// <param name="processId"></param>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <param name="ErrorState"></param>
        /// <param name="ErrorSeverity"></param>
        /// <returns></returns>
        public DataSet UpdateDocumentStatusForLock(string action, int processId, int userId, string token, int orgId, out string message, out int ErrorState, out int ErrorSeverity)
        {
            return new UploadDocDAL().UpdateDocumentStatusForLock(action, processId, userId, token, orgId, out message, out ErrorState, out ErrorSeverity);
        }

    }
}
