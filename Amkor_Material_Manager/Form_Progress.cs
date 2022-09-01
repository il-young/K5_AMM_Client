using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Amkor_Material_Manager
{
    public partial class Form_Progress : Form
    {
        public int nChangeIndex = 0;
        public bool bState = false;

        System.Diagnostics.Stopwatch sw_ShowCheck = new System.Diagnostics.Stopwatch();

        public Form_Progress()
        {
            InitializeComponent();
        }

        public void Progress_Exit()
        {
            timer1.Stop();
            timer1.Enabled = false;
            if (sw_ShowCheck.IsRunning == true)
            {
                sw_ShowCheck.Stop();
                sw_ShowCheck.Reset();
            }

            bState = false;
            Dispose();
        }

        public void Form_Show(string strMsg, int nType)
        {
            try
            {
                Form_ChangeBackground(nType);
                label1.Text = strMsg;

                if (nType == 0)
                {
                    bState = true;
                    Show();
                    sw_ShowCheck.Start();
                    timer1.Start();
                    timer1.Enabled = true;
                }
                else if (nType == 1004)
                {
                    bState = true;
                }
                else if (nType == 1005)
                {
                    bState = true;
                    Show();
                }
                else if(nType == 1006)
                {
                    bState = true;
                    ShowDialog();
                }
                else
                {
                    bState = true;
                    Show();
                    sw_ShowCheck.Start();
                    timer1.Start();
                    timer1.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }

        }

        public void Form_Hide()
        {
            bState = false;
            Hide();
        }

        public void Form_ChangeBackground(int n)
        {
            if (n == 0)
            {
                label1.BackColor = System.Drawing.Color.DarkGreen;
                nChangeIndex = 1;
            }
            else if(n > 0 && n < 1000)
            {
                label1.BackColor = System.Drawing.Color.RoyalBlue;
                nChangeIndex = 0;
            }
            else if(n == 1005)
            {
                label1.BackColor = System.Drawing.Color.DarkGreen;
                nChangeIndex = 0;
            }
            else
            {
                label1.BackColor = System.Drawing.Color.Red;
                nChangeIndex = 0;
            }
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            bState = false;
            timer1.Stop();
            timer1.Enabled = false;
            if (sw_ShowCheck.IsRunning == true)
            {
                sw_ShowCheck.Stop();
                sw_ShowCheck.Reset();
            }
            //this.Close();
            Hide();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (sw_ShowCheck.ElapsedMilliseconds > (long)6000) //7s
                {
                    bState = false;
                    timer1.Stop();
                    timer1.Enabled = false;
                    if (sw_ShowCheck.IsRunning == true)
                    {
                        sw_ShowCheck.Stop();
                        sw_ShowCheck.Reset();
                    }

                    //this.Close();
                    Hide();
                }
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
        }
    }
}
