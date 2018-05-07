using System;

namespace ComputationalBiology.AlignmentScoreProvider
{
    /// <summary>
    /// Simple score calculator based on lecture 2 slides.
    /// </summary>
    public class SimpleScoreProvider : AlignmentScoreProviderBase
    {
        public override int LookupPairwiseAlignmentScore(char source, char target)
        {
            if (Char.IsWhiteSpace(source)) { throw new ArgumentNullException("source"); }
            if (Char.IsWhiteSpace(target)) { throw new ArgumentNullException("target"); }

            if (source == target)
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }
    }
}
