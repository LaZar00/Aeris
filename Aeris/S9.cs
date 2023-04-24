using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Imaging;


// Sample fields for testing high layers:
// Layer 2 - woa_3
// Layer 3 - anfrst_1

namespace Aeris
{

    using static Palette;
    using static FileTools;

    public static class S9
    {

        public const int MAX_NUM_TEXTURES = 42;
        public const int MAX_LAYERS = 4;
        public const int TEXTURE_WIDTH = 256;
        public const int TEXTURE_HEIGHT = 256;
        public const int SIZE_BORDERFRAME = 16;

        // Public VARS
        public static Section_9 Section9;
        public static DirectBitmap[] textureImage = new DirectBitmap[42];
        public static DirectBitmap bmpBgImage;     // Public Image for rendering main Aeris window.
        public static List<S9_ZList> Section9Z = new List<S9_ZList>();
        public static ListStates[] ListParams;
        public static int MaxParams, MaxSublayers;
        public static int MaxTiles;              // We will count the max number of Tiles.
        public static bool IsFirstLayer;

        // Layer Dimensions for Background Aeris
        public static int iLayersMaxWidth, iLayersMaxHeight, iLayersbmpPosX, iLayersbmpPosY;

        // Layer Dimensions for Output Hashed and Base Images
        public static int iLayersMaxWidthL0, iLayersMaxHeightL0, iLayersbmpPosXL0, iLayersbmpPosYL0;
        public static int iLayersMaxWidthL2, iLayersMaxHeightL2, iLayersbmpPosXL2, iLayersbmpPosYL2;


        // LayerChecked 
        public static bool bLayerChecked0, bLayerChecked1, bLayerChecked2, bLayerChecked3;


        // This Structure will help to maintain a list of params/state
        public partial struct ListStates
        {
            public bool[] States;
            public int MaxState;
            public int MinState;
        }


        // List Structure of Layers/Textures/Tiles/Palettes links.
        // Somehow, the Z order would be Layer.ID.
        public partial struct S9_ZList
        {
            public int Z;
            public int ZTileID;
            public uint ZTileBigID;
            public int ZSublayer;          // We will enumerate Sublayers by number 0, 1, 2, 3... and so on.
            public int ZLayer;
            public int ZTile;
            public int ZTileSize;
            public int ZTileTex;
            public int ZTileAbs;
            public int ZParam;
            public int ZState;
            public int ZBlendMode;
            public int ZBlending;
            public int ZDepth;
            public int ZTexture;
            public int ZPalette;
            public int ZSourceX;
            public int ZSourceY;
            public int ZDestX;
            public int ZDestY;
        }


        // Main Section 9 Structure
        public partial struct DataTile
        {
            public ushort blank;
            public short destX;
            public short destY;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] unknown1;
            public byte sourceX;
            public byte unknown2;
            public byte sourceY;
            public byte unknown3;
            public byte sourceX2;
            public byte unknown4;
            public byte sourceY2;
            public byte unknown5;
            public ushort Width;
            public ushort Height;
            public byte paletteID;
            public byte unknown6;
            public ushort ID;
            public byte param;
            public byte statePow2;
            public byte blending;
            public byte unknown7;
            public byte BlendMode;
            public byte unknown8;
            public byte textureID;
            public byte unknown9;
            public byte textureID2;
            public byte unknown10;
            public byte depth;
            public byte unknown11;
            public uint bigID;
            public uint sourceXBig;
            public uint sourceYBig;
            public ushort blank2;
            public Bitmap imgTile;        // We will use original Bitmap object for 8bpp tiles
            public byte state;            // Used only for the code, not in data file (calculation from statePow2)
        }

