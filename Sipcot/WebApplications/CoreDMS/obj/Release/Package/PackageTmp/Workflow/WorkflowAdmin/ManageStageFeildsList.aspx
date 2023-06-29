<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageStageFeildsList.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageStageFeildsList" %>

<%@ Register TagPrefix="WF" TagName="WorkFlowWizard" Src="WorkFlowWizardMenu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" language="javascript">

        function MoveUpSelectedRow(lnk) {
            var row = lnk.parentNode.parentNode;
            var StageFeildId = row.cells[4].innerHTML; //StageId of particular row from StageList Gridview
            var StageFeildOrder = row.cells[14].innerHTML; //sort order
            document.getElementById("<%= hdnStageFeildId.ClientID %>").value = StageFeildId;
            document.getElementById("<%= hdnStageFeildOrder.ClientID %>").value = parseInt(StageFeildOrder) - 1;

            __doPostBack("<%= btnMoveUp.UniqueID %>", "OnClick");
        }

        function MoveDownSelectedRow(lnk) {
            var row = lnk.parentNode.parentNode;
            var StageFeildId = row.cells[4].innerHTML; //StageId of particular row from StageList Gridview
            var StageFeildOrder = row.cells[14].innerHTML; //sort order
            document.getElementById("<%= hdnStageFeildId.ClientID %>").value = StageFeildId;
            document.getElementById("<%= hdnStageFeildOrder.ClientID %>").value = parseInt(StageFeildOrder) + 1;

            __doPostBack("<%= btnMoveDown.UniqueID %>", "OnClick");
        }

    </script>
    <script type="text/javascript" language="javascript">
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <WF:WorkFlowWizard ID="WorkFlowWizard1" runat="server" ActiveItemName="Fields" />
    <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="GVDiv">
                <div class="InfoDisplay">
                    <h3>
                        <asp:Label ID="lblCurrentStageNameHeader" runat="server" Text="Stage Name: " />
                        <asp:Label ID="lblCurrentStageNameHeaderValue" runat="server" Text="-" />
                    </h3>
                </div>
                <asp:Label ID="lblMessage" ForeColor="Red" runat="server" Text=""></asp:Label>
                <h4>
                    <asp:Label ID="lblInheritedFieldHeader" runat="server" Text="Inherited field details" /></h4>
                <asp:GridView ID="gridInheritedFields" runat="server" AutoGenerateColumns="true"
                    CssClass="mGrid" PagerStyle-CssClass="pgr" OnRowDataBound="gridInheritedFields_RowDataBound"
                    AlternatingRowStyle-CssClass="alt" EmptyDataText="No inherited fields are available"
                    OnPageIndexChanging="gridInheritedFields_PageIndexChanging" AllowPaging="true"
                    PageSize="10">
                    <Columns>
                        <asp:TemplateField HeaderText="Edit">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkView" runat="server" Text="" ToolTip="View" CommandArgument='' TagName="Read"><img src="../images/viewicon.png"/></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                        PageButtonCount="5" PreviousPageText=" " />
                    <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                </asp:GridView>
                <h4>
                    <asp:Label ID="lblEditableFields" runat="server" Text="Editable field details" /></h4>
                <asp:GridView ID="gridStageList" runat="server" AutoGenerateColumns="true" CssClass="mGrid"
                    PagerStyle-CssClass="pgr" OnPageIndexChanging="gridStageList_PageIndexChanging"
                    AllowPaging="true" PageSize="10" AlternatingRowStyle-CssClass="alt" EmptyDataText="No fields are available"
                    OnRowDataBound="gridStageList_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Edit">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" Text="" ToolTip="Edit" TagName="Read" CommandArgument=''><img src="../images/Edit.png"/></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sort">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkMoveUp" runat="server" Text="" ToolTip="Move Up" CommandArgument='' TagName="Read"
                                    OnClientClick="return MoveUpSelectedRow(this)"><img src="../images/moveup.png"/></asp:LinkButton>
                                <asp:LinkButton ID="lnkMoveDown" runat="server" Text="" ToolTip="Move Down" CommandArgument='' TagName="Read"
                                    OnClientClick="return MoveDownSelectedRow(this)"><img src="../images/movedown.png"/></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                        PageButtonCount="5" PreviousPageText=" " />
                    <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                </asp:GridView>
                <asp:HiddenField ID="hdnSaveStatus" runat="server" />
                <asp:HiddenField ID="hdnStageFeildId" runat="server" />
                <asp:HiddenField ID="hdnStageFeildOrder" runat="server" />
                <asp:HiddenField ID="hdnStageId" runat="server" />
                <asp:Button ID="btnMoveUp" runat="server" Text="" Visible="false" OnClick="btnMoveUp_Click" TagName="Read"/>
                <asp:Button ID="btnMoveDown" runat="server" Text="" Visible="false" OnClick="btnMoveDown_Click" TagName="Read"/>
                <asp:Button ID="btnGotoStageFeilds" runat="server" Text="Add Stage Fields" OnClick="btnGotoStageFeilds_Click"
                    CausesValidation="false" CssClass="btnaddstagefields" TagName="Read" />
                <asp:Button ID="btnGoBacktoStage" runat="server" Text="Go Back To Stages" meta:resourcekey="btnGoBacktoStage"
                    OnClick="btnGoBacktoStage_Click" CssClass="btngobackto" TagName="Read" />
            </div>
            <br />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
