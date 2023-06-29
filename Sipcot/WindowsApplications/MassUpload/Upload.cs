using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Reflection;
using Client;
using System.Xml;
using System.Xml.Serialization;

namespace MassUpload
{
    public partial class Upload : Form
    {
        string AppName = string.Empty, AppVersion = string.Empty,
            AppDescription = string.Empty,
            AppCopyright = string.Empty,
            AppCompany = string.Empty;

        public Upload()
        {
            InitializeComponent();

            //Show application name and version
            // Application Name
            AppName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
            // Assembly version
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //App description
            AppDescription = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute), false)).Description;
            //App copyright
            AppDescription = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright;
            //App Company
            AppCompany = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company;

            lblVersion.Text = AppName + " " + AppVersion;
            notifyIcon.Text = AppName + " " + AppVersion;
            notifyIcon.Visible = false;

            lblChannel.Text = "\t\t\t Channel: " + Global.Channel;


            // Listview initialization for column sort
            lvwColumnSorter = new ListViewColumnSorter();
            lvwUploads.ListViewItemSorter = lvwColumnSorter;
            lvwUploads.Sorting = SortOrder.Ascending;
            lvwUploads.AutoArrange = true;

            lvwColumnSorter._SortModifier = ListViewColumnSorter.SortModifiers.SortByText;

        }

        #region Public variables
        private ListViewColumnSorter lvwColumnSorter = null;

        static int maxThreads = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfThreads"].ToString());
        private BackgroundWorker[] threadArray = new BackgroundWorker[maxThreads];
        static int _numberBackGroundThreads;
        // public static FileInfo[] SelectedFilesArray;
        //Array to hold file that needs to be upload.
        public static FileInfo[] UploadFilesArray;
        public int TotalFilesCount = 0, // To keep total file selected for upload
            Uploadedcount = 0; // To track uploaded files count
        #endregion

        #region progress
        //To get uploaded files count
        public int countuploaded()
        {
            Uploadedcount = 0;
            lvwUploads.Invoke((Action)(() =>
            {
                for (int i = 0; i < lvwUploads.Items.Count; i++)
                {
                    if (lvwUploads.Items[i].SubItems[2].Text == "100%")
                    {
                        Uploadedcount++;
                    }
                }
            }));
            return Uploadedcount;
        }

        //for changing the progress bar and the lables according to the listview!
        public void changeProgessbarStatus()
        {
            try
            {
                progressBar.Invoke((Action)(() =>
                {
                    countuploaded();
                    int progess = (Uploadedcount * 100) / TotalFilesCount;
                    int currentuploadingcount = Uploadedcount + 1;
                    this.progressBar.Maximum = 100;
                    this.progressBar.Minimum = 0;
                    this.progressBar.Value = progess;

                    if (Uploadedcount == TotalFilesCount)
                    {
                        lblStatus.Text = "Finished Uploading all " + TotalFilesCount + "  files.." as String;
                        lblPercentage.Text = progess.ToString() + " %";

                        btnBrowse.Enabled = true;
                        btnUpload.Enabled = true;
                        btnclear.Enabled = true;
                    }
                    else
                    {
                        lblStatus.Text = "Uploading file " + currentuploadingcount + " of " + (TotalFilesCount) as String;
                        lblPercentage.Text = progess.ToString() + " %";
                    }
                }));
            }
            catch (Exception ex)
            {
                Logger.logException(ex, Global.UserName);
            }
        }

        //for updating the progress in list  according to the file path uploded progress change
        int LastItemIndex = 0;
        private void UpdateFileProgress(string Filename, string Percentage, string Status)
        {
            //the below invoke methord is used to avoid problem with multiple threads
            lvwUploads.Invoke((Action)(() =>
            {
                for (int i = 0; i < lvwUploads.Items.Count; i++)
                {
                    if (lvwUploads.Items[i].SubItems[4].Text == Filename)
                    {
                        if (Percentage != string.Empty)
                            lvwUploads.Items[i].SubItems[2].Text = String.Format("{0:0.##}%", Percentage);
                        lvwUploads.Items[i].SubItems[3].Text = Status;

                        //If upload completed uncheck the item and disable
                        if (Status == "Completed")
                        {
                            lvwUploads.Items[i].Checked = false;
                            lvwUploads.Items[i].BackColor = Color.LawnGreen;
                        }
                        else
                        {
                            //Move currently uploading item to top
                            if (LastItemIndex != lvwUploads.Items[i].Index)
                            {
                                if (maxThreads == 1) //If only one thread then move currently uploading item to top
                                    lvwUploads.TopItem = lvwUploads.Items[i];
                                lvwUploads.Items[i].BackColor = Color.Pink;
                            } LastItemIndex = lvwUploads.Items[i].Index;
                        }
                    }
                }
            }));
        }


        /// <summary>
        /// To return size in kb
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>

        private string GetsizeinKB(string Size)
        {
            string asize = string.Empty;
            int s = Convert.ToInt32(Size);

            asize = (s / 1024).ToString() + " KB";

            return asize;
        }

        // getting result in progress change
        void uploadStreamWithProgress_ProgressChanged(object sender, StreamWithProgress.ProgressChangedEventArgs e)
        {
            int percentage = 0;
            string filepath = ((Client.StreamWithProgress)(sender)).file.Name;

            if (e.Length != 0)
            {
                percentage = (int)(e.BytesRead * 100 / e.Length);
                //checking if percentage  uploaded is 100% or not, accoringly it will change the status.!
                if (percentage == 100)
                {
                    //Logger.logTrace("uploadStreamWithProgress_ProgressChanged filepath:-" + filepath + "percentage;-" + percentage.ToString(), Global.UserName);
                    UpdateFileProgress(filepath, percentage.ToString(), "Completed");

                    changeProgessbarStatus();

                    // Update data(xml) as upload completed
                    Update(new Data()
                        {
                            FileName = Path.GetFileName(filepath),
                            Status = "Completed"
                        });
                }
                else
                {
                    UpdateFileProgress(filepath, percentage.ToString(), "Uploading..");
                }
            }
        }
        #endregion

        #region private methods

        //common function to Bind Files  to the Listview 
        private void AddingFilesToListView(OpenFileDialog ofd)
        {
            Logger.logTrace("Upload - Adding files to list view from temporary array.", Global.UserName);
            for (int i = 0; i < ofd.FileNames.Length; i++)
            {
                //checking for duplicate 
                ListViewItem FDuplicate = lvwUploads.FindItemWithText(Path.GetFileName(ofd.FileNames[i].ToString()));
                if (FDuplicate == null)
                {
                    ListViewItem item = new ListViewItem();
                    item.Checked = true;
                    item.Text = Path.GetFileName(ofd.FileNames[i].ToString());

                    item.SubItems.Add(GetsizeinKB(new FileInfo(ofd.FileNames[i].ToString()).Length.ToString()));
                    item.SubItems.Add(String.Format("{0:0.##}%", "0"));
                    item.SubItems.Add("Pending");
                    item.SubItems.Add(ofd.FileNames[i].ToString());
                    lvwUploads.Items.Add(item);
                    AddActivity("Added to the list  file:-" + Path.GetFileName(ofd.FileNames[i].ToString()));
                }
                else
                {
                    AddActivity("Not added to the list as the file:-" + Path.GetFileName(ofd.FileNames[i].ToString()) + " already exists in the list");
                }
            }
            Logger.logTrace("Upload - Completed adding files to list view from temporary array.", Global.UserName);
        }

        //common function to Bind Log to the riched text box
        private void AddActivity(string activity)
        {
            try
            {
                //adding log to the rich textbox
                //the below code used in this way to avoid multi threading error
                rtbStatus.Invoke((Action)(() =>
                {
                    //checking for adding in next line
                    if (rtbStatus.Text.Length > 0)
                    {
                        //appending vales to the textbox
                        rtbStatus.Text = rtbStatus.Text.Insert(0, DateTime.Now.ToString() + ": " + activity + Environment.NewLine);
                    }
                    else
                    {
                        //appending vales to the textbox
                        rtbStatus.Text = rtbStatus.Text.Insert(0, DateTime.Now.ToString() + ": " + activity + Environment.NewLine);
                    }

                }));
            }
            catch (Exception ex)
            {
                Logger.logException(ex, Global.UserName);
            }
        }

        // To prepare array for file upload
        private void PrepareArrayForFilesUpload()
        {
            try
            {
                Logger.logTrace("Upload - Preparing files array for upload.", Global.UserName);
                string filepath = string.Empty;

                //setting array length with the checked listbox checked items count
                UploadFilesArray = new FileInfo[lvwUploads.CheckedItems.Count];
                int index = 0;

                //Get tha max id from data file(xml)
                int MaxId = GetMaxId() + 1;

                for (int i = 0; i < lvwUploads.Items.Count; i++)
                {
                    Application.DoEvents();
                    if (lvwUploads.Items[i].Checked == true)
                    {
                        filepath = lvwUploads.Items[i].SubItems[4].Text;
                        UploadFilesArray[index] = new FileInfo(filepath);
                        index++;

                        //Add to data (xml) if it is a fresh upload
                        if (!Global.IsResume)
                        {
                            Insert(new Data()
                            {
                                Id = MaxId,
                                FileName = Path.GetFileName(filepath),
                                FilePath = filepath,
                                Status = "Pending"
                            });
                            MaxId++;
                        }
                    }
                }

                // Save dv to xml
                if (!Global.IsResume)
                    save();

                Logger.logTrace("Upload - Finished preparation of files array for upload.", Global.UserName);
            }
            catch (Exception ex)
            {
                Logger.logException(ex, Global.UserName);
            }
        }

        /// <summary>
        /// Uploads file to the server by invoking service
        /// </summary>
        /// <param name="fileCounter"></param>
        /// <param name="Filepath"></param>
        /// <param name="Filename"></param>
        private void UploadFile(int fileCounter, string Filepath, string Filename)
        {
            try
            {
                Logger.logTrace("Upload - Started  uploading file: " + Filepath, Global.UserName);
                AddActivity("Started uploading file: " + Filepath);

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(Filepath);
                //Call service and post data
                ServiceReference1.ITransferService cleintupload = new ServiceReference1.TransferServiceClient();
                ServiceReference1.RemoteFileInfo uploadRequestInfo = new ServiceReference1.RemoteFileInfo();
                using (System.IO.FileStream stream = new System.IO.FileStream(Filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    uploadRequestInfo.ChannelInfo = Global.FileUploadPath;
                    uploadRequestInfo.FileName = Filename;
                    uploadRequestInfo.Length = fileInfo.Length;

                    // To post data and update progress for each file
                    using (StreamWithProgress uploadStreamWithProgress = new StreamWithProgress(stream))
                    {
                        uploadStreamWithProgress.ProgressChanged += uploadStreamWithProgress_ProgressChanged;
                        uploadRequestInfo.FileByteStream = uploadStreamWithProgress;
                        // Invoke service method
                        cleintupload.UploadFile(uploadRequestInfo);
                    }
                }
                AddActivity("Upload - Finished uploading file: " + Filepath);
                Logger.logTrace("Finised uploading file: " + Filepath, Global.UserName);
            }
            catch (Exception ex)
            {
                UpdateFileProgress(Filepath, "", "Failed");

                Logger.logException(ex, Global.UserName);
                AddActivity("Failed uploading file: " + Filepath + ". Error Info:" + ex.Message);
            }
        }
        #endregion

        #region thread
        /// <summary>
        /// Function used to intialize the Threads as per the app.config value
        /// </summary>
        public void InitializeThread()
        {
            for (int f = 0; f < maxThreads; f++)
            {
                threadArray[f] = new BackgroundWorker();
                threadArray[f].DoWork += new DoWorkEventHandler(backgroundWorkerFiles_DoWork);
                threadArray[f].RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorkerFiles_RunWorkerCompleted);
                //  threadArray[f].ProgressChanged += new ProgressChangedEventHandler(backgroundWorkerFiles_ProgressChanged);
                threadArray[f].WorkerReportsProgress = true;
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
            string Filepath = UploadFilesArray[myProcessArguments].FullName;
            string Filename = UploadFilesArray[myProcessArguments].Name;
            UploadFile(myProcessArguments, Filepath, Filename);

            int progress = 0, findPercentage = 0;

            findPercentage = ((myProcessArguments + 1) * 100) / (UploadFilesArray.Length);
            progress = 0;
            progress += findPercentage;
            //Report back to the UI
            string progressStatus = "Uploading file " + (myProcessArguments + 1).ToString() + " of " + (UploadFilesArray.Length);
            ((BackgroundWorker)se).ReportProgress(progress, progressStatus);
        }

        /// <summary>
        /// Creates thread for file upload
        /// </summary>
        /// <param name="filesCount"></param>
        private void CreateThreads(int filesCount)
        {
            Logger.logTrace("Upload - Started creating threads.", Global.UserName);
            int fileCounter = 0;
            for (fileCounter = 0; fileCounter < filesCount; fileCounter++)
            {
                //Use the thread array to process each iteration
                //choose the first unused thread.
                bool fileProcessed = false;
                while (!fileProcessed)
                {
                    for (int threadNum = 0; threadNum < maxThreads; threadNum++)
                    {
                        if (!threadArray[threadNum].IsBusy)
                        {
                            //Call the "RunWorkerAsync()" method of the thread.  
                            //This will call the delegate method "backgroundWorkerFiles_DoWork()" method defined above.  
                            //The parameter passed (the loop counter "fileCounter") will be available through the delegate's argument "e" through the ".Argument" property.
                            threadArray[threadNum].RunWorkerAsync(fileCounter);
                            fileProcessed = true;
                            break;
                        }
                    }
                    //If all threads are being used, sleep awhile before checking again
                    if (!fileProcessed)
                    {
                        Thread.Sleep(50);
                        Application.DoEvents();
                    }
                }
            }
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
        /// To update the Progress Bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void backgroundWorkerFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    // Use this method to report progress to GUI
        //    this.progressBar.Maximum = 100;
        //    this.progressBar.Minimum = 0;
        //    this.progressBar.Value = e.ProgressPercentage;

        //    lblStatus.Text = e.UserState as String;
        //    lblPercentage.Text = String.Format("Total Progress: {0} %", e.ProgressPercentage);
        //    panelUploadControls.Enabled = false;

        //    if (Convert.ToInt32(e.ProgressPercentage) == 100)
        //    {
        //        lblStatus.Text = "All the files have been uploaded successfully.";
        //        lblPercentage.Text = String.Format("Total Progress: {0} %", e.ProgressPercentage);
        //        panelUploadControls.Enabled = true;
        //    }

        //}

        #endregion

        #region Form events
        //Used for browsing the files
        OpenFileDialog ofd = new OpenFileDialog();
        DialogResult result = new DialogResult();
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.logTrace("Upload - User clicked browse button.", Global.UserName);
                //checking for folder upload or file upload
                if (rbFile.Checked)
                {
                    Logger.logTrace("Upload - User using file selection mode.", Global.UserName);

                    //adding file dialog to an object
                    ofd = openFileDialog;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        SelectFiles();
                    }
                }
                else
                {
                    Logger.logTrace("Upload - User using folder selection mode.", Global.UserName);

                    //for folder wise upload
                    result = FolderLocator.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        SelectFilesFromFolder();
                    }
                }

                if (lvwUploads.Items.Count > 0)
                    btnUpload.Enabled = true;

                Logger.logTrace("Finised btnBrowse_Click", Global.UserName);

                //Clear xml data
                ClearXmlData();
            }
            catch (Exception ex)
            {
                Logger.logException(ex, Global.UserName);
            }
        }

        //Select files : multi selection mode
        void SelectFiles()
        {
            lvwUploads.Invoke((Action)(() =>
            {
                // Add selected files to list view : exclude duplicate files i.e., already exist in lisview).
                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    Application.DoEvents();
                    //checking for duplicate 
                    ListViewItem FDuplicate = lvwUploads.FindItemWithText(Path.GetFileName(ofd.FileNames[i].ToString()));
                    if (FDuplicate == null)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Checked = true;
                        item.Text = Path.GetFileName(ofd.FileNames[i].ToString());

                        item.SubItems.Add(GetsizeinKB(new FileInfo(ofd.FileNames[i].ToString()).Length.ToString()));
                        item.SubItems.Add(String.Format("{0:0.##}%", "0"));
                        item.SubItems.Add("Pending");
                        item.SubItems.Add(ofd.FileNames[i].ToString());
                        lvwUploads.Items.Add(item);
                        Logger.logTrace("Upload - File added to the list: " + Path.GetFileName(ofd.FileNames[i].ToString()), Global.UserName);
                        AddActivity("Added to the list  file: " + Path.GetFileName(ofd.FileNames[i].ToString()));
                    }
                    else
                    {
                        Logger.logTrace("Upload - File not added to the list as the file: " + Path.GetFileName(ofd.FileNames[i].ToString()) + " already exists in the list.", Global.UserName);
                        AddActivity("File not added to the list as the file: " + Path.GetFileName(ofd.FileNames[i].ToString()) + " already exists in the list");
                    }
                }
            }));
        }

        //Select files : folder selection mode
        void SelectFilesFromFolder()
        {
            Logger.logTrace("getting the folder location from the FolderLocatordialogresult", Global.UserName);

            //setting the folder path to the textbox
            txtFolderLocation.Text = FolderLocator.SelectedPath;

            //adding file to temp array to insert properly to the main array
            FileInfo[] TempFiles = new FileInfo[Directory.GetFiles(txtFolderLocation.Text).Length];

            DirectoryInfo dirfile; dirfile = new DirectoryInfo(FolderLocator.SelectedPath.ToString());

            TempFiles = dirfile.GetFiles().ToArray();

            lvwUploads.Invoke((Action)(() =>
           {
               //adding to the main array 
               for (int i = 0; i < TempFiles.Length; i++)
               {
                   Application.DoEvents();
                   //checking for duplicate 
                   ListViewItem FDuplicate = lvwUploads.FindItemWithText(TempFiles[i].Name);
                   if (FDuplicate == null)
                   {
                       ListViewItem item = new ListViewItem();
                       item.Checked = true;
                       item.Text = TempFiles[i].Name;

                       item.SubItems.Add(GetsizeinKB(TempFiles[i].Length.ToString()));
                       item.SubItems.Add(String.Format("{0:0.##}%", "0"));
                       item.SubItems.Add("Pending");
                       item.SubItems.Add(TempFiles[i].FullName);
                       lvwUploads.Items.Add(item);
                       Logger.logTrace("Upload - Added to the list  file: " + TempFiles[i].Name, Global.UserName);
                       AddActivity("Upload - Added file to the list view: " + TempFiles[i].Name);
                   }
                   else
                   {
                       Logger.logTrace("Upload - Not added to the list as the file:" + TempFiles[i].Name + " already exists in the list.", Global.UserName);
                       AddActivity("Upload - File not added to the list view as the file: " + TempFiles[i].Name + " already exists in the list.");
                   }
               }
           }));
        }

        private void btncancel_Click(object sender, EventArgs e)
        {
            Logger.logTrace("Started  btncancel_Click", Global.UserName);

            // Cancel the asynchronous operation. 
            for (int threadNum = 0; threadNum < maxThreads; threadNum++)
            {
                this.threadArray[threadNum].CancelAsync();
            }
            Logger.logTrace("Finised  btncancel_Click", Global.UserName);
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton.Checked && radioButton.Name == "rbFolder")
                txtFolderLocation.Enabled = true;
            else
                txtFolderLocation.Enabled = false;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            (new frmAbout()).ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Upload_FormClosing(sender, null);
        }

        private void Upload_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if threads are running show a confirmation message to user before closing the application.
            bool ThreadRunning = false;
            for (int threadNum = 0; threadNum < threadArray.Length; threadNum++)
                if (threadArray != null && threadArray[threadNum] != null && threadArray[threadNum].IsBusy)
                {
                    ThreadRunning = true;
                    break;
                }

            if (ThreadRunning)
            {
                if (Thread.CurrentThread.IsAlive)
                    if (MessageBox.Show("Upload process is not completed.\nDo you really want to abort the process and exit the application?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // e.Cancel = true;
                        Thread.CurrentThread.Abort();
                        closeApplication();
                    }
            }
            else
            {
                closeApplication();
            }
        }

        private void btnclear_Click(object sender, EventArgs e)
        {
            try
            {
                lblPercentage.Text = lblStatus.Text = string.Empty;
                progressBar.Value = 0;
                txtFolderLocation.Clear();
                lvwUploads.Items.Clear();
                rtbStatus.Clear();
                //Array.Clear(SelectedFilesArray, 0, SelectedFilesArray.Length);
                Array.Clear(UploadFilesArray, 0, UploadFilesArray.Length);                
            }
            catch (Exception ex)
            {
                Logger.logException(ex, Global.UserName);
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            Logger.logTrace("Upload - User clicked upload button.", Global.UserName);

            if (lvwUploads.Items.Count > 0)
            {
                btnclear.Enabled = false;

                //Prepare files (path) array for upload
                PrepareArrayForFilesUpload();

                //Set total files count from UploadFilesArray 
                //and already upload completed files count : because if user adding more files after one upload cycle completion adn uploading again..
                // need to consider already uploaded file else progressbar will through error
                TotalFilesCount = UploadFilesArray.Length + countuploaded();

                // Initialize threads
                InitializeThread();

                //initiating the work by pasing total length 
                CreateThreads(UploadFilesArray.Length);
            }
            else
            {
                Logger.logTrace("Upload - No files to upload.", Global.UserName);
                MessageBox.Show("Please select files to upload.", "Upload", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //form load
        private void Upload_Load(object sender, EventArgs e)
        {
            btnUpload.Enabled = false;

            //Initialize: load dataview from xml data
            SelectAll();
        }

        #endregion

        #region notify

        private void Form_Resize(object sender, System.EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon.Visible = true;
                notifyIcon.BalloonTipTitle = AppName + " " + AppVersion;
                notifyIcon.BalloonTipText = AppDescription + "." + Environment.NewLine +
                    "developed by " + AppCompany +
                    Environment.NewLine + AppCopyright;
                notifyIcon.ShowBalloonTip(500);
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }
        #endregion

        private void closeApplication()
        {
            notifyIcon.Dispose();
            Environment.Exit(0);
        }

        #region handle xml using ds

        string DataXmlFilepath = Environment.CurrentDirectory + @"\Data.xml";

        DataSet ds = new DataSet();
        DataView dv = new DataView();

        void save()
        {
            lvwUploads.Invoke((Action)(() =>
            {
                ds.WriteXml(DataXmlFilepath, XmlWriteMode.WriteSchema);
            }));
        }
        void Insert(Data entity)
        {
            string Filename = entity.FileName.Replace("'", "");

            // Check file name exists, if exists delete
            dv.RowFilter = "FileName='" + Filename + "'";
            dv.Sort = "Id";
            if (dv.Count > 0)
            {
                dv.Delete(0);
            }
            dv.RowFilter = "";

            DataRow dr = dv.Table.NewRow();
            dr[0] = entity.Id;
            dr[1] = Filename;
            dr[2] = entity.FilePath;
            dr[3] = entity.Status;
            dr[4] = entity.Remarks;
            dv.Table.Rows.Add(dr);
            // save();

        }

        /// <summary>
        /// Updates a record in the Category table.
        /// </summary>
        void Update(Data entity)
        {
            string Filename = entity.FileName.Replace("'", "");
            DataRow dr = Select(Filename);
            // dr[0] = entity.FileName;
            // dr[2] = entity.FilePath;
            dr[3] = entity.Status;
            // dr[4] = entity.Remarks;
            save();
        }

        /// <summary>
        /// Deletes a record from the Category table by a composite primary key.
        /// </summary>
        void Delete(int Id)
        {
            dv.RowFilter = "Id='" + Id + "'";
            dv.Sort = "Id";
            if (dv.Count > 0)
            {
                dv.Delete(0);
            }
            dv.RowFilter = "";
            save();
        }

        /// <summary>
        /// Selects a single record by Id.
        /// </summary>
        DataRow Select(int Id)
        {
            dv.RowFilter = "Id='" + Id + "'";
            dv.Sort = "Id";
            DataRow dr = null;
            if (dv.Count > 0)
            {
                dr = dv[0].Row;
            }
            dv.RowFilter = "";
            return dr;
        }

        /// <summary>
        /// Selects a single record by FileName.
        /// </summary>
        DataRow Select(string FileName)
        {
            dv.RowFilter = "FileName='" + FileName.Replace("'", "") + "'";
            dv.Sort = "Id";
            DataRow dr = null;
            if (dv.Count > 0)
            {
                dr = dv[0].Row;
            }
            dv.RowFilter = "";
            return dr;
        }

        /// <summary>
        /// Selects all records from the Category table.
        /// </summary>
        DataView SelectAll()
        {
            if (!File.Exists(DataXmlFilepath))
            {
                var xmlString = CreateXML(new Data());

                // Create xml document                
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString.ToString());
                xmlDoc.Save(DataXmlFilepath);
            }

            ds.Clear();
            ds.ReadXml(DataXmlFilepath, XmlReadMode.ReadSchema);
            dv = ds.Tables[1].DefaultView;
            return dv;
        }

        /// <summary>
        /// Get max id from dataview
        /// </summary>
        /// <param name="XMLString"></param>
        /// <param name="YourClassObject"></param>
        /// <returns></returns>
        private int GetMaxId()
        {
            int maxId = 0;
            dv.RowFilter = "Id = max(Id)";
            if (dv.Count > 0)
            {
                maxId = Convert.ToInt32(dv[0].Row[0]);
            }
            dv.RowFilter = "";
            return maxId;
        }

        /// <summary>
        /// To get Pending file count
        /// </summary>
        /// <returns></returns>
        private int GetNotUploadedCount()
        {
            int count = 0;
            dv.RowFilter = "Status = 'Pending'";
            if (dv.Count > 0)
            {
                count = dv.Count;
            }
            dv.RowFilter = "";
            return count;
        }

        private int GetFilesCountFromXml()
        {
            int count = 0;
            dv.RowFilter = "";
            if (dv.Count > 0)
            {
                count = dv.Count;
            }
            return count;
        }

        //Clear data from Xml
        void ClearXmlData()
        {
            Logger.logTrace("Upload - Started clearing xml data.", Global.UserName);
            dv.RowFilter = "";
            if (dv.Count > 0)
            {
                dv.Table.Clear();
            }
            save();
            Logger.logTrace("Upload - Completed clearing xml data.", Global.UserName);
        }

        private void AddingFilesToListViewFromXml()
        {
            Logger.logTrace("Upload - Adding file to list view from xml.", Global.UserName);

            dv.RowFilter = "";
            dv.Sort = "Id";
            if (dv.Count > 0)
            {
                for (int counter = 0; counter < dv.Count; counter++)
                {
                    ListViewItem item = new ListViewItem();

                    string upload_status = dv[counter].Row["Status"].ToString();

                    if (upload_status == "Completed")
                        item.Checked = false;
                    else
                        item.Checked = true;

                    item.Text = dv[counter].Row["FileName"].ToString();
                    item.SubItems.Add(GetsizeinKB(new FileInfo(dv[counter].Row["FilePath"].ToString()).Length.ToString()));
                    item.SubItems.Add(String.Format("{0:0.##}%", upload_status == "Completed" ? "100" : "0"));
                    item.SubItems.Add(upload_status == "Completed" ? "Completed" : "Pending");
                    item.SubItems.Add(dv[counter].Row["FilePath"].ToString());
                    lvwUploads.Items.Add(item);
                    AddActivity("Added file to the list view  from xml: " + dv[counter].Row["FileName"].ToString());
                }
            }


            Logger.logTrace("Upload - Completed adding file to list view from xml.", Global.UserName);
        }

        Object CreateObject(string XMLString, Object YourClassObject)
        {
            XmlSerializer oXmlSerializer = new XmlSerializer(YourClassObject.GetType());
            //The StringReader will be the stream holder for the existing XML file 
            YourClassObject = oXmlSerializer.Deserialize(new StringReader(XMLString));
            //initially deserialized, the data is represented by an object without a defined type 
            return YourClassObject;
        }

        string CreateXML(Object YourClassObject)
        {
            XmlDocument xmlDoc = new XmlDocument();   //Represents an XML document, 
            // Initializes a new instance of the XmlDocument class.          
            XmlSerializer xmlSerializer = new XmlSerializer(YourClassObject.GetType());
            // Creates a stream whose backing store is memory. 
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, YourClassObject);
                xmlStream.Position = 0;
                //Loads the XML document from the specified string.
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }

        string SerializeAnObject(Object item)
        {
            if (item == null)
                return null;

            var stringBuilder = new StringBuilder();
            var itemType = item.GetType();

            new XmlSerializer(itemType).Serialize(new StringWriter(stringBuilder), item);

            return stringBuilder.ToString();
        }

        #endregion

        private void lvwUploads_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Do not allow to check the items already uploaded
            if (lvwUploads.Items[e.Index].SubItems[3].Text == "Completed") e.NewValue = CheckState.Unchecked;
        }

        private void Upload_Shown(object sender, EventArgs e)
        {
            //Get the Pending count: if there is any not uploaded images prompt user to resume the activity
            // If user selects YES, resume else clear data from Xml
            int NotUploadedCount = GetNotUploadedCount();
            if (NotUploadedCount > 0)
                if (MessageBox.Show("Previous upload was unsuccessful. Do you want to resume the upload?" +
                    Environment.NewLine + Environment.NewLine + "Select Yes to resume." +
                    Environment.NewLine + "Select No to abort.",
                    "Upload", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Logger.logTrace("Upload - User resumed previously aborted upload.", Global.UserName);

                    Global.IsResume = true;

                    // Add file to list view from xml
                    lblStatus.Text = "Initializing files..";
                    AddingFilesToListViewFromXml();

                    if (lvwUploads.Items.Count > 0)
                    {
                        //Prepare files (path) array for upload
                        lblStatus.Text = "Initializing upload..";
                        PrepareArrayForFilesUpload();

                        //Set total files count from Xml
                        lblStatus.Text = "Checking files base..";
                        TotalFilesCount = GetFilesCountFromXml();

                        // Initialize threads
                        lblStatus.Text = "Initializing threads..";
                        InitializeThread();

                        //initiating the work by pasing total length 
                        lblStatus.Text = "Creating threads..";
                        CreateThreads(UploadFilesArray.Length);
                    }
                }
                else
                {
                    Global.IsResume = false;
                    // Clear data
                    ClearXmlData();
                }
        }

        private void lvwUploads_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView myListView = (ListView)sender;

            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.ColumnToSort)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.OrderOfSort == SortOrder.Ascending)
                {
                    lvwColumnSorter.OrderOfSort = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.OrderOfSort = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.ColumnToSort = e.Column;
                lvwColumnSorter.OrderOfSort = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            myListView.Sort();
        }

        // To avoid form flickering
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

    }
}
