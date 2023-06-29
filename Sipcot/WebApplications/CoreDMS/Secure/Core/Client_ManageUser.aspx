<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="Client_ManageUser.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.Client_ManageUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/hex_md5.js") %>"></script>
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



        $(document).ready(function () {
            $(this).keydown(function (event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                    document.getElementById("btnUpdate").click();
                }
            });

        });

        function EncryptPassword1(str) {


            if (document.getElementById('<%= txtNewpassword.ClientID %>').value != "" && document.getElementById('<%= txtConfirmpassword.ClientID %>').value != "") {


 
                var md5encyptretryn = hex_md5(document.getElementById('<%= txtNewpassword.ClientID %>').value);
                var passstrretypepasswordn = str + (md5encyptretryn);
                document.getElementById('<%= txtNewpassword.ClientID  %>').value = hex_md5(passstrretypepasswordn);


                var md5encyptOldPassword2 = hex_md5(document.getElementById('<%= txtConfirmpassword.ClientID %>').value);
                var passstrretypepasswordn = str + (md5encyptOldPassword2);
                document.getElementById('<%= txtConfirmpassword.ClientID  %>').value = hex_md5(passstrretypepasswordn);

              

                btnUpdateControlID = "<%= btnUpdate.ClientID %>";


            }
        }






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
            var Newpassword = $.trim($("#<%= txtNewpassword.ClientID %>").val());



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
            if (ValidateInputData(msgControl, action, userName, description, userEmailId, mobileNo, firstName, lastName, isDomainUser, Domain, Newpassword)) {
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

        function ValidateInputData(msgControl, action, userName, description, userEmailId, mobileNo, firstName, lastName, isDomainUser, Domain, Newpassword) {
            $(msgControl).html("");
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;

            if (userName.length < 2) {
                $(msgControl).html("User Name Should Contain Atleast Two Characters!");
                return false;
            }
            if (Newpassword.length < 8) {
                $(msgControl).html("Password cannot be empty!");
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
        location.href = "Client_SearchUser.aspx";
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
        .Thirdtd {
            padding-left: 20px;
        }

        .fourthtd {
            padding-left: 10px;
            padding-top: 6px;
        }
          .fifththtd {
            padding-left: 2px;
            padding-top: 6px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label CssClass="CurrentPath" ID="lblPagePath" runat="server" Text="Home  &gt;  Configure User"></asp:Label>
    <div class="GVDiv">
        <asp:Label CssClass="PageHeadings" ID="lblHeading" runat="server" Text="Edit User"></asp:Label>
        <div id="divMsg" runat="server" style="color: Red">
            &nbsp;
        </div>
        <fieldset>
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td>
                        <nobr><asp:Label ID="lblUserName" runat="server" CssClass="LabelStyle" Text="User Name"></asp:Label>
                        </nobr>
                        &nbsp;
                    </td>
                    <td>
                        <asp:TextBox ID="txtUserName" runat="server" Style="margin-left: 0px" MaxLength="200" BackColor="#CCCCCC" ForeColor="#CCCCCC"></asp:TextBox>
                    </td>
                    <td class="Thirdtd" style="display:none">
                        <asp:Label ID="lblHead7" runat="server" CssClass="LabelStyle" Text="Description"></asp:Label>
                    </td>
                    <td class="fourthtd" style="display:none">
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" MaxLength="1000" BackColor="#CCCCCC" ForeColor="#CCCCCC"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblHead" runat="server" CssClass="LabelStyle" Text="First Name"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtFirstName" runat="server" MaxLength="30" OnKeyPress="return CheckvarcharKeyInfo(event)" BackColor="#CCCCCC" ForeColor="#CCCCCC"></asp:TextBox>
                    </td>
                    <td class="Thirdtd">
                        <nobr> <asp:Label ID="lblHead1" runat="server" CssClass="LabelStyle" Text="Last Name"></asp:Label>
                        </nobr>
                    </td>
                    <td class="fourthtd">
                        <asp:TextBox ID="txtLastName" runat="server" Style="margin-left: 0px" MaxLength="30"
                            OnKeyPress="return CheckvarcharKeyInfo(event)" BackColor="#CCCCCC" ForeColor="#CCCCCC"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblHead2" runat="server" CssClass="LabelStyle" Text="Email"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtUserEmailId" runat="server" Style="margin-left: 0px" MaxLength="100"></asp:TextBox>
                    </td>
                    <td class="Thirdtd">
                        <asp:Label ID="lblHead3" runat="server" CssClass="LabelStyle" Text="Mobile"></asp:Label>
                    </td>
                    <td class="fourthtd">
                        <asp:TextBox ID="txtMobileNo" runat="server" Style="margin-left: 0px" MaxLength="14"
                            OnKeyPress="return CheckNumericKeyInfo(event)"></asp:TextBox>
                    </td>
                </tr>

                <tr>
                    <td>
                        <nobr><asp:Label ID="lblHead0" runat="server" CssClass="LabelStyle" Text="Department"></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label7" runat="server" Text="*"></asp:Label></nobr>
                    </td>
                    <td>
                        <asp:ListBox ID="drpDepartment" runat="server" SelectionMode="Multiple"></asp:ListBox>
                    </td>
                    <td class="Thirdtd">&nbsp;</td>
                    <td class="fourthtd">
                        &nbsp;</td>
                </tr>
                <tr >
                    <td >
                        <asp:Label ID="Label1" runat="server" CssClass="LabelStyle" Text="New Password"></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label8" runat="server" Text="*"></asp:Label>
                    </td>
                    <td class="fifththtd" >
                        <asp:TextBox ID="txtNewpassword" runat="server" Style="margin-left: 0px" MaxLength="20" TextMode="Password" ValidationGroup="Password"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="Regex4" runat="server" ControlToValidate="txtNewpassword" ValidationGroup="Password"
                            ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}"
                            ErrorMessage="Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase Alphabet, 1 Number and 1 Special Character" ForeColor="Red" Display="None" SetFocusOnError="True" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter password" ControlToValidate="txtNewpassword" Display="Dynamic" SetFocusOnError="True" ToolTip="Please enter password" ValidationGroup="Password"></asp:RequiredFieldValidator>
                    </td>
                    <td class="Thirdtd">&nbsp;</td>
                    <td class="fourthtd">&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label9" runat="server" CssClass="LabelStyle" Text="Confirm Password"></asp:Label>
                    </td>
                    <td class="fifththtd">
                        <asp:TextBox ID="txtConfirmpassword" runat="server" Style="margin-left: 0px" MaxLength="20" TextMode="Password" ValidationGroup="Password"></asp:TextBox>
                        <asp:CompareValidator ID="CompareValidator" runat="server" ControlToValidate="txtConfirmpassword" ControlToCompare="txtNewpassword" ErrorMessage="Password does not match!" ValidationGroup="Password">
                        </asp:CompareValidator>


                    </td>
                    <td class="Thirdtd" colspan="2">

                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span style="font-size: 9px; color: #FF0000">(Password should contain minimum 8 and maximum 20 characters, at least one special character and one numeric value.)</span> </td>
                    <td class="Thirdtd">
                        &nbsp;</td>
                    <td class="fourthtd">
                        &nbsp;</td>
                </tr>
                <tr>
                    <td >
                        &nbsp;</td>
                    <td></td>
                     <td>
                        <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btnsave" OnClick="btnUpdate_Click" style="height: 26px"  />
                        &nbsp;<asp:Button ID="btnClose" runat="server" Text="Close" CssClass="btnclose" OnClientClick="Close(); return false;"
                            TagName="Read" />
                    </td>
                     <td></td>
                </tr>
                                <tr style="display: none">
                    <td>
                        <asp:Label ID="lblHead6" runat="server" CssClass="LabelStyle" Text="Active"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" Checked="true" runat="server" Text=" " />
                    </td>
                </tr>
                <tr>
                    <td style="display: none">
                        <asp:Label ID="lblHead4" runat="server" CssClass="LabelStyle" Text="User Role"></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="Label6" runat="server" Text="*"></asp:Label>
                    </td>
                    <td style="display: none">
                        <asp:DropDownList ID="drpUserGroup" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="Thirdtd">&nbsp;</td>
                    <td class="fourthtd">
                        <asp:CheckBox ID="chkSelectAllDepartment" runat="server" Text="Select All" CssClass="RadioButtonStlye" Style="display: none" />
                        <br />
                    </td>
                </tr>
                <tr style="display: none">
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
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                </tr>
                <tr>
                    <td colspan="4"></td>
                </tr>
            </table>
            <asp:ValidationSummary ID="ValidationSummary1" ValidationGroup="Password" runat="server" ShowMessageBox="True" />
            <table>
                <tr>
                    <td colspan="4">
                        <asp:Button ID="btnsearchagain" runat="server" Text="Search Again" class="btnsearchagain"
                            TagName="Read" OnClick="btnsearchagain_Click" Style="display: none" />
                        <asp:Button ID="btnSave" runat="server" Text="Change Password" CssClass="btnsave"
                            TagName="Edit" OnClick="btnSave_Click" ValidationGroup="Password" Style="display: none" />
                        <asp:Button ID="btnresetpassword" runat="server" Text="Reset Password" class="btnresetpassword" Style="display: none"
                            TagName="Edit" OnClick="btnresetpassword_Click" />
                    </td>
                    <td>
                        <asp:Button ID="btnSubmit" TagName="Read" class="HiddenButton" runat="server" Text="SendEmail" Style="display: none"
                            OnClick="btnSubmit_Click" ValidationGroup="Password" />
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
