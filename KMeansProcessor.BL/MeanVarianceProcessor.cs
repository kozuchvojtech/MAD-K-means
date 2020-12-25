using KMeansProcessor.BL.Model;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KMeansProcessor.BL
{
    public static class MeanVarianceProcessor
    {
        public static void CalculateMean(DataColumn column) => column.Mean = GetMean(column.Data);
        public static void CalculateTotalVariance(DataColumn column) => column.TotalVariance = GetTotalVariance(column.Data, column.Mean);
        public static double GetVariance(IEnumerable<double> data, double mean) => data.Sum(d => Math.Pow(Math.Sqrt(Math.Pow(d - mean, 2)), 2));
        public static Vector<double> GetAverageInstance(IEnumerable<Vector<double>> vectors) => Vector<double>.Build.DenseOfEnumerable(Enumerable.Range(0, vectors.First().Count).Select(cId => GetMean(vectors.Select(v => v.ElementAt(cId)))));
        private static double GetMean(IEnumerable<double> data) => data.Sum() / data.Count();
        private static double GetTotalVariance(IEnumerable<double> data, double mean) => data.Sum(d => Math.Pow(Math.Sqrt(Math.Pow(d - mean, 2)), 2)) / data.Count();
    }
}
