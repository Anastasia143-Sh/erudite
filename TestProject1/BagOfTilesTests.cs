using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    [TestClass]
    public sealed class BagOfTilesTests
    {
        private int GetTotalTileCount()
        {
            // Суммарное количество фишек из GenerateTiles()
            return 8 + 4 + 4 + 9 + 6 + 4 + 5 + 10 + 4 + 5 + 5 + 5 +  // веса 1
                   2 + 3 + 4 + 3 + 3 +                                 // веса 2
                   2 + 2 +                                             // веса 3
                   2 + 2 +                                             // веса 4
                   1 + 1 + 1 + 1 +                                     // веса 5
                   1 + 1 +                                             // веса 8
                   1 + 1 + 1;                                          // веса 10
            // Итого: 69 + 15 + 4 + 4 + 4 + 2 + 3 = 101? Пересчитаем точно:
            // А:8, В:4, Д:4, Е:9, И:6, К:4, Н:5, О:10, П:4, Р:5, С:5, Т:5 = 8+4=12+4=16+9=25+6=31+4=35+5=40+10=50+4=54+5=59+5=64+5=69
            // Б:2, Г:3, Л:4, М:3, У:3 = 2+3=5+4=9+3=12+3=15, всего 69+15=84
            // Ы:2, Ь:2 = 4, всего 88
            // З:2, Я:2 = 4, всего 92
            // Ж:1, Х:1, Ц:1, Ч:1 = 4, всего 96
            // Ш:1, Э:1 = 2, всего 98
            // Ф:1, Щ:1, Ю:1 = 3, всего 101
            return 101;
        }

        [TestMethod]
        public void Constructor_ShouldCreateFullSetOfTiles()
        {
            var bag = new BagOfTiles();
            Assert.AreEqual(GetTotalTileCount(), bag.RemainingCount);
        }

        [TestMethod]
        public void DrawTiles_ShouldReturnRequestedCount()
        {
            var bag = new BagOfTiles();
            var drawn = bag.DrawTiles(5);
            Assert.AreEqual(5, drawn.Count);
            Assert.AreEqual(GetTotalTileCount() - 5, bag.RemainingCount);
        }

        [TestMethod]
        public void DrawTiles_WhenNotEnoughTiles_ShouldReturnRemaining()
        {
            var bag = new BagOfTiles();
            int total = GetTotalTileCount();
            var drawn1 = bag.DrawTiles(total);
            Assert.AreEqual(total, drawn1.Count);
            Assert.AreEqual(0, bag.RemainingCount);
            var drawn2 = bag.DrawTiles(10);
            Assert.AreEqual(0, drawn2.Count);
            Assert.AreEqual(0, bag.RemainingCount);
        }

        [TestMethod]
        public void ReturnTiles_ShouldIncreaseRemainingCount()
        {
            var bag = new BagOfTiles();
            var drawn = bag.DrawTiles(10);
            int remainingAfterDraw = bag.RemainingCount;
            bag.ReturnTiles(drawn);
            Assert.AreEqual(remainingAfterDraw + 10, bag.RemainingCount);
        }

        [TestMethod]
        public void ShuffleTiles_ShouldReturnSameListWithDifferentOrder()
        {
            var bag = new BagOfTiles();
            var original = bag.DrawTiles(20);
            var shuffled = bag.ShuffleTiles(new List<Tile>(original));
            Assert.AreEqual(original.Count, shuffled.Count);
            // Проверка, что порядок изменился (вероятностно, но почти всегда)
            bool orderChanged = false;
            for (int i = 0; i < original.Count; i++)
            {
                if (original[i] != shuffled[i])
                {
                    orderChanged = true;
                    break;
                }
            }
            Assert.IsTrue(orderChanged);
        }
    }
}
