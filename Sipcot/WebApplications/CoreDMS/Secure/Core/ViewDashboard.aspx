<%@ Page Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true" CodeBehind="ViewDashboard.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.ViewDashboard" %>


<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl" TagPrefix="rjs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">



    <style type="text/css">
        .count-box {
            box-shadow: -10px -5px 40px 0 rgba(0, 0, 0, 0.1);
            padding: 30px;
            width: 100%;
            border-radius: 20px;
        }

        .GVDiv {
            margin-top: 0px;
        }

         .labelAlign {
               float: right;
        }

        .panel-tile {
            position: relative;
            overflow: hidden;
        }

        .panel {
            position: relative;
            margin-bottom: 27px;
            background-color: #ffffff;
            border-radius: 3px;
        }

        .text-center {
            text-align: center !important;
        }

        .parent {
            margin: 1rem;
            padding: 2rem 2rem;
            text-align: center;
        }

        .child {
            display: inline-block;
            padding: 1rem 1rem;
            vertical-align: middle;
        }
    </style>


    <link href="<%= Page.ResolveClientUrl("~/bootstrap.css") %>" rel="stylesheet" type="text/css" />
    <link href="<%= Page.ResolveClientUrl("~/bootstrap.min.css") %>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery.min.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/bootstrap.min.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/chart.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/chartjs-plugin-datalabels.min.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/Chart.bundle.js") %>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

    <ul class="nav nav-tabs">

        <li class="active"><a data-toggle="tab" href="#home">Dashboard</a></li>
        <li><a data-toggle="tab" href="#menu1">User Details</a></li>

        <li style="visibility:hidden"><a data-toggle="tab" href="#tab3">Department wise Details</a></li>


    </ul>


    <div class="tab-content">


        <div id="menu1" class=" tab-pane fade ">


            <div class="child" style="padding-left: 30%;">
                <canvas style="height: 450px; width: 450px" id="myChart2"></canvas>
            </div>


        </div>


        <div id="home" class=" tab-pane fade in active">


            <div  style="padding-left: 10%;">
                <%--<canvas id="myChart" style="height: 400px; width: 650px"></canvas>   --%>

                <div class="row no-gutters">
                    <div class="section-title col-lg-12 col-md-12">
                        <div class="count-box">
                            <p style="font-size: 24px; margin-bottom: 16px;padding-left: 45%;">DASHBOARD</p>


                            <div class="row">
                                <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                                    <div class="count-box">
                                        
                                        <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                            <asp:Label ID="lblDept0" runat="server"></asp:Label>
                                        </span>

                                        </h4>
                                        <span>Docs Count:-<asp:Label  ID="LblDept0_Doc_count" runat="server"></asp:Label></span>
                                        <br /><span>Page Count:-<asp:Label ID="lblPagecount0" runat="server"></asp:Label></span>
                                    </div>
                                </div>

                     <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                         <div class="count-box">

                              <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                 <asp:Label ID="lblDept1" runat="server"></asp:Label>

                             </span></h4>

                             <span>Docs Count:-    <asp:Label CssClass="labelAlign" ID="lblDept1_Doc_count" runat="server"></asp:Label></span>
                            
                             <br /><span>Page Count:
                                  <asp:Label ID="lblPagecount1" CssClass="labelAlign" runat="server"></asp:Label>
                             </span>
                         </div>
                     </div>
                                <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                                    <div class="count-box">
                                        
                                        <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                            <asp:Label ID="lblDept2" runat="server"></asp:Label>

                                        </span></h4>
                                        
                                        <span>Docs Count:-<asp:Label CssClass="labelAlign" ID="lblDept2_Doc_count" runat="server"></asp:Label></span>
                                        
                                            
                                        
                                        
                                        <br /><span>Page Count:
                                  <asp:Label ID="lblPageCount2" CssClass="labelAlign" runat="server"></asp:Label>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                                    <div class="count-box">
                                        <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                            <asp:Label ID="lblDept3" runat="server"></asp:Label>

                                        </span></h4>
                                        
                                        <span>Docs Count:- <asp:Label ID="lblDoc3"  CssClass="labelAlign" runat="server"></asp:Label></span>
                                        
                                        
                                        <br /><span>Page Count:
                                  <asp:Label ID="lblpagecount3" CssClass="labelAlign" runat="server"></asp:Label>
                                        </span>
                                    </div>
                                </div>
                           




