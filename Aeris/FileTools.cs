using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Aeris
{
    public static class FileTools
    {

        public partial struct STRUCT_FIELDHDR
        {
            public ushort blank;
            public int numSections;
            public int[] offsetSection;
        }

        public partial struct STRUCT_SECTION
        {
            public int sectionSize;
            public byte[] sectionData;
        }

        public partial struct STRUCT_FIELD
        {
            public STRUCT_FIELDHDR fieldHDR;
            public STRUCT_SECTION[] fieldSection;
            public byte[] tagFF7;
        }

        public static STRUCT_FIELD Field;

        public static string strGlobalPath, strGlobalFieldName, strFileFieldName, strGlobalPathFieldFolder, strGlobalPathSaveFieldFolder;

        public static string strGlobalUnswizzledBatchInput, strGlobalUnswizzledBatchOutput;
        public static string strGlobalSwizzledBatchInput, strGlobalSwizzledBatchOutput;

        public static string strGlobalExportAllTextures, strGlobalImportTexture;
        public static string strGlobalLoadUnswizzleExternal, strGlobalLoadSwizzleExernal, strGlobalLoadSwizzleExternalFolder;
        public static string strGlobalSaveUnswizzleExternal, strGlobalSaveSwizzleExternal;

        public static string strGlobalUnswizzleAllBaseTextures, strGlobalSwizzledBaseInput, strGlobalUnswizzledBaseOutput;

        public static bool bFieldModified;


        // Files we can have:
        // Base files:           woa_3_00_00.png                     (-woa_3-,    tex -00-, pal -00-)
        // (Swizzled)
        // Base Hashed files:    woa_3_15_03_954d5794353d1975.png    (-woa_3-,    tex -15-, pal -03-, hash -954d5794353d1975-)
        // (Swizzled)
        // Treated Hashed files: datiao_8_15_05_02_00.png            (-datiao_8-, tex -15-, pal -05-, param -02-, state -00-)
        // (Unswizzled)

        public static bool SplitFileNameAndCheckHash(string strLongFileName, ref string strFieldName, ref int iTexture, ref int iPalette, ref int iParam, ref int iState, ref int iTileID, ref string strHash)
        {
            string[] strFileSplit;
            bool bIsHashed;
            bIsHashed = false;
            strFileSplit = strLongFileName.Split('_');


            // Let's check if loaded field has "-" or not and we put the field name
            if (strGlobalFieldName.Split('_').Count() > 1)
            {
                strFieldName = strFileSplit[0] + "_" + strFileSplit[1];
            }
            else
            {
                strFieldName = strFileSplit[0];
            }


            // Let's put the other params
            if (strFileSplit[strFileSplit.Count() - 1].Length < 12)
            {
                // Not hashed
                // We need to know if is Base or Treated
                if (strFileSplit.Count() < 6)
                {
                    // Base
                    iTexture = Int32.Parse(strFileSplit[strFileSplit.Count() - 2]);
                    iPalette = Int32.Parse(strFileSplit[strFileSplit.Count() - 1]);
                }
                else
                {
                    // Treated Hash
                    iTexture = Int32.Parse(strFileSplit[strFileSplit.Count() - 5]);
                    iPalette = Int32.Parse(strFileSplit[strFileSplit.Count() - 4]);
                    iParam = Int32.Parse(strFileSplit[strFileSplit.Count() - 3]);
                    iState = Int32.Parse(strFileSplit[strFileSplit.Count() - 2]);
                    iTileID = Int32.Parse(strFileSplit[strFileSplit.Count() - 1]);
                    bIsHashed = true;
                }
            }
            else
            {
                // Hashed
                iTexture = Int32.Parse(strFileSplit[strFileSplit.Count() - 3]);
                iPalette = Int32.Parse(strFileSplit[strFileSplit.Count() - 2]);
                strHash = strFileSplit[strFileSplit.Count() - 1];
                bIsHashed = true;
            }

            return bIsHashed;
        }

        public static bool ValidateFilewithField(string strField)
        {
            return strField == strGlobalFieldName;
        }

        public static int GetPathTexturePalette(string strHashedUnsTexFile, ref string strHash, ref int iSwizzledPathTexture, ref int iSwizzledPathPalette)
        {
            int iResult;
            string[] strPathFolder;

            iResult = 1;

            strPathFolder = strHashedUnsTexFile.Split(Path.DirectorySeparatorChar);

            try
            {
                if (strPathFolder.Count() > 0)
                {
                    if (strPathFolder[strPathFolder.Count() - 1].Length > 5)
                    {
                        strHash = strPathFolder[strPathFolder.Count() - 1];
                        iSwizzledPathTexture = Int32.Parse(strPathFolder[strPathFolder.Count() - 2].Split('_')[0]);
                        iSwizzledPathPalette = Int32.Parse(strPathFolder[strPathFolder.Count() - 2].Split('_')[1]);
                    }
                    else
                    {
                        strHash = "";
                        iSwizzledPathTexture = Int32.Parse(strPathFolder.Last().Split('_')[0]);
                        iSwizzledPathPalette = Int32.Parse(strPathFolder.Last().Split('_')[1]);
                    }

                    iResult = 0;
                }
            }
            catch (Exception ex)
            {
                iResult = -1;
            }

            return iResult;
        }


        // Procedure to clear the memory data of the field.
        public static void Clear_MemoryFieldData()
        {
            int i;

            // Clear FF7 Tag.
                if (Field.tagFF7 != null) Field.tagFF7 = null;

            // Clear Sections Data
            if (Field.fieldSection != null)
            {

                for (i = 0; i < Field.fieldHDR.numSections; i++)
                {
                    if (Field.fieldSection[i].sectionData != null)
                    {
                        Field.fieldSection[i].sectionData = null;
                        Field.fieldSection[i].sectionSize = 0;
                    }
                }

                Field.fieldSection = null;
            }

            // Clear Field HDR Data
            if (Field.fieldHDR.offsetSection != null)
                for (i = 0; i < Field.fieldHDR.numSections; i++)
                    Field.fieldHDR.offsetSection[i] = 0;

            Field.fieldHDR.numSections = 0;
        }


        // As we will have the feature to Save the Field, we will load the different sections of the
        // Field (1-3 and 5-8) into memory.
        // Sections 4 and 9 will be treated directly to work with them.
        public static int Load_Field(string strFileName)
        {
            int iResult, i;
            byte[] fieldArray;
            iResult = 0;
            try
            {
                // We will clear first the memory data of the previous field if any.
                fieldArray = null;

                Clear_MemoryFieldData();

                fieldArray = File.ReadAllBytes(strFileName);

                using (var fieldMemory = new MemoryStream(fieldArray))
                {
                    using (var fieldReader = new BinaryReader(fieldMemory))
                    {
                        // Let's read the Field and put it in memory.
                        // First we read the field header.
                        Field.fieldHDR.blank = fieldReader.ReadUInt16();
                        Field.fieldHDR.numSections = (int)fieldReader.ReadUInt32();
                        Field.fieldHDR.offsetSection = new int[Field.fieldHDR.numSections];

                        for (i = 0; i <= Field.fieldHDR.numSections - 1; i++)
                            Field.fieldHDR.offsetSection[i] = (int)fieldReader.ReadUInt32();

                        // Second we will read each of the sections and put it in memory.
                        Field.fieldSection = new STRUCT_SECTION[Field.fieldHDR.numSections];

                        for (i = 0; i <= Field.fieldHDR.numSections - 1; i++)
                        {
                            Field.fieldSection[i].sectionSize = (int)fieldReader.ReadUInt32();
                            Field.fieldSection[i].sectionData = new byte[Field.fieldSection[i].sectionSize];
                            Field.fieldSection[i].sectionData = fieldReader.ReadBytes(Field.fieldSection[i].sectionSize);
                        }

                        // We prepare the "FINAL FANTASY7" Tag for the Field.
                        // NOT all the fields have this ending tag.
                        Field.tagFF7 = Encoding.ASCII.GetBytes("FINAL FANTASY7");
                    }
                }

                if (iResult == 0)
                {
                    // Now we need to prepare for Aeris Section 4 (Palettes)...
                    using (var sectionMemory = new MemoryStream(Field.fieldSection[3].sectionData))
                    {
                        using (var sectionReader = new BinaryReader(sectionMemory))
                        {
                            S4.Load_Section4(sectionReader);
                        }
                    }

                    // and Section 9 (Tiles/Textures) in its Structures.
                    using (var sectionMemory = new MemoryStream(Field.fieldSection[8].sectionData))
                    {
                        using (var sectionReader = new BinaryReader(sectionMemory))
                        {
                            S9.Load_Section9(sectionReader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                iResult = -1;
            }

            return iResult;
        }

        public static int Save_Field(string strFileName)
        {
            // Here we will save the field from the memory data we have of it.
            // For doing this, before saving the field into disk, we will create it into memory.
            // Some steps:
            // --- Preparations
            // - Prepare Section 4 (Palettes) from Section4 structure.
            // - Prepare Section 9 (Tiles/Textures) from Section9 structure.
            // - Prepare/Update Field HDR with the known Sections in memory plus the
            // new Section 4 and Section 9 in case we have edited them.
            // --- Saving the Array
            // - First of we need to Redimension the Byta Array to the size of the Field. 
            // - Put Field HDR in memory Byte Array.
            // - Put Each Section in memory Byte Array.
            // - Put FF7Tag at the end of Byte Array.
            // - Write Field in memory to file.

            byte[] fieldSaveArray;
            int iResult, i;
            iResult = 0;
            fieldSaveArray = null;
            try
            {

                // - Prepare/Update Field HDR with the known Sections in memory plus the
                // new Section 4 and Section 9 in case we have edited them.
                S4.Prepare_Section4();
                S9.Prepare_Section9();

                // Now let's Update/Prepare Field HDR with the info
                // (maybe we have updated Section 4 And/Or Section 9).
                Update_FieldHDR();

                // To put in memory array the file, first we need to calculate the total size
                // of the Array (all the field).
                Redimension_fieldSaveArray(ref fieldSaveArray);

                // Let's prepare the Memory Writer
                using (var fieldSaveMemory = new MemoryStream(fieldSaveArray))
                {
                    using (var fieldSaveWriter = new BinaryWriter(fieldSaveMemory))
                    {

                        // - Put Field HDR in memory Byte Array.
                        fieldSaveWriter.Write(Field.fieldHDR.blank);
                        fieldSaveWriter.Write(Field.fieldHDR.numSections);

                        for (i = 0; i < Field.fieldHDR.numSections; i++)
                            fieldSaveWriter.Write(Field.fieldHDR.offsetSection[i]);

                        // - Put Each Section in memory Byte Array.
                        for (i = 0; i < Field.fieldHDR.numSections; i++)
                        {
                            fieldSaveWriter.Write(Field.fieldSection[i].sectionSize);
                            fieldSaveWriter.Write(Field.fieldSection[i].sectionData);
                        }

                        // - Put FF7Tag at the end of Byte Array.
                        fieldSaveWriter.Write(Field.tagFF7);
                    }
                }


                // - Write Field in memory to file.
                using (var fieldSaveWriter = new BinaryWriter(File.Open(strFileName, FileMode.Create)))
                {
                    fieldSaveWriter.Write(fieldSaveArray);
                }
            }
            catch (Exception ex)
            {
                iResult = -1;
            }

            return iResult;
        }

        public static void Redimension_fieldSaveArray(ref byte[] fieldSaveArray)
        {
            // First we will calculate the size in an integer var.
            int iByteDimension;

            // The initial Field Array size for Field HDR is the struct STRUCT_FIELDHDR
            // This would be 2 + 4 + numSections * 4
            iByteDimension = 6 + Field.fieldHDR.numSections * 4;

            // Then we calculate the sizes of the Sections for Byte Array and redimension it.
            for (int i = 0; i < Field.fieldHDR.numSections; i++)
                iByteDimension = iByteDimension + 4 + Field.fieldSection[i].sectionSize;

            // Finally redimension the Byte Array for the FF7 Tag.
            iByteDimension = iByteDimension + 14;
            fieldSaveArray = new byte[iByteDimension];
        }

        public static void Update_FieldHDR()
        {
            int i;
            int offsetCounter;

            // Initial offset for Section 0 (or Section 1, depending if you see it as 1-9)
            offsetCounter = 0x2A;

            // We need to update the offsets of each section in case Section 4 (Palette) has been
            // changed, like in case of 'fr_e' Field.

            for (i = 0; i < Field.fieldHDR.numSections; i++)
            {
                Field.fieldSection[i].sectionSize = Field.fieldSection[i].sectionData.Length;

                if (i > 0)
                    offsetCounter = offsetCounter + Field.fieldSection[i - 1].sectionData.Length + 4;

                Field.fieldHDR.offsetSection[i] = offsetCounter;
            }
        }
    }
}