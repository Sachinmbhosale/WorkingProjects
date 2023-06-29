using System;
using System.ComponentModel;
using System.Data;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Configuration;
using System.IO;
using System.Threading;
using WindowsForm_SERVICE;
using OfficeConverter;
using Ionic.Zip;


namespace WindowsService
{
    public partial class DMSInfoSearchService : ServiceBase
    {
        static string ApplicationName = "Writer DMS InfoSearch Serivce";

        static string ConnectionString = ConfigurationManager.AppSettings["DMSInfoSearch_ConnectionStringKey"].ToString();
        static string UploadDrive = ConfigurationManager.AppSettings["UploadDrive"].ToString();
        static int maxThreads = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfThreads"].ToString());
        static string TempWorkFolder = ConfigurationManager.AppSettings["TempFolder"].ToString();

        private static BackgroundWorker[] threadArray = new BackgroundWorker[maxThreads];
        static int _numberBackGroundThreads;
        //static string sSource;
        //static string sLog;
        //static string sEvent;
        static string spParametersList;
        static DataSet ds = new DataSet();

        public DMSInfoSearchService()
        {
            InitializeComponent();

            Logger.Trace(ApplicationName + " Intializing", ApplicationName);
        }

        public static System.Timers.Timer timer1;
        protected override void OnStart(string[] args)
        {
            //#if DEBUG
            //            base.RequestAdditionalTime(100000); // 600*1000ms = 10 minutes timeout
            //            Debugger.Launch(); // launch and attach debugger
            //#endif

            base.OnStart(args);

            Logger.Trace(ApplicationName + " Started", ApplicationName);

            int Stimer = Convert.ToInt32(ConfigurationManager.AppSettings["TimerValue"].ToString());

            InitializeThread();

            // System.Timers.Timer timer1 = new System.Timers.Timer(60000);
            timer1 = new System.Timers.Timer(Stimer);
            timer1.Enabled = true;
            timer1.Elapsed += new ElapsedEventHandler(ProcessFile);
            timer1.Start();


        }

        protected override void OnStop()
        {
            Logger.Trace(ApplicationName + " Stopped", ApplicationName);
        }

        static void ProcessFile(object sender, ElapsedEventArgs e)
        {
            try
            {
                Logger.Trace("Calling service to get data for splitting.", ApplicationName);

                string Message = string.Empty,
                  ProcedureName = "USP_WindowsService_ManageSplittingfiles";

                string rData = string.Empty;

                //Create a array list to store parameter(s) with
                spParametersList = string.Empty;
                spArgumentsCollection("@in_vAction", "GetAllDataForSplitting", "varchar");
                spArgumentsCollection("@in_iProcessed", "0", "int");
                spArgumentsCollection("@in_vOriginFileName", string.Empty, "varchar");
                spArgumentsCollection("@in_vDocName", string.Empty, "varchar");
                spArgumentsCollection("@in_vDocVirtualPath", string.Empty, "varchar");
                spArgumentsCollection("@in_vDocPhysicalPath", string.Empty, "varchar");
                spArgumentsCollection("@in_vType", string.Empty, "varchar");
                spArgumentsCollection("@in_vBULKUPLOAD_vError", string.Empty, "varchar");

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
                    Logger.Trace("Received data from database. Count : " + ds.Tables[0].Rows.Count, ApplicationName);

                    CallThreads(ds.Tables[0].Rows.Count);
                }
                else
                {
                    Logger.Trace("No data received from database.", ApplicationName);
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, ApplicationName);
            }
        }

