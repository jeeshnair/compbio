using System.IO;
using System.Threading.Tasks;

namespace ComputationalBiology.FastA
{
    /// <summary>
    /// Lokks up resources by accession.
    /// In future will be wired to look it up directly from web.
    /// </summary>
    public static class FastALookupCient
    {
        public static async Task<SequenceMetadata> LookupByAccessionIdAsync(string Id)
        {
            using (StreamReader fileStream = File.OpenText(string.Format(@"FastAResources\{0}.fasta", Id)))
            {
                string resource = await fileStream.ReadToEndAsync();
                return FastAParser.ParseString(resource);
            }
        }
    }
}
