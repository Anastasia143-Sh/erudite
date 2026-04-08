using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Player
    {
        public string Name { get; set; }
        public string CharacterImagePath { get; set; } // путь к изображению персонажа
        public List<Tile> Hand { get; set; } = new List<Tile>();
        public int Score { get; set; } = 0;

        public Player(string name, string characterImagePath)
        {
            Name = name;
            CharacterImagePath = characterImagePath;
        }

        public void AddTilesToHand(List<Tile> tiles)
        {
            Hand.AddRange(tiles);
        }

        public void RemoveTileFromHand(Tile tile)
        {
            Hand.Remove(tile);
        }
    }
}
