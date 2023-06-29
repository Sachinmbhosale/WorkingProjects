<%@ Page Title="" Language="C#" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Login" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0, minimum-scale=1.0, maximum-scale=1.0">
    <link rel="shortcut icon" href="assets/images/fav.png">
    <title>Login</title>
    <link href="assets/plugins/bootstrap/css/bootstrap.min.css" type="text/css" rel="stylesheet">
    <link href="css/style.css" type="text/css" rel="stylesheet">
    <link href="css/login.css" rel="stylesheet">
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-3.6.0.min.js") %>"></script>
    <%--<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7-vsdoc.js") %>"></script>--%>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/hex_md5.js") %>"></script>
    <%--<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.min.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.js") %>"></script>--%>
    <link href="<%= Page.ResolveClientUrl("~/Styles/Site.css") %>" rel="stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript">
        ///check for keyDownFunction()
        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            loginmessage = "<%= hdnmessage.ClientID %>";

            btnSubmitControlID = "<%= btnSubmit.ClientID %>";

            if (document.getElementById("<%= chkDomainUser.ClientID%>").checked == false) {
                document.getElementById("<%= drpDomain.ClientID%>").disabled = true;
            }
            else {
                document.getElementById("<%= drpDomain.ClientID%>").disabled = false;
            }

            btnrefreshCaptcha = "<%= btnrefreshcapta.ClientID %>";

        });

        function ShowConfirm() {
            confirm("your last session is active, do you want to cancel the previous session...!");
        }

        function EncryptPassword1(str) {
            if (document.getElementById('<%= txtUsername.ClientID %>').value == '') {
                alert('Enter username');
                document.getElementById('<%= txtPassword.ClientID %>').value = ''
                document.getElementById('<%= txtUsername.ClientID %>').focus();

                return false;
            }

            if (document.getElementById('<%= txtPassword.ClientID %>').value == '') {
                alert('Enter password');
                document.getElementById('<%= txtPassword.ClientID %>').value = ''
                document.getElementById('<%= txtPassword.ClientID %>').focus();
                return false;
            }

            var txtCapta = $("#<%= txtCaptcha.ClientID %>").val();
            var capta = $("#<%= hdnCapta.ClientID %>").val();
            if (txtCapta == "") {

                alert('Enter captcha');
                document.getElementById("<%= txtCaptcha.ClientID %>").focus();
                return false;


            }
            if (capta != txtCapta) {

                alert('Invalid captcha');
                document.getElementById("<%= txtCaptcha.ClientID %>").focus();
                document.getElementById("<%= btnrefreshcapta.ClientID %>").click();
                return false;

            }


            if (document.getElementById('<%= txtPassword.ClientID %>').value != "") {

                var md5encypt = hex_md5(document.getElementById('<%= txtPassword.ClientID %>').value);
                var passstr = str + (md5encypt);              
                document.getElementById('<%= txtPassword.ClientID  %>').value = hex_md5(passstr);
                BtnloginControlID = "<%= Btnlogin.ClientID %>";

            }

        }


        function ValidateUser() {
            var msgControl = "#<%= divMsg.ClientID %>";

            // document.getElementById("<%= txtCaptcha.ClientID %>").value = $("#<%= hdnCapta.ClientID %>").val(); // have to remove
              var action = "ValidateUser";
              var username = $("#<%= txtUsername.ClientID %>").val();
            var loginOrgCode = $("#<%= hdnLoginOrgCode.ClientID %>").val();
            var isDomainUser = false; // DUEN - Add
            var Domain = $("#<%= drpDomain.ClientID %>").val();
            if (typeof Domain == 'undefined' || Domain == null) {
                Domain = 0;
            }

            if (ValidateInputData(msgControl, action, loginOrgCode, username)) {

            }
            else {

                return false;
            }
        }
        //********************************************************
        //ValidateInputData Function returns true or false with message to user on contorl specified
        //********************************************************

        function ValidateInputData(msgControl, action, loginOrgCode, username) {
            debugger;

            $(msgControl).html("");
            var txtCapta = $("#<%= txtCaptcha.ClientID %>").val();
            var capta = $("#<%= hdnCapta.ClientID %>").val();


            if (action == "ValidateUser") {
                if (username == "" || username == "username") {
                    $(msgControl).html("Please enter a user name");
                    document.getElementById("<%= txtUsername.ClientID %>").focus();
                    return false;
                }
                return true;
            }

        }
        $(document).ready(function () {
            $("#<%= txtPassword.ClientID %>,#<%= txtUsername.ClientID %>,#<%= txtCaptcha.ClientID %>").keydown(function (event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                    document.getElementById("btnValidate").click();
                }
            });

        });



        //********************************************************
        // DUEN - Add
        //********************************************************
        function checkdomain() {
            if (document.getElementById("<%= chkDomainUser.ClientID%>").checked == true) {
                document.getElementById("<%= drpDomain.ClientID%>").disabled = false;
            }
            else {
                document.getElementById("<%= drpDomain.ClientID%>").selectedIndex = 0;
                document.getElementById("<%= drpDomain.ClientID%>").disabled = true;
            }
        }

    </script>



    <script type="text/javascript">

        function preventBack() { window.history.forward(); }
        setTimeout("preventBack()", 0);
        window.onunload = function () { null };
    </script>
