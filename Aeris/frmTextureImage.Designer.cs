namespace Aeris
{
    partial class FrmTextureImage
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
            this.btnClose = new System.Windows.Forms.Button();
            this.panelTextureImage = new System.Windows.Forms.Panel();
            this.pbTextureImage = new System.Windows.Forms.PictureBox();
            this.folderExportUnsTex = new System.Windows.Forms.FolderBrowserDialog();
            this.panelTextureImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTextureImage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(12, 499);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(516, 35);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // panelTextureImage
            // 
            this.panelTextureImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelTextureImage.Controls.Add(this.pbTextureImage);
            this.panelTextureImage.Location = new System.Drawing.Point(12, 9);
            this.panelTextureImage.Name = "panelTextureImage";
            this.panelTextureImage.Size = new System.Drawing.Size(516, 484);
            this.panelTextureImage.TabIndex = 2;
            // 
            // pbTextureImage
            // 
            this.pbTextureImage.Location = new System.Drawing.Point(0, 0);
            this.pbTextureImage.Name = "pbTextureImage";
            this.pbTextureImage.Size = new System.Drawing.Size(512, 480);
            this.pbTextureImage.TabIndex = 0;
            this.pbTextureImage.TabStop = false;
            // 
            // frmTextureImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(540, 543);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panelTextureImage);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "frmTextureImage";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unswizzled Texture Preview";
            this.panelTextureImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbTextureImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Button btnClose;
        internal System.Windows.Forms.Panel panelTextureImage;
        internal System.Windows.Forms.PictureBox pbTextureImage;
        internal System.Windows.Forms.FolderBrowserDialog folderExportUnsTex;
    }
}