namespace Aeris
{
    partial class FrmSwizzleExternalHashImage
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
            this.txtTileID = new System.Windows.Forms.TextBox();
            this.lbTileID = new System.Windows.Forms.Label();
            this.txtPalette = new System.Windows.Forms.TextBox();
            this.btnSaveSwizzledTexture = new System.Windows.Forms.Button();
            this.btnSwizzleHashFolder = new System.Windows.Forms.Button();
            this.txtHash = new System.Windows.Forms.TextBox();
            this.lbHash = new System.Windows.Forms.Label();
            this.txtState = new System.Windows.Forms.TextBox();
            this.txtParam = new System.Windows.Forms.TextBox();
            this.txtTextureID = new System.Windows.Forms.TextBox();
            this.lbPalette = new System.Windows.Forms.Label();
            this.lbState = new System.Windows.Forms.Label();
            this.lbParam = new System.Windows.Forms.Label();
            this.lbTextureID = new System.Windows.Forms.Label();
            this.btnSwizzleHashImg = new System.Windows.Forms.Button();
            this.txtTextureHeight = new System.Windows.Forms.TextBox();
            this.txtTextureWidth = new System.Windows.Forms.TextBox();
            this.lbTextureHeight = new System.Windows.Forms.Label();
            this.lbTextureWidth = new System.Windows.Forms.Label();
            this.txtScaleFactor = new System.Windows.Forms.TextBox();
            this.lbScaleFactor = new System.Windows.Forms.Label();
            this.panelSwizzledPreview = new System.Windows.Forms.Panel();
            this.pbSwizzledTexturePreview = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnLoadUnsHashImg = new System.Windows.Forms.Button();
            this.ofdTexture = new System.Windows.Forms.OpenFileDialog();
            this.sfdTexture = new System.Windows.Forms.SaveFileDialog();
            this.panelSwizzledPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSwizzledTexturePreview)).BeginInit();
            this.SuspendLayout();
            // 
            // txtTileID
            // 
            this.txtTileID.Location = new System.Drawing.Point(673, 194);
            this.txtTileID.Name = "txtTileID";
            this.txtTileID.ReadOnly = true;
            this.txtTileID.Size = new System.Drawing.Size(66, 22);
            this.txtTileID.TabIndex = 48;
            this.txtTileID.TabStop = false;
            this.txtTileID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbTileID
            // 
            this.lbTileID.AutoSize = true;
            this.lbTileID.Location = new System.Drawing.Point(611, 197);
            this.lbTileID.Name = "lbTileID";
            this.lbTileID.Size = new System.Drawing.Size(48, 17);
            this.lbTileID.TabIndex = 47;
            this.lbTileID.Text = "TileID:";
            // 
            // txtPalette
            // 
            this.txtPalette.Location = new System.Drawing.Point(673, 122);
            this.txtPalette.Name = "txtPalette";
            this.txtPalette.ReadOnly = true;
            this.txtPalette.Size = new System.Drawing.Size(66, 22);
            this.txtPalette.TabIndex = 46;
            this.txtPalette.TabStop = false;
            this.txtPalette.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnSaveSwizzledTexture
            // 
            this.btnSaveSwizzledTexture.Location = new System.Drawing.Point(533, 444);
            this.btnSaveSwizzledTexture.Name = "btnSaveSwizzledTexture";
            this.btnSaveSwizzledTexture.Size = new System.Drawing.Size(224, 40);
            this.btnSaveSwizzledTexture.TabIndex = 30;
            this.btnSaveSwizzledTexture.Text = "Save Swizzled Texture";
            this.btnSaveSwizzledTexture.UseVisualStyleBackColor = true;
            this.btnSaveSwizzledTexture.Click += new System.EventHandler(this.BtnSaveTexture_Click);
            // 
            // btnSwizzleHashFolder
            // 
            this.btnSwizzleHashFolder.Location = new System.Drawing.Point(533, 402);
            this.btnSwizzleHashFolder.Name = "btnSwizzleHashFolder";
            this.btnSwizzleHashFolder.Size = new System.Drawing.Size(223, 40);
            this.btnSwizzleHashFolder.TabIndex = 28;
            this.btnSwizzleHashFolder.Text = "Swizzle Hash Folder";
            this.btnSwizzleHashFolder.UseVisualStyleBackColor = true;
            this.btnSwizzleHashFolder.Click += new System.EventHandler(this.BtnSwizzleTextureHashFolder_Click);
            // 
            // txtHash
            // 
            this.txtHash.Location = new System.Drawing.Point(554, 255);
            this.txtHash.Name = "txtHash";
            this.txtHash.ReadOnly = true;
            this.txtHash.Size = new System.Drawing.Size(185, 22);
            this.txtHash.TabIndex = 45;
            this.txtHash.TabStop = false;
            this.txtHash.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtHash.Visible = false;
            // 
            // lbHash
            // 
            this.lbHash.AutoSize = true;
            this.lbHash.Location = new System.Drawing.Point(552, 235);
            this.lbHash.Name = "lbHash";
            this.lbHash.Size = new System.Drawing.Size(45, 17);
            this.lbHash.TabIndex = 44;
            this.lbHash.Text = "Hash:";
            this.lbHash.Visible = false;
            // 
            // txtState
            // 
            this.txtState.Location = new System.Drawing.Point(673, 170);
            this.txtState.Name = "txtState";
            this.txtState.ReadOnly = true;
            this.txtState.Size = new System.Drawing.Size(66, 22);
            this.txtState.TabIndex = 43;
            this.txtState.TabStop = false;
            this.txtState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtParam
            // 
            this.txtParam.Location = new System.Drawing.Point(673, 146);
            this.txtParam.Name = "txtParam";
            this.txtParam.ReadOnly = true;
            this.txtParam.Size = new System.Drawing.Size(66, 22);
            this.txtParam.TabIndex = 42;
            this.txtParam.TabStop = false;
            this.txtParam.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtTextureID
            // 
            this.txtTextureID.Location = new System.Drawing.Point(673, 98);
            this.txtTextureID.Name = "txtTextureID";
            this.txtTextureID.ReadOnly = true;
            this.txtTextureID.Size = new System.Drawing.Size(66, 22);
            this.txtTextureID.TabIndex = 41;
            this.txtTextureID.TabStop = false;
            this.txtTextureID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbPalette
            // 
            this.lbPalette.AutoSize = true;
            this.lbPalette.Location = new System.Drawing.Point(603, 125);
            this.lbPalette.Name = "lbPalette";
            this.lbPalette.Size = new System.Drawing.Size(56, 17);
            this.lbPalette.TabIndex = 40;
            this.lbPalette.Text = "Palette:";
            // 
            // lbState
            // 
            this.lbState.AutoSize = true;
            this.lbState.Location = new System.Drawing.Point(614, 173);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(45, 17);
            this.lbState.TabIndex = 39;
            this.lbState.Text = "State:";
            // 
            // lbParam
            // 
            this.lbParam.AutoSize = true;
            this.lbParam.Location = new System.Drawing.Point(606, 149);
            this.lbParam.Name = "lbParam";
            this.lbParam.Size = new System.Drawing.Size(53, 17);
            this.lbParam.TabIndex = 38;
            this.lbParam.Text = "Param:";
            // 
            // lbTextureID
            // 
            this.lbTextureID.AutoSize = true;
            this.lbTextureID.Location = new System.Drawing.Point(599, 101);
            this.lbTextureID.Name = "lbTextureID";
            this.lbTextureID.Size = new System.Drawing.Size(60, 17);
            this.lbTextureID.TabIndex = 37;
            this.lbTextureID.Text = "Texture:";
            // 
            // btnSwizzleHashImg
            // 
            this.btnSwizzleHashImg.Enabled = false;
            this.btnSwizzleHashImg.Location = new System.Drawing.Point(533, 360);
            this.btnSwizzleHashImg.Name = "btnSwizzleHashImg";
            this.btnSwizzleHashImg.Size = new System.Drawing.Size(224, 40);
            this.btnSwizzleHashImg.TabIndex = 26;
            this.btnSwizzleHashImg.Text = "Swizzle Hashed Image";
            this.btnSwizzleHashImg.UseVisualStyleBackColor = true;
            this.btnSwizzleHashImg.Click += new System.EventHandler(this.BtnSwizzle_Click);
            // 
            // txtTextureHeight
            // 
            this.txtTextureHeight.Location = new System.Drawing.Point(673, 35);
            this.txtTextureHeight.Name = "txtTextureHeight";
            this.txtTextureHeight.ReadOnly = true;
            this.txtTextureHeight.Size = new System.Drawing.Size(66, 22);
            this.txtTextureHeight.TabIndex = 36;
            this.txtTextureHeight.TabStop = false;
            this.txtTextureHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtTextureWidth
            // 
            this.txtTextureWidth.Location = new System.Drawing.Point(673, 11);
            this.txtTextureWidth.Name = "txtTextureWidth";
            this.txtTextureWidth.ReadOnly = true;
            this.txtTextureWidth.Size = new System.Drawing.Size(66, 22);
            this.txtTextureWidth.TabIndex = 35;
            this.txtTextureWidth.TabStop = false;
            this.txtTextureWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbTextureHeight
            // 
            this.lbTextureHeight.AutoSize = true;
            this.lbTextureHeight.Location = new System.Drawing.Point(554, 38);
            this.lbTextureHeight.Name = "lbTextureHeight";
            this.lbTextureHeight.Size = new System.Drawing.Size(105, 17);
            this.lbTextureHeight.TabIndex = 34;
            this.lbTextureHeight.Text = "Texture Height:";
            // 
            // lbTextureWidth
            // 
            this.lbTextureWidth.AutoSize = true;
            this.lbTextureWidth.Location = new System.Drawing.Point(559, 14);
            this.lbTextureWidth.Name = "lbTextureWidth";
            this.lbTextureWidth.Size = new System.Drawing.Size(100, 17);
            this.lbTextureWidth.TabIndex = 33;
            this.lbTextureWidth.Text = "Texture Width:";
            // 
            // txtScaleFactor
            // 
            this.txtScaleFactor.Location = new System.Drawing.Point(673, 60);
            this.txtScaleFactor.Name = "txtScaleFactor";
            this.txtScaleFactor.ReadOnly = true;
            this.txtScaleFactor.Size = new System.Drawing.Size(66, 22);
            this.txtScaleFactor.TabIndex = 31;
            this.txtScaleFactor.TabStop = false;
            this.txtScaleFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbScaleFactor
            // 
            this.lbScaleFactor.AutoSize = true;
            this.lbScaleFactor.Location = new System.Drawing.Point(568, 63);
            this.lbScaleFactor.Name = "lbScaleFactor";
            this.lbScaleFactor.Size = new System.Drawing.Size(91, 17);
            this.lbScaleFactor.TabIndex = 29;
            this.lbScaleFactor.Text = "Scale Factor:";
            // 
            // panelSwizzledPreview
            // 
            this.panelSwizzledPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelSwizzledPreview.Controls.Add(this.pbSwizzledTexturePreview);
            this.panelSwizzledPreview.Location = new System.Drawing.Point(11, 10);
            this.panelSwizzledPreview.Name = "panelSwizzledPreview";
            this.panelSwizzledPreview.Size = new System.Drawing.Size(516, 516);
            this.panelSwizzledPreview.TabIndex = 27;
            // 
            // pbSwizzledTexturePreview
            // 
            this.pbSwizzledTexturePreview.Location = new System.Drawing.Point(0, 0);
            this.pbSwizzledTexturePreview.Name = "pbSwizzledTexturePreview";
            this.pbSwizzledTexturePreview.Size = new System.Drawing.Size(512, 512);
            this.pbSwizzledTexturePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSwizzledTexturePreview.TabIndex = 0;
            this.pbSwizzledTexturePreview.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(533, 486);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(224, 40);
            this.btnClose.TabIndex = 32;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnLoadUnsHashImg
            // 
            this.btnLoadUnsHashImg.Location = new System.Drawing.Point(533, 318);
            this.btnLoadUnsHashImg.Name = "btnLoadUnsHashImg";
            this.btnLoadUnsHashImg.Size = new System.Drawing.Size(224, 40);
            this.btnLoadUnsHashImg.TabIndex = 25;
            this.btnLoadUnsHashImg.Text = "Load Unswizzled Hashed Image";
            this.btnLoadUnsHashImg.UseVisualStyleBackColor = true;
            this.btnLoadUnsHashImg.Click += new System.EventHandler(this.BtnLoadTexture_Click);
            // 
            // frmSwizzleExternalHashImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(768, 536);
            this.Controls.Add(this.txtTileID);
            this.Controls.Add(this.lbTileID);
            this.Controls.Add(this.txtPalette);
            this.Controls.Add(this.btnSaveSwizzledTexture);
            this.Controls.Add(this.btnSwizzleHashFolder);
            this.Controls.Add(this.txtHash);
            this.Controls.Add(this.lbHash);
            this.Controls.Add(this.txtState);
            this.Controls.Add(this.txtParam);
            this.Controls.Add(this.txtTextureID);
            this.Controls.Add(this.lbPalette);
            this.Controls.Add(this.lbState);
            this.Controls.Add(this.lbParam);
            this.Controls.Add(this.lbTextureID);
            this.Controls.Add(this.btnSwizzleHashImg);
            this.Controls.Add(this.txtTextureHeight);
            this.Controls.Add(this.txtTextureWidth);
            this.Controls.Add(this.lbTextureHeight);
            this.Controls.Add(this.lbTextureWidth);
            this.Controls.Add(this.txtScaleFactor);
            this.Controls.Add(this.lbScaleFactor);
            this.Controls.Add(this.panelSwizzledPreview);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnLoadUnsHashImg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "frmSwizzleExternalHashImage";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Swizzle External Hashed Image";
            this.panelSwizzledPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSwizzledTexturePreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox txtTileID;
        internal System.Windows.Forms.Label lbTileID;
        internal System.Windows.Forms.TextBox txtPalette;
        internal System.Windows.Forms.Button btnSaveSwizzledTexture;
        internal System.Windows.Forms.Button btnSwizzleHashFolder;
        internal System.Windows.Forms.TextBox txtHash;
        internal System.Windows.Forms.Label lbHash;
        internal System.Windows.Forms.TextBox txtState;
        internal System.Windows.Forms.TextBox txtParam;
        internal System.Windows.Forms.TextBox txtTextureID;
        internal System.Windows.Forms.Label lbPalette;
        internal System.Windows.Forms.Label lbState;
        internal System.Windows.Forms.Label lbParam;
        internal System.Windows.Forms.Label lbTextureID;
        internal System.Windows.Forms.Button btnSwizzleHashImg;
        internal System.Windows.Forms.TextBox txtTextureHeight;
        internal System.Windows.Forms.TextBox txtTextureWidth;
        internal System.Windows.Forms.Label lbTextureHeight;
        internal System.Windows.Forms.Label lbTextureWidth;
        internal System.Windows.Forms.TextBox txtScaleFactor;
        internal System.Windows.Forms.Label lbScaleFactor;
        internal System.Windows.Forms.Panel panelSwizzledPreview;
        internal System.Windows.Forms.PictureBox pbSwizzledTexturePreview;
        internal System.Windows.Forms.Button btnClose;
        internal System.Windows.Forms.Button btnLoadUnsHashImg;
        internal System.Windows.Forms.OpenFileDialog ofdTexture;
        internal System.Windows.Forms.SaveFileDialog sfdTexture;
    }
}