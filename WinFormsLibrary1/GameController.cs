using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    internal class GameController
    {
        private List<Player> _players;
        private BagOfTiles _bag;
        private GameBoard _board;
        private int _currentPlayerIndex = 0;

        // Поля для модерации
        private string _pendingWord;
        private Player _pendingPlayer;
        private List<(Player Player, bool? VotedYes)> _votes;
        private List<(char Letter, int Row, int Col)> _pendingWordData;

        public GameController(List<Player> players)
        {
            _players = players;
            _bag = new BagOfTiles();
            _board = new GameBoard();
            InitializePlayers();
        }

        private void InitializePlayers()
        {
            foreach (var player in _players)
            {
                player.AddTilesToHand(_bag.DrawTiles(7));
            }
        }

        // Метод отправляет слово на модерацию (без проверки по словарю)
        public bool SubmitWordForModeration(Player player, List<(char Letter, int Row, int Col)> wordData)
        {
            string word = string.Concat(wordData.Select(w => w.Letter));

            _pendingWord = word;
            _pendingPlayer = player;
            _pendingWordData = wordData;
            _votes = _players
                .Where(p => p != player) // исключаем автора слова
                .Select(p => (p, (bool?)null)) // null — ещё не голосовал
                .ToList();

            return true; // слово отправлено на модерацию
        }

        // Метод для голосования
        public void VoteForWord(Player voter, bool voteYes)
        {
            var voteEntry = _votes.FirstOrDefault(v => v.Player == voter);
            if (voteEntry.Player != null)
            {
                _votes[_votes.IndexOf(voteEntry)] = (voter, voteYes);
            }
        }

        // Завершение модерации — подсчёт голосов
        public bool CompleteModeration()
        {
            if (_votes.Any(v => v.VotedYes == null))
                throw new InvalidOperationException("Не все игроки проголосовали!");

            int yesVotes = _votes.Count(v => v.VotedYes == true);
            int noVotes = _votes.Count(v => v.VotedYes == false);

            bool wordAccepted = yesVotes > noVotes;

            if (wordAccepted)
            {
                // Размещаем буквы на поле
                foreach (var (letter, row, col) in _pendingWordData)
                {
                    var tile = _pendingPlayer.Hand.FirstOrDefault(t => t.Letter == letter);
                    if (tile != null && _board.PlaceTile(tile, row, col))
                    {
                        _pendingPlayer.RemoveTileFromHand(tile);
                    }
                }

                // Подсчёт очков
                var positions = _pendingWordData.Select(w => (w.Row, w.Col)).ToList();
                _pendingPlayer.Score += _board.CalculateScoreForWord(positions);

                // Добор фишек
                _pendingPlayer.AddTilesToHand(_bag.DrawTiles(_pendingWordData.Count));
            }
            else
            {
                // Откат хода: возвращаем фишки в руку игрока
                // Здесь нужно корректно восстановить фишки — предположим, что у нас есть доступ к их значениям
                foreach (var (letter, _, _) in _pendingWordData)
                {
                    // Для восстановления фишки нужно знать её стоимость — в текущей модели она не хранится в _pendingWordData
                    // Возможное решение: хранить полный объект Tile вместо (char, int, int)
                    // Пока используем заглушку — на практике нужно доработать структуру данных
                    //_pendingPlayer.AddTileToHand(new Tile(letter, 0)); // 0 — временное значение
                }
            }

            // Очищаем данные модерации
            _pendingWord = null;
            _pendingPlayer = null;
            _pendingWordData = null;
            _votes = null;

            NextTurn();
            return wordAccepted;
        }

        public bool IsModerationActive => _pendingWord != null;

        public string GetPendingWord() => _pendingWord;
        public Player GetPendingPlayer() => _pendingPlayer;
        public List<(Player, bool?)> GetVotes() => _votes;

        public void NextTurn()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        }

        public Player GetCurrentPlayer() => _players[_currentPlayerIndex];

        public bool IsGameOver()
        {
            return _bag.RemainingTilesCount == 0 && _players.Any(p => p.Hand.Count == 0);
        }

        public Player GetWinner()
        {
            return _players.OrderByDescending(p => p.Score).First();
        }

        public void Surrender(Player player)
        {
            _players.Remove(player);
            if (_players.Count == 1)
            {
                // Последний оставшийся игрок — победитель
            }
        }
    }
}
