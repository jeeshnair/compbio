using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace HiddenMarkovModel
{
    /// <summary>
    /// Implementation of Viterbi most probably path, Traceback and Training specific to GC finding GC Patches.
    /// </summary>
    public class ViterbiImpl
    {
        MarkovParameters parameters;
        ViterbiMatrixElement[,] viterbiProbabilityMatrix;
        string input;

        /// <summary>
        /// Instantiates new class of type ViterbiImpl
        /// </summary>
        public ViterbiImpl(MarkovParameters parameters, string input)
        {
            if (parameters == null) { throw new ArgumentNullException("parameters"); }
            if (String.IsNullOrWhiteSpace(input)) { throw new ArgumentNullException("parameters"); }

            this.parameters = parameters;
            viterbiProbabilityMatrix = new ViterbiMatrixElement[parameters.StateIndices.Keys.Count, input.Length];
            this.input = input;

            IntializeMatrix();
        }

        /// <summary>
        /// First column is easy to calculate and can be seeded from Begin state and first input.
        /// </summary>
        void IntializeMatrix()
        {
            for (int rowIndex = 0; rowIndex <= viterbiProbabilityMatrix.GetUpperBound(0); rowIndex++)
            {
                var element = new ViterbiMatrixElement
                {
                    ContributingPredecessor = null,
                    StateIdentifier = this.parameters.StateIndices[rowIndex],
                    MaxLogProbability =
                        this.parameters.GetEmissionProbability(
                            value: this.input[0], state: this.parameters.StateIndices[rowIndex]) +
                        this.parameters.GetStateTransitionProbabilty('B', this.parameters.StateIndices[rowIndex]),
                    EmissionValue = this.input[0],
                    EmissionValueIndex = 0
                };

                viterbiProbabilityMatrix[rowIndex, 0] = element;
            }
        }

        /// <summary>
        /// Executes Viterbi in a loop , every loop begins with new updated parameters
        /// </summary>
        public List<ViterbiResult> ExecuteViterbiAndTrain(int executionCount)
        {
            var  resultsCollection = new List<ViterbiResult>();

            Stopwatch timer = new Stopwatch();
            timer.Restart();
            for (int i = 1; i <= executionCount; i++)
            {
                ViterbiResult result = FindViterbiProbabilityScoreAndPath();
                resultsCollection.Add(result);
                this.parameters.UpdateParameters(result);
            }
            timer.Stop();
            Console.WriteLine("Timetaken: {0}", timer.ElapsedMilliseconds);

            return resultsCollection;
        }

        /// <summary>
        /// Executes Viterbi on give input as per the defined parameters, scores and returns result which includes
        /// a)A state representation b)Viterbi log probability c)And Subsequences in positive model.
        /// </summary>
        public ViterbiResult FindViterbiProbabilityScoreAndPath()
        {
            for (int columnIndex = 1; columnIndex <= viterbiProbabilityMatrix.GetUpperBound(1); columnIndex++)
            {
                for (int rowIndex = 0; rowIndex <= viterbiProbabilityMatrix.GetUpperBound(0); rowIndex++)
                {
                    var element = new ViterbiMatrixElement
                    {
                        ContributingPredecessor = null,
                        StateIdentifier = this.parameters.StateIndices[rowIndex],
                        EmissionValue = this.input[columnIndex],
                        EmissionValueIndex = columnIndex
                    };

                    double maxProbability = double.MinValue;
                    ViterbiMatrixElement contributingPredecessor = null;

                    // calculate emission probability
                    for (int innerRowIndex = 0; innerRowIndex <= viterbiProbabilityMatrix.GetUpperBound(0); innerRowIndex++)
                    {
                        double probability;
                        probability = viterbiProbabilityMatrix[innerRowIndex, columnIndex - 1].MaxLogProbability +
                                    this.parameters.GetStateTransitionProbabilty(
                                        viterbiProbabilityMatrix[innerRowIndex, columnIndex - 1].StateIdentifier,
                                        this.parameters.StateIndices[rowIndex]);

                        if (probability > maxProbability)
                        {
                            maxProbability = probability;
                            contributingPredecessor = viterbiProbabilityMatrix[innerRowIndex, columnIndex - 1];
                        }
                    }
                    element.MaxLogProbability = this.parameters.GetEmissionProbability(
                                value: this.input[columnIndex], state: this.parameters.StateIndices[rowIndex]) + maxProbability;

                    // set the contributing predecessor so that we dont have to reacalculate in traceback.
                    element.ContributingPredecessor = contributingPredecessor;

                    viterbiProbabilityMatrix[rowIndex, columnIndex] = element;
                }
            }

            // Find the maximum probability
            ViterbiMatrixElement traceBackProbabilityElement = null;
            var traceBackProbability = Double.MinValue;
            for (int rowIndex = viterbiProbabilityMatrix.GetUpperBound(0); rowIndex >= 0; rowIndex--)
            {
                if (viterbiProbabilityMatrix[rowIndex, viterbiProbabilityMatrix.GetUpperBound(1)].MaxLogProbability
                        > traceBackProbability)
                {
                    traceBackProbability = viterbiProbabilityMatrix[rowIndex, viterbiProbabilityMatrix.GetUpperBound(1)]
                        .MaxLogProbability;
                    traceBackProbabilityElement = viterbiProbabilityMatrix[rowIndex, viterbiProbabilityMatrix.GetUpperBound(1)];
                }
            }

            // Viterbi Traceback.
            return ViterbiTraceback(traceBackProbabilityElement, traceBackProbability);
        }

        /// <summary>
        /// Takes Viterbi matrix element corresponding to maximum probability and maximum probability and computes
        /// a)StateRepresentation( FFFLLLFFFL as in dice roll example )
        /// b)All the sub sequences an starting indexes ending at all the states in the model.
        /// c)Probability score.
        /// </summary>
        ViterbiResult ViterbiTraceback(ViterbiMatrixElement traceBackProbabilityElement, double traceBackProbability)
        {
            var stateSequences = new Dictionary<int, List<ViterbiStateSequence>>();
            var statePath = new StringBuilder();
            var subSequence = new StringBuilder[this.parameters.StateIndices.Count];
            var startIndex = new int[this.parameters.StateIndices.Count];

            // Loop till we reach end of traceback list.
            while (traceBackProbabilityElement != null)
            {
                // Add to state identifiers , we will reverse it at the end.
                statePath.Append(traceBackProbabilityElement.StateIdentifier);
                foreach (KeyValuePair<int, char> item in this.parameters.StateIndices)
                {
                    if (!stateSequences.ContainsKey(item.Key))
                    {
                        stateSequences[item.Key] = new List<ViterbiStateSequence>();
                        subSequence[item.Key] = new StringBuilder();
                    }

                    if (traceBackProbabilityElement.StateIdentifier == item.Value)
                    {
                        // Start constructing subsequences at a given state. Again , we will reverse it at the end to align with input.
                        // Genomic indexes are 1 based. Increment the zero based index to reflect this convention.
                        startIndex[item.Key] = traceBackProbabilityElement.EmissionValueIndex + 1; ;
                        subSequence[item.Key].Append(traceBackProbabilityElement.EmissionValue);
                    }
                    else
                    {
                        // Add to subsequence list per state once we are at the end of subsequence ( when state corresponding to matrix element is different )
                        if (subSequence[item.Key].Length > 0)
                        {
                            stateSequences[item.Key].Add(
                                new ViterbiStateSequence
                                {
                                    Index = startIndex[item.Key],
                                    StateSequence = new string(subSequence[item.Key].ToString().ToCharArray().Reverse().ToArray())
                                });

                            //Reset the index and sequence variable
                            startIndex[item.Key] = 0;
                            subSequence[item.Key].Clear();
                        }
                    }

                    // If we are going to reach end of traceback then add the current calculated subsequence.
                    if (traceBackProbabilityElement.ContributingPredecessor == null
                        && subSequence[item.Key].Length > 0)
                    {
                        stateSequences[item.Key].Add(
                              new ViterbiStateSequence
                              {
                                  Index = startIndex[item.Key],
                                  StateSequence = new string(subSequence[item.Key].ToString().ToCharArray().Reverse().ToArray())
                              });
                    }
                }

                traceBackProbabilityElement = traceBackProbabilityElement.ContributingPredecessor;
            }

            // Construct the result.
            var result = new ViterbiResult()
            {
                ProbabilityScore = traceBackProbability,
                StateTransitionRepresentaton = new string(statePath.ToString().ToCharArray().Reverse().ToArray()),
                ParametersUsed = this.parameters.Clone()
            };

            // Add all the state subsequences.
            result.StateSequences = stateSequences;

            return result;
        }
    }
}
