<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="DocumentDownload.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.DocumentDownload"
    EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId_BS.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken_BS.ClientID %>";
            pageIdContorlID = "<%= hdnPageId_BS.ClientID %>";
            CountControlsID = "<%=hdnCountControls_BS.ClientID %>";
            btnFilterRow = "<%= btnFilterRow_BS.ClientID %>";
            hdnTotalRowCount = "<%= hdnTotalRowCount_BS.ClientID %>";

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
            var DivText = document.getElementById("divRecordCountText").innerHTML;
            $("#divRecordCountText").html(DivText + " Record(s)");

            //Hide unnecessary columns
            hideColumn_BS('1,3,4,6');
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
                        if (subitemArray.length > 6) {
                            subitemArray[6] = subitemArray[6].split(",");
                        }
                        rightsArray.push(subitemArray);

                    }
                }
            }


        }



        function pageLoad() {
            var msgControl = "#<%= divMsg_BS.ClientID %>";
            var SearchCriteria = getParameterByName("Search");
            if (SearchCriteria != null && SearchCriteria.length > 0 && document.getElementById("<%=hdnSearchString_BS.ClientID %>").value.length == 0) {
                document.getElementById("<%=hdnSearchString_BS.ClientID %>").value = SearchCriteria;
                loginOrgIdControlID = "<%= hdnLoginOrgId_BS.ClientID %>";
                loginTokenControlID = "<%= hdnLoginToken_BS.ClientID %>";
                pageIdContorlID = "<%= hdnPageId_BS.ClientID %>";
                return CallPostScalar(msgControl, "SearchDocuments", SearchCriteria);
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

            switch (SearchCondition.toUpperCase()) {
                case "ANYWHERE":
                    WhereClause += DbField + " like '%" + IndexValue + "%' ) ";
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

        function SearchDocuments_BS() {

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



            var params = SearchOption + '|' + DocumentType + '|' + Department + '|' + Refid + '|' + Keywords + '|' + check + '|' + MainTagID + '|' + SubTagID + '|' + PageNo + '|' + RowsPerPage;

            //Loop through dynamic index controls and construct SQL where clause
            WhereClause = "";
            var arrayDBCols = $('#ctl00_ContentPlaceHolder2_ContentPlaceHolder1_hdnDBCOLMapping_BS').val().split("$");
            var data = rightsArray[0];
            for (var i = 0; i < arrayDBCols.length; i++) {

                var controlname = data[i];

                var controlname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_' + controlname;
                var fields = document.getElementById(controlname);
                try {

                    if (fields != null && fields.type == "text") {
                        var FieldValue = fields.value;
                        if (FieldValue == "") {
                            //Nothing
                        }
                        else {
                            var DbField = GetDBMappingCol_BS(arrayDBCols, controlname.replace("ctl00_ContentPlaceHolder2_ContentPlaceHolder1_", ""));
                            //Call method to construct query for SQL where condition from dynamic controls
                            construct_SearchByIndexValue_Query_BS(DbField, FieldValue, SearchOption);
                        }
                    }

                    else if (fields != null && fields.type == "select-one") {
                        if (fields.options[fields.selectedIndex].text == "--Select--") {
                            //Nothing
                        }
                        else {
                            var DbField = GetDBMappingCol_BS(arrayDBCols, controlname.replace("ctl00_ContentPlaceHolder2_ContentPlaceHolder1_", ""));
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
            params += "|" + WhereClause;

            if (ValidateInputData_BS(msgControl, "SearchDocuments")) {
                $("#divSearchResults").html("");
                document.getElementById("<%=hdnSearchString_BS.ClientID %>").value = params;
                return CallPostScalar(msgControl, "SearchDocuments", params);
            }
        }

        function ValidateData_BS() {

            var msgControl = "#<%= divMsg_BS.ClientID %>";
            var action = "SearchDocuments";
            return ValidateInputData_BS(msgControl, action);
        }

        function ValidateInputData_BS(msgControl, action) {
            $(msgControl).html("");
            var retval = true;
            var DocumentType = $("#<%= cmbDocumentType_BS.ClientID %>").val();
            var Department = $("#<%= cmbDepartment_BS.ClientID %>").val();
            if (action == "SearchDocuments") {
                if (document.getElementById("<%= cmbDocumentType_BS.ClientID  %>").value == 0) {
                    $(msgControl).html("Please Select Project Type!");
                    retval = false;
                }
                else if (document.getElementById("<%= cmbDepartment_BS.ClientID  %>").value == 0) {
                    $(msgControl).html("Please Select Department!");
                    retval = false;
                }
                return retval;
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
            var e = document.getElementById(ID.id);
            var drop = e.options[e.selectedIndex].value;
            if (drop != "0" && drop != "<Select>") {
                (document.getElementById('<%=btnCommonSubmitSub2_BS.ClientID%>')).click();
            }
            return false;
        }

        function Redirect_BS(ProcessID, DocId, DepID, Active, PageNo, MainTagId, SubTagId, SlicingStatus, Watermark) {
            var msgControl = "#<%= divMsg_BS.ClientID %>";
            $(msgControl).html("");
            var info = getAcrobatInfo_BS();
            if (true) {//parseInt(info.acrobatVersion, 10) > 10) {
                if (SlicingStatus == 'Uploaded') {
                    window.location.href = "DocumentDownloadDetails.aspx?id=" + ProcessID + '&docid=' + DocId + '&depid=' + DepID + '&active=' + Active + '&PageNo=' + PageNo + '&MainTagId=' + MainTagId + '&SubTagId=' + SubTagId + '&Search=' + document.getElementById("<%=hdnSearchString_BS.ClientID %>").value + '&Page=DocumentDownload' + '&Watermark=' + Watermark;
                }
                else {

                    $(msgControl).html("Kindly Wait For Few Minutes Document Activity is Pending!");
                    document.getElementById('<%=divMsg_BS.ClientID%>').style.color = 'Red';
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

        var getAcrobatInfo_BS = function () {

            var getBrowserName_BS = function () {
                return this.name = this.name || function () {
                    var userAgent = navigator ? navigator.userAgent.toLowerCase() : "other";

                    if (userAgent.indexOf("chrome") > -1) return "chrome";
                    else if (userAgent.indexOf("safari") > -1) return "safari";
                    else if (userAgent.indexOf("msie") > -1) return "ie";
                    else if (userAgent.indexOf("firefox") > -1) return "firefox";
                    return userAgent;
                } ();
            };

            var getActiveXObject_BS = function (name) {
                try { return new ActiveXObject(name); } catch (e) { }
            };

            var getNavigatorPlugin_BS = function (name) {
                for (key in navigator.plugins) {
                    var plugin = navigator.plugins[key];
                    if (plugin.name == name) return plugin;
                }
            };

            var getPDFPlugin_BS = function () {
                return this.plugin = this.plugin || function () {
                    if (getBrowserName_BS() == 'ie') {
                        //
                        // load the activeX control
                        // AcroPDF.PDF is used by version 7 and later
                        // PDF.PdfCtrl is used by version 6 and earlier
                        return getActiveXObject_BS('AcroPDF.PDF') || getActiveXObject_BS('PDF.PdfCtrl');
                    }
                    else {
                        return getNavigatorPlugin_BS('Adobe Acrobat') || getNavigatorPlugin_BS('Chrome PDF Viewer') || getNavigatorPlugin_BS('WebKit built-in PDF');
                    }
                } ();
            };

            var isAcrobatInstalled_BS = function () {
                return !!getPDFPlugin_BS();
            };

            var getAcrobatVersion_BS = function () {
                try {
                    var plugin = getPDFPlugin_BS();

                    if (getBrowserName_BS() == 'ie') {
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
                browser: getBrowserName_BS(),
                acrobat: isAcrobatInstalled_BS() ? 'installed' : false,
                acrobatVersion: getAcrobatVersion_BS()
            };
        };
    </script>
</asp:Content>
<asp:Content ID="Content4_BS" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCurrentPath_BS" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Document Download"></asp:Label>
    <div class="GVDiv">
       <asp:UpdatePanel ID="UpdatePanel2_BS" runat="server">
                <ContentTemplate>
                    <div id="divMsg_BS" runat="server" style="color: Red">
                        &nbsp;</div>
                </ContentTemplate>
            </asp:UpdatePanel>
        <div class="DivInlineBlock">
            <asp:UpdatePanel ID="UpdatePanel1_BS" runat="server" UpdateMode="Always">
                <ContentTemplate>
                <fieldset>
                    <table>
                        <tr>
                            <td style="width: 120px">
                                <asp:Label ID="Label3_BS" runat="server" CssClass="LabelStyle" Text="Project Type"></asp:Label>
                                &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="lblPageDescription1_BS" runat="server"
                                    Text="*"></asp:Label>
                            </td>
                            <td colspan="3">
                                <asp:DropDownList ID="cmbDocumentType_BS" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDocumentType_SelectedIndexChanged_BS">
                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label5_BS" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                                &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label1_BS" runat="server" Text="*"></asp:Label>
                            </td>
                            <td colspan="3">
                                <asp:DropDownList ID="cmbDepartment_BS" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDepartment_SelectedIndexChanged_BS">
                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label9_BS" runat="server" CssClass="LabelStyle" Text="Ref ID"></asp:Label>
                            </td>
                            <td colspan="3">
                                <asp:TextBox ID="txtRefid_BS" runat="server" Style="margin-left: 0px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label12_BS" runat="server" CssClass="LabelStyle" Text="Keywords "></asp:Label>
                            </td>
                            <td colspan="3">
                                <asp:TextBox ID="txtKeyword_BS" runat="server" Style="margin-left: 0px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label15_BS" runat="server" CssClass="LabelStyle" Text="Main Tags"></asp:Label>
                            </td>
                            <td class="style2" colspan="3">
                                <asp:DropDownList ID="cmbMainTag_BS" runat="server" AutoPostBack="false">
                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label16_BS" runat="server" CssClass="LabelStyle" Text="Sub Tags"></asp:Label>
                            </td>
                            <td class="style2" colspan="3">
                                <asp:DropDownList ID="cmbSubTag_BS" runat="server" AutoPostBack="false">
                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
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
                     
                    <table width="100%">
                        <tr>
                            <td>
                                <asp:Label ID="lblIndexProprties_BS" runat="server" CssClass="LabelStyle" Text="Index Properties"></asp:Label>
                            </td>
                            <td colspan="3">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <div style="height: 188px; width: 500px; overflow-y: scroll">
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
                   </fieldset>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblArchive_BS" runat="server" CssClass="LabelStyle" Text="Include deleted documents"></asp:Label>
                            </td>
                            <td>
                                <asp:CheckBox ID="chkArchive_BS" CssClass="LabelStyle" runat="server" TagName="Read" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <br />
                                <input type="button" id="Search_BS" value="Search" class="btnsearch" onclick="SearchDocuments_BS();"
                                    tagname="Read" />
                                <asp:Button ID="btnAdvanceSearch_BS" runat="server" Text="Advanced Search" CssClass="btnadvancedsearch"
                                    Width="150px" tagname="Read" OnClick="btnAdvanceSearch_Click_BS" OnClientClick="return ValidateData_BS()" />
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
        <div class="DivInlineBlock">
         
            <div style="width: 700px; height: 550px; overflow: auto;">
                <table width="600" border="0" align="center" class="generalBox">
                    <tr>
                        <td height="8" colspan="3" align="left">
                            <div id="divRecordCountText">
                                &nbsp;</div>
                        </td>
                    </tr>
                    <tr>
                        <td height="8" colspan="3">
                            <div id="paging" style="visibility: hidden;">
                                <a>Rows per page
                                    <select name="RowsPerPage" id="RowsPerPage_BS" runat="server" style="width: 50px" onchange="OnPagingIndexClick_BS(1);">
                                        <option>5</option>
                                        <option selected="selected">10</option>
                                        <option>25</option>
                                        <option>50</option>
                                        <option>100</option>
                                    </select>
                                    Current Page
                                    <select name="CurrentPage" id="CurrentPage_BS" runat="server" style="width: 50px" onchange="OnPageIndexChange_BS();">
                                    </select>
                                    of
                                    <asp:Label ID="TotalPages_BS" runat="server" CssClass="LabelStyle" Text=""></asp:Label>
                                    <asp:HiddenField ID="hdnPageNo_BS" Value="1" runat="server" />
                                    <asp:HiddenField ID="hdnSortColumn_BS" Value="ASC" runat="server" />
                                    <asp:HiddenField ID="hdnSortOrder_BS" runat="server" />
                                    <asp:HiddenField ID="hdnTotalRowCount_BS" runat="server" />
                                    <asp:HiddenField ID="hdnRowsPerPage_BS" Value="10" runat="server" />
                                    <asp:Button ID="btnFilterRow_BS" runat="server" Height="25px" class="HiddenButton" Text="&lt;&lt; Remove"
                                        OnClientClick="createPagingIndexes_BS(); return false;" TagName="Read" />
                                </a>
                            </div>
                            <center>
                                <div id="divSearchResults">
                                    &nbsp;</div>
                            </center>
                        </td>
                    </tr>
                    <tr>
                        <td height="8" colspan="3" align="left">
                            <div id="divPagingText">
                                &nbsp;</div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
