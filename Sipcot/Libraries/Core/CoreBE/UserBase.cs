using System;
using System.Runtime.Serialization;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    [DataContract]
    public class UserBase
    {
        public UserBase()
        {
            //Logged User infromation
            //=======================
            UserId = 0;
            UserOrgId = 0;
            UserName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            EmailId = string.Empty;
            ConfidentialityAgreement = false;
            IsDomainUser = false;
            MobileNo = string.Empty;
            GroupId = 0;
            GroupName = string.Empty;
            
            //Session Management
            //=====================
            //To identify a user and create object under logged organisation Id
            LoginOrgId = 0;
            LoginToken = string.Empty;
            // To check the orgnisation is Parent organisation
            LoginParentOrgId = 0; 
            // used for masterpage data.
            LoginOrgName = string.Empty;
            LoginOrgCode = string.Empty;
            //To sent mail sent
            OrgEmailId = string.Empty;
            LoginWebsiteUrl = string.Empty;
            LanguageID = 0;
            LanguageCode = string.Empty;
            OutofOffice = 0;
        }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public int UserOrgId { get; set; }
        [DataMember]
        public int UserParentOrgId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string EmailId { get; set; }
        [DataMember]
        public string MobileNo { get; set; }

        [DataMember]
        public bool ConfidentialityAgreement { get; set; }
        public DateTime UserLoggedInTime { get; set; }
        public DateTime UserLastLoggedInTime { get; set; }

        [DataMember]
        public int GroupId { get; set; }
        [DataMember]
        public string GroupName { get; set; }

        //Common & Login Organization specific
        [DataMember]
        public int LoginOrgId { get; set; }
        [DataMember]
        public string LoginToken { get; set; }
        public int LoginParentOrgId { get; set; }

        public string LoginOrgName { get; set; }
        public string LoginOrgCode { get; set; }
        public string OrgEmailId { get; set; }
        public string LoginWebsiteUrl { get; set; }

        [DataMember]
        public int LanguageID { get; set; }
        [DataMember]
        public string LanguageCode { get; set; }
        [DataMember]
        public int OutofOffice { get; set; }
        

        [DataMember]
        public bool IsDomainUser { get; set; }
    }
}