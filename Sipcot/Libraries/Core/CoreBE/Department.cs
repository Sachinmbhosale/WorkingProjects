using System.Runtime.Serialization;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    [DataContract]
    public class Department
    {
        public Department()
        {
            DepartmentId = 0;
            //DepartmentParentId = 0;
            DepartmentName = string.Empty;
            Description = string.Empty;
            CreatedDate = string.Empty;
            Head = string.Empty;
            Active = false;
            SystemUser = string.Empty;
        }
        [DataMember]
        public int DepartmentId { get; set; }
        [DataMember]
        public string DepartmentName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Head { get; set; }
        [DataMember]
        public string CreatedDate { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string SystemUser { get; set; }
    }
}
