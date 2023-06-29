<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PDFViewer.ascx.cs" Inherits="PdfViewer.PDFViewer" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<link href="<%= Page.ResolveClientUrl("~/AnnotaionCSS/PDFViewer.css") %>" rel="stylesheet"
    type="text/css" />
<link href="<%= Page.ResolveClientUrl("~/Annotations/AnnotationToolbar.touchDevices.css") %>"
    rel="stylesheet" type="text/css" />
<link href="<%= Page.ResolveClientUrl("~/Annotations/jquery.mobile-1.0.1.min.css") %>"
    rel="stylesheet" type="text/css" />
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Annotations/Leadtools.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Annotations/Leadtools.Controls.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Annotations/Leadtools.Annotations.Core.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Annotations/Leadtools.Annotations.Rendering.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Annotations/Leadtools.Annotations.designers.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Annotations/Leadtools.Annotations.Automation.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Annotations/PropertyGrid.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Annotations/AnnotationsDemo.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Annotations/canvas2image.js") %>"></script>
<script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Annotations/node.js") %>"></script>
<meta name="GENERATOR" content="MSHTML 9.00.8112.16457" />
<asp:Panel ID="MainPanel" runat="server" Height="100%" Width="810px" Wrap="False">
    <asp:Panel ID="ToolStripPanel" CssClass="toolbar" runat="server">
        <asp:Table ID="ImageControls" runat="server">
            <asp:TableRow ID="MainTableRow1" runat="server">
                <asp:TableCell ID="ImageControls_Part1" runat="server">
                    <asp:Table ID="ToolStripTable" runat="server" CellPadding="5" Height="40px">
                        <asp:TableRow ID="TableRow1" VerticalAlign="Middle" HorizontalAlign="Left">
                            <asp:TableCell ID="TableCell1" runat="server" VerticalAlign="Middle" HorizontalAlign="Center"> 
                     <a id="PreviousPage" href="#" title="PreviousPage" onclick="navigationHandler('PREVIOUS'); return false;"><img src="../../AnnotaionImages/backward.png" alt="Delete" /></a>
                   
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell23" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                     <a id="NextPage"  href="#" title="NextPage" onclick="navigationHandler('NEXT'); return false;"><img src="../../AnnotaionImages/Forward.png" alt="Delete" /></a>
                            </asp:TableCell>
                            <asp:TableCell ID="zoomViewerSelect" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                           <select id="zoomSelect"  class="controlsSelect" onchange="zoomViewer(); return false;"><option>Actual Size</option><option>Fit Page</option><option>Fit Width</option><option>100%</option><option>10%</option><option>25%</option><option>50%</option><option>75%</option><option>100%</option><option>125%</option><option>200%</option><option>400%</option><option>800%</option><option>1600%</option><option>2400%</option><option>3200%</option><option>6400%</option></select>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell7" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                                 <a id="ZoomInButton1" href="#" title="Zoom Out" onclick="clickEffect(this);zoomIn(); return false;">
                                <img src="../../AnnotaionImages/Zoom In_24x24.png" alt="highlight" /></a>
                            </asp:TableCell>
                            <asp:TableCell href="#" ID="TableCell6" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                         <a id="ZoomOutButton" title="Zoom Out" onclick="clickEffect(this);zoomOut(); return false;">
                                <img src="../../AnnotaionImages/Zoom Out_24x24.png" alt="highlight" /></a>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:TableCell>
                <asp:TableCell ID="ImageControls_Part2" runat="server">
                    <asp:Table ID="ToolStripTable2" runat="server">
                        <asp:TableRow ID="AnnotationTollbar" VerticalAlign="Middle" HorizontalAlign="Left">
                            <asp:TableCell ID="TableCell8" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                               <a id="Undo" href="#" title="Undo" onclick="clickEffect(this);annUndo(); return false;"><img src="../../AnnotaionImages/Undo_24x24.png" alt="Undo" /></a>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell9" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                        <a id="Redo"  href="#" title="Redo" onclick="clickEffect(this);annRedo(); return false;"><img src="../../AnnotaionImages/Redo_24x24.png" alt="Redo" /></a>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell22" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">                                      
                    <a id="Select" href="#" title="Select" onclick="clickEffect(this);onObjectClicked(-1); return false;">
                        <img src="../../AnnotaionImages/cursor_drag_arrow.png" alt="Select" /></a> 
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell17" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                        <a id="Note" href="#" title="Note" onclick="clickEffect(this);onObjectClicked(-15); return false;" >
                                <img src="../../AnnotaionImages/sticky-note.png" alt="Note"/></a>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell18" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                         <a id="Line" href="#" title="Line" onclick="clickEffect(this);onObjectClicked(-2); return false;">
                                <img src="../../AnnotaionImages/pencil.png" alt="Line" /></a>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell19" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
 <a id="Rectangle" href="#"  title="Rectangle" onclick="clickEffect(this);onObjectClicked(-3); return false;">
                                <img src="../../AnnotaionImages/rectangle.png" alt="Rectangle" /></a>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell20" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                                                    <a id="Highlight" href="#"  title="Highlight" onclick="clickEffect(this);onObjectClicked(-11); return false;">
                                <img src="../../AnnotaionImages/highlight.png" alt="highlight" /></a>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell4" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">               
                        <a id="Freehand" href="#"  title="Freehand" onclick="clickEffect(this);onObjectClicked(-10); return false;">
                             <img src="../../AnnotaionImages/Actions-draw-freehand-icon.png" alt="Freehand" /></a>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell5" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                              <a id="Text" href="#"  title="Text" onclick="clickEffect(this);onObjectClicked(-12); return false;">
                             <img src="../../AnnotaionImages/Text.png" alt="Text" /></a>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell21" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                     <a id="Delete"  href="#" title="Delete" onclick="clickEffect(this);annDelete(); return false;"><img src="../../AnnotaionImages/eraser.png" alt="Delete" /></a>
                            </asp:TableCell>
                            <asp:TableCell ID="TableCell3" runat="server" VerticalAlign="Middle" HorizontalAlign="Center">
                  <a title="Save Image" href="#" id="SaveImage" onclick="clickEffect(this);SaveAnnotation(); return false;">
                   <img src="../../AnnotaionImages/Save.png" alt="Save Image"/></a> 
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <asp:Panel ID="PicPanel" runat="server" Height="100%" Width="100%">
        <asp:Table ID="Table1" runat="server" GridLines="Both" Height="100%" Style="margin-right: 0px"
            Width="100%">
            <asp:TableRow ID="trImageViewer" runat="server" VerticalAlign="Top" HorizontalAlign="Left">
                <asp:TableCell ID="PicCell" runat="server" CssClass="page-viewer-placeholder">
                
                            <div style="margin: 0px auto; position: relative;" id="Div1" data-role="page">
                                <div style="padding: 0px;" id="Content" data-role="content">
                                    <div style="border-width: 0px; margin: 0px auto; width: 100%; height: 100%; float: left;
                                        position: relative;" id="imageViewerContainer">
                                    </div>
                                   <%-- <div id="Watermark" class="watermark">No Preview</div>--%>
                                </div>
                            </div>
                       
             
                    <!-- ## DIALOGS ## -->
                    <section style="margin: 0px auto;" id="propertiesPage" data-role="dialog" data-rel="back"
                        data-transition="flip" data-theme="d">
    <header data-role="header">
