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
using System.Runtime.CompilerServices;

namespace InvisibleMouse;

public partial class MouseRunner : Form
{
    private VideoCaptureDevice videoSource;
    int framerateHalver = 4;
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
        var im = new Bitmap(@"C:\Users\wenkm\AppData\Local\Temp\ModifiedPNG.PNG");
        pictureBox1.Image = im;
    
        var imCopy = new Bitmap(im.Width, im.Height);
        using (Graphics g = Graphics.FromImage(imCopy))
        {
            g.DrawImage(im, 0, 0);
        }

        GetBlackDots(imCopy);

        // Dispose the 'imCopy' object after using it
        imCopy.Dispose();
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
            try
            {
                bmp.Save(@"C:\Users\wenkm\AppData\Local\Temp\ModifiedPNG.PNG");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving the image: " + ex.Message);
            }

            return bmp;
        }
    }

    public List<String> GetBlackDots(Bitmap bitmapImage)
    {

        Color pixelColor;
        var list = new List<String>();
        for (int y = 0; y < bitmapImage.Height; y++)
        {
            for (int x = 0; x < bitmapImage.Width; x++)
            {
                pixelColor = bitmapImage.GetPixel(x, y);
                if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0)
                    list.Add(String.Format("x:{0} y:{1}", x, y));
            }
        }
        return list;
    }
}