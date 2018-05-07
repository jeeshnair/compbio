using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExpectationMaximization;

namespace Homework3ArtifactGenerator
{
    class Program
    {
        const string InputDataFile0 = @"InputData\InputData_0.txt";
        const string InputDataFile1 = @"InputData\InputData_1.txt";
        const string outputLocationAssignmentSample = @"c:\temp\output_hw3_assigmentdata.txt";
        const string outputLocationFinalRun = @"c:\temp\output_hw3_finalrundata.txt";

        static void Main(string[] args)
        {
            List<double> inputs = null;
            List<ExpectationMaximizationResults> resultsCollection =
                new List<ExpectationMaximizationResults>();

            using (var fileStream = File.Open(outputLocationFinalRun, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream))
            {

                inputs = ReadInputsFromFile(InputDataFile1);
                resultsCollection = new List<ExpectationMaximizationResults>();

                WriteToConsoleAndFile("Processing final Test data", streamWriter);
                for (int i = 1; i < 6; i++)
                {
                    var emMaximization = new ExpectationMaximizationImpl
                        (inputs.ToArray(), mixtureCount: i);

                    ExpectationMaximizationResults results = emMaximization.FindConvergedMeans();
                    resultsCollection.Add(results);

                    WriteToConsoleAndFile(
                        String.Format("---------Run {0} Results------", i), streamWriter);
                    WriteToConsoleAndFile(results.PrettyPrint(), streamWriter);
                }

                WriteToConsoleAndFile(
                    String.Format(
                        "K Achieving maximum BIC Score {0}",
                        FindRunAchievingMaximumBICScore(resultsCollection)), streamWriter);

                streamWriter.Flush();
                fileStream.Flush();
            }

            using (var fileStream = File.Open(outputLocationAssignmentSample, FileMode.Create))
            using (var streamWriter = new StreamWriter(fileStream))
            {

                resultsCollection.Clear();
                inputs = ReadInputsFromFile(InputDataFile0);

                WriteToConsoleAndFile("Processing sample test data", streamWriter);
                for (int i = 1; i < 6; i++)
                {
                    var emMaximization = new ExpectationMaximizationImpl
                        (inputs.ToArray(), mixtureCount: i);

                    ExpectationMaximizationResults results = emMaximization.FindConvergedMeans();
                    resultsCollection.Add(results);

                    WriteToConsoleAndFile(
                        String.Format("---------Run {0} Results------", i), streamWriter);
                    WriteToConsoleAndFile(results.PrettyPrint(), streamWriter);
                }

                WriteToConsoleAndFile(
                 String.Format(
                     "K Achieving maximum BIC Score {0}",
                     FindRunAchievingMaximumBICScore(resultsCollection)), streamWriter);
            }

            Console.ReadLine();
        }

        static List<double> ReadInputsFromFile(string fileName)
        {
            var inputs = new List<double>();
            using (StreamReader fileStream = File.OpenText(fileName))
            {
                while (!fileStream.EndOfStream)
                {
                    string line = fileStream.ReadLine();
                    inputs.AddRange(line.Split(' ').Select(c => Convert.ToDouble(c)));
                }
            }

            return inputs;
        }

        static int FindRunAchievingMaximumBICScore(List<ExpectationMaximizationResults> results)
        {
            double maxBicScore = results[0].Results.Last().BicScore;
            int classificationCount = results[0].Results.Last().Means.Count;
            foreach (ExpectationMaximizationResults item in results)
            {
                if (item.Results.Last().BicScore > maxBicScore)
                {
                    maxBicScore = item.Results.Last().Means.Count;
                    classificationCount = item.Results.Last().Means.Count;
                }
            }

            return classificationCount;
        }

        static void WriteToConsoleAndFile(string toWrite, StreamWriter streamWriter)
        {
            Console.WriteLine(toWrite);
            streamWriter.WriteLine(toWrite);
        }
    }
}
