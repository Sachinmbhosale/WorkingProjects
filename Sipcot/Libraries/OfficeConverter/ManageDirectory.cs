using System;
using System.IO;

namespace OfficeConverter
{
    public class ManageDirectory
    {
        String[] FileInput;
        public void CopyDirectory(string Src, string Dst)
        {
            String[] Files;
            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
                Dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
            Files = Directory.GetFileSystemEntries(Src, "*.pdf");
            foreach (string Element in Files)
            {
                try
                {
                    File.Copy(Element, Dst + Path.GetFileName(Element), true);
                }
                catch
                {
                    continue;

                }
            }
        }

        ///// <summary>
        ///// To copy the jpeg files alone from the directory
        ///// </summary>
        ///// <param name="Src"></param>
        ///// <param name="Dst"></param>
        public void CopyDirectoryFiles(string Src, string Dst, string Extension)
        {


            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
                Dst += Path.DirectorySeparatorChar;
            CreateDirectory(Dst);
            if (Extension == ".pdf")
            {
                FileInput = Directory.GetFileSystemEntries(Src, "*.pdf");
            }
            if (Extension == ".jpeg")
            {
                FileInput = Directory.GetFileSystemEntries(Src, "*.jpeg");
            }
            else if (Extension == ".jpg")
            {
                FileInput = Directory.GetFileSystemEntries(Src, "*.jpg");
            }
            else if (Extension == ".png")
            {
                FileInput = Directory.GetFileSystemEntries(Src, "*.png");
            }
            foreach (string Element in FileInput)
            {
                try
                {
                    File.Copy(Element, Dst + Path.GetFileName(Element), true);
                }
                catch
                {
                    continue;

                }
            }
        }


        // DMS5-3362BS 

        /// <summary>
        /// To create directory
        /// </summary>
        /// <param name="DirectoryPath"></param>
        public void CreateDirectory(string DirectoryPath)
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
        }



        /// <summary>
        /// To delete directory
        /// </summary>
        /// <param name="DirectoryPath"></param>
        public void DeleteDirectory(string DirectoryPath)
        {
            try
            {
                if (Directory.Exists(DirectoryPath))
                {
                    Directory.Delete(DirectoryPath, true);
                }

            }
            catch (Exception ex)
            {

                Logger.TraceErrorLog("Error in deleting directory" + ex.Message);
            }

        }

        public void FileCopy(string sourcePath, string destinationPath)
        {
            try
            {
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                    File.Copy(sourcePath, destinationPath);
                }
                else
                {
                    if (File.Exists(sourcePath))
                    {
                        File.Copy(sourcePath, destinationPath);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog("Error in File copy" + ex.Message);
            }
        }


    }
}
