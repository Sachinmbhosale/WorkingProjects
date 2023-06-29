<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true" CodeBehind="Rept_Patientcount.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.Rept_Patientcount1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="-1" />
    <meta http-equiv="Version" content="11.0.6" />   
    
  
     
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div id="wrapper">  
        <div id="statusPopup" style="display: none; z-index: 1000; padding-left: 630px; background-color:white;  ">
            <div>
                <img src="<%= Page.ResolveClientUrl("~/Images/loading.gif") %>" alt="Loading......" />
            </div>
        </div>
        <div style="margin:0 auto; text-align:center; background-color:white;">
            <iframe width="800" height="600" src="https://app.powerbi.com/view?r=eyJrIjoiMjljZjM5NWUtMWVjYy00NWQ5LTlhMDItMTRlY2MyYTRmNTU5IiwidCI6ImM3MTRlZjNjLTI2M2QtNDg0OC05NmMzLTM4MzE5ZWU5YTg1NSIsImMiOjEwfQ%3D%3D" frameborder="0" style="background-color:white;" ></iframe>
        </div>
        
    </div>
</asp:Content>

