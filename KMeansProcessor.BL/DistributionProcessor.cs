using KMeansProcessor.BL.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KMeansProcessor.BL
{
    public static class DistributionProcessor
    {
        public static double Minimum { get; set; }
        public static double Maximum { get; set; }
        public static double StepSize => (Maximum - Math.Abs(Minimum)) / 20;

        public static void CalculateNormalDistribution(DataColumn column)
        {
            column.NormalDistribution = new List<(double, double)>();
            (var minimum, var maximum) = GetColumnBoundaries(column);

            for (double i = minimum; i < maximum; i += StepSize)
            {
                column.NormalDistribution.Add((i, GetNormalDistribution(i, column.Mean, column.TotalVariance)));
            }
        }

        public static void CalculateNormalDistributionEmpirical(DataColumn column)
        {
            var normalDistributionsEmpirical = new ConcurrentBag<double>();
            var intervals = GetIntervals(column);

            Parallel.ForEach(intervals, interval =>
            {
                normalDistributionsEmpirical.Add(column.Data.Count(d => d >= interval.From && d <= interval.To));
            });

            column.NormalDistributionEmpirical = normalDistributionsEmpirical.ToList();
        }

        public static (double Minimum, double Maximum) GetColumnBoundaries(DataColumn column)
        {
            var minimum = column.Data.Min() - column.Data.Count() * 0.05;
            var maximum = column.Data.Max() + column.Data.Count() * 0.05;

            return (minimum, maximum);
        }

        public static (double, double) GetColumnBoundaries(List<DataColumn> columns)
        {
            var boundaries = columns.Select(GetColumnBoundaries);
            return (boundaries.Min(b => b.Minimum), boundaries.Max(b => b.Maximum));
        }

        private static IEnumerable<(double From, double To)> GetIntervals(DataColumn column)
        {
            (var minimum, var maximum) = GetColumnBoundaries(column);

            for (double i = minimum; i < maximum; i += StepSize)
            {
                yield return (i, i + StepSize);
            }
        }

        public static void CalculateCumulativeDistribution(DataColumn column)
        {
            column.CumulativeDistribution = new List<(double, double)>();

            for (double i = Minimum; i < Maximum; i += StepSize)
            {
                double normalDistributionSum = 0;

                for (double j = Minimum; j < i; j += StepSize)
                {
                    normalDistributionSum += GetNormalDistribution(j, column.Mean, column.TotalVariance);
                }

                column.CumulativeDistribution.Add((i, normalDistributionSum));
            }
        }

        public static void CalculateCumulativeDistributionEmpirical(DataColumn column)
        {
            column.CumulativeDistributionEmpirical = new List<(double, double)>();

            for (double i = Minimum; i < Maximum; i += StepSize)
            {
                double normalDistributionSum = 0;

                for (double j = Minimum; j < i; j += StepSize)
                {
                    normalDistributionSum += column.Data.Count(d => d >= j && d <= (j + StepSize));
                }

                column.CumulativeDistributionEmpirical.Add((i, normalDistributionSum));
            }
        }

        private static double GetNormalDistribution(double x, double mean, double variance) => 1 / Math.Sqrt(2 * Math.PI * variance) * Math.Exp(-Math.Pow(x - mean, 2) / (2 * variance));
    }
}