</div>
<br />
<div class="row">
     <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                                    <div class="count-box">
                                        <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                            <asp:Label ID="lblDept4" runat="server"></asp:Label>

                                        </span></h4>
                                        
                                        <span>Docs Count:-<asp:Label ID="lblDoc4" CssClass="labelAlign" runat="server"></asp:Label></span>
                                        
                                            
                                        
                                        
                                        <br /><span>Page Count:
                                  <asp:Label ID="lblDoccount4" CssClass="labelAlign" runat="server"></asp:Label>
                                        </span>
                                    </div>
                                </div>
                            


<div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                                    <div class="count-box">
                                        <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                            <asp:Label ID="Lbldept5" runat="server"></asp:Label>

                                        </span>

                                        </h4>
                                        
                                        <span>Docs Count:-</span>
                                        <span data-toggle="counter-up">
                                            <asp:Label ID="LblDoc5" runat="server"></asp:Label>
                                        </span>
                                        
                                        <br /><span>Page Count:
                                  <asp:Label ID="lblpagecount5" runat="server"></asp:Label>
                                        </span>
                                    </div>
                                </div>


<div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                         <div class="count-box">
                             <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                 <asp:Label ID="lbldept6" runat="server"></asp:Label>

                             </span></h4>
                             
                             <span>Docs Count:-</span>
                             <span data-toggle="counter-up">
                                 <asp:Label ID="lbldoc6" runat="server"></asp:Label>
                             </span>
                             
                             <br /><span>Page Count:
                                  <asp:Label ID="lbldoccount6" runat="server"></asp:Label>
                             </span>
                         </div>
                     </div>


                                <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                                    <div class="count-box">
                                        <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                            <asp:Label ID="lblDept7" runat="server"></asp:Label>

                                        </span></h4>
                                        
                                        <span>Docs Count:-</span>
                                        <span data-toggle="counter-up">
                                            <asp:Label ID="lblDoc7" runat="server"></asp:Label>
                                        </span>
                                        
                                        <br /><span>Page Count:
                                  <asp:Label ID="lblpagecount7" runat="server"></asp:Label>
                                        </span>
                                    </div>
                                </div>


</div>

<br />


                            <div class="row">
                                
                           
                     
                                <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                                    <div class="count-box">
                                        <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                            <asp:Label ID="lblDept8" runat="server"></asp:Label>

                                        </span></h4>
                                        
                                        <span>Docs Count:-</span>
                                        <span data-toggle="counter-up">
                                            <asp:Label ID="lbldoc8" runat="server"></asp:Label>
                                        </span>
                                        
                                        <br /><span>Page Count:
                                  <asp:Label ID="lblpagecount8" runat="server"></asp:Label>
                                        </span>
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                                    <div class="count-box">
                                        <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                            <asp:Label ID="lblDept9" runat="server"></asp:Label>

                                        </span></h4>
                                        
                                        <span>Docs Count:-</span>
                                        <span data-toggle="counter-up">
                                            <asp:Label ID="lbldoc9" runat="server"></asp:Label>
                                        </span>
                                        
                                        <br /><span>Page Count:
                                  <asp:Label ID="lblpagecount9" runat="server"></asp:Label>
                                        </span>
                                    </div>
                                </div>


 <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                                    <div class="count-box">
                                        <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                            <asp:Label ID="lblDept10" runat="server"></asp:Label>

                                        </span>

                                        </h4>
                                        <span>Docs Count:-</span>
                                        <span data-toggle="counter-up">
                                            <asp:Label ID="lblDoc10" runat="server"></asp:Label>
                                        </span>
                                        
                                        <br /><span>Page Count:
                                  <asp:Label ID="lblpagecount10" runat="server"></asp:Label>
                                        </span>
                                    </div>
                                </div>
                               
                     <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color: #f5f5f5; border-radius: 20px">
                         <div class="count-box">
                             
                             <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                 <asp:Label ID="lbldept11" runat="server"></asp:Label>

                             </span></h4>
                             <span>Docs Count:-</span>

                             <span data-toggle="counter-up">
                                 <asp:Label ID="lbldoc11" runat="server"></asp:Label>
                             </span>
                             
                             <br /><span>Page Count:
                                  <asp:Label ID="lblpagecount11" runat="server"></asp:Label>
                             </span>
                         </div>
                     </div>


