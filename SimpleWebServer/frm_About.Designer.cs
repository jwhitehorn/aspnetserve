namespace SimpleWebServer {
    partial class frm_About {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblJasonWhitehorn = new System.Windows.Forms.LinkLabel();
            this.lblaspNETserveHomepage = new System.Windows.Forms.LinkLabel();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "aspNETserve";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            this.lblVersion.Location = new System.Drawing.Point(13, 36);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(235, 23);
            this.lblVersion.TabIndex = 1;
            this.lblVersion.Text = "Version 0.8";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Copyright 2006-2008,";
            // 
            // lblJasonWhitehorn
            // 
            this.lblJasonWhitehorn.AutoSize = true;
            this.lblJasonWhitehorn.Location = new System.Drawing.Point(117, 90);
            this.lblJasonWhitehorn.Name = "lblJasonWhitehorn";
            this.lblJasonWhitehorn.Size = new System.Drawing.Size(87, 13);
            this.lblJasonWhitehorn.TabIndex = 3;
            this.lblJasonWhitehorn.TabStop = true;
            this.lblJasonWhitehorn.Text = "Jason Whitehorn";
            this.lblJasonWhitehorn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblJasonWhitehorn_LinkClicked);
            // 
            // lblaspNETserveHomepage
            // 
            this.lblaspNETserveHomepage.AutoSize = true;
            this.lblaspNETserveHomepage.Location = new System.Drawing.Point(42, 59);
            this.lblaspNETserveHomepage.Name = "lblaspNETserveHomepage";
            this.lblaspNETserveHomepage.Size = new System.Drawing.Size(206, 13);
            this.lblaspNETserveHomepage.TabIndex = 5;
            this.lblaspNETserveHomepage.TabStop = true;
            this.lblaspNETserveHomepage.Text = "http://code.google.com/p/aspNETserve/";
            this.lblaspNETserveHomepage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblaspNETserveHomepage_LinkClicked);
            // 
            // btnClose
            // 
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(90, 124);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frm_About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 159);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblaspNETserveHomepage);
            this.Controls.Add(this.lblJasonWhitehorn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "About aspNETserve";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel lblJasonWhitehorn;
        private System.Windows.Forms.LinkLabel lblaspNETserveHomepage;
        private System.Windows.Forms.Button btnClose;
    }
}