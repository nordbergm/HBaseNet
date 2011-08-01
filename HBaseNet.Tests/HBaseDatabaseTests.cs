using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using Xunit;

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
                HBaseDatabase db = new HBaseDatabase(connection);
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
                HBaseDatabase db = new HBaseDatabase(connection);
            }
        }

        [Fact]
        public void ClosesConnectionWhenClosed()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();
            HBaseDatabase db = new HBaseDatabase(connection);

            using (mockRepository.Record())
            {
                Expect.Call(connection.Close).Repeat.Once();
            }

            using (mockRepository.Playback())
            {
                db.Close();
            }
        }

        [Fact]
        public void DisposesConnectionWhenDisposed()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.Stub<IHBaseConnection>();

            using (mockRepository.Record())
            {
                Expect.Call(connection.Dispose).Repeat.Once();
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
            IList<byte[]> tableNames = new List<byte[]>() { Encoding.UTF8.GetBytes("table1"), Encoding.UTF8.GetBytes("table2") };

            using (mockRepository.Record())
            {
                SetupResult.For(connection.GetTables()).Return(tableNames);
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                var tables = db.GetTables();

                foreach (var tableName in tableNames)
                {
                    Assert.Contains(tableName, tables.Select(t => t.Name).ToList());
                }
            }
        }
    }
}
