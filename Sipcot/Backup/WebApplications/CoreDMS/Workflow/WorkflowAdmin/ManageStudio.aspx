<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageStudio.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageStudio" %>

<%@ Register Assembly="AjaxControlToolKit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../DragAndDrop/Javascripts/jquery-ui-1.9.2.custom.min.js" type="text/javascript"></script>
    <link href="../DragAndDrop/css/jquery-ui-1.9.2.custom.min.css" rel="stylesheet" type="text/css" />
    <link href="<%= Page.ResolveClientUrl("~/SiteStyle.css") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {

            //Accordian
            $("div.accordian").accordion({
                autoHeight: false,
                collapsible: true,
                active: false

            });


        });
    </script>
    <script type="text/javascript">
        var appendtoAccordian = "";

        function ConnectAddNewProcess() {

            //process Unconfirm

            SetFromValue("AddProcess", "Process");

            $("#processleft-pane,#processright-pane").sortable({
                connectWith: "#processleft-pane,#processright-pane",

                stop: function () {
                    $('#divProcessDest li').addClass('addnew');
                    ShowProcessProperties();

                },
                receive: function (event, ui) { },
                revert: true

            });
            $("#processleft-pane,#processright-pane").disableSelection();

            $("#processright-pane").on("sortreceive", function (event, ui) {
                var $list = $(this);

                if ($list.children().length > 1) {
                    alert('Please save the selected process');
                    $(ui.sender).sortable('cancel');

                }


            });
        }


        function ConnectunconfirmProcess() {

            //process Unconfirm
            SetFromValue("EditProcess", "Process");

            $("#sortableProcessUnconfirm,#processright-pane").sortable({
                connectWith: "#sortableProcessUnconfirm,#processright-pane",

                stop: function () {
                    $("div.accordian").accordion({ header: "h3", collapsible: true, active: false });
                    $('#divProcessDest li').addClass('addnew');
                    document.getElementById("<%= hiddenProcessSaveStatus.ClientID %>").value = "Yes";
                    ShowProcessProperties();
                   
                },
                receive: function (event, ui) { }


            });
            $("#sortableProcessUnconfirm,#processright-pane").disableSelection();

            $("#processright-pane").on("sortreceive", function (event, ui) {
                var $list = $(this);



                if ($list.children().length > 1) {
                    alert('Please save the selected Workflow');
                    $(ui.sender).sortable('cancel');

                }

            });
        }

        function SetFromValue(value, Action) {
            if (Action == "Process") {
                var Processcount = $("#processright-pane").children().length;
                if (Processcount < 1) {
                    // appendtoAccordian = value;
                    document.getElementById("<%= hdnActionProcess.ClientID %>").value = value;
                    // CheckAddOREdit();
                }
            }




        }

        //Process Section


        //On Process Edit li click
        $(function () {
            $("#processright-pane").on('click', 'li', function (event) {
                var hdnprocessaction = document.getElementById("<%= hdnActionProcess.ClientID %>");
                var hiddenProcessId = document.getElementById("<%= hiddenProcessId.ClientID %>");
                document.getElementById("<%= hiddenProcessSaveStatus.ClientID %>").value = "Yes";

                id = $(this).attr("id");

                $("#processright-pane li").css({
                    backgroundColor: ''
                });

                $(this).addClass('addnew');
                if (hdnprocessaction.value == "EditProcess") {
                    hiddenProcessId.value = id;
                    document.getElementById('<%= btnProcessEdit.ClientID %>').click();
                    ShowProcessProperties();
                }


            });
        });


        function ShowProcessProperties() {
            var hiddenProcessId = document.getElementById("<%= hiddenProcessId.ClientID %>");
            var hdnprocessaction = document.getElementById("<%= hdnActionProcess.ClientID %>");
            var pnlP = document.getElementById("<%= pnlProcess.ClientID %>");

            var lblmsg = document.getElementById("<%= lblMessageProcess.ClientID %>");

            lblmsg.innerHTML = "";
            var chkProcessActive = document.getElementById("<%= chkProcessActive.ClientID %>");
            chkProcessActive.checked = true;
            chkProcessActive.disabled = true;

            if (hdnprocessaction.value == "EditProcess") {
                var Id = $("#processright-pane li").last().attr('id');
                hiddenProcessId.value = Id;

                document.getElementById('<%= btnProcessEdit.ClientID %>').click();

            }


            pnlP.style.display = "block";
            // DisableSaveButtons();
        }


        function CheckEmptyTextProcessName() {

            var ddlOrganization = document.getElementById("<%= ddlOrganization.ClientID %>");
            var ProName = document.getElementById("<%= txtProcessName.ClientID %>");
            var ProDesp = document.getElementById("<%= txtProcessDescription.ClientID %>");
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

        function HidePanels(action) {
            //Hideing panels
            var pnlProcess = document.getElementById("<%= pnlProcess.ClientID %>");
            var pnlWorkflow = document.getElementById("<%= pnlWorkflow.ClientID %>");
            var pnlStages = document.getElementById("<%= pnlStages.ClientID %>");
            var pnlStagesAdd = document.getElementById("<%= pnlStagesAdd.ClientID %>");
            var pnlStatusAdd = document.getElementById("<%= pnlStatusAdd.ClientID %>");
            var pnlStatus = document.getElementById("<%= pnlStatus.ClientID %>");
            var pnlNotifications = document.getElementById("<%= pnlNotifications.ClientID %>");

            if (action == "Process") {
                pnlProcess.style.display = 'block';
                pnlWorkflow.style.display = 'none';
                pnlStages.style.display = 'none';
                pnlStatusAdd.style.display = 'none';
                pnlStatus.style.display = 'none';
                pnlNotifications.style.display = 'none';
                pnlStagesAdd.style.display = 'none';
            }
            if (action == "Workflow") {
                pnlProcess.style.display = 'none';
                pnlWorkflow.style.display = 'block';
                pnlStages.style.display = 'none';
                pnlStatusAdd.style.display = 'none';
                pnlStatus.style.display = 'none';
                pnlNotifications.style.display = 'none';
                pnlStagesAdd.style.display = 'none';
            }

            if (action == "Stages") {
                pnlProcess.style.display = 'none';
                pnlWorkflow.style.display = 'none';
                pnlStages.style.display = 'block';
                pnlStatusAdd.style.display = 'none';
                pnlStatus.style.display = 'none';
                pnlNotifications.style.display = 'none';
                pnlStagesAdd.style.display = 'none';
            }
            if (action == "StatusAdd") {
                pnlProcess.style.display = 'none';
                pnlWorkflow.style.display = 'none';
                pnlStages.style.display = 'none';
                pnlStatusAdd.style.display = 'block';
                pnlStatus.style.display = 'none';
                pnlNotifications.style.display = 'none';
                pnlStagesAdd.style.display = 'none';
            }

            if (action == "Status") {
                pnlProcess.style.display = 'none';
                pnlWorkflow.style.display = 'none';
                pnlStages.style.display = 'none';
                pnlStatusAdd.style.display = 'none';
                pnlStatus.style.display = 'block';
                pnlNotifications.style.display = 'none';
                pnlStagesAdd.style.display = 'none';
            }
            if (action == "Notification") {
                pnlProcess.style.display = 'none';
                pnlWorkflow.style.display = 'none';
                pnlStages.style.display = 'none';
                pnlStatusAdd.style.display = 'none';
                pnlStatus.style.display = 'none';
                pnlNotifications.style.display = 'block';
                pnlStagesAdd.style.display = 'none';
            }

            if (action == "None") {
                pnlProcess.style.display = 'none';
                pnlWorkflow.style.display = 'none';
                pnlStages.style.display = 'none';
                pnlStatusAdd.style.display = 'none';
                pnlStatus.style.display = 'none';
                pnlNotifications.style.display = 'none';
                pnlStagesAdd.style.display = 'none';
            }

        }



        function HideProperty(buttonid) {
            var hdnprocessaction = document.getElementById("<%= hdnActionProcess.ClientID %>");
            var hdnActionWorkflow = document.getElementById("<%= hdnActionWorkflow.ClientID %>");
            var btnProcessCancel = document.getElementById("<%= btnProcessCancel.ClientID %>");
            var btnWorkflowCancel = document.getElementById("<%= btnWorkflowCancel.ClientID %>");
            var btnStageAddCancel = document.getElementById("<%= btnStageAddCancel.ClientID %>");
            var btnStageCancel = document.getElementById("<%= btnStageCancel.ClientID %>");
            var btnStatusAddCancel = document.getElementById("<%= btnStatusAddCancel.ClientID %>");
            var btnStatusCancel = document.getElementById("<%= btnStatusCancel.ClientID %>");
            var btnNotificationCancel = document.getElementById("<%= btnNotificationCancel.ClientID %>");

            var processleftpane = $("#processleft-pane");

            if (btnProcessCancel.id === buttonid) {

                HidePanels("Process");
                $('#divProcessDest li').removeClass('addnew');
                var processplaneX = $('#processright-pane  li');
                var processplaneleft = $('#processleft-pane  li').length;
                $(processplaneX).remove();

                if (hdnprocessaction.value == "AddProcess") {

                    processleftpane.append($('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+ Add Process </li>'));

                }
                $("#workflowleft-pane li").remove();
                $("#workflowleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span> + Add Workflow </li>');
                $('#divWorkflowDest li').remove();
                if (hdnprocessaction.value == "EditProcess") {
                    $("#sortableProcessUnconfirm").append(('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span> ' + processplaneX[0].innerText + ' </li>'));

                    if (processplaneleft == 0) {
                        if ($("#processleft-pane").children().length == 0) {
                            $("#processleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+ Add Process </li>');
                        }
                    }
                }

                RemoveElementsfromDiv("Process");
            }

            if (btnWorkflowCancel.id === buttonid) {
                HidePanels("Workflow");
                var Workflowpanel = $('#Workflowright-pane  li');
                $(Workflowpanel).remove();

                if (hdnActionWorkflow.value == "AddWorkflow") {
                    if ($("#workflowleft-pane").children().length == 0) {

                        $("#workflowleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+ Add Workflow </li>');
                        $('#divWorkflowDest li').remove();
                    }

                }

                RemoveElementsfromDiv("Workflow");

            }

            if (btnStageCancel.id === buttonid || btnStageAddCancel.id === buttonid) {
                HidePanels("Stages");
                if ($("#workflowstageleft-pane").children().length == 0) {
                    $("#workflowstageleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+ Add Stages </li>');
                    $('#divStageDest li').remove();
                }
                RemoveElementsfromDiv("Stages");
            }

            if (btnStatusAddCancel.id === buttonid) {
                HidePanels("StatusAdd");
                if ($("#Statusleft-pane").children().length == 0) {
                    $("#Statusleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+ Add Status </li>');
                    $('#divStatusDest li').remove();
                }
                RemoveElementsfromDiv("StatusAdd");
            }

            if (btnStatusCancel.id === buttonid) {
                HidePanels("Status");
                if ($("#Statusleft-pane").children().length == 0) {
                    $("#Statusleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+ Add Status </li>');
                    $('#divStatusDest li').remove();
                }
                RemoveElementsfromDiv("Status");
            }

            if (btnNotificationCancel.id === buttonid) {
                HidePanels("Notification");
                if ($("#Notificationleft-pane").children().length == 0) {
                    $("#Notificationleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+ Add Notification </li>');
                    $('#divNotiDest li').remove();
                }
                RemoveElementsfromDiv("Notification");
            }
        }


        //Check Maxlength
        function MaxLength(txtbox, maxLength) {
            if (txtbox.value.length >= maxLength) {
                txtbox.value = txtbox.value.substring(0, maxLength - 1);
            }
        }

        //Building sections
        function BuildProcess(Values) {
            var hiddenPId = document.getElementById('<%= hiddenProcessId.ClientID %>');

            var div = "#processright-pane";
            CommonSplitAndBind(div, ShowProcessProperties, Values);
            GoBackToProcess();
            //DisableSaveButtons();
        }

        function CommonSplitAndBind(div, callFunction, Values) {
            $(div).empty();
            if (Values != "") {
                var strarray = Values.split(',');



                for (var i = 0; i < strarray.length; i++) {

                    var PId = strarray[i].split('-').splice(0, 1).join("")
                    var Pvalue = strarray[i].split('-').splice(1, 2).join("")
                    $(div).append('<li class="addnew" id=' + PId + '><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>' + Pvalue + '</li>');


                }

            }
            //            $button.addClass("eWorkflow");
            // ChangeBackgroundColor(div);

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
                $('#divStatusDest input').css("background-color", "rgb(221, 221, 221)");
            }
            else if (action == "Fields") {
                $('#divFieldsDest input').css("background-color", "rgb(221, 221, 221)");
            }
            else if (action == "Notification") {
                $('#divNotiDest  input').css("background-color", "rgb(221, 221, 221)");
            }


        }

        //Change li text as we change in process textbox
        function CopyProcesstextToLi() {
            var hdnprocessaction = document.getElementById("<%= hdnActionProcess.ClientID %>")
            if (hdnprocessaction.value == "EditProcess") {
                var hiddenProcessId = document.getElementById("<%= hiddenProcessId.ClientID %>").value;
                $('#processright-pane li').find('#' + hiddenProcessId).val($("#<%= txtProcessName.ClientID %>").val());
                return false;
            }
        }




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

        //Validate Owners
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


        //section Workflow
        // DMS WorkFLow 1
        function BuildWorkflows(Values) {
            //  Haschild = Values;


            var div = "#Workflowright-pane";
            CommonSplitAndBindInput(div, Values);
            GetBackWorkflow();
            WorkFlowClick1();
            //DisableSaveButtons();
        }


        //DMS Workflow 2

        //On Workflow li click

        function WorkFlowClick() {
            $("#divWorkflowDest").on('click', 'li', function () {

                var hdnWorkflowaction = document.getElementById("<%= hdnActionWorkflow.ClientID %>");
                var hiddenWorkflowId = document.getElementById("<%= hiddenWorkflowId.ClientID %>");

                id = $(this).attr("id");

                document.getElementById('<%= hiddenWorkflowId.ClientID %>').value = id;

                $("#divWorkflowDest input").css({
                    backgroundColor: ''
                });
                $("#divWorkflowDest li").css({
                    backgroundColor: ''
                });
                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });
                hiddenWorkflowId.value = id;
                CheckAddorEditWorkflow();

                if (hdnWorkflowaction.value == "EditWorkflow") {

                    document.getElementById('<%= btnWorkflowEdit.ClientID %>').click();

                }
                ShowWorkflowProperties();

            });
        }
        function WorkFlowClick1() {
            $("#divWorkflowDest").on('click', 'input', function () {

                var hdnWorkflowaction = document.getElementById("<%= hdnActionWorkflow.ClientID %>");
                var hiddenWorkflowId = document.getElementById("<%= hiddenWorkflowId.ClientID %>");
                document.getElementById("<%= hiddenWorkflowSaveStatus.ClientID %>").value = "Yes";
                id = $(this).attr("id");

                document.getElementById('<%= hiddenWorkflowId.ClientID %>').value = id;

                $("#divWorkflowDest input").css({
                    backgroundColor: ''
                });

                $("#divWorkflowDest li").css({
                    backgroundColor: 'rgb(221, 221, 221)'
                });

                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });
                hiddenWorkflowId.value = id;
                CheckAddorEditWorkflow();

                if (hdnWorkflowaction.value == "EditWorkflow") {

                    document.getElementById('<%= btnWorkflowEdit.ClientID %>').click();

                }
                ShowWorkflowProperties();

                RemoveElementsfromDiv("Stages");
            });
        }
        //Workflow 3
        function AddWorkflowNew() {

            SetFromValue("AddWorkflow", "Workflow");

            $("#workflowleft-pane,#Workflowright-pane").sortable({
                connectWith: "#workflowleft-pane,#Workflowright-pane",

                stop: function () {
                    $('#divWorkflowDest li').addClass('addnew');
                    WorkFlowClick();
                },
                receive: function (event, ui) {
                },
                revert: true


            });
            $("#workflowleft-pane,#Workflowright-pane").disableSelection();

            $("#Workflowright-pane").on("sortreceive", function (event, ui) {
                var $list = $(this);


                var ProcessSaveStatus = document.getElementById("<%= hiddenProcessSaveStatus.ClientID %>");
                if (($('#processright-pane').children().length == 0) || (ProcessSaveStatus.value != "Yes")) {
                    alert('Please save/click the  process');
                    HidePanels("None");
                    $(ui.sender).sortable('cancel');
                    return false;
                }
            });

        }

        //function to determine add or edit process
        function CheckAddorEditWorkflow() {
            var hdnWorkflowaction = document.getElementById("<%= hdnActionWorkflow.ClientID %>");
            var hiddenWorkflowId = document.getElementById("<%= hiddenWorkflowId.ClientID %>").value;


            try {
                if (hiddenWorkflowId == "undefined" || hiddenWorkflowId == undefined) {
                    hdnWorkflowaction.value = "AddWorkflow";
                }
                else {
                    hdnWorkflowaction.value = "EditWorkflow";


                }
            }
            catch (err) { }
        }





        //Show workflow Properties
        function ShowWorkflowProperties() {

            var pnlW = document.getElementById("<%= pnlWorkflow.ClientID %>");
            var msg = document.getElementById("<%= lblMessageWorkflow.ClientID %>");
            var txtWorkflowName = document.getElementById("<%= txtWorkflowName.ClientID %>");
            var txtWorkflowDescription = document.getElementById("<%= txtWorkflowDescription.ClientID %>");
            var ddp = document.getElementById("<%= ddlDmsProject.ClientID%>");
            ClearControls(ddp, txtWorkflowName, txtWorkflowDescription, msg);
            var ChkActiveWorkflow = document.getElementById("<%= ChkActiveWorkflow.ClientID %>");
            ChkActiveWorkflow.checked = true;
            ChkActiveWorkflow.disabled = true;
            pnlW.style.display = "block";

            HidePanels("Workflow");

            // DisableSaveButtons();
        }

        //to clear controls
        function ClearControls(dropDown, txtname, txtdesp, lbl) {
            txtname.value = "";
            txtdesp.value = "";
            lbl.innerHTML = "";
            dropDown.value = "0";
        }
        //Common Build
        function CommonSplitAndBindInput(div, Values) {
            $(div).empty();
            if (Values != "") {
                var strarray = Values.split(',');



                for (var i = 0; i < strarray.length; i++) {
                    var $button = $('<input/>', {
                        type: 'button',
                        id: strarray[i].split('-').splice(0, 1).join(""),
                        value: strarray[i].split('-').splice(1, 2).join("")

                    });
                    $button.appendTo(div);
                    $button.addClass("eWorkflow");
                }
                // ChangeBackgroundColor(div);
            }
        }

        //Build Stages
        function BuildStages(Values) {

            var div = '#Workflowstageright-pane';
            CommonSplitAndBindInput(div, Values);

            var chkStageActive = document.getElementById("<%= chkStageActive.ClientID %>");
            if (chkStageActive.checked == false) {
                HidePanels("None");
            }
            //DisableSaveButtons();
        }


        //validation for stages

        function CheckEmptyTextStageName() {

            var StgName = document.getElementById("<%= txtStageName.ClientID %>");
            var tatValue = parseInt(document.getElementById("<%= txtTATDuration.ClientID %>").value);
            var lblMsg = document.getElementById("<%= lblMessageStage.ClientID %>");
            var chkStageActive = document.getElementById("<%= chkStageActive.ClientID %>");
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

            if (chkStageActive.checked == false) {
                var record = confirm('Are you sure want to deactivate ? This stage will not appear from here. ');
                if (record == 1) {

                    return true;
                }
                else if (record == 0) {

                    return false;
                }
            }
            if (re.test(uid)) {
                return true;
            }
            else {
                lblMsg.innerHTML = "Stage name allows alphabets, numbers and few special characters ( _ - # @ ^ $) only.";
                return false;
            }


        }

        function CheckEmptyTextWorkflowName() {
            var WorkfloName = document.getElementById("<%= txtWorkflowName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessageWorkflow.ClientID %>");
            var ChkActiveWorkflow = document.getElementById("<%= ChkActiveWorkflow.ClientID %>");
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z]+$/; var uid;
            uid = WorkfloName.value;

            if (WorkfloName.value == "" || WorkfloName.value == undefined) {
                lblMsg.innerHTML = "Please enter a workflow name.";
                return false;
            }
            if (ChkActiveWorkflow.checked == false) {
                var record = confirm('Are you sure want to deactivate ? This workflow will not appear from here. ');
                if (record == 1) {

                    return true;
                }
                else if (record == 0) {

                    return false;
                }
            }

            if (re.test(uid)) {

                return true;
            }



            else {
                lblMsg.innerHTML = "Workflow name allows alphabets, numbers and few special characters ( _ - # @ ^ $ ) only.";
                return false;
            }
        }

        function SaveStage() {
            document.getElementById("<%=btnStageSubmit.ClientID %>").click();
        }


        //Stages li click
        //On Stages input click
        $(function () {
            $("#divStageDest").on('click', 'input', function () {
                var hdnActionStage = document.getElementById("<%= hdnActionStage.ClientID %>");
                var hiddenStageId = document.getElementById("<%= hiddenStageId.ClientID %>");
                document.getElementById("<%= hiddenStageSaveStatus.ClientID %>").value = "Yes";
               
               
                id = $(this).attr("id");

                document.getElementById('<%= hiddenStageId.ClientID %>').value = id;

                $("#divStageDest input").css({
                    backgroundColor: ''
                });
                $("#divStageDest li").css({
                    backgroundColor: 'rgb(221, 221, 221)'
                });
                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });

                CheckAddorEditStages();

                if (hdnActionStage.value == "EditStages") {
                    hiddenStageId.value = id;
                    document.getElementById('<%= btnStageEdit.ClientID %>').click();

                }

            });
        });

        //On Stages li click
        $(function () {
            $("#divStageDest").on('click', 'li', function () {

                $("#divStageDest li").css({
                    backgroundColor: ''
                });
                $("#divStageDest input").css({
                    backgroundColor: ''
                });
                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });


                document.getElementById('<%= btnStageAdd.ClientID %>').click();

                // ShowStagesProperties();
            });
        });


        //Stages Sortable

        function AddStageNew() {


            $("#workflowstageleft-pane,#Workflowstageright-pane").sortable({
                connectWith: "#workflowstageleft-pan,#Workflowstageright-pane",

                stop: function () {
                    $('#divStageDest li').addClass('addnew');


                },
                receive: function (event, ui) { },
                revert: true

            });
            $("#workflowstageleft-pan,#Workflowstageright-pane").disableSelection();

            $("#Workflowstageright-pane").on("sortreceive", function (event, ui) {
                var $list = $(this);

                var ProcessSaveStatus = document.getElementById("<%= hiddenProcessSaveStatus.ClientID %>");
                var WorkflowSaveStatus = document.getElementById("<%= hiddenWorkflowSaveStatus.ClientID %>");

                if (($('#processright-pane').children().length == 0) || (ProcessSaveStatus.value != "Yes")) {
                    alert('Please save/click the  process');
                    HidePanels("None");
                    $(ui.sender).sortable('cancel');

                }

                else if (($('#Workflowright-pane').children().length == 0) || (WorkflowSaveStatus.value != "Yes")) {
                    alert('Please save/click the selected workflow');
                    HidePanels("None");
                    $(ui.sender).sortable('cancel');

                }




            });

        }

        //Show Stages Properties
        function ShowStagesProperties() {
            var hdnActionStage = document.getElementById("<%= hdnActionStage.ClientID %>");

            var pnlStages = document.getElementById("<%= pnlStages.ClientID %>");
            var pnlStagesAdd = document.getElementById("<%= pnlStagesAdd.ClientID %>");
            CheckAddorEditStages();
            if (hdnActionStage.value != "EditStages") {
                document.getElementById('<%= hiddenStageId.ClientID %>').value = 0;
            }
            if (hdnActionStage.value == "EditStages") {
                pnlStages.style.display = 'block';
                document.getElementById('<%= btnStageEdit.ClientID %>').click();
            }
            else {
                pnlStagesAdd.style.display = 'block';
                document.getElementById('<%= btnStageAdd.ClientID %>').click();
            }

            //DisableSaveButtons();
        }

        function CheckAddorEditStages() {
            var hdnActionStages = document.getElementById("<%= hdnActionStage.ClientID %>");
            var hiddenStageId = document.getElementById("<%= hiddenStageId.ClientID %>").value;
            try {
                if (hiddenStageId == "undefined" || hiddenStageId == undefined || hiddenStageId == "") {
                    hdnActionStages.value = "AddStages";
                }
                else {
                    hdnActionStages.value = "EditStages";
                }
            }
            catch (err) { hdnActionStages.value = "AddStages"; }
        }

        function BuildStageFields(Values) {
            var div = '#divFieldsDest';
            CommonSplitAndBindInput(div, Values);

            //DisableSaveButtons();
        }

        function BuildNotification(Values) {

            var div = '#divNotiDest';
            CommonSplitAndBindInput(div, Values);
            var chkNotificationActive = document.getElementById("<%= chkNotificationActive.ClientID %>");
            if (chkNotificationActive.checked == false) { HidePanels("None"); }
            GoBacktoNotification();
        }



        function GetBackWorkflow() {
            if ($('#workflowleft-pane').children().length == 0) {
                $("#workflowleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span> + Add Workflow </li>');
            }
        }

        function GoBacktoStatus() {
            if ($('#Statusleft-pane').children().length == 0) {
                $("#Statusleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span> + Add Status </li>');
                $('#divStatusDest li').remove();
            }
        }

        //Sortable AddStatusNew()

        function AddStatusNew() {
            $("#Statusleft-pane,#divStatusDest").sortable({
                connectWith: "#Statusleft-pane,#divStatusDest",

                stop: function () {
                    $('#divStatusDest li').addClass('addnew');


                },
                receive: function (event, ui) { },
                revert: true

            });


            $("#divStatusDest").on("sortreceive", function (event, ui) {
                var $list = $(this);

                var ProcessSaveStatus = document.getElementById("<%= hiddenProcessSaveStatus.ClientID %>");
                var WorkflowSaveStatus = document.getElementById("<%= hiddenWorkflowSaveStatus.ClientID %>");
                var StageSaveStatus = document.getElementById("<%= hiddenStageSaveStatus.ClientID %>");

                if (($('#processright-pane').children().length == 0) || (ProcessSaveStatus.value != "Yes")) {
                    alert('Please save/click the  process');
                    HidePanels("None");
                    $(ui.sender).sortable('cancel');

                }

                else if (($('#Workflowright-pane').children().length == 0) || (WorkflowSaveStatus.value != "Yes")) {
                    alert('Please save/click the  workflow');
                    $(ui.sender).sortable('cancel');
                    HidePanels("None");

                }
                else if (($('#Workflowstageright-pane').children().length == 0) || (StageSaveStatus.value != "Yes")) {
                    alert('Please save/click the  stages');
                    $(ui.sender).sortable('cancel');
                    HidePanels("None");

                }


            });

        }

        //Status Properties
        function ShowStatusProperties() {
            var StatusName = document.getElementById("<%= txtStatusName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblStatusMessage.ClientID %>");
            var hdnActionStatus = document.getElementById("<%= hdnActionStatus.ClientID %>");
            var pnlStatus = document.getElementById("<%= pnlStatus.ClientID %>");
            var pnlStatusAdd = document.getElementById("<%= pnlStatusAdd.ClientID %>");
            ClearControls("", StatusName, "", lblMsg);
            CheckAddorEditStatus();
            if (hdnActionStatus.value == "EditStatus") {
                pnlStatus.style.display = 'block';
                document.getElementById('<%= btnStatusEdit.ClientID %>').click();

            }
            else {
                pnlStatusAdd.style.display = 'block';
                document.getElementById('<%= btnStatusAdd.ClientID %>').click();

            }
        }

        function BuildAddStatus(Values) {
            var div = '#divStatusDest';
            CommonSplitAndBindInput(div, Values);
            var ChkStatusActive = document.getElementById("<%= ChkStatusActive.ClientID %>");
            if (ChkStatusActive.checked == false) { HidePanels("None"); }

            GoBacktoStatus();
        }

        function GoBacktostages() {
            if ($('#workflowstageleft-pane').children().length == 0) {
                $("#workflowstageleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+ Add Stages </li>');
            }
        }
        function GoBacktoNotification() {
            if ($('#Notificationleft-pane').children().length == 0) {
                $("#Notificationleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span> + Add Notification </li>');
            }
        }
        function GoBackToProcess() {
            if ($('#processleft-pane').children().length == 0) {
                $("#processleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span> + Add Process </li>');

            }
//            if ($('#processright-pane  li').children())
//          {
//            //DMSENH6-5207 BS
//            var processplaneX = $('#processright-pane  li');
//            $("#sortableProcessUnconfirm").append(('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span> ' + processplaneX[0].innerText + ' </li>'));
//            //DMSENH6-5207 BE
//            }
        }

        function Removestage(hid) {

            $("#divStageDest input[id*='" + hid + "']").remove();
        }

        function Removestatus(hid)
        { $("#divStatusDest input[id*='" + hid + "']").remove(); }

        //Status input click

        $(function () {
            $("#divStatusDest").on('click', 'input', function () {


                var hdnActionStatus = document.getElementById("<%= hdnActionStatus.ClientID %>");
                var hiddenStatusId = document.getElementById("<%= hiddenStatusId.ClientID %>");

                id = $(this).attr("id");

                document.getElementById('<%= hiddenStatusId.ClientID %>').value = id;
                CheckAddorEditStatus();

                $("#divStatusDest input").css({
                    backgroundColor: ''
                });

                $("#divStatusDest li").css({
                    backgroundColor: 'rgb(221, 221, 221)'
                });

                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });

                //               CheckAddorEditStatus();

                //            if (hdnActionStatus.value == "EditStatus") {
                hiddenStatusId.value = id;
                document.getElementById('<%= btnStatusEdit.ClientID %>').click();
                ShowStatusProperties();
                //                }

            });
        });

        $(function () {
            $("#divStatusDest").on('click', 'li', function () {

                CheckAddorEditStatus();
                $("#divStatusDest input").css({
                    backgroundColor: ''
                });
                $("#divStatusDest li").css({
                    backgroundColor: ''
                });
                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });

                document.getElementById('<%= btnStatusAdd.ClientID %>').click();


            });
        });



        // status validation
        function CheckEmptyTextStatusName() {
            var StatusName = document.getElementById("<%= txtStatusName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblStatusMessage.ClientID %>");
            var ChkStatusActive = document.getElementById("<%= ChkStatusActive.ClientID %>");
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z]+$/; var uid;
            uid = StatusName.value;
            CheckAddorEditStatus();
            if (StatusName.value == "" || StatusName.value == undefined) {
                lblMsg.innerHTML = "Please enter a status name .";

                return false;
            }
            if (ChkStatusActive.checked == false) {
                var record = confirm('Are you sure want to deactivate ? This status will not appear here ');
                if (record == 1) {

                    return true;
                }
                else if (record == 0) {

                    return false;
                }
            }

            if (re.test(uid)) {
                return true;
            }
            else {
                lblMsg.innerHTML = "Status name allows alphabets, numbers and few special characters ( _ - # @ ^ $) only.";
                return false;
            }
        }
        //Check for status add or edit
        function CheckAddorEditStatus() {
            var hdnActionStatus = document.getElementById("<%= hdnActionStatus.ClientID %>");
            var hiddenStatusId = document.getElementById("<%= hiddenStatusId.ClientID %>").value;
            try {
                if (hiddenStatusId == "undefined" || hiddenStatusId == undefined || hiddenStatusId == "") {
                    hdnActionStatus.value = "AddStatus";
                }
                else {
                    hdnActionStatus.value = "EditStatus";
                }
            }
            catch (err) { hdnActionStatus.value = "AddStatus"; }


        }

        //Show Fiels PopUp
        function ShowFieldProperties() {

            var hiddenFeildId = document.getElementById("<%= hdnStageFieldId.ClientID %>").value;
            HidePanels("None");
            document.getElementById('frameStageProperties').src = 'WorkflowStudioStageFields.aspx?FieldId=' + hiddenFeildId;
            modelBG.className = "mdBG";
            mb.className = "sentmailbox";

        }


        function HideFieldsProperties() {

            document.getElementById('frameStageProperties').src = 'WorkflowStudioStageFields.aspx';
            modelBG.className = "mdNone";
            mb.className = "mdNone";

            if ($("#StageFieldsleft-pane").children().length == 0) {

                $("#StageFieldsleft-pane").append('<li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span> + Add Fields </li>');
                $('#divFieldsDest li').remove();
            }
            document.getElementById("<%= ClearFieldId.ClientID %>").click();
            //Close button of new field  
            //            if (($("#divFieldsDest li").last().find("input").css("background-color") == 'rgb(255, 0, 0)')) {
            //                $("#divFieldsDest input[type=button]").remove();
            //            }

            return false;
        }

        //Notification Section
        function AddNotificationNew() {
            $("#Notificationleft-pane,#divNotiDest").sortable({
                connectWith: "#Notificationleft-pane,#divNotiDest",

                stop: function () {
                    $('#divNotiDest li').addClass('addnew');


                },
                receive: function (event, ui) { },
                revert: true

            });


            $("#divNotiDest").on("sortreceive", function (event, ui) {
                var $list = $(this);
                var ProcessSaveStatus = document.getElementById("<%= hiddenProcessSaveStatus.ClientID %>");
                var WorkflowSaveStatus = document.getElementById("<%= hiddenWorkflowSaveStatus.ClientID %>");
                var StageSaveStatus = document.getElementById("<%= hiddenStageSaveStatus.ClientID %>");

                if (($('#processright-pane').children().length == 0) || (ProcessSaveStatus.value != "Yes")) {
                    alert('Please save/click the  process');
                    $(ui.sender).sortable('cancel');
                    HidePanels("None");
                }

                else if (($('#Workflowright-pane').children().length == 0) || (WorkflowSaveStatus.value != "Yes")) {
                    alert('Please save/click the  workflow');
                    $(ui.sender).sortable('cancel');
                    HidePanels("None");
                }
                else if (($('#Workflowstageright-pane').children().length == 0) || (StageSaveStatus.value != "Yes")) {
                    alert('Please save/click the  stages');
                    $(ui.sender).sortable('cancel');
                    HidePanels("None");
                }

            });
        }

        function ShowNotificationProperties() {
            var hdnActionNotification = document.getElementById("<%= hdnActionNotification.ClientID %>");
            var pnlNotification = document.getElementById("<%= pnlNotifications.ClientID %>");
            var ddl_Status = (document.getElementById("<%= ddl_Status.ClientID %>"));
            var tatValue = (document.getElementById("<%= txt_TAT.ClientID %>"));
            var lblMsg = document.getElementById("<%= lblNotificationMessage.ClientID %>");
            var chkNotificationActive = document.getElementById("<%= chkNotificationActive.ClientID %>");
            ClearControls(ddl_Status, tatValue, "", lblMsg);
            chkNotificationActive.checked = true;
            chkNotificationActive.disabled = true;
            CheckAddorEditNotification();
            pnlNotification.style.display = 'block';
            if (hdnActionNotification == "EditNotification") {
                document.getElementById('<%= btnNotificationEdit.ClientID %>').click();
            }
            else
            { document.getElementById('<%= btnNotificationAdd.ClientID %>').click(); }


        }
        //Notification validation

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
        function validateDetails() {
            var ddl_Status = (document.getElementById("<%= ddl_Status.ClientID %>").value);
            var tatValue = parseInt(document.getElementById("<%= txt_TAT.ClientID %>").value);
            var lblMsg = document.getElementById("<%= lblNotificationMessage.ClientID %>");

            if (ddl_Status == "0") {
                lblMsg.innerHTML = "Please select an stage status.";
                return false;
            }

            if ((isNaN(tatValue)) || (tatValue <= 0)) {
                lblMsg.innerHTML = "Please enter the TAT value in minutes.";
                return false;
            }

            return true;
        }

        $(function () {
            $("#divNotiDest").on('click', 'li', function () {

                CheckAddorEditNotification();
                $("#divNotiDest input").css({
                    backgroundColor: ''
                });
                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });

                ShowNotificationProperties();


            });
        });
        $(function () {
            $("#divNotiDest").on('click', 'input', function () {
                var hdnActionNotification = document.getElementById("<%= hdnActionNotification.ClientID %>");
                var hdnNotificationId = document.getElementById("<%= hdnNotificationId.ClientID %>");

                id = $(this).attr("id");

                document.getElementById('<%= hdnNotificationId.ClientID %>').value = id;
                CheckAddorEditNotification();
                $("#divNotiDest input").css({
                    backgroundColor: ''
                });

                $("#divNotiDest li").css({
                    backgroundColor: 'rgb(221, 221, 221)'
                });


                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });


                hdnNotificationId.value = id;
                document.getElementById('<%= btnNotificationEdit.ClientID %>').click();



            });
        });

        function CheckAddorEditNotification() {
            var hdnActionNotification = document.getElementById("<%= hdnActionNotification.ClientID %>");
            var hdnNotificationId = document.getElementById("<%= hdnNotificationId.ClientID %>").value;
            try {
                if (hdnNotificationId == "undefined" || hdnNotificationId == undefined || hdnNotificationId == "") {
                    hdnActionNotification.value = "AddNotification";
                }
                else {
                    hdnActionNotification.value = "EditNotification";
                }

            }
            catch (err) { hdnActionNotification.value = "AddNotification"; }
        }


        var specialKeys = new Array();
        specialKeys.push(8); //Backspace

        function IsNumeric(e) {
            var keyCode = e.which ? e.which : e.keyCode
            var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
            return ret;
        }

        //Sortable Fields
        function AddFieldsnew() {
            $("#StageFieldsleft-pane,#divFieldsDest").sortable({
                connectWith: "#StageFieldsleft-pane,#divFieldsDest",

                stop: function () {
                    $('#divFieldsDest li').addClass('addnew');

                },
                receive: function (event, ui) { },
                revert: true

            });


            $("#divFieldsDest").on("sortreceive", function (event, ui) {
                var $list = $(this);
                var ProcessSaveStatus = document.getElementById("<%= hiddenProcessSaveStatus.ClientID %>");
                var WorkflowSaveStatus = document.getElementById("<%= hiddenWorkflowSaveStatus.ClientID %>");
                var StageSaveStatus = document.getElementById("<%= hiddenStageSaveStatus.ClientID %>");

                if (($('#processright-pane').children().length == 0) || (ProcessSaveStatus.value != "Yes")) {
                    alert('Please save/click the  process');
                    HidePanels("None");
                    $(ui.sender).sortable('cancel');
                }

                else if (($('#Workflowright-pane').children().length == 0) || (WorkflowSaveStatus.value != "Yes")) {
                    alert('Please save/click the selected workflow');
                    HidePanels("None");
                    $(ui.sender).sortable('cancel');
                }
                else if (($('#Workflowstageright-pane').children().length == 0) || (StageSaveStatus.value != "Yes")) {
                    alert('Please save/click the selected stages');
                    HidePanels("None");
                    $(ui.sender).sortable('cancel');
                }


            });
        }

        //stagefields li click

        $(function () {
            $("#divFieldsDest").on('click', 'li', function () {

                document.getElementById("<%= hdnStageFieldId.ClientID %>").value = "";


                $("#divFieldsDest li").css({
                    backgroundColor: ''
                });
                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });

                ShowFieldProperties();



            });
        });

        $(function () {
            $("#divFieldsDest").on('click', 'input', function () {
                var HdnStageFieldId = document.getElementById("<%= hdnStageFieldId.ClientID %>");
                id = $(this).attr("id");

                HdnStageFieldId.value = id;
                $("#divFieldsDest input").css({
                    backgroundColor: ''
                });

                $("#divFieldsDest li").css({
                    backgroundColor: 'rgb(221, 221, 221)'
                });

                $(this).css({
                    backgroundColor: 'rgb(255, 228, 32);'
                });

                ShowFieldProperties();



            });
        });


        function LoadALLStageDetails() {
            var hdnFieldsvalues = document.getElementById("<%= hdnFieldsvalues.ClientID %>");
            var hdnnotificationvalues = document.getElementById("<%= hdnnotificationvalues.ClientID %>");
            var hdnstatusvalues = document.getElementById("<%= hdnstatusvalues.ClientID %>");
            BuildNotification(hdnnotificationvalues.value);
            BuildStageFields(hdnFieldsvalues.value);
            BuildAddStatus(hdnstatusvalues.value);
            hdnFieldsvalues.value = "";
            hdnnotificationvalues.value = "";
            hdnstatusvalues.value = "";
            GoBacktostages();

        }

        //function for drag and drop validation
        function ValidateWokflowDragAndDrop(panel) {
            var isundefined = "";

            var listItems = $(panel);

            if (listItems.length > 0) {
                listItems.each(function (li) {


                    var id = ($(this).attr('id'));
                    if (id == undefined || id == "undefined") {
                        isundefined = id;
                        return isundefined;

                    }
                });
            }
            else {
                isundefined = id;
                return isundefined;
            }
        }

        //Added for warning message when the user change the data entry type and save
        function ConfirmBox() {
            var proceed = window.confirm("The Data entry type has been changed due to which all the stage fields coordinates values mapped to that stage will be cleared." + '\n' + "Do you really want to continue? Click OK to continue, Cancel to return");

            if (proceed)
                SaveFunctionality();
        }

        function SaveFunctionality() {
            document.getElementById("<%=btnStageSubmit.ClientID %>").click();
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
                                            <div id="divProcessSource">
                                                <ul id="processleft-pane" style="margin-left: -38px;">
                                                    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+
                                                        Process </li>
                                                </ul>
                                            </div>
                                            <h3 onclick="ConnectunconfirmProcess()">
                                                <a href="#">Processes</a></h3>
                                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
                                            <ContentTemplate><div>
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
                                            </div></ContentTemplate>
                                            </asp:UpdatePanel>
                                            

                                        </div>
                                        <h3>
                                            <a href="#">Workflow</a></h3>
                                        <div class="accordian">
                                            <h3 onclick="AddWorkflowNew();">
                                                <a href="#">Add New</a></h3>
                                            <div id="divWorkflowSource">
                                                <ul id="workflowleft-pane" style="margin-left: -38px;">
                                                    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+
                                                        Workflow</li>
                                                </ul>
                                            </div>
                                        </div>
                                        <h3>
                                            <a href="#">Stages</a></h3>
                                        <div class="accordian">
                                            <h3 onclick="AddStageNew();">
                                                <a href="#">Add Stage</a></h3>
                                            <div id="divStagesSource">
                                                <ul id="workflowstageleft-pane" style="margin-left: -38px;">
                                                    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+
                                                        Stages</li>
                                                </ul>
                                            </div>
                                        </div>
                                        <h3 onclick="AddStatusNew();">
                                            <a href="#">Status</a></h3>
                                        <div class="accordian">
                                            <h3>
                                                Add New</h3>
                                            <div id="divStatusSource">
                                                <ul id="Statusleft-pane" style="margin-left: -38px;">
                                                    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+
                                                        Status</li>
                                                </ul>
                                            </div>
                                        </div>
                                        <h3 onclick="AddFieldsnew()">
                                            <a href="#">Fields</a></h3>
                                        <div class="accordian">
                                            <h3>
                                                Add New</h3>
                                            <div id="divStageFieldsSource">
                                                <ul id="StageFieldsleft-pane" style="margin-left: -38px;">
                                                    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+
                                                        Fields</li>
                                                </ul>
                                            </div>
                                        </div>
                                        <h3>
                                            <a href="#">Notifications</a></h3>
                                        <div class="accordian">
                                            <h3 onclick="AddNotificationNew();">
                                                Add New</h3>
                                            <div id="divNotificatonSource">
                                                <ul id="Notificationleft-pane" style="margin-left: -38px;">
                                                    <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>+
                                                        Notification</li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: top; width: 30%;border-right: thin  double #ff0000;">
                        <table >
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
                                                    <ul id="StageFieldsright-pane">
                                                    </ul>
                                                </div>
                                            </td>
                                            <td class="aligntop">
                                                <b>Notifications</b>
                                                <div id="divNotiDest" class="stgdetails" style="border-right: none">
                                                    <ul id="Notificationright-pane">
                                                    </ul>
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
                        <asp:Button ID="btnConfirm" runat="server" Text="Confirm" OnClientClick="return CheckallConfiguration();"
                            CssClass="displayNone" OnClick="btnConfirm_Click" />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:Panel ID="pnlProcess" runat="server" CssClass="displayNone">
                                    <asp:Button ID="btnProcessEdit" runat="server" OnClick="btnProcessEdit_Click" CssClass="displayNone" />
                                    <div class="GVDiv">
                                        <ajax:TabContainer runat="Server" ID="Processtab" ActiveTabIndex="0" CssClass="ajax__myTab">
                                            <ajax:TabPanel ID="ProcessControls" runat="server">
                                                <HeaderTemplate>
                                                    Process Details</HeaderTemplate>
                                                <ContentTemplate>
                                                    <table>
                                                        <tr>
                                                            <td colspan="2">
                                                                <%--   <h3>
                                                                    <asp:Label ID="lblEditProcess" runat="server" Text="Process Details" meta:resourcekey="lblEditProcess" /></h3>--%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <asp:Label ID="lblMessageProcess" ForeColor="Red" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
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
                                                                <asp:CheckBox ID="chkProcessActive" runat="server" Enabled="false" Checked="true" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                &nbsp;&nbsp;&nbsp;
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btnSave" runat="server" Text="Save" OnClientClick="return CheckEmptyTextProcessName();"
                                                                    OnClick="btnProcessSave_Click" CssClass="btnsave" />
                                                                <asp:Button ID="btnProcessCancel" runat="server" Text="Cancel" OnClientClick="return HideProperty(this.id);"
                                                                    OnClick="btnProcessCancel_Click" CssClass="btncancel" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ContentTemplate>
                                            </ajax:TabPanel>
                                            <ajax:TabPanel ID="ProcessOwners" runat="server">
                                                <HeaderTemplate>
                                                    Process Owners</HeaderTemplate>
                                                <ContentTemplate>
                                                    <table>
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
                                                                        TagName="Add" meta:resourcekey="btnAddUser_ProcessOwner" CssClass="btndownarrow" />
                                                                    <asp:Button ID="btnRemoveUser_ProcessOwner" runat="server" Text="" OnClick="btnRemoveUser_ProcessOwner_Click"
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
                                                </ContentTemplate>
                                            </ajax:TabPanel>
                                        </ajax:TabContainer>
                                    </div>
                                </asp:Panel>
                                <asp:HiddenField ID="hdnCurrentPanelProcessOwner" runat="server" Value="0" />
                                <asp:HiddenField ID="hdnCurrentSubPanelProcessOwner" runat="server" Value="0" />
                                <asp:Panel ID="pnlWorkflow" runat="server" CssClass="displayNone">
                                    <asp:Button ID="btnWorkflowEdit" runat="server" OnClick="btnWorkflowEdit_Click" CssClass="displayNone" />
                                    <div class="GVDiv">
                                        <ajax:TabContainer runat="Server" ID="Workflow" ActiveTabIndex="0" CssClass="ajax__myTab">
                                            <ajax:TabPanel ID="WorkflowControls" runat="server">
                                                <HeaderTemplate>
                                                    Workflow Details</HeaderTemplate>
                                                <ContentTemplate>
                                                    <table>
                                                        <asp:Label ID="lblMessageWorkflow" runat="server" ForeColor="Red"></asp:Label>
                                                        <tr>
                                                            <td colspan="2">
                                                                <%--  <h3>
                                                                    <asp:Label ID="lblEditWorkFlow" runat="server" Text="Add/Edit Workflow Details" meta:resourcekey="lblEditWorkFlow" /></h3>--%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                            </td>
                                                        </tr>
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
                                                                <asp:TextBox ID="txtWorkflowName" runat="server" MaxLength="50"></asp:TextBox>
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
                                                                <asp:CheckBox ID="ChkActiveWorkflow" runat="server" Enabled="false" Checked="true" />
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
                                                        <tr>
                                                            <td>
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btnWorkflowSave" runat="server" Text="Save" OnClientClick="return CheckEmptyTextWorkflowName();"
                                                                    OnClick="btnWorkflowSave_Click" CssClass="btnsave" />
                                                                <asp:Button ID="btnWorkflowCancel" runat="server" Text="Cancel" OnClientClick="return HideProperty(this.id);"
                                                                    OnClick="btnWorkflowCancel_Click" CssClass="btncancel" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </ContentTemplate>
                                            </ajax:TabPanel>
                                            <ajax:TabPanel ID="WorkflowOwners" runat="server">
                                                <HeaderTemplate>
                                                    Workflow Owners</HeaderTemplate>
                                                <ContentTemplate>
                                                    <table>
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
                                                                        meta:resourcekey="btnAddUser_WorkflowOwner" CssClass="btndownarrow" />
                                                                    <asp:Button ID="btnRemoveUser_WorkflowOwner" TagName="Add" runat="server" Text=""
                                                                        OnClick="btnRemoveUser_WorkflowOwner_Click" meta:resourcekey="btnRemoveUser_WorkflowOwner"
                                                                        CssClass="btnuparrow" />
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
                                                </ContentTemplate>
                                            </ajax:TabPanel>
                                        </ajax:TabContainer>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlStages" runat="server" CssClass="displayNone">
                                    <div class="GVDiv">
                                        <ajax:TabContainer runat="Server" ID="TabContainer1" ActiveTabIndex="0" CssClass="ajax__myTab">
                                            <ajax:TabPanel ID="TabPanel1" runat="server">
                                                <HeaderTemplate>
                                                    Stage Details</HeaderTemplate>
                                                <ContentTemplate>
                                                    <table>
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
                                                                <asp:DropDownList ID="ddlDataEntry" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDataEntry_SelectedIndexChanged">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <asp:PlaceHolder ID="TempaltePathPlaceHolder" runat="server" Visible="false">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblUploadedPath" runat="server" Text="Template Path" />
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblStageTemplatePath" runat="server" Style="color: gray; font-size: small;"></asp:Label>
                                                                </td>
                                                            </tr>
                                                        </asp:PlaceHolder>
                                                        <asp:Panel ID="StageFileControlPlaceHolder" runat="server">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblSelectFile" runat="server" Text="Select File" />
                                                                </td>
                                                                <td>
                                                                    <ajax:AsyncFileUpload ID="StageTemplateUpload" runat="server" CompleteBackColor="Lime"
                                                                        ErrorBackColor="Red" ThrobberID="Throbber" UploadingBackColor="#66CCFF" CssClass="imageUploaderField"
                                                                        OnUploadedComplete="StageTemplateUpload_UploadedComplete" OnClientUploadStarted="uploadStart"
                                                                        OnClientUploadComplete="uploadComplete" />
                                                                    <asp:Label ID="Throbber" runat="server" Style="display: none">
                          <img alt="Loading..." src="<%= Page.ResolveClientUrl("~/Images/indicator.gif")%>" /></asp:Label>
                                                                    <span style="color: gray; font-size: small;">* supported file extension: pdf.</span>
                                                                </td>
                                                            </tr>
                                                        </asp:Panel>
                                                        <asp:PlaceHolder ID="BackgroundImagePlaceHolder" runat="server" Visible="false">
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
                                                        </asp:PlaceHolder>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblStageActive" runat="server" Text="Active" meta:resourcekey="lblStageActive" />
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox ID="chkStageActive" runat="server" Enabled="false" Checked="true" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <div style="float: right;">
                                                        <asp:Button ID="btnStageDelete" runat="server" Text="Delete" TagName="Add" OnClick="btnStageDelete_Click"
                                                            OnClientClick="return confirm('Are you sure you want to delete this stage')"
                                                            CssClass="btnsave" />
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
                                                </ContentTemplate>
                                            </ajax:TabPanel>
                                            <ajax:TabPanel ID="TabPanel2" runat="server">
                                                <HeaderTemplate>
                                                    Stage Owner Mapping</HeaderTemplate>
                                                <ContentTemplate>
                                                    <table>
                                                        <tr>
                                                            <td colspan="2">
                                                                <div id="divStageOwner">
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="lblAvailableUsers_StageOwner" runat="server" Text="Available Users"
                                                                                    meta:resourcekey="lblAvailableUsers_StageOwner" /><br />
                                                                            </td>
                                                                            <td>
                                                                                <asp:ListBox ID="lstAvailableUsers_StageOwner" runat="server" Height="150" Width="250"
                                                                                    CssClass="stageOwner"></asp:ListBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                            </td>
                                                                            <td>
                                                                                <center>
                                                                                    <asp:Button ID="btnAddUser_StageOwner" TagName="Add" runat="server" Text="" OnClick="btnAddUser_StageOwner_Click"
                                                                                        meta:resourcekey="btnAddUser_StageOwner" CssClass="btndownarrow" />
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
                                                                                <asp:ListBox ID="lstAssignedUsers_StageOwner" runat="server" Height="150" Width="250"
                                                                                    CssClass="stageOwner"></asp:ListBox>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                                <b>
                                                                    <asp:Label ID="lblHdrStageUserMapping" runat="server" Text="Stage User Mapping" meta:resourcekey="lblHdrStageUserMapping" /></b>
                                                                <div id="divUserMain" class="accordian">
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="lblAvailableUsers_StageUser" runat="server" Text="Available Users"
                                                                                    meta:resourcekey="lblAvailableUsers_StageUser" />
                                                                            </td>
                                                                            <td>
                                                                                <asp:ListBox ID="lstAvailableUsers_StageUser" runat="server" Height="150" Width="250"
                                                                                    CssClass="stageuser"></asp:ListBox>
                                                                            </td>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                            </td>
                                                                            <td>
                                                                                <center>
                                                                                    <asp:Button ID="btnAddUser_StageUser" TagName="Add" runat="server" Text="" OnClick="btnAddUser_StageUser_Click"
                                                                                        meta:resourcekey="btnAddUser_StageUser" CssClass="btndownarrow" />
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
                                                                                <asp:ListBox ID="lstAssignedUsers_StageUser" runat="server" Height="150" Width="250"
                                                                                    CssClass="stageuser"></asp:ListBox>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                                <b>
                                                                    <asp:Label ID="lblHdrStageUserMapping_Sub_Group" runat="server" Text="User Groups"
                                                                        meta:resourcekey="lblHdrStageUserMapping_Sub_Group" /></b>
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
                                                                                        CssClass="btndownarrow" />
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
                                                </ContentTemplate>
                                            </ajax:TabPanel>
                                        </ajax:TabContainer>
                                        <asp:HiddenField ID="hdnFileUpload" runat="server" />
                                        <asp:HiddenField ID="hdnStageTemplatePath" runat="server" />
                                        <asp:HiddenField ID="hdnPagesCount" runat="server" />
                                        <asp:HiddenField ID="hdnPageNo" runat="server" />
                                        <asp:HiddenField ID="hdnDataentryId" runat="server" />
                                        <asp:HiddenField ID="hdnDataEntryType" runat="server" />
                                        <asp:HiddenField ID="hdnDataEntryName" runat="server" />
                                        <asp:HiddenField ID="hdnStageName" runat="server" />
                                    </div>
                                    <asp:Button ID="btnStageEdit" runat="server" OnClick="btnStageEdit_Click" CssClass="displayNone" />
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
                                            <asp:Button ID="btnStageOk" runat="server" OnClick="btnStageAddOk_Click" Text="OK"
                                                CausesValidation="false" meta:resourcekey="btnOk" CssClass="btnsave" TagName="Add" />
                                            <asp:Button ID="btnStageAddCancel" runat="server" Text="Cancel" OnClick="btnStageAddCancel_Click"
                                                OnClientClick="return HideProperty(this.id);" meta:resourcekey="btnCancel" CssClass="btncancel"
                                                TagName="Read" />
                                        </div>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlStatus" runat="server" class="displayNone">
                                    <asp:Button ID="btnStatusEdit" runat="server" OnClick="btnStatusEdit_Click" CssClass="displayNone" />
                                    <div class="GVDiv">
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
                                                            <asp:Label ID="lblEditStatusDetails" runat="server" Text="Status Details" meta:resourcekey="lblEditStatusDetails" /></h3>
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
                                                        <asp:CheckBox ID="ChkStatusActive" runat="server" Enabled="false" Checked="true" />
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
                                            <asp:Button ID="btnStatusDelete" runat="server" Text="Delete" TagName="Add" OnClick="btnStatusDelete_Click"
                                                OnClientClick="return confirm('Are you sure you want to delete this status')"
                                                CssClass="btnsave" />
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
                                                            <asp:Label ID="lblEditNotificationDetails" runat="server" Text="Notification Details"
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
                                                        <asp:CheckBox ID="chkNotificationActive" runat="server" Enabled="false" Checked="true" />
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
                                <asp:Button ID="ClearFieldId" runat="server" CssClass="displayNone" OnClick="ClearField_Click" />
                                <!--Using Iframe for fields html End-->
                            </ContentTemplate>
                </asp:UpdatePanel> </td> </tr>
            </table>
        </div>
    </div>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <asp:HiddenField ID="hdnSaveStatus" runat="server" />
            <asp:HiddenField ID="hiddenProcessId" runat="server" />
            <asp:HiddenField ID="hiddenWorkflowId" runat="server" />
            <asp:HiddenField ID="hiddenStageId" runat="server" Value="" />
            <asp:HiddenField ID="hdnStageFieldId" runat="server" Value="" />
            <asp:HiddenField ID="hdnNotificationId" runat="server" Value="" />
            <asp:HiddenField ID="hiddenOrganizationId" runat="server" />
            <asp:HiddenField ID="hdnErrorStatus" runat="server" Value="" />
            <asp:HiddenField ID="hdnSaveFlagStatus" runat="server" />
            <asp:HiddenField ID="hdnCopytoli" runat="server" />
            <asp:HiddenField ID="hdnWorkFlowCount" runat="server" />
            <asp:HiddenField ID="hdnAction" runat="server" />
            <asp:HiddenField ID="hdnActionProcess" runat="server" />
            <asp:HiddenField ID="hdnActionWorkflow" runat="server" />
            <asp:HiddenField ID="hdnActionStage" runat="server" />
            <asp:HiddenField ID="hdnActionStatus" runat="server" />
            <asp:HiddenField ID="hdnActionNotification" runat="server" />
            <asp:HiddenField ID="hiddenStatusId" runat="server" />
            <asp:HiddenField ID="hdnFieldsvalues" runat="server" />
            <asp:HiddenField ID="hdnnotificationvalues" runat="server" />
            <asp:HiddenField ID="hdnstatusvalues" runat="server" />
            <asp:HiddenField ID="hdnConfirmActive" runat="server" />
            <asp:HiddenField ID="hiddenProcessSaveStatus" runat="server" />
            <asp:HiddenField ID="hiddenWorkflowSaveStatus" runat="server" />
            <asp:HiddenField ID="hiddenStageSaveStatus" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
