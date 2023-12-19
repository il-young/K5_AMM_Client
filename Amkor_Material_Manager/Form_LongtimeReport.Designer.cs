namespace Amkor_Material_Manager
{
    partial class Form_LongtimeReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_LongtimeReport));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nud_Mon = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tb_hour = new System.Windows.Forms.MaskedTextBox();
            this.cb_interval2 = new System.Windows.Forms.ComboBox();
            this.cb_interval = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.clb_mail = new System.Windows.Forms.CheckedListBox();
            this.btn_mailAdd = new System.Windows.Forms.Button();
            this.btn_mailDel = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tb_header = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tb_tail = new System.Windows.Forms.TextBox();
            this.btn_mailEN = new System.Windows.Forms.Button();
            this.btn_Test = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_save = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tb_subject = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Mon)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nud_Mon);
            this.groupBox1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(198, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "검색 기간 설정";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(126, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 27);
            this.label2.TabIndex = 2;
            this.label2.Text = "이전";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(109, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "개월";
            // 
            // nud_Mon
            // 
            this.nud_Mon.Font = new System.Drawing.Font("굴림", 20F, System.Drawing.FontStyle.Bold);
            this.nud_Mon.Location = new System.Drawing.Point(6, 20);
            this.nud_Mon.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.nud_Mon.Name = "nud_Mon";
            this.nud_Mon.Size = new System.Drawing.Size(97, 38);
            this.nud_Mon.TabIndex = 0;
            this.nud_Mon.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_Mon.ValueChanged += new System.EventHandler(this.nud_Mon_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tb_hour);
            this.groupBox2.Controls.Add(this.cb_interval2);
            this.groupBox2.Controls.Add(this.cb_interval);
            this.groupBox2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.Location = new System.Drawing.Point(12, 89);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(198, 96);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Report 주기";
            // 
            // tb_hour
            // 
            this.tb_hour.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold);
            this.tb_hour.Location = new System.Drawing.Point(64, 61);
            this.tb_hour.Mask = "90시";
            this.tb_hour.Name = "tb_hour";
            this.tb_hour.Size = new System.Drawing.Size(113, 29);
            this.tb_hour.TabIndex = 12;
            this.tb_hour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tb_hour.ValidatingType = typeof(System.DateTime);
            // 
            // cb_interval2
            // 
            this.cb_interval2.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold);
            this.cb_interval2.FormattingEnabled = true;
            this.cb_interval2.Location = new System.Drawing.Point(105, 25);
            this.cb_interval2.Name = "cb_interval2";
            this.cb_interval2.Size = new System.Drawing.Size(72, 27);
            this.cb_interval2.TabIndex = 10;
            this.cb_interval2.SelectedIndexChanged += new System.EventHandler(this.cb_interval2_SelectedIndexChanged);
            // 
            // cb_interval
            // 
            this.cb_interval.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cb_interval.FormattingEnabled = true;
            this.cb_interval.Items.AddRange(new object[] {
            "주",
            "월"});
            this.cb_interval.Location = new System.Drawing.Point(6, 25);
            this.cb_interval.Name = "cb_interval";
            this.cb_interval.Size = new System.Drawing.Size(79, 27);
            this.cb_interval.TabIndex = 10;
            this.cb_interval.SelectedIndexChanged += new System.EventHandler(this.cb_interval_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.clb_mail);
            this.groupBox3.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox3.Location = new System.Drawing.Point(216, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(198, 173);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Mail List";
            // 
            // clb_mail
            // 
            this.clb_mail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clb_mail.FormattingEnabled = true;
            this.clb_mail.Location = new System.Drawing.Point(3, 22);
            this.clb_mail.Name = "clb_mail";
            this.clb_mail.Size = new System.Drawing.Size(192, 148);
            this.clb_mail.TabIndex = 0;
            this.clb_mail.SelectedIndexChanged += new System.EventHandler(this.clb_mail_SelectedIndexChanged);
            // 
            // btn_mailAdd
            // 
            this.btn_mailAdd.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold);
            this.btn_mailAdd.Location = new System.Drawing.Point(420, 32);
            this.btn_mailAdd.Name = "btn_mailAdd";
            this.btn_mailAdd.Size = new System.Drawing.Size(72, 24);
            this.btn_mailAdd.TabIndex = 3;
            this.btn_mailAdd.Text = "Insert";
            this.btn_mailAdd.UseVisualStyleBackColor = true;
            this.btn_mailAdd.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_mailDel
            // 
            this.btn_mailDel.Font = new System.Drawing.Font("굴림", 14F, System.Drawing.FontStyle.Bold);
            this.btn_mailDel.Location = new System.Drawing.Point(420, 161);
            this.btn_mailDel.Name = "btn_mailDel";
            this.btn_mailDel.Size = new System.Drawing.Size(72, 24);
            this.btn_mailDel.TabIndex = 4;
            this.btn_mailDel.Text = "Delete";
            this.btn_mailDel.UseVisualStyleBackColor = true;
            this.btn_mailDel.Click += new System.EventHandler(this.btn_mailDel_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tb_header);
            this.groupBox4.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox4.Location = new System.Drawing.Point(10, 245);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(482, 76);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Head";
            // 
            // tb_header
            // 
            this.tb_header.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_header.Location = new System.Drawing.Point(3, 22);
            this.tb_header.Multiline = true;
            this.tb_header.Name = "tb_header";
            this.tb_header.Size = new System.Drawing.Size(476, 51);
            this.tb_header.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tb_tail);
            this.groupBox5.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox5.Location = new System.Drawing.Point(10, 324);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(482, 77);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Tail";
            // 
            // tb_tail
            // 
            this.tb_tail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_tail.Location = new System.Drawing.Point(3, 22);
            this.tb_tail.Multiline = true;
            this.tb_tail.Name = "tb_tail";
            this.tb_tail.Size = new System.Drawing.Size(476, 52);
            this.tb_tail.TabIndex = 0;
            // 
            // btn_mailEN
            // 
            this.btn_mailEN.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_mailEN.Location = new System.Drawing.Point(42, 403);
            this.btn_mailEN.Name = "btn_mailEN";
            this.btn_mailEN.Size = new System.Drawing.Size(117, 35);
            this.btn_mailEN.TabIndex = 8;
            this.btn_mailEN.Text = "ENABLE";
            this.btn_mailEN.UseVisualStyleBackColor = true;
            this.btn_mailEN.Click += new System.EventHandler(this.btn_mailEN_Click);
            // 
            // btn_Test
            // 
            this.btn_Test.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_Test.Location = new System.Drawing.Point(165, 403);
            this.btn_Test.Name = "btn_Test";
            this.btn_Test.Size = new System.Drawing.Size(105, 35);
            this.btn_Test.TabIndex = 9;
            this.btn_Test.Text = "TEST";
            this.btn_Test.UseVisualStyleBackColor = true;
            this.btn_Test.Click += new System.EventHandler(this.btn_Test_Click);
            // 
            // btn_close
            // 
            this.btn_close.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_close.Location = new System.Drawing.Point(387, 403);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(105, 35);
            this.btn_close.TabIndex = 10;
            this.btn_close.Text = "Close";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // btn_save
            // 
            this.btn_save.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_save.Location = new System.Drawing.Point(276, 403);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(105, 35);
            this.btn_save.TabIndex = 11;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tb_subject);
            this.groupBox6.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox6.Location = new System.Drawing.Point(10, 191);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(482, 51);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Subject";
            // 
            // tb_subject
            // 
            this.tb_subject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_subject.Location = new System.Drawing.Point(3, 22);
            this.tb_subject.Multiline = true;
            this.tb_subject.Name = "tb_subject";
            this.tb_subject.Size = new System.Drawing.Size(476, 26);
            this.tb_subject.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 410);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 20);
            this.label3.TabIndex = 12;
            this.label3.Text = "NOW";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form_LongtimeReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 450);
            this.Controls.Add(this.btn_mailEN);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.btn_Test);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btn_mailDel);
            this.Controls.Add(this.btn_mailAdd);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_LongtimeReport";
            this.Text = "LongtimeReport Setting";
            this.Load += new System.EventHandler(this.Form_LongtimeReport_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Mon)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nud_Mon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckedListBox clb_mail;
        private System.Windows.Forms.Button btn_mailAdd;
        private System.Windows.Forms.Button btn_mailDel;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tb_header;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox tb_tail;
        private System.Windows.Forms.Button btn_mailEN;
        private System.Windows.Forms.Button btn_Test;
        private System.Windows.Forms.ComboBox cb_interval2;
        private System.Windows.Forms.ComboBox cb_interval;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.MaskedTextBox tb_hour;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox tb_subject;
        private System.Windows.Forms.Label label3;
    }
}