</div>
<br />

                            <div class="row">


<div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color:#f5f5f5;border-radius:20px">
                        <div class="count-box">

                           <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                               <asp:Label ID="lbldept13" runat="server">Project  II</asp:Label>

                               </span> </h4>

                           <span>Docs Count:- </span>
                           <span data-toggle="counter-up">
                               <asp:Label ID="lbldoc13" runat="server">6690</asp:Label>
                           </span>
<br />
                            <span>Page Count:
                                  <asp:Label ID="lblpagecount13" runat="server">890990</asp:Label>
                            </span>
                        </div>
                     </div>
                     <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color:#f5f5f5;border-radius:20px">
                        <div class="count-box">

                           <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                               <asp:Label ID="lblDept14" runat="server">INTERNAL AUDIT</asp:Label>
                               </span> </h4>
                           <span>Docs Count:-</span>
                           <span data-toggle="counter-up">
                               <asp:Label ID="lbldoc14" runat="server">222</asp:Label>
                           </span>
<br />
                            <span>Page Count:
                                  <asp:Label ID="lblPagecount14" runat="server">23996</asp:Label>
                            </span>
                        </div>
                     </div>  



 <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color:#f5f5f5;border-radius:20px">
                        <div class="count-box">

                           <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                               <asp:Label ID="lblDept15" runat="server">Environment Cell</asp:Label>

                               </span>

                           </h4>

                           <span>Docs Count:-</span>
                           <span data-toggle="counter-up">
                               <asp:Label ID="lbldoc15" runat="server">114</asp:Label>
                           </span>
<br />
                             <span>Page Count:
                                  <asp:Label ID="lblpagecount15" runat="server">26827</asp:Label>
                            </span>
                        </div>
                     </div>
		            <div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color:#f5f5f5;border-radius:20px">
                        <div class="count-box">

                           <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                               <asp:Label ID="lblDept16" runat="server">CS</asp:Label>

                               </span> </h4>

                           <span>Docs Count:-</span>
                           <span data-toggle="counter-up">
                               <asp:Label ID="lblDoc16" runat="server">113</asp:Label>
                           </span>
<br />
                            <span>Page Count:
                                  <asp:Label ID="lblpagecount16" runat="server">20057</asp:Label>
                            </span>
                        </div>
                     </div>
</div>

<br />

<div class="row">
<div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color:#f5f5f5;border-radius:20px">
                        <div class="count-box">

                           <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                               <asp:Label ID="lbldept17" runat="server">Planning</asp:Label>

                               </span> </h4>

                           <span>No of Docs:-</span>
                           <span data-toggle="counter-up">
                               <asp:Label ID="lbldoc17" runat="server">21</asp:Label>
                           </span>
<br />

                            <span>Page Count:
                                  <asp:Label ID="lblpagecount17" runat="server">1503</asp:Label>
                            </span>
                        </div>
                     </div>

