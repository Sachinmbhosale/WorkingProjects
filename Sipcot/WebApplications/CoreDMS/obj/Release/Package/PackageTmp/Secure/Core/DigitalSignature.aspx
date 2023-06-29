<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="DigitalSignature.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.DigitalSignature" %>

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
        function ShowApplySignaturePopUP() {

            var msgControl = "#<%= divMsg.ClientID %>";
            if (TestCheckBox()) {
                document.getElementById('<%=btnadd.ClientID %>').disabled = true;
                CertificateBG.className = "mdBG";

                Certificate.className = "mdbox";
                return false;
            }
            else {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Select atleast one document.");
                return false;
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

        function ValidateCertificate(sender, args) {


            var result;
            var msgControl = "#<%= divMsg.ClientID %>";
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1);
            var err = new Error();
            if (filext == 'pfx' || filext == 'PFX') {

                return true;
            }
            else {

                err.name = 'My API Input Error';
                err.message = 'Certificate format not supported! (Supported format ' + filext + ')';
                throw (err);
                return false;
            }
        }


        function validateCertficateDetails() {


            var msgControl = "#<%= divCertMsg.ClientID %>";
            var Password = document.getElementById("<%= txtpassword.ClientID %>").value;
            var auther = document.getElementById("<%= txtauther.ClientID %>").value;
            var Title = document.getElementById("<%= txtTitle.ClientID %>").value;
            var subject = document.getElementById("<%= txtsubject.ClientID %>").value;
            var keywods = document.getElementById("<%= txtkeywods.ClientID %>").value;
            var creater = document.getElementById("<%= txtcreater.ClientID %>").value;
            var producer = document.getElementById("<%= txtproducer.ClientID %>").value;
            var Reason = document.getElementById("<%= txtReason.ClientID %>").value;
            var contact = document.getElementById("<%= txtcontact.ClientID %>").value;
            var location = document.getElementById("<%= txtlocation.ClientID %>").value;
            if (Password.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Password cannot be empty!");
                return false;
            }
            else if (auther.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Author cannot be empty!");
                return false;
            }
            else if (Title.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Title  cannot be empty!");
                return false;
            }
            else if (subject.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Subject cannot be empty!");
                return false;
            }
            else if (keywods.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Keywords cannot be empty!");
                return false;
            }
            else if (creater.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Creator cannot be empty!");
                return false;
            }
            else if (producer.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Producer cannot be empty!");
                return false;
            }
            else if (Reason.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Reason cannot be empty!");
                return false;
            }
            else if (contact.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Contact cannot be empty!");
                return false;
            }
            else if (location.length <= 0) {

                $(msgControl).css("color", "Red");
                $(msgControl).html("Location cannot be empty!");
                return false;
            }

            return true;
        }
        function enablesave() {
            document.getElementById('<%=btnadd.ClientID %>').disabled = false;

        }

        function TestCheckBox() {

            var Status = false;
            var TargetBaseControl = document.getElementById('<%= this.grdView.ClientID %>');


            if (TargetBaseControl == null) return false;

            //get target child control.
            var TargetChildControl = "chkSelected";

            //get all the control of the type INPUT in the base control.
            var Inputs = TargetBaseControl.getElementsByTagName("input");

            for (var n = 0; n < Inputs.length; ++n) {
                if (Inputs[n].type == 'checkbox' && Inputs[n].id.indexOf(TargetChildControl, 0) >= 0 && Inputs[n].checked) {

                    Status = true;
                }

            }
            return Status;
        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Digital Signature"></asp:Label>
    <div id="MainDiv">
        <table>
            <tr>
                <td>
                    &nbsp;&nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divMsg" runat="server" style="color: Red">
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="DDLSignedStatus" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DDLSignedStatus_SelectedIndexChanged">
                        <asp:ListItem>Digitally Signed</asp:ListItem>
                        <asp:ListItem>Not Digitally Signed</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;&nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="CheckSelectPageWise" Text="Select PageWise" runat="server" OnCheckedChanged="CheckSelectPageWise_CheckedChanged"
                        AutoPostBack="True" />
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;&nbsp;&nbsp;
                </td>
            </tr>
        </table>
        <div id="GridView">
            <asp:GridView ID="grdView" runat="server" CssClass="mGrid" PagerStyle-CssClass="pgr"
                EmptyDataText="No record found." AllowSorting="True" AlternatingRowStyle-CssClass="alt"
                CellPadding="10" CellSpacing="5" OnRowDataBound="grdView_RowDataBound" OnRowCommand="grdView_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Select">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelected" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelected_CheckedChanged" />
                        </ItemTemplate>
                        <HeaderStyle CssClass="GridItem1" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="View">
                        <ItemTemplate>
                            <asp:Button ID="btnview" runat="server" Text="" CssClass="ViewDigitalSignature" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                CommandName="View" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                    PageButtonCount="5" PreviousPageText=" " />
                <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
            </asp:GridView>
        </div>
        <div class="PagingTD" id="PageDiv" runat="server">
            <div class="PagingControl">
                <table border="0" cellpadding="0" cellspacing="0" style="vertical-align: middle;
                    padding-left: 443px;">
                    <tr>
                        <td style="font-size: 8.5pt;" class="style7">
                            Rows per page
                            <asp:DropDownList ID="ddlRows" Style="margin: 5px 5px 5px 5px; width: 50px;" runat="server"
                                AutoPostBack="True" OnSelectedIndexChanged="ddlRows_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                        </td>
                        <td>
                            <asp:Button ID="lnkbtnFirst" CssClass="GridPageFirstInactive" ToolTip="First" CommandName="First"
                                runat="server" OnCommand="GetPageIndex"></asp:Button>
                        </td>
                        <td style="width: 6px">
                        </td>
                        <td>
                            <asp:Button ID="lnkbtnPre" CssClass="GridPagePreviousInactive" ToolTip="Previous"
                                CommandName="Previous" runat="server" OnCommand="GetPageIndex"></asp:Button>
                        </td>
                        <td style="width: 6px">
                        </td>
                        <td style="font-size: 8.5pt;">
                            Page
                            <asp:DropDownList ID="ddlPage" runat="server" AutoPostBack="True" Width="50px" OnSelectedIndexChanged="ddlPage_SelectedIndexChanged">
                            </asp:DropDownList>
                            of
                            <asp:Label ID="lblTotalPages" runat="server"></asp:Label>
                        </td>
                        <td style="width: 6px">
                        </td>
                        <td>
                            <asp:Button ID="lnkbtnNext" CssClass="GridPageNextInactive" ToolTip="Next" runat="server"
                                CommandName="Next" OnCommand="GetPageIndex"></asp:Button>
                        </td>
                        <td style="width: 6px">
                        </td>
                        <td>
                            <asp:Button ID="lnkbtnLast" CssClass="GridPageLastInactive" ToolTip="Last" runat="server"
                                CommandName="Last" OnCommand="GetPageIndex"></asp:Button>
                        </td>
                    </tr>
                </table>
                <asp:Label ID="lblMessage" runat="server" EnableViewState="false"></asp:Label>
            </div>
        </div>
        &nbsp; &nbsp; &nbsp; &nbsp;
    </div>
    <div id="ViewerDivBG" class="mdNone">
    </div>
    <div id="ViewerDiv" class="mdNone">
        <asp:Button ID="btnCancel" runat="server" Text="" OnClientClick="HideMD('Viewer'); return false;"
            CssClass="CloseButton" CausesValidation="false" meta:resourcekey="btnCancel" />
        <div id="divContainer" style="max-height: 630px; overflow-y: auto;">
            <uc1:PDFViewer ID="PDFViewer1" runat="server" />
        </div>
    </div>
    <div id="CertificateBG" class="mdNone">
    </div>
    <div id="Certificate" class="mdNone">
        <table>
            <tr>
                <td>
                    <div id="divCertMsg" runat="server" style="color: Red">
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnCertificateCancel" runat="server" Text="" OnClientClick="HideMD('Certficate'); return false;"
                        CssClass="CloseButton" CausesValidation="false" meta:resourcekey="btnCancel" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblCertificate" runat="server" Text="Certificate"></asp:Label>
                </td>
                <td>
                    <asp:AsyncFileUpload ID="AsyncFileUpload1" runat="server" AsyncPostBackTimeout="1600"
                        CssClass="LabelStyle" Width="200px" CompleteBackColor="Lime" ErrorBackColor="Red"
                        OnClientUploadComplete="enablesave" OnClientUploadStarted="ValidateCertificate"
                        ThrobberID="Throbber" UploadingBackColor="#66CCFF" OnUploadedComplete="AsyncFileUpload1_UploadedComplete" />
                    <asp:Label ID="Throbber" runat="server" Style="display: none">
                          <img alt="Loading..." src="<%= Page.ResolveClientUrl("~/Images/indicator.gif")%>" /></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblpassword" runat="server" Text="Password"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtpassword" runat="server" TextMode="Password" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblAuther" runat="server" Text="Author"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtauther" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblTitle" runat="server" Text="Title"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtTitle" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblsubject" runat="server" Text="Subject"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtsubject" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblKeywords" runat="server" Text="Keywords"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtkeywods" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblcreater" runat="server" Text="Creator"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtcreater" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblproducer" runat="server" Text="Producer"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtproducer" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblReason" runat="server" Text="Reason"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtReason" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblcontact" runat="server" Text="Contact"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtcontact" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbllocation" runat="server" Text="Location"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtlocation" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkvisibleSignature" Text="Check to make Signature visible" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;&nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnadd" runat="server" class="DigitalSign" Text="Add" OnClientClick="return validateCertficateDetails();"
                        OnClick="btnadd_Click" />
                </td>
            </tr>
        </table>
    </div>
    <asp:Button ID="btnCallFromJavascript" class="HiddenButton" runat="server" OnClick="btnCallFromJavascript_Click"
        TagName="Read" />
    <asp:Button ID="btnAddDigitalSignature" class="DigitalSign" runat="server" OnClientClick="return ShowApplySignaturePopUP();"
        Text="Apply" />
    <asp:Button ID="btnRemoveDigitalSignature" class="btnDocclear" runat="server" Text="Remove"
        OnClick="btnRemoveDigitalSignature_Click" />
    <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
    <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
    <asp:HiddenField ID="hdnSearchCriteria" runat="server" Value="" />
    <asp:HiddenField ID="hdnSearchAction" runat="server" Value="" />
    <asp:HiddenField ID="hdnAction" runat="server" Value="" />
    <asp:HiddenField ID="hdnFileLocation" runat="server" Value="" />
    <asp:HiddenField ID="hdnEncrpytFileName" runat="server" Value="" />
    <asp:HiddenField ID="hdnPageNo" runat="server" Value="" />
    <asp:HiddenField ID="hdnpagecount" runat="server" Value="" />
</asp:Content>
