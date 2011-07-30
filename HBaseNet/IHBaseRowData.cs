using System.Collections.Generic;

namespace HBaseNet
{
    public interface IHBaseRowData
    {
        string Key { get; }
        IDictionary<string, IHBaseCellData> Columns { get; }
    }
}
