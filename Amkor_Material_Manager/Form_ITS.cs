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
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms.DataVisualization.Charting;
using System.Security.Cryptography;

namespace Amkor_Material_Manager
{
    public partial class Form_ITS : Form
    {
        int nnTabIndex = 0;

        //Excel
        public static bool[] bExcelUse = new bool[5] { true, true, true, true, true }; //Excel 변환 작업 수앻 List

        //public static bool[] bGroupUse = new bool[6] { true, true, true, true, true, true }; 
        public static bool[] bGroupUse = new bool[5] { true, true, true, true, true }; //220829_ilyoung_타워그룹추가

        public static bool[] bTowerUse = new bool[4] { true, true, true, true};
        public static bool bExcel_Start = false;
        public string strExcelfilePath = "";
        public static int nExcelIndex = 0;

        //timeset
        public static string strTimeset_date_st = "", strTimeset_date_ed = "";
        public static string strTimeset_hour_st = "", strTimeset_hour_ed = "";
        public static string strTimeset_Min_st = "", strTimeset_Min_ed = "";
        public static bool bSearch_sid = false;
        ///////

        public static bool IsDateGathering = false;
        public static bool bUpdate_Timer = false;

        public int nSum = 0;

        ///ASM DB
        public MsSqlManager MSSql = null;
        bool bASMconnect = false;
        string strASM_TowerLocation1 = "", strASM_TowerLocation2 = "", strASM_TowerLocation3 = "";
        int nDbUpdate = -1;


        struct divde_inch
        {
            public bool bFlag;
            public int nReelCount;
            public string strLocation;
            public double nQty;
        }




        public Form_ITS()
        {
            InitializeComponent();

            Fnc_Init();
            timer1.Start();
        }

        public void Fnc_Init()
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.StartupPath + @"\Excel");
            
            if (!di.Exists) { di.Create(); }
            strExcelfilePath = di.ToString();

            comboBox_searchtype.SelectedIndex = 0;
            comboBox_type.SelectedIndex = 0;
            comboBox_group.SelectedIndex = AMM_Main.nDefaultGroup - 1;

            comboBox_type2.SelectedIndex = 0;
            comboBox_all.SelectedIndex = 0;

            comboBox_group2.SelectedIndex = AMM_Main.nDefaultGroup - 1;

            tabControl_ITS.SelectedIndex = 0;

/*            strASM_TowerLocation1 = "Amkor.B-Line.S10-2_Kitting.20_Material_Tower";
            strASM_TowerLocation2 = "Amkor.B-Line.S10-2_Kitting.20_Material_Tower2";
            strASM_TowerLocation3 = "Amkor.B-Line.S10-2_Kitting.20_Material_Tower3";

            if(AMM_Main.strMatchTab == "TRUE")
            {
                Fnc_InitMSSql();
            }*/
        }

        private void tabControl_ITS_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tabNo = tabControl_ITS.SelectedIndex;

            nnTabIndex = tabNo;

            //[210813_Sangik.choi_장기보관관리기능추가 by이종명수석님
            int listk_count =  dataGridView_LTlist.Rows.Count;
            string strPickingID = label_pickid_LT.Text;
            //]210813_Sangik.choi_장기보관관리기능추가 by이종명수석님


            if (tabNo == 0)
            {
                comboBox_type.SelectedIndex = 0;
                comboBox_group.SelectedIndex = AMM_Main.nDefaultGroup - 1;

                Fnc_Process_CalMaterialInfo();

                bUpdate_Timer = true;

                //[210813_Sangik.choi_장기보관관리기능추가 by이종명수석님
                textBox_badge.Text = "";


                if (listk_count != 0)
                {
                    AMM_Main.AMM.Delete_PickReadyinfo(AMM_Main.strDefault_linecode, strPickingID);
                }
                //]210813_Sangik.choi_장기보관관리기능추가 by이종명수석님


            }
            //[210818_Sangik.choi_capa 조회 탭 추가 by이종명수석님
            else if (tabNo == 1)
            {
                bUpdate_Timer = true;

                if (listk_count != 0)
                {
                    AMM_Main.AMM.Delete_PickReadyinfo(AMM_Main.strDefault_linecode, strPickingID);
                }
                Fnc_Init_datagrid_capa();


            }
            //]210818_Sangik.choi_capa 조회 탭 추가 by이종명수석님

            else if (tabNo == 2)
            {
                //[210813_Sangik.choi_장기보관관리기능추가 by이종명수석님

                if (listk_count != 0)
                {
                    AMM_Main.AMM.Delete_PickReadyinfo(AMM_Main.strDefault_linecode, strPickingID);
                }
                //]210813_Sangik.choi_장기보관관리기능추가 by이종명수석님
                textBox_badge.Text = "";

                comboBox_type2.SelectedIndex = 0;
                comboBox_group2.SelectedIndex = AMM_Main.nDefaultGroup - 1;

                button_search.Visible = false;
                textBox_sid.Visible = false;
                label_sid.Visible = false;
                textBox_sid.Text = "";

                Application.DoEvents();

                Fnc_Update_timeset();

                comboBox_group2_SelectedIndexChanged(sender, e);

                bUpdate_Timer = false;
            }
