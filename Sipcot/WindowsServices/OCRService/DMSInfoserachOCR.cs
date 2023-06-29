using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Configuration;
using System.Threading;
using System.Timers;
using System.IO;

namespace OCRService
{
    partial class DMSInfoserachOCR : ServiceBase
    {
        // Flag to debug : Only for developer for debugging purpose
        #region parameter declaration and initialization
        static bool debug = false;

        static string ApplicationName = "Writer DMS InfoSearch OCR Serivce";
        static string ConnectionString = ConfigurationManager.AppSettings["DMSInfoSearch_ConnectionStringKey"].ToString();
        static int maxThreads = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfThreads"].ToString());
        private static BackgroundWorker[] threadArray = new BackgroundWorker[maxThreads];
        static int _numberBackGroundThreads;
        static string spParametersList;

        static string WorkFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\workfolder\";

        static DataSet dsDocumentDetails = new DataSet();
        static DataSet Resultds = new DataSet();
        public static System.Timers.Timer timer1;
        #endregion

        public DMSInfoserachOCR()
        {
            Logger.Trace("Service Intializing", ApplicationName);
            InitializeComponent();

            if (!Directory.Exists(WorkFolder))
            {
                Logger.Trace("Creating workfolder in service execution folder. Workfolder path: " + WorkFolder, ApplicationName);
                Directory.CreateDirectory(WorkFolder);
            }
        }

        protected override void OnStart(string[] args)
        {
            #region debug service in developer mode
            if (debug) // Only for developer for debugging purpose
            {
                Logger.Trace("Debugging service in developer mode. Set debug value to false.", ApplicationName);
#if DEBUG
                base.RequestAdditionalTime(100000); // 600*1000ms = 10 minutes timeout
                Debugger.Launch(); // launch and attach debugger
#endif

                GetDocumentsToPerformOCR(null, null);
                PerformOCR(0);
            }
            #endregion

            base.OnStart(args);
            Logger.Trace("Service Started", ApplicationName);
            int Stimer = Convert.ToInt32(ConfigurationManager.AppSettings["TimerValue"].ToString());
            InitializeThread();
            timer1 = new System.Timers.Timer(Stimer);
            timer1.Enabled = true;
            timer1.Elapsed += new ElapsedEventHandler(GetDocumentsToPerformOCR);
            timer1.Start();
            Logger.Trace("Service timer started with the value " + Stimer, ApplicationName);
        }

        protected override void OnStop()
        {
            Logger.Trace("Service Stopped", ApplicationName);
        }

        /// <summary>
        /// Function used to intialize the Threads as per the app.config value
        /// </summary>
        public void InitializeThread()
        {
            Logger.Trace("Initialize Thread Started. Thread Count :" + maxThreads.ToString(), ApplicationName);
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

            Logger.Trace("Initialize Thread Finished", ApplicationName);
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
            PerformOCR(myProcessArguments);
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
            Logger.Trace("service continues to work", ApplicationName);
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
        /// Get all document based on OCR tags from Upload table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void GetDocumentsToPerformOCR(object sender, ElapsedEventArgs e)
        {

            Logger.Trace("Started GetDocumentsToPerformOCR", ApplicationName);
            try
            {
                dsDocumentDetails = DatabaseUpdate("USP_OCRService_ManageOCR", "GetDocumentDetailsForOCR", string.Empty, string.Empty, string.Empty, string.Empty);
                if (dsDocumentDetails != null && dsDocumentDetails.Tables.Count > 0 && dsDocumentDetails.Tables[0].Rows.Count > 0)
                {
                    Logger.Trace("Received Result From SQL to Process: with total records count is " + dsDocumentDetails.Tables[0].Rows.Count, ApplicationName);
                    if (!debug) // Do not calll thread when it is debug mode
                        CallThreads(dsDocumentDetails.Tables[0].Rows.Count);
                }
                else
                {
                    Logger.Trace("No Files Data Received From SQL to Process", ApplicationName);
                }
                Logger.Trace("Completed GetDocumentsToPerformOCR", ApplicationName);
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, ApplicationName);
            }
        }

