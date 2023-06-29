<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentAddNew.aspx.cs"
    Inherits="Lotex.EnterpriseSolutions.WebUI.DptAddNew" MasterPageFile="~/SecureMaster.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7-vsdoc.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.min.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Scripts/jquery-1.7.js") %>"></script>
    <script type="text/javascript" language="javascript" src="<%=Page.ResolveClientUrl("~/Assets/Scripts/AjaxPostScripts.js") %>"></script>
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/Assets/Scripts/master.js") %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">
        //Department_Add Final Validation
        function DeptAddValidate() {

            var Deptname = document.getElementById('<%= txtDptName.ClientID %>');
            if (Deptname.value.length < 2) {
                var msgControl = "#<%= divMsg.ClientID %>";
                $(msgControl).html("Department Name should contain atleast two character!");
                return false;
            }
            else {
                return true;
            }
        }

        //texbox validation - Only Characters and '-' Symbol
        function CheckNumericKeyInfo(event) {
            var char1 = (event.keyCode ? event.keyCode : event.which);
            if ((char1 >= 65 && char1 <= 90) || (char1 >= 97 && char1 <= 122) || char1 == 45 || char1 == 32 || char1 == 8 || char1 == 46) {
                RetVal = true;
            }
            else {
                RetVal = false;
            }
            return RetVal;
        }

    </script>
    <asp:Label CssClass="CurrentPath" ID="lblPagePath" runat="server" Text="Home  > Department"></asp:Label>
    <div class="GVDiv">
    <asp:Label CssClass="PageHeadings" ID="lblHeading" runat="server" Text="Add New Department"></asp:Label>
    <br/>
        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="lblPageDescription" runat="server"
            Text="*"></asp:Label>
        <asp:Label CssClass="CurrentPath" ID="lblPageDescription0" runat="server" Text=" - Indicates mandatory fields"></asp:Label>
        <div id="divMsg" runat="server" style="color: Red; font-family: Calibri; font-size: small;">
        </div>
        <fieldset>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblDprtmntName" runat="server" CssClass="LabelStyle" Text="Department Name "></asp:Label>
                        <asp:Label CssClass="MandratoryFieldMarkStyle" ID="lblPageDescription1" runat="server"
                            Text="*"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDptName" runat="server" OnKeyPress="return CheckNumericKeyInfo(event)"
                            MaxLength="20"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDescription" runat="server" CssClass="LabelStyle" Text="Description"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblHead" runat="server" CssClass="LabelStyle" Text="Head "></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtHead" runat="server" OnKeyPress="return CheckNumericKeyInfo(event)"
                            MaxLength="20"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style7">
                    </td>
                    <td>
                        <div visible="false">
                            <asp:TextBox ID="FreeText1" runat="server" Visible="false"></asp:TextBox></div>
                        <div visible="false">
                            <asp:TextBox ID="FreeText2" runat="server" Visible="false"></asp:TextBox></div>
                        <div visible="false">
                            <asp:TextBox ID="FreeText3" runat="server" Visible="false"></asp:TextBox></div>
                        <div visible="false">
                            <asp:TextBox ID="FreeText4" runat="server" Visible="false"></asp:TextBox></div>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnsearchagain" runat="server" Text="Search Again" CssClass="btnsearchagain"
                             TagName="Read" OnClick="btnsearchagain_Click" />
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btnsave" OnClick="btnSubmit_Click"
                            TagName="Add" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btncancel" OnClick="btnCancel_Click"
                            TagName="Add" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <asp:HiddenField ID="hdnLoginOrgId" runat="server" Value="" />
    <asp:HiddenField ID="hdnLoginToken" runat="server" Value="" />
    <asp:HiddenField ID="HiddenField1" runat="server" Value="" />
    <asp:HiddenField ID="hdnPageId" runat="server" Value="" />
    <asp:HiddenField ID="hdnPageRights" runat="server" Value="" />
</asp:Content>
