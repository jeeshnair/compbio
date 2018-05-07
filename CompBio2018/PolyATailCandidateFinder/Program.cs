using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PolyATailCandidateFinder
{
    class Program
    {
        /// <summary>
        /// Unfiltered file with all potential candidates
        /// </summary>
        const string samFilePath = @"C:\AssignmentNotes\Homework5_Jeesh\hw5-candidates-alpha.txt";

        /// <summary>
        /// Filtered file with poly a candidates identified
        /// </summary>
        const string samFileFilteredPath = @"C:\AssignmentNotes\Homework5_Jeesh\hw5-candidates-alpha_filtered.txt";

        /// <summary>
        /// File capturing lines which could not be parsed
        /// </summary>
        const string samFileUnprocessedPath = @"C:\AssignmentNotes\Homework5_Jeesh\hw5-candidates-alpha_unprocessed.txt";

        /// <summary>
        /// File capturing the strings left of cleavage site.
        /// </summary>
        const string polyACleavageUpstream = @"C:\AssignmentNotes\Homework5_Jeesh\hw5-candidates-alpha_cleavagesite.txt";

        static void Main(string[] args)
        {
            using (StreamReader sr = File.OpenText(samFilePath))
            using (StreamWriter sw = File.CreateText(samFileFilteredPath))
            using (StreamWriter sw1 = File.CreateText(samFileUnprocessedPath))
            using (StreamWriter sw2 = File.CreateText(polyACleavageUpstream))
            {
                string s = String.Empty;
                int lineCount = 0;

                while ((s = sr.ReadLine()) != null)
                {
                    lineCount++;

                    try
                    {
                        Console.WriteLine("Processing New line {0}", lineCount);
                        string[] columns = s.Split('\t');

                        string id = columns[0];
                        int nmIndex = 16;
                        int alignmentScore = Convert.ToInt32(columns[11].Split(':')[2]);
                        int notMatchedCount = Convert.ToInt32(columns[nmIndex].Split(':')[2]);

                        // Parse the CIGAR string
                        string finalCigarPart =
                            Regex.Split(columns[5], "([0-9]+[MIDNSHPX=])").Where(p => !String.IsNullOrWhiteSpace(p)).Last();

                        // Look for short clipped sequences
                        int shortClippedSequence = 0;
                        if (finalCigarPart.EndsWith("S"))
                        {
                            shortClippedSequence = Convert.ToInt32(finalCigarPart.Substring(0, finalCigarPart.Length - 1));
                        }

                        // Filter further by alignment score short clipped sequence length and number of As
                        if (alignmentScore <= -3 && shortClippedSequence >= 4 && columns[9].EndsWith("AAAAA"))
                        {
                            sw.WriteLine(s);
                            sw2.WriteLine("{0}\t{1}", id, FindCleavageSite(columns[9]));
                        }
                    }
                    catch (Exception ex)
                    {
                        // If we fail a step let us capture the failed line in a file.
                        sw1.WriteLine("Exception {0}: Line {1}", ex.Message, s);
                    }
                }

                sw.Flush();
                sw1.Flush();
                sw2.Flush();
            }
        }

        /// <summary>
        /// Finds the cleavage site given sequence.
        /// </summary>
        /// <remarks>
        /// This is closely coupled with implementation of candidate finder logic above.
        /// </remarks>
        static string FindCleavageSite(string fullSequence)
        {
            int countofAs = 0;
            for (int i = fullSequence.Length - 1; i >= 0; i--)
            {
                if (fullSequence[i] != 'A' && countofAs >= 6)
                {
                    return fullSequence.Substring(0, i + 1);
                }
                else if (fullSequence[i] == 'A' )
                {
                    countofAs++;
                }
            }

            return fullSequence;
        }
    }
}
