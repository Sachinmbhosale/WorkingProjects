<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageProcessOwners.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageProcessOwners"
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
                <asp:Label ID="lblCurrentStageNameHeader" runat="server" Text="Process Name: " />
                <asp:Label ID="lblCurrentStageNameHeaderValue" runat="server" Text="-" />
            </h3>
        </div>
        <div>
        <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
        </div>
        <div id="accordionUserMapping">
            <asp:Label ID="lblHdrProcessOwnerMapping" runat="server" Text="Process Owner Mapping"
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
                                    meta:resourcekey="lblAvailableUsers_ProcessOwner" /><br/>
                                <asp:ListBox ID="lstAvailableUsers_ProcessOwner" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                           <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <br />
                            <asp:Button ID="btnAddUser_ProcessOwner" runat="server" Text="" OnClick="btnAddUser_ProcessOwner_Click"  TagName="Add" 
                                meta:resourcekey="btnAddUser_ProcessOwner" CssClass="btnrightarrow"/>
                            <br />
                            <br />
                            <br />
                            <br />
                           
                            <asp:Button ID="btnRemoveUser_ProcessOwner" runat="server" Text="" OnClick="btnRemoveUser_ProcessOwner_Click"  TagName="Add" 
                                meta:resourcekey="btnRemoveUser_ProcessOwner" CssClass="btnleftarrow"/>
                        </td>
                        <td>
                            &nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <div>
                                <asp:Label ID="lblAssignedUsers_ProcessOwner" runat="server" Text="Assigned Process Owners"
                                    meta:resourcekey="lblAssignedUsers_ProcessOwner" /><br/>
                                <asp:ListBox ID="lstAssignedUsers_ProcessOwner" runat="server" Height="250" Width="400">
                                </asp:ListBox>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <br />
        <asp:Button ID="btnGoBacktoProcess" runat="server" Text="Go Back To Process" meta:resourcekey="btnGoBacktoProcess" TagName="Read"
        OnClick="btnGoBacktoStage_Click" CssClass="btngobackto"/>
    </div>
    <br />
    <asp:HiddenField ID="hdnCurrentPanel" runat="server" Value="0" />
    <asp:HiddenField ID="hdnCurrentSubPanel" runat="server" Value="0" />
    
</asp:Content>
