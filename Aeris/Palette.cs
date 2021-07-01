using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;


namespace Aeris
{

    public class Palette
    {

        public const int MAX_PAL_COLORS = 256;
        public const int COEF = 8;

        public static int indexFirstBlack;


        public partial struct ARGB_PALETTE
        {
            public List<Color> ARGB_COLORS;
        }

        public static List<ARGB_PALETTE> ARGB_BASEPAL;

        public static bool IsBlack16Bit(Color tmpColor)
        {
            if (tmpColor.R == 0 & tmpColor.G == 0 & tmpColor.B == 0)
            {
                return true;
            }

            return false;
        }

        public static Color GetPalColor(int iPalette, int iColor)
        {
            Color PALColor;

            
            PALColor = Color.FromArgb(255, S4.Section4.dataPalette[iPalette].Pal[iColor].Red,
                                           S4.Section4.dataPalette[iPalette].Pal[iColor].Green,
                                           S4.Section4.dataPalette[iPalette].Pal[iColor].Blue);

            return PALColor;
        }


        public static void Clear_BASEARGB()
        {
            if (ARGB_BASEPAL != null)
            {
                foreach (var itmABPAL in ARGB_BASEPAL) itmABPAL.ARGB_COLORS.Clear();
                ARGB_BASEPAL.Clear();

                ARGB_BASEPAL = null;
            }
        }


        public static void Load_BASEARGB()
        {
            int iPalette, iColor;
            ARGB_PALETTE APAL;

            // Clear ARGB_BASEPAL.
            Clear_BASEARGB();

            ARGB_BASEPAL = new List<ARGB_PALETTE>();

            // Prepare Palettes
            for (iPalette = 0; iPalette < S4.Section4.numPalettes; iPalette++)
            {
                APAL = new ARGB_PALETTE();
                APAL.ARGB_COLORS = new List<Color>();

                // Add each color to the ARGB_COLORS
                for (iColor = 0; iColor < MAX_PAL_COLORS; iColor++)
                {
                    APAL.ARGB_COLORS.Add(GetPalColor(iPalette, iColor));
                }
              
                ARGB_BASEPAL.Add(APAL);
            }
        }

        public static Color Get16bitColor(ushort c16bit)
        {
            Color retColor;

            retColor = Color.FromArgb(255, 
                                      (c16bit >> 11 & 0x1F) * COEF, 
                                      (c16bit >> 6 & 0x1F) * COEF, 
                                      (c16bit & 0x1F) * COEF);

            if (retColor.R == 0 & retColor.G == 0 & retColor.B == 0)
            {
                retColor = Color.FromArgb(0, retColor.R, retColor.G, retColor.B);
            }

            return retColor;
        }

        public static ushort Put16bitColor(Color cInputColor)
        {
            ushort retShort;

            if (cInputColor.A == 0)
            {
                retShort = 0;
            }
            else
            {
                retShort = (ushort)((cInputColor.R) / COEF << 11 |
                                    (cInputColor.G) / COEF << 6 |
                                    (cInputColor.B) / COEF);
            }

            return retShort;
        }

