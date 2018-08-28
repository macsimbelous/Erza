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
            List<TagsCount> it = new List<TagsCount>();
            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                command.CommandText = "SELECT count(tag_id), tag_id FROM image_tags GROUP BY tag_id";
                using(SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        it.Add(new TagsCount(reader.GetInt64(1), reader.GetInt64(0)));
                    }
                    reader.Close();
                }
            }
            using (SQLiteTransaction transact = connection.BeginTransaction())
            {
                for (int i = 0; i < it.Count; i++)
                {
                    if (Abort)
                    {
                        transact.Rollback();
                        synchronizationContext.Post(EndProgress, false);
                        return;
                    }
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "UPDATE tags SET count = @count WHERE tag_id = @tag_id;";
                        command.Parameters.AddWithValue("count", it[i].Count);
                        command.Parameters.AddWithValue("tag_id", it[i].TagID);
                        command.ExecuteNonQuery();
                    }
                    synchronizationContext.Post(RefreshProgress, i + 1);
                }
                transact.Commit();
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
    public class TagsCount
    {
        public long TagID;
        public long Count;
        public TagsCount(long TagID, long Count)
        {
            this.Count = Count;
            this.TagID = TagID;
        }
    }
}
