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

        IList<IHBaseTableData> GetTables();
        IDictionary<byte[], IHBaseColumnFamilyData> GetColumnFamilies(byte[] tableName);
        IHBaseRowData GetRow(byte[] tableName, byte[] rows);
        IList<IHBaseRowData> GetRows(IList<byte[]> rows, byte[] tableName, IList<byte[]> columns, long? timestamp);
        IList<IHBaseRowData> Scan(byte[] tableName, byte[] startRow, IList<byte[]> columns, long? timestamp = null, int? numRows = null);
        IList<IHBaseRowData> ScanWithStop(byte[] tableName, byte[] startRow, byte[] stopRow, IList<byte[]> columns, long? timestamp = null, int? numRows = null);
        IList<IHBaseRowData> ScanWithPrefix(byte[] tableName, byte[] startRowPrefix, IList<byte[]> columns, int? numRows = null);
        IList<IHBaseCellData> GetColumn(byte[] tableName, byte[] row, byte[] column);
        void CreateTable(IHBaseTableData tableData);
        void DeleteTable(byte[] tableName);
        void EnableTable(byte[] tableName);
        void DisableTable(byte[] tableName);
        void MutateRows(byte[] tableName, IList<IHBaseMutation> mutations, long? timestamp = null);
    }
}
