<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageWorkflowBulkUpload.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageWorkflowBulkUpload" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">

        function uploadStart(sender, args) {
            if (ValidateMandatory()) {
                var filename = args.get_fileName();
                var filext = filename.substring(filename.lastIndexOf(".") + 1).toLowerCase();
                if (filext == 'xls' || filext == 'xlsx') {
                    return true;
                }
                else {
                    var err = new Error();
                    err.name = 'My API Input Error';
                    err.message = 'Please select excel Files with extension xls or xlsx!';


                    ///document.getElementById('<%=AsyncFileUpload1.ClientID %>').innerText = "";

                    throw (err);

                    return false;
                }
            }
            else {
                return false;
            }
        }
        function DoPostback() {
            if (ValidateMandatory()) {
                document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
            }
            else {
                return false;
            }
        }

        function ValidateMandatory() {
            var ddlOrganization = document.getElementById("<%= ddlOrg.ClientID %>");
            var ddlProcess = document.getElementById("<%= ddlProcess.ClientID %>");
            var ddlWorkflow = document.getElementById("<%= ddlWorkflow.ClientID %>");
            var ddlStage = document.getElementById("<%= ddlStage.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");

            if (ddlOrganization.value == "0" || ddlProcess.value == "0" || ddlWorkflow.value == "0" || ddlStage.value == "0") {
                lblMsg.innerHTML = "Please select all mandatory fields.";
                return false;
            }
            else {return true;}
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="GVDiv">
                <table>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblOrg" runat="server" Text="Organizaion"></asp:Label><span
                            style="color: Red; font-size: medium">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOrg" runat="server" AutoPostBack="true" OnSelectedIndexChanged="dddlorg_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblProcess" runat="server" Text="Process Name"></asp:Label><span
                            style="color: Red; font-size: medium">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlProcess" runat="server" OnSelectedIndexChanged="ddlProcess_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblWorkflow" runat="server" Text="Workflow Name"></asp:Label><span
                            style="color: Red; font-size: medium">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlWorkflow" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlWorkflow_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblStage" runat="server" Text="Stage Name"></asp:Label><span
                            style="color: Red; font-size: medium">*</span>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlStage" runat="server" AutoPostBack="true" 
                                onselectedindexchanged="ddlStage_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblFileUpload" runat="server" Text="File Name"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Throbber" runat="server" Style="display: none">
                            <img alt="Loading..." src="<%= Page.ResolveClientUrl("~/Images/indicator.gif")%>" /></asp:Label>
                            <ajax:AsyncFileUpload ID="AsyncFileUpload1" runat="server" OnUploadedComplete="AsyncFileUpload1_OnUploadedComplete"
                                OnClientUploadComplete="DoPostback" CssClass="LabelStyle" Width="160px" CompleteBackColor="Lime"
                                ErrorBackColor="Red" ThrobberID="Throbber" OnClientUploadStarted="uploadStart"
                                UploadingBackColor="#66CCFF" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:GridView ID="GridView1" runat="server" AllowPaging="true" Width="600px"
                            CssClass="mGrid" PagerStyle-CssClass="pgr" AlternatingRowStyle-CssClass="alt"
                     CellPadding="10" CellSpacing="5"  
                            PageSize="10" OnPageIndexChanging="GridView1_OnPageIndexChanging" 
                               >
                            </asp:GridView>
                        </td>
                    </tr>
                     <tr>
                    <td>
                          <asp:Button ID="btnCallFromJavascript" runat="server" class="HiddenButton" 
                                 OnClick="btnCallFromJavascript_Click" TagName="Read" />   
                         </td>
                         <td>
                            
                             <asp:Button ID="btnRedirect" runat="server" CssClass="btngotoworkflow" 
                                 OnClick="btnRedirect_Click" OnClientClick="return ValidateMandatory();" 
                                 Text="Goto Mapping" />
                                  <asp:Button ID="btncommit" runat="server" CssClass="btncommit"  Enabled="false"
                                 OnClick="btncommit_Click" Text="Commit" />
                         </td>
                       
                </table>
            </div>
            <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
           
            <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
            <asp:HiddenField ID="hdnUserId" runat="server" Value="" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
