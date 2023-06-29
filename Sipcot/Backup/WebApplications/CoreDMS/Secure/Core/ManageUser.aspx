<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="ManageUser.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.ManageUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script language="javascript" type="text/javascript">

        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";
            pageRightsContorlId = "<%= hdnPageRights.ClientID %>";
            btnSubmitControlID = "<%= btnSubmit.ClientID %>";
            btnSaveControlID = "<%= btnSave.ClientID %>";
            hdnCredentialsControlID = "<%= hdnCredentials.ClientID %>";
            btnresetpasswordControlID = "<%= btnresetpassword.ClientID %>";
            hdnOrglinkControlID = "<%= hdnOrglink.ClientID %>";
            chkDomainUserControlID = "<%= chkDomainUser.ClientID %>";
            if (document.getElementById("<%= chkDomainUser.ClientID %>").checked == true) {
                document.getElementById("<%= drpDomain.ClientID %>").disabled = false;
            }
            else {
                document.getElementById("<%= drpDomain.ClientID %>").disabled = true;
            }
        });

        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }
        function getSelectValues() {
            var select = document.getElementById("<%= drpDepartment.ClientID %>");
            var result = [];
            var Status;
            var options = select && select.options;
            var opt;

            for (var i = 0, iLen = options.length; i < iLen; i++) {
                opt = options[i];

                if (opt.selected) {
                    result.push(opt.value || opt.text);
                    Status = true;
                }
            }
            if (Status != true) {
                result = null;
            }
            return result;
        }
        function Submit() {

            var msgControl = "#<%= divMsg.ClientID %>";
            var action = $("#<%= hdnAction.ClientID %>").val();
            var userName = $.trim($("#<%= txtUserName.ClientID %>").val());
            var description = $.trim($("#<%= txtDescription.ClientID %>").val());
            var userEmailId = $.trim($("#<%= txtUserEmailId.ClientID %>").val());
            var mobileNo = $.trim($("#<%= txtMobileNo.ClientID %>").val());
            var firstName = $.trim($("#<%= txtFirstName.ClientID %>").val());
            var lastName = $.trim($("#<%= txtLastName.ClientID %>").val());
            var currentUserd = $("#<%= hdnCurrentUserId.ClientID %>").val();
            var userGroupId = $("#<%= drpUserGroup.ClientID %>").val();
            var departmentId = getSelectValues();
            if (departmentId == null && userName != "administrator") {
                $(msgControl).html("Please select department!");
                return false;
            }
            //            if (departmentId != null && departmentId.toString().split(",").length > 1) {
            //                $(msgControl).html("Please select only one department!");
            //                return false;
            //            }

            var Domain = $("#<%= drpDomain.ClientID %>").val();

            if (typeof Domain == 'undefined' || Domain == null) {
                Domain = 0;
            }
            var isActive = false;
            var isDomainUser = false;
            if ($("#<%= chkActive.ClientID %>").is(':checked')) isActive = true;
            if ($("#<%= chkDomainUser.ClientID %>").is(':checked')) isDomainUser = true;
            var params = userName + '|' + description + '|' + userEmailId
                + '|' + mobileNo + '|' + firstName + '|' + lastName
                + '|' + userGroupId + '|' + departmentId + '|' + isActive + '|' + currentUserd + '|' + isDomainUser + '|' + Domain;
            if (ValidateInputData(msgControl, action, userName, description, userEmailId, mobileNo, firstName, lastName, isDomainUser, Domain)) {
                document.getElementById("<%= btnSave.ClientID %>").disabled = true;

                return CallPostScalar(msgControl, action, params);
            }
            else {
                return false;
            }
        }
        //********************************************************
        //Enable sumit button
        //********************************************************
        function enable() {
            document.getElementById("<%= btnSave.ClientID %>").disabled = false;

        }
        //********************************************************
        //ValidateInputData Function returns true or false with message to user on contorl specified
        //********************************************************

        function ValidateInputData(msgControl, action, userName, description, userEmailId, mobileNo, firstName, lastName, isDomainUser, Domain) {
            $(msgControl).html("");
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;

            if (userName.length < 2) {
                $(msgControl).html("User Name Should Contain Atleast Two Characters!");
                return false;
            }
            else if (firstName.length < 2) {
                $(msgControl).html("First Name Should Contain Atleast Two Characters!");
                return false;
            }
            else if (userName.toUpperCase() != 'ADMINISTRATOR' && lastName.length < 2) {
                $(msgControl).html("Last Name Should Contain Atleast Two Characters!");
                return false;
            }
            else if (reg.test(userEmailId) == false) {
                $(msgControl).html("Please Verify that Correct Email Address is Entered!");
                return false;
            }
            else if (mobileNo.length < 10) {
                $(msgControl).html("Please Verify that Correct Mobile No is Entered!");
                return false;
            }
            else if (document.getElementById("<%= drpUserGroup.ClientID  %>").value == "0") {
                $(msgControl).html("Please Select a User Role!");
                return false;
            }
            else if (document.getElementById("<%= drpDepartment.ClientID  %>").value == "0") {
                $(msgControl).html("Please Select User a Department!");
                return false;
            }
            else if (isDomainUser == true && Domain == "0") {
                $(msgControl).html("Please Select an Active Directory!");
                return false;
            }
            else {
                return true;
            }
        }
        //********************************************************
        //ClearData Function clears the form
        //********************************************************

        function ClearData() {
            document.getElementById("<%= txtUserName.ClientID %>").value = "";
            document.getElementById("<%= txtDescription.ClientID  %>").value = "";
            document.getElementById("<%= txtUserEmailId.ClientID  %>").value = "";
            document.getElementById("<%= txtMobileNo.ClientID  %>").value = "";
            document.getElementById("<%= txtFirstName.ClientID  %>").value = "";
            document.getElementById("<%= txtLastName.ClientID  %>").value = "";
            document.getElementById("<%= drpUserGroup.ClientID  %>").value = "";
            document.getElementById("<%= drpDepartment.ClientID  %>").value = "";
            document.getElementById("<%= btnSave.ClientID %>").disabled = false;

        }
        //********************************************************
        //ClearData Function navigate to Homepage
        //********************************************************

        function Close() {
            var type = getParameterByName("action");
            if (type == "edittempuser") {
                location.href = "BulkUploadUsers.aspx";
            }
            else {
                location.href = "SearchUser.aspx";
            }
        }        
         
    </script>
    <script type="text/javascript" language="javascript">

        //        $(document).ready(function () {

        //            divMenuConfig.className = "SubMenuBase";
        //            divMenuDocument.className = "MenuHide";


        //            $("#mnuUsers").slideDown("fast");
        //            MenuUser.className = "SubMenuLinkSelected";
        //            lnkUsers.className = "MenuLInkStyleSelected";
        //            lnkUserAdd.className = "MenuLInkStyleSelected";
        //            mnuUsers.className = "SubMenuBaseSelected";
        //            mmConfig.className = "MenudivSelected";
        //            mUsersAddBase.className = "SubMenuItemLinkSelected";
        //        });

        //texbox validation - Only Characters and '-' Symbol
        function CheckvarcharKeyInfo(event) {
            var char1 = (event.keyCode ? event.keyCode : event.which);
            if ((char1 >= 65 && char1 <= 90) || (char1 >= 97 && char1 <= 122) || char1 == 45 || char1 == 32 || char1 == 8 || char1 == 9) {
                RetVal = true;
            }
            else {
                RetVal = false;
            }
            return RetVal;
        }

        //texbox validation - Only numbers and '-' Symbol
        function CheckNumericKeyInfo(event) {
            var char1 = (event.keyCode ? event.keyCode : event.which);
            if ((char1 >= 48 && char1 <= 57) || char1 == 45 || char1 == 43 || char1 == 8 || char1 == 9) {
                RetVal = true;
            }
            else {
                RetVal = false;
            }
            return RetVal;
        }

        //********************************************************
        // DUEN - Add
        //********************************************************
        function checkdomain() {
            if (document.getElementById("<%= chkDomainUser.ClientID%>").checked == true) {
                document.getElementById("<%= drpDomain.ClientID%>").disabled = false;
            }
            else {
                document.getElementById("<%= drpDomain.ClientID%>").selectedIndex = 0;
                document.getElementById("<%= drpDomain.ClientID%>").disabled = true;
            }
        }

        function ToggleDepartmentSelection() {
            var selectObject = document.getElementById("<%= drpDepartment.ClientID %>");
            var isSelected = selectObject.options[0].selected;
            var checkObj = document.getElementById("<%= chkSelectAllDepartment.ClientID %>");
            checkObj.checked = (!isSelected);
            ToggleSelection(selectObject, isSelected);
        }

        function ToggleSelection(selectObject, isSelected) {
            for (var i = 0, j = selectObject.options.length; i < j; i++) {
                selectObject.options[i].selected = (!isSelected);
            }
        }
            
    </script>
    <style type="text/css">
        .Thirdtd
        {
            padding-left: 20px;
        }
        .fourthtd
        {
            padding-left: 10px;
            padding-top: 6px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label CssClass="CurrentPath" ID="lblPagePath" runat="server" Text="Home  &gt;  Configure User"></asp:Label>
    <div class="GVDiv">
        <asp:Label CssClass="PageHeadings" ID="lblHeading" runat="server" Text="Edit User"></asp:Label>
        <div id="divMsg" runat="server" style="color: Red">
            &nbsp;</div>
        <fieldset>
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td>
                        <nobr><asp:Label ID="lblUserName" runat="server" CssClass="LabelStyle" Text="User Name"></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="lblPageDescription1" runat="server"
                            Text="*"></asp:Label></nobr>
                        &nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="txtUserName" runat="server" Style="margin-left: 0px" MaxLength="200"></asp:TextBox>
                    </td>
                    <td class="Thirdtd">
                        <asp:Label ID="lblHead7" runat="server" CssClass="LabelStyle" Text="Description"></asp:Label>
                    </td>
                    <td class="fourthtd">
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblHead" runat="server" CssClass="LabelStyle" Text="First Name"></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label2" runat="server" Text="*"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtFirstName" runat="server" MaxLength="30" OnKeyPress="return CheckvarcharKeyInfo(event)"></asp:TextBox>
                    </td>
                    <td class="Thirdtd">
                        <nobr> <asp:Label ID="lblHead1" runat="server" CssClass="LabelStyle" Text="Last Name"></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label3" runat="server" Text="*"></asp:Label></nobr>
                    </td>
                    <td class="fourthtd">
                        <asp:TextBox ID="txtLastName" runat="server" Style="margin-left: 0px" MaxLength="30"
                            OnKeyPress="return CheckvarcharKeyInfo(event)"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblHead2" runat="server" CssClass="LabelStyle" Text="Email"></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label4" runat="server" Text="*"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtUserEmailId" runat="server" Style="margin-left: 0px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="Thirdtd">
                        <asp:Label ID="lblHead3" runat="server" CssClass="LabelStyle" Text="Mobile"></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label5" runat="server" Text="*"></asp:Label>
                    </td>
                    <td class="fourthtd">
                        <asp:TextBox ID="txtMobileNo" runat="server" Style="margin-left: 0px" MaxLength="14"
                            OnKeyPress="return CheckNumericKeyInfo(event)"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblHead6" runat="server" CssClass="LabelStyle" Text="Active"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" Checked="true" runat="server" Text=" " />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblHead4" runat="server" CssClass="LabelStyle" Text="User Role"></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label6" runat="server" Text="*"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="drpUserGroup" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="Thirdtd">
                        <nobr><asp:Label ID="lblHead0" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label7" runat="server" Text="*"></asp:Label></nobr>
                    </td>
                    <td class="fourthtd">
                        <asp:CheckBox ID="chkSelectAllDepartment" runat="server" Text="Select All" CssClass="RadioButtonStlye" />
                        <br />
                        <asp:ListBox ID="drpDepartment" runat="server" SelectionMode="Multiple"></asp:ListBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="lblDomainUser" CssClass="LabelStyle" Text="Active Directory User "></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkDomainUser" runat="server" onclick="return checkdomain();" />
                    </td>
                    <td class="Thirdtd">
                        <asp:Label ID="lblDomainName" runat="server" CssClass="LabelStyle" Text="Active Directory"></asp:Label>
                    </td>
                    <td class="fourthtd">
                        <asp:DropDownList ID="drpDomain" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <table>
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                    </td>
                </tr>
            </table>
            <table>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="btnsearchagain" runat="server" Text="Search Again" class="btnsearchagain"
                            TagName="Read" OnClick="btnsearchagain_Click" />
                        <asp:Button ID="btnSave" runat="server" Text="Submit" CssClass="btnsave" OnClientClick="Submit(); return false;"
                            TagName="Edit" />
                        <asp:Button ID="btnClose" runat="server" Text="Close" CssClass="btnclose" OnClientClick="Close(); return false;"
                            TagName="Read" />
                        <asp:Button ID="btnresetpassword" runat="server" Text="Reset Password" class="btnresetpassword"
                            TagName="Edit" OnClick="btnresetpassword_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btnSubmit" TagName="Read" class="HiddenButton" runat="server" Text="SendEmail"
                            OnClick="btnSubmit_Click" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="LabelDiv">
            &nbsp;<asp:HiddenField ID="hdnUserName" runat="server" Value="" />
            <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
            <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
            <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
            <asp:HiddenField ID="hdnAction" runat="server" Value="" />
            <asp:HiddenField ID="hdnCredentials" runat="server" Value="" />
            <asp:HiddenField ID="hdnCurrentUserId" runat="server" Value="" />
            <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
            <asp:HiddenField ID="hdnOrglink" runat="server" Value="" />
        </div>
    </div>
    <br />
</asp:Content>
