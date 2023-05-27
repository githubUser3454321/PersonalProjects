using AForge.Video;
using AForge.Video.DirectShow;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace InvisibleMouse;

public partial class MouseRunner : Form
{
    private VideoCaptureDevice videoSource;
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
        pictureBox1.Image?.Dispose();
        pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
    }

}