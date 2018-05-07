using System.Collections.Generic;
using System.Text;

namespace ExpectationMaximization
{
    /// <summary>
    /// Individual Iteration result for EM Algorithm.
    /// </summary>
    public class ExpectationMaximizationIterationResult
    {
        List<double> means = new List<double>();
        double[,] probabilityDensities;
        double[,] latentVariables;
        double[] inputs;

        public const int PadIndex = 11;

        /// <summary>
        /// Creates new instance of ExpectationMaximizationIterationResult class.
        /// </summary>
        public ExpectationMaximizationIterationResult(
            int mixtureCount, double[] inputs)
        {
            probabilityDensities = new double[inputs.Length, mixtureCount];
            latentVariables = new double[inputs.Length, mixtureCount];
            this.inputs = inputs;
        }

        /// <summary>
        ///  All the mean calculated for the iteration.
        /// </summary>
        public List<double> Means { get { return this.means; } }

        /// <summary>
        ///  All the probability densities for an iteration
        /// </summary>
        public double[,] ProbabilityDensities { get { return probabilityDensities; } }

        /// <summary>
        ///  All the latent variables z[ij] for an iteration.
        /// </summary>
        public double[,] LatentVariables { get { return latentVariables; } }

        /// <summary>
        /// Log likely hood for the iteration.
        /// </summary>
        public double LogLikelyHood { get; set; }

        /// <summary>
        /// The Bayesian Information Criteria.
        /// </summary>
        public double BicScore { get; set; }

        /// <summary>
        /// Prints the result in a readable format.
        /// </summary>
        public string PrettyPrintMeansAndScores()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            for (int i = 0; i < this.Means.Count; i++)
            {
                stringBuilder.Append(this.Means[i].ToString().PadRight(PadIndex, ' '));
            }
            stringBuilder.Append(this.LogLikelyHood.ToString().PadRight(PadIndex, ' '));
            stringBuilder.Append(this.BicScore.ToString().PadRight(PadIndex, ' '));

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Prints the result of probability sampling in a readable format.
        /// </summary>
        public string PrettyPrintProbabilitySampling()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("--Iteration Result: Probability Sampling--");

            int inputUpperLimit = this.ProbabilityDensities.GetUpperBound(0) >= 35 ? 
                35 : this.ProbabilityDensities.GetUpperBound(0);

            for (int i = 0; i <= inputUpperLimit; i++)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(
                           this.inputs[i].ToString().PadRight(20, ' '));

                for (int j = 0; j <= this.ProbabilityDensities.GetUpperBound(1); j++)
                {
                    stringBuilder.Append(
                            this.ProbabilityDensities[i, j].ToString("E5").PadRight(25, ' '));
                }
            }

            return stringBuilder.ToString();
        }
    }
}
