using System;
using System.Collections.Generic;
using System.Text;

namespace DbExporter
{
    class ExportOptions
    {
        public string Server { get; set; }

        public string Database { get; set; }

        public bool IsWindowsAuthentication { get; set; } = false;

        public string User { get; set; }

        public string Password { get; set; }

        public string Query { get; set; }

        public string OutputPath { get; set; }

        public OutputFormat Format { get; set; } = OutputFormat.Csv;

        public string TableName { get; set; } = "<table>";

        public IReadOnlyList<string> ExcludeColumns { get; set; } = new List<string>();

        public Encoding Encoding { get; set; } = Encoding.GetEncoding("shift_jis");
    }
}
