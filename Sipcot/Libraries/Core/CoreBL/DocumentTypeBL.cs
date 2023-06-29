using System;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class DocumentTypeBL
    {
        public DocumentTypeBL() { }
        public Results SearchDocumentTypes(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
           
            Results results = null;
            DocumentTypeDAL dal = new DocumentTypeDAL();
            try
            {
                results = dal.SearchDocumentTypes(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }

        public Results DocTypeCheckBeforeRemove(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            DocumentTypeDAL dal = new DocumentTypeDAL();
            try
            {
                results = dal.DocTypeCheckBeforeRemove(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results ManageDocumentType(DocumentType docType, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            DocumentTypeDAL dal = new DocumentTypeDAL();
            try
            {
                results = dal.ManageDocumentType(docType, action, loginOrgId, loginToken);
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "DUPLICATENAME";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results GetDocumnetTypeForAOrg(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            DocumentTypeDAL dal = new DocumentTypeDAL();
            try
            {
                results = dal.GetDocumnetTypeForAOrg(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
    }
}
