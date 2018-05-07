using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalBiology.FastA;
using ComputationalBiology.SequenceAlignment;

namespace ComputationalBiology.EmpiricalAlignmentSignificance
{
    /// <summary>
    /// Class encapsulating p value calculation for a given alignment algorithm and inputs.
    /// </summary>
    public class PValueCalculator<T> where T: AlignmentImplementationBase
    {
        readonly AlignmentImplementationBase alignmentImpl = null;
        readonly int permutationLimit = 0;
        Random randomGenerator = new Random();
        StringBuilder stringBuilder = null;

        public PValueCalculator(T alignmentImpl, int permutationLimit)
        {
            if (alignmentImpl == null) { throw new ArgumentNullException("alignmentImpl"); }
            this.alignmentImpl = alignmentImpl;
            this.permutationLimit = permutationLimit;
        }

        /// <summary>
        /// Calculates P value and returns it in scientific notation string.
        /// </summary>
        /// <returns></returns>
        public async Task<string> CalculatePValueAsync()
        {
            // Get the actual score for the sequences we tried to align
            AlignmentImplementationResults results = alignmentImpl.FindOptimalAlignment();

            var scoringTasks = new List<Task<int[]>>();
            // Outer loop runs permutation loop.
            // Inner loop permutes the sequence.
            for (int i = 0; i < permutationLimit; i++)
            {
                scoringTasks.Add(this.PermuteStringAndScoreAsync());
            }

            int[][] scores = await Task.WhenAll(scoringTasks);
            List<int> flattenedScores = FlattenedArray(scores);

            // Find scores which are better than the actual score for initial pair of sequence.
            int[] equalOrBetterScores = flattenedScores.Where(
                s => s >= results.AlignmentScore).ToArray();

            return (((double)equalOrBetterScores.Length + 1) / 
                ((double)flattenedScores.Count + 1)).ToString("E5");
        }

        /// <summary>
        /// Flattens two dimensional array into a single array
        /// </summary>
        List<int> FlattenedArray(int[][] scores)
        {
            var flattenedScores = new List<int>();
            foreach (int[] row in scores)
            {
                foreach (int column in row)
                {
                    flattenedScores.Add(column);
                }
            }

            return flattenedScores;
        }

        /// <summary>
        /// Permutes and returns array of all scores for all permutations.
        /// </summary>
        Task<int[]> PermuteStringAndScoreAsync()
        {
            var scoringTasks = new List<Task<int>>();

            stringBuilder = new StringBuilder(this.alignmentImpl.TargetSequence.Sequence);

            for (int j = this.alignmentImpl.TargetSequence.Sequence.Length - 1; j > 0; j--)
            {
                int random = randomGenerator.Next(0, j);
                char temp = stringBuilder[j];

                stringBuilder[j] = stringBuilder[random];
                stringBuilder[random] = temp;
            }

            scoringTasks.Add(this.FindScoreAsync(
                   this.alignmentImpl.SequenceToMatch,
                   new SequenceMetadata
                   {
                       AccessionId = this.alignmentImpl.TargetSequence.AccessionId,
                       Description = this.alignmentImpl.TargetSequence.Description,
                       Sequence = stringBuilder.ToString()
                   }));

            return Task.WhenAll(scoringTasks);
        }

        /// <summary>
        /// Creates instance of algorithm and scores the inputs.
        /// </summary>
        Task<int> FindScoreAsync(SequenceMetadata sequenceToMatch, SequenceMetadata targetSequence)
        {
            var algorithm =
                 (T)Activator.CreateInstance(
                     typeof(T),
                     sequenceToMatch,
                     targetSequence,
                     this.alignmentImpl.ScoreProvider,
                     this.alignmentImpl.GapOpenPenality);

            AlignmentImplementationResults results = algorithm.FindOptimalAlignment();

            return Task.FromResult<int>(results.AlignmentScore);
        }
    }
}
