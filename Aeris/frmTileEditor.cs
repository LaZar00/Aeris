using System;
using System.Drawing;
using System.Windows.Forms;


namespace Aeris
{

    using static S4;
    using static S9;

    using static TileEditor;
    using static Palette;

    public partial class FrmTileEditor : Form
    {

        readonly private FrmAeris frmAeris;


        public FrmTileEditor(FrmAeris inFrmAeris)
        {
            InitializeComponent();

            this.frmAeris = inFrmAeris;
            Owner = inFrmAeris;
        }

        public static Bitmap bmpPALInfoColorPALTE = new Bitmap(68, 66);
        public static Rectangle rectPALInfoColorPALTE;
        public static Bitmap bmpPALInfoColorTE = new Bitmap(68, 66);
        public static Rectangle rectPALInfoColorTE;


        private void FrmTileEditor_Load(object sender, EventArgs e)
        {
            btnCommitALL.Enabled = false;
            btnCommitIMAGE.Enabled = false;
            btnCommitINFO.Enabled = false;
            bActivateIFP = true;
        }

        private void FrmTileEditor_Shown(object sender, EventArgs e)
        {
            // Here we will initialize some things of the Tile Editor with the Tile pressed
            // and populate the Form with the Tile INFO and IMAGE.
            PopulateTE();
            CheckNextPrevTileButtons();

            if (cbActivateIFP.Enabled)
                GetIndexFirstBlack(Int32.Parse(txtPaletteID.Text));
        }

        public void PopulateTE()
        {

            // Initialize INFO part.
            InitializeTILEINFO();

            // Initialize GRAPHIC part.
            InitializeTILEIMAGE();

            if (bIsDirectTile)
            {
                cbActivateIFP.Checked = false;
                cbActivateIFP.Enabled = false;
                pbPalette.Enabled = false;
                btnPickColor.Enabled = false;
                btnInfo.Enabled = false;
                btnDraw.Enabled = false;

                pbPalette.Cursor = Cursors.Default;
                pbTileEdit.Cursor = Cursors.Default;
            }
            else
            {
                if (Section9.pal_ignoreFirstPixel[iTEPalette] == 1)
                {
                    cbActivateIFP.Checked = bActivateIFP;
                    cbActivateIFP.Enabled = true;
                }
                else
                {
                    cbActivateIFP.Checked = false;
                    cbActivateIFP.Enabled = false;
                }

                pbPalette.Enabled = true;
                btnPickColor.Enabled = true;
                btnInfo.Enabled = true;
                btnDraw.Enabled = true;
                btnInfo.Checked = true;

                DrawButtons_Checker();
            }
        }

        private void CbActivateIFP_CheckedChanged(object sender, EventArgs e)
        {
            bActivateIFP = cbActivateIFP.Checked;
            InitializeTILEIMAGE();

            // Change selected color
            UpdatePickedColorValues();
        }

        private void PbPalette_MouseMove(object sender, MouseEventArgs e)
        {
            if (!btnInfo.Checked | bIsDirectTile)
            {
                return;
            }

            int xCursor, yCursor, iColor;

            Color cColor;

            xCursor = e.X;
            yCursor = e.Y;

            if (e.X > 255) xCursor = 255;
            if (e.Y > 255) yCursor = 255;

            iColor = yCursor / 16 * 16 + xCursor / 16;

            using (Graphics g = Graphics.FromImage(bmpPALInfoColorPALTE))
            {
                if (e.X > 128)
                {
                    xCursor = e.X - (bmpPALInfoColorPALTE.Width + 5);
                }
                else
                {
                    xCursor = e.X + 5;
                }

                if (e.Y > 128)
                {
                    yCursor = e.Y - (bmpPALInfoColorPALTE.Height + 5);
                }
                else
                {
                    yCursor = e.Y + 5;
                }

                using (var br = new SolidBrush(Color.Lavender))
                {
                    ImageTools.FillRoundedRectangle(g, 
                                                    new Rectangle(0, 0, 
                                                                  bmpPALInfoColorPALTE.Width - 1,
                                                                  bmpPALInfoColorPALTE.Height - 1),
                                                    7, br);
                }

                using (var p = new Pen(Color.DarkSlateBlue, 1))
                {
                    ImageTools.DrawRoundedRectangle(g, 
                                                    new Rectangle(0, 0, 
                                                                  bmpPALInfoColorPALTE.Width - 1,
                                                                  bmpPALInfoColorPALTE.Height - 1),
                                                    7, p);
                }

                cColor = ARGB_BASEPAL[iTEPalette].ARGB_COLORS[iColor];

                g.DrawString("Idx: " + iColor.ToString("000") + 
                             "\n\r R: " + cColor.R.ToString("000") +
                             "\n\r G: " + cColor.G.ToString("000") +
                             "\n\r B: " + cColor.B.ToString("000"), 
                             new Font("Tahoma", 8, FontStyle.Bold), 
                             new SolidBrush(Color.DarkBlue), 4, 0);
            }


            // Define Rectangle Part
            rectPALInfoColorPALTE = new Rectangle(xCursor, yCursor, bmpPALInfoColorPALTE.Width, bmpPALInfoColorPALTE.Height);
            bmpPALInfoColorPALTE = ImageTools.ChangeOpacity(bmpPALInfoColorPALTE, 0.8f);
            bPALInfoColorPALTEMouseMove = true;
            pbPalette.Invalidate();
        }

