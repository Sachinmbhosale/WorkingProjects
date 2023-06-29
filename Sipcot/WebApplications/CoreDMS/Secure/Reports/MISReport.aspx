<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="MISReport.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Reports.MISReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <link href="../../App_Themes/common/common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/GridRowSelection.js" type="text/javascript"></script>
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
        });

        function validate() {

            var msgControl = "#<%= divMsg.ClientID %>";
                    var result = true;

                    if (document.getElementById("<%= cmbDocumentType.ClientID  %>").value == "0") {
                $(msgControl).html("Please Select Project Type!");
                result = false;
            }
            else if (document.getElementById("<%= cmbDepartment.ClientID  %>").value == "0") {
                $(msgControl).html("Please Select Department!");
                result = false;
            }
            else if (document.getElementById("<%= txtCreatedDateFrom.ClientID  %>").value == "") {
                $(msgControl).html("Please Select Report From Date!");
                result = false;
            }
            else if (document.getElementById("<%= txtCreatedDateTo.ClientID  %>").value == "") {
                $(msgControl).html("Please Select Report To Date!");
                result = false;
            }
            else {
                var fromDate = document.getElementById("<%= txtCreatedDateFrom.ClientID  %>").value;
                var throughDate = document.getElementById("<%= txtCreatedDateTo.ClientID  %>").value;
                if (fromDate > throughDate) {
                    $(msgControl).html("From Date should be less than To Date!");
                    result = false;
                }
            }
            return result;

                }


    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:SiteMapPath ID="SiteMapPath1" runat="server" Font-Size="Small" BorderStyle="None">
        <CurrentNodeStyle ForeColor="#666666" />
        <PathSeparatorTemplate>
            <asp:Image ID="Image4" runat="server" ImageUrl="~/Images/list_arrow.gif" />
        </PathSeparatorTemplate>
    </asp:SiteMapPath>
    <div class="GVDiv">
        <table>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td align="left">
                                            <div id="divMsg" runat="server" style="color: Red; font-family: Calibri; font-size: small">
                                                &nbsp;<asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
                                                    SelectMethod="GetData" TypeName="Lotex.EnterpriseSolutions.WebUI.Secure.Reports.MainReportDataSetTableAdapters.USP_MainReportTableAdapter">
                                                    <SelectParameters>
                                                        <asp:Parameter Name="in_iOrgID" Type="Int32" />
                                                        <asp:Parameter Name="in_dStartDate" Type="DateTime" />
                                                        <asp:Parameter Name="in_dEndDate" Type="DateTime" />
                                                        <asp:Parameter Name="in_vLoginToken" Type="String" />
                                                        <asp:Parameter Name="in_iLoginOrgId" Type="Int32" />
                                                    </SelectParameters>
                                                </asp:ObjectDataSource>
                                                <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" OldValuesParameterFormatString="original_{0}"
                                                    SelectMethod="GetData" TypeName="Lotex.EnterpriseSolutions.WebUI.Secure.Reports.MainReportDataSetTableAdapters.USP_SubMainReportTableAdapter">
                                                    <SelectParameters>
                                                        <asp:Parameter Name="in_iOrgID" Type="Int32" />
                                                        <asp:Parameter Name="in_dStartDate" Type="DateTime" />
                                                        <asp:Parameter Name="in_dEndDate" Type="DateTime" />
                                                        <asp:Parameter Name="in_vLoginToken" Type="String" />
                                                        <asp:Parameter Name="in_iLoginOrgId" Type="Int32" />
                                                    </SelectParameters>
                                                </asp:ObjectDataSource>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <asp:Label ID="lblSubHeading" CssClass="SubHeadings" runat="server" Text="Report Filters"></asp:Label>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <div id="divGrid" style="width: 300px;">
                                                &nbsp;
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <fieldset>
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label4" runat="server" CssClass="LabelStyle" Text="Project Type"></asp:Label>
                                                <asp:Label ID="Label5" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                                            </td>
                                            <asp:UpdatePanel ID="DocumentPanel" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                <ContentTemplate>
                                                    <td>
                                                        <asp:DropDownList ID="cmbDocumentType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDocumentType_SelectedIndexChanged">
                                                            <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label6" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                                                <asp:Label ID="Label7" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbDepartment" runat="server" AutoPostBack="True">
                                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="From"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCreatedDateFrom" runat="server"></asp:TextBox>
                                                <asp:CalendarExtender ID="CalendarExtender1" runat="server" Enabled="True" PopupButtonID="btnCalenderStart"
                                                    Format="yyyy-MM-dd" TargetControlID="txtCreatedDateFrom">
                                                </asp:CalendarExtender>
                                                <asp:ImageButton ID="btnCalenderStart" runat="server" CssClass="ImageButtonStyle"
                                                    Height="16px" ImageUrl="~/Images/CalenderImage.png" Width="16px" />
                                                <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="To"></asp:Label>
                                                <asp:TextBox ID="txtCreatedDateTo" runat="server"></asp:TextBox>
                                                <asp:CalendarExtender ID="txtCreatedDateTo_CalendarExtender" runat="server" PopupButtonID="btnCalenderTo"
                                                    Format="yyyy-MM-dd" Enabled="True" TargetControlID="txtCreatedDateTo">
                                                </asp:CalendarExtender>
                                                <asp:ImageButton ID="btnCalenderTo" runat="server" CssClass="ImageButtonStyle" Height="16px"
                                                    ImageUrl="~/Images/CalenderImage.png" Width="16px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>
                                                <asp:RadioButtonList ID="RdoPeriod" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RdoPeriod_SelectedIndexChanged" RepeatDirection="Horizontal">
                                                    <asp:ListItem>Last 1 week</asp:ListItem>
                                                    <asp:ListItem>Last 1 month</asp:ListItem>
                                                    <asp:ListItem>Last 6 months</asp:ListItem>
                                                </asp:RadioButtonList>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td></td>
                                            <td>
                                                <asp:Button ID="btnsearchSub" runat="server" Text="Search" OnClick="btnsearchSub_Click" CssClass="btnsearch" />
                                                <asp:Button ID="btnclear" runat="server" Text="Clear" OnClick="btnclear_Click" CssClass="btnclear" />
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                                <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                                <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <table border="0">
                                    <tr>
                                        <td height="8" colspan="3" valign="top">
                                            <div>
                                                <table width="1200">
                                                    <tr>
                                                        <td colspan="2">
                                                            <div style="overflow: auto;">
                                                                <%--              <asp:GridView ID="grdView" runat="server" AutoGenerateColumns="true" UseAccessibleHeader="true"
                                                                    AlternatingRowStyle-CssClass="alt" EmptyDataText="No record found for selected criteria."
                                                                    CssClass="mGrid" PagerStyle-CssClass="pgr" OnRowDataBound="grdView_RowDataBound"
                                                                    AllowSorting="True" OnSorting="grdView_Sorting">
                                                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                                                    <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                                                                        PageButtonCount="5" PreviousPageText=" " />
                                                                    <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                                                                </asp:GridView>--%>


                                                                <asp:GridView ID="grdView" runat="server" AutoGenerateColumns="true" UseAccessibleHeader="true"
                                                                    AlternatingRowStyle-CssClass="alt" EmptyDataText="No record found for selected criteria."
                                                                    CssClass="mGrid" PagerStyle-CssClass="pgr" OnRowDataBound="grdView_RowDataBound"
                                                                    AllowSorting="True" OnSorting="grdView_Sorting">
                                                                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                                                    <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                                                                        PageButtonCount="5" PreviousPageText=" " />
                                                                    <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                                                                </asp:GridView>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="PagingTD">
                                                            <table class="PagingControl">
                                                                <tr>
                                                                    <td>
                                                                        <table border="0" cellpadding="0" cellspacing="0" style="vertical-align: middle;">
                                                                            <tr>
                                                                                <td style="font-size: 8.5pt;" class="style7">Rows per page
                                                                                    <asp:DropDownList ID="ddlRows" runat="server" AutoPostBack="True" Width="50px" OnSelectedIndexChanged="ddlRows_SelectedIndexChanged">
                                                                                    </asp:DropDownList>
                                                                                </td>
                                                                                <td></td>
                                                                                <td>
                                                                                    <asp:Button ID="lnkbtnFirst" CssClass="GridPageFirstInactive" ToolTip="First" CommandName="First"
                                                                                        runat="server" OnCommand="GetPageIndex"></asp:Button>
                                                                                </td>
                                                                                <td style="width: 6px"></td>
                                                                                <td>
                                                                                    <asp:Button ID="lnkbtnPre" CssClass="GridPagePreviousInactive" ToolTip="Previous"
                                                                                        CommandName="Previous" runat="server" OnCommand="GetPageIndex"></asp:Button>
                                                                                </td>
                                                                                <td style="width: 6px"></td>
                                                                                <td style="font-size: 8.5pt;">Page
                                                                                    <asp:DropDownList ID="ddlPage" runat="server" AutoPostBack="True" Width="50px" OnSelectedIndexChanged="ddlPage_SelectedIndexChanged">
                                                                                    </asp:DropDownList>
                                                                                    of
                                                                                    <asp:Label ID="lblTotalPages" runat="server"></asp:Label>
                                                                                </td>
                                                                                <td style="width: 6px"></td>
                                                                                <td>
                                                                                    <asp:Button ID="lnkbtnNext" CssClass="GridPageNextInactive" ToolTip="Next" runat="server"
                                                                                        CommandName="Next" OnCommand="GetPageIndex"></asp:Button>
                                                                                </td>
                                                                                <td style="width: 6px"></td>
                                                                                <td>
                                                                                    <asp:Button ID="lnkbtnLast" CssClass="GridPageLastInactive" ToolTip="Last" runat="server"
                                                                                        CommandName="Last" OnCommand="GetPageIndex"></asp:Button>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </td>
                                                                    <td style="width: 12px"></td>
                                                                    <td>
                                                                        <asp:Button ID="lnkbtnExport" CssClass="Export" ToolTip="Export" runat="server" OnClick="lnkbtnExport_Click"></asp:Button>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td style="width: 6px; background-color: #D4D0C8;"></td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:Label ID="lblMessage" runat="server" EnableViewState="false"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
