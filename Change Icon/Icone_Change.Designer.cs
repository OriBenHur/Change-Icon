﻿using System.ComponentModel;
using System.Windows.Forms;

namespace Change_Icon
{
    partial class IconChange
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IconChange));
            this.Folder_textBox = new System.Windows.Forms.TextBox();
            this.Folder_button = new System.Windows.Forms.Button();
            this.Icone_textBox = new System.Windows.Forms.TextBox();
            this.Folder_label = new System.Windows.Forms.Label();
            this.Icon_label = new System.Windows.Forms.Label();
            this.Icon_button = new System.Windows.Forms.Button();
            this.Set = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Folder_Error = new System.Windows.Forms.ErrorProvider(this.components);
            this.Reset_Folder = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Folder_Error)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Folder_textBox
            // 
            this.Folder_textBox.Location = new System.Drawing.Point(50, 34);
            this.Folder_textBox.Name = "Folder_textBox";
            this.Folder_textBox.Size = new System.Drawing.Size(193, 20);
            this.Folder_textBox.TabIndex = 0;
            // 
            // Folder_button
            // 
            this.Folder_button.Location = new System.Drawing.Point(246, 33);
            this.Folder_button.Name = "Folder_button";
            this.Folder_button.Size = new System.Drawing.Size(96, 23);
            this.Folder_button.TabIndex = 1;
            this.Folder_button.Text = "Browse Folder";
            this.Folder_button.UseVisualStyleBackColor = true;
            this.Folder_button.Click += new System.EventHandler(this.Folder_button_Click);
            // 
            // Icone_textBox
            // 
            this.Icone_textBox.Location = new System.Drawing.Point(50, 64);
            this.Icone_textBox.Name = "Icone_textBox";
            this.Icone_textBox.Size = new System.Drawing.Size(193, 20);
            this.Icone_textBox.TabIndex = 2;
            // 
            // Folder_label
            // 
            this.Folder_label.AutoSize = true;
            this.Folder_label.Location = new System.Drawing.Point(8, 37);
            this.Folder_label.Name = "Folder_label";
            this.Folder_label.Size = new System.Drawing.Size(39, 13);
            this.Folder_label.TabIndex = 4;
            this.Folder_label.Text = "Folder:";
            // 
            // Icon_label
            // 
            this.Icon_label.AutoSize = true;
            this.Icon_label.Location = new System.Drawing.Point(8, 67);
            this.Icon_label.Name = "Icon_label";
            this.Icon_label.Size = new System.Drawing.Size(31, 13);
            this.Icon_label.TabIndex = 5;
            this.Icon_label.Text = "Icon:";
            // 
            // Icon_button
            // 
            this.Icon_button.Location = new System.Drawing.Point(246, 63);
            this.Icon_button.Name = "Icon_button";
            this.Icon_button.Size = new System.Drawing.Size(96, 23);
            this.Icon_button.TabIndex = 6;
            this.Icon_button.Text = "Browse Icon";
            this.Icon_button.UseVisualStyleBackColor = true;
            this.Icon_button.Click += new System.EventHandler(this.Icon_button_Click);
            // 
            // Set
            // 
            this.Set.Location = new System.Drawing.Point(275, 208);
            this.Set.Name = "Set";
            this.Set.Size = new System.Drawing.Size(75, 23);
            this.Set.TabIndex = 7;
            this.Set.Text = "Set";
            this.Set.UseVisualStyleBackColor = true;
            this.Set.Click += new System.EventHandler(this.Set_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(50, 103);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // Folder_Error
            // 
            this.Folder_Error.ContainerControl = this;
            // 
            // Reset_Folder
            // 
            this.Reset_Folder.Location = new System.Drawing.Point(193, 208);
            this.Reset_Folder.Name = "Reset_Folder";
            this.Reset_Folder.Size = new System.Drawing.Size(75, 23);
            this.Reset_Folder.TabIndex = 9;
            this.Reset_Folder.Text = "Reset Folder";
            this.Reset_Folder.UseVisualStyleBackColor = true;
            this.Reset_Folder.Click += new System.EventHandler(this.Reset_Folder_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(368, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check For Updates";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // IconChange
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 239);
            this.Controls.Add(this.Reset_Folder);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Set);
            this.Controls.Add(this.Icon_button);
            this.Controls.Add(this.Icon_label);
            this.Controls.Add(this.Folder_label);
            this.Controls.Add(this.Icone_textBox);
            this.Controls.Add(this.Folder_button);
            this.Controls.Add(this.Folder_textBox);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "IconChange";
            this.Text = "Icon Change";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Icon_Change_FormClosed);
            this.Shown += new System.EventHandler(this.IconChange_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Folder_Error)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox Folder_textBox;
        private Button Folder_button;
        private TextBox Icone_textBox;
        private Label Folder_label;
        private Label Icon_label;
        private Button Icon_button;
        private Button Set;
        private PictureBox pictureBox1;
        private ErrorProvider Folder_Error;
        private Button Reset_Folder;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        
    }
}