        private void PbPalette_MouseLeave(object sender, EventArgs e)
        {
            if (!btnInfo.Checked | bIsDirectTile)
            {
                return;
            }

            Refresh_Palette(ref pbPalette, iTEPalette);
            bPALInfoColorPALTEMouseMove = false;
            pbPalette.Invalidate();
        }

        private void PbPalette_Paint(object sender, PaintEventArgs e)
        {
            if (!btnInfo.Checked)
            {
                return;
            }

            if (bPALInfoColorPALTEMouseMove)
            {
                e.Graphics.DrawImage(bmpPALInfoColorPALTE, rectPALInfoColorPALTE);
            }
        }

        private void PbTileEdit_MouseMove(object sender, MouseEventArgs e)
        {
            if (!btnInfo.Checked | bIsDirectTile)
            {
                return;
            }

            int xCursor, yCursor, iColor;

            Color cColor;

            xCursor = e.X;
            yCursor = e.Y;

            if (e.X > 255) xCursor = 255;
            if (e.Y > 255) yCursor = 255;

            iColor = TileMatrix[xCursor / iTETileSizeFill,
                                yCursor / iTETileSizeFill];

            if (cbActivateIFP.Enabled &&
                cbActivateIFP.Checked &&
                iColor == indexFirstBlack)
                            iColor = 0;

            using (Graphics g = Graphics.FromImage(bmpPALInfoColorTE))
            {
                if (e.X > 128)
                {
                    xCursor = e.X - (bmpPALInfoColorTE.Width + 5);
                }
                else
                {
                    xCursor = e.X + 5;
                }

                if (e.Y > 128)
                {
                    yCursor = e.Y - (bmpPALInfoColorTE.Height + 5);
                }
                else
                {
                    yCursor = e.Y + 5;
                }

                using (var br = new SolidBrush(Color.Lavender))
                {
                    ImageTools.FillRoundedRectangle(g, 
                                                    new Rectangle(0, 0, 
                                                                  bmpPALInfoColorTE.Width - 1, 
                                                                  bmpPALInfoColorTE.Height - 1),
                                                    7, br);
                }

                using (var p = new Pen(Color.DarkSlateBlue, 1))
                {
                    ImageTools.DrawRoundedRectangle(g, 
                                                    new Rectangle(0, 0, 
                                                                  bmpPALInfoColorTE.Width - 1, 
                                                                  bmpPALInfoColorTE.Height - 1), 
                                                    7, p);
                }

                cColor = ARGB_BASEPAL[iTEPalette].ARGB_COLORS[iColor];

                g.DrawString("Idx: " + iColor.ToString("000") + 
                             "\n\r R: " + cColor.R.ToString("000") +
                             "\n\r G: " + cColor.G.ToString("000") +
                             "\n\r B: " + cColor.B.ToString("000"), 
                             new Font("Tahoma", 8, FontStyle.Bold), 
                             new SolidBrush(Color.DarkBlue), 4, 0);
            }


            // Define Rectangle Part
            rectPALInfoColorTE = new Rectangle(xCursor, yCursor, bmpPALInfoColorTE.Width, bmpPALInfoColorTE.Height);
            bmpPALInfoColorTE = ImageTools.ChangeOpacity(bmpPALInfoColorTE, 0.8f);

            bPALInfoColorTEMouseMove = true;

            pbTileEdit.Invalidate();
        }

        private void PbTileEdit_MouseLeave(object sender, EventArgs e)
        {
            if (!btnInfo.Checked | bIsDirectTile)
            {
                return;
            }

            Draw_Tile(ref pbTileEdit, cbActivateIFP.Checked);
            bPALInfoColorTEMouseMove = false;
            pbTileEdit.Invalidate();
        }

