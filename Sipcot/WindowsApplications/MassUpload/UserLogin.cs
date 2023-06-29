using System;
using System.Windows.Forms;
using System.Reflection;

namespace MassUpload
{
    public partial class UserLogin : Form
    {
        public UserLogin()
        {
            InitializeComponent();

            string AppName = string.Empty, AppVersion = string.Empty;
            //Show application name and version
            // Application Name
            AppName = Assembly.GetExecutingAssembly().GetName().Name.ToString();
            // Assembly version
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            lblVersion.Text = AppName + " " + AppVersion;

            Logger.logTrace("Login - " + AppName + " " + AppVersion, Global.UserName);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.logTrace("Login - User clicked login button.", Global.UserName);
                string UserName = txtUsername.Text.Trim().ToString();
                string Password = txtPassword.Text.Trim().ToString();
                if (UserName != string.Empty && Password != string.Empty)
                {
                    Login(UserName, Password);
                }
                else
                {
                    MessageBox.Show("Username and Password cannot be empty!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Logger.logException(ex, Global.UserName);
            }
        }

        private void Login(string UserName, string Password)
        {
            try
            {
                string ErrorState = string.Empty, Message = string.Empty, Channel = string.Empty, FileUploadPath = string.Empty;

                Logger.logTrace("Login - Calling service for authentication.", Global.UserName);
                ServiceReference1.ITransferService cleintupload = new ServiceReference1.TransferServiceClient();
                ServiceReference1.Authentication AuthenticationRequestInfo = new ServiceReference1.Authentication()
                {
                    UserName = UserName,
                    Passwword = Password
                };

                string response = cleintupload.Aunthenticate(AuthenticationRequestInfo);

                Logger.logTrace("Login - Service response: " + response, Global.UserName);

                // Split string by pipeline(|).
                string[] DataWithKeyValue = response.Split('|');
                foreach (string data in DataWithKeyValue)
                {
                    string key = string.Empty, value = string.Empty;

                    string[] KeyValue = data.Split('~');

                    if (KeyValue.Length > 0)
                        key = KeyValue[0];
                    if (KeyValue.Length > 1)
                        value = KeyValue[1];

                    switch (key)
                    {
                        case "ErrorState":
                            ErrorState = value;
                            break;
                        case "Message":
                            Message = value;
                            break;
                        case "Channel":
                            Channel = value;
                            break;
                        case "FileUploadPath":
                            FileUploadPath = value;
                            break;
                        default:
                            break;
                    }

                }

                if (ErrorState == "0")
                {
                    Logger.logTrace("Login - Aunthentication success.", Global.UserName);
                    Global.UserName = UserName;
                    Global.Password = Password;
                    Global.Channel = Channel;
                    Global.FileUploadPath = FileUploadPath;

                    this.Hide();
                    Upload upload = new Upload();
                    upload.ShowDialog();
                }
                else if (ErrorState == "-1")
                {
                    Logger.logTrace("Login - " + Message, Global.UserName);
                    MessageBox.Show(Message, "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    Logger.logTrace("Login - " + Message, Global.UserName);
                    MessageBox.Show(Message, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (System.Threading.ThreadAbortException)
            {
                // Exception caught from Upload form on form closing: due to thread abort.
                MessageBox.Show("Upload is aborted by user."+
                Environment.NewLine + Environment.NewLine + "Tip: You can resume upload on next run.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Logger.logException(ex, Global.UserName);
                MessageBox.Show("Login - Error occured! Please contact administrator.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Return")
                btnLogin_Click(sender, e);
        }
    }
}
