namespace Eris
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
            dataGridView1 = new System.Windows.Forms.DataGridView();
            tag_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            tag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            localization = new System.Windows.Forms.DataGridViewTextBoxColumn();
            type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            new_toolStripButton = new System.Windows.Forms.ToolStripButton();
            edit_toolStripButton = new System.Windows.Forms.ToolStripButton();
            delete_toolStripButton = new System.Windows.Forms.ToolStripButton();
            search_toolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            find_toolStripButton = new System.Windows.Forms.ToolStripButton();
            count_toolStripButton = new System.Windows.Forms.ToolStripButton();
            count_all_toolStripButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { tag_id, tag, localization, type, count, description });
            dataGridView1.Location = new System.Drawing.Point(0, 48);
            dataGridView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new System.Drawing.Size(1038, 570);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellMouseEnter += dataGridView1_CellMouseEnter;
            dataGridView1.RowPrePaint += dataGridView1_RowPrePaint;
            // 
            // tag_id
            // 
            tag_id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            tag_id.DataPropertyName = "tag_id";
            tag_id.FillWeight = 80F;
            tag_id.HeaderText = "ИД";
            tag_id.MinimumWidth = 80;
            tag_id.Name = "tag_id";
            tag_id.ReadOnly = true;
            tag_id.ToolTipText = "Идентификатор тега";
            tag_id.Width = 80;
            // 
            // tag
            // 
            tag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            tag.DataPropertyName = "tag";
            tag.FillWeight = 58.14433F;
            tag.HeaderText = "Тег";
            tag.Name = "tag";
            tag.ReadOnly = true;
            tag.ToolTipText = "Название тега";
            // 
            // localization
            // 
            localization.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            localization.DataPropertyName = "localization";
            localization.FillWeight = 58.14433F;
            localization.HeaderText = "Тег на русском";
            localization.Name = "localization";
            localization.ReadOnly = true;
            // 
            // type
            // 
            type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            type.DataPropertyName = "type";
            type.FillWeight = 29.07216F;
            type.HeaderText = "Тип";
            type.Name = "type";
            type.ReadOnly = true;
            type.ToolTipText = "Тип тега";
            // 
            // count
            // 
            count.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            count.DataPropertyName = "count";
            count.HeaderText = "Ссылки";
            count.Name = "count";
            count.ReadOnly = true;
            count.ToolTipText = "Количество ссылок на тег";
            // 
            // description
            // 
            description.DataPropertyName = "description";
            description.HeaderText = "Описание";
            description.Name = "description";
            description.ReadOnly = true;
            description.Visible = false;
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { new_toolStripButton, edit_toolStripButton, delete_toolStripButton, search_toolStripTextBox, find_toolStripButton, count_toolStripButton, count_all_toolStripButton });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(1038, 39);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // new_toolStripButton
            // 
            new_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            new_toolStripButton.Image = Properties.Resources.Add_Green256;
            new_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            new_toolStripButton.Name = "new_toolStripButton";
            new_toolStripButton.Size = new System.Drawing.Size(36, 36);
            new_toolStripButton.Text = "Новый тег";
            new_toolStripButton.Click += new_toolStripButton_Click;
            // 
            // edit_toolStripButton
            // 
            edit_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            edit_toolStripButton.Image = Properties.Resources.Edit256;
            edit_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            edit_toolStripButton.Name = "edit_toolStripButton";
            edit_toolStripButton.Size = new System.Drawing.Size(36, 36);
            edit_toolStripButton.Text = "Редактировать тег";
            edit_toolStripButton.Click += edit_toolStripButton_Click;
            // 
            // delete_toolStripButton
            // 
            delete_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            delete_toolStripButton.Image = Properties.Resources.Remove__Delete__Red256;
            delete_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            delete_toolStripButton.Name = "delete_toolStripButton";
            delete_toolStripButton.Size = new System.Drawing.Size(36, 36);
            delete_toolStripButton.Text = "Удалить тег";
            delete_toolStripButton.Click += delete_toolStripButton_Click;
            // 
            // search_toolStripTextBox
            // 
            search_toolStripTextBox.Name = "search_toolStripTextBox";
            search_toolStripTextBox.Size = new System.Drawing.Size(250, 39);
            search_toolStripTextBox.TextChanged += search_toolStripTextBox_TextChanged;
            // 
            // find_toolStripButton
            // 
            find_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            find_toolStripButton.Image = Properties.Resources.Search256;
            find_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            find_toolStripButton.Name = "find_toolStripButton";
            find_toolStripButton.Size = new System.Drawing.Size(36, 36);
            find_toolStripButton.Text = "Поиск";
            find_toolStripButton.Click += find_toolStripButton_Click;
            // 
            // count_toolStripButton
            // 
            count_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            count_toolStripButton.Image = Properties.Resources.Charting_Blue_New256;
            count_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            count_toolStripButton.Name = "count_toolStripButton";
            count_toolStripButton.Size = new System.Drawing.Size(36, 36);
            count_toolStripButton.Text = "Подсчитать ссылки на тег";
            count_toolStripButton.Click += count_toolStripButton_Click;
            // 
            // count_all_toolStripButton
            // 
            count_all_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            count_all_toolStripButton.Image = Properties.Resources.Charting_Blue_Add256;
            count_all_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            count_all_toolStripButton.Name = "count_all_toolStripButton";
            count_all_toolStripButton.Size = new System.Drawing.Size(36, 36);
            count_all_toolStripButton.Text = "Подсчитать ссылки на все теги";
            count_all_toolStripButton.Click += count_all_toolStripButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1038, 618);
            Controls.Add(toolStrip1);
            Controls.Add(dataGridView1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "Form1";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Эрис - редактор тегов";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton new_toolStripButton;
        private System.Windows.Forms.ToolStripButton edit_toolStripButton;
        private System.Windows.Forms.ToolStripButton delete_toolStripButton;
        private System.Windows.Forms.ToolStripButton count_toolStripButton;
        private System.Windows.Forms.ToolStripButton count_all_toolStripButton;
        private System.Windows.Forms.ToolStripButton find_toolStripButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn tag_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn tag;
        private System.Windows.Forms.DataGridViewTextBoxColumn localization;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn count;
        private System.Windows.Forms.DataGridViewTextBoxColumn description;
        private System.Windows.Forms.ToolStripTextBox search_toolStripTextBox;
    }
}

