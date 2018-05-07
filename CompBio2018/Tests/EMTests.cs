using System;
using ExpectationMaximization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blosum62ScoreProviderTests
{
    [TestClass]
    public class EMTests
    {
        [TestMethod]
        public void ExpectationMaximizationMainline()
        {
            var inputs = new double[] { 9, 10, 11, 20, 21, 22, 46, 49, 55, 57 };
            for (int i = 1; i < 6; i++)
            {
                var emMaximization = new ExpectationMaximizationImpl
                    (inputs, mixtureCount: i);

                ExpectationMaximizationResults results = emMaximization.FindConvergedMeans();

                Console.WriteLine("---------Run {0} Results------", i);
                Console.WriteLine(results.PrettyPrint());

                Assert.IsNotNull(results);
            }
        }
    }
}
