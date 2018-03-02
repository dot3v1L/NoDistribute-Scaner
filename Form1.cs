using System;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NoDistribute_Scaner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tStatus.ForeColor = Color.Black;
            tError.ForeColor = Color.Black;
            using (var open = new OpenFileDialog())
            {
                open.Title = "Select file";
                open.Filter = ".exe|*.exe";

                if (open.ShowDialog() == DialogResult.OK)
                {
                    tFile.Text = open.FileName;
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.UploadProgressChanged += new UploadProgressChangedEventHandler(Upload);
                        webClient.UploadFileCompleted += new UploadFileCompletedEventHandler(Complete);
                        webClient.UploadFileAsync(new Uri("http://nodistribute.com/scan/index.php"), open.FileName);
                    }
                }
            }
        }

        private void Complete(object sender, UploadFileCompletedEventArgs e)
        {
            try
            {
                string str = Encoding.UTF8.GetString(e.Result);
                
                if (str.Contains("You have reached the scan limit, try again in a few hours or"))
                {
                    Text = "NoDistribute Scaner [Limit scan]";
                    tError.Text = "Error [Limit scan]";
                    tError.ForeColor = Color.Red;
                }

                string[] res = GetResult(str);
                if (res[0] != "")
                {
                    Text = String.Format("NoDistribute Scaner [{0}]", res[1]);
                    textBox2.Text = res[0];
                }
                
                tError.Text = String.Format("Error [{0}]", e.Error.Message);
            }
            catch (Exception){}
        }

        private void Upload(object sender, UploadProgressChangedEventArgs e)
        {
            try
            {
                tStatus.Text = String.Format("Status Uploading [{0}%]", e.ProgressPercentage);
                tProgres.Value = e.ProgressPercentage;
                tStatus.ForeColor = Color.Green;
            }
            catch (Exception){}
        }

        private string[] GetResult(string str)
        {
            string [] pattern = {"(?<=Verified By NoDistribute: ).*", "(?<=Detected by: ).*"};
            string [] result = new string[2];

            for (int i = 0; i < pattern.Length; i++)
            {
                Regex regex = new Regex(pattern[i]);
                Match match = regex.Match(str);
                result[i] = match.Value;
            }
            return result;
        }
    }
}
