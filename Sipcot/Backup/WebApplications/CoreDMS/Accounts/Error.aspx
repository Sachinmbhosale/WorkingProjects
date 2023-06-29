<%@ Page Title="" Language="C#" MasterPageFile="~/General.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7-vsdoc.js") %>"></script>
      <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.min.js") %>"></script>
        <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.js") %>"></script>
    <script type="text/javascript" language="javascript" src="<%=Page.ResolveClientUrl("~/Assets/Scripts/AjaxPostScripts.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Assets/Scripts/master.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="295" height="157" border="0" align="center" class="loginBox">
        
        <tr>
            <td height="23" colspan="3">
                <h2>
                    Error</h2>
            </td>
        </tr>
        <tr>
            <td height="8" colspan="3">
                <center>
                    <div id="divMsg" style="color: Red">
                         An error occurred, please try again. If problem exists please contact Administrator.</div>
                </center>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <a href="Login.aspx" class="forgotPassword">Login Again</a>
            </td>
        </tr>
    </table>
</asp:Content>
