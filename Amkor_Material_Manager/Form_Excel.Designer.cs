namespace Amkor_Material_Manager
{
    partial class Form_Excel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Excel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_G3 = new System.Windows.Forms.CheckBox();
            this.checkBox_G2 = new System.Windows.Forms.CheckBox();
            this.checkBox_G1 = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_Dsel5 = new System.Windows.Forms.CheckBox();
            this.checkBox_Dsel4 = new System.Windows.Forms.CheckBox();
            this.checkBox_Dsel3 = new System.Windows.Forms.CheckBox();
            this.checkBox_Dsel2 = new System.Windows.Forms.CheckBox();
            this.checkBox_Dsel1 = new System.Windows.Forms.CheckBox();
            this.button_Start = new System.Windows.Forms.Button();
            this.checkBox_G5 = new System.Windows.Forms.CheckBox();
            this.checkBox_G4 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_G5);
            this.groupBox1.Controls.Add(this.checkBox_G4);
            this.groupBox1.Controls.Add(this.checkBox_G3);
            this.groupBox1.Controls.Add(this.checkBox_G2);
            this.groupBox1.Controls.Add(this.checkBox_G1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(448, 124);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Option1 - 타워 선택";
            // 
            // checkBox_G3
            // 
            this.checkBox_G3.AutoSize = true;
            this.checkBox_G3.Location = new System.Drawing.Point(319, 44);
            this.checkBox_G3.Name = "checkBox_G3";
            this.checkBox_G3.Size = new System.Drawing.Size(79, 23);
            this.checkBox_G3.TabIndex = 0;
            this.checkBox_G3.Text = "Group 3";
            this.checkBox_G3.UseVisualStyleBackColor = true;
            // 
            // checkBox_G2
            // 
            this.checkBox_G2.AutoSize = true;
            this.checkBox_G2.Location = new System.Drawing.Point(175, 42);
            this.checkBox_G2.Name = "checkBox_G2";
            this.checkBox_G2.Size = new System.Drawing.Size(79, 23);
            this.checkBox_G2.TabIndex = 0;
            this.checkBox_G2.Text = "Group 2";
            this.checkBox_G2.UseVisualStyleBackColor = true;
            // 
            // checkBox_G1
            // 
            this.checkBox_G1.AutoSize = true;
            this.checkBox_G1.Location = new System.Drawing.Point(33, 42);
            this.checkBox_G1.Name = "checkBox_G1";
            this.checkBox_G1.Size = new System.Drawing.Size(79, 23);
            this.checkBox_G1.TabIndex = 0;
            this.checkBox_G1.Text = "Group 1";
            this.checkBox_G1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_Dsel5);
            this.groupBox2.Controls.Add(this.checkBox_Dsel4);
            this.groupBox2.Controls.Add(this.checkBox_Dsel3);
            this.groupBox2.Controls.Add(this.checkBox_Dsel2);
            this.groupBox2.Controls.Add(this.checkBox_Dsel1);
            this.groupBox2.Location = new System.Drawing.Point(12, 142);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(448, 273);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "데이터 선택";
            // 
            // checkBox_Dsel5
            // 
            this.checkBox_Dsel5.AutoSize = true;
            this.checkBox_Dsel5.Location = new System.Drawing.Point(33, 200);
            this.checkBox_Dsel5.Name = "checkBox_Dsel5";
            this.checkBox_Dsel5.Size = new System.Drawing.Size(80, 23);
            this.checkBox_Dsel5.TabIndex = 0;
            this.checkBox_Dsel5.Text = "배출 정보";
            this.checkBox_Dsel5.UseVisualStyleBackColor = true;
            this.checkBox_Dsel5.Visible = false;
            // 
            // checkBox_Dsel4
            // 
            this.checkBox_Dsel4.AutoSize = true;
            this.checkBox_Dsel4.Location = new System.Drawing.Point(33, 160);
            this.checkBox_Dsel4.Name = "checkBox_Dsel4";
            this.checkBox_Dsel4.Size = new System.Drawing.Size(136, 23);
            this.checkBox_Dsel4.TabIndex = 0;
            this.checkBox_Dsel4.Text = "입/출고 (상세 정보)";
            this.checkBox_Dsel4.UseVisualStyleBackColor = true;
            // 
            // checkBox_Dsel3
            // 
            this.checkBox_Dsel3.AutoSize = true;
            this.checkBox_Dsel3.Location = new System.Drawing.Point(33, 120);
            this.checkBox_Dsel3.Name = "checkBox_Dsel3";
            this.checkBox_Dsel3.Size = new System.Drawing.Size(133, 23);
            this.checkBox_Dsel3.TabIndex = 0;
            this.checkBox_Dsel3.Text = "입/출고 (SID 기준)";
            this.checkBox_Dsel3.UseVisualStyleBackColor = true;
            // 
            // checkBox_Dsel2
            // 
            this.checkBox_Dsel2.AutoSize = true;
            this.checkBox_Dsel2.Location = new System.Drawing.Point(33, 80);
            this.checkBox_Dsel2.Name = "checkBox_Dsel2";
            this.checkBox_Dsel2.Size = new System.Drawing.Size(118, 23);
            this.checkBox_Dsel2.TabIndex = 0;
            this.checkBox_Dsel2.Text = "재고 (상세 정보)";
            this.checkBox_Dsel2.UseVisualStyleBackColor = true;
            // 
            // checkBox_Dsel1
            // 
            this.checkBox_Dsel1.AutoSize = true;
            this.checkBox_Dsel1.Location = new System.Drawing.Point(33, 40);
            this.checkBox_Dsel1.Name = "checkBox_Dsel1";
            this.checkBox_Dsel1.Size = new System.Drawing.Size(115, 23);
            this.checkBox_Dsel1.TabIndex = 0;
            this.checkBox_Dsel1.Text = "재고 (SID 기준)";
            this.checkBox_Dsel1.UseVisualStyleBackColor = true;
            // 
            // button_Start
            // 
            this.button_Start.BackColor = System.Drawing.Color.White;
            this.button_Start.Image = ((System.Drawing.Image)(resources.GetObject("button_Start.Image")));
            this.button_Start.Location = new System.Drawing.Point(478, 23);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(112, 95);
            this.button_Start.TabIndex = 1;
            this.button_Start.UseVisualStyleBackColor = false;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            this.button_Start.MouseHover += new System.EventHandler(this.button_Start_MouseHover);
            // 
            // checkBox_G5
            // 
            this.checkBox_G5.AutoSize = true;
            this.checkBox_G5.Location = new System.Drawing.Point(177, 73);
            this.checkBox_G5.Name = "checkBox_G5";
            this.checkBox_G5.Size = new System.Drawing.Size(79, 23);
            this.checkBox_G5.TabIndex = 1;
            this.checkBox_G5.Text = "Group 5";
            this.checkBox_G5.UseVisualStyleBackColor = true;
            // 
            // checkBox_G4
            // 
            this.checkBox_G4.AutoSize = true;
            this.checkBox_G4.Location = new System.Drawing.Point(33, 71);
            this.checkBox_G4.Name = "checkBox_G4";
            this.checkBox_G4.Size = new System.Drawing.Size(79, 23);
            this.checkBox_G4.TabIndex = 2;
            this.checkBox_G4.Text = "Group 4";
            this.checkBox_G4.UseVisualStyleBackColor = true;
            // 
            // Form_Excel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(612, 432);
            this.Controls.Add(this.button_Start);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "Form_Excel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Excel 저장";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_G3;
        private System.Windows.Forms.CheckBox checkBox_G2;
        private System.Windows.Forms.CheckBox checkBox_G1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_Dsel5;
        private System.Windows.Forms.CheckBox checkBox_Dsel4;
        private System.Windows.Forms.CheckBox checkBox_Dsel3;
        private System.Windows.Forms.CheckBox checkBox_Dsel2;
        private System.Windows.Forms.CheckBox checkBox_Dsel1;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.CheckBox checkBox_G5;
        private System.Windows.Forms.CheckBox checkBox_G4;
    }
}