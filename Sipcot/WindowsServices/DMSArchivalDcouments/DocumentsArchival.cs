using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using System.Configuration;
using System.IO;
using System.Threading;
using Common;


namespace DMSArchivalDcouments
{
    public partial class DocumentsArchival : ServiceBase
    {
        static int maxThreads = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfThreads"].ToString());
        static string ConnectionString = ConfigurationManager.AppSettings["DMSInfoSearch_ConnectionStringKey"].ToString();
        static string ArchivalPath = ConfigurationManager.AppSettings["ArchivalPath"].ToString();
        private static BackgroundWorker[] threadArray = new BackgroundWorker[maxThreads];
        static int _numberBackGroundThreads;
        static string sSource;
        static string sLog;
        static string sEvent;
        static string spParametersList;
        static DataSet ds = new DataSet();
        static DataSet Resultds = new DataSet();
        public DocumentsArchival()
        {
            InitializeComponent();

            sSource = "Writer DMS InfoSearch Archival Serivce";
            sLog = "Application";
            sEvent = "Service Intializing";

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);


            EventLog.WriteEntry(sSource, sEvent,
                EventLogEntryType.Information, 0); 
        }
        public static System.Timers.Timer timer1;
        protected override void OnStart(string[] args)
        {

            base.OnStart(args);
            sSource = "Writer DMS InfoSearch Serivce";
            sLog = "Application";
            sEvent = "Service Started";
            int Stimer = Convert.ToInt32(ConfigurationManager.AppSettings["TimerValue"].ToString());
            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);


            EventLog.WriteEntry(sSource, sEvent,
                EventLogEntryType.Information, 234);

            InitializeThread();

