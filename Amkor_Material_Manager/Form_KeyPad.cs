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
    public partial class Form_KeyPad : Form
    {
        string strCount = "";

        public Form_KeyPad()
        {
            InitializeComponent();
        }

        public void Fnc_Show(int n)
        {
            try
            {
                textBox_setcount.Text = n.ToString();
                ShowDialog();
            }
            catch
            {

            }
        }

        private void button_1_Click(object sender, EventArgs e)
        {
            strCount = strCount + "1";

            if (Int32.Parse(strCount) > 5)
                strCount = "5";

            textBox_setcount.Text = strCount;
        }

        private void button_2_Click(object sender, EventArgs e)
        {
            strCount = strCount + "2";

            if (Int32.Parse(strCount) > 5)
                strCount = "5";

            textBox_setcount.Text = strCount;
        }

        private void button_3_Click(object sender, EventArgs e)
        {
            strCount = strCount + "3";

            if (Int32.Parse(strCount) > 5)
                strCount = "5";

            textBox_setcount.Text = strCount;
        }

        private void button_4_Click(object sender, EventArgs e)
        {
            strCount = strCount + "4";

            if (Int32.Parse(strCount) > 5)
                strCount = "5";

            textBox_setcount.Text = strCount;
        }

        private void button_5_Click(object sender, EventArgs e)
        {
            strCount = strCount + "5";

            if (Int32.Parse(strCount) > 5)
                strCount = "5";

            textBox_setcount.Text = strCount;
        }

        private void button_6_Click(object sender, EventArgs e)
        {
            strCount = strCount + "6";

            if (Int32.Parse(strCount) > 5)
                strCount = "5";

            textBox_setcount.Text = strCount;
        }

        private void button_7_Click(object sender, EventArgs e)
        {
            strCount = strCount + "7";

            if (Int32.Parse(strCount) > 5)
                strCount = "5";

            textBox_setcount.Text = strCount;
        }

        private void button_8_Click(object sender, EventArgs e)
        {
            strCount = strCount + "8";

            if (Int32.Parse(strCount) > 5)
                strCount = "5";

            textBox_setcount.Text = strCount;
        }

        private void button_9_Click(object sender, EventArgs e)
        {
            strCount = strCount + "9";

            if (Int32.Parse(strCount) > 5)
                strCount = "5";

            textBox_setcount.Text = strCount;
        }

        private void button_10_Click(object sender, EventArgs e)
        {
            strCount = strCount + "0";

            if (Int32.Parse(strCount) > 5)
                strCount = "5";

            textBox_setcount.Text = strCount;
        }

        private void button_del_Click(object sender, EventArgs e)
        {
            if (strCount.Length < 1)
                return;

            strCount = strCount.Substring(0, strCount.Length - 1);
            textBox_setcount.Text = strCount;
        }

        private void button_apply_Click(object sender, EventArgs e)
        {
            Form_StripMark.nNewcount = Int32.Parse(textBox_setcount.Text);

            Fnc_Exit();
        }

        public void Fnc_Exit()
        {
            Hide();
            Dispose();
        }
    }
}
