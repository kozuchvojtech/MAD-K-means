using KMeansProcessor.BL;
using KMeansProcessor.BL.Model;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;

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
            var data = DataProvider.GetData(fileName);

            var textBoxes = data.Columns.Select((c, i) => DisplayMeanVariance(c, data.Columns.Count, i)).ToList();
            textBoxes.ForEach(tb => MeanVarianceGrid.Children.Add(tb));    
        }

        private RichTextBox DisplayMeanVariance(DataColumn column, int columnsCount, int columnIndex)
        {
            var textBox = new RichTextBox
            {
                Margin = new System.Windows.Thickness(columnIndex * (1500 / columnsCount), 0, 1500 - (columnIndex + 1) * (1500 / columnsCount), 500)
            };

            var paragraph = new Paragraph(new Run(column.Name));
            paragraph.FontSize = 30;
            paragraph.FontFamily = new System.Windows.Media.FontFamily("Arial");
            paragraph.TextAlignment = System.Windows.TextAlignment.Center;

            textBox.Document.Blocks.Add(paragraph);

            textBox.Document.Blocks.Add(new Paragraph(new Run($"mean {column.Mean}")));
            textBox.Document.Blocks.Add(new Paragraph(new Run($"variance {column.Mean}")));

            return textBox;
        }
    }
}
