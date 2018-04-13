/* Copyright © Macsim Belous 2012 */
/* This file is part of Erza.

    Foobar is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Foobar is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <https://www.gnu.org/licenses/>.*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Eris
{
    public partial class Form1 : Form
    {
        SQLiteDataAdapter adapter;
        DataTable table;
        SQLiteConnection connection;
        int index_search = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //dataGridView1.AutoGenerateColumns = true;
            table = new DataTable();
            connection = new SQLiteConnection(@"data source=C:\utils\data\Erza.sqlite");
            connection.Open();
            adapter = new SQLiteDataAdapter();
            adapter.SelectCommand = new SQLiteCommand("SELECT * FROM tags", connection);
            SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);
            adapter.Fill(this.table);
            adapter.UpdateCommand = builder.GetUpdateCommand();
            adapter.InsertCommand = builder.GetInsertCommand();
            adapter.DeleteCommand = builder.GetDeleteCommand();
            adapter.Update(this.table);
            this.dataGridView1.DataSource = table;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            adapter.Update(this.table);
            connection.Clone();
        }

        private void edit_toolStripButton_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = this.dataGridView1.SelectedRows[0];
                EditForm form = new EditForm();
                form.TagID = (long)row.Cells["tag_id"].Value;
                form.TagName = (string)row.Cells["tag"].Value;
                form.TagCount = (long)row.Cells["count"].Value;
                form.TagType = (long)row.Cells["type"].Value;
                if (!System.DBNull.Value.Equals(row.Cells["localization"].Value))
                {
                    form.TagNameRus = (string)row.Cells["localization"].Value;
                }
                else
                {
                    form.TagNameRus = String.Empty;
                }
                if (!System.DBNull.Value.Equals(row.Cells["description"].Value))
                {
                    form.TagDescription = (string)row.Cells["description"].Value;
                }
                else
                {
                    form.TagDescription = String.Empty;
                }
                if (form.ShowDialog() == DialogResult.OK)
                {
                    row.Cells["tag"].Value = form.TagName;
                    //row.Cells["count"].Value = form.TagCount;
                    row.Cells["type"].Value = form.TagType;
                    if (String.IsNullOrEmpty(form.TagNameRus))
                    {
                        row.Cells["localization"].Value = System.DBNull.Value;
                    }
                    else
                    {
                        row.Cells["localization"].Value = form.TagNameRus;
                    }
                    if (String.IsNullOrEmpty(form.TagDescription))
                    {
                        row.Cells["description"].Value = System.DBNull.Value;
                    }
                    else
                    {
                        row.Cells["description"].Value = form.TagDescription;
                    }
                    this.adapter.Update(this.table);
                }
            }
        }

        private void delete_toolStripButton_Click(object sender, EventArgs e)
        {
            if(dataGridView1.SelectedRows.Count <= 0) { return; }
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
            this.adapter.Update(this.table);
        }

        private void new_toolStripButton_Click(object sender, EventArgs e)
        {
            EditForm form = new EditForm();
            form.TagID = -1;
            form.TagName = String.Empty;
            form.TagCount = 0;
            form.TagType = 0;
            form.TagNameRus = String.Empty;
            form.TagDescription = String.Empty;
            if (form.ShowDialog() == DialogResult.OK)
            {
                DataRow row = this.table.NewRow();
                row["tag"] = form.TagName;
                row["count"] = 0;
                row["type"] = form.TagType;
                if (String.IsNullOrEmpty(form.TagNameRus))
                {
                    row["localization"] = System.DBNull.Value;
                }
                else
                {
                    row["localization"] = form.TagNameRus;
                }
                if (String.IsNullOrEmpty(form.TagDescription))
                {
                    row["description"] = System.DBNull.Value;
                }
                else
                {
                    row["description"] = form.TagDescription;
                }
                this.table.Rows.Add(row);
                this.adapter.Update(this.table);
                int f = 0;
                f = dataGridView1.CurrentRow.Index;
                table.Clear();
                this.adapter.Fill(this.table);
                dataGridView1.ClearSelection();
                dataGridView1.Rows[f].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = f;
            }
        }

        private void find_toolStripButton_Click(object sender, EventArgs e)
        {
            for (; index_search < dataGridView1.RowCount;)
            {
                if (dataGridView1["tag", index_search].FormattedValue.ToString().Contains(this.search_toolStripTextBox.Text.Trim()))
                {
                    dataGridView1.CurrentCell = dataGridView1[0, index_search];
                    if (index_search < dataGridView1.RowCount - 1)
                        index_search++;
                    else
                        index_search = 0;
                    return;
                }
                else
                {
                    if (index_search < dataGridView1.RowCount - 1)
                        index_search++;
                    else
                        index_search = 0;
                }
            }
        }

        private void count_toolStripButton_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                long count_rows;
                DataGridViewRow row = this.dataGridView1.SelectedRows[0];
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "SELECT count(*) FROM image_tags WHERE image_tags.tag_id = @tag_id;";
                    command.Parameters.AddWithValue("tag_id", row.Cells["tag_id"].Value);
                    command.Connection = connection;
                    count_rows = System.Convert.ToInt64(command.ExecuteScalar());
                }
                row.Cells["count"].Value = count_rows;
                this.adapter.Update(this.table);
            }
        }

        private void count_all_toolStripButton_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Произвести подсчёт ссылок на все теги?","Подтверждение",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                CalculateAllLinksForm form = new CalculateAllLinksForm();
                form.table = table;
                form.connection = connection;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    this.adapter.Update(this.table);
                }
            }
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex > -1 && e.RowIndex < dataGridView1.RowCount)
            {
                if ((long)dataGridView1.Rows[e.RowIndex].Cells["type"].Value == 1)
                    ((DataGridView)sender).Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightBlue;

                if ((long)dataGridView1.Rows[e.RowIndex].Cells["type"].Value == 2)
                    ((DataGridView)sender).Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCyan;
            }
        }
    }
}
