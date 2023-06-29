<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageWorkflowDmsFieldsMapping.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageWorkflowDmsFieldsMapping" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <script language="javascript" type="text/javascript">
        function Validate(Name) {
            var selected = true;
            var msgControl = "#<%= divMsg.ClientID %>";
            var btnMapId = document.getElementById("<%= btnmap.ClientID %>");
            var btnRemove = document.getElementById("<%= btnremove.ClientID %>");
            var btncommit = document.getElementById("<%= btncommit.ClientID %>");
            if (Name == btnMapId.id) {
                var ControlListBox = document.getElementById('<%= lstFieldsFromDMS.ClientID %>');
                for (var i = 0; i < ControlListBox.options.length; i++) {
                    if (ControlListBox.options[i].selected) {
                        selected = false;
                        break;
                    } 
                }
                if (selected) {
                    $(msgControl).html("Please select dms field.");
                    return false;
                }
                selected = true;
                var FieldListBox = document.getElementById('<%= lstFieldsFromWorkflow.ClientID %>');
                for (var i = 0; i < FieldListBox.options.length; i++) {
                    if (FieldListBox.options[i].selected) {
                        selected = false;
                        break;
                    }
                }
                if (selected) {
                    $(msgControl).html("Please select a workflow field.");
                    return false;
                }

            }
            else if (Name == btnRemove.id) {
                selected = true;
                var MappedListBox = document.getElementById('<%= lstMappedcontrolfields.ClientID %>');
                for (var i = 0; i < MappedListBox.options.length; i++) {
                    if (MappedListBox.options[i].selected) {
                        selected = false;
                        break;
                    }
                }
                if (selected) {
                    $(msgControl).html("Please select a item to remove from mapped controls fields.");
                    return false;

                }
            }
            else if (Name == btncommit.id) {

                var MappedListBox = document.getElementById('<%= lstMappedcontrolfields.ClientID %>');
                if (MappedListBox.options.length <= 0) {
                    $(msgControl).html("Kindly add values before commit");
                    return false;
                }
            }

        }
        </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div id="divMsg" runat="server" style="color: Red; font-family: Calibri; font-size: small">
                </div>
            <div class="GVDiv">
                <table class="Manageworkflowtable">

                    <tr>
                        <td>
                            <asp:Label ID="lblProjectType" runat="server" Text="ProjectType" CssClass="ManageworkflowLabels"></asp:Label><span
                            style="color: Red; font-size: medium">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlprojecttype" runat="server"  AutoPostBack="true"
                                onselectedindexchanged="ddlprojecttype_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lbldepartment" runat="server" Text="Department" CssClass="ManageworkflowLabels"></asp:Label><span
                            style="color: Red; font-size: medium">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddldepartment" runat="server" AutoPostBack="true"
                                onselectedindexchanged="ddldepartment_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblFieldsFromDMS" runat="server" Text="Fields from DMS" CssClass="ManageworkflowLabels"></asp:Label>
                        </td>
                        <td>
                        </td>
                        <td>
                            <asp:Label ID="lblFieldsFromStages" runat="server" Text="Fields from Workflow" CssClass="ManageworkflowLabels"></asp:Label>
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                        <td>
                            <asp:Label ID="lblFMappedControlsFields" runat="server" Text="Mapped Controls Fields"
                                CssClass="ManageworkflowLabels"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="td">
                            <asp:ListBox ID="lstFieldsFromDMS" runat="server" 
                                CssClass="Manageworkflowlistbox" 
                              >
                            </asp:ListBox>
                        </td>
                        <td class="td">
                        </td>
                        <td class="td">
                            <asp:ListBox ID="lstFieldsFromWorkflow" runat="server" 
                                CssClass="Manageworkflowlistbox"
                                >
                            </asp:ListBox>
                        </td>
                        <td class="td">
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnmap" runat="server" Text="Map Fields"  CssClass="btnadd"
                                            Width="100px" OnClientClick="return Validate(this.id);"  OnClick="btnmap_Click"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnremove" runat="server" Text="Remove" 
                                            CssClass="btnremovesearchcondition" Width="100px" 
                                            OnClientClick="return Validate(this.id);" onclick="btnremove_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btncommit" runat="server" Text="Commit"  Width="100px"
                                            CssClass="btncommit" OnClientClick="return Validate(this.id);" 
                                            onclick="btncommit_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnGotoWorkflow" runat="server" Text="Bulk Upload" Width="100px" 
                                            meta:resourcekey="btnGotoWorkflow" CssClass="btngotoworkflow" 
                                            onclick="btnGotoWorkflow_Click" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnCallFromJavascript" class="HiddenButton" runat="server" TagName="Read"
                                             />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="td">
                            <asp:ListBox ID="lstMappedcontrolfields" runat="server" CssClass="Manageworkflowlcontrollistbox">
                            </asp:ListBox>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                <asp:HiddenField ID="hdnProcessId" runat="server" Value="" />
                <asp:HiddenField ID="hdnWorkflowId" runat="server" Value="" />
                 <asp:HiddenField ID="hdnStageId" runat="server" Value="" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
