using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

/// <summary>
/// Summary description for ASPNET2ConfigurationSection
/// </summary>
public class USP_Name : ConfigurationSection
{
    /// <summary>
    /// Returns an ASPNET2Configuration instance
    /// </summary>
    public static USP_Name GetConfig(string strServiceName)
    {
        return ConfigurationManager.GetSection("SPNAMES/" + strServiceName) as USP_Name;
    }


    [ConfigurationProperty("SPNAME",IsRequired=false)]
    public string SPNAME
    {
        get
        {
            return this["SPNAME"] as string;
        }
    }

    [ConfigurationProperty("SPReturnType", IsRequired=true)]
    public int SPReturnType
    {
        get
        {
            return (int) this["SPReturnType"];
        }
    }

    [ConfigurationProperty("Parameters")]
    public USP_ParameterList Parameters
    {
        get
        {
            return this["Parameters"] as USP_ParameterList;
        }
    } 
}
