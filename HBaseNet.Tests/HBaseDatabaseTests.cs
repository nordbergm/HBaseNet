using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using Xunit;
using HBaseNet.Protocols;

namespace HBaseNet.Tests
{
    public class HBaseDatabaseTests
    {
        [Fact]
        public void OpensClosedConnectionWhenConstructed()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.StrictMock<IHBaseConnection>();

            using (mockRepository.Record())
            {
                SetupResult.For(connection.IsOpen).Return(false);
                Expect.Call(connection.Open).Repeat.Once();
            }

            using (mockRepository.Playback())
            {
                new HBaseDatabase(connection);
            }
        }

        [Fact]
        public void DoesNotOpenOpenedConnectionWhenConstructed()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.StrictMock<IHBaseConnection>();

            using (mockRepository.Record())
            {
                SetupResult.For(connection.IsOpen).Return(true);
                DoNotExpect.Call(connection.Open);
            }

            using (mockRepository.Playback())
            {
                new HBaseDatabase(connection);
            }
        }

        [Fact]
        public void ClosesConnectionWhenClosed()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.StrictMock<IHBaseConnection>();

            using (mockRepository.Record())
            {
                SetupResult.For(connection.IsOpen).Return(true);
                Expect.Call(connection.Close);
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                db.Close();
            }
        }

        [Fact]
        public void DisposesConnectionWhenDisposed()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.StrictMock<IHBaseConnection>();

            using (mockRepository.Record())
            {
                SetupResult.For(connection.IsOpen).Return(true);
                Expect.Call(connection.Dispose);
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                db.Dispose();
            }
        }

        [Fact]
        public void ReturnsTables()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();

            IHBaseTableData table1 = mockRepository.Stub<IHBaseTableData>();
            IHBaseTableData table2 = mockRepository.Stub<IHBaseTableData>();
            IHBaseColumnFamilyData cf1 = mockRepository.Stub<IHBaseColumnFamilyData>();
            IHBaseColumnFamilyData cf2 = mockRepository.Stub<IHBaseColumnFamilyData>();

            using (mockRepository.Record())
            {
                SetupResult.For(table1.Name).Return(Encoding.UTF8.GetBytes("table1"));
                SetupResult.For(table2.Name).Return(Encoding.UTF8.GetBytes("table2"));

                SetupResult.For(cf1.Name).Return(Encoding.UTF8.GetBytes("cf1"));
                SetupResult.For(cf2.Name).Return(Encoding.UTF8.GetBytes("cf2"));

                SetupResult.For(table1.ColumnFamilies).Return(new Dictionary<byte[], IHBaseColumnFamilyData> { { Encoding.UTF8.GetBytes("cf1"), cf1 } });
                SetupResult.For(table2.ColumnFamilies).Return(new Dictionary<byte[], IHBaseColumnFamilyData> { { Encoding.UTF8.GetBytes("cf2"), cf2 } });

                SetupResult.For(connection.GetTables()).Return(new List<IHBaseTableData> { table1, table2 });
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                var tables = db.GetTables();

                Assert.Equal(2, tables.Count);
                Assert.Equal(Encoding.UTF8.GetBytes("table1"), tables[0].Name);
                Assert.Equal(Encoding.UTF8.GetBytes("table2"), tables[1].Name);
                Assert.Equal(1, tables[0].ColumnFamilies.Keys.Count);
                Assert.Equal(1, tables[1].ColumnFamilies.Keys.Count);
                Assert.Equal(Encoding.UTF8.GetBytes("cf1"), tables[0].ColumnFamilies.Values.Single().Name);
                Assert.Equal(Encoding.UTF8.GetBytes("cf2"), tables[1].ColumnFamilies.Values.Single().Name);
            }
        }
    }
}
