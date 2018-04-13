using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Data.SQLite;

namespace Eris
{
    public partial class CalculateAllLinksForm : Form
    {
        Thread thread;
        public DataTable table;
        bool Abort = false;
        SynchronizationContext synchronizationContext;
        public SQLiteConnection connection;
        public CalculateAllLinksForm()
        {
            InitializeComponent();
        }
        private void LongRunningTask(object o)
        {
            int count_rows = this.table.Rows.Count;
            for (int i = 0; i < count_rows; i++)
            {
                if (Abort)
                {
                    synchronizationContext.Post(EndProgress, false);
                    return;
                }
                DataRow row = table.Rows[i];
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "SELECT count(*) FROM image_tags WHERE image_tags.tag_id = @tag_id;";
                    command.Parameters.AddWithValue("tag_id", row["tag_id"]);
                    command.Connection = connection;
                    row["count"] = System.Convert.ToInt64(command.ExecuteScalar());
                }
                synchronizationContext.Post(RefreshProgress, i+1);
            }
            synchronizationContext.Post(EndProgress, true);
        }
        private void RefreshProgress(object progress) // это для вызова  через Пост/Сенд
        {
            progressBar1.Value = (int)progress;
        }
        private void EndProgress(object status)
        {
            if ((bool)status)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        private void CalculateAllLinksForm_Load(object sender, EventArgs e)
        {
            this.progressBar1.Maximum = this.table.Rows.Count;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Step = 1;
            this.progressBar1.Value = 0;
            this.Abort = false;
            synchronizationContext = SynchronizationContext.Current;
            thread = new Thread(LongRunningTask);
            thread.Start(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Abort = true;
        }
    }
}
