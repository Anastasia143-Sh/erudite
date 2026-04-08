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
        private Tile[,] _board = new Tile[Size, Size];
        private bool[,] _bonusCells = new bool[Size, Size]; // true — бонусная клетка
        private Dictionary<(int, int), string> _bonusTypes = new Dictionary<(int, int), string>();

        public GameBoard()
        {
            InitializeBonusCells();
        }

        private void InitializeBonusCells()
        {
            // Центральная клетка — Double Word (DW)
            _bonusCells[7, 7] = true;
            _bonusTypes[(7, 7)] = "DW";

            // Тройное слово (TW) — угловые зоны и симметричные позиции
            int[,] twPositions = {
                {0, 0}, {0, 7}, {0, 14},
                {7, 0}, {7, 14},
                {14, 0}, {14, 7}, {14, 14}
            };
            for (int i = 0; i < twPositions.GetLength(0); i++)
            {
                int row = twPositions[i, 0];
                int col = twPositions[i, 1];
                _bonusCells[row, col] = true;
                _bonusTypes[(row, col)] = "TW";
            }

            // Двойное слово (DW) — симметрично относительно центра
            int[,] dwPositions = {
                {1, 1}, {2, 2}, {3, 3}, {4, 4},
                {1, 13}, {2, 12}, {3, 11}, {4, 10},
                {13, 1}, {12, 2}, {11, 3}, {10, 4},
                {13, 13}, {12, 12}, {11, 11}, {10, 10}
            };
            for (int i = 0; i < dwPositions.GetLength(0); i++)
            {
                int row = dwPositions[i, 0];
                int col = dwPositions[i, 1];
                _bonusCells[row, col] = true;
                _bonusTypes[(row, col)] = "DW";
            }

            // Тройное письмо (TL) — ближе к центру, но не в центре
            int[,] tlPositions = {
                {0, 3}, {0, 11},
                {2, 6}, {2, 8},
                {3, 0}, {3, 7}, {3, 14},
                {6, 2}, {6, 6}, {6, 8}, {6, 12},
                {7, 3}, {7, 11},
                {8, 2}, {8, 6}, {8, 8}, {8, 12},
                {11, 0}, {11, 7}, {11, 14},
                {12, 6}, {12, 8},
                {14, 3}, {14, 11}
            };
            for (int i = 0; i < tlPositions.GetLength(0); i++)
            {
                int row = tlPositions[i, 0];
                int col = tlPositions[i, 1];
                _bonusCells[row, col] = true;
                _bonusTypes[(row, col)] = "TL";
            }

            // Двойное письмо (DL) — разбросано по полю
            int[,] dlPositions = {
                {1, 5}, {1, 9},
                {4, 1}, {4, 5}, {4, 9}, {4, 13},
                {5, 1}, {5, 3}, {5, 7}, {5, 11}, {5, 13},
                {9, 1}, {9, 3}, {9, 7}, {9, 11}, {9, 13},
                {10, 5}, {10, 9},
                {13, 1}, {13, 5}, {13, 9}, {13, 13}
            };
            for (int i = 0; i < dlPositions.GetLength(0); i++)
            {
                int row = dlPositions[i, 0];
                int col = dlPositions[i, 1];
                _bonusCells[row, col] = true;
                _bonusTypes[(row, col)] = "DL";
            }
        }

        public bool PlaceTile(Tile tile, int row, int col)
        {
            if (row < 0 || row >= Size || col < 0 || col >= Size)
                return false;

            if (_board[row, col] != null)
                return false;

            _board[row, col] = tile;
            return true;
        }

        public int CalculateScoreForWord(List<(int Row, int Col)> positions)
        {
            int wordScore = 0;
            int wordMultiplier = 1; // множитель для всего слова

            foreach (var (row, col) in positions)
            {
                var tile = _board[row, col];
                if (tile != null)
                {
                    int letterScore = tile.Value;

                    // Применяем бонусы за букву (DL, TL)
                    if (_bonusCells[row, col])
                    {
                        string bonusType = _bonusTypes[(row, col)];
                        switch (bonusType)
                        {
                            case "DL":
                                letterScore *= 2;
                                break;
                            case "TL":
                                letterScore *= 3;
                                break;
                            case "DW":
                                wordMultiplier *= 2;
                                break;
                            case "TW":
                                wordMultiplier *= 3;
                                break;
                        }
                    }

                    wordScore += letterScore;
                }
            }

            // Применяем множитель слова после суммирования всех букв
            wordScore *= wordMultiplier;
            return wordScore;
        }

        public bool IsCellEmpty(int row, int col) => _board[row, col] == null;

        // Дополнительный метод для получения типа бонуса клетки
        public string GetBonusType(int row, int col)
        {
            if (_bonusCells[row, col])
                return _bonusTypes[(row, col)];
            return "None";
        }

        // Метод для проверки, является ли клетка бонусной
        public bool IsBonusCell(int row, int col)
        {
            return _bonusCells[row, col];
        }

        // Метод для получения всех занятых клеток
        public List<(int Row, int Col)> GetOccupiedCells()
        {
            var occupied = new List<(int, int)>();
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (_board[r, c] != null)
                    {
                        occupied.Add((r, c));
                    }
                }
            }
            return occupied;
        }

        // Метод для очистки всей доски
        public void ClearBoard()
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    _board[r, c] = null;
                }
            }
        }

        // Метод для получения плитки на заданной позиции
        public Tile GetTileAt(int row, int col)
        {
            if (row >= 0 && row < Size && col >= 0 && col < Size)
            {
                return _board[row, col];
            }
            return null;
        }

        // Метод для проверки валидности позиции
        public bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < Size && col >= 0 && col < Size;
        }
    }
}
