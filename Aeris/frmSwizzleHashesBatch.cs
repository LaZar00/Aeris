using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace Aeris
{

    using static FileTools;

    public partial class FrmSwizzleHashesBatch : Form
    {

        private readonly FrmAeris frmAeris;

        public FrmSwizzleHashesBatch(FrmAeris inFrmAeris)
        {
            InitializeComponent();

            this.frmAeris = inFrmAeris;
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
                                "Select Input Folder where the Unswizzled Textures are:";

                fbdEX.folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;
                fbdEX.folderBrowser.ShowNewFolderButton = false;

                if (strGlobalSwizzledBatchInput != null)
                {
                    fbdEX.folderBrowser.SelectedPath = strGlobalSwizzledBatchInput;
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
                        // Put Global folder for input unswizzled.
                        strGlobalUnswizzledBatchInput = fbdEX.folderBrowser.SelectedPath;

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
                                "Select Output Folder where to put the Swizzled Textures:";

                fbdEX.folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;

                if (strGlobalSwizzledBatchOutput != null)
                {
                    fbdEX.folderBrowser.SelectedPath = strGlobalSwizzledBatchOutput;
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
                        // Put Global folder for input unswizzled.
                        strGlobalSwizzledBatchOutput = fbdEX.folderBrowser.SelectedPath;

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
            string[] strFoldersTexPalList, strHashedFoldersList;    // Folder's Lists
            string strFolderName;
            string strHash, strFileField, strProcessFileName;
            int iTexture, iPalette, iCounter, iResult;
            DateTime TimeIn, TimeOut;
            TimeSpan TimeDiff, TotalTime;

            iTexture = 0;
            iPalette = 0;
            TotalTime = TimeSpan.Zero;

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


            // Initial Vars
            // Clear the results RTB
            logEvents.ClearEvents(rtbResult);
            iCounter = 0;


            // Steps:
            // 1. Check and Read if so the .txt list of Texture Links
            // 2. Get the iTexture And iPalette from the Folder
            // 3. Get the Hashed Named Folders List
            // 4. Create each hashed texture image
            // 5. Save the Swizzled Texture to output directory


            // 1. Check and Read if so the .txt list of Texture Links
            if (File.Exists(txtInputFolder.Text + "\\listTextureLinks.txt"))
            {
                SwizzleHash.Read_TextureLinks(txtInputFolder.Text);
                logEvents.AddEventText("INFO: File 'listTextureLinks.txt' FOUND.", rtbResult);
            }
            else
            {
                logEvents.AddEventText("INFO: File 'listTextureLinks.txt' NOT FOUND.", rtbResult);
            }

            try
            {

                // 2. Get the iTexture And iPalette from the Folders
                // We must read the initial set of "texture_palette" folders into memory.
                strFoldersTexPalList = Directory.GetDirectories(txtInputFolder.Text);
                strHash = "";
                foreach (var strFolderTexPalPath in strFoldersTexPalList)
                {
                    iResult = GetPathTexturePalette(strFolderTexPalPath, 
                                                    ref strHash, 
                                                    ref iTexture, 
                                                    ref iPalette);

                    strFolderName = strHash;

                    if (iResult == 1)
                    {
                        MessageBox.Show("The folder structure is not correct.", "Information");
                        return;
                    }

                    if (iPalette > frmAeris.cbPalettes.Items.Count - 1)
                    {
                        logEvents.AddEventText("WARNING: The folder '" + strFolderName +
                                               "'has not the correct Palette.", rtbResult);
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
                                logEvents.AddEventText("WARNING: The folder '" + strFolderName + "\\" + 
                                                        strHash + "' maybe is not a hashed folder.", rtbResult);
                            }
                            else
                            {
                                // 4. Create/Cut the hashed texture images
                                strFileField = "";
                                strProcessFileName = "";

                                TimeIn = DateTime.Now;
                                iResult = SwizzleHash.SwizzleHashedImagesBatch(strHashedFolderPath, 
                                                                               txtOutputFolder.Text, 
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
                                            logEvents.AddEventText("WARNING: The file '" + strProcessFileName + 
                                                                   "' is not of the loaded field.", rtbResult);
                                            break;
                                        }
                                    case 1:
                                        {
                                            TimeDiff = TimeOut - TimeIn;
                                            TotalTime += TimeDiff;
                                            logEvents.AddEventText("DONE: " + 
                                                        strHashedFolderPath.Split(Path.DirectorySeparatorChar).Last() + 
                                                        "\tDuration: " + 
                                                        TimeDiff.Seconds.ToString("00") + "." +                                                                          
                                                        TimeDiff.Milliseconds.ToString("000") + " ms.",
                                                        rtbResult);

                                            iCounter++;
                                            break;
                                        }
                                    case 2:
                                        {
                                            MessageBox.Show("There are no files in this folder.", "Information");
                                            break;
                                        }
                                    case 4:
                                        {
                                            logEvents.AddEventText("WARNING: The file '" + strProcessFileName + 
                                                                   "' has not a proportional Scale Factor for this field.", rtbResult);
                                            break;
                                        }
                                }
                            }
                        }
                    }
                }

                logEvents.AddEventText("SWIZZLE FINISHED" + 
                                       "\t\tTotal Duration: " + TotalTime.Minutes.ToString("00") + ":" +
                                       TotalTime.Seconds.ToString("00") + "." +
                                       TotalTime.Milliseconds.ToString("000") + " ms." +
                                       "\tHashed Folders: " + iCounter.ToString(),
                                       rtbResult);
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                MessageBox.Show("Error running the Swizzle process.", "Error");
            }
        }

        private void RtbResult_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rtbResult = sender as RichTextBox;

            rtbResult.SelectionStart = rtbResult.Text.Length;

            // Scroll it automatically
            rtbResult.ScrollToCaret();
        }
    }
}
