<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageProcess.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageProcess"
    Culture="auto" meta:resourcekey="ManageProcess" UICulture="auto" EnableEventValidation="false" %>

<%@ Register TagPrefix="WF" TagName="WorkFlowWizard" Src="WorkFlowWizardMenu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">
        function SelectSingleRadiobutton(rdbtn) {
            var rdBtn = rdbtn;
            var rdBtnList = document.getElementsByTagName("input");
            for (i = 0; i < rdBtnList.length; i++) {
                if (rdBtnList[i].type == "radio" && rdBtnList[i].id != rdBtn.id) {
                    rdBtnList[i].checked = false;
                }
            }
        }
    </script>
    <script language="javascript" type="text/javascript">
        function MaxLength(txtbox, maxLength) {
            if (txtbox.value.length >= maxLength) {
                txtbox.value = txtbox.value.substring(0, maxLength - 1);
            }
        }
    </script>
    <script type="text/javascript">
        function Validate() {
            var gv = document.getElementById("<%=gridProcess.ClientID%>");
            var rbs = gv.getElementsByTagName("input");
            var lblMsg = document.getElementById("<%= lblMessageGrid.ClientID %>");
            var flag = 0;
            for (var i = 0; i < rbs.length; i++) {

                if (rbs[i].type == "radio") {
                    if (rbs[i].checked) {
                        flag = 1;
                        break;
                    }
                }
            }
            if (flag == 0) {
                lblMsg.innerHTML = "Please select a row to proceed.";
                return false;
            }

        }  

    </script>
    <script type="text/javascript">

        function ShowMD() {
            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            var txtProcessname = document.getElementById("<%= txtProcessName.ClientID %>");
            var txtProcessDescription = document.getElementById("<%= txtProcessDescription.ClientID %>");
            var ddlOrganization = document.getElementById("<%= ddlOrganization.ClientID %>");

            txtProcessname.value = "";
            txtProcessDescription.value = "";
            ddlOrganization.value = "0";
            modelBG.className = "mdBG";
            mb.className = "mdbox";
            return false;
        };

        function ShowMDReload(ProcessID, ProcessName, ProcessDescription, IsActive, Organization) {

            var txtProcessname = document.getElementById("<%= txtProcessName.ClientID %>");
            var txtProcessDescription = document.getElementById("<%= txtProcessDescription.ClientID %>");
            txtProcessname.value = ProcessName;
            txtProcessDescription.value = ProcessDescription;
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");
            var ddlOrganization = document.getElementById("<%= ddlOrganization.ClientID %>");
            ddlOrganization.value = Organization;

            if (IsActive == false) {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }
            modelBG.className = "mdBG";
            mb.className = "mdbox";
            return false;
        };

        function HideMD() {

            modelBG.className = "mdNone";
            mb.className = "mdNone";
        };

         
    </script>
    <script type="text/javascript">
        function GetSelectedRow(lnk) {
            var row = lnk.parentNode.parentNode;
            var ProcessId = row.cells[3].innerHTML;
            var Processname = row.cells[4].innerHTML;
            var Processdescription = row.cells[5].innerHTML.replace(/(&nbsp;)/g, '');
            var Isactive = row.cells[8].innerHTML;
            var Organization = row.cells[9].innerHTML;

            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            var txtProcessname = document.getElementById("<%= txtProcessName.ClientID %>");
            var txtProcessDescription = document.getElementById("<%= txtProcessDescription.ClientID %>");
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");
            var ddlOrganization = document.getElementById("<%= ddlOrganization.ClientID %>");

            if (Isactive == "Inactive") {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }


            txtProcessname.value = Processname;
            txtProcessDescription.value = Processdescription;
            ddlOrganization.value = Organization;
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save Changes";
            document.getElementById("<%= hiddenProcessId.ClientID %>").value = ProcessId;

            modelBG.className = "mdBG";
            mb.className = "mdbox";



            return false;
        }



    </script>
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            var ProcessName = document.getElementById("<%= txtProcessName.ClientID %>").value;
            var ProcessDesc = document.getElementById("<%= txtProcessDescription.ClientID %>").value;
            var ProcessActive = document.getElementById("<%= chkActive.ClientID %>").checked;

            var Organization = document.getElementById("<%= ddlOrganization.ClientID %>").value;

            if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value == "ADD_ERROR") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReload("0", ProcessName, ProcessDesc, ProcessActive, Organization);
            }

            else if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value == "EDIT_ERROR") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReload("0", ProcessName, ProcessDesc, ProcessActive, Organization);

            }


        });


        function CheckEmptyTextProcessName() {

            var ddlOrganization = document.getElementById("<%= ddlOrganization.ClientID %>");

            var ProName = document.getElementById("<%= txtProcessName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var hiddenOrganizationId = document.getElementById("<%= hiddenOrganizationId.ClientID %>");
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z ]+$/; var uid;
            uid = ProName.value;

            hiddenOrganizationId.value = ddlOrganization.value;

            if (ddlOrganization.value == "0" || ProName.value == undefined) {
                lblMsg.innerHTML = "Please select an organization name.";
                return false;
            }
            if (ProName.value == "" || ProName.value == undefined) {
                lblMsg.innerHTML = "Please enter a process name.";
                return false;
            }
            if (re.test(uid)) {
                return true;
            }
            else {
                lblMsg.innerHTML = "Process name allows alphabets, numbers and few special characters ( _ - # @ ^ $) only.";
                return false;
            }

        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <WF:WorkFlowWizard ID="WorkFlowWizard1" runat="server" ActiveItemName="Process" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="GVDiv">
                <asp:Label ID="lblMessageGrid" ForeColor="Red" runat="server"></asp:Label>
                <asp:GridView ID="gridProcess" runat="server" OnRowDataBound="gridProcess_RowDataBound"
                    AutoGenerateColumns="true" DataKeyNames="Process Id" CssClass="mGrid" PagerStyle-CssClass="pgr"
                    PageSize="10" OnPageIndexChanging="gridProcess_PageIndexChanging" AlternatingRowStyle-CssClass="alt"
                    EmptyDataText="No process are defined" CellPadding="10" CellSpacing="5" AllowPaging="True">
                    <Columns>
                        <asp:TemplateField HeaderText="Edit">
                            <ItemTemplate>
                                <asp:RadioButton ID="RowSelector" runat="server" OnClick="javascript:SelectSingleRadiobutton(this)"
                                    TagName="Read" />
                                <asp:LinkButton ID="lnkEdit" runat="server" Text="" ToolTip="Edit" OnClientClick="return GetSelectedRow(this)"
                                    TagName="Read" CommandArgument=''><img src="../images/Edit.png"/></asp:LinkButton>
                                <asp:LinkButton ID="lnkManageProcessOwners" runat="server" CommandArgument="" CausesValidation="false"
                                    TagName="Read" ToolTip="Go To Process Owners"><img src="../images/stageusers.png"/></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                        PageButtonCount="5" PreviousPageText=" " />
                    <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                </asp:GridView>
                <asp:Button ID="btnAddProcess" runat="server" Text="Add Process" OnClientClick="return ShowMD();"
                    TagName="Add" CausesValidation="false" meta:resourcekey="btnAddProcess" CssClass="btnaddworkflow" />
                <asp:Button ID="btnGotoWorkflow" runat="server" Text="Go To Workflow" OnClientClick="return Validate();"
                    TagName="Read" OnClick="btnGotoWorkflow_Click" meta:resourcekey="btnGotoWorkflow"
                    CssClass="btngotoworkflow" />
                     <asp:Button ID="btnGotoWorkflowStudio" runat="server" Text="Workflow Studio" 
                    TagName="Read" OnClick="btnGotoWorkflowStudio_Click" 
                    CssClass="btngotoworkflow" />
            </div>
            <asp:HiddenField ID="hdnSaveStatus" runat="server" />
            <asp:HiddenField ID="hiddenProcessId" runat="server" />
            <asp:HiddenField ID="hiddenOrganizationId" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="modelBG" class="mdNone">
    </div>
    <div id="mb" class="mdNone">
        <div id="Content">
            <table>
                <tr>
                    <td colspan="2">
                        <h3>
                            <asp:Label ID="lblEditProcess" runat="server" Text="Edit Process Details" meta:resourcekey="lblEditProcess" /></h3>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblOrganization" runat="server" Text="Organization"></asp:Label><span
                            style="color: Red; font-size: medium">*</span>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlOrganization" AutoPostBack="false">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblProcessName" meta:resourcekey="lblProcessName" Text="Process Name" />
                        <span style="color: Red; font-size: medium">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtProcessName" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblProcessDescription" runat="server" Text="Process Description" meta:resourcekey="lblProcessDescription"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtProcessDescription" runat="server" TextMode="MultiLine" onkeyup="javascript:MaxLength(this, 250)"
                            MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblProcessActive" runat="server" Text="Active" meta:resourcekey="lblProcessActive"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" runat="server" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnErrorStatus" runat="server" Value="" />
        </div>
        <div style="float: right;">
            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" TagName="Add"
                OnClientClick="return CheckEmptyTextProcessName();" CssClass="btnsave" meta:resourcekey="btnSave" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="HideMD(); return false;"
                TagName="Read" CssClass="btncancel" CausesValidation="false" meta:resourcekey="btnCancel" />
        </div>
    </div>
</asp:Content>
