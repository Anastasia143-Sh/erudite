namespace erudite
{
    /// <summary>
    /// Форма начального экрана игры, где пользователь настраивает параметры новой игры
    /// Позволяет выбрать количество игроков, ввести их имена и получить справку об игре
    /// </summary>
    public partial class InitialForm : Form
    {
        public InitialForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки «Закрыть»
        /// Закрывает форму и завершает работу приложения
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Обработчик события нажатия кнопки «Начать игру»
        /// Запускает процесс настройки игры:
        /// запрашивает количество игроков (2–4), их имена, затем открывает форму выбора персонажей
        /// </summary>
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

        /// <summary>
        /// Обработчик события нажатия кнопки «Об игре» 
        /// Отображает информационное окно с описанием правил, целей и особенностей игры 
        /// </summary>
        private void btnAboutGame_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Добро пожаловать в игру Эрудит\n\n" +
                "Количество игроков: 2-4\n" +
                "Цель игры: набрать больше очков, чем соперники, составляя слова из букв на игровом поле\n" +
                "Особенности:\n" +
                "- перед началом игры участники выбирают себе персонажа из мультсериала «Смешарики»\n" +
                "- способ переноса букв: сначала нажать на букву, затем на клетку поля, буква перенесется\n" +
                "- фиксация слов: фиксировать можно ТОЛЬКО ОДНО слово, за один ход можно совершить несколько фиксаций\n" +
                "- самое первое слово должно проходить через центр поля\n" +
                "\n" +
                "Хорошей игры",
                "Об игре",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
