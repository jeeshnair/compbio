
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkovModel.DiceRoll
{
    /// <summary>
    /// Positive Model here is loaded die , Back ground is fair.
    /// Code below is just an experiment and is not optimal.
    /// For example : use of dictionary makes this code more verbose than it should be.
    /// </summary>
    public class DiceRollParameters : MarkovParameters
    {
        Dictionary<string, double> stateTransitionProbability = new Dictionary<string, double>()
        {
            {"B_L", Math.Log(0.52)},
            {"B_F", Math.Log(0.48)},
            {"F_L", Math.Log(0.17)},
            {"F_F", Math.Log(0.83)},
            {"L_L", Math.Log(0.6)},
            {"L_F", Math.Log(0.4)},
        };

        Dictionary<string, double> emissionProbability = new Dictionary<string, double>()
        {
            {"L_1", Math.Log((double)1 / 10)},
            {"L_2", Math.Log((double)1 / 10)},
            {"L_3", Math.Log((double)1 / 10)},
            {"L_4", Math.Log((double)1 / 10)},
            {"L_5", Math.Log((double)1 / 10)},
            {"L_6", Math.Log((double)1 / 2)},
            {"F_1", Math.Log((double)1 / 6)},
            {"F_2", Math.Log((double)1 / 6)},
            {"F_3", Math.Log((double)1 / 6)},
            {"F_4", Math.Log((double)1 / 6)},
            {"F_5", Math.Log((double)1 / 6)},
            {"F_6", Math.Log((double)1 / 6)},
        };

        Dictionary<int, char> stateIndices = new Dictionary<int, char>()
        {
            { 0,'L' },
            { 1,'F' },
        };

        public override Dictionary<int, char> StateIndices { get { return this.stateIndices; } }

        public override double GetStateTransitionProbabilty(char fromState, char toState)
        {
            double returnValue;
            if (!this.stateTransitionProbability.TryGetValue(String.Format("{0}_{1}", fromState, toState), out returnValue))
            {
                throw new InvalidOperationException("No state transition probability found");
            }

            return returnValue;
        }

        public override double GetEmissionProbability(char value, char state)
        {
            double returnValue;
            if (!this.emissionProbability.TryGetValue(String.Format("{0}_{1}", state, value), out returnValue))
            {
                throw new InvalidOperationException("No Emission probability found");
            }

            return returnValue;
        }

        public override void UpdateParameters(ViterbiResult result)
        {
            this.ClearEmissionProbabilities();

            foreach (KeyValuePair<int, char> item in this.StateIndices)
            {
                List<ViterbiStateSequence> statesequences = result.StateSequences[item.Key];
                double totalStateEmissions = 0;
                double countOfOne = 0;
                double countOfTwo = 0;
                double countOfThree = 0;
                double countOfFour = 0;
                double countOfFive = 0;
                double countOfSix = 0;
                foreach (ViterbiStateSequence viterbiSequence in statesequences)
                {
                    totalStateEmissions = totalStateEmissions + viterbiSequence.StateSequence.Length;
                    foreach (char emission in viterbiSequence.StateSequence)
                    {
                        switch (emission)
                        {
                            case '1':
                                countOfOne++;
                                break;
                            case '2':
                                countOfTwo++;
                                break;
                            case '3':
                                countOfThree++;
                                break;
                            case '4':
                                countOfFour++;
                                break;
                            case '5':
                                countOfFive++;
                                break;
                            case '6':
                                countOfSix++;
                                break;
                            default:
                                break;
                        }
                    }
                }

                emissionProbability[String.Format("{0}_{1}", item.Value, "1")] = Math.Log(countOfOne / totalStateEmissions);
                emissionProbability[String.Format("{0}_{1}", item.Value, "2")] = Math.Log(countOfTwo / totalStateEmissions);
                emissionProbability[String.Format("{0}_{1}", item.Value, "3")] = Math.Log(countOfThree / totalStateEmissions);
                emissionProbability[String.Format("{0}_{1}", item.Value, "4")] = Math.Log(countOfFour / totalStateEmissions);
                emissionProbability[String.Format("{0}_{1}", item.Value, "5")] = Math.Log(countOfFive / totalStateEmissions);
                emissionProbability[String.Format("{0}_{1}", item.Value, "6")] = Math.Log(countOfSix / totalStateEmissions);
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
                    allFromStateTransitionCounts[result.StateTransitionRepresentaton[i - 1]]
                        = allFromStateTransitionCounts[result.StateTransitionRepresentaton[i - 1]] + 1;
                }

                string specificStateTransitionKey = String.Format("{0}_{1}", result.StateTransitionRepresentaton[i - 1], result.StateTransitionRepresentaton[i]);
                if (!specificStateTransitionCounts.ContainsKey(specificStateTransitionKey))
                {
                    specificStateTransitionCounts.Add(specificStateTransitionKey, 1);
                }
                else
                {
                    specificStateTransitionCounts[specificStateTransitionKey] = specificStateTransitionCounts[specificStateTransitionKey] + 1;
                }
            }

            foreach (KeyValuePair< string, double> item in specificStateTransitionCounts)
            {
                // Big assumptions here on format of keys.
                this.stateTransitionProbability[item.Key] = Math.Log(
                    item.Value / allFromStateTransitionCounts[item.Key.Split('_')[0][0]]);
            }
        }

        public void ClearEmissionProbabilities()
        {
            this.emissionProbability = this.emissionProbability.ToDictionary(p => p.Key, p => 0d);
        }

        public override string PrettyPrint()
        {
            var formattedReturn = new StringBuilder();
            formattedReturn.AppendLine("HMM : State transistion probability");
            foreach (KeyValuePair<string, double> item in stateTransitionProbability)
            {
                formattedReturn.AppendLine(String.Format("{0} : {1}", item.Key, item.Value));
            }

            formattedReturn.AppendLine("HMM : Emission probability");
            foreach (KeyValuePair<string, double> item in emissionProbability)
            {
                formattedReturn.AppendLine(String.Format("{0} : {1}", item.Key, item.Value));
            }

            return formattedReturn.ToString();
        }

        public override MarkovParameters Clone()
        {
            var copiedObject = new DiceRollParameters
            {
                emissionProbability = new Dictionary<string, double>(this.emissionProbability),
                stateIndices = new Dictionary<int, char>(this.stateIndices),
                stateTransitionProbability = new Dictionary<string, double>(this.stateTransitionProbability)
            };

            return copiedObject;
        }
    }
}
