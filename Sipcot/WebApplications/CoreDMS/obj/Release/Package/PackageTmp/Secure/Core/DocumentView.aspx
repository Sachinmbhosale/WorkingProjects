<%@ Page Language="C#" MasterPageFile="~/DocumentMaster.Master" AutoEventWireup="true"
    CodeBehind="DocumentView.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Secure.Core.DocumentView" %>

<%@ MasterType VirtualPath="~/DocumentMaster.Master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Head" runat="server">
            <script type="text/javascript">

                function preventBack() { window.history.forward(); }
                setTimeout("preventBack()", 0);
                window.onunload = function () { null };
            </script>
    <script language="javascript" type="text/javascript">
        function Redirect(ProcessID, DocId, DepID, Active, PageNo, MainTagId, SubTagId, SlicingStatus, Watermark) {
            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            var info = getAcrobatInfo();
            if (true) {
                if (SlicingStatus == 'Uploaded') {
                    window.location.href = "DocumentDownloadDetails.aspx?id=" + ProcessID + '&docid=' + DocId + '&depid=' + DepID + '&active=' + Active + '&PageNo=' + PageNo + '&MainTagId=' + MainTagId + '&SubTagId=' + SubTagId + '&Search=' + document.getElementById("<%=hdnSearchString.ClientID %>").value + '&Page=DocumentDownload' + '&Watermark=' + Watermark;
                }
                else {

                    $(msgControl).html("Kindly Wait For Few Minutes Document Activity is Pending!");
                    document.getElementById('<%=divMsg.ClientID%>').style.color = 'Red';
                    return false;
                }
            }
            else {
                var MsgAdobeDownload = '<%= ConfigurationManager.AppSettings["MsgAdobeDownload"].ToString() %>';
                var LinkAdobeDownload = '<%= ConfigurationManager.AppSettings["LinkAdobeDownload"].ToString() %>';
                var IsConfirm = window.confirm(MsgAdobeDownload);
                if (IsConfirm == true) {
                    window.open(LinkAdobeDownload);
                }
                return false;
            }
        }

        var getAcrobatInfo = function () {

            var getBrowserName = function () {
                return this.name = this.name || function () {
                    var userAgent = navigator ? navigator.userAgent.toLowerCase() : "other";

                    if (userAgent.indexOf("chrome") > -1) return "chrome";
                    else if (userAgent.indexOf("safari") > -1) return "safari";
                    else if (userAgent.indexOf("msie") > -1) return "ie";
                    else if (userAgent.indexOf("firefox") > -1) return "firefox";
                    return userAgent;
                } ();
            };

            var getActiveXObject = function (name) {
                try { return new ActiveXObject(name); } catch (e) { }
            };

            var getNavigatorPlugin = function (name) {
                for (key in navigator.plugins) {
                    var plugin = navigator.plugins[key];
                    if (plugin.name == name) return plugin;
                }
            };

            var getPDFPlugin = function () {
                return this.plugin = this.plugin || function () {
                    if (getBrowserName() == 'ie') {
                        //
                        // load the activeX control
                        // AcroPDF.PDF is used by version 7 and later
                        // PDF.PdfCtrl is used by version 6 and earlier
                        return getActiveXObject('AcroPDF.PDF') || getActiveXObject('PDF.PdfCtrl');
                    }
                    else {
                        return getNavigatorPlugin('Adobe Acrobat') || getNavigatorPlugin('Chrome PDF Viewer') || getNavigatorPlugin('WebKit built-in PDF');
                    }
                } ();
            };

            var isAcrobatInstalled = function () {
                return !!getPDFPlugin();
            };

            var getAcrobatVersion = function () {
                try {
                    var plugin = getPDFPlugin();

                    if (getBrowserName() == 'ie') {
                        var versions = plugin.GetVersions().split(',');
                        var latest = versions[0].split('=');
                        return parseFloat(latest[1]);
                    }

                    if (plugin.version) return parseInt(plugin.version);
                    return plugin.name

                }
                catch (e) {
                    return null;
                }
            }

            //
            // The returned object
            // 
            return {
                browser: getBrowserName(),
                acrobat: isAcrobatInstalled() ? 'installed' : false,
                acrobatVersion: getAcrobatVersion()
            };
        };
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td>
                <div id="divMsg" runat="server">
                </div>
                <center>
                    <div style="width: 1100px; height: 500px; overflow: auto;">
                        <asp:GridView ID="gvDocument" runat="server" EmptyDataText="No Workflows Are Available"  PageSize="10"
                            CssClass="mGrid" PagerStyle-CssClass="pgr" GridLines="None" OnRowDataBound="gvDocument_RowDataBound"
                            CellPadding="10" CellSpacing="5" AllowPaging="True" OnPageIndexChanging="gvDocument_PageIndexChanging"
                            AlternatingRowStyle-CssClass="alt">
                            <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                            <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                                PageButtonCount="5" PreviousPageText=" " />
                            <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                        </asp:GridView>
                    </div>
                </center>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hdnLoginOrgId" runat="server" />
    <asp:HiddenField ID="hdnLoginToken" runat="server" />
    <asp:HiddenField ID="hdnSearchString" runat="server" />
</asp:Content>
