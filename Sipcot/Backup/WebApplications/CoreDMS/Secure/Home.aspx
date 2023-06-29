<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="Home.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Home" %>

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
    <div class="CenterBaseDiv">
        <table>
            <tr>
                <td>
                    <div style="text-align: center; width: 1225px;">
                        <asp:Label CssClass="PageHeadingsAllign" Text="Dashboard" ID="lblDashboard" Style="font-weight: bold;"
                            runat="server">
                        </asp:Label>
                    </div>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <table class="style1" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td runat="server" id="tdDashboard">
                    <div style="text-align: center; width: 600px;">
                        <h1 class="style13">
                            Dashboard</h1>
                    </div>
                    <div id="divMsg" runat="server">
                    </div>
                </td>
                <td valign="top">
                    <table border="0" cellpadding="0" cellspacing="0">
                        <tr>
                            <td runat="server" id="tdCustomerData">
                                <div style="text-align: left; width: 700px;">
                                    <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td>
                                                <div>
                                                    <div class="LabelDiv">
                                                        <asp:Label ID="lblHeading0" runat="server" CssClass="LabelStyle" Text="Customer" /></div>
                                                    <asp:Label ID="lblCustomerName" runat="server" CssClass="LabelStyle" Text=""></asp:Label><br />
                                                    <br />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div>
                                                    <div class="LabelDiv">
                                                        <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="Address" /></div>
                                                    <asp:Label ID="lblAddress" runat="server" CssClass="LabelStyle" Text=""></asp:Label><br />
                                                    <br />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div>
                                                    <div class="LabelDiv">
                                                        <asp:Label ID="Label4" runat="server" CssClass="LabelStyle" Text="Email" />
                                                    </div>
                                                    <asp:Label ID="lblOrgEmail" runat="server" CssClass="LabelStyle" Text=""></asp:Label>
                                                    <br />
                                                    <br />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div>
                                                    <div class="LabelDiv">
                                                        <asp:Label ID="Label5" runat="server" CssClass="LabelStyle" Text="Phone"></asp:Label></div>
                                                    <asp:Label ID="lblPhoneNo" runat="server" CssClass="LabelStyle" Text=""></asp:Label>
                                                    <br />
                                                    <br />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div>
                                                    <div class="LabelDiv">
                                                        <asp:Label ID="Label6" runat="server" CssClass="LabelStyle" Text="Fax" /></div>
                                                    <asp:Label ID="lblFaxNo" runat="server" Text="" CssClass="LabelStyle"></asp:Label>
                                                    <br />
                                                    <br />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div>
                                                    <div class="LabelDiv">
                                                        <asp:Label ID="Label7" runat="server" CssClass="LabelStyle" Text="Contact Person" /></div>
                                                    <asp:Label ID="lblContactPerson" runat="server" CssClass="LabelStyle" Text=""></asp:Label>
                                                    <br />
                                                    <br />
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div>
                                                    <div class="LabelDiv">
                                                        <asp:Label ID="Label8" runat="server" CssClass="LabelStyle" Text="Mobile:" /></div>
                                                    <asp:Label ID="lblContactMobile" runat="server" CssClass="LabelStyle" Text=""></asp:Label>
                                                    <br />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <asp:LinkButton ID="lnkEditCustomer" runat="server" CssClass="LinkStyle" OnClick="lnkEditCustomer_Click"
                                        Enabled="False" Visible="False">Edit Customer Info >>
                                    </asp:LinkButton>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="style11">
                    &nbsp;
                </td>
                <td valign="top">
                    <div style="text-align: left; width: 600px;">
                        <table border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td colspan="4">
                                    <asp:Label ID="Label17" runat="server" CssClass="SubDivHeaderItemText">User Details</asp:Label><br />
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <asp:Label ID="lblDocumentName0" runat="server" CssClass="LabelStyle">You have Logged in as:&nbsp;
                                    </asp:Label>
                                    <asp:Label ID="lblUserName" runat="server" CssClass="LabelStyle"></asp:Label><%--<asp:Label
                                        ID="lblOrgName" runat="server" CssClass="LabelStyle" Style="visibility: hidden;">
                                    </asp:Label>--%>
                                </td>
                                <td valign="top">
                                    <asp:Label ID="lblDay" runat="server" CssClass="LabelStyleGray">Jan 03, Thursday, 
                                    </asp:Label>
                                    &nbsp;<asp:Label ID="lblTime" runat="server" CssClass="LabelStyleGray">10:20 AM
                                    </asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                    <asp:Label ID="lblDocumentName4" runat="server" CssClass="LabelStyle">Your Last Login:
                                    </asp:Label>
                                    &nbsp;&nbsp;&nbsp; &nbsp;
                                    <asp:Label ID="lblLastLoggedDay" runat="server" CssClass="LabelStyleGray">Jan 03, Thursday, 
                                    </asp:Label>
                                    &nbsp;<asp:Label ID="lblLastLoggedTime" runat="server" CssClass="LabelStyleGray">10:20 AM</asp:Label>
                                    <br />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <br />
                    <br />
                    <asp:Label ID="lblDocumentName7" runat="server" CssClass="LabelStyleGray">
                    </asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblDocumentName10" runat="server" CssClass="LabelStyleGray">
                    </asp:Label>
                    <br />
                    <br />
                    <asp:Label ID="lblDocumentName11" runat="server" CssClass="LabelStyleGray">
                    </asp:Label>
                    <br />
                    <br />
                    <br />
                    <asp:LinkButton ID="lnkCustomerLogin" runat="server" CssClass="LinkStyle" OnClick="lnkCustomerLogin_Click">Login to Customer Portal >>
                    </asp:LinkButton>
                </td>
            </tr>
        </table>
        <br />
    </div>
</asp:Content>
