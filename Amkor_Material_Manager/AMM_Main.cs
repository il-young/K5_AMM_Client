using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

using AMM;
using System.IO;
using System.Net.Mail;
using System.Globalization;

namespace Amkor_Material_Manager
{
    public partial class AMM_Main : Form
    {
        public static AMM.AMM AMM = new AMM.AMM();

        public string Version = "";
        public static string strAMM_Connect = "NG";

        //기본 정보        
        public static string strDefault_linecode = "", strDefault_Group = "", strDefault_Start = "", strSMSearchEnable = "", strMatchTab = "", strNumberPad = "", strAppVersion = "";
        public static bool strAutoUpdate = false;
        public static string strRequestor_id = "", strRequestor_name = "", strLogfilePath = "";
        public static string strAdminID = "", strAdminPW = "";
        public static bool bAdminLogin = false, bProcessing = false, IsExit = false;
        public static int nDefaultGroup = 0, nProgress = 0, nSelectedWin = 0;
        public static bool bThread_Order = false;
        //public static bool[] bTAlarm = { false, false, false, false, false, false };
        public static bool[] bTAlarm = { false, false, false, false, false}; //220829_ilyoung_타워그룹추가

        int nColorindex = 0;

        Form_Order Frm_Order = new Form_Order();
        Form_ITS Frm_ITS = new Form_ITS();
        Form_History Frm_History = new Form_History();
        Form_Monitor Frm_Monitor = new Form_Monitor();
        Form_Set Frm_Set = new Form_Set();

        Thread Thread_Progress = null;

        public AMM_Main()
        {
            Process thisProc = Process.GetCurrentProcess();

            if (IsProcessOpen("Amkor_Material_Manager") == false)
            {

            }
            else
            {
                if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
                {
                    MessageBox.Show("프로그램이 이미 실행 중 입니다. 종료 후 다시 실행 하십시오");
                    System.Environment.Exit(1);
                    return;
                }
            }

            InitializeComponent();
            Fnc_init();
        }
        public bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }
        public void Fnc_init()
        {
            Frm_Order.MdiParent = this;
            Frm_Order.Location = new Point(0, 0);
            Frm_Order.Size = new Size(1013, 390);

            Frm_ITS.MdiParent = this;
            Frm_ITS.Location = new Point(0, 0);
            Frm_ITS.Size = new Size(1013, 669);

            Frm_History.MdiParent = this;
            Frm_History.Location = new Point(0, 0);
            Frm_History.Size = new Size(1013, 669);

            Frm_Monitor.MdiParent = this;
            Frm_Monitor.Location = new Point(0, 390);
            Frm_Monitor.Size = new Size(1013, 279);

            Frm_Set.MdiParent = this;
            Frm_Set.Location = new Point(0, 0);
            Frm_Set.Size = new Size(1013, 669);

            strDefault_linecode = ConfigurationManager.AppSettings["Lincode"];
            strDefault_Group = ConfigurationManager.AppSettings["Group"];
            strDefault_Start = ConfigurationManager.AppSettings["Startup"];
            strSMSearchEnable = ConfigurationManager.AppSettings["SM_Enable"];
            strMatchTab = ConfigurationManager.AppSettings["Match_Tab"];
            strNumberPad = ConfigurationManager.AppSettings["Number_Pad"];
            strAutoUpdate = bool.Parse( ConfigurationManager.AppSettings["AutoUpdate"]);
            
            strAppVersion = Fnc_Load_UpdateInfo();

            

            if (strDefault_linecode == "")
                strDefault_linecode = "AS41100 ";

            if (strDefault_Group == "")
                strDefault_Group = "4";

            nDefaultGroup = Int32.Parse(strDefault_Group);

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.StartupPath + @"\Log");
            if (!di.Exists) { di.Create(); }
            strLogfilePath = di.ToString();

            strAdminID = "amkor";
            strAdminPW = "Amkor123!";

