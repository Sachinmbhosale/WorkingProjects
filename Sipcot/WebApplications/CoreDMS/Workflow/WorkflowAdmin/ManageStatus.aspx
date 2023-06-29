<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageStatus.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageStatus"
    Culture="auto" meta:resourcekey="ManageStatus" UICulture="auto" EnableEventValidation="false" %>

<%@ Register TagPrefix="WF" TagName="WorkFlowWizard" Src="WorkFlowWizardMenu.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            var StatusName = document.getElementById("<%= txtStatusName.ClientID %>").value;
            var StatusDesc = document.getElementById("<%= txtStatusDescription.ClientID %>").value;
            var StatusActive = document.getElementById("<%= chkActive.ClientID %>").checked
            var ConfirmOnSubmit = document.getElementById("<%= chkConfirmOnSubmit.ClientID %>").checked
            var SendNotification = document.getElementById("<%= chkSendNotification.ClientID %>").checked
            var ConfirmMsgOnSubmit = document.getElementById("<%= txtConfirmMsgOnSubmit.ClientID %>").value;
            var MoveToStageValue = document.getElementById("<%= ddl_Stages.ClientID %>").value;

            if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value == "EDIT_ERROR") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReload("0", StatusName, StatusDesc, StatusActive, ConfirmOnSubmit, SendNotification, ConfirmMsgOnSubmit, MoveToStageValue);
            }
            else if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value == "PAGE_CHANGE") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReloadMasterData();
            }
        });

        function ShowMDReloadMasterData() {
            modelBG.className = "mdBG";
            mb.className = "mdboxScroll";
            return false;
        }

        function ShowMDReload(StatusID, StatusName, StatusDescription, IsActive, ConfirmOnSubmit, SendNotification, ConfirmMsgOnSubmit, MoveToStageValue) {

            var txtStatusName = document.getElementById("<%= txtStatusName.ClientID %>");
            var txtStatusDescription = document.getElementById("<%= txtStatusDescription.ClientID %>");
            var txtConfirmMsgOnSubmit = document.getElementById("<%= txtConfirmMsgOnSubmit.ClientID %>");
            var ddl_Stages = document.getElementById("<%= ddl_Stages.ClientID %>");
            txtStatusName.value = StatusName;
            txtStatusDescription.value = StatusDescription;
            txtConfirmMsgOnSubmit.value = ConfirmMsgOnSubmit;
            ddl_Stages.value = MoveToStageValue;
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");
            var chkConfirmOnSubmit = document.getElementById("<%= chkConfirmOnSubmit.ClientID %>");
            var chkSendNotification = document.getElementById("<%= chkSendNotification.ClientID %>");
            var SaveButton = document.getElementById("<%= btnSave.ClientID %>");


            if (IsActive == false) {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }

            if (ConfirmOnSubmit == false) {
                chkConfirmOnSubmit.checked = false;
            }
            else { chkConfirmOnSubmit.checked = true; }

            if (SendNotification == false) {
                chkSendNotification.checked = false;
            }
            else { chkSendNotification.checked = true; }

            modelBG1.className = "mdBG";
            mb1.className = "mdbox";
            return false;
        };

    </script>
    <script language="javascript" type="text/javascript">
        function MaxLength(txtbox, maxLength) {
            if (txtbox.value.length >= maxLength) {
                txtbox.value = txtbox.value.substring(0, maxLength - 1);
            }
        }
    </script>
    <script language="javascript" type="text/javascript">
        function SelectSingleRadiobutton(rdbtnid) {
            var rdBtn = document.getElementById(rdbtnid);
            var rdBtnList = document.getElementsByTagName("input");
            for (i = 0; i < rdBtnList.length; i++) {
                if (rdBtnList[i].type == "radio" && rdBtnList[i].id != rdBtn.id) {
                    rdBtnList[i].checked = false;
                }
            }
        }
    </script>
    <script type="text/javascript">
        function ShowMD() {
            modelBG.className = "mdBG";
            mb.className = "mdboxScroll";
            return false;
        };
        function HideMD() {

            modelBG.className = "mdNone";
            mb.className = "mdNone";
            return false;
        };

         
    </script>
    <script type="text/javascript">
        function GetSelectedRow(lnk) {

            var row = lnk.parentNode.parentNode;
            var StatusId = row.cells[3].innerHTML;
            var currentStatusId = row.cells[2].innerHTML;
            var Statusname = row.cells[4].innerHTML;
            var Statusdescription = row.cells[5].innerHTML.replace(/(&nbsp;)/g, '');
            var Isactive = row.cells[8].innerHTML;
            var ConfirmMsgOnSubmit = row.cells[10].innerHTML.replace(/(&nbsp;)/g, '');
            var ConfirmationStatus = row.cells[15].innerHTML;
            var NotificationStatus = row.cells[16].innerHTML;
            var selectedMoveToStages = row.cells[17].innerHTML;

            var txtStatusName = document.getElementById("<%= txtStatusName.ClientID %>");
            var labelmassge = document.getElementById("<%=lblMessage.ClientID %>");
            var txtStatusDescription = document.getElementById("<%= txtStatusDescription.ClientID %>");
            var chkActive = document.getElementById("<%= chkActive.ClientID %>");
            var ddl_Stages = document.getElementById("<%= ddl_Stages.ClientID %>");

            var txtConfirmMsgOnSubmit = document.getElementById("<%= txtConfirmMsgOnSubmit.ClientID %>");
            var chkConfirmOnSubmit = document.getElementById("<%= chkConfirmOnSubmit.ClientID %>");
            var chkSendNotification = document.getElementById("<%= chkSendNotification.ClientID %>");

            if (Isactive == "Inactive") {
                chkActive.checked = false;
            }
            else { chkActive.checked = true; }

            if (ConfirmationStatus == "No") {
                chkConfirmOnSubmit.checked = false;
            }
            else { chkConfirmOnSubmit.checked = true; }

            if (NotificationStatus == "No") {
                chkSendNotification.checked = false;
            }
            else { chkSendNotification.checked = true; }

            txtStatusName.value = Statusname;
            txtStatusDescription.value = Statusdescription;
            txtConfirmMsgOnSubmit.value = ConfirmMsgOnSubmit;
            ddl_Stages.value = selectedMoveToStages;

            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save Changes";
            document.getElementById("<%= hdnStatusId.ClientID %>").value = StatusId;
            document.getElementById("<%= hdncurrentstatusid.ClientID %>").value = currentStatusId;
            labelmassge.innerHTML = "";

            modelBG1.className = "mdBG";
            mb1.className = "mdbox";

            return false;
        }

        function HideMD1() {

            modelBG1.className = "mdNone";
            mb1.className = "mdNone";
            return false;
        };

        function CheckEmptyTextStatusName() {
            var StatusName = document.getElementById("<%= txtStatusName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z]+$/; var uid;
            uid = StatusName.value;

            if (StatusName.value == "" || StatusName.value == undefined) {
                lblMsg.innerHTML = "Please enter a status name .";

                return false;
            }
            if (re.test(uid)) {
                return true;
            }
            else {
                lblMsg.innerHTML = "Status name allows alphabets, numbers and few special characters ( _ - # @ ^ $) only.";
                return false;
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <WF:WorkFlowWizard ID="WorkFlowWizard1" runat="server" ActiveItemName="Status" />
    <div class="GVDiv">
        <div class="InfoDisplay">
            <h3>
                <asp:Label ID="lblCurrentStageNameHeader" runat="server" Text="Stage Name: " />
                <asp:Label ID="lblCurrentStageNameHeaderValue" runat="server" Text="-" />
            </h3>
        </div>
        <asp:Label ID="lblErrorMessage" runat="server" Text="" ForeColor="Green"></asp:Label>
        <asp:GridView ID="gridStage" runat="server" OnRowDataBound="gridStage_RowDataBound"
            OnPageIndexChanging="gridStage_PageIndexChanging" DataKeyNames="CurrstageStatusesId"
            AllowPaging="True" CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
            EmptyDataText=" Stage statuses are not yet associated">
            <Columns>
                <asp:TemplateField HeaderText="Edit">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" TagName="Read" runat="server" CommandArgument="" OnClientClick="return GetSelectedRow(this)"
                            Text="" ToolTip="Edit"><img  alt="" src="../images/Edit.png"/></asp:LinkButton>
                               <asp:LinkButton ID="lnkDelete" runat="server" CommandArgument="" CausesValidation="false" OnClick="btnDelete_Click"
                                    TagName="Read" ToolTip="Delete"><img src="../images/delete.png"/></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
            <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                PageButtonCount="5" PreviousPageText=" " />
            <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
        </asp:GridView>
        <asp:Button ID="btnAdd"  TagName="Add"  runat="server" Text="Associate Status" OnClientClick="return ShowMD();"
            meta:resourcekey="btnAdd" CssClass="btnaddstatus"/>
        <asp:Button ID="btnGoBacktoStage" TagName="Read" runat="server" Text="Go Back To Stages" meta:resourcekey="btnGoBacktoStage"
            OnClick="btnGoBacktoStage_Click" CssClass="btngobackto"/>
    </div>
    <asp:HiddenField ID="hdnSaveStatus" runat="server" />
    <asp:HiddenField runat="server" ID="hdnStatusId" />
    <asp:HiddenField ID="hdncurrentstatusid" runat="server" />
    <div id="modelBG" class="mdNone">
    </div>
    <div id="mb" class="mdNone">
        <div id="Content">
            <h3>
                <asp:Label ID="lblAvailableStatuses" runat="server" Text="Available Status" meta:resourcekey="lblAvailableStatuses" /></h3>
            <div style="overflow-y: auto;">
                <asp:GridView ID="gridStageMaster" runat="server" AutoGenerateColumns="true" DataKeyNames="status id"
                    AllowPaging="true" CssClass="mGrid" PagerStyle-CssClass="pgr" OnRowDataBound="gridStatusMaster_RowDataBound"
                    AlternatingRowStyle-CssClass="alt" EmptyDataText="Master statuses are not available to select"
                    OnPageIndexChanging="gridStageMaster_PageIndexChanging" PageSize="10">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="ChkStageMaster" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                        PageButtonCount="5" PreviousPageText=" " />
                    <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                </asp:GridView>
            </div>
        </div>
        <div style="float: right;">
            <asp:Button ID="btnOk" runat="server" OnClick="btnOk_Click" Text="OK" CausesValidation="false"
                meta:resourcekey="btnAdd_OK" CssClass="btnsave" TagName="Add"  />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return HideMD();"
                meta:resourcekey="btnAdd_Cancel" CssClass="btncancel" TagName="Read"/>
        </div>
    </div>
    <div id="modelBG1" class="mdNone">
    </div>
    <div id="mb1" class="mdNone">
        <div id="#">
            <table>
                <tr>
                    <td colspan="2">
                        <h3>
                            <asp:Label ID="lblEditStatusDetails" runat="server" Text="Edit Status Details" meta:resourcekey="lblEditStatusDetails" /></h3>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblEditStatusName" runat="server" Text="Status Name" meta:resourcekey="lblEditStatusName" /><span
                            style="color: Red; font-size: medium">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtStatusName" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblEditStatusDescription" runat="server" Text="Status Description"
                            meta:resourcekey="lblEditStatusDescription" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtStatusDescription" runat="server" TextMode="MultiLine" onkeyup="javascript:MaxLength(this, 250)"
                            MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblEditStatusActive" runat="server" Text="Active" meta:resourcekey="lblEditStatusActive" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblEditStatusSendNotification" runat="server" Text="Send Notification"
                            meta:resourcekey="lblEditStatusSendNotification" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkSendNotification" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblEditStatusConfirmationOnSubmit" runat="server" Text="Need Confirmation on Submit"
                            meta:resourcekey="lblEditStatusConfirmationOnSubmit" />
                    </td>
                    <td>
                        <asp:CheckBox ID="chkConfirmOnSubmit" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblEditStatusConfirmationMessage" runat="server" Text="Confirmation Message on Submit"
                            meta:resourcekey="lblEditStatusConfirmationMessage" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtConfirmMsgOnSubmit" runat="server" TextMode="MultiLine" onkeyup="javascript:MaxLength(this, 250)"
                            MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblEditStatusMoveToStage" runat="server" Text="Move to Different Stage"
                            meta:resourcekey="lblEditStatusMoveToStage" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddl_Stages" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnErrorStatus" runat="server" Value="" />
        </div>
        <div style="float: right;">
            <asp:Button ID="btnSave" runat="server"  TagName="Add"  Text="Save" OnClick="btnSave_Click" OnClientClick="return CheckEmptyTextStatusName();"
                meta:resourcekey="btnEdit_Save" CssClass="btnsave"/>
            <asp:Button ID="btCancel" TagName="Read" runat="server" Text="Cancel" OnClientClick="return HideMD1(); return false;"
                CausesValidation="false" meta:resourcekey="btnEdit_Cancel" CssClass="btncancel"/>
        </div>
         
    </div>
    <!--DMS5-4268 BS -->
      <asp:Button ID="btnShowPopup" runat="server" style="display:none" />
            <ajax:ModalPopupExtender ID="ModalPopup" runat="server" TargetControlID="btnShowPopup" PopupControlID="pnlpopup"
CancelControlID="btnNo"  BackgroundCssClass="modalBackground">
</ajax:ModalPopupExtender>
            <asp:Panel ID="pnlpopup" runat="server" BackColor="White" Height="93px" Width="400px"
                Style="display: none">
                 <div class="GVDiv">
                <table width="100%" style="width: 100%; height: 100%"
                    cellpadding="0" cellspacing="0">
                                    <tr>
                        <td colspan="2" align="left" style="padding: 5px; font-family: Calibri">
                            <asp:Label ID="lblUser" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right" style="padding-right: 15px;padding-top: 12px;">
                            <asp:Button ID="btnYes" OnClick="btnYes_Click" runat="server" Text="Delete" />
                            <asp:Button ID="btnNo"  runat="server" Text="Cancel" />
                                                   </td>
                    </tr>
                </table>
                </div>
            </asp:Panel>
              <!--DMS5-4268 BE -->
</asp:Content>
