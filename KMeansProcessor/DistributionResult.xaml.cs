using KMeansProcessor.BL;
using KMeansProcessor.BL.Model;
using ScottPlot;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;

namespace KMeansProcessor
{
    public partial class DistributionResult : Page
    {
        private const int Threshold = 1000;

        public DistributionResult()
        {
            InitializeComponent();
        }

        public DistributionResult(string fileName) : this()
        {
            var columns = DataProvider.FetchData(fileName);
            columns = columns.Where(c => c.IsNumeric).ToList();

            var data = DataProvider.GetData(columns);

            foreach (var column in data.Columns)
            {
                column.Data = column.Data.Take(Threshold);
            }

            (var minimum, var maximum) = DistributionProcessor.GetColumnBoundaries(data.Columns);

            DistributionProcessor.Minimum = minimum;
            DistributionProcessor.Maximum = maximum;

            data.Columns.ForEach(DistributionProcessor.CalculateNormalDistribution);
            data.Columns.ForEach(DistributionProcessor.CalculateNormalDistributionEmpirical);
            data.Columns.ForEach(DistributionProcessor.CalculateCumulativeDistribution);
            data.Columns.ForEach(DistributionProcessor.CalculateCumulativeDistributionEmpirical);

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
                Margin = new System.Windows.Thickness(columnIndex*(1800/columnsCount), 0, 1800 - (columnIndex+1)*(1800/columnsCount), 500)
            };

            (var minimum, var maximum) = DistributionProcessor.GetColumnBoundaries(column);

            var normalDistributionEmpirical = new ScottPlot.Statistics.Histogram(column.Data.ToArray(), binCount: 10, min: minimum, max: maximum);
            double barWidth = normalDistributionEmpirical.binSize * 1.2;

            wpfPlot.plt.PlotBar(normalDistributionEmpirical.bins, normalDistributionEmpirical.countsFrac.Select(v => v / normalDistributionEmpirical.binSize).ToArray(), barWidth: barWidth, outlineWidth: 0, fillColor: Color.Gray);
            wpfPlot.plt.PlotScatter(column.NormalDistribution.Select(nd => nd.x).ToArray(), column.NormalDistribution.Select(nd => nd.y).ToArray(), markerSize: 0, lineWidth: 2);
            wpfPlot.plt.AxisAutoY(margin: 0);
            wpfPlot.plt.Axis(x1: minimum);
            wpfPlot.plt.Ticks(numericFormatStringY: "0.00");
            wpfPlot.plt.Title(column.Name);

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
