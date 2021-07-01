using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

// Notes about some fields backgrounds Base Images

// blinst_2: The Tiles 640,641 Of Texture 02 are "out of boundaries" With a 
// DestX = 10000 both.
// cos_btm: The Tile 626 Of Texture 02 Is "out of boundaries" With a
// DestX of 10000.
// jtmpin2: The Tiles 132,163,244,245 Of Texture 00 are "out of boundaries" With a
// DestX of 10000.
// md_e1:	 The Textures 24,25,26 seems Not To be used. All have Tiles With
// DestX of 10000.
// nivinn_2: The Tile 480 Of Texture 01 Is "out of boundaries" With a
// DestX of 10000.
// trnad_1: The Tile 0 Of Texture 00 Is "out of boundaries" With a
// DestX of -3184.
// trnad_3: The Tile 0 Of Texture 00 Is "out of boundaries" With a
// DestX of -2616.

namespace Aeris
{
    public static class SwizzleBase
    {

        // Structure to help create Unswizzled Base Images when Exporting All Base Images (BI) process and
        // reverse process to create Swizzled Base Textures from these Unswizzled Base Images.
        public partial struct st_BI
        {
            public string strFileName;                    // This is only useful for the Swizzle
            public int iLayer;
            public int iParam;
            public int iState;
            public int iMainHigh15ByPal;
            public int iSecondaryHigh15ByPal;
            public int iMainLow15ByPal;
            public int iSecondaryLow15ByPal;
            public bool bHigh15;
            public int iUniqueSublayerID;
            public HashSet<st_JoinSublayerIDs> hsJoinSublayerIDs;
            public bool bTileSeparation;
            public int iIndexTileSeparation;
            public Bitmap bmpBaseImage;
            public int iDuplicateDest;                // We will use this to know the level of duplication for Tex < 15
            public int iDuplicateDestHigh15;          // We will use this to know the level of duplication for Tex > 14
            public int iDuplicateDestParam;           // We will use this to know the level of duplication by Param for Tex < 15
            public int iDuplicateDestParamHigh15;     // We will use this to know the level of duplication by Param for Tex > 14
        }

        public partial struct st_UniqueSublayerID
        {
            public int iUniqueSublayerID;
            public int iParam;
            public int iState;
        }

        public partial struct st_JoinSublayerIDs
        {
            public int iMainTileID;
            public HashSet<int> hsSecondaryTileIDs;
        }

        public partial struct st_High15ByPal
        {
            public int iMainPalette;
            public int iSecondaryPalette;
        }

        public partial struct st_Low15ByPal
        {
            public int iMainPalette;
            public int iSecondaryPalette;
        }

        public partial struct st_TileSeparationLayer
        {
            public int iLayer;
            public List<int> lstTilesSeparated;  // This is for fields like "blue_2", and the detach process.
        }

        public static st_BI[] lstUnswizzledBaseImages;
        public static SortedList<string, Bitmap> lstSwizzledBaseTextures;
        public static int iScaleFactorL0, iScaleFactorL2;

        public static void Initialize_lstUnswizzledBaseImages()
        {
            int iCounter;
            if (lstUnswizzledBaseImages != null)
            {
                for (iCounter = 0; iCounter < lstUnswizzledBaseImages.Count(); iCounter++)
                {
                    if (lstUnswizzledBaseImages[iCounter].bmpBaseImage != null)
                            lstUnswizzledBaseImages[iCounter].bmpBaseImage.Dispose();

                    if (lstUnswizzledBaseImages[iCounter].hsJoinSublayerIDs != null)
                           lstUnswizzledBaseImages[iCounter].hsJoinSublayerIDs.Clear();
                }

                lstUnswizzledBaseImages = null;
            }
        }

        public static void lstUnswizzledBaseImages_NewEntry(bool bInitializeBitmap, int iLayer)
        {
            // Vars to unify PosX/PosX of Global Layer position            
            int iLayerMaxWidthGlobal, iLayerMaxHeightGlobal;

            if (iLayer < 2)
            {
                iLayerMaxWidthGlobal = S9.iLayersMaxWidthL0;
                iLayerMaxHeightGlobal = S9.iLayersMaxHeightL0;
            }
            else
            {
                iLayerMaxWidthGlobal = S9.iLayersMaxWidthL2;
                iLayerMaxHeightGlobal = S9.iLayersMaxHeightL2;
            }

            if (lstUnswizzledBaseImages == null)
            {
                Array.Resize(ref lstUnswizzledBaseImages, 1);
            }
            else
            {
                Array.Resize(ref lstUnswizzledBaseImages, lstUnswizzledBaseImages.Count() + 1);
            }

            // Prepare the image
            if (bInitializeBitmap)
            {
                lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].bmpBaseImage = 
                            new Bitmap(iLayerMaxWidthGlobal, iLayerMaxHeightGlobal, PixelFormat.Format32bppArgb);
            }

