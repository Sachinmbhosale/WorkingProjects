<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="WorkflowReport_TAT.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkflowReport_TAT" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">
        function SelectSingleRadiobutton(rdbtn) {
            var rdBtn = rdbtn;
            var rdBtnList = document.getElementsByTagName("input");
            for (i = 0; i < rdBtnList.length; i++) {
                if (rdBtnList[i].type == "radio" && rdBtnList[i].id != rdBtn.id) {
                    rdBtnList[i].checked = false;
                }
            }

            var row = rdbtn.parentNode.parentNode;
            var FeildDataId = row.cells[1].innerHTML;
            var ProcessId = row.cells[2].innerHTML;
            var WorkflowId = row.cells[4].innerHTML;
            var StageId = row.cells[6].innerHTML;
            document.getElementById("<%= HdnFeildDataId.ClientID %>").value = FeildDataId;
            document.getElementById("<%= HdnProcessId.ClientID %>").value = ProcessId;
            document.getElementById("<%= HdnWorkflowId.ClientID %>").value = WorkflowId;
            document.getElementById("<%= HdnStageId.ClientID %>").value = StageId;
        }
    </script>
    <script type="text/javascript">
        function Validate() {
            var gv = document.getElementById("<%=GridReport.ClientID%>");
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            if (gv != null) {
                var rbs = gv.getElementsByTagName("input");
                var flag = 0;
                for (var i = 0; i < rbs.length; i++) {

                    if (rbs[i].type == "radio") {
                        if (rbs[i].checked) {
                            flag = 1;
                            break;
                        }
                    }
                }
                if (flag == 0) {
                    lblMsg.innerHTML = "Please select a row to proceed.";
                    return false;
                }
            }
            else {
                lblMsg.innerHTML = "Please search for pending workitems.";
                return false;
            }
        }

        function ValidateSearch() {

            return true;

        }
        

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="HdnFeildDataId" runat="server" />
    <asp:HiddenField ID="HdnProcessId" runat="server" />
    <asp:HiddenField ID="HdnWorkflowId" runat="server" />
    <asp:HiddenField ID="HdnStageId" runat="server" />
    <div class="GVDiv">
        <table width="100%">
            <tr>
                <td colspan="6">
                    <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width: 120px">
                    <asp:Label ID="lblProcessName" runat="server" Text="Process Name"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlProcess" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProcess_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td>
                    &nbsp;&nbsp;
                </td>
                <td>
                    <asp:Label ID="lblWorkflowName" runat="server" Text="Workflow Name"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlWorkflow" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlWorkflow_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblStageName" runat="server" Text="Stage Name"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlStage" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStage_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td>
                    &nbsp;&nbsp;
                </td>
                <td>
                    <asp:Label ID="lblSatust" runat="server" Text="Status Name" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                </td>
                <td>
                    &nbsp;&nbsp;
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Generate Report"  TagName="Read" OnClientClick="return ValidateSearch();"
                        OnClick="btnSearch_Click" CssClass="btngeneratereport" />
                </td>
            </tr>
        </table>
        <asp:GridView ID="GridReport" runat="server" AutoGenerateColumns="true" AllowPaging="true"
            CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
            PageSize="50" EmptyDataText="No report is available" CellPadding="10" CellSpacing="50"
            OnRowDataBound="GridReport_RowDataBound" OnPageIndexChanging="GridReport_PageIndexChanging">
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
            <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                PageButtonCount="5" PreviousPageText=" " />
            <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
        </asp:GridView>
    </div>
</asp:Content>
