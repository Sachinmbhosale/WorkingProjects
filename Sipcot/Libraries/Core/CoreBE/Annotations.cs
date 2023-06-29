using System.Runtime.Serialization;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    [DataContract]
    public class Annotations
    {
        public Annotations()
        {
            DocumentId = 0;
            PageNo = 0;
            XAnnotations = string.Empty;
            xDocumentWithAnnotations = string.Empty;
            //added for delete annotatation
            xPageNoMappings = string.Empty;

        }
        [DataMember]
        public int DocumentId { get; set; }
        [DataMember]
        public int PageNo { get; set; }
        [DataMember]
        public string XAnnotations { get; set; }
        [DataMember]
        public string xDocumentWithAnnotations { get; set; }
        [DataMember]
        public string xPageNoMappings { get; set; }

    }
}
