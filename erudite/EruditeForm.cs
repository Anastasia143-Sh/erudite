using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace erudite
{
    /// <summary>
    /// Основная форма 
    /// Отображает игровое поле, руку игрока, счёт, реализует логику размещения фишек, голосования и обмена
    /// </summary>
    public partial class EruditeForm : Form
    {
        private GameController gameController;
        private BagOfTiles _bagOfTiles;
        private InitialForm _previousForm;
        private List<Player> _players = new List<Player>();
        private const int CELL_SIZE = 40;
        private Tile _selectedTile;
        private Button _selectedTileButton;
        private (int row, int col)? _targetCell;
        private Stack<(int row, int col, Tile tile)> _placedTilesDuringTurn = new Stack<(int, int, Tile)>();
        private bool _isTurnInProgress = false; 
        private bool _isFix = false; // зафиксировано ли слово в текущем ходу

        private List<(int row, int col)> _fixedTiles = new List<(int, int)>(); // зафиксированные (но ещё не принятые) клетки
        private List<(int row, int col)> _acceptedTiles = new List<(int, int)>(); // уже принятые клетки (сохраняются между ходами)
        private int _tempScore = 0; // временные очки за зафиксированное слово (до голосования)
        private List<string> _wordsToVote = new List<string>(); // слова, ожидающие голосования
        //private bool _canPlace = true; // разрешено ли размещать новые фишки (после фиксации – false)


        public EruditeForm(InitialForm previousForm, List<Player> players)
        {
            InitializeComponent();
            _previousForm = previousForm;
            _players = players;
            this.Load += EruditeForm_Load;
        }

        /// <summary>
        /// Обработчик события загрузки формы
        /// Инициализирует игровой контроллер, подписывается на события, настраивает визуальные элементы
        /// При ошибке возвращает пользователя на начальную форму
        /// </summary>
        private void EruditeForm_Load(object sender, EventArgs e)
        {
            try
            {
                gameController = new GameController(_players);
                gameController.OnPlayerTurnStarted += OnCurrentPlayerChanged;
                gameController.OnPlayerScored += OnPlayerScored;
                gameController.OnGameEnded += OnGameFinished;

                InitializeBoardVisuals();
                InitializePlayerHand();
                _bagOfTiles = gameController.GetBag();
                lblCountChips.Text = $"{_bagOfTiles.RemainingCount}";
                UpdateScoreDisplay();

                // Сброс состояния хода
                ResetTurnState();

                gameController.StartNextTurn();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке игры: {ex.Message}");
                _previousForm.Show();
                this.Close();
            }
        }

        /// <summary>
        /// Сбрасывает переменные, связанные с текущим ходом (вызывается при начале нового хода)
        /// Очищает временные данные, обновляет визуальное состояние поля
        /// </summary>
        private void ResetTurnState()
        {
            _isTurnInProgress = false;
            _isFix = false;
            //_canPlace = true;
            _placedTilesDuringTurn.Clear();
            _fixedTiles.Clear();
            _wordsToVote.Clear();
            _tempScore = 0;
            _selectedTile = null;
            _selectedTileButton = null;
            _targetCell = null;
            UpdateBoardVisuals();  
        }

        /// <summary>
        /// Обработчик смены текущего игрока
        /// Обновляет интерфейс, отображает информацию о текущем игроке, показывает сообщение о передаче хода
        /// </summary>
        /// <param name="player">Текущий игрок, которому передаётся ход</param>
        private void OnCurrentPlayerChanged(Player player)
        {
            ResetTurnState();
            InitializePlayerHand();
            UpdatePlayerLabels();
            UpdateScoreDisplay();
            // Сообщение о передаче хода (требование 5)
            MessageBox.Show($"Ход передается игроку: {player.Name}. Передайте ему/ей ноутбук, после нажмите ОК", "Передача хода");
        }

        /// <summary>
        /// Обработчик начисления очков игроку
        /// Обновляет отображение счёта и показывает уведомление о набранных очках
        /// </summary>
        /// <param name="player">Игрок, которому начислены очки</param>
        /// <param name="score">Количество начисленных очков</param>
        private void OnPlayerScored(Player player, int score)
        {
            UpdateScoreDisplay();
            MessageBox.Show($"{player.Name} набрал {score} очков!");
        }

        /// <summary>
        /// Обработчик завершения игры
        /// Отображает сообщение о победителе и возвращает пользователя на начальную форму
        /// </summary>
        /// <param name="winner">Игрок, победивший в игре</param>
        private void OnGameFinished(Player winner)
        {
            var results = gameController.GetFinalScores();
            // Если в finalScores нет кого-то, добавим из players
            var allPlayers = gameController.GetPlayersSortedByScore();
            var sb = new StringBuilder();
            sb.AppendLine($"Игра окончена! Победитель: {winner?.Name ?? "никто"}");
            sb.AppendLine("Результаты:");
            foreach (var player in allPlayers)
            {
                int score = results.ContainsKey(player) ? results[player] : player.Score;
                sb.AppendLine($"{player.Name}: {score} очков");
            }
            MessageBox.Show(sb.ToString(), "Конец игры");
            _previousForm.Show();
            this.Close();
        }

        /// <summary>
        /// Инициализирует визуальное представление игрового поля (15×15 клеток)
        /// Создаёт кнопки для каждой клетки и добавляет их на панель boardPanel
        /// </summary>
        private void InitializeBoardVisuals()
        {
            boardPanel.SuspendLayout();
            var buttonsToAdd = new List<Button>();
            for (int row = 0; row < 15; row++)
                for (int col = 0; col < 15; col++)
                    buttonsToAdd.Add(CreateBoardCell(row, col));
            boardPanel.Controls.AddRange(buttonsToAdd.ToArray());
            boardPanel.ResumeLayout(false);
            boardPanel.PerformLayout();
        }

        /// <summary>
        /// Создаёт кнопку для клетки игрового поля
        /// Настраивает внешний вид и обработчик клика
        /// </summary>
        /// <param name="row">Индекс строки клетки</param>
        /// <param name="col">Индекс столбца клетки</param>
        /// <returns>Кнопка, представляющая клетку поля</returns>
        private Button CreateBoardCell(int row, int col)
        {
            Button button = new Button
            {
                Size = new Size(CELL_SIZE, CELL_SIZE),
                Location = new Point(col * CELL_SIZE, row * CELL_SIZE),
                Tag = (row, col),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                Text = "",
                Enabled = true
            };
            button.Click += BoardCell_Click;
            return button;
        }

        /// <summary>
        /// Обработчик клика по клетке игрового поля
        /// Размещает выбранную фишку на поле, если это разрешено правилами
        /// </summary>
        private void BoardCell_Click(object sender, EventArgs e)
        {

            Button clickedCell = (Button)sender;
            var (row, col) = ((int, int))clickedCell.Tag;

            if (_selectedTile == null)
            {
                MessageBox.Show($"Клик по клетке: {row + 1}, {col + 1}");
                return;
            }

            _targetCell = (row, col);
            bool success = gameController.GetBoard().PlaceTile(_selectedTile, row, col);
            if (success)
            {
                _isTurnInProgress = true;
                _placedTilesDuringTurn.Push((row, col, _selectedTile));
                var currentPlayer = gameController.GetCurrentPlayer();
                currentPlayer.Hand.Remove(_selectedTile);

                // Обновляем интерфейс: цвет клетки – GhostWhite (размещена, но не зафиксирована)
                UpdateBoardVisuals();
                InitializePlayerHand();

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
                MessageBox.Show("Не удалось разместить фишку на этой клетке");
            }
        }

        /// <summary>
        /// Обновляет внешний вид игрового поля с учётом статуса клеток (принятые, зафиксированные, размещённые)
        /// Изменяет текст и цвет кнопок клеток в зависимости от наличия фишек и их статуса
        /// </summary>
        private void UpdateBoardVisuals()
        {
            boardPanel.SuspendLayout();
            foreach (Control control in boardPanel.Controls)
            {
                if (control is Button cell && cell.Tag is (int row, int col))
                {
                    var tile = gameController.GetBoard().GetTile(row, col);
                    bool hasTile = tile != null;
                    bool wasTile = !string.IsNullOrEmpty(cell.Text);

                    if (hasTile != wasTile || (hasTile && cell.Text != tile.Letter.ToString()))
                    {
                        cell.Text = hasTile ? tile.Letter.ToString() : "";
                        cell.Enabled = !hasTile;
                    }

                    // Определяем цвет клетки
                    if (_acceptedTiles.Contains((row, col)))
                        cell.BackColor = Color.Lavender; // принятые клетки
                    else if (_fixedTiles.Contains((row, col)))
                        cell.BackColor = Color.LightGreen; // зафиксированные (ожидают голосования)
                    else if (_placedTilesDuringTurn.Any(p => p.row == row && p.col == col))
                        cell.BackColor = Color.GhostWhite; // размещённые, но ещё не зафиксированные
                    else
                        cell.BackColor = Color.Transparent; // пустые
                }
            }
            boardPanel.ResumeLayout(true);
        }

        /// <summary>
        /// Инициализирует отображение руки текущего игрока (набор фишек)
        /// Очищает панель handPanel и добавляет кнопки для каждой фишки в руке игрока
        /// </summary>
        private void InitializePlayerHand()
        {
            handPanel.SuspendLayout();
            handPanel.Controls.Clear();
            var currentPlayer = gameController.GetCurrentPlayer();
            var buttonsToAdd = new List<Button>();
            for (int i = 0; i < currentPlayer.Hand.Count; i++)
                buttonsToAdd.Add(CreateTileButton(currentPlayer.Hand[i], i));
            handPanel.Controls.AddRange(buttonsToAdd.ToArray());
            handPanel.ResumeLayout(true);
        }

        /// <summary>
        /// Создаёт кнопку для фишки в руке игрока 
        /// Настраивает внешний вид и обработчик клика
        /// </summary>
        /// <param name="tile">Фишка, которую представляет кнопка</param>
        /// <param name="index">Индекс фишки в руке игрока</param>
        /// <returns>Кнопка, представляющая фишку</returns>
        private Button CreateTileButton(Tile tile, int index)
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

        /// <summary>
        /// Обработчик клика по кнопке фишки в руке игрока
        /// Выбирает или отменяет выбор фишки, изменяет цвет кнопки
        /// </summary>
        private void TileButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            Tile tile = (Tile)clickedButton.Tag;

            if (_selectedTile == tile)
            {
                _selectedTile = null;
                _selectedTileButton = null;
                clickedButton.BackColor = SystemColors.Control;
                return;
            }

            _selectedTile = tile;
            _selectedTileButton = clickedButton;
            clickedButton.BackColor = Color.LightBlue;
            _targetCell = null;
        }

        /// <summary>
        /// Обновляет отображение имён и счетов игроков на экране
        /// Заполняет метки для до 4 игроков, скрывает лишние, если игроков меньше
        /// </summary>
        private void UpdatePlayerLabels()
        {
            var cur = gameController.GetCurrentPlayer();
            switch (cur.ImageIndex)
            {
                case 0: pictureBox1.Image = Properties.Resources.крош2; break;
                case 1: pictureBox1.Image = Properties.Resources.ежик1; break;
                case 2: pictureBox1.Image = Properties.Resources.бараш1; break;
                case 3: pictureBox1.Image = Properties.Resources.совунья; break;
                case 4: pictureBox1.Image = Properties.Resources.нюша; break;
                case 5: pictureBox1.Image = Properties.Resources.карыч; break;
                case 6: pictureBox1.Image = Properties.Resources.копатыч; break;
                case 7: pictureBox1.Image = Properties.Resources.лосяш; break;
                case 8: pictureBox1.Image = Properties.Resources.пин; break;
            }
            lblInfo.Text = $"Сейчас ходит: {cur.Name}";
            lblName.Text = cur.Name;
        }

        /// <summary>
        /// Обновляет отображение счёта всех игроков
        /// Заполняет метки с именами и очками для до 4 игроков
        /// </summary>
        private void UpdateScoreDisplay()
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


        /// <summary>
        /// Обработчик нажатия кнопки «Фиксация» (btnFix)
        /// Фиксирует размещённое слово, проверяет валидность хода, вычисляет очки и слова
        /// </summary>
        private void btnFix_Click(object sender, EventArgs e)
        {
            if (_placedTilesDuringTurn.Count == 0)
            {
                MessageBox.Show("Нет размещённых фишек для фиксации.");
                return;
            }

            var placedList = _placedTilesDuringTurn.ToList();
            

            // Валидация хода
            var moveTiles = placedList.Select(p => (p.row, p.col, p.tile)).ToList();
            if (!gameController.GetBoard().IsValidMove(moveTiles))
            {
                MessageBox.Show("Некорректный ход: слово не проходит через центр (первый ход) или не касается существующих букв.");
                return;
            }

            // Вычисляем очки и слова
            var (words, totalScore) = gameController.GetBoard().CalculateTurnScore(moveTiles);
            if (words.Count == 0 || totalScore == 0)
            {
                MessageBox.Show("Не удалось определить слово для фиксации.");
                return;
            }

            // Запоминаем слова и очки для голосования
            _wordsToVote.AddRange(words);
            _tempScore += totalScore;

            // Переносим фишки из временного стека в список зафиксированных
            foreach (var (row, col, tile) in placedList)
            {
                _fixedTiles.Add((row, col));
            }
            _placedTilesDuringTurn.Clear();

            _isFix = true;

            UpdateBoardVisuals();
            MessageBox.Show($"Слово зафиксировано! Начислено {totalScore} очков (будут учтены после голосования).");
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Завершить ход» (btnComplete)
        /// Если слово не зафиксировано, предлагает передать ход, если зафиксировано — запускает голосование
        /// </summary>
        private void btnComplete_Click(object sender, EventArgs e)
        {
            if (!_isFix)
            {
                DialogResult res = MessageBox.Show("Вы не вставили ни одного слова. Передать ход следующему?",
                    "Завершение хода", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    // Передаём ход, не начисляя очков
                    gameController.NextPlayer();
                }
                return;
            }

            // Есть зафиксированное слово – запускаем голосование
            StartVoting();
        }

        /// <summary>
        /// Запускает процесс голосования за зафиксированные слова.
        /// Показывает диалог голосования, обрабатывает результат (принятие/отклонение).
        /// </summary>
        private void StartVoting()
        {
            var currentPlayer = gameController.GetCurrentPlayer();
            var otherPlayers = _players.Where(p => p != currentPlayer && !p.HasResigned).ToList();

            using (var voteForm = new VoteForm(currentPlayer.Name, _wordsToVote, _tempScore, otherPlayers))
            {
                var result = voteForm.ShowDialog();
                if (result == DialogResult.OK && voteForm.Accepted)
                {
                    AcceptWords();
                }
                else
                {
                    RejectWords();
                }
            }
        }

        /// <summary>
        /// Принимает слова после успешного голосования
        /// Начисляет очки, добирает фишки до 7, обновляет интерфейс, передаёт ход
        /// </summary>
        private void AcceptWords()
        {
            // Фиксируем зафиксированные клетки как принятые
            _acceptedTiles.AddRange(_fixedTiles);
            _fixedTiles.Clear();

            // Начисляем очки
            var currentPlayer = gameController.GetCurrentPlayer();
            currentPlayer.AddScore(_tempScore);
            UpdateScoreDisplay();

            // Добираем фишки до 7
            RefillPlayerHand(currentPlayer);

            // Сбрасываем временные переменные
            _wordsToVote.Clear();
            _tempScore = 0;
            _isFix = false;
            //_canPlace = true;

            UpdateBoardVisuals(); // клетки станут Lavender
            InitializePlayerHand();

            // Передаём ход
            gameController.NextPlayer();
        }

        /// <summary>
        /// Отклоняет слова после голосования
        /// Возвращает фишки игроку, предлагает продолжить ход или передать его следующему
        /// </summary>
        private void RejectWords()
        {
            var currentPlayer = gameController.GetCurrentPlayer();
            // Возвращаем фишки с поля в руку
            foreach (var (row, col) in _fixedTiles)
            {
                var tile = gameController.GetBoard().GetTile(row, col);
                if (tile != null)
                {
                    gameController.GetBoard().RemoveTile(row, col);
                    currentPlayer.Hand.Add(tile);
                }
            }
            _fixedTiles.Clear();
            _wordsToVote.Clear();
            _tempScore = 0;
            _isFix = false;
            //_canPlace = true;

            UpdateBoardVisuals();
            InitializePlayerHand();

            DialogResult res = MessageBox.Show("Слова отклонены. Продолжить ввод слов или передать ход следующему?",
                "Отклонено", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (res == DialogResult.Yes)
            {
                // Продолжаем ход – игрок может снова размещать фишки
            }
            else
            {
                gameController.NextPlayer();
            }
        }

        /// <summary>
        /// Добирает фишки игроку до 7 из мешка, если это возможно
        /// Обновляет отображение количества оставшихся фишек
        /// </summary>
        /// <param name="player">Игрок, которому нужно добрать фишки</param>
        private void RefillPlayerHand(Player player)
        {
            int needed = 7 - player.Hand.Count;
            if (needed > 0 && _bagOfTiles.RemainingCount > 0)
            {
                var newTiles = _bagOfTiles.DrawTiles(needed);
                player.Hand.AddRange(newTiles);
                lblCountChips.Text = $"{_bagOfTiles.RemainingCount}";
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Отмена» (btnCancell)
        /// Отменяет последнюю размещённую фишку (до фиксации слова), возвращает её игроку
        /// </summary>
        private void btnCancell_Click(object sender, EventArgs e)
        {
            if (_isFix)
            {
                MessageBox.Show("Нельзя отменять фишки после фиксации слова.");
                return;
            }
            if (_placedTilesDuringTurn.Count == 0)
            {
                MessageBox.Show("Нет ходов для отмены.");
                return;
            }

            var (row, col, tile) = _placedTilesDuringTurn.Pop();
            gameController.GetBoard().RemoveTile(row, col);
            var currentPlayer = gameController.GetCurrentPlayer();
            currentPlayer.Hand.Add(tile);
            UpdateBoardVisuals();
            InitializePlayerHand();

            if (_placedTilesDuringTurn.Count == 0)
                _isTurnInProgress = false;
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Обмен фишек» (btnExchangeChips)
        /// Позволяет игроку обменять фишки, если нет незафиксированных фишек и достаточно фишек в мешке
        /// </summary>
        private void btnExchangeChips_Click(object sender, EventArgs e)
        {
            var currentPlayer = gameController.GetCurrentPlayer();

            if (_isFix || _placedTilesDuringTurn.Count > 0)
            {
                MessageBox.Show("Нельзя обменивать фишки после фиксации слова или при наличии незафиксированных фишек. Завершите или отмените ход.");
                return;
            }

            if (!gameController.CanExchangeTiles(currentPlayer.Hand.Count))
            {
                MessageBox.Show($"В мешке недостаточно фишек для обмена. Осталось: {_bagOfTiles.RemainingCount}");
                return;
            }

            var selectedTiles = ShowExchangeSelectionDialog(currentPlayer.Hand);
            if (selectedTiles == null || selectedTiles.Count == 0) return;

            DialogResult confirm = MessageBox.Show($"Вы уверены, что хотите обменять {selectedTiles.Count} фишек? Ход будет передан следующему игроку.",
                "Подтверждение обмена", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No) return;

            // Выполняем обмен
            gameController.ExchangeTiles(currentPlayer, selectedTiles);
            InitializePlayerHand();
            lblCountChips.Text = $"{_bagOfTiles.RemainingCount}";
            ResetTurnState();
        }

        /// <summary>
        /// Показывает диалог выбора фишек для обмена
        /// Позволяет игроку выбрать фишки из своей руки для обмена, подсвечивает выбранные
        /// </summary>
        /// <param name="playerHand">Текущая рука игрока (список фишек)</param>
        /// <returns>Список выбранных фишек или null, если выбор отменён</returns>
        private List<Tile> ShowExchangeSelectionDialog(List<Tile> playerHand)
        {
            var selectedTiles = new List<Tile>();
            var form = new Form
            {
                Text = "Выберите фишки для обмена",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };
            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = true };
            var okButton = new Button { Text = "Обменять", Size = new Size(80, 30), DialogResult = DialogResult.OK, Enabled = false };
            var cancelButton = new Button { Text = "Отмена", Size = new Size(80, 30), DialogResult = DialogResult.Cancel };
            var buttonPanel = new Panel { Height = 40, Dock = DockStyle.Bottom };
            okButton.Location = new Point(100, 5);
            cancelButton.Location = new Point(200, 5);
            buttonPanel.Controls.AddRange(new Control[] { okButton, cancelButton });

            foreach (var tile in playerHand)
            {
                var btn = new Button
                {
                    Text = tile.Letter.ToString(),
                    Size = new Size(40, 40),
                    Tag = tile,
                    BackColor = Color.LightGray
                };
                btn.Click += (s, _) =>
                {
                    var b = (Button)s;
                    var t = (Tile)b.Tag;
                    if (selectedTiles.Contains(t))
                    {
                        selectedTiles.Remove(t);
                        b.BackColor = Color.LightGray;
                    }
                    else
                    {
                        selectedTiles.Add(t);
                        b.BackColor = Color.LightBlue;
                    }
                    okButton.Enabled = selectedTiles.Count > 0;
                };
                panel.Controls.Add(btn);
            }

            form.Controls.Add(panel);
            form.Controls.Add(buttonPanel);
            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            return form.ShowDialog() == DialogResult.OK ? selectedTiles : null;
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Сдаться» (btnGiveUp)
        /// Позволяет текущему игроку сдаться, выбывает из игры, передаёт ход следующему
        /// </summary>
        private void btnGiveUp_Click(object sender, EventArgs e)
        {
            var current = gameController.GetCurrentPlayer();
            DialogResult res = MessageBox.Show($"Вы уверены, что хотите сдаться? Игрок {current.Name} выбывает.",
                "Сдача", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.Yes)
            {
                gameController.ResignPlayer(current);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки «Выход» (btnExit)
        /// </summary>
        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите завершить игру?\nПрогресс будет потерян.",
                "Подтверждение выхода", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _previousForm.Show();
                this.Close();
            }
        }
    }
}