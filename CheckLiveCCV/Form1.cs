using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace CheckLiveCCV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        public class Root
        {
            public int error { get; set; }
            public string msg { get; set; }
        }

        async void Request(string dt)
        {
            var options = new RestClientOptions()
            {
                MaxTimeout = -1,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36",
            };
            var client = new RestClient(options);
            var request = new RestRequest("https://uncoder.eu.org/cc-checker/api.php", Method.Post);
            request.AddHeader("accept", "*/*");
            request.AddHeader("accept-language", "en-US,en;q=0.9,vi;q=0.8,ar;q=0.7,de;q=0.6");
            request.AddHeader("content-type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("origin", "https://uncoder.eu.org");
            request.AddHeader("priority", "u=1, i");
            request.AddHeader("referer", "https://uncoder.eu.org/cc-checker/");
            request.AddHeader("sec-ch-ua", "\"Chromium\";v=\"124\", \"Google Chrome\";v=\"124\", \"Not-A.Brand\";v=\"99\"");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("sec-fetch-dest", "empty");
            request.AddHeader("sec-fetch-mode", "cors");
            request.AddHeader("sec-fetch-site", "same-origin");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            var body = Encode(dt);
            request.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            Root parsed_json = JsonConvert.DeserializeObject<Root>(response.Content);

            if (parsed_json != null)
            {
                string msg = parsed_json.msg;

                if (msg.Contains("Unknown"))
                {
                    try
                    {
                        txtUnknown.Text += dt + Environment.NewLine;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Lỗi!");
                    }
                }
                else if (msg.Contains("Live"))
                {
                    try
                    {
                        txtLive.Text += dt + Environment.NewLine;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Lỗi!");
                    }
                }
                else
                {
                    try
                    {
                        txtDie.Text += dt + Environment.NewLine;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Lỗi!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Lỗi kết nối đến server!");
            }

        }

        public string Encode(string card)
        {
            string encode_string = string.Empty;
            encode_string = HttpUtility.UrlEncode(card);
            return "data=" + encode_string;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Request(txtRaw.Text);
            var linesRead = File.ReadLines(txtFilePath.Text);
            foreach (var lineRead in linesRead)
            {
                Thread.Sleep(100);
                Request(lineRead);
            }
        }
        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtRaw.Text = File.ReadAllText(openFileDialog1.FileName);
                    txtFilePath.Text = openFileDialog1.FileName;
                    btnStart.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //btnStart.Enabled = false;
        }
    }
}


