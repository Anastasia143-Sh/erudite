using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    /// <summary>
    /// Игрок
    /// </summary>
    public class Player
    {
        public string Name { get; set; }
        public int ImageIndex { get; set; }
        public int Score { get; set; } = 0;
        public List<Tile> Hand { get; set; } = new List<Tile>();
        public bool HasResigned { get; set; } = false; // сдался игрок или нет

        public Player(string name, int imageIndex)
        {
            Name = name;
            ImageIndex = imageIndex;
        }

        /// <summary>
        /// Добавляет указанное количество очков к текущему счёту игрока
        /// </summary>
        /// <param name="points">Количество очков для добавления</param>
        public void AddScore(int points)
        {
            Score += points;
        }

        /// <summary>
        /// Отмечает игрока как сдавшегося (вышедшего из игры)
        /// </summary>
        public void Resign() 
        {
            HasResigned = true;
        }
    }

}
