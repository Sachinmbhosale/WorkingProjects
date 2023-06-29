using System;
using System.Collections.Generic;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;


namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class GroupDAL : BaseDAL
    {
        public GroupDAL() { }
        public Results SearchGroups(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            DateTime? FromDate = null;
            FromDate = filter.FromDate.Length > 1 ? FormatScriptDateToSystemDate(filter.FromDate) : FromDate;

            DateTime? Todate = null;
            Todate = filter.ToDate.Length > 0 ? FormatScriptDateToSystemDate(filter.ToDate) : Todate;

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(7);
                dbManager.AddParameters(0, "@in_iCurrGroupId", filter.CurrGroupId);
                dbManager.AddParameters(1, "@in_vGroupName", filter.GroupName);
                dbManager.AddParameters(2, "@in_vCreaedDateFrom", FromDate);
                dbManager.AddParameters(3, "@in_vCreaedDateTo", Todate);
                dbManager.AddParameters(4, "@in_vAction", action);
                dbManager.AddParameters(5, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(6, "@in_iLoginOrgId", loginOrgId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_SearchGroupsById"))
                {
                    while (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.ActionStatus == "SUCCESS")
                        {
                            if (results.Groups == null)
                            {
                                results.Groups = new List<Group>();
                            }
                            Group group = new Group();
                            group.GroupId = Convert.ToInt32(dbRdr["GroupId"].ToString());
                            group.GroupName = dbRdr["GroupName"].ToString();
                            group.Description = dbRdr["Description"].ToString();
                            group.UserCount = Convert.ToInt32(dbRdr["UserCount"].ToString());
                            group.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                            group.CreatedDate = FormatSystemDateToScriptDate(Convert.ToDateTime(dbRdr["CreatedDate"].ToString()));
                            group.Fixed = Convert.ToBoolean(dbRdr["Fixed"].ToString());
                            results.Groups.Add(group);
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
            /*SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "USP_SearchGroupsById";
                if (filter.CurrGroupId > 0)
                {
                    dbCmd.Parameters.Add("@in_iCurrGroupId", SqlDbType.Int).Value = filter.CurrGroupId;
                }
                if (filter.GroupName != string.Empty)
                {
                    dbCmd.Parameters.Add("@in_vGroupName", SqlDbType.VarChar, 300).Value = filter.GroupName;
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
                dbRdr = dbCmd.ExecuteReader();

                while (dbRdr.Read())
                {
                    results.ActionStatus = dbRdr["ActionStatus"].ToString();
                    if (results.ActionStatus == "SUCCESS")
                    {
                        if (results.Groups == null)
                        {
                            results.Groups = new List<Group>();
                        }
                        Group group = new Group();
                        group.GroupId = Convert.ToInt32(dbRdr["GroupId"].ToString());
                        group.GroupName = dbRdr["GroupName"].ToString();
                        group.Description = dbRdr["Description"].ToString();
                        group.UserCount = Convert.ToInt32(dbRdr["UserCount"].ToString());
                        group.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                        group.CreatedDate = FormatSystemDateToScriptDate(Convert.ToDateTime(dbRdr["CreatedDate"].ToString()));
                        group.Fixed = Convert.ToBoolean(dbRdr["Fixed"].ToString());
                        results.Groups.Add(group);
                    }
                }
            }
            catch (Exception ex)
            {
                results.Message = ex.ToString();
                throw ex;
            }
            finally
            {
                if (dbRdr != null)
                {
                    dbRdr.Dispose();
                }
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return results;*/
        }
        public Results ManageGroups(Group group, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            IDataReader dbRdr = null;

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(8);
                dbManager.AddParameters(0, "@in_iCurrGroupId", group.GroupId);
                dbManager.AddParameters(1, "@in_vGroupName", group.GroupName);
                dbManager.AddParameters(2, "@in_vDescription", group.Description);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(5, "@in_vAction", action);
                dbManager.AddParameters(6, "@in_bFixed", group.Fixed);
                dbManager.AddParameters(7, "@in_bActive", group.Active);

               // dbManager.Connection.BeginTransaction();
                using (dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_ManageGroup"))
                {
                    if (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                        {
                            int currentGroupId = 0;
                            if (action == "AddGroup")
                            {
                                currentGroupId = Convert.ToInt32(dbRdr["IdentityId"].ToString());
                            }
                            else
                            {
                                currentGroupId = group.GroupId;
                            }
                            results.ActionStatus = dbRdr["ActionStatus"].ToString();
                                                       
                            dbManager.CreateParameters(6);
                            dbManager.AddParameters(0, "@in_iPageName", 0);
                            dbManager.AddParameters(1, "@in_vGroupRights", string.Empty);
                            dbManager.AddParameters(2, "@in_vAction", "ExpaireRights");
                            dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                            dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);
                            dbManager.AddParameters(5, "@in_iCurrGroupId", currentGroupId);

                            dbRdr.Close();
                            using (dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_InsertGroupRights"))
                            {
                                if (dbRdr.Read() && dbRdr["ActionStatus"].ToString() == ActionStatus.SUCCESS.ToString())
                                {
                                    results.ActionStatus = dbRdr["ActionStatus"].ToString();
                                    if (group.groupRights != null)
                                    {

                                        foreach (GroupRights grpRights in group.groupRights)
                                        {
                                            dbManager.CreateParameters(6);
                                            dbManager.AddParameters(0, "@in_iPageName", grpRights.Pagename);
                                            dbManager.AddParameters(1, "@in_vGroupRights", grpRights.Rights);
                                            dbManager.AddParameters(2, "@in_vAction", "InsertRights");
                                            dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                                            dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);
                                            dbManager.AddParameters(5, "@in_iCurrGroupId", currentGroupId);

                                            dbRdr.Close();
                                            using (dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_InsertGroupRights"))
                                            {
                                                if (dbRdr.Read())
                                                {
                                                    results.ActionStatus = dbRdr["ActionStatus"].ToString();
                                                    if (results.ActionStatus != ActionStatus.SUCCESS.ToString())
                                                    {
                                                        Exception ex = new Exception();
                                                        throw ex;
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (dbRdr != null)
                            {
                                dbRdr.Close();
                                
                            }                           
                           
                            return results;
                        }
                    }
                    if (dbRdr != null)
                    {
                        dbRdr.Close();                       
                    }
                    
                }
            //    dbManager.CommitTransaction();
            }
            catch (Exception ex)
            {
              //  dbManager.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return results;
            /*SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            SqlTransaction myTrans = null;
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                myTrans = dbCon.BeginTransaction();
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.Transaction = myTrans;


                dbCmd.CommandText = "USP_ManageGroup";
                dbCmd.Parameters.Add("@in_iCurrGroupId", SqlDbType.Int).Value = group.GroupId;

                dbCmd.Parameters.Add("@in_vGroupName", SqlDbType.VarChar, 100).Value = group.GroupName;
                dbCmd.Parameters.Add("@in_vDescription", SqlDbType.VarChar, 1000).Value = group.Description;
                //session
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                dbCmd.Parameters.Add("@in_bFixed", SqlDbType.Bit).Value = group.Fixed;

                dbRdr = dbCmd.ExecuteReader();

                if (dbRdr.Read())
                {
                    results.ActionStatus = dbRdr["ActionStatus"].ToString();
                    if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                    {
                        int currentGroupId = 0;
                        if (action == "AddGroup")
                        {
                            currentGroupId = Convert.ToInt32(dbRdr["IdentityId"].ToString());
                        }
                        else
                        {
                            currentGroupId = group.GroupId;
                        }
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();

                        dbRdr.Close();
                        dbCmd = new SqlCommand();
                        dbCmd.Connection = dbCon;
                        dbCmd.CommandType = CommandType.StoredProcedure;
                        dbCmd.Transaction = myTrans;
                        dbCmd.CommandText = "USP_InsertGroupRights";
                        dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = "ExpaireRights";
                        dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                        dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                        dbCmd.Parameters.Add("@in_iCurrGroupId", SqlDbType.Int).Value = currentGroupId;

                        dbRdr = dbCmd.ExecuteReader();

                        if (dbRdr.Read() && dbRdr["ActionStatus"].ToString() == ActionStatus.SUCCESS.ToString())
                        {
                            results.ActionStatus = dbRdr["ActionStatus"].ToString();
                            if (group.groupRights != null)
                            {

                                foreach (GroupRights grpRights in group.groupRights)
                                {
                                    dbRdr.Close();
                                    dbCmd = new SqlCommand();
                                    dbCmd.Connection = dbCon;
                                    dbCmd.CommandType = CommandType.StoredProcedure;
                                    dbCmd.Transaction = myTrans;
                                    dbCmd.CommandText = "USP_InsertGroupRights";
                                    dbCmd.Parameters.Add("@in_iPageName", SqlDbType.VarChar, 100).Value = grpRights.Pagename;
                                    dbCmd.Parameters.Add("@in_vGroupRights", SqlDbType.VarChar, 500).Value = grpRights.Rights;
                                    dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = "InsertRights";
                                    dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                                    dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                                    dbCmd.Parameters.Add("@in_iCurrGroupId", SqlDbType.Int).Value = currentGroupId;
                                    dbRdr = dbCmd.ExecuteReader();

                                    if (dbRdr.Read())
                                    {
                                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                                        if (results.ActionStatus != ActionStatus.SUCCESS.ToString())
                                        {
                                            Exception ex = new Exception();
                                            throw ex;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dbRdr != null)
                        {
                            dbRdr.Close();
                            dbRdr = null;
                        }
                        myTrans.Rollback();
                        return results;
                    }
                }
                if (dbRdr != null)
                {
                    dbRdr.Close();
                    dbRdr = null;
                }
                myTrans.Commit();
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
                results.Message = ex.ToString();
                if (results.ActionStatus != string.Empty)
                {
                    results.ActionStatus = ActionStatus.ERROR.ToString();
                }
                throw ex;
            }
            finally
            {
                if (dbRdr != null)
                {
                    dbRdr.Dispose();
                }
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return results;*/
        }
        public Results GetGroupRightsWithGroupId(int currentGroupId, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";


            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);
                dbManager.AddParameters(0, "@in_iCurrGroupId", currentGroupId);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetGroupRightsWithGroupId"))
                {
                    while (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                        {
                            if (results.Groups == null)
                            {
                                results.Groups = new List<Group>();
                                Group grp = new Group();
                                results.Groups.Add(grp);
                                results.Groups[0].groupRights = new List<GroupRights>();

                            }
                            GroupRights right = new GroupRights();
                            right.Pagename = dbRdr["PageName"].ToString();
                            right.Rights = dbRdr["Rights"].ToString();

                            results.Groups[0].groupRights.Add(right);
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

            /*SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "USP_GetGroupRightsWithGroupId";

                dbCmd.Parameters.Add("@in_iCurrGroupId", SqlDbType.Int).Value = currentGroupId;

                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                //sesstion
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                dbRdr = dbCmd.ExecuteReader();

                while (dbRdr.Read())
                {
                    results.ActionStatus = dbRdr["ActionStatus"].ToString();
                    if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                    {
                        if (results.Groups == null)
                        {
                            results.Groups = new List<Group>();
                            Group grp = new Group();
                            results.Groups.Add(grp);
                            results.Groups[0].groupRights = new List<GroupRights>();

                        }
                        GroupRights right = new GroupRights();
                        right.Pagename = dbRdr["PageName"].ToString();
                        right.Rights = dbRdr["Rights"].ToString();

                        results.Groups[0].groupRights.Add(right);
                    }
                }
            }
            catch (Exception ex)
            {
                results.Message = ex.ToString();
                throw ex;
            }
            finally
            {
                if (dbRdr != null)
                {
                    dbRdr.Dispose();
                }
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return results;*/
        }
        public Results GetAppPages(int loginParentOrgId, string CommaSeparatedApplicationIds)
        {
            Results results = new Results();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(2);
                dbManager.AddParameters(0, "@in_iLoginParentOrgId", loginParentOrgId);
                dbManager.AddParameters(1, "@in_vCommaSeparatedApplicationIds", CommaSeparatedApplicationIds);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetAppPages"))
                {

                    while (dbRdr.Read())
                    {
                        //results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.Items == null)
                        {
                            results.Items = new List<Item>();

                        }
                        Item item = new Item();
                        item.Key = dbRdr["Id"].ToString();
                        item.Value = dbRdr["Value"].ToString();
                        results.Items.Add(item);
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

           /* SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;
                dbCmd.Parameters.Add("@in_iLoginParentOrgId", SqlDbType.Int).Value = loginParentOrgId;
                dbCmd.Parameters.Add("@in_vCommaSeparatedApplicationIds", SqlDbType.VarChar).Value = CommaSeparatedApplicationIds;
                dbCmd.CommandText = "USP_GetAppPages";

                dbRdr = dbCmd.ExecuteReader();

                while (dbRdr.Read())
                {
                    //results.ActionStatus = dbRdr["ActionStatus"].ToString();
                    if (results.Items == null)
                    {
                        results.Items = new List<Item>();

                    }
                    Item item = new Item();
                    item.Key = dbRdr["Id"].ToString();
                    item.Value = dbRdr["Value"].ToString();
                    results.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                results.Message = ex.ToString();
                throw ex;
            }
            finally
            {
                if (dbRdr != null)
                {
                    dbRdr.Dispose();
                }
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return results;*/
        }
        public Results GetAppRights(string applicationId)
        {
            Results results = new Results();


            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(1);
                dbManager.AddParameters(0, "@in_vApplicationId", applicationId);
                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetAppRights"))
                {
                    while (dbRdr.Read())
                    {
                        //results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.Items == null)
                        {
                            results.Items = new List<Item>();

                        }
                        Item item = new Item();
                        item.Key = dbRdr["Id"].ToString();
                        item.Value = dbRdr["Value"].ToString();
                        results.Items.Add(item);
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

            /*SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "USP_GetAppRights";

                dbRdr = dbCmd.ExecuteReader();

                while (dbRdr.Read())
                {
                    //results.ActionStatus = dbRdr["ActionStatus"].ToString();
                    if (results.Items == null)
                    {
                        results.Items = new List<Item>();

                    }
                    Item item = new Item();
                    item.Key = dbRdr["Id"].ToString();
                    item.Value = dbRdr["Value"].ToString();
                    results.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                results.Message = ex.ToString();
                throw ex;
            }
            finally
            {
                if (dbRdr != null)
                {
                    dbRdr.Dispose();
                }
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return results;*/
        }
        public DataSet GetMenuItemDatasetByGroupId(int groupId, string PageName, string action, string loginOrgId, string loginToken)
        {
            DataSet dsMenu = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_vPageName", PageName);
                dbManager.AddParameters(1, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(2, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(3, "@in_bAllMenuItem", 0);
                dbManager.AddParameters(4, "@in_vAction", action);

                dsMenu = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetDyanmicMenuItemsByGroupId");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return dsMenu;
        }
        //DMS5-3929M
        public Results GetApps(string action,int LoginOrgId, string LoginToken)
        {
            Results results = new Results();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(6);
                dbManager.AddParameters(0, "@in_vLoginToken", LoginToken);
                dbManager.AddParameters(1, "@in_iLoginOrgId", LoginOrgId);
                dbManager.AddParameters(2, "@in_vAction", action);// DMS5-3929A
                dbManager.AddParameters(3, "@out_iErrorState",0,ParameterDirection.Output);
                dbManager.AddParameters(4, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(5, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetApplications"))
                {
                    while (dbRdr.Read())
                    {
                        //results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.Items == null)
                        {
                            results.Items = new List<Item>();

                        }
                        Item item = new Item();
                        item.Key = dbRdr["Id"].ToString();
                        item.Value = dbRdr["Value"].ToString();
                        results.Items.Add(item);
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


            /*SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar).Value = LoginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = LoginOrgId;
                dbCmd.CommandText = "USP_GetApplications";

                dbRdr = dbCmd.ExecuteReader();

                while (dbRdr.Read())
                {
                    //results.ActionStatus = dbRdr["ActionStatus"].ToString();
                    if (results.Items == null)
                    {
                        results.Items = new List<Item>();

                    }
                    Item item = new Item();
                    item.Key = dbRdr["Id"].ToString();
                    item.Value = dbRdr["Value"].ToString();
                    results.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                results.Message = ex.ToString();
                throw ex;
            }
            finally
            {
                if (dbRdr != null)
                {
                    dbRdr.Dispose();
                }
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return results;*/
        }
    }
}
