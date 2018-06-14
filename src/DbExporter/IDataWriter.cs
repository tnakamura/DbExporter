using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;

namespace DbExporter
{
    interface IDataWriter
    {
        Task WriteAllAsync(DbDataReader reader);
    }
}
