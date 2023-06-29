using System;
using System.IO;

public class FileUtility
{
    public static string ConvertFileToBase4String(string fileName)
    {
        //Converting the File to string base64
        byte[] bytes = System.IO.File.ReadAllBytes(fileName);
        return Convert.ToBase64String(bytes);
    }

    public static bool CreateFileFromBase4String(string fileSavePath, string FileContent)
    {
        bool Success = false;
        try
        {
            File.WriteAllBytes(fileSavePath, Convert.FromBase64String(FileContent));
            Success = true;
        }
        catch
        {
            throw;
        }
        return Success;
    }
}