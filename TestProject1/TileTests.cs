using ClassLibrary;

namespace TestProject1
{
    [TestClass]
    public sealed class TileTests
    {
        [TestMethod]
        public void Constructor_ShouldSetLetterAndWeight()
        {
            var tile = new Tile('А', 2);
            Assert.AreEqual('А', tile.Letter);
            Assert.AreEqual(2, tile.Weight);
        }

        [TestMethod]
        public void Properties_ShouldBeSettable()
        {
            var tile = new Tile('Б', 3);
            tile.Letter = 'В';
            tile.Weight = 4;
            Assert.AreEqual('В', tile.Letter);
            Assert.AreEqual(4, tile.Weight);
        }
    }
}
