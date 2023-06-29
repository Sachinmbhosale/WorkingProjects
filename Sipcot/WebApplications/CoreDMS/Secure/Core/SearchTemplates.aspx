<%@ Page Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="SearchTemplates.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.SearchTemplates" %>

<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl"
    TagPrefix="rjs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
            <script type="text/javascript">

                function preventBack() { window.history.forward(); }
                setTimeout("preventBack()", 0);
                window.onunload = function () { null };
            </script>
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
            var templateName = $.trim($("#<%= txtTemplateName.ClientID %>").val());
            var createdDateFrom = $("#<%= txtCreatedDateFrom.ClientID %>").val();
            var createDateTo = $("#<%= txtCreatedDateTo.ClientID %>").val();
            var currentPageId = $("#" + pageIdContorlID).val();
            var params = templateName + '|' + createdDateFrom + '|' + createDateTo + '|'
                + currentPageId + '|' + deletItemId + '|' + deletItemName;
            document.getElementById("<%=hdnSearchString.ClientID %>").value = params;

            return CallPostScalar(msgControl, action, params);
        }

        //********************************************************
        //ClearData Function clears the form
        //********************************************************

        function ClearData() {
            document.getElementById("<%= txtTemplateName.ClientID %>").value = "";
            document.getElementById("<%= txtCreatedDateFrom.ClientID %>").value = "";
            document.getElementById("<%= txtCreatedDateTo.ClientID %>").value = "";
        }
        //********************************************************
        //AddNew Function navigate to naviage to Create new item
        //********************************************************

        function AddNew() {
            location.href = "AddNewDocumentTemplate.aspx?action=add";
        }

        //********************************************************
        //Delete User Function 
        //********************************************************

        function DeleteTemplate(id, name) {
            if (confirm("Do you want to delete the details of the template - " + name + "?") == true) {
                var action = "DeleteTemplateAndSearch";
                Search(action, id, name);
            }
        }
        //********************************************************
        //Search User Function 
        //********************************************************

        function SearchTemplates() {
            var action = "SearchTemplates";
            Search(action, 0, '');
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
                document.getElementById("<%= txtTemplateName.ClientID %>").value = Seararry[0];
                document.getElementById("<%= txtCreatedDateFrom.ClientID %>").value = Seararry[1];
                document.getElementById("<%= txtCreatedDateTo.ClientID %>").value = Seararry[2];
                document.getElementById("<%=hdnSearchString.ClientID %>").value = SearchCriteria;
                var loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
                var loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
                var pageIdContorlID = "<%= hdnPageId.ClientID %>";
                return CallPostScalar(msgControl, "SearchTemplates", SearchCriteria);

            }
        }


        function Redirect(TemplateId, TemplateName) {
            window.location.href = "ManageTemplate.aspx?action=edit&id=" + TemplateId + '&tname' + TemplateName + '&Search=' + document.getElementById("<%=hdnSearchString.ClientID %>").value;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Templates"></asp:Label>
        <div class="GVDiv">
            <asp:Label ID="lblPageHeader" runat="server" CssClass="PageHeadings" Text="Search Templates"></asp:Label>
            <div id="divMsg" runat="server" style="color: Red">
                &nbsp;</div>
            <fieldset>
                <table>
                    <tr>
                        <asp:Label ID="lblSubHeading" CssClass="SubHeadings" runat="server" Text="Search Filters"></asp:Label>
                        <td style="vertical-align: top;">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblTemplateName" runat="server" CssClass="LabelStyle" Text="Template Name"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtTemplateName" runat="server"></asp:TextBox>
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
                                        <asp:RadioButton ID="rdbLast6Months" runat="server" CssClass="RadioButtonStlye" Text="Last 6 months"
                                            GroupName="Period" onclick="PopulateDatePeriod('Last 6 months');" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                    <td>
                                        <input type="button" value="Search" onclick="SearchTemplates();" class="btnsearch" tagname="Read" />
                                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnCurrentTemplateId" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnSearchString" runat="server" Value="" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td height="8" align="right">
                            <div id="divRecordCountText">
                                &nbsp;</div>
                        </td>
                    </tr>
                    <tr>
                        <td class="GridWidth">
                            <center>
                                <div id="divSearchResults" class="searchResult">
                                    &nbsp;</div>
                            </center>
                        </td>
                    </tr>
                    <tr>
                        <td height="8" align="right">
                            <div id="divPagingText">
                                &nbsp;</div>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </div>
    </div>
</asp:Content>
