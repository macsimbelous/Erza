namespace Erza_Scarlet
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tags_textBox = new System.Windows.Forms.TextBox();
            this.tags_label = new System.Windows.Forms.Label();
            this.start_button = new System.Windows.Forms.Button();
            this.file_progressBar = new System.Windows.Forms.ProgressBar();
            this.all_progressBar = new System.Windows.Forms.ProgressBar();
            this.file_progres_label = new System.Windows.Forms.Label();
            this.all_progres_label = new System.Windows.Forms.Label();
            this.settings_groupBox = new System.Windows.Forms.GroupBox();
            this.db_groupBox = new System.Windows.Forms.GroupBox();
            this.add_info_db_checkBox = new System.Windows.Forms.CheckBox();
            this.select_db_button = new System.Windows.Forms.Button();
            this.connection_string_db_textBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.download_groupBox = new System.Windows.Forms.GroupBox();
            this.dovnload_povtor_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.dovnload_povtor_label = new System.Windows.Forms.Label();
            this.create_sub_dir_checkBox = new System.Windows.Forms.CheckBox();
            this.downloading_checkBox = new System.Windows.Forms.CheckBox();
            this.select_dwn_dir_button = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.download_dir_textBox = new System.Windows.Forms.TextBox();
            this.sites_groupBox = new System.Windows.Forms.GroupBox();
            this.password_textBox = new System.Windows.Forms.TextBox();
            this.login_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gelbooru_checkBox = new System.Windows.Forms.CheckBox();
            this.danbooru_checkBox = new System.Windows.Forms.CheckBox();
            this.yande_re_checkBox = new System.Windows.Forms.CheckBox();
            this.konachan_checkBox = new System.Windows.Forms.CheckBox();
            this.sankakucomplex_checkBox = new System.Windows.Forms.CheckBox();
            this.stop_button = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stat_files_label = new System.Windows.Forms.Label();
            this.stat_sites_label = new System.Windows.Forms.Label();
            this.select_dwn_folder_dialog = new System.Windows.Forms.FolderBrowserDialog();
            this.select_db_dialog = new System.Windows.Forms.OpenFileDialog();
            this.UserAgent_label = new System.Windows.Forms.Label();
            this.UserAgent_textBox = new System.Windows.Forms.TextBox();
            this.settings_groupBox.SuspendLayout();
            this.db_groupBox.SuspendLayout();
            this.download_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dovnload_povtor_numericUpDown)).BeginInit();
            this.sites_groupBox.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tags_textBox
            // 
            this.tags_textBox.Location = new System.Drawing.Point(15, 25);
            this.tags_textBox.Name = "tags_textBox";
            this.tags_textBox.Size = new System.Drawing.Size(428, 20);
            this.tags_textBox.TabIndex = 0;
            // 
            // tags_label
            // 
            this.tags_label.AutoSize = true;
            this.tags_label.Location = new System.Drawing.Point(12, 9);
            this.tags_label.Name = "tags_label";
            this.tags_label.Size = new System.Drawing.Size(31, 13);
            this.tags_label.TabIndex = 1;
            this.tags_label.Text = "Теги";
            // 
            // start_button
            // 
            this.start_button.Location = new System.Drawing.Point(452, 23);
            this.start_button.Name = "start_button";
            this.start_button.Size = new System.Drawing.Size(75, 23);
            this.start_button.TabIndex = 2;
            this.start_button.Text = "Старт";
            this.start_button.UseVisualStyleBackColor = true;
            this.start_button.Click += new System.EventHandler(this.start_button_Click);
            // 
            // file_progressBar
            // 
            this.file_progressBar.Location = new System.Drawing.Point(15, 64);
            this.file_progressBar.Name = "file_progressBar";
            this.file_progressBar.Size = new System.Drawing.Size(593, 23);
            this.file_progressBar.TabIndex = 3;
            // 
            // all_progressBar
            // 
            this.all_progressBar.Location = new System.Drawing.Point(15, 106);
            this.all_progressBar.Name = "all_progressBar";
            this.all_progressBar.Size = new System.Drawing.Size(593, 23);
            this.all_progressBar.TabIndex = 4;
            // 
            // file_progres_label
            // 
            this.file_progres_label.AutoSize = true;
            this.file_progres_label.Location = new System.Drawing.Point(12, 48);
            this.file_progres_label.Name = "file_progres_label";
            this.file_progres_label.Size = new System.Drawing.Size(36, 13);
            this.file_progres_label.TabIndex = 5;
            this.file_progres_label.Text = "Фаил";
            // 
            // all_progres_label
            // 
            this.all_progres_label.AutoSize = true;
            this.all_progres_label.Location = new System.Drawing.Point(12, 90);
            this.all_progres_label.Name = "all_progres_label";
            this.all_progres_label.Size = new System.Drawing.Size(37, 13);
            this.all_progres_label.TabIndex = 6;
            this.all_progres_label.Text = "Всего";
            // 
            // settings_groupBox
            // 
            this.settings_groupBox.Controls.Add(this.db_groupBox);
            this.settings_groupBox.Controls.Add(this.download_groupBox);
            this.settings_groupBox.Controls.Add(this.sites_groupBox);
            this.settings_groupBox.Location = new System.Drawing.Point(15, 189);
            this.settings_groupBox.Name = "settings_groupBox";
            this.settings_groupBox.Size = new System.Drawing.Size(593, 306);
            this.settings_groupBox.TabIndex = 7;
            this.settings_groupBox.TabStop = false;
            this.settings_groupBox.Text = "Настройки";
            // 
            // db_groupBox
            // 
            this.db_groupBox.Controls.Add(this.add_info_db_checkBox);
            this.db_groupBox.Controls.Add(this.select_db_button);
            this.db_groupBox.Controls.Add(this.connection_string_db_textBox);
            this.db_groupBox.Controls.Add(this.label5);
            this.db_groupBox.Location = new System.Drawing.Point(6, 213);
            this.db_groupBox.Name = "db_groupBox";
            this.db_groupBox.Size = new System.Drawing.Size(581, 83);
            this.db_groupBox.TabIndex = 2;
            this.db_groupBox.TabStop = false;
            this.db_groupBox.Text = "Базаданных";
            // 
            // add_info_db_checkBox
            // 
            this.add_info_db_checkBox.AutoSize = true;
            this.add_info_db_checkBox.Location = new System.Drawing.Point(6, 19);
            this.add_info_db_checkBox.Name = "add_info_db_checkBox";
            this.add_info_db_checkBox.Size = new System.Drawing.Size(154, 17);
            this.add_info_db_checkBox.TabIndex = 3;
            this.add_info_db_checkBox.Text = "Добавлять в базуданных";
            this.add_info_db_checkBox.UseVisualStyleBackColor = true;
            // 
            // select_db_button
            // 
            this.select_db_button.Location = new System.Drawing.Point(496, 53);
            this.select_db_button.Name = "select_db_button";
            this.select_db_button.Size = new System.Drawing.Size(75, 23);
            this.select_db_button.TabIndex = 2;
            this.select_db_button.Text = "Выбрать";
            this.select_db_button.UseVisualStyleBackColor = true;
            this.select_db_button.Click += new System.EventHandler(this.select_db_button_Click);
            // 
            // connection_string_db_textBox
            // 
            this.connection_string_db_textBox.Location = new System.Drawing.Point(6, 55);
            this.connection_string_db_textBox.Name = "connection_string_db_textBox";
            this.connection_string_db_textBox.Size = new System.Drawing.Size(484, 20);
            this.connection_string_db_textBox.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Строка подключения";
            // 
            // download_groupBox
            // 
            this.download_groupBox.Controls.Add(this.dovnload_povtor_numericUpDown);
            this.download_groupBox.Controls.Add(this.dovnload_povtor_label);
            this.download_groupBox.Controls.Add(this.create_sub_dir_checkBox);
            this.download_groupBox.Controls.Add(this.downloading_checkBox);
            this.download_groupBox.Controls.Add(this.select_dwn_dir_button);
            this.download_groupBox.Controls.Add(this.label4);
            this.download_groupBox.Controls.Add(this.download_dir_textBox);
            this.download_groupBox.Location = new System.Drawing.Point(6, 123);
            this.download_groupBox.Name = "download_groupBox";
            this.download_groupBox.Size = new System.Drawing.Size(581, 84);
            this.download_groupBox.TabIndex = 1;
            this.download_groupBox.TabStop = false;
            this.download_groupBox.Text = "Загрузка";
            // 
            // dovnload_povtor_numericUpDown
            // 
            this.dovnload_povtor_numericUpDown.Location = new System.Drawing.Point(329, 17);
            this.dovnload_povtor_numericUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.dovnload_povtor_numericUpDown.Name = "dovnload_povtor_numericUpDown";
            this.dovnload_povtor_numericUpDown.Size = new System.Drawing.Size(40, 20);
            this.dovnload_povtor_numericUpDown.TabIndex = 7;
            // 
            // dovnload_povtor_label
            // 
            this.dovnload_povtor_label.AutoSize = true;
            this.dovnload_povtor_label.Location = new System.Drawing.Point(238, 20);
            this.dovnload_povtor_label.Name = "dovnload_povtor_label";
            this.dovnload_povtor_label.Size = new System.Drawing.Size(85, 13);
            this.dovnload_povtor_label.TabIndex = 6;
            this.dovnload_povtor_label.Text = "Число попыток";
            // 
            // create_sub_dir_checkBox
            // 
            this.create_sub_dir_checkBox.AutoSize = true;
            this.create_sub_dir_checkBox.Location = new System.Drawing.Point(91, 19);
            this.create_sub_dir_checkBox.Name = "create_sub_dir_checkBox";
            this.create_sub_dir_checkBox.Size = new System.Drawing.Size(141, 17);
            this.create_sub_dir_checkBox.TabIndex = 4;
            this.create_sub_dir_checkBox.Text = "Создавать подкаталог";
            this.create_sub_dir_checkBox.UseVisualStyleBackColor = true;
            // 
            // downloading_checkBox
            // 
            this.downloading_checkBox.AutoSize = true;
            this.downloading_checkBox.Location = new System.Drawing.Point(6, 19);
            this.downloading_checkBox.Name = "downloading_checkBox";
            this.downloading_checkBox.Size = new System.Drawing.Size(79, 17);
            this.downloading_checkBox.TabIndex = 3;
            this.downloading_checkBox.Text = "Скачивать";
            this.downloading_checkBox.UseVisualStyleBackColor = true;
            // 
            // select_dwn_dir_button
            // 
            this.select_dwn_dir_button.Location = new System.Drawing.Point(496, 53);
            this.select_dwn_dir_button.Name = "select_dwn_dir_button";
            this.select_dwn_dir_button.Size = new System.Drawing.Size(75, 23);
            this.select_dwn_dir_button.TabIndex = 2;
            this.select_dwn_dir_button.Text = "Выбрать";
            this.select_dwn_dir_button.UseVisualStyleBackColor = true;
            this.select_dwn_dir_button.Click += new System.EventHandler(this.select_dwn_dir_button_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Каталог для загрузки";
            // 
            // download_dir_textBox
            // 
            this.download_dir_textBox.Location = new System.Drawing.Point(6, 55);
            this.download_dir_textBox.Name = "download_dir_textBox";
            this.download_dir_textBox.Size = new System.Drawing.Size(484, 20);
            this.download_dir_textBox.TabIndex = 0;
            // 
            // sites_groupBox
            // 
            this.sites_groupBox.Controls.Add(this.UserAgent_textBox);
            this.sites_groupBox.Controls.Add(this.UserAgent_label);
            this.sites_groupBox.Controls.Add(this.password_textBox);
            this.sites_groupBox.Controls.Add(this.login_textBox);
            this.sites_groupBox.Controls.Add(this.label2);
            this.sites_groupBox.Controls.Add(this.label1);
            this.sites_groupBox.Controls.Add(this.gelbooru_checkBox);
            this.sites_groupBox.Controls.Add(this.danbooru_checkBox);
            this.sites_groupBox.Controls.Add(this.yande_re_checkBox);
            this.sites_groupBox.Controls.Add(this.konachan_checkBox);
            this.sites_groupBox.Controls.Add(this.sankakucomplex_checkBox);
            this.sites_groupBox.Location = new System.Drawing.Point(6, 19);
            this.sites_groupBox.Name = "sites_groupBox";
            this.sites_groupBox.Size = new System.Drawing.Size(581, 99);
            this.sites_groupBox.TabIndex = 0;
            this.sites_groupBox.TabStop = false;
            this.sites_groupBox.Text = "Сайты";
            // 
            // password_textBox
            // 
            this.password_textBox.Location = new System.Drawing.Point(278, 41);
            this.password_textBox.Name = "password_textBox";
            this.password_textBox.Size = new System.Drawing.Size(293, 20);
            this.password_textBox.TabIndex = 8;
            // 
            // login_textBox
            // 
            this.login_textBox.Location = new System.Drawing.Point(47, 41);
            this.login_textBox.Name = "login_textBox";
            this.login_textBox.Size = new System.Drawing.Size(174, 20);
            this.login_textBox.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Пароль";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Логин";
            // 
            // gelbooru_checkBox
            // 
            this.gelbooru_checkBox.AutoSize = true;
            this.gelbooru_checkBox.Location = new System.Drawing.Point(468, 20);
            this.gelbooru_checkBox.Name = "gelbooru_checkBox";
            this.gelbooru_checkBox.Size = new System.Drawing.Size(90, 17);
            this.gelbooru_checkBox.TabIndex = 4;
            this.gelbooru_checkBox.Text = "gelbooru.com";
            this.gelbooru_checkBox.UseVisualStyleBackColor = true;
            // 
            // danbooru_checkBox
            // 
            this.danbooru_checkBox.AutoSize = true;
            this.danbooru_checkBox.Location = new System.Drawing.Point(340, 20);
            this.danbooru_checkBox.Name = "danbooru_checkBox";
            this.danbooru_checkBox.Size = new System.Drawing.Size(122, 17);
            this.danbooru_checkBox.TabIndex = 3;
            this.danbooru_checkBox.Text = "danbooru.donmai.us";
            this.danbooru_checkBox.UseVisualStyleBackColor = true;
            // 
            // yande_re_checkBox
            // 
            this.yande_re_checkBox.AutoSize = true;
            this.yande_re_checkBox.Location = new System.Drawing.Point(267, 20);
            this.yande_re_checkBox.Name = "yande_re_checkBox";
            this.yande_re_checkBox.Size = new System.Drawing.Size(67, 17);
            this.yande_re_checkBox.TabIndex = 2;
            this.yande_re_checkBox.Text = "yande.re";
            this.yande_re_checkBox.UseVisualStyleBackColor = true;
            // 
            // konachan_checkBox
            // 
            this.konachan_checkBox.AutoSize = true;
            this.konachan_checkBox.Location = new System.Drawing.Point(169, 20);
            this.konachan_checkBox.Name = "konachan_checkBox";
            this.konachan_checkBox.Size = new System.Drawing.Size(92, 17);
            this.konachan_checkBox.TabIndex = 1;
            this.konachan_checkBox.Text = "konachan.net";
            this.konachan_checkBox.UseVisualStyleBackColor = true;
            // 
            // sankakucomplex_checkBox
            // 
            this.sankakucomplex_checkBox.AutoSize = true;
            this.sankakucomplex_checkBox.Location = new System.Drawing.Point(7, 20);
            this.sankakucomplex_checkBox.Name = "sankakucomplex_checkBox";
            this.sankakucomplex_checkBox.Size = new System.Drawing.Size(156, 17);
            this.sankakucomplex_checkBox.TabIndex = 0;
            this.sankakucomplex_checkBox.Text = "chan.sankakucomplex.com";
            this.sankakucomplex_checkBox.UseVisualStyleBackColor = true;
            // 
            // stop_button
            // 
            this.stop_button.Location = new System.Drawing.Point(533, 23);
            this.stop_button.Name = "stop_button";
            this.stop_button.Size = new System.Drawing.Size(75, 23);
            this.stop_button.TabIndex = 8;
            this.stop_button.Text = "Стоп";
            this.stop_button.UseVisualStyleBackColor = true;
            this.stop_button.Click += new System.EventHandler(this.stop_button_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 500);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(622, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(52, 17);
            this.toolStripStatusLabel1.Text = "Начало ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.stat_files_label);
            this.groupBox1.Controls.Add(this.stat_sites_label);
            this.groupBox1.Location = new System.Drawing.Point(15, 133);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(593, 52);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Статистика";
            // 
            // stat_files_label
            // 
            this.stat_files_label.AutoSize = true;
            this.stat_files_label.Location = new System.Drawing.Point(6, 32);
            this.stat_files_label.Name = "stat_files_label";
            this.stat_files_label.Size = new System.Drawing.Size(47, 13);
            this.stat_files_label.TabIndex = 1;
            this.stat_files_label.Text = "Файлы:";
            // 
            // stat_sites_label
            // 
            this.stat_sites_label.AutoSize = true;
            this.stat_sites_label.Location = new System.Drawing.Point(6, 16);
            this.stat_sites_label.Name = "stat_sites_label";
            this.stat_sites_label.Size = new System.Drawing.Size(45, 13);
            this.stat_sites_label.TabIndex = 0;
            this.stat_sites_label.Text = "Сайты: ";
            // 
            // select_db_dialog
            // 
            this.select_db_dialog.FileName = "openFileDialog1";
            // 
            // UserAgent_label
            // 
            this.UserAgent_label.AutoSize = true;
            this.UserAgent_label.Location = new System.Drawing.Point(4, 73);
            this.UserAgent_label.Name = "UserAgent_label";
            this.UserAgent_label.Size = new System.Drawing.Size(57, 13);
            this.UserAgent_label.TabIndex = 9;
            this.UserAgent_label.Text = "UserAgent";
            // 
            // UserAgent_textBox
            // 
            this.UserAgent_textBox.Location = new System.Drawing.Point(67, 70);
            this.UserAgent_textBox.Name = "UserAgent_textBox";
            this.UserAgent_textBox.Size = new System.Drawing.Size(504, 20);
            this.UserAgent_textBox.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 522);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.stop_button);
            this.Controls.Add(this.settings_groupBox);
            this.Controls.Add(this.all_progres_label);
            this.Controls.Add(this.file_progres_label);
            this.Controls.Add(this.all_progressBar);
            this.Controls.Add(this.file_progressBar);
            this.Controls.Add(this.start_button);
            this.Controls.Add(this.tags_label);
            this.Controls.Add(this.tags_textBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Эрза Скарлет";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.settings_groupBox.ResumeLayout(false);
            this.db_groupBox.ResumeLayout(false);
            this.db_groupBox.PerformLayout();
            this.download_groupBox.ResumeLayout(false);
            this.download_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dovnload_povtor_numericUpDown)).EndInit();
            this.sites_groupBox.ResumeLayout(false);
            this.sites_groupBox.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tags_textBox;
        private System.Windows.Forms.Label tags_label;
        private System.Windows.Forms.Button start_button;
        private System.Windows.Forms.ProgressBar file_progressBar;
        private System.Windows.Forms.ProgressBar all_progressBar;
        private System.Windows.Forms.Label file_progres_label;
        private System.Windows.Forms.Label all_progres_label;
        private System.Windows.Forms.GroupBox settings_groupBox;
        private System.Windows.Forms.GroupBox db_groupBox;
        private System.Windows.Forms.Button select_db_button;
        private System.Windows.Forms.TextBox connection_string_db_textBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox download_groupBox;
        private System.Windows.Forms.Button select_dwn_dir_button;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox download_dir_textBox;
        private System.Windows.Forms.GroupBox sites_groupBox;
        private System.Windows.Forms.CheckBox danbooru_checkBox;
        private System.Windows.Forms.CheckBox yande_re_checkBox;
        private System.Windows.Forms.CheckBox konachan_checkBox;
        private System.Windows.Forms.CheckBox sankakucomplex_checkBox;
        private System.Windows.Forms.CheckBox gelbooru_checkBox;
        private System.Windows.Forms.CheckBox add_info_db_checkBox;
        private System.Windows.Forms.CheckBox create_sub_dir_checkBox;
        private System.Windows.Forms.CheckBox downloading_checkBox;
        private System.Windows.Forms.Button stop_button;
        private System.Windows.Forms.TextBox password_textBox;
        private System.Windows.Forms.TextBox login_textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown dovnload_povtor_numericUpDown;
        private System.Windows.Forms.Label dovnload_povtor_label;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label stat_files_label;
        private System.Windows.Forms.Label stat_sites_label;
        private System.Windows.Forms.FolderBrowserDialog select_dwn_folder_dialog;
        private System.Windows.Forms.OpenFileDialog select_db_dialog;
        private System.Windows.Forms.TextBox UserAgent_textBox;
        private System.Windows.Forms.Label UserAgent_label;
    }
}

