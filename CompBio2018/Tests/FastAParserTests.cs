using ComputationalBiology.FastA;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComputationalBiology.Tests
{
    [TestClass]
    public class FastAParserTests
    {
        [TestMethod]
        public void FastAParserMainline()
        {
            string testString = @"sp|P15172|MYOD1_HUMAN Myoblast determination protein 1 OS=Homo sapiens GN=MYOD1 PE=1 SV=3
MELLSPPLRDVDLTAPDGSLCSFATTDDFYDDPCFDSPDLRFFEDLDPRLMHVGALLKPE
EHSHFPAAVHPAPGAREDEHVRAPSGHHQAGRCLLWACKACKRKTTNADRRKAATMRERR
RLSKVNEAFETLKRCTSSNPNQRLPKVEILRNAIRYIEGLQALLRDQDAAPPGAAAAFYA
PGPLPPGRGGEHYSGDSDASSPRSNCSDGMMDYSGPPSGARRRNCYEGAYYNEAPSEPRP
GKSAAVSSLDCLSSIVERISTESPAAPALLLADVPSESPPRRQEAAAPSEGESSGDPTQS
PDAAPQCPAGANPNPIYQVL";

            SequenceMetadata item = FastAParser.ParseString(testString);
            Assert.AreEqual("P15172", item.AccessionId, "AccessionId mismatch");
            Assert.AreEqual(
                "MYOD1_HUMAN Myoblast determination protein 1 OS=Homo sapiens GN=MYOD1 PE=1 SV=3",
                item.Description,
                "AccessionId mismatch");
            Assert.IsTrue(
                item.Sequence.Contains("MELLSPPLRDVDLTAPDGSLCSFATTDDFYDDPCFDSPDLRFFEDLDPRLMHVGALLKPEEHSHFPAAVHPAPGAREDEHVRAPSGHHQAGRCLLWACKACKRKTTNADRRKAATMRERR"),
                "Sequence is wrong");
        }
    }
}
