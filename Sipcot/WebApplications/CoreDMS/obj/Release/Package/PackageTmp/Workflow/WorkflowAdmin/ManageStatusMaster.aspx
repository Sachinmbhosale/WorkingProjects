<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master" AutoEventWireup="true" CodeBehind="ManageStatusMaster.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageStatusMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">

    function ShowMD() {
        document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
        var txtStatusName = document.getElementById("<%= txtStatusName.ClientID %>");
        var txtStatusDescription = document.getElementById("<%= txtStatusDescription.ClientID %>");
       
        document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "";
        var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");

        txtStatusName.value = "";
        txtStatusDescription.value = "";
    
        Checkboxactive.checked = false;
        modelBG.className = "mdBG";
        mb.className = "mdbox";
        return false;
    };

    function ShowMDReload(TypeID, StatusName, StatusDescription, IsActive) {

        var txtStatusName = document.getElementById("<%= txtStatusName.ClientID %>");
        var txtStatusDescription = document.getElementById("<%= txtStatusDescription.ClientID %>");
       
        txtStatusName.value = StatusName;
        txtStatusDescription.value = StatusDescription;
       
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



    function CheckEmptyTextStatusName() {
        var StatusName = document.getElementById("<%= txtStatusName.ClientID %>");
        var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
        var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z ]+$/; var uid;
        uid = StatusName.value;


        if (StatusName.value == "" || StatusName.value == undefined) {
            lblMsg.innerHTML = "Please enter a Status name .";
            return false;
        }
        if (re.test(uid)) {
            return true;
        }
        else {
            lblMsg.innerHTML = "Status name allows alphabets, numbers and few special characters ( _ - # @ ^ $) only.";
            return false;
        }

    }

    function GetSelectedRow(lnk) {
        var row = lnk.parentNode.parentNode;
        var StatusId = row.cells[3].innerHTML;  //StatusId
        var StatusName = row.cells[4].innerHTML;
        var StatusDescription = row.cells[5].innerHTML.replace(/(&nbsp;)/g, '');
        var Isactive = row.cells[7].innerHTML;
     

        document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
        var txtStatusName = document.getElementById("<%= txtStatusName.ClientID %>");
        var txtStatusDescription = document.getElementById("<%= txtStatusDescription.ClientID %>");
        var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");

       

        if (Isactive == "Inactive") {
            Checkboxactive.checked = false;
        }
        else { Checkboxactive.checked = true; }


        txtStatusName.value = StatusName;
        txtStatusDescription.value = StatusDescription;
       
        document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save Changes";
        document.getElementById("<%= hiddenMasterId.ClientID %>").value = StatusId;

        modelBG.className = "mdBG";
        mb.className = "mdbox";

        return false;
    }
    </script>
      <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            var StatusName = document.getElementById("<%= txtStatusName.ClientID %>").value;
            var StatusDescription = document.getElementById("<%= txtStatusDescription.ClientID %>").value;
            var IsActive = document.getElementById("<%= chkActive.ClientID %>").checked;
            

            if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value == "ADD_ERROR") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReload("0", StatusName, StatusDescription, IsActive);
            }

            else if (document.getElementById("<%= hdnErrorStatus.ClientID %>").value == "EDIT_ERROR") {
                document.getElementById("<%= hdnErrorStatus.ClientID %>").value = "";
                ShowMDReload("0", StatusName, StatusDescription, IsActive);

            }
        });


        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

  
    <div class="GVDiv">
        <asp:GridView ID="GridStatusMaster" runat="server" OnRowDataBound="GridStatusMaster_RowDataBound"
            AutoGenerateColumns="true"  CssClass="mGrid" PagerStyle-CssClass="pgr" PageSize="10"
             AlternatingRowStyle-CssClass="alt" OnPageIndexChanging="GridStatusMaster_PageIndexChanging"
            EmptyDataText="No Status Are Defined" CellPadding="10" CellSpacing="5" AllowPaging="True">
             <Columns>
                <asp:TemplateField HeaderText="Edit">
                    <ItemTemplate>
                       
                        <asp:LinkButton ID="lnkEdit" TagName="Read" runat="server" Text="" ToolTip="Edit" OnClientClick="return GetSelectedRow(this)"
                            CommandArgument=''><img src="../images/Edit.png"/></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
            <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                PageButtonCount="5" PreviousPageText=" " />
            <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
        </asp:GridView>
        <asp:Button ID="btnAddStatus"  TagName="Add"  runat="server" Text="Add Status" OnClientClick="return ShowMD();"
        CausesValidation="false" CssClass="btnadd"/>
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
                            <asp:Label ID="lblEditMaster" runat="server" Text="Add/Edit Status Details"  /></h3>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblStatusName"  Text="Status Name" />
                        <span style="color: Red; font-size: medium">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtStatusName" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblStatusDescription" runat="server" Text="Status Description"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtStatusDescription" runat="server" TextMode="MultiLine" onkeyup="javascript:MaxLength(this, 250)" MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblStatusActive" runat="server" Text="Active" ></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" runat="server" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnErrorStatus" runat="server" Value="" />
        </div>
        <div style="float: right;">
            <asp:Button ID="btnSave" runat="server"  TagName="Add"  Text="Save" OnClick="btnSave_Click" OnClientClick="return CheckEmptyTextStatusName();"
                meta:resourcekey="btnSave" CssClass="btnsave" />
            <asp:Button ID="btnCancel" runat="server" TagName="Read" Text="Cancel" OnClientClick="HideMD(); return false;"
                CausesValidation="false" meta:resourcekey="btnCancel" CssClass="btncancel" />
        </div>
    </div>
</asp:Content>
