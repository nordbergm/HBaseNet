using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using Xunit;

namespace HBaseNet.Protocols.Thrift.IntegrationTests
{
    [PrioritizedFixture]
    public class HBaseThriftConnectionTests
    {
        private const string Host = "hbase";
        private const int Port = 9091;

        private static readonly byte[] TableName = Encoding.UTF8.GetBytes("t" + Guid.NewGuid().ToString().Normalize());
        private static readonly byte[] ColFamName = Encoding.UTF8.GetBytes("cf" + Guid.NewGuid().ToString().Normalize());
        private static readonly byte[] RowKey1 = Encoding.UTF8.GetBytes("k" + Guid.NewGuid().ToString().Normalize());
        private static readonly byte[] RowKey2 = Encoding.UTF8.GetBytes("k" + Guid.NewGuid().ToString().Normalize());
        private static readonly byte[] ColName = Encoding.UTF8.GetBytes("c" + Guid.NewGuid().ToString().Normalize());
        private static readonly byte[] ColNameToDelete = Encoding.UTF8.GetBytes("c" + Guid.NewGuid().ToString().Normalize());
        private static readonly byte[] FqColName;
        private static readonly byte[] FqColNameToDelete;
        private static readonly byte[] CellVal = Encoding.UTF8.GetBytes("v" + Guid.NewGuid().ToString().Normalize());
        private static long deleteTimestamp = 0;

        static HBaseThriftConnectionTests()
        {
            FqColName = new byte[ColFamName.Length + ColName.Length + 1];
            ColFamName.CopyTo(FqColName, 0);
            FqColName[ColFamName.Length] = (byte)':';
            ColName.CopyTo(FqColName, ColFamName.Length + 1);

            FqColNameToDelete = new byte[ColFamName.Length + ColNameToDelete.Length + 1];
            ColFamName.CopyTo(FqColNameToDelete, 0);
            FqColNameToDelete[ColFamName.Length] = (byte)':';
            ColNameToDelete.CopyTo(FqColNameToDelete, ColFamName.Length + 1);
        }

        [Fact]
        [TestPriority(0)]
        public void CreatesTable()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseTableData tableData = mockRepository.Stub<IHBaseTableData>();
            IHBaseColumnFamilyData cfData = mockRepository.Stub<IHBaseColumnFamilyData>();

            using (mockRepository.Record())
            {
                SetupResult.For(tableData.Name).Return(TableName);
                SetupResult.For(cfData.Name).Return(ColFamName);
                SetupResult.For(cfData.MaxVersions).Return(3);
                SetupResult.For(tableData.ColumnFamilies).Return(new Dictionary<byte[], IHBaseColumnFamilyData> { { ColFamName, cfData } });
            }

            using (mockRepository.Playback())
            {
                using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
                {
                    connection.Open();

                    connection.CreateTable(tableData);
                }
            }
        }

        [Fact]
        [TestPriority(1)]
        public void GetTables()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();

                var table = connection.GetTables().SingleOrDefault(t => t.Name.SequenceEqual(TableName));

