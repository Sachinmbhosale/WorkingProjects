v<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="ChanagePassword.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7-vsdoc.js") %>"></script>

    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/hex_md5.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.min.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.js") %>"></script>
    <script type="text/javascript" language="javascript" src="<%=Page.ResolveClientUrl("~/Assets/Scripts/AjaxPostScripts.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Assets/Scripts/master.js") %>"></script>
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
        });



        function EncryptPassword1(str) {


            if (document.getElementById('<%= txtNewPassword.ClientID %>').value != "" && document.getElementById('<%= txtRetryNewPassord.ClientID %>').value != "") {


                var md5encyptOldPassword1 = hex_md5(document.getElementById('<%= txtOldPassword.ClientID %>').value);


                var md5encyptretry1 = hex_md5(document.getElementById('<%= txtOldPassword.ClientID %>').value);
                var passstrretypepassword1 = str + (md5encyptretry1);
                document.getElementById('<%= txtOldPassword.ClientID  %>').value = hex_md5(passstrretypepassword1);


                var md5encyptretryn = hex_md5(document.getElementById('<%= txtNewPassword.ClientID %>').value);
                var passstrretypepasswordn = str + (md5encyptretryn);
                document.getElementById('<%= txtNewPassword.ClientID  %>').value = hex_md5(passstrretypepasswordn);


                var md5encyptOldPassword2 = hex_md5(document.getElementById('<%= txtRetryNewPassord.ClientID %>').value);

                var md5encyptOldPassword2 = hex_md5(document.getElementById('<%= TextBox1.ClientID %>').value);

                var passstrretypepasswordn = str + (md5encyptOldPassword2);

                document.getElementById('<%= TextBox1.ClientID  %>').value = hex_md5(passstrretypepasswordn);

                btnSubmitControlID = "<%= btnSubmit.ClientID %>";


            }
        }





<%--        function EncryptPassword1(str) {

            if (document.getElementById('<%= txtNewPassword.ClientID %>').value != "" && document.getElementById('<%= txtRetryNewPassord.ClientID %>').value != "") {

                var md5encyptOldPassword1 = hex_md5(document.getElementById('<%= txtOldPassword.ClientID %>').value);

                var md5encyptretry1 = hex_md5(document.getElementById('<%= txtOldPassword.ClientID %>').value);
                var passstrretypepassword1 = str + (md5encyptretry1);
                document.getElementById('<%= txtOldPassword.ClientID  %>').value = hex_md5(passstrretypepassword1);

                var md5encyptOldPasswordn = hex_md5(document.getElementById('<%= txtNewPassword.ClientID %>').value);

                var md5encyptretryn = hex_md5(document.getElementById('<%= txtNewPassword.ClientID %>').value);
                var md5encyptretryn = hex_md5(document.getElementById('<%= TextBox1.ClientID %>').value);                
                var passstrretypepasswordn = str + (md5encyptretryn);
                document.getElementById('<%= TextBox1.ClientID  %>').value = hex_md5(passstrretypepasswordn);

                var md5encyptOldPassword2 = hex_md5(document.getElementById('<%= txtRetryNewPassord.ClientID %>').value);                             
                document.getElementById('<%= txtRetryNewPassord.ClientID  %>').value = hex_md5(passstrretypepasswordn);

                 btnSubmitControlID = "<%= btnSubmit.ClientID %>";


            }


                }--%>



        function ValidateUser() {
          
            var msgControl = "#<%= divMsg.ClientID %>";
            var action = "ChangePassword";
            var username = $("#<%= hdnUserName.ClientID %>").val();
         
       
            var params = username + '|' + password + '|' + newPassword;
            if (ValidateInputData(msgControl, action)) {
                return CallPostScalar(msgControl, action, params);
            }
            else {
                return false;
            }
        }
        //********************************************************
        //ValidateInputData Function returns true or false with message to user on contorl specified
        //********************************************************

        function ValidateInputData(msgControl, action) {
            debugger;
            $(msgControl).html("");
            var regularExpression = /^(?=.*[0-9])(?=.*[!@#$%^&_)(*.,><~?])[a-zA-Z0-9!@#$%^&_)(*.,><~?]{8,10}$/;
            if (action == "ChangePassword") {
                
                   if (newPassword.length < 8 || newPassword.length > 10) {
                    $(msgControl).html("New Password should be 8-10 characters long. Please re-enter the password");
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


        <div id="divmsgsuccess" runat="server" style="color: Green">
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

<%--                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtOldPassword" CssClass="ValidationMessage" Display="None"  ID="reqUserName" ErrorMessage="Current Password is mandatory."></asp:RequiredFieldValidator>--%>
                        <asp:TextBox ID="txtOldPassword" runat="server" MaxLength="50" Text="" EnableViewState="False"
                            TextMode="Password" ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td height="18" colspan="1">
                        <asp:Label ID="Label2" CssClass="LabelStyle" runat="server" Text="New Password :"></asp:Label>
                    </td>
                    <td height="38" colspan="1">
<%--                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNewPassword" CssClass="ValidationMessage"  Display="None"  ID="RequiredFieldValidator1" ErrorMessage="Curren Password is mandatory."></asp:RequiredFieldValidator>--%>
                        <asp:TextBox ID="txtNewPassword" runat="server" MaxLength="50" Text="" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td height="18" colspan="1">
                        <asp:Label ID="Label3" CssClass="LabelStyle" runat="server" Text="Re-Enter New Password :"></asp:Label>
                    </td>
                    <td height="38" colspan="1">

                        <%--<asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtNewPassword" ControlToValidate="txtRetryNewPassord" ErrorMessage="Password does not match."></asp:CompareValidator>--%>
                        <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="txtRetryNewPassord" CssClass="ValidationMessage" Display="None" ID="RequiredFieldValidator2" ErrorMessage="Curren Password is mandatory."></asp:RequiredFieldValidator>--%>
                        <asp:TextBox ID="txtRetryNewPassord" runat="server" MaxLength="50" Text="" TextMode="Password"></asp:TextBox>
                    </td>
                </tr>
                <tr >
                    <td style="display:none"  colspan="2">&nbsp;
                        <asp:TextBox ID="TextBox1"  runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:HiddenField ID="hdnUserName" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                          <asp:HiddenField ID="hidtemppass" runat="server" Value="" />
                        <asp:HiddenField ID="hdnpass" runat="server" Value="" />
                      <%--  <input type="button" id="btnSubmit" value="Save" OnClientClick="ValidateUser();" OnClick="Btnlogin_Click" class="btnsave" />--%>

                        <asp:Button ID="btnSubmit" class="btnsave"  runat="server" Text="Save"   OnClick="Btnlogin_Click"  />
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