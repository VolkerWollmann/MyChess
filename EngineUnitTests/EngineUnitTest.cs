using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyChessEngine;

namespace EngineUnitTests
{
    [TestClass]
    public class EngineClassTests
    {
        [TestMethod]
        public void CreateEngine()
        {
            ChessEngine chessEngine = new ChessEngine();
            Assert.IsNotNull(chessEngine);
        }
    }
}
