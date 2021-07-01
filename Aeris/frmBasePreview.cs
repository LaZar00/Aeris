using System;
using System.Drawing;
using System.Windows.Forms;

namespace Aeris
{
    public partial class frmBasePreview : Form
    {

        private frmAeris frmAeris;

        public frmBasePreview(frmAeris frmAeris)
        {
            InitializeComponent();

            this.frmAeris = frmAeris;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void DrawBase()
        {
            DirectBitmap bmpBase;

            bmpBase = new DirectBitmap(S9.iLayersMaxWidth, S9.iLayersMaxHeight);

            S9.Render_S9BaseLayer(ref bmpBase, frmAeris);

            if (pbBasePreview.Image != null) pbBasePreview.Image.Dispose();

            pbBasePreview.Image = new Bitmap(bmpBase.Bitmap);

            bmpBase.Dispose();
        }

        private void frmBasePreview_Load(object sender, EventArgs e)
        {
            ImageTools.ClearPictureBox(pbBasePreview, 1, null);
            DrawBase();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (pbBasePreview.Image != null)
                {
                    // Set filter options and filter index.
                    sfdSaveBasePreview.Title = "Save Base As .PNG";
                    sfdSaveBasePreview.Filter = ".PNG File|*.png";
                    sfdSaveBasePreview.InitialDirectory = FileTools.strGlobalPath;
                    sfdSaveBasePreview.FilterIndex = 1;
                    sfdSaveBasePreview.FileName = FileTools.strGlobalFieldName + "_Base";
                    sfdSaveBasePreview.FilterIndex = 1;
                    if (sfdSaveBasePreview.ShowDialog() == DialogResult.OK)
                    {
                        if (sfdSaveBasePreview.FileName != "")
                        {
                            if (sfdSaveBasePreview.FilterIndex == 1)
                            {
                                ImageTools.WriteBitmap((Bitmap)pbBasePreview.Image, 
                                                       sfdSaveBasePreview.FileName);
                                MessageBox.Show("Base Image exported in .PNG format.", "Information");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving Base Image.", "Error");
            }
        }
    }
}
