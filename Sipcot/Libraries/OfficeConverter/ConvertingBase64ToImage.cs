using System;
using System.Drawing;
using System.IO;

namespace OfficeConverter
{
    public class ConvertingBase64ToImage
    {
        public void LoadImage(string base64img,string outputpath)
        {
            //get a temp image from bytes, instead of loading from disk
            //data:image/gif;base64,
            //this image is a single pixel (black)
            byte[] bytes = Convert.FromBase64String(base64img);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            image.Save(outputpath, System.Drawing.Imaging.ImageFormat.Jpeg);
          
        }


      
    }
}