        private void PbTileEdit_Paint(object sender, PaintEventArgs e)
        {
            if (!btnInfo.Checked | bIsDirectTile)
            {
                return;
            }

            if (bPALInfoColorTEMouseMove)
            {
                e.Graphics.DrawImage(bmpPALInfoColorTE, rectPALInfoColorTE);
            }
        }

        private void BtnPickColor_Click(object sender, EventArgs e)
        {
            DrawButtons_Checker();
        }

        private void BtnDraw_Click(object sender, EventArgs e)
        {
            DrawButtons_Checker();
        }

        private void BtnInfo_CheckedChanged(object sender, EventArgs e)
        {
            DrawButtons_Checker();
        }

        public void DrawButtons_Checker()
        {
            pbPalette.Cursor = ImageTools.CrossCUR;
            pbTileEdit.Cursor = ImageTools.CrossCUR;

            if (btnDraw.Checked)
            {

                pbTileEdit.Cursor = ImageTools.PencilCUR;

                if (btnPickColor.Checked)
                {
                    pbPalette.Cursor = ImageTools.PickerCUR;
                }
            }
            else if (btnPickColor.Checked)
            {
                pbPalette.Cursor = ImageTools.PickerCUR;
                pbTileEdit.Cursor = ImageTools.PickerCUR;
            }
        }

        private void PbPalette_MouseDown(object sender, MouseEventArgs e)
        {
            if (btnPickColor.Checked)
            {
                int xCursor, yCursor;

                xCursor = e.X;
                yCursor = e.Y;

                if (e.X > 255) xCursor = 255;
                if (e.Y > 255) yCursor = 255;

                iIndexPickedColor = xCursor / iTETileSize +
                                    ((yCursor / iTETileSize) * iTETileSize); ;

                UpdatePickedColorValues();
            }
        }

        private void PbTileEdit_MouseDown(object sender, MouseEventArgs e)
        {
            int xCursor, yCursor;
            if (bIsDirectTile)
            {
                return;
            }

            xCursor = e.X;
            yCursor = e.Y;

            if (e.X > 255) xCursor = 255;
            if (e.Y > 255) yCursor = 255;

            if (btnDraw.Checked)
            {
                if (cbActivateIFP.Enabled)
                {
                    if (iIndexPickedColor != 0)
                    {
                        TileMatrix[xCursor / iTETileSizeFill,
                                   yCursor / iTETileSizeFill] = (byte)iIndexPickedColor;
                    }
                    else
                    {
                        TileMatrix[xCursor / iTETileSizeFill,
                                   yCursor / iTETileSizeFill] = (byte)indexFirstBlack;
                    }
                }
                else
                {
                    TileMatrix[xCursor / iTETileSizeFill,
                               yCursor / iTETileSizeFill] = (byte)iIndexPickedColor;
                }

                InitializeTILEIMAGE();

                btnCommitIMAGE.Enabled = true;
                CheckCommitButtons();
            }
            else if (btnPickColor.Checked)
            {
                iIndexPickedColor = TileMatrix[xCursor / iTETileSizeFill,
                                               yCursor / iTETileSizeFill];

                UpdatePickedColorValues();
            }
        }

        private void TxtID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtPaletteID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtDestX_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtDestY_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtBlending_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtBlendMode_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtParam_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtState_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtTextureID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtSourceX_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtSourceY_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtTextureID2_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtSourceX2_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtSourceY2_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtBigID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtSourceXBigID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void TxtSourceYBigID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        public DialogResult CommitChanges()
        {
            DialogResult mbResult;

            mbResult = DialogResult.Yes;

            if (btnCommitALL.Enabled)
            {
                mbResult = MessageBox.Show("You have made some changes in TILE INFO and TILE IMAGE.\n\r" + 
                                           "Do you want to Commit ALL the Tile changes?", 
                                           "Information", 
                                           MessageBoxButtons.YesNoCancel);

                if (mbResult == DialogResult.Yes)
                {
                    TECommitINFO(this, frmAeris);
                    TECommitIMAGE(frmAeris);

                    // Refresh Texture and Background
                    frmAeris.RefreshBackground(iTETexture);
                }
            }
            else
            {
                if (btnCommitINFO.Enabled)
                {
                    mbResult = MessageBox.Show("You have made some changes in TILE INFO.\n\r" + 
                                               "Do you want to Commit the TILE INFO changes?", 
                                               "Information", 
                                               MessageBoxButtons.YesNoCancel);

                    if (mbResult == DialogResult.Yes)
                    {
                        TECommitINFO(this, frmAeris);

                        // Refresh Texture and Background
                        frmAeris.RefreshBackground(iTETexture);
                    }
                }

                if (btnCommitIMAGE.Enabled)
                {
                    mbResult = MessageBox.Show("You have made some changes in the TILE IMAGE.\n\r" + 
                                               "Do you want to Commit the TILE IMAGE changes?", 
                                               "Information", 
                                               MessageBoxButtons.YesNoCancel);

                    if (mbResult == DialogResult.Yes)
                    {
                        TECommitIMAGE(frmAeris);

                        // Refresh Texture and Background
                        frmAeris.RefreshBackground(iTETexture);
                    }
                }
            }

            return mbResult;
        }

