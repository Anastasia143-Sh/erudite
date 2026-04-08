namespace erudite
{
    partial class InitialForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnClose = new Button();
            btnAboutGame = new Button();
            btnStart = new Button();
            SuspendLayout();
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.Lavender;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Location = new Point(447, 536);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(139, 46);
            btnClose.TabIndex = 0;
            btnClose.Text = "закрыть";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // btnAboutGame
            // 
            btnAboutGame.BackColor = Color.Lavender;
            btnAboutGame.FlatStyle = FlatStyle.Flat;
            btnAboutGame.Location = new Point(447, 484);
            btnAboutGame.Name = "btnAboutGame";
            btnAboutGame.Size = new Size(139, 46);
            btnAboutGame.TabIndex = 1;
            btnAboutGame.Text = "об игре";
            btnAboutGame.UseVisualStyleBackColor = false;
            btnAboutGame.Click += btnAboutGame_Click;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.Lavender;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.Location = new Point(447, 434);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(139, 44);
            btnStart.TabIndex = 2;
            btnStart.Text = "старт";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // InitialForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.заставка;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1033, 594);
            Controls.Add(btnStart);
            Controls.Add(btnAboutGame);
            Controls.Add(btnClose);
            DoubleBuffered = true;
            Name = "InitialForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "InitialForm";
            ResumeLayout(false);
        }

        #endregion

        private Button btnClose;
        private Button btnAboutGame;
        private Button btnStart;
    }
}
