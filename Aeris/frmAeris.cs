using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace Aeris
{
    public partial class frmAeris : Form
    {
        private frmTileEditor frmTileEditor;
        private frmBasePreview frmBasePreview;
        private frmTextureImage frmTextureImage;
        private frmSwizzleExternalHashImage frmSwizzleExternalHashImage;
        private frmSwizzleExternalBaseImages frmSwizzleExternalBaseImages;
        private frmSwizzleHashesBatch frmSwizzleHashesBatch;
        private frmUnswizzleExternalHashTextures frmUnswizzleExternalHashTextures;
        private frmUnswizzleHashesBatch frmUnswizzleHashesBatch;

        public frmAeris()
        {
            InitializeComponent();

            frmTileEditor = new frmTileEditor(this);
            frmBasePreview = new frmBasePreview(this);
            frmTextureImage = new frmTextureImage(this);
            frmSwizzleExternalHashImage = new frmSwizzleExternalHashImage(this);
            frmSwizzleExternalBaseImages = new frmSwizzleExternalBaseImages(this);
            frmSwizzleHashesBatch = new frmSwizzleHashesBatch(this);
            frmUnswizzleExternalHashTextures = new frmUnswizzleExternalHashTextures(this);
            frmUnswizzleHashesBatch = new frmUnswizzleHashesBatch(this);
        }


        // Bools for use in modules depending on the checked menu options.
        public static bool Initializing;
        public static bool LoadNewField;
        public static int iBGZoom;
        public static CheckedListBox newCLB;
        public static TabPage newTab;

        // Public bmpPALInfoColor As Bitmap = New Bitmap(56, 50)
        public static Bitmap bmpPALInfoColor = new Bitmap(68, 66);
        public static Rectangle rectPALInfoColor;
        public static bool bPALInfoColorMouseMove;
        public static bool bMarkTileBackground;


        private void frmAeris_Load(object sender, EventArgs e)
        {
            // Load field IDs info.
            FieldIDs.PopulateFields();

            // Prepare Horizontal/Vertical Resolution for DPI Aware program
            // and work with Bitmaps.
            Graphics g = CreateGraphics();
            ImageTools.HORZ_RES = g.DpiX;
            ImageTools.VERT_RES = g.DpiY;
            g.Dispose();

            // Indicate the first time Background Zoom
            iBGZoom = 1;

            // Put new cursor in Palette PictureBox
            ImageTools.LoadCursors();

            pbPalette.Cursor = ImageTools.CrossCUR;
            pbTexture.Cursor = ImageTools.HandCUR;
            pbTile.Cursor = ImageTools.EditCUR;
            pbBackground.Cursor = ImageTools.MoveCUR;

            bMarkTileBackground = false;
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Repairfr_eToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int iResult;

            DialogResult mbButtons;
            mbButtons = MessageBox.Show("NOTE:\n\r\n\r" +
                                        "The main purpose of this fix is to add the shadow of Diamond Weapon " +
                                        "in the vanilla field 'fr_e' in FF7 PC port. This fix changes some things:" +
                                        "\n\r\n\r * Changes the palettes of the tiles of Layer 3 to use the same palette." +
                                        "\n\r * Changes the image of the shadow of Layer 3 using external images." +
                                        "\n\r * Changes Section 0/1 script to make work the shadow correctly." +
                                        "\n\r\n\rWARNING: If you want to fix a modded 'fr_e' field, it will OVERWRITE " +
                                        "ALL THE SCRIPT SECTION." +
                                        "\n\r\n\rDo you want to change it anyway?",
                                        "Information",
                                        MessageBoxButtons.YesNo);

            if (mbButtons == DialogResult.Yes)
            {

                iResult = Repair_fr_e.Repair_Brokenfr_e();

                if (iResult == 0)
                {
                    MessageBox.Show("DONE: Field 'fr_e' repaired.",
                                    "Information",
                                    MessageBoxButtons.OK);

                    // Finally we need to update/refresh the values in the loaded field.
                    S9.PreRender_S9Tiles();
                    S9.Load_ZList(rtbEvents);

                    cbTextures.SelectedIndex = 26;

                    RefreshBackground(cbTextures.SelectedIndex);
                }
                else if (iResult == 1)
                {
                    MessageBox.Show("ERROR: Some of the files for apply the fix in the field does not exist.",
                                    "Information",
                                    MessageBoxButtons.OK);
                }
                else if (iResult == 2)
                {
                    MessageBox.Show("INFO: This field is different from the vanilla one. " +
                                    "The fix can not be applied to the opened field.",
                                    "Information",
                                    MessageBoxButtons.OK);
                }
                else if (iResult == 4)
                {
                    MessageBox.Show("One of the Tiles of the selected texture has assigned a non " +
                                    "existant PaletteID. This is NOT the original vanilla 'fr_e' field.",
                                    "Warning");
                }
                else
                {
                    MessageBox.Show("Error trying to repair Field 'fr_e'.",
                                    "Warning",
                                    MessageBoxButtons.OK);
                }

                Repairfr_eToolStripMenuItem2.Enabled = false;
            }
        }

        public void Initialize()
        {
            // Let's tell the tool that it is initializing.
            Initializing = true;

            // Main form useful things
            // Init Layer things
            gbLayers.Enabled = false;

            // Init ComboBoxes
            cbLayer0.Checked = false;
            cbLayer1.Checked = false;
            cbLayer2.Checked = false;
            cbLayer3.Checked = false;
            S9.bLayerChecked0 = false;
            S9.bLayerChecked1 = false;
            S9.bLayerChecked2 = false;
            S9.bLayerChecked3 = false;
            cbPalettes.Items.Clear();
            cbPalettes.SelectedIndex = -1;
            cbPalettes.Enabled = false;
            cbTextures.Items.Clear();
            cbTextures.SelectedIndex = -1;
            cbTextures.Enabled = false;

            // Init PictureBoxes
            ImageTools.ClearPictureBox(pbBackground, 0, panelpbBackground);
            ImageTools.ClearPictureBox(pbPalette, 1, null);
            ImageTools.ClearPictureBox(pbTexture, 2, panelpbTexture);
            ImageTools.ClearPictureBox(pbTile, 2, panelpbTile);

            // Init Tile Section
            txtLayer.Text = "";
            txtTile.Text = "";
            txtID.Text = "";
            txtBigID.Text = "";
            txtTileSize.Text = "";
            txtBlendMode.Text = "";
            txtParam.Text = "";
            txtState.Text = "";
            txtPalette.Text = "";
            txtDestX.Text = "";
            txtDestY.Text = "";
            txtTexture.Text = "";
            txtSrcX.Text = "";
            txtSrcY.Text = "";
            txtTexture2.Text = "";
            txtSrcX2.Text = "";
            txtSrcY2.Text = "";
            cbBlending.Checked = false;
            cbIFX.Checked = false;
            btnTileRight.Enabled = false;
            btnTileLeft.Enabled = false;
            tcParams.Enabled = false;
            gbSublayers.Enabled = false;


            // MenuOptions
            PaletteToolStripMenuItem.Enabled = false;
            TextureToolStripMenuItem.Enabled = false;
            StageToolStripMenuItem.Enabled = false;
            SaveFieldToolStripMenuItem2.Enabled = false;
            SaveFieldAsToolStripMenuItem2.Enabled = false;


            // Logging things
            logEvents.ClearEvents(rtbEvents);


            // Initialize Vars
            // Clear List of Params/States
            S9.Clear_ListParams();
            S9.MaxSublayers = 0;


            // Clear textureImages.
            S9.Clear_TextureImages();

            // ZList
            S9.Section9Z.Clear();

            // TileSeparation
            SwizzleHash.lstTileSeparation.Clear();
            Initializing = false;
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int iLoadResult;

            // Set filter options and filter index.
            openFile.Title = "Open PC Uncompressed Field";
            openFile.Filter = "All Files (*.*)|*.*";
            openFile.FilterIndex = 1;
            openFile.FileName = null;

            // Check Initial Directory
            if (FileTools.strGlobalPathFieldFolder != null)
            {
                openFile.InitialDirectory = FileTools.strGlobalPathFieldFolder;
            }
            else
            {
                openFile.InitialDirectory = FileTools.strGlobalPath;
            }

            try
            {
                // Process input if the user clicked OK.
                if (openFile.ShowDialog(this) == DialogResult.OK)
                {
                    if (File.Exists(openFile.FileName))
                    {
                        FileTools.strFileFieldName = Path.GetFileName(openFile.FileName);
                        FileTools.strGlobalFieldName = Path.GetFileNameWithoutExtension(FileTools.strFileFieldName);

                        // Initialize things.
                        Initialize();


                        // We load the field.
                        iLoadResult = FileTools.Load_Field(openFile.FileName);
                        if (iLoadResult == -1)
                        {
                            MessageBox.Show("Error opening Field file '" + FileTools.strFileFieldName + "'.",
                                            "Error");
                            return;
                        }

                        // Load TileSeparation File if there is any.
                        // The unique field until now that need this is 'blue_2'.
                        if (File.Exists(FileTools.strGlobalPath + "\\tileseparation\\" +
                                        FileTools.strGlobalFieldName + ".txt"))
                        {
                            SwizzleHash.Read_TileSeparation(FileTools.strGlobalPath + "\\tileseparation\\" +
                                                               FileTools.strGlobalFieldName + ".txt");
                        }


                        // Prepare Param/Status Tab
                        Prepare_ParamStatus();


                        // Prepare Palettes things.
                        Prepare_Palettes();

                        cbPalettes.Enabled = true;
                        if (cbPalettes.Items.Count > 0)
                        {
                            cbPalettes.SelectedIndex = 0;
                        }


                        // Activate some Layer things.
                        gbLayers.Enabled = true;
                        if (S9.Section9.Layer[1].layerFlag == 1 &&
                            S9.Section9.Layer[1].numTiles > 0)
                        {
                            cbLayer1.Enabled = true;
                        }
                        else
                        {
                            cbLayer1.Enabled = false;
                        }

                        if (S9.Section9.Layer[2].layerFlag == 1 &&
                            S9.Section9.Layer[2].numTiles > 0)
                        {
                            cbLayer2.Enabled = true;
                        }
                        else
                        {
                            cbLayer2.Enabled = false;
                        }

                        if (S9.Section9.Layer[3].layerFlag == 1 &&
                            S9.Section9.Layer[3].numTiles > 0)
                        {
                            cbLayer3.Enabled = true;
                        }
                        else
                        {
                            cbLayer3.Enabled = false;
                        }


                        // Prepare ZList.
                        S9.Load_ZList(rtbEvents);

                        // Prepare Sections
                        if (S9.MaxSublayers > 0)
                        {
                            Prepare_Sublayers();
                        }


                        // Prepare Texture things.
                        Prepare_Textures();

                        cbTextures.Enabled = true;
                        if (cbTextures.Items.Count > 0)
                        {
                            cbTextures.SelectedIndex = S9.Section9Z[0].ZTexture;
                        }


                        // Activate Tile things.
                        btnTileRight.Enabled = true;
                        btnTileLeft.Enabled = true;


                        // Activate Render things if bRenderEffects is active.
                        if (ImageTools.bRenderEffects)
                        {
                            tcParams.Enabled = true;
                        }


                        // Render Section 9 Initial Background, Layer 0 (or nothing if it has not Layer 0?).
                        cbLayer0.Checked = true;

                        // Activate Menu and Window Items
                        PaletteToolStripMenuItem.Enabled = true;
                        TextureToolStripMenuItem.Enabled = true;
                        StageToolStripMenuItem.Enabled = true;
                        SaveFieldAsToolStripMenuItem2.Enabled = true;
                        SaveFieldToolStripMenuItem2.Enabled = true;
                        Repairfr_eToolStripMenuItem2.Enabled = false;

                        // Activate Menu Item for Repair Field fr_e
                        if (string.Compare(FileTools.strGlobalFieldName, FieldIDs._FIELD_FR_E) == 0)
                        {
                            if (S9.Section9.Textures[26].textureFlag == 1)
                            {
                                Repairfr_eToolStripMenuItem2.Enabled = true;
                            }
                        }


                        // Initialize Title
                        FileTools.bFieldModified = false;
                        Update_AerisTitle();

                        // Set Global Path for Fields
                        FileTools.strGlobalPathFieldFolder = Path.GetDirectoryName(openFile.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error preparing data of the Field for Aeris.", "Error");
            }
        }

        public void Update_AerisTitle()
        {
            Text = "Field: (" + FileTools.strGlobalFieldName + " | " +
                   FieldIDs.stFieldIDNameList.Find(x => x.FieldName.Contains(FileTools.strGlobalFieldName)).FieldID.ToString("000") + 
                   ") - Aeris";

            if (FileTools.bFieldModified)
            {
                Text = Text + " *";
            }
        }

        private void cbPalettes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPalettes.Items.Count > 0 & cbPalettes.SelectedIndex > -1)
            {
                Palette.Refresh_Palette(ref pbPalette, cbPalettes.SelectedIndex);
            }
        }

        private void cbTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTextures.Items.Count > 0 & cbTextures.SelectedIndex > -1)
            {
                if (S9.textureImage[cbTextures.SelectedIndex] != null)
                {
                    Render_Texture();
                    txtTileTex.Text = "0";
                    SetpbTile(0, cbTextures.SelectedIndex);
                    gbTile.Enabled = true;
                }
                else
                {
                    ImageTools.ClearPictureBox(pbTexture, 1, panelpbTexture);
                    ImageTools.ClearPictureBox(pbTile, 2, null);
                    gbTile.Enabled = false;
                }
            }
        }

        public void Render_Layers()
        {
            ImageTools.ClearPictureBox(pbBackground, 0, panelpbBackground);
            logEvents.ClearEvents(rtbEvents);

            S9.Render_S9Layers(this);

            if (cbLayer0.Checked | cbLayer1.Checked | cbLayer2.Checked | cbLayer3.Checked)
            {
                ApplyZoomBG();
            }
        }

        public void Render_Texture()
        {

            // ClearPictureBox(pbTexture, 1, panelpbTexture)

            if (cbTextures.Items.Count > 0)
            {
                if (S9.textureImage[cbTextures.SelectedIndex] != null)
                {
                    if (pbTexture.Image != null)
                    {
                        pbTexture.Image.Dispose();
                    }

                    pbTexture.Image = new Bitmap(S9.textureImage[cbTextures.SelectedIndex].Bitmap);
                }
            }
        }

        private void cbLayer0_CheckedChanged(object sender, EventArgs e)
        {
            S9.bLayerChecked0 = !S9.bLayerChecked0;

            if (Initializing) return;

            Render_Layers();
        }

        private void cbLayer1_CheckedChanged(object sender, EventArgs e)
        {
            if (Initializing) return;

            S9.bLayerChecked1 = !S9.bLayerChecked1;

            if (cbLayer1.Checked)
            {
                gbSublayers.Enabled = true;
            }
            else
            {
                gbSublayers.Enabled = false;
            }

            Render_Layers();
        }

        private void cbLayer2_CheckedChanged(object sender, EventArgs e)
        {
            S9.bLayerChecked2 = !S9.bLayerChecked2;

            if (Initializing) return;

            Render_Layers();
        }

        private void cbLayer3_CheckedChanged(object sender, EventArgs e)
        {
            S9.bLayerChecked3 = !S9.bLayerChecked3;

            if (Initializing) return;

            Render_Layers();
        }

        private void RenderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderToolStripMenuItem.Checked)
            {
                ImageTools.bRenderEffects = true;
                tcParams.Enabled = true;
            }
            else
            {
                ImageTools.bRenderEffects = false;
                tcParams.Enabled = false;
            }

            Render_Layers();
        }

        private void BlackImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BlackStageToolStripMenuItem.Checked = false;

            if (BlackImageToolStripMenuItem.Checked)
            {
                ImageTools.FillBGTransparent = false;
                ImageTools.FillBGBlackStage = true;
                ImageTools.FillBGBlackImage = false;
            }
            else
            {
                ImageTools.FillBGTransparent = true;
                ImageTools.FillBGBlackStage = false;
                ImageTools.FillBGBlackImage = false;
            }

            Render_Layers();
        }

        private void BlackStageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BlackImageToolStripMenuItem.Checked = false;

            if (BlackStageToolStripMenuItem.Checked)
            {
                ImageTools.FillBGTransparent = false;
                ImageTools.FillBGBlackStage = false;
                ImageTools.FillBGBlackImage = true;
            }
            else
            {
                ImageTools.FillBGTransparent = true;
                ImageTools.FillBGBlackStage = false;
                ImageTools.FillBGBlackImage = false;
            }

            Render_Layers();
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logEvents.ClearEvents(rtbEvents);
        }

        private void ActivateLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActivateLoggingToolStripMenuItem.Checked)
            {
                logEvents.bActivateLogging = true;
            }
            else
            {
                logEvents.bActivateLogging = false;
            }
        }

        private void SaveEventsAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Set filter options and filter index.
            saveFile.Title = "Save Events As...";
            saveFile.Filter = "TXT|*.txt";
            saveFile.InitialDirectory = FileTools.strGlobalPath;
            saveFile.FilterIndex = 1;
            saveFile.FileName = null;
            try
            {
                saveFile.FileName = Path.GetFileNameWithoutExtension(openFile.FileName);
                saveFile.FilterIndex = 1;

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    if (saveFile.FileName != "")
                    {
                        if (saveFile.FilterIndex == 1)
                        {
                            logEvents.SaveEvents(saveFile.FileName, rtbEvents);
                            MessageBox.Show("DONE: Events saved in text format.", "Information");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: There has been some error saving the event log.", "Error");
            }
        }

        public void ApplyZoomBG()
        {
            int xPoint, yPoint;
            int iWidth, iHeight, iNewWidth, iNewHeight;
            double dAspectRatio;
            Bitmap bmpMarkBgImage, bmpNewBgImage;
            int ipbWidth, ipbHeight;
            var bResetAutoScroll = default(bool);

            if (S9.bmpBgImage != null &&
                (panelpbBackground.Width > 0 & panelpbBackground.Height > 0))
            {

                // ClearPictureBox(pbBackground, 0, panelpbBackground)

                bmpMarkBgImage = new Bitmap(S9.bmpBgImage.Bitmap);

                // Let's mark the tile in the background if option selected.
                if (bMarkTileBackground & gbTile.Enabled)
                {

                    // Let's mark the tile on the Background. ;)
                    using (Graphics g = Graphics.FromImage(bmpMarkBgImage))
                    {
                        var rectTile = new Rectangle(S9.iLayersbmpPosX + Int32.Parse(txtDestX.Text),
                                                     S9.iLayersbmpPosY + Int32.Parse(txtDestY.Text),
                                                     Int32.Parse(txtTileSize.Text), Int32.Parse(txtTileSize.Text));
                        Color cColor1, cColor2;
                        cColor1 = Color.GreenYellow;
                        cColor2 = Color.Turquoise;
                        using (var pen1 = new Pen(cColor1, 2))
                        {
                            g.DrawRectangle(pen1, rectTile);
                        }

                        using (var pen2 = new Pen(cColor2, 2))
                        {
                            pen2.DashPattern = (new float[] { 2f, 2f });
                            g.DrawRectangle(pen2, rectTile);
                        }
                    }
                }


                // Draw the image zooming it or stretching it.
                if (AutoFillToolStripMenuItem.Checked)
                {
                    // Stretch image if AutoFill checked.
                    // Restore PictureBox Background original dimension.

                    iWidth = S9.bmpBgImage.Width;
                    iHeight = S9.bmpBgImage.Height;
                    ipbWidth = panelpbBackground.Width - 12;
                    ipbHeight = panelpbBackground.Height - 12;
                    dAspectRatio = iWidth / (double)iHeight;
                    if (ipbWidth / (double)ipbHeight > dAspectRatio)
                    {
                        ipbWidth = (int)Math.Round(dAspectRatio * ipbHeight);
                    }
                    else
                    {
                        ipbHeight = (int)Math.Round(ipbWidth / dAspectRatio);
                    }

                    iNewWidth = ipbWidth;
                    iNewHeight = ipbHeight;
                }
                else
                {
                    // No Stretch image.
                    // Here we prepare the image with zoom if needed.

                    iNewWidth = S9.bmpBgImage.Width * iBGZoom;
                    iNewHeight = S9.bmpBgImage.Height * iBGZoom;
                }

                if (pbBackground.Width != iNewWidth | pbBackground.Height != iNewHeight)
                {
                    pbBackground.Size = new Size(iNewWidth, iNewHeight);
                    if (panelpbBackground.Height - 4 < pbBackground.Height)
                    {
                        yPoint = 0;
                        bResetAutoScroll = true;
                    }
                    else
                    {
                        yPoint = (panelpbBackground.Height - 4 - pbBackground.Height) / 2;
                    }

                    if (panelpbBackground.Width - 4 < pbBackground.Width)
                    {
                        xPoint = 0;
                        bResetAutoScroll = true;
                    }
                    else
                    {
                        xPoint = (panelpbBackground.Width - 4 - pbBackground.Width) / 2;
                    }

                    if (bResetAutoScroll)
                    {
                        panelpbBackground.AutoScrollPosition = new Point(0, 0);
                        pbBackground.Top = 0;
                        pbBackground.Left = 0;
                    }

                    pbBackground.Location = new Point(xPoint, yPoint);
                }


                // Draw Image
                bmpNewBgImage = new Bitmap(iNewWidth, iNewHeight);

                using (Graphics g = Graphics.FromImage(bmpNewBgImage))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.DrawImage(bmpMarkBgImage, 0, 0, iNewWidth, iNewHeight);
                }


                // Put image in Background
                if (pbBackground.Image != null) pbBackground.Image.Dispose();

                pbBackground.Image = new Bitmap(bmpNewBgImage);

                bmpNewBgImage.Dispose();
                bmpMarkBgImage.Dispose();
            }
        }

        private void AutoFillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            X2ToolStripMenuItem.Checked = false;
            X4ToolStripMenuItem.Checked = false;
            iBGZoom = 1;
            Render_Layers();
        }

        private void X2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            X4ToolStripMenuItem.Checked = false;
            AutoFillToolStripMenuItem.Checked = false;
            if (X2ToolStripMenuItem.Checked)
            {
                iBGZoom = 2;
            }
            else
            {
                iBGZoom = 1;
            }

            Render_Layers();
        }

        private void X4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            X2ToolStripMenuItem.Checked = false;
            AutoFillToolStripMenuItem.Checked = false;
            if (X4ToolStripMenuItem.Checked)
            {
                iBGZoom = 4;
            }
            else
            {
                iBGZoom = 1;
            }

            Render_Layers();
        }

        private void ExportToPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (S9.textureImage[cbTextures.SelectedIndex] !=  null)
                {
                    // Set filter options and filter index.
                    saveFile.Title = "Export Texture As .PNG";
                    saveFile.Filter = ".PNG File|*.png";
                    saveFile.InitialDirectory = FileTools.strGlobalPath;
                    saveFile.FilterIndex = 1;
                    saveFile.FileName = FileTools.strGlobalFieldName + "_" + 
                                        cbTextures.SelectedIndex.ToString("00") + "_00.png";
                    saveFile.FilterIndex = 1;

                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        if (saveFile.FileName != "")
                        {
                            if (saveFile.FilterIndex == 1)
                            {
                                ImageTools.WriteBitmap(S9.textureImage[cbTextures.SelectedIndex].Bitmap,
                                                       saveFile.FileName);

                                MessageBox.Show("Texture exported in .PNG format.", "Information");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting Texture.", "Error");
            }
        }

        private void ExportAllToPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strTextureFile;
            int iNumTexture;

            FolderBrowserDialogEX fbdEX = new FolderBrowserDialogEX();

            try
            {
                // We must select the directory from where to read the files.
                fbdEX.folderBrowser.Description = "Select Output Folder for Export All " +
                                                                      "Swizzled Textures of the field:";

                if (FileTools.strGlobalExportAllTextures != null)
                {
                    fbdEX.folderBrowser.SelectedPath = FileTools.strGlobalExportAllTextures;
                }
                else
                {
                    fbdEX.folderBrowser.SelectedPath = FileTools.strGlobalPath;
                }

                fbdEX.Tmr.Start();
                if (fbdEX.folderBrowser.ShowDialog(this) == DialogResult.OK)
                {
                    if (fbdEX.folderBrowser.SelectedPath != "")
                    {
                        // Put Global folder for export all base.
                        FileTools.strGlobalExportAllTextures = fbdEX.folderBrowser.SelectedPath;

                        for (iNumTexture = 0; iNumTexture < S9.MAX_NUM_TEXTURES; iNumTexture++)
                        {
                            if (S9.textureImage[iNumTexture] != null)
                            {
                                strTextureFile = FileTools.strGlobalExportAllTextures + "\\" + 
                                                 FileTools.strGlobalFieldName + "_" + 
                                                 iNumTexture.ToString("00") + "_00.png";

                                ImageTools.WriteBitmap(S9.textureImage[iNumTexture].Bitmap, 
                                                       strTextureFile);

                            }
                        }

                        MessageBox.Show("DONE: Export of All Swizzled Textures of the field.", "Information");
                    }
                }

                fbdEX.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: Exporting All Swizzled Textures.", "Error");
            }
        }

        public void Prepare_ParamStatus()
        {
            int iTabParam, iCLBState;
            // Let's check for Layer1 or greater the param and status conditions.
            // Let's create a List for this.

            // Initialize
            tcParams.TabPages.Clear();

            // Let's prepare params/states tab control
            if (S9.MaxParams > 0)
            {
                for (iTabParam = 1; iTabParam <= S9.MaxParams; iTabParam++)
                {
                    if (S9.ListParams[iTabParam].MinState != 1000)
                    {
                        // If ListParams(iTabParam).MaxState > 0 Or ListParams(iTabParam).MinState > 0 Then
                        newTab = new TabPage("P " + iTabParam.ToString("00"));
                        newCLB = new CheckedListBox();

                        // Create State list in checkedlistbox
                        for (iCLBState = 0; iCLBState <= S9.ListParams[iTabParam].MaxState; iCLBState++)
                        {
                            if (S9.ListParams[iTabParam].States[iCLBState])
                            {
                                newCLB.Items.Add("State " + iCLBState.ToString());
                                newCLB.Name = "clbState" + (iTabParam - 1).ToString();
                                newCLB.Size = new Size(172, 208);
                                newCLB.CheckOnClick = true;
                                newCLB.BorderStyle = BorderStyle.Fixed3D;
                                S9.ListParams[iTabParam].States[iCLBState] = false;
                            }
                        }

                        // Add a Handler
                        newCLB.ItemCheck += clbStatus_ItemCheck;

                        // Add CLB to TabPage
                        newTab.Controls.Add(newCLB);

                        newTab.Name = "tabParam" + iTabParam.ToString();
                        newTab.BorderStyle = BorderStyle.Fixed3D;

                        // Add TabPage to TabControl
                        tcParams.TabPages.Add(newTab);
                        tcParams.ItemSize = new Size(32, 20);
                    }
                }
            }
        }

        public void tcParams_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            // Get the item from the collection.
            TabControl tcParams = sender as TabControl;
            TabPage newTabPage = tcParams.TabPages[e.Index];
            
            // Get the real bounds for the tab rectangle.
            Rectangle newtabBounds = tcParams.GetTabRect(e.Index);

            // Use our own font.
            Font tabFont = e.Font;

            // Draw string. Center the text.
            var stringFlags = new StringFormat();
            stringFlags.Alignment = StringAlignment.Near;
            stringFlags.LineAlignment = StringAlignment.Center;

            if (e.State == DrawItemState.Selected)
            {
                g.FillRectangle(Brushes.LightSlateGray, newtabBounds);
            }
            else
            {
                g.FillRectangle(Brushes.LightGray, newtabBounds);
            }

            g.DrawString(newTabPage.Text, tabFont, Brushes.Black, newtabBounds, new StringFormat(stringFlags));
        }

        public void clbStatus_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int iTabParam, iClbState;
            bool bclbState;
            string[] strSplit;

            CheckedListBox clbStatus = sender as CheckedListBox;

            strSplit = tcParams.SelectedTab.Text.ToString().Split(new char(), ' ');
            iTabParam = Int32.Parse(strSplit[1]);

            strSplit = clbStatus.SelectedItem.ToString().Split(new char(), ' ');
            iClbState = Int32.Parse(strSplit[1]);

            bclbState = Convert.ToBoolean(e.NewValue);

            S9.ListParams[iTabParam].States[iClbState] = bclbState;
            Render_Layers();
        }

        public void Prepare_Sublayers()
        {
            int iclbSublayers, iZID;
            clbSublayers.Items.Clear();

            for (iclbSublayers = 0; iclbSublayers < S9.MaxSublayers; iclbSublayers++)
            {
                iZID = (from itemZID in S9.Section9Z
                        where itemZID.ZLayer == 1 &
                              itemZID.ZSublayer == iclbSublayers
                        select itemZID.ZTileID).First();

                clbSublayers.Items.Add("Num: " + iclbSublayers.ToString("00") + 
                                       "  (ID: " + iZID.ToString("0000") + ")");

                clbSublayers.SetItemCheckState(clbSublayers.Items.Count - 1, CheckState.Checked);
            }
        }

        // Procedure to prevent double-click changing state.
        private void clbSublayers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (clbSublayers.GetItemChecked(clbSublayers.SelectedIndex))
            {
                clbSublayers.SetItemChecked(clbSublayers.SelectedIndex, false);
            }
            else
            {
                clbSublayers.SetItemChecked(clbSublayers.SelectedIndex, true);
            }
        }

        private void clbSublayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render_Layers();
        }

        private void pbTile_Click(object sender, EventArgs e)
        {
            frmTileEditor.ShowDialog(this);
        }

        public void SetpbTile(int iTile, int iTexture)
        {

            var sortZTile = (from sortZItem in S9.Section9Z
                             where sortZItem.ZTexture == iTexture &
                                   sortZItem.ZTileTex == iTile
                             select sortZItem).First();

            txtLayer.Text = sortZTile.ZLayer.ToString();
            txtTile.Text = sortZTile.ZTile.ToString();
            txtID.Text = sortZTile.ZTileID.ToString();
            txtBigID.Text = sortZTile.ZTileBigID.ToString();
            txtTileSize.Text = sortZTile.ZTileSize.ToString();

            cbBlending.Checked = Convert.ToBoolean(sortZTile.ZBlending);
            cbIFX.Checked = Convert.ToBoolean(S9.Section9.pal_ignoreFirstPixel[sortZTile.ZPalette]);

            txtParam.Text = sortZTile.ZParam.ToString();
            txtState.Text = sortZTile.ZState.ToString();
            txtBlendMode.Text = sortZTile.ZBlendMode.ToString();

            if (cbTextures.SelectedItem.ToString().Length > 25)
            {
                txtPalette.Text = "-";
            }
            else
            {
                txtPalette.Text = sortZTile.ZPalette.ToString();
            }

            txtSrcX.Text = S9.Section9.Layer[sortZTile.ZLayer].layerTiles[sortZTile.ZTile].sourceX.ToString();
            txtSrcY.Text = S9.Section9.Layer[sortZTile.ZLayer].layerTiles[sortZTile.ZTile].sourceY.ToString();
            txtDestX.Text = S9.Section9.Layer[sortZTile.ZLayer].layerTiles[sortZTile.ZTile].destX.ToString();
            txtDestY.Text = S9.Section9.Layer[sortZTile.ZLayer].layerTiles[sortZTile.ZTile].destY.ToString();
            txtSrcX2.Text = S9.Section9.Layer[sortZTile.ZLayer].layerTiles[sortZTile.ZTile].sourceX2.ToString();
            txtSrcY2.Text = S9.Section9.Layer[sortZTile.ZLayer].layerTiles[sortZTile.ZTile].sourceY2.ToString();
            txtTexture.Text = S9.Section9.Layer[sortZTile.ZLayer].layerTiles[sortZTile.ZTile].textureID.ToString();
            txtTexture2.Text = S9.Section9.Layer[sortZTile.ZLayer].layerTiles[sortZTile.ZTile].textureID2.ToString();

            LoadTile(iTexture);

            // Let's mark the first Tile clicked.
            ImageTools.UpdateMarkTileTexture(ref pbTexture, cbTextures.SelectedIndex,
                                             Int32.Parse(txtLayer.Text),
                                             Int32.Parse(txtTile.Text));

            // Let's mark the new tile in the background
            if (bMarkTileBackground)
            {
                ApplyZoomBG();
            }
        }

        private void MarkTileBackgroundToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            bMarkTileBackground = !bMarkTileBackground;
            ApplyZoomBG();
        }

        public void LoadTile(int iTexture)
        {
            int iLayer, iTile;

            // Put image tile in PictureBox
            if (S9.textureImage[iTexture] != null)
            {
                if (pbTile.Image != null) pbTile.Image.Dispose();

                pbTile.Image = new Bitmap(panelpbTile.Width - 4, panelpbTile.Height - 4);

                iLayer = Int32.Parse(txtLayer.Text);
                iTile = Int32.Parse(txtTile.Text);
                iTexture = cbTextures.SelectedIndex;

                using (var g = Graphics.FromImage(pbTile.Image))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.PixelOffsetMode = PixelOffsetMode.Half;
                    {
                        g.DrawImage(S9.Section9.Layer[iLayer].layerTiles[iTile].imgTile, 
                                    new Rectangle(0, 0, panelpbTile.Width - 4, panelpbTile.Height - 4), 
                                    new Rectangle(0, 0, S9.Section9.Layer[iLayer].layerTiles[iTile].imgTile.Width,
                                                        S9.Section9.Layer[iLayer].layerTiles[iTile].imgTile.Height), 
                                    GraphicsUnit.Pixel);
                    }
                }
            }
        }

        public void btnTileRight_Click(object sender, EventArgs e)
        {
            int iLayer, iTile, iTexture;
            iLayer = Int32.Parse(txtLayer.Text);
            iTile = Int32.Parse(txtTileTex.Text);
            iTile = iTile + 1;
            if (Int32.Parse(txtTexture2.Text) == 0)
            {
                iTexture = Int32.Parse(txtTexture.Text);
            }
            else
            {
                iTexture = Int32.Parse(txtTexture2.Text);
            }

            if (iTile < S9.Section9.Textures[iTexture].MaxTiles)
            {
                txtTileTex.Text = iTile.ToString();
            }
            else
            {
                txtTileTex.Text = S9.Section9.Textures[iTexture].MaxTiles.ToString();
            }

            SetpbTile(Int32.Parse(txtTileTex.Text), iTexture);
        }

        public void btnTileLeft_Click(object sender, EventArgs e)
        {
            int iLayer, iTile, iTexture;
            iLayer = Int32.Parse(txtLayer.Text);
            iTile = Int32.Parse(txtTileTex.Text);
            iTile = iTile - 1;
            if (Int32.Parse(txtTexture2.Text) == 0)
            {
                iTexture = Int32.Parse(txtTexture.Text);
            }
            else
            {
                iTexture = Int32.Parse(txtTexture2.Text);
            }

            if (iTile > 0)
            {
                txtTileTex.Text = iTile.ToString();
            }
            else
            {
                txtTileTex.Text = "0";
            }

            SetpbTile(Int32.Parse(txtTileTex.Text), iTexture);
        }

        private void pbTexture_MouseDown(object sender, MouseEventArgs e)
        {
            int xPos, yPos, iTexture, iTileSize;
            if (cbTextures.Items.Count > 0 & cbTextures.SelectedIndex > -1)
            {
                if (S9.textureImage[cbTextures.SelectedIndex] != null)
                {
                    iTileSize = Int32.Parse(txtTileSize.Text);
                    xPos = e.X / iTileSize * iTileSize;
                    yPos = e.Y / iTileSize * iTileSize;
                    iTexture = cbTextures.SelectedIndex;

                    // Let's search the clicked Tile.
                    var sortZTile = (from ZItem in S9.Section9Z
                                     where ZItem.ZTexture == iTexture &
                                           ZItem.ZSourceX == xPos &
                                           ZItem.ZSourceY == yPos
                                     select ZItem).ToList();

                    if (sortZTile.Count() > 0)
                    {
                        txtTileTex.Text = sortZTile[0].ZTileTex.ToString();

                        // SetpbTile(iTile, iTexture)
                        SetpbTile(sortZTile[0].ZTileTex, iTexture);
                        ApplyZoomBG();
                    }
                }
            }
        }

        private void pbTexture_MouseMove(object sender, MouseEventArgs e)
        {
            int xPos, yPos, iTileSize, iTexture;
            if (cbTextures.Enabled)
            {
                if (S9.textureImage[cbTextures.SelectedIndex] != null)
                {
                    iTileSize = Int32.Parse(txtTileSize.Text);
                    xPos = e.X / iTileSize * iTileSize;
                    yPos = e.Y / iTileSize * iTileSize;
                    iTexture = cbTextures.SelectedIndex;

                    // Let's search the clicked Tile.
                    var sortZTile = (from ZItem in S9.Section9Z
                                     where ZItem.ZTexture == iTexture & 
                                           ZItem.ZSourceX == xPos & 
                                           ZItem.ZSourceY == yPos
                                     select ZItem).ToList();

                    if (sortZTile.Count() > 0)
                    {
                        // Let's mark the Tile we are moving.
                        ImageTools.UpdateMoveTileTexture(ref pbTexture, 
                                                         cbTextures.SelectedIndex, 
                                                         sortZTile[0].ZLayer, 
                                                         sortZTile[0].ZTile, 
                                                         Int32.Parse(txtLayer.Text),
                                                         Int32.Parse(txtTile.Text));
                    }
                }
            }
        }

        private void pbTexture_MouseLeave(object sender, EventArgs e)
        {
            if (cbTextures.Items.Count > 0)
            {
                if (cbTextures.SelectedIndex > -1)
                {
                    ImageTools.UpdateMarkTileTexture(ref pbTexture, 
                                                     cbTextures.SelectedIndex, 
                                                     Int32.Parse(txtLayer.Text), 
                                                     Int32.Parse(txtTile.Text));
                }
            }
        }

        private void PreviewTextureToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (S9.textureImage[cbTextures.SelectedIndex] != null)
            {
                ImageTools.ClearPictureBox(frmTextureImage.pbTextureImage, 1, panelpbTexture);

                frmTextureImage.pbTextureImage.SizeMode = PictureBoxSizeMode.Normal;

                if (S9.textureImage[cbTextures.SelectedIndex] != null)
                {
                    frmTextureImage.pbTextureImage.Image = new Bitmap(frmTextureImage.pbTextureImage.Width, 
                                                                      frmTextureImage.pbTextureImage.Height);

                    using (var g = Graphics.FromImage(frmTextureImage.pbTextureImage.Image))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.CompositingMode = CompositingMode.SourceCopy;

                        g.DrawImage(S9.textureImage[cbTextures.SelectedIndex].Bitmap,
                                    new Rectangle(0, 0, frmTextureImage.pbTextureImage.Width, 
                                                        frmTextureImage.pbTextureImage.Height),
                                    0, 0, S9.textureImage[cbTextures.SelectedIndex].Width,
                                          S9.textureImage[cbTextures.SelectedIndex].Height,
                                    GraphicsUnit.Pixel);
                    }
                }

                frmTextureImage.Text = "Base Texture Preview";
                frmTextureImage.ShowDialog(this);
            }
        }

        private void UnswizzleInternalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (S9.textureImage[cbTextures.SelectedIndex] != null)
            {
                SwizzleBase.UnswizzleInternalBaseTexture(cbTextures.SelectedIndex, frmTextureImage);

                frmTextureImage.Text = "Unswizzled Texture Preview";
                frmTextureImage.ShowDialog(this);
            }
        }

        private void SwizzleExternalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSwizzleExternalHashImage.ShowDialog(this);
        }

        private void UnswizzleExternalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUnswizzleExternalHashTextures.ShowDialog(this);
        }

        private void SwizzleHashedBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSwizzleHashesBatch.ShowDialog(this);
        }

        private void UnswizzleHashedBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUnswizzleHashesBatch.ShowDialog(this);
        }

        private void BasePreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBasePreview.ShowDialog(this);
        }

        private void SwizzleAllExportedBaseToPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSwizzleExternalBaseImages.ShowDialog(this);
        }

        private void pbPalette_MouseMove(object sender, MouseEventArgs e)
        {
            int xCursor, yCursor, iColor;
            Color cColor;
            if (cbPalettes.Items.Count > 0)
            {
                if (cbPalettes.SelectedIndex > -1)
                {
                    xCursor = e.X;
                    yCursor = e.Y;
                    if (e.X > 255)
                        xCursor = 255;
                    if (e.Y > 255)
                        yCursor = 255;
                    iColor = yCursor / 16 * 16 + xCursor / 16;
                    using (Graphics g = Graphics.FromImage(bmpPALInfoColor))
                    {
                        if (e.X > 128)
                        {
                            xCursor = e.X - (bmpPALInfoColor.Width + 5);
                        }
                        else
                        {
                            xCursor = e.X + 5;
                        }

                        if (e.Y > 128)
                        {
                            yCursor = e.Y - (bmpPALInfoColor.Height + 5);
                        }
                        else
                        {
                            yCursor = e.Y + 5;
                        }

                        using (var br = new SolidBrush(Color.Lavender))
                        {
                            ImageTools.FillRoundedRectangle(g, 
                                                            new Rectangle(0, 0, bmpPALInfoColor.Width - 1, 
                                                                                bmpPALInfoColor.Height - 1),
                                                            7, br);
                        }

                        using (var p = new Pen(Color.DarkSlateBlue, 1))
                        {
                            ImageTools.DrawRoundedRectangle(g, 
                                                            new Rectangle(0, 0, bmpPALInfoColor.Width - 1, 
                                                                                bmpPALInfoColor.Height - 1), 
                                                            7, p);
                        }

                        cColor = Palette.ARGB_BASEPAL[cbPalettes.SelectedIndex].ARGB_COLORS[iColor];
                        g.DrawString("Idx: " + iColor.ToString("000") + 
                                     "\n\r R: " + cColor.R.ToString("000") +
                                     "\n\r G: " + cColor.G.ToString("000") +
                                     "\n\r B: " + cColor.B.ToString("000"), 
                                     new Font("Tahoma", 8, FontStyle.Bold), 
                                     new SolidBrush(Color.DarkBlue), 4, 0);
                    }


                    // Define Rectangle Part
                    rectPALInfoColor = new Rectangle(xCursor, yCursor, bmpPALInfoColor.Width, bmpPALInfoColor.Height);
                    bmpPALInfoColor = ImageTools.ChangeOpacity(bmpPALInfoColor, 0.8f);
                    bPALInfoColorMouseMove = true;
                    pbPalette.Invalidate();
                }
            }
        }

        private void pbPalette_MouseLeave(object sender, EventArgs e)
        {
            if (cbPalettes.Items.Count > 0 & cbPalettes.SelectedIndex > -1)
            {
                Palette.Refresh_Palette(ref pbPalette, cbPalettes.SelectedIndex);
            }

            bPALInfoColorMouseMove = false;
            pbPalette.Invalidate();
        }

        private void pbPalette_Paint(object sender, PaintEventArgs e)
        {
            if (bPALInfoColorMouseMove)
            {
                e.Graphics.DrawImage(bmpPALInfoColor, rectPALInfoColor);
            }
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            int i;
            if (clbSublayers.Items.Count > 0)
            {
                for (i = 0; i < clbSublayers.Items.Count; i++)
                    clbSublayers.SetItemChecked(i, true);
            }

            Render_Layers();
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            int i;
            if (clbSublayers.Items.Count > 0)
            {
                for (i = 0; i < clbSublayers.Items.Count; i++)
                    clbSublayers.SetItemChecked(i, false);
            }

            Render_Layers();
        }

        private void UnswizzleAllInternalBaseTexturestoPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int iResult;

            FolderBrowserDialogEX fbdEX = new FolderBrowserDialogEX();

            try
            {
                // We must select the directory from where to read the files.

                fbdEX.folderBrowser.Description = "Select the Output Folder where to Export All the Base Images Unswizzled:";

                fbdEX.folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;

                if (FileTools.strGlobalUnswizzleAllBaseTextures != null)
                {
                    fbdEX.folderBrowser.SelectedPath = FileTools.strGlobalUnswizzleAllBaseTextures;
                }
                else
                {
                    fbdEX.folderBrowser.SelectedPath = FileTools.strGlobalPath;
                }

                fbdEX.Tmr.Start();
                if (fbdEX.folderBrowser.ShowDialog(this) == DialogResult.OK)
                {
                    if (fbdEX.folderBrowser.SelectedPath != "")
                    {
                        // Put Global folder for export all base.
                        FileTools.strGlobalUnswizzleAllBaseTextures = fbdEX.folderBrowser.SelectedPath;

                        iResult = SwizzleBase.UnswizzleFieldTexturesToBaseImages(FileTools.strGlobalUnswizzleAllBaseTextures);

                        if (iResult == 0)
                        {
                            MessageBox.Show("The Unswizzle of All Base Textures into Base Images in .PNG format is DONE.", 
                                            "Information");
                        }
                        else if (iResult == -4)
                        {
                            MessageBox.Show("ERROR: There has been some problem reading the EABI Template file for this field.", 
                                            "Warning");
                        }
                        else
                        {
                            MessageBox.Show("ERROR: There has been some problem Exporting All Base Images to .PNG.", 
                                            "Information");
                        }
                    }
                }

                fbdEX.folderBrowser.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Unswizzle process for All Base Images...", "Error");
            }
        }

        private void frmAeris_Resize(object sender, EventArgs e)
        {
            if (this.Text.Length > 0 & this.Text != "Aeris")
            {
                Render_Layers();
            }
        }


        // '''''''''''''''''''''''''''''''
        // ''''''''''''''''''''''''''''''' Panning of Background
        // '''''''''''''''''''''''''''''''
        // Global Variables
        public static int _xPos;
        public static int _yPos;
        private bool _dragging;

        // Register mouse events
        private void pbBackground_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender == null) return;
            _dragging = false;
        }

        private void pbBackground_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender == null) return;

            if (e.Button != MouseButtons.Left) return;

            _dragging = true;
            _xPos = e.X;
            _yPos = e.Y;
        }

        private void pbBackground_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                panelpbBackground.AutoScrollPosition = 
                        new Point(-(panelpbBackground.AutoScrollPosition.X + e.X - _xPos), 
                                  -(panelpbBackground.AutoScrollPosition.Y + e.Y - _yPos));
            }
        }
        // '''''''''''''''''''''''''''''''
        // ''''''''''''''''''''''''''''''' End of Panning of Background
        // '''''''''''''''''''''''''''''''

        private void SaveFieldAsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int iResult;

            // Set filter options and filter index.
            saveFile.Title = "Save Field As...";
            saveFile.Filter = "All files (*.*)|*.*";
            saveFile.FilterIndex = 1;
            saveFile.FileName = null;
            try
            {

                // Check Initial Directory
                if (FileTools.strGlobalPathSaveFieldFolder != null)
                {
                    saveFile.InitialDirectory = FileTools.strGlobalPathSaveFieldFolder;
                }
                else
                {
                    saveFile.InitialDirectory = FileTools.strGlobalPathFieldFolder;
                }

                saveFile.FileName = FileTools.strFileFieldName;
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    if (saveFile.FileName.Length > 0)
                    {
                        iResult = FileTools.Save_Field(saveFile.FileName);

                        if (iResult == 0)
                        {
                            MessageBox.Show("DONE: Field has been saved as '" + saveFile.FileName + "'.", 
                                            "Information");

                            // Update Aeris Title
                            FileTools.bFieldModified = false;

                            FileTools.strGlobalPathSaveFieldFolder = Path.GetDirectoryName(saveFile.FileName);
                            FileTools.strFileFieldName = Path.GetFileName(saveFile.FileName);

                            Update_AerisTitle();
                        }
                        else
                        {
                            MessageBox.Show("There has been some error saving the Field '" + saveFile.FileName + "'.", 
                                            "Error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the saving Field process.", 
                                "Error");
            }
        }

        private void ImportTextureToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int iResult, iTexture;
            bool TypeTextureID2;


            // Set filter options and filter index.
            openFile.Title = "Import Texture...";
            openFile.Filter = "PNG Images (*.png)|*.png|All Files (*.*)|*.*";
            openFile.FilterIndex = 1;
            openFile.FileName = null;

            try
            {
                // Let's try to predict the name.
                if (gbTile.Enabled)
                {
                    openFile.FileName = FileTools.strGlobalFieldName + "_" + 
                                        cbTextures.SelectedIndex.ToString("00") + "_" + 
                                        String.Format(txtPalette.Text, "00") + ".png";

                    if (FileTools.strGlobalImportTexture != null)
                        openFile.InitialDirectory = FileTools.strGlobalImportTexture;
                    else
                        openFile.InitialDirectory = FileTools.strGlobalPath;

                }
                else
                {
                    MessageBox.Show("Please, you must have selected an existant texture for import.", "Information");
                    return;
                }

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    if (openFile.FileName != "")
                    {
                        iTexture = Int32.Parse(txtTexture2.Text);

                        if (iTexture == 0)
                        {
                            iTexture = Int32.Parse(txtTexture.Text);
                            TypeTextureID2 = false;
                        }
                        else
                        {                            
                            TypeTextureID2 = true;
                        }

                        iResult = ImageTools.ImportTexture(openFile.FileName, 
                                                           Int32.Parse(txtLayer.Text), 
                                                           iTexture, 
                                                           TypeTextureID2);

                        if (iResult == 0)
                        {
                            MessageBox.Show("DONE: Texture imported correctly.", 
                                            "Information");

                            RefreshBackground(iTexture);

                            FileTools.strGlobalImportTexture = Path.GetDirectoryName(openFile.FileName);
                        }
                        else if (iResult == 1)
                        {
                            MessageBox.Show("You are trying to import an image without the correct size for a Texture (256x256). Process aborted.", 
                                            "Information");
                        }
                        else if (iResult == 2)
                        {
                        }
                        // We check this inside the process.
                        else if (iResult == 3)
                        {
                            MessageBox.Show("The selected texture in Aeris has not any matching Tile.", 
                                            "Information");
                        }
                        else if (iResult == 4)
                        {
                            MessageBox.Show("One of the Tiles of the selected texture has assigned a non " +
                                            "existant PaletteID.",
                                            "Warning");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error importing texture.", 
                                "Error");
            }
        }

        private void SaveFieldToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int iResult;
            string strSaveFileName;
            saveFile.FilterIndex = 1;
            saveFile.FileName = null;

            try
            {
                // Check Initial Directory
                if (FileTools.strGlobalPathSaveFieldFolder != null)
                {
                    strSaveFileName = FileTools.strGlobalPathSaveFieldFolder;
                }
                else
                {
                    strSaveFileName = FileTools.strGlobalPathFieldFolder;
                }

                strSaveFileName += "\\" + FileTools.strFileFieldName;

                if (strSaveFileName.Length > 0)
                {
                    iResult = FileTools.Save_Field(strSaveFileName);
                    if (iResult == 0)
                    {
                        // Update Aeris Title
                        FileTools.bFieldModified = false;
                        Update_AerisTitle();
                    }
                    else
                    {
                        MessageBox.Show("There has been some error saving the Field '" + saveFile.FileName + "'.", 
                                        "Error");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in the saving Field process.", 
                                "Error");
            }
        }

        private void ExportToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set filter options and filter index.
            saveFile.Title = "Save Palette As...";
            saveFile.Filter = "PAL (GIMP Format) file|*.gpl|Windows PAL (RIFF Format) file|*.pal|All files (*.*)|*.*";
            saveFile.InitialDirectory = FileTools.strGlobalPath;
            saveFile.FilterIndex = 1;
            saveFile.FileName = null;

            try
            {
                saveFile.FileName = FileTools.strGlobalFieldName + "_" + cbPalettes.SelectedIndex.ToString("00");

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    if (saveFile.FileName.Length > 0)
                    {
                        if (saveFile.FilterIndex == 1)
                        {
                            Palette.ExportGIMPPAL(saveFile.FileName, cbPalettes.SelectedIndex);
                            MessageBox.Show("Palette in GIMP .GPL Format exported.", 
                                            "Information");
                        }
                        else if (saveFile.FilterIndex == 2)
                        {
                            Palette.ExportMSPAL(saveFile.FileName, cbPalettes.SelectedIndex);
                            MessageBox.Show("Palette in Windows RIFF .PAL Format exported.", 
                                            "Information");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting Palette.",
                                "Error");
            }
        }

        private void ImportPaletteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int iResult, iPalette;

            // Set filter options and filter index.
            openFile.Title = "Import Palette...";
            openFile.Filter = "Microsoft PAL (*.pal)|*.pal|All Files (*.*)|*.*";
            openFile.InitialDirectory = FileTools.strGlobalPath;
            openFile.FilterIndex = 1;
            openFile.FileName = null;
            try
            {
                iPalette = cbPalettes.SelectedIndex;

                // Let's try to predict the name.
                openFile.FileName = FileTools.strGlobalFieldName + "_" + iPalette.ToString("00");

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    if (openFile.FileName != "")
                    {
                        iPalette = Int32.Parse(Path.GetFileNameWithoutExtension(openFile.FileName).Split('_').Last());
                        iResult = Palette.ImportMSPAL(openFile.FileName, iPalette);

                        if (iResult == 0)
                        {
                            MessageBox.Show("DONE: Palette imported correctly.", 
                                            "Information");

                            Palette.Load_BASEARGB();

                            RefreshBackground(cbTextures.SelectedIndex);

                            if (cbPalettes.Items.Count > 0 & cbPalettes.SelectedIndex > -1)
                            {
                                Palette.Refresh_Palette(ref pbPalette, cbPalettes.SelectedIndex);
                            }
                        }
                        else if (iResult == -1)
                        {
                            MessageBox.Show("There has been some error importing the Palette.", 
                                            "Information");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR: There has been some error importing the palette.", "Error");
            }
        }

        private void Prepare_Palettes()
        {
            int iPalette = 0;
            cbPalettes.Items.Clear();

            for (iPalette = 0; iPalette < S4.Section4.numPalettes; iPalette++)
                   cbPalettes.Items.Add("PaletteID: " + iPalette.ToString());
        }

        private void Prepare_Textures()
        {
            int iTexture = 0;
            string strTexture = "";

            cbTextures.Items.Clear();

            for (iTexture = 0; iTexture < S9.MAX_NUM_TEXTURES; iTexture++)
            {
                if (S9.Section9.Textures[iTexture].textureFlag == 1)
                {
                    if (S9.Section9.Textures[iTexture].Depth < 2)
                        strTexture = "TextureID: " + iTexture.ToString();
                    else
                        strTexture = "TextureID: " + iTexture.ToString() + " (Direct - NO Palette)";
                }
                else
                {
                    strTexture = "TextureID: " + iTexture.ToString() + " (-)";
                }

                cbTextures.Items.Add(strTexture);
            }
        }

        public void RefreshBackground(int iTETexture)
        {
            S9.Prepare_ListParams();
            S9.PreRender_S9Tiles();
            S9.Load_ZList(rtbEvents);

            SetpbTile(Int32.Parse(txtTileTex.Text), iTETexture);
            Prepare_ParamStatus();
            Prepare_Sublayers();
            Render_Layers();
        }

    }
}
