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
    }
}
