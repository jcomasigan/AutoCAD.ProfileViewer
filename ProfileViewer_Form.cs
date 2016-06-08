using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProfileViewer
{
    public partial class ProfileViewer_Form : Form
    {
        public ProfileViewer_Form()
        {
            InitializeComponent();
            majorCont_cmbx.DataSource = Main.GetLayerNames();
            minorCont_cmbx.DataSource = Main.GetLayerNames();
        }

        private void selectPline_btn_Click(object sender, EventArgs e)
        {
            string objType = "";
            ObjectId profileLineId = Main.SelectPolyline(out objType);
            if (profileLineId != ObjectId.Null)
            {
                selectPline_btn.Text = objType + " SELECTED";
                GlobalVars.profileLineId = profileLineId;
            }
        }

        private void majorOnly_chbx_CheckedChanged(object sender, EventArgs e)
        {
            if(majorOnly_chbx.CheckState == CheckState.Checked)
            {
                minorCont_cmbx.Enabled = false;
            }
            else
            {
                minorCont_cmbx.Enabled = true;
            }
        }

        private void plot_btn_Click(object sender, EventArgs e)
        {
            Main.GetProfile(GlobalVars.profileLineId, majorCont_cmbx.SelectedItem.ToString(), minorCont_cmbx.SelectedItem.ToString());
        }
    }
}
