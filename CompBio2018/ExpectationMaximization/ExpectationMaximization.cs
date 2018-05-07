using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpectationMaximization
{
    /// <summary>
    /// Implementation of EM Algorithm.
    /// </summary>
    public class ExpectationMaximizationImpl
    {
        double[] inputs = null;
        int mixtureCount = 0;
        double probabilityRatio= 1;
        Random random = new Random();

        /// <summary>
        /// Instantiates a new isntance of EM Class.
        /// </summary>
        /// <remarks>
        /// Implementation assumes equal probability weightage across mixtures
        /// and a variance of 1.
        /// </remarks>
        public ExpectationMaximizationImpl(double[] inputs, int mixtureCount)
        {
            if (inputs == null || inputs.Length <= 0) { throw new ArgumentException("inputs"); }
            this.inputs = inputs;
            this.mixtureCount = mixtureCount;
            probabilityRatio = 1d / mixtureCount;
        }

        /// <summary>
        /// Finds the converged mean for the given input.
        /// </summary>
        public ExpectationMaximizationResults FindConvergedMeans()
        {
            var emResults = new ExpectationMaximizationResults();

            List<double> initialMeans = this.GetInitializationMeans();
            return this.CalculateExpectationAndMaximize(emResults, initialMeans);
        }

        /// <summary>
        ///  Recursive function to calculate the 
        /// </summary>
        ExpectationMaximizationResults CalculateExpectationAndMaximize(
            ExpectationMaximizationResults results,
            List<double> currentMeans)
        {
            var newIterationResult = new ExpectationMaximizationIterationResult(
                this.mixtureCount, this.inputs);
            newIterationResult.Means.AddRange(currentMeans);

            double previousLogLikeyHood = 0;
            if (results.Results.Count > 0)
            {
                previousLogLikeyHood = results.Results.Last().LogLikelyHood;
            }

            var newMeans = new List<double>();

            // Calculate the probability Densities
            // use the last calculated mean.
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < mixtureCount; j++)
                {
                    double probabilityDensity =
                       probabilityRatio * (Math.Pow(
                            Math.E,
                            -Math.Pow((inputs[i] - currentMeans[j]), 2) / 2) / Math.Sqrt(2 * Math.PI));
                    newIterationResult.ProbabilityDensities[i, j] = probabilityDensity;
                }
            }

            // Calculate the latent/hidden variables
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < mixtureCount; j++)
                {
                    double latentVariable = newIterationResult.ProbabilityDensities[i, j];
                    double sumOfLatentVariables = 0;
                    for (int k = 0; k < mixtureCount; k++)
                    {
                        sumOfLatentVariables = sumOfLatentVariables +
                            newIterationResult.ProbabilityDensities[i, k];
                    }

                    newIterationResult.LatentVariables[i, j] = latentVariable / sumOfLatentVariables;
                }
            }

            double logLikelyHood = 0;

            // Calculate the new mean Loglikely hood and BIC score 
            // for mixtures based on new hidden variable
            for (int j = 0; j < mixtureCount; j++)
            {
                double sumOfProductOfInputsAndHiddenVariable = 0;
                double sumOfHiddenVariable = 0;

                for (int i = 0; i < inputs.Length; i++)
                {
                    sumOfProductOfInputsAndHiddenVariable
                        = sumOfProductOfInputsAndHiddenVariable +
                            (inputs[i] * newIterationResult.LatentVariables[i, j]);

                    sumOfHiddenVariable
                      = sumOfHiddenVariable + newIterationResult.LatentVariables[i, j];

                    logLikelyHood = logLikelyHood + (newIterationResult.LatentVariables[i, j] *
                        Math.Pow(inputs[i] - currentMeans[j], 2)) / 2;
                }

                newMeans.Add(Math.Round(sumOfProductOfInputsAndHiddenVariable / sumOfHiddenVariable, 2));
            }

            newIterationResult.LogLikelyHood = Math.Round((this.inputs.Length * Math.Log(this.probabilityRatio)) -
                (this.inputs.Length * 0.5 * Math.Log(2 * Math.PI)) - logLikelyHood, 4);
            newIterationResult.BicScore = Math.Round((2 * newIterationResult.LogLikelyHood)
                - (this.mixtureCount * Math.Log(this.inputs.Length)), 4);

            results.Results.Add(newIterationResult);


            if (Math.Abs(previousLogLikeyHood - newIterationResult.LogLikelyHood) > .0001)
            {
                this.CalculateExpectationAndMaximize(results, newMeans);
            }

            return results;
        }

        /// <summary>
        ///  Initializes the means with random value selected between Min and max of inputs.
        /// </summary>
        List<double> GetInitializationMeans()
        {
            var means = new List<double>();

            for (int i = 0; i < this.mixtureCount; i++)
            {
                means.Add(this.random.Next(
                    (int)this.inputs.Min(), (int)this.inputs.Max()));
            }

            return means;
        }
    }
}
