using System;
using System.Collections.Generic;
using System.Linq;
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

        #region Table Operations

        public IList<IHBaseTableData> GetTables()
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("Connection must be open to retrieve tables.");
            }

            return Client.getTableNames()
                    .Select(t => (IHBaseTableData)new HBaseTableData(t, Client.isTableEnabled(t), GetColumnFamilies(t)))
                    .ToList();
        }

        public IDictionary<byte[], IHBaseColumnFamilyData> GetColumnFamilies(byte[] tableName)
        {
            return Client.getColumnDescriptors(tableName).ToDictionary(cd => cd.Key, cd => (IHBaseColumnFamilyData)new HBaseColumnFamilyData(cd.Value));
        }

        #endregion

        #region Row Operations

        public IHBaseRowData GetRow(byte[] tableName, byte[] row)
        {
            var result = Client.getRow(tableName, row);

            if (result != null && result.Count > 0)
            {
                return new HBaseRowData(result);
            }

            return null;
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

        #endregion

        #region Column Operations

        public IList<IHBaseCellData> GetColumn(byte[] tableName, byte[] row, byte[] column)
        {
            return Client.get(tableName, row, column).Select(c => (IHBaseCellData)new HBaseCellData(c)).ToList();
        }

        #endregion

        #region Scanner Operations

        public IList<IHBaseRowData> Scan(byte[] tableName, byte[] startRow, IList<byte[]> columns, long? timestamp = null, int? numRows = null)
        {
            List<byte[]> columnsList = columns.ToList();

            if (timestamp.HasValue)
            {
                return ExhaustScanner(Client.scannerOpenTs(tableName, startRow, columnsList, timestamp.Value), numRows);
            }

            return ExhaustScanner(Client.scannerOpen(tableName, startRow, columnsList), numRows);
        }

        public IList<IHBaseRowData> ScanWithStop(byte[] tableName, byte[] startRow, byte[] stopRow, IList<byte[]> columns, long? timestamp = null, int? numRows = null)
        {
            List<byte[]> columnsList = columns.ToList();

            if (timestamp.HasValue)
            {
                return ExhaustScanner(Client.scannerOpenWithStopTs(tableName, startRow, stopRow, columnsList, timestamp.Value), numRows);
            }

            return ExhaustScanner(Client.scannerOpenWithStop(tableName, startRow, stopRow, columnsList), numRows);
        }

        public IList<IHBaseRowData> ScanWithPrefix(byte[] tableName, byte[] startRowPrefix, IList<byte[]> columns, int? numRows = null)
        {
            return ExhaustScanner(Client.scannerOpenWithPrefix(tableName, startRowPrefix, columns.ToList()), numRows);
        }

        private IList<IHBaseRowData> ExhaustScanner(int id, int? numRows = null)
        {
            if (numRows.HasValue)
            {
                return Client.scannerGetList(id, numRows.Value).Select(r => (IHBaseRowData)new HBaseRowData(r)).ToList();
            }

            return Client.scannerGet(id).Select(r => (IHBaseRowData)new HBaseRowData(r)).ToList();
        }

        #endregion

        #region Table Operations

        public void CreateTable(IHBaseTableData tableData)
        {
            Client.createTable(
                tableData.Name, 
                tableData.ColumnFamilies.Select(cf => new HBaseColumnFamilyData(cf.Value).ColumnDescriptor).ToList());
        }

        public void DeleteTable(byte[] tableName)
        {
            Client.deleteTable(tableName);
        }

        public void EnableTable(byte[] tableName)
        {
            Client.enableTable(tableName);
        }

        public void DisableTable(byte[] tableName)
        {
            Client.disableTable(tableName);
        }

        #endregion

        #region Mutation Operations

        public void MutateRows(byte[] tableName, IList<IHBaseMutation> mutations, long? timestamp = null)
        {
            if (timestamp.HasValue)
            {
                Client.mutateRowsTs(tableName, this.ConvertToClientMutation(mutations).ToList(), timestamp.Value);
            }
            else
            {
                Client.mutateRows(tableName, this.ConvertToClientMutation(mutations).ToList());   
            }
        }

        private IEnumerable<BatchMutation> ConvertToClientMutation(IEnumerable<IHBaseMutation> mutations)
        {
            var mutationsByRow = mutations.GroupBy(m => m.Row, new ByteArrayEqualityComparer());

            foreach (var mbr in mutationsByRow)
            {
                yield return new BatchMutation
                                {
                                    Row = mbr.Key,
                                    Mutations = mbr.Select(m => new Mutation
                                                                    {
                                                                        Column = m.Column,
                                                                        IsDelete = m.IsDelete,
                                                                        Value = m.Value
                                                                    })
                                                    .ToList()
                                };
            }

        }

        #endregion
    }
}
