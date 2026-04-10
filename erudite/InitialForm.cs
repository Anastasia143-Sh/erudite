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
            int playerCount = 0;
            bool validInput = false;

            while (!validInput)
            {
                string playerCountInput = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите количество игроков (2-4):", "Настройка игры", "2");
                if (string.IsNullOrEmpty(playerCountInput))
                {
                    DialogResult result = MessageBox.Show(
                        $"Вы хотите отменить создание игры?",
                        "Отмена ввода",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        return;
                    }
                }
                if (!int.TryParse(playerCountInput, out playerCount) || playerCount < 2 || playerCount > 4)
                {
                    MessageBox.Show("Введите корректное число игроков (от 2 до 4)", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    validInput = true;
                }
            }

            List<string> playerNames = new List<string>();
            for (int i = 0; i < playerCount; i++)
            {
                string playerName = Microsoft.VisualBasic.Interaction.InputBox
                    ($"Введите имя игрока {i + 1}:", "Ввод имени", $"Игрок {i + 1}");

                if (string.IsNullOrEmpty(playerName))
                {
                    DialogResult result = MessageBox.Show(
                        $"Вы хотите отменить создание игры?",
                        "Отмена ввода",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        return;
                    }
                    else
                    {
                        i--;
                        continue;
                    }
                }
                playerNames.Add(playerName);
            }

            Choice newForm = new Choice(this, playerNames, playerCount);
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