        public void CheckNextPrevTileButtons()
        {
            btnNextTile.Enabled = true;
            btnPrevTile.Enabled = true;
            if (iTEAbsTileNum == 0)
            {
                btnPrevTile.Enabled = false;
                btnNextTile.Enabled = true;
            }

            // Let's check we don't go lower than Tile 0 and higher than Tile 256 or max tile.
            if (iTEAbsTileNum == Section9.Textures[iTETexture].MaxTiles)
            {
                btnPrevTile.Enabled = true;
                btnNextTile.Enabled = false;
            }

//            iTEAbsTileNum = Int32.Parse(ufrmAeris.txtTileTex.Text);

            btnCommitALL.Enabled = false;
            btnCommitINFO.Enabled = false;
            btnCommitIMAGE.Enabled = false;
        }

        private void BtnPrevTile_Click(object sender, EventArgs e)
        {
            DialogResult mbResult;

            mbResult = CommitChanges();
            if (mbResult == DialogResult.Cancel)
            {
                return;
            }

            frmAeris.BtnTileLeft_Click(sender, new EventArgs());
            PopulateTE();
            CheckNextPrevTileButtons();
        }

        private void BtnNextTile_Click(object sender, EventArgs e)
        {
            DialogResult mbResult;

            mbResult = CommitChanges();
            if (mbResult == DialogResult.Cancel)
            {
                return;
            }

            frmAeris.BtnTileRight_Click(sender, new EventArgs());
            PopulateTE();
            CheckNextPrevTileButtons();
        }

        private void FrmTileEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult mbResult;

            mbResult = CommitChanges();

