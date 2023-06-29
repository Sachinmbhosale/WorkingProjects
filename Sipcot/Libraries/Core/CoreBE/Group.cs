using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    //This function is used to store user details      
    [DataContract]
    public class Group
    {
        public Group()
        {
            GroupId = 0;
            //GroupParentId = 0;
            GroupName = string.Empty;
            Description = string.Empty;
            UserCount = 0;
            CreatedDate = string.Empty;
            groupRights = null;
            Active = false;
            Fixed = false;
        }
        [DataMember]
        public int GroupId { get; set; }
        [DataMember]
        //public int GroupParentId { get; set; }
        //[DataMember]
        public string GroupName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int UserCount { get; set; }
        [DataMember]
        public string CreatedDate { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public bool Fixed { get; set; }
        [DataMember]
        public List<GroupRights> groupRights { get; set; }
    }
}
