using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class GameBoard
    {
        private const int Size = 15;
        private Tile[,] grid; // хранение фишек на поле
        public bool[,] IsDoubleLetter { get; set; } = new bool[Size, Size]; // х2 за букву
        public bool[,] IsTripleLetter { get; set; } = new bool[Size, Size]; // х3 за букву
        public bool[,] IsDoubleWord { get; set; } = new bool[Size, Size]; // х2 за слово
        public bool[,] IsTripleWord { get; set; } = new bool[Size, Size]; // х3 за слово

        public GameBoard()
        {
            grid = new Tile[Size, Size];
            InitializePremiumCells();
        }

        private void InitializePremiumCells() // задает расположение премиальных клеток
        {
            // Центр — тройное слово
            IsTripleWord[7, 7] = true;

            // Двойные буквы (пример расположения)
            var doubleLetterPositions = new List<(int, int)>
            {
                (0, 3), (0, 11), (2, 6), (2, 8),
                (3, 0), (3, 7), (3, 13), (6, 2),
                (6, 6), (6, 8), (6, 12), (7, 3),
                (7, 11), (8, 12), (13, 1), (13, 13) // дописать
            };

            foreach (var (row, col) in doubleLetterPositions)
                IsDoubleLetter[row, col] = true;

            // Тройные буквы
            var tripleLetterPositions = new List<(int, int)>
            {
                (1, 5), (1, 9), (5, 1), (9, 1),
                (13, 5), (5, 13), (13, 9), (9, 13)
            };

            foreach (var (row, col) in tripleLetterPositions)
                IsTripleLetter[row, col] = true;

            // Двойные слова
            var doubleWordPositions = new List<(int, int)>
            {
                (1, 1), (2, 2), (3, 3), (4, 4),
                (13, 1), (12, 2), (11, 3), (10, 4),
                (1, 13), (2, 12), (3, 11), (4, ),
                (13, 13), (12, 12), (11, 11), (10, 10)
            };

            foreach (var (row, col) in doubleWordPositions)
                IsDoubleWord[row, col] = true;

            // Тройные слова
            var tripleWordPositions = new List<(int, int)>
            {
                (0, 0), (0, 14), (15, 0), (14, 14),
                (0, 7), (7, 0), (14, 7), (7, 14)
            };

            foreach (var (row, col) in tripleWordPositions)
                IsTripleWord[row, col] = true;
        }

        public bool PlaceTile(Tile tile, int row, int col)
        {
            if (row < 0 || row >= Size || col < 0 || col >= Size || grid[row, col] != null)
                return false;

            grid[row, col] = tile;
            return true;
        }

        public bool RemoveTile(int row, int col)
        {
            if (row < 0 || row >= Size || col < 0 || col >= Size || grid[row, col] == null)
                return false;

            grid[row, col] = null;
            return true;
        }

        public Tile GetTile(int row, int col) => grid[row, col];

        public bool IsCellOccupied(int row, int col)
        {
            return row >= 0 && row < Size && col >= 0 && col < Size && grid[row, col] != null;
        }

        public List<(int row, int col)> GetAdjacentTiles(int row, int col)
        {
            var adjacent = new List<(int, int)>();
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int newRow = row + dr[i];
                int newCol = col + dc[i];

                if (newRow >= 0 && newRow < Size && newCol >= 0 && newCol < Size && grid[newRow, newCol] != null)
                    adjacent.Add((newRow, newCol));
            }

            return adjacent;
        }

        public (List<string> words, int totalScore) CalculateTurnScore(List<(int row, int col, Tile tile)> placedTiles)
        {
            var words = new List<string>();
            int totalScore = 0;
            bool wordMultiplierApplied = false;

            foreach (var (row, col, tile) in placedTiles)
            {
                // Проверяем слова по горизонтали и вертикали
                var horizontalWord = GetWordAtPosition(row, col, 0, 1);
                var verticalWord = GetWordAtPosition(row, col, 1, 0);

                if (!string.IsNullOrEmpty(horizontalWord) && !words.Contains(horizontalWord))
                {
                    int wordScore = CalculateWordScoreWithMultipliers(horizontalWord, row, col, 0, 1, ref wordMultiplierApplied);
                    words.Add(horizontalWord);
                    totalScore += wordScore;
                }

                if (!string.IsNullOrEmpty(verticalWord) && !words.Contains(verticalWord))
                {
                    int wordScore = CalculateWordScoreWithMultipliers(verticalWord, row, col, 1, 0, ref wordMultiplierApplied);
                    words.Add(verticalWord);
                    totalScore += wordScore;
                }
            }

            return (words, totalScore);
        }

        private string GetWordAtPosition(int startRow, int startCol, int rowStep, int colStep)
        {
            var word = new StringBuilder();

            // Двигаемся влево/вверх до начала слова
            int currentRow = startRow;
            int currentCol = startCol;
            while (currentRow - rowStep >= 0 && currentCol - colStep >= 0 &&
                   grid[currentRow - rowStep, currentCol - colStep] != null)
            {
                currentRow -= rowStep;
                currentCol -= colStep;
            }

            // Собираем слово до конца
            while (currentRow < Size && currentCol < Size && grid[currentRow, currentCol] != null)
            {
                word.Append(grid[currentRow, currentCol].Letter);
                currentRow += rowStep;
                currentCol += colStep;
            }

            return word.Length > 1 ? word.ToString() : string.Empty;
        }

        private int CalculateWordScoreWithMultipliers(string word, int startRow, int startCol, int rowStep, int colStep, ref bool wordMultiplierApplied)
        {
            int baseScore = 0;
            int wordMultiplier = 1;

            int currentRow = startRow;
            int currentCol = startCol;

            for (int i = 0; i < word.Length; i++)
            {
                var tile = grid[currentRow, currentCol];
                int letterScore = tile.Weight;

                // Применяем множители букв
                if (IsDoubleLetter[currentRow, currentCol])
                    letterScore *= 2;
                else if (IsTripleLetter[currentRow, currentCol])
                    letterScore *= 3;

                baseScore += letterScore;

                // Запоминаем множители слов (применяются один раз за ход)
                if (!wordMultiplierApplied)
                {
                    if (IsDoubleWord[currentRow, currentCol])
                        wordMultiplier *= 2;
                    else if (IsTripleWord[currentRow, currentCol])
                        wordMultiplier *= 3;
                }

                currentRow += rowStep;
                currentCol += colStep;
            }

            if (!wordMultiplierApplied && wordMultiplier > 1)
            {
                wordMultiplierApplied = true; // Применяем только один раз за ход
            }

            return baseScore * wordMultiplier;
        }

        public bool IsValidMove(List<(int row, int col, Tile tile)> placedTiles)
        {
            if (placedTiles.Count == 0) return false;

            // Первое размещение должно включать центральную клетку
            if (GetAllTilesCount() == 0)
            {
                return placedTiles.Any(t => t.row == 7 && t.col == 7);
            }

            // Все фишки должны быть смежными
            if (!AreTilesConnected(placedTiles)) return false;

            // Фишки должны соединяться с существующими на поле
            if (!ConnectsToExistingTiles(placedTiles)) return false;

            // Слова должны быть валидными (проверка через словарь)
            var (words, _) = CalculateTurnScore(placedTiles);
            return AreWordsValid(words);
        }

        private bool AreTilesConnected(List<(int row, int col, Tile tile)> placedTiles)
        {
            // Простая проверка: все фишки должны быть в одной линии (горизонтальной или вертикальной)
            bool sameRow = placedTiles.All(t => t.row == placedTiles[0].row);
            bool sameCol = placedTiles.All(t => t.col == placedTiles[0].col);

            if (!(sameRow || sameCol)) return false;

            // Если в одной строке/столбце, проверяем, что они идут подряд
            var sorted = sameRow
                ? placedTiles.OrderBy(t => t.col).ToList()
                : placedTiles.OrderBy(t => t.row).ToList();

            for (int i = 1; i < sorted.Count; i++)
            {
                int prev = sameRow ? sorted[i - 1].col : sorted[i - 1].row;
                int curr = sameRow ? sorted[i].col : sorted[i].row;

                if (curr != prev + 1) return false;
            }

            return true;
        }

        private bool ConnectsToExistingTiles(List<(int row, int col, Tile tile)> placedTiles)
        {
            foreach (var (row, col, _) in placedTiles)
            {
                // Проверяем соседние клетки
                if (row > 0 && grid[row - 1, col] != null) return true;
                if (row < Size - 1 && grid[row + 1, col] != null) return true;
                if (col > 0 && grid[row, col - 1] != null) return true;
                if (col < Size - 1 && grid[row, col + 1] != null) return true;
            }
            return false;
        }

        private int GetAllTilesCount()
        {
            int count = 0;
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (grid[r, c] != null) count++;
            return count;
        }

        private bool AreWordsValid(List<string> words)
        {
            // Здесь должна быть интеграция со словарём
            // Для примера — простая проверка длины
            return words.All(word => word.Length >= 2);
        }

        public void ClearBoard()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    grid[r, c] = null;
        }

        public List<(int row, int col)> GetEmptyCellsAround(int row, int col)
        {
            var emptyCells = new List<(int, int)>();
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int newRow = row + dr[i];
                int newCol = col + dc[i];

                if (newRow >= 0 && newRow < Size && newCol >= 0 && newCol < Size && grid[newRow, newCol] == null)
                    emptyCells.Add((newRow, newCol));
            }

            return emptyCells;
        }

        public string[,] GetBoardState()
        {
            var state = new string[Size, Size];
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    state[r, c] = grid[r, c]?.Letter.ToString() ?? "";
                }
            }
            return state;
        }

        public bool IsBoardEmpty()
        {
            return GetAllTilesCount() == 0;
        }
    }
}
