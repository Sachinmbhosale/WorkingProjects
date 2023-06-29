using System.ServiceModel;
using System.ServiceModel.Web;

namespace GenService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService
    {

        [WebInvoke(Method = "POST", UriTemplate = "GEN", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        string RunStoredProcedureReturnsDataset(StoredProcedureReturnsDataset ProcedureReturnsDataset);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    

    public class StoredProcedureReturnsDataset 
    {
        [MessageHeader(MustUnderstand = true)]
        public string ProcedureName_Encrypted;

        [MessageHeader(MustUnderstand = true)]
        public string ParametersList_Encrypted;

        [MessageHeader(MustUnderstand = true)]
        public string ConnectionString_Encrypted;


    }

}
