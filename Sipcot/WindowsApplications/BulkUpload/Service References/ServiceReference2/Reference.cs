﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WindowsForm_SERVICE.ServiceReference2 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference2.IFileUploadService")]
    public interface IFileUploadService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileUploadService/UploadFile", ReplyAction="http://tempuri.org/IFileUploadService/UploadFileResponse")]
        string UploadFile(string FileName, string FileContent);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFileUploadServiceChannel : WindowsForm_SERVICE.ServiceReference2.IFileUploadService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FileUploadServiceClient : System.ServiceModel.ClientBase<WindowsForm_SERVICE.ServiceReference2.IFileUploadService>, WindowsForm_SERVICE.ServiceReference2.IFileUploadService {
        
        public FileUploadServiceClient() {
        }
        
        public FileUploadServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FileUploadServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileUploadServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileUploadServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string UploadFile(string FileName, string FileContent) {
            return base.Channel.UploadFile(FileName, FileContent);
        }
    }
}