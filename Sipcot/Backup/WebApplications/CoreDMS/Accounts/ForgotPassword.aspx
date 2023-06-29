<%@ Page Title="" Language="C#" MasterPageFile="~/General.Master" AutoEventWireup="true"
    CodeBehind="ForgotPassword.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.ForgotPassword" %>
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
   <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Assets/Scripts/master.js") %>"></script>
    <style type="text/css">
        .style5
        {
            width: 100%;
        }
        .style6
        {
            width: 150px;
        }
        .style7
        {
            width: 133px;
            height: 36px;
        }
        .style8
        {
            height: 36px;
        }
        .style9
        {
            width: 14px;
            height: 36px;
        }
        .style10
        {
            width: 14px;
        }
        .style14
        {
            width: 133px;
            height: 19px;
        }
        .style15
        {
            width: 14px;
            height: 19px;
        }
        .style16
        {
            height: 19px;
        }
        .style18
        {
            height: 36px;
            width: 197px;
        }
        .style19
        {
            width: 197px;
        }
        .style20
        {
            height: 19px;
            width: 197px;
        }
        .style21
        {
            width: 133px;
            height: 20px;
        }
        .style22
        {
            width: 14px;
            height: 20px;
        }
        .style23
        {
            height: 20px;
            width: 197px;
        }
        .style24
        {
            height: 20px;
        }
    </style>
    <script language="javascript" type="text/javascript">

        ///check for keyDownFunction()
        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            btnSubmitControlID = "<%= btnSubmit.ClientID %>";
            hdnOrglinkControlID = "<%= hdnOrglink.ClientID %>";
            btnRequestPasswordControlID = "<%= btnRequestPassword.ClientID %>";
        });

        function ForgotPasswordRequest() {
            var msgControl = "#<%= divMsg.ClientID %>";
            var action = "ForgotPassword";
            var loginOrgName = $("#<%= hdnLoginOrgName.ClientID %>").val();
            var username = $("#<%= txtUsername.ClientID %>").val();
            var params = loginOrgName + '|' + username;
            if (ValidateInputData(msgControl, action, username)) {

                document.getElementById("<%= btnRequestPassword.ClientID %>").disabled = true;

                return CallPostScalar(msgControl, action, params);
            }
            else {

                return false;
            }
        }
        //********************************************************
        //ValidateInputData Function returns true or false with message to user on contorl specified
        //********************************************************

        function ValidateInputData(msgControl, action, username) {
            $(msgControl).html("");
            if (action == "ForgotPassword") {
                if (username == "") {
                    $(msgControl).html("Username cannot be blank.");
                    document.getElementById("<%= txtUsername.ClientID %>").focus();
                    return false;
                }
                return true;
            }
        }
        //********************************************************
        //ClearData Function clears the form
        //********************************************************

        function ClearData() {
            document.getElementById("<%= txtUsername.ClientID %>").value = "";
            document.getElementById("<%= txtUsername.ClientID %>").focus();
            document.getElementById("<%= btnRequestPassword.ClientID %>").disabled = false;
        }
        //********************************************************
        //ClearData Function navigate to Homepage
        //********************************************************

            

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div align="center" class="CenterBaseDiv">
        <div class="LoginPanel">
            <table class="style5">
                <tr>
                    <td align="center" colspan="4">
                        <div id="divMsg" runat="server" class="MessageMediumStyle">
                            &nbsp;</div>
                    </td>
                </tr>
                <tr>
                    <td align="right" class="style7">
                        <asp:Label ID="Label5" runat="server" CssClass="LabelMediumStyle" Text="User ID"
                            Style="color: black; font-weight: bold;"></asp:Label>
                    </td>
                    <td align="right" class="style9">
                        &nbsp;
                    </td>
                    <td align="left" class="style18">
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="TextBoxMediumStyle" Width="192px"
                            MaxLength="50"></asp:TextBox>
                    </td>
                    <td align="left" class="style8">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="style6">
                        &nbsp;
                    </td>
                    <td class="style10">
                        &nbsp;
                    </td>
                     <td colspan="2">
                                <div>
                        <asp:HiddenField ID="hdnLoginOrgName" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                        <asp:HiddenField ID="hdnOrglink" runat="server" Value="" />
                        <%--                        <input type="button" class="ButtonStyle" id="btnRequestPassword" value="Submit" onclick="ForgotPasswordRequest();"
                            style="color: #000000; background-color: #f5deb3; font-weight: bold;" />--%>
                        <asp:Button ID="btnRequestPassword" class="btnlogin" runat="server" Text="Submit"
                            OnClientClick="ForgotPasswordRequest();return false;" />
                        <input type="button" class="btnclear" id="btnClear" value="Clear" onclick="ClearData();"/>
                        <asp:Button ID="btnClose" runat="server" Text="Close" CssClass="btnclose" OnClick="btnClose_Click"/>
                        <asp:Button ID="btnSubmit" class="HiddenButton" runat="server" Text="Login" OnClick="btnSubmit_Click"
                            BackColor="Wheat" Font-Bold="True" />
                            </div>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
