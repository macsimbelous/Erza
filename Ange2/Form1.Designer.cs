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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.view_in_window_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.view_fullscreen_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openOuterSoftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edittagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slideshowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyhashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copytowallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copytodirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAllToDirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveAllToDirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recreate_preview_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.autocompleteMenu1 = new AutocompleteMenuNS.AutocompleteMenu();
            this.tags_textBox = new System.Windows.Forms.TextBox();
            this.option_comboBox = new System.Windows.Forms.ComboBox();
            this.search_button = new System.Windows.Forms.Button();
            this.slide_show_button = new System.Windows.Forms.Button();
            this.imageListView1 = new Manina.Windows.Forms.ImageListView();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.view_in_window_ToolStripMenuItem,
            this.view_fullscreen_ToolStripMenuItem,
            this.openOuterSoftToolStripMenuItem,
            this.edittagsToolStripMenuItem,
            this.slideshowToolStripMenuItem,
            this.copyhashToolStripMenuItem,
            this.copytowallToolStripMenuItem,
            this.copytodirToolStripMenuItem,
            this.copyAllToDirToolStripMenuItem,
            this.MoveAllToDirToolStripMenuItem,
            this.recreate_preview_ToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(257, 268);
            // 
            // view_in_window_ToolStripMenuItem
            // 
            this.view_in_window_ToolStripMenuItem.Name = "view_in_window_ToolStripMenuItem";
            this.view_in_window_ToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.view_in_window_ToolStripMenuItem.Text = "Просмотр в окне";
            this.view_in_window_ToolStripMenuItem.Click += new System.EventHandler(this.view_in_window_ToolStripMenuItem_Click);
            // 
            // view_fullscreen_ToolStripMenuItem
            // 
            this.view_fullscreen_ToolStripMenuItem.Name = "view_fullscreen_ToolStripMenuItem";
            this.view_fullscreen_ToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.view_fullscreen_ToolStripMenuItem.Text = "Просморт на полный экран";
            this.view_fullscreen_ToolStripMenuItem.Click += new System.EventHandler(this.view_fullscreen_ToolStripMenuItem_Click);
            // 
            // openOuterSoftToolStripMenuItem
            // 
            this.openOuterSoftToolStripMenuItem.Name = "openOuterSoftToolStripMenuItem";
            this.openOuterSoftToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.openOuterSoftToolStripMenuItem.Text = "Открыть во внешней программе";
            this.openOuterSoftToolStripMenuItem.Click += new System.EventHandler(this.openOuterSoftToolStripMenuItem_Click);
            // 
            // edittagsToolStripMenuItem
            // 
            this.edittagsToolStripMenuItem.Name = "edittagsToolStripMenuItem";
            this.edittagsToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.edittagsToolStripMenuItem.Text = "Редактировать теги";
            this.edittagsToolStripMenuItem.Click += new System.EventHandler(this.edittagsToolStripMenuItem_Click);
            // 
            // slideshowToolStripMenuItem
            // 
            this.slideshowToolStripMenuItem.Name = "slideshowToolStripMenuItem";
            this.slideshowToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.slideshowToolStripMenuItem.Text = "Слайдшоу";
            this.slideshowToolStripMenuItem.Click += new System.EventHandler(this.slideshowToolStripMenuItem_Click);
            // 
            // copyhashToolStripMenuItem
            // 
            this.copyhashToolStripMenuItem.Name = "copyhashToolStripMenuItem";
            this.copyhashToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.copyhashToolStripMenuItem.Text = "Копировать хэш";
            this.copyhashToolStripMenuItem.Click += new System.EventHandler(this.copyhashToolStripMenuItem_Click);
            // 
            // copytowallToolStripMenuItem
            // 
            this.copytowallToolStripMenuItem.Name = "copytowallToolStripMenuItem";
            this.copytowallToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.copytowallToolStripMenuItem.Text = "Копировать в обои";
            this.copytowallToolStripMenuItem.Click += new System.EventHandler(this.copytowallToolStripMenuItem_Click);
            // 
            // copytodirToolStripMenuItem
            // 
            this.copytodirToolStripMenuItem.Name = "copytodirToolStripMenuItem";
            this.copytodirToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.copytodirToolStripMenuItem.Text = "Копировать в каталог";
            this.copytodirToolStripMenuItem.Click += new System.EventHandler(this.copytodirToolStripMenuItem_Click);
            // 
            // copyAllToDirToolStripMenuItem
            // 
            this.copyAllToDirToolStripMenuItem.Name = "copyAllToDirToolStripMenuItem";
            this.copyAllToDirToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.copyAllToDirToolStripMenuItem.Text = "Копировать всё в каталог";
            this.copyAllToDirToolStripMenuItem.Click += new System.EventHandler(this.copyAllToDirToolStripMenuItem_Click);
            // 
            // MoveAllToDirToolStripMenuItem
            // 
            this.MoveAllToDirToolStripMenuItem.Name = "MoveAllToDirToolStripMenuItem";
            this.MoveAllToDirToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.MoveAllToDirToolStripMenuItem.Text = "Переместить всё в каталог";
            this.MoveAllToDirToolStripMenuItem.Click += new System.EventHandler(this.MoveAllToDirToolStripMenuItem_Click);
            // 
            // recreate_preview_ToolStripMenuItem
            // 
            this.recreate_preview_ToolStripMenuItem.Name = "recreate_preview_ToolStripMenuItem";
            this.recreate_preview_ToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.recreate_preview_ToolStripMenuItem.Text = "Пересоздать эскиз";
            this.recreate_preview_ToolStripMenuItem.Click += new System.EventHandler(this.recreate_preview_ToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.deleteToolStripMenuItem.Text = "Удалить";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 668);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1283, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(136, 17);
            this.toolStripStatusLabel1.Text = "Изображений найдено:";
            // 
            // autocompleteMenu1
            // 
            this.autocompleteMenu1.Colors = ((AutocompleteMenuNS.Colors)(resources.GetObject("autocompleteMenu1.Colors")));
            this.autocompleteMenu1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.autocompleteMenu1.ImageList = null;
            this.autocompleteMenu1.Items = new string[0];
            this.autocompleteMenu1.TargetControlWrapper = null;
            // 
            // tags_textBox
            // 
            this.tags_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.autocompleteMenu1.SetAutocompleteMenu(this.tags_textBox, this.autocompleteMenu1);
            this.tags_textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tags_textBox.Location = new System.Drawing.Point(0, 4);
            this.tags_textBox.Name = "tags_textBox";
            this.tags_textBox.Size = new System.Drawing.Size(1080, 29);
            this.tags_textBox.TabIndex = 8;
            // 
            // option_comboBox
            // 
            this.option_comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.option_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.option_comboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.option_comboBox.FormattingEnabled = true;
            this.option_comboBox.Items.AddRange(new object[] {
            "Теги",
            "Теги ИЛИ",
            "Часть тега",
            "MD5"});
            this.option_comboBox.Location = new System.Drawing.Point(1086, 3);
            this.option_comboBox.Name = "option_comboBox";
            this.option_comboBox.Size = new System.Drawing.Size(121, 32);
            this.option_comboBox.TabIndex = 9;
            // 
            // search_button
            // 
            this.search_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.search_button.Image = global::Ange.Properties.Resources.search25;
            this.search_button.Location = new System.Drawing.Point(1212, 3);
            this.search_button.Name = "search_button";
            this.search_button.Size = new System.Drawing.Size(32, 32);
            this.search_button.TabIndex = 10;
            this.search_button.UseVisualStyleBackColor = true;
            this.search_button.Click += new System.EventHandler(this.search_button_Click);
            // 
            // slide_show_button
            // 
            this.slide_show_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.slide_show_button.Image = global::Ange.Properties.Resources.images25;
            this.slide_show_button.Location = new System.Drawing.Point(1248, 3);
            this.slide_show_button.Name = "slide_show_button";
            this.slide_show_button.Size = new System.Drawing.Size(32, 32);
            this.slide_show_button.TabIndex = 11;
            this.slide_show_button.UseVisualStyleBackColor = true;
            this.slide_show_button.Click += new System.EventHandler(this.slide_show_button_Click);
            // 
            // imageListView1
            // 
            this.imageListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imageListView1.ContextMenuStrip = this.contextMenuStrip1;
            this.imageListView1.Location = new System.Drawing.Point(0, 41);
            this.imageListView1.Name = "imageListView1";
            this.imageListView1.PersistentCacheDirectory = "";
            this.imageListView1.PersistentCacheSize = ((long)(100));
            this.imageListView1.Size = new System.Drawing.Size(1283, 627);
            this.imageListView1.TabIndex = 7;
            this.imageListView1.ThumbnailSize = new System.Drawing.Size(300, 225);
            this.imageListView1.UseWIC = true;
            this.imageListView1.ItemDoubleClick += new Manina.Windows.Forms.ItemDoubleClickEventHandler(this.imageListView1_ItemDoubleClick);
            this.imageListView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.imageListView1_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1283, 690);
            this.Controls.Add(this.slide_show_button);
            this.Controls.Add(this.search_button);
            this.Controls.Add(this.option_comboBox);
            this.Controls.Add(this.tags_textBox);
            this.Controls.Add(this.imageListView1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Ange";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

