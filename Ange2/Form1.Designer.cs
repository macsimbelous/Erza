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
            add_tag_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            copytagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            search_button = new System.Windows.Forms.Button();
            slide_show_button = new System.Windows.Forms.Button();
            imageListView1 = new Manina.Windows.Forms.ImageListView();
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            autocompleteMenu1 = new AutocompleteMenuNS.AutocompleteMenu();
            tags_textBox = new System.Windows.Forms.TextBox();
            option_comboBox = new System.Windows.Forms.ComboBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            contextMenuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { view_in_window_ToolStripMenuItem, view_fullscreen_ToolStripMenuItem, openOuterSoftToolStripMenuItem, open_in_explorer_toolStripMenuItem1, add_tag_ToolStripMenuItem, copytagToolStripMenuItem, slideshowToolStripMenuItem, copyhashToolStripMenuItem, find_similar_ToolStripMenuItem, toolStripSeparator2, copytowallToolStripMenuItem, copytodirToolStripMenuItem, copyAllToDirToolStripMenuItem, MoveAllToDirToolStripMenuItem, recreate_preview_ToolStripMenuItem, toolStripSeparator1, deleteToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(357, 496);
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
            // add_tag_ToolStripMenuItem
            // 
            add_tag_ToolStripMenuItem.Name = "add_tag_ToolStripMenuItem";
            add_tag_ToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            add_tag_ToolStripMenuItem.Text = "Добавить теги";
            add_tag_ToolStripMenuItem.Click += add_tag_ToolStripMenuItem_Click;
            // 
            // copytagToolStripMenuItem
            // 
            copytagToolStripMenuItem.Name = "copytagToolStripMenuItem";
            copytagToolStripMenuItem.Size = new System.Drawing.Size(356, 32);
            copytagToolStripMenuItem.Text = "Копировать теги";
            copytagToolStripMenuItem.Click += copytagToolStripMenuItem_Click;
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
            statusStrip1.Size = new System.Drawing.Size(1924, 32);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(204, 25);
            toolStripStatusLabel1.Text = "Изображений найдено:";
            // 
            // search_button
            // 
            search_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            search_button.Image = (System.Drawing.Image)resources.GetObject("search_button.Image");
            search_button.Location = new System.Drawing.Point(1798, 7);
            search_button.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            search_button.Name = "search_button";
            search_button.Size = new System.Drawing.Size(53, 40);
            search_button.TabIndex = 10;
            toolTip1.SetToolTip(search_button, "Поиск");
            search_button.UseVisualStyleBackColor = true;
            search_button.Click += search_button_Click;
            // 
            // slide_show_button
            // 
            slide_show_button.Image = Properties.Resources.images25;
            slide_show_button.Location = new System.Drawing.Point(1859, 7);
            slide_show_button.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            slide_show_button.Name = "slide_show_button";
            slide_show_button.Size = new System.Drawing.Size(53, 42);
            slide_show_button.TabIndex = 11;
            toolTip1.SetToolTip(slide_show_button, "Слайд шоу");
            slide_show_button.UseVisualStyleBackColor = true;
            slide_show_button.Click += slide_show_button_Click;
            // 
            // imageListView1
            // 
            imageListView1.ContextMenuStrip = contextMenuStrip1;
            imageListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            imageListView1.Location = new System.Drawing.Point(4, 65);
            imageListView1.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            imageListView1.Name = "imageListView1";
            imageListView1.PersistentCacheDirectory = "";
            imageListView1.PersistentCacheSize = 100L;
            imageListView1.Size = new System.Drawing.Size(1916, 958);
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
            autocompleteMenu1.SetAutocompleteMenu(tags_textBox, autocompleteMenu1);
            tags_textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            tags_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            tags_textBox.Location = new System.Drawing.Point(4, 7);
            tags_textBox.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            tags_textBox.Name = "tags_textBox";
            tags_textBox.Size = new System.Drawing.Size(1578, 35);
            tags_textBox.TabIndex = 8;
            tags_textBox.KeyDown += tags_textBox_KeyDown;
            // 
            // option_comboBox
            // 
            option_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            option_comboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            option_comboBox.FormattingEnabled = true;
            option_comboBox.Items.AddRange(new object[] { "Теги", "Теги ИЛИ", "Часть тега", "MD5" });
            option_comboBox.Location = new System.Drawing.Point(1590, 7);
            option_comboBox.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            option_comboBox.Name = "option_comboBox";
            option_comboBox.Size = new System.Drawing.Size(200, 37);
            option_comboBox.TabIndex = 9;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(imageListView1, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(1924, 1030);
            tableLayoutPanel1.TabIndex = 12;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel2.Controls.Add(tags_textBox, 0, 0);
            tableLayoutPanel2.Controls.Add(option_comboBox, 1, 0);
            tableLayoutPanel2.Controls.Add(slide_show_button, 3, 0);
            tableLayoutPanel2.Controls.Add(search_button, 2, 0);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(4, 5);
            tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            tableLayoutPanel2.Size = new System.Drawing.Size(1916, 48);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1924, 1062);
            Controls.Add(tableLayoutPanel1);
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
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem view_fullscreen_ToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem add_tag_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copytagToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}

