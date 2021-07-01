using System;
using System.Drawing;
using System.Windows.Forms;


namespace Aeris
{
    public static class TileEditor
    {


        public const int TETILESIZE_WIDTH = 257;
        public const int TETILESIZE_HEIGHT = 257;

        public static bool bIsDirectTile, bActivateIFP;
        public static bool bPALInfoColorPALTEMouseMove, bPALInfoColorTEMouseMove;
        public static S9.dataTile dataTileTE, dataTileTEBackup;
        public static byte[,] TileMatrix;
        public static ushort[,] TileMatrix2Bytes;
        public static int iTELayer, iTETexture, iTEPalette;
        public static int iTETileNum, iTEAbsTileNum, iTESourceX, iTESourceY, iTETileSize, iTETileSizeFill;
        public static int iIndexPickedColor;

        public static void Load_TileMatrix()
        {
            int xPos, yPos;

            TileMatrix = new byte[iTETileSize, iTETileSize];

            for (yPos = 0; yPos < iTETileSize; yPos++)
            {
                for (xPos = 0; xPos < iTETileSize; xPos++)
                    TileMatrix[xPos, yPos] = S9.Section9.Textures[iTETexture].
                                                    textureMatrix[iTESourceX + xPos, iTESourceY + yPos];
            }
        }

        public static void Load_TileMatrix2Bytes()
        {
            int xPos, yPos;

            TileMatrix2Bytes = new ushort[iTETileSize, iTETileSize];

            for (yPos = 0; yPos < iTETileSize; yPos++)
            {
                for (xPos = 0; xPos < iTETileSize; xPos++)
                    TileMatrix2Bytes[xPos, yPos] = S9.Section9.Textures[iTETexture].
                                                    textureMatrix2Bytes[iTESourceX + xPos, iTESourceY + yPos];
            }
        }

        public static void UpdatePickedColorValues(frmTileEditor frmTileEditor)
        {
            int indexPAL;

            if (!bIsDirectTile)
            {
                indexPAL = iIndexPickedColor;

                if (frmTileEditor.cbActivateIFP.Enabled)
                {
                    if (frmTileEditor.cbActivateIFP.Checked)
                    {
                        if (Palette.indexFirstBlack == indexPAL) indexPAL = 0;
                    }
                    else
                    {
                        if (indexPAL == 0) indexPAL = Palette.indexFirstBlack;
                    }
                }

                frmTileEditor.txtIdx.Text = indexPAL.ToString();

                frmTileEditor.txtR.Text = S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Red.ToString();
                frmTileEditor.txtG.Text = S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Green.ToString();
                frmTileEditor.txtB.Text = S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Blue.ToString();
                frmTileEditor.txtM.Text = S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Mask.ToString();

                frmTileEditor.btnColor.BackColor = Palette.GetPalColor(iTEPalette, indexPAL);
            }
        }

