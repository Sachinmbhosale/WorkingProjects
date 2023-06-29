using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class Pricing : PageBase
    {
        Price objPrice = new Price();
        public const string PageName = "PRICING_MST_SEARCH";
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";

            btnSearch.Attributes.Add("onclick", "javascript:return validate()");
            if (!IsPostBack)
            {
                if (Request.QueryString["Search"] != null)
                {
                    string[] search = Request.QueryString["Search"].Split('|');
                    Get_DropdownDetails();
                    cmbCustomer.Items.FindByText(search[0]).Selected = true;
                    LoadDocumentTypes();
                    cmbDocumentType.Items.FindByText(search[1]).Selected = true;
                    cmbBillingType.Items.FindByText(search[2]).Selected = true;
                    Price_GridResult("SearchPrice");
                }
                else
                {
                    cmbBillingType.SelectedIndex = 0;
                    Grid_result.Visible = false;
                    Get_DropdownDetails();
                }
            }
        }

        protected void Price_GridResult(string action)
        {
            Grid_result.Visible = true;
            objPrice.CustomerId = Convert.ToInt32(cmbCustomer.SelectedValue);
            objPrice.DocumentTypeId = Convert.ToInt32(cmbDocumentType.SelectedValue);
            objPrice.BillType = cmbBillingType.SelectedValue;
            Results result = new PriceBL().ManagePrice(objPrice, action, hdnLoginToken.Value, Convert.ToInt32(hdnLoginOrgId.Value));

            if (result.ErrorState == 0)
            {
                if (action == "SearchPrice")
                {
                    if (result.ResultDS.Tables.Count > 0 && result.ResultDS.Tables[0].Rows.Count > 0)
                    {
                        divMsg.InnerText = string.Empty;
                        Grid_result.Visible = true;
                        Grid_result.DataSource = result.ResultDS;
                        Grid_result.DataBind();
                    }
                    else
                    {
                        divMsg.Style.Add("color", "red");
                        divMsg.InnerText = "No Records Found.";
                    }
                }
                else
                {
                    divMsg.Style.Add("color", "green");
                    divMsg.InnerText = result.Message;
                }
            }
            else
            {
                divMsg.Style.Add("color", "red");
                divMsg.InnerText = result.Message;
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Price_GridResult("SearchPrice");
        }

        protected void btnadd_Click(object sender, EventArgs e)
        {
            Response.Redirect("PricingMasterAddNew.aspx");
        }

        protected void Imbtn_Edit_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ImgEdit = (ImageButton)sender;
            GridViewRow Row = (GridViewRow)ImgEdit.NamingContainer;
            Label Slno = (Label)Row.FindControl("Lbl_Slno");
            Response.Redirect("PricingMasterAddNew.aspx?id=" + Slno.Text + "&search=" + cmbCustomer.SelectedItem.Text.ToString() + "|" + cmbDocumentType.SelectedItem.Text.ToString() + "|" + cmbBillingType.SelectedItem.Text.ToString());
        }

        protected void Imbtn_Delete_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton delete = (ImageButton)sender;
            GridViewRow row = (GridViewRow)delete.NamingContainer;
            Label Price_Slno = (Label)row.FindControl("Lbl_Slno");            
            objPrice.PriceId = Convert.ToInt32(Price_Slno.Text);            
            Price_GridResult("DeletePrice");
        }

        protected void Grid_result_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string Price_Slno = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Pricing_iID"));
                ImageButton Delete = (ImageButton)e.Row.FindControl("Imbtn_Delete");
                Delete.Attributes.Add("onclick", "javascript:return ConfirmationBox('" + Price_Slno + "')");
            }
        }
        public void Get_DropdownDetails()
        {
            cmbCustomer.Items.Clear();
            cmbDocumentType.Items.Add(new ListItem("<Select>", "0"));
            SecurityBL bl = new SecurityBL();
            SearchFilter filter = new SearchFilter();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            try
            {
                cmbCustomer.Items.Add(new ListItem("<Select>", "0"));
                Results res = bl.SearchOrgs(filter, "SearchOrgs", loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                if (res.ActionStatus == "SUCCESS" && res.Orgs != null)
                {
                    foreach (Org org in res.Orgs)
                    {
                        cmbCustomer.Items.Add(new ListItem(org.OrgName, org.OrgId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedValue.ToString() != "0")
            {
                LoadDocumentTypes();
            }

        }

        protected void LoadDocumentTypes()
        {
            cmbDocumentType.Items.Clear();
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
            filter.CurrOrgId = Convert.ToInt32(cmbCustomer.SelectedValue);
            try
            {
                string action = "GetDocumnetTypeForAOrg";
                Results res = bl.GetDocumnetTypeForAOrg(filter, action, hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value);

                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    cmbDocumentType.Items.Add(new ListItem("<Select>", "0"));
                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        cmbDocumentType.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
