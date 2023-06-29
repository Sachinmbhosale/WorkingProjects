using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    [DataContract]
    public class DocumentDownload
    {
        public DocumentDownload()
        {
            ProcessID = 0;
            DocId = 0;
            DepID = 0;
            FileName = string.Empty;
            Version = string.Empty;
            TagStatus = string.Empty;
            Type = string.Empty;
            Active = 0;
            PageNo = 0;
            MainTagId = 0;
            SubTagId = 0;
            IndexField1 = string.Empty;
            IndexField2 = string.Empty;
            IndexField3 = string.Empty;
            IndexField4 = string.Empty;
            IndexField5 = string.Empty;
            IndexCount = 0;
            SlicingStatus = string.Empty;
            TotalRowcount = 0;
            RowsCount = 0;
            //DocDeatils = new List<string>();

            HtmlTable = string.Empty;
        }

        [DataMember]
        public string HtmlTable { get; set; }

        [DataMember]
        public int ProcessID { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string TagStatus { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public int DocId { get; set; }
        [DataMember]
        public int DepID { get; set; }
        [DataMember]
        public int Active { get; set; }
        [DataMember]
        public int PageNo { get; set; }
        [DataMember]
        public int MainTagId { get; set; }
        [DataMember]
        public int SubTagId { get; set; }
        [DataMember]
        public List<string> DocDeatils { get; set; }
        [DataMember]
        public int IndexCount { get; set; }
        [DataMember]
        public string IndexField1 { get; set; }
        [DataMember]
        public string IndexField2 { get; set; }
        [DataMember]
        public string IndexField3 { get; set; }
        [DataMember]
        public string IndexField4 { get; set; }
        [DataMember]
        public string IndexField5 { get; set; }
        [DataMember]
        public string SlicingStatus { get; set; }
        [DataMember]
        public int TotalRowcount { get; set; }
        [DataMember]
        public int RowsCount { get; set; }

    }
}
