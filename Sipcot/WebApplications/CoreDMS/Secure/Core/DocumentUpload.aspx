<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="DocumentUpload.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.DocumentUpload"
    EnableEventValidation="false" %>
<%@ Register Src="PDFViewer.ascx" TagName="PDFViewer" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        function preventBack() { window.history.forward(); }
        setTimeout("preventBack()", 0);
        window.onunload = function () { null };
    </script>
    <script language="javascript" type="text/javascript">
                $(document).ready(function () {
                    loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageNo.ClientID %>";
            CountControlsID = "<%=hdnCountControls.ClientID %>";
                });
    </script>
    <script type="text/javascript">

        function validate() {

            var msgControl = "#<%= divMsg.ClientID %>";
            var pnlStatus = document.getElementById("<%= divMsg.ClientID %>");


            pnlStatus.style.display = 'block';

       
            var doctype = document.getElementById('<%=cmbDocumentType.ClientID %>');
            var dept = document.getElementById('<%=cmbDepartment.ClientID %>');
            //$("#<%= txtKeyword.ClientID %>").val() = $("#<%= txtRefid.ClientID %>").val();
            //var keyword = $.trim($("#<%= txtKeyword.ClientID %>").val());
            var keyword = $("#<%= txtRefid.ClientID %>").val();
            var count = $("#<%=hdnCountControls.ClientID %>").val();
            var IndexNames = $("#<%=hdnIndexNames.ClientID %>").val().split('|');
            var MinLen = $("#<%=hdnIndexMinLen.ClientID %>").val().split('|');
            var EntryType = $("#<%=hdnIndexType.ClientID %>").val().split('|');
            var DataType = $("#<%=hdnIndexDataType.ClientID %>").val().split('|');
            var Mandatory = $("#<%=hdnMandatory.ClientID %>").val().split('|');
            //alert(Mandatory);
            var Controlnames = $("#<%=hdnControlNames.ClientID %>").val().split('|');
            var retval = true;
            if (keyword.length < 2) {
                $(msgControl).html("Please select both Document Type and Department!");
                return false;
            }
            //alert("");

            for (i = 0; i < parseInt(count); i++) {
                if (EntryType[i] != "Multiple Field Selection") {

                    //DMS5-4101M replaced the space with empty string
                    var controlname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_' + IndexNames[i].replace(/\s+/g, '');
                    var Index = document.getElementById(controlname);
                    if (DataType[i] == "DateTime") {
                        if ((parseInt(Index.value.length) < 10) && (Mandatory[i] == "true")) {
                            $(msgControl).html(IndexNames[i] + " field date not in correct format!");
                            return false;
                        }
                    }
                    else if (DataType[i] == "Boolen") {
                        if ((parseInt(Index.value.length) < 1) && (Mandatory[i] == "true")) {
                            $(msgControl).html(IndexNames[i] + " field should contain '0' or '1'!");
                            return false;
                        }
                    }
                    else {
                        if ((parseInt($.trim(Index.value).length) == 0) && (Mandatory[i] == "true")) {
                            $(msgControl).html(IndexNames[i] + " field cannot be blank!");
                            return false;
                        }
                        else if ((parseInt($.trim(Index.value).length) < parseInt(MinLen[i].length)) && (Mandatory[i] == "true")) {
                            $(msgControl).html(IndexNames[i] + " field should contain atleast " + MinLen[i] + " values!");
                            return false;
                        }
                    }
                }

                else {

                    //DMS5-4101M replaced the space with empty string
                    var controlname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_' + IndexNames[i].replace(/\s+/g, '');
                    var Index = document.getElementById(controlname);
                    if ((Index != null) && (Index.options[Index.selectedIndex].text.toLowerCase() == "--select--") && (Mandatory[i] == "true")) {
                        $(msgControl).html("Please select " + IndexNames[i] + "!");
                        return false;
                    }
                    //DMS5-4101M replaced the space with empty string
                    controlname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_' + Controlnames[i].replace(/\s+/g, '');
                    Index = document.getElementById(controlname);

                    if ((Index.options[Index.selectedIndex].text.toLowerCase() == "--select--") && (Mandatory[i] == "true")) {
                        $(msgControl).html("Please select " + IndexNames[i] + "!");
                        return false;
                    }
                }

            }

            return true;
        }

        function SetHiddenVal(param) {
            if (param == "Dynamic") {
                document.getElementById("<%= hdnDynamicControlIndexChange.ClientID %>").value = "1";
            }
            else {
                document.getElementById("<%= hdnDynamicControlIndexChange.ClientID %>").value = "0";
            }
            return true;
        }

        //function maxsize(sender, args)
        //{
        //    alert("Max File size allowed 24mb...!");
        //    var err = new Error();
        //    err.name = 'My API Input Error';
        //    err.message = 'Max Size!';;
        //    throw (err);
        //    return false;
        //}

        function uploadStart(sender, args) {
            document.getElementById("<%= hdnPreviewCheckBoxChecked.ClientID %>").value = document.getElementById("<%= chkPreviewRequired.ClientID %>").checked;
            var result;
            var msgControl = "#<%= divMsg.ClientID %>";
            var filename = args.get_fileName();

            var filext = filename.substring(filename.lastIndexOf(".") + 1).toLowerCase();
            var doctype = document.getElementById('<%=cmbDocumentType.ClientID %>');
            var dept = document.getElementById('<%=cmbDepartment.ClientID %>');
            if (doctype.value != '0' && dept.value != '0') {
                if (filext == 'tif' || filext == 'tiff' || filext == 'pdf') {
                    return true;
                }
                else {
                    var err = new Error();
                    err.name = 'My API Input Error';
                    err.message = 'Please select supported Files!';


                    ///document.getElementById('<%=AsyncFileUpload1.ClientID %>').innerText = "";

                    throw (err);
                    return false;
                }
            }
            else {
                //you cannot cancel the upload using set_cancel(true)
                //cause an error
                //will  automatically trigger event OnClientUploadError
                var err = new Error();
                err.name = 'My API Input Error';
                err.message = 'Please select both Document Type and Department!';;
                throw (err);
                return false;
            }
        }
        function enablesave() {
            document.getElementById("<%= hdnPreviewCheckBoxChecked.ClientID %>").value = document.getElementById("<%= chkPreviewRequired.ClientID %>").checked;
                document.getElementById('<%=btnSave.ClientID %>').disabled = false;

                document.getElementById('<%=btnSubmit.ClientID%>').click();
                if (document.getElementById("<%= chkPreviewRequired.ClientID %>").checked == true) {
                    document.getElementById('<%=btnPreview.ClientID %>').disabled = false;

                }
                document.getElementById("<%= chkPreviewRequired.ClientID %>").disabled = true;
        }


        function loadImageAndAnnotations(imgPath) {

            // Call annotation library setImage() function to load image to viewer
            setImage(imgPath, true);

            //Fit Width
            setTimeout(function () {
                var zoomSelect = document.getElementById("zoomSelect");
                zoomSelect.selectedIndex = 2;
                $("#zoomSelect").change();
            }, 100);
        }

        function navigationHandler(action) {
            var PageNo = parseInt(document.getElementById("<%=hdnPageNo.ClientID %>").value, "10");
                var PagesCount = parseInt(document.getElementById("<%=hdnPagesCount.ClientID %>").value, "10");
                if (typeof PageNo != 'undefined' && typeof PagesCount != 'undefined') {

                    // First Page
                    if (action.toUpperCase() == 'FIRST' && PageNo > 0 && PageNo <= PagesCount && PageNo != 1) {
                        document.getElementById("<%= hdnAction.ClientID %>").value = action;
                        document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                    }

                    // Previous page
                    else if (action.toUpperCase() == 'PREVIOUS' && PageNo > 1 && PageNo <= PagesCount) {
                        document.getElementById("<%= hdnAction.ClientID %>").value = action;
                        document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                    }

                    // Next page
                    else if (action.toUpperCase() == 'NEXT' && PageNo > 0 && PageNo < PagesCount) {
                        document.getElementById("<%= hdnAction.ClientID %>").value = action;
                        document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                    }

                    // Last page
                    else if (action.toUpperCase() == 'LAST' && PageNo > 0 && PageNo <= PagesCount && PageNo != PagesCount) {
                        document.getElementById("<%= hdnAction.ClientID %>").value = action;
                        document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                    }

                    // Goto page
                    else if (action.toUpperCase() == 'GOTO' && PageNo > 0 && PageNo <= PagesCount) {
                        document.getElementById("<%= hdnAction.ClientID %>").value = action;
                        document.getElementById("<%= btnCallFromJavascript.ClientID %>").click();
                    }
                }
            }

    </script>
    <style type="text/css">
        .auto-style1 {
            height: 30px;
        }

        .auto-style2 {
            height: 33px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" value="" name="hdnpagecount1" id="hdnpagecount1" />
    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Style="color: #808080" Text="Home  &gt;  Document Upload">
    </asp:Label>
    <div class="GVDiv">
        <div class="DivInlineBlock">
            <asp:Label ID="Label1" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
            <asp:Label ID="Label2" runat="server" CssClass="CurrentPath" Text="- Indicates mandatory fields">
            </asp:Label>
            <div id="divMsg" runat="server" style="color: Red">
            </div>
            <fieldset>
                <table style="width: 1200px;">
                    <tr>
                        <td style="vertical-align: top;">
                            <table style="width: 425px;">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                                        <asp:Label ID="Label4" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="cmbDocumentType" runat="server" OnSelectedIndexChanged="cmbDocumentType_SelectedIndexChanged"
                                                    AutoPostBack="True">
                                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                </asp:DropDownList>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label5" runat="server" CssClass="LabelStyle" Text="Document Type"></asp:Label>
                                        <asp:Label ID="Label11" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*">
                                        </asp:Label>
                                    </td>
                                    <td colspan="2">
                                        <asp:UpdatePanel ID="UpdatePanel41" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="cmbDepartment" runat="server" OnSelectedIndexChanged="cmbDepartment_SelectedIndexChanged"
                                                    AutoPostBack="True">
                                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                </asp:DropDownList>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr style="visibility: hidden">
                                    <td>
                                        <asp:Label ID="lblPreviewRequired" runat="server" CssClass="LabelStyle" Text="Preview Required"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkPreviewRequired" Text="(Selection WILL slow down the upload)"
                                            runat="server" Checked="true" />
                                    </td>
                                </tr>
                                <tr style="visibility: hidden">
                                    <td>
                                        <asp:Label ID="lblPreviewRequired0" runat="server" CssClass="LabelStyle" Text="Select File Type"></asp:Label>
                                    </td>
                                    <td>
                                        <%-- <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                                            <ContentTemplate>--%>
                                        <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="RadioButtonList1_SelectedIndexChanged" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="PDFFiles" Selected="True">Pdf,Tiff,Img</asp:ListItem>
                                            <%--<asp:ListItem Value="DWGFiles">Dwg,Dxf </asp:ListItem>--%>
                                        </asp:RadioButtonList>
                                        <%-- </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="btnPreview" EventName="RadioButtonList1" />
                                            </Triggers>
                                        </asp:UpdatePanel>--%>
                                    </td>
                                </tr>
                                <tr id="PDF" runat="server">

                                    <td class="auto-style1">
                                        <asp:Label ID="Label6" runat="server" CssClass="LabelStyle" Text="Select File to Upload" Width="120px"></asp:Label>
                                        <asp:Label ID="Label7" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                                        <br />
                                        <span style="font-size: 10px">(max file size allowed 24mb *)</span>
                                    </td>
                                    <td colspan="2" class="auto-style1">
                                        <table cellpadding="0" cellspacing="0" border="0">
                                            <tr>
                                                <td>
                                                    <asp:AsyncFileUpload ID="AsyncFileUpload1" runat="server" AsyncPostBackTimeout="1600"
                                                        CssClass="LabelStyle" Width="160px" CompleteBackColor="Lime" ErrorBackColor="Red"
                                                        ThrobberID="Throbber" OnClientUploadStarted="uploadStart" OnClientUploadComplete="enablesave"
                                                        OnUploadedComplete="AsyncFileUpload1_UploadedComplete" UploadingBackColor="#66CCFF" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr id="DWG" runat="server" style="visibility: hidden">
                                    <%-- style="visibility: hidden"--%>
                                    <td>
                                        <asp:Label ID="Label17" runat="server" CssClass="LabelStyle" Text="Select File to Upload" Width="120px"></asp:Label>
                                        <asp:Label ID="Label16" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*"></asp:Label>
                                        <br />
                                    </td>
                                    <td colspan="2">
                                        <asp:FileUpload ID="FileUpload1" runat="server" Width="160px" onchange="Button1.click()" />
                                        &nbsp;
                                         <asp:Button ID="Button11" runat="server" Text="CADPREVIEW" OnClick="Button1_Click" CssClass="btnpreview" />

                                    </td>

                                </tr>
                                <tr>
                                    <td class="auto-style2">
                                        <asp:Label ID="Label9" runat="server" CssClass="LabelStyle" Text="Ref ID"></asp:Label>
                                    </td>
                                    <td class="auto-style2">
                                        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                            <ContentTemplate>
                                                <asp:TextBox ID="txtRefid" runat="server" ReadOnly="True" Height="15px" Width="90px" Onchange="fillkeyword()"></asp:TextBox>
                                                <asp:Button ID="btnPreview" runat="server" OnClick="btnPreview_Click" TagName="Read"
                                                    Text="Preview" CssClass="btnpreview" />

                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="cmbDepartment" EventName="SelectedIndexChanged" />
                                                <asp:AsyncPostBackTrigger ControlID="cmbDocumentType" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>

                                    <td class="auto-style2">
                                        <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                        </asp:UpdatePanel>
                                        <asp:Label ID="Throbber" runat="server" Style="display: none">
                            <img alt="Loading..." src="<%= Page.ResolveClientUrl("~/Images/indicator.gif")%>" /></asp:Label>
                                    </td>
                                </tr>
                                <tr style="display: none">
                                    <td>
                                        <asp:Label ID="Label12" runat="server" CssClass="LabelStyle" Text="Keywords "></asp:Label>
                                        <asp:Label ID="Label10" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*">
                                        </asp:Label>
                                    </td>
                                </tr>
                                <tr style="display: none">
                                    <td colspan="3">
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                            <ContentTemplate>
                                                <asp:TextBox ID="txtKeyword" runat="server" Width="330px" Height="81px" TextMode="MultiLine"></asp:TextBox>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr style="display: none">
                                    <td>
                                        <asp:Label ID="Label13" runat="server" CssClass="LabelStyle" Text="Keyword Suggestions">
                                        </asp:Label>
                                    </td>
                                </tr>
                                <tr style="display: none">
                                    <td colspan="3">
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                            <ContentTemplate>
                                                <asp:Panel ID="pnlKeywords" runat="server" CssClass="OutlinePanel" Width="330px">
                                                    <asp:Button ID="Button1" runat="server" Text="Keys" Height="26px" OnCommand="Button4_Command"
                                                        TagName="Read" />
                                                    &nbsp;&nbsp;&nbsp;
                                                    <asp:Button ID="Button2" runat="server" Height="26px" Text="Keys" OnCommand="Button4_Command"
                                                        TagName="Read" />
                                                    &nbsp;&nbsp;&nbsp;
                                                    <asp:Button ID="Button3" runat="server" Height="26px" Text="Keys" OnCommand="Button4_Command"
                                                        TagName="Read" />
                                                    &nbsp;&nbsp;&nbsp;
                                                    <asp:Button ID="Button4" runat="server" Height="26px" Text="Keys" OnCommand="Button4_Command"
                                                        TagName="Read" />
                                                    &nbsp;&nbsp;&nbsp;
                                                    <asp:Button ID="Button5" runat="server" Height="26px" Text="Keys" OnCommand="Button4_Command"
                                                        TagName="Read" />
                                                    &nbsp;&nbsp;&nbsp;
                                                    <asp:Button ID="Button6" runat="server" Height="26px" Text="Keys" OnCommand="Button4_Command"
                                                        TagName="Read" />
                                                    &nbsp;
                                                    <asp:Button ID="Button7" runat="server" Height="26px" Text="Keys" OnCommand="Button4_Command"
                                                        TagName="Read" />
                                                    &nbsp;
                                                    <asp:Button ID="Button8" runat="server" Height="26px" Text="Keys" OnCommand="Button4_Command"
                                                        TagName="Read" />
                                                    &nbsp;
                                                    <asp:Button ID="Button9" runat="server" Height="26px" Text="Keys" OnCommand="Button4_Command"
                                                        TagName="Read" />
                                                    &nbsp;
                                                    <asp:Button ID="Button10" runat="server" Height="26px" Text="Keys" OnCommand="Button4_Command"
                                                        TagName="Read" />
                                                    &nbsp;
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <table cellpadding="3" cellspacing="3">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label14" runat="server" CssClass="LabelStyle" Text="Index Properties">
                                                    </asp:Label>
                                                    <asp:Label ID="Label15" runat="server" CssClass="MandratoryFieldMarkStyle" Text="*">
                                                    </asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="style9">
                                                    <asp:UpdatePanel ID="Indexupdatepanel" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                                        <ContentTemplate>
                                                            <asp:Panel ID="pnlIndexpro" runat="server">
                                                            </asp:Panel>
                                                        </ContentTemplate>
                                                        <Triggers>
                                                            <asp:AsyncPostBackTrigger ControlID="cmbDepartment" EventName="SelectedIndexChanged" />
                                                            <asp:AsyncPostBackTrigger ControlID="cmbDocumentType" EventName="SelectedIndexChanged" />
                                                        </Triggers>
                                                    </asp:UpdatePanel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3">
                                                    <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                                                    <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                                                    <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                                                    <asp:HiddenField ID="hdnFileLocation" runat="server" Value="" />
                                                    <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                                                        <ContentTemplate>
                                                            <asp:HiddenField ID="hdnCountControls" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnIndexNames" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnIndexMinLen" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnIndexType" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnIndexDataType" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnMandatory" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnSubDrpID" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnPageCount" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnSrc" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnUploaded" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnControlNames" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnDynamicControlIndexChange" runat="server" Value="0" />
                                                            <asp:HiddenField ID="hdnPagesCount" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnPageNo" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnAnnotaionXML" runat="server" />
                                                            <asp:HiddenField ID="hdnAnnotionwithDoc" runat="server" />
                                                            <asp:HiddenField ID="hdnMainvalueid" runat="server" Value="" />
                                                            <asp:HiddenField ID="hdnPreviewCheckBoxChecked" runat="server" Value="" />
                                                            <asp:HiddenField ID="Hidcontentlength" runat="server" />
                                                            <asp:HiddenField ID="Hidconttype" runat="server" />
                                                            <asp:HiddenField ID="btnenable" runat="server" />
                                                            <asp:HiddenField ID="HidMisFile" runat="server" />
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <table width="100%">
                                            <tr>
                                                <td class="style3" style="width: auto; height: auto" align="center">
                                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btnsave" OnClick="btnSave_Click"
                                                        CausesValidation="False" TagName="Upload" Enabled="false" />
                                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btncancel" CausesValidation="False"
                                                        OnClick="btnCancel_Click" TagName="Read" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Always" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:Panel ID="NavigatePanel" runat="server">
                                        <input id="btnFirst" onclick="navigationHandler('FIRST');" type="button" class="btnfirst"
                                            title="First" />
                                        <input id="btnPrevious" onclick="navigationHandler('PREVIOUS');" type="button" class="btnleftarrow"
                                            title="Previous" />
                                        <input id="btnNext" onclick="navigationHandler('NEXT');" type="button" class="btnrightarrow"
                                            title="Next" />
                                        <input id="btnLast" onclick="navigationHandler('LAST');" type="button" class="btnlast"
                                            title="Last" />
                                        <asp:UpdatePanel ID="upPagecount" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                            <ContentTemplate>
                                                <asp:DropDownList ID="ddlpagecount" runat="server" Style="width: 50px;">
                                                </asp:DropDownList>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:Panel>
                                      <iframe id="myiframe" runat="server" width="800" height="600" frameborder="0" style="background-color: white;"></iframe>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <%--                 <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>--%>
                           

                            <div id="pdfviewer" runat="server">
                                
                                <uc1:PDFViewer ID="PDFViewer1" runat="server" style="display:none" />
                            </div>
                            <div style="margin: 0 auto; text-align: center; background-color: white;" id="Cadviewer" runat="server">
                               
                            </div>
                            <%-- </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnPreview" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>--%>
                        </td>
                    </tr>
                </table>
            </fieldset>
            <div class="style1" style="height: 25px; width: 100px;">
            </div>
            <div class="DivInlineBlock">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <asp:Button ID="btnSubmit" class="HiddenButton" runat="server" Text="GenerateKeywords"
                            TagName="Read" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnCallFromJavascript" class="HiddenButton" runat="server" OnClick="btnCallFromJavascript_Click"
                            TagName="Read" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>
