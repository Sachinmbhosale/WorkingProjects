using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;


namespace GenServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class TransferService : ITransferService
    {

        public RemoteFileInfo DownloadFile(DownloadRequest request)
        {
            RemoteFileInfo result = new RemoteFileInfo();
            try
            {
                string filePath = System.IO.Path.Combine(@"c:\Uploadfiles", request.FileName);
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);

                // check if exists
                if (!fileInfo.Exists)
                    throw new System.IO.FileNotFoundException("File not found",
                                                              request.FileName);

                // open stream
                System.IO.FileStream stream = new System.IO.FileStream(filePath,
                          System.IO.FileMode.Open, System.IO.FileAccess.Read);

                // return result 
                result.FileName = request.FileName;
                result.Length = fileInfo.Length;
                result.FileByteStream = stream;
            }
            catch (Exception)
            {

            }
            return result;
        }

        public void UploadFile(RemoteFileInfo request)
        {
            try
            {
                FileStream targetStream = null;
                Stream sourceStream = request.FileByteStream;

                string uploadFolder = request.ChannelInfo;
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
                string filePath = Path.Combine(uploadFolder, request.FileName);
                Logger.logTrace("Started UploadFile" + request.FileName, 0);
                using (targetStream = new FileStream(filePath, FileMode.Create,
                                      FileAccess.Write, FileShare.None))
                {
                    //read from the input stream in 65000 byte chunks

                    const int bufferLen = 65000;
                    byte[] buffer = new byte[bufferLen];
                    int count = 0;
                    while ((count = sourceStream.Read(buffer, 0, bufferLen)) > 0)
                    {
                        // save to output stream
                        targetStream.Write(buffer, 0, count);
                    }
                    targetStream.Close();
                    sourceStream.Close();
                }
                Logger.logTrace("Finished UploadFile" + request.FileName, 0);
            }
            catch (Exception ex)
            {
                Logger.logException(ex, 0);
            }
        }

        public string Aunthenticate(Authentication request)
        {
            string result = string.Empty;
            try
            {
                Logger.logTrace("Started Aunthenticate", 0);

                string XmlPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"\" + ConfigurationManager.AppSettings["XmlFilePath"].ToString();

                DataSet ds = new DataSet();
                ds.ReadXml(XmlPath);
                DataView dv = ds.Tables[1].DefaultView;
                dv.RowFilter = "Username ='" + request.UserName + "' AND Password ='" + request.Passwword + "' AND Active='True'";
                if (dv.Count > 0)
                    result = "ErrorState~0|Message:Success|Channel~" + dv[0].Row["Channel"].ToString() + "|FileUploadPath~" + dv[0].Row["FileUploadPath"].ToString();
                else
                    result = "ErrorState~-1|Message~Authentication failed. Invalid username or password." +
                        Environment.NewLine + Environment.NewLine + "Tip:" +
                        Environment.NewLine + "1. Channel not created." +
                        Environment.NewLine + "2. Channel could be disabled.";

                Logger.logTrace("Finished Aunthentication", 0);
            }
            catch (Exception ex)
            {
                Logger.logException(ex, 0);
                result = "ErrorState~1|Message~Service failed to authenticate. Please contact administrator.";
            }

            return result;
        }


    }
}
