using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class GameController
    {
        private List<Player> players;
        private BagOfTiles bag;
        private GameBoard board;
        private int currentPlayerIndex = 0;
        private bool isGameActive = true;
        private Dictionary<Player, int> finalScores = new Dictionary<Player, int>();

        public event Action<Player> OnPlayerTurnStarted;
        public event Action<List<string>, int> OnWordValidationStarted;
        public event Action<Player, int> OnPlayerScored;
        public event Action<Player> OnPlayerResigned;
        public event Action<Player> OnGameEnded;

        public GameController(List<Player> players)
        {
            this.players = players;
            bag = new BagOfTiles();
            board = new GameBoard();
            InitializePlayers();
        }

        private void InitializePlayers() // раздает по 7 фишек из мешка и запускает первый ход
        {
            foreach (var player in players)
            {
                player.Hand = bag.DrawTiles(7);
            }
            StartNextTurn();
        }

        public void EndTurn(List<(int row, int col, Tile tile)> placedTiles) // завершает ход игрока
        {
            var currentPlayer = GetCurrentPlayer();

            // Проверяем корректность хода
            if (!board.IsValidMove(placedTiles))
            {
                // Возвращаем фишки игроку
                ReturnTilesToPlayer(placedTiles, currentPlayer);
                return;
            }

            // Запускаем модерацию слов
            if (placedTiles.Count > 0)
            {
                var (words, score) = board.CalculateTurnScore(placedTiles);
                StartWordValidation(words, score);
            }
            else
            {
                // Если игрок ничего не поставил, просто переходим к следующему ходу
                NextPlayer();
            }
        }

        private void ReturnTilesToPlayer(List<(int row, int col, Tile tile)> tiles, Player player) // возвращает фишки на руку игрока, если ход невалиден
        {
            foreach (var (row, col, tile) in tiles)
            {
                board.RemoveTile(row, col);
                player.Hand.Add(tile);
            }
        }

        private void StartWordValidation(List<string> words, int score) // проверка слов
        {
            OnWordValidationStarted?.Invoke(words, score); 

            // В реальном приложении здесь будет ожидание голосования игроков
            // Для примера сразу принимаем слова
            CompleteTurn(words, score);
        }

        private void CompleteTurn(List<string> words, int score) 
        {
            var currentPlayer = GetCurrentPlayer();
            currentPlayer.AddScore(score); // добавляет очки игроку
            OnPlayerScored?.Invoke(currentPlayer, score);

            // Обновляем руку игрока
            RefillPlayerHand(currentPlayer);

            // Переходим к следующему игроку
            NextPlayer();
        }

        private void RefillPlayerHand(Player player) // дополняет руку игрока до 7 фишек из мешка, если есть фишки
        {
            int tilesNeeded = 7 - player.Hand.Count;
            if (tilesNeeded > 0 && bag.RemainingCount > 0)
            {
                var newTiles = bag.DrawTiles(tilesNeeded);
                player.Hand.AddRange(newTiles);
            }
        }

        public void NextPlayer() // переходит к следующему активному игроку
        {
            do
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            } while (players[currentPlayerIndex].HasResigned); // пропускает сдавшихся игроков

            if (IsGameOver())
            {
                EndGame();
            }
            else
            {
                StartNextTurn();
            }
        }

        public void StartNextTurn() // уведомляет о начале хода текущего игрока через событие
        {
            var currentPlayer = GetCurrentPlayer();
            OnPlayerTurnStarted?.Invoke(currentPlayer);
        }

        public void ResignPlayer(Player player) // обрабатывает сдачу игрока
        {
            player.Resign(); // отмечает как сдавшегося
            finalScores[player] = player.Score; // сохраняет его финальный счет
            OnPlayerResigned?.Invoke(player);

            if (IsGameOver())
            {
                EndGame();
            }
        }

        private bool IsGameOver() // проверка на окончание игры
        {
            var activePlayers = players.Where(p => !p.HasResigned).ToList();

            // Если осталось 2 игрока и один сдался — второй выигрывает
            if (players.Count == 2 && activePlayers.Count == 1)
                return true;

            // Если осталось меньше 2 активных игроков
            return activePlayers.Count < 2;
        }

        private void EndGame() // завершение игры
        {
            isGameActive = false;

            // Собираем финальные результаты
            foreach (var player in players)
            {
                if (!finalScores.ContainsKey(player))
                    finalScores[player] = player.Score;
            }

            var winner = players // определяет победителя
                .Where(p => !p.HasResigned)
                .OrderByDescending(p => p.Score)
                .FirstOrDefault();

            OnGameEnded?.Invoke(winner);
        }

        public Player GetCurrentPlayer() => players[currentPlayerIndex]; 

        public GameBoard GetBoard() => board;

        public BagOfTiles GetBag() => bag;

        public List<Player> GetPlayers() => players;

        public bool IsActive() => isGameActive;

        public Dictionary<Player, int> GetFinalScores() => finalScores;

        // Метод для обмена фишек
        public void ExchangeTiles(Player player, List<Tile> tilesToExchange)
        {
            if (!isGameActive || player != GetCurrentPlayer()) return;

            // Возвращаем фишки в мешок
            bag.ReturnTiles(tilesToExchange);

            // Удаляем фишки из руки игрока
            foreach (var tile in tilesToExchange)
            {
                player.Hand.Remove(tile);
            }

            // Даём новые фишки
            var newTiles = bag.DrawTiles(tilesToExchange.Count);
            player.Hand.AddRange(newTiles);

            // Пропускаем ход
            NextPlayer();
        }

        // Проверка, может ли игрок обменять фишки (должно быть достаточно фишек в мешке)
        public bool CanExchangeTiles(int count) => bag.RemainingCount >= count;

        // Получение информации о текущем состоянии игры
        public GameState GetGameState()
        {
            return new GameState
            {
                CurrentPlayer = GetCurrentPlayer(),
                Players = players.ToList(),
                BoardState = board.GetBoardState(),
                RemainingTiles = bag.RemainingCount,
                IsActive = isGameActive
            };
        }
    }

    // Класс для передачи состояния игры
    public class GameState
    {
        public Player CurrentPlayer { get; set; }
        public List<Player> Players { get; set; }
        public string[,] BoardState { get; set; }
        public int RemainingTiles { get; set; }
        public bool IsActive { get; set; }
    }
}

