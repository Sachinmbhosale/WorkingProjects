<%@ Page Title="" Language="C#" MasterPageFile="~/General.Master" AutoEventWireup="true"
    CodeBehind="ConfigurationPage.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.ConfigurationPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .input-large
        {
            width: 300px !important;
        }
        .input-xlarge
        {
            width: 600px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="CenterBaseDiv" style="overflow-y: scroll; height: 500px;">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
        <div id="DatabaseSettings">
            <h3>
                Database Settings</h3>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblDatabaseSystem" runat="server" Text="Database System"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlDatabaseSystem" runat="server" AutoPostBack="True" CssClass="input-large"
                            OnSelectedIndexChanged="ddlDatabaseSystem_SelectedIndexChanged">
                            <asp:ListItem Value="SqlServer" Text="SQL SERVER"></asp:ListItem>
                            <asp:ListItem Value="MySql" Text="MY SQL"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblHostname" runat="server" Text="Hostname"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtHostname" runat="server" CssClass="input-large"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblPort" runat="server" Text="Port"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPort" runat="server" CssClass="input-large"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblServer" runat="server" Text="Server"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtServer" runat="server" CssClass="input-large"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDatabase" runat="server" Text="Database"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDatabase" runat="server" CssClass="input-large"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblUsername" runat="server" Text="Username"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="input-large"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblPassword" runat="server" Text="Password"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input-large"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button ID="btnTestConnection" runat="server" Text="Test Connection" OnClick="btnTestConnection_Click" />
                        <asp:Label ID="lblTestConnectionStatus" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div id="DocumentStorageSettings">
            <h3>
                Document Storage Settings</h3>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblUploadedDocumentsPath" runat="server" Text="Uploaded documents path"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtUploadedDocumentsPath" runat="server" CssClass="input-xlarge"
                            OnTextChanged="txtUploadedDocumentsPath_TextChanged" AutoPostBack="True"></asp:TextBox>
                        <asp:Label ID="lblUploadedDocumentsPathStatus" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblArchivedDocumentsPath" runat="server" Text="Archived documents path"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtArchivedDocumentsPath" runat="server" CssClass="input-xlarge"
                            OnTextChanged="txtArchivedDocumentsPath_TextChanged" AutoPostBack="True"></asp:TextBox>
                        <asp:Label ID="lblArchivedDocumentsPathStatus" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblVersionedDocumentsPath" runat="server" Text="Versioned documents path"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtVersionedDocumentsPath" runat="server" CssClass="input-xlarge"
                            OnTextChanged="txtVersionedDocumentsPath_TextChanged" AutoPostBack="True"></asp:TextBox>
                        <asp:Label ID="lblVersionedDocumentsPathStatus" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblTemporaryPath" runat="server" Text="Temporary path"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtTemporaryPath" runat="server" CssClass="input-xlarge" OnTextChanged="txtTemporaryPath_TextChanged"
                            AutoPostBack="True"></asp:TextBox>
                        <asp:Label ID="lblTemporaryPathStatus" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div id="OtherSettings">
            <h3>
                Other Settings</h3>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblBulkUploadInstallerUrl" runat="server" Text="Bulk upload application installer url"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtBulkUploadInstallerUrl" runat="server" CssClass="input-xlarge"
                            OnTextChanged="txtBulkUploadInstallerUrl_TextChanged" AutoPostBack="True"></asp:TextBox>
                        <asp:Label ID="lblBulkUploadInstallerUrlStatus" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDocumentDownloadWebsiteUrl" runat="server" Text="Document download website url"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDocumentDownloadWebsiteUrl" runat="server" CssClass="input-xlarge"
                            OnTextChanged="txtDocumentDownloadWebsiteUrl_TextChanged" 
                            AutoPostBack="True"></asp:TextBox>
                        <asp:Label ID="lblDocumentDownloadWebsiteUrlStatus" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblEnableTextLog" runat="server" Text="Enable text log"></asp:Label>
                        <asp:Label ID="lblEnableTextLogStatus" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkEnableTextLog" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblEnableDatabaseLog" runat="server" Text="Enable database log"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkEnableDatabaseLog" runat="server" />
                        <asp:Label ID="lblEnableDatabaseLogStatus" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
    </div>
</asp:Content>
