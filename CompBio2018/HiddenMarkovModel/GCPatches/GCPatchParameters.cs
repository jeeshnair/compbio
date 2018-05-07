using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel.GCPatches
{
    /// <summary>
    /// Class encpasulating parameters for GC Patches in archaea.
    /// </summary>
    public class GCPatchParameters : MarkovParameters
    {
        Dictionary<int, char> stateReverseLookupIndices = new Dictionary<int, char>()
        {
            { 0,'-' },
            { 1,'+' },
        };

        Dictionary<int, char> emissionReverseLookupIndices = new Dictionary<int, char>()
        {
            { 0, 'A' },
            { 1 , 'C' },
            { 2 , 'G' },
            { 3, 'T' },
        };

        /// <summary>
        /// Index of emission in parameter lookup matrix
        /// </summary>
        Dictionary<char, int> emissionLookupIndices = new Dictionary<char, int>()
        {
            { 'A',0 },
            { 'C',1 },
            { 'G',2 },
            { 'T',3 },
        };

        /// <summary>
        /// Index of state in parameter lookup matrix
        /// </summary>
        Dictionary<char, int> stateLookupIndices = new Dictionary<char, int>()
        {
            { '-',0 },
            { '+',1 },
            { 'B',2 },
        };

        /// <summary>
        /// Initial state transition probability values
        /// Begin is the last state.
        /// </summary>
        double[,] stateTransitionProbabilities =
        {
            { Math.Log(.9999),Math.Log(.0001) },
            { Math.Log(.01),Math.Log(.99) },
            { Math.Log(.9999),Math.Log(.0001) },
        };

        /// <summary>
        /// Initial emission probabilities.
        /// </summary>
        double[,] emissionProbabilities =
        {
            { Math.Log(.25),Math.Log(.25),Math.Log(.25),Math.Log(.25) },
            { Math.Log(.20),Math.Log(.30),Math.Log(.30),Math.Log(.20) },
        };

        public override Dictionary<int, char> StateIndices { get { return this.stateReverseLookupIndices; } }

        public override double GetEmissionProbability(char value, char state)
        {
            value = Char.ToUpperInvariant(value);
            // Treat anything other than AaCcGG as T;
            if (value != 'A' && value != 'C' && value != 'G' && value != 'T')
            {
                value = 'T';
            }
            return emissionProbabilities[stateLookupIndices[state], emissionLookupIndices[value]];
        }

        public override double GetStateTransitionProbabilty(char fromState, char toState)
        {
            return stateTransitionProbabilities[stateLookupIndices[fromState], stateLookupIndices[toState]];
        }

        public override void UpdateParameters(ViterbiResult result)
        {
            this.ClearProbabilities();

            //calculate the emission probabilities.
            foreach (KeyValuePair<int, char> item in this.StateIndices)
            {
                List<ViterbiStateSequence> statesequences = result.StateSequences[item.Key];
                double totalStateEmissions = 0;
                double countOfA = 0;
                double countOfC = 0;
                double countOfT = 0;
                double countOfG = 0;

                // calculate new emission probabilities
                foreach (ViterbiStateSequence viterbiSequence in statesequences)
                {
                    totalStateEmissions = totalStateEmissions + viterbiSequence.StateSequence.Length;
                    foreach (char emission in viterbiSequence.StateSequence)
                    {
                        switch (char.ToUpperInvariant(emission))
                        {
                            case 'A':
                                countOfA++;
                                break;
                            case 'C':
                                countOfC++;
                                break;
                            case 'G':
                                countOfG++;
                                break;
                            default:
                                countOfT++;
                                break;
                        }
                    }
                }

                emissionProbabilities[this.stateLookupIndices[item.Value], this.emissionLookupIndices['A']] =
                    Math.Log(countOfA / totalStateEmissions);
                emissionProbabilities[this.stateLookupIndices[item.Value], this.emissionLookupIndices['C']] =
                    Math.Log(countOfC / totalStateEmissions);
                emissionProbabilities[this.stateLookupIndices[item.Value], this.emissionLookupIndices['G']] =
                    Math.Log(countOfG / totalStateEmissions);
                emissionProbabilities[this.stateLookupIndices[item.Value], this.emissionLookupIndices['T']] =
                    Math.Log(countOfT / totalStateEmissions);
            }

            Dictionary<string, double> specificStateTransitionCounts = new Dictionary<string, double>();
            Dictionary<char, double> allFromStateTransitionCounts = new Dictionary<char, double>();
            for (int i = 1; i < result.StateTransitionRepresentaton.Length; i++)
            {
                if (!allFromStateTransitionCounts.ContainsKey(result.StateTransitionRepresentaton[i - 1]))
                {
                    allFromStateTransitionCounts.Add(result.StateTransitionRepresentaton[i - 1], 1);
                }
                else
                {
                    // Increment from to any state transition count.
                    allFromStateTransitionCounts[result.StateTransitionRepresentaton[i - 1]]
                        = allFromStateTransitionCounts[result.StateTransitionRepresentaton[i - 1]] + 1;
                }

                string specificStateTransitionKey =
                    String.Format(
                        "{0}_{1}",
                        result.StateTransitionRepresentaton[i - 1],
                        result.StateTransitionRepresentaton[i]);

                // Increment specific from to to state transition count.
                if (!specificStateTransitionCounts.ContainsKey(specificStateTransitionKey))
                {
                    specificStateTransitionCounts.Add(specificStateTransitionKey, 1);
                }
                else
                {
                    specificStateTransitionCounts[specificStateTransitionKey] =
                        specificStateTransitionCounts[specificStateTransitionKey] + 1;
                }
            }

            // calculate the state transition prbabilities.
            foreach (KeyValuePair<string, double> item in specificStateTransitionCounts)
            {
                // Big assumptions here on format of keys.
                char fromState = item.Key.Split('_')[0][0];
                char toState = item.Key.Split('_')[1][0];
                this.stateTransitionProbabilities[
                    this.stateLookupIndices[fromState],
                    this.stateLookupIndices[toState]] = Math.Log(
                        item.Value / allFromStateTransitionCounts[fromState]);
            }
        }

        /// <summary>
        /// Resets the probability parameters
        /// </summary>
        public void ClearProbabilities()
        {
            this.emissionProbabilities = new double[,]
            {
                {double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity },
                {double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity },
            };

            this.stateTransitionProbabilities = new double[,]
            {
                {double.NegativeInfinity,double.NegativeInfinity },
                {double.NegativeInfinity,double.NegativeInfinity },
                { Math.Log(.9999),Math.Log(.0001) },
            };
        }

        public override string PrettyPrint()
        {
            var formattedReturn = new StringBuilder();

            formattedReturn.AppendLine("HMM : State transition probability");
            //Print header rows.
            formattedReturn.Append(String.Empty.PadRight(6, ' '));
            for (int j = 0; j <= stateTransitionProbabilities.GetUpperBound(1); j++)
            {
                formattedReturn.Append(this.stateReverseLookupIndices[j].ToString().PadRight(25, ' '));
            }
            formattedReturn.AppendLine();

            //Not printing Begin => -/+ state transitions.
            for (int i = 0; i <= stateTransitionProbabilities.GetUpperBound(0)-1; i++)
            {
                formattedReturn.Append(this.stateReverseLookupIndices[i].ToString().PadRight(6, ' '));
                for (int j = 0; j <= stateTransitionProbabilities.GetUpperBound(1); j++)
                {
                    formattedReturn.Append(stateTransitionProbabilities[i, j].ToString().PadRight(25, ' '));
                }
                formattedReturn.AppendLine();
            }

            formattedReturn.AppendLine();
            formattedReturn.AppendLine("HMM : Emission probability");
            formattedReturn.Append(String.Empty.PadRight(6, ' '));
            for (int j = 0; j <= emissionProbabilities.GetUpperBound(1); j++)
            {
                formattedReturn.Append(this.emissionReverseLookupIndices[j].ToString().PadRight(25, ' '));
            }

            formattedReturn.AppendLine();
            for (int i = 0; i <= emissionProbabilities.GetUpperBound(0); i++)
            {
                formattedReturn.Append(this.stateReverseLookupIndices[i].ToString().PadRight(6, ' '));
                for (int j = 0; j <= emissionProbabilities.GetUpperBound(1); j++)
                {
                    formattedReturn.Append(emissionProbabilities[i, j].ToString().PadRight(25, ' '));
                }
                formattedReturn.AppendLine();
            }

            return formattedReturn.ToString();
        }

        public override MarkovParameters Clone()
        {
            var copiedObject = new GCPatchParameters
            {
                emissionProbabilities = (double[,])this.emissionProbabilities.Clone(),
                emissionLookupIndices = new Dictionary<char, int>(this.emissionLookupIndices),
                emissionReverseLookupIndices = new Dictionary<int, char>(this.emissionReverseLookupIndices),
                stateLookupIndices = new Dictionary<char, int>(this.stateLookupIndices),
                stateReverseLookupIndices = new Dictionary<int, char>(this.stateReverseLookupIndices),
                stateTransitionProbabilities = (double[,])this.stateTransitionProbabilities.Clone()
            };

            return copiedObject;
        }
    }
}
