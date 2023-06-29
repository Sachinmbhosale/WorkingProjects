<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="DocumentDownloadSearch.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.DocumentDownloadSearch" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";
            btnFilterRow = "<%= btnFilterRow.ClientID %>";
            hdnTotalRowCount = "<%= hdnTotalRowCount.ClientID %>";
            hdnTotalRowCountOCR = "<%= hdnTotalRowCount_OCR.ClientID %>";
            tabSearch = "<%= Search.ClientID %>";
        });
        var RedirectRequired = 'No'; //DMS04-3406M
        function SetHiddenVal(param) {
            if (param == "Dynamic") {
                document.getElementById("<%= hdnDynamicControlIndexChange.ClientID %>").value = "1";
            }
            else {
                document.getElementById("<%= hdnDynamicControlIndexChange.ClientID %>").value = "0";
            }
            return true;
        };

        function CheckAll(oCheckbox) {
            var GridView1 = document.getElementById("<%=GridView1.ClientID %>");
            for (i = 1; i < GridView1.rows.length; i++) {
                GridView1.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = oCheckbox.checked;
            }
        }
        function ValidateInputData() {

            var DocumentType = document.getElementById("<%= cmbDocumentType.ClientID %>").value;

            var Department = document.getElementById("<%= cmbDepartment.ClientID %>").value;
            var msg = document.getElementById("<%= lblMsg.ClientID %>");


            if (document.getElementById("<%= cmbDocumentType.ClientID  %>").value == 0) {
                msg.innerHTML = "Please Select Branch!";
                msg.setAttribute("display", "block");
                return false;
            }
            else if (document.getElementById("<%= cmbDepartment.ClientID  %>").value == 0) {
                msg.innerHTML = "Please Select Department!";
                msg.setAttribute("display", "block");
                return false;
            }
            /* DMS04-3396 BS */
            else if (!ValidateSpaceInText()) {
                msg.innerHTML = "Please remove space from the text and try!";
                msg.setAttribute("display", "block");
                return false;
            }
            /* DMS04-3396 BE */
        }

    </script>
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId_BS.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken_BS.ClientID %>";
            pageIdContorlID = "<%= hdnPageId_BS.ClientID %>";
            CountControlsID = "<%=hdnCountControls_BS.ClientID %>";
            btnFilterRow_BS = "<%=btnFilterRow_BS.ClientID %>";
            hdnTotalRowCount_BS = "<%=hdnTotalRowCount_BS.ClientID %>";

        });



        function hideColumn_BS(col) {

            if (col == "") {
                alert("Invalid Column");
                return;
            }

            var tbl = document.getElementById("tblData");
            if (tbl != null) {

                for (var i = 0; i < tbl.rows.length; i++) {
                    for (var j = 0; j < tbl.rows[i].cells.length; j++) {
                        // tbl.rows[i].cells[j].style.display = "";

                        if (col.indexOf(j) >= 0)
                            tbl.rows[i].cells[j].style.display = "none";
                    }
                }
            }
        }

        function SetHiddenVal_BS(param) {
            if (param == "Dynamic") {
                document.getElementById("<%= hdnDynamicControlIndexChange_BS.ClientID %>").value = "1";
            }
            else {
                document.getElementById("<%= hdnDynamicControlIndexChange_BS.ClientID %>").value = "0";
            }
            return true;
        }

        //Newcode
        function createPagingIndexes_BS() {
            //*/
            //Dropdown binding
            //debugger;
            var indexCount = document.getElementById("<%= hdnTotalRowCount_BS.ClientID%>").value;

            var rowsPerPage = document.getElementById("<%= hdnRowsPerPage_BS.ClientID%>").value
            // var MaxPages = indexCount / rowsPerPage + (indexCount % rowsPerPage);
            var MaxPages = indexCount / rowsPerPage;
            MaxPages = Math.ceil(MaxPages)

            var min = 1,
                max = MaxPages,
                select = document.getElementById("<%= CurrentPage_BS.ClientID%>");

            $(select).empty();
            for (var i = min; i <= max; i++) {
                var opt = document.createElement('option');
                opt.value = i;
                opt.innerHTML = i;
                if (document.getElementById("<%= hdnPageNo_BS.ClientID%>").value == i)
                    opt.setAttribute('selected', 'selected');
                //select.add(option, 0);
                select.appendChild(opt);
            }

            var TotalPages = document.getElementById("<%= TotalPages_BS.ClientID%>");

            TotalPages.innerHTML = MaxPages;
            var DivText = document.getElementById("divRecordCountText_BS").innerHTML;
            $("#divRecordCountText_BS").html(DivText + " Record(s)");

            //Hide unnecessary columns- 1:TotalRows, 3:MainTag, 4:SubTag, 6:ProcessId, 11: Deleted
            hideColumn_BS('1,3,4,6,11');

        }

        function OnPageIndexChange_BS() {
            OnPagingIndexClick_BS(document.getElementById("<%= CurrentPage_BS.ClientID%>").value);
        }

        function OnPagingIndexClick_BS(index) {
            document.getElementById("<%= hdnPageNo_BS.ClientID%>").value = index;
            RowsPerPage = document.getElementById("<%= RowsPerPage_BS.ClientID%>").value;
            document.getElementById("<%= hdnRowsPerPage_BS.ClientID%>").value = RowsPerPage;
            SearchDocuments_BS();
        }
        //Newcode
        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
        function RearrangingControlnames_BS() {

            var Controlnames = $("#<%=hdnControlNames_BS.ClientID %>").val();


            rightsArray = new Array();
            var dataArray = Controlnames.split("#");
            if (dataArray != null) {
                if (dataArray.length > 0) {

                    for (var i = 0, j = dataArray.length; i < j; i++) {
                        var data = dataArray[i];
                        subitemArray = data.split("|")
                        //Removes this condition while debugging
                        //                        if (subitemArray.length > 6) {
                        //                            subitemArray[6] = subitemArray[6].split(",");
                        //                        }
                        rightsArray.push(subitemArray);

                    }
                }
            }


        }



        function pageLoad() {

            var msgControl = "#<%= divMsg_BS.ClientID %>";
            var SearchCriteria = getParameterByName("Search");

            if (SearchCriteria != null && SearchCriteria.length > 0) {
                //if search critearea is not a advanced search, load data
                if (!(SearchCriteria.indexOf('AdvancedSearch') >= 0)) {
                    document.getElementById("<%=hdnSearchString_BS.ClientID %>").value = SearchCriteria;
                    loginOrgIdControlID = "<%= hdnLoginOrgId_BS.ClientID %>";
                    loginTokenControlID = "<%= hdnLoginToken_BS.ClientID %>";
                    pageIdContorlID = "<%= hdnPageId_BS.ClientID %>";
                    //return CallPostScalar(msgControl, "SearchDocuments", SearchCriteria);

                }
            }

        }

        function GetDBMappingCol_BS(tmpDBCols, fieldName) {
            var DBMappingCol = "";
            for (var i = 0; i < tmpDBCols.length; i++) {
                tmpRows = tmpDBCols[i].split("|");
                if (fieldName == tmpRows[0]) {
                    DBMappingCol = tmpRows[1];
                    break;
                }
            }
            return DBMappingCol;
        }


        var WhereClause = "";
        function construct_SearchByIndexValue_Query_BS(DbField, IndexValue, SearchCondition) {

            WhereClause += " AND ( "; //( LEN('" + IndexValue + "') = 0 OR ";
            debugger;
            switch (SearchCondition.toUpperCase()) {
                case "ANYWHERE":
                    if (DbField == "UPLOAD_vFld1") {
                        //WhereClause += DbField + " IN ('" + IndexValue + "' )) ";
                        WhereClause += DbField + " like '%" + IndexValue + "%' ) ";
                    }
                    else {
                        WhereClause += DbField + " like '%" + IndexValue + "%' ) ";
                    }

                    break;
                case "STARTS WITH":
                    WhereClause += DbField + " like '" + IndexValue + "%' ) ";
                    break;
                case "ENDS WITH":
                    WhereClause += DbField + " like '%" + IndexValue + "' ) ";
                    break;
                case "NOWHERE":
                    WhereClause += DbField + " not like '%" + IndexValue + "%' ) ";
                    break;
                case "WHOLE WORD":
                    WhereClause += DbField + " = '" + IndexValue + "' ) ";
                    break;
                case "SOUNDS LIKE":
                    WhereClause += "SOUNDEX(" + DbField + ") = SOUNDEX('" + IndexValue + "') ) ";
                    break;
                default:
                    break;
            }
        }
        function Redirect_D() {
            RedirectRequired = 'Yes' //DMS04-3406M
            SearchDocuments_BS();
        }
        function SearchDocuments_BS() {
            debugger;

            //clear previous search querystring data
            $("[id$=hdnSearchString]").val("");

            RearrangingControlnames_BS();
            var msgControl = "#<%= divMsg_BS.ClientID %>";

            var DocumentType = $("#<%= cmbDocumentType_BS.ClientID %>").val();
            var Department = $("#<%= cmbDepartment_BS.ClientID %>").val();
            var Refid = $("#<%= txtRefid_BS.ClientID %>").val();
            var Keywords = $("#<%= txtKeyword_BS.ClientID %>").val();
            var count = $("#<%=hdnCountControls_BS.ClientID %>").val();
            var archive = document.getElementById("<%=chkArchive_BS.ClientID %>").checked;
            var e = document.getElementById("<%=cmbMainTag_BS.ClientID %>");
            var MainTagID = e.options[e.selectedIndex].value;
            e = document.getElementById("<%=cmbSubTag_BS.ClientID %>");
            var SubTagID = e.options[e.selectedIndex].value;
            e = document.getElementById("<%=cmbSearchOption_BS.ClientID %>");
            var SearchOption = e.options[e.selectedIndex].value;
            var PageNo = document.getElementById("<%= hdnPageNo_BS.ClientID%>").value;
            var RowsPerPage = document.getElementById("<%= hdnRowsPerPage_BS.ClientID%>").value

            var check;
            if (archive == true) {
                check = 0;
            }
            else {
                check = 1;
            }

            debugger;
            var params = SearchOption + '|' + DocumentType + '|' + Department + '|' + Refid + '|' + Keywords + '|' + check + '|' + MainTagID + '|' + SubTagID + '|' + PageNo + '|' + RowsPerPage;

            //Loop through dynamic index controls and construct SQL where clause
            WhereClause = "";
            // var arrayDBCols = $('#ctl00_ContentPlaceHolder1_Search_NormalSearch_hdnDBCOLMapping_BS').val().split('$');
            var arrayDBCols = $('#ctl00_ContentPlaceHolder2_ContentPlaceHolder1_Search_NormalSearch_hdnDBCOLMapping_BS').val().split('$');
            var data = rightsArray[0];
            for (var i = 0; i < arrayDBCols.length; i++) {

                var controlname = data[i];

                //var controlname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_Search_NormalSearch_' + controlname;
                //var controlname = 'ctl00_ContentPlaceHolder1_Search_NormalSearch_' + controlname.replace(/\s+/g, '');
                var controlname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_Search_NormalSearch_' + controlname.replace(/\s+/g, '');
                var fields = document.getElementById(controlname);
                try {

                    if (fields != null && fields.type == "text") {
                        var FieldValue = fields.value;
                        if (FieldValue == "") {
                            //Nothing
                        }
                        else {
                            // var DbField = GetDBMappingCol_BS(arrayDBCols, controlname.replace("ctl00_ContentPlaceHolder2_ContentPlaceHolder1_Search_NormalSearch_", ""));
                            //var DbField = GetDBMappingCol_BS(arrayDBCols, controlname.replace("ctl00_ContentPlaceHolder1_Search_NormalSearch_", ""));
                            var DbField = GetDBMappingCol_BS(arrayDBCols, controlname.replace("ctl00_ContentPlaceHolder2_ContentPlaceHolder1_Search_NormalSearch_", ""));
                            //Call method to construct query for SQL where condition from dynamic controls

                            if (SearchOption.toUpperCase() == "ANYWHERE") {
                                if (DbField == "UPLOAD_vFld1") {
                                    FieldValue = FieldValue.replace(/,/g, "','");
                                }
                            }

                            construct_SearchByIndexValue_Query_BS(DbField, FieldValue, SearchOption);
                        }
                    }

                    else if (fields != null && fields.type == "select-one") {
                        if (fields.options[fields.selectedIndex].text == "--Select--") {
                            //Nothing
                        }
                        else {
                            // var DbField = GetDBMappingCol_BS(arrayDBCols, controlname.replace("ctl00_ContentPlaceHolder2_ContentPlaceHolder1_Search_NormalSearch_", ""));
                            //  var DbField = GetDBMappingCol_BS(arrayDBCols, controlname.replace("ctl00_ContentPlaceHolder1_Search_NormalSearch_", ""));
                            var DbField = GetDBMappingCol_BS(arrayDBCols, controlname.replace("ctl00_ContentPlaceHolder2_ContentPlaceHolder1_Search_NormalSearch_", ""));
                            var FieldValue = fields.options[fields.selectedIndex].text;
                            //Call method to construct query for SQL where condition from dynamic controls
                            construct_SearchByIndexValue_Query_BS(DbField, FieldValue, SearchOption);
                        }
                    }
                }
                catch (ex) {
                }

            }


            // Append dynamic fields whereclause to data
            $("#<%=hdnSearchString_BS.ClientID %>").val(WhereClause);
            params += "|" + WhereClause;
            //alert(params);
            if (ValidateInputData_BS(msgControl, "SearchDocuments_BS")) {
                $("#divSearchResults_BS").html("");
                document.getElementById("<%=hdnSearchString_BS.ClientID %>").value = params;
                if (RedirectRequired != 'Yes') { //DMS04-3406M
                    $("#<%=btnSubmit.ClientID %>").click();

                    //return CallPostScalar(msgControl, "SearchDocuments", params);
                }
                else {
                    RedirectRequired = 'No'; //DMS04-3406M
                    window.location.href = "DigitalSignature.aspx?Action=" + 'SearchDocuments' + '&parms=' + params;
                }
            }
        }



        function ValidateInputData_BS(msgControl, action) {
            $(msgControl).html("");
            var retval = true;
            var DocumentType = $("#<%= cmbDocumentType_BS.ClientID %>").val();
            var Department = $("#<%= cmbDepartment_BS.ClientID %>").val();
            if (action == "SearchDocuments_BS") {
                if (document.getElementById("<%= cmbDocumentType_BS.ClientID  %>").value == 0) {
                    $(msgControl).html("Please Select Branch!");
                    retval = false;
                }
                else if (document.getElementById("<%= cmbDepartment_BS.ClientID  %>").value == 0) {
                    $(msgControl).html("Please Select Department!");
                    retval = false;
                }
                return retval;
            }
        }


        function ValidateOCR() {
            var msgControl = "#<%= divMsg_OCR.ClientID %>";
            var DocumentType = $("#<%= cmbDocumentType_OCR.ClientID %>").val();
            var Department = $("#<%= cmbDepartment_OCR.ClientID %>").val();
            if (DocumentType == 0) {
                $(msgControl).html("Please Select Branch!");
                return false;
            }
            else if (Department == 0) {
                $(msgControl).html("Please Select Department!");
                return false;
            }


        }
        function ValidateBS() {

            var msgControl = "#<%= divMsg_BS.ClientID %>";
            var DocumentType = $("#<%= cmbDocumentType_BS.ClientID %>").val();
            var Department = $("#<%= cmbDepartment_BS.ClientID %>").val();
            if (DocumentType == 0) {
                $(msgControl).html("Please Select Project Type!");
                return false;
            }
            else if (Department == 0) {
                $(msgControl).html("Please Select Department!");
                return false;
            }


        }
        function LoadSub_BS(ID) {
            var e = document.getElementById(ID.id);
            var drop = e.options[e.selectedIndex].value;
            if (drop != "0" && drop != "<Select>") {
                var drpID = document.getElementById("<%=hdnSubDrpID_BS.ClientID %>");
                drpID.value = drop;
                (document.getElementById('<%=btnCommonSubmitSub_BS.ClientID%>')).click();
            }
            else {
                var seconddroplst = ID.id.substring(ID.id.length - 1, ID.id.length);
                var seconddroplast = ID.id.substring(0, ID.id.length - 1);
                // document.getElementById(seconddroplst + "drp_" + seconddroplast).options.length = 1;
            }
            return false;
        }

        function LoadSubTag_BS(ID) {
            debugger;
            var e = document.getElementById(ID.id);
            var drop = e.options[e.selectedIndex].value;
            if (drop != "0" && drop != "<Select>") {
                (document.getElementById('<%=btnCommonSubmitSub2_BS.ClientID%>')).click();
            }
            return false;
        }

        /* DMS04-3396 BS */
        function CheckSpace(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if ((charCode == 32)) {

                return false;
            }
        }
        function ValidateSpaceInText() {
            var isvalidate = true
            if (document.getElementById("<%= cbxFullText.ClientID %>").checked == true) {
                var txtId = document.getElementById("<%= hdnSearchTextboxId.ClientID %>").value;
                var dataArray = txtId.split("|");
                if (dataArray != null) {
                    if (dataArray.length > 0) {

                        for (var i = 0, j = dataArray.length; i < j; i++) {
                            var data = dataArray[i];
                            var controlname = 'ctl00_ContentPlaceHolder1_Search_AdvancedSearch_' + data;
                            var Index = document.getElementById(controlname);
                            if (Index != null && Index.value.match(/\s/g)) {

                                isvalidate = false;
                            }
                        }

                    }

                }



            }
            return isvalidate;
        }

        /* DMS04-3396 BE */

    </script>
    <style type="text/css">
        .auto-style1 {
            width: 125px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Document Download Search"></asp:Label>--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="GVDiv">
                <asp:TabContainer runat="Server" ID="Search" ActiveTabIndex="0" CssClass="ajax__myTab"
                    OnActiveTabChanged="Search_ActiveTabChanged">
                    <asp:TabPanel ID="NormalSearch" runat="server">
                        <HeaderTemplate>
                            Document Search
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel ID="UpdatePanel2_BS" runat="server">
                                <ContentTemplate>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="DivInlineBlock">
                                <asp:UpdatePanel ID="UpdatePanel1_BS" runat="server">
                                    <ContentTemplate>
                                        <table>
                                            <tr>
                                                <td style="width: 120px">
                                                    <asp:Label ID="Label3_BS" runat="server" CssClass="LabelStyle" Text='Branch'></asp:Label>
                                                    &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="lblPageDescription1_BS"
                                                        runat="server" Text="*"></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:DropDownList ID="cmbDocumentType_BS" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDocumentType_SelectedIndexChanged_BS">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label5_BS" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                                                    &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label1_BS" runat="server"
                                                        Text="*"></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:DropDownList ID="cmbDepartment_BS" runat="server" AutoPostBack="True" OnSelectedIndexChanged="btnCommonSubmitSub_Click_BS">
                                                        <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>

                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="Label9_BS" runat="server" CssClass="LabelStyle" Text="Document ID"></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtRefid_BS" runat="server" Style="margin-left: 0px;"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="Label12_BS" runat="server" CssClass="LabelStyle" Text="Keywords "></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtKeyword_BS" runat="server" Style="margin-left: 0px;"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="Label15_BS" runat="server" CssClass="LabelStyle" Text="Main Tags"></asp:Label>
                                                </td>
                                                <td class="style2" colspan="3">
                                                    <asp:DropDownList ID="cmbMainTag_BS" runat="server" AutoPostBack="false">
                                                        <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="Label16_BS" runat="server" CssClass="LabelStyle" Text="Sub Tags"></asp:Label>
                                                </td>
                                                <td class="style2" colspan="3">
                                                    <asp:DropDownList ID="cmbSubTag_BS" runat="server" AutoPostBack="false">
                                                        <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="lblSearchType_BS" runat="server" CssClass="LabelStyle" Text="Search Option"></asp:Label>
                                                </td>
                                                <td class="style2" colspan="3">
                                                    <asp:DropDownList ID="cmbSearchOption_BS" runat="server" AutoPostBack="false">
                                                        <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                        <table style="width: 100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblIndexProprties_BS" runat="server" CssClass="LabelStyle" Text="Index Properties"><span>Index Properties</span><br /></span></asp:Label>
                                                </td>
                                                <td colspan="1"></td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <div style="height: 400px; width: 350px; overflow-y: scroll">
                                                        <asp:UpdatePanel ID="Indexupdatepanel_BS" runat="server">
                                                            <ContentTemplate>
                                                                <asp:Panel ID="pnlIndexpro_BS" runat="server">
                                                                </asp:Panel>
                                                                <asp:Button ID="btnCommonSubmitSub_BS" runat="server" Text="Button" OnClick="btnCommonSubmitSub_Click_BS"
                                                                    CausesValidation="False" Style="visibility: hidden" />
                                                                <asp:Button ID="btnCommonSubmitSub2_BS" runat="server" Text="Button" OnClick="btnCommonSubmitSub2_Click_BS"
                                                                    CausesValidation="False" Style="visibility: hidden" />
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <table>
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="lblArchive_BS" runat="server" CssClass="LabelStyle" Text="Include deleted documents"></asp:Label>
                                                </td>
                                                <td class="auto-style1">
                                                    <asp:CheckBox ID="chkArchive_BS" CssClass="LabelStyle" runat="server" TagName="Read" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <br />
                                                    <asp:Button ID="btnSubmit" runat="server" CssClass="btnsearch" OnClick="btnSubmit_Click"
                                                        Text="Search" OnClientClick="return ValidateBS();" Style="display: none" />


                                                    <input type="button" id="Search_BS" value="Search" class="btnsearch" onclick="SearchDocuments_BS();"
                                                        tagname="Read" />
                                                    <%--<asp:ImageButton ID="btnExcelDownload" runat="server" ImageUrl="~/Images/excel.png" OnClick="btnExcelDownload_Click" Style="height: 30px" Width="30px" />--%>

                                                    <input type="button" id="btnDigtialSignature" value="Digital Signature" class="DigitalSign"
                                                        onclick="Redirect_D();" tagname="Read" style="display: none" />

                                                    <asp:Button ID="btnGlobalsearch" runat="server" CssClass="btnsearch"
                                                        OnClientClick="return  ValidateGS();" Style="display: none" Text="Global Search" Width="110px" OnClick="btnGlobalsearch_Click" />

                                                    <asp:HiddenField ID="hdnLoginOrgId_BS" runat="server" Value="" />
                                                    <asp:HiddenField ID="hdnLoginToken_BS" runat="server" Value="" />
                                                    <asp:HiddenField ID="hdnPageId_BS" runat="server" Value="" />
                                                    <asp:HiddenField ID="hdnAction_BS" runat="server" Value="" />
                                                    <asp:HiddenField ID="hdnCountControls_BS" runat="server" Value="" />
                                                    <asp:HiddenField ID="hdnSearchString_BS" runat="server" Value="" />
                                                    <asp:HiddenField ID="hdnDBCOLMapping_BS" runat="server" Value="" />
                                                    <asp:UpdatePanel ID="UpdatePanel8_BS" runat="server">
                                                        <ContentTemplate>
                                                            <asp:HiddenField ID="hdnIndexNames_BS" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnIndexMinLen_BS" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnIndexType_BS" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnIndexDataType_BS" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnSubDrpID_BS" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnControlNames_BS" runat="server" Value="" />
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:HiddenField ID="hdnDynamicControlIndexChange_BS" runat="server" Value="0" />
                            </div>


                            <div class="DivInlineBlock" style="width: 70%">
                                <div style="overflow: auto;">
                                    <table width="100%" border="0" align="center" class="generalBox">

                                        <tr>
                                            <td>


                                                <center>
                                                    <div id="divSearchResults_BS">
                                                        <div style="height: 500px; width: auto; overflow-y: scroll; overflow-x: scroll">
                                                            <asp:GridView ID="GridView1" runat="server"
                                                                OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging"
                                                                CssClass="mGrid" AllowSorting="True" OnRowCreated="GridView1_RowCreated" PageSize="100">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="View">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/Images/view.png" OnClick="ImageButton2_Click" Style="display: none" />
                                                                            <asp:HiddenField ID="Hidview" runat="server" Value='<%# Eval("View") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Download">
                                                                        <ItemTemplate>
                                                                            <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/Images/Downloadwithann.png" OnClick="ImageButton3_Click" />
                                                                            <asp:HiddenField ID="Hiddownload" runat="server" Value='<%# Eval("UPLOAD_vDocPhysicalPath") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                           
                                                        </div>
                                                         <div style="text-align: left;">

                                                                <asp:ImageButton ID="btnExcelDownload" runat="server" ImageUrl="~/Images/excel.png" OnClick="btnExcelDownload_Click" Style="height: 30px" Width="30px" />

                                                            </div>



                                                    </div>
                                                </center>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="8" align="left">
                                                <div id="divRecordCountText_BS" style="color: green; visibility: hidden;">
                                                    &nbsp;
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="8" align="left">
                                                <div id="divPagingText_BS">
                                                    &nbsp;
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div style="text-align: left; position: relative; width: -120%;">
                                    <asp:Label ID="lbltotalrowcount" runat="server"></asp:Label>
                                    &nbsp; &nbsp; &nbsp; &nbsp;
                                                    <asp:Label ID="lbltotalpagecount" runat="server"></asp:Label>
                                </div>
                                <div id="paging_BS" style="visibility: hidden;">
                                    <a>Rows per page
                                                        <select id="RowsPerPage_BS" runat="server" style="width: 50px" onchange="OnPagingIndexClick_BS(1);">
                                                        </select>
                                        Current Page
                                                        <select id="CurrentPage_BS" runat="server" style="width: 50px" onchange="OnPageIndexChange_BS();">
                                                        </select>
                                        of
                                                        <asp:Label ID="TotalPages_BS" runat="server" CssClass="LabelStyle"></asp:Label>
                                        <asp:HiddenField ID="hdnPageNo_BS" Value="1" runat="server" />
                                        <asp:HiddenField ID="hdnSortColumn_BS" Value="ASC" runat="server" />
                                        <asp:HiddenField ID="hdnSortOrder_BS" runat="server" />
                                        <asp:HiddenField ID="hdnTotalRowCount_BS" runat="server" />
                                        <asp:HiddenField ID="hdnRowsPerPage_BS" Value="10" runat="server" />
                                        <asp:Button ID="btnFilterRow_BS" runat="server" Height="25px" class="HiddenButton"
                                            Text="&lt;&lt; Remove" OnClientClick="createPagingIndexes_BS(); return false;"
                                            TagName="Read" />
                                    </a>

                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:TabPanel>

                    <%--                            <div class="DivInlineBlock" style="width: 900px; overflow: auto; margin-top: -70px">
                                <div>
                                    <table border="0" align="center" class="generalBox">
                                        <tr>
                                            <td height="8" align="left">
                                                <div id="divRecordCountText_BS" style="color: green; visibility: hidden;">
                                                    &nbsp;
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="8">
                                                <div id="paging_BS" style="visibility: hidden;">
                                                    <a>Rows per page
                                                        <select id="RowsPerPage_BS" runat="server" style="width: 50px" onchange="OnPagingIndexClick_BS(1);">
                                                        </select>
                                                        Current Page
                                                        <select id="CurrentPage_BS" runat="server" style="width: 50px" onchange="OnPageIndexChange_BS();">
                                                        </select>
                                                        of
                                                        <asp:Label ID="TotalPages_BS" runat="server" CssClass="LabelStyle"></asp:Label>
                                                        <asp:HiddenField ID="hdnPageNo_BS" Value="1" runat="server" />
                                                        <asp:HiddenField ID="hdnSortColumn_BS" Value="ASC" runat="server" />
                                                        <asp:HiddenField ID="hdnSortOrder_BS" runat="server" />
                                                        <asp:HiddenField ID="hdnTotalRowCount_BS" runat="server" />
                                                        <asp:HiddenField ID="hdnRowsPerPage_BS" Value="10" runat="server" />
                                                        <asp:Button ID="btnFilterRow_BS" runat="server" Height="25px" class="HiddenButton"
                                                            Text="&lt;&lt; Remove" OnClientClick="createPagingIndexes_BS(); return false;"
                                                            TagName="Read" />
                                                    </a>

                                                </div>
                                                <center style="overflow: auto">
                                                    <div id="divSearchResults_BS" style="width: 1000px; overflow: auto">
                                                        <asp:GridView ID="GridView1" runat="server" CellPadding="5" ForeColor="#333333" GridLines="None"
                                                            OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging"
                                                            CssClass="mGrid">
                                                        </asp:GridView>
                                                    </div>
                                                </center>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="8" align="left">
                                                <div id="divPagingText_BS">
                                                    &nbsp;
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:TabPanel>--%>
                    <asp:TabPanel ID="AdvancedSearch" runat="server">
                        <HeaderTemplate>
                            OCR Search
                        </HeaderTemplate>
                        <ContentTemplate>
                            <div id="divResultMsg">
                            </div>
                            <div id="divMsg" runat="server" style="color: red;">
                            </div>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblMsg" ForeColor="Red" runat="server"></asp:Label>
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td valign="top">
                                                    <table cellpadding="3" cellspacing="3" class="style1" border="0">
                                                        <tr>
                                                            <td style="width: 120px">
                                                                <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="Project Type"></asp:Label>
                                                                &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="lblPageDescription1" runat="server"
                                                                    Text="*"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="cmbDocumentType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDocumentType_SelectedIndexChanged">
                                                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label5" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                                                                &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label1" runat="server" Text="*"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="cmbDepartment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDepartment_SelectedIndexChanged">
                                                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <asp:Panel ID="pnlQuery" runat="server">
                                                        <table cellpadding="3" cellspacing="3" class="style1" border="0">
                                                            <tr>
                                                                <td style="width: 120px">
                                                                    <asp:Label ID="lblQuery" runat="server" CssClass="LabelStyle" Text="Query"></asp:Label>
                                                                </td>
                                                                <td colspan="3">
                                                                    <asp:DropDownList ID="dropQueries" runat="server" AutoPostBack="True" OnSelectedIndexChanged="dropQueries_SelectedIndexChanged">
                                                                        <asp:ListItem Value="0">--Select--</asp:ListItem>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>
                                                                    <asp:Button ID="btnDeleteQuery" runat="server" class="clearbutton" Width="100px"
                                                                        OnClientClick="return confirm('Are you sure you want to delete selected query?')"
                                                                        meta:resourcekey="btnDeleteQuery" OnClick="DeleteQuery" Text="Delete Query" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="Label4" runat="server" CssClass="LabelStyle" Text="Enable Full Text"></asp:Label>
                                                                </td>
                                                                <td colspan="3">
                                                                    <asp:CheckBox ID="cbxFullText" runat="server" OnCheckedChanged="AddQuery" AutoPostBack="True" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <div style="margin-top: 10px; margin-bottom: 10px;">
                                                            <asp:Button Text="Edit Query" class="" runat="server" ID="EditQueryButton" OnClick="EditQuery"
                                                                Visible="False" />
                                                        </div>
                                                        <asp:Label ID="lblError" ForeColor="Red" EnableViewState="False" runat="server" />
                                                        <asp:Panel ID="Results" runat="server">
                                                            <table>
                                                                <tr>
                                                                    <td class="contentCell">
                                                                        <h1>
                                                                            <asp:Literal ID="Literal1" runat="server" meta:resourcekey="Results" /></h1>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </asp:Panel>
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnlIndexDropdown" runat="server" Width="500px">
                                                        <table width="100%">
                                                            <tr>
                                                                <td>
                                                                    <asp:Label ID="lblIndexProprties" runat="server" CssClass="LabelStyle" Text="Search Criteria"></asp:Label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <div id="divdrops">
                                                                        <asp:Panel ID="pnlIndexpro" runat="server">
                                                                            <div style="width: 600px; overflow: auto;">
                                                                                <table>
                                                                                    <tr>
                                                                                        <td>
                                                                                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                                                                <ContentTemplate>
                                                                                                    <asp:PlaceHolder ID="plhClauses" runat="Server" />
                                                                                                </ContentTemplate>
                                                                                            </asp:UpdatePanel>
                                                                                        </td>
                                                                                    </tr>
                                                                                </table>
                                                                            </div>
                                                                            <br />
                                                                            <asp:Button ID="btnAddQuery" runat="server" class="btnadd" meta:resourcekey="btnAddQuery"
                                                                                Width="100px" OnClick="AddQuery" Text="New / Clear" ToolTip="New or clear query"
                                                                                OnClientClick="return SetPageIndex(1);" />
                                                                            <asp:Button ID="btnAddClause" Text="Add Clause" meta:resourcekey="btnAddClause" class="btnadd"
                                                                                Width="100px" runat="server" OnClick="btnAddClauseClick" OnClientClick="return SetPageIndex(1);" />
                                                                            <asp:Button ID="btnRemoveClause" Text="Remove Clause" class="removebutton" meta:resourcekey="btnRemoveClause"
                                                                                Width="115px" runat="server" OnClick="btnRemoveClauseClick" OnClientClick="return SetPageIndex(1);" />
                                                                            <asp:Button ID="btnSearch" runat="server" class="btnsearch" OnClick="btnPerformQueryClick"
                                                                                OnClientClick="return ValidateInputData();" Text="Search" />
                                                                            <div class="fieldgroup">
                                                                                <asp:Panel ID="SaveQuery" runat="server">
                                                                                    <br />
                                                                                    <table cellpadding="3" cellspacing="3" class="style1" border="0">
                                                                                        <tr>
                                                                                            <td style="width: 120px">
                                                                                                <asp:Label ID="Label2" runat="server" Text="Query Name" CssClass="LabelStyle" AssociatedControlID="txtQueryName" />
                                                                                            </td>
                                                                                            <td colspan="3">
                                                                                                <asp:TextBox ID="txtQueryName" runat="server" />
                                                                                                <asp:Button ID="btnSaveQuery" runat="server" class="btnsave" meta:resourcekey="btnSaveQuery"
                                                                                                    Width="100px" OnClick="btnSaveQueryClick" Text="Save Query" />
                                                                                            </td>
                                                                                        </tr>
                                                                                        <tr style="visibility: hidden">
                                                                                            <td style="width: 120px">
                                                                                                <asp:Label ID="Label7" runat="server" CssClass="LabelStyle" Text="Public Query:"
                                                                                                    meta:resourcekey="chkGlobalQuery" AssociatedControlID="chkGlobalQuery" />
                                                                                            </td>
                                                                                            <td colspan="3">
                                                                                                <asp:CheckBox ID="chkGlobalQuery" runat="server" />
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </asp:Panel>
                                                                                <div class="submit">
                                                                                    <asp:Button ID="btnPerformQuery" Text="Search" meta:resourcekey="btnPerformQuery"
                                                                                        class="btnsearch" runat="server" OnClick="btnPerformQueryClick" Visible="False" />
                                                                                </div>
                                                                            </div>
                                                                        </asp:Panel>
                                                                        <asp:Button ID="btnCommonSubmitSub" runat="server" Text="Button" CausesValidation="False"
                                                                            Style="visibility: hidden" />
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                                <td rowspan="5" style="padding-left: 8px;" valign="top">
                                                    <div>
                                                        <table>
                                                            <tr>
                                                                <td align="left">
                                                                    <div id="divRecordCountText" runat="server">
                                                                        &nbsp;
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <div id="paging" runat="server" style="visibility: hidden;">
                                                                        <a>Rows per page
                                                                            <select id="RowsPerPage" runat="server" class="input-mini" onchange="OnPagingIndexClick(1);"
                                                                                style="width: 50px;">
                                                                                <option>10</option>
                                                                                <option>20</option>
                                                                                <option>30</option>
                                                                                <option>40</option>
                                                                                <option>50</option>
                                                                            </select>
                                                                            Current Page
                                                                            <select id="CurrentPage" runat="server" class="input-mini" onchange="OnPageIndexChange();"
                                                                                style="width: 50px;">
                                                                            </select>
                                                                            of
                                                                            <asp:Label ID="TotalPages" runat="server" CssClass="LabelStyle"></asp:Label>
                                                                            <asp:HiddenField ID="hdnPageNo" Value="1" runat="server" />
                                                                            <asp:HiddenField ID="hdnSortColumn" Value="ASC" runat="server" />
                                                                            <asp:HiddenField ID="hdnSortOrder" runat="server" />
                                                                            <asp:HiddenField ID="hdnTotalRowCount" runat="server" />
                                                                            <asp:HiddenField ID="hdnRowsPerPage" Value="10" runat="server" />
                                                                            <asp:Button ID="btnFilterRow" runat="server" Height="25px" class="HiddenButton" Text="&lt;&lt; Remove"
                                                                                TagName="Read" />
                                                                        </a>
                                                                    </div>
                                                                    <div id="divSearchResultsOuter">
                                                                        <center>
                                                                            <div id="divSearchResults" runat="server" style="width: 750px; overflow: auto;">
                                                                                &nbsp;
                                                                            </div>
                                                                        </center>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td height="8" align="left">
                                                                    <div id="divPagingText" runat="server">
                                                                        &nbsp;
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:Button ID="btnSearchAgain" runat="server" Text="SearchAgain" class="HiddenButton"
                                                                        OnClick="btnSearchAgain_Click" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:HiddenField ID="hdnUCControlValues" runat="server" />
                                                                    <asp:HiddenField ID="hdnUCControlNames" runat="server" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <br />
                                                                <asp:HiddenField ID="hdnLoginOrgId" runat="server" />
                                                                <asp:HiddenField ID="hdnLoginToken" runat="server" />
                                                                <asp:HiddenField ID="hdnPageId" runat="server" />
                                                                <asp:HiddenField ID="hdnAction" runat="server" />
                                                                <asp:HiddenField ID="hdnSearchString" runat="server" />
                                                                <asp:HiddenField ID="hdnDBCOLMapping" runat="server" />
                                                                <asp:HiddenField ID="hdnIndexNames" runat="server" />
                                                                <asp:HiddenField ID="hdnIndexMinLen" runat="server" />
                                                                <asp:HiddenField ID="hdnIndexType" runat="server" />
                                                                <asp:HiddenField ID="hdnIndexDataType" runat="server" />
                                                                <asp:HiddenField ID="hdnSubDrpID" runat="server" />
                                                                <asp:HiddenField ID="hdnSearchTextboxId" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:HiddenField ID="hdnDynamicControlIndexChange" runat="server" Value="0" />
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="OCRSearch" runat="server" Visible="false">
                        <HeaderTemplate>
                            OCR Search
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel ID="UpdatePanel2_OCR" runat="server">
                                <ContentTemplate>
                                    <div id="divMsg_OCR" runat="server" style="color: Red">
                                        &nbsp;
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="DivInlineBlock">
                                <asp:UpdatePanel ID="UpdatePanel1_OCR" runat="server">
                                    <ContentTemplate>
                                        <table>
                                            <tr>
                                                <td style="width: 120px">
                                                    <asp:Label ID="Label3_OCR" runat="server" CssClass="LabelStyle" Text="Project Type"></asp:Label>
                                                    &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="lblPageDescription1_OCR"
                                                        runat="server" Text="*"></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:DropDownList ID="cmbDocumentType_OCR" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDocumentType_SelectedIndexChanged_BS">
                                                        <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label5_OCR" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                                                    &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label1_OCR" runat="server"
                                                        Text="*"></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:DropDownList ID="cmbDepartment_OCR" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDepartment_SelectedIndexChanged_BS">
                                                        <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="Label9_OCR" runat="server" CssClass="LabelStyle" Text="Ref ID"></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtRefid_OCR" runat="server" Style="margin-left: 0px;"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="Label12_OCR" runat="server" CssClass="LabelStyle" Text="Keywords "></asp:Label>
                                                </td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtKeyword_OCR" runat="server" Style="margin-left: 0px;"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="Label15_OCR" runat="server" CssClass="LabelStyle" Text="Main Tags"></asp:Label>
                                                </td>
                                                <td class="style2" colspan="3">
                                                    <asp:DropDownList ID="cmbMainTag_OCR" runat="server" AutoPostBack="false">
                                                        <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="Label16_OCR" runat="server" CssClass="LabelStyle" Text="Sub Tags"></asp:Label>
                                                </td>
                                                <td class="style2" colspan="3">
                                                    <asp:DropDownList ID="cmbSubTag_OCR" runat="server" AutoPostBack="false">
                                                        <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr style="display: none">
                                                <td>
                                                    <asp:Label ID="lblSearchType_OCR" runat="server" CssClass="LabelStyle" Text="Search Option"></asp:Label>
                                                </td>
                                                <td class="style2" colspan="3">
                                                    <asp:DropDownList ID="cmbSearchOption_OCR" runat="server" AutoPostBack="false">
                                                        <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblIndexProprties_OCR" runat="server" CssClass="LabelStyle" Text="Index Properties"></asp:Label>
                                                </td>
                                                <td colspan="3"></td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <div style="height: 400px; width: 500px; overflow-y: scroll">
                                                        <asp:UpdatePanel ID="Indexupdatepanel_OCR" runat="server">
                                                            <ContentTemplate>
                                                                <asp:Panel ID="pnlIndexpro_OCR" runat="server">
                                                                </asp:Panel>
                                                                <asp:Button ID="btnCommonSubmitSub_OCR" runat="server" Text="Button" OnClick="btnCommonSubmitSub_Click_BS"
                                                                    CausesValidation="False" Style="visibility: hidden" />
                                                                <asp:Button ID="btnCommonSubmitSub2_OCR" runat="server" Text="Button" OnClick="btnCommonSubmitSub2_Click_BS"
                                                                    CausesValidation="False" Style="visibility: hidden" />
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lblArchive_OCR" runat="server" CssClass="LabelStyle" Text="Include deleted documents"></asp:Label>
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="chkArchive_OCR" CssClass="LabelStyle" runat="server" TagName="Read" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="2">
                                                    <br />
                                                    <asp:Button ID="btnSearchOCR" runat="server" class="btnsearch" OnClick="btnPerformQueryClick"
                                                        OnClientClick="return ValidateOCR();" Text="Search" />
                                                    <%-- <asp:Button ID="Button2" runat="server" class="btnsearch" OnClick="btnPerformQueryClick"
                                                        OnClientClick="return ValidateOCR();" Text="Search" />
                                                    <input type="button" id="Button1" value="Digital Signature" class="DigitalSign" onclick="Redirect_D();"
                                                        tagname="Read" />--%>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:HiddenField ID="hdnDynamicControlIndexChange_OCR" runat="server" Value="0" />
                            </div>
                            <div class="DivInlineBlock">
                                <div style="width: 556px; overflow: auto" runat="server" id="OCRGridContents">
                                    <table width="550" border="0" align="center" class="generalBox">
                                        <tr>
                                            <td height="8" align="left">
                                                <div id="DivRecordsOCR">
                                                    <center>
                                                        <div id="divRecordCountText_OCR" runat="server" style="width: 550px; overflow: auto;">
                                                            &nbsp;
                                                        </div>
                                                    </center>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="8">
                                                <div id="paging_OCR" runat="server" style="visibility: hidden;">
                                                    <a>Rows per page
                                                        <select id="RowsPerPage_OCR" runat="server" class="input-mini" onchange="OnPagingIndexClick(1);"
                                                            style="width: 50px;">
                                                            <option>10</option>
                                                            <option>20</option>
                                                            <option>30</option>
                                                            <option>40</option>
                                                            <option>50</option>
                                                        </select>
                                                        Current Page
                                                        <select id="CurrentPage_OCR" runat="server" class="input-mini" onchange="OnPageIndexChange();"
                                                            style="width: 50px;">
                                                        </select>
                                                        of
                                                        <asp:Label ID="TotalPages_OCR" runat="server" CssClass="LabelStyle"></asp:Label>
                                                        <asp:HiddenField ID="hdnPageNo_OCR" Value="1" runat="server" />
                                                        <asp:HiddenField ID="hdnSortColumn_OCR" Value="ASC" runat="server" />
                                                        <asp:HiddenField ID="hdnSortOrder_OCR" runat="server" />
                                                        <asp:HiddenField ID="hdnTotalRowCount_OCR" runat="server" />
                                                        <asp:HiddenField ID="hdnRowsPerPage_OCR" Value="10" runat="server" />
                                                        <asp:Button ID="btnFilterRow_OCR" runat="server" Height="25px" class="HiddenButton"
                                                            Text="&lt;&lt; Remove" TagName="Read" />
                                                    </a>
                                                </div>
                                                <center>
                                                    <asp:GridView ID="GridOCR" runat="server" CssClass="mGrid"
                                                        OnRowDataBound="GridOCR_RowDataBound" EmptyDataText="No records are found">
                                                        <AlternatingRowStyle CssClass="alt" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="View">
                                                                <ItemTemplate>
                                                                    <asp:ImageButton ID="imgOCRView" ImageUrl="../../Assets/Skin/Images/view.png" runat="server"
                                                                        OnClick="OCRView" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <PagerStyle CssClass="pgr" />
                                                    </asp:GridView>
                                                </center>
                                                <%--<center style="overflow:auto">
                                                    <div id="div1" style="width:1000px;overflow:auto">
                                                        <asp:GridView ID="GridView2" runat="server" CellPadding="5" ForeColor="#333333" GridLines="None"
                                                            OnRowDataBound="GridView1_RowDataBound" AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging"
                                                            CssClass="mGrid">
                                                        </asp:GridView>
                                                    </div>
                                                </center>--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td height="8" align="left">
                                                <div id="divPagingText_OCR" runat="server">
                                                    &nbsp;
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:TabPanel>
                </asp:TabContainer>
            </div>

            <div id="divMsg_BS" runat="server" style="color: Red">
                &nbsp;
            </div>
            <asp:HiddenField ID="hdnPageRights" Value="10" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>


    <!-- Script for hiding table cells/columns -->
    <script type="text/javascript" language="javascript">
        function hideColumn(col) {

            if (col == "") {
                alert("Invalid Column");
                return;
            }

            var tbl = document.getElementById("tblData");
            if (tbl != null) {

                for (var i = 0; i < tbl.rows.length; i++) {
                    for (var j = 0; j < tbl.rows[i].cells.length; j++) {
                        // tbl.rows[i].cells[j].style.display = "";

                        if (col.indexOf(j) >= 0)
                            tbl.rows[i].cells[j].style.display = "none";
                    }
                }
            }
        }
    </script>
    <!-- Script for paging -->
    <script type="text/javascript" language="javascript">

        function createPagingIndexes() {
            //*/
            //Dropdown binding

            var indexCountObj = document.getElementById("<%= hdnTotalRowCount.ClientID%>")

            if (indexCountObj != null) {
                var indexCount = indexCountObj.value;

                var rowsPerPage = document.getElementById("<%= hdnRowsPerPage.ClientID%>").value
                // var MaxPages = indexCount / rowsPerPage + (indexCount % rowsPerPage);
                var MaxPages = indexCount / rowsPerPage;
                MaxPages = Math.ceil(MaxPages)

                var min = 1,
                    max = MaxPages,
                    select = document.getElementById("<%= CurrentPage.ClientID%>");

                $(select).empty();
                for (var i = min; i <= max; i++) {
                    var opt = document.createElement('option');
                    opt.value = i;
                    opt.innerHTML = i;
                    if (document.getElementById("<%= hdnPageNo.ClientID%>").value == i)
                        opt.setAttribute('selected', 'selected');
                    //select.add(option, 0);
                    select.appendChild(opt);
                }


                var TotalPages = document.getElementById("<%= TotalPages.ClientID%>");

                //document.getElementById("<%= TotalPages.ClientID%>").value = MaxPages;
                //MaxPages = Math.round(MaxPages);
                TotalPages.innerHTML = MaxPages;

                var DivText = document.getElementById("<%=divRecordCountText.ClientID%>").innerHTML;
                $("<%=divRecordCountText.ClientID %>").html(DivText + " Record(s)");

                //Hide unnecessary columns- 1:TotalRows, 3:MainTag, 4:SubTag, 6:ProcessId, 11: Deleted
                hideColumn_BS('1,3,4,6,11');
            }

            var indexCountObjOCR = document.getElementById("<%= hdnTotalRowCount_OCR.ClientID%>")
            if (indexCountObjOCR != null) {
                var indexCountocr = indexCountObjOCR.value;

                var rowsPerPageocr = document.getElementById("<%= hdnRowsPerPage_OCR.ClientID%>").value
                // var MaxPages = indexCount / rowsPerPage + (indexCount % rowsPerPage);
                var MaxPagesocr = indexCountocr / rowsPerPageocr;
                MaxPagesocr = Math.ceil(MaxPagesocr)

                var minocr = 1,
                    maxocr = MaxPagesocr,
                    selectOcr = document.getElementById("<%= CurrentPage_OCR.ClientID%>");

                //for ocr paging
                $(selectOcr).empty();
                for (var i = minocr; i <= maxocr; i++) {
                    var optOcr = document.createElement('option');
                    optOcr.value = i;
                    optOcr.innerHTML = i;
                    if (document.getElementById("<%= hdnPageNo_OCR.ClientID%>").value == i)
                        optOcr.setAttribute('selected', 'selected');
                    //select.add(option, 0);
                    selectOcr.appendChild(optOcr);
                }


                var TotalPagesOcr = document.getElementById("<%= TotalPages_OCR.ClientID%>");

                TotalPagesOcr.innerHTML = MaxPagesocr;

                var DivTextOCR = document.getElementById("<%=divRecordCountText_OCR.ClientID%>").innerHTML;
                $("<%=divRecordCountText_OCR.ClientID %>").html(DivTextOCR + " Record(s)");


            }


        }





        function OnPageIndexChange() {

            var DocumentType = $("#<%= cmbDocumentType.ClientID %>").val();
            var Department = $("#<%= cmbDepartment.ClientID %>").val();
            if (DocumentType != "0" && Department != "0") {
                OnPagingIndexClick(document.getElementById("<%= CurrentPage.ClientID%>").value);
            }

            var DocumentTypeocr = $("#<%= cmbDocumentType_OCR.ClientID %>").val();
            var Departmentocr = $("#<%= cmbDepartment_OCR.ClientID %>").val();
            if (DocumentTypeocr != "0" && Departmentocr != "0") {
                OnPagingIndexClick(document.getElementById("<%= CurrentPage_OCR.ClientID%>").value);
            }


        }

        function OnPagingIndexClick(index) {

            var DocumentType = $("#<%= cmbDocumentType.ClientID %>").val();
            var Department = $("#<%= cmbDepartment.ClientID %>").val();

            if (DocumentType != "0" && Department != "0") {
                document.getElementById("<%= hdnPageNo.ClientID%>").value = index;
                RowsPerPage = document.getElementById("<%= RowsPerPage.ClientID %>").value;
                document.getElementById("<%= hdnRowsPerPage.ClientID %>").value = RowsPerPage;
                document.getElementById('<%=btnSearch.ClientID %>').click();
            }

            var DocumentTypeocr = $("#<%= cmbDocumentType_OCR.ClientID %>").val();
            var Departmentocr = $("#<%= cmbDepartment_OCR.ClientID %>").val();

            if (DocumentTypeocr != "0" && Departmentocr != "0") {
                document.getElementById("<%= hdnPageNo_OCR.ClientID%>").value = index;
                RowsPerPage_OCR = document.getElementById("<%= RowsPerPage_OCR.ClientID %>").value;
                document.getElementById("<%= hdnRowsPerPage_OCR.ClientID %>").value = RowsPerPage_OCR;
                document.getElementById('<%=btnSearchOCR.ClientID %>').click();
            }



        }

        function SetPageIndex(index) {
            return ValidateInputData();
            document.getElementById("<%= hdnPageNo.ClientID %>").value = index;
            document.getElementById("<%= hdnPageNo_OCR.ClientID %>").value = index;

        }


        function SetPageIndexRemove(index) {
            document.getElementById("<%= hdnPageNo.ClientID %>").value = index;
            document.getElementById("<%= hdnPageNo_OCR.ClientID %>").value = index;
        }
    </script>
    <!-- Script for document viewer -->
    <script type="text/javascript" language="javascript">

        function Redirect(ProcessID, DocId, DepID, Active, PageNo, MainTagId, SubTagId, SlicingStatus, Watermark) {
            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            var info = getAcrobatInfo();
            if (true) {//parseInt(info.acrobatVersion, 10) > 10) {
                if (SlicingStatus == 'Uploaded') {
                    window.location.href = "DocumentDownloadDetails.aspx?id=" + ProcessID + '&docid=' + DocId + '&depid=' + DepID + '&active=' + Active + '&PageNo=' + PageNo + '&MainTagId=' + MainTagId + '&SubTagId=' + SubTagId + '&Search=' + document.getElementById("<%=hdnSearchString.ClientID %>").value + '&Page=DocumentDownloadSearch' + '&Watermark=' + Watermark;
                }
                else {

                    $(msgControl).html("Kindly Wait For Few Minutes Document Activity is Pending!");
                    document.getElementById('<%=divMsg.ClientID%>').style.color = 'Red';
                    return false;
                }
            }
            else {
                var MsgAdobeDownload = '<%= ConfigurationManager.AppSettings["MsgAdobeDownload"].ToString() %>';
                var LinkAdobeDownload = '<%= ConfigurationManager.AppSettings["LinkAdobeDownload"].ToString() %>';
                var IsConfirm = window.confirm(MsgAdobeDownload);
                if (IsConfirm == true) {
                    window.open(LinkAdobeDownload);
                }
                return false;
            }
        }

        var getAcrobatInfo = function () {

            var getBrowserName = function () {
                return this.name = this.name || function () {
                    var userAgent = navigator ? navigator.userAgent.toLowerCase() : "other";

                    if (userAgent.indexOf("chrome") > -1) return "chrome";
                    else if (userAgent.indexOf("safari") > -1) return "safari";
                    else if (userAgent.indexOf("msie") > -1) return "ie";
                    else if (userAgent.indexOf("firefox") > -1) return "firefox";
                    return userAgent;
                }();
            };

            var getActiveXObject = function (name) {
                try { return new ActiveXObject(name); } catch (e) { }
            };

            var getNavigatorPlugin = function (name) {
                for (key in navigator.plugins) {
                    var plugin = navigator.plugins[key];
                    if (plugin.name == name) return plugin;
                }
            };

            var getPDFPlugin = function () {
                return this.plugin = this.plugin || function () {
                    if (getBrowserName() == 'ie') {
                        //
                        // load the activeX control
                        // AcroPDF.PDF is used by version 7 and later
                        // PDF.PdfCtrl is used by version 6 and earlier
                        return getActiveXObject('AcroPDF.PDF') || getActiveXObject('PDF.PdfCtrl');
                    }
                    else {
                        return getNavigatorPlugin('Adobe Acrobat') || getNavigatorPlugin('Chrome PDF Viewer') || getNavigatorPlugin('WebKit built-in PDF');
                    }
                }();
            };

            var isAcrobatInstalled = function () {
                return !!getPDFPlugin();
            };

            var getAcrobatVersion = function () {
                try {
                    var plugin = getPDFPlugin();

                    if (getBrowserName() == 'ie') {
                        var versions = plugin.GetVersions().split(',');
                        var latest = versions[0].split('=');
                        return parseFloat(latest[1]);
                    }

                    if (plugin.version) return parseInt(plugin.version);
                    return plugin.name

                }
                catch (e) {
                    return null;
                }
            }

            //
            // The returned object
            // 
            return {
                browser: getBrowserName(),
                acrobat: isAcrobatInstalled() ? 'installed' : false,
                acrobatVersion: getAcrobatVersion()
            };
        };

        //Newcode
        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        //        function pageLoad() {
        //            var msgControl = "#<%= divMsg.ClientID %>";
        //            var SearchCriteria = getParameterByName("Search");
        //            if (SearchCriteria != null && SearchCriteria.length > 0 && document.getElementById("<%=hdnSearchString.ClientID %>").value.length == 0) {
        //                document.getElementById("<%=hdnSearchString.ClientID %>").value = SearchCriteria;
        //                loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
        //                loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
        //                pageIdContorlID = "<%= hdnPageId.ClientID %>";
        //                document.getElementById('<%=btnSearchAgain.ClientID%>').click();
        //            }
        //        }
    </script>
    <script type="text/javascript" language="javascript">

        function FillcontrolValue(ucControl, ucValues) {
            var Value = "";
            var strControlNames = "";
            var ctlHiddenControlValus = ucValues;
            var row1 = "";
            var controlVal = "";
            strControlNames = ucControl.value.split("#||#");

            for (var i = 0; i < strControlNames.length; i++) {

                row = strControlNames[i].split("#|#");

                controlVal += document.getElementById(row[0]).value + "#|#" +
                    document.getElementById(row[1]).value + "#|#" +
                    document.getElementById(row[2]).value + "#|#";
                if (document.getElementById(row[3]) != null) {
                    controlVal += document.getElementById(row[3]).value + "#|##||#";
                }
                else {
                    controlVal += "#|#" + document.getElementById(row[4]).value + "#||#";
                }

            }

            ucValues.value = controlVal.substring(0, controlVal.length - 4);
            return true;
        }
    </script>
</asp:Content>
