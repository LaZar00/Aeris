using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Aeris
{
    public static class ImageTools
    {

        public static float HORZ_RES;
        public static float VERT_RES;

        public static Cursor PickerCUR;
        public static Cursor PencilCUR;
        public static Cursor CrossCUR;
        public static Cursor HandCUR;
        public static Cursor EditCUR;
        public static Cursor MoveCUR;

        public partial struct ImageRGBColors
        {
            public HashSet<int> RGBUnswizzled;
            public HashSet<int> RGBSwizzled;
            public string strFile;
            public int Layer;
            public HashSet<int> TileID;
            public int FirstTexture;
            public int FirstPalette;
            public int FirstTileID;
            public int Texture;
            public int Palette;
            public int Param;
            public int State;
            public bool Used;
        }


        // lstImagesRGBColors -> This helps on the Unswizzle proces and maintains a list of colors
        // for a given Unswizzled texture.
        public static List<ImageRGBColors> lstImagesRGBColors = new List<ImageRGBColors>();
        public static bool FillBGBlackStage, FillBGBlackImage, FillBGTransparent;
        public static bool bRenderEffects;


        // For type we have: 0-pbbackground, 1-pbtexture, 2-pbtile
        public static void ClearPictureBox(PictureBox pbInputPictureBox, int itype, Panel pinPanel)
        {
            Graphics g;
            switch (itype)
            {
                case 0:
                    {
                        if (pbInputPictureBox.Image != null)
                        {
                            g = Graphics.FromImage(pbInputPictureBox.Image);
                            g.Clear(Color.Transparent);
                        }

                        if (FillBGBlackStage)
                        {
                            pbInputPictureBox.BackColor = Color.Black;
                            pinPanel.BackColor = Color.Black;
                        }
                        else
                        {
                            pbInputPictureBox.BackColor = Color.Transparent;
                            pinPanel.BackColor = Color.Transparent;
                        }

                        break;
                    }

                case 1:
                    {
                        if (pbInputPictureBox.Image != null)
                        {
                            g = Graphics.FromImage(pbInputPictureBox.Image);
                            g.Clear(Color.Transparent);
                        }

                        break;
                    }

                case 2:
                    {
                        if (pbInputPictureBox.Image != null)
                        {
                            g = Graphics.FromImage(pbInputPictureBox.Image);
                            g.Clear(Color.Empty);
                        }

                        break;
                    }
            }

            pbInputPictureBox.Invalidate();
        }

        public static void GetListOfColors(Bitmap bmpUnsTexture, ref HashSet<int> iRGB)
        {
            int xPos, yPos;
            Color cColor;

            for (yPos = 0; yPos < bmpUnsTexture.Height; yPos++)
            {

                for (xPos = 0; xPos < bmpUnsTexture.Width; xPos++)
                {
                    cColor = bmpUnsTexture.GetPixel(xPos, yPos);

                    // Let's not count White/Black/Transparent.
                    if (!(cColor.R == 0 & cColor.G == 0 & cColor.B == 0 |
                          cColor.R == 255 & cColor.G == 255 & cColor.B == 255 |
                          cColor.A == 0))
                        iRGB.Add(ColorTranslator.ToWin32(cColor));
                }
            }
        }

        public static void AddRGBColorsToList(Bitmap bmpSwizzledTexture, 
                                              Bitmap bmpUnswizzledTexture, 
                                              string strUnsFile, 
                                              int iLayer, 
                                              int iFirstTexture, 
                                              int iFirstPalette, 
                                              int iFirstTileID, 
                                              int iTexture, 
                                              int iPalette, 
                                              SortedList<int, string> slTileID, 
                                              int iParam, 
                                              int iState)
        {
            ImageRGBColors stImageRGBColors = new ImageRGBColors();
            bool bUpdate = false;
            int iUpdateIndex, iUpdateTileIndex;


            // With the TileIDs as a Hash List, we need to check first if there is any.
            iUpdateIndex = 0;
            iUpdateTileIndex = 0;

            while (iUpdateIndex < lstImagesRGBColors.Count & !bUpdate)
            {
                iUpdateTileIndex = 0;

                while (iUpdateTileIndex < slTileID.Count & !bUpdate)
                {
                    if (lstImagesRGBColors[iUpdateIndex].strFile == strUnsFile)
                    {
                        if (lstImagesRGBColors[iUpdateIndex].TileID.Contains(slTileID.Keys[iUpdateTileIndex]))
                        {
                            bUpdate = true;
                        }
                        else
                        {
                            iUpdateTileIndex = iUpdateTileIndex + 1;
                        }
                    }
                    else
                    {
                        iUpdateTileIndex = iUpdateTileIndex + 1;
                    }
                }

                if (!bUpdate)
                    iUpdateIndex = iUpdateIndex + 1;
            }

            if (bUpdate)
            {
                // We update the actual list
                stImageRGBColors = lstImagesRGBColors[iUpdateIndex];
                stImageRGBColors.TileID.Add(slTileID.Keys[iUpdateTileIndex]);
            }
            else
            {
                // We add the color to the list.
                stImageRGBColors.strFile = strUnsFile;
                stImageRGBColors.Layer = iLayer;
                stImageRGBColors.FirstTexture = iFirstTexture;
                stImageRGBColors.FirstPalette = iFirstPalette;
                stImageRGBColors.FirstTileID = iFirstTileID;
                stImageRGBColors.Texture = iTexture;
                stImageRGBColors.Palette = iPalette;
                stImageRGBColors.Param = iParam;
                stImageRGBColors.State = iState;
                stImageRGBColors.Used = false;

                stImageRGBColors.TileID = new HashSet<int>();
                foreach (var itmTileID in slTileID)
                    stImageRGBColors.TileID.Add(itmTileID.Key);

                stImageRGBColors.RGBUnswizzled = new HashSet<int>();
                stImageRGBColors.RGBSwizzled = new HashSet<int>();

                GetListOfColors(bmpUnswizzledTexture, ref stImageRGBColors.RGBUnswizzled);
                GetListOfColors(bmpSwizzledTexture, ref stImageRGBColors.RGBSwizzled);

                lstImagesRGBColors.Add(stImageRGBColors);
            }
        }

        public static void ResetUsedRGBColors()
        {
            ImageRGBColors stImageRGBColors;
            int iCounter;

            for (iCounter = 0; iCounter < lstImagesRGBColors.Count; iCounter++)
            {
                stImageRGBColors = lstImagesRGBColors[iCounter];
                stImageRGBColors.Used = false;

                lstImagesRGBColors[iCounter] = stImageRGBColors;
            }
        }

        public static void UpdateRGBColorsList(Bitmap bmpInputTexture, 
                                               string strUnsFile, 
                                               int iLayer, 
                                               int iUnsTileID, 
                                               int iTexture, 
                                               int iPalette, 
                                               int iParam, 
                                               int iState)
        {
            ImageRGBColors stImageRGBColors = new ImageRGBColors();
            int indexofRGBColor;

            // Let's search the RGBColor to mark as used by its strUnsFile (original).
            indexofRGBColor = lstImagesRGBColors.IndexOf(lstImagesRGBColors.Single(x => (x.strFile == strUnsFile)));
            stImageRGBColors = lstImagesRGBColors[indexofRGBColor];

            if (iLayer < 2)
            {
                stImageRGBColors.Texture = iTexture;
                stImageRGBColors.Palette = iPalette;
            }

            // Update also colors list (it must have more colors)
            stImageRGBColors.RGBSwizzled = new HashSet<int>();
            GetListOfColors(bmpInputTexture, ref stImageRGBColors.RGBSwizzled);

            lstImagesRGBColors[indexofRGBColor] = stImageRGBColors;
        }

        public static int CountMatchedColors(HashSet<int> listedImageiRGB, 
                                             HashSet<int> iRGB)
        {
            int iCountedMatchedColors;

            iCountedMatchedColors = 0;

            if (listedImageiRGB.Count >= iRGB.Count)
            {
                foreach (var iRGBColor in iRGB)
                {
                    if (listedImageiRGB.Contains(iRGBColor))
                    {
                        iCountedMatchedColors = iCountedMatchedColors + 1;
                    }
                }
            }
            else
            {
                foreach (var iRGBColor in listedImageiRGB)
                {
                    if (iRGB.Contains(iRGBColor))
                    {
                        iCountedMatchedColors = iCountedMatchedColors + 1;
                    }
                }
            }

            return iCountedMatchedColors;
        }

        public static void UpdateRGBColorsUsed(string strFile)
        {
            ImageRGBColors stRGBColor = new ImageRGBColors();
            int indexRGBColor;

            indexRGBColor = lstImagesRGBColors.IndexOf(lstImagesRGBColors.Single(x => (x.strFile == strFile)));

            stRGBColor = lstImagesRGBColors[indexRGBColor];
            stRGBColor.Used = true;

            lstImagesRGBColors[indexRGBColor] = stRGBColor;
        }

        public static int FindBestHashDetected(Bitmap bmpSwizzledTexture, 
                                               Bitmap bmpUnswizzledTexture, 
                                               List<ImageRGBColors> querylstImagesRGBColors)
        {

            HashSet<int> iRGBUnswizzled = new HashSet<int>();
            int iMaxUnswizzledMatchedColors, iBestUnswizzledNumTexture;
            int iMaxUnswizzledTotalColors;
            int iActualUnswizzledMatchedColors;
            int iActualUnswizzledTotalColors;

            HashSet<int> iRGBSwizzled = new HashSet<int>();
            int iMaxSwizzledMatchedColors, iBestSwizzledNumTexture;
            int iMaxSwizzledTotalColors;
            var iActualSwizzledMatchedColors = default(int);
            var iActualSwizzledTotalColors = default(int);

            int iNumTexture;
            string strUnswizzledFile, strSwizzledFile;

            int iBestTexture;
            bool bUpdate;

            iBestUnswizzledNumTexture = 9999;
            iBestSwizzledNumTexture = 9999;

            iMaxSwizzledMatchedColors = 0;
            iMaxUnswizzledMatchedColors = 0;

            iNumTexture = 0;
            iBestTexture = 9999;

            GetListOfColors(bmpUnswizzledTexture, ref iRGBUnswizzled);
            GetListOfColors(bmpSwizzledTexture, ref iRGBSwizzled);

            iMaxSwizzledTotalColors = 0;
            iMaxUnswizzledTotalColors = 0;


            // Ok. For each texture in ImagesRGBColors let's discover how much colors match. For Swizzled and Unswizzled.
            foreach (var listedImage in querylstImagesRGBColors)
            {

                // If listedImage.Texture < iTexture Or listedImage.Palette < iPalette And
                // Not listedImage.Used Then

                if (!listedImage.Used)
                {
                    iActualUnswizzledTotalColors = listedImage.RGBUnswizzled.Count;
                    iActualSwizzledTotalColors = listedImage.RGBSwizzled.Count;

                    iActualUnswizzledMatchedColors = CountMatchedColors(listedImage.RGBUnswizzled, iRGBUnswizzled);
                    iActualSwizzledMatchedColors = CountMatchedColors(listedImage.RGBSwizzled, iRGBSwizzled);

                    if (iActualUnswizzledMatchedColors >= iMaxUnswizzledMatchedColors)
                    {
                        bUpdate = true;

                        if (iActualUnswizzledMatchedColors == iMaxUnswizzledMatchedColors)
                            if (iMaxUnswizzledTotalColors <= iActualUnswizzledTotalColors)
                                bUpdate = false;

                        if (bUpdate)
                        {
                            iMaxUnswizzledMatchedColors = iActualUnswizzledMatchedColors;
                            iMaxUnswizzledTotalColors = iActualUnswizzledTotalColors;

                            iBestUnswizzledNumTexture = iNumTexture;

                            strUnswizzledFile = listedImage.strFile;
                        }
                    }
                }

                if (iActualSwizzledMatchedColors >= iMaxSwizzledMatchedColors)
                {
                    bUpdate = true;

                    if (iActualSwizzledMatchedColors == iMaxSwizzledMatchedColors)
                        if (iMaxSwizzledTotalColors <= iActualSwizzledTotalColors)
                            bUpdate = false;

                    if (bUpdate)
                    {
                        iMaxSwizzledMatchedColors = iActualSwizzledMatchedColors;
                        iMaxSwizzledTotalColors = iActualSwizzledTotalColors;

                        iBestSwizzledNumTexture = iNumTexture;

                        strSwizzledFile = listedImage.strFile;
                    }
                }

                iNumTexture = iNumTexture + 1;
            }


            // Let's try to determine the texture that we must use.
            if (iBestUnswizzledNumTexture == iBestSwizzledNumTexture &
                iBestUnswizzledNumTexture != 9999)
            {
                iBestTexture = iBestUnswizzledNumTexture;
            }
            else if (iBestUnswizzledNumTexture != 9999 &
                     iBestSwizzledNumTexture != 9999)
            {
                if (iMaxUnswizzledMatchedColors > iMaxSwizzledMatchedColors)
                    iBestTexture = iBestUnswizzledNumTexture;
                else if (iMaxUnswizzledMatchedColors < iMaxSwizzledMatchedColors)
                    iBestTexture = iBestSwizzledNumTexture;
                else
                {
                    if (iMaxUnswizzledTotalColors > iMaxSwizzledTotalColors)
                    {
                        iBestTexture = iBestSwizzledNumTexture;
                    }
                    else
                    {
                        iBestTexture = iBestUnswizzledNumTexture;
                    }
                }
            }
            else
            {
                if (iBestUnswizzledNumTexture == 9999 &
                    iBestSwizzledNumTexture != 9999)
                        iBestTexture = iBestSwizzledNumTexture;
                else if (iBestUnswizzledNumTexture != 9999 &
                         iBestSwizzledNumTexture == 9999)
                            iBestTexture = iBestUnswizzledNumTexture;
                else
                    iBestTexture = 9999;
            }

            return iBestTexture;
        }

        public static void SearchRGBListFile(int iUnsTileID, int iUnsParam, int iUnsState, 
                                             int iFirstTexture, int iFirstPalette, 
                                             ref string strBaseFileRGBColor, string strPath)
        {
            string strHash;
            string[] strBaseFile;
            int iFolderTexture, iFolderPalette;
            ImageRGBColors stRGBColor;

            iFolderTexture = 0;
            iFolderPalette = 0;
            strHash = "";

            // First let's get the hash and texture/palette of the file. This will be the same.
            FileTools.GetPathTexturePalette(Path.GetDirectoryName(strBaseFileRGBColor), 
                                            ref strHash,
                                            ref iFolderTexture,
                                            ref iFolderPalette);

            // Let's Check if the Texture_Palette subfolder corresponds to the one of the strBaseFileRGBColor.
            if (iFolderTexture == iFirstTexture & iFolderPalette == iFirstPalette)
            {
                strBaseFile = Path.GetFileNameWithoutExtension(strBaseFileRGBColor).Split('_');

                strBaseFileRGBColor = Path.GetDirectoryName(strBaseFileRGBColor) + "\\" +
                                      FileTools.strGlobalFieldName + "_" +
                                      strBaseFile[strBaseFile.Count() - 5] + "_" +
                                      strBaseFile[strBaseFile.Count() - 4] + "_" +
                                      iUnsParam.ToString("00") + "_" +
                                      iUnsState.ToString("00") + "_";

                // Used this for semkin_4
                // Used this (strBaseFile(strBaseFile.Count - 1)) for blin68_2 example and all other fields.
                switch (FileTools.strGlobalFieldName)
                {
                    case "semkin_4":
                    case "woa_3":
                        {
                            strBaseFileRGBColor = strBaseFileRGBColor + iUnsTileID.ToString("0000") + ".png";
                            break;
                        }

                    default:
                        {
                            strBaseFileRGBColor = strBaseFileRGBColor + strBaseFile[strBaseFile.Count() - 1] + ".png";
                            break;
                        }
                }
            }
            else
            {
                stRGBColor = (from itmRGBColor in lstImagesRGBColors
                              where itmRGBColor.TileID.Contains(iUnsTileID) &
                                    itmRGBColor.Param == iUnsParam &
                                    itmRGBColor.State == iUnsState &
                                    itmRGBColor.strFile.Contains(strHash)
                              select itmRGBColor).First();

                strBaseFileRGBColor = stRGBColor.strFile;
            }
        }

        public static void UpdateMarkTileTexture(ref PictureBox pbInputPictureBox, 
                                                 int iTexture, int iLayer, int iTile)
        {

            // Let's mark the tile on Texture Image. ;)
            int iRectPosX, iRectPosY;
            Bitmap bmpUpdateTexture;

            if (S9.textureImage[iTexture] != null)
            {
                bmpUpdateTexture = new Bitmap(S9.textureImage[iTexture].Bitmap);

                using (Graphics g = Graphics.FromImage(bmpUpdateTexture))
                {
                    {
                        // First we will draw the tile again without marks.
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.CompositingMode = CompositingMode.SourceCopy;
                        g.PixelOffsetMode = PixelOffsetMode.Half;

                        g.DrawImage(S9.textureImage[iTexture].Bitmap, 
                                    new Rectangle(0, 0, pbInputPictureBox.Width, pbInputPictureBox.Height), 
                                    0, 0, S9.TEXTURE_WIDTH,
                                          S9.TEXTURE_WIDTH,
                                    GraphicsUnit.Pixel);


                        // Second we will mark the new tile.
                        if (S9.Section9.Layer[iLayer].layerTiles[iTile].textureID2 == 0)
                        {
                            iRectPosX = S9.Section9.Layer[iLayer].layerTiles[iTile].sourceX;
                            iRectPosY = S9.Section9.Layer[iLayer].layerTiles[iTile].sourceY;
                        }
                        else
                        {
                            iRectPosX = S9.Section9.Layer[iLayer].layerTiles[iTile].sourceX2;
                            iRectPosY = S9.Section9.Layer[iLayer].layerTiles[iTile].sourceY2;
                        }

                        Rectangle rectTile = new Rectangle(iRectPosX, iRectPosY,
                                                           S9.Section9.Layer[iLayer].layerTiles[iTile].imgTile.Width,
                                                           S9.Section9.Layer[iLayer].layerTiles[iTile].imgTile.Height);

                        Color cColor1, cColor2;

                        cColor1 = Color.GreenYellow;
                        cColor2 = Color.Turquoise;

                        using (var pen1 = new Pen(cColor1, 2))
                            g.DrawRectangle(pen1, rectTile);

                        using (var pen2 = new Pen(cColor2, 2))
                        {
                            pen2.DashPattern = (new float[] { 2f, 2f });
                            g.DrawRectangle(pen2, rectTile);
                        }
                    }

                    if (pbInputPictureBox.Image != null)
                                pbInputPictureBox.Image.Dispose();
        
                    pbInputPictureBox.Image = new Bitmap(bmpUpdateTexture);
                }

                bmpUpdateTexture.Dispose();
            }
        }

        public static void UpdateMoveTileTexture(ref PictureBox pbInputPictureBox, 
                                                 int iTexture, 
                                                 int iLayer, int iTile, 
                                                 int iMarkLayer, int iMarkTile)
        {


            // Let's mark the tile on Texture Image. ;)
            int iTileSize, iTilePosX, iTilePosY, xPos, yPos;
            Bitmap bmpUpdateTile, bmpPictureBox;
            Color cColor;

            // Let's mark the first Tile clicked if it is not the tile where we are moving.
            UpdateMarkTileTexture(ref pbInputPictureBox, iTexture, iMarkLayer, iMarkTile);

            if (S9.textureImage[iTexture] != null)
            {
                {
                    // We get the image of the the tile in pbTexture.Image.
                    iTileSize = S9.Section9.Layer[iLayer].layerTiles[iTile].imgTile.Width;
                    if (S9.Section9.Layer[iLayer].layerTiles[iTile].textureID2 == 0)
                    {
                        iTilePosX = S9.Section9.Layer[iLayer].layerTiles[iTile].sourceX;
                        iTilePosY = S9.Section9.Layer[iLayer].layerTiles[iTile].sourceY;
                    }
                    else
                    {
                        iTilePosX = S9.Section9.Layer[iLayer].layerTiles[iTile].sourceX2;
                        iTilePosY = S9.Section9.Layer[iLayer].layerTiles[iTile].sourceY2;
                    }

                    bmpPictureBox = new Bitmap(pbInputPictureBox.Image);

                    bmpUpdateTile = bmpPictureBox.Clone(new Rectangle(iTilePosX, iTilePosY, iTileSize, iTileSize),
                                                        PixelFormat.Format32bppArgb);

                    // Blend the image
                    for (yPos = 0; yPos < iTileSize; yPos++)
                    {

                        for (xPos = 0; xPos < iTileSize; xPos++)
                        {
                            cColor = bmpUpdateTile.GetPixel(xPos, yPos);

                            cColor = Color.FromArgb(80, cColor.R, cColor.G, cColor.B);

                            bmpUpdateTile.SetPixel(xPos, yPos, cColor);
                        }
                    }


                    // Draw the tile with Blended colors.
                    using (Graphics g = Graphics.FromImage(bmpPictureBox))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.CompositingMode = CompositingMode.SourceCopy;
                        g.PixelOffsetMode = PixelOffsetMode.Half;

                        g.DrawImage(bmpUpdateTile, 
                                    new Rectangle(iTilePosX, iTilePosY, iTileSize, iTileSize), 
                                    0, 0, iTileSize, iTileSize, GraphicsUnit.Pixel);

                        if (pbInputPictureBox.Image != null)
                                pbInputPictureBox.Image.Dispose();

                        pbInputPictureBox.Image = new Bitmap(bmpPictureBox);
                    }
                }

                bmpUpdateTile.Dispose();
                bmpPictureBox.Dispose();
            }
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
                if (codec.MimeType == mimeType)
                    return codec;

            return null;
        }

        public static void WriteBitmap(Bitmap bmpTexture, string strFile)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                bmpTexture.Save(memStream, ImageFormat.Png);

                using (FileStream flWrite = new FileStream(strFile, FileMode.Create, FileAccess.Write))
                    memStream.WriteTo(flWrite);
            }
        }

        public static void ReadBitmap(ref Bitmap bmpTexture, string strFile)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                using (FileStream flRead = new FileStream(strFile, FileMode.Open, FileAccess.Read))
                    flRead.CopyTo(memStream);

                bmpTexture = new Bitmap(memStream);

                bmpTexture.SetResolution(HORZ_RES, VERT_RES);
            }
        }

        public static Bitmap ChangeOpacity(Image imgInput, float sOpacityValue)
        {
            var bmp = new Bitmap(imgInput.Width, imgInput.Height);
            var cmColorMatrix = new ColorMatrix();
            var imgAttribute = new ImageAttributes();

            cmColorMatrix.Matrix33 = sOpacityValue;

            imgAttribute.SetColorMatrix(cmColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(imgInput, 
                            new Rectangle(0, 0, bmp.Width, bmp.Height), 
                            0, 0, imgInput.Width, imgInput.Height, 
                            GraphicsUnit.Pixel, imgAttribute);
            }
                
            return bmp;
        }

        public static void DrawRoundedRectangle(Graphics g, Rectangle r, int radius, Pen p)
        {
            GraphicsPath gPath = RoundedRectangle(r, radius);

            using (gPath)
                g.DrawPath(p, gPath);
        }

        public static void FillRoundedRectangle(Graphics g, Rectangle r, int radius, Brush br)
        {
            GraphicsPath gPath = RoundedRectangle(r, radius);

            using (gPath)
                g.FillPath(br, gPath);
        }

        public static GraphicsPath RoundedRectangle(Rectangle r, int radius)
        {
            GraphicsPath gPath = new GraphicsPath();
            int d = radius * 2;

            gPath.AddLine(r.Left + d, r.Top, r.Right - d, r.Top);
            gPath.AddArc(Rectangle.FromLTRB(r.Right - d, r.Top, r.Right, r.Top + d), -90, 90);
            gPath.AddLine(r.Right, r.Top + d, r.Right, r.Bottom - d);
            gPath.AddArc(Rectangle.FromLTRB(r.Right - d, r.Bottom - d, r.Right, r.Bottom), 0, 90);
            gPath.AddLine(r.Right - d, r.Bottom, r.Left + d, r.Bottom);
            gPath.AddArc(Rectangle.FromLTRB(r.Left, r.Bottom - d, r.Left + d, r.Bottom), 90, 90);
            gPath.AddLine(r.Left, r.Bottom - d, r.Left, r.Top + d);
            gPath.AddArc(Rectangle.FromLTRB(r.Left, r.Top, r.Left + d, r.Top + d), 180, 90);
            gPath.CloseFigure();

            return gPath;
        }

        public static bool CheckImageAlpha(Bitmap bmpInputBitmap)
        {
            bool bResult;
            int iPosX, iPosY;
            Color cColor;

            bResult = true;

            for (iPosY = 0; iPosY < bmpInputBitmap.Height; iPosY++)
            {                
                for (iPosX = 0; iPosX < bmpInputBitmap.Width; iPosX++)
                {
                    cColor = bmpInputBitmap.GetPixel(iPosX, iPosY);

                    if (cColor.A != 0 &
                        cColor.R != 0 &
                        cColor.G != 0 &
                        cColor.B != 0)

                             bResult = false;
                }
            }

            return bResult;
        }

        public static int ImportTexture(string strFileName, 
                                        int iLayer, 
                                        int iTexture, 
                                        bool bTypeTextureID2)
        {
            Bitmap bmpImported = null;
            int iResult;

            iResult = 0;

            ReadBitmap(ref bmpImported, strFileName);

            // Check if it is a texture.
            if (bmpImported.Width != S9.TEXTURE_WIDTH | 
                bmpImported.Height != S9.TEXTURE_HEIGHT)
                        iResult = 1;

            // Check palettes/colors if palettized.
            if (iResult == 0)

                if (S9.Section9.Layer[iLayer].layerTiles[0].depth < 2)
                {
                    // Palettized Texture
                    iResult = ImportPalettizedTexture(bmpImported, iLayer, iTexture, bTypeTextureID2);
                }
                else
                {
                    // Direct Color Texture
                    iResult = ImportDirectColorTexture(bmpImported, iLayer, iTexture, bTypeTextureID2);
                }

            bmpImported.Dispose();

            return iResult;
        }

        public static int ImportPalettizedTexture(Bitmap bmpImported, 
                                                  int iLayer, 
                                                  int iTexture, 
                                                  bool bTypeTextureID2)
        {
            int iResult;
            byte[,] newTextureMatrix;
            List<S9.S9_ZList> lstTilesTexture = new List<S9.S9_ZList>();

            // Let's check first if all the colors are in the palette(s).
            iResult = GetListTilesOfTexture(iTexture, ref lstTilesTexture);

            if (iResult == 0)
            {
                // Now we need to update the TextureMatrix of this Texture with the indexes
                // of the colors for the imported texture.
                newTextureMatrix = new byte[S9.TEXTURE_WIDTH, S9.TEXTURE_HEIGHT];

                iResult = S9.Create_TextureMatrix(ref newTextureMatrix,
                                                  bmpImported, 
                                                  lstTilesTexture);

                // iResult = 0, texture Matrix updated correctly
                if (iResult == 0)
                    // We now can update the textureMatrix of Section9.
                    S9.Section9.Textures[iTexture].textureMatrix = newTextureMatrix;
            }

            return iResult;
        }

        public static int ImportDirectColorTexture(Bitmap bmpImported,
                                                   int iLayer,
                                                   int iTexture,
                                                   bool bTypeTextureID2)
        {
            int iResult;
            ushort[,] newTextureMatrix2Bytes;
            List<S9.S9_ZList> lstTilesTexture = new List<S9.S9_ZList>();

            // Let's check first if all the colors are in the palette(s).
            iResult = GetListTilesOfTexture(iTexture, ref lstTilesTexture);

            if (iResult == 0)
            {
                // Now we need to update the TextureMatrix of this Texture with the indexes
                // of the colors for the imported texture.
                newTextureMatrix2Bytes = new ushort[S9.TEXTURE_WIDTH, S9.TEXTURE_HEIGHT];

                iResult = S9.Create_TextureMatrix2Bytes(ref newTextureMatrix2Bytes,
                                                        bmpImported,
                                                        lstTilesTexture);

                // iResult = 0, texture Matrix updated correctly
                if (iResult == 0)
                    // We now can update the textureMatrix of Section9.
                    S9.Section9.Textures[iTexture].textureMatrix2Bytes = newTextureMatrix2Bytes;
            }

            return iResult;
        }

        public static int GetListTilesOfTexture(int iTexture,
                                                ref List<S9.S9_ZList> lstTilesTexture)
        {
            // With this function we will check if the colors of the input image exists
            // in the palette(s) assigned to the diferent tiles.
            // We must do this by each tile.
            int iResult;

            iResult = 0;

            // Let's retrieve the list of tiles of this texture.
            // We will make this faster if we check the textureID from the beginning.
            lstTilesTexture = (from itmZList in S9.Section9Z
                               where itmZList.ZTexture == iTexture
                               select itmZList).ToList();

            if (lstTilesTexture.Count == 0) iResult = 3;

            return iResult;
        }

        public partial struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        public static Bitmap ResizeImage(Bitmap InputImage, int iSize)
        {
            return new Bitmap(InputImage, new Size(iSize, iSize));
        }


        // Create a cursor from a bitmap, resize it to 32x32, assign a specified hotspot.
        public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = ResizeImage(bmp, 32).GetHicon();
            IconInfo tmp = new IconInfo();

            GetIconInfo(ptr, ref tmp);

            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;

            ptr = CreateIconIndirect(ref tmp);

            return new Cursor(ptr);
        }

        public static void LoadCursors()
        {
            PencilCUR = CreateCursor(Aeris.Properties.Resources.pencil, 4, 27);
            CrossCUR = CreateCursor(Aeris.Properties.Resources.cross, 15, 15);
            PickerCUR = CreateCursor(Aeris.Properties.Resources.picker, 4, 27);
            HandCUR = CreateCursor(Aeris.Properties.Resources.hand, 13, 3);
            EditCUR = CreateCursor(Aeris.Properties.Resources.edit, 15, 15);
            MoveCUR = CreateCursor(Aeris.Properties.Resources.move, 15, 15);
        }
    }



    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());
        }

        public DirectBitmap(DirectBitmap inputDirectBitmap)
        {
            Width = inputDirectBitmap.Width;
            Height = inputDirectBitmap.Height;
            Bits = new Int32[Width * Height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());

            Bits = inputDirectBitmap.Bits;
        }

        public DirectBitmap(Bitmap inputBitmap)
        {
            Width = inputBitmap.Width;
            Height = inputBitmap.Height;
            Bits = new Int32[Width * Height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppArgb, BitsHandle.AddrOfPinnedObject());

            Bitmap = inputBitmap;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }

        public void SetPixel(int x, int y, Color colour)
        {
            int index = x + (y * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];

            Color result = Color.FromArgb(col);

            return result;
        }

        public void Clear(Color colorClear)
        {
            int x, y;

            for (y = 0; y < Height; y++) 
            {
                for (x = 0; x < Width; x++)
                       Bits[x + (y * Width)] = colorClear.ToArgb();
            }
        }

        public void DrawTileToBackground(DirectBitmap inputTile, int iSrcX, int iSrcY, bool sourceOver)
        {
            int x, y;

            for (y = 0; y < inputTile.Height; y++)
            {
                for (x = 0; x < inputTile.Width; x++)
                {
                    if (sourceOver)
                    { 
                        if (inputTile.GetPixel(x, y).A != 0)

                            Bits[(iSrcX + x) + ((iSrcY + y) * Width)] =
                                    inputTile.Bits[x + (y * inputTile.Width)];
                    }
                    else
                    {
                        Bits[(iSrcX + x) + ((iSrcY + y) * Width)] =
                                inputTile.Bits[x + (y * inputTile.Width)];
                    }
                }
            }
        }

        public void GetTileFromBackground(DirectBitmap inputBackground, int iSrcX, int iSrcY)
        {
            int x, y;

            for (y = 0; y < Height; y++)
            {
                for (x = 0; x < Width; x++)
                    Bits[x + (y * Width)] = inputBackground.Bits[(iSrcX + x) + ((iSrcY + y) * inputBackground.Width)];
            }
        }

        public void DrawTextureTileToBackgroundTile(S9.dataTile inputBaseDataTile,
                                                    int iTexture, int iSrcX, int iSrcY)
        {
            int xTile, yTile;

            for (yTile = 0; yTile < Height; yTile++)
            {
                for (xTile = 0; xTile < Width; xTile++)
                    Bits[xTile + (yTile * Width)] =
                        S9.textureImage[iTexture].Bits[(iSrcX + xTile) + ((iSrcY + yTile) * S9.TEXTURE_WIDTH)];
            }
        }

        public void Render_Tile(S9.dataTile inputDataTile)
        {
            int xTile, yTile, iTexture, iSrcX, iSrcY;

            if (inputDataTile.textureID2 == 0)
            {
                iTexture = inputDataTile.textureID;
                iSrcX = inputDataTile.sourceX;
                iSrcY = inputDataTile.sourceY;
            }
            else
            {
                iTexture = inputDataTile.textureID2;
                iSrcX = inputDataTile.sourceX2;
                iSrcY = inputDataTile.sourceY2;
            }

            if (inputDataTile.blending == 1)
            {
                Color bgtilePixel, textilePixel;

                for (yTile = 0; yTile < Height; yTile++)
                {

                    for (xTile = 0; xTile < Width; xTile++)
                    {
                        bgtilePixel = GetPixel(xTile, yTile);
                        textilePixel = S9.textureImage[iTexture].GetPixel(iSrcX + xTile, iSrcY + yTile);

                        if (bgtilePixel.A != 0)
                            // Put the Color in the tile image of the background
                            if (textilePixel.A != 0)
                                SetPixel(xTile, yTile, Palette.BlendColor(bgtilePixel,
                                                                          textilePixel,
                                                                          inputDataTile.BlendMode));
                            else
                                SetPixel(xTile, yTile, bgtilePixel);
                        else
                            SetPixel(xTile, yTile, textilePixel);
                    }
                }
            }
            else
            {
                DrawTextureTileToBackgroundTile(inputDataTile, iTexture, iSrcX, iSrcY);
            }
        }

    }
}