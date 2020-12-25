using KMeansProcessor.BL.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KMeansProcessor.BL
{
    public static class DistributionProcessor
    {
        public static int Minimum { get; set; }
        public static int Maximum { get; set; }

        public static void CalculateNormalDistribution(DataColumn column)
        {
            column.NormalDistribution = new List<(double, double)>();

            for (double i = Minimum; i < Maximum; i += 0.1)
            {
                column.NormalDistribution.Add((i, GetNormalDistribution(i, column.Mean, column.TotalVariance)));
            }
        }

        public static void CalculateNormalDistributionEmpirical(DataColumn column)
        {
            column.NormalDistributionEmpirical = new List<double>();

            var intervals = Enumerable.Range(Minimum, (Maximum - Minimum) *2).Select(n => (from: (float)n / 2, to: (float)(n + 1) / 2));

            foreach (var interval in intervals)
            {
                column.NormalDistributionEmpirical.Add(column.Data.Count(d => d >= interval.from && d <= interval.to));
            }
        }

        public static void CalculateCumulativeDistribution(DataColumn column)
        {
            column.CumulativeDistribution = new List<(double, double)>();

            for (double i = Minimum; i < Maximum; i += 0.1)
            {
                double normalDistributionSum = 0;

                for (double j = Minimum; j < i; j += 0.1)
                {
                    normalDistributionSum += GetNormalDistribution(j, column.Mean, column.TotalVariance);
                }

                column.CumulativeDistribution.Add((i, normalDistributionSum));
            }
        }

        public static void CalculateCumulativeDistributionEmpirical(DataColumn column)
        {
            column.CumulativeDistributionEmpirical = new List<(double, double)>();

            for (double i = Minimum; i < Maximum; i += 0.5)
            {
                double normalDistributionSum = 0;

                for (double j = Minimum; j < i; j += 0.5)
                {
                    normalDistributionSum += column.Data.Count(d => d >= j && d <= (j + 0.5));
                }

                column.CumulativeDistributionEmpirical.Add((i, normalDistributionSum));
            }
        }

        private static double GetNormalDistribution(double x, double mean, double variance) => 1 / Math.Sqrt(2 * Math.PI * variance) * Math.Exp(-Math.Pow(x - mean, 2) / (2 * variance));
    }
}
