<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="WorkflowProcessInitiation.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkflowProcessInitiation" %>


<%@ Register Src="~/Workflow/WorkflowAdmin/WorkflowPDFViewer.ascx" TagName="WorkflowPDFViewer"
    TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100% !important; overflow: hidden;">
        <colgroup>
            <col width="30%" />
            <col width="*" />
        </colgroup>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 35% !important; vertical-align: top; padding: 0px !important">
                <table>
                    <tr id="trWorkFlowDtls" runat="server">
                        <td>
                            <div style="border: 2px solid #fdb813; border-radius: 8px; background: linear-gradient(to bottom, rgba(228,228,228,1) 92%,rgba(228,228,228,1) 92%,#FFC53C); padding: 0 16px 16px 16px;">
                                <div class="card-head">Upload PO Document</div>
                                <table cellpadding="0" cellspacing="0" border="0">
                                    <tr>
                                        <td class="auto-style1">
                                            <asp:Label ID="Label6" runat="server" CssClass="LabelStyle" Text="Select File to Upload"></asp:Label>
                                            <asp:Label ID="Label7" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                                            <br />
                                        </td>
                                        <td>

                                            <asp:FileUpload ID="FileuploadPO" runat="server" />
                                        </td>
                                    </tr>
                                    <tr id="DWG" runat="server">
                                        <td>
                                            <br />
                                        </td>
                                        <td colspan="2">

                                            <asp:Button ID="btnPreview" runat="server" Text="Preview" CssClass="btnpreview" OnClick="btnPreview_Click" />

                                        </td>

                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr id="tr1" runat="server">
                        <td>
                            <div style="border: 2px solid #fdb813; border-radius: 8px; background: linear-gradient(to bottom, rgba(228,228,228,1) 92%,rgba(228,228,228,1) 92%,#FFC53C); padding: 0px 16px 16px 16px;">
                                <div class="card-head">Upload Supporting files</div>
                                <table>
                                    <tr>
                                        <td>Supporting file 1</td>
                                        <td>
                                            <asp:FileUpload ID="fileSupportfile1" runat="server" /> 
                                            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click" >View</asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Supporting file 2</td>
                                        <td>
                                            <asp:FileUpload ID="fileSupportfile2" runat="server" />
                                            <asp:LinkButton ID="LinkButton2" runat="server" OnClick="LinkButton2_Click">View</asp:LinkButton>
                                        </td>
                                    </tr>

                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr id="tr2" runat="server">
                        <td>
                            <div style="border: 2px solid #fdb813; border-radius: 8px; background: linear-gradient(to bottom, rgba(228,228,228,1) 92%,rgba(228,228,228,1) 92%,#FFC53C); padding: 0px 16px 16px 16px;">
                                <div class="card-head">Status</div>
                                <table>
                                    <tr>
                                        <td>Select Status</td>
                                        <td>
                                            <asp:DropDownList ID="DDLStatus" runat="server">
                                                <asp:ListItem Selected="True" Value="0">--Select--</asp:ListItem>
                                                <asp:ListItem Value="616">PO Upload</asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <asp:Panel ID="DataEntryControls" runat="server">

                        <tr>
                            <td colspan="2">

                                <table>
                                    <tr>
                                        <td colspan="2"></td>
                                    </tr>
                                </table>


                            </td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td>

                                    <span style="margin-left: 200px">
                                        <asp:Button ID="btnSave" TagName="Add" runat="server" Text="Submit" OnClick="btnSave_Clcik"
                                            CssClass="btnsave" />
                                        <asp:Button ID="btnCancel" TagName="Read" CausesValidation="false" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                                            CssClass="btncancel" />
                                    </span>
                                    <asp:HiddenField ID="hfFileName" runat="server" Value="" />
                                    <asp:HiddenField ID="hfFileExtension" runat="server" Value="" />
                                    <asp:HiddenField ID="hfFilePath" runat="server" Value="" />

                                    <asp:HiddenField ID="Fldsupportfile2" runat="server" Value="" />
                                    <asp:HiddenField ID="Hidfile2" runat="server" Value="" />
                                    <asp:HiddenField ID="Fldsupportfile1" runat="server" Value="" />
                                    <asp:HiddenField ID="Hidfile1" runat="server" Value="" />
                                    <asp:HiddenField ID="hidFilePath" runat="server" Value="" />

                        </td>
                    </tr>
                </table>
            </td>

            <td style="width: 65% !important" valign="top" id="tr" runat="server">

                        <table style="width: 100%">
                            <colgroup>
                                <col width="50%" />
                                <col width="50%" />
                            </colgroup>
                            <tr>
                                <td>

                                    <iframe id="myiframe" style="width: 800px; height:600px;" runat="server"></iframe>
                                </td>
                                <%-- <td id="tdTenderDoc" runat="server" visible="false">
                                    <span>
                                        <h3>Tender Document</h3>
                                    </span>
                                    <iframe id="Iframe1" height="600" width="400" runat="server"></iframe>
                                </td>--%>
                            </tr>
                        </table>
    

                <div style="display: none">

                            <uc1:WorkflowPDFViewer ID="WFPDFViewer" runat="server" />
                            <asp:Label ID="lblMessageImg" ForeColor="Red" runat="server"></asp:Label>

                </div>
            </td>


        </tr>
        <tr>
            <td></td>
        </tr>
    </table>

</asp:Content>
