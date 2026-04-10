using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary
{
    /// <summary>
    /// Представляет игровое поле размером 15×15 клеток
    /// Управляет размещением фишек, подсчётом очков с учётом премиальных клеток и проверкой корректности ходов
    /// </summary>
    public class GameBoard
    {
        private const int Size = 15;
        private Tile[,] grid; // фишки на поле
        public bool[,] IsDoubleLetter { get; set; } = new bool[Size, Size]; // массив, отмечающий клетки с удвоением очков за букву
        public bool[,] IsTripleLetter { get; set; } = new bool[Size, Size]; // массив, отмечающий клетки с утроением очков за букву
        public bool[,] IsDoubleWord { get; set; } = new bool[Size, Size]; // массив, отмечающий клетки с удвоением очков за всё слово
        public bool[,] IsTripleWord { get; set; } = new bool[Size, Size]; // массив, отмечающий клетки с утроением очков за всё слово

        public GameBoard()
        {
            grid = new Tile[Size, Size];
            InitializePremiumCells();
        }

        /// <summary>
        /// Задает расположение премиальных клеток
        /// </summary>
        private void InitializePremiumCells() 
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

        /// <summary>
        /// Размещает фишку на игровом поле в указанной клетке, если она пуста и находится в пределах поля
        /// </summary>
        /// <param name="tile">Фишка, которую нужно разместить</param>
        /// <param name="row">Номер строки (0–14)</param>
        /// <param name="col">Номер столбца (0–14)</param>
        /// <returns>True, если фишка успешно размещена, иначе — false</returns>
        public bool PlaceTile(Tile tile, int row, int col)
        {
            if (row < 0 || row >= Size || col < 0 || col >= Size || grid[row, col] != null)
                return false;
            grid[row, col] = tile;
            return true;
        }

        /// <summary>
        /// Удаляет фишку с игрового поля из указанной клетки, если она занята и находится в пределах поля
        /// </summary>
        /// <param name="row">Номер строки (0–14)</param>
        /// <param name="col">Номер столбца (0–14)</param>
        /// <returns>True, если фишка успешно удалена, иначе — false</returns>
        public bool RemoveTile(int row, int col)
        {
            if (row < 0 || row >= Size || col < 0 || col >= Size || grid[row, col] == null)
                return false;
            grid[row, col] = null;
            return true;
        }

        /// <summary>
        /// Получает фишку, находящуюся в указанной клетке игрового поля
        /// </summary>
        /// <param name="row">Номер строки (0–14)</param>
        /// <param name="col">Номер столбца (0–14)</param>
        /// <returns>Фишка в указанной клетке или null, если клетка пуста</returns>
        public Tile GetTile(int row, int col) => grid[row, col];

        /// <summary>
        /// Подсчитывает общий счёт за ход: находит все слова, образованные новыми фишками,
        /// и рассчитывает их очки с учётом премиальных клеток
        /// </summary>
        /// <param name="placedTiles">Список размещённых фишек с указанием их координат</param>
        /// <returns>Кортеж (список слов, общий счёт за ход)</returns>
        public (List<string> words, int totalScore) CalculateTurnScore(List<(int row, int col, Tile tile)> placedTiles)
        {
            var words = new List<string>(); // Список для хранения уникальных слов, образованных в текущем ходе
            int totalScore = 0; // Общий счёт за ход
            bool wordMultiplierApplied = false; // Флаг, отслеживающий применение множителя слова
            var newTilePositions = placedTiles.Select(t => (t.row, t.col)).ToHashSet(); // Множество координат новых размещённых фишек — для быстрой проверки, является ли клетка новой в текущем ходе

            // Обрабатываем каждую размещённую фишку — проверяем, образует ли она слова по горизонтали и вертикали
            foreach (var (row, col, tile) in placedTiles)
            {
                // Получаем слово и позиции всех его клеток по горизонтали (направление: 0 строк, 1 столбец — движение вправо)
                var (horizontalWord, hPositions) = GetWordAndPositions(row, col, 0, 1);

                // Получаем слово и позиции всех его клеток по вертикали (направление: 1 строка, 0 столбцов — движение вниз)
                var (verticalWord, vPositions) = GetWordAndPositions(row, col, 1, 0);

                // Проверяем горизонтальное слово: не пустое и ещё не учтено в общем списке слов
                if (!string.IsNullOrEmpty(horizontalWord) && !words.Contains(horizontalWord))
                {
                    int wordScore = CalculateWordScoreWithPositions(horizontalWord, hPositions, newTilePositions, ref wordMultiplierApplied); // Рассчитываем очки за слово

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

        /// <summary>
        /// Находит слово, проходящее через заданную клетку в указанном направлении, и собирает координаты всех его клеток
        /// </summary>
        /// <param name="startRow">Начальная строка для поиска слова</param>
        /// <param name="startCol">Начальный столбец для поиска слова</param>
        /// <param name="rowStep">Шаг по строкам (0 — горизонтально, 1 — вертикально)</param>
        /// <param name="colStep">Шаг по столбцам (1 — горизонтально, 0 — вертикально)</param>
        /// <returns>Кортеж (слово как строка, список координат клеток слова)</returns>
        private (string word, List<(int row, int col)> positions) GetWordAndPositions(int startRow, int startCol, int rowStep, int colStep)
        {
            
            var word = new StringBuilder(); // Создаём объект для постепенного построения строки слова
            
            var positions = new List<(int, int)>(); // Список для хранения координат всех клеток, входящих в найденное слово

            // Находим начало слова в указанном направлении
            // Перемещаемся назад по направлению движения (против rowStep/colStep), пока не достигнем края поля или пустой клетки
            int currentRow = startRow;
            int currentCol = startCol;
            while (currentRow - rowStep >= 0 && currentCol - colStep >= 0 &&
                   grid[currentRow - rowStep, currentCol - colStep] != null)
            {
                // Сдвигаемся на одну клетку назад 
                currentRow -= rowStep;
                currentCol -= colStep;
            }

            // Собираем буквы слова и их координаты, двигаясь вперёд от начала слова
            // Продолжаем, пока не выйдем за границы поля или не встретим пустую клетку
            while (currentRow < Size && currentCol < Size && grid[currentRow, currentCol] != null)
            {
                // Добавляем букву из текущей клетки в строку слова
                word.Append(grid[currentRow, currentCol].Letter);
                // Запоминаем координаты текущей клетки в списке позиций
                positions.Add((currentRow, currentCol));
                // Переходим к следующей клетке в заданном направлении
                currentRow += rowStep;
                currentCol += colStep;
            }

            // Проверяем валидность найденного слова
            // Слово считается действительным, только если состоит более чем из одной буквы
            if (word.Length > 1)
            {
                // Возвращаем собранное слово и список его позиций на игровом поле
                return (word.ToString(), positions);
            }
            else
            {
                // Если слово слишком короткое (1 буква или пустое), возвращаем пустые значения
                return (string.Empty, new List<(int, int)>());
            }
        }


        /// <summary>
        /// Рассчитывает очки за слово с учётом премиальных клеток и новых фишек
        /// Учитывает удвоение/утроение букв для новых фишек и применяет множитель слова один раз
        /// </summary>
        /// <param name="word">Слово, за которое начисляются очки</param>
        /// <param name="positions">Список координат клеток, составляющих слово</param>
        /// <param name="newTilePositions">Множество позиций новых фишек, размещённых в этом ходу</param>
        /// <param name="wordMultiplierApplied">Флаг, указывающий, был ли уже применён множитель слова</param>
        /// <returns>Количество очков за слово</returns>
        private int CalculateWordScoreWithPositions(
            string word,
            List<(int row, int col)> positions,
            HashSet<(int, int)> newTilePositions,
            ref bool wordMultiplierApplied)
        {
            int baseScore = 0; // Без учета множителей

            int wordMultiplier = 1; // Множитель для всего слова

            for (int i = 0; i < positions.Count; i++)
            {
                var (row, col) = positions[i];
                // Получаем фишку, расположенную на текущей клетке игрового поля
                var tile = grid[row, col];
                // Базовая стоимость буквы (без учёта премиальных клеток)
                int letterScore = tile.Weight;

                // Проверяем, является ли текущая клетка новой (размещённой в текущем ходу)
                bool isNew = newTilePositions.Contains((row, col));

                // Если фишка новая, применяем множители для отдельных букв (если есть)
                if (isNew)
                {
                    if (IsDoubleLetter[row, col])
                        letterScore *= 2; 
                    else if (IsTripleLetter[row, col])
                        letterScore *= 3; 
                }

                // Добавляем стоимость текущей буквы (с учётом возможных множителей) к общей базовой сумме
                baseScore += letterScore;

                // Применяем множители слова только один раз за ход и только для новых фишек
                if (!wordMultiplierApplied && isNew)
                {
                    if (IsDoubleWord[row, col])
                        wordMultiplier *= 2; 
                    else if (IsTripleWord[row, col])
                        wordMultiplier *= 3; 
                }
            }

            // Если был применён множитель слова, устанавливаем флаг, чтобы не применять его повторно для других слов в этом ходу
            if (wordMultiplier > 1)
                wordMultiplierApplied = true;

            // Итоговый счёт: базовая сумма умножается на множитель слова
            return baseScore * wordMultiplier;
        }


        /// <summary>
        /// Проверяет, что все новые фишки соединены друг с другом (через соседство)
        /// и хотя бы одна из них касается существующей фишки на поле
        /// Использует алгоритм BFS для проверки связности
        /// </summary>
        /// <param name="placedTiles">Список размещённых в ходе фишек с координатами</param>
        /// <param name="newTilePositions">Множество позиций новых фишек</param>
        /// <returns>True, если фишки образуют связную группу и касаются старых фишек, иначе — false</returns>
        private bool AreAllNewTilesConnectedToBoard(
            List<(int row, int col, Tile tile)> placedTiles,
            HashSet<(int, int)> newTilePositions)
        {
            // Создаём множество позиций существующих фишек (уже находящихся на поле до текущего хода)
            var existingPositions = new HashSet<(int, int)>();
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    // Если клетка занята фишкой и фишка не является новой (не из текущего хода) — добавляем в множество
                    if (grid[r, c] != null && !newTilePositions.Contains((r, c)))
                        existingPositions.Add((r, c));

            // Если на поле нет существующих фишек — это первый ход игры
            if (existingPositions.Count == 0)
                return false;

            // Объединяем позиции всех фишек: существующих и новых — для обхода связности
            var allPositions = new HashSet<(int, int)>(existingPositions);
            allPositions.UnionWith(newTilePositions);

            // Множество посещённых клеток в процессе обхода (чтобы не посещать повторно)
            var visited = new HashSet<(int, int)>();
            // Очередь для алгоритма BFS (поиск в ширину)
            var queue = new Queue<(int, int)>();

            // Начинаем обход с первой новой фишки (если новые фишки вообще есть)
            if (newTilePositions.Count > 0)
            {
                var start = newTilePositions.First();
                visited.Add(start); // отмечаем стартовую клетку как посещённую
                queue.Enqueue(start); // добавляем её в очередь для обработки
            }
            else return false; // если новых фишек нет — ход некорректен

            // Алгоритм BFS: обходим все достижимые клетки из стартовой позиции
            while (queue.Count > 0)
            {
                // Берём следующую клетку из очереди
                var (r, c) = queue.Dequeue();
                // Получаем координаты всех соседних клеток (по вертикали и горизонтали)
                foreach (var (nr, nc) in GetNeighbors(r, c))
                {
                    // Если соседняя клетка занята фишкой (новой или существующей) и ещё не посещена
                    if (allPositions.Contains((nr, nc)) && !visited.Contains((nr, nc)))
                    {
                        visited.Add((nr, nc)); // отмечаем как посещённую
                        queue.Enqueue((nr, nc)); // добавляем в очередь для дальнейшего обхода
                    }
                }
            }

            // Проверяем, что все новые фишки были посещены в процессе BFS\
            bool allNewVisited = newTilePositions.All(pos => visited.Contains(pos));

            // Проверяем, что хотя бы одна новая фишка граничит с уже существующей фишкой на поле\
            bool touchesExisting = newTilePositions.Any(pos =>
                GetNeighbors(pos.Item1, pos.Item2).Any(n => existingPositions.Contains(n)));

            return allNewVisited && touchesExisting;
        }

        /// <summary>
        /// Возвращает список соседних клеток для заданной клетки
        /// Используется для проверки связности фишек на поле
        /// </summary>
        /// <param name="row">Номер строки клетки (0–14)</param>
        /// <param name="col">Номер столбца клетки (0–14)</param>
        /// <returns>Набор координат соседних клеток в пределах поля</returns>
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

        /// <summary>
        /// Подсчитывает количество существующих (не новых) фишек на игровом поле
        /// Используется для определения, является ли текущий ход первым в игре
        /// </summary>
        /// <param name="newTilePositions">Множество позиций новых фишек, размещённых в текущем ходу</param>
        /// <returns>Количество фишек, уже находящихся на поле до текущего хода</returns>
        private int GetExistingTilesCount(HashSet<(int, int)> newTilePositions)
        {
            int count = 0;
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (grid[r, c] != null && !newTilePositions.Contains((r, c)))
                        count++;
            return count;
        }

        /// <summary>
        /// Проверяет корректность хода игрока
        /// </summary>
        /// <param name="placedTiles">Список размещённых фишек с указанием их координат и значений</param>
        /// <returns>True, если ход корректен, иначе — false</returns>
        public bool IsValidMove(List<(int row, int col, Tile tile)> placedTiles)
        {
            // Если не размещено ни одной фишки — ход некорректен
            if (placedTiles.Count == 0) return false;

            // Создаём множество координат новых размещённых фишек для быстрой проверки их наличия
            var newTilePositions = placedTiles.Select(t => (t.row, t.col)).ToHashSet();

            // Получаем количество существующих фишек на поле (не считая новых)
            int existingCount = GetExistingTilesCount(newTilePositions);

            // Проверяем, является ли текущий ход первым в игре (на поле нет старых фишек)
            if (existingCount == 0)
            {
                // При первом ходе хотя бы одна фишка должна быть размещена в центральной клетке (7, 7)
                bool passesThroughCenter = placedTiles.Any(t => t.row == 7 && t.col == 7);

                if (!passesThroughCenter)
                    return false;
            }
            else
            {
                // Для последующих ходов проверяем связность
                if (!AreAllNewTilesConnectedToBoard(placedTiles, newTilePositions))
                    return false;
            }
            var (words, _) = CalculateTurnScore(placedTiles);

            return AreWordsValid(words);
        }

        /// <summary>
        /// Проверяет валидность всех слов, образованных в ходе
        /// Длина каждого слова не менее 2 букв.
        /// </summary>
        /// <param name="words">Список слов, образованных новыми фишками</param>
        /// <returns>True, если все слова валидны, иначе — false</returns>
        private bool AreWordsValid(List<string> words) 
        {
            return words.All(word => word.Length >= 2);
        }

        /// <summary>
        /// Возвращает текущее состояние игрового поля в виде двумерного массива строк
        /// Каждая ячейка содержит букву фишки или пустую строку, если клетка пуста
        /// </summary>
        /// <returns>Двумерный массив строк размером 15×15, отражающий текущее состояние поля</returns>
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
    }
}
