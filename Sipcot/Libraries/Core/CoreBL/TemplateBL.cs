using System;
using System.Collections.Generic;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;
using System.Data;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class TemplateBL
    {
        public TemplateBL() { }

        public DataSet GetTemplateDetails(SearchFilter Entityobj)
        {
            return new TemplateDAL().GetTemplateDetails(Entityobj);
        }

        public Results SearchTemplates(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            TemplateDAL dal = new TemplateDAL();
            try
            {
                results = dal.SearchTemplates(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results ManageTemplates(Template tpl, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            TemplateDAL dal = new TemplateDAL();
            try
            {
                results = dal.ManageTemplates(tpl, action, loginOrgId, loginToken);
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results GetAllTemplatesForOrg(int OrgId, string TempateName, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            TemplateDAL dal = new TemplateDAL();
            try
            {
                dal.GetAllTemplatesForOrg(OrgId, TempateName, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results GetTemplateFieldList(Template tpl, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            TemplateDAL dal = new TemplateDAL();
            try
            {
                results = dal.GetTemplateFieldList(tpl, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        #region GetTemplateFiledsByDocumenttypeandDepartment
        public Results GetTemplateDetails(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            TemplateDAL dal = new TemplateDAL();
            try
            {
                results = dal.GetTemplateDetails(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        #endregion
        #region GetTemplateMultiValues
        public Results GetTemplateMultiValues(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            TemplateDAL dal = new TemplateDAL();
            try
            {
                results = dal.GetTemplateMultiValues(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        #endregion

        #region ExportDocumentType
        public Results ExportDocumentType(SearchFilter filter, string action, string loginOrgId, string loginToken)
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
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        #endregion

        public DataSet ManageListItems(IndexField filter, string action, string loginOrgId, string loginToken)
        {
          return new TemplateDAL().ManageListItems(filter, action, loginOrgId, loginToken);         
        }

        #region Get Tag Details
        public DataSet GetTaggedSubTag(string Documentid, string MaintagID)
        {
            return new TemplateDAL().GeTaggedtSubTag(Documentid, MaintagID);
        }
        public DataSet GetTaggedMainTag(string Documentid)
        {
            return new TemplateDAL().GeTaggedtMainTag(Documentid);
        }

        public DataSet GetTagDetails(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
          return new TemplateDAL().GetTagDetails(filter, action, loginOrgId, loginToken);            
        }
        #endregion

        public System.Data.DataTable GetTempcolumns(List<string> SearchData,string strColumns)
        {
            System.Data.DataTable dsSearcData = null;
            DocumentSearchDAL dal = new DocumentSearchDAL();
            try
            {
                dsSearcData = dal.GetTempcolumns(SearchData, strColumns);
            }
            catch (Exception ex)
            {
            }
            return dsSearcData;
        }
        public System.Data.DataTable GetSearcData(List<string> SearchData)
        {
            System.Data.DataTable dsSearcData = null;
            DocumentSearchDAL dal = new DocumentSearchDAL();
            try
            {
                dsSearcData = dal.GetSearcData(SearchData);
            }
            catch (Exception ex)
            {
            }
            return dsSearcData;
        }
        public System.Data.DataTable GetColumn(int UserId, int OrgId, string DocuType, int DeptId)
        {
            System.Data.DataTable dsProDept = null;
            DocumentSearchDAL dal = new DocumentSearchDAL();
            try
            {
                dsProDept = dal.GetColumn(UserId, OrgId, DocuType, DeptId);
            }
            catch (Exception ex)
            {
            }
            return dsProDept;
        }
        public string GetStartPage(List<string> SearchData)
        {
            string TagPages = string.Empty;
            Results results = null;
            DocumentSearchDAL dal = new DocumentSearchDAL();

            try
            {
                TagPages = dal.GetStartPage(SearchData);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages("", results.ActionStatus);
            }
            return TagPages;
        }
    }
}
