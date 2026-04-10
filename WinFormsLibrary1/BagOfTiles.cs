using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    /// <summary>
    /// Мешок с фишками
    /// </summary>
    public class BagOfTiles
    {
        private List<Tile> tiles;
        private Random random = new Random(); 

        public BagOfTiles()
        {
            tiles = GenerateTiles();
            ShuffleTiles(tiles);
        }

        /// <summary>
        /// Создаёт начальный набор фишек(с учётом частоты букв и их стоимости)
        /// </summary>
        /// <returns>Список фишек, составляющих полный набор игры</returns>
        private List<Tile> GenerateTiles() 
        {
            var tiles = new List<Tile>();

            tiles.AddRange(Enumerable.Repeat(new Tile('А', 1), 8));
            tiles.AddRange(Enumerable.Repeat(new Tile('В', 1), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('Д', 1), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('Е', 1), 9));
            tiles.AddRange(Enumerable.Repeat(new Tile('И', 1), 6));
            tiles.AddRange(Enumerable.Repeat(new Tile('К', 1), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('Н', 1), 5));
            tiles.AddRange(Enumerable.Repeat(new Tile('О', 1), 10));
            tiles.AddRange(Enumerable.Repeat(new Tile('П', 1), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('Р', 1), 5));
            tiles.AddRange(Enumerable.Repeat(new Tile('С', 1), 5));
            tiles.AddRange(Enumerable.Repeat(new Tile('Т', 1), 5));

            tiles.AddRange(Enumerable.Repeat(new Tile('Б', 2), 2));
            tiles.AddRange(Enumerable.Repeat(new Tile('Г', 2), 3));
            tiles.AddRange(Enumerable.Repeat(new Tile('Л', 2), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('М', 2), 3));
            tiles.AddRange(Enumerable.Repeat(new Tile('У', 2), 3));

            tiles.AddRange(Enumerable.Repeat(new Tile('Ы', 3), 2));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ь', 3), 2));

            tiles.AddRange(Enumerable.Repeat(new Tile('З', 4), 2));
            tiles.AddRange(Enumerable.Repeat(new Tile('Я', 4), 2));

            tiles.AddRange(Enumerable.Repeat(new Tile('Ж', 5), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Х', 5), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ц', 5), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ч', 5), 1));

            tiles.AddRange(Enumerable.Repeat(new Tile('Ш', 8), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Э', 8), 1));

            tiles.AddRange(Enumerable.Repeat(new Tile('Ф', 10), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Щ', 10), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ю', 10), 1));

            return tiles;
        }

        /// <summary>
        /// Выдаёт из мешка указанное количество случайных фишек. Если фишек недостаточно, возвращает все оставшиеся
        /// </summary>
        /// <param name="count">Количество фишек для выдачи</param>
        /// <returns>Список выданных фишек</returns>
        public List<Tile> DrawTiles(int count) 
        {
            var drawn = new List<Tile>();
            for (int i = 0; i < count && tiles.Count > 0; i++)
            {
                var randomIndex = random.Next(tiles.Count);
                drawn.Add(tiles[randomIndex]);
                tiles.RemoveAt(randomIndex);
            }
            return drawn;
        }

        /// <summary>
        /// Перемешивает список фишек случайным образом
        /// </summary>
        /// <param name="tiles">Список фишек для перемешивания</param>
        /// <returns>Перемешанный список фишек</returns>
        public List<Tile> ShuffleTiles(List<Tile> tiles) 
        {
            return tiles.OrderBy(x => random.Next()).ToList();
        }

        /// <summary>
        /// Возвращает указанные фишки обратно в мешок и перемешивает
        /// </summary>
        /// <param name="returnedTiles">Список фишек, которые нужно вернуть в мешок</param>
        public void ReturnTiles(List<Tile> returnedTiles) 
        {
            tiles.AddRange(returnedTiles);
            ShuffleTiles(tiles);
        }

        /// <summary>
        /// Свойство только для чтения, возвращающее текущее количество оставшихся в мешке фишек
        /// Позволяет отслеживать, сколько фишек ещё доступно для раздачи игрокам
        /// </summary>
        public int RemainingCount => tiles.Count; 
    }
}
