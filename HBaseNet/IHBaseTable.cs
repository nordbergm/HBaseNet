using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HBaseNet
{
    public interface IHBaseTable
    {
        string Name { get; }
        HBaseRow GetRow(string row);
        IHBaseTableScan Scan();
    }
}
