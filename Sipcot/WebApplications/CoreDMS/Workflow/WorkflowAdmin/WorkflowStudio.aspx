<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="WorkflowStudio.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkflowStudio" %>

<%@ Register Assembly="AjaxControlToolKit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!--Accordian Part Start -->
    <script src="../DragAndDrop/Javascripts/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var saveStatus = "Yes";
        var ModuleName = "";
        var EditAddMode = "2";
        var id = "";
        var Copytoli = "";
        var Haschild = "";
        var appendtoAccordian = ""
        var ConfirmedProcess = "";
        $(document).ready(function () {

            //Accordian
            $("div.accordian").accordion({
                autoHeight: false,
                collapsible: true,
                active: false

            });


        });
    </script>
    <link href="../DragAndDrop/css/jquery-ui-1.9.2.custom.min.css" rel="stylesheet" type="text/css" />
    <!--Accordian Part  End -->
    <script type="text/javascript">
        function ConnectAddNewProcess() {

            //process Unconfirm
            SetFromValue("A", "Process");

            $("#processleft-pane,#processright-pane").sortable({
                connectWith: "#processleft-pane,#processright-pane",

                stop: function () {
                    ShowProcessProperties();

                },
                receive: function (event, ui) { },
                revert: true

            });
            $("#processleft-pane,#processright-pane").disableSelection();

            $("#processright-pane").on("sortreceive", function (event, ui) {
                var $list = $(this);

                if ($list.children().length > 1) {
                    alert('Please save the selected process a');
                    $(ui.sender).sortable('cancel');

                }


            });
        }

        function ConnectunconfirmProcess() {

            //process Unconfirm
            SetFromValue("U", "");
            $("#sortableProcessUnconfirm,#processright-pane").sortable({
                connectWith: "#sortableProcessUnconfirm,#processright-pane",

                stop: function () {
                    ShowProcessProperties();
                    loadDrop("Process");
                },
                receive: function (event, ui) { }


            });
            $("#sortableProcessUnconfirm,#processright-pane").disableSelection();

            $("#processright-pane").on("sortreceive", function (event, ui) {
                var $list = $(this);

                if ($list.children().length > 1) {
                    alert('Please save the selected process u');
                    $(ui.sender).sortable('cancel');

                }



            });
        }
        function SetFromValue(value, Action) {
            var count = $("#processright-pane").children().length;
            if (count < 1) {
                appendtoAccordian = value;
                CheckAddOREdit();
            }

        }
       

        //On Process Edit li click
        $(function () {
            $("#processright-pane").on('click', 'input', function (event) {

               
                //                var target = $(event.target);
                //                if (EditAddMode == 2) {
                //                    id = $(this).attr("id");

                //                    $(this).addClass('addnewProcess');
                //                    ShowProcessProperties();

                //                }
                //                else {
                //                   
                                    id = $(this).attr("id");

                                    $("#processright-pane li").css({
                                        backgroundColor: ''
                                    });
                                    $(this).addClass('activeprocess');

                                    document.getElementById('<%= hiddenProcessId.ClientID %>').value = id;
                                    if (EditAddMode == 1) {
                                        document.getElementById("<%= hdnAction.ClientID %>").value = "ProcessOnClick";
                                        document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save Changes";
                                        document.getElementById('<%= btnProcessEdit.ClientID %>').click();

                                    }
                                //}
                                DisableSaveButtons();
            });
        });


        function CheckAddOREdit() {
            if (appendtoAccordian == "A") {
                EditAddMode = 2;
            }
            else if ((appendtoAccordian == "U")) {
                EditAddMode = 1;
            }
        }

        //On Workflow li click
        $(function () {
            $("#divWorkflowDest").on('click', 'input', function () {
                id = $(this).attr("id");
                document.getElementById("<%= hdnCopytoli.ClientID %>").value = id;
                document.getElementById('<%= hiddenWorkflowId.ClientID %>').value = id;

                $("#divWorkflowDest input").css({
                    backgroundColor: ''
                });
                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });
                

            });
        });

        //On Stages li click
        $(function () {
            $("#divStageDest").on('click', 'input', function () {
                id = $(this).attr("id");
                document.getElementById("<%= hdnCopytoli.ClientID %>").value = id;
                document.getElementById('<%= hiddenStageId.ClientID %>').value = id;
                $("#divStageDest input").css({
                    backgroundColor: ''
                });
                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });
                
            });
        });


        function uploadComplete(sender, args) {

            document.getElementById("<%=btnStageSave.ClientID %>").disabled = false;
        }
        //Checking the file type for upload
        function uploadStart(sender, args) {

            var result;
            var lblMsg = document.getElementById("<%= lblMessageStage.ClientID %>");
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1).toLowerCase();

            if (filext == 'pdf') {
                //DMS5-3903A
                document.getElementById("<%=btnStageSave.ClientID %>").disabled = true;
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

        function DisableSaveButtons() {
            var btnProcessSave = document.getElementById("<%= btnSave.ClientID %>");
            var btnWorkflowSave = document.getElementById("<%= btnWorkflowSave.ClientID %>");
            var btnStageSave = document.getElementById("<%= btnStageSave.ClientID %>");
            var btnStatusSave = document.getElementById("<%= btnStatusSave.ClientID %>");
            var btnNotificationSave = document.getElementById("<%= btnNotificationSave.ClientID %>");
            if (ConfirmedProcess === "Yes") {
                btnProcessSave.disabled = true;
                btnWorkflowSave.disabled = true;
                btnStageSave.disabled = true;
                btnStatusSave.disabled = true;
                btnNotificationSave.disabled = true;
            }
        }

        function ShowProcessProperties() {

            if ($('#divProcessDest .single-itemp').length >= 1) {
                $('#divProcessDest .ui-state-default').remove();
                HidePanels();
            }
            else if ($('#divProcessDest .ui-state-default').length >= 1) {
                $('#divProcessDest .single-itemp').remove();
                HidePanels();

            }


            var pnlP = document.getElementById("<%= pnlProcess.ClientID %>");
            var processErrorMessage = document.getElementById("<%= lblMessageProcess.ClientID %>");
            ShowSinglePanel(pnlP);
            processErrorMessage.value = "";
            DisableSaveButtons();
        }

        function ChangeBackgroundColor(div) {
            $(div + "  input[type='button']").click(function () {
                $(div + ' input').css({
                    backgroundColor: ''
                });
                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });
            });
        }



        function ShowWorkflowProperties() {

            var pnlW = document.getElementById("<%= pnlWorkflow.ClientID %>");
            var tblWorkflowControls = document.getElementById("<%= tblWorkflowControls.ClientID %>");
            var tblWorkflowOwner = document.getElementById("<%= tblWorkflowOwner.ClientID %>");

            var msg = document.getElementById("<%= lblMessageWorkflow.ClientID %>");
//            if (msg != null)
//            { msg = msg.innerHTML; }
            var txtWorkflowName = document.getElementById("<%= txtWorkflowName.ClientID %>");
            var txtWorkflowDescription = document.getElementById("<%= txtWorkflowDescription.ClientID %>");
            var ddp = document.getElementById("<%= ddlDmsProject.ClientID%>");
            if (EditAddMode == 2) {
                document.getElementById('<%= hiddenWorkflowId.ClientID %>').value = 0;
            }
            if (tblWorkflowControls != null) {
                tblWorkflowControls.style.display = 'block';
            }
            if (tblWorkflowOwner != null) {
                tblWorkflowOwner.style.display = 'none';
            }
            ShowSinglePanel(pnlW);

            ClearControls(ddp, txtWorkflowName, txtWorkflowDescription, msg);
            if ((EditAddMode == 1) && (Haschild != "")) {
                document.getElementById('<%= btnWorkflowEdit.ClientID %>').click();
            }
            //return false;

            DisableSaveButtons();
        }
        //to clear controls
        function ClearControls(dropDown, txtname, txtdesp, lbl) {
            txtname.value = "";
            txtdesp.value = "";
            lbl.value = "";
            dropDown.value = "0";
        }


        function ShowStagesProperties() {
            var pnlStages = document.getElementById("<%= pnlStages.ClientID %>");
            var pnlStagesAdd = document.getElementById("<%= pnlStagesAdd.ClientID %>");
            if (EditAddMode == 2) {
                document.getElementById('<%= hiddenStageId.ClientID %>').value = 0;
            }
            if ((EditAddMode == 1) && (Haschild != "")) {
                pnlStages.style.display = 'block';
                document.getElementById('<%= btnStageEdit.ClientID %>').click();
            }
            else {
                pnlStagesAdd.style.display = 'block';
                document.getElementById('<%= btnStageAdd.ClientID %>').click();
            }
            ShowSinglePanel(pnlStages);

            // return false;

            DisableSaveButtons();
        }


        function ShowStatusProperties() {
            var pnlStatus = document.getElementById("<%= pnlStatus.ClientID %>");
            var pnlStatusAdd = document.getElementById("<%= pnlStatusAdd.ClientID %>");
            if ((EditAddMode == 1) && (Haschild != "")) {
                pnlStatus.style.display = 'block';
                document.getElementById('<%= btnStatusEdit.ClientID %>').click();
                ShowSinglePanel(pnlStatus);
            }
            else {
                pnlStatusAdd.style.display = 'block';
                document.getElementById('<%= btnStatusAdd.ClientID %>').click();
                ShowSinglePanel(pnlStatusAdd);
            }
            DisableSaveButtons();

            // return false;


        }
        function ShowNotificationProperties() {
            var pnlNotification = document.getElementById("<%= pnlNotifications.ClientID %>");
            document.getElementById('<%= hdnNotificationId.ClientID %>').value = 0;
            pnlNotification.style.display = 'block';
            if ((EditAddMode == 1) && (Haschild != "")) {
                document.getElementById('<%= btnNotificationEdit.ClientID %>').click();
            }
            else
            { document.getElementById('<%= btnNotificationAdd.ClientID %>').click(); }

            ShowSinglePanel(pnlNotification);
            DisableSaveButtons();
        }
        function ShowFieldProperties() {
            var hiddenFeildId = document.getElementById("<%= hdnStageFieldId.ClientID %>").value;
            HidePanels();
            document.getElementById('frameStageProperties').src = 'WorkflowStudioStageFields.aspx?FieldId=' + hiddenFeildId;
            modelBG.className = "mdBG";
            mb.className = "sentmailbox";
            //document.getElementById('<%= btnStageFieldEdit.ClientID %>').click();
            DisableSaveButtons();
            // return false;
        }


        function HideFieldsProperties() {
            document.getElementById('frameStageProperties').src = 'WorkflowStudioStageFields.aspx';
            modelBG.className = "mdNone";
            mb.className = "mdNone";

            //Close button of new field  
            if (($("#divFieldsDest li").last().find("input").css("background-color") == 'rgb(255, 0, 0)')) {
                $("#divFieldsDest input[type=button]").remove();
            }

            return false;
        }

        function CopyProcesstextToLi() {

            if (EditAddMode == 1) {
                var hiddenProcessId = document.getElementById("<%= hiddenProcessId.ClientID %>").value;
                $('#divProcessDest').find('#' + hiddenProcessId).val($("#<%= txtProcessName.ClientID %>").val());
            }
            else {
                $('#divProcessDest').find('#btnInputAddProcess').val($("#<%= txtProcessName.ClientID %>").val());
            }

        }

        function CopyWorkflowToLi() {
//            var liId = document.getElementById("<%= hdnCopytoli.ClientID %>").value;
//            var Div = '#Workflowright-pane';
//            var txtvalue = "#<%= txtWorkflowName.ClientID %>";
//            CopyToLi(liId, Div, txtvalue);
            //            // document.getElementById("<%= hdnCopytoli.ClientID %>").value = "";
            return false;
        }

        function CopyStagesToLi() {
            var liId = document.getElementById("<%= hdnCopytoli.ClientID %>").value;
            var Div = '#Workflowstageright-pane';
            var txtvalue = "#<%= txtStageName.ClientID %>";
            CopyToLi(liId, Div, txtvalue);
            //document.getElementById("<%= hdnCopytoli.ClientID %>").value = "";
        }

        function CopyToLi(liId, Div, Txt) {
            $(Div).find('#' + liId).val($(Txt).val());

        }


        function CheckEmptyTextProcessName() {

            var ddlOrganization = document.getElementById("<%= ddlOrganization.ClientID %>");

            var ProName = document.getElementById("<%= txtProcessName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessageProcess.ClientID %>");
            var hiddenOrganizationId = document.getElementById("<%= hiddenOrganizationId.ClientID %>");
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z ]+$/; var uid;
            uid = ProName.value;

            hiddenOrganizationId.value = ddlOrganization.value;

            if (ddlOrganization.value == "0" || ProName.value == undefined) {
                lblMsg.innerHTML = "Please select an organization name.";
                return false;
            }
            if (ProName.value == "" || ProName.value == undefined) {
                lblMsg.innerHTML = "Please enter a process name.";
                return false;
            }
            if (re.test(uid)) {
                return true;
            }
            else {
                lblMsg.innerHTML = "Process name allows alphabets, numbers and few special characters ( _ - # @ ^ $) only.";
                return false;
            }

        }



        function HidePanels() {
            //Hideing panels
            var pnlProcess = document.getElementById("<%= pnlProcess.ClientID %>");
            var pnlWorkflow = document.getElementById("<%= pnlWorkflow.ClientID %>");
            var pnlStages = document.getElementById("<%= pnlStages.ClientID %>");
            var pnlStatusAdd = document.getElementById("<%= pnlStatusAdd.ClientID %>");
            var pnlStatus = document.getElementById("<%= pnlStatus.ClientID %>");
            var pnlNotifications = document.getElementById("<%= pnlNotifications.ClientID %>");
            pnlProcess.style.display = 'none';
            pnlWorkflow.style.display = 'none';
            pnlStages.style.display = 'none';
            pnlStatusAdd.style.display = 'none';
            pnlStatus.style.display = 'none';
            pnlNotifications.style.display = 'none';

        }

        function ChangeUlColor(UlId) {

        }

        function ShowSinglePanel(Panel) {
            //Hideing panels
            var pnlProcess = document.getElementById("<%= pnlProcess.ClientID %>");
            var pnlWorkflow = document.getElementById("<%= pnlWorkflow.ClientID %>");
            var pnlStages = document.getElementById("<%= pnlStages.ClientID %>");
            var pnlStagesAdd = document.getElementById("<%= pnlStagesAdd.ClientID %>");
            var pnlStatusAdd = document.getElementById("<%= pnlStatusAdd.ClientID %>");
            var pnlStatus = document.getElementById("<%= pnlStatus.ClientID %>");
            var pnlNotifications = document.getElementById("<%= pnlNotifications.ClientID %>");
            if (pnlProcess != null) {
                pnlProcess.style.display = 'none';
            }
            if (pnlWorkflow != null) {
                pnlWorkflow.style.display = 'none';
            }
            if (pnlStages != null) {
                pnlStages.style.display = 'none';
            }
            if (pnlStagesAdd != null) {
                pnlStagesAdd.style.display = 'none';
            }
            if (pnlStatusAdd != null) {
                pnlStatusAdd.style.display = 'none';
            }
            if (pnlStatus != null) {
                pnlStatus.style.display = 'none';
            }
            if (pnlNotifications != null) {
                pnlNotifications.style.display = 'none';
            }
            Panel.style.display = 'block';
        }

        function loadDrop(action) {

            if (action == "Process") {
                if(EditAddMode == 1)
                {
                    var Id = $("#processright-pane li").last().attr('id');
                    document.getElementById('<%= hiddenProcessId.ClientID %>').value = Id;
                    document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save Changes";
                    document.getElementById('<%= hdnAction.ClientID %>').value = "ProcessDrag";
                    document.getElementById('<%= btnProcessEdit.ClientID %>').click();
                }

            }

        }

        function RemoveElementsfromDiv(action) {

            if (action == "Process") {
                $('#processright-pane li').remove();
                $('#divWorkflowDest input').remove();
              
            

                $('#Workflowstageright-pane').empty();
                $('#Workflowstageright-pane input').remove();
//                $('#divStagesDest input').remove();
                $('#divStatusDest input').remove();
                $('#divFieldsDest input').remove();
                $('#divNotiDest input').remove();

            }
            else if (action == "Workflow") {


                $('#Workflowstageright-pane input').remove();
                $('#Workflowstageright-pane').empty();
                $('#divStatusDest input').remove();
                $('#divFieldsDest input').remove();
                $('#divNotiDest input').remove();
                $('#Workflowright-pane input').css("background-color", "rgb(221, 221, 221)");
            }

            else if (action == "Stages") {

                $('#divStatusDest input').remove();
                $('#divFieldsDest input').remove();
                $('#divNotiDest input').remove();
                $('#Workflowstageright-pane input').css("background-color", "rgb(221, 221, 221)");
            }

            else if (action == "Status") {

                $('#divStatusDest input').remove();
                $('#divStatusDest li').css("background-color", "rgb(221, 221, 221)");
            }
            else if (action == "Fields") {

                $('#divFieldsDest input').remove();
                $('#divFieldsDest li').css("background-color", "rgb(221, 221, 221)");
            }
            else if (action == "Notification") {

                $('#divNotiDest input').remove();
                $('#divFieldsDest li').css("background-color", "rgb(221, 221, 221)");
            }

            HidePanels();
        }

        function HideProperty(buttonid) {

            var btnProcessCancel = document.getElementById("<%= btnProcessCancel.ClientID %>");
            var btnWorkflowCancel = document.getElementById("<%= btnWorkflowCancel.ClientID %>");
            var btnStageCancel = document.getElementById("<%= btnStageCancel.ClientID %>");
            var btnStatusAddCancel = document.getElementById("<%= btnStatusAddCancel.ClientID %>");
            var btnStatusCancel = document.getElementById("<%= btnStatusCancel.ClientID %>");
            var btnNotificationCancel = document.getElementById("<%= btnNotificationCancel.ClientID %>");
           
            

            if (btnProcessCancel.id === buttonid) {
                
                HidePanels();
                var processplaneX = $('#processright-pane  li')
                $(processplaneX).remove();




                if (appendtoAccordian == "A") {
                    $("#processleft-pane").append($(processplaneX));
                }

                if (appendtoAccordian == "U") {
                    $("#sortableProcessUnconfirm").append($(processplaneX));
                }
                if (appendtoAccordian == "C") {
                    $("#sortableProcessConfirm").append($(processplaneX));
                }

                RemoveElementsfromDiv("Process");

            }

            if (btnWorkflowCancel.id === buttonid) {
                HidePanels();
                RemoveElementsfromDiv("Workflow");

            }

            if (btnStageCancel.id === buttonid) {
                HidePanels();
                RemoveElementsfromDiv("Stages");
            }

            if (btnStatusAddCancel.id === buttonid) {
                HidePanels();
            }

            if (btnStatusCancel.id === buttonid) {
                var pnlStatus = document.getElementById("<%= pnlStatus.ClientID %>");	
                pnlStatus.style.display = 'none';
                RemoveElementsfromDiv("Status");
                saveStatus = "Yes";
            }

            if (btnNotificationCancel.id === buttonid) {
                HidePanels();
                RemoveElementsfromDiv("Notification");
            }
        }



        function CheckEmptyTextWorkflowName() {


            var WorkfloName = document.getElementById("<%= txtWorkflowName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessageWorkflow.ClientID %>");
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z]+$/; var uid;
            uid = WorkfloName.value;

            if (WorkfloName.value == "" || WorkfloName.value == undefined) {
                lblMsg.innerHTML = "Please enter a workflow name.";

               
                return false;
            }
            if (re.test(uid)) {
                if (EditAddMode == 2) {
                    document.getElementById('<%= hiddenWorkflowId.ClientID %>').value = 0;
                    document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Add Changes";
                }
                return true;
            }
            else {
                lblMsg.innerHTML = "Workflow name allows alphabets, numbers and few special characters ( _ - # @ ^ $ ) only.";
                return false;
            }

            
        }


        function setNotifyTat() {

            e = document.getElementById("<%=ddl_Category.ClientID%>");
            var Category = e.options[e.selectedIndex].text;
            if (Category == "Notification") {
                document.getElementById("<%=txt_TAT.ClientID%>").value = 0;
                document.getElementById("<%=txt_TAT.ClientID%>").readOnly = true;

            }
            else {

                document.getElementById("<%=txt_TAT.ClientID%>").readOnly = false;
            }
            return false;
        }


        var specialKeys = new Array();
        specialKeys.push(8); //Backspace

        function IsNumeric(e) {
            var keyCode = e.which ? e.which : e.keyCode
            var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
            return ret;
        }

        //handleing empty process owners
        function validateSelectedItem(controlName, errMessage, label) {

            if (controlName.value == ""
                 || controlName.value == undefined) {
                label.innerHTML = "";
                label.innerHTML = errMessage;
                ShowProcessProperties();
                return false;
            }
            return true;
        }
        //Add li to ul
        function AddlitoUl() {
            var ul = document.getElementById("processright-pane");
            var li = document.createElement("li");
            li.appendChild(document.createTextNode("Four"));
            ul.appendChild(li);
        }
        function ChangeAddProcessButtonColor(hndStatusFlag) {

            $('#processright-pane').find('#btnInputAddProcess').css("background-color", "rgb(0, 128, 0)");
            saveStatus = hndStatusFlag;
            return false;

        }
        function ChangeAddworkflowButtonColor() {
            var liId = document.getElementById("<%= hdnCopytoli.ClientID %>").value;
            $('#Workflowright-pane ').find("input").attr(liId).css("background-color", "rgb(0, 128, 0)");
          //  $('#Workflowright-pane').find('#' + liId).css("background-color", "rgb(0, 128, 0)");
            return false;
        }

        function ChangeAddStageButtonColor() {
            var liId = document.getElementById("<%= hdnCopytoli.ClientID %>").value;
            $('#Workflowstageright-pane ').find("input").attr(liId).css("background-color", "rgb(0, 128, 0)");
           // $('#Workflowstageright-pane').find('#' + liId).css("background-color", "rgb(0, 128, 0)");

            return false;
        }


        function CommonAlert(ModuleName, ClearDiv) {

            alert('Please save the ' + ModuleName + ' before continuing.');
            $(ClearDiv).remove();
        }

        function BuildProcess(Values) {
            Haschild = Values;
            var hiddenPId = document.getElementById('<%= hiddenProcessId.ClientID %>');
            var saveFlag = document.getElementById("<%= hdnSaveStatus.ClientID %>");
            var div = "#processright-pane";
            CommonSplitAndBind(hiddenPId, saveFlag, div, ShowWorkflowProperties, Values);
            DisableSaveButtons();
        }

        function BuildWorkflows(Values) {
            Haschild = Values;
            var hiddenWId = document.getElementById('<%= hiddenWorkflowId.ClientID %>');
            var saveFlag = document.getElementById("<%= hdnSaveStatus.ClientID %>");
            var div = "#Workflowright-pane";
           CommonSplitAndBind(hiddenWId, saveFlag, div, ShowWorkflowProperties, Values);
            DisableSaveButtons();
        }

        function BuildStages(Values) {
            Haschild = Values;
            var hiddenStageId = document.getElementById('<%= hiddenStageId.ClientID %>');
            var saveFlag = document.getElementById("<%= hdnSaveStatus.ClientID %>");
            var div = '#Workflowstageright-pane';
            CommonSplitAndBind(hiddenStageId, saveFlag, div, ShowStagesProperties, Values);
            DisableSaveButtons();
        }

        function BuildStageFields(Values) {

            var hiddenStageFeildId = document.getElementById('<%= hdnStageFieldId.ClientID %>');
            var saveFlag = document.getElementById("<%= hdnSaveStatus.ClientID %>");
            // var buttonid = document.getElementById('<%= btnStageFieldEdit.ClientID %>');
            var div = '#divFieldsDest';
            CommonSplitAndBind(hiddenStageFeildId, saveFlag, div, ShowFieldProperties, Values);
            DisableSaveButtons();
        }

        function BuildStageStatus(Values) {
            var hiddenStatusId = document.getElementById('<%= hdnStatusId.ClientID %>');
            var saveFlag = document.getElementById("<%= hdnSaveStatus.ClientID %>");
            var div = '#divStatusDest';
            CommonSplitAndBind(hiddenStatusId, saveFlag, div, ShowStatusProperties, Values);
            DisableSaveButtons();
        }

        function BuildAddStatus(Values) {
            var div = '#divStatusDest';
            BuildAddParts(div, Values);
            saveStatus = "Yes";
            DisableSaveButtons();
        }

        function BuildAddStages(Values) {
            var div = '#Workflowstageright-pane li';
            BuildAddParts(div, Values);
            saveStatus = "Yes";
            DisableSaveButtons();
        }

        function BuildAddNotification(Values) {
            var div = '#divNotiDest';
            BuildAddParts(div, Values);
            saveStatus = "Yes";
            DisableSaveButtons();
        }
        function BuildFields(Values) {

            var div = '#divFieldsDest';
            BuildAddParts(div, Values);
           
        }

        function CheckProcessLength() {

            if (($("#processright-pane li").find("li").length >= 2) && ($("#processright-pane li").last().find("li").css("background-color") == 'rgb(255, 0, 0)')) {

                $("#processright-pane li").last().find("li").remove();
            }

            if (($("#processright-pane li").last().find("li").css("background-color") == 'rgb(255, 0, 0)')) {
                $('#divWorkflowDest input').remove();
                $('#divStagesDest input').remove();
                $('#divStatusDest input').remove();
                $('#divFieldsDest input').remove();
                $('#divNotiDest input').remove();

            }
            HidePanels();
        }

        function BuildAddParts(div, Values) {
            $(div).empty();

            var str = Values.split(',');

            for (var i = 0; i < str.length; i++) {
                var $buttonStatus = $('<input/>', {
                    type: 'button',
                    value: str[i].split(',').splice(0, 1).join(""),
                    click: function () { }

                });
               // $buttonStatus.css("background-color", "rgb(0, 128, 0)");
                $buttonStatus.appendTo(div);
                $buttonStatus.addClass("eWorkflow");

            }
        }

        function BuildNotification(Values) {
            var hiddenNId = document.getElementById('<%= hdnNotificationId.ClientID %>');
            var saveFlag = document.getElementById("<%= hdnSaveStatus.ClientID %>");
            var div = '#divNotiDest';
            CommonSplitAndBind(hiddenNId, saveFlag, div, ShowNotificationProperties, Values);
        }


        function CommonSplitAndBind(hiddenId, savestatus, div, callFunction, Values) {
            $(div).empty();
            if (Values != "") {
                var strarray = Values.split(',');



                for (var i = 0; i < strarray.length; i++) {
                    var $button = $('<input/>', {
                        type: 'button',
                        id: strarray[i].split('-').splice(0, 1).join(""),
                        value: strarray[i].split('-').splice(1, 2).join(""),
                        click: function () {
                            hiddenId.value = this.id;
                            callFunction();
                            //savestatus.value = "Save Changes";

                        }
                    });
                }


                $button.appendTo(div);
                $button.addClass("eWorkflow");
               // ChangeBackgroundColor(div);
            }
        }
        function MaxLength(txtbox, maxLength) {
            if (txtbox.value.length >= maxLength) {
                txtbox.value = txtbox.value.substring(0, maxLength - 1);
            }
        }
        //Submit Stages
        function SaveStage() {
            document.getElementById("<%=btnStageSubmit.ClientID %>").click();
        }

        //Check Configuration
        function CheckallConfiguration() {
            //            var btnConfirm = document.getElementById("<%= btnConfirm.ClientID %>");
            //            btnConfirm.disabled = true;
            HidePanels();
            if (

($("#processright-pane li").last().css("background-color") == 'rgb(0, 128, 0)') &&
($("#Workflowright-pane li").last().find("input").css("background-color") == 'rgb(0, 128, 0)') &&
($("#Workflowstageright-pane li").find("input").css("background-color") == 'rgb(0, 128, 0)') &&
($("#divStatusDest").find("input").css("background-color") == 'rgb(0, 128, 0)') &&
($("#divNotiDest").last().find("input").css("background-color") == 'rgb(0, 128, 0)') &&
($("#divFieldsDest").last().find("input").css("background-color") == 'rgb(0, 128, 0)')

) {
                //                btnConfirm.disabled = false;
                return window.confirm('Are you sure you want to continue?')
            }
            else {
                alert('Please check the configuration');
                return false;
            }

        }

        // status validation
        function CheckEmptyTextStatusName() {
            var StatusName = document.getElementById("<%= txtStatusName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblStatusMessage.ClientID %>");
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

        //Notification validation


        function validateDetails() {
            var tatValue = parseInt(document.getElementById("<%= txt_TAT.ClientID %>").value);
            var lblMsg = document.getElementById("<%= lblNotificationMessage.ClientID %>");

            if ((isNaN(tatValue)) || (tatValue <= 0)) {
                lblMsg.innerHTML = "Please enter the TAT value in minutes.";
                return false;
            }

            return true;
        }

        function AddProcessNew() {
            //Process Drag and drop


            $("#processleft-pane li").draggable({
                containment: '#processright-pane',
                //cursor: 'move',
                helper: 'clone',
                scroll: false,
                connectToSortable: '#processright-pane',
                appendTo: '#processright-pane',
                revert: "invalid",
                drag: function (event, ui) {


                },
                start: function () { }, //document.getElementById('<%=hdnSaveFlagStatus.ClientID%>').value='No' },
                stop: function (event, ui) {
                    CheckProcessLength();
                    ModuleName = 'process';
                    //                    if (saveStatus != "Yes") {
                    //                        return false;
                    //                    }


                    saveStatus = "No";


                }
            });

            $("#processright-pane").sortable({
                sort: function () { },
                placeholder: 'ui-state-highlight',
                receive: function () { },
                update: function (event, ui) { }
            });


            $("#processright-pane li").live('dblclick', function (event) {

                $(this).remove();

                var target = $(event.target);
                if (target.is("li")) {

                    $(this).css({
                        backgroundColor: ''
                    })
                    $("#sortableProcessUnconfirm").append($(this));
                    //$("#sortableProcessConfirm").append($(this));

                }
                RemoveElementsfromDiv("Process");


            })

            $("#processright-pane").droppable({
                accept: "#processleft-pane li",
                accept: ":not(.ui-sortable-helper)",
                drop: function (event, ui) {
                    if ($(ui.draggable).find('.single-itemp').length == 0) {
                        $(ui.draggable).html("<div style='cursor:move' class='single-itemp' id='ProcessAdd'><input type='button' value='Add Process' id='btnInputAddProcess' style='width:95px;height:45px;background-color:red;' onclick='return ShowProcessProperties();' /></div>");
                    }

                }
            });

            $("#processleft-pane").droppable({
                accept: "#processright-pane li",
                drop: function (event, ui) { }
            });

            $("ul, li").disableSelection();

        }

        function AddWorkflowNew() {

            //Workflow Drag and drop
            var limitworkflow = 10;
            var counterworkflow = 0;

            $("#workflowleft-pane li").draggable({
                containment: '#Workflowright-pane',
                //  cursor: 'move',
                helper: 'clone',
                scroll: false,
                connectToSortable: '#Workflowright-pane',
                appendTo: '#Workflowright-pane',
                revert: "invalid",
                create: function (event, ui) { },
                start: function () {

                },
                stop: function (event, ui) {
                    ShowWorkflowProperties();
                    EditAddMode = 2;
                    document.getElementById("<%= btnWorkflowSave.ClientID %>").style.display = "block";

                    ClearDiv = '#Workflowright-pane li:last';
//                    if (($('#processright-pane  li').length == 0) ||
//                        ($("#processright-pane li").last().css("background-color") == 'rgb(255, 0, 0)')) {
//                        ModuleName = 'process';
//                        CommonAlert(ModuleName, ClearDiv);
//                    }

//                    else if (($('#Workflowright-pane  div').length >= 2) &&
//                     ($("#Workflowright-pane li:nth-last-child(1)").last().prev().find("input").css("background-color") == 'rgb(255, 0, 0)') && (saveStatus != "Yes")) {
//                        ModuleName = 'workflow';
//                        CommonAlert(ModuleName, ClearDiv);
//                    }

//                    else if ((Haschild == "") && ($("#processright-pane li").css("background-color") == 'rgb(255, 228, 32)')) {
//                        return false;
//                    }
                    saveStatus = "No";
                    counterworkflow++;

                }
            });

            $("#Workflowright-pane").sortable({
                sort: function () { },
                placeholder: 'ui-state-highlight',
                receive: function () { },
                update: function (event, ui) { }
            });


            $("#Workflowright-pane li").live('dblclick', function () {
                $(this).remove();
                HideProperty('ctl00_ContentPlaceHolder1_btnWorkflowCancel');
            })

            $("#Workflowright-pane").droppable({
                accept: "#workflowleft-pane li",
                accept: ":not(.ui-sortable-helper)",
                drop: function (event, ui) {
                    if ($(ui.draggable).find('.single-itemw').length == 0) {
                        $(ui.draggable).html("<div style='cursor:move' class='single-itemw' ><input type='button' value='Add Workflow' style='width:95px;height:45px;background-color:red;' onclick='return ShowWorkflowProperties();'  id='btnInputAddWorkflow" + counterworkflow + "'  /></div>");
                    }

                }
            });

            $("#workflowleft-pane").droppable({
                accept: "#Workflowright-pane li",
                drop: function (event, ui) { }
            });

            $("ul, li").disableSelection();

        }

        //Add Stage Drag and drop

        function AddStageNew() {
            //drag and drop stage
            var limitstages = 12;
            var counterStages = 0;

            $("#workflowstageleft-pane li").draggable({
                containment: '#Workflowstageright-pane',
                // cursor: 'move',
                helper: 'clone',
                scroll: false,
                connectToSortable: '#Workflowstageright-pane',
                appendTo: '#Workflowstageright-pane',
                revert: "invalid",
                start: function () { },
                stop: function (event, ui) {
                    var pnlStagesAdd = document.getElementById("<%= pnlStagesAdd.ClientID %>");
                    ShowSinglePanel(pnlStagesAdd);
                    document.getElementById('<%= btnStageAdd.ClientID %>').click();
                    //                    ClearDiv = '#Workflowstageright-pane li:last';

                    //                    if (($('#processright-pane  li').length == 0) ||
                    //                        ($("#processright-pane li").last().css("background-color") == 'rgb(255, 0, 0)')) {
                    //                        ModuleName = 'process';
                    //                        CommonAlert(ModuleName, ClearDiv);

                    //                    }

                    //                    else if (($('#Workflowright-pane  div').length == 0) ||
                    //                        ($("#Workflowright-pane li").last().find("input").css("background-color") == 'rgb(255, 0, 0)')) {
                    //                        ModuleName = 'workflow';
                    //                        CommonAlert(ModuleName, ClearDiv);
                    //                    }

                    //                    else if (($('#Workflowstageright-pane  div').length == 1)) {
                    //                        return;
                    //                    }

                    //                    else if (($('#Workflowstageright-pane  div').length >= 2) &&
                    //                     ($("#Workflowstageright-pane li:nth-last-child(1)").last().prev().find("input").css("background-color") == 'rgb(255, 0, 0)')) {

                    //                        ModuleName = 'stages';
                    //                        CommonAlert(ModuleName, ClearDiv);
                    //                    }

                    //                    saveStatus = "No";
                    counterStages++;
                    if (counterStages == limitstages) {
                        alert('Maximum 12 stages can be added!');
                        $("#workflowstageleft-pane li").droppable("disable");


                    }
                }
            });

            $("#Workflowstageright-pane").sortable({
                sort: function () { },
                placeholder: 'ui-state-highlight',
                receive: function () { },
                update: function (event, ui) { }
            });


            $("#Workflowstageright-pane li").live('dblclick', function () {
                $(this).remove();
                HidePanels();
            })

            $("#Workflowstageright-pane").droppable({
                accept: "#workflowstageleft-pane li",
                accept: ":not(.ui-sortable-helper)",
                drop: function (event, ui) {
                    if ($(ui.draggable).find('.single-item').length == 0) {
                        $(ui.draggable).html("<div style='cursor:move' class='single-item'><input type='button' value='Add Stages' style='width:95px;height:45px;background-color:red;' id='btnInputAddStage" + counterStages + "'   onclick='return ShowStagesProperties();' /><br /></div>");
                    }

                }
            });

            $("#workflowstageleft-pane").droppable({
                accept: "#Workflowstageright-pane li",
                drop: function (event, ui) { }
            });

            $("ul, li").disableSelection();

        }

        function AddStatusNew() {
            //Drag and drop Status

            $("#Statusleft-pane li").draggable({
                containment: '#divStatusDest',
                //cursor: 'move',
                helper: 'clone',
                scroll: false,
                connectToSortable: '#divStatusDest',
                appendTo: '#divStatusDest',
                revert: "invalid",

                start: function () { },
                stop: function (event, ui) {

                    ClearDiv = '#divStatusDest li:last';
                    if (($('#processright-pane  li').length == 0) ||
                        ($("#processright-pane li").last().css("background-color") == 'rgb(255, 0, 0)')) {
                        ModuleName = 'process';
                        CommonAlert(ModuleName, ClearDiv);

                    }

                    else if (($('#Workflowright-pane  div').length == 0) ||
                        ($("#Workflowright-pane li").last().find("input").css("background-color") == 'rgb(255, 0, 0)')) {
                        ModuleName = 'workflow';
                        CommonAlert(ModuleName, ClearDiv);
                    }

                    else if (($('#Workflowstageright-pane  li').length == 0) ||
                        ($("#Workflowstageright-pane li").last().find("input").css("background-color") == 'rgb(255, 0, 0)')) {
                        ModuleName = 'stages';
                        CommonAlert(ModuleName, ClearDiv);
                    }

                    else if (($('#divStatusDest div').length == 1) && ($("#Workflowstageright-pane li").last().find("input").css("background-color") == 'rgb(0, 128, 0)')) {
                        return;
                    }

                    else if (($('#divStatusDest  li').length >= 2) &&
                     ($("#divStatusDest li:nth-last-child(1)").last().prev().find("input").css("background-color") == 'rgb(255, 0, 0)')) {

                        ModuleName = 'status';
                        CommonAlert(ModuleName, ClearDiv);
                    }

                }
            });

            $("#divStatusDest").sortable({
                sort: function () { },
                placeholder: 'ui-state-highlight',
                receive: function () { },
                update: function (event, ui) { }
            });


            $("#divStatusDest input[type=button]").live('dblclick', function () {
                $(this).remove();
                HidePanels();
            })

            $("#divStatusDest").droppable({
                accept: "#Statusleft-pane li",
                accept: ":not(.ui-sortable-helper)",
                drop: function (event, ui) {
                    if ($(ui.draggable).find('.single-item').length == 0) {
                        $(ui.draggable).html("<div style='cursor:move' class='single-item'><input type='button' value='Add Status' id='btnInputAddStatus' style='width:115px;height:45px;background-color:red;' onclick='return ShowStatusProperties();' /><br /></div>");
                    }

                }
            });

            $("#Statusleft-pane").droppable({
                accept: "#divStatusDest",
                drop: function (event, ui) { }
            });

            $("ul, li").disableSelection();
        }

        function AddNotificationNew() {  //Notification Drag and drop

            $("#Notificationleft-pane li").draggable({
                containment: '#divNotiDest',
                //cursor: 'move',
                helper: 'clone',
                scroll: false,
                connectToSortable: '#divNotiDest',
                appendTo: '#divNotiDest',
                revert: "invalid",

                start: function () { },
                stop: function (event, ui) {
                    ClearDiv = '#divNotiDest li:last';
                    if (($('#processright-pane  li').length == 0) ||
                        ($("#processright-pane li").last().css("background-color") == 'rgb(255, 0, 0)')) {
                        ModuleName = 'process';
                        CommonAlert(ModuleName, ClearDiv);

                    }

                    else if (($('#Workflowright-pane  div').length == 0) ||
                        ($("#Workflowright-pane li").last().find("input").css("background-color") == 'rgb(255, 0, 0)')) {
                        ModuleName = 'workflow';
                        CommonAlert(ModuleName, ClearDiv);
                    }

                    else if (($('#Workflowstageright-pane  li').length == 0) ||
                        ($("#Workflowstageright-pane li").last().find("input").css("background-color") == 'rgb(255, 0, 0)')) {
                        ModuleName = 'stages';
                        CommonAlert(ModuleName, ClearDiv);
                    }

                    else if (($('#divNotiDest div').length == 1) && ($("#Workflowstageright-pane li").last().find("input").css("background-color") == 'rgb(0, 128, 0)')) {
                        return;
                    }

                    else if (($('#divNotiDest  li').length >= 2) &&
                     ($("#divNotiDest li:nth-last-child(1)").last().prev().find("input").css("background-color") == 'rgb(255, 0, 0)')) {

                        ModuleName = 'notification';
                        CommonAlert(ModuleName, ClearDiv);
                    }
                }
            });

            $("#divNotiDest").sortable({
                sort: function () { },
                placeholder: 'ui-state-highlight',
                receive: function () { },
                update: function (event, ui) { }
            });


            $("#divNotiDest input[type=button]").live('dblclick', function () {
                $(this).remove();
                HidePanels();
            })

            $("#divNotiDest").droppable({
                accept: "#Notificationleft-pane li",
                accept: ":not(.ui-sortable-helper)",
                drop: function (event, ui) {
                    if ($(ui.draggable).find('.single-item').length == 0) {
                        $(ui.draggable).html("<div style='cursor:move' class='single-item' ><input type='button' value='Add Notification' id='btnInputAddNoti' style='width:115px;height:45px;background-color:red;' onclick='return ShowNotificationProperties();' /><br /></div>");
                    }

                }
            });

            $("#Notificationleft-pane").droppable({
                accept: "#divNotiDest",
                drop: function (event, ui) { }
            });

            $("ul, li").disableSelection();
        }


        function AddFieldsnew() {


            //Drag and drop fields
            $("#StageFieldsleft-pane li").draggable({
                containment: '#divFieldsDest',
                //cursor: 'move',
                helper: 'clone',
                scroll: false,
                connectToSortable: '#divFieldsDest',
                appendTo: '#divFieldsDest',
                revert: "invalid",

                start: function () { },
                stop: function (event, ui) {
                    ClearDiv = '#divFieldsDest li:last';
                    if (($('#processright-pane  li').length == 0) ||
                        ($("#processright-pane li").last().css("background-color") == 'rgb(255, 0, 0)')) {
                        ModuleName = 'process';
                        CommonAlert(ModuleName, ClearDiv);

                    }

                    else if (($('#Workflowright-pane  div').length == 0) ||
                        ($("#Workflowright-pane li").last().find("input").css("background-color") == 'rgb(255, 0, 0)')) {
                        ModuleName = 'workflow';
                        CommonAlert(ModuleName, ClearDiv);
                    }

                    else if (($('#Workflowstageright-pane  li').length == 0) ||
                        ($("#Workflowstageright-pane li").last().find("input").css("background-color") == 'rgb(255, 0, 0)')) {
                        ModuleName = 'stages';
                        CommonAlert(ModuleName, ClearDiv);
                    }

                    else if (($('#divFieldsDest div').length == 1) && ($("#Workflowstageright-pane li").last().find("input").css("background-color") == 'rgb(0, 128, 0)')) {
                        return;
                    }

                    else if (($('#divNotiDest  li').length >= 2) &&
                     ($("#divFieldsDest li:nth-last-child(1)").last().prev().find("input").css("background-color") == 'rgb(255, 0, 0)')) {

                        ModuleName = 'fields';
                        CommonAlert(ModuleName, ClearDiv);
                    }

                }
            });

            $("#divFieldsDest").sortable({
                sort: function () { },
                placeholder: 'ui-state-highlight',
                receive: function () { },
                update: function (event, ui) { }
            });


            $("#divFieldsDest input[type=button]").live('dblclick', function () {
                $(this).remove();
                HidePanels();
            })

            $("#divFieldsDest").droppable({
                accept: "#StageFieldsleft-pane li",
                accept: ":not(.ui-sortable-helper)",
                drop: function (event, ui) {
                    if ($(ui.draggable).find('.single-item').length == 0) {
                        $(ui.draggable).html("<div style='cursor:move' class='single-item' ><input type='button' value='Add Fields' id='btnInputAddNoti' style='width:115px;height:45px;background-color:red;' onclick='return ShowFieldProperties();' /><br /></div>");
                    }

                }
            });

            $("#StageFieldsleft-pane").droppable({
                accept: "#divFieldsDest",
                drop: function (event, ui) { }
            });

            $("ul, li").disableSelection();
        }



        //        //workflow add click
        //        $("#divWorkflowDest li ").click(function () {
        //            alert('add naga');
        //        });
        //validation for stages

        function CheckEmptyTextStageName() {

            var StgName = document.getElementById("<%= txtStageName.ClientID %>");
            var tatValue = parseInt(document.getElementById("<%= txtTATDuration.ClientID %>").value);
            var lblMsg = document.getElementById("<%= lblMessageStage.ClientID %>");
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


        function LoadALLStageDetails() {

        }
      
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="GVDiv">
        <div class="wrapper">
            <table style="border: thin solid;">
                <tr>
                    <td style="border-right: thin  double #ff0000; vertical-align: top; width: 20%; float: none;">
                        <table>
                            <tr>
                                <td>
                                    <b>ToolBar</b>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <div class="accordian" style="position: fixed; margin-left: inherit;">
                                        <h3>
                                            <a href="#">Process</a></h3>
                                        <div class="accordian">
                                            <h3 onclick="ConnectAddNewProcess();">
                                                Add New</h3>
                                            <div>
                                                <ul id="processleft-pane">
                                                    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>Add
                                                        Process </li>   
                                                </ul>
                                            </div>
                                            <h3 onclick="ConnectunconfirmProcess()">
                                                <a href="#">Processes</a></h3>
                                            <div>
                                                <asp:Repeater ID="ReptUnConfirmedProcess" runat="server">
                                                    <HeaderTemplate>
                                                        <ul id="sortableProcessUnconfirm">
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <li class="ui-state-default" id="<%# Eval("ProcessID")%>"><span class="ui-icon ui-icon-arrowthick-2-n-s">
                                                        </span>
                                                            <%#Eval("ProcessName") %></li>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </ul>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </div>
                                           
                                        </div>
                                        <h3>
                                            <a href="#">Workflow</a></h3>
                                        <div class="accordian">
                                            <h3 onclick="AddWorkflowNew();">
                                                <a href="#">Add New</a></h3>
                                            <div id="divWorkflowSource">
                                                <ul id="workflowleft-pane">
                                                    <li>
                                                        <div>
                                                            Add Workflow</div>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                        <h3>
                                            <a href="#">Stages</a></h3>
                                        <div class="accordian">
                                            <h3 onclick="AddStageNew();">
                                                <a href="#">Add Stage</a></h3>
                                            <div id="divStagesSource">
                                                <ul id="workflowstageleft-pane">
                                                    <li>
                                                        <div>
                                                            Add Stages</div>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                        <h3 onclick="AddStatusNew();">
                                            <a href="#">Status</a></h3>
                                        <div class="accordian">
                                            <h3>
                                                Add New</h3>
                                            <div id="divStatusSource">
                                                <ul id="Statusleft-pane">
                                                    <li>
                                                        <div>
                                                            Add Status</div>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                        <h3 onclick="AddFieldsnew()">
                                            <a href="#">Fields</a></h3>
                                        <div class="accordian">
                                            <h3>
                                                Add New</h3>
                                            <div id="divStageFieldsSource">
                                                <ul id="StageFieldsleft-pane">
                                                    <li>
                                                        <div>
                                                            Add Fields</div>
                                                    </li>
                                            </div>
                                        </div>
                                        <h3>
                                            <a href="#">Notifications</a></h3>
                                        <div class="accordian">
                                            <h3 onclick="AddNotificationNew();">
                                                Add New</h3>
                                            <div id="divNotificatonSource">
                                                <ul id="Notificationleft-pane">
                                                    <li>
                                                        <div>
                                                            Add Notification</div>
                                                    </li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: top; width: 30%">
                        <table style="border-right: thin  double #ff0000;">
                            <tr>
                                <b>Process</b>
                                <td>
                                    <div id="divProcessDest">
                                        <ul id="processright-pane" class="fbox">
                                        </ul>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="border-bottom: thick  double;">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Workflow</b>
                                    <div id="divWorkflowDest">
                                        <ul id="Workflowright-pane" class="fbox">
                                        </ul>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="border-bottom: thick  double;">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <b>Stages</b>
                                    <div id="divStageDest">
                                        <ul id="Workflowstageright-pane" class="fbox">
                                        </ul>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="border-bottom: thick  double;">
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="aligntop">
                                                <b>Stage Statuses</b>
                                                <div id="divStatusDest" class="stgdetails">
                                                </div>
                                            </td>
                                            <td class="aligntop">
                                                <b>Stage Fields</b>
                                                <div id="divFieldsDest" class="stgdetails">
                                                </div>
                                            </td>
                                            <td class="aligntop">
                                                <b>Notifications</b>
                                                <div id="divNotiDest" class="stgdetails" style="border: none;">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: top; position: relative;">
                        <b>Properties</b>
                        <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CssClass="btnconfirm" OnClientClick="return CheckallConfiguration();"
                            OnClick="btnConfirm_Click" />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:Panel ID="pnlProcess" runat="server" CssClass="displayNone">
                                    <asp:Button ID="btnProcessEdit" runat="server" OnClick="btnProcessEdit_Click" CssClass="displayNone" />
                                    <div class="GVDiv">
                                        <table>
                                            <tr>
                                                <td colspan="2">
                                                    <h3>
                                                        <asp:Label ID="lblEditProcess" runat="server" Text="Add/Edit Process Details" meta:resourcekey="lblEditProcess" /></h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblMessageProcess" ForeColor="Red" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <table id="tblProcessControls" runat="server">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblOrganization" runat="server" Text="Organization"></asp:Label><span
                                                                    style="color: Red; font-size: medium">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList runat="server" ID="ddlOrganization" AutoPostBack="false">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label runat="server" ID="lblProcessName" meta:resourcekey="lblProcessName" Text="Process Name" />
                                                                <span style="color: Red; font-size: medium">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtProcessName" runat="server" MaxLength="50" onkeyup="return CopyProcesstextToLi();"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblProcessDescription" runat="server" Text="Process Description" meta:resourcekey="lblProcessDescription"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtProcessDescription" runat="server" TextMode="MultiLine" onkeyup="javascript:MaxLength(this, 250)"
                                                                    MaxLength="250"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblProcessActive" runat="server" Text="Active" meta:resourcekey="lblProcessActive"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="chkProcessActive" runat="server" />
                                                            </td>
                                                        </tr>
                                                        
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <div id="accordionProcessUserMapping">
                                                        <div id="divProcessOwner">
                                                            <table runat="server" id="tblProcessOwner" class="displayNone">
                                                                <tr>
                                                                    <td>
                                                                        &nbsp;&nbsp;&nbsp;
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblAvailableUsers_ProcessOwner" runat="server" Text="Available Users"
                                                                            meta:resourcekey="lblAvailableUsers_ProcessOwner" /><br />
                                                                    </td>
                                                                    <td>
                                                                        <asp:ListBox ID="lstAvailableUsers_ProcessOwner" runat="server" Height="150" Width="250">
                                                                        </asp:ListBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                    </td>
                                                                    <td>
                                                                        <center>
                                                                            <asp:Button ID="btnAddUser_ProcessOwner" runat="server" Text="" OnClick="btnAddUser_ProcessOwner_Click"
                                                                                OnClientClick="return validateSelectedItem(ctl00_ContentPlaceHolder1_lstAvailableUsers_ProcessOwner,'Please select a user name to assign',ctl00_ContentPlaceHolder1_lblMessageProcess);"
                                                                                TagName="Add" meta:resourcekey="btnAddUser_ProcessOwner" CssClass="btndownarrow" />
                                                                            <asp:Button ID="btnRemoveUser_ProcessOwner" runat="server" Text="" OnClick="btnRemoveUser_ProcessOwner_Click"
                                                                                OnClientClick="return validateSelectedItem(ctl00_ContentPlaceHolder1_lstAssignedUsers_ProcessOwner,'Please select a user name to remove',ctl00_ContentPlaceHolder1_lblMessageProcess);"
                                                                                TagName="Add" meta:resourcekey="btnRemoveUser_ProcessOwner" CssClass="btnuparrow" />
                                                                        </center>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="lblAssignedUsers_ProcessOwner" runat="server" Text="Assigned Process Owners"
                                                                            meta:resourcekey="lblAssignedUsers_ProcessOwner" /><br />
                                                                    </td>
                                                                    <td>
                                                                        <asp:ListBox ID="lstAssignedUsers_ProcessOwner" runat="server" Height="150" Width="250">
                                                                        </asp:ListBox>
                                                                    </td>
                                                                </tr>

                                                            </table>
                                                        </div>
                                                    </div>
                                                    <br />
                                                    <asp:HiddenField ID="hdnCurrentPanelProcessOwner" runat="server" Value="0" />
                                                    <asp:HiddenField ID="hdnCurrentSubPanelProcessOwner" runat="server" Value="0" />
                                                </td>
                                            </tr>
                                            <tr>
                                                           
                                                            <td colspan="2" class="floatright">
                                                                <asp:Button ID="btnSave" runat="server" Text="Save" OnClientClick="return CheckEmptyTextProcessName();"
                                                                    OnClick="btnProcessSave_Click" CssClass="btnsave" />
                                                                <asp:Button ID="btnProcessCancel" runat="server" Text="Cancel" OnClientClick="return HideProperty(this.id);"
                                                                    OnClick="btnProcessCancel_Click" CssClass="btncancel" />
                                                            </td>
                                                        </tr>
                                        </table>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlWorkflow" runat="server" CssClass="displayNone">
                                    <asp:Button ID="btnWorkflowEdit" runat="server" OnClick="btnWorkflowEdit_Click" CssClass="displayNone" />
                                    <div class="GVDiv">
                                     <asp:Label id="lblMessageWorkflow" runat="server"></asp:Label>
                                        <table>
                                            <tr>
                                                <td colspan="2">
                                                    <h3>
                                                        <asp:Label ID="lblEditWorkFlow" runat="server" Text="Edit Workflow Details" meta:resourcekey="lblEditWorkFlow" /></h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                              
                                       
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <table id="tblWorkflowControls" runat="server">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblDMSProjectName" runat="server" Text="DMS Project Name"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList runat="server" ID="ddlDmsProject">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblWorkflowName" runat="server" Text="Workflow Name" meta:resourcekey="lblWorkflowName"></asp:Label><span
                                                                    style="color: Red; font-size: medium">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtWorkflowName" runat="server" MaxLength="50" onkeyup="return CopyWorkflowToLi();"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblWorkflowDescription" runat="server" Text=" Workflow Description"
                                                                    meta:resourcekey="lblWorkflowDescription"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtWorkflowDescription" runat="server" TextMode="MultiLine" onkeyup="javascript:MaxLength(this, 250)"
                                                                    MaxLength="250"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblWorkflowActive" runat="server" Text="Active" meta:resourcekey="lblWorkflowActive"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="ChkActiveWorkflow" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblSortOrder" runat="server" Text="Sort Order"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:ListBox ID="lstSortOrder" runat="server"></asp:ListBox>
                                                            </td>
                                                            <td style="display: none">
                                                                <input type="button" value="&#x25B2;" onclick="Move_Items('up')" />
                                                                <br></br>
                                                                <input type="button" value="&#x25BC;" onclick="Move_Items('down')" />
                                                            </td>
                                                        </tr>
                                                        
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <table runat="server" id="tblWorkflowOwner" class="displayNone">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblAvailableUsers_WorkflowOwner" runat="server" Text="Available Users"
                                                                    meta:resourcekey="lblAvailableUsers_WorkflowOwner" /><br />
                                                            </td>
                                                            <td>
                                                                <asp:ListBox ID="lstAvailableUsers_WorkflowOwner" runat="server" Height="150" Width="250">
                                                                </asp:ListBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                            </td>
                                                            <td>
                                                                <center>
                                                                    <asp:Button ID="btnAddUser_WorkflowOwner" TagName="Add" runat="server" Text="" OnClick="btnAddUser_WorkflowOwner_Click"
                                                                        meta:resourcekey="btnAddUser_WorkflowOwner" CssClass="btndownarrow" OnClientClick="return validateSelectedItem(ctl00_ContentPlaceHolder1_lstAvailableUsers_WorkflowOwner,'Please select a user name to assign',ctl00_ContentPlaceHolder1_lblworkflowProcess);" />
                                                                    <asp:Button ID="btnRemoveUser_WorkflowOwner" TagName="Add" runat="server" Text=""
                                                                        OnClick="btnRemoveUser_WorkflowOwner_Click" meta:resourcekey="btnRemoveUser_WorkflowOwner"
                                                                        CssClass="btnuparrow" OnClientClick="return validateSelectedItem(ctl00_ContentPlaceHolder1_lstAvailableUsers_WorkflowOwner,'Please select a user name to remove',ctl00_ContentPlaceHolder1_lblWorkflowProcess);" />
                                                                </center>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblAssignedUsers_WorkflowOwner" runat="server" Text="Assigned Workflow Owners"
                                                                    meta:resourcekey="lblAssignedUsers_WorkflowOwner" /><br />
                                                            </td>
                                                            <td>
                                                                <asp:ListBox ID="lstAssignedUsers_WorkflowOwner" runat="server" Height="150" Width="250">
                                                                </asp:ListBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                          
                                        </table>

                                        <div> <asp:Button ID="btnWorkflowSave" runat="server" Text="Save" OnClientClick="return CheckEmptyTextWorkflowName();"
                                                                    OnClick="btnWorkflowSave_Click" CssClass="btnsave" /> 
                                                                <asp:Button ID="btnWorkflowCancel" runat="server" Text="Cancel" OnClientClick="return HideProperty(this.id);"
                                                                    OnClick="btnWorkflowCancel_Click" CssClass="btncancel" /></div>
                                        
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlStages" runat="server" CssClass="displayNone">
                                    <asp:Button ID="btnStageEdit" runat="server" OnClick="btnStageEdit_Click" CssClass="displayNone" />
                                    <div class="GVDiv">
                                        <table>
                                            <tr>
                                                <td colspan="2">
                                                    <h3>
                                                        <asp:Label ID="lblEditStageDetails" runat="server" Text="Edit Stage Details" meta:resourcekey="lblEditStageDetails" /></h3>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <asp:Label ID="lblMessageStage" ForeColor="Red" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblStageName" runat="server" Text="Stage Name" meta:resourcekey="lblStageName"></asp:Label><span
                                                        style="color: Red; font-size: medium">*</span>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtStageName" runat="server" MaxLength="50" onkeyup="return CopyStagesToLi();"></asp:TextBox>
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
                                                    <asp:CheckBox ID="chkStageActive" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                        <table>
                                                    <tr>
                                                        <td colspan="2">
                                                            <h3>
                                                                Stage Owner Mapping</h3>
                                                            <%-- <b>
                                                    <asp:Label ID="lblHdrStageOwnerMapping" runat="server" Text="Stage Owner Mapping"
                                                        meta:resourcekey="lblHdrStageOwnerMapping" /></b>--%>
                                                            <div id="divStageOwner">
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblAvailableUsers_StageOwner" runat="server" Text="Available Users"
                                                                                meta:resourcekey="lblAvailableUsers_StageOwner" /><br />
                                                                        </td>
                                                                        <td>
                                                                            <asp:ListBox ID="lstAvailableUsers_StageOwner" runat="server" Height="150" Width="250">
                                                                            </asp:ListBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                        </td>
                                                                        <td>
                                                                            <center>
                                                                                <asp:Button ID="btnAddUser_StageOwner" TagName="Add" runat="server" Text="" OnClick="btnAddUser_StageOwner_Click"
                                                                                    meta:resourcekey="btnAddUser_StageOwner" CssClass="btndownarrow" OnClientClick="return validateSelectedItem(ctl00_ContentPlaceHolder1_lstAvailableUsers_StageOwner,'Please select a user name to assign',ctl00_ContentPlaceHolder1_lblMessageStage);" />
                                                                                <asp:Button ID="btnRemoveUser_StageOwner" TagName="Add" runat="server" Text="" OnClick="btnRemoveUser_StageOwner_Click"
                                                                                    meta:resourcekey="btnRemoveUser_StageOwner" CssClass="btnuparrow" />
                                                                            </center>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblAssignedUsers_StageOwner" runat="server" Text="Assigned Stage Owners"
                                                                                meta:resourcekey="lblAssignedUsers_StageOwner" /><br />
                                                                        </td>
                                                                        <td>
                                                                            <asp:ListBox ID="lstAssignedUsers_StageOwner" runat="server" Height="150" Width="250">
                                                                            </asp:ListBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                            <b>
                                                                <asp:Label ID="lblHdrStageUserMapping" runat="server" Text="Stage User Mapping" meta:resourcekey="lblHdrStageUserMapping" /></b>
                                                            <div id="divUserMain" class="accordian">
                                                                <div id="accordionStageUserMapping">
                                                                    <div id="divUserSub">
                                                                        <table>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblAvailableUsers_StageUser" runat="server" Text="Available Users"
                                                                                        meta:resourcekey="lblAvailableUsers_StageUser" />
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ListBox ID="lstAvailableUsers_StageUser" runat="server" Height="150" Width="250">
                                                                                    </asp:ListBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                </td>
                                                                                <td>
                                                                                    <center>
                                                                                        <asp:Button ID="btnAddUser_StageUser" TagName="Add" runat="server" Text="" OnClick="btnAddUser_StageUser_Click"
                                                                                            meta:resourcekey="btnAddUser_StageUser" CssClass="btndownarrow" OnClientClick="return validateSelectedItem(ctl00_ContentPlaceHolder1_lstAvailableUsers_StageUser,'Please select a user name to assign',ctl00_ContentPlaceHolder1_lblMessageStage);" />
                                                                                        <asp:Button ID="btnRemoveUser_StageUser" TagName="Add" runat="server" Text="" OnClick="btnRemoveUser_StageUser_Click"
                                                                                            meta:resourcekey="btnRemoveUser_StageUser" CssClass="btnuparrow" />
                                                                                    </center>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblAssignedUsers_StageUser" runat="server" Text="Assigned Stage Users"
                                                                                        meta:resourcekey="lblAssignedUsers_StageUser" />
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ListBox ID="lstAssignedUsers_StageUser" runat="server" Height="150" Width="250">
                                                                                    </asp:ListBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                    <asp:Label ID="lblHdrStageUserMapping_Sub_Group" runat="server" Text="User Groups"
                                                                        meta:resourcekey="lblHdrStageUserMapping_Sub_Group" />
                                                                    <div id="divUserGroupSub">
                                                                        <table>
                                                                            <tr>
                                                                                <td>
                                                                                    <asp:Label ID="lblAvailableUsers_StageUserGroup" runat="server" Text="Available UserGroups"
                                                                                        meta:resourcekey="lblAvailableUsers_StageUserGroup" />
                                                                                    <br />
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ListBox ID="lstAvailableUsers_StageUserGroup" runat="server" Height="150" Width="250">
                                                                                    </asp:ListBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                </td>
                                                                                <td>
                                                                                    <center>
                                                                                        <asp:Button ID="btnAddUser_StageUserGroup" TagName="Add" runat="server" Text="" OnClick="btnAddUser_StageUserGroup_Click"
                                                                                            meta:resourcekey="btnAddUser_StageUserGroup" CssClass="btndownarrow" />
                                                                                        <asp:Button ID="btnRemoveUser_StageUserGroup" TagName="Add" runat="server" Text=""
                                                                                            OnClick="btnRemoveUser_StageUserGroup_Click" meta:resourcekey="btnRemoveUser_StageUserGroup"
                                                                                            CssClass="btnuparrow" />
                                                                                    </center>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td>
                                                                                    <div>
                                                                                        <asp:Label ID="lblAssignedUsers_StageUserGroup" runat="server" Text="Assigned Stage UserGroups"
                                                                                            meta:resourcekey="lblAssignedUsers_StageUserGroup" />
                                                                                    </div>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:ListBox ID="lstAssignedUsers_StageUserGroup" runat="server" Height="150" Width="250">
                                                                                    </asp:ListBox>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <b>
                                                                <asp:Label ID="lblHdrNotificationUserMapping" runat="server" Text="Notification User Mapping"
                                                                    meta:resourcekey="lblHdrNotificationUserMapping" /></b>
                                                            <div id="divNotification" class="accordian">
                                                                <table>
                                                                    <tr>
                                                                        <td>
                                                                            <div>
                                                                                <asp:Label ID="lblAvailableUsers_NotoficationUser" runat="server" Text="Available Users"
                                                                                    meta:resourcekey="lblAvailableUsers_NotoficationUser" />
                                                                            </div>
                                                                        </td>
                                                                        <td>
                                                                            <asp:ListBox ID="lstAvailableUsers_NotoficationUser" runat="server" Height="150"
                                                                                Width="250"></asp:ListBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                        </td>
                                                                        <td>
                                                                            <center>
                                                                                <asp:Button ID="btnAddUser_NotoficationUser" TagName="Add" runat="server" Text=""
                                                                                    OnClick="btnAddUser_NotoficationUser_Click" meta:resourcekey="btnAddUser_NotoficationUser"
                                                                                    CssClass="btndownarrow" OnClientClick="return validateSelectedItem(ctl00_ContentPlaceHolder1_lstAvailableUsers_NotoficationUser,'Please select a user name to assign',ctl00_ContentPlaceHolder1_lblMessageStage);" />
                                                                                <asp:Button ID="btnRemoveUser_NotoficationUser" TagName="Add" runat="server" Text=""
                                                                                    OnClick="btnRemoveUser_NotoficationUser_Click" meta:resourcekey="btnRemoveUser_NotoficationUser"
                                                                                    CssClass="btnuparrow" /></center>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td>
                                                                            <asp:Label ID="lblAssignedUsers_NotoficationUser" runat="server" Text="Assigned Notification Users"
                                                                                meta:resourcekey="lblAssignedUsers_NotoficationUser" /><br />
                                                                        </td>
                                                                        <td>
                                                                            <asp:ListBox ID="lstAssignedUsers_NotoficationUser" runat="server" Height="150" Width="250">
                                                                            </asp:ListBox>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                            <asp:HiddenField ID="hdnCurrentPanel" runat="server" Value="0" />
                                                            <asp:HiddenField ID="hdnCurrentSubPanel" runat="server" Value="0" />
                                                        </td>
                                                    </tr>
                                                </table>
                                        <asp:HiddenField ID="hdnFileUpload" runat="server" />
                                        <asp:HiddenField ID="hdnStageTemplatePath" runat="server" />
                                        <asp:HiddenField ID="hdnPagesCount" runat="server" />
                                        <asp:HiddenField ID="hdnPageNo" runat="server" />
                                        <asp:HiddenField ID="hdnDataentryId" runat="server" />
                                        <asp:HiddenField ID="hdnDataEntryType" runat="server" />
                                        <asp:HiddenField ID="hdnDataEntryName" runat="server" />
                                        <asp:HiddenField ID="hdnStageName" runat="server" />
                                        <div style="float: right;">
                                            <asp:Button ID="btnStageSave" runat="server" Text="Save" TagName="Add" OnClick="btnStageSave_Click"
                                                OnClientClick="return CheckEmptyTextStageName();" meta:resourcekey="btnSave"
                                                CssClass="btnsave" />
                                            <asp:Button ID="btnStageCancel" runat="server" TagName="Read" Text="Cancel" OnClientClick="return HideProperty(this.id);"
                                                CausesValidation="false" meta:resourcekey="btCancel" CssClass="btncancel" OnClick="btnStageCancel_Click" />
                                            <asp:Button ID="btnDataEntryClick" runat="server" Text="" Visible="false" OnClientClick="EnableControl();return false;"
                                                TagName="Add" />
                                            <asp:Button ID="btnStageSubmit" runat="server" CssClass="MnuAdminClear" OnClick="btnStageSubmit_Click"
                                                TagName="Add" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlStagesAdd" runat="server" class="displayNone">
                                    <asp:Button ID="btnStageAdd" runat="server" OnClick="btnStageAdd_Click" CssClass="displayNone" />
                                    <div class="GVDiv">
                                        <div id="Content">
                                            <h3>
                                                <asp:Label ID="lblAvailableStages" runat="server" Text="Available Stages" meta:resourcekey="lblAvailableStages"></asp:Label>
                                            </h3>
                                            <div style="overflow-y: auto;">
                                                <asp:GridView ID="gridStageMaster" runat="server" AutoGenerateColumns="true" CssClass="mGrid"
                                                    PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" OnRowDataBound="gridStageMaster_RowDataBound"
                                                    EmptyDataText="Stages Are Not Available">
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
                                            <asp:Button ID="btnOk" runat="server" OnClick="btnStageAddOk_Click" Text="OK" CausesValidation="false"
                                                meta:resourcekey="btnOk" CssClass="btnsave" TagName="Add" />
                                            <asp:Button ID="btnStageAddCancel" runat="server" Text="Cancel" OnClientClick="return HideMD();"
                                                OnClick="btnStageAddCancel_Click" meta:resourcekey="btnCancel" CssClass="btncancel"
                                                TagName="Read" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlStatus" runat="server" class="displayNone">
                                    <asp:Button ID="btnStatusEdit" runat="server" OnClick="btnStatusEdit_Click" CssClass="displayNone" />
                                    <div class="GVDiv">
                                        <h3>
                                            <asp:Label ID="lblAvailableStatuses" runat="server" Text="Available Status" meta:resourcekey="lblAvailableStatuses" /></h3>
                                        <div style="overflow-y: auto;">
                                            <asp:GridView ID="gridStatusMaster" runat="server" AutoGenerateColumns="true" AllowPaging="true"
                                                CssClass="mGrid" PagerStyle-CssClass="pgr" OnRowDataBound="gridStatusMaster_RowDataBound"
                                                AlternatingRowStyle-CssClass="alt" EmptyDataText="Master statuses are not available to select"
                                                OnPageIndexChanging="gridStatusMaster_PageIndexChanging" PageSize="10">
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
                                                        <asp:Label ID="lblStatusMessage" ForeColor="Red" runat="server"></asp:Label>
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
                                                        <asp:CheckBox ID="ChkStatusActive" runat="server" />
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
                                        </div>
                                        <div style="float: right;">
                                            <asp:Button ID="btnStatusSave" runat="server" TagName="Add" Text="Save" OnClick="btnStatusSave_Click"
                                                OnClientClick="return CheckEmptyTextStatusName();" meta:resourcekey="btnEdit_Save"
                                                CssClass="btnsave" />
                                            <asp:Button ID="btnStatusCancel" TagName="Read" runat="server" Text="Cancel" OnClientClick="return HideProperty(this.id);"
                                                OnClick="btnStatusCancel_Click" CausesValidation="false" meta:resourcekey="btnEdit_Cancel"
                                                CssClass="btncancel" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlStatusAdd" runat="server" class="displayNone">
                                    <asp:Button ID="btnStatusAdd" runat="server" OnClick="btnStatusAdd_Click" CssClass="displayNone" />
                                    <div class="GVDiv">
                                        <h3>
                                            <asp:Label ID="lblAddStages" runat="server" Text="Available Status" meta:resourcekey="lblAvailableStatuses" /></h3>
                                        <div style="overflow-y: auto;">
                                            <asp:GridView ID="gridStatusMasterAdd" runat="server" AutoGenerateColumns="true"
                                                AllowPaging="true" CssClass="mGrid" PagerStyle-CssClass="pgr" OnRowDataBound="gridStatusMaster_RowDataBound"
                                                AlternatingRowStyle-CssClass="alt" EmptyDataText="Master statuses are not available to select"
                                                OnPageIndexChanging="gridStatusMasterAdd_PageIndexChanging" PageSize="10">
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
                                        <div style="float: right;">
                                            <asp:Button ID="btnStatusOk" runat="server" OnClick="btnStatusOk_Click" Text="OK"
                                                CausesValidation="false" meta:resourcekey="btnAdd_OK" CssClass="btnsave" TagName="Add" />
                                            <asp:Button ID="btnStatusAddCancel" runat="server" Text="Cancel" OnClientClick="return HideProperty(this.id);"
                                                meta:resourcekey="btnAdd_Cancel" CssClass="btncancel" TagName="Read" OnClick="btnStatusAddCancel_Click" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlNotifications" runat="server" class="displayNone">
                                    <asp:Button ID="btnNotificationAdd" runat="server" OnClick="btnNotificationAdd_Click"
                                        CssClass="displayNone" />
                                    <asp:Button ID="btnNotificationEdit" runat="server" OnClick="btnNotificationEdit_Click"
                                        CssClass="displayNone" />
                                    <div class="GVDiv">
                                        <div>
                                            <table>
                                                <tr>
                                                    <td colspan="2">
                                                        <h3>
                                                            <asp:Label ID="lblEditNotificationDetails" runat="server" Text="Edit Notification Details"
                                                                meta:resourcekey="lblEditNotificationDetails" /></h3>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:Label ID="lblNotificationMessage" ForeColor="Red" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label runat="server" ID="lblNotificationStatus" Text="Stage Status" meta:resourcekey="lblNotificationStatus" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddl_Status" runat="server">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblNotificationCategory" runat="server" Text="Category" meta:resourcekey="lblNotificationCategory"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddl_Category" runat="server" onchange="setNotifyTat()">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblNotificationTAT" runat="server" Text="TAT (in minutes)" meta:resourcekey="lblNotificationTAT"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txt_TAT" runat="server" onkeypress="return IsNumeric(event);" MaxLength="4"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblNotificationActive" runat="server" Text="Active" meta:resourcekey="lblNotificationActive"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chkNotificationActive" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div style="float: right;">
                                            <asp:Button ID="btnNotificationSave" runat="server" Text="Save" OnClientClick="javascript:return validateDetails();"
                                                meta:resourcekey="btnSave" CssClass="btnsave" TagName="Add" OnClick="btnNotificationSave_Click" />
                                            <asp:Button ID="btnNotificationCancel" runat="server" Text="Cancel" OnClientClick="return HideProperty(this.id);"
                                                meta:resourcekey="btnCancel" CssClass="btncancel" TagName="Read" OnClick="btnNotificationCancel_Click" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <!--Using Iframe for fields html start-->
                                <div id="modelBG" class="mdNone">
                                </div>
                                <div id="mb" class="mdNone">
                                    <div>
                                        <asp:Button ID="btnStageFieldEdit" runat="server" OnClick="btnStageFieldEdit_Click"
                                            CssClass="displayNone" />
                                        <asp:ImageButton ID="ImageCloseButton" runat="server" ImageUrl="~/Images/DeleteIcon.png"
                                            Style="float: right; margin-top: -10px;" OnClientClick="return HideFieldsProperties();" />
                                        <iframe id="frameStageProperties" width="1285px" height="500px" scrolling="auto">
                                        </iframe>
                                    </div>
                                </div>
                                <!--Using Iframe for fields html End-->
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <asp:HiddenField ID="hdnSaveStatus" runat="server" />
    <asp:HiddenField ID="hiddenProcessId" runat="server" />
    <asp:HiddenField ID="hiddenWorkflowId" runat="server" />
    <asp:HiddenField ID="hiddenStageId" runat="server" Value="" />
    <asp:HiddenField ID="hdnStageFieldId" runat="server" Value="" />
    <asp:HiddenField ID="hdnStatusId" runat="server" Value="" />
    <asp:HiddenField ID="hdnNotificationId" runat="server" Value="" />
    <asp:HiddenField ID="hiddenOrganizationId" runat="server" />
    <asp:HiddenField ID="hdnErrorStatus" runat="server" Value="" />
    <asp:HiddenField ID="hdnSaveFlagStatus" runat="server" />
    <asp:HiddenField ID="hdnCopytoli" runat="server" />
    <asp:HiddenField ID="hdnWorkFlowCount" runat="server" />
    <asp:HiddenField ID="hdnAction" runat="server" />
    <asp:HiddenField ID="hdnFieldsvalues" runat="server" />
    <asp:HiddenField ID="hdnnotificationvalues" runat="server" />
      <asp:HiddenField ID="hdnstatusvalues" runat="server" />
</asp:Content>
