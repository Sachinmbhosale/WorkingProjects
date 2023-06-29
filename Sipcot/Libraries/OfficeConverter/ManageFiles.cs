using System;
using System.IO;

namespace OfficeConverter
{
    public class ManageFiles
    {
        /// <summary>
        /// To delete files from directory
        /// </summary>
        /// <param name="directoryPath">Directory path</param>
        /// <param name="fileExtension">File extension. Ex: *.pdf. Default is *.* (all files)</param>
        /// <returns>Boolean value true or false i.e., success or failed respectively</returns>
        public static bool DeleteFiles(string directoryPath, string fileExtension = "*.*")
        {
            bool success = true;

            if (Directory.Exists(directoryPath))
                Array.ForEach(Directory.GetFiles(directoryPath, fileExtension), File.Delete);
            else
                success = false;

            return success;
        }

        /// <summary>
        /// Copy files from source loaction to destination location
        /// </summary>
        /// <param name="sourceLocation"></param>
        /// <param name="destinationLocation"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static bool CopyFiles(string sourceLocation, string destinationLocation, string fileExtension = "*.*")
        {
            bool success = true;

            if (!Directory.Exists(destinationLocation))
                Directory.CreateDirectory(destinationLocation);

            //Copy all the files
            foreach (string newPath in Directory.GetFiles(sourceLocation, fileExtension, SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourceLocation, destinationLocation), true);

            return success;
        }

        public static bool RenameFile(string filePath, string fileName)
        {
            bool success = true;

            //if (File.Exists(filePath))
            //{
            //    System.IO.File.Delete(filePath);
            //}

            //System.IO.File.Move("oldfilename", filePath);

            return success;
        }
    }
}
