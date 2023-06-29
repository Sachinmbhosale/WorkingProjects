<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="DocumentTypeReport.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Reports.DocumentTypeReport" %>

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
            if (document.getElementById("<%= drpOrg.ClientID  %>").value == "0") {
                $(msgControl).html("Please Select Customer!");
                result = false;
            }
            else if (document.getElementById("<%= drpDocType.ClientID  %>").value == "-1") {
                $(msgControl).html("Please Select Project Type!");
                result = false;
            }
            else if (document.getElementById("<%= drpAction.ClientID  %>").value == "0") {
                $(msgControl).html("Please Select Action Type!");
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
              
                <td colspan="2">
                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                        <tr>
                            <td align="left">
                                <div id="divMsg" runat="server" style="color: Red; font-family: Calibri; font-size: small">
                                    &nbsp;</div>
                            </td>
                        </tr>
                    </table>
                    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="GetData" TypeName="Lotex.EnterpriseSolutions.WebUI.Secure.Reports.MainReportDataSetTableAdapters.USP_CustTypeDocTypeReportTableAdapter">
                        <SelectParameters>
                            <asp:Parameter Name="in_iOrgID" Type="Int32" />
                            <asp:Parameter Name="in_iDocTypeId" Type="Int32" />
                            <asp:Parameter Name="in_dStartDate" Type="DateTime" />
                            <asp:Parameter Name="in_dEndDate" Type="DateTime" />
                            <asp:Parameter Name="in_vLoginToken" Type="String" />
                            <asp:Parameter Name="in_iLoginOrgId" Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
            </tr>
            <tr>
                <asp:Label ID="lblSubHeading" CssClass="SubHeadings" runat="server" Text="Report Filters"></asp:Label>
                <td style="vertical-align: top;">
                <fieldset>
                    <table>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblOrgName" runat="server" CssClass="LabelStyle" Text="Customer Name"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpOrg" runat="server"  OnSelectedIndexChanged="drpOrg_SelectedIndexChanged"
                                            AutoPostBack="True">
                                            <asp:ListItem Value="0">&lt;select&gt;</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblDoctType" runat="server" CssClass="LabelStyle" Text="Project Type"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpDocType" runat="server" >
                                            <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblAction" runat="server" CssClass="LabelStyle" Text="Action Type"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="drpAction" runat="server" >
                                            <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                            <asp:ListItem Value="1000">Upload</asp:ListItem>
                                            <asp:ListItem Value="1001">Download</asp:ListItem>
                                            <asp:ListItem Value="1004">Viewed</asp:ListItem>
                                            <asp:ListItem Value="1011">Printed</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                      <tr> 
                      <td></td>
                      <td>
                                <asp:Label ID="Label1" runat="server" CssClass="LabelStyle" Text="Upload/Download Date :"></asp:Label>
                            </td>
                            
                            </tr>
                        <tr>
                             <td>
                                <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="From"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCreatedDateFrom" runat="server" ></asp:TextBox>
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
                                <asp:TextBox ID="txtCreatedDateTo" runat="server"  ></asp:TextBox>
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
                                <asp:Button ID="btnGenerate" runat="server"  Text="Generate Report"   CssClass="btngeneratereport"
                                    OnClick="btnGenerate_Click" />
                            </td>
                        </tr>
                              </ContentTemplate>
                        </asp:UpdatePanel>
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
                    <table width="600" border="0">
                        <tr>
                            <td height="8" colspan="3" valign="top">
                                <center>
                                    <div id="pnlMainDiv" style=" background-color: rgb(234, 235, 229);overflow:auto; ">
                                        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Calibri" Font-Size="Small"
                                            Height="616px" InteractiveDeviceInfos="(Collection)" WaitMessageFont-Names="Calibri"
                                            WaitMessageFont-Size="Small" Width="1000px" Visible="False" PageCountMode="Actual"
                                            AsyncRendering="False">
                                            <LocalReport ReportPath="Secure\Reports\CustTypeDocTypeReport.rdlc">
                                            </LocalReport>
                                        </rsweb:ReportViewer>
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
