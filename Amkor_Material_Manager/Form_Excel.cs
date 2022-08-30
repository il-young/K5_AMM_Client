using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Amkor_Material_Manager
{
    public partial class Form_Excel : Form
    {
        string strSavefilePath = "";

        public Form_Excel()
        {
            InitializeComponent();
            Fnc_Init();
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            if (checkBox_G1.Checked == false && checkBox_G2.Checked == false && checkBox_G3.Checked == false
                && checkBox_G4.Checked == false && checkBox_G5.Checked == false)//220829_ilyoung_타워그룹추가 
            {
                MessageBox.Show("그룹은 최소 하나 이상 선택 되어야 합니다.");
                checkBox_G1.Checked = true;
                return;
            }

            Fnc_Save_FullStateInfo();
        }

        public void Fnc_Init()
        {
            checkBox_Dsel1.ForeColor = Color.DarkBlue;
            checkBox_Dsel2.ForeColor = Color.DarkBlue;
            checkBox_Dsel3.ForeColor = Color.DarkBlue;
            checkBox_Dsel4.ForeColor = Color.DarkBlue;
            checkBox_Dsel5.ForeColor = Color.DarkBlue;

            checkBox_G1.ForeColor = Color.DarkBlue;
            checkBox_G2.ForeColor = Color.DarkBlue;
            checkBox_G3.ForeColor = Color.DarkBlue;



            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Application.StartupPath + @"\Config");
            if (!di.Exists) { di.Create(); }
            strSavefilePath = di.ToString();

            Fnc_Load_FullStateInfo();

            if(Form_ITS.nExcelIndex == 0)
            {
                checkBox_Dsel1.Visible = true;
                checkBox_Dsel2.Visible = true;
                checkBox_Dsel3.Visible = false;
                checkBox_Dsel4.Visible = false;
                //checkBox_Dsel5.Visible = false;
            }
            else if(Form_ITS.nExcelIndex == 1)
            {
                checkBox_Dsel1.Visible = false;
                checkBox_Dsel2.Visible = false;
                checkBox_Dsel3.Visible = true;
                checkBox_Dsel4.Visible = true;
                //checkBox_Dsel5.Visible = true;
            }
        }

        public void Fnc_Save_FullStateInfo()
        {
            string strPath = strSavefilePath + "\\Excel_config.ini";

            string text = checkBox_G1.Checked + ";" + checkBox_G2.Checked + ";" + checkBox_G3.Checked + ";" + 
                 checkBox_Dsel1.Checked + ";" + checkBox_Dsel2.Checked + ";" + checkBox_Dsel3.Checked + ";" + checkBox_Dsel4.Checked + ";" + checkBox_Dsel5.Checked;
            System.IO.File.WriteAllText(strPath, text);

            Form_ITS.bExcelUse[0] = checkBox_Dsel1.Checked;
            Form_ITS.bExcelUse[1] = checkBox_Dsel2.Checked;
            Form_ITS.bExcelUse[2] = checkBox_Dsel3.Checked;
            Form_ITS.bExcelUse[3] = checkBox_Dsel4.Checked;
            Form_ITS.bExcelUse[4] = checkBox_Dsel5.Checked;

            Form_ITS.bGroupUse[0] = checkBox_G1.Checked;
            Form_ITS.bGroupUse[1] = checkBox_G2.Checked;
            Form_ITS.bGroupUse[2] = checkBox_G3.Checked;
            Form_ITS.bGroupUse[3] = checkBox_G4.Checked;    //220829_ilyoung_타워그룹추가
            Form_ITS.bGroupUse[4] = checkBox_G5.Checked;    //220829_ilyoung_타워그룹추가


            Form_ITS.bExcel_Start = true;

            Excel_Exit();
        }

        private void Fnc_Load_FullStateInfo()
        {
            string strPath = strSavefilePath + "\\Excel_config.ini";

            if (!File.Exists(strPath))
            {
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
                            checkBox_G1.Checked = true;
                        else
                            checkBox_G1.Checked = false;

                        if (strSplit[1] == "True")
                            checkBox_G2.Checked = true;
                        else
                            checkBox_G2.Checked = false;

                        if (strSplit[2] == "True")
                            checkBox_G3.Checked = true;
                        else
                            checkBox_G3.Checked = false;

                        //220829_ilyoung_타워그룹추가
                        if (strSplit[3] == "True")
                            checkBox_G4.Checked = true;
                        else
                            checkBox_G4.Checked = false;

                        if (strSplit[4] == "True")
                            checkBox_G5.Checked = true;
                        else
                            checkBox_G5.Checked = false;
                        //220829_ilyoung_타워그룹추가


                        if (strSplit[7] == "True")
                            checkBox_Dsel1.Checked = true;
                        else
                            checkBox_Dsel1.Checked = false;

                        if (strSplit[8] == "True")
                            checkBox_Dsel2.Checked = true;
                        else
                            checkBox_Dsel2.Checked = false;

                        if (strSplit[9] == "True")
                            checkBox_Dsel3.Checked = true;
                        else
                            checkBox_Dsel3.Checked = false;

                        if (strSplit[10] == "True")
                            checkBox_Dsel4.Checked = true;
                        else
                            checkBox_Dsel4.Checked = false;

                        if (strSplit[11] == "True")
                            checkBox_Dsel5.Checked = true;
                        else
                            checkBox_Dsel5.Checked = false;
                    }
                }
                catch
                { }
            }
        }

        public void Excel_Exit()
        {
            this.Dispose();
        }

        private void button_Start_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.SetToolTip(this.button_Start, "파일 저장");
        }
    }
}
