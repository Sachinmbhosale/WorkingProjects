<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkFlowDataEntry_View.aspx.cs"
    Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkFlowDataEntry_View" %>

<%@ Register Src="~/Workflow/WorkflowAdmin/WorkflowPDFViewer.ascx" TagName="WorkflowPDFViewer"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
</head>
<body style="font-family: Sans-Serif; font-size: small;">
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
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
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
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z ]+$/;

            var i = 0;

            if (ctrlList != "") {
                for (i = 0; i < ctrls.length; i++) {
                    var ctrlType = ctrls[i].split("|")[1];
                    var ctrl = ctrls[i].split("|")[0];


                    var control = document.getElementById("ctl00_ContentPlaceHolder1_" + ctrl);

                    if ((ctrlType == "TextBox") && (control.value.trim() != "")) {
                        control.style.color = "black";
                        if (!(re.test(control.value.trim()))) {
                            lblMsg.innerHTML = "Data entry fields allow alphabets, numbers and few special characters ( _ - # @ ^ $) only.";
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
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }

                // Previous page
                else if (action.toUpperCase() == 'PREVIOUS' && PageNo > 1 && PageNo <= PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }

                // Next page
                else if (action.toUpperCase() == 'NEXT' && PageNo > 0 && PageNo < PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }

                // Last page
                else if (action.toUpperCase() == 'LAST' && PageNo > 0 && PageNo <= PagesCount && PageNo != PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }

                // Goto page
                else if (action.toUpperCase() == 'GOTO' && PageNo > 0 && PageNo <= PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }
            }
        }

        function SetDDLControlName(controlId) {
            document.getElementById("<%= hdnCurrentDDL.ClientID %>").value = controlId;
            return true;
        }

    </script>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table>
        <tr>
            <td>
                <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 400px; border-right: 2px double black; vertical-align: top;">
                <table>
                    <tr>
                        <td>
                            <div style="border: 2px solid #87a93e; border-radius: 8px; background-color: #DADADA">
                                <table>
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
                    <tr>
                        <td colspan="2">
                            <table class="DataEntryDiv" runat="server" id="table_PrevStage">
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label1" Font-Bold="true" runat="server" Text="Index fields of previous stages"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Always">
                                            <ContentTemplate>
                                                <div style="overflow-y: auto; height: 140px;">
                                                    <asp:PlaceHolder ID="ControlPlaceHolder_PrevStage" runat="server"></asp:PlaceHolder>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <table class="DataEntryDiv" runat="server" id="table_curStage">
                                <tr>
                                    <td align="center">
                                        <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="Index fields of current stage"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                                            <ContentTemplate>
                                                <div style="overflow-y: auto; height: 210px;">
                                                    <asp:PlaceHolder ID="ControlPlaceHolder_CurStage" runat="server"></asp:PlaceHolder>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <span style="margin-left: 115px">
                                <asp:Label ID="lblStatus" runat="server" Text="Status :"></asp:Label>
                                <asp:DropDownList ID="DDLStatus" runat="server">
                                </asp:DropDownList>
                                <span style="color: Red; font-size: medium">*</span> </span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <span style="margin-left: 200px">
                                        <asp:Button ID="btnSave" TagName="Add" runat="server" Text="Submit" OnClick="btnSave_Clcik"
                                            CssClass="btnsave" />
                                        <asp:Button ID="btnCancel" TagName="Read" runat="server" Text="Cancel" OnClick="btnCancel_Click"
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
                                    <asp:Button ID="btnCallFromJavascript" TagName="Read" class="HiddenButton" runat="server"
                                        OnClick="btnCallFromJavascript_Click" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 600px" valign="top">
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <uc1:WorkflowPDFViewer ID="WFPDFViewer" runat="server" />
                        <asp:Label ID="lblMessageImg" ForeColor="Red" runat="server"></asp:Label>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
