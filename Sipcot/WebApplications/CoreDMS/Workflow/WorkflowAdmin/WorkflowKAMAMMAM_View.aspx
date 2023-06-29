<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="WorkflowKAMAMMAM_View.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkflowKAMAMMAM_View" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/Workflow/WorkflowAdmin/WorkflowPDFViewer.ascx" TagName="WorkflowPDFViewer"
    TagPrefix="uc1" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
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

        fieldset {
            padding-bottom: 0;
            padding-top: -30px;
            margin-top: -10px;
        }

        .fldSet table {
            margin-top: -20px;
        }

        .lnk {
            color: blue;
            text-decoration: underline;
        }

        .pgrstyle {
            float: left !important;
            vertical-align: top;
            padding-bottom: 15px !important;
            border: 0px !important;
        }

        .auto-style1 {
            height: 19px;
        }
    </style>
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>

    <script type="text/javascript">

        function ViewDocument(vId) {
            $("#<%=hiddControlId.ClientID%>").val(vId);
            $("#<%=btnViewPDF.ClientID%>").click();
            return false;
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
            return false;
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

                    if (control != null) {

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

function ViewTender(vUrl) {
    window.open(vUrl, "_blank");

    return false;
}

function CalculateDays() {
    //alert(11);
    $("#<%=btncal.ClientID%>").click();

}
function CalculateManDays() {
    //alert(11);
    $("#<%=btncalculate.ClientID%>").click();

}
function DaystakenforExecution() {
    //alert(11);
    $("#<%=btncalculate.ClientID%>").click();

}
function DaystakenforPaymentRealisation() {
    //alert(11);
    $("#<%=btndeductinvoiceAmt.ClientID%>").click();

}
function CustNameLocation() {
    //alert(11);
    $("#<%=btnCustNameLocation.ClientID%>").click();

}
function InvoiceAmount() {
    //alert(11);
    $("#<%=btninvoiceamount.ClientID%>").click();

}

function CalculateAmount() {
    //  alert(11);
    $("#<%=btndeduct.ClientID%>").click();

}

function CalculateNoDays() {
    //  alert(11);
    $("#<%=btnpaymentcal.ClientID%>").click();

}
function BudgetoryCalculateNoDays() {
    //  alert(11);
    $("#<%=btnBudgetoryCal.ClientID%>").click();

}

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width: 100% !important; overflow: hidden;">
        <colgroup>
            <col width="30%" />
            <col width="*" />
        </colgroup>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 20% !important; vertical-align: top; padding: 0px !important">
                <table>
                    <tr id="trWorkFlowDtls" runat="server">
                        <td>
                            <div style="border: 2px solid #fdb813; border-radius: 8px; background: linear-gradient(to bottom, rgba(228,228,228,1) 92%,rgba(228,228,228,1) 92%,#FFC53C); padding: 16px; display: none">
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
                    <asp:Panel ID="DataEntryControls" runat="server">
                        <tr style="display: block">

                            <td colspan="2">
                                <table class="DataEntryDiv" runat="server" id="table_PrevStage">
                                    <tr>
                                        <td align="center">
                                            <span style="text-align: left">
                                                <asp:Label ID="Label1" Font-Bold="true" runat="server" Text="Index fields of previous stages"></asp:Label>
                                            </span>
                                            &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;  <span style="text-align: right">
                                                <asp:LinkButton ID="LnkShowDetails" runat="server" OnClick="LnkShowDetails_Click" Style="text-align: right">Product Details</asp:LinkButton></span>
                                        </td>

                                        <%-- <td style="width: 90px;">--%>
                                        <%--<asp:LinkButton ID="LnkShowDetails" runat="server" OnClick="LnkShowDetails_Click" Style="text-align: right">Product Details</asp:LinkButton>--%>
                                        <%--</td>--%>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div style="overflow-y: auto; height:auto">
                                                <asp:PlaceHolder ID="ControlPlaceHolder_PrevStage" runat="server"></asp:PlaceHolder>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table class="DataEntryDiv" runat="server" id="table_curStage" style="display:none">
                                    <tr>
                                        <td align="center">
                                            <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="Index fields of current stage"></asp:Label>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td>
                                            <div style="overflow-y: auto; height: auto;">
                                                <asp:UpdatePanel ID="UpCurrStage" runat="server" UpdateMode="Always">
                                                    <ContentTemplate>
                                                        <asp:PlaceHolder ID="ControlPlaceHolder_CurStage" runat="server">
                                                            <table>
                                                                <tr>
                                                                    <td style="width: 170px;">
                                                                      
                                                                        <asp:Label ID="lblDaysToReceivePurchaseOrder" runat="server"></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="Label4" Font-Bold="true" runat="server"></asp:Label>

                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </asp:PlaceHolder>

                                                    </ContentTemplate>
                                                    <Triggers>

                                                        <asp:PostBackTrigger ControlID="btnViewPDF" />


                                                    </Triggers>

                                                </asp:UpdatePanel>

                                                <br />
                                                <br />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr style="display:none">
                                        <td>
                                            <asp:GridView ID="Grid_Checklist" runat="server" CssClass="mGrid"></asp:GridView>
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
                                            <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Always">
                                                <ContentTemplate>
                                                    <div style="overflow-y: auto; height: 140px;">
                                                        <asp:PlaceHolder ID="ControlPlaceHolder_Information" runat="server"></asp:PlaceHolder>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                </table>
                                <fieldset class="fldSet" id="fldStatus" runat="server" style="display:none">
                                    <legend style="color: rgb(0,0,0)" class="lgnd">
                                        <h4>Change Status </h4>
                                    </legend>
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" RenderMode="Inline">
                                        <ContentTemplate>
                                            <table width="100%">
                                                <colgroup>
                                                    <col width="30%" />
                                                    <col width="*" />
                                                </colgroup>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblStatus" runat="server" Text="Status:"></asp:Label>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="DDLStatus" runat="server" Width="250px" OnSelectedIndexChanged="DDLStatus_SelectedIndexChanged"
                                                            AutoPostBack="true">
                                                        </asp:DropDownList>
                                                        <span style="color: Red; font-size: medium">*</span>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="DDLStatus" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr style="display:none">
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Inline">
                                <ContentTemplate>
                                    <span style="margin-left: 200px">
                                        <asp:Button ID="btnSave" TagName="Add" runat="server" Text="Submit" OnClick="btnSave_Clcik"
                                            CssClass="btnsave" />
                                        <asp:Button ID="btnsubmitandMail" TagName="Add" runat="server" Text="Mail" Style="display: none"
                                            CssClass="btnemail" OnClientClick="return ShowMD();" />
                                        <asp:Button ID="btnCancel" TagName="Read" CausesValidation="false" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                                            CssClass="btncancel" />
                                        <asp:Button ID="btnClose" TagName="Read" runat="server" Text="Close" OnClientClick="return CloseWindow(this);"
                                            CssClass="btncancel" />

                                        <asp:Button ID="btnViewPDF" TagName="Read" runat="server" Text="View" OnClick="btnViewTender_Click" Style="display: none"
                                            CssClass="btncancel" />
                                        <asp:Button ID="btnviewPreviouslydocs" TagName="Read" runat="server" Text="Pres docs View" Style="display: none"
                                            CssClass="btncancel" OnClick="btnviewPreviouslydocs_Click" />

                                        <asp:Button ID="btnselectProduct" TagName="Read" runat="server" Text="Product"
                                            CssClass="btncancel" OnClick="btnselectProduct_Click" Style="display: none" />
                                        <asp:Button ID="btncalculate" TagName="Read" runat="server" Text="Dif" OnClick="btncalculate_Click" Style="display: none" />
                                        <asp:Button ID="btncal" TagName="Read" runat="server" Text="Dif" OnClick="btncal_Click" Style="display: none" />

                                        <asp:Button ID="btndeduct" TagName="Read" runat="server" Text="Dif" OnClick="btndeduct_Click" Style="display: none" />
                                        <asp:Button ID="btndeductinvoiceAmt" TagName="Read" runat="server" Text="Dif" OnClick="btndeductinvoiceAmt_Click" Style="display: none" />
                                        <asp:Button ID="btninvoiceamount" TagName="Read" runat="server" Text="Dif" OnClick="btninvoiceamount_Click" Style="display: none" />
                                        <asp:Button ID="btnpaymentcal" TagName="Read" runat="server" OnClick="btnpaymentcal_Click" Style="display: none" />
                                        <asp:Button ID="btnBudgetoryCal" TagName="Read" runat="server" OnClick="btnBudgetoryCal_Click" Style="display: none" />
                                        <asp:Button ID="btnCustNameLocation" TagName="Read" runat="server" OnClick="btnCustNameLocation_Click" Style="display: none" />


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
                                    <asp:HiddenField ID="HidStampingStatus" runat="server" Value="" />
                                    <asp:HiddenField ID="HidPreviousStatusID" runat="server" Value="" />
                                    <asp:HiddenField ID="HidQuotationType" runat="server" Value="" />
                                    <asp:HiddenField ID="HidProductType" runat="server" Value="" />
                                    <asp:HiddenField ID="hiddControlId" runat="server" />
                                    <asp:HiddenField ID="HidProductselectedvalue" runat="server" />
                                    <asp:HiddenField ID="HidUniquecodeStamping" runat="server" />
                                    <asp:HiddenField ID="HidStageId" runat="server" />
                                    <asp:HiddenField ID="HidStampingSelectedValue" runat="server" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btndeduct" EventName="Click" />



                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 80% !important" valign="top" id="tr" runat="server">


                <asp:ImageButton ID="imgbuttonRptPopup" runat="server" ImageUrl="~/Images/Reports.jpg" Height="35px" Width="45px" OnClick="imgbuttonRptPopup_Click" />
                <asp:ImageButton ID="ImgDwnld" runat="server" Style="padding-left: 75%; display:none" ImageUrl="~/Images/Download.png" OnClick="ImgDwnld_Click" /><br />
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional" RenderMode="Inline">


                    <ContentTemplate>

                        <table style="width: 100%">
                            <colgroup>
                                <col width="50%" />
                                <col width="50%" />
                            </colgroup>
                            <tr>
                                <td>

                                    <iframe id="myiframe" runat="server"></iframe>
                                </td>
                            </tr>

                        </table>
                        <asp:Button Text="Download Report" runat="server" ID="btnOpenDownlRpt" Style="display: none" />
                        <ajax:ModalPopupExtender ID="mdlDownloadRpt" runat="server" TargetControlID="btnOpenDownlRpt"
                            PopupControlID="pnlDownloadReport" CancelControlID="imgClosePopUp" OkControlID="btnclosePopup" BackgroundCssClass="modalBackground">
                        </ajax:ModalPopupExtender>

                        <asp:Panel ID="pnlDownloadReport" runat="server">
                            <div class="model-table">
                                <div class="GVDiv" style="width: 650px">
                                    <asp:ImageButton ID="ImageButton2" runat="server" Style="float: right"
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
                                                    <rsweb:ReportViewer ID="rptReports" runat="server" Width="600" Visible="false">
                                                    </rsweb:ReportViewer>
                                                    <asp:HiddenField ID="hiddWorkFlowFieldId" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td></td>
                                                <td style="text-align: right">
                                                    <asp:Button ID="btnDownload" runat="server" Text="Download" OnClick="btnDwnlReport_Click" align="right" />
                                                    <asp:Button ID="btnclosePopup" runat="server" Text="Cancel" align="right" />

                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </div>
                            </div>
                        </asp:Panel>


                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnDownload" />



                    </Triggers>
                </asp:UpdatePanel>

                <div style="display: none">
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <uc1:WorkflowPDFViewer ID="WFPDFViewer" runat="server" />
                            <asp:Label ID="lblMessageImg" ForeColor="Red" runat="server"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Button ID="btnShowPopup" runat="server" Style="display: none" />

                        <ajax:ModalPopupExtender ID="ModalPopup" runat="server" TargetControlID="btnShowPopup"
                            PopupControlID="pnlpopup" CancelControlID="imgClosePopUp" OkControlID="btnNo" BackgroundCssClass="modalBackground">
                        </ajax:ModalPopupExtender>

                        <asp:Panel ID="pnlpopup" runat="server">
                            <div class="model-table">
                                <div class="GVDiv" style="width: 650px">
                                    <asp:ImageButton ID="imgClosePopUp" runat="server" Style="float: right"
                                        ImageUrl="~/Images/close.png" />
                                    <table style="width: 100%">
                                        <tr>
                                            <td style="overflow: hidden!important">
                                                <asp:GridView ID="GridHistory" runat="server" AllowPaging="true" OnPageIndexChanging="GridHistory_Paging"
                                                    CssClass="mGrid" PagerStyle-CssClass="pgr" PageSize="10" AlternatingRowStyle-CssClass="alt"
                                                    CellPadding="10" CellSpacing="5" EmptyDataText="No records found !">
                                                </asp:GridView>
                                                <asp:GridView ID="GrdViewDetails" runat="server" AllowPaging="true" OnPageIndexChanging="GrdViewDetails_PagingPaging"
                                                    CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                                    CellPadding="5" CellSpacing="5" EmptyDataText="No records found !">
                                                    <PagerStyle CssClass="pgrstyle" HorizontalAlign="Left" />
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
                            </div>
                        </asp:Panel>
                        </div>
                   
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanelMessage" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="btnProduct" runat="server" Style="display: none" />
                        <ajax:ModalPopupExtender ID="ModalPopProduct" runat="server" TargetControlID="btnProduct"
                            PopupControlID="pnlProduct" CancelControlID="imgprodclose" OkControlID="btnprodOK" BackgroundCssClass="modalPopProd">
                        </ajax:ModalPopupExtender>
                        <asp:Panel ID="pnlProduct" runat="server">

                            <div class="GVDiv" style="width: 1250px; height: 450px;">
                                <asp:ImageButton ID="imgprodclose" runat="server" Style="float: right"
                                    ImageUrl="~/Images/close.png" />
                                <table id="tblproduct" runat="server" style="border: 1px solid #fdb813; border-radius: 8px; padding: 10px; width: 98% !important;">
                                    <tr>
                                        <td>Product Name :</td>
                                        <td>
                                            <asp:DropDownList ID="ddlproductNameMst" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlproductNameMst_SelectedIndexChanged"></asp:DropDownList></td>
                                        <td>Product code :</td>
                                        <td colspan="2" style="padding-left: 10px">
                                            <asp:Label ID="lblproductcode" runat="server" Font-Bold="True" ForeColor="#3333FF"></asp:Label>
                                        </td>
                                        <td>&nbsp;</td>

                                    </tr>
                                    <tr>
                                        <td>Quantity :</td>
                                        <td>
                                            <asp:TextBox ID="txtquantity" runat="server" AutoPostBack="true" OnTextChanged="txtquantity_TextChanged"></asp:TextBox>
                                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Numbers"
                                                TargetControlID="txtquantity" />
                                        </td>
                                        <td>Rate per Unit : </td>
                                        <td>
                                            <asp:Label ID="lblrateperunit" runat="server"></asp:Label>
                                        </td>
                                        <td>Total Value of Order :</td>
                                        <td>
                                            <asp:Label ID="lbltotalvalueoforder" runat="server" Font-Bold="True" ForeColor="#3333FF">
                                                 
                                            </asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Product Batch No :</td>
                                        <td>
                                            <asp:TextBox ID="txtproductbatchno" runat="server"></asp:TextBox></td>
                                        <td>Mfg. Date :</td>
                                        <td>
                                            <asp:TextBox ID="txtMfgDate" runat="server" AutoPostBack="True" OnTextChanged="txtMfgDate_TextChanged" EnableViewState="False"></asp:TextBox>
                                            <img alt="Icon" src="../../Images/cal.gif" id="Image1" />

                                            <ajax:CalendarExtender ID="CalendarExtender1" runat="server"
                                                TargetControlID="txtMfgDate" PopupButtonID="Image1" Format="MM/dd/yyyy" />
                                        </td>
                                        <td>Exp. Date :</td>
                                        <td>
                                            <asp:TextBox ID="txtExpDate" runat="server" EnableViewState="False"></asp:TextBox>
                                            <img alt="Icon" src="../../Images/cal.gif" id="Imageexp" />

                                            <ajax:CalendarExtender ID="CalendarExtender2" runat="server"
                                                TargetControlID="txtExpDate" PopupButtonID="Imageexp" />
                                        </td>

                                    </tr>
                                    <tr>
                                        <td class="auto-style1">Residual Shelf Life :</td>
                                        <td class="auto-style1">
                                            <asp:Label ID="lblresideualshelflife" runat="server" Font-Bold="True" ForeColor="#3333FF"></asp:Label></td>
                                        <td class="auto-style1"></td>
                                        <td class="auto-style1"></td>
                                        <td class="auto-style1"></td>
                                        <td class="auto-style1"></td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td colspan="3" style="text-align: center">

                                            <asp:Label ID="lblmsgprod" runat="server" Font-Bold="True" ForeColor="#3333FF"></asp:Label>
                                        </td>
                                        <td align="center">
                                            <asp:Button ID="btnsubmit" runat="server" Text="Add Product" OnClick="btnsubmit_Click" />
                                            <asp:Button ID="btnprodOK" runat="server" Text="Cancel" CssClass="btn-success" />
                                        </td>
                                        <td>&nbsp;</td>
                                    </tr>
                                </table>

                                <asp:GridView ID="GrdProductWF3" runat="server" Style1="padding-left: 10px" CssClass="mGrid"></asp:GridView>
                                <asp:HiddenField ID="HidProduct" runat="server" />

                            </div>

                        </asp:Panel>





                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnsubmit" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ddlproductNameMst" EventName="SelectedIndexChanged" />



                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>

    </table>
    <div id="modelBG" class="mdNone">
    </div>
    <div id="mb" class="mdNone">
        <div id="Content">
            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/DeleteIcon.png" Style="float: right; margin-top: -10px;" OnClientClick="return HideMD();" />
            <iframe id="frameSendMail" width="1285px" height="500px" scrolling="auto"></iframe>
        </div>
    </div>


</asp:Content>
