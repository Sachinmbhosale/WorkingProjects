using System.Drawing;
using System.IO;

namespace OfficeConverter
{
    public class Watermark
    {

        public void WaterMarkJpegImage(string JpegImage, string WatermarkImage)
        {
            string FileName = Path.GetFileNameWithoutExtension(JpegImage);
            string WatermarkedImagePath = JpegImage.Replace(FileName, FileName + "_Watermark");

            using (Image image = Image.FromFile(JpegImage))
            using (Image watermarkImage = Image.FromFile(WatermarkImage))
            using (Graphics imageGraphics = Graphics.FromImage(image))
            using (TextureBrush watermarkBrush = new TextureBrush(watermarkImage))
            {
                int x = (image.Width / 2 - watermarkImage.Width / 2);
                int y = (image.Height / 2 - watermarkImage.Height / 2);
                watermarkBrush.TranslateTransform(x, y);
                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
                image.Save(WatermarkedImagePath);
            }

            if (File.Exists(WatermarkedImagePath) && File.Exists(JpegImage))
                File.Delete(JpegImage);

            File.Copy(WatermarkedImagePath, JpegImage);
            File.Delete(WatermarkedImagePath);
        }


        public void DrawTextOnImage(string ImagePath, string WaterMarkText)
        {
            // setup default settings
            string fontName ="Arial"; float fontSize=40;
            System.Drawing.Color myWatermarkColor = Color.SteelBlue;
            System.Drawing.Font myFont = new Font(fontName,fontSize,FontStyle.Bold|FontStyle.Italic);

            // Update the applicaton by reloading the image
            Image Image = Image.FromFile(ImagePath);

            int opac = 0;
            string sOpacity = "50%";

            // Determine the opacity of the watermark
            switch (sOpacity)
            {
                case "100%":
                    opac = 255; // 1 * 255
                    break;
                case "75%":
                    opac = 191; // .75 * 255
                    break;
                case "50%":
                    opac = 127; // .5 * 255
                    break;
                case "25%":
                    opac = 64;  // .25 * 255
                    break;
                case "10%":
                    opac = 25;  // .10 * 255
                    break;
                default:
                    opac = 127; // default at 50%; .5 * 255
                    break;
            }

            // Get a graphics context
            Graphics g = Graphics.FromImage(Image);

            // Create a solid brush to write the watermark text on the image
            Brush myBrush = new SolidBrush(Color.FromArgb(opac, myWatermarkColor));

            // Calculate the size of the text
            SizeF sz = g.MeasureString(WaterMarkText, myFont);

            // Creae a copy of variables to keep track of the
            // drawing position (X,Y)
            int X;
            int Y;

            // Set the drawing position based on the users
            // selection of placing the text at the bottom or
            // top of the image
            if (true)
            {
                X = (int)(Image.Width - sz.Width) / 2;
                Y = (int)(Image.Height + sz.Height) / 2;
            }
            //else // bottom of the image
            //{
            //    X = (int)(Image.Width - sz.Width) / 2;
            //    Y = (int)(Image.Height - sz.Height);
            //}

            // draw the water mark text
            g.DrawString(WaterMarkText, myFont, myBrush, new Point(X, Y));

            string FileName = Path.GetFileNameWithoutExtension(ImagePath);
            string WatermarkedImagePath = ImagePath.Replace(FileName, FileName + "_Watermark");

            using (Bitmap tempImage = new Bitmap(Image))
            {
                tempImage.Save(WatermarkedImagePath); //, System.Drawing.Imaging.ImageFormat.Jpeg);
            }   

            if (File.Exists(WatermarkedImagePath) && File.Exists(ImagePath))
                File.Delete(ImagePath);

            File.Copy(WatermarkedImagePath, ImagePath);
            File.Delete(WatermarkedImagePath);
        }

    }
}
