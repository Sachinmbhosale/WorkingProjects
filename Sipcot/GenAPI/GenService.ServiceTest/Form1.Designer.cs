


namespace WindowsFormESAF_SERVICE
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button8 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblErrorState = new System.Windows.Forms.Label();
            this.lblErrorSeverity = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblTitle = new System.Windows.Forms.ToolStripLabel();
            this.pnlServices = new System.Windows.Forms.Panel();
            this.btnGetDocument = new System.Windows.Forms.Button();
            this.pnlOutputStatus = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtErrorSeverity = new System.Windows.Forms.TextBox();
            this.txtErrorState = new System.Windows.Forms.TextBox();
            this.rtbOutputParamValues = new System.Windows.Forms.RichTextBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.panelDownloadDocInput = new System.Windows.Forms.Panel();
            this.btnOKGetDocument = new System.Windows.Forms.Button();
            this.txtDocumentId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panelDocDetailsInput = new System.Windows.Forms.Panel();
            this.btnOkDocDetails = new System.Windows.Forms.Button();
            this.txtDepartment = new System.Windows.Forms.TextBox();
            this.txtProjectType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.lblResultsetTitle = new System.Windows.Forms.ToolStripLabel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.pnlServices.SuspendLayout();
            this.pnlOutputStatus.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panelDownloadDocInput.SuspendLayout();
            this.panelDocDetailsInput.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(20, 21);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(149, 23);
            this.button8.TabIndex = 11;
            this.button8.Text = "Get Document Details";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(610, 225);
            this.dataGridView1.TabIndex = 12;
            // 
            // lblErrorState
            // 
            this.lblErrorState.AutoSize = true;
            this.lblErrorState.Location = new System.Drawing.Point(17, 12);
            this.lblErrorState.Name = "lblErrorState";
            this.lblErrorState.Size = new System.Drawing.Size(57, 13);
            this.lblErrorState.TabIndex = 13;
            this.lblErrorState.Text = "Error State";
            // 
            // lblErrorSeverity
            // 
            this.lblErrorSeverity.AutoSize = true;
            this.lblErrorSeverity.Location = new System.Drawing.Point(17, 41);
            this.lblErrorSeverity.Name = "lblErrorSeverity";
            this.lblErrorSeverity.Size = new System.Drawing.Size(70, 13);
            this.lblErrorSeverity.TabIndex = 14;
            this.lblErrorSeverity.Text = "Error Severity";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(17, 70);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(50, 13);
            this.lblMessage.TabIndex = 15;
            this.lblMessage.Text = "Message";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTitle});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(610, 25);
            this.toolStrip1.TabIndex = 16;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lblTitle
            // 
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(137, 22);
            this.lblTitle.Text = "Gen API testing interface";
            // 
            // pnlServices
            // 
            this.pnlServices.Controls.Add(this.panelDownloadDocInput);
            this.pnlServices.Controls.Add(this.panelDocDetailsInput);
            this.pnlServices.Controls.Add(this.btnGetDocument);
            this.pnlServices.Controls.Add(this.button8);
            this.pnlServices.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlServices.Location = new System.Drawing.Point(0, 0);
            this.pnlServices.Name = "pnlServices";
            this.pnlServices.Size = new System.Drawing.Size(300, 192);
            this.pnlServices.TabIndex = 17;
            // 
            // btnGetDocument
            // 
            this.btnGetDocument.Location = new System.Drawing.Point(20, 60);
            this.btnGetDocument.Name = "btnGetDocument";
            this.btnGetDocument.Size = new System.Drawing.Size(149, 23);
            this.btnGetDocument.TabIndex = 12;
            this.btnGetDocument.Text = "Get Document";
            this.btnGetDocument.UseVisualStyleBackColor = true;
            this.btnGetDocument.Click += new System.EventHandler(this.btnGetDocument_Click);
            // 
            // pnlOutputStatus
            // 
            this.pnlOutputStatus.Controls.Add(this.splitter2);
            this.pnlOutputStatus.Controls.Add(this.label1);
            this.pnlOutputStatus.Controls.Add(this.txtMessage);
            this.pnlOutputStatus.Controls.Add(this.txtErrorSeverity);
            this.pnlOutputStatus.Controls.Add(this.txtErrorState);
            this.pnlOutputStatus.Controls.Add(this.rtbOutputParamValues);
            this.pnlOutputStatus.Controls.Add(this.lblErrorState);
            this.pnlOutputStatus.Controls.Add(this.lblErrorSeverity);
            this.pnlOutputStatus.Controls.Add(this.lblMessage);
            this.pnlOutputStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOutputStatus.Location = new System.Drawing.Point(300, 0);
            this.pnlOutputStatus.Name = "pnlOutputStatus";
            this.pnlOutputStatus.Size = new System.Drawing.Size(310, 192);
            this.pnlOutputStatus.TabIndex = 18;
            // 
            // splitter2
            // 
            this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter2.Location = new System.Drawing.Point(0, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 192);
            this.splitter2.TabIndex = 21;
            this.splitter2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Output parameter";
            // 
            // txtMessage
            // 
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMessage.Location = new System.Drawing.Point(106, 68);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(292, 42);
            this.txtMessage.TabIndex = 19;
            // 
            // txtErrorSeverity
            // 
            this.txtErrorSeverity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtErrorSeverity.Location = new System.Drawing.Point(106, 39);
            this.txtErrorSeverity.Name = "txtErrorSeverity";
            this.txtErrorSeverity.ReadOnly = true;
            this.txtErrorSeverity.Size = new System.Drawing.Size(100, 20);
            this.txtErrorSeverity.TabIndex = 18;
            // 
            // txtErrorState
            // 
            this.txtErrorState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtErrorState.Location = new System.Drawing.Point(106, 10);
            this.txtErrorState.Name = "txtErrorState";
            this.txtErrorState.ReadOnly = true;
            this.txtErrorState.Size = new System.Drawing.Size(100, 20);
            this.txtErrorState.TabIndex = 17;
            // 
            // rtbOutputParamValues
            // 
            this.rtbOutputParamValues.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbOutputParamValues.Location = new System.Drawing.Point(106, 116);
            this.rtbOutputParamValues.Name = "rtbOutputParamValues";
            this.rtbOutputParamValues.ReadOnly = true;
            this.rtbOutputParamValues.Size = new System.Drawing.Size(292, 64);
            this.rtbOutputParamValues.TabIndex = 16;
            this.rtbOutputParamValues.Text = "";
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.dataGridView1);
            this.panelBottom.Controls.Add(this.toolStrip2);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBottom.Location = new System.Drawing.Point(0, 217);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(610, 250);
            this.panelBottom.TabIndex = 19;
            // 
            // panelDownloadDocInput
            // 
            this.panelDownloadDocInput.Controls.Add(this.btnOKGetDocument);
            this.panelDownloadDocInput.Controls.Add(this.txtDocumentId);
            this.panelDownloadDocInput.Controls.Add(this.label5);
            this.panelDownloadDocInput.Location = new System.Drawing.Point(5, 95);
            this.panelDownloadDocInput.Name = "panelDownloadDocInput";
            this.panelDownloadDocInput.Size = new System.Drawing.Size(280, 100);
            this.panelDownloadDocInput.TabIndex = 14;
            this.panelDownloadDocInput.Visible = false;
            // 
            // btnOKGetDocument
            // 
            this.btnOKGetDocument.Location = new System.Drawing.Point(187, 57);
            this.btnOKGetDocument.Name = "btnOKGetDocument";
            this.btnOKGetDocument.Size = new System.Drawing.Size(75, 23);
            this.btnOKGetDocument.TabIndex = 5;
            this.btnOKGetDocument.Text = "OK";
            this.btnOKGetDocument.UseVisualStyleBackColor = true;
            this.btnOKGetDocument.Click += new System.EventHandler(this.btnOKGetDocument_Click);
            // 
            // txtDocumentId
            // 
            this.txtDocumentId.Location = new System.Drawing.Point(88, 24);
            this.txtDocumentId.Name = "txtDocumentId";
            this.txtDocumentId.Size = new System.Drawing.Size(174, 20);
            this.txtDocumentId.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Document Id";
            // 
            // panelDocDetailsInput
            // 
            this.panelDocDetailsInput.Controls.Add(this.btnOkDocDetails);
            this.panelDocDetailsInput.Controls.Add(this.txtDepartment);
            this.panelDocDetailsInput.Controls.Add(this.txtProjectType);
            this.panelDocDetailsInput.Controls.Add(this.label3);
            this.panelDocDetailsInput.Controls.Add(this.label2);
            this.panelDocDetailsInput.Location = new System.Drawing.Point(3, 50);
            this.panelDocDetailsInput.Name = "panelDocDetailsInput";
            this.panelDocDetailsInput.Size = new System.Drawing.Size(284, 118);
            this.panelDocDetailsInput.TabIndex = 13;
            this.panelDocDetailsInput.Visible = false;
            // 
            // btnOkDocDetails
            // 
            this.btnOkDocDetails.Location = new System.Drawing.Point(187, 85);
            this.btnOkDocDetails.Name = "btnOkDocDetails";
            this.btnOkDocDetails.Size = new System.Drawing.Size(75, 23);
            this.btnOkDocDetails.TabIndex = 4;
            this.btnOkDocDetails.Text = "OK";
            this.btnOkDocDetails.UseVisualStyleBackColor = true;
            this.btnOkDocDetails.Click += new System.EventHandler(this.btnOkDocDetails_Click);
            // 
            // txtDepartment
            // 
            this.txtDepartment.Location = new System.Drawing.Point(88, 53);
            this.txtDepartment.Name = "txtDepartment";
            this.txtDepartment.Size = new System.Drawing.Size(174, 20);
            this.txtDepartment.TabIndex = 3;
            // 
            // txtProjectType
            // 
            this.txtProjectType.Location = new System.Drawing.Point(88, 24);
            this.txtProjectType.Name = "txtProjectType";
            this.txtProjectType.Size = new System.Drawing.Size(174, 20);
            this.txtProjectType.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Department";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Project Type";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblResultsetTitle});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(610, 25);
            this.toolStrip2.TabIndex = 13;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // lblResultsetTitle
            // 
            this.lblResultsetTitle.Name = "lblResultsetTitle";
            this.lblResultsetTitle.Size = new System.Drawing.Size(54, 22);
            this.lblResultsetTitle.Text = "Resultset";
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.pnlOutputStatus);
            this.pnlTop.Controls.Add(this.pnlServices);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 25);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(610, 192);
            this.pnlTop.TabIndex = 20;
            // 
            // splitter1
            // 
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 217);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(610, 3);
            this.splitter1.TabIndex = 12;
            this.splitter1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 467);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "GenAPI Tester";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlServices.ResumeLayout(false);
            this.pnlOutputStatus.ResumeLayout(false);
            this.pnlOutputStatus.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.panelDownloadDocInput.ResumeLayout(false);
            this.panelDownloadDocInput.PerformLayout();
            this.panelDocDetailsInput.ResumeLayout(false);
            this.panelDocDetailsInput.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblErrorState;
        private System.Windows.Forms.Label lblErrorSeverity;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lblTitle;
        private System.Windows.Forms.Panel pnlServices;
        private System.Windows.Forms.Panel pnlOutputStatus;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.RichTextBox rtbOutputParamValues;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.TextBox txtErrorSeverity;
        private System.Windows.Forms.TextBox txtErrorState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel lblResultsetTitle;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Button btnGetDocument;
        private System.Windows.Forms.Panel panelDocDetailsInput;
        private System.Windows.Forms.TextBox txtDepartment;
        private System.Windows.Forms.TextBox txtProjectType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelDownloadDocInput;
        private System.Windows.Forms.TextBox txtDocumentId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOKGetDocument;
        private System.Windows.Forms.Button btnOkDocDetails;
    }
}