<H1>          </H1></header>
    <div id="pg2" data-role="content">
    </div>
    <footer style="margin: 0px auto; text-align: center;" class="ui-grid-a" data-role="footer"><a id="btnDone" 
data-role="button"></a></footer>
   </section>
                    <div id="overlay">
                    </div>
                    <div>
                        <div id="imageControlsDiv" class="controlsDiv">
                            <div class="controlsLeftDiv">
                                <label class="controlsLeftLabel">
                                    Image:</label></div>
                            <div class="controlsRightDiv">
                                <select id="ImagesIds" onchange="onImageChanged()" name="Image"></select>
                                 </div>
                            <div class="controlsOKDiv">
                                <input id="imageControlsOkButton" class="button" value="Close" type="button" /></div>
                        </div>
                    </div>
                    <div id="imageLoadOverlay">
                    </div>
                    <div>
                        <div id="imageLoadDiv">
                            <p id="loadingText">
                                Loading new image</p>
                            <img style="width: 100px; height: 75px;" alt="Loading" src="../../Annotations/loading.gif" /></div>
                    </div>
                    <section style="margin: 0px auto;" id="mediaPlayerPage" data-role="dialog" data-transition="flip"
                        data-theme="d"><header data-role="header">
<H1></H1></header>
    <div id="videoDiv" data-role="content">
    </div>
    <footer data-role="footer"><a id="btnClose" 
