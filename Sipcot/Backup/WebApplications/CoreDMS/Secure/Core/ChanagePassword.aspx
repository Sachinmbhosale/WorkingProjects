<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="ChanagePassword.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7-vsdoc.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.min.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.js") %>"></script>
    <script type="text/javascript" language="javascript" src="<%=Page.ResolveClientUrl("~/Assets/Scripts/AjaxPostScripts.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Assets/Scripts/master.js") %>"></script>
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
        });

        function ValidateUser() {
            var msgControl = "#<%= divMsg.ClientID %>";
            var action = "ChangePassword";
            var username = $("#<%= hdnUserName.ClientID %>").val();
            var password = $("#<%= txtOldPassword.ClientID %>").val();
            var newPassword = $("#<%= txtNewPassword.ClientID %>").val();
            var retryPassword = $("#<%= txtRetryNewPassord.ClientID %>").val();
            var params = username + '|' + password + '|' + newPassword;
            if (ValidateInputData(msgControl, action, password, newPassword, retryPassword)) {
                return CallPostScalar(msgControl, action, params);
            }
            else {
                return false;
            }
        }
        //********************************************************
        //ValidateInputData Function returns true or false with message to user on contorl specified
        //********************************************************

        function ValidateInputData(msgControl, action, password, newPassword, retryPassword) {
            $(msgControl).html("");
            var regularExpression = /^(?=.*[0-9])(?=.*[!@#$%^&_)(*.,><~?])[a-zA-Z0-9!@#$%^&_)(*.,><~?]{8,10}$/;
            if (action == "ChangePassword") {
                if (password == "") {
                    $(msgControl).html("Password fields cannot be blank.");
                    document.getElementById("<%= txtOldPassword.ClientID %>").focus();
                    return false;
                }
                else if (newPassword == "") {
                    $(msgControl).html("Password fields cannot be blank");
                    document.getElementById("<%= txtNewPassword.ClientID %>").focus();
                    return false;
                }
                else if (password == newPassword) {
                    $(msgControl).html("New Password cannot be same as Current Password");
                    document.getElementById("<%= txtNewPassword.ClientID %>").value = "";
                    document.getElementById("<%= txtNewPassword.ClientID %>").focus();
                    return false;
                }
                else if (newPassword.length < 8 || newPassword.length > 10) {
                    $(msgControl).html("New Password should be 8-10 characters long. Please re-enter the password");
                    document.getElementById("<%= txtNewPassword.ClientID %>").focus();
                    return false;
                }
                else if (retryPassword == "") {
                    $(msgControl).html("Password fields cannot be blank");
                    document.getElementById("<%= txtRetryNewPassord.ClientID %>").focus();
                    return false;
                }
                else if (newPassword != retryPassword) {
                    $(msgControl).html("New password and Re-Enter new password does not match. Please try again.");
                    document.getElementById("<%= txtNewPassword.ClientID %>").value = "";
                    document.getElementById("<%= txtRetryNewPassord.ClientID %>").value = "";
                    document.getElementById("<%= txtNewPassword.ClientID %>").focus();
                    return false;
                }
                else if (!regularExpression.test(newPassword)) {
                    $(msgControl).html("New Password should contain atleast one number and one special character.");
                    return false;
                }
                return true;
            }
        }
        //********************************************************
        //ClearData Function clears the form
        //********************************************************

        function ClearData() {
            document.getElementById("<%= txtOldPassword.ClientID %>").value = ""
            document.getElementById("<%= txtNewPassword.ClientID %>").value = ""
            document.getElementById("<%= txtRetryNewPassord.ClientID %>").value = ""
            document.getElementById("<%= txtOldPassword.ClientID %>").focus();
        }
        //********************************************************
        //ClearData Function navigate to Homepage
        //********************************************************

        
        
        $(document).ready(function () {
            $(this).keydown(function (event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                    document.getElementById("btnSubmit").click();
                }
            });

        });

    </script>
    <style type="text/css">
    .table1
    {
        height:157px;
        width:450px;
        border:0px;
        
    }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="GVDiv">
     <h3>
                            Change Password</h3>
     <div id="divMsg" runat="server" style="color: Red">
                                &nbsp;</div>
        <fieldset>
            <table class="table1" align="left">
                <tr>
                    <td height="23" colspan="2">
                       
                              <asp:Label ID="Label4" runat="server" Text="(Password should contain minimum 8 characters, at least one special character and one numeric value.)"></asp:Label>
                    </td>

                </tr>
                <tr>
                    <td height="18" colspan="1">
                        <asp:Label ID="Label1" CssClass="LabelStyle" runat="server" Text="Current Password :"></asp:Label>
                    </td>
                    <td height="38" colspan="1">
                        <asp:TextBox ID="txtOldPassword" runat="server" MaxLength="50" Text="" EnableViewState="False"
                            TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td height="18" colspan="1">
                        <asp:Label ID="Label2" CssClass="LabelStyle" runat="server" Text="New Password :"></asp:Label>
                    </td>
                    <td height="38" colspan="1">
                        <asp:TextBox ID="txtNewPassword" runat="server" MaxLength="50" Text="" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td height="18" colspan="1">
                        <asp:Label ID="Label3" CssClass="LabelStyle" runat="server" Text="Re-Enter New Password :"></asp:Label>
                    </td>
                    <td height="38" colspan="1">
                        <asp:TextBox ID="txtRetryNewPassord" runat="server" MaxLength="50" Text="" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:HiddenField ID="hdnUserName" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                        <input type="button" id="btnSubmit" value="Save" onclick="ValidateUser();" class="btnsave" />
                        <input type="button" id="btnClear" value="Clear" onclick="ClearData();" class="btnclear" />
                        <asp:Button ID="btnCancel" runat="server" CssClass="btnclose" Text="Close" OnClick="btnCancel_Click" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
</asp:Content>
