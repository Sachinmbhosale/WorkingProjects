<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkFlowSentMail.aspx.cs"
    Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkFlowSentMail1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Workflow/WorkflowAdmin/WorkflowPDFViewer.ascx" TagName="WorkflowPDFViewer"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../../Scripts/jquery-1.7-vsdoc.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery-1.7.min.js"></script>
    <script type="text/javascript" src="../../Scripts/jquery-1.7.js"></script>
    <script type="text/javascript" language="javascript" src="../../Scripts/AjaxPostScripts.js"></script>
    <link href="../css/jquery.Jcrop.css" rel="stylesheet" type="text/css" />
    <link href="~/Workflow/css/workflow_styles.css" rel="stylesheet" type="text/css" />
    <link id="Link1" rel="shortcut icon" href="../../Images/favicon.ico" type="image/x-icon" />
    <link id="Link2" rel="icon" href="../../Images/favicon.ico" type="image/ico" />
    <script language="javascript" type="text/javascript">
        window.history.forward();
    </script>
    <script language="javascript" type="text/javascript">
        function navigationHandler(action) {



            // Previous page
            if (action.toUpperCase() == 'PREVIOUS') {
                document.getElementById("<%= hdnAction.ClientID %>").value = action;
                document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
            }

            // Next page
            else if (action.toUpperCase() == 'NEXT') {
                document.getElementById("<%= hdnAction.ClientID %>").value = action;
                document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
            }



        }

        function loadImageAndAnnotations(imgPath) {

            // Call annotation library setImage() function to load image to viewer
            setImage(imgPath, true);

            //Fit Width
            setTimeout(function () {
                var zoomSelect = document.getElementById("zoomSelect");
                zoomSelect.selectedIndex = 2;
                $("#zoomSelect").change();
            }, 100);
        }

        function validateMail() {
            var MailTo = $.trim($("#<%= txtMailto.ClientID %>").val());
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            var res = MailTo.split(",");
            var ReturnR = true;
            if (res.length > 0) {
                for (i = 0; i < res.length; i++) {
                    var data = res[i];
                    if (reg.test(data) == false) {
                        ReturnR = false;
                        break;
                    }
                }

            }
            return ReturnR;
        }

        function Validate() {


            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            var Subject = $.trim($("#<%= txtsubject.ClientID %>").val());
            var Message = $.trim($("#<%= txtmessage.ClientID %>").val());

            if (!validateMail()) {
                $(msgControl).html("Please Verify that Correct Email Address is Entered!");
                return false;
            }
            else if (Subject.length < 2) {
                $(msgControl).html("Please enter the  subject!");
                return false;
            }
            else if (Message.length < 2) {
                $(msgControl).html("Please enter the  message!");
                return false;
            }

        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="sm1" runat="server" AsyncPostBackTimeout="1600" />
    <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="GVDiv">
                <table>
                    <tr>
                        <td style="vertical-align: top">
                            <table class="GVDiv" style="width: 530px;">
                                <div id="divMsg" runat="server" style="word-wrap: break-word; color: Red;">
                                </div>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblusers" runat="server" Text="Available Users" CssClass="labelBold"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="GVDiv">
                                        <asp:CheckBoxList ID="chklstLoadUsers" runat="server" AutoPostBack="True" OnSelectedIndexChanged="chklstLoadUsers_SelectedIndexChanged">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label1" runat="server" Text=" Mail To" CssClass="labelBold"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtMailto" runat="server" TextMode="MultiLine" Width="480px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblSubject" runat="server" Text="Subject" CssClass="labelBold"> </asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtsubject" runat="server" Width="480px"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblMessage" runat="server" Text="Message" CssClass="labelBold"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtmessage" runat="server" Height="150px" TextMode="MultiLine" Text="Hi,  You are getting this mail because you requested for Download Document Link"
                                            Width="480px"></asp:TextBox>
                                    </td>
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnSendmail" runat="server" CssClass="btnemail" OnClick="btnSendmail_Click"
                                                OnClientClick="return Validate();" Text="Send Mail" />
                                        </td>
                                    </tr>
                            </table>
                        </td>
                        <td>
                            <uc1:WorkflowPDFViewer ID="WFPDFViewer" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
            <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
            <asp:HiddenField ID="hdnAction" runat="server" Value="" />
            <asp:HiddenField ID="hdnfilepath" runat="server" Value="" />
            <asp:Button ID="btnCallFromJavascript" class="HiddenButton" runat="server" OnClick="btnCallFromJavascript_Click"
                TagName="Read" />
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
