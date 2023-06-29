<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkFlowWizardMenu.ascx.cs"
    Inherits="Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin.WorkFlowWizardMenu" 
    meta:resourcekey="WorkFlowWizardMenu" %>

<div class="MainStripBGWizard">
    <center>
        <asp:Menu ID="mnuWFWizard" runat="server" Enabled="False" Orientation="Horizontal"
            StaticSubMenuIndent="" Visible="true">
            <Items>
                <asp:MenuItem ImageUrl="../images/process_icon.png" Text="&amp;nbsp;&amp;nbsp;Process"
                    Value="Process" SeparatorImageUrl="../images/sep_03.png" Selected="True" meta:resourcekey="mnuProcess" ></asp:MenuItem>
                <asp:MenuItem ImageUrl="../images/workflow_icon.png" Text="&amp;nbsp;&amp;nbsp;Work Flow"
                    Value="Work Flow" SeparatorImageUrl="../images/sep_03.png" meta:resourcekey="mnuWorkFlow" ></asp:MenuItem>
                <asp:MenuItem ImageUrl="../images/stage_icon.png" SeparatorImageUrl="../images/sep_03.png"
                    Text="&amp;nbsp;&amp;nbsp;Stages" Value="Stages" meta:resourcekey="mnuStages" ></asp:MenuItem>
                <asp:MenuItem ImageUrl="../images/fields_icon.png" SeparatorImageUrl="../images/sep_03.png"
                    Text="&amp;nbsp;&amp;nbsp;Fields" Value="Fields" meta:resourcekey="mnuFields" ></asp:MenuItem>
                <asp:MenuItem ImageUrl="../images/users_icon.png" SeparatorImageUrl="../images/sep_03.png"
                    Text="&amp;nbsp;&amp;nbsp;Users" Value="Users" meta:resourcekey="mnuUsers" ></asp:MenuItem>
                <asp:MenuItem ImageUrl="../images/status_icon.png" Text="&amp;nbsp;&amp;nbsp;Status"
                    Value="Status" SeparatorImageUrl="../images/sep_03.png" meta:resourcekey="mnuStatuses" ></asp:MenuItem>
                <asp:MenuItem ImageUrl="../images/notification_icon.png" Text="&amp;nbsp;&amp;nbsp;Notification"
                    Value="Notification" meta:resourcekey="mnuNotification" ></asp:MenuItem>
            </Items>
            <LevelMenuItemStyles>
                <asp:MenuItemStyle CssClass="WizardTabMenu" Font-Underline="False" />
            </LevelMenuItemStyles>
            <LevelSelectedStyles>
                <asp:MenuItemStyle CssClass="WizardTabMenuSelected" Font-Underline="False" />
            </LevelSelectedStyles>
        </asp:Menu>
    </center>
</div>
