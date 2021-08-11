using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace Aeris
{
    public static class SwizzleHash
    {

        public struct ParamsStates
        {
            public SortedList<int, string> TileID;
            public int Texture;
            public int Palette;
            public int Param;
            public int State;
            public bool Unique;
        }

        public struct st_TileSeparation
        {
            public int Texture;
            public int Palette;
            public List<int> lstTileIDSeparate;  // This is for fields like "blue_2", and the detach process.
        }

        public struct TextureLink
        {
            public int FirstTextureID;
            public int FirstPalette;
            public string FirstTextureHash;
            public int Texture;
            public int Palette;
            public int Param;
            public int State;
            public int TileID;
            public string MatchTextureHash;
        }


        // VirtualType:   This says if hash exception is: 0 - No Virtual, 
        // 1 - Virtual without processing, more files,
        // 2 - Virtual with less files
        // 3 - Virtual with change hash texture destination (like an Addition)

        public struct HashExceptions
        {
            public int VirtualType;
            public string FirstTextureHash;
            public int Texture;
            public int Palette;
            public int Param;
            public int State;
            public int TileID;
            public string MatchTextureHash;
            public int MatchTexture;              // The Match Texture/Palette are for Vitual Type 2
            public int MatchPalette;
            public int DestTexture;               // The Destination Tex/Pal/Param/State/TileID for Vitual Type 3
            public int DestPalette;
            public int DestParam;
            public int DestState;
            public int DestTileID;
        }


        // This is a different type of Hash Exception (used mainly in blue_2)
        // 4 - Virtual with tile separation list
        public struct UnswizzledImagesList
        {
            public Bitmap bmpUnswizzledImages;
            public ParamsStates stUnswizzledTextureParamsStates;
        }

        public static List<TextureLink> lstTextureLinks = new List<TextureLink>();
        public static List<HashExceptions> lstHashExceptions = new List<HashExceptions>();
        public static List<st_TileSeparation> lstTileSeparation = new List<st_TileSeparation>();
        public static List<UnswizzledImagesList> lstUnswizzledImagesList;
        public static bool bTextureLinks, bHashExceptions, bTileSeparation;

        public static int iLayerMaxWidthGlobal, iLayerMaxHeightGlobal, iLayerbmpPosXGlobal, iLayerbmpPosYGlobal;
        public static int iScaleFactorL0, iScaleFactorL2;


        // Procedure to prepare/initialize the list of images & params/states for work with
        // unswizzled processed images.
        public static void InitUnswizzledImagesList()
        {
            if (lstUnswizzledImagesList != null)
            {
                foreach (var stUnswizzledImage in lstUnswizzledImagesList)
                    if (stUnswizzledImage.bmpUnswizzledImages != null)
                           stUnswizzledImage.bmpUnswizzledImages.Dispose();

                lstUnswizzledImagesList.Clear();
            }
            else
            {
                lstUnswizzledImagesList = new List<UnswizzledImagesList>();
            }
        }


        // iTypes for Unswizzling Hashes.
        // iType = 0 Add Entry Directly
        // iType = 1 Add Entry by palette if it does not exists the given key.

        // iTypes for Unswizzling Base Images
        // iType = 2 This type is for Export All Base Images. 
        // We will check if there is a Texture > &HE with same Param/State only.
        // iType = 3 This type is for Export All Base Images. 
        // We will check If there is a Texture > &HE with same Palette/Param/State.
        // iType = 4 This type is for Export All Base Images. 
        // We will check if there is a Texture < &HF with same Palette/Param/State.
        // iType = 5 This type is for Export All Base Images (Case param = 0 and status = 0). 
        // We will check if there is a Texture > &HE with same Palette/Param/State containing TileID = 0.
        // iType = 6 This type is for Export All Base Images (Case param = 0 and status = 0). 
        // We will check if there is a Texture > &HE containing TileID = 0 (the input is also TileID = 0).
        // iType = 7 Indicates that the image is Unique for further searching. Only 1 case.
        // iType = 8 This type is for Export All Base Images (Case param = 0 and status = 0). 
        // We will check if there is a Texture > &HE containing TileID = 0 (the input is also TileID = 0)
        // and Palette = UnsPalette.
        // iType = 9 This type is for Export All Base Images. 
        // We will check if there is a Texture < &HE with same Param/State only.
        // iType = 10 This type is for Export All Base Images (layer 1). 
        // We will check if there is a Texture < &HE with same Texture and Param = 0 only.

        public static void lstUnswizzledImagesListAddEntry(Bitmap bmpUnsImage, 
                                                    List<S9.S9_ZList> sortZList, 
                                                    int TileIDAddTileSeparation, int iType)
        {

            bool bAddNew = true;
            bool bFound = false;
            int iIndexKey = 0;
            UnswizzledImagesList stUnswizzledImages;

            ParamsStates stParamsStates;
            stParamsStates = new ParamsStates();

            int iUnsLayer, iUnsTexture, iUnsPalette, iUnsParam, iUnsState, iUnsTileID;

            iUnsLayer = sortZList[0].ZLayer;
            iUnsTexture = sortZList[0].ZTexture;
            iUnsPalette = sortZList[0].ZPalette;
            iUnsParam = sortZList[0].ZParam;
            iUnsState = sortZList[0].ZState;
            iUnsTileID = sortZList[0].ZTileID + TileIDAddTileSeparation;

            // Supposedly by key there is no need to check anything?
            switch (iType)
            {
                case 1:
                    {
                        // Put entry by palette
                        // Let's check if the Key exists.

                        while (iIndexKey < lstUnswizzledImagesList.Count & bAddNew)
                        {
                            if (lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.TileID.ContainsKey(iUnsTileID))
                            {
                                bAddNew = false;
                            }
                            else
                            {
                                iIndexKey = iIndexKey + 1;
                            }
                        }

                        break;
                    }

                case 2:
                    {
                        iIndexKey = 0;
                        while (iIndexKey < lstUnswizzledImagesList.Count & !bFound)
                        {
                            {
                                if (lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Texture > 0xE &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Param == iUnsParam &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.State == iUnsState)
                                {
                                    bFound = true;
                                }
                                else
                                {
                                    iIndexKey = iIndexKey + 1;
                                }
                            }
                        }

                        if (bFound)
                        {
                            var withlstUIL = lstUnswizzledImagesList[iIndexKey];

                            CopyUnswizzledTextures(ref withlstUIL.bmpUnswizzledImages, 
                                                   bmpUnsImage);

                            // There is no need to Add a new entry either.
                            bAddNew = false;
                        }

                        break;
                    }

                case 3:
                    {
                        iIndexKey = 0;
                        while (iIndexKey < lstUnswizzledImagesList.Count & !bFound)
                        {
                            {
                                if (lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Texture > 0xE &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Palette == iUnsPalette &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Param == iUnsParam &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.State == iUnsState)
                                {
                                    bFound = true;
                                }
                                else
                                {
                                    iIndexKey = iIndexKey + 1;
                                }
                            }
                        }

                        if (bFound)
                        {
                            var withlstUIL = lstUnswizzledImagesList[iIndexKey];

                            CopyUnswizzledTextures(ref withlstUIL.bmpUnswizzledImages,
                                                   bmpUnsImage);

                            // There is no need to Add a new entry either.
                            bAddNew = false;
                        }

                        break;
                    }

                case 4:
                    {
                        iIndexKey = 0;
                        while (iIndexKey < lstUnswizzledImagesList.Count & !bFound)
                        {
                            {
                                if (lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Texture < 0xF &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Palette == iUnsPalette &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Param == iUnsParam &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.State == iUnsState)
                                {
                                    bFound = true;
                                }
                                else
                                {
                                    iIndexKey = iIndexKey + 1;
                                }
                            }
                        }

                        if (bFound)
                        {
                            var withlstUIL = lstUnswizzledImagesList[iIndexKey];
                            CopyUnswizzledTextures(ref withlstUIL.bmpUnswizzledImages, bmpUnsImage);

                            // There is no need to Add a new entry either.
                            bAddNew = false;
                        }

                        break;
                    }

                case 5:
                    {
                        iIndexKey = 0;
                        while (iIndexKey < lstUnswizzledImagesList.Count & !bFound)
                        {
                            {
                                if (lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Texture > 0xE &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Palette == iUnsPalette &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.TileID.ContainsKey(0) &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Param == iUnsParam &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.State == iUnsState)
                                {
                                    bFound = true;
                                }
                                else
                                {
                                    iIndexKey = iIndexKey + 1;
                                }
                            }
                        }

                        if (bFound)
                        {
                            var withlstUIL = lstUnswizzledImagesList[iIndexKey];
                            CopyUnswizzledTextures(ref withlstUIL.bmpUnswizzledImages, bmpUnsImage);

                            // There is no need to Add a new entry either.
                            bAddNew = false;
                        }

                        break;
                    }

                case 6:
                    {
                        iIndexKey = 0;
                        while (iIndexKey < lstUnswizzledImagesList.Count & !bFound)
                        {
                            {
                                if (!lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Unique)
                                {
                                    if (lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.TileID.ContainsKey(iUnsTileID) &
                                        lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Param == iUnsParam &
                                        lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.State == iUnsState)
                                    {
                                        bFound = true;
                                    }
                                    else
                                    {
                                        iIndexKey = iIndexKey + 1;
                                    }
                                }
                                else
                                {
                                    iIndexKey = iIndexKey + 1;
                                }
                            }
                        }

                        if (bFound)
                        {
                            var withlstUIL = lstUnswizzledImagesList[iIndexKey];
                            CopyUnswizzledTextures(ref withlstUIL.bmpUnswizzledImages, bmpUnsImage);

                            // There is no need to Add a new entry either.
                            bAddNew = false;
                        }

                        break;
                    }

                case 7:
                    {
                        stParamsStates.Unique = true;
                        break;
                    }

                case 8:
                    {
                        iIndexKey = 0;
                        while (iIndexKey < lstUnswizzledImagesList.Count & !bFound)
                        {
                            {
                                if ((lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Texture > 0xE &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Palette == iUnsPalette &
                                    iUnsTileID > 0x100 &&
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.TileID.Keys[0] == 0 |
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.TileID.Keys[0] == 2 |
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.TileID.Keys[0] > 0x100) &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Param == iUnsParam &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.State == iUnsState)          // this is for blin68_1
                                {
                                    bFound = true;
                                }
                                else
                                {
                                    iIndexKey = iIndexKey + 1;
                                }
                            }
                        }

                        if (bFound)
                        {
                            var withlstUIL = lstUnswizzledImagesList[iIndexKey];
                            CopyUnswizzledTextures(ref withlstUIL.bmpUnswizzledImages, bmpUnsImage);

                            // There is no need to Add a new entry either.
                            bAddNew = false;
                        }

                        break;
                    }

                case 9:
                    {
                        iIndexKey = 0;
                        while (iIndexKey < lstUnswizzledImagesList.Count & !bFound)
                        {
                            {
                                if (lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Texture < 0xE &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Param == iUnsParam &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.State == iUnsState)
                                {
                                    bFound = true;
                                }
                                else
                                {
                                    iIndexKey = iIndexKey + 1;
                                }
                            }
                        }

                        if (bFound)
                        {
                            var withlstUIL = lstUnswizzledImagesList[iIndexKey];
                            CopyUnswizzledTextures(ref withlstUIL.bmpUnswizzledImages, bmpUnsImage);

                            // There is no need to Add a new entry either.
                            bAddNew = false;
                        }

                        break;
                    }

                case 10:
                    {
                        iIndexKey = 0;
                        while (iIndexKey < lstUnswizzledImagesList.Count & !bFound)
                        {
                            {
                                if (lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Texture < 0xF &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Palette == iUnsPalette &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.TileID.Keys[0] > 0 &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.Param == iUnsParam &
                                    lstUnswizzledImagesList[iIndexKey].stUnswizzledTextureParamsStates.State == iUnsState)
                                {
                                    bFound = true;
                                }
                                else
                                {
                                    iIndexKey = iIndexKey + 1;
                                }
                            }
                        }

                        if (bFound)
                        {
                            var withlstUIL = lstUnswizzledImagesList[iIndexKey];
                            CopyUnswizzledTextures(ref withlstUIL.bmpUnswizzledImages, bmpUnsImage);

                            // There is no need to Add a new entry either.
                            bAddNew = false;
                        }

                        break;
                    }
            }

            if (bAddNew)
            {
                stParamsStates.Texture = iUnsTexture;
                stParamsStates.Palette = iUnsPalette;
                stParamsStates.Param = iUnsParam;
                stParamsStates.State = iUnsState;

                stParamsStates.TileID = new SortedList<int, string>();
                stParamsStates.TileID.Add(iUnsTileID, iUnsTexture.ToString("00") + "_" + 
                                          iUnsPalette.ToString("00") + "_" + 
                                          iUnsParam.ToString("00") + "_" + 
                                          iUnsState.ToString("00"));

                stUnswizzledImages.bmpUnswizzledImages = new Bitmap(bmpUnsImage);

                stUnswizzledImages.stUnswizzledTextureParamsStates = stParamsStates;
                lstUnswizzledImagesList.Add(stUnswizzledImages);
            }
        }


        // Public Function GetIndexUnswizzledTexture(iUnsParam, iUnsState) As Integer
        // Dim iIdxLstParamsStates As Integer = 0
        // Dim bFound As Boolean = False

        // While iIdxLstParamsStates <lstUnswizzledImagesList.Count() And
        // Not bFound

        // If iUnsParam = lstUnswizzledImagesList(iIdxLstParamsStates).stUnswizzledTextureParamsStates.Param And
        // iUnsState = lstUnswizzledImagesList(iIdxLstParamsStates).stUnswizzledTextureParamsStates.State Then

        // bFound = True
        // Else
        // iIdxLstParamsStates = iIdxLstParamsStates + 1
        // End If

        // End While

        // If Not bFound Then iIdxLstParamsStates = -1

        // Return iIdxLstParamsStates

        // End Function


        // This procedure helps in getting the First Texture and Palette of a given Hash.
        public static void GetFirstTexturePalette(int iUnsLayer, 
                                                  int iUnsTileID, 
                                                  int iUnsTexture, 
                                                  int iUnsPalette, 
                                                  int iUnsParam, 
                                                  int iUnsState, 
                                                  ref int iFirstTileID, 
                                                  ref int iFirstTexture, 
                                                  ref int iFirstPalette)
        {
            int iPaletteCounter, iTextureCounter, iTileIDVar;
            bool bFirstTextureFound, bFirstPaletteFound;
            S9.S9_ZList queryFirstValues;

            iFirstTileID = 0;

            // Let's search the First Texture/Palette values independently by Layer.
            switch (iUnsLayer)
            {
                case 0:
                    {
                        var queryTexPal = (from sZItem in S9.Section9Z
                                           where sZItem.ZLayer == iUnsLayer
                                           orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                           select sZItem).First();
                        iFirstTexture = queryTexPal.ZTexture;
                        iFirstPalette = queryTexPal.ZPalette;
                        iFirstTileID = queryTexPal.ZTileID;
                        break;
                    }

                case 1:
                    {
                        // Ok, Layer 1, and for hashed dumps this means Texture > &HE.
                        // The most easy case is to check if Param > 0. 
                        // Then with Param/State we can know the First Values easily.
                        // The most difficult case is to check values for Param = 0 (State = 0).
                        // There are fields that I prefer to check standalone.

                        switch (FileTools.strGlobalFieldName)
                        {
                            case "blin68_2":
                                {
                                    var queryFirstSpecialValues = (from sZItem in S9.Section9Z
                                                                   where sZItem.ZPalette == iUnsPalette
                                                                   orderby sZItem.ZTileID, sZItem.ZTexture
                                                                   select sZItem).First();
                                    iFirstTexture = queryFirstSpecialValues.ZTexture;
                                    iFirstPalette = queryFirstSpecialValues.ZPalette;
                                    iFirstTileID = queryFirstSpecialValues.ZTileID;
                                    return;
                                }

                            case "gidun_1":
                            case "sandun_1":
                                {
                                    var queryFirstSpecialValues = (from sZItem in S9.Section9Z
                                                                   where sZItem.ZTexture == 0xF
                                                                   orderby sZItem.ZPalette, sZItem.ZTileID
                                                                   select sZItem).First();
                                    iFirstTexture = queryFirstSpecialValues.ZTexture;
                                    iFirstPalette = queryFirstSpecialValues.ZPalette;
                                    iFirstTileID = queryFirstSpecialValues.ZTileID;
                                    return;
                                }

                            case "trnad_1":
                                {
                                    var queryFirstSpecialValues = (from sZItem in S9.Section9Z
                                                                   where sZItem.ZTexture > 0xE & sZItem.ZTileID == iUnsTileID
                                                                   orderby sZItem.ZTexture, sZItem.ZPalette
                                                                   select sZItem).First();
                                    iFirstTexture = queryFirstSpecialValues.ZTexture;
                                    iFirstPalette = queryFirstSpecialValues.ZPalette;
                                    iFirstTileID = queryFirstSpecialValues.ZTileID;
                                    return;
                                }

                            case "blue_2":
                                {
                                    var queryFirstSpecialValues = (from sZItem in S9.Section9Z
                                                                   where sZItem.ZTexture > 0xE & sZItem.ZPalette == iUnsPalette & sZItem.ZTileID == iUnsTileID & sZItem.ZParam == iUnsParam & sZItem.ZState == iUnsState
                                                                   orderby sZItem.ZTexture, sZItem.ZPalette
                                                                   select sZItem).First();
                                    iFirstTexture = queryFirstSpecialValues.ZTexture;
                                    iFirstPalette = queryFirstSpecialValues.ZPalette;
                                    iFirstTileID = queryFirstSpecialValues.ZTileID;
                                    return;
                                }

                            case "smkin_4":
                                {
                                    var queryFirstSpecialValues = (from sZItem in S9.Section9Z
                                                                   where sZItem.ZTexture > 0xE & sZItem.ZParam == iUnsParam & sZItem.ZState == iUnsState
                                                                   orderby sZItem.ZTileID, sZItem.ZTexture, sZItem.ZPalette
                                                                   select sZItem).First();
                                    iFirstTexture = queryFirstSpecialValues.ZTexture;
                                    iFirstPalette = queryFirstSpecialValues.ZPalette;
                                    iFirstTileID = queryFirstSpecialValues.ZTileID;
                                    return;
                                }

                            case "nivl_b1":
                            case "nivl_b12":
                                {
                                    var queryFirstSpecialValues = (from sZItem in S9.Section9Z
                                                                   where sZItem.ZTexture > 0xE & sZItem.ZPalette == iUnsPalette & sZItem.ZParam == iUnsParam & sZItem.ZState == iUnsState
                                                                   orderby sZItem.ZTileID, sZItem.ZTexture, sZItem.ZPalette
                                                                   select sZItem).First();
                                    iFirstTexture = queryFirstSpecialValues.ZTexture;
                                    iFirstPalette = queryFirstSpecialValues.ZPalette;
                                    iFirstTileID = queryFirstSpecialValues.ZTileID;
                                    return;
                                }

                            case "snow":
                                {
                                    if (iUnsPalette == 12 | iUnsPalette == 13)
                                    {
                                        var queryFirstSpecialValues = (from sZItem in S9.Section9Z
                                                                       where sZItem.ZTexture > 0xE & sZItem.ZTileID == iUnsTileID
                                                                       orderby sZItem.ZTileID, sZItem.ZTexture, sZItem.ZPalette
                                                                       select sZItem).First();
                                        iFirstTexture = queryFirstSpecialValues.ZTexture;
                                        iFirstPalette = 12;
                                        iFirstTileID = queryFirstSpecialValues.ZTileID;
                                    }
                                    else
                                    {
                                        var queryFirstSpecialValues = (from sZItem in S9.Section9Z
                                                                       where sZItem.ZTexture > 0xE & sZItem.ZPalette == iUnsPalette & sZItem.ZTileID == iUnsTileID
                                                                       orderby sZItem.ZTileID, sZItem.ZTexture, sZItem.ZPalette
                                                                       select sZItem).First();
                                        iFirstTexture = queryFirstSpecialValues.ZTexture;
                                        iFirstPalette = queryFirstSpecialValues.ZPalette;
                                        iFirstTileID = queryFirstSpecialValues.ZTileID;
                                    }

                                    return;
                                }

                            // This field has hashes for layer 0 (not usual)
                            case "las2_3":
                                {
                                    if (iUnsTexture < 0xF)
                                    {
                                        var queryFirstSpecialValues = (from sZItem in S9.Section9Z
                                                                       where sZItem.ZLayer == iUnsLayer & sZItem.ZTexture < 0xF
                                                                       orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                                       select sZItem).First();
                                        iFirstTexture = queryFirstSpecialValues.ZTexture;
                                        iFirstPalette = queryFirstSpecialValues.ZPalette;
                                        iFirstTileID = queryFirstSpecialValues.ZTileID;
                                    }
                                    else
                                    {
                                        var queryFirstSpecialValues = (from sZItem in S9.Section9Z
                                                                       where sZItem.ZTexture > 0xE & sZItem.ZParam == iUnsParam & sZItem.ZState == iUnsState & sZItem.ZTileID == iUnsTileID
                                                                       orderby sZItem.ZTileID, sZItem.ZTexture, sZItem.ZPalette
                                                                       select sZItem).First();
                                        iFirstTexture = queryFirstSpecialValues.ZTexture;
                                        iFirstPalette = queryFirstSpecialValues.ZPalette;
                                        iFirstTileID = queryFirstSpecialValues.ZTileID;
                                    }

                                    return;
                                }
                        }

                        if (iUnsParam > 0)
                        {
                            // For Param > 1 this can be done by searching the First Texture/Palette with a query.
                            // We need, though, look for this images that has the same TileID = 0 Or only 1 TileID,
                            // in which case we will do the search by TileID. If not, will do by Palette.
                            var iNumTileIDs = (from sZItem in S9.Section9Z
                                               where sZItem.ZLayer == iUnsLayer &
                                                     (sZItem.ZTexture > 0xE & sZItem.ZTexture < 0x1A) &
                                                     sZItem.ZParam == iUnsParam &
                                                     sZItem.ZState == iUnsState
                                               orderby sZItem.ZTileID
                                               group sZItem by new
                                               {
                                                   sZItem.ZTileID
                                               } into ZTileIDGroup
                                               select new { ZTileIDGroup.Key, ZTileIDGroup }).Distinct().ToList();

                            if (iNumTileIDs.Count() == 1 | iNumTileIDs[0].Key.ZTileID == 0)
                            {
                                queryFirstValues = (from sZItem in S9.Section9Z
                                                    where sZItem.ZLayer == iUnsLayer &
                                                          (sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture) &
                                                          sZItem.ZTileID == iNumTileIDs[0].Key.ZTileID &
                                                          sZItem.ZParam == iUnsParam &
                                                          sZItem.ZState == iUnsState
                                                    orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                    select sZItem).First();
                            }
                            else if (iUnsTexture > 15)
                            {
                                queryFirstValues = (from sZItem in S9.Section9Z
                                                    where sZItem.ZLayer == iUnsLayer &
                                                          (sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture) &
                                                          sZItem.ZParam == iUnsParam &
                                                          sZItem.ZState == iUnsState
                                                    orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                    select sZItem).First();
                            }
                            else
                            {
                                queryFirstValues = (from sZItem in S9.Section9Z
                                                    where sZItem.ZLayer == iUnsLayer &
                                                          (sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture) &
                                                          sZItem.ZPalette == iUnsPalette &
                                                          sZItem.ZParam == iUnsParam &
                                                          sZItem.ZState == iUnsState
                                                    orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                    select sZItem).First();
                            }

                            iFirstTexture = queryFirstValues.ZTexture;
                            iFirstPalette = queryFirstValues.ZPalette;
                            iFirstTileID = queryFirstValues.ZTileID;
                        }
                        else
                        {

                            // For Param = 0 we can have this combination of files:
                            // 1.-  1 Texture with 1 Palette, unique
                            // 2.-  1 Texture with 2+ Palettes
                            // 3.-  2+ Textuers with 1 Palette, unique
                            // 4.-  2+ Textures with 2+ Palettes (different TileID)

                            // Let's get the Number of Textuers and Palettes for the input Texture/Palette combination.
                            // We will do this with the TileID. So get first the TileID of this Tex/Pal combination.
                            var lstTileIDs = (from sZItemTIDs in S9.Section9Z
                                              where sZItemTIDs.ZLayer == iUnsLayer &
                                                    sZItemTIDs.ZTexture == iUnsTexture &
                                                    sZItemTIDs.ZPalette == iUnsPalette
                                              orderby sZItemTIDs.ZTileID
                                              group sZItemTIDs by new
                                              {
                                                  sZItemTIDs.ZTileID
                                              } into ZTileIDGroupTIDs
                                              select new
                                              {
                                                  ZTileIDGroupTIDs.Key,
                                                  ZTileIDGroupTIDs
                                              });

                            // Let's select directly First TileID depending on the number of TileIDs of Tex/Pal combination.
                            if (lstTileIDs.Count() > 1)
                            {
                                if (lstTileIDs.Where(x => x.Key.ZTileID == iUnsTileID).Count() > 0)
                                {
                                    iFirstTileID = iUnsTileID;
                                }
                                else
                                {
                                    iFirstTileID = lstTileIDs.First().Key.ZTileID;
                                }
                            }
                            else
                            {
                                iFirstTileID = lstTileIDs.First().Key.ZTileID;
                            }

                            // If first TileID is TileID = 0 and Count = 1 and Param = 0 and State = 0 we will get second TileID
                            // This is an exception for those Fields that has only 1 tile (and is transparent) like "whitein" Field.
                            switch (FileTools.strGlobalFieldName)
                            {
                                case "whitein":
                                    {
                                        if (lstTileIDs.Count() > 1)
                                        {
                                            if (lstTileIDs.First().Key.ZTileID == 0)
                                            {
                                                if (lstTileIDs.First().ZTileIDGroupTIDs.Count() == 1)
                                                {
                                                    if (lstTileIDs.First().ZTileIDGroupTIDs.First().ZParam == 0)
                                                    {
                                                        iFirstTileID = lstTileIDs.ElementAt(1).Key.ZTileID;
                                                    }
                                                }
                                            }
                                        }

                                        break;
                                    }
                            }

                            iTileIDVar = iFirstTileID;


                            // If lstTileIDs Count = 1 we can decide by Tex/Pal the First values.
                            if (lstTileIDs.Count() == 1)
                            {
                                // Unique Tile ID
                                // Let's get the list of palettes/textures used.
                                // This is the easy First Tex/Pal. We will get the first Tex/Pal of this TileID.
                                var lstTextures = (from sZItem in S9.Section9Z
                                                   where sZItem.ZLayer == iUnsLayer &
                                                         sZItem.ZTileID == lstTileIDs.First().Key.ZTileID &
                                                         sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture
                                                   orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                   group sZItem by new
                                                   {
                                                       sZItem.ZTexture
                                                   } into ZTextureGroup
                                                   select new
                                                   {
                                                       ZTextureGroup.Key,
                                                       ZTextureGroup
                                                   }).ToList();

                                var lstPalettes = (from sZItem in S9.Section9Z
                                                   where sZItem.ZLayer == iUnsLayer &
                                                         sZItem.ZTileID == lstTileIDs.First().Key.ZTileID &
                                                         sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture
                                                   orderby sZItem.ZPalette, sZItem.ZTileID
                                                   group sZItem by new
                                                   {
                                                       sZItem.ZPalette
                                                   } into ZPaletteGroup
                                                   select new
                                                   {
                                                       ZPaletteGroup.Key,
                                                       ZPaletteGroup
                                                   }).ToList();

                                // Some fields (like lastmap) are bugged. Layer 1, TileID 3856 for this field should be Layer 0.
                                // In this case it returns Textures and Palettes = 0
                                if (lstTextures.Count() == 0 & lstPalettes.Count() == 0)
                                {
                                    iFirstTexture = iUnsTexture;
                                    iFirstPalette = iUnsPalette;
                                    return;
                                }

                                // Let's Check if first TileID is 0. There are cases where can be more than one Tex/Pal combination.
                                if (lstTileIDs.First().Key.ZTileID == 0)
                                {
                                    // Normally only Texture 15. So we will check only palette.
                                    // We can check if we have only 1 palette or multiple palettes also to make this fast.
                                    if (lstPalettes.Count == 1)
                                    {
                                        iFirstTexture = lstTextures[0].Key.ZTexture;
                                        iFirstPalette = lstPalettes[0].Key.ZPalette;
                                    }
                                    else
                                    {
                                        // For multiple palettes we need to see if it is for same image or a new image.
                                        // We will do this with the Tile number.
                                        int iDiffPalettes;

                                        iPaletteCounter = 0;
                                        bFirstPaletteFound = false;
                                        iDiffPalettes = lstPalettes[iPaletteCounter].Key.ZPalette;

                                        while (iPaletteCounter < lstPalettes.Count & 
                                               !bFirstPaletteFound)
                                        {
                                            if (lstPalettes[iPaletteCounter].Key.ZPalette == iUnsPalette &
                                                lstPalettes[iPaletteCounter].ZPaletteGroup.First().ZParam == 
                                                            lstTextures[0].ZTextureGroup.First().ZParam &&
                                                lstPalettes[iPaletteCounter].ZPaletteGroup.First().ZState ==
                                                            lstTextures[0].ZTextureGroup.First().ZState)
                                            {
                                                bFirstPaletteFound = true;
                                            }
                                            else
                                            {
                                                iPaletteCounter = iPaletteCounter + 1;
                                            }
                                        }

                                        if (bFirstPaletteFound)
                                        {
                                            if (iPaletteCounter > 0)
                                            {
                                                iDiffPalettes = 
                                                    Math.Abs(lstPalettes[iPaletteCounter - 1].Key.ZPalette - lstPalettes[iPaletteCounter].Key.ZPalette);
                                            }
                                            else
                                            {
                                                iDiffPalettes = 0;
                                            }

                                            if (lstPalettes[iPaletteCounter].ZPaletteGroup.First().ZTile < 0xFF &
                                                iDiffPalettes > 1)
                                            {
                                                iFirstTexture = lstTextures[0].Key.ZTexture;
                                                iFirstPalette = lstPalettes[iPaletteCounter].Key.ZPalette;
                                            }
                                            else
                                            {
                                                iFirstTexture = lstTextures[0].Key.ZTexture;
                                                iFirstPalette = lstPalettes[0].Key.ZPalette;
                                            }
                                        }
                                        else
                                        {
                                            iFirstTexture = lstTextures[0].Key.ZTexture;
                                            iFirstPalette = lstPalettes[0].Key.ZPalette;
                                        }
                                    }
                                }
                                else
                                {
                                    if (lstTileIDs.First().ZTileIDGroupTIDs.First().ZTile < 0xFF)
                                    {
                                        if (lstTileIDs.First().ZTileIDGroupTIDs.First().ZTile > 0x3F)
                                        {
                                            queryFirstValues = (from sZItem in S9.Section9Z
                                                                where sZItem.ZLayer == iUnsLayer &
                                                                      (sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture) &
                                                                      sZItem.ZTileID == lstTileIDs.First().Key.ZTileID &
                                                                      sZItem.ZPalette == iUnsPalette &
                                                                      sZItem.ZParam == iUnsParam
                                                                orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                                select sZItem).First();
                                        }
                                        else if (lstTileIDs.First().Key.ZTileID >= 0x800)          // This is for eals_1
                                        {
                                            queryFirstValues = (from sZItem in S9.Section9Z
                                                                where sZItem.ZLayer == iUnsLayer &
                                                                      (sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture) &
                                                                      sZItem.ZTileID == lstTileIDs.First().Key.ZTileID &
                                                                      sZItem.ZPalette == iUnsPalette &
                                                                      sZItem.ZParam == iUnsParam
                                                                orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                                select sZItem).First();
                                        }
                                        else
                                        {
                                            queryFirstValues = (from sZItem in S9.Section9Z
                                                                where sZItem.ZLayer == iUnsLayer &
                                                                      (sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture) &
                                                                      sZItem.ZTileID == lstTileIDs.First().Key.ZTileID &
                                                                      sZItem.ZParam == iUnsParam
                                                                orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                                select sZItem).First();
                                        }
                                    }
                                    else if (lstTileIDs.First().ZTileIDGroupTIDs.First().ZTile < 0x300)            // This is for mtnvl3
                                    {
                                        queryFirstValues = (from sZItem in S9.Section9Z
                                                            where sZItem.ZLayer == iUnsLayer &
                                                                  (sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture) &
                                                                  sZItem.ZPalette == iUnsPalette &
                                                                  sZItem.ZParam == iUnsParam
                                                            orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                            select sZItem).First();
                                    }
                                    else
                                    {
                                        // Let's check how many Params we can have altough the first may be 0. This is for crater_2 basically.
                                        var queryNumParams = (from sZItem in S9.Section9Z
                                                              where sZItem.ZLayer == iUnsLayer &
                                                                    (sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture) &
                                                                    sZItem.ZPalette <= iUnsPalette &
                                                                    sZItem.ZTileID == iUnsTileID
                                                              select sZItem.ZParam).Distinct().Count();
                                        if (queryNumParams == 1)
                                        {
                                            queryFirstValues = (from sZItem in S9.Section9Z
                                                                where sZItem.ZLayer == iUnsLayer &
                                                                      (sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture) &
                                                                      sZItem.ZPalette <= iUnsPalette &
                                                                      sZItem.ZTileID == lstTileIDs.First().Key.ZTileID &
                                                                      sZItem.ZParam == iUnsParam
                                                                orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                                select sZItem).First();
                                        }
                                        else
                                        {
                                            queryFirstValues = (from sZItem in S9.Section9Z
                                                                where sZItem.ZLayer == iUnsLayer &
                                                                      (sZItem.ZTexture > 0xE && sZItem.ZTexture <= iUnsTexture) &
                                                                      sZItem.ZPalette <= iUnsPalette &
                                                                      sZItem.ZTileID == lstTileIDs.First().Key.ZTileID
                                                                orderby sZItem.ZTexture, sZItem.ZPalette, sZItem.ZTileID
                                                                select sZItem).First();
                                        }
                                    }

                                    iFirstTexture = queryFirstValues.ZTexture;
                                    iFirstPalette = queryFirstValues.ZPalette;
                                    iFirstTileID = queryFirstValues.ZTileID;
                                }
                            }
                            else
                            {
                                // Multiple Tile IDs
                                // The TileID 0 can have different Tex/Pal combinations, some are TileID = 0.
                                // Let's get also the Number of Tex/Pal for a given TileID.
                                var lstTextures = (from sZItem in S9.Section9Z
                                                   join sZItemTileID in lstTileIDs
                                                        on sZItem.ZTileID equals sZItemTileID.Key.ZTileID
                                                   where sZItem.ZLayer == iUnsLayer &
                                                         sZItemTileID.Key.ZTileID >= iTileIDVar &
                                                         sZItem.ZTexture > 0xE
                                                   orderby sZItem.ZTexture, sZItem.ZTileID
                                                   group sZItem by new
                                                   {
                                                       sZItem.ZTexture
                                                   } into ZTextureGroup
                                                   select new
                                                   {
                                                       ZTextureGroup.Key,
                                                       ZTextureGroup
                                                   }).ToList();


                                var lstPalettes = (from sZItem in S9.Section9Z
                                                   join sZItemTileID in lstTileIDs 
                                                        on sZItem.ZTileID equals sZItemTileID.Key.ZTileID
                                                   where sZItem.ZLayer == iUnsLayer &
                                                         sZItem.ZTexture > 0xE
                                                   orderby sZItem.ZPalette
                                                   group sZItem by new
                                                   {
                                                       sZItem.ZPalette
                                                   } into ZPaletteGroup
                                                   select new
                                                   {
                                                       ZPaletteGroup.Key,
                                                       ZPaletteGroup
                                                   }).ToList();

                                // Let's check for each Tile ID those who has the same Texture/Palette
                                // If Multiple Textures and Palettes, check lower Texture for input palette.

                                // If there is only 1 combination of Texture/Palette we can treat it directly.
                                if (lstPalettes.Count() == 1)
                                {
                                    if (lstTextures.Count() == 1)
                                    {
                                        iFirstTexture = lstTextures.First().Key.ZTexture;
                                        iFirstPalette = lstPalettes.First().Key.ZPalette;
                                    }
                                    else
                                    {
                                        // More than one texture per Palette. Check by TileID.
                                        bFirstTextureFound = false;
                                        iTextureCounter = 0;

                                        while (iTextureCounter < lstTextures.Count() &
                                               !bFirstTextureFound)
                                        {
                                            if (lstTextures[iTextureCounter].ZTextureGroup.First().ZTileID == iUnsTileID)
                                            {
                                                bFirstTextureFound = true;
                                            }
                                            else
                                            {
                                                iTextureCounter = iTextureCounter + 1;
                                            }
                                        }

                                        if (bFirstTextureFound)
                                        {
                                            iFirstTexture = lstTextures[iTextureCounter].Key.ZTexture;
                                            iFirstPalette = lstPalettes[0].Key.ZPalette;
                                        }
                                        else
                                        {
                                            iFirstTexture = lstTextures[0].Key.ZTexture;
                                            iFirstPalette = lstPalettes[0].Key.ZPalette;
                                        }
                                    }
                                }
                                else
                                {
                                    // This is the most difficult case. Multiple textures, with multiple ids, with multiple palettes.
                                    // Due the difficulty of this case, we will detect it manually. This means, we will use the Field Name to select some values.
                                    switch (FileTools.strGlobalFieldName)
                                    {
                                        case "ancnt2":
                                            {
                                                iFirstTexture = lstTextures[0].Key.ZTexture;
                                                iFirstPalette = lstPalettes[0].Key.ZPalette;

                                                if (lstPalettes.Count() > 1)
                                                {

                                                    // Search first texture for this palette
                                                    foreach (var itmPalette in lstPalettes)
                                                    {
                                                        if (itmPalette.ZPaletteGroup.First().ZTileID == iUnsTileID &
                                                            itmPalette.ZPaletteGroup.First().ZPalette < iUnsPalette)
                                                        {
                                                            iFirstTexture = lstTextures[0].Key.ZTexture;
                                                            iFirstPalette = itmPalette.Key.ZPalette;
                                                        }
                                                    }
                                                }

                                                break;
                                            }

                                        default:
                                            {
                                                iFirstTexture = lstTextures[0].Key.ZTexture;
                                                iFirstPalette = lstPalettes[0].Key.ZPalette;

                                                if (lstPalettes.Count() > 1)
                                                {

                                                    // Search first texture for this palette
                                                    foreach (var itmPalette in lstPalettes)
                                                    {
                                                        if (itmPalette.Key.ZPalette == iUnsPalette &
                                                            itmPalette.ZPaletteGroup.First().ZParam == iUnsParam &
                                                            itmPalette.ZPaletteGroup.First().ZState == iUnsState)
                                                        {
                                                            iFirstTexture = lstTextures[0].Key.ZTexture;
                                                            iFirstPalette = itmPalette.Key.ZPalette;
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                    }
                                }
                            }
                        }

                        break;
                    }

                case 2:
                case 3:
                    {
                        switch (FileTools.strGlobalFieldName)
                        {
                            case "trnad_3":
                                {
                                    var queryForTexture = (from sZItem in S9.Section9Z
                                                           where sZItem.ZLayer == iUnsLayer & sZItem.ZParam == iUnsParam
                                                           orderby sZItem.ZParam, sZItem.ZTexture, sZItem.ZPalette
                                                           select sZItem).First();
                                    iFirstTexture = queryForTexture.ZTexture;
                                    iFirstPalette = queryForTexture.ZPalette;
                                    iFirstTileID = queryForTexture.ZTileID;
                                    break;
                                }

                            default:
                                {
                                    var queryForTexture = (from sZItem in S9.Section9Z
                                                           where sZItem.ZLayer == iUnsLayer & sZItem.ZPalette <= iUnsPalette
                                                           orderby sZItem.ZTexture, sZItem.ZTileID
                                                           select sZItem).First();

                                    iFirstTexture = queryForTexture.ZTexture;
                                    iFirstTileID = queryForTexture.ZTileID;

                                    iFirstPalette = (from sZItem in S9.Section9Z
                                                     where sZItem.ZLayer == iUnsLayer & sZItem.ZTexture <= iUnsTexture
                                                     orderby sZItem.ZPalette
                                                     select sZItem.ZPalette).First();
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

        public static void GetLastTexturePalette(int iUnsTexture, int iUnsPalette, int iUnsTileID, int iUnsParam, int iUnsState, int iUnsLayer, int iFirstTexture, int iFirstPalette, ref int iLastTexture, ref int iLastPalette)
        {
            List<ImageTools.ImageRGBColors> CheckLastTexture, CheckLastPalette, CheckLastItem;

            if (iUnsParam == 0)
            {
                CheckLastTexture = (from itmImage in ImageTools.lstImagesRGBColors
                                    where (itmImage.Layer == iUnsLayer & itmImage.Texture > 0xE && itmImage.Texture < iUnsTexture) & itmImage.TileID.Contains(iUnsTileID) & itmImage.Param == iUnsParam
                                    orderby itmImage.Texture
                                    select itmImage).ToList();
            }
            else
            {
                CheckLastTexture = (from itmImage in ImageTools.lstImagesRGBColors
                                    where itmImage.Texture >= iFirstTexture & itmImage.Texture < iUnsTexture & itmImage.Palette == iUnsPalette & itmImage.Param == iUnsParam & itmImage.State == iUnsState
                                    orderby itmImage.Texture
                                    select itmImage).ToList();
            }

            if (CheckLastTexture.Count == 0)
            {
                if (iUnsParam == 0)
                {
                    CheckLastPalette = (from itmImage in ImageTools.lstImagesRGBColors
                                        where (itmImage.Layer == iUnsLayer & itmImage.Texture > 0xE & itmImage.Palette >= iFirstPalette && itmImage.Palette < iUnsPalette) & itmImage.TileID.Contains(iUnsTileID) & itmImage.Param == iUnsParam
                                        orderby itmImage.Palette
                                        select itmImage).ToList();
                }
                else
                {
                    CheckLastPalette = (from itmImage in ImageTools.lstImagesRGBColors
                                        where itmImage.Texture == iUnsTexture & (itmImage.Palette >= iFirstPalette & itmImage.Palette < iUnsPalette) & itmImage.Param == iUnsParam & itmImage.State == iUnsState
                                        orderby itmImage.Palette
                                        select itmImage).ToList();
                }

                if (CheckLastPalette.Count == 0)
                {
                    if (iUnsParam == 0)
                    {
                        CheckLastItem = (from itmImage in ImageTools.lstImagesRGBColors
                                         where itmImage.Layer == iUnsLayer & itmImage.Texture > 0xE & itmImage.TileID.Contains(iUnsTileID) & itmImage.Param == iUnsParam
                                         orderby itmImage.Palette
                                         select itmImage).ToList();
                    }
                    else
                    {
                        CheckLastItem = (from itmImage in ImageTools.lstImagesRGBColors
                                         where itmImage.Param == iUnsParam & itmImage.State == iUnsState
                                         orderby itmImage.Palette
                                         select itmImage).ToList();
                    }

                    if (CheckLastItem.Count == 0)
                    {
                        iLastTexture = iUnsTexture;
                        iLastPalette = iUnsPalette;
                    }
                    else
                    {
                        iLastTexture = CheckLastItem.First().Texture;
                        iLastPalette = CheckLastItem.First().Palette;
                    }
                }
                else
                {
                    iLastTexture = CheckLastPalette.First().Texture;
                    iLastPalette = CheckLastPalette.First().Palette;
                }
            }
            else
            {
                iLastTexture = CheckLastTexture.First().Texture;
                iLastPalette = CheckLastTexture.First().Palette;
            }
        }

        public static bool CheckImageByCRC32(string strCRC32, 
                                      int iUnsTileID, 
                                      int iUnsLayer, int iUnsTexture, int iUnsPalette, 
                                      int iFirstTexture, int iFirstPalette, int iFirstTileID, 
                                      int iLastTexture, int iLastPalette, 
                                      int iUnsParam, int iUnsState, 
                                      ref string strUnsFile)
        {

            List<HashCRC.CRC32List> querylstImagesCRC32;

            bool bResult = false;

            if (iLastTexture < iUnsTexture)
            {
                if (iUnsLayer > 1)
                {
                    querylstImagesCRC32 = (from itmImage in HashCRC.lstImagesCRC32
                                           where itmImage.CRC32 == strCRC32 &
                                                 itmImage.FirstTexture == iFirstTexture &
                                                 itmImage.FirstPalette == iFirstPalette &
                                                 itmImage.FirstTileID == iFirstTileID &
                                                 itmImage.Param == iUnsParam &
                                                 itmImage.State == iUnsState &
                                                 itmImage.Used == false
                                           select itmImage).ToList();
                }
                else
                {
                    querylstImagesCRC32 = (from itmImage in HashCRC.lstImagesCRC32
                                           where itmImage.CRC32 == strCRC32 &
                                                 itmImage.FirstTexture == iFirstTexture &
                                                 itmImage.FirstPalette == iFirstPalette &
                                                 itmImage.FirstTileID == iFirstTileID &
                                                 itmImage.Param == iUnsParam &
                                                 itmImage.State == iUnsState &
                                                 itmImage.Used == false
                                           select itmImage).ToList();
                }
            }
            else
            {
                querylstImagesCRC32 = (from itmImage in HashCRC.lstImagesCRC32
                                       where itmImage.CRC32 == strCRC32 &
                                             itmImage.FirstTexture == iFirstTexture &
                                             itmImage.FirstPalette == iFirstPalette &
                                             itmImage.FirstTileID == iFirstTileID &
                                             Int32.Parse(itmImage.TileID) == iUnsTileID &
                                             itmImage.Param == iUnsParam &
                                             itmImage.State == iUnsState &
                                             itmImage.Used == false
                                       select itmImage).ToList();
            }

            if (querylstImagesCRC32.Count > 0)
            {
                strUnsFile = querylstImagesCRC32[0].strFile;
                HashCRC.UpdateCRC32Used(strUnsFile);
                bResult = true;
            }

            return bResult;
        }

        public static bool CheckImageByRGBColor(Bitmap bmpUnsInputTexture, 
                                                Bitmap itmUnswizzledTexture, 
                                                int iUnsLayer, 
                                                int iNumPalettesByParam, 
                                                int iUnsTexture, int iUnsPalette, int iUnsTileID, 
                                                int iFirstTexture, int iFirstPalette, int iFirstTileID, 
                                                int iLastTexture, int iLastPalette, 
                                                int iUnsParam, int iUnsState, 
                                                ref string strUnsFile, 
                                                ref bool bUseSameRGBColorForList)
        {
            var querylstImagesRGBColors = new List<ImageTools.ImageRGBColors>();
            bool bResult = false;
            int iBestHashDetected;

            // If there is not a file, we must detect the hash by comparing colors,
            // from the Unswizzled and the Swizzled textures.

            querylstImagesRGBColors.Clear();

            // Here we need to adapt the search by the same method that creating the Unswizzled
            // Textures parts. Check if Param = 0 or > 0, and If Param = 0, check by number of
            // Palettes.
            switch (iUnsLayer)
            {
                case 0:
                    {
                        querylstImagesRGBColors = (from itmImage in ImageTools.lstImagesRGBColors
                                                   where itmImage.Layer == iUnsLayer & itmImage.Used == false
                                                   select itmImage).ToList();
                        break;
                    }

                case 1:
                    {
                        if (iUnsParam > 0)
                        {
                            var iNumTileIDs = (from sZItem in S9.Section9Z
                                               where sZItem.ZLayer == iUnsLayer &
                                                     sZItem.ZTexture > 0xE & sZItem.ZTexture <= iUnsTexture &
                                                     sZItem.ZParam == iUnsParam &
                                                     sZItem.ZState == iUnsState
                                               orderby sZItem.ZTileID
                                               group sZItem by sZItem.ZTileID).Distinct().ToList();

                            if (iNumTileIDs.Count == 1)
                            {
                                if (FileTools.strGlobalFieldName == "crater_2")
                                {
                                    querylstImagesRGBColors = (from itmImage in ImageTools.lstImagesRGBColors
                                                               where itmImage.FirstTileID == iFirstTileID &
                                                                     itmImage.FirstTexture == iFirstTexture &
                                                                     itmImage.FirstPalette == iFirstPalette &
                                                                     itmImage.Param == iUnsParam &
                                                                     itmImage.State == iUnsState &
                                                                     itmImage.Used == false
                                                               select itmImage).ToList();
                                }
                                else
                                {
                                    querylstImagesRGBColors = (from itmImage in ImageTools.lstImagesRGBColors
                                                               where itmImage.FirstTileID == iFirstTileID &
                                                                     itmImage.Param == iUnsParam &
                                                                     itmImage.State == iUnsState &
                                                                     itmImage.Used == false
                                                               select itmImage).ToList();
                                }
                            }
                            else
                            {
                                // Count by Palettes.
                                querylstImagesRGBColors = (from itmImage in ImageTools.lstImagesRGBColors
                                                           where itmImage.FirstTileID == iFirstTileID &
                                                                 itmImage.FirstTexture == iFirstTexture &
                                                                 itmImage.FirstPalette == iFirstPalette &
                                                                 itmImage.Param == iUnsParam &
                                                                 itmImage.State == iUnsState &
                                                                 itmImage.Used == false
                                                           select itmImage).ToList();
                            }
                        }
                        else
                        {
                            var iNumTileIDs = (from sZItem in S9.Section9Z
                                               where sZItem.ZLayer == iUnsLayer &
                                                     sZItem.ZTexture > 0xE & sZItem.ZTexture <= iUnsTexture &
                                                     sZItem.ZParam == iUnsParam
                                               orderby sZItem.ZTileID
                                               group sZItem by sZItem.ZTileID).Distinct().ToList();

                            if (iNumTileIDs.Count == 1)
                            {
                                if (FileTools.strGlobalFieldName == "crater_2")
                                {
                                    querylstImagesRGBColors = (from itmImage in ImageTools.lstImagesRGBColors
                                                               where itmImage.FirstTileID == iFirstTileID &
                                                                     itmImage.FirstTexture == iFirstTexture &
                                                                     itmImage.FirstPalette == iFirstPalette &
                                                                     itmImage.Palette == iUnsPalette &
                                                                     itmImage.Param == iUnsParam &
                                                                     itmImage.Used == false
                                                               select itmImage).ToList();
                                }
                                else
                                {
                                    querylstImagesRGBColors = (from itmImage in ImageTools.lstImagesRGBColors
                                                               where itmImage.FirstTileID == iFirstTileID &
                                                                     itmImage.FirstTexture == iFirstTexture &
                                                                     itmImage.FirstPalette == iFirstPalette &
                                                                     itmImage.Param == iUnsParam &
                                                                     itmImage.Used == false
                                                               select itmImage).ToList();
                                }
                            }
                            else
                            {
                                querylstImagesRGBColors = (from itmImage in ImageTools.lstImagesRGBColors
                                                           where itmImage.FirstTileID == iFirstTileID &
                                                                 itmImage.FirstTexture == iFirstTexture &
                                                                 itmImage.FirstPalette == iFirstPalette &
                                                                 itmImage.Param == iUnsParam &
                                                                 itmImage.Used == false
                                                           select itmImage).ToList();
                            }
                        }

                        break;
                    }

                case 2:
                case 3:
                    {
                        querylstImagesRGBColors = (from itmImage in ImageTools.lstImagesRGBColors
                                                   where itmImage.Layer == iUnsLayer &
                                                         (itmImage.Texture != iUnsTexture |
                                                          itmImage.Palette != iUnsPalette) &
                                                         itmImage.Param == iUnsParam &
                                                         itmImage.State == iUnsState &
                                                         itmImage.Used == false
                                                   select itmImage).ToList();
                        break;
                    }
            }

            if (querylstImagesRGBColors.Count > 0)
            {
                if (querylstImagesRGBColors.Count == 1)
                {
                    // If iUnsLayer <> 1 Then
                    strUnsFile = querylstImagesRGBColors[0].strFile;
                    ImageTools.UpdateRGBColorsUsed(strUnsFile);
                    bResult = true;
                }
                else
                {
                    iBestHashDetected = ImageTools.FindBestHashDetected(bmpUnsInputTexture,
                                                                        itmUnswizzledTexture, 
                                                                        querylstImagesRGBColors);
                    if (iBestHashDetected != 9999)
                    {
                        strUnsFile = querylstImagesRGBColors[iBestHashDetected].strFile;
                        ImageTools.UpdateRGBColorsUsed(strUnsFile);
                        bResult = true;
                        bUseSameRGBColorForList = true;
                    }
                }
            }

            return bResult;
        }

        public static int UnswizzleHashedTextureBatch(string strOutputDirectory, 
                                                      string strFileField, 
                                                      string strUnsHash, 
                                                      int iUnsTexture, 
                                                      int iUnsPalette, 
                                                      Bitmap bmpUnsInputTexture, 
                                                      ref string strUnsFile,
                                                      string strOutputFolder)
        {

            // Ok. Let's try to output all the Unswizzled Textures by param/state.
            // More or less is the same as the Swizzled, but limiting the output to a given param/state and palette.
            Bitmap bmpFirstTexture;

            List<S9.S9_ZList> sortZTexture = new List<S9.S9_ZList>();

            int iFirstTileID, iFirstTexture, iFirstPalette, iLastTexture, iLastPalette;
            int iUnsLayer, iUnsTileID, iUnsParam, iUnsState;
            string strCRC32;
            int iResult, iNumPalettesByParam;

            iResult = 0;

            // This vars will be used to reuse the same RGB Color for a given param/state.
            bool bUseSameRGBColorForList = false;
            bool bNextHashDetection = false;

            List<HashExceptions> queryHashException;
            List<HashExceptions> queryHashExceptionT2;
            List<HashExceptions> queryHashExceptionT3;

            string strBaseFileRGBColor = "";

            iFirstTileID = 0;
            iFirstTexture = 0;
            iFirstPalette = 0;
            iLastTexture = 0;
            iLastPalette = 0;

            // Var for use in Hash Exception Type 2 (Addition)
            string strUnsFileAddition, strUnsFileAdditionFolder;

            strUnsFileAddition = "";
            strUnsFileAdditionFolder = "";

            // Let's check that parameters are correct (like in 'lastmap' field which palette = 8 is wrong).
            if (iUnsPalette > S4.Section4.numPalettes - 1)
            {
                return 3;
            }

            // Let's get a CRC32 hash checking the palette colors.
            strCRC32 = HashCRC.CreateHash_IDX_PAL(bmpUnsInputTexture);

            // Let's init the UnswizzledImagesList
            InitUnswizzledImagesList();

            try
            {
                // Let's do this depending on the texture input.
                // First Texture:   Find first texture used in the given hash and save it.
                // Next Texture(s): Find next textures used in the given hash, 
                // For each hashed texture that corresponds, overwrite the image joined (checking CRC or RGBPalette).
                // Save it in place of the first texture and add to TextureLinks.

                // Let's get the layer of this texture.
                iUnsLayer = (from sZItem in S9.Section9Z
                             where sZItem.ZTexture == iUnsTexture & 
                                   sZItem.ZPalette == iUnsPalette
                             orderby sZItem.ZParam
                             select sZItem.ZLayer).First();

                iResult = Obtain_HashedTextureLayer(iUnsLayer, 
                                                    iUnsTexture, iUnsPalette, 
                                                    bmpUnsInputTexture);

                foreach (var stUnswizzledImage in lstUnswizzledImagesList)
                {

                    // For processing unswizzled textures we will need to know the layer and other values.
                    if (stUnswizzledImage.stUnswizzledTextureParamsStates.TileID.Keys[0] < 6666)
                    {
                        {
                            GetFirstTexturePalette(iUnsLayer, 
                                                   stUnswizzledImage.stUnswizzledTextureParamsStates.TileID.Keys[0], 
                                                   iUnsTexture, iUnsPalette,
                                                   stUnswizzledImage.stUnswizzledTextureParamsStates.Param,
                                                   stUnswizzledImage.stUnswizzledTextureParamsStates.State,
                                                   ref iFirstTileID, ref iFirstTexture, ref iFirstPalette);
                        }
                    }


                    // Let's determine if the image processed is first image or secondary image (join process).
                    // = 9999 condition is for field "blin_2", or the special tiles.
                    if ((iFirstTexture == stUnswizzledImage.stUnswizzledTextureParamsStates.Texture &
                         iFirstPalette == stUnswizzledImage.stUnswizzledTextureParamsStates.Palette &
                         iFirstTileID == stUnswizzledImage.stUnswizzledTextureParamsStates.TileID.Keys[0]) |
                        stUnswizzledImage.stUnswizzledTextureParamsStates.TileID.Keys[0] >= 6666)
                    {

                        // -------------------------------------------------------------------------
                        // First Texture:   Find first texture used in the given hash and save it,
                        // adding CRC32 and RGBList.
                        // -------------------------------------------------------------------------

                        {
                            // Let's put the Param and State in variables.
                            iUnsTexture = stUnswizzledImage.stUnswizzledTextureParamsStates.Texture;
                            iUnsPalette = stUnswizzledImage.stUnswizzledTextureParamsStates.Palette;
                            iUnsParam = stUnswizzledImage.stUnswizzledTextureParamsStates.Param;
                            iUnsState = stUnswizzledImage.stUnswizzledTextureParamsStates.State;

                            iUnsTileID = stUnswizzledImage.stUnswizzledTextureParamsStates.TileID.Keys[0];

                            strUnsFile = strOutputDirectory + "\\" + strFileField + "_" + 
                                         iUnsTexture.ToString("00") + "_" + 
                                         iUnsPalette.ToString("00") + "_" + 
                                         iUnsParam.ToString("00") + "_" + 
                                         iUnsState.ToString("00") + "_" + 
                                         iUnsTileID.ToString("0000") + ".png";

                            // We have the info in strUnsFile for both previous cases, so, we can
                            // save it outside the checking
                            if (!Directory.Exists(Path.GetDirectoryName(strUnsFile)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(strUnsFile));
                            }

                            ImageTools.WriteBitmap(stUnswizzledImage.bmpUnswizzledImages, strUnsFile);

                            // We need to write the CRC32 and RGBList for this texture.
                            HashCRC.AddCRC32ToList(strCRC32, strUnsFile, 
                                                   iFirstTexture, iFirstPalette, iFirstTileID, 
                                                   iUnsTexture, iUnsPalette, iUnsTileID, 
                                                   iUnsParam, iUnsState);

                            ImageTools.AddRGBColorsToList(bmpUnsInputTexture, 
                                                          stUnswizzledImage.bmpUnswizzledImages, 
                                                          strUnsFile, iUnsLayer, 
                                                          iFirstTexture, iFirstPalette, iFirstTileID, 
                                                          iUnsTexture, iUnsPalette,
                                                          stUnswizzledImage.stUnswizzledTextureParamsStates.TileID,
                                                          iUnsParam, iUnsState);
                        }


                        // Let's check if there is any First Texture we need to duplicate.
                        // This is Virtual Type 2 Hash Exception, or Addition ("+"), but changing some values.
                        if (bHashExceptions)
                        {
                            // Let's check if the hash of input file is in list of Hash Exceptions list.
                            // This is the portion of code for Virtual Exceptions Type 2 (Add Image to Unswizzled Hashed Image)
                            // -------------------------------------------------------------------------------------------------
                            queryHashExceptionT2 = (from itmHashException in lstHashExceptions
                                                    where itmHashException.MatchTextureHash == strUnsHash &
                                                          itmHashException.MatchTexture == iUnsTexture &
                                                          itmHashException.MatchPalette == iUnsPalette &
                                                          itmHashException.Param == iUnsParam &
                                                          itmHashException.State == iUnsState &
                                                          itmHashException.TileID == iUnsTileID &
                                                          itmHashException.VirtualType == 2
                                                    select itmHashException).ToList();

                            if (queryHashExceptionT2.Count > 0)
                            {
                                // Let's prepare the file to load.
                                strUnsFileAdditionFolder = strOutputFolder;

                                // As we can have more than one exception type 2, we will need to loop.
                                strUnsFileAddition = strUnsFileAdditionFolder + "\\" +
                                                     queryHashExceptionT2[0].Texture.ToString("00") + "_" +
                                                     queryHashExceptionT2[0].Palette.ToString("00") + "\\" +
                                                     queryHashExceptionT2[0].FirstTextureHash + "\\" + 
                                                     strFileField + "_" +
                                                     queryHashExceptionT2[0].Texture.ToString("00") + "_" +
                                                     queryHashExceptionT2[0].Palette.ToString("00") + "_" +
                                                     queryHashExceptionT2[0].Param.ToString("00") + "_" +
                                                     queryHashExceptionT2[0].State.ToString("00") + "_" +
                                                     queryHashExceptionT2[0].TileID.ToString("0000") + ".png";

                                if (!Directory.Exists(Path.GetDirectoryName(strUnsFileAddition)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(strUnsFileAddition));
                                }

                                // Duplicate First Texture image.
                                ImageTools.WriteBitmap(stUnswizzledImage.bmpUnswizzledImages, strUnsFileAddition);

                                // Add Texture Link.
                                Add_TextureLink(queryHashExceptionT2[0].FirstTextureHash,
                                                queryHashExceptionT2[0].MatchTexture, 
                                                queryHashExceptionT2[0].MatchPalette,
                                                queryHashExceptionT2[0].Param,
                                                queryHashExceptionT2[0].State,
                                                queryHashExceptionT2[0].MatchTextureHash,
                                                queryHashExceptionT2[0].TileID,
                                                queryHashExceptionT2[0].MatchTexture,
                                                queryHashExceptionT2[0].MatchPalette);

                                // We need to write the CRC32 and RGBList for this texture.
                                HashCRC.AddCRC32ToList(strCRC32, strUnsFileAddition, 
                                                       iFirstTexture, iFirstPalette, iFirstTileID,
                                                       iUnsTexture, iUnsPalette, iUnsTileID, 
                                                       iUnsParam, iUnsState);

                                ImageTools.AddRGBColorsToList(bmpUnsInputTexture, 
                                                              stUnswizzledImage.bmpUnswizzledImages,
                                                              strUnsFileAddition, iUnsLayer, 
                                                              iFirstTexture, iFirstPalette, iFirstTileID, 
                                                              iUnsTexture, iUnsPalette, 
                                                              stUnswizzledImage.stUnswizzledTextureParamsStates.TileID, 
                                                              iUnsParam, iUnsState);
                            }
                        }
                    }
                    else
                    {
                        // -------------------------------------------------------------------------
                        // Next Texture(s): Find next textures used in the given hash, 
                        // For each hashed texture that corresponds:
                        // - we must unswizzle the texture of every palette of the next texture, 
                        // - overwrite the image joined (checking CRC32 Or RGBPalette).
                        // - Save it in place of the first texture and add to TextureLinks.
                        // -------------------------------------------------------------------------
                        bool bJoinImage = false;
                        Bitmap bmpUnsTexture = null;

                        // Let's put the Param and State in variables.
                        iUnsTexture = stUnswizzledImage.stUnswizzledTextureParamsStates.Texture;
                        iUnsPalette = stUnswizzledImage.stUnswizzledTextureParamsStates.Palette;
                        iUnsParam = stUnswizzledImage.stUnswizzledTextureParamsStates.Param;
                        iUnsState = stUnswizzledImage.stUnswizzledTextureParamsStates.State;
                        iUnsTileID = stUnswizzledImage.stUnswizzledTextureParamsStates.TileID.Keys[0];

                        iNumPalettesByParam = (from sZItem in S9.Section9Z
                                               where (sZItem.ZTexture > 0xE & sZItem.ZTexture < 0x1A) &
                                                      sZItem.ZParam == iUnsParam
                                               group sZItem by sZItem.ZPalette).Distinct().Count();

                        // Clean bUseSameRGBColorForList and strBaseFileRGBColor if TileID = 256 (&H100).
                        // The first two conditions are for fix this code for whitein.
                        // The second two conditions are for fix this code for psdun_4.
                        // The third two condition are for fix this code for semkin_4.

                        if ((iUnsTileID == 0x100 | iUnsTileID == 0x120) | 
                            (FileTools.strGlobalFieldName == "psdun_4" & iUnsTileID == 0xFFE) |
                             FileTools.strGlobalFieldName == "crater_2")
                        {
                            bUseSameRGBColorForList = false;
                            strBaseFileRGBColor = "";
                        }


                        // As we can have hash exceptions, let's check if there is any for this check.
                        if (bHashExceptions)
                        {
                            // Let's check if the hash of input file is in list of Hash Exceptions list.
                            // This is the portion of code for Virtual Exceptions Type 0 (Virtual Hash Link)
                            // ------------------------------------------------------------------------------
                            queryHashException = (from itmHashException in lstHashExceptions
                                                  where itmHashException.MatchTextureHash == strUnsHash &
                                                        itmHashException.Param == iUnsParam &
                                                        itmHashException.State == iUnsState &
                                                        itmHashException.TileID == iFirstTileID &
                                                        itmHashException.VirtualType == 0
                                                  select itmHashException).ToList();

                            if (queryHashException.Count() == 0)
                            {
                                bNextHashDetection = false;
                            }
                            else
                            {
                                // Let's prepare the file to load.
                                strUnsFile = strOutputFolder;

                                // Prepare name of the file
                                strUnsFile = strUnsFile + "\\" + 
                                             queryHashException[0].Texture.ToString("00") + "_" +
                                             queryHashException[0].Palette.ToString("00") + "\\" +
                                             queryHashException[0].FirstTextureHash + "\\" + 
                                             strFileField + "_" +
                                             queryHashException[0].Texture.ToString("00") + "_" +
                                             queryHashException[0].Palette.ToString("00") + "_" +
                                             queryHashException[0].Param.ToString("00") + "_" +
                                             queryHashException[0].State.ToString("00") + "_" +
                                             queryHashException[0].TileID.ToString("0000") + ".png";

                                bNextHashDetection = true;
                                bJoinImage = true;

                                HashCRC.UpdateCRC32Used(strUnsFile);
                                ImageTools.UpdateRGBColorsUsed(strUnsFile);
                            }


                            // Let's check if the hash of input file is in list of Hash Exceptions list.
                            // This is the portion of code for Virtual Exceptions Type 2 (Add Image to Unswizzled Hashed Image)
                            // -------------------------------------------------------------------------------------------------
                            queryHashExceptionT2 = (from itmHashException in lstHashExceptions
                                                    where itmHashException.MatchTextureHash == strUnsHash &
                                                          itmHashException.MatchTexture == iUnsTexture &
                                                          itmHashException.MatchPalette == iUnsPalette &
                                                          itmHashException.Param == iUnsParam &
                                                          itmHashException.State == iUnsState &
                                                          itmHashException.TileID == iUnsTileID &
                                                          itmHashException.VirtualType == 2
                                                    select itmHashException).ToList();

                            if (queryHashExceptionT2.Count > 0)
                            {
                                // Let's prepare the file to load.
                                strUnsFileAdditionFolder = strOutputFolder;

                                // As we can have more than one exception type 2, we will need to loop.
                                foreach (var itmHashExceptionT2 in queryHashExceptionT2)
                                {
                                    // Prepare name of the file to add the duplicated hash.
                                    strUnsFileAddition = strUnsFileAdditionFolder + "\\" + 
                                                         itmHashExceptionT2.Texture.ToString("00") + "_" + 
                                                         itmHashExceptionT2.Palette.ToString("00") + "\\" + 
                                                         itmHashExceptionT2.FirstTextureHash + "\\" + 
                                                         strFileField + "_" + 
                                                         itmHashExceptionT2.Texture.ToString("00") + "_" + 
                                                         itmHashExceptionT2.Palette.ToString("00") + "_" + 
                                                         itmHashExceptionT2.Param.ToString("00") + "_" + 
                                                         itmHashExceptionT2.State.ToString("00") + "_" + 
                                                         itmHashExceptionT2.TileID.ToString("0000") + ".png";

                                    // Load File where to add this image.
                                    bmpFirstTexture = null;

                                    ImageTools.ReadBitmap(ref bmpFirstTexture, strUnsFileAddition);

                                    // Add/Copy the image
                                    // We need to copy only the Tiles for the new loaded
                                    // Texture for its palette.
                                    CopyUnswizzledTextures(ref bmpFirstTexture, 
                                                           stUnswizzledImage.bmpUnswizzledImages);

                                    // Save the image
                                    ImageTools.WriteBitmap(bmpFirstTexture, strUnsFileAddition);

                                    bmpFirstTexture.Dispose();
                                }
                            }


                            // Let's check if the hash of input file is in list of Hash Exceptions list.
                            // This is the portion of code for Virtual Exceptions Type 3 (Tex/Pal/
                            // Change Tex/Pal/Param/State Destination for Add to Unswizzled Hashed Image
                            // -----------------------------------------------------------------------------------------------------------------
                            queryHashExceptionT3 = (from itmHashException in lstHashExceptions
                                                    where itmHashException.MatchTextureHash == strUnsHash &
                                                          itmHashException.DestTexture == iUnsTexture &
                                                          itmHashException.DestPalette == iUnsPalette &
                                                          itmHashException.DestParam == iUnsParam &
                                                          itmHashException.DestState == iUnsState &
                                                          itmHashException.DestTileID == iUnsTileID &
                                                          itmHashException.VirtualType == 3
                                                    select itmHashException).ToList();

                            if (queryHashExceptionT3.Count == 0)
                            {
                                bNextHashDetection = bNextHashDetection | false;
                            }
                            else
                            {
                                // Let's prepare the file to load.
                                strUnsFileAdditionFolder = strOutputFolder;

                                foreach (var itmHashExceptionT3 in queryHashExceptionT3)
                                {
                                    // Prepare name of the file where to put the destination texture/pal combination.
                                    strUnsFileAddition = strUnsFileAdditionFolder + "\\" + 
                                                         itmHashExceptionT3.Texture.ToString("00") + "_" + 
                                                         itmHashExceptionT3.Palette.ToString("00") + "\\" + 
                                                         itmHashExceptionT3.FirstTextureHash + "\\" + 
                                                         strFileField + "_" + 
                                                         itmHashExceptionT3.Texture.ToString("00") + "_" + 
                                                         itmHashExceptionT3.Palette.ToString("00") + "_" + 
                                                         itmHashExceptionT3.Param.ToString("00") + "_" + 
                                                         itmHashExceptionT3.State.ToString("00") + "_" + 
                                                         itmHashExceptionT3.TileID.ToString("0000") + ".png";

                                    // Check if image exists. If so, load the previous unswizzled image to copy this one.
                                    // Else, let's create a new one.
                                    bmpFirstTexture = null;

                                    if (File.Exists(strUnsFileAddition))
                                    {
                                        ImageTools.ReadBitmap(ref bmpFirstTexture, strUnsFileAddition);

                                        // Add/Copy the image
                                        // We need to copy only the Tiles for the new loaded
                                        // Texture for its palette.
                                        CopyUnswizzledTextures(ref bmpFirstTexture, 
                                                               stUnswizzledImage.bmpUnswizzledImages);
                                    }

                                    // Save the image
                                    ImageTools.WriteBitmap(bmpFirstTexture, strUnsFileAddition);

                                    bmpFirstTexture.Dispose();

                                    // Let's put the TextureLink
                                    Add_TextureLink(itmHashExceptionT3.FirstTextureHash, 
                                                    itmHashExceptionT3.DestTexture, 
                                                    itmHashExceptionT3.DestPalette, 
                                                    itmHashExceptionT3.DestParam, 
                                                    itmHashExceptionT3.DestState, 
                                                    itmHashExceptionT3.MatchTextureHash, 
                                                    itmHashExceptionT3.TileID, 
                                                    itmHashExceptionT3.Texture, 
                                                    itmHashExceptionT3.Palette);
                                }
                            }
                        }

                        if (!bNextHashDetection)
                        {
                            if (!bUseSameRGBColorForList)
                            {
                                // Let's know the last texture used.
                                // Check by Texture or Palette?
                                GetLastTexturePalette(iUnsTexture, iUnsPalette, 
                                                      iUnsTileID, iUnsParam, iUnsState, 
                                                      iUnsLayer, 
                                                      iFirstTexture, iFirstPalette, 
                                                      ref iLastTexture, ref iLastPalette);

                                // -----------------------------------------
                                // Let's Check Texture CRC32 in List Entries
                                // -------------------------------------------
                                bJoinImage = CheckImageByCRC32(strCRC32, iUnsTileID, iUnsLayer, 
                                                               iUnsTexture, iUnsPalette, 
                                                               iFirstTexture, iFirstPalette, iFirstTileID, 
                                                               iLastTexture, iLastPalette, 
                                                               iUnsParam, iUnsState, ref strUnsFile);

                                // We need to update RGBColors list also. If not there could be inconsistences.
                                if (bJoinImage)
                                    ImageTools.UpdateRGBColorsUsed(strUnsFile);

                                // ---------------------------------------------
                                // Let's Check Texture RGB Color in List Entries
                                // ----------------------------------------------
                                if (!bJoinImage)
                                {
                                    bJoinImage = CheckImageByRGBColor(bmpUnsInputTexture, 
                                                                      stUnswizzledImage.bmpUnswizzledImages, 
                                                                      iUnsLayer, iNumPalettesByParam, 
                                                                      iUnsTexture, iUnsPalette, iUnsTileID, 
                                                                      iFirstTexture, iFirstPalette, iFirstTileID, 
                                                                      iLastTexture, iLastPalette, 
                                                                      iUnsParam, iUnsState, 
                                                                      ref strUnsFile, ref bUseSameRGBColorForList);

                                    if (bJoinImage)
                                    {
                                        // We need to update CRC32 list also. If not there could be inconsistences.
                                        HashCRC.UpdateCRC32Used(strUnsFile);

                                        // This will help to not repeat the search if there are other Params/States in this same
                                        // texture.
                                        strBaseFileRGBColor = strUnsFile;
                                    }
                                }
                            }
                            else
                            {
                                // Although we have the strBaseFile for use the BestColor of the RGB Colors List, 
                                // there are some fields that have in a different palette/texture the image.
                                // Let's retrieve the file to be sure from the lstRGBColors.
                                ImageTools.SearchRGBListFile(iUnsTileID, iUnsParam, iUnsState,
                                                             iFirstTexture, iFirstPalette, 
                                                             ref strBaseFileRGBColor, strOutputDirectory);

                                strUnsFile = strBaseFileRGBColor;

                                HashCRC.UpdateCRC32Used(strUnsFile);
                                ImageTools.UpdateRGBColorsUsed(strUnsFile);

                                bJoinImage = true;
                            }
                        }

                        if (bJoinImage)
                        {

                            // We found the texture to which we must join the one we are
                            // working with, for a given param/state and CRC32/RGBColor. Steps:
                            // 1.- Load the first texture.
                            // 2.- Copy the one we are working with into the loaded one.
                            // 3.- Create the link and put it in the list.
                            // 4.- Save the Texture.

                            // 1.- Load the first texture.
                            bmpFirstTexture = null;
                            ImageTools.ReadBitmap(ref bmpFirstTexture, strUnsFile);

                            // We need to copy only the Tiles for the new loaded
                            // Texture for its palette.
                            CopyUnswizzledTextures(ref bmpFirstTexture, 
                                                   stUnswizzledImage.bmpUnswizzledImages);

                            bmpUnsTexture = new Bitmap(bmpFirstTexture);

                            bmpFirstTexture.Dispose();

                            // 4. Finally we make the texture link and put it in the list of texture links.
                            // If the InputHash is the same as the other hash, we will not put the link.
                            Add_TextureLink(Path.GetDirectoryName(strUnsFile).Split(Path.DirectorySeparatorChar).Last(), 
                                            iUnsTexture, iUnsPalette, 
                                            iUnsParam, iUnsState, 
                                            strUnsHash, iUnsTileID, 
                                            iFirstTexture, iFirstPalette);


                            // Let's update (if needed) the Color List with the new joined image (update texture/palette).
                            HashCRC.UpdateCRC32List(strCRC32, strUnsFile, iUnsLayer, iUnsTileID, 
                                                    iUnsTexture, iUnsPalette, 
                                                    iUnsParam, iUnsState);

                            ImageTools.UpdateRGBColorsList(bmpUnsTexture, 
                                                           strUnsFile, iUnsLayer, iUnsTileID, 
                                                           iUnsTexture, iUnsPalette, 
                                                           iUnsParam, iUnsState);
                        }
                        else
                        {
                            // There is no other texture, so, we will write it directly.
                            // Supposedly is the first texture that uses a concrete param/state.
                            strUnsFile = strOutputDirectory + "\\" + strFileField + "_" + 
                                         iUnsTexture.ToString("00") + "_" + 
                                         iUnsPalette.ToString("00") + "_" + 
                                         iUnsParam.ToString("00") + "_" + 
                                         iUnsState.ToString("00") + "_" + 
                                         iUnsTileID.ToString("0000") + ".png";

                            // We need to add this info also in the CRC32 And RGBColors lists
                            HashCRC.AddCRC32ToList(strCRC32, strUnsFile, 
                                                   iFirstTexture, iFirstPalette, iFirstTileID, 
                                                   iUnsTexture, iUnsPalette, 
                                                   iUnsTileID, iUnsParam, iUnsState);

                            ImageTools.AddRGBColorsToList(bmpUnsInputTexture, 
                                                          stUnswizzledImage.bmpUnswizzledImages, 
                                                          strUnsFile, iUnsLayer, 
                                                          iFirstTexture, iFirstPalette, iFirstTileID, 
                                                          iUnsTexture, iUnsPalette, 
                                                          stUnswizzledImage.stUnswizzledTextureParamsStates.TileID, 
                                                          iUnsParam, iUnsState);

                            // Let's prepare the image to write.
                            bmpUnsTexture = new Bitmap(stUnswizzledImage.bmpUnswizzledImages);
                        }


                        // Save the file.
                        // We have the info in strUnsFile, we can save it
                        if (!Directory.Exists(Path.GetDirectoryName(strUnsFile)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(strUnsFile));
                        }

                        // Save Unswizzled
                        ImageTools.WriteBitmap(bmpUnsTexture, strUnsFile);
                        bmpUnsTexture.Dispose();
                        bJoinImage = false;
                    }
                }


                // Dipose List of Bitmaps and Params/State for the next image.
                InitUnswizzledImagesList();
            }
            catch (Exception ex)
            {
                iResult = 1;
            }

            return iResult;
        }

        public static void SwizzleHashedImage(Bitmap bmpUnsImage, ref Bitmap bmpSwizzledTexture, 
                                              int iSwizzleTexTexture, int iSwizzleTexPalette, 
                                              int iSwizzleTexParam, int iSwizzleTexState, 
                                              int iSwizzleTexTileID, int iScaleFactor)
        {
            int iTileSize;
            var sortZTexture = new List<S9.S9_ZList>();
            var iIdxHashExceptionTileSeparation = default(int);
            bool bRenderTile, bHasSeparateTile, bIsSpecialSeparateTileFile;


            // Prepare Vars (this will help for tile separation).
            bRenderTile = true;
            bHasSeparateTile = false;
            bIsSpecialSeparateTileFile = false;
            sortZTexture.Clear();


            // We must search in the list of the Texture/Palette Swizzled Hash image inputted.
            if (bTileSeparation)
            {
                // Search the index where Texture and Palette are
                bHasSeparateTile = false;
                if (iSwizzleTexTileID > 6666)
                {
                    iSwizzleTexTileID = iSwizzleTexTileID - 6666;
                    bIsSpecialSeparateTileFile = true;
                }

                while (iIdxHashExceptionTileSeparation < lstTileSeparation.Count & !bHasSeparateTile)
                {
                    if (lstTileSeparation[iIdxHashExceptionTileSeparation].Texture == iSwizzleTexTexture &
                        lstTileSeparation[iIdxHashExceptionTileSeparation].Palette == iSwizzleTexPalette)
                    {
                        bHasSeparateTile = true;
                    }
                    else
                    {
                        iIdxHashExceptionTileSeparation = iIdxHashExceptionTileSeparation + 1;
                    }
                }
            }

            switch (FileTools.strGlobalFieldName)
            {
                case "ancnt2":
                    {
                        if (iSwizzleTexParam != 0)
                        {
                            sortZTexture = (from sortZItem in S9.Section9Z
                                            where sortZItem.ZTexture == iSwizzleTexTexture &
                                                  sortZItem.ZPalette == iSwizzleTexPalette &
                                                  sortZItem.ZParam == iSwizzleTexParam &
                                                  sortZItem.ZState == iSwizzleTexState
                                            orderby sortZItem.ZTileID
                                            select sortZItem).ToList();
                        }
                        else
                        {
                            sortZTexture = (from sortZItem in S9.Section9Z
                                            where sortZItem.ZTexture == iSwizzleTexTexture &
                                                  sortZItem.ZPalette == iSwizzleTexPalette &
                                                  sortZItem.ZParam == iSwizzleTexParam &
                                                  sortZItem.ZTileID == iSwizzleTexTileID
                                            orderby sortZItem.ZTileID
                                            select sortZItem).ToList();
                        }

                        break;
                    }

                case "blin2_i":
                    {
                        if (iSwizzleTexParam > 0 | iSwizzleTexTileID > 0xC00)               // iSwizzleTexTileID > &HC00 is for blin2_i
                        {
                            sortZTexture = (from sortZItem in S9.Section9Z
                                            where sortZItem.ZTexture == iSwizzleTexTexture &
                                                  sortZItem.ZPalette == iSwizzleTexPalette &
                                                  sortZItem.ZParam == iSwizzleTexParam &
                                                  sortZItem.ZState == iSwizzleTexState
                                            orderby sortZItem.ZTileID
                                            select sortZItem).ToList();
                        }
                        else
                        {
                            sortZTexture = (from sortZItem in S9.Section9Z
                                            where sortZItem.ZTexture == iSwizzleTexTexture &
                                                  sortZItem.ZPalette == iSwizzleTexPalette &
                                                  sortZItem.ZParam == iSwizzleTexParam &
                                                  sortZItem.ZTileID == iSwizzleTexTileID
                                            orderby sortZItem.ZTileID
                                            select sortZItem).ToList();
                        }

                        break;
                    }

                case "blue_2":
                    {
                        sortZTexture = (from sortZItem in S9.Section9Z
                                        where sortZItem.ZTexture == iSwizzleTexTexture &
                                              sortZItem.ZPalette == iSwizzleTexPalette &
                                              sortZItem.ZParam == iSwizzleTexParam &
                                              sortZItem.ZState == iSwizzleTexState &
                                              sortZItem.ZTileID == iSwizzleTexTileID
                                        orderby sortZItem.ZTileID
                                        select sortZItem).ToList();
                        break;
                    }

                case "snow":
                    {
                        sortZTexture = (from sortZItem in S9.Section9Z
                                        where sortZItem.ZTexture == iSwizzleTexTexture &
                                              sortZItem.ZPalette == iSwizzleTexPalette &
                                              sortZItem.ZTileID >= iSwizzleTexTileID
                                        orderby sortZItem.ZTileID
                                        select sortZItem).ToList();
                        break;
                    }

                default:
                    {
                        sortZTexture = (from sortZItem in S9.Section9Z
                                        where sortZItem.ZTexture == iSwizzleTexTexture &
                                              sortZItem.ZPalette == iSwizzleTexPalette &
                                              sortZItem.ZParam == iSwizzleTexParam &
                                              sortZItem.ZState == iSwizzleTexState
                                        orderby sortZItem.ZTileID
                                        select sortZItem).ToList();
                        break;
                    }
            }

            if (sortZTexture.Count > 0)
            {
                iTileSize = sortZTexture[0].ZTileSize * iScaleFactor;

                using (var g = Graphics.FromImage(bmpSwizzledTexture))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.CompositingMode = CompositingMode.SourceCopy;

                    foreach (var ZItem in sortZTexture)
                    {
                        if (bTileSeparation & bHasSeparateTile)
                        {
                            if (lstTileSeparation[iIdxHashExceptionTileSeparation].lstTileIDSeparate.Contains(ZItem.ZTile))
                            {
                                if (bIsSpecialSeparateTileFile)
                                {
                                    bRenderTile = true;
                                }
                                else
                                {
                                    bRenderTile = false;
                                }
                            }
                            else if (bIsSpecialSeparateTileFile)
                            {
                                bRenderTile = false;
                            }
                            else
                            {
                                bRenderTile = true;
                            }
                        }

                        if (bRenderTile)
                        {
                            g.DrawImage(bmpUnsImage, 
                                        ZItem.ZSourceX * iScaleFactor, ZItem.ZSourceY * iScaleFactor, 
                                        new Rectangle((iLayerbmpPosXGlobal + ZItem.ZDestX) * iScaleFactor, 
                                                      (iLayerbmpPosYGlobal + ZItem.ZDestY) * iScaleFactor, 
                                                      iTileSize, iTileSize), 
                                        GraphicsUnit.Pixel);
                        }
                    }
                }
            }
        }

        public static int SwizzleHashedExternalImage(string strInputDirectory, string strInputHash, 
                                                     ref int iSwizzleTexTexture, ref int iSwizzleTexPalette, 
                                                     ref int iSwizzleTexParam, ref int iSwizzleTexState, 
                                                     ref int iSwizzleTexTileID, ref string strFileField, 
                                                     ref Bitmap bmpSwizzledCompleteTexture)
        {
            Bitmap bmpSwizzledTexture, bmpUnsImage;
            string[] strListHashedUnsTexFiles;
            string strHash;
            int iScaleFactor;
            bool bHashedTexture;

            bmpUnsImage = null;

            // 1. Get Hashed Images file names from the folder.
            strListHashedUnsTexFiles = Directory.GetFiles(strInputDirectory, "*.png", SearchOption.TopDirectoryOnly);
            strHash = strInputHash;
            if (strListHashedUnsTexFiles.Count() == 0)
                return 2;
            try
            {

                // 2. Process each Swizzled Texture and output the different TextureIDs
                foreach (var strHashedUnsTexFile in strListHashedUnsTexFiles)
                {
                    bHashedTexture = FileTools.
                                     SplitFileNameAndCheckHash(Path.GetFileNameWithoutExtension(strHashedUnsTexFile),
                                                               ref strFileField, 
                                                               ref iSwizzleTexTexture, ref iSwizzleTexPalette, 
                                                               ref iSwizzleTexParam, ref iSwizzleTexState, 
                                                               ref iSwizzleTexTileID, ref strHash);

                    if (!FileTools.ValidateFilewithField(strFileField))
                    {
                        strFileField = Path.GetFileNameWithoutExtension(strHashedUnsTexFile);
                        return 0;
                    }

                    // Load texture and detect Scale Factor.
                    ImageTools.ReadBitmap(ref bmpUnsImage, strHashedUnsTexFile);

                    //iScaleFactor = bmpLoadedTexture.Width / S9.iLayersMaxWidth;
                    int iTexture = iSwizzleTexTexture, iPalette = iSwizzleTexPalette;
                    int iLayer = (from itmZList in S9.Section9Z
                                  where itmZList.ZTexture == iTexture &
                                        itmZList.ZPalette == iPalette
                                  select itmZList.ZLayer).Distinct().First();

                    if (iLayer < 2)
                    {
                        if (iScaleFactorL0 == 0)
                        {
                            // Prepare ScaleFactor for L0/L1
                            iScaleFactorL0 = bmpUnsImage.Width / S9.iLayersMaxWidthL0;

                            iLayerMaxWidthGlobal = S9.iLayersMaxWidthL0;
                            iLayerMaxHeightGlobal = S9.iLayersMaxHeightL0;

                            iLayerbmpPosXGlobal = S9.iLayersbmpPosXL0;
                            iLayerbmpPosYGlobal = S9.iLayersbmpPosYL0;
                        }

                        iScaleFactor = iScaleFactorL0;
                    }
                    else
                    {
                        if (iScaleFactorL2 == 0)
                        {
                            // Prepare ScaleFactor for L2/L3
                            iScaleFactorL2 = bmpUnsImage.Width / S9.iLayersMaxWidthL2;

                            iLayerMaxWidthGlobal = S9.iLayersMaxWidthL2;
                            iLayerMaxHeightGlobal = S9.iLayersMaxHeightL2;

                            iLayerbmpPosXGlobal = S9.iLayersbmpPosXL2;
                            iLayerbmpPosYGlobal = S9.iLayersbmpPosYL2;
                        }

                        iScaleFactor = iScaleFactorL2;
                    }

                    if (bmpUnsImage.Width % iLayerMaxWidthGlobal != 0) return 4;


                    // Prepare Bitmap for the Swizzled Texture.
                    bmpSwizzledTexture = new Bitmap(S9.TEXTURE_WIDTH * iScaleFactor,
                                                    S9.TEXTURE_HEIGHT * iScaleFactor);

                    SwizzleHashedImage(bmpUnsImage, ref bmpSwizzledTexture, 
                                       iSwizzleTexTexture, iSwizzleTexPalette, 
                                       iSwizzleTexParam, iSwizzleTexState, iSwizzleTexTileID, iScaleFactor);

                    if (bmpSwizzledCompleteTexture == null)
                            bmpSwizzledCompleteTexture = new Bitmap(S9.TEXTURE_WIDTH * iScaleFactor, 
                                                                    S9.TEXTURE_HEIGHT * iScaleFactor);

                    using (Graphics g = Graphics.FromImage(bmpSwizzledCompleteTexture))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.CompositingMode = CompositingMode.SourceOver;
                        g.DrawImage(bmpSwizzledTexture,
                                    new Rectangle(0, 0, bmpSwizzledTexture.Width, bmpSwizzledTexture.Height),
                                    0, 0, bmpSwizzledTexture.Width, bmpSwizzledTexture.Height,
                                    GraphicsUnit.Pixel);
                    }

                    bmpUnsImage.Dispose();
                    bmpSwizzledTexture.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when processing the Individual Texture for Swizzle."+
                                "\n\rTexture: " + iSwizzleTexTexture.ToString() + 
                                "\n\rPalette: " + iSwizzleTexPalette.ToString() +
                                "\n\rParam: " + iSwizzleTexParam.ToString() +
                                "\n\rState: " + iSwizzleTexState.ToString());
                return 0;
            }

            return 1;
        }


        // In this process we must create (n)Hashed Swizzled Textures & Palettes combination 
        // From one Unswizzled Texture. (n) can be >= 0.
        // Steps:
        // 1. Get Hashed Images file names from the folder
        // 2. Check the number of different Swizzled Textures we must ouput
        // 3. For Each Swizzled Texture we can have, Check the number of different Swizzled Palettes we must output.
        // 4. Process each Swizzled Texture / Palette and output the different TextureIDs / PaletteID.   
        // 5. Save complete Swizzled Texture for each TextureID/PaletteID as needed.

        public static int SwizzleHashedImagesBatch(string strInputDirectory, 
                                                   string strOutputDirectory, 
                                                   string strInputHash, 
                                                   int iSwizzleTexTexture, 
                                                   int iSwizzleTexPalette, 
                                                   string strFileField, 
                                                   ref string strProcessFileName)
        {
            List<TextureLink> queryTextureLinks = new List<TextureLink>();
            //List<S9.S9_ZList> queryZItemsList = new List<S9.S9_ZList>();
            string[] strListHashedUnsTexFiles;
            string strHash, strHashedSwizzledTexFile, strFileHash;
            int iSwizzledPathTexture, iSwizzledPathPalette;
            int iSwizzleTexParam, iSwizzleTexState, iSwizzleTexTileID, iScaleFactor;
            Bitmap bmpSwizzledCompleteTexture, bmpSwizzledTexture, bmpUnsImage, bmpSwizTexTmp;
            bool bHashedTexture;

            strFileHash = "";
            strProcessFileName = "";
            bmpSwizzledCompleteTexture = null;
            bmpUnsImage = null;
            bmpSwizTexTmp = null;
            iSwizzleTexParam = 0;
            iSwizzleTexState = 0;
            iSwizzleTexTileID = 0;
            iSwizzledPathTexture = 0;
            iSwizzledPathPalette = 0;

            iScaleFactor = 0;
            iScaleFactorL0 = 0;
            iScaleFactorL2 = 0;

            // 1. Get Hashed Images file names from the folder.
            strListHashedUnsTexFiles = Directory.GetFiles(strInputDirectory, "*.png", SearchOption.TopDirectoryOnly);
            strHash = strInputHash;

            if (strListHashedUnsTexFiles.Count() == 0)
                return 2;
            try
            {

                // We will process the Unswizzled Textures in two steps:
                // 1.- We will treat the Base texture and save it (not in TextureLink).
                // 2.- We will treat each Texture Link, load the Base Texture, update it, and save it.


                // -------------------------------------------------------------------------
                // 1.- We will treat the Base texture and save it (not in TextureLink).
                // -------------------------------------------------------------------------
                foreach (var strHashedUnsTexFile in strListHashedUnsTexFiles)
                {
                    bHashedTexture = FileTools.SplitFileNameAndCheckHash(
                                                  Path.GetFileNameWithoutExtension(strHashedUnsTexFile), 
                                                  ref strFileField, ref iSwizzleTexTexture, ref iSwizzleTexPalette,
                                                  ref iSwizzleTexParam, ref iSwizzleTexState, 
                                                  ref iSwizzleTexTileID, ref strHash);


                    // Check if it is from the loaded field
                    if (!FileTools.ValidateFilewithField(strFileField))
                    {
                        strProcessFileName = Path.GetFileNameWithoutExtension(strHashedUnsTexFile);
                        return 0;
                    }


                    // '''''''''''''''''''''''''''''''''''''''''''''''''
                    // We can now prepare the first Swizzled Texture.
                    // Load texture with determined sizes and detect scale factor.
                    ImageTools.ReadBitmap(ref bmpUnsImage, strHashedUnsTexFile);

                    //iScaleFactor = bmpLoadedTexture.Width / S9.iLayersMaxWidth;
                    int iLayer = (from itmZList in S9.Section9Z
                                  where itmZList.ZTexture == iSwizzleTexTexture &
                                        itmZList.ZPalette == iSwizzleTexPalette
                                  select itmZList.ZLayer).Distinct().First();

                    if (iLayer < 2)
                    {
                        if (iScaleFactorL0 == 0)
                        {
                            // Prepare ScaleFactor for L0/L1
                            iScaleFactorL0 = bmpUnsImage.Width / S9.iLayersMaxWidthL0;

                            iLayerMaxWidthGlobal = S9.iLayersMaxWidthL0;
                            iLayerMaxHeightGlobal = S9.iLayersMaxHeightL0;

                            iLayerbmpPosXGlobal = S9.iLayersbmpPosXL0;
                            iLayerbmpPosYGlobal = S9.iLayersbmpPosYL0;
                        }

                        iScaleFactor = iScaleFactorL0;
                    }
                    else
                    {
                        if (iScaleFactorL2 == 0)
                        {
                            // Prepare ScaleFactor for L2/L3
                            iScaleFactorL2 = bmpUnsImage.Width / S9.iLayersMaxWidthL2;

                            iLayerMaxWidthGlobal = S9.iLayersMaxWidthL2;
                            iLayerMaxHeightGlobal = S9.iLayersMaxHeightL2;

                            iLayerbmpPosXGlobal = S9.iLayersbmpPosXL2;
                            iLayerbmpPosYGlobal = S9.iLayersbmpPosYL2;
                        }

                        iScaleFactor = iScaleFactorL2;
                    }

                    if (bmpUnsImage.Width % iLayerMaxWidthGlobal != 0) return 4;


                    // Prepare Bitmap for the Swizzled Texture.
                    bmpSwizzledTexture = new Bitmap(S9.TEXTURE_WIDTH * iScaleFactor,
                                                    S9.TEXTURE_HEIGHT * iScaleFactor);

                    SwizzleHashedImage(bmpUnsImage, ref bmpSwizzledTexture,
                                       iSwizzleTexTexture, iSwizzleTexPalette,
                                       iSwizzleTexParam, iSwizzleTexState,
                                       iSwizzleTexTileID, iScaleFactor);

                    bmpUnsImage.Dispose();
                    // End preparing first Swizzled Texture
                    // '''''''''''''''''''''''''''''''''''''''''''''''''


                    if (bmpSwizzledCompleteTexture == null)
                        bmpSwizzledCompleteTexture = new Bitmap(S9.TEXTURE_WIDTH * iScaleFactor,
                                                                S9.TEXTURE_HEIGHT * iScaleFactor);

                    using (Graphics g = Graphics.FromImage(bmpSwizzledCompleteTexture))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.CompositingMode = CompositingMode.SourceOver;
                        g.DrawImage(bmpSwizzledTexture, 
                                    0, 0, bmpSwizzledTexture.Width, bmpSwizzledTexture.Height);
                    }

                    bmpSwizzledTexture.Dispose();
                }

                // 4. Save complete Base Swizzled Texture 
                strProcessFileName = strOutputDirectory + "\\" + strFileField + "_" + 
                                     iSwizzleTexTexture.ToString("00") + "_" + 
                                     iSwizzleTexPalette.ToString("00") + "_" + 
                                     strHash + ".png";

                // '''''''''''''''' Check if Base Texture exists. There can be some Textures that
                // '''''''''''''''' use different Palette and they can be Base Texture.
                if (File.Exists(strProcessFileName))
                {
                    ImageTools.ReadBitmap(ref bmpUnsImage, strProcessFileName);

                    using (Graphics g = Graphics.FromImage(bmpSwizzledCompleteTexture))
                    {
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.CompositingMode = CompositingMode.SourceOver;

                        g.DrawImage(bmpUnsImage, 
                                    0, 0, bmpUnsImage.Width, bmpUnsImage.Height);
                    }

                    bmpUnsImage.Dispose();
                }

                ImageTools.WriteBitmap(bmpSwizzledCompleteTexture, strProcessFileName);

                bmpSwizzledCompleteTexture.Dispose();
                bmpSwizzledCompleteTexture = null;



                // -------------------------------------------------------------------------------------------
                // 2.- We will treat each Texture Link, load the First Hashed Texture, update it, and save it.
                // -------------------------------------------------------------------------------------------
                foreach (var strHashedUnsTexFile in strListHashedUnsTexFiles)
                {
                    bHashedTexture = FileTools.SplitFileNameAndCheckHash(
                                            Path.GetFileNameWithoutExtension(strHashedUnsTexFile), 
                                            ref strFileField, ref iSwizzleTexTexture, ref iSwizzleTexPalette,
                                            ref iSwizzleTexParam, ref iSwizzleTexState, 
                                            ref iSwizzleTexTileID, ref strHash);


                    // Check if it is from the loaded field
                    if (!FileTools.ValidateFilewithField(strFileField))
                    {
                        strProcessFileName = Path.GetFileNameWithoutExtension(strHashedUnsTexFile);
                        return 0;
                    }

                    // Get the Texture and Palette of the Path. We will do the search of the Links with
                    // the First Texture and Palette.
                    FileTools.GetPathTexturePalette(Path.GetDirectoryName(strHashedUnsTexFile), 
                                                    ref strFileHash, 
                                                    ref iSwizzledPathTexture, 
                                                    ref iSwizzledPathPalette);

                    switch (FileTools.strGlobalFieldName)
                    {
                        case "icedun_2":
                        case "las2_1":
                        case "smkin_4":
                        case "trnad_3":
                        case "nivl_b1":
                        case "nivl_b12":
                            {
                                queryTextureLinks = (from itmTextureLink in lstTextureLinks
                                                     where itmTextureLink.FirstTextureHash == strInputHash &
                                                           itmTextureLink.FirstTextureID == iSwizzleTexTexture &
                                                           itmTextureLink.FirstPalette == iSwizzleTexPalette &
                                                           itmTextureLink.Param == iSwizzleTexParam &
                                                           itmTextureLink.State == iSwizzleTexState
                                                     orderby itmTextureLink.Texture, itmTextureLink.Palette, 
                                                             itmTextureLink.Param, itmTextureLink.State, 
                                                             itmTextureLink.TileID
                                                     select itmTextureLink).ToList();
                                break;
                            }

                        case "las2_2":
                            {
                                queryTextureLinks = (from itmTextureLink in lstTextureLinks
                                                     where itmTextureLink.FirstTextureHash == strInputHash &
                                                           itmTextureLink.FirstTextureID == iSwizzleTexTexture &
                                                           itmTextureLink.FirstPalette == iSwizzleTexPalette &
                                                           itmTextureLink.Param == iSwizzleTexParam &
                                                           itmTextureLink.State == iSwizzleTexState &
                                                           itmTextureLink.TileID == iSwizzleTexTileID
                                                     orderby itmTextureLink.Texture, itmTextureLink.Palette, 
                                                             itmTextureLink.Param, itmTextureLink.State, 
                                                             itmTextureLink.TileID
                                                     select itmTextureLink).ToList();
                                break;
                            }

                        case "las2_3":
                            {
                                queryTextureLinks = (from itmTextureLink in lstTextureLinks
                                                     where itmTextureLink.FirstTextureHash == strInputHash &
                                                           itmTextureLink.FirstTextureID == iSwizzleTexTexture &
                                                           itmTextureLink.FirstPalette == iSwizzleTexPalette &
                                                           itmTextureLink.Param == iSwizzleTexParam &
                                                           itmTextureLink.State == iSwizzleTexState &
                                                           itmTextureLink.TileID == iSwizzleTexTileID
                                                     orderby itmTextureLink.Texture, itmTextureLink.Palette,
                                                             itmTextureLink.Param, itmTextureLink.State,
                                                             itmTextureLink.TileID
                                                     select itmTextureLink).ToList();
                                break;
                            }

                        default:
                            {
                                queryTextureLinks = (from itmTextureLink in lstTextureLinks
                                                     where itmTextureLink.FirstTextureHash == strInputHash &
                                                           itmTextureLink.FirstTextureID == iSwizzleTexTexture &
                                                           itmTextureLink.FirstPalette == iSwizzleTexPalette
                                                     orderby itmTextureLink.Texture, itmTextureLink.Palette,
                                                             itmTextureLink.Param, itmTextureLink.State,
                                                             itmTextureLink.TileID
                                                     select itmTextureLink).ToList();
                                break;
                            }
                    }


                    // Let's check TextureLinks. If we have TextureLinks, we can work with the next image.
                    if (queryTextureLinks.Count() > 0)
                    {
                        // '''''''''''''''''''''''''''''''''''''''''''''''''
                        // We can now prepare the first Swizzled Texture.
                        // Load texture with determined sizes and detect scale factor.
                        ImageTools.ReadBitmap(ref bmpUnsImage, strHashedUnsTexFile);

                        foreach (var queryTextureLink in queryTextureLinks)
                        {
                            iSwizzleTexTexture = queryTextureLink.Texture;
                            iSwizzleTexPalette = queryTextureLink.Palette;
                            iSwizzleTexParam = queryTextureLink.Param;
                            iSwizzleTexState = queryTextureLink.State;
                            iSwizzleTexTileID = queryTextureLink.TileID;
                            strHash = queryTextureLink.MatchTextureHash;

                            // Swizzle Secondary Unswizzled Texture.
                            // Prepare Bitmap for the Swizzled Texture.
                            bmpSwizzledTexture = new Bitmap(S9.TEXTURE_WIDTH * iScaleFactor, 
                                                            S9.TEXTURE_HEIGHT * iScaleFactor);

                            SwizzleHashedImage(bmpUnsImage, ref bmpSwizzledTexture, 
                                               iSwizzleTexTexture, iSwizzleTexPalette, 
                                               iSwizzleTexParam, iSwizzleTexState, 
                                               iSwizzleTexTileID, iScaleFactor);


                            // Load Base Texture (it must have been done)
                            // Prepare Name
                            strHashedSwizzledTexFile = strOutputDirectory + "\\" + strFileField + "_" + 
                                                       iSwizzleTexTexture.ToString("00") + "_" +
                                                       iSwizzleTexPalette.ToString("00") + "_" +
                                                       strHash + ".png";


                            // Let's check if the file exists, if not, we will create a new one.
                            if (!File.Exists(strHashedSwizzledTexFile))
                            {
                                // This special case is a new Base Texture. Let's put it for save.
                                bmpSwizTexTmp = new Bitmap(S9.TEXTURE_WIDTH * iScaleFactor, 
                                                           S9.TEXTURE_HEIGHT * iScaleFactor);
                            }
                            else
                            {
                                // This is a Secondary Texture. We need to add it to the Base Texture.
                                // Join both Swizzled Textures in the Complete Texture Bitmap.
                                ImageTools.ReadBitmap(ref bmpSwizTexTmp, strHashedSwizzledTexFile);
                            }

                            using (Graphics g = Graphics.FromImage(bmpSwizTexTmp))
                            {
                                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                                g.CompositingMode = CompositingMode.SourceOver;

                                g.DrawImage(bmpSwizzledTexture,
                                            0, 0, bmpSwizzledTexture.Width, bmpSwizzledTexture.Height);
                            }

                            bmpSwizzledTexture.Dispose();


                            // Prepare the Complete Texture
                            if (bmpSwizzledCompleteTexture == null)
                                bmpSwizzledCompleteTexture = new Bitmap(S9.TEXTURE_WIDTH * iScaleFactor, 
                                                                        S9.TEXTURE_HEIGHT * iScaleFactor);

                            using (Graphics g = Graphics.FromImage(bmpSwizzledCompleteTexture))
                            {
                                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                                g.CompositingMode = CompositingMode.SourceOver;

                                g.DrawImage(bmpSwizTexTmp, 
                                            0, 0, bmpSwizTexTmp.Width, bmpSwizTexTmp.Height);
                            }

                            bmpSwizTexTmp.Dispose();


                            // 4. Save complete Base Swizzled Texture 
                            strProcessFileName = strHashedSwizzledTexFile;

                            ImageTools.WriteBitmap(bmpSwizzledCompleteTexture, strProcessFileName);

                            bmpSwizzledCompleteTexture.Dispose();
                            bmpSwizzledCompleteTexture = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            return 1;
        }

        // Procedure to Copy  Uns Texture B  TO  Uns Texture A
        public static void CopyUnswizzledTextures(ref Bitmap bmpUnsTextureA, Bitmap bmpUnsTextureB)
        {
            using (Graphics g = Graphics.FromImage(bmpUnsTextureA))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.CompositingMode = CompositingMode.SourceOver;

                Rectangle rectTile = new Rectangle(0, 0, bmpUnsTextureA.Width, bmpUnsTextureA.Height);

                g.DrawImage(bmpUnsTextureB, rectTile, 
                            0, 0, bmpUnsTextureB.Width, bmpUnsTextureB.Height, 
                            GraphicsUnit.Pixel);
            }
        }


        // This option works with the files in "\hashexceptions\XXXXX.txt" were XXXXX is the Field Name.
        // This helps to solve little things for colors mismatching, hashes inconsistences, and other things.
        // The format is:
        // Line beggining with:    '#'   - Comment, not readed.
        // '*'   - Virtual Hash for Add a TextureLink (not drawn) which will be outputted Swizzled after. (more hashes that the main set)
        // '+'   - Virtual Hash for Add Unswizzled Image into another (less hashes to create main set)
        // We can use this also for create a Duplicate of the First Texture if needed (nivl_b1, nivl_b12)
        // '&'   - Virtual Hash for Change Tex/Pal/Param/State Destination of Unswizzled Image that goes into another. Sample crater_2
        // Normal Hash (Hash Exception), FMT:    Hash of First Texture, Texture Number of First Texture, Palette Number of First Texture, Param, State, Hash Exception Match of Other Texture, TileID of First Texture
        // This option helps to say to which unswizzled image pertains the new portion of an unswizzled image.
        // Hash Exception Type 1 (Virtual Link, more hashed images), format:
        // Hash of First Texture, Texture Number Which pertains, Palette Number which pertains, 
        // Param, State, Hash Exception Match of Other Texture, TileID which pertains
        // Hash Exception Type 2 (Addition, less hashed images), format:
        // Hash of First Texture, Texture of First Texture, Palette of First Texture, Param, State, 
        // Hash Exception Match Of Other Texture, TileID Of First Texture, Texture Of Other Texture,
        // Palette Of Other Texture
        // Hash Exception Type 3 (Change Unswizzled Image Destination), format:
        // Hash of Destination Texture, Texture of Destination Texture, Palette of Destination Texture, 
        // Param of Destination Texture, State of Destination Texture, Hash of Other Texture, 
        // TileID of Other Texture, Texture of Other Texture, Palette of Other Texture, 
        // Param of Other Texture, State of Other Texture, TileID of Destination Texture
        // 

        public static void Read_HashExceptions(string strHashExceptionsFile)
        {
            string strLine;
            string[] strLineSplit;
            var stHashException = new HashExceptions();
            lstHashExceptions.Clear();
            bHashExceptions = false;
            using (var fileHashExceptions = new StreamReader(strHashExceptionsFile))
            {
                while (!fileHashExceptions.EndOfStream)
                {
                    strLine = fileHashExceptions.ReadLine();
                    if (!string.IsNullOrEmpty(strLine))
                    {
                        if (strLine[0] != '#')
                        {
                            switch (strLine[0])
                            {
                                case '*':
                                    {
                                        strLineSplit = strLine.Substring(1).Split(',');
                                        stHashException.VirtualType = 1;
                                        break;
                                    }

                                case '+':
                                    {
                                        strLineSplit = strLine.Substring(1).Split(',');
                                        stHashException.MatchTexture = Int32.Parse(strLineSplit[7]);
                                        stHashException.MatchPalette = Int32.Parse(strLineSplit[8]);
                                        stHashException.VirtualType = 2;
                                        break;
                                    }

                                case '&':
                                    {
                                        strLineSplit = strLine.Substring(1).Split(',');
                                        stHashException.VirtualType = 3;
                                        stHashException.DestTexture = Int32.Parse(strLineSplit[7]);
                                        stHashException.DestPalette = Int32.Parse(strLineSplit[8]);
                                        stHashException.DestParam = Int32.Parse(strLineSplit[9]);
                                        stHashException.DestState = Int32.Parse(strLineSplit[10]);
                                        stHashException.DestTileID = Int32.Parse(strLineSplit[11]);
                                        break;
                                    }

                                default:
                                    {
                                        strLineSplit = strLine.Split(',');
                                        stHashException.VirtualType = 0;
                                        break;
                                    }
                            }

                            if (strLineSplit.Count() > 6)
                            {
                                stHashException.FirstTextureHash = strLineSplit[0];
                                stHashException.Texture = Int32.Parse(strLineSplit[1]);
                                stHashException.Palette = Int32.Parse(strLineSplit[2]);
                                stHashException.Param = Int32.Parse(strLineSplit[3]);
                                stHashException.State = Int32.Parse(strLineSplit[4]);
                                stHashException.MatchTextureHash = strLineSplit[5];
                                stHashException.TileID = Int32.Parse(strLineSplit[6]);
                            }
                            else
                            {
                                stHashException.FirstTextureHash = strLineSplit[0];
                                stHashException.Texture = Int32.Parse(strLineSplit[1]);
                                stHashException.Palette = Int32.Parse(strLineSplit[2]);
                                stHashException.Param = Int32.Parse(strLineSplit[3]);
                                stHashException.State = Int32.Parse(strLineSplit[4]);
                                stHashException.MatchTextureHash = strLineSplit[5];
                                stHashException.TileID = -1;
                            }

                            bHashExceptions = true;
                            lstHashExceptions.Add(stHashException);
                        }
                    }
                }
            }
        }

        public static bool ProcessVirtualHashExceptionType1(List<HashExceptions> queryHashExceptionList)
        {
            bool bResult;

            bResult = true;

            if (queryHashExceptionList.Count > 0)
            {
                foreach (var itmHashException in queryHashExceptionList)
                    Add_TextureLink(itmHashException.FirstTextureHash, 
                                    itmHashException.Texture, 
                                    itmHashException.Palette, 
                                    itmHashException.Param, 
                                    itmHashException.State, 
                                    itmHashException.MatchTextureHash, 
                                    itmHashException.TileID, 
                                    itmHashException.Texture,
                                    itmHashException.Palette);
            }
            else
            {
                bResult = false;
            }

            return bResult;
        }


        // Function to Add a TextureLink. We do this to avoid duplicates.
        public static void Add_TextureLink(string FirstTextureHash, int Texture, int Palette, int Param, int State, string MatchTextureHash, int TileID, int FirstTexture, int FirstPalette)
        {

            // Let's check if duplicated
            int iCounter = (from itmTextureLink in lstTextureLinks
                            where itmTextureLink.FirstTextureID == FirstTexture & itmTextureLink.FirstPalette == FirstPalette & (itmTextureLink.FirstTextureHash ?? "") == (FirstTextureHash ?? "") & itmTextureLink.Texture == Texture & itmTextureLink.Palette == Palette & itmTextureLink.Param == Param & itmTextureLink.State == State & (itmTextureLink.MatchTextureHash ?? "") == (MatchTextureHash ?? "") & itmTextureLink.TileID == TileID
                            select itmTextureLink).Count();
            if (iCounter == 0)
            {
                var stTextureLink = new TextureLink();
                stTextureLink.FirstTextureID = FirstTexture;
                stTextureLink.FirstPalette = FirstPalette;
                stTextureLink.FirstTextureHash = FirstTextureHash;
                stTextureLink.Texture = Texture;
                stTextureLink.Palette = Palette;
                stTextureLink.Param = Param;
                stTextureLink.State = State;
                stTextureLink.TileID = TileID;
                stTextureLink.MatchTextureHash = MatchTextureHash;
                lstTextureLinks.Add(stTextureLink);
            }
        }


        // Function for Write the txtTextureLinks.txt File reading structure in memory
        public static void Write_TextureLinks(string strSavePath)
        {
            if (lstTextureLinks.Count > 0)
            {
                using (var fileTextureLinks = new StreamWriter(strSavePath + "\\listTextureLinks.txt", false))
                {
                    fileTextureLinks.WriteLine("# Link Format: First Texture TextureID, First Texture Palette, First Texture Hash, Other TextureID, Other Palette, Param, State, Other Hash of TextureID, First TileID of the First Texture Hash");
                    fileTextureLinks.WriteLine("");

                    lstTextureLinks = lstTextureLinks.OrderBy(x => x.FirstTextureHash).ThenBy(x => x.Texture).ThenBy(x => x.Palette).ToList();

                    foreach (var TextureLink in lstTextureLinks)
                        fileTextureLinks.WriteLine(TextureLink.FirstTextureID.ToString() + "," + 
                                                   TextureLink.FirstPalette.ToString() + "," + 
                                                   TextureLink.FirstTextureHash + "," + 
                                                   TextureLink.Texture.ToString() + "," + 
                                                   TextureLink.Palette.ToString() + "," + 
                                                   TextureLink.Param.ToString() + "," + 
                                                   TextureLink.State.ToString() + "," + 
                                                   TextureLink.MatchTextureHash + "," + 
                                                   TextureLink.TileID.ToString());
                }
            }

            lstTextureLinks.Clear();
        }


        // Function to Read txtTextureLinks.txt File and put them in structure
        public static void Read_TextureLinks(string strLoadPath)
        {
            string strLine;
            string[] strLineSplit;
            var stTextureLink = new TextureLink();
            lstTextureLinks.Clear();
            bTextureLinks = false;
            using (var fileTextureLinks = new StreamReader(strLoadPath + "\\listTextureLinks.txt"))
            {
                fileTextureLinks.ReadLine();
                fileTextureLinks.ReadLine();
                while (!fileTextureLinks.EndOfStream)
                {
                    strLine = fileTextureLinks.ReadLine();
                    strLineSplit = strLine.Split(',');
                    stTextureLink.FirstTextureID = Int32.Parse(strLineSplit[0]);
                    stTextureLink.FirstPalette = Int32.Parse(strLineSplit[1]);
                    stTextureLink.FirstTextureHash = strLineSplit[2];
                    stTextureLink.Texture = Int32.Parse(strLineSplit[3]);
                    stTextureLink.Palette = Int32.Parse(strLineSplit[4]);
                    stTextureLink.Param = Int32.Parse(strLineSplit[5]);
                    stTextureLink.State = Int32.Parse(strLineSplit[6]);
                    stTextureLink.MatchTextureHash = strLineSplit[7];
                    stTextureLink.TileID = Int32.Parse(strLineSplit[8]);
                    lstTextureLinks.Add(stTextureLink);
                }
            }

            if (lstTextureLinks.Count > 0)
            {
                bTextureLinks = true;
            }
        }


        // This option works with the files in "\tileseparation\XXXXX.txt" were XXXXX is the Field Name.
        // This helps to solve things like the sublayers of some fields (like blue_2) where they are
        // joined with same Texture/Palette/Param/State/TileID and the separation must be done manually.
        // The format is:
        // Line beggining with:     FORMAT: ! - Separation symbol hash exception
        // - Texture
        // - Palette
        // | - Symbol where the tiles begin.
        // - Individual tiles separated by comma.
        public static void Read_TileSeparation(string strTileSeparationFile)
        {
            string strLine;
            string[] strLineSplit, strTilesSplit;
            st_TileSeparation stTileSeparation = new st_TileSeparation();

            lstTileSeparation.Clear();

            bHashExceptions = false;

            using (var fileTileSeparation = new StreamReader(strTileSeparationFile))
            {
                while (!fileTileSeparation.EndOfStream)
                {
                    strLine = fileTileSeparation.ReadLine();
                    if (!string.IsNullOrEmpty(strLine))
                    {
                        if (strLine[0] != '#')
                        {
                            switch (strLine[0])
                            {
                                case '!':
                                    {
                                        strLineSplit = strLine.Substring(1).Split('|');
                                        stTileSeparation.Texture = Int32.Parse(strLineSplit[0].Split(',')[0]);
                                        stTileSeparation.Palette = Int32.Parse(strLineSplit[0].Split(',')[1]);
                                        stTileSeparation.lstTileIDSeparate = new List<int>();
                                        strTilesSplit = strLineSplit[1].Split(',');

                                        foreach (var iTile in strTilesSplit)
                                            stTileSeparation.lstTileIDSeparate.Add(Int32.Parse(iTile));
                                        break;
                                    }
                            }

                            bTileSeparation = true;
                            lstTileSeparation.Add(stTileSeparation);
                        }
                    }
                }
            }
        }

        public static void Draw_HashedImage(List<S9.S9_ZList> sortZList, int iUnsLayer,
                                            Bitmap bmpInputUnswizzledTexture, int iType)
        {
            Bitmap bmpUnswizzledTexture;
            int iTileCounter, iIdxHashExceptionTileSeparation;
            bool bRenderTile, bHasSeparateTile;

            // Prepare Vars
            bRenderTile = true;
            bHasSeparateTile = false;
            iIdxHashExceptionTileSeparation = 0;

            if (sortZList[0].ZLayer < 2)
            {
                iLayerbmpPosXGlobal = S9.iLayersbmpPosXL0;
                iLayerbmpPosYGlobal = S9.iLayersbmpPosYL0;

                iLayerMaxWidthGlobal = S9.iLayersMaxWidthL0;
                iLayerMaxHeightGlobal = S9.iLayersMaxHeightL0;
            }
            else
            {
                iLayerbmpPosXGlobal = S9.iLayersbmpPosXL2;
                iLayerbmpPosYGlobal = S9.iLayersbmpPosYL2;

                iLayerMaxWidthGlobal = S9.iLayersMaxWidthL2;
                iLayerMaxHeightGlobal = S9.iLayersMaxHeightL2;
            }

            // Define bitmap size of the hashed image
            bmpUnswizzledTexture = new Bitmap(iLayerMaxWidthGlobal, 
                                              iLayerMaxHeightGlobal);

            // We must search in the list of the Texture/Palette Swizzled Hash image inputted.
            if (bTileSeparation)
            {
                // Search the index where Texture and Palette are
                bHasSeparateTile = false;
                while (iIdxHashExceptionTileSeparation < lstTileSeparation.Count &
                       !bHasSeparateTile)
                {
                    if (lstTileSeparation[iIdxHashExceptionTileSeparation].Texture == sortZList[0].ZTexture &
                        lstTileSeparation[iIdxHashExceptionTileSeparation].Palette == sortZList[0].ZPalette)
                    {
                        bHasSeparateTile = true;
                    }
                    else
                    {
                        iIdxHashExceptionTileSeparation = iIdxHashExceptionTileSeparation + 1;
                    }
                }
            }


            // Let's decide which palette/tile/texture render to the background/stage.
            // Let's check if it is an indexed tile or a direct tile.
            using (var g = Graphics.FromImage(bmpUnswizzledTexture))
            {
                // Let's make it simple and copy the tile of the texture into the layer.
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.CompositingMode = CompositingMode.SourceOver;

                iTileCounter = 0;

                foreach (var sortZTile in sortZList)
                {
                    // Let's create the Image from Input Texture.
                    // There is one special image (blue_2) where we will need to check individual tiles.

                    if (bTileSeparation & bHasSeparateTile)
                    {
                        if (lstTileSeparation[iIdxHashExceptionTileSeparation].lstTileIDSeparate.Contains(sortZTile.ZTile))
                        {
                            bRenderTile = false;
                        }
                        else
                        {
                            bRenderTile = true;
                        }
                    }

                    if (bRenderTile)
                    {
                        g.DrawImage(bmpInputUnswizzledTexture,
                                    new Rectangle(iLayerbmpPosXGlobal + sortZTile.ZDestX,
                                                  iLayerbmpPosYGlobal + sortZTile.ZDestY,
                                                  sortZTile.ZTileSize, sortZTile.ZTileSize),
                                    sortZTile.ZSourceX, sortZTile.ZSourceY, sortZTile.ZTileSize, sortZTile.ZTileSize,
                                    GraphicsUnit.Pixel);

                        iTileCounter = iTileCounter + 1;
                    }
                }
            }



            // Let's check for those images with only 1 tile (like whitin) if it is Alpha all the pixels.
            // If so, we will not add the image to the list.
            switch (FileTools.strGlobalFieldName)
            {
                case "whitein":
                    {
                        if (iTileCounter == 1)
                        {
                            if (!ImageTools.CheckImageAlpha(bmpUnswizzledTexture))
                            {
                                // Add to the list.
                                lstUnswizzledImagesListAddEntry(bmpUnswizzledTexture, sortZList, 0, iType);
                            }
                        }
                        else
                        {
                            // Add to the list.
                            lstUnswizzledImagesListAddEntry(bmpUnswizzledTexture, sortZList, 0, iType);
                        }

                        break;
                    }

                default:
                    {
                        // Add to the list.
                        lstUnswizzledImagesListAddEntry(bmpUnswizzledTexture, sortZList, 0, iType);
                        break;
                    }
            }


            // PROCESSING for special fields with tile separation, like blue_2.
            if (bTileSeparation & bHasSeparateTile)
            {
                using (var g = Graphics.FromImage(bmpUnswizzledTexture))
                {
                    iTileCounter = 0;

                    // Let's make it simple and copy the tile of the texture into the layer.
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.CompositingMode = CompositingMode.SourceOver;

                    foreach (var itmTile in lstTileSeparation[iIdxHashExceptionTileSeparation].lstTileIDSeparate)
                    {
                        S9.S9_ZList individualZTile = (from sZItem in S9.Section9Z
                                                       where sZItem.ZTile == itmTile
                                                       select sZItem).First();

                        g.DrawImage(bmpInputUnswizzledTexture,
                                    new Rectangle(iLayerbmpPosXGlobal + individualZTile.ZDestX,
                                                  iLayerbmpPosYGlobal + individualZTile.ZDestY,
                                                  individualZTile.ZTileSize, individualZTile.ZTileSize),
                                    individualZTile.ZSourceX, individualZTile.ZSourceY,
                                    individualZTile.ZTileSize, individualZTile.ZTileSize,
                                    GraphicsUnit.Pixel);

                        iTileCounter = iTileCounter + 1;
                    }

                    // Add to the list.
                    lstUnswizzledImagesListAddEntry(bmpUnswizzledTexture, sortZList, 6666, iType);
                }
            }
        }

        public static int Obtain_HashedTextureLayer(int iLayer,
                                                    int iUnsTexture,
                                                    int iUnsPalette,
                                                    Bitmap bmpInputUnswizzledTexture)
        {
            List<S9.S9_ZList> sortZList = new List<S9.S9_ZList>();
            int iTileCounter, iResult, iNumPalettesByParamState;

            iResult = 0;
            iTileCounter = 0;

            try
            {
                switch (iLayer)
                {
                    case 0:
                        {
                            // Base Layer 0
                            sortZList = (from sortZItem in S9.Section9Z
                                         where sortZItem.ZLayer == iLayer &
                                               sortZItem.ZTexture == iUnsTexture &
                                               sortZItem.ZPalette == iUnsPalette
                                         orderby sortZItem.Z, sortZItem.ZTexture, sortZItem.ZPalette
                                         select sortZItem).ToList();

                            if (sortZList.Count > 0)
                            {
                                Draw_HashedImage(sortZList, iLayer, bmpInputUnswizzledTexture, 0);
                            }

                            break;
                        }

                    case 1:
                        {
                            // Base Layer 1
                            // We will do 3 works here.
                            // First work, create base layer with sublayers of main image
                            // Second work, create params/states for textures < &HF
                            // Third work, create params/states for textures > &HE

                            // We will check this for TileID, Param And State
                            // First work, create base layer with sublayers of main image
                            sortZList = (from sortZItem in S9.Section9Z
                                         where sortZItem.ZLayer == iLayer &
                                               (sortZItem.ZTexture == iUnsTexture && iUnsTexture < 0xF) &
                                               sortZItem.ZPalette == iUnsPalette &
                                               sortZItem.ZParam == 0
                                         orderby sortZItem.ZTexture, 
                                                 sortZItem.ZPalette
                                         select sortZItem).ToList();

                            if (sortZList.Count > 0)
                            {
                                Draw_HashedImage(sortZList, iLayer, bmpInputUnswizzledTexture, 0);
                            }


                            // Second work, create params/states for textures < &HF
                            var lstParams = (from sortZItem in S9.Section9Z
                                             where sortZItem.ZLayer == iLayer &
                                                   (sortZItem.ZTexture == iUnsTexture && iUnsTexture < 0xF) &
                                                   sortZItem.ZPalette == iUnsPalette
                                             group sortZItem by new
                                             {
                                                 sortZItem.ZParam
                                             } into ZParamGroup
                                             select new
                                             {
                                                 ZParamGroup.Key
                                             }).ToList();

                            if (lstParams.Count > 0)
                            {
                                foreach (var itmParam in lstParams)
                                {
                                    if (itmParam.Key.ZParam > 0)
                                    {
                                        var lstStates = (from sortZItem in S9.Section9Z
                                                         where sortZItem.ZLayer == iLayer &
                                                               (sortZItem.ZTexture == iUnsTexture && iUnsTexture < 0xF) &
                                                               sortZItem.ZPalette == iUnsPalette &
                                                               sortZItem.ZParam == itmParam.Key.ZParam
                                                         group sortZItem by new
                                                         {
                                                             sortZItem.ZState
                                                         } into ZStateGroup
                                                         select new
                                                         {
                                                             ZStateGroup.Key
                                                         }).ToList();

                                        if (lstStates.Count > 0)
                                        {
                                            foreach (var itmState in lstStates)
                                            {
                                                sortZList = (from sortZitem in S9.Section9Z
                                                             where sortZitem.ZLayer == iLayer &
                                                                   (sortZitem.ZTexture == iUnsTexture && iUnsTexture < 0xF) &
                                                                   sortZitem.ZPalette == iUnsPalette &
                                                                   sortZitem.ZParam == itmParam.Key.ZParam &
                                                                   sortZitem.ZState == itmState.Key.ZState
                                                             orderby sortZitem.ZPalette, sortZitem.ZTexture
                                                             select sortZitem).ToList();

                                                if (sortZList.Count > 0)
                                                {
                                                    Draw_HashedImage(sortZList, iLayer,
                                                                     bmpInputUnswizzledTexture, 4);
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            // Third work, create params/states for textures > &HE
                            var lstParams2 = (from sortZItem in S9.Section9Z
                                              where sortZItem.ZLayer == iLayer &
                                                    (sortZItem.ZTexture == iUnsTexture && iUnsTexture > 0xE) &
                                                    sortZItem.ZPalette == iUnsPalette
                                              orderby sortZItem.ZParam
                                              group sortZItem by new
                                              {
                                                  sortZItem.ZParam
                                              } into ZParamGroup
                                              select new
                                              {
                                                  ZParamGroup.Key
                                              }).ToList();


                            if (lstParams2.Count() > 0)
                            {
                                foreach (var itmParam in lstParams2)
                                {
                                    if (itmParam.Key.ZParam == 0)
                                    {
                                        var lstTileID = (from sortZItem in S9.Section9Z
                                                         where sortZItem.ZLayer == iLayer &
                                                               (sortZItem.ZTexture == iUnsTexture && iUnsTexture > 0xE) &
                                                               sortZItem.ZPalette == iUnsPalette &
                                                               sortZItem.ZParam == itmParam.Key.ZParam
                                                         orderby sortZItem.ZTileID
                                                         group sortZItem by new
                                                         {
                                                             sortZItem.ZTileID
                                                         } into ZTileIDGroup
                                                         select new
                                                         {
                                                             ZTileIDGroup.Key
                                                         }).ToList();

                                        foreach (var itmTileID in lstTileID)
                                        {
                                            if (itmTileID.Key.ZTileID == 0)
                                            {
                                                var lstPalettes = (from sortZItem in S9.Section9Z
                                                                   where sortZItem.ZLayer == iLayer &
                                                                         (sortZItem.ZTexture == iUnsTexture && iUnsTexture > 0xE) &
                                                                         sortZItem.ZPalette == iUnsPalette &
                                                                         sortZItem.ZTileID == itmTileID.Key.ZTileID &
                                                                         sortZItem.ZParam == itmParam.Key.ZParam
                                                                   orderby sortZItem.ZTile,
                                                                           sortZItem.ZTexture,
                                                                           sortZItem.ZPalette
                                                                   group sortZItem by new
                                                                   {
                                                                       sortZItem.ZPalette
                                                                   } into ZPaletteGroup
                                                                   select new
                                                                   {
                                                                       ZPaletteGroup.Key
                                                                   }).ToList();

                                                if (lstPalettes.Count() > 0)
                                                {
                                                    int iPaletteCounter = 0;
                                                    foreach (var itmPalette in lstPalettes)
                                                    {
                                                        sortZList = (from sortZitem in S9.Section9Z
                                                                     where sortZitem.ZLayer == iLayer &
                                                                           (sortZitem.ZTexture == iUnsTexture && iUnsTexture > 0xE) &
                                                                           sortZitem.ZPalette == iUnsPalette &
                                                                           sortZitem.ZTileID == itmTileID.Key.ZTileID &
                                                                           sortZitem.ZParam == itmParam.Key.ZParam
                                                                     orderby sortZitem.ZTexture, sortZitem.ZPalette
                                                                     select sortZitem).ToList();

                                                        if (sortZList.Count > 0)
                                                        {
                                                            if (sortZList[0].ZTileID == 0)
                                                            {
                                                                if (iPaletteCounter == 0)
                                                                {
                                                                    if (lstPalettes.Count > 1)
                                                                    {
                                                                        if (lstPalettes[1].Key.ZPalette < lstPalettes[0].Key.ZPalette - 1)
                                                                        {
                                                                            Draw_HashedImage(sortZList, iLayer,
                                                                                             bmpInputUnswizzledTexture, 7);
                                                                        }
                                                                        else
                                                                        {
                                                                            Draw_HashedImage(sortZList, iLayer,
                                                                                             bmpInputUnswizzledTexture, 0);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Draw_HashedImage(sortZList, iLayer,
                                                                                         bmpInputUnswizzledTexture, 0);
                                                                    }
                                                                }
                                                                else if (Math.Abs(lstPalettes[iPaletteCounter].Key.ZPalette - lstPalettes[iPaletteCounter - 1].Key.ZPalette) < 4)
                                                                {
                                                                    Draw_HashedImage(sortZList, iLayer,
                                                                                     bmpInputUnswizzledTexture, 6);
                                                                }
                                                                else
                                                                {
                                                                    Draw_HashedImage(sortZList, iLayer,
                                                                                     bmpInputUnswizzledTexture, 0);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Draw_HashedImage(sortZList, iLayer,
                                                                                 bmpInputUnswizzledTexture, 6);
                                                            }
                                                        }

                                                        iPaletteCounter = iPaletteCounter + 1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                sortZList = (from sortZitem in S9.Section9Z
                                                             where sortZitem.ZLayer == iLayer &
                                                                   (sortZitem.ZTexture == iUnsTexture && iUnsTexture > 0xE) &
                                                                   sortZitem.ZPalette == iUnsPalette &
                                                                   sortZitem.ZTileID == itmTileID.Key.ZTileID &
                                                                   sortZitem.ZParam == itmParam.Key.ZParam
                                                             orderby sortZitem.ZTexture, sortZitem.ZPalette
                                                             select sortZitem).ToList();

                                                if (sortZList.Count > 0)
                                                {
                                                    if (itmTileID.Key.ZTileID < 0x32)
                                                    {
                                                        Draw_HashedImage(sortZList, iLayer,
                                                                         bmpInputUnswizzledTexture, 0);
                                                    }
                                                    else if (itmTileID.Key.ZTileID > 0x7F & itmTileID.Key.ZTileID < 0xFF)
                                                    {
                                                        Draw_HashedImage(sortZList, iLayer,
                                                                         bmpInputUnswizzledTexture, 3);
                                                    }
                                                    else if (itmTileID.Key.ZTileID == 0x100)
                                                    {
                                                        Draw_HashedImage(sortZList, iLayer,
                                                                         bmpInputUnswizzledTexture, 5);
                                                    }
                                                    else if (itmTileID.Key.ZTileID > 0x800 &
                                                             FileTools.strGlobalFieldName != "whitein")
                                                    {
                                                        Draw_HashedImage(sortZList, iLayer,
                                                                         bmpInputUnswizzledTexture, 0);
                                                    }
                                                    else
                                                    {
                                                        Draw_HashedImage(sortZList, iLayer,
                                                                         bmpInputUnswizzledTexture, 8);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var lstStates = (from sortZItem in S9.Section9Z
                                                         where sortZItem.ZLayer == iLayer &
                                                               (sortZItem.ZTexture == iUnsTexture && iUnsTexture > 0xE) &
                                                               sortZItem.ZPalette == iUnsPalette &
                                                               sortZItem.ZParam == itmParam.Key.ZParam
                                                         group sortZItem by new
                                                         {
                                                             sortZItem.ZState
                                                         } into ZStateGroup
                                                         select new
                                                         {
                                                             ZStateGroup.Key
                                                         }).ToList();

                                        if (lstStates.Count() > 0)
                                        {
                                            if (FileTools.strGlobalFieldName == "blue_2")
                                            {
                                                foreach (var itmState in lstStates)
                                                {
                                                    var lstTileID = (from sortZitem in S9.Section9Z
                                                                     where sortZitem.ZLayer == iLayer &
                                                                           (sortZitem.ZTexture == iUnsTexture && sortZitem.ZTexture > 0xE) &
                                                                           sortZitem.ZPalette == iUnsPalette &
                                                                           sortZitem.ZParam == itmParam.Key.ZParam &
                                                                           sortZitem.ZState == itmState.Key.ZState
                                                                     group sortZitem by new
                                                                     {
                                                                         sortZitem.ZTileID
                                                                     } into ZTileIDGroup
                                                                     select new
                                                                     {
                                                                         ZTileIDGroup.Key
                                                                     }).Distinct().ToList();

                                                    foreach (var itmTileID in lstTileID)
                                                    {
                                                        sortZList = (from sortZitem in S9.Section9Z
                                                                     where sortZitem.ZLayer == iLayer &
                                                                           (sortZitem.ZTexture == iUnsTexture && sortZitem.ZTexture > 0xE) &
                                                                           sortZitem.ZPalette == iUnsPalette &
                                                                           sortZitem.ZTileID == itmTileID.Key.ZTileID &
                                                                           sortZitem.ZParam == itmParam.Key.ZParam &
                                                                           sortZitem.ZState == itmState.Key.ZState
                                                                     orderby sortZitem.ZTexture, sortZitem.ZPalette, sortZitem.ZTileID
                                                                     select sortZitem).ToList();

                                                        if (sortZList.Count > 0)
                                                        {
                                                            Draw_HashedImage(sortZList, iLayer,
                                                                             bmpInputUnswizzledTexture, 0);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (var itmState in lstStates)
                                                {
                                                    iNumPalettesByParamState = (from sortZItem in S9.Section9Z
                                                                                where sortZItem.ZLayer == iLayer &
                                                                                      (sortZItem.ZTexture == iUnsTexture && iUnsTexture > 0xE) &
                                                                                      sortZItem.ZPalette == iUnsPalette &
                                                                                      sortZItem.ZParam == itmParam.Key.ZParam &
                                                                                      sortZItem.ZState == itmState.Key.ZState
                                                                                group sortZItem by sortZItem.ZPalette).Distinct().Count();

                                                    if (iNumPalettesByParamState == 1)
                                                    {
                                                        sortZList = (from sortZitem in S9.Section9Z
                                                                     where sortZitem.ZLayer == iLayer &
                                                                           (sortZitem.ZTexture == iUnsTexture && iUnsTexture > 0xE) &
                                                                           sortZitem.ZPalette == iUnsPalette &
                                                                           sortZitem.ZParam == itmParam.Key.ZParam &
                                                                           sortZitem.ZState == itmState.Key.ZState
                                                                     orderby sortZitem.ZTexture, sortZitem.ZPalette, sortZitem.ZTileID
                                                                     select sortZitem).ToList();

                                                        if (sortZList.Count > 0)
                                                        {
                                                            if (sortZList[0].ZTileID < 0x100)
                                                            {
                                                                Draw_HashedImage(sortZList, iLayer,
                                                                                 bmpInputUnswizzledTexture, 0);
                                                            }
                                                            else
                                                            {
                                                                Draw_HashedImage(sortZList, iLayer,
                                                                                 bmpInputUnswizzledTexture, 8);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var lstTileID = (from sortZItem in S9.Section9Z
                                                                         where sortZItem.ZLayer == iLayer &
                                                                               (sortZItem.ZTexture == iUnsTexture && iUnsTexture > 0xE) &
                                                                               sortZItem.ZPalette == iUnsPalette &
                                                                               sortZItem.ZParam == itmParam.Key.ZParam &
                                                                               sortZItem.ZState == itmState.Key.ZState
                                                                         group sortZItem by new
                                                                         {
                                                                             sortZItem.ZTileID
                                                                         } into ZTileIDGroup
                                                                         select new
                                                                         {
                                                                             ZTileIDGroup.Key
                                                                         }).ToList();

                                                        foreach (var itmTileID in lstTileID)
                                                        {
                                                            sortZList = (from sortZitem in S9.Section9Z
                                                                         where sortZitem.ZLayer == iLayer &
                                                                               (sortZitem.ZTexture == iUnsTexture && iUnsTexture > 0xE) &
                                                                               sortZitem.ZPalette == iUnsPalette &
                                                                               sortZitem.ZTileID == itmTileID.Key.ZTileID &
                                                                               sortZitem.ZParam == itmParam.Key.ZParam &
                                                                               sortZitem.ZState == itmState.Key.ZState
                                                                         orderby sortZitem.ZPalette, sortZitem.ZTexture
                                                                         select sortZitem).ToList();

                                                            if (sortZList.Count > 0)
                                                            {
                                                                Draw_HashedImage(sortZList, iLayer,
                                                                                 bmpInputUnswizzledTexture, 3);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    case 2:
                        {
                            // Layer 2
                            sortZList = (from sortZItem in S9.Section9Z
                                         where sortZItem.ZLayer == iLayer
                                         orderby sortZItem.ZTexture, sortZItem.ZPalette
                                         select sortZItem).ToList();

                            // We will check this for TileID, Param And State
                            var lstTileIDs = (from sortZItem in S9.Section9Z
                                              where sortZItem.ZLayer == iLayer
                                              group sortZItem by new
                                              {
                                                  sortZItem.ZTileID
                                              } into ZTileIDGroup
                                              select new
                                              {
                                                  ZTileIDGroup.Key
                                              }).ToList();

                            if (lstTileIDs.Count > 0)
                            {
                                foreach (var itmTileID in lstTileIDs)
                                {
                                    var lstParams = (from sortZItem in S9.Section9Z
                                                     where sortZItem.ZLayer == iLayer &
                                                           sortZItem.ZTileID == itmTileID.Key.ZTileID
                                                     group sortZItem by new
                                                     {
                                                         sortZItem.ZParam
                                                     } into ZParamGroup
                                                     select new
                                                     {
                                                         ZParamGroup.Key
                                                     }).ToList();

                                    if (lstParams.Count() > 0)
                                    {
                                        foreach (var itmParam in lstParams)
                                        {
                                            var lstState = (from sortZItem in S9.Section9Z
                                                            where sortZItem.ZLayer == iLayer &
                                                                  sortZItem.ZTileID == itmTileID.Key.ZTileID &
                                                                  sortZItem.ZParam == itmParam.Key.ZParam
                                                            group sortZItem by new
                                                            {
                                                                sortZItem.ZState
                                                            } into ZStateGroup
                                                            select new
                                                            {
                                                                ZStateGroup.Key
                                                            }).ToList();

                                            if (lstState.Count() > 0)
                                            {
                                                foreach (var itmState in lstState)
                                                {
                                                    sortZList = (from sortZItem in S9.Section9Z
                                                                 where sortZItem.ZLayer == iLayer &
                                                                       sortZItem.ZTileID == itmTileID.Key.ZTileID &
                                                                       sortZItem.ZTexture == iUnsTexture &
                                                                       sortZItem.ZPalette == iUnsPalette &
                                                                       sortZItem.ZParam == itmParam.Key.ZParam &
                                                                       sortZItem.ZState == itmState.Key.ZState
                                                                 orderby sortZItem.ZTexture, sortZItem.ZPalette
                                                                 select sortZItem).ToList();

                                                    if (sortZList.Count > 0)
                                                    {
                                                        Draw_HashedImage(sortZList, iLayer,
                                                                         bmpInputUnswizzledTexture, 0);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    case 3:
                        {
                            // Layer 3
                            switch (FileTools.strGlobalFieldName)
                            {
                                case "trnad_3":
                                    {
                                        var lstParams = (from sortZItem in S9.Section9Z
                                                         where sortZItem.ZLayer == iLayer &
                                                               sortZItem.ZTexture == iUnsTexture &
                                                               sortZItem.ZPalette == iUnsPalette
                                                         group sortZItem by new
                                                         {
                                                             sortZItem.ZParam
                                                         } into ZParamGroup
                                                         select new
                                                         {
                                                             ZParamGroup.Key
                                                         }).ToList();

                                        foreach (var itmParam in lstParams)
                                        {
                                            sortZList = (from sortZItem in S9.Section9Z
                                                         where sortZItem.ZLayer == iLayer &
                                                               sortZItem.ZTexture == iUnsTexture &
                                                               sortZItem.ZPalette == iUnsPalette &
                                                               sortZItem.ZParam == itmParam.Key.ZParam
                                                         orderby sortZItem.ZParam
                                                         select sortZItem).ToList();

                                            if (sortZList.Count > 0)
                                            {
                                                Draw_HashedImage(sortZList, iLayer,
                                                                 bmpInputUnswizzledTexture, 3);
                                            }
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        sortZList = (from sortZItem in S9.Section9Z
                                                     where sortZItem.ZLayer == iLayer &
                                                           sortZItem.ZTexture == iUnsTexture &
                                                           sortZItem.ZPalette == iUnsPalette
                                                     orderby sortZItem.ZTexture, sortZItem.ZPalette
                                                     select sortZItem).ToList();

                                        if (sortZList.Count > 0)
                                        {
                                            Draw_HashedImage(sortZList, iLayer,
                                                             bmpInputUnswizzledTexture, 0);
                                        }

                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                if (CommandLine.bCmd)
                {
                    Console.WriteLine("Error while doing Obtain Hashed Texture process." + "Layer: " +
                                      iLayer.ToString() + ", " + "Tile: " + iTileCounter.ToString());
                }
                else
                {
                    MessageBox.Show("Error while doing Obtain Hashed Texture process.\n\rLayer: " +
                                    iLayer.ToString() + "\n\rTile: " + iTileCounter.ToString());
                }

                iResult = -1;
            }

            return iResult;
        }

    }
}