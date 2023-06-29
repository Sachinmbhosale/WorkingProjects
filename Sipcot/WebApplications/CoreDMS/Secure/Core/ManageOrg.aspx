<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="ManageOrg.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.ManageOrg" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        .style1
        {
            width: 731px;
        }
        .style2
        {
            width: 500px;
            height: 463px;
            vertical-align: top;
        }
        .style3
        {
            width: 625px;
            height: 463px;
            vertical-align: top;
        }
    </style>
            <script type="text/javascript">

                function preventBack() { window.history.forward(); }
                setTimeout("preventBack()", 0);
                window.onunload = function () { null };
            </script>
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";
            btnSubmitControlID = "<%= btnSubmit.ClientID %>";
            hdnCredentialsControlID = "<%= hdnCredentials.ClientID %>";
            hdnOrglinkControlID = "<%= hdnOrglink.ClientID %>";
            pageRightsContorlId = "<%= hdnPageRights.ClientID %>"
        });
        function Submit() {
            var msgControl = "#<%= divMsg.ClientID %>";
            var action = $("#<%= hdnAction.ClientID %>").val();
            var currentOrgId = $("#<%= hdnCurrentOrgId.ClientID %>").val();
            var customerName = $.trim($("#<%= txtCustomerName.ClientID %>").val());
            var address = $.trim($("#<%= txtAddress.ClientID %>").val());
            address = address.replace(/[\n\r]/g, '');
            var orgEmail = $.trim($("#<%= txtOrgEmail.ClientID %>").val());
            var phoneNo = $("#<%= txtPhoneNo.ClientID %>").val();
            var faxNo = $("#<%= txtFaxNo.ClientID %>").val();
            var contactPerson = $.trim($("#<%= txtContactPerson.ClientID %>").val());
            var contactMobile = $("#<%= txtContactMobile.ClientID %>").val();
            var LogoFileName = $.trim($("#<%= hdnLogoFileName.ClientID %>").val());
            var orgGreeting = $.trim($("#<%= txtOrgGreeting.ClientID %>").val());
            var orgDetails = $.trim($("#<%= txtOrgDetails.ClientID %>").val());

            //DMS5-3929BS to get the checkbox list value
            var checkedApplicationIds = "";
            var selectedItems = $("[id*=cblApplication] input");
            var itemarr = document.getElementById("<%=cblApplication.ClientID %>").getElementsByTagName("span");
            if (selectedItems.length > 0) {
                for (var i = 0; i < selectedItems.length; i++)
                    if (selectedItems[i].checked)
                        checkedApplicationIds+=itemarr[i].getAttribute("dvalue")+',';
            }
            //DMS5-3929BE
            //Added a new parameter along with params for application selected
            var params = customerName + '|' + address + '|' + orgEmail
                + '|' + phoneNo + '|' + faxNo + '|' + contactPerson + '|' + contactMobile + '|' + currentOrgId + '|' + LogoFileName + '|' + orgGreeting + '|' + orgDetails + '|' + checkedApplicationIds;
            if (ValidateInputData(msgControl, action, customerName, address, orgEmail, phoneNo, faxNo, contactPerson, contactMobile, checkedApplicationIds)) {
                //                if (parseInt(LogoFileName.length, 10) > 0) {
                //                    (document.getElementById('<%=btnMoveLogo.ClientID%>')).click();
                //                }

                document.getElementById("<%= btnUpdate.ClientID %>").disabled = true;
                return CallPostScalar(msgControl, action, params);
            }
            else {
                return false;
            }
        }
        //********************************************************
        //enable sumit button
        //********************************************************
        function enable() {
            document.getElementById("<%= btnUpdate.ClientID %>").disabled = false;

        }
        //********************************************************
        //ValidateInputData Function returns true or false with message to user on contorl specified
        //********************************************************
        // DMS5-3952 --  Condition newly added: else if(checkedApplicationIds.length < 2) 
        function ValidateInputData(msgControl, action, customerName, address, orgEmail, phoneNo, faxNo, contactPerson, contactMobile, checkedApplicationIds) {
            $(msgControl).html("");
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            var retval;

            if (customerName.length < 2) {
                $(msgControl).html("Customer name should contain atleast two characters!");
                return false;
            }
            else if (reg.test(orgEmail) == false) {
                $(msgControl).html("Please verify Email address!");
                return false;
            }
            else if (phoneNo.length < 10) {
                $(msgControl).html("Please verify Phone number (Landline number with STD Code)!");
                return false;
            }
            else if (checkedApplicationIds.length < 2) {
                $(msgControl).html("Please select an application!");
                return false;
            }
            if (faxNo.length == 0 || faxNo.length > 9) {
                retval = true;
            }
            else {
                $(msgControl).html("Please verify Fax number!");
                return false;
            }
            if (contactPerson.length == 0 || contactPerson.length > 2) {  //Writer UTC Bug 01 Removed
                retval = true;
            }
            else {
                $(msgControl).html("Contact Person name should contain atleast two characters!");
                return false;
            }
            if (contactMobile.length == 0 || contactMobile.length > 9) {
                retval = true;
            }
            else {
                $(msgControl).html("Please verify Contact person Mobile number!");
                return false;
            }

            return retval;
        }

        //********************************************************
        //Disable save button till the file is uploaded
        //********************************************************
        function uploadStart(sender, args) {
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1);
            if (filext == 'jpg' || filext == 'bmp' || filext == 'jpeg' || filext == 'png' || filext == 'gif') {
                document.getElementById("<%= btnUpdate.ClientID %>").disabled = true;
                return true;
            }
            else {
                var err = new Error();
                err.name = 'My API Input Error';
                err.message = 'File format not supported! (Supported format jpg,jpeg,bmp,gif)';
                throw (err);
                return false;
            }
        }

        //********************************************************
        //Enable save button after file is uploaded
        //********************************************************
        function enablesave(sender, args) {
            document.getElementById("<%= btnUpdate.ClientID %>").disabled = false;
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1);
            filename = document.getElementById("<%= txtCustomerName.ClientID %>").value;
            document.getElementById("<%= hdnLogoFileName.ClientID %>").value = filename + "." + filext;

            var authority = getRootWebSitePath();

            var timestamp = new Date().getTime();
            document.getElementById("<%= imgCustLogo.ClientID %>").src = authority + "/GenericHandler.ashx?f=" + '<%=ConfigurationManager.AppSettings["TempWorkFolder"].ToString().Replace(@"\", @"\\") %>' + "Logo\\" + '<%=Session["LoggedUserId"]%>' + "\\" + filename + "." + filext + "#" + timestamp;
            return true;

        }

        function getRootWebSitePath() {
            var _location = document.location.toString();
            var applicationNameIndex = _location.indexOf('/', _location.indexOf('://') + 3);
            var applicationName = _location.substring(0, applicationNameIndex) + '/';
            var webFolderIndex = _location.indexOf('/', _location.indexOf(applicationName) + applicationName.length);
            var webFolderFullPath = _location.substring(0, webFolderIndex);
            //If develoment environement WEB folder won't be there
            webFolderFullPath = webFolderFullPath.replace('/Accounts', '');
            webFolderFullPath = webFolderFullPath.replace('/secure', '');
            webFolderFullPath = webFolderFullPath.replace('/Workflow', '');
            return webFolderFullPath;
        }


        //********************************************************
        //ClearData Function clears the form
        //********************************************************

        function ClearData() {
            document.getElementById("<%= txtCustomerName.ClientID %>").value = "";
            document.getElementById("<%= txtAddress.ClientID  %>").value = "";
            document.getElementById("<%= txtOrgEmail.ClientID  %>").value = "";
            document.getElementById("<%= txtPhoneNo.ClientID  %>").value = "";
            document.getElementById("<%= txtFaxNo.ClientID  %>").value = "";
            document.getElementById("<%= txtContactPerson.ClientID  %>").value = "";
            document.getElementById("<%= txtContactMobile.ClientID  %>").value = "";
            document.getElementById("<%= btnUpdate.ClientID %>").disabled = false;
            document.getElementById("ctl00_ContentPlaceHolder2_ContentPlaceHolder1_imgCustLogo").src = '../../Assets/Logo/Company_logo.jpg';
            document.getElementById("<%= txtOrgGreeting.ClientID %>").value = "";
            document.getElementById("<%= txtOrgDetails.ClientID %>").value = "";
        }

      
    </script>
    <style type="text/css">
        .Secondtd
        {
            padding-left: 30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label CssClass="CurrentPath" ID="lblPagePath" runat="server" Text="Home  &gt;  Customer"></asp:Label>
    <div class="GVDiv">
        <asp:Label CssClass="PageHeadings" ID="lblHeading" runat="server" Text="Add New Customer"></asp:Label>
        <div id="divMsg" runat="server" style="color: Red">
            &nbsp;</div>
        <fieldset>
            <table class="style1" border="0">
                <tr>
                    <td>
                        <div>
                            <div>
                                <asp:Label ID="lblHeading0" runat="server" CssClass="LabelStyle" Text="Customer" /><span
                                    class="AstrickStyle">&nbsp;*</span></div>
                            <asp:TextBox ID="txtCustomerName" runat="server" TextMode="SingleLine" MaxLength="100"
                                Text="" OnKeyPress="return CheckvarcharKeyInfo(event);" TabIndex="1"></asp:TextBox>
                        </div>
                    </td>
                    <td class="Secondtd">
                        <div>
                            <div>
                                <asp:Label ID="lblOrgGreeting" runat="server" CssClass="LabelStyle" Text="Organization Greeting" /></div>
                            <asp:TextBox ID="txtOrgGreeting" runat="server" TextMode="SingleLine" MaxLength="100"
                                TabIndex="2"></asp:TextBox>
                        </div>
                    </td>
                    <td>
                        <div>
                            <div>
                                <asp:Label ID="lblOrgDetails" runat="server" CssClass="LabelStyle" Text="Organization Details" /></div>
                            <asp:TextBox ID="txtOrgDetails" runat="server" TextMode="SingleLine" MaxLength="100"
                                TabIndex="3"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div>
                            <div>
                                <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="Address" /></div>
                            <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" MaxLength="1000"
                                TabIndex="4"></asp:TextBox>
                        </div>
                    </td>
                    <td class="Secondtd">
                        <div>
                            <div>
                                <asp:Label ID="Label4" runat="server" CssClass="LabelStyle" Text="Email" /><span
                                    class="AstrickStyle">&nbsp;*</span>
                            </div>
                            <asp:TextBox ID="txtOrgEmail" runat="server" TextMode="SingleLine" MaxLength="100"
                                Text="" TabIndex="5"></asp:TextBox>
                        </div>
                    </td>
                    <td>
                        <div>
                            <div>
                                <asp:Label ID="Label5" runat="server" CssClass="LabelStyle" Text="Phone"></asp:Label><span
                                    class="AstrickStyle">&nbsp;*</span>
                            </div>
                            <asp:TextBox ID="txtPhoneNo" runat="server" TextMode="SingleLine" MaxLength="14"
                                OnKeyPress="return CheckNumericKeyInfo(event)" Text="" TabIndex="6"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div>
                            <div>
                                <asp:Label ID="Label6" runat="server" CssClass="LabelStyle" Text="Fax" /></div>
                            <asp:TextBox ID="txtFaxNo" runat="server" TextMode="SingleLine" MaxLength="12" Text=""
                                OnKeyPress="return CheckNumericKeyInfo(event)" TabIndex="7"></asp:TextBox>
                        </div>
                    </td>
                    <td class="Secondtd">
                        <div>
                            <div>
                                <asp:Label ID="Label7" runat="server" CssClass="LabelStyle" Text="Contact Person" /></div>
                            <asp:TextBox ID="txtContactPerson" runat="server" TextMode="SingleLine" MaxLength="30"
                                Text="" OnKeyPress="return CheckvarcharKeyInfo(event)" TabIndex="8"></asp:TextBox>
                        </div>
                    </td>
                    <td>
                        <div>
                            <div>
                                <asp:Label ID="Label8" runat="server" CssClass="LabelStyle" Text="Mobile:" /></div>
                            <asp:TextBox ID="txtContactMobile" runat="server" TextMode="SingleLine" MaxLength="14"
                                Text="" OnKeyPress="return CheckNumericKeyInfo(event)" TabIndex="9"></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel ID="LogoUpdatePanel" RenderMode="Block" UpdateMode="Always" runat="server">
                            <ContentTemplate>
                                <div>
                                    <asp:Label ID="lblLogo" runat="server" CssClass="LabelStyle" Text="Logo:" />
                                    <asp:Label ID="lblChangeLogo" runat="server" CssClass="LabelStyle" Text="Change Logo:" />
                                </div>
                                <asp:AsyncFileUpload ID="AsyncFULogo" runat="server" CssClass="LabelStyle" Width="203px"
                                    CompleteBackColor="Lime" ErrorBackColor="Red" ThrobberID="Throbber" OnClientUploadStarted="uploadStart"
                                    OnClientUploadComplete="enablesave" OnUploadedComplete="AsyncFULogo_UploadedComplete"
                                    UploadingBackColor="#66CCFF" TabIndex="10" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <br />
                    </td>
                    <td>
                        <img runat="server" id="imgCustLogo" src="~/Assets/Logo/Company_logo.jpg" alt="Customer Logo"
                            height="86" width="255" />
                        <asp:Label ID="lblError" runat="server" Text="(Note: Image Size Should be less than 1 MB,Width should be 255 px and Height should be 86px)"></asp:Label>
                    </td>
                    <td>
                        <div>
                            <div>
                                <asp:Label ID="lblApplicationlist" runat="server" CssClass="LabelStyle" Text="Applications:" />
                            </div>
                            <asp:CheckBoxList ID="cblApplication" runat="server">
                            </asp:CheckBoxList>
                        </div>
                    </td>
                    <td>
                    </td>
                    <td colspan="2">
                        <div style="width: 100%">
                            &nbsp;<asp:HiddenField ID="hdnUserName" runat="server" Value="" />
                            <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                            <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                            <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                            <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                            <asp:HiddenField ID="hdnCurrentOrgId" runat="server" Value="" />
                            <asp:HiddenField ID="hdnLogoFileName" runat="server" Value="" />
                            <asp:HiddenField ID="hdnCredentials" runat="server" Value="" />
                            <asp:HiddenField ID="hdnOrglink" runat="server" Value="" />
                            <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                            <asp:Button ID="btnSubmit" class="HiddenButton" TagName="Read" runat="server" Text="SentMail"
                                OnClick="btnSubmit_Click" />
                            <asp:Button ID="btnMoveLogo" class="HiddenButton" TagName="Read" runat="server" Text="MoveLogo"
                                OnClick="btnMoveLogo_Click" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td colspan="2">
                        <asp:Button ID="btnsearchagain" runat="server" Text="Search Again" CssClass="btnsearchagain"
                            TagName="Read" OnClick="btnsearchagain_Click" />
                        <asp:Button ID="btnUpdate" runat="server" Text="Submit" CssClass="btnsave" OnClientClick="Submit();return false;"
                            TagName="Edit" TabIndex="11" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btnclear" OnClientClick="ClearData(); return false;"
                            TagName="Read" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
</asp:Content>
