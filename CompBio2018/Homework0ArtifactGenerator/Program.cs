using System;
using System.IO;
using System.Text;
using ComputationalBiology.AlignmentScoreProvider.Blosum62;
using ComputationalBiology.EmpiricalAlignmentSignificance;
using ComputationalBiology.FastA;
using ComputationalBiology.SequenceAlignment;
using ComputationalBiology.SequenceAlignment.SmithWaterman;

namespace ComputationalBiology.TestArtifactGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputLocation = @"c:\temp\output_jeeshn.txt";
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

            using (var fileStream = File.Open(outputLocation, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream))
            {
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
                string pValue = pvalueCalculator.CalculatePValueAsync().Result;
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
                            item1 = FastALookupCient.LookupByAccessionIdAsync(proteinAccessionIds[i]).Result;

                            item2 = FastALookupCient.LookupByAccessionIdAsync(proteinAccessionIds[j]).Result;

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

                            WriteToConsoleAndFile("Alignment", streamWriter);
                            WriteToConsoleAndFile(result.PrettyPrint(), streamWriter);

                            scoreMatrix[i, j] = result.AlignmentScore;

                            if (item1.AccessionId == "P15172" &&
                                (item2.AccessionId == "Q10574" || item2.AccessionId == "O95363"))
                            {
                                pvalueCalculator = new PValueCalculator<SmithWatermanImplementation>
                                 (
                                     alignmentImpl: localAlignmentImpl,
                                     permutationLimit: 999
                                 );

                                //Print p values
                                pValue = pvalueCalculator.CalculatePValueAsync().Result;
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
            Console.ReadLine();
        }

        static void WriteToConsoleAndFile(string toWrite, StreamWriter streamWriter)
        {
            Console.WriteLine(toWrite);
            streamWriter.WriteLine(toWrite);
        }
    }
}
