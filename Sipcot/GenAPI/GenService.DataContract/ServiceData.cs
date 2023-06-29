using System.Runtime.Serialization;
using System.Data;

namespace GenServiceDataContractLibrary
{
    [DataContract]
    public class ServiceData
    {
        public ServiceData()
        {
            Resultsets = new DataSet();

            API_Key = string.Empty;
            Method = string.Empty;
            Input = string.Empty;

            Message = string.Empty;
            ErrorState = 0;
            ErrorSeverity = 0;

            OutputParamValues = string.Empty;
            FileContent = string.Empty;
            FilePath = string.Empty;
        }

        [DataMember]
        public string API_Key { get; set; }

        [DataMember]
        public string Method { get; set; }

        [DataMember]
        public string Input { get; set; }

        [DataMember]
        public string CustomInput { get; set; }

        [DataMember]
        public DataSet Resultsets { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public int ErrorState { get; set; }
        [DataMember]
        public int ErrorSeverity { get; set; }
        [DataMember]
        public string OutputParamValues { get; set; }
        [DataMember]
        public string FileContent { get; set; }
        [DataMember]
        public string FilePath { get; set; }
    }
}
