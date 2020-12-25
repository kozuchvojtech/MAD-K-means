using KMeansProcessor.BL;
using KMeansProcessor.BL.Model;
using ScottPlot;
using System;
using System.Linq;
using System.Text.Json;
using System.Windows.Controls;

namespace KMeansProcessor
{
    public partial class KMeansResult : Page
    {
        private readonly string fileName;

        public KMeansResult()
        {
            InitializeComponent();
        }

        public KMeansResult(string fileName) : this()
        {
            this.fileName = fileName;
            ProcessKMeans();
        }

        private void ProcessKMeans()
        {
            KMeansPlot.Reset();

            var vectors = DataProvider.GetVectors(fileName);
            var clusters = BL.KMeansProcessor.Process(vectors, (int)KValueSlider.Value).ToList();

            clusters.ForEach(c => PrintCluster(c, KMeansPlot.plt));

            KMeansPlot.Render();
        }

        private void PrintCluster(Cluster cluster, Plot plot)
        {
            var scatter = plot.PlotScatter(cluster.Vectors.Select(v => v.ElementAt(0)).ToArray(), cluster.Vectors.Select(v => v.ElementAt(1)).ToArray(), lineStyle: LineStyle.None, markerSize: 7);
            plot.PlotPoint(cluster.Centroid.ElementAt(0), cluster.Centroid.ElementAt(1), markerShape: MarkerShape.cross, color: scatter.color, markerSize: 12);

            Console.WriteLine($"Centroid: {JsonSerializer.Serialize(cluster.Centroid)}\n");
            Console.WriteLine($"Vectors: {JsonSerializer.Serialize(cluster.Vectors)}\n");
        }

        private void ChangeKValue(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e) => ProcessKMeans();
    }
}
