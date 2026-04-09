using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using ClassLibrary;

namespace erudite
{
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

        private void Choice_Load(object sender, EventArgs e)
        {
            UpdatePlayerTurnLabel();
        }

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


        private void btnClose_Click(object sender, EventArgs e)
        {
            _previousForm.Show();
            this.Close();
        }

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
            }
            else
            {
                // Создаём список игроков на основе выбранных данных
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
                    EruditeForm newForm = new EruditeForm(_previousForm, players); // Передаём список игроков
                    newForm.Show();
                    this.Close();
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            imageIndex = (imageIndex + 1) % 9; // Цикл: 0→1→2→0
            UpdatePlayerTurnLabel();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            imageIndex = (imageIndex - 1 + 9) % 9; // Цикл: 2→1→0→2
            UpdatePlayerTurnLabel();
        }
    }
}
