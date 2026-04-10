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
                (7, 11), (8, 2), (8, 6), (8, 8),
                (8, 12), (11, 0), (11, 7), (11, 13),
                (12, 6), (12, 8), (14, 3), (14, 11)
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
                (1, 13), (2, 12), (3, 11), (4, 10),
                (13, 13), (12, 12), (11, 11), (10, 10)
            };

            foreach (var (row, col) in doubleWordPositions)
                IsDoubleWord[row, col] = true;

            // Тройные слова
            var tripleWordPositions = new List<(int, int)>
            {
                (0, 0), (0, 14), (14, 0), (14, 14),
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
                int letterScore = tile.Weight;

                bool isNew = newTilePositions.Contains((row, col));

                if (isNew)
                {
                    if (IsDoubleLetter[row, col]) letterScore *= 2;
                    else if (IsTripleLetter[row, col]) letterScore *= 3;
                }

                baseScore += letterScore;

                if (!wordMultiplierApplied && isNew)
                {
                    if (IsDoubleWord[row, col]) wordMultiplier *= 2;
                    else if (IsTripleWord[row, col]) wordMultiplier *= 3;
                }
            }

            if (wordMultiplier > 1) wordMultiplierApplied = true;

            return baseScore * wordMultiplier;
        }

        // Вспомогательный метод для обратной совместимости (если нужен)
        private string GetWordAtPosition(int startRow, int startCol, int rowStep, int colStep)
        {
            return GetWordAndPositions(startRow, startCol, rowStep, colStep).word;
        }

        // Проверяет, что все новые фишки соединены друг с другом (через соседство) 
        // и хотя бы одна из них касается существующей фишки.
        private bool AreAllNewTilesConnectedToBoard(List<(int row, int col, Tile tile)> placedTiles, HashSet<(int, int)> newTilePositions)
        {
            // Находим все позиции существующих фишек (не новых)
            var existingPositions = new HashSet<(int, int)>();
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (grid[r, c] != null && !newTilePositions.Contains((r, c)))
                        existingPositions.Add((r, c));

            // Если нет существующих фишек, то это первый ход (проверка центра делается отдельно)
            if (existingPositions.Count == 0)
                return false; // первый ход обрабатывается в IsValidMove отдельно

            // BFS по всем новым фишкам и существующим, чтобы проверить связность
            var allPositions = new HashSet<(int, int)>(existingPositions);
            allPositions.UnionWith(newTilePositions);

            var visited = new HashSet<(int, int)>();
            var queue = new Queue<(int, int)>();

            // Начинаем обход с первой новой фишки (если есть)
            if (newTilePositions.Count > 0)
            {
                var start = newTilePositions.First();
                visited.Add(start);
                queue.Enqueue(start);
            }
            else return false;

            while (queue.Count > 0)
            {
                var (r, c) = queue.Dequeue();
                foreach (var (nr, nc) in GetNeighbors(r, c))
                {
                    if (allPositions.Contains((nr, nc)) && !visited.Contains((nr, nc)))
                    {
                        visited.Add((nr, nc));
                        queue.Enqueue((nr, nc));
                    }
                }
            }

            // Все ли новые фишки посещены?
            bool allNewVisited = newTilePositions.All(pos => visited.Contains(pos));

            // Хотя бы одна новая фишка касается существующей?
            bool touchesExisting = newTilePositions.Any(pos =>
                GetNeighbors(pos.Item1, pos.Item2).Any(n => existingPositions.Contains(n)));


            return allNewVisited && touchesExisting;
        }

        // Возвращает соседние клетки (по горизонтали/вертикали) без проверки границ
        private IEnumerable<(int, int)> GetNeighbors(int row, int col)
        {
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };
            for (int i = 0; i < 4; i++)
            {
                int nr = row + dr[i];
                int nc = col + dc[i];
                if (nr >= 0 && nr < Size && nc >= 0 && nc < Size)
                    yield return (nr, nc);
            }
        }

        // Количество существующих фишек (не учитывая новые)
        private int GetExistingTilesCount(HashSet<(int, int)> newTilePositions)
        {
            int count = 0;
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (grid[r, c] != null && !newTilePositions.Contains((r, c)))
                        count++;
            return count;
        }

        // Исправленный метод IsValidMove
        public bool IsValidMove(List<(int row, int col, Tile tile)> placedTiles)
        {
            if (placedTiles.Count == 0) return false;

            var newTilePositions = placedTiles.Select(t => (t.row, t.col)).ToHashSet();
            int existingCount = GetExistingTilesCount(newTilePositions);

            // Первый ход (на поле нет старых фишек)
            if (existingCount == 0)
            {
                bool passesThroughCenter = placedTiles.Any(t => t.row == 7 && t.col == 7);
                if (!passesThroughCenter)
                    return false;
            }
            else
            {
                // Не первый ход: проверяем связность новых фишек между собой и касание старых
                if (!AreAllNewTilesConnectedToBoard(placedTiles, newTilePositions))
                    return false;
            }

            // Проверка слов через словарь (длина >= 2)
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
