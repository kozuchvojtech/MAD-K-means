using System.Collections.Generic;

namespace KMeansProcessor.BL.Model
{
    public class DataColumn
    {
        public string Name { get; set; }
        public double Mean { get; set; }
        public double TotalVariance { get; set; }
        public IEnumerable<double> Data { get; set; }
        public List<(double x, double y)> NormalDistribution { get; set; }
        public List<double> NormalDistributionEmpirical { get; set; }
        public List<(double x, double y)> CumulativeDistribution { get; set; }
        public List<(double x, double y)> CumulativeDistributionEmpirical { get; set; }
    }
}
