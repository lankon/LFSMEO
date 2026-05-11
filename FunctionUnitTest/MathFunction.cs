using RGBTester.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RGBTester.Logic.Function;

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

        [TestMethod]
        public void Calculate_WLD_Test()
        {
            Wavelength WL = new Wavelength();
            double res = 0;
            double[] Wavelength = new double[4096];
            double[] Intensity = new double[4096];

            for (int i = 0; i < Wavelength.Length; i++)
            {
                Wavelength[i] = Wavelength.Length;
                Intensity[i] = 0;
            }

            string path = @"D:\Virtual_Spectrum_Data.csv";

            if (!File.Exists(path))
            {
                Assert.AreEqual(5.0, res, 1e-6);
                return;
            }

            string[] lines = File.ReadAllLines(path);
            int index = 0;

            IEnumerable<string> dataLines = lines.Skip(1);
            foreach (string line in dataLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] values = line.Split(',');

                if (values.Length < 2) continue; //確保有波長以及強度值

                if (float.TryParse(values[0], out float nm_value))
                    Wavelength[index] = nm_value;

                if (float.TryParse(values[1], out float intensity_value))
                    Intensity[index] = intensity_value;

                index++;
                if (index >= Wavelength.Length)
                    break;
            }

            for (int i = index; i < Wavelength.Length; i++)
            {
                Wavelength[i] = Wavelength[index - 1] + 1;
                Intensity[i] = 0;
            }

            //Act
            res = WL.Calculate_WLD(Wavelength, Intensity);
            //Assert
            Assert.AreEqual(565, res, 1e-6);
        }
    }

}