<div class="col-lg-3 col-md-6 d-md-flex align-items-md-stretch" style="background-color:#f5f5f5;border-radius:20px">
                       
                                                              
                                    <div class="count-box">

                                        <h4><span class="h5 ml-0 mt-4" style="font-size: 20px;">
                                            <asp:Label ID="Label16total" runat="server"> Total</asp:Label>

                                        </span></h4>

                                        <span>Total Docs Count:-</span>
                                        <span data-toggle="counter-up">
                                            <asp:Label ID="lblTotalDocs" runat="server">19941</asp:Label>
                                        </span>
<br />
                                        <span>Total Page Count:
                                  <asp:Label ID="Lbltotalpages" runat="server">3050306</asp:Label>
                                        </span>
                                    </div>
                                </div>






                      
                  


                        </div>
                    </div>
                </div>
            </div>



            <%--<canvas id="myChart" width="5" height="5"></canvas>   --%>

            <table style="width: 50%; visibility: hidden">
                <tr>

                    <td style="width: 80%; height: 400px; padding-left: 10px;">
                        <div class="GVDiv">
                            <table style="width: 100%; height: auto">
                                <tr style="display: none">
                                    <td style="text-align: center;">
                                        <asp:Label ID="lable1" runat="server" Text="Overall total count:" Font-Bold="True"></asp:Label>
                                    </td>
                                </tr>
                                <%--<tr>
                                        <td style="text-align: center; height: 25px; display: none">
                                            <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                                <ContentTemplate>
                                                    <asp:Label ID="lblChart1ErrMsg" Text="*" runat="server" Visible="false" ForeColor="Red"></asp:Label>
                                                </ContentTemplate>
                                                <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="ddlProjType" EventName="SelectedIndexChanged" />
                                                        <asp:AsyncPostBackTrigger ControlID="ddlDept" EventName="SelectedIndexChanged" />
                                                    </Triggers>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>--%>
                                <tr>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>
                                        <div>
                                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="mGrid" CellPadding="5" ShowFooter="True" OnRowDataBound="GridView1_RowDataBound" Width="100%">
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
                                        <%--<asp:Chart ID="chrtOverall0" runat="server" Palette="None" Height="210px" Width="160px" >
                                                            <Series>
                                                              
                                                            </Series>
                                                            <ChartAreas>
                                                                <asp:ChartArea Name="ChartArea1" BackColor="Transparent" Area3DStyle-Enable3D="false"></asp:ChartArea>
                                                            </ChartAreas>
                                                            <Legends>
                                                  
                                                            </Legends>
                                                        </asp:Chart>--%>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                    <td style="width: 5%; height: auto; display: none">
                        <div class="GVDiv">
                            <table style="width: 30%; height: auto">
                                <tr>
                                    <td colspan="2" style="text-align: Left; height: 25px">&nbsp;</td>
                                </tr>
                                <tr>
                                    <td style="text-align: left; width: 30%; height: 25px">&nbsp;</td>
                                    <td style="width: 30%; height: 25px" align="left">
                                        <div style="padding-left: 120px;">
                                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <%--<asp:Chart ID="chrtOverall" runat="server" Palette="None" Height="270px" Width="460px" >
                                                            <Series>
                                                               
                                                            </Series>
                                                            <ChartAreas>
                                                                <asp:ChartArea Name="ChartArea1" BackColor="Transparent" Area3DStyle-Enable3D="false"></asp:ChartArea>
                                                            </ChartAreas>
                                                            <Legends>
                                               
                                                            </Legends>
                                                        </asp:Chart>--%>
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
                                    <td colspan="2" style="text-align: center; height: 25px"></td>
                                </tr>
                                <tr>
                                    <td colspan="2"></td>
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
        </div>
        <div id="tab3" class=" tab-pane fade" style="visibility:hidden">

            <div class="child" style="padding-left: 30%;">
                <canvas style="height: 450px; width: 450px" id="chartJSContainer"></canvas>
            </div>
        </div>

        <asp:Literal ID="ltChartData" runat="server"></asp:Literal>

    </div>

    <script type="text/javascript">
        var chartData;
        var chartDept;
        var chart_No_Of_Doc;
        var chart_page_Counts;

        var DeptCount;
        var DeptName;


        var userStatus;
        var user_count;


        $(document).ready(function () {
            createChart();
            //creatchartwithHeaders();





        });


        // A $( document ).ready() block.
        //$(document).ready(function () {
        //    alert('ready');

        //    var empId = '304';

        //    $.ajax({
        //        url: '/secure/Core/Dashboard.aspx/GetDashboardTotalCount',
        //        method: 'Post',
        //        contentType: "application/json",
        //        data: '{employeeId:' + empId + '}',
        //        dataType: "json",
        //        success: function (data) {

        //            alert(JSON.stringify(data));
        //        },
        //        error: function (err) {
        //            alert(335);
        //            alert(JSON.stringify(err));
        //        }
        //    });


        //    createChart();
        //});

        function creatchartwithHeaders() {
            var ctx = document.getElementById('myChart').getContext('2d');
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: ['Label 1', 'Label 2', 'Label 3'],
                    datasets: [{
                        label: 'My Dataset',
                        data: [10, 20, 30],
                        backgroundColor: [
                            'rgba(255, 99, 132, 0.2)',
                            'rgba(54, 162, 235, 0.2)',
                            'rgba(255, 206, 86, 0.2)'
                        ],
                        borderColor: [
                            'rgba(255, 99, 132, 1)',
                            'rgba(54, 162, 235, 1)',
                            'rgba(255, 206, 86, 1)'
                        ],
                        borderWidth: 1
                    }]
                },
                options: {}
            });
            Chart.plugins.register({
                afterDatasetsDraw: function (chart) {
                    var ctx = chart.ctx;
                    chart.data.datasets.forEach(function (dataset, i) {
                        var meta = chart.getDatasetMeta(i);
                        if (!meta.hidden) {
                            meta.data.forEach(function (element, index) {
                                // Draw the text in black, with the specified font
                                ctx.fillStyle = 'rgb(0, 0, 0)';
                                var fontSize = 16;
                                var fontStyle = 'normal';
                                var fontFamily = 'Helvetica Neue';
                                ctx.font = Chart.helpers.fontString(fontSize, fontStyle, fontFamily);

                                // Just naively convert to string for now
                                var dataString = dataset.data[index].toString();

                                // Make sure alignment settings are correct
                                ctx.textAlign = 'center';
                                ctx.textBaseline = 'middle';

                                var padding = 5;
                                var position = element.tooltipPosition();
                                ctx.fillText(dataString, position.x, position.y - (fontSize / 2) - padding);
                            });
                        }
                    });
                }
            });

        }
        function createChart() {

            //testing doughnut chart
            var options = {
                type: 'doughnut',
                data: {
                    labels: DeptName,//["Red", "Blue", "Yellow", "Green", "Purple", "Orange"],
                    datasets: [{
                        label: 'User Count',
                        data: DeptCount,//[12, 19, 3, 5, 2, 3],
                        backgroundColor: [
                            'rgb(255, 99, 132)',
                            'rgb(54, 162, 235)',
                            'rgb(255, 205, 86)',
                            'rgb(0, 204, 0)',

                            'rgb(255, 99, 132)',
                            'rgb(54, 162, 235)',
                            'rgb(255, 205, 86)',
                            'rgb(0, 204, 0)',



                            'rgb(255, 163, 26)'

                        ],
                    }]
                },
                options: {
                    plugins: {

                        title: {
                            display: true,
                            text: 'Dept Wise User Details',
                            padding: {
                                top: 10,
                                bottom: 30
                            }
                        },
                        legend: {
                            position: 'right',
                            labels: {
                                generateLabels: (chart) => {
                                    const datasets = chart.data.datasets;
                                    return datasets[0].data.map((data, i) => ({
                                        text: `${chart.data.labels[i]} = ${data}`,
                                        fillStyle: datasets[0].backgroundColor[i],
                                        index: i
                                    }))
                                }
                            }
                        }
                    }
                }
            }
            var ctxd = document.getElementById('chartJSContainer').getContext('2d');
            new Chart(ctxd, options);


            ////////////new chart
            var options_ = {
                type: 'doughnut',
                data: {
                    labels: ['Active', 'inactive'],//userStatus,//["Red", "Blue", "Yellow", "Green", "Purple", "Orange"],
                    datasets: [{
                        label: 'User Count',
                        data: user_count,//[12, 19, 3, 5, 2, 3],
                        backgroundColor: [
                            'rgb(255, 99, 132)',
                            'rgb(54, 162, 235)'

                        ],
                    }]
                },
                options: {
                    plugins: {

                        title: {
                            display: true,
                            text: 'User Details',
                            padding: {
                                top: 10,
                                bottom: 30
                            }
                        },
                        legend: {
                            position: 'right',
                            labels: {
                                generateLabels: (chart) => {
                                    const datasets = chart.data.datasets;
                                    return datasets[0].data.map((data, i) => ({
                                        text: `${chart.data.labels[i]} = ${data}`,
                                        fillStyle: datasets[0].backgroundColor[i],
                                        index: i
                                    }))
                                }
                            }
                        }
                    }
                }
            }
            var ctx1 = document.getElementById('myChart2').getContext('2d');
            new Chart(ctx1, options_);


            //doughnut chart ends here
            //testing bar chart
            var ctx = document.getElementById("myChart");
            debugger;
            var data = {
                labels: DeptName,//["2 Jan", "9 Jan", "16 Jan", "23 Jan", "30 Jan", "6 Feb", "13 Feb"],
                datasets: [{
                    label: 'No of Documents',
                    data: chart_No_Of_Doc,//[15, 87, 56, 50, 88, 60, 45],//
                    backgroundColor: "#4082c4"
                }

                    //added by prathamesh for extra dataset
                    , {
                    label: 'page count',
                    data: chart_page_Counts,//[130, 51, 91, 80, 68, 70, 59],//
                    backgroundColor: "#fdb813"
                },
                {
                    label: 'Total Size',
                    data: [13, 4, 1],//chart_page_Counts,//[130, 51, 91, 80, 68, 70, 59],//
                    backgroundColor: "#575cao"
                }

                    //ends here
                ]
            }
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: data,
                options: {
                    //"hover": {
                    //  "animationDuration": 0
                    //},
                    "animation": {
                        "duration": 1,
                        "onComplete": function () {
                            var chartInstance = this.chart,
                                ctx = chartInstance.ctx;

                            ctx.font = Chart.helpers.fontString(Chart.defaults.global.defaultFontSize, Chart.defaults.global.defaultFontStyle, Chart.defaults.global.defaultFontFamily);
                            ctx.textAlign = 'center';
                            ctx.textBaseline = 'bottom';

                            this.data.datasets.forEach(function (dataset, i) {
                                var meta = chartInstance.controller.getDatasetMeta(i);
                                meta.data.forEach(function (bar, index) {
                                    var data = dataset.data[index];
                                    ctx.fillText(data, bar._model.x, bar._model.y - 5);
                                });
                            });
                        }
                    },
                    //legend: {
                    //  "display": false
                    //},
                    tooltips: {
                        "enabled": true
                    },
                    // scales: {
                    //   yAxes: [{
                    //     display: false,
                    //     gridLines: {
                    //       display: false
                    //     },
                    //     ticks: {
                    //       max: Math.max(...data.datasets[0].data) + 10,
                    //       display: false,
                    //       beginAtZero: true
                    //     }
                    //   }],
                    //   xAxes: [{
                    //     gridLines: {
                    //       display: false
                    //     },

                    //ticks: {
                    //       beginAtZero: true
                    //     }
                    //   }]
                    // }
                    scales: {
                        y: {
                            beginAtZero: true,
                            max: 100
                        }
                    }
                }
            });





            //bar chart testing close here

            /////////////end here
            //end here
        }
    </script>
</asp:Content>

