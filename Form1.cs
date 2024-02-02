using NReco.VideoConverter;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
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
            stopwatch.Stop();

            System.Media.SystemSounds.Exclamation.Play();

            string outputPathResult = e.Result as string;

            button1.Enabled = true;
            button2.Enabled = true;
            checkBox1.Enabled = true;
            
            if(!outputDate) textBox1.Enabled = true;

            StringBuilder message = new StringBuilder();
            message.AppendLine($"Conversion Finished! Took: {stopwatch.Elapsed.Hours:D2}:{stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds:D2}");
            message.AppendLine($"Output Path: {outputPathResult}");

            DialogResult result = MessageBox.Show(message.ToString(), "Converter", MessageBoxButtons.OKCancel);
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

        private void OnConvert(object sender, DoWorkEventArgs e)
        {
            var converter = new FFMpegConverter();
            FileInfo fileInfo = new FileInfo(path);


            if (outputDate)
            {
                string formattedCreationDate = fileInfo.CreationTime.ToString("yyyy-MM-dd");
                outputFile = formattedCreationDate;
            }
            else
            {
                if (String.IsNullOrEmpty(textBox1.Text))
                {
                    outputFile = fileInfo.Name;
                }
                else outputFile = textBox1.Text;
            }

            if(!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            string outputPathToFile = $"{outputPath}\\{outputFile}.mp4";

            converter.ConvertMedia(path, outputPathToFile, "mp4");

            e.Result = outputPathToFile;
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
