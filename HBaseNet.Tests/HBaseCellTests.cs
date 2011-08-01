using System.Collections.Generic;
using System.Text;
using HBaseNet.Protocols;
using Rhino.Mocks;
using Xunit;

namespace HBaseNet.Tests
{
    public class HBaseCellTests
    {
        [Fact]
        public void HasLatestVersionsValue()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseCellData cellData = mockRepository.Stub<IHBaseCellData>();

            using (mockRepository.Record())
            {
                SetupResult.For(cellData.Values).Return(new Dictionary<long, byte[]> {{ 1, Encoding.UTF8.GetBytes("old") }, { 2, Encoding.UTF8.GetBytes("new") }});
            }

            using (mockRepository.Playback())
            {
                HBaseCell cell = new HBaseCell(cellData);
                Assert.Equal(Encoding.UTF8.GetBytes("new"), cell.Value);
            }
        }

        [Fact]
        public void HasVersions()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseCellData cellData = mockRepository.Stub<IHBaseCellData>();

            using (mockRepository.Record())
            {
                SetupResult.For(cellData.Values).Return(new Dictionary<long, byte[]> { { 1, Encoding.UTF8.GetBytes("old") }, { 2, Encoding.UTF8.GetBytes("new") } });
            }

            using (mockRepository.Playback())
            {
                HBaseCell cell = new HBaseCell(cellData);
                Assert.Equal(2, cell.Versions.Count);
            }
        }
    }
}
