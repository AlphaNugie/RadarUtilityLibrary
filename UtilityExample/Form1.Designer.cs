namespace UtilityExample
{
    partial class Form1
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
            this.richTextBox_Input = new System.Windows.Forms.RichTextBox();
            this.button_Parse = new System.Windows.Forms.Button();
            this.richTextBox_RadarPacket = new System.Windows.Forms.RichTextBox();
            this.comboBox_ProtocolVersion = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // richTextBox_Input
            // 
            this.richTextBox_Input.Location = new System.Drawing.Point(30, 13);
            this.richTextBox_Input.Name = "richTextBox_Input";
            this.richTextBox_Input.Size = new System.Drawing.Size(781, 315);
            this.richTextBox_Input.TabIndex = 0;
            this.richTextBox_Input.Text = "";
            // 
            // button_Parse
            // 
            this.button_Parse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.button_Parse.Font = new System.Drawing.Font("等线", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Parse.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button_Parse.Location = new System.Drawing.Point(542, 348);
            this.button_Parse.Name = "button_Parse";
            this.button_Parse.Size = new System.Drawing.Size(96, 36);
            this.button_Parse.TabIndex = 1;
            this.button_Parse.Text = "解析";
            this.button_Parse.UseVisualStyleBackColor = false;
            this.button_Parse.Click += new System.EventHandler(this.Button_Parse_Click);
            // 
            // richTextBox_RadarPacket
            // 
            this.richTextBox_RadarPacket.Location = new System.Drawing.Point(30, 348);
            this.richTextBox_RadarPacket.Name = "richTextBox_RadarPacket";
            this.richTextBox_RadarPacket.Size = new System.Drawing.Size(486, 290);
            this.richTextBox_RadarPacket.TabIndex = 2;
            this.richTextBox_RadarPacket.Text = "";
            // 
            // comboBox_ProtocolVersion
            // 
            this.comboBox_ProtocolVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ProtocolVersion.FormattingEnabled = true;
            this.comboBox_ProtocolVersion.Items.AddRange(new object[] {
            "v2.1.3",
            "v2.1.2"});
            this.comboBox_ProtocolVersion.Location = new System.Drawing.Point(658, 356);
            this.comboBox_ProtocolVersion.Name = "comboBox_ProtocolVersion";
            this.comboBox_ProtocolVersion.Size = new System.Drawing.Size(121, 22);
            this.comboBox_ProtocolVersion.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 650);
            this.Controls.Add(this.comboBox_ProtocolVersion);
            this.Controls.Add(this.richTextBox_RadarPacket);
            this.Controls.Add(this.button_Parse);
            this.Controls.Add(this.richTextBox_Input);
            this.Font = new System.Drawing.Font("等线", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox_Input;
        private System.Windows.Forms.Button button_Parse;
        private System.Windows.Forms.RichTextBox richTextBox_RadarPacket;
        private System.Windows.Forms.ComboBox comboBox_ProtocolVersion;
    }
}