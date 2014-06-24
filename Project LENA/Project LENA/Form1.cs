using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/* ---------------------- Added Libraries ---------------------- */
using System.Runtime.InteropServices; // DLLImport
using System.Threading; // CancellationToken
using System.Xml; // Loading xml file parameters
using BitMiracle.LibTiff.Classic; // Use Tiff images
using System.Diagnostics; // Stopwatch
using Microsoft.WindowsAPICodePack.Taskbar; // Taskbar Progress
using System.IO; // BinaryReader, open and save files
using System.Numerics; // Incoporates the use of complex numbers

namespace Project_LENA
{
    public partial class Form1 : Form
    {
        #region Form Initiallization
        Functions functions;
        MLMVN mlmvn;

        CancellationTokenSource cTokenSource1; // Declare a System.Threading.CancellationTokenSource for the fourth tab.
        PauseTokenSource pTokenSource1; // Declaring a usermade pausetoken for the fourth tab.
        CancellationTokenSource cTokenSource2; // Declare a System.Threading.CancellationTokenSource for the third tab.
        PauseTokenSource pTokenSource2; // Declaring a usermade pausetoken for the third tab.

        public Form1()
        {
            functions = new Functions(this);
            mlmvn = new MLMVN(this);
            this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            this.Location = new Point(0, 0);
            InitializeComponent(); // begins the initialization of the form     
            this.Text = "Project LENA " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            openFileDialog1.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), @"Resources");
            openFileDialog2.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), @"Resources");
            openFileDialog3.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), @"Resources");
            openFileDialog4.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), @"Resources");
            openFileDialog5.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), @"Resources");
            openFileDialog6.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), @"Resources");
            this.AutoSize = false;
            this.Height = 250; // inintiallizes the height of the form

        }

        // Avoids flickering
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        // Help button on windows form
        private void Form1_helpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("This program uses an intelligent approach to image processing using MLMVN, " +
                "otherwise known as Multilayer feedforward neural network based on multi-valued neurons." +
                "\n\nThis program can create and generate an image suitable to be filtered, generate samples " +
                "used to create weights used to process images, generate the weights learned from this process," +
                "and filter a noisy image from the weights created. \n\nAdditional help can be provided by " +
                "hovering the cursor over a label of an element of interest.",
                "About Project LENA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            e.Cancel = true; // does not display question mark on cursor after message
        }

        string Title = "Project LENA " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // Selecting different tabs
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage.Name == tabPage1.Name)
            {
                this.timer3.Enabled = false;
                this.timer4.Enabled = false;
                this.timer5.Enabled = false;
                this.timer6.Enabled = false;
                this.timer7.Enabled = false;
                this.timer8.Enabled = false;
                this.timer9.Enabled = false;
                this.timer10.Enabled = false;
                this.timer11.Enabled = false;
                this.timer14.Enabled = false;
                this.timer15.Enabled = false;
                this.timer16.Enabled = false;
                if (this.checkBox5.Checked == false) this.timer1.Enabled = true; // Makes tab smaller; user checked checkbox to generate grayscale image
                else if (this.Height >= 370) this.timer13.Enabled = true;
                else if (this.checkBox5.Checked == true) this.timer12.Enabled = true; // Makes tab larger; user checked checkbox to generate grayscale image
                else this.timer2.Enabled = true;
            }

            if (e.TabPage.Name == tabPage2.Name)
            {
                this.timer1.Enabled = false;
                this.timer2.Enabled = false;
                this.timer5.Enabled = false;
                this.timer6.Enabled = false;
                this.timer7.Enabled = false;
                this.timer8.Enabled = false;
                this.timer9.Enabled = false;
                this.timer10.Enabled = false;
                this.timer11.Enabled = false;
                this.timer12.Enabled = false;
                this.timer13.Enabled = false;
                this.timer14.Enabled = false;
                this.timer15.Enabled = false;
                this.timer16.Enabled = false;
                if (this.Height >= 330) this.timer3.Enabled = true;
                else this.timer4.Enabled = true;
            }

            if (e.TabPage.Name == tabPage3.Name)
            {
                this.timer1.Enabled = false;
                this.timer2.Enabled = false;
                this.timer3.Enabled = false;
                this.timer4.Enabled = false;
                this.timer7.Enabled = false;
                this.timer8.Enabled = false;
                this.timer10.Enabled = false;
                this.timer11.Enabled = false;
                this.timer12.Enabled = false;
                this.timer13.Enabled = false;
                this.timer14.Enabled = false;
                this.timer15.Enabled = false;
                if (this.button7.Enabled == false) this.timer9.Enabled = true; // Makes tab larger; user clicked 'Learn'
                else if (this.button12.Enabled == false) this.timer16.Enabled = true; // Makes tab larger; user clicked 'Test'
                else if (this.Height >= 450) this.timer5.Enabled = true;
                else this.timer6.Enabled = true;
            }

            if (e.TabPage.Name == tabPage4.Name)
            {
                this.timer1.Enabled = false;
                this.timer2.Enabled = false;
                this.timer3.Enabled = false;
                this.timer4.Enabled = false;
                this.timer5.Enabled = false;
                this.timer6.Enabled = false;
                this.timer9.Enabled = false;
                this.timer12.Enabled = false;
                this.timer13.Enabled = false;
                this.timer16.Enabled = false;
                if (this.Height >= 650 && this.button10.Enabled == false) this.timer15.Enabled = true;
                else if (this.button10.Enabled == false) this.timer10.Enabled = true; // Makes tab larger; user clicked 'Process Image'
                else if (radioButton3.Checked == true && this.Height > 390 || radioButton4.Checked == true && this.Height > 390) timer14.Enabled = true;
                else if (radioButton3.Checked == true || radioButton4.Checked == true) this.timer11.Enabled = true; // radiobutton enabled              
                else if (this.Height >= 250) this.timer7.Enabled = true;
                else this.timer8.Enabled = true;
            }
        }
        #endregion

        #region Timers
        // When user clicks tab 1 from a larger tab
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 250) this.timer1.Enabled = false;
            else this.Height -= 20;
        }

        // When user clicks tab 1 from a smaller tab
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 250) this.timer2.Enabled = false;
            else this.Height += 20;
        }

        // When user clicks tab 2 from a larger tab
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 330) this.timer3.Enabled = false;
            else this.Height -= 20;
        }

        // When user clicks tab 2 from a smaller tab
        private void timer4_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 330) this.timer4.Enabled = false;
            else this.Height += 20;
        }

        // When user clicks tab 3 from a larger tab
        private void timer5_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 450) this.timer5.Enabled = false;
            else this.Height -= 20;
        }

        // When user clicks tab 3 from a smaller tab
        private void timer6_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 450) this.timer6.Enabled = false;
            else this.Height += 20;
        }

        // When user clicks tab 4 from a larger tab
        private void timer7_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 250) this.timer7.Enabled = false;
            else this.Height -= 20;
        }

        // When user clicks tab 4 from a smaller tab
        private void timer8_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 250) this.timer8.Enabled = false;
            else this.Height += 20;
        }

        // When user clicks 'Learn' on tab 3
        private void timer9_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 650) this.timer9.Enabled = false;
            else this.Height += 20;
        }

        // When user clicks 'Process Image' on tab 4
        private void timer10_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 650) this.timer10.Enabled = false;
            else this.Height += 20;
        }

        // When user checks a radio button on tab 4
        private void timer11_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 390) this.timer11.Enabled = false;
            else this.Height += 20;
        }

        // When user has grayscale image checkbox on on tab 1 from a smaller tab
        private void timer12_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 370) this.timer12.Enabled = false;
            else this.Height += 20;
        }

        // When user has grayscale image checkbox on on tab 1 from a larger tab
        private void timer13_Tick(object sender, EventArgs e)
        {

            if (this.Height <= 370) this.timer13.Enabled = false;
            else this.Height -= 20;
        }

        // When user checked a radio button on tab 4 from a larger tab
        private void timer14_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 390) this.timer14.Enabled = false;
            else this.Height -= 20;
        }
        // when user clicked 'Process Image' on tab 4 from a larger tab
        private void timer15_Tick(object sender, EventArgs e)
        {
            if (this.Height <= 650) this.timer15.Enabled = false;
            else this.Height -= 20;
        }
        // When user clicks 'Test' on tab 3
        private void timer16_Tick(object sender, EventArgs e)
        {
            if (this.Height >= 610) this.timer16.Enabled = false;
            else this.Height += 20;
        }
        #endregion

        #region Controls

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
            {
                textBox6.Enabled = true;
                button19.Enabled = true;
            }
            else
            {
                textBox6.Enabled = false;
                button19.Enabled = false;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == 2)
            {
                checkBox1.Enabled = true;
            }
            else
            {
                checkBox1.Enabled = false;
            }
        }

        // Process using pixels, tab 2
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox7.Enabled = false;
            comboBox6.Enabled = true;
        }

        // Process using patches, tab 2
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox6.Enabled = false;
            comboBox7.Enabled = true;
        }

        // process using pixels, tab 4
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Height <= 390) this.timer11.Enabled = true;

            label23.Visible = true;
            label23.Text = "Number of sectors:";
            toolTip1.SetToolTip(label23, "The number of sectors to be processed from the unit circle.\r\nUsed for classification in the learning algorithm.");

            textBox13.Visible = true;
            textBox13.Location = new Point(117, 26);

            label24.Visible = true;
            label24.Text = "Input layer size:";
            toolTip1.SetToolTip(label24, "The size of the layers.");
            label24.Location = new Point(210, 30);

            textBox16.Visible = true;
            textBox16.Location = new Point(296, 26);

            label25.Visible = true;
            label25.Text = "Hidden layer size:";
            label25.Location = new Point(390, 30);
            toolTip1.SetToolTip(label25, "The size of the hidden layers used in the weights.");

            textBox17.Visible = true;
            textBox17.Clear();
            textBox17.MinimumSize = new Size(58, 20);
            textBox17.MaximumSize = new Size(58, 20);
            textBox17.Size = new Size(58, 20);
            textBox17.Location = new Point(486, 26);

            label26.Visible = true;
            label26.Text = "Kernel size:";
            toolTip1.SetToolTip(label26, "Size of the kernel surrounding the pixel being processed.");

            comboBox4.Visible = true;
            comboBox4.DropDownStyle = ComboBoxStyle.DropDown;
            comboBox4.Location = new Point(82, 57);
            comboBox4.Items.Clear();
            comboBox4.Items.Add("3 x 3");
            comboBox4.Items.Add("5 x 5");
            comboBox4.Items.Add("7 x 7");
            toolTip1.SetToolTip(comboBox4, "Size of the kernel surrounding the pixel being processed.");

            label5.Visible = false;

            textBox19.Visible = false;
        }

        // process using patches, tab 4
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Height <= 390) this.timer11.Enabled = true;

            label23.Visible = true;
            label23.Text = "Method:";
            toolTip1.SetToolTip(label23, "The patch function to be used.");

            comboBox4.Visible = true;
            comboBox4.Location = new Point(67, 26);
            comboBox4.Items.Clear();
            comboBox4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox4.Items.Add("Legacy method");
            comboBox4.Items.Add("New patch method");
            toolTip1.SetToolTip(comboBox4, "The patch function to be used.");

            label24.Visible = true;
            label24.Text = "Number of sectors:";
            label24.Location = new Point(220, 30);
            toolTip1.SetToolTip(label24, "The number of sectors to be processed from the unit circle.\r\nUsed for classification in the learning algorithm.");
            //toolTip1.SetToolTip(label24, "The size to be overlapped by each patch.");

            textBox13.Visible = true;
            textBox13.Location = new Point(322, 26);

            label25.Visible = true;
            label25.Text = "Step:";
            toolTip1.SetToolTip(label25, "The size to be overlapped by each patch.");
            label25.Location = new Point(424, 29);

            textBox16.Visible = true;
            textBox16.Location = new Point(462, 27);

            label26.Visible = true;
            label26.Text = "Network size:";
            toolTip1.SetToolTip(label26, "The network array to be used to generate the weights.");

            textBox17.Visible = true;
            textBox17.MinimumSize = new Size(100, 20);
            textBox17.MaximumSize = new Size(154, 20);
            Size size = TextRenderer.MeasureText(textBox17.Text, textBox17.Font);
            textBox17.Width = size.Width;
            textBox17.Height = size.Height;
            textBox17.Location = new Point(92, 57);

            label5.Visible = true;
            toolTip1.SetToolTip(label26, "The number of hidden layers in the output.");

            textBox19.Visible = true;
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(textBox17.Text, textBox17.Font);
            textBox17.Width = size.Width;
            textBox17.Height = size.Height;
            //if (textBox17.Width >= 115)
            //{
                label5.Location = new Point(textBox17.Location.X + textBox17.Width + 30, 60);
                textBox19.Location = new Point(label5.Location.X + label5.Width + 6, 57);
            //}
        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(textBox20.Text, textBox20.Font);
            textBox20.Width = size.Width;
            textBox20.Height = size.Height;
            if (textBox20.Width >= 115)
            {
                label16.Location = new Point(textBox20.Location.X + textBox20.Width + 23, 30);
                comboBox2.Location = new Point(label16.Location.X + label16.Width + 6, 26);
            }
        }

        // Load color image
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                textBox11.Text = openFileDialog1.FileName;
        }

        // Create Grayscale Image
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox11.Text))
            {
                MessageBox.Show("Please input the color image.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            else
            {
                Tiff image = Tiff.Open(textBox11.Text, "r");
                // Obtain basic tag information of the image
                #region GetTagInfo
                int width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                byte bits = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                #endregion

                int imageSize = height * width * 3;
                int[] raster = new int[imageSize];

                byte[] scanline = new byte[image.ScanlineSize()];

                // Initiallization of RGB values
                int[,] red = new int[height, width];
                int[,] green = new int[height, width];
                int[,] blue = new int[height, width];

                // Initiallization of YUV values
                double[,] Y = new double[height, width];
                double[,] U = new double[height, width];
                double[,] V = new double[height, width];

                // I want the closest result to VEGA
                for (int i = height - 1; i != -1; i--)
                {
                    image.ReadScanline(scanline, i);
                    for (int j = 0; j < width; j++)
                    {
                        red[i, j] = scanline[3 * j]; // PSNR: INFINITY, Channel is correct
                        green[i, j] = scanline[3 * j + 1]; // PSNR: INFINITY, Channel is correct
                        blue[i, j] = scanline[3 * j + 2]; // PSNR: INFINITY, Channel is correct


                        // This is what MATLAB uses to convert RGB to YUV - PSNR 26.25002 to VEGA
                        //Y[i, j] = (0.257 * red[i, j]) + (0.504 * green[i, j]) + (0.098 * blue[i, j]) + 16;
                        //U[i, j] = -(0.148 * red[i, j]) - (0.291 * green[i, j]) + (0.439 * blue[i, j]) + 128;
                        //V[i, j] = (0.439 * red[i, j]) - (0.368 * green[i, j]) - (0.071 * blue[i, j]) + 128;

                        // Using bitwise shift operations to convert, no success - PSNR 26.21524
                        //Y[i, j] = ((66 * red[i, j] + 129 * green[i, j] + 25 * blue[i, j] + 128) >> 8) + 16;

                        // CCIR Recommendation 709 - PSNR 30.61359 to VEGA
                        //Y[i, j] = (0.2125 * red[i, j]) + (0.7154 * green[i, j]) + (0.0721 * blue[i, j]);

                        // Conversion from RGB to YUV, as written here:
                        // http://www.eagle.tamut.edu/faculty/igor/MY%20CLASSES/CS-467/Lecture-12.pdf on slide 18
                        // Also part of the CCIR Recommendation 601-1 - PSNR 27.8757 to VEGA
                        Y[i, j] = (0.299 * red[i, j]) + (0.587 * green[i, j]) + (0.114 * blue[i, j]);
                        U[i, j] = -(0.14713 * red[i, j]) - (0.28886 * green[i, j]) + (0.436 * blue[i, j]);
                        V[i, j] = (0.615 * red[i, j]) - (0.51499 * green[i, j]) - (0.10001 * blue[i, j]);

                    }
                }

                #region Merge YUV - (for debugging purposes)

                //byte[,] YUV = new byte[height, image.ScanlineSize()];

                //for (int i = 0; i < height; i++)
                //{
                //    for (int j = 0; j < width; j++)
                //    {
                //        YUV[i, 3 * j] = Convert.ToByte(Y[i, j]);
                //    }
                //}

                //for (int i = 0; i < height; i++)
                //{
                //    for (int j = 0; j < width; j++)
                //    {
                //        YUV[i, 3 * j + 1] = Convert.ToByte(U[i, j]);
                //    }
                //}
                //for (int i = 0; i < height; i++)
                //{
                //    for (int j = 0; j < width; j++)
                //    {
                //        YUV[i, 3 * j + 2] = Convert.ToByte(V[i, j]);
                //    }
                //}
                #endregion

                saveFileDialog2.FileName = Path.GetFileNameWithoutExtension(textBox11.Text) + "_Y" + ".tif";

                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    using (Tiff output = Tiff.Open(saveFileDialog2.FileName, "w"))
                    {
                        // Write the tiff tags to the file
                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                        output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, 1);

                        //output.SetField(TiffTag.IMAGEWIDTH, width);
                        //output.SetField(TiffTag.IMAGELENGTH, height);
                        //output.SetField(TiffTag.BITSPERSAMPLE, bits);
                        //output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                        //output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        //output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                        //output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        //output.SetField(TiffTag.ROWSPERSTRIP, height);
                        //output.SetField(TiffTag.XRESOLUTION, dpiX);
                        //output.SetField(TiffTag.YRESOLUTION, dpiY);
                        //output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
                        //output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        //output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                        byte[] im = new byte[width * sizeof(byte /*can be changed depending on the format of the image)*/)];

                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (Y[i, j] > 255) Y[i, j] = 255;
                                if (Y[i, j] < 0) Y[i, j] = 0;
                                im[j] = Convert.ToByte(Y[i, j]);
                            }
                            output.WriteScanline(im, i);
                        }
                        output.WriteDirectory();
                        output.Dispose();

                        #region Save YUV image - (for debugging purposes)

                        //// Write the tiff tags to the file
                        //output.SetField(TiffTag.IMAGEWIDTH, width);
                        //output.SetField(TiffTag.IMAGELENGTH, height);
                        //output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        //output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.SEPARATE);
                        //output.SetField(TiffTag.PHOTOMETRIC, Photometric.YCBCR);
                        //output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        //output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                        ////output.YCBCRCOEFFICIENTS
                        //  //  output.YCBCRSUBSAMPLING
                        //    //    output.YCBCRPOSITIONING = 

                        //byte[] im = new byte[image.ScanlineSize() * sizeof(byte /*can be changed depending on the format of the image*/)];

                        //for (int i = 0; i < height; i++)
                        //{

                        //    for (int j = 0; j < image.ScanlineSize(); j++)
                        //    {
                        //        im[j] = YUV[i, j];
                        //    }
                        //    output.WriteEncodedStrip(i, im, image.ScanlineSize());
                        //}
                        #endregion
                    }
                    //System.Diagnostics.Process.Start(FileName);                   
                }
                textBox18.Text = saveFileDialog2.FileName;
                image.Dispose();
            }
        }

        // Generate Noisy Image
        private void button3_Click(object sender, EventArgs e)
        {
            #region Error Checking
            if (string.IsNullOrEmpty(textBox18.Text))
            {
                MessageBox.Show("Please input the image.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please input the Gaussian noise to add to the image.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            Tiff image = Tiff.Open(textBox18.Text, "r");

            // Obtain basic tag information of the image
            #region GetTagInfo
            int width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            byte bits = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
            byte pixel = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
            #endregion

            #region Grayscale Image
            //if (checkBox4.Checked == false)
            // Attempt to do the grayscale image check via pixel
            if (pixel == 1)
            {
                double noise = Convert.ToDouble(textBox1.Text);

                // Obtain basic tag information of the image
                //#region GetTagInfo
                //int width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                //int height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                //byte bits = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                //byte pixel = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
                //#endregion

                if (string.IsNullOrEmpty(textBox18.Text))
                {
                    MessageBox.Show("Please input the grayscale image.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please enter the noise.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                byte[,] grey = new byte[height, width];
                grey = Functions.Tiff2Array(image, height, width);

                double greysum = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        greysum += grey[i, j];
                    }
                }

                double mean = greysum / (height * width); // image mean

                double variancesum = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        variancesum += Math.Pow((grey[i, j] - mean), 2);
                    }
                }
                double dispersion;
                dispersion = variancesum / (height * width); // image dispersion

                double standarddev;
                standarddev = Math.Sqrt(dispersion);

                // int width = Convert.ToInt32(textBox1.Text);
                // int height = Convert.ToInt32(textBox2.Text);

                // calls the "createRandomTiff" method
                double[,] y = Functions.createRandomTiff(width, height, mean, standarddev, noise, grey, checkBox3.Checked);

                string fileName = textBox18.Text;

                if (checkBox3.Checked == true)
                    fileName = Path.GetFileNameWithoutExtension(textBox18.Text) + "_Gauss_" + Convert.ToString(noise) + ".tif";
                if (checkBox3.Checked == false)
                    fileName = Path.GetFileNameWithoutExtension(textBox18.Text) + "_Gauss_" + Convert.ToString(noise) + "_Noise" + ".tif";

                saveFileDialog2.FileName = fileName;

                if (saveFileDialog2.ShowDialog() == DialogResult.OK) // Test result.
                {
                    using (Tiff output = Tiff.Open(saveFileDialog2.FileName, "w"))
                    {
                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                        output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        output.SetField(TiffTag.ROWSPERSTRIP, height);
                        output.SetField(TiffTag.XRESOLUTION, 88.0);
                        output.SetField(TiffTag.YRESOLUTION, 88.0);
                        output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                        output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                        byte[] im = new byte[width * sizeof(byte /*can be changed depending on the format of the image*/)];

                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                im[j] = Convert.ToByte(y[i, j]);
                            }
                            output.WriteScanline(im, i);
                        }
                        output.WriteDirectory();
                        output.Dispose();
                    }
                }
                image.Dispose();
            }
            #endregion

            #region Color Image
            if (pixel == 3)
            {
                double noise = Convert.ToDouble(textBox1.Text);

                //// Obtain basic tag information of the image
                //#region GetTagInfo
                //int width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                //int height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                //byte bits = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                //byte pixel = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
                //#endregion

                int test = (int)bits;

                if (string.IsNullOrEmpty(textBox18.Text))
                {
                    MessageBox.Show("Please input the grayscale image.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("Please enter the noise.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int imageSize = height * width * 3;
                byte[] raster = new byte[imageSize];

                byte[] scanline = new byte[image.ScanlineSize()];

                // Read the image into the memory buffer
                byte[,] red = new byte[height, width];
                byte[,] green = new byte[height, width];
                byte[,] blue = new byte[height, width];

                //for (int i = height - 1; i != -1; i--)
                for (int i = 0; height > i; i++)
                {
                    image.ReadScanline(scanline, i); // EVIL BUG HERE
                    for (int j = 0; j < width; j++)
                    {
                        red[i, j] = scanline[3 * j]; // PSNR: INFINITY, Channel is correct
                        green[i, j] = scanline[3 * j + 1]; // PSNR: INFINITY, Channel is correct
                        blue[i, j] = scanline[3 * j + 2]; // PSNR: INFINITY, Channel is correct
                    }
                }

                byte[,] RGB = new byte[height, image.ScanlineSize()];

                #region Grayscale Gaussian noise
                if (checkBox4.Checked)
                {
                    #region Y
                    double[,] y = new double[height, width];
                    byte[,] Y = new byte[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            y[i, j] = (0.299 * red[i, j]) + (0.587 * green[i, j]) + (0.114 * blue[i, j]);

                            Y[i, j] = Convert.ToByte(y[i, j]);
                        }

                    }

                    double ysum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ysum += Y[i, j];
                        }
                    }

                    double mean_Y = ysum / (height * width); // image mean

                    double variancesum_Y = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            variancesum_Y += Math.Pow((Y[i, j] - mean_Y), 2);
                        }
                    }
                    double dispersion_Y;
                    dispersion_Y = variancesum_Y / (height * width); // image dispersion

                    double standarddev_Y;
                    standarddev_Y = Math.Sqrt(dispersion_Y);

                    Random φ = new Random(); // Greek letter phi
                    Random Γ = new Random();// Greek letter gamma

                    // sine and cosine variables for the Box-Muller algorithm
                    double[,] z1 = new double[height, width];
                    double[,] z2 = new double[height, width];

                    // normally distributed variables gathered from Box-Muller algorithm with added image mean and sigma
                    double[,] x1 = new double[height, width];
                    double[,] x2 = new double[height, width];

                    double number; // used to fix bug

                    // applying Gaussian noise to each pixel
                    for (int i = 0; i < height; ++i)
                    {
                        for (int j = 0; j < width; ++j)
                        {
                            // the Box-Muller algorithm
                            z1[i, j] = Math.Cos(2 * Math.PI * φ.NextDouble()) * Math.Sqrt(-2 * Math.Log(Γ.NextDouble()));
                            z2[i, j] = Math.Sin(2 * Math.PI * φ.NextDouble()) * Math.Sqrt(-2 * Math.Log(Γ.NextDouble()));

                            number = φ.NextDouble(); // fixes bug (for some reason)

                            x1[i, j] = mean_Y + z1[i, j] * noise * standarddev_Y;
                            x2[i, j] = mean_Y + z2[i, j] * noise * standarddev_Y;

                            #region Polar form of the Box-Muller algorithm
                            //do
                            //{
                            //    x3[i, j] = 2 * φ.NextDouble() - 1;
                            //    x4[i, j] = 2 * Γ.NextDouble() - 1;
                            //    w[i, j] = x3[i, j] * x3[i, j] + x4[i, j] * x4[i, j];
                            //} while (w[i, j] >= 1);

                            //w[i, j] = Math.Sqrt((-2 * Math.Log(w[i, j])) / w[i, j]);
                            //z1[i, j] = x3[i, j] * w[i, j];
                            //z2[i, j] = x4[i, j] * w[i, j];

                            //x1[i, j] = m + z1[i, j] * noise * σ;
                            //x2[i, j] = m + z2[i, j] * noise * σ;
                            #endregion
                        }
                    }

                    #endregion

                    #region Red

                    double redsum = 0;
                    double[,] r = new double[height, width];

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            redsum += red[i, j];
                        }
                    }

                    double mean_R = redsum / (height * width); // image mean

                    // Applies the Y Gaussian noise to red
                    #region Checkbox status
                    if (checkBox3.Checked == true)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    r[i, j] = red[i, j] + x1[i, j] - mean_Y;
                                if (j % 2 == 0)
                                    r[i, j] = red[i, j] + x2[i, j] - mean_Y;

                                if (r[i, j] > 255) r[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (r[i, j] < 0) r[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }

                    else if (checkBox3.Checked == false)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    r[i, j] = x1[i, j];
                                if (j % 2 == 0)
                                    r[i, j] = x2[i, j];

                                if (r[i, j] > 255) r[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (r[i, j] < 0) r[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }
                    #endregion

                    #endregion

                    #region Green
                    double[,] g = new double[height, width];
                    double greensum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            greensum += green[i, j];
                        }
                    }

                    double mean_G = greensum / (height * width); // image mean

                    // Applies the Y Gaussian noise to green
                    #region Checkbox status
                    if (checkBox3.Checked == true)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    g[i, j] = green[i, j] + x1[i, j] - mean_Y;
                                if (j % 2 == 0)
                                    g[i, j] = green[i, j] + x2[i, j] - mean_Y;

                                if (g[i, j] > 255) g[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (g[i, j] < 0) g[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }

                    else if (checkBox3.Checked == false)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    g[i, j] = x1[i, j];
                                if (j % 2 == 0)
                                    g[i, j] = x2[i, j];

                                if (g[i, j] > 255) g[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (g[i, j] < 0) g[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }
                    #endregion

                    #endregion

                    #region Blue
                    double[,] b = new double[height, width];
                    double bluesum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            bluesum += blue[i, j];
                        }
                    }

                    double mean_B = bluesum / (height * width); // image mean

                    // Applies the Y Gaussian noise to blue
                    #region Checkbox status
                    if (checkBox3.Checked == true)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    b[i, j] = blue[i, j] + x1[i, j] - mean_Y;
                                if (j % 2 == 0)
                                    b[i, j] = blue[i, j] + x2[i, j] - mean_Y;

                                if (b[i, j] > 255) b[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (b[i, j] < 0) b[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }

                    else if (checkBox3.Checked == false)
                    {
                        for (int i = 0; i < height; ++i)
                        {
                            for (int j = 0; j < width; ++j)
                            {
                                if (j % 2 != 0)
                                    b[i, j] = x1[i, j];
                                if (j % 2 == 0)
                                    b[i, j] = x2[i, j];

                                if (b[i, j] > 255) b[i, j] = 255; // Whenever processed value of pixel is above 255, cap it at 255
                                if (b[i, j] < 0) b[i, j] = 0; // Whenever processed value of pixel is below 0, cap it at 0
                            }
                        }
                    }
                    #endregion

                    #endregion

                    #region Merge RGB
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j] = Convert.ToByte(r[i, j]);
                        }
                    }

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j + 1] = Convert.ToByte(g[i, j]);
                        }
                    }
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j + 2] = Convert.ToByte(b[i, j]);
                        }
                    }
                    #endregion
                }
                #endregion

                #region Color Gaussian noise
                if (!checkBox4.Checked)
                {
                    #region Red

                    double redsum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            redsum += red[i, j];
                        }
                    }

                    double mean_R = redsum / (height * width); // image mean

                    double variancesum_R = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            variancesum_R += Math.Pow((red[i, j] - mean_R), 2);
                        }
                    }
                    double dispersion_R;
                    dispersion_R = variancesum_R / (height * width); // image dispersion

                    double standarddev_R;
                    standarddev_R = Math.Sqrt(dispersion_R);

                    // int width = Convert.ToInt32(textBox1.Text);
                    // int height = Convert.ToInt32(textBox2.Text);

                    // calls the "createRandomTiff" method

                    double[,] r = Functions.createRandomTiff(width, height, mean_R, standarddev_R, noise, red, checkBox3.Checked);
                    #endregion

                    #region Green
                    double greensum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            greensum += green[i, j];
                        }
                    }

                    double mean_G = greensum / (height * width); // image mean

                    double variancesum_G = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            variancesum_G += Math.Pow((green[i, j] - mean_G), 2);
                        }
                    }
                    double dispersion_G;
                    dispersion_G = variancesum_G / (height * width); // image dispersion

                    double standarddev_G;
                    standarddev_G = Math.Sqrt(dispersion_G);

                    // int width = Convert.ToInt32(textBox1.Text);
                    // int height = Convert.ToInt32(textBox2.Text);

                    // calls the "createRandomTiff" method

                    double[,] g = Functions.createRandomTiff(width, height, mean_G, standarddev_G, noise, green, checkBox3.Checked);
                    #endregion

                    #region Blue
                    double bluesum = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            bluesum += blue[i, j];
                        }
                    }

                    double mean_B = bluesum / (height * width); // image mean

                    double variancesum_B = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            variancesum_B += Math.Pow((blue[i, j] - mean_B), 2);
                        }
                    }
                    double dispersion_B;
                    dispersion_B = variancesum_B / (height * width); // image dispersion

                    double standarddev_B;
                    standarddev_B = Math.Sqrt(dispersion_B);

                    // int width = Convert.ToInt32(textBox1.Text);
                    // int height = Convert.ToInt32(textBox2.Text);

                    // calls the "createRandomTiff" method

                    double[,] b = Functions.createRandomTiff(width, height, mean_B, standarddev_B, noise, blue, checkBox3.Checked);
                    #endregion

                    #region Merge RGB
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j] = Convert.ToByte(r[i, j]);
                        }
                    }

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j + 1] = Convert.ToByte(g[i, j]);
                        }
                    }
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            RGB[i, 3 * j + 2] = Convert.ToByte(b[i, j]);
                        }
                    }
                    #endregion
                }
                #endregion



                string fileName = textBox18.Text;
                if (checkBox4.Checked)
                {
                    if (checkBox3.Checked == true)
                        fileName = Path.GetFileNameWithoutExtension(textBox18.Text) + "_Gauss_Y_" + Convert.ToString(noise) + ".tif";
                    if (checkBox3.Checked == false)
                        fileName = Path.GetFileNameWithoutExtension(textBox18.Text) + "_Gauss_Y_" + Convert.ToString(noise) + "_Noise" + ".tif";
                }
                if (!checkBox4.Checked)
                {
                    if (checkBox3.Checked == true)
                        fileName = Path.GetFileNameWithoutExtension(textBox18.Text) + "_Gauss_" + Convert.ToString(noise) + ".tif";
                    if (checkBox3.Checked == false)
                        fileName = Path.GetFileNameWithoutExtension(textBox18.Text) + "_Gauss_" + Convert.ToString(noise) + "_Noise" + ".tif";
                }
                saveFileDialog2.FileName = fileName;

                if (saveFileDialog2.ShowDialog() == DialogResult.OK) // Test result.
                {
                    using (Tiff output = Tiff.Open(saveFileDialog2.FileName, "w"))
                    {
                        //output.SetField(TiffTag.IMAGEWIDTH, width);
                        //output.SetField(TiffTag.IMAGELENGTH, height);
                        //output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                        //output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        //output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        //output.SetField(TiffTag.ROWSPERSTRIP, height);
                        //output.SetField(TiffTag.XRESOLUTION, 88.0);
                        //output.SetField(TiffTag.YRESOLUTION, 88.0);
                        //output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        //output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        //output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                        //output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        //output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

                        // Write the tiff tags to the file
                        output.SetField(TiffTag.IMAGEWIDTH, width);
                        output.SetField(TiffTag.IMAGELENGTH, height);
                        output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                        output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        output.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        output.SetField(TiffTag.ROWSPERSTRIP, height);
                        output.SetField(TiffTag.XRESOLUTION, 88.0);
                        output.SetField(TiffTag.YRESOLUTION, 88.0);
                        output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                        output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);


                        //output.SetField(TiffTag.IMAGEWIDTH, width);
                        //output.SetField(TiffTag.IMAGELENGTH, height);
                        //output.SetField(TiffTag.COMPRESSION, Compression.NONE);
                        //output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        //output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                        //output.SetField(TiffTag.BITSPERSAMPLE, 8);
                        //output.SetField(TiffTag.SAMPLESPERPIXEL, 3);

                        //int width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                        //int height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                        //byte bits = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
                        //byte pixel = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();

                        byte[] im = new byte[image.ScanlineSize() * sizeof(byte /*can be changed depending on the format of the image*/)];

                        for (int i = 0; i < height; i++)
                        {

                            for (int j = 0; j < image.ScanlineSize(); j++)
                            {
                                im[j] = RGB[i, j];
                            }
                            output.WriteEncodedStrip(i, im, image.ScanlineSize());
                        }
                        output.WriteDirectory();
                        output.Dispose();
                    }
                }
                image.Dispose();
            }
            #endregion
        }

        // Load Clean Image
        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK) // Test result.
                textBox15.Text = openFileDialog2.FileName;
        }

        // Load Noisy Image
        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK) // Test result.
                textBox14.Text = openFileDialog3.FileName;
        }

        // Generate Samples
        private void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false;

            // open the images
            Tiff cleanimage = Tiff.Open(textBox15.Text, "r");
            Tiff noisedimage = Tiff.Open(textBox14.Text, "r"); ;

            #region Error Checking
            // Error Windows when no image entered
            if (cleanimage == null || noisedimage == null)
            {
                button6.Enabled = true;
                MessageBox.Show("Invalid or no image entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Error Windows when no radio button checked
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                button6.Enabled = true;
                MessageBox.Show("No inplementation checked.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Error Windows when no number of samples entered
            if (comboBox8.Text == "")
            {
                button6.Enabled = true;
                MessageBox.Show("Please enter the number of samples.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            int sSize = Convert.ToInt32(comboBox8.Text);

            // Obtain basic tag information of the image
            #region GetTagInfo
            int width = cleanimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int height = cleanimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            byte bits = cleanimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
            byte pixel = cleanimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
            double dpiX = cleanimage.GetField(TiffTag.XRESOLUTION)[0].ToDouble();
            double dpiY = cleanimage.GetField(TiffTag.YRESOLUTION)[0].ToDouble();
            #endregion

            // The clean image
            byte[,] clean = new byte[height, width];
            clean = Functions.Tiff2Array(cleanimage, height, width);

            // The noisy image
            byte[,] noised = new byte[height, width];
            noised = Functions.Tiff2Array(noisedimage, height, width);

            #region Samples using Pixels
            if (radioButton1.Checked) // Process using pixels
            {
                int kernel = 0;

                if (comboBox6.Text == "")
                {
                    // Error Windows when no number of samples entered
                    button6.Enabled = true;
                    MessageBox.Show("No kernel size selected.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    char[] c = comboBox6.Text.ToCharArray(); // seperates compbox elements into an array

                    for (int i = 0; i < c.Length; i++)
                    {
                        if (c[i].ToString() == " " || c[i].ToString() == "x" || c[i].ToString() == "X")
                            break;
                        else
                            kernel = Convert.ToInt32(comboBox6.Text.Substring(0, i + 1));
                    }

                    // ************************  Let the user enter any odd number as size of the pixel
                    if (kernel % 2 != 1)
                    {
                        button6.Enabled = true;
                        MessageBox.Show("Please enter an odd number", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string fileName = Path.GetFileNameWithoutExtension(textBox14.Text) + "_Samples_" + comboBox8.Text + "_Pixels_" + kernel + "x" + kernel + ".txt";

                saveFileDialog1.FileName = fileName;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Functions.LearnSet(clean, noised, kernel, sSize, saveFileDialog1.FileName);
                }
            }
            #endregion

            #region Samples using Patches
            else if (radioButton2.Checked) // Process using patches
            {
                // combobox values
                int kernel = 0;

                if (comboBox7.Text == "")
                {
                    // Error Windows when no number of samples entered
                    button6.Enabled = true;
                    MessageBox.Show("No kernel size selected.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    char[] c = comboBox7.Text.ToCharArray(); // seperates compbox elements into an array

                    for (int i = 0; i < c.Length; i++)
                    {
                        if (c[i].ToString() == " " || c[i].ToString() == "x" || c[i].ToString() == "X")
                            break;
                        else
                            kernel = Convert.ToInt32(comboBox7.Text.Substring(0, i + 1));
                    }

                    // ************************  Let the user enter any odd number as size of the patch
                    if (kernel % 2 != 1)
                    {
                        button6.Enabled = true;
                        MessageBox.Show("Please enter an odd number", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }


                string fileName = Path.GetFileNameWithoutExtension(textBox14.Text) + "_Samples_" + comboBox8.Text + "_Patches_" + kernel + "x" + kernel + ".txt";

                saveFileDialog1.FileName = fileName;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Functions.LearnSetPatch(clean, noised, kernel, sSize, saveFileDialog1.FileName);
                }
            }
            #endregion

            cleanimage.Dispose();
            noisedimage.Dispose();
            button6.Enabled = true;
        }

        // Learn button
        private async void button7_Click(object sender, EventArgs e)
        {
            this.button7.Enabled = false;

            #region Error checking
            // Error Windows when no image entered
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                button7.Enabled = true;
                MessageBox.Show("Samples file not entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (comboBox1.SelectedIndex == -1)
            {
                button7.Enabled = true;
                MessageBox.Show("No weight type selected.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox1.SelectedIndex == 1 && string.IsNullOrEmpty(textBox6.Text))
            {
                button7.Enabled = true;
                MessageBox.Show("Selected existing weights, but no weights entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Error Windows when no radio button checked
            if (string.IsNullOrEmpty(textBox20.Text) || string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox7.Text))
            {
                button7.Enabled = true;
                MessageBox.Show("Check for empty parameters.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox2.SelectedIndex == -1)
            {
                button7.Enabled = true;
                MessageBox.Show("No output type selected.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox3.SelectedIndex == -1)
            {
                button7.Enabled = true;
                MessageBox.Show("No stopping criteria algorithm selected.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text))
            {
                button7.Enabled = true;
                MessageBox.Show("Check for empty threshold values.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            #region Variable Initiallization
            string Weights = textBox6.Text;

            string Samples = textBox2.Text;

            // New cancellation token
            cTokenSource2 = new CancellationTokenSource();

            // Create a cancellation token from CancellationTokenSource
            var cToken = cTokenSource2.Token;

            // New pause token
            pTokenSource2 = new PauseTokenSource();

            // Create a pause token from PauseTokenSource
            var pToken = pTokenSource2.Token;

            bool randomWeights = false;
            if (comboBox1.SelectedIndex == 0)
            {
                randomWeights = true;
            }
            if (comboBox1.SelectedIndex == 1)
            {
                randomWeights = false;
            }

            int NumberofSamples = Convert.ToInt32(textBox8.Text);

            double GlobalThreshold = Convert.ToDouble(textBox4.Text);
            double LocalThreshold = Convert.ToDouble(textBox5.Text);

            // convert string array to int
            string[] a = textBox20.Text.Split(',', '.');
            int[] networkSize = new int[4];
            for (int i = 0; i < a.Length; i++)
            {
                networkSize[i] = Convert.ToInt32(a[i]);
            }

            // determine number of samples
            int[] inputsPerSample = new int[a.Length];
            inputsPerSample[0] = networkSize[a.Length - 1] + Convert.ToInt32(textBox21.Text);
            for (int i = 1; i < a.Length; i++)
                inputsPerSample[i] = networkSize[0] + Convert.ToInt32(textBox21.Text);
            // end for

            int NumberofSectors = Convert.ToInt32(textBox7.Text);
            #endregion

            this.comboBox3.Enabled = false;
            this.checkBox1.Enabled = false;
            this.textBox5.Enabled = false;
            this.textBox4.Enabled = false;
            this.button15.Enabled = true;
            this.checkBox6.Enabled = true;
            this.button21.Enabled = true;
            this.timer9.Enabled = true;

            // Begin processing
            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();
            this.Text = Title + " (Working)";
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

            try
            {
                Complex[][,] weights = await Task.Run(() => mlmvn.Learning(Samples, NumberofSamples, Weights, 4, networkSize, inputsPerSample, NumberofSectors, GlobalThreshold, LocalThreshold, randomWeights, cTokenSource2.Token, pTokenSource2.Token));

                string[] imagename = Path.GetFileNameWithoutExtension(textBox2.Text).Split('_');

                string fileName = imagename[0] + "_" + imagename[1] + "_" + imagename[2] + "_" + imagename[3] + "_Samples_" + NumberofSamples + "_Network_[" +
                        networkSize[0] + "," + networkSize[1] + "," + networkSize[2] + "," + networkSize[3] + "]" + "_RMSE_" + GlobalThreshold + ".wgt";

                // Stop timing
                stopwatch.Stop();

                // Write result
                SetText2("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine);

                saveFileDialog4.FileName = fileName;

                if (saveFileDialog4.ShowDialog() == DialogResult.OK) // Test result.
                {
                    MLMVN.saveMlmvnWeights(saveFileDialog4.FileName, weights, networkSize);
                }
            }
            catch (OperationCanceledException)
            {
                SetText2("\r\nProgress canceled.\r\n");

                // Stop timing
                stopwatch.Stop();

                // Write result
                SetText2("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine + Environment.NewLine);

                // Set the CancellationTokenSource to null when the work is complete.
                cTokenSource2 = null;
            }
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);

            cTokenSource2 = null;

            if (comboBox3.SelectedIndex == 2)
            {
                checkBox1.Enabled = true;
            }

            this.comboBox3.Enabled = true;
            this.textBox5.Enabled = true;
            this.textBox4.Enabled = true;
            this.button7.Enabled = true;
            this.button15.Enabled = false;
            this.checkBox6.Enabled = false;

            this.Text = Title;
        }


        // Load Noisy Image
        private void button8_Click(object sender, EventArgs e)
        {
            if (openFileDialog3.ShowDialog() == DialogResult.OK) // Test result.
                textBox9.Text = openFileDialog3.FileName;
        }

        // Load Weights
        private void button9_Click(object sender, EventArgs e)
        {
            if (openFileDialog4.ShowDialog() == DialogResult.OK) // Test result.
                textBox10.Text = openFileDialog4.FileName;
        }

        // Process Image
        private async void button10_Click(object sender, EventArgs e)
        {
            button10.Enabled = false; // Process Image button
            button11.Enabled = true; // Cancel button
            checkBox2.Checked = false; // Pause button; unchecked            
            checkBox2.Enabled = true; // Pause button; enabled
            radioButton3.Enabled = false; // process using pixels button
            radioButton4.Enabled = false; // process using patches button
            button17.Enabled = false; // load parameters button
            button22.Enabled = false; // save parameters button

            // open the noisy image
            Tiff noisyimage = Tiff.Open(textBox9.Text, "r");

            // open the weights
            string weights = textBox10.Text;

            #region Error checking
            // Error Windows when no image entered
            if (noisyimage == null)
            {
                button10.Enabled = true;
                button11.Enabled = false;
                checkBox2.Enabled = false;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
                button17.Enabled = true;
                button22.Enabled = true;
                MessageBox.Show("Invalid or no image entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (weights == "")
            {
                button10.Enabled = true;
                button11.Enabled = false;
                checkBox2.Enabled = false;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
                button17.Enabled = true;
                button22.Enabled = true;
                MessageBox.Show("No weights entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Error Windows when no radio button checked
            if (!radioButton3.Checked && !radioButton4.Checked)
            {
                button10.Enabled = true;
                button11.Enabled = false;
                checkBox2.Enabled = false;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
                button17.Enabled = true;
                button22.Enabled = true;
                MessageBox.Show("No inplementation checked.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            // New cancellation token
            cTokenSource1 = new CancellationTokenSource();

            // Create a cancellation token from CancellationTokenSource
            var cToken = cTokenSource1.Token;

            // New pause token
            pTokenSource1 = new PauseTokenSource();

            // Create a pause token from PauseTokenSource
            var pToken = pTokenSource1.Token;

            // Obtain basic tag information of the image
            #region GetTagInfo
            int width = noisyimage.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int height = noisyimage.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
            byte bits = noisyimage.GetField(TiffTag.BITSPERSAMPLE)[0].ToByte();
            byte pixel = noisyimage.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToByte();
            double dpiX = noisyimage.GetField(TiffTag.XRESOLUTION)[0].ToDouble();
            double dpiY = noisyimage.GetField(TiffTag.YRESOLUTION)[0].ToDouble();
            #endregion

            // Display information
            SetText1("Image information:" + Environment.NewLine);
            SetText1("Width is : " + width + "\r\nHeight is: " + height + "\r\nDpi is: " + dpiX
                + "\r\nThe scanline is " + noisyimage.ScanlineSize() + ".\r\nBits per Sample is: " + bits + "\r\nSample per pixel is: " + pixel + "\r\n" + Environment.NewLine);

            // Store the intensity values of the image to 2d array                              
            byte[,] noisy = new byte[height, width];
            noisy = Functions.Tiff2Array(noisyimage, height, width);

            // remove the loaded image from memory
            noisyimage.Dispose();

            // Update title text
            this.Text = Title + " (Working)";

            #region Process using pixels
            if (radioButton3.Checked) // Process using pixels
            {
                try
                {
                    if (string.IsNullOrEmpty(textBox13.Text) || string.IsNullOrEmpty(textBox16.Text) || string.IsNullOrEmpty(textBox17.Text) ||
                        comboBox4.SelectedIndex == -1)
                    {
                        button10.Enabled = true;
                        button11.Enabled = false;
                        checkBox2.Enabled = false;
                        radioButton3.Enabled = true;
                        radioButton4.Enabled = true;
                        button17.Enabled = true;
                        button22.Enabled = true;
                        MessageBox.Show("Please load or enter parameters.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // parameters
                    int numberofsectors = Convert.ToInt32(textBox13.Text);
                    int inLayerSize = Convert.ToInt32(textBox16.Text);
                    int hidLayerSize = Convert.ToInt32(textBox17.Text);

                    // combobox values
                    int kernel = 0;

                    if (comboBox4.Text == "")
                    {
                        // Error Windows when no number of samples entered
                        button6.Enabled = true;
                        MessageBox.Show("No kernel size selected.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        char[] c = comboBox4.Text.ToCharArray(); // seperates compbox elements into an array

                        for (int i = 0; i < c.Length; i++)
                        {
                            if (c[i].ToString() == " " || c[i].ToString() == "x" || c[i].ToString() == "X")
                                break;
                            else
                                kernel = Convert.ToInt32(comboBox4.Text.Substring(0, i + 1));
                        }

                        // ************************  Let the user enter any odd number as size of the patch
                        if (kernel % 2 != 1)
                        {
                            MessageBox.Show("Please enter an odd number", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            button10.Enabled = true;
                            button11.Enabled = false;
                            checkBox2.Enabled = false;
                            radioButton3.Enabled = true;
                            radioButton4.Enabled = true;
                            button17.Enabled = true;
                            button22.Enabled = true;
                            this.Text = Title;
                            return;
                        }
                    }

                    // Enable the resize event
                    this.timer10.Enabled = true;

                    // Initiallization of progress bar elements
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                    progressBar1.Maximum = height + 8;
                    progressBar1.Value = 0;
                    TaskbarManager.Instance.SetProgressValue(0, progressBar1.Maximum);
                    NativeMethods.SetState(progressBar1, 1);

                    progressBar1.Value += 4;
                    TaskbarManager.Instance.SetProgressValue(progressBar1.Value, progressBar1.Maximum);

                    byte[,] denoised = await Task.Run(() => mlmvn.Activation(noisy, kernel, weights, numberofsectors, inLayerSize, hidLayerSize, cTokenSource1.Token, pTokenSource1.Token, progressBar1.Value, progressBar1.Maximum));

                    // Stop timing
                    stopwatch.Stop();

                    // Write result
                    SetText1("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine);

                    string fileName = Path.GetFileNameWithoutExtension(textBox9.Text) + "_Pixels_" + kernel + ".tif";

                    saveFileDialog2.FileName = fileName;

                    if (saveFileDialog2.ShowDialog() == DialogResult.OK) // Test result.
                    {

                        functions.WriteToFile(denoised, width, height, bits, pixel, dpiX, dpiY, saveFileDialog2.FileName);

                    }
                }
                catch (OperationCanceledException)
                {
                    SetText1("\r\nProgress canceled.\r\n");
                    // Set the CancellationTokenSource to null when the work is complete.
                    cTokenSource1 = null;

                    this.Text = Title;
                    // Stop timing
                    stopwatch.Stop();

                    // Write result
                    SetText1("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine);
                    button10.Enabled = true;
                    button11.Enabled = false;
                    checkBox2.Enabled = false;
                    radioButton3.Enabled = true;
                    radioButton4.Enabled = true;
                    button17.Enabled = true;
                    button22.Enabled = true;
                    return;
                }
            }
            #endregion

            #region Process using patches

            else if (radioButton4.Checked) // Process using patches
            {
                try
                {
                    if (string.IsNullOrEmpty(textBox13.Text) || string.IsNullOrEmpty(textBox16.Text) || string.IsNullOrEmpty(textBox17.Text))
                    {
                        MessageBox.Show("Please load or enter parameters.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        button10.Enabled = true;
                        button11.Enabled = false;
                        checkBox2.Enabled = false;
                        radioButton3.Enabled = true;
                        radioButton4.Enabled = true;
                        button17.Enabled = true;
                        button22.Enabled = true;
                        this.Text = Title;
                        return;
                    }


                    // network size
                    string[] a = textBox17.Text.Split(',', '.');
                    int[] networkSize = new int[a.Length];
                    int layer = a.Length;


                    for (int i = 0; i < a.Length; i++)
                    {
                        networkSize[i] = Convert.ToInt32(a[i]);
                    }

                    // determine number of samples
                    int[] inputsPerSample = new int[layer];
                    inputsPerSample[0] = networkSize[layer - 1] + Convert.ToInt32(textBox19.Text);
                    for (int i = 1; i < layer; i++)
                        inputsPerSample[i] = networkSize[0] + Convert.ToInt32(textBox19.Text);
                    // end for

                    // parameters
                    int numberofsectors = Convert.ToInt32(textBox13.Text);
                    int step = Convert.ToInt32(textBox16.Text);

                    byte[,] denoised = null;

                    // Initiallization of progress bar elements
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                    int range_x;
                    int pSize = (int)Math.Sqrt(networkSize[3]);

                    // using old patch method
                    if (comboBox4.SelectedIndex == 0)
                    {

                        int range_y = (height - pSize) / step + 2;
                        range_x = (width - pSize) / step + 2;
                        progressBar1.Maximum = (range_x * range_y) + 4;// * ( 4 + range_y % 4) + 4; // range_x * (range into fourths + range_Y % 4) + 4
                    }
                    // using new patch method
                    else if (comboBox4.SelectedIndex == 1)
                    {
                        int interval = pSize - (step * 2);
                        int range_y = (height - (pSize - step)) / interval + 2;
                        range_x = (width - (pSize - step)) / interval + 2;
                        progressBar1.Maximum = (range_x * range_y) + 4;
                    }
                    else
                    {
                        MessageBox.Show("No patch method entered.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        button10.Enabled = true;
                        button11.Enabled = false;
                        checkBox2.Enabled = false;
                        radioButton3.Enabled = true;
                        radioButton4.Enabled = true;
                        button17.Enabled = true;
                        button22.Enabled = true;
                        this.Text = Title;
                        return;
                    }

                    // Enable the resize event
                    this.timer10.Enabled = true;


                    progressBar1.Step = range_x;
                    progressBar1.Value = 0;
                    TaskbarManager.Instance.SetProgressValue(0, progressBar1.Maximum);
                    NativeMethods.SetState(progressBar1, 1);
                    progressBar1.Value += 2;
                    TaskbarManager.Instance.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
                    if (comboBox4.SelectedIndex == 0)
                    {
                        denoised = await Task.Run(() => mlmvn.fdenoiseNeural(noisy, step, weights, layer, networkSize, inputsPerSample, numberofsectors, cTokenSource1.Token, pTokenSource1.Token, progressBar1.Value, progressBar1.Maximum));
                    }
                    else if (comboBox4.SelectedIndex == 1)
                    {
                        denoised = await Task.Run(() => mlmvn.fdenoiseNeural2(noisy, step, weights, layer, networkSize, inputsPerSample, numberofsectors, cTokenSource1.Token, pTokenSource1.Token, progressBar1.Value, progressBar1.Maximum));
                    }
                    string fileName = Path.GetFileNameWithoutExtension(textBox9.Text) + "_Patches_" + pSize + "_Network_[" +
                        networkSize[0] + "," + networkSize[1] + "," + networkSize[2] + "," + networkSize[3] + "]" + ".tif";

                    // Stop timing
                    stopwatch.Stop();

                    // Write result
                    SetText1("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine);

                    saveFileDialog2.FileName = fileName;

                    if (saveFileDialog2.ShowDialog() == DialogResult.OK) // Test result.
                    {
                        functions.WriteToFile(denoised, width, height, bits, pixel, dpiX, dpiY, saveFileDialog2.FileName);
                    }
                }
                catch (OperationCanceledException)
                {
                    SetText1("\r\nProgress canceled.\r\n");
                    // Set the CancellationTokenSource to null when the work is complete.
                    cTokenSource1 = null;

                    this.Text = Title;
                    // Stop timing
                    stopwatch.Stop();

                    // Write result
                    SetText1("Time elapsed: " + stopwatch.Elapsed + Environment.NewLine);
                    button10.Enabled = true;
                    button11.Enabled = false;
                    checkBox2.Enabled = false;
                    radioButton3.Enabled = true;
                    radioButton4.Enabled = true;
                    button17.Enabled = true;
                    button22.Enabled = true;
                    this.Text = Title;
                    return;
                }
            }
            #endregion

            // Set the CancellationTokenSource to null when the work is complete.
            cTokenSource1 = null;

            this.Text = Title;

            button10.Enabled = true;
            button11.Enabled = false;
            checkBox2.Enabled = false;
            radioButton3.Enabled = true;
            radioButton4.Enabled = true;
            button17.Enabled = true;
            button22.Enabled = true;
            progressBar1.Value = 0;
            TaskbarManager.Instance.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
        }

        // Cancel Button
        private void button11_Click(object sender, EventArgs e)
        {
            if (cTokenSource1 != null)
            {
                // progressBar color to red
                NativeMethods.SetState(progressBar1, 2);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused); // Fix to Windows 7 Progressbar bug
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                cTokenSource1.Cancel();
            }
        }

        // Test Weights
        private async void button12_Click(object sender, EventArgs e)
        {
            this.button12.Enabled = false;
            #region Variable Initiallization
            string Weights = textBox6.Text;

            string Samples = textBox2.Text;
            #endregion

            #region Error checking
            // Error Windows when no image entered
            if (Samples == "")
            {
                button12.Enabled = true;
                MessageBox.Show("Samples file not entered.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Weights == "")
            {
                button12.Enabled = true;
                MessageBox.Show("Please input existing weights for testing.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Error Windows when no radio button checked
            if (string.IsNullOrEmpty(textBox20.Text) || string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox7.Text))
            {
                button12.Enabled = true;
                MessageBox.Show("Check for empty parameters.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            int NumberofSamples = Convert.ToInt32(textBox8.Text);

            // convert string array to int
            string[] a = textBox20.Text.Split(',', '.');
            int[] networkSize = new int[4];
            for (int i = 0; i < a.Length; i++)
            {
                networkSize[i] = Convert.ToInt32(a[i]);
            }

            // determine number of samples
            int[] inputsPerSample = new int[a.Length];
            inputsPerSample[0] = networkSize[a.Length - 1] + Convert.ToInt32(textBox21.Text);
            for (int i = 1; i < a.Length; i++)
                inputsPerSample[i] = networkSize[0] + Convert.ToInt32(textBox21.Text);
            // end for

            int NumberofSectors = Convert.ToInt32(textBox7.Text);

            this.button12.Enabled = false;
            this.timer16.Enabled = true;

            int[,] output = await Task.Run(() => mlmvn.TEST(Samples, NumberofSamples, Weights, 4, networkSize, inputsPerSample, NumberofSectors));

            this.button12.Enabled = true;
        }

        // Load Samples
        private void button13_Click(object sender, EventArgs e)
        {
            if (openFileDialog6.ShowDialog() == DialogResult.OK) // Test result.
                textBox2.Text = openFileDialog6.FileName;
        }

        // Load Weights
        private void button19_Click(object sender, EventArgs e)
        {
            if (openFileDialog4.ShowDialog() == DialogResult.OK) // Test result.
                textBox6.Text = openFileDialog4.FileName;
        }

        // Easter egg =P
        private void label22_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("TextLenaHalf.tif");
        }

        // Pause Button - Indeed!
        public void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked == true)
            {
                this.Text = Title + " (Paused)";
                SetText1("\r\nProcess is paused." + Environment.NewLine);
                this.checkBox2.Text = "Resume";
                NativeMethods.SetState(progressBar1, 3);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error); // Fix to Windows 7 Progressbar bug
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                button11.Enabled = false;
                pTokenSource1.IsPaused = !pTokenSource1.IsPaused;
            }
            if (this.checkBox2.Checked == false)
            {
                this.Text = Title + " (Working)";
                SetText1("Process is resumed." + Environment.NewLine);
                this.checkBox2.Text = "Pause";
                NativeMethods.SetState(progressBar1, 1);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
                button11.Enabled = true;
                pTokenSource1.IsPaused = !pTokenSource1.IsPaused;
            }
        }

        //private void checkBox4_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.checkBox4.Checked == true)
        //    {
        //        textBox18.Size = new Size(365, 20);
        //        textBox18.Location = new Point(88, 26);
        //        label29.Text = "Color Image:";
        //        toolTip1.SetToolTip(label29, "The color RGB Tiff image to be corrupted by Gaussian noise.\r\nDrag and drop function is supported.");
        //        checkBox3.Text = "Add Gaussian noise to color image";
        //        toolTip1.SetToolTip(button18, "Load the color image.");
        //    }
        //    if (this.checkBox4.Checked == false)
        //    {
        //        textBox18.Size = new Size(342, 20);
        //        textBox18.Location = new Point(111, 26);
        //        label29.Text = "Grayscale Image:";
        //        toolTip1.SetToolTip(label29, "The grayscale Y Tiff image to be corrupted by Gaussian noise.\r\nDrag and drop function is supported.");
        //        checkBox3.Text = "Add Gaussian noise to grayscale image";
        //        toolTip1.SetToolTip(button18, "Load the grayscale image.");
        //    }
        //}

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox5.Checked == true)
            {
                this.timer12.Enabled = true;
            }
            if (this.checkBox5.Checked == false)
            {
                timer1.Enabled = true;
            }
        }

        // Loading the parameters using an xml file
        private void button17_Click(object sender, EventArgs e)
        {
            // string textbox13, string textbox16, string textbox17, int combobox4)
            if (openFileDialog5.ShowDialog() == DialogResult.OK) // Load image parameters
            {
                xmlImageParams(openFileDialog5.FileName);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) // Test result.
                openFileDialog1.Filter = "TIFF Image (*.tif;*.tiff)|*.tif;.tiff|All files (*.*)|*.*";
            textBox18.Text = openFileDialog1.FileName;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox6.Checked == true)
            {
                this.Text = Title + " (Paused)";
                SetText2("\r\nProcess is paused." + Environment.NewLine);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                this.checkBox6.Text = "Resume";
                button15.Enabled = false;
                pTokenSource2.IsPaused = !pTokenSource2.IsPaused;
            }
            if (this.checkBox6.Checked == false)
            {
                this.Text = Title + " (Working)";
                SetText2("Process is resumed." + Environment.NewLine);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
                this.checkBox6.Text = "Pause";
                button15.Enabled = true;
                pTokenSource2.IsPaused = !pTokenSource2.IsPaused;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (cTokenSource2 != null)
            {
                cTokenSource2.Cancel();
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (openFileDialog5.ShowDialog() == DialogResult.OK) // Load image parameters
            {
                xmlWeightParams(openFileDialog5.FileName);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            saveFileDialog3.FileName = "Weight_Parameters.xml";

            if (saveFileDialog3.ShowDialog() == DialogResult.OK)
            {
                // defines the xml settings for the file
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true; // allows indentation

                // xml name based on file dialogbox name
                XmlWriter parameters = XmlWriter.Create(saveFileDialog3.FileName, settings);

                // start the generation of the xml parameters
                parameters.WriteStartDocument();

                // xml parameters
                parameters.WriteStartElement("Method");
                parameters.WriteAttributeString("type", "Weight_Parameters");
                parameters.WriteStartElement("Parameters");
                parameters.WriteElementString("Size_Of_Network", textBox20.Text);
                parameters.WriteElementString("Output", Convert.ToString(comboBox2.SelectedIndex));
                parameters.WriteElementString("Output_Neurons", textBox21.Text);
                parameters.WriteElementString("Samples_in_Learning", textBox8.Text);
                parameters.WriteElementString("Number_Of_Sectors", textBox7.Text);
                parameters.WriteElementString("Stopping_Criteria", Convert.ToString(comboBox3.SelectedIndex));
                parameters.WriteElementString("Angular_RMSE", Convert.ToString(checkBox1.Checked));
                parameters.WriteElementString("Local_Threshold", textBox5.Text);
                parameters.WriteElementString("Global_Threshold", textBox4.Text);

                // proper closure and disposing of the file and memory
                parameters.Flush();
                parameters.Close();
                parameters.Dispose();
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            if (radioButton3.Checked) // Save pixel parameters
            {
                saveFileDialog3.FileName = "Pixel_Parameters.xml";

                if (saveFileDialog3.ShowDialog() == DialogResult.OK)
                {
                    // defines the xml settings for the file
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true; // allows indentation

                    // xml name based on file dialogbox name
                    XmlWriter parameters = XmlWriter.Create(saveFileDialog3.FileName, settings);

                    // start the generation of the xml parameters
                    parameters.WriteStartDocument();

                    // xml parameters
                    parameters.WriteStartElement("Method");
                    parameters.WriteAttributeString("type", "Pixel_Parameters");
                    parameters.WriteStartElement("Parameters");
                    parameters.WriteElementString("Number_of_Sectors", textBox13.Text);
                    parameters.WriteElementString("Input_Layer_Size", textBox16.Text);
                    parameters.WriteElementString("Hidden_Layer_Size", textBox17.Text);
                    parameters.WriteElementString("Kernel", Convert.ToString(comboBox4.SelectedIndex));

                    // proper closure and disposing of the file and memory
                    parameters.Flush();
                    parameters.Close();
                    parameters.Dispose();
                }
            }
            else if (radioButton4.Checked) // Save patch parameters
            {
                saveFileDialog3.FileName = "Patch_Parameters.xml";

                if (saveFileDialog3.ShowDialog() == DialogResult.OK)
                {
                    // defines the xml settings for the file
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true; // allows indentation

                    // xml name based on file dialogbox name
                    XmlWriter parameters = XmlWriter.Create(saveFileDialog3.FileName, settings);

                    // start the generation of the xml parameters
                    parameters.WriteStartDocument();

                    // xml parameters
                    parameters.WriteStartElement("Method");
                    parameters.WriteAttributeString("type", "Patch_Parameters");
                    parameters.WriteStartElement("Parameters");
                    parameters.WriteElementString("Patch_Method", Convert.ToString(comboBox4.SelectedIndex));
                    parameters.WriteElementString("Number_of_Sectors", textBox13.Text);
                    parameters.WriteElementString("Step", textBox16.Text);
                    parameters.WriteElementString("Network_Size", textBox17.Text);
                    parameters.WriteElementString("Output_Neurons", textBox19.Text);
                    

                    // proper closure and disposing of the file and memory
                    parameters.Flush();
                    parameters.Close();
                    parameters.Dispose();
                }
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            string[] a = textBox20.Text.Split(',');
            saveFileDialog3.FileName = "Patch_Parameters.xml";

            if (saveFileDialog3.ShowDialog() == DialogResult.OK)
            {
                // defines the xml settings for the file
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true; // allows indentation

                // xml name based on file dialogbox name
                XmlWriter parameters = XmlWriter.Create(saveFileDialog3.FileName, settings);

                // start the generation of the xml parameters
                parameters.WriteStartDocument();

                // xml parameters
                parameters.WriteStartElement("Method");
                parameters.WriteAttributeString("type", "Patch_Parameters");
                parameters.WriteStartElement("Parameters");
                parameters.WriteElementString("Patch_Method", Convert.ToString(comboBox4.SelectedIndex));
                parameters.WriteElementString("Number_of_Sectors", textBox13.Text);
                parameters.WriteElementString("Step", textBox16.Text);
                parameters.WriteElementString("Network_Size", textBox17.Text);
                parameters.WriteElementString("Output_Neurons", textBox19.Text);

                // proper closure and disposing of the file and memory
                parameters.Flush();
                parameters.Close();
                parameters.Dispose();
            }
        }
        #endregion

        #region Control Features

        #region Drag and drop
        /* Consists of 2 parts:
         *      Entering the region within an object
         *      Dragging and dropping an element onto the object
         */

        private void textBox11_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox11_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox11.Text = fileName;
            }
        }

        private void textBox15_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox15_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox15.Text = fileName;
            }
        }

        private void textBox14_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox14_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox14.Text = fileName;
            }
        }

        private void textBox9_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox9_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox9.Text = fileName;
            }
        }

        private void textBox10_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox10_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox10.Text = fileName;
            }
        }

        private void textBox18_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }
        private void textBox18_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox18.Text = fileName;
            }
        }

        private void groupBox4_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void groupBox4_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                xmlImageParams(fileName);
            }
        }

        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox2.Text = fileName;
            }
        }

        private void textBox6_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void textBox6_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                textBox6.Text = fileName;
            }
        }

        private void groupBox6_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.Copy;

            else
                e.Effect = DragDropEffects.None;
        }

        private void groupBox6_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string fileName in files)
            {
                xmlWeightParams(fileName);
            }
        }
        #endregion

        #region Keypress Events

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox20_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar)
        && e.KeyChar != ',' && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox16_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox17_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (radioButton3.Checked) // Process using pixels
            {
                if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            else if (radioButton4.Checked) // Process using patches
            {
                if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar)
        && e.KeyChar != ',' && e.KeyChar != ' ')
                {
                    e.Handled = true;
                }
            }
        }

        private void comboBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void comboBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar)
        && e.KeyChar != 'x' && e.KeyChar != 'X'
        && e.KeyChar != ' ')
            {
                e.Handled = true;

            }
        }

        private void comboBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (radioButton3.Checked) // Process using pixels
            {
                if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar)
        && e.KeyChar != 'x' && e.KeyChar != 'X'
        && e.KeyChar != ' ')
                {
                    e.Handled = true;

                }
            }
        }

        private void comboBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar)
        && e.KeyChar != 'x' && e.KeyChar != 'X'
        && e.KeyChar != ' ')
            {
                e.Handled = true;

            }
        }

        #endregion

        #endregion

        #region Form Functions

        delegate void SetTextCallback(string text);

        // Textbox for tab 4
        public void SetText1(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox12.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText1);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox12.AppendText(text);
            }
        }

        // Textbox for tab 3
        public void SetText2(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.textBox12.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText2);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox3.AppendText(text);
            }
        }

        delegate int SetProgressCallback(int value);

        // Progressbar for tab 4
        public int SetProgress1(int value)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (progressBar1.InvokeRequired)
            {
                SetProgressCallback e = new SetProgressCallback(SetProgress1);
                this.Invoke(e, new object[] { value });
            }
            else
            {
                this.progressBar1.Value += value;
            }
            return progressBar1.Value;
        }

        // read parameters from xml file
        public void xmlImageParams(string FileName)
        {
            // Loading from a file
            XmlReader Xml = XmlReader.Create(FileName);

            while (Xml.Read())
            {
                if (Xml.NodeType == XmlNodeType.Element && Xml.Name == "Method")
                {
                    if (Xml.GetAttribute(0) == "Pixel_Parameters")
                    {
                        while (Xml.NodeType != XmlNodeType.EndElement)
                        {
                            Xml.Read();
                            if (Xml.Name == "Parameters")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.Name == "Number_of_Sectors")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox13.Text = Xml.Value; // Number of sectors
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Input_Layer_Size")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox16.Text = Xml.Value; // Input layer size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Hidden_Layer_Size")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox17.Text = Xml.Value; // Hidden layer size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Kernel")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                comboBox4.SelectedIndex = Convert.ToInt32(Xml.Value); // Kernel
                                            }
                                        }
                                        Xml.Read();
                                    }
                                }
                            }
                        }
                    }
                    else if (Xml.GetAttribute(0) == "Patch_Parameters")
                    {
                        while (Xml.NodeType != XmlNodeType.EndElement)
                        {
                            Xml.Read();
                            if (Xml.Name == "Parameters")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.Name == "Patch_Method")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                comboBox4.SelectedIndex = Convert.ToInt32(Xml.Value); // Patch method
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Number_of_Sectors")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox13.Text = Xml.Value; // Number of sectors
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Step")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox16.Text = Xml.Value; // Step
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Network_Size")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox17.Text = Xml.Value; // Network size
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Output_Neurons")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox19.Text = Xml.Value; // Output neurons
                                            }
                                        }
                                        Xml.Read();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // proper closure and disposing of the file and memory
            Xml.Close();
            Xml.Dispose();
        }

        public void xmlWeightParams(string FileName)
        {
            // Loading from a file
            XmlReader Xml = XmlReader.Create(FileName);

            while (Xml.Read())
            {
                if (Xml.NodeType == XmlNodeType.Element && Xml.Name == "Method")
                {
                    if (Xml.GetAttribute(0) == "Weight_Parameters")
                    {
                        while (Xml.NodeType != XmlNodeType.EndElement)
                        {
                            Xml.Read();
                            if (Xml.Name == "Parameters")
                            {
                                while (Xml.NodeType != XmlNodeType.EndElement)
                                {
                                    Xml.Read();
                                    if (Xml.Name == "Size_Of_Network")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox20.Text = Xml.Value; // Size of Network
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Output")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                comboBox2.SelectedIndex = Convert.ToInt32(Xml.Value); // Output
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Output_Neurons")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox21.Text = Xml.Value; // Output Neurons
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Samples_in_Learning")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox8.Text = Xml.Value; // Samples in Learning
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Number_Of_Sectors")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox7.Text = Xml.Value; // Number of Sectors
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Stopping_Criteria")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                comboBox3.SelectedIndex = Convert.ToInt32(Xml.Value); // Stopping Criteria
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Angular_RMSE")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                checkBox1.Checked = Convert.ToBoolean(Xml.Value); // Angular RMSE
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Local_Threshold")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox5.Text = Xml.Value; // Local Threshold
                                            }
                                        }
                                        Xml.Read();
                                    }
                                    if (Xml.Name == "Global_Threshold")
                                    {
                                        while (Xml.NodeType != XmlNodeType.EndElement)
                                        {
                                            Xml.Read();
                                            if (Xml.NodeType == XmlNodeType.Text)
                                            {
                                                textBox4.Text = Xml.Value; // Global Threshold
                                            }
                                        }
                                        Xml.Read();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // proper closure and disposing of the file and memory
            Xml.Close();
            Xml.Dispose();
        }
        #endregion

        private void textBox21_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox19_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)
        && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }

    public static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}

