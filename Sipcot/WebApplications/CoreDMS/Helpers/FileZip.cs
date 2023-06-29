using System;
using Ionic.Zip;
using System.IO;


public static class FileZip
{
    /// <summary>
    /// Zip the file in same location where the original file is exist.
    /// </summary>
    /// <param name="FilePath"></param>
    /// <param name="DeleteSource">Delete source file. Default value is false.</param>
    /// <returns></returns>
    public static bool Zip(string FilePath, bool DeleteSource = false)
    {
        bool Status = false;
        try
        {
            using (ZipFile zip = new ZipFile())
            {
                if (File.Exists(FilePath))
                {
                    zip.AddFile(FilePath, "");
                    zip.Save(IOUtility.beforedot(FilePath) + ".zip");
                    if (DeleteSource)
                        File.Delete(FilePath);
                }
            }
            Status = true;
        }
        catch
        {
            throw;
        }
        return Status;
    }

    /// <summary>
    /// Unzip the file in same location where the zipped file is exist.
    /// </summary>
    /// <param name="FilePath">Zipped file path</param>
    public static bool Unzip(string FilePath, bool DeleteSource = false)
    {
        bool Status = false;
        try
        {
            string ExtractFolderPath = FilePath.Replace(Path.GetFileName(FilePath), string.Empty);

            using (ZipFile zip = ZipFile.Read(FilePath))
            {
                zip.ExtractAll(ExtractFolderPath, ExtractExistingFileAction.DoNotOverwrite);
            }
            if (DeleteSource)
                File.Delete(FilePath);
            Status = true;
        }
        catch
        {
            throw;
        }
        return Status;
    }

    public static bool Unzip1(string FilePath, string Destpath, bool DeleteSource = false)
    {
        bool Status = false;
        try
        {
            string pth=FilePath.Split('#')[0];
            string ext = FilePath.Split('#')[1];
            string ExtractFolderPath = pth.Replace(Path.GetFileName(pth), string.Empty);
            string fname = Path.GetFileName(pth.Replace(".zip", ext));


            using (ZipFile zip = ZipFile.Read(pth))
            {
                zip.ExtractAll(ExtractFolderPath, ExtractExistingFileAction.DoNotOverwrite);
            }
            File.Copy(pth.Replace(".zip", ext), Destpath + "/" + fname);

        }
        catch (Exception ex)
        {
            string sMsg = ex.Message;
            throw;
        }
        return Status;
    }
    //public static bool Unzip1(string FilePath, string Destpath, bool DeleteSource = false)
    //{
    //    bool Status = false;
    //    try
    //    {
    //        string ExtractFolderPath = FilePath.Replace(Path.GetFileName(FilePath), string.Empty);
    //        string fname = Path.GetFileName(FilePath.Replace(".zip", ".pdf"));


    //        using (ZipFile zip = ZipFile.Read(FilePath))
    //        {
    //            zip.ExtractAll(ExtractFolderPath, ExtractExistingFileAction.DoNotOverwrite);
    //        }
    //        File.Copy(FilePath.Replace(".zip", ".pdf"), Destpath + "/" + fname);

    //    }
    //    catch (Exception ex)
    //    {
    //        string sMsg = ex.Message;
    //        throw;
    //    }
    //    return Status;
    //}
}
