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
    public partial class Form_Timeset : Form
    {
        public string strTimeset_date_st = "", strTimeset_date_ed = "";
        public string strTimeset_hour_st = "", strTimeset_hour_ed = "";
        public string strTimeset_Min_st = "", strTimeset_Min_ed = "";

        public Form_Timeset()
        {
            InitializeComponent();
            Fnc_Init();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true) //Office
            {
                //08:30~-5:30
                DateTime dToday = DateTime.Now;
                dateTimePicker_st.Value = dToday.Date;
                dateTimePicker_ed.Value = dToday.Date;

                comboBox_Hour_st.SelectedIndex = 8;
                comboBox_Min_st.SelectedIndex = 6;

                comboBox_Hour_ed.SelectedIndex = 17;
                comboBox_Min_ed.SelectedIndex = 6;
            }
            else if (radioButton2.Checked == true) //Night
            {
                //22:00~-6:00
                DateTime dToday = DateTime.Now;
                dateTimePicker_st.Value = dToday.AddDays(-1).Date;
                dateTimePicker_ed.Value = dToday.Date;

                comboBox_Hour_st.SelectedIndex = 22;
                comboBox_Min_st.SelectedIndex = 0;

                comboBox_Hour_ed.SelectedIndex = 6;
                comboBox_Min_ed.SelectedIndex = 0;
            }
            else if (radioButton3.Checked == true) //Day
            {
                //06:00~14:00
                DateTime dToday = DateTime.Now;
                dateTimePicker_st.Value = dToday.Date;
                dateTimePicker_ed.Value = dToday.Date;

                comboBox_Hour_st.SelectedIndex = 6;
                comboBox_Min_st.SelectedIndex = 0;

                comboBox_Hour_ed.SelectedIndex = 14;
                comboBox_Min_ed.SelectedIndex = 0;
            }
            else if (radioButton4.Checked == true) //Swing
            {
                //14:00~22:00
                DateTime dToday = DateTime.Now;
                dateTimePicker_st.Value = dToday.Date;
                dateTimePicker_ed.Value = dToday.Date;

                comboBox_Hour_st.SelectedIndex = 14;
                comboBox_Min_st.SelectedIndex = 0;

                comboBox_Hour_ed.SelectedIndex = 22;
                comboBox_Min_ed.SelectedIndex = 0;
            }
            else if (radioButton5.Checked == true) //All day
            {
                //00:00~-23:59
                DateTime dToday = DateTime.Now;
                dateTimePicker_st.Value = dToday.Date;
                dateTimePicker_ed.Value = dToday.Date;

                comboBox_Hour_st.SelectedIndex = 0;
                comboBox_Min_st.SelectedIndex = 0;

                comboBox_Hour_ed.SelectedIndex = 23;
                comboBox_Min_ed.SelectedIndex = 12;
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            Fnc_UpdateInfo();

            if (AMM_Main.nSelectedWin == 2)
            {
                Form_ITS.strTimeset_date_st = strTimeset_date_st;
                Form_ITS.strTimeset_date_ed = strTimeset_date_ed;
                Form_ITS.strTimeset_hour_st = strTimeset_hour_st;
                Form_ITS.strTimeset_hour_ed = strTimeset_hour_ed;
                Form_ITS.strTimeset_Min_st = strTimeset_Min_st;
                Form_ITS.strTimeset_Min_ed = strTimeset_Min_ed;
                Form_ITS.bSearch_sid = checkBox_sid.Checked;
            }
            else if(AMM_Main.nSelectedWin == 3)
            {
                Form_History.strTimeset_date_st = strTimeset_date_st;
                Form_History.strTimeset_date_ed = strTimeset_date_ed;
                Form_History.strTimeset_hour_st = strTimeset_hour_st;
                Form_History.strTimeset_hour_ed = strTimeset_hour_ed;
                Form_History.strTimeset_Min_st = strTimeset_Min_st;
                Form_History.strTimeset_Min_ed = strTimeset_Min_ed;
            }

            Timeset_Exit();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Timeset_Exit();
        }

        public void Fnc_Init()
        {
            DateTime dToday = DateTime.Now;
            dateTimePicker_st.Value = dToday.Date;
            dateTimePicker_ed.Value = dToday.Date;

            comboBox_Hour_st.SelectedIndex = 0;
            comboBox_Min_st.SelectedIndex = 0;

            comboBox_Hour_ed.SelectedIndex = dToday.Hour;

            int nCal = (dToday.Minute + dToday.Minute % 5) / 5;
            if (nCal > 11)
                nCal = 11;

            comboBox_Min_ed.SelectedIndex = nCal; //0,5,10,15,20,25,30,35,40,45,50,55,59

            radioButton1.Checked = true;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;
            radioButton5.Checked = false;

            if (AMM_Main.nSelectedWin == 2)
            {
                checkBox_sid.Visible = true;
                checkBox_sid.Checked = false;
            }
            else
            {
                checkBox_sid.Visible = false;
            }
        }

        public void Timeset_Exit()
        {
            this.Dispose();
        }

        public void Fnc_UpdateInfo()
        {
            strTimeset_date_st = string.Format("{0}-{1:00}-{2:00}", dateTimePicker_st.Value.Year, dateTimePicker_st.Value.Month, dateTimePicker_st.Value.Day);
            strTimeset_date_ed = string.Format("{0}-{1:00}-{2:00}", dateTimePicker_ed.Value.Year, dateTimePicker_ed.Value.Month, dateTimePicker_ed.Value.Day);

            strTimeset_hour_st = comboBox_Hour_st.Text;
            strTimeset_hour_ed = comboBox_Hour_ed.Text;
            strTimeset_Min_st = comboBox_Min_st.Text;
            strTimeset_Min_ed = comboBox_Min_ed.Text;
        }
    }
}
