<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageFormDataEntry.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageFormDataEntry" %>
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
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
        .focus
        {
            outline-size: 0.1px;
            outline-style: solid;
            outline-color: #C0C0C0;
        }
        
    </style>
    <script language="javascript" type="text/javascript">
        //synchronous scrolling
        $(document).ready(function () {

            //Focusing the first control on the control binded div
            $("[id$=ControlPlaceHolder_CurStage] input").first().focus(); //textbox
            $("[id$=ControlPlaceHolder_CurStage] select").first().focus(); //dropdown
            $("[id$=ControlPlaceHolder_CurStage] checkbox").first().focus(); //checkbox
            $("[id$=ControlPlaceHolder_CurStage] file").first().focus(); //file upload

            $('input[type="checkbox"]').focus(function () {
                $(this).addClass("focus");
            });
            $('input[type="checkbox"]').blur(function () {
                $(this).removeClass("focus");
            });

            $('input[type="text"]').focus(function () {
                $(this).addClass("focus");
            });
            $('input[type="text"]').blur(function () {
                $(this).removeClass("focus");
            });
            $('input[type="file"]').focus(function () {
                $(this).addClass("focus");
            });
            $('input[type="file"]').blur(function () {
                $(this).removeClass("focus");
            });
            $('select').focus(function () {
                $(this).addClass("focus");
            });
            $('select').blur(function () {
                $(this).removeClass("focus");
            });

            var $tb1 = $("[id$=ControlPlaceHolder_CurStage]");
            var $tb2 = $("[id$=divcropbox]");
            $tb1.scroll(function () {
                $tb2.scrollTop($tb1.scrollTop());
                $tb2.scrollLeft($tb1.scrollLeft());
            });

        });

        function setFocus() {          

            $('input[type="checkbox"]').focus(function () {
                $(this).addClass("focus");
            });
            $('input[type="checkbox"]').blur(function () {
                $(this).removeClass("focus");
            });

            $('input[type="text"]').focus(function () {
                $(this).addClass("focus");
            });
            $('input[type="text"]').blur(function () {
                $(this).removeClass("focus");
            });
            $('input[type="file"]').focus(function () {
                $(this).addClass("focus");
            });
            $('input[type="file"]').blur(function () {
                $(this).removeClass("focus");
            });

            $('select').focus(function () {
                $(this).addClass("focus");
            });
            $('select').blur(function () {
                $(this).removeClass("focus");
            });

            var $tb1 = $("[id$=ControlPlaceHolder_CurStage]");
            var $tb2 = $("[id$=divcropbox]");
            $tb1.scroll(function () {
                $tb2.scrollTop($tb1.scrollTop());
                $tb2.scrollLeft($tb1.scrollLeft());
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
                    if (control != null && control != undefined) {
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
                    if (control != null && control != undefined) {
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
            }
            return validateMaxValue(ctrlMaxValidationList);
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
            var lblMsg = document.getElementById("<%= lblErrorMessage.ClientID %>");
            var statusdropdown = document.getElementById("<%= DDLStatus.ClientID %>");
            lblMsg.innerHTML = "";
            var ctrls = ctrlList.split("##");

            var i = 0;
            if (ctrlList != "") {
                for (i = 0; i < ctrls.length; i++) {
                    var ctrlType = ctrls[i].split("|")[1];
                    var ctrl = ctrls[i].split("|")[0];


                    var control = document.getElementById("ctl00_ContentPlaceHolder1_" + ctrl);

                    if (control != null && control != undefined) {
                        control.style.color = "black";
                        if ((ctrlType == "DropDown") && (control.value <= 0)) {
                            lblMsg.innerHTML = "Please provide all mandatory data.";
                            control.focus();
                            return false;

                        }
                        else if ((ctrlType == "TextBox") && (control.value.trim() == "")) {
                            lblMsg.innerHTML = "Please provide all mandatory data.";
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
                                control.focus();
                                return false;
                            }

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
        // To validate textbox data mismatch
        function ValidateDataMismatch(txtBoxOneID, txtBoxTwoID, LabelName) {
            var lblMaskErrorMessage = document.getElementById("<%= lblMessage.ClientID %>");
            var txtBoxOneControlID = document.getElementById("ctl00_ContentPlaceHolder1_" + txtBoxOneID)
            var txtBoxOneValue = txtBoxOneControlID.value;
            var txtBoxTwoControlID = document.getElementById("ctl00_ContentPlaceHolder1_" + txtBoxTwoID)
            var txtBoxTwoValue = txtBoxTwoControlID.value;
            var LabelID = document.getElementById("ctl00_ContentPlaceHolder1_" + txtBoxOneID + "_Label")
            var LabelText = LabelID.textContent;
           // txtBoxTwoControlID.style.visibility = "hidden";
            $(txtBoxOneControlID).on('focusout', function () {
                txtBoxTwoControlID.style.visibility = "visible";
                // txtBoxTwoControlID.title = "Please re-enter '" + LabelText+"'";
                //txtBoxTwoControlID.focus();
            });
            $(txtBoxTwoControlID).on('focusout', function () {
                txtBoxOneControlID.style.visibility = "visible";
                txtBoxTwoControlID.style.visibility = "hidden";
               // txtBoxOneControlID.focus();
            });
            // DMS5 - 3960 added textboxtwo length condition > 0
            if (txtBoxOneValue.toLowerCase().trim() != txtBoxTwoValue.toLowerCase().trim() && txtBoxTwoValue.length > 0 && txtBoxOneValue.length != txtBoxTwoValue.length) {
                lblMaskErrorMessage.innerHTML = "Data entered in the first field not matching with retype. Field: " + LabelText;         
                $("body").scrollTop(0);
                return false;
            }
            else {
                lblMaskErrorMessage.innerHTML = "";
            }
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
            var partialMonths = Math.abs(parseInt(dt.getMonth() - df.getMonth()));
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
    <div class="GVDiv">
        <table runat="server" id="tblDetails">
            <tr>
                <td>
                    <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top;">
                    <div style="border: 1px solid #87a93e; border-radius: 8px; background-color: #DADADA">
                
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
                <td style="width: 600px" valign="top" id="tblMessageImg" runat="server">
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <asp:Label ID="lblMessageImg" ForeColor="Red" runat="server"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
       
        <table id="tblImageSection" runat="server">
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblErrorMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
               
               
                <td colspan="2" >
                 <asp:Panel ID="PanelFormControls" runat="server">
                    <asp:Label ID="lblCurrentStage" runat="server" Font-Bold="true" Text="Data Entry"></asp:Label>
                    <table runat="server" id="table_curStage" style="border: 1px solid #87a93e;">
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <div id="ControlPlaceHolder_CurStage" runat="server" style="height: 400px;
                                            position: relative; overflow: auto;">
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                           
                            </td>
                        </tr>
                    </table>
                    </asp:Panel>
                </td>
                 
                
                <td colspan="2" id="tdTable_PrevStage" runat="server">
                    <asp:Label ID="lblOriginalImage" runat="server" Text="Original Image" Font-Bold="true"></asp:Label>
                    <table id="table_PrevStage" style="border: 1px solid #87a93e;">
                        <tr>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
                                    <ContentTemplate>
                                        <div id="divcropbox" style="overflow: scroll; width: 600px; height: 400px;" runat="server">
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>                   
                    <table>
                        <tr>
                            <td>
                                <span style="margin-left: 115px">
                                    <asp:Label ID="lblStatus" runat="server" Text="Status :"></asp:Label>
                                    <asp:DropDownList ID="DDLStatus" runat="server" OnSelectedIndexChanged="DDLStatus_SelectedIndexChanged"
                                            AutoPostBack="True">
                                    </asp:DropDownList>
                                    <span style="color: Red; font-size: medium">*</span> </span>
                            </td>
                            <td>
                                <asp:HiddenField ID="hdnlastSortOrder" runat="server" Value="0" />
                                
                            </td>
                            <td>
                            <asp:Button ID="btnSave"  TagName="Add"  runat="server" Text="Submit" CssClass="btnsave" OnClick="btnSave_Click" />
                                        <asp:Button ID="btnsubmitandMail" TagName="Add" runat="server" Text="Mail"
                                            CssClass="btnemail" OnClientClick="return ShowMD();"/>
                            </td>
                           
                            <td>
                                <asp:Button ID="btnCancel" CausesValidation="false" TagName="Read" runat="server" Text="Cancel" CssClass="btncancel" OnClick="btnCancel_Click" />
                            </td>
                        </tr>
                    </table>
                  
                </td>
            </tr>
            <tr>
                <td>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                        <ContentTemplate>
                            <span style="margin-left: 200px">
                                <asp:Button ID="btnClose" TagName="Read" runat="server" Text="Close" OnClientClick="return CloseWindow(this);"
                                    CssClass="btncancel" />
                            </span>
                            <asp:HiddenField ID="hdnSaveStatus" runat="server" />
                            <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                            <asp:HiddenField ID="hdnPagesCount" runat="server" Value="" />
                            <asp:HiddenField ID="hdnPageNo" runat="server" Value="" />
                            <asp:HiddenField ID="hdnCurrentDDL" runat="server" Value="" />
                            <asp:HiddenField ID="hdnSelectionControl" runat="server" Value="" />
                            <asp:HiddenField ID="hdnFielExtension" runat="server" Value="" />
                            <asp:HiddenField ID="hdnDataType" runat="server" />
                            <asp:HiddenField ID="hdnX1" runat="server" />
                            <asp:HiddenField ID="hdnX2" runat="server" />
                            <asp:HiddenField ID="hdnY1" runat="server" />
                            <asp:HiddenField ID="hdnY2" runat="server" />
                            <asp:HiddenField ID="hdnWidth" runat="server" />
                            <asp:HiddenField ID="hdnHeight" runat="server" />
                            <asp:HiddenField ID="hdnShowBackgroundImage" runat="server" Value="true" />
                             <asp:HiddenField ID="hdnStatusMessage" runat="server" Value="" />
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
        
    </div>
    
    <div id="modelBG" class="mdNone">
    </div>
    <div id="mb" class="mdNone">
        <div id="Content">
           <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/Images/DeleteIcon.png" style="float: right;margin-top: -10px;" OnClientClick="return HideMD();"/>
        <iframe id="frameSendMail"  width="1285px" height="500px" scrolling="auto"></iframe>
        </div>
        </div>
</asp:Content>
