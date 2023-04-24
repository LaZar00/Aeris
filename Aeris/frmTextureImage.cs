using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aeris
{
    public partial class FrmTextureImage : Form
    {

        public FrmTextureImage(FrmAeris inFrmAeris)
        {
            InitializeComponent();

            this.Owner = inFrmAeris;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
