using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STOL_Training_Tool_Core.UI
{
    public partial class TeleportDialog : Form
    {
        public bool DontShowAgain { get; private set; } = false;

        public TeleportDialog()
        {
            InitializeComponent();
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            Close();
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            Close();
        }

        private void buttonNoShow_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            DontShowAgain = true;
            Close();
        }
    }
}
