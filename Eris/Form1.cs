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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //dataGridView1.AutoGenerateColumns = true;
            table = new DataTable();
            connection = new SQLiteConnection(@"data source=C:\utils\Erza\Erza.sqlite");
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
            EditForm form = new EditForm();
            form.TagID = (long)this.dataGridView1.SelectedRows[0].Cells[0].Value;
            form.ShowDialog();
        }
    }
}
