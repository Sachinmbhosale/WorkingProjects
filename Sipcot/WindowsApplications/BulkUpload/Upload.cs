using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;
using System.Deployment.Application;
using System.Web;
using System.Threading;

namespace WindowsForm_SERVICE
{
    public partial class Upload : Form
    {
        public static string OpenDirectory = string.Empty;
        public static string spParametersList = string.Empty;
        public static string OrgID = "1";
        public static string LoginToken = "36033a0b-53c3-4c14-b22e-f7203df66576";
        public static string ProjectID;
        public static string DepartmentID;
        public static string BatchName;
        public static DataSet DS = new DataSet();
        public static DataTable PdfTable = new DataTable();
        public static DataTable TifTable = new DataTable();
        public static DirectoryInfo dirfile;
        public static FileInfo[] pdffilesforcount;
        public static FileInfo[] tiffilesforcount;
        public static FileInfo[] pdffiles;
        public static FileInfo[] tiffiles;
        public static FileInfo[] FilestoUpload;
        public static string FolderLocation;

        static int maxThreads = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfThreads"].ToString());
        private BackgroundWorker[] threadArray = new BackgroundWorker[maxThreads];
        static int _numberBackGroundThreads;
        static string SaveFolder = ConfigurationManager.AppSettings["UploadFolder"].ToString() + LoginToken;

        public Upload()
        {
            InitializeComponent();
        }

        private void Upload_Load(object sender, EventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                NameValueCollection QS = GetQueryStringParameters();
                OrgID = QS.Get("OrgID").ToString();
                LoginToken = QS.Get("LoginToken").ToString();
                SaveFolder = ConfigurationManager.AppSettings["UploadFolder"].ToString() + QS.Get("LoginToken").ToString();
            }
            InitializeThread();
            LoadProjectTypes();

            cmbBatch.BindingContext = new BindingContext();
            cmbBatch.DataBindings.Clear();
            cmbBatch.DataSource = null;
            cmbBatch.Items.Clear();
            cmbBatch.Items.Add(new ComboBoxItem("0", "--select--"));
            cmbBatch.SelectedIndex = 0;

        }

