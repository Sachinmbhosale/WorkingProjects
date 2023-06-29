<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="WorkflowDataEntry.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkflowDataEntry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/Workflow/WorkflowAdmin/WorkflowPDFViewer.ascx" TagName="WorkflowPDFViewer"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
            <td style="width: 400px; border-right: 2px double black; vertical-align: top;">
                <table>
             
                    <tr>
                        <td>
                            <div style="border: 2px solid #87a93e; border-radius: 8px; background-color: #DADADA">
                                <table>
                                     <tr>
                                        <td style="width: 170px;">
                                            <asp:Label ID="lblWorkItem" runat="server" Text="Workitem ID :"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblWorkItemID" Font-Bold="true" runat="server"></asp:Label>
                                        </td>
                                         <td>  
                                            <asp:LinkButton ID="lnkShowHistory" runat="server"  OnClick="btnShowHistory_Click">History</asp:LinkButton>
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
                           <table><tr><td colspan="2"></td></tr></table>
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
                            <br />
                            <div style="margin-left: 115px">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Always">
                                    <ContentTemplate>
                                        <asp:Label ID="lblStatus" runat="server" Text="Status :"></asp:Label>
                                        <asp:DropDownList ID="DDLStatus" runat="server" OnSelectedIndexChanged="DDLStatus_SelectedIndexChanged"
                                            AutoPostBack="True">
                                        </asp:DropDownList>
                                        <span style="color: Red; font-size: medium">*</span>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>    
                     </asp:Panel>             
                   <tr>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <span style="margin-left: 200px">
                                        <asp:Button ID="btnSave" TagName="Add" runat="server" Text="Submit" OnClick="btnSave_Clcik"
                                            CssClass="btnsave" />
                                             <asp:Button ID="btnsubmitandMail" TagName="Add" runat="server" Text="Mail"
                                            CssClass="btnemail" OnClientClick="return ShowMD();"/>
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

         
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Button ID="btnShowPopup" runat="server" Style="display: none" />
                  
                        <ajax:ModalPopupExtender ID="ModalPopup" runat="server" TargetControlID="btnShowPopup"
                            PopupControlID="pnlpopup" CancelControlID="imgClosePopUp" OkControlID="btnNo" BackgroundCssClass="modalBackground">
                        </ajax:ModalPopupExtender>
                        <asp:Panel ID="pnlpopup" runat="server"  Style="display: none">                   
                            <div class="GVDiv">
                              <asp:ImageButton ID="imgClosePopUp" runat="server" Style="float: right" 
                    ImageUrl="~/Images/close.png" />
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
                                        <td align="center" >
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
            <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/DeleteIcon.png" style="float: right;margin-top: -10px;" OnClientClick="return HideMD();" />
        <iframe id="frameSendMail"  width="1285px" height="500px" scrolling="auto"></iframe>
        </div>
        </div>
</asp:Content>
