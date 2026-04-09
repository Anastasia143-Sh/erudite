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
        public int ImageIndex { get; set; }
        public int Score { get; set; } = 0;
        public List<Tile> Hand { get; set; } = new List<Tile>();
        public bool HasResigned { get; set; } = false;

        public Player(string name, int imageIndex)
        {
            Name = name;
            ImageIndex = imageIndex;
        }

        public void AddScore(int points)
        {
            Score += points;
        }

        public void Resign()
        {
            HasResigned = true;
        }
    }

}
