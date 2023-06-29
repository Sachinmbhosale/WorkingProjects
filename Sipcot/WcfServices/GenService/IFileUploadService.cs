using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

namespace GenService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFileUploadService" in both code and config file together.
    [ServiceContract]
    public interface IFileUploadService
    {
        [WebInvoke(Method = "POST", UriTemplate = "UploadFile?fileName={fileName}")]
        [OperationContract]
        string UploadFile(string FileName, string FileContent);
    }


    public class FileInfo
    {
        [MessageHeader(MustUnderstand = true)]
        public string fileName;

        [MessageHeader(MustUnderstand = true)]
        public Stream fileContents;
    }
}
