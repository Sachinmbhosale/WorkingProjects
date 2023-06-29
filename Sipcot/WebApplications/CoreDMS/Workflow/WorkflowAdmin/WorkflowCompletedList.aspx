<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master" Async="true"
    AutoEventWireup="true" CodeBehind="WorkflowCompletedList.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkflowCompletedList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

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
        }
        function GoToDataEntry(e) {
            e.cells[0].getElementsByTagName("input")[0].checked = true;
            document.getElementById("<%=btnGotoDataEntry.ClientID %>").click();
        }
        function RedirectToDataEntry(mUrl) {

            window.location.href = "<%=ResolveUrl("~")%>Workflow/WorkflowAdmin/WorkflowDataEntry.aspx" + mUrl;
            return false;
        }


    </script>
    <script type="text/javascript">
        function Validate() {
            var gv = document.getElementById("<%=GridPendingList.ClientID%>");
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
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var ddlProc = document.getElementById("<%= ddlProcess.ClientID %>");
            var ddlwork = document.getElementById("<%= ddlWorkflow.ClientID %>");

            if (ddlProc.value == "0" || ddlProc.value == undefined) {
                lblMsg.innerHTML = "Please select the process name.";
                return false;
            }
            else if (ddlwork.value == "0" || ddlwork.value == undefined) {
                lblMsg.innerHTML = "Please select the workflow name.";
                return false;
            }

            return true;

        }

        function ValidateAdd() {
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var ddlProc = document.getElementById("<%= ddlProcess.ClientID %>");
            var ddlwork = document.getElementById("<%= ddlWorkflow.ClientID %>");
            var ddlstage = document.getElementById("<%= ddlStage.ClientID %>");
            if (ddlProc.value == "0" || ddlProc.value == undefined) {
                lblMsg.innerHTML = "Please select the process name.";
                return false;
            }
            else if (ddlwork.value == "0" || ddlwork.value == undefined) {
                lblMsg.innerHTML = "Please select the workflow name.";
                return false;
            }
            else if (ddlstage.value == "0" || ddlstage.value == undefined) {
                lblMsg.innerHTML = "Please select the StageName name.";
                return false;
            }
            return true;

        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Always" >
        <ContentTemplate>
            <div class="GVDiv">
                <table width="100%">
                    <tr>
                        <td colspan="6">
                            <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td k>
                            <asp:Label ID="lblProcessName" runat="server" Text="Process Name"></asp:Label>
                            <span style="color: Red; font-size: medium">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlProcess" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProcess_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblWorkflowName" runat="server" Text="Workflow Name"></asp:Label>
                            <span style="color: Red; font-size: medium">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlWorkflow" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlWorkflow_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblStageName" runat="server" Text="Stage Name"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlStage" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStage_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblSatust" runat="server" Text="Status Name" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Button ID="btnSearch" runat="server" TagName="Read" Text="Search" OnClientClick="return ValidateSearch();"
                                OnClick="btnSearch_Click" CssClass="btnsearch" />
                           
                        </td>
                    </tr>
                </table>
            </div>
            <div class="GVDiv">
                <asp:GridView ID="GridPendingList" runat="server" AutoGenerateColumns="true" DataKeyNames="Id#"
                    CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                    EmptyDataText="No list are found" CellPadding="10" CellSpacing="5" AllowPaging="True"
                    PageSize="10" OnRowDataBound="GridPendingList_RowDataBound" OnPageIndexChanging="GridPendingList_PageIndexChanging">
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:RadioButton ID="RowSelector" TagName="Read" runat="server" OnClick="javascript:SelectSingleRadiobutton(this)" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                        PageButtonCount="5" PreviousPageText=" " />
                    <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                </asp:GridView>
                <asp:Panel ID="ButtonPanel" runat="server">
                    <asp:Button ID="btnGotoDataEntry" runat="server" Text="Goto Next Stage" OnClick="btnGotoDataEntry_Click"
                        TagName="Read" OnClientClick="return Validate();" CssClass="btndataentry" />
                    <asp:Button ID="btnAddNew" runat="server" Text="Add New WorkItem" OnClick="btnAddNew_Click" Style="display: none"
                        TagName="Add" OnClientClick="return ValidateAdd();" CssClass="btnaddworkitem" />
                </asp:Panel>
                <asp:Button Text="Download Report" runat="server" ID="btnOpenDownlRpt" Style="display: none" />
                <ajax:modalpopupextender id="mdlDownloadRpt" runat="server" targetcontrolid="btnOpenDownlRpt"
                    popupcontrolid="pnlDownloadReport" cancelcontrolid="imgClosePopUp" okcontrolid="btnCancel" backgroundcssclass="modalBackground">
                </ajax:modalpopupextender>

                <asp:Panel ID="pnlDownloadReport" runat="server">
                    <div class="model-table">
                        <div class="GVDiv" style="width: 650px">
                            <asp:ImageButton ID="imgClosePopUp" runat="server" Style="float: right"
                                ImageUrl="~/Images/close.png" />

                            <h2>Download Report</h2>

                            <fieldset>
                                <table style="width: 100%">
                                    <tr valign="top">
                                        <td style="overflow: hidden!important">
                                            <span style="font-weight: 600">Select File  
                                            </span>
                                        </td>
                                        <td>:
                                        </td>
                                        <td>
                                            <asp:CheckBoxList ID="chkReports" runat="server">
                                                <asp:ListItem Text="Draft Warranty Guarantee Certificate" Value="DraftWarrantyGuaranteeCert" />
                                                <asp:ListItem Text="Draft Fall Clause Certificate" Value="DraftFallClauseCert" />
                                                <asp:ListItem Text="Draft Undertaking Letter" Value="DraftUndertakingLetter" />
                                                <asp:ListItem Text="Draft Penalty Clause Cert" Value="DraftPenaltyClauseCert" />
                                                <asp:ListItem Text="Railway Letter-West Central Railway" Value="RL-WestCentralRailway" />
                                                <asp:ListItem Text="Draft Delivery Extension Letter" Value="DraftDeliveryExtensionLetter" />
                                                <asp:ListItem Text="Draft GST Declaration Certificate" Value="DraftGSTDeclarationCertificate" />
                                                <asp:ListItem Text="Draft Product Genuinity Certificate" Value="DraftProductGenuinityCertificate" />
                                                <asp:ListItem Text="Draft Shell Life Confirmation Letter" Value="DraftShellLifeConfirmationLetter" />
                                                <asp:ListItem Text="Draft Short Supply Letter" Value="DraftShortSupplyLetter" />
                                            </asp:CheckBoxList>
                                            <rsweb:reportviewer id="rptReports" runat="server" width="600" visible="false">
                                            </rsweb:reportviewer>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td style="text-align: right">
                                            <asp:Button ID="btnDownload" runat="server" Text="Download" OnClick="btnDwnlReport_Click" align="right" />
                                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" align="right" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownload" />
        </Triggers>
    </asp:UpdatePanel>

    <div style="display: none">
        <asp:HiddenField ID="hiddWorkFlowFieldId" runat="server" />
        <asp:HiddenField ID="hidStageid" runat="server" />
    </div>

    <script type="text/javascript">
        function OpenReportDownload(vWorkFlowId) {

            $("#<%=hiddWorkFlowFieldId.ClientID%>").val(vWorkFlowId);

            $("#<%=btnOpenDownlRpt.ClientID%>").click();

            return false;
        }
    </script>
</asp:Content>
