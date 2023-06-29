using System;
using System.IO;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing;
//using System.ServiceModel.Web;

namespace WorkflowNotifications
{
    public static class Logger
    {
        //private static string ProcessResponce(object RequestObj, WebOperationContext objWebOperationContext)
        //{
        //    return JsonSerde.GetJson(RequestObj) + ":" + WebContext.GetOutgoingResponseHeaders(objWebOperationContext);
        //}
        //private static string ProcessRequest(object ResponceObj, WebOperationContext objWebOperationContext)
        //{
        //    return JsonSerde.GetJson(ResponceObj) + ":" + WebContext.GetIncomingRequestHeaders(objWebOperationContext);
        //}

        //public static void TraceRequestResponceToDB(string strFOID, string strMethodName, object RequestObj, object ResponceObj, WebOperationContext CurrentWeb)
        //{

        //    //Send To DB
        //    DALTrace.SendTraceToDB(strFOID, strMethodName,DateTime.Now, ProcessRequest(RequestObj, CurrentWeb), DateTime.Now, ProcessResponce(ResponceObj, CurrentWeb), string.Empty);

        //}

        //public static void TraceRequestResponceToDB(string strFOID,string strMethodName,object RequestObj, object ResponceObj, WebOperationContext CurrentWeb,Exception Error)
        //{

        //    //Send To DB
        //    DALTrace.SendTraceToDB(strFOID, strMethodName, DateTime.Now, ProcessRequest(RequestObj, CurrentWeb), DateTime.Now, ProcessResponce(ResponceObj, CurrentWeb), Error.Message.ToString() + " Source : "  +  Error.Source.ToString());

        //}

