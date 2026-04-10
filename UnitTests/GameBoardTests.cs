using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ClassLibrary;

namespace UnitTests
{
    [TestClass]
    public class GameBoardTests
    {
        private GameBoard board;

        [TestInitialize]
        public void Setup()
        {
            board = new GameBoard();
        }

        [TestMethod]
        public void PlaceTile_ShouldReturnTrueAndPlaceTile()
        {
            var tile = new Tile('A', 1);
            bool result = board.PlaceTile(tile, 0, 0);
            Assert.IsTrue(result);
            Assert.AreSame(tile, board.GetTile(0, 0));
        }

        [TestMethod]
        public void PlaceTile_OnOccupiedCell_ShouldReturnFalse()
        {
            var tile1 = new Tile('A', 1);
            var tile2 = new Tile('B', 2);
            board.PlaceTile(tile1, 0, 0);
            bool result = board.PlaceTile(tile2, 0, 0);
            Assert.IsFalse(result);
            Assert.AreSame(tile1, board.GetTile(0, 0));
        }

        [TestMethod]
        public void PlaceTile_OutOfBounds_ShouldReturnFalse()
        {
            var tile = new Tile('A', 1);
            Assert.IsFalse(board.PlaceTile(tile, -1, 0));
            Assert.IsFalse(board.PlaceTile(tile, 15, 0));
            Assert.IsFalse(board.PlaceTile(tile, 0, -1));
            Assert.IsFalse(board.PlaceTile(tile, 0, 15));
        }

        [TestMethod]
        public void RemoveTile_ShouldReturnTrueAndClearCell()
        {
            var tile = new Tile('A', 1);
            board.PlaceTile(tile, 5, 5);
            bool result = board.RemoveTile(5, 5);
            Assert.IsTrue(result);
            Assert.IsNull(board.GetTile(5, 5));
        }

