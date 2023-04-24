using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;


// The main purpose of this module is to make the conversion of a 24bit image (not 32 bit yet)
// into a multi-texture/multi-indexedpalette version.
namespace Aeris
{

    using static FileTools;

    public static class Repair_fr_e
    {
        public partial struct ARGB_NEWPALETTE
        {
            public Color[] ARGB_COLORS;
            public int iNumColors;
        }

        public static ARGB_NEWPALETTE[] ARGB_NEWPAL;
        public static HashSet<Color> lstColorList;
        public static Hashtable hlstTilePal = new Hashtable();
        public static int iNumTotalTiles;

        public static int Repair_Brokenfr_e()
        {
            int iResult;
            byte[] byteSection0;
            RichTextBox tmpRTB = new RichTextBox();

            try
            {
                // Check if the files needed are present.
                // Check if the loaded field is fr_e
                iResult = CheckPreliminars();

                if (iResult == 0)
                {
                    // If the checking is ok, now we need to update the Tiles of Layer 0
                    // with the new paletteID.
                    Modify_Layer3Tiles();
                    S9.Load_ZList(tmpRTB);

                    // Fix Textures.
                    // As the textures are fixed, we need to import them.
                    iResult = Fix_Textures();
                }

                // Now we need to apply the fixed script.
                if (iResult == 0)
                {
                    // We read the section0.bin file.
                    byteSection0 = File.ReadAllBytes(strGlobalPath + "\\fr_e_fixresources\\section0.bin");

                    if (byteSection0.Length > 0)
                    {
                        Field.fieldSection[0].sectionSize = byteSection0.Length;
                        Field.fieldSection[0].sectionData = byteSection0;
                    }
                }

            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                iResult = 99;
            }

            tmpRTB.Dispose();

            return iResult;
        }

        public static int CheckPreliminars()
        {
            int iResult;
            iResult = 0;

            // Check if files for do the fix exists
            if (!File.Exists(strGlobalPath + "\\fr_e_fixresources\\fr_e_16_00.png") | 
                !File.Exists(strGlobalPath + "\\fr_e_fixresources\\fr_e_17_00.png") | 
                !File.Exists(strGlobalPath + "\\fr_e_fixresources\\section0.bin"))
            {
                iResult = 1;
            }
            // Check if fr_e need to be updated.
            else if (S9.Section9.Layer[3].layerTiles[0].paletteID != 1)
            {
                iResult = 2;
            }

            return iResult;
        }

        public static void Modify_Layer3Tiles()
        {
            int iNumTile;

            for (iNumTile = 0; iNumTile < S9.Section9.Layer[3].numTiles; iNumTile++)
            {
                S9.Section9.Layer[3].layerTiles[iNumTile].paletteID = 0;
                S9.Section9.Layer[3].layerTiles[iNumTile].BlendMode = 1;
            }
        }

        public static int Fix_Textures()
        {
            int iResult;

            iResult = ImageTools.ImportTexture(strGlobalPath + "\\fr_e_fixresources\\fr_e_16_00.png", 3, 16, true);

            if (iResult == 0)
            {
                iResult = ImageTools.ImportTexture(strGlobalPath + "\\fr_e_fixresources\\fr_e_17_00.png", 3, 17, true);
            }

            return iResult;
        }
    }
}