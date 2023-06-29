using System;
//using System.Web.UI.WebControls;
using MuPDFLib;
using System.Configuration;

public class Pdf2ImageConverter
{
    //Converting Pdf files to Jpeg.
    public string ConvertPdf2Jpeg(String sourcePdfPath, string targetPath)
    {
        //Logger.TraceErrorLog("Starting Splitting Pdf File fn=ConvertPdf2Jpeg SourcePath=" + sourcePdfPath);
        try
        {
            float DotsPerImage = Convert.ToInt32(ConfigurationManager.AppSettings["DotsPerImage"].ToString());
            int MAXPixels = Convert.ToInt32(ConfigurationManager.AppSettings["MAXPixels"].ToString());
            RenderType renderType;
            string RenderTypeT = ConfigurationManager.AppSettings["RenderType"].ToString();
            if (RenderTypeT.ToLower() == "monochrome")
            {
                renderType = RenderType.Monochrome;
            }
            else if (RenderTypeT.ToLower() == "grayscale")
            {
                renderType = RenderType.Grayscale;
            }
            else
            {
                renderType = RenderType.RGB;
            }            

            //Converting PDF to Jpeg 
            MuPdfConverter.ConvertPdfToTiff(sourcePdfPath, targetPath, DotsPerImage, renderType, false, true, MAXPixels, "");

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return targetPath;
    }

}
