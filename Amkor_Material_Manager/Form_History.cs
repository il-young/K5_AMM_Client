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

namespace Amkor_Material_Manager
{
    public partial class Form_History : Form
    {
        int nnTabIndex = 0;

        //Excel
        public string strExcelfilePath = "";

        //timeset
        public static string strTimeset_date_st = "", strTimeset_date_ed = "";
        public static string strTimeset_hour_st = "", strTimeset_hour_ed = "";
        public static string strTimeset_Min_st = "", strTimeset_Min_ed = "";
       
        public static bool IsDateGathering = false;
        public string strView_Material = "";

        public bool bEventSearch = true;

        public Form_History()
        {
            InitializeComponent();

            Fnc_Init();
        }

        public void Fnc_Init()
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.StartupPath + @"\Excel");
            
            if (!di.Exists) { di.Create(); }
            strExcelfilePath = di.ToString();

            comboBox_group2.SelectedIndex = AMM_Main.nDefaultGroup - 1;

            tabControl_History.SelectedIndex = 0;
        }

        private void tabControl_ITS_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tabNo = tabControl_History.SelectedIndex;

            nnTabIndex = tabNo;

            Fnc_Init_datagrid(nnTabIndex);

            if (tabNo == 0)
            {
                textBox_mtlinput.Focus();
                Application.DoEvents();
            }
            else if (tabNo == 1)
            {
                comboBox_group2.SelectedIndex = AMM_Main.nDefaultGroup - 1;

                Application.DoEvents();

                Fnc_Update_timeset();

                string strEquipid = "TWR" + AMM_Main.nDefaultGroup.ToString();
                Fnc_Process_GetEvent(AMM_Main.strDefault_linecode, strEquipid);
            }
        }

        private void Fnc_Init_datagrid(int nNum)
        {
            if (nNum == 0)
            {
                dataGridView_info.Columns.Clear();
                dataGridView_info.Rows.Clear();
                dataGridView_info.Refresh();

                dataGridView_info.Columns.Add("NO", "NO");
                dataGridView_info.Columns.Add("작업일자", "작업일자");
                dataGridView_info.Columns.Add("작업내용", "작업내용");
                dataGridView_info.Columns.Add("UID", "UID");
                dataGridView_info.Columns.Add("SID", "SID");
                dataGridView_info.Columns.Add("LOTID", "LOTID");
                dataGridView_info.Columns.Add("QTY", "QTY");
                dataGridView_info.Columns.Add("INCH", "INCH");
                dataGridView_info.Columns.Add("위치", "위치");
                dataGridView_info.Columns.Add("PICKID", "PICKID");
                dataGridView_info.Columns.Add("요청사번", "요청자사번");
                dataGridView_info.Columns.Add("요청자", "요청자");
                dataGridView_info.Columns.Add("제조일", "제조일");
                dataGridView_info.Columns.Add("제조사", "제조사");
            }
            else if (nNum == 1)
            {
                dataGridView_event.Columns.Clear();
                dataGridView_event.Rows.Clear();
                dataGridView_event.Refresh();

                dataGridView_event.Columns.Add("NO", "NO");
                dataGridView_event.Columns.Add("발생일자", "발생일자");
                dataGridView_event.Columns.Add("CODE", "CODE");
                dataGridView_event.Columns.Add("TYPE", "TYPE");
                dataGridView_event.Columns.Add("에러명", "에러명");
                dataGridView_event.Columns.Add("DESCRIPT", "DESCRIPT");
                dataGridView_event.Columns.Add("조치내용", "조치내용");

                bEventSearch = true;
            }
            else if(nNum == 99)
            {
                dataGridView_info.Columns.Clear();
                dataGridView_info.Rows.Clear();
                dataGridView_info.Refresh();

                dataGridView_info.Columns.Add("조회 자료가 없습니다.", "조회 자료가 없습니다.");
            }
            else if (nNum == 100)
            {
                dataGridView_event.Columns.Clear();
                dataGridView_event.Rows.Clear();
                dataGridView_event.Refresh();

                dataGridView_event.Columns.Add("조회 자료가 없습니다.", "조회 자료가 없습니다.");

                bEventSearch = false;
            }
        }     

        private int Fnc_Process_GetMaterialHistory(string strUid)
        {
            IsDateGathering = true;

            var MtlList = AMM_Main.AMM.GetMaterial_Tracking(strUid);

            int nMtlCount = MtlList.Rows.Count;

            if (MtlList.Rows.Count == 0)
            {
                Fnc_Init_datagrid(99); ///조회 자료가 없습니다.
                return nMtlCount;
            }

            Fnc_Init_datagrid(0);

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
                data.Equipid = MtlList.Rows[i]["PICKID"].ToString(); data.Equipid = data.Equipid.Trim(); ///PickID
                data.Requestor = MtlList.Rows[i]["REQUESTOR"].ToString(); data.Requestor = data.Requestor.Trim();
                data.Input_type = MtlList.Rows[i]["INPUT_TYPE"].ToString(); data.Input_type = data.Input_type.Trim();
                string strStatus = MtlList.Rows[i]["STATUS"].ToString(); strStatus = strStatus.Trim();

                if(strStatus == "IN")
                {
                    data.Input_type = strStatus + "_" + data.Input_type;
                }
                else if(strStatus == "OUT")
                {
                    data.Input_type = "OUT";
                }

                list.Add(data);
            }

            list.Sort(CompareStorageData);

            int nIndex = 1;

            foreach (var item in list)
            {
                string strnQty = string.Format("{0:0,0}", Int32.Parse(item.Quantity));
                string strdate = item.Input_date;
                strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                    + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                string strName = "";
                if (item.Requestor != "")
                {
                    DataTable dtRequestor = AMM_Main.AMM.GetUserInfo(item.Requestor, 0);
                    if(dtRequestor.Rows.Count != 0)
                        strName = dtRequestor.Rows[0]["NAME"].ToString(); strName = strName.Trim();
                }
                dataGridView_info.Rows.Add(new object[14] { nIndex++, strdate, item.Input_type, item.UID, item.SID, item.LOTID, item.Quantity, item.Inch, item.Tower_no, item.Equipid, item.Requestor, strName, item.Production_date, item.Manufacturer });
            }

            IsDateGathering = false;

            return nMtlCount;
        }
        public void Fnc_Process_GetEvent(string strLincode, string strEqid)
        {
            IsDateGathering = true;

            string strToday = string.Format("{0}-{1:00}-{2:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            string strHead = string.Format("{0:00}:{1:00}:{2:00}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            label_updatedate2.Text = "최근 조회: " + strToday + " " + strHead;

            DataTable dtEvent = AMM_Main.AMM.GetEqEvent(strLincode, strEqid);

            int nCount = dtEvent.Rows.Count;

            if (nCount == 0)
            {
                Fnc_Init_datagrid(100); ///조회 자료가 없습니다.
                IsDateGathering = false;
                return;
            }            

            string stSaveTime_st = label_Value_time_st.Text.Replace(":", string.Empty);
            string stSaveTime_ed = label_Value_time_ed.Text.Replace(":", string.Empty);

            string stSaveDate_st = label_Value_date_st.Text.Replace("-", string.Empty);
            string stSaveDate_ed = label_Value_date_ed.Text.Replace("-", string.Empty);

            string strDate_st = string.Format("{0}{1:00}00", stSaveDate_st, stSaveTime_st);
            string strDate_ed = string.Format("{0}{1:00}00", stSaveDate_ed, stSaveTime_ed);

            Fnc_Init_datagrid(1);

            List<EventData> list = new List<EventData>();

            for (int i = 0; i < nCount; i++)
            {
                EventData data = new EventData();

                data.date = dtEvent.Rows[i]["DATETIME"].ToString(); data.date = data.date.Trim();                

                if (Double.Parse(strDate_st) < Double.Parse(data.date) && Double.Parse(strDate_ed) > Double.Parse(data.date))
                {
                    data.code = dtEvent.Rows[i]["ERROR_CODE"].ToString(); data.code = data.code.Trim();
                    data.type = dtEvent.Rows[i]["ERROR_TYPE"].ToString(); data.type = data.type.Trim();
                    data.name = dtEvent.Rows[i]["ERROR_NAME"].ToString(); data.name = data.name.Trim();
                    data.descript = dtEvent.Rows[i]["ERROR_DESCRIPT"].ToString(); data.descript = data.descript.Trim();
                    data.action = dtEvent.Rows[i]["ERROR_ACTION"].ToString(); data.action = data.action.Trim();

                    list.Add(data);
                }
            }

            list.Sort(CompareEventData); //date sort

            int nIndex = 1;

            if(list.Count == 0)
            {
                Fnc_Init_datagrid(100); ///조회 자료가 없습니다.
                IsDateGathering = false;
                return;
            }

            foreach (var item in list)
            {
                string strdate = item.date;
                strdate = strdate.Substring(0, 4) + "-" + strdate.Substring(4, 2) + "-" + strdate.Substring(6, 2) + " "
                    + strdate.Substring(8, 2) + ":" + strdate.Substring(10, 2) + ":" + strdate.Substring(12, 2);

                dataGridView_event.Rows.Add(new object[7] { nIndex++, strdate, item.code, item.type, item.name, item.descript, item.action });
            }

            //탑 랭커 계산
            dataGridView_data.Columns.Clear();
            dataGridView_data.Rows.Clear();
            dataGridView_data.Columns.Add("INX", "INX");
            dataGridView_data.Columns.Add("Code", "Code");
            dataGridView_data.Columns.Add("Qty", "Qty");
            dataGridView_data.Columns.Add("Name", "Name");
            dataGridView_data.Refresh();

            list.Sort(CompareEventData2); ///code sort
            int nRowcount = list.Count;

            string strSetCode = "", strSetName = "";
            int nCodecount = 0, nIdx = 0;

            for (int i = 0; i < nRowcount; i++)
            {
                string strCode = list[i].code;
                string strName = list[i].name;

                if (strSetCode != strCode)
                {
                    if (strSetCode != "")
                    {
                        dataGridView_data.Rows.Add(new object[4] { nIdx, strSetCode, nCodecount, strSetName });

                        strSetCode = strCode;
                        strSetName = strName;
                        nCodecount = 1;
                        nIdx++;
                    }
                    else
                    {
                        strSetCode = strCode;
                        strSetName = strName;
                        nCodecount = 1;
                        nIdx++;
                    }
                }
                else
                {
                    nCodecount++;
                }

                if (i == nRowcount - 1)
                {
                    dataGridView_data.Rows.Add(new object[4] { nIdx, strSetCode, nCodecount, strSetName });
                }
            }

            this.dataGridView_data.Sort(this.dataGridView_data.Columns["Qty"], ListSortDirection.Descending);
            //////////////////////////////

            label_rank1_code.Text = "-"; label_rank1_count.Text = "-"; label_rank1_name.Text = "-";
            label_rank2_code.Text = "-"; label_rank2_count.Text = "-"; label_rank2_name.Text = "-";
            label_rank3_code.Text = "-"; label_rank3_count.Text = "-"; label_rank3_name.Text = "-";
            label_rank4_code.Text = "-"; label_rank4_count.Text = "-"; label_rank4_name.Text = "-";
            label_rank5_code.Text = "-"; label_rank5_count.Text = "-"; label_rank5_name.Text = "-";

            int nEventcount = dataGridView_data.RowCount - 1;


            if (nEventcount < 1)
            {
                IsDateGathering = false;
                return;
            }

            label_rank1_code.Text = dataGridView_data.Rows[0].Cells[1].Value.ToString(); //Code
            label_rank1_count.Text = dataGridView_data.Rows[0].Cells[2].Value.ToString(); //Qty
            label_rank1_name.Text = dataGridView_data.Rows[0].Cells[3].Value.ToString(); //Name

            if (nEventcount < 2)
            {
                IsDateGathering = false;
                return;
            }
            label_rank2_code.Text = dataGridView_data.Rows[1].Cells[1].Value.ToString(); //Code
            label_rank2_count.Text = dataGridView_data.Rows[1].Cells[2].Value.ToString(); //Qty
            label_rank2_name.Text = dataGridView_data.Rows[1].Cells[3].Value.ToString(); //Name

            if (nEventcount < 3)
            {
                IsDateGathering = false;
                return;
            }
            label_rank3_code.Text = dataGridView_data.Rows[2].Cells[1].Value.ToString(); //Code
            label_rank3_count.Text = dataGridView_data.Rows[2].Cells[2].Value.ToString(); //Qty
            label_rank3_name.Text = dataGridView_data.Rows[2].Cells[3].Value.ToString(); //Name

            if (nEventcount < 4)
            {
                IsDateGathering = false;
                return;
            }
            label_rank4_code.Text = dataGridView_data.Rows[3].Cells[1].Value.ToString(); //Code
            label_rank4_count.Text = dataGridView_data.Rows[3].Cells[2].Value.ToString(); //Qty
            label_rank4_name.Text = dataGridView_data.Rows[3].Cells[3].Value.ToString(); //Name

            if (nEventcount < 5)
            {
                IsDateGathering = false;
                return;
            }

            label_rank5_code.Text = dataGridView_data.Rows[4].Cells[1].Value.ToString(); //Code
            label_rank5_count.Text = dataGridView_data.Rows[4].Cells[2].Value.ToString(); //Qty
            label_rank5_name.Text = dataGridView_data.Rows[4].Cells[3].Value.ToString(); //Name
            

            IsDateGathering = false;
        }

        private void button_excel_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult1 = MessageBox.Show("액셀 저장을 시작합니다. 계속하시겠습니까?", "Excel Save", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            if (strView_Material == "")
            {
                MessageBox.Show("저장할 데이터가 없습니다.");
                return;
            }

            IsDateGathering = true;

            string strPath2 = strExcelfilePath + "\\";
            string strDate2 = string.Format("{0}{1:00}{2:00}_{3}_{4}_{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);           
            strPath2 = strPath2 + "History_" + strView_Material + "_" + strDate2;

            string strPathName = "";

            strPathName = strPath2 + ".xlsx";

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
                    MessageBox.Show("같은 파일의 이름이 열려 있습니다.  해당 파일을 닫고 다시 실행 하십시오.");
                    return;
                }
                else
                {
                    File.Delete(strPathName);
                }
            }

            Fnc_ExcelCreate_MtlHistory(strPathName, strView_Material);

            IsDateGathering = false;

        }       

        public void Fnc_ExcelCreate_MtlHistory(string strPath, string strUid)
        {
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }

            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);            

            /////Input save////////
            int nCellcount = 0;

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Name = strUid;

            int nGcount = dataGridView_info.RowCount;

            if(nGcount == 0)
            {             
                MessageBox.Show("데이터가 없습니다. 조회 후 다시 시도 하십시오.");

                xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);

                return;
            }

            nCellcount = 0;

            xlWorkSheet.Cells[1, 2] = "No";
            xlWorkSheet.Cells[1, 3] = "작업일자";
            xlWorkSheet.Cells[1, 4] = "작업내용";
            xlWorkSheet.Cells[1, 5] = "UID";
            xlWorkSheet.Cells[1, 6] = "SID";
            xlWorkSheet.Cells[1, 7] = "LOTID";
            xlWorkSheet.Cells[1, 8] = "QTY";
            xlWorkSheet.Cells[1, 9] = "INCH";
            xlWorkSheet.Cells[1, 10] = "위치";
            xlWorkSheet.Cells[1, 11] = "PICKID";
            xlWorkSheet.Cells[1, 12] = "요청사번";
            xlWorkSheet.Cells[1, 13] = "요청자";
            xlWorkSheet.Cells[1, 14] = "제조일";
            xlWorkSheet.Cells[1, 15] = "제조사";
            
            for (int i = 0; i < nGcount; i++)
            {
                xlWorkSheet.Cells[2 + nCellcount, 2] = nCellcount + 1;
                xlWorkSheet.Cells[2 + nCellcount, 3] = dataGridView_info.Rows[i].Cells[1].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 4] = dataGridView_info.Rows[i].Cells[2].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 5] = dataGridView_info.Rows[i].Cells[3].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 6] = dataGridView_info.Rows[i].Cells[4].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 7] = dataGridView_info.Rows[i].Cells[5].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 8] = dataGridView_info.Rows[i].Cells[6].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 9] = dataGridView_info.Rows[i].Cells[7].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 10] = dataGridView_info.Rows[i].Cells[8].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 11] = dataGridView_info.Rows[i].Cells[9].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 12] = dataGridView_info.Rows[i].Cells[10].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 13] = dataGridView_info.Rows[i].Cells[11].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 14] = dataGridView_info.Rows[i].Cells[12].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 15] = dataGridView_info.Rows[i].Cells[13].Value.ToString();

                nCellcount++;
            }

            xlWorkSheet.Columns.AutoFit();           
            /////////////////////////////////////////
            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            System.Diagnostics.Process.Start(strPath);            
        }

        public void Fnc_ExcelCreate_EventHistory(string strPath, string strEqid)
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
            object misValue = System.Reflection.Missing.Value;

            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet2 = xlWorkBook.Worksheets.Add(misValue, misValue, 1, misValue);            

            /////Input save////////
            int nCellcount = 0;

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            xlWorkSheet.Name = "EVENT_" + strEqid;

            int nGcount = dataGridView_event.RowCount;
            nCellcount = 0;

            xlWorkSheet.Cells[1, 2] = "No";
            xlWorkSheet.Cells[1, 3] = "발생일자";
            xlWorkSheet.Cells[1, 4] = "CODE";
            xlWorkSheet.Cells[1, 5] = "TYPE";
            xlWorkSheet.Cells[1, 6] = "에러명";
            xlWorkSheet.Cells[1, 7] = "DESCRIPT";
            xlWorkSheet.Cells[1, 8] = "조치내용";
            xlWorkSheet.Columns.AutoFit();

            for (int i = 0; i < nGcount; i++)
            {
                xlWorkSheet.Cells[2 + nCellcount, 2] = nCellcount + 1;
                xlWorkSheet.Cells[2 + nCellcount, 3] = dataGridView_event.Rows[i].Cells[1].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 4] = dataGridView_event.Rows[i].Cells[2].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 5] = dataGridView_event.Rows[i].Cells[3].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 6] = dataGridView_event.Rows[i].Cells[4].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 7] = dataGridView_event.Rows[i].Cells[5].Value.ToString();
                xlWorkSheet.Cells[2 + nCellcount, 8] = dataGridView_event.Rows[i].Cells[6].Value.ToString();

                nCellcount++;
            }

            xlWorkSheet.Columns.AutoFit();
            /////////////////////////////////////////////

            xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            xlWorkSheet2.Name = "TOP5";

            nGcount = dataGridView_data.RowCount - 1;
            nCellcount = 0;

            xlWorkSheet2.Cells[1, 2] = "No";
            xlWorkSheet2.Cells[1, 3] = "CODE";
            xlWorkSheet2.Cells[1, 4] = "횟수";
            xlWorkSheet2.Cells[1, 5] = "알람명";
            xlWorkSheet2.Columns.AutoFit();

            for (int i = 0; i < nGcount; i++)
            {
                xlWorkSheet2.Cells[2 + nCellcount, 2] = nCellcount + 1;
                xlWorkSheet2.Cells[2 + nCellcount, 3] = "EC" + dataGridView_data.Rows[i].Cells[1].Value.ToString();
                xlWorkSheet2.Cells[2 + nCellcount, 4] = dataGridView_data.Rows[i].Cells[2].Value.ToString();
                xlWorkSheet2.Cells[2 + nCellcount, 5] = dataGridView_data.Rows[i].Cells[3].Value.ToString();

                nCellcount++;
            }

            xlWorkSheet2.Columns.AutoFit();            
            /////////////////////////////////////////
            xlWorkBook.SaveAs(strPath, Excel.XlFileFormat.xlOpenXMLWorkbook, misValue, misValue, false, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkSheet2);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            System.Diagnostics.Process.Start(strPath);
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

            int nGroup = comboBox_group2.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();
            

            if(strDate_st == "" || strDate_st == "")
            {
                IsDateGathering = false;
                return;
            }

            Fnc_Process_GetEvent(AMM_Main.strDefault_linecode, strEquipid);

            IsDateGathering = false;
        }

        private void button_excel2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult1 = MessageBox.Show("액셀 저장을 시작합니다. 계속하시겠습니까?", "Excel Save", MessageBoxButtons.YesNo);
            if (dialogResult1 == DialogResult.No)
            {
                return;
            }

            if (bEventSearch == false)
            {
                MessageBox.Show("저장할 데이터가 없습니다.");
                return;
            }

            IsDateGathering = true;

            int nGroup = comboBox_group2.SelectedIndex + 1;
            string strEquipid = "TWR" + nGroup.ToString();

            string strPath2 = strExcelfilePath + "\\";
            string strDate2 = string.Format("{0}{1:00}{2:00}_{3}_{4}_{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            strPath2 = strPath2 + "Event_" + AMM_Main.strDefault_linecode +"_"+ strEquipid + "_" + strDate2;

            string strPathName = "";

            strPathName = strPath2 + ".xlsx";

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
                    MessageBox.Show("같은 파일의 이름이 열려 있습니다.  해당 파일을 닫고 다시 실행 하십시오.");
                    return;
                }
                else
                {
                    File.Delete(strPathName);
                }
            }

            Fnc_ExcelCreate_EventHistory(strPathName, strEquipid);

            IsDateGathering = false;
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

        private void textBox_mtlinput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {                
                int nCount = Fnc_Process_GetMaterialHistory(textBox_mtlinput.Text);

                if(nCount < 1)
                {
                    strView_Material = "";
                    textBox_mtlinput.Focus();
                }
                else
                {
                    strView_Material = textBox_mtlinput.Text;
                    textBox_mtlinput.Text = "";
                    textBox_mtlinput.Focus();
                }                                
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

            int nGroup = comboBox_group2.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();            

            if (strDate_st == "" || strDate_ed == "") //210825_Sagnik.choi_strDate_ed 가 아니라 strDate_st 두개로 되어있어서 수정하였음
            {
                IsDateGathering = false;
                return;
            }

            Fnc_Process_GetEvent(AMM_Main.strDefault_linecode, strEquipid);

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
            
            int nGroup = comboBox_group2.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();            

            IsDateGathering = false;
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            IsDateGathering = true;

            string strDate_st = "", strDate_ed = "";
            strDate_st = strTimeset_date_st.Replace("-", string.Empty);
            strDate_st = strDate_st.Trim();
            strDate_st = strDate_st + strTimeset_hour_st + strTimeset_Min_st;

            strDate_ed = strTimeset_date_ed.Replace("-", string.Empty);
            strDate_ed = strDate_ed.Trim();
            strDate_ed = strDate_ed + strTimeset_hour_ed + strTimeset_Min_ed;

            int nGroup = comboBox_group2.SelectedIndex + 1;

            string strEquipid = "TWR" + nGroup.ToString();


            if (strDate_st == "" || strDate_ed == "") //210825_Sagnik.choi_strDate_ed 가 아니라 strDate_st 두개로 되어있어서 수정하였음
            {
                IsDateGathering = false;
                return;
            }

            Fnc_Process_GetEvent(AMM_Main.strDefault_linecode, strEquipid);

            IsDateGathering = false;
        }

        private void button_update_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button_update, "정보 업데이트");
        }

        int CompareEventData(EventData obj1, EventData obj2)
        {
            return obj1.date.CompareTo(obj2.date);
        }

        int CompareEventData2(EventData obj1, EventData obj2)
        {
            return obj1.code.CompareTo(obj2.code);
        }

        int CompareStorageData(StorageData obj1, StorageData obj2)
        {
            return obj1.Input_date.CompareTo(obj2.Input_date);
        }
    }
}