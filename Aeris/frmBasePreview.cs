using System;
using System.Drawing;
using System.Windows.Forms;

namespace Aeris
{

    using static S9;

    using static FileTools;

    public partial class FrmBasePreview : Form
    {

        readonly private FrmAeris frmAeris;

        public FrmBasePreview(FrmAeris inFrmAeris)
        {
            InitializeComponent();

            this.frmAeris = inFrmAeris;
            Owner = inFrmAeris;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void DrawBase()
        {
            DirectBitmap bmpBase;

            bmpBase = new DirectBitmap(iLayersMaxWidth, iLayersMaxHeight);

            Render_S9BaseLayer(ref bmpBase, frmAeris);

            if (pbBasePreview.Image != null) pbBasePreview.Image.Dispose();

            pbBasePreview.Image = new Bitmap(bmpBase.Bitmap);

            bmpBase.Dispose();
        }

        private void FrmBasePreview_Load(object sender, EventArgs e)
        {
            ImageTools.ClearPictureBox(pbBasePreview, 1, null);
            DrawBase();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (pbBasePreview.Image != null)
                {
                    // Set filter options and filter index.
                    sfdSaveBasePreview.Title = "Save Base As .PNG";
                    sfdSaveBasePreview.Filter = ".PNG File|*.png";
                    sfdSaveBasePreview.InitialDirectory = strGlobalPath;
                    sfdSaveBasePreview.FilterIndex = 1;
                    sfdSaveBasePreview.FileName = strGlobalFieldName + "_Base";
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
                strExceptionVar = ex.Message;
                MessageBox.Show("Error saving Base Image.", "Error");
            }
        }
    }
}
