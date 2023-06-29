<%@ Page Title="" Language="C#" MasterPageFile="~/DocumentMaster.Master" AutoEventWireup="true"
    CodeBehind="BatchUpload.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.ManageSoftData" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script language="javascript" type="text/javascript">

        hdnTotalRowCount = "<%= hdnTotalRowCount.ClientID %>";
        btnFilterRow = "<%= btnFilterRow.ClientID %>";
        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";

            btnFilterRow = "<%= btnFilterRow.ClientID %>";
            hdnTotalRowCount = "<%= hdnTotalRowCount.ClientID %>";
        });

        function validate() {
            var msgControl = "#<%= divMsg.ClientID %>";
            if (document.getElementById("<%= cmbDocumentType.ClientID  %>").value == 0) {
                $(msgControl).html("Please Select Project Type!");
                return false;
            }
            else if (document.getElementById("<%= cmbDepartment.ClientID  %>").value == 0) {
                $(msgControl).html("Please Select Department!");
                return false;
            }
            return true;
        }

        function uploadStart(sender, args) {
            var result;
            var msgControl = "#<%= divMsg.ClientID %>";
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1);

            if (document.getElementById("<%= cmbDocumentType.ClientID  %>").value == 0) {
                var err = new Error();
                err.name = 'My API Input Error';
                err.message = 'Please Select Project Type!';
                throw (err);
                return false;
            }

            if (filext == 'xls' || filext == 'xlsx' || filext == 'csv' || filext == 'txt') {
                return true;
            }
            else {
                var err = new Error();
                err.name = 'My API Input Error';
                err.message = 'File format not supported! (Supported format xls,xlsx,csv,txt)';
                throw (err);
                return false;
            }
        }


        function getBatchUploadData() {
            //
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";
            //
            var msgControl = "#<%= divMsg.ClientID %>";
            var DocumentType = $("#<%= cmbDocumentType.ClientID %>").val();
            var DepartmentID = $("#<%= cmbDepartment.ClientID %>").val();
            var Filter = $("#<%= cmbFilter.ClientID %>").val();
            var PageNo = document.getElementById("<%= hdnPageNo.ClientID%>").value;
            var RowsPerPage = document.getElementById("<%= hdnRowsPerPage.ClientID%>").value

            var params = DocumentType + '|' + Filter + '|' + DepartmentID + '|' + '0' + '|' + PageNo + '|' + RowsPerPage;
            if (validate()) {
                $("#divSearchResults").html("");
                return CallPostScalar(msgControl, "GetBatchUploadData", params);
            }
        }

        function ReloadDataOnUpload() {
            (document.getElementById('<%=btnReloadGrid.ClientID%>')).click();
            return false;
        }

        // Clear AsyncFileUploadControl content/Path
        function success() {
            //            $('input[type="file"]').each(function () {

            //                $("#" + this.id).replaceWith($("#" + this.id).clone(true));
            //            });
            //            //For other browsers
            //            $('input[type="file"]').each(function () { $("#" + this.id).val(""); });
            getBatchUploadData();
            $(".k-upload-files.k-reset").find("li").remove();
        }

        function DeleteData(ProcessID) {
            var display = confirm("Are you sure you want to delete the selected record?");
            if (display == true) {
                //
                loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
                loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
                pageIdContorlID = "<%= hdnPageId.ClientID %>";
                //
                var msgControl = "#<%= divMsg.ClientID %>";
                var DocumentType = $("#<%= cmbDocumentType.ClientID %>").val();
                var DepartmentID = $("#<%= cmbDepartment.ClientID %>").val();
                var Filter = $("#<%= cmbFilter.ClientID %>").val();
                var PageNo = document.getElementById("<%= hdnPageNo.ClientID%>").value;
                var RowsPerPage = document.getElementById("<%= hdnRowsPerPage.ClientID%>").value;
                var DeleteID = ProcessID;
                var params = DocumentType + '|' + Filter + '|' + DepartmentID + '|' + ProcessID + '|' + PageNo + '|' + RowsPerPage;
                if (validate()) {
                    $("#divSearchResults").html("");
                    return CallPostScalar(msgControl, "GetBatchUploadData", params);
                }
            }
            return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Batch Upload">
   </asp:Label>
    <div class="GVDiv">
       
       <fieldset>
        <div class="DivInlineBlock">
            <asp:UpdatePanel ID="DocumentPanel" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
               
                    <table cellpadding="4" cellspacing="4" border="0">
                        <tr>
                            <td>
                                <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="Project Type"></asp:Label>
                                <asp:Label ID="Label4" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:DropDownList ID="cmbDocumentType" runat="server" 
                                    AutoPostBack="True" OnSelectedIndexChanged="cmbDocumentType_SelectedIndexChanged">
                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                                <asp:Label ID="Label5" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:DropDownList ID="cmbDepartment" runat="server" 
                                    AutoPostBack="True">
                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="style5">
                                <asp:Label ID="Label1" runat="server" CssClass="LabelStyle" Text="Filter"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="Label8" runat="server" CssClass="MandratoryFieldMarkStyle" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="cmbFilter" runat="server" AutoPostBack="True"
                                    >
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                   
                </ContentTemplate>
            </asp:UpdatePanel>
            <table cellpadding="4" cellspacing="4" border="0">
                <tr>
                    <td class="style5">
                        <br />
                        <asp:Label ID="Label6" runat="server" CssClass="LabelStyle" Text="Select File (csv,xls,xlsx,txt)">
                        </asp:Label>
                        <asp:Label ID="Label7" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                        <br />
                        <!--Changed the width from 283px to 160px to make compatible with firefox and chrome--->
                        <asp:AsyncFileUpload ID="AsyncFileUpload1" runat="server" CssClass="LabelStyle" Width="160px"
                            CompleteBackColor="Lime" ErrorBackColor="Red" ThrobberID="Throbber" OnClientUploadStarted="uploadStart"
                            OnUploadedComplete="AsyncFileUpload1_UploadedComplete" UploadingBackColor="#66CCFF"
                            OnClientUploadComplete="success" />
                        <asp:Label ID="Throbber" runat="server" Style="display: none">
                            <img alt="Loading..." src="<%= Page.ResolveClientUrl("~/Images/indicator.gif")%>" /></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="style5">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="style5">
                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                        <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                        <asp:Button ID="btnReloadGrid" runat="server" Style="visibility: hidden;" OnClientClick="getBatchUploadData();return false;" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="DivInlineBlock">
            <div id="divMsg" runat="server" style="color: Red">
                &nbsp;</div>
            <div style="width: inherit; height: inherit;">
                <table width="600" border="0" align="center" class="generalBox">
                    <tr>
                        <td height="8" colspan="3" align="left">
                            <div id="divRecordCountText">
                                &nbsp;</div>
                        </td>
                    </tr>
                    <tr>
                        <td height="8" colspan="3">
                            <div id="paging" style="visibility:hidden;">
                                <a>Rows per page
                                    <select name="RowsPerPage" id="RowsPerPage" runat="server" class="input-mini" onchange="OnPagingIndexClick(1);" style=" width: 50px; ">
                                        <option>5</option>
                                        <option selected="selected">10</option>
                                        <option>25</option>
                                        <option>50</option>
                                        <option>100</option>
                                    </select>
                                    Current Page
                                    <select name="CurrentPage" id="CurrentPage" runat="server" class="input-mini" onchange="OnPageIndexChange();" style=" width: 50px; ">
                                    </select>
                                    of
                                    <asp:Label ID="TotalPages" runat="server" CssClass="LabelStyle" Text=""></asp:Label>
                                    <asp:HiddenField ID="hdnPageNo" Value="1" runat="server" />
                                    <asp:HiddenField ID="hdnSortColumn" Value="ASC" runat="server" />
                                    <asp:HiddenField ID="hdnSortOrder" runat="server" />
                                    <asp:HiddenField ID="hdnTotalRowCount" runat="server" />
                                    <asp:HiddenField ID="hdnRowsPerPage" Value="10" runat="server" />
                                    <asp:Button ID="btnFilterRow" runat="server" Height="25px" class="HiddenButton" Text="&lt;&lt; Remove"
                                        OnClientClick="createPagingIndexes(); return false;" TagName="Read" />
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
         </fieldset>
    </div>
    <script language="javascript" type="text/javascript">

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

        function createPagingIndexes() {
            //*/
            //Dropdown binding

            var indexCount = document.getElementById("<%= hdnTotalRowCount.ClientID%>").value;

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

            //Hide unnecessary columns
            hideColumn('1');
        }

        function OnPageIndexChange() {
            OnPagingIndexClick(document.getElementById("<%= CurrentPage.ClientID%>").value);
        }

        function OnPagingIndexClick(index) {
            document.getElementById("<%= hdnPageNo.ClientID%>").value = index;
            RowsPerPage = document.getElementById("<%= RowsPerPage.ClientID%>").value;
            document.getElementById("<%= hdnRowsPerPage.ClientID%>").value = RowsPerPage;
            getBatchUploadData();
        }
    </script>
</asp:Content>
