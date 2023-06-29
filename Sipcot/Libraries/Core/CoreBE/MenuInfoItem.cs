using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    //This function is used to store user details      
    [DataContract]
    public class MenuInfoItem
    {
        public MenuInfoItem()
        {
            MenuId = 0;
            ParentId = 0;
            PageName = string.Empty;
            DisplayName = string.Empty;
            URL = string.Empty;
            Active = false;
        }
        [DataMember]
        public int MenuId { get; set; }
        [DataMember]
        public int ParentId { get; set; }
        [DataMember]
        public string PageName { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string URL { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public List<GroupRights> groupRights { get; set; }
    }
}
