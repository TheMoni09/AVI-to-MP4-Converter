using NReco.VideoConverter;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AVI_to_MP4_Converter
{
    public partial class Form1 : Form
    {
        private string path = "";
        private bool outputDate = false;
        private string outputFile = "";
        private string outputPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)}\\Converter Output";
        private Stopwatch stopwatch;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Start a background thread for the conversion
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += OnConvert;
            worker.RunWorkerCompleted += ConversionCompleted;

            // Disable UI controls during conversion
            button1.Enabled = false;
            button2.Enabled = false;
            checkBox1.Enabled = false;
            textBox1.Enabled = false;

            stopwatch = Stopwatch.StartNew();

            worker.RunWorkerAsync();
        }

        private void ConversionCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Media.SystemSounds.Exclamation.Play();

            stopwatch.Stop();

            button1.Enabled = true;
            button2.Enabled = true;
            checkBox1.Enabled = true;
            textBox1.Enabled = true;

            MessageBox.Show($"Conversion Finished! Took: {stopwatch.Elapsed.Hours:D2}:{stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds:D2}", "Converter");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OnBrowseInput(sender, e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            OnCheckboxValueChanged(sender, e);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            OnUserOpenedInput(sender, e);
        }

        private void OnConvert(object sender, EventArgs e)
        {
            var converter = new FFMpegConverter();

            if (outputDate)
            {
                FileInfo fileInfo = new FileInfo(path);
                string formattedCreationDate = fileInfo.CreationTime.ToString("yyyy-MM-dd");
                outputFile = formattedCreationDate;
            }
            else
            {
                outputFile = this.textBox1.Text;
            }

            if(!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            converter.ConvertMedia(path, $"{outputPath}\\{outputFile}.mp4", "mp4");
        }

        private void OnCheckboxValueChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (checkbox.Checked)
            {
                this.textBox1.Enabled = true;
            }
            else this.textBox1.Enabled = false;

            outputDate = checkbox.Checked;
        }

        private void OnUserOpenedInput(object sender, CancelEventArgs e)
        {
            OpenFileDialog dialog = (OpenFileDialog)sender;
            path = dialog.FileName;
        }

        private void OnBrowseInput(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
        }
    }
}
