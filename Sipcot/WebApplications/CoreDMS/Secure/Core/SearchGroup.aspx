<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="SearchGroup.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.SearchGroup" %>

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
            var groupName = $.trim($("#<%= txtGroupName.ClientID %>").val());
            var createdDateFrom = $("#<%= txtCreatedDateFrom.ClientID %>").val();
            var createDateTo = $("#<%= txtCreatedDateTo.ClientID %>").val();
            var currentPageId = $("#" + pageIdContorlID).val();
            var params = groupName + '|' + createdDateFrom + '|' + createDateTo + '|' + currentPageId + '|' + deletItemId + '|' + deletItemName;
            if (ValidateInputData(msgControl, action, groupName, createdDateFrom, createDateTo)) {
                document.getElementById("<%=hdnSearchString.ClientID %>").value = params;
                $("#divSearchResults").html("");
                return CallPostScalar(msgControl, action, params);
            }
            else {
                return false;
            }
        }
        //********************************************************
        //ValidateInputData Function returns true or false with message to user on contorl specified
        //********************************************************

        function ValidateInputData(msgControl, action, groupName, createdDateFrom, createDateTo) {
            $(msgControl).html("");
            if (action == "SearchGroups" || action == "DeleteGroupAndSearch") {
                //                
                return true;
            }
        }
        //********************************************************
        //ClearData Function clears the form
        //********************************************************

        function ClearData() {
            document.getElementById("<%= txtGroupName.ClientID %>").value = "";
            document.getElementById("<%= txtCreatedDateFrom.ClientID %>").value = "";
            document.getElementById("<%= txtCreatedDateTo.ClientID %>").value = "";
            document.getElementById("<%= txtGroupName.ClientID %>").focus();
        }
        //********************************************************
        //AddNew Function navigate to naviage to Create new page
        //********************************************************

        function AddNew() {
            location.href = "ManageGroup.aspx?action=add";
        }

        //********************************************************
        //Delete Group Function 
        //********************************************************

        function DeleteGroup(id, name) {
            if (confirm("Do you want to delete the details of the role - " + name + "?") == true) {
                var currentPageId = $("#<%= hdnPageId.ClientID %>").val();
                var action = "DeleteGroupAndSearch";
                Search(action, id, name);
            }
        }

        //********************************************************
        //Search Group Function 
        //********************************************************

        function SearchGroups() {
            var currentPageId = $("#<%= hdnPageId.ClientID %>").val();
            var action = "SearchGroups";
            Search(action, 0, '');
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
                document.getElementById("<%= txtGroupName.ClientID %>").value = Seararry[0];
                document.getElementById("<%= txtCreatedDateFrom.ClientID %>").value = Seararry[1];
                document.getElementById("<%= txtCreatedDateTo.ClientID %>").value = Seararry[2];
                document.getElementById("<%=hdnSearchString.ClientID %>").value = SearchCriteria;
                var loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
                var loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
                var pageIdContorlID = "<%= hdnPageId.ClientID %>";
                return CallPostScalar(msgControl, "SearchGroups", SearchCriteria);

            }
        }



        function Redirect(GroupId, GroupName) {
            window.location.href = "ManageGroup.aspx?action=edit&id=" + GroupId + '&groupname=' + GroupName + '&Search=' + document.getElementById("<%=hdnSearchString.ClientID %>").value;
        }

         
    </script>
    <style type="text/css">
        .style1
        {
            width: 718px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Configure User Roles"></asp:Label>
    <div class="GVDiv">
        <asp:Label ID="lblPageHeader" runat="server" CssClass="PageHeadings" Text="Search User Roles"></asp:Label><fieldset>
            <table>
                <tr>
                    <td colspan="2">
                        <table cellpadding="0" cellspacing="0" border="0" width="100%">
                            <tr>
                                <td align="left">
                                    <div id="divMsg" runat="server" style="color: Red">
                                        &nbsp;</div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <asp:Label ID="lblSubHeading" CssClass="SubHeadings" runat="server" Text="Search Filters"></asp:Label>
                <tr>
                    <td style="vertical-align: top;">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblUserName3" runat="server" CssClass="LabelStyle" Text="Role Name"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtGroupName" runat="server" Style="margin-left: 0px"></asp:TextBox>
                                </td>
                                <td>
                                </td>
                                <td>
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
                                    <asp:TextBox ID="txtCreatedDateTo" runat="server" CssClass="TextBoxStyle" ToolTip="Date Format - dd/mm/yyyy"></asp:TextBox>
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
                                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClientClick="return SearchGroups()"
                                        CssClass="btnsearch" TagName="Read" />
                                </td>
                                <td>
                                </td>
                                <td>
                                </td>
                            </tr>
                        </table>
                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                        <asp:HiddenField ID="HiddenField1" runat="server" Value="" />
                        <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnSearchString" runat="server" Value="" />
                        <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <div id="divRecordCountText" class="LabelStyle">
                            &nbsp;</div>
                    </td>
                </tr>
                <tr>
                    <td valign="top" class="style1">
                        <table class="GridWidth">
                            <tr>
                                <td height="8" colspan="3" valign="top">
                                    <center>
                                        <div id="divSearchResults" class="searchResult">
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
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
</asp:Content>
