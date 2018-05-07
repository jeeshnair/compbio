using System;
using ComputationalBiology.AlignmentScoreProvider;
using ComputationalBiology.FastA;

namespace ComputationalBiology.SequenceAlignment
{
    /// <summary>
    /// Base class to  abstract various alignment implementations.
    /// </summary>
    public abstract class AlignmentImplementationBase
    {
        /// <summary>
        /// Gets or sets the gap open penality to be applied.
        /// </summary>
        public int GapOpenPenality
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the target sequence metadata.
        /// </summary>
        public SequenceMetadata TargetSequence
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the search sequence  metadata.
        /// </summary>
        public SequenceMetadata SequenceToMatch
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the score provider to use.
        /// </summary>
        public AlignmentScoreProviderBase ScoreProvider
        {
            get;
            private set;
        }


        /// <summary>
        /// Creates a new instance alignment algorithm.
        /// </summary>
        public AlignmentImplementationBase(
          SequenceMetadata sequenceTomatch, 
          SequenceMetadata targetSequence, 
          AlignmentScoreProviderBase scoreProvider, int gapOpenPenality)
        {
            if (sequenceTomatch == null) { throw new ArgumentNullException("sequenceTomatch"); }
            if (targetSequence == null) { throw new ArgumentNullException("targetSequence"); }
            if (scoreProvider == null) { throw new ArgumentNullException("scoreProvider"); }

            this.TargetSequence = targetSequence;
            this.SequenceToMatch = sequenceTomatch;
            this.ScoreProvider = scoreProvider;
            this.GapOpenPenality = gapOpenPenality;
        }

        /// <summary>
        /// Finds optimal alignment and returns the results.
        /// </summary>
        /// <returns></returns>
        public abstract AlignmentImplementationResults FindOptimalAlignment();
    }
}
