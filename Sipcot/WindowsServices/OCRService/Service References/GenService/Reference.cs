﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OCRService.GenService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="StoredProcedureReturnsDataset", Namespace="http://schemas.datacontract.org/2004/07/GenService")]
    [System.SerializableAttribute()]
    public partial class StoredProcedureReturnsDataset : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ConnectionString_EncryptedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ParametersList_EncryptedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ProcedureName_EncryptedField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ConnectionString_Encrypted {
            get {
                return this.ConnectionString_EncryptedField;
            }
            set {
                if ((object.ReferenceEquals(this.ConnectionString_EncryptedField, value) != true)) {
                    this.ConnectionString_EncryptedField = value;
                    this.RaisePropertyChanged("ConnectionString_Encrypted");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ParametersList_Encrypted {
            get {
                return this.ParametersList_EncryptedField;
            }
            set {
                if ((object.ReferenceEquals(this.ParametersList_EncryptedField, value) != true)) {
                    this.ParametersList_EncryptedField = value;
                    this.RaisePropertyChanged("ParametersList_Encrypted");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ProcedureName_Encrypted {
            get {
                return this.ProcedureName_EncryptedField;
            }
            set {
                if ((object.ReferenceEquals(this.ProcedureName_EncryptedField, value) != true)) {
                    this.ProcedureName_EncryptedField = value;
                    this.RaisePropertyChanged("ProcedureName_Encrypted");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="GenService.IService")]
    public interface IService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/RunStoredProcedureReturnsDataset", ReplyAction="http://tempuri.org/IService/RunStoredProcedureReturnsDatasetResponse")]
        string RunStoredProcedureReturnsDataset(OCRService.GenService.StoredProcedureReturnsDataset ProcedureReturnsDataset);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChannel : OCRService.GenService.IService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceClient : System.ServiceModel.ClientBase<OCRService.GenService.IService>, OCRService.GenService.IService {
        
        public ServiceClient() {
        }
        
        public ServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string RunStoredProcedureReturnsDataset(OCRService.GenService.StoredProcedureReturnsDataset ProcedureReturnsDataset) {
            return base.Channel.RunStoredProcedureReturnsDataset(ProcedureReturnsDataset);
        }
    }
}