data-role="button"></a></footer>
  </section>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <table border="1" width="100%">
        <tr>
            <td style="width: 90%">
                <div style="margin: 0px auto; position: relative;" id="page1" data-role="page">
                    <div id="toolRibbon1" class="toolBarDiv">
                        <div style="width: 0px; height: 0px;" id="fileInput">
                            <input style="visibility: collapse;" id="inputFileBrowser" accept="text/xml" type="file" /></div>
                        <div id="optionsToolBar" class="ToolBar">
                        </div>
                    </div>
                    <div id="toolRibbon2" class="toolBarDiv">
                        <div id="annObjectsToolBar" class="ToolBar">
                        </div>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:HiddenField ID="HiddenPageNumber" runat="server" />
<asp:HiddenField ID="HiddenBrowserWidth" runat="server" />
<asp:HiddenField ID="HiddenBrowserHeight" runat="server" />
<asp:Button ID="HiddenPageNav" runat="server" Height="0px" Width="0px" BorderStyle="None"
    BackColor="Transparent" />
<script type="text/javascript">
    function changePage(pageNum) {
        getBrowserDimensions();
        document.getElementById("<%=HiddenPageNumber.ClientID%>").value = pageNum;
        document.getElementById("<%=HiddenPageNav.ClientID%>").click();
    }

    function getElement(aID) {
        return (document.getElementById) ?
       document.getElementById(aID) : document.all[aID];
    }



    function to_image() {
        var canvas = document.getElementById("ImageViewer_annotationCanvas");
        document.getElementById("ImageViewer_annotationCanvas").src = canvas.toDataURL();
        Canvas2Image.saveAsPNG(canvas);
    }


    function SaveAnnotation() {
        //Get xml :  only annotations
        var anxml = annSave();

        //Apply annotations on image
        annBurn();

        //Get base64string :  Image and annotations
        var DocWithAnn = onActionClicked('saveImage');

        SaveDocumentAnnotation(anxml, DocWithAnn);
    }

    function getBrowserDimensions() {

        if (typeof (window.innerWidth) == 'number') {
            //Non-IE
            browserWidth = window.innerWidth;
            browserHeight = window.innerHeight;
        } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
            //IE 6+ in 'standards compliant mode'
            browserWidth = document.documentElement.clientWidth;
            browserHeight = document.documentElement.clientHeight;
        } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
            //IE 4 compatible
            browserWidth = document.body.clientWidth;
            browserHeight = document.body.clientHeight;
        }

        //        $('#imageViewerContainer').width(browserWidth);
        //        $('#imageViewerContainer').height(browserHeight);

        document.getElementById("<%=HiddenBrowserWidth.ClientID%>").value = browserWidth;
        document.getElementById("<%=HiddenBrowserHeight.ClientID%>").value = browserHeight;
    }

</script>
<script type="text/javascript">

    $(document).ready(function () {
        //  SetImage("images/1.jpg");
        //do jQuery stuff when DOM is ready
        getBrowserDimensions();
    });
</script>
<body />
