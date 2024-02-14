using NReco.VideoConverter;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace AVI_to_MP4_Converter
{
    public partial class Form1 : Form
    {
        private string path = "";
        private bool FilenameAsDate = false;
        private string ROOT_DIRECTORY = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)}\\Converter Output";
        private Stopwatch stopwatch;
        private string OutputPath;

        public enum SoundOptions
        {
            None,
            Exclamation,
            Asterisk,
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (FileExists())
            {
                DialogResult result = RaiseDialogBox("The output file already exist", icon: MessageBoxIcon.Warning);
                if (result != DialogResult.OK) return;
            }

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

        private bool FileExists()
        {
            OutputPath = CreateOutputPath();

            return File.Exists(OutputPath);
        }

        private void ConversionCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            stopwatch.Stop();

            button1.Enabled = true;
            button2.Enabled = true;
            checkBox1.Enabled = true;
            if(!FilenameAsDate) textBox1.Enabled = true;

            StringBuilder message = new StringBuilder();
            message.AppendLine($"Conversion Finished! Took: {stopwatch.Elapsed.Hours:D2}:{stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds:D2}");
            message.AppendLine($"Output Path: {OutputPath}");

            RaiseDialogBox(message.ToString());
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

        private string CreateOutputPath()
        {
            FileInfo fileInfo = new FileInfo(path);
            string outputFile;

            if (FilenameAsDate)
            {
                outputFile = fileInfo.CreationTime.ToString("yyyy-MM-dd");
            }
            else
            {
                if (String.IsNullOrEmpty(textBox1.Text))
                {
                    outputFile = Path.ChangeExtension(fileInfo.Name, null);
                }
                else outputFile = textBox1.Text;
            }

            return $"{ROOT_DIRECTORY}\\{outputFile}.mp4";
        }

        private void OnConvert(object sender, DoWorkEventArgs e)
        {
            var converter = new FFMpegConverter();

            if (!Directory.Exists(ROOT_DIRECTORY))
            {
                Directory.CreateDirectory(ROOT_DIRECTORY);
            }

            converter.ConvertMedia(path, OutputPath, "mp4");
        }

        private void OnCheckboxValueChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            if (!checkbox.Checked)
            {
                this.textBox1.Enabled = true;
            }
            else this.textBox1.Enabled = false;

            FilenameAsDate = checkbox.Checked;
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

        private DialogResult RaiseDialogBox(string message,
            SoundOptions sound = SoundOptions.None,
            MessageBoxButtons btns = MessageBoxButtons.OKCancel,
            string Title = "Converter", 
            MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            switch (sound)
            {
                case SoundOptions.Exclamation:
                    SystemSounds.Exclamation.Play(); 
                    break;
                case SoundOptions.Asterisk: 
                    SystemSounds.Asterisk.Play();
                    break;
                default:
                    SystemSounds.Exclamation.Play();
                    break;
            }


            DialogResult result = MessageBox.Show(message, Title, btns, icon);
            
            return result;
        }
    }
}