        public partial struct Layer
        {
            // Public LayerID As Integer                       ' This is not in the file, but I wanted to add to check layer.
            public byte layerFlag;                         // This is readed in layers 1-3 but we'll use it for layer 0 also.
            public ushort Width;
            public ushort Height;
            public ushort numTiles;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] unknown16;   // This only for layer 1
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] unknown10;   // This only for layers 2-3
            public ushort depth;
            public ushort blank;
            public ushort blank2;
            public DataTile[] layerTiles;
        }

        public partial struct St_Texture
        {
            public ushort textureID;
            public ushort textureFlag;
            public ushort Size;
            public ushort Depth;
            public byte[,] textureMatrix;
            public ushort[,] textureMatrix2Bytes;
            public int MaxTiles;
        }

        public partial struct Section_9
        {
            public uint S9_size;
            public ushort zeroes;
            public ushort usePaddles;
            public byte activated;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] HDR_PALETTE;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] pal_ignoreFirstPixel;
            public uint palzeroes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] HDR_BACK;
            public Layer[] Layer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] HDR_TEXTURE;
            public St_Texture[] Textures;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] ENDTag;
        }


        public static void Clear_ListParams()
        {
            if (ListParams != null)
            {
                for (int i = 0; i < MaxParams; i++)
                {
                    for (int j = 0; j < ListParams[i].MaxState; j++)
                        ListParams[i].States[j] = false;

                    ListParams[i].MaxState = 0;
                    ListParams[i].MinState = 1000;
                }

                MaxParams = 0;
            }
        }

        public static void GetDrawLayersDimensions()
        {
            int iMinTmpWidth, iMaxTmpWidth, iMinTmpHeight, iMaxTmpHeight;
            int iMinTmpWidthL0, iMaxTmpWidthL0, iMinTmpHeightL0, iMaxTmpHeightL0;
            int iMinTmpWidthL2, iMaxTmpWidthL2, iMinTmpHeightL2, iMaxTmpHeightL2;

            int iNumTile, iNumLayer, iMaxTileSize, iTileSize;

            // To get Texture dimensions we will get the Min/Max values for DestY/DestX
            // of each Layer of the field.

            iMinTmpWidth = 0;
            iMaxTmpWidth = 0;
            iMinTmpHeight = 0;
            iMaxTmpHeight = 0;

            iMinTmpWidthL0 = 0;
            iMaxTmpWidthL0 = 0;
            iMinTmpHeightL0 = 0;
            iMaxTmpHeightL0 = 0;

            iMinTmpWidthL2 = 0;
            iMaxTmpWidthL2 = 0;
            iMinTmpHeightL2 = 0;
            iMaxTmpHeightL2 = 0;

            iMaxTileSize = 0;

            // Get Max and Min values of Width/Height checking all the layers.
            for (iNumLayer = 0; iNumLayer < MAX_LAYERS; iNumLayer++)
            {
                {

                    if (Section9.Layer[iNumLayer].layerFlag == 1 &&
                        Section9.Layer[iNumLayer].numTiles > 0)
                    {
                        iTileSize = Section9.Layer[iNumLayer].layerTiles[0].imgTile.Width;

                        if (iMaxTileSize < iTileSize)
                        {
                            iMaxTileSize = iTileSize;
                        }

                        for (iNumTile = 0; iNumTile < Section9.Layer[iNumLayer].numTiles; iNumTile++)
                        {
                            if ((Section9.Layer[iNumLayer].layerTiles[iNumTile].destX < 5000 &
                                Section9.Layer[iNumLayer].layerTiles[iNumTile].destX > -2000) &
                               (Section9.Layer[iNumLayer].layerTiles[iNumTile].destY < 5000 &
                                Section9.Layer[iNumLayer].layerTiles[iNumTile].destY > -2000))
                            {
                                if (iMinTmpWidth > Section9.Layer[iNumLayer].layerTiles[iNumTile].destX)
                                    iMinTmpWidth = Section9.Layer[iNumLayer].layerTiles[iNumTile].destX;

                                if (iMaxTmpWidth < Section9.Layer[iNumLayer].layerTiles[iNumTile].destX + iTileSize)
                                    iMaxTmpWidth = Section9.Layer[iNumLayer].layerTiles[iNumTile].destX + iTileSize;

                                if (iMinTmpHeight > Section9.Layer[iNumLayer].layerTiles[iNumTile].destY)
                                    iMinTmpHeight = Section9.Layer[iNumLayer].layerTiles[iNumTile].destY;

                                if (iMaxTmpHeight < Section9.Layer[iNumLayer].layerTiles[iNumTile].destY + iTileSize)
                                    iMaxTmpHeight = Section9.Layer[iNumLayer].layerTiles[iNumTile].destY + iTileSize;

                                if (iNumLayer < 2)
                                {
                                    if (iMinTmpWidthL0 > Section9.Layer[iNumLayer].layerTiles[iNumTile].destX)
                                        iMinTmpWidthL0 = Section9.Layer[iNumLayer].layerTiles[iNumTile].destX;

                                    if (iMaxTmpWidthL0 < Section9.Layer[iNumLayer].layerTiles[iNumTile].destX + iTileSize)
                                        iMaxTmpWidthL0 = Section9.Layer[iNumLayer].layerTiles[iNumTile].destX + iTileSize;

                                    if (iMinTmpHeightL0 > Section9.Layer[iNumLayer].layerTiles[iNumTile].destY)
                                        iMinTmpHeightL0 = Section9.Layer[iNumLayer].layerTiles[iNumTile].destY;

                                    if (iMaxTmpHeightL0 < Section9.Layer[iNumLayer].layerTiles[iNumTile].destY + iTileSize)
                                        iMaxTmpHeightL0 = Section9.Layer[iNumLayer].layerTiles[iNumTile].destY + iTileSize;
                                }
                                else
                                {
                                    if (iMinTmpWidthL2 > Section9.Layer[iNumLayer].layerTiles[iNumTile].destX)
                                        iMinTmpWidthL2 = Section9.Layer[iNumLayer].layerTiles[iNumTile].destX;

                                    if (iMaxTmpWidthL2 < Section9.Layer[iNumLayer].layerTiles[iNumTile].destX + iTileSize)
                                        iMaxTmpWidthL2 = Section9.Layer[iNumLayer].layerTiles[iNumTile].destX + iTileSize;

                                    if (iMinTmpHeightL2 > Section9.Layer[iNumLayer].layerTiles[iNumTile].destY)
                                        iMinTmpHeightL2 = Section9.Layer[iNumLayer].layerTiles[iNumTile].destY;

                                    if (iMaxTmpHeightL2 < Section9.Layer[iNumLayer].layerTiles[iNumTile].destY + iTileSize)
                                        iMaxTmpHeightL2 = Section9.Layer[iNumLayer].layerTiles[iNumTile].destY + iTileSize;
                                }
                            }
                        }
                    }
                }
            }

            // We need to check if there is some shifting.
            int iShiftWidth, iShiftHeight;
            int iShiftWidthL0, iShiftHeightL0, iShiftWidthL2, iShiftHeightL2;

            iShiftWidth = (iMaxTmpWidth - Math.Abs(iMinTmpWidth)) / 2 * -1;
            iShiftHeight = (iMaxTmpHeight - Math.Abs(iMinTmpHeight)) / 2 * -1;

            iShiftWidthL0 = (iMaxTmpWidthL0 - Math.Abs(iMinTmpWidthL0)) / 2 * -1;
            iShiftHeightL0 = (iMaxTmpHeightL0 - Math.Abs(iMinTmpHeightL0)) / 2 * -1;

            iShiftWidthL2 = (iMaxTmpWidthL2 - Math.Abs(iMinTmpWidthL2)) / 2 * -1;
            iShiftHeightL2 = (iMaxTmpHeightL2 - Math.Abs(iMinTmpHeightL2)) / 2 * -1;


            iLayersMaxWidth = iMaxTmpWidth - iMinTmpWidth;
            iLayersMaxHeight = iMaxTmpHeight - iMinTmpHeight;

            iLayersMaxWidthL0 = iMaxTmpWidthL0 - iMinTmpWidthL0;
            iLayersMaxHeightL0 = iMaxTmpHeightL0 - iMinTmpHeightL0;

            iLayersMaxWidthL2 = iMaxTmpWidthL2 - iMinTmpWidthL2;
            iLayersMaxHeightL2 = iMaxTmpHeightL2 - iMinTmpHeightL2;


            // We will put a frame for the stage.
            iLayersMaxWidth += SIZE_BORDERFRAME;
            iLayersMaxHeight += SIZE_BORDERFRAME;
            iLayersbmpPosX = iLayersMaxWidth / 2 + iShiftWidth;
            iLayersbmpPosY = iLayersMaxHeight / 2 + iShiftHeight;

            iLayersbmpPosXL0 = iLayersMaxWidthL0 / 2 + iShiftWidthL0;
            iLayersbmpPosYL0 = iLayersMaxHeightL0 / 2 + iShiftHeightL0;

            iLayersbmpPosXL2 = iLayersMaxWidthL2 / 2 + iShiftWidthL2;
            iLayersbmpPosYL2 = iLayersMaxHeightL2 / 2 + iShiftHeightL2;
        }

        public static void Load_Section9(BinaryReader fSection9)
        {
            // Read the Header of Section 9.
            Section9.zeroes = fSection9.ReadUInt16();
            Section9.usePaddles = fSection9.ReadUInt16();
            Section9.activated = fSection9.ReadByte();
            Section9.HDR_PALETTE = fSection9.ReadBytes(7);
            Section9.pal_ignoreFirstPixel = fSection9.ReadBytes(20);
            Section9.palzeroes = fSection9.ReadUInt32();

            // Read the Background Layers
            ReadLayers(fSection9);

            // Read the Background Textures (max = 42)?
            ReadTextures(fSection9);

            // Check: Reading last bytes.
            Section9.ENDTag = fSection9.ReadBytes(3);
            if (Encoding.ASCII.GetString(Section9.ENDTag) != "END")
            {
                MessageBox.Show("Error reading Section 9 of PC Uncompressed Field...", "Warning");
            }
            else
            {
                // PreRender Tiles drawn with its PaletteID.
                PreRender_S9Tiles();
            }

            // Let's detect the highest width/height and XPos/YPos where to draw having in mind the tiles.
            // Let's do this here to check this values only once.
            GetDrawLayersDimensions();
        }

        public static void ReadLayers(BinaryReader fSection9)
        {
            int i, iLayer;

            // We at least have Layer 1 (and I think Layer 2), but we redeem anyway for the MAX_LAYERS.
            Section9.Layer = new Layer[4];

            // Read the Background Header Layers
            Section9.HDR_BACK = fSection9.ReadBytes(4);

            // Read the Background Layers and Data Tiles, Layers are 0, 1, 2 and 3.
            for (iLayer = 0; iLayer <= 3; iLayer++)
            {
                // layerFlag is not needed for Layer 0, but we will use it anyway.
                if (iLayer == 0)
                {
                    Section9.Layer[iLayer].layerFlag = 1;
                }
                else
                {
                    Section9.Layer[iLayer].layerFlag = fSection9.ReadByte();
                }

                if (Section9.Layer[iLayer].layerFlag == 1 | iLayer == 0)
                {
                    Section9.Layer[iLayer].Width = fSection9.ReadUInt16();
                    Section9.Layer[iLayer].Height = fSection9.ReadUInt16();
                    Section9.Layer[iLayer].numTiles = fSection9.ReadUInt16();

                    switch (iLayer)
                    {
                        case 0:
                            {
                                Section9.Layer[iLayer].depth = fSection9.ReadUInt16();
                                break;
                            }

                        case 1:
                            {
                                Section9.Layer[iLayer].unknown16 = fSection9.ReadBytes(16);
                                break;
                            }

                        case 2:
                        case 3:
                            {
                                Section9.Layer[iLayer].unknown10 = fSection9.ReadBytes(10);
                                break;
                            }
                    }

                    // Read the Background Layer Data Tiles
                    Section9.Layer[iLayer].layerTiles = new DataTile[Section9.Layer[iLayer].numTiles];
                    Section9.Layer[iLayer].blank = fSection9.ReadUInt16();

                    for (i = 0; i < Section9.Layer[iLayer].numTiles; i++)
                        ReadTile(fSection9, ref Section9.Layer[iLayer].layerTiles[i], i, iLayer);

                    Section9.Layer[iLayer].blank2 = fSection9.ReadUInt16();
                }
            }

            // With the Base Info of Layers/Tiles we can construct the Palette.
            // Prepare ARGB Colors And List of Params.
            Load_BASEARGB();
            Prepare_ListParams();
        }

        public static void Prepare_ListParams()
        {
            int i, j, iLayer, iTile;

            // Initialize Vars
            ListParams = null;
            ListParams = new ListStates[MaxParams + 1];

            for (i = 1; i <= MaxParams; i++)
                ListParams[i].MinState = 1000;

            for (iLayer = 1; iLayer <= MAX_LAYERS - 1; iLayer++)
            {
                {
                    if (Section9.Layer[iLayer].layerFlag == 1)
                    {
                        for (iTile = 0; iTile < Section9.Layer[iLayer].numTiles; iTile++)
                        {
                            // Check MaxState and MinState
                            {
                                if (ListParams[Section9.Layer[iLayer].layerTiles[iTile].param].MaxState < 
                                    Section9.Layer[iLayer].layerTiles[iTile].state)

                                        ListParams[Section9.Layer[iLayer].layerTiles[iTile].param].MaxState = 
                                                Section9.Layer[iLayer].layerTiles[iTile].state;

                                if (ListParams[Section9.Layer[iLayer].layerTiles[iTile].param].MinState > 
                                    Section9.Layer[iLayer].layerTiles[iTile].state)

                                    ListParams[Section9.Layer[iLayer].layerTiles[iTile].param].MinState = 
                                                Section9.Layer[iLayer].layerTiles[iTile].state;
                            }
                        }
                    }
                }
            }

            for (i = 1; i <= MaxParams; i++)
                ListParams[i].States = new bool[ListParams[i].MaxState + 1];

            for (i = 1; i <= MaxParams; i++)
            {
                
                for (j = 0; j <= ListParams[i].MaxState; j++)
                {
                    if (j >= ListParams[i].MinState & j <= ListParams[i].MaxState)
                        ListParams[i].States[j] = true;
                    else
                        ListParams[i].States[j] = false;
                }
            }
        }

        public static void ReadTile(BinaryReader fSection9, ref DataTile layerTile, int iTile, int iLayer)
        {
            layerTile.blank = fSection9.ReadUInt16();
            layerTile.destX = fSection9.ReadInt16();
            layerTile.destY = fSection9.ReadInt16();
            layerTile.unknown1 = fSection9.ReadBytes(4);
            layerTile.sourceX = fSection9.ReadByte();
            layerTile.unknown2 = fSection9.ReadByte();
            layerTile.sourceY = fSection9.ReadByte();
            layerTile.unknown3 = fSection9.ReadByte();
            layerTile.sourceX2 = fSection9.ReadByte();
            layerTile.unknown4 = fSection9.ReadByte();
            layerTile.sourceY2 = fSection9.ReadByte();
            layerTile.unknown5 = fSection9.ReadByte();
            layerTile.Width = fSection9.ReadUInt16();
            layerTile.Height = fSection9.ReadUInt16();
            layerTile.paletteID = fSection9.ReadByte();
            layerTile.unknown6 = fSection9.ReadByte();
            layerTile.ID = fSection9.ReadUInt16();
            layerTile.param = fSection9.ReadByte();
            layerTile.statePow2 = fSection9.ReadByte();
            layerTile.blending = fSection9.ReadByte();
            layerTile.unknown7 = fSection9.ReadByte();
            layerTile.BlendMode = fSection9.ReadByte();
            layerTile.unknown8 = fSection9.ReadByte();
            layerTile.textureID = fSection9.ReadByte();
            layerTile.unknown9 = fSection9.ReadByte();
            layerTile.textureID2 = fSection9.ReadByte();

            // This is for field "bugin1"
            if (layerTile.textureID2 > 42)
            {
                layerTile.textureID2 = 15;
            }

            // This is for field "trnad_3"
            if (layerTile.textureID2 > 0 & iLayer == 0 & strGlobalFieldName == "trnad_3")
            {
                layerTile.textureID2 = 0;
            }

            layerTile.unknown10 = fSection9.ReadByte();
            layerTile.depth = fSection9.ReadByte();
            layerTile.unknown11 = fSection9.ReadByte();
            layerTile.bigID = fSection9.ReadUInt32();
            layerTile.sourceXBig = fSection9.ReadUInt32();
            layerTile.sourceYBig = fSection9.ReadUInt32();
            layerTile.blank2 = fSection9.ReadUInt16();

            // I found in some field that param = 2 when Layer is 0. That's not possible.
            // Param = 0 and State = 0 for Layer 0.
            // Let's correct this here. We also prepare some vars about params/states.
            if (iLayer == 0)
            {
                layerTile.param = 0;
                layerTile.state = 0;
            }
            else
            {
                // Check MaxParams
                if (MaxParams < layerTile.param)
                    MaxParams = layerTile.param;
                // Prepare state
                if (layerTile.statePow2 > 0)
                {
                    layerTile.state = (byte)Math.Round(Math.Log(layerTile.statePow2, 2d));
                }
                else
                {
                    layerTile.state = 0;
                }
            }
        }

        public static void ReadTextures(BinaryReader fSection9)
        {
            int iTexture, textureMatrixWidth, textureMatrixHeight, xTexture, yTexture;

            // Read the Textures Header
            Section9.HDR_TEXTURE = fSection9.ReadBytes(7);

            textureMatrixWidth = TEXTURE_WIDTH;
            textureMatrixHeight = TEXTURE_HEIGHT;

            // Read Textures Data
            {

                if (Section9.Textures == null)
                    Section9.Textures = new St_Texture[MAX_NUM_TEXTURES];

                for (iTexture = 0; iTexture < MAX_NUM_TEXTURES; iTexture++)
                {                  

                    Section9.Textures[iTexture].textureID = (ushort)iTexture;
                    Section9.Textures[iTexture].textureFlag = fSection9.ReadUInt16();

                    if (Section9.Textures[iTexture].textureFlag == 1)
                    {
                        Section9.Textures[iTexture].Size = fSection9.ReadUInt16();
                        Section9.Textures[iTexture].Depth = fSection9.ReadUInt16();

                        if (Section9.Textures[iTexture].Depth < 2)
                        {
                            Section9.Textures[iTexture].textureMatrix = new byte[textureMatrixWidth, textureMatrixHeight];
                          
                            for (yTexture = 0; yTexture <= textureMatrixWidth - 1; yTexture++)
                            {
                                for (xTexture = 0; xTexture <= textureMatrixHeight - 1; xTexture++)
                                    Section9.Textures[iTexture].textureMatrix[xTexture, yTexture] = fSection9.ReadByte();
                            }
                        }
                        else
                        {
                            Section9.Textures[iTexture].textureMatrix2Bytes = new ushort[textureMatrixWidth, textureMatrixHeight];

                            for (yTexture = 0; yTexture <= textureMatrixWidth - 1; yTexture++)
                            {
                                for (xTexture = 0; xTexture <= textureMatrixHeight - 1; xTexture++)
                                    Section9.Textures[iTexture].textureMatrix2Bytes[xTexture, yTexture] = fSection9.ReadUInt16();
                            }
                        }

                        // Let's prepare the texture image space.
                        if (textureImage[iTexture] != null)
                        {
                            textureImage[iTexture].Dispose();
                            textureImage[iTexture] = null;
                        }
                        textureImage[iTexture] = new DirectBitmap(TEXTURE_WIDTH, TEXTURE_HEIGHT);
                    }
                }
            }
        }

        public static void PreRender_IndexedTile(ref DataTile dataTile, 
                                                 int iTexture, 
                                                 int iSourceX, 
                                                 int iSourceY)
        {
            int xTile, yTile;
            int iPalette;
            Color tilePixelColor;
            ColorPalette tmpPal;
            byte bytePixel;

            tmpPal = dataTile.imgTile.Palette;
            iPalette = dataTile.paletteID;

            if (iPalette < S4.Section4.numPalettes)
            {
                for (yTile = 0; yTile < dataTile.imgTile.Width; yTile++)
                {
                    for (xTile = 0; xTile < dataTile.imgTile.Height; xTile++)
                    {
                        bytePixel = Section9.Textures[iTexture].textureMatrix[iSourceX + xTile,
                                                                              iSourceY + yTile];

                        Set8bppIndexedPixel(ref dataTile.imgTile, xTile, yTile, bytePixel);
                        tilePixelColor = ARGB_BASEPAL[iPalette].ARGB_COLORS[bytePixel];

                        if (S4.Section4.dataPalette[iPalette].Pal[bytePixel].Red == 0 &
                            S4.Section4.dataPalette[iPalette].Pal[bytePixel].Green == 0 &
                            S4.Section4.dataPalette[iPalette].Pal[bytePixel].Blue == 0 &
                            S4.Section4.dataPalette[iPalette].Pal[bytePixel].Mask == 0)
                        {
                            tilePixelColor = ARGB_BASEPAL[iPalette].ARGB_COLORS[0];
                        }

                        if (Section9.pal_ignoreFirstPixel[iPalette] == 1 & bytePixel == 0)
                        {
                            tilePixelColor = Color.FromArgb(0,
                                                            0,
                                                            0,
                                                            0);
                        }

                        tmpPal.Entries[bytePixel] = tilePixelColor;

                        // Let's put here directly the newly Tile drawn into the texture bitmap.   
                        textureImage[iTexture].SetPixel(iSourceX + xTile, iSourceY + yTile, tilePixelColor);

                    }
                }
            }

            dataTile.imgTile.Palette = tmpPal;
        }

        public static void PreRender_DirectTile(ref DataTile dataTile, 
                                                int iTexture,
                                                int iSourceX,
                                                int iSourceY)
        {
            Color cTextureDirectColor;
            int xTile, yTile;

            for (yTile = 0; yTile <= dataTile.imgTile.Width - 1; yTile++)
            {
                for (xTile = 0; xTile <= dataTile.imgTile.Height - 1; xTile++)
                {
                    cTextureDirectColor = 
                        Get16bitColor(Section9.Textures[iTexture].
                                      textureMatrix2Bytes[dataTile.sourceX + xTile,
                                      dataTile.sourceY + yTile]);

                    // Let's put the color in the image of the Tile with Direct color.
                    dataTile.imgTile.SetPixel(xTile, yTile, cTextureDirectColor);

                    // Let's put here directly the newly Tile drawn into the texture bitmap.
                    textureImage[iTexture].SetPixel(iSourceX + xTile, iSourceY + yTile, cTextureDirectColor);
                }
            }
        }

        public static void PreRender_S9Tiles()
        {
            // Standard Vars
            int iLayer, iTile, iTileWidth, iTileHeight;       // counter layer, counter tile, tile size, tile x/y pos for draw
            int iTexture, iSourceX, iSourceY;            // pixel value (indexed color of the paletteID)

            // We will prerender each Tile of each Layer with its palette.
            for (iLayer = 0; iLayer < MAX_LAYERS; iLayer++)
            {
                {

                    if (Section9.Layer[iLayer].layerFlag == 1 &
                        Section9.Layer[iLayer].numTiles > 0)
                    {
                        // If .layerTiles(0).Width > 16 Then
                        if (iLayer > 1)
                        {
                            iTileWidth = 32;
                            iTileHeight = 32;
                        }
                        else
                        {
                            iTileWidth = 16;
                            iTileHeight = 16;
                        }

                        for (iTile = 0; iTile < Section9.Layer[iLayer].numTiles; iTile++)
                        {
                            {

                                // Let's determine the texture and x/y source positions of the texture used.
                                iTexture = Section9.Layer[iLayer].layerTiles[iTile].textureID;
                                iSourceX = Section9.Layer[iLayer].layerTiles[iTile].sourceX;
                                iSourceY = Section9.Layer[iLayer].layerTiles[iTile].sourceY;

                                if (Section9.Layer[iLayer].layerTiles[iTile].textureID2 >= 0xF)
                                {
                                    iTexture = Section9.Layer[iLayer].layerTiles[iTile].textureID2;
                                    iSourceX = Section9.Layer[iLayer].layerTiles[iTile].sourceX2;
                                    iSourceY = Section9.Layer[iLayer].layerTiles[iTile].sourceY2;
                                }

                                // Check if there is textureFlag
                                if (Section9.Textures[iTexture].textureFlag == 1)
                                {
                                    // We check if the texture has palette or direct color.
                                    if (Section9.Textures[iTexture].Depth < 2)
                                    {
                                        // We create the Tile palettized.
                                        Section9.Layer[iLayer].layerTiles[iTile].imgTile = 
                                                        new Bitmap(iTileWidth, iTileHeight, PixelFormat.Format8bppIndexed);

                                        PreRender_IndexedTile(ref Section9.Layer[iLayer].layerTiles[iTile],
                                                              iTexture, iSourceX, iSourceY);
                                    }
                                    else
                                    {
                                        // We create the Tile with Direct Color
                                        Section9.Layer[iLayer].layerTiles[iTile].imgTile = 
                                                        new Bitmap(iTileWidth, iTileHeight, PixelFormat.Format32bppArgb);

                                        PreRender_DirectTile(ref Section9.Layer[iLayer].layerTiles[iTile],
                                                             iTexture, iSourceX, iSourceY);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AssignZDepthToLayerAsPerFLEVEL(ref S9_ZList ZItem)
        {
            switch (strGlobalFieldName)
            {
                case "crater_1":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 3840;
                        }

                        break;
                    }

                case "del3":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 256;
                        }

                        break;
                    }

                case "hill":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 512;
                        }

                        break;
                    }

                case "hill2":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 512;
                        }

                        break;
                    }

                case "junonr2":
                    {
                        if (ZItem.ZLayer == 2)
                        {
                            ZItem.ZTileID = 415;
                        }

                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 416;
                        }

                        break;
                    }

                case "junonl2":
                    {
                        if (ZItem.ZLayer == 2)
                        {
                            ZItem.ZTileID = 433;
                        }

                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 434;
                        }

                        break;
                    }

                case "junair":
                    {
                        if (ZItem.ZLayer == 2)
                        {
                            ZItem.ZTileID = 8;
                        }

                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 16;
                        }

                        break;
                    }

                case "las1_1":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 3584;
                        }

                        break;
                    }

                case "loslake1":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 594;
                        }

                        break;
                    }

                case "trnad_3":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 4080;
                        }

                        break;
                    }

                case "trnad_4":
                    {
                        if (ZItem.ZLayer == 2)
                        {
                            ZItem.ZTileID = 4092;
                        }

                        break;
                    }

                case "ujunon2":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 4094;
                        }

                        break;
                    }

                case "ujunon3":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 4080;
                        }

                        break;
                    }

                case "woa_1":
                    {
                        if (ZItem.ZLayer == 2)
                        {
                            ZItem.ZTileID = 628;
                        }

                        break;
                    }

                case "woa_2":
                    {
                        if (ZItem.ZLayer == 2)
                        {
                            ZItem.ZTileID = 706;
                        }

                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 705;
                        }

                        break;
                    }

                case "woa_3":
                    {
                        if (ZItem.ZLayer == 2)
                        {
                            ZItem.ZTileID = 466;
                        }

                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 465;
                        }

                        break;
                    }

                case "zcoal_1":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 3840;
                        }

                        break;
                    }

                case "zcoal_3":
                    {
                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 4080;
                        }

                        break;
                    }

                case "ztruck":
                    {
                        if (ZItem.ZLayer == 2)
                        {
                            ZItem.ZTileID = 3856;
                        }

                        if (ZItem.ZLayer == 3)
                        {
                            ZItem.ZTileID = 3600;
                        }

                        break;
                    }
            }
        }

        public static void Load_ZList(RichTextBox rtbResult)
        {
            int iLayer, iTile, iTileCounter, iTileTexCounter;
            int oldTexture, oldID;

            S9_ZList ZItem;

            if (Section9Z != null) Section9Z.Clear();

            iTileCounter = 0;
            ZItem = new S9_ZList { Z = 0 };

            for (iLayer = 0; iLayer <= MAX_LAYERS - 1; iLayer++)
            {
                if (Section9.Layer[iLayer].layerFlag == 1 &
                    Section9.Layer[iLayer].numTiles > 0)
                {
                    {

                        for (iTile = 0; iTile < Section9.Layer[iLayer].numTiles; iTile++)
                        {
                            {
                                ZItem.ZLayer = iLayer;
                                ZItem.ZTileID = Section9.Layer[iLayer].layerTiles[iTile].ID;
                                ZItem.ZTileBigID = Section9.Layer[iLayer].layerTiles[iTile].bigID;

                                // Some fields have assigned ZDepth by script.
                                AssignZDepthToLayerAsPerFLEVEL(ref ZItem);

                                ZItem.ZTile = iTile;
                                ZItem.ZTileAbs = iTileCounter;
                                ZItem.ZTileSize = Section9.Layer[iLayer].layerTiles[iTile].imgTile.Width;
                                ZItem.ZParam = Section9.Layer[iLayer].layerTiles[iTile].param;
                                ZItem.ZState = Section9.Layer[iLayer].layerTiles[iTile].state;
                                ZItem.ZBlendMode = Section9.Layer[iLayer].layerTiles[iTile].BlendMode;
                                ZItem.ZBlending = Section9.Layer[iLayer].layerTiles[iTile].blending;
                                ZItem.ZDepth = Section9.Layer[iLayer].layerTiles[iTile].depth;
                                ZItem.ZPalette = Section9.Layer[iLayer].layerTiles[iTile].paletteID;
                                ZItem.ZDestX = Section9.Layer[iLayer].layerTiles[iTile].destX;
                                ZItem.ZDestY = Section9.Layer[iLayer].layerTiles[iTile].destY;

                                if (Section9.Layer[iLayer].layerTiles[iTile].textureID2 == 0)
                                {
                                    ZItem.ZTexture = Section9.Layer[iLayer].layerTiles[iTile].textureID;
                                    ZItem.ZSourceX = Section9.Layer[iLayer].layerTiles[iTile].sourceX;
                                    ZItem.ZSourceY = Section9.Layer[iLayer].layerTiles[iTile].sourceY;
                                }
                                else
                                {
                                    ZItem.ZTexture = Section9.Layer[iLayer].layerTiles[iTile].textureID2;
                                    ZItem.ZSourceX = Section9.Layer[iLayer].layerTiles[iTile].sourceX2;
                                    ZItem.ZSourceY = Section9.Layer[iLayer].layerTiles[iTile].sourceY2;
                                }
                            }

                            if (iLayer == 1 & ZItem.ZTileID > 4000 & ZItem.ZTexture > 0xE)
                                ZItem.Z = ZItem.ZTileID;
                            else
                                ZItem.Z = 4096 - ZItem.ZTileID;

                            Section9Z.Add(ZItem);
                            iTileCounter++;
                        }
                    }
                }
            }


            // ----- Let's prepare the Tiles/Textures order values for searching.
            // This is needed for show the Tile and Info part and because at first glance the Tiles are not
            // ordered by Texture/Tile.
            iTileTexCounter = 0;

            Section9Z = Section9Z.OrderBy(x => x.ZTexture).ThenBy(x => x.ZSourceY).ThenBy(x => x.ZSourceX).ToList();
            oldTexture = Section9Z[0].ZTexture;

            for (iTile = 0; iTile <= Section9Z.Count - 1; iTile++)
            {
                ZItem = Section9Z[iTile];

                if (logEvents.bActivateLogging)
                {
                    logEvents.AddEventText(
                                 "LY: " + ZItem.ZLayer.ToString("0") +
                                 "\tID: " + ZItem.ZTileID.ToString("0000") +
                                 "\tBGID: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].bigID.ToString("0000") +
                                 "\tTL: " + ZItem.ZTile.ToString("0000") +
                                 "\tPL: " + ZItem.ZPalette.ToString("00") +
                                 "\tBL: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].blending.ToString("0") +
                                 "\tTT: " + ZItem.ZBlendMode.ToString("0") +
                                 "\tTX: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].textureID.ToString("00") +
                                 "\tTX2: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].textureID2.ToString("00") +
                                 "\tPR: " + ZItem.ZParam.ToString("00") +
                                 "\tST: " + ZItem.ZState.ToString("00") +
                                 "\tDP: " + ZItem.ZDepth.ToString("00") +
                                 "\tSRCX: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].sourceX.ToString("0000") +
                                 "\tSRCY: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].sourceY.ToString("0000") +
                                 "\tDSTX: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].destX.ToString("0000") +
                                 "\tDSTY: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].destY.ToString("0000") +
                                 "\tSRCX2: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].sourceX2.ToString("0000") +
                                 "\tSRCY2: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].sourceY2.ToString("0000") +
                                 "\tSRCXB: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].sourceXBig.ToString("0000") +
                                 "\tSRCYB: " + Section9.Layer[ZItem.ZLayer].layerTiles[ZItem.ZTile].sourceYBig.ToString("0000"),
                                 rtbResult);
                }

                if (oldTexture == Section9Z[iTile].ZTexture)
                {
                    ZItem.ZTileTex = iTileTexCounter;
                    iTileTexCounter++;
                }
                else
                {
                    Section9.Textures[oldTexture].MaxTiles = iTileTexCounter - 1;
                    iTileTexCounter = 1;
                    ZItem.ZTileTex = 0;
                    oldTexture = Section9Z[iTile].ZTexture;
                }

                Section9Z[iTile] = ZItem;
            }

            Section9.Textures[oldTexture].MaxTiles = iTileTexCounter - 1;


            // ----- Let's prepare the Sublayers for Layer 1.
            // Let's Order the ZList and Put some tile/texture order values for searching.
            // This is needed for show the Tile and Info part and because at first glance the Tiles are not
            // ordered by Texture/Tile.
            Section9Z = Section9Z.OrderBy(x => x.ZLayer).ThenBy(x => x.ZTileID).ToList();

            oldID = -1;
            MaxSublayers = -1;

            for (iTile = 0; iTile <= Section9Z.Count - 1; iTile++)
            {
                if (Section9Z[iTile].ZLayer == 1)
                {
                    ZItem = Section9Z[iTile];

                    if (oldID < Section9Z[iTile].ZTileID)
                    {
                        MaxSublayers++;
                        oldID = Section9Z[iTile].ZTileID;
                    }

                    ZItem.ZSublayer = MaxSublayers;
                    Section9Z[iTile] = ZItem;
                }
            }

            if (MaxSublayers != -1) MaxSublayers++;


            // ------ Let's leave the final Order by TileAbsolute value.
            Section9Z = Section9Z.OrderBy(x => x.ZTileAbs).ToList();
        }

        public static bool CheckedSublayer(int ZSublayer, FrmAeris frmAeris)
        {
            return (frmAeris.clbSublayers.GetItemCheckState(ZSublayer) == CheckState.Checked);
        }

        public static void Render_S9Layers(FrmAeris frmAeris)
        {
            DirectBitmap bmpBgTile;

            bool bRenderTile;
            List<S9_ZList> sortZList = new List<S9_ZList>();

            // Bitmap Background init
            bmpBgImage = new DirectBitmap(iLayersMaxWidth, iLayersMaxHeight);

            if (ImageTools.FillBGBlackImage)
            {
                bmpBgImage.Clear(Color.Black);
            }
            else
            {
                bmpBgImage.Clear(Color.Transparent);
            }


            sortZList.Clear();
                sortZList = (from sortZItem in Section9Z
                             where (sortZItem.ZLayer == 0 & bLayerChecked0 | 
                                    sortZItem.ZLayer == 1 & bLayerChecked1 | 
                                    sortZItem.ZLayer == 2 & bLayerChecked2 | 
                                    sortZItem.ZLayer == 3 & bLayerChecked3) &&
                                   (sortZItem.ZDestX > -2000 & sortZItem.ZDestX < 5000)  // check "out of boundaries" tiles

                             orderby sortZItem.Z, sortZItem.ZTileBigID descending
                             select sortZItem).ToList();

                if (sortZList.Count > 0)
                {

                    // Let's tell to the List which Tiles to Render and which not.
                    foreach (var sortZItem in sortZList)
                    {
                        bRenderTile = false;

                        // Let's tell to the List which Tiles to Render and which not.
                        switch (sortZItem.ZLayer)
                        {
                            case 0:
                                {
                                    bRenderTile = true;
                                    break;
                                }

                            case 1:
                                {
                                    if (CheckedSublayer(sortZItem.ZSublayer, frmAeris))
                                    {
                                        if (sortZItem.ZTexture < 0xF & sortZItem.ZParam == 0)
                                        {
                                            bRenderTile = true;
                                        }
                                        else if (ImageTools.bRenderEffects)
                                        {
                                            if (sortZItem.ZParam > 0)
                                            {
                                                if (IsCheckedParamState(sortZItem.ZParam, sortZItem.ZState))
                                                {
                                                    bRenderTile = true;
                                                }
                                            }
                                            else
                                            {
                                                bRenderTile = true;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 2:
                            case 3:
                                {
                                    if (ImageTools.bRenderEffects)
                                    {
                                        if (sortZItem.ZParam > 0)
                                        {
                                            if (IsCheckedParamState(sortZItem.ZParam, sortZItem.ZState))
                                            {
                                                bRenderTile = true;
                                            }
                                        }
                                        else
                                        {
                                            bRenderTile = true;
                                        }
                                    }

                                    break;
                                }
                        }

                        if (logEvents.bActivateLogging)
                        {
                            logEvents.AddEventText(
                                         "Z: " + sortZItem.Z.ToString("000000") +
                                         "\tLY: " + sortZItem.ZLayer.ToString("0") +
                                         "\tID: " + sortZItem.ZTileID.ToString("0000") +
                                         "\tTL: " + sortZItem.ZTile.ToString("0000") +
                                         "\tPL: " + sortZItem.ZPalette.ToString("00") +
                                         "\tBL: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].blending.ToString("0") +
                                         "\tTT: " + sortZItem.ZBlendMode.ToString("0") +
                                         "\tTX: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].textureID.ToString("00") +
                                         "\tTX2: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].textureID2.ToString("00") +
                                         "\tPR: " + sortZItem.ZParam.ToString("00") +
                                         "\tST: " + sortZItem.ZState.ToString("00") +
                                         "\tDP: " + sortZItem.ZDepth.ToString("00") +
                                         "\tSRCX: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].sourceX.ToString("0000") +
                                         "\tSRCY: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].sourceY.ToString("0000") +
                                         "\tDSTX: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].destX.ToString("0000") +
                                         "\tDSTY: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].destY.ToString("0000") +
                                         "\tSRCX2: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].sourceX2.ToString("0000") +
                                         "\tSRCY2: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].sourceY2.ToString("0000") +
                                         "\tBGID: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].bigID.ToString("0000") +
                                         "\tSRCXB: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].sourceXBig.ToString("0000") +
                                         "\tSRCYB: " + Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile].sourceYBig.ToString("0000"),
                                         frmAeris.rtbEvents);
                        }

                        if (bRenderTile)
                        {

                            // Let's decide which palette/tile/texture render to the background/stage.
                            // Let's check if it is an indexed tile or a direct tile.
                            {

                                bmpBgTile = new DirectBitmap(sortZItem.ZTileSize, sortZItem.ZTileSize);

                                bmpBgTile.GetTileFromBackground(bmpBgImage, iLayersbmpPosX + sortZItem.ZDestX,
                                                                            iLayersbmpPosY + sortZItem.ZDestY);

                                bmpBgTile.Render_Tile(Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile]);

                                bmpBgImage.DrawTileToBackground(bmpBgTile,
                                                                iLayersbmpPosX + sortZItem.ZDestX,
                                                                iLayersbmpPosY + sortZItem.ZDestY,
                                                                true);

                                bmpBgTile.Dispose();
                            }
                        }
                    }
                }
        }

        public static void Render_S9BaseLayer(ref DirectBitmap bmpBase, FrmAeris frmAeris)
        {
            List<S9_ZList> sortZList = new List<S9_ZList>();

            //Bitmap bmpBgTile;
            DirectBitmap bmpBgTile;

            // Prepare the background.
            bmpBase.Clear(Color.Black);

            // Prepare Tile list
            sortZList.Clear();
            sortZList = (from sortZItem in Section9Z
                         where (sortZItem.ZState == ListParams[sortZItem.ZParam].MinState) &
                               (sortZItem.ZDestX > -2000 & sortZItem.ZDestX < 5000)  // check "out of boundaries" tiles
                         orderby sortZItem.Z, 
                                 sortZItem.ZTileBigID descending
                         select sortZItem).ToList();


            // Let's decide which palette/tile/texture render to the background/stage.
            // Let's check if it is an indexed tile or a direct tile.
            if (sortZList.Count > 0)
            {
                foreach (var sortZItem in sortZList)
                {
                    bmpBgTile = new DirectBitmap(sortZItem.ZTileSize, sortZItem.ZTileSize);

                    bmpBgTile.GetTileFromBackground(bmpBase, iLayersbmpPosX + sortZItem.ZDestX,
                                                             iLayersbmpPosY + sortZItem.ZDestY);

                    bmpBgTile.Render_Tile(Section9.Layer[sortZItem.ZLayer].layerTiles[sortZItem.ZTile]);

                    bmpBase.DrawTileToBackground(bmpBgTile, iLayersbmpPosX + sortZItem.ZDestX,
                                                            iLayersbmpPosY + sortZItem.ZDestY,
                                                    true);

                    bmpBgTile.Dispose();
                }
            }
        }

        public static bool IsCheckedParamState(int iParam, int iState)
        {
            return ListParams[iParam].States[iState];
        }


        // What is this procedure for?
        // Ok. As we can Repair for example fr_e with new Palettes, we need to update also
        // the base memory data of Section 9 with new data (new Tiles and Textures).
        public static void Prepare_Section9()
        {
            byte[] bgArray;       // Background Array
            int iByteDimension, inumLayer, inumTile;

            // Now we will get the size of the complete Background Array.
            iByteDimension = Redimension_BackgroundArray();
            bgArray = new byte[iByteDimension];


            // Now we can write things.
            // Let's begin by writing Background HD, Palette HDR and Tiles.
            using (var bgMemory = new MemoryStream(bgArray))
            {
                using (var bgWriter = new BinaryWriter(bgMemory))
                {
                    {
                        // ' Background HDR (S9_size is not added now):
                        bgWriter.Write(Section9.zeroes);
                        bgWriter.Write(Section9.usePaddles);
                        bgWriter.Write(Section9.activated);

                        // ' Background PALETTE HDR:
                        bgWriter.Write(Section9.HDR_PALETTE);
                        bgWriter.Write(Section9.pal_ignoreFirstPixel);
                        bgWriter.Write(Section9.palzeroes);

                        // ' Background LAYERS + TILES:
                        bgWriter.Write(Section9.HDR_BACK);

                        for (inumLayer = 0; inumLayer <= 3; inumLayer++)
                        {
                            if (inumLayer > 0)
                                bgWriter.Write(Section9.Layer[inumLayer].layerFlag);

                            if (Section9.Layer[inumLayer].layerFlag == 1 | inumLayer == 0)
                            {
                                bgWriter.Write(Section9.Layer[inumLayer].Width);
                                bgWriter.Write(Section9.Layer[inumLayer].Height);
                                bgWriter.Write(Section9.Layer[inumLayer].numTiles);

                                switch (inumLayer)
                                {
                                    case 0:
                                        {
                                            bgWriter.Write(Section9.Layer[inumLayer].depth);
                                            break;
                                        }

                                    case 1:
                                        {
                                            bgWriter.Write(Section9.Layer[inumLayer].unknown16);
                                            break;
                                        }

                                    case 2:
                                    case 3:
                                        {
                                            bgWriter.Write(Section9.Layer[inumLayer].unknown10);
                                            break;
                                        }
                                }

                                bgWriter.Write(Section9.Layer[inumLayer].blank);

                                // Write Tiles of the Layer
                                for (inumTile = 0; inumTile < Section9.Layer[inumLayer].numTiles; inumTile++)
                                {
                                    var argbgWriter = bgWriter;

                                    WriteTile(ref argbgWriter, 
                                              Section9.Layer[inumLayer].layerTiles[inumTile], 
                                              inumTile, 
                                              inumLayer);
                                }

                                bgWriter.Write(Section9.Layer[inumLayer].blank2);
                            }
                        }

                        // ' Background TEXTURES:
                        // Write Texture Section HDR:
                        bgWriter.Write(Section9.HDR_TEXTURE);

                        for (int inumTexture = 0; inumTexture <= MAX_NUM_TEXTURES - 1; inumTexture++)
                        {
                            bgWriter.Write(Section9.Textures[inumTexture].textureFlag);

                            // If textureFlag = 1 we add-> 2 + 2:  Size + Depth
                            if (Section9.Textures[inumTexture].textureFlag == 1)
                            {
                                bgWriter.Write(Section9.Textures[inumTexture].Size);
                                bgWriter.Write(Section9.Textures[inumTexture].Depth);

                                // We calculate here the Dimension of the Texture Data.
                                // All the Textures have 256 pixels Width * 256 pixels Height                
                                // It depends On Depth. If Depth < 1 Is Of 1 Byte, If Depth = 2, 2 bytes
                                if (Section9.Textures[inumTexture].Depth < 2)
                                {
                                    for (int y = 0; y <= TEXTURE_HEIGHT - 1; y++)
                                    {
                                        for (int x = 0; x <= TEXTURE_WIDTH - 1; x++)
                                            bgWriter.Write(Section9.Textures[inumTexture].textureMatrix[x, y]);
                                    }
                                }
                                else
                                {
                                    for (int y = 0; y <= TEXTURE_HEIGHT - 1; y++)
                                    {
                                        for (int x = 0; x <= TEXTURE_WIDTH - 1; x++)
                                            bgWriter.Write(Section9.Textures[inumTexture].textureMatrix2Bytes[x, y]);
                                    }
                                }
                            }
                        }

                        // Finally we need to add the "END" Tag of Texture Section.
                        bgWriter.Write(Section9.ENDTag);
                    }
                }

                // Finally we change the Section9 of the loaded field for this one.
                Field.fieldSection[8].sectionData = bgArray;
            }
        }

        public static void WriteTile(ref BinaryWriter bgWriter, DataTile stTile, int inumTile, int inumLayer)
        {
            bgWriter.Write(stTile.blank);
            bgWriter.Write(stTile.destX);
            bgWriter.Write(stTile.destY);
            bgWriter.Write(stTile.unknown1);
            bgWriter.Write(stTile.sourceX);
            bgWriter.Write(stTile.unknown2);
            bgWriter.Write(stTile.sourceY);
            bgWriter.Write(stTile.unknown3);
            bgWriter.Write(stTile.sourceX2);
            bgWriter.Write(stTile.unknown4);
            bgWriter.Write(stTile.sourceY2);
            bgWriter.Write(stTile.unknown5);
            bgWriter.Write(stTile.Width);
            bgWriter.Write(stTile.Height);
            bgWriter.Write(stTile.paletteID);
            bgWriter.Write(stTile.unknown6);
            bgWriter.Write(stTile.ID);
            bgWriter.Write(stTile.param);
            bgWriter.Write(stTile.statePow2);
            bgWriter.Write(stTile.blending);
            bgWriter.Write(stTile.unknown7);
            bgWriter.Write(stTile.BlendMode);
            bgWriter.Write(stTile.unknown8);
            bgWriter.Write(stTile.textureID);
            bgWriter.Write(stTile.unknown9);
            bgWriter.Write(stTile.textureID2);
            bgWriter.Write(stTile.unknown10);
            bgWriter.Write(stTile.depth);
            bgWriter.Write(stTile.unknown11);
            bgWriter.Write(stTile.bigID);
            bgWriter.Write(stTile.sourceXBig);
            bgWriter.Write(stTile.sourceYBig);
            bgWriter.Write(stTile.blank2);
        }

        public static int Redimension_BackgroundArray()
        {
            int inumLayer, inumTexture;
            int iByteDimension;

            // ' References:
            // ' Background HDR (S9_size is not added now):
            // 2 + 2 + 1: zeroes 2 bytes, usePaddles 2, activated 1
            iByteDimension = 5;

            // ' Background PALETTE HDR:
            // 7 + 20 + 4: "PALETTE" 7 + ignoreFirstPixel 20 + palzeroes 4
            iByteDimension += 31;

            // Background LAYERS:
            for (inumLayer = 0; inumLayer < MAX_LAYERS; inumLayer++)
            {
                if (inumLayer == 0)
                {
                    // Background Layer 0:
                    // Each Tile has a size of 52bytes.
                    // 4 + 2 + 2 + 2 + 2 + 2 + (numTiles * 52, if Depth < 2) + 2:
                    // "BACK" + Width 2 + Height 2 + numTiles 2 + Depth 2 + blank 2 + Tiles * 52 + blank2 2
                    iByteDimension = iByteDimension + 16 + (Section9.Layer[0].numTiles * 52);
                }
                else
                {
                    // Background Layer 1-3:
                    // 1:  layerFlag
                    iByteDimension++;

                    if (Section9.Layer[inumLayer].layerFlag == 1)
                    {
                        // Background Layer 1:
                        // 2 + 2 + 2 + 2 + 16 + 2:
                        // Width 2 + Height 2 + numTiles 2 + blank 2 + (numTiles * 52, If Depth < 2) + unknown16 + blank2 2
                        // Background Layer 2,3:
                        // 2 + 2 + 2 + 2 + 10 + 2:  
                        // Width 2 + Height 2 + numTiles 2 + blank 2 + (numTiles * 52, If Depth < 2) + unknown10 + blank2 2
                        iByteDimension = iByteDimension + 10 + (Section9.Layer[inumLayer].numTiles * 52);
                        switch (inumLayer)
                        {
                            case 1:
                                {
                                    iByteDimension += 16;
                                    break;
                                }

                            case 2:
                            case 3:
                                {
                                    iByteDimension += 10;
                                    break;
                                }
                        }
                    }
                }
            }

            // ' Background TEXTURES:
            // We calculate the Dimension of the Textures (they can be a Matrix of 1 or 2 bytes),
            // depending on the Depth.
            // First we add the Texture Section HDR.
            // 7: "TEXTURE"
            iByteDimension += 7;

            // For Each Texture (max = 42) we have:
            // 2: textureFlag
            for (inumTexture = 0; inumTexture <= MAX_NUM_TEXTURES - 1; inumTexture++)
            {
                iByteDimension += 2;

                // If textureFlag = 1 we add-> 2 + 2:  Size + Depth
                if (Section9.Textures[inumTexture].textureFlag == 1)
                {
                    iByteDimension += 4;

                    // We calculate here the Dimension of the Texture Data.
                    // All the Textures have 256 pixels Width * 256 pixels Height                
                    // It depends On Depth. If Depth < 1 Is Of 1 Byte, If Depth = 2, 2 bytes
                    if (Section9.Textures[inumTexture].Depth < 2)
                    {
                        iByteDimension += 0x10000;
                    }
                    else
                    {
                        iByteDimension += 0x20000;
                    }
                }
            }

            // Finally we need to add the "END" Tag of Texture Section.
            iByteDimension += 3;

            return iByteDimension;
        }


        // Returns 0 As created ok.
        // Returns 2 As some color not found in palette.
        public static int Create_TextureMatrix(ref byte[,] newtextureMatrix,
                                               Bitmap bmpInput,
                                               List<S9_ZList> lstTilesTexture)
        {

            int iResult;
            int iNumListTile, iNumTile, xPosTile, yPosTile, iTileSize;
            int iPalette, iSourceX, iSourceY;

            Color tmpColor;
            byte byteColor = 0;

            bool bColorNotFound;

            iResult = 0;
            bColorNotFound = false;

            // We will put the new Palettized Texture in a TextureMatrix var.
            // Now we can update the TextureMatrix of bytes (each byte is the indexed color of the palette of
            // the Tile.

            // For each Tile found in the input bitmap we will check if the indexed colors are
            // in the palette of the tile.
            // We will also update the Texture Matrix with this color for not duplicate work.
            iNumListTile = 0;
            iTileSize = lstTilesTexture[iNumListTile].ZTileSize;

            while (iNumListTile < lstTilesTexture.Count &
                   !bColorNotFound)
            {
                xPosTile = 0;
                yPosTile = 0;

                iNumTile = lstTilesTexture[iNumListTile].ZTile;
                iPalette = lstTilesTexture[iNumListTile].ZPalette;

                iSourceX = lstTilesTexture[iNumListTile].ZSourceX;
                iSourceY = lstTilesTexture[iNumListTile].ZSourceY;

                // Let's check if Palette exists.
                if (iPalette >= S4.Section4.numPalettes)
                {
                    return 4;
                }

                GetIndexFirstBlack(iPalette);

                while (yPosTile < iTileSize &
                       !bColorNotFound)
                {
                    while (xPosTile < iTileSize &
                           !bColorNotFound)
                    {
                        // Determine the x,y position of pixel.

                        // Let's get the indexed color of the palette as per Tile.
                        tmpColor = bmpInput.GetPixel(iSourceX + xPosTile, iSourceY + yPosTile);

                        if (tmpColor.A == 0)
                            byteColor = 0;
                        else
                            iResult = GetIndexOfColor(iPalette, tmpColor, ref byteColor);

                        if (iResult > 0)
                        {
                            MessageBox.Show("Color (R:" + tmpColor.R.ToString("000") +
                                                 ", G:" + tmpColor.G.ToString("000") +
                                                 ", B:" + tmpColor.B.ToString("000") +
                                            ") not found in PaletteID: " + iPalette.ToString() +
                                            "\n\r\n\rTile number in Texture: " + iNumTile.ToString() +
                                            ".", "Information", MessageBoxButtons.OK);

                            bColorNotFound = true;
                        }
                        else
                        {
                            // We have found the color and can create the Texture Matrix
                            // with the new Color of the input bitmap.
                            newtextureMatrix[iSourceX + xPosTile, iSourceY + yPosTile] = byteColor;
                            xPosTile++;
                        }
                    }

                    yPosTile++;
                    xPosTile = 0;
                }

                iNumListTile++;
            }

            return iResult;
        }

        // Returns 0 As created ok.
        // Returns 2 As some color not found in palette.
        public static int Create_TextureMatrix2Bytes(ref ushort[,] newtextureMatrix2Bytes,
                                                     Bitmap bmpInput,
                                                     List<S9_ZList> lstTilesTexture)
        {

            int iResult;
            int iNumListTile;
            int xPosTile, yPosTile, iTileSize;
            int iSourceX, iSourceY;

            iResult = 0;

            // We will put the new Palettized Texture in a TextureMatrix var.
            // Now we can update the TextureMatrix of bytes (each byte is the indexed color of the palette of
            // the Tile.

            // For Direct Color textures we don't need to check the colors as with
            // Palettized textures. We can put the color directly to its place while the
            // tile exists.
            iNumListTile = 0;
            iTileSize = lstTilesTexture[0].ZTileSize;

            while (iNumListTile < lstTilesTexture.Count)
            {
                iSourceX = lstTilesTexture[iNumListTile].ZSourceX;
                iSourceY = lstTilesTexture[iNumListTile].ZSourceY;

                for (yPosTile = 0; yPosTile < iTileSize; yPosTile++)
                {
                    for (xPosTile = 0; xPosTile < iTileSize; xPosTile++)
                    {
                        // Put each color directly.
                        newtextureMatrix2Bytes[iSourceX + xPosTile, iSourceY + yPosTile] =
                                 Put16bitColor(bmpInput.GetPixel(iSourceX + xPosTile, iSourceY + yPosTile));
                    }
                }

                iNumListTile++;
            }

            return iResult;
        }

        public static void Clear_TextureImages()
        {
            for (int i = 0; i < MAX_NUM_TEXTURES; i++)
                if (textureImage[i] != null)
                {
                    textureImage[i].Dispose();
                    textureImage[i] = null;
                }
        }

        public static int GetNumRealTextures()
        {
            int iTexCounter = 0;

            for (int i = 0; i < MAX_NUM_TEXTURES; i++)
            {
                if (textureImage[i] != null) iTexCounter++;
            }

            return iTexCounter;
        }


    }
}