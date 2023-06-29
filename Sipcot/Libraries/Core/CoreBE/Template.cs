using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    [DataContract]
    public class Template
    {
        public Template()
        {
            TemplateId = 0;
            TemplateName = string.Empty;
            Active = false;
            FieldCount = 0;
            IndexFields = null;
            Tag = string.Empty;
            SubTag = string.Empty;
            CanDelete = 0;
            UploadFIleName = string.Empty;
            UploadFIleNameSeperator = string.Empty;
            IndexFieldDetails = string.Empty;
            IndexListDetails = string.Empty;
            TagListDetails = string.Empty;

        }
        [DataMember]
        public int TemplateId { get; set; }
        [DataMember]
        public int TemplateSrlNo { get; set; }
        [DataMember]
        public string TemplateName { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public List<IndexField> IndexFields { get; set; }
        [DataMember]
        public int FieldCount { get; set; }
        [DataMember]
        public string CreatedDate { get; set; }
        [DataMember]
        public List<MainTag> Tags { get; set; }
        [DataMember]
        public List<Category> Category { get; set; }
        [DataMember]
        public string Tag { get; set; }
        [DataMember]
        public string SubTag { get; set; }
        [DataMember]
        public int CanDelete { get; set; }

        [DataMember]
        public string UploadFIleName { get; set; }
        [DataMember]
        public string UploadFIleNameSeperator { get; set; }
        [DataMember]
        public string IndexFieldDetails { get; set; }
        [DataMember]
        public string IndexListDetails { get; set; }
         [DataMember]
        public string TagListDetails { get; set; }

        
    }
}
