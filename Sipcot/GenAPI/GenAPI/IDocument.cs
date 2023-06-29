using System.ServiceModel;
using GenServiceDataContractLibrary;

namespace GenAPI
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDocument" in both code and config file together.
    [ServiceContract]
    public interface IDocument
    {
        [OperationContract]
        ServiceData GetDocumentInfo(ServiceData data);

        [OperationContract]
        ServiceData DownloadDocument(ServiceData data);
    }
}
