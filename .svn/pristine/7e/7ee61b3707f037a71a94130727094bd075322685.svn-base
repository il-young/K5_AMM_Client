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
    public partial class Form_Login : Form
    {
        public Form_Login()
        {
            InitializeComponent();
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (textBox_id.Text == AMM_Main.strAdminID && textBox_pw.Text == AMM_Main.strAdminPW)
            {
                AMM_Main.bAdminLogin = true;
                LogIn_Exit();
            }
            else
            {
                AMM_Main.bAdminLogin = false;
                MessageBox.Show("ID 또는 비밀번호가 틀립니다. 다시 시도 하여 주십시오.");
                textBox_id.Text = "amkor";
                textBox_pw.Text = "";
                textBox_pw.Focus();
            }
        }

        public void LogIn_Exit()
        {
            this.Dispose();
        }

        public void LogIn_Init()
        {
            textBox_id.Text = "amkor";
            textBox_pw.Text = "";
            textBox_pw.Focus();
        }
    }
}
