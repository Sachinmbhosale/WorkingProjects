using System.Configuration;

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
