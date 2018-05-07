using System;
using System.Collections.Generic;
using System.Linq;

namespace WeightedMatrixModel
{
    /// <summary>
    /// Weight matrix parameters representing Full consensus for string AATAAA
    /// </summary>
    public class FullConsensusWmm : WeightMatrixParameters
    {
        double[,] wmmModel =
        {
            { 2,2,double.NegativeInfinity,2,2,2 },
            { double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity },
            { double.NegativeInfinity,double.NegativeInfinity,2,double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity },
            { double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity,double.NegativeInfinity },
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
            return new FullConsensusWmm
            {
                wmmModel = (double[,])this.wmmModel.Clone()
            };
        }

        public override double GetRelativeEntropy()
        {
            double[] entropies = new double[this.wmmModel.GetUpperBound(1)+1];
            for (int j = 0; j <= this.wmmModel.GetUpperBound(1); j++)
            {
                for (int i = 0; i <= this.wmmModel.GetUpperBound(0); i++)
                {
                    if (!double.IsNegativeInfinity(this.wmmModel[i, j]))
                    {
                        entropies[j] = entropies[j] + (Math.Pow(2, this.wmmModel[i, j]) * .25 * this.wmmModel[i, j]);
                    }
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

        /// <remarks>Assignment does not need it . Hence this is not implemented</remarks>
        public override void UpateParameters(Dictionary<int, WmmSequenceResult> result)
        {
            throw new NotImplementedException();
        }
    }
}
