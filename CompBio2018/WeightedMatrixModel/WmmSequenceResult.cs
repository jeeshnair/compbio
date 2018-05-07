using System.Collections.Generic;

namespace WeightedMatrixModel
{
    /// <summary>
    /// Class encapsulating the result from Wmm Scan algorithm.
    /// </summary>
    public class WmmSequenceResult
    {
        List<WmmSequenceResultItem> foundSequences = new List<WmmSequenceResultItem>();

        /// <summary>
        /// Returns all the sequence matches found through Weight matrix based sequence scan.
        /// </summary>
        public List<WmmSequenceResultItem> FoundSequences { get { return this.foundSequences; } }

        /// <summary>
        /// Gets or sets the parameters used.
        /// </summary>
        public WeightMatrixParameters ParametersUsed { get; set; }
    }
}