        /// <summary>
        /// Function used to intialize the Threads as per the app.config value
        /// </summary>
        public void InitializeThread()
        {
            Logger.Trace("Initializing BackgroundWorker. Thread(s) Count : " + maxThreads, ApplicationName);

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
            ProcessDocument(myProcessArguments);

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

        /// <summary>
        /// Function which will be calling the threads
        /// </summary>
        /// <param name="g"></param>
        private static void CallThreads(int g)
        {
            Logger.Trace("Invoking threads for each record/document.", ApplicationName);
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
        /// Function to Upload File to Server
        /// </summary>
        /// <param name="f"></param>
        /// 
        private static void TrackingError(string processid, string error)
        {
            Logger.Trace("Logging error to database.", ApplicationName);

            string Message = string.Empty,
                       ProcedureName = "USP_WindowsService_ManageSplittingfiles";

            string rData = string.Empty;

            //Create a array list to store parameter(s) with
            spParametersList = string.Empty;
            spArgumentsCollection("@in_vAction", "BatchUpdateError", "varchar");
            spArgumentsCollection("@in_iProcessed", processid, "int");
            spArgumentsCollection("@in_vOriginFileName", string.Empty, "varchar");
            spArgumentsCollection("@in_vDocName", string.Empty, "varchar");
            spArgumentsCollection("@in_vDocVirtualPath", string.Empty, "varchar");
            spArgumentsCollection("@in_vDocPhysicalPath", string.Empty, "varchar");
            spArgumentsCollection("@in_vType", string.Empty, "varchar");
            spArgumentsCollection("@in_vBULKUPLOAD_vError", error, "varchar");

            Crypt crypt = new Crypt();
            GenService.IService cleintupload = new GenService.ServiceClient();
            GenService.StoredProcedureReturnsDataset contractInfo = new GenService.StoredProcedureReturnsDataset()
            {
                ConnectionString_Encrypted = crypt.Encrypt(ConnectionString),
                ProcedureName_Encrypted = crypt.Encrypt(ProcedureName),
                ParametersList_Encrypted = crypt.Encrypt(spParametersList)
            };
            rData = cleintupload.RunStoredProcedureReturnsDataset(contractInfo);

            DataSet DS = DSXML.ConvertXMLToDataSet(rData);
            if (DS != null && DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0]["RESULT"].ToString() == "SUCCESS")
            {
                Logger.Trace("Successfully logged error to database.", ApplicationName);
            }

            else
            {
                Logger.Trace("Failed to log error to database", ApplicationName);
            }

        }

        private static void ProcessDocument(int f)
        {
            Logger.Trace("Processing document. Row number: " + f, ApplicationName);
            timer1.Stop();
            Logger.Trace("Timer Stopped.", ApplicationName);

            try
            {

                // Check drive exists
                if (!Directory.Exists(UploadDrive))
                {
                    Logger.Trace("Drive " + UploadDrive + " not found or inaccessible.", ApplicationName);
                    return;
                }

                #region variable declaration and initialization
                // Declare and initailize parameters
                bool IsDocumentAppend;
                if (ds.Tables[0].Rows[f]["bIsUploaded"].ToString().Trim().Length > 2)
                {
                    IsDocumentAppend = Convert.ToBoolean(ds.Tables[0].Rows[f]["bIsUploaded"].ToString());
                }
                else
                {
                    IsDocumentAppend = Convert.ToBoolean(Convert.ToInt32(ds.Tables[0].Rows[f]["bIsUploaded"].ToString()));
                }

                string FileName = ds.Tables[0].Rows[f]["FileName"].ToString();  //Filename before encrypt
                string FileExtension = ds.Tables[0].Rows[f]["FileType"].ToString().ToLower();
                string FileNameWithExtension = FileName + "." + FileExtension;

                string EncryptedFileName = Encryptdata(ds.Tables[0].Rows[f]["RefID"].ToString());
                string EncryptedFileNameWithExtension = EncryptedFileName + "." + FileExtension;

                string FilePathFromBulkUpload = ds.Tables[0].Rows[f]["Source"].ToString();

                string DocumentSaveLocation = UploadDrive + ds.Tables[0].Rows[f]["NewFileSavePath"].ToString();
                string ExtractFilePath = DocumentSaveLocation + EncryptedFileNameWithExtension;

                string DocVirtualPath = ds.Tables[0].Rows[f]["NewFileSavePath"].ToString();
                string DocPhysicalPath = ExtractFilePath;
                string DocumentUploadedPath = string.Empty;
                int PageCount = 0;


                string Folder = TempWorkFolder + DateTime.Now.ToString("HH:mm:ss tt").ToString().Replace(":", "").Replace(" ", "");

                string UploadedFileTempPath = Folder + "\\" + EncryptedFileNameWithExtension;
                string AppendFileTempPath = Folder + "\\" + FileNameWithExtension;

                string DocType = string.Empty;
                string DBUpdateAction = string.Empty;
                #endregion

                #region Fresh upload: Move documents from bulk uploaded location to Original document storage location (with zip).
                if (!IsDocumentAppend) // New upload
                {
                    DBUpdateAction = "NewFileUpload";

                    Logger.Trace("Fresh upload.", ApplicationName);
                    Logger.Trace("File " + FileNameWithExtension + " is processing.", ApplicationName);

                    if (!Directory.Exists(DocumentSaveLocation))
                        Directory.CreateDirectory(DocumentSaveLocation);

                    Logger.Trace("Copy file. Source : " + FilePathFromBulkUpload + " Destination  : " + ExtractFilePath, ApplicationName);
                    File.Copy(FilePathFromBulkUpload, ExtractFilePath, true);

                    switch (FileExtension.ToLower())
                    {
                        case "tif":
                        case "tiff":


                        case "jpg":
                        case "bmp":
                        case "jpeg":
                        case "png":
                        case "gif":
                        case "giff":
                            PageCount = new Image2Pdf().tiff2PDF(ExtractFilePath, ExtractFilePath.Substring(0, ExtractFilePath.LastIndexOf(".")), true);
                            //Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of Image2Pdf().tiff2PDF " + PageCount, Session["LoggedUserId"].ToString());
                            break;
                        case "pdf":
                            PageCount = new Image2Pdf().ExtractPages(ExtractFilePath, ExtractFilePath.Substring(0, ExtractFilePath.LastIndexOf(".")));
                            //Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of Image2Pdf().ExtractPages " + PageCount, Session["LoggedUserId"].ToString());
                                 break;
                        case "doc":
                        case "docx":
                            PageCount = new MSWord().Convert(ExtractFilePath, ExtractFilePath.Substring(0, ExtractFilePath.LastIndexOf(".")) + ".pdf", true);
                            //Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSWord " + PageCount, Session["LoggedUserId"].ToString());
                            break;
                        case "ppt":
                        case "pptx":
                            PageCount = new MSPowerPoint().Convert(ExtractFilePath, ExtractFilePath.Substring(0, ExtractFilePath.LastIndexOf(".")) + ".pdf", true);
                            //Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSPowerPoint " + PageCount, Session["LoggedUserId"].ToString());
                            break;
                        case "xls":
                        case "xlsx":
                            PageCount = new MSExcel().Convert(ExtractFilePath, ExtractFilePath.Substring(0, ExtractFilePath.LastIndexOf(".")) + ".pdf", true);
                            ////Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSExcel " + PageCount, Session["LoggedUserId"].ToString());
                            break;
                       
                        default:
                            // result = "Failed";
                            break;
                    }





                    //if (FileExtension == "pdf")
                    //{
                    //    DocType = "application/pdf";

                    //    // Extract file and get the pages count
                    //    Logger.Trace("Invoke document extract. Path : " + ExtractFilePath, ApplicationName);
                    //    int count = new Image2Pdf().ExtractPages(ExtractFilePath);
                    //}
                    //else if (FileExtension == "tif")
                    //{
                    //    DocType = "image/tiff";

                    //    // Extract file and get the pages count
                    //    Logger.Trace("Invoke document extract. Path : " + ExtractFilePath, ApplicationName);
                    //    int count = new Image2Pdf().tiff2PDF(ExtractFilePath, ExtractFilePath.Substring(0, ExtractFilePath.LastIndexOf(".")), true);
                    //}
                    //else
                    //{
                    //    Logger.Trace("Invalid file extension : " + FileExtension, ApplicationName);
                    //}

                    // Zip file 
                    Logger.Trace("Zipping document. File path : " + ExtractFilePath, ApplicationName);
                    if (File.Exists(beforedot(ExtractFilePath) + ".zip"))
                    {
                        File.Delete(beforedot(ExtractFilePath) + ".zip");
                    }
                    if (File.Exists(ExtractFilePath))
                    {
                        zipDocument(ExtractFilePath);
                    }

                }
                #endregion

                #region Append new docs to existing : Bring the splitted documents to temp folder from original documents storage location
                // include/append newly uploaded documents(with zip).
                else
                {
                    DocumentUploadedPath = ds.Tables[0].Rows[f]["Destination"].ToString(); // DMS refers this locaion
                    DBUpdateAction = "AppendToExistingFile";
                    Logger.Trace("Append document.", ApplicationName);
                    Logger.Trace("File " + FileNameWithExtension + " is processing.", ApplicationName);

                    if (!Directory.Exists(Folder))
                        Directory.CreateDirectory(Folder);
                    int index = DocumentUploadedPath.LastIndexOf("\\");
                    string ExtractPath = DocumentUploadedPath.Substring(0, index) + "\\";
                    // Unzip the existing document
                    Unzip(ExtractPath, EncryptedFileName);
                    Logger.Trace("Unzip the existing document." + DocumentUploadedPath, ApplicationName);


                    // Version the existing document
                    Logger.Trace("Version the existing document." + DocumentUploadedPath, ApplicationName);
                    //To Do

                    Logger.Trace("Copying already uploaded document to temporary folder path for appending purpose.", ApplicationName);
                    File.Copy(DocumentUploadedPath, UploadedFileTempPath);

                    Logger.Trace("Copying new document(bulk uploaded) to temporary folder path for appending purpose.", ApplicationName);
                    File.Copy(FilePathFromBulkUpload, AppendFileTempPath);

                    if (FileExtension == "pdf")
                    {
                        int count;

                        string[] source = new string[] { UploadedFileTempPath, AppendFileTempPath };
                        Logger.Trace("Merging files.", ApplicationName);
                        PDFMerging.MergeFilesForbulk(DocumentUploadedPath, source);
                        Logger.Trace("Merging files completed.", ApplicationName);

                        // Extract file and get the pages count
                        Logger.Trace("Invoke document extract. Path : " + DocumentUploadedPath, ApplicationName);
                        count = new Image2Pdf().ExtractPages(DocumentUploadedPath);
                    }
                    else
                    {
                        DirectoryInfo di = new DirectoryInfo(DocumentUploadedPath.Substring(0, DocumentUploadedPath.LastIndexOf(".")));
                        int count = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;

                        string[] source = new string[] { UploadedFileTempPath, AppendFileTempPath };
                        Logger.Trace("Merging files.", ApplicationName);
                        TiffUtil.TiffUtil.mergeTiffPages(source, DocumentUploadedPath, count, "After");
                        Logger.Trace("Merging files completed.", ApplicationName);

                        // Extract file and get the pages count
                        Logger.Trace("Invoke document extract. Path : " + DocumentUploadedPath, ApplicationName);
                        count = new Image2Pdf().tiff2PDF(DocumentUploadedPath, DocumentUploadedPath.Substring(0, DocumentUploadedPath.LastIndexOf(".")), true);
                    }

                    // Zip the document
                    Logger.Trace("Zip the merged document." + DocumentUploadedPath, ApplicationName);
                    if (File.Exists(beforedot(DocumentUploadedPath) + ".zip"))
                    {
                        File.Delete(beforedot(DocumentUploadedPath) + ".zip");

                    }
                    if (File.Exists(DocumentUploadedPath))
                    {
                        zipDocument(DocumentUploadedPath);
                    }


                }
                #endregion

                #region update database and delete processed files
                Logger.Trace("Call service to update database.", ApplicationName);
                string Message = string.Empty,
                ProcedureName = "USP_WindowsService_ManageSplittingfiles";

                string rData = string.Empty;

                //Create a array list to store parameter(s) with
                spParametersList = string.Empty;
                spArgumentsCollection("@in_vAction", DBUpdateAction, "varchar");
                spArgumentsCollection("@in_iProcessed", ds.Tables[0].Rows[f]["ID"].ToString(), "varchar");
                spArgumentsCollection("@in_vOriginFileName", FileNameWithExtension, "varchar");
                spArgumentsCollection("@in_vDocName", EncryptedFileNameWithExtension, "varchar");
                spArgumentsCollection("@in_vDocVirtualPath", DocVirtualPath, "varchar");
                spArgumentsCollection("@in_vDocPhysicalPath", DocPhysicalPath, "varchar");
                spArgumentsCollection("@in_vType", DocType, "varchar");
                spArgumentsCollection("@in_vBULKUPLOAD_vError", string.Empty, "varchar");

                Crypt crypt = new Crypt();
                GenService.IService cleintupload = new GenService.ServiceClient();
                GenService.StoredProcedureReturnsDataset contractInfo = new GenService.StoredProcedureReturnsDataset()
                {
                    ConnectionString_Encrypted = crypt.Encrypt(ConnectionString),
                    ProcedureName_Encrypted = crypt.Encrypt(ProcedureName),
                    ParametersList_Encrypted = crypt.Encrypt(spParametersList)
                };
                rData = cleintupload.RunStoredProcedureReturnsDataset(contractInfo);

                DataSet DS = DSXML.ConvertXMLToDataSet(rData);
                if (DS != null && DS.Tables.Count > 0 && DS.Tables[1].Rows.Count > 0 && DS.Tables[1].Rows[0]["RESULT"].ToString() == "SUCCESS")
                {
                    Logger.Trace("Database update success.", ApplicationName);

                    // In case of New Upload
                    if (File.Exists(ExtractFilePath.Replace(Path.GetExtension(ExtractFilePath), ".zip")) && File.Exists(FilePathFromBulkUpload))
                    {
                        Logger.Trace("Deleting file. Path : " + FilePathFromBulkUpload, ApplicationName);
                        File.Delete(FilePathFromBulkUpload);
                    }

                    // In case of Document Append
                    if (File.Exists(DocumentUploadedPath.Replace(Path.GetExtension(ExtractFilePath), ".zip")) && File.Exists(FilePathFromBulkUpload))
                    {
                        Logger.Trace("Deleting file. Path : " + FilePathFromBulkUpload, ApplicationName);
                        File.Delete(FilePathFromBulkUpload);
                    }
                }
                else
                {
                    Logger.Trace("Database update failed.", ApplicationName);
                }
                #endregion

                #region Delete directory once all the files are processed.

                int filesCount = 0;
                string DirectoryToDelete = FilePathFromBulkUpload.ToLower().Replace("\\" + FileNameWithExtension, string.Empty); // Upto date/time
                DirectoryToDelete = DirectoryToDelete.Substring(0, DirectoryToDelete.LastIndexOf("\\")); //Upto token

                DirectoryInfo TempUploadDirectory = new DirectoryInfo(DirectoryToDelete);
                filesCount += TempUploadDirectory.GetFiles("*.pdf", SearchOption.AllDirectories).Length;
                filesCount += TempUploadDirectory.GetFiles("*.tif", SearchOption.AllDirectories).Length;

                if (filesCount == 0)
                {
                    try
                    {
                        Logger.Trace("Deleting directory. Path : " + DirectoryToDelete, ApplicationName);
                        Directory.Delete(DirectoryToDelete, true);

                    }
                    catch (IOException ex)
                    {
                        throw ex;
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                TrackingError(ds.Tables[0].Rows[f]["ID"].ToString(), ex.ToString());
                Logger.Trace("Exception : " + ex.Message, ApplicationName);
            }

            timer1.Start();
            Logger.Trace("Timer started agan.", ApplicationName);

        }

        public static string spArgumentsCollection(string spParmName, string spParmValue, string spPramValueType)
        {
            spParametersList += "[" + spParmName + "|" + spParmValue + "|" + spPramValueType + "]";
            return spParametersList;
        }

        public static string Encryptdata(string password)
        {
            string strmsg = string.Empty;
            byte[] encode = new
            byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }
        /// <summary>
        /// Unzip the document
        /// </summary>
        /// <param name="ExtractFilePath"></param>
        /// <param name="FileName"></param>
        public static void Unzip(string ExtractFilePath, string FileName)
        {
            try
            {
                string path = ExtractFilePath + FileName + ".zip";

                using (ZipFile zip = ZipFile.Read(path))
                {
                    zip.ExtractAll(ExtractFilePath, ExtractExistingFileAction.DoNotOverwrite);
                }


            }
            catch (Exception)
            {
                Logger.Trace("Error while unzipping.", "TraceStatus");

            }
        }

        /// <summary>
        /// Zip the document
        /// </summary>
        /// <param name="ActualFolder"></param>
        /// <param name="temp"></param>
        /// 

        public static void zipDocument(string Tempfilepath)
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    if (File.Exists(Tempfilepath))
                    {
                        zip.AddFile(Tempfilepath, "");
                        zip.Save(beforedot(Tempfilepath) + ".zip");
                        File.Delete(Tempfilepath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("error in zipping the document " + ex.Message, "zipDocument");
                throw ex;
            }
        }

        public static string beforedot(string s)
        {
            int l = s.LastIndexOf(".");
            return s.Substring(0, l);
        }



    }
}
