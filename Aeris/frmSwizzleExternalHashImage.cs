using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Aeris
{

    public partial class frmSwizzleExternalHashImage : Form
    {

        private frmAeris frmAeris;

        public frmSwizzleExternalHashImage(frmAeris frmAeris)
        {
            InitializeComponent();

            this.frmAeris = frmAeris;
        }

        public Bitmap bmpUnsImage;
        public int iSwizzleTexTileID, iSwizzleTexParam, iSwizzleTexState, iSwizzleTexPalette, iSwizzleTexTexture;
        public string strSwizzleTexFileField, strSwizzleTexFileName;
        public int iSwizzleTexWidth, iSwizzleTexHeight, iSwizzleTexDrawPosX, iSwizzleTexDrawPosY;
        public bool bHashedTexture;
        public string strHash;

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnLoadTexture_Click(object sender, EventArgs e)
        {
            int iScaleFactor;


            // Set filter options and filter index.
            ofdTexture.Title = "Open Unswizzled Hashed Image";
            ofdTexture.Filter = "PNG files (*.png)|*.png";
            ofdTexture.FilterIndex = 1;
            ofdTexture.FileName = null;

            if (FileTools.strGlobalLoadSwizzleExernal != null)
                ofdTexture.InitialDirectory = FileTools.strGlobalLoadSwizzleExernal;
            else
                ofdTexture.InitialDirectory = FileTools.strGlobalPath;

            try
            {
                // Process input if the user clicked OK.
                if (ofdTexture.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(ofdTexture.FileName))
                    {
                        // Initialize Vars.
                        lbHash.Visible = false;
                        txtHash.Visible = false;
                        txtTextureID.Text = "";
                        txtParam.Text = "";
                        txtState.Text = "";
                        txtTextureWidth.Text = "";
                        txtTextureHeight.Text = "";
                        txtScaleFactor.Text = "";
                        txtHash.Text = "";
                        strHash = "";

                        iScaleFactor = 0;

                        // Get file name data.
                        strSwizzleTexFileName = Path.GetFileNameWithoutExtension(ofdTexture.FileName);


                        // Let's check if loaded fields equals field of texture.
                        bHashedTexture = FileTools.SplitFileNameAndCheckHash(strSwizzleTexFileName,
                                                                             ref strSwizzleTexFileField,
                                                                             ref iSwizzleTexTexture, ref iSwizzleTexPalette,
                                                                             ref iSwizzleTexParam, ref iSwizzleTexState,
                                                                             ref iSwizzleTexTileID, ref strHash);

                        if (!FileTools.ValidateFilewithField(strSwizzleTexFileField))
                        {
                            MessageBox.Show("WARNING: " + "File: " + strSwizzleTexFileName +
                                            ".png is not from the loaded field in the tool.",
                                            "Warning");
                            return;
                        }


                        // Load Texture
                        ImageTools.ReadBitmap(ref bmpUnsImage, ofdTexture.FileName);

                        //iScaleFactor = bmpLoadedTexture.Width / S9.iLayersMaxWidth;
                        int iLayer = (from itmZList in S9.Section9Z
                                      where itmZList.ZTexture == iSwizzleTexTexture &
                                            itmZList.ZPalette == iSwizzleTexPalette
                                      select itmZList.ZLayer).Distinct().First();

                        if (iScaleFactor == 0)
                        {
                            if (iLayer < 2)
                            {
                                // Prepare LayerMaxWidth for L0/L1
                                SwizzleHash.iLayerMaxWidthGlobal = S9.iLayersMaxWidthL0;
                                SwizzleHash.iLayerMaxHeightGlobal = S9.iLayersMaxHeightL0;
                                SwizzleHash.iLayerbmpPosXGlobal = S9.iLayersbmpPosXL0;
                                SwizzleHash.iLayerbmpPosYGlobal = S9.iLayersbmpPosYL0;
                            }
                            else
                            {
                                // Prepare LayerMaxWidth for L2/L3
                                SwizzleHash.iLayerMaxWidthGlobal = S9.iLayersMaxWidthL2;
                                SwizzleHash.iLayerMaxHeightGlobal = S9.iLayersMaxHeightL2;
                                SwizzleHash.iLayerbmpPosXGlobal = S9.iLayersbmpPosXL2;
                                SwizzleHash.iLayerbmpPosYGlobal = S9.iLayersbmpPosYL2;
                            }
                        }

                        // Check relationship of original width of field and the one of the loaded
                        // Unswizzled Hashed Image. If the modulus is different of zero, the image
                        // has not been created with the actual version of Aeris.
                        if (SwizzleHash.iLayerMaxWidthGlobal % bmpUnsImage.Width != 0)
                        {
                            MessageBox.Show("The loaded Unswizzled Texture seems to not correspond " +
                                            "to one exported from this version of Aeris.", "Error");

                            return;
                        }
                        else
                        {
                            iScaleFactor = bmpUnsImage.Width / S9.iLayersMaxWidthL0;                            
                        }


                        if (bHashedTexture)
                        {
                            txtHash.Visible = true;
                            lbHash.Visible = true;
                            txtHash.Text = "Needs more data";
                        }
                        else
                        {
                            txtHash.Visible = false;
                            lbHash.Visible = false;
                        }

                        txtTextureID.Text = iSwizzleTexTexture.ToString();
                        txtPalette.Text = iSwizzleTexPalette.ToString();
                        txtParam.Text = iSwizzleTexParam.ToString();
                        txtState.Text = iSwizzleTexState.ToString();
                        txtTileID.Text = iSwizzleTexTileID.ToString();
                        txtTextureWidth.Text = bmpUnsImage.Width.ToString();
                        txtTextureHeight.Text = bmpUnsImage.Height.ToString();
                        txtScaleFactor.Text = iScaleFactor.ToString();

//                        ImageTools.ClearPictureBox(pbSwizzledTexturePreview, 1, panelSwizzledPreview);

                        pbSwizzledTexturePreview.SizeMode = PictureBoxSizeMode.Zoom;

                        if (pbSwizzledTexturePreview.Image != null) pbSwizzledTexturePreview.Image.Dispose();
                        pbSwizzledTexturePreview.Image = new Bitmap(bmpUnsImage);

                        btnSwizzleHashImg.Enabled = true;

                        FileTools.strGlobalLoadSwizzleExernal = Path.GetDirectoryName(ofdTexture.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading Unswizzled Texture.", "Error");
            }
        }

        private void btnSwizzle_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmpSwizzledTexture;
                int iScaleFactor;

                iScaleFactor = Int32.Parse(txtScaleFactor.Text);

                bmpSwizzledTexture = new Bitmap(S9.TEXTURE_WIDTH * iScaleFactor, 
                                                S9.TEXTURE_HEIGHT * iScaleFactor);

                SwizzleHash.SwizzleHashedImage(bmpUnsImage, ref bmpSwizzledTexture, 
                                               iSwizzleTexTexture, iSwizzleTexPalette, 
                                               iSwizzleTexParam, iSwizzleTexState, 
                                               iSwizzleTexTileID, iScaleFactor);

                if (bmpSwizzledTexture != null)
                {
                    ImageTools.ClearPictureBox(pbSwizzledTexturePreview, 1, panelSwizzledPreview);

                    if (pbSwizzledTexturePreview.Image != null) pbSwizzledTexturePreview.Image.Dispose();

                    pbSwizzledTexturePreview.SizeMode = PictureBoxSizeMode.StretchImage;
                    pbSwizzledTexturePreview.Image = new Bitmap(bmpSwizzledTexture);

                    txtTextureWidth.Text = pbSwizzledTexturePreview.Image.Width.ToString();
                    txtTextureHeight.Text = pbSwizzledTexturePreview.Image.Height.ToString();

                    bmpSwizzledTexture.Dispose();
                }

                btnSwizzleHashImg.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to Swizzle the loaded Texture.", "Error");
            }
        }

        private void btnSwizzleTextureHashFolder_Click(object sender, EventArgs e)
        {
            string strFileName;
            Bitmap bmpSwizzledCompleteTexture;
            int iResult;

            try
            {
                FolderBrowserDialogEX fbdEX = new FolderBrowserDialogEX();

                // We must select the directory from where to read the files.
                fbdEX.folderBrowser.Description = 
                                    "Select Input Folder (with Hashed Name) where the Unswizzled Textures are:";

                fbdEX.folderBrowser.RootFolder = Environment.SpecialFolder.MyComputer;

                fbdEX.folderBrowser.ShowNewFolderButton = false;

                if (FileTools.strGlobalLoadSwizzleExternalFolder != null)
                {
                    fbdEX.folderBrowser.SelectedPath = 
                                    FileTools.strGlobalLoadSwizzleExternalFolder;
                }
                else
                {
                    fbdEX.folderBrowser.SelectedPath = FileTools.strGlobalPath;
                }

                fbdEX.Tmr.Start();
                if (fbdEX.folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (fbdEX.folderBrowser.SelectedPath != "")
                    {
                        // Put Global folder for load hashed textures.
                        FileTools.strGlobalLoadSwizzleExternalFolder =
                                        fbdEX.folderBrowser.SelectedPath;

                        strHash = FileTools.strGlobalLoadSwizzleExternalFolder.Split(Path.DirectorySeparatorChar).Last();

                        if (strHash.Length > 14 & strHash.Length < 20)
                        {
                            lbHash.Visible = true;
                            txtHash.Visible = true;
                            btnSwizzleHashImg.Enabled = false;
                            txtHash.Text = strHash;
                            strFileName = "";

                            bmpSwizzledCompleteTexture = null;

                            iResult = SwizzleHash.SwizzleHashedExternalImage(FileTools.strGlobalLoadSwizzleExternalFolder, 
                                                                             null, 
                                                                             ref iSwizzleTexTexture, 
                                                                             ref iSwizzleTexPalette, 
                                                                             ref iSwizzleTexParam, 
                                                                             ref iSwizzleTexState, 
                                                                             ref iSwizzleTexTileID, 
                                                                             ref strFileName, 
                                                                             ref bmpSwizzledCompleteTexture);

                            switch (iResult)
                            {
                                case 0:
                                    {
                                        MessageBox.Show("The file: " + strFileName + 
                                                        ".png is not from the loaded Field.", 
                                                        "Warning");
                                        break;
                                    }

                                case 1:
                                    {
                                        if (bmpSwizzledCompleteTexture == null)
                                        {
                                            MessageBox.Show("There has been some problem when doing the Complete " +
                                                            "Texture.", "Information");
                                        }
                                        else
                                        {
                                            txtTextureWidth.Text = bmpSwizzledCompleteTexture.Width.ToString();
                                            txtTextureHeight.Text = bmpSwizzledCompleteTexture.Height.ToString();
                                            txtTextureID.Text = iSwizzleTexTexture.ToString();
                                            txtPalette.Text = iSwizzleTexPalette.ToString();

                                            txtScaleFactor.Text = (bmpSwizzledCompleteTexture.Width / S9.TEXTURE_WIDTH).ToString();

                                            //ImageTools.ClearPictureBox(pbSwizzledTexturePreview, 1, panelSwizzledPreview);

                                            if (pbSwizzledTexturePreview.Image != null) pbSwizzledTexturePreview.Image.Dispose();

                                            pbSwizzledTexturePreview.SizeMode = PictureBoxSizeMode.StretchImage;
                                            pbSwizzledTexturePreview.Image = new Bitmap(bmpSwizzledCompleteTexture);

                                            bmpSwizzledCompleteTexture.Dispose();
                                        }

                                        btnSwizzleHashImg.Enabled = false;
                                        break;
                                    }

                                case 2:
                                    {
                                        MessageBox.Show("There are no files in the folder.", "Information");
                                        break;
                                    }
                                case 4:
                                    {
                                        MessageBox.Show("WARNING: The file '" + strFileName + ".png' has not " +
                                                               "a proportional Scale Factor for this field.", 
                                                        "Warning");
                                        break;
                                    }
                            }
                        }
                    }
                }

                fbdEX.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to Swizzle the Texture/s of Hash Folder.", "Error");
            }
        }

        private void btnSaveTexture_Click(object sender, EventArgs e)
        {
            Bitmap bmpSave;

            try
            {
                // Set filter options and filter index.
                sfdTexture.Title = "Save Texture As .PNG";
                sfdTexture.Filter = ".PNG File|*.png";
                sfdTexture.FilterIndex = 1;
                sfdTexture.FileName = FileTools.strGlobalFieldName + "_" + 
                                      iSwizzleTexTexture.ToString("00") + "_" + 
                                      iSwizzleTexPalette.ToString("00") + "_" + 
                                      iSwizzleTexParam.ToString("00") + "_" + 
                                      iSwizzleTexState.ToString("00") + "_" + 
                                      iSwizzleTexTileID.ToString("00") + "_" + 
                                      strHash + ".png";

                if (FileTools.strGlobalSaveSwizzleExternal != null)
                {
                    sfdTexture.InitialDirectory = FileTools.strGlobalSaveSwizzleExternal;
                }
                else
                {
                    sfdTexture.InitialDirectory = FileTools.strGlobalPath;
                }

                if (sfdTexture.ShowDialog() == DialogResult.OK)
                {
                    if (sfdTexture.FileName != "")
                    {
                        if (sfdTexture.FilterIndex == 1)
                        {
                            bmpSave = new Bitmap(pbSwizzledTexturePreview.Image);

                            ImageTools.WriteBitmap(bmpSave, sfdTexture.FileName);

                            MessageBox.Show("Texture exported in .PNG format.", "Information");

                            FileTools.strGlobalSaveSwizzleExternal = Path.GetDirectoryName(sfdTexture.FileName);

                            bmpSave.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting Texture.", "Error");
            }
        }
    }
}
