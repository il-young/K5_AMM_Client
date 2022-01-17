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
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;

namespace Amkor_Material_Manager
{
    public partial class Form_Order : Form
    {
        Form_Progress Frm_Process = new Form_Progress();

        Thread Thread_Progress = null;

        string strSelSid = "";
        string strSelLotid = "";
        string strSelUid = "";
        string strKeepingcount = "";
        string strPickingID = "";
        string strDefaultPickingID = "";
        int nTabIndex = 0;
        int nReadyMTLcount = 0;
        int nSelected_groupid = -1;
        int nMonitorIndex = 0;
        int nMethod = 0; //2021.06.18
        public static bool bForceSMClose = false; //2021.06.18
        public static bool bSM_ListMade = false; //2021.06.18

        public static string strPadSid = "";
        public static string strPadReelSid = "";
        public static string strPadReelqty = "";

        System.Diagnostics.Stopwatch sw_Inputcheck = new System.Diagnostics.Stopwatch();
        bool bStopwatch = false;
        string strSavefilePath = "";

        /*
        //Strip Mark
        public bool bSMDataLoad = false;

        string[] strGetData_AmkorID = new string[50]; //colm 2
        string[] strGetData_SubID = new string[50]; //colm 3
        string[] strGetData_LotNo = new string[50]; //colm 4
        string[] strGetData_CustName = new string[50]; //colm 7
        string[] strGetData_StripMark = new string[50]; //colm 13
        string[] strGetData_BizType = new string[50]; //colm 29

        string[] strGetData_BomSID = new string[50];
        string[] strGetData_BomItem = new string[50];

        int nListcnt = 0;
        int nBomListcnt = 0;
        int nBomTowercount = 0;
        */

        public Form_Order()
        {
            InitializeComponent();

            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.StartupPath + @"\Config");
            if (!di.Exists) { di.Create(); }
            strSavefilePath = di.ToString();

            timer1.Start();
        }

        public void Fnc_Init()
        {
            tabControl_Order.SelectedIndex = 0;

            textBox_input_sid.Text = "";
            textBox_input_sid.Focus();

            Fnc_Monitor_GetReadyInfo(AMM_Main.strDefault_linecode);

            if (!AMM_Main.bThread_Order)
            {
                ThreadStart();                
            }
        }

        public void Fnc_MtlListCheck()
        {
            if (nReadyMTLcount != 0)
            {
                AMM_Main.AMM.Delete_PickReadyinfo(AMM_Main.strDefault_linecode, strPickingID);
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
            while (AMM_Main.bThread_Order)
            {
                if (this != null)
                {
                    Fnc_Tab_Monitor();
                }

                if (AMM_Main.IsExit)
                {
                    timer1.Stop();
                    return;
                }

                Thread.Sleep(1000);
            }
        }

        private void tabControl_Order_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tabNo = tabControl_Order.SelectedIndex;

            nTabIndex = tabNo;

            if (tabNo == 0)
            {
                this.Location = new Point(0, 0);
                this.Size = new Size(1013, 390);

                /////////
                if (nReadyMTLcount != 0)
                {
                    AMM_Main.AMM.Delete_PickReadyinfo(AMM_Main.strDefault_linecode, strPickingID);
                }

                nReadyMTLcount = 0;

                comboBox_group.SelectedIndex = -1;

                textBox_sid.Text = "";
                label_Totalcount.Text = "";
                dataGridView_view.Columns.Clear();
                dataGridView_view.Rows.Clear();
                dataGridView_view.Refresh();
                //////////

                AMM_Main.strRequestor_id = "";
                AMM_Main.strRequestor_name = "";

                textBox_input_sid.Text = "";
                textBox_input_sid.Focus();

                ////Ready table 에 있는 자재 삭제                
            }
            else if (tabNo == 1)
            {
                this.Location = new Point(0, 0);
                this.Size = new Size(1013, 669);

                if (AMM_Main.strRequestor_id == "" || AMM_Main.strRequestor_name == "")
                {
                    tabControl_Order.SelectedIndex = 0;
                }

                nMonitorIndex = 0;
            }
            else if(tabNo == 2)
            {
                this.Location = new Point(0, 0);
                this.Size = new Size(1013, 669);

                AMM_Main.strRequestor_name = "";
                Fnc_Monitor_GetRequest(AMM_Main.strDefault_linecode);
            }
        }
      
        private void textBox_input_sid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                string strSid = textBox_input_sid.Text.Replace(';', ' ');
                strSid = strSid.Trim();
                string strName = AMM_Main.AMM.User_check(strSid);
                strName = strName.Trim();

                if (strName == "NO_INFO")
                {
                    string str = string.Format("등록 되지 않은 사용자 입니다.\n등록 후 사용 하세요.", 1000);

                    Frm_Process.Form_Show(str, 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }

                    textBox_input_sid.Text = "";
                    return;
                }

                label_Requestor.Text = strSid + " / " + strName;
                AMM_Main.strRequestor_id = strSid;
                AMM_Main.strRequestor_name = strName;

                dataGridView_ready.Columns.Clear();
                dataGridView_ready.Rows.Clear();
                dataGridView_ready.Refresh();

                dataGridView_ready.Columns.Add("No", "N0");
                dataGridView_ready.Columns.Add("SID", "SID");
                dataGridView_ready.Columns.Add("벤더#", "벤더#");
                dataGridView_ready.Columns.Add("UID", "UID");
                dataGridView_ready.Columns.Add("수량", "수량");
                dataGridView_ready.Columns.Add("위치", "위치");
                dataGridView_ready.Columns.Add("인치", "인치");
                dataGridView_ready.Columns.Add("투입", "투입");

                comboBox_group.SelectedIndex = AMM_Main.nDefaultGroup - 1;
                comboBox_method.SelectedIndex = 0;  //SID 조회

                label_stripmark.ForeColor = System.Drawing.Color.White;
                textBox_stripmark.Enabled = false;

                label_reelid.ForeColor = System.Drawing.Color.White;
                textBox_reelid.Enabled = false;

                label_sid.ForeColor = System.Drawing.Color.Black;
                textBox_sid.Enabled = true;

                textBox_input_sid.Clear();
                textBox_input_sid.Text = "OK";
                tabControl_Order.SelectedIndex = 1;

                textBox_sid.Focus();                             

                nReadyMTLcount = 0;

                //Fnc_Load_TowerUseInfo();
                Fnc_Get_PickID(AMM_Main.nDefaultGroup.ToString());
                                
