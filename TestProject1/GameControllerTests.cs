using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;

namespace TestProject1
{
    [TestClass]
    public sealed class GameControllerTests
    {
        [TestMethod]
        public void Constructor_ShouldInitializePlayersWith7TilesEachAndStartFirstTurn()
        {
            var players = new List<Player>
            {
                new Player("P1", 0),
                new Player("P2", 1)
            };
            var controller = new GameController(players);
            foreach (var p in players)
            {
                Assert.AreEqual(7, p.Hand.Count);
            }
            Assert.AreEqual(players[0], controller.GetCurrentPlayer());
        }

        [TestMethod]
        public void NextPlayer_ShouldSwitchToNextActivePlayer()
        {
            var players = new List<Player>
            {
                new Player("P1", 0),
                new Player("P2", 1),
                new Player("P3", 2)
            };
            var controller = new GameController(players);
            Assert.AreEqual(players[0], controller.GetCurrentPlayer());
            controller.NextPlayer();
            Assert.AreEqual(players[1], controller.GetCurrentPlayer());
            controller.NextPlayer();
            Assert.AreEqual(players[2], controller.GetCurrentPlayer());
            controller.NextPlayer();
            Assert.AreEqual(players[0], controller.GetCurrentPlayer());
        }

        [TestMethod]
        public void NextPlayer_ShouldSkipResignedPlayers()
        {
            var players = new List<Player>
            {
                new Player("P1", 0),
                new Player("P2", 1),
                new Player("P3", 2)
            };
            var controller = new GameController(players);
            players[1].Resign();
            controller.NextPlayer(); // от P0 -> должен перейти к P2, пропустив P1
            Assert.AreEqual(players[2], controller.GetCurrentPlayer());
        }

        [TestMethod]
        public void ResignPlayer_ShouldMarkPlayerAsResignedAndSaveScore()
        {
            var players = new List<Player>
            {
                new Player("P1", 0),
                new Player("P2", 1)
            };
            var controller = new GameController(players);
            players[0].AddScore(50);
            controller.ResignPlayer(players[0]);
            Assert.IsTrue(players[0].HasResigned);
            var finalScores = controller.GetFinalScores();
            Assert.AreEqual(50, finalScores[players[0]]);
        }

        [TestMethod]
        public void ResignPlayer_WhenTwoPlayersAndOneResigns_ShouldEndGameWithWinner()
        {
            var players = new List<Player>
            {
                new Player("P1", 0),
                new Player("P2", 1)
            };
            var controller = new GameController(players);
            Player winner = null;
            controller.OnGameEnded += (w) => winner = w;
            players[0].AddScore(30);
            players[1].AddScore(20);
            controller.ResignPlayer(players[0]); // P1 сдаётся
            Assert.IsTrue(players[0].HasResigned);
            Assert.IsNotNull(winner);
            Assert.AreEqual(players[1], winner);
        }


        [TestMethod]
        public void ExchangeTiles_OnlyCurrentPlayerCanExchange()
        {
            var players = new List<Player>
            {
                new Player("P1", 0),
                new Player("P2", 1)
            };
            var controller = new GameController(players);
            var notCurrent = players[1];
            var handBefore = notCurrent.Hand.ToList();
            var toExchange = handBefore.Take(2).ToList();
            controller.ExchangeTiles(notCurrent, toExchange);
            // Ничего не должно измениться
            Assert.AreEqual(7, notCurrent.Hand.Count);
            Assert.AreEqual(players[0], controller.GetCurrentPlayer());
            foreach (var tile in toExchange)
            {
                Assert.IsTrue(notCurrent.Hand.Contains(tile));
            }
        }

        [TestMethod]
        public void CanExchangeTiles_ShouldReturnTrueIfEnoughTilesInBag()
        {
            var players = new List<Player> { new Player("P1", 0) };
            var controller = new GameController(players);
            var bag = controller.GetBag();
            int remaining = bag.RemainingCount;
            Assert.IsTrue(controller.CanExchangeTiles(remaining));
            Assert.IsFalse(controller.CanExchangeTiles(remaining + 1));
        }

        [TestMethod]
        public void GetPlayersSortedByScore_ShouldReturnDescendingOrder()
        {
            var players = new List<Player>
            {
                new Player("P1", 0),
                new Player("P2", 1),
                new Player("P3", 2)
            };
            players[0].AddScore(10);
            players[1].AddScore(30);
            players[2].AddScore(20);
            var controller = new GameController(players);
            var sorted = controller.GetPlayersSortedByScore();
            Assert.AreEqual(players[1], sorted[0]);
            Assert.AreEqual(players[2], sorted[1]);
            Assert.AreEqual(players[0], sorted[2]);
        }

        [TestMethod]
        public void GetGameState_ShouldReturnCurrentState()
        {
            var players = new List<Player>
            {
                new Player("P1", 0),
                new Player("P2", 1)
            };
            var controller = new GameController(players);
            var state = controller.GetGameState();
            Assert.AreEqual(controller.GetCurrentPlayer(), state.CurrentPlayer);
            Assert.AreEqual(2, state.Players.Count);
            Assert.IsNotNull(state.BoardState);
            Assert.AreEqual(controller.GetBag().RemainingCount, state.RemainingTiles);
            Assert.IsTrue(state.IsActive);
        }
    }
}
