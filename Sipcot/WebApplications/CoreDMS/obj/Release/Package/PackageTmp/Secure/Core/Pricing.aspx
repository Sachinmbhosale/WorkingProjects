<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="Pricing.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Pricing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        function validate() {
            var msgControl = "#<%= divMsg.ClientID %>";
            var Customer = document.getElementById('<%=cmbCustomer.ClientID %>');
            var DocuType = document.getElementById('<%=cmbDocumentType.ClientID %>');
            var Billtype = document.getElementById('<%=cmbBillingType.ClientID %>');
            if (Customer.value == 0) {
                $(msgControl).html("Please Select Customer.");
                return false;
            }
            else if (DocuType.value == 0) {
                $(msgControl).html("Please Select Project Type.");
                return false;
            }
            else if (Billtype.value == 0) {
                $(msgControl).html("Please Select Bill Type.");
                return false;
            }
            return true;
        }
        function ConfirmationBox(Pricing_ID) {
            var result = confirm('Are you sure you want to delete Record No : ' + Pricing_ID + ' Details');
            if (result) {
                return true;
            }
            else {
                return false;
            }
        }
    
    </script>
    <div class="DivPageHeader">
        <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Search Pricing Master
"></asp:Label>
    </div>
    <div class="GVDiv">
        <asp:Label ID="lblPageHeader" runat="server" CssClass="PageHeadings" Text="Pricing Master
"></asp:Label>
        <fieldset>
            <table>
                <asp:Label ID="lblSubHeading" CssClass="SubHeadings" runat="server" Text="Search Filters"></asp:Label>
                <tr>
                    <td>
                        <div id="divMsg" runat="server" style="color: Red; font-family: Calibri; font-size: small;">
                        </div>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDepartmentName" runat="server" CssClass="LabelStyle" Text="Customer"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbCustomer" runat="server" Width="190px" AutoPostBack="True"
                            OnSelectedIndexChanged="cmbCustomer_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="Project Type
"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbDocumentType" runat="server" Width="190px">
                            <asp:ListItem Selected="True" Value="0">&lt;Select&gt;</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" CssClass="LabelStyle" Text="Billing Type
"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="cmbBillingType" runat="server" Width="190px">
                            <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                            <asp:ListItem>Upload</asp:ListItem>
                            <asp:ListItem>Download</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click"
                            CssClass="btnsearch" TagName="Read" />
                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                        <asp:HiddenField ID="HiddenField1" runat="server" Value="" />
                        <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                    </td>
                </tr>
                
            </table>
            <table class="GridWidth"><tr>
                    <td>
                    </td>
                    <td>
                        <asp:GridView ID="Grid_result" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                            PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" CellPadding="10"
                            CellSpacing="5" OnRowDataBound="Grid_result_RowDataBound">
                            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                                PageButtonCount="5" PreviousPageText=" " />
                            <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                            <Columns>
                                <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="Imbtn_Edit" runat="server" ImageUrl="~/Images/Edit.jpg" OnClick="Imbtn_Edit_Click"
                                            Style="height: 16px" TagName="Edit" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="Imbtn_Delete" runat="server" ImageUrl="~/Images/Delete.jpg"
                                            OnClick="Imbtn_Delete_Click" Style="width: 16px" TagName="Delete" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Serial No">
                                    <ItemTemplate>
                                        <asp:Label ID="Lbl_Slno" runat="server" Text='<%# Eval("Pricing_iID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Customer">
                                    <ItemTemplate>
                                        <asp:Label ID="Lbl_Customer" runat="server" Text='<%# Eval("Pricing_vCustomer") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Project Type">
                                    <ItemTemplate>
                                        <asp:Label ID="Lbl_DocuType" runat="server" Text='<%# Eval("Pricing_vDocument") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Billing Type">
                                    <ItemTemplate>
                                        <asp:Label ID="Lbl_BillType" runat="server" Text='<%# Eval("Pricing_vBilling") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Charge">
                                    <ItemTemplate>
                                        <asp:Label ID="Lbl_Charge" runat="server" Text='<%# Eval("Pricing_dCharges") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Currency">
                                    <ItemTemplate>
                                        <asp:Label ID="Lbl_Currency" runat="server" Text='<%# Eval("Pricing_vCurrency") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr></table>
        </fieldset>
    </div>
</asp:Content>
