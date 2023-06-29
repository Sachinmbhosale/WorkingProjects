<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApplicationTabs.ascx.cs"
    Inherits="Lotex.EnterpriseSolutions.WebUI.UserControls.ApplicationTabs" %>
<div style="float: right;">
    <asp:ImageButton ID="imgButtonAdmin" runat="server" ImageUrl="../Workflow/images/header_admin_deselect.png"
        BorderStyle="None" BorderWidth="0px" CssClass="MainMenubtn" PostBackUrl="" />
    <asp:ImageButton ID="imgButtonDMS" runat="server" ImageUrl="../Workflow/images/header_dms_deselect.png"
        CssClass="MainMenubtn" onclick="imgButtonDMS_Click" PostBackUrl="" />
    <asp:ImageButton ID="imgButtonWorkFlow" runat="server" ImageUrl="../Workflow/images/header_workflow_deselect.png"
        CssClass="MainMenubtn" onclick="imgButtonWorkFlow_Click" PostBackUrl="" />

</div>

