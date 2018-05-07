
using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel
{
    /// <summary>
    /// Class encpasulating Viterbi Result
    /// </summary>
    public class ViterbiResult
    {
        /// <summary>
        /// All possible sequences ending in state.
        /// </summary>
        public Dictionary<int,List<ViterbiStateSequence>> StateSequences { get; set; }

        /// <summary>
        /// Probabily score as determined by Viterbi Algorithm.
        /// </summary>
        public double ProbabilityScore { get; set; }

        /// <summary>
        /// State transition representation.
        /// </summary>
        public string StateTransitionRepresentaton { get; set; }

        /// <summary>
        /// Gets or sets the parameters used.
        /// </summary>
        public MarkovParameters ParametersUsed { get; set; }

        /// <summary>
        /// Prints readable version of Viterbi result.
        /// </summary>
        public string PrettyPrint(int interestedStateIndex, int numberOfHits)
        {
            var formattedReturn = new StringBuilder();
            formattedReturn.AppendLine("Viterbi Result");
            formattedReturn.AppendLine(this.ParametersUsed.PrettyPrint());
            formattedReturn.AppendLine(
                String.Format("Log probability : {0}", this.ProbabilityScore));
            formattedReturn.AppendLine(
                String.Format("Total number of hits : {0}", this.StateSequences[interestedStateIndex].Count));

            if (this.StateSequences[interestedStateIndex].Count == 0)
            {
                formattedReturn.AppendLine("No hits found");
                return formattedReturn.ToString();
            }

            numberOfHits = Math.Min(numberOfHits, this.StateSequences[interestedStateIndex].Count);

            // Printing hits in order they were found.
            formattedReturn.AppendLine();
            formattedReturn.AppendLine(String.Format("Printing first {0} hits", numberOfHits));
            formattedReturn.AppendLine("Start Index - End Index - Length");
            for (
                int i = this.StateSequences[interestedStateIndex].Count - 1; i >= (this.StateSequences[interestedStateIndex].Count - numberOfHits); i--)
            {
                formattedReturn.AppendLine(
                    String.Format(
                        "{0} - {1} - {2}",
                        this.StateSequences[interestedStateIndex][i].Index,
                        this.StateSequences[interestedStateIndex][i].Index + this.StateSequences[interestedStateIndex][i].StateSequence.Length,
                        this.StateSequences[interestedStateIndex][i].StateSequence.Length));
            }

            return formattedReturn.ToString();
        }

        /// <summary>
        ///  Prints all hits.
        /// </summary>
        public string PrettyPrintAllHits(int interestedStateIndex)
        {
            return this.PrettyPrint(interestedStateIndex, this.StateSequences[interestedStateIndex].Count);
        }
    }
}
