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
    public partial class Form_NumberPad : Form
    {
        public string strCount = "";
        public int nType = 0; 
        public Form_NumberPad()
        {
            InitializeComponent();
        }

        private void button_1_Click(object sender, EventArgs e)
        {
            strCount = strCount + "1";
            textBox_data.Text = strCount;
        }

        private void button_2_Click(object sender, EventArgs e)
        {
            strCount = strCount + "2";
            textBox_data.Text = strCount;
        }

        private void button_3_Click(object sender, EventArgs e)
        {
            strCount = strCount + "3";
            textBox_data.Text = strCount;
        }

        private void button_4_Click(object sender, EventArgs e)
        {
            strCount = strCount + "4";
            textBox_data.Text = strCount;
        }

        private void button_5_Click(object sender, EventArgs e)
        {
            strCount = strCount + "5";
            textBox_data.Text = strCount;
        }

        private void button_6_Click(object sender, EventArgs e)
        {
            strCount = strCount + "6";
            textBox_data.Text = strCount;
        }

        private void button_7_Click(object sender, EventArgs e)
        {
            strCount = strCount + "7";
            textBox_data.Text = strCount;
        }

        private void button_8_Click(object sender, EventArgs e)
        {
            strCount = strCount + "8";
            textBox_data.Text = strCount;
        }

        private void button_9_Click(object sender, EventArgs e)
        {
            strCount = strCount + "9";
            textBox_data.Text = strCount;
        }

        private void button_10_Click(object sender, EventArgs e)
        {
            strCount = strCount + "0";
            textBox_data.Text = strCount;
        }

        private void button_del_Click(object sender, EventArgs e)
        {
            if (strCount.Length < 1)
                return;

            strCount = strCount.Substring(0, strCount.Length - 1);
            textBox_data.Text = strCount;
        }

        private void button_complete_Click(object sender, EventArgs e)
        {
            if(nType == 1)
            {
                Form_Order.strPadSid = textBox_data.Text;
            }
            else if(nType == 2)
            {
                Form_Order.strPadReelSid = textBox_data.Text;
            }
            else if(nType == 3)
            {
                Form_Order.strPadReelqty = textBox_data.Text;
            }

            this.Dispose();
        }
    }
}
