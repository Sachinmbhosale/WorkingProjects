    <%@ Page Title="" Language="C#" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Login" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
         <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7-vsdoc.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.min.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.js") %>"></script>
    <script type="text/javascript" language="javascript" src="<%=Page.ResolveClientUrl("~/Scripts/AjaxPostScripts.js") %>"></script>
           <link href="<%= Page.ResolveClientUrl("~/Styles/Site.css") %>" rel="stylesheet" type="text/css" />
         
    <style type="text/css">
        html, body
        {
            margin: 0;
            padding: 0;
            min-height: 100%;
        }
        
        body
        {
            background-image: url('../Images/img_bg.jpg');
            background-position-x: center;
            background-position-y: bottom;
            background-repeat: no-repeat;
            background-attachment: scroll;
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
        }
        .bluredBackground
        {
            /*background-image: url('Images/img_bg.jpg'); /*border: 1px solid black;*/
            background-position: center bottom;
            opacity: 1;
            filter: alpha(opacity=100); /*For IE8 and earlier */
            background-repeat: no-repeat;
            background-attachment: scroll;
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
            width: 100%;
            height: 100%;
            position: absolute;
            top: 0px;
            left: 0px;
        }
        
        .background
        {
            background-image: url('../Images/img_loginbg.gif');
            background-position-x: center;
            background-position-y: bottom;
            background-repeat: no-repeat;
            background-attachment: scroll;
            -webkit-background-size: cover;
            -moz-background-size: cover;
            -o-background-size: cover;
            background-size: cover;
            vertical-align: text-top;
            padding-left: 15%;
        }
        
        a:link, a:visited
        {
            color: #000000;
        }
        
        .user
        {

           /* background:#FFFFFF url('Images/user_img.jpg') no-repeat 4px 4px;*/
            padding-left: 5px;
            border: 0px solid #CCC;
            height: 20px;
            background-color: #FFFFFF;
        }
        .pwd
        {
           /* background: url(Images/pwd_small.png) no-repeat;*/
            padding-left: 5px;
            border: 0px solid #CCC;
            background-color: #FFFFFF;
            height: 20px;
        }
        .imgsize
        {
            width: 21px;
            height: 21px;
        }
    </style>
    
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


        function ValidateUser() {
            var msgControl = "#<%= divMsg.ClientID %>";
            //document.getElementById("<%= txtCaptcha.ClientID %>").value = $("#<%= hdnCapta.ClientID %>").val(); // have to remove
            var action = "ValidateUser";
            var username = $("#<%= txtUsername.ClientID %>").val();
            var password = $("#<%= txtPassword.ClientID %>").val();
            var loginOrgCode = $("#<%= hdnLoginOrgCode.ClientID %>").val();
            var username = $("#<%= txtUsername.ClientID %>").val();
            var password = $("#<%= txtPassword.ClientID %>").val();
            var isDomainUser = false; // DUEN - Add
            var Domain = $("#<%= drpDomain.ClientID %>").val();
            if (typeof Domain == 'undefined' || Domain == null) {
                Domain = 0;
            }
            if ($("#<%= chkDomainUser.ClientID %>").is(':checked')) isDomainUser = true;
            var params = loginOrgCode + '|' + username + '|' + password + '|' + isDomainUser + '|' + Domain;

            if (ValidateInputData(msgControl, action, loginOrgCode, username, password)) {
                return CallPostScalar(msgControl, action, params);
            }
            else {
                return false;
            }
        }
        //********************************************************
        //ValidateInputData Function returns true or false with message to user on contorl specified
        //********************************************************

        function ValidateInputData(msgControl, action, loginOrgCode, username, password) {
            $(msgControl).html("");
            var txtCapta = $("#<%= txtCaptcha.ClientID %>").val();
            var capta = $("#<%= hdnCapta.ClientID %>").val();
            if (action == "ValidateUser") {
                if (username == "" || username == "username") {
                    $(msgControl).html("Please enter a user name");
                    document.getElementById("<%= txtUsername.ClientID %>").focus();
                    return false;
                }
                else if (password == "" || password == "password") {
                    $(msgControl).html("Please enter a password");
                    document.getElementById("<%= txtPassword.ClientID %>").focus();
                    return false;
                }
                else if (document.getElementById("<%= chkDomainUser.ClientID%>").checked == true && $("#<%= drpDomain.ClientID %>").val() == "0") {
                    $(msgControl).html("Please select domain");
                    document.getElementById("<%= drpDomain.ClientID %>").focus();
                    return false;
                }
                else if (txtCapta == "") {
                    $(msgControl).html("Please enter the captcha");
                    document.getElementById("<%= txtCaptcha.ClientID %>").focus();
                    return false;
                }
                else if (capta != txtCapta) {
                    $(msgControl).html("Invalid captcha..!");
                    document.getElementById("<%= txtCaptcha.ClientID %>").focus();
                    document.getElementById("<%= hdnmessage.ClientID %>").value = "Invalid captcha..!";
                    document.getElementById("<%= btnrefreshcapta.ClientID %>").click();
                    return false;

                }

                return true;
            }
        }

        /*---Added newly by sabina to login on enter key press from password textbox to make compatibile with firefox and chrome----*/
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

    <script type="text/javascript" language="javascript">

        //service url
        var authority = getRootWebSitePath();
        var coreServiceURL = authority + "/CoreService.svc";

        function getRootWebSitePath() {
            var _location = document.location.toString();
            var applicationNameIndex = _location.indexOf('/', _location.indexOf('://') + 3);
            var applicationName = _location.substring(0, applicationNameIndex) + '/';
            var webFolderIndex = _location.indexOf('/', _location.indexOf(applicationName) + applicationName.length);
            var webFolderFullPath = _location.substring(0, webFolderIndex);
            //If develoment environement WEB folder won't be there
            webFolderFullPath = webFolderFullPath.replace('/Accounts', '');
            return webFolderFullPath;
        }

    </script>

    <script language="javascript" type="text/javascript">
        function prevent_previous_page_return() {
            window.history.forward();
        }
        function imgLogo_onclick() {

        }

    </script>
