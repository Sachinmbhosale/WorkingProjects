using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ionic.Zip;
using System.IO;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Web.Configuration;


public static class ConfigHelper
{
    static Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
    static AppSettingsSection appSettingsSection = (AppSettingsSection)configuration.GetSection("appSettings");
    static ConnectionStringsSection connStringSection = (ConnectionStringsSection)configuration.GetSection("connectionStrings");

    public static string ReadAppSetting(string key)
    {
        string configValue = string.Empty;
        if (appSettingsSection != null)
        {
            configValue = appSettingsSection.Settings[key].Value;
        }
        return configValue;
    }

    public static string ReadConnectionString(string key)
    {
        string configValue = string.Empty;
        if (connStringSection != null)
        {
            configValue = connStringSection.ConnectionStrings[key].ConnectionString;
        }
        return configValue;
    }

    public static void ModifyAppSetting(string key, string value)
    {
        if (appSettingsSection != null)
        {
            appSettingsSection.Settings[key].Value = value;
            configuration.Save();
        }
    }

    public static void ModifyConnectionString(string key, string value)
    {
        if (connStringSection != null)
        {
            connStringSection.ConnectionStrings[key].ConnectionString = value;
            configuration.Save();
        }
    }

    public static void RemoveAppSetting(string key)
    {
        if (appSettingsSection != null)
        {
            appSettingsSection.Settings.Remove(key);
            configuration.Save();
        }
    }

    //var configuration = WebConfigurationManager.OpenWebConfiguration("~");
    //var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");
    //section.ConnectionStrings["SqlServerConnString"].ConnectionString = ConnectionString;
    //configuration.Save();

}
