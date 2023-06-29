<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageWorkflow.aspx.cs"
    Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.ManageWorkflow"
    MasterPageFile="~/Workflow/WorkflowMaster/WorkflowAdmin.Master" EnableEventValidation="false"
    Culture="auto" meta:resourcekey="ManageWorkflow" UICulture="auto" %>

<%@ Register TagPrefix="WF" TagName="WorkFlowWizard" Src="WorkFlowWizardMenu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1
        {
            height: 30px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        function MaxLength(txtbox, maxLength) {
            if (txtbox.value.length >= maxLength) {
                txtbox.value = txtbox.value.substring(0, maxLength - 1);
            }
        }
    </script>
    <script src="../../Scripts/jquery-1.10.2.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        function SelectSingleRadiobutton(rdbtnid) {
            var rdBtn = document.getElementById(rdbtnid);
            var rdBtnList = document.getElementsByTagName("input");
            for (i = 0; i < rdBtnList.length; i++) {
                if (rdBtnList[i].type == "radio" && rdBtnList[i].id != rdBtn.id) {
                    rdBtnList[i].checked = false;
                }
            }
        }

        function Validate(buttonId) {
            var gv = document.getElementById("<%=gridWorkflow.ClientID%>");
            var rbs = gv.getElementsByTagName("input");
            var lblMsg = document.getElementById("<%= lblMessageGrid.ClientID %>");
            var PreviewButtonId = document.getElementById("<%= btnPreview.ClientID %>");
            var flag = 0;
            for (var i = 0; i < rbs.length; i++) {
                if (rbs[i].type == "radio") {
                    if (rbs[i].checked) {
                        flag = 1;
                        break;
                    }
                }
            }
            if (flag == 0) {
                lblMsg.innerHTML = "Please select a row to proceed.";
                return false;
            }

            else {
                if (buttonId.id == PreviewButtonId.id) {
                    ShowConfirmPopUp(); // DMSENH6-4732   

                }

            }
        }


        function ShowMD() {
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "";
            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            var txtWorkflowName = document.getElementById("<%= txtWorkflowName.ClientID %>");
            var txtWorkflowDescription = document.getElementById("<%= txtWorkflowDescription.ClientID %>");
            document.getElementById("<%= ddlDmsProject.ClientID%>").value = "0";

            txtWorkflowName.value = "";
            txtWorkflowDescription.value = "";
            modelBG.className = "mdBG";
            mb.className = "mdbox";
            return false;
        };

        // DMSENH6-4732 BS
        function ShowConfirmPopUp() {
       
            divConfirm.className = "mdBG";
            divConfirmControls.className = "confirmbox";
            return false;
        }
        // DMSENH6-4732 BE
        function ShowMDReload(WorkflowIdId, WorkflowName, WorkflowDescription, IsActive, DMSProject) {

            var txtWorkflowName = document.getElementById("<%= txtWorkflowName.ClientID %>");
            var txtWorkflowDescription = document.getElementById("<%= txtWorkflowDescription.ClientID %>");
            txtWorkflowName.value = WorkflowName;
            txtWorkflowDescription.value = WorkflowDescription;
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");
            document.getElementById("<%= ddlDmsProject.ClientID%>").value = DMSProject;

            if (IsActive == false) {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }
            modelBG.className = "mdBG";
            mb.className = "mdbox";
            return false;
        };

        function HideMD() {

            modelBG.className = "mdNone";
            mb.className = "mdNone";
            divConfirmControls.className = "mdNone"; // DMSENH6-4732 
            divClone.className = "mdNone";
            divCloneControls.className = "mdNone";
        }



        function GetSelectedRow(lnk) {
            var row = lnk.parentNode.parentNode;
            var WorkflowId = row.cells[3].innerHTML;
            var Workflowname = row.cells[4].innerHTML;
            var Workflowdescription = row.cells[5].innerHTML.replace(/(&nbsp;)/g, '');
            var Isactive = row.cells[9].innerHTML;
            var ProjectName = row.cells[11].innerHTML;
            var SortOrder = row.cells[12].innerHTML;
            var WorkflowConfirmed = row.cells[13].innerHTML;

            //to disable save buttton
            var btnSave = document.getElementById("<%= btnSave.ClientID %>");
            if (WorkflowConfirmed === "Confirmed") {
                btnSave.disabled = true;
            }
            else { btnSave.disabled = false; }

            document.getElementById("<%= lblMessage.ClientID %>").innerHTML = "";
            var txtWorkflowName = document.getElementById("<%= txtWorkflowName.ClientID %>");
            var txtWorkflowDescription = document.getElementById("<%= txtWorkflowDescription.ClientID %>");
            var lstSortOrder = document.getElementById("<%= lstSortOrder.ClientID%>");
            var Checkboxactive = document.getElementById("<%= chkActive.ClientID %>");
            if (Isactive == "Inactive") {
                Checkboxactive.checked = false;
            }
            else { Checkboxactive.checked = true; }
            document.getElementById("<%= ddlDmsProject.ClientID%>").value = (ProjectName == "" || ProjectName == "&nbsp;") ? "0" : ProjectName;
            //to bind selected value from DB for list box
            for (var i = 0, j = lstSortOrder.options.length; i < j; ++i) {
                if (lstSortOrder.options[i].innerHTML === SortOrder) {
                    lstSortOrder.selectedIndex = i;
                    break;
                }
            }
            // document.getElementById("<%= lstSortOrder.ClientID%>").value = (SortOrder == "" || SortOrder == "&nbsp;") ? "0" : SortOrder;
            txtWorkflowName.value = Workflowname;
            txtWorkflowDescription.value = Workflowdescription;
            document.getElementById("<%= hdnSaveStatus.ClientID %>").value = "Save Changes";
            document.getElementById("<%= hiddenWorkflowId.ClientID %>").value = WorkflowId;

            modelBG.className = "mdBG";
            mb.className = "mdbox";

            return false;
        }


        $(document).ready(function () {

            var WorkflowName = document.getElementById("<%= txtWorkflowName.ClientID %>").value;
            var WorkflowDesc = document.getElementById("<%= txtWorkflowDescription.ClientID %>").value;
            var WorkflowActive = document.getElementById("<%= chkActive.ClientID %>").checked;
            var DmsProject = document.getElementById("<%= ddlDmsProject.ClientID%>").value;

            var hdnErrorStatus = document.getElementById("<%= hdnErrorStatus.ClientID %>").value;

            if ((hdnErrorStatus == "ADD_ERROR") || (hdnErrorStatus == "EDIT_ERROR")) {
                hdnErrorStatus.value = "";
                ShowMDReload("0", WorkflowName, WorkflowDesc, WorkflowActive, DmsProject);
            }

        });

        function CheckEmptyTextWorkflowName() {

            var WorkfloName = document.getElementById("<%= txtWorkflowName.ClientID %>");
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var re = /^[a-z 0-9 \_\-\#\@\^\$ A-Z]+$/; var uid;
            uid = WorkfloName.value;

            if (WorkfloName.value == "" || WorkfloName.value == undefined) {
                lblMsg.innerHTML = "Please enter a workflow name.";
                return false;
            }
            if (re.test(uid)) {
                return Move_Items('up');
                return true;
            }
            else {
                lblMsg.innerHTML = "Workflow name allows alphabets, numbers and few special characters ( _ - # @ ^ $ ) only.";
                return false;
            }

        }

        function Move_Items(direction) {
            var lblMsg = document.getElementById("<%= lblMessage.ClientID %>");
            var listbox = document.getElementById('<%= lstSortOrder.ClientID %>');
            var selIndex = listbox.selectedIndex;

            if (selIndex == -1) {
                lblMsg.innerHTML = "Please select  sort order.";
                return false;
            }

            var increment = -1;
            if (direction == 'up')
                increment = -1;
            else
                increment = 1;

            if ((selIndex + increment) < 0 ||
        (selIndex + increment) > (listbox.options.length - 1)) {
                return;
            }

            var selValue = listbox.options[selIndex].value;
            var selText = listbox.options[selIndex].text;
            listbox.options[selIndex].value = listbox.options[selIndex + increment].value
            listbox.options[selIndex].text = listbox.options[selIndex + increment].text

            listbox.options[selIndex + increment].value = selValue;
            listbox.options[selIndex + increment].text = selText;

            listbox.selectedIndex = selIndex + increment;


        }
        function ErrorrMessage() {
            document.getElementById("<%= lblMessageGrid.ClientID %>").innerHTML = "This workflow cannot be editable,Since it is confirmed";

        }

        function CheckEmptyCloneDropdown() {
            ShowClonePopUp();
            var ddlCloneOrg = document.getElementById("<%= ddlOrganizationClone.ClientID %>"); // $("[id$=ddlOrganizationClone]");
            var ddlCloneProcess = document.getElementById("<%= ddlProcessClone.ClientID %>"); // $("[id$=ddlProcessClone]");
            var ddlCloneWorkflow = document.getElementById("<%= ddlWorkflowClone.ClientID %>"); // $("[id$=ddlWorkflowClone]");
            var lblMsgClone = document.getElementById("<%= lblMessageClone.ClientID %>"); // $("[id$=lblMessageClone]");

            if (ddlCloneOrg.value == "0" || ddlCloneOrg.value == undefined) {
                lblMsgClone.innerHTML = "Please select an organization name.";
                return false;
            }

            if (ddlCloneProcess.value == "0" || ddlCloneProcess.value == undefined) {
                lblMsgClone.innerHTML = "Please select an process name.";
                return false;
            }
            if (ddlCloneWorkflow.value == "0" || ddlCloneWorkflow.value == undefined) {
                lblMsgClone.innerHTML = "Please select an workflow name.";
                return false;
            }
            else {
                lblMsgClone.innerHTML = "";
                return true;
            }
        }

        function ShowClonePopUp() {
            divClone.className = "mdBG";
            divCloneControls.className = "mdbox";
        }

        function FillWorkflowName() {
            var workflowvalue = document.getElementById("<%=ddlWorkflowClone.ClientID %>");
            var selectedText = workflowvalue.options[workflowvalue.selectedIndex].text;
            document.getElementById("<%=txtWorkflowRename.ClientID%>").value = selectedText;
        }

        function ConfirmWorkflow() {
            var hdnconfirmedworkflowvalue = document.getElementById("<%=hdnconfirmed.ClientID %>").value;
            if (hdnconfirmedworkflowvalue == "Confirmed") {
                alert("This workflow is already confirmed!.");
                return false;
            }
            else { return window.confirm('Are you sure you want to continue?') }
        }
   
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <WF:WorkFlowWizard ID="WorkFlowWizard1" runat="server" ActiveItemName="Work Flow" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="GVDiv">
                <asp:Label ID="lblMessageGrid" ForeColor="Red" runat="server"></asp:Label>
                <asp:GridView ID="gridWorkflow" runat="server" OnRowDataBound="gridWorkflow_RowDataBound"
                    AllowPaging="true" DataKeyNames="Workflow Id" AutoGenerateColumns="true" CssClass="mGrid"
                    PagerStyle-CssClass="pgr" CellPadding="10" CellSpacing="5" OnPageIndexChanging="gridWorkflow_PageIndexChanging"
                    PageSize="10" AlternatingRowStyle-CssClass="alt" EmptyDataText="Workflows are not available">
                    <Columns>
                        <asp:TemplateField HeaderText="Edit">
                            <ItemTemplate>
                                <asp:RadioButton ID="RowSelector" TagName="Read" runat="server" OnClick="javascript:SelectSingleRadiobutton(this.id)" />
                                <asp:LinkButton ID="lnkEdit" TagName="Read" runat="server" Text="" ToolTip="Edit"
                                    OnClientClick="return GetSelectedRow(this);" CommandArgument=''><img src="../images/Edit.png"/></asp:LinkButton>
                                <asp:LinkButton ID="lnkManageWorkflowOwners" TagName="Read" runat="server" CommandArgument=""
                                    CausesValidation="false" ToolTip="Go To Workflow Owners"><img src="../images/stageusers.png"/></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                    <PagerSettings FirstPageText="<<" LastPageText=">>" Mode="NumericFirstLast" NextPageText=" "
                        PageButtonCount="5" PreviousPageText=" " />
                    <PagerStyle CssClass="pgr" BorderStyle="None"></PagerStyle>
                </asp:GridView>
                <asp:Button ID="btnAddWorkflow" runat="server" TagName="Add" Text="Add Workflow"
                    OnClientClick="return ShowMD();" CssClass="btnaddworkflow" CausesValidation="false"
                    meta:resourcekey="btnAddWorkflow" />
                <asp:Button ID="btnGoBacktoProcess" runat="server" Text="Back To Process" meta:resourcekey="btnGoBacktoProcess"
                    CssClass="btngobackto" OnClick="btnGoBacktoProcess_Click" TagName="Read" />
                <asp:Button ID="btnGotoStage" runat="server" Text="Go To Stages" OnClientClick="return Validate(this);"
                    CssClass="btngotostage" OnClick="btnGotoStage_Click" meta:resourcekey="btnGotoStage"
                    TagName="Read" />
                <asp:Button ID="btnPreview" runat="server" Text="Preview" OnClientClick="return Validate(this);"
                    OnClick="btnPreview_Click" CssClass="btnpreview" TagName="Read" />
                <asp:Button ID="btnAddPdfcontrols" runat="server" Text="Map Pdf Controls" OnClientClick="return Validate(this);"
                    CssClass="btnmapping" TagName="Read" OnClick="btnAddPdfcontrols_Click" />
                <asp:Button ID="btnClone" runat="server" Text="Clone Workflow" OnClientClick="ShowClonePopUp();" TagName="Read"
                    Visible="false" CssClass="btnclone" />
            </div>
            <asp:HiddenField ID="hdnSaveStatus" runat="server" />
            <asp:HiddenField ID="hdnWorkFlowCount" runat="server" />
            <asp:HiddenField ID="hiddenWorkflowId" runat="server" />
            <asp:HiddenField ID="hiddenWorkflowConfirmed" runat="server" />
            <asp:HiddenField ID="hdnconfirmed" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="modelBG" class="mdNone">
    </div>
    <div id="mb" class="mdNone">
        <div id="Content">
            <table>
                <tr>
                    <td colspan="2">
                        <h3>
                            <asp:Label ID="lblEditWorkFlow" runat="server" Text="Edit Workflow Details" meta:resourcekey="lblEditWorkFlow" /></h3>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDMSProjectName" runat="server" Text="DMS Project Name"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlDmsProject">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblWorkflowName" runat="server" Text="Workflow Name" meta:resourcekey="lblWorkflowName"></asp:Label><span
                            style="color: Red; font-size: medium">*</span>
                    </td>
                    <td>
                        <asp:TextBox ID="txtWorkflowName" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblWorkflowDescription" runat="server" Text=" Workflow Description"
                            meta:resourcekey="lblWorkflowDescription"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtWorkflowDescription" runat="server" TextMode="MultiLine" onkeyup="javascript:MaxLength(this, 250)"
                            MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblWorkflowActive" runat="server" Text="Active" meta:resourcekey="lblWorkflowActive"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox ID="chkActive" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblSortOrder" runat="server" Text="Sort Order"></asp:Label>
                    </td>
                    <td>
                        <asp:ListBox ID="lstSortOrder" runat="server"></asp:ListBox>
                    </td>
                    <td style="display: none">
                        <input type="button" value="&#x25B2;" onclick="Move_Items('up')" /></br>
                        <input type="button" value="&#x25BC;" onclick="Move_Items('down')" />
                    </td>
                </tr>
            </table>
            <asp:HiddenField ID="hdnErrorStatus" runat="server" Value="" />
        </div>
        <div style="float: right;">
            <asp:Button ID="btnSave" runat="server" TagName="Add" Text="Save" OnClick="btnSave_Click"
                OnClientClick="return CheckEmptyTextWorkflowName();" CssClass="btnsave" meta:resourcekey="btnSave" />
            <asp:Button ID="btnCancel" runat="server" TagName="Read" Text="Cancel" OnClientClick="HideMD(); return false;"
                CssClass="btncancel" CausesValidation="false" meta:resourcekey="btnCancel" />
        </div>
    </div>
    <!-- Confirmation Pop Up HTML Started -->
    <div id="divConfirm" class="mdNone">
    </div>
    <div id="divConfirmControls" class="mdNone">
        <center>
            <asp:Panel ID="pnlConfirm" runat="server">
                <table>
                    <tr>
                        <td>
                            <div id="loading">
                                <b>Loading....</b>
                            </div>
                            <div id="chart">
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center; padding-top: 15px;">
                            <asp:Button ID="btnOk" runat="server" Text="Close" CssClass="btnclose" TagName="Read"/>
                            <asp:Button ID="btnConfirm" runat="server" Text="Confirm" OnClick="btnConfirm_Click"
                                CssClass="btnconfirm" OnClientClick="return ConfirmWorkflow();" TagName="Read"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <center>
                                <div id="WorkFlowCount" style="padding-top: 20px;">
                                </div>
                            </center>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </center>
    </div>
    <!-- Confirmation Pop Up HTML Ended-->
    <!--Clone Pop Up HTML Started-->
    <div id="divClone" class="mdNone">
    </div>
    <div id="divCloneControls" class="mdNone">
        <asp:Panel ID="pnlClone" runat="server">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <table>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblMessageClone" ForeColor="Red" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblCloneOrganization" runat="server" Text="Organization Name"></asp:Label><span
                                    style="color: Red; font-size: medium">*</span>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlOrganizationClone" runat="server" OnSelectedIndexChanged="ddlOrganizationClone_SelectedIndexChanged"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblCloneProcess" runat="server" Text="Process Name To Clone"></asp:Label><span
                                    style="color: Red; font-size: medium">*</span>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlProcessClone" runat="server" OnSelectedIndexChanged="ddlProcessClone_SelectedIndexChanged"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblCloneWorkflow" runat="server" Text="Workflow Name To Clone"></asp:Label><span
                                    style="color: Red; font-size: medium">*</span>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlWorkflowClone" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblWorkflowRename" runat="server" Text="Workflow ReName"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtWorkflowRename" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:Button ID="btnCloneOk" runat="server" Text="OK" CssClass="btnOK" OnClientClick="return CheckEmptyCloneDropdown();"
                                    OnClick="btnOKCloneClick" TagName="Read" />
                                <asp:Button ID="btnCloneCancel" runat="server" Text="Cancel" CssClass="btncancel"
                                    OnClientClick="HideMD(); return false;" TagName="Read" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
    </div>
    <!--Clone Pop Up HTML Ended-->
    <!-- Pictorial representation scripts started -->
    <script src="../../Scripts/jsapi.JS" type="text/javascript"></script>
    <script type="text/javascript">

        function drawChart() {
            $('#loading').show();
            $.ajax({
                type: "POST",
                url: "../../Workflow/WorkflowAdmin/ManageWorkflow.aspx/GetChartData",
                data: '{}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    var data = new google.visualization.DataTable();
                    data.addColumn('string', 'Entity');
                    data.addColumn('string', 'ParentEntity');
                    data.addColumn('string', 'ToolTip');
                    for (var i = 0; i < r.d.length; i++) {
                        var memberId = r.d[i][0].toString();
                        var memberName = r.d[i][1];
                        var parentId = r.d[i][2] != null ? r.d[i][2].toString() : '';
                        data.addRows([[{ v: memberId,
                            f: memberName //+ '<div><img src = "Pictures/' + memberId + '.jpg" /></div>'
                        }, parentId, memberName]]);
                    }
                    var chart = new google.visualization.OrgChart($("#chart")[0]);
                    chart.draw(data, { allowHtml: true });
                },
                complete: function () {
                    $('#loading').hide();

                },
                failure: function (r) {
                    alert(r.d);
                },
                error: function (r) {
                    alert(r.d);
                }

            });
            var Text = $("#<%=hdnWorkFlowCount.ClientID %>").val();
            Text = Text.replace(/n/g, '');
            $("#WorkFlowCount").html(Text);
        }
    </script>
    <!-- Pictorial representation scripts ends -->
</asp:Content>
