using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;
using Newtonsoft.Json;

namespace OCRService
{
    public class DSXML
    {
        // Function to convert passed XML data to dataset
        public static DataSet ConvertXMLToDataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                // Load the XmlTextReader from the stream
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }// Use this function to get XML string from a dataset

        // Function to convert passed dataset to XML data
        public static string ConvertDataSetToXML(DataSet xmlDS)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;
            try
            {
                stream = new MemoryStream();
                // Load the XmlTextReader from the stream
                writer = new XmlTextWriter(stream, Encoding.Unicode);
                // Write to the file with the WriteXml method.
                xmlDS.WriteXml(writer);
                int count = (int)stream.Length;
                byte[] arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, count);
                UnicodeEncoding utf = new UnicodeEncoding();
                return utf.GetString(arr).Trim();
            }
            catch
            {
                return String.Empty;
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }


        /// <summary>
        ///  Convert JSON to dataset
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>

        public static DataSet ConvertJsonStringToDataSet(string jsonString)
        {
            DataSet ds = new DataSet();
            try
            {
                XmlDocument xd = new XmlDocument();
                jsonString = "{ \"rootNode\": {" + jsonString.Trim().TrimStart('{').TrimEnd('}') + "} }";
                xd = (XmlDocument)JsonConvert.DeserializeXmlNode(jsonString);
                ds.ReadXml(new XmlNodeReader(xd));
                return ds;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            finally
            {
                //DisposeObjects.DisposeObjList(ds);
            }
        }

    }
}
