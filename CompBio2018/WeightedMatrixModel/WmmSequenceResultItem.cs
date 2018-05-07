using System;

namespace WeightedMatrixModel
{
    /// <summary>
    /// Class encapsulating the Wmm result item.
    /// </summary>
    public class WmmSequenceResultItem
    {
        /// <summary>
        /// Log likelyhood Score of the subsequence
        /// </summary>
        public double LogLikelihoodScore { get; set; }

        /// <summary>
        /// Likely hood score
        /// </summary>
        public double LikelihoodScore
        {
            get
            {
                return Math.Pow(2, this.LogLikelihoodScore);
            }
        }

        /// <summary>
        /// Sequence.
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Index of the hit.
        /// </summary>
        public int DistanceFromCleavageSite { get; set; }

        /// <summary>
        /// Latent of hidden variable on the E Step.
        /// </summary>
        public double LatentVariable { get; set; }
    }
}
