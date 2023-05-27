using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;

namespace InvisibleMouse;

public partial class MouseRunner : Form
{
    private VideoCaptureDevice videoSource;
    int framerateHalver = 2;
    int FramerateBuffer = 0;
    public MouseRunner()
    {
        InitializeComponent();

        FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
        videoSource.NewFrame += VideoSource_NewFrame;
        videoSource.Start();
    }

    private void KillCam_Click(object sender, EventArgs e)
    {
        if (videoSource != null && videoSource.IsRunning)
            videoSource.SignalToStop();

    }
    private void KillAll_Click(object sender, EventArgs e)
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = "/C taskkill  /f /im InvisibleMouse.exe";
        process.StartInfo = startInfo;
        process.Start();
    }
    private void Start_Click_1(object sender, EventArgs e)
    {
        FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
        videoSource.NewFrame += VideoSource_NewFrame;
        videoSource.Start();
    }

    private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
    {
        if (framerateHalver != 0)
            if (framerateHalver <= FramerateBuffer++)
                FramerateBuffer = 0;
            else return;


        pictureBox1.Image?.Dispose();
        //pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        int.TryParse(textBox1.Text, out int i);
        ConvertImage(eventArgs.Frame, Color.FromArgb(250, 250, 250), Color.White, i == -1 ? 100 : i);
       // Bitmap image = new Bitmap(@"C:\Users\wenkm\AppData\Local\Temp\ModifiedPNG.PNG");
        
       // var newImage = (Bitmap)image.Clone();new Bitmap(@"C:\Users\wenkm\AppData\Local\Temp\ModifiedPNG.PNG")
        //image?.Dispose();
        pictureBox1.Image = new Bitmap(@"C:\Users\wenkm\AppData\Local\Temp\ModifiedPNG.PNG");
        ToLowestPixel((Bitmap)newImage.Clone(), Color.Black);

    }
    private static unsafe Bitmap ConvertImage(Bitmap bmp, Color source, Color targetColor, double threshold)
    {
        var thresh = threshold * threshold;
        var target = targetColor.ToArgb();

        using (bmp = new Bitmap(bmp))
        {
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            int sR = source.R, sG = source.G, sB = source.B;
            var length = (int*)data.Scan0 + bmp.Height * bmp.Width;

            for (var p = (int*)data.Scan0; p < length; p++)
            {
                var r = ((*p >> 16) & 255) - sR;
                var g = ((*p >> 8) & 255) - sG;
                var b = ((*p >> 0) & 255) - sB;

                if (r * r + g * g + b * b > thresh) continue;
                *p = target;
            }
            bmp.UnlockBits(data);
            bmp.Save(@"C:\Users\wenkm\AppData\Local\Temp\ModifiedPNG.PNG");

            return bmp;
        }
    }

    public static void ToLowestPixel(Bitmap image, Color lowestColor)
    {
        int lowestX = 0;
        int lowestY = 0;

        Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
        BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        IntPtr ptr = bmpData.Scan0;

        int bytes = Math.Abs(bmpData.Stride) * image.Height;
        byte[] rgbValues = new byte[bytes];

        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

        // Iterate through each pixel of the image
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                int index = y * bmpData.Stride + 4 * x;

                byte blue = rgbValues[index];
                byte green = rgbValues[index + 1];
                byte red = rgbValues[index + 2];

                Color pixelColor = Color.FromArgb(red, green, blue);

                if (pixelColor.GetBrightness() < 0.5)
                    if (pixelColor.GetBrightness() < lowestColor.GetBrightness())
                    {
                        lowestX = x;
                        lowestY = y;
                        lowestColor = pixelColor;
                    }

            }
        }
        image.UnlockBits(bmpData);
        Console.WriteLine("Lowest Dark Pixel: X = " + lowestX + ", Y = " + lowestY);

        //var cursor = new Cursor(handle: sa);
        //Cursor.Position = new Point(cursor.Position.X - 20, cursor.Position.Y - 20);

    }
}