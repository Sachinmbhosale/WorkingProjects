<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="CheckerDashboard.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.CheckerDashboard" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";
            btnFilterRow = "<%= btnFilterRow.ClientID %>";
            hdnTotalRowCount = "<%= hdnTotalRowCount.ClientID %>";

        });

        function SetHiddenVal(param) {
            if (param == "Dynamic") {
                document.getElementById("<%= hdnDynamicControlIndexChange.ClientID %>").value = "1";
            }
            else {
                document.getElementById("<%= hdnDynamicControlIndexChange.ClientID %>").value = "0";
            }
            return true;
        }
        function ValidateInputData() {

            var DocumentType = document.getElementById("<%= cmbDocumentType.ClientID %>").value;

            var Department = document.getElementById("<%= cmbDepartment.ClientID %>").value;
            var msg = document.getElementById("<%= lblMsg.ClientID %>");


            if (document.getElementById("<%= cmbDocumentType.ClientID  %>").value == 0) {
                msg.innerHTML = "Please Select Project Type!";
                msg.setAttribute("display", "block");
                return false;
            }
            else if (document.getElementById("<%= cmbDepartment.ClientID  %>").value == 0) {
                msg.innerHTML = "Please Select Department!";
                msg.setAttribute("display", "block");
                return false;
            }

        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Document Download Search"></asp:Label>
    <div class="GVDiv">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <fieldset style="overflow-x: auto;">
                    <div class="DivInlineBlock">
                        <div id="divResultMsg">
                        </div>
                        <table cellpadding="3" cellspacing="3" class="style1" border="0">
                            <tr>
                                <td colspan="2">
                                    <asp:Label ID="lblMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                                </td>
                            </tr>
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
                                <tr style="display: none">
                                    <td>
                                        <asp:Label ID="Label4" runat="server" CssClass="LabelStyle" Text="Enable Full Text"></asp:Label>
                                    </td>
                                    <td colspan="3">
                                        <asp:CheckBox ID="cbxFullText" runat="server" OnCheckedChanged="AddQuery" AutoPostBack="True" />
                                    </td>
                                </tr>
                            </table>
                            <div style="margin-top: 10px; margin-bottom: 10px;">
                                <asp:Button Text="Edit Query" runat="server" ID="EditQueryButton" OnClick="EditQuery"
                                    Visible="False" />
                            </div>
                            <asp:Label ID="lblError" ForeColor="red" EnableViewState="false" runat="Server" />
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
                        <asp:Panel ID="pnlIndexDropdown" runat="server">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblIndexProprties" runat="server" CssClass="LabelStyle" Text="Search Criteria"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
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
                                                <asp:Button ID="btnAddQuery" runat="server" meta:resourcekey="btnAddQuery" OnClick="AddQuery"
                                                    class="btnadd" Width="100px" OnClientClick="return SetPageIndex(1);" Text="New / Clear"
                                                    ToolTip="New or clear query" />
                                                <asp:Button ID="btnAddClause" runat="Server" meta:resourcekey="btnAddClause" OnClick="btnAddClauseClick"
                                                    class="btnadd" Width="100px" OnClientClick="return SetPageIndex(1);" Text="Add Clause" />
                                                <asp:Button ID="btnRemoveClause" runat="Server" meta:resourcekey="btnRemoveClause"
                                                    class="removebutton" Width="115px" OnClick="btnRemoveClauseClick" OnClientClick="return SetPageIndex(1);"
                                                    Text="Remove Clause" />
                                                <asp:Button ID="btnSearch" runat="server" OnClick="btnPerformQueryClick" Text="Search"
                                                    class="btnsearch" OnClientClick="return ValidateInputData();" />
                                                <div class="fieldgroup">
                                                    <asp:Panel ID="SaveQuery" runat="server">
                                                        <br />
                                                        <table border="0" cellpadding="3" cellspacing="3" class="style1">
                                                            <tr>
                                                                <td style="width: 120px">
                                                                    <asp:Label ID="Label2" runat="server" AssociatedControlID="txtQueryName" CssClass="LabelStyle"
                                                                        Text="Query Name" />
                                                                </td>
                                                                <td colspan="3">
                                                                    <asp:TextBox ID="txtQueryName" runat="Server" />
                                                                    <asp:Button ID="btnSaveQuery" runat="Server" meta:resourcekey="btnSaveQuery" OnClick="btnSaveQueryClick"
                                                                        class="btnsave" Width="100px" Text="Save Query" />
                                                                </td>
                                                            </tr>
                                                            <tr style="visibility: hidden">
                                                                <td style="width: 120px">
                                                                    <asp:Label ID="Label7" runat="server" AssociatedControlID="chkGlobalQuery" CssClass="LabelStyle"
                                                                        meta:resourcekey="chkGlobalQuery" Text="Public Query:" />
                                                                </td>
                                                                <td colspan="3">
                                                                    <asp:CheckBox ID="chkGlobalQuery" runat="server" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                    <div class="submit">
                                                        <asp:Button ID="btnPerformQuery" runat="Server" meta:resourcekey="btnPerformQuery"
                                                            OnClick="btnPerformQueryClick" Text="Search" Visible="false" />
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                            <asp:Button ID="btnCommonSubmitSub" runat="server" CausesValidation="False" Style="visibility: hidden"
                                                Text="Button" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <table>
                            <tr>
                                <td colspan="2">
                                    <br />
                                    <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnSearchString" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnDBCOLMapping" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnIndexNames" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnIndexMinLen" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnIndexType" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnIndexDataType" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnSubDrpID" runat="server" Value="" />
                                </td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="hdnDynamicControlIndexChange" runat="server" Value="0" />
                    </div>
                    <div class="DivInlineBlock">
                        <div id="divMsg" runat="server" style="color: Red">
                            &nbsp;</div>
                        <div style="width: 600px; overflow: auto;">
                            <table width="600" border="0" align="center" class="generalBox">
                                <tr>
                                    <td height="8" colspan="3" align="left">
                                        <div id="divRecordCountText" runat="server">
                                            &nbsp;</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="8" colspan="3">
                                        <div id="paging" runat="server" style="visibility: hidden;">
                                            <a>Rows per page
                                                <select name="RowsPerPage" id="RowsPerPage" runat="server" class="input-mini" onchange="OnPagingIndexClick(1);"
                                                    style="width: 50px;">
                                                    <option>5</option>
                                                    <option selected="selected">10</option>
                                                    <option>25</option>
                                                    <option>50</option>
                                                    <option>100</option>
                                                </select>
                                                Current Page
                                                <select name="CurrentPage" id="CurrentPage" runat="server" class="input-mini" onchange="OnPageIndexChange();"
                                                    style="width: 50px;">
                                                </select>
                                                of
                                                <asp:Label ID="TotalPages" runat="server" CssClass="LabelStyle" Text=""></asp:Label>
                                                <asp:HiddenField ID="hdnPageNo" Value="1" runat="server" />
                                                <asp:HiddenField ID="hdnSortColumn" Value="ASC" runat="server" />
                                                <asp:HiddenField ID="hdnSortOrder" runat="server" />
                                                <asp:HiddenField ID="hdnTotalRowCount" runat="server" />
                                                <asp:HiddenField ID="hdnRowsPerPage" Value="10" runat="server" />
                                                <asp:Button ID="btnFilterRow" runat="server" Height="25px" class="HiddenButton" Text="&lt;&lt; Remove"
                                                    TagName="Read" />
                                            </a>
                                        </div>
                                        <center>
                                            <div id="divSearchResults" runat="server">
                                                &nbsp;</div>
                                        </center>
                                    </td>
                                </tr>
                                <tr>
                                    <td height="8" colspan="3" align="left">
                                        <div id="divPagingText" runat="server">
                                            &nbsp;</div>
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
                    </div>
                    </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </div>
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

                //Hide unnecessary columns
                hideColumn('1,3,4,6');
            }
        }


        //        function Se() {
        //            //*/
        //            //Dropdown binding

        //            var indexCount = document.getElementById("<%= hdnTotalRowCount.ClientID%>").value;

        //            var rowsPerPage = document.getElementById("<%= hdnRowsPerPage.ClientID%>").value
        //            // var MaxPages = indexCount / rowsPerPage + (indexCount % rowsPerPage);
        //            var MaxPages = indexCount / rowsPerPage;
        //            MaxPages = Math.ceil(MaxPages)

        //            var min = 1,
        //                max = MaxPages,
        //                select = document.getElementById("<%= CurrentPage.ClientID%>");
        //            $(select).empty();
        //            for (var i = min; i <= max; i++) {
        //                var opt = document.createElement('option');
        //                opt.value = i;
        //                opt.innerHTML = i;
        //                if (document.getElementById("<%= hdnPageNo.ClientID %>").value == i)
        //                    opt.setAttribute('selected', 'selected');
        //                //select.add(option, 0);
        //                select.appendChild(opt);
        //            }
        ////            var TotalPages = document.getElementById("<%= TotalPages.ClientID%>");
        ////            //document.getElementById("<%= TotalPages.ClientID%>").value = MaxPages;
        ////            //MaxPages = Math.round(MaxPages);
        ////            TotalPages.innerHTML = MaxPages;
        ////            var DivText = document.getElementById("<%=divRecordCountText.ClientID%>").innerHTML;
        ////            $("<%=divRecordCountText.ClientID %>").html(DivText + " Record(s)");

        ////            //Hide unnecessary columns
        ////            hideColumn('1,3,4,6');
        //        }


        function OnPageIndexChange() {
            OnPagingIndexClick(document.getElementById("<%= CurrentPage.ClientID%>").value);
        }

        function OnPagingIndexClick(index) {
            document.getElementById("<%= hdnPageNo.ClientID%>").value = index;
            RowsPerPage = document.getElementById("<%= RowsPerPage.ClientID%>").value;
            document.getElementById("<%= hdnRowsPerPage.ClientID%>").value = RowsPerPage;
            document.getElementById('<%=btnSearch.ClientID%>').click();

        }

        function SetPageIndex(index) {
            return ValidateInputData();
            var Pageno = document.getElementById("<%= hdnPageNo.ClientID%>");
            if (Pageno != null) {
                document.getElementById("<%= hdnPageNo.ClientID%>").value = index;
            }

        }
    </script>
    <!-- Script for document viewer -->
    <script type="text/javascript" language="javascript">
        function Redirect(ProcessID, DocId, DepID, Active, PageNo, MainTagId, SubTagId, SlicingStatus) {
            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            var info = getAcrobatInfo();
            if (true) {//parseInt(info.acrobatVersion, 10) > 10) {
                if (SlicingStatus == 'Uploaded') {
                    window.location.href = "DocumentDownloadDetails.aspx?id=" + ProcessID + '&docid=' + DocId + '&depid=' + DepID + '&active=' + Active + '&PageNo=' + PageNo + '&MainTagId=' + MainTagId + '&SubTagId=' + SubTagId + '&Search=' + document.getElementById("<%=hdnSearchString.ClientID %>").value + '&Page=CheckerDashBoard';
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
                } ();
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
                } ();
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

        function pageLoad() {
            var msgControl = "#<%= divMsg.ClientID %>";
            var SearchCriteria = getParameterByName("Search");
            if (SearchCriteria != null && SearchCriteria.length > 0 && document.getElementById("<%=hdnSearchString.ClientID %>").value.length == 0) {
                document.getElementById("<%=hdnSearchString.ClientID %>").value = SearchCriteria;
                loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
                loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
                pageIdContorlID = "<%= hdnPageId.ClientID %>";
                document.getElementById('<%=btnSearchAgain.ClientID%>').click();
            }
        }
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
