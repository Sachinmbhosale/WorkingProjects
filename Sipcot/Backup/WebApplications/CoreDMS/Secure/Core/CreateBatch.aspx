<%@ Page Title="" Language="C#" MasterPageFile="~/DocumentMaster.Master" AutoEventWireup="true"
    CodeBehind="CreateBatch.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.CreateBatch"
    EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <link href="../../App_Themes/common/common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/GridRowSelection.js" type="text/javascript"></script>
    <script type="text/javascript">
        function validate() {
            var msgControl = "#<%= divMsg.ClientID %>";
            var cmbBatchName = $("#<%= cmbBatchName.ClientID %>").val();
            var txtBatchName = $.trim($("#<%= txtBatchName.ClientID %>").val());
            if (cmbBatchName == "0" && txtBatchName.length == 0) {
                $(msgControl).html("Please enter Batch Name!");
                return false;
            }
            return true;
        }
        function BrowserIdentification() {
            var Browser;
            var isOpera = !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;
            // Opera 8.0+ (UA detection to detect Blink/v8-powered Opera)
            var isFirefox = typeof InstallTrigger !== 'undefined';   // Firefox 1.0+
            var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;
            // At least Safari 3+: "[object HTMLElementConstructor]"
            var isChrome = !!window.chrome && !isOpera;              // Chrome 1+
            var isIE = /*@cc_on!@*/false || !!document.documentMode; // At least IE6

            if (isOpera) {
                Browser = "opera";
            }
            else if (isFirefox) {
                Browser = "firefox";
            }
            else if (isSafari) {
                Browser = "safari";
            }
            else if (isChrome) {
                Browser = "chrome";
            }
            else if (isIE) {
                Browser = "ie";
            }
            return Browser;
        }
        function InstallApp() {
            var OrgID = $("#<%= hdnLoginOrgId.ClientID %>").val();
            var LoginToken = $("#<%= hdnLoginToken.ClientID %>").val();
            var ClickOnceLink = '<%= ConfigurationManager.AppSettings["ClickOnceLink"].ToString() %>' + '?OrgID=' + OrgID + '&LoginToken=' + LoginToken;
            var browser = BrowserIdentification();
            if (browser != "chrome") {
                window.open(ClickOnceLink);
            }
            else {
                alert("This feature is not supported in Google Chrome! Please use IE or Mozilla Firefox.");
            }
            return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="Breadcrum">
        <asp:SiteMapPath ID="SiteMapPath1" runat="server" Font-Size="Small" BorderStyle="None" CssClass="CurrentPath">
            <CurrentNodeStyle ForeColor="#666666" />
            <PathSeparatorTemplate>
                <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/list_arrow.gif" />
            </PathSeparatorTemplate>
        </asp:SiteMapPath>
    </div>
    <div id="Controls" class="GVDiv">
    <fieldset>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always" RenderMode="Block">
            <ContentTemplate>
                <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="Project Type"></asp:Label>
                <asp:Label ID="Label4" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                &nbsp&nbsp&nbsp&nbsp&nbsp
                <asp:DropDownList ID="cmbDocumentType" runat="server" AutoPostBack="True" Style="margin-left: 2px;"
                    OnSelectedIndexChanged="cmbDocumentType_SelectedIndexChanged">
                    <asp:ListItem Value="0">--Select--</asp:ListItem>
                </asp:DropDownList>
                <br />
                <br />
                <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                <asp:Label ID="Label5" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                <asp:DropDownList ID="cmbDepartment" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDepartment_SelectedIndexChanged">
                    <asp:ListItem Value="0">--Select--</asp:ListItem>
                </asp:DropDownList>
                <br />
                <br />
                <asp:Label ID="Label1" runat="server" CssClass="LabelStyle" Text="Batch Name"></asp:Label>
                <asp:Label ID="Label6" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                <asp:DropDownList ID="cmbBatchName" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbBatchName_SelectedIndexChanged">
                    <asp:ListItem Value="0">--Select--</asp:ListItem>
                </asp:DropDownList>
                <br />
                <br />
                <asp:Panel ID="Statuspanel" runat="server">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="lblSearchbyStatus" runat="server" CssClass="LabelStyle" Text="Status"></asp:Label>
                                <asp:Label ID="Label8" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                            </td>
                            <td style="padding-left: 30px">
                                &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp
                                <asp:DropDownList ID="ddlstatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlstatus_SelectedIndexChanged"
                                    Style="margin-top: 0px">
                                    <asp:ListItem>All</asp:ListItem>
                                    <asp:ListItem>Uploaded</asp:ListItem>
                                    <asp:ListItem>Pending</asp:ListItem>
                                    <asp:ListItem>NotUploaded</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td style="padding-left: 30px">
                                <asp:Label ID="Label7" runat="server" CssClass="LabelStyle" Text="Display Only Selected"></asp:Label>
                            </td>
                            <td style="padding-left: 10px">
                                <asp:CheckBox ID="chkDisplaySelected" runat="server" AutoPostBack="true" OnCheckedChanged="chkDisplaySelected_CheckedChanged" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Button ID="btnRun" runat="server" Text="Bulk Upload" Style="position: absolute;
                    margin-top: 22px;" CssClass="btnbulkupload" OnClientClick="return InstallApp();return false;" />
                <br />
                <br />
                <br />
                <br />
                <asp:Label ID="lblBatchName" runat="server" CssClass="LabelStyle" Text="New Batch Name"></asp:Label>&nbsp
                <asp:TextBox ID="txtBatchName" runat="server"></asp:TextBox>
                <br />
                <br />
                <asp:Button ID="btnProcess" runat="server" Text="Create" Style="margin-left: 104px;" CssClass="btn"
                    OnClick="btnProcess_Click" OnClientClick="return validate();" />
                &nbsp&nbsp<asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click"  CssClass="btn"
                   />
                &nbsp;&nbsp;
                <asp:CheckBox ID="chkSelectPage" runat="server" AutoPostBack="true" Text="Select Page Wise"
                    OnCheckedChanged="chkSelectPage_CheckedChanged" />
                &nbsp;
                <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="true" Text="Select All"
                    OnCheckedChanged="chkSelectAll_CheckedChanged" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="Grid">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always" RenderMode="Block">
                <ContentTemplate>
                    <div id="divMsg" runat="server" style="color: Red">
                        &nbsp;</div>
                    <div id="customGridDiv">
                        <asp:GridView ID="grdView" runat="server" CssClass="mGrid" PagerStyle-CssClass="pgr"
                            EmptyDataText="No record found." OnRowDataBound="grdView_RowDataBound" AllowSorting="True"
                            OnSorting="grdView_Sorting" AlternatingRowStyle-CssClass="alt" CellPadding="10"
                            CellSpacing="5">
                            <Columns>
                                <asp:TemplateField HeaderText="Select">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelected" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelected_CheckedChanged" />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="GridItem1" />
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                                PageButtonCount="5" PreviousPageText=" " />
                            <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                        </asp:GridView>
                    </div>
                    <div class="PagingTD" id="PageDiv" runat="server">
                        <div class="PagingControl">
                            <table border="0" cellpadding="0" cellspacing="0" style="vertical-align: middle;
                                padding-left: 443px;">
                                <tr>
                                    <td style="font-size: 8.5pt;" class="style7">
                                        Rows per page
                                        <asp:DropDownList ID="ddlRows" Style="margin: 5px 5px 5px 5px; width: 50px;" runat="server"
                                            AutoPostBack="True" OnSelectedIndexChanged="ddlRows_SelectedIndexChanged">
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
                                        <asp:DropDownList ID="ddlPage" runat="server" AutoPostBack="True" Width="50px" OnSelectedIndexChanged="ddlPage_SelectedIndexChanged">
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
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        </fieldset>
    </div>
    <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
    <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
    <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
    <asp:HiddenField ID="hdnAction" runat="server" Value="" />
    <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
    <asp:HiddenField ID="hdnBatchatcion" runat="server" Value="" />
</asp:Content>
