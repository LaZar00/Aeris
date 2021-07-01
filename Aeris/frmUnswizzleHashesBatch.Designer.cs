namespace Aeris
{
    partial class frmUnswizzleHashesBatch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUnswizzleHashesBatch));
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.btnOutputFolder = new System.Windows.Forms.Button();
            this.btnInputFolder = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.txtInputFolder = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.lbSelectOutputFolder = new System.Windows.Forms.Label();
            this.lbSelectInputFolder = new System.Windows.Forms.Label();
            this.lbNote = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rtbResult
            // 
            this.rtbResult.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbResult.Location = new System.Drawing.Point(12, 306);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.ReadOnly = true;
            this.rtbResult.Size = new System.Drawing.Size(613, 102);
            this.rtbResult.TabIndex = 19;
            this.rtbResult.TabStop = false;
            this.rtbResult.Text = "";
            this.rtbResult.WordWrap = false;
            this.rtbResult.TextChanged += new System.EventHandler(this.rtbResult_TextChanged);
            // 
            // btnOutputFolder
            // 
            this.btnOutputFolder.Location = new System.Drawing.Point(590, 262);
            this.btnOutputFolder.Name = "btnOutputFolder";
            this.btnOutputFolder.Size = new System.Drawing.Size(31, 23);
            this.btnOutputFolder.TabIndex = 17;
            this.btnOutputFolder.Text = "...";
            this.btnOutputFolder.UseVisualStyleBackColor = true;
            this.btnOutputFolder.Click += new System.EventHandler(this.btnOutputFolder_Click);
            // 
            // btnInputFolder
            // 
            this.btnInputFolder.Location = new System.Drawing.Point(590, 179);
            this.btnInputFolder.Name = "btnInputFolder";
            this.btnInputFolder.Size = new System.Drawing.Size(31, 23);
            this.btnInputFolder.TabIndex = 16;
            this.btnInputFolder.Text = "...";
            this.btnInputFolder.UseVisualStyleBackColor = true;
            this.btnInputFolder.Click += new System.EventHandler(this.btnInputFolder_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(12, 263);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(576, 22);
            this.txtOutputFolder.TabIndex = 12;
            // 
            // txtInputFolder
            // 
            this.txtInputFolder.Location = new System.Drawing.Point(12, 179);
            this.txtInputFolder.Name = "txtInputFolder";
            this.txtInputFolder.Size = new System.Drawing.Size(576, 22);
            this.txtInputFolder.TabIndex = 10;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(329, 423);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(296, 44);
            this.btnClose.TabIndex = 18;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 423);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(296, 44);
            this.btnRun.TabIndex = 14;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lbSelectOutputFolder
            // 
            this.lbSelectOutputFolder.Location = new System.Drawing.Point(12, 226);
            this.lbSelectOutputFolder.Name = "lbSelectOutputFolder";
            this.lbSelectOutputFolder.Size = new System.Drawing.Size(609, 40);
            this.lbSelectOutputFolder.TabIndex = 15;
            this.lbSelectOutputFolder.Text = "Select Output folder where the Unswizzled Hashed Images will be put. The format o" +
    "f the folders is \"texture_palette\\hashed texture\\.png files\":";
            // 
            // lbSelectInputFolder
            // 
            this.lbSelectInputFolder.AutoSize = true;
            this.lbSelectInputFolder.Location = new System.Drawing.Point(12, 137);
            this.lbSelectInputFolder.Name = "lbSelectInputFolder";
            this.lbSelectInputFolder.Size = new System.Drawing.Size(449, 34);
            this.lbSelectInputFolder.TabIndex = 13;
            this.lbSelectInputFolder.Text = "Select Input folder where the hashed swizzled textures files are saved.\r\n(MANDATO" +
    "RY: for the loaded Field):";
            // 
            // lbNote
            // 
            this.lbNote.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbNote.Location = new System.Drawing.Point(12, 9);
            this.lbNote.Name = "lbNote";
            this.lbNote.Size = new System.Drawing.Size(613, 97);
            this.lbNote.TabIndex = 11;
            this.lbNote.Text = resources.GetString("lbNote.Text");
            // 
            // frmUnswizzleHashesBatch
            // 
            this.AcceptButton = this.btnRun;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(634, 477);
            this.Controls.Add(this.rtbResult);
            this.Controls.Add(this.btnOutputFolder);
            this.Controls.Add(this.btnInputFolder);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.txtInputFolder);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.lbSelectOutputFolder);
            this.Controls.Add(this.lbSelectInputFolder);
            this.Controls.Add(this.lbNote);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "frmUnswizzleHashesBatch";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unswizzle Hashed Textures (Batch)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.RichTextBox rtbResult;
        internal System.Windows.Forms.Button btnOutputFolder;
        internal System.Windows.Forms.Button btnInputFolder;
        internal System.Windows.Forms.TextBox txtOutputFolder;
        internal System.Windows.Forms.TextBox txtInputFolder;
        internal System.Windows.Forms.Button btnClose;
        internal System.Windows.Forms.Button btnRun;
        internal System.Windows.Forms.Label lbSelectOutputFolder;
        internal System.Windows.Forms.Label lbSelectInputFolder;
        internal System.Windows.Forms.Label lbNote;
    }
}