﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Rei
{
    public partial class Form1 : Form
    {
        SQLiteConnection Connection;
        //List<string> Tags;
        BindingList<string> Tags;
        List<TypeTag> Types;
        bool EditMode = false;
        bool NewMode = false;
        public Form1()
        {
            InitializeComponent();
            Types = new List<TypeTag>();
            Types.Add(new TypeTag(0, "General"));
            Types.Add(new TypeTag(1, "Artist"));
            Types.Add(new TypeTag(3, "Copyright"));
            Types.Add(new TypeTag(4, "Character"));
            Types.Add(new TypeTag(5, "Circle"));
            Types.Add(new TypeTag(6, "Faults"));
            Types.Add(new TypeTag(8, "Medium"));
            Types.Add(new TypeTag(9, "Meta"));
            Types.Add(new TypeTag(2, "Studio"));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Connection = new SQLiteConnection(@"data source=C:\utils\data\Erza.sqlite");
            Connection.Open();
            Tags = new BindingList<string>();
            using (SQLiteCommand command = new SQLiteCommand())
            {

                command.CommandText = "SELECT tag FROM favtags";
                command.Connection = Connection;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tags.Add(reader.GetString(0));
                    }
                    reader.Close();
                }
            }
            listBox1.DataSource = Tags;

            
            this.type_comboBox.DataSource = Types;
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Connection.Close();
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            string tag = (string)listBox1.SelectedValue;
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "SELECT * FROM favtags WHERE tag = @tag";
                command.Parameters.AddWithValue("tag", tag);
                command.Connection = Connection;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tag_textBox.Text = (string)reader["tag"];
                        long temp = Convert.ToInt64(reader["type"]);
                        //type_comboBox.Text = Convert.ToInt64(reader["type"]).ToString();
                        foreach (TypeTag t in Types)
                        {
                            if (t.Type == temp)
                            {
                                this.type_comboBox.SelectedItem = t;
                            }
                        }
                        time_textBox.Text = Convert.ToDateTime(reader["time_stamp"]).ToString();
                        object o = reader["description"];
                        if (o != DBNull.Value)
                        {
                            description_textBox.Text = (string)o;
                        }
                        new_button.Enabled = true;
                        edit_button.Enabled = true;
                        tag_textBox.ReadOnly = true;
                        description_textBox.ReadOnly = true;
                        type_comboBox.Enabled = false;
                        save_button.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Тег " + tag + " отсутствует в БД!");
                    }
                    reader.Close();
                }
            }
        }

        private void edit_button_Click(object sender, EventArgs e)
        {
            EditMode = true;
            listBox1.Enabled = false;
            new_button.Enabled = false;
            edit_button.Enabled = false;
            save_button.Enabled = true;
            tag_textBox.ReadOnly = false;
            description_textBox.ReadOnly = false;
            type_comboBox.Enabled = true;
        }

        private void new_button_Click(object sender, EventArgs e)
        {
            EditMode = true;
            NewMode = true;
            listBox1.Enabled = false;
            new_button.Enabled = false;
            edit_button.Enabled = false;
            save_button.Enabled = true;
            tag_textBox.ReadOnly = false;
            description_textBox.ReadOnly = false;
            type_comboBox.Enabled = true;

            tag_textBox.Text = String.Empty;
            description_textBox.Text = String.Empty;
            type_comboBox.SelectedItem = Types[0];
            time_textBox.Text = String.Empty;
        }

        private void del_button_Click(object sender, EventArgs e)
        {
            string tag = (string)listBox1.SelectedValue;
            if (EditMode)
            {

            }
            else
            {
                RemoveTag(tag);
                Tags.Remove(tag);
                listBox1.Refresh();
            }
        }
        private void RemoveTag(string Tag)
        {
            using (SQLiteCommand command = new SQLiteCommand())
            {
                command.CommandText = "DELETE FROM favtags WHERE tag = @tag";
                command.Parameters.AddWithValue("tag", Tag);
                command.Connection = Connection;
                command.ExecuteNonQuery();
            }
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            if (NewMode)
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "INSERT INTO favtags (tag, type, description) VALUES (@tag, @type, @description)";
                    command.Parameters.AddWithValue("tag", tag_textBox.Text);
                    command.Parameters.AddWithValue("type", ((TypeTag)type_comboBox.SelectedItem).Type);
                    command.Parameters.AddWithValue("description", description_textBox.Text);
                    command.Connection = Connection;
                    command.ExecuteNonQuery();
                }
                Tags.Add(tag_textBox.Text);
            }
            else
            {
                Tags[listBox1.SelectedIndex] = tag_textBox.Text;
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    command.CommandText = "UPDATE favtags SET tag = @tag, type = @type, description = @description WHERE tag = @tagold";
                    command.Parameters.AddWithValue("tag", tag_textBox.Text);
                    command.Parameters.AddWithValue("tagold", (string)listBox1.SelectedValue);
                    command.Parameters.AddWithValue("type", ((TypeTag)type_comboBox.SelectedItem).Type);
                    command.Parameters.AddWithValue("description", description_textBox.Text);
                    command.Connection = Connection;
                    command.ExecuteNonQuery();
                }
                
            }
            new_button.Enabled = true;
            edit_button.Enabled = true;
            tag_textBox.ReadOnly = true;
            description_textBox.ReadOnly = true;
            type_comboBox.Enabled = false;
            save_button.Enabled = false;
            listBox1.Enabled = true;
            NewMode = false;
            EditMode = false;
        }
    }
    public class TypeTag
    {
        public long Type;
        public string Name;
        public string NameRus;
        public TypeTag(long Type, string Name)
        {
            this.Type = Type;
            this.Name = Name;
        }
        public TypeTag(long Type, string Name, string NameRus)
        {
            this.Type = Type;
            this.Name = Name;
            this.NameRus = NameRus;
        }
        override public string ToString()
        {
            return this.Name;
        }
    }
}
