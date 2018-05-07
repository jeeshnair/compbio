using System;
using System.Collections.Generic;
using System.Text;
using ComputationalBiology.AlignmentScoreProvider;

namespace ComputationalBiology.SequenceAlignment
{
    /// <summary>
    /// Captures result of Smith waterman algorithm
    /// </summary>
    public class AlignmentImplementationResults
    {
        readonly List<AlignmentItem> searchAlignnmentIndexes = new List<AlignmentItem>();
        readonly List<AlignmentItem> targetAlignnmentIndexes = new List<AlignmentItem>();

        /// <summary>
        /// Gets or sets the alignment score provider.
        /// </summary>
        public AlignmentScoreProviderBase ScoreProvider { get; set; }

        /// <summary>
        /// Gets or sets the index and characters aligned in search sequence
        /// </summary>
        public List<AlignmentItem> SearchAlignmentIndexes
        {
            get
            {
                return this.searchAlignnmentIndexes;
            }
        }

        /// <summary>
        /// Gets or sets the index and characters aligned in target sequence
        /// </summary>
        public List<AlignmentItem> TargetAlignmentIndexes
        {
            get
            {
                return this.targetAlignnmentIndexes;
            }
        }

        /// <summary>
        /// Gets the score matrix associated with result.
        /// </summary>
        public int[,] ScoreMatrix
        {
            get; set;
        }

        /// <summary>
        /// Gets the Optimal alignment score.
        /// </summary>
        public int AlignmentScore { get; set; }

        /// <summary>
        /// Gets the Optimal alignment sequence with gaps for target string.
        /// </summary>
        public string TargetSequenceAlignment { get; set; }

        /// <summary>
        /// Gets or sets the target sequence accession id.
        /// </summary>
        public string TargetAccessionId { get; set; }

        /// <summary>
        /// Gets the Optimal alignment sequence with gaps for search string.
        /// </summary>
        public string SearchSequenceAlignment { get; set; }

        /// <summary>
        /// Gets or sets the search sequence accession id.
        /// </summary>
        public string SearchAccessionId { get; set; }

        /// <summary>
        /// Pretty print function.
        /// </summary>
        public string PrettyPrint()
        {
            var stringBuilder = new StringBuilder();
            var searchAlignmentIndexes = new List<AlignmentItem>(this.SearchAlignmentIndexes);
            var targetAlignmentIndexes = new List<AlignmentItem>(this.TargetAlignmentIndexes);
            int lineBlock = 60;

            while (searchAlignmentIndexes.Count > 0)
            {
                int takeCount = searchAlignmentIndexes.Count >= lineBlock ? lineBlock : searchAlignmentIndexes.Count;
                var lines = new string[3];

                for (int i = 0; i < takeCount; i++)
                {
                    lines[0] = lines[0] + searchAlignmentIndexes[i].AlignedCharacter;
                    if (searchAlignmentIndexes[i].AlignedCharacter != '-' && targetAlignmentIndexes[i].AlignedCharacter != '-')
                    {
                        if (searchAlignmentIndexes[i].AlignedCharacter == targetAlignmentIndexes[i].AlignedCharacter)
                        {
                            lines[1] = lines[1] + searchAlignmentIndexes[i].AlignedCharacter;
                        }
                        else if (this.ScoreProvider.LookupPairwiseAlignmentScore(
                            searchAlignmentIndexes[i].AlignedCharacter, targetAlignmentIndexes[i].AlignedCharacter) > 0)
                        {
                            lines[1] = lines[1] + "+";
                        }
                        else if (this.ScoreProvider.LookupPairwiseAlignmentScore(
                            searchAlignmentIndexes[i].AlignedCharacter, targetAlignmentIndexes[i].AlignedCharacter) <= 0)
                        {
                            lines[1] = lines[1] + " ";
                        }
                    }
                    else
                    {
                        lines[1] = lines[1] + " ";
                    }

                    lines[2] = lines[2] + targetAlignmentIndexes[i].AlignedCharacter;
                }

                string paddedLine0Prefix = String.Format("{0}:  {1}", this.SearchAccessionId, searchAlignmentIndexes[0].Index).PadRight(15, ' ');
                string paddedLine1Prefix = String.Empty.PadRight(15, ' ');
                string paddedLine2Prefix = String.Format("{0}:  {1}", this.TargetAccessionId, targetAlignmentIndexes[0].Index).PadRight(15, ' ');

                stringBuilder.AppendFormat("{0}{1}", paddedLine0Prefix,lines[0]);
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("{0}{1}", paddedLine1Prefix, lines[1]);
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("{0}{1}", paddedLine2Prefix, lines[2]);
                stringBuilder.AppendLine();

                if (searchAlignmentIndexes.Count >= lineBlock)
                {
                    searchAlignmentIndexes.RemoveRange(0, lineBlock);
                }
                else
                {
                    searchAlignmentIndexes.Clear();
                }

                if (targetAlignmentIndexes.Count >= lineBlock)
                {
                    targetAlignmentIndexes.RemoveRange(0, lineBlock);
                }
                else
                {
                    targetAlignmentIndexes.Clear();
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Print score matrix for the result
        /// </summary>
        /// <returns></returns>
        public string PrettyPrintScoreMatrix()
        {
            if(this.ScoreMatrix == null) { throw new ArgumentNullException("ScoreMatrix not set"); }

            var stringBuilder = new StringBuilder();
            for (int i = 0; i <= this.ScoreMatrix.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= this.ScoreMatrix.GetUpperBound(1); j++)
                {
                    stringBuilder.Append(this.ScoreMatrix[i, j].ToString().PadRight(6,' '));
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
