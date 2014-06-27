namespace Moka
{
    partial class AddTaggetFileForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.files_listBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.add_file_button = new System.Windows.Forms.Button();
            this.add_dir_button = new System.Windows.Forms.Button();
            this.del_file_button = new System.Windows.Forms.Button();
            this.clear_button = new System.Windows.Forms.Button();
            this.OK_button = new System.Windows.Forms.Button();
            this.Cancel_button = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.dir_checkBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tags_textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // files_listBox
            // 
            this.files_listBox.FormattingEnabled = true;
            this.files_listBox.Location = new System.Drawing.Point(12, 28);
            this.files_listBox.Name = "files_listBox";
            this.files_listBox.Size = new System.Drawing.Size(404, 277);
            this.files_listBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Файлы";
            // 
            // add_file_button
            // 
            this.add_file_button.Location = new System.Drawing.Point(422, 28);
            this.add_file_button.Name = "add_file_button";
            this.add_file_button.Size = new System.Drawing.Size(117, 23);
            this.add_file_button.TabIndex = 2;
            this.add_file_button.Text = "Добавить";
            this.add_file_button.UseVisualStyleBackColor = true;
            this.add_file_button.Click += new System.EventHandler(this.add_file_button_Click);
            // 
            // add_dir_button
            // 
            this.add_dir_button.Location = new System.Drawing.Point(422, 57);
            this.add_dir_button.Name = "add_dir_button";
            this.add_dir_button.Size = new System.Drawing.Size(117, 23);
            this.add_dir_button.TabIndex = 3;
            this.add_dir_button.Text = "Добавить каталог";
            this.add_dir_button.UseVisualStyleBackColor = true;
            this.add_dir_button.Click += new System.EventHandler(this.add_dir_button_Click);
            // 
            // del_file_button
            // 
            this.del_file_button.Location = new System.Drawing.Point(422, 86);
            this.del_file_button.Name = "del_file_button";
            this.del_file_button.Size = new System.Drawing.Size(117, 23);
            this.del_file_button.TabIndex = 4;
            this.del_file_button.Text = "Удалить";
            this.del_file_button.UseVisualStyleBackColor = true;
            this.del_file_button.Click += new System.EventHandler(this.del_file_button_Click);
            // 
            // clear_button
            // 
            this.clear_button.Location = new System.Drawing.Point(422, 115);
            this.clear_button.Name = "clear_button";
            this.clear_button.Size = new System.Drawing.Size(117, 23);
            this.clear_button.TabIndex = 5;
            this.clear_button.Text = "Удалить всё";
            this.clear_button.UseVisualStyleBackColor = true;
            this.clear_button.Click += new System.EventHandler(this.clear_button_Click);
            // 
            // OK_button
            // 
            this.OK_button.Location = new System.Drawing.Point(383, 345);
            this.OK_button.Name = "OK_button";
            this.OK_button.Size = new System.Drawing.Size(75, 23);
            this.OK_button.TabIndex = 6;
            this.OK_button.Text = "ОК";
            this.OK_button.UseVisualStyleBackColor = true;
            this.OK_button.Click += new System.EventHandler(this.OK_button_Click);
            // 
            // Cancel_button
            // 
            this.Cancel_button.Location = new System.Drawing.Point(464, 345);
            this.Cancel_button.Name = "Cancel_button";
            this.Cancel_button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_button.TabIndex = 7;
            this.Cancel_button.Text = "Отмена";
            this.Cancel_button.UseVisualStyleBackColor = true;
            this.Cancel_button.Click += new System.EventHandler(this.Cancel_button_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Картинки|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff";
            // 
            // dir_checkBox
            // 
            this.dir_checkBox.AutoSize = true;
            this.dir_checkBox.Location = new System.Drawing.Point(12, 311);
            this.dir_checkBox.Name = "dir_checkBox";
            this.dir_checkBox.Size = new System.Drawing.Size(193, 17);
            this.dir_checkBox.TabIndex = 8;
            this.dir_checkBox.Text = "Добавлять каталоги рекурсивно";
            this.dir_checkBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 331);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Теги";
            // 
            // tags_textBox
            // 
            this.tags_textBox.Location = new System.Drawing.Point(12, 347);
            this.tags_textBox.Name = "tags_textBox";
            this.tags_textBox.Size = new System.Drawing.Size(365, 20);
            this.tags_textBox.TabIndex = 9;
            // 
            // AddTaggetFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 376);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tags_textBox);
            this.Controls.Add(this.dir_checkBox);
            this.Controls.Add(this.Cancel_button);
            this.Controls.Add(this.OK_button);
            this.Controls.Add(this.clear_button);
            this.Controls.Add(this.del_file_button);
            this.Controls.Add(this.add_dir_button);
            this.Controls.Add(this.add_file_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.files_listBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AddTaggetFileForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Добавление картинок";
            this.Load += new System.EventHandler(this.AddTaggetFileForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox files_listBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button add_file_button;
        private System.Windows.Forms.Button add_dir_button;
        private System.Windows.Forms.Button del_file_button;
        private System.Windows.Forms.Button clear_button;
        private System.Windows.Forms.Button OK_button;
        private System.Windows.Forms.Button Cancel_button;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox dir_checkBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tags_textBox;
    }
}