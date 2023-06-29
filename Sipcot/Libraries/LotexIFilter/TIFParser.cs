using System;
using System.IO;

namespace Lotex.IFilter.Parser
{
    public class TIFParser
    {
        public static string Extract(string file)
        {
            string fileExtension = Path.GetExtension(file);

            //get file name without extenstion 
            string fileName = Convert.ToString(file).Replace(fileExtension, string.Empty);

            if (fileExtension == ".jpg" || fileExtension == ".JPG" || fileExtension == ".TIF") // or // ImageFormat.Jpeg.ToString()
            {
                try
                {
                    //OCR Operations ... 
                    MODI.Document md = new MODI.Document();
                    md.Create(file);
                    md.OCR(MODI.MiLANGUAGES.miLANG_ENGLISH, true, true);
                    MODI.Image image = (MODI.Image)md.Images[0];
                    return image.Layout.Text;
                    #region<<writing to text file>>
                    ////create text file with the same Image file name 
                    //FileStream createFile = new FileStream(fileName + ".txt", FileMode.CreateNew);

                    ////save the image text in the text file 
                    //StreamWriter writeFile = new StreamWriter(createFile);
                    //writeFile.Write(image.Layout.Text);
                    //writeFile.Close();
                    #endregion<<writing to text file>>
                }
                catch (Exception ex)
                {
                   
                    throw new Exception("Parser cannot covert the file"+ex.ToString());
                }


            }
            return "";
        }

    }
}
