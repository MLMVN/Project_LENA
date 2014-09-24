using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


using BitMiracle.LibTiff.Classic;
using System.IO;

namespace Project_LENA
{
    public partial class Form2 : Form
    {
        string cleanimage;
        string noisyimage;
        private double Percentage;

        ///// <summary>
        ///// Inherits from PictureBox; adds Interpolation Mode Setting
        ///// </summary>
        //public class PictureBoxWithInterpolationMode : PictureBox
        //{
        //    public InterpolationMode InterpolationMode { get; set; }

        //    protected override void OnPaint(PaintEventArgs paintEventArgs)
        //    {
        //        paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        //        base.OnPaint(paintEventArgs);
        //    }
        //}


        public Form2(string clean, string noisy)
        {
            InitializeComponent();           

            cleanimage = clean;
            noisyimage = noisy;            

            // read bytes of an image
            byte[] buffer = File.ReadAllBytes(clean);

            // create a memory streams out of the bytes read
            MemoryStream ms = new MemoryStream(buffer);

            //open a tiff stored in the memory stream!
            pictureBox1.Image = Image.FromStream(ms);

            label1.Text = Path.GetFileName(clean);

            // read bytes of an image
            byte[] buffer2 = File.ReadAllBytes(noisy);

            // create a memory streams out of the bytes read
            MemoryStream ms2 = new MemoryStream(buffer2);

            //open a tiff stored in the memory stream!
            pictureBox2.Image = Image.FromStream(ms2);

            pictureBox1.Width = pictureBox1.Image.Width;
            pictureBox1.Height = pictureBox1.Image.Height;

            pictureBox2.Width = pictureBox2.Image.Width;
            pictureBox2.Height = pictureBox2.Image.Height;

            label2.Text = Path.GetFileName(noisy);

            int formwidth = pictureBox1.Width + pictureBox2.Width + 40;
            int formheight = pictureBox1.Height + 138;

            //if (this.MinimumSize.Width < maxwidth && this.MinimumSize.Height < maxheight)
            this.Size = new Size(formwidth, formheight);
            //else this.MaximumSize = this.MinimumSize;

            panel1.Width = (this.Width) / 2 - 20;
            panel1.Height = this.Height - 12 - 126;
            panel2.Width = (this.Width) / 2 - 20;
            panel2.Height = this.Height - 12 - 126;
            panel2.Left = (panel1.Width + 12);
            label2.Left = panel2.Left;

            comboBox1.Left = 44;
            comboBox1.Top = this.Height - 61;

            string[] a = comboBox1.Text.Split(' ');
            Percentage = Convert.ToDouble(a[0]) / 100;          
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            panel1.Width = (this.Width) / 2 - 20;
            panel1.Height = this.Height - 12 - 126;
            panel2.Width = (this.Width) / 2 - 20;
            panel2.Height = this.Height - 12 - 126;
            panel2.Left = (panel1.Width + 12);
            label2.Left = panel2.Left;
            comboBox1.Left = 44;
            comboBox1.Top = this.Height - 61;
        }

        private Point PanStartPoint;

        private Point RectStartPoint;
        private Rectangle Rect = new Rectangle();
        private Rectangle notRect = new Rectangle();
        private Brush selectionBrush = new SolidBrush(Color.FromArgb(128, 72, 145, 220));
        private bool CreatedRect;
        private bool WidthLeft = false;
        private bool WidthRight = false;
        private bool HeightTop = false;
        private bool HeightBottom = false;
        private int Resized = 0;
        private Point MousePos;
        private double InitialPercentage;


        #region pictureBox1

