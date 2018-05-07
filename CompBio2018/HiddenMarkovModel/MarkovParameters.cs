using System.Collections.Generic;

namespace HiddenMarkovModel
{
    /// <summary>
    /// Base class transition and emission probabilities.
    /// </summary>
    public abstract class MarkovParameters
    {
        /// <summary>
        /// Reverse loop of state index in Vterbi matrix to actual state.
        /// </summary>
        public abstract Dictionary<int, char> StateIndices { get; }

        /// <summary>
        /// Returns state transition probability given from and to state.
        /// </summary>
        public abstract double GetStateTransitionProbabilty(char fromState, char toState);

        /// <summary>
        /// Returns emission probability given emission value and state
        /// </summary>
        public abstract double GetEmissionProbability(char value, char state);

        /// <summary>
        /// Updates parameters given the viterbi result.
        /// </summary>
        public abstract void UpdateParameters(ViterbiResult result);

        /// <summary>
        /// Prints formatted parameter values
        /// </summary>
        public abstract string PrettyPrint();

        /// <summary>
        /// Deep copies the object.
        /// </summary>
        /// <returns></returns>
        public abstract MarkovParameters Clone();
    }
}
