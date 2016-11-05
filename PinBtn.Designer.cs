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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PinBtn));
            this.btnStartMirror = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.stopMirrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartMirror
            // 
            this.btnStartMirror.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnStartMirror.BackgroundImage")));
            this.btnStartMirror.Location = new System.Drawing.Point(0, 0);
            this.btnStartMirror.Name = "btnStartMirror";
            this.btnStartMirror.Size = new System.Drawing.Size(33, 34);
            this.btnStartMirror.TabIndex = 0;
            this.btnStartMirror.UseVisualStyleBackColor = true;
            this.btnStartMirror.Click += new System.EventHandler(this.btnStartMirror_Click);
            this.btnStartMirror.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnStartMirror_MouseDown);
            this.btnStartMirror.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnStartMirror_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Interval = 5;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stopMirrorToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 48);
            this.contextMenuStrip1.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextMenuStrip1_Closing);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // stopMirrorToolStripMenuItem
            // 
            this.stopMirrorToolStripMenuItem.Name = "stopMirrorToolStripMenuItem";
            this.stopMirrorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopMirrorToolStripMenuItem.Text = "Stop Mirror";
            this.stopMirrorToolStripMenuItem.Click += new System.EventHandler(this.stopMirrorToolStripMenuItem_Click);
            // 
            // PinBtn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.ClientSize = new System.Drawing.Size(39, 37);
            this.Controls.Add(this.btnStartMirror);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
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