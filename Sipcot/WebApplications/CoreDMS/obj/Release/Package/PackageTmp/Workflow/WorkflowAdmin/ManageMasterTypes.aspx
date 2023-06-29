<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageMasterTypes.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageMasterTypes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function ShowMD() {
            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            var txtTypeName = document.getElementById("<%= txtTypeName.ClientID %>");
            var txtTypeDescription = document.getElementById("<%= txtTypeDescription.ClientID %>");
            var ddlParentType = document.getElementById("<%= ddlParentType.ClientID %>");
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "";
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");

            txtTypeName.value = "";
            txtTypeDescription.value = "";
            ddlParentType.value = 0;
            Checkboxactive.checked = false;
            modelBG.className = "mdBG";
            mb.className = "mdbox";
            return false;
        };

        function ShowMDReload(TypeID, TypeName, TypeDescription, IsActive, ParentType) {

            var txtTypeName = document.getElementById("<%= txtTypeName.ClientID %>");
            var txtTypeDescription = document.getElementById("<%= txtTypeDescription.ClientID %>");
            var ddlParentType = document.getElementById("<%= ddlParentType.ClientID %>");
            txtTypeName.value = TypeName;
            txtTypeDescription.value = TypeDescription;
            ddlParentType.value = ParentType;
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");

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

        function MaxLength(txtbox, maxLength) {
            if (txtbox.value.length >= maxLength) {
                txtbox.value = txtbox.value.substring(0, maxLength - 1);
            }
        }



        function CheckEmptyTextTypeName() {
            var TypeName = document.getElementById("<%= txtTypeName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z ]+$/; var uid;
            uid = TypeName.value;


            if (TypeName.value == "" || TypeName.value == undefined) {
                lblMsg.innerHTML = "Please enter a type name .";
                return false;
            }
            if (re.test(uid)) {
                return true;
            }
            else {
                lblMsg.innerHTML = "Type name allows alphabets, numbers and few special characters ( _ - # @ ^ $) only.";
                return false;
            }

        }

        function GetSelectedRow(lnk) {
            var row = lnk.parentNode.parentNode;
            var TypeId = row.cells[3].innerHTML;  //Type ID
            var Typename = row.cells[4].innerHTML;
            var Typedescription = row.cells[5].innerHTML.replace(/(&nbsp;)/g, '');
            var Isactive = row.cells[9].innerHTML;
            var parentId = row.cells[8].innerHTML;

            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            var txtTypeName = document.getElementById("<%= txtTypeName.ClientID %>");
            var txtTypeDescription = document.getElementById("<%= txtTypeDescription.ClientID %>");
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");

            var ddlParentType = document.getElementById("<%= ddlParentType.ClientID %>");

            if (Isactive == "Inactive") {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }


            txtTypeName.value = Typename;
            txtTypeDescription.value = Typedescription;
            ddlParentType.value = parentId;
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save Changes";
            document.getElementById("<%= hiddenMasterId.ClientID %>").value = TypeId;

            modelBG.className = "mdBG";
            mb.className = "mdbox";

            return false;
        }
    </script>
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            var TypeName = document.getElementById("<%= txtTypeName.ClientID %>").value;
            var TypeDescription = document.getElementById("<%= txtTypeDescription.ClientID %>").value;
            var IsActive = document.getElementById("<%= chkActive.ClientID %>").checked;
            var ParentType = document.getElementById("<%= ddlParentType.ClientID %>").value;

            if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value == "ADD_ERROR") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReload("0", TypeName, TypeDescription, IsActive, ParentType);
            }

            else if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value == "EDIT_ERROR") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReload("0", TypeName, TypeDescription, IsActive, ParentType);

            }
        });


        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="GVDiv">
        <asp:GridView ID="gridMasterTypes" runat="server" OnRowDataBound="gridMasterTypes_RowDataBound"
            AutoGenerateColumns="true" DataKeyNames="Type Id" CssClass="mGrid" PagerStyle-CssClass="pgr"
            PageSize="10" AlternatingRowStyle-CssClass="alt" OnPageIndexChanging="gridMasterTypes_PageIndexChanging"
            EmptyDataText="No Types Are Defined" CellPadding="10" CellSpacing="5" AllowPaging="True">
            <Columns>
                <asp:TemplateField HeaderText="Edit">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" Text="" ToolTip="Edit" OnClientClick="return GetSelectedRow(this)"
                            TagName="Read" CommandArgument=''><img src="../images/Edit.png"/> </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
            <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                PageButtonCount="5" PreviousPageText=" " />
            <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
        </asp:GridView>
        <asp:Button ID="btnAddTypes" runat="server" Text="Add Types" OnClientClick="return ShowMD();"
            CausesValidation="false" CssClass="btnadd" TagName="Add" />
    </div>
    <asp:HiddenField ID="hdnSaveStatus" runat="server" />
    <asp:HiddenField ID="hiddenMasterId" runat="server" />
    <div id="modelBG" class="mdNone">
    </div>
    <div id="mb" class="mdNone">
        <div id="Content">
            <table>
                <tr>
                    <td colspan="2">
                        <h3>
                            <asp:Label ID="lblEditMaster" runat="server" Text="Add/Edit Type Details" /></h3>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblParentType" runat="server" Text="Parent Master Type"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlParentType" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblTypeName" Text="Master Type Name" />
                        <span style="color: Red; font-size: medium">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtTypeName" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbltypeDescription" runat="server" Text="Master Type Description"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtTypeDescription" runat="server" TextMode="MultiLine" onkeyup="javascript:MaxLength(this, 250)"
                            MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblTypeActive" runat="server" Text="Active"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" runat="server" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnErrorStatus" runat="server" Value="" />
        </div>
        <div style="float: right;">
            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="return CheckEmptyTextTypeName();"
                meta:resourcekey="btnSave" CssClass="btnsave" TagName="Add" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="HideMD(); return false;"
                CausesValidation="false" meta:resourcekey="btnCancel" CssClass="btncancel" TagName="Read"/>
        </div>
    </div>
</asp:Content>
