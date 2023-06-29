<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="WorkflowDetail_Report.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkflowDetail_Report" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
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
    <style type="text/css">
        .auto-style1 {
        }
    </style>
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
            <tr >
                <td style="width: 120px">Tender Executive</td>
                <td>
                    <asp:DropDownList ID="ddlTe" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlProcess" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProcess_SelectedIndexChanged" Style="display: none">
                    </asp:DropDownList>
                </td>
                <td>&nbsp;&nbsp;Business Unit
                </td>
                <td>
                    <asp:DropDownList ID="ddlBusinessUnit" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlWorkflow" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlWorkflow_SelectedIndexChanged" Style="display: none">
                    </asp:DropDownList>
                </td>
                <td class="auto-style1">Category</td>
                <td>
                    <asp:DropDownList ID="ddlCategory" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                </td>

            </tr>
            <tr >
                <td>Mode of Supply</td>
                <td>
                    <asp:DropDownList ID="ddlModeofSupply" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlStage" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStage_SelectedIndexChanged" Style="display: none">
                    </asp:DropDownList>
                </td>
                <td>&nbsp;&nbsp;
                    Product(s)
                </td>
                <td>
                    <asp:DropDownList ID="ddlProducts" runat="server" AutoPostBack="true">
                        <asp:ListItem>--Select--</asp:ListItem>
                        <asp:ListItem>Oncology</asp:ListItem>
                        <asp:ListItem>Non-Oncology</asp:ListItem>
                        <asp:ListItem>Both</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" Style="display: none">
                    </asp:DropDownList>
                </td>
                <td class="auto-style1">Tender/Budgetary Quotation</td>
                <td><asp:DropDownList ID="ddlTenderBudgetory" runat="server" AutoPostBack="true">
                    <asp:ListItem>--Select--</asp:ListItem>
                    <asp:ListItem>Tender</asp:ListItem>
                    <asp:ListItem>Budgetary</asp:ListItem>
                </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Tender Due (From) Date</td>
                <td>
                    <asp:TextBox ID="txtTenderfromdate" runat="server"></asp:TextBox>
                    <img alt="Icon" src="../../Images/cal.gif" id="Image2" />
                    <ajax:CalendarExtender ID="CalendarExtender1" runat="server"
                        TargetControlID="txtTenderfromdate" PopupButtonID="Image2" Format="MM/dd/yyyy" />
                </td>
                <td>To Date</td>
                <td>
                    <asp:TextBox ID="txtTenderTodate" runat="server"></asp:TextBox>
                      <img alt="Icon" src="../../Images/cal.gif" id="ImgTodate" />
                    <ajax:CalendarExtender ID="CalendarExtender2" runat="server"
                        TargetControlID="txtTenderTodate" PopupButtonID="ImgTodate" Format="MM/dd/yyyy" />
                </td>
                <td class="auto-style1" colspan="2">
                    <asp:Button ID="btnGenerate" runat="server" Text="Generate Report" TagName="Read" OnClientClick="return ValidateSearch();"
                       CssClass="btngeneratereport" OnClick="btnGenerate_Click" />
                &nbsp;<asp:Button ID="btnSearch" runat="server" Text="Export to Excel" TagName="Read" OnClientClick="return ValidateSearch();"
                        OnClick="btnSearch_Click" CssClass="btngeneratereport" />
                </td>
            </tr>
        </table>
        <asp:GridView ID="GridReport" runat="server" AllowPaging="True"
            CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EmptyDataText="No report is available" CellPadding="10" CellSpacing="10"
            OnRowDataBound="GridReport_RowDataBound" OnPageIndexChanging="GridReport_PageIndexChanging">
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>

            <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                PageButtonCount="5" PreviousPageText=" " />
            <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
        </asp:GridView>
    </div>
</asp:Content>
