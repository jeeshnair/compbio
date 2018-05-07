using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ComputationalBiology.AlignmentScoreProvider;
using ComputationalBiology.AlignmentScoreProvider.Blosum62;
using ComputationalBiology.EmpiricalAlignmentSignificance;
using ComputationalBiology.FastA;
using ComputationalBiology.SequenceAlignment;
using ComputationalBiology.SequenceAlignment.SmithWaterman;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComputationalBiology.Tests
{
    [TestClass]
    public class SmithWatermanTests
    {
        [TestMethod]
        public async Task SmithWatermanImplementationMainlineWithSimpleScoringScheme()
        {
            SequenceMetadata item1 = await
                FastALookupCient.LookupByAccessionIdAsync("Q10574");

            SequenceMetadata item2 = await
                FastALookupCient.LookupByAccessionIdAsync("P15172");

            var localAlignmentImpl = new SmithWatermanImplementation(
                sequenceTomatch: item1,
                targetSequence: item2,
                scoreProvider: new SimpleScoreProvider(),
                gapOpenPenality: 1);

            AlignmentImplementationResults result
                = localAlignmentImpl.FindOptimalAlignment();

            Console.WriteLine("--Optimal Alignment--");
            Console.WriteLine(result.TargetSequenceAlignment);
            Console.WriteLine(result.SearchSequenceAlignment);

            Console.WriteLine("--Optimal Score--");
            Console.WriteLine(result.AlignmentScore);

            Assert.AreEqual(expected: 19,
                actual: result.AlignmentScore,
                message: "Mismatching alignment scores");

            Assert.AreEqual(
                expected: "VE-IL-RNA-IRY-I-E-GL-QA-LL-RDQD",
                actual: result.TargetSequenceAlignment,
                message: "Mismatching target alignment sequence");

            Assert.AreEqual(
                expected: "-FE-TL-QMA-QKY-I-E-CL-SQ-IL-KQD",
                actual: result.SearchSequenceAlignment,
                message: "Mismatching target alignment sequence");
        }

        /// <summary>
        /// Test does not do any fancy asserts
        /// </summary>
        [TestMethod]
        public async Task SmithWatermanP15172ToP17542WithBlosum62ScoringScheme()
        {
            SequenceMetadata item1 = await
                FastALookupCient.LookupByAccessionIdAsync("P10085");

             SequenceMetadata item2 = await
                FastALookupCient.LookupByAccessionIdAsync("P15172");

            var localAlignmentImpl = new SmithWatermanImplementation(
              sequenceTomatch: item1,
              targetSequence: item2,
              scoreProvider: new Blosum62ScoreProvider(),
              gapOpenPenality: 4);

            //var localAlignmentImpl = new SmithWatermanImplementation(
            //    sequenceTomatch: "KEVLAR",
            //    targetSequence: "KNIEVIL",
            //    scoreProvider: new Blosum62ScoreProvider(),
            //    gapOpenPenality: 4);

            AlignmentImplementationResults result
              = localAlignmentImpl.FindOptimalAlignment();

            Console.WriteLine("--Optimal Alignment--");
            Console.WriteLine(result.TargetSequenceAlignment);
            Console.WriteLine(result.SearchSequenceAlignment);

            Console.WriteLine("--Optimal Score--");
            Console.WriteLine(result.AlignmentScore);

            var pvalueCalculator = new PValueCalculator<SmithWatermanImplementation>
                (
                    alignmentImpl: localAlignmentImpl,
                    permutationLimit: 1
                );

            string pValue = await pvalueCalculator.CalculatePValueAsync();
            Assert.IsNotNull(result);

            Console.WriteLine("--Calculated P value--");
            Console.WriteLine(pValue);

            Console.WriteLine(result.PrettyPrint());
            Console.WriteLine(result.PrettyPrintScoreMatrix());
        }

        [TestMethod]
        public async Task Homework2()
        {
            var proteinAccessionIds = new string[]
            {
                "P15172",
                "P17542",
                "P10085",
                "P16075",
                "P13904",
                "Q90477",
                "Q8IU24",
                "P22816",
                "Q10574",
                "O95363"
            };

            using (var fileStream = File.Open("outputs.txt", FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                // Input data for a simple sequence
                var item1 = new SequenceMetadata
                {
                    AccessionId = "X1",
                    Sequence = "deadly"
                };

                var item2 = new SequenceMetadata
                {
                    AccessionId = "X2",
                    Sequence = "ddgearlyk"
                };

                // Run the local alignment .
                var localAlignmentImpl = new SmithWatermanImplementation(
                    sequenceTomatch: item1,
                    targetSequence: item2,
                    scoreProvider: new Blosum62ScoreProvider(),
                    gapOpenPenality: 4);

                AlignmentImplementationResults result
                    = localAlignmentImpl.FindOptimalAlignment();

                // "Capture" output
                WriteToConsoleAndFile(
                    String.Format("{0} vs {1}", item1.AccessionId, item2.AccessionId), streamWriter);
                WriteToConsoleAndFile("Alignment Score", streamWriter);
                WriteToConsoleAndFile(result.AlignmentScore.ToString(), streamWriter);

                WriteToConsoleAndFile("Alignment", streamWriter);
                WriteToConsoleAndFile(result.PrettyPrint(), streamWriter);

                WriteToConsoleAndFile("Score Matrix", streamWriter);
                WriteToConsoleAndFile(result.PrettyPrintScoreMatrix(), streamWriter);

                // P-value calculation
                var pvalueCalculator = new PValueCalculator<SmithWatermanImplementation>
                (
                    alignmentImpl: localAlignmentImpl,
                    permutationLimit: 999
                );

                //Print p values
                string pValue = await pvalueCalculator.CalculatePValueAsync();
                WriteToConsoleAndFile("Empirical p-value", streamWriter);
                WriteToConsoleAndFile(pValue, streamWriter);
                WriteToConsoleAndFile(String.Empty, streamWriter);

                int[,] scoreMatrix = new int[10, 10];

                for (int i = 0; i < proteinAccessionIds.Length; i++)
                {
                    for (int j = 0; j < proteinAccessionIds.Length; j++)
                    {
                        // no need to compare same sequences
                        if (i != j)
                        {
                            item1 = await
                                FastALookupCient.LookupByAccessionIdAsync(proteinAccessionIds[i]);

                            item2 = await
                                FastALookupCient.LookupByAccessionIdAsync(proteinAccessionIds[j]);

                            localAlignmentImpl = new SmithWatermanImplementation(
                                sequenceTomatch: item1,
                                targetSequence: item2,
                                scoreProvider: new Blosum62ScoreProvider(),
                                gapOpenPenality: 4);

                            result = localAlignmentImpl.FindOptimalAlignment();

                            // "Capture" output
                            WriteToConsoleAndFile(
                                String.Format("{0} vs {1}", item1.AccessionId, item2.AccessionId), streamWriter);
                            WriteToConsoleAndFile("Alignment Score", streamWriter);
                            WriteToConsoleAndFile(result.AlignmentScore.ToString(), streamWriter);

                            scoreMatrix[i, j] = result.AlignmentScore;

                            WriteToConsoleAndFile("Alignment", streamWriter);
                            WriteToConsoleAndFile(result.PrettyPrint(), streamWriter);

                            if (item1.AccessionId == "P15172" &&
                                (item2.AccessionId == "Q10574" || item2.AccessionId == "O95363"))
                            {
                                pvalueCalculator = new PValueCalculator<SmithWatermanImplementation>
                                 (
                                     alignmentImpl: localAlignmentImpl,
                                     permutationLimit: 999
                                 );

                                //Print p values
                                pValue = await pvalueCalculator.CalculatePValueAsync();
                                WriteToConsoleAndFile("Empirical p-value", streamWriter);
                                WriteToConsoleAndFile(pValue, streamWriter);
                                WriteToConsoleAndFile(String.Empty, streamWriter);
                            }
                        }
                    }
                }

                var stringBuilder = new StringBuilder();
                for (int i = 0; i <= scoreMatrix.GetUpperBound(0); i++)
                {
                    for (int j = 0; j <= scoreMatrix.GetUpperBound(1); j++)
                    {
                        if (j >= i)
                        {
                            stringBuilder.Append(scoreMatrix[i, j].ToString().PadRight(6, ' '));
                        }
                        else
                        {
                            stringBuilder.Append("0".PadRight(6, ' '));
                        }
                    }
                    stringBuilder.AppendLine();
                }

                WriteToConsoleAndFile("Protein scoring matrix", streamWriter);
                WriteToConsoleAndFile(stringBuilder.ToString(), streamWriter);

                streamWriter.Flush();
                fileStream.Flush();
            }
        }

        static void WriteToConsoleAndFile(string toWrite, StreamWriter streamWriter)
        {
            Console.WriteLine(toWrite);
            streamWriter.WriteLine(toWrite);
        }
    }
}
