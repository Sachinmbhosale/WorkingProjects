<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageStageUsers.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageStageUsers"
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
                <asp:Label ID="lblCurrentStageNameHeader" runat="server" Text="Stage Name: " />
                <asp:Label ID="lblCurrentStageNameHeaderValue" runat="server" Text="-" />
            </h3>
        </div>
        <div>
            <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
        </div>
        <div id="accordionUserMapping">
            <%--            <asp:Label ID="lblHdrProcessOwnerMapping" runat="server" Text="Process Owner Mapping"
                meta:resourcekey="lblHdrProcessOwnerMapping" />
            <div id="divProcessOwner">
                <table>
                    <tr>
                        <td colspan="5">
                            <div class="InfoDisplay">
                                <asp:Label ID="lblProcessOwner_CurrentProcess" runat="server" Text="Current Process&nbsp;&nbsp;&nbsp;&nbsp;: "
                                    meta:resourcekey="lblProcessOwner_CurrentProcess" />
                                <asp:Label ID="lblProcessOwner_CurrentProcessValue" runat="server" Text="----" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <asp:Label ID="lblAvailableUsers_ProcessOwner" runat="server" Text="Available Users"
                                    meta:resourcekey="lblAvailableUsers_ProcessOwner" />
                                <asp:ListBox ID="lstAvailableUsers_ProcessOwner" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                        <td>
                            <br />
                            <br />
                            <asp:Button ID="btnAddUser_ProcessOwner" runat="server" Text=" > " OnClick="btnAddUser_ProcessOwner_Click"
                                meta:resourcekey="btnAddUser_ProcessOwner" />
                            <br />
                            <br />
                            <br />
                            <br />
                            <br />
                            <asp:Button ID="btnRemoveUser_ProcessOwner" runat="server" Text=" < " OnClick="btnRemoveUser_ProcessOwner_Click"
                                meta:resourcekey="btnRemoveUser_ProcessOwner" />
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <div>
                                <asp:Label ID="lblAssignedUsers_ProcessOwner" runat="server" Text="Assigned Process Owners"
                                    meta:resourcekey="lblAssignedUsers_ProcessOwner" />
                                <asp:ListBox ID="lstAssignedUsers_ProcessOwner" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
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
                                    meta:resourcekey="lblAvailableUsers_WorkflowOwner" />
                                <asp:ListBox ID="lstAvailableUsers_WorkflowOwner" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                        <td>
                            <br />
                            <br />
                            <asp:Button ID="btnAddUser_WorkflowOwner" runat="server" Text=" > " OnClick="btnAddUser_WorkflowOwner_Click"
                                meta:resourcekey="btnAddUser_WorkflowOwner" />
                            <br />
                            <br />
                            <br />
                            <br />
                            <br />
                            <asp:Button ID="btnRemoveUser_WorkflowOwner" runat="server" Text=" < " OnClick="btnRemoveUser_WorkflowOwner_Click"
                                meta:resourcekey="btnRemoveUser_WorkflowOwner" />
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <div>
                                <asp:Label ID="lblAssignedUsers_WorkflowOwner" runat="server" Text="Assigned Workflow Owners"
                                    meta:resourcekey="lblAssignedUsers_WorkflowOwner" />
                                <asp:ListBox ID="lstAssignedUsers_WorkflowOwner" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>--%>
            <asp:Label ID="lblHdrStageOwnerMapping" runat="server" Text="Stage Owner Mapping"
                meta:resourcekey="lblHdrStageOwnerMapping" />
            <div id="divStageOwner">
                <table>
                    <tr>
                        <td colspan="5">
                            <div class="InfoDisplay">
                                <asp:Label ID="lblStageOwner_CurrentProcess" runat="server" Text="Current Process&nbsp;&nbsp;&nbsp;&nbsp;: "
                                    meta:resourcekey="lblStageOwner_CurrentProcess" />
                                <asp:Label ID="lblStageOwner_CurrentProcessValue" runat="server" Text="----" />
                            </div>
                            <div class="InfoDisplay">
                                <asp:Label ID="lblStageOwner_CurrentWorkFlow" runat="server" Text="Current WorkFlow&nbsp;: "
                                    meta:resourcekey="lblStageOwner_CurrentWorkFlow" />
                                <asp:Label ID="lblStageOwner_CurrentWorkFlowValue" runat="server" Text="----" />
                            </div>
                            <div class="InfoDisplay">
                                <asp:Label ID="lblStageOwner_CurrentStage" runat="server" Text="Current Stage&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;: "
                                    meta:resourcekey="lblStageOwner_CurrentStage" />
                                <asp:Label ID="lblStageOwner_CurrentStageValue" runat="server" Text="----" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <asp:Label ID="lblAvailableUsers_StageOwner" runat="server" Text="Available Users"
                                    meta:resourcekey="lblAvailableUsers_StageOwner" /><br />
                                <asp:ListBox ID="lstAvailableUsers_StageOwner" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <br />
                            <asp:Button ID="btnAddUser_StageOwner"  TagName="Add"  runat="server" Text="" OnClick="btnAddUser_StageOwner_Click"
                                meta:resourcekey="btnAddUser_StageOwner" CssClass="btnrightarrow" />
                            <br />
                            <br />
                            <br />
                            <br />
                            <asp:Button ID="btnRemoveUser_StageOwner"  TagName="Add"  runat="server" Text="" OnClick="btnRemoveUser_StageOwner_Click"
                                meta:resourcekey="btnRemoveUser_StageOwner" CssClass="btnleftarrow" />
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <div>
                                <asp:Label ID="lblAssignedUsers_StageOwner" runat="server" Text="Assigned Stage Owners"
                                    meta:resourcekey="lblAssignedUsers_StageOwner" /><br />
                                <asp:ListBox ID="lstAssignedUsers_StageOwner" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Label ID="lblHdrStageUserMapping" runat="server" Text="Stage User Mapping" meta:resourcekey="lblHdrStageUserMapping" />
            <div id="divUserMain">
                <div id="accordionStageUserMapping">
                    <asp:Label ID="lblHdrStageUserMapping_Sub_User" runat="server" Text="Stage Users"
                        meta:resourcekey="lblHdrStageUserMapping_Sub_User" />
                    <div id="divUserSub">
                        <table>
                            <tr>
                                <td colspan="5">
                                    <div class="InfoDisplay">
                                        <asp:Label ID="lblStageUser_CurrentProcess" runat="server" Text="Current Process&nbsp;&nbsp;&nbsp;&nbsp;: "
                                            meta:resourcekey="lblStageUser_CurrentProcess" />
                                        <asp:Label ID="lblStageUser_CurrentProcessValue" runat="server" Text="----" />
                                    </div>
                                    <div class="InfoDisplay">
                                        <asp:Label ID="lblStageUser_CurrentWorkFlow" runat="server" Text="Current WorkFlow&nbsp;: "
                                            meta:resourcekey="lblStageUser_CurrentWorkFlow" />
                                        <asp:Label ID="lblStageUser_CurrentWorkFlowValue" runat="server" Text="----" />
                                    </div>
                                    <div class="InfoDisplay">
                                        <asp:Label ID="lblStageUser_CurrentStage" runat="server" Text="Current Stage&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;: "
                                            meta:resourcekey="lblStageUser_CurrentStage" />
                                        <asp:Label ID="lblStageUser_CurrentStageValue" runat="server" Text="----" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div>
                                        <asp:Label ID="lblAvailableUsers_StageUser" runat="server" Text="Available Users"
                                            meta:resourcekey="lblAvailableUsers_StageUser" />
                                        <br />
                                        <asp:ListBox ID="lstAvailableUsers_StageUser" runat="server" Height="250" Width="400">
                                        </asp:ListBox>
                                    </div>
                                </td>
                                <td>
                                    &nbsp;&nbsp;&nbsp;
                                </td>
                                <td>
                                    <br />
                                    <asp:Button ID="btnAddUser_StageUser"  TagName="Add"  runat="server" Text="" OnClick="btnAddUser_StageUser_Click"
                                        meta:resourcekey="btnAddUser_StageUser" CssClass="btnrightarrow" />
                                    <br />
                                    <br />
                                    <br />
                                    <br />
                                    <asp:Button ID="btnRemoveUser_StageUser"  TagName="Add" runat="server" Text="" OnClick="btnRemoveUser_StageUser_Click"
                                        meta:resourcekey="btnRemoveUser_StageUser" CssClass="btnleftarrow" />
                                </td>
                                <td>
                                    &nbsp;&nbsp;&nbsp;
                                </td>
                                <td>
                                    <div>
                                        <asp:Label ID="lblAssignedUsers_StageUser" runat="server" Text="Assigned Stage Users"
                                            meta:resourcekey="lblAssignedUsers_StageUser" />
                                        <br />
                                        <asp:ListBox ID="lstAssignedUsers_StageUser" runat="server" Height="250" Width="400">
                                        </asp:ListBox>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Label ID="lblHdrStageUserMapping_Sub_Group" runat="server" Text="User Groups"
                        meta:resourcekey="lblHdrStageUserMapping_Sub_Group" />
                    <div id="divUserGroupSub">
                        <table>
                            <tr>
                                <td colspan="5">
                                    <div class="InfoDisplay">
                                        <asp:Label ID="lblStageUserGroup_CurrentProcess" runat="server" Text="Current Process&nbsp;&nbsp;&nbsp;&nbsp;: "
                                            meta:resourcekey="lblStageUserGroup_CurrentProcess" />
                                        <asp:Label ID="lblStageUserGroup_CurrentProcessValue" runat="server" Text="----" />
                                    </div>
                                    <div class="InfoDisplay">
                                        <asp:Label ID="lblStageUserGroup_CurrentWorkFlow" runat="server" Text="Current WorkFlow&nbsp;: "
                                            meta:resourcekey="lblStageUserGroup_CurrentWorkFlow" />
                                        <asp:Label ID="lblStageUserGroup_CurrentWorkFlowValue" runat="server" Text="----" />
                                    </div>
                                    <div class="InfoDisplay">
                                        <asp:Label ID="lblStageUserGroup_CurrentStage" runat="server" Text="Current Stage&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;: "
                                            meta:resourcekey="lblStageUserGroup_CurrentStage" />
                                        <asp:Label ID="lblStageUserGroup_CurrentStageValue" runat="server" Text="----" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div>
                                        <asp:Label ID="lblAvailableUsers_StageUserGroup" runat="server" Text="Available UserGroups"
                                            meta:resourcekey="lblAvailableUsers_StageUserGroup" />
                                        <br />
                                        <asp:ListBox ID="lstAvailableUsers_StageUserGroup" runat="server" Height="250" Width="400">
                                        </asp:ListBox>
                                    </div>
                                </td>
                                <td>
                                    &nbsp;&nbsp;&nbsp;
                                </td>
                                <td>
                                    <br />
                                    <asp:Button ID="btnAddUser_StageUserGroup"  TagName="Add"  runat="server" Text="" OnClick="btnAddUser_StageUserGroup_Click"
                                        meta:resourcekey="btnAddUser_StageUserGroup" CssClass="btnrightarrow" />
                                    <br />
                                    <br />
                                    <br />
                                    <br />
                                    <asp:Button ID="btnRemoveUser_StageUserGroup"  TagName="Add"  runat="server" Text="" OnClick="btnRemoveUser_StageUserGroup_Click"
                                        meta:resourcekey="btnRemoveUser_StageUserGroup" CssClass="btnleftarrow" />
                                </td>
                                <td>
                                    &nbsp;&nbsp;&nbsp;
                                </td>
                                <td>
                                    <div>
                                        <asp:Label ID="lblAssignedUsers_StageUserGroup" runat="server" Text="Assigned Stage UserGroups"
                                            meta:resourcekey="lblAssignedUsers_StageUserGroup" />
                                        <br />
                                        <asp:ListBox ID="lstAssignedUsers_StageUserGroup" runat="server" Height="250" Width="400">
                                        </asp:ListBox>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <asp:Label ID="lblHdrNotificationUserMapping" runat="server" Text="Notification User Mapping"
                meta:resourcekey="lblHdrNotificationUserMapping" />
            <div id="divNotification">
                <table>
                    <tr>
                        <td colspan="5">
                            <div class="InfoDisplay">
                                <asp:Label ID="lblNotificationUser_CurrentProcess" runat="server" Text="Current Process&nbsp;&nbsp;&nbsp;&nbsp;: "
                                    meta:resourcekey="lblNotificationUser_CurrentProcess" />
                                <asp:Label ID="lblNotificationUser_CurrentProcessValue" runat="server" Text="----" />
                            </div>
                            <div class="InfoDisplay">
                                <asp:Label ID="lblNotificationUser_CurrentWorkFlow" runat="server" Text="Current WorkFlow&nbsp;: "
                                    meta:resourcekey="lblNotificationUser_CurrentWorkFlow" />
                                <asp:Label ID="lblNotificationUser_CurrentWorkFlowValue" runat="server" Text="----" />
                            </div>
                            <div class="InfoDisplay">
                                <asp:Label ID="lblNotificationUser_CurrentStage" runat="server" Text="Current Stage&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;: "
                                    meta:resourcekey="lblNotificationUser_CurrentStage" />
                                <asp:Label ID="lblNotificationUser_CurrentStageValue" runat="server" Text="----" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <asp:Label ID="lblAvailableUsers_NotoficationUser" runat="server" Text="Available Users"
                                    meta:resourcekey="lblAvailableUsers_NotoficationUser" />
                                <br />
                                <asp:ListBox ID="lstAvailableUsers_NotoficationUser" runat="server" Height="250"
                                    Width="400"></asp:ListBox>
                            </div>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <br />
                            <asp:Button ID="btnAddUser_NotoficationUser"  TagName="Add"  runat="server" Text="" OnClick="btnAddUser_NotoficationUser_Click"
                                meta:resourcekey="btnAddUser_NotoficationUser" CssClass="btnrightarrow" />
                            <br />
                            <br />
                            <br />
                            <br />
                            <asp:Button ID="btnRemoveUser_NotoficationUser"  TagName="Add"  runat="server" Text="" OnClick="btnRemoveUser_NotoficationUser_Click"
                                meta:resourcekey="btnRemoveUser_NotoficationUser" CssClass="btnleftarrow" />
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <div>
                                <asp:Label ID="lblAssignedUsers_NotoficationUser" runat="server" Text="Assigned Notification Users"
                                    meta:resourcekey="lblAssignedUsers_NotoficationUser" /><br />
                                <asp:ListBox ID="lstAssignedUsers_NotoficationUser" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <br />
        <asp:Button ID="btnGoBacktoStage" runat="server" TagName="Read" Text="Go Back To Stages" meta:resourcekey="btnGoBacktoStage"
            OnClick="btnGoBacktoStage_Click" CssClass="btngobackto" />
    </div>
    <br />
    <asp:HiddenField ID="hdnCurrentPanel" runat="server" Value="0" />
    <asp:HiddenField ID="hdnCurrentSubPanel" runat="server" Value="0" />
</asp:Content>
