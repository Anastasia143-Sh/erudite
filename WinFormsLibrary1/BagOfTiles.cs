using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class BagOfTiles
    {
        private List<Tile> _tiles;

        public BagOfTiles()
        {
            _tiles = GenerateTiles();
            ShuffleTiles();
        }

        private List<Tile> GenerateTiles()
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

        private void ShuffleTiles()
        {
            var random = new Random();
            _tiles = _tiles.OrderBy(x => random.Next()).ToList(); // сортировка по случайным значениям
        }

        public List<Tile> DrawTiles(int count)
        {
            var drawnTiles = _tiles.Take(count).ToList(); // берет первые фишки
            _tiles.RemoveRange(0, drawnTiles.Count); // удаляет их из мешка
            return drawnTiles;
        }

        public void ReturnTiles(List<Tile> tiles)
        {
            _tiles.AddRange(tiles); // добавление переданных фишек в мешок
            ShuffleTiles();
        }
        public int RemainingTilesCount => _tiles.Count;
    }
}
