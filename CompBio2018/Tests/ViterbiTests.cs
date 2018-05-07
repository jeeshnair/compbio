using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ComputationalBiology.FastA;
using HiddenMarkovModel;
using HiddenMarkovModel.DiceRoll;
using HiddenMarkovModel.GCPatches;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blosum62ScoreProviderTests
{
    [TestClass]
    public class ViterbiTests
    {
        [TestMethod]
        public async Task ViterbiDiceRollTenRuns()
        {
            var diceRollParams = new DiceRollParameters();
            SequenceMetadata item1 = await
                FastALookupCient.LookupByAccessionIdAsync("DiceRoll");
            var viterbiDiceRoll = new ViterbiImpl(diceRollParams, input: item1.Sequence);
            List<ViterbiResult> results = viterbiDiceRoll.ExecuteViterbiAndTrain(executionCount: 10);

            for (int i = 0; i < 9; i++)
            {
                Console.WriteLine("Iteration {0}", i + 1);
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine(results[i].PrettyPrint(interestedStateIndex: 1, numberOfHits: 5));
                Console.WriteLine(results[i].StateTransitionRepresentaton);
            }

            Console.WriteLine("Iteration {0}", 10);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine(results[9].PrettyPrintAllHits(interestedStateIndex: 1));
            Console.WriteLine(results[9].StateTransitionRepresentaton);

            Assert.IsNotNull(results);
        }

        [TestMethod]
        public async Task ViterbiGCPatchTenRuns()
        {
            var gcPatchParameters = new GCPatchParameters();
            SequenceMetadata item1 = await
                FastALookupCient.LookupByAccessionIdAsync("GCF_000091665.1_ASM9166v1_genomic");
            var viterbigcPatch = new ViterbiImpl(gcPatchParameters, input: item1.Sequence);
            List<ViterbiResult> results = viterbigcPatch.ExecuteViterbiAndTrain(executionCount: 10);

            for (int i = 0; i < 9; i++)
            {
                Console.WriteLine("Iteration {0}", i+1);
                Console.WriteLine("---------------------------------------------------------------------------");
                Console.WriteLine(results[i].PrettyPrint(interestedStateIndex: 1, numberOfHits: 5));
            }

            Console.WriteLine("Iteration {0}", 10);
            Console.WriteLine("---------------------------------------------------------------------------");
            Console.WriteLine(results[9].PrettyPrintAllHits(interestedStateIndex: 1));

            Assert.IsNotNull(results);
        }
    }
}
