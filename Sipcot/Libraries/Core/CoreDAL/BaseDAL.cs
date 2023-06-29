using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class BaseDAL
    {
        public string DbConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString;
            }
        }
        #region DataBase Connection
        public SqlConnection OpenConnection()
        {
            SqlConnection dbCon = new SqlConnection(DbConnectionString);
            dbCon.Open();
            return dbCon;
        }
        //To Close the Connection
        public void CloseConnection(SqlConnection dbCon)
        {
            if (dbCon != null)
            {
                if (dbCon.State == ConnectionState.Open)
                {
                    dbCon.Close();
                }
            }
        }
        #endregion
        public DataProvider ConfiguredDataProvider
        {
            get
            {
                string DatabaseSystem = ConfigurationManager.AppSettings["DatabaseSystem"];
                switch (DatabaseSystem.ToUpper())
                {
                    default:
                    case "SQLSERVER":
                        return DataProvider.SqlServer;
                    case "MYSQL":
                        return DataProvider.MySql;
                }
            }
        }

        #region General Functions
        public DateTime FormatScriptDateToSystemDate(string inDate)
        {
            string[] indateParts = inDate.Split('/');
            return new DateTime(Convert.ToInt32(indateParts[2]), Convert.ToInt32(indateParts[1]), Convert.ToInt32(indateParts[0]));
        }
        public string FormatSystemDateToScriptDate(DateTime outDate)
        {
            return outDate.Day.ToString() + "/" + outDate.Month.ToString() + "/" + outDate.Year;
        }
        /// <summary>
        /// To check the database is accessible
        /// </summary>
        /// <returns></returns>
        public bool DatabaseCheck()
        {
            bool status = false;
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                object objstatus = dbManager.ExecuteScalar(CommandType.StoredProcedure, "USP_DatabaseCheck");
                if ((bool)objstatus)
                {
                    status = true;
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
            return status;
        }

        public bool GetLogoImageName(string loginOrgCode, ref string logoImageName, ref string loginOrgName)
        {
            bool status = false;
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(1);
                dbManager.AddParameters(0, "@in_vLoginOrgCode", loginOrgCode);
                object obj = dbManager.ExecuteScalar(CommandType.StoredProcedure,"USP_GetLogoImageName");

                if (obj.GetType().ToString() != "System.DBNull")
                {
                    string data = (string)obj;
                    string[] dataList = data.Split('|');
                    if (dataList.Length == 2)
                    {
                        logoImageName = dataList[0];
                        loginOrgName = dataList[1];
                        status = true;
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
            return status;
        }

        public static string FrameXML<T>(List<T> list)
        {
            string frameXML = string.Empty;
            if (list != null && list.Count > 0)
            {

                frameXML = "<" + "List" + ">";
                foreach (T t in list)
                {

                    frameXML += "<" + t.GetType().Name + ">";
                    PropertyInfo[] infoPorpoties = t.GetType().GetProperties();
                    for (int i = 0; i < t.GetType().GetProperties().Length; i++)
                    {
                        object val = t.GetType().GetProperties()[i].GetValue(t, null);
                        if (val != null)
                        {

                            frameXML += "<" + t.GetType().GetProperties()[i].Name + ">" + val.ToString()
                                    + "</" + t.GetType().GetProperties()[i].Name + ">";
                        }
                        else
                        {
                            frameXML += "<" + t.GetType().GetProperties()[i].Name + ">" + string.Empty
                                        + "</" + t.GetType().GetProperties()[i].Name + ">";
                        }

                    }

                    frameXML += "</" + t.GetType().Name + ">";
                }
                frameXML += "</" + "List" + ">";
            }
            return frameXML;
        }
        #endregion

        public string createHtmlTable(DataSet ds)
        {
            string yesterday = "", application = "DMS";
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                int indexCount = ds.Tables[0].Rows.Count;

            }
            string MessageBody = "<p> Dear " + "User" + @",<br/><br/>
                                    Report Date: <b>" + yesterday + @"</b><br />
                                    Application : <b>" + application + @"</b> <br /> ";


            string TableStartHTML = "<table id=\"tblData\" class=\"mGrid\" AlternatingRowStyle-CssClass=\"alt\">" +
                                       "<tr>";
            string TableEndHTML = "</tr></table><br /><br />";
            string DisclaimerText = " &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;. <br/><br/> Thanks & Regards, <br/> <b>ABC Corporation</b><br/><br/><br/><font color=#8585AD>PLEASE DON'T REPLY TO THIS MAIL SINCE IT IS SYSTEM GENERATED.</font><br/><br/><br/><br/> ";
            string CompleteMessage = "";
            string FullTableHTML = "";
            int TableCount = ds.Tables.Count;
            string EachRowHtml = "";
            string TableHeaderTDHTML = "";
            string[] EachTableHTML = new string[TableCount]; // string for Each Table HTML Code//
            var RowCount = 0;

            if (TableCount > 0)
            {
                int CurrentTableCount = 0;
                for (int TV = 0; TV < TableCount; TV++)
                {
                    CurrentTableCount = TV;
                    int ColumnCount = ds.Tables[TV].Columns.Count;
                    RowCount = ds.Tables[TV].Rows.Count;
                    string[] ColumnNames = new string[ColumnCount];
                    for (int CTV = 0; CTV < ColumnCount; CTV++)
                    {
                        ColumnNames[CTV] = ds.Tables[TV].Columns[CTV].ColumnName.ToString();


                        TableHeaderTDHTML = TableHeaderTDHTML +
                                            "<th" + "><b>"
                                            + ColumnNames[CTV]
                                            + "</b></th>";


                    }
                    for (int sm = 0; sm < RowCount; sm++)
                    {
                        int ColCnt = ds.Tables[TV].Columns.Count;

                        for (int CR = 0; CR < ColCnt; CR++)
                        {
                            if (CR == 0)
                            {
                                EachRowHtml = EachRowHtml + "<tr>";
                            }

                            string SubRowHTML = "<td>" +

                              ds.Tables[TV].Rows[sm][CR].ToString() + @"</td>";
                            EachRowHtml = EachRowHtml + SubRowHTML;

                            if (CR == ColumnCount - 1)
                            {
                                EachRowHtml = EachRowHtml + "</tr>";
                            }
                        }
                    }
                    EachTableHTML[TV] = TableStartHTML + TableHeaderTDHTML + EachRowHtml + TableEndHTML;
                    FullTableHTML = FullTableHTML + EachTableHTML[TV];
                    TableHeaderTDHTML = string.Empty;
                    EachRowHtml = string.Empty;
                }
            }
            CompleteMessage = MessageBody + FullTableHTML + DisclaimerText; // this string Contains HTML Code for 

            return FullTableHTML;
        }

        public string QueryParser(string query, DataProvider dataProvider)
        {
            string output = query;
            string DatabaseSystem = ConfigurationManager.AppSettings["DatabaseSystem"];
            switch (DatabaseSystem.ToUpper())
            {
                default:
                case "SQLSERVER":
                    break;
                case "MYSQL":
                    output = query.ToLower();
                    output = output.Replace("dbo.", string.Empty);
                    output = output.Replace("(nolock)", string.Empty);
                    output = output.Replace("@", string.Empty);
                    output = output.Replace("=", " default ");
                    output = output.Replace("&apos;", "'");
                    output = output.Replace("isnull", "ifnull");
                    output = output.Replace("getdate()", "now()");

                    break;
            }
            return output;
        }
    }
}