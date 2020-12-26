namespace smartlivingroom
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lamp_button = new System.Windows.Forms.Button();
            this.tv_button = new System.Windows.Forms.Button();
            this.record_button = new System.Windows.Forms.Button();
            this.stop_button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(599, 573);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lamp_button
            // 
            this.lamp_button.Location = new System.Drawing.Point(2, 582);
            this.lamp_button.Name = "lamp_button";
            this.lamp_button.Size = new System.Drawing.Size(75, 25);
            this.lamp_button.TabIndex = 1;
            this.lamp_button.Text = "lamp";
            this.lamp_button.UseVisualStyleBackColor = true;
            this.lamp_button.Click += new System.EventHandler(this.lamp_button_Click);
            // 
            // tv_button
            // 
            this.tv_button.Location = new System.Drawing.Point(83, 582);
            this.tv_button.Name = "tv_button";
            this.tv_button.Size = new System.Drawing.Size(75, 25);
            this.tv_button.TabIndex = 3;
            this.tv_button.Text = "tv";
            this.tv_button.UseVisualStyleBackColor = true;
            this.tv_button.Click += new System.EventHandler(this.tv_button_Click);
            // 
            // record_button
            // 
            this.record_button.Location = new System.Drawing.Point(444, 582);
            this.record_button.Name = "record_button";
            this.record_button.Size = new System.Drawing.Size(75, 25);
            this.record_button.TabIndex = 5;
            this.record_button.Text = "record";
            this.record_button.UseVisualStyleBackColor = true;
            this.record_button.Visible = false;
            this.record_button.Click += new System.EventHandler(this.record_button_Click);
            // 
            // stop_button
            // 
            this.stop_button.Location = new System.Drawing.Point(525, 582);
            this.stop_button.Name = "stop_button";
            this.stop_button.Size = new System.Drawing.Size(75, 25);
            this.stop_button.TabIndex = 6;
            this.stop_button.Text = "stop";
            this.stop_button.UseVisualStyleBackColor = true;
            this.stop_button.Visible = false;
            this.stop_button.Click += new System.EventHandler(this.stop_button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 609);
            this.Controls.Add(this.stop_button);
            this.Controls.Add(this.record_button);
            this.Controls.Add(this.tv_button);
            this.Controls.Add(this.lamp_button);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button lamp_button;
        private System.Windows.Forms.Button tv_button;
        private System.Windows.Forms.Button record_button;
        private System.Windows.Forms.Button stop_button;
    }
}

