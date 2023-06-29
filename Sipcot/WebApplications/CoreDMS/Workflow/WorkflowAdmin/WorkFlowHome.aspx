<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="WorkFlowHome.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkFlowHome"
    EnableEventValidation="false" Culture="auto" meta:resourcekey="WorkFlowHome"
    UICulture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="../../App_Themes/common/common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/GridRowSelection.js" type="text/javascript"></script>
    <script type="text/javascript">
        function GetWorkFlow(id, ProcessName) {
            document.getElementById("<%= hdnProcessId.ClientID %>").value = id
            document.getElementById("<%= hdnAction.ClientID %>").value = 'GetWorkFlow'
            document.getElementById("<%= hdnProcessName.ClientID %>").value = ProcessName;
            document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();


        }
        function GetStages(id, WorkflowName) {
            document.getElementById("<%= hdnWorkflowId.ClientID %>").value = id
            document.getElementById("<%= hdnAction.ClientID %>").value = 'GetStages'
            document.getElementById("<%= hdnWorkFlowName.ClientID %>").value = WorkflowName;
            document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();


        }
        function GetStatus(id, StageName) {
            document.getElementById("<%= hdnStageId.ClientID %>").value = id
            document.getElementById("<%= hdnAction.ClientID %>").value = 'GetStatus'
            document.getElementById("<%= hdnStageName.ClientID %>").value = StageName;
            document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();


        }
        function GetProcess(id, StageName) {

            document.getElementById("<%= hdnAction.ClientID %>").value = 'GetProcess'

            document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();


        }
        function GetDataEntry(DataId, DataEntryMode, ProcessId, WorkFlowID, StageId) {
            document.getElementById("<%= hdnDataEntryType.ClientID %>").value = DataEntryMode;
            document.getElementById("<%= hdnDataId.ClientID %>").value = DataId;
            document.getElementById("<%= hdnWorkflowId.ClientID %>").value = WorkFlowID;
            document.getElementById("<%= hdnProcessId.ClientID %>").value = ProcessId;

            document.getElementById("<%= hdnStageId.ClientID %>").value = StageId;

            document.getElementById("<%= hdnAction.ClientID %>").value = 'RedirectToDataEntry'
            document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
        }
    </script>
    <div class="GVDiv" style="min-height: 500px; padding-left:50px; padding-right:50px;">
        <h3>
            <asp:Label ID="lblMessageHeader" runat="server" Text="Welcome to Workflow" meta:resourcekey="lblMessageHeader"></asp:Label></h3>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always" RenderMode="Inline">
            <ContentTemplate>
               
                    <div id="MainPanel" runat="server">
                    </div>
                    <asp:Button ID="btnCallFromJavascript" runat="server" class="HiddenButton" TagName="Read"
                        OnClick="btnCallFromJavascript_Click" />
                
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Always" RenderMode="Inline">
            <ContentTemplate>
                <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                <asp:HiddenField ID="hdnProcessId" runat="server" Value="" />
                <asp:HiddenField ID="hdnWorkflowId" runat="server" Value="" />
                <asp:HiddenField ID="hdnProcessName" runat="server" Value="" />
                <asp:HiddenField ID="hdnWorkFlowName" runat="server" Value="" />
                <asp:HiddenField ID="hdnStageName" runat="server" Value="" />
                <asp:HiddenField ID="hdnStageId" runat="server" Value="" />
                <asp:HiddenField ID="hdnDataId" runat="server" Value="" />
                <asp:HiddenField ID="hdnDataEntryType" runat="server" Value="" />
                <asp:HiddenField ID="hdnAction" runat="server" Value="" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
