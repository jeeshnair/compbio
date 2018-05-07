using System;
using System.Text;

namespace ComputationalBiology.FastA
{
    /// <summary>
    /// Simple fasta parser which returns a fasta item.
    /// </summary>
    public static class FastAParser
    {
        public static SequenceMetadata ParseString(string fastaResource)
        {
            if (string.IsNullOrWhiteSpace(fastaResource))
            {
                throw new ArgumentNullException("fastaResource");
            }

            string[] lines = fastaResource.Split(
                new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);

            if (lines.Length == 0)
            {
                throw new InvalidOperationException("Invalid format");
            }

            // parse the header line 
            string[] headerparts = lines[0].Split('|');
            var sequence = new StringBuilder();
            for (int i = 1; i < lines.Length; i++)
            {
                sequence.Append(lines[i]);
            }

            return new SequenceMetadata
            {
                AccessionId = headerparts.Length == 2 ? headerparts[1] : headerparts[0],
                Description = headerparts.Length == 2 ? headerparts[2] : headerparts[0],
                Sequence = sequence.ToString()
            };
        }
    }
}
