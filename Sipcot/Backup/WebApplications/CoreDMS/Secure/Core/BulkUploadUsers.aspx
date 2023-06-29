<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="BulkUploadUsers.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.BulkUploadUsers"
    EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link href="../../App_Themes/common/common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/GridRowSelection.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
        });

        function uploadStart(sender, args) {
            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            var result;
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1);

            if (filext == 'xlsx') {
                return true;
            }
            else {
                var err = new Error();
                err.name = 'My API Input Error';
                err.message = 'File format not supported! (Supported format xlsx)';
                throw (err);
                return false;
            }
        }

        function processExcelData() {
            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            document.getElementById('<%=hdnBtnAction.ClientID%>').value = "ReadExcelData";
            document.getElementById('<%=btnSubmit.ClientID%>').click();
        }

        function deleteuser(UserID) {
            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            if (confirm("Do you want to delete this user?") == true) {
                document.getElementById('<%=hdnBtnAction.ClientID%>').value = "DeleteUser";
                document.getElementById('<%=hdnUserID.ClientID%>').value = UserID;
                document.getElementById('<%=btnSubmitSave.ClientID%>').click();
            }
        }

        function deleteall() {
            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            if (confirm("Do you want to discard these users?") == true) {
                document.getElementById('<%=hdnBtnAction.ClientID%>').value = "DiscardUsers";
                document.getElementById('<%=btnSubmitSave.ClientID%>').click();
                return false;
            }
            return false;
        }

        function commitusers() {
            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            if (confirm("Do you want to commit these users?") == true) {
                document.getElementById('<%=hdnBtnAction.ClientID%>').value = "CommitUsers";
                document.getElementById('<%=btnSubmitSave.ClientID%>').click();
                return false;
            }
            else {
                return false;
            }
        }
        

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="Breadcrum" class="Breadcrum">
        <asp:SiteMapPath ID="SiteMapPath1" runat="server" Font-Size="Small" BorderStyle="None">
            <CurrentNodeStyle ForeColor="#666666" />
            <PathSeparatorTemplate>
                <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/list_arrow.gif" />
            </PathSeparatorTemplate>
        </asp:SiteMapPath>
    </div>
    <div id="Controls" class="GVDiv">
        <fieldset>
            <%-- <asp:UpdatePanel ID="ControlsUpdatePanel" runat="server" UpdateMode="Conditional"
            RenderMode="Inline">
            <ContentTemplate>--%>
            <!--Commented the update panel and added width property and style to asyncfileupload to make it compatibile with firefox and chrome... by sabina-->
            <asp:Label ID="lblFileUpload" runat="server" Text="Choose a file to upload : "></asp:Label>
            <asp:AsyncFileUpload ID="UsersAsyncFileUpload" runat="server" AsyncPostBackTimeout="1600"
                CompleteBackColor="Lime" OnUploadedComplete="UsersAsyncFileUpload_UploadedComplete"
                ErrorBackColor="Red" ThrobberID="Throbber" OnClientUploadStarted="uploadStart"
                OnClientUploadComplete="processExcelData" UploadingBackColor="#66CCFF" Width="160px"
                CssClass="ServerControl3" />
            <asp:Label ID="Throbber" runat="server" Style="display: none">
        <img alt="Loading..." src="<%= Page.ResolveClientUrl("~/Images/indicator.gif")%>" /></asp:Label>
            <%--      </ContentTemplate>
        </asp:UpdatePanel>--%>
            <!--Added css to make button compatibile with chrome-->
            <asp:Button ID="btnDownloadTemplate" runat="server" Style="margin-left: 1039px;"
                Text="Download Template" TagName="Read" CssClass="btndownloadtemplate" OnClick="btnDownloadTemplate_Click" />
            <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always" RenderMode="Inline">
                <ContentTemplate>
                    <asp:Button ID="btnSubmit" class="HiddenButton" runat="server" Text="ProcessExcel"
                        TagName="Read" OnClick="btnSubmit_Click" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="Grid">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always" RenderMode="Block">
                    <ContentTemplate>
                        <div id="divMsg" runat="server" style="color: Red">
                        </div>
                        <asp:Button ID="btnDiscard" runat="server" Text="Discard Users" OnClientClick="return deleteall();return false;" 
                            TagName="Read" Style="margin: 15px 15px 15px 2px;" />
                        <asp:Button ID="btnCommit" runat="server" Text="Commit Users" OnClientClick="return commitusers();return false;"
                            TagName="Read" Style="margin: 15px 15px 15px 15px;" />
                        <div id="customGridDiv">
                            <asp:GridView ID="grdView" runat="server" AutoGenerateColumns="true" EmptyDataText="No record found."
                                CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                                CellPadding="10" CellSpacing="5" OnRowDataBound="grdView_RowDataBound" AllowSorting="True"
                                OnSorting="grdView_Sorting">
                            </asp:GridView>
                        </div>
                        <div class="PagingTD" id="PageDiv" runat="server">
                            <div class="PagingControl">
                                <table border="0" cellpadding="0" cellspacing="0" style="vertical-align: middle;
                                    padding-left: 443px;">
                                    <tr>
                                        <td style="font-size: 8.5pt;" class="style7">
                                            Rows per page
                                            <asp:DropDownList ID="ddlRows" Style="margin: 5px 5px 5px 5px;width:50px;" runat="server" AutoPostBack="True"
                                                OnSelectedIndexChanged="ddlRows_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="lnkbtnFirst" CssClass="GridPageFirstInactive" ToolTip="First" CommandName="First"
                                                runat="server" OnCommand="GetPageIndex"></asp:Button>
                                        </td>
                                        <td style="width: 6px">
                                        </td>
                                        <td>
                                            <asp:Button ID="lnkbtnPre" CssClass="GridPagePreviousInactive" ToolTip="Previous"
                                                CommandName="Previous" runat="server" OnCommand="GetPageIndex"></asp:Button>
                                        </td>
                                        <td style="width: 6px">
                                        </td>
                                        <td style="font-size: 8.5pt;">
                                            Page
                                            <asp:DropDownList ID="ddlPage" runat="server" AutoPostBack="True" Style="width:50px;" OnSelectedIndexChanged="ddlPage_SelectedIndexChanged">
                                            </asp:DropDownList>
                                            of
                                            <asp:Label ID="lblTotalPages" runat="server"></asp:Label>
                                        </td>
                                        <td style="width: 6px">
                                        </td>
                                        <td>
                                            <asp:Button ID="lnkbtnNext" CssClass="GridPageNextInactive" ToolTip="Next" runat="server"
                                                CommandName="Next" OnCommand="GetPageIndex"></asp:Button>
                                        </td>
                                        <td style="width: 6px">
                                        </td>
                                        <td>
                                            <asp:Button ID="lnkbtnLast" CssClass="GridPageLastInactive" ToolTip="Last" runat="server"
                                                CommandName="Last" OnCommand="GetPageIndex"></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                                <asp:Label ID="lblMessage" runat="server" EnableViewState="false"></asp:Label>
                            </div>
                        </div>
                        <asp:Button ID="btnSubmitSave" class="HiddenButton" runat="server" Text="Submit"
                            TagName="Read" OnClick="btnSubmit_Click" />
                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                        <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                        <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                        <asp:HiddenField ID="hdnBtnAction" runat="server" Value="" />
                        <asp:HiddenField ID="hdnUserID" runat="server" Value="" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </fieldset>
    </div>
</asp:Content>
