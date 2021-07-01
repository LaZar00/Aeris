using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace Aeris
{
    public static class HashCRC
    {

        public partial struct CRC32List
        {
            public string CRC32;
            public string strFile;
            public int FirstTexture;
            public int FirstPalette;
            public int FirstTileID;
            public int Texture;
            public int Palette;
            public string TileID;
            public int Param;
            public int State;
            public bool Used;
        }

        public static List<CRC32List> lstImagesCRC32 = new List<CRC32List>();
        public static SortedSet<int> HashIDX_PAL_List;

        public static void AddCRC32ToList(string strCRC32, string strUnsFile, int iFirstTexture, int iFirstPalette, int iFirstTileID, int iTexture, int iPalette, int iTileID, int iParam, int iState)
        {
            var CRC32Image = new CRC32List();

            CRC32Image.CRC32 = strCRC32;
            CRC32Image.strFile = strUnsFile;
            CRC32Image.FirstTexture = iFirstTexture;
            CRC32Image.FirstPalette = iFirstPalette;
            CRC32Image.FirstTileID = iFirstTileID;
            CRC32Image.Texture = iTexture;
            CRC32Image.Palette = iPalette;
            CRC32Image.TileID = iTileID.ToString();
            CRC32Image.Param = iParam;
            CRC32Image.State = iState;

            lstImagesCRC32.Add(CRC32Image);
        }

        public static void UpdateCRC32Used(string strFile)
        {
            CRC32List stCRC32;
            int indexCRC32;

            indexCRC32 = lstImagesCRC32.IndexOf(lstImagesCRC32.Single(x => (x.strFile == strFile)));

            stCRC32 = lstImagesCRC32[indexCRC32];
            stCRC32.Used = true;

            lstImagesCRC32[indexCRC32] = stCRC32;
        }

        public static void ResetUsedCRC32()
        {
            CRC32List stCRC32;
            int iCounter;

            for (iCounter = 0; iCounter < lstImagesCRC32.Count; iCounter++)
            {
                stCRC32 = lstImagesCRC32[iCounter];
                stCRC32.Used = false;

                lstImagesCRC32[iCounter] = stCRC32;
            }
        }

        public static void UpdateCRC32List(string strCRC32, string strUnsFile, int iLayer, int iUnsTileID, int iTexture, int iPalette, int iParam, int iState)
        {
            CRC32List stCRC32;
            int indexofCRC32;

            // Let's search the RGBColor to mark as used by its strUnsFile (original).
            indexofCRC32 = lstImagesCRC32.IndexOf(lstImagesCRC32.Single(x => (x.strFile ?? "") == (strUnsFile ?? "")));

            stCRC32 = lstImagesCRC32[indexofCRC32];

            stCRC32.Texture = iTexture;
            stCRC32.Palette = iPalette;

            lstImagesCRC32[indexofCRC32] = stCRC32;
        }


        // This procedure is only for the postprocess method of hashes mismatched.
        // We will replace the strUnsFile with strNewUnsFile
        public static void ReplaceCRC32File(string strFile, string strNewUnsFile)
        {
            CRC32List stCRC32;
            int indexCRC32;

            indexCRC32 = lstImagesCRC32.IndexOf(lstImagesCRC32.Single(x => (x.strFile ?? "") == (strFile ?? "")));

            stCRC32 = lstImagesCRC32[indexCRC32];
            stCRC32.strFile = strNewUnsFile;

            lstImagesCRC32[indexCRC32] = stCRC32;
        }

        public static string CreateHash_IDX_PAL(Bitmap bmpTexture)
        {
            int xTexPos, yTexPos;
            int iColor;
            Color cColor;

            string strHashValue;
            byte[] arrayBytes;

            HashIDX_PAL_List = new SortedSet<int>();

            // Let's put in the Indexed Palette the Colors used for the 32ARGB Texture (without Alpha).
            for (yTexPos = 0; yTexPos < bmpTexture.Height; yTexPos++)
            {
                for (xTexPos = 0; xTexPos < bmpTexture.Width; xTexPos++)
                {
                    cColor = bmpTexture.GetPixel(xTexPos, yTexPos);

                    if (!(cColor.A == 0) |
                          cColor.R == 0 & cColor.G == 0 & cColor.B == 0 |
                          cColor.R == 255 & cColor.G == 255 & cColor.B == 255)
                    {
                        iColor = ColorTranslator.ToWin32(cColor);
                        HashIDX_PAL_List.Add(iColor);
                    }
                }
            }


            arrayBytes = HashIDX_PAL_List.SelectMany(x => Encoding.UTF8.GetBytes(x.ToString())).ToArray();
            //strHashValue = QuickCrc32.ComputeHash(arrayBytes).ToString();
            strHashValue = Crc32.ComputeHash(arrayBytes).ToString();

            return strHashValue;
        }


        // ------------------------------------------------
        // -------------- CRC32 CODE ----------------------
        // ------------------------------------------------
        public static class QuickCrc32
        {
            private static readonly uint[] Table = new uint[256];

            static QuickCrc32()
            {
                const uint poly = 0xedb88320;

                if (Table[1] == 0)
                {
                    for (uint i = 0; i < Table.Length; ++i)
                    {
                        var temp = i;
                        for (var j = 8; j > 0; --j)
                        {
                            if ((temp & 1) == 1)
                                temp = (temp >> 1) ^ poly;
                            else
                                temp >>= 1;
                        }
                        Table[i] = temp;
                    }
                }
            }

            public static uint ComputeHash(byte[] bytes)
            {
                var crc = 0xffffffff;
                foreach (var t in bytes)
                {
                    var index = (byte)((crc & 0xff) ^ t);
                    crc = (crc >> 8) ^ Table[index];
                }
                return ~crc;
            }
        }


        public static class Crc32
        {

            private static readonly int[] crcTab = new int[256];

            public static int ComputeHash(byte[] B)
            {
                int i;
                int crc;

                if (crcTab[1] == 0)
                    CreateLookupTable(crcTab, 32, true, 0x4C11DB7);

                crc = unchecked((int)0xFFFFFFFF);

                for (i = B.GetLowerBound(0); i <= B.GetUpperBound(0); i++)
                    crc = crcTab[(crc ^ B[i]) & 0xFF] ^ (((crc & unchecked((int)0xFFFFFF00)) / 0x100) & 0xFFFFFF);

                return crc ^ unchecked((int)0xFFFFFFFF);
            }

            // -------------- Helper-Functions for Lookup-Table-Generation ----------------------

            private static void CreateLookupTable(int[] crcTab, int BitLen, bool Reflected, int Poly)
            {
                int r;
                int i;
                int V;
                int BM;

                if (BitLen == 32)
                    BM = unchecked((int)0x80000000);
                else
                    BM = (int)Math.Pow(2, BitLen - 1);

                for (V = 0; V <= crcTab.GetUpperBound(0); V++)
                {
                    r = V;

                    if (Reflected)
                        r = Reflect(V, (BitLen < 8) ? BitLen : 8);

                    if (BitLen > 8) r = SHL(r, BitLen - 8);

                    for (i = 0; i < ((BitLen < 8) ? BitLen : 8); i++)
                        r = SHL(r, 1) ^ (Convert.ToBoolean(r & BM) ? Poly : 0);

                    if (Reflected) r = Reflect(r, BitLen);

                    if (BitLen == 32) crcTab[V] = r;
                    else crcTab[V] = (r & (int)Math.Pow(2, (BitLen - 1)));
                }
            }

            private static int Reflect(int V, int Bits)
            {
                int ReflectRet;
                int i;
                int M;

                ReflectRet = V;

                for (i = 0; i < Bits; i++)
                {
                    if (Bits - i - 1 == 31)
                        M = unchecked((int)0x80000000);
                    else
                        M = (int)Math.Pow(2, Bits - i - 1);

                    ReflectRet = Convert.ToBoolean(V & 1) ? (ReflectRet | M) : (ReflectRet & ~M);

                    V = SHR(V, 1);
                }

                return ReflectRet;
            }

            private static int SHL(int V, int Bits)
            {
                int SHLRet;

                if (Bits == 0)
                {
                    SHLRet = V;
                }
                else
                {
                    SHLRet = ((V & ((int)Math.Pow(2, 31 - Bits) - 1)) * (int)Math.Pow(2, Bits)) |
                             (Convert.ToBoolean(V & (int)Math.Pow(2, (31 - Bits))) ? unchecked((int)0x80000000) : 0);
                }

                return SHLRet;
            }

            private static int SHR(int V, int Bits)
            {
                int SHRRet;

                if (V < 0)
                    SHRRet = (((V & 0x7FFFFFFF) / (int)Math.Pow(2, Bits)) | ((int)Math.Pow(2, 31 - Bits)));
                else
                    SHRRet = (V / (int)Math.Pow(2, Bits));

                return SHRRet;
            }
        }
    }
}