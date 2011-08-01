using System.Collections.Generic;

namespace HBaseNet
{
    public interface IHBaseTable
    {
        byte[] Name { get; }
        HBaseRow GetRow(byte[] row, IList<byte[]> columns = null, long? timestamp = null);
        IList<HBaseRow> GetRows(IList<byte[]> rows, IList<byte[]> columns = null, long? timestamp = null);
        IHBaseTableScan Scan();
    }
}
