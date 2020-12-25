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
        public DistributionResult()
        {
            InitializeComponent();
        }

        public DistributionResult(string fileName) : this()
        {
            var data = DataProvider.GetData(fileName);

            data.Columns.ForEach(DistributionProcessor.CalculateNormalDistribution);
            data.Columns.ForEach(DistributionProcessor.CalculateNormalDistributionEmpirical);
            data.Columns.ForEach(DistributionProcessor.CalculateCumulativeDistribution);
            data.Columns.ForEach(DistributionProcessor.CalculateCumulativeDistributionEmpirical);

            DisplayNormalDistribution(data.Columns.First(), NormalDistributionX.plt, Color.CornflowerBlue);
            DisplayNormalDistribution(data.Columns.ElementAt(1), NormalDistributionY.plt, Color.DarkOrange);

            data.Columns.ForEach(c => DisplayCumulativeDistribution(c, CumulativeDistribution.plt));
            data.Columns.ForEach(c => DisplayCumulativeDistributionEmpirical(c, CumulativeDistributionEmpirical.plt));

            CumulativeDistribution.plt.Title("Cumulative distribution");
            CumulativeDistribution.plt.Legend(true);
            CumulativeDistributionEmpirical.plt.Title("Cumulative distribution empirical");
            CumulativeDistributionEmpirical.plt.Legend(true);
        }

        private void DisplayNormalDistribution(DataColumn column, Plot plot, Color lineColor)
        {
            var normalDistributionEmpirical = new ScottPlot.Statistics.Histogram(column.Data.ToArray(), binSize: 0.5, min: -7, max: 7);
            double barWidth = normalDistributionEmpirical.binSize * 1.2;

            plot.PlotBar(normalDistributionEmpirical.bins, normalDistributionEmpirical.countsFrac, barWidth: barWidth, outlineWidth: 0, fillColor: Color.Gray);
            plot.PlotScatter(column.NormalDistribution.Select(nd => nd.x).ToArray(), column.NormalDistribution.Select(nd => nd.y).ToArray(), markerSize: 0, lineWidth: 2, color: lineColor);
            plot.AxisAutoY(margin: 0);
            plot.Axis(x1: -7);
            plot.Ticks(numericFormatStringY: "0.00");
            plot.Title($"Normal distribution - {column.Name}");
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
