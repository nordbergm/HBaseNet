using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.HBase;
using Thrift.Protocol;
using Thrift.Transport;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseThriftConnection : IHBaseConnection
    {
        public HBaseThriftConnection(string host, int port)
        {
            Transport = new TBufferedTransport(new TSocket(host, port));
            Protocol = new TBinaryProtocol(Transport);
            Client = new Hbase.Client(Protocol);
        }

        public Hbase.Client Client { get; private set; }
        public TTransport Transport { get; private set; }
        public TProtocol Protocol { get; private set; }

        public bool IsOpen
        {
            get { return Transport.IsOpen; }
        }

        public void Open()
        {
            Transport.Open();
        }

        public void Close()
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("The connection is already closed.");
            }

            Transport.Close();
        }

        public void Dispose()
        {
            if (IsOpen)
            {
                this.Close();
            }
        }

        public IList<byte[]> GetTables()
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("Connection must be open to retrieve tables.");
            }

            return Client.getTableNames();
        }


        public IList<IHBaseRowData> GetRows(IList<byte[]> rows, byte[] tableName, IList<byte[]> columns, long? timestamp)
        {
            List<byte[]> rowsList = rows.ToList();

            if (columns == null || columns.Count == 0)
            {
                if (timestamp.HasValue)
                {
                    return Client.getRowsTs(tableName, rowsList, timestamp.Value).Select(r => (IHBaseRowData)new HBaseRowData(r)).ToList();
                }

                return Client.getRows(tableName, rowsList).Select(r => (IHBaseRowData)new HBaseRowData(r)).ToList();
            }

            List<byte[]> columnsList = columns.ToList();

            if (timestamp.HasValue)
            {
                return Client.getRowsWithColumnsTs(tableName, rowsList, columnsList, timestamp.Value).Select(r => (IHBaseRowData)new HBaseRowData(r)).ToList();
            }

            return Client.getRowsWithColumns(tableName, rowsList, columnsList).Select(r => (IHBaseRowData)new HBaseRowData(r)).ToList();
        }
    }
}
