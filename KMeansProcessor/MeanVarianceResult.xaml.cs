using KMeansProcessor.BL;
using KMeansProcessor.BL.Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace KMeansProcessor
{
    public partial class MeanVarianceResult : Page
    {
        public MeanVarianceResult()
        {
            InitializeComponent();
        }

        public MeanVarianceResult(string fileName) : this()
        {
            var columns = DataProvider.FetchData(fileName);
            columns = columns.Where(c => c.IsNumeric).ToList();

            var data = DataProvider.GetData(columns);

            var textBoxes = data.Columns.Select((c, i) => DisplayMeanVariance(c, data.Columns.Count, i)).ToList();
            textBoxes.ForEach(tb => MeanVarianceGrid.Children.Add(tb));

            var summary = new RichTextBox
            {
                Margin = new Thickness(0, 525, 0, 250),
                Background = new SolidColorBrush(Colors.LightGray),
                FontSize = 20,
                BorderThickness = new Thickness(0)
            };

            var totalMeanParagraph = new Paragraph();
            totalMeanParagraph.Inlines.Add(new Run("Total mean "));
            totalMeanParagraph.Inlines.Add(new Bold(new Run($"({string.Join(", ", data.Columns.Select(c => c.Mean).Select(ai => ai.ToString("0.00")))})")));
            totalMeanParagraph.FontSize = 25;
            totalMeanParagraph.FontFamily = new FontFamily("Arial");
            totalMeanParagraph.TextAlignment = TextAlignment.Center;

            var globalVariance = data.Columns.Sum(c => MeanVarianceProcessor.GetVariance(c.Data, c.Mean));

            var globalVarianceParagraph = new Paragraph();
            globalVarianceParagraph.Inlines.Add(new Run("Global variance "));
            globalVarianceParagraph.Inlines.Add(new Bold(new Run($"{globalVariance/data.Count:0.00}")));
            globalVarianceParagraph.FontSize = 25;
            globalVarianceParagraph.FontFamily = new FontFamily("Arial");
            globalVarianceParagraph.TextAlignment = TextAlignment.Center;

            summary.Document.Blocks.Add(totalMeanParagraph);
            summary.Document.Blocks.Add(globalVarianceParagraph);

            MeanVarianceGrid.Children.Add(summary);
        }

        private RichTextBox DisplayMeanVariance(DataColumn column, int columnsCount, int columnIndex)
        {
            var textBox = new RichTextBox
            {
                Margin = new Thickness(columnIndex * (1500 / columnsCount), 150, 1500 - (columnIndex + 1) * (1500 / columnsCount), 450),
                Background = new SolidColorBrush(Colors.LightGray),
                FontSize = 20,
                BorderThickness = new Thickness(0)
            };

            var paragraph = new Paragraph(new Run(column.Name));
            paragraph.FontSize = 30;
            paragraph.FontFamily = new FontFamily("Arial");
            paragraph.TextAlignment = TextAlignment.Center;

            textBox.Document.Blocks.Add(paragraph);

            var meanParagraph = new Paragraph();
            meanParagraph.Padding = new Thickness(10, 0, 0, 0);
            meanParagraph.Inlines.Add(new Run("mean"));
            meanParagraph.Inlines.Add(new Run("\t"));
            meanParagraph.Inlines.Add(new Bold(new Run($"{column.Mean:0.00}")));

            var varianceParagraph = new Paragraph();
            varianceParagraph.Padding = new Thickness(10, 0, 0, 0);
            varianceParagraph.Inlines.Add(new Run("variance"));
            varianceParagraph.Inlines.Add(new Run("\t"));
            varianceParagraph.Inlines.Add(new Bold(new Run($"{column.TotalVariance:0.00}")));

            textBox.Document.Blocks.Add(meanParagraph);
            textBox.Document.Blocks.Add(varianceParagraph);

            return textBox;
        }
    }
}
