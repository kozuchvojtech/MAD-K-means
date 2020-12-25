using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace KMeansProcessor.BL.Model
{
    public class Cluster
    {
        public Vector<double> Centroid { get; set; }
        public List<Vector<double>> Vectors { get; set; }
    }
}
