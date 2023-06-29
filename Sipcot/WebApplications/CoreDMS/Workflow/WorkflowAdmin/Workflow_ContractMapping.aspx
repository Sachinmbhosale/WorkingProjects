<%@ Page Title="" Language="C#" MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master" AutoEventWireup="true" CodeBehind="Workflow_ContractMapping.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.Workflow_ContractMapping" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Src="~/Workflow/WorkflowAdmin/WorkflowPDFViewer.ascx" TagPrefix="uc1" TagName="WorkflowPDFViewer" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

        .gridviewData {
            text-align: right;
        }
    </style>
    <script src="../../Scripts/jquery-1.10.2.js"></script>
    <script type="text/javascript">

        function CloseWindow(obj) {
            window.close();
            return false;
        }

        function closeWin() {

            window.close();
        }

        function GetProductPath(source, eventArgs) {

            var FilePath = eventArgs.get_value();
            alert(FilePath);

            $("#<%=hdnSrc.ClientID%>").val(FilePath);
            }


            function uploadStart(sender, args) {

                var result;

                var filename = args.get_fileName();
                var filext = filename.substring(filename.lastIndexOf(".") + 1).toLowerCase();

                if (doctype.value != '0' && dept.value != '0') {
                    if (filext == 'pdf'
                    ) {
                        return true;
                    }
                    else {
                        var err = new Error();
                        err.name = 'My API Input Error';
                        err.message = 'Please select supported Files!';


                        ///document.getElementById('<%=FileUpload1.ClientID %>').innerText = "";

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
                    err.message = 'Please select both Project Type and Department!';;
                    throw (err);
                    return false;
                }
            }

            function Download(url) {
                alert(url);
                document.getElementById('myiframe').src = url;
            };

            function PreviewDoc(vUrl) {
                window.open(vUrl);

                return false;
            }


            function openTab(src) {
                alert(1)
                var src = string.Empty;
                src = GetSrc("Handler") + filePath + "#toolbar=0";

                alert(src)
                $("#myiframe").attr("src", src);

                //window.open(src, '_blank');

            }


            function CalculateTotalAmt(vObj) {

                var mId = $(vObj).attr("id").split("_Txtquantity")[0];

                if ($(vObj).closest("tr").find(".lblRate").text() != undefined && $(vObj).val() != undefined) {

                    var mVal = parseFloat($(vObj).closest("tr").find(".lblRate").text()) * parseFloat($(vObj).val());

                    $(vObj).closest("tr").find(".lblTotal").text(mVal.toFixed(2));

                    $(vObj).closest("tr").find("#" + mId + "_hiddTotal").val(mVal.toFixed(2));
                }
            }



            function PopupShown(sender, args) {
                sender._popupBehavior._element.style.zIndex = 99999999;
            }
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="GVDiv">

        <table style="width: 100%">
            <colgroup>
                <col width="52%" />
                <col width="*" />
            </colgroup>
            <tr valign="top">
                <td>
                    <table align="right" style="border: 2px solid #fdb813; border-radius: 8px; padding: 5px; width: 100%;">
                        <tr>
                            <td>
                                <div style="height: 30px">
                                    <b>Select Type Of Quotation</b>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div style="height: 40px">

                                    <asp:RadioButtonList ID="CTypeOfQuotation1" runat="server" RepeatDirection="Horizontal" Width="450px">
                                        <asp:ListItem Selected="True">Tender</asp:ListItem>
                                        <asp:ListItem>Budgetary Quotation</asp:ListItem>
                                    </asp:RadioButtonList>

                                </div>
                            </td>

                        </tr>
                        <tr>
                            <td>
                                <div style="height: 100px;">
                                    <label id="lblTender">Upload File</label>
                                    <asp:FileUpload ID="FileUpload1" runat="server" />


                                    <%--  <asp:Button ID="btnPreview" runat="server" Text="Preview" CssClass="btn btn-secondary" OnClick="btnPreview_Click" />--%>
                                    <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btn btn-secondary" OnClick="btnUpload_Click" />
                                    <asp:Button ID="Button1" TagName="Add" runat="server" Text="Add Product" OnClick="GetSelectedRecordsPopup1" CssClass="btn btn-secondary" />

                                    <div class="input-group-append">

                                        <%-- <asp:Label ID="lblmsg" runat="server"></asp:Label>--%>
                                        <asp:HiddenField ID="hfFileName" runat="server" Value="" />
                                        <asp:HiddenField ID="hfFileExtension" runat="server" Value="" />
                                        <asp:HiddenField ID="hfFilePath" runat="server" Value="" />
                                        <asp:HiddenField ID="hidFilePath" runat="server" Value="" />
                                        <asp:HiddenField ID="hfFileSize" runat="server" Value="" />
                                    </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblmsg" runat="server"></asp:Label></td>
                        </tr>


                        <tr>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                                    <ContentTemplate>
                                        <asp:Button ID="btnShowPopup" runat="server" Style="display: none" />

                                        <ajax:ModalPopupExtender ID="ModalPopup" runat="server" TargetControlID="btnShowPopup"
                                            PopupControlID="pnlpopup" CancelControlID="imgClosePopUp" OkControlID="btnNo" BackgroundCssClass="modalBackground">
                                        </ajax:ModalPopupExtender>

                                        <center>
                                            <asp:Panel ID="pnlpopup" Style="width: 80%; height: 720px; padding-top: 25%" runat="server">


                                                <div class="model-table">
                                                    <div class="GVDiv">
                                                        <asp:ImageButton ID="imgClosePopUp" runat="server" Style="float: right"
                                                            ImageUrl="~/Images/close.png" />
                                                        <table>
                                                            <tr id="serachbtn" runat="server">
                                                                <td>
                                                                    <div style="height: 40px">
                                                                        <label id="lblsearch">Search :</label>
                                                                        <asp:TextBox ID="txtContactsSearch" runat="server" Font-Bold="true" Style="width: 258px" AutoPostBack="true" autocomplete="off" onSelect="GetProductPath" OnTextChanged="txtContactsSearch_TextChanged"></asp:TextBox>
                                                                        <ajax:AutoCompleteExtender ID="AutoCompleteExtender1" ServiceMethod="GetProductRefDtls" MinimumPrefixLength="2" CompletionListCssClass="completionList" CompletionInterval="100"
                                                                            EnableCaching="false" CompletionSetCount="10" TargetControlID="txtContactsSearch" runat="server" OnClientShown="PopupShown">
                                                                        </ajax:AutoCompleteExtender>

                                                                        <asp:Button ID="btnclear" runat="server" Text="Clear" OnClick="btnclear_Click" />
                                                                        <%-- <asp:Button ID="Button1" TagName="Add" runat="server" Text="Add" OnClick="GetSelectedRecords1" CssClass="btnsave" />--%>


                                                                        <%-- <asp:Button ID="btnReset" runat="server" Text="Reset" OnClick="btnReset_Click" />--%>
                                                                        <asp:HiddenField ID="hiddTenderRefId" runat="server" />

                                                                        <asp:HiddenField ID="hdnSrc" runat="server" Value="" />

                                                                    </div>
                                                                </td>
                                                            </tr>

                                                            <tr id="tblgrid" runat="server">
                                                                <td>
                                                                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" AllowPaging="true"
                                                                        OnPageIndexChanging="OnPageIndexChanging1" PageSize="5" Style1="padding-left: 10px" CssClass="mGrid" Width="100%">
                                                                        <Columns>
                                                                            <asp:BoundField ItemStyle-Width="30px" DataField="Id" HeaderText="ID" ItemStyle-CssClass="gridviewData" />
                                                                            <asp:BoundField ItemStyle-Width="200px" DataField="Brand_Name" HeaderText="Product Name" />
                                                                            <asp:BoundField ItemStyle-Width="100px" DataField="Business_Unit" HeaderText="Product Category" />
                                                                            <asp:BoundField ItemStyle-Width="120px" DataField="NEW_MATERIAL_CODE" HeaderText="New Product Code" ItemStyle-CssClass="gridviewData" />
                                                                            <asp:BoundField ItemStyle-Width="120px" DataField="RATE_EXCL_GST" HeaderText="Rate Exclusive(GST)" ItemStyle-CssClass="gridviewData" />
                                                                            <asp:BoundField ItemStyle-Width="120px" DataField="GST_Amount" HeaderText="GST Amount" ItemStyle-CssClass="gridviewData" />
                                                                            <asp:BoundField ItemStyle-Width="120px" DataField="RATE_INCLUSIVE_GST" HeaderText="Rate Inclusive(GST)" ItemStyle-CssClass="gridviewData" />
                                                                            <asp:TemplateField HeaderText="Action" ItemStyle-Width="50px">
                                                                                <ItemTemplate>
                                                                                    <asp:CheckBox ID="chkRow" runat="server" CssClass='<%#Bind("Id") %>' />
                                                                                    <asp:HiddenField ID="hdnFldSelectedValues" runat="server" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </asp:GridView>

                                                                    <asp:HiddenField ID="HiddenField1" runat="server" Value="" />
                                                                    <asp:HiddenField ID="HiddenField2" runat="server" Value="" />


                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="center">
                                                                    <asp:Button ID="btnNo" Style="display: none" runat="server" Text="OK" Width="100" />
                                                                    <asp:Button ID="btnadd" TagName="Add" runat="server" Text="Add" OnClick="GetSelectedRecords" CssClass="btnsave" />
                                                                    <%--<asp:Button ID="Button2" TagName="Add" runat="server" Text="Add" OnClick="GetSelectedRecords" CssClass="btnsave" />--%>

                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                        </center>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>

                    </table>

                    <table id="tblproduct" align="left" style="border: 2px solid #fdb813; border-radius: 8px; padding: 5px; width: 98% !important; display: none" runat="server">

                        <tr id="grdproduct" runat="server">
                            <td>
                                <asp:GridView ID="grvProduct" display="none" Visible="false" runat="server" AutoGenerateColumns="false" AllowPaging="true"
                                    OnPageIndexChanging="OnPageIndexChanging" PageSize="5" Style1="padding-left: 10px" CssClass="mGrid" Width="100%">
                                    <Columns>
                                        <asp:BoundField ItemStyle-Width="30px" DataField="Id" HeaderText="ID" ItemStyle-CssClass="gridviewData" />
                                        <asp:BoundField ItemStyle-Width="200px" DataField="Brand_Name" HeaderText="Product Name" />
                                        <asp:BoundField ItemStyle-Width="100px" DataField="Business_Unit" HeaderText="Product Category" />
                                        <asp:BoundField ItemStyle-Width="100px" DataField="NEW_MATERIAL_CODE" HeaderText="New Product Code" ItemStyle-CssClass="gridviewData" />
                                        <asp:BoundField ItemStyle-Width="120px" DataField="RATE_EXCL_GST" HeaderText="Rate Exclusive(GST)" ItemStyle-CssClass="gridviewData" />
                                        <asp:BoundField ItemStyle-Width="120px" DataField="GST_Amount" HeaderText="GST Amount" ItemStyle-CssClass="gridviewData" />
                                        <asp:BoundField ItemStyle-Width="100px" DataField="RATE_INCLUSIVE_GST" HeaderText="Rate Inclusive(GST)" ItemStyle-CssClass="gridviewData" />
                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="50px">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkRow" runat="server" CssClass='<%#Bind("Id")%>' />
                                                <asp:HiddenField ID="hdnFldSelectedValues" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:HiddenField ID="hdnPID" runat="server" Value="" />
                                <asp:HiddenField ID="hdnProductName" runat="server" Value="" />
                            </td>
                        </tr>
                        <tr id="addbtn" runat="server" style="padding-left: 92%; display: none">
                            <td style="padding-left: 90%">
                                <%--   <asp:Button ID="btnadd" TagName="Add" runat="server" Text="Add" OnClick="GetSelectedRecords" CssClass="btnsave" />--%>
                            </td>
                            <%--<td></td>--%>
                        </tr>
                        <tr id="grdAddPro" runat="server" style="display: none; overflow-y: scroll; height: 250px;">
                            <td><b><u>Selected Rows</u></b>
                                <asp:UpdatePanel ID="UpdatePanel41" runat="server" UpdateMode="Always" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:GridView ID="GrdAddProduct" runat="server" AutoGenerateColumns="false" AllowPaging="true" CssClass="mGrid"
                                            OnPageIndexChanging="OnPageIndexChanging2" PageSize="50" Style="padding-left: 10px">
                                            <Columns>
                                                <asp:BoundField ItemStyle-Width="30px" DataField="Id" HeaderText="ID" ItemStyle-CssClass="gridviewData" />
                                                <asp:BoundField ItemStyle-Width="100px" DataField="Brand_Name" HeaderText="Product Name" />
                                                <asp:BoundField ItemStyle-Width="80px" DataField="Business_Unit" HeaderText="Product Category" />
                                                <asp:BoundField ItemStyle-Width="100px" DataField="NEW_MATERIAL_CODE" HeaderText="New Product Code" ItemStyle-CssClass="gridviewData" />
                                                
                                                  <asp:BoundField ItemStyle-Width="80px" DataField="RATE_EXCL_GST" HeaderText="Rate Exclusive(GST) " />
                                                  <asp:BoundField ItemStyle-Width="80px" DataField="GST_Amount" HeaderText="GST Amount" />

                                                <asp:BoundField ItemStyle-Width="100px" DataField="RATE_INCLUSIVE_GST" HeaderText="Rate Inclusive(GST)" Visible="false" />
                                                <asp:TemplateField HeaderText="Rate Inclusive(GST)" ItemStyle-Width="100px" ItemStyle-CssClass="gridviewData">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRate" runat="server" Text='<%# Bind("RATE_INCLUSIVE_GST") %>' CssClass="lblRate" ItemStyle-CssClass="gridviewData"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Quantity" ItemStyle-Width="60px">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Txtquantity" runat="server" onchange="CalculateTotalAmt(this)" Width="60px" Style="text-align: right" ItemStyle-CssClass="gridviewData"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total" ItemStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <%--<asp:UpdatePanel runat="server">
                                                            <ContentTemplate>--%>
                                                        <asp:Label ID="lblTotal" runat="server" CssClass="lblTotal" Width="50px" ItemStyle-CssClass="gridviewData"></asp:Label>
                                                        <asp:HiddenField ID="hiddTotal" runat="server"></asp:HiddenField>
                                                        <%-- </ContentTemplate>
                                                        </asp:UpdatePanel>--%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>

                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:HiddenField ID="hdnprdCategory" runat="server" />
                                <asp:HiddenField ID="hdnPCode" runat="server" />
                                <asp:HiddenField ID="hdnlbltotal" runat="server" />
                                <asp:HiddenField ID="hdnqty" runat="server" />
                                <asp:HiddenField ID="hdnrate" runat="server" />
                                <asp:HiddenField ID="hdnrate1" runat="server" />
                                <asp:HiddenField ID="hdnDocId" runat="server" />
                            </td>
                            <%--<td></td>--%>
                        </tr>

                        <tr>
                            <td id="btn1" runat="server" style="display: none">
                                <div>
                                    <%--<asp:Button ID="btnSave" TagName="Add" runat="server" Text="Submit" OnClick="btnSave_Clcik"
                                        CssClass="btnsave" />--%>

                                    <center>
                                        <asp:Button ID="btnSave" TagName="Add" runat="server" Text="Submit" OnClick="btnSave_Clcik"
                                            CssClass="btnsave" />



                                        <asp:Button ID="btnClose" TagName="Read" runat="server" Text="Close" OnClientClick="closeWin()"
                                            CssClass="btncancel" />
                                    </center>
                                    <%--  OnClientClick="return CloseWindow(this);" --%>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top">
                    <table style="width: 100%">
                        <%--<colgroup>
                            <col width="50%" />
                            <col width="50%" />
                        </colgroup>--%>
                        <tr>
                            <%-- <td id="download" runat="server" style="display:none">
                                <%-- <asp:Button ID="btnDownload"  runat="server" Text="Download" OnClick="btnDownload_Click"
                                        CssClass="btnsave" />
                            </td>--%>
                            <td>
                                <%--<asp:Button ID="btnDownload"  runat="server" Text="Download" OnClick="btnDownload_Click"
                                        CssClass="btnsave" style="display:none" />--%>
                                <iframe id="myiframe" style="width: 600px; height: 677px;" runat="server"></iframe>
                            </td>
                            <%-- <td id="tdTenderDoc" runat="server" visible="false">
                                    <span>
                                        <h3>Tender Document</h3>
                                    </span>
                                    <iframe id="Iframe1" height="600" width="400" runat="server"></iframe>
                                </td>--%>
                        </tr>
                    </table>
                    <div style="display: none">
                        <uc1:WorkflowPDFViewer ID="WFPDFViewer" runat="server" />
                        <asp:Label ID="lblMessageImg" ForeColor="Red" runat="server"></asp:Label>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

