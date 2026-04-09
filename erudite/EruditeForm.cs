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
        private GameController gameController;
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
            try
            {
                // 2. Инициализация игрового контроллера
                gameController = new GameController(_players);
                gameController.OnPlayerTurnStarted += OnCurrentPlayerChanged;
                gameController.OnPlayerScored += OnPlayerScored;
                gameController.OnWordValidationStarted += ShowWordValidationDialog;
                gameController.OnGameEnded += OnGameFinished;

                InitializeBoardVisuals();
                InitializePlayerHand();

                UpdatePlayerLabels();
                UpdateScoreDisplay();

                gameController.NextPlayer();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке игры: {ex.Message}");
                _previousForm.Show();
                this.Close();
            }
        }

        private void OnCurrentPlayerChanged(Player player)
        {
            UpdatePlayerLabels();
            InitializePlayerHand();
        }

        private void OnPlayerScored(Player player, int score)
        {
            UpdateScoreDisplay();
            MessageBox.Show($"{player.Name} набрал {score} очков!");
        }

        private void ShowWordValidationDialog(List<string> words, int score)
        {
            MessageBox.Show($"Проверяемые слова: {string.Join(", ", words)}\nОчки: {score}");
        }

        private void OnGameFinished(Player winner)
        {
            MessageBox.Show($"Игра окончена! Победитель: {winner.Name}");
            _previousForm.Show();
            this.Close();
        }

        private void InitializeBoardVisuals()
        {
            for (int row = 0; row < 15; row++)
            {
                for (int col = 0; col < 15; col++)
                {
                    Button cell = CreateBoardCell(row, col);
                    boardPanel.Controls.Add(cell);
                }
            }
        }

        private Button CreateBoardCell(int row, int col)
        {
            Button button = new Button
            {
                Size = new Size(40, 40),
                Location = new Point(col * 40, row * 40),
                Tag = (row, col),
                FlatStyle = FlatStyle.Flat
            };
            //button.Click += BoardCell_Click;
            return button;
        }

        private void InitializePlayerHand()
        {
            handPanel.Controls.Clear();
            var currentPlayer = gameController.GetCurrentPlayer();

            for (int i = 0; i < currentPlayer.Hand.Count; i++)
            {
                Button tileButton = CreateTileButton(currentPlayer.Hand[i], i);
                handPanel.Controls.Add(tileButton);
            }
        }

        private Button CreateTileButton(Tile tile, int index)
        {
            Button button = new Button
            {
                Text = tile.Letter.ToString(),
                Size = new Size(35, 35),
                Location = new Point(index * 40, 0),
                Tag = tile,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            //button.Click += TileButton_Click;
            return button;
        }

        private void UpdatePlayerLabels()
        {
            // Обновляем аватар текущего игрока
            switch (_players[playerIndex].ImageIndex)
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

        private void btnComplete_Click(object sender, EventArgs e)
        {

        }

        private void btnExchangeChips_Click(object sender, EventArgs e)
        {

        }

        private void btnGiveUp_Click(object sender, EventArgs e)
        {

        }
    }
}
