using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for ASPNET2ConfigurationStateCollection
/// </summary>
public class USP_ParameterList : ConfigurationElementCollection
{
    public USP_ParameterDetails this[int index]
    {
        get
        {
            return base.BaseGet(index) as USP_ParameterDetails;
        }
        set
        {
            if (base.BaseGet(index) != null)
            {
                base.BaseRemoveAt(index);
            }
            this.BaseAdd(index, value);
        }
    }


    protected override ConfigurationElement CreateNewElement()
    {
        return new USP_ParameterDetails();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
        return ((USP_ParameterDetails)element).Name;
    } 
}
