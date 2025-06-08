using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            Application.OpenForms["TapSend"]?.Show(); 
            this.Close();
        }

    }
}
