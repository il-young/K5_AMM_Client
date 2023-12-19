using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Amkor_Material_Manager
{
    public partial class Form_LongtimeReport : Form
    {
        public delegate void EvtMakeExcelReport(int month);
        public event EvtMakeExcelReport MakeExcelReportEvent;

        public delegate int EvtGetDataGridRowCount();
        public event EvtGetDataGridRowCount GetDataGridRowCountEvent;

        public delegate int EvtCehckExcelExportComp();
        public event EvtCehckExcelExportComp CehckExcelExportCompEvent;


        public static DialogResult InputBox(string title, string content, ref string value)
        {
            Form form = new Form();
            PictureBox picture = new PictureBox();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.ClientSize = new Size(300, 100);
            form.Controls.AddRange(new Control[] { label, picture, textBox, buttonOk, buttonCancel });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            form.Text = title;
            //picture.Image = Properties.Resources.Clogo;
            picture.SizeMode = PictureBoxSizeMode.StretchImage;
            label.Text = content;
            textBox.Text = value;
            buttonOk.Text = "확인";
            buttonCancel.Text = "취소";

            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            picture.SetBounds(10, 10, 50, 50);
            label.SetBounds(65, 17, 100, 20);
            textBox.SetBounds(65, 40, 220, 20);
            buttonOk.SetBounds(135, 70, 70, 20);
            buttonCancel.SetBounds(215, 70, 70, 20);

            DialogResult dialogResult = form.ShowDialog();

            value = textBox.Text;
            return dialogResult;
        }

        public Form_LongtimeReport()
        {
            InitializeComponent();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form_LongtimeReport_Load(object sender, EventArgs e)
        {
            nud_Mon.Value = Properties.Settings.Default.LongTimeReelReportMonth;

            tb_header.Text = Properties.Settings.Default.LongTimeReelReporthead;
            tb_tail.Text = Properties.Settings.Default.LongTimeReelReportTail;
            tb_hour.Text = Properties.Settings.Default.LongTimeReelReportHour.ToString("D2");
            tb_subject.Text = Properties.Settings.Default.LongTimeReelReportSubject; 

            cb_interval.SelectedIndex = Properties.Settings.Default.LongTimeReelReportInterval1;

            initInterval();

            cb_interval2.SelectedIndex = Properties.Settings.Default.LongTimeReelReportInterval2;

            clb_mail.CheckOnClick = true;

            if(Properties.Settings.Default.LongTermReelReportEN == true)
            {
                btn_mailEN.Text = "ENABLE";
            }
            else
            {
                btn_mailEN.Text = "DISABLE";
            }

            setMail();
        }

        private void initInterval()
        {
            cb_interval2.Items.Clear();

            if(cb_interval.SelectedIndex == 0) // 주
            {
                cb_interval2.Items.Add("일");
                cb_interval2.Items.Add("월");
                cb_interval2.Items.Add("화");
                cb_interval2.Items.Add("수");
                cb_interval2.Items.Add("목");
                cb_interval2.Items.Add("금");
                cb_interval2.Items.Add("토");                
            }
            else
            {
                for(int i = 0; i < 30; i++)
                {
                    cb_interval2.Items.Add(i + 1);
                }
            }

            cb_interval2.SelectedIndex = 0;
        }

        private void setMail()
        {
            string[] temp = Properties.Settings.Default.LongTimeReelReportMail.Split(';');

            for(int i = 0; i < temp.Length; i++)
            {
                if(temp[i] != "")
                    clb_mail.Items.Add(temp[i].Split(',')[1], bool.Parse(temp[i].Split(',')[0]));
            }
        }

        private void cb_interval_SelectedIndexChanged(object sender, EventArgs e)
        {
            initInterval();
        }

        private void nud_Mon_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LongTimeReelReportMonth = (int)nud_Mon.Value;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string val = "";
            if (DialogResult.OK == InputBox("메일 입력", "input mail", ref val))
            {
                clb_mail.Items.Add(val, true);
                clb_mail.SelectedIndex = clb_mail.Items.Count - 1;
            }
        }

        private void btn_mailDel_Click(object sender, EventArgs e)
        {
            if(clb_mail.SelectedIndex != -1)
            {
                clb_mail.Items.RemoveAt(clb_mail.SelectedIndex);
                clb_mail.SelectedIndex = clb_mail.Items.Count - 1;
            }
        }

        private void clb_mail_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cb_interval2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LongTimeReelReportMonth = (int)nud_Mon.Value;
            Properties.Settings.Default.LongTimeReelReportInterval1 = cb_interval.SelectedIndex;
            Properties.Settings.Default.LongTimeReelReportInterval2 = cb_interval2.SelectedIndex;
            Properties.Settings.Default.LongTimeReelReporthead = tb_header.Text;
            Properties.Settings.Default.LongTimeReelReportTail = tb_tail.Text;
            Properties.Settings.Default.LongTimeReelReportSubject = tb_subject.Text;

            Properties.Settings.Default.LongTimeReelReportHour = int.Parse(tb_hour.Text.Replace("시",""));

            Properties.Settings.Default.Save();
            mailSave();
        }

        private void mailSave()
        {
            string temp = "";

            for (int i = 0; i < clb_mail.Items.Count; i++)
            {
                temp += clb_mail.CheckedItems.Cast<string>().Where(m => m == clb_mail.Items[i].ToString()).ToList().Count == 0 ? "FALSE," : "TRUE,";
                temp += clb_mail.Items[i].ToString() + ";";
            }

            Properties.Settings.Default.LongTimeReelReportMail = temp;
            Properties.Settings.Default.Save();
        }

        private void btn_Test_Click(object sender, EventArgs e)
        {
            MakeExcelReportEvent(Properties.Settings.Default.LongTimeReelReportMonth-1);

            System.Threading.Thread.Sleep(1000);

            if(GetDataGridRowCountEvent() != 0)
            {
                SendLongTermReportMail();
            }
            else
            {
                MessageBox.Show("Data가 없습니다.");
            }

        }

        private void SendLongTermReportMail()
        {
            try
            {
                MailMessage message = new MailMessage();

                CultureInfo ciCurr = CultureInfo.CurrentCulture;
                int weekNum = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

                for (int i = 0; i < clb_mail.Items.Count; i++)
                {
                    message.To.Add(clb_mail.Items[i].ToString());
                }

                message.Subject = "[TEST]" + Properties.Settings.Default.LongTimeReelReportSubject.Replace("nn", weekNum.ToString("D2"));
                message.From = new System.Net.Mail.MailAddress("Amkor.Skynet@amkor.co.kr");
                message.Body = Properties.Settings.Default.LongTimeReelReporthead.Replace("nn", weekNum.ToString("D2")) + Environment.NewLine +                    
                    Properties.Settings.Default.LongTimeReelReportTail.Replace("nn", weekNum.ToString("D2"));

                if (Properties.Settings.Default.LongTermReelReportPath == "")
                {
                    Properties.Settings.Default.LongTermReelReportPath = System.Environment.CurrentDirectory + "\\LongTermReel";
                    Properties.Settings.Default.Save();
                }

                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Properties.Settings.Default.LongTermReelReportPath);
                System.IO.FileInfo[] fi = di.GetFiles();                

                Array.Sort<System.IO.FileInfo>(fi, delegate (System.IO.FileInfo x, System.IO.FileInfo y) { return x.CreationTime.CompareTo(y.CreationTime); });

                fi.Reverse();
                System.Net.Mail.Attachment attachment = new Attachment(fi[fi.Count() - 1].FullName);

                message.Attachments.Add(attachment);
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("10.101.10.6");
                smtp.Credentials = new System.Net.NetworkCredential("Amkor.Skynet@amkor.co.kr", "");
                smtp.Port = 25;

                smtp.Send(message);

                MessageBox.Show("테스트 메일이 전송 되었습니다.");
            }
            catch (Exception ex)
            {

            }
            
        }

        private void btn_mailEN_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.LongTermReelReportEN = !Properties.Settings.Default.LongTermReelReportEN;
            Properties.Settings.Default.Save();

            btn_mailEN.Text = Properties.Settings.Default.LongTermReelReportEN == true ? "ENABLE" : "DISABLE";
        }
    }
}

