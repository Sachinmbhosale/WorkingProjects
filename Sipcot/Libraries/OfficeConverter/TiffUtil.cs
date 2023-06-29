using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TiffUtil
{

    public class TiffUtil
    {
        #region Variable & Class Definitions

        private static System.Drawing.Imaging.ImageCodecInfo tifImageCodecInfo;

        private static EncoderParameter tifEncoderParameterMultiFrame;
        private static EncoderParameter tifEncoderParameterFrameDimensionPage;
        private static EncoderParameter tifEncoderParameterFlush;
        private static EncoderParameter tifEncoderParameterCompression;
        private static EncoderParameter tifEncoderParameterLastFrame;
        private static EncoderParameter tifEncoderParameter24BPP;
        private static EncoderParameter tifEncoderParameter1BPP;

        private static EncoderParameters tifEncoderParametersPage1;
        private static EncoderParameters tifEncoderParametersPageX;
        private static EncoderParameters tifEncoderParametersPageLast;

        private static System.Drawing.Imaging.Encoder tifEncoderSaveFlag;
        private static System.Drawing.Imaging.Encoder tifEncoderCompression;
        private static System.Drawing.Imaging.Encoder tifEncoderColorDepth;

        private static bool encoderAssigned;

        public static string tempDir;
        public static bool initComplete;

        public TiffUtil(string tempPath)
        {
            try
            {
                if (!initComplete)
                {
                    if (!tempPath.EndsWith("\\"))
                        tempDir = tempPath + "\\";
                    else
                        tempDir = tempPath;

                    Directory.CreateDirectory(tempDir);
                    initComplete = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        #endregion

        #region Retrieve Page Count of a multi-page TIFF file

        public static int getPageCount(string fileName)
        {
            int pageCount = -1;

            try
            {
                Image img = Bitmap.FromFile(fileName);
                pageCount = img.GetFrameCount(FrameDimension.Page);
                img.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return pageCount;
        }

        public static int getPageCount(Image img)
        {
            int pageCount = -1;
            try
            {
                pageCount = img.GetFrameCount(FrameDimension.Page);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return pageCount;
        }

        #endregion

        #region Retrieve multiple single page images from a single multi-page TIFF file

        public static Image[] getTiffImages(Image sourceImage, int[] pageNumbers)
        {
            MemoryStream ms = null;
            Image[] returnImage = new Image[pageNumbers.Length];

            try
            {
                Guid objGuid = sourceImage.FrameDimensionsList[0];
                FrameDimension objDimension = new FrameDimension(objGuid);

                for (int i = 0; i < pageNumbers.Length; i++)
                {
                    ms = new MemoryStream();
                    sourceImage.SelectActiveFrame(objDimension, pageNumbers[i]);
                    sourceImage.Save(ms, ImageFormat.Tiff);
                    returnImage[i] = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ms.Close();
            }

            return returnImage;
        }

        public static Image[] getTiffImages(Image sourceImage)
        {
            MemoryStream ms = null;
            int pageCount = getPageCount(sourceImage);

            Image[] returnImage = new Image[pageCount];

            try
            {
                Guid objGuid = sourceImage.FrameDimensionsList[0];
                FrameDimension objDimension = new FrameDimension(objGuid);

                for (int i = 0; i < pageCount; i++)
                {
                    ms = new MemoryStream();
                    sourceImage.SelectActiveFrame(objDimension, i);
                    sourceImage.Save(ms, ImageFormat.Tiff);
                    returnImage[i] = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                ms.Close();
            }

            return returnImage;
        }

        public static Image[] getTiffImages(string sourceFile, int[] pageNumbers)
        {
            Image[] returnImage = new Image[pageNumbers.Length];

            try
            {
                Image sourceImage = Bitmap.FromFile(sourceFile);
                returnImage = getTiffImages(sourceImage, pageNumbers);
                sourceImage.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                returnImage = null;
            }

            return returnImage;
        }

        #endregion

        #region Retrieve a specific page from a multi-page TIFF image

        public static Image getTiffImage(string sourceFile, int pageNumber)
        {
            Image returnImage = null;

            try
            {
                Image sourceImage = Image.FromFile(sourceFile);
                returnImage = getTiffImage(sourceImage, pageNumber);
                sourceImage.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                returnImage = null;
            }

            return returnImage;
        }

        public static Image getTiffImage(Image sourceImage, int pageNumber)
        {
            MemoryStream ms = null;
            Image returnImage = null;

            try
            {
                ms = new MemoryStream();
                Guid objGuid = sourceImage.FrameDimensionsList[0];
                FrameDimension objDimension = new FrameDimension(objGuid);
                sourceImage.SelectActiveFrame(objDimension, pageNumber);
                sourceImage.Save(ms, ImageFormat.Tiff);
                returnImage = Image.FromStream(ms);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ms.Close();
            }

            return returnImage;
        }

        public static bool getTiffImage(string sourceFile, string targetFile, int pageNumber)
        {
            bool response = false;

            try
            {
                Image returnImage = getTiffImage(sourceFile, pageNumber);
                returnImage.Save(targetFile);
                returnImage.Dispose();
                response = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response;
        }

        #endregion

        #region Split a multi-page TIFF file into multiple single page TIFF files

        public static string[] splitTiffPages(string sourceFile, string targetDirectory)
        {
            string[] returnImages;

            try
            {
                Image sourceImage = Bitmap.FromFile(sourceFile);
                Image[] sourceImages = splitTiffPages(sourceImage);

                int pageCount = sourceImages.Length;

                returnImages = new string[pageCount];
                for (int i = 0; i < pageCount; i++)
                {
                    FileInfo fi = new FileInfo(sourceFile);
                    string babyImg = targetDirectory + "\\" + fi.Name.Substring(0, (fi.Name.Length - fi.Extension.Length)) + "_PAGE" + (i + 1).ToString().PadLeft(3, '0') + fi.Extension;
                    sourceImages[i].Save(babyImg);
                    returnImages[i] = babyImg;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                returnImages = null;
            }

            return returnImages;
        }

        public static Image[] splitTiffPages(Image sourceImage)
        {
            Image[] returnImages;

            try
            {
                int pageCount = getPageCount(sourceImage);
                returnImages = new Image[pageCount];

                for (int i = 0; i < pageCount; i++)
                {
                    Image img = getTiffImage(sourceImage, i);
                    returnImages[i] = (Image)img.Clone();
                    img.Dispose();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                returnImages = null;
            }

            return returnImages;
        }

        #endregion

        #region Merge multiple single page TIFF to a single multi page TIFF


        public static bool mergeTiffPages(string[] sourceFiles, string targetFile, int position, string pos)
        {
            bool response = false;

            try
            {
                assignEncoder();

                //To Get Count
                int pageCount1 = getPageCount(sourceFiles[0]);
                int pageCount2 = getPageCount(sourceFiles[1]);


                //To Split Tiff Pages into Single Tiff
                Image sourceImage1 = Bitmap.FromFile(sourceFiles[0]);
                Image[] sourceImages1 = splitTiffPages(sourceImage1);

                Image sourceImage2 = Bitmap.FromFile(sourceFiles[1]);
                Image[] sourceImages2 = splitTiffPages(sourceImage2);

                //To attach Before the current document
                if (position == 0 && pos == "Before")
                {
                    Image finalImage = Image.FromFile((sourceFiles[1]));
                    finalImage.Save(targetFile, tifImageCodecInfo, tifEncoderParametersPage1);
                    for (int i = 1; i < pageCount2; i++)
                    {
                        Image img2= sourceImages2[i];
                        finalImage.SaveAdd(img2, tifEncoderParametersPageX);
                        img2.Dispose();
                    }
                    for (int j = 0; j < pageCount1; j++)
                    {
                        Image img1 = sourceImages1[j];
                        finalImage.SaveAdd(img1, tifEncoderParametersPageX);
                        img1.Dispose();
                    }
                    finalImage.SaveAdd(tifEncoderParametersPageLast);
                    finalImage.Dispose();
                    response = true;
                }
                else if (position == pageCount1 && pos == "After") //To attach After the current document
                {
                    Image finalImage = Image.FromFile((sourceFiles[0]));
                    finalImage.Save(targetFile, tifImageCodecInfo, tifEncoderParametersPage1);
                    for (int i = 1; i < pageCount1; i++)
                    {
                        Image img1 = sourceImages1[i];
                        finalImage.SaveAdd(img1, tifEncoderParametersPageX);
                        img1.Dispose();
                    }
                    for (int j = 0; j < pageCount2; j++)
                    {
                        Image img2 = sourceImages2[j];
                        finalImage.SaveAdd(img2, tifEncoderParametersPageX);
                        img2.Dispose();
                    }
                    finalImage.SaveAdd(tifEncoderParametersPageLast);
                    finalImage.Dispose();
                    response = true;
                }
                else
                {
                    Image finalImage = Image.FromFile((sourceFiles[0]));
                    finalImage.Save(targetFile, tifImageCodecInfo, tifEncoderParametersPage1);

                    // All other pages
                    for (int i = 1; i < pageCount1; i++)
                    {
                        if (i == position)
                        {
                            for (int j = 0; j < pageCount2; j++)
                            {
                                Image img2 = sourceImages2[j];
                                finalImage.SaveAdd(img2, tifEncoderParametersPageX);
                                img2.Dispose();
                            }
                        }
                        Image img = sourceImages1[i];
                        finalImage.SaveAdd(img, tifEncoderParametersPageX);
                        img.Dispose();
                    }

                    // Last page
                    finalImage.SaveAdd(tifEncoderParametersPageLast);
                    finalImage.Dispose();
                    response = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response = false;
            }

            return response;
        }
        public static bool deleteTiffPages(string[] sourceFiles, string targetFile, int position)
        {
            bool response = false;

            try
            {
                assignEncoder();

                // If only 1 page was passed, copy directly to output
                //if (sourceFiles.Length == 1)
                //{
                //    File.Copy(sourceFiles[0], targetFile, true);
                //    return true;
                //}

                int pageCount1 = getPageCount(sourceFiles[0]);

                Image sourceImage1 = Bitmap.FromFile(sourceFiles[0]);
                Image[] sourceImages1 = splitTiffPages(sourceImage1);
                Image finalImage;
                // First page
                finalImage = Image.FromFile((sourceFiles[0]));
                //Image finalImage = Image.FromFile("");
                finalImage.Save(targetFile, tifImageCodecInfo, tifEncoderParametersPage1);

                // All other pages
                for (int i = 1; i < pageCount1; i++)
                {
                    if (i == position)
                    {
                        continue;
                    }
                    Image img = sourceImages1[i];
                    finalImage.SaveAdd(img, tifEncoderParametersPageX);
                    img.Dispose();
                }

                // Last page
                finalImage.SaveAdd(tifEncoderParametersPageLast);
                finalImage.Dispose();
                response = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response = false;
            }

            return response;
        }
        public static bool mergeTiffPages(string sourceFile, string targetFile, int[] pageNumbers)
        {
            bool response = false;

            try
            {
                assignEncoder();

                // Get individual Images from the original image 
                Image sourceImage = Bitmap.FromFile(sourceFile);
                MemoryStream ms = new MemoryStream();
                Image[] sourceImages = new Image[pageNumbers.Length];
                Guid guid = sourceImage.FrameDimensionsList[0];
                FrameDimension objDimension = new FrameDimension(guid);
                for (int i = 0; i < pageNumbers.Length; i++)
                {
                    sourceImage.SelectActiveFrame(objDimension, pageNumbers[i]);
                    sourceImage.Save(ms, ImageFormat.Tiff);
                    sourceImages[i] = Image.FromStream(ms);
                }

                // Merge individual Images into one Image 
                // First page
                Image finalImage = sourceImages[0];
                finalImage.Save(targetFile, tifImageCodecInfo, tifEncoderParametersPage1);
                // All other pages
                for (int i = 1; i < pageNumbers.Length; i++)
                {
                    finalImage.SaveAdd(sourceImages[i], tifEncoderParametersPageX);
                }
                // Last page
                finalImage.SaveAdd(tifEncoderParametersPageLast);
                finalImage.Dispose();

                response = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response;
        }

        public static bool mergeTiffPagesAlternate(string sourceFile, string targetFile, int[] pageNumbers)
        {
            bool response = false;

            try
            {
                // Initialize the encoders, occurs once for the lifetime of the class
                assignEncoder();

                // Get individual Images from the original image 
                Image sourceImage = Bitmap.FromFile(sourceFile);
                MemoryStream[] msArray = new MemoryStream[pageNumbers.Length];
                Guid guid = sourceImage.FrameDimensionsList[0];
                FrameDimension objDimension = new FrameDimension(guid);
                for (int i = 0; i < pageNumbers.Length; i++)
                {
                    msArray[i] = new MemoryStream();
                    sourceImage.SelectActiveFrame(objDimension, pageNumbers[i]);
                    sourceImage.Save(msArray[i], ImageFormat.Tiff);
                }

                // Merge individual page streams into single stream
                MemoryStream ms = mergeTiffStreams(msArray);
                Image targetImage = Bitmap.FromStream(ms);
                targetImage.Save(targetFile);

                response = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return response;
        }

        public static System.IO.MemoryStream mergeTiffStreams(System.IO.MemoryStream[] tifsStream)
        {
            EncoderParameters ep = null;
            System.IO.MemoryStream singleStream = new System.IO.MemoryStream();

            try
            {
                assignEncoder();

                Image imgTif = Image.FromStream(tifsStream[0]);

                if (tifsStream.Length > 1)
                {
                    // Multi-Frame
                    ep = new EncoderParameters(2);
                    ep.Param[0] = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.MultiFrame);
                    ep.Param[1] = new EncoderParameter(tifEncoderCompression, (long)EncoderValue.CompressionRle);
                }
                else
                {
                    // Single-Frame
                    ep = new EncoderParameters(1);
                    ep.Param[0] = new EncoderParameter(tifEncoderCompression, (long)EncoderValue.CompressionRle);
                }

                //Save the first page
                imgTif.Save(singleStream, tifImageCodecInfo, ep);

                if (tifsStream.Length > 1)
                {
                    ep = new EncoderParameters(2);
                    ep.Param[0] = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.FrameDimensionPage);

                    //Add the rest of pages
                    for (int i = 1; i < tifsStream.Length; i++)
                    {
                        Image pgTif = Image.FromStream(tifsStream[i]);

                        ep.Param[1] = new EncoderParameter(tifEncoderCompression, (long)EncoderValue.CompressionRle);

                        imgTif.SaveAdd(pgTif, ep);
                    }

                    ep = new EncoderParameters(1);
                    ep.Param[0] = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.Flush);
                    imgTif.SaveAdd(ep);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ep != null)
                {
                    ep.Dispose();
                }
            }

            return singleStream;
        }

        #endregion

        #region Internal support functions

        private static void assignEncoder()
        {
            try
            {
                if (encoderAssigned == true)
                    return;

                foreach (ImageCodecInfo ici in ImageCodecInfo.GetImageEncoders())
                {
                    if (ici.MimeType == "image/tiff")
                    {
                        tifImageCodecInfo = ici;
                    }
                }

                tifEncoderSaveFlag = System.Drawing.Imaging.Encoder.SaveFlag;
                tifEncoderCompression = System.Drawing.Imaging.Encoder.Compression;
                tifEncoderColorDepth = System.Drawing.Imaging.Encoder.ColorDepth;

                tifEncoderParameterMultiFrame = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.MultiFrame);
                tifEncoderParameterFrameDimensionPage = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.FrameDimensionPage);
                tifEncoderParameterFlush = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.Flush);
                tifEncoderParameterCompression = new EncoderParameter(tifEncoderCompression, (long)EncoderValue.CompressionRle);
                tifEncoderParameterLastFrame = new EncoderParameter(tifEncoderSaveFlag, (long)EncoderValue.LastFrame);
                tifEncoderParameter24BPP = new EncoderParameter(tifEncoderColorDepth, (long)24);
                tifEncoderParameter1BPP = new EncoderParameter(tifEncoderColorDepth, (long)8);

                // ******************************************************************* //
                // *** Have only 1 of the following 3 groups assigned for encoders *** //
                // ******************************************************************* //

                // Regular 
                tifEncoderParametersPage1 = new EncoderParameters(1);
                tifEncoderParametersPage1.Param[0] = tifEncoderParameterMultiFrame;
                tifEncoderParametersPageX = new EncoderParameters(1);
                tifEncoderParametersPageX.Param[0] = tifEncoderParameterFrameDimensionPage;
                tifEncoderParametersPageLast = new EncoderParameters(1);
                tifEncoderParametersPageLast.Param[0] = tifEncoderParameterFlush;

                //// Regular 
                //tifEncoderParametersPage1 = new EncoderParameters(2); 
                //tifEncoderParametersPage1.Param[0] = tifEncoderParameterMultiFrame;
                //tifEncoderParametersPage1.Param[1] = tifEncoderParameterCompression;
                //tifEncoderParametersPageX = new EncoderParameters(2); 
                //tifEncoderParametersPageX.Param[0] = tifEncoderParameterFrameDimensionPage; 
                //tifEncoderParametersPageX.Param[1] = tifEncoderParameterCompression;
                //tifEncoderParametersPageLast = new EncoderParameters(2); 
                //tifEncoderParametersPageLast.Param[0] = tifEncoderParameterFlush;
                //tifEncoderParametersPageLast.Param[1] = tifEncoderParameterLastFrame;

                //// 24 BPP Color 
                //tifEncoderParametersPage1 = new EncoderParameters(2); 
                //tifEncoderParametersPage1.Param[0] = tifEncoderParameterMultiFrame;
                //tifEncoderParametersPage1.Param[1] = tifEncoderParameter24BPP;
                //tifEncoderParametersPageX = new EncoderParameters(2); 
                //tifEncoderParametersPageX.Param[0] = tifEncoderParameterFrameDimensionPage;
                //tifEncoderParametersPageX.Param[1] = tifEncoderParameter24BPP;
                //tifEncoderParametersPageLast = new EncoderParameters(2); 
                //tifEncoderParametersPageLast.Param[0] = tifEncoderParameterFlush;
                //tifEncoderParametersPageLast.Param[1] = tifEncoderParameterLastFrame;

                //// 1 BPP BW 
                //tifEncoderParametersPage1 = new EncoderParameters(2); 
                //tifEncoderParametersPage1.Param[0] = tifEncoderParameterMultiFrame;
                //tifEncoderParametersPage1.Param[1] = tifEncoderParameterCompression;
                //tifEncoderParametersPageX = new EncoderParameters(2); 
                //tifEncoderParametersPageX.Param[0] = tifEncoderParameterFrameDimensionPage;
                //tifEncoderParametersPageX.Param[1] = tifEncoderParameterCompression;
                //tifEncoderParametersPageLast = new EncoderParameters(2); 
                //tifEncoderParametersPageLast.Param[0] = tifEncoderParameterFlush;
                //tifEncoderParametersPageLast.Param[1] = tifEncoderParameterLastFrame;

                encoderAssigned = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        private static Bitmap ConvertToGrayscale(Bitmap source)
        {
            try
            {
                Bitmap bm = new Bitmap(source.Width, source.Height);
                Graphics g = Graphics.FromImage(bm);

                ColorMatrix cm = new ColorMatrix(new float[][] { new float[] { 0.5f, 0.5f, 0.5f, 0, 0 }, new float[] { 0.5f, 0.5f, 0.5f, 0, 0 }, new float[] { 0.5f, 0.5f, 0.5f, 0, 0 }, new float[] { 0, 0, 0, 1, 0, 0 }, new float[] { 0, 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 0, 1 } });
                ImageAttributes ia = new ImageAttributes();
                ia.SetColorMatrix(cm);
                g.DrawImage(source, new Rectangle(0, 0, source.Width, source.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, ia);
                g.Dispose();

                return bm;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        #endregion
    }
}