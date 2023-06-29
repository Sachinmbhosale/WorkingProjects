/* ==============================================================================================   
Author     : Robbin Thomas
Create date: 
=================================================================================================   
** Change History   
** Date:        Author:             Issue ID        Description:  
** ----------   -------------       ----------      ----------------------------
** 03 Mar 2015  Yogeesha Naik                       Corrected log messages (user friendly)       
** 02 June 2015 Sharath              DMS5-4332      A completed workflow should not be editable or reopened once last stage has Closed/Rejected.
** 15 June 2015 Sharath              DMS5-4368      The notification mails need Work Item ID also to be displayed.
=================================================================================================  */

using System;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace WorkflowNotifications
{
    public partial class frmWorkFlowNotification : Form
    {
        public frmWorkFlowNotification()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Logger.Trace("Timer tick started..", "Info");
            string strNotificationMailSQL = string.Empty;

            WorkflowNotificationData WFNotification = new WorkflowNotificationData();
            try
            {
                DBResult notificationResult = new DBResult();


                notificationResult = GetNotificationData();


                if (notificationResult.ErrorState == 0)
                {
                    //Data set details:
                    //0	Escalation & Reminder Notification 
                    //1	ProcessOwners
                    //2	WorkflowOwners
                    //3	StageOwners
                    //4	StageUsers
                    //5	NotificationUsers
                    //6	Orgs
                    //7 data creation notification

                    DataTable dtNotifications = notificationResult.dsResult.Tables[0];
                    DataTable dtProcessOwners = notificationResult.dsResult.Tables[1];
                    DataTable dtWorkflowOwners = notificationResult.dsResult.Tables[2];
                    DataTable dtStageOwners = notificationResult.dsResult.Tables[3];
                    DataTable dtStageUsers = notificationResult.dsResult.Tables[4];
                    DataTable dtNotificationUsers = notificationResult.dsResult.Tables[5];
                    DataTable dtOrgs = notificationResult.dsResult.Tables[6];

                    string strTo = string.Empty;
                    string strFrom = string.Empty;
                    string strCC = string.Empty;
                    string strBCC = string.Empty;

                    //send escalatios & reminders                    
                    if (notificationResult.dsResult != null
                        && notificationResult.dsResult.Tables.Count > 0
                        && notificationResult.dsResult.Tables[0].Rows.Count > 0)
                    {
                        Logger.Trace("Sending escalation and reminders.", "Info");

                        foreach (DataRow dr in notificationResult.dsResult.Tables[0].Rows)
                        {
                            strFrom = GetMailFromDetails(dtOrgs, dr);
                            strTo = GetMailToDetails(dtStageUsers, dr);
                            if (strTo.Trim() != "")
                            {
                                strCC = GetMailCCDetails(dtProcessOwners, dtWorkflowOwners, dtStageOwners, dtNotificationUsers, dr);
                                strNotificationMailSQL = SendMessage(dr, strFrom, strTo, strCC, strBCC, "");

                                SaveNotificationData(strNotificationMailSQL);
                                Logger.Trace("Saved notification data..", "Success");
                            }
                        }


                    }
                    else
                    {
                        Logger.Trace("No data to process." + DateTime.Now.ToString(), "Info");
                    }


                    //send data notification
                    if (notificationResult.dsResult != null
                        && notificationResult.dsResult.Tables.Count > 6
                        && notificationResult.dsResult.Tables[7].Rows.Count > 0)
                    {
                        Logger.Trace("Sending data notifications.", "Info");

                        strNotificationMailSQL = string.Empty;
                        string strNotificationUpdateSQL = string.Empty;

                        foreach (DataRow dr in notificationResult.dsResult.Tables[7].Rows)
                        {
                            strFrom = GetMailFromDetails(dtOrgs, dr);
                            strTo = GetMailToDetails(dtStageUsers, dr);
                            if (strTo.Trim() != "")
                            {
                                strNotificationMailSQL = SendMessage(dr, strFrom, strTo, strCC, strBCC, "Notification");
                                SaveNotificationData(strNotificationMailSQL);
                                Logger.Trace("Saved notification data..", "Success");

                                string FieldDataID = dr["WorkflowStageFieldData_iID"].ToString();

                                strNotificationUpdateSQL = " UPDATE WorkflowStageFieldData SET WorkflowStageFieldData_bNotificationSent = 1 WHERE WorkflowStageFieldData_iID = " + FieldDataID;
                                SaveNotificationData(strNotificationUpdateSQL);
                                Logger.Trace("Updated notification data..", "Success");
                            }
                        }


                    }
                    else
                    {
                        Logger.Trace("No data to process." + DateTime.Now.ToString(), "Info");
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: "+ex.Message, "Error");
            }
            finally
            {
                timer1.Enabled = true;
                Logger.Trace("Timer enabled", "Info");
            }
        }

        private string GetMailFromDetails(DataTable dtOrgs, DataRow currentRow)
        {
            Logger.Trace("Getting mail 'From' address..", "Info");
            string strFrom = string.Empty;
            string currentORGId = currentRow["WorkflowStageFieldData_iOrgID"].ToString();
            dtOrgs.DefaultView.RowFilter = "ORGS_iId=1";
            if (dtOrgs.DefaultView.Count > 0)
            {
                strFrom = dtOrgs.DefaultView[0]["ORGS_vEmailId"].ToString();
                
            }
            Logger.Trace("From address : " + strFrom, "Info");
            return strFrom;
        }

        private string GetMailToDetails(DataTable dtStageUsers, DataRow currentRow)
        {
            Logger.Trace("Getting mail 'To' address..", "Info");
            string strTo = string.Empty;
            string currentStageId = currentRow["WorkflowStageFieldData_iStageID"].ToString();
            dtStageUsers.DefaultView.RowFilter = "WorkflowUserMapping_iStageId=" + currentStageId;

            if (dtStageUsers.DefaultView.Count > 0)
            {
                DataTable dtTemp = dtStageUsers.DefaultView.ToTable();

                foreach (DataRow dr in dtTemp.Rows)
                {

                    string startdateTemp = dr["USERS_dOutofOfficeStartDate"].ToString();
                    string enddateTemp = dr["USERS_dOutofOfficeEndDate"].ToString();
                    string currDateTimeTemp = dr["DBCurrentDate"].ToString();

                    if (!IsUserOutOfOffice(startdateTemp, enddateTemp, currDateTimeTemp))
                    {
                        strTo += dr["USERS_vEmailId"].ToString() + ";";
                    }

                }
            }

            Logger.Trace("Mail 'To' address : " + strTo, "Info");
            return strTo;
        }

        private bool IsUserOutOfOffice(string startdateTemp, string enddateTemp, string currDateTimeTemp)
        {
            string startdate = string.Empty;
            string enddate = string.Empty;
            string currDateTime = string.Empty;

            try
            {
                if (startdateTemp != "")
                {
                    string stDateVal = startdateTemp.Replace(":", "/").Replace(" ", "/");
                    string dayfield = stDateVal.Split('/')[0];
                    string monthfield = stDateVal.Split('/')[1];
                    string yearfield = stDateVal.Split('/')[2];
                    string hourfield = stDateVal.Split('/')[3];
                    string minfield = stDateVal.Split('/')[4];

                    startdate = yearfield + monthfield + dayfield + hourfield + minfield;
                }
            }
            catch
            {
                startdate = "";
            }

            try
            {
                if (enddateTemp != "")
                {
                    string stDateVal = enddateTemp.Replace(":", "/").Replace(" ", "/");
                    string dayfield = stDateVal.Split('/')[0];
                    string monthfield = stDateVal.Split('/')[1];
                    string yearfield = stDateVal.Split('/')[2];
                    string hourfield = stDateVal.Split('/')[3];
                    string minfield = stDateVal.Split('/')[4];

                    enddate = yearfield + monthfield + dayfield + hourfield + minfield;
                }
            }
            catch 
            {
                enddate = "";
            }

            try
            {
                if (currDateTimeTemp != "")
                {
                    string stDateVal = currDateTimeTemp.Replace(":", "/").Replace(" ", "/").Replace("-", "/");
                    string dayfield = stDateVal.Split('/')[0];
                    string monthfield = stDateVal.Split('/')[1];
                    string yearfield = stDateVal.Split('/')[2];
                    string hourfield = stDateVal.Split('/')[3];
                    string minfield = stDateVal.Split('/')[4];

                    currDateTime = yearfield + monthfield + dayfield + hourfield + minfield;
                }
            }
            catch 
            {
                startdate = "";
            }

            if ((startdate != "") && (enddate != ""))
            {
                if ((Convert.ToInt64(startdate) <= Convert.ToInt64(currDateTime)) && (Convert.ToInt64(enddate) >= Convert.ToInt64(currDateTime)))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetMailCCDetails(DataTable dtProcessOwners, DataTable dtWorkflowOwners, DataTable dtStageOwners, DataTable dtNotificationUsers, DataRow currentRow)
        {
            string strCC = string.Empty;
            Logger.Trace("Getting mail 'CC' address..", "Info");
            string currentStageId = currentRow["WorkflowStageFieldData_iStageID"].ToString();
            dtNotificationUsers.DefaultView.RowFilter = "WorkflowUserMapping_iStageId=" + currentStageId;
            if (dtNotificationUsers.DefaultView.Count > 0)
            {
                DataTable dtTemp = dtNotificationUsers.DefaultView.ToTable();
                foreach (DataRow dr in dtTemp.Rows)
                {
                    string startdateTemp = dr["USERS_dOutofOfficeStartDate"].ToString();
                    string enddateTemp = dr["USERS_dOutofOfficeEndDate"].ToString();
                    string currDateTimeTemp = dr["DBCurrentDate"].ToString();

                    if (!IsUserOutOfOffice(startdateTemp, enddateTemp, currDateTimeTemp))
                    {
                        strCC += dr["USERS_vEmailId"].ToString() + ";";
                    }
                }
            }

            string currentWorkflowId = currentRow["WorkflowStageFieldData_iWorkFlowID"].ToString();
            dtNotificationUsers.DefaultView.RowFilter = "WorkflowUserMapping_iWorkflowId=" + currentWorkflowId;
            if (dtNotificationUsers.DefaultView.Count > 0)
            {
                DataTable dtTemp = dtNotificationUsers.DefaultView.ToTable();
                foreach (DataRow dr in dtTemp.Rows)
                {
                    string startdateTemp = dr["USERS_dOutofOfficeStartDate"].ToString();
                    string enddateTemp = dr["USERS_dOutofOfficeEndDate"].ToString();
                    string currDateTimeTemp = dr["DBCurrentDate"].ToString();

                    if (!IsUserOutOfOffice(startdateTemp, enddateTemp, currDateTimeTemp))
                    {
                        strCC += dr["USERS_vEmailId"].ToString() + ";";
                    }
                }
            }

            string NotificationCategory = currentRow["Notification_vCategory"].ToString();

            if (NotificationCategory == "Escalation")
            {
                string currentEscWorkflowId = currentRow["WorkflowStageFieldData_iWorkFlowID"].ToString();
                dtWorkflowOwners.DefaultView.RowFilter = "WorkflowUserMapping_iWorkflowId=" + currentEscWorkflowId;
                if (dtWorkflowOwners.DefaultView.Count > 0)
                {
                    DataTable dtTemp = dtWorkflowOwners.DefaultView.ToTable();
                    foreach (DataRow dr in dtTemp.Rows)
                    {
                        string startdateTemp = dr["USERS_dOutofOfficeStartDate"].ToString();
                        string enddateTemp = dr["USERS_dOutofOfficeEndDate"].ToString();
                        string currDateTimeTemp = dr["DBCurrentDate"].ToString();

                        if (!IsUserOutOfOffice(startdateTemp, enddateTemp, currDateTimeTemp))
                        {
                            strCC += dr["USERS_vEmailId"].ToString() + ";";
                        }
                    }
                }

                string currentEscProcessId = currentRow["WorkflowStageFieldData_iProcessID"].ToString();
                dtProcessOwners.DefaultView.RowFilter = "WorkflowUserMapping_iProcessId=" + currentEscProcessId;
                if (dtProcessOwners.DefaultView.Count > 0)
                {
                    DataTable dtTemp = dtProcessOwners.DefaultView.ToTable();
                    foreach (DataRow dr in dtTemp.Rows)
                    {
                        string startdateTemp = dr["USERS_dOutofOfficeStartDate"].ToString();
                        string enddateTemp = dr["USERS_dOutofOfficeEndDate"].ToString();
                        string currDateTimeTemp = dr["DBCurrentDate"].ToString();

                        if (!IsUserOutOfOffice(startdateTemp, enddateTemp, currDateTimeTemp))
                        {
                            strCC += dr["USERS_vEmailId"].ToString() + ";";
                        }
                    }
                }

            }

            Logger.Trace("Mail 'CC' address : " + strCC, "Info");
            return strCC;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public string SendMessage(DataRow drMailData, string strFrom, string strTo, string strCC, string strBCC, string strAction)
        {
            Logger.Trace("Sending email started..", "Info");
            string action = string.Empty;
            bool status = false;
            string strSendMailSQL = string.Empty;
            StreamReader reader = null;
            string readFile = string.Empty;
            string messageBody = "";
            int bMailSent = 0;
            try
            {
                if (strAction == "")
                {
                    action = drMailData["Notification_vCategory"].ToString();
                }
                else
                {
                    action = strAction;
                }

                string OrgId = drMailData["WorkflowStageFieldData_iOrgID"].ToString();
                string ProcessID = drMailData["WorkflowStageFieldData_iProcessID"].ToString();
                string WorkFlowID = drMailData["WorkflowStageFieldData_iWorkFlowID"].ToString();
                string StageID = drMailData["WorkflowStageFieldData_iStageID"].ToString();
                string StatusID = drMailData["WorkflowStageFieldData_iStatusID"].ToString();
                string FieldDataID = drMailData["WorkflowStageFieldData_iID"].ToString();
                string NotificationId = "0";
                if (strAction == "")
                {
                    NotificationId = drMailData["Notification_iId"].ToString();
                }

                string currentOrgId = drMailData["WorkflowStageFieldData_iOrgID"].ToString();



                string OriginFileName = drMailData["WorkflowStageFieldData_vOriginFileName"].ToString();
                string WorkflowProcessName = drMailData["WorkflowProcess_vName"].ToString();
                string WorkflowName = drMailData["Workflow_vName"].ToString();
                string WorkflowStageName = drMailData["WorkflowStage_vDisplayName"].ToString();
                string WorkflowStageTATDuration = drMailData["WorkflowStage_iTATDuration"].ToString();

                string NotificationCategory = strAction;
                if (strAction == "")
                {
                    NotificationCategory = drMailData["Notification_vCategory"].ToString();
                }

                string strSubject = string.Empty;

                if (action == "Escalation") //TAT breach
                {
                    reader = new StreamReader("MailFormat_Escalation.htm");
                    strSubject = "Workflow - Workitem TAT breach";
                }
                else if (action == "Reminder") //task reminder
                {
                    reader = new StreamReader("MailFormat_Reminder.htm");
                    strSubject = "Workflow - Workitem task reminder";
                    //DMS5-4332
                    strCC = string.Empty; // For reminder maill CC's not required.
                }
                else if (action == "Notification") //task Notification
                {
                    reader = new StreamReader("MailFormat_Notification.htm");
                    strSubject = "Workflow - New workitem has been added";
                }

                readFile = reader.ReadToEnd();
                messageBody = "";
                messageBody = readFile;
                //messageBody = messageBody.Replace("$$UserName$$", UserName);
                //messageBody = messageBody.Replace("$$OrgEmail$$", OrgEmailId);
                //messageBody = messageBody.Replace("$$OrgName$$", LoginOrgName);
                //messageBody = messageBody.Replace("$$FirstName$$", FirstName);
                //messageBody = messageBody.Replace("$$LastName$$", LastName);
                messageBody = messageBody.Replace("$$WorkItemID$$", FieldDataID);//DMS5-4368
                messageBody = messageBody.Replace("$$ProcessName$$", WorkflowProcessName);
                messageBody = messageBody.Replace("$$WorkflowName$$", WorkflowName);
                messageBody = messageBody.Replace("$$StageName$$", WorkflowStageName);
                messageBody = messageBody.Replace("$$DocumentName$$", OriginFileName);
                messageBody = messageBody.Replace("$$Link$$", "");
                messageBody = messageBody.Replace("$$OrgName$$", "");

                messageBody = messageBody.ToString();

                try
                {
                    Logger.Trace("Send email with subject: " + strSubject, "Info");
                    MailHelper.SendMailMessage(strFrom, strTo, strCC, strBCC, strSubject, messageBody);
                    status = true;
                    bMailSent = 1;
                    Logger.Trace("Sending email finished.", "Success");
                }
                catch (Exception ex)
                {
                    Logger.Trace("Exception: "+ex.Message, "Error");
                    Logger.TraceErrorLog(ex.Message.ToString());
                    bMailSent = 0;
                }

                strSendMailSQL = "INSERT INTO [dbo].[WorkflowNotificationHistory] " +
                       "([NotificationHistory_iOrgId] " +
                       ",[NotificationHistory_iProcessId] " +
                       ",[NotificationHistory_iWorkflowId] " +
                       ",[NotificationHistory_iStageId] " +
                       ",[NotificationHistory_iStatusId] " +
                       ",[NotificationHistory_iNotificationId] " +
                       ",[NotificationHistory_iFieldDataId] " +
                       ",[NotificationHistory_vFrom] " +
                       ",[NotificationHistory_vTo] " +
                       ",[NotificationHistory_vCC] " +
                       ",[NotificationHistory_vBCC] " +
                       ",[NotificationHistory_vMailBody] " +
                       ",[NotificationHistory_vMailSubject] " +
                       ",[NotificationHistory_iCreatedBy] " +
                       ",[NotificationHistory_dCreatedDateTime] " +
                       ",[NotificationHistory_bSent]) " +
                 " VALUES  (" + OrgId + "," +
                 ProcessID + "," +
                 WorkFlowID + "," +
                 StageID + "," +
                 StatusID + "," +
                 NotificationId + "," +
                 FieldDataID + "," +
                 "'" + strFrom + "'," +
                 "'" + strTo + "'," +
                 "'" + strCC + "'," +
                 "'" + strBCC + "'," +
                 "'" + messageBody + "'," +
                 "'" + strSubject + "'," +
                 "1," +
                 "getdate()," +
                 bMailSent + ")";

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, "Error");
                Logger.TraceErrorLog(ex.Message.ToString());
                status = false;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            Logger.Trace("Sending email completed.", "Info");
            return strSendMailSQL;
        }

        private DBResult SaveNotificationData(string strNotificationMailSQL)
        {
            Logger.Trace("Saving notifications data started..", "Info");
            DBResult objresult = new DBResult();
            WorkflowNotificationData objNotification = new WorkflowNotificationData();
            if (strNotificationMailSQL != "")
            {
                objNotification.WorkflowNotificationData_NotificationQuery = strNotificationMailSQL;
                objresult = objNotification.ManageNotificationData(objNotification, "SaveNotificationData");
            }
            Logger.Trace("Saving notifications data completed.", "Info");
            return objresult;
        }

        private DBResult GetNotificationData()
        {
            Logger.Trace("Getting notifications data started..", "Info");
            DBResult objresult = new DBResult();

            WorkflowNotificationData objNotification = new WorkflowNotificationData();
            
            objresult = objNotification.ManageNotificationData(objNotification, "GetNotificationData");
            Logger.Trace("Getting notifications data completed.", "Info");
            return objresult;
        }

        private void rtbResponse_TextChanged(object sender, EventArgs e)
        {
            rtbResponse.SelectionStart = rtbResponse.Text.Length; //Set the current caret position at the end
            rtbResponse.ScrollToCaret(); //Now scroll it automatically
        }
    }
}
