using System;
using System.Collections.Generic;
using HBaseNet.Protocols;

namespace HBaseNet
{
    public interface IHBaseConnection : IDisposable
    {
        void Open();
        bool IsOpen { get; }
        void Close();

        IList<byte[]> GetTables();
        IList<IHBaseRowData> GetRows(IList<byte[]> rows, byte[] tableName, IList<byte[]> columns, long? timestamp);
        IList<IHBaseRowData> Scan(byte[] tableName, byte[] startRow, IList<byte[]> columns, long? timestamp = null, int? numRows = null);
        IList<IHBaseRowData> ScanWithStop(byte[] tableName, byte[] startRow, byte[] stopRow, IList<byte[]> columns, long? timestamp = null, int? numRows = null);
        IList<IHBaseRowData> ScanWithPrefix(byte[] tableName, byte[] startRowPrefix, IList<byte[]> columns, int? numRows = null);
    }
}
