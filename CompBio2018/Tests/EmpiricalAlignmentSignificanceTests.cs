using System;
using System.Threading.Tasks;
using ComputationalBiology.AlignmentScoreProvider;
using ComputationalBiology.EmpiricalAlignmentSignificance;
using ComputationalBiology.FastA;
using ComputationalBiology.SequenceAlignment.SmithWaterman;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComputationalBiology.Tests
{
    [TestClass]
    public class EmpiricalAlignmentSignificanceTests
    {
        [TestMethod]
        public async Task PValueMainline()
        {
            SequenceMetadata item1 = await
                FastALookupCient.LookupByAccessionIdAsync("Q10574");

            SequenceMetadata item2 = await
                FastALookupCient.LookupByAccessionIdAsync("P15172");

            var localAlignmentImpl = new SmithWatermanImplementation(
               sequenceTomatch: item1,
               targetSequence:  item2,
               scoreProvider: new SimpleScoreProvider(),
               gapOpenPenality: 1);

            var pvalueCalculator = new PValueCalculator<SmithWatermanImplementation>
                (
                    alignmentImpl: localAlignmentImpl,
                    permutationLimit: 10
                );

            string result = await pvalueCalculator.CalculatePValueAsync();
            Assert.IsNotNull(result);

            Console.WriteLine("--Calculated P value--");
            Console.WriteLine(result);
        }
    }
}
