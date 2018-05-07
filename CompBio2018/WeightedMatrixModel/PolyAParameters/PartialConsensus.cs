using System;
using System.Collections.Generic;
using System.Linq;

namespace WeightedMatrixModel
{
    /// <summary>
    /// Weight matrix parameters representing Partial consensus for sequence AATAAA
    /// </summary>
    public class PartialConsensus : WeightMatrixParameters
    {
        /// <summary>
        /// Log 2 likely hood ratio of foreground to background.
        /// </summary>
        double[,] wmmModel =
        {
            // A
            { 1.76,1.76,-2.32,1.76,1.76,1.76 },
            // C
            { -2.32,-2.32,-2.32,-2.32,-2.32,-2.32 },
            // T
            { -2.32,-2.32,1.76,-2.32,-2.32,-2.32 },
            // G
            { -2.32,-2.32,-2.32,-2.32,-2.32,-2.32 },
        };

        public override double[,] WmmModel
        {
            get
            {
                return this.wmmModel;
            }
        }

        public override WeightMatrixParameters Clone()
        {
            return new PartialConsensus
            {
                wmmModel = (double[,])this.wmmModel.Clone()
            };
        }

        public override double GetRelativeEntropy()
        {
            double[] entropies = new double[this.wmmModel.GetUpperBound(1) + 1];
            for (int j = 0; j <= this.wmmModel.GetUpperBound(1); j++)
            {
                for (int i = 0; i <= this.wmmModel.GetUpperBound(0); i++)
                {
                    entropies[j] = entropies[j] + Math.Pow(2, this.wmmModel[i, j]) * .25 * this.wmmModel[i, j];
                }
            }

            return entropies.Sum();
        }

        public override double GetWmmScore(char character, int position)
        {
            int rowIndex = 0;
            switch (character)
            {
                case 'A':
                    rowIndex = 0;
                    break;
                case 'C':
                    rowIndex = 1;
                    break;
                case 'T':
                    rowIndex = 2;
                    break;
                case 'G':
                    rowIndex = 3;
                    break;
                case 'N':
                    return (this.wmmModel[0, position] + this.wmmModel[1, position] + this.wmmModel[2, position] + this.wmmModel[3, position]) / 4;
                default:
                    throw new ArgumentNullException("Invalid character");
            }

            return this.wmmModel[rowIndex, position];
        }

        public override void UpateParameters(Dictionary<int, WmmSequenceResult> resultCollection)
        {
            var countOfBases = new double[4, 6];

            // Calculate new Wmm weighted by Yij
            foreach (WmmSequenceResult result in resultCollection.Values)
            {
                foreach (WmmSequenceResultItem foundSequence in result.FoundSequences)
                {
                    // foundSequence.Sequence.Length is the motif length and should be same for all motif instances
                    for (int i = 0; i < foundSequence.Sequence.Length; i++)
                    {
                        switch (foundSequence.Sequence[i])
                        {
                            case 'A':
                                countOfBases[0, i] = countOfBases[0, i] + foundSequence.LatentVariable;
                                break;
                            case 'C':
                                countOfBases[1, i] = countOfBases[1, i] + foundSequence.LatentVariable;
                                break;
                            case 'T':
                                countOfBases[2, i] = countOfBases[2, i] + foundSequence.LatentVariable;
                                break;
                            case 'G':
                                countOfBases[3, i] = countOfBases[3, i] + foundSequence.LatentVariable;
                                break;
                            case 'N':
                                countOfBases[0, i] = countOfBases[0, i] + (0.25 * foundSequence.LatentVariable);
                                countOfBases[1, i] = countOfBases[1, i] + (0.25 * foundSequence.LatentVariable);
                                countOfBases[2, i] = countOfBases[2, i] + (0.25 * foundSequence.LatentVariable);
                                countOfBases[3, i] = countOfBases[3, i] + (0.25 * foundSequence.LatentVariable);
                                break;
                            default:
                                throw new ArgumentNullException("Invalid character");
                        }
                    }
                }
            }

            this.wmmModel = new double[4, 6];
            for (int j = 0; j <= countOfBases.GetUpperBound(1); j++)
            {
                double totalCount = 0;
                for (int i = 0; i <= countOfBases.GetUpperBound(0); i++)
                {
                    totalCount = totalCount + countOfBases[i, j];
                }

                for (int i = 0; i <= countOfBases.GetUpperBound(0); i++)
                {
                    // Calculate new Wmm with updated frequency and uniform background.
                    this.wmmModel[i, j] = Math.Log((countOfBases[i, j] / totalCount) / .25, 2);
                }
            }
        }
    }
}
