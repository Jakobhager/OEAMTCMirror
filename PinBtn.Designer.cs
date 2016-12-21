namespace OEAMTCMirror
{
    partial class PinBtn
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
            this.components = new System.ComponentModel.Container();
            this.btnStartMirror = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stopMirrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartMirror
            // 
            this.btnStartMirror.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnStartMirror.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnStartMirror.FlatAppearance.BorderColor = System.Drawing.Color.Maroon;
            this.btnStartMirror.FlatAppearance.BorderSize = 0;
            this.btnStartMirror.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Maroon;
            this.btnStartMirror.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gainsboro;
            this.btnStartMirror.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartMirror.Location = new System.Drawing.Point(10, 0);
            this.btnStartMirror.Name = "btnStartMirror";
            this.btnStartMirror.Size = new System.Drawing.Size(26, 20);
            this.btnStartMirror.TabIndex = 0;
            this.btnStartMirror.UseVisualStyleBackColor = true;
            this.btnStartMirror.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnStartMirror_MouseDown);
            this.btnStartMirror.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnStartMirror_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stopMirrorToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(183, 26);
            this.contextMenuStrip1.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextMenuStrip1_Closing);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // stopMirrorToolStripMenuItem
            // 
            this.stopMirrorToolStripMenuItem.Name = "stopMirrorToolStripMenuItem";
            this.stopMirrorToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.stopMirrorToolStripMenuItem.Text = "Spiegelung beenden";
            this.stopMirrorToolStripMenuItem.Click += new System.EventHandler(this.stopMirrorToolStripMenuItem_Click);
            // 
            // PinBtn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(36, 20);
            this.Controls.Add(this.btnStartMirror);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(1, 1);
            this.Name = "PinBtn";
            this.Text = "PinBtn";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Maroon;
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartMirror;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem stopMirrorToolStripMenuItem;
    }
}