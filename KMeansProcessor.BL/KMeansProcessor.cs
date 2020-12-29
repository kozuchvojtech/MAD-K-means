using KMeansProcessor.BL.Model;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KMeansProcessor.BL
{
    public static class KMeansProcessor
    {
        public static IEnumerable<Cluster> Process(IEnumerable<Record> records, int k)
        {
            var random = new Random();

            var centroids = records.OrderBy(x => random.Next()).Take(k).ToList();

            var clusters = centroids.Select(c => new Cluster 
                                    { 
                                        Centroid = c, 
                                        Vectors = new List<Record> { c } 
                                    })
                                    .ToList();

            var centroidChanged = false;

            do
            {
                centroidChanged = false;
                clusters.ForEach(c => c.Vectors = new List<Record>());

                foreach (var record in records.Except(centroids))
                {
                    var cluster = clusters.First(c => GetEuclidean(record, c.Centroid) == clusters.Min(c => GetEuclidean(record, c.Centroid)));
                    cluster.Vectors.Add(record);
                }

                foreach (var cluster in clusters)
                {
                    var averageInstance = GetAverageInstance(cluster.Vectors);

                    if (!Compare(averageInstance, cluster.Centroid, 0.5))
                    {
                        cluster.Centroid = averageInstance;
                        centroidChanged = true;
                    }
                }

                centroids = clusters.Select(c => c.Centroid).ToList();
            }
            while (centroidChanged);

            Console.Clear();

            foreach (var cluster in clusters.Select((c,i) => new { Item = c, Index = i }))
            {
                Console.WriteLine($"Cluster {cluster.Index}");

                var ratio = (double)cluster.Item.Vectors.Count/records.Count();

                Console.WriteLine($"Number of vectors \t {cluster.Item.Vectors.Count} ({ratio*100:0.00}%)");
                Console.WriteLine($"SSE \t\t\t {cluster.Item.Vectors.Sum(v => GetEuclidean(v, cluster.Item.Centroid))}\n");                
            }

            return clusters;
        }

        private static double GetEuclidean(Record record, Record centroid)
        {
            (var vectorA, var vectorB) = GetVectors(record, centroid);
            return Distance.Euclidean(vectorA, vectorB);
        }

        private static bool Compare(Record record, Record centroid, double error)
        {
            (var vectorA, var vectorB) = GetVectors(record, centroid);
            return vectorA.AlmostEqual(vectorB, error);
        }

        private static (Vector<double> vectorA, Vector<double> vectorB) GetVectors(Record recordA, Record recordB)
        {
            var nominalData = recordA.NominalData.Select((v, i) => v == recordB.NominalData.ElementAt(i) ? (double)0 : 1).ToList();

            var vectorA = recordA.NumericData;
            var vectorB = recordB.NumericData;

            nominalData.ForEach(nd =>
            {
                vectorA.Add(nd);
                vectorB.Add(nd);
            });

            return (vectorA, vectorB);
        }

        private static Record GetAverageInstance(List<Record> records)
        {
            var averageNumericInstance = MeanVarianceProcessor.GetAverageInstance(records.Select(r => r.NumericData));

            var commonNominalData = records.First()
                                           .NominalData
                                           .Select((v, i) => records.Select(r => r.NominalData.ElementAt(i))
                                                                    .GroupBy(v => v)
                                                                    .OrderByDescending(g => g.Count())
                                                                    .First()
                                                                    .Key)
                                           .ToList();

            return new Record
            {
                NumericData = averageNumericInstance,
                NominalData = commonNominalData
            };
        }
    }
}
