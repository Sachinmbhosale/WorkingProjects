<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="CustomerLogin.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.CustomerLogin" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1
        {
            width: 100%;
            height: 329px;
        }
        .class=
        {
            height: 65px;
        }
        
        .style10
        {
            width: 347px;
        }
        .style11
        {
            width: 11px;
        }
        .style13
        {
            font-family: Arial;
            text-align: center;
            font-style: italic;
            color: #C0C0C0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Customer Portal Login"></asp:Label>
    <div class="GVDiv">
       <asp:Label ID="lblPageHeader" runat="server" CssClass="PageHeadings" Text="Customer Portal Login"></asp:Label>
    <fieldset>
    <table>
        <tr>
            <td colspan="2">
                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                    <tr>
                        <td align="left">
                            <div id="divMsg" runat="server" style="color: Red">
                                &nbsp;</div>
                        </td>                       
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top;">
                <div class="SearchFilterDiv">
                    <asp:Label ID="lblSubHeading" CssClass="SubHeadings" runat="server" Text="Select Customer"></asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblUserName" runat="server" CssClass="LabelStyle" Text="Customer Name"></asp:Label>
                    <br />
                    <br />
                    <asp:DropDownList ID="cmbCustomers" runat="server">
                    </asp:DropDownList>
                    <br />
                    <br />
                    <asp:Button ID="btnSubmit" class="btn" runat="server" Text="Login >>" Width="100px"  OnClick="btnSubmit_Click" />
                    <br />
                    <br />
                </div>
            </td>
            <td valign="top" >
                <table width="600" border="0" >
                    <tr>
                        <td height="8" colspan="3" valign="top" >
                            <center>
                                <div id="divSearchResults">
                                    &nbsp;</div>
                            </center>
                        </td>
                    </tr>
                    <tr>
                        <td height="8" colspan="3" align="right">
                            <div id="divPagingText">
                                &nbsp;</div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </fieldset>
    </div>
  
</asp:Content>
