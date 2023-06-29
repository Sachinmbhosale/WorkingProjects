using System.Xml.Linq;

namespace MassUpload
{
    public class Data
    {
        public Data()
        {
            Id = 0;
            FileName = string.Empty;
            FilePath = string.Empty;
            Status = string.Empty;
            Remarks = string.Empty;
        }
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
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
