using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using Xunit;
using HBaseNet.Protocols;
using IHBaseMutation = HBaseNet.Mutations.IHBaseMutation;

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

        [Fact]
        public void CreatesTable()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.StrictMock<IHBaseConnection>();
            IHBaseTableData tableData = mockRepository.Stub<IHBaseTableData>();
            IHBaseColumnFamilyData columnFamilyData = mockRepository.Stub<IHBaseColumnFamilyData>();

            using (mockRepository.Record())
            {
                SetupResult.For(columnFamilyData.Name).Return(Encoding.UTF8.GetBytes("cf1"));

                SetupResult.For(tableData.Name).Return(Encoding.UTF8.GetBytes("t"));
                SetupResult.For(tableData.ColumnFamilies).Return(new Dictionary<byte[], IHBaseColumnFamilyData> { { Encoding.UTF8.GetBytes("cf1"), columnFamilyData } });

                SetupResult.For(connection.IsOpen).Return(true);

                Expect.Call(() => connection.CreateTable(tableData)).IgnoreArguments().WhenCalled((mi) =>
                {
                    IHBaseTableData td = (IHBaseTableData)mi.Arguments[0];

                    Assert.Equal(tableData.Name, td.Name);
                    Assert.Equal(tableData.ColumnFamilies.Count, td.ColumnFamilies.Count);
                    Assert.Equal(tableData.ColumnFamilies.First().Value.Name, td.ColumnFamilies.First().Value.Name);
                });
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                HBaseTable table = new HBaseTable(Encoding.UTF8.GetBytes("t"), db);
                HBaseColumnFamily cf = new HBaseColumnFamily(Encoding.UTF8.GetBytes("cf1"), table);
                table.ColumnFamilies.Add(cf.Name, cf);

                db.CreateTable(table);
            }
        }

        [Fact]
        public void DeletesTable()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.StrictMock<IHBaseConnection>();
            IHBaseTableData tableData = mockRepository.Stub<IHBaseTableData>();

            byte[] tableName = Encoding.UTF8.GetBytes("t");

            using (mockRepository.Record())
            {
                SetupResult.For(connection.IsOpen).Return(true);
                SetupResult.For(tableData.Name).Return(tableName);
                Expect.Call(() => connection.DeleteTable(tableName));
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                db.DeleteTable(tableName);
            }
        }

        [Fact]
        public void DisablesTable()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.StrictMock<IHBaseConnection>();
            IHBaseTableData tableData = mockRepository.Stub<IHBaseTableData>();

            byte[] tableName = Encoding.UTF8.GetBytes("t");

            using (mockRepository.Record())
            {
                SetupResult.For(connection.IsOpen).Return(true);
                SetupResult.For(tableData.Name).Return(tableName);
                Expect.Call(() => connection.DisableTable(tableName));
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                db.DisableTable(tableName);
            }
        }

        [Fact]
        public void EnablesTable()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.StrictMock<IHBaseConnection>();
            IHBaseTableData tableData = mockRepository.Stub<IHBaseTableData>();

            byte[] tableName = Encoding.UTF8.GetBytes("t");

            using (mockRepository.Record())
            {
                SetupResult.For(connection.IsOpen).Return(true);
                SetupResult.For(tableData.Name).Return(tableName);
                Expect.Call(() => connection.EnableTable(tableName));
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                db.EnableTable(tableName);
            }
        }

        [Fact]
        public void MutatesRows()
        {
            MockRepository mockRepository = new MockRepository();
            IHBaseConnection connection = mockRepository.StrictMock<IHBaseConnection>();
            Mutations.IHBaseMutation mutation = mockRepository.Stub<Mutations.IHBaseMutation>();

            byte[] tableName = Encoding.UTF8.GetBytes("t");
            byte[] row = Encoding.UTF8.GetBytes("r");
            byte[] column = Encoding.UTF8.GetBytes("c");

            mutation.Row = row;
            mutation.Column = column;

            using (mockRepository.Record())
            {
                SetupResult.For(connection.IsOpen).Return(true);
                
                Expect.Call(() => connection.MutateRows(null, null, null))
                    .IgnoreArguments()
                    .WhenCalled((mi) =>
                                    {
                                        byte[] tableArg = (byte[])mi.Arguments[0];
                                        IList<Protocols.IHBaseMutation> mutationArg = (IList<Protocols.IHBaseMutation>)mi.Arguments[1];
                                        long? timestampArg = (long?)mi.Arguments[2];

                                        Assert.Equal(tableName, tableArg);
                                        Assert.Equal(1, mutationArg.Count);
                                        Assert.Equal(123, timestampArg);
                                    });
            }

            using (mockRepository.Playback())
            {
                HBaseDatabase db = new HBaseDatabase(connection);
                db.MutateRows(tableName, new List<IHBaseMutation> { mutation }, 123);
            }
        }
    }
}