            sSource = "Writer DMS InfoSearch Serivce";
            sLog = "Application";
            sEvent = "Service Timer Started";

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);


            EventLog.WriteEntry(sSource, sEvent,
                EventLogEntryType.Information, 234);

            // System.Timers.Timer timer1 = new System.Timers.Timer(60000);
            timer1 = new System.Timers.Timer(Stimer);
            timer1.Enabled = true;
            timer1.Elapsed += new ElapsedEventHandler(GetArchivalFiles);
            timer1.Start();
        }
        public static string spArgumentsCollection(string spParmName, string spParmValue, string spPramValueType)
        {
            spParametersList += "[" + spParmName + "|" + spParmValue + "|" + spPramValueType + "]";
            return spParametersList;
        }
        static void GetArchivalFiles(object sender, ElapsedEventArgs e)
        {

            Logger.logTrace("Started GetArchivalFiles", "Logs");
            try
            {
                sSource = "Writer DMS InfoSearch Archival Serivce";
                sLog = "Application";
                sEvent = "Service Started Processing Files";

                if (!EventLog.SourceExists(sSource))
                    EventLog.CreateEventSource(sSource, sLog);



                EventLog.WriteEntry(sSource, sEvent,
                    EventLogEntryType.Information, 0);

             

                string rData = string.Empty;

                //Create a array list to store parameter(s) with
                string Message = string.Empty,
                 ProcedureName = "USP_ManageDocumemntArchival";
                //Create a array list to store parameter(s) with
                spParametersList = string.Empty;
                spArgumentsCollection("@in_vAction", "GetDocumentForArchival", "varchar");
                spArgumentsCollection("@in_iProcessId", string.Empty, "varchar");
                spArgumentsCollection("@in_vPhysicalPath", string.Empty, "varchar");

                Crypt crypt = new Crypt();
                GenService.IService cleintupload = new GenService.ServiceClient();
                GenService.StoredProcedureReturnsDataset contractInfo = new GenService.StoredProcedureReturnsDataset()
                {
                    ConnectionString_Encrypted = crypt.Encrypt(ConnectionString),
                    ProcedureName_Encrypted = crypt.Encrypt(ProcedureName),
                    ParametersList_Encrypted = crypt.Encrypt(spParametersList)
                };
                rData = cleintupload.RunStoredProcedureReturnsDataset(contractInfo);
                ds = DSXML.ConvertXMLToDataSet(rData);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    sSource = "Writer DMS InfoSearch Archival Serivce";
                    sLog = "Application";
                    sEvent = "Received Result From SQL to Process";

                    if (!EventLog.SourceExists(sSource))
                        EventLog.CreateEventSource(sSource, sLog);

            

                    EventLog.WriteEntry(sSource, sEvent,
                        EventLogEntryType.Information, 0);

                    CallThreads(ds.Tables[0].Rows.Count);
                }
                else
                {
                    sSource = "Writer DMS InfoSearch Archival Serivce";
                    sLog = "Application";
                    sEvent = "No Files Data Received From SQL to Process";

                    if (!EventLog.SourceExists(sSource))
                        EventLog.CreateEventSource(sSource, sLog);

                    Logger.logTrace("GetArchivalFiles is null", "Logs");

                    EventLog.WriteEntry(sSource, sEvent,
                        EventLogEntryType.Information, 0);
                }
            }
            catch (Exception ex)
            {
                sSource = "Writer DMS InfoSearch Archival Serivce";
                sLog = "Application";
                sEvent = "thrown Error in service call";

                if (!EventLog.SourceExists(sSource))
                    EventLog.CreateEventSource(sSource, sLog);
                Logger.logException(ex, "ErrorLog");


                EventLog.WriteEntry(sSource, sEvent,
                    EventLogEntryType.Information, 0);
            }
            Logger.logTrace("Completed GetArchivalFiles", "Logs");

        }
        protected override void OnStop()
        {
            sSource = "Writer DMS InfoSearch Archival Serivce";
            sLog = "Application";
            sEvent = "Service Stoped";

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);



            EventLog.WriteEntry(sSource, sEvent,
                EventLogEntryType.Information, 0);
        }

        private static void ArchivingFiles(string processId,string OrginalFilePath,string OrgId,string EncryptedFilename)
        {
            try
            {
                Logger.logTrace("Started ArchivingFiles", "Logs");
                timer1.Stop();

                if (!Directory.Exists(ArchivalPath))
                {
                    Directory.CreateDirectory(ArchivalPath);
                }
                string Filepath = ArchivalPath + "\\" + OrgId + "\\" + DateTime.Now.ToString("ddMMyyyy");

                if (!Directory.Exists(Filepath))
                {
                    Directory.CreateDirectory(ArchivalPath);
                }

                copyDirectory(beforedot(OrginalFilePath), Filepath + "\\" + beforedot(EncryptedFilename));



                File.Copy(beforedot(OrginalFilePath) + ".zip", Filepath + "\\" + beforedot(EncryptedFilename) + ".zip", true);
              //File.Copy(OrginalFilePath, Filepath + "\\" + EncryptedFilename, true);

                string rData = string.Empty;

                //Create a array list to store parameter(s) with
                string Message = string.Empty,
                 ProcedureName = "USP_ManageDocumemntArchival";
                //Create a array list to store parameter(s) with
                spParametersList = string.Empty;
                spArgumentsCollection("@in_vAction", "UpdateDocumentAfterArchival", "varchar");
                spArgumentsCollection("@in_iProcessId", processId, "varchar");
                spArgumentsCollection("@in_vPhysicalPath", Filepath + "\\" + EncryptedFilename, "varchar");

                Crypt crypt = new Crypt();
                GenService.IService cleintupload = new GenService.ServiceClient();
                GenService.StoredProcedureReturnsDataset contractInfo = new GenService.StoredProcedureReturnsDataset()
                {
                    ConnectionString_Encrypted = crypt.Encrypt(ConnectionString),
                    ProcedureName_Encrypted = crypt.Encrypt(ProcedureName),
                    ParametersList_Encrypted = crypt.Encrypt(spParametersList)
                };
                rData = cleintupload.RunStoredProcedureReturnsDataset(contractInfo);

                Resultds = DSXML.ConvertXMLToDataSet(rData);
                if (Resultds != null && Resultds.Tables.Count > 0 && Resultds.Tables[0].Rows.Count > 0)
                {
                    if (Resultds.Tables[0].Rows[0]["AStatus"].ToString() == "Success")
                    {
                        Directory.Delete(beforedot(OrginalFilePath), true);
                        File.Delete(beforedot(OrginalFilePath) + ".zip");
                    }

                }


                Logger.logTrace("Completed ArchivingFiles", "Logs");

                timer1.Start();

            }
            catch (Exception ex)
            {
                Logger.logException(ex, "ErrorLog");
            }
        }
        public static void copyDirectory(string Src, string Dst)
        {
            String[] Files;
            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
                Dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
            Files = Directory.GetFileSystemEntries(Src, "*.pdf");
            foreach (string Element in Files)
            {
                try
                {
                    File.Copy(Element, Dst + Path.GetFileName(Element), true);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
        private static string beforedot(string s)
        {
            int l = s.LastIndexOf(".");
            return s.Substring(0, l);
        }

        private static void CallThreads(int g)
        {

            for (int f = 0; f < g; f++)
            {

                //Use the thread array to process ech iteration
                //choose the first unused thread.
                bool fileProcessed = false;
                while (!fileProcessed)
                {

                    int threadNum = 0;
                    for (threadNum = 0; threadNum < maxThreads; threadNum++)
                    {

                        if (!threadArray[threadNum].IsBusy)
                        {
                            //Call the "RunWorkerAsync()" method of the thread.  
                            //This will call the delegate method "backgroundWorkerFiles_DoWork()" method defined above.  
                            //The parameter passed (the loop counter "f") will be available through the delegate's argument "e" through the ".Argument" property.
                            threadArray[threadNum].RunWorkerAsync(f);
                            fileProcessed = true;
                            break;
                        }
                    }
                    //If all threads are being used, sleep awhile before checking again
                    if (!fileProcessed)
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
        }


        /// <summary>
        /// Function used to intialize the Threads as per the app.config value
        /// </summary>
        public void InitializeThread()
        {

            sSource = "Writer DMS InfoSearch Archival Serivce";
            sLog = "Application";
            sEvent = "Initialize Thread Started. Thread Count : " + maxThreads.ToString();

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);


            EventLog.WriteEntry(sSource, sEvent,
                EventLogEntryType.Information, 234);

            for (int f = 0; f < maxThreads; f++)
            {
                threadArray[f] = new BackgroundWorker();
                threadArray[f].DoWork +=
                    new DoWorkEventHandler(backgroundWorkerFiles_DoWork);
                threadArray[f].RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(backgroundWorkerFiles_RunWorkerCompleted);
                threadArray[f].ProgressChanged +=
                    new ProgressChangedEventHandler(backgroundWorkerFiles_ProgressChanged);
                threadArray[f].WorkerReportsProgress = false;
                threadArray[f].WorkerSupportsCancellation = true;

            }

            sSource = "Writer DMS InfoSearch Archival Serivce";
            sLog = "Application";
            sEvent = "Initialize Thread Finished";

            if (!EventLog.SourceExists(sSource))
                EventLog.CreateEventSource(sSource, sLog);


            EventLog.WriteEntry(sSource, sEvent,
                EventLogEntryType.Information, 234);
        }

        /// <summary>
        /// backgroundWorker dowork -- This function will be doing the main functinalities of backgroundWorker
        /// </summary>
        /// <param name="se"></param>
        /// <param name="e"></param>
        private void backgroundWorkerFiles_DoWork(object se, DoWorkEventArgs e)
        {

            //Just for fun - increment the count of the number of threads we are currently using.  Can show this number in the GUI.
            _numberBackGroundThreads--;

            // Get argument from DoWorkEventArgs argument.  Can use any type here with cast
            int myProcessArguments = (int)e.Argument;

            // "ProcessFile" is the name of my method that does the main work.  Replace with your own method!  
            // Can return reulsts from this method, i.e. a status (OK, FAIL etc)

            ArchivingFiles(ds.Tables[0].Rows[myProcessArguments]["PROCESSID"].ToString(), ds.Tables[0].Rows[myProcessArguments]["FILEPATH"].ToString(), ds.Tables[0].Rows[myProcessArguments]["OrgId"].ToString(), ds.Tables[0].Rows[myProcessArguments]["EncryptedFileName"].ToString());

        }

        /// <summary>
        /// To update the Progress Bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        /// <summary>
        /// backgroundWorker completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerFiles_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _numberBackGroundThreads--;
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("my service is continuing in working");
        }
    }
}
