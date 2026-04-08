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
        private HashSet<string> _dictionary; // словарь допустимых слов

        public GameController(List<Player> players, HashSet<string> dictionary)
        {
            _players = players;
            _dictionary = dictionary;
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

        public bool PlayWord(Player player, List<(char Letter, int Row, int Col)> wordData)
        {
            // Проверка, что слово есть в словаре
            string word = string.Concat(wordData.Select(w => w.Letter));
            if (!_dictionary.Contains(word))
                return false;

            // Размещение букв на поле
            foreach (var (letter, row, col) in wordData)
            {
                var tile = player.Hand.FirstOrDefault(t => t.Letter == letter);
                if (tile == null || !_board.PlaceTile(tile, row, col))
                    return false;
                player.RemoveTileFromHand(tile);
            }

            // Подсчёт очков
            var positions = wordData.Select(w => (w.Row, w.Col)).ToList();
            player.Score += _board.CalculateScoreForWord(positions);

            // Добор фишек
            player.AddTilesToHand(_bag.DrawTiles(wordData.Count));

            NextTurn();
            return true;
        }

        public void NextTurn()
        {
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
        }

        public Player GetCurrentPlayer() => _players[_currentPlayerIndex];

        public bool IsGameOver()
        {
            // Игра заканчивается, если мешок пуст и у одного игрока нет фишек
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
