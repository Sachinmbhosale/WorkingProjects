using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for ASPNET2ConfigurationState
/// </summary>
public class USP_ParameterDetails : ConfigurationElement
{
    [ConfigurationProperty("name", IsRequired=true)]
    public string Name
    {
        get
        {
            return this["name"] as string;
        }
    }


    [ConfigurationProperty("datatype", IsRequired = false)]
    public string datatype
    {
        get
        {
            return this["datatype"] as string;
        }
    }
}
