using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
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
            this.Hide();
            string objType = "";
            ObjectId profileLineId = Main.SelectPolyline(out objType);
            if (profileLineId != ObjectId.Null)
            {
                selectPline_btn.Text = objType + " SELECTED";
                GlobalVars.profileLineId = profileLineId;
            }
            this.Show();
        }

        private void majorOnly_chbx_CheckedChanged(object sender, EventArgs e)
        {
            if(majorOnly_chbx.CheckState == CheckState.Checked)
            {
                minorCont_cmbx.Enabled = false;
                GlobalVars.majorOnly = true;
            }
            else
            {
                minorCont_cmbx.Enabled = true;
                GlobalVars.majorOnly = false;
            }
        }

        private void plot_btn_Click(object sender, EventArgs e)
        {
            if (GlobalVars.profileLineId != ObjectId.Null)
            {
                plot_btn.Enabled = false;
                try
                {
                    Main.GetProfile(GlobalVars.profileLineId, majorCont_cmbx.SelectedItem.ToString(), minorCont_cmbx.SelectedItem.ToString());
                    MessageBox.Show("Successfully created profile. Inserted at: " + insertionButton.Text);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("ERROR: " + ex.Message);
                }

                plot_btn.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please select profile line and/or insertion point");
            }
        }

        private void insertionButton_Click(object sender, EventArgs e)
        {
            insertionButton.Text = Main.GetInsertionPoint();
        }
    }
}
