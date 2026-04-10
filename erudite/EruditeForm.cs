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
        private BagOfTiles _bagOfTiles;
        private InitialForm _previousForm;
        private List<Player> _players = new List<Player>();
        private PictureBox[,] boardCells = new PictureBox[15, 15];
        private const int CELL_SIZE = 40;
        private Tile _selectedTile; // Выбранная фишка
        private Button _selectedTileButton; // Кнопка с выбранной фишкой
        private (int row, int col)? _targetCell; // Целевая клетка (если выбрана)
        private Stack<(int row, int col, Tile tile)> _placedTilesDuringTurn = new Stack<(int, int, Tile)>(); // Хранит размещённые фишки текущего хода
        private bool _isTurnInProgress = false; // Флаг: идёт ли текущий ход

        public EruditeForm(InitialForm previousForm, List<Player> players)
        {
            InitializeComponent();
            _previousForm = previousForm;
            _players = players;
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
                // визуальные компоненты
                InitializeBoardVisuals();
                InitializePlayerHand();
                _bagOfTiles = gameController.GetBag();
                lblCountChips.Text = $"{_bagOfTiles.RemainingCount}";
                // обновление игроков и счета
                //UpdatePlayerLabels();
                UpdateScoreDisplay();

                gameController.StartNextTurn();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке игры: {ex.Message}");
                _previousForm.Show();
                this.Close();
            }
        }

        private void OnCurrentPlayerChanged(Player player) // обновляет интерфейс при смене игрока (аватар, руку)
        {
            UpdatePlayerLabels();
            InitializePlayerHand();
        }

        private void OnPlayerScored(Player player, int score) // показывает сообщение о набранных очках и обновляет счёт
        {
            UpdateScoreDisplay();
            MessageBox.Show($"{player.Name} набрал {score} очков!");
        }

        private void ShowWordValidationDialog(List<string> words, int score) // отображает проверяемые слова и очки
        {
            MessageBox.Show($"Проверяемые слова: {string.Join(", ", words)}\nОчки: {score}");
        }

        private void OnGameFinished(Player winner) // показывает победителя, возвращает к главному меню
        {
            MessageBox.Show($"Игра окончена! Победитель: {winner.Name}");
            _previousForm.Show();
            this.Close();
        }

        private void InitializeBoardVisuals() // создаёт 225 кнопок (15×15) для клеток поля, добавляет их в boardPanel;
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

        private Button CreateBoardCell(int row, int col) // создаёт одну кнопку-клетку с координатами в Tag
        {
            Button button = new Button
            {
                Size = new Size(40, 40),
                Location = new Point(col * 40, row * 40),
                Tag = (row, col),
                FlatStyle = FlatStyle.Flat
            };
            button.Click += BoardCell_Click;
            return button;
        }

        private void BoardCell_Click(object sender, EventArgs e)
        {
            Button clickedCell = (Button)sender;
            var (row, col) = ((int, int))clickedCell.Tag;

            // Если фишка не выбрана, просто показываем информацию о клетке
            if (_selectedTile == null)
            {
                MessageBox.Show($"Клик по клетке: {row + 1}, {col + 1}");
                return;
            }

            // Сохраняем целевую клетку
            _targetCell = (row, col);

            // Пытаемся разместить фишку
            bool success = gameController.GetBoard().PlaceTile(_selectedTile, row, col);

            if (success)
            {
                // Устанавливаем флаг, что ход начался
                _isTurnInProgress = true;

                // Записываем размещённую фишку в стек
                _placedTilesDuringTurn.Push((row, col, _selectedTile));

                // Удаляем фишку из руки игрока
                var currentPlayer = gameController.GetCurrentPlayer();
                currentPlayer.Hand.Remove(_selectedTile);

                // Обновляем интерфейс
                UpdateBoardVisuals();
                InitializePlayerHand(); // Обновляем руку игрока

                // Сбрасываем выделение
                _selectedTile = null;
                if (_selectedTileButton != null)
                {
                    _selectedTileButton.BackColor = SystemColors.Control;
                    _selectedTileButton = null;
                }
                _targetCell = null;
            }
            else
            {
                MessageBox.Show("Не удалось разместить фишку на этой клетке!\nВозможно, клетка занята или ход невалиден.");
            }
        }


        private void UpdateBoardVisuals()
        {
            foreach (Control control in boardPanel.Controls)
            {
                if (control is Button cell && cell.Tag is (int row, int col))
                {
                    var tile = gameController.GetBoard().GetTile(row, col);
                    if (tile != null)
                    {
                        cell.Text = tile.Letter.ToString();
                        cell.Enabled = false;
                        cell.BackColor = Color.GhostWhite;
                    }
                    else
                    {
                        cell.Text = "";
                        cell.Enabled = true;
                        cell.BackColor = Color.Transparent;
                    }
                }
            }
        }


        private void InitializePlayerHand() // очищает панель руки, создаёт кнопки для фишек текущего игрока
        {
            handPanel.Controls.Clear();
            var currentPlayer = gameController.GetCurrentPlayer();

            for (int i = 0; i < currentPlayer.Hand.Count; i++)
            {
                Button tileButton = CreateTileButton(currentPlayer.Hand[i], i);
                handPanel.Controls.Add(tileButton);
            }
        }

        private Button CreateTileButton(Tile tile, int index) // создаёт кнопку для одной фишки (буква, стиль)
        {
            Button button = new Button
            {
                Text = tile.Letter.ToString(),
                Size = new Size(40, 40),
                Location = new Point(index * 40, 0),
                Tag = tile,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            button.Click += TileButton_Click;
            return button;
        }

        private void TileButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            Tile tile = (Tile)clickedButton.Tag;

            // Если та же фишка уже выбрана — снимаем выделение
            if (_selectedTile == tile)
            {
                _selectedTile = null;
                _selectedTileButton = null;
                clickedButton.BackColor = SystemColors.Control; // Сбрасываем цвет
                return;
            }

            // Выделяем новую фишку
            _selectedTile = tile;
            _selectedTileButton = clickedButton;
            clickedButton.BackColor = Color.LightBlue; // Визуальное выделение

            // Сбрасываем выбор клетки — игрок может выбрать новую
            _targetCell = null;
        }


        private void UpdatePlayerLabels() // обновляет аватар и имя текущего игрока
        {
            // Обновляем аватар текущего игрока
            switch (gameController.GetCurrentPlayer().ImageIndex)
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
            lblInfo.Text = $"Сейчас ходит: {gameController.GetCurrentPlayer().Name}";
            lblName.Text = gameController.GetCurrentPlayer().Name;
        }

        private void UpdateScoreDisplay() // обновляет таблицу счёта для всех игроков 
        {
            var scoreLabels = new[]
            {
                (lblName1Player, lblScores1Player),
                (lblName2Player, lblScores2Player),
                (lblName3Player, lblScores3Player),
                (lblName4Player, lblScores4Player)
            };

            for (int i = 0; i < scoreLabels.Length; i++)
            {
                var (nameLabel, scoreLabel) = scoreLabels[i];
                if (i < _players.Count)
                {
                    nameLabel.Text = _players[i].Name;
                    scoreLabel.Text = _players[i].Score.ToString();
                    nameLabel.Visible = true;
                    scoreLabel.Visible = true;
                }
                else
                {
                    nameLabel.Visible = false;
                    scoreLabel.Visible = false;
                }
            }
        }

        // Метод для добавления очков текущему игроку
        private void AddScoreToCurrentPlayer(int points)
        {
            gameController.GetCurrentPlayer().Score += points;
            UpdateScoreDisplay(); // Обновляем отображение сразу после изменения
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Показываем диалоговое окно с подтверждением
            DialogResult result = MessageBox.Show
            (
                "Вы уверены, что хотите завершить игру?\nПрогресс будет потерян.",
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

        private void btnComplete_Click(object sender, EventArgs e)
        {

        }

        private void btnExchangeChips_Click(object sender, EventArgs e)
        {

        }

        private void btnGiveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnCancell_Click(object sender, EventArgs e)
        {
            // Проверяем, есть ли что отменять
            if (!_isTurnInProgress || _placedTilesDuringTurn.Count == 0)
            {
                MessageBox.Show("Нет ходов для отмены.");
                return;
            }

            // Берём последнюю размещённую фишку
            var (row, col, tile) = _placedTilesDuringTurn.Pop();

            // Удаляем фишку с поля
            gameController.GetBoard().RemoveTile(row, col);

            // Возвращаем фишку в руку текущего игрока
            var currentPlayer = gameController.GetCurrentPlayer();
            currentPlayer.Hand.Add(tile);

            // Обновляем интерфейс
            UpdateBoardVisuals();
            InitializePlayerHand();

            // Если стек пуст — завершаем ход
            if (_placedTilesDuringTurn.Count == 0)
            {
                _isTurnInProgress = false;
                MessageBox.Show("Все фишки возвращены в руку. Ход отменён.");
            }
        }
    }
}