using System.Runtime.Serialization;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    [DataContract]
    public class DocumentType
    {
        public DocumentType()
        {
            DocumentTypeId = 0;
            OrgId = 0;
            TemplateId = string.Empty;
            DocumentTypeName = string.Empty;

            Description = string.Empty;
            GroupCount = 0;
            Active = false;
            CreatedDate = string.Empty;
            CanDelete = 0;
            TemplateName = string.Empty;
            DepartmentName = string.Empty;
            TemplateDepId = string.Empty;
            XMLDocType = string.Empty;
            DepartmentId = string.Empty;
            ArchivalD = string.Empty;
            WaterMarkT = string.Empty;
            Makerchecker = false;

        }
        [DataMember]
        public int DocumentTypeId { get; set; }
        [DataMember]
        public int OrgId { get; set; }
        [DataMember]
        public string TemplateId { get; set; }
        [DataMember]
        public string DocumentTypeName { get; set; }

        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int GroupCount { get; set; }
        [DataMember]
        public string GroupIds { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string CreatedDate { get; set; }
        [DataMember]
        public int CanDelete { get; set; }
        [DataMember]
        public string TemplateName { get; set; }
        [DataMember]
        public string DepartmentName { get; set; }
        [DataMember]
        public string TemplateDepId { get; set; }
        [DataMember]
        public string XMLDocType { get; set; }
        [DataMember]
        public string DepartmentId { get; set; }
        [DataMember]
        public string ArchivalD { get; set; }
        [DataMember]
        public string WaterMarkT { get; set; }
        [DataMember]
        public bool Makerchecker { get; set; }
    }
}
