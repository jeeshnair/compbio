using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ComputationalBiology.AlignmentScoreProvider.Blosum62
{
    /// <summary>
    /// A class encapsulating BLOSUM 62 score matrix.
    /// </summary>
    public class Blosum62ScoreProvider : AlignmentScoreProviderBase
    {
        const string blosum62FileName = @"Resources\BLOSUM62.txt";
        static Dictionary<string,int> blosum62LookupTable = new Dictionary<string, int>();

        static Blosum62ScoreProvider()
        {
            InitializeLookupDictionaryFromFile();
        }

        public override int LookupPairwiseAlignmentScore(char source, char target)
        {
            if (Char.IsWhiteSpace(source)) { throw new ArgumentNullException("source"); }
            if (Char.IsWhiteSpace(target)) { throw new ArgumentNullException("target"); }

            int returnValue;
            if (!blosum62LookupTable.TryGetValue(String.Format(
                                "{0}-{1}",
                                source.ToString().ToUpperInvariant(),
                                target.ToString().ToUpperInvariant()), out returnValue))
            {
                throw new InvalidOperationException("No matching scores found for supplied inputs.");
            }

            return returnValue;
        }

        static void InitializeLookupDictionaryFromFile()
        {
            using (StreamReader fileStream = File.OpenText(blosum62FileName))
            {
                MatchCollection headers = Regex.Matches(
                    fileStream.ReadLine(), @"[A-Za-z*]");

                while (!fileStream.EndOfStream)
                {
                    string line = fileStream.ReadLine();
                    MatchCollection pairwiseScores = Regex.Matches(line, @"\-*\d+");

                    for (int i = 0; i < pairwiseScores.Count; i++)
                    {
                        blosum62LookupTable[
                            String.Format(
                                "{0}-{1}",
                                line[0],
                                headers[i])] =
                            Convert.ToInt32(pairwiseScores[i].Value);
                    }
                }
            }
        }
    }
}
