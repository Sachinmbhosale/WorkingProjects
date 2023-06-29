<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageStageFeildsEdit.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageStageFeildsEdit" %>

<%@ Register Assembly="AjaxControlToolKit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register TagPrefix="WF" TagName="WorkFlowWizard" Src="WorkFlowWizardMenu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript" src="../WorkflowJS/jquery.min.js"></script>
    <script type="text/javascript" language="javascript" src="../WorkflowJS/jquery.Jcrop.js"></script>
    <link href="<%= Page.ResolveClientUrl("~/SiteStyle.css") %>" rel="stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript">
        // Remember to invoke within jQuery(window).load(...)
        // If you don't, Jcrop may not initialize properly
        jQuery(window).load(function () {
            if ($("[id$=divFormPreview]") != null && $("[id$=divFormPreview]") != undefined && $("[id$=divFormCropbox]") != null && $("[id$=divFormCropbox]") != undefined) {
                var $tb1 = $("[id$=divFormCropbox]");
                var $tb2 = $("[id$=divFormPreview]");
                $tb1.scroll(function () {
                    $tb2.scrollTop($tb1.scrollTop());
                });
            }

            $('#<%=cropbox.ClientID %>').Jcrop({
                onChange: showPreview, //get the preview of document
                onSelect: showCoords, //Get the coordinates
                aspectRatio: 0,
                setSelect: [10, 10, 10, 10],
                minSize: [10, 10]
            });



            $('#<%=FormCropbox.ClientID %>').Jcrop({
                onSelect: getCoords, //Get the coordinates
                onChange: getPreview,
                aspectRatio: 0,
                setSelect: [10, 10, 10, 10],
                minSize: [10, 10]
            });




            var hiddenstageid = document.getElementById("<%= HiddenStageFieldId.ClientID%>");
            if (hiddenstageid != null && hiddenstageid != undefined) {
                if (hiddenstageid.value == "") {

                    document.getElementById("<%= RadioFeildDataType.ClientID %>").disabled = true;
                    document.getElementById("<%= txtMin.ClientID %>").disabled = true;
                    document.getElementById("<%= txtMax.ClientID %>").disabled = true;

                }
                EnableDisableType();
            }

        });

        //Get the coordinates
        function showCoords(c) {
            $("#<%=hdnX1.ClientID %>").val(c.x);
            $("#<%=hdnY1.ClientID %>").val(c.y);
            $("#<%=hdnX2.ClientID %>").val(c.x2);
            $("#<%=hdnY2.ClientID %>").val(c.y2);
            $("#<%=hdnWidth.ClientID %>").val(c.w);
            $("#<%=hdnHeight.ClientID %>").val(c.h);
        };

        function getCoords(c) {
            $("#<%=hdnX1.ClientID %>").val(c.x);
            $("#<%=hdnY1.ClientID %>").val(c.y);
            $("#<%=hdnX2.ClientID %>").val(c.x2);
            $("#<%=hdnY2.ClientID %>").val(c.y2);
            $("#<%=hdnWidth.ClientID %>").val(c.w);
            $("#<%=hdnHeight.ClientID %>").val(c.h);

            renderPreviewControl();
            return false;
        };


        function renderPreviewControl() {
            var rdoFieldType = $("[id$=RadioSingle]");
            var divContainer = $("[id$=divFormPreview]");
            var previewControl = $("[id$=previewControl]");
            var RdSingleMain = $("[id$=RadioSingleValueMain]");
            var RdMultiMain = $("[id$=RadioMultiValueMain]");
            var RdSingleSub = $("[id$=RadioSingleValueSub]");
            var RdMultiSub = $("[id$=RadioMultiValueSub]");
            var Rdlist = $("[id$=RadioFeildDataType]");
            var Radiosingle = $("[id$=RadioSingle]");
            var RdAutoAge = $("[id$=RadioAutoAge]");
            var RdAutoFeilds = $("[id$=RadioAutoCalculateFeild]");
            var RdDocumentUpload = $("[id$=RadioImageUpload]");
            var RdDualDataEntryNone = $("[id$=RadioDualDataEntryNone]");
            var RdDualDataEntryMask1 = $("[id$=RadioDualDataEntryMask1]");
            var RdDualDataEntryMask2 = $("[id$=RadioDualDataEntryMask2]");
            var DualDataEntryMaskBoth = $("[id$=RadioDualDataEntryMaskBoth]");
            var x1 = $("#<%=hdnX1.ClientID %>").val();
            var y1 = $("#<%=hdnY1.ClientID %>").val();
            var x2 = $("#<%=hdnX2.ClientID %>").val();
            var y2 = $("#<%=hdnY2.ClientID %>").val();
            var width = $("#<%=hdnWidth.ClientID %>").val();
            var height = $("#<%=hdnHeight.ClientID %>").val();
            var controlHtml = '';

            //            if (rdoFieldType.is(":checked")) {     

            //Textbox
            var radio = Rdlist.find("input");

            if (rdoFieldType.is(":checked") == true) {
                for (var i = 0; i < radio.length; i++) {
                    if (radio[i].checked) {
                        if (radio[i].value == "String") {
                            controlHtml = '<input type="text" ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px;width:' + width + 'px;height:' + height + 'px;position:absolute; padding:0px;"/>';
                        }
                        else if (radio[i].value == "Date") {
                            controlHtml = '<input type="text" ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px;width:' + width + 'px;height:' + height + 'px;position:absolute; padding:0px;"/>';
                        }
                        else if (radio[i].value == "Number") {
                            controlHtml = '<input type="text" ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px;width:' + width + 'px;height:' + height + 'px;position:absolute; padding:0px;"/>';
                        }
                        else if (radio[i].value == "Boolean") {
                            controlHtml = '<input type="checkbox" ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px; position:absolute; padding:0px;"/>';
                        }
                    }
                }
            }
            else if (RdDocumentUpload.is(":checked") == true) {
                controlHtml = '<input type="file" ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px; width:' + width + 'px;height:' + height + 'px; position:absolute; padding:0px;"/>';
            }
            else if (RdDualDataEntryNone.is(":checked") == true || RdDualDataEntryMask1.is(":checked") == true || RdDualDataEntryMask2.is(":checked") == true || DualDataEntryMaskBoth.is(":checked") == true || RdAutoAge.is(":checked") == true || RdAutoFeilds.is(":checked") == true) {
                controlHtml = '<input type="text" ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px; width:' + width + 'px;height:' + height + 'px; position:absolute; padding:0px;"/>';
            }
            else if (RdSingleMain.is(":checked") == true) {
                controlHtml = '<select ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px;width:' + width + 'px;height:' + height + 'px; position:absolute; padding:0px;"></select>';

            }
            else if (Radiosingle.is(":checked") == true) {
                controlHtml = '<select ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px;width:' + width + 'px;height:' + height + 'px; position:absolute; padding:0px;"></select>';
            }
            else if (RdMultiMain.is(":checked") == true) {
                controlHtml = '<select ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px;width:' + width + 'px;height:' + height + 'px; position:absolute; padding:0px;"></select>';

            }
            else if (RdSingleSub.is(":checked") == true) {
                controlHtml = '<select ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px;width:' + width + 'px;height:' + height + 'px; position:absolute; padding:0px;"></select>';

            }
            else if (RdMultiSub.is(":checked") == true) {
                controlHtml = '<select ID="previewControl" style="z-index:90000;left:' + x1 + 'px;top:' + y1 + 'px;width:' + width + 'px;height:' + height + 'px; position:absolute; padding:0px;"></select>';

            }
            previewControl.remove(); // remove previously added control
            divContainer.append(controlHtml);
        }

        function getPreview(coords) {
            if (parseInt(coords.w) > 0) {
                var prev_img = $("[id$=divFormPreview]");  //to get the div id inwhich the image is located
                //or however you get a handle to the IMG
                //Get height and width of div
                var prev_width = prev_img.width();
                var prev_height = prev_img.height();
                var rx = prev_width / coords.w;
                var ry = prev_height / coords.h;
                //to get real image height and width
                var img = $("[id$=FormPreview]");
                //or however you get a handle to the IMG
                var width = img.width();
                var height = img.height();
                coords.x = $("#<%=hdnX1.ClientID %>").val();
                coords.y = $("#<%=hdnY1.ClientID %>").val()
                jQuery('#<%=hdnImageWidth.ClientID %>').val(width);
                jQuery('#<%=hdnImageHeight.ClientID %>').val(height);
                jQuery('#<%=hdnResolution.ClientID %>').val(width + '*' + height);
            }
        }

        function showPreview(coords) {
            if (parseInt(coords.w) > 0) {
                var prev_img = $("[id$=divpreview]");  //to get the div id inwhich the image is located
                //or however you get a handle to the IMG
                //Get height and width of div
                var prev_width = prev_img.width();
                var prev_height = prev_img.height();
                var rx = (prev_width / coords.w);
                var ry = (prev_height / coords.h);
                //to get real image height and width
                var img = jQuery('#<%= cropbox.ClientID %>');
                //or however you get a handle to the IMG
                var width = img.width();
                var height = img.height();
                jQuery('#<%=hdnImageWidth.ClientID %>').val(width);
                jQuery('#<%=hdnImageHeight.ClientID %>').val(height);
                jQuery('#<%=hdnResolution.ClientID %>').val(width + '*' + height);

                jQuery('#<%=preview.ClientID %>').width(coords.w);
                jQuery('#<%=preview.ClientID %>').height(coords.h);

                jQuery('#<%=preview.ClientID %>').css({
                    width: Math.round(rx * width) + 'px',
                    height: Math.round(ry * height) + 'px',
                    marginLeft: '-' + Math.round(rx * coords.x) + 'px',
                    marginTop: '-' + Math.round(ry * coords.y) + 'px'
                });
            }
        }

      
    </script>
    <script type="text/javascript" language="javascript">

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
        //Bind default value to label text same as fieldname
        function BindLabelText() {
            document.getElementById("<%=txtLabelText.ClientID%>").value = document.getElementById("<%=txtFieldName.ClientID %>").value;
        }


        //Validation on capture coordinate button click
        //DMS04-3895
        function Validate(e) {

            var Isvalidate = true;
            var fieldname = document.getElementById("<%= txtFieldName.ClientID %>");
            var Errormsg = document.getElementById("<%= lblMessage.ClientID %>");
            var Radiosingle = document.getElementById("<%= RadioSingle.ClientID%>");
            var RadioSingleValueMain = document.getElementById("<%= RadioSingleValueMain.ClientID%>");
            var RadioMultiValueMain = document.getElementById("<%= RadioMultiValueMain.ClientID%>");
            var RadioSingleValueSub = document.getElementById("<%= RadioSingleValueSub.ClientID%>");
            var RadioMultiValueSub = document.getElementById("<%= RadioMultiValueSub.ClientID%>");
            var minval = document.getElementById("<%= txtMin.ClientID %>");
            var maxval = document.getElementById("<%= txtMax.ClientID %>");
            var ddmainvalue = document.getElementById("<%= DDLMultiValueMain.ClientID %>");

            var DDLMultiValueSub1 = document.getElementById("<%= DDLMultiValueSub1.ClientID %>");
            var DDLMultiValueSub2 = document.getElementById("<%= DDLMultiValueSub2.ClientID %>");
            var txtLabelText = document.getElementById("<%= txtLabelText.ClientID %>");
            //Added new Feildtypes for validations
            var RdDocumentUpload = document.getElementById("<%= RadioImageUpload.ClientID %>");
            var RdDualDataEntry = document.getElementById("<%= RadioDualDataEntry.ClientID %>");
            var RdAutoAge = document.getElementById("<%= RadioAutoAge.ClientID %>");
            var RdAutoFeilds = document.getElementById("<%= RadioAutoCalculateFeild.ClientID %>");

            var RdDualDataEntryNone = document.getElementById("<%= RadioDualDataEntryNone.ClientID %>");
            var RdDualDataEntryMask1 = document.getElementById("<%= RadioDualDataEntryMask1.ClientID %>");
            var RdDualDataEntryMask2 = document.getElementById("<%= RadioDualDataEntryMask2.ClientID %>");
            var DualDataEntryMaskBoth = document.getElementById("<%= RadioDualDataEntryMaskBoth.ClientID %>");
            var RdCurrent = document.getElementById("<%= RadioCurrentDate.ClientID %>");
            var RdRefer = document.getElementById("<%= RadioReferDate.ClientID %>");
            var drpReferDate = document.getElementById("<%= DDLReferDate.ClientID %>");

            if (fieldname.value == "") {
                Errormsg.innerHTML = "Field name cannot be empty.";
                fieldname.focus();
                Isvalidate = false;
                return false;
            }
            if (RdDualDataEntry.checked == true && (RdDualDataEntryNone.checked == false &&
                RdDualDataEntryMask1.checked == false && RdDualDataEntryMask2.checked == false && DualDataEntryMaskBoth.checked == false)) {
                Errormsg.innerHTML = "Please choose sub type dual data entry.";
                Isvalidate = false;
                return false;
            }
            else if (RdAutoAge.checked == true && (RdCurrent.checked == false && RdRefer.checked == false)) {
                Errormsg.innerHTML = "Please choose sub type date calculation.";
                Isvalidate = false;
                return false;
            }
            if (Radiosingle.checked == false && RadioSingleValueMain.checked == false && RadioMultiValueMain.checked == false && RadioSingleValueSub.checked == false &&
            RadioMultiValueSub.checked == false && RdDocumentUpload.checked == false &&
            RdAutoFeilds.checked == false && RdAutoAge.checked == false && RdDualDataEntry.checked == false) {
                Errormsg.innerHTML = "Please choose field type.";
                Isvalidate = false;
                return false;
            }
            else {
                Isvalidate = true;
            }
            if ((RadioSingleValueMain.checked == true || RadioMultiValueMain.checked == true) && ddmainvalue.selectedIndex == 0) {
                Errormsg.innerHTML = "Please choose an appropriate multi value main item details.";
                Isvalidate = false;
                return false;
            }

            if ((RadioSingleValueSub.checked == true || RadioMultiValueSub.checked == true) && (DDLMultiValueSub1.selectedIndex == 0 || DDLMultiValueSub2.selectedIndex == 0)) {
                Errormsg.innerHTML = "Please choose an appropriate multi value subitem details.";
                Isvalidate = false;
                return false;
            }


            var RB1 = document.getElementById("<%=RadioFeildDataType.ClientID%>");
            var radio = RB1.getElementsByTagName("input");
            if (Radiosingle.checked == false) {
                for (var i = 0; i < radio.length; i++) {
                    if (radio[i].checked) {
                        radio[i].checked = false;
                        Isvalidate = false;
                        return false;
                    }
                }

            }

            if (Radiosingle.checked == true) {
                var isChecked = false;
                for (var i = 0; i < radio.length; i++) {
                    if (radio[i].checked) {
                        isChecked = true;
                        break;
                    }
                }
                if (!isChecked) {
                    Errormsg.innerHTML = "Please choose field data type.";
                    Isvalidate = false;
                    return false;
                }

            }

            if (parseInt(minval.value) > parseInt(maxval.value)) {
                Errormsg.innerHTML = "Min Length cannot be greater than Max Length.";
                Isvalidate = false;
                return false;
            }

            //ReferDate Value is please select
            if (RdRefer.checked == true && drpReferDate.value == "0" || drpReferDate.value == undefined) {
                Errormsg.innerHTML = "Please select custom date.";
                Isvalidate = false;
                return false;
            }
            if (txtLabelText.value == "") {
                Errormsg.innerHTML = "Label text cannot be empty.";
                txtLabelText.focus();
                Isvalidate = false;
                return false;
            }

            //DMS04-3895 BS
            if (e.id.indexOf("btnSave") > -1) {
                var X1 = document.getElementById("<%= txtX1.ClientID %>");
                var X2 = document.getElementById("<%= txtX2.ClientID %>");
                var Y1 = document.getElementById("<%= txtY1.ClientID %>");
                var Y2 = document.getElementById("<%= txtY2.ClientID %>");

                var pageNo = document.getElementById("<%= txtPageNumber.ClientID %>");
                var dataEntryType = document.getElementById("<%= hdnDataType.ClientID %>");

                if (dataEntryType != null && dataEntryType.value != null && dataEntryType.value != undefined && dataEntryType.value != "Normal") {
                    if (X2 != null && X2 != undefined) {
                        if (X2.value == "" || X2.value == 0) {
                            X1.value = X2.value = Y1.value = Y2.value = pageNo.value = 0;
                            Isvalidate = false;
                        }
                    }
                    if (Isvalidate == false) {
                        Isvalidate = window.confirm("Position for the field is not captured. Do you want to continue?");
                    }
                }
            }
            //DMS04-3895 BE

            return Isvalidate;
        }


        function clearlblmessage() {
            var Errormsg = document.getElementById("<%= lblMessage.ClientID %>");
            Errormsg.innerHTML = "";
            return true;
        }
        function EnableDisableType() {
            var RdSingle = document.getElementById("<%= RadioSingle.ClientID %>");
            var RdSingleMain = document.getElementById("<%= RadioSingleValueMain.ClientID %>");
            var RdMultiMain = document.getElementById("<%= RadioMultiValueMain.ClientID %>");
            var RdSingleSub = document.getElementById("<%= RadioSingleValueSub.ClientID %>");
            var RdMultiSub = document.getElementById("<%= RadioMultiValueSub.ClientID %>");
            var Rdlist = document.getElementById("<%= RadioFeildDataType.ClientID %>");
            var min = document.getElementById("<%= txtMin.ClientID %>");
            var max = document.getElementById("<%= txtMax.ClientID %>");

            //Added new Feildtypes for validations
            var RdDocumentUpload = document.getElementById("<%= RadioImageUpload.ClientID %>");
            var RdDualDataEntry = document.getElementById("<%= RadioDualDataEntry.ClientID %>");
            var RdAutoAge = document.getElementById("<%= RadioAutoAge.ClientID %>");
            var RdCurrent = document.getElementById("<%= RadioCurrentDate.ClientID %>");
            var RdRefer = document.getElementById("<%= RadioReferDate.ClientID %>");
            var RdAutoFeilds = document.getElementById("<%= RadioAutoCalculateFeild.ClientID %>");
            var Errormsg = document.getElementById("<%= lblMessage.ClientID %>");
            var RdDualDataEntryNone = document.getElementById("<%= RadioDualDataEntryNone.ClientID %>");
            var RdDualDataEntryMask1 = document.getElementById("<%= RadioDualDataEntryMask1.ClientID %>");
            var RdDualDataEntryMask2 = document.getElementById("<%= RadioDualDataEntryMask2.ClientID %>");
            var DualDataEntryMaskBoth = document.getElementById("<%= RadioDualDataEntryMaskBoth.ClientID %>");
            var Hiddenfield = document.getElementById("<%= HiddenFieldPopUp.ClientID %>");
            var drpReferDate = document.getElementById("<%= DDLReferDate.ClientID %>");
            //disableing sub radio buttons
            if (Hiddenfield.value != "EditMode") {
                SubDualataenrtyDisabledtrue();
                RdCurrent.disabled = true;
                RdRefer.disabled = true;
            }
            else {
                SubDualataenrtyDisabledfalse();
                RdCurrent.disabled = false;
                RdRefer.disabled = false;
            }
            //make default selection as string
            if (RdSingleMain.checked == true || RdSingleSub.checked == true || RdDocumentUpload.checked == true) {
                Rdlist.disabled = true;
                min.disabled = true;
                max.disabled = true;
                //DMS5-4293
                //Errormsg.innerHTML = "";
                var defaulttypeselection = Rdlist.getElementsByTagName('input');
                for (var i = 0; i < defaulttypeselection.length; i++) {
                    defaulttypeselection[0].checked = true;
                    defaulttypeselection[0].disabled = false;
                }
                DisablesubSpecialfields();
                return true;
            }
            function DisablesubSpecialfields() {
                RdDualDataEntryNone.disabled = true;
                RdDualDataEntryMask1.disabled = true;
                RdDualDataEntryMask2.disabled = true;
                DualDataEntryMaskBoth.disabled = true;
                RdDualDataEntryNone.checked = false;
                RdDualDataEntryMask1.checked = false;
                RdDualDataEntryMask2.checked = false;
                DualDataEntryMaskBoth.checked = false;
                RdCurrent.checked = false;
                RdRefer.checked = false;
                drpReferDate.disabled = true;
                drpReferDate.selectedIndex = 0;
            }
            //default selection for dual dataentry
            if (RdDualDataEntry.checked == true) {
                Rdlist.disabled = false;
                //DMS5-4293
                //Errormsg.innerHTML = "";
                var defaulttypeselection = Rdlist.getElementsByTagName('input');
                for (var i = 0; i < defaulttypeselection.length; i++) {
                    defaulttypeselection[0].disabled = false;
                    defaulttypeselection[1].disabled = true;
                    defaulttypeselection[2].disabled = false;
                    defaulttypeselection[3].disabled = true;
                }
                if (RdDualDataEntry.checked == true) {
                    SubDualataenrtyDisabledfalse();
                    RdCurrent.checked = false;
                    RdRefer.checked = false;
                    //DMS5-4293
                   // Errormsg.innerHTML = "";
                    drpReferDate.disabled = true;
                    drpReferDate.selectedIndex = 0;
                }
                else {
                    if (Hiddenfield.value != "EditMode") {
                        SubDualDataEnrtyCheckedFalse();
                    }
                    SubDualataenrtyDisabledtrue();
                }
                return true;
            }
            //to disable sub dual dat enrty items
            function SubDualataenrtyDisabledfalse() {
                RdDualDataEntryNone.disabled = false;
                RdDualDataEntryMask1.disabled = false;
                RdDualDataEntryMask2.disabled = false;
                DualDataEntryMaskBoth.disabled = false;
            }
            //Make date as default selection
            if (RdAutoAge.checked == true) {
                Rdlist.disabled = true;
                min.disabled = true;
                max.disabled = true;
                //Errormsg.innerHTML = "";
                var defaulttypeselection = Rdlist.getElementsByTagName('input');
                for (var i = 0; i < defaulttypeselection.length; i++) {
                    defaulttypeselection[1].checked = true;
                    defaulttypeselection[1].disabled = false;
                }
                if (RdAutoAge.checked == true) {
                    RdCurrent.disabled = false;
                    RdRefer.disabled = false;
                    SubDualataenrtyDisabledtrue();
                    SubDualDataEnrtyCheckedFalse();
                    //DMS5-4293
                   // Errormsg.innerHTML = "";
                }
                else {
                    if (Hiddenfield.value != "EditMode") {
                        RdCurrent.checked = false;
                        RdRefer.checked = false;
                    }
                    RdCurrent.disabled = true;
                    RdRefer.disabled = true;
                }
                return true;
            }
            //function to uncheck sub dual date entry items
            function SubDualDataEnrtyCheckedFalse() {
                RdDualDataEntryNone.checked = false;
                RdDualDataEntryMask1.checked = false;
                RdDualDataEntryMask2.checked = false;
                DualDataEntryMaskBoth.checked = false;
            }
            //maake number as default selection
            if (RdAutoFeilds.checked == true) {
                Rdlist.disabled = true;
                min.disabled = true;
                max.disabled = true;
                Errormsg.innerHTML = "";
                var defaulttypeselection = Rdlist.getElementsByTagName('input');
                for (var i = 0; i < defaulttypeselection.length; i++) {
                    defaulttypeselection[2].checked = true;
                    defaulttypeselection[2].disabled = false;
                }
                SubDualataenrtyDisabledtrue();
                RdCurrent.checked = false;
                RdRefer.checked = false;              
                return true;
            }
            function SubDualataenrtyDisabledtrue() {
                RdDualDataEntryNone.disabled = true;
                RdDualDataEntryMask1.disabled = true;
                RdDualDataEntryMask2.disabled = true;
                DualDataEntryMaskBoth.disabled = true;
            }
            if (RdCurrent.checked == true) {
                if (Hiddenfield.value == "EditMode") {
                    RdRefer.disabled = true;
                    drpReferDate.disabled = true;
                }
                else {
                    RdRefer.disabled = false;
                    drpReferDate.disabled = true;
                    SubDualataenrtyDisabledfalse();
                   // Errormsg.innerHTML = "";
                }
            }
            else if (RdRefer.checked == true) {
                if (Hiddenfield.value == "EditMode") {
                    RdCurrent.disabled = true;
                }
                else {
                    RdCurrent.disabled = false;
                    SubDualataenrtyDisabledfalse();
                }
                //                if (drpReferDate.selectedIndex == -1 || drpReferDate.selectedIndex == 0) {
                //                    Errormsg.innerHTML = "No fields with date are available for this stage.";
                //                    return false;
                //                }
            }
            if (RdRefer.checked == true) {
                drpReferDate.disabled = false;
            }
            else {
                drpReferDate.disabled = true;
                drpReferDate.selectedIndex = 0;
            }
            if (RdDocumentUpload.checked == true || RdAutoAge.checked == true || RdCurrent.checked == true || RdRefer.checked == true ||
             RdAutoFeilds.checked == true || RdDualDataEntryNone.checked == true || RdDualDataEntryMask1.checked == true || RdDualDataEntryMask2.checked == true || DualDataEntryMaskBoth.checked == true) {
                Rdlist.disabled = true;
                min.disabled = true;
                max.disabled = true;
              //  Errormsg.innerHTML = "";
            }
            if (RdSingle.checked == true) {
                var inputRadioArray = Rdlist.getElementsByTagName('input');
                DisablesubSpecialfields();
                Rdlist.disabled = false;
                min.disabled = false;
                max.disabled = false;
               // Errormsg.innerHTML = "";
                for (var i = 0; i < inputRadioArray.length; i++) {
                    //to disable min and max texbox except number field is check
                    if (inputRadioArray[0].checked == true || inputRadioArray[2].checked == true) {
                        min.disabled = false;
                        max.disabled = false;
                    }
                    else {
                        min.disabled = true;
                        max.disabled = true;
                    }
                }
                return true;
            }
            else if (RdSingleMain.checked == true) {
                Rdlist.disabled = true;
                min.disabled = true;
                max.disabled = true;
                //Errormsg.innerHTML = "";
                var inputElementArray = Rdlist.getElementsByTagName('input');

                for (var i = 0; i < inputElementArray.length; i++) {
                    var inputElement = inputElementArray[i];

                    inputElement.checked = false;
                }
                return true;
            }
            else if (RdMultiMain.checked == true) {
                DisablesubSpecialfields();
                Rdlist.disabled = true;
                min.disabled = true;
                max.disabled = true;
                var inputElementArraymain = Rdlist.getElementsByTagName('input');

                for (var i = 0; i < inputElementArraymain.length; i++) {
                    var inputElement = inputElementArraymain[i];

                    inputElement.checked = false;
                }
                return true;
            }
            else if (RdSingleSub.checked == true) {
                DisablesubSpecialfields();
                Rdlist.disabled = true;
                min.disabled = true;
                max.disabled = true;
               // Errormsg.innerHTML = "";
                var inputElementArraysub = Rdlist.getElementsByTagName('input');

                for (var i = 0; i < inputElementArraysub.length; i++) {
                    var inputElement = inputElementArraysub[i];

                    inputElement.checked = false;
                }
                return true;
            }
            else if (RdMultiSub.checked == true) {
                DisablesubSpecialfields();
                Rdlist.disabled = true;
                min.disabled = true;
                max.disabled = true;
                //Errormsg.innerHTML = "";
                var inputElementArraymultisub = Rdlist.getElementsByTagName('input');

                for (var i = 0; i < inputElementArraymultisub.length; i++) {
                    var inputElement = inputElementArraymultisub[i];

                    inputElement.checked = false;
                }
                return true;
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

        function inputOnlyOperators(event) {
            var specialKeys = new Array();
            specialKeys.push(41); //operator ")"
            specialKeys.push(40); // operator "("
            specialKeys.push(42); // operator "*"
            specialKeys.push(46); // operator "/"
            specialKeys.push(45); // operator "-"
            specialKeys.push(43); // operator "+"
            var keyCode = event.which ? event.which : event.keyCode
            var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
            return ret;
        }

        //to move listbox items to textarea on double clcik
        // function: UnAssignment
        function assignList() {
            var listBoxStageFeildNames = $("[id$=lstStageFeildNames]");
            var selectedItem = listBoxStageFeildNames.find(":selected").text();
            var txtArea = $("[id$=txtFormulaArea]");
            var Errormsg = document.getElementById("<%= lblErrorFormula.ClientID %>");
           // Errormsg.innerHTML = "";
            //txtArea.append(selectedItem);
            txtArea.insertAtCaret(selectedItem);
        }

        // function: UnAssignment
        function unassignList() {
            var listBoxOperators = $("[id$=lstOperators]");
            var selectedOperator = listBoxOperators.find(":selected").text();
            var txtAreaOp = $("[id$=txtFormulaArea]");
            var Errormsg = document.getElementById("<%= lblErrorFormula.ClientID %>");
            //Errormsg.innerHTML = "";
            // txtAreaOp.append(selectedOperator);
            txtAreaOp.insertAtCaret(selectedOperator);
        }

        //List box cursor position handler
        jQuery.fn.extend({
            insertAtCaret: function (myValue) {
                return this.each(function (i) {
                    if (document.selection) {
                        //For browsers like Internet Explorer
                        this.focus();
                        var sel = document.selection.createRange();
                        sel.text = myValue;
                        this.focus();
                    }
                    else if (this.selectionStart || this.selectionStart == '0') {
                        //For browsers like Firefox and Webkit based
                        var startPos = this.selectionStart;
                        var endPos = this.selectionEnd;
                        var scrollTop = this.scrollTop;
                        this.value = this.value.substring(0, startPos) + myValue + this.value.substring(endPos, this.value.length);
                        this.focus();
                        this.selectionStart = startPos + myValue.length;
                        this.selectionEnd = startPos + myValue.length;
                        this.scrollTop = scrollTop;
                    } else {
                        this.value += myValue;
                        this.focus();
                    }
                });
            }
        });

        //Function to click a button on page dropdown value change
        function ddlDropChange() {
            document.getElementById("<%= btnDDLDrop.ClientID %>").click();
        }

        function navigationHandler(action) {
            var PageNo = document.getElementById("<%= DDLDrop.ClientID %>").value;
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

  
   
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <WF:WorkFlowWizard ID="WorkFlowWizard1" runat="server" ActiveItemName="Fields" />
    <%--  <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Always">
        <ContentTemplate>--%>
    <div class="GVDiv" id="MainDiv" runat="server">
        <asp:HiddenField ID="HiddenStageFieldId" runat="server" />
        <div class="InfoDisplay">
            <h3>
                <asp:Label ID="lblCurrentStageNameHeader" runat="server" Text="Stage Name: " />
                <asp:Label ID="lblCurrentStageNameHeaderValue" runat="server" Text="-" />
            </h3>
        </div>
        <table>
            <tr>
                <td colspan="2">
                    <h3>
                        <asp:Label ID="lblEditFields" runat="server" Text="Add/Edit Stage Field Details"
                            meta:resourcekey="lblEditFields" /></h3>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblFieldName" runat="server" Text="Field Name"></asp:Label>
                    <span style="color: Red; font-size: medium">*</span>
                </td>
                <td>
                    <asp:TextBox ID="txtFieldName" runat="server" MaxLength="50" onkeypress="return clearlblmessage();"
                        onkeyup="BindLabelText()"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <asp:Label ID="lblFieldTypeSingle" runat="server" Text="Field Type"></asp:Label>
                    <span style="color: Red; font-size: medium">*</span>
                </td>
                <td>
                    <table style="border: 1px solid grey">
                        <tr>
                            <td class="AlignFeildTypes">
                                <asp:RadioButton ID="RadioSingle" Text="Single" runat="server" GroupName="Rad" onclick="return EnableDisableType();"
                                    onchange="return clearlblmessage();" TagName="Read" />
                            </td>
                            <td class="AlignFeildTypes">
                                <asp:RadioButton ID="RadioSingleValueMain" Text="Single Value Main" runat="server"
                                    TagName="Read" onclick="return EnableDisableType();" onchange="return clearlblmessage();"
                                    GroupName="Rad" /><br />
                                <asp:RadioButton ID="RadioMultiValueMain" Text="Multi Value Main" runat="server"
                                    TagName="Read" onclick="return EnableDisableType();" onchange="return clearlblmessage();"
                                    GroupName="Rad" Style="margin-top: 7px;" /><br />
                                <asp:DropDownList ID="DDLMultiValueMain" runat="server" Style="margin-top: 7px;">
                                </asp:DropDownList>
                            </td>
                            <td class="AlignFeildTypes">
                                <asp:RadioButton ID="RadioSingleValueSub" Text="Single Value Sub" onclick="return EnableDisableType();"
                                    TagName="Read" onchange="return clearlblmessage();" runat="server" GroupName="Rad" /><br />
                                <asp:RadioButton ID="RadioMultiValueSub" Text="Multi Value Sub" onclick="return EnableDisableType();"
                                    TagName="Read" onchange="return clearlblmessage();" runat="server" GroupName="Rad"
                                    Style="margin-top: 7px;" /><br />
                                <asp:DropDownList ID="DDLMultiValueSub1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DDLMultiValueSub1_SelectedIndexChanged"
                                    Style="margin-top: 7px;">
                                </asp:DropDownList>
                                <br />
                                <asp:DropDownList ID="DDLMultiValueSub2" runat="server" Style="margin-top: 7px;">
                                </asp:DropDownList>
                            </td>
                            <td class="AlignFeildTypes">
                                <asp:RadioButton ID="RadioImageUpload" Text="File Upload" onclick="return EnableDisableType();"
                                    TagName="Read" runat="server" GroupName="Rad" />
                            </td>
                            <td class="AlignFeildTypes">
                                <asp:RadioButton ID="RadioDualDataEntry" Text="Dual Data Entry" onclick="return EnableDisableType();"
                                    TagName="Read" runat="server" GroupName="Rad" />
                                <br />
                                <br />
                                <asp:RadioButton ID="RadioDualDataEntryNone" Text="No Masking" onclick="return EnableDisableType();"
                                    TagName="Read" runat="server" GroupName="Dual" /><br />
                                <asp:RadioButton ID="RadioDualDataEntryMask1" Text="Mask First Entry" onclick="return EnableDisableType();"
                                    TagName="Read" runat="server" GroupName="Dual" /><br />
                                <asp:RadioButton ID="RadioDualDataEntryMask2" Text="Mask Second Entry" onclick="return EnableDisableType();"
                                    TagName="Read" runat="server" GroupName="Dual" /><br />
                                <asp:RadioButton ID="RadioDualDataEntryMaskBoth" Text="Mask Both" onclick="return EnableDisableType();"
                                    TagName="Read" runat="server" GroupName="Dual" /><br />
                            </td>
                            <td class="AlignFeildTypes">
                                <asp:RadioButton ID="RadioAutoAge" Text="Date Calculation" onclick="return EnableDisableType();"
                                    TagName="Read" runat="server" GroupName="Rad" /><br />
                                <br />
                                <asp:RadioButton ID="RadioCurrentDate" Text="With Current Date" onclick="return EnableDisableType();"
                                    TagName="Read" runat="server" GroupName="RadSub" /><br />
                                <asp:RadioButton ID="RadioReferDate" Text="Custom" onchange="return EnableDisableType();"
                                    TagName="Read" AutoPostBack="true" runat="server" GroupName="RadSub" OnCheckedChanged="RadioReferDate_CheckedChanged" /><br />
                                <asp:DropDownList ID="DDLReferDate" runat="server">
                                </asp:DropDownList>
                            </td>
                            <td class="AlignFeildTypes">
                                <asp:RadioButton ID="RadioAutoCalculateFeild" Text="Formula" onchange="return EnableDisableType();" 
                                    TagName="Read" AutoPostBack="true" runat="server" GroupName="Rad" OnCheckedChanged="RadioAutoCalculateFeild_CheckedChanged" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblFieldDataType" runat="server" Text="Field Data Type"></asp:Label>
                </td>
                <td>
                    <asp:RadioButtonList ID="RadioFeildDataType" runat="server" onclick="return EnableDisableType();"
                        TagName="Read" RepeatDirection="Horizontal">
                        <asp:ListItem Text="String"></asp:ListItem>
                        <asp:ListItem Text="Date"></asp:ListItem>
                        <asp:ListItem Text="Number"></asp:ListItem>
                        <asp:ListItem Text="Boolean"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblMin" runat="server" Text="Min Length"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtMin" runat="server" MaxLength="3" onkeypress="return onlyNumbers(event,this);"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblX1" runat="server" Text="X1" CssClass="MnuAdminClear"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtX1" runat="server" CssClass="MnuAdminClear"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblX2" runat="server" Text="X2" CssClass="MnuAdminClear"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtX2" runat="server" CssClass="MnuAdminClear"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblMax" runat="server" Text="Max Length"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtMax" runat="server" MaxLength="3" onkeypress="return onlyNumbers(event,this);"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblY1" runat="server" Text="Y1" CssClass="MnuAdminClear"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtY1" runat="server" CssClass="MnuAdminClear"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblY2" runat="server" Text="Y2" CssClass="MnuAdminClear"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtY2" runat="server" CssClass="MnuAdminClear"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblActive" runat="server" Text="Active"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="CheckActive" runat="server" />
                </td>
                <td>
                    <asp:Label ID="lblPageNumber" runat="server" Text="PageNumber" CssClass="MnuAdminClear"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtPageNumber" runat="server" CssClass="MnuAdminClear"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblMandatory" runat="server" Text="Mandatory"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="CheckMandatory" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblShowInDashboard" runat="server" Text="Show field in dashboard"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkShowInDashboard" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblDisplay" runat="server" Text="Show field in following stages"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="CheckDisplay" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblEditable" runat="server" Text="Edit field value in following stages"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkEditable" runat="server" />
                </td>
            </tr>
         
              <tr>
                <td>
                    <asp:Label ID="lblRemarks" runat="server" Text="Enable as information field"></asp:Label>
                </td>
                <td>
                   <asp:CheckBox ID="chkRemarks" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblMandatoryErrorMessage" runat="server" Text="Mandatory Error Message"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtMandatoryErrorMessage" runat="server" MaxLength="70"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblLabelText" runat="server" Text="Label Text"></asp:Label>
                    <span style="color: Red; font-size: medium">*</span>
                </td>
                <td>
                    <asp:TextBox ID="txtLabelText" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <asp:HiddenField ID="hdnDataType" runat="server" />
                    <asp:Button ID="btnCaptureCoordinates" runat="server" Text="Capture Position" CssClass="btnCaptureCoordinates"
                        TagName="Add" OnClick="btnCaptureCoordinates_Click" OnClientClick="return Validate(this);" />
                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="return Validate(this);"
                        TagName="Add" CssClass="btnsave" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                        TagName="Read" CssClass="btncancel" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="hdnParentRowId" runat="server" />
        <asp:HiddenField ID="HiddenFieldPopUp" runat="server" />
        <asp:ModalPopupExtender ID="ModalPopupExtenderFormula" runat="server" TargetControlID="HiddenFieldPopUp"
            PopupControlID="pnlpopupformula" CancelControlID="imgClosePopUp" BackgroundCssClass="modalBackground">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlpopupformula" runat="server" CssClass="pnlBackGround" Style="display: none">
            <div class="GVDiv" style="height: 400px;">
                <asp:ImageButton ID="imgClosePopUp" runat="server" Style="float: right" OnClick="imgClosePopUp_Click"
                    ImageUrl="~/Images/close.png" />
                <table id="Table1" runat="server">
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td colspan="2">
                                        <asp:Label ID="lblErrorFormula" runat="server" ForeColor="Red"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblstagefieldNames" runat="server" Text="Stages Field Names"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblOpearators" runat="server" Text="Operators"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:ListBox ID="lstStageFeildNames" runat="server" ondblclick="assignList();" Height="120px">
                                        </asp:ListBox>
                                    </td>
                                    <td>
                                        <asp:ListBox ID="lstOperators" runat="server" ondblclick="unassignList();" Height="120px">
                                        </asp:ListBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        Formula Area
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtFormulaArea" TextMode="MultiLine" Width="400px" Height="150px"
                                            runat="server" onkeypress="return inputOnlyOperators(event);"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnTestFormula" runat="server" Text="Test Formula" OnClick="TestFormula" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="border: 1px solid black;">
                        </td>
                        <td style="vertical-align: top;">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <table>
                                        <tr>
                                            <td colspan="2">
                                                <asp:PlaceHolder ID="PlaceHolderFormula" runat="server"></asp:PlaceHolder>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Result
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtFormulResult" runat="server" ReadOnly="true"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Button ID="btnExecute" runat="server" Text="Execute Formula" OnClick="btnExecute_Click"
                                                    TagName="Read" Enabled="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </div>
    <div id="DivImage" runat="server" class="cropImagebox">
        <asp:Panel ID="TagPanel" runat="server">
            <table>
                <tr>
                    <td>
                        <div style="border: 1px solid #87a93e; background-color: #DADADA">
                            <table>
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
                                        <asp:Label ID="lblField" runat="server" Text="Field Name :"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblFieldDescription" Font-Bold="true" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                    <td>
                       
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:HiddenField ID="hdnX1" runat="server" />
                        <asp:HiddenField ID="hdnX2" runat="server" />
                        <asp:HiddenField ID="hdnY1" runat="server" />
                        <asp:HiddenField ID="hdnY2" runat="server" />
                        <asp:HiddenField ID="hdnTemplateFolderPath" runat="server" Value="" />
                        <asp:HiddenField ID="hdnFielExtension" runat="server" />
                        <asp:HiddenField ID="hdnPageNo" runat="server" Value="1" />
                        <asp:HiddenField ID="hdnPagesCount" runat="server" Value="1" />
                        <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                        <asp:HiddenField ID="hdnShowBackgroundImage" runat="server" Value="true" />
                        <asp:HiddenField ID="hdnWidth" runat="server" />
                        <asp:HiddenField ID="hdnHeight" runat="server" />
                        <asp:HiddenField ID="hdnResolution" runat="server" />
                        <asp:TextBox ID="txtResolution" runat="server" CssClass="HiddenButton"></asp:TextBox>
                        <asp:Button ID="btnCallFromJavascript" class="HiddenButton" runat="server" TagName="Read"
                            OnClick="btnCallFromJavascript_Click" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblOriginal" runat="server" Text="Original Image"></asp:Label>
                        <div id="divcropbox" style="overflow: auto; width: 638px; height: 400px; border: 1px solid #87a93e"
                            runat="server">
                            <img src="" id="cropbox" runat="server" alt="" />
                        </div>
                        <div id="divFormCropbox" style="overflow: auto; width: 638px; height: 400px; border: 1px solid #87a93e"
                            runat="server">
                            <img src="" id="FormCropbox" runat="server" alt="" />
                        </div>
                    </td>
                    <td valign="top">
                        <asp:Label ID="lblPreview" runat="server" Text="Preview Image"></asp:Label>
                        <div id="divpreview" style="max-width: 580px; max-height: 200px; overflow: hidden;
                            width: 580px; height: 200px; border: 1px solid #87a93e" runat="server">
                            <img src="" id="preview" runat="server" alt="" />
                        </div>
                        <div id="divFormPreview" style="overflow: scroll; max-width: 580px; max-height: 400px;
                            position: relative; width: 580px; height: 400px; border: 1px solid #87a93e" runat="server">
                            <img src="" id="FormPreview" runat="server" alt="" />
                        </div>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <input id="btnFirst" onclick="navigationHandler('FIRST');" type="button" class="btnfirst"
                            title="First" />
                        <input id="btnPrevious" onclick="navigationHandler('PREVIOUS');" type="button" class="btnleftarrow"
                            title="Previous" />
                        <asp:DropDownList ID="DDLDrop" runat="server" TagName="Read" CssClass="ddlPage">
                        </asp:DropDownList>
                        <input id="btnNext" onclick="navigationHandler('NEXT');" type="button" class="btnrightarrow"
                            tagname="Read" title="Next" />
                        <input id="btnLast" onclick="navigationHandler('LAST');" type="button" class="btnlast"
                            tagname="Read" title="Last" />
                    </td>
                </tr>
                <asp:HiddenField ID="hdnSrc" runat="server" />
            </table>
            <asp:Button ID="btnDDLDrop" runat="server" class="HiddenButton" OnClick="btnDDLDrop_Click" />
            <center>
                <asp:Button ID="btnOK" runat="server" TagName="Add" Text="OK" OnClick="btnClose_Click"
                    CssClass="btnOK" />
                <asp:Button ID="btnCancelCrop" runat="server" TagName="Read" Text="Cancel" CssClass="btncancel"
                    OnClick="btnCancelCrop_Click" />
            </center>
        </asp:Panel>
        <asp:HiddenField ID="hdnImageWidth" runat="server" />
        <asp:HiddenField ID="hdnImageHeight" runat="server" />
    </div>
    <%--    </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