                string strLog = string.Format("PICK LIST 생성 시작 - 사번:{0}, PICKID:{1}", label_Requestor.Text, strPickingID);
                Fnc_SaveLog(strLog, 1);
            }
        }
        private void Fnc_Get_PickID(string strGroupinfo)
        {
            // GetPickIDNo - query = string.Format(@"SELECT * FROM TB_IDNUNMER_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}'", strLinecode, strEquipid);

            ///Pick id load
            string equipid = "TWR" + strGroupinfo;
            var tableList = AMM_Main.AMM.GetPickIDNo(AMM_Main.strDefault_linecode, equipid);

            if (tableList.Rows.Count == 0)
            {
                if (strGroupinfo == "1")
                    label_pickid.Text = "PL0000001";
                else if (strGroupinfo == "2")
                    label_pickid.Text = "PM0000001";
                else if (strGroupinfo == "3")
                    label_pickid.Text = "PN0000001";


            }
            else
            {
                string strprefix = tableList.Rows[0]["PICK_PREFIX"].ToString();
                strprefix = strprefix.Trim();
                string strNo = tableList.Rows[0]["PICK_NUM"].ToString();
                strNo = strNo.Trim();
                label_pickid.Text = strprefix + strNo;
            }                      

            strPickingID = label_pickid.Text;

            if (AMM_Main.strDefault_Group == strGroupinfo)
                strDefaultPickingID = strPickingID;

            Fnc_Update_PickID(AMM_Main.strDefault_linecode, equipid, strPickingID);
        }
        private void Fnc_Update_PickID(string strlinecode, string streqid, string strCurPickID)
        {
            string strGetNo = strCurPickID.Substring(strCurPickID.Length - 7);
            string strGetPrefix = strCurPickID.Substring(0, 2);

            int nGetNo = Int32.Parse(strGetNo);

            if (nGetNo == 9999999)
                nGetNo = 0;

            nGetNo = nGetNo + 1;
            strGetNo = nGetNo.ToString();
            int nLength = strGetNo.Length;
            char[] chSetNo = new char[7];

            for (int n = 0; n < 7 - nLength; n++)
            {
                chSetNo[n] = '0';
            }

            for (int m = 0; m < nLength; m++)
            {
                chSetNo[7 - nLength + m] = strGetNo.Substring(m, 1)[0];
            }

            string text = new string(chSetNo);
            AMM_Main.AMM.Delete_PickIDNo(strlinecode, streqid);
            AMM_Main.AMM.SetPickIDNo(strlinecode, streqid, strGetPrefix, text);
        }        
        
        private void textBox_sid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (comboBox_method.SelectedIndex == 0)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (char)13) // Keys.Enter
                {       
                    string strSid = "";
                    int nLength = 0;

                    strSid = textBox_sid.Text;
                    nLength = strSid.Length;

                    if (nLength < 3 || nLength > 9)
                    {
                        return;
                    }



                    if(comboBox_group.SelectedIndex < 3)
                    {
                        ///SID 정보 가져 오기
                        int nGroup = comboBox_group.SelectedIndex;

                        ///HY20201124                    
                        if (AMM_Main.bTAlarm[nGroup])
                        {
                            string str = string.Format("타워 그룹 {0} 이 알람 상태 입니다.\n알람 해제를 요청 하세요\n\n리스트 생성은 가능 합니다.", nGroup + 1);
                            Frm_Process.Form_Show(str, 1000);

                            while (Frm_Process.bState)
                            {
                                Application.DoEvents();
                                Thread.Sleep(1);
                            }
                        }
                        ///////

                        string strGroup = (nGroup + 1).ToString();

                        if (strSid == "")
                            return;

                        Fnc_SetMtlInfo_FromSID(AMM_Main.strDefault_linecode, strGroup, strSid, false);

                        string strLog = string.Format("릴 조회 - 사번:{0}, PICKID:{1}, SID:{2}", label_Requestor.Text, strPickingID, strSid);
                        Fnc_SaveLog(strLog, 1);
                    }
                    else // 220112 bykim  ALL List 조회
                    {
                        if (strSid == "")
                            return;


                        for (int i =0; i<3; i++)
                        {
                            if (AMM_Main.bTAlarm[i])
                            {
                                string str = string.Format("타워 그룹 {0} 이 알람 상태 입니다.\n알람 해제를 요청 하세요\n\n리스트 생성은 가능 합니다.", i + 1);
                                Frm_Process.Form_Show(str, 1000);

                                while (Frm_Process.bState)
                                {
                                    Application.DoEvents();
                                    Thread.Sleep(1);
                                }
                            }

                        }

                        Fnc_GetMtlInfo_SID_ALL(AMM_Main.strDefault_linecode, strSid, false);



                        string strLog = string.Format("릴 조회 - 사번:{0}, PICKID:{1}, SID:{2}", label_Requestor.Text, strPickingID, strSid);
                        Fnc_SaveLog(strLog, 1);
                    }

                   
                }
            }
            else if (comboBox_method.SelectedIndex == 1)
            {
                //Strip Mark
                if (e.KeyChar == (char)13)
                {
                    string strSM = "";
                    int nLength = 0;

                    strSM = textBox_stripmark.Text;
                    nLength = strSM.Length;

                    if (nLength < 3)
                        return;

                    textBox_stripmark.Clear();
                    //Fnc_ViewLotlist(strSM);
                }
            }
            else if (comboBox_method.SelectedIndex == 2)
            {
                if (e.KeyChar == (char)13)
                {
                    //UID 정보 가져 오기
                    string strUid = "";

                    strUid = textBox_reelid.Text;

                    int nGroup = comboBox_group.SelectedIndex;

                    ///HY20201124
                    if (AMM_Main.bTAlarm[nGroup])
                    {
                        string str = string.Format("타워 그룹 {0} 이 알람 상태 입니다.\n알람 해제를 요청 하세요\n\n리스트 생성은 가능 합니다.", nGroup + 1);
                        Frm_Process.Form_Show(str, 1000);

                        while (Frm_Process.bState)
                        {
                            Application.DoEvents();
                            Thread.Sleep(1);
                        }
                    }
                    ///////
                    
                    string strGroup = (nGroup + 1).ToString();

                    int nReturn = Fnc_GetMtlInfo_ReelID(AMM_Main.strDefault_linecode, strGroup, strUid, false);

                    if (nReturn == 0)  //OK
                    {
                        textBox_reelcount.Focus();
                    }
                    else if (nReturn == 1) //다른 그룹에 있음
                    {
                        comboBox_group.Focus();
                    }
                    else if (nReturn == 2) //자재 없음
                    {
                        textBox_sid.Clear();
                        textBox_sid.Focus();
                    }
                }
            }
        }

        private void textBox_reelcount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)13)
            {

                if(comboBox_group.SelectedIndex != 3)
                {
                    if (textBox_reelcount.Text == "")
                        return;

                    int nRowcount = dataGridView_view.Rows.Count;

                    if (nRowcount < 1)
                    {
                        textBox_reelcount.Text = "";
                        return;
                    }

                    int nIndex = dataGridView_view.CurrentCell.RowIndex;

                    //string strNo = dataGridView_view.Rows[nIndex].Cells[0].Value.ToString();

                    strSelSid = dataGridView_view.Rows[nIndex].Cells[0].Value.ToString();
                    strSelLotid = dataGridView_view.Rows[nIndex].Cells[1].Value.ToString();

                    //strSelUid = dataGridView_view.Rows[nIndex].Cells[3].Value.ToString();

                    ///
                    int nMethod = comboBox_method.SelectedIndex;
                    if (nMethod == 0)
                        strKeepingcount = dataGridView_view.Rows[nIndex].Cells[2].Value.ToString();
                    else if (nMethod == 1)
                        strKeepingcount = dataGridView_view.Rows[nIndex].Cells[2].Value.ToString();  // 추가 확인이 필요함.
                    else if (nMethod == 2)
                    {
                        strSelUid = dataGridView_view.Rows[nIndex].Cells[2].Value.ToString();
                        strKeepingcount = dataGridView_view.Rows[nIndex].Cells[3].Value.ToString();
                    }

                    int nRequestcount = Int32.Parse(textBox_reelcount.Text);
                    int nKeepCount = Int32.Parse(strKeepingcount);

                    //if(strNo == "" || nRequestcount == 0)
                    if (nRequestcount == 0)
                    {
                        string str = string.Format("수량을 입력 하여 주십시오", 1);
                        Frm_Process.Form_Show(str, 1);

                        while (Frm_Process.bState)
                        {
                            Application.DoEvents();
                            Thread.Sleep(1);
                        }

                        textBox_reelcount.Text = "";
                        textBox_reelcount.Focus();
                        return;
                    }


                    int nCheckcount = nReadyMTLcount + nRequestcount;
                    if (nCheckcount > 20)
                    {
                        string str = string.Format("배출 수량이 너무 많습니다.20개 초과!\n한 개 리스트에 자재 20개 까지 담을 수 있습니다.", 1);
                        Frm_Process.Form_Show(str, 1);

                        while (Frm_Process.bState)
                        {
                            Application.DoEvents();
                            Thread.Sleep(1);
                        }
                        textBox_reelcount.Text = (nCheckcount - 20).ToString();
                        textBox_reelcount.Focus();
                        return;
                    }



                    if (nRequestcount > nKeepCount)
                    {
                        int nAllCount = 0;
                        for (int i = 0; i < dataGridView_view.RowCount; i++)
                        {
                            nAllCount += Convert.ToInt16(dataGridView_view.Rows[i].Cells[2].Value);

                        }

                        if (nRequestcount > nAllCount)
                        {
                            string str = string.Format("보유 수량 보다 요청 수량이 많습니다.\n다시 입력 하여 주십시오", 1);
                            Frm_Process.Form_Show(str, 1);

                            while (Frm_Process.bState)
                            {
                                Application.DoEvents();
                                Thread.Sleep(1);
                            }

                            textBox_reelcount.Text = (nRequestcount - nKeepCount).ToString();
                            textBox_reelcount.Focus();
                            return;
                        }

                        ////자재 업데이트
                        int nGroup = comboBox_group.SelectedIndex;
                        string strGroup = (nGroup + 1).ToString();

                        int nLoopCount = 0;

                        while (nRequestcount > 0)
                        {


                            strSelSid = dataGridView_view.Rows[nIndex + nLoopCount].Cells[0].Value.ToString();
                            strSelLotid = dataGridView_view.Rows[nIndex + nLoopCount].Cells[1].Value.ToString();

                            nKeepCount = Convert.ToInt32(dataGridView_view.Rows[nIndex + nLoopCount].Cells[2].Value.ToString());

                            if (nRequestcount <= nKeepCount)
                            {
                                if (nMethod != 2)
                                    Fnc_RequestMaterial(AMM_Main.strDefault_linecode, strGroup, strSelSid, strSelLotid, nRequestcount, strPickingID);
                                else
                                    Fnc_RequestMaterial_uid(AMM_Main.strDefault_linecode, strGroup, strSelUid, nRequestcount, strPickingID);
                            }
                            else
                            {
                                if (nMethod != 2)
                                    Fnc_RequestMaterial(AMM_Main.strDefault_linecode, strGroup, strSelSid, strSelLotid, nKeepCount, strPickingID);
                                else
                                    Fnc_RequestMaterial_uid(AMM_Main.strDefault_linecode, strGroup, strSelUid, nKeepCount, strPickingID);
                            }


                            if (nMethod == 0) //SID
                            {
                                //bool lastmtl = false;
                                //if (nRequestcount == nKeepCount)
                                //    lastmtl = true;
                                Fnc_SetMtlInfo_FromSID(AMM_Main.strDefault_linecode, strGroup, strSelSid, true);

                                textBox_sid.Text = "";
                                textBox_sid.Focus();
                            }
                            else if (nMethod == 1) //S/M
                            {
                                textBox_stripmark.Focus();
                            }
                            else if (nMethod == 2) //Reel ID
                            {
                                dataGridView_view.Columns.Clear();
                                dataGridView_view.Rows.Clear();
                                dataGridView_view.Refresh();

                                textBox_reelid.Text = "";
                                textBox_reelid.Focus();
                            }

                            Fnc_Picklist_Comfirm();

                            nRequestcount -= nKeepCount;
                        }

                    }
                    else
                    {

                        ////자재 업데이트
                        int nGroup = comboBox_group.SelectedIndex;
                        string strGroup = (nGroup + 1).ToString();

                        if (nMethod != 2)
                            Fnc_RequestMaterial(AMM_Main.strDefault_linecode, strGroup, strSelSid, strSelLotid, nRequestcount, strPickingID);
                        else
                            Fnc_RequestMaterial_uid(AMM_Main.strDefault_linecode, strGroup, strSelUid, nRequestcount, strPickingID);

                        textBox_reelcount.Text = "";

                        if (nMethod == 0) //SID
                        {
                            //bool lastmtl = false;
                            //if (nRequestcount == nKeepCount)
                            //    lastmtl = true;

                            Fnc_SetMtlInfo_FromSID(AMM_Main.strDefault_linecode, strGroup, strSelSid, true);

                            textBox_sid.Text = "";
                            textBox_sid.Focus();
                        }
                        else if (nMethod == 1) //S/M
                        {
                            textBox_stripmark.Focus();
                        }
                        else if (nMethod == 2) //Reel ID
                        {
                            dataGridView_view.Columns.Clear();
                            dataGridView_view.Rows.Clear();
                            dataGridView_view.Refresh();

                            textBox_reelid.Text = "";
                            textBox_reelid.Focus();
                        }

                        Fnc_Picklist_Comfirm();
                        /*
                        ///SID 정보 가져 오기 
                        if (strSelSid == "")
                        {
                            textBox_sid.Text = "";
                            textBox_sid.Focus();
                            return;
                        }
                        */

                    }
                }
                else //220112 All Data 반출대기 리스트 보내기
                {
                    if (textBox_reelcount.Text == "")
                        return;

                    int nRowcount = dataGridView_view.Rows.Count;

                    if (nRowcount < 1)
                    {
                        textBox_reelcount.Text = "";
                        return;
                    }

                    int nIndex = dataGridView_view.CurrentCell.RowIndex;

                    strSelSid = dataGridView_view.Rows[nIndex].Cells[0].Value.ToString();
                    strSelLotid = dataGridView_view.Rows[nIndex].Cells[1].Value.ToString();



                    int nRequestcount = Int32.Parse(textBox_reelcount.Text);
                    int nKeepCount = 0;


                    int nMethod = comboBox_method.SelectedIndex;
                    for(int i = 0; i < nRowcount; i++)
                    {
                        strKeepingcount = dataGridView_view.Rows[i].Cells[2].Value.ToString();
                        nKeepCount += Int32.Parse(strKeepingcount);
                    }

                    if(nRequestcount > nKeepCount)
                    {
                        string str = string.Format("보유 수량 보다 요청 수량이 많습니다.\n다시 입력 하여 주십시오", 1);
                        Frm_Process.Form_Show(str, 1);

                        while (Frm_Process.bState)
                        {
                            Application.DoEvents();
                            Thread.Sleep(1);
                        }

                        textBox_reelcount.Text = (nRequestcount - nKeepCount).ToString();
                        textBox_reelcount.Focus();
                        return;
                    }

                    Fnc_RequestMaterial_ALL(AMM_Main.strDefault_linecode, strSelSid, strSelLotid, nRequestcount, strPickingID);

                }
                
            }
        }
        private void Fnc_SetMtlInfo_FromSID(string strlinecode, string strGroup, string strSID, bool lastmtl)
        {
            int nReturn = Fnc_GetMtlInfo_SID(strlinecode, strGroup, strSID, lastmtl);

            if (nReturn == 0)  //OK
            {
                textBox_reelcount.Focus();
            }
            else if (nReturn == 1) //다른 그룹에 있음
            {
                //comboBox_group.Focus();
                textBox_sid.Clear();
                textBox_sid.Focus();
            }
            else if (nReturn == 2) //자재 없음
            {
                textBox_sid.Clear();
                textBox_sid.Focus();
            }
        }

        private void Fnc_UpdateReadyInfo(string pickid)
        {
            dataGridView_ready.Columns.Clear();
            dataGridView_ready.Rows.Clear();
            dataGridView_ready.Refresh();

            dataGridView_ready.Columns.Add("No", "N0");
            dataGridView_ready.Columns.Add("SID", "SID");
            dataGridView_ready.Columns.Add("벤더#", "벤더#");
            dataGridView_ready.Columns.Add("UID", "UID");
            dataGridView_ready.Columns.Add("수량", "수량");
            dataGridView_ready.Columns.Add("위치", "위치");
            dataGridView_ready.Columns.Add("인치", "인치");
            dataGridView_ready.Columns.Add("투입", "투입");

            var MtlList = AMM_Main.AMM.GetPickingReadyinfo_ID(pickid);

            List<StorageData> list = new List<StorageData>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                list.Add(data);
            }

            list.Sort(CompareStorageData);

            nReadyMTLcount = 0;
            for (int n = 0; n < list.Count; n++)
            {
                nReadyMTLcount++;
                dataGridView_ready.Rows.Add(new object[8] { nReadyMTLcount, list[n].SID, list[n].LOTID, list[n].UID, list[n].Quantity, list[n].Tower_no, list[n].Inch, list[n].Input_type });                
                
            }

            label_Totalcount.Text = nReadyMTLcount.ToString();
            //[211215_Sangik.choi_Pickid 수량 제한 수정

            
            ///SID 정보 가져 오기 
            if (textBox_sid.Text == "")
                return;

            int nGroup = nSelected_groupid;
            string strGroup = (nGroup + 1).ToString();

            Fnc_SetMtlInfo_FromSID(AMM_Main.strDefault_linecode, strGroup, textBox_sid.Text, false);
        }

        public void Fnc_Check_TwrUse(int nGroup)
        {
            string strTwrName = "";

            for(int n = 1; n < 5; n++)
            {
                strTwrName = string.Format("T0{0}0{1}", nGroup, n);
                string strUse = AMM_Main.AMM.Get_Twr_Use(strTwrName);

                if(n == 1)
                {
                    if (strUse == "USE")
                    {
                        checkBox_tower1.ForeColor = Color.Black;
                        checkBox_tower1.Text = strTwrName + " 사용";
                        checkBox_tower1.Checked = true;
                    }
                    else
                    {
                        checkBox_tower1.ForeColor = Color.Red;
                        checkBox_tower1.Text = strTwrName + " 사용 안함";
                        checkBox_tower1.Checked = false;
                    }
                }
                else if (n == 2)
                {
                    if (strUse == "USE")
                    {
                        checkBox_tower2.ForeColor = Color.Black;
                        checkBox_tower2.Text = strTwrName + " 사용";
                        checkBox_tower2.Checked = true;
                    }
                    else
                    {
                        checkBox_tower2.ForeColor = Color.Red;
                        checkBox_tower2.Text = strTwrName + " 사용 안함";
                        checkBox_tower2.Checked = false;
                    }
                }

            }
        }

        private void comboBox_group_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = "";

            int nSel = comboBox_group.SelectedIndex;

            if (nSel < 0)
                return;



            if(nSel != 3)
            {
                int nCheckCount = dataGridView_ready.Rows.Count;

                if (nCheckCount > 0)
                {
                    str = string.Format("배출 대기 리스트가 존재 합니다.\n리스트 삭제 또는 완료 후 선택 가능 합니다.");
                    Frm_Process.Form_Show(str, 1);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }

                    Fnc_Check_TwrUse(nSel + 1);
                    comboBox_group.SelectedIndex = nSelected_groupid;

                    return;
                }

                ///HY20210126 
                string strTwr = string.Format("T0{0}01", nSel + 1);
                string strReelState = AMM_Main.AMM.Get_Twr_State(AMM_Main.strDefault_linecode, strTwr);

                if (strReelState == "PICK_FAIL")
                {
                    str = string.Format("그룹 {0} 타워 내부에 배출 릴이 있습니다.\n리스트를 생성 해도 배출 할 수 없습니다.\n{0}-1번 타워에 있는 릴 제거 후 리스트를 생성 하세요.", nSel + 1);
                    Frm_Process.Form_Show(str, 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }

                strTwr = string.Format("T0{0}02", nSel + 1);
                strReelState = AMM_Main.AMM.Get_Twr_State(AMM_Main.strDefault_linecode, strTwr);

                if (strReelState == "PICK_FAIL")
                {
                    str = string.Format("그룹 {0} 타워 내부에 배출 릴이 있습니다.\n리스트를 생성 해도 배출 할 수 없습니다.\n{0}-2번 타워에 있는 릴 제거 후 리스트를 생성 하세요.", nSel + 1);
                    Frm_Process.Form_Show(str, 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }

                strTwr = string.Format("T0{0}03", nSel + 1);
                strReelState = AMM_Main.AMM.Get_Twr_State(AMM_Main.strDefault_linecode, strTwr);

                if (strReelState == "PICK_FAIL")
                {
                    str = string.Format("그룹 {0} 타워 내부에 배출 릴이 있습니다.\n리스트를 생성 해도 배출 할 수 없습니다.\n{0}-3번 타워에 있는 릴 제거 후 리스트를 생성 하세요.", nSel + 1);
                    Frm_Process.Form_Show(str, 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }

                strTwr = string.Format("T0{0}04", nSel + 1);
                strReelState = AMM_Main.AMM.Get_Twr_State(AMM_Main.strDefault_linecode, strTwr);

                if (strReelState == "PICK_FAIL")
                {
                    str = string.Format("그룹 {0} 타워 내부에 배출 릴이 있습니다.\n리스트를 생성 해도 배출 할 수 없습니다.\n{0}-4번 타워에 있는 릴 제거 후 리스트를 생성 하세요.", nSel + 1);
                    Frm_Process.Form_Show(str, 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }
                ///////

                if (AMM_Main.nDefaultGroup == nSel + 1)
                {
                    strPickingID = strDefaultPickingID;
                    label_pickid.Text = strDefaultPickingID;
                    nSelected_groupid = nSel;
                    Fnc_Check_TwrUse(nSel + 1);
                    return;
                }

                int nMtl = dataGridView_view.RowCount;

                if (nMtl > 0)
                {
                    textBox_sid.Text = "";
                    dataGridView_view.Columns.Clear();
                    dataGridView_view.Rows.Clear();
                    dataGridView_view.Refresh();
                }

                nSelected_groupid = nSel;

                int nGroup = nSelected_groupid;
                string strGroup = (nGroup + 1).ToString();

                Fnc_Get_PickID(strGroup);
                //////

                int n = comboBox_method.SelectedIndex;
                Fnc_Check_TwrUse(nSel + 1);

                if (n == 0) //SID
                {
                    label_stripmark.ForeColor = System.Drawing.Color.White;
                    textBox_stripmark.Enabled = false;

                    label_reelid.ForeColor = System.Drawing.Color.White;
                    textBox_reelid.Enabled = false;

                    label_sid.ForeColor = System.Drawing.Color.Black;
                    textBox_sid.Enabled = true;

                    textBox_sid.Focus();
                }
                else if (n == 1) //S/M
                {
                    label_stripmark.ForeColor = System.Drawing.Color.Black;
                    textBox_stripmark.Enabled = true;

                    label_reelid.ForeColor = System.Drawing.Color.White;
                    textBox_reelid.Enabled = false;

                    label_sid.ForeColor = System.Drawing.Color.White;
                    textBox_sid.Enabled = false;

                    textBox_stripmark.Focus();
                }
                else if (n == 2) //Reel ID
                {
                    label_stripmark.ForeColor = System.Drawing.Color.White;
                    textBox_stripmark.Enabled = false;

                    label_reelid.ForeColor = System.Drawing.Color.Black;
                    textBox_reelid.Enabled = true;

                    label_sid.ForeColor = System.Drawing.Color.White;
                    textBox_sid.Enabled = false;

                    textBox_reelid.Focus();
                }
            }
            else if(nSel == 3)
            {
                int nCheckCount = dataGridView_ready.Rows.Count;

                if (nCheckCount > 0)
                {
                    str = string.Format("배출 대기 리스트가 존재 합니다.\n리스트 삭제 또는 완료 후 선택 가능 합니다.");
                    Frm_Process.Form_Show(str, 1);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }

                    Fnc_Check_TwrUse(1);
                    Fnc_Check_TwrUse(2);
                    Fnc_Check_TwrUse(3);
                    Fnc_Check_TwrUse(4);
                    comboBox_group.SelectedIndex = nSelected_groupid;

                    return;
                }


                ///HY20210126 
                string strTwr = string.Format("T0{0}01", 1);
                string strReelState = AMM_Main.AMM.Get_Twr_State(AMM_Main.strDefault_linecode, strTwr);

                if (strReelState == "PICK_FAIL")
                {
                    str = string.Format("그룹 {0} 타워 내부에 배출 릴이 있습니다.\n리스트를 생성 해도 배출 할 수 없습니다.\n{0}-1번 타워에 있는 릴 제거 후 리스트를 생성 하세요.", nSel + 1);
                    Frm_Process.Form_Show(str, 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }

                strTwr = string.Format("T0{0}02", 2);
                strReelState = AMM_Main.AMM.Get_Twr_State(AMM_Main.strDefault_linecode, strTwr);

                if (strReelState == "PICK_FAIL")
                {
                    str = string.Format("그룹 {0} 타워 내부에 배출 릴이 있습니다.\n리스트를 생성 해도 배출 할 수 없습니다.\n{0}-2번 타워에 있는 릴 제거 후 리스트를 생성 하세요.", nSel + 1);
                    Frm_Process.Form_Show(str, 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }

                strTwr = string.Format("T0{0}03", 3);
                strReelState = AMM_Main.AMM.Get_Twr_State(AMM_Main.strDefault_linecode, strTwr);

                if (strReelState == "PICK_FAIL")
                {
                    str = string.Format("그룹 {0} 타워 내부에 배출 릴이 있습니다.\n리스트를 생성 해도 배출 할 수 없습니다.\n{0}-3번 타워에 있는 릴 제거 후 리스트를 생성 하세요.", nSel + 1);
                    Frm_Process.Form_Show(str, 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }

                strTwr = string.Format("T0{0}04", 4);
                strReelState = AMM_Main.AMM.Get_Twr_State(AMM_Main.strDefault_linecode, strTwr);

                if (strReelState == "PICK_FAIL")
                {
                    str = string.Format("그룹 {0} 타워 내부에 배출 릴이 있습니다.\n리스트를 생성 해도 배출 할 수 없습니다.\n{0}-4번 타워에 있는 릴 제거 후 리스트를 생성 하세요.", nSel + 1);
                    Frm_Process.Form_Show(str, 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }









            }
            
        }

        private void comboBox_method_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBox_method.SelectedIndex;
            nMethod = n; //2021.06.18

            textBox_reelid.Text = "";
            textBox_stripmark.Text = "";
            textBox_sid.Text = "";

            if (n == 0) //SID
            {
                label_stripmark.ForeColor = System.Drawing.Color.White;
                textBox_stripmark.Enabled = false;

                label_reelid.ForeColor = System.Drawing.Color.White;
                textBox_reelid.Enabled = false;

                label_sid.ForeColor = System.Drawing.Color.Black;
                textBox_sid.Enabled = true;

                textBox_sid.Focus();
            }
            else if(n == 1) //S/M
            {
                AMM_Main.strSMSearchEnable = "TRUE";

                if (AMM_Main.strSMSearchEnable == "TRUE")
                {
                    //string str = string.Format("S/M 조회 기능이 Disable 되어 있어 사용 할 수 없습니다.\n사용하길 원하시면 관리자에게 문의 하세요.", 1);
                    //Frm_Process.Form_Show(str, 1);
                    //MessageBox.Show(str);

                    //comboBox_method.SelectedIndex = 0;

                    Form_StripMark Frm_StripMark = new Form_StripMark();
                    Frm_StripMark.Fnc_Show();

                    if(bSM_ListMade)
                        Fnc_View_Monitor_SM(AMM_Main.strDefault_linecode, AMM_Main.strRequestor_id);

                    bSM_ListMade = false;

                    return;
                }

                /*
                label_stripmark.ForeColor = System.Drawing.Color.Black;
                textBox_stripmark.Enabled = true;

                label_reelid.ForeColor = System.Drawing.Color.White;
                textBox_reelid.Enabled = false;

                label_sid.ForeColor = System.Drawing.Color.White;
                textBox_sid.Enabled = false;

                textBox_stripmark.Focus();
                */
            }
            else if(n == 2) //Reel ID
            {
                label_stripmark.ForeColor = System.Drawing.Color.White;
                textBox_stripmark.Enabled = false;

                label_reelid.ForeColor = System.Drawing.Color.Black;
                textBox_reelid.Enabled = true;

                label_sid.ForeColor = System.Drawing.Color.White;
                textBox_sid.Enabled = false;

                textBox_reelid.Focus();
            }
        }

        public int Fnc_GetMtlInfo_SID(string strlinecode, string strGroup, string sid, bool lastmtl)
        {
            dataGridView_view.Columns.Clear();
            dataGridView_view.Rows.Clear();
            dataGridView_view.Refresh();

            // GetMTLInfo - query = string.Format(@"SELECT * FROM TB_MTL_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}'", strLinecode, strEquipid);
            string equipid = "TWR" + strGroup;
            var MtlList = AMM_Main.AMM.GetMTLInfo(strlinecode, equipid);

            List<StorageData> list = new List<StorageData>();

            string strMsg = "", strCheckSid = "";
            int nCheckGroup = 0;
            int nSidLength = sid.Length;

            if (MtlList.Rows.Count < 1)
            {
                for (int n = 1; n < 4; n++) //210923_Sangik.choi_자재 조회 시 7번 그룹 표기 안되는 문제 조치
                {
                    equipid = "TWR" + n.ToString();

                    var MtlList2 = AMM_Main.AMM.GetMTLInfo(strlinecode, equipid);

                    MtlList2.DefaultView.Sort = "SID";
                    DataTable sortedDT = MtlList2.DefaultView.ToTable();

                    for (int i = 0; i < MtlList2.Rows.Count; i++)
                    {
                        StorageData datacheck = new StorageData();

                        datacheck.UID = sortedDT.Rows[i]["UID"].ToString(); datacheck.UID = datacheck.UID.Trim();
                        datacheck.SID = sortedDT.Rows[i]["SID"].ToString(); datacheck.SID = datacheck.SID.Trim();

                        string strCheck = datacheck.SID.Substring(datacheck.SID.Length - nSidLength);
                        bool bCheck = false;

                        if (sid == strCheck)
                            bCheck = true;

                        if (datacheck.UID != "" && bCheck)
                        {
                            if (nCheckGroup != n) //strCheckSid != datacheck.SID && 
                            {
                                string str = string.Format("SID# {0} , 그룹 # {1}\n", datacheck.SID, n);
                                strMsg = strMsg + str;

                                strCheckSid = datacheck.SID;
                                nCheckGroup = n;
                            }
                        }
                    }
                }

                if(strMsg == "")
                {
                    if (!lastmtl)
                    {
                        Frm_Process.Form_Show("자재 없음! 핸들러 또는 MC에게 문의 하세요!", 1000);

                        while (Frm_Process.bState)
                        {
                            Application.DoEvents();
                            Thread.Sleep(1);
                        }
                    }

                    return 2;
                }
                else
                {
                    if (!lastmtl)
                    {
                        strMsg = strMsg + "\n보관 중 입니다. 그룹을 확인 하세요";

                        Frm_Process.Form_Show(strMsg, 1);

                        while (Frm_Process.bState)
                        {
                            Application.DoEvents();
                            Thread.Sleep(1);
                        }
                    }

                    return 1;
                }                
            }
            else
            {
                for (int i = 0; i < MtlList.Rows.Count; i++)
                {
                    StorageData data = new StorageData();

                    data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                    data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();

                    if (data.SID == "101416632")
                    {

                    }

                    if (data.UID != "" && data.SID.Contains(sid))
                    {
                        data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                        data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                        data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                        data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                        data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                        data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                        data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                        data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                        /// 중복자재, 배출 리스트 or 배출 준비 자재 확인!
                        string strJudge = AMM_Main.AMM.GetPickingReadyinfo(data.UID);
                        string strJudge2 = AMM_Main.AMM.GetPickingListinfo(data.UID);

                        //// Tower 제외
                        string strTowerNo = data.Tower_no.Substring(4, 1);
                        string strJudge3 = "OK";

                        if(!checkBox_tower1.Checked)
                        {
                            if(strTowerNo == "1")
                                strJudge3 = "NG";
                        }

                        if (!checkBox_tower2.Checked)
                        {
                            if (strTowerNo == "2")
                                strJudge3 = "NG";
                        }


                        if (strJudge == "OK" && strJudge2 == "OK" && strJudge3 == "OK")
                        {
                            list.Add(data);
                        }
                        else if (strJudge == "ERROR")
                            AMM_Main.strAMM_Connect = "NG";

                    }
                }

                if (list.Count == 0)
                {
                    strMsg = "";

                    for (int n = 1; n < 4; n++)
                    {
                        equipid = "TWR" + n.ToString();

                        var MtlList2 = AMM_Main.AMM.GetMTLInfo(strlinecode, equipid);

                        MtlList2.DefaultView.Sort = "SID";
                        DataTable sortedDT = MtlList2.DefaultView.ToTable();

                        for (int i = 0; i < MtlList2.Rows.Count; i++)
                        {
                            StorageData datacheck = new StorageData();

                            datacheck.UID = sortedDT.Rows[i]["UID"].ToString(); datacheck.UID = datacheck.UID.Trim();
                            datacheck.SID = sortedDT.Rows[i]["SID"].ToString(); datacheck.SID = datacheck.SID.Trim();

                            string strCheck = datacheck.SID.Substring(datacheck.SID.Length - nSidLength);
                            bool bCheck = false;

                            if (sid == strCheck)
                                bCheck = true;

                            if (datacheck.UID != "" && bCheck)
                            {                              
                                string strJudge = AMM_Main.AMM.GetPickingReadyinfo(datacheck.UID);
                                string strJudge2 = AMM_Main.AMM.GetPickingListinfo(datacheck.UID);

                                if (strJudge == "OK" && strJudge2 == "OK")
                                {
                                    if (strCheckSid != datacheck.SID || nCheckGroup != n)
                                    {
                                        string str = string.Format("SID# {0} , 그룹 # {1}\n", datacheck.SID, n);
                                        strMsg = strMsg + str;

                                        strCheckSid = datacheck.SID;
                                        nCheckGroup = n;
                                    }
                                }                                    
                            }
                        }
                    }

                    if (strMsg == "")
                    {
                        if (!lastmtl)
                        {
                            Frm_Process.Form_Show("자재 없음! 핸들러 또는 MC에게 문의 하세요!", 1000);
                            while (Frm_Process.bState)
                            {
                                Application.DoEvents();
                                Thread.Sleep(1);
                            }
                        }

                        return 2;
                    }
                    else
                    {
                        if (!lastmtl)
                        {
                            strMsg = strMsg + "\n보관 중 입니다. 그룹을 확인 하세요";

                            Frm_Process.Form_Show(strMsg, 1);

                            while (Frm_Process.bState)
                            {
                                Application.DoEvents();
                                Thread.Sleep(1);
                            }
                        }

                        return 1;
                    }
                }


                list.Sort(CompareStorageData);
            }

            if (!lastmtl)
            {
                nCheckGroup = 0;

                for (int n = 1; n < 4; n++)//210923_Sangik.choi_자재 조회 시 7번그룹 표기 안되는 문제 조치
                {
                    if (n != Int32.Parse(strGroup))
                    {
                        equipid = "TWR" + n.ToString();

                        var MtlList2 = AMM_Main.AMM.GetMTLInfo(strlinecode, equipid);

                        MtlList2.DefaultView.Sort = "SID";
                        DataTable sortedDT = MtlList2.DefaultView.ToTable();

                        for (int i = 0; i < MtlList2.Rows.Count; i++)
                        {
                            StorageData datacheck = new StorageData();

                            datacheck.UID = sortedDT.Rows[i]["UID"].ToString(); datacheck.UID = datacheck.UID.Trim();
                            datacheck.SID = sortedDT.Rows[i]["SID"].ToString(); datacheck.SID = datacheck.SID.Trim();

                            string strCheck = datacheck.SID.Substring(datacheck.SID.Length - nSidLength);
                            bool bCheck = false;

                            if (sid == strCheck)
                                bCheck = true;

                            if (datacheck.UID != "" && bCheck)
                            {
                                if (nCheckGroup != n)
                                {
                                    string str = string.Format("SID# {0} , 그룹 # {1}\n", datacheck.SID, n);
                                    strMsg = strMsg + str;

                                    strCheckSid = datacheck.SID;
                                    nCheckGroup = n;
                                }
                            }
                        }
                    }
                }

                if (strMsg != "")
                {
                    strMsg = "4자리가 같은 자재가 다른 그룹에도 있습니다.\n전체 SID 를 확인 후 계속 진행 하세요.\n\n" + strMsg;

                    Frm_Process.Form_Show(strMsg, 1);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }
            }
            
            //dataGridView_view.Columns.Add("NO", "NO");
            dataGridView_view.Columns.Add("SID", "SID");
            dataGridView_view.Columns.Add("벤더#", "벤더#");
            dataGridView_view.Columns.Add("보유수량", "보유수량");
            dataGridView_view.Columns.Add("위치", "위치");

            string strSetLotid = "", strSetSID = "", strCompareSID = "";
            int nReelcount = 0;
            int nIdx = 0;
            int nSidcount = 0;

            equipid = "TWR" + strGroup;
            strMsg = "";
                
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].SID == "101416632")
                {

                }

                if(strCompareSID != list[i].SID)
                {
                    string str = string.Format("SID# {0}\n", list[i].SID);
                    strMsg = strMsg + str;

                    strCompareSID = list[i].SID;
                    nSidcount++;
                }

                if(strSetLotid != list[i].LOTID)
                {
                    if (strSetLotid != "")
                    {
                        //dataGridView_view.Rows.Add(new object[5] { nIdx, strSetSID, strSetLotid, nReelcount, equipid });
                        dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, equipid });

                        strSetLotid = list[i].LOTID;
                        strSetSID = list[i].SID;
                        nReelcount = 1;
                        
                        nIdx++;
                    }
                    else
                    {
                        strSetLotid = list[i].LOTID;
                        strSetSID = list[i].SID;
                        nReelcount = 1;
                        nIdx++;
                    }
                }
                else
                {
                    nReelcount++;
                }

                if (i == list.Count - 1)
                {
                    //dataGridView_view.Rows.Add(new object[5] { nIdx, strSetSID, strSetLotid, nReelcount, equipid });
                    dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, equipid });
                }
            }

            if(nSidcount > 1 && !lastmtl)
            {
                strMsg = strMsg + "\n여러개 SID가 존재 합니다.\n전체 자리를 확인 후 계속 진행 하세요.";

                Frm_Process.Form_Show(strMsg, 1);

                while (Frm_Process.bState)
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }
            return 0;
        }



        public int Fnc_GetMtlInfo_SID_ALL(string strlinecode, string sid, bool lastmtl)
        {

            dataGridView_view.Columns.Clear();
            dataGridView_view.Rows.Clear();
            dataGridView_view.Refresh();

            // GetMTLInfo - query = string.Format(@"SELECT * FROM TB_MTL_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}'", strLinecode, strEquipid);
            //string equipid = "TWR" + strGroup;


            var MtlList = AMM_Main.AMM.GetMTLInfo(strlinecode);

            List<StorageData> list = new List<StorageData>();
            List<StorageData> list2 = new List<StorageData>();
            List<StorageData> list3 = new List<StorageData>();

            string strMsg = "", strCheckSid = "";
            int nCheckGroup = 0;
            int nSidLength = sid.Length;

            if (MtlList.Rows.Count < 1)
            {
                if (!lastmtl)
                {
                    Frm_Process.Form_Show("자재 없음! 핸들러 또는 MC에게 문의 하세요!", 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }

                return 2;
            }
            else
            {
                for (int i = 0; i < MtlList.Rows.Count; i++)
                {
                    StorageData data = new StorageData();

                    data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                    data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();

                    if (data.SID == "101416632")
                    {

                    }

                    if (data.UID != "" && data.SID.Contains(sid))
                    {
                        data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                        data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                        data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                        data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                        data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                        data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                        data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                        data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                        /// 중복자재, 배출 리스트 or 배출 준비 자재 확인!
                        string strJudge = AMM_Main.AMM.GetPickingReadyinfo(data.UID);
                        string strJudge2 = AMM_Main.AMM.GetPickingListinfo(data.UID);

                        //// Tower 제외
                        string strTowerNo = data.Tower_no.Substring(4, 1);
                        string strJudge3 = "OK";

                        if (!checkBox_tower1.Checked)
                        {
                            if (strTowerNo == "1")
                                strJudge3 = "NG";
                        }

                        if (!checkBox_tower2.Checked)
                        {
                            if (strTowerNo == "2")
                                strJudge3 = "NG";
                        }


                        if (strJudge == "OK" && strJudge2 == "OK" && strJudge3 == "OK")
                        {
                            if(data.Tower_no.Substring(2,1) == "1")  list.Add(data);
                            else if (data.Tower_no.Substring(2, 1) == "2") list2.Add(data);
                            else if (data.Tower_no.Substring(2, 1) == "3") list3.Add(data);
                        }
                        else if (strJudge == "ERROR")
                            AMM_Main.strAMM_Connect = "NG";

                    }
                }

                if (list.Count == 0 && list2.Count == 0 && list3.Count == 0)
                {
                    if (strMsg == "")
                    {
                        if (!lastmtl)
                        {
                            Frm_Process.Form_Show("자재 없음! 핸들러 또는 MC에게 문의 하세요!", 1000);
                            while (Frm_Process.bState)
                            {
                                Application.DoEvents();
                                Thread.Sleep(1);
                            }
                        }

                        return 2;
                    }
                }


                list.Sort(CompareStorageData);
                list2.Sort(CompareStorageData);
                list3.Sort(CompareStorageData);
            }


            //dataGridView_view.Columns.Add("NO", "NO");
            dataGridView_view.Columns.Add("SID", "SID");
            dataGridView_view.Columns.Add("벤더#", "벤더#");
            dataGridView_view.Columns.Add("보유수량", "보유수량");
            dataGridView_view.Columns.Add("위치", "위치");

            string strSetLotid = "", strSetSID = "", strCompareSID = "", strTWID = "";
            int nReelcount = 0;
            int nIdx = 0;
            int nSidcount = 0;

            //equipid = "TWR" + strGroup;
            strMsg = "";

            if(list.Count != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].SID == "101416632")
                    {

                    }

                    if (strCompareSID != list[i].SID)
                    {
                        string str = string.Format("SID# {0}\n", list[i].SID);
                        strMsg = strMsg + str;

                        strCompareSID = list[i].SID;
                        nSidcount++;
                    }

                    if (strSetLotid != list[i].LOTID)
                    {
                        if (strSetLotid != "" && strTWID == list[i].Tower_no.ToString().Substring(2, 1))
                        {

                            dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, "TW" + list[i].Tower_no.Substring(2, 1) });

                            strSetLotid = list[i].LOTID;
                            strSetSID = list[i].SID;
                            strTWID = list[i].Tower_no.Substring(2, 1);
                            nReelcount = 1;

                        }
                        else if (i == 0)
                        {
                            strSetLotid = list[i].LOTID;
                            strSetSID = list[i].SID;
                            strTWID = list[i].Tower_no.Substring(2, 1);
                            nReelcount = 1;

                        }
                    }
                    else
                    {

                        if (strTWID != list[i].Tower_no.Substring(2, 1))
                        {
                            dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, "TW" + strTWID });
                            strSetLotid = list[i].LOTID;
                            strSetSID = list[i].SID;
                            strTWID = list[i].Tower_no.Substring(2, 1);
                            nReelcount = 1;
                        }
                        else nReelcount++;
                    }

                    if (i == list.Count - 1)
                    {
                        dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, "TW" + list[i].Tower_no.Substring(2, 1) });
                    }
                }
            }

            strSetLotid = "";
            strSetSID = "";
            strCompareSID = "";
            strTWID = "";
            nReelcount = 0;

            if (list2.Count != 0)
            {
                for (int i = 0; i < list2.Count; i++)
                {


                    if (strSetLotid != list2[i].LOTID)
                    {
                        if (strSetLotid != "" && strTWID == list2[i].Tower_no.ToString().Substring(2, 1))
                        {

                            dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, "TW" + list2[i].Tower_no.Substring(2, 1) });

                            strSetLotid = list2[i].LOTID;
                            strSetSID = list2[i].SID;
                            strTWID = list2[i].Tower_no.Substring(2, 1);
                            nReelcount = 1;

                        }
                        else if (i == 0)
                        {
                            strSetLotid = list2[i].LOTID;
                            strSetSID = list2[i].SID;
                            strTWID = list2[i].Tower_no.Substring(2, 1);
                            nReelcount = 1;

                        }
                    }
                    else
                    {

                        if (strTWID != list2[i].Tower_no.Substring(2, 1))
                        {
                            dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, "TW" + strTWID });
                            strSetLotid = list2[i].LOTID;
                            strSetSID = list2[i].SID;
                            strTWID = list2[i].Tower_no.Substring(2, 1);
                            nReelcount = 1;
                        }
                        else nReelcount++;
                    }

                    if (i == list2.Count - 1)
                    {
                        dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, "TW" + list2[i].Tower_no.Substring(2, 1) });
                    }
                }
            }

            strSetLotid = "";
            strSetSID = "";
            strCompareSID = "";
            strTWID = "";
            nReelcount = 0;

            if (list3.Count != 0)
            {
                for (int i = 0; i < list3.Count; i++)
                {


                    if (strSetLotid != list3[i].LOTID)
                    {
                        if (strSetLotid != "" && strTWID == list3[i].Tower_no.ToString().Substring(2, 1))
                        {

                            dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, "TW" + list3[i].Tower_no.Substring(2, 1) });

                            strSetLotid = list3[i].LOTID;
                            strSetSID = list3[i].SID;
                            strTWID = list3[i].Tower_no.Substring(2, 1);
                            nReelcount = 1;

                        }
                        else if (i == 0)
                        {
                            strSetLotid = list3[i].LOTID;
                            strSetSID = list3[i].SID;
                            strTWID = list3[i].Tower_no.Substring(2, 1);
                            nReelcount = 1;

                        }
                    }
                    else
                    {

                        if (strTWID != list3[i].Tower_no.Substring(2, 1))
                        {
                            dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, "TW" + strTWID });
                            strSetLotid = list3[i].LOTID;
                            strSetSID = list3[i].SID;
                            strTWID = list3[i].Tower_no.Substring(2, 1);
                            nReelcount = 1;
                        }
                        else nReelcount++;
                    }

                    if (i == list3.Count - 1)
                    {
                        dataGridView_view.Rows.Add(new object[4] { strSetSID, strSetLotid, nReelcount, "TW" + list3[i].Tower_no.Substring(2, 1) });
                    }
                }
            }

            if (nSidcount > 1 && !lastmtl)
            {
                strMsg = strMsg + "\n여러개 SID가 존재 합니다.\n전체 자리를 확인 후 계속 진행 하세요.";

                Frm_Process.Form_Show(strMsg, 1);

                while (Frm_Process.bState)
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                }
            }

            return 0;
        }

        public int Fnc_GetMtlInfo_ReelID(string strlinecode, string strGroup, string uid, bool lastmtl)
        {
            dataGridView_view.Columns.Clear();
            dataGridView_view.Rows.Clear();
            dataGridView_view.Refresh();

            string equipid = "TWR" + strGroup;
            var MtlList = AMM_Main.AMM.GetMTLInfo(strlinecode, equipid);

            List<StorageData> list = new List<StorageData>();

            if (MtlList.Rows.Count < 1)
            {
                for (int n = 1; n < 4; n++)//210923_Sangik.choi_자재 조회 시 7번그룹 표기 안되는 문제 조치
                {
                    equipid = "TWR" + n.ToString();

                    var MtlList2 = AMM_Main.AMM.GetMTLInfo(strlinecode, equipid);

                    for (int i = 0; i < MtlList2.Rows.Count; i++)
                    {
                        StorageData datacheck = new StorageData();

                        datacheck.UID = MtlList2.Rows[i]["UID"].ToString(); datacheck.UID = datacheck.UID.Trim();
                        datacheck.SID = MtlList2.Rows[i]["SID"].ToString(); datacheck.SID = datacheck.SID.Trim();

                        if (datacheck.UID ==  uid)
                        {
                            string str = string.Format("해당 자재는 Group # {0} 에서 배출 가능 합니다.", n);
                            Frm_Process.Form_Show(str, n);

                            while (Frm_Process.bState)
                            {
                                Application.DoEvents();
                                Thread.Sleep(1);
                            }

                            return 1;
                        }
                    }
                }

                if (!lastmtl)
                {
                    Frm_Process.Form_Show("자재 없음! 핸들러 또는 MC에게 문의 하세요!", 1000);
                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                }

                return 2;
            }
            else
            {
                for (int i = 0; i < MtlList.Rows.Count; i++)
                {
                    StorageData data = new StorageData();

                    data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                    data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();

                    if (data.UID == uid)
                    {
                        data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                        data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                        data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                        data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                        data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                        data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                        data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                        data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                        /// 중복자재, 배출 리스트 or 배출 준비 자재 확인!
                        list.Add(data);
                    }
                }

                if (list.Count == 0)
                {
                    for (int n = 1; n < 4; n++)//210923_Sangik.choi_자재 조회 시 7번그룹 표기 안되는 문제 조치
                    {
                        equipid = "TWR" + n.ToString();

                        var MtlList2 = AMM_Main.AMM.GetMTLInfo(strlinecode, equipid);

                        for (int i = 0; i < MtlList2.Rows.Count; i++)
                        {
                            StorageData datacheck = new StorageData();

                            datacheck.UID = MtlList2.Rows[i]["UID"].ToString(); datacheck.UID = datacheck.UID.Trim();
                            datacheck.SID = MtlList2.Rows[i]["SID"].ToString(); datacheck.SID = datacheck.SID.Trim();

                            if (datacheck.UID == uid)
                            {
                                string str = string.Format("해당 자재는 Group # {0} 에서 배출 가능 합니다.", n);
                                Frm_Process.Form_Show(str, n);

                                while (Frm_Process.bState)
                                {
                                    Application.DoEvents();
                                    Thread.Sleep(1);
                                }
                                return 1;
                            }
                        }
                    }

                    if (!lastmtl)
                    {
                        Frm_Process.Form_Show("자재 없음! 핸들러 또는 MC에게 문의 하세요!", 1000);
                        while (Frm_Process.bState)
                        {
                            Application.DoEvents();
                            Thread.Sleep(1);
                        }
                    }

                    return 2;
                }


                list.Sort(CompareStorageData);
            }

            //dataGridView_view.Columns.Add("NO", "NO");
            dataGridView_view.Columns.Add("SID", "SID");
            dataGridView_view.Columns.Add("벤더#", "벤더#");
            dataGridView_view.Columns.Add("UID", "UID");
            dataGridView_view.Columns.Add("보유수량", "보유수량");
            dataGridView_view.Columns.Add("위치", "위치");

            string strSetLotid = "", strSetSID = "", strSetUID = "";
            int nReelcount = 0;
            int nIdx = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (strSetLotid != list[i].LOTID)
                {
                    if (strSetLotid != "")
                    {
                        //dataGridView_view.Rows.Add(new object[6] { nIdx, strSetSID, strSetLotid, strSetUID, nReelcount, equipid });
                        dataGridView_view.Rows.Add(new object[5] { strSetSID, strSetLotid, strSetUID, nReelcount, equipid });

                        strSetLotid = list[i].LOTID;
                        strSetSID = list[i].SID;
                        strSetUID = list[i].UID;
                        nReelcount = 1;

                        nIdx++;
                    }
                    else
                    {
                        strSetLotid = list[i].LOTID;
                        strSetSID = list[i].SID;
                        strSetUID = list[i].UID;
                        nReelcount = 1;
                        nIdx++;
                    }
                }
                else
                {
                    nReelcount++;
                }

                if (i == list.Count - 1)
                {
                    //dataGridView_view.Rows.Add(new object[6] { nIdx, strSetSID, strSetLotid, strSetUID, nReelcount, equipid });
                    dataGridView_view.Rows.Add(new object[5] { strSetSID, strSetLotid, strSetUID, nReelcount, equipid });
                }
            }

            return 0;
        }
        public int Fnc_RequestMaterial(string strlinecode, string strGroup, string strSid, string strLotid, int nCount, string strPickingid)
        {
            //////다시 코딩////
            string equipid = "TWR" + strGroup;
            var MtlList = AMM_Main.AMM.GetMTLInfo_SID(strlinecode, equipid, strSid);

            List<StorageData> list_first = new List<StorageData>();

            List<StorageData> list = new List<StorageData>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();

                if (data.LOTID == strLotid)
                {
                    data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                    data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                    data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                    data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                    data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                    data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                    data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                    data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                    string strJudge = AMM_Main.AMM.GetPickingReadyinfo(data.UID);
                    string strJudge2 = AMM_Main.AMM.GetPickingListinfo(data.UID);

                    //// Tower 제외
                    string strTowerNo = data.Tower_no.Substring(4, 1);
                    string strJudge3 = "OK";

                    if (!checkBox_tower1.Checked)
                    {
                        if (strTowerNo == "1")
                            strJudge3 = "NG";
                    }

                    if (!checkBox_tower2.Checked)
                    {
                        if (strTowerNo == "2")
                            strJudge3 = "NG";
                    }



                    if (strJudge == "OK" && strJudge2 == "OK" && strJudge3 == "OK")
                        list_first.Add(data);
                    else if (strJudge == "ERROR")
                        AMM_Main.strAMM_Connect = "NG";
                }
            }


            list.Sort(CompareStorageData);
            list = list_first.OrderByDescending(x => x.Quantity).ToList();  // 220112 bykim List 수량 기준 정렬



            if (list.Count < nCount)
            {
                string str = string.Format("요청 수량 보다 배출 가능 수량이 적습니다.\n다시 자재 조회 하여 주십시오.", 1);

                Frm_Process.Form_Show(str, 1);

                while (Frm_Process.bState)
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                }

                return -1;
            }

            for(int n = 0; n<nCount; n++)
            {
                nReadyMTLcount++;
                dataGridView_ready.Rows.Add(new object[8] { nReadyMTLcount, list[n].SID, list[n].LOTID, list[n].UID, list[n].Quantity, list[n].Tower_no, list[n].Inch, list[n].Input_type });
                string strJudge = AMM_Main.AMM.SetPicking_Readyinfo(strlinecode, equipid, strPickingid, list[n].UID, AMM_Main.strRequestor_id, list[n].Tower_no, list[n].SID, list[n].LOTID, list[n].Quantity,
                    list[n].Manufacturer, list[n].Production_date, list[n].Inch, list[n].Input_type, "AMM_SID");

                if(strJudge == "NG")
                {
                    MessageBox.Show("DB 저장 실패!");
                }
            }

            label_Totalcount.Text = nReadyMTLcount.ToString();


            return 0;
        }
        public int Fnc_RequestMaterial_ALL(string strlinecode, string strSid, string strLotid, int nCount, string strPickingid)
        {
            var MtlList = AMM_Main.AMM.GetMTLInfo_SID_ALL(strlinecode, strSid);

            List<StorageData> list_first = new List<StorageData>();

            List<StorageData> list = new List<StorageData>();

            List<StorageData> List_Sort1 = new List<StorageData>();
            List<StorageData> List_Sort2 = new List<StorageData>();

            List<StorageData> List_Sort7 = new List<StorageData>();
            List<StorageData> List_Sort13 = new List<StorageData>();


            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                string strJudge = AMM_Main.AMM.GetPickingReadyinfo(data.UID);
                string strJudge2 = AMM_Main.AMM.GetPickingListinfo(data.UID);

                //// Tower 제외
                string strTowerNo = data.Tower_no.Substring(4, 1);
                string strJudge3 = "OK";

                if (!checkBox_tower1.Checked)
                {
                    if (strTowerNo == "1")
                        strJudge3 = "NG";
                }

                if (!checkBox_tower2.Checked)
                {
                    if (strTowerNo == "2")
                        strJudge3 = "NG";
                }



                if (strJudge == "OK" && strJudge2 == "OK" && strJudge3 == "OK")
                    list_first.Add(data);
                else if (strJudge == "ERROR")
                    AMM_Main.strAMM_Connect = "NG";
                
            }

            list_first.Sort(CompareStorageData);

            list = list_first.OrderByDescending(x => x.Quantity).ToList(); // 220112 bykim List 수량 기준 정렬
            if ((Int32.Parse(list[0].Quantity)) > (Int32.Parse(list[list.Count - 1].Quantity)))
            {
                list = list_first.OrderBy(x => x.Quantity).ToList();
            }

            string equipid = "";


            for (int n = 0; n<MtlList.Rows.Count; n++)
            {
                if (nCount == 0) break;

                if(Int32.Parse(list[n].Quantity) % 1000 != 0)
                {
                    List_Sort1.Add(list[n]);
                }
                else
                {
                    List_Sort2.Add(list[n]);

                }

            }

            List_Sort1 = List_Sort1.OrderByDescending(x => x.Quantity).ToList(); // 220112 bykim List 수량 기준 정렬

            if(List_Sort1.Count != 0)
            {
                if ((Int32.Parse(List_Sort1[0].Quantity)) > (Int32.Parse(List_Sort1[List_Sort1.Count - 1].Quantity)))
                {
                    List_Sort1 = List_Sort1.OrderBy(x => x.Quantity).ToList();
                }

            }


            List_Sort2 = List_Sort2.OrderBy(x => x.Input_date).ToList();
            if (List_Sort2.Count != 0)
            {
                if ((Int64.Parse(List_Sort2[0].Input_date)) > (Int64.Parse(List_Sort2[List_Sort2.Count - 1].Input_date)))
                {
                    List_Sort2 = List_Sort2.OrderByDescending(x => x.Input_date).ToList();

                }
            }
            for (int n = 0; n < nCount; n++)
            {

                if( (List_Sort1.Count - n) > 0)
                {
                    nReadyMTLcount++;
                    dataGridView_ready.Rows.Add(new object[8] { nReadyMTLcount, List_Sort1[n].SID, List_Sort1[n].LOTID, List_Sort1[n].UID, List_Sort1[n].Quantity, List_Sort1[n].Tower_no, List_Sort1[n].Inch, List_Sort1[n].Input_type });

                    equipid = "TWR" + List_Sort1[n].Tower_no.Substring(2, 1);
                    string strJudge = AMM_Main.AMM.SetPicking_Readyinfo(strlinecode, equipid, strPickingid, List_Sort1[n].UID, AMM_Main.strRequestor_id, List_Sort1[n].Tower_no, List_Sort1[n].SID, List_Sort1[n].LOTID, List_Sort1[n].Quantity,
                        List_Sort1[n].Manufacturer, List_Sort1[n].Production_date, List_Sort1[n].Inch, List_Sort1[n].Input_type, "AMM_SID");

                    if (strJudge == "NG")
                    {
                        MessageBox.Show("DB 저장 실패!");
                    }
                }
                else
                {
                    nReadyMTLcount++;
                    dataGridView_ready.Rows.Add(new object[8] { nReadyMTLcount, List_Sort2[n-List_Sort1.Count].SID, List_Sort2[n-List_Sort1.Count].LOTID, List_Sort2[n-List_Sort1.Count].UID, List_Sort2[n-List_Sort1.Count].Quantity, List_Sort2[n-List_Sort1.Count].Tower_no, List_Sort2[n-List_Sort1.Count].Inch, List_Sort2[n-List_Sort1.Count].Input_type });

                    equipid = "TWR" + List_Sort2[n-List_Sort1.Count].Tower_no.Substring(2, 1);
                    string strJudge = AMM_Main.AMM.SetPicking_Readyinfo(strlinecode, equipid, strPickingid, List_Sort2[n-List_Sort1.Count].UID, AMM_Main.strRequestor_id, List_Sort2[n-List_Sort1.Count].Tower_no, List_Sort2[n-List_Sort1.Count].SID, List_Sort2[n-List_Sort1.Count].LOTID, List_Sort2[n-List_Sort1.Count].Quantity,
                        List_Sort2[n-List_Sort1.Count].Manufacturer, List_Sort2[n-List_Sort1.Count].Production_date, List_Sort2[n-List_Sort1.Count].Inch, List_Sort2[n-List_Sort1.Count].Input_type, "AMM_SID");

                    if (strJudge == "NG")
                    {
                        MessageBox.Show("DB 저장 실패!");
                    }
                }
               
            }



            label_Totalcount.Text = nReadyMTLcount.ToString();

            return 0;

        }




            public int Fnc_RequestMaterial_uid(string strlinecode, string strGroup, string strUid, int nCount, string strPickingid)
        {
            //////다시 코딩////

            // GetMTLInfo_UID - query = string.Format(@"SELECT * FROM TB_MTL_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}' and UID='{2}'", strLinecode, strEquipid, strUID);
            string equipid = "TWR" + strGroup;
            var MtlList = AMM_Main.AMM.GetMTLInfo_UID(strlinecode, equipid, strUid);

            List<StorageData> list = new List<StorageData>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();

                if (data.UID == strUid)
                {
                    data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                    data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                    data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                    data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                    data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                    data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                    data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                    data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                    string strJudge = AMM_Main.AMM.GetPickingReadyinfo(data.UID);
                    string strJudge2 = AMM_Main.AMM.GetPickingListinfo(data.UID);

                    //// Tower 제외
                    string strTowerNo = data.Tower_no.Substring(4, 1);
                    string strJudge3 = "OK";

                    if (!checkBox_tower1.Checked)
                    {
                        if (strTowerNo == "1")
                            strJudge3 = "NG";
                    }

                    if (!checkBox_tower2.Checked)
                    {
                        if (strTowerNo == "2")
                            strJudge3 = "NG";
                    }



                    if (strJudge == "OK" && strJudge2 == "OK" && strJudge3 == "OK")
                        list.Add(data);
                    else if (strJudge == "ERROR")
                        AMM_Main.strAMM_Connect = "NG";
                }
            }

            list.Sort(CompareStorageData);

            if (list.Count < nCount)
            {
                string str = string.Format("요청 수량 보다 배출 가능 수량이 적습니다.\n다시 자재 조회 하여 주십시오.", 1);
                Frm_Process.Form_Show(str, 1);

                while (Frm_Process.bState)
                {
                    Application.DoEvents();
                    Thread.Sleep(1);
                }

                return -1;
            }

            for (int n = 0; n < nCount; n++)
            {
                nReadyMTLcount++;
                dataGridView_ready.Rows.Add(new object[8] { nReadyMTLcount, list[n].SID, list[n].LOTID, list[n].UID, list[n].Quantity, list[n].Tower_no, list[n].Inch, list[n].Input_type });
                string strJudge = AMM_Main.AMM.SetPicking_Readyinfo(strlinecode, equipid, strPickingid, list[n].UID, AMM_Main.strRequestor_id, list[n].Tower_no, list[n].SID, list[n].LOTID, list[n].Quantity,
                    list[n].Manufacturer, list[n].Production_date, list[n].Inch, list[n].Input_type, "AMM_UID");

                if (strJudge == "NG")
                {
                    MessageBox.Show("DB 저장 실패!");
                }
            }

            label_Totalcount.Text = nReadyMTLcount.ToString();


            return 0;
        }

        int CompareStorageData(StorageData obj1, StorageData obj2)
        {
            return obj1.UID.CompareTo(obj2.UID);
        }        

        int CompareStorageData_Qty(StorageData obj1, StorageData obj2)
        {
            return obj1.Quantity.CompareTo(obj2.Quantity);
        }

    private void dataGridView_view_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int colIndex = e.ColumnIndex;

            if (colIndex != 0)
                colIndex = 0;

            if (rowIndex == -1)
                return;

            strSelSid = dataGridView_view.Rows[rowIndex].Cells[0].Value.ToString();
            strSelLotid = dataGridView_view.Rows[rowIndex].Cells[1].Value.ToString();
            strKeepingcount = dataGridView_view.Rows[rowIndex].Cells[2].Value.ToString();

            textBox_reelcount.Focus();
        }        

        public void Fnc_Tab_Monitor()
        {
            if(nTabIndex == 0)
            {
                if (bStopwatch)
                {
                    sw_Inputcheck.Stop();
                    sw_Inputcheck.Reset();
                    bStopwatch = false;
                }
            }
            else if(nTabIndex == 1)
            {
                ///////////입력 시간 체크 2분간 수량 입력 없을 시 자동으로 로그인 창 이동///////////
                if (nReadyMTLcount == 0)
                {
                    if (bStopwatch == false)
                    {
                        sw_Inputcheck.Start();
                        bStopwatch = true;
                    }
                }
                else
                {
                    if (bStopwatch)
                    {
                        sw_Inputcheck.Stop();
                        sw_Inputcheck.Reset();
                        bStopwatch = false;
                    }
                }

                int nTimelimit = 100000; //2021.06.18

                if (nMethod == 1) //SM 청구
                    nTimelimit = 600000; //600s
                else
                    nTimelimit = 100000; //100s

                if (sw_Inputcheck.ElapsedMilliseconds > (long)nTimelimit)
                {
                    sw_Inputcheck.Stop();
                    sw_Inputcheck.Reset();
                    bStopwatch = false;

                    if (nMethod == 1)
                        bForceSMClose = true;

                    if (tabControl_Order.InvokeRequired)
                    {
                        //tabControl_Order.Invoke(new Action(() => { tabControl_Order.SelectedIndex = 0; }));
                    }

                    return;
                }
            }
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (nReadyMTLcount == 0)
                return;

            int nIndex = dataGridView_ready.CurrentCell.RowIndex;

            string strDeleteUID;
            strDeleteUID = dataGridView_ready.Rows[nIndex].Cells[3].Value.ToString();

            string strJudge = AMM_Main.AMM.Delete_PickReadyinfo_ReelID(AMM_Main.strDefault_linecode, strDeleteUID);

            if (strJudge == "NG")
            {
                AMM_Main.strAMM_Connect = "NG";
                return;
            }

            Fnc_UpdateReadyInfo(strPickingID);
        }

        private void button_out_Click(object sender, EventArgs e)
        {
            
            int nGroup = nSelected_groupid;
            string strGroup = (nGroup + 1).ToString();

            if (strPickingID != "")
            {
                if(comboBox_group.SelectedIndex != 3)
                {
                    Fnc_Picklist_Comfirm();
                    Fnc_Save_TowerUseInfo();
                    Fnc_Picklist_Send(AMM_Main.strDefault_linecode, "TWR" + strGroup, strPickingID);
                }
                else
                {
                    //Fnc_Picklist_Comfirm();
                    Fnc_Save_TowerUseInfo();
                    Fnc_Picklist_Send_All(AMM_Main.strDefault_linecode, strPickingID);
                }


            }
        }

        private void Fnc_Picklist_Send(string strlincode, string strequip, string strPickID)
        {
            if (strPickID == "")
            {
                string str = string.Format("배출 ID 정보가 없습니다.");
                Fnc_AlartMessage(str, 1);
                return;
            }
            ///Picklist 생성
            DataTable dt = AMM_Main.AMM.GetPickingReadyinfo_ID(strPickID);

            int nCount = dt.Rows.Count;

            if (nCount == 0)
            {
                string str = string.Format("리스트 생성 목록이 없습니다.");
                Fnc_AlartMessage(str, 1);
                return;
            }

            StorageData data = new StorageData();

            string strJudge = "";

            for (int i = 0; i < nCount; i++)
            {
                data.Linecode = dt.Rows[i]["LINE_CODE"].ToString(); data.Linecode = data.Linecode.Trim();
                data.Equipid = dt.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.UID = dt.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.Requestor = dt.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();
                data.Tower_no = dt.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.SID = dt.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.LOTID = dt.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = dt.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = dt.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = dt.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = dt.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = dt.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                strJudge = AMM_Main.AMM.SetPicking_Listinfo(strlincode, strequip, strPickID, data.UID, AMM_Main.strRequestor_id, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, "AMM");
                
                if (strJudge == "NG")
                {
                    string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                    Fnc_AlartMessage(str, 1000);
                    AMM_Main.strAMM_Connect = "NG";

                    return;
                }
                else if (strJudge == "DUPLICATE")
                {
                    string str = string.Format("자재 리스트가 중복 되었습니다.\n SID = '{0}', UID = '{1}'", data.SID, data.UID);
                    Fnc_AlartMessage(str, 1);
                }
            }

            strJudge = AMM_Main.AMM.Delete_PickReadyinfo(strlincode, strPickID);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                Fnc_AlartMessage(str, 1000);
                AMM_Main.strAMM_Connect = "NG";

                return;
            }
            ///Pick ID Info
            ///
            strJudge = AMM_Main.AMM.SetPickingID(strlincode, strequip, strPickID, nReadyMTLcount.ToString(), AMM_Main.strRequestor_id);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                Fnc_AlartMessage(str, 1000);
                AMM_Main.strAMM_Connect = "NG";
            }

            tabControl_Order.SelectedIndex = 2;

            Fnc_Monitor_GetPickingid(AMM_Main.strRequestor_id);

            string strReturn = Fnc_ListCheck();

            if(strReturn == "NO_INFO")
                Fnc_Monitor_GetOutList(strlincode, strPickID);
            else
                Fnc_Monitor_GetOutList(strlincode, strReturn);

            nMonitorIndex = 2;

            AMM_Main.strRequestor_id = "";
            AMM_Main.strRequestor_name = "";

            string strLog = string.Format("PICK LIST 생성 완료 - 사번:{0}, PICKID:{1}, 수량:{2}", label_Requestor.Text, strPickingID, nCount.ToString());
            Fnc_SaveLog(strLog, 1);
        }


        private void Fnc_Picklist_Send_All(string strlincode, string strPickID)
        {
            if (strPickID == "")
            {
                string str = string.Format("배출 ID 정보가 없습니다.");
                Fnc_AlartMessage(str, 1);
                return;
            }
            ///Picklist 생성
            DataTable dt = AMM_Main.AMM.GetPickingReadyinfo_ID(strPickID);

            int nCount = dt.Rows.Count;

            if (nCount == 0)
            {
                string str = string.Format("리스트 생성 목록이 없습니다.");
                Fnc_AlartMessage(str, 1);
                return;
            }

            StorageData data = new StorageData();

            string strJudge = "";


            bool[] bFlag_Group = new bool[3];
            int[] nCount_Group = new int[3];


            for (int i = 0; i < nCount; i++)
            {
                data.Linecode = dt.Rows[i]["LINE_CODE"].ToString(); data.Linecode = data.Linecode.Trim();
                data.Equipid = dt.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.UID = dt.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.Requestor = dt.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();
                data.Tower_no = dt.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.SID = dt.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.LOTID = dt.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = dt.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = dt.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = dt.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = dt.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = dt.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                strJudge = AMM_Main.AMM.SetPicking_Listinfo(strlincode, "TWR"+data.Tower_no.Substring(2,1), strPickID, data.UID, AMM_Main.strRequestor_id, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, "AMM");

                try
                {
                    bFlag_Group[Convert.ToInt32(data.Tower_no.Substring(2, 1)) - 1] = true;
                    nCount_Group[Convert.ToInt32(data.Tower_no.Substring(2, 1)) - 1]++;
                }
                catch
                {

                }

                if (strJudge == "NG")
                {
                    string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                    Fnc_AlartMessage(str, 1000);
                    AMM_Main.strAMM_Connect = "NG";

                    return;
                }
                else if (strJudge == "DUPLICATE")
                {
                    string str = string.Format("자재 리스트가 중복 되었습니다.\n SID = '{0}', UID = '{1}'", data.SID, data.UID);
                    Fnc_AlartMessage(str, 1);
                }
            }
            strJudge = AMM_Main.AMM.Delete_PickReadyinfo(strlincode, strPickID);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                Fnc_AlartMessage(str, 1000);
                AMM_Main.strAMM_Connect = "NG";

                return;
            }
            ///Pick ID Info
            ///
            //strJudge = AMM_Main.AMM.SetPickingID(strlincode, strequip, strPickID, nReadyMTLcount.ToString(), AMM_Main.strRequestor_id);
            if (bFlag_Group[0] == true) strJudge = AMM_Main.AMM.SetPickingID(strlincode, "TWR1", strPickID, nCount_Group[0].ToString(), AMM_Main.strRequestor_id);
            if (bFlag_Group[1] == true) strJudge = AMM_Main.AMM.SetPickingID(strlincode, "TWR2", strPickID, nCount_Group[1].ToString(), AMM_Main.strRequestor_id);
            if (bFlag_Group[2] == true) strJudge = AMM_Main.AMM.SetPickingID(strlincode, "TWR3", strPickID, nCount_Group[2].ToString(), AMM_Main.strRequestor_id);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                Fnc_AlartMessage(str, 1000);
                AMM_Main.strAMM_Connect = "NG";
            }

            tabControl_Order.SelectedIndex = 2;

            Fnc_Monitor_GetPickingid(AMM_Main.strRequestor_id);

            string strReturn = Fnc_ListCheck();

            if (strReturn == "NO_INFO")
                Fnc_Monitor_GetOutList(strlincode, strPickID);
            else
                Fnc_Monitor_GetOutList(strlincode, strReturn);

            nMonitorIndex = 2;

            AMM_Main.strRequestor_id = "";
            AMM_Main.strRequestor_name = "";

            string strLog = string.Format("PICK LIST 생성 완료 - 사번:{0}, PICKID:{1}, 수량:{2}", label_Requestor.Text, strPickingID, nCount.ToString());
            Fnc_SaveLog(strLog, 1);
        }
            public void Fnc_AlartMessage(string strMsg, int nindex)
        {
            string str = "";
            bool bstate = false;
            try
            {
                str = string.Format(strMsg, nindex);
                Frm_Process.Form_Show(str, nindex);
            }
            catch (Exception ex)
            {
                str = ex.ToString();
                bstate = true;
            }

            while (Frm_Process.bState && !bstate)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }
        }

        public void Fnc_Monitor_GetRequest(string strlincode)
        {
            nMonitorIndex = 0;
            label_OutID.Text = "-";
            label_remain.Text = "-";
            label_completetime.Text = "-";

            dataGridView_requestor.Columns.Clear();
            dataGridView_requestor.Rows.Clear();
            dataGridView_requestor.Refresh();

            dataGridView_pickinglist.Columns.Clear();
            dataGridView_pickinglist.Rows.Clear();
            dataGridView_pickinglist.Refresh();

            //dataGridView_Outmonitoring.Columns.Clear();
            //dataGridView_Outmonitoring.Rows.Clear();
            //dataGridView_Outmonitoring.Refresh();


            dataGridView_requestor.Columns.Add("사번", "사번");
            dataGridView_requestor.Columns.Add("이름", "이름");
            dataGridView_requestor.Columns.Add("건수", "건수");

            //GetPickingID_ALL()-query = string.Format(@"SELECT * FROM TB_PICK_ID_INFO WHERE LINE_CODE='{0}'", strLinecode);

            DataTable dt = AMM_Main.AMM.GetPickingID_ALL(strlincode);

            int nCount = dt.Rows.Count;

            if (nCount == 0)
            {
                int nTab = tabControl_Order.SelectedIndex;

                if(nTab == 2)
                {
                    listBox_Outlist.DataSource = null;
                    string str = string.Format("진행 중인 항목이 없습니다.");
                    Fnc_AlartMessage(str, 1);

                    tabControl_Order.SelectedIndex = 0;
                }                

                return;
            }

            List<string> list = new List<string>();
            for(int i = 0; i < nCount; i++)
            {
                string str = dt.Rows[i]["REQUESTOR"].ToString();
                str = str.Trim();

                list.Add(str);
            }

            list.Sort();

            string strName = "";    
            string strSetSid = "";
            int nRequestcount = 0;

            for (int i = 0; i < list.Count; i++)
            {                
                if (strSetSid != list[i])
                {
                    if (strSetSid != "")
                    {
                        dataGridView_requestor.Rows.Add(new object[3] { strSetSid, strName, nRequestcount });

                        strSetSid = list[i];
                        //User_check()-query = string.Format(@"SELECT * FROM TB_USER_INFO WHERE SID='{0}'", strSid)
                        strName = AMM_Main.AMM.User_check(strSetSid);
                        strName = strName.Trim();
                        nRequestcount = 1;
                    }
                    else
                    {
                        strSetSid = list[i];
                        strName = AMM_Main.AMM.User_check(strSetSid);
                        strName = strName.Trim();
                        nRequestcount = 1;
                    }
                }
                else
                {
                    nRequestcount++;
                }

                if (i == list.Count - 1)
                {
                    dataGridView_requestor.Rows.Add(new object[3] { strSetSid, strName, nRequestcount });
                }
            }

            nCount = dataGridView_requestor.RowCount;

            for(int n = 0; n<nCount; n++)
            {
                dataGridView_requestor.Rows[n].Selected = false;
            }
        }

        public void Fnc_Monitor_GetPickingid(string strRequestor)
        {
            dataGridView_pickinglist.Columns.Clear();
            dataGridView_pickinglist.Rows.Clear();
            dataGridView_pickinglist.Refresh();

            //dataGridView_Outmonitoring.Columns.Clear();
            //dataGridView_Outmonitoring.Rows.Clear();
            //dataGridView_Outmonitoring.Refresh();

            dataGridView_pickinglist.Columns.Add("이름", "이름");
            dataGridView_pickinglist.Columns.Add("생성일", "생성일");
            dataGridView_pickinglist.Columns.Add("ID", "ID");
            dataGridView_pickinglist.Columns.Add("수량", "수량");
            dataGridView_pickinglist.Columns.Add("위치", "위치");


            //GetPickingID_Requestor()-query = string.Format(@"SELECT * FROM TB_PICK_ID_INFO WHERE REQUESTOR='{0}'", strRequestor)
            DataTable dt = AMM_Main.AMM.GetPickingID_Requestor(strRequestor);

            int nCount = dt.Rows.Count;

            if (nCount == 0)
            {
                //User Update       
                if(nTabIndex == 2)
                    Fnc_Monitor_GetRequest(AMM_Main.strDefault_linecode);
                return;
            }

            for(int n = 0; n < nCount; n++)
            {
                string strDateTime = dt.Rows[n]["DATETIME"].ToString(); strDateTime = strDateTime.Trim();
                strDateTime = strDateTime.Substring(2, 12);
                string strRe = string.Format("{0}-{1} {2}:{3}:{4}", strDateTime.Substring(2, 2), strDateTime.Substring(4, 2)
                    , strDateTime.Substring(6, 2), strDateTime.Substring(8, 2), strDateTime.Substring(10, 2));

                string strPickid = dt.Rows[n]["PICKID"].ToString(); strPickid = strPickid.Trim();
                string strQty = dt.Rows[n]["QTY"].ToString(); strQty = strQty.Trim();
                string strEquipid = dt.Rows[n]["EQUIP_ID"].ToString(); strEquipid = strEquipid.Trim();

                string strName = AMM_Main.AMM.User_check(strRequestor);
                strName = strName.Trim();

                dataGridView_pickinglist.Rows.Add(new object[5] { strName, strRe, strPickid, strQty, strEquipid });
            }

            dataGridView_pickinglist.Sort(dataGridView_pickinglist.Columns["생성일"], ListSortDirection.Ascending);

            dataGridView_pickinglist.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            dataGridView_pickinglist.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            dataGridView_pickinglist.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView_pickinglist.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView_pickinglist.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        public string Fnc_ListCheck()
        {
            int nCount = dataGridView_pickinglist.RowCount;
            int nRequestor_count = dataGridView_requestor.RowCount;

            if (nCount > 0)
            {
                string strPickid = dataGridView_pickinglist.Rows[0].Cells[2].Value.ToString();
                string strPickName = dataGridView_pickinglist.Rows[0].Cells[0].Value.ToString();

                dataGridView_pickinglist.Rows[0].DefaultCellStyle.ForeColor = Color.White;
                dataGridView_pickinglist.Rows[0].DefaultCellStyle.BackColor = Color.YellowGreen;

                for(int i = 0; i< nCount; i++)
                    dataGridView_pickinglist.Rows[i].Selected = false;

                for(int i = 0; i< nRequestor_count; i++)
                {
                    string strName = dataGridView_requestor.Rows[i].Cells[1].Value.ToString();
                    if (strName == strPickName)
                    {
                        dataGridView_requestor.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                        dataGridView_requestor.Rows[i].DefaultCellStyle.BackColor = Color.YellowGreen;
                    }

                    dataGridView_requestor.Rows[i].Selected = false;
                }

                return strPickid; //PickID
            }
            else
            {
                return "NO_INFO";
            }
        }

        public void Fnc_Monitor_GetOutList(string strLinecode, string strPickingid)
        {
            //dataGridView_Outmonitoring.Columns.Clear();
            //dataGridView_Outmonitoring.Rows.Clear();
            //dataGridView_Outmonitoring.Refresh();

            //dataGridView_Outmonitoring.Columns.Add("NO", "NO");
            //dataGridView_Outmonitoring.Columns.Add("SID", "SID");
            //dataGridView_Outmonitoring.Columns.Add("UID", "UID");
            //dataGridView_Outmonitoring.Columns.Add("위치", "위치");
            //dataGridView_Outmonitoring.Columns.Add("상태", "상태");

            List<string> _item = new List<string>();


            // GetPickingListinfo()- query1 = string.Format(@"SELECT * FROM TB_PICK_LIST_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}' and PICKID='{2}'", strLinecode, strEquipid, strPickingid)
            DataTable dt = AMM_Main.AMM.GetPickingListinfo(strLinecode, strPickingid);

            int nCount = dt.Rows.Count;

            if (nCount == 0)
            {
                /////////////////////////
                ///대기 자재 있는지 확인 후 있으면 Display
                ///Update
                string strReturn = "";

                if (dataGridView_pickinglist.Rows.Count == 0)
                {

                    return;
                }

                string strName = dataGridView_pickinglist.Rows[0].Cells[0].Value.ToString();
                DataTable dtNamme = AMM_Main.AMM.GetUserInfo(strName, 1);

                if(dtNamme.Rows.Count < 1)
                {
                    strReturn = "NO_INFO";
                }
                else
                {
                    nMonitorIndex = 0;
                    string strSid = dtNamme.Rows[0]["SID"].ToString();
                    strSid = strSid.Trim();
                    Fnc_Monitor_GetRequest(AMM_Main.strDefault_linecode);
                    Fnc_Monitor_GetPickingid(strSid);
                    strReturn = Fnc_ListCheck();
                }
                                
                /////////////////////////
                if (strReturn == "NO_INFO")
                {
                    //string str = string.Format("완료 되었습니다.");
                    //Fnc_AlartMessage(str, 0);

                    nMonitorIndex = 0;
                    label_OutID.Text = "-";
                    tabControl_Order.SelectedIndex = 0;
                    return;
                }
                else
                {
                    dt = null;
                    dt = AMM_Main.AMM.GetPickingListinfo(strLinecode, strReturn);
                    nCount = dt.Rows.Count;
                    strPickingid = strReturn;

                    //dataGridView_Outmonitoring.Columns.Clear();
                    //dataGridView_Outmonitoring.Rows.Clear();
                    //dataGridView_Outmonitoring.Refresh();

                    //dataGridView_Outmonitoring.Columns.Add("NO", "NO");
                    //dataGridView_Outmonitoring.Columns.Add("SID", "SID");
                    //dataGridView_Outmonitoring.Columns.Add("UID", "UID");
                    //dataGridView_Outmonitoring.Columns.Add("위치", "위치");
                    //dataGridView_Outmonitoring.Columns.Add("상태", "상태");
                    nMonitorIndex = 2;
                }                
            }

            label_OutID.Text = strPickingid;

            int nReadycount = 0;
            for (int n = 0; n < nCount; n++)
            {    
                string strSid = dt.Rows[n]["SID"].ToString(); strSid = strSid.Trim();
                string strUid = dt.Rows[n]["UID"].ToString(); strUid = strUid.Trim();
                string strTower = dt.Rows[n]["TOWER_NO"].ToString(); strTower = strTower.Trim();
                string strStatus = dt.Rows[n]["STATUS"].ToString(); strStatus = strStatus.Trim();

               // dataGridView_Outmonitoring.Rows.Add(new object[4] { strSid, strUid, strTower, strStatus });

                if (strStatus == "READY")
                {
                    nReadycount++;
                    //dataGridView_Outmonitoring.Rows[n].DefaultCellStyle.ForeColor = Color.Black;
                    //dataGridView_Outmonitoring.Rows[n].DefaultCellStyle.BackColor = Color.White;
                }
                else
                {
                    //dataGridView_Outmonitoring.Rows[n].DefaultCellStyle.ForeColor = Color.White;
                    //dataGridView_Outmonitoring.Rows[n].DefaultCellStyle.BackColor = Color.Green;
                }
                string strList = strSid + " ; " + strStatus + " ; " + strTower + " ; " + strUid;
                _item.Add(strList);
            }

            listBox_Outlist.DataSource = null;
            listBox_Outlist.DataSource = _item;

            // dataGridView_Outmonitoring.Sort(dataGridView_Outmonitoring.Columns["UID"], ListSortDirection.Ascending);
            // dataGridView_Outmonitoring.Rows[0].Selected = false;


            // dataGridView_Outmonitoring.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            // dataGridView_Outmonitoring.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            // dataGridView_Outmonitoring.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            // dataGridView_Outmonitoring.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Fnc_Monitor_Caltime(nReadycount);
        }

        public void Fnc_Monitor_Caltime(int ncount)
        {
            int nCal = ncount * 19;
            int nMin = nCal / 60;
            int nSec = nCal % 60;

            label_remain.Text = string.Format("{0}분 {1}초", nMin, nSec);

            if (nCal == 0)
                return;

            DateTime dToday = DateTime.Now;
            TimeSpan ts = new TimeSpan(0, 0, nCal);

            dToday = dToday + ts;

            label_completetime.Text = dToday.ToString();
        }
        public void Fnc_Monitor_GetReadyInfo(string strlinecode)
        {
            //int[] nCount = new int[6] { 0, 0, 0, 0, 0, 0 };

            //[210805_Sanigk_choi_타워그룹추가
            int[] nCount = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            //]210805_Sanigk_choi_타워그룹추가



            // GetPickingID - query = string.Format(@"SELECT * FROM TB_PICK_ID_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}'", strLinecode, strEquipid);

            DataTable dt = AMM_Main.AMM.GetPickingID(strlinecode, "TWR1"); 
            nCount[0] = dt.Rows.Count;
            dt = null;

            dt = AMM_Main.AMM.GetPickingID(strlinecode, "TWR2");
            nCount[1] = dt.Rows.Count;
            dt = null;

            dt = AMM_Main.AMM.GetPickingID(strlinecode, "TWR3");
            nCount[2] = dt.Rows.Count;
            dt = null;



            string str = string.Format("{0} 건", nCount[0]);

            if (nCount[0] == 0)
                label_G1.BackColor = Color.Blue;
            else
                label_G1.BackColor = Color.Green;
            label_G1.Text = str;

            str = string.Format("{0} 건", nCount[1]);
            if (nCount[1] == 0)
                label_G2.BackColor = Color.Blue;
            else
                label_G2.BackColor = Color.Green;
            label_G2.Text = str;

            str = string.Format("{0} 건", nCount[2]);
            if (nCount[2] == 0)
                label_G3.BackColor = Color.Blue;
            else
                label_G3.BackColor = Color.Green;
            label_G3.Text = str;



            Fnc_DrawDoughnutChart_Ready(nCount[0], nCount[1], nCount[2]);




        }
        public void Fnc_DrawDoughnutChart_Ready(int nGroup1, int nGroup2, int nGroup3) //210823_Sangik.choi_타워그룹추가 
        {
            chart1.Series.Clear();
            chart1.Legends.Clear();

            //Add a new Legend(if needed) and do some formating
            chart1.Legends.Add("MyLegend");
            chart1.Legends[0].LegendStyle = LegendStyle.Table;
            chart1.Legends[0].Docking = Docking.Bottom;
            chart1.Legends[0].Alignment = StringAlignment.Center;
            chart1.Legends[0].Title = "리스트 현황";
            chart1.Legends[0].BorderColor = Color.Black;

            //Add a new chart-series
            string seriesname = "MySeriesName";
            chart1.Series.Add(seriesname);
            //set the chart-type to "Pie"
            chart1.Series[seriesname].ChartType = SeriesChartType.Doughnut;

            //Add some datapoints so the series. in this case you can pass the values to this method

            string strValue1 = string.Format("Group 1 \n{0} EA", nGroup1);
            string strValue2 = string.Format("Group 2 \n{0} EA", nGroup2);
            string strValue3 = string.Format("Group 3 \n{0} EA", nGroup3);



            if (nGroup1 > 0)
                chart1.Series[seriesname].Points.AddXY(strValue1, nGroup1);

            if (nGroup2 > 0)
                chart1.Series[seriesname].Points.AddXY(strValue2, nGroup2);

            if (nGroup3 > 0)
                chart1.Series[seriesname].Points.AddXY(strValue3, nGroup3);

   


            chart1.Series[seriesname].LabelBackColor = Color.Green;
            chart1.Series[seriesname].LabelForeColor = Color.White;

            if (nGroup1 == 0 && nGroup2 == 0 && nGroup3 == 0 )
            {
                strValue1 = string.Format("대기 자재 없음");
                chart1.Series[seriesname].Points.AddXY(strValue1, 100);
                chart1.Series[seriesname].Font = new Font("Calibri", 13.00F, FontStyle.Bold);
                chart1.Series[seriesname].LabelBackColor = Color.Red;
                chart1.Series[seriesname].LabelForeColor = Color.White;                
            }

            chart1.Series[seriesname].Font = new Font("Calibri", 13.00F, FontStyle.Regular);
            chart1.ChartAreas[0].BackColor = Color.FromArgb(224, 224, 224);
            chart1.Legends[0].Enabled = false;
            chart1.ChartAreas[0].Area3DStyle.Enable3D = true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(AMM_Main.bThread_Order)
            {
                if (AMM_Main.IsExit)
                    timer1.Stop();

                Fnc_Monitor_GetReadyInfo(AMM_Main.strDefault_linecode);

                if(nMonitorIndex == 2)
                {
                    if(label_OutID.Text != "" && label_OutID.Text != "-" )
                        Fnc_Monitor_GetOutList(AMM_Main.strDefault_linecode, label_OutID.Text);
                }
                else if(nMonitorIndex == 0 && nTabIndex == 2)
                {
                    Fnc_Monitor_GetRequest(AMM_Main.strDefault_linecode);
                }
            }

            if (AMM_Main.IsExit)
                timer1.Stop();
        }

        private void dataGridView_requestor_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int colIndex = e.ColumnIndex;

            if (colIndex != 0)
                colIndex = 0;

            if (rowIndex == -1)
                return;

            string strsid = dataGridView_requestor.Rows[rowIndex].Cells[0].Value.ToString();

            int nCount = dataGridView_requestor.RowCount;

            if (nCount > 0)
            {
                for (int i = 0; i < nCount; i++)
                {
                    if (i == rowIndex)
                    {
                        dataGridView_requestor.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                        dataGridView_requestor.Rows[i].DefaultCellStyle.BackColor = Color.YellowGreen;
                    }
                    else
                    {
                        dataGridView_requestor.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                        dataGridView_requestor.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    dataGridView_requestor.Rows[i].Selected = false;
                }
            }

            Fnc_Monitor_GetPickingid(strsid);

            nMonitorIndex = 1;
        }

        private void dataGridView_pickinglist_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int colIndex = e.ColumnIndex;

            if (colIndex != 0)
                colIndex = 0;

            if (rowIndex == -1)
                return;

            string strid = dataGridView_pickinglist.Rows[rowIndex].Cells[2].Value.ToString();

            int nCount = dataGridView_pickinglist.RowCount;

            if (nCount > 0)
            {
                for (int i = 0; i < nCount; i++)
                {
                    if (i == rowIndex)
                    {
                        dataGridView_pickinglist.Rows[i].DefaultCellStyle.ForeColor = Color.White;
                        dataGridView_pickinglist.Rows[i].DefaultCellStyle.BackColor = Color.YellowGreen;
                    }
                    else
                    {
                        dataGridView_pickinglist.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                        dataGridView_pickinglist.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    dataGridView_pickinglist.Rows[i].Selected = false;
                }
            }

            Fnc_Monitor_GetOutList(AMM_Main.strDefault_linecode, strid);

            nMonitorIndex = 2;
        }

        public void Fnc_Save_TowerUseInfo()
        {
            string strPath = strSavefilePath + "\\TowerUse_config.ini";

            string text = checkBox_tower1.Checked + ";" + checkBox_tower2.Checked + ";" ;
            System.IO.File.WriteAllText(strPath, text);

            Form_ITS.bTowerUse[0] = checkBox_tower1.Checked;
            Form_ITS.bTowerUse[1] = checkBox_tower2.Checked;
  
        }

        private void Fnc_Load_TowerUseInfo()
        {
            string strPath = strSavefilePath + "\\TowerUse_config.ini";

            if (!File.Exists(strPath))
            {
                checkBox_tower1.Checked = true;
                checkBox_tower2.Checked = true;


                Form_ITS.bTowerUse[0] = checkBox_tower1.Checked;
                Form_ITS.bTowerUse[1] = checkBox_tower1.Checked;
                Form_ITS.bTowerUse[2] = checkBox_tower1.Checked;
                Form_ITS.bTowerUse[3] = checkBox_tower1.Checked;

                return;
            }
            else
            {
                try
                {
                    string[] lines = System.IO.File.ReadAllLines(strPath);
                    int nLength = lines.Length;

                    if (nLength != 0)
                    {
                        string[] strSplit = lines[0].Split(';');
                        if (strSplit[0] == "True")
                            checkBox_tower1.Checked = true;
                        else
                            checkBox_tower1.Checked = false;

                        if (strSplit[1] == "True")
                            checkBox_tower2.Checked = true;
                        else
                            checkBox_tower2.Checked = false;

                    }
                    else
                    {
                        checkBox_tower1.Checked = true;
                        checkBox_tower2.Checked = true;

                    }

                    Form_ITS.bTowerUse[0] = checkBox_tower1.Checked;
                    Form_ITS.bTowerUse[1] = checkBox_tower1.Checked;
                    Form_ITS.bTowerUse[2] = checkBox_tower1.Checked;
                    Form_ITS.bTowerUse[3] = checkBox_tower1.Checked;
                }
                catch
                { }
            }
        }

        public void Fnc_DeleteReady(int nindex)
        {
            string strDeleteUID;
            strDeleteUID = dataGridView_ready.Rows[nindex].Cells[3].Value.ToString();


            //Delete_PickReadyinfo_ReelID()-query = string.Format("DELETE FROM TB_PICK_READY_INFO WHERE LINE_CODE='{0}' and UID='{1}'", strLinecode, strReelid);
            string strJudge = AMM_Main.AMM.Delete_PickReadyinfo_ReelID(AMM_Main.strDefault_linecode, strDeleteUID);

            if (strJudge == "NG")
            {
                AMM_Main.strAMM_Connect = "NG";
                return;
            }

            Fnc_UpdateReadyInfo(strPickingID);
        }
        public void Fnc_Picklist_Comfirm()
        {
            string strPrefix = label_pickid.Text.Substring(0, 2);

            int nCount = dataGridView_ready.Rows.Count;

            if (nCount < 1)
                return;

            for(int n = 0; n<nCount; n++)
            {
                string strPosition = dataGridView_ready.Rows[n].Cells[5].Value.ToString().Substring(2,1);


                if (strPrefix == "PL")
                {
                    if (strPosition != "1")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
                else if (strPrefix == "PM")
                {
                    if (strPosition != "2")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
                else if (strPrefix == "PN")
                {
                    if (strPosition != "3")
                    {
                        Fnc_DeleteReady(n);
                    }
                }


            }
        }

        public void Fnc_SaveLog(string strLog, int nType) ///설비별 개별 로그 저장
        {
            string strPath = "";
            if (nType == 0)
                strPath = AMM_Main.strLogfilePath + "\\AMM_system_";
            else if (nType == 1)
                strPath = AMM_Main.strLogfilePath + "\\AMM_order_";
            else if (nType == 2)
                strPath = AMM_Main.strLogfilePath + "\\AMM_setting_";

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

        private void checkBox_tower1_CheckedChanged(object sender, EventArgs e)
        {
            int n = comboBox_group.SelectedIndex;
            Fnc_Check_TwrUse(n + 1);
        }

        private void textBox_input_sid_Click(object sender, EventArgs e)
        {
            strPadSid = "";

            if (AMM_Main.strNumberPad == "TRUE")
            {
                Form_NumberPad Frm_NumberPad = new Form_NumberPad();
                Frm_NumberPad.nType = 1;
                Frm_NumberPad.ShowDialog();

                textBox_input_sid.Text = strPadSid;

                string strSid = textBox_input_sid.Text.Replace(';', ' ');
                strSid = strSid.Trim();
                string strName = AMM_Main.AMM.User_check(strSid);
                strName = strName.Trim();

                if (strName == "NO_INFO")
                {
                    string str = string.Format("등록 되지 않은 사용자 입니다.\n등록 후 사용 하세요.", 1000);

                    Frm_Process.Form_Show(str, 1000);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }

                    textBox_input_sid.Text = "";
                    return;
                }

                label_Requestor.Text = strSid + " / " + strName;
                AMM_Main.strRequestor_id = strSid;
                AMM_Main.strRequestor_name = strName;

                dataGridView_ready.Columns.Clear();
                dataGridView_ready.Rows.Clear();
                dataGridView_ready.Refresh();

                dataGridView_ready.Columns.Add("No", "N0");
                dataGridView_ready.Columns.Add("SID", "SID");
                dataGridView_ready.Columns.Add("벤더#", "벤더#");
                dataGridView_ready.Columns.Add("UID", "UID");
                dataGridView_ready.Columns.Add("수량", "수량");
                dataGridView_ready.Columns.Add("위치", "위치");
                dataGridView_ready.Columns.Add("인치", "인치");
                dataGridView_ready.Columns.Add("투입", "투입");

                comboBox_group.SelectedIndex = AMM_Main.nDefaultGroup - 1;
                comboBox_method.SelectedIndex = 0;  //SID 조회

                label_stripmark.ForeColor = System.Drawing.Color.White;
                textBox_stripmark.Enabled = false;

                label_reelid.ForeColor = System.Drawing.Color.White;
                textBox_reelid.Enabled = false;

                label_sid.ForeColor = System.Drawing.Color.Black;
                textBox_sid.Enabled = true;

                textBox_input_sid.Clear();
                textBox_input_sid.Text = "OK";
                tabControl_Order.SelectedIndex = 1;

                textBox_sid.Focus();

                nReadyMTLcount = 0;

                //Fnc_Load_TowerUseInfo();
                Fnc_Get_PickID(AMM_Main.nDefaultGroup.ToString());

                string strLog = string.Format("PICK LIST 생성 시작 - 사번:{0}, PICKID:{1}", label_Requestor.Text, strPickingID);
                Fnc_SaveLog(strLog, 1);
            }
        }

        private void textBox_sid_Click(object sender, EventArgs e)
        {
            if (AMM_Main.strNumberPad == "TRUE")
            {
                Form_NumberPad Frm_NumberPad = new Form_NumberPad();
                Frm_NumberPad.nType = 2;
                Frm_NumberPad.ShowDialog();

                textBox_sid.Text = strPadReelSid;

                if (comboBox_method.SelectedIndex == 0)
                {
                    string strSid = "";
                    int nLength = 0;

                    strSid = textBox_sid.Text;
                    nLength = strSid.Length;

                    if (nLength < 3 || nLength > 9)
                    {
                        return;
                    }

                    ///SID 정보 가져 오기
                    int nGroup = comboBox_group.SelectedIndex;

                    ///HY20201124                    
                    if (AMM_Main.bTAlarm[nGroup])
                    {
                        string str = string.Format("타워 그룹 {0} 이 알람 상태 입니다.\n알람 해제를 요청 하세요\n\n리스트 생성은 가능 합니다.", nGroup + 1);
                        Frm_Process.Form_Show(str, 1000);

                        while (Frm_Process.bState)
                        {
                            Application.DoEvents();
                            Thread.Sleep(1);
                        }
                    }
                    ///////

                    string strGroup = (nGroup + 1).ToString();

                    if (strSid == "")
                        return;

                    Fnc_SetMtlInfo_FromSID(AMM_Main.strDefault_linecode, strGroup, strSid, false);

                    string strLog = string.Format("릴 조회 - 사번:{0}, PICKID:{1}, SID:{2}", label_Requestor.Text, strPickingID, strSid);
                    Fnc_SaveLog(strLog, 1);
                }
                else if (comboBox_method.SelectedIndex == 1)
                {
                    string strSM = "";
                    int nLength = 0;

                    strSM = textBox_stripmark.Text;
                    nLength = strSM.Length;

                    if (nLength < 3)
                        return;

                    textBox_stripmark.Clear();
                    //Fnc_ViewLotlist(strSM);
                }
                else if (comboBox_method.SelectedIndex == 2)
                {//UID 정보 가져 오기
                    string strUid = "";

                    strUid = textBox_reelid.Text;

                    int nGroup = comboBox_group.SelectedIndex;

                    ///HY20201124
                    if (AMM_Main.bTAlarm[nGroup])
                    {
                        string str = string.Format("타워 그룹 {0} 이 알람 상태 입니다.\n알람 해제를 요청 하세요\n\n리스트 생성은 가능 합니다.", nGroup + 1);
                        Frm_Process.Form_Show(str, 1000);

                        while (Frm_Process.bState)
                        {
                            Application.DoEvents();
                            Thread.Sleep(1);
                        }
                    }
                    ///////

                    string strGroup = (nGroup + 1).ToString();

                    int nReturn = Fnc_GetMtlInfo_ReelID(AMM_Main.strDefault_linecode, strGroup, strUid, false);

                    if (nReturn == 0)  //OK
                    {
                        textBox_reelcount.Focus();
                    }
                    else if (nReturn == 1) //다른 그룹에 있음
                    {
                        comboBox_group.Focus();
                    }
                    else if (nReturn == 2) //자재 없음
                    {
                        textBox_sid.Clear();
                        textBox_sid.Focus();
                    }
                }
            }
        }

        private void textBox_reelcount_Click(object sender, EventArgs e)
        {
            if (AMM_Main.strNumberPad == "TRUE")
            {
                Form_NumberPad Frm_NumberPad = new Form_NumberPad();
                Frm_NumberPad.nType = 3;
                Frm_NumberPad.ShowDialog();

                textBox_reelcount.Text = strPadReelqty;

                if (textBox_reelcount.Text == "")
                    return;

                int nRowcount = dataGridView_view.Rows.Count;

                if (nRowcount < 1)
                {
                    textBox_reelcount.Text = "";
                    return;
                }

                int nIndex = dataGridView_view.CurrentCell.RowIndex;

                //string strNo = dataGridView_view.Rows[nIndex].Cells[0].Value.ToString();

                strSelSid = dataGridView_view.Rows[nIndex].Cells[0].Value.ToString();
                strSelLotid = dataGridView_view.Rows[nIndex].Cells[1].Value.ToString();

                //strSelUid = dataGridView_view.Rows[nIndex].Cells[3].Value.ToString();

                ///
                int nMethod = comboBox_method.SelectedIndex;
                if (nMethod == 0)
                    strKeepingcount = dataGridView_view.Rows[nIndex].Cells[2].Value.ToString();
                else if (nMethod == 1)
                    strKeepingcount = dataGridView_view.Rows[nIndex].Cells[2].Value.ToString();  // 추가 확인이 필요함.
                else if (nMethod == 2)
                {
                    strSelUid = dataGridView_view.Rows[nIndex].Cells[2].Value.ToString();
                    strKeepingcount = dataGridView_view.Rows[nIndex].Cells[3].Value.ToString();
                }

                int nRequestcount = Int32.Parse(textBox_reelcount.Text);
                int nKeepCount = Int32.Parse(strKeepingcount);

                //if(strNo == "" || nRequestcount == 0)
                if (nRequestcount == 0)
                {
                    string str = string.Format("수량을 입력 하여 주십시오", 1);
                    Frm_Process.Form_Show(str, 1);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }

                    textBox_reelcount.Text = "";
                    textBox_reelcount.Focus();
                    return;
                }

                if (nRequestcount > nKeepCount)
                {
                    string str = string.Format("보유 수량 보다 요청 수량이 많습니다.\n다시 입력 하여 주십시오", 1);
                    Frm_Process.Form_Show(str, 1);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }

                    textBox_reelcount.Text = (nRequestcount - nKeepCount).ToString();
                    textBox_reelcount.Focus();
                    return;
                }

                int nCheckcount = nReadyMTLcount + nRequestcount;
                if (nCheckcount > 20)
                {
                    string str = string.Format("배출 수량이 너무 많습니다.20개 초과!\n한 개 리스트에 자재 20개 까지 담을 수 있습니다.", 1);
                    Frm_Process.Form_Show(str, 1);

                    while (Frm_Process.bState)
                    {
                        Application.DoEvents();
                        Thread.Sleep(1);
                    }
                    textBox_reelcount.Text = (nCheckcount - 20).ToString();
                    textBox_reelcount.Focus();
                    return;
                }

                ////자재 업데이트
                int nGroup = comboBox_group.SelectedIndex;
                string strGroup = (nGroup + 1).ToString();

                if (nMethod != 2)
                    Fnc_RequestMaterial(AMM_Main.strDefault_linecode, strGroup, strSelSid, strSelLotid, nRequestcount, strPickingID);
                else
                    Fnc_RequestMaterial_uid(AMM_Main.strDefault_linecode, strGroup, strSelUid, nRequestcount, strPickingID);

                textBox_reelcount.Text = "";

                if (nMethod == 0) //SID
                {
                    //bool lastmtl = false;
                    //if (nRequestcount == nKeepCount)
                    //    lastmtl = true;

                    Fnc_SetMtlInfo_FromSID(AMM_Main.strDefault_linecode, strGroup, strSelSid, true);

                    textBox_sid.Text = "";
                    textBox_sid.Focus();
                }
                else if (nMethod == 1) //S/M
                {
                    textBox_stripmark.Focus();
                }
                else if (nMethod == 2) //Reel ID
                {
                    dataGridView_view.Columns.Clear();
                    dataGridView_view.Rows.Clear();
                    dataGridView_view.Refresh();

                    textBox_reelid.Text = "";
                    textBox_reelid.Focus();
                }

                Fnc_Picklist_Comfirm();
            }
        }

        public void Fnc_View_Monitor_SM(string strLine, string strRequestor)
        {
            tabControl_Order.SelectedIndex = 2;

            Fnc_Monitor_GetPickingid(strRequestor);

            string strReturn = Fnc_ListCheck();

            Fnc_Monitor_GetOutList(strLine, strReturn);

            nMonitorIndex = 2;

            AMM_Main.strRequestor_id = "";
            AMM_Main.strRequestor_name = "";

            string strLog = string.Format("PICK LIST 생성 완료 - 사번:{0} , SM신청", strRequestor);
            Fnc_SaveLog(strLog, 1);
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button_All_Del_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult1 = MessageBox.Show("대기 List를 삭제 하시겠습니까?", "List Delete", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.Yes)
            {
                if (nReadyMTLcount == 0)
                    return;

                string strDeleteUID;
                for (int i = 0; i < dataGridView_ready.RowCount; i++)
                {
                    strDeleteUID = dataGridView_ready.Rows[i].Cells[3].Value.ToString();

                    string strJudge = AMM_Main.AMM.Delete_PickReadyinfo_ReelID(AMM_Main.strDefault_linecode, strDeleteUID); if (strJudge == "NG")
                    {
                        AMM_Main.strAMM_Connect = "NG";
                        break;
                    }
                }

                Fnc_UpdateReadyInfo(strPickingID);
            }

        }
    }

    public class StorageData
    {
        public string Linecode = "";
        public string Equipid = "";
        public string Input_date = "";
        public string Tower_no = "";
        public string UID = "";
        public string SID = "";
        public string LOTID = "";
        public string Quantity = "";
        public string Manufacturer = "";
        public string Production_date = "";
        public string Inch = "";
        public string Input_type = "";
        public string Requestor = "";
    }

    public class EventData
    {
        public string date = "";
        public string code = "";
        public string type = "";
        public string name = "";
        public string descript = "";
        public string action = "";
    }
}
