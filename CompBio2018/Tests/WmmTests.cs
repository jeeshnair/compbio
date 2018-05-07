using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeightedMatrixModel;

namespace Blosum62ScoreProviderTests
{
    [TestClass]
    public class WmmTests
    {
        [TestMethod]
        public void WmmFullConsensusTest()
        {
            var wmmParameters = new FullConsensusWmm();
            var wmmImpl = new WmmSequenceScanImpl(
                wmmParameters, 
                sequence: "AAATAAACATGCTAGCTTTTATTCCAGTTCTAACCAAAAAAATAAACCCTCGTTCCACAGAAGCTGCCATCAAGTATTTCCTCACGC", 
                scanLength: 6);
            WmmSequenceResult result = wmmImpl.FindScoringSequences();

            Assert.IsNotNull(result);
            List<WmmSequenceResultItem> positiveScore = result.FoundSequences.Where(s => s.LogLikelihoodScore > 0).ToList();
        }

        [TestMethod]
        public void WmmPartialConsensusTest()
        {
            var wmmParameters = new PartialConsensus();
            var wmmImpl = new WmmSequenceScanImpl(
                wmmParameters,
                sequence: "AAATAAACATGCTAGCTTTTATTCCAGTTCTAACCAAAAAAATAAACCCTCGTTCCACAGAAGCTGCCATCAAGTATTTCCTCACGC",
                scanLength: 6);
            WmmSequenceResult result = wmmImpl.FindScoringSequences();

            Assert.IsNotNull(result);
            List<WmmSequenceResultItem> positiveScore = result.FoundSequences.Where(s => s.LogLikelihoodScore > 0).ToList();
        }

