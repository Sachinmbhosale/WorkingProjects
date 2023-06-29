<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="DocumentDownloadDetails.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.DocumentDownloadDetails"
    EnableEventValidation="false" %>

<%@ Register Src="PDFViewer.ascx" TagName="PDFViewer" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        #statusPopup
        {
            padding-left: 395px;
            padding-top: 60px;
            top: 0%;
            left: 0%;
        }
    </style>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/PDFPageCount.js") %>"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageNo.ClientID %>";
        });

        var message = 'Success';
        function disablecommit(sender, args) {

            document.getElementById('<%=btnCommitChanges.ClientID%>').disabled = true;
            document.getElementById('<%=btnDicardChanges.ClientID%>').disabled = true;
            var result;
            var msgControl = "#<%= divMsg.ClientID %>";
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1);
            var err = new Error();
            if (document.getElementById("<%= drpPageCount.ClientID  %>").value < 0) {

                err.name = 'My API Input Error';
                err.message = 'Please select positon!';
                throw (err);
                return false;
            }

            if (filext == 'pdf' || filext == 'tif' || filext == 'tiff' || filext == 'TIF' || filext == 'TIFF' || filext == 'PDF') {
                document.getElementById("<%= hdnUploaded.ClientID %>").value = "FALSE"; // enable scond time upload SET hidden variable as false which is consider in code behind

                return true;
            }
            else {

                err.name = 'My API Input Error';
                err.message = 'File format not supported! (Supported format ' + filext + ')';
                throw (err);
                return false;
            }
        }
        /*DMS04-3417 BS*/
        function Confirmationfordelete() {
            var Isbool;
            var status = confirm("Are you sure you want to delete ?");
            if (status == true) {
                Isbool = true;
            }
            else {
                Isbool = false;
            }
            return Isbool;
        }

        /*DMS04-3417 BE*/
        function enablesave() {
            document.getElementById('<%=btnReloadiFrame.ClientID%>').click();
            document.getElementById('<%=btnCommitChanges.ClientID%>').disabled = false;
            document.getElementById('<%=btnDicardChanges.ClientID%>').disabled = false;

        }

        function Initialisation() {

            alert("Document has been Edited Successfully");
            document.getElementById('<%=btnDicardChanges.ClientID%>').disabled = false;

        }

        function enabledelete() {
            document.getElementById("<%= hdnButtonAction.ClientID %>").value = "Delete";
            document.getElementById('<%=btnReloadiFrame.ClientID%>').click();

        }

        function drpDeleteChange() {

            var x = document.getElementById('<%=drpDeleteCount.ClientID%>').value;
            if (x == 0) {
                document.getElementById('<%=btnDeletePages.ClientID%>').disabled = true;
                return false;
            }
            else {
                document.getElementById('<%=btnDeletePages.ClientID%>').disabled = false;
                return false;
            }

        }

        //*************************************************************************************************
        // Save Tag function
        //*************************************************************************************************

        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }



        function validateTagPages() {

            var Isval = true;
            var txttagPages = document.getElementById("<%= txttagpages.ClientID %>").value;
            //var SpecialCharachter = /^[^a-zA-Z._]+$/
            var SpecialCharachter = /[^0-9,\-]/;
            var lastchar = txttagPages.slice(-1);
            if (txttagPages.length != 0) {
                if (lastchar == "," || lastchar == "-") {

                    Isval = false;
                    message = 'Page selection contain extra Characters  remove it and try !';
                }
                else if (SpecialCharachter.test(txttagPages.trim())) {
                    Isval = false;
                    message = 'Page selection contain extra Characters  remove it and try !';
                }
            }
            return Isval;

        }

        //DMS04-3470D
        /*
        function savetagdetails(Taction) {

        var tagtextboxvalue = document.getElementById('<%=txttagpages.ClientID%>').value;
        if (tagtextboxvalue.length > 0) {
        if (validateTagPages()) {
        var Action = Taction;


        var msgControl = "#<%= divMsg.ClientID %>";
        var UploadID = getParameterByName("id");
        var MainTagID = $("#<%= cmbMainTag.ClientID %>").val();
        var SubTagID = $("#<%= cmbSubTag.ClientID %>").val();


        var TotalPages = document.getElementById("<%=hdnPagesCount.ClientID %>").value;
        // var TotalPages = $("#<%= DDLDrop.ClientID %> option").length;
        var PageID = getpagenumbers(tagtextboxvalue, TotalPages);
        if (MainTagID != "0" && MainTagID != "<Select>") {
        var params = UploadID + '|' + TotalPages + '|' + PageID + '|' + MainTagID + '|' + SubTagID;
        document.getElementById('totalpagescount').innerHTML = TotalPages;
        if (PageID != '') {
        return CallPostScalar(msgControl, Action, params);


        }
        else {
        alert(message);
        }
        }
        else {

        alert("Please select a Tag before saving");
        }
        }
        else {
        alert(message);
        }
        }
        else {
        alert("Kindly Enter a Pagenumber");

        }
        return false;
        }
        */

        //DMS04-3470BS -- Removed tag selection warning message at the time of tag delete
        function savetagdetails(Taction) {

            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");

            var tagtextboxvalue = document.getElementById('<%=txttagpages.ClientID%>').value;
            if (tagtextboxvalue.length > 0) {
                if (validateTagPages()) {
                    var Action = Taction;

                    var UploadID = getParameterByName("id");
                    var MainTagID = $("#<%= cmbMainTag.ClientID %>").val();
                    var SubTagID = $("#<%= cmbSubTag.ClientID %>").val();

                    var TotalPages = document.getElementById("<%=hdnPagesCount.ClientID %>").value;
                    // var TotalPages = $("#<%= DDLDrop.ClientID %> option").length;
                    var PageID = getpagenumbers(tagtextboxvalue, TotalPages);
                    var params = "";

                    if (Taction == "DeleteTagDetails") {
                        // Delete tag
                        params = UploadID + '|' + TotalPages + '|' + PageID + '|' + MainTagID + '|' + SubTagID;
                        document.getElementById('totalpagescount').innerHTML = TotalPages;

                        if (PageID != '') {
                            return CallPostScalar(msgControl, Action, params);
                        }
                        else {
                            $(msgControl).html(message);
                        }
                    }
                    else {
                        // Save or update
                        if (MainTagID != "0" && MainTagID != "<Select>") {
                            params = UploadID + '|' + TotalPages + '|' + PageID + '|' + MainTagID + '|' + SubTagID;
                            document.getElementById('totalpagescount').innerHTML = TotalPages;

                            if (PageID != '') {
                                return CallPostScalar(msgControl, Action, params);
                            }
                            else {
                                $(msgControl).html(message);
                            }
                        }
                        else {
                            $(msgControl).html("");
                            $(msgControl).html("Please select a tag before saving!");
                        }
                    }
                }
                else {
                    $(msgControl).html(message);
                }
            }
            else {
                $(msgControl).html("Please enter page number(s)!");
            }
            return false;
        }
        //DMS04-3470BE

        //*************************************************************************************************
        // Adobe Controller JS function
        //*************************************************************************************************

        function printPdf() {
            var PrintLoaderDiv = document.getElementById('PrintInnerPanel');
            PrintLoaderDiv.innerHTML = "";
            PrintLoaderDiv.innerHTML = '<object id="PrintFrame" onreadystatechange="idPdf_onreadystatechange()"' + 'width="1px" height="1px" type="application/pdf"' + 'data="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + 'print.pdf" type="application/pdf">' + '<span>PDF plugin is not available.</span>' + '</object>';
            var msgControl = "#<%= divMsg.ClientID %>";
            var params = getParameterByName("id");
            return CallPostScalar(msgControl, "UpdateTrackTableOnPrint", params);
        }

        function PrintPdf() {
            var pdf = document.getElementById('PrintFrame');
            pdf.Print();
        }

        function idPdf_onreadystatechange() {
            var pagecount = document.getElementById("<%= DDLDrop.ClientID %>");
            var printready = document.getElementById('PrintFrame');
            if (pagecount.options.length <= 50) {
                if (printready.readyState === 4)
                    setTimeout(PrintPdf, 1000);
            }
            else if (pagecount.options.length > 51 && pagecount.options.length <= 100) {
                if (printready.readyState === 4)
                    setTimeout(PrintPdf, 2000);
            }
            else if (pagecount.options.length > 101 && pagecount.options.length <= 500) {
                if (printready.readyState === 4)
                    setTimeout(PrintPdf, 5000);
            }
            else if (pagecount.options.length > 501 && pagecount.options.length <= 1000) {
                if (printready.readyState === 4)
                    setTimeout(PrintPdf, 15000);
            }
            else if (pagecount.options.length > 1001 && pagecount.options.length <= 1500) {
                if (printready.readyState === 4)
                    setTimeout(PrintPdf, 20000);
            }
            else if (pagecount.options.length > 1501 && pagecount.options.length <= 2000) {
                if (printready.readyState === 4)
                    setTimeout(PrintPdf, 25000);
            }
        }


        function OnDDlchanged(PageID) {
            var msgControl = "#<%= divMsg.ClientID %>";
            var UploadID = getParameterByName("id");
            document.getElementById("<%= DDLDrop.ClientID %>").value = PageID;
            var params = UploadID + '|' + PageID;
            document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;
            return CallPostScalar(msgControl, "GetTagDetails", params);

        }

        function gotoPage() {
            navigationHandler('GOTO');
        }

        function navigationHandler(action) {
            var PageNo = parseInt(document.getElementById("<%=hdnPageNo.ClientID %>").value, 10);
            var PagesCount = parseInt($("#<%= DDLDrop.ClientID %> option").length, 10);
            var TempPagenumber = parseInt(document.getElementById("<%=hdntempPagecount.ClientID %>").value, 10);

            if (typeof PageNo != 'undefined' && typeof PagesCount != 'undefined') {

                // First Page
                if (action.toUpperCase() == 'FIRST' && PageNo > 0 && TempPagenumber <= PagesCount && TempPagenumber != 1) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }

                // Previous page
                else if (action.toUpperCase() == 'PREVIOUS' && TempPagenumber > 1 && TempPagenumber <= PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }

                // Next page
                else if (action.toUpperCase() == 'NEXT' && TempPagenumber > 0 && TempPagenumber < PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }

                // Last page
                else if (action.toUpperCase() == 'LAST' && TempPagenumber > 0 && TempPagenumber <= PagesCount && TempPagenumber != PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }

                // Goto page
                else if (action.toUpperCase() == 'GOTO' && TempPagenumber > 0 && TempPagenumber <= PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }
            }
        }


        function setLayoutMode(objLayout) {
            var objPdf = document.getElementById('frame1');
            var sel = objLayout.selectedIndex;
            var layout = objLayout.options[sel].value;

            objPdf.setLayoutMode(layout);
        }

        function setView(objView) {
            var objPdf = document.getElementById('frame1');
            var sel = objView.selectedIndex;
            var view = objView.options[sel].value;

            objPdf.setView(view);
        }

        function setReader() {
            var objPdf = document.getElementById('frame1');
            var objView = document.getElementById('view');
            var objLayout = document.getElementById('layout');

            setView(objView);
            setLayoutMode(objLayout);

            objPdf.setShowToolbar(0);
            objPdf.setCurrentPage(1);
            objPdf.setPageMode("none");
        }

        function LoadSubTag(ID) {
            var e = document.getElementById(ID.id);
            var drop = e.options[e.selectedIndex].value;
            if (drop != "0" && drop != "<Select>") {
                //document.getElementById('<%=ddldocumentview.ClientID %>').value = 'Orginal View';
                (document.getElementById('<%=btnCommonSubmitSub2.ClientID%>')).click();
            }
            return false;
        }
        function LoadPagesForTag(ID) {
            var DocumentView = $("#<%= ddldocumentview.ClientID %>").val();
            var e = document.getElementById("<%= cmbSubTag.ClientID %>");
            var CmdSubtagValue = e.options[e.selectedIndex].value; //TaskWDMS-S-5046 A
            document.getElementById("<%= hdnsubtagvalue.ClientID %>").value = CmdSubtagValue; //TaskWDMS-S-5046 A
            //TaskWDMS-S-5046 BS A
            if (DocumentView == 'Tagged View') {
                document.getElementById('<%=btnCommonSubmitSub3.ClientID%>').click();
            }
            return false; //TaskWDMS-S-5046 BE A
        }

        function deletebtndis() {
            document.getElementById('<%=btnDeletePages.ClientID%>').disabled = true;
            return true;
        }



        function validateInput() {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            var email = document.getElementById('<%=txtMailTo.ClientID %>').value;
            if (reg.test(email) == false) {
                alert("Please Verify that Correct Email Address is Entered!");
                document.getElementById('<%=txtMailTo.ClientID %>').focus();
                return false;
            }
            else {
                return true;
            }
        }
        /*DMS04-3400 BS Annotations should work in all browsers*/
        function RemoveXmlHeadingForFirefox(AnotationXML) {
            var isFirefox = typeof InstallTrigger !== 'undefined';   // Firefox 1.0+
            var XML;
            if (isFirefox) {

                XML = AnotationXML.substring(39, AnotationXML.length);
                XML = XML.replace(/\s+/g, '');

            }
            else {
                XML = AnotationXML;
            }
            return XML;
        }
        /*DMS04-3400 BE Annotations should work in all browsers*/
        function SaveDocumentAnnotation(annotations, documentWithAnnotations) {
            /*DMS04-3400 BS Annotations should work in all browsers*/
            // Chrome browser will return xml header, replace to work annotation save functionality
            annotations = annotations.replace('<?xml version="1.0" encoding="utf-8"?>', '');
            annotations = RemoveXmlHeadingForFirefox(annotations);


            /*DMS04-3400 BE Annotations should work in all browsers*/
            //Get xml :  only annotations
            document.getElementById("<%=hdnAnnotaionXML.ClientID%>").value = Encoder.htmlEncode(annotations, false);

            //Get base64string :  Image and annotations
            document.getElementById("<%=hdnAnnotionwithDoc.ClientID%>").value = Encoder.htmlEncode(documentWithAnnotations, false);

            // Call code behind method to save annotations to database
            document.getElementById("<%= hdnAction.ClientID %>").value = 'SAVEANNOTATIONS';
            document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
        }

        function loadImageAndAnnotations(imgPath) {

            // Call annotation library setImage() function to load image to viewer
            setImage(imgPath);

            //Get xml :  only annotations
            var xmlAnnotations = Encoder.htmlDecode(document.getElementById("<%=hdnAnnotaionXML.ClientID%>").value);

            // Call annotation library annLoad() function to load image to viewer
            if (typeof xmlAnnotations != 'undefined' && xmlAnnotations.toString().length > 0) {
                loadAnnotations(xmlAnnotations, 1);
            }

            // Load tag details of currently viewed page
            loadTagDetails();
            setTimeout(function () {
                var zoomSelect = document.getElementById("zoomSelect");
                zoomSelect.selectedIndex = 2;
                $("#zoomSelect").change();
            }, 100);
        }

        function loadTagDetails() {
            // Load tag details of currently viewed page

            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageNo.ClientID %>";

            // document id
            var UploadID = getParameterByName("id");

            // current value of PageNo dropdown
            var PageID = $("#<%= DDLDrop.ClientID %>").val();

            document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;
            var params = UploadID + '|' + PageID;
            //new code 
            var TotalPages = document.getElementById("<%=hdnPagesCount.ClientID %>").value;
            document.getElementById('totalpagescount').innerHTML = TotalPages;
            //new code 
            var msgControl = "#<%= divMsg.ClientID %>";
            return CallPostScalar(msgControl, "GetTagDetails", params);
        }



        function getpagenumbers(Pagenumberdetails, TotalPages) {
            var pageNumbersXml = '<xmlTable>';
            var pagenum = Pagenumberdetails;
            var Isval = true;
            // 1-5,15,18,19
            //pushing all the results 
            var pagenumarray = pagenum.split(',');
            for (i = 0; i < pagenumarray.length; i++) {
                var Tpageno = pagenumarray[i];
                //checking if pageno contain delimitter if yes
                if (Tpageno.indexOf('-') > 0) {
                    var pagen = Tpageno.split('-');
                    if (pagen.length <= 2) {

                        var x = parseInt(pagen[0], 10);
                        //checking if firstvalue is 0 or not
                        if (x == 0) {
                            Isval = false;
                            message = 'Invalid page number! Please enter valid page number(s).';  // DMS04-3712M -- message modified
                            break;
                        }
                        var y = parseInt(pagen[1], 10);
                        if (x < y) {

                            for (k = x; k <= y; k++) {
                                if (k <= TotalPages) {
                                    pageNumbersXml += '<pageNo><page>' + k + '</page></pageNo>';
                                }
                                else {
                                    Isval = false;
                                    message = 'Invalid page number! Please enter valid page number(s).';  // DMS04-3712M -- message modified
                                    break;
                                }
                            }
                        }
                        else {
                            //if unexpeced value comes then to throw alert for user  
                            Isval = false;
                            message = 'Page number range(s) should be in ascending order!';
                            break;

                        }
                    }
                    else {
                        Isval = false;
                        message = 'Page selection contain extra charachters remove it and try!';
                        break;
                    }
                }
                else {
                    //if no delimitter then set the value directly.
                    if (parseInt(Tpageno, 10) <= parseInt(TotalPages, 10) && parseInt(Tpageno, 10) > 0) {
                        pageNumbersXml += '<pageNo><page>' + Tpageno + '</page></pageNo>';
                    }
                    else {
                        Isval = false;
                        message = 'Invalid page number! Please enter valid page number(s).';  // DMS04-3712M -- message modified
                        break;
                    }
                }
            }
            pageNumbersXml += '</xmlTable>';
            pageNumbersXml = pageNumbersXml.replace(/\s+/g, '');
            if (Isval != true) {
                pageNumbersXml = '';
            }

            return pageNumbersXml;
        }
        function validate() {
            var msgControl = "#<%= divMsg.ClientID %>";


            var keyword = $.trim($("#<%= txtKeyword.ClientID %>").val());
            var count = $("#<%=hdnCountControls.ClientID %>").val();
            var IndexNames = $("#<%=hdnIndexNames.ClientID %>").val().split('|');
            var MinLen = $("#<%=hdnIndexMinLen.ClientID %>").val().split('|');
            var EntryType = $("#<%=hdnIndexType.ClientID %>").val().split('|');
            var DataType = $("#<%=hdnIndexDataType.ClientID %>").val().split('|');
            var Mandatory = $("#<%=hdnMandatory.ClientID %>").val().split('|');
            var Controlnames = $("#<%=hdnControlNames.ClientID %>").val().split('|');
            var retval = true;
            if (keyword.length < 2) {
                $(msgControl).html("Keyword field should contain atleast two characters!");
                return false;
            }
            for (i = 0; i < parseInt(count); i++) {
                if (EntryType[i] != "Multiple Field Selection") {
                    var controlname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_' + IndexNames[i].replace(/\s+/g, '');
                    var Index = document.getElementById(controlname);
                    if (DataType[i] == "DateTime") {
                        if ((parseInt(Index.value.length) < 10) && (Mandatory[i] == "true")) {
                            $(msgControl).html(IndexNames[i] + " field date not in correct format!");
                            return false;
                        }
                    }
                    else if (DataType[i] == "Boolen") {
                        if ((parseInt(Index.value.length) < 1) && (Mandatory[i] == "true")) {
                            $(msgControl).html(IndexNames[i] + " field should contain '0' or '1'!");
                            return false;
                        }
                    }
                    else {
                        if ((parseInt($.trim(Index.value).length) == 0) && (Mandatory[i] == "true")) {
                            $(msgControl).html(IndexNames[i] + " field cannot be blank!");
                            return false;
                        }
                        else if ((parseInt($.trim(Index.value).length) < parseInt(MinLen[i].length)) && (Mandatory[i] == "true")) {
                            $(msgControl).html(IndexNames[i] + " field should contain atleast " + MinLen[i] + " values!");
                            return false;
                        }
                    }
                }

                else {
                    var controlname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_' + IndexNames[i];
                    var Index = document.getElementById(controlname);
                    if ((Index != null) && (Index.options[Index.selectedIndex].text.toLowerCase() == "--select--") && (Mandatory[i] == "true")) {
                        $(msgControl).html("Please select " + IndexNames[i] + "!");
                        return false;
                    }
                    controlname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_' + Controlnames[i];
                    Index = document.getElementById(controlname);
                    if ((Index.options[Index.selectedIndex].text.toLowerCase() == "--select--") && (Mandatory[i] == "true")) {
                        $(msgControl).html("Please select " + IndexNames[i] + "!");
                        return false;
                    }
                }

            }
            return true;
        }
        //TaskWDMS-S-5046 BS A
        function DDLDocumentViewChanged() {

            var e = document.getElementById("<%= cmbSubTag.ClientID %>");
            var CmdSubtagValue = e.options[e.selectedIndex].value;
            document.getElementById("<%= hdnsubtagvalue1.ClientID %>").value = CmdSubtagValue; 
            document.getElementById("<%= btnddlchanged.ClientID %>").click();
        }
        //TaskWDMS-S-5046 BE A
        //Email pop up

        function ShowMD() {

            modelBG.className = "mdBG";
            mb.className = "mdbox";
            return false;
        };
        function HideMDs() {

            modelBG.className = "mdNone";
            mb.className = "mdNone";
        };


        //* Certficate Section */

        function ValidateCertificate(sender, args) {


            var result;
            var msgControl = "#<%= divMsg.ClientID %>";
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1);
            var err = new Error();
            if (filext == 'pfx' || filext == 'PFX') {

                return true;
            }
            else {

                err.name = 'My API Input Error';
                err.message = 'Certificate format not supported! (Supported format ' + filext + ')';
                throw (err);
                return false;
            }
        }

        function HideMD(Action) {




            CertificateBG.className = "mdNone";

            Certificate.className = "mdNone";

        };

        function ShowApplySignaturePopUP() {
            var msgControl = "#<%= divMsg.ClientID %>";
            if (document.getElementById("<%= hdnIsDigitallySigned.ClientID %>").value == 'False') {
                document.getElementById('<%=btnadd.ClientID %>').disabled = true;
                CertificateBG.className = "mdBG";

                Certificate.className = "mdbox";
            }
            else {
                $(msgControl).html("Already Signature Is Applied..!");
            }
            return false;
        }
        function RemoveSignature() {

            var msgControl = "#<%= divMsg.ClientID %>";
            if (document.getElementById("<%= hdnIsDigitallySigned.ClientID %>").value == 'False') {
                $(msgControl).html("No signature is applied to remove.");
                return false;
            }
            else {
                return true;
            }

        }

        function validateCertficateDetails() {


            var msgControl = "#<%= divMsg.ClientID %>";
            var Password = document.getElementById("<%= txtpassword.ClientID %>").value;
            var auther = document.getElementById("<%= txtauther.ClientID %>").value;
            var Title = document.getElementById("<%= txtTitle.ClientID %>").value;
            var subject = document.getElementById("<%= txtsubject.ClientID %>").value;
            var keywods = document.getElementById("<%= txtkeywods.ClientID %>").value;
            var creater = document.getElementById("<%= txtcreater.ClientID %>").value;
            var producer = document.getElementById("<%= txtproducer.ClientID %>").value;
            var Reason = document.getElementById("<%= txtReason.ClientID %>").value;
            var contact = document.getElementById("<%= txtcontact.ClientID %>").value;
            var location = document.getElementById("<%= txtlocation.ClientID %>").value;
            if (Password.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Password cannot be empty!");
                return false;
            }
            else if (auther.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Author cannot be empty!");
                return false;
            }
            else if (Title.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Title  cannot be empty!");
                return false;
            }
            else if (subject.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Subject cannot be empty!");
                return false;
            }
            else if (keywods.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Keywords cannot be empty!");
                return false;
            }
            else if (creater.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Creator cannot be empty!");
                return false;
            }
            else if (producer.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Producer cannot be empty!");
                return false;
            }
            else if (Reason.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Reason cannot be empty!");
                return false;
            }
            else if (contact.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Contact cannot be empty!");
                return false;
            }
            else if (location.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Location cannot be empty!");
                return false;
            }

            return true;
        }

        function enableCersave() {
            document.getElementById('<%=btnadd.ClientID %>').disabled = false;

        }
        function Redirect_History() {
            var DcoumentId = getParameterByName("id");
            window.location.href = "DocumentHistoryView.aspx?id=" + DcoumentId;
            return false;
        }

        //*************************************************************************************************
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Document Download">
    </asp:Label>
    <div class="GVDiv">
        <asp:Label ID="lblPageHeader" runat="server" CssClass="PageHeadings" Text="Document Details"
            Font-Bold="True"></asp:Label>
        &nbsp; &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Button ID="btnCancel" CssClass="btnsearchagain"
            runat="server" Text="Search Again" OnClick="btnCancel_Click" TagName="Read" />
        <br />
        <div>
            <div id="divMsg" runat="server" style="word-wrap: break-word; color: Red;">
            </div>
            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
        </div>
        <table cellpadding="3" cellspacing="3">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="Project Type  :"> </asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblDocType" runat="server" CssClass="LabelStyle"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label5" runat="server" CssClass="LabelStyle" Text="Department :"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblDept" runat="server" CssClass="LabelStyle"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label9" runat="server" CssClass="LabelStyle" Text="Ref ID :"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblRefID" runat="server" CssClass="LabelStyle"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label15" runat="server" Text="Version :" CssClass="LabelStyle"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblVersion" runat="server" CssClass="LabelStyle"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label12" runat="server" CssClass="LabelStyle" Text="Keywords :"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtKeyword" runat="server" TextMode="MultiLine" ReadOnly="True"></asp:TextBox>
                            </td>
                        </tr>
                        <asp:Panel ID="TagPanel" runat="server">
                            <tr>
                                <td>
                                    <asp:Label ID="Label7" runat="server" CssClass="LabelStyle" Text="Page"></asp:Label>
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="DDLDrop" AutoPostBack="false" runat="server" TagName="Read">
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="hdnsubtagvalue1" runat="server" Value="0" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <input id="btnFirst" onclick="navigationHandler('FIRST');" type="button" class="btnfirst"
                                        title="First" />
                                    <input id="btnPrevious" onclick="navigationHandler('PREVIOUS');" type="button" class="btnleftarrow"
                                        title="Previous" />
                                    <input id="btnNext" onclick="navigationHandler('NEXT');" type="button" class="btnrightarrow"
                                        title="Next" />
                                    <input id="btnLast" onclick="navigationHandler('LAST');" type="button" class="btnlast"
                                        title="Last" />
                                </td>
                            </tr>
                            <tr>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label10" runat="server" CssClass="LabelStyle" Text="Tag Details" Font-Bold="True"></asp:Label>
                                </td>
                                <td align="left">
                                    <asp:UpdatePanel ID="UpdatePanel12" runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <div id="TotalDiv" runat="server">
                                                <span id="taggedpagescount"></span>&nbsp of &nbsp<span id="totalpagescount"></span>
                                                pages tagged
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDcoumentView" runat="server" CssClass="LabelStyle" Text="Document View"></asp:Label>
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel10" runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="ddldocumentview" runat="server">
                                                <asp:ListItem>Original View</asp:ListItem>
                                                <asp:ListItem>Tagged View</asp:ListItem>
                                                <asp:ListItem>NonTagged View</asp:ListItem>
                                                <asp:ListItem>FullText View</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Button ID="btnddlchanged" runat="server" Text="Button" Style="visibility: hidden"
                                                CausesValidation="False" UseSubmitBehavior="false" OnClick="btnddlchanged_Click" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label8" runat="server" CssClass="LabelStyle" Text="Main Tag"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="cmbMainTag" runat="server" AutoPostBack="false">
                                        <asp:ListItem Value="0">---select---</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label16" runat="server" CssClass="LabelStyle" Text="Sub Tag"></asp:Label>
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="cmbSubTag" runat="server">
                                                <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Button ID="btnCommonSubmitSub2" runat="server" Text="Button" OnClick="btnCommonSubmitSub2_Click"
                                                CausesValidation="False" Style="visibility: hidden" UseSubmitBehavior="false" />
                                            <asp:Button ID="btnCommonSubmitSub3" runat="server" Text="Button" OnClick="btnCommonSubmitSub3_Click"
                                                CausesValidation="False" Style="visibility: hidden" UseSubmitBehavior="false" />
                                            <asp:HiddenField ID="hdnsubtagvalue" runat="server" Value="0" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lbltagpages" runat="server" CssClass="LabelStyle" Text="Page Numbers"></asp:Label>
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel11" runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <asp:TextBox ID="txttagpages" runat="server"></asp:TextBox>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Button ID="btnSaveTag" runat="server" Text="Save Tag" CssClass="btnsave" Width="100px"
                                                TagName="Read" OnClientClick="return savetagdetails('AddOrUpdateTagDetails')"
                                                OnClick="btnSaveTag_Click" />
                                            <asp:Button ID="btndeletetag" runat="server" Text="Delete Tag" CssClass="clearbutton"
                                                Width="100px" TagName="Read" OnClientClick="return savetagdetails('DeleteTagDetails')"
                                                OnClick="btndeletetag_Click" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </asp:Panel>
                        <asp:Panel ID="RemarksPanel" runat="server">
                            <tr>
                                <td>
                                    <asp:Label ID="lblstatus" runat="server" CssClass="LabelStyle" Text="Document Status"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlDocStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlDocStatus_SelectedIndexChanged">
                                        <asp:ListItem Value="0">--Select--</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblStatusRemarks" runat="server" CssClass="LabelStyle" Text="Status Remarks"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlStatusRemarks" runat="server">
                                        <asp:ListItem Value="0">--Select--</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblRemarks" runat="server" CssClass="LabelStyle" Text="Remarks"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtRemarks" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblOldRemarks" runat="server" CssClass="LabelStyle" Text="Previous remarks"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtoldremarks" runat="server" Style="color: #888; border-color: #FFFFFF"
                                        TextMode="MultiLine" ReadOnly="True"></asp:TextBox>
                                </td>
                            </tr>
                        </asp:Panel>
                        <asp:Panel ID="DocumentDetails" runat="server">
                            <tr>
                                <td>
                                    <asp:Label ID="Label14" runat="server" CssClass="LabelStyle" Text="Index Properties"
                                        Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div class="GVDiv">
                                        <asp:UpdatePanel ID="Indexupdatepanel" runat="server">
                                            <ContentTemplate>
                                                <asp:Panel ID="pnlIndexpro" runat="server">
                                                </asp:Panel>
                                                <asp:Button ID="btnInedxSave" runat="server" Text="Save Index Values" CssClass="btnsave"
                                                    Width="140px" TagName="Read" OnClick="btnInedxSave_Click" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </td>
                            </tr>
                        </asp:Panel>
                        <asp:Panel ID="pnlControls" runat="server">
                            <tr>
                                <td>
                                    <asp:Label ID="lblDocEdit" runat="server" CssClass="LabelStyle" Text="PDF/TIFF Doc Edit"
                                        Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="Insert New Page(s)"
                                        Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label1" runat="server" CssClass="LabelStyle" Text="Attach Page at "> </asp:Label>
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel6" runat="server" >
                                        <ContentTemplate>
                                            <asp:DropDownList ID="drpPageCount" runat="server" TagName="Edit">
                                            </asp:DropDownList>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblDocUpload" runat="server" CssClass="LabelStyle" Text="Upload New File to Attach"> </asp:Label>
                                </td>
                                <td>
                                    <asp:AsyncFileUpload ID="AsyncFileUpload1" runat="server" CssClass="LabelStyle" CompleteBackColor="Lime"
                                        ErrorBackColor="Red" ThrobberID="Throbber" OnUploadedComplete="AsyncFileUpload1_UploadedComplete"
                                        OnClientUploadStarted="disablecommit" UploadingBackColor="#66CCFF" OnClientUploadComplete="enablesave"
                                        Width="213px" />
                                    <asp:Label ID="Throbber" runat="server" Style="display: none" TagName="Edit"> <img alt="Loading..." src="<%= Page.ResolveClientUrl("~/Images/indicator.gif")%>" /></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label4" runat="server" CssClass="LabelStyle" Text="Delete Page(s)"
                                        Font-Bold="True" Visible="false"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label6" runat="server" CssClass="LabelStyle" Text="Delete Page(s)"></asp:Label>
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="drpDeleteCount" runat="server">
                                            </asp:DropDownList>
                                            &nbsp
                                            <asp:Button ID="btnDeletePages" runat="server" Text="" CssClass="clearbutton" Enabled="false"
                                                OnClick="btnDeletePages_Click" TagName="Delete" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <%-- <asp:UpdatePanel ID="updatePanelSubmit" runat="server" UpdateMode="Always">
                             <ContentTemplate>--%>
                            <tr>
                                <td colspan="2">
                                    <br />
                                    <asp:Button ID="btnCommitChanges" runat="server" Text="Commit Changes" OnClick="btnCommitChanges_Click"
                                        CssClass="btncommit" TagName="Upload" />
                                    <asp:Button ID="btnDicardChanges" runat="server" Text="Discard Changes" CssClass="btnclose"
                                        Width="130px" OnClick="btnDicardChanges_Click" TagName="Upload" />
                                </td>
                            </tr>
                            <%-- </contenttemplate>
                     </asp:UpdatePanel>--%>
                        </asp:Panel>
                        <asp:Panel ID="pnlEmail" runat="server">
                            <tr>
                                <td colspan="2">
                                    <div id="modelBG" class="mdNone">
                                    </div>
                                    <div id="mb" class="mdNone">
                                        <asp:UpdatePanel ID="updatePanelMail" runat="server">
                                            <ContentTemplate>
                                                <table>
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:Label ID="lblMesg" runat="server" Text="" Visible="false"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblMailTo" runat="server" Text="Mail To" CssClass="LabelStyle"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtMailTo" runat="server" Width="350px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblName" runat="server" Text="Name" CssClass="LabelStyle"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtName" runat="server" Width="350px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblSubject" runat="server" Text="Subject" CssClass="LabelStyle"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtsubject" runat="server" Width="350px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblMessage" runat="server" Text="Message" CssClass="LabelStyle"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Height="100px" Width="350px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            &nbsp;
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btnSendMail" runat="server" Text="Send Mail" TagName="Share" OnClick="btnSendMail_Click"
                                                                CssClass="btnemail" OnClientClick="return validateInput();" />
                                                            <asp:Button ID="btnEmailCancel" runat="server" Text="Close" CssClass="btnclose" OnClientClick="HideMDs(); return false;" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </td>
                            </tr>
                        </asp:Panel>
                    </table>
                </td>
                <td style="vertical-align: top;">
                    <table>
                        <tr>
                            <td>
                                <div class="DivInlineBlockPage">
                                    <asp:Button ID="btnLock" runat="server" ToolTip="Edit" CssClass="image-button document-edit"
                                        OnClick="btnLock_Click" TagName="Edit" />
                                    <asp:Button ID="btnDownload" CssClass="image-button document-download" runat="server"
                                        Text="" ToolTip="Download orginal document" OnClick="btnDownload_Click" TagName="Download" />
                                    <asp:Button ID="btnDownloadWithAnn" CssClass="image-button document-download-with-annotation"
                                        runat="server" ToolTip="Download document with annotations" TagName="Download Annotated"
                                        OnClick="btnDownloadWithAnn_Click" />
                                    <asp:Button ID="btnUpdate" runat="server" CssClass="image-button document-upload"
                                        ToolTip="Upload newer version" OnClick="btnUpdate_Click1" TagName="Upload" />
                                    <asp:Button ID="btnDelete" runat="server" CssClass="image-button document-delete"
                                        OnClientClick="return Confirmationfordelete();" ToolTip="Delete/discard document"
                                        OnClick="btnDelete_Click1" TagName="Delete" />
                                    <asp:Button ID="btnSendEmail" runat="server" CssClass="image-button document-email"
                                        ToolTip="Email" OnClientClick="return ShowMD();" TagName="Delete" />
                                    
                                    <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>
                                        <asp:Button ID="btnHistory" runat="server" CssClass="image-button document-History"
                                        ToolTip="History" OnClientClick="return Redirect_History();" TagName="Delete" />
                                            <asp:Button ID="btnprint" runat="server" CssClass="image-button document-print" ToolTip="Print"
                                                TagName="Print" OnClick="btnprint_Click" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <asp:Label ID="lblsignatureDetails" runat="server" Text="" CssClass="LabelStyle"></asp:Label>
                                  <%--  <asp:UpdatePanel ID="UpdatePanel13" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>--%>
                                            <uc1:PDFViewer ID="PDFViewer1" runat="server" />
                                       <%-- </ContentTemplate>
                                    </asp:UpdatePanel>--%>
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <select id="layout" onchange="setLayoutMode(this);" style="visibility: hidden">
                                                <option value="DontCare">Dont Care</option>
                                                <option value="SinglePage">Single Page</option>
                                                <option value="OneColumn">One Column</option>
                                                <option selected="selected" value="TwoColumnLeft">Two Column Left</option>
                                                <option value="TwoColumnRight">Two Column Right</option>
                                            </select>
                                            <select id="view" onchange="setView(this);" style="visibility: hidden">
                                                <option value="Fit">Fit Page</option>
                                                <option value="FitH">Fit Width</option>
                                                <option selected="selected" value="FitV">Fit Height</option>
                                                <option value="FitB">Fit Bounding Box</option>
                                                <option value="FitBH">Fit Bounding Box Width</option>
                                                <option value="FitBV">Fit Bounding Box Height</option>
                                            </select>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="btnReloadiFrame" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="btnDeletePages" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>
                                            <div id="PrintOuterPanel">
                                                <div id="PrintInnerPanel">
                                                </div>
                                            </div>
                                            <asp:Button ID="btnReloadiFrame" class="HiddenButton" runat="server" Text="ReloadiFrame"
                                                OnClick="btnReloadiFrame_Click" TagName="Read" />
                                            <asp:Button ID="btnCallFromJavascript" class="HiddenButton" runat="server" OnClick="btnCallFromJavascript_Click"
                                                TagName="Read" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <div id="CertificateBG" class="mdNone">
        </div>
        <div id="Certificate" class="mdNone">
            <table>
                <tr>
                    <td>
                        <div id="divCertMsg" runat="server" style="color: Red">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnCertificateCancel" TagName="Edit" runat="server" Text="" OnClientClick="HideMD('Certficate'); return false;"
                            CssClass="CloseButton" CausesValidation="false" meta:resourcekey="btnCancel" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblCertificate" runat="server" Text="Certificate"></asp:Label>
                    </td>
                    <td>
                        <asp:AsyncFileUpload ID="AsyncFileUpload2" runat="server" AsyncPostBackTimeout="1600"
                            CssClass="LabelStyle" Width="200px" CompleteBackColor="Lime" ErrorBackColor="Red"
                            OnClientUploadComplete="enableCersave" OnClientUploadStarted="ValidateCertificate"
                            ThrobberID="Throbber" UploadingBackColor="#66CCFF" OnUploadedComplete="AsyncFileUpload2_UploadedComplete" />
                        <asp:Label ID="Label11" runat="server" Style="display: none">
                          <img alt="Loading..." src="<%= Page.ResolveClientUrl("~/Images/indicator.gif")%>" /></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblpassword" runat="server" Text="Password"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtpassword" runat="server" TextMode="Password" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblAuther" runat="server" Text="Author"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtauther" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblTitle" runat="server" Text="Title"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtTitle" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label13" runat="server" Text="Subject"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="TextBox1" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblKeywords" runat="server" Text="Keywords"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtkeywods" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblcreater" runat="server" Text="Creator"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtcreater" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblproducer" runat="server" Text="Producer"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtproducer" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtReason" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblcontact" runat="server" Text="Contact"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtcontact" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbllocation" runat="server" Text="Location"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtlocation" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox ID="chkvisibleSignature" Text="Check to make Signature visible" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;&nbsp;&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnadd" runat="server" TagName="Edit" Text="Add" OnClientClick="return validateCertficateDetails();"
                            class="DigitalSign" OnClick="btnadd_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="Buttons" style="padding-left: 0.6em">
            <asp:Button ID="btnAddDigitalSignature" runat="server" OnClientClick="return ShowApplySignaturePopUP();"
                class="DigitalSign" Text="Apply Signature" TagName="Edit" />
            <asp:Button ID="btnRemoveDigitalSignature" runat="server" Text="Remove Signature"
                class="btnDocclear" OnClientClick="return RemoveSignature();" OnClick="btnRemoveDigitalSignature_Click"
                TagName="Edit" />
        </div>
        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Always" RenderMode="Inline">
            <ContentTemplate>
                <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                <asp:HiddenField ID="hdnPagesCount" runat="server" Value="" />
                <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                <asp:HiddenField ID="hdnCountControls" runat="server" Value="" />
                <asp:HiddenField ID="hdnButtonAction" runat="server" Value="" />
                <asp:HiddenField ID="hdnFileName" runat="server" Value="" />
                <asp:HiddenField ID="hdnFileLocation" runat="server" Value="" />
                <asp:HiddenField ID="hdnEncrpytFileName" runat="server" Value="" />
                <asp:HiddenField ID="hdnTempFileLocation" runat="server" Value="" />
                <asp:HiddenField ID="hdnPDFPathForObject" runat="server" Value="" />
                <asp:HiddenField ID="hdnFramePath" runat="server" Value="" />
                <asp:HiddenField ID="hdnFileType" runat="server" Value="" />
                <asp:HiddenField ID="hdnPageNo" runat="server" Value="" />
                <asp:HiddenField ID="hdnUploaded" runat="server" Value="" />
                <asp:HiddenField ID="hdnAnnotaionXML" runat="server" />
                <asp:HiddenField ID="hdnAnnotionwithDoc" runat="server" />
                <asp:HiddenField ID="hdnIndexNames" runat="server" Value="" />
                <asp:HiddenField ID="hdnIndexMinLen" runat="server" Value="" />
                <asp:HiddenField ID="hdnIndexType" runat="server" Value="" />
                <asp:HiddenField ID="hdnIndexDataType" runat="server" Value="" />
                <asp:HiddenField ID="hdnMandatory" runat="server" Value="" />
                <asp:HiddenField ID="hdnControlNames" runat="server" Value="" />
                <asp:HiddenField ID="hdnMainvalueid" runat="server" Value="" />
                <asp:HiddenField ID="hdntempPagecount" runat="server" Value="" />
                <asp:HiddenField ID="hdnIsDigitallySigned" runat="server" Value="" />
                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
