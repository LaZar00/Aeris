using System;

using System.IO;
using System.Windows.Forms;

namespace Aeris
{

    using static FileTools;

    public partial class FrmUnswizzleExternalBaseTextures : Form
    {

        public FrmUnswizzleExternalBaseTextures(FrmAeris inFrmAeris)
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
            try
            {
                FolderBrowserDialogEX fbdEX = new FolderBrowserDialogEX();

                // We must select the directory from where to read the files.
                fbdEX.folderBrowser.Description =
                                "Select the Input folder where are All the Swizzled Base Textures " +
                                "for this field in .PNG format:";

                fbdEX.folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;
                fbdEX.folderBrowser.ShowNewFolderButton = false;

                if (txtInputFolder.Text != "")
                    strGlobalSwizzledBaseInput = txtInputFolder.Text;

                if (strGlobalSwizzledBaseInput != null)
                {
                    fbdEX.folderBrowser.SelectedPath = strGlobalSwizzledBaseInput;
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
                        strGlobalSwizzledBaseInput = fbdEX.folderBrowser.SelectedPath;
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
            try
            {
                FolderBrowserDialogEX fbdEX = new FolderBrowserDialogEX();

                // We must select the directory from where to read the files.
                fbdEX.folderBrowser.Description =
                                "Select the Output folder where to put All the Unswizzled Base Images for " +
                                "this field in .PNG format:";

                fbdEX.folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;
                fbdEX.folderBrowser.ShowNewFolderButton = false;

                if (txtOutputFolder.Text != "") 
                    strGlobalUnswizzledBaseOutput = txtOutputFolder.Text;

                if (strGlobalUnswizzledBaseOutput != null)
                {
                    fbdEX.folderBrowser.SelectedPath = strGlobalUnswizzledBaseOutput;
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
                        strGlobalUnswizzledBaseOutput = fbdEX.folderBrowser.SelectedPath;

                        txtOutputFolder.Text = fbdEX.folderBrowser.SelectedPath;
                    }
                }

                fbdEX.Dispose();
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                MessageBox.Show("Error selecting Output folder.", "Error");
            }
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {

            int iResult;

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

            try
            {
                // Put Global folder for export all base.
                iResult = SwizzleBase.UnswizzleExternalFieldTexturesToBaseImages(txtOutputFolder.Text, 
                                                                                 txtInputFolder.Text,
                                                                                 ref rtbResult);

                switch (iResult)
                {
                    case -4:
                        {
                            MessageBox.Show("There has been an Exception Error when unswizzling the images (BI process).", "Error");
                            break;
                        }
                    case -1:
                        {
                            MessageBox.Show("There has been an Exception Error when unswizzling the images.", "Error");
                            break;
                        }
                    case 2:
                        {
                            MessageBox.Show("There are not any files in that folder.",
                                            "Information");
                            break;
                        }
                    case 3:
                        {
                            // File <field_BI.txt> does not exists.
                            MessageBox.Show("There are not enough External Base Texture files for this field. " +
                                            "You need all the External Base Texture files of the field.",
                                            "Warning");
                            break;
                        }
                    case 4:
                        {
                            // Texture images has not the same proportional scale (multiple of 256 pixels)
                            MessageBox.Show("The scale/proportion of some of the images is different to the other " +
                                            "or it is not a multiple of 256 pixels. All the images must be a " +
                                            "multiple of 256 pixels and must have the same scale.", "Warning");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                MessageBox.Show("Error running the Unswizzle for All External Base Textures.", "Error");
            }
        }


    }
}