        public static void InitializeTILEINFO(frmTileEditor frmTileEditor, frmAeris frmAeris)
        {
            frmTileEditor.txtTileLayer.Text = frmAeris.txtLayer.Text;
            frmTileEditor.txtTileNum.Text = frmAeris.txtTile.Text;

            iTELayer = Int32.Parse(frmAeris.txtLayer.Text);
            iTETileNum = Int32.Parse(frmAeris.txtTile.Text);
            iTEAbsTileNum = Int32.Parse(frmAeris.txtTileTex.Text);

            dataTileTE = S9.Section9.Layer[Int32.Parse(frmTileEditor.txtTileLayer.Text)].
                                    layerTiles[Int32.Parse(frmTileEditor.txtTileNum.Text)];
            dataTileTEBackup = dataTileTE;

            frmTileEditor.txtID.Text = dataTileTE.ID.ToString();
            frmTileEditor.txtPaletteID.Text = dataTileTE.paletteID.ToString();

            iTEPalette = Int32.Parse(frmTileEditor.txtPaletteID.Text);

            if (iTEPalette > S4.Section4.numPalettes - 1)
            {
                iTEPalette = 0;
            }

            frmTileEditor.txtWidth.Text = dataTileTE.Width.ToString();
            frmTileEditor.txtHeight.Text = dataTileTE.Height.ToString();
            frmTileEditor.txtDestX.Text = dataTileTE.destX.ToString();
            frmTileEditor.txtDestY.Text = dataTileTE.destY.ToString();
            frmTileEditor.txtDepth.Text = dataTileTE.depth.ToString();
            frmTileEditor.txtBlending.Text = dataTileTE.blending.ToString();
            frmTileEditor.txtBlendMode.Text = dataTileTE.BlendMode.ToString();
            frmTileEditor.txtParam.Text = dataTileTE.param.ToString();
            frmTileEditor.txtState.Text = dataTileTE.state.ToString();
            frmTileEditor.txtTextureID.Text = dataTileTE.textureID.ToString();
            frmTileEditor.txtSourceX.Text = dataTileTE.sourceX.ToString();
            frmTileEditor.txtSourceY.Text = dataTileTE.sourceY.ToString();

            iTETexture = Int32.Parse(frmTileEditor.txtTextureID.Text);
            iTESourceX = Int32.Parse(frmTileEditor.txtSourceX.Text);
            iTESourceY = Int32.Parse(frmTileEditor.txtSourceY.Text);

            frmTileEditor.txtTextureID2.Text = dataTileTE.textureID2.ToString();
            frmTileEditor.txtSourceX2.Text = dataTileTE.sourceX2.ToString();
            frmTileEditor.txtSourceY2.Text = dataTileTE.sourceY2.ToString();

            if (dataTileTE.textureID2 > 0)
            {
                iTETexture = Int32.Parse(frmTileEditor.txtTextureID2.Text);
                iTESourceX = Int32.Parse(frmTileEditor.txtSourceX2.Text);
                iTESourceY = Int32.Parse(frmTileEditor.txtSourceY2.Text);
            }

            frmTileEditor.txtBigID.Text = dataTileTE.bigID.ToString();
            frmTileEditor.txtSourceXBigID.Text = dataTileTE.sourceXBig.ToString();
            frmTileEditor.txtSourceYBigID.Text = dataTileTE.sourceYBig.ToString();

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

            if (S9.Section9.Textures[iTETexture].Depth > 1)
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
            UpdatePickedColorValues(frmTileEditor);
        }

        public static void InitializeTILEIMAGE(frmTileEditor frmTileEditor)
        {
            if (bIsDirectTile)
            {
                // Initialize Tile Image in PictureBox (for TileSize 32)
                Draw_TileDirect(ref frmTileEditor.pbTileEdit);

                frmTileEditor.txtPaletteID.Enabled = false;
            }
            else
            {
                // Initialize Palette PictureBox colors of Tile Editor
                ImageTools.ClearPictureBox(frmTileEditor.pbPalette, 1, null);
                Palette.Refresh_Palette(ref frmTileEditor.pbPalette, iTEPalette);

                // Initialize Tile Image in PictureBox (for TileSize 16)
                Draw_Tile(ref frmTileEditor.pbTileEdit, frmTileEditor);
                frmTileEditor.txtPaletteID.Enabled = true;
            }
        }

        public static void Draw_Tile(ref PictureBox pbInputTile, frmTileEditor frmTileEditor)
        {
            int x, y, indexPAL;
            SolidBrush sBrush = new SolidBrush(Color.Empty);

            // Dim BMP As Bitmap = New Bitmap(pbInputTile.Size.Width - 4, pbInputTile.Size.Height - 4)
            Bitmap tileBitmap = new Bitmap(TETILESIZE_WIDTH, TETILESIZE_HEIGHT);
            Pen p = new Pen(Brushes.Black);

            p.Color = Color.Black;

            // Create Image
            using (var g = Graphics.FromImage(tileBitmap))
            {
                for (y = 0; y <= iTETileSize; y++)
                {
                    for (x = 0; x <= iTETileSize; x++)
                    {
                        if (x < iTETileSize & y < iTETileSize)
                        {
                            // Get the Palette's Fill Color
                            indexPAL = TileMatrix[x, y];
                            sBrush.Color = Palette.ARGB_BASEPAL[iTEPalette].ARGB_COLORS[indexPAL];

                            if (frmTileEditor.cbActivateIFP.Checked)
                            {
                                if (S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Red == 0 &
                                    S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Green == 0 &
                                    S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Blue == 0 &
                                    S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Mask == 0)
                                {
                                    sBrush.Color = Palette.ARGB_BASEPAL[iTEPalette].ARGB_COLORS[0];
                                }

                                if (S9.Section9.pal_ignoreFirstPixel[iTEPalette] == 1 & indexPAL == 0)
                                {
                                    sBrush.Color = Color.FromArgb(0, sBrush.Color.R, sBrush.Color.G, sBrush.Color.B);
                                }
                            }

                            // Draw Rectangle as per Palette
                            g.FillRectangle(sBrush, x * iTETileSizeFill,
                                                    y * iTETileSizeFill,
                                                    iTETileSizeFill,
                                                    iTETileSizeFill);
                        }

                        // Draw a grid to do it cool
                        g.DrawRectangle(p, x * iTETileSizeFill,
                                           y * iTETileSizeFill,
                                           (x + 1) * iTETileSizeFill,
                                           (y + 1) * iTETileSizeFill);
                    }
                }
            }


            if (pbInputTile.Image != null) pbInputTile.Image.Dispose();

            pbInputTile.Image = new Bitmap(tileBitmap);

            p.Dispose();
            sBrush.Dispose();
            tileBitmap.Dispose();
        }

