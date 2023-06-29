using System.Collections.Generic;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    public class SearchFilter
    {

        public SearchFilter()
        {
            CurrUserId = 0;
            UserName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            EmpId = string.Empty;

            CurrGroupId = 0;
            GroupName = string.Empty;

            CurrOrgId = 0;
            OrgName = string.Empty;
            OrgEmailId = string.Empty;
            UserEmailId = string.Empty;
            FromDate = string.Empty;
            ToDate = string.Empty;
            PageId = 0;

            CurrDocumentTypeId = 0;
            DocumentTypeName = string.Empty;
            CurrTemplateId = 0;
            TemplateName = string.Empty;
            DepartmentName = string.Empty;
            DocumentTypeID = 0;
            Documentname = string.Empty;
            DepartmentID = 0;
            Refid = string.Empty;
            keywords = string.Empty;
            TemplateFields = new List<string>();
            UploadDocID = 0;
            StartDate = string.Empty;
            EndDate = string.Empty;
            Active = 0;
            DocVirPath = string.Empty;
            DocPhyPath = string.Empty;
            SearchOption = string.Empty;
            BatchUploadFilterId = 0;
            BatchName = string.Empty;
            BatchData = string.Empty;
            BatchId = 0;
            //added for delete Tags
            xPageNoMappings = string.Empty;
            //Paging
            PagingSortExp = string.Empty;
            startRowIndex = 0;
            endRowIndex = 0;
            currentPageNumber = 0;
            PagingPageSize = 0;
            PagingTotalRows = 0;
            IsExport = false;
            DeleteID = 0;
            Subaction = string.Empty;
            DocPageNo = 0;
            RowsPerPage = 0;

            WhereClause = string.Empty;
            taggedPages = string.Empty;
            CurrentDocumentId = 0;
            GenStatusID = 0;

        }

        //serach Data
        public int CurrUserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmpId { get; set; }
        public int CurrGroupId { get; set; }
        public string GroupName { get; set; }
        public int CurrOrgId { get; set; }
        public string OrgName { get; set; }
        public string OrgEmailId { get; set; }
        public string UserEmailId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int PageId { get; set; }
        public string Documentname { get; set; }
        public string DepartmentName { get; set; }

        public int CurrDocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public int CurrTemplateId { get; set; }
        public string TemplateName { get; set; }

        public int DocumentTypeID { get; set; }
        public int DepartmentID { get; set; }
        public string Refid { get; set; }
        public string keywords { get; set; }
        public List<string> TemplateFields { get; set; }
        public int UploadDocID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Active { get; set; }
        public string DocVirPath { get; set; }
        public string DocPhyPath { get; set; }

        public int BatchUploadFilterId { get; set; }
        public int MainTagID { get; set; }
        public int SubTagID { get; set; }
        public int TotalPages { get; set; }
        public string SearchOption { get; set; }
        public string BatchName { get; set; }
        public string BatchData { get; set; }
        public int BatchId { get; set; }
        //added for delete Tags
        public string xPageNoMappings { get; set; }
        //Paging
        public string PagingSortExp { get; set; }
        public int startRowIndex { get; set; }
        public int endRowIndex { get; set; }
        public int currentPageNumber { get; set; }
        public int PagingPageSize { get; set; }
        public int PagingTotalRows { get; set; }
        public bool IsExport { get; set; }

        public int DeleteID { get; set; }
        public string Subaction { get; set; }
        public int DocPageNo { get; set; }
        public int RowsPerPage { get; set; }

        public string WhereClause { get; set; }
        public string taggedPages { get; set; }

        public int CurrentDocumentId { get; set; }
        public int GenStatusID { get; set; }

    }

}
