<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.Dashboard" %>

<%--<asp:Content ID="Content1" ContentPlaceHolderID="test" runat="server">
</asp:Content>--%>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        .GVDiv
        {
            margin-top: 0px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

    <script src="Content/bootstrap/js/bootstrap.min.js"></script>
    <link href="Content/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="~/Scripts/jquery-1.7.1.min.js"></script>

    <%--<asp:Legend Docking="Bottom" Title="Count vs Type" TableStyle="Wide"
                                                        BorderDashStyle="Solid" BorderColor="#e8eaf1" TitleSeparator="Line" TitleFont="TimesNewRoman"
                                                        TitleSeparatorColor="#e8eaf1">
                                                    </asp:Legend>--%>
    <div class="GVDiv">
        <table style="width: 100%; border-style: solid;">
            <tr>
                <td style="text-align: right; width: 5%">
                    <asp:Image ID="Image1" runat="server" Height="38px" ImageUrl="~/Images/Home.png" Width="46px" />
                </td>
                <td colspan="3" style="text-align: left">&nbsp;<asp:Label ID="lblHeader" runat="server" Text="Dashboard" Font-Bold="True" Font-Italic="False" Font-Size="Large"></asp:Label>
                </td>
            </tr>
        </table>
        <div class="container-fluid">
            <%-- <asp:Series Name="Series1"></asp:Series>--%>
            <fieldset>
                <asp:Panel ID="pnlError" runat="server">
                    <asp:Label ID="lblErrMsg" runat="server" Text="*" Visible="false" ForeColor="Red"></asp:Label>
                </asp:Panel>
                <div id="divMsg" runat="server" style="color: Red">
                    &nbsp;
                </div>
                <div class="form-group col-lg-12 col-md-12">
                    <table style="width: 65%; padding-left:100px; display:none " class="table table-bordered">
                        <tr>
                            <td style="width: 25%" align="right">
                                <asp:Label ID="lblProjType" runat="server" Text="Project Type:" Font-Bold="True"></asp:Label>
                            </td>
                            <td style="width: 25%" align="left">
                                 <%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                       <asp:DropDownList ID="ddlProjType" Width="98%" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlProjType_SelectedIndexChanged">
                                           <asp:ListItem>Select</asp:ListItem>
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>--%>
                            </td>
                            <td align="right" style="width: 19%">
                                <asp:Label ID="lblDept" runat="server" Text="Department:" Font-Bold="True"></asp:Label>
                            </td>
                            <td>
                               <%-- <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlDept" Width="80%" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlDept_SelectedIndexChanged">
                                            <asp:ListItem>Select</asp:ListItem>
                                    <asp:ListItem Value="Twowheeler">Twowheeler</asp:ListItem>
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                    <Triggers>
                                        <%--<asp:AsyncPostBackTrigger ControlID="ddlProjType" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>--%>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="form-group col-lg-10 col-md-10">
                    <table style="width: 100%; ">
                        <tr>
                          
                            <td style="width: 40%; height: 400px;">
                                <div class="GVDiv">
                                    <table style="width: 100%; height: auto">
                                        <tr style="display:none">
                                            <td style="text-align: center;">
                                                <asp:Label ID="lable1" runat="server" Text="Overall total count:" Font-Bold="True"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: center; height: 25px; display:none">
                                                <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                                    <ContentTemplate>
                                                        <asp:Label ID="lblChart1ErrMsg" Text="*" runat="server" Visible="false" ForeColor="Red"></asp:Label>
                                                    </ContentTemplate>
                                                   <%-- <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="ddlProjType" EventName="SelectedIndexChanged" />
                                                        <asp:AsyncPostBackTrigger ControlID="ddlDept" EventName="SelectedIndexChanged" />
                                                    </Triggers>--%>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td ></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div>
                                                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"  CssClass="mGrid" CellPadding="5"  ShowFooter="True" OnRowDataBound="GridView1_RowDataBound" Width="100%">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Department">
                                                                <FooterTemplate>
                                                                    <strong>Total Count</strong>
                                                                </FooterTemplate>
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click" Text='<%# Eval("Department_vName") %>'></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <FooterStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="UPLOAD_iDocType" Visible="False">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblUPLOADiDocType" runat="server" Text='<%# Eval("UPLOAD_iDocType") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="UPLOAD_iDepartment" Visible="False">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbliDepartment" runat="server" Text='<%# Eval("UPLOAD_iDepartment") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Department" Visible="False">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbliDepartmentname" runat="server" Text='<%# Eval("Department_vName") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="No. of Documents">
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblfnofodocs" runat="server"></asp:Label>

                                                                </FooterTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblnoofdocs" runat="server" Text='<%# Eval("NoofDocuments") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <FooterStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="No. of Pages">
                                                                <FooterTemplate>
                                                                    <asp:Label ID="lblfnoofpages" runat="server"></asp:Label>
                                                                </FooterTemplate>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblnoofpages" runat="server" Text='<%# Eval("NoofPages") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <FooterStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>

                                         
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                        <asp:Chart ID="chrtOverall0" runat="server" Palette="None" Height="210px" Width="160px" >
                                                            <Series>
                                                                <%-- <asp:Series Name="Series1"></asp:Series>--%>
                                                            </Series>
                                                            <ChartAreas>
                                                                <asp:ChartArea Name="ChartArea1" BackColor="Transparent" Area3DStyle-Enable3D="false"></asp:ChartArea>
                                                            </ChartAreas>
                                                            <Legends>
                                                                <%--<asp:Legend Docking="Bottom" Title="Count vs Type" TableStyle="Wide"
                                                        BorderDashStyle="Solid" BorderColor="#e8eaf1" TitleSeparator="Line" TitleFont="TimesNewRoman"
                                                        TitleSeparatorColor="#e8eaf1">
                                                    </asp:Legend>--%>
                                                            </Legends>
                                                        </asp:Chart>
                                                    </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                            <td style="width: 60%; height:auto ; ">
                                <div class="GVDiv">
                                    <table style="width: 60%; height: auto">
                                        <tr>
                                            <td colspan="2" style="text-align: Left; height: 25px">
                                                &nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left; width:30%; height: 25px">
                                                &nbsp;</td>
                                            <td style="width: 70%; height: 25px" align="left">
                                                       <div style="padding-left:120px;">
                                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:Chart ID="chrtOverall" runat="server" Palette="None" Height="270px" Width="460px" >
                                                            <Series>
                                                                <%-- <asp:Series Name="Series1"></asp:Series>--%>
                                                            </Series>
                                                            <ChartAreas>
                                                                <asp:ChartArea Name="ChartArea1" BackColor="Transparent" Area3DStyle-Enable3D="false"></asp:ChartArea>
                                                            </ChartAreas>
                                                            <Legends>
                                                                <%--<asp:Legend Docking="Bottom" Title="Count vs Type" TableStyle="Wide"
                                                        BorderDashStyle="Solid" BorderColor="#e8eaf1" TitleSeparator="Line" TitleFont="TimesNewRoman"
                                                        TitleSeparatorColor="#e8eaf1">
                                                    </asp:Legend>--%>
                                                            </Legends>
                                                        </asp:Chart>
                                                    </ContentTemplate>
                                                    <%--<Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="ddlProjType" EventName="SelectedIndexChanged" />
                                                        <asp:AsyncPostBackTrigger ControlID="ddlDept" EventName="SelectedIndexChanged" />
                                                    </Triggers>--%>
                                                </asp:UpdatePanel>
                                                    </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style="text-align: center; height: 25px">
                                                
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">&nbsp;</td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>

                                <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                                <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                                <asp:HiddenField ID="hdnAction" runat="server" Value="" />

                            </td>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </fieldset>
        </div>

    </div>
</asp:Content>

<%--<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

</asp:Content>--%>