        public static void Draw_TileDirect(ref PictureBox pbInputTile)
        {
            int x;
            int y;

            SolidBrush sBrush = new SolidBrush(Color.Empty);

            // Dim BMP As Bitmap = New Bitmap(pbInputTile.Size.Width - 4, pbInputTile.Size.Height - 4)
            Bitmap tileBMP = new Bitmap(TETILESIZE_WIDTH, TETILESIZE_HEIGHT);
            Pen p = new Pen(Brushes.Black);

            p.Color = Color.Black;

            // Create Image
            using (var g = Graphics.FromImage(tileBMP))
            {
                for (y = 0; y <= iTETileSize; y++)
                {
                    for (x = 0; x <= iTETileSize; x++)
                    {
                        if (x < iTETileSize & y < iTETileSize)
                        {
                            // Get the Palette's Fill Color
                            sBrush.Color = Palette.Get16bitColor(TileMatrix2Bytes[x, y]);

                            // Draw Rectangle as per Palette
                            g.FillRectangle(sBrush, x * iTETileSizeFill, 
                                                    y * iTETileSizeFill, 
                                                    iTETileSizeFill, 
                                                    iTETileSizeFill);
                        }

                        // Draw a grid to do it cool
                         g.DrawRectangle(p, x * iTETileSizeFill, 
                                            y * iTETileSizeFill, 
                                            (x + 1) * iTETileSizeFill, 
                                            (y + 1) * iTETileSizeFill);
                    }
                }
            }

            if (pbInputTile.Image != null) pbInputTile.Image.Dispose();

            pbInputTile.Image = new Bitmap(tileBMP);

            p.Dispose();
            sBrush.Dispose();
            tileBMP.Dispose();
        }

        public static void StringMessage(string strMessage)
        {
            MessageBox.Show(strMessage, "Information", MessageBoxButtons.OK);
        }

