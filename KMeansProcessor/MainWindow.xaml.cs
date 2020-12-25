using Microsoft.Win32;
using System.Windows;

namespace KMeansProcessor
{
    public partial class MainWindow : Window
    {
        private string fileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FetchData(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if(openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;

                KMeansDataBtn.IsEnabled = true;
                DistributionDataBtn.IsEnabled = true;
                MeanVarianceDataBtn.IsEnabled = true;
                DatasetDataBtn.IsEnabled = true;

                ResultsFrame.Navigate(new KMeansResult(fileName));
            }
        }

        private void LoadKMeansData(object sender, RoutedEventArgs e) => ResultsFrame.Navigate(new KMeansResult(fileName));

        private void LoadDistributionData(object sender, RoutedEventArgs e) => ResultsFrame.Navigate(new DistributionResult(fileName));

        private void LoadMeanVariance(object sender, RoutedEventArgs e) => ResultsFrame.Navigate(new MeanVarianceResult(fileName));

        private void LoadDataset(object sender, RoutedEventArgs e) => ResultsFrame.Navigate(new DatasetResult(fileName));
    }
}
