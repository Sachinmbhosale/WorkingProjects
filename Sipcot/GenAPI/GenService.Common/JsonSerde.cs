/* ============================================================================  
Author     : PRATHEESH A 
Create date: 16 May 2013
Description: To generate/build json string
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                 Description:  
** ----------   -------------       ----------      ----------------------------

=============================================================================== */
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Data;
using Newtonsoft.Json;

namespace GenService.Common
{
    public class JsonSerde
    {
        public static string GetJson(Object obj)
        {
            MemoryStream stream1 = new MemoryStream();
            StreamReader sr = new StreamReader(stream1);
            string retString = string.Empty;
            try
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
                ser.WriteObject(stream1, obj);
                stream1.Position = 0;
                retString = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sr.Close();
                sr.Dispose();
                stream1.Close();
                stream1.Dispose();
            }
            return retString;
        }
        /// <summary>
        /// Building Json string 
        /// </summary>
        /// <param name="DS"></param>
        /// <param name="strServiceName">We will need </param>
        /// <returns></returns>
        public static string BuildJsonString(DataSet DS, string strServiceName)
        {
            try
            {
                //int SPResultType = Convert.ToInt32(USP_Name.GetConfig(strServiceName).SPReturnType);
                //string Result = String.Empty;
                //if (SPResultType == 1 || SPResultType == 2)
                //{
                //    /*
                //     [{"ColumnName1":"Value1","ColumnName2":"Value2","ColumnName3":"Value3","ColumnName4":"Value4"},
                //     {"ColumnName1":"Value1","ColumnName2":"Value2","ColumnName3":"Value3","ColumnName4":"Value4"}]
                //     * */
                //    Result = @"""[";

                //    foreach (DataRow DR in DS.Tables[0].Rows)
                //    {
                //        Result = Result + "{";

                //        foreach (DataColumn DC in DS.Tables[0].Columns)
                //        {
                //            Result = Result + @"""" + DC.ColumnName + @""":""" + DR[DC.ColumnName].ToString() + @""",";
                //        }
                //        Result = Result.Substring(0, Result.Length - 2) + @"""},";
                //    }
                //    Result = Result.Substring(0, Result.Length - 1) + "]";

                //}
                //else
                //{
                //    /*
                //        [{"tablename1":[{"ColumnName1":"Value1","ColumnName2":"Value2","ColumnName3":"Value3","ColumnName4":"Value4"},
                //        {"ColumnName1":"Value1","ColumnName2":"Value2","ColumnName3":"Value3","ColumnName4":"Value4"},
                //        {"ColumnName1":"Value1","ColumnName2":"Value2","ColumnName3":"Value3","ColumnName4":"Value4"},
                //        {"ColumnName1":"Value1","ColumnName2":"Value2","ColumnName3":"Value3","ColumnName4":"Value4"}]},

                //        {"tablename2":[{"ColumnName1":"Value1","ColumnName2":"Value2","ColumnName3":"Value3","ColumnName4":"Value4"},
                //        {"ColumnName1":"Value1","ColumnName2":"Value2","ColumnName3":"Value3"},
                //        {"ColumnName1":"Value1","ColumnName2":"Value2","ColumnName3":"Value3",
                //        {"ColumnName1":"Value1","ColumnName2":"Value2","ColumnName3":"Value3"]}]
                //     * */

                //    foreach (DataTable DT in DS.Tables)
                //    {
                //        Result = Result + @"{""" + DT.TableName + @""":""[";

                //        foreach (DataRow DR in DT.Rows)
                //        {
                //            Result = Result + "{";

                //            foreach (DataColumn DC in DT.Columns)
                //            {
                //                Result = Result + @"""" + DC.ColumnName + @""":""" + DR[DC.ColumnName].ToString() + @""",";
                //            }
                //            Result = Result.Substring(0, Result.Length - 1) + "},";
                //        }

                //        Result = Result.Substring(0, Result.Length - 1) + @"]""},";
                //    }
                //    Result = Result.Substring(0, Result.Length - 1);


                //}
                //single line code to build json
                return JsonConvert.SerializeObject(DS, Formatting.None);
            }
            catch (Exception)
            {
                return @"[{""ecode"":""" + 1 + @"""," + @"""msg"":" + Utility.GetAppSettingValue("ErrorMsg1") + "}]";
            }
        }

        //Dataset to Json String
        public static string BuildXmlStringFromDataset(DataSet ds, string strServiceName)
        {
            try
            {
                StringWriter sw = new StringWriter();
                ds.WriteXml(sw, XmlWriteMode.IgnoreSchema);
                System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
                xd.LoadXml(sw.ToString());
                return sw.ToString();
            }
            catch 
            {
                return @"[{""ecode"":""" + 1 + @"""," + @"""msg"":" + Utility.GetAppSettingValue("ErrorMsg1") + "}]";
            }
        }

    }
}