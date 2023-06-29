using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


public static class IOUtility
{

    public static string beforedot(string s)
    {
        int l = s.LastIndexOf(".");
        return s.Substring(0, l);
    }

    public static string afterdot(string s)
    {
        string[] array = s.Split('.');
        string Ext = "." + array[array.Length - 1];
        return Ext;
    }

    public static bool ValidatePath(string path)
    {
        bool IsValid = false;

        if (Directory.Exists(path))
        {
            IsValid = true;
        }
        else
        {
            Directory.CreateDirectory(path);
            IsValid = true;
        }

        return IsValid;
    }
}
