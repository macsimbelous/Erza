namespace Eris
{
    partial class EditForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.id_tag_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tag_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tag_rus_textBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.count_links_textBox = new System.Windows.Forms.TextBox();
            this.type_tag_comboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.description_tag_textBox = new System.Windows.Forms.TextBox();
            this.save_button = new System.Windows.Forms.Button();
            this.cancel_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ИД тега";
            // 
            // id_tag_textBox
            // 
            this.id_tag_textBox.Enabled = false;
            this.id_tag_textBox.Location = new System.Drawing.Point(12, 25);
            this.id_tag_textBox.Name = "id_tag_textBox";
            this.id_tag_textBox.Size = new System.Drawing.Size(100, 20);
            this.id_tag_textBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Тег";
            // 
            // tag_textBox
            // 
            this.tag_textBox.Location = new System.Drawing.Point(12, 64);
            this.tag_textBox.Name = "tag_textBox";
            this.tag_textBox.Size = new System.Drawing.Size(339, 20);
            this.tag_textBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Тег на русском";
            // 
            // tag_rus_textBox
            // 
            this.tag_rus_textBox.Location = new System.Drawing.Point(12, 103);
            this.tag_rus_textBox.Name = "tag_rus_textBox";
            this.tag_rus_textBox.Size = new System.Drawing.Size(339, 20);
            this.tag_rus_textBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(227, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Тип тега";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(118, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Количество ссылок";
            // 
            // count_links_textBox
            // 
            this.count_links_textBox.Enabled = false;
            this.count_links_textBox.Location = new System.Drawing.Point(121, 25);
            this.count_links_textBox.Name = "count_links_textBox";
            this.count_links_textBox.Size = new System.Drawing.Size(100, 20);
            this.count_links_textBox.TabIndex = 9;
            // 
            // type_tag_comboBox
            // 
            this.type_tag_comboBox.FormattingEnabled = true;
            this.type_tag_comboBox.Location = new System.Drawing.Point(230, 25);
            this.type_tag_comboBox.Name = "type_tag_comboBox";
            this.type_tag_comboBox.Size = new System.Drawing.Size(121, 21);
            this.type_tag_comboBox.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 126);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Описание тега";
            // 
            // description_tag_textBox
            // 
            this.description_tag_textBox.Location = new System.Drawing.Point(12, 142);
            this.description_tag_textBox.Multiline = true;
            this.description_tag_textBox.Name = "description_tag_textBox";
            this.description_tag_textBox.Size = new System.Drawing.Size(339, 178);
            this.description_tag_textBox.TabIndex = 12;
            // 
            // save_button
            // 
            this.save_button.Location = new System.Drawing.Point(357, 25);
            this.save_button.Name = "save_button";
            this.save_button.Size = new System.Drawing.Size(75, 23);
            this.save_button.TabIndex = 13;
            this.save_button.Text = "Сохранить";
            this.save_button.UseVisualStyleBackColor = true;
            this.save_button.Click += new System.EventHandler(this.save_button_Click);
            // 
            // cancel_button
            // 
            this.cancel_button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel_button.Location = new System.Drawing.Point(357, 54);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(75, 23);
            this.cancel_button.TabIndex = 14;
            this.cancel_button.Text = "Отмена";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.cancel_button_Click);
            // 
            // EditForm
            // 
            this.AcceptButton = this.save_button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel_button;
            this.ClientSize = new System.Drawing.Size(443, 334);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.save_button);
            this.Controls.Add(this.description_tag_textBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.type_tag_comboBox);
            this.Controls.Add(this.count_links_textBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tag_rus_textBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tag_textBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.id_tag_textBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Редактирование тега";
            this.Load += new System.EventHandler(this.EditForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox id_tag_textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tag_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tag_rus_textBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox count_links_textBox;
        private System.Windows.Forms.ComboBox type_tag_comboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox description_tag_textBox;
        private System.Windows.Forms.Button save_button;
        private System.Windows.Forms.Button cancel_button;
    }
}