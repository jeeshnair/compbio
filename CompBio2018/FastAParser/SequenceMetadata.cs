using System.Collections.Generic;

namespace ComputationalBiology.FastA
{
    /// <summary>
    /// Class encapsulating fastA parsed resource.
    /// </summary>
    public class SequenceMetadata
    {
        /// <summary>
        /// Gets or sets the accession Id
        /// </summary>
        public string AccessionId { get; set; }

        /// <summary>
        /// Gets or sets the description of the sequence
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the Sequence.
        /// </summary>
        public string Sequence { get; set; }
    }
}