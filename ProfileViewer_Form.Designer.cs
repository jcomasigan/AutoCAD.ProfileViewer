namespace ProfileViewer
{
    partial class ProfileViewer_Form
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.selectPline_btn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.majorCont_cmbx = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.minorCont_cmbx = new System.Windows.Forms.ComboBox();
            this.majorOnly_chbx = new System.Windows.Forms.CheckBox();
            this.plot_btn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.selectPline_btn);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(351, 76);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Profile Polyline";
            // 
            // selectPline_btn
            // 
            this.selectPline_btn.Location = new System.Drawing.Point(74, 27);
            this.selectPline_btn.Name = "selectPline_btn";
            this.selectPline_btn.Size = new System.Drawing.Size(261, 23);
            this.selectPline_btn.TabIndex = 1;
            this.selectPline_btn.Text = ".......";
            this.selectPline_btn.UseVisualStyleBackColor = true;
            this.selectPline_btn.Click += new System.EventHandler(this.selectPline_btn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Major:";
            // 
            // majorCont_cmbx
            // 
            this.majorCont_cmbx.FormattingEnabled = true;
            this.majorCont_cmbx.Location = new System.Drawing.Point(74, 19);
            this.majorCont_cmbx.Name = "majorCont_cmbx";
            this.majorCont_cmbx.Size = new System.Drawing.Size(261, 21);
            this.majorCont_cmbx.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Profile Line:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.majorOnly_chbx);
            this.groupBox2.Controls.Add(this.minorCont_cmbx);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.majorCont_cmbx);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(13, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(351, 129);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Contour Layers";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Minor:";
            // 
            // minorCont_cmbx
            // 
            this.minorCont_cmbx.FormattingEnabled = true;
            this.minorCont_cmbx.Location = new System.Drawing.Point(74, 77);
            this.minorCont_cmbx.Name = "minorCont_cmbx";
            this.minorCont_cmbx.Size = new System.Drawing.Size(261, 21);
            this.minorCont_cmbx.TabIndex = 2;
            // 
            // majorOnly_chbx
            // 
            this.majorOnly_chbx.AutoSize = true;
            this.majorOnly_chbx.Location = new System.Drawing.Point(74, 54);
            this.majorOnly_chbx.Name = "majorOnly_chbx";
            this.majorOnly_chbx.Size = new System.Drawing.Size(145, 17);
            this.majorOnly_chbx.TabIndex = 3;
            this.majorOnly_chbx.Text = "Major Contour Layer Only";
            this.majorOnly_chbx.UseVisualStyleBackColor = true;
            this.majorOnly_chbx.CheckedChanged += new System.EventHandler(this.majorOnly_chbx_CheckedChanged);
            // 
            // plot_btn
            // 
            this.plot_btn.Location = new System.Drawing.Point(61, 252);
            this.plot_btn.Name = "plot_btn";
            this.plot_btn.Size = new System.Drawing.Size(263, 23);
            this.plot_btn.TabIndex = 4;
            this.plot_btn.Text = "PLOT";
            this.plot_btn.UseVisualStyleBackColor = true;
            this.plot_btn.Click += new System.EventHandler(this.plot_btn_Click);
            // 
            // ProfileViewer_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 501);
            this.Controls.Add(this.plot_btn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ProfileViewer_Form";
            this.Text = "ProfileViewer_Form";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button selectPline_btn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox majorCont_cmbx;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox majorOnly_chbx;
        private System.Windows.Forms.ComboBox minorCont_cmbx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button plot_btn;
    }
}