using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;
using System;
using System.Collections.Generic;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class DocTagBL
    {
        public Results SaveDetails(List<string> SearchData, string loginOrgId, string loginToken)
        {
            Results results = null;
            DocTagDAL dal = new DocTagDAL();

            try
            {
                results = dal.SaveDetails(SearchData, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages("", results.ActionStatus);
            }
            return results;
        }

        public Results DeleteData(List<string> SearchData, string loginOrgId, string loginToken)
        {
            Results results = null;
            DocTagDAL dal = new DocTagDAL();

            try
            {
                results = dal.DeleteData(SearchData, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages("", results.ActionStatus);
            }
            return results;
        }

        public string GetFilePath(int Id, int DocTypeId, int DeptId)
        {
            string FilePath = string.Empty;
            Results results = null;
            DocTagDAL dal = new DocTagDAL();

            try
            {
                FilePath = dal.GetFilePath(Id, DocTypeId, DeptId);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages("", results.ActionStatus);
            }
            return FilePath;
        }

        public string GetTagPages(List<string> SearchData)
        {
            string TagPages = string.Empty;
            Results results = null;
            DocTagDAL dal = new DocTagDAL();

            try
            {
                TagPages = dal.GetTagPages(SearchData);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages("", results.ActionStatus);
            }
            return TagPages;
        }

        public string GetDownloadRights(List<string> SearchData)
        {
            string RCOUNT = string.Empty;
            Results results = null;
            DocTagDAL dal = new DocTagDAL();

            try
            {
                RCOUNT = dal.GetDownloadRights(SearchData);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages("", results.ActionStatus);
            }
            return RCOUNT;
        }

        public System.Data.DataTable Gettotaltagpages(int Docid, int Pageno, int iMaintag, int iSubtag)
        {
            System.Data.DataTable dsDomain = null;
            DocTagDAL dal = new DocTagDAL();
            try
            {
                dsDomain = dal.Gettotaltagpages(Docid, Pageno, iMaintag, iSubtag);
            }
            catch (Exception ex)
            {
            }
            return dsDomain;
        }
    }
}
