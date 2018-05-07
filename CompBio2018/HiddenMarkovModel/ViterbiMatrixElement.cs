namespace HiddenMarkovModel
{
    /// <summary>
    /// Class representing a single element of Viterbi probabiluty Matrix.
    /// </summary>
    public class ViterbiMatrixElement
    {
        /// <summary>
        /// Term responsible for max log probability
        /// </summary>
        public ViterbiMatrixElement ContributingPredecessor { get; set; }

        /// <summary>
        /// Actual max log probability
        /// </summary>
        public double MaxLogProbability { get; set; }

        /// <summary>
        /// Identifier for the state. 
        /// </summary>
        public char StateIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the emission value associated with element.
        /// </summary>
        public char EmissionValue { get; set; }

        /// <summary>
        /// Index of emitted value with respect to the input string
        /// </summary>
        public int EmissionValueIndex { get; set; }
    }
}
