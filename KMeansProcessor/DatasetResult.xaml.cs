using KMeansProcessor.BL;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace KMeansProcessor
{
    /// <summary>
    /// Interaction logic for DatasetResult.xaml
    /// </summary>
    public partial class DatasetResult : Page
    {
        public DatasetResult()
        {
            InitializeComponent();
        }

        public DatasetResult(string fileName) : this()
        {
            var data = DataProvider.GetData(fileName, false);

            var totalRowsTitle = new Paragraph();
            totalRowsTitle.Inlines.Add(new Run("Total rows"));
            totalRowsTitle.Padding = new Thickness(0, 100, 0, 0);

            TotalRowsRTb.Document.Blocks.Add(totalRowsTitle);
            TotalRowsRTb.Document.Blocks.Add(new Paragraph(new Bold(new Run(data.Count.ToString()))));

            var totalColumnsTitle = new Paragraph();
            totalColumnsTitle.Inlines.Add(new Run("Total columns"));
            totalColumnsTitle.Padding = new Thickness(0, 100, 0, 0);

            TotalColumnsRTb.Document.Blocks.Add(totalColumnsTitle);
            TotalColumnsRTb.Document.Blocks.Add(new Paragraph(new Bold(new Run(data.Columns.Count.ToString()))));
        }
    }
}
