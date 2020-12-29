using System.Collections.Generic;

namespace KMeansProcessor.BL.Model
{
    public class Cluster
    {
        public Record Centroid { get; set; }
        public List<Record> Vectors { get; set; }
    }
}
