using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ErzaLib;
using System.Data.SQLite;

namespace Ange
{
    public partial class AddTagForm : Form
    {
        public List<string> Tags;
        public string NewTag;
        public string[] NewTags;
        public SQLiteConnection Erza;
        public AddTagForm()
        {
            InitializeComponent();
        }

        private void AddTagForm_Load(object sender, EventArgs e)
        {
            //Tags.Sort();
            //BindingList<string> temp = new BindingList<string>(Tags);
            //this.comboBox1.DataSource = temp;
            //this.comboBox1.DataSource = Tags;
            this.comboBox1.AutoCompleteMode = AutoCompleteMode.None;
        }

        private void OK_button_Click(object sender, EventArgs e)
        {
            this.NewTag = this.comboBox1.Text;
            NewTags = NewTag.Trim().Split(' ');
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            string ts = this.comboBox1.Text;
            List<string> temp;
            if (ts.Length < 3)
            {
                temp = ErzaDB.SearchTags(ts, true, true, Erza);
            }
            else
            {
                temp = ErzaDB.SearchTags(ts, false, true, Erza);
            }
            if (temp.Count > 0)
            {
                this.comboBox1.DataSource = temp;
                this.comboBox1.DroppedDown = true;

                this.comboBox1.Text = ts;
                this.comboBox1.SelectionStart = ts.Length;
                Cursor.Current = Cursors.Default;
            }
            else
            {
                //this.comboBox1.DroppedDown = false;
                //temp = new List<string>();
                temp.Add(ts);
                this.comboBox1.DataSource = temp;
                this.comboBox1.Text = ts;
                this.comboBox1.SelectionStart = ts.Length;
                Cursor.Current = Cursors.Default;
            }
        }

        private void button_quadruple_amputee_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "quadruple_amputee";
        }

        private void button_women_livestock_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "women_livestock";
        }

        private void button_bdsm_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "bdsm";
        }

        private void button_bondage_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "bondage";
        }

        private void button_blindfold_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "blindfold";
        }

        private void button_leash_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "leash";
        }

        private void button_pet_play_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "pet_play";
        }

        private void button_milking_machine_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "milking_machine";
        }

        private void button_gag_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "gag";
        }

        private void button_ball_gag_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "ball_gag";
        }

        private void button_dildo_gag_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "dildo_gag";
        }

        private void button_ring_gag_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "ring_gag";
        }

        private void button_guro_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "guro";
        }

        private void button_decapitation_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "decapitation";
        }

        private void button_brain_sex_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "brain_sex";
        }

        private void button_bestiality_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "bestiality";
        }

        private void button_dog_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "dog";
        }

        private void button_horse_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "horse";
        }

        private void button_pig_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "pig";
        }

        private void button_bull_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "bull";
        }

        private void button_tentacle_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "tentacle";
        }

        private void button_orc_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "orc";
        }

        private void button_sex_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "sex";
        }

        private void button_vaginal_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "vaginal";
        }

        private void button_anal_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "anal";
        }

        private void button_oral_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "oral";
        }

        private void button_fellatio_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "fellatio";
        }

        private void button_nipple_penetration_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "nipple_penetration";
        }

        private void button_rape_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "rape";
        }

        private void button_forced_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "forced";
        }

        private void button_group_sex_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "group_sex";
        }

        private void button_double_penetration_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "double_penetration";
        }

        private void button_triple_penetration_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "triple_penetration";
        }

        private void button_trap_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "trap";
        }

        private void button_yaoi_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "yaoi";
        }

        private void button_intersex_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "intersex";
        }

        private void button_futanari_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "futanari";
        }

        private void button_yuri_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Length > 0)
            {
                this.comboBox1.Text = this.comboBox1.Text + " ";
            }
            this.comboBox1.Text = this.comboBox1.Text + "yuri";
        }
    }
}