        public static void Trace(string msgStr, string StrAgentId)
        {
            string strTemp = string.Empty;
            AppSettingsReader appSettings = new AppSettingsReader();
            string TraceRequired = appSettings.GetValue("logTraceIsEnabled", strTemp.GetType()).ToString();

            try
            {
                // Update dashboard
                RichTextBox rtbMessage = (Application.OpenForms["frmWorkFlowNotification"] as frmWorkFlowNotification).rtbResponse;
                if (rtbMessage.TextLength > 10000) rtbMessage.Clear();

                if (StrAgentId.ToUpper() == "SUCCESS")
                    rtbMessage.SelectionColor = Color.Green;
                else if (StrAgentId.ToUpper() == "ERROR")
                    rtbMessage.SelectionColor = Color.Red;
                else
                    rtbMessage.SelectionColor = Color.Black;


                rtbMessage.AppendText(Environment.NewLine + DateTime.Now + " " + msgStr);
                rtbMessage.Refresh();

                if (TraceRequired.ToLower() == "true")
                {
                    string strErrorFile = "";
                    string foldername = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Logs\\" + StrAgentId + "\\";
                    strErrorFile = DateTime.Now.ToString("yyyyMMdd");
                    strErrorFile = foldername + strErrorFile + ".Log";
                    if (File.Exists(foldername) == false)
                    {
                        System.IO.Directory.CreateDirectory(@foldername);
                    }
                    StreamWriter sw;
                    if (File.Exists(strErrorFile))
                    {
                        sw = File.AppendText(strErrorFile);
                    }
                    else
                    {
                        sw = File.CreateText(strErrorFile);
                    }
                    sw.WriteLine(DateTime.Now.ToString() + ": " + msgStr);
                    sw.Close();
                }
            }
            catch { }
        }
        public static void TraceErrorLog(string msgStr)
        {
            string strTemp = string.Empty;
            AppSettingsReader appSettings = new AppSettingsReader();
            string TraceRequired = appSettings.GetValue("logTraceIsEnabled", strTemp.GetType()).ToString();
            if (TraceRequired.ToLower() == "true")
            {
                string strErrorFile = "";
                string foldername = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "ApplicationLog\\";
                strErrorFile = DateTime.Now.ToString("yyyyMMdd");
                strErrorFile = foldername + strErrorFile + ".Log";
                if (File.Exists(foldername) == false)
                {
                    System.IO.Directory.CreateDirectory(@foldername);
                }
                StreamWriter sw;
                if (File.Exists(strErrorFile))
                {
                    sw = File.AppendText(strErrorFile);
                }
                else
                {
                    sw = File.CreateText(strErrorFile);
                }
                sw.WriteLine(DateTime.Now.ToString() + ": " + msgStr);
                sw.Close();
            }

            //sample code
            //public TUConfirmation UpdateUploadMode(UploadMode UM)
            //{
            //    Logger.Trace("Updating the upload mode for the Agent " + WebOperationContext.Current.IncomingRequest.Headers["agentid"].ToString() + " Application Version " + WebOperationContext.Current.IncomingRequest.Headers["AppVer"].ToString(), WebOperationContext.Current.IncomingRequest.Headers["agentid"].ToString());
            //    TUConfirmation retTUConfirmation = new TUConfirmation();
            //    string strFOID = WebOperationContext.Current.IncomingRequest.Headers["agentid"].ToString();
            //    string uniqueid = WebOperationContext.Current.IncomingRequest.Headers["uniqueid"].ToString();
            //    string password = WebOperationContext.Current.IncomingRequest.Headers["password"].ToString();
            //    //string no_of_groups=WebOperationContext.Current.IncomingRequest.Headers["no_of_groups"].ToString();
            //    //string no_of_individuals=WebOperationContext.Current.IncomingRequest.Headers["no_of_individuals"].ToString() ;
            //    //string agentid=WebOperationContext.Current.IncomingRequest.Headers["agentid"].ToString() ;
            //    string strAppVersion = WebOperationContext.Current.IncomingRequest.Headers["AppVer"].ToString();
            //    Logger.Trace("Calling AuthenticateUserForTransaction ", strFOID);
            //    DataSet ds = new DataBase().AuthenticateUserForTransaction(strFOID, uniqueid, password, ConfigurationSettings.AppSettings["TestingModeValue"].ToString(), strAppVersion);
            //    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0][0].ToString().Trim() != "")
            //    {

            //        //DALService.Device dv = new DALService.Device();
            //        //dv.DeviceID = D.DeviceID;


            //        DataBase objDataBase = new DataBase();
            //        retTUConfirmation.agentid = strFOID;
            //        retTUConfirmation.ecode = "";
            //        retTUConfirmation.guid = "";
            //        retTUConfirmation.tuid = "";
            //        Logger.Trace("Calling UpdateUploadMode ", strFOID);
            //        string strStatus = objDataBase.UpdateUploadMode(strFOID, UM.Mode);
            //        if (string.Equals(strStatus.ToLower(), "success"))
            //        {
            //            retTUConfirmation.msg = "success";
            //            Logger.Trace("Updating the upload mode success for the agent - " + strFOID, strFOID);
            //        }
            //        else
            //        {
            //            retTUConfirmation.ecode = "error";
            //            retTUConfirmation.msg = "failure";
            //            Logger.Trace("Updating the upload mode failure for the agent - " + strFOID, strFOID);
            //        }
            //    }
            //    else
            //    {
            //        retTUConfirmation.agentid = strFOID.ToString();
            //        retTUConfirmation.guid = "";
            //        retTUConfirmation.tuid = "";
            //        retTUConfirmation.ecode = "error";
            //        retTUConfirmation.msg = "Credentials does not match";
            //        Logger.Trace("Method [UpdateUploadMode] Returning as Error because of " + retTUConfirmation.msg, strFOID);
            //    }
            //    //retMsg = "{\"msg\":\"Save Successfully Completed\",\"ecode\":\"\",\"scode\":\"0001\",\"uniqueid\":\"\",\"no_of_trans_saved\":\"" + this.transUploads.Count.ToString() + "\",\"agentid\":\"\",\"datetime\":\"" + DateTime.Now.ToString() + "\"}";
            //    return retTUConfirmation;
            //}
        }
    }

}
