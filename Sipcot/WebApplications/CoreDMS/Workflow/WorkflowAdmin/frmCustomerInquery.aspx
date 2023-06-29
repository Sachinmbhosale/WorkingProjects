<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master" AutoEventWireup="true" CodeBehind="frmCustomerInquery.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.frmCustomerInquery" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            height: 20px;
        }

        .auto-style2 {
            height: 43px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:UpdatePanel ID="UpdatePanelMessage" runat="server" UpdateMode="Conditional">
            <ContentTemplate>


                <div class="GVDiv">
                    <fieldset style="border: 1px solid #fdb813; border-radius: 8px; padding: 10px; width: 98% !important;">
                        <legend>Client Details:</legend>
                        <table id="tblproduct" runat="server" style="padding: 10px; width: 98% !important;">
                            <tr>
                                <td>Client Name :</td>
                                <td>
                                    <asp:TextBox ID="txtclientname" runat="server"></asp:TextBox>
                                </td>
                                <td>Place of Origin:</td>
                                <td>
                                    <asp:TextBox ID="txtplaceoforigin" runat="server"></asp:TextBox>
                                </td>

                            </tr>
                            <tr>
                                <td>Destination :</td>
                                <td>
                                    <asp:TextBox ID="txtdestination" runat="server"></asp:TextBox>

                                </td>
                                <td>Date of Survey : </td>
                                <td>
                                    <asp:TextBox ID="txtdateofsurvery" runat="server"></asp:TextBox>

                                    <img alt="Icon" src="../../Images/cal.gif" id="Image2" />
                                    <ajax:CalendarExtender ID="CalendarExtender1" runat="server"
                                        TargetControlID="txtdateofsurvery" PopupButtonID="Image2" Format="MM/dd/yyyy" />
                                </td>
                            </tr>
                            <tr>
                                <td>Estimated Volume :</td>
                                <td>
                                    <asp:TextBox ID="txtestimatedvolume" runat="server"></asp:TextBox></td>
                                <td>Movement Type :</td>
                                <td>
                                    <asp:DropDownList ID="ddlmovementtype" runat="server">
                                        <asp:ListItem Selected="True" Value="0">--Select--</asp:ListItem>
                                        <asp:ListItem>Domestic </asp:ListItem>
                                        <asp:ListItem>International </asp:ListItem>
                                    </asp:DropDownList>
                                </td>

                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>&nbsp;</td>
                                <td colspan="2">
                                    <asp:Button ID="btnsubmit1" runat="server" OnClick="btnsubmit1_Click" Text="Save" />
                                    &nbsp;<asp:Button ID="btnprodOK0" runat="server" CssClass="btn-success" Text="Cancel" />
                                </td>
                            </tr>

                        </table>
                    </fieldset>

                    <fieldset style="border: 1px solid #fdb813; border-radius: 8px; padding: 10px; width: 98% !important;">
                        <legend>Add Product Details:</legend>
                        <table style=" padding: 10px; width: 62% !important; ">
                            <tr>
                                <td class="auto-style1" style="background-color: #C0C0C0">Product </td>
                                <td class="auto-style1" style="background-color: #C0C0C0">Items</td>
                                <td class="auto-style1" style="background-color: #C0C0C0">Volume</td>
                                <td class="auto-style1" style="background-color: #C0C0C0">&nbsp;</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:DropDownList ID="ddlproduct" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlproduct_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlItems" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtvolume" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:LinkButton ID="lnkbtnaddproduct" runat="server" OnClick="lnkbtnaddproduct_Click">AddProduct</asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">
                                    <asp:GridView ID="GrdProductWF3" runat="server" CssClass="mGrid" Style1="padding-left: 10px">
                                    </asp:GridView>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="HidProduct" runat="server" />
                        <asp:HiddenField ID="HidDocid" runat="server" />
                    </fieldset>


                </div>







            </ContentTemplate>
            <Triggers>
                <%--               <asp:AsyncPostBackTrigger ControlID="btnsubmit" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ddlproductcodeMst" EventName="SelectedIndexChanged" />--%>
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>
