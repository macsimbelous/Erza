/* Copyright © Macsim Belous 2012 */
/* This file is part of Moka.

    Foobar is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Moka
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings1.Default.Login = this.login_textBox.Text;
            Settings1.Default.Password = this.password_textBox.Text;
            Settings1.Default.ConnectionString = this.ConnectionString_textBox.Text;
            Settings1.Default.danbooru = this.danbooru_checkBox.Checked;
            Settings1.Default.gelbooru = this.gelbooru_checkBox.Checked;
            Settings1.Default.imouto = this.imouto_checkBox.Checked;
            Settings1.Default.konachan = this.konachan_checkBox.Checked;
            Settings1.Default.sankaku = this.sankaku_checkBox.Checked;
            Settings1.Default.Save();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            this.ConnectionString_textBox.Text = Settings1.Default.ConnectionString;
            this.login_textBox.Text = Settings1.Default.Login;
            this.password_textBox.Text = Settings1.Default.Password;
            this.danbooru_checkBox.Checked = Settings1.Default.danbooru;
            this.gelbooru_checkBox.Checked = Settings1.Default.gelbooru;
            this.imouto_checkBox.Checked = Settings1.Default.imouto;
            this.konachan_checkBox.Checked = Settings1.Default.konachan;
            this.sankaku_checkBox.Checked = Settings1.Default.sankaku;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ConnectionString_textBox.Text = "data source=" + this.openFileDialog1.FileName;
            }
        }
    }
}
