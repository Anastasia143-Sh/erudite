namespace erudite
{
    partial class Choice
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
            lblInfo = new Label();
            btnClose = new Button();
            btnCoice = new Button();
            btnNext = new Button();
            btnPrev = new Button();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // lblInfo
            // 
            lblInfo.BackColor = Color.Lavender;
            lblInfo.Location = new Point(84, 36);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(360, 68);
            lblInfo.TabIndex = 0;
            lblInfo.Text = "label1";
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.Lavender;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(203, 674);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(136, 52);
            btnClose.TabIndex = 1;
            btnClose.Text = "закрыть";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // btnCoice
            // 
            btnCoice.BackColor = Color.Lavender;
            btnCoice.FlatStyle = FlatStyle.Flat;
            btnCoice.Location = new Point(203, 616);
            btnCoice.Name = "btnCoice";
            btnCoice.Size = new Size(136, 52);
            btnCoice.TabIndex = 2;
            btnCoice.Text = "выбрать";
            btnCoice.UseVisualStyleBackColor = false;
            btnCoice.Click += btnCoice_Click;
            // 
            // btnNext
            // 
            btnNext.BackColor = Color.Lavender;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Location = new Point(345, 616);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(56, 52);
            btnNext.TabIndex = 3;
            btnNext.Text = ">";
            btnNext.UseVisualStyleBackColor = false;
            // 
            // btnPrev
            // 
            btnPrev.BackColor = Color.Lavender;
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.Location = new Point(141, 616);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(56, 52);
            btnPrev.TabIndex = 4;
            btnPrev.Text = "<";
            btnPrev.UseVisualStyleBackColor = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(120, 173);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(289, 403);
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // Choice
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.фон2;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(527, 738);
            Controls.Add(pictureBox1);
            Controls.Add(btnPrev);
            Controls.Add(btnNext);
            Controls.Add(btnCoice);
            Controls.Add(btnClose);
            Controls.Add(lblInfo);
            DoubleBuffered = true;
            Name = "Choice";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Choice";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label lblInfo;
        private Button btnClose;
        private Button btnCoice;
        private Button btnNext;
        private Button btnPrev;
        private PictureBox pictureBox1;
    }
}