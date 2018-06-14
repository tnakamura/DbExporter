using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DbExporter
{
    class CsvWriter : IDataWriter
    {
        StreamWriter InnerWriter { get; }

        ExportOptions Options { get; }

        public CsvWriter(ExportOptions options, StreamWriter innerWriter)
        {
            Options = options;
            InnerWriter = innerWriter;
        }

        public async Task WriteAllAsync(DbDataReader reader)
        {
            var columns = reader.GetColumnSchema();
            var indexies = GetOutputColumnIndexies(columns);

            await WriteColumnsAsync(columns, indexies);

            while (await reader.ReadAsync())
            {
                await WriteRecordAsync(reader, indexies);
            }
        }

        int[] GetOutputColumnIndexies(IReadOnlyList<DbColumn> columns)
        {
            var indexies = new List<int>();
            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                if (!Options.ExcludeColumns.Contains(column.ColumnName))
                {
                    indexies.Add(i);
                }
            }
            return indexies.ToArray();
        }

        async Task WriteRecordAsync(IDataRecord record, int[] indexies)
        {
            var first = true;
            foreach (var i in indexies)
            {
                if (!first)
                {
                    await InnerWriter.WriteAsync(",");
                }
                await WriteCellAsync(record[i]);
                first = false;
            }

            await InnerWriter.WriteLineAsync();
        }

        async Task WriteColumnsAsync(IReadOnlyList<DbColumn> columns, int[] indexies)
        {
            var first = true;
            foreach (var i in indexies)
            {
                if (!first)
                {
                    await InnerWriter.WriteAsync(",");
                }
                var column = columns[i];
                await InnerWriter.WriteAsync(column.ColumnName);
                first = false;
            }
            await InnerWriter.WriteLineAsync();
        }

        async Task WriteCellAsync(object cell)
        {
            string value;
            if (cell == DBNull.Value)
            {
                return;
            }
            else if (cell is bool)
            {
                value = cell.ToString().ToLower();
            }
            else if (cell is string
                || cell is Guid
                || cell is DateTime
                || cell is DateTimeOffset
                || cell is TimeSpan)
            {
                value = $"'{cell}'";
            }
            else if (cell is byte[])
            {
                var binary = (byte[])cell;
                value = "0x" + string.Join("", binary.Select(b => Convert.ToString(b, 16)));
            }
            else
            {
                value = cell.ToString();
            }

            await InnerWriter.WriteAsync(value);
        }
    }
}
