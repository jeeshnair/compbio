using System;

namespace WeightedMatrixModel
{
    /// <summary>
    /// Class encapsulating Wmm sequence scan.
    /// </summary>
    public class WmmSequenceScanImpl
    {
        WeightMatrixParameters parameters;
        string sequence;
        int scanLength;

        /// <summary>
        /// Instantiates new instance of WmmSequenceScanImpl
        /// </summary>
        public WmmSequenceScanImpl(WeightMatrixParameters parameters, string sequence, int scanLength)
        {
            if( parameters == null) { throw new ArgumentNullException("parameters"); }
            if (String.IsNullOrWhiteSpace(sequence)) { throw new ArgumentNullException("sequence"); }

            this.parameters = parameters;
            this.scanLength = scanLength;
            this.sequence = sequence;
        }

        /// <summary>
        /// Finds matching sequences per the weight matrix model.
        /// </summary>
        public WmmSequenceResult FindScoringSequences()
        {
            var returnResult = new WmmSequenceResult()
            {
                ParametersUsed = this.parameters.Clone()
            };

            // Scan through sliding window of scan length and calculate the Wmm based score for each instance.
            for (int i = 0; i < sequence.Length - scanLength + 1; i++)
            {
                double score = 0;
                for (int j = 0; j < scanLength; j++)
                {
                    score = score + this.parameters.GetWmmScore(sequence[i + j], j);
                }

                var resultToAdd = new WmmSequenceResultItem
                {
                    DistanceFromCleavageSite = sequence.Length - i,
                    LogLikelihoodScore = score,
                    Sequence = sequence.Substring(i, scanLength),
                };

                returnResult.FoundSequences.Add(resultToAdd);
            }
            return returnResult;
        }
    }
}
