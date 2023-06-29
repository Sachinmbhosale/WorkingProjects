<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true" CodeBehind="DocumentTag.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.DocumentTag" %>

<%@ Register Src="PDFViewer.ascx" TagName="PDFViewer" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        .style1 {
            height: 19px;
        }

        .style2 {
            height: 28px;
        }
    </style>
    <%-- <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/PDFPageCount.js") %>"></script>--%>
    <script type="text/javascript">


        function gotoPage() {
            //debugger;
            var page = $("#<%= DDLDrop.ClientID %>").val();
            if (page != null) {

                var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                ContentLoaderDiv.innerHTML = "";
                ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="820" width="800" style="margin-top:0px" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + page + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
                var msgControl = "#<%= divMsg.ClientID %>";
                var UploadID = getParameterByName("id");
                var PageID = $("#<%= DDLDrop.ClientID %>").val();
                var params = UploadID + '|' + PageID;
                document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;
                var Totalpagecount = document.getElementById("<%=Hidtotalpagecount.ClientID %>").value;
                document.getElementById('totalpagescount').innerHTML = Totalpagecount + ' Pages';
                return false;
            }
            else {
                page = 1;
                var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                ContentLoaderDiv.innerHTML = "";
                ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="820" width="800" style="margin-top:0px" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + page + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
                var msgControl = "#<%= divMsg.ClientID %>";
                var UploadID = getParameterByName("id");
                var PageID = $("#<%= DDLDrop.ClientID %>").val();
                var params = UploadID + '|' + PageID;
                document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;
                var Totalpagecount = document.getElementById("<%=Hidtotalpagecount.ClientID %>").value;
                document.getElementById('totalpagescount').innerHTML = Totalpagecount + ' Pages';
                return false;
            }

        }

        

        function firstPage() {

            var page = document.getElementById("<%= DDLDrop.ClientID %>");
            if (page.options.length != 0) {
                page.selectedIndex = 0;
                var UploadID = getParameterByName("id");
                var PageID = $("#<%= DDLDrop.ClientID %>").val();
                var params = UploadID + '|' + PageID;
                document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;

                var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                ContentLoaderDiv.innerHTML = "";
                ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="710" width="800" style="margin-top:0px" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + PageID + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
            }
        }

        function lastPage() {

            var page = document.getElementById("<%= DDLDrop.ClientID %>");
            if (page.options.length != 0) {
                page.selectedIndex = page.options.length - 1;
                var UploadID = getParameterByName("id");
                var PageID = $("#<%= DDLDrop.ClientID %>").val();
                var params = UploadID + '|' + PageID;
                document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;

                var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                ContentLoaderDiv.innerHTML = "";
                ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="710" width="800" style="margin-top:0px" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + PageID + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
            }
        }


        function nextPage() {

            var page = document.getElementById("<%= DDLDrop.ClientID %>");
            if (page.options.length != 0) {


                var setpages = document.getElementById("<%= txtPages.ClientID %>");
                var LastVal = page.options[page.options.length - 1].value;

                var page2 = $("#<%= DDLDrop.ClientID %>").val();

                if (page2 == null) {
                    var PageID = $("#<%= DDLDrop.ClientID %>").val();
                    var SelectedVal = PageID + 1;
                }
                else {
                    var SelectedVal = page.options[page.selectedIndex].value;
                }
                if (parseInt(SelectedVal, 10) != parseInt(LastVal, 10)) {
                    page.selectedIndex = page.selectedIndex + 1;
                    var UploadID = getParameterByName("id");
                    var PageID = $("#<%= DDLDrop.ClientID %>").val();
                    var params = UploadID + '|' + PageID;
                    document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;

                    var str = setpages.value;
                    var n = str.search(PageID);
                    //alert(n);            
                    if (n != "-1") {
                        alert(PageID + " Page is already added");
                        return false;
                    }

                    if (setpages.value == "") {
                        if (page.selectedIndex == "1") {
                            setpages.value = SelectedVal;
                            setpages.value = setpages.value + ',' + PageID;
                        }
                        else {
                            setpages.value = setpages.value + PageID;
                        }

                    }
                    else {
                        setpages.value = setpages.value + ',' + PageID;
                    }


                    var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                    ContentLoaderDiv.innerHTML = "";
                    ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="710" width="800" style="margin-top:0px" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + PageID + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
                }
            }
        }

        function previousPage() {

            var page = document.getElementById("<%= DDLDrop.ClientID %>");
            if (page.options.length != 0) {
                var setpages = document.getElementById("<%= txtPages.ClientID %>");
                var SelectedVal = page.options[page.selectedIndex].value;
                if (parseInt(SelectedVal, 10) != 1) {

                    page.selectedIndex = page.selectedIndex - 1;
                    var UploadID = getParameterByName("id");
                    var PageID = $("#<%= DDLDrop.ClientID %>").val();
                    if (PageID == null) {
                        PageID = SelectedVal;
                        page.selectedIndex = 0;
                    }

                    var params = UploadID + '|' + PageID;
                    document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;
                    var str = setpages.value;
                    var n = str.search(SelectedVal);
                    //alert(n);
                    if (page.selectedIndex != "-1") {
                        if (n != "-1") {
                            var strR = ',' + SelectedVal;
                            setpages.value = str.replace(strR, '');
                        }
                    }


                    var ModiPageCount = document.getElementById("<%=HidModifyPageCount.ClientID %>").value
                    var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');


                    ContentLoaderDiv.innerHTML = "";
                    ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="710" width="800" style="margin-top:0px" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + PageID + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";

                }

            }
        }
        
        


        function loadpdf() {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
                loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
                pageIdContorlID = "<%= hdnPageId.ClientID %>";
                var msgControl = "#<%= divMsg.ClientID %>";
                var Totalpagecount = document.getElementById("<%=Hidtotalpagecount.ClientID %>").value;
                var PageNo = document.getElementById("<%=hdnPageNo.ClientID %>").value;
                if (PageNo != 0 && PageNo != undefined && PageNo != '') {
                }
                else {
                    PageNo = 1;
                }
                var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                ContentLoaderDiv.innerHTML = "";
                ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: absolute;"><iframe id="frame1"  style="margin-top:0px;width:800px;height:800px" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + PageNo + ".pdf#scrollbar=0&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
                //Removed #scrollbar=0&toolbar=0&statusbar=0&messages=0&navpanes=0 - This will disable the controls on adobe reader
            var UploadID = getParameterByName("id");

            
            var params = UploadID + '|' + PageNo;
            var DDLDrop = document.getElementById("<%= DDLDrop.ClientID %>");
            if (DDLDrop != undefined && PageNo != 0 && PageNo != undefined && PageNo != '') {

                if (DDLDrop.options.length != 0) {
                    //                    document.getElementById('totalpagescount').innerHTML = DDLDrop.options[DDLDrop.options.length - 1].value + ' Pages';
                    document.getElementById('totalpagescount').innerHTML = Totalpagecount + ' Pages';

                }
                else {
                    var DDLTagpagecount = document.getElementById("<%= DDLTagpagecount.ClientID %>");
                    document.getElementById('totalpagescount').innerHTML = Totalpagecount + ' Pages';
                    //                    document.getElementById('totalpagescount').innerHTML = '0 Pages';
                }
                DDLDrop.selectedIndex = PageNo - 1;
                params = UploadID + '|' + PageNo;
                document.getElementById("<%=hdnPageNo.ClientID %>").value = PageNo;

            }
            else {
                params = UploadID + '|' + 1;
                if (DDLDrop != undefined) {
                    document.getElementById('totalpagescount').innerHTML = Totalpagecount + ' Pages';
                    document.getElementById("<%=hdnPageNo.ClientID %>").value = 1;
                }

            }
        }

        function gotoTagPage() {
            debugger;
            //var page = $("#<%= DDLTagpagecount.ClientID %>").val();
            //
            //var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
            //ContentLoaderDiv.innerHTML = "";
            //ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="710" width="800" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + page + ".pdf#scrollbar=1&toolbar=1&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
            //var msgControl = "#<%= divMsg.ClientID %>";
            //var UploadID = getParameterByName("id");
            //var PageID = $("#<%= DDLTagpagecount.ClientID %>").val();
            //var params = UploadID + '|' + PageID;
            //document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;
            //            return false;


            var page = $("#<%= DDLTagpagecount.ClientID %>").val();
            if (page != null) {

                var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                ContentLoaderDiv.innerHTML = "";
                ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="820" width="800" style="margin-top:0px" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + page + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
                var msgControl = "#<%= divMsg.ClientID %>";
                var UploadID = getParameterByName("id");
                var PageID = $("#<%= DDLTagpagecount.ClientID %>").val();
                var params = UploadID + '|' + PageID;
                document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;
                var Totalpagecount = document.getElementById("<%=Hidtotalpagecount.ClientID %>").value;
                document.getElementById('totalpagescount').innerHTML = Totalpagecount + ' Pages';
                return false;
            }
            else {
                page = 1;
                var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                ContentLoaderDiv.innerHTML = "";
                ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="820" width="800" style="margin-top:0px" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + page + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
                var msgControl = "#<%= divMsg.ClientID %>";
                var UploadID = getParameterByName("id");
                var PageID = $("#<%= DDLTagpagecount.ClientID %>").val();
                var params = UploadID + '|' + PageID;
                document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;
                var Totalpagecount = document.getElementById("<%=Hidtotalpagecount.ClientID %>").value;
                document.getElementById('totalpagescount').innerHTML = Totalpagecount + ' Pages';
                return false;
            }
        }

        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }



        function TagnextPage() {
            var page = document.getElementById("<%= DDLTagpagecount.ClientID %>");
            if (page.options.length != 0) {
                var LastVal = page.options[page.options.length - 1].value;

                var page2 = $("#<%= DDLTagpagecount.ClientID %>").val();

                if (page2 == null) {
                    var PageID = $("#<%= DDLTagpagecount.ClientID %>").val();
                    var SelectedVal = PageID + 1;
                }
                else {
                    var SelectedVal = page.options[page.selectedIndex].value;
                }
                if (parseInt(SelectedVal, 10) != parseInt(LastVal, 10)) {
                    page.selectedIndex = page.selectedIndex + 1;
                    var UploadID = getParameterByName("id");
                    var PageID = $("#<%= DDLTagpagecount.ClientID %>").val();
                    var params = UploadID + '|' + PageID;
                    document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;
                    var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                    ContentLoaderDiv.innerHTML = "";
                    ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="820" width="800" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + PageID + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";

                }
            }
        }



        function TagpreviousPage() {

            var page = document.getElementById("<%= DDLTagpagecount.ClientID %>");
            if (page.options.length != 0) {
                var SelectedVal = page.options[page.selectedIndex].value;
                if (parseInt(SelectedVal, 10) != 1) {

                    page.selectedIndex = page.selectedIndex - 1;
                    var UploadID = getParameterByName("id");
                    var PageID = $("#<%= DDLTagpagecount.ClientID %>").val();
                    if (PageID == null) {
                        PageID = SelectedVal;
                    }

                    var params = UploadID + '|' + PageID;
                    document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;

                    var ModiPageCount = document.getElementById("<%=HidModifyPageCount.ClientID %>").value
                    var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');


                    ContentLoaderDiv.innerHTML = "";
                    ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="820" width="800" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + PageID + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";

                }

            }
        }


        function TaglastPage() {

            var page = document.getElementById("<%= DDLTagpagecount.ClientID %>");
            if (page.options.length != 0) {
                page.selectedIndex = page.options.length - 1;
                var UploadID = getParameterByName("id");
                var PageID = $("#<%= DDLTagpagecount.ClientID %>").val();
                var params = UploadID + '|' + PageID;
                document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;

                var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                ContentLoaderDiv.innerHTML = "";
                ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="820" width="800" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + PageID + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
            }
        }


        function TagfirstPage() {

            var page = document.getElementById("<%= DDLTagpagecount.ClientID %>");
            if (page.options.length != 0) {
                page.selectedIndex = 0;
                var UploadID = getParameterByName("id");
                var PageID = $("#<%= DDLTagpagecount.ClientID %>").val();
                var params = UploadID + '|' + PageID;
                document.getElementById("<%=hdnPageNo.ClientID %>").value = PageID;

                var ContentLoaderDiv = document.getElementById('ContentLoaderDiv');
                ContentLoaderDiv.innerHTML = "";
                ContentLoaderDiv.innerHTML = '<div id="pdfdiv" style="position: relative;"><iframe id="frame1"  height="820" width="800" src="' + document.getElementById("<%= hdnPDFPathForObject.ClientID  %>").value + "\\" + PageID + ".pdf#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1" + '"' + " /></iframe></div>";
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Label ID="lblCurrentPath" runat="server" CssClass="CurrentPath" Text="Home  &gt;  Document Tag"></asp:Label>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="GVDiv">

                <div class="PageContentsDiv">
                    <div class="DivInlineBlock">
                        <table>
                            <tr align="left">
                                <td colspan="3">
                                    <table cellpadding="0" cellspacing="0" border="0" width="90%">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPageHeader" runat="server" CssClass="PageHeadings" Text="Document Tagging" Font-Bold="True"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center" style="vertical-align: top">
                                    <table cellpadding="3" cellspacing="0" border="0" width="400px" style="text-align: left; vertical-align: top">

                                        <tr>
                                            <td>
                                                <asp:Label ID="lblProjectType" runat="server" CssClass="LabelStyle" Text="Project Type"></asp:Label>
                                                <span style="color: Red">*</span>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                            <td>
                                                <asp:Label ID="lblDocType" runat="server" CssClass="LabelStyle"></asp:Label>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblDepartment" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                                                <span style="color: Red">*</span>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                            <td>
                                                <asp:Label ID="lblDept" runat="server" CssClass="LabelStyle"></asp:Label>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label15" runat="server" CssClass="LabelStyle" Text="Main Doc Type"></asp:Label>
                                                <span style="color: Red">*</span>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbMainTag" runat="server" CssClass="ComboStyle"
                                                    Width="162px" AutoPostBack="True" OnSelectedIndexChanged="cmbMainTag_SelectedIndexChanged">
                                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="reqMainTag" runat="server" ValidationGroup="DOCSUBMIT"
                                                    ControlToValidate="cmbMainTag" ErrorMessage="Main Doc Type is mandatory." Display="None"
                                                    SetFocusOnError="True" InitialValue="0" Width="20px"></asp:RequiredFieldValidator>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label16" runat="server" CssClass="LabelStyle" Text="Sub Doc Type"></asp:Label>
                                                <span style="color: Red">*</span>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="cmbSubTag" runat="server" CssClass="ComboStyle"
                                                    Width="162px" AutoPostBack="true" OnSelectedIndexChanged="cmbSubTag_SelectedIndexChanged">
                                                    <asp:ListItem Value="0">&lt;Select&gt;</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="reqSubTag" runat="server" ValidationGroup="DOCSUBMIT"
                                                    ControlToValidate="cmbSubTag" ErrorMessage="Sub Doc Type is mandatory." Display="None"
                                                    SetFocusOnError="True" InitialValue="0" Width="20px"></asp:RequiredFieldValidator>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <hr style="border: thin solid gray; color: gray" width="90%" align="left" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label20" runat="server" CssClass="LabelStyle" Text="Total Pages"></asp:Label>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                            <td>
                                                <span id="totalpagescount">Pages</span>
                                                <asp:Label ID="lbltotalnoofPages" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <hr style="border: thin dotted gray; color: gray" width="50%" align="center" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label21" runat="server" CssClass="LabelStyle" Text="Untagged Pages"></asp:Label>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="DDLDrop" runat="server" AutoPostBack="false" Style="width: 100px;"
                                                    CssClass="ComboStyle" TagName="Read">
                                                </asp:DropDownList><br />
                                                <asp:Label ID="lbluntagpage" runat="server" Text=""></asp:Label>
                                                <div>
                                                    <input onclick="firstPage();" name="First" type="button" class="btnfirst" />
                                                    <input onclick="previousPage();" name="Previous" type="button" class="btnleftarrow" />
                                                    <input onclick="nextPage();" name="Next" type="button" class="btnrightarrow" />
                                                    <input onclick="lastPage();" name="Last" type="button" class="btnlast" />
                                                </div>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <hr style="border: thin dotted gray; color: gray" width="50%" align="center" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label19" runat="server" CssClass="LabelStyle" Text="Tagged Pages"></asp:Label>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="DDLTagpagecount" runat="server" AutoPostBack="True" Style="width: 100px;"
                                                    CssClass="ComboStyle" TagName="Read" OnSelectedIndexChanged="DDLTagpagecount_SelectedIndexChanged">
                                                </asp:DropDownList><br />
                                                <asp:Label ID="lbltotaltagepage" runat="server" Text=""></asp:Label>
                                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always" RenderMode="Inline">
                                                    <ContentTemplate>
                                                        <asp:Panel ID="Panel2" runat="server">
                                                            <input onclick="TagfirstPage();" name="First" type="button" class="btnfirst" />
                                                            <input onclick="TagpreviousPage();" name="Previous" type="button" class="btnleftarrow" />
                                                            <input onclick="TagnextPage();" name="Next" type="button" class="btnrightarrow" />
                                                            <input onclick="TaglastPage();" name="Last" type="button" class="btnlast" />
                                                        </asp:Panel>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <hr style="border: thin solid gray; color: gray" width="90%" align="left" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="Enter page range or selective pages"
                                                    Width="120px"></asp:Label>
                                                <span style="color: Red">*</span>
                                                <br />
                                                <span>(e.g. 1-5,15,16,19)</span>
                                            </td>
                                            <td colspan="2">
                                                <asp:TextBox ID="txtPages" runat="server" CssClass="TextBoxStyle" Width="200px"
                                                    TextMode="MultiLine"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="reqPages" runat="server" ControlToValidate="txtPages"
                                                    Display="None" ErrorMessage="Page No is mandatory." InitialValue="" SetFocusOnError="True"
                                                    ValidationGroup="DOCSUBMIT" Width="20px"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="REPages" runat="server" ControlToValidate="txtPages"
                                                    Display="None" ErrorMessage="Only 0-9,- Special charactor allowed..!" SetFocusOnError="true"
                                                    ValidationExpression="^[0-9,-]+$" ValidationGroup="DOCSUBMIT" Width="20px" />
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <hr style="border: thin solid gray; color: gray" width="90%" align="left" />
                                                <div id="divMsg" runat="server" style="color: Red" align="center">
                                                    <asp:Label ID="lblmsg" runat="server" Text=""></asp:Label>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4" align="center">
                                                <asp:Button ID="btnClearTag" runat="server" CssClass="btnsearch" Text="Clear Tag" 
                                                    ValidationGroup="DOCSUBMIT" OnClick="btnClearTag_Click" />
                                                &nbsp;&nbsp;<asp:Button ID="btnSubmit" runat="server" CssClass="btnsearch" Text="Submit" OnClick="btnSubmit_Click"
                                                    ValidationGroup="DOCSUBMIT" />
                                                &nbsp;<asp:Button ID="btnCancel" CssClass="btnsearchagain" runat="server" Text="Search Again"
                                                    OnClick="btnCancel_Click" TagName="Read" />
                                                <asp:Button ID="btnpostback" runat="server" Text="Postback" OnClick="btnpostback_Click"
                                                    Style="display: none" />
                                            </td>
                                        </tr>
                                        <tr style="display:none">
                                            <td colspan="4" align="center">
                                                <asp:Button ID="btnNextDoc" runat="server" OnClick="btnNextDoc_Click" ToolTip="Next Document" TagName="Next Document" Text="Next Document" CssClass="nxtprev" />
                                                <asp:Button ID="btnPrevDoc" runat="server" TagName="Previous Document" Text="Previous Document" OnClick="btnPrevDoc_Click" CssClass="nxtprev" />
                                                <p>
                                                    <asp:Label ID="lblRecords" runat="server" Font-Bold="True" Font-Size="Medium" CssClass="LabelStyle"></asp:Label>
                                                </p>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <hr style="border: thin solid gray; color: gray" width="90%" align="left" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <asp:Panel ID="pnlIndexpro" runat="server">
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                            <td></td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="5" align="center">&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: top;">
                                    <div class="SearchFilterDiv">
                                        <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnPageNo" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnCountControls" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnFileName" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnFileLocation" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnFileType" runat="server" Value="" />
                                        <asp:HiddenField ID="HFProjectId" runat="server" Value="0" />
                                        <asp:HiddenField ID="HFUserId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnPDFPathForObject" runat="server" Value="" />
                                        <asp:HiddenField ID="hdnEncrpytFileName" runat="server" Value="" />
                                        <asp:HiddenField ID="HidModifyPageCount" runat="server" Value="" />
                                        <asp:HiddenField ID="Hidmaintag" runat="server" Value="" />
                                        <asp:HiddenField ID="Hidsubtag" runat="server" Value="" />
                                        <asp:HiddenField ID="Hidtotalpagecount" runat="server" Value="" />
                                        &nbsp;
                                    <br />
                                    </div>
                                </td>
                                <td style="vertical-align: top;" class="style1">&nbsp;
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </div>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="DOCSUBMIT"
                        ShowMessageBox="True" />
                    <div class="DivInlineBlockPage">
                        <div id="ContentLoaderDiv">
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
