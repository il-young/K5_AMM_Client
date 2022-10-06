namespace Amkor_Material_Manager
{
    partial class AMM_Main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AMM_Main));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label_day = new System.Windows.Forms.Label();
            this.label_time = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button_order = new System.Windows.Forms.Button();
            this.button_monitor = new System.Windows.Forms.Button();
            this.button_inventory = new System.Windows.Forms.Button();
            this.button_history = new System.Windows.Forms.Button();
            this.button_request = new System.Windows.Forms.Button();
            this.button_setting = new System.Windows.Forms.Button();
            this.label_state = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1264, 54);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(136, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(258, 29);
            this.label1.TabIndex = 2;
            this.label1.Text = "Amkor Material Manager";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label_day
            // 
            this.label_day.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_day.AutoSize = true;
            this.label_day.BackColor = System.Drawing.Color.White;
            this.label_day.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_day.Location = new System.Drawing.Point(898, 18);
            this.label_day.Name = "label_day";
            this.label_day.Size = new System.Drawing.Size(104, 23);
            this.label_day.TabIndex = 3;
            this.label_day.Text = "0000/00/00";
            // 
            // label_time
            // 
            this.label_time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_time.AutoSize = true;
            this.label_time.BackColor = System.Drawing.Color.White;
            this.label_time.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_time.Location = new System.Drawing.Point(1006, 18);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(80, 23);
            this.label_time.TabIndex = 3;
            this.label_time.Text = "00:00:00";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(78)))), ((int)(((byte)(88)))));
            this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Info;
            this.label2.Location = new System.Drawing.Point(0, 733);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1264, 28);
            this.label2.TabIndex = 4;
            this.label2.Text = "Copyright 2020 - Amkor Technology Korea Automation Engineering ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(78)))), ((int)(((byte)(88)))));
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Dock = System.Windows.Forms.DockStyle.Left;
            this.label3.Location = new System.Drawing.Point(0, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(247, 679);
            this.label3.TabIndex = 6;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // button_order
            // 
            this.button_order.BackColor = System.Drawing.Color.White;
            this.button_order.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_order.Image = ((System.Drawing.Image)(resources.GetObject("button_order.Image")));
            this.button_order.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_order.Location = new System.Drawing.Point(14, 69);
            this.button_order.Name = "button_order";
            this.button_order.Size = new System.Drawing.Size(219, 75);
            this.button_order.TabIndex = 7;
            this.button_order.Text = "   릴 주문";
            this.button_order.UseVisualStyleBackColor = false;
            this.button_order.Click += new System.EventHandler(this.button_order_Click);
            // 
            // button_monitor
            // 
            this.button_monitor.BackColor = System.Drawing.Color.White;
            this.button_monitor.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_monitor.Image = ((System.Drawing.Image)(resources.GetObject("button_monitor.Image")));
            this.button_monitor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_monitor.Location = new System.Drawing.Point(14, 312);
            this.button_monitor.Name = "button_monitor";
            this.button_monitor.Size = new System.Drawing.Size(219, 75);
            this.button_monitor.TabIndex = 7;
            this.button_monitor.Text = "   설비 모니터링";
            this.button_monitor.UseVisualStyleBackColor = false;
            this.button_monitor.Visible = false;
            this.button_monitor.Click += new System.EventHandler(this.button_monitor_Click);
            // 
            // button_inventory
            // 
            this.button_inventory.BackColor = System.Drawing.Color.White;
            this.button_inventory.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_inventory.Image = ((System.Drawing.Image)(resources.GetObject("button_inventory.Image")));
            this.button_inventory.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_inventory.Location = new System.Drawing.Point(14, 150);
            this.button_inventory.Name = "button_inventory";
            this.button_inventory.Size = new System.Drawing.Size(219, 75);
            this.button_inventory.TabIndex = 7;
            this.button_inventory.Text = "   재고 조회";
            this.button_inventory.UseVisualStyleBackColor = false;
            this.button_inventory.Click += new System.EventHandler(this.button_inventory_Click);
            // 
            // button_history
            // 
            this.button_history.BackColor = System.Drawing.Color.White;
            this.button_history.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_history.Image = ((System.Drawing.Image)(resources.GetObject("button_history.Image")));
            this.button_history.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_history.Location = new System.Drawing.Point(14, 231);
            this.button_history.Name = "button_history";
            this.button_history.Size = new System.Drawing.Size(219, 75);
            this.button_history.TabIndex = 7;
            this.button_history.Text = "   이력 조회";
            this.button_history.UseVisualStyleBackColor = false;
            this.button_history.Click += new System.EventHandler(this.button_history_Click);
            // 
            // button_request
            // 
            this.button_request.BackColor = System.Drawing.Color.White;
            this.button_request.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_request.Image = ((System.Drawing.Image)(resources.GetObject("button_request.Image")));
            this.button_request.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_request.Location = new System.Drawing.Point(14, 552);
            this.button_request.Name = "button_request";
            this.button_request.Size = new System.Drawing.Size(219, 75);
            this.button_request.TabIndex = 7;
            this.button_request.Text = "   권한 요청";
            this.button_request.UseVisualStyleBackColor = false;
            this.button_request.Click += new System.EventHandler(this.button_login_Click);
            // 
            // button_setting
            // 
            this.button_setting.BackColor = System.Drawing.Color.White;
            this.button_setting.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_setting.Image = ((System.Drawing.Image)(resources.GetObject("button_setting.Image")));
            this.button_setting.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_setting.Location = new System.Drawing.Point(14, 633);
            this.button_setting.Name = "button_setting";
            this.button_setting.Size = new System.Drawing.Size(219, 75);
            this.button_setting.TabIndex = 7;
            this.button_setting.Text = "   설정";
            this.button_setting.UseVisualStyleBackColor = false;
            this.button_setting.Click += new System.EventHandler(this.button_setting_Click);
            // 
            // label_state
            // 
            this.label_state.BackColor = System.Drawing.Color.Red;
            this.label_state.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label_state.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_state.ForeColor = System.Drawing.SystemColors.Info;
            this.label_state.Location = new System.Drawing.Point(247, 54);
            this.label_state.Name = "label_state";
            this.label_state.Size = new System.Drawing.Size(1017, 5);
            this.label_state.TabIndex = 9;
            this.label_state.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AMM_Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this.label_state);
            this.Controls.Add(this.button_setting);
            this.Controls.Add(this.button_request);
            this.Controls.Add(this.button_history);
            this.Controls.Add(this.button_inventory);
            this.Controls.Add(this.button_monitor);
            this.Controls.Add(this.button_order);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label_time);
            this.Controls.Add(this.label_day);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MaximizeBox = false;
            this.Name = "AMM_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Amkor Material Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AMM_Main_FormClosing);
            this.Load += new System.EventHandler(this.AMM_Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label_day;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_order;
        private System.Windows.Forms.Button button_monitor;
        private System.Windows.Forms.Button button_inventory;
        private System.Windows.Forms.Button button_history;
        private System.Windows.Forms.Button button_request;
        private System.Windows.Forms.Button button_setting;
        private System.Windows.Forms.Label label_state;
    }
}

