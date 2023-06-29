using System;
using System.Web.UI;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Net;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public enum DataProvider
    {
        Oracle, SqlServer, OleDb, Odbc, MySql
    }

    public partial class ConfigurationPage : System.Web.UI.Page
    {

        #region Property declaration
        protected string ConnectionString
        {
            get { return Convert.ToString(ViewState["ConnectionString"]); }
            set { ViewState["ConnectionString"] = value; }
        }

        protected string DatabasePassword
        {
            get { return Convert.ToString(ViewState["DatabasePassword"]); }
            set { ViewState["DatabasePassword"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadSettingsToUI();
                // Call database system index change event to show/hide controls as per database system selection
                ddlDatabaseSystem_SelectedIndexChanged(sender, e);
            }
            lblMessage.Text = string.Empty;
        }

        private bool TestDatabaseConnection()
        {
            bool isConnectionSuccess = false;
            string connectionString = string.Empty;
            DatabasePassword = txtPassword.Text;

            string DatabaseSystem = ddlDatabaseSystem.SelectedItem.Value;
            switch (DatabaseSystem.ToUpper())
            {
                case "SQLSERVER":
                    connectionString = @"Data Source=" + txtServer.Text + ";Initial Catalog=" + txtDatabase.Text + ";Persist Security Info=True;User ID=" + txtUsername.Text + ";Password=" + DatabasePassword + ";";
                    isConnectionSuccess = AppConfiguration.CheckDatabaseMSSQL(connectionString);
                    break;
                case "MYSQL":
                    connectionString = "Data Source=" + txtHostname.Text + ";Port=" + txtPort.Text + ";Database=" + txtDatabase.Text + ";User ID=" + txtUsername.Text + ";Password=" + DatabasePassword + ";";
                    isConnectionSuccess = AppConfiguration.CheckDatabaseMYSQL(connectionString);
                    break;
                default:
                    break;
            }

            // If connection success save connection string in property else clear
            if (isConnectionSuccess)
                ConnectionString = connectionString;
            else
                ConnectionString = string.Empty;

            return isConnectionSuccess;
        }

        protected void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                bool isConnectionSuccess = false;
                isConnectionSuccess = TestDatabaseConnection();
                if (isConnectionSuccess)
                    lblTestConnectionStatus.Text = MessageFormatter.GetFormattedSuccessMessage("Connection to the database success.");
                else
                    lblTestConnectionStatus.Text = MessageFormatter.GetFormattedNoticeMessage("Connection to the database failed.");
            }
            catch(Exception ex)
            {
                lblTestConnectionStatus.Text = MessageFormatter.GetFormattedErrorMessage(ex.Message);
            }
        }

        protected void ddlDatabaseSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            string DatabaseSystem = ddlDatabaseSystem.SelectedItem.Value;
            switch (DatabaseSystem.ToUpper())
            {
                case "SQLSERVER":
                    lblServer.Visible = txtServer.Visible = true;
                    lblHostname.Visible = txtHostname.Visible = false;
                    lblPort.Visible = txtPort.Visible = false;
                    break;
                case "MYSQL":
                    lblHostname.Visible = txtHostname.Visible = true;
                    lblPort.Visible = txtPort.Visible = true;
                    lblServer.Visible = txtServer.Visible = false;
                    break;
                default:
                    break;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private bool SaveSettings()
        {
            bool isSaveSuccess = false;

            try
            {
                // Save connection string
                if (string.Equals(ConnectionString, string.Empty))
                    TestDatabaseConnection();
                if (!string.Equals(ConnectionString, string.Empty))
                {
                    string DatabaseSystem = ddlDatabaseSystem.SelectedItem.Value;
                    ConfigHelper.ModifyAppSetting("DatabaseSystem", DatabaseSystem);
                    ConfigHelper.ModifyConnectionString("SqlServerConnString", ConnectionString);
                }
                else
                {
                    // Connection to the database failed.
                }

                // Uploaded documents path
                ConfigHelper.ModifyAppSetting("ImageLocation", txtUploadedDocumentsPath.Text);
                // Archived documents path
                ConfigHelper.ModifyAppSetting("ArchiveFolder", txtArchivedDocumentsPath.Text);
                // Versioned documents path
                ConfigHelper.ModifyAppSetting("VersionFolder", txtVersionedDocumentsPath.Text);
                // Temporary path
                ConfigHelper.ModifyAppSetting("TempWorkFolder", txtTemporaryPath.Text);

                lblMessage.Text = MessageFormatter.GetFormattedSuccessMessage("Success.");
            }
            catch
            {
                lblMessage.Text = MessageFormatter.GetFormattedErrorMessage("Failed.");
            }

            return isSaveSuccess;
        }

        private void LoadSettingsToUI()
        {
            try
            {
                // Database system
                string DatabaseSystem = ConfigHelper.ReadAppSetting("DatabaseSystem");
                ddlDatabaseSystem.SelectedItem.Selected = false;
                ddlDatabaseSystem.Items.FindByValue(DatabaseSystem).Selected = true;

                //Load database connection settings
                ReadConnectionString(DatabaseSystem);

                // Uploaded documents path
                txtUploadedDocumentsPath.Text = ConfigHelper.ReadAppSetting("ImageLocation");
                txtUploadedDocumentsPath_TextChanged(null, null);

                // Archived documents path
                txtArchivedDocumentsPath.Text = ConfigHelper.ReadAppSetting("ArchiveFolder");
                txtArchivedDocumentsPath_TextChanged(null, null);

                // Versioned documents path
                txtVersionedDocumentsPath.Text = ConfigHelper.ReadAppSetting("VersionFolder");
                txtVersionedDocumentsPath_TextChanged(null, null);

                // Temporary path
                txtTemporaryPath.Text = ConfigHelper.ReadAppSetting("TempWorkFolder");
                txtTemporaryPath_TextChanged(null, null);

                // Bulk Upload Installer Url
                txtBulkUploadInstallerUrl.Text = ConfigHelper.ReadAppSetting("ClickOnceLink");
                txtBulkUploadInstallerUrl_TextChanged(null, null);

                // Document Download Website Url
                txtDocumentDownloadWebsiteUrl.Text = ConfigHelper.ReadAppSetting("DocumentPath");
                txtDocumentDownloadWebsiteUrl_TextChanged(null, null);

                chkEnableTextLog.Checked = Convert.ToBoolean(ConfigHelper.ReadAppSetting("logTraceIsEnabled"));
            }
            catch (Exception ex)
            {
                lblMessage.Text = MessageFormatter.GetFormattedErrorMessage(ex.Message);
            }
        }

        private void ReadConnectionString(string DatabaseSystem)
        {
            string connectionString = ConfigHelper.ReadConnectionString("SqlServerConnString");
            if (connectionString.Length > 0)
            {
                string initialCatalog = string.Empty;
                string dataSource = string.Empty;
                string userId = string.Empty;
                string password = string.Empty;
                string port = string.Empty;

                switch (DatabaseSystem.ToUpper())
                {
                    case "SQLSERVER":
                        initialCatalog = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
                        dataSource = new SqlConnectionStringBuilder(connectionString).DataSource;
                        userId = new SqlConnectionStringBuilder(connectionString).UserID;
                        password = new SqlConnectionStringBuilder(connectionString).Password;
                        break;
                    case "MYSQL":
                        initialCatalog = GetConnectionStringPart("Database", connectionString);
                        dataSource = GetConnectionStringPart("Data Source", connectionString);
                        userId = GetConnectionStringPart("User ID", connectionString);
                        password = GetConnectionStringPart("Password", connectionString);
                        port = GetConnectionStringPart("Port", connectionString);
                        break;
                }

                txtDatabase.Text = initialCatalog;
                txtServer.Text = txtHostname.Text = dataSource;
                txtUsername.Text = userId;
                txtPassword.Text = password;
                txtPort.Text = port;
            }
        }

        private string GetConnectionStringPart(string key, string connectionString)
        {
            string result = string.Empty;
            string evaluation = connectionString;

            evaluation = Regex.Match(connectionString, @"(?<=" + key + "=).*(?=;)").Value;
            if (evaluation.Contains(";"))
                result = evaluation.Replace(key + "=", string.Empty).Substring(0, evaluation.IndexOf(';'));

            return result;
        }

        protected void txtUploadedDocumentsPath_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string textboxValue = txtUploadedDocumentsPath.Text;
                bool isValid = IOUtility.ValidatePath(textboxValue);
                if (isValid)
                    lblUploadedDocumentsPathStatus.Text = MessageFormatter.GetFormattedSuccessMessage("Validated");
                else
                    lblUploadedDocumentsPathStatus.Text = MessageFormatter.GetFormattedNoticeMessage("Invalid path!");
            }
            catch (Exception ex)
            {
                lblUploadedDocumentsPathStatus.Text = MessageFormatter.GetFormattedErrorMessage(ex.Message);
            }
        }

        protected void txtArchivedDocumentsPath_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string textboxValue = txtArchivedDocumentsPath.Text;
                bool isValid = IOUtility.ValidatePath(textboxValue);
                if (isValid)
                    lblArchivedDocumentsPathStatus.Text = MessageFormatter.GetFormattedSuccessMessage("Validated");
                else
                    lblArchivedDocumentsPathStatus.Text = MessageFormatter.GetFormattedNoticeMessage("Invalid path!");
            }
            catch (Exception ex)
            {
                lblArchivedDocumentsPathStatus.Text = MessageFormatter.GetFormattedErrorMessage(ex.Message);
            }
        }

        protected void txtVersionedDocumentsPath_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string textboxValue = txtVersionedDocumentsPath.Text;
                bool isValid = IOUtility.ValidatePath(textboxValue);
                if (isValid)
                    lblVersionedDocumentsPathStatus.Text = MessageFormatter.GetFormattedSuccessMessage("Validated");
                else
                    lblVersionedDocumentsPathStatus.Text = MessageFormatter.GetFormattedNoticeMessage("Invalid path!");
            }
            catch (Exception ex)
            {
                lblVersionedDocumentsPathStatus.Text = MessageFormatter.GetFormattedErrorMessage(ex.Message);
            }
        }

        protected void txtTemporaryPath_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string textboxValue = txtTemporaryPath.Text;
                bool isValid = IOUtility.ValidatePath(textboxValue);
                if (isValid)
                    lblTemporaryPathStatus.Text = MessageFormatter.GetFormattedSuccessMessage("Validated");
                else
                    lblTemporaryPathStatus.Text = MessageFormatter.GetFormattedNoticeMessage("Invalid path!");
            }
            catch (Exception ex)
            {
                lblTemporaryPathStatus.Text = MessageFormatter.GetFormattedErrorMessage(ex.Message);
            }
        }

        protected bool CheckUrlExists(string url)
        {
            // If the url does not contain Http. Add it.
            if (!url.Contains("http://"))
            {
                url = "http://" + url;
            }
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }

        protected void txtBulkUploadInstallerUrl_TextChanged(object sender, EventArgs e)
        {
            bool isValid = CheckUrlExists(txtBulkUploadInstallerUrl.Text);
            if (isValid)
                lblBulkUploadInstallerUrlStatus.Text = MessageFormatter.GetFormattedSuccessMessage("Valid");
            else
                lblBulkUploadInstallerUrlStatus.Text = MessageFormatter.GetFormattedErrorMessage("Invalid!");
        }

        protected void txtDocumentDownloadWebsiteUrl_TextChanged(object sender, EventArgs e)
        {
            bool isValid = CheckUrlExists(txtDocumentDownloadWebsiteUrl.Text);
            if (isValid)
                lblDocumentDownloadWebsiteUrlStatus.Text = MessageFormatter.GetFormattedSuccessMessage("Valid");
            else
                lblDocumentDownloadWebsiteUrlStatus.Text = MessageFormatter.GetFormattedErrorMessage("Invalid!");
        }

    }
}
