using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Data.Common;

namespace DbExporter
{
    class Exporter
    {
        ExportOptions Options { get; }

        public Exporter(ExportOptions options)
        {
            Options = options;
        }

        DbConnection CreateConnection()
        {
            return CreateSqlConnection();
        }

        SqlConnection CreateSqlConnection()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = Options.Server;
            builder.InitialCatalog = Options.Database;
            if (Options.IsWindowsAuthentication)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = Options.User;
                builder.Password = Options.Password;
            }
            var connection = new SqlConnection(builder.ToString());
            return connection;
        }

        IDataWriter CreateDataWriter(StreamWriter innerWriter)
        {
            switch (Options.Format)
            {
                case OutputFormat.Csv:
                    return new CsvWriter(Options, innerWriter);
                case OutputFormat.Sql:
                    return new SqlWriter(Options, innerWriter);
                default:
                    throw new NotSupportedException($"{Options.Format} is not supported");
            }
        }

        public async Task ExportAsync()
        {
            using (var stream = File.OpenWrite(Options.OutputPath))
            {
                using (var writer = new StreamWriter(stream, Options.Encoding))
                {
                    using (var connection = CreateConnection())
                    {
                        await connection.OpenAsync();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = Options.Query;
                            command.CommandType = CommandType.Text;

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                var dataWriter = CreateDataWriter(writer);
                                await dataWriter.WriteAllAsync(reader);
                            }
                        }
                    }
                }
            }
        }
    }
}
