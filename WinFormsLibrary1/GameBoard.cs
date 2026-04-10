using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary
{
    public class GameBoard
    {
        private const int Size = 15;
        private Tile[,] grid;
        public bool[,] IsDoubleLetter { get; set; } = new bool[Size, Size];
        public bool[,] IsTripleLetter { get; set; } = new bool[Size, Size];
        public bool[,] IsDoubleWord { get; set; } = new bool[Size, Size];
        public bool[,] IsTripleWord { get; set; } = new bool[Size, Size];

        public GameBoard()
        {
            grid = new Tile[Size, Size];
            InitializePremiumCells();
        }

        private void InitializePremiumCells() { /* без изменений */ }

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
        public bool IsCellOccupied(int row, int col) =>
            row >= 0 && row < Size && col >= 0 && col < Size && grid[row, col] != null;

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

        // Основной метод подсчёта очков – теперь использует позиции слов
        public (List<string> words, int totalScore) CalculateTurnScore(List<(int row, int col, Tile tile)> placedTiles)
        {
            var words = new List<string>();
            int totalScore = 0;
            bool wordMultiplierApplied = false;
            var newTilePositions = placedTiles.Select(t => (t.row, t.col)).ToHashSet();

            foreach (var (row, col, tile) in placedTiles)
            {
                // Получаем слово и его клетки по горизонтали и вертикали
                var (horizontalWord, hPositions) = GetWordAndPositions(row, col, 0, 1);
                var (verticalWord, vPositions) = GetWordAndPositions(row, col, 1, 0);

                if (!string.IsNullOrEmpty(horizontalWord) && !words.Contains(horizontalWord))
                {
                    int wordScore = CalculateWordScoreWithPositions(horizontalWord, hPositions, newTilePositions, ref wordMultiplierApplied);
                    words.Add(horizontalWord);
                    totalScore += wordScore;
                }

                if (!string.IsNullOrEmpty(verticalWord) && !words.Contains(verticalWord))
                {
                    int wordScore = CalculateWordScoreWithPositions(verticalWord, vPositions, newTilePositions, ref wordMultiplierApplied);
                    words.Add(verticalWord);
                    totalScore += wordScore;
                }
            }

            return (words, totalScore);
        }

        // Возвращает слово и список координат клеток, из которых оно состоит
        private (string word, List<(int row, int col)> positions) GetWordAndPositions(int startRow, int startCol, int rowStep, int colStep)
        {
            var word = new StringBuilder();
            var positions = new List<(int, int)>();

            // Идём в начало слова
            int currentRow = startRow;
            int currentCol = startCol;
            while (currentRow - rowStep >= 0 && currentCol - colStep >= 0 &&
                   grid[currentRow - rowStep, currentCol - colStep] != null)
            {
                currentRow -= rowStep;
                currentCol -= colStep;
            }

            // Собираем слово и позиции
            while (currentRow < Size && currentCol < Size && grid[currentRow, currentCol] != null)
            {
                word.Append(grid[currentRow, currentCol].Letter);
                positions.Add((currentRow, currentCol));
                currentRow += rowStep;
                currentCol += colStep;
            }

            if (word.Length > 1)
                return (word.ToString(), positions);
            else
                return (string.Empty, new List<(int, int)>());
        }

        // Подсчёт очков по готовым позициям (без повторного обхода поля)
        private int CalculateWordScoreWithPositions(string word, List<(int row, int col)> positions,
                                                    HashSet<(int, int)> newTilePositions, ref bool wordMultiplierApplied)
        {
            int baseScore = 0;
            int wordMultiplier = 1;

            for (int i = 0; i < positions.Count; i++)
            {
                var (row, col) = positions[i];
                var tile = grid[row, col];
                if (tile == null)
                    throw new InvalidOperationException($"Нет фишки в клетке ({row},{col}) при подсчёте очков для слова {word}");

                int letterScore = tile.Weight;

                // Множители букв – только для новых клеток
                if (newTilePositions.Contains((row, col)))
                {
                    if (IsDoubleLetter[row, col])
                        letterScore *= 2;
                    else if (IsTripleLetter[row, col])
                        letterScore *= 3;
                }

                baseScore += letterScore;

                // Множители слов – только для новых клеток и только один раз за ход
                if (!wordMultiplierApplied && newTilePositions.Contains((row, col)))
                {
                    if (IsDoubleWord[row, col])
                        wordMultiplier *= 2;
                    else if (IsTripleWord[row, col])
                        wordMultiplier *= 3;
                }
            }

            if (wordMultiplier > 1)
                wordMultiplierApplied = true;

            return baseScore * wordMultiplier;
        }

        // Вспомогательный метод для обратной совместимости (если нужен)
        private string GetWordAtPosition(int startRow, int startCol, int rowStep, int colStep)
        {
            return GetWordAndPositions(startRow, startCol, rowStep, colStep).word;
        }

        public bool IsValidMove(List<(int row, int col, Tile tile)> placedTiles)
        {
            if (placedTiles.Count == 0) return false;

            // Первое размещение должно включать центральную клетку
            if (GetAllTilesCount() == 0) // первый ход
            {
                bool passesThroughCenter = placedTiles.Any(t => t.row == 7 && t.col == 7);
                if (!passesThroughCenter)
                {
                    return false; // первое слово должно проходить через (7,7)
                }
            }
            else // не первый ход
            {
                // Все фишки должны быть смежными
                if (!AreTilesConnected(placedTiles)) return false;

                // Фишки должны соединяться с существующими на поле
                if (!ConnectsToExistingTiles(placedTiles)) return false;
            }

            // Слова должны быть валидными (проверка через словарь)
            var (words, _) = CalculateTurnScore(placedTiles);
            return AreWordsValid(words);
        }

        private bool AreTilesConnected(List<(int row, int col, Tile tile)> placedTiles) // проверяет, что фишки размещены в одной линии и подряд.
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

        private bool ConnectsToExistingTiles(List<(int row, int col, Tile tile)> placedTiles) // проверяет, что хотя бы одна фишка касается уже существующих на поле
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

        private int GetAllTilesCount() // подсчитывает общее количество фишек на поле
        {
            int count = 0;
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (grid[r, c] != null) count++;
            return count;
        }

        private bool AreWordsValid(List<string> words) // проверяет слова на валидность
        {
            // Здесь должна быть интеграция со словарём
            // Для примера — простая проверка длины
            return words.All(word => word.Length >= 2);
        }

        public void ClearBoard() // очищает доску
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    grid[r, c] = null;
        }

        public List<(int row, int col)> GetEmptyCellsAround(int row, int col) // возвращает список пустых клеток вокруг заданной
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

        public string[,] GetBoardState() // возвращает текущее состояние поля в виде двумерного массива строк
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

        public bool IsBoardEmpty() // проверяет, пуста ли доска
        {
            return GetAllTilesCount() == 0;
        }
    }
}
