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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tags_toolStripSpringTextBox = new WindowsFormsApp1.ToolStripSpringTextBox();
            this.tag_toolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.plus_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.option_toolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.search_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.slideshow_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.imageListView1 = new Manina.Windows.Forms.ImageListView();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
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
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(48, 48);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tags_toolStripSpringTextBox,
            this.tag_toolStripComboBox,
            this.plus_toolStripButton,
            this.option_toolStripComboBox,
            this.search_toolStripButton,
            this.slideshow_toolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1283, 55);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tags_toolStripSpringTextBox
            // 
            this.tags_toolStripSpringTextBox.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tags_toolStripSpringTextBox.Name = "tags_toolStripSpringTextBox";
            this.tags_toolStripSpringTextBox.Size = new System.Drawing.Size(711, 55);
            this.tags_toolStripSpringTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripSpringTextBox1_KeyDown);
            // 
            // tag_toolStripComboBox
            // 
            this.tag_toolStripComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tag_toolStripComboBox.Name = "tag_toolStripComboBox";
            this.tag_toolStripComboBox.Size = new System.Drawing.Size(268, 55);
            // 
            // plus_toolStripButton
            // 
            this.plus_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.plus_toolStripButton.Image = global::Ange.Properties.Resources.plus;
            this.plus_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.plus_toolStripButton.Name = "plus_toolStripButton";
            this.plus_toolStripButton.Size = new System.Drawing.Size(52, 52);
            this.plus_toolStripButton.Text = "toolStripButton1";
            this.plus_toolStripButton.Click += new System.EventHandler(this.plus_toolStripButton_Click);
            // 
            // option_toolStripComboBox
            // 
            this.option_toolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.option_toolStripComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.option_toolStripComboBox.Items.AddRange(new object[] {
            "Теги",
            "Теги ИЛИ",
            "Часть тега",
            "MD5"});
            this.option_toolStripComboBox.Name = "option_toolStripComboBox";
            this.option_toolStripComboBox.Size = new System.Drawing.Size(101, 55);
            // 
            // search_toolStripButton
            // 
            this.search_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.search_toolStripButton.Image = global::Ange.Properties.Resources.search;
            this.search_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.search_toolStripButton.Name = "search_toolStripButton";
            this.search_toolStripButton.Size = new System.Drawing.Size(52, 52);
            this.search_toolStripButton.Text = "toolStripButton2";
            this.search_toolStripButton.Click += new System.EventHandler(this.search_toolStripButton_Click);
            // 
            // slideshow_toolStripButton
            // 
            this.slideshow_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.slideshow_toolStripButton.Image = global::Ange.Properties.Resources.images;
            this.slideshow_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.slideshow_toolStripButton.Name = "slideshow_toolStripButton";
            this.slideshow_toolStripButton.Size = new System.Drawing.Size(52, 52);
            this.slideshow_toolStripButton.Text = "toolStripButton3";
            this.slideshow_toolStripButton.Click += new System.EventHandler(this.slideshow_toolStripButton_Click);
            // 
            // imageListView1
            // 
            this.imageListView1.ContextMenuStrip = this.contextMenuStrip1;
            this.imageListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageListView1.Location = new System.Drawing.Point(0, 55);
            this.imageListView1.Name = "imageListView1";
            this.imageListView1.PersistentCacheDirectory = "";
            this.imageListView1.PersistentCacheSize = ((long)(100));
            this.imageListView1.Size = new System.Drawing.Size(1283, 613);
            this.imageListView1.TabIndex = 7;
            this.imageListView1.ThumbnailSize = new System.Drawing.Size(300, 225);
            this.imageListView1.UseWIC = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1283, 690);
            this.Controls.Add(this.imageListView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Ange";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStrip toolStrip1;
        private WindowsFormsApp1.ToolStripSpringTextBox tags_toolStripSpringTextBox;
        private System.Windows.Forms.ToolStripComboBox tag_toolStripComboBox;
        private System.Windows.Forms.ToolStripButton plus_toolStripButton;
        private System.Windows.Forms.ToolStripComboBox option_toolStripComboBox;
        private System.Windows.Forms.ToolStripButton search_toolStripButton;
        private System.Windows.Forms.ToolStripButton slideshow_toolStripButton;
        private Manina.Windows.Forms.ImageListView imageListView1;
    }
}