        /// <summary>
        /// Load Project Types / Documents Type
        /// </summary>
        private void LoadProjectTypes()
        {
            try
            {
                Logger.Trace("Started Loading Project Types" + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                string Message = string.Empty,
                  ConnectionString = ConfigurationManager.AppSettings["DMSInfoSearch_ConnectionString"].ToString(),
                  ProcedureName = "USP_SearchDocumentTypeId";
                DataSet ds = new DataSet();
                string rData = string.Empty;

                //Create a array list to store parameter(s) with
                spParametersList = string.Empty;
                spArgumentsCollection("@in_iCurrDocumnetTypeId", "0", "int");
                spArgumentsCollection("@DepartmentID", "0", "int");
                spArgumentsCollection("@CurrTemplateId", "0", "int");
                spArgumentsCollection("@in_iCurrOrgId", "0", "int");
                spArgumentsCollection("@in_vCreaedDateFrom", string.Empty, "varchar");
                spArgumentsCollection("@in_vCreaedDateTo", string.Empty, "varchar");
                spArgumentsCollection("@in_vAction", "DocumentTypeForWindows", "varchar");
                spArgumentsCollection("@in_vLoginToken", LoginToken, "varchar");
                spArgumentsCollection("@in_iLoginOrgId", OrgID, "varchar");
                spArgumentsCollection("@in_vDocumentTypeName", string.Empty, "varchar");

                Crypt crypt = new Crypt();
                ServiceReference1.IService cleintupload = new ServiceReference1.ServiceClient();
                ServiceReference1.StoredProcedureReturnsDataset contractInfo = new ServiceReference1.StoredProcedureReturnsDataset()
                {
                    ConnectionString_Encrypted = crypt.Encrypt(ConnectionString),
                    ProcedureName_Encrypted = crypt.Encrypt(ProcedureName),
                    ParametersList_Encrypted = crypt.Encrypt(spParametersList)
                };

                rData = cleintupload.RunStoredProcedureReturnsDataset(contractInfo);

                DataSet DS = DSXML.ConvertXMLToDataSet(rData);
                if (DS != null && DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 1)
                {
                    Logger.Trace("Project Types DS binding to combobox" + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                    cmbProjectType.BindingContext = new BindingContext();
                    cmbProjectType.DataBindings.Clear();
                    cmbProjectType.Items.Clear();
                    cmbProjectType.DataSource = DS.Tables[0];
                    cmbProjectType.DisplayMember = "DocumentTypeName";
                    cmbProjectType.ValueMember = "DocumentTypeId";
                }
                else
                {
                    Logger.Trace("Project Types DS Empty or Null" + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                    cmbProjectType.BindingContext = new BindingContext();
                    cmbProjectType.DataBindings.Clear();
                    cmbProjectType.DataSource = null;
                    cmbProjectType.Items.Clear();
                    cmbProjectType.Items.Add(new ComboBoxItem("0", "--select--"));
                    cmbProjectType.SelectedIndex = 0;
                }
                Logger.Trace("Finished Loading Project Types" + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                select();

            }
            catch (Exception ex)
            {
                Logger.Trace(ex.Message.ToString() + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
            }
        }

        /// <summary>
        /// Load Departments
        /// </summary>
        private void select()
        {
            cmbDepartment.Items.Add(new ComboBoxItem("0", "--select--"));
            cmbDepartment.SelectedIndex = 0;
            cmbFileType.Items.Add(new ComboBoxItem("0", "--select--"));
            cmbFileType.Items.Add(new ComboBoxItem("1", ".tif"));
            cmbFileType.Items.Add(new ComboBoxItem("2", ".pdf"));
            cmbFileType.SelectedIndex = 0;


        }

        private void LoadDepartments()
        {
            try
            {
                cmbDepartment.DataSource = null;

                Logger.Trace("Started Loading Departments " + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                string Message = string.Empty,
                ConnectionString = ConfigurationManager.AppSettings["DMSInfoSearch_ConnectionString"].ToString(),
                ProcedureName = "USP_SearchDepartmentById";
                DataSet ds = new DataSet();
                string rData = string.Empty;

                //Create a array list to store parameter(s) with
                spParametersList = string.Empty;
                spArgumentsCollection("@in_iCurrDepartmentId", "0", "int");
                spArgumentsCollection("@docType_iId", cmbProjectType.SelectedValue.ToString(), "varchar");
                spArgumentsCollection("@in_vDepartmentName", string.Empty, "varchar");
                spArgumentsCollection("@in_vCreaedDateFrom", string.Empty, "varchar");
                spArgumentsCollection("@in_vCreaedDateTo", string.Empty, "varchar");
                spArgumentsCollection("@in_vAction", "DepartmentsForWindows", "varchar");
                spArgumentsCollection("@in_vLoginToken", LoginToken, "varchar");
                spArgumentsCollection("@in_iLoginOrgId", OrgID, "varchar");

                Crypt crypt = new Crypt();
                ServiceReference1.IService cleintupload = new ServiceReference1.ServiceClient();
                ServiceReference1.StoredProcedureReturnsDataset contractInfo = new ServiceReference1.StoredProcedureReturnsDataset()
                {
                    ConnectionString_Encrypted = crypt.Encrypt(ConnectionString),
                    ProcedureName_Encrypted = crypt.Encrypt(ProcedureName),
                    ParametersList_Encrypted = crypt.Encrypt(spParametersList)
                };
                rData = cleintupload.RunStoredProcedureReturnsDataset(contractInfo);

                DataSet DS = DSXML.ConvertXMLToDataSet(rData);
                if (DS != null && DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 1)
                {
                    Logger.Trace("Departments DS binding to combobox " + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                    cmbDepartment.BindingContext = new BindingContext();
                    cmbDepartment.DataBindings.Clear();

                    cmbDepartment.DataSource = DS.Tables[0];
                    cmbDepartment.DisplayMember = "DepartmentName";
                    cmbDepartment.ValueMember = "DepartmentID";
                }
                else
                {
                    Logger.Trace("Departments DS Empty or Null " + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                    cmbDepartment.BindingContext = new BindingContext();
                    cmbDepartment.DataBindings.Clear();
                    cmbDepartment.DataSource = null;
                    cmbDepartment.Items.Clear();
                    cmbDepartment.Items.Add(new ComboBoxItem("0", "--select--"));
                    cmbDepartment.SelectedIndex = 0;
                }
                Logger.Trace("Finished Loading Departments" + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
            }

            catch (Exception ex)
            {
                Logger.Trace(ex.Message.ToString() + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
            }
        }

        /// <summary>
        /// Load Batchs corresponding to Project Type and Department
        /// </summary>
        private void LoadBatchs()
        {
            try
            {
                Logger.Trace("Started Loading Batchs " + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                if (cmbDepartment.SelectedValue.ToString() != "0" && cmbProjectType.SelectedValue.ToString() != "0")
                {
                    string Message = string.Empty,
                       ConnectionString = ConfigurationManager.AppSettings["DMSInfoSearch_ConnectionString"].ToString(),
                       ProcedureName = "USP_BulkUpload_ManageBulkUpload";
                    DataSet ds = new DataSet();
                    string rData = string.Empty;

                    //Create a array list to store parameter(s) with
                    spParametersList = string.Empty;
                    spArgumentsCollection("@in_iDocTypeId", cmbProjectType.SelectedValue.ToString(), "varchar");
                    spArgumentsCollection("@in_iDepartmentID", cmbDepartment.SelectedValue.ToString(), "varchar");
                    spArgumentsCollection("@in_iBatchId", "0", "int");
                    spArgumentsCollection("@FileType", string.Empty, "varchar");
                    spArgumentsCollection("@in_vAction", "GetBatches", "varchar");
                    spArgumentsCollection("@in_vLoginToken", LoginToken, "varchar");
                    spArgumentsCollection("@in_iLoginOrgId", OrgID, "varchar");
                    spArgumentsCollection("@in_vBulkUploadPath", string.Empty, "varchar");
                    spArgumentsCollection("@in_bBulkUploadProcessed", "0", "int");

                    spArgumentsCollection("@out_vMessage", string.Empty, "varchar");
                    spArgumentsCollection("@out_iErrorState", "0", "int");
                    spArgumentsCollection("@out_iErrorSeverity", "0", "int");

                    Crypt crypt = new Crypt();
                    ServiceReference1.IService cleintupload = new ServiceReference1.ServiceClient();
                    ServiceReference1.StoredProcedureReturnsDataset contractInfo = new ServiceReference1.StoredProcedureReturnsDataset()
                    {
                        ConnectionString_Encrypted = crypt.Encrypt(ConnectionString),
                        ProcedureName_Encrypted = crypt.Encrypt(ProcedureName),
                        ParametersList_Encrypted = crypt.Encrypt(spParametersList)
                    };
                    rData = cleintupload.RunStoredProcedureReturnsDataset(contractInfo);

                    DataSet DS = DSXML.ConvertXMLToDataSet(rData);
                    if (DS != null && DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0)
                    {
                        Logger.Trace("Batchs DS binding to combobox " + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                        cmbBatch.BindingContext = new BindingContext();
                        cmbBatch.DataSource = DS.Tables[0];
                        cmbBatch.DisplayMember = "TextField";
                        cmbBatch.ValueMember = "ValueField";
                    }
                    else
                    {
                        Logger.Trace("Batchs DS Empty or Null " + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                        cmbBatch.BindingContext = new BindingContext();
                        cmbBatch.DataBindings.Clear();
                        cmbBatch.DataSource = null;
                        cmbBatch.Items.Clear();
                        cmbBatch.Items.Add(new ComboBoxItem("0", "--select--"));
                        cmbBatch.SelectedIndex = 0;
                    }

                }
                else
                {
                    cmbBatch.BindingContext = new BindingContext();
                    cmbBatch.DataSource = null;
                    cmbBatch.DataBindings.Clear();
                    cmbBatch.Items.Clear();
                    cmbBatch.Items.Add(new ComboBoxItem("0", "--select--"));
                    cmbBatch.SelectedIndex = 0;
                }
                Logger.Trace("Finished Loading Batchs " + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
            }
            catch (Exception ex)
            {
                Logger.Trace(ex.Message.ToString() + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
            }
        }

        /// <summary>
        /// Method used to concatenate SP arguments
        /// </summary>
        /// <param name="spParmName"></param>
        /// <param name="spParmValue"></param>
        /// <param name="spPramValueType"></param>
        /// <returns></returns>
        public static string spArgumentsCollection(string spParmName, string spParmValue, string spPramValueType)
        {
            spParametersList += "[" + spParmName + "|" + spParmValue + "|" + spPramValueType + "]";
            return spParametersList;
        }

        /// <summary>
        /// To get the Login OrgID and LoginToken after installing the Windows App from Web Application
        /// </summary>
        /// <returns></returns>
        private NameValueCollection GetQueryStringParameters()
        {
            NameValueCollection nameValueTable = new NameValueCollection();

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                string queryString = ApplicationDeployment.CurrentDeployment.ActivationUri.Query;
                nameValueTable = HttpUtility.ParseQueryString(queryString);
            }

            return (nameValueTable);
        }

        /// <summary>
        /// To fill the combobox
        /// </summary>
        public class ComboBoxItem
        {
            public string Value;
            public string Text;
            public ComboBoxItem(string val, string text)
            {
                Value = val;
                Text = text;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        /// <summary>
        /// Project Type Combobox Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbProjectType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            LoadDepartments();

            cmbBatch.BindingContext = new BindingContext();
            cmbBatch.DataSource = null;
            cmbBatch.Items.Clear();
            cmbBatch.Items.Add(new ComboBoxItem("0", "--select--"));
            cmbBatch.SelectedIndex = 0;
            ProjectID = "";
            DepartmentID = "";
            BatchName = "";
        }

        /// <summary>
        /// Department Combobox Selected Index Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbDepartment_SelectionChangeCommitted(object sender, EventArgs e)
        {

            if (cmbDepartment.SelectedValue.ToString() != "0" && cmbProjectType.SelectedValue.ToString() != "0")
            {
                LoadBatchs();
                ProjectID = cmbProjectType.SelectedValue.ToString();
                DepartmentID = cmbDepartment.SelectedValue.ToString();
            }
            else
            {
                cmbBatch.BindingContext = new BindingContext();
                cmbBatch.DataSource = null;
                cmbBatch.Items.Clear();
                cmbBatch.Items.Add(new ComboBoxItem("0", "--select--"));
                cmbBatch.SelectedIndex = 0;
                ProjectID = "";
                DepartmentID = "";
                BatchName = "";
            }
        }

        /// <summary>
        /// To Browse and Select the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = FolderLocator.ShowDialog();
            if (result == DialogResult.OK)
            {
                OpenDirectory = FolderLocator.SelectedPath.ToString();
                txtFolderLocation.Text = OpenDirectory;
                lblStatus.Text = OpenDirectory;
                FolderLocation = FolderLocator.SelectedPath.ToString();
            }
        }

        /// <summary>
        /// Upload button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Started Processing on upload Button Click", "Token_" + LoginToken + "-OrgID_" + OrgID);
                if (cmbProjectType.SelectedValue.ToString() == null || cmbProjectType.SelectedValue.ToString() == "0" || cmbProjectType.SelectedValue.ToString() == "")
                {
                    MessageBox.Show("Please select Project Type", "Important Message");
                    cmbProjectType.Focus();
                }
                else if (cmbDepartment.SelectedValue.ToString() == null || cmbDepartment.SelectedValue.ToString() == "0" || cmbDepartment.SelectedValue.ToString() == "")
                {
                    MessageBox.Show("Please select Department", "Important Message");
                    cmbDepartment.Focus();
                }
                else if (cmbBatch.SelectedValue.ToString() == null || cmbBatch.SelectedValue.ToString() == "0" || cmbBatch.SelectedValue.ToString() == "")
                {
                    MessageBox.Show("Please select Batch", "Important Message");
                    cmbBatch.Focus();
                }
                else if (cmbFileType.SelectedItem.ToString() == null || cmbFileType.SelectedItem.ToString() == "0" || cmbFileType.SelectedItem.ToString() == string.Empty)
                {
                    MessageBox.Show("Please select a File Type", "Important Message");
                    cmbFileType.Focus();
                }
                else if (txtFolderLocation.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Please select folder", "Important Message");
                    txtFolderLocation.Focus();
                }
                else
                {


                    string Message = string.Empty,
                        ConnectionString = ConfigurationManager.AppSettings["DMSInfoSearch_ConnectionString"].ToString(),
                        ProcedureName = "USP_BulkUpload_ManageBulkUpload";
                    DataSet ds = new DataSet();
                    string rData = string.Empty;

                    //Create a array list to store parameter(s) with
                    spParametersList = string.Empty;
                    spArgumentsCollection("@in_iDocTypeId", cmbProjectType.SelectedValue.ToString(), "varchar");
                    spArgumentsCollection("@in_iDepartmentID", cmbDepartment.SelectedValue.ToString(), "varchar");
                    spArgumentsCollection("@in_iBatchId", cmbBatch.SelectedValue.ToString(), "int");
                    spArgumentsCollection("@FileType", cmbFileType.SelectedItem.ToString(), "varchar");
                    spArgumentsCollection("@in_vAction", "GetDataForBulkUpload", "varchar");
                    spArgumentsCollection("@in_vLoginToken", LoginToken, "varchar");
                    spArgumentsCollection("@in_iLoginOrgId", OrgID, "varchar");
                    spArgumentsCollection("@in_vBulkUploadPath", string.Empty, "varchar");
                    spArgumentsCollection("@in_bBulkUploadProcessed", "0", "int");

                    spArgumentsCollection("@out_vMessage", string.Empty, "varchar");
                    spArgumentsCollection("@out_iErrorState", "0", "int");
                    spArgumentsCollection("@out_iErrorSeverity", "0", "int");

                    Crypt crypt = new Crypt();
                    ServiceReference1.IService cleintupload = new ServiceReference1.ServiceClient();
                    ServiceReference1.StoredProcedureReturnsDataset contractInfo = new ServiceReference1.StoredProcedureReturnsDataset()
                    {
                        ConnectionString_Encrypted = crypt.Encrypt(ConnectionString),
                        ProcedureName_Encrypted = crypt.Encrypt(ProcedureName),
                        ParametersList_Encrypted = crypt.Encrypt(spParametersList)
                    };
                    rData = cleintupload.RunStoredProcedureReturnsDataset(contractInfo);
                    DS = DSXML.ConvertXMLToDataSet(rData);
                    if (DS == null || DS.Tables.Count == 0 || DS.Tables[0].Rows.Count == 0)
                    {
                        MessageBox.Show("Unable to retrive data from server. \n Possible Reasons Are : \n 1. Connectivity Issue. \n 2. File format not defined in project level. \n 3. No valid records present in the selected batch", "Important Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {

                        dirfile = new DirectoryInfo(FolderLocator.SelectedPath.ToString());
                        pdffilesforcount = dirfile.GetFiles("*.pdf");
                        tiffilesforcount = dirfile.GetFiles("*.tif");
                        pdffiles = dirfile.GetFiles("*.pdf");
                        tiffiles = dirfile.GetFiles("*.tif");

                        DS.Tables[0].DefaultView.RowFilter = "[FileNames] LIKE '%.pdf'";
                        PdfTable = DS.Tables[0].DefaultView.ToTable();
                        if (PdfTable.Rows.Count > 0)
                        {
                            PdfTable.PrimaryKey = new DataColumn[1] { PdfTable.Columns[0] };
                        }
                        DS.Tables[0].DefaultView.RowFilter = "[FileNames] LIKE '%.tif'";
                        TifTable = DS.Tables[0].DefaultView.ToTable();
                        if (TifTable.Rows.Count > 0)
                        {
                            TifTable.PrimaryKey = new DataColumn[1] { TifTable.Columns[0] };
                        }

                        if (PdfTable.Rows.Count > 0 && pdffiles.Length > 0)
                        {
                            for (int i = 0; i < pdffiles.Length; i++)
                            {
                                DataRow Row = PdfTable.Rows.Find(pdffiles[i].Name);
                                if (Row == null)
                                {
                                    if (!Directory.Exists(FolderLocator.SelectedPath.ToString() + "\\Unmatched Files"))
                                    {
                                        Directory.CreateDirectory(FolderLocator.SelectedPath.ToString() + "\\Unmatched Files");
                                    }
                                    try
                                    {
                                        pdffiles[i].MoveTo(FolderLocator.SelectedPath.ToString() + "\\Unmatched Files\\" + pdffiles[i].Name);
                                        Logger.TraceErrorLog("Moving UnMatched PDF FILE" + " (From batch -" + cmbBatch.SelectedValue.ToString() + ")-" + FolderLocator.SelectedPath.ToString() + "\\Unmatched Files\\" + pdffiles[i].Name);
                                        Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + pdffiles[i].Name + "Unmatched Move Success");
                                    }
                                    catch (IOException ex)
                                    {
                                        Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + pdffiles[i].Name + "Unmatched Move Failed");
                                        throw new Exception(ex.Message);
                                    }
                                }
                            }
                        }
                        else if (pdffiles.Length > 0)
                        {
                            for (int i = 0; i < pdffiles.Length; i++)
                            {
                                try
                                {
                                    pdffiles[i].MoveTo(FolderLocator.SelectedPath.ToString() + "\\Unmatched Files\\" + pdffiles[i].Name);
                                    Logger.TraceErrorLog("Moving UnMatched PDF FILE" + " (From batch -" + cmbBatch.SelectedValue.ToString() + ")-" + FolderLocator.SelectedPath.ToString() + "\\Unmatched Files\\" + pdffiles[i].Name);
                                    Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + pdffiles[i].Name + "Unmatched Move Success");
                                }
                                catch (IOException ex)
                                {
                                    Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + pdffiles[i].Name + "Unmatched Move Failed");
                                    throw new Exception(ex.Message);
                                }
                            }
                        }

                        if (TifTable.Rows.Count > 0 && tiffiles.Length > 0)
                        {
                            for (int i = 0; i < tiffiles.Length; i++)
                            {
                                DataRow Row = TifTable.Rows.Find(tiffiles[i].Name);
                                if (Row == null)
                                {
                                    if (!Directory.Exists(FolderLocator.SelectedPath.ToString() + "\\Unmatched Files"))
                                    {
                                        Directory.CreateDirectory(FolderLocator.SelectedPath.ToString() + "\\Unmatched Files");
                                    }
                                    try
                                    {
                                        tiffiles[i].MoveTo(FolderLocator.SelectedPath.ToString() + "\\Unmatched Files\\" + tiffiles[i].Name);
                                        Logger.TraceErrorLog("Moving UnMatched tif FILE where TifTable.Rows.Count > 0 && tiffiles.Length > 0" + " (From batch -" + cmbBatch.SelectedValue.ToString() + ")-" + "-" + FolderLocator.SelectedPath.ToString() + "\\Unmatched Files\\" + tiffiles[i].Name);
                                        Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + tiffiles[i].Name + "Unmatched Move Success");
                                    }
                                    catch (IOException ex)
                                    {
                                        Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + tiffiles[i].Name + "Unmatched Move Failed");
                                        throw new Exception(ex.Message);
                                    }
                                }
                            }
                        }
                        else if (tiffiles.Length > 0)
                        {
                            for (int i = 0; i < tiffiles.Length; i++)
                            {
                                try
                                {
                                    tiffiles[i].MoveTo(FolderLocator.SelectedPath.ToString() + "\\Unmatched Files\\" + tiffiles[i].Name);
                                    Logger.TraceErrorLog("Moving UnMatched tif FILE where tiffiles.Length > 0" + " (From batch -" + cmbBatch.SelectedValue.ToString() + ")-" + "-" + FolderLocator.SelectedPath.ToString() + "\\Unmatched Files\\" + tiffiles[i].Name);
                                    Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + tiffiles[i].Name + "Unmatched Move Success");
                                }
                                catch (IOException ex)
                                {
                                    Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + tiffiles[i].Name + "Unmatched Move Failed");
                                    throw new Exception(ex.Message);
                                }
                            }
                        }


                        pdffiles = dirfile.GetFiles("*.pdf");
                        tiffiles = dirfile.GetFiles("*.tif");

                        if (pdffiles.Length == 0 && tiffiles.Length == 0)
                        {
                            MessageBox.Show("Zero Matching Files found. \n\n\n1. PDF files in batch `" + cmbBatch.Text + "`  is " + PdfTable.Rows.Count +
                                            ", Matching PDF files found in selected folder is  " + pdffiles.Length.ToString() +
                                            " out of " + pdffilesforcount.Length.ToString() + " files. \n\n\n" +
                                            "TIF files in batch `" + cmbBatch.Text + "`  is " + TifTable.Rows.Count +
                                            ", Matching TIF files found in selected folder is  " + tiffiles.Length.ToString() +
                                            " out of " + tiffiles.Length.ToString() + " files. \n\n\n", "Information");
                        }
                        else if (
                            MessageBox.Show("PDF files in batch `" + cmbBatch.Text + "`  is " + PdfTable.Rows.Count +
                                            ", Matching PDF files found in selected folder is  " + pdffiles.Length.ToString() +
                                            " out of " + pdffilesforcount.Length.ToString() + " files. \n\n\n" +
                                            "TIF files in batch '" + cmbBatch.Text + "' is " + TifTable.Rows.Count +
                                            ", Matching TIF files in selected folder :  " + tiffiles.Length.ToString() +
                                            " out of " + tiffilesforcount.Length.ToString() + " files. \n\n\n" +
                                            " Do you wish to continue?", "Confirm Continue", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            string[] extensions = new[] { ".pdf", ".tif" };

                            BatchName = cmbBatch.SelectedValue.ToString();

                            SaveFolder = ConfigurationManager.AppSettings["UploadFolder"].ToString() + LoginToken;
                            SaveFolder = SaveFolder + "\\" + DateTime.Now.ToString("HH:mm:ss tt").ToString().Replace(":", "").Replace(" ", "") + "\\";


                            //To get the file details for upload
                            FilestoUpload = dirfile.GetFiles()
                              .Where(f => extensions.Contains(f.Extension.ToLower()))
                              .ToArray();

                            //Assigning Threads for Upload
                            CallThreads(FilestoUpload.Length);
                        }
                        Logger.Trace("Finished Processing on upload" + " (From batch -" + cmbBatch.SelectedValue.ToString() + ")-" + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                    }
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), "Error Occured. Please contact administrator");
                Logger.Trace(ex.Message.ToString() + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
            }
        }

        /// <summary>
        /// Function used to intialize the Threads as per the app.config value
        /// </summary>
        public void InitializeThread()
        {
            for (int f = 0; f < maxThreads; f++)
            {
                threadArray[f] = new BackgroundWorker();
                threadArray[f].DoWork +=
                    new DoWorkEventHandler(backgroundWorkerFiles_DoWork);
                threadArray[f].RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(backgroundWorkerFiles_RunWorkerCompleted);
                threadArray[f].ProgressChanged +=
                    new ProgressChangedEventHandler(backgroundWorkerFiles_ProgressChanged);
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
            UploadFile(myProcessArguments);

            int progress = 0, findPercentage = 0;

            findPercentage = ((myProcessArguments + 1) * 100) / (FilestoUpload.Length);
            progress = 0;
            progress += findPercentage;
            //Report back to the UI
            string progressStatus = "Uploading file " + (myProcessArguments + 1).ToString() + " of " + (FilestoUpload.Length);
            ((BackgroundWorker)se).ReportProgress(progress, progressStatus);


        }

        /// <summary>
        /// To update the Progress Bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerFiles_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Use this method to report progress to GUI
            this.progressBar.Maximum = 100;
            this.progressBar.Minimum = 0;
            this.progressBar.Value = e.ProgressPercentage;
            lblStatus.Text = e.UserState as String;
            lblPercentage.Text = String.Format("Total Progress: {0} %", e.ProgressPercentage);
            progressPanel.Visible = true;
            controlsPanel.Enabled = false;

            if (Convert.ToInt32(e.ProgressPercentage) == 100)
            {
                lblStatus.Text = "All the files have been uploaded successfully.";
                lblPercentage.Text = String.Format("Total Progress: {0} %", e.ProgressPercentage);
                controlsPanel.Enabled = true;
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
        /// Function which will be calling the threads
        /// </summary>
        /// <param name="g"></param>
        private void CallThreads(int g)
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
                            Logger.Trace("Calling Thread(" + threadNum + ") with value of f as  " + f.ToString() + ", to upload file " + FilestoUpload[f].Name, "Token_" + LoginToken + "-OrgID_" + OrgID);
                            break;
                        }
                    }
                    //If all threads are being used, sleep awhile before checking again
                    if (!fileProcessed)
                    {
                        Thread.Sleep(5000);
                        Application.DoEvents();
                    }
                }
            }
        }

        /// <summary>
        /// Function to Upload File to Server
        /// </summary>
        /// <param name="f"></param>
        private static void UploadFile(int f)
        {
            try
            {
                Crypt crypt = new Crypt();
                //Converting the File to string base64
                byte[] bytes = System.IO.File.ReadAllBytes(FilestoUpload[f].FullName);
                string Data = Convert.ToBase64String(bytes);

                string ProcessID = string.Empty;

                if (!Directory.Exists(FolderLocation + "\\Uploaded"))
                {
                    Directory.CreateDirectory(FolderLocation + "\\Uploaded");
                }

                if (FilestoUpload[f].Extension.ToLower() == ".pdf")
                {
                    DataRow Row = PdfTable.Rows.Find(FilestoUpload[f].Name);
                    if (Row != null)
                    {
                        ProcessID = Row["ProcessID"].ToString();
                    }
                }
                else if (FilestoUpload[f].Extension.ToLower() == ".tif" || FilestoUpload[f].Extension == ".tiff")
                {
                    DataRow Row = TifTable.Rows.Find(FilestoUpload[f].Name);
                    if (Row != null)
                    {
                        ProcessID = Row["ProcessID"].ToString();
                    }
                }
                //Call REST and get a response back

                string Res = string.Empty;
                try
                {
                    ServiceReference2.FileUploadServiceClient x = new ServiceReference2.FileUploadServiceClient();
                    Res = x.UploadFile(SaveFolder + FilestoUpload[f].Name, Data);


                    if (Res != null && Res.Contains("SUCCESS"))
                    {
                        Logger.TraceErrorLog("Upload success for FILE" + " (From batch -" + BatchName + ")-" + "-" + FolderLocation + "\\" + FilestoUpload[f].Name);
                        Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + FilestoUpload[f].Name + " Upload Success");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Trace(ex.Message.ToString() + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                    Logger.TraceErrorLog("Upload failed for FILE" + " (From batch -" + BatchName + ")-" + "-" + FolderLocation + "\\" + FilestoUpload[f].Name);
                    Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + FilestoUpload[f].Name + " Upload Fail");
                    throw new Exception(ex.Message);
                }

                try
                {
                    //Save to DTD_BulkUpload table
                    if (Res != null && Res.Contains("SUCCESS"))
                    {
                        string Message = string.Empty,
                             ConnectionString = ConfigurationManager.AppSettings["DMSInfoSearch_ConnectionString"].ToString(),
                             ProcedureName = "USP_BulkUpload_ManageBulkUpload";
                        DataSet ds = new DataSet();
                        string rData = string.Empty;

                        //Create a array list to store parameter(s) with
                        spParametersList = string.Empty;
                        spArgumentsCollection("@in_iDocTypeId", ProcessID, "varchar");
                        spArgumentsCollection("@in_iDepartmentID", "0", "varchar");
                        spArgumentsCollection("@in_iBatchId", "0", "int");
                        spArgumentsCollection("@FileType", string.Empty, "varchar");
                        spArgumentsCollection("@in_vAction", "UPLOAD", "varchar");
                        spArgumentsCollection("@in_vLoginToken", LoginToken, "varchar");
                        spArgumentsCollection("@in_iLoginOrgId", OrgID, "varchar");
                        spArgumentsCollection("@in_vBulkUploadPath", SaveFolder + FilestoUpload[f].Name, "varchar");
                        spArgumentsCollection("@in_bBulkUploadProcessed", "0", "int");

                        spArgumentsCollection("@out_vMessage", string.Empty, "varchar");
                        spArgumentsCollection("@out_iErrorState", "0", "int");
                        spArgumentsCollection("@out_iErrorSeverity", "0", "int");

                        ServiceReference1.IService cleintupload = new ServiceReference1.ServiceClient();
                        ServiceReference1.StoredProcedureReturnsDataset contractInfo = new ServiceReference1.StoredProcedureReturnsDataset()
                        {
                            ConnectionString_Encrypted = crypt.Encrypt(ConnectionString),
                            ProcedureName_Encrypted = crypt.Encrypt(ProcedureName),
                            ParametersList_Encrypted = crypt.Encrypt(spParametersList)
                        };
                        rData = cleintupload.RunStoredProcedureReturnsDataset(contractInfo);

                        DS = DSXML.ConvertXMLToDataSet(rData);

                        if (DS != null && DS.Tables.Count > 0 && DS.Tables[0].Rows.Count > 0 && DS.Tables[0].Rows[0]["ActionStatus"].ToString() == "SUCCESS")
                        {
                            Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + FilestoUpload[f].Name + " Detail Update Success");
                            File.Move(FolderLocation + "\\" + FilestoUpload[f].Name, FolderLocation + "\\Uploaded\\" + FilestoUpload[f].Name);
                            Logger.TraceErrorLog("Moved successful FILE" + " (From batch -" + BatchName + ")-" + "-" + FolderLocation + "\\Uploaded\\" + FilestoUpload[f].Name);
                            Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + FilestoUpload[f].Name + " Move Success");
                        }
                    }
                }
                catch (IOException ex)
                {
                    Logger.Trace(ex.Message.ToString() + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                    Logger.TraceErrorLog("File move failed for FILE" + " (From batch -" + BatchName + ")-" + "-" + FolderLocation + "\\" + FilestoUpload[f].Name);
                    Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + FilestoUpload[f].Name + " Move Fail");
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    Logger.Trace(ex.Message.ToString() + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                    Logger.TraceErrorLog("Service call failed for FILE" + " (From batch -" + BatchName + ")-" + "-" + FolderLocation + "\\" + FilestoUpload[f].Name);
                    Logger.TraceFileStatus("FILE" + " (From batch -" + BatchName + ")- " + " - " + FilestoUpload[f].Name + " Detail Update Fail");
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.Trace(ex.Message.ToString() + "Token_" + LoginToken + "-OrgID_" + OrgID, "Token_" + LoginToken + "-OrgID_" + OrgID);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cmbBatch_SelectionChangeCommitted(object sender, EventArgs e)
        {
            BatchName = cmbBatch.SelectedValue.ToString();
        }

        private string Serialize(string fileName)
        {
            using (FileStream reader = new FileStream(fileName, FileMode.Open))
            {
                byte[] buffer = new byte[reader.Length];
                reader.Read(buffer, 0, (int)reader.Length);
                return Convert.ToBase64String(buffer);
            }
        }

    }


}
