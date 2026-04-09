namespace ClassLibrary
{
    public class Tile
    {
        public char Letter { get; set; }
        public int Weight { get; set; }

        public Tile(char letter, int weight)
        {
            Letter = letter;
            Weight = weight;
        }
    }
}
