using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Shakalizator.Servises
{
    public class VertorService
    {
        public Image FixedResol(Image imgPhoto, int width, int height)
        {
                    int sourceWidth = imgPhoto.Width;
                    int sourceHeight = imgPhoto.Height;

                    if (sourceWidth < sourceHeight) {
                        int temp = height;
                        height = width;
                        width = temp;
                    }

                    int sourceX = 0;
                    int sourceY = 0;
                    int destX = 0;
                    int destY = 0;

                    float nPercent = 0;
                    float nPercentW = 0;
                    float nPercentH = 0;

                    nPercentW = (float)width / (float)sourceWidth;
                    nPercentH = (float)height / (float)sourceHeight;
                    if (nPercentH < nPercentW) {
                        nPercent = nPercentH;
                        destX = Convert.ToInt16((width - sourceWidth * nPercent) / 2);
                    } else {
                        nPercent = nPercentW;
                        destY = Convert.ToInt16((height - sourceHeight * nPercent) / 2);
                    }

                    int destWidth = (int)(sourceWidth * nPercent);
                    int destHeight = (int)(sourceHeight * nPercent);

            using (Bitmap bmPhoto = new Bitmap(imgPhoto, width, height)) {
               
                Image bmImg = new Bitmap(bmPhoto,destWidth,destHeight);
                return bmImg;
            }
        }

        public Image ClearlyFixedResol(Image imgPhoto, int width, int height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = (float)width / (float)sourceWidth;
            nPercentH = (float)height / (float)sourceHeight;
            if (nPercentH > nPercentW) {
                nPercent = nPercentH;
                destX = Convert.ToInt16((width - sourceWidth * nPercent) / 2);
            } else {
                nPercent = nPercentW;
                destY = Convert.ToInt16((height - sourceHeight * nPercent) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            using (Bitmap bmPhoto = new Bitmap(imgPhoto, width, height)) {
                Image bmImg = new Bitmap(bmPhoto, destWidth, destHeight);
                int ErorH = 0;
                int ErorW = 0;
                int x1 = 0;
                int x2 = 0;
                int y1 = 0;
                int y2 = 0;
                if (bmImg.Height > height) {
                    ErorH = (bmImg.Height - height) / 2 - 1;
                    x2 = width;
                    y2 = height;
                } else {
                    ErorW = (bmImg.Width - width) / 2 - 1;
                    x2 = width;
                    y2 = height;
                }
                using (var result = new Bitmap(width, height)) {
                    using (var result2 = (Bitmap)bmImg) {
                        for (int i = x1; i < x2-1; i++) {
                            for (int j = y1; j < y2-1; j++) {
                                result.SetPixel(i - x1, j - y1, result2.GetPixel(i + ErorW, j + ErorH));
                            }
                        }
                        bmImg = NewBitmap(result);
                    }
                }
                return bmImg;
            }
        }

        public void FixedSize(List<string> imgPathList,int width1,int height1, int basesize, string savepath,int width2,int heigth2,bool isClearly)
        {
     
            if (!Directory.Exists(savepath)) {
                Directory.CreateDirectory(savepath);
            }
            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
            myStopwatch.Start(); //запуск
            foreach (string imgPath in imgPathList) {
                int size = basesize;
                int width = width1;
                int height = height1;
                using (FileStream fs = File.OpenRead(imgPath)) {
                    using (Image img = Image.FromStream(fs)) {

                        ImageFormat img_format = img.RawFormat;
                        ImageCodecInfo codec = GetEncoder(img_format);


                        if (codec == null) {
                            System.Windows.Forms.MessageBox.Show("Файл поврежден или не является изображением!");
                            return;
                        }

                        var quality = 100L;

                        var filename = Path.GetFileName(imgPath);

                        System.Drawing.Imaging.Encoder encoder = Encoder.Quality;//System.Drawing.Imaging.Encoder.Compression;
                        EncoderParameters codecParams = new EncoderParameters(1);
                        Image Reimg = img;
                        if (img.Width > img.Height && width2!=0 && heigth2!=0) {
                            width = width2;
                            height = heigth2;
                        }
                        if (width != 0 && height != 0) {
                            if (isClearly) {
                                Reimg = ClearlyFixedResol(img, width, height);
                            } else {
                                Reimg = FixedResol(img, width, height);
                            }
                        } else if (img.Width > 5000 || img.Height > 5000) {
                            Reimg = FixedResol(img, 5120, 4096);
                        }

                        var stream = new MemoryStream();
                        stream.SetLength(0);
                        var resized = 0L;
                        //100 процентов    

                        using (var stream2 = new MemoryStream()) {
                            codecParams.Param[0] = new EncoderParameter(encoder, quality);
                            img.Save(stream, codec, codecParams);
                            resized = stream.Length;
                            if (resized <= size) {
                                size = 0;
                            }
                        }

                        if (size != 0) {
                            double currentsize;
                            do {
                                stream = new MemoryStream();
                                codecParams.Param[0] = new EncoderParameter(encoder, quality);
                                Reimg.Save(stream, codec, codecParams);
                                resized = stream.Length;
                                currentsize = 100 - (((double)size / (double)resized) * 100);
                                
                                    if (currentsize >= 20 && quality > 20) { quality -= 10L; } 
                                    else {
                                        quality--;
                                    }
                               
                            } while (resized > size);
                        }
                        File.Delete(Path.Combine(savepath, filename));

                        if (size != 0) {
                            using (var file = File.Create(Path.Combine(savepath, filename))) {
                                stream.CopyTo(file);
                            }

                            Reimg.Save(Path.Combine(savepath, filename), codec, codecParams);
                        } else {
                            codecParams.Param[0] = new EncoderParameter(encoder, 100L);
                            Reimg.Save(Path.Combine(savepath, filename), codec, codecParams);
                        }
                      
                        stream.Close();
                        img.Dispose();
                    }
               }
            }
            myStopwatch.Stop();
            
        }


        public Image FixedResolForMinValue(Image imgPhoto, int width, int height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            if (sourceWidth < sourceHeight) {
                int temp = height;
                height = width;
                width = temp;
            }

            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = (float)width / (float)sourceWidth;
            nPercentH = (float)height / (float)sourceHeight;
            if (nPercentH < nPercentW) {
                nPercent = nPercentW;
                destX = Convert.ToInt16((width - sourceWidth * nPercent) / 2);
            } else {
                nPercent = nPercentH;
                destY = Convert.ToInt16((height - sourceHeight * nPercent) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            using (Bitmap bmPhoto = new Bitmap(imgPhoto, width, height)) {
              
                Image bmImg = new Bitmap(bmPhoto, destWidth, destHeight);
                return bmImg;
            }
        }

        public ImageCodecInfo GetEncoder(string mimeType)
        {

            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < encoders.Length; i++) {
                if (encoders[i].MimeType == mimeType) {
                    return encoders[i];
                }
            }
            return null;
        }

        public ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs) {
                if (codec.FormatID == format.Guid) {
                    return codec;
                }
            }
            return null;
        }

        public Bitmap GetBitmap(string path)
        {
            Bitmap source = null;

            try {
                byte[] imageData = File.ReadAllBytes(path);
                MemoryStream stream = new MemoryStream(imageData, false);
                source = new Bitmap(stream);
            }
            catch (Exception) {
            }

            return source;
        }

        public Bitmap NewBitmap(Image inputImg)
        {
            Bitmap source = null;

            try {
                source = new Bitmap(inputImg);
            }
            catch (Exception) {
                source = new Bitmap(inputImg,5120,4096);
            }

            return source;
        }

    }
}
