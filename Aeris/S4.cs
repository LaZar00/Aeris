using System;
using System.IO;


namespace Aeris
{
    public static class S4
    {


        public partial struct S4_PaletteBytes
        {
            public byte Red;
            public byte Green;
            public byte Blue;
            public byte Mask;
        }

        public partial struct dataPal
        {
            public S4_PaletteBytes[] Pal;
        }

        public partial struct Section_4
        {
            // Public sectionSize As UInt32
            public uint iPalettesSize;
            public ushort PalX;
            public ushort PalY;
            public ushort numColorsPalette;
            public ushort numPalettes;
            public dataPal[] dataPalette;
        }


        // Public VARS
        public static Section_4 Section4;


        public static void Load_Section4(BinaryReader fSection4)
        {
            int iColor, iPalette;
            ushort color;

            // Read Section 4
            // Section4.sectionSize = fSection4.ReadUInt32()
            Section4.iPalettesSize = fSection4.ReadUInt32();
            Section4.PalX = fSection4.ReadUInt16();
            Section4.PalY = fSection4.ReadUInt16();
            Section4.numColorsPalette = fSection4.ReadUInt16();
            Section4.numPalettes = fSection4.ReadUInt16();

            // Read the pages with the palettes.
            // Prepare the palettes.
            Section4.dataPalette = new dataPal[Section4.numPalettes];

            for (int i = 0; i <= Section4.numPalettes - 1; i++)
                Section4.dataPalette[i].Pal = new S4_PaletteBytes[Palette.MAX_PAL_COLORS];

            // Read the colors of the palette.
            // i = numcolors, j = numpages
            for (iPalette = 0; iPalette <= Section4.numPalettes - 1; iPalette++)
            {
                for (iColor = 0; iColor <= Palette.MAX_PAL_COLORS - 1; iColor++)
                {
                    color = fSection4.ReadUInt16();
                    Section4.dataPalette[iPalette].Pal[iColor].Red = Convert.ToByte((color & 0x1F) * Palette.COEF);
                    Section4.dataPalette[iPalette].Pal[iColor].Green = Convert.ToByte((color >> 5 & 0x1F) * Palette.COEF);
                    Section4.dataPalette[iPalette].Pal[iColor].Blue = Convert.ToByte((color >> 10 & 0x1F) * Palette.COEF);

                    if ((color >> 15 & 1) == 1)
                    {
                        Section4.dataPalette[iPalette].Pal[iColor].Mask = 1;
                    }
                    else
                    {
                        Section4.dataPalette[iPalette].Pal[iColor].Mask = 0;
                    }
                }
            }
        }


        // What is this procedure for?
        // Ok. As we can Repair for example fr_e with new Palettes, we need to update also
        // the base memory data of Section 4 with new data (new Palettes).
        public static void Prepare_Section4()
        {
            byte[] palettesArray;
            int inumPal, inumColor;
            palettesArray = null;

            // Now we will count the size of the array.
            // 4 (PalettesSize) + 2 (PalX) + 2 (PalY) + 2 (NumColors) + 2 (NumPalettes) +
            // Each color has 2 bytes, so we add 512 * num palettes
            palettesArray = new byte[12 + (512 * Section4.numPalettes)];

            // Now we can populate the palettesArray.
            using (var palettesMemory = new MemoryStream(palettesArray))
            {
                using (var paletteWriter = new BinaryWriter(palettesMemory))
                {
                    // First let's put the header of the palettes.
                    paletteWriter.Write(Section4.iPalettesSize);
                    paletteWriter.Write(Section4.PalX);
                    paletteWriter.Write(Section4.PalY);
                    paletteWriter.Write(Section4.numColorsPalette);
                    paletteWriter.Write(Section4.numPalettes);

                    // Palette Data
                    for (inumPal = 0; inumPal < Section4.numPalettes; inumPal++)
                    {
                        // Now we need to convert dataPalette format (M, R, G, B) to 2 bytes integer format.
                        for (inumColor = 0; inumColor < Palette.MAX_PAL_COLORS; inumColor++)
                            paletteWriter.Write(GetUInt16Color(Section4.dataPalette[inumPal].Pal[inumColor]));
                    }
                }
            }

            // Finally we change the Section4 of the loaded field for this one.
            FileTools.Field.fieldSection[3].sectionData = palettesArray;
        }


        // Function to Convert ARGB Color to Paletizzed Image format.
        public static ushort GetUInt16Color(S4_PaletteBytes stColor)
        {
            ushort iColor;

            if (stColor.Mask == 1)
            {
                iColor = Convert.ToUInt16(1 << 15 | stColor.Blue / Palette.COEF << 10 | 
                                                    stColor.Green / Palette.COEF << 5 | 
                                                    stColor.Red / Palette.COEF);
            }
            else
            {
                iColor = Convert.ToUInt16(stColor.Blue / Palette.COEF << 10 | 
                                          stColor.Green / Palette.COEF << 5 | 
                                          stColor.Red / Palette.COEF);
            }

            return iColor;
        }
    }
}