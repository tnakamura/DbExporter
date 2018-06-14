using System;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;

namespace DbExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var app = new CommandLineApplication(throwOnUnexpectedArg: false);

            var server = app.Option(
                "-s | --server <host>",
                "The server name",
                CommandOptionType.SingleValue);
            var database = app.Option(
                "-d | --database <database>",
                "The database name",
                CommandOptionType.SingleValue);
            var user = app.Option(
                "-u | --user <user>",
                "The user name",
                CommandOptionType.SingleValue);
            var password = app.Option(
                "-p | --password <password>",
                "The password",
                CommandOptionType.SingleValue);
            var windowsAuth = app.Option(
                "-w | --windows",
                "Use windows authentication",
                CommandOptionType.NoValue);
            var query = app.Option(
                "-q | --query <query>",
                "The select query",
                CommandOptionType.SingleValue);
            var output = app.Option(
                "-o | --output <path>",
                "The output file path",
                CommandOptionType.SingleValue);
            var format = app.Option(
                "-f | --format <format>",
                "The output file format",
                CommandOptionType.SingleValue);
            var table = app.Option(
                "-t | --table <table>",
                "The export table name",
                CommandOptionType.SingleValue);
            var exclude = app.Option(
                "--exclude <columns>",
                "The exclude columns",
                CommandOptionType.MultipleValue);

            app.HelpOption("-? | -h | --help");

            app.OnExecute(() =>
            {
                var options = new ExportOptions();

                if (server.HasValue())
                {
                    options.Server = server.Value();
                }
                if (database.HasValue())
                {
                    options.Database = database.Value();
                }
                if (user.HasValue())
                {
                    options.User = user.Value();
                }
                if (password.HasValue())
                {
                    options.Password = password.Value();
                }
                if (windowsAuth.HasValue())
                {
                    options.IsWindowsAuthentication = true;
                }
                if (query.HasValue())
                {
                    options.Query = query.Value();
                }
                if (output.HasValue())
                {
                    options.OutputPath = output.Value();
                }
                if (format.HasValue())
                {
                    var f = format.Value().ToLower();
                    if (f == "csv")
                    {
                        options.Format = OutputFormat.Csv;
                    }
                    else if (f == "sql")
                    {
                        options.Format = OutputFormat.Sql;
                    }
                }
                if (table.HasValue())
                {
                    options.TableName = table.Value();
                }
                if (exclude.HasValue())
                {
                    options.ExcludeColumns = exclude.Values;
                }

                var exporter = new Exporter(options);
                exporter.ExportAsync().GetAwaiter().GetResult();

                return 0;
            });

            app.Execute(args);
        }
    }
}