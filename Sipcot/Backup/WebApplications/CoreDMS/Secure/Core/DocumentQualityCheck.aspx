<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="DocumentQualityCheck.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.DocumentQualityCheck" %>

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
            if (document.getElementById("<%= drpPageCount.ClientID  %>").value == 0) {

                err.name = 'My API Input Error';
                err.message = 'Please select positon!';
                throw (err);
                return false;
            }

            if (filext == 'pdf' || filext == 'tif' || filext == 'tiff' || filext == 'TIF' || filext == 'TIFF' || filext == 'PDF') {
                return true;
            }
            else {

                err.name = 'My API Input Error';
                err.message = 'File format not supported! (Supported format ' + filext + ')';
                throw (err);
                return false;
            }
        }

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

        function deletebtndis() {
            document.getElementById('<%=btnDeletePages.ClientID%>').disabled = true;
            return true;
        }





        function SaveDocumentAnnotation(annotations, documentWithAnnotations) {

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
                            message = 'PageNo Cannot be 0 !'
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
                                    message = 'PageNo Entered is greater than total pages !'
                                    break;
                                }
                            }
                        }
                        else {
                            //if unexpeced value comes then to throw alert for user  
                            Isval = false;
                            message = 'Page selection should be in ascending order !'
                            break;

                        }
                    }
                    else {
                        Isval = false;
                        message = 'Page selection contain extra charachters remove it and try !'
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
                        message = 'PageNo Entered is greater than total pages !'
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
        function validateUpdate() {
            var msgControl = "#<%= divMsg.ClientID %>";
            document.getElementById('<%=divMsg.ClientID%>').style.color = 'Red';
            if (document.getElementById("<%= ddlDocStatus.ClientID  %>").value == 0) {
                $(msgControl).html("Please Select Status before saving!");
                return false;
            }
            return true;
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
                    var controlname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_' + IndexNames[i];
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

        function DDLDocumentViewChanged() {


            document.getElementById("<%= btnddlchanged.ClientID %>").click();
        }

        //*************************************************************************************************
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="PageContentsDiv">
        <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Document Download">
        </asp:Label><br />
        <asp:Label ID="lblPageHeader" runat="server" CssClass="PageHeadings" Text="Document Details"
            Font-Bold="True"></asp:Label>
        &nbsp; &nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<asp:Button ID="btnCancel" CssClass="ButtonStyle"
            runat="server" Text="Search Again" OnClick="btnCancel_Click" TagName="Read" />
        <br />
        <div>
            <asp:Label ID="lblResult" runat="server" Text="" Visible="false"></asp:Label>
        </div>
        <div class="" style="float: left">
            <%-- <asp:Panel ID="DocumentDetails_Main" runat="server">--%>
            <table cellpadding="3" cellspacing="3" class="style1">
                <tr>
                    <td class="style5" colspan="2">
                        <div id="divMsg" runat="server" style="color: Red">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="style5">
                        <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="Project Type  :"> </asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblDocType" runat="server" CssClass="LabelStyle"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="style5">
                        <asp:Label ID="Label5" runat="server" CssClass="LabelStyle" Text="Department :"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblDept" runat="server" CssClass="LabelStyle"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="style5">
                        <asp:Label ID="Label9" runat="server" CssClass="LabelStyle" Text="Ref ID :"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="lblRefID" runat="server" CssClass="LabelStyle"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="style2">
                        <asp:Label ID="Label15" runat="server" Text="Version :" CssClass="LabelStyle"></asp:Label>
                    </td>
                    <td class="style2">
                        <asp:Label ID="lblVersion" runat="server" CssClass="LabelStyle"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="style2">
                        <asp:Label ID="Label12" runat="server" CssClass="LabelStyle" Text="Keywords :"></asp:Label>
                    </td>
                    <td class="style2">
                        <asp:TextBox ID="txtKeyword" runat="server" CssClass="TextBoxStyle" TextMode="MultiLine"
                            Height="63px" ReadOnly="True" Width="193px"></asp:TextBox>
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
                                    <asp:DropDownList ID="DDLDrop" AutoPostBack="false" Style="width: 57px;" runat="server"
                                        TagName="Read">
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <input id="btnFirst" onclick="navigationHandler('FIRST');" value="<< " type="button"
                                class="ButtonStyle" title="First" />
                            <input id="btnPrevious" onclick="navigationHandler('PREVIOUS');" value="< " type="button"
                                class="ButtonStyle" title="Previous" />
                            <input id="btnNext" onclick="navigationHandler('NEXT');" value=" >" type="button"
                                class="ButtonStyle" title="Next" />
                            <input id="btnLast" onclick="navigationHandler('LAST');" value=" >>" type="button"
                                class="ButtonStyle" title="Last" />
                        </td>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                        <td class="style5">
                            <asp:Label ID="Label10" runat="server" CssClass="LabelStyle" Text="Tag Details" Font-Bold="True"></asp:Label>
                        </td>
                        <td align="left">
                            <div id="TotalDiv" runat="server">
                                <span id="taggedpagescount"></span>&nbsp of &nbsp<span id="totalpagescount"></span>
                                pages tagged
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="style2">
                            <asp:Label ID="lblDcoumentView" runat="server" CssClass="LabelStyle" Text="Document View"></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:UpdatePanel ID="UpdatePanel10" runat="server" UpdateMode="Always">
                                <ContentTemplate>
                                    <asp:DropDownList ID="ddldocumentview" runat="server" CssClass="ComboStyle" Width="160px">
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
                        <td class="style2">
                            <asp:Label ID="Label8" runat="server" CssClass="LabelStyle" Text="Main Tag"></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:DropDownList ID="cmbMainTag" runat="server" CssClass="ComboStyle" Width="160px"
                                AutoPostBack="false">
                                <asp:ListItem Value="0">---select---</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="style2">
                            <asp:Label ID="Label16" runat="server" CssClass="LabelStyle" Text="Sub Tag"></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="cmbSubTag" runat="server" CssClass="ComboStyle" Width="160px"
                                        AutoPostBack="false">
                                        <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:Button ID="btnCommonSubmitSub2" runat="server" Text="Button" OnClick="btnCommonSubmitSub2_Click"
                                        CausesValidation="False" Style="visibility: hidden" UseSubmitBehavior="false" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                    </tr>
                    <tr>
                        <td class="style2">
                            <asp:Label ID="lbltagpages" runat="server" CssClass="LabelStyle" Text="Page Numbers"></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:TextBox ID="txttagpages" runat="server" CssClass="TextBoxStyle"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Button ID="btnSaveTag" CssClass="ButtonStyle" runat="server" Text="Save Tag"
                                        TagName="Read" OnClientClick="savetagdetails('AddOrUpdateTagDetails')" />
                                    <asp:Button ID="btndeletetag" CssClass="ButtonStyle" runat="server" Text="Delete Tag"
                                        TagName="Read" OnClientClick="savetagdetails('DeleteTagDetails')" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="RemarksPanel" runat="server">
                    <tr>
                        <td class="style2">
                            <asp:Label ID="lblstatus" runat="server" CssClass="LabelStyle" Text="Document Status"></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:UpdatePanel ID="UpdatePanel11" runat="server">
                                <ContentTemplate>
                                    <asp:DropDownList ID="ddlDocStatus" runat="server" CssClass="ComboStyle" Width="160px"
                                        AutoPostBack="True" OnSelectedIndexChanged="ddlDocStatus_SelectedIndexChanged">
                                        <asp:ListItem>--Select--</asp:ListItem>
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="style2">
                            <asp:Label ID="lblStatusRemarks" runat="server" CssClass="LabelStyle" Text="Status Remarks"></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:DropDownList ID="ddlStatusRemarks" runat="server" CssClass="ComboStyle" Width="160px">
                                <asp:ListItem>--Select--</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="style2">
                            <asp:Label ID="lblRemarks" runat="server" CssClass="LabelStyle" Text="Remarks"></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:TextBox ID="txtRemarks" runat="server" CssClass="TextBoxStyle" TextMode="MultiLine"
                                Height="63px" Width="193px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="style2">
                            <asp:Label ID="lblOldRemarks" runat="server" CssClass="LabelStyle" Text="Old Remarks"></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:TextBox ID="txtoldremarks" runat="server" Style="color: #888; border-color: #FFFFFF"
                                TextMode="MultiLine" Height="63px" ReadOnly="True" Width="193px"></asp:TextBox>
                        </td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="DocumentDetails" runat="server">
                    <tr>
                        <td class="style5" colspan="2">
                            <asp:Label ID="Label14" runat="server" CssClass="LabelStyle" Text="Index Properties"
                                Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="style5" colspan="2">
                            <asp:UpdatePanel ID="Indexupdatepanel" runat="server">
                                <ContentTemplate>
                                    <asp:Panel ID="pnlIndexpro" runat="server" CssClass="outlineboxFree">
                                    </asp:Panel>
                                    <asp:Button ID="btnInedxSave" CssClass="ButtonStyle" runat="server" Text="Save Index Values"
                                        TagName="Read" OnClick="btnInedxSave_Click" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="style5" colspan="2">
                            <asp:Panel ID="pnlControls" runat="server">
                                <table cellpadding="3" cellspacing="3" class="style1">
                                    <tr>
                                        <td class="style5" colspan="2">
                                            <asp:Label ID="lblDocEdit" runat="server" CssClass="LabelStyle" Text="PDF/TIFF Doc Edit"
                                                Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style5" colspan="2">
                                            <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="Insert New Page(s)"
                                                Font-Bold="True"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style5">
                                            <asp:Label ID="Label1" runat="server" CssClass="LabelStyle" Text="Attach After Page "> </asp:Label>
                                        </td>
                                        <td class="style5">
                                            <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                                <ContentTemplate>
                                                    <asp:DropDownList ID="drpPageCount" runat="server" TagName="Edit">
                                                    </asp:DropDownList>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style5">
                                            <asp:Label ID="lblDocUpload" runat="server" CssClass="LabelStyle" Text="Upload New File to Attach"> </asp:Label>
                                        </td>
                                        <td class="style5">
                                            <asp:AsyncFileUpload ID="AsyncFileUpload1" runat="server" CssClass="LabelStyle" CompleteBackColor="Lime"
                                                ErrorBackColor="Red" ThrobberID="Throbber" OnUploadedComplete="AsyncFileUpload1_UploadedComplete"
                                                OnClientUploadStarted="disablecommit" UploadingBackColor="#66CCFF" OnClientUploadComplete="enablesave"
                                                Width="213px" EnableViewState="True" />
                                            <asp:Label ID="Throbber" runat="server" Style="display: none" TagName="Edit"> <img alt="Loading..." src="<%= Page.ResolveClientUrl("~/Images/indicator.gif")%>" /></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style5" colspan="2">
                                            <asp:Label ID="Label4" runat="server" CssClass="LabelStyle" Text="Delete Page(s)"
                                                Font-Bold="True" Visible="false"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style5">
                                            <asp:Label ID="Label6" runat="server" CssClass="LabelStyle" Text="Delete Page(s)"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                <ContentTemplate>
                                                    <asp:DropDownList ID="drpDeleteCount" runat="server" Style="height: 22px">
                                                    </asp:DropDownList>
                                                    &nbsp
                                                    <asp:Button ID="btnDeletePages" runat="server" CssClass="ButtonStyle" Text="Delete"
                                                        Enabled="false" OnClick="btnDeletePages_Click" TagName="Delete" />
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Always">
                                        <ContentTemplate>
                                            <tr>
                                                <td class="style5" colspan="2" align="center">
                                                    <asp:Button ID="btnCommitChanges" runat="server" CssClass="ButtonStyle" Text="Commit Changes"
                                                        OnClick="btnCommitChanges_Click" TagName="Upload" />
                                                    &nbsp;<asp:Button ID="btnDicardChanges" runat="server" CssClass="ButtonStyle" Text="Discard Changes"
                                                        OnClick="btnDicardChanges_Click" TagName="Upload" />
                                                </td>
                                            </tr>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </asp:Panel>
            </table>
            <%-- </asp:Panel>--%>
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
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="DivInlineBlockPage" style="float: left">
            <asp:Button ID="btnLock" runat="server" ToolTip="Edit" CssClass="image-button document-edit"
                OnClick="btnLock_Click" TagName="Edit" />
            <asp:Button ID="btnUpdate" runat="server" CssClass="image-button document-upload"
                ToolTip="Upload newer version" OnClick="btnUpdate_Click1" TagName="Upload" />
            <asp:Button ID="btnDelete" runat="server" CssClass="image-button document-delete"
                ToolTip="Delete/discard document" OnClick="btnDelete_Click1" TagName="Delete" />
            <uc1:PDFViewer ID="PDFViewer1" runat="server" />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
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
    </div>
</asp:Content>
