using System.Collections.Generic;
using System.Text;
using HBaseNet.Protocols;
using Rhino.Mocks;
using Xunit;

namespace HBaseNet.Tests
{
    public class HBaseRowTests
    {
        [Fact]
        public void LoadsOnLoad()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();
            IHBaseRowData rowData = mockRepository.Stub<IHBaseRowData>();
            IHBaseCellData cellData = mockRepository.Stub<IHBaseCellData>();

            byte[] tableName = Encoding.UTF8.GetBytes("t");
            byte[] rowKey = Encoding.UTF8.GetBytes("r");
            byte[] columnName = Encoding.UTF8.GetBytes("c");

            using (mockRepository.Record())
            {
                SetupResult.For(rowData.Key).Return(rowKey);
                SetupResult.For(rowData.Columns).Return(new Dictionary<byte[], IList<IHBaseCellData>> { { columnName, new List<IHBaseCellData> { cellData } } });
                SetupResult.For(connection.GetRow(tableName, rowKey)).Return(rowData);
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                HBaseRow row = new HBaseRow(Encoding.UTF8.GetBytes("r"), new HBaseTable(Encoding.UTF8.GetBytes("t"), db));
                row.Load();

                Assert.Equal(1, row.Columns.Keys.Count);
            }
        }

        [Fact]
        public void HasColumns()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();
            IHBaseRowData rowData = mockRepository.Stub<IHBaseRowData>();
            HBaseDatabase db = new HBaseDatabase(connection);

            byte[] columnName = Encoding.UTF8.GetBytes("c");

            using (mockRepository.Record())
            {
                SetupResult.For(rowData.Key).Return(Encoding.UTF8.GetBytes("k"));
                SetupResult.For(rowData.Columns).Return(new Dictionary<byte[], IList<IHBaseCellData>>() { { columnName, new List<IHBaseCellData>() }});
            }

            using (mockRepository.Playback())
            {
                HBaseRow row = new HBaseRow(rowData, new HBaseTable(Encoding.UTF8.GetBytes("t"), db));
                Assert.Contains(columnName, row.Columns.Keys);
            }
        }
    }
}