        [TestMethod]
        public void WmmAlpha()
        {
            var wmmPartialParameters = new PartialConsensus();
            var wmmFullParameters = new FullConsensusWmm();

            string[] sequences = new[]{
                "CTTCGAAGCGAAAAGTCCTAATAGTAGAAGAACCCTCCATAAACCTGGAGTGACTATATGGATGCCCCCCACCCTACCACACATTCGAAGAAC",
                "CTCAAAAAAAAAAAAAAAAGATAATGGCTTCTTGAAAAAACAAAGAAATCAACCTGAAGGAATTCCTGATGGCCAAAGCTAGAACAATCTGAG",
                "CGGTTTAAGAATACATCCTTGTATAATCTGACATACAAATTTGTCATTTCCTGCACATGCACACCATTGTTAAAAAAAAAAAAAAAAAGCCAG"
                };

            var wmmFullConsensesResults = new List<WmmSequenceResult>();
            foreach (string sequence in sequences)
            {
                var wmmScan = new WmmSequenceScanImpl(
                    wmmFullParameters, sequence, scanLength: 6);
                wmmFullConsensesResults.Add(wmmScan.FindScoringSequences());
            }

            Console.WriteLine("Enumerating Hits with LLR >0 for Full consensus");
            var wmmFullConsensesResultItemFlattened = new List<WmmSequenceResultItem>();
            var wmmFullConsensesResultItemFlattenedBestScoring = new List<WmmSequenceResultItem>();

            foreach (WmmSequenceResult fullConsensusResult in wmmFullConsensesResults)
            {
                WmmSequenceResultItem bestScoring = null;

                foreach (WmmSequenceResultItem item in fullConsensusResult.FoundSequences)
                {
                    if (item.LogLikelihoodScore > 0)
                    {
                        wmmFullConsensesResultItemFlattened.Add(item);
                        if (bestScoring == null || bestScoring.LogLikelihoodScore <= item.LogLikelihoodScore)
                        {
                            bestScoring = item;
                        }
                    }
                }
                if (bestScoring != null)
                {
                    wmmFullConsensesResultItemFlattenedBestScoring.Add(bestScoring);
                }
            }

            Console.WriteLine("Total Count of hits: {0}", wmmFullConsensesResultItemFlattened.Count);
            Console.WriteLine(
                "Count of Sequences with hits: {0}", wmmFullConsensesResultItemFlattenedBestScoring.Count);
            if (wmmFullConsensesResultItemFlattened.Count > 0)
            {
                Console.WriteLine(
                    "Average Distance to putative cleavage site {0}", 
                    wmmFullConsensesResultItemFlattenedBestScoring.Average(b => b.DistanceFromCleavageSite));
            }
            Console.WriteLine("Wmm0");
            Console.WriteLine(wmmFullParameters.Print());
            Console.WriteLine("Relative Entropy :{0}", wmmFullParameters.GetRelativeEntropy());

            var wmmPartialConsensesResults = new List<WmmSequenceResult>();
            foreach (string sequence in sequences)
            {
                var wmmScan = new WmmSequenceScanImpl(
                    wmmPartialParameters, sequence, scanLength: 6);
                wmmPartialConsensesResults.Add(wmmScan.FindScoringSequences());
            }
            Console.WriteLine("Enumerating Hits with LLR >0 for Partial consensus");
            var wmmPartialConsensesResultItemFlattened = new List<WmmSequenceResultItem>();
            var wmmPartialConsensesResultItemFlattenedBestScoring = new List<WmmSequenceResultItem>();

            foreach (WmmSequenceResult partialConsensusResult in wmmPartialConsensesResults)
            {
                WmmSequenceResultItem bestScoring = null;

                foreach (WmmSequenceResultItem item in partialConsensusResult.FoundSequences)
                {
                    if (item.LogLikelihoodScore > 0)
                    {
                        wmmPartialConsensesResultItemFlattened.Add(item);
                        if (bestScoring == null || bestScoring.LogLikelihoodScore <= item.LogLikelihoodScore)
                        {
                            bestScoring = item;
                        }
                    }
                }
                if (bestScoring != null)
                {
                    wmmPartialConsensesResultItemFlattenedBestScoring.Add(bestScoring);
                }
            }
            Console.WriteLine("Total Count : {0}", wmmPartialConsensesResultItemFlattened.Count);
            Console.WriteLine(
                "Count of Sequences with hits: {0}", wmmPartialConsensesResultItemFlattenedBestScoring.Count);
            if (wmmPartialConsensesResultItemFlattened.Count > 0)
            {
                Console.WriteLine(
                    "Average Distance to putative cleavage site {0}", 
                    wmmPartialConsensesResultItemFlattenedBestScoring.Average(b => b.DistanceFromCleavageSite));
            }
            Console.WriteLine("Wmm1");
            Console.WriteLine(wmmPartialParameters.Print());
            Console.WriteLine("Relative Entropy :{0}", wmmPartialParameters.GetRelativeEntropy());

            var wmmImpl = new MemeImpl(
                wmmPartialParameters,
                sequences: sequences,
                scanLength: 6);
            Dictionary<int, WmmSequenceResult> result = wmmImpl.FindScoringSequences(iterationCount: 1);


            Console.WriteLine("Enumerating Hits with LLR >0 for Partial consensus");
            var wmmPartialConsensesResultItemIteratedFlattened = new List<WmmSequenceResultItem>();
            var wmmPartialConsensesResultItemIteratedFlattenedBestScoring = new List<WmmSequenceResultItem>();
            foreach (var partialConsensusResult in result)
            {
                WmmSequenceResultItem bestScoring = null;

                foreach (WmmSequenceResultItem item in partialConsensusResult.Value.FoundSequences)
                {
                    if (item.LogLikelihoodScore > 0)
                    {
                        wmmPartialConsensesResultItemIteratedFlattened.Add(item);
                        if (bestScoring == null || bestScoring.LogLikelihoodScore <= item.LogLikelihoodScore)
                        {
                            bestScoring = item;
                        }
                    }
                }
                if (bestScoring != null)
                {
                    wmmPartialConsensesResultItemIteratedFlattenedBestScoring.Add(bestScoring);
                }
            }
            Console.WriteLine("Count : {0}", wmmPartialConsensesResultItemIteratedFlattened.Count);
            Console.WriteLine(
                "Count of Sequences with hits: {0}", wmmPartialConsensesResultItemIteratedFlattenedBestScoring.Count);
            if (wmmPartialConsensesResultItemIteratedFlattened.Count > 0)
            {
                Console.WriteLine(
                    "Average Distance to putative cleavage site {0}", 
                    wmmPartialConsensesResultItemIteratedFlattenedBestScoring.Average(b => b.DistanceFromCleavageSite));
            }
            Console.WriteLine("Wmm1");
            Console.WriteLine(result[0].ParametersUsed.Print());
            Console.WriteLine("Relative Entropy :{0}", result[0].ParametersUsed.GetRelativeEntropy());

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WmmHumaGenome()
        {
            var wmmPartialParameters = new PartialConsensus();
            var wmmFullParameters = new FullConsensusWmm();

            var sequences = new List<string>();
            using (StreamReader sr = File.OpenText(@"C:\temp\filtered_cleavagesite.txt"))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    sequences.Add(s.Split('\t')[1]);
                }
            };

            var wmmFullConsensesResults = new List<WmmSequenceResult>();
            foreach (string sequence in sequences)
            {
                var wmmScan = new WmmSequenceScanImpl(
                    wmmFullParameters, sequence, scanLength: 6);
                wmmFullConsensesResults.Add(wmmScan.FindScoringSequences());
            }

            Console.WriteLine("Enumerating Hits with LLR >0 for Full consensus");
            var wmmFullConsensesResultItemFlattened = new List<WmmSequenceResultItem>();
            var wmmFullConsensesResultItemFlattenedBestScoring = new List<WmmSequenceResultItem>();

            foreach (WmmSequenceResult fullConsensusResult in wmmFullConsensesResults)
            {
                WmmSequenceResultItem bestScoring = null;

                foreach (WmmSequenceResultItem item in fullConsensusResult.FoundSequences)
                {
                    if (item.LogLikelihoodScore > 0)
                    {
                        wmmFullConsensesResultItemFlattened.Add(item);
                        if (bestScoring == null || bestScoring.LogLikelihoodScore <= item.LogLikelihoodScore)
                        {
                            bestScoring = item;
                        }
                    }
                }
                if (bestScoring != null)
                {
                    wmmFullConsensesResultItemFlattenedBestScoring.Add(bestScoring);
                }
            }

