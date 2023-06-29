using System;
using System.Collections.Generic;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;


namespace CoreServiceLib
{
    // NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
    public class CoreService : ICoreService
    {

        #region ICoreService Members

        //public string GetScalar(string action, string methodParams, string browser)// GET
        //{
        //    return "1";
        //}
        //public Results PostScalar(string action, string methodParams, string browser) //POST
        //{

        //    Results result = new Results();

        //    result.status = 0; //Failed
        //    result.message = "Service Unavaialble. Please try later.";
        //    result.UserBaseData = new UserBase();
        //    result.UserBaseData.Token = string.Empty;

        //    return result;
        //}
        public Results PostScalar(string loginOrgId, string loginToken, string action, string methodParams, string browser) //POST
        {
            Results results = null;
            bool serviceFailed = false;
            switch (action)
            {
                case "ValidateUser": //3 Params(OrgName, Username & Passwrod)
                    results = CallValidateUser(action, methodParams, browser, ref  serviceFailed);
                    break;
                case "ChangePassword": //2 Params(Username Passwrod & NewPasword)
                    results = CallChangePassword(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "ForgotPassword": //2 Params(Username Passwrod & NewPasword)
                    results = CallForgotPassword(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "SearchOrgs": //
                case "DeleteOrgAndSearch": //
                    results = CallSearchOrgs(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "AddOrg":
                case "EditOrg":
                    //2 Params(Username Passwrod & NewPasword)
                    results = CallManageOrg(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "SearchUsers": //
                case "DeleteUserAndSearch": //
                    results = CallSearchUsers(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "AddUser":
                case "EditUser":
                case "EditTempUser":
                    //2 Params(Username Passwrod & NewPasword)
                    results = CallManageUser(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "SearchGroups": //
                case "DeleteGroupAndSearch": //
                    results = CallSearchGroups(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "SearchDocumentTypes": //
                case "DeleteDocumentTypeAndSearch": //
                case "ExportDocumentType":
                    results = CallSearchDocumentTypes(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "AddDocumentType": //
                case "EditDocumentType": //
                    results = CallManageDocumentType(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "SearchTemplates": //
                case "DeleteTemplateAndSearch": //
                    results = CallSearchTemplates(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "SearchDocuments":
                    results = CallSearchDocuments(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "SearchDepartmentsForDeptPage":
                case "DeleteDepartmentAndSearch":
                    results = CallSearchDepartmentsForDeptPage(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "GetBatchUploadData":
                    results = CallGetBatchUploadData(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "AddOrUpdateTagDetails":
                case "DeleteTagDetails":
                    results = CallManageTag(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "GetTagDetails":
                    results = CallGetTagDetails(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "UpdateTrackTableOnPrint":
                    results = CallTrackPrint(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                case "DocTypeCheckBeforeRemove":
                    results = DocTypeCheckBeforeRemove(loginOrgId, loginToken, action, methodParams, browser, ref  serviceFailed);
                    break;
                default:
                    break;
            }
            if (serviceFailed)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages("ServiceFailed", results.ActionStatus);
            }
            return results;
        }
        public Results CallValidateUser(string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results rs = new Results();
            string[] userdata = null;
            User loginUser = new User();
            SecurityBL bl = new SecurityBL();
            userdata = methodParams.Split('|');
            if (userdata.Length == 5)
            {
                loginUser.LoginOrgCode = userdata[0].Trim();
                loginUser.UserName = userdata[1].Trim();
                loginUser.Password = userdata[2].Trim();
                loginUser.LoginToken = System.Guid.NewGuid().ToString();
                loginUser.LoginOrgId = 0;
                loginUser.DomainUser = Convert.ToBoolean(userdata[3].Trim());
                loginUser.DomainId = Convert.ToInt32(userdata[4]);
                rs = bl.ValidateUserManagement(ref loginUser, action);
            }
            else
            {
                serviceFailed = true;
            }
            return rs;
        }
        public Results CallChangePassword(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results rs = new Results();
            string[] userdata = null;
            User loginUser = new User();
            SecurityBL bl = new SecurityBL();
            userdata = methodParams.Split('|');
            if (userdata.Length == 3)
            {
                loginUser.UserName = userdata[0].Trim();
                loginUser.Password = userdata[1].Trim();
                loginUser.NewPassword = userdata[2].Trim();
                loginUser.LoginToken = loginToken;
                loginUser.LoginOrgId = Convert.ToInt32(loginOrgId);
                rs = bl.LoggedUserManagement(ref loginUser, action);
            }
            else
            {
                serviceFailed = true;
            }
            return rs;
        }

        public Results DocTypeCheckBeforeRemove(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            DocumentTypeBL bl = new DocumentTypeBL();

            userdata = methodParams.Split('|');
            if (userdata.Length == 2)
            {
                filter.DepartmentID = Convert.ToInt32(userdata[0].Trim());
                filter.DocumentTypeID = Convert.ToInt32(userdata[1].Trim());

                results = bl.DocTypeCheckBeforeRemove(filter, action, loginOrgId, loginToken);
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }
        public Results CallForgotPassword(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results rs = new Results();
            string[] userdata = null;
            User loginUser = new User();
            SecurityBL bl = new SecurityBL();
            userdata = methodParams.Split('|');
            if (userdata.Length == 2)
            {
                loginUser.LoginOrgCode = userdata[0].Trim();
                loginUser.UserName = userdata[1].Trim();
                rs = bl.ValidateUserManagement(ref loginUser, action);
            }
            else
            {
                serviceFailed = true;
            }
            return rs;
        }
        public Results CallSearchOrgs(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            SecurityBL bl = new SecurityBL();
            userdata = methodParams.Split('|');
            if (userdata.Length == 7)
            {
                string subAction = string.Empty;
                string deleteMessage = string.Empty;
                if (action == "DeleteOrgAndSearch")
                {
                    subAction = "DeleteOrg";
                    Org customer = new Org();
                    customer.OrgName = userdata[6].Trim();
                    customer.OrgId = Convert.ToInt32(userdata[5].Trim());
                    results = bl.ManageOrgs(customer, subAction, loginOrgId, loginToken);
                    if (results.ActionStatus == "SUCCESS")
                    {
                        deleteMessage = results.Message;
                        subAction = "SearchOrgs";
                    }
                }
                else
                {
                    subAction = "SearchOrgs";
                }
                if (subAction == "SearchOrgs")//if the deletion is success or only search is to be done, execute the below code
                {
                    filter.OrgName = userdata[0].Trim();
                    filter.OrgEmailId = userdata[1].Trim();
                    filter.FromDate = userdata[2].Trim();
                    filter.ToDate = userdata[3].Trim();
                    filter.PageId = Convert.ToInt32(userdata[4]);

                    results = bl.SearchOrgs(filter, subAction, loginOrgId, loginToken);
                    if (results.ActionStatus == "SUCCESS")
                    {
                        results.Message = deleteMessage;
                        int count = 0;
                        if (results.Orgs != null) { count = results.Orgs.Count; }
                        if (count == 0)
                        {
                            results.RecordCountText = "No Records Found. Please try with another Search Criteria";
                            results.PagingText = " ";
                        }
                        else
                        {
                            results.RecordCountText = "Displaying 1 to " + count.ToString() + " of " + count.ToString() + " Record(s)";
                            results.PagingText = "Page 1";
                        }
                    }
                    else
                    {
                        results.Message = deleteMessage + "</br>" + results.Message;
                    }
                }
                else
                {
                    serviceFailed = true;
                }
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }
        public Results CallManageOrg(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            Org customer = new Org();
            SecurityBL bl = new SecurityBL();
            userdata = methodParams.Split('|');
            if ((action == "AddOrg" || action == "EditOrg") && userdata.Length >= 8)
            {
                customer.OrgName = userdata[0].Trim();
                customer.OrgAddress = userdata[1].Trim();
                customer.OrgEmailId = userdata[2].Trim();
                customer.PhoneNo = userdata[3].Trim();
                customer.FaxNo = userdata[4].Trim();
                customer.ContactPerson = userdata[5].Trim();
                customer.ContactMobile = userdata[6].Trim();
                customer.OrgId = Convert.ToInt32(userdata[7].Trim());
                customer.LogoFileName = userdata[8].Trim();
                customer.OrgGreeting = userdata[9].Trim();// newly added for adding Organization greeting 6/feb/2014
                customer.OrgDetails = userdata[10].Trim();// newly added for adding Organization Details 6/feb/2014
                customer.AppId = Convert.ToString(userdata[11].Trim());
                results = bl.ManageOrgs(customer, action, loginOrgId, loginToken);
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }
        public Results CallSearchUsers(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            UserBL bl = new UserBL();
            userdata = methodParams.Split('|');
            if (userdata.Length == 9)
            {
                string subAction = string.Empty;
                string deleteMessage = string.Empty;
                if (action == "DeleteUserAndSearch")
                {
                    subAction = "DeleteUser";
                    User user = new User();
                    user.UserName = userdata[0].Trim();
                    user.UserId = Convert.ToInt32(userdata[7].Trim());
                    results = bl.ManageUser(user, subAction, loginOrgId, loginToken);
                    if (results.ActionStatus == "SUCCESS")
                    {
                        deleteMessage = results.Message;
                        subAction = "SearchUsers";
                    }
                }
                else
                {
                    subAction = "SearchUsers";
                }
                if (subAction == "SearchUsers")//if the deletion is success or only search is to be done, execute the below code
                {
                    filter.UserName = userdata[0].Trim();
                    filter.UserEmailId = userdata[1].Trim();
                    filter.FirstName = userdata[2].Trim();
                    filter.LastName = userdata[3].Trim();
                    filter.FromDate = userdata[4].Trim();
                    filter.ToDate = userdata[5].Trim();
                    filter.PageId = Convert.ToInt32(userdata[6]);

                    results = bl.SearchUsers(filter, subAction, loginOrgId, loginToken);
                    if (results.ActionStatus == "SUCCESS")
                    {
                        results.Message = deleteMessage;
                        int count = 0;
                        if (results.Users != null) { count = results.Users.Count; }
                        if (count == 0)
                        {
                            results.RecordCountText = "No Records Found. Please try with another Search Criteria";
                            results.PagingText = " ";
                        }
                        else
                        {
                            results.RecordCountText = "Displaying 1 to " + count.ToString() + " of " + count.ToString() + " Record(s)";
                            results.PagingText = "Page 1";
                        }
                    }
                    else
                    {
                        results.Message = deleteMessage + "</br>" + results.Message;
                    }
                }
                else
                {
                    serviceFailed = true;
                }
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }
        public Results CallManageUser(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            User user = new User();
            UserBL bl = new UserBL();
            userdata = methodParams.Split('|');
            if ((action == "AddUser" || action == "EditUser" || action == "EditTempUser") && userdata.Length == 12)
            {
                user.UserName = userdata[0].Trim();
                user.Description = userdata[1].Trim();
                user.EmailId = userdata[2].Trim();
                user.MobileNo = userdata[3].Trim();
                user.FirstName = userdata[4].Trim();
                user.LastName = userdata[5].Trim();
                user.GroupId = Convert.ToInt32(userdata[6].Trim());
                user.DepartmentIdsForUserCreattion = userdata[7].Trim();
                if (userdata[8].Trim() == "true")
                {
                    user.Active = true;
                }
                else
                {
                    user.Active = false;
                }
                user.UserId = Convert.ToInt32(userdata[9].Trim());
                if (userdata[10].Trim() == "true")
                {
                    user.DomainUser = true;
                }
                else
                {
                    user.DomainUser = false;
                }
                user.DomainId = Convert.ToInt32(userdata[11]);
                results = bl.ManageUser(user, action, loginOrgId, loginToken);
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }
        public Results CallSearchGroups(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            GroupBL bl = new GroupBL();
            userdata = methodParams.Split('|');
            if (userdata.Length == 6)
            {
                string subAction = string.Empty;
                string deleteMessage = string.Empty;
                if (action == "DeleteGroupAndSearch")
                {
                    subAction = "DeleteGroup";
                    Group group = new Group();
                    group.GroupName = userdata[5].Trim();
                    group.GroupId = Convert.ToInt32(userdata[4].Trim());
                    results = bl.ManageGroups(group, subAction, loginOrgId, loginToken);
                    if (results.ActionStatus == "SUCCESS")
                    {
                        deleteMessage = results.Message;
                        subAction = "SearchGroups";
                    }
                }
                else
                {
                    subAction = "SearchGroups";
                }
                if (subAction == "SearchGroups")//if the deletion is success or only search is to be done, execute the below code
                {
                    filter.GroupName = userdata[0].Trim(); ;
                    filter.FromDate = userdata[1].Trim();
                    filter.ToDate = userdata[2].Trim();
                    filter.PageId = Convert.ToInt32(userdata[3]);

                    results = bl.SearchGroups(filter, subAction, loginOrgId, loginToken);
                    if (results.ActionStatus == "SUCCESS")
                    {
                        results.Message = deleteMessage;
                        int count = 0;
                        if (results.Groups != null) { count = results.Groups.Count; }
                        if (count == 0)
                        {
                            results.RecordCountText = "No Records Found. Please try with another Search Criteria";
                            results.PagingText = " ";
                        }
                        else
                        {
                            results.RecordCountText = "Displaying 1 to " + count.ToString() + " of " + count.ToString() + " Record(s)";
                            results.PagingText = "Page 1";
                        }
                    }
                    else
                    {
                        results.Message = deleteMessage + "</br>" + results.Message;
                    }
                }
                else
                {
                    serviceFailed = true;
                }
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }
        public Results CallManageDocumentType(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            string[] InitialData = null;
            string[] DocTypeData = null;


            DocumentType doc = new DocumentType();
            DocumentTypeBL bl = new DocumentTypeBL();
            InitialData = methodParams.Split('!');
            userdata = InitialData[0].Split('|');
            DocTypeData = InitialData[1].Split('#');
            doc.XMLDocType = "<DocType>";
            for (int i = 0; i < DocTypeData.Length; i++)
            {

                string[] dataFields = DocTypeData[i].Split('|');
                doc.XMLDocType += "<DocTypeDetails>";
                doc.XMLDocType += "<DepartmentValue>" + dataFields[1] + "</DepartmentValue>";
                doc.XMLDocType += "<templateValue>" + dataFields[2] + "</templateValue>";
                doc.XMLDocType += "<ArchivalD>" + dataFields[3] + "</ArchivalD>";
                doc.XMLDocType += "<WaterMarkT>" + dataFields[4] + "</WaterMarkT>";
                doc.XMLDocType += "<MakerChecker>" + dataFields[5] + "</MakerChecker>";
                doc.XMLDocType += "</DocTypeDetails>";

            }

            doc.XMLDocType += "</DocType>";


            if ((action == "AddDocumentType" || action == "EditDocumentType") && userdata.Length == 5)
            {
                doc.DocumentTypeName = userdata[0].Trim();
                doc.Description = userdata[1].Trim();
                doc.Active = Convert.ToBoolean(userdata[2].Trim());
                doc.GroupIds = userdata[3].Trim();
                doc.DocumentTypeId = Convert.ToInt32(userdata[4].Trim());
                results = bl.ManageDocumentType(doc, action, loginOrgId, loginToken);
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }
        public Results CallTrackPrint(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            SearchFilter doc = new SearchFilter();
            UploadDocBL bl = new UploadDocBL();
            doc.UploadDocID = Convert.ToInt32(methodParams);
            results = bl.UpdateTrackTableOnDownload(doc, action, loginOrgId, loginToken);
            return results;
        }
        public Results PostTable(string loginOrgId, string loginToken, string action, string methodParams, string data, string browser)
        {
            Results results = null;
            bool serviceFailed = false;
            switch (action)
            {
                case "AddGroup":
                case "EditGroup":
                    //2 Params(Groupname Passwrod & NewPasword)
                    results = CallManageGroup(loginOrgId, loginToken, action, methodParams, data, browser, ref  serviceFailed);
                    break;
                case "AddTemplate":
                case "EditTemplate":
                    //2 Params(Groupname Passwrod & NewPasword)
                    results = CallManageTemplate(loginOrgId, loginToken, action, methodParams, data, browser, ref  serviceFailed);
                    break;
                default:
                    break;
            }
            if (serviceFailed)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages("ServiceFailed", results.ActionStatus);
            }
            return results;

        }
        public Results CallManageGroup(string loginOrgId, string loginToken, string action, string methodParams, string data, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            string[] datacollection = null;
            Group group = new Group();
            GroupBL bl = new GroupBL();
            userdata = methodParams.Split('|');
            if ((action == "AddGroup" || action == "EditGroup") && userdata.Length == 3)
            {
                group.GroupName = userdata[0].Trim();
                group.Description = userdata[1].Trim();
                group.GroupId = Convert.ToInt32(userdata[2].Trim());
                if (data != string.Empty)
                {
                    datacollection = data.Split("#".ToCharArray());
                    for (int i = 0; i < datacollection.Length; i++)
                    {

                        string[] dataFields = datacollection[i].Split('|');

                        if (dataFields.Length > 1)
                        {
                            GroupRights grpRights = new GroupRights();
                            grpRights.Pagename = dataFields[1];
                            for (int j = 2; j < dataFields.Length; j++)
                            {
                                if (dataFields[j] != string.Empty)
                                {
                                    grpRights.Rights += grpRights.Rights == string.Empty ? dataFields[j] : "," + dataFields[j];
                                }
                            }
                            if (group.groupRights == null)
                            {
                                group.groupRights = new List<GroupRights>();
                            }
                            group.groupRights.Add(grpRights);
                        }
                    }
                }
                results = bl.ManageGroups(group, action, loginOrgId, loginToken);
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }
        public Results CallManageTemplate(string loginOrgId, string loginToken, string action, string methodParams, string data, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            string[] datacollection = null;
            Template tpl = new Template();
            TemplateBL bl = new TemplateBL();
            userdata = methodParams.Split('|');
            string[] IndexDetails = data.Split('!');
            string IndexData = IndexDetails[0].ToString();
            tpl.IndexListDetails = IndexDetails[1].ToString();
            tpl.TagListDetails = IndexDetails[2].ToString();
            string xmlDocument = string.Empty;
            
            if ((action == "AddTemplate" || action == "EditTemplate") && userdata.Length == 5)
            {


                tpl.TemplateName = userdata[0].Trim();
                tpl.Active = Convert.ToBoolean(userdata[1]);
                tpl.TemplateId = Convert.ToInt32(userdata[2].Trim());
                tpl.UploadFIleNameSeperator = userdata[3].Trim();
                tpl.UploadFIleName = userdata[4].Trim();
                if (data != string.Empty)
                {
                    datacollection = IndexData.Split("#".ToCharArray());
                    xmlDocument = "<Index>";
                    for (int i = 0; i < datacollection.Length; i++)
                    {

                        string[] dataFields = datacollection[i].Split('|');
                        if (tpl.IndexFields == null) { tpl.IndexFields = new List<IndexField>(); }

                        if (dataFields.Length > 1)
                        {
                            xmlDocument += "<IndexDetails>";
                            xmlDocument += "<IndexName>" + dataFields[1].Trim() + "</IndexName>";
                            xmlDocument += "<EntryType>" + dataFields[2] + "</EntryType>";
                            xmlDocument += "<DataType>" + dataFields[3] + "</DataType>";
                            xmlDocument += "<MinLength>" + dataFields[4] + "</MinLength>";
                            xmlDocument += "<MaxLength>" + dataFields[5] + "</MaxLength>";
                            xmlDocument += "<Mandatory>" + dataFields[6] + "</Mandatory>";
                            xmlDocument += "<Display>" + dataFields[7] + "</Display>";
                            xmlDocument += "<ActiveIndex>" + dataFields[8] + "</ActiveIndex>";
                            xmlDocument += "<SortOrder>" + dataFields[0] + "</SortOrder>";
                            xmlDocument += "<IndexFldId>" + dataFields[10] + "</IndexFldId>";
                            xmlDocument += "<Haschild>" + dataFields[11] + "</Haschild>";
                            xmlDocument += "</IndexDetails>";

                        }
                    }

                }
                xmlDocument = xmlDocument + "</Index>";
                tpl.IndexFieldDetails = xmlDocument;

                results = bl.ManageTemplates(tpl, action, loginOrgId, loginToken);
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }

        public Results CallSearchDocumentTypes(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            DocumentTypeBL bl = new DocumentTypeBL();
            DocumentType docType = new DocumentType();
            userdata = methodParams.Split('|');
            if (userdata.Length == 6)
            {
                string subAction = string.Empty;
                string deleteMessage = string.Empty;
                if (action == "DeleteDocumentTypeAndSearch")
                {
                    subAction = "DeleteDocumentType";

                    docType.DocumentTypeName = userdata[5].Trim();
                    docType.DocumentTypeId = Convert.ToInt32(userdata[4].Trim());
                    results = bl.ManageDocumentType(docType, subAction, loginOrgId, loginToken);
                    if (results.ActionStatus == "SUCCESS")
                    {
                        deleteMessage = results.Message;
                        subAction = "SearchDocumentTypes";
                    }
                }
                else if (action == "ExportDocumentType")
                {
                    filter.CurrDocumentTypeId = Convert.ToInt32(userdata[4].Trim());
                    subAction = "ExportDocumentType";
                }
                else
                {
                    subAction = "SearchDocumentTypes";
                }
                if (subAction == "SearchDocumentTypes" || subAction == "ExportDocumentType")//if the deletion is success or only search is to be done, execute the below code
                {

                    filter.DocumentTypeName = userdata[0].Trim();
                    filter.FromDate = userdata[1].Trim();
                    filter.ToDate = userdata[2].Trim();
                    filter.PageId = Convert.ToInt32(userdata[3]);

                    results = bl.SearchDocumentTypes(filter, subAction, loginOrgId, loginToken);
                    if (results.ActionStatus == "SUCCESS")
                    {
                        results.Message = deleteMessage;
                        int count = 0;
                        if (results.DocumentTypes != null) { count = results.DocumentTypes.Count; }
                        if (count == 0)
                        {
                            results.RecordCountText = "No Records Found. Please try with another Search Criteria";
                            results.PagingText = " ";
                        }
                        else
                        {
                            results.RecordCountText = "Displaying 1 to " + count.ToString() + " of " + count.ToString() + " Record(s)";
                            results.PagingText = "Page 1";
                        }
                    }
                    else
                    {
                        results.Message = deleteMessage + "</br>" + results.Message;
                    }
                }
                else
                {
                    serviceFailed = true;
                }
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }
        public Results CallSearchTemplates(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            TemplateBL bl = new TemplateBL();
            userdata = methodParams.Split('|');
            if (userdata.Length == 6)
            {
                string subAction = string.Empty;
                string deleteMessage = string.Empty;
                if (action == "DeleteTemplateAndSearch")
                {
                    subAction = "DeleteTemplate";
                    Template docType = new Template();
                    docType.TemplateName = userdata[5].Trim();
                    docType.TemplateId = Convert.ToInt32(userdata[4].Trim());
                    results = bl.ManageTemplates(docType, subAction, loginOrgId, loginToken);
                    if (results.ActionStatus == "SUCCESS")
                    {
                        deleteMessage = results.Message;
                        subAction = "SearchTemplates";
                    }
                }
                else
                {
                    subAction = "SearchTemplates";
                }
                if (subAction == "SearchTemplates")//if the deletion is success or only search is to be done, execute the below code
                {
                    filter.TemplateName = userdata[0].Trim();
                    filter.FromDate = userdata[1].Trim();
                    filter.ToDate = userdata[2].Trim();
                    filter.PageId = Convert.ToInt32(userdata[3]);

                    results = bl.SearchTemplates(filter, subAction, loginOrgId, loginToken);
                    if (results.ActionStatus == "SUCCESS")
                    {
                        results.Message = deleteMessage;
                        int count = 0;
                        if (results.Templates != null) { count = results.Templates.Count; }
                        if (count == 0)
                        {
                            results.RecordCountText = "No Records Found. Please try with another Search Criteria";
                            results.PagingText = " ";
                        }
                        else
                        {
                            results.RecordCountText = "Displaying 1 to " + count.ToString() + " of " + count.ToString() + " Record(s)";
                            results.PagingText = "Page 1";
                        }
                    }
                    else
                    {
                        results.Message = deleteMessage + "</br>" + results.Message;
                    }
                }
                else
                {
                    serviceFailed = true;
                }
            }
            else
            {
                serviceFailed = true;
            }
            return results;
        }
        public Results CallSearchDocuments(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            DocumentBL bl = new DocumentBL();
            userdata = methodParams.Split('|');
            filter.SearchOption = userdata[0];
            filter.DocumentTypeID = Convert.ToInt32(userdata[1]);
            filter.DepartmentID = Convert.ToInt32(userdata[2]);
            filter.Refid = userdata[3].Trim();
            filter.keywords = userdata[4].Trim();
            filter.Active = Convert.ToInt32(userdata[5].Trim());
            filter.MainTagID = Convert.ToInt32(userdata[6].Trim());
            filter.SubTagID = Convert.ToInt32(userdata[7].Trim());
            filter.DocPageNo = Convert.ToInt32(userdata[8].Trim());
            filter.RowsPerPage = 10;
            filter.WhereClause = userdata[10];

            results = bl.SearchDocuments(filter, action, loginOrgId, loginToken);
            if (results.ActionStatus == "SUCCESS")
            {
                int count = 0, TotalRowcount = 0;
                if (results.DocumentDownloads != null) { count = results.DocumentDownloads[0].RowsCount; TotalRowcount = results.DocumentDownloads[0].TotalRowcount; }
                if (count == 0)
                {
                    results.DocumentDownloads[0].HtmlTable = "";
                    results.RecordCountText = "No Records Found. Please try with another Search Criteria";
                    results.PagingText = " ";
                }
                else
                {

                    int intMaxVal = (filter.DocPageNo * filter.RowsPerPage) < TotalRowcount ? (filter.DocPageNo * filter.RowsPerPage) : TotalRowcount;
                    results.RecordCountText = "Displaying " + (((filter.DocPageNo - 1) * (filter.RowsPerPage) + 1)) + " - " + intMaxVal + " out of " + TotalRowcount + " Record(s)";
                    results.PagingText = "Page No " + filter.DocPageNo.ToString();
                }
            }
            return results;
        }
        public Results CallSearchDepartmentsForDeptPage(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            DepartmentBL bl = new DepartmentBL();
            userdata = methodParams.Split('|');

            filter.DepartmentName = userdata[0];
            filter.StartDate = userdata[1];
            filter.EndDate = userdata[2];
            filter.DepartmentID = Convert.ToInt32(userdata[4]);
            results = bl.SearchDepartments(filter, action, loginOrgId, loginToken);
            if (results.ActionStatus == "SUCCESS")
            {
                int count = 0;
                if (results.Departments != null) { count = results.Departments.Count; }
                if (count == 0)
                {
                    results.RecordCountText = "No Records Found. Please try with another Search Criteria";
                    results.PagingText = " ";
                }
                else
                {
                    results.RecordCountText = "Displaying 1 to " + count.ToString() + " of " + count.ToString() + " Record(s)";
                    results.PagingText = "Page 1";
                }
            }
            return results;
        }

        public Results CallGetBatchUploadData(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            BatchUploadBL bl = new BatchUploadBL();
            userdata = methodParams.Split('|');

            filter.DocumentTypeID = Convert.ToInt32(userdata[0].Trim());
            filter.BatchUploadFilterId = Convert.ToInt32(userdata[1].Trim());
            filter.DepartmentID = Convert.ToInt32(userdata[2].Trim());
            filter.DeleteID = Convert.ToInt32(userdata[3].Trim());

            filter.DocPageNo = Convert.ToInt32(userdata[4].Trim());
            filter.RowsPerPage = Convert.ToInt32(userdata[5].Trim());

            results = bl.GetBatchUploadData(filter, action, loginOrgId, loginToken);
            if (results.ActionStatus == "SUCCESS")
            {
                int count = 0, TotalRowcount=0;
                if (results.BatchUpload != null) 
                {
                    if (results.BatchUpload.Count != 0)
                    {
                        count = results.BatchUpload[0].RowsCount; TotalRowcount = results.BatchUpload[0].TotalRowcount;
                    }
                }
                if (count == 0)
                {
                   // results.BatchUpload[0].batchData = "";
                    results.RecordCountText = "No Records Found. Please try with another Search Criteria";
                    results.PagingText = " ";
                }
                else
                {
                    int intMaxVal = (filter.DocPageNo * filter.RowsPerPage) < TotalRowcount ? (filter.DocPageNo * filter.RowsPerPage) : TotalRowcount;
                    results.RecordCountText = "Displaying " + (((filter.DocPageNo - 1) * (filter.RowsPerPage) + 1)) + " - " + intMaxVal + " out of " + TotalRowcount + " Record(s)";
                    results.PagingText = "Page No " + filter.DocPageNo.ToString();
                }
            }

            return results;
        }

        public Results CallManageTag(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            DocumentBL bl = new DocumentBL();
            userdata = methodParams.Split('|');

            filter.UploadDocID = Convert.ToInt32(userdata[0].Trim());
            filter.TotalPages = Convert.ToInt32(userdata[1].Trim());
            //changed as a part of tag search//
            filter.taggedPages =userdata[2].Trim();
            //changed as a part of tag search//
            filter.MainTagID = Convert.ToInt32(userdata[3].Trim());
            filter.SubTagID = Convert.ToInt32(userdata[4].Trim());
            results = bl.ManageTagDetails(filter, action, loginOrgId, loginToken);
            return results;
        }

        public Results CallGetTagDetails(string loginOrgId, string loginToken, string action, string methodParams, string browser, ref bool serviceFailed)
        {
            Results results = new Results();
            string[] userdata = null;
            SearchFilter filter = new SearchFilter();
            DocumentBL bl = new DocumentBL();
            userdata = methodParams.Split('|');

            filter.UploadDocID = Convert.ToInt32(userdata[0].Trim());
            filter.PageId = Convert.ToInt32(userdata[1].Trim());
            results = bl.GetTagDetails(filter, action, loginOrgId, loginToken);
            return results;
        }
        #endregion
        //public string GetData(int value)
        //{
        //    return string.Format("You entered: {0}", value);
        //}

        //public CompositeType GetDataUsingDataContract(CompositeType composite)
        //{
        //    if (composite.BoolValue)
        //    {
        //        composite.StringValue += "Suffix";
        //    }
        //    return composite;
        //}



        //#region ICoreService Members

        //public Results PostScalar(string action, string methodParams)
        //{
        //    Results results = new Results();
        //    results.status = 0; //Failed
        //    results.message = "Service Unavaialble. Please try later.";
        //    results.UserBaseData = new UserBase();
        //    results.UserBaseData.Token = string.Empty;
        //    return results;
        //}

        //#endregion       
    }
}
