using System;
using System.Drawing;
using System.Windows.Forms;


namespace Aeris
{

    public partial class frmTileEditor : Form
    {
        private frmAeris frmAeris;

        public frmTileEditor(frmAeris frmAeris)
        {
            InitializeComponent();

            this.frmAeris = frmAeris;
        }


        public static Bitmap bmpPALInfoColorPALTE = new Bitmap(68, 66);
        public static Rectangle rectPALInfoColorPALTE;
        public static Bitmap bmpPALInfoColorTE = new Bitmap(68, 66);
        public static Rectangle rectPALInfoColorTE;


        private void frmTileEditor_Load(object sender, EventArgs e)
        {
            btnCommitALL.Enabled = false;
            btnCommitIMAGE.Enabled = false;
            btnCommitINFO.Enabled = false;
            TileEditor.bActivateIFP = true;
        }

        private void frmTileEditor_Shown(object sender, EventArgs e)
        {
            // Here we will initialize some things of the Tile Editor with the Tile pressed
            // and populate the Form with the Tile INFO and IMAGE.
            PopulateTE();
            CheckNextPrevTileButtons();

            if (cbActivateIFP.Enabled)
                Palette.GetIndexFirstBlack(Int32.Parse(txtPaletteID.Text));
        }

        public void PopulateTE()
        {

            // Initialize INFO part.
            TileEditor.InitializeTILEINFO(this, frmAeris);

            // Initialize GRAPHIC part.
            TileEditor.InitializeTILEIMAGE(this);

            if (TileEditor.bIsDirectTile)
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
                if (S9.Section9.pal_ignoreFirstPixel[TileEditor.iTEPalette] == 1)
                {
                    cbActivateIFP.Checked = TileEditor.bActivateIFP;
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

        private void cbActivateIFP_CheckedChanged(object sender, EventArgs e)
        {
            TileEditor.bActivateIFP = cbActivateIFP.Checked;
            TileEditor.InitializeTILEIMAGE(this);

            // Change selected color
            TileEditor.UpdatePickedColorValues(this);
        }

        private void pbPalette_MouseMove(object sender, MouseEventArgs e)
        {
            if (!btnInfo.Checked | TileEditor.bIsDirectTile)
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

                cColor = Palette.ARGB_BASEPAL[TileEditor.iTEPalette].ARGB_COLORS[iColor];

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
            TileEditor.bPALInfoColorPALTEMouseMove = true;
            pbPalette.Invalidate();
        }

        private void pbPalette_MouseLeave(object sender, EventArgs e)
        {
            if (!btnInfo.Checked | TileEditor.bIsDirectTile)
            {
                return;
            }

            Palette.Refresh_Palette(ref pbPalette, TileEditor.iTEPalette);
            TileEditor.bPALInfoColorPALTEMouseMove = false;
            pbPalette.Invalidate();
        }

        private void pbPalette_Paint(object sender, PaintEventArgs e)
        {
            if (!btnInfo.Checked)
            {
                return;
            }

            if (TileEditor.bPALInfoColorPALTEMouseMove)
            {
                e.Graphics.DrawImage(bmpPALInfoColorPALTE, rectPALInfoColorPALTE);
            }
        }

        private void pbTileEdit_MouseMove(object sender, MouseEventArgs e)
        {
            if (!btnInfo.Checked | TileEditor.bIsDirectTile)
            {
                return;
            }

            int xCursor, yCursor, iColor;

            Color cColor;

            xCursor = e.X;
            yCursor = e.Y;

            if (e.X > 255) xCursor = 255;
            if (e.Y > 255) yCursor = 255;

            iColor = TileEditor.TileMatrix[xCursor / TileEditor.iTETileSizeFill,
                                           yCursor / TileEditor.iTETileSizeFill];

            if (cbActivateIFP.Enabled &&
                cbActivateIFP.Checked &&
                iColor == Palette.indexFirstBlack)
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

                cColor = Palette.ARGB_BASEPAL[TileEditor.iTEPalette].ARGB_COLORS[iColor];

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

            TileEditor.bPALInfoColorTEMouseMove = true;

            pbTileEdit.Invalidate();
        }

        private void pbTileEdit_MouseLeave(object sender, EventArgs e)
        {
            if (!btnInfo.Checked | TileEditor.bIsDirectTile)
            {
                return;
            }

            TileEditor.Draw_Tile(ref pbTileEdit, this);
            TileEditor.bPALInfoColorTEMouseMove = false;
            pbTileEdit.Invalidate();
        }

        private void pbTileEdit_Paint(object sender, PaintEventArgs e)
        {
            if (!btnInfo.Checked | TileEditor.bIsDirectTile)
            {
                return;
            }

            if (TileEditor.bPALInfoColorTEMouseMove)
            {
                e.Graphics.DrawImage(bmpPALInfoColorTE, rectPALInfoColorTE);
            }
        }

        private void btnPickColor_Click(object sender, EventArgs e)
        {
            DrawButtons_Checker();
        }

        private void btnDraw_Click(object sender, EventArgs e)
        {
            DrawButtons_Checker();
        }

        private void btnInfo_CheckedChanged(object sender, EventArgs e)
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

        private void pbPalette_MouseDown(object sender, MouseEventArgs e)
        {
            if (btnPickColor.Checked)
            {
                int xCursor, yCursor;

                xCursor = e.X;
                yCursor = e.Y;

                if (e.X > 255) xCursor = 255;
                if (e.Y > 255) yCursor = 255;

                TileEditor.iIndexPickedColor = xCursor / TileEditor.iTETileSize +
                                               ((yCursor / TileEditor.iTETileSize) * TileEditor.iTETileSize); ;

                TileEditor.UpdatePickedColorValues(this);
            }
        }

        private void pbTileEdit_MouseDown(object sender, MouseEventArgs e)
        {
            int xCursor, yCursor;
            if (TileEditor.bIsDirectTile)
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
                    if (TileEditor.iIndexPickedColor != 0)
                    {
                        TileEditor.TileMatrix[xCursor / TileEditor.iTETileSizeFill,
                                              yCursor / TileEditor.iTETileSizeFill] = (byte)TileEditor.iIndexPickedColor;
                    }
                    else
                    {
                        TileEditor.TileMatrix[xCursor / TileEditor.iTETileSizeFill,
                                              yCursor / TileEditor.iTETileSizeFill] = (byte)Palette.indexFirstBlack;
                    }
                }
                else
                {
                    TileEditor.TileMatrix[xCursor / TileEditor.iTETileSizeFill,
                                          yCursor / TileEditor.iTETileSizeFill] = (byte)TileEditor.iIndexPickedColor;
                }

                TileEditor.InitializeTILEIMAGE(this);

                btnCommitIMAGE.Enabled = true;
                CheckCommitButtons();
            }
            else if (btnPickColor.Checked)
            {
                TileEditor.iIndexPickedColor = TileEditor.TileMatrix[xCursor / TileEditor.iTETileSizeFill,
                                                                     yCursor / TileEditor.iTETileSizeFill];

                TileEditor.UpdatePickedColorValues(this);
            }
        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtPaletteID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtDestX_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtDestY_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtBlending_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtBlendMode_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtParam_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtState_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtTextureID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtSourceX_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtSourceY_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtTextureID2_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtSourceX2_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtSourceY2_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtBigID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtSourceXBigID_TextChanged(object sender, EventArgs e)
        {
            btnCommitINFO.Enabled = true;
            CheckCommitButtons();
        }

        private void txtSourceYBigID_TextChanged(object sender, EventArgs e)
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
                    TileEditor.TECommitINFO(this, frmAeris);
                    TileEditor.TECommitIMAGE(frmAeris);

                    // Refresh Texture and Background
                    frmAeris.RefreshBackground(TileEditor.iTETexture);
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
                        TileEditor.TECommitINFO(this, frmAeris);

                        // Refresh Texture and Background
                        frmAeris.RefreshBackground(TileEditor.iTETexture);
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
                        TileEditor.TECommitIMAGE(frmAeris);

                        // Refresh Texture and Background
                        frmAeris.RefreshBackground(TileEditor.iTETexture);
                    }
                }
            }

            return mbResult;
        }

        public void CheckNextPrevTileButtons()
        {
            btnNextTile.Enabled = true;
            btnPrevTile.Enabled = true;
            if (TileEditor.iTEAbsTileNum == 0)
            {
                btnPrevTile.Enabled = false;
                btnNextTile.Enabled = true;
            }

            // Let's check we don't go lower than Tile 0 and higher than Tile 256 or max tile.
            if (TileEditor.iTEAbsTileNum == S9.Section9.Textures[TileEditor.iTETexture].MaxTiles)
            {
                btnPrevTile.Enabled = true;
                btnNextTile.Enabled = false;
            }

//            TileEditor.iTEAbsTileNum = Int32.Parse(ufrmAeris.txtTileTex.Text);

            btnCommitALL.Enabled = false;
            btnCommitINFO.Enabled = false;
            btnCommitIMAGE.Enabled = false;
        }

        private void btnPrevTile_Click(object sender, EventArgs e)
        {
            DialogResult mbResult;

            mbResult = CommitChanges();
            if (mbResult == DialogResult.Cancel)
            {
                return;
            }

            frmAeris.btnTileLeft_Click(sender, new EventArgs());
            PopulateTE();
            CheckNextPrevTileButtons();
        }

        private void btnNextTile_Click(object sender, EventArgs e)
        {
            DialogResult mbResult;

            mbResult = CommitChanges();
            if (mbResult == DialogResult.Cancel)
            {
                return;
            }

            frmAeris.btnTileRight_Click(sender, new EventArgs());
            PopulateTE();
            CheckNextPrevTileButtons();
        }

        private void frmTileEditor_FormClosing(object sender, FormClosingEventArgs e)
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

        private void btnClose_Click(object sender, EventArgs e)
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

        private void btnCommitINFO_Click(object sender, EventArgs e)
        {
            if (TileEditor.TECommitINFO(this, frmAeris))
            {
                PopulateTE();

                btnCommitINFO.Enabled = false;
                CheckCommitButtons();

                // Refresh Texture and Background
                frmAeris.RefreshBackground(TileEditor.iTETexture);
            }
        }

        private void btnCommitIMAGE_Click(object sender, EventArgs e)
        {
            if (TileEditor.TECommitIMAGE(frmAeris))
            {
                PopulateTE();

                btnCommitIMAGE.Enabled = false;
                CheckCommitButtons();

                // Refresh Texture and Background
                frmAeris.RefreshBackground(TileEditor.iTETexture);
            }
        }

        private void btnCommitALL_Click(object sender, EventArgs e)
        {
            if (TileEditor.TECommitINFO(this, frmAeris))
            {
                if (TileEditor.TECommitIMAGE(frmAeris))
                {
                    PopulateTE();

                    btnCommitINFO.Enabled = false;
                    btnCommitIMAGE.Enabled = false;
                    btnCommitALL.Enabled = false;

                    // Refresh Texture and Background
                    frmAeris.RefreshBackground(TileEditor.iTETexture);
                }
            }
        }
    }
}
