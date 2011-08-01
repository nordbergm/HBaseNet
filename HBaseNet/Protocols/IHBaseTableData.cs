using System.Collections.Generic;

namespace HBaseNet.Protocols
{
    public interface IHBaseTableData
    {
        byte[] Name { get; }
        IDictionary<byte[], IHBaseColumnFamilyData> ColumnFamilies { get; }
    }
}
