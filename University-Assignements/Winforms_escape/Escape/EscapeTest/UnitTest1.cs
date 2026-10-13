using Microsoft.VisualStudio.TestTools.UnitTesting;
using Escape.Model;

namespace EscapeTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            EscapeModel model = new EscapeModel();
            Assert.IsNotNull(model._saveAndLoad); // Persistence loaded.
            model.loadGame(11);
            Assert.AreEqual(model._player.X, 0);
            Assert.AreEqual(model._player.Y, 5);  // Player position right.

            Assert.AreEqual(model._enemies[0].X, 0);
            Assert.AreEqual(model._enemies[0].Y, 10);
            model.stepUnits();
            Assert.AreEqual(model._enemies[0].X, 0);
            Assert.AreEqual(model._enemies[0].Y, 9);  // Enemy stepped in the right direction
        }
    }
}