        // Start Rectangle
        //
        private void pictureBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PanStartPoint = e.Location;
            }
            if (e.Button == MouseButtons.Left)
            {
                // Determine the initial rectangle coordinates...
                RectStartPoint = e.Location;
                CreatedRect = false;
                //Invalidate();
            }

        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }  

        private void pictureBox1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {          
            if (e.Delta != 0)
            {
                Resized += 1;
                MousePos = e.Location;               

                InitialPercentage = Percentage;

                //currentLocation.X += Convert.ToInt32((MousePosition.X - currentLocation.X) / Percentage);
                //currentLocation.Y += Convert.ToInt32((MousePosition.X - currentLocation.Y) / Percentage);               

                if (e.Delta > 0)
                {
                    if (Percentage < 3)
                    {
                        if (Percentage >= .2)
                        {
                            Percentage = Math.Round(Percentage + (Percentage * ((double)e.Delta / 480)), 2); //.25
                        }
                        if (Percentage < .2)
                        {
                            Percentage += Math.Round(Percentage * ((double)e.Delta / 480), 2); //.25
                        }
                        comboBox1.Text = Percentage * 100 + " %";
                    }
                    //Set maximum percentage to zoom
                    if (Percentage >= 3)
                    {
                        Percentage = 3;
                        comboBox1.Text = Percentage * 100 + " %";
                    }
                }
                else if (e.Delta <= 0)
                {
                    if (Percentage > 0.5)
                    {
                        if (Percentage >= .2)
                        {
                            Percentage = Math.Round(Percentage * 4 / ((double)e.Delta / -24), 2); //.80
                        }
                        if (Percentage < .2)
                        {
                            Percentage = Math.Round(Percentage * 4 / ((double)e.Delta / -24), 4); //.80
                        }
                        comboBox1.Text = Percentage * 100 + " %";
                    }
                    //Set minimum percentage to zoom
                    else if (Percentage <= 0.5)
                    {
                        Percentage = 0.5;
                        comboBox1.Text = Percentage * 100 + " %";
                    }                 
                }
            }

            int intPercent = Convert.ToInt32(Percentage);
            Point currentLocation = new Point(MousePos.X, MousePos.Y);

            //Point test = new Point((int)(MousePos.X / (InitialPercentage)),
            //    (int)(MousePos.Y / (InitialPercentage)));      

            double testX = MousePos.X / InitialPercentage;
            double testY = MousePos.Y / InitialPercentage;


            //Point test2 = new Point((int)(testX * (Percentage)),
            //    (int)(testY * (Percentage)));

            double test2X = testX * Percentage;
            double test2Y = testY * Percentage;


            int test3 = Convert.ToInt32(pictureBox1.Image.Width * (Percentage));
            int test4 = Convert.ToInt32(pictureBox1.Image.Height * (Percentage));

            int test5 = Convert.ToInt32((testX) - (test2X));
            int test6 = Convert.ToInt32((testY) - (test2Y));


            pictureBox1.Location = new Point(test5, test6);
            pictureBox1.Size = new Size(test3, test4);

            pictureBox2.Location = new Point(test5, test6);
            pictureBox2.Size = new Size(test3, test4);

            //pictureBox1.Width = test3;
            //pictureBox1.Height = test4;                    

            //Graphics g = e.Graphics;
            //PictureBox picbox = (PictureBox)sender;
            //g.Clear(picbox.BackColor);

            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            //g.DrawImage(pictureBox1.Image, test5, test6, test3, test4);

            double test7X = RectStartPoint.X / InitialPercentage;
            double test7Y = RectStartPoint.Y / InitialPercentage;

            RectStartPoint.X = Convert.ToInt32(test7X * Percentage);
            RectStartPoint.Y = Convert.ToInt32(test7Y * Percentage);

            if (CreatedRect == true)
            {
                double test8X = Rect.X / InitialPercentage;
                double test8Y = Rect.Y / InitialPercentage;

                double test9W = Rect.Width / InitialPercentage;
                double test9H = Rect.Height / InitialPercentage;

                Rect.Width = Convert.ToInt32(test9W * Percentage);
                Rect.Height = Convert.ToInt32(test9H * Percentage);
                Rect.X = Convert.ToInt32(test8X * Percentage);
                Rect.Y = Convert.ToInt32(test8Y * Percentage);

                toolStripStatusLabel2.Text = String.Format("Selection size: {0} by {1} pixels", Convert.ToInt32(test9W), Convert.ToInt32(test9H));

                //pictureBox1.Invalidate();
            }
        }

        // Draw Rectangle
        //
        private void pictureBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = String.Format(label1.Text + "  X: {0}; Y: {1}", Convert.ToInt32(e.X / Percentage), Convert.ToInt32(e.Y / Percentage));
            Control c = (Control)sender;
            pictureBox2.Left = c.Left;
            pictureBox2.Top = c.Top;

            #region Right Click Panning
            if (e.Button == MouseButtons.Right)
            {
                Cursor = Cursors.SizeAll;

                // Panel is wider than image
                if (panel1.Width >= pictureBox1.Width)
                {
                    if (c.Left + (e.X - PanStartPoint.X) >= 0 && c.Left + (e.X - PanStartPoint.X) <= panel1.Width - pictureBox1.Width)
                    {
                        c.Left += (e.X - PanStartPoint.X);
                    }
                    else if (c.Left + (e.X - PanStartPoint.X) < 0)
                    {
                        c.Left = 0;
                    }
                    else if (c.Left + (e.X - PanStartPoint.X) > panel1.Width - pictureBox1.Width)
                    {
                        c.Left = panel1.Width - pictureBox1.Width;
                    }
                }
                // Panel is taller than image
                if (panel1.Height >= pictureBox1.Height)
                {

                    if (c.Top + (e.Y - PanStartPoint.Y) >= 0 && c.Top + (e.Y - PanStartPoint.Y) <= panel1.Height - pictureBox1.Height)
                    {
                        c.Top = (c.Top + e.Y) - PanStartPoint.Y;
                    }
                    else if (c.Top + (e.Y - PanStartPoint.Y) < 0)
                    {
                        c.Top = 0;
                    }
                    else if (c.Top + (e.Y - PanStartPoint.Y) > panel1.Height - pictureBox1.Height)
                    {
                        c.Top = panel1.Height - pictureBox1.Height;
                    }
                }
                // Panel is narrower than image
                if (panel1.Width < pictureBox1.Width)
                {
                    if (c.Left + (e.X - PanStartPoint.X) < 0 && c.Left + (e.X - PanStartPoint.X) > panel1.Width - pictureBox1.Width)
                    {
                        c.Left += (e.X - PanStartPoint.X);
                    }
                    else if (c.Left + (e.X - PanStartPoint.X) > 0)
                    {
                        c.Left = 0;
                    }
                    else if (c.Left + (e.X - PanStartPoint.X) < panel1.Width - pictureBox1.Width)
                    {
                        c.Left = panel1.Width - pictureBox1.Width;
                    }
                }
                // Panel is shorter than image
                if (panel1.Height < pictureBox1.Height)
                {
                    if (c.Top + (e.Y - PanStartPoint.Y) < 0 && c.Top + (e.Y - PanStartPoint.Y) > panel1.Height - pictureBox1.Height)
                    {
                        c.Top += (e.Y - PanStartPoint.Y);
                    }
                    else if (c.Top + (e.Y - PanStartPoint.Y) > 0)
                    {
                        c.Top = 0;
                    }
                    else if (c.Top + (e.Y - PanStartPoint.Y) < panel1.Height - pictureBox1.Height)
                    {
                        c.Top = panel1.Height - pictureBox1.Height;
                    }
                }
                c.BringToFront();

            }
            #endregion

            #region Left Click Rectangle
            if (e.Button == MouseButtons.Left)
            {
                Cursor = Cursors.Cross;
                Point tempEndPoint = e.Location;

                Rect.Location = new Point(
                    Math.Min(RectStartPoint.X, Math.Max(tempEndPoint.X, 0)),
                    Math.Min(RectStartPoint.Y, Math.Max(tempEndPoint.Y, 0)));


                if (tempEndPoint.X <= pictureBox1.Width && tempEndPoint.X >= 0)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - tempEndPoint.X);
                }
                else if (tempEndPoint.X > pictureBox1.Width)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - pictureBox1.Width);
                }
                else if (tempEndPoint.X < 0)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - 0);
                }

                if (tempEndPoint.Y <= pictureBox1.Height && tempEndPoint.Y >= 0)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - tempEndPoint.Y);
                }
                else if (tempEndPoint.Y > pictureBox1.Height)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - pictureBox1.Height);
                }
                else if (tempEndPoint.Y < 0)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - 0);
                }

                pictureBox1.Invalidate();
                pictureBox2.Invalidate();
                toolStripStatusLabel2.Text = String.Format("Selection size: {0} by {1} pixels", Convert.ToInt32(Rect.Width / Percentage), Convert.ToInt32(Rect.Height / Percentage));

                if (panel1.Width < pictureBox1.Width)
                {
                    if (e.X > Math.Abs(c.Left))
                    {
                        WidthLeft = false;
                        //timer1.Enabled = false;                       
                    }
                    if (e.X < panel1.Width + Math.Abs(c.Left))
                    {
                        WidthRight = false;
                        //timer1.Enabled = false;
                    }

                    if (e.X < Math.Abs(c.Left) && c.Left <= 0)
                    {
                        WidthLeft = true;
                        timer1.Enabled = true;
                        timer2.Enabled = true;
                    }
                    else if (c.Left > 0)
                    {
                        WidthLeft = false;
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        c.Left = 0;
                    }

                    if (e.X > panel1.Width + Math.Abs(c.Left) && c.Left >= panel1.Width - pictureBox1.Width)
                    {
                        WidthRight = true;
                        timer1.Enabled = true;
                        timer2.Enabled = true;
                    }
                    else if (c.Left < panel1.Width - pictureBox1.Width)
                    {
                        WidthRight = false;
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        c.Left = panel1.Width - pictureBox1.Width;
                    }
                }

                if (panel1.Height < pictureBox1.Height)
                {
                    if (e.Y > Math.Abs(c.Top))
                    {
                        HeightTop = false;
                        //timer1.Enabled = false;                       
                    }
                    if (e.Y < panel1.Height + Math.Abs(c.Top))
                    {
                        HeightBottom = false;
                        //timer1.Enabled = false;
                    }

                    if (e.Y < Math.Abs(c.Top) && c.Top <= 0)
                    {
                        HeightTop = true;
                        timer1.Enabled = true;
                        timer2.Enabled = true;
                    }
                    else if (c.Top > 0)
                    {
                        HeightTop = false;
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        c.Top = 0;
                    }

                    if (e.Y > panel1.Height + Math.Abs(c.Top) && c.Top >= panel1.Height - pictureBox1.Height)
                    {
                        HeightBottom = true;
                        timer1.Enabled = true;
                        timer2.Enabled = true;
                    }
                    else if (c.Top < panel1.Height - pictureBox1.Height)
                    {
                        HeightBottom = false;
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        c.Top = panel1.Height - pictureBox1.Height;
                    }
                }
                c.BringToFront();
            }
            #endregion
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();

            if (e.Button == MouseButtons.Right)
            {
                
                //if (Rect.Contains(e.Location))
                //{
                Cursor = Cursors.Default;
                //}
            }
            if (e.Button == MouseButtons.Left)
            {
                Control c = (Control)sender;
                pictureBox1.Left = c.Left;
                pictureBox1.Top = c.Top;

                timer1.Enabled = false;
                timer2.Enabled = false;

                if (panel1.Width < pictureBox1.Width)
                {
                    if (c.Left >= 0)
                    {
                        c.Left = 0;
                    }
                    else if (c.Left <= panel1.Width - pictureBox1.Width)
                    {
                        c.Left = panel1.Width - pictureBox1.Width;
                    }

                    if (c.Top >= 0)
                    {
                        c.Top = 0;
                    }
                    else if (c.Top <= panel1.Height - pictureBox1.Height)
                    {
                        c.Top = panel1.Height - pictureBox1.Height;
                    }
                }

                Point tempEndPoint = e.Location;
                if (RectStartPoint.X == tempEndPoint.X)
                {
                    Rect.Location = new Point(
                    Math.Min(RectStartPoint.X, tempEndPoint.X),
                    Math.Min(RectStartPoint.Y, tempEndPoint.Y));
                    Rect.Size = new Size(
                    Math.Abs(RectStartPoint.X - tempEndPoint.X),
                    Math.Abs(RectStartPoint.Y - tempEndPoint.Y));
                    //Rect.Size = new Size(
                    //    Math.Min(Math.Abs(RectStartPoint.X - tempEndPoint.X), Math.Abs(RectStartPoint.X - pictureBox1.ClientRectangle.Width)),
                    //    Math.Min(Math.Abs(RectStartPoint.Y - tempEndPoint.Y), Math.Abs(RectStartPoint.Y - pictureBox1.ClientRectangle.Height)));

                    toolStripStatusLabel2.Text = String.Format("  ");
                    Invalidate();
                }

                CreatedRect = true;
                notRect.Location = new Point(0, 0);
                notRect.Size = pictureBox1.Size;
                Invalidate();
                //if (!Rect.Contains(e.Location))
                //{
                //    CreatedRect = true;
                //    notRect.Location = new Point(0, 0);
                //    notRect.Size = pictureBox1.Size;
                //    Invalidate();
                //    // I can add custom select code here!

                //}
                Cursor = Cursors.Default;
            }
        }

        // Draw Area
        //
        private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Draw the rectangle...
            if (pictureBox1.Image != null)
            {
                if (Rect != null && Rect.Width > 0 && Rect.Height > 0)
                {
                    if (CreatedRect == false)
                    {
                        e.Graphics.FillRectangle(selectionBrush, Rect);

                        using (Pen p = new Pen(Color.FromArgb(255, 51, 153, 255), Convert.ToSingle(Percentage)))
                        {
                            e.Graphics.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width - (int)(Percentage), Rect.Height - (int)(Percentage)); //(int)(Percentage) done to bug with indexation
                            if (Rect.Width == 1)
                            {
                                e.Graphics.DrawLine(p, Rect.X, Rect.Y, Rect.X, Rect.Y + Rect.Height - (int)(Percentage));
                            }
                            if (Rect.Height == 1)
                            {
                                e.Graphics.DrawLine(p, Rect.X, Rect.Y, Rect.X + Rect.Width - (int)(Percentage), Rect.Y);
                            }
                        }
                    }


                    if (CreatedRect == true)
                    {
                        using (Brush notselectedBrush = new SolidBrush(Color.FromArgb(128, 40, 40, 40)))
                        {
                            Rectangle InverseRect = new Rectangle(0, 0, Rect.X, pictureBox1.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.X, 0, pictureBox1.Width, Rect.Y);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.X + Rect.Width, Rect.Y, pictureBox1.Width, pictureBox1.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.X, Rect.Y + Rect.Height, Rect.Width, pictureBox1.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                        }
                        using (Pen p = new Pen(Color.FromArgb(255, 255, 255, 255), Convert.ToSingle(Percentage)))
                        {
                            e.Graphics.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width - (int)(Percentage), Rect.Height - (int)(Percentage));
                            if (Rect.Width == 1)
                            {
                                e.Graphics.DrawLine(p, Rect.X, Rect.Y, Rect.X, Rect.Y + Rect.Height - (int)(Percentage));
                            }
                            if (Rect.Height == 1)
                            {
                                e.Graphics.DrawLine(p, Rect.X, Rect.Y, Rect.X + Rect.Width - (int)(Percentage), Rect.Y);
                            }
                        }
                        //using (Brush selectedBrush = new SolidBrush(Color.FromArgb(0, 72, 145, 220)))
                        //{
                        //    e.Graphics.FillRectangle(selectedBrush, Rect);
                        //}                      
                    }
                }         
            }
        }       
        #endregion

        #region pictureBox2

        // Start Rectangle
        //
        private void pictureBox2_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PanStartPoint = e.Location;
            }
            if (e.Button == MouseButtons.Left)
            {
                // Determine the initial rectangle coordinates...
                RectStartPoint = e.Location;
                CreatedRect = false;
                //Invalidate();
            }

        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Focus();
        }

        private void pictureBox2_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                Resized += 1;
                MousePos = e.Location;

                InitialPercentage = Percentage;

                //currentLocation.X += Convert.ToInt32((MousePosition.X - currentLocation.X) / Percentage);
                //currentLocation.Y += Convert.ToInt32((MousePosition.X - currentLocation.Y) / Percentage);               

                if (e.Delta > 0)
                {
                    if (Percentage < 3)
                    {
                        if (Percentage >= .2)
                        {
                            Percentage = Math.Round(Percentage + (Percentage * ((double)e.Delta / 480)), 2); //.25
                        }
                        if (Percentage < .2)
                        {
                            Percentage += Math.Round(Percentage * ((double)e.Delta / 480), 2); //.25
                        }
                        comboBox1.Text = Percentage * 100 + " %";
                    }
                    //Set maximum percentage to zoom
                    if (Percentage >= 3)
                    {
                        Percentage = 3;
                        comboBox1.Text = Percentage * 100 + " %";
                    }
                }
                else if (e.Delta <= 0)
                {
                    if (Percentage > 0.5)
                    {
                        if (Percentage >= .2)
                        {
                            Percentage = Math.Round(Percentage * 4 / ((double)e.Delta / -24), 2); //.80
                        }
                        if (Percentage < .2)
                        {
                            Percentage = Math.Round(Percentage * 4 / ((double)e.Delta / -24), 4); //.80
                        }
                        comboBox1.Text = Percentage * 100 + " %";
                    }
                    //Set minimum percentage to zoom
                    else if (Percentage <= 0.5)
                    {
                        Percentage = 0.5;
                        comboBox1.Text = Percentage * 100 + " %";
                    }
                }
            }

            int intPercent = Convert.ToInt32(Percentage);
            Point currentLocation = new Point(MousePos.X, MousePos.Y);

            //Point test = new Point((int)(MousePos.X / (InitialPercentage)),
            //    (int)(MousePos.Y / (InitialPercentage)));      

            double testX = MousePos.X / InitialPercentage;
            double testY = MousePos.Y / InitialPercentage;


            //Point test2 = new Point((int)(testX * (Percentage)),
            //    (int)(testY * (Percentage)));

            double test2X = testX * Percentage;
            double test2Y = testY * Percentage;


            int test3 = Convert.ToInt32(pictureBox1.Image.Width * (Percentage));
            int test4 = Convert.ToInt32(pictureBox1.Image.Height * (Percentage));

            int test5 = Convert.ToInt32((testX) - (test2X));
            int test6 = Convert.ToInt32((testY) - (test2Y));


            pictureBox1.Location = new Point(test5, test6);
            pictureBox1.Size = new Size(test3, test4);

            pictureBox2.Location = new Point(test5, test6);
            pictureBox2.Size = new Size(test3, test4);

            //pictureBox1.Width = test3;
            //pictureBox1.Height = test4;                    

            //Graphics g = e.Graphics;
            //PictureBox picbox = (PictureBox)sender;
            //g.Clear(picbox.BackColor);

            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            //g.DrawImage(pictureBox1.Image, test5, test6, test3, test4);

            double test7X = RectStartPoint.X / InitialPercentage;
            double test7Y = RectStartPoint.Y / InitialPercentage;

            RectStartPoint.X = Convert.ToInt32(test7X * Percentage);
            RectStartPoint.Y = Convert.ToInt32(test7Y * Percentage);

            if (CreatedRect == true)
            {
                double test8X = Rect.X / InitialPercentage;
                double test8Y = Rect.Y / InitialPercentage;

                double test9W = Rect.Width / InitialPercentage;
                double test9H = Rect.Height / InitialPercentage;

                Rect.Width = Convert.ToInt32(test9W * Percentage);
                Rect.Height = Convert.ToInt32(test9H * Percentage);
                Rect.X = Convert.ToInt32(test8X * Percentage);
                Rect.Y = Convert.ToInt32(test8Y * Percentage);

                toolStripStatusLabel2.Text = String.Format("Selection size: {0} by {1} pixels", Convert.ToInt32(test9W), Convert.ToInt32(test9H));

                //pictureBox2.Invalidate();
            }
        }

        // Draw Rectangle
        //
        private void pictureBox2_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = String.Format(label1.Text + "  X: {0}; Y: {1}", (int)(e.X / Percentage), (int)(e.Y / Percentage));
            Control c = (Control)sender;
            pictureBox1.Left = c.Left;
            pictureBox1.Top = c.Top;

            #region Right Click Panning
            if (e.Button == MouseButtons.Right)
            {
                Cursor = Cursors.SizeAll;

                // Panel is wider than image
                if (panel2.Width >= pictureBox2.Width)
                {
                    if (c.Left + (e.X - PanStartPoint.X) >= 0 && c.Left + (e.X - PanStartPoint.X) <= panel2.Width - pictureBox2.Width)
                    {
                        c.Left += (e.X - PanStartPoint.X);
                    }
                    else if (c.Left + (e.X - PanStartPoint.X) < 0)
                    {
                        c.Left = 0;
                    }
                    else if (c.Left + (e.X - PanStartPoint.X) > panel2.Width - pictureBox2.Width)
                    {
                        c.Left = panel2.Width - pictureBox2.Width;
                    }
                }
                // Panel is taller than image
                if (panel2.Height >= pictureBox2.Height)
                {

                    if (c.Top + (e.Y - PanStartPoint.Y) >= 0 && c.Top + (e.Y - PanStartPoint.Y) <= panel2.Height - pictureBox2.Height)
                    {
                        c.Top = (c.Top + e.Y) - PanStartPoint.Y;
                    }
                    else if (c.Top + (e.Y - PanStartPoint.Y) < 0)
                    {
                        c.Top = 0;
                    }
                    else if (c.Top + (e.Y - PanStartPoint.Y) > panel2.Height - pictureBox2.Height)
                    {
                        c.Top = panel2.Height - pictureBox2.Height;
                    }
                }
                // Panel is narrower than image
                if (panel2.Width < pictureBox2.Width)
                {
                    if (c.Left + (e.X - PanStartPoint.X) < 0 && c.Left + (e.X - PanStartPoint.X) > panel2.Width - pictureBox2.Width)
                    {
                        c.Left += (e.X - PanStartPoint.X);
                    }
                    else if (c.Left + (e.X - PanStartPoint.X) > 0)
                    {
                        c.Left = 0;
                    }
                    else if (c.Left + (e.X - PanStartPoint.X) < panel2.Width - pictureBox2.Width)
                    {
                        c.Left = panel2.Width - pictureBox2.Width;
                    }
                }
                // Panel is shorter than image
                if (panel2.Height < pictureBox2.Height)
                {
                    if (c.Top + (e.Y - PanStartPoint.Y) < 0 && c.Top + (e.Y - PanStartPoint.Y) > panel2.Height - pictureBox2.Height)
                    {
                        c.Top += (e.Y - PanStartPoint.Y);
                    }
                    else if (c.Top + (e.Y - PanStartPoint.Y) > 0)
                    {
                        c.Top = 0;
                    }
                    else if (c.Top + (e.Y - PanStartPoint.Y) < panel2.Height - pictureBox2.Height)
                    {
                        c.Top = panel2.Height - pictureBox2.Height;
                    }
                }
                c.BringToFront();

            }
            #endregion

            #region Left Click Rectangle
            if (e.Button == MouseButtons.Left)
            {
                Cursor = Cursors.Cross;
                Point tempEndPoint = e.Location;

                Rect.Location = new Point(
                    Math.Min(RectStartPoint.X, Math.Max(tempEndPoint.X, 0)),
                    Math.Min(RectStartPoint.Y, Math.Max(tempEndPoint.Y, 0)));


                if (tempEndPoint.X <= pictureBox2.Width && tempEndPoint.X >= 0)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - tempEndPoint.X);
                }
                else if (tempEndPoint.X > pictureBox2.Width)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - pictureBox2.Width);
                }
                else if (tempEndPoint.X < 0)
                {
                    Rect.Width = Math.Abs(RectStartPoint.X - 0);
                }

                if (tempEndPoint.Y <= pictureBox2.Height && tempEndPoint.Y >= 0)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - tempEndPoint.Y);
                }
                else if (tempEndPoint.Y > pictureBox2.Height)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - pictureBox2.Height);
                }
                else if (tempEndPoint.Y < 0)
                {
                    Rect.Height = Math.Abs(RectStartPoint.Y - 0);
                }

                pictureBox1.Invalidate();
                pictureBox2.Invalidate();
                toolStripStatusLabel2.Text = String.Format("Selection size: {0} by {1} pixels", (int)(Rect.Width / Percentage), (int)(Rect.Height / Percentage));

                if (panel2.Width < pictureBox2.Width)
                {
                    if (e.X > Math.Abs(c.Left))
                    {
                        WidthLeft = false;
                        //timer1.Enabled = false;                       
                    }
                    if (e.X < panel2.Width + Math.Abs(c.Left))
                    {
                        WidthRight = false;
                        //timer1.Enabled = false;
                    }

                    if (e.X < Math.Abs(c.Left) && c.Left <= 0)
                    {
                        WidthLeft = true;
                        timer1.Enabled = true;
                        timer2.Enabled = true;
                    }
                    else if (c.Left > 0)
                    {
                        WidthLeft = false;
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        c.Left = 0;
                    }

                    if (e.X > panel2.Width + Math.Abs(c.Left) && c.Left >= panel2.Width - pictureBox2.Width)
                    {
                        WidthRight = true;
                        timer1.Enabled = true;
                        timer2.Enabled = true;
                    }
                    else if (c.Left < panel2.Width - pictureBox2.Width)
                    {
                        WidthRight = false;
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        c.Left = panel2.Width - pictureBox2.Width;
                    }
                }

                if (panel2.Height < pictureBox2.Height)
                {
                    if (e.Y > Math.Abs(c.Top))
                    {
                        HeightTop = false;
                        //timer1.Enabled = false;                       
                    }
                    if (e.Y < panel2.Height + Math.Abs(c.Top))
                    {
                        HeightBottom = false;
                        //timer1.Enabled = false;
                    }

                    if (e.Y < Math.Abs(c.Top) && c.Top <= 0)
                    {
                        HeightTop = true;
                        timer1.Enabled = true;
                        timer2.Enabled = true;
                    }
                    else if (c.Top > 0)
                    {
                        HeightTop = false;
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        c.Top = 0;
                    }

                    if (e.Y > panel2.Height + Math.Abs(c.Top) && c.Top >= panel2.Height - pictureBox2.Height)
                    {
                        HeightBottom = true;
                        timer1.Enabled = true;
                        timer2.Enabled = true;
                    }
                    else if (c.Top < panel2.Height - pictureBox2.Height)
                    {
                        HeightBottom = false;
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        c.Top = panel2.Height - pictureBox2.Height;
                    }
                }
                c.BringToFront();
            }
            #endregion
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox2.Focus();

            if (e.Button == MouseButtons.Right)
            {

                //if (Rect.Contains(e.Location))
                //{
                Cursor = Cursors.Default;
                //}
            }
            if (e.Button == MouseButtons.Left)
            {
                Control c = (Control)sender;
                pictureBox2.Left = c.Left;
                pictureBox2.Top = c.Top;

                timer1.Enabled = false;
                timer2.Enabled = false;

                if (panel2.Width < pictureBox2.Width)
                {
                    if (c.Left >= 0)
                    {
                        c.Left = 0;
                    }
                    else if (c.Left <= panel2.Width - pictureBox2.Width)
                    {
                        c.Left = panel2.Width - pictureBox2.Width;
                    }

                    if (c.Top >= 0)
                    {
                        c.Top = 0;
                    }
                    else if (c.Top <= panel2.Height - pictureBox2.Height)
                    {
                        c.Top = panel2.Height - pictureBox2.Height;
                    }
                }

                Point tempEndPoint = e.Location;
                if (RectStartPoint.X == tempEndPoint.X)
                {
                    Rect.Location = new Point(
                    Math.Min(RectStartPoint.X, tempEndPoint.X),
                    Math.Min(RectStartPoint.Y, tempEndPoint.Y));
                    Rect.Size = new Size(
                    Math.Abs(RectStartPoint.X - tempEndPoint.X),
                    Math.Abs(RectStartPoint.Y - tempEndPoint.Y));
                    //Rect.Size = new Size(
                    //    Math.Min(Math.Abs(RectStartPoint.X - tempEndPoint.X), Math.Abs(RectStartPoint.X - pictureBox2.ClientRectangle.Width)),
                    //    Math.Min(Math.Abs(RectStartPoint.Y - tempEndPoint.Y), Math.Abs(RectStartPoint.Y - pictureBox2.ClientRectangle.Height)));

                    toolStripStatusLabel2.Text = String.Format("  ");
                    Invalidate();
                }

                CreatedRect = true;
                notRect.Location = new Point(0, 0);
                notRect.Size = pictureBox2.Size;
                Invalidate();
                //if (!Rect.Contains(e.Location))
                //{
                //    CreatedRect = true;
                //    notRect.Location = new Point(0, 0);
                //    notRect.Size = pictureBox2.Size;
                //    Invalidate();
                //    // I can add custom select code here!

                //}
                Cursor = Cursors.Default;
            }
        }

        // Draw Area
        //
        private void pictureBox2_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            // Draw the rectangle...
            if (pictureBox2.Image != null)
            {
                if (Rect != null && Rect.Width > 0 && Rect.Height > 0)
                {
                    if (CreatedRect == false)
                    {
                        e.Graphics.FillRectangle(selectionBrush, Rect);

                        using (Pen p = new Pen(Color.FromArgb(255, 51, 153, 255), Convert.ToSingle(Percentage)))
                        {
                            e.Graphics.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width - (int)(Percentage), Rect.Height - (int)(Percentage));
                            if (Rect.Width == 1)
                            {
                                e.Graphics.DrawLine(p, Rect.X, Rect.Y, Rect.X, Rect.Y + Rect.Height - (int)(Percentage));
                            }
                            if (Rect.Height == 1)
                            {
                                e.Graphics.DrawLine(p, Rect.X, Rect.Y, Rect.X + Rect.Width - (int)(Percentage), Rect.Y);
                            }
                        }
                    }


                    if (CreatedRect == true)
                    {
                        using (Brush notselectedBrush = new SolidBrush(Color.FromArgb(128, 40, 40, 40)))
                        {
                            //e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            //Rectangle InverseRect = new Rectangle(0, 0, Rect.Location.X, pictureBox2.Height);
                            //e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            //InverseRect = new Rectangle(Rect.Location.X, 0, pictureBox2.Width, Rect.Location.Y);
                            //e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            //InverseRect = new Rectangle(Rect.Location.X + Rect.Width, Rect.Location.Y, pictureBox2.Width, pictureBox2.Height);
                            //e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            //InverseRect = new Rectangle(Rect.Location.X, Rect.Location.Y + Rect.Height, Rect.Width, pictureBox2.Height);
                            //e.Graphics.FillRectangle(notselectedBrush, InverseRect);

                            Rectangle InverseRect = new Rectangle(0, 0, Rect.X, pictureBox2.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.X, 0, pictureBox2.Width, Rect.Y);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.X + Rect.Width, Rect.Y, pictureBox2.Width, pictureBox2.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                            InverseRect = new Rectangle(Rect.X, Rect.Y + Rect.Height, Rect.Width, pictureBox2.Height);
                            e.Graphics.FillRectangle(notselectedBrush, InverseRect);
                        }
                        using (Pen p = new Pen(Color.FromArgb(255, 255, 255, 255), Convert.ToSingle(Percentage)))
                        {
                            e.Graphics.DrawRectangle(p, Rect.X, Rect.Y, Rect.Width - (int)(Percentage), Rect.Height - (int)(Percentage));
                            if (Rect.Width == 1)
                            {
                                e.Graphics.DrawLine(p, Rect.X, Rect.Y, Rect.X, Rect.Y + Rect.Height - (int)(Percentage));
                            }
                            if (Rect.Height == 1)
                            {
                                e.Graphics.DrawLine(p, Rect.X, Rect.Y, Rect.X + Rect.Width - (int)(Percentage), Rect.Y);
                            }
                        }
                        //using (Brush selectedBrush = new SolidBrush(Color.FromArgb(0, 72, 145, 220)))
                        //{
                        //    e.Graphics.FillRectangle(selectedBrush, Rect);
                        //}                      
                    }
                }
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            int zoomedWidth = Convert.ToInt32(Rect.Size.Width / Percentage);
            int zoomedHeight = Convert.ToInt32(Rect.Size.Height / Percentage);
            int zoomedX = Convert.ToInt32(Rect.Location.X / Percentage);
            int zoomedY = Convert.ToInt32(Rect.Location.Y / Percentage);
            try
            {
                #region Clean image
                Tiff Cleanimage = Tiff.Open(cleanimage, "r");

                // Obtain basic tag information of the image
                #region GetTagInfo
                int cleanwidth = Cleanimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int cleanheight = Cleanimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                byte cleanbits = Cleanimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                byte cleanpixel = Cleanimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
                #endregion

                if (cleanpixel == 1)
                {
                    byte[,] clean = new byte[cleanheight, cleanwidth];

                    // store the image information in 2d byte array
                    // reserve memory for storing the size of 1 line
                    byte[] scanline = new byte[Cleanimage.ScanlineSize()];
                    // reserve memory for the size of image
                    //byte[,] im = new byte[heightc, widthc];
                    for (int i = 0; i < cleanheight; i++)
                    {
                        Cleanimage.ReadScanline(scanline, i);
                        {
                            for (int j = 0; j < cleanwidth; j++)
                            {
                                clean[i, j] = scanline[j];
                            }
                        }
                    } // end grabbing intensity values 

                    saveFileDialog1.Title = "Save Clean Fragment As";
                    saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(cleanimage) + "_Fragment" + ".tif";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (Tiff output = Tiff.Open(saveFileDialog1.FileName, "w"))
                        {
                            output.SetField(TiffTag.IMAGEWIDTH, zoomedWidth);
                            output.SetField(TiffTag.IMAGELENGTH, zoomedHeight);
                            output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                            output.SetField(TiffTag.BITSPERSAMPLE, 8);
                            output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                            output.SetField(TiffTag.ROWSPERSTRIP, zoomedHeight);
                            output.SetField(TiffTag.XRESOLUTION, 88.0);
                            output.SetField(TiffTag.YRESOLUTION, 88.0);
                            output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                            output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                            output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                            output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                            output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                            byte[] im = new byte[zoomedWidth * sizeof(byte /*can be changed depending on the format of the image*/)];

                            for (int i = 0; i < zoomedHeight; ++i) // change i to initial, change heightc to final
                            {
                                for (int j = 0; j < zoomedWidth; ++j)
                                {
                                    im[j] = clean[zoomedY + i, zoomedX + j];
                                }
                                output.WriteScanline(im, i);
                            }
                            output.WriteDirectory();
                            output.Dispose();
                        }
                    }
                }
                if (cleanpixel == 3)
                {
                    byte[] scanline = new byte[Cleanimage.ScanlineSize()];

                    // Read the image into the memory buffer
                    byte[,] redc = new byte[cleanheight, cleanwidth];
                    byte[,] greenc = new byte[cleanheight, cleanwidth];
                    byte[,] bluec = new byte[cleanheight, cleanwidth];

                    //for (int i = height - 1; i != -1; i--)
                    for (int i = 0; cleanheight > i; i++)
                    {
                        Cleanimage.ReadScanline(scanline, i); // EVIL BUG HERE
                        for (int j = 0; j < cleanwidth; j++)
                        {
                            redc[i, j] = scanline[3 * j]; // PSNR: INFINITY, Channel is correct
                            greenc[i, j] = scanline[3 * j + 1]; // PSNR: INFINITY, Channel is correct
                            bluec[i, j] = scanline[3 * j + 2]; // PSNR: INFINITY, Channel is correct
                        }
                    }
                }
                #endregion

                #region Noisy image
                Tiff Noisyimage = Tiff.Open(noisyimage, "r");

                // Obtain basic tag information of the image
                #region GetTagInfo
                int noisywidth = Noisyimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int noisyheight = Noisyimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                byte noisybits = Noisyimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                byte noisypixel = Noisyimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
                #endregion

                if (noisypixel == 1)
                {
                    byte[,] noisy = new byte[noisyheight, noisywidth];

                    // store the image information in 2d byte array
                    // reserve memory for storing the size of 1 line
                    byte[] scanline = new byte[Noisyimage.ScanlineSize()];
                    // reserve memory for the size of image
                    //byte[,] im = new byte[heightc, widthc];
                    for (int i = 0; i < noisyheight; i++)
                    {
                        Noisyimage.ReadScanline(scanline, i);
                        {
                            for (int j = 0; j < noisywidth; j++)
                            {
                                noisy[i, j] = scanline[j];
                            }
                        }
                    } // end grabbing intensity values 

                    saveFileDialog1.Title = "Save Noisy Fragment As";
                    saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(noisyimage) + "_Fragment" + ".tif";

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (Tiff output = Tiff.Open(saveFileDialog1.FileName, "w"))
                        {
                            output.SetField(TiffTag.IMAGEWIDTH, zoomedWidth);
                            output.SetField(TiffTag.IMAGELENGTH, zoomedHeight);
                            output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                            output.SetField(TiffTag.BITSPERSAMPLE, 8);
                            output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                            output.SetField(TiffTag.ROWSPERSTRIP, zoomedHeight);
                            output.SetField(TiffTag.XRESOLUTION, 88.0);
                            output.SetField(TiffTag.YRESOLUTION, 88.0);
                            output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                            output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                            output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                            output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                            output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                            byte[] im = new byte[zoomedWidth * sizeof(byte /*can be changed depending on the format of the image*/)];

                            for (int i = 0; i < zoomedHeight; ++i) // change i to initial, change heightc to final
                            {
                                for (int j = 0; j < zoomedWidth; ++j)
                                {
                                    im[j] = noisy[zoomedY + i, zoomedX + j];
                                }
                                output.WriteScanline(im, i);
                            }
                            output.WriteDirectory();
                            output.Dispose();
                        }
                    }
                }
                #endregion

            }
            catch (InvalidOperationException)
            {
                Console.Write("Images must be the same resolution and color depth in order to crop images.");
            }
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            if (panel1.Width <= pictureBox1.Left + pictureBox1.Width && pictureBox1.Left > 0)
            {
                pictureBox1.Left = panel1.Width - pictureBox1.Width;
                if (pictureBox1.Left < 0)
                {
                    pictureBox1.Left = 0;
                }
            }

            if (pictureBox1.Top + pictureBox1.Height > panel1.Height && pictureBox1.Top > 0)
            {
                pictureBox1.Top = panel1.Height - pictureBox1.Height;
                if (pictureBox1.Top < 0)
                {
                    pictureBox1.Top = 0;
                }
            }


            if (pictureBox1.Left < 0 && panel1.Width > pictureBox1.Left + pictureBox1.Width)
            {
                pictureBox1.Left = panel1.Width - pictureBox1.Width;
                if (pictureBox1.Left > 0)
                {
                    pictureBox1.Left = 0;
                }
            }
            if (pictureBox1.Top < 0 && panel1.Height > pictureBox1.Top + pictureBox1.Height)
            {
                pictureBox1.Top = panel1.Height - pictureBox1.Height;
                if (pictureBox1.Top > 0)
                {
                    pictureBox1.Top = 0;
                }
            }
        }

        private void panel2_SizeChanged(object sender, EventArgs e)
        {
            if (panel2.Width <= pictureBox2.Left + pictureBox2.Width && pictureBox2.Left > 0)
            {
                pictureBox2.Left = panel2.Width - pictureBox2.Width;
                if (pictureBox2.Left < 0)
                {
                    pictureBox2.Left = 0;
                }
            }

            if (pictureBox2.Top + pictureBox2.Height > panel2.Height && pictureBox2.Top > 0)
            {
                pictureBox2.Top = panel2.Height - pictureBox2.Height;
                if (pictureBox2.Top < 0)
                {
                    pictureBox2.Top = 0;
                }
            }


            if (pictureBox2.Left < 0 && panel2.Width > pictureBox2.Left + pictureBox2.Width)
            {
                pictureBox2.Left = panel2.Width - pictureBox2.Width;
                if (pictureBox2.Left > 0)
                {
                    pictureBox2.Left = 0;
                }
            }
            if (pictureBox2.Top < 0 && panel2.Height > pictureBox2.Top + pictureBox2.Height)
            {
                pictureBox2.Top = panel2.Height - pictureBox2.Height;
                if (pictureBox2.Top > 0)
                {
                    pictureBox2.Top = 0;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (WidthLeft == true) pictureBox1.Left += 10;
            else if (WidthRight == true) pictureBox1.Left -= 10;

            if (HeightTop == true) pictureBox1.Top += 10;
            else if (HeightBottom == true) pictureBox1.Top -= 10;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (WidthLeft == true) pictureBox2.Left += 10;
            else if (WidthRight == true) pictureBox2.Left -= 10;

            if (HeightTop == true) pictureBox2.Top += 10;
            else if (HeightBottom == true) pictureBox2.Top -= 10;
        }

        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // By using Sender, one method could handle multiple ComboBoxes
            ComboBox cbx = sender as ComboBox;
            if (cbx != null)
            {
                // Always draw the background
                e.DrawBackground();

                // Drawing one of the items?
                if (e.Index >= 0)
                {
                    // Set the string alignment.  Choices are Center, Near and Far
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Far;
                    sf.Alignment = StringAlignment.Far;

                    // Set the Brush to ComboBox ForeColor to maintain any ComboBox color settings
                    // Assumes Brush is solid
                    Brush brush = new SolidBrush(cbx.ForeColor);

                    // If drawing highlighted selection, change brush
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                        brush = SystemBrushes.HighlightText;

                    // Draw the string
                    e.Graphics.DrawString(cbx.Items[e.Index].ToString(), cbx.Font, brush, e.Bounds, sf);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] a = comboBox1.Text.Split(' ');
            Percentage = Convert.ToDouble(a[0]) / 100;


            int test3 = Convert.ToInt32(pictureBox1.Image.Width * (Percentage));
            int test4 = Convert.ToInt32(pictureBox1.Image.Height * (Percentage));


            pictureBox1.Location = new Point(0,0);
            pictureBox1.Size = new Size(test3, test4);

            pictureBox2.Location = new Point(0,0);
            pictureBox2.Size = new Size(test3, test4);


            Rect.Width = (int)(Rect.Width * Percentage);
            Rect.Height = (int)(Rect.Height * Percentage);
            Rect.X = (int)(Rect.X * Percentage);
            Rect.Y = (int)(Rect.Y * Percentage);
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar)
        && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.'
                && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }           
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                string[] a = comboBox1.Text.Split(' ', '%');
                int value;

                if (int.TryParse(a[0], out value))
                {
                    if (value >= 50 && value <= 300) Percentage = Convert.ToDouble(value) / 100;
                    else if (value < 50) Percentage = 0.5;
                    else if (value > 300) Percentage = 3;

                    int test3 = Convert.ToInt32(pictureBox1.Image.Width * (Percentage));
                    int test4 = Convert.ToInt32(pictureBox1.Image.Height * (Percentage));

                    pictureBox1.Location = new Point(0, 0);
                    pictureBox1.Size = new Size(test3, test4);

                    pictureBox2.Location = new Point(0, 0);
                    pictureBox2.Size = new Size(test3, test4);


                    //Rect.Width = (int)(Rect.Width * Percentage);
                    //Rect.Height = (int)(Rect.Height * Percentage);
                    //Rect.X = (int)(Rect.X * Percentage);
                    //Rect.Y = (int)(Rect.Y * Percentage);
                }
            }
        }          
    }
}
