
namespace View
{
    partial class Form1
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
            this.ServerTextBox = new System.Windows.Forms.TextBox();
            this.ServerInputTextBox = new System.Windows.Forms.TextBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.NameInputTextBox = new System.Windows.Forms.TextBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.HelpMenuStrip = new System.Windows.Forms.MenuStrip();
            this.HelpButtonMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.controlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.HelpMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ServerTextBox
            // 
            this.ServerTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.ServerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ServerTextBox.Enabled = false;
            this.ServerTextBox.Location = new System.Drawing.Point(13, 17);
            this.ServerTextBox.Name = "ServerTextBox";
            this.ServerTextBox.ReadOnly = true;
            this.ServerTextBox.Size = new System.Drawing.Size(43, 13);
            this.ServerTextBox.TabIndex = 0;
            this.ServerTextBox.Text = "Server:";
            // 
            // ServerInputTextBox
            // 
            this.ServerInputTextBox.Location = new System.Drawing.Point(50, 12);
            this.ServerInputTextBox.Name = "ServerInputTextBox";
            this.ServerInputTextBox.Size = new System.Drawing.Size(100, 20);
            this.ServerInputTextBox.TabIndex = 1;
            this.ServerInputTextBox.Text = "localHost";
            // 
            // NameTextBox
            // 
            this.NameTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.NameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NameTextBox.Enabled = false;
            this.NameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NameTextBox.Location = new System.Drawing.Point(168, 17);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.ReadOnly = true;
            this.NameTextBox.Size = new System.Drawing.Size(39, 13);
            this.NameTextBox.TabIndex = 2;
            this.NameTextBox.Text = "Name:";
            // 
            // NameInputTextBox
            // 
            this.NameInputTextBox.Location = new System.Drawing.Point(202, 12);
            this.NameInputTextBox.Name = "NameInputTextBox";
            this.NameInputTextBox.Size = new System.Drawing.Size(100, 20);
            this.NameInputTextBox.TabIndex = 3;
            this.NameInputTextBox.Text = "player";
            // 
            // ConnectButton
            // 
            this.ConnectButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ConnectButton.Location = new System.Drawing.Point(327, 10);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(75, 23);
            this.ConnectButton.TabIndex = 4;
            this.ConnectButton.TabStop = false;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.ServerInputTextBox);
            this.panel1.Controls.Add(this.ConnectButton);
            this.panel1.Controls.Add(this.ServerTextBox);
            this.panel1.Controls.Add(this.NameInputTextBox);
            this.panel1.Controls.Add(this.NameTextBox);
            this.panel1.Controls.Add(this.HelpMenuStrip);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(889, 45);
            this.panel1.TabIndex = 5;
            // 
            // HelpMenuStrip
            // 
            this.HelpMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.HelpMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HelpButtonMenu});
            this.HelpMenuStrip.Location = new System.Drawing.Point(825, 12);
            this.HelpMenuStrip.Name = "HelpMenuStrip";
            this.HelpMenuStrip.Size = new System.Drawing.Size(172, 24);
            this.HelpMenuStrip.TabIndex = 5;
            // 
            // HelpButtonMenu
            // 
            this.HelpButtonMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.HelpButtonMenu.Name = "HelpButtonMenu";
            this.HelpButtonMenu.Size = new System.Drawing.Size(44, 20);
            this.HelpButtonMenu.Text = "Help";
            // 
            // controlsToolStripMenuItem
            // 
            this.controlsToolStripMenuItem.Name = "controlsToolStripMenuItem";
            this.controlsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.controlsToolStripMenuItem.Text = "Controls";
            this.controlsToolStripMenuItem.Click += new System.EventHandler(this.ControlsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowText;
            this.ClientSize = new System.Drawing.Size(888, 749);
            this.Controls.Add(this.panel1);
            this.MainMenuStrip = this.HelpMenuStrip;
            this.MaximumSize = new System.Drawing.Size(9999, 9999);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.HelpMenuStrip.ResumeLayout(false);
            this.HelpMenuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox ServerTextBox;
        private System.Windows.Forms.TextBox ServerInputTextBox;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.TextBox NameInputTextBox;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip HelpMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem HelpButtonMenu;
        private System.Windows.Forms.ToolStripMenuItem controlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

