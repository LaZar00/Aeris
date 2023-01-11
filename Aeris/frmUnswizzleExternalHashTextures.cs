using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Aeris
{
    public partial class frmUnswizzleExternalHashTextures : Form
    {
        private frmAeris frmAeris;

        public frmUnswizzleExternalHashTextures(frmAeris frmAeris)
        {
            InitializeComponent();

            this.frmAeris = frmAeris;
        }

        public Bitmap bmpSwizzledTexture;
        public int iUnswizzleTexTileID, iUnswizzleTexParam, iUnswizzleTexState;
        public int iUnswizzleTexTexture, iUnswizzleTexPalette;
        public string strUnswizzleTexFileField, strUnswizzleTexFileName;
        public int iUnswizzleTexWidth, iUnswizzleTexHeight, iUnswizzleTexDrawPosX, iUnswizzleTexDrawPosY;
        public bool bHashedTexture;
        public string strHash;

        public void InitializeUnswizzleExternal()
        {
            txtTextureWidth.Text = "";
            txtTextureHeight.Text = "";
            txtScaleFactor.Text = "";

            txtTextureID.Text = "";
            txtPalette.Text = "";

            txtParam.Text = "";
            txtParam.Visible = false;
            txtState.Text = "";
            txtState.Visible = false;

            txtTileID.Text = "";
            txtTileID.Visible = false;

            txtHash.Text = "";
            txtHash.Visible = false;

            lbHash.Visible = false;
            lbUnsTextures.Visible = false;
            lbParam.Visible = false;
            lbState.Visible = false;

            cbUnsTextures.Items.Clear();
            cbUnsTextures.Enabled = false;
            cbUnsTextures.Visible = false;

            btnUnswizzleHashTex.Enabled = false;
            btnSaveHashImgs.Enabled = false;

            ImageTools.ClearPictureBox(pbSwizzledTexturePreview, 1, panelSwizzledPreview);
        }

        private void frmUnswizzleExternal_Load(object sender, EventArgs e)
        {
            if (strUnswizzleTexFileName != FileTools.strGlobalFieldName)
            {
                InitializeUnswizzleExternal();
                strUnswizzleTexFileName = FileTools.strGlobalFieldName;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnLoadTexture_Click(object sender, EventArgs e)
        {

            // Set filter options and filter index.
            ofdTexture.Title = "Open Swizzled Hashed Texture";
            ofdTexture.Filter = "PNG files (*.png)|*.png";
            ofdTexture.FilterIndex = 1;
            ofdTexture.FileName = null;

            if (FileTools.strGlobalLoadUnswizzleExternal != null)
            {
                ofdTexture.InitialDirectory = FileTools.strGlobalLoadUnswizzleExternal;
            }
            else
            {
                ofdTexture.InitialDirectory = FileTools.strGlobalPath;
            }

            try
            {
                // Process input if the user clicked OK.
                if (ofdTexture.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(ofdTexture.FileName))
                    {

                        // Initialize Vars.
                        InitializeUnswizzleExternal();

                        // Get file name data.
                        strUnswizzleTexFileName = Path.GetFileNameWithoutExtension(ofdTexture.FileName);

                        // Let's check if loaded fields equals field of texture.
                        bHashedTexture = FileTools.SplitFileNameAndCheckHash(strUnswizzleTexFileName, 
                                                                             ref strUnswizzleTexFileField, 
                                                                             ref iUnswizzleTexTexture, 
                                                                             ref iUnswizzleTexPalette, 
                                                                             ref iUnswizzleTexParam, 
                                                                             ref iUnswizzleTexState, 
                                                                             ref iUnswizzleTexTileID, 
                                                                             ref strHash);

                        if (!FileTools.ValidateFilewithField(strUnswizzleTexFileField))
                        {
                            MessageBox.Show("WARNING: " + "File: " + strUnswizzleTexFileName + 
                                            ".png is not from the loaded field in the tool.", "Warning");

                            return;
                        }

                        // Load Texture
                        ImageTools.ReadBitmap(ref bmpSwizzledTexture, ofdTexture.FileName);

                        if (bmpSwizzledTexture.Width % S9.TEXTURE_WIDTH != 0)
                        {
                            MessageBox.Show("WARNING: Probably you have loaded a non Swizzled Texture image. " +
                                            "This image will not be processed.", "Warning");
                            return;
                        }


                        if (bHashedTexture)
                        {
                            txtHash.Visible = true;
                            lbHash.Visible = true;
                        }
                        else
                        {
                            txtHash.Visible = false;
                            lbHash.Visible = false;
                        }

                        txtTextureID.Text = iUnswizzleTexTexture.ToString();
                        txtPalette.Text = iUnswizzleTexPalette.ToString();
                        txtHash.Text = strHash;

                        txtTextureWidth.Text = bmpSwizzledTexture.Width.ToString();
                        txtTextureHeight.Text = bmpSwizzledTexture.Height.ToString();
                        txtScaleFactor.Text = (bmpSwizzledTexture.Width / S9.TEXTURE_WIDTH).ToString();

                        ImageTools.ClearPictureBox(pbSwizzledTexturePreview, 1, panelSwizzledPreview);

                        pbSwizzledTexturePreview.SizeMode = PictureBoxSizeMode.StretchImage;

                        if (pbSwizzledTexturePreview.Image != null) pbSwizzledTexturePreview.Image.Dispose();

                        pbSwizzledTexturePreview.Image = new Bitmap(bmpSwizzledTexture);

                        // Check if the scale is 1 or greater
                        if (bmpSwizzledTexture.Width / S9.TEXTURE_WIDTH > 1)
                        {
                            btnUnswizzleHashTex.Enabled = false;

                            MessageBox.Show("It seems this textures has been upscaled. You can only unswizzle textures of vanilla scale.", "Information");
                        }
                        else
                            btnUnswizzleHashTex.Enabled = true;

                        btnSaveHashImgs.Enabled = false;

                        FileTools.strGlobalLoadUnswizzleExternal = Path.GetDirectoryName(ofdTexture.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening Texture.", "Error");
            }
        }

        private void btnUnswizzle_Click(object sender, EventArgs e)
        {
            int iResult;
            int iUnswizzleTexLayer;

            try
            {

                if (!bHashedTexture)
                {
                    MessageBox.Show("Right now it is only possible to Unswizzle Hashed Textures.", "Information");
                    return;
                }

                // Dipose List of Bitmaps and Params/State for the next image.
                SwizzleHash.InitUnswizzledImagesList();
                cbUnsTextures.Items.Clear();


                // Get the Layer.
                iUnswizzleTexLayer = (from sZItem in S9.Section9Z
                                      where sZItem.ZTexture == iUnswizzleTexTexture &
                                            sZItem.ZPalette == iUnswizzleTexPalette
                                      orderby sZItem.ZParam
                                      select sZItem.ZLayer).First();

                iResult = SwizzleHash.Obtain_HashedTextureLayer(iUnswizzleTexLayer,
                                                                iUnswizzleTexTexture,
                                                                iUnswizzleTexPalette,
                                                                (Bitmap)pbSwizzledTexturePreview.Image);

                if (iResult == -1)
                {
                    return;
                }
                else if (iResult == 3)
                {
                    MessageBox.Show("This Texture has not Unswizzled data for creating an image.", "Warning");
                    return;
                }

                if (SwizzleHash.lstUnswizzledImagesList.Count() > 0)
                {
                    ImageTools.ClearPictureBox(pbSwizzledTexturePreview, 1, panelSwizzledPreview);

                    // Populate list of Textures.
                    foreach (var stUnswizzledImage in SwizzleHash.lstUnswizzledImagesList)
                    {
                        {
                            if (bHashedTexture)
                            {
                                cbUnsTextures.Items.Add(strUnswizzleTexFileField + "_" +
                                    stUnswizzledImage.stUnswizzledTextureParamsStates.Texture.ToString("00") + "_" +
                                    stUnswizzledImage.stUnswizzledTextureParamsStates.Palette.ToString("00") + "_" +
                                    stUnswizzledImage.stUnswizzledTextureParamsStates.Param.ToString("00") + "_" +
                                    stUnswizzledImage.stUnswizzledTextureParamsStates.State.ToString("00") + "_" +
                                    stUnswizzledImage.stUnswizzledTextureParamsStates.TileID.Keys[0].ToString("0000"));

                                lbParam.Visible = true;
                                lbState.Visible = true;
                                lbTileID.Visible = true;
                                txtParam.Visible = true;
                                txtState.Visible = true;
                                txtTileID.Visible = true;
                            }
                            else
                            {
                                cbUnsTextures.Items.Add(strUnswizzleTexFileField + "_" + 
                                                        iUnswizzleTexTexture.ToString("00") + "_" + 
                                                        iUnswizzleTexPalette.ToString("00"));
                            }
                        }
                    }


                    // Activate objects.
                    cbUnsTextures.Enabled = true;
                    cbUnsTextures.Visible = true;
                    lbUnsTextures.Visible = true;
                    btnSaveHashImgs.Enabled = true;


                    // Populate objects.
                    txtTextureWidth.Text = SwizzleHash.lstUnswizzledImagesList[0].bmpUnswizzledImages.Width.ToString();
                    txtTextureHeight.Text = SwizzleHash.lstUnswizzledImagesList[0].bmpUnswizzledImages.Height.ToString();


                    // Select first texture.
                    cbUnsTextures.SelectedIndex = 0;

                }

                btnUnswizzleHashTex.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to Unswizzle the loaded Texture.", "Error");
            }
        }

        private void cbUnsTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbUnsTextures.SelectedIndex < SwizzleHash.lstUnswizzledImagesList.Count())
            {
                {
                    iUnswizzleTexParam = 
                        SwizzleHash.lstUnswizzledImagesList[cbUnsTextures.SelectedIndex].stUnswizzledTextureParamsStates.Param;

                    iUnswizzleTexState = 
                        SwizzleHash.lstUnswizzledImagesList[cbUnsTextures.SelectedIndex].stUnswizzledTextureParamsStates.State;

                    iUnswizzleTexTileID = 
                        SwizzleHash.lstUnswizzledImagesList[cbUnsTextures.SelectedIndex].stUnswizzledTextureParamsStates.TileID.Keys[0];

                    txtParam.Text = iUnswizzleTexParam.ToString();
                    txtState.Text = iUnswizzleTexState.ToString();
                    txtTileID.Text = iUnswizzleTexTileID.ToString();

                    pbSwizzledTexturePreview.SizeMode = PictureBoxSizeMode.Zoom;

                    if (pbSwizzledTexturePreview.Image != null) pbSwizzledTexturePreview.Image.Dispose();

                    pbSwizzledTexturePreview.Image = 
                        new Bitmap(SwizzleHash.lstUnswizzledImagesList[cbUnsTextures.SelectedIndex].bmpUnswizzledImages);

                    pbSwizzledTexturePreview.Invalidate();
                }
            }
        }

        private void btnSaveTexture_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogEX fbdEX = new FolderBrowserDialogEX();

            try
            {
                // We must select the directory from where to read the files.
                fbdEX.folderBrowser.Description = 
                            "Select Output Folder for Save All Unswizzled Textures:";

                fbdEX.folderBrowser.RootFolder = 
                            Environment.SpecialFolder.MyComputer;

                if (FileTools.strGlobalSaveUnswizzleExternal != null)
                {
                    fbdEX.folderBrowser.SelectedPath = 
                            FileTools.strGlobalSaveUnswizzleExternal;
                }
                else
                {
                    fbdEX.folderBrowser.SelectedPath = 
                            FileTools.strGlobalPath;
                }

                fbdEX.Tmr.Start();
                if (fbdEX.folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (fbdEX.folderBrowser.SelectedPath != "")
                    {
                        // Put Global folder for export all unswizzled images.
                        FileTools.strGlobalSaveUnswizzleExternal =
                            fbdEX.folderBrowser.SelectedPath;

                        for (int i = 0, loopTo = SwizzleHash.lstUnswizzledImagesList.Count() - 1; i <= loopTo; i++)
                        {
                            {
                                //ImageTools.WriteBitmap(SwizzleHash.lstUnswizzledImagesList[i].bmpUnswizzledImages, 
                                //                       FileTools.strGlobalSaveUnswizzleExternal + "\\" + 
                                //                       cbUnsTextures.Items[i] + ".png");

                                SwizzleHash.lstUnswizzledImagesList[i].bmpUnswizzledImages.Save(
                                    FileTools.strGlobalSaveUnswizzleExternal + "\\" + 
                                    cbUnsTextures.Items[i] + ".png");
                            }
                        }
                    }

                    MessageBox.Show("Unswizzled Textures saved.", "Information");
                }

                fbdEX.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving Unswizzled Textures in the Folder.", "Error");
            }
        }
    }
}
