using System.Runtime.Serialization;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    //This function is used to store user details      
    [DataContract]
    public class User : UserBase
    {
        public User()
        {
            EmpId = string.Empty;
            Password = string.Empty;
            NewPassword = string.Empty;

            GroupId = 0;
            DepartmentId = 0;
            GroupName = string.Empty;
            Active = false;
            DomainUser = false;
            DomainId = 0;
            DomainName = string.Empty;
            DepartmentIdsForUserCreattion = string.Empty;
        }
        public string EmpId { get; set; }   //user employee Id
        public string Password { get; set; }
        public string NewPassword { get; set; }

        public int DepartmentId { get; set; }
        public string DepartmentIdsForUserCreattion { get; set; }

        public string Description { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string CreatedDate { get; set; }
        [DataMember]
        public bool DomainUser { get; set; }
        [DataMember]
        public int DomainId { get; set; }
        [DataMember]
        public string DomainName { get; set; }
    }
}