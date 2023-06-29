using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using GenServiceLibrary;

namespace GenService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FileUploadService" in code, svc and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FileUploadService : IFileUploadService
    {
        public string UploadFile(string FileName, string FileContent)
        {
            //save file
            try
            {
                Logger.logTrace(FileName + " Started upload", 100);

                if (!Directory.Exists(FileName.Substring(0, FileName.LastIndexOf("\\"))))
                {
                    Directory.CreateDirectory(FileName.Substring(0, FileName.LastIndexOf("\\")));
                }
                File.WriteAllBytes(FileName, Convert.FromBase64String(FileContent));

                Logger.logTrace(FileName + " upload success", 100);
                return "SUCCESS";

            }
            catch (IOException ex)
            {
                Logger.logException(ex, 100);
                Logger.logTrace(FileName + " upload fail", 100);
                return "ERROR";
            }
            catch (Exception ex)
            {
                Logger.logException(ex, 100);
                Logger.logTrace(FileName + " upload fail", 100);
                return "ERROR";
            }
        }
    }
}