        public static bool TECommitINFO(frmTileEditor frmTileEditor, frmAeris frmAeris)
        {
            int iValue = 0;
            string strcbTextures = "";

            // We need here to check the values before do the Commit.
            // Some values have some reasonable restrictions.

            // Check ID
            if (int.TryParse(frmTileEditor.txtID.Text, out iValue))
            {
                if (iValue < 0 | iValue > 4096)
                {
                    StringMessage("The ID value can not be negative or greater than 4096.");
                    frmTileEditor.txtID.Text = dataTileTEBackup.ID.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in ID.");
                frmTileEditor.txtID.Text = dataTileTEBackup.ID.ToString();
                return false;
            }


            // Check PaletteID
            if (int.TryParse(frmTileEditor.txtPaletteID.Text, out iValue))
            {
                if (iValue < 0 | iValue > S4.Section4.numPalettes - 1)
                {
                    StringMessage("The PaletteID value can not be negative or greater than " + 
                                  "the current number of Palettes.");
                    frmTileEditor.txtPaletteID.Text = dataTileTEBackup.paletteID.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in PaletteID.");
                frmTileEditor.txtPaletteID.Text = dataTileTEBackup.paletteID.ToString();
                return false;
            }


            // Check DestX
            if (int.TryParse(frmTileEditor.txtDestX.Text, out iValue))
            {
                if (iValue < -32768 | iValue > 32767)
                {
                    StringMessage("The DestX value can not be lower than -32768 or greater than 32767.");
                    frmTileEditor.txtDestX.Text = dataTileTEBackup.destX.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in DestX.");
                frmTileEditor.txtDestX.Text = dataTileTEBackup.destX.ToString();
                return false;
            }


            // Check DestY
            if (int.TryParse(frmTileEditor.txtDestY.Text, out iValue))
            {
                if (iValue < -32768 | iValue > 32767)
                {
                    StringMessage("The DestY value can not be lower than -32768 or greater than 32767.");
                    frmTileEditor.txtDestY.Text = dataTileTEBackup.destY.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in DestY.");
                frmTileEditor.txtDestY.Text = dataTileTEBackup.destY.ToString();
                return false;
            }


            // Check Blending
            if (int.TryParse(frmTileEditor.txtBlending.Text, out iValue))
            {
                if (iValue < 0 | iValue > 1)
                {
                    StringMessage("The Blending value can only be 0 or 1.");
                    frmTileEditor.txtBlending.Text = dataTileTEBackup.blending.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in Blending.");
                frmTileEditor.txtBlending.Text = dataTileTEBackup.blending.ToString();
                return false;
            }


            // Check BlendingMode
            if (int.TryParse(frmTileEditor.txtBlendMode.Text, out iValue))
            {
                if (iValue < 0 | iValue > 3)
                {
                    StringMessage("The Blending Mode value can only be 0...3.");
                    frmTileEditor.txtBlendMode.Text = dataTileTEBackup.BlendMode.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in Blending Mode.");
                frmTileEditor.txtBlendMode.Text = dataTileTEBackup.BlendMode.ToString();
                return false;
            }


            // Check Param
            if (int.TryParse(frmTileEditor.txtParam.Text, out iValue))
            {
                if (iValue < 0 | iValue > 255)
                {
                    StringMessage("The Param value can only be 0...255.");
                    frmTileEditor.txtParam.Text = dataTileTEBackup.param.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in Param.");
                frmTileEditor.txtParam.Text = dataTileTEBackup.param.ToString();
                return false;
            }


            // Check State
            if (int.TryParse(frmTileEditor.txtState.Text, out iValue))
            {
                if (iValue < 0 | iValue > 255)
                {
                    StringMessage("The State value can only be 0...255.");
                    frmTileEditor.txtState.Text = dataTileTEBackup.state.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in State.");
                frmTileEditor.txtState.Text = dataTileTEBackup.state.ToString();
                return false;
            }


            // Check TextureID
            if (int.TryParse(frmTileEditor.txtTextureID.Text, out iValue))
            {
                if (iValue < 0 | iValue > S9.MAX_NUM_TEXTURES)
                {
                    StringMessage("The TextureID value can only be 0..." + S9.MAX_NUM_TEXTURES + ".");
                    frmTileEditor.txtTextureID.Text = dataTileTEBackup.textureID.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in TextureID.");
                frmTileEditor.txtTextureID.Text = dataTileTEBackup.textureID.ToString();
                return false;
            }

            // Check TextureID2
            if (int.TryParse(frmTileEditor.txtTextureID2.Text, out iValue))
            {
                if (iValue < 0 | iValue > S9.MAX_NUM_TEXTURES)
                {
                    StringMessage("The TextureID2 value can only be 0..." + S9.MAX_NUM_TEXTURES + ".");
                    frmTileEditor.txtTextureID2.Text = dataTileTEBackup.textureID2.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in TextureID2.");
                frmTileEditor.txtTextureID2.Text = dataTileTEBackup.textureID2.ToString();
                return false;
            }

            // Check TextureID and/or TextureID2 existance.
            if (Int32.Parse(frmTileEditor.txtTextureID2.Text) > 0)
            {
                strcbTextures = frmAeris.cbTextures.Items[Int32.Parse(frmTileEditor.txtTextureID2.Text)].ToString();

                if (strcbTextures.IndexOf("-") > 0)
                {
                    StringMessage("There is not any texture in the TextureID2 assigned.");
                    frmTileEditor.txtTextureID2.Text = dataTileTEBackup.textureID2.ToString();
                    return false;
                }
            }
            else
            {
                strcbTextures = frmAeris.cbTextures.Items[Int32.Parse(frmTileEditor.txtTextureID.Text)].ToString();

                if (strcbTextures.IndexOf("-") > 0)
                {
                    StringMessage("There is not any texture in the TextureID assigned.");
                    frmTileEditor.txtTextureID.Text = dataTileTEBackup.textureID.ToString();
                    return false;
                }
            }


            // Check SourceX
            if (int.TryParse(frmTileEditor.txtSourceX.Text, out iValue))
            {
                if (iValue < 0 | iValue > 240)
                {
                    if (iValue % 16 != 0)
                    {
                        StringMessage("The SourceX value must be a multiple of 16.");
                        frmTileEditor.txtSourceX.Text = dataTileTEBackup.sourceX.ToString();
                        return false;
                    }

                    StringMessage("The SourceX value can only be 0...240.");
                    frmTileEditor.txtSourceX.Text = dataTileTEBackup.sourceX.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in SourceX.");
                frmTileEditor.txtSourceX.Text = dataTileTEBackup.sourceX.ToString();
                return false;
            }


            // Check SourceY
            if (int.TryParse(frmTileEditor.txtSourceY.Text, out iValue))
            {
                if (iValue < 0 | iValue > 240)
                {
                    if (iValue % 16 != 0)
                    {
                        StringMessage("The SourceY value must be a multiple of 16.");
                        frmTileEditor.txtSourceY.Text = dataTileTEBackup.sourceY.ToString();
                        return false;
                    }

                    StringMessage("The SourceY value can only be 0...240.");
                    frmTileEditor.txtSourceY.Text = dataTileTEBackup.sourceY.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in SourceY.");
                frmTileEditor.txtSourceY.Text = dataTileTEBackup.sourceY.ToString();
                return false;
            }


            // Check SourceX2
            if (int.TryParse(frmTileEditor.txtSourceX2.Text, out iValue))
            {
                if (iValue < 0 | iValue > 240)
                {
                    if (iValue % 16 != 0)
                    {
                        StringMessage("The SourceX2 value must be a multiple of 16.");
                        frmTileEditor.txtSourceX2.Text = dataTileTEBackup.sourceX2.ToString();
                        return false;
                    }

                    StringMessage("The SourceX2 value can only be 0...240.");
                    frmTileEditor.txtSourceX2.Text = dataTileTEBackup.sourceX2.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in SourceX2.");
                frmTileEditor.txtSourceX2.Text = dataTileTEBackup.sourceX2.ToString();
                return false;
            }


            // Check SourceY2
            if (int.TryParse(frmTileEditor.txtSourceY2.Text, out iValue))
            {
                if (iValue < 0 | iValue > 240)
                {
                    if (iValue % 16 != 0)
                    {
                        StringMessage("The SourceY2 value must be a multiple of 16.");
                        frmTileEditor.txtSourceY2.Text = dataTileTEBackup.sourceY2.ToString();
                        return false;
                    }

                    StringMessage("The SourceY2 value can only be 0...240.");
                    frmTileEditor.txtSourceY2.Text = dataTileTEBackup.sourceY2.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in SourceY2.");
                frmTileEditor.txtSourceY2.Text = dataTileTEBackup.sourceY2.ToString();
                return false;
            }


            // Check BigID
            if (int.TryParse(frmTileEditor.txtBigID.Text, out iValue))
            {
                if (iValue < 0)
                {
                    StringMessage("The BigID can not be negative.");
                    frmTileEditor.txtBigID.Text = dataTileTEBackup.bigID.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in BigID.");
                frmTileEditor.txtBigID.Text = dataTileTEBackup.bigID.ToString();
                return false;
            }


            // Check SourceXBig
            if (int.TryParse(frmTileEditor.txtSourceXBigID.Text, out iValue))
            {
                if (iValue < 0)
                {
                    StringMessage("The SourceX BigID can not be negative.");
                    frmTileEditor.txtSourceXBigID.Text = dataTileTEBackup.sourceXBig.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in SourceX BigID.");
                frmTileEditor.txtSourceXBigID.Text = dataTileTEBackup.sourceXBig.ToString();
                return false;
            }


            // Check SourceYBig
            if (int.TryParse(frmTileEditor.txtSourceYBigID.Text, out iValue))
            {
                if (iValue < 0)
                {
                    StringMessage("The SourceY BigID can not be negative.");
                    frmTileEditor.txtSourceYBigID.Text = dataTileTEBackup.sourceYBig.ToString();
                    return false;
                }
            }
            else
            {
                StringMessage("You can not put alphanumeric values in SourceY BigID.");
                frmTileEditor.txtSourceYBigID.Text = dataTileTEBackup.sourceYBig.ToString();
                return false;
            }


            // Update dataTile DATA.
            dataTileTE.ID = UInt16.Parse(frmTileEditor.txtID.Text);
            dataTileTEBackup.ID = dataTileTE.ID;
            dataTileTE.paletteID = Byte.Parse(frmTileEditor.txtPaletteID.Text);
            dataTileTEBackup.paletteID = dataTileTE.paletteID;
            dataTileTE.destX = Int16.Parse(frmTileEditor.txtDestX.Text);
            dataTileTEBackup.destX = dataTileTE.destX;
            dataTileTE.destY = Int16.Parse(frmTileEditor.txtDestY.Text);
            dataTileTEBackup.destY = dataTileTE.destY;
            dataTileTE.blending = Byte.Parse(frmTileEditor.txtBlending.Text);
            dataTileTEBackup.blending = dataTileTE.blending;
            dataTileTE.BlendMode = Byte.Parse(frmTileEditor.txtBlendMode.Text);
            dataTileTEBackup.BlendMode = dataTileTE.BlendMode;
            dataTileTE.param = Byte.Parse(frmTileEditor.txtParam.Text);
            dataTileTEBackup.param = dataTileTE.param;
            dataTileTE.state = Byte.Parse(frmTileEditor.txtState.Text);
            dataTileTEBackup.state = dataTileTE.state;
            dataTileTE.statePow2 = (byte)(Math.Pow(2, dataTileTE.state));
            dataTileTEBackup.statePow2 = dataTileTE.statePow2;
            dataTileTE.textureID = Byte.Parse(frmTileEditor.txtTextureID.Text);
            dataTileTEBackup.textureID = dataTileTE.textureID;
            dataTileTE.textureID2 = Byte.Parse(frmTileEditor.txtTextureID2.Text);
            dataTileTEBackup.textureID2 = dataTileTE.textureID2;
            dataTileTE.sourceX = Byte.Parse(frmTileEditor.txtSourceX.Text);
            dataTileTEBackup.sourceX = dataTileTE.sourceX;
            dataTileTE.sourceY = Byte.Parse(frmTileEditor.txtSourceY.Text);
            dataTileTEBackup.sourceY = dataTileTE.sourceY;
            dataTileTE.sourceX2 = Byte.Parse(frmTileEditor.txtSourceX2.Text);
            dataTileTEBackup.sourceX2 = dataTileTE.sourceX2;
            dataTileTE.sourceY2 = Byte.Parse(frmTileEditor.txtSourceY2.Text);
            dataTileTEBackup.sourceY2 = dataTileTE.sourceY2;
            dataTileTE.bigID = UInt32.Parse(frmTileEditor.txtBigID.Text);
            dataTileTEBackup.bigID = dataTileTE.bigID;
            dataTileTE.sourceXBig = UInt32.Parse(frmTileEditor.txtSourceXBigID.Text);
            dataTileTEBackup.sourceXBig = dataTileTE.sourceXBig;
            dataTileTE.sourceYBig = UInt32.Parse(frmTileEditor.txtSourceYBigID.Text);
            dataTileTEBackup.sourceYBig = dataTileTE.sourceYBig;


            // Now that we have verified the values, we need to update
            // the original Tile of Section9.
            S9.Section9.Layer[iTELayer].layerTiles[iTETileNum] = dataTileTE;

            // Update Aeris Title
            FileTools.bFieldModified = true;
            frmAeris.Update_AerisTitle();
            return true;
        }

        public static bool TECommitIMAGE(frmAeris frmAeris)
        {
            int ixPos, iyPos, ixSource, iySource, iTexture;
            try
            {
                // Here we will update the original TextureMatrix with the TileMatrix new values.
                if (dataTileTEBackup.textureID2 > 0)
                {
                    iTexture = dataTileTEBackup.textureID2;
                    ixSource = dataTileTEBackup.sourceX2;
                    iySource = dataTileTEBackup.sourceY2;
                }
                else
                {
                    iTexture = dataTileTEBackup.textureID;
                    ixSource = dataTileTEBackup.sourceX;
                    iySource = dataTileTEBackup.sourceY;
                }

                for (iyPos = 0; iyPos < iTETileSize; iyPos++)
                {
                    for (ixPos = 0; ixPos < iTETileSize; ixPos++)
                        S9.Section9.Textures[iTexture].
                                    textureMatrix[ixSource + ixPos, iySource + iyPos] = TileMatrix[ixPos, iyPos];
                }

                // Update Aeris Title
                FileTools.bFieldModified = true;

                frmAeris.Update_AerisTitle();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}