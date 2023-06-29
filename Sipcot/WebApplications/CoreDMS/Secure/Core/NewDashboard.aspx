<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true" CodeBehind="NewDashboard.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.NewDashboard" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>


<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <link rel="stylesheet" href="http://localhost:55361/code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
    <link rel="stylesheet" href="/resources/demos/style.css">
    <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId_BS.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken_BS.ClientID %>";
            pageIdContorlID = "<%= hdnPageId_BS.ClientID %>";



        });
        function SetHiddenVal_BS(param) {
            if (param == "Dynamic") {
                document.getElementById("<%= hdnDynamicControlIndexChange_BS.ClientID %>").value = "1";
            }
            else {
                document.getElementById("<%= hdnDynamicControlIndexChange_BS.ClientID %>").value = "0";
            }
            return true;
        }

        //function valid()
        //{
        //alert();
        //if ($('#<%= cmbDocumentType_BS.ClientID %>').val() != "" && $('#<%= cmbDepartment_BS.ClientID %>').val() && $('#<%= cmdIndex_BS.ClientID %>').val() && $.trim(('#<%= TextBox1.ClientID %>').val()) != "") {
        //$('#<%= btnSearch.ClientID %>').removeProp('Enabled');
        // }
        // }
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Dashboard"></asp:Label>

    <div class="GVDiv">

        <div id="divMsg_BS" runat="server" style="color: Red">
            &nbsp;
        </div>
        <div class="DivInlineBlock">
            <table>
                <tr>
                    <td style="width: 180px">
                        <asp:Label ID="Label3_BS" runat="server" CssClass="LabelStyle" Text="Project Type"></asp:Label>
                        &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="lblPageDescription1_BS"
                            runat="server" Text="*"></asp:Label>
                    </td>
                    <td colspan="3" style="width: 580px">
                        <asp:DropDownList ID="cmbDocumentType_BS" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDocumentType_SelectedIndexChanged_BS">
                            <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label5_BS" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                        &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label1_BS" runat="server"
                            Text="*"></asp:Label>
                    </td>
                    <td colspan="3">
                        <asp:DropDownList ID="cmbDepartment_BS" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDepartmentType_SelectedIndexChanged_BS">
                            <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" CssClass="LabelStyle" Text="Index Field"></asp:Label>
                        &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label5" runat="server"
                            Text="*"></asp:Label>
                    </td>
                    <td colspan="3">
                        <asp:DropDownList ID="cmdIndex_BS" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmdIndex_BS_SelectedIndexChanged_BS">
                            <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" CssClass="LabelStyle" Text="Index Search Value"></asp:Label>
                        &nbsp;<asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label2"
                            runat="server" Text="*"></asp:Label>
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                        <br />
                        <label style="color: red; font-size: 10px">* (in case of date, input format : "dd/mm/yyyy" i.e. "24/05/2017" ) </label>
                    
                    </td>
                </tr>
                <tr><td colspan="5">&nbsp;</td></tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" class="btnsearch" />
                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                        <br />

                        <asp:Label ID="err" runat="server" Text="" Style="color: red"></asp:Label>
                    </td>

                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblResult" runat="server" Text="" Style="font-weight: bold;"></asp:Label>
                    </td>

                </tr>
            </table>
            <br />
            <asp:Chart ID="Chart1" runat="server" Height="300px" Width="400px" Visible="true">
                <Titles>
                    <asp:Title ShadowOffset="3" Name="Items" />
                </Titles>
                <Legends>
                    <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="False" Name="Default" LegendStyle="Row" />
                </Legends>
                <Series>
                    <asp:Series Name="Default" />
                </Series>
                <ChartAreas>
                    <asp:ChartArea Name="ChartArea1" BorderWidth="0" />
                </ChartAreas>
            </asp:Chart>
            <br />
            <label style="color:#f00;font-size:10px">* x-axis : Index Field<br />* y-axis: Nos. of file(s)</label>
        </div>

    </div>


    <asp:HiddenField ID="hdnLoginOrgId_BS" runat="server" />
    <asp:HiddenField ID="hdnLoginToken_BS" runat="server" />
    <asp:HiddenField ID="hdnPageId_BS" runat="server" />
    <asp:HiddenField ID="hdnAction_BS" runat="server" />
    <asp:HiddenField ID="hdnDynamicControlIndexChange_BS" runat="server" Value="0" />
</asp:Content>
