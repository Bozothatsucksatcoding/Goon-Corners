using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace goon_corners;



public partial class Form1 : Form
{

    const int WS_EX_LAYERED = 0x80000;
    const int WS_EX_TRANSPARENT = 0x20;
    const int GWL_EXSTYLE = -20;

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


    string[] imagePaths;
    Image[] currentImages = new Image[4];
    Random rand = new Random();
    System.Windows.Forms.Timer timer;


    public Form1()
    {
        InitializeComponent();

        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;
        this.BackColor = Color.LimeGreen;
        this.TransparencyKey = Color.LimeGreen;
        this.Bounds = Screen.PrimaryScreen.WorkingArea;


        int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
        SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);

        string assets = Path.Combine(Application.StartupPath, "assets");
        if (!Directory.Exists(assets))
        {
            MessageBox.Show("Assets missing. Reinstall required.");
            Environment.Exit(1);
        }

        imagePaths = Directory.GetFiles(assets, "*.*")
            .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (imagePaths.Length < 4)
        {
            MessageBox.Show("Not enough images in assets. Please add at least 4.");
            Environment.Exit(1);
        }



        timer = new System.Windows.Forms.Timer();
        timer.Interval = 10000;
        timer.Tick += (s, e) =>
        {
            for (int i = 0; i < 4; i++)
            {
                currentImages[i]?.Dispose();
                string path = imagePaths[rand.Next(imagePaths.Length)];
                currentImages[i] = Image.FromFile(path);
            }

            Invalidate();
        };
        timer.Start();

    }

    private void LoadNewImages()
    {
        for (int i = 0; i < 4; i++)
        {
            currentImages[i]?.Dispose();
            string path = imagePaths[rand.Next(imagePaths.Length)];
            currentImages[i] = Image.FromFile(path);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (currentImages == null)
            return;


        if (currentImages[0] != null)
            e.Graphics.DrawImage(currentImages[0], 0, 0, currentImages[0].Width, currentImages[0].Height);

        if (currentImages[1] != null)
            e.Graphics.DrawImage(currentImages[1], Width - currentImages[1].Width, 0, currentImages[1].Width, currentImages[1].Height);

        if (currentImages[2] != null)
            e.Graphics.DrawImage(currentImages[2], 0, Height - currentImages[2].Height, currentImages[2].Width, currentImages[2].Height);


        if (currentImages[3] != null)
            e.Graphics.DrawImage(currentImages[3], Width - currentImages[3].Width, Height - currentImages[3].Height, currentImages[3].Width, currentImages[3].Height);


    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {

        foreach (var img in currentImages)
            img?.Dispose();

        base.OnFormClosing(e);

    }

}
