using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HBaseNet.Protocols;
using Rhino.Mocks;
using Xunit;

namespace HBaseNet.Tests
{
    public class HBaseRowTests
    {
        [Fact]
        public void HasColumns()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseRowData rowData = mockRepository.Stub<IHBaseRowData>();
            IHBaseCellData cellData = mockRepository.Stub<IHBaseCellData>();
            byte[] columnName = Encoding.UTF8.GetBytes("c");

            using (mockRepository.Record())
            {
                SetupResult.For(cellData.Values).Return(new Dictionary<long, byte[]>());
                SetupResult.For(rowData.Key).Return(Encoding.UTF8.GetBytes("k"));
                SetupResult.For(rowData.Columns).Return(new Dictionary<byte[], IHBaseCellData>() { { columnName, cellData } });
            }

            using (mockRepository.Playback())
            {
                HBaseRow row = new HBaseRow(rowData);
                Assert.Contains(columnName, row.Columns.Keys);
            }
        }

        [Fact]
        public void GetRowsRequiresRows()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();

            HBaseDatabase db = new HBaseDatabase(connection);
            HBaseTable table = new HBaseTable(Encoding.UTF8.GetBytes("stub"), db);

            Assert.Throws<ArgumentNullException>(() => table.GetRows(null));
            Assert.Throws<ArgumentException>(() => table.GetRows(new List<byte[]>()));
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
    }
}
