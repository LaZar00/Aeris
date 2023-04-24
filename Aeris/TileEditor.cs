using System;
using System.Drawing;
using System.Windows.Forms;


namespace Aeris
{

    using static S9;

    using static Palette;
    using static FileTools;

    public static class TileEditor
    {

        public const int TETILESIZE_WIDTH = 257;
        public const int TETILESIZE_HEIGHT = 257;

        public static bool bIsDirectTile, bActivateIFP;
        public static bool bPALInfoColorPALTEMouseMove, bPALInfoColorTEMouseMove;
        public static DataTile dataTileTE, dataTileTEBackup;
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
                    TileMatrix[xPos, yPos] = Section9.Textures[iTETexture].
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
                    TileMatrix2Bytes[xPos, yPos] = Section9.Textures[iTETexture].
                                                   textureMatrix2Bytes[iTESourceX + xPos, iTESourceY + yPos];
            }
        }


        public static void Draw_Tile(ref PictureBox pbInputTile, bool bActivateIFP)
        {
            int x, y, indexPAL;
            SolidBrush sBrush = new SolidBrush(Color.Empty);

            // Dim BMP As Bitmap = New Bitmap(pbInputTile.Size.Width - 4, pbInputTile.Size.Height - 4)
            Bitmap tileBitmap = new Bitmap(TETILESIZE_WIDTH, TETILESIZE_HEIGHT);
            Pen p = new Pen(Brushes.Black)
            {
                Color = Color.Black,
            };

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
                            sBrush.Color = ARGB_BASEPAL[iTEPalette].ARGB_COLORS[indexPAL];

                            if (bActivateIFP)
                            {
                                if (S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Red == 0 &
                                    S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Green == 0 &
                                    S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Blue == 0 &
                                    S4.Section4.dataPalette[iTEPalette].Pal[indexPAL].Mask == 0)
                                {
                                    sBrush.Color = ARGB_BASEPAL[iTEPalette].ARGB_COLORS[0];
                                }

                                if (Section9.pal_ignoreFirstPixel[iTEPalette] == 1 & indexPAL == 0)
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
            Pen p = new Pen(Brushes.Black)
            {
                Color = Color.Black,
            };

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
                            sBrush.Color = Get16bitColor(TileMatrix2Bytes[x, y]);

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

        public static bool TECommitINFO(FrmTileEditor frmTileEditor, FrmAeris frmAeris)
        {
            string strcbTextures;

            // We need here to check the values before do the Commit.
            // Some values have some reasonable restrictions.

            // Check ID
            if (int.TryParse(frmTileEditor.txtID.Text, out int iValue))
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
                if (iValue < 0 | iValue > MAX_NUM_TEXTURES)
                {
                    StringMessage("The TextureID value can only be 0..." + MAX_NUM_TEXTURES + ".");
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
                if (iValue < 0 | iValue > MAX_NUM_TEXTURES)
                {
                    StringMessage("The TextureID2 value can only be 0..." + MAX_NUM_TEXTURES + ".");
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
            Section9.Layer[iTELayer].layerTiles[iTETileNum] = dataTileTE;

            // Update Aeris Title
            bFieldModified = true;
            frmAeris.Update_AerisTitle();
            return true;
        }

        public static bool TECommitIMAGE(FrmAeris frmAeris)
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
                        Section9.Textures[iTexture].
                                    textureMatrix[ixSource + ixPos, iySource + iyPos] = TileMatrix[ixPos, iyPos];
                }

                // Update Aeris Title
                bFieldModified = true;

                frmAeris.Update_AerisTitle();
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                return false;
            }

            return true;
        }
    }
}