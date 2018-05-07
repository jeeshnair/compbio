using System;
using System.Collections.Generic;
using System.Text;

namespace WeightedMatrixModel
{
    /// <summary>
    /// Base WMM parameters
    /// </summary>
    public abstract class WeightMatrixParameters
    {
        /// <summary>
        /// Gets the Wmm model matrix.
        /// </summary>
        public abstract double[,] WmmModel { get; }
        
        /// <summary>
        /// Returns a WMM score given a character and position.
        /// </summary>
        public abstract double GetWmmScore(char character, int position);

        /// <summary>
        /// Update the parameters given Wmm iteration result
        /// </summary>
        public abstract void UpateParameters(Dictionary<int, WmmSequenceResult> result);

        /// <summary>
        /// Clone the object
        /// </summary>
        public abstract WeightMatrixParameters Clone();

        /// <summary>
        /// Get relative entropy.
        /// </summary>
        public abstract double GetRelativeEntropy();

        /// <summary>
        /// Print the matrix
        /// </summary>
        /// <returns>Formatted string</returns>
        public virtual string Print()
        {
            var formattedReturn = new StringBuilder();
            formattedReturn.Append(String.Empty.PadRight(6, ' '));
            for (int j = 0; j <= this.WmmModel.GetUpperBound(1); j++)
            {
                formattedReturn.Append(String.Format("Col{0}", j + 1).PadRight(25, ' '));
            }
            formattedReturn.AppendLine();

            for (int i = 0; i <= this.WmmModel.GetUpperBound(0) ; i++)
            {
                switch (i)
                {
                    case 0:
                        formattedReturn.Append("A".PadRight(6, ' '));
                        break;
                    case 1:
                        formattedReturn.Append("C".PadRight(6, ' '));
                        break;
                    case 2:
                        formattedReturn.Append("T".PadRight(6, ' '));
                        break;
                    case 3:
                        formattedReturn.Append("G".PadRight(6, ' '));
                        break;
                }

                for (int j = 0; j <= this.WmmModel.GetUpperBound(1); j++)
                {
                    formattedReturn.Append(this.WmmModel[i, j].ToString().PadRight(25, ' '));
                }
                formattedReturn.AppendLine();
            }

            return formattedReturn.ToString();
        }
    }
}
