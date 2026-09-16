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
using ErzaLib2;

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
            List<ImageInfo> images = new List<ImageInfo>();
            string sql = "SELECT image_id, tags FROM images WHERE deleted = 0 AND file_path IS NOT NULL;";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ImageInfo image = new ImageInfo();
                    //image.ImageID = (long)reader["image_id"];
                    image.ImageID = reader.GetInt64(0);
                    if (!reader.IsDBNull(1))
                    {
                        image.TagIDs = ErzaDB.ParseStringOfTagIDs(reader.GetString(1));
                    }
                    /*object tags = reader["tags"];
                    if (tags != DBNull.Value)
                    {
                        image.TagIDs = ParseStringOfTagIDs((string)tags);
                    }*/
                    images.Add(image);
                }
                reader.Close();
            }
            synchronizationContext.Post(StartProgress, images.Count);
            using (SQLiteTransaction transact = connection.BeginTransaction())
            {
                //Обнуляем счётчики в БД
                using (SQLiteCommand command = new SQLiteCommand("UPDATE tags SET count = 0", connection))
                {
                    command.ExecuteNonQuery();
                }
                for (int i = 0; i < images.Count; i++)
                {
                    if (Abort)
                    {
                        transact.Rollback();
                        synchronizationContext.Post(EndProgress, false);
                        return;
                    }
                    if (images[i].TagIDs.Count > 0)
                    {
                        foreach (long tag in images[i].TagIDs)
                        {
                            using (SQLiteCommand command = new SQLiteCommand("UPDATE tags SET count = count + 1 WHERE tag_id = @tag_id", connection))
                            {
                                command.Parameters.AddWithValue("tag_id", tag);
                                command.ExecuteNonQuery();
                            }
                        }
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
        private void StartProgress(object progress) // это для вызова  через Пост/Сенд
        {
            this.progressBar1.Maximum = (int)progress;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Step = 1;
            this.progressBar1.Value = 0;
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
