<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageMasterValues.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageMasterValues" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .HiddenButton
        {
            visibility: hidden;
            text-align: center;
        }
    </style>
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {

            var ValueName = document.getElementById("<%= txtValueName.ClientID %>").value;
            var ValueDescription = document.getElementById("<%= txtValueDescription.ClientID %>").value;
            var IsActive = document.getElementById("<%= chkActive.ClientID %>").checked;
            var MasterType = document.getElementById("<%= ddlMasterType.ClientID %>").value;

            var hdnErrorStatus = document.getElementById("<%= hdnErrorStatus.ClientID %>");
            var ErrorStatus = hdnErrorStatus.value;

            if ((ErrorStatus == "ADD_ERROR") || (ErrorStatus == "EDIT_ERROR") || (ErrorStatus == "FILL_MASTER_VALUES")) {
                hdnErrorStatus.value = "";
                ShowMDReload("0", ValueName, ValueDescription, IsActive, MasterType);
            }
        });

        function ShowMDReload(ValueID, ValueName, ValueDescription, IsActive, MasterType) {
            var txtValueName = document.getElementById("<%= txtValueName.ClientID %>");
            var txtValueDescription = document.getElementById("<%= txtValueDescription.ClientID %>");
            var ddlMasterType = document.getElementById("<%= ddlMasterType.ClientID %>").value;
            txtValueName.value = ValueName;
            txtValueDescription.value = ValueDescription;
            ddlMasterType.value = MasterType;
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");

            if (IsActive == false) {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }
            modelBG.className = "mdBG";
            mb.className = "mdbox";
            return false;
        };
    </script>
    <script type="text/javascript">

        function ShowMD() {
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "";
            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            var txtValueName = document.getElementById("<%= txtValueName.ClientID %>");
            var txtValueDescription = document.getElementById("<%= txtValueDescription.ClientID %>");
            var ddlMasterType = document.getElementById("<%= ddlMasterType.ClientID %>");
            var ddlParentValue = document.getElementById("<%= ddlParentValue.ClientID %>");

            var hdnErrorStatus = document.getElementById("<%= hdnErrorStatus.ClientID %>");
            hdnErrorStatus.value = "";

            txtValueName.value = "";
            txtValueDescription.value = "";
            ddlMasterType.value = "0";

            $(ddlParentValue).empty();
            var option = document.createElement("option");
            option.text = "--Select--";
            option.value = 0;
            ddlParentValue.add(option);

            modelBG.className = "mdBG";
            mb.className = "mdbox";
            return false;
        };

        function HideMD() {

            modelBG.className = "mdNone";
            mb.className = "mdNone";
        };

        function MaxLength(txtbox, maxLength) {
            if (txtbox.value.length >= maxLength) {
                txtbox.value = txtbox.value.substring(0, maxLength - 1);
            }
        }



        function CheckEmptyTextValueName() {
            var ValueName = document.getElementById("<%= txtValueName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var ddlMasterType = document.getElementById("<%= ddlMasterType.ClientID %>");
            var ddlParentValue = document.getElementById("<%= ddlParentValue.ClientID %>")
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z ]+$/; var uid;
            uid = ValueName.value;

            document.getElementById("<%= hiddenParentValueId.ClientID %>").value = ddlParentValue.value;
            document.getElementById("<%= hiddenMasterTypeId.ClientID %>").value = ddlMasterType.value;

            if (ddlMasterType.value == "0" || ValueName.value == undefined) {
                lblMsg.innerHTML = "Please select a master value type .";
                return false;
            }
            if (ValueName.value == "" || ValueName.value == undefined) {
                lblMsg.innerHTML = "Please enter a master value name .";
                return false;
            }

            if (re.test(uid)) {
                return true;
            }
            else {
                lblMsg.innerHTML = "Value name allows alphabets, numbers and few special characters ( _ - # @ ^ $) only.";
                return false;
            }

        }

        function GetSelectedRow(lnk) {
            var row = lnk.parentNode.parentNode;
            var ValueId = row.cells[3].innerHTML; //Value ID
            var TypeName = row.cells[4].innerHTML;
            var TypeId = row.cells[5].innerHTML;
            var Valuename = row.cells[6].innerHTML;
            var Valuedescription = row.cells[7].innerHTML.replace(/(&nbsp;)/g, '');
            var Parentmstrvalue = row.cells[8].innerHTML;
            var Isactive = row.cells[11].innerHTML;

            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            var txtValueName = document.getElementById("<%= txtValueName.ClientID %>");
            var txtValueDescription = document.getElementById("<%= txtValueDescription.ClientID %>");
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");
            if (Isactive == "Inactive") {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }

            document.getElementById("<%= ddlMasterType.ClientID%>").value = TypeId;
            document.getElementById("<%= ddlParentValue.ClientID %>").value = Parentmstrvalue;

            txtValueName.value = Valuename;
            txtValueDescription.value = Valuedescription;
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save Changes";
            document.getElementById("<%= hiddenValueId.ClientID %>").value = ValueId;
            document.getElementById("<%= hiddenParentValueId.ClientID %>").value = Parentmstrvalue;
            document.getElementById("<%= hiddenMasterTypeId.ClientID %>").value = TypeId;
            //            modelBG.className = "mdBG";
            //            mb.className = "mdbox";

            document.getElementById("<%= btnHiddenSubmit.ClientID %>").click();

            return false;

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="GVDiv">
        <asp:GridView ID="gridMasterValues" runat="server" OnRowDataBound="gridMasterValue_RowDataBound"
            PageSize="10" AutoGenerateColumns="true" DataKeyNames="Value Id" CssClass="mGrid"
            PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt" EmptyDataText="No Values Are Defined"
            CellPadding="10" CellSpacing="5" AllowPaging="True" OnPageIndexChanging="gridMasterValue_PageIndexChanging">
            <Columns>
                <asp:TemplateField HeaderText="Edit">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" Text="" ToolTip="Edit" OnClientClick="return GetSelectedRow(this)" TagName="Read"
                            CommandArgument=''><img src="../images/Edit.png"/></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
            <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                PageButtonCount="5" PreviousPageText=" " />
            <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
        </asp:GridView>
        <asp:Button ID="btnAddValues" runat="server" Text="Add Values" OnClientClick="return ShowMD();"  TagName="Add" 
            CausesValidation="false" CssClass="btnadd" />
    </div>
    <asp:HiddenField ID="hdnSaveStatus" runat="server" />
    <asp:HiddenField ID="hiddenValueId" runat="server" />
    <asp:HiddenField ID="hiddenParentValueId" runat="server" />
    <asp:HiddenField ID="hiddenMasterTypeId" runat="server" />
    <asp:Button ID="btnHiddenSubmit" runat="server" Height="25px" class="HiddenButton"
        Text="HiddenSubmit" TagName="Read" OnClick="btnHiddenSubmit_Click" />
    <div id="modelBG" class="mdNone">
    </div>
    <div id="mb" class="mdNone">
        <div id="Content">
            <table>
                <tr>
                    <td colspan="2">
                        <h3>
                            <asp:Label ID="lblEditMasterValue" runat="server" Text="Add/Edit Value Details" /></h3>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblMasterType" runat="server" Text="Master Type"></asp:Label><span
                            style="color: Red; font-size: medium">*</span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlMasterType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlMasterValue_SelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblValueName" Text="Value Name" />
                        <span style="color: Red; font-size: medium">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtValueName" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblValueDescription" runat="server" Text="Value Description"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtValueDescription" runat="server" TextMode="MultiLine" onkeyup="javascript:MaxLength(this, 250)"
                            MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblValueActive" runat="server" Text="Active"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblParentValue" runat="server" Text="Parent Master Value"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlParentValue" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnErrorStatus" runat="server" Value="" />
        </div>
        <div style="float: right;">
            <asp:Button ID="btnSave" runat="server" Text="Save" OnClientClick="return CheckEmptyTextValueName();"  TagName="Add" 
                OnClick="btnSave_Click" meta:resourcekey="btnSave" CssClass="btnsave" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="HideMD(); return false;"
                CausesValidation="false" meta:resourcekey="btnCancel" CssClass="btncancel" TagName="Read"/>
        </div>
    </div>
</asp:Content>
