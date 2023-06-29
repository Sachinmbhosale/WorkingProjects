using System.Runtime.Serialization;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    //This function is used to store user details      
    [DataContract]
    public class Org
    {
        public Org()
        {
            OrgId = 0;
            OrgCode = string.Empty;
            OrgParentId = 0;
            OrgName = string.Empty;
            OrgAddress = string.Empty;
            OrgCulture = string.Empty;
            OrgEmailId = string.Empty;
            PhoneNo = string.Empty;
            FaxNo = string.Empty;
            ContactPerson = string.Empty;
            ContactMobile = string.Empty;
            CreatedDate = string.Empty;
            LogoFileName = string.Empty;
            LogoPath = string.Empty;
            OrgGreeting = string.Empty;
            OrgDetails = string.Empty;
            AppId = string.Empty;
        }
        [DataMember]
        public int OrgId { get; set; }
        [DataMember]
        public int OrgParentId { get; set; }
        [DataMember]
        public string OrgCode { get; set; }
        [DataMember]
        public string OrgName { get; set; }
        [DataMember]
        public string OrgAddress { get; set; }
        [DataMember]
        public string OrgCulture { get; set; }
        [DataMember]
        public string OrgEmailId { get; set; }
        [DataMember]
        public string PhoneNo { get; set; }
        [DataMember]
        public string FaxNo { get; set; }
        [DataMember]
        public string ContactPerson { get; set; }
        [DataMember]
        public string ContactMobile { get; set; }
        [DataMember]
        public string CreatedDate { get; set; }
        [DataMember]
        public string LogoFileName { get; set; }
        [DataMember]
        public string LogoPath { get; set; }
        [DataMember]
        public string OrgGreeting { get; set; }
        [DataMember]
        public string OrgDetails { get; set; }
        [DataMember]
        public string AppId { get; set; }
    }
}
