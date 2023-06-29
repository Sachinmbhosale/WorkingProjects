using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;

namespace OCRService
{
    public class OCRHelper
    {
        //Create a array list to store parameter(s) with
        string Message = string.Empty,
           OcrLangData = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\tessdata",
            OcrLang = ConfigurationManager.AppSettings["OCR_Language"].ToString();

        public string DoOCR(string filePath)
        {
            Bitmap imgSource = new Bitmap(filePath);

            string ocrContent = string.Empty;

            using (tessnet2.Tesseract ocr = new tessnet2.Tesseract())
            {
                ocr.Init(OcrLangData, OcrLang, false);

                List<tessnet2.Word> m_words = ocr.DoOCR(imgSource, Rectangle.Empty);
                if (m_words != null)
                {
                    int LastLineIndex = 0;
                    foreach (tessnet2.Word word in m_words)
                    {
                        if (LastLineIndex == word.LineIndex)
                            ocrContent += word.Text + " ";
                        else
                            ocrContent += Environment.NewLine + word.Text;

                        LastLineIndex = word.LineIndex;
                    }
                }
            }
            GC.Collect();
            return ocrContent;
        }
    }
}