</head>
<body style="background-color: #FFFFFF;">

    <form id="form1" runat="server">
    <div style="position: absolute; padding-top: 13%; padding-left: 7%;">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/logo_writer.gif" Height="80px" />
    </div>
    <div id="LoginForm" style="padding-top: 8%; padding-left: 65%; padding-right: 0px;
        text-align: right;">
        <div style="width: 500px; height: 540px;" class="background">
            <div id="DivLoginMain" style="padding-top: 20%; padding-left: 5%;">
                <br />
                <table style="border-style: none; border-color: inherit; border-width: 0px; width: 100%; border-collapse: collapse; text-align: left; margin-left: 17px; margin-top: 35px;">
                    <tr>
                        <td style="width: 30px; vertical-align: top; padding-left: 8%;">
                            <%--<img src="Images/logo_writer.jpg" alt="InfoSmart" style="width: 150px;" />--%>
                        </td>
                        <td style="font-size: 12px; color: #000000">
                             <div id="divMsg" runat="server" class="MessageMediumStyle">
                                    &nbsp;</div>
                            <%--<strong>&nbsp;Login Authentication</strong>--%>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div id="DivLoginSelect" runat="server">
                                <table style="border-collapse: collapse;border:0px;">
                                    <tr>
                                        <td style="width: 20px; vertical-align: top;">
                                            <%-- User Name <span class="Star">*</span><br />
                                            <br />--%>
                                             <asp:Label ID="Label1" runat="server" Text="User Name" ToolTip="User Name"
                            Style="color: Black; font-weight: bold;"></asp:Label>
                                            <%--<asp:Image ID="Image2" runat="server" ImageUrl="~/Images/user_img.jpg" CssClass="imgsize" />--%>
                                        </td>
                                        <td style="width: 250px">
                                            <asp:TextBox runat="server" ID="txtUsername" Width="220px" MaxLength="20" CssClass="user" TabIndex="1"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUserName" CssClass="ValidationMessage"
                                                ID="reqUserName" ErrorMessage="User Name is mandatory."></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20px; vertical-align: top;">
                                            <%-- Password <span class="Star">*</span><br />
                                            <br />--%>
                                             <asp:Label ID="Label3" runat="server" Text="Password" ToolTip="Password"
                            Style="color: Black; font-weight: bold;"></asp:Label>
                                           <%-- <asp:Image ID="Image3" runat="server" ImageUrl="~/Images/pwd_img.jpg" CssClass="imgsize" />--%>
                                        </td>
                                        <td style="width: 250px;">
                                            <asp:TextBox runat="server" TextMode="Password" ID="txtPassword" Width="220px" MaxLength="20"
                                                CssClass="pwd" TabIndex="2"></asp:TextBox>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" CssClass="ValidationMessage"
                                                ID="reqPassword" ErrorMessage="Password is mandatory."></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%-- Password <span class="Star">*</span><br />
                                            <br />--%>
                                           <asp:Label ID="lblcaptcha" runat="server" Text="Captcha" ToolTip="Captcha" Style="color: black; font-weight: bold;"></asp:Label>
                                        </td>
                                        <td style="width: 250px;">
                                          <asp:TextBox ID="txtCaptcha" runat="server" Width="220px" TabIndex="3"
                                                        autocomplete="off" ></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td  style="width: 250px;">
                                            <asp:Image ID="imgCaptcha" runat="server" Style="vertical-align: middle" />
                                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" OnClick="btnRefresh_Click"
                                    TabIndex="9" />
                                        </td>
                                    </tr>

                                    <tr>
                                        <td>
                                              <asp:Label ID="lblDomainUser" runat="server" Text="Check for Active Directory Login" ToolTip="Check for Active Directory Login"
                            Style="color: Black; font-weight: bold;"></asp:Label>
                                        </td>
                                        <td  style="width: 250px;">
                                           <asp:CheckBox ID="chkDomainUser" runat="server" onclick="return checkdomain();" TabIndex="5" />
                                        </td>
                                    </tr>


                                    <tr>
                                        <td>
                                            <asp:Label ID="Label2" runat="server" Text="Active Directory" ToolTip="Active Directory" Style="color: Black; font-weight: bold;"></asp:Label>
                                        </td>
                                        <td  style="width: 250px;">
                                          <asp:DropDownList ID="drpDomain" runat="server" TabIndex="6">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                           <a href="forgotPassword.aspx" class="LinkStyle" style="rgb(44, 3, 255);" 
                                                    tabindex="7">Forgot Password?</a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <input type="button"  class="btnlogin"
                         id="btnValidate" value="Login" 
                            onclick="ValidateUser(); return false;" tabindex="7" />
                        &nbsp
                        <asp:HiddenField ID="hdnLoginOrgCode" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                        <asp:HiddenField ID="hdnCapta" runat="server" Value="" />
                          <asp:HiddenField ID="hdnmessage" runat="server" Value="" />
                        <asp:Button ID="btnSubmit" class="HiddenButton" runat="server" Text="Login" OnClick="btnSubmit_Click"
                            TabIndex="20" />
                        <asp:Button ID="btnrefreshcapta" runat="server" class="HiddenButton" Text="capta" 
                            onclick="btnrefreshcapta_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <%--<tr>
                                                <td colspan="2" style="text-align: right; padding-right: 10px; padding-top: 5px;">
                                                    <asp:LinkButton ID="lnkLoginSelectBankToLogin" runat="server" Width="100px" OnClick="lnkLoginSelectBankToLogin_Click"
                                                        Text="Back to Login">
                                                    </asp:LinkButton>
                                                </td>
                                            </tr>--%>
                                </table>
                            </div>
                            <%-- A--%>
                           <%-- <div id="DivChangePasword" runat="server" style="border: 1px solid grey; padding-top: 5px"
                                visible="false">
                                <table style="width: 100%; border-collapse: collapse; text-align: left; vertical-align: top;">
                                    <tr>
                                        <td style="vertical-align: top; padding: 6px;">
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="DivForgetPassword" runat="server" style="border: 1px solid grey; padding-top: 5px"
                                visible="false">
                                <table style="width: 100%; border-collapse: collapse; text-align: left; vertical-align: top">
                                    <tr>
                                        <td style="vertical-align: top;">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 6px;">
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </div>--%>
                        </td>
                    </tr>
                </table>
                <br />
                <br />
                <br />
                <br />
                <br />
                <asp:Panel ID="ErrPanel" runat="server">
                </asp:Panel>
            </div>
        </div>
    </div>
    </form>




    <%--<div align="center">
        <asp:ScriptManager ID="sm1" runat="server" AsyncPostBackTimeout="1600" />
        <div class="LoginPanel">
            <table class="style5" border="0">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <tr>
                            <td align="center" colspan="4">
                                <div id="divMsg" runat="server" class="MessageMediumStyle">
                                    &nbsp;</div>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="style7">
                                <asp:Label ID="Label5" runat="server" Text="User ID" Style="color: black;
                                    font-weight: bold;"></asp:Label>
                            </td>
                            <td align="right" class="style9">
                                &nbsp;
                            </td>
                            <td align="left" class="style18">
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="TextBoxMediumStyle" Width="192px"
                                    TabIndex="1"></asp:TextBox>
                            </td>
                            <td align="left" class="style8">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="style6">
                                <asp:Label ID="Label6" runat="server" Text="Password" ToolTip="Password" Style="color: black; font-weight: bold;"></asp:Label>
                            </td>
                            <td align="right" class="style10">
                                &nbsp;
                            </td>
                            <td align="left" class="style19">
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="TextBoxMediumStyle" Width="193px"
                                    TextMode="Password" TabIndex="2"></asp:TextBox>
                            </td>
                            <td align="left">
                                &nbsp;
                            </td>
                        </tr>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:UpdatePanel ID="UP1" runat="server">
                    <ContentTemplate>
                        <tr>
                            <td align="right" class="style6">
                                <asp:Label ID="lblcaptcha" runat="server" Text="Captcha" ToolTip="Captcha" Style="color: black; font-weight: bold;"></asp:Label>
                            </td>
                            <td align="right" class="style10">
                                &nbsp;
                            </td>
                            <td align="left" class="style19">
                                <asp:TextBox ID="txtCaptcha" runat="server" Width="193px" CssClass="TextBoxMediumStyle"
                                    autocomplete="off" TabIndex="3"></asp:TextBox>
                            </td>
                            <td align="left">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="style6">
                            </td>
                            <td align="right" class="style10">
                                &nbsp;
                            </td>
                            <td align="left" colspan="2">
                                <asp:Image ID="imgCaptcha" runat="server" Style="vertical-align: middle" />
                                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" OnClick="btnRefresh_Click"
                                    TabIndex="8" />
                            </td>
                        </tr>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <tr>
                    <td align="right" class="style6">
                        <asp:Label ID="lblDomainUser" runat="server" Text="Check for Active Directory Login" ToolTip="Check for Active Directory Login"
                            Style="color: Black; font-weight: bold;"></asp:Label>
                    </td>
                    <td align="right" class="style10">
                        &nbsp;
                    </td>
                    <td align="left" class="style19">
                        <asp:CheckBox ID="chkDomainUser" runat="server" onclick="return checkdomain();" TabIndex="4" />
                    </td>
                    <td align="left">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="right" class="style6">
                        <asp:Label ID="Label2" runat="server" Text="Active Directory" ToolTip="Active Directory" Style="color: Black; font-weight: bold;"></asp:Label>
                    </td>
                    <td align="right" class="style10">
                        &nbsp;
                    </td>
                    <td align="left" class="style19">
                        <asp:DropDownList ID="drpDomain" runat="server" 
                             TabIndex="5">
                        </asp:DropDownList>
                    </td>
                    <td align="left">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="style14">
                    </td>
                    <td class="style15">
                    </td>
                    <td align="right" class="style20">
                        <a href="forgotPassword.aspx" class="LinkStyle" style="rgb(44, 3, 255);" 
                            tabindex="7">Forgot Password?</a>
                    </td>
                    <td class="style16">
                    </td>
                </tr>
                <tr>
                    <td class="style6">
                        &nbsp;
                    </td>
                    <td class="style10">
                        &nbsp;
                    </td>
                    <td align="left" class="style19">
                        <input type="button"  class="btnlogin"
                         id="btnValidate" value="Login" 
                            onclick="ValidateUser();return false;" tabindex="6" />
                        &nbsp
                        <asp:HiddenField ID="hdnLoginOrgCode" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                        <asp:HiddenField ID="hdnCapta" runat="server" Value="" />
                          <asp:HiddenField ID="hdnmessage" runat="server" Value="" />
                        <asp:Button ID="btnSubmit" class="HiddenButton" runat="server" Text="Login" OnClick="btnSubmit_Click"
                            TabIndex="20" />
                        <asp:Button ID="btnrefreshcapta" runat="server" class="HiddenButton" Text="capta" 
                            onclick="btnrefreshcapta_Click" />
                    </td>
                    <%--<td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </div>--%>
</body>
    </html>
