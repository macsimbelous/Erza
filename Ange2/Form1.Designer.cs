namespace Ange
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            view_in_window_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            view_fullscreen_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            openOuterSoftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            open_in_explorer_toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            edittagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            slideshowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyhashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            find_similar_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            copytowallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copytodirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copyAllToDirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            MoveAllToDirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            recreate_preview_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            imageList1 = new System.Windows.Forms.ImageList(components);
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            imageListView1 = new Manina.Windows.Forms.ImageListView();
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            autocompleteMenu1 = new AutocompleteMenuNS.AutocompleteMenu();
            tags_textBox = new System.Windows.Forms.TextBox();
            option_comboBox = new System.Windows.Forms.ComboBox();
            search_button = new System.Windows.Forms.Button();
            slide_show_button = new System.Windows.Forms.Button();
            contextMenuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { view_in_window_ToolStripMenuItem, view_fullscreen_ToolStripMenuItem, openOuterSoftToolStripMenuItem, open_in_explorer_toolStripMenuItem1, edittagsToolStripMenuItem, slideshowToolStripMenuItem, copyhashToolStripMenuItem, find_similar_ToolStripMenuItem, toolStripSeparator2, copytowallToolStripMenuItem, copytodirToolStripMenuItem, copyAllToDirToolStripMenuItem, MoveAllToDirToolStripMenuItem, recreate_preview_ToolStripMenuItem, toolStripSeparator1, deleteToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(357, 464);
            // 
            // view_in_window_ToolStripMenuItem
            // 
            view_in_window_ToolStripMenuItem.Name = "view_in_window_ToolStripMenuItem";
            view_in_window_ToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            view_in_window_ToolStripMenuItem.Text = "Просмотр в окне";
            view_in_window_ToolStripMenuItem.Click += view_in_window_ToolStripMenuItem_Click;
            // 
            // view_fullscreen_ToolStripMenuItem
            // 
            view_fullscreen_ToolStripMenuItem.Name = "view_fullscreen_ToolStripMenuItem";
            view_fullscreen_ToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            view_fullscreen_ToolStripMenuItem.Text = "Просморт на полный экран";
            view_fullscreen_ToolStripMenuItem.Click += view_fullscreen_ToolStripMenuItem_Click;
            // 
            // openOuterSoftToolStripMenuItem
            // 
            openOuterSoftToolStripMenuItem.Name = "openOuterSoftToolStripMenuItem";
            openOuterSoftToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            openOuterSoftToolStripMenuItem.Text = "Открыть во внешней программе";
            openOuterSoftToolStripMenuItem.Click += openOuterSoftToolStripMenuItem_Click;
            // 
            // open_in_explorer_toolStripMenuItem1
            // 
            open_in_explorer_toolStripMenuItem1.Name = "open_in_explorer_toolStripMenuItem1";
            open_in_explorer_toolStripMenuItem1.Size = new System.Drawing.Size(356, 32);
            open_in_explorer_toolStripMenuItem1.Text = "Открыть в проводнике";
            open_in_explorer_toolStripMenuItem1.Click += open_in_explorer_toolStripMenuItem1_Click;
            // 
            // edittagsToolStripMenuItem
            // 
            edittagsToolStripMenuItem.Name = "edittagsToolStripMenuItem";
            edittagsToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            edittagsToolStripMenuItem.Text = "Редактировать теги";
            edittagsToolStripMenuItem.Click += edittagsToolStripMenuItem_Click;
            // 
            // slideshowToolStripMenuItem
            // 
            slideshowToolStripMenuItem.Name = "slideshowToolStripMenuItem";
            slideshowToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            slideshowToolStripMenuItem.Text = "Слайдшоу";
            slideshowToolStripMenuItem.Click += slideshowToolStripMenuItem_Click;
            // 
            // copyhashToolStripMenuItem
            // 
            copyhashToolStripMenuItem.Name = "copyhashToolStripMenuItem";
            copyhashToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            copyhashToolStripMenuItem.Text = "Копировать хэш";
            copyhashToolStripMenuItem.Click += copyhashToolStripMenuItem_Click;
            // 
            // find_similar_ToolStripMenuItem
            // 
            find_similar_ToolStripMenuItem.Name = "find_similar_ToolStripMenuItem";
            find_similar_ToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            find_similar_ToolStripMenuItem.Text = "Найти похожие";
            find_similar_ToolStripMenuItem.Click += find_similar_ToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(353, 6);
            // 
            // copytowallToolStripMenuItem
            // 
            copytowallToolStripMenuItem.Name = "copytowallToolStripMenuItem";
            copytowallToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            copytowallToolStripMenuItem.Text = "Копировать в обои";
            copytowallToolStripMenuItem.Click += copytowallToolStripMenuItem_Click;
            // 
            // copytodirToolStripMenuItem
            // 
            copytodirToolStripMenuItem.Name = "copytodirToolStripMenuItem";
            copytodirToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            copytodirToolStripMenuItem.Text = "Копировать в каталог";
            copytodirToolStripMenuItem.Click += copytodirToolStripMenuItem_Click;
            // 
            // copyAllToDirToolStripMenuItem
            // 
            copyAllToDirToolStripMenuItem.Name = "copyAllToDirToolStripMenuItem";
            copyAllToDirToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            copyAllToDirToolStripMenuItem.Text = "Копировать всё в каталог";
            copyAllToDirToolStripMenuItem.Click += copyAllToDirToolStripMenuItem_Click;
            // 
            // MoveAllToDirToolStripMenuItem
            // 
            MoveAllToDirToolStripMenuItem.Name = "MoveAllToDirToolStripMenuItem";
            MoveAllToDirToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            MoveAllToDirToolStripMenuItem.Text = "Переместить всё в каталог";
            MoveAllToDirToolStripMenuItem.Click += MoveAllToDirToolStripMenuItem_Click;
            // 
            // recreate_preview_ToolStripMenuItem
            // 
            recreate_preview_ToolStripMenuItem.Name = "recreate_preview_ToolStripMenuItem";
            recreate_preview_ToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            recreate_preview_ToolStripMenuItem.Text = "Пересоздать эскиз";
            recreate_preview_ToolStripMenuItem.Click += recreate_preview_ToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(353, 6);
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            deleteToolStripMenuItem.Text = "Удалить";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            imageList1.ImageSize = new System.Drawing.Size(16, 16);
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new System.Drawing.Point(0, 1030);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(3, 0, 23, 0);
            statusStrip1.Size = new System.Drawing.Size(1988, 32);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(204, 25);
            toolStripStatusLabel1.Text = "Изображений найдено:";
            // 
            // imageListView1
            // 
            imageListView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            imageListView1.ContextMenuStrip = contextMenuStrip1;
            imageListView1.Location = new System.Drawing.Point(0, 61);
            imageListView1.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            imageListView1.Name = "imageListView1";
            imageListView1.PersistentCacheDirectory = "";
            imageListView1.PersistentCacheSize = 100L;
            imageListView1.Size = new System.Drawing.Size(1988, 962);
            imageListView1.TabIndex = 7;
            imageListView1.ThumbnailSize = new System.Drawing.Size(300, 225);
            imageListView1.UseWIC = true;
            imageListView1.ItemHover += imageListView1_ItemHover;
            imageListView1.ItemDoubleClick += imageListView1_ItemDoubleClick;
            imageListView1.KeyDown += imageListView1_KeyDown;
            // 
            // autocompleteMenu1
            // 
            autocompleteMenu1.Colors = (AutocompleteMenuNS.Colors)resources.GetObject("autocompleteMenu1.Colors");
            autocompleteMenu1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            autocompleteMenu1.ImageList = null;
            autocompleteMenu1.TargetControlWrapper = null;
            // 
            // tags_textBox
            // 
            tags_textBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            autocompleteMenu1.SetAutocompleteMenu(tags_textBox, autocompleteMenu1);
            tags_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            tags_textBox.Location = new System.Drawing.Point(0, 7);
            tags_textBox.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            tags_textBox.Name = "tags_textBox";
            tags_textBox.Size = new System.Drawing.Size(1645, 40);
            tags_textBox.TabIndex = 8;
            tags_textBox.KeyDown += tags_textBox_KeyDown;
            // 
            // option_comboBox
            // 
            option_comboBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            option_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            option_comboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold);
            option_comboBox.FormattingEnabled = true;
            option_comboBox.Items.AddRange(new object[] { "Теги", "Теги ИЛИ", "Часть тега", "MD5" });
            option_comboBox.Location = new System.Drawing.Point(1653, 6);
            option_comboBox.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            option_comboBox.Name = "option_comboBox";
            option_comboBox.Size = new System.Drawing.Size(200, 41);
            option_comboBox.TabIndex = 9;
            // 
            // search_button
            // 
            search_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            search_button.Image = Properties.Resources.search25;
            search_button.Location = new System.Drawing.Point(1861, -2);
            search_button.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            search_button.Name = "search_button";
            search_button.Size = new System.Drawing.Size(53, 62);
            search_button.TabIndex = 10;
            search_button.UseVisualStyleBackColor = true;
            search_button.Click += search_button_Click;
            // 
            // slide_show_button
            // 
            slide_show_button.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            slide_show_button.Image = Properties.Resources.images25;
            slide_show_button.Location = new System.Drawing.Point(1922, -2);
            slide_show_button.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            slide_show_button.Name = "slide_show_button";
            slide_show_button.Size = new System.Drawing.Size(53, 62);
            slide_show_button.TabIndex = 11;
            slide_show_button.UseVisualStyleBackColor = true;
            slide_show_button.Click += slide_show_button_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1988, 1062);
            Controls.Add(slide_show_button);
            Controls.Add(search_button);
            Controls.Add(option_comboBox);
            Controls.Add(tags_textBox);
            Controls.Add(imageListView1);
            Controls.Add(statusStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            Name = "Form1";
            Text = "Ange";
            FormClosing += Form1_FormClosing;
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            contextMenuStrip1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem view_fullscreen_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edittagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slideshowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copytowallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem copyhashToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copytodirToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem openOuterSoftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAllToDirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveAllToDirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem view_in_window_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recreate_preview_ToolStripMenuItem;
        private Manina.Windows.Forms.ImageListView imageListView1;
        private AutocompleteMenuNS.AutocompleteMenu autocompleteMenu1;
        private System.Windows.Forms.TextBox tags_textBox;
        private System.Windows.Forms.ComboBox option_comboBox;
        private System.Windows.Forms.Button search_button;
        private System.Windows.Forms.Button slide_show_button;
        private System.Windows.Forms.ToolStripMenuItem find_similar_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem open_in_explorer_toolStripMenuItem1;
    }
}

