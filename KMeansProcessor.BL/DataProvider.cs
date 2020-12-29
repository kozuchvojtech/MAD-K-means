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
            var columns = FetchData(fileName);

            var dataColumns = columns.Select(c => new DataColumn 
                                     { 
                                        Name = c.Title, 
                                        Data = c.Data.Cast<double>() 
                                     })
                                     .ToList();

            dataColumns.ForEach(MeanVarianceProcessor.CalculateMean);
            dataColumns.ForEach(MeanVarianceProcessor.CalculateTotalVariance);

            return new Data
            {
                Count = columns.First().Data.Count,
                Columns = dataColumns
            };
        }

        public static IEnumerable<Vector<double>> GetVectors(string fileName)
        {
            var columns = FetchData(fileName);
            return GetVectors(columns);
        }

        public static IEnumerable<Vector<double>> GetVectors(List<Column> columns)
        {
            var records = Enumerable.Range(0, columns.First().Data.Count).Select(i => columns.Select(c => c.Data.ElementAt(i)));
            return records.Select(Vector<double>.Build.DenseOfEnumerable);
        }

        public static List<Column> FetchData(string fileName)
        {
            using TextReader reader = File.OpenText(fileName);

            CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.Delimiter = ",";

            csv.Read();
            csv.ReadHeader();

            var index = 0;
            var headers = new Dictionary<int,string>();
            while (csv.TryGetField(index, out string headerColumn))
            {
                headers.Add(index, headerColumn);
                index++;
            }

            var typeConverterOptions = new TypeConverterOptionsCache();
            typeConverterOptions.GetOptions(typeof(double)).NumberStyle = NumberStyles.Any;

            csv.Configuration.TypeConverterOptionsCache = typeConverterOptions;

            var columns = new Dictionary<int, List<string>>();

            while(csv.Read())
            {
                index = 0;
                while(csv.TryGetField(index, out string field))
                {
                    if(columns.ContainsKey(index))
                    {
                        columns[index].Add(field);
                    }
                    else
                    {
                        columns.Add(index, new List<string> { field });
                    }

                    index++;
                }
            }

            return columns.Select((c, i) => Normalize(headers[i], c.Value)).ToList();
        }

        private static Column Normalize(string title, List<string> fields)
        {
            var data = Enumerable.Empty<double>();

            try
            {
                data = fields.Select(f => double.Parse(f)).ToList();
            }
            catch
            {
                var fieldsIds = fields.GroupBy(v => v)
                                      .Select((g, i) => new { Value = g.Key, Id = (double)i })
                                      .ToDictionary(f => f.Value, f => f.Id);

                data = fields.Select(f => fieldsIds[f]).ToList();
            }

            return new Column { Title = title, Data = data.ToList() };
        }
    }
}
