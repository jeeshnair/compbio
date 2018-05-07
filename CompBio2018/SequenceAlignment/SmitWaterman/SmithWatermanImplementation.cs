using System;
using ComputationalBiology.AlignmentScoreProvider;
using ComputationalBiology.FastA;

namespace ComputationalBiology.SequenceAlignment.SmithWaterman
{
    /// <summary>
    /// Encapsulates the implementation of the local alignment algorithm.
    /// </summary>
    public class SmithWatermanImplementation : AlignmentImplementationBase
    {
        int[,] substitutionMatrix = null;

        /// <summary>
        /// Creates a new instance of SmithWatermanImplementation
        /// </summary>
        /// <param name="sequenceTomatch">Sequence to match</param>
        /// <param name="targetSequence">Sequence to match against.</param>
        public SmithWatermanImplementation(
            SequenceMetadata sequenceTomatch,
            SequenceMetadata targetSequence,
            AlignmentScoreProviderBase scoreProvider,
            int gapOpenPenality) :
                base(sequenceTomatch, targetSequence, scoreProvider, gapOpenPenality)
        {
            this.IntializeSubstitutionMatrix();
        }

        /// <summary>
        /// Finds optimal alignment and returns local alignment scores and sequences.
        /// </summary>
        /// <returns></returns>
        public override AlignmentImplementationResults FindOptimalAlignment()
        {
            for (int rowIndex = 1; rowIndex < this.SequenceToMatch.Sequence.Length + 1; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex < this.TargetSequence.Sequence.Length + 1; columnIndex++)
                {
                    int diagonallyDerivedSubstitution =
                         substitutionMatrix[rowIndex - 1, columnIndex - 1] + this.ScoreProvider.LookupPairwiseAlignmentScore(
                             this.SequenceToMatch.Sequence[rowIndex - 1], this.TargetSequence.Sequence[columnIndex - 1]);
                    int verticallyDerivedSubstitution =
                        substitutionMatrix[rowIndex - 1, columnIndex] - this.GapOpenPenality;
                    int horizontallyDerivedSubstitution =
                        substitutionMatrix[rowIndex, columnIndex - 1] - this.GapOpenPenality;

                    int effectiveSubstitution = Math.Max(
                        val1: 0,
                        val2: Math.Max(
                            diagonallyDerivedSubstitution,
                            Math.Max(horizontallyDerivedSubstitution, verticallyDerivedSubstitution)));

                    substitutionMatrix[rowIndex, columnIndex] = effectiveSubstitution;
                }
            }

            return TracebackOptimalSequences();
        }

        /// <summary>
        /// Traceback logic to find out the optimally aligning sequences.
        /// </summary>
        /// <returns></returns>
        AlignmentImplementationResults TracebackOptimalSequences()
        {
            int maximumAlignmentScore = 0;
            int maximumrowIndex = 0;
            int maximumcolumnIndex = 0;
            for (int rowIndex = 0; rowIndex < this.SequenceToMatch.Sequence.Length + 1; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < this.TargetSequence.Sequence.Length + 1; columnIndex++)
                {
                    if (substitutionMatrix[rowIndex, columnIndex] > maximumAlignmentScore)
                    {
                        maximumAlignmentScore = substitutionMatrix[rowIndex, columnIndex];
                        maximumrowIndex = rowIndex;
                        maximumcolumnIndex = columnIndex;
                    }
                }
            }

            string targetSequenceAlignment = String.Empty;
            string searchSequenceAlignment = String.Empty;

            var result = new AlignmentImplementationResults
            {
                AlignmentScore = maximumAlignmentScore,
                ScoreProvider = this.ScoreProvider,
                ScoreMatrix = substitutionMatrix,
                TargetSequenceAlignment = targetSequenceAlignment,
                TargetAccessionId = this.TargetSequence.AccessionId,
                SearchAccessionId = this.SequenceToMatch.AccessionId,
                SearchSequenceAlignment = searchSequenceAlignment
            };

            this.TracebackContributingNeighbour(
                maximumAlignmentScore,
                maximumrowIndex,
                maximumcolumnIndex,
                result);

