using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Data;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    [DataContract]
    public class Results
    {
        public Results()
        {
            ActionStatus = string.Empty;
            IdentityId = string.Empty;
            PageId = 0; //default
            UserData = null;
            Users = null;
            Orgs = null;
            Groups = null;
            Items = null;
            MenuInfoItems = null;
            Departments = null;
            Templates = null;
            //need to rmove later
            UserPassData = string.Empty;
            NewOrgCode = string.Empty;
            NextProcessID = 0;
            PageMainTagID = 0;
            PageSubTagID = 0;
            TotalTagPages = 0;

            IndexField1Name = string.Empty;
            IndexField2Name = string.Empty;
            IndexField3Name = string.Empty;
            IndexField4Name = string.Empty;
            IndexField5Name = string.Empty;
            IndexIDs = string.Empty;
            DocTypeCheckDeleteStatus = string.Empty;
            returncode = 0;
            ResultDS = new DataSet();

            // Output parameters to track db execution status
            ErrorState = 0;
            ErrorSeverity = 0;
            Message = string.Empty;
        }

        [DataMember]
        public string ActionStatus { get; set; }
        [DataMember]
        public string IdentityId { get; set; }
        [DataMember]
        public int PageId { get; set; }
        [DataMember]
        public string RecordCountText { get; set; }
        [DataMember]
        public string PagingText { get; set; }
        [DataMember]
        public List<Item> Items { get; set; }
        [DataMember]
        public UserBase UserData { get; set; }
        [DataMember]
        public List<User> Users { get; set; }
        [DataMember]
        public List<Org> Orgs { get; set; }
        [DataMember]
        public List<Group> Groups { get; set; }
        [DataMember]
        public List<Department> Departments { get; set; }
        [DataMember]
        public List<MenuInfoItem> MenuInfoItems { get; set; }
        [DataMember]
        public List<Template> Templates { get; set; }
        [DataMember]
        public List<DocumentType> DocumentTypes { get; set; }
        [DataMember]
        public List<DocumentDownload> DocumentDownloads { get; set; }
        [DataMember]
        public List<IndexField> IndexFields { get; set; }
        [DataMember]
        public List<BatchUploadBE> BatchUpload { get; set; }
        [DataMember]
        public List<MainTag> MainTag { get; set; }
        [DataMember]
        public List<SubTag> SubTag { get; set; }
        [DataMember]
        public string UserPassData { get; set; }
        [DataMember]
        public string NewOrgCode { get; set; }
        [DataMember]
        public int NextProcessID { get; set; }
        [DataMember]
        public int PageMainTagID { get; set; }
        [DataMember]
        public int PageSubTagID { get; set; }
        [DataMember]
        public int TotalTagPages { get; set; }
        [DataMember]
        public string IndexField1Name { get; set; }
        [DataMember]
        public string IndexField2Name { get; set; }
        [DataMember]
        public string IndexField3Name { get; set; }
        [DataMember]
        public string IndexField4Name { get; set; }
        [DataMember]
        public string IndexField5Name { get; set; }
        [DataMember]
        public string IndexIDs { get; set; }
        [DataMember]
        public string DocTypeCheckDeleteStatus { get; set; }
        [DataMember]
        public int returncode { get; set; }
        [DataMember]
        public DataSet ResultDS { get; set; }

        // Output parameters to track db execution status
        [DataMember]
        public int ErrorState { get; set; }
        [DataMember]
        public int ErrorSeverity { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}
