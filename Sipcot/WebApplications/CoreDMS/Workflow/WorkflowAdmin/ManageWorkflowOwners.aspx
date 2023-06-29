<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageWorkflowOwners.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageWorkflowOwners"
    Culture="auto" meta:resourcekey="ManageStageUsers" UICulture="auto" EnableEventValidation="false" %>

<%@ Register TagPrefix="WF" TagName="WorkFlowWizard" Src="WorkFlowWizardMenu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="../css/accordionStyles.css" type="text/css" />
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script src="../../Scripts/accordionScripts.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {

            var icons = {
                header: "ui-icon-circle-arrow-e",
                activeHeader: "ui-icon-circle-arrow-s"
            };

            $("#accordionUserMapping").accordion({
                collapsible: true,
                heightStyle: "content",
                icons: icons,
            });

            $("#toggle").button().click(function () {
                if ($("#accordionUserMapping").accordion("option", "icons")) {
                    $("#accordionUserMapping").accordion("option", "icons", null);
                } else {
                    $("#accordionUserMapping").accordion("option", "icons", icons);
                }
            });

            var hdnCurrentPanel = document.getElementById("<%=hdnCurrentPanel.ClientID %>").value;
            if ( hdnCurrentPanel== null || hdnCurrentPanel =="")
            {
                hdnCurrentPanel="0";
            }

            $( "#accordionUserMapping").accordion( "option", "active", parseInt(hdnCurrentPanel));


        });

        $(function () {
            $("#accordionStageUserMapping").accordion({
                collapsible: true,
                heightStyle: "content"
            });

            var hdnCurrentSubPanel = document.getElementById("<%=hdnCurrentSubPanel.ClientID %>").value;
            if ( hdnCurrentSubPanel== null || hdnCurrentSubPanel =="")
            {
                hdnCurrentSubPanel="0";
            }

            $( "#accordionStageUserMapping").accordion( "option", "active", parseInt(hdnCurrentSubPanel));

        });


        function validateSelectedItem(controlName, errMessage) {
            if (document.getElementById(controlName).value == ""
                 || document.getElementById(controlName).value == undefined) {
                 document.getElementById("<%= lblMessage.ClientID %>").innerHTML = errMessage;
                return false;
            }
            return true;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <WF:WorkFlowWizard ID="WorkFlowWizard1" runat="server" ActiveItemName="Users" />
    <div class="GVDiv">
        <div class="InfoDisplay">
            <h3>
                <asp:Label ID="lblCurrentStageNameHeader" runat="server" Text="Workflow Name: " />
                <asp:Label ID="lblCurrentStageNameHeaderValue" runat="server" Text="-" />
            </h3>
        </div>
        <div>
            <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
        </div>
        <div id="accordionUserMapping">
            <asp:Label ID="lblHdrWorkflowOwnerMapping" runat="server" Text="Workflow Owner Mapping"
                meta:resourcekey="lblHdrWorkflowOwnerMapping" />
            <div id="divWorkflowOwner">
                <table>
                    <tr>
                        <td colspan="5">
                            <div class="InfoDisplay">
                                <asp:Label ID="lblWorkFlowOwner_CurrentProcess" runat="server" Text="Current Process&nbsp;&nbsp;&nbsp;&nbsp;: "
                                    meta:resourcekey="lblWorkFlowOwner_CurrentProcess" />
                                <asp:Label ID="lblWorkFlowOwner_CurrentProcessValue" runat="server" Text="----" />
                            </div>
                            <div class="InfoDisplay">
                                <asp:Label ID="lblWorkFlowOwner_CurrentWorkFlow" runat="server" Text="Current WorkFlow&nbsp;: "
                                    meta:resourcekey="lblWorkFlowOwner_CurrentWorkFlow" />
                                <asp:Label ID="lblWorkFlowOwner_CurrentWorkFlowValue" runat="server" Text="----" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <asp:Label ID="lblAvailableUsers_WorkflowOwner" runat="server" Text="Available Users"
                                    meta:resourcekey="lblAvailableUsers_WorkflowOwner" /><br />
                                <asp:ListBox ID="lstAvailableUsers_WorkflowOwner" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <br />
                            <asp:Button ID="btnAddUser_WorkflowOwner"  TagName="Add"  runat="server" Text="" OnClick="btnAddUser_WorkflowOwner_Click"
                                meta:resourcekey="btnAddUser_WorkflowOwner" CssClass="btnrightarrow"/>
                            <br />
                            <br />
                            <br />
                            <br />
                            <asp:Button ID="btnRemoveUser_WorkflowOwner"  TagName="Add"  runat="server" Text="" OnClick="btnRemoveUser_WorkflowOwner_Click"
                                meta:resourcekey="btnRemoveUser_WorkflowOwner" CssClass="btnleftarrow"/>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <div>
                                <asp:Label ID="lblAssignedUsers_WorkflowOwner" runat="server" Text="Assigned Workflow Owners"
                                    meta:resourcekey="lblAssignedUsers_WorkflowOwner" /><br />
                                <asp:ListBox ID="lstAssignedUsers_WorkflowOwner" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <br />
        <asp:Button ID="btnGoBacktoWorkflows" runat="server" Text="Go Back To Workflow"
            meta:resourcekey="btnGoBacktoWorkflows" OnClick="btnGoBacktoStage_Click" CssClass="btngobackto" TagName="Read"/>
    </div>
    <br />
    <asp:HiddenField ID="hdnCurrentPanel" runat="server" Value="0" />
    <asp:HiddenField ID="hdnCurrentSubPanel" runat="server" Value="0" />
</asp:Content>
