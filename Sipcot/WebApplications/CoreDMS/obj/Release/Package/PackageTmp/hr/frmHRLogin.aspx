<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="frmHRLogin.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.hr.frmHRLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <style>
        html, body
        {
            margin: 0;
            padding: 0;
            min-height: 100%;
        }
        
        body
        {
            /*background-image: url('Images/img_bg.jpg');*/
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
            background-image: url('Images/img_loginbg.gif');
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
</head>
<body>
    <form id="form1" runat="server">
     <div style="width: 100%; height: 100%; position: relative;">
       <%-- <div style="width: 100%; height: 15%; position: relative; background-image: url('Images/bg_hb1.jpg');">
        </div>--%>
        <div style="position: relative;padding: 6% 0 6% 0">
            <div style="position: absolute; padding-left: 3%; padding-top: 13%;">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/hr/Images/logo_writer_new.png" Height="95px" />
            </div>
             <div style="position: absolute; padding-left: 3%; padding-top: 13%;">
                <asp:Image ID="Image4" runat="server" ImageUrl="~/hr/Images/logo_writer_new.png" Height="95px" />
            </div>
             <div style="position: absolute; padding-left: 33%; padding-top: 15.5%;">
               <a style="font-size:24px;font-weight:600;color:#000000">HR - DMS</a>
            </div>
            <div style="position: absolute; width: 50%">
                <asp:Panel ID="ErrPanel" runat="server">
                </asp:Panel>
            </div>
           
            <div id="LoginForm" style="padding-top: 1%; padding-bottom: 1%; padding-left: 65%;
                padding-right: 0px; text-align: right; background-color: #FFFFFF; height: 100%;">
                <div style="width: 380px; height: 430px;" class="background">
                    <div id="DivLoginMain" style="padding-top: 20%; padding-left: 5%;">
                        <br />
                        <br />
                        <br />
                        <br />
                        <br />
                        <br />
                        <table style="width: 100%; border-collapse: collapse; text-align: left; border: 0px;">
                            <tr>
                                <td style="width: 150px; vertical-align: top; padding-left: 8%;">
                                </td>
                                <td style="font-size: 12px; color: #000000">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div id="DivLoginSelect" runat="server">
                                        <table style="border-collapse: collapse; border: 0px;">
                                            <tr>
                                                <td style="width: 20px; vertical-align: top;">
                                                    <asp:Image ID="Image2" runat="server" ImageUrl="~/hr/Images/user_img.jpg" CssClass="imgsize" />
                                                </td>
                                                <td style="width: 250px">
                                                    <asp:TextBox runat="server" ID="txtUserName" Width="220px" MaxLength="20" CssClass="user"></asp:TextBox>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtUserName" CssClass="ValidationMessage"
                                                        ID="reqUserName" ErrorMessage="User Name is mandatory."></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="width: 20px; vertical-align: top;">
                                                    <asp:Image ID="Image3" runat="server" ImageUrl="~/hr/Images/pwd_img.jpg" CssClass="imgsize" />
                                                </td>
                                                <td style="width: 250px;">
                                                    <asp:TextBox runat="server" TextMode="Password" ID="txtPassword" Width="220px" MaxLength="20"
                                                        CssClass="pwd"></asp:TextBox>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" CssClass="ValidationMessage"
                                                        ID="reqPassword" ErrorMessage="Password is mandatory."></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    &nbsp;
                                                </td>
                                                <td style="text-align: left; padding-left: 55%;">
                                                    <asp:Button class="submitButton" ID="btnLogin" runat="server" Font-Bold="True" Font-Names="CG Omega"
                                                        Font-Size="12pt" Font-Strikeout="False" Font-Underline="False" Text="Login" OnClick="btnLogin_Click" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                </td>
                                                <td>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div id="DivChangePasword" runat="server" style="border: 1px solid grey; padding-top: 5px"
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
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <%--<div style="width: 100%; height: 15%; position: relative; background-image: url('Images/bg_hb2.jpg');">
        </div>--%>
    </div>
    </form>
</body>
</html>
