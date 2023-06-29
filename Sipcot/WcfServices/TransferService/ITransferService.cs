using System;
using System.ServiceModel;
namespace GenServiceLibrary
{
    // NOTE: If you change the interface name "ITransferService" here, you must also update the reference to "ITransferService" in Web.config.
    [ServiceContract]
    public interface ITransferService
    {
        [OperationContract]
        RemoteFileInfo DownloadFile(DownloadRequest request);

        [OperationContract]
        void UploadFile(RemoteFileInfo request);

        [OperationContract]
        string Aunthenticate(Authentication request);


    }
    [MessageContract]
    public class DownloadRequest
    {
        [MessageBodyMember]
        public string FileName;
    }
    public class Authentication
    {
        [MessageHeader(MustUnderstand = true)]
        public string UserName;
        [MessageHeader(MustUnderstand = true)]
        public string Passwword;
    }

    [MessageContract]
    public class RemoteFileInfo : IDisposable
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName;

        [MessageHeader(MustUnderstand = true)]
        public long Length;

        [MessageHeader(MustUnderstand = true)]
        public string ChannelInfo;

        [MessageBodyMember(Order = 1)]
        public System.IO.Stream FileByteStream;

        public void Dispose()
        {
            if (FileByteStream != null)
            {
                FileByteStream.Close();
                FileByteStream = null;
            }
        }



    }

}