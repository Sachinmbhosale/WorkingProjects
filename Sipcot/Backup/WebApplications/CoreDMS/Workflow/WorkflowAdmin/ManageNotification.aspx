<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageNotification.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageNotification"
    Culture="auto" meta:resourcekey="ManageNotification" UICulture="auto" EnableEventValidation="false" %>

<%@ Register TagPrefix="WF" TagName="WorkFlowWizard" Src="WorkFlowWizardMenu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            var StatusName = document.getElementById("<%= ddl_Status.ClientID %>").value;
            var Category = document.getElementById("<%= ddl_Category.ClientID %>").value;
            var StatusActive = document.getElementById("<%= chkActive.ClientID %>").checked
            var strTAT = document.getElementById("<%= txt_TAT.ClientID %>").value;

            if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value != "") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReload("0", StatusName, Category, StatusActive, strTAT);
            }
        });

        function ShowMDReload(StatusID, StatusName, Category, StatusActive, strTAT) {

            var ddlStatusName = document.getElementById("<%= ddl_Status.ClientID %>");
            var ddlCategory = document.getElementById("<%= ddl_Category.ClientID %>");
            var txtTAT = document.getElementById("<%= txt_TAT.ClientID %>");
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");

            ddlStatusName.value = StatusName;
            ddlCategory.value = Category;
            txtTAT.value = strTAT;

            if (StatusActive == false) {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }


            modelBG.className = "mdBG";
            mb.className = "mdbox";
            return false;
        };

    </script>
    <script language="javascript" type="text/javascript">

        function validateDetails() {
            var tatValue = parseInt(document.getElementById("<%= txt_TAT.ClientID %>").value);
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");

            if ((isNaN(tatValue)) || (tatValue <= 0)) {
                lblMsg.innerHTML = "Please enter the TAT value in minutes.";
                return false;
            }

            return true;
        }

        function ShowMD() {
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "";
            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            document.getElementById("<%= txt_TAT.ClientID %>").value = "";
            document.getElementById("<%= ddl_Status.ClientID %>").value = "0";
            document.getElementById("<%= ddl_Category.ClientID %>").value = "Reminder";

            modelBG.className = "mdBG";
            mb.className = "mdbox";

            return false;
        }

        function HideMD() {

            modelBG.className = "mdNone";
            mb.className = "mdNone";
        };
    </script>
    <script language="javascript" type="text/javascript">

        function GetSelectedRow(lnk) {
            var row = lnk.parentNode.parentNode;
            var StatusId = row.cells[2].innerHTML;
            var category = row.cells[4].innerHTML;
            var vtat = row.cells[5].innerHTML;
            var Notificationid = row.cells[1].innerHTML;
            var Active = row.cells[11].innerHTML;


            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            var tat = document.getElementById("<%=txt_TAT.ClientID %>");
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");
            var SaveButton = document.getElementById("<%= btnSave.ClientID %>");


            if (Active == "false") {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }

            document.getElementById("<%= ddl_Status.ClientID %>").value = StatusId;
            document.getElementById("<%= ddl_Category.ClientID %>").value = category;
            document.getElementById("<%= hdnNotificationid.ClientID %>").value = Notificationid;
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save Changes";
            tat.value = vtat;

            modelBG.className = "mdBG";
            mb.className = "mdbox";



            return false;
        }


        function setNotifyTat() {

            e = document.getElementById("<%=ddl_Category.ClientID%>");
            var Category = e.options[e.selectedIndex].text;
            if (Category == "Notification") {
                document.getElementById("<%=txt_TAT.ClientID%>").value = 0;
                document.getElementById("<%=txt_TAT.ClientID%>").readOnly = true;

            }
            else {

                document.getElementById("<%=txt_TAT.ClientID%>").readOnly = false;
            }
            return false;
        }


        var specialKeys = new Array();
        specialKeys.push(8); //Backspace

        function IsNumeric(e) {
            var keyCode = e.which ? e.which : e.keyCode
            var ret = ((keyCode >= 48 && keyCode <= 57) || specialKeys.indexOf(keyCode) != -1);
            return ret;
        }
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <WF:WorkFlowWizard ID="WorkFlowWizard1" runat="server" ActiveItemName="Notification" />
    <div class="GVDiv">
        <div class="InfoDisplay">
            <h3>
                <asp:Label ID="lblCurrentStageNameHeader" runat="server" Text="Stage Name: " />
                <asp:Label ID="lblCurrentStageNameHeaderValue" runat="server" Text="-" />
            </h3>
        </div>
        <asp:GridView ID="gridNotification" runat="server" DataKeyNames="NotificationId"
            PageSize="10" OnPageIndexChanging="gridNotification_PageIndexChanging" AllowPaging="True"
            CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
            EmptyDataText=" Notifications are not available" OnRowDataBound="gridNotification_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="Edit">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CommandArgument="" OnClientClick="return GetSelectedRow(this)" TagName="Read"
                            Text="" ToolTip="Edit"><img src="../images/Edit.png"/>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
            <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                PageButtonCount="5" PreviousPageText=" " />
            <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
        </asp:GridView>
        <asp:HiddenField ID="hdnSaveStatus" runat="server" />
        <asp:HiddenField ID="hdnNotificationid" runat="server" />
        <asp:Button ID="btnAddNotification" runat="server" Text="Add Notification" OnClientClick="return ShowMD();"
            TagName="Add" CausesValidation="false" OnClick="btnAddNotification_Click" meta:resourcekey="btnAddNotification"
            CssClass="btnaddnotification" />
        <asp:Button ID="btnGoBacktoStage" runat="server" Text="Go Back To Stages" meta:resourcekey="btnGoBacktoStage" TagName="Read"
            OnClick="btnGoBacktoStage_Click" CssClass="btngobackto" />
    </div>
    <div id="modelBG" class="mdNone">
    </div>
    <div id="mb" class="mdNone">
        <div id="Content">
            <table>
                <tr>
                    <td colspan="2">
                        <h3>
                            <asp:Label ID="lblEditNotificationDetails" runat="server" Text="Edit Notification Details"
                                meta:resourcekey="lblEditNotificationDetails" /></h3>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblNotificationStatus" Text="Stage Status" meta:resourcekey="lblNotificationStatus" />
                    </td>
                    <td>
                        <asp:DropDownList ID="ddl_Status" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblNotificationCategory" runat="server" Text="Category" meta:resourcekey="lblNotificationCategory"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddl_Category" runat="server" onchange="setNotifyTat()">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblNotificationTAT" runat="server" Text="TAT (in minutes)" meta:resourcekey="lblNotificationTAT"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txt_TAT" runat="server" onkeypress="return IsNumeric(event);" MaxLength="4"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblNotificationActive" runat="server" Text="Active" meta:resourcekey="lblNotificationActive"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" runat="server" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnErrorStatus" runat="server" Value="" />
        </div>
        <div style="float: right;">
            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="javascript:return validateDetails();"
                meta:resourcekey="btnSave" CssClass="btnsave" TagName="Add" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="HideMD(); return false;"
                meta:resourcekey="btnCancel" CssClass="btncancel" TagName="Read" />
        </div>
    </div>
</asp:Content>
