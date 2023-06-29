using System.Xml.Linq;

namespace TransferService
{
    public class UploadChannel
    {
        public UploadChannel()
        {
            Channel = string.Empty;
            FileUploadPath = string.Empty;
            Description = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            EmailId = string.Empty;
            Active = true;
        }

        public const string XmlFile = @"App_Data/Channels.xml";
        public const string KeyName = "UploadChannel";
        public const string TopElement = "Channel";
        public const string Edit = "<a href=\"#\" onclick=\"DeleteSelectedRow(this);\">Delete</a>";

        public int ChannelId { get; set; }
        public string Channel { get; set; }
        public string FileUploadPath { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailId { get; set; }
        public bool Active { get; set; }
    }

    public static class XmlExtensionMethods
    {
        public static string GetAsString(this XAttribute attr)
        {
            string ret = string.Empty;

            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                ret = attr.Value;
            }

            return ret;
        }

        public static int GetAsInt32(this XAttribute attr)
        {
            int ret = 0;
            int value = 0;

            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                if (int.TryParse(attr.Value, out value))
                    ret = value;
            }

            return ret;
        }

    }
}