            // Prepare the list with some values
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].iMainHigh15ByPal = -1;
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].iSecondaryHigh15ByPal = -1;
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].iMainLow15ByPal = -1;
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].iSecondaryLow15ByPal = -1;
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].bHigh15 = false;
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].iUniqueSublayerID = -1;
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].bTileSeparation = false;
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].iDuplicateDest = 0;
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].iDuplicateDestHigh15 = 0;
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].iDuplicateDestParam = 0;
            lstUnswizzledBaseImages[lstUnswizzledBaseImages.Count() - 1].iDuplicateDestParamHigh15 = 0;
        }




        // ------------------------------------------------------------------------------------------------
        // 
        // FROM SWIZZLED INTERNAL TEXTURE TO UNSWIZZLED BASE IMAGE
        // 
        // ------------------------------------------------------------------------------------------------

        public static void DrawTileInBaseImage(S9.S9_ZList itmZList, int iIndexBI)
        {
            int iLayerbmpPosXGlobal, iLayerbmpPosYGlobal;

            if (itmZList.ZLayer < 2)
            {
                iLayerbmpPosXGlobal = S9.iLayersbmpPosXL0;
                iLayerbmpPosYGlobal = S9.iLayersbmpPosYL0;
            }
            else
            {
                iLayerbmpPosXGlobal = S9.iLayersbmpPosXL2;
                iLayerbmpPosYGlobal = S9.iLayersbmpPosYL2;
            }

            using (Graphics g = Graphics.FromImage(lstUnswizzledBaseImages[iIndexBI].bmpBaseImage))
            {
                    g.DrawImage(S9.textureImage[itmZList.ZTexture].Bitmap,
                                new Rectangle(iLayerbmpPosXGlobal + itmZList.ZDestX,
                                              iLayerbmpPosYGlobal + itmZList.ZDestY,
                                              itmZList.ZTileSize,
                                              itmZList.ZTileSize),
                                itmZList.ZSourceX,
                                itmZList.ZSourceY,
                                itmZList.ZTileSize,
                                itmZList.ZTileSize,
                                GraphicsUnit.Pixel);
            }

            //using (Graphics g = Graphics.FromImage(lstUnswizzledBaseImages[iIndexBI].bmpBaseImage))
            //{
            //    g.DrawImage(S9.Section9.Layer[itmZList.ZLayer].layerTiles[itmZList.ZTile].imgTile,
            //                S9.iLayersbmpPosX + itmZList.ZDestX,
            //                S9.iLayersbmpPosY + itmZList.ZDestY);
            //}
        }

        public static void UpdateTileByLayerInBaseImage(S9.S9_ZList itmZList)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;
            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        !lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }

            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByLayerInBaseImageHigh15(S9.S9_ZList itmZList)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;
            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].bHigh15 = true;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }

            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByParamInBaseImage(S9.S9_ZList itmZList)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null) {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                        lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        !lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }


            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByParamInBaseImageHigh15(S9.S9_ZList itmZList)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                        lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].bHigh15 = true;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByLayerInBaseImageSameDest(S9.S9_ZList itmZList, int iDuplicateDest)
        {
            bool bFound;
            int iIndexBI;

            // Let's check if there is any Base Image for duplicated tiles.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        !lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == iDuplicateDest &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].iDuplicateDest = iDuplicateDest;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByLayerInBaseImageSameDestHigh15(S9.S9_ZList itmZList, int iDuplicateDest)
        {
            bool bFound;
            int iIndexBI;

            // Let's check if there is any Base Image for duplicated tiles.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == iDuplicateDest &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].bHigh15 = true;
                lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 = iDuplicateDest;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByParamInBaseImageSameDest(S9.S9_ZList itmZList, int iDuplicateDest)
        {
            bool bFound;
            int iIndexBI;

            // Let's check if there is any Base Image for duplicated tiles.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                        lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        !lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == iDuplicateDest &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam = iDuplicateDest;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByParamInBaseImageSameDestHigh15(S9.S9_ZList itmZList, int iDuplicateDest)
        {
            bool bFound;
            int iIndexBI;

            // Let's check if there is any Base Image for duplicated tiles.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                        lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == iDuplicateDest)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].bHigh15 = true;
                lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 = iDuplicateDest;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByTileIDInBaseImage(S9.S9_ZList itmZList)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                        lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        !lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == itmZList.ZTileID &
                        lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID = itmZList.ZTileID;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByTileIDInBaseImageHigh15(S9.S9_ZList itmZList)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                        lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == itmZList.ZTileID &
                        lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID = itmZList.ZTileID;
                lstUnswizzledBaseImages[iIndexBI].bHigh15 = true;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByJoinTileIDInBaseImage(S9.S9_ZList itmZList, int iMainTileID)
        {
            bool bFound;
            int iIndexBI, iIndexJoinedTileIDs;
            st_JoinSublayerIDs itmJoinSublayerIDs;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;
            iIndexJoinedTileIDs = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                        lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {

                        // Now we check if the iMainTileID is in hsJoinSublayerIDs
                        iIndexJoinedTileIDs = 0;
                        if (lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs != null)
                        {
                            foreach (var currentItmJoinSublayerIDs in lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs)
                            {
                                itmJoinSublayerIDs = currentItmJoinSublayerIDs;
                                if (itmJoinSublayerIDs.iMainTileID == iMainTileID)
                                {
                                    bFound = true;
                                    break;
                                }
                                else
                                {
                                    iIndexJoinedTileIDs = iIndexJoinedTileIDs + 1;
                                }
                            }
                        }

                        if (!bFound)
                        {
                            iIndexBI = iIndexBI + 1;
                        }
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Prepare JoinSublayerIDs
                lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs = new HashSet<st_JoinSublayerIDs>();
                itmJoinSublayerIDs.iMainTileID = iMainTileID;
                itmJoinSublayerIDs.hsSecondaryTileIDs = new HashSet<int>();
                lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs.Add(itmJoinSublayerIDs);
                iIndexJoinedTileIDs = lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs.Count - 1;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") +
                                                                itmZList.ZTexture.ToString("00") +
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);

            // Add the ZTileID if it is not the main iMainTileID
            if (itmZList.ZTileID != iMainTileID)
            {
                lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs.ElementAtOrDefault(iIndexJoinedTileIDs).hsSecondaryTileIDs.Add(itmZList.ZTileID);
            }
        }

        public static void UpdateTileByPaletteInBaseImageHigh15(S9.S9_ZList itmZList, 
                                                         int iMainHigh15ByPal, int iSecondaryHigh15ByPal)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer & lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam & lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState & lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == iMainHigh15ByPal & lstUnswizzledBaseImages[iIndexBI].iSecondaryHigh15ByPal == iSecondaryHigh15ByPal & lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 & !lstUnswizzledBaseImages[iIndexBI].bTileSeparation & lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 & lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 & lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 & lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal = iMainHigh15ByPal;
                lstUnswizzledBaseImages[iIndexBI].iSecondaryHigh15ByPal = iSecondaryHigh15ByPal;
                lstUnswizzledBaseImages[iIndexBI].bHigh15 = true;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" + 
                                                                itmZList.ZLayer.ToString("0") + 
                                                                itmZList.ZTexture.ToString("00") + 
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByPaletteInBaseImageLow15(S9.S9_ZList itmZList, 
                                                        int iMainLow15ByPal, int iSecondaryLow15ByPal)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                        lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == iMainLow15ByPal &
                        lstUnswizzledBaseImages[iIndexBI].iSecondaryLow15ByPal == iSecondaryLow15ByPal &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal = iMainLow15ByPal;
                lstUnswizzledBaseImages[iIndexBI].iSecondaryLow15ByPal = iSecondaryLow15ByPal;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" + 
                                                                itmZList.ZLayer.ToString("0") + 
                                                                itmZList.ZTexture.ToString("00") + 
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static void UpdateTileByTileSeparationInBaseImage(S9.S9_ZList itmZList, 
                                                          int iIndexTileSeparation, 
                                                          List<st_TileSeparationLayer> lstTileSeparation)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            if (lstUnswizzledBaseImages != null)
            {
                while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
                {
                    if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                        lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                        lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                        lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                        lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                        (lstUnswizzledBaseImages[iIndexBI].bTileSeparation &&
                         lstUnswizzledBaseImages[iIndexBI].iIndexTileSeparation == iIndexTileSeparation) &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                        lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                    {
                        bFound = true;
                    }
                    else
                    {
                        iIndexBI = iIndexBI + 1;
                    }
                }
            }

            if (!bFound)
            {
                // Create new image/entry in lstUnswizzledBaseImages
                lstUnswizzledBaseImages_NewEntry(true, itmZList.ZLayer);
                iIndexBI = lstUnswizzledBaseImages.Count() - 1;

                // Add special tile data
                lstUnswizzledBaseImages[iIndexBI].bTileSeparation = true;
                lstUnswizzledBaseImages[iIndexBI].iIndexTileSeparation = iIndexTileSeparation;
                if (itmZList.ZTexture > 0xE)
                    lstUnswizzledBaseImages[iIndexBI].bHigh15 = true;

                // Add basic tile data
                lstUnswizzledBaseImages[iIndexBI].strFileName = FileTools.strGlobalFieldName + "_" +
                                                                itmZList.ZLayer.ToString("0") + 
                                                                itmZList.ZTexture.ToString("00") + 
                                                                itmZList.ZTile.ToString("0000") + ".png";

                lstUnswizzledBaseImages[iIndexBI].iLayer = itmZList.ZLayer;
                lstUnswizzledBaseImages[iIndexBI].iParam = itmZList.ZParam;
                lstUnswizzledBaseImages[iIndexBI].iState = itmZList.ZState;
            }


            // Draw tile in image
            DrawTileInBaseImage(itmZList, iIndexBI);
        }

        public static int UnswizzleFieldTexturesToBaseImages(string strOutputFolder)
        {

            var sortZList = new List<S9.S9_ZList>();
            var hsUniqueSublayerID = new HashSet<st_UniqueSublayerID>();
            var hsJoinSublayerIDs = new HashSet<st_JoinSublayerIDs>();
            var lstHigh15ByPal = new List<st_High15ByPal>();
            var lstLow15ByPal = new List<st_Low15ByPal>();
            List<st_TileSeparationLayer> lstTileSeparation;
            st_UniqueSublayerID itmUniqueSublayerID;

            int iResult;
            int iLayer, iParam, iState, iTileID;
            int iMainTileID, iMainHigh15ByPal, iSecondaryHigh15ByPal, iIndexHigh15ByPal;
            int iMainLow15ByPal, iSecondaryLow15ByPal, iIndexLow15ByPal, iIndexTileSeparation;
            int iDestX, iDestY, iDestXParam, iDestYParam;
            int iDestXHigh15, iDestYHigh15, iDestXParamHigh15, iDestYParamHigh15;
            int iDuplicateDest, iDuplicateDestHigh15, iDuplicateDestParam, iDuplicateDestParamHigh15;
            int iTileAbs = 0;

            iResult = 0;
            lstTileSeparation = null;
            iSecondaryHigh15ByPal = 0;
            iSecondaryLow15ByPal = 0;

            try
            {
                Initialize_lstUnswizzledBaseImages();

                iResult = Read_BITemplates(ref hsUniqueSublayerID,
                                           ref hsJoinSublayerIDs,
                                           ref lstHigh15ByPal,
                                           ref lstLow15ByPal,
                                           ref lstTileSeparation);

                if (iResult != 0) return iResult;

                // sortZList = (From sortZItem In Section9Z
                // Order By sortZItem.ZLayer,
                // sortZItem.ZTexture,
                // sortZItem.ZTile).ToList()

                sortZList = (from sortZItem in S9.Section9Z
                             where (sortZItem.ZTexture < 0xF | sortZItem.ZTexture > 0x19) &
                                   (sortZItem.ZDestX > -2500 & sortZItem.ZDestX < 5000)
                             orderby sortZItem.ZLayer,
                                     sortZItem.ZDestX,
                                     sortZItem.ZDestY,
                                     sortZItem.ZParam,
                                     sortZItem.ZState
                             select sortZItem).ToList();

                sortZList = sortZList.Concat(from sortZItem in S9.Section9Z
                                             where (sortZItem.ZTexture > 0xE & sortZItem.ZTexture < 0x1A) &
                                                   (sortZItem.ZDestX > -2500 & sortZItem.ZDestX < 5000)
                                             orderby sortZItem.ZLayer,
                                                     sortZItem.ZDestX,
                                                     sortZItem.ZDestY,
                                                     sortZItem.ZParam,
                                                     sortZItem.ZState
                                             select sortZItem).ToList();


                // We will export base images following the render background method.
                // Get first values
                iLayer = sortZList[0].ZLayer;
                iParam = sortZList[0].ZParam;
                iState = sortZList[0].ZState;
                iTileID = sortZList[0].ZTileID;
                iDuplicateDest = 0;
                iDuplicateDestParam = 0;
                iDuplicateDestHigh15 = 0;
                iDuplicateDestParamHigh15 = 0;
                iDestX = 9999;
                iDestY = 9999;
                iDestXHigh15 = 9999;
                iDestYHigh15 = 9999;
                iDestXParam = 9999;
                iDestYParam = 9999;
                iDestXParamHigh15 = 9999;
                iDestYParamHigh15 = 9999;

                foreach (S9.S9_ZList itmZList in sortZList)
                {

                    iTileAbs = itmZList.ZTileAbs;

                    // We will extract some sublayers by TileID if needed
                    // ...supposedly we have loaded them in hsUniqueSublayerID
                    itmUniqueSublayerID.iUniqueSublayerID = itmZList.ZTileID;
                    if (itmZList.ZParam == 0)
                    {
                        itmUniqueSublayerID.iParam = -1;
                        itmUniqueSublayerID.iState = -1;
                    }
                    else
                    {
                        itmUniqueSublayerID.iParam = itmZList.ZParam;
                        itmUniqueSublayerID.iState = itmZList.ZState;
                    }

                    // or Join TileIDs...
                    // ...supposedly we have a list of the joined ones in hsJoinSublayerIDs
                    if (itmZList.ZParam == 0)
                    {
                        iMainTileID = MustJoinTileID(itmZList, hsJoinSublayerIDs);
                    }
                    else
                    {
                        iMainTileID = -1;
                    }

                    // or High15 by Palette...
                    // supposedly we have a list of the Palettes in lstHigh15ByPal
                    iMainHigh15ByPal = -1;
                    if (itmZList.ZTexture > 0xE & itmZList.ZTexture < 0x1A & lstHigh15ByPal.Count > 0)
                    {
                        iIndexHigh15ByPal = IsHigh15ByPal(itmZList, lstHigh15ByPal);
                        if (iIndexHigh15ByPal > -1)
                        {
                            iMainHigh15ByPal = lstHigh15ByPal[iIndexHigh15ByPal].iMainPalette;
                            iSecondaryHigh15ByPal = lstHigh15ByPal[iIndexHigh15ByPal].iSecondaryPalette;
                        }
                    }

                    // or Low15 by Palette...
                    // supposedly we have a list of the Palettes in lstLow15ByPal
                    iMainLow15ByPal = -1;
                    if (itmZList.ZLayer == 1 & itmZList.ZTexture < 0xF && lstLow15ByPal.Count > 0)
                    {
                        iIndexLow15ByPal = IsLow15ByPal(itmZList, lstLow15ByPal);
                        if (iIndexLow15ByPal > -1)
                        {
                            iMainLow15ByPal = lstLow15ByPal[iIndexLow15ByPal].iMainPalette;
                            iSecondaryLow15ByPal = lstLow15ByPal[iIndexLow15ByPal].iSecondaryPalette;
                        }
                    }

                    // or TileSeparation...
                    // supposedly we have a list of the Tiles that must be separated in lstTileSeparation
                    iIndexTileSeparation = -1;
                    if (lstTileSeparation != null)
                    {
                        iIndexTileSeparation = IsTileSeparation(itmZList, lstTileSeparation);
                    }

                    if (iIndexTileSeparation > -1)
                    {

                        // Update Tile in Base Image by TileSeparation
                        UpdateTileByTileSeparationInBaseImage(itmZList, iIndexTileSeparation, lstTileSeparation);
                    }
                    else if (itmZList.ZLayer == 1 &&
                             hsUniqueSublayerID.Count > 0 &&
                             hsUniqueSublayerID.Contains(itmUniqueSublayerID))
                    {

                        // Update Tile in Base Image by Unique Sublayer ID
                        if (itmZList.ZTexture > 0xE & itmZList.ZTexture < 0x1A)
                        {
                            UpdateTileByTileIDInBaseImageHigh15(itmZList);
                        }
                        else
                        {
                            UpdateTileByTileIDInBaseImage(itmZList);
                        }
                    }
                    else if (iMainTileID > -1)
                    {

                        // Update Tile in Base Image Joining Sublayer IDs.
                        UpdateTileByJoinTileIDInBaseImage(itmZList, iMainTileID);
                    }
                    else if (iMainHigh15ByPal > -1)
                    {

                        // Update Tile in Base Image with High15ByPal palette numbers for Tex>14.
                        UpdateTileByPaletteInBaseImageHigh15(itmZList, iMainHigh15ByPal, iSecondaryHigh15ByPal);
                    }
                    else if (iMainLow15ByPal > -1)
                    {

                        // Update Tile in Base Image with High15ByPal palette numbers for Tex>14.
                        UpdateTileByPaletteInBaseImageLow15(itmZList, iMainLow15ByPal, iSecondaryLow15ByPal);
                    }
                    else
                    {
                        if (itmZList.ZLayer != iLayer)
                        {
                            iDuplicateDest = 0;
                            iDuplicateDestParam = 0;
                            iDuplicateDestHigh15 = 0;
                            iDuplicateDestParamHigh15 = 0;

                            // Update vars
                            iLayer = itmZList.ZLayer;
                            iParam = itmZList.ZParam;
                            iState = itmZList.ZState;
                            iDestX = 9999;
                            iDestY = 9999;
                            iDestXHigh15 = 9999;
                            iDestYHigh15 = 9999;
                            iDestXParam = 9999;
                            iDestYParam = 9999;
                            iDestXParamHigh15 = 9999;
                            iDestYParamHigh15 = 9999;
                        }

                        if (itmZList.ZParam == 0)
                        {
                            if (itmZList.ZTexture > 0xE & itmZList.ZTexture < 0x1A)
                            {
                                if (itmZList.ZDestX == iDestXHigh15 &
                                    itmZList.ZDestY == iDestYHigh15)
                                {
                                    iDuplicateDestHigh15 = iDuplicateDestHigh15 + 1;
                                    UpdateTileByLayerInBaseImageSameDestHigh15(itmZList, iDuplicateDestHigh15);
                                }
                                else
                                {
                                    iDuplicateDestHigh15 = 0;
                                    UpdateTileByLayerInBaseImageHigh15(itmZList);
                                }

                                iDestXHigh15 = itmZList.ZDestX;
                                iDestYHigh15 = itmZList.ZDestY;
                            }
                            else
                            {
                                if (itmZList.ZDestX == iDestX &
                                    itmZList.ZDestY == iDestY)
                                {
                                    iDuplicateDest = iDuplicateDest + 1;
                                    UpdateTileByLayerInBaseImageSameDest(itmZList, iDuplicateDest);
                                }
                                else
                                {
                                    iDuplicateDest = 0;
                                    UpdateTileByLayerInBaseImage(itmZList);
                                }

                                iDestX = itmZList.ZDestX;
                                iDestY = itmZList.ZDestY;
                            }
                        }
                        else if (itmZList.ZTexture > 0xE & itmZList.ZTexture < 0x1A)
                        {
                            if (itmZList.ZDestX == iDestXParamHigh15 &
                                itmZList.ZDestY == iDestYParamHigh15 &
                                itmZList.ZParam == iParam &
                                itmZList.ZState == iState)
                            {
                                iDuplicateDestParamHigh15 = iDuplicateDestParamHigh15 + 1;
                                UpdateTileByParamInBaseImageSameDestHigh15(itmZList, iDuplicateDestParamHigh15);
                            }
                            else
                            {
                                iDuplicateDestParamHigh15 = 0;
                                UpdateTileByParamInBaseImageHigh15(itmZList);
                            }

                            iDestXParamHigh15 = itmZList.ZDestX;
                            iDestYParamHigh15 = itmZList.ZDestY;
                            iParam = itmZList.ZParam;
                            iState = itmZList.ZState;
                        }
                        else
                        {
                            if (itmZList.ZDestX == iDestXParam &
                                itmZList.ZDestY == iDestYParam &
                                itmZList.ZParam == iParam &
                                itmZList.ZState == iState)
                            {
                                iDuplicateDestParam = iDuplicateDestParam + 1;
                                UpdateTileByParamInBaseImageSameDest(itmZList, iDuplicateDestParam);
                            }
                            else
                            {
                                iDuplicateDestParam = 0;
                                UpdateTileByParamInBaseImage(itmZList);
                            }

                            iDestXParam = itmZList.ZDestX;
                            iDestYParam = itmZList.ZDestY;
                            iParam = itmZList.ZParam;
                            iState = itmZList.ZState;
                        }
                    }
                }

                // Now we will output of images of lstUnswizzledBaseImages
                foreach (var itmBI in lstUnswizzledBaseImages)
                    // Once we have some Base Image, output it. To do so, we will prepare the name
                    // of the image with some reconigzable data. Then write the bitmap in .png format.
                    ImageTools.WriteBitmap(itmBI.bmpBaseImage, 
                                           strOutputFolder + "\\" + itmBI.strFileName);


                // Finally we will create the Texture/Palette/TileID links for All Base Images.
                Write_BIInfo(strOutputFolder, lstTileSeparation);
            }
            catch (Exception ex)
            {
                int iTileAbsError = 0;

                iTileAbsError = iTileAbs;

                iResult = -1;
            }

            return iResult;
        }

        public static int IsTileSeparation(S9.S9_ZList itmZList, List<st_TileSeparationLayer> lstTileSeparation)
        {
            int iIndexTileSeparation;
            int iTileSeparationCounter;
            iIndexTileSeparation = -1;
            iTileSeparationCounter = 0;
            while (iTileSeparationCounter < lstTileSeparation.Count & iIndexTileSeparation == -1)
            {
                if (lstTileSeparation[iTileSeparationCounter].iLayer == itmZList.ZLayer && lstTileSeparation[iTileSeparationCounter].lstTilesSeparated.Contains(itmZList.ZTile))
                {
                    iIndexTileSeparation = iTileSeparationCounter;
                }
                else
                {
                    iTileSeparationCounter = iTileSeparationCounter + 1;
                }
            }

            return iIndexTileSeparation;
        }

        public static int IsHigh15ByPal(S9.S9_ZList itmZList, List<st_High15ByPal> lstHigh15ByPal)
        {
            int iResult;
            int iHigh15ByPalCounter;
            iResult = -1;
            iHigh15ByPalCounter = 0;
            while (iHigh15ByPalCounter < lstHigh15ByPal.Count & iResult == -1)
            {
                if (lstHigh15ByPal[iHigh15ByPalCounter].iMainPalette == itmZList.ZPalette | lstHigh15ByPal[iHigh15ByPalCounter].iSecondaryPalette == itmZList.ZPalette)
                {
                    iResult = iHigh15ByPalCounter;
                }
                else
                {
                    iHigh15ByPalCounter = iHigh15ByPalCounter + 1;
                }
            }

            return iResult;
        }

        public static int IsLow15ByPal(S9.S9_ZList itmZList, List<st_Low15ByPal> lstLow15ByPal)
        {
            int iResult;
            int iLow15ByPalCounter;
            iResult = -1;
            iLow15ByPalCounter = 0;
            while (iLow15ByPalCounter < lstLow15ByPal.Count & iResult == -1)
            {
                if (lstLow15ByPal[iLow15ByPalCounter].iMainPalette == itmZList.ZPalette | lstLow15ByPal[iLow15ByPalCounter].iSecondaryPalette == itmZList.ZPalette)
                {
                    iResult = iLow15ByPalCounter;
                }
                else
                {
                    iLow15ByPalCounter = iLow15ByPalCounter + 1;
                }
            }

            return iResult;
        }

        public static int MustJoinTileID(S9.S9_ZList itmZList, HashSet<st_JoinSublayerIDs> hsJoinSublayerIDs)
        {
            int iMainTileID;
            iMainTileID = -1;
            foreach (var itmJoinSublayerIDs in hsJoinSublayerIDs)
            {
                if (itmJoinSublayerIDs.iMainTileID == itmZList.ZTileID | itmJoinSublayerIDs.hsSecondaryTileIDs.Contains(itmZList.ZTileID))
                {
                    iMainTileID = itmJoinSublayerIDs.iMainTileID;
                    break;
                }
            }

            return iMainTileID;
        }


        // This procedure reads the TileIDs we must discriminate for a given field
        // when doing the "Export All Base Images" option.
        // The format is <name of the field>.txt and contains:
        // one line (or two for the Joined Tiles) With the TileIDs involved, separated by comma (",")
        // Sample:
        // Content:  <UniqueSublayerIDs>
        // <JoinSublayerIDs>
        // <High15ByPal>
        // <Low15ByPal>
        // <TileSeparation>
        // 
        // <UniqueSublayerIDs>:  Here goes the TileIDs, separated by comma ",":
        // 4081,4082,4083
        // -  You can even indicate the Param/State if needed:
        // 4081,4082,4083_02_01
        // <JoinSublayerIDs>:    Here goes the TileIDs that must be joined, separated by comma ",":
        // 1:2+3,368:720+730
        // This means, we must Join the TileIDs 2 & 3 (Secondary) with the
        // TileID 1 (Main).
        // <High15ByPal>:        This outputs the image By a given Palette. You can join 2 palettes.
        // <Low15ByPal>          Sample:
        // 5+6,7,9+10
        // This means that Aeris outputs one file with Tex>14 or Tex<15, depending
        // on <High15ByPal> or <Low15ByPal>, joining palette 5 and palette6, outputs
        // another image with palette 7, and outputs another image with palette 9 and 10.
        // 
        // <TileSeparation>:     This separates tiles from a given layer. You can add different TileSeparation tiles
        // separated by comma ",".
        // Sample:
        // 1:1+4+5+6+7+8,1:16+17+19+32+44
        // This outpus two images, one with Tiles 1+4+5+6+7+8 in Layer 1.
        // And another image with Tiles 16+17+19+32+44 in Layer 1.
        // 
        // The rest of tiles that do not use any of this keys will use the standard algorithm, presumably
        // the same as Palmer. But you have a bit more of control for some things.

        public static int Read_BITemplates(ref HashSet<st_UniqueSublayerID> hsUniqueSublayerID, 
                                           ref HashSet<st_JoinSublayerIDs> hsJoinSublayerIDs, 
                                           ref List<st_High15ByPal> lstHigh15ByPal, 
                                           ref List<st_Low15ByPal> lstLow15ByPal, 
                                           ref List<st_TileSeparationLayer> lstTileSeparation)
        {
            st_UniqueSublayerID itmUniqueSublayerID;
            st_JoinSublayerIDs itmJoinSublayerIDs;
            st_High15ByPal itmHigh15ByPal;
            st_Low15ByPal itmLow15ByPal;
            st_TileSeparationLayer itmTileSeparation;

            int iResult;
            string strBITemplateLine;
            string[] itmTileIDSplit;


            iResult = 0;
            strBITemplateLine = "";

            try
            {
                if (File.Exists(FileTools.strGlobalPath + "\\BITemplates\\" +
                                FileTools.strGlobalFieldName + ".txt"))
                {
                    using (var fileBITemplate = new StreamReader(FileTools.strGlobalPath + 
                                                                 "\\BITemplates\\" +
                                                                 FileTools.strGlobalFieldName + ".txt"))
                    {
                        while (!fileBITemplate.EndOfStream)
                        {
                            strBITemplateLine = fileBITemplate.ReadLine();

                            if (strBITemplateLine.Split('=')[0] == "UniqueSublayerID" &
                                strBITemplateLine.Split('=')[1].Length > 0)
                            {

                                // Process <UniqueSublayerIDs>
                                foreach (var itmEntryTileID in strBITemplateLine.Split('=')[1].Split(','))
                                {
                                    itmTileIDSplit = itmEntryTileID.Split('_');
                                    if (itmTileIDSplit.Count() > 1)
                                    {
                                        itmUniqueSublayerID.iUniqueSublayerID = Int32.Parse(itmTileIDSplit[0]);
                                        itmUniqueSublayerID.iParam = Int32.Parse(itmTileIDSplit[1]);
                                        itmUniqueSublayerID.iState = Int32.Parse(itmTileIDSplit[2]);
                                    }
                                    else
                                    {
                                        itmUniqueSublayerID.iUniqueSublayerID = Int32.Parse(itmTileIDSplit[0]);
                                        itmUniqueSublayerID.iParam = -1;
                                        itmUniqueSublayerID.iState = -1;
                                    }

                                    hsUniqueSublayerID.Add(itmUniqueSublayerID);
                                }
                            }
                            else if (strBITemplateLine.Split('=')[0] == "JoinSublayerIDs" &
                                     strBITemplateLine.Split('=')[1].Length > 0)
                            {

                                // Process <JoinSublayerIDs>
                                foreach (var itmEntryTileID in strBITemplateLine.Split('=')[1].Split(','))
                                {
                                    itmTileIDSplit = itmEntryTileID.Split(':');
                                    itmJoinSublayerIDs.iMainTileID = Int32.Parse(itmTileIDSplit[0]);
                                    itmJoinSublayerIDs.hsSecondaryTileIDs = new HashSet<int>();
                                    if (itmTileIDSplit.Count() > 1)
                                    {
                                        foreach (var itmSecondaryTile in itmTileIDSplit[1].Split('+'))
                                            itmJoinSublayerIDs.hsSecondaryTileIDs.Add(Int32.Parse(itmSecondaryTile));
                                    }

                                    hsJoinSublayerIDs.Add(itmJoinSublayerIDs);
                                }
                            }
                            else if (strBITemplateLine.Split('=')[0] == "High15ByPal")
                            {
                                if (strBITemplateLine.Split('=')[1].Length > 0)
                                {

                                    // Process <High15ByPal>
                                    foreach (var itmHigh15Pal in strBITemplateLine.Split('=')[1].Split(','))
                                    {
                                        itmHigh15ByPal.iMainPalette = Int32.Parse(itmHigh15Pal.Split('+')[0]);
                                        if (itmHigh15Pal.Split('+').Count() > 1)
                                        {
                                            itmHigh15ByPal.iSecondaryPalette = Int32.Parse(itmHigh15Pal.Split('+')[1]);
                                        }
                                        else
                                        {
                                            itmHigh15ByPal.iSecondaryPalette = -1;
                                        }

                                        lstHigh15ByPal.Add(itmHigh15ByPal);
                                    }
                                }
                            }
                            else if (strBITemplateLine.Split('=')[0] == "Low15ByPal")
                            {
                                if (strBITemplateLine.Split('=')[1].Length > 0)
                                {

                                    // Process <Low15ByPal>
                                    foreach (var itmLow15Pal in strBITemplateLine.Split('=')[1].Split(','))
                                    {
                                        itmLow15ByPal.iMainPalette = Int32.Parse(itmLow15Pal.Split('+')[0]);
                                        if (itmLow15Pal.Split('+').Count() > 1)
                                        {
                                            itmLow15ByPal.iSecondaryPalette = Int32.Parse(itmLow15Pal.Split('+')[1]);
                                        }
                                        else
                                        {
                                            itmLow15ByPal.iSecondaryPalette = -1;
                                        }

                                        lstLow15ByPal.Add(itmLow15ByPal);
                                    }
                                }
                            }
                            else if (strBITemplateLine.Split('=')[0] == "TileSeparation")
                            {
                                if (strBITemplateLine.Split('=')[1].Length > 0)
                                {

                                    // Process <TileSeparation>
                                    lstTileSeparation = new List<st_TileSeparationLayer>();
                                    foreach (var itmTileSep in strBITemplateLine.Split('=')[1].Split(','))
                                    {
                                        itmTileSeparation.iLayer = Int32.Parse(itmTileSep.Split(':')[0]);
                                        itmTileSeparation.lstTilesSeparated = new List<int>();
                                        if (itmTileSep.Split(':')[1].Split('+').Count() > 1)
                                        {
                                            foreach (var itmIndivTile in itmTileSep.Split(':')[1].Split('+'))
                                                itmTileSeparation.lstTilesSeparated.Add(Int32.Parse(itmIndivTile));
                                        }
                                        else
                                        {
                                            itmTileSeparation.lstTilesSeparated.Add(Int32.Parse(itmTileSep.Split(':')[1]));
                                        }

                                        lstTileSeparation.Add(itmTileSeparation);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                iResult = -4;
            }

            return iResult;
        }


        // Function to Save <field>_BI.txt File And put them In BI Structure
        // ################### INFO
        // iMainHigh15ByPal,               = -1 (Not used by Palette)
        // iSecondaryHigh15ByPal        > -1 (Uses palette (Tex>14) for determine the tiles and can even be joined)
        // iMainLow15ByPal,               = -1 (Not used by Palette)
        // iSecondaryLow15ByPal        > -1 (Uses palette (Tex<15) for determine the tiles and can even be joined)

        // UniqueSublayerID  -->           = -1 (Not used TileID of BITileIDs folder)
        // UniqueSublayerID  -->           > -1 (Used TileID of BITileIDs folder)

        // DuplicateDest -->               = 0 (NO DestX/DestY duplication for Param = 0)
        // DuplicateDest -->               > 0 Duplicated DestX/DestY for Param = 0, times duplicated
        // DuplicateDestHigh15 -->         = 0 (NO DestX/DestY duplication for Param = 0, Tex>14 & Tex<26)
        // DuplicateDestHigh15 -->         > 0 Duplicated DestX/DestY for Param = 0, Tex>14 & Tex<26, times duplicated
        // DuplicateDestParam -->          = 0 (NO DestX/DestY duplication for Param > 0)
        // DuplicateDestParam -->          > 0 (Duplicated DestX/DestY duplication for Param > 0, times duplicated)
        // DuplicateDestParamHigh15 -->    = 0 (NO DestX/DestY duplication for Param > 0, Tex>14 & Tex<26)
        // DuplicateDestParamHigh15 -->    > 0 (Duplicated DestX/DestY duplication for Param > 0, Tex>14 & Tex<26, times duplicated)

        // hsJoinSublayerIDs               = -1 The TileID has not to be joined
        // > -1 The TileIDs in hsSecondaryTileIDs have to be joined to the first.

        // lstTileSeparation               Tiles separated in a given Layer. The position on the list in the Index
        // used in lstBaseImages.iIndexTileSeparation.
        public static void Write_BIInfo(string strSavePath, List<st_TileSeparationLayer> lstTileSeparation)
        {
            string strWriteLine;
            using (var fileWriterBI = new StreamWriter(strSavePath + "\\" +
                                                       FileTools.strGlobalFieldName + "_BI.txt", 
                                                       false))
            {

                // Some bytes of header
                fileWriterBI.WriteLine("# BI Info Format: FileName, " + "Layer, Param, State, High15ByPalette, Low15ByPalette, " + "High15 (if Tex>14 And Tex<26), UniqueSublayerID, " + "DuplicateDest, DuplicateDestHigh15, " + "DuplicateDestParam, DuplicateDestParamHigh15, " + "JoinedTileIDs, TileSeparation");
                fileWriterBI.WriteLine("");

                // For Each Entry we need to prepare the WriteLine to put in stream.
                // We will do this ordering lstUnswizzledBaseImages
                foreach (var itmBI in lstUnswizzledBaseImages.OrderBy(x => x.strFileName).ToList())
                {
                    strWriteLine = itmBI.strFileName + ",";
                    strWriteLine = strWriteLine + itmBI.iLayer.ToString("0") + ",";
                    strWriteLine = strWriteLine + itmBI.iParam.ToString("0") + ",";
                    strWriteLine = strWriteLine + itmBI.iState.ToString("0") + ",";


                    // High15ByPal
                    strWriteLine = strWriteLine + itmBI.iMainHigh15ByPal.ToString();
                    if (itmBI.iSecondaryHigh15ByPal > -1)
                    {
                        strWriteLine = strWriteLine + "+" + itmBI.iSecondaryHigh15ByPal.ToString();
                    }

                    strWriteLine = strWriteLine + ",";


                    // Low15ByPal
                    strWriteLine = strWriteLine + itmBI.iMainLow15ByPal.ToString();
                    if (itmBI.iSecondaryLow15ByPal > -1)
                    {
                        strWriteLine = strWriteLine + "+" + itmBI.iSecondaryLow15ByPal.ToString();
                    }

                    strWriteLine = strWriteLine + ",";
                    strWriteLine = strWriteLine + itmBI.bHigh15.ToString() + ",";


                    // UniqueSublayerID
                    if (itmBI.iUniqueSublayerID == -1)
                    {
                        strWriteLine = strWriteLine + "-1,";
                    }
                    else
                    {
                        if (itmBI.iParam == 0)
                        {
                            strWriteLine = strWriteLine + itmBI.iUniqueSublayerID.ToString();
                        }
                        else
                        {
                            strWriteLine = strWriteLine + itmBI.iUniqueSublayerID.ToString() + "_" + itmBI.iParam.ToString("00") + "_" + itmBI.iState.ToString("00");
                        }

                        strWriteLine = strWriteLine + ",";
                    }

                    strWriteLine = strWriteLine + itmBI.iDuplicateDest.ToString() + ",";
                    strWriteLine = strWriteLine + itmBI.iDuplicateDestHigh15.ToString() + ",";
                    strWriteLine = strWriteLine + itmBI.iDuplicateDestParam.ToString() + ",";
                    strWriteLine = strWriteLine + itmBI.iDuplicateDestParamHigh15.ToString();


                    // JoinSublayerIDs
                    if (itmBI.hsJoinSublayerIDs != null)
                    {
                        foreach (var itmJoinTileID in itmBI.hsJoinSublayerIDs)
                        {
                            var lstSortJoinTileID = itmJoinTileID.hsSecondaryTileIDs.ToList();
                            lstSortJoinTileID.Sort();
                            strWriteLine = strWriteLine + "," + itmJoinTileID.iMainTileID.ToString() + ":" + lstSortJoinTileID[0].ToString();
                            if (lstSortJoinTileID.Count > 1)
                            {
                                for (int i = 1; i < lstSortJoinTileID.Count; i++)
                                    strWriteLine = strWriteLine + "+" + lstSortJoinTileID[i].ToString();
                            }
                        }
                    }
                    else
                    {
                        strWriteLine = strWriteLine + ",-1";
                    }


                    // TileSeparation
                    if (itmBI.bTileSeparation)
                    {
                        strWriteLine = strWriteLine + "," + lstTileSeparation[itmBI.iIndexTileSeparation].lstTilesSeparated[0].ToString();
                        foreach (var itmTileSepar in lstTileSeparation[itmBI.iIndexTileSeparation].lstTilesSeparated.Skip(1))
                            strWriteLine = strWriteLine + "+" + itmTileSepar.ToString();
                    }
                    else
                    {
                        strWriteLine = strWriteLine + ",-1";
                    }


                    // Return carrier for next line
                    fileWriterBI.WriteLine(strWriteLine);
                }
            }
        }


        // Function to Read <field>_BI.txt File And put them In BI Structure (lstUnswizzledBaseImages)
        public static int Read_BIInfo(string strLoadPath, 
                               ref HashSet<st_UniqueSublayerID> hsUniqueSublayerID, 
                               ref List<st_TileSeparationLayer> lstTileSeparation)
        {
            string strLine;
            string[] strLineSplit;
            string[] strLineHigh15ByPal;
            string[] strLineLow15ByPal;
            var itmUniqueSublayerID = default(st_UniqueSublayerID);
            st_JoinSublayerIDs itmJoinSublayerIDs;
            st_TileSeparationLayer itmTileSeparation;
            int iIndexTileSeparation;
            int iResult;

            iResult = 0;
            iIndexTileSeparation = 0;

            Initialize_lstUnswizzledBaseImages();

            if (!File.Exists(strLoadPath + "\\" + FileTools.strGlobalFieldName + "_BI.txt"))
            {
                iResult = 3;
            }

            if (iResult == 0)
            {
                using (var fileReadBI = new StreamReader(strLoadPath + "\\" +
                                                         FileTools.strGlobalFieldName + "_BI.txt"))
                {
                    fileReadBI.ReadLine();
                    fileReadBI.ReadLine();

                    while (!fileReadBI.EndOfStream & iResult == 0)
                    {
                        strLine = fileReadBI.ReadLine();
                        strLineSplit = strLine.Split(',');

                        if (File.Exists(strLoadPath + "\\" + strLineSplit[0]))
                        {

                            lstUnswizzledBaseImages_NewEntry(false, Int32.Parse(strLineSplit[1]));

                            var indexlstUBI = lstUnswizzledBaseImages.Count() - 1;

                            lstUnswizzledBaseImages[indexlstUBI].strFileName = strLineSplit[0];

                            ImageTools.ReadBitmap(ref lstUnswizzledBaseImages[indexlstUBI].bmpBaseImage, 
                                                  strLoadPath + "\\" + 
                                                  lstUnswizzledBaseImages[indexlstUBI].strFileName);

                            lstUnswizzledBaseImages[indexlstUBI].iLayer = Int32.Parse(strLineSplit[1]);

                            // Check if the size of image corresponds even upscaled.
                            if (lstUnswizzledBaseImages[indexlstUBI].iLayer < 2)
                            {
                                if (iScaleFactorL0 == 0)
                                {
                                    if (lstUnswizzledBaseImages[indexlstUBI].bmpBaseImage.Width % S9.iLayersMaxWidthL0 != 0)
                                        return 4;

                                    // Prepare ScaleFactor for L0/L1
                                    iScaleFactorL0 = lstUnswizzledBaseImages[indexlstUBI].bmpBaseImage.Width / S9.iLayersMaxWidthL0;
                                }
                            }
                            else
                            {
                                if (iScaleFactorL2 == 0)
                                {
                                    if (lstUnswizzledBaseImages[indexlstUBI].bmpBaseImage.Width % S9.iLayersMaxWidthL2 != 0)
                                        return 4;

                                    // Prepare ScaleFactor for L2/L3
                                    iScaleFactorL2 = lstUnswizzledBaseImages[indexlstUBI].bmpBaseImage.Width / S9.iLayersMaxWidthL2;
                                }
                            }

                            lstUnswizzledBaseImages[indexlstUBI].iParam = Int32.Parse(strLineSplit[2]);
                            lstUnswizzledBaseImages[indexlstUBI].iState = Int32.Parse(strLineSplit[3]);


                            // Assign High15ByPal if needed
                            strLineHigh15ByPal = strLineSplit[4].Split('+');

                            lstUnswizzledBaseImages[indexlstUBI].iMainHigh15ByPal = 
                                            Int32.Parse(strLineHigh15ByPal[0]);
                            if (lstUnswizzledBaseImages[indexlstUBI].iMainHigh15ByPal == -1 |
                                strLineHigh15ByPal.Count() == 1)
                            {
                                lstUnswizzledBaseImages[indexlstUBI].iSecondaryHigh15ByPal = -1;
                            }
                            else
                            {
                                lstUnswizzledBaseImages[indexlstUBI].iSecondaryHigh15ByPal = 
                                            Int32.Parse(strLineHigh15ByPal[1]);
                            }


                            // Assign Low15ByPal if needed
                            strLineLow15ByPal = strLineSplit[5].Split('+');

                            lstUnswizzledBaseImages[indexlstUBI].iMainLow15ByPal = 
                                            Int32.Parse(strLineLow15ByPal[0]);

                            if (lstUnswizzledBaseImages[indexlstUBI].iMainLow15ByPal == -1 |
                                strLineLow15ByPal.Count() == 1)
                            {
                                lstUnswizzledBaseImages[indexlstUBI].iSecondaryLow15ByPal = -1;
                            }
                            else
                            {
                                lstUnswizzledBaseImages[indexlstUBI].iSecondaryLow15ByPal = 
                                            Int32.Parse(strLineLow15ByPal[1]);
                            }

                            lstUnswizzledBaseImages[indexlstUBI].bHigh15 = Convert.ToBoolean(strLineSplit[6]);


                            // Assign iUniqueSublayerID if needed
                            lstUnswizzledBaseImages[indexlstUBI].iUniqueSublayerID = 
                                            Int32.Parse(strLineSplit[7].Split('_')[0]);

                            if (lstUnswizzledBaseImages[indexlstUBI].iUniqueSublayerID > -1)
                            {
                                itmUniqueSublayerID.iUniqueSublayerID = 
                                            lstUnswizzledBaseImages[indexlstUBI].iUniqueSublayerID;

                                if (lstUnswizzledBaseImages[indexlstUBI].iParam > 0)
                                {
                                    itmUniqueSublayerID.iParam = lstUnswizzledBaseImages[indexlstUBI].iParam;
                                    itmUniqueSublayerID.iState = lstUnswizzledBaseImages[indexlstUBI].iState;
                                }
                                else
                                {
                                    itmUniqueSublayerID.iParam = -1;
                                    itmUniqueSublayerID.iState = -1;
                                }
                            }

                            hsUniqueSublayerID.Add(itmUniqueSublayerID);


                            lstUnswizzledBaseImages[indexlstUBI].iDuplicateDest = Int32.Parse(strLineSplit[8]);
                            lstUnswizzledBaseImages[indexlstUBI].iDuplicateDestHigh15 = Int32.Parse(strLineSplit[9]);
                            lstUnswizzledBaseImages[indexlstUBI].iDuplicateDestParam = Int32.Parse(strLineSplit[10]);
                            lstUnswizzledBaseImages[indexlstUBI].iDuplicateDestParamHigh15 = Int32.Parse(strLineSplit[11]);


                            // Load hsJoinSublayerIDs
                            if (strLineSplit[12] != "-1")
                            {
                                lstUnswizzledBaseImages[indexlstUBI].hsJoinSublayerIDs =
                                                new HashSet<st_JoinSublayerIDs>();

                                foreach (var itmhsJoinTileID in strLineSplit[12].Split(','))
                                {
                                    itmJoinSublayerIDs.iMainTileID = Int32.Parse(itmhsJoinTileID.Split(':')[0]);
                                    itmJoinSublayerIDs.hsSecondaryTileIDs = new HashSet<int>();

                                    foreach (var itmSecondaryTileID in itmhsJoinTileID.Split(':')[1].Split('+'))
                                        itmJoinSublayerIDs.hsSecondaryTileIDs.Add(Int32.Parse(itmSecondaryTileID));

                                    lstUnswizzledBaseImages[indexlstUBI].hsJoinSublayerIDs.Add(itmJoinSublayerIDs);
                                }
                            }


                            // Read TileSeparation
                            if (strLineSplit[13] != "-1")
                            {

                                // Process <TileSeparation>
                                // Assign to lstUnswizzledBaseImages
                                lstUnswizzledBaseImages[indexlstUBI].bTileSeparation = true;
                                lstUnswizzledBaseImages[indexlstUBI].iIndexTileSeparation = iIndexTileSeparation;


                                // Assign to lstTileSeparation
                                if (lstTileSeparation == null)
                                {
                                    lstTileSeparation = new List<st_TileSeparationLayer>();
                                }

                                itmTileSeparation.iLayer = lstUnswizzledBaseImages[indexlstUBI].iLayer;
                                itmTileSeparation.lstTilesSeparated = new List<int>();

                                // Assign each different Tile to lstTileSeparation
                                foreach (var itmTileSep in strLineSplit[13].Split('+'))
                                    itmTileSeparation.lstTilesSeparated.Add(Int32.Parse(itmTileSep));

                                lstTileSeparation.Add(itmTileSeparation);

                                iIndexTileSeparation = iIndexTileSeparation + 1;
                            }
                            else
                            {
                                lstUnswizzledBaseImages[indexlstUBI].bTileSeparation = false;
                            }
                        }
                        else
                        {
                            iResult = 2;
                        }
                    }
                }
            }

            return iResult;
        }




        // ------------------------------------------------------------------------------------------------
        // 
        // FROM UNSWIZZLED EXTERNAL BASE IMAGE TO EXTERNAL SWIZZLED TEXTURE (EVEN UPSCALED)
        // 
        // ------------------------------------------------------------------------------------------------
        public static void DrawTileInBaseTexture(S9.S9_ZList itmZList, int iIndexBI, int iScaleFactor)
        {
            string strTexPal;
            int iIndexSwizzledTexture;

            // Vars to unify PosX/PosX of Global Layer position            
            int iLayerbmpPosXGlobal, iLayerbmpPosYGlobal;

            if (itmZList.ZLayer < 2)
            {
                iLayerbmpPosXGlobal = S9.iLayersbmpPosXL0;
                iLayerbmpPosYGlobal = S9.iLayersbmpPosYL0;
            }
            else
            {
                iLayerbmpPosXGlobal = S9.iLayersbmpPosXL2;
                iLayerbmpPosYGlobal = S9.iLayersbmpPosYL2;
            }

            // Put the tile in Swizzled Texture Image
            strTexPal = itmZList.ZTexture.ToString("00") + "_" + "00";

            iIndexSwizzledTexture = lstSwizzledBaseTextures.IndexOfKey(strTexPal);

            if (iIndexSwizzledTexture == -1)
            {
                lstSwizzledBaseTextures.Add(strTexPal, new Bitmap(S9.TEXTURE_WIDTH * iScaleFactor, 
                                                                  S9.TEXTURE_HEIGHT * iScaleFactor));
            }

            using (Graphics g = Graphics.FromImage(lstSwizzledBaseTextures[strTexPal]))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.CompositingMode = CompositingMode.SourceCopy;

                g.DrawImage(lstUnswizzledBaseImages[iIndexBI].bmpBaseImage,
                            itmZList.ZSourceX * iScaleFactor,
                            itmZList.ZSourceY * iScaleFactor,
                            new Rectangle((iLayerbmpPosXGlobal + itmZList.ZDestX) * iScaleFactor,
                                          (iLayerbmpPosYGlobal + itmZList.ZDestY) * iScaleFactor,
                                          itmZList.ZTileSize * iScaleFactor, 
                                          itmZList.ZTileSize * iScaleFactor), 
                            GraphicsUnit.Pixel);
            }
        }

        public static void Initialize_lstBaseTextures()
        {
            if (lstSwizzledBaseTextures != null)
            {
                foreach (var itmSwizzledTexture in lstSwizzledBaseTextures)
                    itmSwizzledTexture.Value.Dispose();

                lstSwizzledBaseTextures.Clear();
            }
            else
            {
                lstSwizzledBaseTextures = new SortedList<string, Bitmap>();
            }
        }

        public static void PutTileByLayerInBaseTexture(S9.S9_ZList itmZList, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;


            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    !lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                    lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                    !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }


            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static void PutTileByLayerInBaseTextureHigh15(S9.S9_ZList itmZList, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;


            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                    lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                    !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }


            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static void PutTileByParamInBaseTexture(S9.S9_ZList itmZList, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                    lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    !lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                    lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                    !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }


            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static void PutTileByParamInBaseTextureHigh15(S9.S9_ZList itmZList, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                    lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                    lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                    !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }


            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static void PutTileByLayerInBaseTextureSameDest(S9.S9_ZList itmZList, int iDuplicateDest, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;

            // Let's check if there is any Base Image for duplicated tiles.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    !lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                    lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                    !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == iDuplicateDest &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }


            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static void PutTileByLayerInBaseTextureSameDestHigh15(S9.S9_ZList itmZList, int iDuplicateDest, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;

            // Let's check if there is any Base Image for duplicated tiles.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                    lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                    !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == iDuplicateDest &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }


            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static void PutTileByParamInBaseTextureSameDest(S9.S9_ZList itmZList, int iDuplicateDest, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;

            // Let's check if there is any Base Image for duplicated tiles.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                    lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    !lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                    lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                    !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == iDuplicateDest &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }


            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static void PutTileByParamInBaseTextureSameDestHigh15(S9.S9_ZList itmZList, int iDuplicateDest, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;

            // Let's check if there is any Base Image for duplicated tiles.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                    lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                    lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                    !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == iDuplicateDest)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }


            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static void PutTileByTileIDInBaseTexture(S9.S9_ZList itmZList, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                    lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    !lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == itmZList.ZTileID &
                    lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                    !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }

            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static void PutTileByTileIDInBaseTextureHigh15(S9.S9_ZList itmZList, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                    lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].bHigh15 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == itmZList.ZTileID &
                    lstUnswizzledBaseImages[iIndexBI].hsJoinSublayerIDs == null &
                    !lstUnswizzledBaseImages[iIndexBI].bTileSeparation &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }

            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static void PutTileByTileSeparationInBaseImage(S9.S9_ZList itmZList, int iIndexTileSeparation, int iScaleFactor)
        {
            bool bFound;
            int iIndexBI;

            // Locate the image to be updated (or create new if needed) by Param/State.
            bFound = false;
            iIndexBI = 0;

            while (iIndexBI < lstUnswizzledBaseImages.Count() & !bFound)
            {
                if (lstUnswizzledBaseImages[iIndexBI].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iIndexBI].iParam == itmZList.ZParam &
                    lstUnswizzledBaseImages[iIndexBI].iState == itmZList.ZState &
                    lstUnswizzledBaseImages[iIndexBI].iMainHigh15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iMainLow15ByPal == -1 &
                    lstUnswizzledBaseImages[iIndexBI].iUniqueSublayerID == -1 &
                    (lstUnswizzledBaseImages[iIndexBI].bTileSeparation &&
                     lstUnswizzledBaseImages[iIndexBI].iIndexTileSeparation == iIndexTileSeparation) &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iIndexBI].iDuplicateDestParamHigh15 == 0)
                {
                    bFound = true;
                }
                else
                {
                    iIndexBI = iIndexBI + 1;
                }
            }

            // Draw tile in Swizzled Texture Image
            DrawTileInBaseTexture(itmZList, iIndexBI, iScaleFactor);
        }

        public static int SwizzleBaseImagesToFieldTextures(string strInputDirectory, 
                                                           string strOutputDirectory, 
                                                           ref string strProcessFileName,
                                                           ref RichTextBox rtbResult)
        {

            var sortZList = new List<S9.S9_ZList>();
            var hsUniqueSublayerID = new HashSet<st_UniqueSublayerID>();
            List<st_TileSeparationLayer> lstTileSeparation;
            st_UniqueSublayerID itmUniqueSublayerID;
            int iLayer, iParam, iState, iTileID;
            int iUnjoinTileIDBI, iIndexHigh15ByPal, iIndexLow15ByPal, iIndexTileSeparation;
            int iDestX, iDestY, iDestXParam, iDestYParam;
            int iDestXHigh15, iDestYHigh15, iDestXParamHigh15, iDestYParamHigh15;
            int iDuplicateDest, iDuplicateDestHigh15, iDuplicateDestParam, iDuplicateDestParamHigh15;
            int iScaleFactor, iResult;
            DateTime TimeIn, TimeOut;
            TimeSpan TimeDiff, TotalTime;

            // Dim iTileAbs As Integer

            iResult = 0;
            lstTileSeparation = null;
            TotalTime = TimeSpan.Zero;

            iScaleFactorL0 = 0;
            iScaleFactorL2 = 0;

            try
            {
                Initialize_lstBaseTextures();
                TimeIn = DateTime.Now;

                // Ok, we know that there are Texture Links for Base Images (MANDATORY).
                // Let's use them.
                iResult = Read_BIInfo(strInputDirectory, ref hsUniqueSublayerID, ref lstTileSeparation);

                if (iResult != 0) return iResult;


                // sortZList = (From sortZItem In Section9Z
                // Order By sortZItem.ZLayer,
                // sortZItem.ZTexture,
                // sortZItem.ZTile).ToList()

                sortZList = (from sortZItem in S9.Section9Z
                             where (sortZItem.ZTexture < 0xF | sortZItem.ZTexture > 0x19) &&
                                   sortZItem.ZDestX < 5000 & sortZItem.ZDestX > -2500
                             orderby sortZItem.ZLayer, 
                                     sortZItem.ZDestX, 
                                     sortZItem.ZDestY, 
                                     sortZItem.ZParam, 
                                     sortZItem.ZState
                             select sortZItem).ToList();

                sortZList = sortZList.Concat(from sortZItem in S9.Section9Z
                                             where (sortZItem.ZTexture > 0xE & sortZItem.ZTexture < 0x1A) &&
                                                   sortZItem.ZDestX < 5000 & sortZItem.ZDestX > -2500
                                             orderby sortZItem.ZLayer, 
                                                     sortZItem.ZDestX, 
                                                     sortZItem.ZDestY, 
                                                     sortZItem.ZParam, 
                                                     sortZItem.ZState
                                             select sortZItem).ToList();


                // The process will be similar to the Export All Base Images.
                // We will create the textures by TileAbs basis.
                iLayer = sortZList[0].ZLayer;
                iParam = sortZList[0].ZParam;
                iState = sortZList[0].ZState;
                iTileID = sortZList[0].ZTileID;
                iDuplicateDest = 0;
                iDuplicateDestParam = 0;
                iDuplicateDestHigh15 = 0;
                iDuplicateDestParamHigh15 = 0;
                iDestX = 9999;
                iDestY = 9999;
                iDestXHigh15 = 9999;
                iDestYHigh15 = 9999;
                iDestXParam = 9999;
                iDestYParam = 9999;
                iDestXParamHigh15 = 9999;
                iDestYParamHigh15 = 9999;

                foreach (var itmZList in sortZList)
                {
                    // Let's select the ScaleFactor. Could be different between L0/L1 and L2/L3.
                    if (itmZList.ZLayer < 2) iScaleFactor = iScaleFactorL0;
                    else iScaleFactor = iScaleFactorL2;

                    // iTileAbs = itmZList.ZTileAbs

                    // We will recover the use of UniqueSublayerID and JoinSublayerIDs from
                    // the Export of All Base Images.
                    // This preparations are for UniqueSublayerID...
                    itmUniqueSublayerID.iUniqueSublayerID = itmZList.ZTileID;
                    if (itmZList.ZParam == 0)
                    {
                        itmUniqueSublayerID.iParam = -1;
                        itmUniqueSublayerID.iState = -1;
                    }
                    else
                    {
                        itmUniqueSublayerID.iParam = itmZList.ZParam;
                        itmUniqueSublayerID.iState = itmZList.ZState;
                    }

                    // ...and this for process JoinSublayerIDs (Unjoin)
                    if (itmZList.ZParam == 0)
                    {
                        iUnjoinTileIDBI = MustUnJoinTileID(itmZList);
                    }
                    else
                    {
                        iUnjoinTileIDBI = -1;
                    }

                    // or High15 by Palette...
                    // supposedly we have a list of the Palettes in lstHigh15ByPal
                    iIndexHigh15ByPal = -1;
                    if (itmZList.ZTexture > 0xE & itmZList.ZTexture < 0x1A)
                    {
                        iIndexHigh15ByPal = GetHigh15ByPal(itmZList);
                    }

                    // or Low15 by Palette...
                    // supposedly we have a list of the Palettes in lstLow15ByPal
                    iIndexLow15ByPal = -1;
                    if (itmZList.ZLayer == 1 & itmZList.ZTexture < 0xF)
                    {
                        iIndexLow15ByPal = GetLow15ByPal(itmZList);
                    }

                    // or TileSeparation...
                    // supposedly we have a list of the Tiles that must be separated in lstTileSeparation
                    iIndexTileSeparation = -1;
                    if (lstTileSeparation != null)
                    {
                        iIndexTileSeparation = IsTileSeparation(itmZList, lstTileSeparation);
                    }

                    if (iIndexTileSeparation > -1)
                    {

                        // Draw Tile in Base Image by TileSeparation
                        PutTileByTileSeparationInBaseImage(itmZList, iIndexTileSeparation, iScaleFactor);
                    }
                    else if (itmZList.ZLayer == 1 && hsUniqueSublayerID.Count > 0 && hsUniqueSublayerID.Contains(itmUniqueSublayerID))
                    {

                        // Draw Tile in Base Texture by UniqueSublayerID
                        if (itmZList.ZTexture > 0xE & itmZList.ZTexture < 0x1A)
                        {
                            PutTileByTileIDInBaseTextureHigh15(itmZList, iScaleFactor);
                        }
                        else
                        {
                            PutTileByTileIDInBaseTexture(itmZList, iScaleFactor);
                        }
                    }
                    else if (iUnjoinTileIDBI > -1)
                    {

                        // Draw tile in Swizzled Texture Image with Unjoin.
                        DrawTileInBaseTexture(itmZList, iUnjoinTileIDBI, iScaleFactor);
                    }
                    else if (iIndexHigh15ByPal > -1)
                    {

                        // Draw tile in Swizzled Texture Image with High15ByPal.
                        DrawTileInBaseTexture(itmZList, iIndexHigh15ByPal, iScaleFactor);
                    }
                    else if (iIndexLow15ByPal > -1)
                    {

                        // Draw tile in Swizzled Texture Image with High15ByPal.
                        DrawTileInBaseTexture(itmZList, iIndexLow15ByPal, iScaleFactor);
                    }
                    else
                    {
                        if (itmZList.ZLayer != iLayer)
                        {
                            iDuplicateDest = 0;
                            iDuplicateDestParam = 0;
                            iDuplicateDestHigh15 = 0;
                            iDuplicateDestParamHigh15 = 0;

                            // Update vars
                            iLayer = itmZList.ZLayer;
                            iParam = itmZList.ZParam;
                            iState = itmZList.ZState;
                            iDestX = 9999;
                            iDestY = 9999;
                            iDestXHigh15 = 9999;
                            iDestYHigh15 = 9999;
                            iDestXParam = 9999;
                            iDestYParam = 9999;
                            iDestXParamHigh15 = 9999;
                            iDestYParamHigh15 = 9999;
                        }

                        if (itmZList.ZParam == 0)
                        {
                            if (itmZList.ZTexture > 0xE & itmZList.ZTexture < 0x1A)
                            {
                                if (itmZList.ZDestX == iDestXHigh15 & itmZList.ZDestY == iDestYHigh15)
                                {
                                    iDuplicateDestHigh15 = iDuplicateDestHigh15 + 1;
                                    PutTileByLayerInBaseTextureSameDestHigh15(itmZList, iDuplicateDestHigh15, iScaleFactor);
                                }
                                else
                                {
                                    iDuplicateDestHigh15 = 0;
                                    PutTileByLayerInBaseTextureHigh15(itmZList, iScaleFactor);
                                }

                                iDestXHigh15 = itmZList.ZDestX;
                                iDestYHigh15 = itmZList.ZDestY;
                            }
                            else
                            {
                                if (itmZList.ZDestX == iDestX & itmZList.ZDestY == iDestY)
                                {
                                    iDuplicateDest = iDuplicateDest + 1;
                                    PutTileByLayerInBaseTextureSameDest(itmZList, iDuplicateDest, iScaleFactor);
                                }
                                else
                                {
                                    iDuplicateDest = 0;
                                    PutTileByLayerInBaseTexture(itmZList, iScaleFactor);
                                }

                                iDestX = itmZList.ZDestX;
                                iDestY = itmZList.ZDestY;
                            }
                        }
                        else if (itmZList.ZTexture > 0xE & itmZList.ZTexture < 0x1A)
                        {
                            if (itmZList.ZDestX == iDestXParamHigh15 & itmZList.ZDestY == iDestYParamHigh15 & itmZList.ZParam == iParam & itmZList.ZState == iState)
                            {
                                iDuplicateDestParamHigh15 = iDuplicateDestParamHigh15 + 1;
                                PutTileByParamInBaseTextureSameDestHigh15(itmZList, iDuplicateDestParamHigh15, iScaleFactor);
                            }
                            else
                            {
                                iDuplicateDestParamHigh15 = 0;
                                PutTileByParamInBaseTextureHigh15(itmZList, iScaleFactor);
                            }

                            iDestXParamHigh15 = itmZList.ZDestX;
                            iDestYParamHigh15 = itmZList.ZDestY;
                            iParam = itmZList.ZParam;
                            iState = itmZList.ZState;
                        }
                        else
                        {
                            if (itmZList.ZDestX == iDestXParam & itmZList.ZDestY == iDestYParam & itmZList.ZParam == iParam & itmZList.ZState == iState)
                            {
                                iDuplicateDestParam = iDuplicateDestParam + 1;
                                PutTileByParamInBaseTextureSameDest(itmZList, iDuplicateDestParam, iScaleFactor);
                            }
                            else
                            {
                                iDuplicateDestParam = 0;
                                PutTileByParamInBaseTexture(itmZList, iScaleFactor);
                            }

                            iDestXParam = itmZList.ZDestX;
                            iDestYParam = itmZList.ZDestY;
                            iParam = itmZList.ZParam;
                            iState = itmZList.ZState;
                        }
                    }
                }

                TimeOut = DateTime.Now;
                TimeDiff = TimeOut - TimeIn;
                TotalTime = TotalTime + TimeDiff;


                // -------------------------------------------------------------------------
                // Finally
                // * Write Swizzled Image.
                // -------------------------------------------------------------------------
                foreach (var itmSwizzledTexture in lstSwizzledBaseTextures)
                {
                    // Prepare FileName
                    strProcessFileName = strOutputDirectory + "\\" +
                                         FileTools.strGlobalFieldName + "_" + 
                                         itmSwizzledTexture.Key + ".png";

                    // Write image Swizzled
                    ImageTools.WriteBitmap(itmSwizzledTexture.Value, strProcessFileName);
                    itmSwizzledTexture.Value.Dispose();
                }

                if (CommandLine.bCmd)
                {
                    Console.WriteLine("SWIZZLE FINISHED\t\tDuration: " + TotalTime.Minutes.ToString("00") + ":" +
                                                                         TotalTime.Seconds.ToString("00") + "." +
                                                                         TotalTime.Milliseconds.ToString("000") + " ms.");
                }
                else
                {
                    logEvents.AddEventText("SWIZZLE FINISHED\t\tDuration: " + TotalTime.Minutes.ToString("00") + ":" +
                                                                              TotalTime.Seconds.ToString("00") + "." +
                                                                              TotalTime.Milliseconds.ToString("000") + " ms.",
                                                                              rtbResult);
                }
            }
            catch (Exception ex)
            {
                // Dim iStopTileAbs As Integer

                // iStopTileAbs = iTileAbs

                iResult = -1;
            }

            return iResult;
        }

        public static int GetLow15ByPal(S9.S9_ZList itmZList)
        {
            int iIndexLow15ByPal;
            int iBICounter;
            iIndexLow15ByPal = -1;
            iBICounter = 0;
            while (iBICounter < lstUnswizzledBaseImages.Count() & iIndexLow15ByPal == -1)
            {
                if (lstUnswizzledBaseImages[iBICounter].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iBICounter].iParam == itmZList.ZParam &
                    lstUnswizzledBaseImages[iBICounter].iState == itmZList.ZState &
                    !lstUnswizzledBaseImages[iBICounter].bHigh15 &
                    lstUnswizzledBaseImages[iBICounter].iUniqueSublayerID == -1 &
                    lstUnswizzledBaseImages[iBICounter].hsJoinSublayerIDs == null &
                    lstUnswizzledBaseImages[iBICounter].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iBICounter].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iBICounter].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iBICounter].iDuplicateDestParamHigh15 == 0 &
                    (lstUnswizzledBaseImages[iBICounter].iMainLow15ByPal == itmZList.ZPalette |
                     lstUnswizzledBaseImages[iBICounter].iSecondaryLow15ByPal == itmZList.ZPalette))
                {
                    iIndexLow15ByPal = iBICounter;
                }
                else
                {
                    iBICounter = iBICounter + 1;
                }
            }

            return iIndexLow15ByPal;
        }

        public static int GetHigh15ByPal(S9.S9_ZList itmZList)
        {
            int iIndexHigh15ByPal;
            int iBICounter;
            iIndexHigh15ByPal = -1;
            iBICounter = 0;
            while (iBICounter < lstUnswizzledBaseImages.Count() & iIndexHigh15ByPal == -1)
            {
                if (lstUnswizzledBaseImages[iBICounter].iLayer == itmZList.ZLayer &
                    lstUnswizzledBaseImages[iBICounter].iParam == itmZList.ZParam &
                    lstUnswizzledBaseImages[iBICounter].iState == itmZList.ZState &
                    lstUnswizzledBaseImages[iBICounter].bHigh15 &
                    lstUnswizzledBaseImages[iBICounter].iUniqueSublayerID == -1 &
                    lstUnswizzledBaseImages[iBICounter].hsJoinSublayerIDs == null &
                    lstUnswizzledBaseImages[iBICounter].iDuplicateDest == 0 &
                    lstUnswizzledBaseImages[iBICounter].iDuplicateDestHigh15 == 0 &
                    lstUnswizzledBaseImages[iBICounter].iDuplicateDestParam == 0 &
                    lstUnswizzledBaseImages[iBICounter].iDuplicateDestParamHigh15 == 0 &
                    (lstUnswizzledBaseImages[iBICounter].iMainHigh15ByPal == itmZList.ZPalette |
                     lstUnswizzledBaseImages[iBICounter].iSecondaryHigh15ByPal == itmZList.ZPalette))
                {
                    iIndexHigh15ByPal = iBICounter;
                }
                else
                {
                    iBICounter = iBICounter + 1;
                }
            }

            return iIndexHigh15ByPal;
        }

        public static int MustUnJoinTileID(S9.S9_ZList itmZList)
        {
            int iUnjoinTileID;
            int iBICounter;

            iUnjoinTileID = -1;
            iBICounter = 0;

            foreach (var itmBI in lstUnswizzledBaseImages)
            {
                if (itmBI.hsJoinSublayerIDs != null)
                {
                    foreach (var itmJoinSublayerIDs in itmBI.hsJoinSublayerIDs)
                    {
                        if (itmJoinSublayerIDs.iMainTileID == itmZList.ZTileID |
                            itmJoinSublayerIDs.hsSecondaryTileIDs.Contains(itmZList.ZTileID))
                        {
                            iUnjoinTileID = iBICounter;
                            break;
                        }
                    }
                }

                if (iUnjoinTileID > -1)
                    break;
                iBICounter = iBICounter + 1;
            }

            return iUnjoinTileID;
        }

        public static void UnswizzleInternalBaseTexture(int iUnsIntTexture, frmTextureImage frmTextureImage)
        {
            Bitmap bmpUnsIntTexture;
            List<S9.S9_ZList> sortZTexture;

            ImageTools.ClearPictureBox(frmTextureImage.pbTextureImage, 1, frmTextureImage.panelTextureImage);

            sortZTexture = (from sortZItem in S9.Section9Z
                            where sortZItem.ZTexture == iUnsIntTexture
                            orderby sortZItem.ZTexture,
                                    sortZItem.ZSourceY,
                                    sortZItem.ZSourceX
                            select sortZItem).ToList();

            bmpUnsIntTexture = new Bitmap(S9.iLayersMaxWidth, S9.iLayersMaxHeight, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(bmpUnsIntTexture))
            {
                foreach (var sortZItem in sortZTexture)
                {
                    {
                        // Let's render the Tile.
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.CompositingMode = CompositingMode.SourceCopy;

                        g.DrawImage(S9.Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].imgTile,
                                    S9.iLayersbmpPosX + S9.Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].destX,
                                    S9.iLayersbmpPosY + S9.Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].destY);
                    }
                }
            }

            frmTextureImage.pbTextureImage.SizeMode = PictureBoxSizeMode.Zoom;

            if (frmTextureImage.pbTextureImage.Image != null)
                frmTextureImage.pbTextureImage.Image.Dispose();

            frmTextureImage.pbTextureImage.Image = new Bitmap(bmpUnsIntTexture);

            bmpUnsIntTexture.Dispose();
        }
    }
}