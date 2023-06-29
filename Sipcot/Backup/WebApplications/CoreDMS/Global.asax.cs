using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.IO;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBL;
using Lotex.EnterpriseSolutions.CoreBE;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            Logger.Trace("Session End", "0");           
            int userId=0;
            int orgId = 0;
            List<string>DataDetails=null;
            int processId=0;
            DataSet dsData = new DataSet();
            UploadDocBL objUploadDocBL = new UploadDocBL();
            string token = string.Empty;
            string message = string.Empty;
            int ErrorState = 0;
            int ErrorSeverity = 0;
            Results rs = new Results();
            userId = Convert.ToInt32(Session["LoggedUserId"]);
                DataDetails = (List<string>)Session["DocumentData"];
                rs.UserData = (CoreBE.UserBase)Session["LoggedUser"];
                if (rs.UserData!=null)
                {
                    token = rs.UserData.LoginToken;
                }
                
                orgId = Convert.ToInt32(Session["OrgID"]);
                if (DataDetails != null)
                {
                    processId = Convert.ToInt32(DataDetails[3]);
                    if (processId!=0)
                    {
                        dsData = objUploadDocBL.UpdateDocumentStatusForLock("UnLock Document", processId, userId, token, orgId, out message, out ErrorState, out ErrorSeverity);
                    }
                   
                }
        }

        protected void Application_End(object sender, EventArgs e)
        {
            try
            {
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                if (Directory.Exists(TempFolder))
                {
                    Directory.Delete(TempFolder, true);
                }
            }
            catch
            { 
            
            }
        }
    }
}