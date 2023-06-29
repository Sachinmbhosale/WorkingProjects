using System;
using System.Data;
using iTextSharp.text.pdf;
using System.IO;

namespace OfficeConverter
{
    public class GettingControlsFromPDF
    {
        public DataTable getcontrolsnames(string inputFile)
        {
            DataTable Dt = new DataTable();
            Dt.Columns.Add("ControlName", typeof(string)); //adding columns
            Dt.Columns.Add("ControlType", typeof(string));


            string ControlType = string.Empty;
            PdfReader reader = new PdfReader(inputFile);
            AcroFields form = reader.AcroFields;
            try
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    foreach (string key in form.Fields.Keys)
                    {
                        switch (form.GetFieldType(key))
                        {
                            case AcroFields.FIELD_TYPE_CHECKBOX:
                                ControlType = "CheckBox";
                                break;
                            //Create Checkbox
                            case AcroFields.FIELD_TYPE_COMBO:
                                ControlType = "ComboBox";
                                break;
                            //Create Combo Box
                            case AcroFields.FIELD_TYPE_LIST:
                                ControlType = "ListBox";
                                break;
                            //Create List
                            case AcroFields.FIELD_TYPE_RADIOBUTTON:
                                ControlType = "RadioButton";
                                break;
                            //Create Radio button
                            case AcroFields.FIELD_TYPE_NONE:
                                ControlType = "None";
                                break;
                            case AcroFields.FIELD_TYPE_PUSHBUTTON:
                                ControlType = "PushButton";
                                break;
                            //Create Submit Button
                            case AcroFields.FIELD_TYPE_SIGNATURE:
                                ControlType = "Signature";
                                break;
                            //Create Signature
                            case AcroFields.FIELD_TYPE_TEXT:
                                ControlType = "TextBox";
                                break;
                            default:
                                // result = "Failed";
                                break;
                                //Create textbox/Qs header
                                int fileType = form.GetFieldType(key);
                                string fieldValue = form.GetField(key);
                                //float[] a = form.GetFieldPositions(key);
                                //string translatedFileName = form.GetTranslatedFieldName(key);
                                AcroFields.Item test = form.GetFieldItem(key);


                        }
                        Dt.Rows.Add(key, ControlType);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                reader.Close();
            }
            return Dt;
        }

        private string Returnbool(string Value)
        {
            string Returnv = string.Empty;
            try
            {

                Returnv = Value == "0" ? "True" : "False";
                bool b = Convert.ToBoolean(Returnv);
                if (b == true)
                {
                    Returnv = "Yes";
                }
                else
                {
                    Returnv = "No";
                }
            }
            catch
            {

            }

            return Returnv;

        }
        public void FillControlValues(string FilePath, DataTable DT, string newfilepath)
        {


            string pdfTemplate = FilePath;



            string newFile = newfilepath;

            PdfReader pdfReader = new PdfReader(pdfTemplate);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(
                        newFile, FileMode.Create));

            AcroFields pdfFormFields = pdfStamper.AcroFields;
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                switch (DT.Rows[i]["ControlType"].ToString())
                {
                    case "CheckBox":
                        pdfFormFields.SetField(DT.Rows[i]["Controlname"].ToString(), Returnbool(DT.Rows[i]["ControlValue"].ToString()));
                        break;
                    //Create Checkbox
                    case "ComboBox":
                        break;
                    //Create Combo Box
                    case "ListBox":
                        break;
                    //Create List
                    case "RadioButton":
                        break;
                    //Create Radio button
                    case "None":
                        break;
                    case "PushButton":
                        break;
                    //Create Submit Button
                    case "Signature":
                        break;
                    //Create Signature
                    case "TextBox":
                        pdfFormFields.SetField(DT.Rows[i]["Controlname"].ToString(), DT.Rows[i]["ControlValue"].ToString());
                        break;
                    default:
                        // result = "Failed";
                        break;

                }
            }
            pdfStamper.FormFlattening = true;

            // close the pdf
            pdfStamper.Close();
        }



    }

}


