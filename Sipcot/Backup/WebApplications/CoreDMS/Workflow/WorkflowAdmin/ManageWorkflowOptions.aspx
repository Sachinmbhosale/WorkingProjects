<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageWorkflowOptions.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageWorkflowOptions"
    Culture="auto" meta:resourcekey="ManageWorkflowOptions" UICulture="auto" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<%--Calender Scripts Starts --%>
    <script src="../WorkflowJS/jquery-1.4.1.js" type="text/javascript"></script>
    <script src="../WorkflowJS/jquery.dynDateTime.min.js" type="text/javascript"></script>
    <script src="../WorkflowJS/calendar-en.min.js" type="text/javascript"></script>
    <link href="../css/calendar-blue.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%=txtOOOStartDate.ClientID %>").dynDateTime({
                showsTime: true,
                ifFormat: "%d/%m/%Y %H:%M",
                daFormat: "%l;%M %p, %e %m, %Y",
                align: "BR",
                electric: false,
                singleClick: false,
                displayArea: ".siblings('.dtcDisplayArea')",
                button: ".next()"
            });
        });
</script>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=txtOOOEndDate.ClientID %>").dynDateTime({
            showsTime: true,
            ifFormat: "%d/%m/%Y %H:%M",
            daFormat: "%l;%M %p, %e %m, %Y",
            align: "BR",
            electric: false,
            singleClick: false,
            displayArea: ".siblings('.dtcDisplayArea')",
            button: ".next()"
        });
    });
</script>
    <%--Calender Scripts ends --%>
    <script language="javascript" type="text/javascript">
        function validateDate() {

            var stardDate = document.getElementById("<%= txtOOOStartDate.ClientID %>");
            var endDate = document.getElementById("<%= txtOOOEndDate.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            lblMsg.value = "";

            var chkOutofOffice = document.getElementById("<%= chkOutofOffice.ClientID %>");

            if (chkOutofOffice.checked == true) {
                if (stardDate.value == ""
                 || stardDate.value == undefined) {
                    lblMsg.style.color = "Red";
                    lblMsg.innerHTML = "Please specify 'From Date'";
                    return false;
                }

                if (endDate.value == ""
                 || endDate.value == undefined) {
                    lblMsg.style.color = "Red";
                    lblMsg.innerHTML = "Please specify 'To Date'";
                    return false;
                }

                var stDateVal = stardDate.value.replace(":", "/").replace(" ", "/");
                var dayfield = stDateVal.split("/")[0];
                var monthfield = stDateVal.split("/")[1];
                var yearfield = stDateVal.split("/")[2];
                var hhfield = stDateVal.split("/")[3];
                var mmfield = stDateVal.split("/")[4];
                
                var stDt = new Date(yearfield, monthfield - 1, dayfield, hhfield, mmfield);


                var enDateVal = endDate.value.replace(":", "/").replace(" ", "/");
                dayfield = enDateVal.split("/")[0];
                monthfield = enDateVal.split("/")[1];
                yearfield = enDateVal.split("/")[2];
                hhfield = enDateVal.split("/")[3];
                mmfield = enDateVal.split("/")[4];

                var enDt = new Date(yearfield, monthfield - 1, dayfield, hhfield, mmfield);


                if (enDt < stDt) {
                    lblMsg.style.color = "Red";
                    lblMsg.innerHTML = "'From Date' should be less than 'To Date'";
                    return false;
                }
            }
            return true;
        }

        function CheckChange() {

            var stardDate = document.getElementById("<%= txtOOOStartDate.ClientID %>");
            var endDate = document.getElementById("<%= txtOOOEndDate.ClientID %>");
            var chkOutofOffice = document.getElementById("<%= chkOutofOffice.ClientID %>");

            if (chkOutofOffice.checked == false) {
                stardDate.value = "";
                endDate.value = "";
            }

            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="GVDiv" style="height: 400px">
        <h3>
            <asp:Label ID="lblWorkflowOptionHeader" runat="server" Text="Workflow Options" meta:resourcekey="lblWorkflowOptionHeader"></asp:Label></h3>
        <table>
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblSetOutOfOffice" runat="server" Text="Out of Office" meta:resourcekey="lblSetOutOfOffice"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkOutofOffice" runat="server" Checked="false" onchange="return CheckChange();" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblOOOStartDate" runat="server" Text="From" meta:resourcekey="lblOOOStartDate"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtOOOStartDate" runat="server"></asp:TextBox>
                     <img src="../images/cal.gif" />
                    <%--<asp:CalendarExtender runat="server" ID="calOOOStartDate" Format="dd/MM/yyyy hh:mm"
                        TargetControlID="txtOOOStartDate">
                    </asp:CalendarExtender>--%>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblOOOEndDate" runat="server" Text="To" meta:resourcekey="lblOOOEndDate"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtOOOEndDate" runat="server"></asp:TextBox>
                     <img src="../images/cal.gif" />
                   <%-- <asp:CalendarExtender runat="server" ID="calOOOEndDate" Format="dd/MM/yyyy hh:mm"
                        TargetControlID="txtOOOEndDate">
                    </asp:CalendarExtender>--%>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center" style="padding-top:10px">
                    <asp:Button ID="btnSaveOptions" runat="server" Text="Save" OnClientClick="return validateDate();"  TagName="Add" 
                        meta:resourcekey="btnSaveOptions" OnClick="btnSaveOptions_Click" CssClass="btnsave" />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
