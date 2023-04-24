using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Aeris
{

    using static FileTools;

    public partial class FrmUnswizzleHashesBatch : Form
    {

        public FrmUnswizzleHashesBatch(FrmAeris inFrmAeris)
        {
            InitializeComponent();

            this.Owner = inFrmAeris;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnInputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogEX fbdEX = new FolderBrowserDialogEX();

            try
            {
                // We must select the directory from where to read the files.
                fbdEX.folderBrowser.Description = 
                                "Select Input Folder where the Swizzled Textures are:";

                fbdEX.folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;
                fbdEX.folderBrowser.ShowNewFolderButton = false;

                if (strGlobalUnswizzledBatchInput != null)
                {
                    fbdEX.folderBrowser.SelectedPath = strGlobalUnswizzledBatchInput;
                }
                else
                {
                    fbdEX.folderBrowser.SelectedPath = strGlobalPath;
                }

                fbdEX.Tmr.Start();
                if (fbdEX.folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (fbdEX.folderBrowser.SelectedPath != "")
                    {
                        // Put Global folder for input swizzled.
                        strGlobalSwizzledBatchInput = fbdEX.folderBrowser.SelectedPath;

                        txtInputFolder.Text = fbdEX.folderBrowser.SelectedPath;
                    }
                }

                fbdEX.Dispose();
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                MessageBox.Show("Error selecting Input folder.", "Error");
            }
        }

        private void BtnOutputFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogEX fbdEX = new FolderBrowserDialogEX();

            try
            {
                // We must select the directory from where to read the files.
                fbdEX.folderBrowser.Description = 
                                "Select Output Folder where to put Unswizzled Textures:";

                fbdEX.folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;

                if (strGlobalUnswizzledBatchOutput != null)
                {
                    fbdEX.folderBrowser.SelectedPath = strGlobalUnswizzledBatchOutput;
                }
                else
                {
                    fbdEX.folderBrowser.SelectedPath = strGlobalPath;
                }

                fbdEX.Tmr.Start();
                if (fbdEX.folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (fbdEX.folderBrowser.SelectedPath != "")
                    {
                        // Put Global folder for output unswizzled.
                        strGlobalUnswizzledBatchOutput = fbdEX.folderBrowser.SelectedPath;

                        txtOutputFolder.Text = fbdEX.folderBrowser.SelectedPath;
                    }
                }

                fbdEX.Dispose();
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                MessageBox.Show("Error selecting Output Folder.", "Error");
            }
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            Bitmap bmpInputTexture;
            string[] strFileList;
            string strFileName, strFieldName, strUnsFile, strHashExceptionsFile;
            string strHash, strOutputFolder;
            int iTexture, iPalette, iParam, iState, iTileID, iCounter, iResult;

            DateTime TimeIn, TimeOut;
            TimeSpan TimeDiff, TotalTime;

            bool bIsHashed, bResultT1;
            int iPrevTexture, iPrevPalette;

            List<SwizzleHash.HashExceptions> queryHashExceptionT1List;

            if (txtInputFolder.Text.Length == 0 | txtOutputFolder.Text.Length == 0)
            {
                MessageBox.Show("One of the folders is not selected.", "Information");
                return;
            }

            if (!Directory.Exists(txtInputFolder.Text) | !Directory.Exists(txtOutputFolder.Text))
            {
                MessageBox.Show("One of the folders does not exists.", "Information");
                return;
            }


            // Let's load HashExceptions list (if exists).
            strHashExceptionsFile = strGlobalPath + "\\hashexceptions\\" + 
                                    strGlobalFieldName + ".txt";

            // Clear events
            logEvents.ClearEvents(rtbResult);

            if (File.Exists(strHashExceptionsFile))
            {
                SwizzleHash.Read_HashExceptions(strHashExceptionsFile);

                logEvents.AddEventText("INFO: Found Hash Exceptions file for this Field.", rtbResult);
            }
            else
            {
                SwizzleHash.bHashExceptions = false;
            }

            try
            {
                // Vars. Init and Clear.
                SwizzleHash.InitUnswizzledImagesList();
                SwizzleHash.lstTextureLinks.Clear();

                HashCRC.lstImagesCRC32.Clear();

                ImageTools.lstImagesRGBColors.Clear();

                iCounter = 0;
                iResult = 0;

                iPrevTexture = 0;
                iPrevPalette = 0;

                iTexture = 0;
                iPalette = 0;
                iParam = 0;
                iState = 0;
                iTileID = 0;
                TotalTime = TimeSpan.Zero;

                // We must read the hashed files into memory.
                strFileList = Directory.GetFiles(txtInputFolder.Text, "*.png", SearchOption.TopDirectoryOnly);

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
                        logEvents.AddEventText("WARNING: The file '" + strFileName + 
                                               ".png' not seems to be a hashed one.", rtbResult);
                    }
                    // Let's check that the field name of the texture matches the field opened of the tool.
                    else if (!ValidateFilewithField(strFieldName))
                    {
                        logEvents.AddEventText("WARNING: The file '" + strFileName + 
                                               ".png' is not of the loaded field.", rtbResult);

                        logEvents.AddEventText("PROCESS CANCELED.", rtbResult);

                        return;
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
                            // We need to treat this Hash as Virtual non processed.
                            logEvents.AddEventText("VIRTUAL: The file '" + strFileName + 
                                                   ".png' will not be processed.", rtbResult);
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

                            strOutputFolder = txtOutputFolder.Text + "\\" + 
                                              iTexture.ToString("00") + "_" + 
                                              iPalette.ToString("00") + "\\" + strHash;   // iTexture and iPalette

                            // 3. Export the .png files of the hashed one into the folder.
                            bmpInputTexture = null;

                            ImageTools.ReadBitmap(ref bmpInputTexture, strLongFileName);

                            strUnsFile = "";

                            TimeIn = DateTime.Now;

                            iResult = SwizzleHash.UnswizzleHashedTextureBatch(strOutputFolder, 
                                                                              strFieldName, 
                                                                              strHash,
                                                                              iTexture, 
                                                                              iPalette, 
                                                                              bmpInputTexture, 
                                                                              ref strUnsFile,
                                                                              txtOutputFolder.Text);

                            TimeOut = DateTime.Now;
                            
                            switch (iResult)
                            {
                                case 1:
                                    {
                                        logEvents.AddEventText("Exception ERROR processing file: " + 
                                                               strFileName + ".png", rtbResult);

                                        return;
                                    }

                                case 3:
                                    {
                                        // This case is for fields that some parameter (texture/palette/...) is not ok,
                                        // like field 'lastmap' which has palette 8, but it does not exists.
                                        logEvents.AddEventText("JUMP: Parameter not correct for file: " + 
                                                               strFileName + ".png", rtbResult);

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

                                logEvents.AddEventText("DONE: " + strFileName + ".png" + "\t\tDuration: " + 
                                                       TimeDiff.Seconds.ToString("00") + "." + 
                                                       TimeDiff.Milliseconds.ToString("000") + " ms.",
                                                       rtbResult);

                                iCounter++;
                            }

                            bmpInputTexture.Dispose();
                        }
                    }
                }

                logEvents.AddEventText("UNSWIZZLE FINISHED" + "\t\tTotal Duration: " + 
                                       TotalTime.Minutes.ToString("00") + ":" + 
                                       TotalTime.Seconds.ToString("00") + "." + 
                                       TotalTime.Milliseconds.ToString("000") + " ms." + 
                                       "\tFiles: " + iCounter.ToString(), rtbResult);


                // Write list of Texture links if there is any data.
                SwizzleHash.Write_TextureLinks(txtOutputFolder.Text);
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                MessageBox.Show("Error running the Unswizzle process.", "Error");
            }
        }

        private void RtbResult_TextChanged(object sender, EventArgs e)
        {
            var rtbResult = sender as RichTextBox;

            rtbResult.SelectionStart = rtbResult.Text.Length;

            // Scroll it automatically
            rtbResult.ScrollToCaret();
        }
    }
}
