using Boardgames.NinthPlanet.Models;
using NUnit.Framework;

namespace Boardgames.NinthPlanet.Tests
{
    [TestFixture]
    public class DeckBuilderTests
    {
        [Test]
        public void GoalDeckContainsAllTheCards()
        {
            var goalDeck = DeckBuilder.CreateGoalDeck();

            // 4 colors 9 numbers
            Assert.AreEqual(4 * 9, goalDeck.Count);

            for (int i = 1; i <= 9; ++i)
            {
                Assert.Contains(new Card(CardColor.Blue, i), goalDeck);
                Assert.Contains(new Card(CardColor.Green, i), goalDeck);
                Assert.Contains(new Card(CardColor.Pink, i), goalDeck);
                Assert.Contains(new Card(CardColor.Yellow, i), goalDeck);
            }
        }

        [Test]
        public void DeckContainsAllTheCards()
        {
            var goalDeck = DeckBuilder.CreateCardDeck();

            // 4 colors 9 numbers and 4 rockets
            Assert.AreEqual(4 * 9 + 4, goalDeck.Count);

            for (int i = 1; i <= 4; ++i)
            {
                Assert.Contains(new Card(CardColor.Rocket, i), goalDeck);
            }

            for (int i = 1; i <= 9; ++i)
            {
                Assert.Contains(new Card(CardColor.Blue, i), goalDeck);
                Assert.Contains(new Card(CardColor.Green, i), goalDeck);
                Assert.Contains(new Card(CardColor.Pink, i), goalDeck);
                Assert.Contains(new Card(CardColor.Yellow, i), goalDeck);
            }
        }
    }
}