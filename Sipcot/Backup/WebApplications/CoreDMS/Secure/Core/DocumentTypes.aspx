<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="DocumentTypes.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.DocumentTypes1" %>

<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl"
    TagPrefix="rjs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";
            FromDateControlId = "#<%= txtCreatedDateFrom.ClientID %>";
            ToDateControlId = "#<%= txtCreatedDateTo.ClientID %>";
            pageRightsContorlId = "<%= hdnPageRights.ClientID %>"
        });

        function Search(action, deletItemId, deletItemName) {
            var msgControl = "#<%= divMsg.ClientID %>";
            var documentTypeName = $("#<%= txtDocumentTypeName.ClientID %>").val();
            var createdDateFrom = $("#<%= txtCreatedDateFrom.ClientID %>").val();
            var createDateTo = $("#<%= txtCreatedDateTo.ClientID %>").val();
            var currentPageId = $("#" + pageIdContorlID).val();
            var params = documentTypeName + '|' + createdDateFrom + '|' + createDateTo + '|'
                + currentPageId + '|' + deletItemId + '|' + deletItemName;
            document.getElementById("<%=hdnSearchString.ClientID %>").value = params;

            return CallPostScalar(msgControl, action, params);
        }

        //********************************************************
        //ClearData Function clears the form
        //********************************************************

        function ClearData() {
            document.getElementById("<%= txtDocumentTypeName.ClientID %>").value = "0";
            document.getElementById("<%= txtCreatedDateFrom.ClientID %>").value = "";
            document.getElementById("<%= txtCreatedDateTo.ClientID %>").value = "";
        }

        //********************************************************
        //Delete User Function 
        //********************************************************

        function DeleteDocumentType(id, name) {
            if (confirm("Do you want to delete the details of the DocumentType - " + name + "?") == true) {
                var action = "DeleteDocumentTypeAndSearch";
                Search(action, id, name);
            }
        }

        //********************************************************
        //Search User Function 
        //********************************************************

        function SearchDocumentTypes() {
            var action = "SearchDocumentTypes";
            Search(action, 0, '');
        }

        //********************************************************
        //Export User Function
        //********************************************************

        function ExportDocumentType(id, name, exportType, tempid) {
            var action = "ExportDocumentType";
            document.getElementById('<%=hdnCurrentDocTypeID.ClientID %>').value = id;
            document.getElementById('<%=hdnDocName.ClientID %>').value = name;
            document.getElementById('<%=hdnExportType.ClientID %>').value = exportType;
            document.getElementById('<%=hdnTemplateId.ClientID %>').value = tempid;

            (document.getElementById('<%=btnExport.ClientID %>')).click();
            return false;
        }
        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        function pageLoad() {
            var msgControl = "#<%= divMsg.ClientID %>";
            var SearchCriteria = getParameterByName("Search");
            if (SearchCriteria.length > 0) {
                var Seararry = SearchCriteria.split('|');
                document.getElementById("<%= txtDocumentTypeName.ClientID %>").value = Seararry[0];
                document.getElementById("<%= txtCreatedDateFrom.ClientID %>").value = Seararry[1];
                document.getElementById("<%= txtCreatedDateTo.ClientID %>").value = Seararry[2];
                var loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
                var loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
                var pageIdContorlID = "<%= hdnPageId.ClientID %>";
                document.getElementById("<%=hdnSearchString.ClientID %>").value = SearchCriteria;
                return CallPostScalar(msgControl, "SearchDocumentTypes", SearchCriteria);

            }
        }
        function Redirect(DocumentTypeId, DocumentTypeName) {
            window.location.href = "ManageDocumentType.aspx?action=edit&id=" + DocumentTypeId + '&doctyp' + DocumentTypeName + '&Search=' + document.getElementById("<%=hdnSearchString.ClientID %>").value;
        }
             
         
    </script>
    <style type="text/css">
        .ButtonStyle
        {
            height: 26px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Configure  &gt;  Project Types"></asp:Label>
        <div class="GVDiv">
            <asp:Label ID="lblPageHeader" runat="server" CssClass="PageHeadings" Text="Search Project Types"></asp:Label>
            <div id="divMsg" runat="server" style="color: Red">
                &nbsp;</div>
            <fieldset>
                <table>
                    <tr>
                        <asp:Label ID="lblSubHeading" CssClass="SubHeadings" runat="server" Text="Search Filters"></asp:Label>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblDepartmentName" runat="server" CssClass="LabelStyle" Text="Project Type"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtDocumentTypeName" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" CssClass="LabelStyle" Text="Created Date From"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCreatedDateFrom" runat="server" ToolTip="Date Format - dd/mm/yyyy"></asp:TextBox>
                                        <rjs:PopCalendar Control="txtCreatedDateFrom" Format="dd/MM/yyyy" ID="pcto1" runat="server"
                                            Separator="/"></rjs:PopCalendar>
                                    </td>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="To"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCreatedDateTo" runat="server" ToolTip="Date Format - dd/mm/yyyy"></asp:TextBox>
                                        <rjs:PopCalendar Control="txtCreatedDateTo" Format="dd/MM/yyyy" ID="PopCalendar1"
                                            runat="server" Separator="/"></rjs:PopCalendar>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td colspan="3">
                                        <asp:RadioButton ID="rdbLast1Week" runat="server" CssClass="RadioButtonStlye" Text="Last 1 week"
                                            GroupName="Period" onclick="PopulateDatePeriod('Last 1 week');" />
                                        <asp:RadioButton ID="rdbLast1Month" runat="server" CssClass="RadioButtonStlye" Text="Last 1 month"
                                            GroupName="Period" onclick="PopulateDatePeriod('Last 1 month');" />
                                        <asp:RadioButton ID="RadioButton1" runat="server" CssClass="RadioButtonStlye" Text="Last 6 months"
                                            GroupName="Period" onclick="PopulateDatePeriod('Last 6 months');" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClientClick="SearchDocumentTypes();return false;"
                                            CssClass="btnsearch" TagName="Read" />
                                        <asp:Button ID="btnExport" runat="server" Style="visibility: hidden" TagName="Read"
                                            OnClick="btnExport_Click" />
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                            <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                            <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                            <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                            <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                            <asp:HiddenField ID="hdnCurrentDocTypeID" runat="server" Value="" />
                            <asp:HiddenField ID="hdnDocName" runat="server" Value="" />
                            <asp:HiddenField ID="hdnExportType" runat="server" Value="" />
                            <asp:HiddenField ID="hdnTemplateId" runat="server" Value="" />
                            <asp:HiddenField ID="hdnSearchString" runat="server" Value="" />
                        </td>
                        <td valign="top">
                            <asp:GridView ID="GridView2" runat="server">
                            </asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td height="8" colspan="3" align="right">
                            <div id="divRecordCountText" class="LabelStyle">
                                &nbsp;</div>
                        </td>
                    </tr>
                    <tr>
                        <td class="GridWidth">
                            <center>
                                <div id="divSearchResults"  class="searchResult">
                                    &nbsp;</div>
                            </center>
                        </td>
                    </tr>
                    <tr>
                        <td height="8" colspan="3" align="right">
                            <div id="divPagingText">
                                &nbsp;</div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </div>
    </div>
</asp:Content>
