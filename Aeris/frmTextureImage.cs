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
    public partial class frmTextureImage : Form
    {
        private frmAeris frmAeris;

        public frmTextureImage(frmAeris frmAeris)
        {
            InitializeComponent();

            this.frmAeris = frmAeris;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
