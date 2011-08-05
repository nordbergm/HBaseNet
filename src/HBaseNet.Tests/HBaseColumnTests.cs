using System.Collections.Generic;
using System.Text;
using HBaseNet.Protocols;
using Rhino.Mocks;
using Xunit;

namespace HBaseNet.Tests
{
    public class HBaseColumnTests
    {
        [Fact]
        public void RegardsFirstValueAsLatest()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();
            IHBaseCellData cellData1 = mockRepository.Stub<IHBaseCellData>();
            IHBaseCellData cellData2 = mockRepository.Stub<IHBaseCellData>();

            HBaseRow row = new HBaseRow(Encoding.UTF8.GetBytes("r"),
                                        new HBaseTable(Encoding.UTF8.GetBytes("t"), new HBaseDatabase(connection)));

            using (mockRepository.Record())
            {
                SetupResult.For(cellData1.Timestamp).Return(1);
                SetupResult.For(cellData1.Value).Return(Encoding.UTF8.GetBytes("old"));
                SetupResult.For(cellData2.Timestamp).Return(2);
                SetupResult.For(cellData2.Value).Return(Encoding.UTF8.GetBytes("new"));
            }

            using (mockRepository.Playback())
            {
                HBaseColumn column = new HBaseColumn(Encoding.UTF8.GetBytes("c"), row, new List<IHBaseCellData> { cellData2, cellData1 });
                Assert.Equal(Encoding.UTF8.GetBytes("new"), column.Value);
            }
        }

        [Fact]
        public void HasVersions()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();
            IHBaseCellData cellData1 = mockRepository.Stub<IHBaseCellData>();
            IHBaseCellData cellData2 = mockRepository.Stub<IHBaseCellData>();

            HBaseRow row = new HBaseRow(Encoding.UTF8.GetBytes("r"),
                                        new HBaseTable(Encoding.UTF8.GetBytes("t"), new HBaseDatabase(connection)));

            using (mockRepository.Record())
            {
                SetupResult.For(cellData1.Timestamp).Return(1);
                SetupResult.For(cellData1.Value).Return(Encoding.UTF8.GetBytes("old"));
                SetupResult.For(cellData2.Timestamp).Return(2);
                SetupResult.For(cellData2.Value).Return(Encoding.UTF8.GetBytes("new"));
            }

            using (mockRepository.Playback())
            {
                HBaseColumn column = new HBaseColumn(Encoding.UTF8.GetBytes("c"), row, new List<IHBaseCellData> { cellData2, cellData1 });
                Assert.Equal(2, column.Cells.Count);
            }
        }
    }
}
