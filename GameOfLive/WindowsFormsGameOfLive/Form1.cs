using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameModel;
using GuiHelper;

namespace WindowsFormsGameOfLive
{
    public partial class Form1 : Form
    {
        private Bitmap Bmp;
        private IGameOfLifeModel gameModel;
        private readonly Color GameBackground = Color.Black;
        private readonly Color GameForeground = Color.White;

        public Form1()
        {
            InitializeComponent();
            picBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GameModel_Init(int.Parse(tbWidth.Text), int.Parse(tbHeight.Text));
        }

        private void picBox_Click(object sender, EventArgs ev)
        {
            var me = (MouseEventArgs)ev;

            var xBoxToBmpFactor = (double)Bmp.Width / picBox.Width;
            var yBoxToBmpFactor = (double)Bmp.Height / picBox.Height;

            var x = (int)Math.Floor((decimal)(me.X + 1) * Bmp.Width / picBox.Width);
            x += picBox.Width - Bmp.Width;

            var y = (int)Math.Floor((decimal)(me.Y + 1) * Bmp.Height / picBox.Height);
            y += picBox.Height - Bmp.Height;

            var oldClr = this.Bmp.GetPixel(x, y);
            var newClr = (oldClr.ToArgb() != Color.Black.ToArgb()) ? Color.Black : Color.White;
            
            this.Bmp.SetPixel(x, y, newClr);
            (this.picBox as Control).Refresh();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            NextStep();
        }

        private void NextStep()
        {
            var sw = Stopwatch.StartNew();
            gameModel.SingleStep();
            Console.WriteLine($"gol.NextStep(): {sw.ElapsedMilliseconds}ms");
            var newBitmap = MatrixHelper.ConvertToBitmap(gameModel.Board, GameBackground, GameForeground);
            picBox.Image = newBitmap;
            Console.WriteLine($"gol.NextStep() + Display(newBoard): {sw.ElapsedMilliseconds}ms");
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
            else
            {
                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            NextStep();
        }

        private void btInit_Click(object sender, EventArgs e)
        {
            GameModel_Init(int.Parse(tbWidth.Text), int.Parse(tbHeight.Text));
        }

        private void GameModel_Init(int width, int height)
        {
            Bmp = new Bitmap(width, height);
            this.picBox.Image = Bmp;

            gameModel = new GameOfLifeMatrixModel();
            gameModel.Init(width, height);
            this.picBox.Image = MatrixHelper.ConvertToBitmap(gameModel.Board, GameBackground, GameForeground);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                timer1.Interval = trackBar1.Value;
                lbInterval.Text = $"{trackBar1.Value}ms";
            }));

        }
    }
}
