/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * ----------       -------------       ----------          ------------------------------------------------------------------

 * 15 06 2015       gokuldas.palapatta  DMS5-4370	        Version Control has been implemented, but system does not have the menu to view/retrieve earlier versions of the document.( a new button and function added in java script to redirect
====================================================================================================================================== */




using System;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class DocumentBL
    {
        public DocumentBL() { }

        public string GetDocumentReferenceId(SearchFilter Entityobj)
        {
            return new DocumentDAL().GetDocumentReferenceId(Entityobj);
        }

        public int ManageDocumentUpload(string XMLUploadData, string xmlPageNoMappings, string LoginOrgId, string LoginToken, string sAction, int iProcessID)
        {
            return new DocumentDAL().ManageDocumentUpload(XMLUploadData, xmlPageNoMappings, LoginOrgId, LoginToken, sAction, iProcessID);
        }

        #region SearchDocuments
        public Results SearchDocuments(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            DocumentDAL dal = new DocumentDAL();
            try
            {
                results = dal.SearchDocuments(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            return results;
        }

        

        public Results SearchDocumentsForMakerChecker(SearchFilter filter, string action, string loginOrgId, string loginToken, string Makerchecker)
        {
            Results results = null;
            DocumentDAL dal = new DocumentDAL();
            try
            {
                results = dal.SearchDocumentsForMakerChecker(filter, action, loginOrgId, loginToken, Makerchecker);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            return results;
        }
        public Results GetAvailablePagesForDocumentView(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            DocumentDAL dal = new DocumentDAL();
            try
            {
                results = dal.GetAvailablePagesForDocumentView(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            return results;
        }
        public Results GetDocumetRemarksAndStatus(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            DocumentDAL dal = new DocumentDAL();
            try
            {
                results = dal.GetDocumetRemarksAndStatus(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            return results;
        }
        public Results ManageTagDetails(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            DocumentDAL dal = new DocumentDAL();
            try
            {
                results = dal.ManageTagDetails(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            //results.Message = "Tag details saved successfully.";
            return results;
        }

        public Results GetTagDetails(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            DocumentDAL dal = new DocumentDAL();
            try
            {
                results = dal.GetTagDetails(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            return results;
        }

        //Method : Upload Docu Details
        public Results ManageUploadedDocuments(string XML, string LoginOrgId, string LoginToken, string sAction, int iProcessID, bool SplittingSeperated, string xmlPageNoMappings = "", string xmlTagPageNoMaping = "")
        {
            Results results = null;
            DocumentDAL dal = new DocumentDAL();
            try
            {
                results = dal.ManageUploadedDocuments(XML, xmlPageNoMappings,xmlTagPageNoMaping, LoginOrgId, LoginToken, sAction, iProcessID,SplittingSeperated);
            }
            catch (Exception)
            {

            }
            return results;
        }

        public Results ManageDigitalSignature(string action, string CertificatePath,string @in_xSignatureDetails, string LoginOrgId, string LoginToken)
        {
            Results results = null;
            DocumentDAL dal = new DocumentDAL();
            try
            {
                results = dal.ManageDigitalSignature(action, CertificatePath, @in_xSignatureDetails, LoginOrgId, LoginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            return results;
        }
        public Results GetSignatureDetails(string action,int DocumentId, string LoginOrgId, string LoginToken)
        {
            Results results = null;
            DocumentDAL dal = new DocumentDAL();
            try
            {
                results = dal.GetSignatureDetails(action,DocumentId, LoginOrgId, LoginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            return results;
        }
        /*DMS5-4370	BS*/
        public Results GetDocumetHistoryDetails(string action, int DocumentId, string LoginOrgId, string LoginToken)
        {
            Results results = null;
            DocumentDAL dal = new DocumentDAL();
            try
            {
                results = dal.GetDocumetHistoryDetails(action, DocumentId, LoginOrgId, LoginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            return results;
        }
        /*DMS5-4370	BE*/
        #endregion

    }
}
