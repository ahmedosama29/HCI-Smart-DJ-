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
using System.Reflection;
using System.Text;
using System.Net.Sockets;
class Client
{
    int byteCT;
    public NetworkStream stream;
    byte[] sendData;
    public TcpClient client;

    public bool connectToSocket(string host, int portNumber)
    {
        try
        {
            client = new TcpClient(host, portNumber);
            stream = client.GetStream();
            Console.WriteLine("connection made ! with " + host);
            return true;
        }
        catch (System.Net.Sockets.SocketException e)
        {
            Console.WriteLine("Connection Failed: " + e.Message);
            return false;
        }
    }

    public string recieveMessage()
    {
        try
        {

            byte[] receiveBuffer = new byte[1024];
            int bytesReceived = stream.Read(receiveBuffer, 0, 1024);
            Console.WriteLine(bytesReceived);
            string data = Encoding.UTF8.GetString(receiveBuffer, 0, bytesReceived);
            Console.WriteLine(data);
            if (data.Contains("ahmed yasser"))
            {
                Console.WriteLine("Found 'ahmed yasser' in the message.");
                return "ahmed yasser";
            }
            if (data.Contains("omar yasser"))
            {
                Console.WriteLine("Found 'ahmed yasser' in the message.");
                return "omar yasser";
            }
            return data;
        }
        catch (System.Exception e)
        {

        }

        return null;
    }

}
public class TuioDemo : Form, TuioListener
{
    private TuioClient client;
    private int[] Music;// = new int[200];
    private int[] Catch;// = new int[200];
    bool stringW = false;
    int next_x_start = 0;
    int next_x_end = 0;
    int next_y_start = 0;
    int next_y_end = 0;
    int previous_x_start = 0;
    int previous_x_end = 0;
    int previous_y_start = 0;
    int previous_y_end = 0;
    int index = 0;
    int whichList = 0;
    int whichProfile = 0;
    int whichSong = 0;
    public float id74 = -1;
    public float id22 = -1;
    private Dictionary<long, TuioObject> objectList;
    private Dictionary<long, TuioCursor> cursorList;
    private Dictionary<long, TuioBlob> blobList;
    private int screen = 0;  // Track if profile mode is active
    private string[] profiles = new string[3] { "Profile 1", "Profile2", "Profile 3" };
    private List<Rectangle> playlistBoundaries = new List<Rectangle>();
    private WaveOutEvent outputDevice = null;// Explicitly declare the type
    private AudioFileReader audioFile = null; // Explicitly declare the type
    //
    private bool isPlaying = false;
    //
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
    public int f5 = 0;
    public int f4 = 0;
    public int f3 = 0;
    public int f2 = 0;
    public int flag_User = 0;
    public string flag_LastMusic = null;
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
        width = screen_width;
        height = screen_height;
        //this.WindowState = FormWindowState.M;
        //this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.LightGray; // Set background color to light gray
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
        //stream();
    }
    public void stream()
    {
        Client c = new Client();
        c.connectToSocket("localhost", 5000);
        string msg = "";
        msg = c.recieveMessage();
        if(msg == "ahmed yasser")
        {
            screen = 1;
            whichList = 0;
            whichProfile = 1;
        }
        if (msg == "omar yasser")
        {
            screen = 5;
            whichList = 0;
            whichProfile = 2;
        }
        Console.WriteLine("Connection Terminated !");
        c.stream.Close();
        c.client.Close();

        //while (true)
        //{
        //    if (msg == "q")
        //    {
        //        break;
        //    }
        //}
    }
    private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
    {


    }
    private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        client.removeTuioListener(this);

        client.disconnect();
        System.Environment.Exit(0);
    }
    public void addTuioObject(TuioObject o)
    {
        id74 = o.X;
        id22 = o.Y;
        //Console.WriteLine("TUIO marker detected: " + o.SymbolID + o.X);
        //Console.WriteLine("TUIO marker detected: " + o.X);
        lock (objectList)
        {
            objectList.Add(o.SessionID, o);

        }
        if (o.SymbolID == 1)  // When marker 1 is detected
        {
            screen = 1;
            whichProfile = 1;
            whichList = 0;
            whichSong = 0;
            Invalidate();  // Refresh the screen to show profile mode
        }
        if (o.SymbolID == 2)  // When marker 1 is detected
        {
            screen = 5;
            whichProfile = 2;
            whichList = 0;
            whichSong = 0;
            Invalidate();  // Refresh the screen to show profile mode
        }
        if (verbose) Console.WriteLine("add obj " + o.SymbolID + " (" + o.SessionID + ") " + o.X + " " + o.Y + " " + o.Angle);

    }
    public void updateTuioObject(TuioObject o)
    {
        id74 = o.X;
        id22 = o.Y;
        //Console.WriteLine("TUIO marker detected: " + o.X);
        if (outputDevice != null && isPlaying == true && o.SymbolID == 10)
        {


            if (verbose) Console.WriteLine("set obj " + o.SymbolID + " " + o.SessionID + " " + o.X + " " + o.Y + " " + o.Angle + " " + o.MotionSpeed + " " + o.RotationSpeed + " " + o.MotionAccel + " " + o.RotationAccel);
            float volume = (float)(Math.Sin(o.Angle));  // Sine of the angle
            volume = Math.Min(1.0f, Math.Max(0.0f, (volume + 1) / 2));  // Normalize to range [0, 1]

            // Set the volume for the audio output
            if (outputDevice != null)
            {
                outputDevice.Volume = volume;  // Adjust volume directly
                Console.WriteLine("Volume set to: " + volume);
            }
        }
    }
    public void removeTuioObject(TuioObject o)
    {
        //foreach (TuioObject tobj in objectList.Values)
        //{
        //    if (Music[tobj.SessionID] == 1 && outputDevices[tobj.SessionID] != null)
        //    {
        //        outputDevices[tobj.SessionID].Stop();
        //        break;
        //    }
        //    else
        //    {
        //        Console.WriteLine("NO music in " + tobj.SessionID.ToString() + "____i_______");
        //    }
        //}

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
    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e); // Ensures any other necessary behavior is preserved

        for (int i = 0; i < playlistBoundaries.Count; i++)
        {
            if (playlistBoundaries[i].Contains(e.Location))
            {
                // Check if the current screen is for User 1 or User 2
                if (screen == 1) // User 1's playlists
                {
                    screen = i + 2; // Set screen flag to 2, 3, or 4
                }
                /*else if (screen == 2 || screen == 3 || screen == 4) // If currently on User 1's playlists
                {
                    // Assuming this means User 1 can access playlists 2, 3, 4
                    screen = i + 2; // Set to 2, 3, 4
                }*/
                else if (screen == 5)// User 2's playlists
                {
                    screen = i + 6; // Set screen flag to 6, 7, or 8
                }

                Invalidate(); // Redraw the form to switch screens
                return;
            }
        }
    }
    public void refresh(TuioTime frameTime)
    {
        Invalidate();
    }
    private void DrawHomeScreen(Graphics g)
    {
        SolidBrush bgrBrush = new SolidBrush(Color.DarkGray); // Set to light gray
        g.FillRectangle(bgrBrush, new Rectangle(0, 0, width, height));
        // Define rectangle dimensions
        int rectWidth = 800;
        int rectHeight = 200;
        int rectX = (this.ClientSize.Width - rectWidth) / 2;
        int rectY = (this.ClientSize.Height - rectHeight) / 2;

        // Create a rounded rectangle path
        GraphicsPath roundedRectPath = new GraphicsPath();
        int cornerRadius = 40;
        roundedRectPath.AddArc(rectX, rectY, cornerRadius, cornerRadius, 180, 90);
        roundedRectPath.AddArc(rectX + rectWidth - cornerRadius, rectY, cornerRadius, cornerRadius, 270, 90);
        roundedRectPath.AddArc(rectX + rectWidth - cornerRadius, rectY + rectHeight - cornerRadius, cornerRadius, cornerRadius, 0, 90);
        roundedRectPath.AddArc(rectX, rectY + rectHeight - cornerRadius, cornerRadius, cornerRadius, 90, 90);
        roundedRectPath.CloseAllFigures();

        // Fill the rounded rectangle
        Brush rectBrush = new SolidBrush(Color.Black); // Adjust color as desired
        g.FillPath(rectBrush, roundedRectPath);

        // Draw the welcome text
        string welcomeText = "Welcome to our music player system";
        Font textFont = new Font("Arial", 16, FontStyle.Bold);
        Brush textBrush = Brushes.White; // Adjust text color as desired

        // Center the text
        SizeF textSize = g.MeasureString(welcomeText, textFont);
        float textX = rectX + (rectWidth - textSize.Width) / 2;
        float textY = rectY + (rectHeight - textSize.Height) / 2;
        g.DrawString(welcomeText, textFont, textBrush, textX, textY);
        // Load the image you want to display
        Image imageToDisplay = Image.FromFile("headset.png"); // Change the path accordingly

        // Calculate the position to draw the image
        int imageWidth = 100;  // Set the desired width of the image
        int imageHeight = 100; // Set the desired height of the image
        int imageX = rectX + (rectWidth - imageWidth) / 2; // Centered horizontally
        int imageY = rectY - imageHeight; // Position above the rectangle

        // Draw the image
        g.DrawImage(imageToDisplay, new Rectangle(imageX, imageY, imageWidth, imageHeight));

        // Remember to dispose of the image after use to free up resources
        imageToDisplay.Dispose();
        // Draw small text under the rectangle
        string loginText = "Login by connecting with Bluetooth or TUIO";
        Font loginFont = new Font("Arial", 12, FontStyle.Regular);
        Brush loginBrush = Brushes.White; // Adjust text color as desired

        // Calculate position for the login text
        SizeF loginTextSize = g.MeasureString(loginText, loginFont);
        float loginTextX = rectX + (rectWidth - loginTextSize.Width) / 2;
        float loginTextY = rectY + rectHeight + 10; // 10 pixels below the rectangle
        g.DrawString(loginText, loginFont, loginBrush, loginTextX, loginTextY);
    }
    private void DrawProfileScreen1(Graphics g)
    {
        playlistBoundaries.Clear(); // Clear previous rectangles
        SolidBrush bgrBrush = new SolidBrush(Color.DarkGray);
        g.FillRectangle(bgrBrush, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));

        // Draw the "Hello, User" text in a black rounded rectangle
        string welcomeText = " ";
        Font welcomeFont = new Font("Arial", 24, FontStyle.Bold);
        Brush welcomeTextBrush = Brushes.White;
        int welcomeRectWidth = this.ClientSize.Width / 2;
        int welcomeRectHeight = 80;
        int welcomeRectX = (this.ClientSize.Width - welcomeRectWidth) / 2;
        int welcomeRectY = 50;
        string[] playlistNames;
        string[] founders;
        string[] images;
        if (screen == 1) // User 1's playlists
        {
            welcomeText = "Hello, Ahmed!";
            playlistNames = new[] { "Summer Vibes", "Egyptian Hits", "Top 50" };
            founders = new[] { "by Omar Yasser", "by Yosef Hossam", "by Shika" };
            images = new[] { "song11.jpg", "song12.png", "song21.png" };
        }
        else // User 2's playlists
        {
            welcomeText = "Hello, Omar!";
            playlistNames = new[] { "Egyptian Nostalgia", "Hip Hop Beats", "90s Egyptian" };
            founders = new[] { "by Seif", "by Bola", "by Aly Ayman" };
            images = new[] { "song31.png", "song32.png", "song33.png" };
        }
        // Create rounded rectangle for welcome text
        GraphicsPath welcomeRoundedRect = new GraphicsPath();
        int cornerRadius = 40;
        welcomeRoundedRect.AddArc(welcomeRectX, welcomeRectY, cornerRadius, cornerRadius, 180, 90);
        welcomeRoundedRect.AddArc(welcomeRectX + welcomeRectWidth - cornerRadius, welcomeRectY, cornerRadius, cornerRadius, 270, 90);
        welcomeRoundedRect.AddArc(welcomeRectX + welcomeRectWidth - cornerRadius, welcomeRectY + welcomeRectHeight - cornerRadius, cornerRadius, cornerRadius, 0, 90);
        welcomeRoundedRect.AddArc(welcomeRectX, welcomeRectY + welcomeRectHeight - cornerRadius, cornerRadius, cornerRadius, 90, 90);
        welcomeRoundedRect.CloseAllFigures();
        g.FillPath(Brushes.Black, welcomeRoundedRect);

        // Draw the welcome text inside the rectangle
        SizeF welcomeTextSize = g.MeasureString(welcomeText, welcomeFont);
        float welcomeTextX = welcomeRectX + (welcomeRectWidth - welcomeTextSize.Width) / 2;
        float welcomeTextY = welcomeRectY + (welcomeRectHeight - welcomeTextSize.Height) / 2;
        g.DrawString(welcomeText, welcomeFont, welcomeTextBrush, welcomeTextX, welcomeTextY);

        // Define playlists based on the user or screen


        // Display playlists side-by-side
        int playlistWidth = 300;
        int playlistHeight = 300;
        int playlistsY = this.ClientSize.Height / 2 - 150; // Center vertically

        int totalPlaylistsWidth = playlistNames.Length * playlistWidth + (playlistNames.Length - 1) * 20; // 20px spacing
        int startX = (this.ClientSize.Width - totalPlaylistsWidth) / 2;

        for (int i = 0; i < playlistNames.Length; i++)
        {
            int playlistX = startX + i * (playlistWidth + 50);

            // Load and draw the playlist image
            Image playlistImage = Image.FromFile(images[i]);
            Rectangle playlistRect = new Rectangle(playlistX, playlistsY, playlistWidth, playlistHeight - 50);
            playlistBoundaries.Add(playlistRect);
            g.DrawImage(playlistImage, playlistRect); // Reserve space for text
            playlistImage.Dispose();
            if (whichList != 0 && whichList == i + 1) // Adjusted to 1-based index
            {
                g.DrawRectangle(new Pen(Color.Black, 10), playlistRect);
            }

            // Define rectangle for playlist and founder name
            int infoRectHeight = 60;
            Rectangle infoRect = new Rectangle(playlistX, playlistsY + playlistHeight - 50, playlistWidth, infoRectHeight);
            g.FillRectangle(Brushes.Black, infoRect);

            // Draw playlist name inside the rectangle
            Font playlistFont = new Font("Arial", 14, FontStyle.Bold);
            Brush playlistBrush = Brushes.White;
            string playlistName = playlistNames[i];
            SizeF playlistNameSize = g.MeasureString(playlistName, playlistFont);
            float playlistNameX = playlistX + (playlistWidth - playlistNameSize.Width) / 2;
            float playlistNameY = playlistsY + playlistHeight - 40; // Slight padding from the top
            g.DrawString(playlistName, playlistFont, playlistBrush, playlistNameX, playlistNameY);

            // Draw founder name inside the rectangle
            Font founderFont = new Font("Arial", 12, FontStyle.Regular);
            Brush founderBrush = Brushes.White;
            string founderName = founders[i];
            SizeF founderNameSize = g.MeasureString(founderName, founderFont);
            float founderNameX = playlistX + (playlistWidth - founderNameSize.Width) / 2;
            float founderNameY = playlistNameY + playlistNameSize.Height + 5; // Space below playlist name
            g.DrawString(founderName, founderFont, founderBrush, founderNameX, founderNameY);
        }
    }
    private void DrawProfileListScreen1(Graphics g)
    {
        // Background color for the entire profile screen
        SolidBrush bgrBrush = new SolidBrush(Color.DarkGray);
        g.FillRectangle(bgrBrush, new Rectangle(0, 0, width, height));

        string playlistImagePath;
        var songs = new[] { new { Number = 0, Title = "", Artist = "", Duration = "" } }; // Placeholder

        // Set the playlist image path and song data based on the screen value
        string userName;

        if (screen == 2)
        {
            userName = "Summer Vibes";
            playlistImagePath = "song11.jpg";
            songs = new[]
            {
                new { Number = 1, Title = "Enta Eh", Artist = "Nancy Ajram", Duration = "5:05" },
                new { Number = 2, Title = "AL Leila", Artist = "Amr Diab", Duration = "3:57" },
                new { Number = 3, Title = "El Ward El Blady", Artist = "Asala", Duration = "3:30" },
            };
        }
        else if (screen == 3)
        {
            userName = "Egyptian Hits";
            playlistImagePath = "song12.png";
            songs = new[]
            {
                new { Number = 1, Title = "Ashouf Feek Youm", Artist = "AbdElfatah Elgireny", Duration = "4:21" },
                new { Number = 2, Title = "Bahebik Wuachtini", Artist = "Hussein Al jasmi", Duration = "3:43" },
                new { Number = 3, Title = "Hazi Mn Alsama", Artist = "Amer Monieb", Duration = "3.56" }
            };
        }
        else if (screen == 4)
        {
            userName = "Top 50";
            playlistImagePath = "song21.png";
            songs = new[]
            {
                new { Number = 1, Title = "Lemeen Haeesh", Artist = "Wael Gassar", Duration = "5:13" },
                new { Number = 2, Title = "Asameek Elketeera", Artist = "Angham", Duration = "3:11" },
                new { Number = 3, Title = "Hatrooh", Artist = "Sherein", Duration = "4:57" },
            };
        }
        else if (screen == 6)
        {
            userName = "Egyptian Nostalgia";
            playlistImagePath = "song31.png";
            songs = new[]
            {
                new { Number = 1, Title = "Ana Mosh Anany", Artist = "Amr Diab", Duration = "4:51" },
                new { Number = 2, Title = "Tamenny Alaik", Artist = "Mohammed Foad", Duration = "4:28" },
                new { Number = 3, Title = "Akheran", Artist = "Sherein", Duration = "4:09" },
            };
        }
        else if (screen == 7)
        {
            userName = "Hip Hop Beats";
            playlistImagePath = "song32.png";
            songs = new[]
            {
                new { Number = 1, Title = "Ana Fi Gharam", Artist = "Sherien", Duration = "3:47" },
                new { Number = 2, Title = "Amel aih fi hayatk", Artist = "Amer Monieb", Duration = "3:15" },
                new { Number = 3, Title = "Lyna Raasa", Artist = "Karmen Seliman", Duration = "3:15" },
            };
        }
        else
        {
            userName = "90s Egyptian";
            playlistImagePath = "song33.png";
            songs = new[]
            {
                new { Number = 1, Title = "Monaya", Artist = "Mostafa Amar", Duration = "3:35" },
                new { Number = 2, Title = "Banadeek Taala", Artist = "Amr Diab", Duration = "3:15" },
                new { Number = 3, Title = "Betkhaby Leh", Artist = "Amr Diab", Duration = "4:16" },
            };
        }

        // Display the playlist image with a black frame
        int playlistImageWidth = 400;
        int playlistImageHeight = 400;
        int framePadding = 10; // Thickness of the black frame
        int imageX = (this.ClientSize.Width - playlistImageWidth) / 2;
        int imageY = 50;

        // Draw black frame
        Rectangle frameRect = new Rectangle(imageX - framePadding, imageY - framePadding, playlistImageWidth + 2 * framePadding, playlistImageHeight + 2 * framePadding);
        g.FillRectangle(Brushes.Black, frameRect);

        // Draw the playlist image
        Image playlistImage = Image.FromFile(playlistImagePath);
        g.DrawImage(playlistImage, new Rectangle(imageX, imageY, playlistImageWidth, playlistImageHeight));
        playlistImage.Dispose();

        // Profile User Name
        Font userNameFont = new Font("Arial", 48, FontStyle.Bold);
        Brush textBrush = Brushes.Black;
        SizeF userNameSize = g.MeasureString(userName, userNameFont);
        float userNameX = 60;
        float userNameY = imageY + playlistImageHeight + 30;
        g.DrawString(userName, userNameFont, textBrush, userNameX, userNameY);

        // Title for the song list
        string songListTitle = "Song List:";
        Font titleFont = new Font("Arial", 24, FontStyle.Bold);
        float titleX = 20;
        float titleY = userNameY + userNameSize.Height + 30;
        g.DrawString(songListTitle, titleFont, textBrush, titleX, titleY);

        // Font and brushes for song details
        Font songFont = new Font("Arial", 20);
        Brush whiteTextBrush = Brushes.White;
        Brush blackTextBrush = Brushes.White;
        Brush greenBrush = new SolidBrush(Color.Green);
        float songY = titleY + 50;
        Brush lightBrush = new SolidBrush(Color.Black);
        Brush darkBrush = new SolidBrush(Color.Black);
        int rectWidth = this.ClientSize.Width - 200;
        int rectHeight = 60;
        int cornerRadius = 40;

        // Draw each song in the list
        foreach (var song in songs)
        {
            Brush backgroundBrush = (song.Number == whichSong) ? greenBrush :
                         (song.Number % 2 == 0) ? lightBrush : darkBrush;
            Brush songTextBrush = (song.Number == whichSong) ? whiteTextBrush :
                                    (song.Number % 2 == 0) ? blackTextBrush : whiteTextBrush;

            // Rounded rectangle for song background
            GraphicsPath path = new GraphicsPath();
            int rectX = (int)titleX;
            int rectY = (int)songY;
            path.AddArc(rectX, rectY, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(rectX + rectWidth - cornerRadius, rectY, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(rectX + rectWidth - cornerRadius, rectY + rectHeight - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(rectX, rectY + rectHeight - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            path.CloseAllFigures();

            g.FillPath(backgroundBrush, path);

            // Draw song information
            string songInfo = $"{song.Number}. {song.Title} - {song.Artist} ({song.Duration})";
            g.DrawString(songInfo, songFont, songTextBrush, titleX + 10, songY + 15);
            songY += rectHeight + 20;
        }
    }
    private void DrawSongScreen1(Graphics g)
    {
        // Background color for the entire profile screen
        SolidBrush bgrBrush = new SolidBrush(Color.DarkGray);
        g.FillRectangle(bgrBrush, new Rectangle(0, 0, width, height));

        // Define the song data (simplified to show a single song)
        string playlistImagePath;
        string songTitle;
        string artistName;

        if (screen == 9)
        {
            playlistImagePath = "song11.jpg";
            songTitle = "Enta Eh";
            artistName = "Nancy Ajram";
        }
        else if (screen == 10)
        {
            playlistImagePath = "song21.png";
            songTitle = "AL Leila";
            artistName = "Amr Diab";
        }
        else if (screen == 11)
        {
            playlistImagePath = "song13.png";
            songTitle = "El Ward El Blady";
            artistName = "Asala";
        }
        else if (screen == 12)
        {
            playlistImagePath = "song12.png";
            songTitle = "Ashouf Feek Youm";
            artistName = "AbdElfatah Elgireny";
        }
        else if (screen == 13)
        {
            playlistImagePath = "song23.jpg";
            songTitle = "Bahebik Wuachtini";
            artistName = "Hussein Al jasmi";
        }
        else if (screen == 14)
        {
            playlistImagePath = "song31.png";
            songTitle = "Hazi Mn Alsama";
            artistName = "Amer Monieb";
        }
        else if (screen == 15)
        {
            playlistImagePath = "song41.png";
            songTitle = "Lemeen Haeesh";
            artistName = "Wael Gassar";
        }
        else if (screen == 16)
        {
            playlistImagePath = "song42.png";
            songTitle = "Asameek Elketeera";
            artistName = "Angham";
        }
        else if (screen == 17)
        {
            playlistImagePath = "song32.png";
            songTitle = "Hatrooh";
            artistName = "Sherine";
        }
        else if (screen == 18)
        {
            playlistImagePath = "song21.png";
            songTitle = "Ana Mosh Anany";
            artistName = "Amr Diab";
        }
        else if (screen == 19)
        {
            playlistImagePath = "song62.png";
            songTitle = "Tamenny Alaik";
            artistName = "Mohammed Foad";
        }
        else if (screen == 20)
        {
            playlistImagePath = "song12.png";
            songTitle = "Akheran";
            artistName = "Sherine";
        }
        else if (screen == 21)
        {
            playlistImagePath = "song12.png";
            songTitle = "Ana Fi Gharam";
            artistName = "Sherine";
        }
        else if (screen == 22)
        {
            playlistImagePath = "song31.png";
            songTitle = "Amel aih fi hayatk";
            artistName = "Amer Monieb";
        }
        else if (screen == 23)
        {
            playlistImagePath = "song73.png";
            songTitle = "Lyna Raasa";
            artistName = "Karmen Seliman";
        }
        else if (screen == 24)
        {
            playlistImagePath = "song81.png";
            songTitle = "Monaya";
            artistName = "Mostafa Amar";
        }
        else if (screen == 25)
        {
            playlistImagePath = "song21.png";
            songTitle = "Banadeek Taala";
            artistName = "Amr Diab";
        }
        else
        {
            playlistImagePath = "song21.png";
            songTitle = "Betkhaby Leh";
            artistName = "Amr Diab";
        }


        // Define image dimensions and position
        int imageWidth = 600;
        int imageHeight = 600;
        int framePadding = 10; // Frame thickness
        int imageX = (this.ClientSize.Width - imageWidth) / 2;
        int imageY = 50;

        // Draw black frame around the song image
        Rectangle frameRect = new Rectangle(imageX - framePadding, imageY - framePadding, imageWidth + 2 * framePadding, imageHeight + 2 * framePadding);
        g.FillRectangle(Brushes.Black, frameRect);

        // Load and draw the song image within the frame
        Image songImage = Image.FromFile(playlistImagePath);
        g.DrawImage(songImage, new Rectangle(imageX, imageY, imageWidth, imageHeight));
        songImage.Dispose();

        // Draw the song title below the image
        Font titleFont = new Font("Arial", 36, FontStyle.Bold);
        Brush textBrush = Brushes.White;
        SizeF titleSize = g.MeasureString(songTitle, titleFont);
        float titleX = (this.ClientSize.Width - titleSize.Width) / 2;
        float titleY = imageY + imageHeight + 20;
        g.DrawString(songTitle, titleFont, textBrush, titleX, titleY);

        // Draw the artist name in smaller font below the title
        Font artistFont = new Font("Arial", 12, FontStyle.Regular);
        SizeF artistSize = g.MeasureString(artistName, artistFont);
        float artistX = (this.ClientSize.Width - artistSize.Width) / 2;
        float artistY = titleY + titleSize.Height + 5;
        g.DrawString(artistName, artistFont, textBrush, artistX, artistY);

        // Draw control icons (Previous, Play/Pause, Next) within circles below the artist name
        Font controlFont = new Font("Arial", 22, FontStyle.Bold);
        string[] controls = { "⏮", "⏯", "⏭" }; // Previous, Play/Pause, Next symbols
        int circleDiameter = 100;
        int controlButtonSpacing = 120;
        int controlStartX = (this.ClientSize.Width / 2) - 135;
        float controlY = artistY + artistSize.Height + 40;

        for (int i = 0; i < controls.Length; i++)
        {
            float circleX = controlStartX + i * controlButtonSpacing - circleDiameter / 2;

            // Draw the circular background for each control
            RectangleF circleRect = new RectangleF(circleX, controlY, circleDiameter, circleDiameter);
            g.FillEllipse(Brushes.Black, circleRect); // Circle background color
            g.DrawEllipse(Pens.Black, circleRect);     // Circle outline

            // Draw the control icon inside the circle
            SizeF iconSize = g.MeasureString(controls[i], controlFont);
            float iconX = circleX + (circleDiameter - iconSize.Width) / 2;
            float iconY = controlY + (circleDiameter - iconSize.Height) / 2;
            g.DrawString(controls[i], controlFont, Brushes.White, iconX, iconY);
        }
    }
    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        // Getting the graphics object
        Graphics g = pevent.Graphics;
        SolidBrush bgrBrush = new SolidBrush(Color.DarkGray); // Set to light gray

        if(screen >= 9 && screen <= 26)
        {
            DrawSongScreen1(g);
        }
        if (screen == 2 || screen == 3 || screen == 4 || screen == 6 || screen == 7 || screen == 8)
        {
            DrawProfileListScreen1(g);
        }
        if (screen == 1 || screen == 5)
        {
            // Profile Screen Design
            DrawProfileScreen1(g);
        }
        if (screen == 0)
        {
            // Home Screen Design
            DrawHomeScreen(g);
        }



        // Draw other elements (cursors, objects, blobs) below this line...
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
                            //Console.WriteLine("Playing audio..." + tobj.SessionID.ToString() + "___________");
                            PlayProfileAudio(tobj.SymbolID);

                            flag_User = 1;
                            index = 0;
                            System.Threading.Thread.Sleep(100);

                            break;
                        case 2:
                            backgroundImagePath = Path.Combine(Environment.CurrentDirectory, "bg1.jpg");
                            objectImagePath = Path.Combine(Environment.CurrentDirectory, "bg1.jpg");
                            PlayProfileAudio(tobj.SymbolID);
                            flag_User = 2;
                            index = 0;
                            System.Threading.Thread.Sleep(100);
                            break;
                        case 0:
                            backgroundImagePath = Path.Combine(Environment.CurrentDirectory, "bg1.jpg");
                            objectImagePath = Path.Combine(Environment.CurrentDirectory, "bg1.jpg");

                            System.Threading.Thread.Sleep(100);


                            break;
                        case 6:
                            /*if (whichProfile == 1 || whichProfile == 2)
                            {
                                if (whichList < 3)
                                {
                                    whichList += 1;
                                }
                                else
                                {
                                    whichList = 0;
                                }
                            }*/
                            if (id22 > 0.8)
                            {
                                if (screen == 12 || screen == 13 || screen == 14)
                                {
                                    screen = 3;
                                }
                                else if (screen == 15 || screen == 16 || screen == 17)
                                {
                                    screen = 4;
                                }
                                else if (screen == 18 || screen == 19 || screen == 20)
                                {
                                    screen = 6;
                                }
                                else if (screen == 21 || screen == 22 || screen == 23)
                                {
                                    screen = 7;
                                }
                                else if (screen == 24 || screen == 25 || screen == 26)
                                {
                                    screen = 8;
                                }
                            }
                            Console.WriteLine("TUIO marker detected X: " + id74 + "TUIO marker detected Y: " + id22);
                            if (id22 < 0.2 && f4 == 0)
                            {
                                if (screen == 1)
                                {
                                    if (whichList == 1)
                                    {
                                        screen = 2;
                                    }
                                    else if (whichList == 2)
                                    {
                                        screen = 3;
                                    }
                                    else if (whichList == 3)
                                    {
                                        screen = 4;
                                    }
                                }
                                if (screen == 5)
                                {
                                    if (whichList == 1)
                                    {
                                        screen = 6;
                                    }
                                    else if (whichList == 2)
                                    {
                                        screen = 7;
                                    }
                                    else if (whichList == 3)
                                    {
                                        screen = 8;
                                    }
                                }
                                f4 = 1;
                                whichSong = 0;
                            }
                            if (id22 > 0.8 && f4 == 0)
                            {
                                if (screen == 1 || screen == 5)
                                {
                                    screen = 0;
                                }
                                else if (screen == 2 || screen == 3 || screen == 4)
                                {
                                    screen = 1;
                                }
                                else if (screen == 6 || screen == 7 || screen == 8)
                                {
                                    screen = 5;
                                }
                                else if (screen == 9 || screen == 10 || screen == 11)
                                {
                                    screen = 2;
                                }
                               
                                f4 = 1;

                            }
                            if (id22 > 0.4 && id22 < 0.6)
                            {
                                f4 = 0;
                            }
                            //////////////////////////////////////////////////////

                            if (id74 > 0.4 && id74 < 0.6)
                            {
                                f3 = 0;
                            }
                            if (id74 > 0.7 && f3 == 0)
                            {
                                if (whichList < 3)
                                {
                                    whichList += 1;
                                }
                                f3 = 1;
                            }
                            if (id74 < 0.3 && f3 == 0)
                            {
                                if (whichList > 0)
                                {
                                    whichList -= 1;
                                }
                                f3 = 1;
                            }
                            System.Threading.Thread.Sleep(100);
                            break;

                        case 7:
                            if (id22 < 0.2 && f5 == 0)
                            {
                                if (screen == 2 || screen == 3 || screen == 4 || screen == 6 || screen == 7 || screen == 8)
                                {
                                    if (whichSong > 1)
                                    {
                                        whichSong -= 1;
                                    }
                                }
                                f5 = 1;
                            }
                            if (id22 > 0.8 && f5 == 0)
                            {
                                if (screen == 2 || screen == 3 || screen == 4 || screen == 6 || screen == 7 || screen == 8)
                                {
                                    if (whichSong < 3)
                                    {
                                        whichSong += 1;
                                    }
                                }
                                f5 = 1;
                            }
                            if (id22 > 0.4 && id22 < 0.6)
                            {
                                f5 = 0;
                            }
                            ///////////////////////////////
                            if (id74 > 0.7)
                            {
                                if(screen == 2)
                                {
                                    if (whichSong == 1)
                                    {
                                        screen = 9;
                                    }
                                    if (whichSong == 2)
                                    {
                                        screen = 10;
                                    }
                                    if (whichSong == 3)
                                    {
                                        screen = 11;
                                    }                               
                                }
                                if (screen == 3)
                                {
                                    if (whichSong == 1)
                                    {
                                        screen = 12;
                                    }
                                    if (whichSong == 2)
                                    {
                                        screen = 13;
                                    }
                                    if (whichSong == 3)
                                    {
                                        screen = 14;
                                    }
                                }
                                if (screen == 4)
                                {
                                    if (whichSong == 1)
                                    {
                                        screen = 15;
                                    }
                                    if (whichSong == 2)
                                    {
                                        screen = 16;
                                    }
                                    if (whichSong == 3)
                                    {
                                        screen = 17;
                                    }
                                }
                                if (screen == 6)
                                {
                                    if (whichSong == 1)
                                    {
                                        screen = 18;
                                    }
                                    if (whichSong == 2)
                                    {
                                        screen = 19;
                                    }
                                    if (whichSong == 3)
                                    {
                                        screen = 20;
                                    }
                                }
                                if (screen == 7)
                                {
                                    if (whichSong == 1)
                                    {
                                        screen = 21;
                                    }
                                    if (whichSong == 2)
                                    {
                                        screen = 22;
                                    }
                                    if (whichSong == 3)
                                    {
                                        screen = 23;
                                    }
                                }
                                if (screen == 8)
                                {
                                    if (whichSong == 1)
                                    {
                                        screen = 24;
                                    }
                                    if (whichSong == 2)
                                    {
                                        screen = 25;
                                    }
                                    if (whichSong == 3)
                                    {
                                        screen = 26;
                                    }
                                }
                            }


                            System.Threading.Thread.Sleep(100);
                            break;
                        case 10:
                            if (isPlaying == false)
                            {
                                Thread thread1 = new Thread(() => AudioPlay(profileFolder, index));
                                thread1.Start();
                                isPlaying = true;
                                f = 1;
                            }
                            System.Threading.Thread.Sleep(100);
                            break;

                        case 8:
                            Console.WriteLine("TUIO marker detected: " + id74);
                            if (id74 > 0.3 && id74 < 0.7)
                            {
                                f2 = 0;
                            }
                            if (isPlaying == true && id74 > 0.8 && f2 == 0)
                            {
                                if (index < 2)
                                {
                                    StopAudio2();
                                    index++;
                                    Thread thread1 = new Thread(() => AudioPlay(profileFolder, index));
                                    thread1.Start();
                                    isPlaying = true;
                                    f = 1;
                                    f2 = 1;
                                }
                            }
                            if (isPlaying == true && id74 < 0.2 && f2 == 0)
                            {
                                if (index > 0)
                                {
                                    StopAudio2();
                                    index--;
                                    Thread thread1 = new Thread(() => AudioPlay(profileFolder, index));
                                    thread1.Start();
                                    isPlaying = true;
                                    f = 1;
                                    f2 = 1;
                                }
                            }
                            System.Threading.Thread.Sleep(100);
                            break;

                        case 11:
                            if (outputDevice != null && isPlaying)
                            {
                                StopAudio();
                            }
                            break;

                        case 9:
                            if (outputDevice != null && isPlaying == false)
                            {
                                PlayAudio();
                            }
                            break;

                        default:
                            // Use default rectangle for other IDs
                            g.FillRectangle(objBrush, new Rectangle(ox - size / 2, oy - size / 2, size, size));
                            g.DrawString(tobj.SymbolID + "", font, fntBrush, new PointF(ox - 10, oy - 10));


                            continue;
                    }
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
    private void StopAudio2()
    {
        if (outputDevice != null && isPlaying)
        {
            outputDevice.Stop();
            isPlaying = false;
            Console.WriteLine("Audio stopped.");
        }
        else
        {
            Console.WriteLine("No audio is playing.");
        }
    }
    private void StopAudio()
    {
        if (outputDevice != null && isPlaying)
        {
            outputDevice.Pause(); // Stop the audio playback
            isPlaying = false;
            Console.WriteLine("Audio stopped.");
        }
        else
        {
            Console.WriteLine("No audio is playing.");
        }
    }
    private void PlayAudio()
    {
        if (outputDevice != null && isPlaying == false)
        {
            outputDevice.Play(); // Stop the audio playback
            isPlaying = true;
            Console.WriteLine("Audio Played.");
        }
        else
        {
            Console.WriteLine("No audio is playing.");
        }
    }
    private void AudioPlay(string profileFolder, int index_User)
    {
        if (Directory.Exists(profileFolder))
        {
            audioFiles2 = Directory.GetFiles(profileFolder, "*.mp3");

            if (index_User >= 0 && index_User < audioFiles2.Length)
            {

                string audioFilePath = audioFiles2[index_User];
                flag_LastMusic = audioFilePath;
                string audioFileName = Path.GetFileName(audioFilePath);
                Console.WriteLine("Playing audio file: " + audioFileName);

                using (var audioFile = new AudioFileReader(audioFilePath))
                using (outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing || f == 1)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    isPlaying = false;
                    f = 0;
                }
            }
            else
            {

                Console.WriteLine("Invalid index. No audio file found at index: " + index_User);
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
        //Console.WriteLine("Profile Folder: " + profileFolder);

        folderName = Path.GetFileName(profileFolder);
        //Console.WriteLine("Folder Name: " + folderName);


        if (Directory.Exists(profileFolder))
        {

            audioFiles = Directory.GetFiles(profileFolder, "*.mp3");

            int audioFileCount = audioFiles.Length;

            //Console.WriteLine("Number of audio files: " + audioFileCount);


            foreach (string audioFile in audioFiles)
            {
                string audioFileName2 = Path.GetFileName(audioFile);
                //Console.WriteLine("Playing audio file: " + audioFileName2);

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