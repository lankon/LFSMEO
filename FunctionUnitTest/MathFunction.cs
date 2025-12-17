using RGBTester.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FunctionUnitTest
{
    [TestClass]
    public sealed class MathFunction
    {
        [TestMethod]
        public void LinearCurveFitting_Test()
        {
            // Arrange
            int[] dac = { 1, 2, 3, 4 };
            double[] current = { 3, 5, 7, 9 }; // y = 2x + 1

            // Act
            var fitting = new LinearCurveFitting(dac, current);

            // Assert
            Assert.AreEqual(2, fitting.Slope, 1e-12);
            Assert.AreEqual(1, fitting.Offset, 1e-12);
        }
    }

}
