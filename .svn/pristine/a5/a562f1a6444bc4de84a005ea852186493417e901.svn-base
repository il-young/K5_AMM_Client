using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace Amkor_Material_Manager
{
    public partial class Form_StripMark : Form
    {
        public string strSM_ID = "";
        int nApplycount = 1;
        public static int nNewcount = 1;

        public Form_StripMark()
        {
            InitializeComponent();
            textBox_sm.ImeMode = ImeMode.Alpha;
            textBox_linecode.Text = "AS41100"; 
        }

        public void Fnc_Show()
        {
            try
            {
                textBox_linecode.Text = "AS41100";  
                Form_Order.bSM_ListMade = false;

                if (AMM_Main.strNumberPad == "TRUE")
                    Fnc_ShowPad();

                timer1.Start();
                textBox_sm.Focus();

                ShowDialog();                
            }
            catch
            {

            }
        }
        public void Fnc_ShowPad()
        {
            Process[] amm = Process.GetProcessesByName("osk");

            if (amm.Length > 0)
            {
                amm[0].Kill();
            }

            Process p = new Process(); p.StartInfo.FileName = "C:\\Windows\\System32\\osk.exe";
            p.StartInfo.Arguments = null;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.Start();

            textBox_sm.Text = "";
            textBox_sm.Focus();
        }

        private void textBox_sm_Click(object sender, EventArgs e)
        {

        }

        private void textBox_sm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(textBox_sm.ImeMode != ImeMode.Alpha)
            {
                textBox_sm.ImeMode = ImeMode.Alpha;
            }

            if (e.KeyChar == (char)13)
            {
                Process[] amm = Process.GetProcessesByName("osk");

                if (amm.Length > 0)
                {
                    amm[0].Kill();
                }

                string strSM = textBox_sm.Text;
                strSM = strSM.ToUpper();
                textBox_sm.Text = strSM;

                string strLine = textBox_linecode.Text;
                strLine = strLine.ToUpper();

                Fnc_Gridinit();

                if (strSM.Length < 3)
                {
                    return;
                }

                var task = Task.Run(async () =>
                {
                    return await AMM_Main.AMM.Wbs_Get_Stripmarking_mtlinfo(strLine, strSM);
                });

                string strinfo = task.Result;
                string[] strComponentInfo = strinfo.Split('\n');

                int nCount = strComponentInfo.Length;

                int nUpdatecount = 0;
                for (int n = 0; n < nCount; n++)
                {
                    int nLength = strComponentInfo.Length;

                    if (nLength > 9)
                    {
                        string[] strGetSid = strComponentInfo[n].Split('\t');

                        if (strGetSid.Length > 1)
                        {
                            Fnc_data_add(strGetSid[0], strGetSid[1], nUpdatecount);
                            nUpdatecount++;
                        }
                    }
                }

                dataGridView_sid.Sort(dataGridView_sid.Columns["SID"], ListSortDirection.Ascending);
            }
        }
        public void Fnc_Gridinit()
        {
            dataGridView_sid.Columns.Clear();
            dataGridView_sid.Rows.Clear();
            dataGridView_sid.Refresh();

            Thread.Sleep(200);

            dataGridView_sid.DefaultCellStyle.Font = new Font("Calibri", 13);
            dataGridView_sid.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 13, FontStyle.Regular);
            dataGridView_sid.RowTemplate.Height = 37;

            dataGridView_sid.Columns.Add("SID", "SID");
            dataGridView_sid.Columns.Add("TYPE", "TYPE");
            dataGridView_sid.Columns.Add("수량", "수량");
            dataGridView_sid.Columns.Add("보유 그룹", "보유 그룹");

            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.HeaderText = "선택";
            checkBoxColumn.Width = 30;
            checkBoxColumn.Name = "checkBoxColumn";
            dataGridView_sid.Columns.Insert(0, checkBoxColumn);

            dataGridView_sid.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView_sid.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView_sid.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView_sid.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView_sid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            dataGridView_sid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView_sid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView_sid.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView_sid.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        public void Fnc_data_add(string strSID, string strType, int nRow)
        {
            //Get_Sid_Location() - query = string.Format(@"SELECT TOWER_NO FROM dbo.TB_MTL_INFO WHERE SID='{0}'", sid);

            string strLoc = AMM_Main.AMM.Get_Sid_Location(strSID);

            bool bCheck = true;
            if (strLoc == "NO_DATA")
            {
                strLoc = "CMS";
                bCheck = false;
            }

            dataGridView_sid.Rows.Add(new object[5] { bCheck, strSID, strType, nApplycount.ToString(), strLoc });
            if (bCheck)
            {
                dataGridView_sid.Rows[nRow].DefaultCellStyle.BackColor = Color.DarkBlue;
                dataGridView_sid.Rows[nRow].DefaultCellStyle.ForeColor = Color.White;
            }
            else
            {
                dataGridView_sid.Rows[nRow].DefaultCellStyle.BackColor = Color.White;
                dataGridView_sid.Rows[nRow].DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        private void dataGridView_sid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int n = dataGridView_sid.Rows.Count;

            if (n < 1)
                return;

            int nIndex = dataGridView_sid.CurrentCell.RowIndex;
            int nColumn = dataGridView_sid.CurrentCell.ColumnIndex;

            string Value = dataGridView_sid.Rows[nIndex].Cells[0].Value.ToString();
            string strQty = dataGridView_sid.Rows[nIndex].Cells[3].Value.ToString();
            string strLoc = dataGridView_sid.Rows[nIndex].Cells[4].Value.ToString();

            if (nColumn == 0)
            {
                if (Value == "True")
                {
                    dataGridView_sid.Rows[nIndex].Cells[0].Value = false;
                    dataGridView_sid.Rows[nIndex].DefaultCellStyle.BackColor = Color.White;
                    dataGridView_sid.Rows[nIndex].DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    if (strLoc == "CMS")
                    {
                        dataGridView_sid.Rows[nIndex].Cells[0].Value = false;
                        dataGridView_sid.Rows[nIndex].DefaultCellStyle.BackColor = Color.White;
                        dataGridView_sid.Rows[nIndex].DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        dataGridView_sid.Rows[nIndex].Cells[0].Value = true;
                        dataGridView_sid.Rows[nIndex].DefaultCellStyle.BackColor = Color.DarkBlue;
                        dataGridView_sid.Rows[nIndex].DefaultCellStyle.ForeColor = Color.White;
                    }
                }
            }
            else if (nColumn == 3)
            {
                if (Value == "True")
                {
                    Form_KeyPad Frm_KeyPad = new Form_KeyPad();
                    Frm_KeyPad.Fnc_Show(Int32.Parse(strQty));

                    dataGridView_sid.Rows[nIndex].Cells[3].Value = nNewcount.ToString();
                }
            }
        }

        private void button_towerout_Click(object sender, EventArgs e)
        {
            Form_Order.strPadSid = "";

            Form_NumberPad Frm_NumberPad = new Form_NumberPad();
            Frm_NumberPad.nType = 1;
            Frm_NumberPad.ShowDialog();

            if (Form_Order.strPadSid == "" || textBox_linecode.Text == "")
            {
                ///Message 처리
                return;
            }

            //작업자 정보 업데이트
            string strSid = Form_Order.strPadSid.Replace(';', ' ');
            strSid = strSid.Trim();
            string strName = AMM_Main.AMM.User_check(strSid);
            strName = strName.Trim();

            AMM_Main.strRequestor_id = strSid;
            AMM_Main.strRequestor_name = strName;
            ///

            Form_Processing Processing_Form = new Form_Processing();
            Processing_Form.ShowDialog();

            strSM_ID = "";
            strSM_ID = Fnc_Get_SMID(textBox_linecode.Text);

            if (strSM_ID == "")
            {
                ///Message 처리
                return;
            }

            SM_Oder_Ready(); //배출 자재 담기

            //리스트 생성
            SM_Send_Picklist();

            Form_Order.bSM_ListMade = true;

            Fnc_Exit();
        }
        public void SM_Oder_Ready()
        {
            string opid = Form_Order.strPadSid;

            if (opid == "")
            {
                return;
            }

            //Total Location 가져 오기
            string strTotalLocation = "";

            for (int n = 0; n < dataGridView_sid.Rows.Count; n++)
            {
                string strLoc = dataGridView_sid.Rows[n].Cells[4].Value.ToString();

                if (!strLoc.Contains("CMS"))
                {
                    if (strLoc.Contains(","))
                    {
                        string[] strSplit_location = strLoc.Split(',');
                        for (int i = 0; i < strSplit_location.Length; i++)
                        {
                            if (!strTotalLocation.Contains(strLoc))
                                strTotalLocation = strTotalLocation + strSplit_location[i] + ",";
                        }
                    }
                    else
                    {
                        if (!strTotalLocation.Contains(strLoc))
                            strTotalLocation = strTotalLocation + strLoc + ",";
                    }
                }
            }

            strTotalLocation = strTotalLocation.Substring(0, strTotalLocation.Length - 1);
            string[] strPickid = new string[10]; //10개 그룹 가정
            int[] nPickCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //10개 그룹 Pick count
            string[] strTotalLocation_split = strTotalLocation.Split(',');

            for (int k = 0; k < strTotalLocation_split.Length; k++)
            {
                if (strTotalLocation_split[k] != "" || strTotalLocation_split[k] != null)
                {
                    if (strTotalLocation_split[k].Length < 3)
                    {
                        int nLocation = Int32.Parse(strTotalLocation_split[k]);
                        strPickid[nLocation] = Fnc_Get_PickID(textBox_linecode.Text, strTotalLocation_split[k]);
                    }
                }
            }
            int[] nPickGroup = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //10개 그룹 Pick count
            int nPickGroupCount = 0;

            for (int n = 0; n < dataGridView_sid.Rows.Count; n++)
            {
                string strEnable = dataGridView_sid.Rows[n].Cells[0].Value.ToString();
                string strSid = dataGridView_sid.Rows[n].Cells[1].Value.ToString();
                string strQty = dataGridView_sid.Rows[n].Cells[3].Value.ToString();
                string strLoc = dataGridView_sid.Rows[n].Cells[4].Value.ToString();

                if (strEnable == "True")
                {
                    if (strLoc.Contains(","))
                    {
                        string[] strGroup = strLoc.Split(',');
                        int nGroupcount = strGroup.Length;

                        string newLocation = strLoc;
                        int orderqty = Int32.Parse(strQty);

                        ////2021.06.15
                        for (int nPick = 0; nPick < nPickGroupCount; nPick++)
                        {
                            for (int m = 0; m < nGroupcount; m++)
                            {
                                if (nPickGroup[nPick] == Int32.Parse(strGroup[m]))
                                {
                                    newLocation = nPickGroup[nPick].ToString();
                                    m = nGroupcount;
                                    nPick = nPickGroupCount;
                                }
                            }
                        }
                        /////////////////////////////////////////

                        for (int m = 0; m < nGroupcount; m++)
                        {
                            string priorityTower = Process_CheckPriorityTower(newLocation);

                            newLocation = "";
                            for (int i = 0; i < nGroupcount; i++)
                            {
                                if (strGroup[i] != priorityTower)
                                {
                                    newLocation = newLocation + strGroup[i] + ",";
                                }
                            }

                            if (newLocation != "")
                                newLocation = newLocation.Substring(0, newLocation.Length - 1);

                            int nGroup = Int32.Parse(priorityTower);

                            ////2021.06.15
                            bool bSave = true;
                            for (int nPick = 0; nPick < nPickGroupCount; nPick++)
                            {
                                if (nPickGroup[nPick] == nGroup)
                                {
                                    bSave = false;
                                    nPick = nPickGroupCount;
                                }
                            }

                            if (bSave)
                            {
                                nPickGroup[nPickGroupCount] = nGroup;
                                nPickGroupCount++;

                                if (nPickGroupCount > 9)
                                    nPickGroupCount = 9;
                            }
                            //////////////////////////////////////////

                            //2021.06.09 동일 Pick id 20개 이상 배출 금지 
                            if (nPickCount[nGroup] > 19)
                            {
                                nPickCount[nGroup] = 0;
                                strPickid[nGroup] = Fnc_Get_PickID(textBox_linecode.Text, nGroup.ToString());
                            }
                            /////

                            int result = Process_Ready_Sid(textBox_linecode.Text, strSid, priorityTower, strPickid[nGroup], opid, orderqty);

                            if (orderqty != result)
                            {
                                orderqty = orderqty - result;
                            }
                            else
                            {
                                m = nGroupcount;
                            }

                            nPickCount[nGroup] = nPickCount[nGroup] + orderqty;
                        }
                    }
                    else
                    {
                        string newLocation = strLoc;
                        int orderqty = Int32.Parse(strQty);

                        int nGroup = Int32.Parse(newLocation);

                        //2021.06.09 동일 Pick id 20개 이상 배출 금지 
                        if (nPickCount[nGroup] > 19)
                        {
                            nPickCount[nGroup] = 0;
                            strPickid[nGroup] = Fnc_Get_PickID(textBox_linecode.Text, nGroup.ToString());
                        }
                        /////
                        int result = Process_Ready_Sid(textBox_linecode.Text, strSid, newLocation, strPickid[nGroup], opid, orderqty);

                        nPickCount[nGroup] = nPickCount[nGroup] + orderqty;
                    }
                }
            }
            ///Order info 저장, Basket 삭제, Basket 저장 시 Order info 확인 중복 체크
        }
        private string Fnc_Get_PickID(string strLinecode, string strGroupinfo)
        {
            ///Pick id load
            string equipid = "TWR" + strGroupinfo;
            var tableList = AMM_Main.AMM.GetPickIDNo(strLinecode, equipid);

            string strPickid = "";

            if (tableList.Rows.Count == 0)
            {
                if (strGroupinfo == "1")
                    strPickid = "PL0000001";
                else if (strGroupinfo == "2")
                    strPickid = "PN0000001";
                else if (strGroupinfo == "3")
                    strPickid = "PM0000001";

            }
            else
            {
                string strprefix = tableList.Rows[0]["PICK_PREFIX"].ToString();
                strprefix = strprefix.Trim();
                string strNo = tableList.Rows[0]["PICK_NUM"].ToString();
                strNo = strNo.Trim();
                strPickid = strprefix + strNo;
            }

            string strGetNo = strPickid.Substring(strPickid.Length - 7);
            string strGetPrefix = strPickid.Substring(0, 2);

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
            AMM_Main.AMM.SetPickIDNo(strLinecode, equipid, text);

            return strPickid;
        }
        private string Fnc_Get_SMID(string strLinecode)
        {
            ///Pick id load
            var tableList = AMM_Main.AMM.GetPickIDNo(strLinecode, "AMM");

            string strSMid = "";

            if (tableList.Rows.Count == 0)
            {
                strSMid = "SM0000001";
            }
            else
            {
                string strprefix = tableList.Rows[0]["PICK_PREFIX"].ToString();
                strprefix = strprefix.Trim();
                string strNo = tableList.Rows[0]["PICK_NUM"].ToString();
                strNo = strNo.Trim();
                strSMid = strprefix + strNo;
            }

            string strGetNo = strSMid.Substring(strSMid.Length - 7);
            string strGetPrefix = strSMid.Substring(0, 2);

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
            AMM_Main.AMM.SetPickIDNo(strLinecode, "AMM", text);

            return strSMid;
        }
        public string Process_CheckPriorityTower(string targetTower)
        {            
            string[] strTargetTower = null;

            if (targetTower.Contains(','))
            {
                strTargetTower = targetTower.Split(',');
            }
            else
            {
                strTargetTower = new string[1];
                strTargetTower[0] = targetTower;
            }            

            return strTargetTower[0];
        }        

        public int Process_Ready_Sid(string strlinecode, string strSid, string strGroup, string strPickid, string strRequestor, int nQty)
        {
            DataTable dt = AMM_Main.AMM.Get_Sid_info(strlinecode, strGroup, strSid);
            dt.DefaultView.Sort = "UID";
            dt = dt.DefaultView.ToTable();

            string[] strGetRequestor = strRequestor.Split(';');

            int nCount = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strGetEquipid = dt.Rows[i]["EQUIP_ID"].ToString(); strGetEquipid = strGetEquipid.Trim();
                string strGetUID = dt.Rows[i]["UID"].ToString(); strGetUID = strGetUID.Trim();
                string strGetSID = dt.Rows[i]["SID"].ToString(); strGetSID = strGetSID.Trim();
                string strGetLotid = dt.Rows[i]["LOTID"].ToString(); strGetLotid = strGetLotid.Trim();
                string strGetInput_date = dt.Rows[i]["DATETIME"].ToString(); strGetInput_date = strGetInput_date.Trim();
                string strGetTower_no = dt.Rows[i]["TOWER_NO"].ToString(); strGetTower_no = strGetTower_no.Trim();
                string strGetQuantity = dt.Rows[i]["QTY"].ToString(); strGetQuantity = strGetQuantity.Trim();
                string strGetManufacturer = dt.Rows[i]["MANUFACTURER"].ToString(); strGetManufacturer = strGetManufacturer.Trim();
                string strGetProduction_date = dt.Rows[i]["PRODUCTION_DATE"].ToString(); strGetProduction_date = strGetProduction_date.Trim();
                string strGetInch = dt.Rows[i]["INCH_INFO"].ToString(); strGetInch = strGetInch.Trim();
                string strGetInput_type = dt.Rows[i]["INPUT_TYPE"].ToString(); strGetInput_type = strGetInput_type.Trim();

                string strTowerUse = AMM_Main.AMM.Get_Twr_Use(strGetTower_no);

                bool bBatchCheck = true;
                /*
                bool bBatchCheck = true;
                if (RPS_Client.nBatchSel == 1)
                {
                    if (strGetLotid == strBatch)
                        bBatchCheck = true;
                    else
                        bBatchCheck = false;
                }
                */

                if (strTowerUse == "USE" && bBatchCheck)
                {
                    string strReadycheck = AMM_Main.AMM.GetPickingReadyinfo(strGetUID);

                    if (strReadycheck == "OK")
                    {
                        string strSuccess = AMM_Main.AMM.SetPicking_Readyinfo(strlinecode, strGetEquipid, strPickid, strGetUID, strGetRequestor[0], strGetTower_no,
                        strGetSID, strGetLotid, strGetQuantity, strGetManufacturer, strGetProduction_date, strGetInch, strGetInput_type, strSM_ID);

                        if (strSuccess == "OK")
                        {
                            //Oreder Info 저장                           

                            nCount++;
                        }

                    }
                }

                if (nCount == nQty)
                    return nQty;
            }

            return nCount;
        }

        public void SM_Send_Picklist()
        {
            //1. Order info 테이블에 STANDBY Pickid           

            string strLinecode = textBox_linecode.Text;

            DataTable dtPickid = AMM_Main.AMM.GetPickingIDinfo_Stripmark(strSM_ID);
            dtPickid.DefaultView.Sort = "PICKID";
            dtPickid = dtPickid.DefaultView.ToTable();

            string strPickids = "", strCheckid = "";

            for (int n = 0; n < dtPickid.Rows.Count; n++)
            {
                string strGetPickid = dtPickid.Rows[n]["PICKID"].ToString(); strGetPickid = strGetPickid.Trim();
                if (strCheckid != strGetPickid)
                {
                    strPickids = strPickids + strGetPickid + ";";
                    strCheckid = strGetPickid;
                }
            }

            if (strPickids == "")
                return;

            strPickids = strPickids.Substring(0, strPickids.Length - 1);

            string[] strPickingidList;
            int nPickinglistcount = 0;

            if (strPickids.Contains(";"))
            {
                strPickingidList = strPickids.Split(';');
                nPickinglistcount = strPickingidList.Length;
            }
            else
            {
                strPickingidList = new string[1];
                strPickingidList[0] = strPickids;
                nPickinglistcount = 1;
            }

            //2. AMM Ready info 테이블 정보 가져옴 (Pickid)
            //3. Send Picklist, TB_PICK_ID_INFO, TB_PICK_LIST_INFO
            for (int n = 0; n < nPickinglistcount; n++)
            {
                StorageData data = new StorageData();

                DataTable dt = AMM_Main.AMM.GetPickingReadyinfo_ID(strPickingidList[n]);
                string strJudge = "";

                string strEuipid = "", strRequestor = "";
                for (int i = 0; i < dt.Rows.Count; i++)
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

                    strEuipid = string.Format("TWR{0}", data.Tower_no.Substring(2, 1));
                    strRequestor = data.Requestor;

                    strJudge = AMM_Main.AMM.SetPicking_Listinfo(strLinecode, strEuipid, strPickingidList[n], data.UID, data.Requestor, data.Tower_no, data.SID, data.LOTID, data.Quantity, data.Manufacturer, data.Production_date, data.Inch, data.Input_type, strSM_ID);

                    if (strJudge == "NG")
                    {        
                        return;
                    }
                    else if (strJudge == "DUPLICATE")
                    {
                        return;
                    }                    
                }

                strJudge = AMM_Main.AMM.Delete_PickReadyinfo(strLinecode, strPickingidList[n]);

                if (strJudge == "NG")
                {
                    return;
                }

                ///Pick ID Info
                strJudge = AMM_Main.AMM.SetPickingID(strLinecode, strEuipid, strPickingidList[n], dt.Rows.Count.ToString(), strRequestor);

                if (strJudge == "NG")
                {
                }
            }
        }

        public void Fnc_Exit()
        {
            timer1.Stop();
            Hide();

            Dispose();
        }

        private void button_minus_Click(object sender, EventArgs e)
        {
            nApplycount--;

            if (nApplycount < 1)
                nApplycount = 1;

            textBox_setcount.Text = nApplycount.ToString();
        }

        private void button_plus_Click(object sender, EventArgs e)
        {
            nApplycount++;

            if (nApplycount > 3)
                nApplycount = 3;

            textBox_setcount.Text = nApplycount.ToString();
        }

        private void button_apply_Click(object sender, EventArgs e)
        {
            int nCount = dataGridView_sid.RowCount;

            if (nCount < 1)
                return;

            if (textBox_setcount.Text != "")
            {
                int nCnt = Int32.Parse(textBox_setcount.Text);

                if (nCnt > 3)
                    nCnt = 3;

                nApplycount = nCnt;

                textBox_setcount.Text = "";
                textBox_setcount.Refresh();
                textBox_setcount.Text = nApplycount.ToString();
            }

            for (int n = 0; n < nCount; n++)
            {
                string Value = dataGridView_sid.Rows[n].Cells[0].Value.ToString();

                if (Value == "True")
                {
                    dataGridView_sid.Rows[n].Cells[3].Value = nApplycount;
                }
            }
        }

        private void checkBox_all_Click(object sender, EventArgs e)
        {
            int nCount = dataGridView_sid.Rows.Count;

            bool bCheck = checkBox_all.Checked;

            for (int n = 0; n < nCount; n++)
            {
                string Value = dataGridView_sid.Rows[n].Cells[0].Value.ToString();
                string strLoc = dataGridView_sid.Rows[n].Cells[4].Value.ToString();

                if (bCheck)
                {
                    if (strLoc != "CMS")
                    {
                        dataGridView_sid.Rows[n].Cells[0].Value = true;
                        dataGridView_sid.Rows[n].DefaultCellStyle.BackColor = Color.DarkBlue;
                        dataGridView_sid.Rows[n].DefaultCellStyle.ForeColor = Color.White;
                    }
                }
                else
                {
                    if (strLoc != "CMS")
                    {
                        dataGridView_sid.Rows[n].Cells[0].Value = false;
                        dataGridView_sid.Rows[n].DefaultCellStyle.BackColor = Color.White;
                        dataGridView_sid.Rows[n].DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Form_Order.bForceSMClose)
            {
                Form_Order.bForceSMClose = false;
                Fnc_Exit();
            }
        }

        private void textBox_setcount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }

            if(textBox_setcount.Text != "")
            {
                int nCnt = Int32.Parse(textBox_setcount.Text);

                if (nCnt > 3)
                    nCnt = 3;

                nApplycount = nCnt;
            }
        }
    }
}
