using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;


namespace Aeris
{

    using static S9;

    using static FileTools;

    public class Win32
    {
        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(int dwProcessId);
    }

    public static class CommandLine
    {

        private const int ATTACH_PARENT_PROCESS = -1;
        public static bool bVerbose;

        // iWorkType values;:
        // 1 Swizzle Hash
        // 2 Unsiwzzle Hash
        // 3 Unswizzle All Base Images of the field (internal loaded field)
        // 4 Swizzle All Base Images of the field (external)
        // 5 Export All Base Textures of the field
        public static int iWorkType;
        public static string strInputFolder, strOutputFolder;
        public static bool bCmd;


        //public bool getbCmd() { return bCmd; }

        public static void CommandMessage()
        {
            Console.WriteLine("======================================================================================================");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("===   Aeris Background Tool for Final Fantasy VII v1.0                                             ===");
            Console.WriteLine("===   Author: L@Zar0 (www.clandlan.net)                                                            ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("===   This command line is for easiness of batch processes over a set of files and                 ===");
            Console.WriteLine("===   work with them.                                                                              ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("===  Usage:                                                                                        ===");
            Console.WriteLine("===            aeris -s/-u [field] [inputFolder] [outputFolder] [-v]                               ===");
            Console.WriteLine("===            aeris -d    [field] [outputFolder] [-v]                                             ===");
            Console.WriteLine("===            aeris -b    [field] [inputFolder] [outputFolder] [-v]                               ===");
            Console.WriteLine("===            aeris -x    [field] [outputFolder] [-v]                                             ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("===  [option]:        -s    For Swizzle Hashed files (external folder, Aeris fmt).                 ===");
            Console.WriteLine("===                   -u    For Unswizzle Hashed files (dump of FFNx, to Aeris fmt).               ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("===                   -d    For Unswizzle All Base Images (internal field, Aeris fmt).             ===");
            Console.WriteLine("===                   -b    For Swizzle All Base Images (external folder, Aeris fmt).              ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("===                   -x    For Export All Base Textures (internal field, Aeris fmt).              ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("===  [field]:               Field file PC Uncompressed.                                            ===");
            Console.WriteLine("===  [inputFolder]:         Folder where the input files are.                                      ===");
            Console.WriteLine("===  [outputFolder]:        Folder where we will put the output files.                             ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("===                   -v    Verbose output (Optional).                                             ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("===  MANDATORY: Use FULL PATH paths.                                                               ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("===  Samples:                                                                                      ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine(@"===   aeris -u D:\ff7\fieldsUPC\woa_3 D:\ff7\dumps\woa_3 D:\ff7\fld UNSW\woa_3                     ===");
            Console.WriteLine(@"===   aeris -s ""D:\FF7\fields UPC\datiao_8"" ""D:\FF7\fld UNSW\datiao_8"" ""D:\FF7\fld SWHD\datiao_8""  ===");
            Console.WriteLine(@"===   aeris -d D:\ff7\fieldsUPC\md1_2 D:\ff7\fieldoutput                                           ===");
            Console.WriteLine("===                                                                                                ===");
            Console.WriteLine("======================================================================================================");
        }

        [STAThread]
       public static void Main()
       {
            var strArgs = Environment.GetCommandLineArgs();
          
            strGlobalPath = Application.StartupPath;
            iWorkType = 0;

            if (strArgs.Count() > 1)
            {
                Win32.AttachConsole(ATTACH_PARENT_PROCESS);
                Console.WriteLine("");

                if (strArgs.Count() < 4 | strArgs.Count() > 6)
                {
                    CommandMessage();
                }
                else
                {
                    bCmd = true;
                    ProcessArgs(strArgs);
                }
            }
            else
            {
                bCmd = false;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmAeris());
            }

            // SendKeys.Send("{ENTER}")
            Environment.Exit(0);
        }

        public static void ProcessArgs(string[] strArgs)
        {
            int iResult;

            // Let's check if we want Verbose output.
            if (strArgs[strArgs.Count() - 1].ToLower() == "-v")
            {
                bVerbose = true;
            }
            else
            {
                bVerbose = false;
            }


            // Let's do some checks.
            switch (strArgs[1].ToLower() ?? "")
            {
                case "-s":
                    {
                        iWorkType = 1;
                        if (bVerbose)
                            Console.WriteLine("Preparing for Swizzle Hashed files...");
                        break;
                    }

                case "-u":
                    {
                        iWorkType = 2;
                        if (bVerbose)
                            Console.WriteLine("Preparing for Unswizzle Hashed files...");
                        break;
                    }

                case "-d":
                    {
                        iWorkType = 3;
                        if (bVerbose)
                            Console.WriteLine("Preparing for Unswizzle All Base Images files...");
                        break;
                    }

                case "-b":
                    {
                        iWorkType = 4;
                        if (bVerbose)
                            Console.WriteLine("Preparing for Swizzle All Base Images files...");
                        break;
                    }

                case "-x":
                    {
                        iWorkType = 5;
                        if (bVerbose)
                            Console.WriteLine("Preparing for Export All Base Texture files...");
                        break;
                    }

                default:
                    {
                        CommandMessage();
                        Console.WriteLine("Incorrect option...");
                        return;
                    }
            }

            if (File.Exists(strArgs[2]))
            {
                // Load Field Into Memory.
                strFileFieldName = Path.GetFileName(strArgs[2]);
                strGlobalFieldName = Path.GetFileNameWithoutExtension(strArgs[2]);

                if (bVerbose)
                    Console.WriteLine("Loading PC Uncompressed Field `" +
                                               strGlobalFieldName + "´.");

                // LOAD FIELD
                if (Convert.ToBoolean(LoadFieldCmd(strArgs[2])))
                {
                    if (bVerbose)
                        Console.WriteLine("Field `" + strArgs[2] + "´ LOADED. ");
                }
                else
                {
                    Console.WriteLine("ERROR: Failed the load process of Field `" + strArgs[2] + "´.");
                }
            }
            else
            {
                Console.WriteLine("The Field `" + strArgs[2] + "´ does not exists...");
                return;
            }

            if (iWorkType == 3 | iWorkType == 5)
            {
                if (Directory.Exists(strArgs[3]))
                {
                    strOutputFolder = strArgs[3];

                    if (iWorkType == 3)
                    {
                        // 3 Unswizzle All Base Images of the field (internal loaded field)
                        iResult = UnswizzleAllInternalBaseTextures();

                        if (iResult == 0)
                        {
                            if (bVerbose)
                                Console.WriteLine("Unswizzle process of All Base Images for Field `" +
                                    strGlobalFieldName + "´ DONE.");
                        }
                        else if (iResult == -4)
                        {
                            if (bVerbose)
                                Console.WriteLine("ERROR: There has been some problem reading the EABI Template file for this field.");
                        }
                        else
                        {
                            Console.WriteLine("ERROR: The Unswizzle process of All Base Images for Field `" +
                                strGlobalFieldName + "´ has failed.");
                        }
                    }
                    // 5 Export All Base Textures of the field
                    else if (Export_AllFieldTextures() == 0)
                    {
                        if (bVerbose)
                            Console.WriteLine("Export process of All Base Textures for Field `" +
                                strGlobalFieldName + "´ DONE.");
                    }
                    else
                    {
                        Console.WriteLine("ERROR: The Export process of All Base Textures for Field `" +
                            strGlobalFieldName + "´ has failed.");
                    }
                }
                else
                {
                    Console.WriteLine("The Output Directory `" + strArgs[3] + "´ does not exists...");
                    return;
                }
            }
            else
            {
                if (Directory.Exists(strArgs[3]))
                {
                    strInputFolder = strArgs[3];
                }
                else
                {
                    Console.WriteLine("The Input Directory `" + strArgs[3] + "´ does not exists...");
                    return;
                }

                if (Directory.Exists(strArgs[4]))
                {
                    strOutputFolder = strArgs[4];
                }
                else
                {
                    Console.WriteLine("The Output Directory `" + strArgs[4] + "´ does not exists...");
                    return;
                }

                if (iWorkType == 1)
                {
                    // 1 Swizzle Hash
                    if (SwizzleHashedImages() == 0)
                    {
                        if (bVerbose)
                            Console.WriteLine("Swizzle process for Field `" +
                                strGlobalFieldName + "´ DONE.");
                    }
                    else
                    {
                        Console.WriteLine("ERROR: The Swizzle process for Field `" +
                            strGlobalFieldName + "´ has failed.");
                    }
                }
                else if (iWorkType == 2)
                {
                    // 2 Unsiwzzle Hash
                    if (UnswizzleHashedTextures() == 0)
                    {
                        if (bVerbose)
                            Console.WriteLine("Unswizzle process for Field `" +
                                strGlobalFieldName + "´ DONE.");
                    }
                    else
                    {
                        Console.WriteLine("ERROR: The Unswizzle process for Field `" +
                            strGlobalFieldName + "´ has failed.");
                    }
                }
                // 4 Swizzle All Base Images of the field (external)

                else if (SwizzleAllExternalBaseImages() == 0)
                {
                    if (bVerbose)
                        Console.WriteLine("Swizzle process of All Base Images for Field `" +
                            strGlobalFieldName + "´ DONE.");
                }
                else
                {
                    Console.WriteLine("ERROR: Swizzle process of All Base Images for Field `" +
                        strGlobalFieldName + "´ has failed.");
                }
            }
        }

        public static int LoadFieldCmd(string strFileField)
        {
            try
            {
                Load_Field(strFileField);

                // Prepare ZList.
                Load_ZList(null);

                // Load TileSeparation File if there is any.
                if (File.Exists(strGlobalPath + "\\tileseparation\\" + strGlobalFieldName + ".txt"))
                {
                    SwizzleHash.Read_TileSeparation(strGlobalPath +
                                                    "\\tileseparation\\" +
                                                    strGlobalFieldName + ".txt");

                    if (bVerbose)
                    {
                        Console.WriteLine("Loaded tile separation file.");
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                return 0;
            }
        }

        public static int SwizzleHashedImages()
        {
            // Do Swizzle
            string[] strFoldersTexPalList, strHashedFoldersList;    // Folder's Lists
            string strFolderName;
            string strHash, strFileField, strProcessFileName;
            int iTexture, iPalette, iCounter, iResult;

            DateTime TimeIn, TimeOut;
            TimeSpan TimeDiff, TotalTime;

            iCounter = 0;
            iResult = 0;

            // Steps:
            // 1. Check and Read if so the .txt list of Texture Links
            // 2. Get the iTexture And iPalette from the Folder
            // 3. Get the Hashed Named Folders List
            // 4. Create each hashed texture image
            // 5. Save the Swizzled Texture to output directory


            // 1. Check and Read if so the .txt list of Texture Links
            if (File.Exists(strInputFolder + "\\listTextureLinks.txt"))
            {
                SwizzleHash.Read_TextureLinks(strInputFolder);

                if (bVerbose)
                    Console.WriteLine("INFO: File `listTextureLinks.txt´ FOUND.");
            }
            else if (bVerbose)
                Console.WriteLine("INFO: File `listTextureLinks.txt´ NOT FOUND.");
            try
            {

                // 2. Get the iTexture And iPalette from the Folders
                // We must read the initial set of "texture_palette" folders into memory.
                strFoldersTexPalList = Directory.GetDirectories(strInputFolder);

                strHash = "";
                iTexture = 0;
                iPalette = 0;
                TotalTime = TimeSpan.Zero;

                foreach (var strFolderTexPalPath in strFoldersTexPalList)
                {
                    iResult = GetPathTexturePalette(strFolderTexPalPath, 
                                                    ref strHash, 
                                                    ref iTexture, ref iPalette);

                    strFolderName = strHash;

                    if (iResult == 1)
                    {
                        Console.WriteLine("WARNING: The folder structure is not correct.");
                        return iResult;
                    }

                    if (iPalette > S4.Section4.numPalettes - 1)
                    {
                        Console.WriteLine("WARNING: The folder `" + strFolderName + 
                                          "´ has not the correct Palette.");
                    }
                    else
                    {
                        // 3. Get the Hashed Named Folders List
                        strHashedFoldersList = Directory.GetDirectories(strFolderTexPalPath);
                        foreach (var strHashedFolderPath in strHashedFoldersList)
                        {
                            strHash = strHashedFolderPath.Split(Path.DirectorySeparatorChar).Last();
                            if (strHash.Length < 10 | strHash.Length > 20)
                            {
                                Console.WriteLine("WARNING: The folder `" + strFolderName + 
                                                  "\\" + strHash + "´ maybe is not a hashed folder.");
                            }
                            else
                            {
                                // 4. Create/Cut the hashed texture images
                                strFileField = "";
                                strProcessFileName = "";

                                TimeIn = DateTime.Now;

                                iResult = SwizzleHash.SwizzleHashedImagesBatch(strHashedFolderPath, 
                                                                               strOutputFolder, 
                                                                               strHash, 
                                                                               iTexture, 
                                                                               iPalette, 
                                                                               strFileField, 
                                                                               ref strProcessFileName);

                                TimeOut = DateTime.Now;

                                switch (iResult)
                                {
                                    case 0:
                                        {
                                            if (bVerbose)
                                            {
                                                Console.WriteLine("WARNING: The file `" + strProcessFileName + 
                                                                  "´ is not of the loaded field.");
                                            }

                                            break;
                                        }

                                    case 1:
                                        {
                                            TimeDiff = TimeOut - TimeIn;
                                            TotalTime += TimeDiff;
                                            if (bVerbose)
                                            {
                                                string strPrint = string.Format("DONE: {0,-40}" + 
                                                    "\tDuration: " + TimeDiff.Seconds.ToString("00") + "." +
                                                                     TimeDiff.Milliseconds.ToString("000") + " ms.",
                                                    strHashedFolderPath.Split(Path.DirectorySeparatorChar).Last());

                                                Console.WriteLine(strPrint);
                                            }

                                            iCounter++;
                                            break;
                                        }

                                    case 2:
                                        {
                                            if (bVerbose)
                                            {
                                                Console.WriteLine("There are no files in this folder.");
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                    }
                }

                if (bVerbose)
                {
                    Console.WriteLine("SWIZZLE FINISHED" + "\t\t" +
                                      "Total Duration: " + TotalTime.Minutes.ToString("00") + ":" +
                                                           TotalTime.Seconds.ToString("00") + "." +
                                                           TotalTime.Milliseconds.ToString("000") +
                                      " ms.\t Hashed Folders: " + iCounter.ToString());
                }
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                iResult = -1;
            }

            return iResult;
        }

        public static int UnswizzleHashedTextures()
        {

            // Do Unswizzle
            Bitmap bmpInputTexture;
            string[] strFileList;
            string strOutputFolderMount;
            string strFileName, strFieldName, strUnsFile, strHashExceptionsFile;
            string strHash;
            int iTexture, iPalette, iParam, iState, iTileID, iCounter, iResult;

            DateTime TimeIn, TimeOut;
            TimeSpan TimeDiff, TotalTime;

            bool bIsHashed, bResultT1;
            int iPrevTexture, iPrevPalette;

            List<SwizzleHash.HashExceptions> queryHashExceptionT1List;


            // Let's load HashExceptions list (if exists).
            strHashExceptionsFile = strGlobalPath + "\\hashexceptions\\" + strGlobalFieldName + ".txt";

            if (File.Exists(strHashExceptionsFile))
            {
                SwizzleHash.Read_HashExceptions(strHashExceptionsFile);
                if (bVerbose)
                {
                    Console.WriteLine("INFO: Found Hash Exceptions file for this Field.");
                }
            }
            else
            {
                SwizzleHash.bHashExceptions = false;
            }

            // Vars. Init and Clear.
            SwizzleHash.InitUnswizzledImagesList();
            SwizzleHash.lstTextureLinks.Clear();

            HashCRC.lstImagesCRC32.Clear();

            ImageTools.lstImagesRGBColors.Clear();

            iCounter = 0;
            iResult = 1;

            iPrevTexture = 0;
            iPrevPalette = 0;

            iTexture = 0;
            iPalette = 0;
            iParam = 0;
            iState = 0;
            iTileID = 0;
            TotalTime = TimeSpan.Zero;

            // We must read the hashed files into memory.
            strFileList = Directory.GetFiles(strInputFolder, "*.png", SearchOption.TopDirectoryOnly);
            try
            {
                foreach (var strLongFileName in strFileList)
                {
                    // Steps:
                    // 1. Get the hash part
                    // 2. Create the directory with texture ID + hash name
                    // 3. Export unswizzled textures in the directories

                    // 1. Get the hash part
                    strFieldName = "";
                    strHash = "";
                    strFileName = Path.GetFileNameWithoutExtension(strLongFileName);

                    bIsHashed = SplitFileNameAndCheckHash(strFileName, 
                                                          ref strFieldName,
                                                          ref iTexture,
                                                          ref iPalette,
                                                          ref iParam,
                                                          ref iState,
                                                          ref iTileID,
                                                          ref strHash);


                    // If not hashed will not process the file right now.
                    if (!bIsHashed)
                    {
                        Console.WriteLine("WARNING: The file `" + strFileName + 
                                          ".png´ not seems to be a hashed one.");
                    }
                    // Let's check that the field name of the texture matches the field opened of the tool.
                    else if (!ValidateFilewithField(strFieldName))
                    {
                        Console.WriteLine("WARNING: The file `" + strFileName + 
                                          ".png´ is not of the loaded field.");

                        Console.WriteLine("PROCESS CANCELED.");

                        return 2;
                    }
                    else
                    {
                        // We need to check if we need to avoid this Hash.
                        // Also, we will add to the HashExceptionsProcessable list the hash if it has the
                        // 'Processable' flag.
                        queryHashExceptionT1List = (from itmHashException in SwizzleHash.lstHashExceptions
                                                    where itmHashException.MatchTextureHash == strHash & 
                                                          itmHashException.Texture == iTexture &
                                                          itmHashException.Palette == iPalette &
                                                          itmHashException.VirtualType == 1
                                                    select itmHashException).ToList();

                        // ------------------------------------------------------------------------
                        // Let's work with the Hash Exceptions Processable Type 1 if there is any.
                        bResultT1 = SwizzleHash.ProcessVirtualHashExceptionType1(queryHashExceptionT1List);
                        // ------------------------------------------------------------------------

                        if (bResultT1)
                        {
                            // We need to avoid this Hash.
                            if (bVerbose)
                            {
                                Console.WriteLine("VIRTUAL: The file `" + strFileName + 
                                                  ".png´ will not be processed.");
                            }
                        }
                        else
                        {
                            // 2. Prepare folder name with hash name
                            // First we will Reset Used RGBColors and CRC32 lists.
                            if (iPrevTexture != iTexture | iPrevPalette != iPalette)
                            {
                                HashCRC.ResetUsedCRC32();
                                ImageTools.ResetUsedRGBColors();

                                iPrevTexture = iTexture;
                                iPrevPalette = iPalette;
                            }

                            strOutputFolderMount = strOutputFolder + "\\" + 
                                                   iTexture.ToString("00") + "_" + 
                                                   iPalette.ToString("00") + "\\" + strHash;   // iTexture and iPalette

                            // 3. Export the .png files of the hashed one into the folder.
                            bmpInputTexture = null;

                            ImageTools.ReadBitmap(ref bmpInputTexture, strLongFileName);

                            strUnsFile = "";

                            TimeIn = DateTime.Now;
                            
                            iResult = SwizzleHash.UnswizzleHashedTextureBatch(strOutputFolderMount, 
                                                                              strFieldName,
                                                                              strHash, 
                                                                              iTexture, 
                                                                              iPalette,
                                                                              bmpInputTexture, 
                                                                              ref strUnsFile,
                                                                              strOutputFolder);

                            TimeOut = DateTime.Now;

                            switch (iResult)
                            {
                                case 1:
                                    {
                                        Console.WriteLine("ERROR processing file: " + strFileName + ".png");

                                        return 2;
                                    }

                                case 3:
                                    {
                                        // This case is for fields that some parameter (texture/palette/...) is not ok,
                                        // like field 'lastmap' which has palette 8, but it does not exists.
                                        Console.WriteLine("JUMP: Parameter not correct for file: " + 
                                                          strFileName + ".png");

                                        strUnsFile = "";
                                        break;
                                    }
                            }


                            // bParamDraw - We have drawn the parameter for the textures with a given palette
                            if (!string.IsNullOrEmpty(strUnsFile))
                            {
                                // Save Image to .PNG format.

                                strFieldName = strFieldName + "_" +
                                               iTexture.ToString("00") + "_" +
                                               strUnsFile;

                                TimeDiff = TimeOut - TimeIn;
                                TotalTime += TimeDiff;

                                if (bVerbose)
                                {
                                    string strPrint = string.Format("DONE: {0,-40}\t" +
                                                                    "Duration: " + TimeDiff.Seconds.ToString("00") + "." +
                                                                                   TimeDiff.Milliseconds.ToString("000") + " ms.", strFileName + ".png");
                                    Console.WriteLine(strPrint);
                                }

                                iCounter++;
                            }

                            bmpInputTexture.Dispose();
                        }
                    }
                }

                if (bVerbose)
                {
                    Console.WriteLine("UNSWIZZLE FINISHED\t\t" + "Total Duration: " + 
                                      TotalTime.Minutes.ToString("00") + ":" +
                                      TotalTime.Seconds.ToString("00") + "." +
                                      TotalTime.Milliseconds.ToString("000") + " ms.\t" +
                                      "Files: " + iCounter.ToString());
                }

                // Write list of Texture links if there is any data.
                SwizzleHash.Write_TextureLinks(strOutputFolder);

                iResult = 0;
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                iResult = -1;
            }

            return iResult;
        }

        public static int UnswizzleAllInternalBaseTextures()
        {
            int iResult;

            try
            {
                iResult = SwizzleBase.UnswizzleFieldTexturesToBaseImages(strOutputFolder);
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                iResult = -1;
            }

            return iResult;
        }

        public static int Export_AllFieldTextures()
        {
            int iResult;
            string strTextureFile;
            int iNumTexture;

            iResult = 0;

            try
            {
                for (iNumTexture = 0; iNumTexture < MAX_NUM_TEXTURES; iNumTexture++)
                {
                    if (textureImage[iNumTexture] != null)
                    {
                        strTextureFile = strOutputFolder + "\\" +
                                         strGlobalFieldName + "_" + 
                                         iNumTexture.ToString("00") + "_00.png";

                        ImageTools.WriteBitmap(textureImage[iNumTexture].Bitmap,
                                               strTextureFile);
                    }
                }
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                iResult = -1;
            }

            return iResult;
        }

        public static int SwizzleAllExternalBaseImages()
        {
            int iResult;
            string strProcessFileName;
            RichTextBox rtbresult = null;

            strProcessFileName = "";

            try
            {
                iResult = SwizzleBase.SwizzleBaseImagesToFieldTextures(strInputFolder,
                                                                       strOutputFolder,
                                                                       ref strProcessFileName,
                                                                       ref rtbresult);

                if (bVerbose)
                {
                    switch (iResult)
                    {
                        case -1:
                            {
                                Console.WriteLine("There has been an Exception Error when swizzling "+
                                                  "the images.");
                                break;
                            }
                        case 2:
                            {
                                Console.WriteLine("Some of the needed unswizzled files of the Base Images are missing.");
                                break;
                            }
                        case 3:
                            {
                                Console.WriteLine("The file '" + strGlobalFieldName +
                                                  "_BI.txt' does not exists in the selected " + "folder.");
                                break;
                            }
                        case 4:
                            {
                                Console.WriteLine("The size of the Unswizzled Base Image of the field does not " +
                                                  "correspond to the real size of the field. Maybe you are using " +
                                                  "another version of the images.");
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                iResult = -1;
            }

            return iResult;
        }
    }
}