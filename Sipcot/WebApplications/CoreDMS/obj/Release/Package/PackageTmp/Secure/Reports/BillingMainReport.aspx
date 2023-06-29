<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="BillingMainReport.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Reports.BillingMainReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";
        });

        function validate() {

            var msgControl = "#<%= divMsg.ClientID %>";
            var result = true;
            if (document.getElementById("<%= drpOrg.ClientID  %>").value == "-1") {
                $(msgControl).html("Please Select Customer!");
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
            $('#pnlMainDiv').hide();
            $('#PnlSubDiv').hide();

            return result;

        }

        function comparedates() {

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
                <td colspan="2">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
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
                    <fieldset>
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblOrgName" runat="server" CssClass="LabelStyle" Text="Customer Name"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="drpOrg" runat="server" AutoPostBack="false">
                                        <asp:ListItem Value="-1">&lt;select&gt;</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="From"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCreatedDateFrom" runat="server"></asp:TextBox>
                                    <asp:CalendarExtender ID="txtCreatedDateFrom_CalendarExtender" runat="server" Enabled="True"
                                        PopupButtonID="btnCalenderStart" Format="yyyy-MM-dd" TargetControlID="txtCreatedDateFrom">
                                    </asp:CalendarExtender>
                                    <asp:ImageButton ID="btnCalenderStart" runat="server" CssClass="ImageButtonStyle"
                                        Height="16px" ImageUrl="~/Images/CalenderImage.png" Width="16px" />
                                </td>
                                <td>
                                    <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="To"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCreatedDateTo" runat="server"></asp:TextBox>
                                    <asp:CalendarExtender ID="txtCreatedDateTo_CalendarExtender" runat="server" PopupButtonID="btnCalenderTo"
                                        Format="yyyy-MM-dd" Enabled="True" TargetControlID="txtCreatedDateTo">
                                    </asp:CalendarExtender>
                                    <asp:ImageButton ID="btnCalenderTo" runat="server" CssClass="ImageButtonStyle" Height="16px"
                                        ImageUrl="~/Images/CalenderImage.png" Width="16px" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:Button ID="btnGenerateSub" runat="server" Text="Sub Main Report" OnClick="btnGenerateSub_Click"
                                        CssClass="btnsubmainreport" />
                                    <asp:Button ID="btnGenerateMain" runat="server" Text="Main Report" OnClick="btnGenerateMain_Click"
                                        CssClass="btnmainreport" />
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
                <td valign="top">
                    <table width="800" border="0">
                        <tr>
                            <td height="8" colspan="3" valign="top">
                                <center>
                                    <div id="pnlMainDiv" style="background-color: rgb(234, 235, 229); overflow: auto;">
                                        <asp:Panel ID="pnlMain" runat="server" Visible="false">
                                            <rsweb:ReportViewer ID="ReportViewerMain" runat="server" EnableTheming="True" Font-Names="Calibri"
                                                Font-Size="Small" Height="616px" WaitMessageFont-Names="Calibri" WaitMessageFont-Size="Small"
                                                Width="1000px" InteractiveDeviceInfos="(Collection)" PageCountMode="Actual" AsyncRendering="False"
                                                SizeToReportContent="True">
                                                <LocalReport ReportPath="Secure\Reports\MainReport.rdlc">
                                                </LocalReport>
                                            </rsweb:ReportViewer>
                                        </asp:Panel>
                                    </div>
                                    <div id="PnlSubDiv" style="background-color: rgb(234, 235, 229); overflow: auto;">
                                        <asp:Panel ID="pnlSub" runat="server" Visible="false">
                                            <rsweb:ReportViewer ID="ReportViewerSub" runat="server" Font-Names="Calibri" Font-Size="X-Small"
                                                BackColor="AliceBlue" Height="615px" WaitMessageFont-Names="Calibri" WaitMessageFont-Size="Small"
                                                Width="1000px" InteractiveDeviceInfos="(Collection)" PageCountMode="Actual">
                                                <LocalReport ReportPath="Secure\Reports\SubMainReport.rdlc">
                                                </LocalReport>
                                            </rsweb:ReportViewer>
                                        </asp:Panel>
                                    </div>
                                </center>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
