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
        public static IEnumerable<Cluster> Process(IEnumerable<Vector<double>> data, int k)
        {
            var random = new Random();

            var centroids = data.OrderBy(x => random.Next()).Take(k).ToList();

            var clusters = centroids.Select(c => new Cluster 
                                    { 
                                        Centroid = c, 
                                        Vectors = new List<Vector<double>> { c } 
                                    })
                                    .ToList();

            var centroidChanged = false;

            do
            {
                centroidChanged = false;
                clusters.ForEach(c => c.Vectors = new List<Vector<double>>());

                foreach (var vector in data.Except(centroids))
                {
                    var cluster = clusters.First(c => Distance.Euclidean(vector, c.Centroid) == clusters.Min(c => Distance.Euclidean(vector, c.Centroid)));
                    cluster.Vectors.Add(vector);
                }

                foreach (var cluster in clusters)
                {
                    var averageInstance = MeanVarianceProcessor.GetAverageInstance(cluster.Vectors);

                    if (!averageInstance.AlmostEqual(cluster.Centroid, 0.5))
                    {
                        cluster.Centroid = averageInstance;
                        centroidChanged = true;
                    }
                }

                centroids = clusters.Select(c => c.Centroid).ToList();
            }
            while (centroidChanged);


            foreach (var cluster in clusters.Select((c,i) => new { Item = c, Index = i }))
            {
                Console.WriteLine($"Cluster {cluster.Index}");

                var ratio = (double)cluster.Item.Vectors.Count/data.Count();

                Console.WriteLine($"Number of vectors \t {cluster.Item.Vectors.Count} ({ratio*100:0.00}%)");
                Console.WriteLine($"SSE \t\t\t {cluster.Item.Vectors.Sum(v => Distance.Euclidean(v, cluster.Item.Centroid))}\n");                
            }

            return clusters;
        }
    }
}
