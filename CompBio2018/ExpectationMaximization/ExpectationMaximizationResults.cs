using System;
using System.Collections.Generic;
using System.Text;

namespace ExpectationMaximization
{
    /// <summary>
    /// Collection of all the EM iteration results
    /// </summary>
    public class ExpectationMaximizationResults
    {
        public ExpectationMaximizationResults()
        {
        }

        IList<ExpectationMaximizationIterationResult> results = new List<ExpectationMaximizationIterationResult>();

        /// <summary>
        /// Gets the iteration result of EM Iterations.
        /// </summary>
        public IList<ExpectationMaximizationIterationResult> Results
        {
            get
            {
                return this.results;
            }
        }

        /// <summary>
        /// Prints results in readable format.
        /// </summary>
        public string PrettyPrint()
        {
            var stringBuilder = new StringBuilder();
            // Take first result and add a header row based on mean count
            for (int i = 0; i < this.Results[0].Means.Count; i++ )
            {
                stringBuilder.Append(
                    String.Format("mu{0}", i + 1).PadRight(ExpectationMaximizationIterationResult.PadIndex, ' '));
            }

            stringBuilder.Append(
                "LogLik".PadRight(ExpectationMaximizationIterationResult.PadIndex, ' '));

            stringBuilder.Append(
                "BIC".PadRight(ExpectationMaximizationIterationResult.PadIndex, ' '));

            foreach (ExpectationMaximizationIterationResult item in this.Results)
            {
                stringBuilder.AppendLine(item.PrettyPrintMeansAndScores());
            }

            foreach (ExpectationMaximizationIterationResult item in this.Results)
            {
                stringBuilder.AppendLine(item.PrettyPrintProbabilitySampling());
            }

            return stringBuilder.ToString();
        }
    }
}
