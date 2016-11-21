namespace OEAMTCMirror
{
    partial class MirroredForm
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
            this.MirrorPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.MirrorPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MirrorPictureBox
            // 
            this.MirrorPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MirrorPictureBox.Location = new System.Drawing.Point(0, 0);
            this.MirrorPictureBox.Name = "MirrorPictureBox";
            this.MirrorPictureBox.Size = new System.Drawing.Size(155, 131);
            this.MirrorPictureBox.TabIndex = 0;
            this.MirrorPictureBox.TabStop = false;
            // 
            // MirroredForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.MirrorPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MirroredForm";
            this.Text = "MirroredScreen";
            ((System.ComponentModel.ISupportInitialize)(this.MirrorPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox MirrorPictureBox;
    }
}