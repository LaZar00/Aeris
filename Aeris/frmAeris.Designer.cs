using System.Windows.Forms;

namespace Aeris
{
    public partial class frmAeris
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAeris));
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.Repairfr_eToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveFieldToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveFieldAsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExportToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.ImportFromToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.TextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PreviewTextureToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.UnswizzleInternalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.UnswizzleExternalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SwizzleExternalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.UnswizzleHashedBatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SwizzleHashedBatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.ExportToPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExportAllToPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.ImportTextureToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.StageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BasePreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.FillBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BlackStageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BlackImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoFillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.X2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.X4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MarkTileBackgroundToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SwizzleAllExportedBaseToPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EventsLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ActivateLoggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveEventsAsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ClearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbSublayers = new System.Windows.Forms.GroupBox();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.clbSublayers = new System.Windows.Forms.CheckedListBox();
            this.gbTile = new System.Windows.Forms.GroupBox();
            this.txtBlendMode = new System.Windows.Forms.TextBox();
            this.lbTransp = new System.Windows.Forms.Label();
            this.lbTile = new System.Windows.Forms.Label();
            this.txtBigID = new System.Windows.Forms.TextBox();
            this.lbBigID = new System.Windows.Forms.Label();
            this.panelpbTile = new System.Windows.Forms.Panel();
            this.pbTile = new System.Windows.Forms.PictureBox();
            this.cbIFX = new System.Windows.Forms.CheckBox();
            this.txtTileTex = new System.Windows.Forms.TextBox();
            this.txtSrcY2 = new System.Windows.Forms.TextBox();
            this.txtSrcX2 = new System.Windows.Forms.TextBox();
            this.lbSrcY2 = new System.Windows.Forms.Label();
            this.lbSrcX2 = new System.Windows.Forms.Label();
            this.txtTexture2 = new System.Windows.Forms.TextBox();
            this.lbTexture2 = new System.Windows.Forms.Label();
            this.txtTexture = new System.Windows.Forms.TextBox();
            this.lbTX = new System.Windows.Forms.Label();
            this.txtPalette = new System.Windows.Forms.TextBox();
            this.lbPalette = new System.Windows.Forms.Label();
            this.txtState = new System.Windows.Forms.TextBox();
            this.lbState = new System.Windows.Forms.Label();
            this.txtParam = new System.Windows.Forms.TextBox();
            this.txtID = new System.Windows.Forms.TextBox();
            this.lbID = new System.Windows.Forms.Label();
            this.lbParam = new System.Windows.Forms.Label();
            this.cbBlending = new System.Windows.Forms.CheckBox();
            this.txtDestY = new System.Windows.Forms.TextBox();
            this.txtDestX = new System.Windows.Forms.TextBox();
            this.lbDestY = new System.Windows.Forms.Label();
            this.lbDestX = new System.Windows.Forms.Label();
            this.txtTileSize = new System.Windows.Forms.TextBox();
            this.lbSize = new System.Windows.Forms.Label();
            this.txtSrcY = new System.Windows.Forms.TextBox();
            this.txtSrcX = new System.Windows.Forms.TextBox();
            this.txtTile = new System.Windows.Forms.TextBox();
            this.txtLayer = new System.Windows.Forms.TextBox();
            this.lbSrcY = new System.Windows.Forms.Label();
            this.lbSrcX = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.lbLayer = new System.Windows.Forms.Label();
            this.btnTileLeft = new System.Windows.Forms.Button();
            this.btnTileRight = new System.Windows.Forms.Button();
            this.panelpbTexture = new System.Windows.Forms.Panel();
            this.pbTexture = new System.Windows.Forms.PictureBox();
            this.panelpbBackground = new System.Windows.Forms.Panel();
            this.pbBackground = new System.Windows.Forms.PictureBox();
            this.rtbEvents = new System.Windows.Forms.RichTextBox();
            this.tcParams = new System.Windows.Forms.TabControl();
            this.gbLayers = new System.Windows.Forms.GroupBox();
            this.cbLayer3 = new System.Windows.Forms.CheckBox();
            this.cbLayer2 = new System.Windows.Forms.CheckBox();
            this.cbLayer1 = new System.Windows.Forms.CheckBox();
            this.cbLayer0 = new System.Windows.Forms.CheckBox();
            this.cbTextures = new System.Windows.Forms.ComboBox();
            this.cbPalettes = new System.Windows.Forms.ComboBox();
            this.pbPalette = new System.Windows.Forms.PictureBox();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.MenuStrip1.SuspendLayout();
            this.gbSublayers.SuspendLayout();
            this.gbTile.SuspendLayout();
            this.panelpbTile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTile)).BeginInit();
            this.panelpbTexture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTexture)).BeginInit();
            this.panelpbBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackground)).BeginInit();
            this.gbLayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPalette)).BeginInit();
            this.SuspendLayout();
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.MenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.PaletteToolStripMenuItem,
            this.TextureToolStripMenuItem,
            this.StageToolStripMenuItem,
            this.EventsLogToolStripMenuItem});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Size = new System.Drawing.Size(1361, 28);
            this.MenuStrip1.TabIndex = 1;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.ToolStripMenuItem1,
            this.Repairfr_eToolStripMenuItem2,
            this.ToolStripSeparator1,
            this.SaveFieldToolStripMenuItem2,
            this.SaveFieldAsToolStripMenuItem2,
            this.ToolStripSeparator8,
            this.CloseToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.FileToolStripMenuItem.Text = "File";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(185, 24);
            this.OpenToolStripMenuItem.Text = "Open Field";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(182, 6);
            // 
            // Repairfr_eToolStripMenuItem2
            // 
            this.Repairfr_eToolStripMenuItem2.Enabled = false;
            this.Repairfr_eToolStripMenuItem2.Name = "Repairfr_eToolStripMenuItem2";
            this.Repairfr_eToolStripMenuItem2.Size = new System.Drawing.Size(185, 24);
            this.Repairfr_eToolStripMenuItem2.Text = "Repair Field fr_e";
            this.Repairfr_eToolStripMenuItem2.Click += new System.EventHandler(this.Repairfr_eToolStripMenuItem2_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(182, 6);
            // 
            // SaveFieldToolStripMenuItem2
            // 
            this.SaveFieldToolStripMenuItem2.Enabled = false;
            this.SaveFieldToolStripMenuItem2.Name = "SaveFieldToolStripMenuItem2";
            this.SaveFieldToolStripMenuItem2.Size = new System.Drawing.Size(185, 24);
            this.SaveFieldToolStripMenuItem2.Text = "Save Field";
            this.SaveFieldToolStripMenuItem2.Click += new System.EventHandler(this.SaveFieldToolStripMenuItem2_Click);
            // 
            // SaveFieldAsToolStripMenuItem2
            // 
            this.SaveFieldAsToolStripMenuItem2.Enabled = false;
            this.SaveFieldAsToolStripMenuItem2.Name = "SaveFieldAsToolStripMenuItem2";
            this.SaveFieldAsToolStripMenuItem2.Size = new System.Drawing.Size(185, 24);
            this.SaveFieldAsToolStripMenuItem2.Text = "Save Field As...";
            this.SaveFieldAsToolStripMenuItem2.Click += new System.EventHandler(this.SaveFieldAsToolStripMenuItem2_Click);
            // 
            // ToolStripSeparator8
            // 
            this.ToolStripSeparator8.Name = "ToolStripSeparator8";
            this.ToolStripSeparator8.Size = new System.Drawing.Size(182, 6);
            // 
            // CloseToolStripMenuItem
            // 
            this.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem";
            this.CloseToolStripMenuItem.Size = new System.Drawing.Size(185, 24);
            this.CloseToolStripMenuItem.Text = "Close";
            this.CloseToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // PaletteToolStripMenuItem
            // 
            this.PaletteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExportToToolStripMenuItem,
            this.ToolStripSeparator10,
            this.ImportFromToolStripMenuItem2});
            this.PaletteToolStripMenuItem.Enabled = false;
            this.PaletteToolStripMenuItem.Name = "PaletteToolStripMenuItem";
            this.PaletteToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.PaletteToolStripMenuItem.Text = "Palette";
            // 
            // ExportToToolStripMenuItem
            // 
            this.ExportToToolStripMenuItem.Name = "ExportToToolStripMenuItem";
            this.ExportToToolStripMenuItem.Size = new System.Drawing.Size(170, 24);
            this.ExportToToolStripMenuItem.Text = "Export To...";
            this.ExportToToolStripMenuItem.Click += new System.EventHandler(this.ExportToToolStripMenuItem_Click);
            // 
            // ToolStripSeparator10
            // 
            this.ToolStripSeparator10.Name = "ToolStripSeparator10";
            this.ToolStripSeparator10.Size = new System.Drawing.Size(167, 6);
            // 
            // ImportFromToolStripMenuItem2
            // 
            this.ImportFromToolStripMenuItem2.Name = "ImportFromToolStripMenuItem2";
            this.ImportFromToolStripMenuItem2.Size = new System.Drawing.Size(170, 24);
            this.ImportFromToolStripMenuItem2.Text = "Import From...";
            this.ImportFromToolStripMenuItem2.Click += new System.EventHandler(this.ImportPaletteToolStripMenuItem2_Click);
            // 
            // TextureToolStripMenuItem
            // 
            this.TextureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PreviewTextureToolStripMenuItem1,
            this.UnswizzleInternalToolStripMenuItem,
            this.ToolStripSeparator6,
            this.UnswizzleExternalToolStripMenuItem,
            this.SwizzleExternalToolStripMenuItem,
            this.ToolStripSeparator4,
            this.UnswizzleHashedBatchToolStripMenuItem,
            this.SwizzleHashedBatchToolStripMenuItem,
            this.ToolStripSeparator5,
            this.ExportToPNGToolStripMenuItem,
            this.ExportAllToPNGToolStripMenuItem,
            this.ToolStripSeparator9,
            this.ImportTextureToolStripMenuItem2});
            this.TextureToolStripMenuItem.Enabled = false;
            this.TextureToolStripMenuItem.Name = "TextureToolStripMenuItem";
            this.TextureToolStripMenuItem.Size = new System.Drawing.Size(69, 24);
            this.TextureToolStripMenuItem.Text = "Texture";
            // 
            // PreviewTextureToolStripMenuItem1
            // 
            this.PreviewTextureToolStripMenuItem1.Name = "PreviewTextureToolStripMenuItem1";
            this.PreviewTextureToolStripMenuItem1.Size = new System.Drawing.Size(306, 24);
            this.PreviewTextureToolStripMenuItem1.Text = "Basic Texture Preview";
            this.PreviewTextureToolStripMenuItem1.Click += new System.EventHandler(this.PreviewTextureToolStripMenuItem1_Click);
            // 
            // UnswizzleInternalToolStripMenuItem
            // 
            this.UnswizzleInternalToolStripMenuItem.Name = "UnswizzleInternalToolStripMenuItem";
            this.UnswizzleInternalToolStripMenuItem.Size = new System.Drawing.Size(306, 24);
            this.UnswizzleInternalToolStripMenuItem.Text = "Unswizzle Internal Preview";
            this.UnswizzleInternalToolStripMenuItem.Click += new System.EventHandler(this.UnswizzleInternalToolStripMenuItem_Click);
            // 
            // ToolStripSeparator6
            // 
            this.ToolStripSeparator6.Name = "ToolStripSeparator6";
            this.ToolStripSeparator6.Size = new System.Drawing.Size(303, 6);
            // 
            // UnswizzleExternalToolStripMenuItem
            // 
            this.UnswizzleExternalToolStripMenuItem.Name = "UnswizzleExternalToolStripMenuItem";
            this.UnswizzleExternalToolStripMenuItem.Size = new System.Drawing.Size(306, 24);
            this.UnswizzleExternalToolStripMenuItem.Text = "Unswizzle External...";
            this.UnswizzleExternalToolStripMenuItem.Click += new System.EventHandler(this.UnswizzleExternalToolStripMenuItem_Click);
            // 
            // SwizzleExternalToolStripMenuItem
            // 
            this.SwizzleExternalToolStripMenuItem.Name = "SwizzleExternalToolStripMenuItem";
            this.SwizzleExternalToolStripMenuItem.Size = new System.Drawing.Size(306, 24);
            this.SwizzleExternalToolStripMenuItem.Text = "Swizzle External...";
            this.SwizzleExternalToolStripMenuItem.Click += new System.EventHandler(this.SwizzleExternalToolStripMenuItem_Click);
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(303, 6);
            // 
            // UnswizzleHashedBatchToolStripMenuItem
            // 
            this.UnswizzleHashedBatchToolStripMenuItem.Name = "UnswizzleHashedBatchToolStripMenuItem";
            this.UnswizzleHashedBatchToolStripMenuItem.Size = new System.Drawing.Size(306, 24);
            this.UnswizzleHashedBatchToolStripMenuItem.Text = "Unswizzle Hashed Textures (Batch)";
            this.UnswizzleHashedBatchToolStripMenuItem.Click += new System.EventHandler(this.UnswizzleHashedBatchToolStripMenuItem_Click);
            // 
            // SwizzleHashedBatchToolStripMenuItem
            // 
            this.SwizzleHashedBatchToolStripMenuItem.Name = "SwizzleHashedBatchToolStripMenuItem";
            this.SwizzleHashedBatchToolStripMenuItem.Size = new System.Drawing.Size(306, 24);
            this.SwizzleHashedBatchToolStripMenuItem.Text = "Swizzle Hashed Images (Batch)";
            this.SwizzleHashedBatchToolStripMenuItem.Click += new System.EventHandler(this.SwizzleHashedBatchToolStripMenuItem_Click);
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            this.ToolStripSeparator5.Size = new System.Drawing.Size(303, 6);
            // 
            // ExportToPNGToolStripMenuItem
            // 
            this.ExportToPNGToolStripMenuItem.Name = "ExportToPNGToolStripMenuItem";
            this.ExportToPNGToolStripMenuItem.Size = new System.Drawing.Size(306, 24);
            this.ExportToPNGToolStripMenuItem.Text = "Export Selected Texture...";
            this.ExportToPNGToolStripMenuItem.Click += new System.EventHandler(this.ExportToPNGToolStripMenuItem_Click);
            // 
            // ExportAllToPNGToolStripMenuItem
            // 
            this.ExportAllToPNGToolStripMenuItem.Name = "ExportAllToPNGToolStripMenuItem";
            this.ExportAllToPNGToolStripMenuItem.Size = new System.Drawing.Size(306, 24);
            this.ExportAllToPNGToolStripMenuItem.Text = "Export All Field Textures...";
            this.ExportAllToPNGToolStripMenuItem.Click += new System.EventHandler(this.ExportAllToPNGToolStripMenuItem_Click);
            // 
            // ToolStripSeparator9
            // 
            this.ToolStripSeparator9.Name = "ToolStripSeparator9";
            this.ToolStripSeparator9.Size = new System.Drawing.Size(303, 6);
            // 
            // ImportTextureToolStripMenuItem2
            // 
            this.ImportTextureToolStripMenuItem2.Name = "ImportTextureToolStripMenuItem2";
            this.ImportTextureToolStripMenuItem2.Size = new System.Drawing.Size(306, 24);
            this.ImportTextureToolStripMenuItem2.Text = "Import Texture...";
            this.ImportTextureToolStripMenuItem2.Click += new System.EventHandler(this.ImportTextureToolStripMenuItem2_Click);
            // 
            // StageToolStripMenuItem
            // 
            this.StageToolStripMenuItem.CheckOnClick = true;
            this.StageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BasePreviewToolStripMenuItem,
            this.ToolStripSeparator7,
            this.FillBackgroundToolStripMenuItem,
            this.ZoomToolStripMenuItem,
            this.RenderToolStripMenuItem,
            this.MarkTileBackgroundToolStripMenuItem2,
            this.ToolStripSeparator3,
            this.UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem,
            this.SwizzleAllExportedBaseToPNGToolStripMenuItem});
            this.StageToolStripMenuItem.Enabled = false;
            this.StageToolStripMenuItem.Name = "StageToolStripMenuItem";
            this.StageToolStripMenuItem.Size = new System.Drawing.Size(59, 24);
            this.StageToolStripMenuItem.Text = "Stage";
            // 
            // BasePreviewToolStripMenuItem
            // 
            this.BasePreviewToolStripMenuItem.Name = "BasePreviewToolStripMenuItem";
            this.BasePreviewToolStripMenuItem.Size = new System.Drawing.Size(312, 24);
            this.BasePreviewToolStripMenuItem.Text = "Base Preview";
            this.BasePreviewToolStripMenuItem.Click += new System.EventHandler(this.BasePreviewToolStripMenuItem_Click);
            // 
            // ToolStripSeparator7
            // 
            this.ToolStripSeparator7.Name = "ToolStripSeparator7";
            this.ToolStripSeparator7.Size = new System.Drawing.Size(309, 6);
            // 
            // FillBackgroundToolStripMenuItem
            // 
            this.FillBackgroundToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BlackStageToolStripMenuItem,
            this.BlackImageToolStripMenuItem});
            this.FillBackgroundToolStripMenuItem.Name = "FillBackgroundToolStripMenuItem";
            this.FillBackgroundToolStripMenuItem.Size = new System.Drawing.Size(312, 24);
            this.FillBackgroundToolStripMenuItem.Text = "Fill Background";
            // 
            // BlackStageToolStripMenuItem
            // 
            this.BlackStageToolStripMenuItem.CheckOnClick = true;
            this.BlackStageToolStripMenuItem.Name = "BlackStageToolStripMenuItem";
            this.BlackStageToolStripMenuItem.Size = new System.Drawing.Size(159, 24);
            this.BlackStageToolStripMenuItem.Text = "Black Stage";
            this.BlackStageToolStripMenuItem.Click += new System.EventHandler(this.BlackStageToolStripMenuItem_Click);
            // 
            // BlackImageToolStripMenuItem
            // 
            this.BlackImageToolStripMenuItem.CheckOnClick = true;
            this.BlackImageToolStripMenuItem.Name = "BlackImageToolStripMenuItem";
            this.BlackImageToolStripMenuItem.Size = new System.Drawing.Size(159, 24);
            this.BlackImageToolStripMenuItem.Text = "Black Image";
            this.BlackImageToolStripMenuItem.Click += new System.EventHandler(this.BlackImageToolStripMenuItem_Click);
            // 
            // ZoomToolStripMenuItem
            // 
            this.ZoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AutoFillToolStripMenuItem,
            this.X2ToolStripMenuItem,
            this.X4ToolStripMenuItem});
            this.ZoomToolStripMenuItem.Name = "ZoomToolStripMenuItem";
            this.ZoomToolStripMenuItem.Size = new System.Drawing.Size(312, 24);
            this.ZoomToolStripMenuItem.Text = "Zoom";
            // 
            // AutoFillToolStripMenuItem
            // 
            this.AutoFillToolStripMenuItem.CheckOnClick = true;
            this.AutoFillToolStripMenuItem.Name = "AutoFillToolStripMenuItem";
            this.AutoFillToolStripMenuItem.Size = new System.Drawing.Size(129, 24);
            this.AutoFillToolStripMenuItem.Text = "AutoFill";
            this.AutoFillToolStripMenuItem.Click += new System.EventHandler(this.AutoFillToolStripMenuItem_Click);
            // 
            // X2ToolStripMenuItem
            // 
            this.X2ToolStripMenuItem.CheckOnClick = true;
            this.X2ToolStripMenuItem.Name = "X2ToolStripMenuItem";
            this.X2ToolStripMenuItem.Size = new System.Drawing.Size(129, 24);
            this.X2ToolStripMenuItem.Text = "x2";
            this.X2ToolStripMenuItem.Click += new System.EventHandler(this.X2ToolStripMenuItem_Click);
            // 
            // X4ToolStripMenuItem
            // 
            this.X4ToolStripMenuItem.CheckOnClick = true;
            this.X4ToolStripMenuItem.Name = "X4ToolStripMenuItem";
            this.X4ToolStripMenuItem.Size = new System.Drawing.Size(129, 24);
            this.X4ToolStripMenuItem.Text = "x4";
            this.X4ToolStripMenuItem.Click += new System.EventHandler(this.X4ToolStripMenuItem_Click);
            // 
            // RenderToolStripMenuItem
            // 
            this.RenderToolStripMenuItem.CheckOnClick = true;
            this.RenderToolStripMenuItem.Name = "RenderToolStripMenuItem";
            this.RenderToolStripMenuItem.Size = new System.Drawing.Size(312, 24);
            this.RenderToolStripMenuItem.Text = "Render Effects";
            this.RenderToolStripMenuItem.Click += new System.EventHandler(this.RenderToolStripMenuItem_Click);
            // 
            // MarkTileBackgroundToolStripMenuItem2
            // 
            this.MarkTileBackgroundToolStripMenuItem2.CheckOnClick = true;
            this.MarkTileBackgroundToolStripMenuItem2.Name = "MarkTileBackgroundToolStripMenuItem2";
            this.MarkTileBackgroundToolStripMenuItem2.Size = new System.Drawing.Size(312, 24);
            this.MarkTileBackgroundToolStripMenuItem2.Text = "Mark Tile in Background";
            this.MarkTileBackgroundToolStripMenuItem2.Click += new System.EventHandler(this.MarkTileBackgroundToolStripMenuItem2_Click);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(309, 6);
            // 
            // UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem
            // 
            this.UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem.Name = "UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem";
            this.UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem.Size = new System.Drawing.Size(312, 24);
            this.UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem.Text = "Unswizzle All Internal Base Textures";
            this.UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem.Click += new System.EventHandler(this.UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem_Click);
            // 
            // SwizzleAllExportedBaseToPNGToolStripMenuItem
            // 
            this.SwizzleAllExportedBaseToPNGToolStripMenuItem.Name = "SwizzleAllExportedBaseToPNGToolStripMenuItem";
            this.SwizzleAllExportedBaseToPNGToolStripMenuItem.Size = new System.Drawing.Size(312, 24);
            this.SwizzleAllExportedBaseToPNGToolStripMenuItem.Text = "Swizzle All External Base Images";
            this.SwizzleAllExportedBaseToPNGToolStripMenuItem.Click += new System.EventHandler(this.SwizzleAllExportedBaseToPNGToolStripMenuItem_Click);
            // 
            // EventsLogToolStripMenuItem
            // 
            this.EventsLogToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ActivateLoggingToolStripMenuItem,
            this.SaveEventsAsToolStripMenuItem1,
            this.ToolStripSeparator2,
            this.ClearToolStripMenuItem});
            this.EventsLogToolStripMenuItem.Name = "EventsLogToolStripMenuItem";
            this.EventsLogToolStripMenuItem.Size = new System.Drawing.Size(92, 24);
            this.EventsLogToolStripMenuItem.Text = "Events Log";
            // 
            // ActivateLoggingToolStripMenuItem
            // 
            this.ActivateLoggingToolStripMenuItem.CheckOnClick = true;
            this.ActivateLoggingToolStripMenuItem.Name = "ActivateLoggingToolStripMenuItem";
            this.ActivateLoggingToolStripMenuItem.Size = new System.Drawing.Size(191, 24);
            this.ActivateLoggingToolStripMenuItem.Text = "Activate Logging";
            this.ActivateLoggingToolStripMenuItem.Click += new System.EventHandler(this.ActivateLoggingToolStripMenuItem_Click);
            // 
            // SaveEventsAsToolStripMenuItem1
            // 
            this.SaveEventsAsToolStripMenuItem1.Name = "SaveEventsAsToolStripMenuItem1";
            this.SaveEventsAsToolStripMenuItem1.Size = new System.Drawing.Size(191, 24);
            this.SaveEventsAsToolStripMenuItem1.Text = "Save Events As...";
            this.SaveEventsAsToolStripMenuItem1.Click += new System.EventHandler(this.SaveEventsAsToolStripMenuItem1_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // ClearToolStripMenuItem
            // 
            this.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem";
            this.ClearToolStripMenuItem.Size = new System.Drawing.Size(191, 24);
            this.ClearToolStripMenuItem.Text = "Clear";
            this.ClearToolStripMenuItem.Click += new System.EventHandler(this.ClearToolStripMenuItem_Click);
            // 
            // gbSublayers
            // 
            this.gbSublayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSublayers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbSublayers.Controls.Add(this.btnUncheckAll);
            this.gbSublayers.Controls.Add(this.btnCheckAll);
            this.gbSublayers.Controls.Add(this.clbSublayers);
            this.gbSublayers.Enabled = false;
            this.gbSublayers.Location = new System.Drawing.Point(1168, 354);
            this.gbSublayers.Name = "gbSublayers";
            this.gbSublayers.Size = new System.Drawing.Size(186, 498);
            this.gbSublayers.TabIndex = 26;
            this.gbSublayers.TabStop = false;
            this.gbSublayers.Text = "Sublayers (Layer 1)";
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Location = new System.Drawing.Point(91, 28);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(90, 24);
            this.btnUncheckAll.TabIndex = 18;
            this.btnUncheckAll.Text = "Uncheck All";
            this.btnUncheckAll.UseVisualStyleBackColor = true;
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Location = new System.Drawing.Point(5, 28);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(78, 24);
            this.btnCheckAll.TabIndex = 17;
            this.btnCheckAll.Text = "Check All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // clbSublayers
            // 
            this.clbSublayers.CheckOnClick = true;
            this.clbSublayers.FormattingEnabled = true;
            this.clbSublayers.Location = new System.Drawing.Point(5, 62);
            this.clbSublayers.Name = "clbSublayers";
            this.clbSublayers.Size = new System.Drawing.Size(176, 429);
            this.clbSublayers.TabIndex = 16;
            this.clbSublayers.ThreeDCheckBoxes = true;
            this.clbSublayers.SelectedIndexChanged += new System.EventHandler(this.clbSublayers_SelectedIndexChanged);
            this.clbSublayers.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.clbSublayers_MouseDoubleClick);
            // 
            // gbTile
            // 
            this.gbTile.BackColor = System.Drawing.SystemColors.Control;
            this.gbTile.Controls.Add(this.txtBlendMode);
            this.gbTile.Controls.Add(this.lbTransp);
            this.gbTile.Controls.Add(this.lbTile);
            this.gbTile.Controls.Add(this.txtBigID);
            this.gbTile.Controls.Add(this.lbBigID);
            this.gbTile.Controls.Add(this.panelpbTile);
            this.gbTile.Controls.Add(this.cbIFX);
            this.gbTile.Controls.Add(this.txtTileTex);
            this.gbTile.Controls.Add(this.txtSrcY2);
            this.gbTile.Controls.Add(this.txtSrcX2);
            this.gbTile.Controls.Add(this.lbSrcY2);
            this.gbTile.Controls.Add(this.lbSrcX2);
            this.gbTile.Controls.Add(this.txtTexture2);
            this.gbTile.Controls.Add(this.lbTexture2);
            this.gbTile.Controls.Add(this.txtTexture);
            this.gbTile.Controls.Add(this.lbTX);
            this.gbTile.Controls.Add(this.txtPalette);
            this.gbTile.Controls.Add(this.lbPalette);
            this.gbTile.Controls.Add(this.txtState);
            this.gbTile.Controls.Add(this.lbState);
            this.gbTile.Controls.Add(this.txtParam);
            this.gbTile.Controls.Add(this.txtID);
            this.gbTile.Controls.Add(this.lbID);
            this.gbTile.Controls.Add(this.lbParam);
            this.gbTile.Controls.Add(this.cbBlending);
            this.gbTile.Controls.Add(this.txtDestY);
            this.gbTile.Controls.Add(this.txtDestX);
            this.gbTile.Controls.Add(this.lbDestY);
            this.gbTile.Controls.Add(this.lbDestX);
            this.gbTile.Controls.Add(this.txtTileSize);
            this.gbTile.Controls.Add(this.lbSize);
            this.gbTile.Controls.Add(this.txtSrcY);
            this.gbTile.Controls.Add(this.txtSrcX);
            this.gbTile.Controls.Add(this.txtTile);
            this.gbTile.Controls.Add(this.txtLayer);
            this.gbTile.Controls.Add(this.lbSrcY);
            this.gbTile.Controls.Add(this.lbSrcX);
            this.gbTile.Controls.Add(this.Label1);
            this.gbTile.Controls.Add(this.lbLayer);
            this.gbTile.Controls.Add(this.btnTileLeft);
            this.gbTile.Controls.Add(this.btnTileRight);
            this.gbTile.Enabled = false;
            this.gbTile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gbTile.Location = new System.Drawing.Point(6, 616);
            this.gbTile.Name = "gbTile";
            this.gbTile.Size = new System.Drawing.Size(259, 236);
            this.gbTile.TabIndex = 25;
            this.gbTile.TabStop = false;
            this.gbTile.Text = "Tile Info";
            // 
            // txtBlendMode
            // 
            this.txtBlendMode.BackColor = System.Drawing.SystemColors.Control;
            this.txtBlendMode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBlendMode.Location = new System.Drawing.Point(88, 174);
            this.txtBlendMode.Name = "txtBlendMode";
            this.txtBlendMode.ReadOnly = true;
            this.txtBlendMode.Size = new System.Drawing.Size(44, 15);
            this.txtBlendMode.TabIndex = 57;
            this.txtBlendMode.TabStop = false;
            this.txtBlendMode.WordWrap = false;
            // 
            // lbTransp
            // 
            this.lbTransp.AutoSize = true;
            this.lbTransp.Location = new System.Drawing.Point(4, 172);
            this.lbTransp.Name = "lbTransp";
            this.lbTransp.Size = new System.Drawing.Size(87, 17);
            this.lbTransp.TabIndex = 56;
            this.lbTransp.Text = "Blend Mode:";
            this.lbTransp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbTile
            // 
            this.lbTile.AutoSize = true;
            this.lbTile.Location = new System.Drawing.Point(23, 39);
            this.lbTile.Name = "lbTile";
            this.lbTile.Size = new System.Drawing.Size(35, 17);
            this.lbTile.TabIndex = 55;
            this.lbTile.Text = "Tile:";
            this.lbTile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBigID
            // 
            this.txtBigID.BackColor = System.Drawing.SystemColors.Control;
            this.txtBigID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBigID.Location = new System.Drawing.Point(189, 34);
            this.txtBigID.Name = "txtBigID";
            this.txtBigID.ReadOnly = true;
            this.txtBigID.Size = new System.Drawing.Size(60, 15);
            this.txtBigID.TabIndex = 54;
            this.txtBigID.TabStop = false;
            this.txtBigID.WordWrap = false;
            // 
            // lbBigID
            // 
            this.lbBigID.AutoSize = true;
            this.lbBigID.Location = new System.Drawing.Point(143, 32);
            this.lbBigID.Name = "lbBigID";
            this.lbBigID.Size = new System.Drawing.Size(45, 17);
            this.lbBigID.TabIndex = 53;
            this.lbBigID.Text = "BigID:";
            this.lbBigID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelpbTile
            // 
            this.panelpbTile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelpbTile.Controls.Add(this.pbTile);
            this.panelpbTile.Location = new System.Drawing.Point(29, 65);
            this.panelpbTile.Name = "panelpbTile";
            this.panelpbTile.Size = new System.Drawing.Size(84, 84);
            this.panelpbTile.TabIndex = 52;
            // 
            // pbTile
            // 
            this.pbTile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbTile.Location = new System.Drawing.Point(0, 0);
            this.pbTile.Name = "pbTile";
            this.pbTile.Size = new System.Drawing.Size(80, 80);
            this.pbTile.TabIndex = 15;
            this.pbTile.TabStop = false;
            this.pbTile.Click += new System.EventHandler(this.pbTile_Click);
            // 
            // cbIFX
            // 
            this.cbIFX.AutoCheck = false;
            this.cbIFX.AutoSize = true;
            this.cbIFX.Location = new System.Drawing.Point(98, 153);
            this.cbIFX.Name = "cbIFX";
            this.cbIFX.Size = new System.Drawing.Size(50, 21);
            this.cbIFX.TabIndex = 51;
            this.cbIFX.Text = "IFP";
            this.cbIFX.UseVisualStyleBackColor = true;
            // 
            // txtTileTex
            // 
            this.txtTileTex.BackColor = System.Drawing.SystemColors.Control;
            this.txtTileTex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTileTex.Location = new System.Drawing.Point(102, 185);
            this.txtTileTex.Name = "txtTileTex";
            this.txtTileTex.ReadOnly = true;
            this.txtTileTex.Size = new System.Drawing.Size(44, 22);
            this.txtTileTex.TabIndex = 49;
            this.txtTileTex.TabStop = false;
            this.txtTileTex.Visible = false;
            this.txtTileTex.WordWrap = false;
            // 
            // txtSrcY2
            // 
            this.txtSrcY2.BackColor = System.Drawing.SystemColors.Control;
            this.txtSrcY2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSrcY2.Location = new System.Drawing.Point(211, 214);
            this.txtSrcY2.Name = "txtSrcY2";
            this.txtSrcY2.ReadOnly = true;
            this.txtSrcY2.Size = new System.Drawing.Size(44, 15);
            this.txtSrcY2.TabIndex = 48;
            this.txtSrcY2.TabStop = false;
            this.txtSrcY2.WordWrap = false;
            // 
            // txtSrcX2
            // 
            this.txtSrcX2.BackColor = System.Drawing.SystemColors.Control;
            this.txtSrcX2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSrcX2.Location = new System.Drawing.Point(211, 198);
            this.txtSrcX2.Name = "txtSrcX2";
            this.txtSrcX2.ReadOnly = true;
            this.txtSrcX2.Size = new System.Drawing.Size(44, 15);
            this.txtSrcX2.TabIndex = 47;
            this.txtSrcX2.TabStop = false;
            this.txtSrcX2.WordWrap = false;
            // 
            // lbSrcY2
            // 
            this.lbSrcY2.AutoSize = true;
            this.lbSrcY2.Location = new System.Drawing.Point(163, 212);
            this.lbSrcY2.Name = "lbSrcY2";
            this.lbSrcY2.Size = new System.Drawing.Size(50, 17);
            this.lbSrcY2.TabIndex = 46;
            this.lbSrcY2.Text = "SrcY2:";
            this.lbSrcY2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbSrcX2
            // 
            this.lbSrcX2.AutoSize = true;
            this.lbSrcX2.Location = new System.Drawing.Point(163, 196);
            this.lbSrcX2.Name = "lbSrcX2";
            this.lbSrcX2.Size = new System.Drawing.Size(50, 17);
            this.lbSrcX2.TabIndex = 45;
            this.lbSrcX2.Text = "SrcX2:";
            this.lbSrcX2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTexture2
            // 
            this.txtTexture2.BackColor = System.Drawing.SystemColors.Control;
            this.txtTexture2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTexture2.Location = new System.Drawing.Point(211, 182);
            this.txtTexture2.Name = "txtTexture2";
            this.txtTexture2.ReadOnly = true;
            this.txtTexture2.Size = new System.Drawing.Size(34, 15);
            this.txtTexture2.TabIndex = 44;
            this.txtTexture2.TabStop = false;
            this.txtTexture2.WordWrap = false;
            // 
            // lbTexture2
            // 
            this.lbTexture2.AutoSize = true;
            this.lbTexture2.Location = new System.Drawing.Point(145, 180);
            this.lbTexture2.Name = "lbTexture2";
            this.lbTexture2.Size = new System.Drawing.Size(68, 17);
            this.lbTexture2.TabIndex = 43;
            this.lbTexture2.Text = "Texture2:";
            this.lbTexture2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTexture
            // 
            this.txtTexture.BackColor = System.Drawing.SystemColors.Control;
            this.txtTexture.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTexture.Location = new System.Drawing.Point(211, 128);
            this.txtTexture.Name = "txtTexture";
            this.txtTexture.ReadOnly = true;
            this.txtTexture.Size = new System.Drawing.Size(34, 15);
            this.txtTexture.TabIndex = 42;
            this.txtTexture.TabStop = false;
            this.txtTexture.WordWrap = false;
            // 
            // lbTX
            // 
            this.lbTX.AutoSize = true;
            this.lbTX.Location = new System.Drawing.Point(153, 126);
            this.lbTX.Name = "lbTX";
            this.lbTX.Size = new System.Drawing.Size(60, 17);
            this.lbTX.TabIndex = 41;
            this.lbTX.Text = "Texture:";
            this.lbTX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPalette
            // 
            this.txtPalette.BackColor = System.Drawing.SystemColors.Control;
            this.txtPalette.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPalette.Location = new System.Drawing.Point(211, 73);
            this.txtPalette.Name = "txtPalette";
            this.txtPalette.ReadOnly = true;
            this.txtPalette.Size = new System.Drawing.Size(44, 15);
            this.txtPalette.TabIndex = 40;
            this.txtPalette.TabStop = false;
            this.txtPalette.WordWrap = false;
            // 
            // lbPalette
            // 
            this.lbPalette.AutoSize = true;
            this.lbPalette.Location = new System.Drawing.Point(157, 71);
            this.lbPalette.Name = "lbPalette";
            this.lbPalette.Size = new System.Drawing.Size(56, 17);
            this.lbPalette.TabIndex = 39;
            this.lbPalette.Text = "Palette:";
            this.lbPalette.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtState
            // 
            this.txtState.BackColor = System.Drawing.SystemColors.Control;
            this.txtState.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtState.Location = new System.Drawing.Point(55, 215);
            this.txtState.Name = "txtState";
            this.txtState.ReadOnly = true;
            this.txtState.Size = new System.Drawing.Size(44, 15);
            this.txtState.TabIndex = 38;
            this.txtState.TabStop = false;
            this.txtState.WordWrap = false;
            // 
            // lbState
            // 
            this.lbState.AutoSize = true;
            this.lbState.Location = new System.Drawing.Point(12, 213);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(45, 17);
            this.lbState.TabIndex = 37;
            this.lbState.Text = "State:";
            this.lbState.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtParam
            // 
            this.txtParam.BackColor = System.Drawing.SystemColors.Control;
            this.txtParam.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtParam.Location = new System.Drawing.Point(55, 197);
            this.txtParam.Name = "txtParam";
            this.txtParam.ReadOnly = true;
            this.txtParam.Size = new System.Drawing.Size(44, 15);
            this.txtParam.TabIndex = 36;
            this.txtParam.TabStop = false;
            this.txtParam.WordWrap = false;
            // 
            // txtID
            // 
            this.txtID.BackColor = System.Drawing.SystemColors.Control;
            this.txtID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtID.Location = new System.Drawing.Point(189, 15);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(44, 15);
            this.txtID.TabIndex = 35;
            this.txtID.TabStop = false;
            this.txtID.WordWrap = false;
            // 
            // lbID
            // 
            this.lbID.AutoSize = true;
            this.lbID.Location = new System.Drawing.Point(163, 13);
            this.lbID.Name = "lbID";
            this.lbID.Size = new System.Drawing.Size(25, 17);
            this.lbID.TabIndex = 34;
            this.lbID.Text = "ID:";
            this.lbID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbParam
            // 
            this.lbParam.AutoSize = true;
            this.lbParam.Location = new System.Drawing.Point(4, 195);
            this.lbParam.Name = "lbParam";
            this.lbParam.Size = new System.Drawing.Size(53, 17);
            this.lbParam.TabIndex = 33;
            this.lbParam.Text = "Param:";
            this.lbParam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbBlending
            // 
            this.cbBlending.AutoCheck = false;
            this.cbBlending.AutoSize = true;
            this.cbBlending.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cbBlending.Location = new System.Drawing.Point(6, 153);
            this.cbBlending.Name = "cbBlending";
            this.cbBlending.Size = new System.Drawing.Size(85, 21);
            this.cbBlending.TabIndex = 32;
            this.cbBlending.Text = "Blending";
            this.cbBlending.UseVisualStyleBackColor = true;
            // 
            // txtDestY
            // 
            this.txtDestY.BackColor = System.Drawing.SystemColors.Control;
            this.txtDestY.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDestY.Location = new System.Drawing.Point(211, 108);
            this.txtDestY.Name = "txtDestY";
            this.txtDestY.ReadOnly = true;
            this.txtDestY.Size = new System.Drawing.Size(44, 15);
            this.txtDestY.TabIndex = 31;
            this.txtDestY.TabStop = false;
            this.txtDestY.WordWrap = false;
            // 
            // txtDestX
            // 
            this.txtDestX.BackColor = System.Drawing.SystemColors.Control;
            this.txtDestX.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDestX.Location = new System.Drawing.Point(211, 91);
            this.txtDestX.Name = "txtDestX";
            this.txtDestX.ReadOnly = true;
            this.txtDestX.Size = new System.Drawing.Size(44, 15);
            this.txtDestX.TabIndex = 30;
            this.txtDestX.TabStop = false;
            this.txtDestX.WordWrap = false;
            // 
            // lbDestY
            // 
            this.lbDestY.AutoSize = true;
            this.lbDestY.Location = new System.Drawing.Point(163, 106);
            this.lbDestY.Name = "lbDestY";
            this.lbDestY.Size = new System.Drawing.Size(50, 17);
            this.lbDestY.TabIndex = 29;
            this.lbDestY.Text = "DestY:";
            this.lbDestY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbDestX
            // 
            this.lbDestX.AutoSize = true;
            this.lbDestX.Location = new System.Drawing.Point(163, 89);
            this.lbDestX.Name = "lbDestX";
            this.lbDestX.Size = new System.Drawing.Size(50, 17);
            this.lbDestX.TabIndex = 28;
            this.lbDestX.Text = "DestX:";
            this.lbDestX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTileSize
            // 
            this.txtTileSize.BackColor = System.Drawing.SystemColors.Control;
            this.txtTileSize.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTileSize.Location = new System.Drawing.Point(189, 53);
            this.txtTileSize.Name = "txtTileSize";
            this.txtTileSize.ReadOnly = true;
            this.txtTileSize.Size = new System.Drawing.Size(34, 15);
            this.txtTileSize.TabIndex = 27;
            this.txtTileSize.TabStop = false;
            this.txtTileSize.WordWrap = false;
            // 
            // lbSize
            // 
            this.lbSize.AutoSize = true;
            this.lbSize.Location = new System.Drawing.Point(149, 51);
            this.lbSize.Name = "lbSize";
            this.lbSize.Size = new System.Drawing.Size(39, 17);
            this.lbSize.TabIndex = 26;
            this.lbSize.Text = "Size:";
            this.lbSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSrcY
            // 
            this.txtSrcY.BackColor = System.Drawing.SystemColors.Control;
            this.txtSrcY.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSrcY.Location = new System.Drawing.Point(211, 162);
            this.txtSrcY.Name = "txtSrcY";
            this.txtSrcY.ReadOnly = true;
            this.txtSrcY.Size = new System.Drawing.Size(44, 15);
            this.txtSrcY.TabIndex = 25;
            this.txtSrcY.TabStop = false;
            this.txtSrcY.WordWrap = false;
            // 
            // txtSrcX
            // 
            this.txtSrcX.BackColor = System.Drawing.SystemColors.Control;
            this.txtSrcX.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSrcX.Location = new System.Drawing.Point(211, 145);
            this.txtSrcX.Name = "txtSrcX";
            this.txtSrcX.ReadOnly = true;
            this.txtSrcX.Size = new System.Drawing.Size(44, 15);
            this.txtSrcX.TabIndex = 24;
            this.txtSrcX.TabStop = false;
            this.txtSrcX.WordWrap = false;
            // 
            // txtTile
            // 
            this.txtTile.BackColor = System.Drawing.SystemColors.Control;
            this.txtTile.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTile.Location = new System.Drawing.Point(60, 41);
            this.txtTile.Name = "txtTile";
            this.txtTile.ReadOnly = true;
            this.txtTile.Size = new System.Drawing.Size(44, 15);
            this.txtTile.TabIndex = 23;
            this.txtTile.TabStop = false;
            this.txtTile.WordWrap = false;
            // 
            // txtLayer
            // 
            this.txtLayer.BackColor = System.Drawing.SystemColors.Control;
            this.txtLayer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLayer.Location = new System.Drawing.Point(60, 22);
            this.txtLayer.Name = "txtLayer";
            this.txtLayer.ReadOnly = true;
            this.txtLayer.Size = new System.Drawing.Size(18, 15);
            this.txtLayer.TabIndex = 22;
            this.txtLayer.TabStop = false;
            this.txtLayer.WordWrap = false;
            // 
            // lbSrcY
            // 
            this.lbSrcY.AutoSize = true;
            this.lbSrcY.Location = new System.Drawing.Point(171, 160);
            this.lbSrcY.Name = "lbSrcY";
            this.lbSrcY.Size = new System.Drawing.Size(42, 17);
            this.lbSrcY.TabIndex = 21;
            this.lbSrcY.Text = "SrcY:";
            this.lbSrcY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbSrcX
            // 
            this.lbSrcX.AutoSize = true;
            this.lbSrcX.Location = new System.Drawing.Point(171, 143);
            this.lbSrcX.Name = "lbSrcX";
            this.lbSrcX.Size = new System.Drawing.Size(42, 17);
            this.lbSrcX.TabIndex = 20;
            this.lbSrcX.Text = "SrcX:";
            this.lbSrcX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(10, 20);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(48, 17);
            this.Label1.TabIndex = 18;
            this.Label1.Text = "Layer:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbLayer
            // 
            this.lbLayer.AutoSize = true;
            this.lbLayer.Location = new System.Drawing.Point(10, 20);
            this.lbLayer.Name = "lbLayer";
            this.lbLayer.Size = new System.Drawing.Size(48, 17);
            this.lbLayer.TabIndex = 18;
            this.lbLayer.Text = "Layer:";
            this.lbLayer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnTileLeft
            // 
            this.btnTileLeft.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTileLeft.Location = new System.Drawing.Point(9, 65);
            this.btnTileLeft.Name = "btnTileLeft";
            this.btnTileLeft.Size = new System.Drawing.Size(20, 82);
            this.btnTileLeft.TabIndex = 17;
            this.btnTileLeft.Text = "◄";
            this.btnTileLeft.UseVisualStyleBackColor = true;
            this.btnTileLeft.Click += new System.EventHandler(this.btnTileLeft_Click);
            // 
            // btnTileRight
            // 
            this.btnTileRight.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTileRight.Location = new System.Drawing.Point(112, 65);
            this.btnTileRight.Name = "btnTileRight";
            this.btnTileRight.Size = new System.Drawing.Size(20, 81);
            this.btnTileRight.TabIndex = 16;
            this.btnTileRight.Text = "►";
            this.btnTileRight.UseVisualStyleBackColor = true;
            this.btnTileRight.Click += new System.EventHandler(this.btnTileRight_Click);
            // 
            // panelpbTexture
            // 
            this.panelpbTexture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelpbTexture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelpbTexture.Controls.Add(this.pbTexture);
            this.panelpbTexture.Location = new System.Drawing.Point(6, 354);
            this.panelpbTexture.Name = "panelpbTexture";
            this.panelpbTexture.Size = new System.Drawing.Size(261, 261);
            this.panelpbTexture.TabIndex = 24;
            // 
            // pbTexture
            // 
            this.pbTexture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbTexture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbTexture.Location = new System.Drawing.Point(0, 0);
            this.pbTexture.Name = "pbTexture";
            this.pbTexture.Size = new System.Drawing.Size(256, 256);
            this.pbTexture.TabIndex = 6;
            this.pbTexture.TabStop = false;
            this.pbTexture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbTexture_MouseDown);
            this.pbTexture.MouseLeave += new System.EventHandler(this.pbTexture_MouseLeave);
            this.pbTexture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbTexture_MouseMove);
            // 
            // panelpbBackground
            // 
            this.panelpbBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelpbBackground.AutoScroll = true;
            this.panelpbBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelpbBackground.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelpbBackground.Controls.Add(this.pbBackground);
            this.panelpbBackground.Location = new System.Drawing.Point(271, 33);
            this.panelpbBackground.Name = "panelpbBackground";
            this.panelpbBackground.Size = new System.Drawing.Size(891, 817);
            this.panelpbBackground.TabIndex = 23;
            // 
            // pbBackground
            // 
            this.pbBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbBackground.Location = new System.Drawing.Point(0, 0);
            this.pbBackground.Name = "pbBackground";
            this.pbBackground.Size = new System.Drawing.Size(885, 811);
            this.pbBackground.TabIndex = 5;
            this.pbBackground.TabStop = false;
            this.pbBackground.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbBackground_MouseDown);
            this.pbBackground.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbBackground_MouseMove);
            this.pbBackground.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbBackground_MouseUp);
            // 
            // rtbEvents
            // 
            this.rtbEvents.BackColor = System.Drawing.SystemColors.Window;
            this.rtbEvents.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rtbEvents.Font = new System.Drawing.Font("Consolas", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbEvents.Location = new System.Drawing.Point(0, 855);
            this.rtbEvents.Name = "rtbEvents";
            this.rtbEvents.ReadOnly = true;
            this.rtbEvents.Size = new System.Drawing.Size(1361, 99);
            this.rtbEvents.TabIndex = 22;
            this.rtbEvents.Text = "";
            this.rtbEvents.WordWrap = false;
            // 
            // tcParams
            // 
            this.tcParams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tcParams.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tcParams.Enabled = false;
            this.tcParams.ItemSize = new System.Drawing.Size(28, 64);
            this.tcParams.Location = new System.Drawing.Point(1172, 130);
            this.tcParams.Name = "tcParams";
            this.tcParams.SelectedIndex = 0;
            this.tcParams.Size = new System.Drawing.Size(180, 216);
            this.tcParams.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tcParams.TabIndex = 21;
            this.tcParams.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tcParams_DrawItem);
            // 
            // gbLayers
            // 
            this.gbLayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbLayers.Controls.Add(this.cbLayer3);
            this.gbLayers.Controls.Add(this.cbLayer2);
            this.gbLayers.Controls.Add(this.cbLayer1);
            this.gbLayers.Controls.Add(this.cbLayer0);
            this.gbLayers.Enabled = false;
            this.gbLayers.Location = new System.Drawing.Point(1168, 31);
            this.gbLayers.Name = "gbLayers";
            this.gbLayers.Size = new System.Drawing.Size(186, 92);
            this.gbLayers.TabIndex = 20;
            this.gbLayers.TabStop = false;
            this.gbLayers.Text = "Layers";
            // 
            // cbLayer3
            // 
            this.cbLayer3.AutoSize = true;
            this.cbLayer3.Location = new System.Drawing.Point(103, 61);
            this.cbLayer3.Name = "cbLayer3";
            this.cbLayer3.Size = new System.Drawing.Size(78, 21);
            this.cbLayer3.TabIndex = 3;
            this.cbLayer3.Text = "Layer 3";
            this.cbLayer3.UseVisualStyleBackColor = true;
            this.cbLayer3.CheckedChanged += new System.EventHandler(this.cbLayer3_CheckedChanged);
            // 
            // cbLayer2
            // 
            this.cbLayer2.AutoSize = true;
            this.cbLayer2.Location = new System.Drawing.Point(10, 61);
            this.cbLayer2.Name = "cbLayer2";
            this.cbLayer2.Size = new System.Drawing.Size(78, 21);
            this.cbLayer2.TabIndex = 2;
            this.cbLayer2.Text = "Layer 2";
            this.cbLayer2.UseVisualStyleBackColor = true;
            this.cbLayer2.CheckedChanged += new System.EventHandler(this.cbLayer2_CheckedChanged);
            // 
            // cbLayer1
            // 
            this.cbLayer1.AutoSize = true;
            this.cbLayer1.Location = new System.Drawing.Point(103, 30);
            this.cbLayer1.Name = "cbLayer1";
            this.cbLayer1.Size = new System.Drawing.Size(78, 21);
            this.cbLayer1.TabIndex = 1;
            this.cbLayer1.Text = "Layer 1";
            this.cbLayer1.UseVisualStyleBackColor = true;
            this.cbLayer1.CheckedChanged += new System.EventHandler(this.cbLayer1_CheckedChanged);
            // 
            // cbLayer0
            // 
            this.cbLayer0.AutoSize = true;
            this.cbLayer0.Location = new System.Drawing.Point(10, 30);
            this.cbLayer0.Name = "cbLayer0";
            this.cbLayer0.Size = new System.Drawing.Size(78, 21);
            this.cbLayer0.TabIndex = 0;
            this.cbLayer0.Text = "Layer 0";
            this.cbLayer0.UseVisualStyleBackColor = true;
            this.cbLayer0.CheckedChanged += new System.EventHandler(this.cbLayer0_CheckedChanged);
            // 
            // cbTextures
            // 
            this.cbTextures.Enabled = false;
            this.cbTextures.FormattingEnabled = true;
            this.cbTextures.Location = new System.Drawing.Point(7, 328);
            this.cbTextures.MaxDropDownItems = 16;
            this.cbTextures.Name = "cbTextures";
            this.cbTextures.Size = new System.Drawing.Size(259, 24);
            this.cbTextures.TabIndex = 19;
            this.cbTextures.SelectedIndexChanged += new System.EventHandler(this.cbTextures_SelectedIndexChanged);
            // 
            // cbPalettes
            // 
            this.cbPalettes.Enabled = false;
            this.cbPalettes.FormattingEnabled = true;
            this.cbPalettes.Location = new System.Drawing.Point(7, 33);
            this.cbPalettes.MaxDropDownItems = 16;
            this.cbPalettes.Name = "cbPalettes";
            this.cbPalettes.Size = new System.Drawing.Size(259, 24);
            this.cbPalettes.TabIndex = 18;
            this.cbPalettes.SelectedIndexChanged += new System.EventHandler(this.cbPalettes_SelectedIndexChanged);
            // 
            // pbPalette
            // 
            this.pbPalette.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbPalette.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbPalette.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pbPalette.Location = new System.Drawing.Point(6, 59);
            this.pbPalette.Name = "pbPalette";
            this.pbPalette.Size = new System.Drawing.Size(261, 261);
            this.pbPalette.TabIndex = 17;
            this.pbPalette.TabStop = false;
            this.pbPalette.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPalette_Paint);
            this.pbPalette.MouseLeave += new System.EventHandler(this.pbPalette_MouseLeave);
            this.pbPalette.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbPalette_MouseMove);
            // 
            // frmAeris
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1361, 954);
            this.Controls.Add(this.gbSublayers);
            this.Controls.Add(this.gbTile);
            this.Controls.Add(this.panelpbTexture);
            this.Controls.Add(this.panelpbBackground);
            this.Controls.Add(this.rtbEvents);
            this.Controls.Add(this.tcParams);
            this.Controls.Add(this.gbLayers);
            this.Controls.Add(this.cbTextures);
            this.Controls.Add(this.cbPalettes);
            this.Controls.Add(this.pbPalette);
            this.Controls.Add(this.MenuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip1;
            this.MinimumSize = new System.Drawing.Size(1375, 1001);
            this.Name = "frmAeris";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Aeris";
            this.Load += new System.EventHandler(this.frmAeris_Load);
            this.Resize += new System.EventHandler(this.frmAeris_Resize);
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.gbSublayers.ResumeLayout(false);
            this.gbTile.ResumeLayout(false);
            this.gbTile.PerformLayout();
            this.panelpbTile.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbTile)).EndInit();
            this.panelpbTexture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbTexture)).EndInit();
            this.panelpbBackground.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbBackground)).EndInit();
            this.gbLayers.ResumeLayout(false);
            this.gbLayers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPalette)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.MenuStrip MenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
        internal System.Windows.Forms.ToolStripMenuItem Repairfr_eToolStripMenuItem2;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem SaveFieldToolStripMenuItem2;
        internal System.Windows.Forms.ToolStripMenuItem SaveFieldAsToolStripMenuItem2;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator8;
        internal System.Windows.Forms.ToolStripMenuItem CloseToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem PaletteToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ExportToToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator10;
        internal System.Windows.Forms.ToolStripMenuItem ImportFromToolStripMenuItem2;
        internal System.Windows.Forms.ToolStripMenuItem TextureToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem PreviewTextureToolStripMenuItem1;
        internal System.Windows.Forms.ToolStripMenuItem UnswizzleInternalToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator6;
        internal System.Windows.Forms.ToolStripMenuItem UnswizzleExternalToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SwizzleExternalToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
        internal System.Windows.Forms.ToolStripMenuItem UnswizzleHashedBatchToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SwizzleHashedBatchToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator5;
        internal System.Windows.Forms.ToolStripMenuItem ExportToPNGToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ExportAllToPNGToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator9;
        internal System.Windows.Forms.ToolStripMenuItem ImportTextureToolStripMenuItem2;
        internal System.Windows.Forms.ToolStripMenuItem StageToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem BasePreviewToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator7;
        internal System.Windows.Forms.ToolStripMenuItem FillBackgroundToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem BlackStageToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem BlackImageToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ZoomToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem AutoFillToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem X2ToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem X4ToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem RenderToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem MarkTileBackgroundToolStripMenuItem2;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
        internal System.Windows.Forms.ToolStripMenuItem UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SwizzleAllExportedBaseToPNGToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem EventsLogToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ActivateLoggingToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SaveEventsAsToolStripMenuItem1;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem ClearToolStripMenuItem;
        internal System.Windows.Forms.GroupBox gbSublayers;
        internal System.Windows.Forms.PictureBox pbTile;
        internal System.Windows.Forms.Panel panelpbTexture;
        internal System.Windows.Forms.Panel panelpbBackground;
        internal System.Windows.Forms.GroupBox gbLayers;
        internal System.Windows.Forms.SaveFileDialog saveFile;
        internal System.Windows.Forms.OpenFileDialog openFile;
        public System.Windows.Forms.Button btnUncheckAll;
        public System.Windows.Forms.Button btnCheckAll;
        public System.Windows.Forms.CheckedListBox clbSublayers;
        public System.Windows.Forms.GroupBox gbTile;
        public System.Windows.Forms.TextBox txtBlendMode;
        public System.Windows.Forms.Label lbTransp;
        public System.Windows.Forms.Label lbTile;
        public System.Windows.Forms.TextBox txtBigID;
        public System.Windows.Forms.Label lbBigID;
        public System.Windows.Forms.Panel panelpbTile;
        public System.Windows.Forms.CheckBox cbIFX;
        public System.Windows.Forms.TextBox txtTileTex;
        public System.Windows.Forms.TextBox txtSrcY2;
        public System.Windows.Forms.TextBox txtSrcX2;
        public System.Windows.Forms.Label lbSrcY2;
        public System.Windows.Forms.Label lbSrcX2;
        public System.Windows.Forms.TextBox txtTexture2;
        public System.Windows.Forms.Label lbTexture2;
        public System.Windows.Forms.TextBox txtTexture;
        public System.Windows.Forms.Label lbTX;
        public System.Windows.Forms.TextBox txtPalette;
        public System.Windows.Forms.Label lbPalette;
        public System.Windows.Forms.TextBox txtState;
        public System.Windows.Forms.Label lbState;
        public System.Windows.Forms.TextBox txtParam;
        public System.Windows.Forms.TextBox txtID;
        public System.Windows.Forms.Label lbID;
        public System.Windows.Forms.Label lbParam;
        public System.Windows.Forms.CheckBox cbBlending;
        public System.Windows.Forms.TextBox txtDestY;
        public System.Windows.Forms.TextBox txtDestX;
        public System.Windows.Forms.Label lbDestY;
        public System.Windows.Forms.Label lbDestX;
        public System.Windows.Forms.TextBox txtTileSize;
        public System.Windows.Forms.Label lbSize;
        public System.Windows.Forms.TextBox txtSrcY;
        public System.Windows.Forms.TextBox txtSrcX;
        public System.Windows.Forms.TextBox txtTile;
        public System.Windows.Forms.TextBox txtLayer;
        public System.Windows.Forms.Label lbSrcY;
        public System.Windows.Forms.Label lbSrcX;
        public System.Windows.Forms.Label Label1;
        public System.Windows.Forms.Label lbLayer;
        public System.Windows.Forms.Button btnTileLeft;
        public System.Windows.Forms.Button btnTileRight;
        public System.Windows.Forms.PictureBox pbTexture;
        public System.Windows.Forms.PictureBox pbBackground;
        public System.Windows.Forms.RichTextBox rtbEvents;
        public System.Windows.Forms.TabControl tcParams;
        public System.Windows.Forms.CheckBox cbLayer3;
        public System.Windows.Forms.CheckBox cbLayer2;
        public System.Windows.Forms.CheckBox cbLayer1;
        public System.Windows.Forms.CheckBox cbLayer0;
        public System.Windows.Forms.PictureBox pbPalette;
        public System.Windows.Forms.ComboBox cbTextures;
        public System.Windows.Forms.ComboBox cbPalettes;
    }
}

