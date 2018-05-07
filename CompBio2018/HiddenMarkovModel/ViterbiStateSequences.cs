namespace HiddenMarkovModel
{
    /// <summary>
    /// Class representing Viterbi sequences.
    /// </summary>
    public class ViterbiStateSequence
    {
        /// <summary>
        /// Gets or sets the starting index of the state sequences
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the sequence for the state.
        /// </summary>
        public string StateSequence { get; set; }
    }
}