        /// <summary>
        /// Save into ocr table after creating OCR
        /// </summary>
        private static void PerformOCR(int recCouter)
        {
            Logger.Trace("Started PerformOCR with the record counter value: " + recCouter, ApplicationName);

            if (!debug) // Do not stop timer when it is debug mode
                timer1.Stop();
            Logger.Trace("Timer stopped.", ApplicationName);

            string rData = string.Empty,
            SplittedDocPath = string.Empty,
            OCRContent = string.Empty,
            DocumentId = string.Empty,
            Exception = string.Empty;

            DataSet Resultds = new DataSet();

            try
            {
                DocumentId = dsDocumentDetails.Tables[0].Rows[recCouter]["DocumentId"].ToString();
                SplittedDocPath = beforedot(dsDocumentDetails.Tables[0].Rows[recCouter]["DocumentPath"].ToString());
                Logger.Trace("Document Id: " + DocumentId, ApplicationName);
                Logger.Trace("Splitted files path: " + SplittedDocPath, ApplicationName);

                if (Directory.Exists(SplittedDocPath))
                {
                    DirectoryInfo di = new DirectoryInfo(SplittedDocPath);
                    FileInfo[] files = di.GetFiles("*.pdf");
                    int count = files != null ? files.Length : 0;

                    Logger.Trace("Total files available in splitted files path is: " + count, ApplicationName);

                    if (count > 0)
                    {
                        // Sort by number
                        Logger.Trace("Sort files array by number.", ApplicationName);
                        Array.Sort(files, new MyCustomComparer());

                        #region Looping through each file from splitted file path
                        Logger.Trace("Looping through each file from splitted file path: Iteration count:" + count, ApplicationName);
                        foreach (FileInfo file in files)
                        {
                            try
                            {
                                string PageNo = Path.GetFileNameWithoutExtension(file.FullName);
                                Logger.Trace("Current file path:" + file.FullName, ApplicationName);

                                string targetPath = WorkFolder + @"\" + Path.GetFileNameWithoutExtension(file.FullName) + ".jpg";
                                Logger.Trace("Output image path (JPG): " + targetPath, ApplicationName);

                                Logger.Trace("Calling Pdf2ImageConverter().ConvertPdf2Jpeg() method to convert pdf to jpeg.", ApplicationName);
                                string outputImagPath = new Pdf2ImageConverter().ConvertPdf2Jpeg(file.FullName, targetPath);
                                Logger.Trace("Output image path (JPG): " + outputImagPath, ApplicationName);

                                Logger.Trace("Calling OCR method to get file content. Make sure 'tessdata folder with data' is available in service executable folder.", ApplicationName);
                                OCRContent = new OCRHelper().DoOCR(outputImagPath);

                                //Replace |,[,] with white space as service will not support.
                                Logger.Trace("Replacing |,[,] with white space as service will not support." + OCRContent.Length, ApplicationName);
                                OCRContent = OCRContent.Replace("|", " ").Replace("[", " ").Replace("]", " ");

                                if (OCRContent.Length > 0)
                                {
                                    Logger.Trace("Document OCR content length: " + OCRContent.Length, ApplicationName);
                                    // Save OCR content to database
                                    Resultds = DatabaseUpdate("USP_OCRService_ManageOCR", "SaveDocumentOCRContent", DocumentId, PageNo, OCRContent, Exception);
                                    if (Resultds != null && Resultds.Tables.Count > 0 && Resultds.Tables[0].Rows.Count > 0)
                                    {
                                        Logger.Trace("OCR Data Saved Successfully OCR for the image: " + file.FullName + ". Status:" + Resultds.Tables[0].Rows[0][0], ApplicationName);
                                    }
                                    else
                                    {
                                        Logger.Trace("OCR Saving Failed file Path:" + file.FullName, ApplicationName);
                                    }
                                }
                                else
                                {
                                    Logger.Trace("Document OCR content length: " + OCRContent.Length, ApplicationName);
                                }
                            }
                            catch (Exception ex)
                            {
                                GC.Collect();
                                Logger.Trace("Exception:" + ex.Message, ApplicationName);
                            }
                            finally
                            {
                                GC.Collect();
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    Logger.Trace("Directory path doen't exist: " + SplittedDocPath, ApplicationName);
                    //Update database as document OCR failed
                    DatabaseUpdate("USP_OCRService_ManageOCR", "DocumentOCRFailed", DocumentId, string.Empty, OCRContent, "Directory path doen't exist: " + SplittedDocPath);
                }

                Logger.Trace("Clearing workfolder. Path: " + WorkFolder, ApplicationName);
                Array.ForEach(Directory.GetFiles(WorkFolder), delegate(string path) { File.Delete(path); });
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, ApplicationName);
                //Update database as document OCR failed
                DatabaseUpdate("USP_OCRService_ManageOCR", "DocumentOCRFailed", DocumentId, string.Empty, OCRContent, ex.Message.ToString());
            }

            if (!debug) // Do not start timer when it is debug mode
            {
                timer1.Start();
                Logger.Trace("Timer started again.", ApplicationName);
            }
        }

        public static DataSet DatabaseUpdate(string ProcedureName, string Action, string DocumentId, string PageNo, string OCRContent, string Exception)
        {
            string rData = string.Empty;

            try
            {
                //Create a array list to store parameter(s) with
                spParametersList = string.Empty;
                spArgumentsCollection("@in_vAction", Action, "varchar");
                spArgumentsCollection("@in_iDocumentId", DocumentId, "varchar");
                spArgumentsCollection("@in_iPageNo", PageNo, "varchar");
                spArgumentsCollection("@in_tOCRContent", OCRContent, "varchar");
                spArgumentsCollection("@in_vException", Exception, "varchar");
                spArgumentsCollection("@out_vMessage", string.Empty, "varchar");
                spArgumentsCollection("@out_iErrorState", "0", "int");
                spArgumentsCollection("@out_iErrorSeverity", "0", "int");

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


            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, ApplicationName);
            }
            return Resultds;
        }

        private static string beforedot(string s)
        {
            int l = s.LastIndexOf(".");
            return s.Substring(0, l);
        }

        public static string spArgumentsCollection(string spParmName, string spParmValue, string spPramValueType)
        {
            spParametersList += "[" + spParmName + "|" + spParmValue + "|" + spPramValueType + "]";
            return spParametersList;
        }
    }

    public class MyCustomComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            // split filename
            string[] parts1 = x.Name.Split('-');
            string[] parts2 = y.Name.Split('-');

            // calculate how much leading zeros we need
            int toPad1 = 10 - parts1[0].Length;
            int toPad2 = 10 - parts2[0].Length;

            // add the zeros, only for sorting
            parts1[0] = parts1[0].Insert(0, new String('0', toPad1));
            parts2[0] = parts2[0].Insert(0, new String('0', toPad2));

            // create the comparable string
            string toCompare1 = string.Join("", parts1);
            string toCompare2 = string.Join("", parts2);

            // compare
            return toCompare1.CompareTo(toCompare2);
        }
    }

}
