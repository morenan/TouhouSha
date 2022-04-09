using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace TouhouSha.Core
{
    public static class ImageHelper
    {
        static public string GetApplicationPath()
        {
            return System.IO.Directory.GetParent(System.Windows.Forms.Application.ExecutablePath).FullName;
        }

        static public ImageSource CreateImage(string filename)
        {
            string bmpfile = filename + ".bmp";
            string pngfile = filename + ".png";
            string jpgfile = filename + ".jpg";
            string jpegfile = filename + ".jpeg";
            if (!File.Exists(filename))
            {
                if (File.Exists(bmpfile)) filename = bmpfile;
                else if (File.Exists(pngfile)) filename = pngfile;
                else if (File.Exists(jpgfile)) filename = jpgfile;
                else if (File.Exists(jpegfile)) filename = jpegfile;
            }
            if (!File.Exists(filename)) return null;

            FileStream fs = null;
            try
            {
                fs = File.OpenRead(filename);
                switch (Path.GetExtension(filename))
                {
                    case ".png":
                        {
                            PngBitmapDecoder decoder = new PngBitmapDecoder(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                            return decoder.Frames.FirstOrDefault();
                        }
                    case ".bmp":
                        {
                            BmpBitmapDecoder decoder = new BmpBitmapDecoder(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                            return decoder.Frames.FirstOrDefault();
                        }
                    case ".jpg":
                    case ".jpeg":
                        {
                            JpegBitmapDecoder decoder = new JpegBitmapDecoder(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                            return decoder.Frames.FirstOrDefault();
                        }
                    default:
                        {
                            BitmapDecoder decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                            return decoder.Frames.FirstOrDefault();
                        }
                }
            }
            catch (Exception exce)
            {
                return null;
            }
            finally
            {
                fs?.Close();
            }
        }

        static public CardImage LoadCardImage(string subdirname, string name)
        {
            string dir = Path.Combine(GetApplicationPath(), "Images", subdirname, name);
            string imgfilename = Path.Combine(dir, name);
            string txtfilename = Path.Combine(dir, name + ".txt");
            ImageSource imgsource = CreateImage(imgfilename);
            StreamReader sr = null;
            CardImage cardimage = new CardImage();
            cardimage.Source = imgsource;
            try
            {
                sr = new StreamReader(txtfilename);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    int i1 = line.IndexOf('=');
                    if (i1 <= 0) continue;
                    string key = line.Substring(0, i1).Trim();
                    string value = line.Substring(i1 + 1).Trim();
                    switch (key.ToUpper())
                    {
                        case "AUTHOR":
                            cardimage.Author = value;
                            break;
                        case "PIXIV ID":
                            cardimage.PixivID = value;
                            break;
                        case "RECT":
                            cardimage.Rect = Rect.Parse(value.Substring(1, value.Length - 2));
                            break;
                    }
                }
                BitmapFrame bitmap = cardimage.Source as BitmapFrame;
                if (bitmap != null)
                {
                    int x = (int)(bitmap.PixelWidth * cardimage.Rect.X);
                    int y = (int)(bitmap.PixelHeight * cardimage.Rect.Y);
                    int w = (int)(bitmap.PixelWidth * cardimage.Rect.Width);
                    int h = (int)(bitmap.PixelHeight * cardimage.Rect.Height);
                    byte[] data = new byte[w * h * 4];
                    bitmap.CopyPixels(new Int32Rect(x, y, w, h), data, w * 4, 0);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        BitmapSource newbitmap = BitmapSource.Create(w, h, bitmap.DpiX, bitmap.DpiY, bitmap.Format, bitmap.Palette, data, w * 4);
                        cardimage.Source = newbitmap;
                    });
                }
            }
            catch (Exception exce)
            {

            }
            finally
            {
                sr?.Close();
            }
            return cardimage;
        }
    }
}
