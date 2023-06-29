<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master"
    AutoEventWireup="true" CodeBehind="WorkflowSearch.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkflowSearch" %>

<%@ Register Src="~/Workflow/WorkflowAdmin/WorkflowPDFViewer.ascx" TagName="WorkflowPDFViewer"
    TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>

    <%-- <asp:Panel ID="DataEntryControls" runat="server">--%>

    <script type="text/javascript">

        function GetSelTender(source, eventArgs) {

            var mhiddTenderRefId = eventArgs.get_value();

            alert(mhiddTenderRefId);

            $("#<%=hiddTenderRefId.ClientID%>").val(mhiddTenderRefId);
        }



        function GetReferencePath(source, eventArgs) {

            var FilePath = eventArgs.get_value();
            alert(FilePath);

            $("#<%=hdnSrc.ClientID%>").val(FilePath);
        }
    </script>

    <style type="text/css">
        .completionList {
            border: solid 1px #444444;
            margin: 0px;
            padding: 2px;
            height: 100px;
            overflow: auto;
            background-color: #FFFFFF;
            width: 300px !important;
        }

        .listItem {
            color: #1C1C1C;
        }

        .itemHighlighted {
            background-color: #ffc0c0;
        }

        .TabStyle .ajax__tab_header {
            cursor: pointer;
            background-color: #f1f1f1;
            font-size: 14px;
            font-weight: bold;
            font-family: Arial, Helvetica, sans-serif;
            height: 30px;
            border-bottom: 1px solid #bebebe;
            width: 500px;
        }

        .TabStyle .ajax__tab_active .ajax__tab_tab {
            border: 1px solid;
            border-color: #bebebe #bebebe #e1e1e1 #bebebe;
            background-color: #e1e1e1;
            padding: 5px;
            border-bottom: none;
        }

            .TabStyle .ajax__tab_active .ajax__tab_tab:hover {
                border: 1px solid;
                border-color: #bebebe #bebebe #e1e1e1 #bebebe;
                background-color: #e1e1e1;
                padding: 5px;
                border-bottom: none;
            }

        .TabStyle .ajax__tab_tab {
            border: 1px solid;
            border-color: #e1e1e1 #e1e1e1 #bebebe #e1e1e1;
            background-color: #f1f1f1;
            color: #777777;
            cursor: pointer;
            text-decoration: none;
            padding: 5px;
        }

            .TabStyle .ajax__tab_tab:hover {
                border: 1px solid;
                border-color: #bebebe #bebebe #e1e1e1 #bebebe;
                background-color: #e1e1e1;
                color: #777777;
                cursor: pointer;
                text-decoration: none;
                padding: 5px;
                border-bottom: none;
            }

        .TabStyle .ajax__tab_active .ajax__tab_tab, .TabStyle .ajax__tab_tab, .TabStyle .ajax__tab_header .ajax__tab_tab {
            margin: 0px 0px 0px 0px;
        }

        .TabStyle .ajax__tab_body {
            font-family: Arial, Helvetica, sans-serif;
            font-size: 10pt;
            border-top: 0;
            border: 1px solid #bebebe;
            border-top: none;
            padding: 5px;
            background-color: #e1e1e1;
            width: 500px;
        }

        .auto-style1 {
            width: 10px;
        }
    </style>

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
    </table>
    <%-- <asp:HiddenField ID="HidDatafieldID" runat="server" />
    <asp:HiddenField ID="hiddTenderRefId" runat="server" />
    <asp:HiddenField ID="hdnSrc" runat="server" Value="" />
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <asp:HiddenField ID="HiddenField2" runat="server" Value="" />--%>
    <table width="100%" style="border: 2px solid #fdb813; border-radius: 8px; padding: 5px;">

        <%-- <asp:Panel ID="DataEntryControls" runat="server">--%>
        <tr valign="top">
            <td>
                <table>
                    <tr>

                        <td>
                            <asp:Label ID="lblInstitutionname" Font-Bold="true" runat="server" Text="Institution Name"></asp:Label>
                            &nbsp;<asp:TextBox ID="TxtInstitutionname" Font-Bold="true" runat="server"></asp:TextBox>&nbsp

                            <asp:Label ID="Label3" Font-Bold="true" runat="server" Text="Tender Reference No"></asp:Label>
                            &nbsp;<asp:TextBox ID="txtTenderRefId" runat="server"></asp:TextBox>
                            <asp:Label ID="Label4" Font-Bold="true" runat="server" Text="Institution Location"></asp:Label>&nbsp;&nbsp;<asp:TextBox ID="txtinstlocation" runat="server"></asp:TextBox>
                            <asp:Button ID="btnprv" runat="server" Text="Search" CssClass="btnprv" OnClick="btnprv_Click" />

                        </td>

                    </tr>
                    <tr id="trGrid" runat="server" style="display: none">
                        <td>

                            <asp:TabContainer runat="Server" ID="Search" ActiveTabIndex="0" CssClass="TabStyle"
                                OnActiveTabChanged="Search_ActiveTabChanged" AutoPostBack="true" Width="750px" Style="overflow-y: auto; height: auto">
                                <asp:TabPanel ID="tabviestatus" runat="server">
                                    <HeaderTemplate>View Document Status</HeaderTemplate>
                                    <ContentTemplate>
                                        <%--            <asp:UpdatePanel ID="UpdatePanel2_BS" runat="server">
                                            <ContentTemplate>--%>

                                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" AllowPaging="True"
                                            PageSize="5" CssClass="mGrid" OnSelectedIndexChanged="GridView1_SelectedIndexChanged">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Tender Reference No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbltenderrefno" runat="server" Text='<%# Eval("TenderRefNo") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Instituton Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblinstitutionname" runat="server" Text='<%# Eval("InstitutonName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Path" Visible="False">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbpath" runat="server" Text='<%# Eval("PODoc") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="RefID" Visible="False">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblrefid" runat="server" Text='<%# Eval("id") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ProcessID" Visible="False">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblProcessId" runat="server" Text='<%# Eval("WorkflowStageFieldData_iProcessID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:CommandField ShowSelectButton="True" />
                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                    <%--    </asp:UpdatePanel>
                                    </ContentTemplate>--%>
                                </asp:TabPanel>
                                <asp:TabPanel ID="Tabviewdownload" runat="server">
                                    <HeaderTemplate>View/Download Documents</HeaderTemplate>
                                    <ContentTemplate>
                                        <%-- <asp:CheckBoxList ID="chkListDocuments" runat="server" AutoPostBack="True" OnSelectedIndexChanged="chkListDocuments_SelectedIndexChanged"></asp:CheckBoxList>--%>
                                        <div id="Docview" style="overflow-y: auto; overflow-x: auto">
                                            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" OnPageIndexChanging="GridView1_PageIndexChanging">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Documents Type">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblcontractno" runat="server" Text='<%# Eval("Document Type") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Documents">
                                                        <ItemTemplate>

                                                            <asp:ImageButton ID="btncontractdocs" runat="server" ImageUrl="~/Images/view.png" OnClick="btncontractdocs_Click" Width="25px" />
                                                            <asp:HiddenField ID="hidcontractdocs" runat="server" Value='<%# Eval("Path") %>' />

                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </ContentTemplate>



                                </asp:TabPanel>

                            </asp:TabContainer>

                        </td>

                        <td style="text-align: left">
                            <div id="divstatus" runat="server">
                                <asp:Label ID="Label2" Font-Bold="true" runat="server" Text="Status"></asp:Label>
                                &nbsp;<asp:DropDownList ID="DDLStatus" runat="server" OnSelectedIndexChanged="DDLStatus_SelectedIndexChanged">
                                    <asp:ListItem Selected="True" Value="0">--Select--</asp:ListItem>
                                    <asp:ListItem Value="640">Approved</asp:ListItem>
                                    <asp:ListItem Value="641">Mis-Matched</asp:ListItem>
                                </asp:DropDownList>

                                &nbsp;<asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btnprv" OnClick="btnSave_Click" />
                            </div>
                        </td>

                    </tr>


                </table>
            </td>
        </tr>
        <%-- </asp:Panel>--%>
    </table>

    <table width="100%">
        <tr valign="top">

            <td width="50%" style="padding-right: 10px;">
                <%-- <asp:UpdatePanel ID="UpdatePanel7" runat="server" RenderMode="Inline">
                    <ContentTemplate>--%>
                <iframe id="Iframe1" runat="server" width="100%" class="frm-iframe"></iframe>
                <%--   </ContentTemplate>
                </asp:UpdatePanel>--%>
            </td>
            <td style="padding-right: 5px;">
                <%--              <asp:UpdatePanel ID="UpdatePanel3" runat="server" RenderMode="Inline">
                    <ContentTemplate>--%>
                <iframe id="myiframe" runat="server" width="100%" class="frm-iframe"></iframe>
                <%--         </ContentTemplate>
                </asp:UpdatePanel>--%>
            </td>
        </tr>


    </table>

    <asp:HiddenField ID="HidDatafieldID" runat="server" />
    <asp:HiddenField ID="hiddTenderRefId" runat="server" />
    <asp:HiddenField ID="hdnSrc" runat="server" Value="" />
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <asp:HiddenField ID="HiddenField2" runat="server" Value="" />
    <asp:HiddenField ID="HidProcessid" runat="server" />
    <asp:HiddenField ID="HidDocid" runat="server" Value="" />
    <asp:HiddenField ID="HidOrgfilepath" runat="server" Value="" />
    <asp:HiddenField ID="HidOrgFilename" runat="server" Value="" />
     <asp:HiddenField ID="HidProductCategory" runat="server" Value="" />

</asp:Content>
