using System;
using System.Collections.Generic;
using System.Linq;

namespace WeightedMatrixModel
{
    public class MemeImpl
    {
        WeightMatrixParameters parameters = null;
        string[] sequences = null;
        int scanLength = 0;
        Dictionary<int, WmmSequenceResult> resultCollection = new Dictionary<int, WmmSequenceResult>();

        public MemeImpl(WeightMatrixParameters parameters, string[] sequences, int scanLength)
        {
            if (parameters == null) { throw new ArgumentNullException("parameters"); }
            if (sequences == null) { throw new ArgumentNullException("sequences"); }

            this.parameters = parameters;
            this.sequences = sequences;
            this.scanLength = scanLength;
        }

        public Dictionary<int, WmmSequenceResult> FindScoringSequences(int iterationCount)
        {
            for (int iteration = 1; iteration <= iterationCount; iteration++)
            {
                // Find all possible Motif instances and their log probability scores.
                for (int i = 0; i < sequences.Length; i++)
                {
                    var wmmImpl = new WmmSequenceScanImpl(
                         parameters,
                         sequence: sequences[i],
                         scanLength: this.scanLength);
                    WmmSequenceResult result = wmmImpl.FindScoringSequences();

                    resultCollection.Add(i, result);
                }

                // Calculate hidden variable a.k.a Yij Hat.
                foreach (WmmSequenceResult result in resultCollection.Values)
                {
                    double sumOfProbabilities = result.FoundSequences.Sum(s => s.LikelihoodScore);
                    foreach (WmmSequenceResultItem foundSequence in result.FoundSequences)
                    {
                        foundSequence.LatentVariable = foundSequence.LikelihoodScore / sumOfProbabilities;
                    }
                }
            }

            this.parameters.UpateParameters(resultCollection);

            // Execute last iteration with new parameters.
            resultCollection.Clear();
            for (int i = 0; i < sequences.Length; i++)
            {
                var wmmImpl = new WmmSequenceScanImpl(
                     parameters,
                     sequence: sequences[i],
                     scanLength: this.scanLength);
                WmmSequenceResult result = wmmImpl.FindScoringSequences();

                resultCollection.Add(i, result);
            }
            return resultCollection;
        }
    }
}