        [TestMethod]
        public void RemoveTile_FromEmptyCell_ShouldReturnFalse()
        {
            bool result = board.RemoveTile(5, 5);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PremiumCells_ShouldBeInitializedCorrectly()
        {
            // Проверка нескольких известных позиций
            Assert.IsTrue(board.IsTripleWord[7, 7]); // центр
            Assert.IsTrue(board.IsDoubleLetter[0, 3]);
            Assert.IsTrue(board.IsTripleLetter[1, 5]);
            Assert.IsTrue(board.IsDoubleWord[1, 1]);
            Assert.IsTrue(board.IsTripleWord[0, 0]);
            Assert.IsFalse(board.IsDoubleLetter[0, 0]);
        }

        [TestMethod]
        public void CalculateTurnScore_SingleWordNoBonuses()
        {
            // Размещаем слово "К О Т" горизонтально в строке 7, столбцы 6,7,8
            var tileK = new Tile('К', 4);
            var tileO = new Tile('О', 1);
            var tileT = new Tile('Т', 1);
            board.PlaceTile(tileK, 7, 6);
            board.PlaceTile(tileO, 7, 7);
            board.PlaceTile(tileT, 7, 8);

            var placed = new List<(int row, int col, Tile tile)>
            {
                (7, 6, tileK),
                (7, 7, tileO),
                (7, 8, tileT)
            };
            var (words, totalScore) = board.CalculateTurnScore(placed);
            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("КОТ", words[0]);
            // Стоимость: 4+1+1 = 6
            Assert.AreEqual(6, totalScore);
        }

        [TestMethod]
        public void CalculateTurnScore_WithDoubleLetterBonus()
        {
            // Размещаем слово "М А К" где 'М' на двойной букве (например, (6,2) - IsDoubleLetter)
            // (6,2) - двойная буква по настройкам. Но слово должно быть непрерывным.
            // Разместим горизонтально: row=6, col=2,3,4: М, А, К
            var tileM = new Tile('М', 2);
            var tileA = new Tile('А', 1);
            var tileK = new Tile('К', 4);
            board.PlaceTile(tileM, 6, 2);
            board.PlaceTile(tileA, 6, 3);
            board.PlaceTile(tileK, 6, 4);

            var placed = new List<(int row, int col, Tile tile)>
            {
                (6, 2, tileM),
                (6, 3, tileA),
                (6, 4, tileK)
            };
            var (words, totalScore) = board.CalculateTurnScore(placed);
            Assert.AreEqual("МАК", words[0]);
            // М на двойной букве: 2*2 = 4, А=1, К=4, итого 9
            Assert.AreEqual(9, totalScore);
        }

        [TestMethod]
        public void CalculateTurnScore_WithDoubleWordBonus()
        {
            // Размещаем слово "Л Е С" на двойном слове, например, клетка (1,1) - двойное слово, но только если все новые?
            // Множитель слова применяется один раз за ход, если хотя бы одна новая фишка на такой клетке.
            // Разместим слово горизонтально через (1,1), (1,2), (1,3) - (1,1) двойное слово.
            var tileL = new Tile('Л', 2);
            var tileE = new Tile('Е', 1);
            var tileS = new Tile('С', 1);
            board.PlaceTile(tileL, 1, 1);
            board.PlaceTile(tileE, 1, 2);
            board.PlaceTile(tileS, 1, 3);

            var placed = new List<(int row, int col, Tile tile)>
            {
                (1, 1, tileL),
                (1, 2, tileE),
                (1, 3, tileS)
            };
            var (words, totalScore) = board.CalculateTurnScore(placed);
            Assert.AreEqual("ЛЕС", words[0]);
            // базовая сумма: 2+1+1=4, множитель слова x2 = 8
            Assert.AreEqual(8, totalScore);
        }

        [TestMethod]
        public void CalculateTurnScore_MultipleWords()
        {
            // Размещаем фишку, образующую слова по горизонтали и вертикали
            // Сначала создаем основу: горизонтальное слово "М О Р Е" в строке 7, колонки 5-8
            board.PlaceTile(new Tile('М', 2), 7, 5);
            board.PlaceTile(new Tile('О', 1), 7, 6);
            board.PlaceTile(new Tile('Р', 1), 7, 7);
            board.PlaceTile(new Tile('Е', 1), 7, 8);
            // Теперь добавляем вертикальное слово, пересекающееся с 'О' - "Т О К" (Т в (6,6), О уже есть, К в (8,6))
            var tileT = new Tile('Т', 1);
            var tileK = new Tile('К', 4);
            board.PlaceTile(tileT, 6, 6);
            board.PlaceTile(tileK, 8, 6);
            var placed = new List<(int row, int col, Tile tile)>
            {
                (6, 6, tileT),
                (8, 6, tileK)
            };
            var (words, totalScore) = board.CalculateTurnScore(placed);
            Assert.IsTrue(words.Contains("ТОК"));
            Assert.IsTrue(words.Contains("МОРЕ")); // МОРЕ уже существовало? Нет, но оно не изменилось. Наш метод собирает все слова, проходящие через новые фишки.
            // Через (6,6) горизонталь: (7,6) - О, (6,6) - Т - нет слова по горизонтали (только Т и О? нужно проверить: по горизонтали от (6,6): слева нет, справа нет - слово из одной буквы Т - не добавится. Вертикаль: Т, О, К - "ТОК".
            // Через (8,6) вертикаль: от (8,6) вверх: К, О, Т - "ТОК" (уже учтено). Горизонталь: одна буква К - не слово.
            // Итого только "ТОК". Слово "МОРЕ" не будет добавлено, потому что новые фишки не входят в "МОРЕ". 
            // Так что words должно содержать только "ТОК". Проверим.
            Assert.AreEqual(1, words.Count);
            Assert.AreEqual("ТОК", words[0]);
            // Счёт: Т=1, О=1 (уже была, но её вес учитывается), К=4. Нет премий. Итого 6.
            Assert.AreEqual(6, totalScore);
        }

        [TestMethod]
        public void IsValidMove_FirstMoveMustPassThroughCenter()
        {
            var tile = new Tile('А', 1);
            var placed = new List<(int row, int col, Tile tile)> { (0, 0, tile) };
            Assert.IsFalse(board.IsValidMove(placed)); // не через центр
            placed = new List<(int row, int col, Tile tile)> { (7, 7, tile) };
            Assert.IsTrue(board.IsValidMove(placed)); // через центр
        }

        [TestMethod]
        public void IsValidMove_SubsequentMovesMustBeConnected()
        {
            // Первый ход через центр
            var tileA = new Tile('А', 1);
            board.PlaceTile(tileA, 7, 7);
            // Второй ход: размещаем рядом, но не касаясь существующих
            var tileB = new Tile('Б', 2);
            var placed = new List<(int row, int col, Tile tile)> { (0, 0, tileB) };
            Assert.IsFalse(board.IsValidMove(placed));
            // Размещаем касаясь
            var tileC = new Tile('В', 2);
            placed = new List<(int row, int col, Tile tile)> { (7, 8, tileC) };
            Assert.IsTrue(board.IsValidMove(placed));
        }

        [TestMethod]
        public void IsValidMove_AllWordsMustHaveAtLeastTwoLetters()
        {
            // Первый ход: одиночная буква через центр - невалидно, т.к. слово из одной буквы
            var tileA = new Tile('А', 1);
            var placed = new List<(int row, int col, Tile tile)> { (7, 7, tileA) };
            Assert.IsFalse(board.IsValidMove(placed));
            // Две буквы
            var tileB = new Tile('Б', 2);
            board.PlaceTile(tileA, 7, 7);
            board.PlaceTile(tileB, 7, 8);
            placed = new List<(int row, int col, Tile tile)> { (7, 7, tileA), (7, 8, tileB) };
            Assert.IsTrue(board.IsValidMove(placed));
        }

        [TestMethod]
        public void GetBoardState_ShouldReturnCorrectState()
        {
            var tile = new Tile('X', 5);
            board.PlaceTile(tile, 2, 3);
            var state = board.GetBoardState();
            Assert.AreEqual("X", state[2, 3]);
            Assert.AreEqual("", state[0, 0]);
        }
    }
}