            Console.WriteLine("Total Count of hits: {0}", wmmFullConsensesResultItemFlattened.Count);
            Console.WriteLine(
                "Count of Sequences with hits: {0}", wmmFullConsensesResultItemFlattenedBestScoring.Count);
            if (wmmFullConsensesResultItemFlattened.Count > 0)
            {
                Console.WriteLine(
                    "Average Distance to putative cleavage site {0}",
                    wmmFullConsensesResultItemFlattenedBestScoring.Average(b => b.DistanceFromCleavageSite));
            }
            Console.WriteLine("Wmm0");
            Console.WriteLine(wmmFullParameters.Print());
            Console.WriteLine("Relative Entropy :{0}", wmmFullParameters.GetRelativeEntropy());

            var wmmPartialConsensesResults = new List<WmmSequenceResult>();
            foreach (string sequence in sequences)
            {
                var wmmScan = new WmmSequenceScanImpl(
                    wmmPartialParameters, sequence, scanLength: 6);
                wmmPartialConsensesResults.Add(wmmScan.FindScoringSequences());
            }
            Console.WriteLine("Enumerating Hits with LLR >0 for Partial consensus");
            var wmmPartialConsensesResultItemFlattened = new List<WmmSequenceResultItem>();
            var wmmPartialConsensesResultItemFlattenedBestScoring = new List<WmmSequenceResultItem>();

            foreach (WmmSequenceResult partialConsensusResult in wmmPartialConsensesResults)
            {
                WmmSequenceResultItem bestScoring = null;

                foreach (WmmSequenceResultItem item in partialConsensusResult.FoundSequences)
                {
                    if (item.LogLikelihoodScore > 0)
                    {
                        wmmPartialConsensesResultItemFlattened.Add(item);
                        if (bestScoring == null || bestScoring.LogLikelihoodScore <= item.LogLikelihoodScore)
                        {
                            bestScoring = item;
                        }
                    }
                }
                if (bestScoring != null)
                {
                    wmmPartialConsensesResultItemFlattenedBestScoring.Add(bestScoring);
                }
            }
            Console.WriteLine("Total Count : {0}", wmmPartialConsensesResultItemFlattened.Count);
            Console.WriteLine(
                "Count of Sequences with hits: {0}", wmmPartialConsensesResultItemFlattenedBestScoring.Count);
            if (wmmPartialConsensesResultItemFlattened.Count > 0)
            {
                Console.WriteLine(
                    "Average Distance to putative cleavage site {0}",
                    wmmPartialConsensesResultItemFlattenedBestScoring.Average(b => b.DistanceFromCleavageSite));
            }
            Console.WriteLine("Wmm1");
            Console.WriteLine(wmmPartialParameters.Print());
            Console.WriteLine("Relative Entropy :{0}", wmmPartialParameters.GetRelativeEntropy());

            var wmmImpl = new MemeImpl(
                wmmPartialParameters,
                sequences: sequences.ToArray(),
                scanLength: 6);
            Dictionary<int, WmmSequenceResult> result = wmmImpl.FindScoringSequences(iterationCount: 1);


            Console.WriteLine("Enumerating Hits with LLR >0 for Partial consensus");
            var wmmPartialConsensesResultItemIteratedFlattened = new List<WmmSequenceResultItem>();
            var wmmPartialConsensesResultItemIteratedFlattenedBestScoring = new List<WmmSequenceResultItem>();
            foreach (var partialConsensusResult in result)
            {
                WmmSequenceResultItem bestScoring = null;

                foreach (WmmSequenceResultItem item in partialConsensusResult.Value.FoundSequences)
                {
                    if (item.LogLikelihoodScore > 0)
                    {
                        wmmPartialConsensesResultItemIteratedFlattened.Add(item);
                        if (bestScoring == null || bestScoring.LogLikelihoodScore <= item.LogLikelihoodScore)
                        {
                            bestScoring = item;
                        }
                    }
                }
                if (bestScoring != null)
                {
                    wmmPartialConsensesResultItemIteratedFlattenedBestScoring.Add(bestScoring);
                }
            }
            Console.WriteLine("Count : {0}", wmmPartialConsensesResultItemIteratedFlattened.Count);
            Console.WriteLine(
                "Count of Sequences with hits: {0}", wmmPartialConsensesResultItemIteratedFlattenedBestScoring.Count);
            if (wmmPartialConsensesResultItemIteratedFlattened.Count > 0)
            {
                Console.WriteLine(
                    "Average Distance to putative cleavage site {0}",
                    wmmPartialConsensesResultItemIteratedFlattenedBestScoring.Average(b => b.DistanceFromCleavageSite));
            }
            Console.WriteLine("Wmm1");
            Console.WriteLine(result[0].ParametersUsed.Print());
            Console.WriteLine("Relative Entropy :{0}", result[0].ParametersUsed.GetRelativeEntropy());

            Assert.IsNotNull(result);
        }
    }
}