            // If Commit canceled
            if (mbResult == DialogResult.Cancel)
            {
                // Avoid closing the TileEditor form
                e.Cancel = true;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void CheckCommitButtons()
        {
            if (btnCommitINFO.Enabled & btnCommitIMAGE.Enabled)
            {
                btnCommitALL.Enabled = true;
            }
            else
            {
                btnCommitALL.Enabled = false;
            }
        }

        private void BtnCommitINFO_Click(object sender, EventArgs e)
        {
            if (TECommitINFO(this, frmAeris))
            {
                PopulateTE();

                btnCommitINFO.Enabled = false;
                CheckCommitButtons();

                // Refresh Texture and Background
                frmAeris.RefreshBackground(iTETexture);
            }
        }

        private void BtnCommitIMAGE_Click(object sender, EventArgs e)
        {
            if (TECommitIMAGE(frmAeris))
            {
                PopulateTE();

                btnCommitIMAGE.Enabled = false;
                CheckCommitButtons();

                // Refresh Texture and Background
                frmAeris.RefreshBackground(iTETexture);
            }
        }

        private void BtnCommitALL_Click(object sender, EventArgs e)
        {
            if (TECommitINFO(this, frmAeris))
            {
                if (TECommitIMAGE(frmAeris))
                {
                    PopulateTE();

                    btnCommitINFO.Enabled = false;
                    btnCommitIMAGE.Enabled = false;
                    btnCommitALL.Enabled = false;

                    // Refresh Texture and Background
                    frmAeris.RefreshBackground(iTETexture);
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////
        /// Initialization Public procedures
        ////////////////////////////////////////////////////////////////////////////////////
        public void InitializeTILEIMAGE()
        {
            if (bIsDirectTile)
            {
                // Initialize Tile Image in PictureBox (for TileSize 32)
                Draw_TileDirect(ref pbTileEdit);

                txtPaletteID.Enabled = false;
            }
            else
            {
                // Initialize Palette PictureBox colors of Tile Editor
                ImageTools.ClearPictureBox(pbPalette, 1, null);
                Refresh_Palette(ref pbPalette, iTEPalette);

                // Initialize Tile Image in PictureBox (for TileSize 16)
                Draw_Tile(ref pbTileEdit, cbActivateIFP.Checked);
                txtPaletteID.Enabled = true;
            }
        }

        public void InitializeTILEINFO()
        {
            txtTileLayer.Text = frmAeris.GetLayer();
            txtTileNum.Text = frmAeris.GetTile();

            iTELayer = Int32.Parse(txtTileLayer.Text);
            iTETileNum = Int32.Parse(txtTileNum.Text);
            iTEAbsTileNum = Int32.Parse(frmAeris.GetTileTexture());

            dataTileTE = Section9.Layer[Int32.Parse(txtTileLayer.Text)].
                                    layerTiles[Int32.Parse(txtTileNum.Text)];
            dataTileTEBackup = dataTileTE;

            txtID.Text = dataTileTE.ID.ToString();
            txtPaletteID.Text = dataTileTE.paletteID.ToString();

            iTEPalette = Int32.Parse(txtPaletteID.Text);

            if (iTEPalette > Section4.numPalettes - 1)
            {
                iTEPalette = 0;
            }

            txtWidth.Text = dataTileTE.Width.ToString();
            txtHeight.Text = dataTileTE.Height.ToString();
            txtDestX.Text = dataTileTE.destX.ToString();
            txtDestY.Text = dataTileTE.destY.ToString();
            txtDepth.Text = dataTileTE.depth.ToString();
            txtBlending.Text = dataTileTE.blending.ToString();
            txtBlendMode.Text = dataTileTE.BlendMode.ToString();
            txtParam.Text = dataTileTE.param.ToString();
            txtState.Text = dataTileTE.state.ToString();
            txtTextureID.Text = dataTileTE.textureID.ToString();
            txtSourceX.Text = dataTileTE.sourceX.ToString();
            txtSourceY.Text = dataTileTE.sourceY.ToString();

            iTETexture = Int32.Parse(txtTextureID.Text);
            iTESourceX = Int32.Parse(txtSourceX.Text);
            iTESourceY = Int32.Parse(txtSourceY.Text);

            txtTextureID2.Text = dataTileTE.textureID2.ToString();
            txtSourceX2.Text = dataTileTE.sourceX2.ToString();
            txtSourceY2.Text = dataTileTE.sourceY2.ToString();

            if (dataTileTE.textureID2 > 0)
            {
                iTETexture = Int32.Parse(txtTextureID2.Text);
                iTESourceX = Int32.Parse(txtSourceX2.Text);
                iTESourceY = Int32.Parse(txtSourceY2.Text);
            }

            txtBigID.Text = dataTileTE.bigID.ToString();
            txtSourceXBigID.Text = dataTileTE.sourceXBig.ToString();
            txtSourceYBigID.Text = dataTileTE.sourceYBig.ToString();

            // Let's initialize some global vars for use in Tile Editor
            // Get TileSize, 16 or 32.
            // Normally, 32x32 pixels tiles are in Layer 2-3.
            // iTETileSize -> Has the real value of TileSize (16 or 32)
            // iTETileSizeFill -> Has the value \ 4 for calculate some x,y positions
            // like fill pictureboxes zoomed pixels or get points x/y with mouse.
            if (iTELayer < 2)
            {
                iTETileSize = 16;
                iTETileSizeFill = 16;
            }
            else
            {
                iTETileSize = 32;
                iTETileSizeFill = 8;
            }

            if (Section9.Textures[iTETexture].Depth > 1)
            {
                bIsDirectTile = true;
                Load_TileMatrix2Bytes();
            }
            else
            {
                bIsDirectTile = false;
                Load_TileMatrix();
            }

            iIndexPickedColor = 0;
            UpdatePickedColorValues();
        }

        public void UpdatePickedColorValues()
        {
            int indexPAL;

            if (!bIsDirectTile)
            {
                indexPAL = iIndexPickedColor;

                if (cbActivateIFP.Enabled)
                {
                    if (cbActivateIFP.Checked)
                    {
                        if (indexFirstBlack == indexPAL) indexPAL = 0;
                    }
                    else
                    {
                        if (indexPAL == 0) indexPAL = indexFirstBlack;
                    }
                }

                txtIdx.Text = indexPAL.ToString();

                txtR.Text = Section4.dataPalette[iTEPalette].Pal[indexPAL].Red.ToString();
                txtG.Text = Section4.dataPalette[iTEPalette].Pal[indexPAL].Green.ToString();
                txtB.Text = Section4.dataPalette[iTEPalette].Pal[indexPAL].Blue.ToString();
                txtM.Text = Section4.dataPalette[iTEPalette].Pal[indexPAL].Mask.ToString();

                btnColor.BackColor = GetPalColor(iTEPalette, indexPAL);
            }
        }


    }
}
