using System;
using System.IO;


public class Logger
{
    public static void Trace(string msgStr, string StrAgentId)
    {
        string strErrorFile = "";
        string foldername = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Logs\\" + StrAgentId + "\\";
        strErrorFile = DateTime.Now.ToString("yyyyMMdd");
        strErrorFile = foldername + strErrorFile + ".Log";
        if (File.Exists(foldername) == false)
        {
            System.IO.Directory.CreateDirectory(@foldername);
        }
        StreamWriter sw;
        if (File.Exists(strErrorFile))
        {
            sw = File.AppendText(strErrorFile);
        }
        else
        {
            sw = File.CreateText(strErrorFile);
        }
        sw.WriteLine(DateTime.Now.ToString() + ": " + msgStr);
        sw.Close();
    }
    public static void TraceErrorLog(string msgStr)
    {

        string strErrorFile = "";
        string foldername = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "ApplicationLog\\";
        strErrorFile = DateTime.Now.ToString("yyyyMMdd");
        strErrorFile = foldername + strErrorFile + ".Log";
        if (File.Exists(foldername) == false)
        {
            System.IO.Directory.CreateDirectory(@foldername);
        }
        StreamWriter sw;
        if (File.Exists(strErrorFile))
        {
            sw = File.AppendText(strErrorFile);
        }
        else
        {
            sw = File.CreateText(strErrorFile);
        }
        sw.WriteLine(DateTime.Now.ToString() + ": " + msgStr);
        sw.Close();
    }
    public static void TraceFileStatus(string msgStr)
    {
        string strErrorFile = "";
        string foldername = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "TraceFileStatus\\";
        strErrorFile = DateTime.Now.ToString("yyyyMMdd");
        strErrorFile = foldername + strErrorFile + ".Log";
        if (File.Exists(foldername) == false)
        {
            System.IO.Directory.CreateDirectory(@foldername);
        }
        StreamWriter sw;
        if (File.Exists(strErrorFile))
        {
            sw = File.AppendText(strErrorFile);
        }
        else
        {
            sw = File.CreateText(strErrorFile);
        }
        sw.WriteLine(DateTime.Now.ToString() + ": " + msgStr);
        sw.Close();
    }
}
