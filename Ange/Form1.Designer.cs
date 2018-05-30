﻿namespace Ange
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
            this.tag_radioButton = new System.Windows.Forms.RadioButton();
            this.md5_radioButton = new System.Windows.Forms.RadioButton();
            this.search_button = new System.Windows.Forms.Button();
            this.slideshow_button = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
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
            this.part_tag_radioButton = new System.Windows.Forms.RadioButton();
            this.search_condition_checkBox = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tag_radioButton
            // 
            this.tag_radioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tag_radioButton.AutoSize = true;
            this.tag_radioButton.Checked = true;
            this.tag_radioButton.Location = new System.Drawing.Point(570, 3);
            this.tag_radioButton.Name = "tag_radioButton";
            this.tag_radioButton.Size = new System.Drawing.Size(43, 17);
            this.tag_radioButton.TabIndex = 1;
            this.tag_radioButton.TabStop = true;
            this.tag_radioButton.Text = "Тег";
            this.toolTip1.SetToolTip(this.tag_radioButton, "Поиск по одному или нескольким тегам");
            this.tag_radioButton.UseVisualStyleBackColor = true;
            this.tag_radioButton.CheckedChanged += new System.EventHandler(this.tag_radioButton_CheckedChanged);
            // 
            // md5_radioButton
            // 
            this.md5_radioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.md5_radioButton.AutoSize = true;
            this.md5_radioButton.Location = new System.Drawing.Point(626, 20);
            this.md5_radioButton.Name = "md5_radioButton";
            this.md5_radioButton.Size = new System.Drawing.Size(48, 17);
            this.md5_radioButton.TabIndex = 2;
            this.md5_radioButton.TabStop = true;
            this.md5_radioButton.Text = "MD5";
            this.toolTip1.SetToolTip(this.md5_radioButton, "Поиск по MD5 хэшу");
            this.md5_radioButton.UseVisualStyleBackColor = true;
            // 
            // search_button
            // 
            this.search_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.search_button.Location = new System.Drawing.Point(718, 10);
            this.search_button.Name = "search_button";
            this.search_button.Size = new System.Drawing.Size(75, 23);
            this.search_button.TabIndex = 3;
            this.search_button.Text = "Поиск";
            this.toolTip1.SetToolTip(this.search_button, "Найти изображения связанные с указанными тегами.\r\nЕсли поле ввода будет пустым то" +
        " поиск вернёт все изображения.");
            this.search_button.UseVisualStyleBackColor = true;
            this.search_button.Click += new System.EventHandler(this.search_button_Click);
            // 
            // slideshow_button
            // 
            this.slideshow_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.slideshow_button.Location = new System.Drawing.Point(799, 10);
            this.slideshow_button.Name = "slideshow_button";
            this.slideshow_button.Size = new System.Drawing.Size(75, 23);
            this.slideshow_button.TabIndex = 4;
            this.slideshow_button.Text = "Слайдшоу";
            this.toolTip1.SetToolTip(this.slideshow_button, "Запустить слайдшоу");
            this.slideshow_button.UseVisualStyleBackColor = true;
            this.slideshow_button.Click += new System.EventHandler(this.slideshow_button_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(0, 41);
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(883, 437);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.VirtualMode = true;
            this.listView1.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView1_DrawItem);
            this.listView1.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView1_RetrieveVirtualItem);
            this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
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
            this.imageList1.ImageSize = new System.Drawing.Size(200, 150);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 481);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(883, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(136, 17);
            this.toolStripStatusLabel1.Text = "Изображений найдено:";
            // 
            // part_tag_radioButton
            // 
            this.part_tag_radioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.part_tag_radioButton.AutoSize = true;
            this.part_tag_radioButton.Location = new System.Drawing.Point(626, 3);
            this.part_tag_radioButton.Name = "part_tag_radioButton";
            this.part_tag_radioButton.Size = new System.Drawing.Size(81, 17);
            this.part_tag_radioButton.TabIndex = 7;
            this.part_tag_radioButton.TabStop = true;
            this.part_tag_radioButton.Text = "Часть тега";
            this.toolTip1.SetToolTip(this.part_tag_radioButton, "Поиск по части тега");
            this.part_tag_radioButton.UseVisualStyleBackColor = true;
            // 
            // search_condition_checkBox
            // 
            this.search_condition_checkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.search_condition_checkBox.AutoSize = true;
            this.search_condition_checkBox.Location = new System.Drawing.Point(570, 21);
            this.search_condition_checkBox.Name = "search_condition_checkBox";
            this.search_condition_checkBox.Size = new System.Drawing.Size(50, 17);
            this.search_condition_checkBox.TabIndex = 8;
            this.search_condition_checkBox.Text = "ИЛИ";
            this.toolTip1.SetToolTip(this.search_condition_checkBox, "Логическое условие используемое при поиске по нескольким тегам");
            this.search_condition_checkBox.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(13, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(551, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.TextUpdate += new System.EventHandler(this.comboBox1_TextUpdate);
            this.comboBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBox1_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 503);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.search_condition_checkBox);
            this.Controls.Add(this.part_tag_radioButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.slideshow_button);
            this.Controls.Add(this.search_button);
            this.Controls.Add(this.md5_radioButton);
            this.Controls.Add(this.tag_radioButton);
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
        private System.Windows.Forms.RadioButton tag_radioButton;
        private System.Windows.Forms.RadioButton md5_radioButton;
        private System.Windows.Forms.Button search_button;
        private System.Windows.Forms.Button slideshow_button;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem view_fullscreen_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edittagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slideshowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copytowallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.RadioButton part_tag_radioButton;
        private System.Windows.Forms.CheckBox search_condition_checkBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem copyhashToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copytodirToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem openOuterSoftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAllToDirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MoveAllToDirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem view_in_window_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recreate_preview_ToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}

