namespace erudite
{
    public partial class InitialForm : Form
    {
        public InitialForm()
        {
            InitializeComponent();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            Choice newForm = new Choice(this);
            newForm.Show();
            this.Hide();
        }

        private void btnAboutGame_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Добро пожаловать в игру Эрудит\n\n" +
                "Количество игроков: 2-4\n" +
                "Цель игры: набрать больше очков, чем соперники, составляя слова из букв на игровом поле\n" +
                "Особенности:\n" +
                "- перед началом игры участники выбирают себе персонажа из мультсериала «Смешарики»\n" +
                "\n" +
                "Хорошей игры",
                "Об игре",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
