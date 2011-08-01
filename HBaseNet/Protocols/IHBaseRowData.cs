using System.Collections.Generic;

namespace HBaseNet.Protocols
{
    public interface IHBaseRowData
    {
        byte[] Key { get; }
        IDictionary<byte[], IList<IHBaseCellData>> Columns { get; }
    }
}