                Assert.NotNull(table);
                Assert.Equal(TableName, table.Name);
                Assert.Equal(1, table.ColumnFamilies.Count);
                Assert.Equal(ColFamName, table.ColumnFamilies.First().Value.Name);
            }
        }

        [Fact]
        [TestPriority(2)]
        public void GetColumnFamilies()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();

                var columnFamilies = connection.GetColumnFamilies(TableName);

                Assert.Equal(1, columnFamilies.Count);
                Assert.Equal(ColFamName, columnFamilies.First().Value.Name);
            }
        }

        [Fact]
        [TestPriority(3)]
        public void MutateRowsCanAdd()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseMutation mutation1 = mockRepository.Stub<IHBaseMutation>();
            IHBaseMutation mutation2 = mockRepository.Stub<IHBaseMutation>();
            IHBaseMutation mutation3 = mockRepository.Stub<IHBaseMutation>();
            IHBaseMutation mutation4 = mockRepository.Stub<IHBaseMutation>();
            
            using (mockRepository.Record())
            {
                SetupResult.For(mutation1.Row).Return(RowKey1);
                SetupResult.For(mutation1.Column).Return(FqColName);
                SetupResult.For(mutation1.IsDelete).Return(false);
                SetupResult.For(mutation1.Value).Return(CellVal);

                SetupResult.For(mutation2.Row).Return(RowKey2);
                SetupResult.For(mutation2.Column).Return(FqColName);
                SetupResult.For(mutation2.IsDelete).Return(false);
                SetupResult.For(mutation2.Value).Return(CellVal);

                SetupResult.For(mutation3.Row).Return(RowKey1);
                SetupResult.For(mutation3.Column).Return(FqColNameToDelete);
                SetupResult.For(mutation3.IsDelete).Return(false);
                SetupResult.For(mutation3.Value).Return(CellVal);

                SetupResult.For(mutation4.Row).Return(RowKey2);
                SetupResult.For(mutation4.Column).Return(FqColNameToDelete);
                SetupResult.For(mutation4.IsDelete).Return(false);
                SetupResult.For(mutation4.Value).Return(CellVal);
            }

            using (mockRepository.Playback())
            {
                using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
                {
                    connection.Open();
                    connection.MutateRows(TableName, new List<IHBaseMutation> { mutation1, mutation2, mutation3, mutation4 });
                }
            }
        }

        [Fact]
        [TestPriority(4)]
        public void CanGetRowExpectMultiColumns()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();
                var row = connection.GetRow(TableName, RowKey1);

                IList<byte[]> columns = row.Columns.Keys.ToList();

                Assert.Equal(2, row.Columns.Count);
                Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
                Assert.Contains(FqColNameToDelete, row.Columns.Keys, new ByteArrayEqualityComparer());
                Assert.Equal(1, row.Columns[columns[0]].Count);
                Assert.Equal(1, row.Columns[columns[1]].Count);
                Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
                Assert.Equal(CellVal, row.Columns[columns[1]][0].Value);
            }
        }

        [Fact]
        [TestPriority(4)]
        public void CanGetRowsAllColumnsRowHasMultiColumns()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();
                var rows = connection.GetRows(new List<byte[]> { RowKey1, RowKey2 }, TableName, null);

                Assert.Equal(2, rows.Count);

                foreach (var row in rows)
                {
                    IList<byte[]> columns = row.Columns.Keys.ToList();

                    Assert.Equal(2, row.Columns.Count);
                    Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Contains(FqColNameToDelete, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Equal(1, row.Columns[columns[0]].Count);
                    Assert.Equal(1, row.Columns[columns[1]].Count);
                    Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
                    Assert.Equal(CellVal, row.Columns[columns[1]][0].Value);   
                }
            }
        }

        [Fact]
        [TestPriority(4)]
        public void CanGetRowsSingleColumnRowHasMultiColumns()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();
                var rows = connection.GetRows(new List<byte[]> { RowKey1, RowKey2 }, TableName, new List<byte[]> { FqColName });

                Assert.Equal(2, rows.Count);

                foreach (var row in rows)
                {
                    IList<byte[]> columns = row.Columns.Keys.ToList();

                    Assert.Equal(1, row.Columns.Count);
                    Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Equal(1, row.Columns[columns[0]].Count);
                    Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
                }
            }
        }

        [Fact]
        [TestPriority(4)]
        public void CanOpenScannerWithStartRowAllColumns()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();

                var rows = connection.Scan(TableName, RowKey1, null);

                // Expect one or two rows because RowKey1 and RowKey2 are not intentionally in order.
                Assert.InRange(rows.Count, 1, 2);

                foreach (var row in rows)
                {
                    IList<byte[]> columns = row.Columns.Keys.ToList();

                    Assert.Equal(2, row.Columns.Count);
                    Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Contains(FqColNameToDelete, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Equal(1, row.Columns[columns[0]].Count);
                    Assert.Equal(1, row.Columns[columns[1]].Count);
                    Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
                    Assert.Equal(CellVal, row.Columns[columns[1]][0].Value);   
                }
            }
        }

        [Fact]
        [TestPriority(4)]
        public void CanOpenScannerWithStartRowAndMaxRowsAllColumns()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();

                var rows = connection.Scan(TableName, RowKey1, null, null, 1);

                Assert.Equal(rows.Count, 1);

                rows = connection.Scan(TableName, RowKey1, null, null, 0);

                Assert.Equal(rows.Count, 0);
            }
        }

        [Fact]
        [TestPriority(4)]
        public void CanOpenScannerWithStartRowAndSingleColumn()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();

                var rows = connection.Scan(TableName, RowKey1, new List<byte[]> { FqColName }, null, 10);

                // Expect one or two rows because RowKey1 and RowKey2 are not intentionally in order.
                Assert.InRange(rows.Count, 1, 2);

                foreach (var row in rows)
                {
                    IList<byte[]> columns = row.Columns.Keys.ToList();

                    Assert.Equal(1, row.Columns.Count);
                    Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Equal(1, row.Columns[columns[0]].Count);
                    Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
                }
            }
        }

        [Fact]
        [TestPriority(4)]
        public void CanOpenScannerWithStartRowAndStopRowAndAllColumns()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();

                string row1 = Encoding.UTF8.GetString(RowKey1);
                string row2 = Encoding.UTF8.GetString(RowKey2);

                byte[] start;
                byte[] stop;

                if (row1.CompareTo(row2) > 0)
                {
                    start = RowKey2;
                    stop = RowKey1;
                }
                else
                {
                    start = RowKey1;
                    stop = RowKey2;
                }

                var rows = connection.ScanWithStop(TableName, start, stop, null, null, 10);

                // The stop row is not included in a stop row scan
                Assert.Equal(1, rows.Count);

                foreach (var row in rows)
                {
                    IList<byte[]> columns = row.Columns.Keys.ToList();

                    Assert.Equal(2, row.Columns.Count);
                    Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Contains(FqColNameToDelete, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Equal(1, row.Columns[columns[0]].Count);
                    Assert.Equal(1, row.Columns[columns[1]].Count);
                    Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
                    Assert.Equal(CellVal, row.Columns[columns[1]][0].Value);   
                }
            }
        }

        [Fact]
        [TestPriority(4)]
        public void CanOpenScannerWithPrefixStartRowAndAllColumns()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();

                // All keys are prefixed with 'k'.
                byte[] startPrefix = new byte[] {(byte) 'k'};

                var rows = connection.ScanWithPrefix(TableName, startPrefix, null, 10);

                Assert.Equal(2, rows.Count);

                foreach (var row in rows)
                {
                    IList<byte[]> columns = row.Columns.Keys.ToList();

                    Assert.Equal(2, row.Columns.Count);
                    Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Contains(FqColNameToDelete, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Equal(1, row.Columns[columns[0]].Count);
                    Assert.Equal(1, row.Columns[columns[1]].Count);
                    Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
                    Assert.Equal(CellVal, row.Columns[columns[1]][0].Value);
                }
            }
        }

        [Fact]
        [TestPriority(4)]
        public void CanGetColumn()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();
                IList<IHBaseCellData> cellData = connection.GetColumn(TableName, RowKey1, FqColNameToDelete);

                Assert.Equal(1, cellData.Count);
                deleteTimestamp = cellData[0].Timestamp;
            }
        }

        [Fact]
        [TestPriority(5)]
        public void MutateRowsCanDelete()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseMutation mutation1 = mockRepository.Stub<IHBaseMutation>();
            IHBaseMutation mutation2 = mockRepository.Stub<IHBaseMutation>();

            using (mockRepository.Record())
            {
                SetupResult.For(mutation1.Row).Return(RowKey1);
                SetupResult.For(mutation1.Column).Return(FqColNameToDelete);
                SetupResult.For(mutation1.IsDelete).Return(true);
                SetupResult.For(mutation1.Value).Return(CellVal);

                SetupResult.For(mutation2.Row).Return(RowKey2);
                SetupResult.For(mutation2.Column).Return(FqColNameToDelete);
                SetupResult.For(mutation2.IsDelete).Return(true);
                SetupResult.For(mutation2.Value).Return(CellVal);
            }

            using (mockRepository.Playback())
            {
                using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
                {
                    connection.Open();
                    connection.MutateRows(TableName, new List<IHBaseMutation> { mutation1, mutation2 });
                }
            }
        }

        [Fact]
        [TestPriority(6)]
        public void CanGetRowRowHasSingleColumn()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();
                var row = connection.GetRow(TableName, RowKey1);

                IList<byte[]> columns = row.Columns.Keys.ToList();

                Assert.Equal(1, row.Columns.Count);
                Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
                Assert.Equal(1, row.Columns[columns[0]].Count);
                Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
            }
        }

        [Fact]
        [TestPriority(6)]
        public void CanGetRowsAllColumnsRowHasSingleColumn()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();
                var rows = connection.GetRows(new List<byte[]> { RowKey1, RowKey2 }, TableName, null);

                Assert.Equal(2, rows.Count);

                foreach (var row in rows)
                {
                    IList<byte[]> columns = row.Columns.Keys.ToList();

                    Assert.Equal(1, row.Columns.Count);
                    Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Equal(1, row.Columns[columns[0]].Count);
                    Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
                }
            }
        }

        [Fact]
        [TestPriority(6)]
        public void CanGetRowsSingleColumnRowHasSingleColumn()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();
                var rows = connection.GetRows(new List<byte[]> { RowKey1, RowKey2 }, TableName, new List<byte[]> { FqColName });

                Assert.Equal(2, rows.Count);

                foreach (var row in rows)
                {
                    IList<byte[]> columns = row.Columns.Keys.ToList();

                    Assert.Equal(1, row.Columns.Count);
                    Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
                    Assert.Equal(1, row.Columns[columns[0]].Count);
                    Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
                }
            }
        }

        ////[Fact]
        ////[TestPriority(6)]
        ////public void CanGetRowsTs()
        ////{
        ////    using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
        ////    {
        ////        connection.Open();
        ////        var rows = connection.GetRows(new List<byte[]> { RowKey1 }, TableName, new List<byte[]> { FqColNameToDelete }, deleteTimestamp);

        ////        Assert.Equal(1, rows.Count);
                
        ////        var row = rows[0];
                
        ////        IList<byte[]> columns = row.Columns.Keys.ToList();

        ////         Should contain two columns as we're getting an older version of the row
        ////        Assert.Equal(1, row.Columns.Count);
        ////        Assert.Contains(FqColName, row.Columns.Keys, new ByteArrayEqualityComparer());
        ////        Assert.Equal(2, row.Columns[columns[0]].Count);
        ////        Assert.Equal(CellVal, row.Columns[columns[0]][0].Value);
        ////        Assert.Equal(CellVal, row.Columns[columns[1]][0].Value);
        ////    }
        ////}

        [Fact]
        [TestPriority(7)]
        public void DisableTable()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();
                connection.DisableTable(TableName);

                var table = connection.GetTables().SingleOrDefault(t => t.Name.SequenceEqual(TableName));

                Assert.False(table.IsEnabled);
            } 
        }

        [Fact]
        [TestPriority(8)]
        public void DeletesTable()
        {
            using (HBaseThriftConnection connection = new HBaseThriftConnection(Host, Port))
            {
                connection.Open();
                connection.DeleteTable(TableName);

                var table = connection.GetTables().SingleOrDefault(t => t.Name.SequenceEqual(TableName));

                Assert.Null(table);
            }
        }
    }
}
