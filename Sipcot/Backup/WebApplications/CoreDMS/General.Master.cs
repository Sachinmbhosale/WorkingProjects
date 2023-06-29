using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class General : MasterPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) 
            {
                imgLogo.Src = GetCompanyLogoPath();
                imgLogo.Alt = GetCompanyName();
                var control = ContentPlaceHolder1.Controls.Cast<Control>()
                              .Where(o => o is TextBox)
                              .FirstOrDefault();

                if (control != null)
                {
                    control.Focus();
                }
                ClearApplicationCache();
            }
        }
        public void ClearApplicationCache()
        {
            List<string> keys = new List<string>();


            // retrieve application Cache enumerator
            IDictionaryEnumerator enumerator = Cache.GetEnumerator();


            // copy all keys that currently exist in Cache
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }


            // delete every key from cache
            for (int i = 0; i < keys.Count; i++)
            {
                Cache.Remove(keys[i]);
            }
        }
    }
}