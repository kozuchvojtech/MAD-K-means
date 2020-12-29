using KMeansProcessor.BL;
using KMeansProcessor.BL.Model;
using ScottPlot;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace KMeansProcessor
{
    public partial class KMeansResult : Page
    {
        private readonly string fileName;
        private List<Cluster> clusters;

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
            var columns = DataProvider.FetchData(fileName);

            var axisColumns = new ObservableCollection<AxisColumn>();
            columns.Select((c, i) => new AxisColumn { Id = i, Title = c.Title }).ToList().ForEach(c => axisColumns.Add(c));

            AxisXColumnCB.ItemsSource = axisColumns;
            AxisXColumnCB.SelectedItem = axisColumns.First();

            AxisYColumnCB.ItemsSource = axisColumns;
            AxisYColumnCB.SelectedItem = axisColumns.ElementAt(1);

            var vectors = DataProvider.GetVectors(columns);

            clusters = BL.KMeansProcessor.Process(vectors, (int)KValueSlider.Value).ToList();

            RenderClusters();
        }

        private void PrintCluster(Cluster cluster, Plot plot)
        {
            var scatter = plot.PlotScatter(cluster.Vectors.Select(v => v.ElementAt(AxisXColumnCB.SelectedIndex)).ToArray(), 
                                           cluster.Vectors.Select(v => v.ElementAt(AxisYColumnCB.SelectedIndex)).ToArray(), 
                                           lineStyle: LineStyle.None, 
                                           markerSize: 7);

            plot.PlotPoint(cluster.Centroid.ElementAt(AxisXColumnCB.SelectedIndex), 
                           cluster.Centroid.ElementAt(AxisYColumnCB.SelectedIndex), 
                           markerShape: MarkerShape.cross, 
                           color: scatter.color,
                           markerSize: 12);
        }

        private void RenderClusters()
        {
            KMeansPlot.Reset();

            clusters.ForEach(c => PrintCluster(c, KMeansPlot.plt));
            KMeansPlot.Render();
        }

        private void ChangeKValue(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e) => ProcessKMeans();

        private void RerenderClusters(object sender, SelectionChangedEventArgs e)
        {
            if (clusters == null || AxisXColumnCB.SelectedIndex < 0 || AxisYColumnCB.SelectedIndex < 0)
            {
                return;
            }

            RenderClusters();
        }
    }

    public class AxisColumn
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
