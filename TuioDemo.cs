/*
	TUIO C# Demo - part of the reacTIVision project
	Copyright (c) 2005-2016 Martin Kaltenbrunner <martin@tuio.org>

	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using TUIO;
using System.IO;
using System.Drawing.Drawing2D;
using NAudio.Wave;
using System.Threading;
public class TuioDemo : Form, TuioListener
{
    private TuioClient client;
    private int[] Music = new int[200];

    private Dictionary<long, TuioObject> objectList;
    private Dictionary<long, TuioCursor> cursorList;
    private Dictionary<long, TuioBlob> blobList;

    private string[] profiles = new string[3] { "Profile 1", "Profile2", "Profile 3" };

    private WaveOutEvent[] outputDevices = new WaveOutEvent[200];// Explicitly declare the type
    private AudioFileReader audioFile = null; // Explicitly declare the type

    private string profileFolder;

    private string[] audioFiles;
    private string[] audioFiles2;
    private string audioFileName;

    public static int width, height;
    private int window_width = 640;
    private int window_height = 480;
    private int window_left = 0;
    private int window_top = 0;
    private int screen_width = Screen.PrimaryScreen.Bounds.Width;
    private int screen_height = Screen.PrimaryScreen.Bounds.Height;

    public int f = 0;

    private bool fullscreen;
    private bool verbose;
    private string folderName;

    Font font = new Font("Arial", 10.0f);
    SolidBrush fntBrush = new SolidBrush(Color.White);
    SolidBrush bgrBrush = new SolidBrush(Color.FromArgb(0, 0, 64));
    SolidBrush curBrush = new SolidBrush(Color.FromArgb(192, 0, 192));
    SolidBrush objBrush = new SolidBrush(Color.FromArgb(64, 0, 0));
    SolidBrush blbBrush = new SolidBrush(Color.FromArgb(64, 64, 64));
    Pen curPen = new Pen(new SolidBrush(Color.Blue), 1);

    public TuioDemo(int port)
    {

        verbose = false;
        fullscreen = false;
        width = window_width;
        height = window_height;

        this.ClientSize = new System.Drawing.Size(width, height);
        this.Name = "Music Player";
        this.Text = "Music Player";

        this.Closing += new CancelEventHandler(Form_Closing);
        this.KeyDown += new KeyEventHandler(Form_KeyDown);
        this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                        ControlStyles.UserPaint |
                        ControlStyles.DoubleBuffer, true);

        objectList = new Dictionary<long, TuioObject>(128);
        cursorList = new Dictionary<long, TuioCursor>(128);
        blobList = new Dictionary<long, TuioBlob>(128);

        client = new TuioClient(port);
        client.addTuioListener(this);

        client.connect();
    }

    private void TuioDemo_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        string text = "Welcome" + folderName;
        Font fonttext = new Font("Arial", 18);
        Brush textBrush = Brushes.White;
        int textY = 0;
        int textX = this.ClientSize.Width / 2 - 100;
        g.DrawString(text, fonttext, textBrush, new PointF(textX, textY));

        String Text = "list of your songs:";
        Font fonttext2 = new Font("Arial", 14);
        Brush textBrush2 = Brushes.White;
        int textY2 = this.ClientSize.Height - 200;
        int textX2 = this.ClientSize.Width / 2 - 100;

        g.DrawString(Text, fonttext2, textBrush2, new PointF(textX2, textY2));
        //string[] Text3 = new string[20];
        for (int i = 0; i < audioFiles.Length; i++)
        {
            audioFileName = Path.GetFileName(audioFiles[i]);
            Console.WriteLine(audioFileName);

            textY2 += 20;

            g.DrawString(audioFileName.ToString(), fonttext2, textBrush2, new PointF(textX2, textY2));
        }
    }

    private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
    {

        if (e.KeyData == Keys.F1)
        {
            if (fullscreen == false)
            {

                width = screen_width;
                height = screen_height;

                window_left = this.Left;
                window_top = this.Top;

                this.FormBorderStyle = FormBorderStyle.None;
                this.Left = 0;
                this.Top = 0;
                this.Width = screen_width;
                this.Height = screen_height;

                fullscreen = true;
            }
            else
            {

                width = window_width;
                height = window_height;

                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Left = window_left;
                this.Top = window_top;
                this.Width = window_width;
                this.Height = window_height;

                fullscreen = false;
            }
        }
        else if (e.KeyData == Keys.Escape)
        {
            this.Close();

        }
        else if (e.KeyData == Keys.V)
        {
            verbose = !verbose;
        }

    }

    private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        client.removeTuioListener(this);

        client.disconnect();
        System.Environment.Exit(0);
    }

    public void addTuioObject(TuioObject o)
    {
        Console.WriteLine("TUIO marker detected: " + o.SymbolID);
        lock (objectList)
        {
            objectList.Add(o.SessionID, o);

        }
        if (verbose) Console.WriteLine("add obj " + o.SymbolID + " (" + o.SessionID + ") " + o.X + " " + o.Y + " " + o.Angle);

    }

    public void updateTuioObject(TuioObject o)
    {

        if (verbose) Console.WriteLine("set obj " + o.SymbolID + " " + o.SessionID + " " + o.X + " " + o.Y + " " + o.Angle + " " + o.MotionSpeed + " " + o.RotationSpeed + " " + o.MotionAccel + " " + o.RotationAccel);
        float volume = (float)(Math.Sin(o.Angle));  // Sine of the angle
        volume = Math.Min(1.0f, Math.Max(0.0f, (volume + 1) / 2));  // Normalize to range [0, 1]

        // Set the volume for the audio output
        if (outputDevices[o.SessionID] != null)
        {
            outputDevices[o.SessionID].Volume = volume;  // Adjust volume directly
            Console.WriteLine("Volume set to: " + volume);
        }
    }

    public void removeTuioObject(TuioObject o)
    {
        foreach (TuioObject tobj in objectList.Values)
        {
            if (Music[tobj.SessionID] == 1 && outputDevices[tobj.SessionID] != null)
            {
                outputDevices[tobj.SessionID].Stop();
                break;
            }
            else
            {
                Console.WriteLine("NO music in " + tobj.SessionID.ToString() + "____i_______");
            }
        }

        lock (objectList)
        {
            objectList.Remove(o.SessionID);
        }


        if (verbose) Console.WriteLine("del obj " + o.SymbolID + " (" + o.SessionID + ")");
    }

    public void addTuioCursor(TuioCursor c)
    {
        lock (cursorList)
        {
            cursorList.Add(c.SessionID, c);
        }
        if (verbose) Console.WriteLine("add cur " + c.CursorID + " (" + c.SessionID + ") " + c.X + " " + c.Y);
    }

    public void updateTuioCursor(TuioCursor c)
    {
        if (verbose) Console.WriteLine("set cur " + c.CursorID + " (" + c.SessionID + ") " + c.X + " " + c.Y + " " + c.MotionSpeed + " " + c.MotionAccel);
    }

    public void removeTuioCursor(TuioCursor c)
    {
        lock (cursorList)
        {
            cursorList.Remove(c.SessionID);
        }
        if (verbose) Console.WriteLine("del cur " + c.CursorID + " (" + c.SessionID + ")");
    }

    public void addTuioBlob(TuioBlob b)
    {
        lock (blobList)
        {
            blobList.Add(b.SessionID, b);
        }
        if (verbose) Console.WriteLine("add blb " + b.BlobID + " (" + b.SessionID + ") " + b.X + " " + b.Y + " " + b.Angle + " " + b.Width + " " + b.Height + " " + b.Area);
    }

    public void updateTuioBlob(TuioBlob b)
    {

        if (verbose) Console.WriteLine("set blb " + b.BlobID + " (" + b.SessionID + ") " + b.X + " " + b.Y + " " + b.Angle + " " + b.Width + " " + b.Height + " " + b.Area + " " + b.MotionSpeed + " " + b.RotationSpeed + " " + b.MotionAccel + " " + b.RotationAccel);
    }

    public void removeTuioBlob(TuioBlob b)
    {
        lock (blobList)
        {
            blobList.Remove(b.SessionID);
        }
        if (verbose) Console.WriteLine("del blb " + b.BlobID + " (" + b.SessionID + ")");
    }

    public void refresh(TuioTime frameTime)
    {
        Invalidate();
    }



    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        // Getting the graphics object
        Graphics g = pevent.Graphics;
        g.FillRectangle(bgrBrush, new Rectangle(0, 0, width, height));

        // draw the cursor path
        if (cursorList.Count > 0)
        {
            lock (cursorList)
            {
                foreach (TuioCursor tcur in cursorList.Values)
                {
                    List<TuioPoint> path = tcur.Path;
                    TuioPoint current_point = path[0];

                    for (int i = 0; i < path.Count; i++)
                    {
                        TuioPoint next_point = path[i];
                        g.DrawLine(curPen, current_point.getScreenX(width), current_point.getScreenY(height), next_point.getScreenX(width), next_point.getScreenY(height));
                        current_point = next_point;
                    }
                    g.FillEllipse(curBrush, current_point.getScreenX(width) - height / 100, current_point.getScreenY(height) - height / 100, height / 50, height / 50);
                    g.DrawString(tcur.CursorID + "", font, fntBrush, new PointF(tcur.getScreenX(width) - 10, tcur.getScreenY(height) - 10));
                }
            }
        }

        // draw the objects
        string objectImagePath;
        string backgroundImagePath;
        //string audioFilePath;
        //string Profilename;
        if (objectList.Count > 0)
        {
            lock (objectList)
            {
                foreach (TuioObject tobj in objectList.Values)
                {
                    int ox = tobj.getScreenX(width);
                    int oy = tobj.getScreenY(height);
                    int size = height / 10;

                    switch (tobj.SymbolID)
                    {
                        case 1:
                            //objectImagePath = Path.Combine(Environment.CurrentDirectory, "bg1.jpg");
                            //backgroundImagePath = Path.Combine(Environment.CurrentDirectory, "bg1.jpg");
                            Console.WriteLine("Playing audio..." + tobj.SessionID.ToString() + "___________");
                            PlayProfileAudio(tobj.SymbolID);


                            System.Threading.Thread.Sleep(1000);

                            break;
                        case 2:
                            backgroundImagePath = Path.Combine(Environment.CurrentDirectory, "bg1.jpg");
                            objectImagePath = Path.Combine(Environment.CurrentDirectory, "bg1.jpg");
                            PlayProfileAudio(tobj.SymbolID);
                            /// Music[tobj.SymbolID] = 1;
                            //audioFilePath = "04._Ana_Mosh_Anany.mp3";
                            //using (var audioFile = new AudioFileReader(audioFilePath))
                            //using (outputDevices[tobj.SessionID] = new WaveOutEvent())
                            //{
                            //    outputDevices[tobj.SessionID].Init(audioFile);
                            //    outputDevices[tobj.SessionID].Play();
                            //    //outputDevices[tobj.SymbolID].Volume=vol;
                            //    Music[tobj.SessionID] = 1;

                            //    Console.WriteLine("Playing audio..." + tobj.SessionID.ToString() + "___________");

                            //    // Keep the program running until the audio finishes playing
                            //    while (outputDevices[tobj.SessionID].PlaybackState == PlaybackState.Playing)
                            //    {

                            //    }

                            //}
                            System.Threading.Thread.Sleep(100);
                            break;
                        case 0:
                            backgroundImagePath = Path.Combine(Environment.CurrentDirectory, "bg1.jpg");
                            objectImagePath = Path.Combine(Environment.CurrentDirectory, "bg1.jpg");

                            System.Threading.Thread.Sleep(100);


                            break;

                        case 10:
                            if (f == 0)
                            {
                                Thread thread1 = new Thread(() => AudioPlay(profileFolder));
                                thread1.Start();
                                f = 1;
                            }
                            //thread1.Join();
                            //Console.WriteLine("a333333");
                            //AudioPlay(profileFolder);

                            break;

                        default:
                            // Use default rectangle for other IDs
                            g.FillRectangle(objBrush, new Rectangle(ox - size / 2, oy - size / 2, size, size));
                            g.DrawString(tobj.SymbolID + "", font, fntBrush, new PointF(ox - 10, oy - 10));


                            continue;
                    }

                    // Debugging the image paths
                    //Console.WriteLine($"Object Image Path: {objectImagePath}");
                    //Console.WriteLine($"Background Image Path: {backgroundImagePath}");

                    //    try
                    //    {
                    //        Draw background image without rotation
                    //        if (File.Exists(backgroundImagePath))
                    //        {
                    //            using (Image bgImage = Image.FromFile(backgroundImagePath))
                    //            {
                    //                if (tobj.SymbolID == 1)
                    //                {
                    //                    g.DrawImage(bgImage, new Rectangle(0, 0, width, height));

                    //                }
                    //                else if (tobj.SymbolID == 0)
                    //                {
                    //                    g.DrawImage(bgImage, new Rectangle(width / 2, 0, width / 2, height));
                    //                }
                    //                // Draw the rotated object
                    //                else
                    //                {
                    //                    g.DrawImage(bgImage, new Rectangle(ox - size / 2, oy - size / 2, size, size));
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Console.WriteLine($"Background image not found: {backgroundImagePath}");
                    //        }

                    //        // Draw object image with rotation
                    //        if (File.Exists(objectImagePath))
                    //        {
                    //            using (Image objectImage = Image.FromFile(objectImagePath))
                    //            {
                    //                // Save the current state of the graphics object
                    //                GraphicsState state = g.Save();

                    //                // Apply transformations for rotation
                    //                g.TranslateTransform(ox, oy);
                    //                g.RotateTransform((float)(tobj.Angle / Math.PI * 180.0f));
                    //                g.TranslateTransform(-ox, -oy);

                    //                // Draw the rotated object
                    //                g.DrawImage(objectImage, new Rectangle(ox - size / 2, oy - size / 2, size, size));

                    //                // Restore the graphics state
                    //                g.Restore(state);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Console.WriteLine($"Object image not found: {objectImagePath}");
                    //            // Fall back to drawing a rectangle
                    //            //g.FillRectangle(objBrush, new Rectangle(ox - size / 2, oy - size / 2, size, size));
                    //        }
                    //    }
                    //    catch
                    //    {
                    //        Console.WriteLine("alooooo");
                    //    }
                    //}


                }
            }
        }



        // draw the blobs
        if (blobList.Count > 0)
        {
            lock (blobList)
            {
                foreach (TuioBlob tblb in blobList.Values)
                {
                    int bx = tblb.getScreenX(width);
                    int by = tblb.getScreenY(height);
                    float bw = tblb.Width * width;
                    float bh = tblb.Height * height;

                    g.TranslateTransform(bx, by);
                    g.RotateTransform((float)(tblb.Angle / Math.PI * 180.0f));
                    g.TranslateTransform(-bx, -by);

                    g.FillEllipse(blbBrush, bx - bw / 2, by - bh / 2, bw, bh);

                    g.TranslateTransform(bx, by);
                    g.RotateTransform(-1 * (float)(tblb.Angle / Math.PI * 180.0f));
                    g.TranslateTransform(-bx, -by);

                    g.DrawString(tblb.BlobID + "", font, fntBrush, new PointF(bx, by));
                }
            }
        }
    }

    private void AudioPlay(string profileFolder)
    {
        if (Directory.Exists(profileFolder))
        {
            audioFiles2 = Directory.GetFiles(profileFolder, "*.mp3");
            foreach (string audio in audioFiles2)
            {
                string audioFilePath = audio; // Full path of the audio file
                string audioFileName = Path.GetFileName(audioFilePath);
                Console.WriteLine("Playing audio file: " + audioFileName);

                using (var audioFile = new AudioFileReader(audioFilePath)) // Use the full path here
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    // Wait until the audio playback is finished before moving to the next file
                    while (outputDevice.PlaybackState == PlaybackState.Playing )
                    {
                        System.Threading.Thread.Sleep(100); // Sleep to avoid busy-waiting
                    }
                    f = 0;
                    Console.WriteLine("ana tl3tttt");
                }
            }
        }
        else
        {
            Console.WriteLine("Directory does not exist.");
        }
    }


    private void PlayProfileAudio(long SymbolID)
    {
        profileFolder = Path.Combine(Environment.CurrentDirectory, "Profile " + SymbolID.ToString());
        Console.WriteLine("Profile Folder: " + profileFolder);

        folderName = Path.GetFileName(profileFolder);
        Console.WriteLine("Folder Name: " + folderName);


        if (Directory.Exists(profileFolder))
        {

            audioFiles = Directory.GetFiles(profileFolder, "*.mp3");

            using (Graphics g2 = this.CreateGraphics())
            {
                TuioDemo_Paint(this, new PaintEventArgs(g2, this.ClientRectangle));
            }

            int audioFileCount = audioFiles.Length;

            Console.WriteLine("Number of audio files: " + audioFileCount);


            foreach (string audioFile in audioFiles)
            {
                string audioFileName2 = Path.GetFileName(audioFile);
                Console.WriteLine("Playing audio file: " + audioFileName2);

            }
        }
        else
        {
            Console.WriteLine("Profile folder does not exist: " + profileFolder);
        }
    }


    private void InitializeComponent()
    {
        this.SuspendLayout();
        // 
        // TuioDemo
        // 
        this.ClientSize = new System.Drawing.Size(278, 244);
        this.Name = "TuioDemo";
        this.Load += new System.EventHandler(this.TuioDemo_Load);
        this.ResumeLayout(false);

    }

    private void TuioDemo_Load(object sender, EventArgs e)
    {

    }

    public static void Main(String[] argv)
    {
        int port = 0;
        switch (argv.Length)
        {
            case 1:
                port = int.Parse(argv[0], null);
                if (port == 0) goto default;
                break;
            case 0:
                port = 3333;
                break;
            default:
                Console.WriteLine("usage: mono TuioDemo [port]");
                System.Environment.Exit(0);
                break;
        }
        TuioDemo app = new TuioDemo(port);
        Application.Run(app);
    }
}