/*            else if(tabNo == 3)
            {
                //[210813_Sangik.choi_장기보관관리기능추가 by이종명수석님

                if (listk_count != 0)
                {
                    AMM_Main.AMM.Delete_PickReadyinfo(AMM_Main.strDefault_linecode, strPickingID);
                }
                textBox_badge.Text = "";

                //]210813_Sangik.choi_장기보관관리기능추가 by이종명수석님
                bUpdate_Timer = false;

                if (bASMconnect == false)
                {
                    MessageBox.Show("해당 Tab은 사용 하실 수 없습니다.");
                    tabControl_ITS.SelectedIndex = 0;
                }
                comboBox_sel.SelectedIndex = 0;
            }*/

            //[210806_Sangik.choi_장기보관관리기능추가 by이종명수석님

            else if (tabNo == 3)
            {
                textBox_badge.Text = "";

                Fnc_Process_LongtermInfo();

                bUpdate_Timer = false;
            }

            //]210806_Sangik.choi_장기보관관리기능추가 by이종명수석님

        }

        //[210818_Sangik.choi_capa 조회 탭 추가 by이종명수석님

        private void Fnc_Init_datagrid_capa()
        {
            List<DataGridView> list = new List<DataGridView>();

            list.Add(dataGridView_group1);
            list.Add(dataGridView_group2);
            list.Add(dataGridView_group3);
            //220829_ilyoung_타워그룹추가
            list.Add(dataGridView_group4);
            list.Add(dataGridView_group5);
            //220829_ilyoung_타워그룹추가


            for ( int i = 0; i<list.Count; i++)
            {
                list[i].Columns.Clear();
                list[i].Rows.Clear();
                list[i].Refresh();

                list[i].Columns.Add("Capa", "Capa");
                list[i].Columns.Add("현재 수량", "현재 수량");
                list[i].Columns.Add("입고 가능 수량", "입고 가능 수량");
                list[i].Columns.Add("적재율(%)", "적재율(%)");
                list[i].AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            }

            var MtlList = AMM_Main.AMM.Get_Capa_inch();

            string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            label_update_capa.Text = "최근 업데이트: " + strToday + " " + strHead;

            int nMtlCount = MtlList.Rows.Count;




            if (MtlList.Rows.Count == 0)
            {
                MessageBox.Show("DB 연결 실패");
                return;
            }

            List<Inchdata> inch_list = new List<Inchdata>();
            int Tot7InchCnt = 0;
            int Tot13InchCnt = 0;
            int Tot7InchCapa = 0;
            int Tot13InchCapa = 0;


            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                Inchdata data = new Inchdata();

                data.Equipid = MtlList.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.Inch_7_cnt = MtlList.Rows[i]["INCH_7_CNT"].ToString(); data.Inch_7_cnt = data.Inch_7_cnt.Trim();

                
                data.Inch_13_cnt = MtlList.Rows[i]["INCH_13_CNT"].ToString(); data.Inch_13_cnt = data.Inch_13_cnt.Trim();
                data.Inch_7_capa = MtlList.Rows[i]["INCH_7_CAPA"].ToString(); data.Inch_7_capa = data.Inch_7_capa.Trim();
                data.Inch_13_capa = MtlList.Rows[i]["INCH_13_CAPA"].ToString(); data.Inch_13_capa = data.Inch_13_capa.Trim();
                data.Inch_7_rate = MtlList.Rows[i]["INCH_7_LOAD_RATE"].ToString(); data.Inch_7_rate = data.Inch_7_rate.Trim();
                data.Inch_13_rate = MtlList.Rows[i]["INCH_13_LOAD_RATE"].ToString(); data.Inch_13_rate = data.Inch_13_rate.Trim();
                

                Tot7InchCnt += int.Parse(data.Inch_7_cnt == "" ? "0" : data.Inch_7_cnt);    //220829_ilyoung_타워그룹추가
                Tot13InchCnt += int.Parse(data.Inch_13_cnt == "" ? "0" : data.Inch_13_cnt); //220829_ilyoung_타워그룹추가
                Tot7InchCapa += int.Parse(data.Inch_7_capa == "" ? "0" : data.Inch_7_capa); //220829_ilyoung_타워그룹추가
                Tot13InchCapa += int.Parse(data.Inch_13_capa == "" ? "0" : data.Inch_13_capa);  //220829_ilyoung_타워그룹추가

                string inch_7_cal = (Int32.Parse(data.Inch_7_capa == "" ? "0" : data.Inch_7_capa) - Int32.Parse(data.Inch_7_cnt == "" ? "0" : data.Inch_7_cnt)).ToString(); //220829_ilyoung_타워그룹추가
                string inch_13_cal = (Int32.Parse(data.Inch_13_capa == "" ? "0" : data.Inch_13_capa) - Int32.Parse(data.Inch_13_cnt == "" ? "0" : data.Inch_13_cnt)).ToString();    //220829_ilyoung_타워그룹추가

                if(int.Parse(inch_7_cal) < 0)
                {
                    inch_13_cal = (int.Parse(inch_13_cal) + int.Parse(inch_7_cal)).ToString();
                    data.Inch_13_rate = (double.Parse((int.Parse(data.Inch_13_capa) - int.Parse(inch_13_cal)).ToString()) / double.Parse(data.Inch_13_capa) * 100).ToString("F2");
                }


                list[i].Rows.Add(new object[4] {  data.Inch_7_capa, data.Inch_7_cnt, inch_7_cal, data.Inch_7_rate });
                list[i].Rows.Add(new object[4] {  data.Inch_13_capa, data.Inch_13_cnt, inch_13_cal, data.Inch_13_rate });

                list[i].Rows[0].HeaderCell.Value = "7\"";
                list[i].Rows[1].HeaderCell.Value = "13\"";

                list[i].Rows[0].Cells[2].Style.ForeColor = Color.Red;
                list[i].Rows[1].Cells[2].Style.ForeColor = Color.Red;

            }



        }

        //]210818_Sangik.choi_capa 조회 탭 추가 by이종명수석님


        private void Fnc_Init_datagrid(int nNum)
        {
            if (nNum == 0)
            {
                dataGridView_info.Columns.Clear();
                dataGridView_info.Rows.Clear();
                dataGridView_info.Refresh();

                dataGridView_info.Columns.Add("NO", "NO");
                dataGridView_info.Columns.Add("SID", "SID");
                dataGridView_info.Columns.Add("릴 수량", "릴 수량");
                dataGridView_info.Columns.Add("Qty", "Qty");
                dataGridView_info.Columns.Add("인치", "인치");
                dataGridView_info.Columns.Add("위치", "위치");
            }
            else if (nNum == 1)
            {
                dataGridView_info.Columns.Clear();
                dataGridView_info.Rows.Clear();
                dataGridView_info.Refresh();

                dataGridView_info.Columns.Add("NO", "NO");
                dataGridView_info.Columns.Add("SID", "SID");
                dataGridView_info.Columns.Add("Batch#", "Batch#");
                dataGridView_info.Columns.Add("UID", "UID");
                dataGridView_info.Columns.Add("Qty", "Qty");                
                dataGridView_info.Columns.Add("투입형태", "투입형태");
                dataGridView_info.Columns.Add("위치", "위치");
                dataGridView_info.Columns.Add("제조일", "제조일");
                dataGridView_info.Columns.Add("투입일", "투입일");
                dataGridView_info.Columns.Add("제조사", "제조사");
                dataGridView_info.Columns.Add("인치", "인치");
            }
            else
            {
                dataGridView_sum.Columns.Clear();
                dataGridView_sum.Rows.Clear();
                dataGridView_sum.Refresh();

                dataGridView_sum.Columns.Add("TWR", "TWR");
                dataGridView_sum.Columns.Add("GROUP #1", "GROUP #1");
                dataGridView_sum.Columns.Add("GROUP #2", "GROUP #2");
                dataGridView_sum.Columns.Add("GROUP #3", "GROUP #3");
                dataGridView_sum.Columns.Add("GROUP #4", "GROUP #4");   //220829_ilyoung_타워그룹추가
                dataGridView_sum.Columns.Add("GROUP #5", "GROUP #5");   //220829_ilyoung_타워그룹추가


            }
        }

        private void Fnc_Init_datagrid2(int nNum)
        {
            label_incount.Text = "-";
            label_returncount.Text = "-";
            label_outcount.Text = "-";

            if (nNum == 0)
            {
                dataGridView_input.Columns.Clear();
                dataGridView_input.Rows.Clear();
                dataGridView_input.Refresh();

                dataGridView_input.Columns.Add("NO", "NO");
                dataGridView_input.Columns.Add("SID", "SID");
                dataGridView_input.Columns.Add("릴 수량", "릴 수량");
                dataGridView_input.Columns.Add("Qty", "Qty");
                dataGridView_input.Columns.Add("인치", "인치");

                dataGridView_return.Columns.Clear();
                dataGridView_return.Rows.Clear();
                dataGridView_return.Refresh();

                dataGridView_return.Columns.Add("NO", "NO");
                dataGridView_return.Columns.Add("SID", "SID");
                dataGridView_return.Columns.Add("릴 수량", "릴 수량");
                dataGridView_return.Columns.Add("Qty", "Qty");
                dataGridView_return.Columns.Add("인치", "인치");

                dataGridView_output.Columns.Clear();
                dataGridView_output.Rows.Clear();
                dataGridView_output.Refresh();

                dataGridView_output.Columns.Add("NO", "NO");
                dataGridView_output.Columns.Add("SID", "SID");
                dataGridView_output.Columns.Add("릴 수량", "릴 수량");
                dataGridView_output.Columns.Add("Qty", "Qty");
                dataGridView_output.Columns.Add("인치", "인치");
            }
            else if (nNum == 1)
            {
                dataGridView_input.Columns.Clear();
                dataGridView_input.Rows.Clear();
                dataGridView_input.Refresh();

                dataGridView_input.Columns.Add("NO", "NO");
                dataGridView_input.Columns.Add("일자", "일자");
                dataGridView_input.Columns.Add("시간", "시간");
                dataGridView_input.Columns.Add("SID", "SID");
                dataGridView_input.Columns.Add("Batch#", "Batch#");
                dataGridView_input.Columns.Add("UID", "UID");
                dataGridView_input.Columns.Add("Qty", "Qty");
                dataGridView_input.Columns.Add("투입형태", "투입형태");
                dataGridView_input.Columns.Add("위치", "위치");
                dataGridView_input.Columns.Add("제조일", "제조일");
                dataGridView_input.Columns.Add("제조사", "제조사");
                dataGridView_input.Columns.Add("인치", "인치");

                dataGridView_return.Columns.Clear();
                dataGridView_return.Rows.Clear();
                dataGridView_return.Refresh();

                dataGridView_return.Columns.Add("NO", "NO");
                dataGridView_return.Columns.Add("일자", "일자");
                dataGridView_return.Columns.Add("시간", "시간");
                dataGridView_return.Columns.Add("SID", "SID");
                dataGridView_return.Columns.Add("Lot#", "Lot#");
                dataGridView_return.Columns.Add("UID", "UID");
                dataGridView_return.Columns.Add("Qty", "Qty");
                dataGridView_return.Columns.Add("투입형태", "투입형태");
                dataGridView_return.Columns.Add("위치", "위치");
                dataGridView_return.Columns.Add("제조일", "제조일");
                dataGridView_return.Columns.Add("제조사", "제조사");
                dataGridView_return.Columns.Add("인치", "인치");

                dataGridView_output.Columns.Clear();
                dataGridView_output.Rows.Clear();
                dataGridView_output.Refresh();

                dataGridView_output.Columns.Add("NO", "NO");
                dataGridView_output.Columns.Add("일자", "일자");
                dataGridView_output.Columns.Add("시간", "시간");
                dataGridView_output.Columns.Add("SID", "SID");
                dataGridView_output.Columns.Add("Batch#", "Batch#");
                dataGridView_output.Columns.Add("UID", "UID");
                dataGridView_output.Columns.Add("수량", "수량");
                dataGridView_output.Columns.Add("인치", "인치");
                dataGridView_output.Columns.Add("배출ID", "배출ID");
                dataGridView_output.Columns.Add("요청자", "요청자");
                dataGridView_output.Columns.Add("위치", "위치");
                dataGridView_output.Columns.Add("Type", "Type");
            }
        }



        //[210806_Sangik.choi_장기보관관리기능추가 by이종명수석님
        private void Fnc_Init_datagrid_longterm()
        {
            dataGridView_longterm.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView_longterm.Columns.Clear();
            dataGridView_longterm.Rows.Clear();
            dataGridView_longterm.Refresh();

            dataGridView_longterm.Columns.Add("SID", "SID");
            dataGridView_longterm.Columns.Add("Batch#", "Batch#");
            dataGridView_longterm.Columns.Add("UID", "UID");
            dataGridView_longterm.Columns.Add("Qty", "Qty");
            dataGridView_longterm.Columns.Add("투입형태", "투입형태");
            dataGridView_longterm.Columns.Add("위치", "위치");
            dataGridView_longterm.Columns.Add("제조일", "제조일");
            dataGridView_longterm.Columns.Add("투입일", "투입일");
            dataGridView_longterm.Columns.Add("제조사", "제조사");
            dataGridView_longterm.Columns.Add("인치", "인치");

            dataGridView_LTlist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView_LTlist.Columns.Clear();
            dataGridView_LTlist.Rows.Clear();
            dataGridView_LTlist.Refresh();

            dataGridView_LTlist.Columns.Add("SID", "SID");
            dataGridView_LTlist.Columns.Add("Batch#", "Batch#");
            dataGridView_LTlist.Columns.Add("UID", "UID");
            dataGridView_LTlist.Columns.Add("Qty", "Qty");
            dataGridView_LTlist.Columns.Add("투입형태", "투입형태");
            dataGridView_LTlist.Columns.Add("위치", "위치");
            dataGridView_LTlist.Columns.Add("제조일", "제조일");
            dataGridView_LTlist.Columns.Add("투입일", "투입일");
            dataGridView_LTlist.Columns.Add("제조사", "제조사");
            dataGridView_LTlist.Columns.Add("인치", "인치");

            label_pickid_LT.Text = "";
            label_count.Text = "";
        }
        //]210806_Sangik.choi_장기보관관리기능추가 by이종명수석님


        //[210806_Sangik.choi_장기보관관리기능추가 by이종명수석님

        public void Fnc_Process_LongtermInfo()
        {
            // IsDateGathering = true;
            Fnc_Init_datagrid_longterm();

            Application.DoEvents();

            comboBox_month.SelectedIndex = 0;
            comboBox_L_group.SelectedIndex = 0;


            int nMonth = comboBox_month.SelectedIndex; //0: SID, 1:Detail info
            int nGroup = comboBox_L_group.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();
            textBox_badge.Text = "";

 /*           if (nGroup != 7)
                Fnc_Process_GetMaterialinfo_longterm(1, strEquipid);
            else
            {
                Fnc_Process_GetMaterialinfo_All(1);
            }*/

            //IsDateGathering = false;
        }

        //]210806_Sangik.choi_장기보관관리기능추가 by이종명수석님


        public void Fnc_Process_CalMaterialInfo()
        {
            IsDateGathering = true;

            try
            {
                Fnc_Init_datagrid(2); //Init

                Application.DoEvents();

                int[] nCount = new int[] { 0, 0, 0, 0, 0};//210831_Sangik.choi_타워그룹추가	//220829_ilyoung_타워그룹추가

                DataTable MtlList = null;            

                string strTowerNo = "", strEquip = "";
                for (int n = 1; n < 5; n++) //220829_ilyoung_타워그룹추가
                {
                    strEquip = "TWR1"; strTowerNo = string.Format("T010{0}", n.ToString());
                    //GetMTLInfo()-query = string.Format(@"SELECT * FROM TB_MTL_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}' and TOWER_NO='{2}');
                    MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo); nCount[0] = MtlList.Rows.Count; MtlList = null;

                    strEquip = "TWR2"; strTowerNo = string.Format("T020{0}", n.ToString());
                    MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo); nCount[1] = MtlList.Rows.Count; MtlList = null;

                    strEquip = "TWR3"; strTowerNo = string.Format("T030{0}", n.ToString());
                    MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo); nCount[2] = MtlList.Rows.Count; MtlList = null;

                    //220829_ilyoung_타워그룹추가
                    strEquip = "TWR4"; strTowerNo = string.Format("T040{0}", n.ToString());
                    MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo); nCount[3] = MtlList.Rows.Count; MtlList = null;

                    strEquip = "TWR5"; strTowerNo = string.Format("T050{0}", n.ToString());
                    MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquip, strTowerNo); nCount[4] = MtlList.Rows.Count; MtlList = null;
                    //220829_ilyoung_타워그룹추가

                    dataGridView_sum.Rows.Add(new object[] { n.ToString(), nCount[0].ToString(), nCount[1].ToString(), nCount[2].ToString(), nCount[3].ToString(), nCount[4].ToString() });//220829_ilyoung_타워그룹추가
                    //]210831_Sangik.choi_타워그룹추가
                }

                int[] nSum = new int[] { 0, 0, 0, 0, 0 };//210831_Sangik.choi_타워그룹추가//220829_ilyoung_타워그룹추가
                string[] strSum = new string[] { "", "", "", "", "" };//210831_Sangik.choi_타워그룹추가//220829_ilyoung_타워그룹추가
                int nTotal = 0;

                for (int j = 0; j < nSum.Length; j++)//210831_Sangik.choi_타워그룹추가  	//220829_ilyoung_타워그룹추가
                {
                    for (int i = 0; i < dataGridView_sum.Rows.Count; i++)     	//220829_ilyoung_타워그룹추가
                    {                    
                        int nCal = dataGridView_sum.Rows[i].Cells[j + 1].Value == null ? 0 : Int32.Parse(dataGridView_sum.Rows[i].Cells[j+1].Value.ToString());//220829_ilyoung_타워그룹추가
                        nSum[j] = nSum[j] + nCal;
                    }

                    strSum[j] = string.Format("{0:0,0}", nSum[j]);
                    nTotal = nTotal + nSum[j];
                }

                dataGridView_sum.Rows.Add(new object[6] {"SUM", strSum[0].ToString(), strSum[1].ToString(), strSum[2].ToString(), strSum[3].ToString(), strSum[4].ToString() });//210831_Sangik.choi_타워그룹추가 //220829_ilyoung_타워그룹추가
                dataGridView_sum.Rows[dataGridView_sum.RowCount-1].DefaultCellStyle.ForeColor = Color.White;
                dataGridView_sum.Rows[dataGridView_sum.RowCount - 1].DefaultCellStyle.BackColor = Color.OrangeRed;
                dataGridView_sum.Rows[dataGridView_sum.RowCount - 1].DefaultCellStyle.Font = new Font("Calibri", 16.00F, FontStyle.Bold);
                dataGridView_sum.Rows[0].Selected = false;
                dataGridView_sum.Rows[dataGridView_sum.RowCount - 1].Selected = false;

                string strnQty = string.Format("{0:0,0}", nTotal);
                label_total.Text = strnQty + " REEL";

                string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                label_updatedate.Text = "최근 업데이트: " + strToday + " " + strHead;

                //////Infomation
                int nType = comboBox_type.SelectedIndex; //0: SID, 1:Detail info
                int nGroup = comboBox_group.SelectedIndex + 1;

                string strEquipid = "TWR" + nGroup.ToString();

                Fnc_Init_datagrid(nType);

                if (nGroup != nCount.Length +1 )//210831_Sangik.choi_타워그룹추가 //220829_ilyoung_타워그룹추가
                    Fnc_Process_GetMaterialinfo(nType, strEquipid);
                else
                {
                    Fnc_Process_GetMaterialinfo_All(nType);
                }

                IsDateGathering = false;


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private int Fnc_Process_GetMaterialinfo_longterm(int nType)
        {

            var MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode);

            var today = DateTime.Now;

            int month = nType;

            string format = "yyyyMMddHHmmss";

            //strEquipid = strEquipid.Replace("TWR", "G"); //20200529

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<StorageData> list = new List<StorageData>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                //[2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님

                DateTime dt = DateTime.ParseExact(data.Input_date, format, null);
                DateTime dt_temp = today.AddMonths(-month);

                int result = DateTime.Compare(dt, dt_temp);


                if (result < 0)
                {
                    list.Add(data);

                }
                //]2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님


            }

            list.Sort(sortlist_date);

            foreach (var item in list)
            {
                //string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));  //210818_Sangik_choi_입출고 조회중 DB 오류로 삭제
                string strdate = item.Input_date;
                strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                    + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                dataGridView_longterm.Rows.Add(new object[10] { item.SID, item.LOTID, item.UID, item.Quantity, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
            }

            return nMtlCount;


        }


        //[2108010_Sangik.choi_장기보관관리기능추가 by이종명수석님


        private int Fnc_Process_GetMaterialinfo_longterm(int nType, string strEquipid)
        {

            var MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquipid);

            var today = DateTime.Now;
            
            int month = nType;

            string format = "yyyyMMddHHmmss";

            strEquipid = strEquipid.Replace("TWR", "G"); //20200529

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<StorageData> list = new List<StorageData>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                //[2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님

                DateTime dt = DateTime.ParseExact(data.Input_date, format, null);
                DateTime dt_temp = today.AddMonths(-month);

                int result = DateTime.Compare(dt, dt_temp);


                if (result < 0)
                {
                    list.Add(data);

                }
                //]2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님


            }

            list.Sort(sortlist_date);

            foreach (var item in list)
            {
                //string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));  //210818_Sangik_choi_입출고 조회중 DB 오류로 삭제
                string strdate = item.Input_date;
                strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                    + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                dataGridView_longterm.Rows.Add(new object[10] { item.SID, item.LOTID, item.UID, item.Quantity, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
            }

            return nMtlCount;


        }

        //]2108010_Sangik.choi_장기보관관리기능추가 by이종명수석님


        //[2108010_Sangik.choi_장기보관관리기능추가 by이종명수석님

        int sortlist_date(StorageData obj1, StorageData obj2)
        {
            return obj1.Input_date.CompareTo(obj2.Input_date);
        }

        //]2108010_Sangik.choi_장기보관관리기능추가 by이종명수석님


        //[2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님

        private void Fnc_Get_PickID(string strGroupinfo)
        {
            // GetPickIDNo - query = string.Format(@"SELECT * FROM TB_IDNUNMER_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}'", strLinecode, strEquipid);

            ///Pick id load
            string equipid =  strGroupinfo;
            var tableList = AMM_Main.AMM.GetPickIDNo(AMM_Main.strDefault_linecode, equipid);

            if (tableList.Rows.Count == 0)
            {
                if (strGroupinfo == "1")
                    label_pickid_LT.Text = "PL0000001";
                else if (strGroupinfo == "2")
                    label_pickid_LT.Text = "PN0000001";
                else if (strGroupinfo == "3")
                    label_pickid_LT.Text = "PM0000001";
                //220829_ilyoung_타워그룹추가
                else if (strGroupinfo == "4")
                    label_pickid_LT.Text = "PO0000001";
                else if (strGroupinfo == "5")
                    label_pickid_LT.Text = "PP0000001";
                //220829_ilyoung_타워그룹추가
            }
            else
            {
                string strprefix = tableList.Rows[0]["PICK_PREFIX"].ToString();
                strprefix = strprefix.Trim();
                string strNo = tableList.Rows[0]["PICK_NUM"].ToString();
                strNo = strNo.Trim();

                label_pickid_LT.Text = strprefix + strNo;
            }


            string strPickingID = label_pickid_LT.Text;
            string strDefaultPickingID = "";


            if (AMM_Main.strDefault_Group == strGroupinfo)
                strDefaultPickingID = strPickingID;


            Fnc_Update_PickID(AMM_Main.strDefault_linecode, equipid, strPickingID);


        }

        //]2108011_Sangik.choi_장기보관관리기능추가 by이종명수석님


        //[210813_Sangik.choi_장기보관관리기능추가(이종명수석님)

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

        //]210813_Sangik.choi_장기보관관리기능추가(이종명수석님)


        private int Fnc_Process_GetMaterialinfo(int nType, string strEquipid)
        {
            //GetMTLInfo()-query = string.Format(@"SELECT * FROM TB_MTL_INFO WHERE LINE_CODE='{0}' and EQUIP_ID='{1}'", strLinecode, strEquipid);

            var MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquipid);

            strEquipid = strEquipid.Replace("TWR", "G"); //20200529

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<StorageData> list = new List<StorageData>();

            divde_inch[] divde_7_13 = new divde_inch[2];

            for (int n = 0; n < 2; n++)
            {
                divde_7_13[n].bFlag = false;
                divde_7_13[n].nQty = 0;
                divde_7_13[n].nReelCount = 0;
                divde_7_13[n].strLocation = "";

            }

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
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

            int nIndex = 1;

            if (nType == 0) //SID
            {
                string strSetSID = "", strinch = "";
                int nReelcount = 0; double nQty = 0;
                int nIdx = 0;


                for (int i = 0; i < nMtlCount; i++)
                {
                    if (strSetSID != list[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            // Grid 출력 부분 ////////////////////////////////////
                            string strnQty = string.Format("{0:0,0}", nQty);

                            if (divde_7_13[1].bFlag == true)
                            {
                                strnQty = string.Format("{0:0,0}", divde_7_13[1].nQty);
                                dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, divde_7_13[1].nReelCount, strnQty, "13", strEquipid });
                                divde_7_13[1].bFlag = false;
                                nIdx++;
                            }

                            if (divde_7_13[0].bFlag == true)
                            {
                                strnQty = string.Format("{0:0,0}", divde_7_13[0].nQty);
                                dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, divde_7_13[0].nReelCount, strnQty, "7", strEquipid });
                                divde_7_13[0].bFlag = false;
                                nIdx++;
                            }


                            if (list[i].Inch == "13")
                            {
                                divde_7_13[1].bFlag = true;
                                divde_7_13[1].nQty = double.Parse(list[i].Quantity);
                                divde_7_13[1].nReelCount = 1;

                                divde_7_13[0].strLocation = "";
                                divde_7_13[0].nQty = 0;

                            }
                            else if (list[i].Inch == "7")
                            {
                                divde_7_13[0].bFlag = true;
                                divde_7_13[0].nQty = double.Parse(list[i].Quantity);
                                divde_7_13[0].nReelCount = 1;


                                divde_7_13[1].strLocation = "";
                                divde_7_13[1].nQty = 0;
                            }


                            //새로운 Data로 변경 
                            strSetSID = list[i].SID;
                            //strInch = list[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list[i].Quantity);

                        }
                        else
                        {

                            if (list[i].Inch == "13")
                            {
                                divde_7_13[1].bFlag = true;
                                divde_7_13[1].nQty = double.Parse(list[i].Quantity);
                                divde_7_13[1].nReelCount = 1;
                            }
                            else if (list[i].Inch == "7")
                            {
                                divde_7_13[0].bFlag = true;
                                divde_7_13[0].nQty = double.Parse(list[i].Quantity);
                                divde_7_13[0].nReelCount = 1;
                            }

                            strSetSID = list[i].SID;
                            nIdx++;
                        }
                    }
                    else
                    {
                        if (list[i].Inch == "13" && divde_7_13[1].bFlag == true)
                        {
                            divde_7_13[1].nQty += double.Parse(list[i].Quantity);
                            divde_7_13[1].nReelCount++;
                        }
                        else if (list[i].Inch == "7" && divde_7_13[0].bFlag == true)
                        {
                            divde_7_13[0].nQty += double.Parse(list[i].Quantity);
                            divde_7_13[0].nReelCount++;
                        }
                        else if (list[i].Inch == "13" && divde_7_13[1].bFlag == false)
                        {
                            divde_7_13[1].bFlag = true;
                            divde_7_13[1].nQty += double.Parse(list[i].Quantity);
                            divde_7_13[1].nReelCount = 1;
                        }
                        else if (list[i].Inch == "7" && divde_7_13[0].bFlag == false)
                        {
                            divde_7_13[0].bFlag = true;
                            divde_7_13[0].nQty += double.Parse(list[i].Quantity);
                            divde_7_13[0].nReelCount = 1;
                        }

                    }

                    if (i == nMtlCount - 1)
                    {
                        if (divde_7_13[1].bFlag == true)
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, divde_7_13[1].nReelCount, divde_7_13[1].nQty, "13", strEquipid });
                            nIdx++;
                        }

                        if (divde_7_13[0].bFlag == true)
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, divde_7_13[0].nReelCount, divde_7_13[0].nQty, "7", strEquipid });

                        }
                    }

                }







                //for (int i = 0; i < nMtlCount; i++)
                //{
                //    if (strSetSID != list[i].SID)
                //    {
                //        if (strSetSID != "")
                //        {
                //            string strnQty = string.Format("{0:0,0}",nQty);
                //            //string strinch = list[i].Inch;
                //            dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, nReelcount, strnQty, strinch, strEquipid });

                //            strSetSID = list[i].SID;
                //            strinch = list[i].Inch;
                //            nReelcount = 1;
                //            nQty = Int32.Parse(list[i].Quantity);
                //            nIdx++;
                //        }
                //        else
                //        {
                //            strSetSID = list[i].SID;
                //            strinch = list[i].Inch;
                //            nReelcount = 1;
                //            nQty = Int32.Parse(list[i].Quantity);
                //            nIdx++;
                //        }
                //    }
                //    else
                //    {
                //        nReelcount++;
                //        nQty = nQty + Int32.Parse(list[i].Quantity);
                //    }

                //    if (i == nMtlCount - 1)
                //    {
                //        string strnQty = string.Format("{0:0,0}", nQty);
                //        //string strinch = list[i].Inch;
                //        dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, nReelcount, strnQty, strinch, strEquipid });
                //    }
                //}
            }
            else if(nType == 1) //Detatil info
            {
                foreach (var item in list)
                {
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    string strdate = item.Input_date;
                    strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " " 
                        + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                    dataGridView_info.Rows.Add(new object[11] { nIndex++, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
                }

            }
            else
            {
                return nMtlCount;
            }

            return nMtlCount;
        }

        private int Fnc_Process_GetMaterialinfo_All(int nType) //nType 0 : SID, 1: 상세 정보
        {
            DataTable MtlList = null;

            List<StorageData> list = new List<StorageData>();
            divde_inch[] divde_7_13 = new divde_inch[2];

            for (int n=0; n < 2 ;n++)
            {
                divde_7_13[n].bFlag = false;
                divde_7_13[n].nQty = 0;
                divde_7_13[n].nReelCount = 0;
                divde_7_13[n].strLocation = "";

            }

            MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode);

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Equipid = MtlList.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                list.Add(data);
            }
            MtlList = null;

            list.Sort(CompareStorageData);

            int nIndex = 1;

            if (nType == 0) //SID
            {
                string strSetSID = "", strLocation = "", strLocation_before = "";
                int nReelcount = 0; double nQty = 0;
                int nIdx = 0;

                for(int i =0; i<nMtlCount; i++)
                 {
                     if (strSetSID != list[i].SID)
                     {
                         if (strSetSID != "")
                         {
                             // Grid 출력 부분 ////////////////////////////////////
                             string strnQty = string.Format("{0:0,0}", nQty);
                             string strinch = list[i].Inch;

                             if (divde_7_13[1].bFlag == true)
                             {
                                 strnQty = string.Format("{0:0,0}", divde_7_13[1].nQty);
                                 strinch = list[i].Inch;

                                string str1 = "";
                                if (divde_7_13[1].strLocation.Contains("1")) str1 += " TWR1 ";
                                if (divde_7_13[1].strLocation.Contains("2")) str1 += " TWR2 ";
                                if (divde_7_13[1].strLocation.Contains("3")) str1 += " TWR3 ";
                                if (divde_7_13[1].strLocation.Contains("4")) str1 += " TWR4 ";	//220829_ilyoung_타워그룹추가
                                if (divde_7_13[1].strLocation.Contains("5")) str1 += " TWR5 ";	//220829_ilyoung_타워그룹추가
                                divde_7_13[1].strLocation = str1;
                                dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, divde_7_13[1].nReelCount, strnQty, "13", divde_7_13[1].strLocation });
                                divde_7_13[1].bFlag = false;
                                 nIdx++;
                             }

                             if (divde_7_13[0].bFlag == true)
                             {
                                 strnQty = string.Format("{0:0,0}", divde_7_13[0].nQty);
                                 strinch = list[i].Inch;

                                string str1 = "";
                                if (divde_7_13[0].strLocation.Contains("1")) str1 += " TWR1";
                                if (divde_7_13[0].strLocation.Contains("2")) str1 += " TWR2";
                                if (divde_7_13[0].strLocation.Contains("3")) str1 += " TWR3";
                                if (divde_7_13[0].strLocation.Contains("4")) str1 += " TWR4 ";	//220829_ilyoung_타워그룹추가
                                if (divde_7_13[0].strLocation.Contains("5")) str1 += " TWR5 ";	//220829_ilyoung_타워그룹추가
                                divde_7_13[0].strLocation = str1;//220829_ilyoung_타워그룹추가
                                dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, divde_7_13[0].nReelCount, strnQty, "7", divde_7_13[0].strLocation });
                                divde_7_13[0].bFlag = false;
                                 nIdx++;
                             }


                             if (list[i].Inch == "13")
                             {
                                 divde_7_13[1].bFlag = true;
                                 divde_7_13[1].strLocation = list[i].Equipid;
                                 divde_7_13[1].nQty = double.Parse(list[i].Quantity);
                                 divde_7_13[1].nReelCount = 1;

                                divde_7_13[0].strLocation = "";
                                divde_7_13[0].nQty = 0;

                            }
                             else if (list[i].Inch == "7")
                             {
                                 divde_7_13[0].bFlag = true;
                                 divde_7_13[0].strLocation = list[i].Equipid;
                                 divde_7_13[0].nQty = double.Parse(list[i].Quantity);
                                 divde_7_13[0].nReelCount = 1;


                                divde_7_13[1].strLocation = "";
                                divde_7_13[1].nQty = 0;
                            }


                             //새로운 Data로 변경 
                             strSetSID = list[i].SID;
                             strLocation = list[i].Equipid;
                             strLocation_before = list[i].Equipid;
                             //strInch = list[i].Inch;
                             nReelcount = 1;
                             nQty = Int32.Parse(list[i].Quantity);
                       
                         }
                         else
                         {

                             if (list[i].Inch == "13")
                             {
                                 divde_7_13[1].bFlag = true;
                                 divde_7_13[1].strLocation = list[i].Equipid;
                                 divde_7_13[1].nQty = double.Parse(list[i].Quantity);
                                 divde_7_13[1].nReelCount = 1;
                             }
                             else if(list[i].Inch == "7")
                             {
                                 divde_7_13[0].bFlag = true;
                                 divde_7_13[0].strLocation = list[i].Equipid;
                                 divde_7_13[0].nQty = double.Parse(list[i].Quantity);
                                 divde_7_13[0].nReelCount = 1;
                             }

                             strSetSID = list[i].SID;
                             nIdx++;
                         }
                     }
                     else
                     {
                         if (list[i].Inch == "13" && divde_7_13[1].bFlag == true)
                         {
                             if (!divde_7_13[1].strLocation.Contains(list[i].Equipid)) divde_7_13[1].strLocation += " " + list[i].Equipid;
                             divde_7_13[1].nQty += double.Parse(list[i].Quantity);
                             divde_7_13[1].nReelCount++;
                         }
                         else if (list[i].Inch == "7" && divde_7_13[0].bFlag == true)
                         {
                             if (!divde_7_13[0].strLocation.Contains(list[i].Equipid)) divde_7_13[0].strLocation += " " + list[i].Equipid;
                             divde_7_13[0].nQty += double.Parse(list[i].Quantity);
                             divde_7_13[0].nReelCount++;
                         }
                         else if(list[i].Inch == "13" && divde_7_13[1].bFlag == false)
                         {
                             divde_7_13[1].bFlag = true;
                            if (!divde_7_13[1].strLocation.Contains(list[i].Equipid))
                            {
                                if (divde_7_13[1].strLocation == "") divde_7_13[1].strLocation += list[i].Equipid;
                                else divde_7_13[1].strLocation += " "+list[i].Equipid;
                            }
                             
                             divde_7_13[1].nQty += double.Parse(list[i].Quantity);
                             divde_7_13[1].nReelCount = 1;
                         }
                         else if(list[i].Inch == "7" && divde_7_13[0].bFlag == false)
                         {
                             divde_7_13[0].bFlag = true;
                             if (!divde_7_13[0].strLocation.Contains(list[i].Equipid))
                            {
                                if (divde_7_13[0].strLocation == "") divde_7_13[0].strLocation += list[i].Equipid;
                                else divde_7_13[0].strLocation += " " + list[i].Equipid;
                            }
                            divde_7_13[0].nQty += double.Parse(list[i].Quantity);
                             divde_7_13[0].nReelCount = 1;
                         }

                     }

                     if (i == nMtlCount - 1)
                     {
                         if (divde_7_13[1].bFlag == true)
                         {
                             string strnQty = string.Format("{0:0,0}", nQty);
                             string strinch = list[i].Inch;
                             dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, divde_7_13[1].nReelCount, divde_7_13[1].nQty, "13", divde_7_13[1].strLocation });
                             nIdx++;
                         }

                         if (divde_7_13[0].bFlag == true)
                         {
                             string strnQty = string.Format("{0:0,0}", nQty);
                             string strinch = list[i].Inch;
                             dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, divde_7_13[0].nReelCount, divde_7_13[0].nQty, "7", divde_7_13[0].strLocation });

                         }
                     }

                 }

 
 /*
                 for (int i = 0; i < nMtlCount; i++)
                 {
                     if (strSetSID != list[i].SID)
                     {
                         if (strSetSID != "")
                         {

                              ////Grid 출력 부분 ////////////////////////////////////
                             string strnQty = string.Format("{0:0,0}", nQty);
                             string strinch = list[i].Inch;
                             dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, nReelcount, strnQty, strinch, strLocation });                           
                             ////////////////////////////////


                             ///새로운 Data로 변경 
                             strSetSID = list[i].SID;
                             strLocation = list[i].Equipid;
                             strLocation_before = list[i].Equipid;
                             //strInch = list[i].Inch;
                             nReelcount = 1;
                             nQty = Int32.Parse(list[i].Quantity);
                             nIdx++;
                         }
                         else
                         {
                             if (strLocation_before != list[i].Equipid)
                             {
                                 if (strLocation == "")
                                     strLocation = list[i].Equipid;
                                 else
                                 {
                                     if (!strLocation.Contains(list[i].Equipid))
                                         strLocation = strLocation + "," + list[i].Equipid;
                                 }
                             }

                             strSetSID = list[i].SID;
                             strLocation_before = list[i].Equipid;
                             //strInch = list[i].Inch;
                             nReelcount = 1;
                             nQty = Int32.Parse(list[i].Quantity);
                             nIdx++;
                         }
                     }
                     else
                     {
                         nReelcount++;
                         nQty = nQty + Int32.Parse(list[i].Quantity);

                         if (strLocation_before != list[i].Equipid)
                         {
                             if (strLocation == "")
                                 strLocation = list[i].Equipid;
                             else
                             {
                                 if (!strLocation.Contains(list[i].Equipid))
                                     strLocation = strLocation + "," + list[i].Equipid;
                             }
                         }

                         strLocation_before = list[i].Equipid;
                     }

                     if (i == nMtlCount - 1)
                     {
                         string strnQty = string.Format("{0:0,0}", nQty);
                         string strinch = list[i].Inch;
                         dataGridView_info.Rows.Add(new object[6] { nIdx, strSetSID, nReelcount, strnQty, strinch, strLocation });
                     }
                 }*/
            }
            else if (nType == 1) //Detatil info
            {
                foreach (var item in list)
                {
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    string strdate = item.Input_date;
                    strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                        + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                    dataGridView_info.Rows.Add(new object[11] { nIndex++, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
                }
            }
            else
            {
                return nMtlCount;
            }

            return nMtlCount;
        }

        private void Fnc_Process_GetMaterialinfo_DetailAll()//상세 정보
        {
            DataTable MtlList = null;

            List<StorageData> list = new List<StorageData>();

            MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode);

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return;
            }

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData data = new StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Equipid = MtlList.Rows[i]["EQUIP_ID"].ToString(); data.Equipid = data.Equipid.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                string str = data.Tower_no.Substring(2, 1);
                int nTwr = Int32.Parse(str) - 1;
                if (bGroupUse[nTwr])
                {
                    list.Add(data);
                }
            }
            MtlList = null;

            list.Sort(CompareStorageData);

            int nIndex = 1;

            foreach (var item in list)
            {
                string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                string strdate = item.Input_date;
                strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                    + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                dataGridView_info.Rows.Add(new object[11] { nIndex++, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
                Application.DoEvents();
            }
        }




        int CompareStorageData(StorageData obj1, StorageData obj2)
        {
            return obj1.SID.CompareTo(obj2.SID);
        }

        int CompareStorageData2(StorageData2 obj1, StorageData2 obj2)
        {
            return obj1.Creation_date.CompareTo(obj2.Creation_date);
        }

        int CompareStorageData3(StorageData2 obj1, StorageData2 obj2)
        {
            return obj1.SID.CompareTo(obj2.SID);
        }

        public void Fnc_ProcessFind(int nType, string strMtl)
        {
            List<StorageData> list = new List<StorageData>();

            DataTable MtlList = null;

            string strEquipid = "TWR";
            bool bSearch = false;
            string strnQty = "";

            if(strMtl.Length == 4 || nType == 1)
                comboBox_sid.Items.Clear();


            for (int j = 1; j < 6; j++) 	//220829_ilyoung_타워그룹추가
            {
                MtlList = AMM_Main.AMM.GetMTLInfo(AMM_Main.strDefault_linecode, strEquipid+j.ToString());

                for (int i = 0; i < MtlList.Rows.Count; i++)
                {
                    StorageData data = new StorageData();

                    data.Equipid = strEquipid + j.ToString();
                    data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                    data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                    data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                    data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                    data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                    data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                    data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                    data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                    data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                    data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                    if (nType == 0)
                    {
                        if (strMtl.Length == 4)
                        {
                            if (data.SID.Length != 0)
                            {
                                string strCheck = data.SID.Substring(data.SID.Length - 4);

                                if (strMtl == strCheck)
                                {
                                    list.Add(data);

                                    int nCombocount = comboBox_sid.Items.Count;
                                    bool bjudge = false;
                                    for (int k = 0; k < nCombocount; k++)
                                    {
                                        string str = comboBox_sid.Items[k].ToString();

                                        if (data.SID == str)
                                        {
                                            bjudge = true;
                                        }
                                    }

                                    if (!bjudge)
                                    {
                                        comboBox_sid.Items.Add(data.SID);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (data.SID == strMtl)
                            {
                                list.Add(data);                                
                            }
                        }
                    }
                    else
                    {
                        if (data.UID == strMtl)
                        {
                            strnQty = string.Format("{0:0,0}", Int32.Parse(data.Quantity));

                            label_info1.Text = data.SID;
                            label_info2.Text = strEquipid + j.ToString();
                            label_info2.Text = label_info2.Text.Replace("TWR", "G");
                            label_info3.Text = "1";
                            label_info4.Text = strnQty;

                            bSearch = true;
                        }
                    }
                }
            }
            list.Sort(CompareStorageData);

            if (bSearch)
                return;

            if (list.Count == 0 || (nType == 1 && bSearch == false))
            {
                label_info1.Text = "-";
                label_info2.Text = "자재 없음!";
                label_info3.Text = "-";
                label_info4.Text = "-";

                return;
            }

            string strLocation = "";
            double nQty = 0;

            for (int i = 0; i < list.Count; i++)
            {
                nQty = nQty + Int32.Parse(list[i].Quantity);
                if(strLocation != list[i].Equipid)
                {
                    if (strLocation == "")
                    {
                        strLocation = list[i].Equipid;
                    }
                    else
                    {
                        if (!strLocation.Contains(list[i].Equipid))
                            strLocation = strLocation + "," + list[i].Equipid;
                    }
                    
                }
            }

            strnQty = string.Format("{0:0,0}", nQty);

            label_info1.Text = list[0].SID;
            label_info2.Text = strLocation;
            label_info2.Text = label_info2.Text.Replace("TWR", "G");
            label_info3.Text = list.Count.ToString();
            label_info4.Text = strnQty;
        }

        public int Fnc_Process_GetINOUT_mtlinfo(int nType, string strEquipid, double strTime_st, double strTime_ed)
        {
            string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            label_updatedate2.Text = "최근 업데이트: " + strToday + " " + strHead;

            var MtlList = AMM_Main.AMM.GetInouthistroy(AMM_Main.strDefault_linecode, strEquipid, strTime_st, strTime_ed);

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<StorageData2> list_input = new List<StorageData2>();
            List<StorageData2> list_return = new List<StorageData2>();
            List<StorageData2> list_out = new List<StorageData2>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData2 data = new StorageData2();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Creation_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Creation_date = data.Creation_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();
                data.pickid = MtlList.Rows[i]["PICKID"].ToString(); data.pickid = data.pickid.Trim();
                data.Status = MtlList.Rows[i]["STATUS"].ToString(); data.Status = data.Status.Trim();
                data.Requestor = MtlList.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();

                if (data.Status == "IN" && data.Input_type == "CART")
                    list_input.Add(data);
                else if (data.Status == "IN" && data.Input_type == "RETURN")
                    list_return.Add(data);
                else if (data.Status == "OUT" || data.Status == "OUT-MANUAL")
                    list_out.Add(data);
            }

            int nIndex = 1;

            if (nType == 0) //SID
            {
                list_input.Sort(CompareStorageData3);

                string strSetSID = "", strInch = "";
                int nReelcount = 0; double nQty = 0;
                int nIdx = 0;

                for (int i = 0; i < list_input.Count; i++)
                {
                    if (strSetSID != list_input[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_input[i].Quantity);
                    }

                    if (i == list_input.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_return.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_return.Count; i++)
                {
                    if (strSetSID != list_return[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_return[i].Quantity);
                    }

                    if (i == list_return.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_out.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0; 

                for (int i = 0; i < list_out.Count; i++)
                {
                    if (strSetSID != list_out[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_out[i].Quantity);
                    }

                    if (i == list_out.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }
            }
            else if (nType == 1) //Detatil info
            {
                list_input.Sort(CompareStorageData2);
                nIndex = 1;
                foreach (var item in list_input)
                {
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_input.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_return.Sort(CompareStorageData2);
                foreach (var item in list_return)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_return.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_out.Sort(CompareStorageData2);
                foreach (var item in list_out)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);

                    string strType = "자동";
                    if (item.pickid == "-" && item.Requestor == "-")
                        strType = "강제배출";

                    dataGridView_output.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Inch ,item.pickid, item.Requestor, item.Tower_no, strType });
                }

            }
            else
            {
                return nMtlCount;
            }

            label_incount.Text = string.Format("{0:0,0}", (int.Parse(label_incount.Text) + list_input.Count).ToString());
            label_returncount.Text = string.Format("{0:0,0}", (int.Parse(label_returncount.Text) + list_return.Count).ToString());
            label_outcount.Text = string.Format("{0:0,0}", (int.Parse(label_outcount.Text) + list_out.Count).ToString());

            return nMtlCount;
        }
        public int Fnc_Process_GetINOUT_mtlinfo_Sid(int nType, string strSearch_sid, double strTime_st, double strTime_ed)
        {
            string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            label_updatedate2.Text = "최근 업데이트: " + strToday + " " + strHead;

            var MtlList = AMM_Main.AMM.GetInouthistroy_Sid(AMM_Main.strDefault_linecode, strSearch_sid, strTime_st, strTime_ed);

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<StorageData2> list_input = new List<StorageData2>();
            List<StorageData2> list_return = new List<StorageData2>();
            List<StorageData2> list_out = new List<StorageData2>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData2 data = new StorageData2();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Creation_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Creation_date = data.Creation_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();
                data.pickid = MtlList.Rows[i]["PICKID"].ToString(); data.pickid = data.pickid.Trim();
                data.Status = MtlList.Rows[i]["STATUS"].ToString(); data.Status = data.Status.Trim();
                data.Requestor = MtlList.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();

                if (data.Status == "IN" && data.Input_type == "CART")
                    list_input.Add(data);
                else if (data.Status == "IN" && data.Input_type == "RETURN")
                    list_return.Add(data);
                else if (data.Status == "OUT" || data.Status == "OUT-MANUAL")
                    list_out.Add(data);
            }

            int nIndex = 1;

            if (nType == 0) //SID
            {
                list_input.Sort(CompareStorageData3);

                string strSetSID = "", strInch = "";
                int nReelcount = 0; double nQty = 0;
                int nIdx = 0;

                for (int i = 0; i < list_input.Count; i++)
                {
                    if (strSetSID != list_input[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_input[i].Quantity);
                    }

                    if (i == list_input.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_return.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_return.Count; i++)
                {
                    if (strSetSID != list_return[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_return[i].Quantity);
                    }

                    if (i == list_return.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_out.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_out.Count; i++)
                {
                    if (strSetSID != list_out[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_out[i].Quantity);
                    }

                    if (i == list_out.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }
            }
            else if (nType == 1) //Detatil info
            {
                list_input.Sort(CompareStorageData2);
                nIndex = 1;
                foreach (var item in list_input)
                {
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_input.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_return.Sort(CompareStorageData2);
                foreach (var item in list_return)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_return.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_out.Sort(CompareStorageData2);
                foreach (var item in list_out)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);

                    string strType = "자동";
                    if (item.pickid == "-" && item.Requestor == "-")
                        strType = "강제배출";

                    dataGridView_output.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Inch, item.pickid, item.Requestor, item.Tower_no, strType });
                }

            }
            else
            {
                return nMtlCount;
            }

            label_incount.Text = string.Format("{0:0,0}", list_input.Count.ToString());
            label_returncount.Text = string.Format("{0:0,0}", list_return.Count.ToString());
            label_outcount.Text = string.Format("{0:0,0}", list_out.Count.ToString());

            return nMtlCount;
        }
        public int Fnc_Process_GetINOUT_mtlinfo_Sid2(int nType, string strEquip, string strSearch_sid, double strTime_st, double strTime_ed)
        {
            string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            label_updatedate2.Text = "최근 업데이트: " + strToday + " " + strHead;

            var MtlList = new DataTable();

            if(int.Parse(strEquip.Replace("TWR","")) == comboBox_group2.Items.Count )
            {
                MtlList = AMM_Main.AMM.GetInouthistroy_Sid3(AMM_Main.strDefault_linecode, strSearch_sid, strTime_st, strTime_ed);
            }
            else
            {
                MtlList = AMM_Main.AMM.GetInouthistroy_Sid2(AMM_Main.strDefault_linecode, strEquip, strSearch_sid, strTime_st, strTime_ed);
            }

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<StorageData2> list_input = new List<StorageData2>();
            List<StorageData2> list_return = new List<StorageData2>();
            List<StorageData2> list_out = new List<StorageData2>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                StorageData2 data = new StorageData2();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Creation_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Creation_date = data.Creation_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();
                data.pickid = MtlList.Rows[i]["PICKID"].ToString(); data.pickid = data.pickid.Trim();
                data.Status = MtlList.Rows[i]["STATUS"].ToString(); data.Status = data.Status.Trim();
                data.Requestor = MtlList.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();

                if (data.Status == "IN" && data.Input_type == "CART")
                    list_input.Add(data);
                else if (data.Status == "IN" && data.Input_type == "RETURN")
                    list_return.Add(data);
                else if (data.Status == "OUT" || data.Status == "OUT-MANUAL")
                    list_out.Add(data);
            }

            int nIndex = 1;

            if (nType == 0) //SID
            {
                list_input.Sort(CompareStorageData3);

                string strSetSID = "", strInch = "";
                int nReelcount = 0; double nQty = 0;
                int nIdx = 0;

                for (int i = 0; i < list_input.Count; i++)
                {
                    if (strSetSID != list_input[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_input[i].SID;
                            strInch = list_input[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_input[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_input[i].Quantity);
                    }

                    if (i == list_input.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_input.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_return.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_return.Count; i++)
                {
                    if (strSetSID != list_return[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_return[i].SID;
                            strInch = list_return[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_return[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_return[i].Quantity);
                    }

                    if (i == list_return.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_return.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }

                list_out.Sort(CompareStorageData3);

                strSetSID = ""; strInch = "";
                nReelcount = 0; nQty = 0;
                nIdx = 0;

                for (int i = 0; i < list_out.Count; i++)
                {
                    if (strSetSID != list_out[i].SID)
                    {
                        if (strSetSID != "")
                        {
                            string strnQty = string.Format("{0:0,0}", nQty);
                            dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });

                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity);
                            nIdx++;
                        }
                        else
                        {
                            strSetSID = list_out[i].SID;
                            strInch = list_out[i].Inch;
                            nReelcount = 1;
                            nQty = Int32.Parse(list_out[i].Quantity);
                            nIdx++;
                        }
                    }
                    else
                    {
                        nReelcount++;
                        nQty = nQty + Int32.Parse(list_out[i].Quantity);
                    }

                    if (i == list_out.Count - 1)
                    {
                        string strnQty = string.Format("{0:0,0}", nQty);
                        dataGridView_output.Rows.Add(new object[5] { nIdx, strSetSID, nReelcount, strnQty, strInch });
                    }
                }
            }
            else if (nType == 1) //Detatil info
            {
                list_input.Sort(CompareStorageData2);
                nIndex = 1;
                foreach (var item in list_input)
                {
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_input.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_return.Sort(CompareStorageData2);
                foreach (var item in list_return)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
                    dataGridView_return.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, item.Manufacturer, item.Inch });
                }

                nIndex = 1;
                list_out.Sort(CompareStorageData2);
                foreach (var item in list_out)
                {
                    string strDate = item.Creation_date.Substring(0, 8);
                    string strTime = item.Creation_date.Substring(8, 6);
                    string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                    strTime = strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);

                    string strType = "자동";
                    if (item.pickid == "-" && item.Requestor == "-")
                        strType = "강제배출";

                    dataGridView_output.Rows.Add(new object[12] { nIndex++, strDate, strTime, item.SID, item.LOTID, item.UID, strnQty, item.Inch, item.pickid, item.Requestor, item.Tower_no, strType });
                }

            }
            else
            {
                return nMtlCount;
            }

            label_incount.Text = string.Format("{0:0,0}", list_input.Count.ToString());
            label_returncount.Text = string.Format("{0:0,0}", list_return.Count.ToString());
            label_outcount.Text = string.Format("{0:0,0}", list_out.Count.ToString());

            return nMtlCount;
        }
        private void button_update_Click(object sender, EventArgs e)
        {
            Fnc_Process_CalMaterialInfo();
        }

        private void button_excel_Click(object sender, EventArgs e)
        {
            Fnc_Process_CalMaterialInfo();
            
            bExcel_Start = false;

            nExcelIndex = 0;

            Form_Excel Excel_Form = new Form_Excel();
            Excel_Form.ShowDialog();

            if(!bExcel_Start)
            {
                return;
            }

            IsDateGathering = true;
            ////bExcelUse[0] = ASM all file, 1: ASM SID sorting , 2: In/out/return All data, 3: In/out/return SID sortinf 

            string strPath = strExcelfilePath + "\\";
            string strPath2 = strExcelfilePath + "\\";
            string stSaveTime_st = "", stSaveTime_ed = "", stSaveDate_st = "", stSaveDate_ed = "";
            //stSaveTime_st = label_Value_stTime.Text.Replace(":", "_");
            //stSaveTime_ed = label_Value_edTime.Text.Replace(":", "_");
            //stSaveDate_st = label_Value_date_st.Text.Replace("-", string.Empty);
            //stSaveDate_ed = label_Value_date_ed.Text.Replace("-", string.Empty);

            string strDate = stSaveDate_st + "_" + stSaveTime_st + "~" + stSaveDate_ed + "_" + stSaveTime_ed;
            string strDate2 = string.Format("{0}{1:00}{2:00}_{3}_{4}_{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            strPath = strPath + "ITS_" + strDate;
            strPath2 = strPath2 + "ITS_" + strDate2;

            string strPathName = "";

            if (bExcelUse[0])//Tower Inventory SID
            {
                strPathName = strPath2 + "_타워재고SID.xlsx";

                if (File.Exists(strPathName))
                {
                    string path = strPathName;
                    bool available = true;
                    try
                    {
                        using (FileStream fs = File.Open(path, FileMode.Open))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        string str = string.Format("{0}", ex);
                        //Fnc_SaveLog("Exception,Excel 파일 생성 실패 " + ex.ToString());
                        available = false;
                    }

                    if (!available)
                    {
                        IsDateGathering = false;
                        MessageBox.Show("[타워 재고 SID]같은 파일의 이름이 열려 있습니다.  해당 파일을 닫고 다시 실행 하십시오.");
                        return;
                    }
                    else
                    {
                        File.Delete(strPathName);
                    }
                }

                Fnc_ExcelCreate_InventoryInfo(strPathName, 0); //0: SID , 1: 상세 정보
            }

            if (bExcelUse[1])//Tower Inventory 상세 정보
            {
                strPathName = strPath2 + "_타워재고상세정보.xlsx";

                if (File.Exists(strPathName))
                {
                    string path = strPathName;
                    bool available = true;
                    try
                    {
                        using (FileStream fs = File.Open(path, FileMode.Open))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        string str = string.Format("{0}", ex);
                        //Fnc_SaveLog("Exception,Excel 파일 생성 실패 " + ex.ToString());
                        available = false;
                    }

                    if (!available)
                    {
                        IsDateGathering = false;
                        MessageBox.Show("[타워 재고 상세 정보]같은 파일의 이름이 열려 있습니다.  해당 파일을 닫고 다시 실행 하십시오.");
                        return;
                    }
                    else
                    {
                        File.Delete(strPathName);
                    }
                }

                //Fnc_ExcelCreate_InventoryInfo_Detail(strPathName, 0); //0: SID , 1: 상세 정보
                Fnc_ExcelCreate_InventoryInfo_Detail_All(strPathName, 0);
            }

            IsDateGathering = false;

            Fnc_Process_CalMaterialInfo();
        }       

        public void Fnc_ExcelCreate_InventoryInfo(string strPath, int nType)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;
            Excel.Worksheet xlWorkSheet4;
            Excel.Worksheet xlWorkSheet5;	//220829_ilyoung_타워그룹추가
            Excel.Worksheet xlWorkSheet6;	//220829_ilyoung_타워그룹추가

            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet2 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet3 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet4 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet5 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);  //220829_ilyoung_타워그룹추가
            xlWorkSheet6 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);  //220829_ilyoung_타워그룹추가


            /////Input save////////
            int nCellcount = 0;

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Name = "ALL";

            Fnc_Init_datagrid(0);
            Fnc_Process_GetMaterialinfo_All(0);

            int nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet.Cells[1, 2] = "No";
            xlWorkSheet.Cells[1, 3] = "SID";
            xlWorkSheet.Cells[1, 4] = "릴수";
            xlWorkSheet.Cells[1, 5] = "TTL";
            xlWorkSheet.Cells[1, 6] = "인치";
            xlWorkSheet.Cells[1, 7] = "위치";
            xlWorkSheet.Columns.AutoFit();

            for (int i = 0; i < nGcount; i++)
            {
                xlWorkSheet.Cells[2 + nCellcount, 2] = nCellcount + 1;
                xlWorkSheet.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();

                nCellcount++;
            }

            xlWorkSheet.Columns.AutoFit();
            /////////////////////////////////////////////

            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet2.Name = "Group 1";

            Fnc_Init_datagrid(0);
            Fnc_Process_GetMaterialinfo(0, "TWR1");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;
            
            xlWorkSheet2.Cells[1, 2] = "No";
            xlWorkSheet2.Cells[1, 3] = "SID";
            xlWorkSheet2.Cells[1, 4] = "릴수";
            xlWorkSheet2.Cells[1, 5] = "TTL";
            xlWorkSheet2.Cells[1, 6] = "TTL";
            xlWorkSheet2.Cells[1, 7] = "위치";
            xlWorkSheet2.Columns.AutoFit();

            if (bGroupUse[0])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet2.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet2.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet2.Columns.AutoFit();
            ///////////////////////////////////////////////////
            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);
            xlWorkSheet3.Name = "Group 2";

            Fnc_Init_datagrid(0);
            Fnc_Process_GetMaterialinfo(0, "TWR2");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet3.Cells[1, 2] = "No";
            xlWorkSheet3.Cells[1, 3] = "SID";
            xlWorkSheet3.Cells[1, 4] = "릴수";
            xlWorkSheet3.Cells[1, 5] = "TTL";
            xlWorkSheet3.Cells[1, 6] = "인치";
            xlWorkSheet3.Cells[1, 7] = "위치";
            xlWorkSheet3.Columns.AutoFit();

            if (bGroupUse[1])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet3.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet3.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet3.Columns.AutoFit();
            /////////////////////////////////////////

            xlWorkSheet4 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(4);
            xlWorkSheet4.Name = "Group 3";

            Fnc_Init_datagrid(0);
            Fnc_Process_GetMaterialinfo(0, "TWR3");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet4.Cells[1, 2] = "No";
            xlWorkSheet4.Cells[1, 3] = "SID";
            xlWorkSheet4.Cells[1, 4] = "릴수";
            xlWorkSheet4.Cells[1, 5] = "TTL";
            xlWorkSheet4.Cells[1, 6] = "인치";
            xlWorkSheet4.Cells[1, 7] = "위치";
            xlWorkSheet4.Columns.AutoFit();

            if (bGroupUse[2])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet4.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet4.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet4.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();

                    nCellcount++;
                }
            }

            xlWorkSheet4.Columns.AutoFit();
            /////////////////////////////////////////

            //]211018_Sangik.choi_재고관리 7번그룹 오류 수정


            /////////////////////////////////////////	//220829_ilyoung_타워그룹추가

            xlWorkSheet5 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(5);
            xlWorkSheet5.Name = "Group 4";

            Fnc_Init_datagrid(0);
            Fnc_Process_GetMaterialinfo(0, "TWR4");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet5.Cells[1, 2] = "No";
            xlWorkSheet5.Cells[1, 3] = "SID";
            xlWorkSheet5.Cells[1, 4] = "릴수";
            xlWorkSheet5.Cells[1, 5] = "TTL";
            xlWorkSheet5.Cells[1, 6] = "인치";
            xlWorkSheet5.Cells[1, 7] = "위치";
            xlWorkSheet5.Columns.AutoFit();

            if (bGroupUse[3])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet5.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet5.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet5.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();

                    nCellcount++;
                }
            }

            xlWorkSheet5.Columns.AutoFit();
            /////////////////////////////////////////	//220829_ilyoung_타워그룹추가
            ///

            /////////////////////////////////////////	//220829_ilyoung_타워그룹추가

            xlWorkSheet6 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(6);
            xlWorkSheet6.Name = "Group 5";

            Fnc_Init_datagrid(0);
            Fnc_Process_GetMaterialinfo(0, "TWR5");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet6.Cells[1, 2] = "No";
            xlWorkSheet6.Cells[1, 3] = "SID";
            xlWorkSheet6.Cells[1, 4] = "릴수";
            xlWorkSheet6.Cells[1, 5] = "TTL";
            xlWorkSheet6.Cells[1, 6] = "인치";
            xlWorkSheet6.Cells[1, 7] = "위치";
            xlWorkSheet6.Columns.AutoFit();

            if (bGroupUse[4])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet6.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet6.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet6.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();

                    nCellcount++;
                }
            }

            xlWorkSheet6.Columns.AutoFit();
            /////////////////////////////////////////	//220829_ilyoung_타워그룹추가





            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkSheet2);
            Marshal.ReleaseComObject(xlWorkSheet3);
            Marshal.ReleaseComObject(xlWorkSheet4);
            Marshal.ReleaseComObject(xlWorkSheet5);	//220829_ilyoung_타워그룹추가
            Marshal.ReleaseComObject(xlWorkSheet6);	//220829_ilyoung_타워그룹추가


            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            //IsDateGathering = false;

            if (nType == 0)
            {
                //string strMsg = "파일(타워재고 SID 기준)  저장 완료! 경로:" + strPath;
                //MessageBox.Show(strMsg);

                System.Diagnostics.Process.Start(strPath);
            }
        }

        public void InitLabel()
        {
            label_incount.Text = "0";
            label_returncount.Text = "0";
            label_outcount.Text = "0";
        }

        public void Fnc_ExcelCreate_INOUTInfo_SID(string strPath, string strStart, string strEnd)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet1;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;

            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet2 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet3 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);


            xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet1.Name = "입고";

            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet2.Name = "리턴";

            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);
            xlWorkSheet3.Name = "출고";

            xlWorkSheet1.Cells[1, 2] = "No";
            xlWorkSheet1.Cells[1, 3] = "SID";
            xlWorkSheet1.Cells[1, 4] = "릴수";
            xlWorkSheet1.Cells[1, 5] = "TTL";
            xlWorkSheet1.Cells[1, 6] = "인치";
            xlWorkSheet1.Cells[1, 7] = "위치";

            xlWorkSheet2.Cells[1, 2] = "No";
            xlWorkSheet2.Cells[1, 3] = "SID";
            xlWorkSheet2.Cells[1, 4] = "릴수";
            xlWorkSheet2.Cells[1, 5] = "TTL";
            xlWorkSheet2.Cells[1, 6] = "인치";
            xlWorkSheet2.Cells[1, 7] = "위치";

            xlWorkSheet3.Cells[1, 2] = "No";
            xlWorkSheet3.Cells[1, 3] = "SID";
            xlWorkSheet3.Cells[1, 4] = "릴수";
            xlWorkSheet3.Cells[1, 5] = "TTL";
            xlWorkSheet3.Cells[1, 6] = "인치";
            xlWorkSheet3.Cells[1, 7] = "위치";

            int nGcount_input = 0, nGcount_return = 0, nGcount_output = 0;
            int nCellcount_input = 0, nCellcount_return = 0, nCellcount_output = 0;

            for (int n = 0; n < bGroupUse.Length; n++)	//220829_ilyoung_타워그룹추가
            {
                string strEqinfo = string.Format("TWR{0}", n + 1);

                if (bGroupUse[n])
                {
                    Fnc_Init_datagrid2(0);

                    InitLabel();

                    Fnc_Process_GetINOUT_mtlinfo(0, strEqinfo, Double.Parse(strStart), Double.Parse(strEnd));

                    nGcount_input = dataGridView_input.RowCount;
                    nGcount_return = dataGridView_return.RowCount;
                    nGcount_output = dataGridView_output.RowCount;

                    for (int i = 0; i < nGcount_input; i++)
                    {
                        xlWorkSheet1.Cells[2 + nCellcount_input, 2] = nCellcount_input + 1;
                        xlWorkSheet1.Cells[2 + nCellcount_input, 3] = dataGridView_input.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 4] = dataGridView_input.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 5] = dataGridView_input.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 6] = dataGridView_input.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 7] = strEqinfo;

                        nCellcount_input++;
                    }

                    for (int i = 0; i < nGcount_return; i++)
                    {
                        xlWorkSheet2.Cells[2 + nCellcount_return, 2] = nCellcount_return + 1;
                        xlWorkSheet2.Cells[2 + nCellcount_return, 3] = dataGridView_return.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 4] = dataGridView_return.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 5] = dataGridView_return.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 6] = dataGridView_return.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 7] = strEqinfo;

                        nCellcount_return++;
                    }

                    for (int i = 0; i < nGcount_output; i++)
                    {
                        xlWorkSheet3.Cells[2 + nCellcount_output, 2] = nCellcount_output + 1;
                        xlWorkSheet3.Cells[2 + nCellcount_output, 3] = dataGridView_output.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 4] = dataGridView_output.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 5] = dataGridView_output.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 6] = dataGridView_output.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 7] = strEqinfo;

                        nCellcount_output++;
                    }
                }
            }

            xlWorkSheet1.Columns.AutoFit();
            xlWorkSheet2.Columns.AutoFit();
            xlWorkSheet3.Columns.AutoFit();
            ///////////////////////////////////////////////////
            /////////////////////////////////////////
            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet1);
            Marshal.ReleaseComObject(xlWorkSheet2);
            Marshal.ReleaseComObject(xlWorkSheet3);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            System.Diagnostics.Process.Start(strPath);            
        }

        public void Fnc_ExcelCreate_INOUTInfo_Detail(string strPath, string strStart, string strEnd)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet1;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;

            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet2 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet3 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);

            xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet1.Name = "입고";

            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet2.Name = "리턴";

            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);
            xlWorkSheet3.Name = "출고";

            xlWorkSheet1.Cells[1, 2] = "No";
            xlWorkSheet1.Cells[1, 3] = "일자";
            xlWorkSheet1.Cells[1, 4] = "시간";
            xlWorkSheet1.Cells[1, 5] = "SID";
            xlWorkSheet1.Cells[1, 6] = "Lot#";
            xlWorkSheet1.Cells[1, 7] = "UID";
            xlWorkSheet1.Cells[1, 8] = "TTL";
            xlWorkSheet1.Cells[1, 9] = "투입형태";
            xlWorkSheet1.Cells[1, 10] = "위치";
            xlWorkSheet1.Cells[1, 11] = "제조일";
            xlWorkSheet1.Cells[1, 12] = "제조사";
            xlWorkSheet1.Cells[1, 13] = "인치";

            xlWorkSheet2.Cells[1, 2] = "No";
            xlWorkSheet2.Cells[1, 3] = "일자";
            xlWorkSheet2.Cells[1, 4] = "시간";
            xlWorkSheet2.Cells[1, 5] = "SID";
            xlWorkSheet2.Cells[1, 6] = "Lot#";
            xlWorkSheet2.Cells[1, 7] = "UID";
            xlWorkSheet2.Cells[1, 8] = "TTL";
            xlWorkSheet2.Cells[1, 9] = "투입형태";
            xlWorkSheet2.Cells[1, 10] = "위치";
            xlWorkSheet2.Cells[1, 11] = "제조일";
            xlWorkSheet2.Cells[1, 12] = "제조사";
            xlWorkSheet2.Cells[1, 13] = "인치";

            xlWorkSheet3.Cells[1, 2] = "No";
            xlWorkSheet3.Cells[1, 3] = "일자";
            xlWorkSheet3.Cells[1, 4] = "시간";
            xlWorkSheet3.Cells[1, 5] = "SID";
            xlWorkSheet3.Cells[1, 6] = "Lot#";
            xlWorkSheet3.Cells[1, 7] = "UID";
            xlWorkSheet3.Cells[1, 8] = "TTL";
            xlWorkSheet3.Cells[1, 9] = "인치";
            xlWorkSheet3.Cells[1, 10] = "배출ID";
            xlWorkSheet3.Cells[1, 11] = "요청자";
            xlWorkSheet3.Cells[1, 12] = "위치";

            int nGcount_input = 0, nGcount_return = 0, nGcount_output = 0;
            int nCellcount_input = 0, nCellcount_return = 0, nCellcount_output = 0;

            for (int n = 0; n < bGroupUse.Length; n++)	//220829_ilyoung_타워그룹추가
            {
                string strEqinfo = string.Format("TWR{0}", n + 1);

                if (bGroupUse[n])
                {

                    Fnc_Init_datagrid2(1);
                    InitLabel();
                    Fnc_Process_GetINOUT_mtlinfo(1, strEqinfo, Double.Parse(strStart), Double.Parse(strEnd));

                    nGcount_input = dataGridView_input.RowCount;
                    nGcount_return = dataGridView_return.RowCount;
                    nGcount_output = dataGridView_output.RowCount;

                    for (int i = 0; i < nGcount_input; i++)
                    {
                        xlWorkSheet1.Cells[2 + nCellcount_input, 2] = nCellcount_input + 1;
                        xlWorkSheet1.Cells[2 + nCellcount_input, 3] = dataGridView_input.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 4] = dataGridView_input.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 5] = dataGridView_input.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 6] = dataGridView_input.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 7] = dataGridView_input.Rows[i].Cells[5].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 8] = dataGridView_input.Rows[i].Cells[6].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 9] = dataGridView_input.Rows[i].Cells[7].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 10] = dataGridView_input.Rows[i].Cells[8].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 11] = dataGridView_input.Rows[i].Cells[9].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 12] = dataGridView_input.Rows[i].Cells[10].Value.ToString();
                        xlWorkSheet1.Cells[2 + nCellcount_input, 13] = dataGridView_input.Rows[i].Cells[11].Value.ToString();
                        //xlWorkSheet1.Cells[2 + nCellcount_input, 14] = strEqinfo;

                        nCellcount_input++;
                    }

                    for (int i = 0; i < nGcount_return; i++)
                    {
                        xlWorkSheet2.Cells[2 + nCellcount_return, 2] = nCellcount_return + 1;
                        xlWorkSheet2.Cells[2 + nCellcount_return, 3] = dataGridView_return.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 4] = dataGridView_return.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 5] = dataGridView_return.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 6] = dataGridView_return.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 7] = dataGridView_return.Rows[i].Cells[5].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 8] = dataGridView_return.Rows[i].Cells[6].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 9] = dataGridView_return.Rows[i].Cells[7].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 10] = dataGridView_return.Rows[i].Cells[8].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 11] = dataGridView_return.Rows[i].Cells[9].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 12] = dataGridView_return.Rows[i].Cells[10].Value.ToString();
                        xlWorkSheet2.Cells[2 + nCellcount_return, 13] = dataGridView_return.Rows[i].Cells[11].Value.ToString();
                        //xlWorkSheet2.Cells[2 + nCellcount_return, 14] = strEqinfo;

                        nCellcount_return++;
                    }

                    for (int i = 0; i < nGcount_output; i++)
                    {
                        xlWorkSheet3.Cells[2 + nCellcount_output, 2] = nCellcount_output + 1;
                        xlWorkSheet3.Cells[2 + nCellcount_output, 3] = dataGridView_output.Rows[i].Cells[1].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 4] = dataGridView_output.Rows[i].Cells[2].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 5] = dataGridView_output.Rows[i].Cells[3].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 6] = dataGridView_output.Rows[i].Cells[4].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 7] = dataGridView_output.Rows[i].Cells[5].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 8] = dataGridView_output.Rows[i].Cells[6].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 9] = dataGridView_output.Rows[i].Cells[7].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 10] = dataGridView_output.Rows[i].Cells[8].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 11] = dataGridView_output.Rows[i].Cells[9].Value.ToString();
                        xlWorkSheet3.Cells[2 + nCellcount_output, 12] = dataGridView_output.Rows[i].Cells[10].Value.ToString();
                        //xlWorkSheet3.Cells[2 + nCellcount_output, 12] = strEqinfo;

                        nCellcount_output++;
                    }
                }
            }

            xlWorkSheet1.Columns.AutoFit();
            xlWorkSheet2.Columns.AutoFit();
            xlWorkSheet3.Columns.AutoFit();
            ///////////////////////////////////////////////////
            /////////////////////////////////////////
            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet1);
            Marshal.ReleaseComObject(xlWorkSheet2);
            Marshal.ReleaseComObject(xlWorkSheet3);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            System.Diagnostics.Process.Start(strPath);
        }

        public void Fnc_ExcelCreate_InventoryInfo_Detail(string strPath, int nType)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet1;
            Excel.Worksheet xlWorkSheet2;
            Excel.Worksheet xlWorkSheet3;
            Excel.Worksheet xlWorkSheet4;   //220829_ilyoung_타워그룹추가
            Excel.Worksheet xlWorkSheet5;	//220829_ilyoung_타워그룹추가



            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet2 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet3 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet4 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);
            xlWorkSheet5 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);


            /////save////////
            int nCellcount = 0;
            
            xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet1.Name = "Group 1";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR1");

            int nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet1.Cells[1, 2] = "No";
            xlWorkSheet1.Cells[1, 3] = "SID";
            xlWorkSheet1.Cells[1, 4] = "Batch#";
            xlWorkSheet1.Cells[1, 5] = "UID";
            xlWorkSheet1.Cells[1, 6] = "Qty";            
            xlWorkSheet1.Cells[1, 7] = "투입형태";
            xlWorkSheet1.Cells[1, 8] = "위치";
            xlWorkSheet1.Cells[1, 9] = "제조일";
            xlWorkSheet1.Cells[1, 10] = "투입일";
            xlWorkSheet1.Cells[1, 11] = "제조사";
            xlWorkSheet1.Cells[1, 12] = "인치";

            if (bGroupUse[0])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet1.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet1.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet1.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet1.Columns.AutoFit();

            /////////////////////////////////////////////////////////////////////////////////////
            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet2.Name = "Group 2";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR2");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet2.Cells[1, 2] = "No";
            xlWorkSheet2.Cells[1, 3] = "SID";
            xlWorkSheet2.Cells[1, 4] = "Batch#";
            xlWorkSheet2.Cells[1, 5] = "UID";
            xlWorkSheet2.Cells[1, 6] = "Qty";            
            xlWorkSheet2.Cells[1, 7] = "투입형태";
            xlWorkSheet2.Cells[1, 8] = "위치";
            xlWorkSheet2.Cells[1, 9] = "제조일";
            xlWorkSheet2.Cells[1, 10] = "투입일";
            xlWorkSheet2.Cells[1, 11] = "제조사";
            xlWorkSheet2.Cells[1, 12] = "인치";

            if (bGroupUse[1])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet2.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet2.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet2.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet2.Columns.AutoFit();
            /////////////////////////////////////////////////////////////////////////////////////
            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);
            xlWorkSheet3.Name = "Group 3";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR3");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet3.Cells[1, 2] = "No";
            xlWorkSheet3.Cells[1, 3] = "SID";
            xlWorkSheet3.Cells[1, 4] = "Batch#";
            xlWorkSheet3.Cells[1, 5] = "UID";
            xlWorkSheet3.Cells[1, 6] = "Qty";            
            xlWorkSheet3.Cells[1, 7] = "투입형태";
            xlWorkSheet3.Cells[1, 8] = "위치";
            xlWorkSheet3.Cells[1, 9] = "제조일";
            xlWorkSheet3.Cells[1, 10] = "투입일";
            xlWorkSheet3.Cells[1, 11] = "제조사";
            xlWorkSheet3.Cells[1, 12] = "인치";

            if (bGroupUse[2])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet3.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet3.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet3.Columns.AutoFit();

            /////////////////////////////////////////
            ///
            /////////////////////////////////////////////////////////////////////////////////////	//220829_ilyoung_타워그룹추가
            xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(4);
            xlWorkSheet3.Name = "Group 4";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR4");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet3.Cells[1, 2] = "No";
            xlWorkSheet3.Cells[1, 3] = "SID";
            xlWorkSheet3.Cells[1, 4] = "Batch#";
            xlWorkSheet3.Cells[1, 5] = "UID";
            xlWorkSheet3.Cells[1, 6] = "Qty";
            xlWorkSheet3.Cells[1, 7] = "투입형태";
            xlWorkSheet3.Cells[1, 8] = "위치";
            xlWorkSheet3.Cells[1, 9] = "제조일";
            xlWorkSheet3.Cells[1, 10] = "투입일";
            xlWorkSheet3.Cells[1, 11] = "제조사";
            xlWorkSheet3.Cells[1, 12] = "인치";

            if (bGroupUse[3])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet3.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet3.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet4.Columns.AutoFit();

            /////////////////////////////////////////	//220829_ilyoung_타워그룹추가
            ///
            /////////////////////////////////////////////////////////////////////////////////////	//220829_ilyoung_타워그룹추가
            xlWorkSheet5 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(5);
            xlWorkSheet5.Name = "Group 5";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo(1, "TWR5");

            nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet3.Cells[1, 2] = "No";
            xlWorkSheet3.Cells[1, 3] = "SID";
            xlWorkSheet3.Cells[1, 4] = "Batch#";
            xlWorkSheet3.Cells[1, 5] = "UID";
            xlWorkSheet3.Cells[1, 6] = "Qty";
            xlWorkSheet3.Cells[1, 7] = "투입형태";
            xlWorkSheet3.Cells[1, 8] = "위치";
            xlWorkSheet3.Cells[1, 9] = "제조일";
            xlWorkSheet3.Cells[1, 10] = "투입일";
            xlWorkSheet3.Cells[1, 11] = "제조사";
            xlWorkSheet3.Cells[1, 12] = "인치";

            if (bGroupUse[4])
            {
                for (int i = 0; i < nGcount; i++)
                {
                    xlWorkSheet3.Cells[2 + nCellcount, 2] = nCellcount + 1;
                    xlWorkSheet3.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                    xlWorkSheet3.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                    nCellcount++;
                }
            }
            xlWorkSheet5.Columns.AutoFit();
            /////////////////////////////////////////	//220829_ilyoung_타워그룹추가
            ///

            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet1);
            Marshal.ReleaseComObject(xlWorkSheet2);
            Marshal.ReleaseComObject(xlWorkSheet3);
            Marshal.ReleaseComObject(xlWorkSheet4);	//220829_ilyoung_타워그룹추가
            Marshal.ReleaseComObject(xlWorkSheet5);	//220829_ilyoung_타워그룹추가

            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            //IsDateGathering = false;

            if (nType == 0)
            {
                //string strMsg = "파일(타워재고 상세 정보)  저장 완료! 경로:" + strPath;
                //MessageBox.Show(strMsg);

                System.Diagnostics.Process.Start(strPath);
            }
        }

        public void Fnc_ExcelCreate_InventoryInfo_Detail_All(string strPath, int nType)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet1;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);           

            /////save////////
            int nCellcount = 0;

            xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet1.Name = "상세정보";

            Fnc_Init_datagrid(1); //상세 정보
            Fnc_Process_GetMaterialinfo_DetailAll();

            int nGcount = dataGridView_info.RowCount;
            nCellcount = 0;

            xlWorkSheet1.Cells[1, 2] = "No";
            xlWorkSheet1.Cells[1, 3] = "SID";
            xlWorkSheet1.Cells[1, 4] = "Batch#";
            xlWorkSheet1.Cells[1, 5] = "UID";
            xlWorkSheet1.Cells[1, 6] = "Qty";
            xlWorkSheet1.Cells[1, 7] = "투입형태";
            xlWorkSheet1.Cells[1, 8] = "위치";
            xlWorkSheet1.Cells[1, 9] = "제조일";
            xlWorkSheet1.Cells[1, 10] = "투입일";
            xlWorkSheet1.Cells[1, 11] = "제조사";
            xlWorkSheet1.Cells[1, 12] = "인치";

            
            for (int i = 0; i < nGcount; i++)
            {
                string stwr = dataGridView_info.Rows[i].Cells[8].Value.ToString().Substring(0, 1);

                xlWorkSheet1.Cells[2 + nCellcount, 2] = nCellcount + 1;
                xlWorkSheet1.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                xlWorkSheet1.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                xlWorkSheet1.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                xlWorkSheet1.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                xlWorkSheet1.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                xlWorkSheet1.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                xlWorkSheet1.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                xlWorkSheet1.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                xlWorkSheet1.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                xlWorkSheet1.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();

                nCellcount++;
            }              

            xlWorkSheet1.Columns.AutoFit();

            /////////////////////////////////////////
            ///

            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet1);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            //IsDateGathering = false;

            if (nType == 0)
            {
                //string strMsg = "파일(타워재고 상세 정보)  저장 완료! 경로:" + strPath;
                //MessageBox.Show(strMsg);

                System.Diagnostics.Process.Start(strPath);
            }
        }

        private void comboBox_group_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nType = comboBox_type.SelectedIndex; //0: SID, 1:Detail info
            int nGroup = comboBox_group.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();

            IsDateGathering = true;

            Fnc_Init_datagrid(nType);

            //if (nGroup != 7)
            if (nGroup != comboBox_group.Items.Count) //210824_Sangik.choi_타워그룹추가	//220829_ilyoung_타워그룹추가
                Fnc_Process_GetMaterialinfo(nType, strEquipid);
            else
            {
                Fnc_Process_GetMaterialinfo_All(nType);
            }

            IsDateGathering = false;
        }

        private void comboBox_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nType = comboBox_type.SelectedIndex;

            Fnc_Init_datagrid(nType);

            if (AMM_Main.nSelectedWin == 2)
            {
                int nGroup = comboBox_group.SelectedIndex + 1;

                string strEquipid = "TWR" + nGroup.ToString();

                IsDateGathering = true;

                Fnc_Init_datagrid(nType);

                //if (nGroup != 7)
                if (nGroup != comboBox_group.Items.Count) //210824_Sangik.choi_타워그룹추가	//220829_ilyoung_타워그룹추가
                    Fnc_Process_GetMaterialinfo(nType, strEquipid);
                else
                {
                    Fnc_Process_GetMaterialinfo_All(nType);
                }

                IsDateGathering = false;
            }
            
        }

        private void comboBox_searchtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox_mtlinput.Text = "";
            textBox_mtlinput.Focus();

            label_info1.Text = "-";
            label_info2.Text = "자재 없음";
            label_info3.Text = "-";
            label_info4.Text = "-";

            int n = comboBox_searchtype.SelectedIndex;

            if (n == 1)
            {
                comboBox_sid.Items.Clear();
                comboBox_sid.Items.Add("Reel ID");
                comboBox_sid.SelectedIndex = 0;
            }
        }

        private void comboBox_type2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nType = comboBox_type2.SelectedIndex;

            Fnc_Init_datagrid2(nType);

            if (AMM_Main.nSelectedWin == 2)
            {
                IsDateGathering = true;

                string strDate_st = "", strDate_ed = "";
                strDate_st = strTimeset_date_st.Replace("-", string.Empty);
                strDate_st = strDate_st.Trim();
                strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

                strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
                strDate_ed = strDate_ed.Trim();
                strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

                int nGroup = -1;
                string strEquipid = "";

                if (bSearch_sid)
                {
                    //int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
                    comboBox_group2.Text = "전체 조회";

                    Fnc_Init_datagrid2(nType);

                    if (strDate_st == "" || strDate_st == "")
                    {
                        IsDateGathering = false;
                        return;
                    }

                    Fnc_Process_GetINOUT_mtlinfo_Sid(nType, textBox_sid.Text, Double.Parse(strDate_st), Double.Parse(strDate_ed));
                }
                else
                {
                    //int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
                    nGroup = comboBox_group2.SelectedIndex + 1;

                    strEquipid = "TWR" + nGroup.ToString();

                    Fnc_Init_datagrid2(nType);

                    if (strDate_st == "" || strDate_st == "")
                    {
                        IsDateGathering = false;
                        return;
                    }

                    InitLabel();

                    if (nGroup != comboBox_group2.Items.Count) //210909_Sangik.choi_입출고정보 7번그룹 추가 //220829_ilyoung_타워그룹추가
                        Fnc_Process_GetINOUT_mtlinfo(nType, strEquipid, Double.Parse(strDate_st), Double.Parse(strDate_ed));
                    else
                    {
                        for(int i = 0; i < comboBox_group2.Items.Count; i++)
                        {
                            Fnc_Process_GetINOUT_mtlinfo(nType, "TWR"+i.ToString(), Double.Parse(strDate_st), Double.Parse(strDate_ed));
                        }
                    }

                }                

                IsDateGathering = false;
            }
        }

        private void comboBox_group2_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsDateGathering = true;

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;            

            int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
            int nGroup = comboBox_group2.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();

            Fnc_Init_datagrid2(nType);

            if(strDate_st == "" || strDate_st == "")
            {
                IsDateGathering = false;
                return;
            }

            InitLabel();

            if (bSearch_sid)
            {
                Fnc_Process_GetINOUT_mtlinfo_Sid2(nType, strEquipid, textBox_sid.Text, Double.Parse(strDate_st), Double.Parse(strDate_ed));
            }
            else
            {
                if (nGroup != comboBox_group2.Items.Count)//210909_Sangik.choi_입출고정보 7번그룹 추가 //220829_ilyoung_타워그룹추가
                    Fnc_Process_GetINOUT_mtlinfo(nType, strEquipid, Double.Parse(strDate_st), Double.Parse(strDate_ed));
                else
                    Fnc_Process_GetINOUT_mtlinfo_Sid2(nType, strEquipid, textBox_sid.Text, Double.Parse(strDate_st), Double.Parse(strDate_ed));
            }            

            IsDateGathering = false;
        }

        private void button_excel2_Click(object sender, EventArgs e)
        {
            bExcel_Start = false;

            nExcelIndex = 1;

            Form_Excel Excel_Form = new Form_Excel();
            Excel_Form.ShowDialog();

            if (!bExcel_Start)
            {
                return;
            }

            IsDateGathering = true;

            string strPath = strExcelfilePath + "\\";
            string stSaveTime_st = "", stSaveTime_ed = "", stSaveDate_st = "", stSaveDate_ed = "";

            //string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            stSaveTime_st = label_Value_time_st.Text.Replace(":", "_");
            stSaveTime_ed = label_Value_time_ed.Text.Replace(":", "_");
            //stSaveTime_ed = strHead.Replace(":", "_");
            //stSaveTime_ed = stSaveTime_ed.Substring(0, 5);
            stSaveDate_st = label_Value_date_st.Text.Replace("-", string.Empty);
            stSaveDate_ed = label_Value_date_ed.Text.Replace("-", string.Empty);
            //stSaveDate_ed = strToday.Replace("-", string.Empty);

            string strDate = stSaveDate_st + "_" + stSaveTime_st + "~" + stSaveDate_ed + "_" + stSaveTime_ed;
            strPath = strPath + "ITS_" + strDate;
            string strPathName = "";

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

            //strDate_ed = strToday.Replace("-", string.Empty);
            //strHead = strHead.Replace(":", string.Empty);
            //strDate_ed = strDate_ed.Trim();
            //strHead = strHead.Trim();
            //strDate_ed = strDate_ed + strHead;

            if (bExcelUse[2])//입출고 SID
            {
                strPathName = strPath + "_입출고SID.xlsx";

                if (File.Exists(strPathName))
                {
                    string path = strPathName;
                    bool available = true;
                    try
                    {
                        using (FileStream fs = File.Open(path, FileMode.Open))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        string str = string.Format("{0}", ex);
                        //Fnc_SaveLog("Exception,Excel 파일 생성 실패 " + ex.ToString());
                        available = false;
                    }

                    if (!available)
                    {
                        IsDateGathering = false;
                        MessageBox.Show("[입출고 SID]같은 파일의 이름이 열려 있습니다.  해당 파일을 닫고 다시 실행 하십시오.");
                        return;
                    }
                    else
                    {
                        File.Delete(strPathName);
                    }
                }

                Fnc_ExcelCreate_INOUTInfo_SID(strPathName, strDate_st, strDate_ed);
            }

            if (bExcelUse[3])//입출고 상세 정보
            {
                strPathName = strPath + "_입출고상세정보.xlsx";

                if (File.Exists(strPathName))
                {
                    string path = strPathName;
                    bool available = true;
                    try
                    {
                        using (FileStream fs = File.Open(path, FileMode.Open))
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        string str = string.Format("{0}", ex);
                        //Fnc_SaveLog("Exception,Excel 파일 생성 실패 " + ex.ToString());
                        available = false;
                    }

                    if (!available)
                    {
                        IsDateGathering = false;
                        MessageBox.Show("[일출고 상세 정보]같은 파일의 이름이 열려 있습니다.  해당 파일을 닫고 다시 실행 하십시오.");
                        return;
                    }
                    else
                    {
                        File.Delete(strPathName);
                    }
                }

                Fnc_ExcelCreate_INOUTInfo_Detail(strPathName, strDate_st, strDate_ed);
            }

            Fnc_Update_timeset();

            IsDateGathering = false;
        }

        private void button_update_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button_update, "정보 업데이트");
        }

        private void button_excel_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button_excel, "액셀 저장");
        }

        private void button_timeset_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button_timeset, "조회 시간 설정");
        }

        private void button_excel2_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button_excel2, "액셀 저장");
        }

        private void comboBox_sid_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBox_searchtype.SelectedIndex;

            if (n == 0)
            {
                //n = comboBox_sid.SelectedIndex;
                string str = comboBox_sid.SelectedItem.ToString();

                textBox_mtlinput.Text = str;
                Fnc_ProcessFind(0, str);

                textBox_mtlinput.Text = "";
            }
        }

        private void textBox_find_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                //Find
                Fnc_Find(textBox_find.Text);
            }
        }

        public void Fnc_Find(string strFind)
        {
            dataGridView_info.ClearSelection();

            int nCount = dataGridView_info.RowCount;
            int nCount2 = dataGridView_info.ColumnCount;

            bool bfind = false;

            for (int m = 1; m < nCount2; m++)
            {
                for (int n = 0; n < nCount; n++)
                {
                    string str = dataGridView_info.Rows[n].Cells[m].Value.ToString();

                    if (str == strFind)
                    {
                        dataGridView_info.Rows[n].Cells[m].Selected = true;
                        dataGridView_info.FirstDisplayedScrollingRowIndex = n;
                        bfind = true;
                        n = nCount;  m = nCount2;
                    }                    
                }
            }

            if (bfind)
                return;

            for (int m = 1; m < nCount2; m++)
            {
                for (int n = 0; n < nCount; n++)
                {
                    string str = dataGridView_info.Rows[n].Cells[m].Value.ToString();

                    if (str.Contains(strFind))
                    {
                        dataGridView_info.Rows[n].Cells[m].Selected = true;
                        dataGridView_info.FirstDisplayedScrollingRowIndex = n;
                        bfind = true;
                        n = nCount; m = nCount2;
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Fnc_Update();
        }

        public void Fnc_Update()
        {
            if(bUpdate_Timer)
                Fnc_Process_CalMaterialInfo();
                //Fnc_Init_datagrid_capa();
        }

        private void dataGridView_info_MouseUp(object sender, MouseEventArgs e)
        {
            nSum = 0;
            foreach (DataGridViewCell cell in dataGridView_info.SelectedCells)
            {
                if (cell.ColumnIndex == 2)
                {
                    var Value = dataGridView_info.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value.ToString();
                    nSum = nSum + Int32.Parse(Value);
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (comboBox_type.SelectedIndex == 0 && nSum != 0)
            {
                string str = string.Format("합계: {0}", nSum);
                MessageBox.Show(str);
            }
        }

/*        private void button_dbload_Click(object sender, EventArgs e)
        {
            int n = comboBox_sel.SelectedIndex;

            if (n == 0)
            {
                Fnc_Process_GetMaterials_Tower1();
                Fnc_Process_GetAMMinfo("TWR1");
            }
            else if (n == 1)
            {
                Fnc_Process_GetMaterials_Tower2();
                Fnc_Process_GetAMMinfo("TWR2");
            }
            else if (n == 2)
            {
                Fnc_Process_GetMaterials_Tower3();
                Fnc_Process_GetAMMinfo("TWR3");
            }

            dataGridView_missmatch.Columns.Clear();
            dataGridView_missmatch.Rows.Clear();

            nDbUpdate = 1;
        }

        private void button_missmatch_Click(object sender, EventArgs e)
        {
            if (nDbUpdate != 1)
            {
                MessageBox.Show("DB 조회가 되지 않았습니다. DB 조회를 먼저 진행 하십시오.");
                return;
            }

            dataGridView_missmatch.Columns.Clear();
            dataGridView_missmatch.Rows.Clear();

            //dataGridView_missmatch.Columns.Add("NO", "NO");
            //dataGridView_missmatch.Columns.Add("UID", "UID");
            //dataGridView_missmatch.Columns.Add("위치", "위치");
            //dataGridView_missmatch.Columns.Add("MISS", "MISS");

            dataGridView_missmatch.Columns.Add("NO", "NO");
            dataGridView_missmatch.Columns.Add("SID", "SID");
            dataGridView_missmatch.Columns.Add("Batch#", "Batch#");
            dataGridView_missmatch.Columns.Add("UID", "UID");
            dataGridView_missmatch.Columns.Add("Qty", "Qty");
            dataGridView_missmatch.Columns.Add("투입형태", "투입형태");
            dataGridView_missmatch.Columns.Add("위치", "위치");
            dataGridView_missmatch.Columns.Add("제조일", "제조일");
            dataGridView_missmatch.Columns.Add("투입일", "투입일");
            dataGridView_missmatch.Columns.Add("제조사", "제조사");
            dataGridView_missmatch.Columns.Add("인치", "인치");
            dataGridView_missmatch.Columns.Add("MISS", "MISS");

            int nStart = 1;

            nStart = Fnc_Missmatch_ASMcompare(nStart);
            Fnc_Missmatch_AMMcompare(nStart);

            nDbUpdate = 2;
        }

        private void button_sync_Click(object sender, EventArgs e)
        {
            if (nDbUpdate != 2)
            {
                MessageBox.Show("Missmatch 확인을 먼저 하십시오");
                return;
            }

            if (IsDateGathering == true)
                return;

            //경고 메세지
            DialogResult ret = MessageBox.Show("동기화 하시겠습까?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ret != DialogResult.Yes)
                return;

            IsDateGathering = true;

            //AMM delete
            int n = comboBox_sel.SelectedIndex;

            string strEqid = "";
            if (n == 0)
                strEqid = "TWR1";
            else if (n == 1)
                strEqid = "TWR2";
            else if (n == 2)
                strEqid = "TWR3";

            int nCount = dataGridView_missmatch.Rows.Count;

            List<StorageData_Compare> uploadList = new List<StorageData_Compare>();

            for (int i = 0; i < nCount; i++)
            {
                StorageData_Compare data = new StorageData_Compare();

                data.SID = dataGridView_missmatch.Rows[i].Cells[1].Value.ToString(); //SID
                data.LOTID = dataGridView_missmatch.Rows[i].Cells[2].Value.ToString(); //LOTOD
                data.UID = dataGridView_missmatch.Rows[i].Cells[3].Value.ToString(); //UID
                data.Quantity = dataGridView_missmatch.Rows[i].Cells[4].Value.ToString(); //QTY
                data.Input_type = dataGridView_missmatch.Rows[i].Cells[5].Value.ToString(); //투입 형태
                data.Tower_no = dataGridView_missmatch.Rows[i].Cells[6].Value.ToString(); //위치
                data.Production_date = dataGridView_missmatch.Rows[i].Cells[7].Value.ToString(); //제조일
                data.Input_date = dataGridView_missmatch.Rows[i].Cells[8].Value.ToString(); //투입일
                data.Manufacturer = dataGridView_missmatch.Rows[i].Cells[9].Value.ToString(); //제조사
                data.Inch = dataGridView_missmatch.Rows[i].Cells[10].Value.ToString(); //인치
                data.Miss = dataGridView_missmatch.Rows[i].Cells[11].Value.ToString(); //Miss Type

                uploadList.Add(data);
            }
            //Tower번호;UID;SID;LOTID;QTY;제조사;제조일;INCH;투입TYPE

            int nNGcount = 0;
            foreach (var item in uploadList)
            {
                if (item.Miss == "AMM")
                {
                    string strFormat = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", item.Tower_no, item.UID, item.SID, item.LOTID, item.Quantity,
                        item.Manufacturer, item.Production_date, "NO INFO", "CART");

                    string strJudge = AMM_Main.AMM.SetLoadComplete("AJ54100", strEqid, strFormat, false);

                    if (strJudge == "NG")
                    {
                        nNGcount++;
                    }
                    else if (strJudge == "DUPLICATE")
                    {
                        nNGcount++;
                    }
                }
                else if (item.Miss == "ASM")
                {
                    string strJudge = AMM_Main.AMM.Delete_MTL_Info(item.UID);

                    if (strJudge == "NG")
                    {
                        nNGcount++;
                    }
                }

                Application.DoEvents();
                Thread.Sleep(100);
            }

            if (nNGcount > 0)
            {
                string str = string.Format("실패 {0}개", nNGcount);
                MessageBox.Show(str);
            }

            dataGridView_missmatch.Columns.Clear();
            dataGridView_missmatch.Rows.Clear();

            IsDateGathering = false;

            MessageBox.Show("완료 되었습니다.");

            int nIndex = comboBox_sel.SelectedIndex;

            if (nIndex == 0)
            {
                Fnc_Process_GetMaterials_Tower1();
                Fnc_Process_GetAMMinfo("TWR1");
            }
            else if (nIndex == 1)
            {
                Fnc_Process_GetMaterials_Tower2();
                Fnc_Process_GetAMMinfo("TWR2");
            }
            else if (nIndex == 2)
            {
                Fnc_Process_GetMaterials_Tower3();
                Fnc_Process_GetAMMinfo("TWR3");
            }

            nDbUpdate = 0;
        }*/

        private void textBox_mtlinput_KeyPress(object sender, KeyPressEventArgs e)
        {
            int n = comboBox_searchtype.SelectedIndex;

            if (n == 0) //SID
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
            }

            if (e.KeyChar == (char)13)
            {
                string strid = "";
                int nLength = 0;

                strid = textBox_mtlinput.Text;
                nLength = strid.Length;

                if (nLength < 3)
                    return;

                Fnc_ProcessFind(n, strid);

                if (nLength == 4)
                {
                    int nCombocount = comboBox_sid.Items.Count;

                    if (nCombocount > 0 && n == 0)
                    {
                        comboBox_sid.SelectedIndex = 0;
                    }
                    else
                    {
                        textBox_mtlinput.Text = "";
                        textBox_mtlinput.Focus();
                    }
                }
                else
                {
                    comboBox_sid.Items.Clear();
                    comboBox_sid.Text = "";
                }

                label_scount.Text = comboBox_sid.Items.Count.ToString();
            }
        }

        private void button_timeset_Click(object sender, EventArgs e)
        {
            Form_Timeset Timeset_Form = new Form_Timeset();
            Timeset_Form.ShowDialog();

            IsDateGathering = true;

            label_Value_date_st.Text = strTimeset_date_st;
            label_Value_date_ed.Text = strTimeset_date_ed;
            label_Value_time_st.Text = strTimeset_hour_st + ":" + strTimeset_Min_st;
            label_Value_time_ed.Text = strTimeset_hour_ed + ":" + strTimeset_Min_ed;

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

            int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
            int nGroup = comboBox_group2.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();

            if(bSearch_sid)
            {
                button_search.Visible = true;
                textBox_sid.Visible = true;
                label_sid.Visible = true;

                comboBox_type2.SelectedIndex = 0;
                nType = 0;
                Fnc_Init_datagrid2(nType);

                textBox_sid.Focus();
            }
            else
            {
                button_search.Visible = false;
                textBox_sid.Visible = false;
                label_sid.Visible = false;

                Fnc_Init_datagrid2(nType);

                InitLabel();

                if (nGroup != comboBox_group2.Items.Count)//220829_ilyoung_타워그룹추가
                    Fnc_Process_GetINOUT_mtlinfo(nType, strEquipid, Double.Parse(strDate_st), Double.Parse(strDate_ed));
                else
                {
                    for(int i = 0; i  < comboBox_group2.Items.Count -1; i++)
                    {
                        Fnc_Process_GetINOUT_mtlinfo(nType, "TWR"+ i.ToString(), Double.Parse(strDate_st), Double.Parse(strDate_ed));
                    }
                }
            }            

            IsDateGathering = false;
        }

        public void Fnc_Update_timeset()
        {
            IsDateGathering = true;

            DateTime dToday = DateTime.Now;

            strTimeset_date_st = string.Format("{0}-{1:00}-{2:00}", dToday.Year, dToday.Month, dToday.Day);
            strTimeset_date_ed = string.Format("{0}-{1:00}-{2:00}", dToday.Year, dToday.Month, dToday.Day);

            strTimeset_hour_st = "08";
            strTimeset_hour_ed = "17";
            strTimeset_Min_st = "30";
            strTimeset_Min_ed = "30";

            label_Value_date_st.Text = strTimeset_date_st;
            label_Value_date_ed.Text = strTimeset_date_ed;
            label_Value_time_st.Text = strTimeset_hour_st + ":" + strTimeset_Min_st;
            label_Value_time_ed.Text = strTimeset_hour_ed + ":" + strTimeset_Min_ed;

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

            int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
            int nGroup = comboBox_group2.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();

            Fnc_Init_datagrid2(nType);

            InitLabel();

            if (nGroup != comboBox_group2.Items.Count)//220829_ilyoung_타워그룹추가
                Fnc_Process_GetINOUT_mtlinfo(nType, strEquipid, Double.Parse(strDate_st), Double.Parse(strDate_ed));
            else
            {
                for (int i = 0; i < comboBox_group2.Items.Count; i++)
                {
                    Fnc_Process_GetINOUT_mtlinfo(nType, "TWR" + i.ToString(), Double.Parse(strDate_st), Double.Parse(strDate_ed));
                }
            }


            IsDateGathering = false;
        }

/*        public void Fnc_InitMSSql()
        {
            if (MSSql != null)
                return;

            string connectionStr = string.Format("server=10.133.146.151;database=SiplaceMaterialManager;user id=sa;password=Siplace.1");
            MSSql = new MsSqlManager(connectionStr);

            if (MSSql.OpenTest() == false)
            {
                MessageBox.Show("ASM DB연결 실패");
                bASMconnect = false;
            }
            else
            {
                bASMconnect = true;
            }
        }
*//*        private int Fnc_Process_GetAMMinfo(string strEquipid)
        {
            dataGridView_amm.Columns.Clear();
            dataGridView_amm.Rows.Clear();

            dataGridView_amm.Columns.Add("NO", "NO");
            dataGridView_amm.Columns.Add("SID", "SID");
            dataGridView_amm.Columns.Add("Batch#", "Batch#");
            dataGridView_amm.Columns.Add("UID", "UID");
            dataGridView_amm.Columns.Add("Qty", "Qty");
            dataGridView_amm.Columns.Add("투입형태", "투입형태");
            dataGridView_amm.Columns.Add("위치", "위치");
            dataGridView_amm.Columns.Add("제조일", "제조일");
            dataGridView_amm.Columns.Add("투입일", "투입일");
            dataGridView_amm.Columns.Add("제조사", "제조사");
            dataGridView_amm.Columns.Add("인치", "인치");

            var MtlList = AMM_Main.AMM.GetMTLInfo("AJ54100", strEquipid); // 수정필요

            strEquipid = strEquipid.Replace("TWR", "G"); //20200529

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                return nMtlCount;
            }

            List<AMM_StorageData> list = new List<AMM_StorageData>();

            for (int i = 0; i < MtlList.Rows.Count; i++)
            {
                AMM_StorageData data = new AMM_StorageData();

                data.UID = MtlList.Rows[i]["UID"].ToString(); data.UID = data.UID.Trim();
                data.SID = MtlList.Rows[i]["SID"].ToString(); data.SID = data.SID.Trim();
                data.Input_date = MtlList.Rows[i]["DATETIME"].ToString(); data.Input_date = data.Input_date.Trim();
                data.Tower_no = MtlList.Rows[i]["TOWER_NO"].ToString(); data.Tower_no = data.Tower_no.Trim();
                data.LOTID = MtlList.Rows[i]["LOTID"].ToString(); data.LOTID = data.LOTID.Trim();
                data.Quantity = MtlList.Rows[i]["QTY"].ToString(); data.Quantity = data.Quantity.Trim();
                data.Manufacturer = MtlList.Rows[i]["MANUFACTURER"].ToString(); data.Manufacturer = data.Manufacturer.Trim();
                data.Production_date = MtlList.Rows[i]["PRODUCTION_DATE"].ToString(); data.Production_date = data.Production_date.Trim();
                data.Inch = MtlList.Rows[i]["INCH_INFO"].ToString(); data.Inch = data.Inch.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();

                list.Add(data);
            }

            list.Sort(CompareStorageData_AMM);

            int nIndex = 1;

            foreach (var item in list)
            {
                string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                string strdate = item.Input_date;
                strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                    + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                dataGridView_amm.Rows.Add(new object[11] { nIndex++, item.SID, item.LOTID, item.UID, strnQty, item.Input_type, item.Tower_no, item.Production_date, strdate, item.Manufacturer, item.Inch });
            }

            return nMtlCount;
        }
        public void Fnc_Process_GetMaterials_Tower1()
        {
            string tid = "";

            tid = "T0101";
            var simmList1 = GetSIMMMaterialList(strASM_TowerLocation1, tid);

            dataGridView_asm.Columns.Clear();
            dataGridView_asm.Rows.Clear();

            dataGridView_asm.Columns.Add("NO", "NO");
            dataGridView_asm.Columns.Add("SID", "SID");
            dataGridView_asm.Columns.Add("Batch#", "Batch#");
            dataGridView_asm.Columns.Add("UID", "UID");
            dataGridView_asm.Columns.Add("Qty", "Qty");
            dataGridView_asm.Columns.Add("투입일", "투입일");
            dataGridView_asm.Columns.Add("제조일", "제조일");
            dataGridView_asm.Columns.Add("제조사", "제조사");
            dataGridView_asm.Columns.Add("위치", "위치");

            int idx = 1;
            foreach (var item in simmList1)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

            tid = "T0102";
            var simmList2 = GetSIMMMaterialList(strASM_TowerLocation1, tid);

            foreach (var item in simmList2)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

 
        }

        public void Fnc_Process_GetMaterials_Tower2()
        {
            string tid = "";

            tid = "T0201";
            var simmList1 = GetSIMMMaterialList(strASM_TowerLocation2, tid);

            dataGridView_asm.Columns.Clear();
            dataGridView_asm.Rows.Clear();

            dataGridView_asm.Columns.Add("NO", "NO");
            dataGridView_asm.Columns.Add("SID", "SID");
            dataGridView_asm.Columns.Add("Batch#", "Batch#");
            dataGridView_asm.Columns.Add("UID", "UID");
            dataGridView_asm.Columns.Add("Qty", "Qty");
            dataGridView_asm.Columns.Add("투입일", "투입일");
            dataGridView_asm.Columns.Add("제조일", "제조일");
            dataGridView_asm.Columns.Add("제조사", "제조사");
            dataGridView_asm.Columns.Add("위치", "위치");

            int idx = 1;
            foreach (var item in simmList1)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

            tid = "T0202";
            var simmList2 = GetSIMMMaterialList(strASM_TowerLocation2, tid);

            foreach (var item in simmList2)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }

 
        }

        public void Fnc_Process_GetMaterials_Tower3()
        {
            string tid = "";

            tid = "T0301";
            var simmList1 = GetSIMMMaterialList(strASM_TowerLocation3, tid);

            dataGridView_asm.Columns.Clear();
            dataGridView_asm.Rows.Clear();

            dataGridView_asm.Columns.Add("NO", "NO");
            dataGridView_asm.Columns.Add("SID", "SID");
            dataGridView_asm.Columns.Add("Batch#", "Batch#");
            dataGridView_asm.Columns.Add("UID", "UID");
            dataGridView_asm.Columns.Add("Qty", "Qty");
            dataGridView_asm.Columns.Add("투입일", "투입일");
            dataGridView_asm.Columns.Add("제조일", "제조일");
            dataGridView_asm.Columns.Add("제조사", "제조사");
            dataGridView_asm.Columns.Add("위치", "위치");

            int idx = 1;
            foreach (var item in simmList1)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }
            tid = "T0302";
            var simmList2 = GetSIMMMaterialList(strASM_TowerLocation3, tid);

            foreach (var item in simmList2)
            {
                dataGridView_asm.Rows.Add(new object[9] { idx, item.SID, item.LotID, item.UID, item.Quantity, item.Date_Input, item.Productiondate, item.Manufacturer, tid });
                idx++;
            }


        }
        public int Fnc_Missmatch_ASMcompare(int idx)
        {
            List<StorageData_Compare> asmList = new List<StorageData_Compare>();

            for (int i = 0; i < dataGridView_asm.Rows.Count; i++)
            {
                StorageData_Compare data = new StorageData_Compare();
                data.SID = dataGridView_asm.Rows[i].Cells[1].Value.ToString(); //SID
                data.LOTID = dataGridView_asm.Rows[i].Cells[2].Value.ToString(); //LOTID
                data.UID = dataGridView_asm.Rows[i].Cells[3].Value.ToString(); //UID
                data.Quantity = dataGridView_asm.Rows[i].Cells[4].Value.ToString(); //QTY
                data.Input_date = dataGridView_asm.Rows[i].Cells[5].Value.ToString(); //투입일
                data.Production_date = dataGridView_asm.Rows[i].Cells[6].Value.ToString(); //제조일
                data.Manufacturer = dataGridView_asm.Rows[i].Cells[7].Value.ToString(); //제조사
                data.Tower_no = dataGridView_asm.Rows[i].Cells[8].Value.ToString(); //위치

                if (data.UID != "")
                    asmList.Add(data);
            }

            asmList.Sort(CompareStorageData);

            List<StorageData_Compare> ammList = new List<StorageData_Compare>();

            for (int i = 0; i < dataGridView_amm.Rows.Count; i++)
            {
                StorageData_Compare data = new StorageData_Compare();
                data.SID = dataGridView_amm.Rows[i].Cells[1].Value.ToString(); //SID
                data.LOTID = dataGridView_amm.Rows[i].Cells[2].Value.ToString(); //LOTOD
                data.UID = dataGridView_amm.Rows[i].Cells[3].Value.ToString(); //UID
                data.Quantity = dataGridView_amm.Rows[i].Cells[4].Value.ToString(); //QTY
                data.Input_type = dataGridView_amm.Rows[i].Cells[5].Value.ToString(); //투입 형태
                data.Tower_no = dataGridView_amm.Rows[i].Cells[6].Value.ToString(); //위치
                data.Production_date = dataGridView_amm.Rows[i].Cells[7].Value.ToString(); //제조일
                data.Input_date = dataGridView_amm.Rows[i].Cells[8].Value.ToString(); //투입일
                data.Manufacturer = dataGridView_amm.Rows[i].Cells[9].Value.ToString(); //제조사
                data.Inch = dataGridView_amm.Rows[i].Cells[10].Value.ToString(); //인치

                if (data.UID != "")
                    ammList.Add(data);
            }

            ammList.Sort(CompareStorageData);

            var missmatchList = GetMissMatchList(asmList, ammList);

            foreach (var item in missmatchList)
            {
                dataGridView_missmatch.Rows.Add(new object[12] { idx++, item.SID, item.LOTID, item.UID, item.Quantity, item.Input_type, item.Tower_no,
                    item.Production_date, item.Input_date, item.Manufacturer,item.Inch, "AMM" });

                dataGridView_missmatch.Rows[idx - 2].DefaultCellStyle.BackColor = Color.White;
                dataGridView_missmatch.Rows[idx - 2].DefaultCellStyle.ForeColor = Color.Blue;
            }

            return idx;
        }

        public int Fnc_Missmatch_AMMcompare(int idx)
        {
            List<StorageData_Compare> asmList = new List<StorageData_Compare>();

            for (int i = 0; i < dataGridView_asm.Rows.Count; i++)
            {
                StorageData_Compare data = new StorageData_Compare();
                data.SID = dataGridView_asm.Rows[i].Cells[1].Value.ToString(); //SID
                data.LOTID = dataGridView_asm.Rows[i].Cells[2].Value.ToString(); //LOTID
                data.UID = dataGridView_asm.Rows[i].Cells[3].Value.ToString(); //UID
                data.Quantity = dataGridView_asm.Rows[i].Cells[4].Value.ToString(); //QTY
                data.Input_date = dataGridView_asm.Rows[i].Cells[5].Value.ToString(); //투입일
                data.Production_date = dataGridView_asm.Rows[i].Cells[6].Value.ToString(); //제조일
                data.Manufacturer = dataGridView_asm.Rows[i].Cells[7].Value.ToString(); //제조사
                data.Tower_no = dataGridView_asm.Rows[i].Cells[8].Value.ToString(); //위치

                if (data.UID != "")
                    asmList.Add(data);
            }

            asmList.Sort(CompareStorageData);

            List<StorageData_Compare> ammList = new List<StorageData_Compare>();

            for (int i = 0; i < dataGridView_amm.Rows.Count; i++)
            {
                StorageData_Compare data = new StorageData_Compare();
                data.SID = dataGridView_amm.Rows[i].Cells[1].Value.ToString(); //SID
                data.LOTID = dataGridView_amm.Rows[i].Cells[2].Value.ToString(); //LOTOD
                data.UID = dataGridView_amm.Rows[i].Cells[3].Value.ToString(); //UID
                data.Quantity = dataGridView_amm.Rows[i].Cells[4].Value.ToString(); //QTY
                data.Input_type = dataGridView_amm.Rows[i].Cells[5].Value.ToString(); //투입 형태
                data.Tower_no = dataGridView_amm.Rows[i].Cells[6].Value.ToString(); //위치
                data.Production_date = dataGridView_amm.Rows[i].Cells[7].Value.ToString(); //제조일
                data.Input_date = dataGridView_amm.Rows[i].Cells[8].Value.ToString(); //투입일
                data.Manufacturer = dataGridView_amm.Rows[i].Cells[9].Value.ToString(); //제조사
                data.Inch = dataGridView_amm.Rows[i].Cells[10].Value.ToString(); //인치

                if (data.UID != "")
                    ammList.Add(data);
            }

            ammList.Sort(CompareStorageData);

            var missmatchList = GetMissMatchList(ammList, asmList);

            foreach (var item in missmatchList)
            {
                dataGridView_missmatch.Rows.Add(new object[12] { idx++, item.SID, item.LOTID, item.UID, item.Quantity, item.Input_type, item.Tower_no,
                    item.Production_date, item.Input_date, item.Manufacturer,item.Inch, "ASM" });
                dataGridView_missmatch.Rows[idx - 2].DefaultCellStyle.BackColor = Color.White;
                dataGridView_missmatch.Rows[idx - 2].DefaultCellStyle.ForeColor = Color.Orange;
            }

            return idx;
        }*/

        public List<StorageData_Compare> GetMissMatchList(List<StorageData_Compare> source, List<StorageData_Compare> compare)
        {
            List<StorageData_Compare> retList = new List<StorageData_Compare>();
            List<string> compareList = new List<string>();

            foreach (var item in compare)
                compareList.Add(item.UID);

            for (int i = 0; i < source.Count; i++)
            {
                if (compareList.Contains(source[i].UID) == false)
                    retList.Add(source[i]);
            }

            return retList;
        }

        private void button_search_Click(object sender, EventArgs e)
        {
            if(textBox_sid.Text == "")
            {
                MessageBox.Show("SID 를 입력 하세요!");
                textBox_sid.Focus();
                return;
            }

            IsDateGathering = true;

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

            comboBox_type2.SelectedIndex = 0;
            comboBox_group2.Text = "전체 조회";

            int nType = comboBox_type2.SelectedIndex; //0: SID, 1:Detail info
            //int nGroup = comboBox_group2.SelectedIndex + 1;

            //string strEquipid = "TWR" + nGroup.ToString();

            Fnc_Init_datagrid2(nType);

            if (strDate_st == "" || strDate_st == "")
            {
                IsDateGathering = false;
                return;
            }

            Fnc_Process_GetINOUT_mtlinfo_Sid(nType, textBox_sid.Text, Double.Parse(strDate_st), Double.Parse(strDate_ed));
           
            IsDateGathering = false;
        }

        private void dataGridView_info_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_find_TextChanged(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click_1(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void comboBox_month_SelectedIndexChanged(object sender, EventArgs e)
        { 


        }

        private void dataGridView_longterm_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }




        //[210809_Sangik.choi_장기보관관리기능추가(이종명수석님)
        private void dataGridView_longterm_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            int current_count = dataGridView_LTlist.Rows.Count;
            string Judge_ready_insert = "";
            int nGroup = comboBox_L_group.SelectedIndex + 1;
            string strEquipid = "TWR" + nGroup.ToString();
            string strPickingid = label_pickid_LT.Text;

            string badge = textBox_badge.Text;

            if (current_count >= 20)
            {
                MessageBox.Show("배출 수량이 너무 많습니다.20개 초과!\n한 개 리스트에 자재 20개 까지 담을 수 있습니다.");
                return;
            }

            if ( badge == "")
            {
                MessageBox.Show("사번을 입력해주세요");
                return;

            }

             string user_check = AMM_Main.AMM.Check_LT_User(badge);


            if (user_check == "OK")
            {
                StorageData_Compare data = new StorageData_Compare();

                data.SID = dataGridView_longterm.Rows[e.RowIndex].Cells["SID"].Value.ToString(); //SID
                data.LOTID = dataGridView_longterm.Rows[e.RowIndex].Cells["Batch#"].Value.ToString(); //LOTOD
                data.UID = dataGridView_longterm.Rows[e.RowIndex].Cells["UID"].Value.ToString(); //UID
                data.Quantity = dataGridView_longterm.Rows[e.RowIndex].Cells["Qty"].Value.ToString(); //QTY
                data.Input_type = dataGridView_longterm.Rows[e.RowIndex].Cells["투입형태"].Value.ToString(); //투입 형태
                data.Tower_no = dataGridView_longterm.Rows[e.RowIndex].Cells["위치"].Value.ToString(); //위치
                data.Production_date = dataGridView_longterm.Rows[e.RowIndex].Cells["제조일"].Value.ToString(); //제조일
                data.Input_date = dataGridView_longterm.Rows[e.RowIndex].Cells["투입일"].Value.ToString(); //투입일
                data.Manufacturer = dataGridView_longterm.Rows[e.RowIndex].Cells["제조사"].Value.ToString(); //제조사
                data.Inch = dataGridView_longterm.Rows[e.RowIndex].Cells["인치"].Value.ToString(); //인치

                string strJudge = AMM_Main.AMM.GetPickingReadyinfo(data.UID);
                string strJudge2 = AMM_Main.AMM.GetPickingListinfo(data.UID);
                string strJudge3 = "OK";
                string strTowerNo = data.Tower_no.Substring(4, 1);

                if (bTowerUse[0] != true)
                {
                    if (strTowerNo == "1")
                        strJudge3 = "NG";
                }
                if (bTowerUse[1] != true)
                {
                    if (strTowerNo == "2")
                        strJudge3 = "NG";
                }



                if (strJudge == "OK" && strJudge2 == "OK" && strJudge3 == "OK")
                {

                    try //[210817_Sangik.choi_예외처리추가
                    {
                        Judge_ready_insert = AMM_Main.AMM.SetPicking_Readyinfo(AMM_Main.strDefault_linecode, strEquipid, strPickingid, data.UID, textBox_badge.Text, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, "AMM_SID");
                        if (Judge_ready_insert == "NG")
                        {
                            MessageBox.Show("DB 저장 실패");

                        }
                        dataGridView_LTlist.Rows.Add(new object[10] { data.SID, data.LOTID, data.UID, data.Quantity, data.Input_type, data.Tower_no, data.Production_date, data.Input_date, data.Manufacturer, data.Inch });
                        dataGridView_longterm.Rows.Remove(dataGridView_longterm.Rows[e.RowIndex]);
                    }

                    catch (Exception ex)
                    {
                        string strex = ex.ToString();
                        MessageBox.Show(strex);

                    }
                    //]210817_Sangik.choi_예외처리추가

                }

                else
                {
                    string str = string.Format("UID# {0} 를 배출 할 수 없습니다.\n", data.UID);
                    MessageBox.Show(str);
                }

            }

            else
            {
                string str = string.Format("등록된 사번이 아닙니다.\n");
                MessageBox.Show(str);
                return;
            }
   

            label_count.Text = dataGridView_LTlist.Rows.Count.ToString();

        }
        //]210809_Sangik.choi_장기보관관리기능추가(이종명수석님)



        //[210813_Sangik.choi_장기보관관리기능추가(이종명수석님)

        private void button_out_Click(object sender, EventArgs e)
        {
            int nGroup = comboBox_L_group.SelectedIndex + 1;
            string strEquipid = "TWR" + nGroup.ToString();
            string strPickingID = label_pickid_LT.Text;
            string badge = textBox_badge.Text;
            string user_check = AMM_Main.AMM.Check_LT_User(badge);
            int count = dataGridView_LTlist.Rows.Count;

            
            try
            {
                if (count > 0 && user_check == "OK" )
                {
                    Fnc_Picklist_Comfirm();  //raw data tower 위치랑 pid 타워 prefix 랑 같은지 확인
                    Fnc_Picklist_Send(AMM_Main.strDefault_linecode, strEquipid, strPickingID);  // ready info table 에서 pick list info 로 데이터 이동
                    Fnc_Process_LongtermInfo();
                }
                else if ( user_check == "NG" )
                {
                    string str = string.Format("등록된 사번이 아닙니다.\n");
                    MessageBox.Show(str);
                    return;
                }
                else
                {
                    string str = string.Format("청구할 수 없습니다. 청구리스트를 확인해주세요.\n");
                    MessageBox.Show(str);
                    return;
                }
            }
            catch (Exception ex)
            {
                string strex = ex.ToString();

                string strLog = string.Format("배출 실패 {0}", strex);
                Fnc_SaveLog(strLog, 1);

                MessageBox.Show(strex);


                return;
            }

        }

        //[210819_Sangik.choi_로그함수추가

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
        //]210819_Sangik.choi_로그함수추가


        //[210817_Sangik.choi_장기보관관리기능추가(이종명수석님)

        private void Fnc_Picklist_Send(string strlincode, string strequip, string strPickID)
        {
            if (strPickID == "")
            {
                MessageBox.Show("배출 ID 정보가 없습니다.");
                return;
            }
            ///Picklist 생성
            DataTable dt = AMM_Main.AMM.GetPickingReadyinfo_ID(strPickID);

            int nCount = dt.Rows.Count;

            if (nCount == 0)
            {
                MessageBox.Show("리스트 생성 목록이 없습니다.");
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

                strJudge = AMM_Main.AMM.SetPicking_Listinfo(strlincode, strequip, strPickID, data.UID, textBox_badge.Text, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, "AMM");

                if (strJudge == "NG")
                {
                    MessageBox.Show("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                    AMM_Main.strAMM_Connect = "NG";
                    return;
                }
                else if (strJudge == "DUPLICATE")
                {
                    string str = string.Format("자재 리스트가 중복 되었습니다.\n SID = '{0}', UID = '{1}'", data.SID, data.UID);
                    MessageBox.Show(str);
                    return;
                }
            }

            strJudge = AMM_Main.AMM.Delete_PickReadyinfo(strlincode, strPickID);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                MessageBox.Show(str);
                AMM_Main.strAMM_Connect = "NG";

                return;
            }
            ///Pick ID Info
            ///
            strJudge = AMM_Main.AMM.SetPickingID(strlincode, strequip, strPickID, label_count.Text, textBox_badge.Text);

            if (strJudge == "NG")
            {
                string str = string.Format("DB 연결을 할 수 없습니다.\n네트워크 연결 상태를 확인 하십시오.");
                MessageBox.Show(str);
                AMM_Main.strAMM_Connect = "NG";
                return;
            }

            string strLog = string.Format("PICK LIST 생성 완료 - 사번:{0}, PICKID:{1}, 수량:{2}", textBox_badge.Text, strPickID, nCount.ToString());


        }

        //]210817_Sangik.choi_장기보관관리기능추가(이종명수석님)




        //[210812_Sangik.choi_장기보관관리기능추가(이종명수석님)

        public void Fnc_Picklist_Comfirm()
        {
            string strPrefix = label_pickid_LT.Text.Substring(0, 2);

            int nCount = dataGridView_LTlist.Rows.Count;

            if (nCount < 1)
                return;

            for (int n = 0; n < nCount; n++)
            {
                string strPosition = dataGridView_LTlist.Rows[n].Cells[5].Value.ToString().Substring(2, 1);


                if (strPrefix == "PL")
                {
                    if (strPosition != "1")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
                else if (strPrefix == "PN")
                {
                    if (strPosition != "2")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
                else if (strPrefix == "PM")
                {
                    if (strPosition != "3")
                    {
                        Fnc_DeleteReady(n);
                    }
                }
            }
        }

        //]210812_Sangik.choi_장기보관관리기능추가(이종명수석님)



        //[210812_Sangik.choi_장기보관관리기능추가(이종명수석님)

        public void Fnc_DeleteReady(int nindex)
        {
            string strDeleteUID;
            strDeleteUID = dataGridView_LTlist.Rows[nindex].Cells[2].Value.ToString();
            string strPickingID = label_pickid_LT.Text;

            //Delete_PickReadyinfo_ReelID()-query = string.Format("DELETE FROM TB_PICK_READY_INFO WHERE LINE_CODE='{0}' and UID='{1}'", strLinecode, strReelid);
            string strJudge = AMM_Main.AMM.Delete_PickReadyinfo_ReelID(AMM_Main.strDefault_linecode, strDeleteUID);

            if (strJudge == "NG")
            {
                AMM_Main.strAMM_Connect = "NG";
                return;
            }

        }
        //]210812_Sangik.choi_장기보관관리기능추가(이종명수석님)



        //[210810_Sangik.choi_장기보관관리기능추가(이종명수석님)

        private void button_display_Click(object sender, EventArgs e)
        {

            string pid = label_pickid_LT.Text;
            if (pid != "")
            {
                string result = AMM_Main.AMM.Delete_PickReadyinfo(AMM_Main.strDefault_linecode, pid); //210817_Sangik.choi_ui 삭제 후 db 에서 삭제
                label_pickid_LT.Text = "";
/*                if (result == "NG")
                {

                    MessageBox.Show("Ready info DB 확인 필요.");
                    return;
                }*/
            }

            Fnc_Init_datagrid_longterm();

            int idx = comboBox_month.SelectedIndex + 1;
            int nGroup = comboBox_L_group.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();

            if (idx <= 12 && idx >= 1)
            {
                if(comboBox_L_group.SelectedIndex != comboBox_L_group.Items.Count -1)
                {
                    Fnc_Process_GetMaterialinfo_longterm(idx, strEquipid);
                }
                else
                {
                    Fnc_Process_GetMaterialinfo_longterm(idx);
                }
                
            }
            else
            {
                Fnc_Process_GetMaterialinfo_All(1);
            }



            Fnc_Get_PickID(strEquipid);




        }
        //]210810_Sangik.choi_장기보관관리기능추가(이종명수석님)


        //[210810_Sangik.choi_장기보관관리기능추가(이종명수석님)

        private void textBox_search_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                //Find
                Fnc_Find_longterm(textBox_search.Text);
            }
        }

        public void Fnc_Find_longterm(string strFind)
        {
            dataGridView_longterm.ClearSelection();

            int nCount = dataGridView_longterm.RowCount;
            int nCount2 = dataGridView_longterm.ColumnCount;



            bool bfind = false;

            for (int m = 0; m < nCount2; m++)
            {
                for (int n = 0; n < nCount; n++)
                {
                    string str = dataGridView_longterm.Rows[n].Cells[m].Value.ToString();

                    if (str == strFind)
                    {
                        dataGridView_longterm.Rows[n].Cells[m].Selected = true;
                        dataGridView_longterm.FirstDisplayedScrollingRowIndex = n;
                        bfind = true;
                        n = nCount; m = nCount2;
                    }
                }
            }

            if (bfind)
                return;

            for (int m = 0; m < nCount2; m++)
            {
                for (int n = 0; n < nCount; n++)
                {
                    string str = dataGridView_longterm.Rows[n].Cells[m].Value.ToString();

                    if (str.Contains(strFind))
                    {
                        dataGridView_longterm.Rows[n].Cells[m].Selected = true;
                        dataGridView_longterm.FirstDisplayedScrollingRowIndex = n;
                        bfind = true;
                        n = nCount; m = nCount2;
                    }
                }
            }
        }


        //[210810_Sangik.choi_장기보관관리기능추가(이종명수석님)


        private void button_delete_LT_Click(object sender, EventArgs e)
        {


            if ( dataGridView_LTlist.CurrentCell == null )
            {
                MessageBox.Show("삭제할 Reel 이 없습니다.");

            }
            else
            {
                int current_index = dataGridView_LTlist.CurrentCell.RowIndex;

                //[210812_Sangik.choi_장기보관관리기능추가(이종명수석님)]
                StorageData_Compare data = new StorageData_Compare();

                data.SID = dataGridView_LTlist.Rows[current_index].Cells["SID"].Value.ToString(); //SID
                data.LOTID = dataGridView_LTlist.Rows[current_index].Cells["Batch#"].Value.ToString(); //LOTOD
                data.UID = dataGridView_LTlist.Rows[current_index].Cells["UID"].Value.ToString(); //UID
                data.Quantity = dataGridView_LTlist.Rows[current_index].Cells["Qty"].Value.ToString(); //QTY
                data.Input_type = dataGridView_LTlist.Rows[current_index].Cells["투입형태"].Value.ToString(); //투입 형태
                data.Tower_no = dataGridView_LTlist.Rows[current_index].Cells["위치"].Value.ToString(); //위치
                data.Production_date = dataGridView_LTlist.Rows[current_index].Cells["제조일"].Value.ToString(); //제조일
                data.Input_date = dataGridView_LTlist.Rows[current_index].Cells["투입일"].Value.ToString(); //투입일
                data.Manufacturer = dataGridView_LTlist.Rows[current_index].Cells["제조사"].Value.ToString(); //제조사
                data.Inch = dataGridView_LTlist.Rows[current_index].Cells["인치"].Value.ToString(); //인치

                string result = AMM_Main.AMM.Delete_PickReadyinfo_ReelID(AMM_Main.strDefault_linecode, data.UID); //210817_Sangik.choi_ui 삭제 후 db 에서 삭제

                if (result == "OK")
                {
                    dataGridView_longterm.Rows.Add(new object[10] { data.SID, data.LOTID, data.UID, data.Quantity, data.Input_type, data.Tower_no, data.Production_date, data.Input_date, data.Manufacturer, data.Inch });
                    dataGridView_LTlist.Rows.Remove(dataGridView_LTlist.Rows[current_index]);
                    label_count.Text = dataGridView_LTlist.Rows.Count.ToString();
                }
                else
                {
                    MessageBox.Show("Pick list 삭제 실패. DB 확인 필요");
                    return;

                }

            }

        }   //]210812_Sangik.choi_장기보관관리기능추가(이종명수석님)]


        private void button_addlist_Click(object sender, EventArgs e)
        {

            if (dataGridView_longterm.Rows.Count < 1)
            {
                MessageBox.Show("자재 조회 후 원하는 항목 선택 후 담아주세요");
                return;
            }

            string list_idx = comboBox_all.SelectedItem.ToString();

            int longterm_row_count = dataGridView_longterm.Rows.Count;

            int current_longterm_index = dataGridView_longterm.CurrentCell.RowIndex;

            int nGroup = comboBox_L_group.SelectedIndex + 1;
            string strEquipid = "TWR" + nGroup.ToString();

            string strPickingid = label_pickid_LT.Text;

            string badge = textBox_badge.Text;
            string user_check = "";



            if ( badge != "")
            {
                user_check = AMM_Main.AMM.Check_LT_User(badge);
                if ( user_check != "OK" )
                {
                    string strLog = string.Format("등록된 사번이 아닙니다.");
                    Fnc_SaveLog(strLog, 1);
                    MessageBox.Show(strLog);

                    return;
                }

            }


            string current_longterm_sid = dataGridView_longterm.Rows[current_longterm_index].Cells[0].Value.ToString();
            string current_longterm_batch = dataGridView_longterm.Rows[current_longterm_index].Cells[1].Value.ToString();
            string Judge_ready_insert = "";

            List<int> idx_list = new List<int>();

            if (list_idx == "동일 Sid 선택" )
            {
                for(int i = 0; i< longterm_row_count; i++)
                {
                    string sid = dataGridView_longterm.Rows[i].Cells[0].Value.ToString();

                    if (sid == current_longterm_sid)
                    {

                        StorageData_Compare data = new StorageData_Compare();

                        data.SID = dataGridView_longterm.Rows[i].Cells["SID"].Value.ToString(); //SID
                        data.LOTID = dataGridView_longterm.Rows[i].Cells["Batch#"].Value.ToString(); //LOTOD
                        data.UID = dataGridView_longterm.Rows[i].Cells["UID"].Value.ToString(); //UID
                        data.Quantity = dataGridView_longterm.Rows[i].Cells["Qty"].Value.ToString(); //QTY
                        data.Input_type = dataGridView_longterm.Rows[i].Cells["투입형태"].Value.ToString(); //투입 형태
                        data.Tower_no = dataGridView_longterm.Rows[i].Cells["위치"].Value.ToString(); //위치
                        data.Production_date = dataGridView_longterm.Rows[i].Cells["제조일"].Value.ToString(); //제조일
                        data.Input_date = dataGridView_longterm.Rows[i].Cells["투입일"].Value.ToString(); //투입일
                        data.Manufacturer = dataGridView_longterm.Rows[i].Cells["제조사"].Value.ToString(); //제조사
                        data.Inch = dataGridView_longterm.Rows[i].Cells["인치"].Value.ToString(); //인치


                        if (dataGridView_LTlist.Rows.Count >= 20)
                        {
                            MessageBox.Show("최대 20개 까지 청구 가능합니다. 다시 조회 후 청구해주세요");
                            break;
                        }
                        else
                        {
                            string strJudge = AMM_Main.AMM.GetPickingReadyinfo(data.UID); 
                            string strJudge2 = AMM_Main.AMM.GetPickingListinfo(data.UID);
                            string strJudge3 = "OK";
                            string strTowerNo = data.Tower_no.Substring(4, 1);

                            if (bTowerUse[0] != true)
                            {
                                if (strTowerNo == "1")
                                    strJudge3 = "NG";
                            }
                            if (bTowerUse[1] != true)
                            {
                                if (strTowerNo == "2")
                                    strJudge3 = "NG";
                            }



                            if (strJudge == "OK" && strJudge2 == "OK" && strJudge3 == "OK")
                            {
                                try
                                {
                                    Judge_ready_insert = AMM_Main.AMM.SetPicking_Readyinfo(AMM_Main.strDefault_linecode, strEquipid, strPickingid, data.UID, badge, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, "AMM_SID");
                                    if (Judge_ready_insert == "OK")
                                    {
                                        dataGridView_LTlist.Rows.Add(new object[10] { data.SID, data.LOTID, data.UID, data.Quantity, data.Input_type, data.Tower_no, data.Production_date, data.Input_date, data.Manufacturer, data.Inch });
                                        idx_list.Add(i);
                                        label_count.Text = dataGridView_LTlist.Rows.Count.ToString();

                                    }
                                    else
                                    {
                                        MessageBox.Show("DB 저장 실패");
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string strex = ex.ToString();

                                    string strLog = string.Format("청구 리스트 추가 실패 {0}", strex);
                                    Fnc_SaveLog(strLog, 1);

                                    MessageBox.Show(strex);


                                    break;
                                }

                            }

                            else
                            {
                                string str = string.Format("UID# {0} 를 배출 할 수 없습니다.\n 배출 상태 및 타워 상태를 확인해주세요", data.UID);
                                MessageBox.Show(str);
                                break;
                            }


                        }

                    }

                }

                idx_list.Reverse();

                for (int j = 0; j < idx_list.Count; j++)
                {
                    int temp = idx_list[j];
                    dataGridView_longterm.Rows.Remove(dataGridView_longterm.Rows[temp]);

                }

            }
            else if (list_idx == "동일 Batch 선택")
            {
                for (int i = 0; i < longterm_row_count; i++)
                {
                    string batch = dataGridView_longterm.Rows[i].Cells[1].Value.ToString();

                    if (batch == current_longterm_batch)
                    {

                        StorageData_Compare data = new StorageData_Compare();

                        data.SID = dataGridView_longterm.Rows[i].Cells["SID"].Value.ToString(); //SID
                        data.LOTID = dataGridView_longterm.Rows[i].Cells["Batch#"].Value.ToString(); //LOTOD
                        data.UID = dataGridView_longterm.Rows[i].Cells["UID"].Value.ToString(); //UID
                        data.Quantity = dataGridView_longterm.Rows[i].Cells["Qty"].Value.ToString(); //QTY
                        data.Input_type = dataGridView_longterm.Rows[i].Cells["투입형태"].Value.ToString(); //투입 형태
                        data.Tower_no = dataGridView_longterm.Rows[i].Cells["위치"].Value.ToString(); //위치
                        data.Production_date = dataGridView_longterm.Rows[i].Cells["제조일"].Value.ToString(); //제조일
                        data.Input_date = dataGridView_longterm.Rows[i].Cells["투입일"].Value.ToString(); //투입일
                        data.Manufacturer = dataGridView_longterm.Rows[i].Cells["제조사"].Value.ToString(); //제조사
                        data.Inch = dataGridView_longterm.Rows[i].Cells["인치"].Value.ToString(); //인치


                        if (dataGridView_LTlist.Rows.Count >= 20)
                        {
                            MessageBox.Show("최대 20개 까지 청구 가능합니다. 다시 조회 후 청구해주세요");
                            break;
                        }
                        else
                        {

                            string strJudge = AMM_Main.AMM.GetPickingReadyinfo(data.UID);
                            string strJudge2 = AMM_Main.AMM.GetPickingListinfo(data.UID);
                            string strJudge3 = "OK";
                            string strTowerNo = data.Tower_no.Substring(4, 1);

                            if (bTowerUse[0] != true)
                            {
                                if (strTowerNo == "1")
                                    strJudge3 = "NG";
                            }
                            if (bTowerUse[1] != true)
                            {
                                if (strTowerNo == "2")
                                    strJudge3 = "NG";
                            }



                            if (strJudge == "OK" && strJudge2 == "OK" && strJudge3 == "OK")
                            {
                                try
                                {
                                    Judge_ready_insert = AMM_Main.AMM.SetPicking_Readyinfo(AMM_Main.strDefault_linecode, strEquipid, strPickingid, data.UID, badge, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, "AMM_SID");
                                    if (Judge_ready_insert == "OK")
                                    {
                                        dataGridView_LTlist.Rows.Add(new object[10] { data.SID, data.LOTID, data.UID, data.Quantity, data.Input_type, data.Tower_no, data.Production_date, data.Input_date, data.Manufacturer, data.Inch });
                                        idx_list.Add(i);
                                        label_count.Text = dataGridView_LTlist.Rows.Count.ToString();

                                    }
                                    else
                                    {
                                        MessageBox.Show("DB 저장 실패");
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string strex = ex.ToString();

                                    string strLog = string.Format("청구리스트 추가 실패 {0}", strex);
                                    Fnc_SaveLog(strLog, 1);

                                    MessageBox.Show(strex);
                                    break;
                                }

                            }

                            else
                            {
                                string str = string.Format("UID# {0} 를 배출 할 수 없습니다.\n 배출 상태 및 타워 상태를 확인해주세요", data.UID);
                                MessageBox.Show(str);
                                break;
                            }


                        }
                    }

                }

                idx_list.Reverse();

                for (int j = 0; j < idx_list.Count; j++)
                {
                    int temp = idx_list[j];
                    dataGridView_longterm.Rows.Remove(dataGridView_longterm.Rows[temp]);

                }
            }
            else if (list_idx == "전체 선택" )
            {
                for (int i = 0; i < longterm_row_count; i++)
                {
                    StorageData_Compare data = new StorageData_Compare();

                    data.SID = dataGridView_longterm.Rows[i].Cells["SID"].Value.ToString(); //SID
                    data.LOTID = dataGridView_longterm.Rows[i].Cells["Batch#"].Value.ToString(); //LOTOD
                    data.UID = dataGridView_longterm.Rows[i].Cells["UID"].Value.ToString(); //UID
                    data.Quantity = dataGridView_longterm.Rows[i].Cells["Qty"].Value.ToString(); //QTY
                    data.Input_type = dataGridView_longterm.Rows[i].Cells["투입형태"].Value.ToString(); //투입 형태
                    data.Tower_no = dataGridView_longterm.Rows[i].Cells["위치"].Value.ToString(); //위치
                    data.Production_date = dataGridView_longterm.Rows[i].Cells["제조일"].Value.ToString(); //제조일
                    data.Input_date = dataGridView_longterm.Rows[i].Cells["투입일"].Value.ToString(); //투입일
                    data.Manufacturer = dataGridView_longterm.Rows[i].Cells["제조사"].Value.ToString(); //제조사
                    data.Inch = dataGridView_longterm.Rows[i].Cells["인치"].Value.ToString(); //인치

                    if (dataGridView_LTlist.Rows.Count >= 20)
                    {
                        MessageBox.Show("최대 20개 까지 청구 가능합니다. 청구 완료 후 다시 조회해주세요");
                        break;
                    }
                    else
                    {

                        string strJudge = AMM_Main.AMM.GetPickingReadyinfo(data.UID);
                        string strJudge2 = AMM_Main.AMM.GetPickingListinfo(data.UID);
                        string strJudge3 = "OK";
                        string strTowerNo = data.Tower_no.Substring(4, 1);

                        if (bTowerUse[0] != true)
                        {
                            if (strTowerNo == "1")
                                strJudge3 = "NG";
                        }
                        if (bTowerUse[1] != true)
                        {
                            if (strTowerNo == "2")
                                strJudge3 = "NG";
                        }



                        if (strJudge == "OK" && strJudge2 == "OK" && strJudge3 == "OK")
                        {
                            try
                            {
                                Judge_ready_insert = AMM_Main.AMM.SetPicking_Readyinfo(AMM_Main.strDefault_linecode, strEquipid, strPickingid, data.UID, badge, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, "AMM_SID");
                                if (Judge_ready_insert == "OK")
                                {
                                    dataGridView_LTlist.Rows.Add(new object[10] { data.SID, data.LOTID, data.UID, data.Quantity, data.Input_type, data.Tower_no, data.Production_date, data.Input_date, data.Manufacturer, data.Inch });
                                    idx_list.Add(i);
                                    label_count.Text = dataGridView_LTlist.Rows.Count.ToString();

                                }
                                else
                                {
                                    MessageBox.Show("DB 저장 실패");
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                string strex = ex.ToString();

                                string strLog = string.Format("배출 실패 {0}", strex);
                                Fnc_SaveLog(strLog, 1);

                                MessageBox.Show(strex);
                                break;
                            }

                        }

                        else
                        {
                            string str = string.Format("UID# {0} 를 배출 할 수 없습니다.\n 배출 상태 및 타워 상태를 확인해주세요", data.UID);
                            MessageBox.Show(str);
                            break;
                        }


                    }

                }

                idx_list.Reverse();

                for (int j = 0; j < idx_list.Count; j++)
                {
                    int temp = idx_list[j];
                    dataGridView_longterm.Rows.Remove(dataGridView_longterm.Rows[temp]);

                }


            }
            else
            {
                MessageBox.Show("청구 리스트 등록 실패 재시도 해주세요");
                return;
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click_1(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox_sid_TextChanged(object sender, EventArgs e)
        {

        }



        //]210810_Sangik.choi_장기보관관리기능추가(이종명수석님)


        public List<ASM_StorageData> GetSIMMMaterialList(string TowerLocation, string tid)
        {
            List<ASM_StorageData> list = new List<ASM_StorageData>();

            try
            {
                DataTable dt;

                dt = MSSql.GetData(GetMaterialListSIMMQuery(TowerLocation, tid));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ASM_StorageData data = new ASM_StorageData();
                    data.UID = dt.Rows[i]["UID"].ToString();
                    data.SID = dt.Rows[i]["Component"].ToString();
                    data.Quantity = dt.Rows[i]["Quantity"].ToString();
                    data.LotID = dt.Rows[i]["SupplierLotID"].ToString();
                    data.Date_Input = dt.Rows[i]["BookedToLocation"].ToString();
                    data.Productiondate = dt.Rows[i]["ProductionDate"].ToString();
                    data.Manufacturer = dt.Rows[i]["SupplierName"].ToString();

                    if (data.UID != "")
                        list.Add(data);
                }

                list.Sort(CompareStorageData_ASM);
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
                //Log.WriteLog(Log4net.EnumLogLevel.ERROR, ex.ToString());
            }

            return list;
        }

        public string GetMaterialListSIMMQuery(string towerLocation, string tid)
        {
            try
            {
                string[] split = towerLocation.Split('.');
                string twrLocation = split[split.Length - 1];

                string query = string.Format(@"
                SELECT
                RANK() OVER (ORDER BY FLOT.ID) AS IDX,
                FLOT.ID AS UID, FLOT.MaterialID AS Component, FLOT.Quantity, FLOT.SupplierLotID, FLOT.BookedToLocation, FLOT.ProductionDate, FLOT.SupplierName
                FROM 
                (
	                SELECT ID FROM FactsLocation WITH (NOLOCK)
	                WHERE Name='{0}'
                ) FLOC
                JOIN 
                (
	                SELECT ID, MaterialID, Quantity, Customer1, LocationID, SupplierLotID, BookedToLocation,ProductionDate,SupplierName
	                FROM FactsLot WITH (NOLOCK)
	                WHERE Customer1='{1}'
                ) FLOT
                ON FLOC.ID = FLOT.LocationID",
                     twrLocation, tid);

                return query;
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
                //Log.WriteLog(Log4net.EnumLogLevel.ERROR, ex.ToString());
            }
            return "";
        }
        int CompareStorageData(StorageData_Compare obj1, StorageData_Compare obj2)
        {
            return obj1.UID.CompareTo(obj2.UID);
        }

        int CompareStorageData_ASM(ASM_StorageData obj1, ASM_StorageData obj2)
        {
            return obj1.SID.CompareTo(obj2.SID);
        }

        int CompareStorageData_AMM(AMM_StorageData obj1, AMM_StorageData obj2)
        {
            return obj1.SID.CompareTo(obj2.SID);
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

public class Inchdata
{
    public string Equipid = "";
    public string Inch_7_cnt = "";
    public string Inch_13_cnt = "";
    public string Inch_7_capa = "";
    public string Inch_13_capa = "";
    public string Inch_7_rate = "";
    public string Inch_13_rate = "";

}

public class StorageData2
{
    public string Creation_date = "";
    public string Equipid = "";
    public string pickid = "";
    public string UID = "";
    public string SID = "";
    public string Status = "";
    public string Tower_no = "";
    public string LOTID = "";
    public string Quantity = "";
    public string Manufacturer = "";
    public string Production_date = "";
    public string Inch = "";
    public string Input_type = "";
    public string Requestor = "";
}

public class ASM_StorageData
{
    public string SID = "";
    public string LotID = "";
    public string UID = "";
    public string Quantity = "";
    public string Date_Input = "";
    public string Productiondate = "";
    public string Manufacturer = "";
}

public class AMM_StorageData
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

public class StorageData_Compare
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
    public string Miss = "";
}