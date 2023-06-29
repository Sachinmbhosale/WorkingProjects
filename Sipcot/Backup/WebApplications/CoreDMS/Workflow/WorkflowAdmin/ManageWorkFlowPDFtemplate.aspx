<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="ManageWorkFlowPDFtemplate.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageWorkFlowPDFtemplate" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">
        function uploadStart(sender, args) {
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1).toLowerCase();
            if (filext == 'pdf') {
                return true;
            }
            else {
                var err = new Error();
                err.name = 'My API Input Error';
                err.message = 'Please select Pdf Files!';


                ///document.getElementById('<%=AsyncFileUpload1.ClientID %>').innerText = "";

                throw (err);
                return false;
            }
        }

        function DoPostback() {
            document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
        }

        function Validate(Name) {
            var selected = true;
            var msgControl = "#<%= divMsg.ClientID %>";
            var btnMapId = document.getElementById("<%= btnmap.ClientID %>");
            var btnRemove = document.getElementById("<%= btnremove.ClientID %>");
            var btncommit = document.getElementById("<%= btncommit.ClientID %>");
            if (Name == btnMapId.id) {
                var ControlListBox = document.getElementById('<%= lstPdfControls.ClientID %>');
                for (var i = 0; i < ControlListBox.options.length; i++) {
                    if (ControlListBox.options[i].selected) {
                        selected = false;
                        break;
                    } 
                }
                if (selected) {
                    $(msgControl).html("Please select a Control or Upload a Pdf template.");
                    return false;
                }
                selected = true;
                var FieldListBox = document.getElementById('<%= lststagefields.ClientID %>');
                for (var i = 0; i < FieldListBox.options.length; i++) {
                    if (FieldListBox.options[i].selected) {
                        selected = false;
                        break;
                    }
                }
                if (selected) {
                    $(msgControl).html("Please select a Field.");
                    return false;
                }

            }
            else if (Name == btnRemove.id) {
                selected = true;
                var MappedListBox = document.getElementById('<%= lstMappedcontrolfields.ClientID %>');
                for (var i = 0; i < MappedListBox.options.length; i++) {
                    if (MappedListBox.options[i].selected) {
                        selected = false;
                        break;
                    }
                }
                if (selected) {
                    $(msgControl).html("Please select a item to remove.");
                    return false;

                }
            }
            else if (Name == btncommit.id) {

                var MappedListBox = document.getElementById('<%= lstMappedcontrolfields.ClientID %>');
                if (MappedListBox.options.length <= 0) {
                    $(msgControl).html("Kindly add values before commit");
                    return false;
                }
            }

        }

    </script>
    <link href="../../css/workflow_styles.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div id="divMsg" runat="server" style="color: Red; font-family: Calibri; font-size: small">
                &nbsp;</div>
            <div>
                <table class="Manageworkflowtable">
                    <tr>
                        <td>
                            <asp:Label ID="lbluploader" runat="server" Text="Upload pdf" CssClass="ManageworkflowLabels"></asp:Label></td>
                                <td>
                                    <asp:AsyncFileUpload ID="AsyncFileUpload1" runat="server" AsyncPostBackTimeout="1600"
                                        CssClass="LabelStyle" Width="160px" CompleteBackColor="Lime" ErrorBackColor="Red"
                                        ThrobberID="Throbber" OnClientUploadStarted="uploadStart" UploadingBackColor="#66CCFF"
                                        OnClientUploadComplete="DoPostback" OnUploadedComplete="AsyncFileUpload1_UploadedComplete" />
                                    </td>                                       
                
                                     </tr>
                  
                               
                  
                </table>
               
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblcontrolsfrompdf" runat="server" Text="Controls from pdf" CssClass="ManageworkflowLabels"></asp:Label>
                        </td>
                        <td>
                        </td>
                        <td>
                            <asp:Label ID="lblFieldsfromStages" runat="server" Text="Fields from Stages" CssClass="ManageworkflowLabels"></asp:Label>
                        </td>
                        <td>
                        </td>
                        <td>
                        </td>
                        <td>
                            <asp:Label ID="lblFMappedControlsFields" runat="server" Text="Mapped Controls Fields"
                                CssClass="ManageworkflowLabels"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="td">
                            <asp:ListBox ID="lstPdfControls" runat="server" CssClass="Manageworkflowlistbox">
                            </asp:ListBox>
                        </td>
                        <td class="td">
                        </td>
                        <td class="td">
                            <asp:ListBox ID="lststagefields" runat="server" CssClass="Manageworkflowlistbox">
                            </asp:ListBox>
                        </td>
                        <td class="td">
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnmap" runat="server" Text="Map" OnClick="btnmap_Click" CssClass="btnadd" Width="80px"
                                            OnClientClick="return Validate(this.id);" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnremove" runat="server" Text="Remove" OnClick="btnremove_Click" CssClass="btnremovesearchcondition" Width="80px"
                                            OnClientClick="return Validate(this.id);" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btncommit" runat="server" Text="Commit" OnClick="btncommit_Click" CssClass="btncommit"
                                            OnClientClick="return Validate(this.id);" />
                                    </td>
                                </tr>
                                <tr>
                                                         
                                <td>
                                        <asp:Button ID="btnGotoWorkflow" runat="server" Text="Workflow"  Width="80px"
                    OnClick="btnGotoWorkflow_Click" meta:resourcekey="btnGotoWorkflow"
                    CssClass="btngotoworkflow" />
                                </td>
                                </tr>

                                <tr>
                                    <td>
                                        <asp:Button ID="btnCallFromJavascript" class="HiddenButton" runat="server" TagName="Read"
                                            OnClick="btnCallFromJavascript_Click" />
                                    </td>
                                </tr>
                               
                            </table>
                        </td>
                        <td class="td">
                            <asp:ListBox ID="lstMappedcontrolfields" runat="server" CssClass="Manageworkflowlcontrollistbox">
                            </asp:ListBox>
                        </td>
                       
                  
                      
                    </tr>
                  
                   
                </table>
                <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                <asp:HiddenField ID="hdnProcessId" runat="server" Value="" />
                <asp:HiddenField ID="hdnWorkflowId" runat="server" Value="" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
