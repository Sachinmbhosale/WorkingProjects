using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using System.Configuration;

public class Logger
{
    public static void logException(Exception objException, string userIdOrName)
    {
        string logLocation = ConfigurationManager.AppSettings["logExceptionPath"];

        logTrace("Error : " + objException.Message.ToString(), userIdOrName);

        StringBuilder preamble = new StringBuilder();
        StreamWriter sw = null;

        try
        {

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["logExceptionIsEnabled"]))
            {
                // If directory not exist create it before writing log
                string strPathName = logLocation + "\\" + userIdOrName + "\\" + DateTime.Today.Date.ToString("dd_MM_yyyy") + ".log";
                if (!Directory.Exists(logLocation + "\\" + userIdOrName))
                {
                    Directory.CreateDirectory(logLocation + "\\" + userIdOrName);
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
                sw.WriteLine(Environment.NewLine);
                sw.WriteLine("User ID		      : " + userIdOrName.ToString());
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
        catch
        {
            //throw new Exception(ex.Message + ex.StackTrace);
        }
        finally { DisposeObjects.DisposeObjList(sw); }
    }

    public static void logTrace(string strActivity, string userIdOrName)
    {

        string logLocation = ConfigurationManager.AppSettings["logPath"];

        StreamWriter swLogTrace = null;
        try
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["logEnabled"]))
            {
                // If directory not exist create it before writing log
                string strPathName = logLocation + "\\" + userIdOrName + "\\" + DateTime.Today.Date.ToString("dd_MM_yyyy") + ".log";
                if (!Directory.Exists(logLocation + "\\" + userIdOrName))
                {
                    Directory.CreateDirectory(logLocation + "\\" + userIdOrName);
                }

                string Activity = " " + strActivity + " ";

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

                swLogTrace.WriteLine(
                    Environment.NewLine +
                    //userIdOrName + " | " +
                DateTime.Now.ToString() + " | " +
                Activity + " | " +
                System.Environment.MachineName + " | " +
                preamble.ToString());

            }
        }
        catch
        {
            throw; // new Exception(ex.Message + ex.StackTrace);
        }
        finally { DisposeObjects.DisposeObjList(swLogTrace); }
    }

    private static string getPreambleMessage()
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
