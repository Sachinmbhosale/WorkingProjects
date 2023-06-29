<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="DocumentHistoryView.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.DocumentHistoryView" %>

<%@ Register Src="PDFViewer.ascx" TagName="PDFViewer" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
    <link href="../../App_Themes/common/common.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/GridRowSelection.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageNo.ClientID %>";

        });
    </script>
    <script type="text/javascript">

        function navigationHandler(action) {
            var PageNo = parseInt(document.getElementById("<%=hdnPageNo.ClientID %>").value, 10);
            var PagesCount = parseInt(document.getElementById("<%=hdnpagecount.ClientID %>").value, 10);


            if (typeof PageNo != 'undefined' && typeof PagesCount != 'undefined') {


                if (action.toUpperCase() == 'PREVIOUS' && PageNo > 1 && PageNo <= PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }

                // Next page
                else if (action.toUpperCase() == 'NEXT' && PageNo > 0 && PageNo < PagesCount) {
                    document.getElementById("<%= hdnAction.ClientID %>").value = action;
                    document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                }

            }
        }

        function loadImageAndAnnotations(imgPath) {

            ViewerDivBG.className = "mdBG";

            ViewerDiv.className = "mdviewerbox";

            // Call annotation library setImage() function to load image to viewer
            setImage(imgPath, true);

            //Fit Width
            setTimeout(function () {
                var zoomSelect = document.getElementById("zoomSelect");
                zoomSelect.selectedIndex = 2;
                $("#zoomSelect").change();
            }, 100);
        }
        function HideMD(Action) {

            if (Action == 'Viewer') {
                ViewerDivBG.className = "mdNone";

                ViewerDiv.className = "mdNone";
            }
            else {

                CertificateBG.className = "mdNone";

                Certificate.className = "mdNone";

            }
        };

     


       
      
       
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Document History View"></asp:Label>
    <div id="MainDiv">
        <table>
            <tr>
                <td>
                    <div id="divMsg" runat="server" style="color: Red">
                    </div>
                </td>
            </tr>
        </table>
        <div id="GridView" style="max-width:100%;  overflow-x: auto;">
            <asp:GridView ID="grdView" runat="server" CssClass="mGrid" PagerStyle-CssClass="pgr"
                EmptyDataText="No record found." AlternatingRowStyle-CssClass="alt"
                CellPadding="10" CellSpacing="5" OnRowCommand="grdView_RowCommand" 
                OnRowDataBound="grdView_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="View">
                        <ItemTemplate>
                            <asp:Button ID="btnview" runat="server" Text="" CssClass="ViewDigitalSignature" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                CommandName="View" />
                            <asp:Button ID="btnDownload" runat="server" Text="" CssClass="DcoumentDownload" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                CommandName="Download" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                    PageButtonCount="5" PreviousPageText=" " />
                <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
            </asp:GridView>
            <asp:Button ID="btnGotoWorkflow" runat="server" Text="Go Back" TagName="Read" 
                CssClass="btnsearchagain" onclick="btnGotoWorkflow_Click" />
        </div>
    </div>
    <div id="ViewerDivBG" class="mdNone">
    </div>
    <div id="ViewerDiv" class="mdNone">
     <asp:Button ID="btnCancel" runat="server" Text="" OnClientClick="HideMD('Viewer'); return false;"
            CssClass="CloseButton" CausesValidation="false" meta:resourcekey="btnCancel" />
    <div id="divContainer" style="max-height:630px; overflow-y:auto;">       
        <uc1:PDFViewer ID="PDFViewer1" runat="server" />
    </div></div>
    <asp:Button ID="btnCallFromJavascript" Text="" class="HiddenButton" runat="server"
        TagName="Read" OnClick="btnCallFromJavascript_Click" />
    <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
    <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
    <asp:HiddenField ID="hdnSearchCriteria" runat="server" Value="" />
    <asp:HiddenField ID="hdnSearchAction" runat="server" Value="" />
    <asp:HiddenField ID="hdnAction" runat="server" Value="" />
    <asp:HiddenField ID="hdnFileLocation" runat="server" Value="" />
    <asp:HiddenField ID="hdnEncrpytFileName" runat="server" Value="" />
    <asp:HiddenField ID="hdnPageNo" runat="server" Value="" />
    <asp:HiddenField ID="hdnpagecount" runat="server" Value="" />
    <asp:HiddenField ID="hdnSearchPageUrl" runat="server" Value="" />
    <asp:HiddenField ID="hdnDcoumentId" runat="server" Value="" />
    <asp:HiddenField ID="hdnFileName" runat="server" Value="" />
</asp:Content>
