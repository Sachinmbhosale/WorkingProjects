<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master" AutoEventWireup="true" CodeBehind="Workflow_ContractSearch.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.Workflow_ContractSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label1" Font-Bold="true" runat="server" Text="Tender Reference No"></asp:Label>&</td>
                <td>
                    <asp:TextBox ID="txtTenderRefId" runat="server"></asp:TextBox>&nbsp </td>
                <td>
                    <asp:Label ID="lblInstitutionname" Font-Bold="true" runat="server" Text="Institution Name"></asp:Label></td>
                <td>
                    <asp:TextBox ID="TxtInstitutionname" Font-Bold="true" runat="server"></asp:TextBox>&nbsp</td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />

                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="5">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" AllowPaging="true"
                        PageSize="5" Style1="padding-left: 10px" CssClass="mGrid" Width="100%">
                    </asp:GridView>
                </td>
            </tr>
            <tr>
<%--                <td colspan="3">
                    <asp:UpdatePanel ID="UpdatePanel7" runat="server" RenderMode="Inline">
                        <ContentTemplate>
                            <h3>PO Document : 
                            <asp:Label ID="lblPodoc" runat="server"></asp:Label></h3>

                            <iframe id="Iframe1" runat="server" width="100%" class="frm-iframe"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td colspan="3">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" RenderMode="Inline">
                        <ContentTemplate>
                            <span>
                                <h3>Tender Document :
                                <asp:Label ID="lbltenderdoc" runat="server" ForeColor="#6600FF"></asp:Label></h3>
                            </span>
                            <iframe id="myiframe" runat="server" width="100%" class="frm-iframe"></iframe>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </td>--%>

            </tr>
        </table>
        <asp:HiddenField ID="HidDatafieldID" runat="server" />
    </div>
</asp:Content>
