using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ClassLibrary;

namespace UnitTests
{
    [TestClass]
    public class PlayerTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializeProperties()
        {
            var player = new Player("Alice", 1);
            Assert.AreEqual("Alice", player.Name);
            Assert.AreEqual(1, player.ImageIndex);
            Assert.AreEqual(0, player.Score);
            Assert.IsNotNull(player.Hand);
            Assert.AreEqual(0, player.Hand.Count);
            Assert.IsFalse(player.HasResigned);
        }

        [TestMethod]
        public void AddScore_ShouldIncreaseScore()
        {
            var player = new Player("Bob", 2);
            player.AddScore(10);
            Assert.AreEqual(10, player.Score);
            player.AddScore(5);
            Assert.AreEqual(15, player.Score);
        }

        [TestMethod]
        public void Resign_ShouldSetHasResignedToTrue()
        {
            var player = new Player("Charlie", 3);
            player.Resign();
            Assert.IsTrue(player.HasResigned);
        }
    }
}
