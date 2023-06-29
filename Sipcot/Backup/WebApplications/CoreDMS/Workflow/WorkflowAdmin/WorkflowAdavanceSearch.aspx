<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="WorkflowAdavanceSearch.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkflowAdavanceSearch" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            var QName = document.getElementById("<%= txtQuery.ClientID %>").value;
            var SaveResult = document.getElementById("<%= hdnSaveResult.ClientID %>").value;
            if (SaveResult == "ERROR") {
                ShowMDReload(QName);
            }
        });


        function validate() {
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
            return validateDate();
        }

        function validateDate() {

            var stardDate = document.getElementById("<%= txtStartDate.ClientID %>");
            var endDate = document.getElementById("<%= txtEndDate.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            lblMsg.value = "";


            var stDateVal = stardDate.value.replace(":", "/").replace(" ", "/");
            var dayfield = stDateVal.split("/")[0];
            var monthfield = stDateVal.split("/")[1];
            var yearfield = stDateVal.split("/")[2];
            //            var hhfield = stDateVal.split("/")[3];
            //            var mmfield = stDateVal.split("/")[4];

            var stDt = new Date(yearfield, monthfield - 1, dayfield); //, hhfield, mmfield);


            var enDateVal = endDate.value.replace(":", "/").replace(" ", "/");
            dayfield = enDateVal.split("/")[0];
            monthfield = enDateVal.split("/")[1];
            yearfield = enDateVal.split("/")[2];
            //            hhfield = enDateVal.split("/")[3];
            //            mmfield = enDateVal.split("/")[4];

            var enDt = new Date(yearfield, monthfield - 1, dayfield); //, hhfield, mmfield);


            if (enDt < stDt) {
                lblMsg.style.color = "Red";
                lblMsg.innerHTML = "'Start Date' should be less than 'End Date'";
                return false;
            }

            return true;
        }

        function ShowMD() {
            document.getElementById("<%= lblPopUpErrorMessage.ClientID %>").innerHTML = "";
            var ddlQuery = document.getElementById("<%= ddlQueries.ClientID %>");

            var txtQuery = document.getElementById("<%= txtQuery.ClientID %>");

            if (ddlQuery.value == "0") {
                if (validate() == true) {

                    txtQuery.value = "";
                    modelBG.className = "mdBG";
                    mb.className = "mdbox";
                }
                else {
                    return false;
                }
            }
            else {
                HideMD();
                return true;
            }


            return false;
        };


        function HideMD() {

            modelBG.className = "mdNone";
            mb.className = "mdNone";
        };

        function CheckEmptyTextQueryName() {
            var QueryName = document.getElementById("<%= txtQuery.ClientID %>");
            var lblerrorMsg = document.getElementById("<%= lblPopUpErrorMessage.ClientID %>");
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z ]+$/; var uid;
            uid = QueryName.value;
            if (QueryName.value == "" || QueryName.value == undefined) {
                lblerrorMsg.innerHTML = "Query name cannot be empty.";
                return false;
            }
            if (re.test(uid)) {
                return true;
            }
            else {
                lblerrorMsg.innerHTML = "Query name allows alphabets, numbers and few special characters ( _ - # @ ^ $ ) only.";
                return false;
            }

        }

        function GetStageFeildId(Id) {
            var row = Id.parentNode.parentNode;
            var StageFeildId = row.cells[1].innerHTML; //StageFieldId 
            //document.getElementById(hdnDataId.ClientID).value = StageFeildId;
            //__doPostBack(btnhiddenStageFeildIdInherietedgrid.UniqueID, "OnClick");
        }

        function ShowMDReload(queryname) {

            var txtqueryname = document.getElementById("<%= txtQuery.ClientID %>");
            txtqueryname.value = queryname;
            modelBG.className = "mdBG";
            mb.className = "mdbox";
            return false;
        };

        var openMyModal = function (source) {
            window.showModalDialog(source, '', 'dialogWidth:1200px;dialogHeight:600px;');
        };

        var openMyWindow = function (source) {
            window.open(source);
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="GVDiv">
                    <table cellpadding="3" cellspacing="3" class="style1" border="0">
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" style="width: 400px">
                                <table width="100%">
                                    <tr>
                                        <td style="width: 250px">
                                            <asp:Label ID="Label1" runat="server" Text="Saved Queries"></asp:Label>
                                        </td>
                                        <td colspan="3">
                                            <asp:DropDownList ID="ddlQueries" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlQueries_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 250px">
                                            <nobr>
                                            <asp:Label ID="lblProcessName" runat="server" Text="Process Name"></asp:Label>
                                            <span style="color: Red; font-size: medium">*</span></nobr>
                                        </td>
                                        <td colspan="3">
                                            <asp:DropDownList ID="ddlProcess" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProcess_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <nobr>
                                            <asp:Label ID="lblWorkflowName" runat="server" Text="Workflow Name"></asp:Label>
                                            <span style="color: Red; font-size: medium">*</span></nobr>
                                        </td>
                                        <td colspan="3">
                                            <asp:DropDownList ID="ddlWorkflow" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlWorkflow_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblStageName" runat="server" Text="Stage Name"></asp:Label>
                                        </td>
                                        <td colspan="3">
                                            <asp:DropDownList ID="ddlStage" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStage_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblStatus" runat="server" Text="Status Name"></asp:Label>
                                        </td>
                                        <td colspan="3">
                                            <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblStartDate" runat="server" Text="Start Date" meta:resourcekey="lblStartDate"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>
                                            <asp:CalendarExtender runat="server" ID="calStartDate" Format="dd/MM/yyyy" TargetControlID="txtStartDate">
                                            </asp:CalendarExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblEndDate" runat="server" Text="End Date" meta:resourcekey="lblEndDate"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEndDate" runat="server"></asp:TextBox>
                                            <asp:CalendarExtender runat="server" ID="calEndDate" Format="dd/MM/yyyy" TargetControlID="txtEndDate">
                                            </asp:CalendarExtender>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td rowspan="4">
                                <table width="100%">
                                    <tr>
                                        <td colspan="4">
                                            <div style="height: 200px; width: 650px; overflow-y: auto;">
                                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                    <ContentTemplate>
                                                        <asp:PlaceHolder ID="PanelQueryHolder" runat="Server" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <div>
                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                        <ContentTemplate>
                                            <asp:Button ID="btnAddClause"  TagName="Add"  Text="Add search condition" runat="Server" OnClick="btnAddClause_Click"
                                                OnClientClick="return validate();"  CssClass="btnaddsearchcondition"/>
                                            <asp:Button ID="btnRemoveClause"  TagName="Add"  Text="Remove search condition" runat="Server" 
                                            OnClick="btnRemoveClause_Click" CssClass="btnremovesearchcondition" />
                                            <asp:Button ID="btnRemoveAllClause"  TagName="Add"  Text="Clear all conditions" 
                                                runat="Server" onclick="btnRemoveAllClause_Click" CssClass="btnclearallcondition"  />
                                            <asp:Button ID="btnSaveQuery"  TagName="Add" Text="Save query" runat="Server" OnClick="btnSaveQuery_Click"
                                                OnClientClick="return ShowMD();" CssClass="btnsavequery" />
                                            <asp:Button ID="btnSearch" TagName="Read" Text="Search" runat="Server" OnClick="btnSearch_Click"
                                                OnClientClick="return validate();" CssClass="btnsearch" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </td>
                            
                        </tr>
                        <tr>
                            <td colspan="2">
                            </td>
                            <asp:GridView ID="GridSearchResult" EmptyDataText="Results not found" CssClass="mGrid"
                                PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" AllowPaging="true"
                                PageSize="10" runat="server" OnPageIndexChanging="GridSearchResult_PageIndexChanging"
                                OnRowDataBound="GridSearchResult_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="View">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkView" TagName="Read" runat="server" Text="" ToolTip="View"><img src="../images/viewicon.png"/></asp:LinkButton>
                                            <asp:LinkButton ID="lnkDocDownload" TagName="Read" runat="server" Text=""  ToolTip="Download" ><img src="../images/docdownload.png"/></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                                    PageButtonCount="5" PreviousPageText=" " />
                                <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                            </asp:GridView>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    .<div id="modelBG" class="mdNone">
    </div>
    <div id="mb" class="mdNone">
        <div id="Content">
            <table>
                <tr>
                    <td colspan="2">
                        <h3>
                            <asp:Label ID="lblSaveSearchQuery" runat="server" Text="Save Search Query" meta:resourcekey="lblEditNotificationDetails" /></h3>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblPopUpErrorMessage" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblQueryName" runat="server" Text="Query Name"></asp:Label><span style="color: Red;
                            font-size: medium">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtQuery" runat="server" MaxLength="500"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div style="float: right;">
            <asp:Button ID="btnSave" runat="server" TagName="Add" Text="Save" OnClientClick="return CheckEmptyTextQueryName();"
                meta:resourcekey="btnSave" OnClick="SaveQueryName" CssClass="btnsave" />
            <asp:Button ID="btnCancel" runat="server" TagName="Read" Text="Cancel" OnClientClick="HideMD(); return false;"
                CausesValidation="false" meta:resourcekey="btnCancel" CssClass="btncancel"/>
            <asp:HiddenField ID="hdnSaveResult" runat="server" />
        </div>
    </div>
</asp:Content>