        public static byte Get8bppIndexedPixel(Bitmap tileBMP, int xPixel, int yPixel)
        {
            BitmapData bmpData;
            byte b;
            bmpData = tileBMP.LockBits(new Rectangle(new Point(xPixel, yPixel), new Size(1, 1)), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            b = Marshal.ReadByte(bmpData.Scan0);
            tileBMP.UnlockBits(bmpData);
            return b;
        }

        public static void Set8bppIndexedPixel(ref Bitmap tileBMP, int x, int y, byte pixelValue)
        {
            BitmapData bmpData;
            bmpData = tileBMP.LockBits(new Rectangle(x, y, 1, 1), ImageLockMode.ReadOnly, tileBMP.PixelFormat);
            Marshal.WriteByte(bmpData.Scan0, pixelValue);
            tileBMP.UnlockBits(bmpData);
        }

        public static Color BlendColor(Color bgtileColor, Color tileColor, int BlendMode)
        {
            int tileR, tileG, tileB;
            int bgtileR, bgtileG, bgtileB;

            tileR = (int)tileColor.R;
            tileG = (int)tileColor.G;
            tileB = (int)tileColor.B;
            bgtileR = (int)bgtileColor.R;
            bgtileG = (int)bgtileColor.G;
            bgtileB = (int)bgtileColor.B;

            switch (BlendMode)
            {
                case 1:
                    {
                        tileR = Math.Min(255, bgtileR + tileR);
                        tileG = Math.Min(255, bgtileG + tileG);
                        tileB = Math.Min(255, bgtileB + tileB);
                        break;
                    }

                case 2:
                    {
                        tileR = Math.Max(0, bgtileR - tileR);
                        tileG = Math.Max(0, bgtileG - tileG);
                        tileB = Math.Max(0, bgtileB - tileB);
                        break;
                    }

                case 3:
                    {
                        tileR = Math.Min(255, bgtileR + tileR / 4);
                        tileG = Math.Min(255, bgtileG + tileG / 4);
                        tileB = Math.Min(255, bgtileB + tileB / 4);
                        break;
                    }

                default:
                    {
                        tileR = (bgtileR + tileR) / 2;
                        tileG = (bgtileG + tileG) / 2;
                        tileB = (bgtileB + tileB) / 2;
                        break;
                    }
            }

            tileColor = Color.FromArgb(255, tileR, tileG, tileB);
            return tileColor;
        }


        //public static void Render_Tile(ref Bitmap bmpBgTile, Bitmap bmpBlendTile, S9.dataTile dataTile)
        public static void Render_Tile(ref DirectBitmap bmpBgTile, Bitmap bmpBlendTile, S9.dataTile dataTile)
        {
            int xTile, yTile;
            Color tilePixelColor, bgPixelColor;

            for (yTile = 0; yTile < dataTile.imgTile.Height; yTile++)
            {

                for (xTile = 0; xTile < dataTile.imgTile.Width; xTile++)
                {
                    // Get Tile and Background Colors
                    // If Black or Alpha Tile, jump
                    tilePixelColor = bmpBlendTile.GetPixel(xTile, yTile);
                    if (!IsBlack16Bit(tilePixelColor) & tilePixelColor.A > 0)
                    {
                        bgPixelColor = bmpBgTile.GetPixel(xTile, yTile);

                        // Blend the Color
                        bgPixelColor = BlendColor(bgPixelColor, tilePixelColor, dataTile.BlendMode);

                        // Put the Color in the tile image of the background
                        bmpBgTile.SetPixel(xTile, yTile, bgPixelColor);
                    }
                }
            }
        }

        public static void Refresh_Palette(ref PictureBox pbInput, int PaletteID)
        {
            long x;
            long y;
            var customColor = default(Color);
            var sBrush = new SolidBrush(customColor);
            var BMP = new Bitmap(pbInput.Size.Width, pbInput.Size.Height);
            Graphics G = Graphics.FromImage(BMP);
            var P = new Pen(Brushes.Black);

            // Create Image
            for (y = 0L; y <= 16L; y++)
            {
                for (x = 0L; x <= 16L; x++)
                {
                    if (x < 16L & y < 16L)
                    {
                        // Get the Palette's Fill Color
                        if (y == 0L)
                        {
                            sBrush.Color = ARGB_BASEPAL[PaletteID].ARGB_COLORS[(int)(x + y)];
                        }
                        else
                        {
                            sBrush.Color = ARGB_BASEPAL[PaletteID].ARGB_COLORS[(int)(x + y * 16L)];
                            if (x + y * 16L > 253L)
                            {
                                sBrush.Color = ARGB_BASEPAL[PaletteID].ARGB_COLORS[(int)(x + y * 16L)];
                            }
                        }

                        // Draw Rectangle as per Palette
                        G.FillRectangle(sBrush, x * 16L, y * 16L, (x + 1L) * 16L, (y + 1L) * 16L);
                    }

                    // Draw a grid to do it cool
                    P.Color = Color.Black;
                    G.DrawRectangle(P, x * 16L, y * 16L, (x + 1L) * 16L, (y + 1L) * 16L);
                }
            }

            if (pbInput.Image != null)
            {
                pbInput.Image.Dispose();
            }
            pbInput.Image = new Bitmap(BMP);

            P.Dispose();
            sBrush.Dispose();
            BMP.Dispose();
        }

        public static void GetIndexFirstBlack(int iPalette)
        {
            bool bFound = false;
            int iColor = 1;

            if (iPalette < S4.Section4.numPalettes)
            {
                while (iColor < Palette.MAX_PAL_COLORS && !bFound)
                {
                    if (S4.Section4.dataPalette[iPalette].Pal[iColor].Red == 0 &&
                        S4.Section4.dataPalette[iPalette].Pal[iColor].Green == 0 &&
                        S4.Section4.dataPalette[iPalette].Pal[iColor].Blue == 0)

                        bFound = true;
                    else
                        iColor++;
                }
            }

            if (!bFound) indexFirstBlack = 0;
            else indexFirstBlack = iColor;
        }

        public static bool IsFirstIndexColor(int iPalette, Color tmpColor)
        {
            return (ARGB_BASEPAL[iPalette].ARGB_COLORS[0].R == tmpColor.R &&
                    ARGB_BASEPAL[iPalette].ARGB_COLORS[0].G == tmpColor.G &&
                    ARGB_BASEPAL[iPalette].ARGB_COLORS[0].B == tmpColor.B);
        }

        public static int GetIndexOfColor(int iPalette, Color tmpColor, ref byte byteIndex)
        {
            int iResult, iNumColor;
            bool bFound;

            iResult = 0;
            bFound = false;
            iNumColor = 0;

            while (iNumColor < MAX_PAL_COLORS && !bFound)
            {
                if (ARGB_BASEPAL[iPalette].ARGB_COLORS[iNumColor] == tmpColor)
                    bFound = true;
                else
                    iNumColor++;
            }

            if (!bFound)
                iResult = 2;
            else
            {
                if (S9.Section9.pal_ignoreFirstPixel[iPalette] == 1)
                    if (iNumColor == 0)
                            iNumColor = indexFirstBlack;

                byteIndex = (byte)iNumColor;
            }

            return iResult;
        }

        public static void ExportMSPAL(string FileName, int PaletteID)
        {
            uint length, offset;
            ushort chunk, i;
            byte[] buffer;
            char aux;
            length = 4 + 4 + 4 + 4 + 4 + 2 + 2 + MAX_PAL_COLORS * 4;
            chunk = MAX_PAL_COLORS * 4 + 4;
            buffer = new byte[(int)(length - 1L + 1)];
            aux = 'R';
            buffer[0] = Convert.ToByte(aux);
            aux = 'I';
            buffer[1] = Convert.ToByte(aux);
            aux = 'F';
            buffer[2] = Convert.ToByte(aux);
            aux = 'F';
            buffer[3] = Convert.ToByte(aux);
            buffer[4] = (byte)(length - 8L & 0xFFL);
            buffer[5] = (byte)(length - 8L >> 8 & 0xFFL);
            buffer[6] = (byte)(length - 8L >> 16 & 0xFFL);
            buffer[7] = (byte)(length - 8L >> 24 & 0xFFL);
            aux = 'P';
            buffer[8] = Convert.ToByte(aux);
            aux = 'A';
            buffer[9] = Convert.ToByte(aux);
            aux = 'L';
            buffer[10] = Convert.ToByte(aux);
            aux = ' ';
            buffer[11] = Convert.ToByte(aux);
            aux = 'd';
            buffer[12] = Convert.ToByte(aux);
            aux = 'a';
            buffer[13] = Convert.ToByte(aux);
            aux = 't';
            buffer[14] = Convert.ToByte(aux);
            aux = 'a';
            buffer[15] = Convert.ToByte(aux);
            buffer[16] = (byte)(chunk & 0xFF);
            buffer[17] = (byte)(chunk >> 8 & 0xFF);
            buffer[18] = 0;
            buffer[19] = 0;
            buffer[20] = 0;
            buffer[21] = 3;
            buffer[22] = MAX_PAL_COLORS & 0xFF;
            buffer[23] = MAX_PAL_COLORS >> 8 & 0xFF;
            for (i = 0; i <= MAX_PAL_COLORS - 1; i++)
            {
                offset = (uint)(24 + i * 4);
                buffer[(int)offset] = ARGB_BASEPAL[PaletteID].ARGB_COLORS[i].R;
                buffer[(int)(offset + 1L)] = ARGB_BASEPAL[PaletteID].ARGB_COLORS[i].G;
                buffer[(int)(offset + 2L)] = ARGB_BASEPAL[PaletteID].ARGB_COLORS[i].B;
                buffer[(int)(offset + 3L)] = 0;
            }

            using (var writer = new BinaryWriter(File.Open(FileName, FileMode.Create)))
            {
                // Write WPAL.
                writer.Write(buffer);
            }
        }

        public static void ExportGIMPPAL(string FileName, int PaletteID)
        {
            FileStream fs = null;
            fs = new FileStream(FileName, FileMode.Create);
            using (var writer = new StreamWriter(fs, Encoding.ASCII))
            {
                // Write header.
                writer.Write("GIMP Palette" + "\n");
                writer.Write("Name: FF7 Palette" + "\n");
                writer.Write("Columns: 16" + "\n");
                writer.Write("#" + "\n");
                writer.Write("# FF7 Palette to GIMP Palette" + "\n");
                writer.Write("# FF7 Graph Tool - by L@Zar0" + "\n");
                writer.Write("#" + "\n");

                // Write each color
                int i;
                for (i = 0; i <= MAX_PAL_COLORS - 1; i++)
                    writer.Write(Convert.ToString(ARGB_BASEPAL[PaletteID].ARGB_COLORS[i].R).PadLeft(3, ' ') + " " +
                                 Convert.ToString(ARGB_BASEPAL[PaletteID].ARGB_COLORS[i].G).PadLeft(3, ' ') + " " +
                                 Convert.ToString(ARGB_BASEPAL[PaletteID].ARGB_COLORS[i].B).PadLeft(3, ' ') + "\n");
            }

            fs.Close();
        }

        public static int ImportMSPAL(string strFileName, int iPalette)
        {
            int iResult;
            uint offset;
            byte[] buffer;
            iResult = 0;
            using (var reader = new BinaryReader(File.Open(strFileName, FileMode.Open)))
            {
                buffer = new byte[(int)(reader.BaseStream.Length - 1L + 1)];

                // Read MSPAL.
                reader.Read(buffer, 0, (int)reader.BaseStream.Length);
            }

            // Now we need to put the data into the S4_Palette section.
            for (int i = 0; i <= MAX_PAL_COLORS - 1; i++)
            {
                offset = (uint)(24 + i * 4);
                S4.Section4.dataPalette[iPalette].Pal[i].Red = buffer[(int)offset];
                S4.Section4.dataPalette[iPalette].Pal[i].Green = buffer[(int)(offset + 1L)];
                S4.Section4.dataPalette[iPalette].Pal[i].Blue = buffer[(int)(offset + 2L)];
            }

            return iResult;
        }
    }
}