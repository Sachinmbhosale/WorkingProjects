using System;
using System.Collections.Generic;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class SecurityDAL : BaseDAL
    {
        #region Constructor
        public SecurityDAL() { }
        #endregion

        #region Login Manger
        public Results ValidateUserManagement(ref User loginUser, string action)
        {
            IDataReader dbRdr = null;
            Results rs = new Results();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(7);
                dbManager.AddParameters(0, "@in_vUserName", loginUser.UserName);
                dbManager.AddParameters(1, "@in_bDomainUser", loginUser.DomainUser);
                dbManager.AddParameters(2, "@in_iDomainID", loginUser.DomainId);
                dbManager.AddParameters(3, "@in_vPassword", loginUser.Password);
                dbManager.AddParameters(4, "@in_vLoginOrgCode", loginUser.LoginOrgCode);
                dbManager.AddParameters(5, "@in_vLoginToken", loginUser.LoginToken);
                dbManager.AddParameters(6, "@in_vAction", action);

                dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_ValidateUser");

                if (dbRdr.Read())
                {
                    rs.ActionStatus = dbRdr["UserStatus"].ToString();
                    if (action == Actions.ValidateUser.ToString() || action == Actions.ValidateUserExplicitly.ToString())
                    {
                        if (rs.ActionStatus == ActionStatus.SUCCESS.ToString() || rs.ActionStatus == ActionStatus.EXPAIRED.ToString())
                        {
                            rs.UserData = new UserBase();
                            rs.UserData.UserId = Convert.ToInt32(dbRdr["UserId"].ToString());
                            rs.UserData.LoginOrgId = Convert.ToInt32(dbRdr["LoginOrgId"].ToString());
                            rs.UserData.LoginToken = dbRdr["LoginToken"].ToString();
                        }
                    }
                    else if (action == Actions.ForgotPassword.ToString())
                    {
                        rs.UserData = new UserBase();
                        rs.UserData.LoginOrgId = Convert.ToInt32(dbRdr["LoginOrgId"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return rs;
        }
        /// <summary>
        /// This function is used to manage users including login
        /// </summary>
        /// <param name="loginUser"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Results LoggedUserManagement(ref User loginUser, string action)
        {
            IDataReader dbRdr = null;
            Results rs = new Results();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(8);
                dbManager.AddParameters(0, "@in_vUserName", loginUser.UserName);
                dbManager.AddParameters(1, "@in_vPassword", loginUser.Password);
                dbManager.AddParameters(2, "@in_vNewPassword", loginUser.NewPassword);
                dbManager.AddParameters(3, "@in_vLoginToken", loginUser.LoginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginUser.LoginOrgId);
                dbManager.AddParameters(5, "@in_bAcceptConfidentailtyAgreement", loginUser.ConfidentialityAgreement);
                dbManager.AddParameters(6, "@in_vAction", action);
                dbManager.AddParameters(7, "@in_UserEmail", loginUser.EmailId);
                //if (action != "ChangePassword")
                //{
                //    //rs.ResultDS = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_LoginManagement");
                //}
                dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_LoginManagement");


                if (dbRdr.Read())
                {
                    rs.ActionStatus = dbRdr["UserStatus"].ToString();
                    if (action == Actions.UserLogin.ToString())
                    {
                        if (rs.ActionStatus == ActionStatus.SUCCESS.ToString() || rs.ActionStatus == ActionStatus.EXPAIRED.ToString())
                        {
                            rs.UserData = new UserBase();
                            rs.UserData.UserId = Convert.ToInt32(dbRdr["UserId"].ToString());
                            rs.UserData.UserName = dbRdr["UserName"].ToString();
                            rs.UserData.EmailId = dbRdr["EmailId"].ToString();
                            if (dbRdr["ConfidentialityAgreement"].ToString() == "True")
                            {
                                rs.UserData.ConfidentialityAgreement = true;
                            }
                            else
                            {
                                rs.UserData.ConfidentialityAgreement = false;
                            }
                            rs.UserData.LastName = dbRdr["LastName"].ToString();
                            rs.UserData.FirstName = dbRdr["FirstName"].ToString();
                            rs.UserData.GroupId = Convert.ToInt32(dbRdr["GroupId"].ToString());
                            rs.UserData.UserOrgId = Convert.ToInt32(dbRdr["UserOrgId"].ToString());
                            rs.UserData.UserParentOrgId = Convert.ToInt32(dbRdr["UserParentOrgId"].ToString());
                            rs.UserData.LoginOrgId = Convert.ToInt32(dbRdr["LoginOrgId"].ToString());
                            rs.UserData.LoginParentOrgId = Convert.ToInt32(dbRdr["LoginParentOrgId"].ToString());
                            rs.UserData.LoginOrgName = dbRdr["LoginOrgName"].ToString();
                            rs.UserData.LoginToken = dbRdr["LoginToken"].ToString();
                            rs.UserData.UserLoggedInTime = Convert.ToDateTime(dbRdr["UserLoggedInTime"].ToString());
                            if (dbRdr["UserLastLoggedInTime"].ToString() == string.Empty)
                            {
                                rs.UserData.UserLastLoggedInTime = Convert.ToDateTime(dbRdr["UserLoggedInTime"].ToString());
                            }
                            else
                            {
                                rs.UserData.UserLastLoggedInTime = Convert.ToDateTime(dbRdr["UserLastLoggedInTime"].ToString());
                            }
                            if (dbRdr["IsDomainUser"].ToString() == "True")
                            {
                                rs.UserData.IsDomainUser = true;
                            }
                            else
                            {
                                rs.UserData.IsDomainUser = false;
                            }
                            rs.UserData.LanguageID = Convert.ToInt32(dbRdr["LanguageID"].ToString());
                            rs.UserData.LanguageCode = dbRdr["LanguageCode"].ToString();
                            rs.UserData.OutofOffice = Convert.ToInt32(dbRdr["OutofOffice"].ToString() == "True" ? "1" : "0");
                            rs.UserData.GroupName = dbRdr["GroupName"].ToString().Trim();
                        }
                    }
                    else if (action == Actions.ChangePassword.ToString())
                    {
                        //do nothing.   Only status is required.
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return rs;
        }

        /// <summary>       
        /// This function is used to get user information with filters
        /// </summary>
        /// <param name="loginUser"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Results GetUsersByUserNameOrgId(string action, string userName, string orgName, int loginOrgId)
        {
            Results rs = new Results();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);

                dbManager.AddParameters(0, "@in_vUserName", userName);
                dbManager.AddParameters(1, "@in_vOrgName", orgName);
                dbManager.AddParameters(2, "@in_iOrgId", loginOrgId);
                dbManager.AddParameters(3, "@in_vAction", action);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetUsersByUserNameOrgId"))
                {
                    while (dbRdr.Read())
                    {
                        rs.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (rs.ActionStatus == ActionStatus.SUCCESS.ToString())
                        {
                            if (rs.Users == null)
                            {
                                rs.Users = new List<User>();
                            }
                            User user = new User();
                            user.UserId = Convert.ToInt32(dbRdr["UserId"].ToString());
                            user.UserOrgId = Convert.ToInt32(dbRdr["OrgId"].ToString());
                            user.LastName = dbRdr["LastName"].ToString();
                            user.FirstName = dbRdr["FirstName"].ToString();
                            if (action == "GetUserDataToSendPassword")
                            {
                                user.Password = dbRdr["Password"].ToString();
                            }
                            user.UserName = dbRdr["UserName"].ToString();
                            user.EmailId = dbRdr["UserEmailId"].ToString();
                            user.OrgEmailId = dbRdr["OrgEmailId"].ToString();
                            rs.Users.Add(user);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return rs;
        }

        #endregion
        #region Organisation Management
        public Results SearchOrgs(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(8);

                DateTime? FromDate = null;
                FromDate = filter.FromDate.Length > 0 ? FormatScriptDateToSystemDate(filter.FromDate) : FromDate;
                DateTime? ToDate = null;
                ToDate = filter.ToDate.Length > 0 ? FormatScriptDateToSystemDate(filter.ToDate) : ToDate;

                dbManager.AddParameters(0, "@in_iCurrOrgId", filter.CurrOrgId);
                dbManager.AddParameters(1, "@in_vOrgName", filter.OrgName);
                dbManager.AddParameters(2, "@in_dCreatedDateFrom", FromDate);
                dbManager.AddParameters(3, "@in_dCreatedDateTo", ToDate);
                dbManager.AddParameters(4, "@in_vAction", action);
                dbManager.AddParameters(5, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(6, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(7, "@in_vOrgEmailId", filter.OrgEmailId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_SearchOrgsById"))
                {
                    while (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                        {
                            if (results.Orgs == null)
                            {
                                results.Orgs = new List<Org>();
                            }
                            Org org = new Org();
                            org.OrgId = Convert.ToInt32(dbRdr["OrgId"].ToString());
                            org.OrgParentId = Convert.ToInt32(dbRdr["ParentOrgID"].ToString());
                            org.OrgName = dbRdr["OrgName"].ToString();
                            org.OrgCode = dbRdr["OrgCode"].ToString();
                            org.OrgAddress = dbRdr["OrgAddress"].ToString();
                            org.PhoneNo = dbRdr["PhoneNo"].ToString();
                            org.FaxNo = dbRdr["FaxNo"].ToString();
                            org.ContactPerson = dbRdr["ContactPerson"].ToString();
                            org.ContactMobile = dbRdr["ContactMobile"].ToString();
                            org.OrgEmailId = dbRdr["OrgEmailId"].ToString();
                            org.OrgGreeting = dbRdr["OrgGreeting"].ToString(); // newly added for selecting Organization greeting 6/feb/2014 vijay
                            org.OrgDetails = dbRdr["OrgDetails"].ToString(); // newly added for selecting Organization details 6/feb/2014 vijay
                            org.CreatedDate = ((dbRdr["CreatedDate"].ToString()));

                            results.Orgs.Add(org);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            /*
            dbCmd.CommandText = "USP_SearchOrgsById";
            if (filter.CurrOrgId > 0)
            {
                dbCmd.Parameters.Add("@in_iCurrOrgId", SqlDbType.Int).Value = filter.CurrOrgId;
            }
            if (filter.OrgName != string.Empty)
            {
                dbCmd.Parameters.Add("@in_vOrgName", SqlDbType.VarChar, 300).Value = filter.OrgName;
            }
            if (filter.FromDate != string.Empty)
            {
                dbCmd.Parameters.Add("@in_vCreaedDateFrom", SqlDbType.DateTime).Value = FormatScriptDateToSystemDate(filter.FromDate);
            }
            if (filter.ToDate != string.Empty)
            {
                dbCmd.Parameters.Add("@in_vCreaedDateTo", SqlDbType.DateTime).Value = FormatScriptDateToSystemDate(filter.ToDate);
            }
            dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
            //sesstion
            dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
            dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
            if (filter.OrgEmailId != string.Empty)
            {
                dbCmd.Parameters.Add("@in_vOrgEmailId", SqlDbType.VarChar, 300).Value = filter.OrgEmailId;
            }
            dbRdr = dbCmd.ExecuteReader();
            */

            return results;
        }

        public Results GetOrgById(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            DataSet ds = new DataSet();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);
                dbManager.AddParameters(0, "@in_iCurrOrgId", filter.CurrOrgId);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);
                if (action == "GetOrgApplication")
                {
                   ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetOrgsById");
                   results.ResultDS = ds;
                }
                else
                {
                    using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetOrgsById"))
                    {
                        while (dbRdr.Read())
                        {
                            results.ActionStatus = dbRdr["ActionStatus"].ToString();
                            if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                            {
                                if (results.Orgs == null)
                                {
                                    results.Orgs = new List<Org>();
                                }
                                Org org = new Org();
                                org.OrgId = Convert.ToInt32(dbRdr["OrgId"].ToString());
                                org.OrgName = dbRdr["OrgName"].ToString();
                                org.OrgAddress = dbRdr["OrgAddress"].ToString();
                                org.PhoneNo = dbRdr["PhoneNo"].ToString();
                                org.FaxNo = dbRdr["FaxNo"].ToString();
                                org.ContactPerson = dbRdr["ContactPerson"].ToString();
                                org.ContactMobile = dbRdr["ContactMobile"].ToString();
                                org.OrgEmailId = dbRdr["OrgEmailId"].ToString();

                                org.CreatedDate = Convert.ToDateTime(dbRdr["CreatedDate"].ToString()).ToShortDateString();
                                org.LogoPath = dbRdr["LogoPath"].ToString();
                                org.OrgGreeting = dbRdr["OrgGreeting"].ToString(); // newly added for selecting Organization greeting 6/feb/2014 vijay
                                org.OrgDetails = dbRdr["OrgDetails"].ToString(); // newly added for selecting Organization Details 6/feb/2014 vijay                          
                                results.Orgs.Add(org);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return results;
        }

        public Results ManageOrgs(Org customer, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(15);

                dbManager.AddParameters(0, "@in_iCurrOrgId", customer.OrgId);
                dbManager.AddParameters(1, "@in_vOrgName", customer.OrgName);
                dbManager.AddParameters(2, "@in_vOrgAddress", customer.OrgAddress);
                dbManager.AddParameters(3, "@in_vOrgEmailId", customer.OrgEmailId);
                dbManager.AddParameters(4, "@in_vPhoneNo", customer.PhoneNo);
                dbManager.AddParameters(5, "@in_vFaxNo", customer.FaxNo);
                dbManager.AddParameters(6, "@in_vContactPerson", customer.ContactPerson);
                dbManager.AddParameters(7, "@in_vContactMobile", customer.ContactMobile);
                dbManager.AddParameters(8, "@in_vLogoName", customer.LogoFileName);
                dbManager.AddParameters(9, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(10, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(11, "@in_vAction", action);
                dbManager.AddParameters(12, "@in_vOrgGreeting", customer.OrgGreeting);
                dbManager.AddParameters(13, "@in_vOrgDetails", customer.OrgDetails);
                dbManager.AddParameters(14, "@in_vApplicationId", customer.AppId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_ManageOrg"))
                {
                    if (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        results.IdentityId = dbRdr["OrgId"].ToString();
                        results.NewOrgCode = dbRdr["OrgCode"].ToString();
                        results.UserPassData = dbRdr["UserPassData"].ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return results;
        }

        public Results GetApplication(string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(6);

                dbManager.AddParameters(0, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(1, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(2, "@in_vAction", action);
                dbManager.AddParameters(3, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(4, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(5, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetApplications");
              
                results.ResultDS = ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return results;
        }
        #endregion
    }
}
