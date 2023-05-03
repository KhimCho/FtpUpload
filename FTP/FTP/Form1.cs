using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        class Ftp
        {
            public string Serv { get; set; }
            public string User { get; set; }
            public string Pass { get; set; }
            public string Filename { get; set; }
            public string FullName { get; set; }

        }

        Ftp input;

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string fileName = ((Ftp)e.Argument).Filename;
            string fULLName = ((Ftp)e.Argument).FullName;
            string Username = ((Ftp)e.Argument).User;
            string Password = ((Ftp)e.Argument).Pass;
            string Server  = ((Ftp)e.Argument).Serv;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", Server, fileName)));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(Username, Password);
            Stream FtpStream = request.GetRequestStream();
            FileStream fs = File.OpenRead(fULLName);
            byte[] buffer = new byte[1024];
            double total = (double)fs.Length;
            int byteRead = 0;
            double read = 0;
            do
            {
                if (!backgroundWorker.CancellationPending)
                {
                    byteRead = fs.Read(buffer, 0, 1024);
                    FtpStream.Write(buffer, 0, 1024);
                    read += (double)byteRead;
                    double percentage = read / total * 100;
                    backgroundWorker.ReportProgress((int)percentage);
                }
            }
            while (byteRead != 0);
            fs.Close();
            FtpStream.Close();

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            status.Text = $"Uploaded: {e.ProgressPercentage}";
            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Update();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            status.Text = "Upload completed!";
        }

        private void btnUpld_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect= false, ValidateNames = true, Filter= "All files|*.*"})
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fi = new FileInfo(ofd.FileName);
                    input.User = user.Text;
                    input.Pass = pass.Text;
                    input.Serv = serv.Text;
                    input.Filename = fi.Name;
                    input.FullName = fi.FullName;
                }
            }
        }
    }
}
