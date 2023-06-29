<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridTest.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.GridTest" 
    MasterPageFile="~/SecureMaster.Master"%>

<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">


   <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.5.0/Chart.min.js"></script>
  <%--       <script language="javascript" type="text/javascript">
          var xValues = ["Italy", "France", "Spain", "USA", "Argentina"];
          var yValues = [55, 49, 44, 24, 45];
          var barColors = ["red", "green", "blue", "orange", "brown"];

          new Chart("myChart", {
              type: "bar",
              data: {
                  labels: xValues,
                  datasets: [{
                      backgroundColor: barColors,
                      data: yValues
                  }]
              },
              options: {
                  legend: { display: false },
                  title: {
                      display: true,
                      text: "World Wine Production 2018"
                  }
              }
          });
</script>--%>

       <script src="js/Chart.js" type="text/javascript"></script>
    <script src="js/jquery-2.1.4.min.js" type="text/javascript"></script>
    
<%--<script type="text/javascript">

    //$(document).ready(function () {

    //    $("btnGeneratePieChart").on('click', function (e) {
    //        e.preventDefault();
    //        var gData = [];
    //        gData[0] = $("#ddlyear").val();
    //        //            gData[1] = $("#ddlMonth").val();

    //        var jsonData = JSON.stringify({
    //            gData: gData
    //        });
    //        $.ajax({
    //            type: "POST",
    //            url: "Deafult.aspx/getTrafficSourceData",
    //            data: jsonData,
    //            contentType: "application/json; charset=utf-8",
    //            dataType: "json",
    //            success: OnSuccess_,
    //            error: OnErrorCall_
    //        });

    //        function OnSuccess_(response) {
    //            var aData = response.d;
    //            var arr = [];
    //            $.each(aData, function (inx, val) {
    //                var obj = {};
    //                obj.color = val.color;
    //                obj.value = val.value;
    //                obj.label = val.label;
    //                arr.push(obj);
    //            });
    //            var ctx = $("#myChart").get(0).getContext("2d");
    //            var myPieChart = new Chart(ctx).Pie(arr);
    //        }

    //        function OnErrorCall_(response) { }
    //        e.preventDefault();
    //    });
    //});

</script>--%>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <asp:GridView ID="GridView1" runat="server" AllowSorting="True" AllowPaging="True"  OnPageIndexChanging="GridView1_PageIndexChanging"  OnRowDataBound="GridView1_RowDataBound" OnSorting="GridView1_Sorting"  >
         </asp:GridView>
       <div>
        <asp:Literal ID="ltChart" runat="server"></asp:Literal>
    </div>


    </asp:Content>

<%--<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
       <%-- <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" OnPageIndexChanging="GridView1_PageIndexChanging" OnRowDataBound="GridView1_RowDataBound" OnSorting="GridView1_Sorting">
        </asp:GridView>
         
    </form></body></html>--%>