<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/SecureMaster.Master" CodeBehind="ReadOCRText.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.ReadOCRText" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <title></title>
    <link href="../../css/workflow_styles.css" rel="stylesheet" type="text/css" />
    <link id="Link2" rel="icon" href="<%= Page.ResolveClientUrl("~/Images/favicon.ico") %>"/>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="GVDiv" style=" margin-top: 10px; ">
 <asp:DataList ID="dataListOCRText" runat="server" RepeatDirection="Horizontal" RepeatColumns="1">
<ItemTemplate>
<table cellpadding="2" cellspacing="0" class="Item">
    <tr>
        <td class="header" align="center" >
            <b><u>
              OCR TEXT</u></b>
        </td>
    </tr>
    <tr>
        <td class="body" style="padding-top:35px;">          
            <%# Eval("DocumentOCR_tOCRContent")%>
          
        </td>
    </tr>
</table>
</ItemTemplate>
<FooterTemplate>
<asp:Label Visible='<%#bool.Parse((dataListOCRText.Items.Count==0).ToString())%>' runat="server" ID="lblNoRecord" Text="No Record Found!"></asp:Label>
</FooterTemplate>
</asp:DataList>
<div style="text-align:center;padding-top: 25px;">
<asp:Repeater ID="rptPager" runat="server" >
    <ItemTemplate>
        <asp:LinkButton ID="lnkPage" runat="server" Text='<%#Eval("Text") %>' CommandArgument='<%# Eval("Value") %>'
            CssClass='<%# Convert.ToBoolean(Eval("Enabled")) ? "page_enabled" : "page_disabled" %>'
            OnClick="Page_Changed" OnClientClick='<%# !Convert.ToBoolean(Eval("Enabled")) ? "return false;" : "" %>'></asp:LinkButton>
   </ItemTemplate>
</asp:Repeater>

        <asp:Button ID="btnCancel" runat="server" Text="Go Back" OnClick="btnCancel_Click" /></div>
</div>
</asp:Content>
