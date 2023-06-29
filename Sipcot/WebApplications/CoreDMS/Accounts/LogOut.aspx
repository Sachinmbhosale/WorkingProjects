<%@ Page Title="" Language="C#" MasterPageFile="~/General.Master" AutoEventWireup="true"
    CodeBehind="LogOut.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.LogOut" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
            <script type="text/javascript">

            function preventBack() { window.history.forward(); }
            setTimeout("preventBack()", 0);
            window.onunload = function () { null };
            </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
    <div id="divMsg" runat="server" class="MessageMediumStyle" style="text-align: center;">
        &nbsp;</div>
</asp:Content>
