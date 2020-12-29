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
        public static Data GetData(string fileName, bool calculateMeanVariance = true)
        {
            var columns = FetchData(fileName);
            return GetData(columns, calculateMeanVariance);
        }

        public static Data GetData(List<Column> columns, bool calculateMeanVariance = true)
        {
            var dataColumns = columns.Select(c => new DataColumn
                                     {
                                         Name = c.Title,
                                         Data = c.Data.Cast<double>()
                                     })
                                     .ToList();

            if (calculateMeanVariance)
            {
                dataColumns.ForEach(MeanVarianceProcessor.CalculateMean);
                dataColumns.ForEach(MeanVarianceProcessor.CalculateTotalVariance);
            }

            return new Data
            {
                Count = columns.First().Data.Count,
                Columns = dataColumns
            };
        }

        public static IEnumerable<Record> GetRecords(List<Column> columns)
        {
            return Enumerable.Range(0, columns.First().Data.Count).Select(i => GetRecord(columns, i));
        }

        private static Record GetRecord(List<Column> columns, int index)
        {
            var numericData = columns.Select(c => c.Data.ElementAt(index)).Where(v => v is double);
            var nominalData = columns.Select(c => c.Data.ElementAt(index)).Except(numericData).Cast<string>();

            return new Record
            {
                NominalData = nominalData,
                NumericData = Vector<double>.Build.DenseOfEnumerable(numericData.Cast<double>())
            };
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
            var isNumeric = false;
            var data = Enumerable.Empty<object>();

            if(fields.All(f => double.TryParse(f, out var value)))
            {
                data = fields.Select(f => double.Parse(f)).Cast<object>();
                isNumeric = true;
            }
            else
            {
                data = fields.Cast<object>();
                isNumeric = false;
            }

            return new Column { Title = title, Data = data.ToList(), IsNumeric = isNumeric };
        }
    }

    public class Record
    {
        public Vector<double> NumericData { get; set; }
        public IEnumerable<string> NominalData { get; set; }
    }
}
