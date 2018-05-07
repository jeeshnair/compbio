namespace ComputationalBiology.AlignmentScoreProvider
{
    public abstract class AlignmentScoreProviderBase
    {
        public abstract int LookupPairwiseAlignmentScore(char source, char target);
    }
}
