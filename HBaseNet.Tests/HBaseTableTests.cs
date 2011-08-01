using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HBaseNet.Protocols;
using Rhino.Mocks;
using Xunit;

namespace HBaseNet.Tests
{
    public class HBaseTableTests
    {
        [Fact]
        public void GetRowsRequiresRows()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();

            HBaseDatabase db = new HBaseDatabase(connection);
            HBaseTable table = new HBaseTable(Encoding.UTF8.GetBytes("stub"), db);

            Assert.Equal("rows", Assert.Throws<ArgumentNullException>(() => table.GetRows(null)).ParamName);
            Assert.Equal("rows", Assert.Throws<ArgumentException>(() => table.GetRows(new List<byte[]>())).ParamName);
        }

        [Fact]
        public void ReturnsRows()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();
            IList<IHBaseRowData> rowData = new List<IHBaseRowData>()
                                               {
                                                   mockRepository.Stub<IHBaseRowData>()
                                               };
            byte[] rowName = Encoding.UTF8.GetBytes("r");
            byte[] columnName = Encoding.UTF8.GetBytes("r");
            byte[] tableName = Encoding.UTF8.GetBytes("t");

            using (mockRepository.Record())
            {
                SetupResult.For(
                    connection
                        .GetRows(
                            new List<byte[]> { rowName },
                            tableName,
                            new List<byte[]> { columnName },
                            123))
                        .Return(rowData);
                SetupResult.For(rowData[0].Columns).Return(new Dictionary<byte[], IHBaseCellData>());
                SetupResult.For(rowData[0].Key).Return(rowName);
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                HBaseTable table = new HBaseTable(tableName, db);

                var rows = table.GetRows(
                            new List<byte[]> { rowName },
                            new List<byte[]> { columnName },
                            123);

                Assert.Contains(rowName, rows.Select(r => r.Key).ToList());
            }
        }

        [Fact]
        public void ScanRequiresColumns()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();
            HBaseDatabase db = new HBaseDatabase(connection);
            HBaseTable table = new HBaseTable(Encoding.UTF8.GetBytes("t"), db);

            Assert.Equal("columns", Assert.Throws<ArgumentNullException>(() => table.Scan(new byte[1], null)).ParamName);
            Assert.Equal("columns", Assert.Throws<ArgumentException>(() => table.Scan(new byte[1], new List<byte[]>())).ParamName);

            Assert.Equal("columns", Assert.Throws<ArgumentNullException>(() => table.ScanWithStop(new byte[1], new byte[1], null)).ParamName);
            Assert.Equal("columns", Assert.Throws<ArgumentException>(() => table.ScanWithStop(new byte[1], new byte[1], new List<byte[]>())).ParamName);

            Assert.Equal("columns", Assert.Throws<ArgumentNullException>(() => table.ScanWithPrefix(new byte[1], null)).ParamName);
            Assert.Equal("columns", Assert.Throws<ArgumentException>(() => table.ScanWithPrefix(new byte[1], new List<byte[]>())).ParamName);
        }

        [Fact]
        public void ScanRequiresStartRow()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();
            HBaseDatabase db = new HBaseDatabase(connection);
            HBaseTable table = new HBaseTable(Encoding.UTF8.GetBytes("t"), db);

            Assert.Equal("startRow", Assert.Throws<ArgumentNullException>(() => table.Scan(null, new List<byte[]> { new byte[1] })).ParamName);
            Assert.Equal("startRow", Assert.Throws<ArgumentNullException>(() => table.Scan(new byte[0], new List<byte[]> { new byte[1] })).ParamName);

            Assert.Equal("startRow", Assert.Throws<ArgumentNullException>(() => table.ScanWithStop(null, new byte[1], new List<byte[]> { new byte[1] })).ParamName);
            Assert.Equal("startRow", Assert.Throws<ArgumentNullException>(() => table.ScanWithStop(new byte[0], new byte[1], new List<byte[]> { new byte[1] })).ParamName);

            Assert.Equal("startRowPrefix", Assert.Throws<ArgumentNullException>(() => table.ScanWithPrefix(null, new List<byte[]> { new byte[1] })).ParamName);
            Assert.Equal("startRowPrefix", Assert.Throws<ArgumentNullException>(() => table.ScanWithPrefix(new byte[0], new List<byte[]> { new byte[1] })).ParamName);
        }

        [Fact]
        public void ScanRequiresStopRow()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();
            HBaseDatabase db = new HBaseDatabase(connection);
            HBaseTable table = new HBaseTable(Encoding.UTF8.GetBytes("t"), db);

            Assert.Equal("stopRow", Assert.Throws<ArgumentNullException>(() => table.ScanWithStop(new byte[1], null, new List<byte[]> { new byte[1] })).ParamName);
            Assert.Equal("stopRow", Assert.Throws<ArgumentNullException>(() => table.ScanWithStop(new byte[1], new byte[0], new List<byte[]> { new byte[1] })).ParamName);
        }

        [Fact]
        public void ScanReturnsRows()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();
            
            IHBaseRowData rowData1 = mockRepository.Stub<IHBaseRowData>();
            IHBaseRowData rowData2 = mockRepository.Stub<IHBaseRowData>();
            IHBaseRowData rowData3 = mockRepository.Stub<IHBaseRowData>();

            IHBaseCellData cellData = mockRepository.Stub<IHBaseCellData>();

            HBaseDatabase db = new HBaseDatabase(connection);
            HBaseTable table = new HBaseTable(Encoding.UTF8.GetBytes("t"), db);

            byte[] startRow1 = Encoding.UTF8.GetBytes("start1");
            byte[] startRow2 = Encoding.UTF8.GetBytes("start1");
            byte[] startRow3 = Encoding.UTF8.GetBytes("start1");
            byte[] stopRow = Encoding.UTF8.GetBytes("stop");
            IList<byte[]> columns = new List<byte[]> { Encoding.UTF8.GetBytes("column") };
            long? timestamp = 123;
            int? numRows = 400;

            using (mockRepository.Record())
            {
                SetupResult.For(cellData.Values).Return(new Dictionary<long, byte[]>());
                SetupResult.For(rowData1.Key).Return(startRow1);
                SetupResult.For(rowData1.Columns).Return(columns.ToDictionary(c => c, c => cellData));
                
                SetupResult.For(rowData2.Key).Return(startRow2);
                SetupResult.For(rowData2.Columns).Return(columns.ToDictionary(c => c, c => cellData));
                
                SetupResult.For(rowData3.Key).Return(startRow3);
                SetupResult.For(rowData3.Columns).Return(columns.ToDictionary(c => c, c => cellData));
                
                SetupResult.For(connection.Scan(table.Name, startRow1, columns, timestamp, numRows)).Return(new List<IHBaseRowData> { rowData1 });
                SetupResult.For(connection.ScanWithStop(table.Name, startRow2, stopRow, columns, timestamp, numRows)).Return(new List<IHBaseRowData> { rowData2 });
                SetupResult.For(connection.ScanWithPrefix(table.Name, startRow3, columns, numRows)).Return(new List<IHBaseRowData> { rowData3 });
            }

            using (mockRepository.Playback())
            {
                var rows = table.Scan(startRow1, columns, timestamp, numRows);
                Assert.Contains(startRow1, rows.Select(r => r.Key));

                rows = table.ScanWithStop(startRow2, stopRow, columns, timestamp, numRows);
                Assert.Contains(startRow2, rows.Select(r => r.Key));

                rows = table.ScanWithPrefix(startRow3, columns, numRows);
                Assert.Contains(startRow3, rows.Select(r => r.Key));
            }
        }
    }
}
