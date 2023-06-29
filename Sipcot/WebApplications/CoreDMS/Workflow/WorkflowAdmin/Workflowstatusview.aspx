<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="Workflowstatusview.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.Workflowstatusview" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Workflow/WorkflowAdmin/WorkflowPDFViewer.ascx" TagName="WorkflowPDFViewer"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .HiddenButton {
            visibility: hidden;
            text-align: center;
        }

        .DataEntryDiv {
            background: linear-gradient(to bottom, rgba(255,255,255,1) 92%,rgba(255,255,255,1) 92%,#FFC53C);
            padding: 15px;
            border: 2px solid #fdb813;
            overflow: auto;
            overflow: scroll;
            width: 490px;
            border-radius: 8px;
        }

        .trStageDtls {
            font-weight: bold;
            zoom: 1.1;
            text-decoration: underline;
            color: blue;
            padding-bottom: 10px !important;
            padding-top: 10px !important;
        }

        .auto-style1 {
            height: 20px;
        }

        .TabStyle .ajax__tab_header {
            cursor: pointer;
            background-color: #f1f1f1;
            font-size: 14px;
            font-weight: bold;
            font-family: Arial, Helvetica, sans-serif;
            height: 30px;
            border-bottom: 1px solid #bebebe;
            width: 500px;
        }

        .TabStyle .ajax__tab_active .ajax__tab_tab {
            border: 1px solid;
            border-color: #bebebe #bebebe #e1e1e1 #bebebe;
            background-color: #e1e1e1;
            padding: 5px;
            border-bottom: none;
        }

            .TabStyle .ajax__tab_active .ajax__tab_tab:hover {
                border: 1px solid;
                border-color: #bebebe #bebebe #e1e1e1 #bebebe;
                background-color: #e1e1e1;
                padding: 5px;
                border-bottom: none;
            }

        .TabStyle .ajax__tab_tab {
            border: 1px solid;
            border-color: #e1e1e1 #e1e1e1 #bebebe #e1e1e1;
            background-color: #f1f1f1;
            color: #777777;
            cursor: pointer;
            text-decoration: none;
            padding: 5px;
        }

            .TabStyle .ajax__tab_tab:hover {
                border: 1px solid;
                border-color: #bebebe #bebebe #e1e1e1 #bebebe;
                background-color: #e1e1e1;
                color: #777777;
                cursor: pointer;
                text-decoration: none;
                padding: 5px;
                border-bottom: none;
            }

        .TabStyle .ajax__tab_active .ajax__tab_tab, .TabStyle .ajax__tab_tab, .TabStyle .ajax__tab_header .ajax__tab_tab {
            margin: 0px 0px 0px 0px;
        }

        .TabStyle .ajax__tab_body {
            font-family: Arial, Helvetica, sans-serif;
            font-size: 10pt;
            border-top: 0;
            border: 1px solid #bebebe;
            border-top: none;
            padding: 5px;
            background-color: #e1e1e1;
            width: 500px;
        }

        .auto-style2 {
            height: 4px;
        }
    </style>
    <script type="text/javascript">
        function MutExChkList(chk) {
            var chkList = chk.parentNode.parentNode.parentNode;
            var chks = chkList.getElementsByTagName("input");
            for (var i = 0; i < chks.length; i++) {
                if (chks[i] != chk && chk.checked) {
                    chks.checked = false;
                }
            }
        }
    </script>
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>

    <script type="text/javascript">


        function onlyNumbers(e, t) {
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

        function validateMaxValue(ctrlList) {

            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var ctrls = ctrlList.split("##");

            var i = 0;

            for (i = 0; i < ctrls.length; i++) {
                var ctrlType = ctrls[i].split("|")[1];
                var ctrl = ctrls[i].split("|")[0];
                var strMinVal = ctrls[i].split("|")[2];
                var strMaxVal = ctrls[i].split("|")[3];



                var control = document.getElementById("ctl00_ContentPlaceHolder1_" + ctrl);

                var minVal = 0;
                var maxVal = 0;
                var controlVal = 0;

                try {
                    minVal = parseInt(strMinVal);
                    maxVal = parseInt(strMaxVal);
                    controlVal = parseInt(control.value.trim());
                }
                catch (e) {
                }

                if (ctrlList != "") {
                    if ((ctrlType == "TextBox") && (control.value.trim() != "")) {
                        control.style.color = "black";
                        if ((control.value.trim().length < minVal) || (control.value.trim().length > maxVal)) {
                            lblMsg.innerHTML = "Minimum/Maximum allowed value exceeds for some data entry fields.";
                            control.style.color = "red";
                            control.focus();
                            return false;
                        }

                    }
                }
            }
            return true;
        }

        function validateSpecialChars(ctrlList, ctrlMaxValidationList) {

            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var ctrls = ctrlList.split("##");
            //DMS5-3897D
            // var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z ]+$/;
            //DMS5-3897A
            var re = /^[\'']+$/;
            var i = 0;

            if (ctrlList != "") {
                for (i = 0; i < ctrls.length; i++) {
                    var ctrlType = ctrls[i].split("|")[1];
                    var ctrl = ctrls[i].split("|")[0];


                    var control = document.getElementById("ctl00_ContentPlaceHolder1_" + ctrl);

                    if ((ctrlType == "TextBox") && (control.value.trim() != "")) {
                        control.style.color = "black";
                        //DMS5-3897M
                        if ((re.test(control.value.trim()))) {
                            lblMsg.innerHTML = "Data entry fields will not allow special character single quote(').";
                            control.style.color = "red";
                            control.focus();
                            return false;
                        }

                    }
                }
            }
            return validateMaxValue(ctrlMaxValidationList);
        }

        // To validate textbox data mismatch
        function ValidateDataMismatch(txtBoxOneID, txtBoxTwoID, LabelName) {
            var lblMaskErrorMessage = document.getElementById("<%= lblMessage.ClientID %>");
            var txtBoxOneControlID = document.getElementById("ctl00_ContentPlaceHolder1_" + txtBoxOneID)
            var txtBoxOneValue = txtBoxOneControlID.value;
            var txtBoxTwoControlID = document.getElementById("ctl00_ContentPlaceHolder1_" + txtBoxTwoID)
            var txtBoxTwoValue = txtBoxTwoControlID.value;
            var LabelID = document.getElementById("ctl00_ContentPlaceHolder1_" + LabelName)
            var LabelText = LabelID.textContent;
            // DMS5 - 3960 added textboxtwo length condition > 0
            if (txtBoxOneValue.toLowerCase().trim() != txtBoxTwoValue.toLowerCase().trim() && txtBoxOneValue.length != txtBoxTwoValue.length && txtBoxTwoValue.length > 0) {
                lblMaskErrorMessage.innerHTML = "Data entered in the first field not matching with retype , Field name : " + LabelText + " ";
                //                //Setting Session Value if Data Mismatch is there
                sessionStorage.setItem("DataNotMatching", "Error");
                $("body").scrollTop(0);
                return false;
            }
            else {
                lblMaskErrorMessage.innerHTML = "";
            }
        }



        //DMS5-4332 BS
        function ValidateMandatoryData(ctrlList, ctrlSPCharList, ctrlMaxValidationList) {
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var statusdropdown = document.getElementById("<%= DDLStatus.ClientID %>");
            if (statusdropdown.value == "0" || statusdropdown.value == undefined) {
                lblMsg.innerHTML = "Please select the status.";
                $("body").scrollTop(0);
                return false;
            }

            //show confirmation message defined for the status
            // Message will be set to hidden variable when status changed.
            var confirmMessage = document.getElementById("<%= hdnStatusMessage.ClientID %>").value;

            var status = true;
            if (confirmMessage != typeof (undefined) && confirmMessage.length > 0)
                status = window.confirm(confirmMessage);

            if (status)
                ValidateData(ctrlList, ctrlSPCharList, ctrlMaxValidationList);
            else
                return false;
        }
        // DMS5-4332 BE

        function ValidateData(ctrlList, ctrlSPCharList, ctrlMaxValidationList) {

            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            lblMsg.innerHTML = "";
            var ctrls = ctrlList.split("##");
            var i = 0;
            if (ctrlList != "") {
                for (i = 0; i < ctrls.length; i++) {
                    var ctrlType = ctrls[i].split("|")[1];
                    var ctrl = ctrls[i].split("|")[0];


                    var control = document.getElementById("ctl00_ContentPlaceHolder1_" + ctrl);
                    control.style.color = "black";
                    if ((ctrlType == "DropDown") && (control.value <= 0)) {
                        lblMsg.innerHTML = "Please provide all mandatory data.";
                        control.focus();
                        return false;

                    }
                    else if ((ctrlType == "TextBox") && (control.value.trim() == "")) {
                        lblMsg.innerHTML = "Please provide all mandatory data.";
                        control.style.color = "red";
                        control.focus();
                        return false;
                    }
                    else if (ctrlType == "MultiDropDown") {

                        var checkbox = control.getElementsByTagName("input");
                        var counter = 0;
                        for (var j = 0; j < checkbox.length; j++) {
                            if (checkbox[j].checked) {
                                counter++;
                                break;
                            }
                        }
                        if (counter <= 0) {
                            lblMsg.innerHTML = "Please provide all mandatory data.";
                            control.style.color = "red";
                            control.focus();
                            return false;
                        }

                    }
                }


            }

            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save";

            return validateSpecialChars(ctrlSPCharList, ctrlMaxValidationList);
        }

        function SetAction(action) {
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = action;
        }

        function loadImageToViewer(imgPath) {

            // Call annotation library setImage() function to load image to viewer
            setImage(imgPath, false);

            //Fit Width
            setTimeout(function () {
                var zoomSelect = document.getElementById("zoomSelect");
                zoomSelect.selectedIndex = 2;
                $("#zoomSelect").change();
            }, 100);
        }

        function CloseWindow(obj) {
            window.close();
            return false;
        }

        function navigationHandler(action) {
            var PageNo = parseInt(document.getElementById("<%=hdnPageNo.ClientID %>").value, "10");
            var PagesCount = parseInt(document.getElementById("<%=hdnPagesCount.ClientID %>").value, "10");
            if (typeof PageNo != 'undefined' && typeof PagesCount != 'undefined') {

                // First Page
                if (action.toUpperCase() == 'FIRST' && PageNo > 0 && PageNo <= PagesCount && PageNo != 1) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;

                }

                    // Previous page
                else if (action.toUpperCase() == 'PREVIOUS' && PageNo > 1 && PageNo <= PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;

                }

                    // Next page
                else if (action.toUpperCase() == 'NEXT' && PageNo > 0 && PageNo < PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;

                }

                    // Last page
                else if (action.toUpperCase() == 'LAST' && PageNo > 0 && PageNo <= PagesCount && PageNo != PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;

                }

                    // Goto page
                else if (action.toUpperCase() == 'GOTO' && PageNo > 0 && PageNo <= PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;

                }
}
}

function SetDDLControlName(controlId) {
    document.getElementById("<%= hdnCurrentDDL.ClientID %>").value = controlId;
    return true;
}
//function to calculate Difference Age
function CalculateDifferenceInDate(FromDate, ToDate, labelId) {
    var CurDate = document.getElementById("ctl00_ContentPlaceHolder1_" + FromDate);
    var RefDate = document.getElementById("ctl00_ContentPlaceHolder1_" + ToDate);
    var labelagedifference = document.getElementById("ctl00_ContentPlaceHolder1_" + labelId)
    var datearrayTo;
    var datearrayfrom = CurDate.value.split("/");
    //RefDate depends on current date or Refer date
    if (RefDate == null) {
        RefDate = ToDate;
        datearrayTo = RefDate.split("/");
    }
    else { datearrayTo = RefDate.value.split("/"); }
    //Converting dd/mm/yyyy to mm/dd/yyyy
    var df = new Date(datearrayfrom[1] + '/' + datearrayfrom[0] + '/' + datearrayfrom[2]);
    var dt = new Date(datearrayTo[1] + '/' + datearrayTo[0] + '/' + datearrayTo[2]);
    var allMonths = Math.abs(parseInt(Number((df - dt) % 31536000000) / 2628000000));
    var allYears = Math.abs(parseInt(Number((df.getTime() - dt.getTime()) / 31536000000)));
    var DaysDiff = Math.abs(parseInt(Number(((df - dt) % 31536000000) % 2628000000) / 86400000));
    var total = allYears + " years  " + allMonths + " months " + DaysDiff + " Days ";
    if (datearrayfrom.length == 1 || datearrayTo.length == 1) {
        labelagedifference.innerHTML = "";
    }
    else {
        labelagedifference.innerHTML = total;
    }
}
function inputOnlyNumbers(e) {
    var specialKeys = new Array();
    specialKeys.push(8); //Backspace
    specialKeys.push(46); //dot
    var keyCode = e.which ? e.which : e.keyCode
    var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
    return ret;
}


function ShowMD() {
    document.getElementById('frameSendMail').src = 'Workflowsentmail.aspx';
    modelBG.className = "mdBG";
    mb.className = "sentmailbox";
    return false;
}
function HideMD() {
    document.getElementById('frameSendMail').src = 'Workflowsentmail.aspx';
    modelBG.className = "mdNone";
    mb.className = "mdNone";
    return false;
}

function uploadStart(sender, args) {
    var result;

    var filename = args.get_fileName();
    var filext = filename.substring(filename.lastIndexOf(".") + 1).toLowerCase();

    if (filext == 'doc' || filext == 'docx' || filext == 'ppt' || filext == 'pptx'
        || filext == 'xls' || filext == 'xlsx' || filext == 'tif' || filext == 'tiff'
        || filext == 'jpg' || filext == 'bmp' || filext == 'jpeg' || filext == 'png'
        || filext == 'gif' || filext == 'giff' || filext == 'pdf' || filext == 'zip'
    ) {
        return true;
    }
    else {

        var err = new Error();

        err.name = 'My API Input Error';
        err.message = 'Please select supported Files!';

        throw (err);
        return false;
    }
}

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td>
                <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>

            </td>
        </tr>


        <tr>
            <td style="width: 550px; vertical-align: top; padding-left: 10px;">
                <table>


                    <tr>

                        <td>

                            <div style="border: 2px solid #fdb813; border-radius: 8px; background: linear-gradient(to bottom, rgba(228,228,228,1) 92%,rgba(228,228,228,1) 92%,#FFC53C); padding: 16px;">
                                <table>
                                    <tr>
                                        <td style="width: 170px;">
                                            <asp:Label ID="lblWorkItem" runat="server" Text="Workitem ID :"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblWorkItemID" Font-Bold="true" runat="server"></asp:Label>
                                        </td>
                                        <td style="display: none">
                                            <asp:LinkButton ID="lnkShowHistory" runat="server" OnClick="btnShowHistory_Click">History</asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 170px;">
                                            <asp:Label ID="lblProcessName" runat="server" Text="Process Name :"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblProcessDescription" Font-Bold="true" runat="server"></asp:Label>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td class="auto-style1">
                                            <asp:Label ID="lblWorkflowName" runat="server" Text="Workflow Name :"></asp:Label>
                                        </td>
                                        <td class="auto-style1">
                                            <asp:Label ID="lblWorkflowDescription" Font-Bold="true" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblStageName" runat="server" Text="Stage Name :"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblStageDescription" Font-Bold="true" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblUserName" runat="server" Text="User Name :"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblUserDescription" Font-Bold="true" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblDateofReceipt" runat="server" Text="Date and Time of Receipt :"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblDateofReceiptDescription" Font-Bold="true" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <asp:Panel ID="DataEntryControls" runat="server">

                        <tr>
                            <td align="center">
                                <br />
                            </td>
                        </tr>

                        <tr>
                            <td colspan="2">


                                <asp:TabContainer runat="Server" ID="Search" ActiveTabIndex="0" CssClass="TabStyle"
                                    OnActiveTabChanged="Search_ActiveTabChanged" Width="550px">


                                    <asp:TabPanel ID="tabviestatus" runat="server">
                                        <HeaderTemplate>
                                            View Document Status
                                        </HeaderTemplate>
                                        <ContentTemplate>

                                            <div id="divMsg_BS" runat="server" style="color: Red; width:auto; overflow:auto" >
                                                <asp:GridView ID="Grd_TrackStatus" runat="server" AutoGenerateColumns="true"
                                                    CssClass="mGrid" AlternatingRowStyle-CssClass="alt" OnPageIndexChanging="Grd_TrackStatus_PageIndexChanging"
                                                    EmptyDataText="No list are found" CellPadding="1" CellSpacing="1" AllowPaging="True"
                                                    PageSize="10">
                                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                                    <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                                                        PageButtonCount="5" PreviousPageText=" " />
                                                    <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                                                </asp:GridView>
                                            </div>



                                        </ContentTemplate>
                                    </asp:TabPanel>
                                    <asp:TabPanel ID="Tabviewdownload" runat="server">
                                        <HeaderTemplate>
                                            View/Download Documents
                                        </HeaderTemplate>
                                        <ContentTemplate>


                                                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" OnPageIndexChanging="GridView1_PageIndexChanging">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Documents Type">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblcontractno" runat="server" Text='<%# Eval("Document Type") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Documents">
                                                                <ItemTemplate>

                                                                    <asp:ImageButton ID="btncontractdocs" runat="server" ImageUrl="~/Images/view.png" OnClick="btncontractdocs_Click" Width="25px" />
                                                                    <asp:HiddenField ID="hidcontractdocs" runat="server" Value='<%# Eval("Path") %>' />

                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>


                                        
                                        </ContentTemplate>
                                    </asp:TabPanel>

                                </asp:TabContainer>


                                <table class="DataEntryDiv" runat="server" id="table_PrevStage" style="display: none">
                                    <tr>
                                        <td align="center">
                                            <asp:Label ID="Label1" Font-Bold="true" runat="server" Text="Index fields of previous stages"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>

                                            <div style="overflow-y: auto; height: 550px;">
                                                <asp:PlaceHolder ID="ControlPlaceHolder_PrevStage" runat="server"></asp:PlaceHolder>
                                            </div>

                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table class="DataEntryDiv" runat="server" id="table_curStage" style="display: none">
                                    <tr>
                                        <td align="center">
                                            <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="Index fields of current stage"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>

                                            <div style="overflow-y: auto; height: auto;">
                                                <asp:PlaceHolder ID="ControlPlaceHolder_CurStage" runat="server"></asp:PlaceHolder>
                                            </div>

                                        </td>
                                    </tr>
                                </table>
                                <table>
                                    <tr>
                                        <td colspan="2"></td>
                                    </tr>
                                </table>
                                <table class="DataEntryDiv" runat="server" id="table_Information">
                                    <tr>
                                        <td align="center">
                                            <asp:Label ID="lblInformation" Font-Bold="true" runat="server" Text="Information fields "></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%--<asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Always">
                                                <ContentTemplate>--%>
                                            <div style="overflow-y: auto; height: 140px;">
                                                <asp:PlaceHolder ID="ControlPlaceHolder_Information" runat="server"></asp:PlaceHolder>
                                            </div>

                                            <%--</ContentTemplate>
                                            </asp:UpdatePanel>--%>
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <div style="margin-left: 5px; display: none">
                                    <%--<asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
                                        <ContentTemplate>--%>
                                    <asp:Label ID="lblStatus" runat="server" Text="Status:"></asp:Label>
                                    <asp:DropDownList ID="DDLStatus" runat="server" Width="110px" OnSelectedIndexChanged="DDLStatus_SelectedIndexChanged"
                                        AutoPostBack="True">
                                    </asp:DropDownList>
                                    <span style="color: Red; font-size: medium">*</span>
                                    <%--    </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="DDLStatus" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>--%>
                                </div>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td>
                  <%--          <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                <ContentTemplate>--%>
                                    <span style="margin-left: 50px">
                                        <asp:Button ID="btnSave" TagName="Add" runat="server" Text="Submit" OnClick="btnSave_Clcik"
                                            CssClass="btnsave" Style="display: none" />
                                        <asp:Button ID="btnsubmitandMail" TagName="Add" runat="server" Text="Mail"
                                            CssClass="btnemail" OnClientClick="return ShowMD();" />
                                        <asp:Button ID="btnCancel" TagName="Read" CausesValidation="false" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                                            CssClass="btncancel" />
                                        <asp:Button ID="btnClose" TagName="Read" runat="server" Text="Close" OnClientClick="return CloseWindow(this);"
                                            CssClass="btncancel" />
                                    </span>
                                    <asp:HiddenField ID="hdnSaveStatus" runat="server" />
                                    <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnPagesCount" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnPageNo" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnCurrentDDL" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnSelectionControl" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnStatusMessage" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnSrc" runat="server" Value="" />
                                    <asp:HiddenField ID="HidPreviousstate" runat="server" Value="" />
                                    <asp:HiddenField ID="HidPreviousStatus" runat="server" Value="" />
                        <%--        </ContentTemplate>
                            </asp:UpdatePanel>--%>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 600px" valign="top">

           <%--     <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Always" RenderMode="Inline">
                    <ContentTemplate>--%>
                        <asp:Panel ID="NavigatePanel" runat="server">
                            <%--          <input onclick="firstPage();" name="First" value="<< " type="button" class="ButtonStyle" />
                            <input onclick="previousPage();" name="Previous" value="< " type="button" class="ButtonStyle" />
                            <input onclick="nextPage();" name="Next" value=" >" type="button" class="ButtonStyle" />
                            <input onclick="lastPage();" name="Last" value=" >>" type="button" class="ButtonStyle" />--%>
                            <%--               <asp:UpdatePanel ID="upPagecount" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:DropDownList ID="ddlpagecount"  runat="server">
                                    </asp:DropDownList>
                                    

                                </ContentTemplate>
                            </asp:UpdatePanel>--%>
                        </asp:Panel>
            <%--        </ContentTemplate>
                </asp:UpdatePanel>--%>
<%--                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>--%>
                        <asp:ImageButton ID="ImgDwnld" runat="server" Style="padding-left: 75%" ImageUrl="~/Images/Download.png" OnClick="ImgDwnld_Click" /><br />
                        <iframe id="myiframe" height="650" width="800" runat="server"></iframe>

             <%--       </ContentTemplate>

                </asp:UpdatePanel>--%>
                <div style="display: none">
                 <%--   <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>--%>
                            <uc1:WorkflowPDFViewer ID="WFPDFViewer" runat="server" />
                            <asp:Label ID="lblMessageImg" ForeColor="Red" runat="server"></asp:Label>
                    <%--    </ContentTemplate>
                    </asp:UpdatePanel>--%>
                </div>
            </td>
        </tr>

    </table>
    <div>
    </div>


    <div id="modelBG" class="mdNone">
    </div>
    <div id="mb" class="mdNone">
        <div id="Content">
            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/DeleteIcon.png" Style="float: right; margin-top: -10px;" OnClientClick="return HideMD();" />
            <iframe id="frameSendMail" width="1285px" height="500px" scrolling="auto"></iframe>
        </div>
    </div>
</asp:Content>
