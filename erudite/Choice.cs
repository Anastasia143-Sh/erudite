using ClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace erudite
{
    /// <summary>
    /// Форма выбора персонажей для игроков 
    /// Позволяет каждому игроку выбрать уникального персонажа из мультсериала «Смешарики»
    /// </summary>
    public partial class Choice : Form
    {
        private InitialForm _previousForm;
        private List<string> _playerNames;
        private List<int> _imageIndexes;
        private int _playerCount;
        private int imageIndex;
        private int playerIndex;

        public Choice(InitialForm previousForm, List<string> playerNames, int playerCount)
        {
            InitializeComponent();
            _previousForm = previousForm;
            _playerNames = playerNames;
            _playerCount = playerCount;
            _imageIndexes = new List<int>();
            imageIndex = 0;
            playerIndex = 0;
            this.Load += Choice_Load;
        }

        /// <summary>
        /// Обработчик события загрузки формы
        /// </summary>
        private void Choice_Load(object sender, EventArgs e)
        {
            UpdatePlayerTurnLabel();
        }

        /// <summary>
        /// Обновляет текст метки (lblInfo) с указанием текущего игрока и отображает соответствующее изображение персонажа
        /// Если все игроки выбрали персонажей, отображает сообщение об окончании выбора
        /// </summary>
        private void UpdatePlayerTurnLabel()
        {
            // Проверка границ массива
            if (playerIndex >= _playerNames.Count)
            {
                lblInfo.Text = "Все игроки выбрали персонажей";
                return;
            }

            // Обновление текста
            lblInfo.Text = $"{_playerNames[playerIndex]} выбирает персонажа";

            // Обновление изображения
            switch (imageIndex)
            {
                case 0:
                    pictureBox1.Image = Properties.Resources.крош2;
                    break;
                case 1:
                    pictureBox1.Image = Properties.Resources.ежик1;
                    break;
                case 2:
                    pictureBox1.Image = Properties.Resources.бараш1;
                    break;
                case 3:
                    pictureBox1.Image = Properties.Resources.совунья;
                    break;
                case 4:
                    pictureBox1.Image = Properties.Resources.нюша;
                    break;
                case 5:
                    pictureBox1.Image = Properties.Resources.карыч;
                    break;
                case 6:
                    pictureBox1.Image = Properties.Resources.копатыч;
                    break;
                case 7:
                    pictureBox1.Image = Properties.Resources.лосяш;
                    break;
                case 8:
                    pictureBox1.Image = Properties.Resources.пин;
                    break;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Закрыть»
        /// Показывает диалоговое окно подтверждения и закрывает форму, возвращая пользователя на начальную форму
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Показываем диалоговое окно с подтверждением
            DialogResult result = MessageBox.Show
            (
                "Вы уверены, что хотите отменить выбор персонажей?",
                "Подтверждение выхода",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                _previousForm.Show();
                this.Close();
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки подтверждения выбора персонажа (btnCoice)
        /// Проверяет, не выбран ли персонаж другим игроком, сохраняет выбор, переходит к следующему игроку
        /// При завершении выбора всех персонажей открывает основную игровую форму
        /// </summary>
        private void btnCoice_Click(object sender, EventArgs e)
        {
            if (_imageIndexes.Contains(imageIndex))
            {
                MessageBox.Show("Один из игроков уже выбрал этого персонажа.\nПожалуйста, выберите другого", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _imageIndexes.Add(imageIndex);

            playerIndex++;

            imageIndex = 0;

            if (playerIndex < _playerCount)
            {
                UpdatePlayerTurnLabel();
                MessageBox.Show($"{_playerNames[playerIndex]} выбирает персонажа. Передайте ему/ей ноутбук, после нажмите ОК", "Выбор персонажей");
            }
            else
            {
                List<Player> players = new List<Player>();
                for (int i = 0; i < _playerCount; i++)
                {
                    Player player = new Player(_playerNames[i], _imageIndexes[i]);
                    players.Add(player);
                }
                DialogResult result = MessageBox.Show(
                "Все персонажи выбраны!",
                "Завершение выбора",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

                if (result == DialogResult.OK)
                {
                    EruditeForm newForm = new EruditeForm(_previousForm, players); 
                    newForm.Show();
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Далее» (btnNext)
        /// Переключает на следующего доступного персонажа (циклически: 0→1→2→...→8→0)
        /// </summary>
        private void btnNext_Click(object sender, EventArgs e)
        {
            imageIndex = (imageIndex + 1) % 9; 
            UpdatePlayerTurnLabel();
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Назад» (btnPrev)
        /// Переключает на предыдущего доступного персонажа (циклически: 8→7→6→…→0→8)
        /// </summary>
        private void btnPrev_Click(object sender, EventArgs e)
        {
            imageIndex = (imageIndex - 1 + 9) % 9; // Цикл: 2→1→0→2
            UpdatePlayerTurnLabel();
        }
    }
}
