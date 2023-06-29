using System;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormESAF_SERVICE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            panelDocDetailsInput.Visible = true;
        }

        private void getDocumentInfo()
        {
            DocumentService.ServiceData docData = new DocumentService.ServiceData()
            {
                API_Key = "AEAD774B-C0E1-433D-BBFF-EFD3D327D73D",
                Method = "GetDocumentInfo"
            };

            string separator = ",";
            docData.Input += "Project Type = " + txtProjectType.Text.Trim();
            docData.Input += separator + "Department = " + txtDepartment.Text.Trim();

            // Invoke service
            docData = new DocumentService.DocumentClient().GetDocumentInfo(docData);

            HandleResult(docData);
        }

        private void GetDocument()
        {
            DocumentService.ServiceData docData = new DocumentService.ServiceData()
            {
                // Manadatory properties initialization
                API_Key = "AEAD774B-C0E1-433D-BBFF-EFD3D327D73D",
                Method = "DownloadDocument",
            };

            // Service inputs
            string separator = ",";
            docData.Input += "Document Id = " + txtDocumentId.Text;
            docData.Input += separator + "Document Type = CAF";
            docData.Input += separator + "Department = IT";

            // Invoke service method
            docData = new DocumentService.DocumentClient().DownloadDocument(docData);

            HandleResult(docData);
        }

        private void btnGetDocument_Click(object sender, EventArgs e)
        {
            panelDownloadDocInput.Visible = true;
        }

        void HandleResult(DocumentService.ServiceData docData)
        {
            txtErrorState.Text = docData.ErrorState.ToString();
            txtErrorSeverity.Text = docData.ErrorSeverity.ToString();
            txtMessage.Text = docData.Message;

            rtbOutputParamValues.Text = docData.OutputParamValues;

            // control appearance 
            if (txtErrorState.Text == "1")
            {
                txtErrorState.BackColor = txtMessage.BackColor = System.Drawing.Color.Pink;
            }
            else if (txtErrorState.Text == "-1")
            {
                txtErrorState.BackColor = txtMessage.BackColor = System.Drawing.Color.Yellow;
            }
            else
            {
                txtErrorState.BackColor = txtMessage.BackColor = txtErrorSeverity.BackColor;
            }

            if (docData.Resultsets != null && docData.Resultsets.Tables.Count > 0)
                dataGridView1.DataSource = docData.Resultsets.Tables[0];

            // Document download result
            if (docData.FileContent != null && docData.FileContent.Length > 0)
            {
                byte[] data = System.Convert.FromBase64String(docData.FileContent);

                string fileSavePath = Path.Combine(@"C:\GenAPI\DownloadedDocuments", Path.GetFileName(docData.FilePath));
                File.WriteAllBytes(fileSavePath, data);

                System.Diagnostics.Process.Start(fileSavePath);

                //MemoryStream ms = new MemoryStream(data);
                //pbDocument.Image = System.Drawing.Image.FromStream(ms);
            }
        }

        //Service helper methods
        /// <summary>
        /// This function built input data, which is collection of some service input's Name, Value,  Data type and direction.
        /// </summary>       
        /// <param name="spParmName">Input Name.</param>
        /// <param name="spParmValue">Input Value.</param>
        /// <param name="spPramValueType">Parameter value type EXACTLY same as SqlDBType. E.g. 'SqlDbType.BigInt' will 'BigInt'. </param>
        /// <returns></returns>
        public string InputCollection(string InputName, string InputValue,
            string InputValueType, string Direction = "")
        {
            return "[" + InputName + "|" + InputValue + "|" + InputValueType +
                "|" + Direction + "]";
        }

        private void btnOkDocDetails_Click(object sender, EventArgs e)
        {
            getDocumentInfo();
            panelDocDetailsInput.Visible = false;
        }

        private void btnOKGetDocument_Click(object sender, EventArgs e)
        {
            GetDocument();
            panelDownloadDocInput.Visible = false;
        }

    }

}
