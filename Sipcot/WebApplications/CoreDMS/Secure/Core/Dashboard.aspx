<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.Dashboard" %>


<%@ Register Assembly="RJS.Web.WebControl.PopCalendar" Namespace="RJS.Web.WebControl" TagPrefix="rjs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%--<asp:Content ID="Content1" ContentPlaceHolderID="test" runat="server">
</asp:Content>--%>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">



    <style type="text/css">
        .GVDiv {
            margin-top: 0px;
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

    <div class="row">
    </div>

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


        <%--added for chart display--%>

        <%--end here--%>
    </div>




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
                <table style="width: 65%; padding-left: 100px; display: none" class="table table-bordered">
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
            <div class="form-group col-lg-12 col-md-12">

                <%--chart code--%>

                <%--<div id="mychartdiv" style="display: block; box-sizing: border-box; height: 400px; width: 400px;" class="parent">--%>
                <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="From"></asp:Label>
                <asp:TextBox ID="txtCreatedDateFrom" runat="server" ToolTip="Date Format - dd/mm/yyyy"></asp:TextBox>

                <asp:CalendarExtender ID="CalendarExtender1" runat="server" Enabled="True" PopupButtonID="btnCalenderStart"
                    Format="yyyy-MM-dd" TargetControlID="txtCreatedDateFrom">
                </asp:CalendarExtender>
                <asp:ImageButton ID="btnCalenderStart" runat="server" CssClass="ImageButtonStyle"
                    Height="16px" ImageUrl="~/Images/CalenderImage.png" Width="16px" />



                <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="To"></asp:Label>

                <asp:TextBox ID="txtCreatedDateTo" runat="server" ToolTip="Date Format - dd/mm/yyyy"></asp:TextBox>
                <asp:CalendarExtender ID="txtCreatedDateTo_CalendarExtender" runat="server" PopupButtonID="btnCalenderTo"
                    Format="yyyy-MM-dd" Enabled="True" TargetControlID="txtCreatedDateTo">
                </asp:CalendarExtender>
                <asp:ImageButton ID="btnCalenderTo" runat="server" CssClass="ImageButtonStyle" Height="16px"
                    ImageUrl="~/Images/CalenderImage.png" Width="16px" />

                <%--<input type="button" id="ShowGarph" value="Show Graph"  runat="server" Onclick="" />--%>
                <asp:Button ID="btnShowGarph" Text="Apply" runat="server" CssClass="btn btn-primary" OnClick="btnShowGarph_Click" />
                <div id="mychartdiv" class="parent">


                    <div class="child">
                        <canvas style="height: 350px; width: 350px" id="myChart"></canvas>
                    </div>



                    <div class="child">
                        <canvas style="height: 350px; width: 350px" id="myChart2"></canvas>
                    </div>


                    <div class="child">
                        <canvas style="height: 350px; width: 350px" id="chartJSContainer"></canvas>
                    </div>
                    <asp:Literal ID="ltChartData" runat="server"></asp:Literal>

                </div>
                <%--end here--%>
                <table style="width: 100%; visibility: hidden">
                    <tr>

                        <td style="width: 80%; height: 400px; padding-left: 120px;">
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
        </fieldset>
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
            var ctx = document.getElementById('chartJSContainer').getContext('2d');
            new Chart(ctx, options);



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
            ////////end here

            ////////////bar chart
            var ctx2 = document.getElementById("myChart").getContext("2d");
            var myChart = new Chart(ctx2, {
                type: 'bar',
                data: {
                    labels: chartDept,//["Jan", "Feb", "Mar", "Apr", "May"],
                    datasets: [{
                        label: 'No of Documents ',
                        data: chart_No_Of_Doc,
                        backgroundColor: 'rgb(255, 99, 132)',


                    }
                        ,
                    {
                        label: 'Total Page Count',
                        data: chart_page_Counts,//[23, 45, 17, 55, 31],
                        backgroundColor: 'rgb(54, 162, 235)',

                    }

                        ,
                    {
                        label: 'Total Size ',
                        data: [13, 7, 18],//[23, 45, 17, 55, 31],
                        backgroundColor: 'rgb(255, 205, 86)',

                    }]
                },
                options: {
                    scales: {

                        yAxes: [{
                            ticks: {
                                beginAtZero: true
                            }
                        }]
                    },
                    plugins: {
                        legend: {
                            position: 'bottom'
                        },
                        title: {
                            display: true,
                            text: 'Dept - Doc Details',
                            padding: {
                                top: 10,
                                bottom: 30
                            }
                        },
                        datalabels: [ChartDataLabels]
                        //{ // This code is used to display data values
                        //    anchor: 'end',
                        //    align: 'top',
                        //    formatter: Math.round,
                        //    font: {
                        //        weight: 'bold',
                        //        size: 16
                        //    }
                        //}
                    }
                }
            });



            /////////////end here
            //end here
        }

        function createChart_old() {

            //chartDept = [Legal, Finance, HRD];

            //chart_No_Of_Doc = ['17', '5', '3'];
            //chart_page_Counts = ['434', '69', '4'];


            //chartDept = ['Legal', 'Finance', 'HRD'];
            //chart_No_Of_Doc = ['17', '5', '3'];
            //chart_page_Counts = ['434', '69', '4'];

            var ctx = document.getElementById("myChart").getContext("2d");
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: chartDept,//["Jan", "Feb", "Mar", "Apr", "May"],
                    datasets: [{
                        label: 'No of Documents ',
                        data: chart_No_Of_Doc,
                        backgroundColor: 'rgb(255, 99, 132)',


                    }
                        ,
                    {
                        label: 'Total Page Count',
                        data: chart_page_Counts,//[23, 45, 17, 55, 31],
                        backgroundColor: 'rgb(54, 162, 235)',

                    }

                        ,
                    {
                        label: 'Total Size ',
                        data: [13, 7, 18],//[23, 45, 17, 55, 31],
                        backgroundColor: 'rgb(255, 205, 86)',

                    }]
                },
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero: true
                            }
                        }]
                    },
                    plugins: {
                        title: {
                            display: true,
                            text: 'Dept - Doc Details',
                            padding: {
                                top: 10,
                                bottom: 30
                            }
                        },
                        datalabels: [ChartDataLabels]
                        //{ // This code is used to display data values
                        //    anchor: 'end',
                        //    align: 'top',
                        //    formatter: Math.round,
                        //    font: {
                        //        weight: 'bold',
                        //        size: 16
                        //    }
                        //}
                    }
                }
            });


            //added by prathamesh for peichart
            debugger;
            var ctx1 = document.getElementById('myChart1').getContext('2d');

            chart1 = new Chart(ctx1, {
                // The type of chart we want to create
                type: 'doughnut',



                // The data for our dataset
                data: {
                    labels: DeptName,
                    datasets: [{

                        label: 'User Count',
                        //   data: mv,//vData.Pune,//JSON.stringify(vData.Pune),//[3,5,8],//JSON.stringify(vData.Pune),//[18, 12, 6, 9, 12, 3, 9],
                        data: DeptCount, //[12, 3, 9],
                        backgroundColor: [
                            'rgb(255, 99, 132)',
                            'rgb(54, 162, 235)',
                            'rgb(255, 205, 86)',
                            'rgb(0, 204, 0)',
                            'rgb(255, 163, 26)'
                        ]
                    }

                    ]

                },


                options: {
                    plugins: {
                        title: {
                            display: true,
                            text: 'Dept wise users',
                            padding: {
                                top: 10,
                                bottom: 30
                            }
                        }
                    }
                }



            });

            debugger;
            var ctx2 = document.getElementById('myChart2').getContext('2d');
            chart2 = new Chart(ctx2, {
                // The type of chart we want to create
                type: 'doughnut',


                // The data for our dataset
                data: {
                    labels: [
                        'Active',
                        'InActive'

                    ],
                    datasets: [{

                        label: userStatus,
                        //   data: mv,//vData.Pune,//JSON.stringify(vData.Pune),//[3,5,8],//JSON.stringify(vData.Pune),//[18, 12, 6, 9, 12, 3, 9],
                        data: user_count,
                        backgroundColor: [
                            'rgb(255, 99, 132)',
                            'rgb(54, 162, 235)'

                        ]
                    }
                    ]

                },

                options: {
                    plugins: {
                        title: {
                            display: true,
                            text: 'Users Details',
                            padding: {
                                top: 10,
                                bottom: 30
                            }
                        }
                    }
                }

            });

            //end here
        }
    </script>


</asp:Content>

<%--<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

</asp:Content>--%>
