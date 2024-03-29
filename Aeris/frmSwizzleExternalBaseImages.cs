﻿using System;

using System.IO;
using System.Windows.Forms;

namespace Aeris
{

    using static FileTools;

    public partial class FrmSwizzleExternalBaseImages : Form
    {

        public FrmSwizzleExternalBaseImages(FrmAeris inFrmAeris)
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
                                "Select the Input folder where are All the Unswizzled Base Images " +
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
                                "Select the Output folder where to put All the Swizzled Base Textures for " +
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
            string strProcessFileName;

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
            strProcessFileName = "";

            try
            {
                iResult = SwizzleBase.SwizzleBaseImagesToFieldTextures(txtInputFolder.Text, 
                                                                       txtOutputFolder.Text, 
                                                                       ref strProcessFileName,
                                                                       ref rtbResult);

                switch (iResult)
                {
                    case -1:
                        {
                            MessageBox.Show("There has been an Exception Error when swizzling the images.", "Error");
                            break;
                        }
                    case 2:
                        {
                            MessageBox.Show("Some of the needed unswizzled files of the Base Images are missing.",
                                            "Information");
                            break;
                        }
                    case 3:
                        {
                            // File <field_BI.txt> does not exists.
                            MessageBox.Show("The file '" + strGlobalFieldName +
                                            "_BI.txt' does not exists in the selected " + "folder.",
                                            "Warning");
                            break;
                        }
                    case 4:
                        {
                            MessageBox.Show("The size of the Unswizzled Base Image of the field does not " +
                                            "correspond to the real size of the field. Maybe you are using " +
                                            "another version of the images or the images have a non " +
                                            "proportional scale.", "Warning");
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                strExceptionVar = ex.Message;
                MessageBox.Show("Error running the Swizzle for All the Base Images.", "Error");
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
