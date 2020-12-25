using CsvHelper;
using CsvHelper.TypeConversion;
using KMeansProcessor.BL.Model;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace KMeansProcessor.BL
{
    public static class DataProvider
    {
        public static Data GetData(string fileName)
        {
            var data = FetchData(fileName);

            var columns = new List<DataColumn>()
            {
                new DataColumn { Name = "X", Data = data.Select(d => d.First()).Cast<double>() },
                new DataColumn { Name = "Y", Data = data.Select(d => d.ElementAt(1)).Cast<double>() }
            };

            columns.ForEach(MeanVarianceProcessor.CalculateMean);
            columns.ForEach(MeanVarianceProcessor.CalculateTotalVariance);

            return new Data
            {
                Count = data.Count,
                Columns = columns
            };
        }

        public static IEnumerable<Vector<double>> GetVectors(string fileName) => FetchData(fileName).Select(d => Vector<double>.Build.DenseOfEnumerable(d.Cast<double>()));

        public static List<List<object>> FetchData(string fileName)
        {
            using TextReader reader = File.OpenText(fileName);

            CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.Delimiter = ",";

            csv.Read();
            csv.ReadHeader();

            var typeConverterOptions = new TypeConverterOptionsCache();
            typeConverterOptions.GetOptions(typeof(double)).NumberStyle = NumberStyles.Any;

            csv.Configuration.TypeConverterOptionsCache = typeConverterOptions;

            var records = new List<List<object>>();

            while(csv.Read())
            {
                var columns = new List<object>();

                var index = 0;
                while(csv.TryGetField(index, out double column))
                {
                    columns.Add(column);
                    index++;
                }

                records.Add(columns);
            }

            return records.Where(r => r.Any()).ToList();
        }
    }
}
