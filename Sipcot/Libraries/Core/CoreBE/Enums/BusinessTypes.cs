/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID           Description:  
** ----------   -------------       ----------          ----------------------------
 * 17 Apr 2015  Yogeesha Naik        DMS5-3947          Rewritten most of the methods related to dynamic controls (made generic)                                 
=============================================================================== */


namespace Lotex.EnterpriseSolutions.CoreBE
{
    public enum BusinessTypes
    {
        Default = 0,
        Organization = 1,
        Location = 2,
        Department = 3,
        DocumentType = 41,
        Designation = 42
    }
    public enum Actions
    {
        ValidateUser = 1,
        ValidateUserExplicitly = 2,
        UserLogin = 3,
        GetMenuPermissionByUserId = 4,
        SearchOrgs = 5,
        ForgotPassword = 6,
        ChangePassword = 7
    }
    public enum ActionStatus
    {
        SUCCESS = 1,
        FAIL = 2,
        DUPLICATENMAE = 3,
        NODATA = 4,
        EXPAIRED = 5,
        ERROR = 6,
        MAILFAILED = 7
    }
    public enum FieldTypes
    {
        DateCalculation_CurrentDate = 0,
        DateCalculation_ReferDate = 1,
        DualDataEntry_MaskNone = 2,
        DualDataEntry_MaskFirst = 3,
        DualDataEntry_MaskSecond = 4,
        DualDatEntry_MaskBoth = 5,
        CalculateField = 6,
        Document = 7,
        DropDown = 8,
        MultiDropDown = 9,
        TextBox = 10
    }
    public enum DBTypes
    {
        String = 0,
        Date = 1,
        Number = 2,
        Boolean = 3
    }
}
