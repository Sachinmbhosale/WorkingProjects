<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="UnAuthorised.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.UnAuthorised" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript" language="javascript">

        window.onload = function () {
            mmHome.className = "MenudivSelected";
            divSUBMENUSTIP.className = "MenuHide";

        }
    
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="width: 100%;">
        &nbsp;
        <div style="text-align: center; font-family: Verdana; font-weight: 700; color: #FF0000">
            <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/imagesCAM5Q0T7.jpg" />
            <br />
            <br />
            YOU ARE NOT AUTHORISED TO VIEW THIS PAGE!</div>
    </div>
</asp:Content>
