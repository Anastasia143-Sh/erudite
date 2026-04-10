using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    /// <summary>
    /// Контроллер игры
    /// </summary>
    public class GameController
    {
        private List<Player> players;
        private BagOfTiles bag;
        private GameBoard board;
        private int currentPlayerIndex = 0; // индекс текущего игрока
        private bool isGameActive = true;
        private Dictionary<Player, int> finalScores = new Dictionary<Player, int>(); // словарь с финальными очками каждого игрока по завершении игры

        public event Action<Player> OnPlayerTurnStarted; // событие, возникающее в начале хода текущего игрока
        public event Action<List<string>, int> OnWordValidationStarted; // событие, сигнализирующее о начале проверки слов
        public event Action<Player, int> OnPlayerScored; // событие, уведомляющее о начислении очков игроку
        public event Action<Player> OnPlayerResigned; // событие, возникающее, когда игрок сдаётся
        public event Action<Player> OnGameEnded; // событие, сигнализирующее об окончании игры и объявлении победителя

        public GameController(List<Player> players)
        {
            this.players = players;
            bag = new BagOfTiles();
            board = new GameBoard();
            InitializePlayers();
        }

        /// <summary>
        /// Раздаёт каждому по 7 фишек из мешка и запускает первый ход
        /// </summary>
        private void InitializePlayers() 
        {
            foreach (var player in players)
            {
                player.Hand = bag.DrawTiles(7);
            }
            StartNextTurn();
        }

        /// <summary>
        /// Переходит к следующему активному игроку (пропускает сдавшихся)
        /// Проверяет, не закончилась ли игра, и если да, завершает её
        /// Иначе запускает ход следующего игрока
        /// </summary>
        public void NextPlayer() 
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

        /// <summary>
        /// Уведомляет о начале хода текущего игрока через событие
        /// </summary>
        public void StartNextTurn() 
        {
            var currentPlayer = GetCurrentPlayer();
            OnPlayerTurnStarted?.Invoke(currentPlayer);
        }

        /// <summary>
        /// Обрабатывает сдачу игрока: отмечает его как сдавшегося, сохраняет финальный счёт,
        /// проверяет, не закончилась ли игра, и передаёт ход следующему активному игроку
        /// </summary>
        /// <param name="player">Игрок, решивший сдаться</param>
        public void ResignPlayer(Player player)
        {
            player.Resign(); // отмечаем как сдавшегося
            finalScores[player] = player.Score; // сохраняем его финальный счёт
            OnPlayerResigned?.Invoke(player);

            if (IsGameOver())
            {
                EndGame();
            }
            else
            {
                NextPlayer(); 
            }
        }

        /// <summary>
        /// Проверяет, не закончилась ли игра
        /// </summary>
        /// <returns>True, если игра должна быть завершена, иначе — false.</returns>
        private bool IsGameOver() // проверка на окончание игры
        {
            var activePlayers = players.Where(p => !p.HasResigned).ToList();

            // Если осталось 2 игрока и один сдался — второй выигрывает
            if (players.Count == 2 && activePlayers.Count == 1)
                return true;

            // Если осталось меньше 2 активных игроков
            return activePlayers.Count < 2;
        }

        /// <summary>
        /// Завершает игру: фиксирует финальные результаты, определяет победителя и уведомляет о завершении через событие
        /// </summary>
        private void EndGame() 
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

        /// <summary>
        /// Получает текущего активного игрока по индексу
        /// </summary>
        /// <returns>Текущий игрок</returns>
        public Player GetCurrentPlayer() => players[currentPlayerIndex];

        /// <summary>
        /// Предоставляет доступ к игровому полю
        /// </summary>
        /// <returns>Экземпляр игрового поля</returns>
        public GameBoard GetBoard() => board;

        /// <summary>
        /// Предоставляет доступ к мешку с фишками
        /// </summary>
        /// <returns>Экземпляр мешка с фишками</returns>
        public BagOfTiles GetBag() => bag;

        /// <summary>
        /// Возвращает финальные результаты игры для всех игроков
        /// </summary>
        /// <returns>Словарь, где ключ — игрок, значение — его финальный счёт</returns>
        public Dictionary<Player, int> GetFinalScores() => finalScores;

        /// <summary>
        /// Позволяет игроку обменять выбранные фишки на новые из мешка
        /// Игрок пропускает ход после обмена
        /// </summary>
        /// <param name="player">Игрок, запрашивающий обмен</param>
        /// <param name="tilesToExchange">Список фишек, которые нужно обменять</param>
        public void ExchangeTiles(Player player, List<Tile> tilesToExchange)
        {
            if (player != GetCurrentPlayer()) return;

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

        /// <summary>
        /// Проверяет возможность обмена фишек: достаточно ли фишек в мешке для обмена указанного количества
        /// </summary>
        /// <param name="count">Количество фишек, которые планируется обменять</param>
        /// <returns>True, если в мешке достаточно фишек; иначе — false</returns>
        public bool CanExchangeTiles(int count) => bag.RemainingCount >= count;

        /// <summary>
        /// Формирует объект с текущим состоянием игры для передачи внешним компонентам
        /// </summary>
        /// <returns>Объект <see cref="GameState"/> с актуальной информацией о состоянии игры</returns>
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

