using System.Collections.Generic;

namespace HBaseNet
{
    public interface IHBaseTable
    {
        byte[] Name { get; }
        HBaseRow GetRow(byte[] row, IList<byte[]> columns = null, long? timestamp = null);
        IList<HBaseRow> GetRows(IList<byte[]> rows, IList<byte[]> columns = null, long? timestamp = null);
        IList<HBaseRow> Scan(byte[] startRow, IList<byte[]> columns, long? timestamp = null, int? numRows = null);
        IList<HBaseRow> ScanWithStop(byte[] startRow, byte[] stopRow, IList<byte[]> columns, long? timestamp = null, int? numRows = null);
        IList<HBaseRow> ScanWithPrefix(byte[] startRowPrefix, IList<byte[]> columns, int? numRows = null);
    }
}
