
namespace Aeris
{
    partial class frmUnswizzleExternalBaseTextures
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUnswizzleExternalBaseTextures));
            this.lbSelectOutputFolder = new System.Windows.Forms.Label();
            this.lbSelectInputFolder = new System.Windows.Forms.Label();
            this.lbNote = new System.Windows.Forms.Label();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.btnOutputFolder = new System.Windows.Forms.Button();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.txtInputFolder = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnInputFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbSelectOutputFolder
            // 
            this.lbSelectOutputFolder.Location = new System.Drawing.Point(12, 197);
            this.lbSelectOutputFolder.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbSelectOutputFolder.Name = "lbSelectOutputFolder";
            this.lbSelectOutputFolder.Size = new System.Drawing.Size(460, 32);
            this.lbSelectOutputFolder.TabIndex = 51;
            this.lbSelectOutputFolder.Text = "Select the Output folder where to Save All the Unswizzled Base Images in .PNG for" +
    "mat (MANDATORY: for the loaded Field):";
            // 
            // lbSelectInputFolder
            // 
            this.lbSelectInputFolder.Location = new System.Drawing.Point(12, 115);
            this.lbSelectInputFolder.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbSelectInputFolder.Name = "lbSelectInputFolder";
            this.lbSelectInputFolder.Size = new System.Drawing.Size(457, 35);
            this.lbSelectInputFolder.TabIndex = 50;
            this.lbSelectInputFolder.Text = "Select the Input folder where are All the Swizzled Base Textures in .PNG format (" +
    "MANDATORY: for the loaded Field):";
            // 
            // lbNote
            // 
            this.lbNote.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbNote.Location = new System.Drawing.Point(12, 11);
            this.lbNote.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbNote.Name = "lbNote";
            this.lbNote.Size = new System.Drawing.Size(460, 79);
            this.lbNote.TabIndex = 49;
            this.lbNote.Text = resources.GetString("lbNote.Text");
            // 
            // rtbResult
            // 
            this.rtbResult.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbResult.Location = new System.Drawing.Point(12, 273);
            this.rtbResult.Margin = new System.Windows.Forms.Padding(2);
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.ReadOnly = true;
            this.rtbResult.Size = new System.Drawing.Size(461, 84);
            this.rtbResult.TabIndex = 48;
            this.rtbResult.TabStop = false;
            this.rtbResult.Text = "";
            this.rtbResult.WordWrap = false;
            // 
            // btnOutputFolder
            // 
            this.btnOutputFolder.Location = new System.Drawing.Point(446, 230);
            this.btnOutputFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnOutputFolder.Name = "btnOutputFolder";
            this.btnOutputFolder.Size = new System.Drawing.Size(23, 19);
            this.btnOutputFolder.TabIndex = 46;
            this.btnOutputFolder.Text = "...";
            this.btnOutputFolder.UseVisualStyleBackColor = true;
            this.btnOutputFolder.Click += new System.EventHandler(this.btnOutputFolder_Click);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(12, 231);
            this.txtOutputFolder.Margin = new System.Windows.Forms.Padding(2);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(433, 20);
            this.txtOutputFolder.TabIndex = 44;
            // 
            // txtInputFolder
            // 
            this.txtInputFolder.Location = new System.Drawing.Point(12, 151);
            this.txtInputFolder.Margin = new System.Windows.Forms.Padding(2);
            this.txtInputFolder.Name = "txtInputFolder";
            this.txtInputFolder.Size = new System.Drawing.Size(433, 20);
            this.txtInputFolder.TabIndex = 43;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(250, 368);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(222, 36);
            this.btnClose.TabIndex = 47;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(12, 368);
            this.btnRun.Margin = new System.Windows.Forms.Padding(2);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(222, 36);
            this.btnRun.TabIndex = 45;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnInputFolder
            // 
            this.btnInputFolder.Location = new System.Drawing.Point(446, 150);
            this.btnInputFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnInputFolder.Name = "btnInputFolder";
            this.btnInputFolder.Size = new System.Drawing.Size(23, 19);
            this.btnInputFolder.TabIndex = 52;
            this.btnInputFolder.Text = "...";
            this.btnInputFolder.UseVisualStyleBackColor = true;
            this.btnInputFolder.Click += new System.EventHandler(this.btnInputFolder_Click);
            // 
            // frmUnswizzleExternalBaseTextures
            // 
            this.AcceptButton = this.btnRun;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(484, 415);
            this.Controls.Add(this.btnInputFolder);
            this.Controls.Add(this.lbSelectOutputFolder);
            this.Controls.Add(this.lbSelectInputFolder);
            this.Controls.Add(this.lbNote);
            this.Controls.Add(this.rtbResult);
            this.Controls.Add(this.btnOutputFolder);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.txtInputFolder);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRun);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmUnswizzleExternalBaseTextures";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unswizzle External Base Textures";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label lbSelectOutputFolder;
        internal System.Windows.Forms.Label lbSelectInputFolder;
        internal System.Windows.Forms.Label lbNote;
        internal System.Windows.Forms.RichTextBox rtbResult;
        internal System.Windows.Forms.Button btnOutputFolder;
        internal System.Windows.Forms.TextBox txtOutputFolder;
        internal System.Windows.Forms.TextBox txtInputFolder;
        internal System.Windows.Forms.Button btnClose;
        internal System.Windows.Forms.Button btnRun;
        internal System.Windows.Forms.Button btnInputFolder;
    }
}