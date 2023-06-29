<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageStages.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageStages"
    EnableEventValidation="false" Culture="auto" meta:resourcekey="ManageStages"
    UICulture="auto" %>

<%--

   Modified by sabina for dataentry
    Added new dropdownlist for dataentry type
    browse button to load template 
    changed the function GetSelectedRow()
    
--%>
<%@ Register TagPrefix="WF" TagName="WorkFlowWizard" Src="WorkFlowWizardMenu.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

        function ShowMDReload(StageID, StageName, StageDescription, StageActive, StageDataEntryType, StageTemplatePath, backgroundImage) {

            var txtProcessname = document.getElementById("<%= txtStageName.ClientID %>");
            var txtProcessDescription = document.getElementById("<%= txtStageDescription.ClientID %>");
            txtProcessname.value = StageName;
            txtProcessDescription.value = StageDescription;
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");
            document.getElementById("<%= lblStageTemplatePath.ClientID %>").innerHTML = StageTemplatePath;
            var showBackgroundImage = document.getElementById("<%=chkBackgroundImage.ClientID %>").checked;
            var dropDataEntry = document.getElementById("<%= ddlDataEntry.ClientID %>");
            dropDataEntry.value = StageDataEntryType;

            if (dropDataEntry.options[dropDataEntry.selectedIndex].text == "Normal" || dropDataEntry.options[dropDataEntry.selectedIndex].text == "Guided") {
                document.getElementById("<%=chkBackgroundImage.ClientID %>").disabled = true;
                showBackgroundImage.checked = backgroundImage;
            }
            else if (dropDataEntry.options[dropDataEntry.selectedIndex].text == "Form") {
                document.getElementById("<%=chkBackgroundImage.ClientID %>").disabled = false;
                showBackgroundImage.checked = backgroundImage;
            }
            if (StageActive == false) {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }
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
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {

            var StageName = document.getElementById("<%= txtStageName.ClientID %>").value;
            var StageDescription = document.getElementById("<%= txtStageDescription.ClientID %>").value;
            var StageActive = document.getElementById("<%= chkActive.ClientID %>").checked;
            var StageDataEntryType = document.getElementById("<%= ddlDataEntry.ClientID %>").value;

            if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value == "ADD_ERROR") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReload("0", StageName, StageDescription, StageActive, StageDataEntryType, "", "");
            }

            else if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value == "EDIT_ERROR") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReload("0", StageName, StageDescription, StageActive, StageDataEntryType, "", "");

            }

        });

    </script>
    <script type="text/javascript">
        function ShowMD() {
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "";
            document.getElementById("<%= lblErrorMessage.ClientID %>").innerHTML = "";
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
            var StageTemplatePath = "";
            var StageId = "";
            var Stagename = "";
            var Stagedescription = "";
            var Isactive = "";
            var TAT = "";
            var showImage = "";
            var row = lnk.parentNode.parentNode;
            if (row.cells[3] != null && row.cells[3] != undefined) {
                StageId = row.cells[3].innerHTML;
            }
            if (row.cells[6] != null && row.cells[6] != undefined) {
                Stagename = row.cells[6].innerHTML;
            }
            if (row.cells[7] != null && row.cells[7] != undefined) {
                Stagedescription = row.cells[7].innerHTML.replace(/(&nbsp;)/g, '');
            }
            if (row.cells[9] != null && row.cells[9] != undefined) {
                Isactive = row.cells[9].innerHTML;
            }
            if (row.cells[10] != null && row.cells[10] != undefined) {
                TAT = row.cells[10].innerHTML;
            }
            if (row.cells[6] != null && row.cells[6] != undefined) {
                document.getElementById("<%=hdnStageName.ClientID %>").value = row.cells[6].innerHTML;
            }
            if (row.cells[11] != null && row.cells[11] != undefined) {
                StageTemplatePath = row.cells[11].innerHTML;
            }
            else {
                StageTemplatePath = "";
            }
            if (row.cells[12] != null && row.cells[12] != undefined) {
                document.getElementById("<%=hdnDataentryId.ClientID %>").value = row.cells[12].innerHTML;
                var DataEntryType = row.cells[12].innerHTML;
                if (DataEntryType != null && DataEntryType != undefined) {
                    var dropDataEntry = document.getElementById("<%= ddlDataEntry.ClientID %>");
                    var stageUpload = document.getElementById("<%=StageTemplateUpload.ClientID %>");
                    dropDataEntry.value = DataEntryType;
                    if (dropDataEntry.options[dropDataEntry.selectedIndex].text == "Normal") {
                        document.getElementById("<%= lblStageTemplatePath.ClientID %>").innerHTML = "";
                        stageUpload.disabled = true;
                    }
                    else if (dropDataEntry.options[dropDataEntry.selectedIndex].text == "Guided" || dropDataEntry.options[dropDataEntry.selectedIndex].text == "Form") {
                        stageUpload.disabled = false;
                    }
                }
            }
            if (row.cells[14] != null && row.cells[14] != undefined) {
                if (row.cells[12] != null && row.cells[12] != undefined) {
                    var backGroundImage = document.getElementById("<%=chkBackgroundImage.ClientID %>");
                    var DataEntryType = row.cells[12].innerHTML;
                    document.getElementById("<%=hdnDataEntryType.ClientID %>").value = DataEntryType;
                    showImage = row.cells[14].innerHTML;
                    if (DataEntryType != null && DataEntryType != undefined) {
                        var dropDataEntry = document.getElementById("<%= ddlDataEntry.ClientID %>");
                        dropDataEntry.value = DataEntryType;
                        if (dropDataEntry.options[dropDataEntry.selectedIndex].text == "Normal" || dropDataEntry.options[dropDataEntry.selectedIndex].text == "Guided") {
                            backGroundImage.disabled = true;
                        }
                        else if (dropDataEntry.options[dropDataEntry.selectedIndex].text == "Form") {
                            if (showImage == "Inactive") {
                                backGroundImage.disabled = false;
                                backGroundImage.checked = false;
                            }
                            else if (showImage == "Active") {
                                backGroundImage.disabled = false;
                                backGroundImage.checked = true;
                            }

                        }
                    }
                }
            }
            if (row.cells[13] != null && row.cells[13] != undefined) {
                document.getElementById("<%=hdnDataEntryName.ClientID %>").value = row.cells[13].innerHTML;
            }
            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            var txtStageName = document.getElementById("<%= txtStageName.ClientID %>");
            var txtStageDescription = document.getElementById("<%= txtStageDescription.ClientID %>");
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");

            if (Isactive == "Inactive") {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }

            if (Stagename != null && Stagename != undefined) {
                txtStageName.value = Stagename;
            }
            if (Stagedescription != null && Stagedescription != undefined) {
                txtStageDescription.value = Stagedescription;
            }

            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save Changes";
            if (StageId != null && StageId != undefined) {
                document.getElementById("<%= hdnStageId.ClientID %>").value = StageId;
            }
            if (TAT != null && TAT != undefined) {
                document.getElementById("<%= txtTATDuration.ClientID %>").value = TAT;
            }
            if (row.cells[12] != null && row.cells[12] != undefined) {

                var DataEntryType = row.cells[12].innerHTML;
                if (DataEntryType != null && DataEntryType != undefined) {
                    var dropDataEntry = document.getElementById("<%= ddlDataEntry.ClientID %>");
                    dropDataEntry.value = DataEntryType;
                    if (StageTemplatePath != null && StageTemplatePath != undefined && dropDataEntry.options[dropDataEntry.selectedIndex].text != "Normal") {
                        document.getElementById("<%= hdnStageTemplatePath.ClientID %>").value = StageTemplatePath;
                        document.getElementById("<%= lblStageTemplatePath.ClientID %>").innerHTML = StageTemplatePath;
                    }
                    else {
                        document.getElementById("<%= lblStageTemplatePath.ClientID %>").innerHTML = "No file need to be uploaded"
                    }
                }
            }
            modelBG1.className = "mdBG";
            mb1.className = "mdbox";

            return false;
        }
        //Checking the file type for upload
        function uploadStart(sender, args) {

            var result;
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1).toLowerCase();

            if (filext == 'pdf') {
                //DMS5-3903A
                document.getElementById("<%=btnSave.ClientID %>").disabled = true;
                return true;
            }
            else {
                //DMS5-3900BS
                var err = new Error();
                err.name = 'File extension not supported';
                err.message = 'Please select supported file!';
                throw (err);
                return false;
                //DMS5-3900BE
            }
        }

        function uploadComplete(sender, args) {
            //DMS5-3903A
            document.getElementById("<%=btnSave.ClientID %>").disabled = false;
        }

        //Enable the browse button based on dataentry type
        function EnableControl() {
            var dropDataEntry = document.getElementById("<%= ddlDataEntry.ClientID %>");
            var stageUpload = document.getElementById("<%=StageTemplateUpload.ClientID %>");
            var StageName = document.getElementById("<%= txtStageName.ClientID %>").value;
            var StageDescription = document.getElementById("<%= txtStageDescription.ClientID %>").value;
            var StageActive = document.getElementById("<%= chkActive.ClientID %>").checked;
            var StageDataEntryType = document.getElementById("<%= ddlDataEntry.ClientID %>").value;
            //enabling or disabling background image checkbox and browse button  on dataentry type change
            document.getElementById("<%= lblStageTemplatePath.ClientID %>").innerHTML = "";
            document.getElementById("<%= hdnStageTemplatePath.ClientID %>").value = "";

            var backgroundImage = document.getElementById("<%=chkBackgroundImage.ClientID %>")
            if (dropDataEntry.options[dropDataEntry.selectedIndex].text == "Normal") {
                stageUpload.disabled = true;
                backgroundImage.disabled = true;
            }
            else if (dropDataEntry.options[dropDataEntry.selectedIndex].text == "Guided") {
                stageUpload.disabled = false;
                backgroundImage.disabled = true;
            }
            else if (dropDataEntry.options[dropDataEntry.selectedIndex].text == "Form") {
                stageUpload.disabled = false;
                backgroundImage.disabled = false;
            }
            if (backgroundImage.disabled == false) {
                ShowMDReload("0", StageName, StageDescription, StageActive, StageDataEntryType, "", backgroundImage.value);
            }
            else {
                ShowMDReload("0", StageName, StageDescription, StageActive, StageDataEntryType, "", "");

            }

        }
        //Reload back to stageedit page
        function ReloadDialogbox() {
            var stageUpload = document.getElementById("<%=StageTemplateUpload.ClientID %>");
            var StageName = document.getElementById("<%= txtStageName.ClientID %>").value;
            var StageDescription = document.getElementById("<%= txtStageDescription.ClientID %>").value;
            var StageActive = document.getElementById("<%= chkActive.ClientID %>").checked;
            var StageDataEntryType = document.getElementById("<%= ddlDataEntry.ClientID %>").value;
            var backgroundImage = document.getElementById("<%=chkBackgroundImage.ClientID %>").value;
            ShowMDReload("0", StageName, StageDescription, StageActive, StageDataEntryType, "", backgroundImage);
        }

        function HideMD1() {

            modelBG1.className = "mdNone";
            mb1.className = "mdNone";
            return false;
        };

        function CheckEmptyTextStageName() {

            var StgName = document.getElementById("<%= txtStageName.ClientID %>");
            var tatValue = parseInt(document.getElementById("<%= txtTATDuration.ClientID %>").value);
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z]+$/; var uid;
            uid = StgName.value;

            if (StgName.value == "" || StgName.value == undefined) {
                lblMsg.innerHTML = "Please enter a stage name .";

                return false;
            }
            if ((isNaN(tatValue)) || (tatValue <= 0)) {

                lblMsg.innerHTML = "Please enter the TAT value in minutes.(Numbers Only)";
                return false;
            }
            if (re.test(uid)) {
                return true;
            }
            else {
                lblMsg.innerHTML = "Stage name allows alphabets, numbers and few special characters ( _ - # @ ^ $) only.";
                return false;
            }


        }
        //normal saving
        function SaveFunctionality() {
            document.getElementById("<%=btnSubmit.ClientID %>").click();
        }
        function onlyNos(e, t) {
            try {
                if (window.event) {
                    var charCode = window.event.keyCode;
                }
                else if (e) {
                    var charCode = e.which;
                }
                else { return true; }
                if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                    return false;
                }
                return true;
            }
            catch (err) {
                alert(err.Description);
            }
        }

        //Added for warning message when the user change the data entry type and save
        function ConfirmBox() {
            var proceed = window.confirm("The Data entry type has been changed due to which all the stage fields coordinates values mapped to that stage will be cleared." + '\n' + "Do you really want to continue? Click OK to continue, Cancel to return");

            if (proceed)
                SaveFunctionality();

            ReloadDialogbox();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <WF:WorkFlowWizard ID="WorkFlowWizard1" runat="server" ActiveItemName="Stages" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="GVDiv">
                <asp:Label ID="lblErrorMessage" runat="server" Text="" ForeColor="Green"></asp:Label>
                <asp:GridView ID="gridStage" runat="server" AutoGenerateColumns="true" OnRowDataBound="gridStage_RowDataBound"
                    OnPageIndexChanging="gridStage_PageIndexChanging" DataKeyNames="Stage Id" CssClass="mGrid"
                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EmptyDataText=" Stages are not yet associated">
                    <Columns>
                        <asp:TemplateField HeaderText="Edit">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" CommandArgument="" OnClientClick="return GetSelectedRow(this)"
                                    Text="" CausesValidation="false" ToolTip="Edit" TagName="Read"><img src="../images/Edit.png"/></asp:LinkButton>
                                <asp:LinkButton ID="lnkDelete" runat="server" CommandArgument="" CausesValidation="false"
                                    OnClick="btnDelete_Click" TagName="Read" ToolTip="Delete"><img src="../images/delete.png"/></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Options">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkManageStageFeilds" runat="server" CommandArgument="" CausesValidation="false"
                                    TagName="Read" ToolTip="Go To StageFields"><img src="../images/stagefileds.png"/></asp:LinkButton>
                                <asp:LinkButton ID="lnkManageStageStatuses" runat="server" CommandArgument="" CausesValidation="false"
                                    TagName="Read" ToolTip="Go To StageStatuses"><img src="../images/stagestatuses.png"/></asp:LinkButton>
                                <asp:LinkButton ID="lnkManageStageUsers" runat="server" CommandArgument="" CausesValidation="false"
                                    TagName="Read" ToolTip="Go To StageUsers"><img src="../images/stageusers.png"/></asp:LinkButton>
                                <asp:LinkButton ID="lnkNotifications" runat="server" CommandArgument="" CausesValidation="false"
                                    TagName="Read" ToolTip="Go To Notifications"><img src="../images/stagenotifications.png"/></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                        PageButtonCount="5" PreviousPageText=" " />
                    <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                </asp:GridView>
                <asp:Button ID="btnAdd" runat="server" Text="Associate Stages" OnClientClick="return ShowMD();"
                    TagName="Add" CausesValidation="false" meta:resourcekey="btnAdd" CssClass="btnaddstage" />
                <asp:Button ID="btnGoBacktoWorkflow" runat="server" Text="Go Back To Workflow" meta:resourcekey="btnGoBacktoWorkflow"
                    OnClick="btnGoBacktoWorkflow_Click" CssClass="btngobackto" TagName="Read" />
            </div>
            <asp:HiddenField ID="hdnSaveStatus" runat="server" />
            <asp:HiddenField ID="hdnStageId" runat="server" />
            <div id="modelBG" class="mdNone">
            </div>
            <div id="mb" class="mdNone">
                <div id="Content">
                    <h3>
                        <asp:Label ID="lblAvailableStages" runat="server" Text="Available Stages" meta:resourcekey="lblAvailableStages"></asp:Label>
                    </h3>
                    <div style="overflow-y: auto;" class="GVDiv">
                        <asp:GridView ID="gridStageMaster" runat="server" AutoGenerateColumns="true" DataKeyNames="Stage Id"
                            CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                            OnRowDataBound="gridStageMaster_RowDataBound" EmptyDataText="Stages Are Not Available">
                            <Columns>
                                <asp:TemplateField>
                                    <%--   <HeaderTemplate>
                        <asp:CheckBox ID="ChkStageMasterHeader" runat="server" OnCheckedChanged="chkboxSelectAll_CheckedChanged" CausesValidation="false"
                            AutoPostBack="true" />
                    </HeaderTemplate>--%>
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
                        meta:resourcekey="btnOk" CssClass="btnsave" TagName="Add" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return HideMD();"
                        meta:resourcekey="btnCancel" CssClass="btncancel" TagName="Read" />
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
                                    <asp:Label ID="lblEditStageDetails" runat="server" Text="Edit Stage Details" meta:resourcekey="lblEditStageDetails" /></h3>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblStageName" runat="server" Text="Stage Name" meta:resourcekey="lblStageName"></asp:Label><span
                                    style="color: Red; font-size: medium">*</span>
                            </td>
                            <td>
                                <asp:TextBox ID="txtStageName" runat="server" MaxLength="50"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblStageDescription" runat="server" Text="Stage Description" meta:resourcekey="lblStageDescription" />
                            </td>
                            <td>
                                <asp:TextBox ID="txtStageDescription" runat="server" TextMode="MultiLine" onkeyup="javascript:MaxLength(this, 250)"
                                    MaxLength="250"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblTAT" runat="server" Text="TAT (in minutes)" />
                            </td>
                            <td>
                                <asp:TextBox ID="txtTATDuration" runat="server" onkeypress="return onlyNos(event,this);"
                                    MaxLength="4"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblDataeEntry" runat="server" Text="Data Entry Mode" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlDataEntry" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblUploadedPath" runat="server" Text="Template Path" />
                            </td>
                            <td>
                                <asp:Label ID="lblStageTemplatePath" runat="server" Style="color: gray; font-size: small;"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblSelectFile" runat="server" Text="Select File" />
                            </td>
                            <td>
                                <ajax:AsyncFileUpload ID="StageTemplateUpload" runat="server" CompleteBackColor="Lime"
                                    ErrorBackColor="Red" ThrobberID="Throbber" UploadingBackColor="#66CCFF" Width="290px"
                                    OnUploadedComplete="StageTemplateUpload_UploadedComplete" OnClientUploadStarted="uploadStart"
                                    OnClientUploadComplete="uploadComplete" />
                                <asp:Label ID="Throbber" runat="server" Style="display: none">
                          <img alt="Loading..." src="<%= Page.ResolveClientUrl("~/Images/indicator.gif")%>" /></asp:Label>
                                <span style="color: gray; font-size: small;">* supported file extension: pdf.</span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblBackgroundImage" runat="server" Text="Background Image" />
                            </td>
                            <td>
                                <asp:CheckBox ID="chkBackgroundImage" runat="server" />
                                <span style="color: gray; font-size: small;">shows template as a background image in
                                    data entry area.</span>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblStageActive" runat="server" Text="Active" meta:resourcekey="lblStageActive" />
                            </td>
                            <td>
                                <asp:CheckBox ID="chkActive" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
                <asp:HiddenField ID="hdnErrorStatus" runat="server" Value="" />
                <asp:HiddenField ID="hdnFileUpload" runat="server" />
                <asp:HiddenField ID="hdnStageTemplatePath" runat="server" />
                <asp:HiddenField ID="hdnPagesCount" runat="server" />
                <asp:HiddenField ID="hdnPageNo" runat="server" />
                <asp:HiddenField ID="hdnDataentryId" runat="server" />
                <asp:HiddenField ID="hdnDataEntryType" runat="server" />
                <asp:HiddenField ID="hdnDataEntryName" runat="server" />
                <asp:HiddenField ID="hdnStageName" runat="server" />
                <div style="float: right;">
                    <asp:Button ID="btnSave" runat="server" Text="Save" TagName="Add" OnClick="btnSave_Click"
                        OnClientClick="return CheckEmptyTextStageName();" meta:resourcekey="btnSave"
                        CssClass="btnsave" />
                    <asp:Button ID="btCancel" runat="server" TagName="Read" Text="Cancel" OnClientClick="return HideMD1(); return false;"
                        CausesValidation="false" meta:resourcekey="btCancel" CssClass="btncancel" />
                    <asp:Button ID="btnDataEntryClick" runat="server" Text="" Visible="false" OnClientClick="EnableControl();return false;"
                        TagName="Add" />
                    <asp:Button ID="btnSubmit" runat="server" CssClass="MnuAdminClear" OnClick="btnSubmit_Click"
                        TagName="Add" />
                </div>
            </div>
            <!--DMS5-4268 BS -->
            <asp:Button ID="btnShowPopup" runat="server" Style="display: none" />
            <ajax:ModalPopupExtender ID="ModalPopup" runat="server" TargetControlID="btnShowPopup"
                PopupControlID="pnlpopup" CancelControlID="btnNo" BackgroundCssClass="modalBackground">
            </ajax:ModalPopupExtender>
            <asp:Panel ID="pnlpopup" runat="server" BackColor="White" Height="93px" Width="400px"
                Style="display: none">
                <div class="GVDiv">
                    <table width="100%" style="width: 100%; height: 100%" cellpadding="0" cellspacing="0">
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
                            <td align="right" style="padding-right: 15px; padding-top: 12px;">
                                <asp:Button ID="btnYes" OnClick="btnYes_Click" runat="server" Text="Delete" />
                                <asp:Button ID="btnNo" runat="server" Text="Cancel" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            </div>
            <!--DMS5-4268 BE -->
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
