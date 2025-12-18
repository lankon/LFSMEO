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

        [TestMethod]
        public void DataFilter_GetPreciseHighLevel_Test()
        {
            // Arrange
            
            List<double> testData = new List<double>
            {
                1.0, 1.1, 1.2, 1.3, 1.4, // 爬坡雜訊
                4.8, 4.9, 5.0, 5.1, 5.0, 5.1, 5.0, 5.2, 5.1, 5.0, // 穩定高電平
                1.5, 1.6, 1.7 // 雜訊
            };

            // Act
            DataFilter dataFilter = new DataFilter();
            double preciseHighLevel = dataFilter.GetPreciseHighLevel(testData, 0.7);
            // Assert
            Assert.AreEqual(5.0, preciseHighLevel, 1e-6);
        }
    }

}
