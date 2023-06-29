/**============================================================================  
Author     : Gokuldas.palapatta
Create date: 28/02/2013
===============================================================================  
** Change History   
** Date:        Author:             Issue ID    	Description: 
**(dd MMM yyyy)
** -----------------------------------------------------------------------------
** 10 jun 2015  Gokuldas.palapatta	DMS5-4339	Soft data upload checking for duplicates
**===============================================================================*/



namespace Lotex.EnterpriseSolutions.CoreBE
{
    public static class CoreMessages
    {
        public static string GetMessages(string action, string actionStatus,string exception = "")
        {
            string message = string.Empty;
            if (action == "ServiceFailed")
            {
                message = "An error occurred, please try again. If problem exists please contact Administrator";
            }
            else if (action == "InvalidToken")
            {
                message = "Invalid Token. Please Try Again";
            }
            else if (action == "ValidateUser" || action == "UserLogin")
            {
                switch (actionStatus)
                {
                    case "INACTIVE":
                        message = "User account is not configured or inactive. Please contact administrator.";
                        break;
                    case "LOCKED":
                        message = "User account is locked. Please contact administrator.";
                        break;
                    case "INVALIDUSER":
                        message = "User does not exist. Please try with a different User Name";
                        break;
                    case "INVALIDPASS":
                        message = "User name and password does not match. Please try again";
                        break;
                    case "USERNAME_EXPIRED":
                        message = "User name is expired. Please contact administrator.";
                        break;
                    case "ERROR":
                        message = exception;
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "SearchOrgs")
            {
                switch (actionStatus)
                {
                    case "NODATA":
                        message = "Customers not found.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "ChangePassword")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "New Password Updated Successfully.";
                        break;
                    case "FAILED":
                        message = "Current Password doesn't match with the password in records. Please try again!";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "AddOrg")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "Customer Added Successfully. Please wait.. ";
                        break;
                    case "DUPLICATENAME":
                        message = "Customer Name already exists.";
                        break;
                    case "DUPLICATEEMAIL":
                        message = "Customer Email already exists.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "EditOrg")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "Customer Updated Successfully.";
                        break;
                    case "DUPLICATENAME":
                        message = "Customer Name already exists.";
                        break;
                    case "DUPLICATEEMAIL":
                        message = "Customer Email already exists.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "AddUser")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "User Added Successfully. ";
                        break;
                    case "DUPLICATENAME":
                        message = "User Name already exists.";
                        break;
                    case "DUPLICATEEMAIL":
                        message = "User Email already exists.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "AddTemplate")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "Template Added Successfully.";
                        break;
                    case "DUPLICATENAME":
                        message = "Template Name already exists.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "EditTemplate")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "Template Updated Successfully.";
                        break;
                    case "DUPLICATENAME":
                        message = "Template Name already exists.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "AddDocumentType")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "ProjectType Added Successfully.";
                        break;
                    case "DUPLICATENAME":
                        message = "ProjectType Name already exists.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "EditDocumentType")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "ProjectType Updated Successfully.";
                        break;
                    case "DUPLICATENAME":
                        message = "ProjectType Name already exists.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "EditUser")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "User Updated Successfully.";
                        break;
                    case "DUPLICATENAME":
                        message = "User Name already exists.";
                        break;
                    case "DUPLICATEEMAIL":
                        message = "User Email already exists.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "DeleteUser")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "User Deleted Successfully.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "ForgotPassword")
            {
                switch (actionStatus)
                {
                    case "FAILED":
                        message = "User does not exist. Please try with a different User Name.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "GetUserDataToSendPassword")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "New password is sent to your email address:";
                        break;
                    case "MAILFAILED":
                        message = "Password has been reset successfully. Sending New password through email failed.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "AddUserSentMail")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "A welcome email with user credentials and website link has been sent to the emailaddress:";
                        break;
                    case "MAILFAILED":
                        message = "Sending Welcome email failed.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }

            else if (action == "AddOrgSentMail")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "A welcome email with user credentials and website link has been sent to customer email address:";
                        break;
                    case "MAILFAILED":
                        message = "Sending Welcome email failed.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "DeleteOrg")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "Customer Deleted Successfully.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "AddGroup")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "Group Added Successfully.";
                        break;
                    case "DUPLICATENAME":
                        message = "Group Name already exists.";
                        break;

                    default:
                        message = string.Empty;
                        break;
                }
            }
            else if (action == "EditGroup")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "Group Updated Successfully.";
                        break;
                    case "DUPLICATENAME":
                        message = "Group Name already exists.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            /* DMS5-4339 BS */
            else if (action == "UpdateBatchData")
            {
                switch (actionStatus)
                {
                    case "SUCCESS":
                        message = "File uploaded successfully.";
                        break;
                    case "DUPLICATENAME":
                        message = "Failed! Duplicate filenames are present.";
                        break;
                    case "FAILED":
                        message = "Something went wrong!.";
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            /* DMS5-4339 BE */
            else //default
            {
                switch (actionStatus)
                {
                    case "ERROR":
                        message = exception;
                        break;
                    default:
                        message = string.Empty;
                        break;
                }
            }
            return message;
        }
    }
}