            ///////AMM Connection
            strAMM_Connect = AMM.Connect();
            int nStart = Int32.Parse(strDefault_Start);
            if (nStart == 0)
                Fnc_Show_OrderViewer();
            else if (nStart == 1)
                Fnc_Show_InventoryViewer();
            else if (nStart == 2)
                Fnc_Show_HistoryViewer();
            
            ThreadStart();
            timer1.Start();
            if(strAutoUpdate == true)
                CheckAppVersion();

            Text = "S/W Version: " + Version;

            Fnc_SaveLog("프로그램 시작.", 0);
        }

        private string Fnc_Load_UpdateInfo()
        {
            string strPath = Application.StartupPath + "\\Auto_Updater.exe.config";
            string strVerPath = Application.StartupPath + "\\Versioninfo.ini";

            if (!File.Exists(strPath))
            {
                //system.io.file.writealltext(strpath, "0");
            }
            else
            {
                if(File.Exists(strVerPath) == true)
                {
                    string[] lines = System.IO.File.ReadAllLines(strVerPath);
                    int nlength = lines.Length;

                    if (nlength > 0)
                        Version = lines[0];
                }
                else
                {
                    Version = "0";
                }
            }

            return Version;
            //return "0";
        }

        private void CheckAppVersion()
        {
            string AppVer = (AMM.ReadAppDate("AMM_CLIENT"));

            //DateTime dateApp = DateTime.Parse(AppVer);

            if (Form_Order.nTabIndex != 1)
            {
                if (strAppVersion != AppVer)
                {
                    using (Form_Progress Frm_Process = new Form_Progress())
                    {
                        Frm_Process.Form_Show("최종 버전이 아닙니다 Update 프로그램을 실행 합니다.", 1006);

                        /*
                        UpdatePopUp = false;
                        ProcessStartInfo psinfo = new ProcessStartInfo();
                        psinfo.UseShellExecute = true;
                        psinfo.FileName = Application.StartupPath + "\\Auto_Updater.exe";
                        psinfo.WorkingDirectory = Application.StartupPath;
                        psinfo.Verb = "runas";
                        Process.Start(psinfo);
                        */
                        
                        Process firstProc = new Process();
                        firstProc.StartInfo.FileName = Application.StartupPath + "\\RunUpdate.bat";
                        firstProc.EnableRaisingEvents = true;                        
                        firstProc.Start();
                        
                    }

                    File.SetLastWriteTime(Application.StartupPath + "\\Auto_Updater.exe.config", DateTime.Now);

                    Close();
                }
                UpdatePopUp = false;
            }
        }

        public void ThreadStart()
        {
            try
            {
                if (Thread_Progress != null)
                {
                    Thread_Progress.Abort();
                    Thread_Progress = null;
                }

                Thread_Progress = new Thread(ThreadProc);
                Thread_Progress.Start();
            }
            catch (Exception ex)
            {
                string str = string.Format("{0}", ex);
                //Log.WriteLog(Log4net.EnumLogLevel.ERROR, ex.ToString());
            }
        }

        public void ThreadProc()
        {
            while (IsExit == false)
            {
                if (this != null)
                {
                    if (Form_ITS.IsDateGathering && nSelectedWin == 2)
                    {
                        Form_Processing Process_Form = new Form_Processing();
                        Process_Form.ShowDialog();
                    }

                    if (Form_History.IsDateGathering && nSelectedWin == 3)
                    {
                        Form_Processing Process_Form = new Form_Processing();
                        Process_Form.ShowDialog();
                    }
                }

                Thread.Sleep(200);
            }
        }

        int AppVerCnt = 0;
        bool UpdatePopUp = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            string strToday = string.Format("{0}/{1:00}/{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            label_day.Text = strToday;
            label_time.Text = strHead;

            if(strAMM_Connect == "OK")
            {
                if(nColorindex == 0)
                {
                    label_state.BackColor = System.Drawing.Color.Green;
                    nColorindex = 1;
                }
                else
                {
                    label_state.BackColor = System.Drawing.Color.Blue;
                    nColorindex = 0;
                }
            }
            else
            {
                label_state.BackColor = System.Drawing.Color.Red;
            }

            if (AppVerCnt++ >= 300 && strAutoUpdate == true && UpdatePopUp == false)
            {
                UpdatePopUp = true;
                CheckAppVersion();
                AppVerCnt = 0;
                
            }
            SendLongTermReportMail();
        }

        bool longTermReport = false;

        bool ExcelExportComp = false;

        public void MakeLongTermReport()
        {
            ExcelExportComp = false;
            Thread th = new Thread(LongTermReportThread);
            th.Start();
        }

        private void LongTermReportThread()
        {
            Form_ITS its = new Form_ITS();

            ExcelExportComp = its.SetLongTermReport();
        }

        

        private void SendLongTermReportMail()
        {
            DateTime ExportStartTime = new DateTime();

            try
            {

                if (Properties.Settings.Default.LongTermReelReportEN == false)
                    return;

                if (Properties.Settings.Default.LongTimeReelReportInterval1 == 0 && (Properties.Settings.Default.LongTimeReelReportInterval2 != (int)DateTime.Now.DayOfWeek))
                    return;

                if (Properties.Settings.Default.LongTimeReelReportInterval1 == 1 && (Properties.Settings.Default.LongTimeReelReportInterval2 != DateTime.Now.Day))
                    return;

                if (Properties.Settings.Default.LongTimeReelReportHour != DateTime.Now.Hour)
                {
                    longTermReport = false;
                    return;
                }

                if (longTermReport == true)
                    return;
                longTermReport = true;

                ExportStartTime = DateTime.Now;
                MakeLongTermReport();

                while (!ExcelExportComp)
                {
                    if ((DateTime.Now - ExportStartTime).TotalSeconds > 60)
                    {
                        MessageBox.Show("Excel 파일을 생성하지 못했습니다.");
                        return;
                    }
                    Application.DoEvents();
                    Thread.Sleep(10);
                }

                MailMessage message = new MailMessage();

                CultureInfo ciCurr = CultureInfo.CurrentCulture;
                int weekNum = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

                for (int i = 0; i < Properties.Settings.Default.LongTimeReelReportMail.Split(';').Length - 1; i++)
                {
                    message.To.Add(Properties.Settings.Default.LongTimeReelReportMail.Split(';')[i].Split(',')[1]);
                }

                message.Subject = Properties.Settings.Default.LongTimeReelReportSubject.Replace("nn", weekNum.ToString("D2"));
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
            }
            catch (Exception ex)
            {

            }
        }

        private void AMM_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
           // DialogResult dialogResult1 = MessageBox.Show("프로그램을 종료 하시겠습니까?", "Exit", MessageBoxButtons.YesNo);
           //if (dialogResult1 == DialogResult.Yes)
           // {
                Form_ITS.bUpdate_Timer = false;

                Frm_Order.Fnc_MtlListCheck();
                bThread_Order = false;
                IsExit = true;

                if (Thread_Progress != null)
                    Thread_Progress.Abort();

                timer1.Stop();

                Frm_Set = null;
                Frm_Monitor = null;
                Frm_History = null;
                Frm_ITS = null;
                Frm_Order = null;
                AMM = null;

                Fnc_SaveLog("프로그램 종료.", 0);

                Thread.Sleep(100);

                System.Environment.Exit(1);
           // }
           // else
           // {
           //     e.Cancel = true;
           // }
        }

        public void Fnc_Show_OrderViewer()
        {
            nSelectedWin = 0;

            Frm_Monitor.Show();
            Frm_Order.Show();
            Frm_History.Hide();
            Frm_ITS.Hide();
            Frm_Set.Hide();

            button_order.ForeColor = System.Drawing.Color.OrangeRed;
            button_monitor.ForeColor = System.Drawing.Color.LightGray;
            button_inventory.ForeColor = System.Drawing.Color.LightGray;
            button_history.ForeColor = System.Drawing.Color.LightGray;
            button_request.ForeColor = System.Drawing.Color.LightGray;
            button_setting.ForeColor = System.Drawing.Color.LightGray;

            Application.DoEvents();

            Frm_Monitor.Fnc_Close();
            Frm_Monitor.Fnc_Init();

            Form_ITS.bUpdate_Timer = false;

            Frm_Order.Fnc_Init();
            bThread_Order = true;

            Form_ITS.IsDateGathering = false;
            Form_History.IsDateGathering = false;

            Fnc_SaveLog("릴 주문 창 이동.", 0);
        }
        
        public void Fnc_Show_MonitorViewer()
        {
            Frm_Order.Fnc_MtlListCheck();

            nSelectedWin = 1;

            Frm_Monitor.Show();
            Frm_Order.Hide();
            Frm_History.Hide();
            Frm_ITS.Hide();
            Frm_Set.Hide();

            button_order.ForeColor = System.Drawing.Color.LightGray;
            button_monitor.ForeColor = System.Drawing.Color.OrangeRed;
            button_inventory.ForeColor = System.Drawing.Color.LightGray;
            button_history.ForeColor = System.Drawing.Color.LightGray;
            button_request.ForeColor = System.Drawing.Color.LightGray;
            button_setting.ForeColor = System.Drawing.Color.LightGray;

            bThread_Order = false;

            Frm_Monitor.Fnc_Close();
            Frm_Monitor.Fnc_Init();

            Form_ITS.bUpdate_Timer = false;

            Form_ITS.IsDateGathering = false;
            Form_History.IsDateGathering = false;

            Fnc_SaveLog("설비 모니터링 창 이동.", 0);
        }

        private void AMM_Main_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AMM.SetEqStart("AS41100", "TWR2");
        }

        public void Fnc_Show_InventoryViewer()
        {
            Frm_Order.Fnc_MtlListCheck();

            nSelectedWin = 2;

            bThread_Order = false;

            Frm_ITS.Show();
            Frm_History.Hide();
            Frm_Order.Hide();
            Frm_Monitor.Hide();
            Frm_Set.Hide();

            button_order.ForeColor = System.Drawing.Color.LightGray;
            button_monitor.ForeColor = System.Drawing.Color.LightGray;
            button_inventory.ForeColor = System.Drawing.Color.OrangeRed;
            button_history.ForeColor = System.Drawing.Color.LightGray;
            button_request.ForeColor = System.Drawing.Color.LightGray;
            button_setting.ForeColor = System.Drawing.Color.LightGray;

            Application.DoEvents();

            Frm_Monitor.Fnc_Close();

            Frm_ITS.Fnc_Init();
            Frm_ITS.Fnc_Process_CalMaterialInfo();

            //Form_ITS.IsDateGathering = false;
            Form_History.IsDateGathering = false;
            Form_ITS.bUpdate_Timer = true;

            Fnc_SaveLog("재고 조회 창 이동.", 0);
        }

        public void Fnc_Show_HistoryViewer()
        {
            Frm_Order.Fnc_MtlListCheck();

            nSelectedWin = 3;

            bThread_Order = false;

            Frm_History.Show();
            Frm_Order.Hide();
            Frm_ITS.Hide();
            Frm_Monitor.Hide();
            Frm_Set.Hide();

            button_order.ForeColor = System.Drawing.Color.LightGray;
            button_monitor.ForeColor = System.Drawing.Color.LightGray;
            button_inventory.ForeColor = System.Drawing.Color.LightGray;
            button_history.ForeColor = System.Drawing.Color.OrangeRed;
            button_request.ForeColor = System.Drawing.Color.LightGray;
            button_setting.ForeColor = System.Drawing.Color.LightGray;

            Application.DoEvents();

            Frm_Monitor.Fnc_Close();

            Frm_History.Fnc_Init();

            Form_ITS.bUpdate_Timer = false;
            Form_ITS.IsDateGathering = false;
            //Form_History.IsDateGathering = false;

            Fnc_SaveLog("이력 조회 창 이동.", 0);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        public void Fnc_Show_RequestViewer()
        {
            Frm_Order.Fnc_MtlListCheck();

            nSelectedWin = 4;

            Frm_Set.Fnc_View(0);
            Frm_Set.Show();
            Frm_Order.Hide();
            Frm_History.Hide();
            Frm_ITS.Hide();
            Frm_Monitor.Hide();

            button_order.ForeColor = System.Drawing.Color.LightGray;
            button_monitor.ForeColor = System.Drawing.Color.LightGray;
            button_inventory.ForeColor = System.Drawing.Color.LightGray;
            button_history.ForeColor = System.Drawing.Color.LightGray;
            button_request.ForeColor = System.Drawing.Color.OrangeRed;
            button_setting.ForeColor = System.Drawing.Color.LightGray;

            bThread_Order = false;
            Frm_Monitor.Fnc_Close();

            Form_ITS.bUpdate_Timer = false;
            Form_ITS.IsDateGathering = false;
            Form_History.IsDateGathering = false;

            Fnc_SaveLog("사용 요청 창 이동.", 0);
        }

        public void Fnc_Show_SettingViewer()
        {
            Frm_Order.Fnc_MtlListCheck();

            nSelectedWin = 5;

            Frm_Set.Fnc_View(1);
            Frm_Set.Show();
            Frm_Order.Hide();
            Frm_History.Hide();
            Frm_ITS.Hide();
            Frm_Monitor.Hide();

            button_order.ForeColor = System.Drawing.Color.LightGray;
            button_monitor.ForeColor = System.Drawing.Color.LightGray;
            button_inventory.ForeColor = System.Drawing.Color.LightGray;
            button_history.ForeColor = System.Drawing.Color.LightGray;
            button_request.ForeColor = System.Drawing.Color.LightGray;
            button_setting.ForeColor = System.Drawing.Color.OrangeRed;

            bThread_Order = false;
            Frm_Monitor.Fnc_Close();

            Form_ITS.bUpdate_Timer = false;
            Form_ITS.IsDateGathering = false;
            Form_History.IsDateGathering = false;

            Fnc_SaveLog("설정 창 이동.", 0);

            bAdminLogin = false;
        }

        private void button_order_Click(object sender, EventArgs e)
        {
            Fnc_Show_OrderViewer();
        }

        private void button_monitor_Click(object sender, EventArgs e)
        {
            Fnc_Show_MonitorViewer();
        }

        private void button_inventory_Click(object sender, EventArgs e)
        {
            Fnc_Show_InventoryViewer();
        }

        private void button_history_Click(object sender, EventArgs e)
        {
            Fnc_Show_HistoryViewer();
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            Fnc_Show_RequestViewer();
        }

        private void button_setting_Click(object sender, EventArgs e)
        {
            if(bAdminLogin == false)
            {
                Form_Login Frm_Login = new Form_Login();

                Frm_Login.LogIn_Init();
                Frm_Login.ShowDialog();
            }

            if (bAdminLogin)
            {
                Fnc_Show_SettingViewer();
            }
        }
        public void Fnc_SaveLog(string strLog, int nType) ///설비별 개별 로그 저장
        {
            string strPath = "";
            if (nType == 0)
                strPath = strLogfilePath + "\\AMM_system_";
            else if(nType == 1)
                strPath = strLogfilePath + "\\AMM_order_";
            else if (nType == 2)
                strPath = strLogfilePath + "\\AMM_setting_";

            string strToday = string.Format("{0}{1:00}{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format(",{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            strPath = strPath + strToday + ".txt";
            strHead = strToday + strHead;

            string strSave;
            strSave = strHead + ',' + strLog;
            Fnc_WriteFile(strPath, strSave);
        }

        private void Fnc_WriteFile(string strFileName, string strLine)
        {
            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(strFileName, true))
            {
                file.WriteLine(strLine);
            }
        }
    }
}
