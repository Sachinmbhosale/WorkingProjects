using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using System.Configuration;

namespace GenServiceLibrary
{
    public static class Logger
    {

        /// <summary>
        /// Returns an application setting based on the passed in string
        /// used primarily to cut down on typing
        /// </summary>
        /// <param name="Key">The name of the key</param>
        /// <returns>The value of the app setting in the web.Config
        //     or String.Empty if no setting found</returns>
        public static string AppSetting(this string Key)
        {
            string ret = string.Empty;
            if (ConfigurationManager.AppSettings[Key] != null)
                ret = ConfigurationManager.AppSettings[Key];
            return ret;
        }

        public static void logException(Exception objException, int userID)
        {
            string logLocation = "logExceptionPath".AppSetting();

            logTrace("Error : " + objException.Message.ToString(), userID);

            StringBuilder preamble = new StringBuilder();
            StreamWriter sw = null;

            try
            {

                if (Convert.ToBoolean("logExceptionIsEnabled".AppSetting()))
                {
                    // If directory not exist create it before writing log
                    string strPathName = logLocation + "\\" + userID + "\\" + DateTime.Today.Date.ToString("dd_MM_yyyy") + ".log";
                    if (!Directory.Exists(logLocation + "\\" + userID))
                    {
                        Directory.CreateDirectory(logLocation + "\\" + userID);
                    }

                    // Instantiating object to get exception info
                    StackTrace stackTrace = new StackTrace();
                    StackFrame stackFrame;
                    MethodBase stackFrameMethod;

                    int frameCount = 0;
                    string typeName;
                    do
                    {
                        frameCount++;
                        stackFrame = stackTrace.GetFrame(frameCount);
                        stackFrameMethod = stackFrame.GetMethod();
                        typeName = stackFrameMethod.ReflectedType.FullName;
                    } while (typeName.StartsWith("System") || typeName.EndsWith("CustomTraceListener"));

                    //log DateTime, Namespace, Class and Method Name
                    preamble.Append(typeName);
                    preamble.Append(".");
                    preamble.Append(stackFrameMethod.Name);
                    preamble.Append("( ");

                    // log parameter types and names
                    ParameterInfo[] parameters = stackFrameMethod.GetParameters();
                    int parameterIndex = 0;
                    while (parameterIndex < parameters.Length)
                    {
                        preamble.Append(parameters[parameterIndex].ParameterType.Name);
                        preamble.Append(" ");
                        preamble.Append(parameters[parameterIndex].Name);
                        parameterIndex++;
                        if (parameterIndex != parameters.Length) preamble.Append(", ");
                    }

                    preamble.Append(" ) ");
                    string strClassFunction = preamble.ToString();


                    sw = new StreamWriter(strPathName, true);
                    sw.WriteLine("User ID		      : " + userID.ToString());
                    sw.WriteLine("Date		      : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                    sw.WriteLine("Source		      : " + objException.Source.ToString().Trim());
                    sw.WriteLine("System Method         : " + objException.TargetSite.Name.ToString());
                    sw.WriteLine("User Method           : " + strClassFunction);
                    sw.WriteLine("Computer	      : " + System.Environment.MachineName.ToString());
                    //sw.WriteLine("OS                : " + Environment.OSVersion.ToString());
                    //sw.WriteLine("Error		      : " + objException.Message.ToString().Trim());
                    //sw.WriteLine("Line              : " + objException.StackTrace.Substring(objException.StackTrace.Length - 7, 7));
                    //sw.WriteLine("Column            : " + System.AppDomain.CurrentDomain.BaseDirectory);
                    sw.WriteLine("Stack                 : " + objException.ToString());
                    sw.WriteLine("=================================================================================================");

                }
            }
            catch (Exception)
            {
              //  throw new Exception(ex.Message + ex.StackTrace);
            }
            finally
            {
                Common.DisposeObjects disposeobj = new Common.DisposeObjects();
                disposeobj.DisposeObjList(sw, preamble);
            }
        }

        public static void logTrace(string strActivity, int userID)
        {

            string logLocation = "logTracePath".AppSetting();

            StreamWriter swLogTrace = null;
            StringBuilder preamble = new StringBuilder();
            try
            {
                if (Convert.ToBoolean("logTraceIsEnabled".AppSetting()))
                {
                    // If directory not exist create it before writing log
                    string strPathName = logLocation + "\\" + userID + "\\" + DateTime.Today.Date.ToString("dd_MM_yyyy") + ".log";
                    if (!Directory.Exists(logLocation + "\\" + userID))
                    {
                        Directory.CreateDirectory(logLocation + "\\" + userID);
                    }

                    string Activity = " " + strActivity + " ";

                    StackTrace stackTrace = new StackTrace();
                    StackFrame stackFrame;
                    MethodBase stackFrameMethod;

                    int frameCount = 0;
                    string typeName;
                    do
                    {
                        frameCount++;
                        stackFrame = stackTrace.GetFrame(frameCount);
                        stackFrameMethod = stackFrame.GetMethod();
                        typeName = stackFrameMethod.ReflectedType.FullName;
                    } while (typeName.StartsWith("System") || typeName.EndsWith("CustomTraceListener"));

                    //log DateTime, Namespace, Class and Method Name                    
                    preamble.Append(typeName);
                    preamble.Append(".");
                    preamble.Append(stackFrameMethod.Name);
                    preamble.Append("( ");

                    // log parameter types and names
                    ParameterInfo[] parameters = stackFrameMethod.GetParameters();
                    int parameterIndex = 0;
                    while (parameterIndex < parameters.Length)
                    {
                        preamble.Append(parameters[parameterIndex].ParameterType.Name);
                        preamble.Append(" ");
                        preamble.Append(parameters[parameterIndex].Name);
                        parameterIndex++;
                        if (parameterIndex != parameters.Length) preamble.Append(", ");
                    }

                    preamble.Append(" )");

                    string strException = string.Empty;

                    swLogTrace = new StreamWriter(strPathName, true);


                    swLogTrace.WriteLine(userID + " | " +
                    DateTime.Now.ToString() + " | " +
                    Activity + " | " +
                    System.Environment.MachineName + " | " +
                    preamble.ToString());

                }
            }
            catch (Exception)
            {
              ///  throw new Exception(ex.Message + ex.StackTrace);
            }
            finally
            {
                Common.DisposeObjects disposeobj = new Common.DisposeObjects();
                disposeobj.DisposeObjList(swLogTrace, preamble);
            }
        }

        public static string getPreambleMessage()
        {

            StringBuilder preamble = new StringBuilder();

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame;
            MethodBase stackFrameMethod;

            int frameCount = 0;
            string typeName;
            do
            {
                frameCount++;
                stackFrame = stackTrace.GetFrame(frameCount);
                stackFrameMethod = stackFrame.GetMethod();
                typeName = stackFrameMethod.ReflectedType.FullName;
            } while (typeName.StartsWith("System") || typeName.EndsWith("CustomTraceListener"));

            //log DateTime, Namespace, Class and Method Name
            preamble.Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            preamble.Append(": ");
            preamble.Append(typeName);
            preamble.Append(".");
            preamble.Append(stackFrameMethod.Name);
            preamble.Append("( ");

            // log parameter types and names
            ParameterInfo[] parameters = stackFrameMethod.GetParameters();
            int parameterIndex = 0;
            while (parameterIndex < parameters.Length)
            {
                preamble.Append(parameters[parameterIndex].ParameterType.Name);
                preamble.Append(" ");
                preamble.Append(parameters[parameterIndex].Name);
                parameterIndex++;
                if (parameterIndex != parameters.Length) preamble.Append(", ");
            }

            preamble.Append(" ): ");

            return preamble.ToString();

        }



    }
}
