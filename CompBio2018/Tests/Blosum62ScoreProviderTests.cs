using System;
using ComputationalBiology.AlignmentScoreProvider.Blosum62;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComputationalBiology.Tests
{
    [TestClass]
    public class Blosum62ScoreProviderTests
    {
        Blosum62ScoreProvider blosum62ScoreProvider = new Blosum62ScoreProvider();
        [TestMethod]
        public void LookupScoreMatch()
        {
            int actualScore = blosum62ScoreProvider.LookupPairwiseAlignmentScore(
                source: 'I', target: 'G');
            Assert.AreEqual(expected: -4, actual: actualScore, message: "Scores do not match");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LookupScoreInvalidInputs()
        {
            int actualScore = blosum62ScoreProvider.LookupPairwiseAlignmentScore(
                source: 'I', target: '9');
        }
    }
}
