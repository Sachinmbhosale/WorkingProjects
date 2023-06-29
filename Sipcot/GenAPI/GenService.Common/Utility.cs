/* ============================================================================  
Author     : PRATHEESH A 
Create date: 16 May 2013
Description: Common Utility Functions
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                 Description:  
** ----------   -------------       ----------      ----------------------------

=============================================================================== */
using System;
using System.Configuration;


namespace GenService.Common
{
    public class Utility
    {
        /// <summary>
        /// To read exception details from exception object
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string GetExceptionDetails(Exception ex)
        {
            // You can add exception line, page etc by concatinating the string
            return ex.Message.ToString();
        }

        /// <summary>
        /// To read the web.config value
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string GetAppSettingValue(string strKey)
        {
            if (ConfigurationManager.AppSettings[strKey] != null)
            {
                return ConfigurationManager.AppSettings[strKey];
            }
            else
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// To determine whether one value is presnt in string[] array or not
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool FindValueInArray(string[] strArray, string value)
        {
            bool Result = false;
            foreach (string s in strArray)
            {
                if (s.ToLower().Trim() == value.ToLower().Trim())
                {
                    Result = true;
                }
            }
            return Result;
        }

        /// <summary>
        /// To change the date format from dd/mm/yyyy to yyyy/mm/dd
        /// </summary>
        /// <param name="inDate"></param>
        /// <returns></returns>
        public static DateTime ConvertToDBDate(string inDate)
        {
            try
            {
                string[] indateParts = inDate.Split('/');
                return new DateTime(Convert.ToInt32(indateParts[2]), Convert.ToInt32(indateParts[1]), Convert.ToInt32(indateParts[0]));
            }
            catch 
            {
                return System.DateTime.Today;
            }
        }
        /// <summary>
        /// To change the date format from yyyy/mm/dd to dd/mm/yyyy
        /// </summary>
        /// <param name="outDate"></param>
        /// <returns></returns>
        public static string ConvertToNormalDate(DateTime outDate)
        {
            return outDate.Day.ToString() + "/" + outDate.Month.ToString() + "/" + outDate.Year;
        }

    }
}
