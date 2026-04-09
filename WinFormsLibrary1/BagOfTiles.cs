using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class BagOfTiles
    {
        private List<Tile> tiles;

        public BagOfTiles()
        {
            tiles = GenerateTiles();
        }

        private List<Tile> GenerateTiles() // создаёт начальный набор фишек
        {
            var tiles = new List<Tile>();
            tiles.AddRange(Enumerable.Repeat(new Tile('А', 1), 8));
            tiles.AddRange(Enumerable.Repeat(new Tile('Б', 3), 2));
            tiles.AddRange(Enumerable.Repeat(new Tile('В', 1), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('Г', 3), 2));
            tiles.AddRange(Enumerable.Repeat(new Tile('Д', 2), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('Е', 1), 8));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ё', 3), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ж', 5), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('З', 5), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('И', 1), 5));
            tiles.AddRange(Enumerable.Repeat(new Tile('Й', 4), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('К', 2), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('Л', 2), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('М', 2), 3));
            tiles.AddRange(Enumerable.Repeat(new Tile('Н', 1), 5));
            tiles.AddRange(Enumerable.Repeat(new Tile('О', 1), 10));
            tiles.AddRange(Enumerable.Repeat(new Tile('П', 2), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('Р', 1), 5));
            tiles.AddRange(Enumerable.Repeat(new Tile('С', 1), 5));
            tiles.AddRange(Enumerable.Repeat(new Tile('Т', 1), 5));
            tiles.AddRange(Enumerable.Repeat(new Tile('У', 2), 4));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ф', 10), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Х', 5), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ц', 5), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ч', 5), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ш', 8), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Щ', 10), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ъ', 10), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ы', 4), 2));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ь', 3), 2));
            tiles.AddRange(Enumerable.Repeat(new Tile('Э', 8), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Ю', 8), 1));
            tiles.AddRange(Enumerable.Repeat(new Tile('Я', 2), 2));
            return tiles;
        }

        public List<Tile> DrawTiles(int count) //  выдаёт указанное количество фишек
        {
            var drawn = new List<Tile>();
            for (int i = 0; i < count && tiles.Count > 0; i++)
            {
                var randomIndex = new Random().Next(tiles.Count);
                drawn.Add(tiles[randomIndex]);
                tiles.RemoveAt(randomIndex);
            }
            return drawn;
        }

        public void ReturnTiles(List<Tile> returnedTiles) // возвращает фишки в мешок
        {
            tiles.AddRange(returnedTiles);
        }

        public int RemainingCount => tiles.Count; // свойство, возвращающее количество оставшихся фишек
    }
}
