<%@ Page Title="" Language="C#" MasterPageFile="~/SecureMaster.Master" AutoEventWireup="true"
    CodeBehind="ManageGroup.aspx.cs" Inherits="Lotex.EnterpriseSolutions.WebUI.ManageGroup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7-vsdoc.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.min.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.js") %>"></script>
    <script type="text/javascript" language="javascript" src="<%=Page.ResolveClientUrl("~/Assets/Scripts/AjaxPostScripts.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Assets/Scripts/master.js") %>"></script>

            <script type="text/javascript">

                function preventBack() { window.history.forward(); }
                setTimeout("preventBack()", 0);
                window.onunload = function () { null };
            </script>
    <script language="javascript" type="text/javascript">
        var rightsArray = new Array();
        var rightsIdentity = 0;
        $(document).ready(function () {
            loginOrgIdControlID = "<%= hdnLoginOrgId.ClientID %>";
            loginTokenControlID = "<%= hdnLoginToken.ClientID %>";
            pageIdContorlID = "<%= hdnPageId.ClientID %>";
            pageRightsContorlId = "<%= hdnPageRights.ClientID %>"

        });

        function Submit() {
            var msgControl = "#<%= divMsg.ClientID %>";
            var action = $("#<%= hdnAction.ClientID %>").val();
            var currentGroupId = $("#<%= hdnCurrentGroupId.ClientID %>").val();
            var groupName = $.trim($("#<%= txtGroupName.ClientID %>").val());
            var description = $("#<%= txtDescription.ClientID %>").val();

            var params = groupName + '|' + description + '|' + currentGroupId;
            if (ValidateInputData(msgControl, action, groupName, description)) {
                AddPageRights();
                var data = GetRightsText();
                return CallPostTable(msgControl, action, params, data);
            }
            else {
                return false;
            }
        }
        //********************************************************
        //ValidateInputData Function returns true or false with message to user on contorl specified
        //********************************************************

        function ValidateInputData(msgControl, action, groupName, description) {
            $(msgControl).html("");
            if (action == "AddGroup" || action == "EditGroup") {
                $(msgControl).html("");
                var retval;

                if (groupName.length < 2) {
                    $(msgControl).html("Role Name Should Contain Atleast Two Characters!");
                    return false;
                }
                else {
                    if (action == "EditGroup") {
                        var pageSelected = false;
                        var selectPages = document.getElementById("<%= lstPages.ClientID %>");
                        for (var i = 0, j = selectPages.options.length; i < j; i++) {
                            if (selectPages.options[i].selected) {
                                pageSelected = true;
                            }
                        }
                        var rightSelected = false;
                        var selectRights = document.getElementById("<%= lstRights.ClientID %>");
                        for (var i = 0, j = selectRights.options.length; i < j; i++) {
                            if (selectRights.options[i].selected) {
                                rightSelected = true;
                            }
                        }
                        if (pageSelected == true && rightSelected == false) {
                            $(msgControl).html("Please select at least one right for the page selected.");
                            return false;
                        }
                        if (pageSelected == false && rightSelected == true) {
                            $(msgControl).html("Please select at least one page for the right selected.");
                            return false;
                        }
                        if (rightsArray.length == 0) {
                            if (pageSelected == true && rightSelected == true) {
                                return true;
                            }
                            $(msgControl).html("Please select at least one page.");
                            return false;
                        }
                        return true;
                    }
                    var pageSelected = false;
                    var selectPages = document.getElementById("<%= lstPages.ClientID %>");
                    for (var i = 0, j = selectPages.options.length; i < j; i++) {
                        if (selectPages.options[i].selected) {
                            pageSelected = true;
                        }
                    }
                    if (!pageSelected) {
                        $(msgControl).html("Please select at least one page.");
                        return false;
                    }
                    else {
                        var rightSelected = false;
                        var selectRights = document.getElementById("<%= lstRights.ClientID %>");
                        for (var i = 0, j = selectRights.options.length; i < j; i++) {
                            if (selectRights.options[i].selected) {
                                rightSelected = true;
                            }
                        }
                        if (!rightSelected) {
                            $(msgControl).html("Please select at least one right.");
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        //********************************************************
        //ClearData Function clears the form
        //********************************************************

        function ClearData() {
            document.getElementById("<%= txtGroupName.ClientID %>").value = "";
            document.getElementById("<%= txtDescription.ClientID  %>").value = "";
            document.getElementById("<%= lstPages.ClientID %>").selectedIndex = -1;
            document.getElementById("<%= lstRights.ClientID %>").selectedIndex = -1;
            document.getElementById("<%=chkSelectAllPages.ClientID %>").checked = false;
            document.getElementById("<%=chkSelectAllRights.ClientID %>").checked = false;
            $("#divGrid").html("");
            document.getElementById("<%= txtGroupName.ClientID %>").focus();
        }
        //********************************************************
        //ClearData Function navigate to Homepage
        //********************************************************

        function Close() {
            location.href = "SearchGroup.aspx";
        }
        //********************************************************
        //ClearData Function navigate to Homepage
        //********************************************************

        function AddPageRights() {

            var dataArray = rightsArray;
            if (dataArray == null) {
                var dataArray = new Array();
            }
            var selectPages = document.getElementById("<%= lstPages.ClientID %>");
            for (var m = 0, n = selectPages.options.length; m < n; m++) {
                if (selectPages.options[m].selected) {
                    var pageName = selectPages.options[m].value;
                    //check whetether the pagename already exists
                    if (CheckRightsSelected()) {
                        var selectObject = document.getElementById("<%= lstRights.ClientID %>");
                        if (CheckPageRightExists(pageName)) {
                            for (var i = 0, j = dataArray.length; i < j; i++) {
                                var data = dataArray[i];
                                if (data[1] == pageName) {// for the page date
                                    for (var x = 0, y = selectObject.options.length; x < y; x++) {
                                        if (selectObject.options[x].selected) {
                                            for (var k = 0, l = data.length; k < l; k++) {
                                                if (data[k] == selectObject.options[x].value) {
                                                    break;
                                                }
                                                else {
                                                    if (data[k] == "") {
                                                        data[k] = selectObject.options[x].value;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else {
                            rightsIdentity = rightsIdentity + 1;
                            var item = [rightsIdentity, pageName];
                            for (var i = 0, j = selectObject.options.length; i < j; i++) {
                                if (selectObject.options[i].selected) {
                                    item.push(selectObject.options[i].value);
                                }
                            }
                            if (item.length < (selectObject.options.length + 2)) {
                                for (var i = item.length, j = (selectObject.options.length + 2); i < j; i++) {
                                    item.push("");
                                }
                            }
                            dataArray.push(item);
                        }
                    }
                }
            }
            rightsArray = dataArray;
            ShowRightsTable();

        }
        function CheckPageRightExists(pageName) {
            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            var dataArray = rightsArray;
            var isExists = false;
            if (dataArray == null) {
                var dataArray = new Array();
            }
            if (dataArray.length > 0) {

                for (var i = 0, j = dataArray.length; i < j; i++) {
                    var data = dataArray[i];
                    if (data[1] == pageName) {
                        isExists = true;
                    }
                }
            }
            //            if (isExists) {
            //                $(msgControl).html("The page(s) already exists in the table");
            //            }
            return (isExists);
        }
        function CheckRightsSelected() {
            var msgControl = "#<%= divMsg.ClientID %>";
            $(msgControl).html("");
            var itemSelected = false;
            var selectObject = document.getElementById("<%= lstRights.ClientID %>");
            for (var i = 0, j = selectObject.options.length; i < j; i++) {
                if (selectObject.options[i].selected) {
                    itemSelected = true;
                }
            }

            if (!itemSelected) {
                $(msgControl).html("Please select rights.");
            }
            return (itemSelected);
        }
        function DeleteRights(id) {

            var dataArray = new Array();

            if (rightsArray != null) {
                if (rightsArray.length > 0) {

                    for (var i = 0, j = rightsArray.length; i < j; i++) {
                        var data = rightsArray[i];
                        if (data[0] != id) {
                            dataArray.push(data);
                        }
                    }
                    rightsArray = dataArray;
                    ShowRightsTable();
                }
            }

        }
        function CheckValue(value) {
            if (typeof(value) == 'undefined')
                return "";
            else
                return value;
        }
        function ShowRightsTable() {
            dataArray = rightsArray;
            $("#divGrid").html("");
            var table = "";
            if (dataArray.length > 0) {
                // Table Creation from Json Collection
                table = '<table border= \"1\" class=\"mGrid\" AlternatingRowStyle-CssClass=\"alt\"  summary=\"User Rights\" ><tr>';
                table += '<th>Delete</th>';
                table += '<th>Page</th>';
                table += '<th>Right 1</th>';
                table += '<th>Right 2</th>';
                table += '<th>Right 3</th>';
                table += '<th>Right 4</th>';
                table += '<th>Right 5</th>';
                table += '<th>Right 6</th>';
                table += '<th>Right 7</th>';
                table += '<th>Right 8</th>';
                table += '<th>Right 9</th>';
                table += '<th>Right 10</th>';
                table += '<th>Right 11</th></tr>';

                for (var i = 0, j = dataArray.length; i < j; i++) {
                    var data = dataArray[i];
                    table += '<tr><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteRights(\'' + data[0] + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/del.gif" alt=\"delete\" /> </a>'
                    + '</td><td>&nbsp;' + CheckValue(data[1])
                    + '</td><td>&nbsp;' + CheckValue(data[2])
                    + '</td><td>&nbsp;' + CheckValue(data[3])
                    + '</td><td>&nbsp;' + CheckValue(data[4])
                    + '</td><td>&nbsp;' + CheckValue(data[5])
                    + '</td><td>&nbsp;' + CheckValue(data[6])
                    + '</td><td>&nbsp;' + CheckValue(data[7])
                    + '</td><td>&nbsp;' + CheckValue(data[8])
                    + '</td><td>&nbsp;' + CheckValue(data[9])
                    + '</td><td>&nbsp;' + CheckValue(data[10])
                    + '</td><td>&nbsp;' + CheckValue(data[11])
                    + '</td><td>&nbsp;' + CheckValue(data[12])
                    + '</td></tr>';
                }
                table += '</table>';
            }
            /* insert the html string*/
            $("#divGrid").html(table);
        }
        function ToggleSelection(selectObject, isSelected) {
            for (var i = 0, j = selectObject.options.length; i < j; i++) {
                selectObject.options[i].selected = (!isSelected);
            }
        }

        function ToggleApplicationSelection() {            
            var selectObject = document.getElementById("<%= lstApplications.ClientID %>");
            var isSelected = selectObject.options[0].selected;
            var checkObj = document.getElementById("<%= chkApplicationSelect.ClientID %>");
            checkObj.checked = (!isSelected);
            ToggleSelection(selectObject, isSelected);
            //Call code behind index changed event
            document.getElementById("<%= btnApplicationSelect.ClientID %>").click();
        }

        function TogglePageSelection() {
        try{
            var selectObject = document.getElementById("<%= lstPages.ClientID %>");
            var isSelected = selectObject.options[0].selected;
            var checkObj = document.getElementById("<%= chkSelectAllPages.ClientID %>");
            checkObj.checked = (!isSelected);
            ToggleSelection(selectObject, isSelected);
            }catch(e){}
        }
        function ToggleRightsSelection() {
        try{
            var selectObject = document.getElementById("<%= lstRights.ClientID %>");
            var isSelected = selectObject.options[0].selected;
            var checkObj = document.getElementById("<%= chkSelectAllRights.ClientID %>");
            checkObj.checked = (!isSelected);
            ToggleSelection(selectObject, isSelected);
            }catch(e){}
        }

        function GetRightsText() {
            var rightsData = "";
            if (rightsArray != null) {
                if (rightsArray.length > 0) {

                    for (var i = 0, j = rightsArray.length; i < j; i++) {
                        var data = rightsArray[i];
                        if (rightsData != "") {
                            rightsData = rightsData + "#"
                        }
                        rightsData = rightsData + data[0] + "|" + data[1] + "|" + data[2] + "|" + data[3] + "|" + data[4] + "|" + data[5] + "|" + data[6] + "|" + data[7] + "|" + data[8] + "|" + data[9] + "|" + data[10] + "|" + data[11] + "|" + data[12];
                    }
                }
            }
            return rightsData;
        }
        function ShowGroupRights(rightText) {
            rightsArray = new Array();
            var dataArray = rightText.split("#");
            if (dataArray != null) {
                if (dataArray.length > 0) {

                    for (var i = 0, j = dataArray.length; i < j; i++) {
                        var data = dataArray[i];
                        subitemArray = data.split("|")
                        if (subitemArray.length < 8) {
                            for (m = subitemArray.length, n = 8; m < n; m++) {
                                subitemArray.push("");
                            }
                        }
                        rightsArray.push(subitemArray);
                    }
                }
            }
            rightsIdentity = i;
            ShowRightsTable();
            return true;
        }
    </script>
    <style type="text/css">
        .style1
        {
            width: 1070px;
        }
        .style2
        {
            width: 270px;
        }
        .style3
        {
            width: 552px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Label CssClass="CurrentPath" ID="lblPagePath" runat="server" Text="Home  &gt;  User Roles"></asp:Label>
            <div class="GVDiv">
                <asp:Label CssClass="PageHeadings" ID="lblHeading" runat="server" Text="Add New User Role"></asp:Label>
                <div id="divMsg" runat="server" style="color: Red">
                    &nbsp;</div>
                <fieldset>
                    <table>
                        <tr>
                            <td valign="top">
                                <div>
                                    <div>
                                        <asp:Label ID="Label1" runat="server" CssClass="LabelStyle" Text="Role"></asp:Label><span
                                            class="AstrickStyle">&nbsp;*</span></div>
                                    <asp:TextBox ID="txtGroupName" runat="server" TextMode="SingleLine" MaxLength="30"
                                        Text="" OnKeyPress="return CheckvarcharKeyInfo(event)"></asp:TextBox>
                                </div>
                            </td>
                            <td>
                                <div>
                                    <div>
                                        <asp:Label ID="Label2" runat="server" CssClass="LabelStyle" Text="Description" /></div>
                                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <!-- Module controls html start -->
                            <td>
                                <div>
                                    <div>
                                        <asp:Label ID="lblApplicationSelect" runat="server" CssClass="LabelStyle" Text="Select Applications" /><span
                                            class="AstrickStyle">&nbsp;*</span>
                                    </div>
                                    <asp:CheckBox ID="chkApplicationSelect" runat="server" Text="Select All" onclick="ToggleApplicationSelection();"
                                        CssClass="RadioButtonStlye" Enabled="false" /><br />
                                </div>
                                <div>
                                    <div>
                                    </div>
                                    <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                                        <ContentTemplate>
                                            <asp:ListBox ID="lstApplications" runat="server" Width="258px" SelectionMode="Single"
                                                AutoPostBack="True" Height="150px" OnSelectedIndexChanged="lstApplications_SelectedIndexChanged"
                                                onclick="$('[id$=chkSelectAllPages]').attr('checked', false); $('[id$=chkSelectAllRights]').attr('checked', false);">
                                            </asp:ListBox>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </td>
                            <td>
                                <div>
                                    <div>
                                        <asp:Label ID="Label4" runat="server" CssClass="LabelStyle" Text="Select Pages" /><span
                                            class="AstrickStyle">&nbsp;*</span>
                                    </div>
                                    <asp:CheckBox ID="chkSelectAllPages" runat="server" Text="Select All" CssClass="RadioButtonStlye"
                                        onclick="TogglePageSelection();" /><br />
                                </div>
                                <div>
                                    <div>
                                        <asp:Label ID="Label7" runat="server" CssClass="LabelStyle" Text=" " /></div>
                                    <asp:ListBox ID="lstPages" runat="server" Width="258px" SelectionMode="Multiple"
                                        Height="150px" onclick="$('[id$=chkSelectAllPages]').attr('checked', false);"></asp:ListBox>
                                </div>
                            </td>
                            <!-- Module controls html end -->
                            <td>
                                <div>
                                    <div>
                                        <asp:Label ID="Label3" runat="server" CssClass="LabelStyle" Text="Select Rights" /><span
                                            class="AstrickStyle">&nbsp;*</span></div>
                                    <asp:CheckBox ID="chkSelectAllRights" runat="server" Text="Select All" CssClass="RadioButtonStlye"
                                        onclick="ToggleRightsSelection();" /><br />
                                </div>
                                <div>
                                    <div>
                                        <asp:Label ID="Label5" runat="server" CssClass="LabelStyle" Text="" /></div>
                                    <asp:ListBox ID="lstRights" runat="server" SelectionMode="Multiple" Width="258px"
                                        Height="150px" onclick="$('[id$=chkSelectAllRights]').attr('checked', false);"></asp:ListBox>
                                    <br />
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
                            <td colspan="3">
                            </td>
                            <tr>
                                <td colspan="3">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                </td>
                            </tr>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td colspan="2">
                                <div>
                                    &nbsp;<asp:HiddenField ID="hdnUserName" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnAction" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnCurrentGroupId" runat="server" Value="" />
                                    <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
                                    <asp:Button ID="btnAdd" runat="server" Text="Add" OnClientClick="AddPageRights();return false"
                                        CssClass="btnadd" TagName="Read" />
                                    <asp:Button ID="btnsearchagain" runat="server" Text="Search Again" TagName="Read"
                                        CssClass="btnsearchagain" OnClick="btnsearchagain_Click" />
                                    <asp:Button ID="btnUpdate" runat="server" Text="Submit" OnClientClick="Submit(); return false;"
                                        CssClass="btnsave" TagName="Edit" />
                                    <asp:Button ID="btnClear" runat="server" Text="Clear" OnClientClick="ClearData(); return false;"
                                        CssClass="btnclear" TagName="Edit" />
                                    <asp:Button ID="btnApplicationSelect" runat="server" OnClick="lstApplications_SelectedIndexChanged"
                                        CssClass="HiddenButton" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <div id="divGrid" style="overflow-x: auto; height: 300px; width: 1000px">
                                    &nbsp;</div>
                                <div id="div1" style="width: 300px;">
                                    &nbsp;</div>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