</head>

<body class="login">

    <div class="login-logo">
        <ul>
            <li><a href="https://www.writerinformation.com/" target="_blank">
                <img src="assets/images/sipcotlogo_1.PNG" alt="Writer Information"></a></li>
            <li> <p style="padding-left:100px; font-size:25px; font-weight:bold; color:black">  STATE INDUSTRIES PROMOTION CORPORATION OF TAMILNADU LTD (SIPCOT)</p></li>
            <li><a href="#" target="_blank">
                <img src="assets/images/n-logo.png" alt="">
            </a></li>
        </ul>
    </div>
    <!-- start: LOGIN -->
    <div class="main-login">

        <!-- start: LOGIN BOX -->
        <div class="box-login">

            <form id="form1" runat="server" class="form-login">

                <div class="logo clearfix">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/newlogo.png" />
                </div>
                <fieldset>
                    <legend>Sign in to your account
                    </legend>

                    <div class="form-group">
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUserName" CssClass="ValidationMessage" Display="None" ID="reqUserName" ErrorMessage="User Name is mandatory."></asp:RequiredFieldValidator>
                        <span class="input-icon">

                            <asp:TextBox runat="server" ID="txtUsername" autocomplete="off" MaxLength="20" CssClass="form-control" placeholder="Username"></asp:TextBox>

                            <i class="ti-user"></i></span>
                    </div>

                    <div class="form-group">
                        <span class="input-icon">
                            <asp:TextBox runat="server" TextMode="Password" ID="txtPassword" autocomplete="off" CssClass="form-control password" placeholder="Password" EnableViewState="False"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" CssClass="ValidationMessage"
                                ID="reqPassword" ErrorMessage="Password is mandatory." Display="None"></asp:RequiredFieldValidator>
                            <i class="ti-lock"></i>

                            <a href="forgotPassword.aspx" class="forgot">Forgot Password?</a>
                        </span>
                    </div>

                    <div id="DivLoginSelect" runat="server">
                        <div class="row">
                            <div class="col-4 pr-0">
                                <asp:Image ID="imgCaptcha" runat="server" Style="vertical-align: middle" />
                            </div>
                            <div class="col-6 p-0">
                                <asp:TextBox runat="server" ID="txtCaptcha" autocomplete="off" TabIndex="3" CssClass="form-control"></asp:TextBox>


                            </div>
                            <div class="col-2 pl-0">
                                <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-block btn-outline-inverse" Text="&#8635;" OnClick="btnRefresh_Click"></asp:Button>
                            </div>
                        </div>


                        <div class="text-center mt-3">
                            <asp:Button ID="Btnlogin" class="btn btn-outline-inverse" runat="server" Text="Login" TabIndex="7" OnClick="Btnlogin_Click" />
                            <%--     OnClientClick="ValidateUser(); return false;" --%>
                        </div>
                        <input type="button" class="btn btn-outline-inverse" id="btnValidate" value="Validate" style="display: none" onclick="ValidateUser(); return false;" />

                        <table style="display: none">

                            <tr>
                                <td>
                                    <asp:Label ID="lblDomainUser" runat="server" Text="Check for Active Directory Login" ToolTip="Check for Active Directory Login"
                                        Style="color: Black; font-weight: bold;"></asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkDomainUser" runat="server" onclick="return checkdomain();" TabIndex="5" />
                                </td>
                            </tr>


                            <tr style="display: none">
                                <td>
                                    <asp:Label ID="Label2" runat="server" Text="Active Directory" ToolTip="Active Directory" Style="color: Black; font-weight: bold;"></asp:Label>
                                </td>
                                <td style="width: 250px;">
                                    <asp:DropDownList ID="drpDomain" runat="server" TabIndex="6">
                                    </asp:DropDownList>
                                </td>
                            </tr>

                            <tr>
                                <td>&nbsp;
                                </td>

                            </tr>
                            <tr>
                                <td>&nbsp;
                                </td>
                                <td>
                                    <div id="divMsg" runat="server" class="MessageMediumStyle">
                            &nbsp;
                        </div>

                                    <asp:HiddenField ID="hdnLoginOrgCode" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnCapta" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnmessage" runat="server" Value="" />
                                    <asp:Button ID="btnSubmit" class="HiddenButton" runat="server" Text="Login" OnClick="btnSubmit_Click"
                                        TabIndex="20" />


                                    <asp:Button ID="btnrefreshcapta" runat="server" class="HiddenButton" Text="capta"
                                        OnClick="btnrefreshcapta_Click" />
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                            </tr>
                        
                        </table>
                        <asp:Panel ID="ErrPanel" runat="server">
                        </asp:Panel>
                        
                    </div>


                </fieldset>
            </form>
        </div>
    </div>

    <ul class="cb-slideshow">
        <li><span>Image 01</span></li>
    </ul>
    <div class="footer-style">
        <div class="container">
            Powered by <a href="https://www.writerinformation.com/" target="_blank">Writer Information.</a>
        </div>
    </div>
</body>
</html>
