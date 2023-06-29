using System.Configuration;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
   public class WorkflowBase
    {
        /// <summary>
        /// Constuctor logic
        /// </summary>
       public WorkflowBase()
       {
           LoginToken = "";
           LoginOrgId = 1;
           UserId = 0;
           UserGroupId = 0;
       }
       
        /// <summary>
        /// Default Properties
        /// </summary>
       public string LoginToken { get; set; }
       public int LoginOrgId { get; set; }
       public int UserId { get; set; }
       public int UserGroupId { get; set; }
       
       public string DbConnectionString
       {
           get
           {
               return ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString;
           }
       }

       public DataProvider ConfiguredDataProvider
       {
           get
           {
               string DatabaseSystem = ConfigurationManager.AppSettings["DatabaseSystem"];
               switch (DatabaseSystem.ToUpper())
               {
                   default:
                   case "SQLSERVER":
                       return DataProvider.SqlServer;
                   case "MYSQL":
                       return DataProvider.MySql;
               }
           }
       }

       public string QueryParser(string query, DataProvider dataProvider)
       {
           string output = query.ToLower();
           string DatabaseSystem = ConfigurationManager.AppSettings["DatabaseSystem"];
           switch (DatabaseSystem.ToUpper())
           {
               default:
               case "SQLSERVER":
                   break;
               case "MYSQL":
                   output = output.Replace("dbo.", string.Empty);
                   output = output.Replace("(nolock)", string.Empty);
                   output = output.Replace("@", string.Empty);
                  // output = output.Replace("=", " default ");
                   output = output.Replace("&apos;", "'");
                   output = output.Replace("isnull", "ifnull");
                   output = output.Replace("getdate()", "now()");
                   output = output.Replace("[", "`");
                   output = output.Replace("]", "`");

                   break;
           }
           return output;
       }
    }
}
