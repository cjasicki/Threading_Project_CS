using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Example14_CS
{
    public partial class Form1 : Form
    {
        private int iSleepCounter = 1000;
        private int iThreadCounter = 0;
        private Thread[] myThread = new Thread[10];
        delegate void UpdateTextBoxDelegate(String value);

        public Form1()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtResult.Text = "";
            lblErrorMsg.Text = "";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            lblErrorMsg.Text = "";
            Int32 intSeed = 10;
            iThreadCounter += 1;
            if (txtSeed.Text.Trim().Length > 0)
            {
                intSeed = Convert.ToInt32(txtSeed.Text);
            }
            if (txtSleepTime.Text.Trim().Length > 0)
            {
                int iSleep = 1;
                try
                {
                    iSleep = Convert.ToInt32(txtSleepTime.Text);
                }
                catch (Exception ex)
                {
                    lblErrorMsg.Text = "Sleep time must be an Integer value." + ex.Message;
                }
                if (iSleep < 10)
                {
                    iSleepCounter = iSleep * 1000;
                }
                else
                {
                    lblErrorMsg.Text = "Sleep time must be an Integer value less than 10.";
                }

            }

            myThread[iThreadCounter] = new Thread(this.WorkerThread);
            myThread[iThreadCounter].IsBackground = true;
            txtResult.Text = txtResult.Text + Environment.NewLine + "Starting thread with seed: " + intSeed.ToString() + " - ThreadID: " + myThread[iThreadCounter].ManagedThreadId.ToString();
            myThread[iThreadCounter].Start(intSeed.ToString() + " : ThreadID - " + myThread[iThreadCounter].ManagedThreadId.ToString() + ":" + txtSleepTime.Text);

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            lblErrorMsg.Text = "";
            if (iThreadCounter > 0)
            {
                txtResult.Text = txtResult.Text + Environment.NewLine + "Stopping thread ID - " + myThread[iThreadCounter].ManagedThreadId.ToString();
            }
            else
            {
                txtResult.Text = txtResult.Text + Environment.NewLine + "No Thread to Stop!";
            }
            if (iThreadCounter > 0)
            {
                if (myThread[iThreadCounter] != null)
                {

                    myThread[iThreadCounter].Abort();
                    iThreadCounter -= 1;
                    if (iThreadCounter < 1)
                    {
                        txtResult.Text = txtResult.Text + Environment.NewLine + "All Threads Stopped!";
                    }
    
                }
            }

        }


        //**********************************************************************
        //** This worker thread will display a value (the seed) in a textbox,
        //** increment the value by 5, wait (default value 1000) milliseconds 
        //**  as set by value in textbox and repeats
        //**********************************************************************
        private void WorkerThread(object objStart)
        {
            Boolean blnErrOccurred = false;
            Int32 iSleepCount = 1000;//Default value 1 second.
            Int32 intShowValue;
            String[] strArgs = objStart.ToString().Split(':');
            intShowValue = Convert.ToInt32(strArgs[0].Trim());
            String strThreadID = strArgs[1];
            Int32 intTime = Convert.ToInt32(strArgs[2].Trim());
            while (true)
            {
                UpdateTextBox("Thread result: " + intShowValue.ToString() + strThreadID + " - Sleep time is " + intTime.ToString());
                intShowValue += 5;

                if (intTime > 0)
                {
                    try
                    {
                        //txtResult.Text = "This should be an error";
                        iSleepCount = intTime * 1000; //Using the TextBox directly makes this non-thread safe.
                    }
                    catch (Exception ex)
                    {
                        lblErrorMsg.Text = "Sleep time must be an Integer value." + ex.Message;
                        blnErrOccurred = true;
                    }
                }
                //if (txtSleepTime.Text.Length > 0)
                //{
                //    try
                //    {
                //        //txtResult.Text = "This should be an error";
                //        iSleepCount = Convert.ToInt32(txtSleepTime.Text) * 1000; //Using the TextBox directly makes this non-thread safe.
                //    }
                //    catch (Exception ex)
                //    {
                //        lblErrorMsg.Text = "Sleep time must be an Integer value." + ex.Message;
                //        blnErrOccurred = true;
                //    }
                //}

                if (blnErrOccurred)
                {
                    Thread.Sleep(2000);
                }
                else
                {
                    Thread.Sleep(iSleepCount);
                }
            }
        }

        //**********************************************************************
        //** This routine allows cross-thread access to the textbox. The textbox
        //** belongs to the main thread. SEE SAFE CROSS-THEADING on MSDN
        //**********************************************************************
        private void UpdateTextBox(String strMsg)
        {
            if (txtResult.InvokeRequired)
            {
                txtResult.Invoke(new UpdateTextBoxDelegate(UpdateTextBox), strMsg);//+ " : " + myThread.ManagedThreadId.ToString());
            }
            else
            {
                txtResult.Text = txtResult.Text + Environment.NewLine + strMsg;// + " : " + myThread.ManagedThreadId.ToString();
            }
        }


    }
}

