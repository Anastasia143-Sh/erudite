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
            // Шаг 1: Ввод количества игроков
            string playerCountInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите количество игроков (2-4):", "Настройка игры", "2");

            if (string.IsNullOrEmpty(playerCountInput))
                return;

            if (!int.TryParse(playerCountInput, out int playerCount) || playerCount < 1 || playerCount > 10)
            {
                MessageBox.Show("Введите корректное число игроков (от 2 до 4)", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> playerNames = new List<string>();
            for (int i = 0; i < playerCount; i++)
            {
                string playerName = Microsoft.VisualBasic.Interaction.InputBox
                    ($"Введите имя игрока {i + 1}:", "Ввод имени", $"Игрок {i + 1}");

                if (string.IsNullOrEmpty(playerName))
                {
                    MessageBox.Show($"Имя игрока {i + 1} не может быть пустым!", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    i--; // Повторяем итерацию для этого игрока
                    continue;
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
