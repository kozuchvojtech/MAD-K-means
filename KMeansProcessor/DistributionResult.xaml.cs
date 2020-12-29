using KMeansProcessor.BL;
using KMeansProcessor.BL.Model;
using ScottPlot;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KMeansProcessor
{
    public partial class DistributionResult : Page
    {
        public DistributionResult()
        {
            InitializeComponent();
        }

        public DistributionResult(string fileName) : this()
        {
            var columns = DataProvider.FetchData(fileName);
            columns = columns.Where(c => c.IsNumeric).ToList();

            var data = DataProvider.GetData(columns);
            var values = data.Columns.SelectMany(c => c.Data);

            DistributionProcessor.Minimum = (int)(values.Min() - data.Count*0.01);
            DistributionProcessor.Maximum = (int)(values.Max() + data.Count*0.01);

            var distributionTasks = new List<Task>();

            distributionTasks.Add(Task.Run(() => data.Columns.ForEach(DistributionProcessor.CalculateNormalDistribution)));
            distributionTasks.Add(Task.Run(() => data.Columns.ForEach(DistributionProcessor.CalculateNormalDistributionEmpirical)));
            distributionTasks.Add(Task.Run(() => data.Columns.ForEach(DistributionProcessor.CalculateCumulativeDistribution)));
            distributionTasks.Add(Task.Run(() => data.Columns.ForEach(DistributionProcessor.CalculateCumulativeDistributionEmpirical)));

            Task.WhenAll(distributionTasks).GetAwaiter().GetResult();

            var normalDistributionPlots = data.Columns.Select((c, i) => DisplayNormalDistribution(c, i, data.Columns.Count)).ToList();
            normalDistributionPlots.ForEach(ndp => PlotsGrid.Children.Add(ndp));

            data.Columns.ForEach(c => DisplayCumulativeDistribution(c, CumulativeDistribution.plt));
            data.Columns.ForEach(c => DisplayCumulativeDistributionEmpirical(c, CumulativeDistributionEmpirical.plt));

            CumulativeDistribution.plt.Title("Cumulative distribution");
            CumulativeDistribution.plt.Legend(true);
            CumulativeDistributionEmpirical.plt.Title("Cumulative distribution empirical");
            CumulativeDistributionEmpirical.plt.Legend(true);
        }

        private WpfPlot DisplayNormalDistribution(DataColumn column, int columnIndex, int columnsCount)
        {
            var wpfPlot = new WpfPlot
            {
                Margin = new System.Windows.Thickness(columnIndex*(1500/columnsCount), 0, 1500 - (columnIndex+1)*(1500/columnsCount), 500)
            };

            var normalDistributionEmpirical = new ScottPlot.Statistics.Histogram(column.Data.ToArray(), binSize: DistributionProcessor.StepSize*5, min: DistributionProcessor.Minimum, max: DistributionProcessor.Maximum);
            double barWidth = normalDistributionEmpirical.binSize * 1.2;

            wpfPlot.plt.PlotBar(normalDistributionEmpirical.bins, normalDistributionEmpirical.countsFrac, barWidth: barWidth, outlineWidth: 0, fillColor: Color.Gray);
            wpfPlot.plt.PlotScatter(column.NormalDistribution.Select(nd => nd.x).ToArray(), column.NormalDistribution.Select(nd => nd.y).ToArray(), markerSize: 0, lineWidth: 2);
            wpfPlot.plt.AxisAutoY(margin: 0);
            wpfPlot.plt.Axis(x1: DistributionProcessor.Minimum);
            wpfPlot.plt.Ticks(numericFormatStringY: "0.00");
            wpfPlot.plt.Title($"Normal distribution - {column.Name}");

            return wpfPlot;
        }

        private void DisplayCumulativeDistribution(DataColumn column, Plot plot)
        {
            plot.PlotScatter(column.CumulativeDistribution.Select(cd => cd.x).ToArray(), column.CumulativeDistribution.Select(cd => cd.y).ToArray(), label: column.Name, markerSize: 0);
        }

        private void DisplayCumulativeDistributionEmpirical(DataColumn column, Plot plot)
        {
            plot.PlotScatter(column.CumulativeDistributionEmpirical.Select(cd => cd.x).ToArray(), column.CumulativeDistributionEmpirical.Select(cd => cd.y).ToArray(), label: column.Name, markerSize: 0);
        }
    }
}
