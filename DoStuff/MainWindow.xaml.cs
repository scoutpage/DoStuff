using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoStuff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker bg;
        int iChosenFunction;
        string OutFileName;

        string TempStr1;
        string TempStr2;
        string TempStr3;

        int TempInt1;
        public MainWindow()
        {
            InitializeComponent();
            bg = new BackgroundWorker();
            bg.DoWork += Bg_DoWork;
            bg.ProgressChanged += Bg_ProgressChanged;
            bg.RunWorkerCompleted += Bg_RunWorkerCompleted;
            bg.WorkerSupportsCancellation = true;
            bg.WorkerReportsProgress = true;

            //File the comboboxes
            cbbChoiceL0.Items.Add("ProgressBar Test");
            cbbChoiceL0.Items.Add("Write file");
            cbbChoiceL0.Items.Add("Lock File");
            cbbChoiceL0.Items.Add("Update LastWriteTime & Lock File");
            cbbChoiceL0.Items.Add("Update LastWriteTime but do NOT Lock File");
            iChosenFunction = -1;

            OutFileName = "MyFile.txt";
            SetFormControls(0);
            btnStop.IsEnabled = false;

            txtTempText1.Text = @"j:\Docs\VSPrj\DoStuff\DoStuff\DoStuff\bin\OutputFile.txt";
            txtTempText2.Text = @"j:\Docs\VSPrj\DoStuff\DoStuff\DoStuff\bin\InputFile.txt";
        }

        private void Bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //MessageBox.Show("Task completed");
            SetFormControls(iChosenFunction);
        }

        private void Bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressbar.Value += 1;
            progressbar.Value %= 10;
            //label.Content = e.ProgressPercentage;
        }

        private void Bg_DoWork(object sender, DoWorkEventArgs e)
        {
            //Progressbar test
            if (iChosenFunction == 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    Thread.Sleep(1000); //do some task
                    bg.ReportProgress(0);

                    if (bg.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            //Write file
            if (iChosenFunction == 1)
            {
                Random tDelay = new Random();
                int DelayCnt = TempInt1 / 100;

                //Open the input file: #2
                var fReadFrom = new FileStream(TempStr2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var srReadFrom = new StreamReader(fReadFrom);
                //Open the output file: #1
                //var fWriteTo = new FileStream(TempStr1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                while (true)
                {
                    if (bg.CancellationPending == true)
                    {
                        e.Cancel = true;
                        srReadFrom.Close();
                        return;
                    }
                    Debug.WriteLine("dafad: " + DelayCnt.ToString());
                    //Do the write file
                    if(DelayCnt < 1)
                    {
                        DelayCnt = TempInt1 / 100;

                        //Read next input line
                        string tStr;
                        tStr = srReadFrom.ReadLine();
                        if(tStr == null)
                        {
                            fReadFrom.Position = 0;
                            srReadFrom.DiscardBufferedData();
                            tStr = srReadFrom.ReadLine();
                        }
                        Debug.WriteLine(tStr);

                        using (StreamWriter sw = File.AppendText(TempStr1))
                        {
                            sw.WriteLine(tStr);
                            sw.Close();
                        }
                        bg.ReportProgress(0);
                    }
                    else DelayCnt--;
                    Thread.Sleep(100);
                }
            }
            //Lock File
            if (iChosenFunction == 2)
            {

                FileStream fs = null;
                fs = new FileStream(TempStr1, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                while (true)
                {
                    if (bg.CancellationPending == true)
                    {
                        e.Cancel = true;
                        fs.Close();
                        return;
                    }
                    Thread.Sleep(100);
                }
            }
            //Update LastWriteTime & Lock File 
            if (iChosenFunction == 3)
            {
                DateTime tDate = new DateTime();
                tDate = DateTime.Now;
                File.SetLastWriteTime(TempStr1, tDate);

                FileStream fs = null;
                fs = new FileStream(TempStr1, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                while (true)
                {
                    if (bg.CancellationPending == true)
                    {
                        e.Cancel = true;
                        fs.Close();
                        return;
                    }
                    Thread.Sleep(100);
                }
            }
            //Update LastWriteTime but do NOT Lock File
            if (iChosenFunction == 4)
            {
                DateTime tDate = new DateTime();
                tDate = DateTime.Now;
                File.SetLastWriteTime(TempStr1, tDate);

                e.Cancel = true;
                Thread.Sleep(250);
                return;
            }
        }

        void SetFormControls(int SetStatus)
        {
            //SetStatus:
            //  cbbChoiceL0: Set as if BGWorker is NOT running
            //  cbbChoiceL0+100: Set as if BGWorker IS running

            //cbbChoiceL0.Items.Add("ProgressBar Test");
            if (SetStatus == 0)
            {
                btnStart.IsEnabled = true; btnStart.Content = "Do Stuff";
                btnStop.IsEnabled = false;

                lblTemp1.IsEnabled = false; txtTempText1.IsEnabled = false;
                lblTemp2.IsEnabled = false; txtTempText2.IsEnabled = false;
                lblTemp3.IsEnabled = false; txtTempText3.IsEnabled = false;

                progressbar.IsEnabled = true;
                progressbar.Value = 0;
                progressbar.Maximum = 10;

                btnTest.IsEnabled = false;
                txtStatus.IsEnabled = true; txtStatus.Text = "Idle";

                cbbChoiceL0.IsEnabled = true;
                cbbChoiceL1.IsEnabled = false;
            }
            //cbbChoiceL0.Items.Add("ProgressBar Test") - Running
            if (SetStatus == 100)
            {
                btnStart.IsEnabled = false; btnStart.Content = "Running";
                btnStop.IsEnabled = true;

                lblTemp1.IsEnabled = false; txtTempText1.IsEnabled = false;
                lblTemp2.IsEnabled = false; txtTempText2.IsEnabled = false;
                lblTemp3.IsEnabled = false; txtTempText3.IsEnabled = false;

                progressbar.IsEnabled = true;

                btnTest.IsEnabled = false;
                txtStatus.IsEnabled = true; txtStatus.Text = "Progressbar increasing";

                cbbChoiceL0.IsEnabled = false;
                cbbChoiceL1.IsEnabled = false;
            }
            //cbbChoiceL0.Items.Add("Write file");
            if (SetStatus == 1)
            {
                btnStart.IsEnabled = true; btnStart.Content = "Do Stuff";
                btnStop.IsEnabled = false;

                lblTemp1.IsEnabled = true; txtTempText1.IsEnabled = true;
                lblTemp1.Content = "File to write to:";
                lblTemp2.IsEnabled = true; txtTempText2.IsEnabled = true;
                lblTemp2.Content = "File to read from:";
                lblTemp3.IsEnabled = false; txtTempText3.IsEnabled = false;

                progressbar.IsEnabled = true;
                progressbar.Value = 0;
                progressbar.Maximum = 10;

                btnTest.IsEnabled = false;
                txtStatus.IsEnabled = true; txtStatus.Text = "Idle";

                cbbChoiceL0.IsEnabled = true;
                cbbChoiceL1.IsEnabled = true;

                cbbChoiceL1.Items.Clear();
                cbbChoiceL1.Items.Add("100");
                cbbChoiceL1.Items.Add("200");
                cbbChoiceL1.Items.Add("250");
                cbbChoiceL1.Items.Add("500");
                cbbChoiceL1.Items.Add("1000");
                cbbChoiceL1.Items.Add("1500");
                cbbChoiceL1.Items.Add("2000");
                cbbChoiceL1.Items.Add("2500");
                cbbChoiceL1.Items.Add("3000");
                cbbChoiceL1.Items.Add("4000");
                cbbChoiceL1.Items.Add("5000");
                cbbChoiceL1.Items.Add("6000");
                cbbChoiceL1.Items.Add("7000");
                cbbChoiceL1.Items.Add("8000");
                cbbChoiceL1.Items.Add("9000");
                cbbChoiceL1.Items.Add("10000");
                cbbChoiceL1.Items.Add("15000");
                cbbChoiceL1.Items.Add("20000");
                cbbChoiceL1.Items.Add("30000");
                cbbChoiceL1.Items.Add("40000");
            }
            //cbbChoiceL0.Items.Add("Write file") - Running
            if (SetStatus == 101)
            {
                btnStart.IsEnabled = false; btnStart.Content = "Running";
                btnStop.IsEnabled = true;

                lblTemp1.IsEnabled = false; txtTempText1.IsEnabled = false;
                lblTemp2.IsEnabled = false; txtTempText2.IsEnabled = false;
                lblTemp3.IsEnabled = false; txtTempText3.IsEnabled = false;

                progressbar.IsEnabled = false;

                btnTest.IsEnabled = false;
                txtStatus.IsEnabled = true; txtStatus.Text = "Writing to file";

                cbbChoiceL0.IsEnabled = false;
                cbbChoiceL1.IsEnabled = false;
            }
            //cbbChoiceL0.Items.Add("Lock File");
            if (SetStatus == 2)
            {
                btnStart.IsEnabled = true; btnStart.Content = "Do Stuff";
                btnStop.IsEnabled = false;

                lblTemp1.IsEnabled = true; txtTempText1.IsEnabled = true;
                lblTemp1.Content = "File to lock:";
                txtTempText1.Text = @"c:\ProgramData\Tecan\Setup and Service\8.0\Batchfiles\EMCTest.txt";
                lblTemp2.IsEnabled = false; txtTempText2.IsEnabled = false;
                lblTemp3.IsEnabled = false; txtTempText3.IsEnabled = false;

                progressbar.IsEnabled = false;

                btnTest.IsEnabled = false;
                txtStatus.IsEnabled = true; txtStatus.Text = "Idle";

                cbbChoiceL0.IsEnabled = true;
                cbbChoiceL1.IsEnabled = false;
                cbbChoiceL1.Items.Clear();
            }
            //cbbChoiceL0.Items.Add("Lock File") - Running
            if (SetStatus == 201)
            {
                btnStart.IsEnabled = false; btnStart.Content = "Running";
                btnStop.IsEnabled = true;

                lblTemp1.IsEnabled = false; txtTempText1.IsEnabled = false;
                lblTemp2.IsEnabled = false; txtTempText2.IsEnabled = false;
                lblTemp3.IsEnabled = false; txtTempText3.IsEnabled = false;

                progressbar.IsEnabled = false;

                btnTest.IsEnabled = false;
                txtStatus.IsEnabled = true; txtStatus.Text = "Locking file";

                cbbChoiceL0.IsEnabled = false;
                cbbChoiceL1.IsEnabled = false;
            }
            //cbbChoiceL0.Items.Add("Update LastWriteTime & Lock File");
            if (SetStatus == 3)
            {
                btnStart.IsEnabled = true; btnStart.Content = "Do Stuff";
                btnStop.IsEnabled = false;

                lblTemp1.IsEnabled = true; txtTempText1.IsEnabled = true;
                lblTemp1.Content = "File to lock:";
                txtTempText1.Text = @"c:\ProgramData\Tecan\Setup and Service\8.0\Batchfiles\EMCTest.txt";
                lblTemp2.IsEnabled = false; txtTempText2.IsEnabled = false;
                lblTemp3.IsEnabled = false; txtTempText3.IsEnabled = false;

                progressbar.IsEnabled = false;

                btnTest.IsEnabled = false;
                txtStatus.IsEnabled = true; txtStatus.Text = "Idle";

                cbbChoiceL0.IsEnabled = true;
                cbbChoiceL1.IsEnabled = false;
                cbbChoiceL1.Items.Clear();
            }
            //cbbChoiceL0.Items.Add("Update LastWriteTime & Lock File") - Running
            if (SetStatus == 301)
            {
                btnStart.IsEnabled = false; btnStart.Content = "Running";
                btnStop.IsEnabled = true;

                lblTemp1.IsEnabled = false; txtTempText1.IsEnabled = false;
                lblTemp2.IsEnabled = false; txtTempText2.IsEnabled = false;
                lblTemp3.IsEnabled = false; txtTempText3.IsEnabled = false;

                progressbar.IsEnabled = false;

                btnTest.IsEnabled = false;
                txtStatus.IsEnabled = true; txtStatus.Text = "Locking file";

                cbbChoiceL0.IsEnabled = false;
                cbbChoiceL1.IsEnabled = false;
            }
            //cbbChoiceL0.Items.Add("Update LastWriteTime but do NOT Lock File");
            if (SetStatus == 4)
            {
                btnStart.IsEnabled = true; btnStart.Content = "Do Stuff";
                btnStop.IsEnabled = false;

                lblTemp1.IsEnabled = true; txtTempText1.IsEnabled = true;
                lblTemp1.Content = "File to lock:";
                txtTempText1.Text = @"c:\ProgramData\Tecan\Setup and Service\8.0\Batchfiles\EMCTest.txt";
                lblTemp2.IsEnabled = false; txtTempText2.IsEnabled = false;
                lblTemp3.IsEnabled = false; txtTempText3.IsEnabled = false;

                progressbar.IsEnabled = false;

                btnTest.IsEnabled = false;
                txtStatus.IsEnabled = true; txtStatus.Text = "Idle";

                cbbChoiceL0.IsEnabled = true;
                cbbChoiceL1.IsEnabled = false;
                cbbChoiceL1.Items.Clear();
            }
            //cbbChoiceL0.Items.Add("Update LastWriteTime but do NOT Lock File") - Running
            if (SetStatus == 401)
            {
                btnStart.IsEnabled = false; btnStart.Content = "Running";
                btnStop.IsEnabled = true;

                lblTemp1.IsEnabled = false; txtTempText1.IsEnabled = false;
                lblTemp2.IsEnabled = false; txtTempText2.IsEnabled = false;
                lblTemp3.IsEnabled = false; txtTempText3.IsEnabled = false;

                progressbar.IsEnabled = false;

                btnTest.IsEnabled = false;
                txtStatus.IsEnabled = true; txtStatus.Text = "Updating LastWriteDate";

                cbbChoiceL0.IsEnabled = false;
                cbbChoiceL1.IsEnabled = false;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (iChosenFunction == 0)
            {
                SetFormControls(100);
                bg.RunWorkerAsync();

            }
            if (iChosenFunction == 1)
            {
                TempStr1 = txtTempText1.Text; //
                TempStr1 = TempStr1.Replace("\"", ""); TempStr1 = TempStr1.Trim();
                TempStr2 = txtTempText2.Text;
                TempStr2 = TempStr2.Replace("\"", ""); TempStr2 = TempStr2.Trim();

                string tStr;
                if(cbbChoiceL1.SelectedIndex != -1)
                {
                    tStr = cbbChoiceL1.SelectedItem.ToString();
                }
                else
                {   txtStatus.Text = "Choose L1 options.";
                    return;
                }
                if (tStr.Trim() != "")
                {
                    try
                    {
                        TempInt1 = Convert.ToInt32(tStr.Trim());
                    }
                    catch (FormatException)
                    {   txtStatus.Text = "Wrong number format: " + tStr;
                        return;
                    }
                    catch (OverflowException)
                    {   txtStatus.Text = "Number too high: " + tStr;
                        return;
                    }
                }
                SetFormControls(101);
                bg.RunWorkerAsync();
            }
            if (iChosenFunction == 2)
            {
                TempStr1 = txtTempText1.Text; //
                TempStr1 = TempStr1.Replace("\"", ""); TempStr1 = TempStr1.Trim();

                SetFormControls(201);
                bg.RunWorkerAsync();
            }
            if (iChosenFunction == 3)
            {
                TempStr1 = txtTempText1.Text; //
                TempStr1 = TempStr1.Replace("\"", ""); TempStr1 = TempStr1.Trim();

                SetFormControls(301);
                bg.RunWorkerAsync();
            }
            if (iChosenFunction == 4)
            {
                TempStr1 = txtTempText1.Text; //
                TempStr1 = TempStr1.Replace("\"", ""); TempStr1 = TempStr1.Trim();

                SetFormControls(401);
                bg.RunWorkerAsync();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            SetFormControls(iChosenFunction);
            bg.CancelAsync();
        }

        private void cbbChoiceL0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            iChosenFunction = cbbChoiceL0.SelectedIndex;

            if (iChosenFunction == 0)
            {
                SetFormControls(iChosenFunction);
            }
            if (iChosenFunction == 1)
            {
                SetFormControls(iChosenFunction);
            }
            if (iChosenFunction == 2)
            {
                SetFormControls(iChosenFunction);
            }
            if (iChosenFunction == 3)
            {
                SetFormControls(iChosenFunction);
            }
            if (iChosenFunction == 4)
            {
                SetFormControls(iChosenFunction);
            }
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Consolesss");
            Debug.WriteLine("Debugsss");
            string path = @"j:\Docs\VSPrj\DoStuff\DoStuff\DoStuff\bin\InputFile.txt";
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                string tStr;
                tStr = sr.ReadToEnd();
                Debug.WriteLine(tStr);
                /*do
                {
                    tStr = sr.ReadLine();
                    if (tStr != null) Console.WriteLine(tStr);
                }
                while (tStr != null);*/
                sr.Close();
            }
        }
    }
}
