using System;

namespace Lotex.EnterpriseSolutions.CoreBE
{
    public class Business
    {
        #region Construtor
        public Business()
        {
            // Business type
            BusinessType = BusinessTypes.Default;
            // User's Organisation
            UserOrgId = 0;//User 

            //Transaction Data
            BusinessId = 0;
            BusinessCode = string.Empty;
            ParentBusinessId = 0;
            ParentBusinessName = string.Empty;
            UserName = string.Empty;
            BusinessFName = string.Empty;
            BusinessLName = string.Empty;
            EmailId = string.Empty;
            PhoneNo = string.Empty;
            FaxNo = string.Empty;
            CreatedDate = null;
            IsActive = false;
            Status = string.Empty;
            ErrorMessage = string.Empty;
            ActionBy = 0;
        }
        #endregion

        #region Propterties

        // Business type
        public BusinessTypes BusinessType { get; set; }

        //Trnasaction Data
        public int BusinessId { get; set; }
        public int UserOrgId { get; set; }
        public string BusinessCode { get; set; }
        public int ParentBusinessId { get; set; }
        public string ParentBusinessName { get; set; }
        public string UserName { get; set; }
        public string BusinessFName { get; set; }
        public string BusinessLName { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public int ActionBy { get; set; }
        #endregion
    }
}
