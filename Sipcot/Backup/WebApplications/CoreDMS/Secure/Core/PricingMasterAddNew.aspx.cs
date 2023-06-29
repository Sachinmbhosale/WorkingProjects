using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class PricingMasterSearch : PageBase
    {
        Price objPrice = new Price();
        DataSet Dataset_Price = new DataSet();

       
        protected void Page_Load(object sender, EventArgs e)
        {
            Results result = new Results();
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";
            string pageRights = GetPageRights();
            hdnPageRights.Value = pageRights;
            ApplyPageRights(pageRights, this.Form.Controls);
            btnSubmit.Attributes.Add("onclick", "javascript:return validate()");
            if (!IsPostBack && Request.QueryString["id"] != null)
            {
                objPrice.PriceId = Convert.ToInt32(Request.QueryString["id"]);
                result = new PriceBL().ManagePrice(objPrice, "GetPrice", hdnLoginToken.Value, Convert.ToInt32(hdnLoginOrgId.Value));
                edit(result.ResultDS);
            }
            else if (!IsPostBack)
            {
                if (Request.QueryString["action"] != null)
                {
                    string action = Request.QueryString["action"].ToString();
                    if (action.ToLower() == "edit")
                    {
                        btnserachagain.Visible = true;
                        Get_DropdownDetails();
                    }
                }
                else
                {
                    btnserachagain.Visible = false;
                    Get_DropdownDetails();
                }


            }
        }
        public void edit(DataSet Dataset_Price)
        {
            SecurityBL bl = new SecurityBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                Results res = bl.SearchOrgs(filter, "SearchOrgs", hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value);
                if (res.ActionStatus == "SUCCESS" && res.Orgs != null)
                {
                    drpCustomer.Items.Add(new ListItem("<Select>", "0"));
                    foreach (Org org in res.Orgs)
                    {
                        drpCustomer.Items.Add(new ListItem(org.OrgName, org.OrgId.ToString()));
                    }

                }
                drpCustomer.SelectedValue = Dataset_Price.Tables[0].Rows[0][0].ToString();
                drpCustomer.Enabled = false;
            }
            catch
            {
              
            }
            drpDocutype.Items.Clear();
            DocumentTypeBL b2 = new DocumentTypeBL();
            SearchFilter filter2 = new SearchFilter();
            filter2.CurrOrgId = Convert.ToInt32(drpCustomer.SelectedValue);
            try
            {
                string action = "GetDocumnetTypeForAOrg";
                Results res = b2.GetDocumnetTypeForAOrg(filter2, action, hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value);

                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    drpDocutype.Items.Add(new ListItem("<Select>", "0"));
                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        drpDocutype.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                    }
                }
                drpDocutype.SelectedValue = Dataset_Price.Tables[0].Rows[0][1].ToString();
                drpDocutype.Enabled = false;
                drpBilltype.Items.Clear();
                drpBilltype.Items.Add(Dataset_Price.Tables[0].Rows[0][2].ToString());
                txtCharge.Text = Dataset_Price.Tables[0].Rows[0][3].ToString();
                drpCurrency.Items.Clear();
                drpCurrency.Items.Add(Dataset_Price.Tables[0].Rows[0][4].ToString());
            }
            catch
            {
            }
        }
        public void Get_DropdownDetails()
        {
            SecurityBL bl = new SecurityBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                Results res = bl.SearchOrgs(filter, "SearchOrgs", hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value);
                if (res.ActionStatus == "SUCCESS" && res.Orgs != null)
                {
                    drpCustomer.Items.Add(new ListItem("<Select>", "0"));
                    foreach (Org org in res.Orgs)
                    {
                        drpCustomer.Items.Add(new ListItem(org.OrgName, org.OrgId.ToString()));
                    }

                }


            }
            catch
            {

            }


        }
        protected void drpCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpCustomer.SelectedValue.ToString() != "0")
            {
                drpDocutype.Items.Clear();
                DocumentTypeBL bl = new DocumentTypeBL();
                SearchFilter filter = new SearchFilter();
                filter.CurrOrgId = Convert.ToInt32(drpCustomer.SelectedValue);
                try
                {
                    string action = "GetDocumnetTypeForAOrg";
                    Results res = bl.GetDocumnetTypeForAOrg(filter, action, hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value);

                    if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                    {
                        drpDocutype.Items.Add(new ListItem("<Select>", "0"));
                        foreach (DocumentType dp in res.DocumentTypes)
                        {
                            drpDocutype.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                        }
                    }
                }
                catch
                {
                }
            }
            else
            {
                drpDocutype.Items.Clear();
                drpDocutype.Items.Add(new ListItem("<Select>", "0"));
            }

        }

        protected void btnSubmit_Click1(object sender, EventArgs e)
        {
            Results result = new Results();
            objPrice.CustomerId = Convert.ToInt32(drpCustomer.SelectedValue);
            objPrice.DocumentTypeId = Convert.ToInt32(drpDocutype.SelectedValue);
            objPrice.BillType = drpBilltype.SelectedValue;
            objPrice.Charges = Convert.ToDecimal(txtCharge.Text);
            objPrice.Currency = drpCurrency.SelectedValue;
            if (Request.QueryString["id"] != null)
            {
                objPrice.PriceId = Convert.ToInt32(Request.QueryString["id"]);
                result = new PriceBL().ManagePrice(objPrice, "UpdatePrice", hdnLoginToken.Value, Convert.ToInt32(hdnLoginOrgId.Value));
            }
            else
            {
                result = new PriceBL().ManagePrice(objPrice, "AddPrice", hdnLoginToken.Value, Convert.ToInt32(hdnLoginOrgId.Value));
            }

            if (result.ErrorState == 0)
            {
                divMsg.Style.Add("color", "green");
                divMsg.InnerHtml = result.Message;
            }
            else
            {
                divMsg.Style.Add("color", "red");
                divMsg.InnerHtml = result.Message;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Pricing.aspx");
        }

        protected void btnserachagain_Click(object sender, EventArgs e)
        {
            string SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
            Response.Redirect("Pricing.aspx?Search=" + SearchCriteria);
        }
    }

}