            return result;
        }

        /// <summary>
        /// Recursive function to track back the optimal alignment.
        /// </summary>
        void TracebackContributingNeighbour(
            int currentValue,
            int currentRowIndex,
            int currentColumnIndex,
            AlignmentImplementationResults result)
        {
            // No need to go further if we have found a begining of new gap.
            if (currentValue == 0)
            {
                return;
            }

            int diagonalNeighbour = substitutionMatrix[currentRowIndex - 1, currentColumnIndex - 1];
            int horizontalNeigbour = substitutionMatrix[currentRowIndex, currentColumnIndex - 1];
            int verticalNeigbour = substitutionMatrix[currentRowIndex - 1, currentColumnIndex];

            int diagonallyDerivedSubstitution =
                      diagonalNeighbour + this.ScoreProvider.LookupPairwiseAlignmentScore(
                           this.SequenceToMatch.Sequence[currentRowIndex - 1], 
                           this.TargetSequence.Sequence[currentColumnIndex - 1]);
            int verticallyDerivedSubstitution = verticalNeigbour - GapOpenPenality;
            int horizontallyDerivedSubstitution = horizontalNeigbour - GapOpenPenality;

            if (currentValue == diagonallyDerivedSubstitution)
            {
                result.TargetSequenceAlignment = 
                    this.TargetSequence.Sequence[currentColumnIndex - 1] + result.TargetSequenceAlignment;
                result.SearchSequenceAlignment = 
                    this.SequenceToMatch.Sequence[currentRowIndex - 1] + result.SearchSequenceAlignment;
                result.TargetAlignmentIndexes.Insert(0,
                    new AlignmentItem
                    {
                        Index = currentColumnIndex - 1,
                        AlignedCharacter = this.TargetSequence.Sequence[currentColumnIndex - 1]
                    });
                result.SearchAlignmentIndexes.Insert(0,
                  new AlignmentItem
                  {
                      Index = currentRowIndex - 1,
                      AlignedCharacter = this.SequenceToMatch.Sequence[currentRowIndex - 1]
                  });

                this.TracebackContributingNeighbour(
                    diagonalNeighbour,
                    currentRowIndex - 1,
                    currentColumnIndex - 1,
                    result);
            }
            else if (currentValue == verticallyDerivedSubstitution)
            {
                result.TargetSequenceAlignment = "-" + result.TargetSequenceAlignment;
                result.SearchSequenceAlignment = 
                    this.SequenceToMatch.Sequence[currentRowIndex - 1] + result.SearchSequenceAlignment;

                result.TargetAlignmentIndexes.Insert(0,
                  new AlignmentItem
                  {
                      // Copy the last index for a gap. It becomes easy to pretty print.
                      Index = result.TargetAlignmentIndexes[0].Index,
                      AlignedCharacter = '-'
                  });
                result.SearchAlignmentIndexes.Insert(0,
                  new AlignmentItem
                  {
                      Index = currentRowIndex - 1,
                      AlignedCharacter = this.SequenceToMatch.Sequence[currentRowIndex - 1]
                  });

                this.TracebackContributingNeighbour(
                    verticalNeigbour,
                    currentRowIndex - 1,
                    currentColumnIndex,
                    result);
            }
            else if (currentValue == horizontallyDerivedSubstitution)
            {
                result.TargetSequenceAlignment = 
                    this.TargetSequence.Sequence[currentColumnIndex - 1] + result.TargetSequenceAlignment;
                result.SearchSequenceAlignment = "-" + result.SearchSequenceAlignment;
                result.TargetAlignmentIndexes.Insert(0,
                   new AlignmentItem
                   {
                       Index = currentColumnIndex - 1,
                       AlignedCharacter = this.TargetSequence.Sequence[currentColumnIndex - 1]
                   });
                result.SearchAlignmentIndexes.Insert(0,
                  new AlignmentItem
                  {
                      // Copy the last index for a gap. It becomes easy to pretty print.
                      Index = result.SearchAlignmentIndexes[0].Index,
                      AlignedCharacter = '-'
                  });

                this.TracebackContributingNeighbour(
                    horizontalNeigbour,
                    currentRowIndex,
                    currentColumnIndex - 1,
                    result);
            }
        }

        /// <summary>
        /// Initialized substitution matrix with local alignment defaults.
        /// </summary>
        void IntializeSubstitutionMatrix()
        {
            substitutionMatrix = new int[
                this.SequenceToMatch.Sequence.Length + 1, this.TargetSequence.Sequence.Length + 1];

            // Initialize top row and left most column to zero.
            // Local alignment of string prefix in targetSequence to all gaps in sequenceTomatch is set to 0.
            for (int columnIndex = 0; columnIndex < this.TargetSequence.Sequence.Length + 1; columnIndex++)
            {
                substitutionMatrix[0, columnIndex] = 0;
            }

            for (int rowIndex = 0; rowIndex < this.SequenceToMatch.Sequence.Length + 1; rowIndex++)
            {
                substitutionMatrix[rowIndex, 0] = 0;
            }
        }
    }
}
