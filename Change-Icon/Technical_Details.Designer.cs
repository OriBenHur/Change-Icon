namespace Change_Icon
{
    partial class Technical_Details
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Technical_Details));
            this.AppName_label = new System.Windows.Forms.Label();
            this.Version_label = new System.Windows.Forms.Label();
            this.AppVersion_label = new System.Windows.Forms.Label();
            this.App_label = new System.Windows.Forms.Label();
            this.build_label = new System.Windows.Forms.Label();
            this.builddate_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AppName_label
            // 
            this.AppName_label.AutoSize = true;
            this.AppName_label.Location = new System.Drawing.Point(14, 12);
            this.AppName_label.Name = "AppName_label";
            this.AppName_label.Size = new System.Drawing.Size(60, 13);
            this.AppName_label.TabIndex = 0;
            this.AppName_label.Text = "App Name:";
            // 
            // Version_label
            // 
            this.Version_label.AutoSize = true;
            this.Version_label.Location = new System.Drawing.Point(14, 37);
            this.Version_label.Name = "Version_label";
            this.Version_label.Size = new System.Drawing.Size(45, 13);
            this.Version_label.TabIndex = 1;
            this.Version_label.Text = "Version:";
            // 
            // AppVersion_label
            // 
            this.AppVersion_label.AutoSize = true;
            this.AppVersion_label.Location = new System.Drawing.Point(80, 37);
            this.AppVersion_label.Name = "AppVersion_label";
            this.AppVersion_label.Size = new System.Drawing.Size(10, 13);
            this.AppVersion_label.TabIndex = 3;
            this.AppVersion_label.Text = " ";
            // 
            // App_label
            // 
            this.App_label.AutoSize = true;
            this.App_label.Location = new System.Drawing.Point(80, 12);
            this.App_label.Name = "App_label";
            this.App_label.Size = new System.Drawing.Size(10, 13);
            this.App_label.TabIndex = 2;
            this.App_label.Text = " ";
            // 
            // build_label
            // 
            this.build_label.AutoSize = true;
            this.build_label.Location = new System.Drawing.Point(80, 66);
            this.build_label.Name = "build_label";
            this.build_label.Size = new System.Drawing.Size(10, 13);
            this.build_label.TabIndex = 5;
            this.build_label.Text = " ";
            this.build_label.Click += new System.EventHandler(this.build_label_Click);
            // 
            // builddate_label
            // 
            this.builddate_label.AutoSize = true;
            this.builddate_label.Location = new System.Drawing.Point(14, 66);
            this.builddate_label.Name = "builddate_label";
            this.builddate_label.Size = new System.Drawing.Size(59, 13);
            this.builddate_label.TabIndex = 4;
            this.builddate_label.Text = "Build Date:";
            // 
            // Technical_Details
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(218, 103);
            this.Controls.Add(this.build_label);
            this.Controls.Add(this.builddate_label);
            this.Controls.Add(this.AppVersion_label);
            this.Controls.Add(this.App_label);
            this.Controls.Add(this.Version_label);
            this.Controls.Add(this.AppName_label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Technical_Details";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AppName_label;
        private System.Windows.Forms.Label Version_label;
        private System.Windows.Forms.Label AppVersion_label;
        private System.Windows.Forms.Label App_label;
        private System.Windows.Forms.Label build_label;
        private System.Windows.Forms.Label builddate_label;
    }
}