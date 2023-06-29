<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="Client_SearchUser.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Client_SearchUser" %>

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
            var userName = $.trim($("#<%= txtUserName.ClientID %>").val());
            var userEmailId = $.trim($("#<%= txtUserEmailId.ClientID %>").val());
            var firstName = $.trim($("#<%= txtFirstName.ClientID %>").val());
            var lastName = $.trim($("#<%= txtLastName.ClientID %>").val());
            var createdDateFrom = $("#<%= txtCreatedDateFrom.ClientID %>").val();
            var createDateTo = $("#<%= txtCreatedDateTo.ClientID %>").val();
            var currentPageId = $("#" + pageIdContorlID).val();
            var params = userName + '|' + userEmailId + '|'
                + firstName + '|' + lastName + '|'
                + createdDateFrom + '|' + createDateTo + '|'
                + currentPageId + '|' + deletItemId + '|' + deletItemName;
            if (ValidateInputData(msgControl, action, userName, userEmailId, firstName, lastName, createdDateFrom, createDateTo)) {
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

        function ValidateInputData(msgControl, action, userName, userEmailId, firstName, lastName, createdDateFrom, createDateTo) {
            $(msgControl).html("");
            if (action == "SearchUsers" || action == "DeleteUserAndSearch") {
                //                
                return true;
            }
        }
        //********************************************************
        //ClearData Function clears the form
        //********************************************************

        function ClearData() {
            document.getElementById("<%= txtUserName.ClientID %>").value = "";
            document.getElementById("<%= txtUserEmailId.ClientID %>").value = "";
            document.getElementById("<%= txtFirstName.ClientID  %>").value = "";
            document.getElementById("<%= txtLastName.ClientID  %>").value = "";
            document.getElementById("<%= txtCreatedDateFrom.ClientID %>").value = "";
            document.getElementById("<%= txtCreatedDateTo.ClientID %>").value = "";
        }
        //********************************************************
        //AddNew Function navigate to naviage to Create new item
        //********************************************************

        function AddNew() {
            location.href = "ManageUser.aspx?action=add";
        }

        //********************************************************
        //Delete User Function 
        //********************************************************

        function DeleteUser(id, name) {
            if (confirm("Do you want to delete the user - " + name + "?") == true) {
                var action = "DeleteUserAndSearch";
                Search(action, id, name);
            }
        }

        //********************************************************
        //Search User Function 
        //********************************************************

        function SearchUsers() {
            var action = "SearchUsers";
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

                document.getElementById("<%= txtUserName.ClientID %>").value = Seararry[0];;
                document.getElementById("<%= txtUserEmailId.ClientID %>").value = Seararry[1];
                document.getElementById("<%= txtFirstName.ClientID  %>").value = Seararry[2];
                document.getElementById("<%= txtLastName.ClientID  %>").value = Seararry[3];
                document.getElementById("<%= txtCreatedDateFrom.ClientID %>").value = Seararry[4];
                document.getElementById("<%= txtCreatedDateTo.ClientID %>").value = Seararry[5];
                document.getElementById("<%=hdnSearchString.ClientID %>").value = SearchCriteria;
                var loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
                var loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
                var pageIdContorlID = "<%= hdnPageId.ClientID %>";
                return CallPostScalar(msgControl, "SearchUsers", SearchCriteria);

            }
        }
        function Redirect(UserId, UserName) {
            window.location.href = "ManageUser.aspx?action=edit&id=" + UserId + '&username=' + UserName + '&Search=' + document.getElementById("<%=hdnSearchString.ClientID %>").value;
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Configure User"></asp:Label>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="GVDiv">
                <fieldset>
                    <asp:Label ID="lblPageHeader" runat="server" CssClass="PageHeadings" Text="Search Users"></asp:Label>
                    <table>
                        <tr>
                            <td colspan="2">
                                <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                    <tr>
                                        <td align="left">
                                            <div id="divMsg" runat="server" style="color: Red">
                                                &nbsp;
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>

                            <td style="vertical-align: top;">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblUserName" runat="server" CssClass="LabelStyle" Text="User Name"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblHead2" runat="server" CssClass="LabelStyle" Text="Email"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtUserEmailId" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblHead" runat="server" CssClass="LabelStyle" Text="First Name"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblHead1" runat="server" CssClass="LabelStyle" Text="Last Name"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr style="display: none">
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
                                    <tr style="display: none">
                                        <td></td>
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
                                        <td></td>
                                        <td colspan="3">
                                            <asp:Button ID="btnSearch" runat="server" Text="Search"
                                                CssClass="btnsearch" TagName="Read" OnClick="btnSearch_Click" />
                                            <asp:Button ID="btnClear" runat="server" Text="Clear"
                                                CssClass="btnclear" TagName="Read" OnClick="btnClear_Click" />
                                        </td>

                                    </tr>
                                </table>
                                <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                                <asp:HiddenField ID="HiddenField1" runat="server" Value="" />
                                <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                                <asp:HiddenField ID="hdnSearchString" runat="server" Value="" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <div id="divRecordCountText" class="LabelStyle">
                                    &nbsp;
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <table class="GridWidth">
                                    <tr>
                                        <td height="8" colspan="3" valign="top">
                                            <center>
                                                <div id="divSearchResults" class="searchResult">
                                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                        <ContentTemplate>
                                                            <asp:GridView ID="GridView1" runat="server" CellPadding="5" ForeColor="#333333" GridLines="None"
                                                                AllowPaging="True" OnPageIndexChanging="GridView1_PageIndexChanging"
                                                                CssClass="mGrid" OnRowCommand="GridView1_RowCommand" OnRowDataBound="GridView1_RowDataBound" AutoGenerateColumns="False">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="User Name">
                                                                        <EditItemTemplate>
                                                                            <asp:Label ID="lblusername" runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblusernames" runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
                                                                            <asp:HiddenField ID="hidUserid" runat="server" Value='<%# Eval("UserId") %>' />
                                                                            <asp:HiddenField ID="HidOrgId" runat="server" Value='<%# Eval("UserOrgId") %>' />
                                                                        </ItemTemplate>

                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="First Name">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblusername" runat="server" Text='<%# Eval("FirstName") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Last Name">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lbllastname" runat="server" Text='<%# Eval("LastName") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Email Id">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblEmailid" runat="server" Text='<%# Eval("UserEmailId") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Active/Inactive">
                                                                        <EditItemTemplate>
                                                                            <asp:CheckBox ID="chkActive" runat="server" Text='<%# Eval("Active") %>' />
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate >
                                                                            <asp:CheckBox ID="chkActives" runat="server" Checked='<%# Eval("Active") %>' AutoPostBack="True" OnCheckedChanged="chkActives_CheckedChanged" onclick="if (!confirm('Are you sure to change the status?','Warning','Ok','Cancel',null,this)) return false;"  />

                                                                            <asp:HiddenField ID="hidActive" runat="server" Value='<%# Eval("Active") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                                        
                                                                    </asp:TemplateField>
                                                                    <asp:CommandField HeaderText="Manage User" ShowSelectButton="True" SelectText="Edit">
                                                                        <ItemStyle Font-Bold="True" ForeColor="Blue" VerticalAlign="Middle" />
                                                                         <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                                                    </asp:CommandField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </div>
                                            </center>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td height="8" colspan="3" align="right">
                                            <div id="divPagingText">
                                                &nbsp;
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
