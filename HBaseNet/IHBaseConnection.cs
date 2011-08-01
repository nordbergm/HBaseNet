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
    }
}
