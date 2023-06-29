<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageGuidedDataEntry.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageGuidedDataEntry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript" src="../WorkflowJS/jquery.min.js"></script>
    <script type="text/javascript" language="javascript" src="../WorkflowJS/jquery.Jcrop.js"></script>
    <script language="Javascript" type="text/javascript">

        //       
        //        $(document).ready(function () {
        //                        ResetFocus();
        //                    });
        //DMS5-4191
        //It works when the control is focused
        function pageLoad() {
            ResetFocus();
        }

        function ddlDropChange() {
            document.getElementById("<%= btnDDLDrop.ClientID %>").click();
        }


        function ResetFocus() {
            $("input[type=text],select").focus(function (e) {
                //Get values from textbox title
                var myString = $(this).attr('title'); // x,y,x1,y1,width,height,pageNo,ImageWidth,ImageHeight
                if (myString == "") return;

                //Function to split based on "|"
                var myArray = myString.split('|');
                if (myArray != 'undefined' && myArray != null) {
                    //Assign each coordinate values in variable
                    var x1 = myArray[0], y1 = myArray[1], x2 = myArray[2], y2 = myArray[3], cordWidth = myArray[4],
                    cordHieght = myArray[5], PageNo = myArray[6], ImageWidth = myArray[7], ImageHeight = myArray[8];

                    // Load Image based on page no.
                    //DMS5-3969 A -- Throwing error when tabbed to the field for which co-ordinate is not captured.
                    if (PageNo != typeof (undefined) && PageNo > 0) {
                        if (document.getElementById("<%=ImageFrame.ClientID %>") != null && document.getElementById("<%=ImageFrame.ClientID %>") != undefined) {
                            document.getElementById("<%=DDLDrop.ClientID %>").value = PageNo;
                            //binding the pagenumber to each control and binding the image accordingly
                            var ImageSrc = document.getElementById("<%=ImageFrame.ClientID %>").src
                            var Imagepath = "", OldFile = "", NewFile = ""
                            OldFile = ImageSrc.split('/').pop();
                            NewFile = PageNo + "." + ImageSrc.split('.').pop()
                            Imagepath = ImageSrc.replace(OldFile, NewFile); //new image with the page the control belongs to

                            document.getElementById("<%=ImageFrame.ClientID %>").src = Imagepath;

                            //Get height and width of div
                            var prev_img = $("[id$=divcropbox]");
                            //to get the div id inwhich the image is located              

                            var prev_width = prev_img.width();
                            var prev_height = prev_img.height();
                            var rx = (prev_width / cordWidth);
                            var ry = (prev_height / cordHieght);

                            // Set CSS for image to zoom
                            jQuery(document.getElementById("<%=ImageFrame.ClientID %>")).css({
                                width: Math.round(rx * ImageWidth) + 'px',
                                height: Math.round(ry * ImageHeight) + 'px',
                                marginLeft: '-' + Math.round(rx * x1) + 'px',
                                marginTop: '-' + Math.round(ry * y1) + 'px'
                            });

                            //Correct scroll position
                            prev_img.scrollTop(0);
                            prev_img.scrollLeft(0);

                            // Remove scroll bar
                            prev_img.css({
                                overflowX: 'hidden'
                            });
                        }
                    }
                }
            });
        }

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
            var statusdropdown = document.getElementById("<%= DDLStatus.ClientID %>");
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

            if (statusdropdown.value == "0" || statusdropdown.value == undefined) {
                lblMsg.innerHTML = "Please select the status.";
                $("body").scrollTop(0);
                return false;
            }

            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save";

            return validateSpecialChars(ctrlSPCharList, ctrlMaxValidationList);
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
    </script>
    <style type="text/css">
        .HiddenButton
        {
            visibility: hidden;
            text-align: center;
        }
        
        .DataEntryDiv
        {
            background: rgb(240,240,240); /* Old browsers */
            background: -moz-linear-gradient(top,  rgba(240,240,240,1) 0%, rgba(255,255,255,1) 92%, rgba(187,213,126,1) 100%); /* FF3.6+ */
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,rgba(240,240,240,1)), color-stop(92%,rgba(255,255,255,1)), color-stop(100%,rgba(187,213,126,1))); /* Chrome,Safari4+ */
            background: -webkit-linear-gradient(top,  rgba(240,240,240,1) 0%,rgba(255,255,255,1) 92%,rgba(187,213,126,1) 100%); /* Chrome10+,Safari5.1+ */
            background: -o-linear-gradient(top,  rgba(240,240,240,1) 0%,rgba(255,255,255,1) 92%,rgba(187,213,126,1) 100%); /* Opera 11.10+ */
            background: -ms-linear-gradient(top,  rgba(240,240,240,1) 0%,rgba(255,255,255,1) 92%,rgba(187,213,126,1) 100%); /* IE10+ */
            background: linear-gradient(to bottom,  rgba(240,240,240,1) 0%,rgba(255,255,255,1) 92%,rgba(187,213,126,1) 100%); /* W3C */
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#f0f0f0', endColorstr='#bbd57e',GradientType=0 ); /* IE6-9 */
            padding: 15px;
            border: 2px solid #87a93e;
            overflow: auto;
            overflow: scroll;
            width: 490px;
            border-radius: 8px;
        }
    </style>
    <script language="javascript" type="text/javascript">

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

        function ValidateMandatoryData(ctrlList, ctrlSPCharList, ctrlMaxValidationList) {
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var statusdropdown = document.getElementById("<%= DDLStatus.ClientID %>");
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

            if (statusdropdown.value == "0" || statusdropdown.value == undefined) {
                lblMsg.innerHTML = "Please select the status.";
                return false;
            }

            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save";

            return validateSpecialChars(ctrlSPCharList, ctrlMaxValidationList);
        }

        function SetAction(action) {
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = action;
        }



        function CloseWindow(obj) {
            window.close();
            return false;
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
            var allYears = Math.abs(parseInt(((df.getTime() - dt.getTime()) / 31536000000)));
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


    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="GVDiv">
        <table>
            <tr>
                <td style="vertical-align: top;">
                    <div style="border: 2px solid #87a93e; border-radius: 8px; background-color: #DADADA;
                        width: 1200px;">
                        <table>
                            <tr>
                                <td style="width: 170px;">
                                    <asp:Label ID="lblWorkItem" runat="server" Text="Workitem ID :"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblWorkItemID" Font-Bold="true" runat="server"></asp:Label>
                                </td>
                                <td>
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
                                <td>
                                    <asp:Label ID="lblWorkflowName" runat="server" Text="Workflow Name :"></asp:Label>
                                </td>
                                <td>
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
        </table>
        <table>
            <tr>
                <td style="width: 600px" valign="top">
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <asp:Label ID="lblMessageImg" ForeColor="Red" runat="server"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <table id="Table1" style="width: 1200px; max-width: 1200px; border: 2px solid #87a93e;
                        border-radius: 8px;" runat="server">
                        <tr>
                            <td align="center">
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <div id="divcropbox" style="overflow: auto; height: 190px; max-width: 1200px;">
                                    <img id="ImageFrame" runat="server" src="" alt="" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanelPaging" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <asp:Label ID="lblPages" runat="server" Text="Page" CssClass="labelStyle"></asp:Label>
                            <asp:DropDownList ID="DDLDrop" runat="server" TagName="Read" CssClass="ddlPage">
                            </asp:DropDownList>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <asp:Button ID="btnDDLDrop" runat="server" class="HiddenButton" OnClick="btnDDLDrop_Click" />
            <asp:Panel ID="GuidedControls" runat="server">
                <tr>
                    <td>
                        <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table id="table_PrevStage" style="overflow: scroll; width: 1200px; height: 200px;
                            border: 1px solid #87a93e; border-radius: 8px;">
                            <tr>
                                <td>
                                    <asp:Label ID="lblPrevStage" Font-Bold="true" runat="server" Text="Index fields of previous stages"></asp:Label>
                                    <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <div id="divPreStage" runat="server" style="overflow: scroll; height: 300px; width: 600px;">
                                                <asp:PlaceHolder ID="ControlPlaceHolder_PrevStage" runat="server"></asp:PlaceHolder>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                                <td>
                                    <asp:Label ID="lblCurrentStage" runat="server" Font-Bold="true" Text="Index fields of current stage"></asp:Label>
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <div id="divCurStage" runat="server" style="overflow: scroll; height: 300px; width: 600px;">
                                                <asp:PlaceHolder ID="ControlPlaceHolder_CurStage" runat="server"></asp:PlaceHolder>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Label ID="lblInformation" Font-Bold="true" runat="server" Text="Information fields "
                                        Visible="false"></asp:Label>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <div style="overflow-y: auto; height: 210px;">
                                                <asp:PlaceHolder ID="ControlPlaceHolder_Information" runat="server"></asp:PlaceHolder>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </asp:Panel>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <span style="margin-left: 115px"><span style="color: Red; font-size: medium">*</span>
                                </span>
                                <asp:Label ID="lblStatus" runat="server" Text="Status :"></asp:Label>
                                <asp:DropDownList ID="DDLStatus" runat="server" OnSelectedIndexChanged="DDLStatus_SelectedIndexChanged"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Button ID="btnSave" runat="server" Text="Submit" CssClass="btnsave" OnClick="btnSave_Click" />
                            </td>
                            <td>
                                <asp:Button ID="btnsubmitandMail" TagName="Add" runat="server" Text="Mail" CssClass="btnemail"
                                    OnClientClick="return ShowMD();" />
                            </td>
                            <td>
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btncancel" TagName="Read"
                                    OnClick="btnCancel_Click" CausesValidation="false" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="return CloseWindow(this);"
                        CssClass="btncancel" />
                    <%--   </span>--%>
                    <asp:HiddenField ID="hdnSaveStatus" runat="server" />
                    <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                    <asp:HiddenField ID="hdnPagesCount" runat="server" Value="1" />
                    <asp:HiddenField ID="hdnPageNo" runat="server" Value="1" />
                    <asp:HiddenField ID="hdnCurrentDDL" runat="server" Value="" />
                    <asp:HiddenField ID="hdnSelectionControl" runat="server" Value="" />
                    <asp:HiddenField ID="hdnLoadImage" runat="server" Value="0" />
                    <asp:HiddenField ID="hdnStatusMessage" runat="server" Value="" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <asp:Button ID="btnShowPopup" runat="server" Style="display: none" />
                            <ajax:ModalPopupExtender ID="ModalPopup" runat="server" TargetControlID="btnShowPopup"
                                PopupControlID="pnlpopup" CancelControlID="imgClosePopUp" OkControlID="btnNo"
                                BackgroundCssClass="modalBackground">
                            </ajax:ModalPopupExtender>
                            <asp:Panel ID="pnlpopup" runat="server" Style="display: none">
                                <div class="GVDiv">
                                    <asp:ImageButton ID="imgClosePopUp" runat="server" Style="float: right" ImageUrl="~/Images/close.png" />
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:GridView ID="GridHistory" runat="server" AllowPaging="true" OnPageIndexChanging="GridHistory_Paging"
                                                    CssClass="mGrid" PagerStyle-CssClass="pgr" PageSize="10" AlternatingRowStyle-CssClass="alt"
                                                    CellPadding="10" CellSpacing="5" EmptyDataText="No records found !">
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Button ID="btnNo" runat="server" Text="OK" Width="100" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </asp:Panel>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        <div id="modelBG" class="mdNone">
        </div>
        <div id="mb" class="mdNone">
            <div id="Content">
                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/DeleteIcon.png"
                    Style="float: right; margin-top: -10px;" OnClientClick="return HideMD();" />
                <iframe id="frameSendMail" width="1285px" height="500px" scrolling="auto"></iframe>
            </div>
        </div>
    </div>
</asp:Content>
