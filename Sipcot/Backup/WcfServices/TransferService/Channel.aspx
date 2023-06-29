<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Channel.aspx.cs" Inherits="TransferService.Channel"
    EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">
    <title>Manage Channel</title>
    <link href="css/bootstrap.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div class="navbar navbar-default navbar-fixed-top">
    </div>
    <div class="container">
        <div class="page-header" id="banner">
            <div class="row">
                <div class="col-lg-8 col-md-7 col-sm-6">
                    <h2>
                        Manage Channels
                    </h2>
                    <%-- <p class="lead">
                        routing upload manager</p>--%>
                </div>
            </div>
        </div>
        <form id="form1" runat="server">
        <div>
            <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
        </div>
        <div class="bs-docs-section">
            <div class="row">
                <div class="col-lg-5">
                    <div class="well bs-component">
                        <div class="form-horizontal">
                            <fieldset>
                                <legend>Enter details </legend>
                                <div class="form-group">
                                    <asp:Label ID="Label8" runat="server" Text="Channel Id" CssClass="col-lg-2 control-label"
                                        for="disabledInput"></asp:Label>
                                    <div class="col-lg-4">
                                        <asp:TextBox ID="txtChannelId" runat="server" class="form-control" ReadOnly="true"></asp:TextBox>
                                    </div>
                                    <asp:Label ID="Label7" runat="server" Text="Active" CssClass="col-lg-2 control-label"></asp:Label>
                                    <asp:CheckBox ID="chkActive" runat="server" Checked="True" />
                                </div>
                                <div class="form-group">
                                   <%-- <asp:Label ID="Label3" runat="server" Text="Channel Name" CssClass="col-lg-2 control-label"></asp:Label>--%>
                                    <div class="col-lg-10">
                                        <asp:TextBox ID="txtChannel" runat="server" class="form-control" placeholder="Channel Name" ToolTip="Channel Name"></asp:TextBox></div>
                                </div>
                                <div class="form-group">
                                   <%-- <asp:Label ID="Label4" runat="server" Text="File upload path" CssClass="col-lg-2 control-label"></asp:Label>--%>
                                    <div class="col-lg-10">
                                        <asp:TextBox ID="txtFileUploadPath" runat="server" class="form-control" placeholder="File upload path" ToolTip="File upload path"></asp:TextBox></div>
                                </div>
                                <div class="form-group">
                                   <%-- <asp:Label ID="Label5" runat="server" Text="Description" CssClass="col-lg-2 control-label"></asp:Label>--%>
                                    <div class="col-lg-10">
                                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" class="form-control" placeholder="Description" ToolTip="Description"></asp:TextBox></div>
                                </div>
                                <div class="form-group">
                                   <%-- <asp:Label ID="Label1" runat="server" Text="User name" CssClass="col-lg-2 control-label"></asp:Label>--%>
                                    <div class="col-lg-10">
                                        <asp:TextBox ID="txtUserName" runat="server" class="form-control" placeholder="User name" ToolTip="User name"></asp:TextBox></div>
                                </div>
                                <div class="form-group">
                                    <%--<asp:Label ID="Label2" runat="server" Text="Password" CssClass="col-lg-2 control-label"></asp:Label>--%>
                                    <div class="col-lg-10">
                                        <asp:TextBox ID="txtPassword" runat="server" class="form-control" placeholder="Password" ToolTip="Password"></asp:TextBox></div>
                                </div>
                                <div class="form-group">
                                    <%--<asp:Label ID="Label6" runat="server" Text="Email Id" CssClass="col-lg-2 control-label"
                                        for="inputEmail"></asp:Label>--%>
                                    <div class="col-lg-10">
                                        <asp:TextBox ID="txtEmailId" runat="server" class="form-control" placeholder="Email Id" ToolTip="Email Id"></asp:TextBox></div>
                                </div>
                                <asp:HiddenField ID="hdnAction" runat="server" />
                                <legend></legend>
                                <div class="pull-right">
                                    <asp:Button ID="btnAddNew" runat="server" Text="New Channel" CssClass="btn btn-primary"
                                        OnClick="btnAddNew_Click" />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete Channel" Visible="false"
                                        CssClass="btn btn-danger" onclick="btnDelete_Click" OnClientClick="return window.confirm('Are you sure?');" />
                                    <asp:Button ID="btnSave" runat="server" Text="Save Channel" CssClass="btn btn-success"
                                        OnClick="btnSave_Click" />
                                </div>
                            </fieldset>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6 col-lg-offset-1">
                    <legend>Available Channels </legend>
                    <div class="bs-component">
                        <asp:GridView ID="gvChannel" runat="server" CssClass="table table-striped table-hover "
                            OnRowDataBound="gvChannel_RowDataBound" OnSelectedIndexChanged="OnSelectedIndexChanged">
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
        </form>
        <footer>
        <div class="row">
          <div class="col-lg-12">

            <ul class="list-unstyled">
              <li class="pull-right"><a href="#top">Back to top</a></li>
            </ul>
          </div>
        </div>
      </footer>
    </div>
    <script type="text/javascript">

        function DeleteSelectedRow(lnk) {
            var row = lnk.parentNode.parentNode;
            var rowIndex = row.rowIndex - 1;
        }

//        function GetSelectedRow(lnk) {
            //        var row = lnk.parentNode.parentNode;
            //        var rowIndex = row.rowIndex - 1;
            //        document.getElementById('<%=hdnAction.ClientID %>').value = "Edit";
            //        document.getElementById('<%=txtChannelId.ClientID %>').value = row.cells[0].innerHTML;
            //        document.getElementById('<%=txtChannel.ClientID %>').value = row.cells[2].innerHTML;
            //        document.getElementById('<%=txtFileUploadPath.ClientID %>').value = row.cells[3].innerHTML;
            //        document.getElementById('<%=txtDescription.ClientID %>').value = row.cells[4].innerHTML;
            //        document.getElementById('<%=txtUserName.ClientID %>').value = row.cells[5].innerHTML;
            //        document.getElementById('<%=txtPassword.ClientID %>').value = row.cells[6].innerHTML;
            //        document.getElementById('<%=txtEmailId.ClientID %>').value = row.cells[7].innerHTML;
            //        document.getElementById('<%=chkActive.ClientID %>').checked = row.cells[8].innerHTML;
            //        // var city = row.cells[1].getElementsByTagName("input")[0].value;
            //return false;
//        }
    </script>
</body>
</html>
