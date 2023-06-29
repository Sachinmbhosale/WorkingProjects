using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using System.Data;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace TransferService
{
    public partial class Channel : System.Web.UI.Page
    {
        private string xmlPath = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            txtPassword.Attributes.Add("type", "password");

            //Clear the previous message
            lblMsg.Text = string.Empty;

            // To avoid _doPostback error: clearing postback data
            ClientScript.GetPostBackEventReference(this, string.Empty);

            if (!Page.IsPostBack)
            {
                //Set max id +1 for creating new channel
                btnAddNew_Click(sender, e);

                // Load gridview with available channels
                gvChannel.DataSource = SelectAll();
                gvChannel.DataBind();
            }
        }

        #region handle xml using ds

        static DataSet ds = new DataSet();
        static DataView dv = new DataView();
        /// <summary>
        /// Inserts a record into the Category table.
        /// </summary>
        /// 
        void save()
        {
            ds.WriteXml(Server.MapPath(UploadChannel.XmlFile), XmlWriteMode.WriteSchema);
        }

        void Insert(UploadChannel entity)
        {
            DataRow dr = dv.Table.NewRow();
            dr[1] = UploadChannel.Edit;
            dr[0] = entity.ChannelId;
            dr[2] = entity.Channel;
            dr[3] = entity.FileUploadPath;
            dr[4] = entity.Description;
            dr[5] = entity.Username;
            dr[6] = entity.Password;
            dr[7] = entity.EmailId;
            dr[8] = entity.Active;
            dv.Table.Rows.Add(dr);
            save();

            lblMsg.Text = MessageFormatter.GetFormattedSuccessMessage("Channel created successfully.");

            //Set max id +1 for creating new channel
            btnAddNew_Click(null,null);
        }

        /// <summary>
        /// Updates a record in the Category table.
        /// </summary>
        void Update(UploadChannel entity)
        {
            DataRow dr = Select(entity.ChannelId);
            dr[2] = entity.Channel;
            dr[3] = entity.FileUploadPath;
            dr[4] = entity.Description;
            dr[5] = entity.Username;
            dr[6] = entity.Password;
            dr[7] = entity.EmailId;
            dr[8] = entity.Active;
            save();

            lblMsg.Text = MessageFormatter.GetFormattedSuccessMessage("Channel updated successfully.");
            btnAddNew_Click(null, null);            
        }

        /// <summary>
        /// Deletes a record from the Category table by a composite primary key.
        /// </summary>
        void Delete(int ChannelId)
        {
            dv.RowFilter = "ChannelId='" + ChannelId + "'";
            dv.Sort = "ChannelId";
            if (dv.Count > 0)
            {
                dv.Delete(0);
            }
            dv.RowFilter = "";
            save();

            MessageFormatter.GetFormattedSuccessMessage("Channel deleted successfully.");
        }

        /// <summary>
        /// Selects a single record from the Category table.
        /// </summary>
        DataRow Select(int ChannelId)
        {
            dv.RowFilter = "ChannelId='" + ChannelId + "'";
            dv.Sort = "ChannelId";
            DataRow dr = null;
            if (dv.Count > 0)
            {
                dr = dv[0].Row;
            }
            dv.RowFilter = "";
            return dr;
        }

        bool CheckDuplicateChannel(string Channel, string ChannelId)
        {
            bool hasDuplicate = false;
            dv.RowFilter = "Channel='" + Channel + "' AND ChannelId <> '" + ChannelId + "'";
            if (dv.Count > 0)
            {
                hasDuplicate = true;
            }
            dv.RowFilter = "";
            return hasDuplicate;
        }

        bool CheckDuplicateUsername(string Username, string ChannelId)
        {
            bool hasDuplicate = false;
            dv.RowFilter = "Username='" + Username + "' AND ChannelId <> '" + ChannelId + "'";
            if (dv.Count > 0)
            {
                hasDuplicate = true;
            }
            dv.RowFilter = "";
            return hasDuplicate;
        }

        /// <summary>
        /// Selects all records from the Category table.
        /// </summary>
        DataView SelectAll()
        {
            if (!File.Exists(Server.MapPath(UploadChannel.XmlFile)))
            {
                var xmlString = CreateXML(new UploadChannel());

                // Create xml document                
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString.ToString());
                xmlDoc.Save(Server.MapPath(UploadChannel.XmlFile));
            }

            ds.Clear();
            ds.ReadXml(Server.MapPath(UploadChannel.XmlFile), XmlReadMode.ReadSchema);
            dv = ds.Tables[1].DefaultView;
            return dv;
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

        bool validateData()
        {
            bool validated = true;
            if (txtChannelId.Text.Length == 0)
            {
                validated = false;
                lblMsg.Text = MessageFormatter.GetFormattedNoticeMessage("Please click on button 'New Channel' to create new channel.");
            }
            else if (txtChannel.Text.Length == 0)
            {
                validated = false;
                txtChannel.Focus();
                lblMsg.Text = MessageFormatter.GetFormattedNoticeMessage("Please enter channel name.");
            }
            else if (txtFileUploadPath.Text.Length == 0)
            {
                validated = false;
                txtFileUploadPath.Focus();
                lblMsg.Text = MessageFormatter.GetFormattedNoticeMessage("Please enter file upload path.");
            }
            //else if (txtDescription.Text.Length == 0)
            //{
            //    validated = false;
            //    lblMsg.Text = MessageFormatter.GetFormattedNoticeMessage("Please enter description.");
            //}
            else if (txtUserName.Text.Length == 0)
            {
                validated = false;
                txtUserName.Focus();
                lblMsg.Text = MessageFormatter.GetFormattedNoticeMessage("Please enter user name.");
            }
            else if (txtPassword.Text.Length == 0)
            {
                validated = false;
                txtPassword.Focus();
                lblMsg.Text = MessageFormatter.GetFormattedNoticeMessage("Please enter password.");
            }

                // Duplicate checks
            if (CheckDuplicateChannel(txtChannel.Text, txtChannelId.Text))
            {
                validated = false;
                txtChannel.Focus();
                lblMsg.Text = MessageFormatter.GetFormattedNoticeMessage("Channel name already exist. Please enter different name.");
            }
            if (CheckDuplicateUsername(txtUserName.Text, txtChannelId.Text))
            {
                validated = false;
                txtUserName.Focus();
                lblMsg.Text = MessageFormatter.GetFormattedNoticeMessage("Username already exist. Please enter different name.");
            }

            return validated;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (validateData())
            {
                UploadChannel entity = new UploadChannel()
                {
                    ChannelId = Convert.ToInt32(txtChannelId.Text),
                    Channel = txtChannel.Text,
                    FileUploadPath = txtFileUploadPath.Text,
                    Description = txtDescription.Text,
                    Username = txtUserName.Text,
                    Password = txtPassword.Text,
                    EmailId = txtEmailId.Text,
                    Active = chkActive.Checked
                };

                if (hdnAction.Value.ToLower() == string.Empty)
                    Insert(entity);
                else
                {
                    Update(entity);                    
                }

                // Reload data to gridview
                gvChannel.DataSource = SelectAll();
                gvChannel.DataBind();
            }
        }

        protected void gvChannel_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gvChannel, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["style"] = "cursor:pointer";
                e.Row.ToolTip = "Click to edit.";

                if(e.Row.Cells[8].Text.ToLower()=="false")
                e.Row.BackColor = System.Drawing.Color.FromName("gray");       
            }

            //Hide unwanted columns
            e.Row.Cells[0].Visible = false;
            e.Row.Cells[1].Visible = false;
            e.Row.Cells[6].Visible = false;
            //e.Row.Cells[8].Visible = false;
            e.Row.Cells[9].Visible = false;

            //Code to encode html tags
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string encoded = e.Row.Cells[i].Text;
                e.Row.Cells[i].Text = Context.Server.HtmlDecode(encoded);
            }
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            hdnAction.Value = string.Empty;
            dv.RowFilter = "ChannelId = max(ChannelId)";
            if (dv.Count > 0)
            {
                txtChannelId.Text = (Convert.ToInt32(dv[0].Row[0]) + 1).ToString();
            }
            else
                txtChannelId.Text = "1";
            dv.RowFilter = "";

            //clear controls
            txtChannel.Text = string.Empty;
            txtFileUploadPath.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtEmailId.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtUserName.Text = string.Empty;
        }

        // To resolve the above error you must set the property EnableEventValidation = "false"
        protected void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            int index = gvChannel.SelectedRow.RowIndex;

            hdnAction.Value = "Edit";

            txtChannelId.Text = gvChannel.SelectedRow.Cells[0].Text;
            txtChannel.Text = gvChannel.SelectedRow.Cells[2].Text;
            txtFileUploadPath.Text = gvChannel.SelectedRow.Cells[3].Text;
            txtDescription.Text = gvChannel.SelectedRow.Cells[4].Text;
            txtUserName.Text = gvChannel.SelectedRow.Cells[5].Text;
            txtPassword.Text = gvChannel.SelectedRow.Cells[6].Text;
            txtEmailId.Text = gvChannel.SelectedRow.Cells[7].Text;
            chkActive.Checked = gvChannel.SelectedRow.Cells[8].Text.Length > 0 ? Convert.ToBoolean(gvChannel.SelectedRow.Cells[8].Text) : true;

            //string message = "Row Index: " + index + "\\nName: " + name + "\\nCountry: " + country;
            //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + message + "');", true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hdnAction.Value.ToLower() != string.Empty)
            Delete(Convert.ToInt32(txtChannelId.Text));

            // Reload data to gridview
            gvChannel.DataSource = SelectAll();
            gvChannel.DataBind();
        }

    }
}