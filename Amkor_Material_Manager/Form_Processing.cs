using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Amkor_Material_Manager
{
    public partial class Form_Processing : Form
    {
        public int nProgress = 0;
        public bool btime = false;

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr windowHandle);

        public Form_Processing()
        {
            InitializeComponent();
            SetForegroundWindow(Handle);
            ProgressInit();
        }
        public void Progress_Exit()
        {
            btime = false;
            timer1.Stop();
            //this.Hide();
            this.Dispose();
        }

        public void ProgressInit()
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Maximum = 100;
            progressBar1.Step = 10;
            progressBar1.Value = 0;

            timer1.Start();
            btime = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Increment(5);
            }
            else
                progressBar1.Value = 0;

            if (!Form_ITS.IsDateGathering && AMM_Main.nSelectedWin == 2)
                Progress_Exit();

            if (!Form_History.IsDateGathering && AMM_Main.nSelectedWin == 3)
                Progress_Exit();

            if(AMM_Main.nSelectedWin != 2 && AMM_Main.nSelectedWin != 3)
                Progress_Exit();
        }
    }
}
