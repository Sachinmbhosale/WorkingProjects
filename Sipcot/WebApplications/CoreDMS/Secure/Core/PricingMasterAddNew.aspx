<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PricingMasterAddNew.aspx.cs"
    Inherits="Lotex.EnterpriseSolutions.WebUI.PricingMasterSearch" MasterPageFile="~/SecureMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        .style2
        {
            width: 160px;
        }
    </style>
    <script type="text/javascript" language="javascript">

        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";
            pageRightsContorlId = "<%= hdnPageRights.ClientID %>"
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        //Final Validation
        function validate() {
            var msgControl = "#<%= divMsg.ClientID %>";
            var Customer = document.getElementById('<%=drpCustomer.ClientID %>');
            var DocuType = document.getElementById('<%=drpDocutype.ClientID %>');
            var BillType = document.getElementById('<%=drpBilltype.ClientID %>');
            var charge = document.getElementById('<%=txtCharge.ClientID %>');
            var currency = document.getElementById('<%=drpCurrency.ClientID %>');
            if (Customer.value == 0) {
                $(msgControl).html("Please Select Customer.");
                return false;
            }
            else if (DocuType.value == 0) {
                $(msgControl).html("Please Select Project Type.");
                return false;
            }
            else if (BillType.value == 0) {
                $(msgControl).html("Please Select Bill Type.");
                return false;
            }
            else if (charge.value.length == 0) {
                $(msgControl).html("Please enter the charges");
                return false;
            }
            else if (currency.value == 0) {
                $(msgControl).html("Please Select Currency.");
                return false;
            }
            else {
                return true;
            }
        }

        //Charges Textbox
        function checkkey(event) {
            var code = (event.keyCode ? event.keyCode : event.which);
            if ((code >= 48 && code <= 57) || code == 46 || code == 8) {
                return true;
            }
            else {
                return false;
            }
        }
    </script>
    <asp:Label CssClass="CurrentPath" ID="lblPagePath" runat="server" Text="Home  &gt;  Pricing Master"></asp:Label>
    <div class="GVDiv">
        <asp:Label CssClass="PageHeadings" ID="lblHeading" runat="server" Text="Add New Pricing Master
"></asp:Label>
        <br />
        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="lblPageDescription" runat="server"
            Text="* "></asp:Label>
        <asp:Label CssClass="CurrentPath" ID="lblPageDescription1" runat="server" Text=" - Indicates mandatory fields"></asp:Label>
        <div id="divMsg" runat="server" style="color: Red; font-family: Calibri; font-size: small;">
        </div>
        <fieldset>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblDescription" runat="server" CssClass="LabelStyle" Text="Customer"></asp:Label>
                        &nbsp;<asp:Label ID="lblDescription4" runat="server" CssClass="MandratoryFieldMarkStyle"
                            Text="*"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:DropDownList ID="drpCustomer" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drpCustomer_SelectedIndexChanged"
                            Style="">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDescription0" runat="server" CssClass="LabelStyle" Text="
Project Type"></asp:Label>
                        <asp:Label ID="lblDescription5" runat="server" CssClass="MandratoryFieldMarkStyle"
                            Text="*"></asp:Label>
                        <br />
                    </td>
                    <td align="left">
                        <asp:DropDownList ID="drpDocutype" runat="server">
                            <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDescription1" runat="server" CssClass="LabelStyle" Text="Billing Type"></asp:Label>
                        <asp:Label ID="lblDescription6" runat="server" CssClass="MandratoryFieldMarkStyle"
                            Text="*"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="drpBilltype" runat="server">
                            <asp:ListItem Selected="True" Value="0">&lt;Select&gt;</asp:ListItem>
                            <asp:ListItem>Upload</asp:ListItem>
                            <asp:ListItem>Download</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDescription2" runat="server" CssClass="LabelStyle" Text="Charge Per Unit"></asp:Label>
                        <asp:Label ID="lblDescription7" runat="server" CssClass="MandratoryFieldMarkStyle"
                            Text="*"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtCharge" runat="server" OnKeyPress="return checkkey(event)" MaxLength="6"></asp:TextBox>
                        &nbsp;&nbsp;
                        <asp:Label CssClass="CurrentPath" ID="lblPageDescription0" runat="server" Text="Enter Charge per unit in decimal value"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDescription3" runat="server" CssClass="LabelStyle" Text="Currency "></asp:Label>
                        <asp:Label ID="lblDescription8" runat="server" CssClass="MandratoryFieldMarkStyle"
                            Text="*"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="drpCurrency" runat="server">
                            <asp:ListItem Selected="True" Value="0">&lt;select&gt;</asp:ListItem>
                            <asp:ListItem Value="INR">INR</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <div>
                <div class="DivRelative">
                    <br />
                    <div class="LabelDiv">
                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                        <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                        <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                    </div>
                    <div class="LabelDiv">
                        <table>
                            <tr>
                                <td>
                                    <asp:Button ID="btnserachagain" runat="server" Text="Search Again" CssClass="btnsearchagain"
                                        TagName="Add" OnClick="btnserachagain_Click" />
                                </td>
                                <td>
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" TagName="Add" CssClass="btnsave"
                                        OnClick="btnSubmit_Click1" />
                                </td>
                                <td>
                                    <asp:Button ID="btnCancel" runat="server" TagName="Add" Text="Cancel" CssClass="btncancel"
                                        OnClick="btnCancel_Click" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <br />
            </div>
        </fieldset>
    </div>
</asp:Content>
