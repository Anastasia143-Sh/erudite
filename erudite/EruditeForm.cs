using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary;

namespace erudite
{
    public partial class EruditeForm : Form
    {
        private InitialForm _previousForm;
        private List<Player> _players = new List<Player>();
        private int playerIndex;
        private PictureBox[,] boardCells = new PictureBox[15, 15];
        private const int CELL_SIZE = 40;

        public EruditeForm(InitialForm previousForm, List<Player> players)
        {
            InitializeComponent();
            _previousForm = previousForm;
            _players = players;
            playerIndex = 0;
            this.Load += EruditeForm_Load;
        }

        private void EruditeForm_Load(object sender, EventArgs e)
        {
            UpdatePlayerLabels();
            UpdateScoreDisplay();
        }

        private void UpdatePlayerLabels()
        {
            // Обновляем аватар текущего игрока
            switch (_players[playerIndex].CharacterImageIndex)
            {
                case 0:
                    pictureBox1.Image = Properties.Resources.крош1;
                    break;
                case 1:
                    pictureBox1.Image = Properties.Resources.ежик;
                    break;
                case 2:
                    pictureBox1.Image = Properties.Resources.бараш;
                    break;
            }

            // Обновляем информацию о текущем игроке
            lblInfo.Text = $"Сейчас ходит: {_players[playerIndex].Name}";
            lblName.Text = _players[playerIndex].Name;
        }

        private void UpdateScoreDisplay()
        {
            // Скрываем все элементы для игроков, которых нет
            lblName1Player.Visible = false;
            lblScores1Player.Visible = false;
            lblName2Player.Visible = false;
            lblScores2Player.Visible = false;
            lblName3Player.Visible = false;
            lblScores3Player.Visible = false;
            lblName4Player.Visible = false;
            lblScores4Player.Visible = false;

            // Показываем и обновляем данные для каждого существующего игрока
            for (int i = 0; i < _players.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        lblName1Player.Text = _players[i].Name;
                        lblScores1Player.Text = _players[i].Score.ToString();
                        lblName1Player.Visible = true;
                        lblScores1Player.Visible = true;
                        break;
                    case 1:
                        lblName2Player.Text = _players[i].Name;
                        lblScores2Player.Text = _players[i].Score.ToString();
                        lblName2Player.Visible = true;
                        lblScores2Player.Visible = true;
                        break;
                    case 2:
                        lblName3Player.Text = _players[i].Name;
                        lblScores3Player.Text = _players[i].Score.ToString();
                        lblName3Player.Visible = true;
                        lblScores3Player.Visible = true;
                        break;
                    case 3:
                        lblName4Player.Text = _players[i].Name;
                        lblScores4Player.Text = _players[i].Score.ToString();
                        lblName4Player.Visible = true;
                        lblScores4Player.Visible = true;
                        break;
                }
            }
        }

        // Метод для добавления очков текущему игроку
        private void AddScoreToCurrentPlayer(int points)
        {
            _players[playerIndex].Score += points;
            UpdateScoreDisplay(); // Обновляем отображение сразу после изменения
        }

        // Метод для смены хода
        private void NextPlayerTurn()
        {
            playerIndex = (playerIndex + 1) % _players.Count; // Переход к следующему игроку с циклом
            UpdatePlayerLabels(); // Обновляем интерфейс
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _previousForm.Show();
            this.Close();
        }
    }
}
