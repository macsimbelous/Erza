namespace Rei
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tag_textBox = new System.Windows.Forms.TextBox();
            this.type_comboBox = new System.Windows.Forms.ComboBox();
            this.description_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.time_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.new_button = new System.Windows.Forms.Button();
            this.edit_button = new System.Windows.Forms.Button();
            this.save_button = new System.Windows.Forms.Button();
            this.del_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ps1_button = new System.Windows.Forms.Button();
            this.bat_button = new System.Windows.Forms.Button();
            this.getgelbooru_checkBox = new System.Windows.Forms.CheckBox();
            this.getidol_checkBox = new System.Windows.Forms.CheckBox();
            this.erza_checkBox = new System.Windows.Forms.CheckBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(3, 22);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(510, 604);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedValueChanged += new System.EventHandler(this.listBox1_SelectedValueChanged);
            // 
            // tag_textBox
            // 
            this.tag_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tag_textBox.Location = new System.Drawing.Point(519, 22);
            this.tag_textBox.Name = "tag_textBox";
            this.tag_textBox.Size = new System.Drawing.Size(359, 20);
            this.tag_textBox.TabIndex = 1;
            // 
            // type_comboBox
            // 
            this.type_comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.type_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.type_comboBox.FormattingEnabled = true;
            this.type_comboBox.Location = new System.Drawing.Point(714, 61);
            this.type_comboBox.Name = "type_comboBox";
            this.type_comboBox.Size = new System.Drawing.Size(164, 21);
            this.type_comboBox.TabIndex = 3;
            // 
            // description_textBox
            // 
            this.description_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.description_textBox.Location = new System.Drawing.Point(519, 102);
            this.description_textBox.Multiline = true;
            this.description_textBox.Name = "description_textBox";
            this.description_textBox.Size = new System.Drawing.Size(360, 156);
            this.description_textBox.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(855, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Тег";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(855, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Тип";
            // 
            // time_textBox
            // 
            this.time_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.time_textBox.Location = new System.Drawing.Point(519, 61);
            this.time_textBox.Name = "time_textBox";
            this.time_textBox.ReadOnly = true;
            this.time_textBox.Size = new System.Drawing.Size(187, 20);
            this.time_textBox.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(670, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Время";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(826, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Описание";
            // 
            // new_button
            // 
            this.new_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.new_button.Location = new System.Drawing.Point(519, 264);
            this.new_button.Name = "new_button";
            this.new_button.Size = new System.Drawing.Size(170, 23);
            this.new_button.TabIndex = 5;
            this.new_button.Text = "Создать";
            this.new_button.UseVisualStyleBackColor = true;
            this.new_button.Click += new System.EventHandler(this.new_button_Click);
            // 
            // edit_button
            // 
            this.edit_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.edit_button.Location = new System.Drawing.Point(708, 264);
            this.edit_button.Name = "edit_button";
            this.edit_button.Size = new System.Drawing.Size(170, 23);
            this.edit_button.TabIndex = 6;
            this.edit_button.Text = "Редактировать";
            this.edit_button.UseVisualStyleBackColor = true;
            this.edit_button.Click += new System.EventHandler(this.edit_button_Click);
            // 
            // save_button
            // 
            this.save_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.save_button.Location = new System.Drawing.Point(519, 293);
            this.save_button.Name = "save_button";
            this.save_button.Size = new System.Drawing.Size(170, 23);
            this.save_button.TabIndex = 7;
            this.save_button.Text = "Сохранить";
            this.save_button.UseVisualStyleBackColor = true;
            this.save_button.Click += new System.EventHandler(this.save_button_Click);
            // 
            // del_button
            // 
            this.del_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.del_button.Location = new System.Drawing.Point(708, 293);
            this.del_button.Name = "del_button";
            this.del_button.Size = new System.Drawing.Size(170, 23);
            this.del_button.TabIndex = 8;
            this.del_button.Text = "Удалить";
            this.del_button.UseVisualStyleBackColor = true;
            this.del_button.Click += new System.EventHandler(this.del_button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(2, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 20);
            this.label5.TabIndex = 13;
            this.label5.Text = "Список тегов";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(625, 322);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Импортировать из Firefox";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ps1_button);
            this.groupBox1.Controls.Add(this.bat_button);
            this.groupBox1.Controls.Add(this.getgelbooru_checkBox);
            this.groupBox1.Controls.Add(this.getidol_checkBox);
            this.groupBox1.Controls.Add(this.erza_checkBox);
            this.groupBox1.Location = new System.Drawing.Point(519, 351);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(359, 89);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Генерация скриптов закачьки";
            // 
            // ps1_button
            // 
            this.ps1_button.Location = new System.Drawing.Point(277, 48);
            this.ps1_button.Name = "ps1_button";
            this.ps1_button.Size = new System.Drawing.Size(75, 23);
            this.ps1_button.TabIndex = 4;
            this.ps1_button.Text = "PS1";
            this.ps1_button.UseVisualStyleBackColor = true;
            this.ps1_button.Click += new System.EventHandler(this.ps1_button_Click);
            // 
            // bat_button
            // 
            this.bat_button.Location = new System.Drawing.Point(277, 19);
            this.bat_button.Name = "bat_button";
            this.bat_button.Size = new System.Drawing.Size(75, 23);
            this.bat_button.TabIndex = 3;
            this.bat_button.Text = "BAT";
            this.bat_button.UseVisualStyleBackColor = true;
            this.bat_button.Click += new System.EventHandler(this.bat_button_Click);
            // 
            // getgelbooru_checkBox
            // 
            this.getgelbooru_checkBox.AutoSize = true;
            this.getgelbooru_checkBox.Location = new System.Drawing.Point(6, 67);
            this.getgelbooru_checkBox.Name = "getgelbooru_checkBox";
            this.getgelbooru_checkBox.Size = new System.Drawing.Size(86, 17);
            this.getgelbooru_checkBox.TabIndex = 2;
            this.getgelbooru_checkBox.Text = "GetGelbooru";
            this.getgelbooru_checkBox.UseVisualStyleBackColor = true;
            // 
            // getidol_checkBox
            // 
            this.getidol_checkBox.AutoSize = true;
            this.getidol_checkBox.Location = new System.Drawing.Point(6, 43);
            this.getidol_checkBox.Name = "getidol_checkBox";
            this.getidol_checkBox.Size = new System.Drawing.Size(59, 17);
            this.getidol_checkBox.TabIndex = 1;
            this.getidol_checkBox.Text = "Getidol";
            this.getidol_checkBox.UseVisualStyleBackColor = true;
            // 
            // erza_checkBox
            // 
            this.erza_checkBox.AutoSize = true;
            this.erza_checkBox.Location = new System.Drawing.Point(6, 19);
            this.erza_checkBox.Name = "erza_checkBox";
            this.erza_checkBox.Size = new System.Drawing.Size(47, 17);
            this.erza_checkBox.TabIndex = 0;
            this.erza_checkBox.Text = "Erza";
            this.erza_checkBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 641);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.del_button);
            this.Controls.Add(this.save_button);
            this.Controls.Add(this.edit_button);
            this.Controls.Add(this.new_button);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.time_textBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.description_textBox);
            this.Controls.Add(this.type_comboBox);
            this.Controls.Add(this.tag_textBox);
            this.Controls.Add(this.listBox1);
            this.Name = "Form1";
            this.Text = "Rei";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox tag_textBox;
        private System.Windows.Forms.ComboBox type_comboBox;
        private System.Windows.Forms.TextBox description_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox time_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button new_button;
        private System.Windows.Forms.Button edit_button;
        private System.Windows.Forms.Button save_button;
        private System.Windows.Forms.Button del_button;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bat_button;
        private System.Windows.Forms.CheckBox getgelbooru_checkBox;
        private System.Windows.Forms.CheckBox getidol_checkBox;
        private System.Windows.Forms.CheckBox erza_checkBox;
        private System.Windows.Forms.Button ps1_button;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

