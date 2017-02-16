using System;
using System.ComponentModel;
using System.Windows.Forms;
using Library;

namespace UserInterface
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DisplayControlText();
            MaximizeBox = MinimizeBox = false;
        }

        private void DisplayControlText()
        {
            Text = @"Source Files Distributor";
            txtDestFolderName.Text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                +
                @"\"
                +
                Guid.NewGuid().ToString("N")
                +
                @"_"
                +
                Guid.NewGuid().ToString("N")
                + @"\";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var fileExtension = String.Format("*.{0}", txtFileExtension.Text);
            var sourceLocation = txtSourceLocationInfo.Text;

            Service service = new Service(fileExtension, sourceLocation, txtDestFolderName.Text);
            backgroundWorker.RunWorkerAsync(service);
            btnStart.Enabled = backgroundWorker.IsBusy == false;
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
        }

        private void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            throw new NotImplementedException();
        }

        // runs on background thread
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var argument = e.Argument as Service;

            if (argument == null)
                throw new NullReferenceException(String.Format("Object of type {0} not passed to background worker thread",
                    typeof(Service)));

            var elapsedTime = argument.Execute();
            e.Result = elapsedTime;
        }

        // runs on UI thread
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(String.Format("Total time taken to distribute files: {0} milliseconds", e.Result));
            btnStart.Enabled = backgroundWorker.IsBusy == false;
        }
    }
}
