<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="Configuration.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Configuration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7-vsdoc.js") %>"></script>
      <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.min.js") %>"></script>
        <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.js") %>"></script>
    <script type="text/javascript" language="javascript" src="<%=Page.ResolveClientUrl("~/Assets/Scripts/AjaxPostScripts.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Assets/Scripts/master.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="400" height="157" border="0" align="center" class="generalBox">
        <tr>
            <td height="8" colspan="3">
                <center>
                    <div id="divMsg" style="color: White">
                        &nbsp;</div>
                </center>
            </td>
        </tr>
        <tr>
            <td height="23" colspan="3">
                <h2>
                    Application Initial Configuration</h2>
            </td>
        </tr>
        <tr>
            
        </tr>
        <tr>
            <td height="38" colspan="3">
                <asp:Button ID="btnCreateGlobalOrg" class="extraLongButton" runat="server" Text="Reset Sercurity Key & Create Global Org"
                    OnClick="btnCreateGlobalOrg_Click" />
                <asp:HiddenField ID="hdnOrgName" runat="server" Value="" />
                <asp:HiddenField ID="hdnOrgId" runat="server" Value="" />
                <asp:HiddenField ID="hdnGuid" runat="server" Value="" />
            </td>
        </tr>
        <tr>
            <td width="119" height="44" align="left" >
                <input type="button" id="btnCancel" class="cancel" value="Cancel" />
            </td>
            <td width="87" colspan="2" align="right" >
                <asp:Button ID="btnGoToLogin" class="longButton" runat="server" Text="Go to Login Page >>"
                    OnClick="btnCreateGlobalOrg_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
