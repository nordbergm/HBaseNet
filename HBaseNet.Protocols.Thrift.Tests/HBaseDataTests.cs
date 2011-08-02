using System.Collections.Generic;
using Thrift.HBase;
using Xunit;
using System.Text;

namespace HBaseNet.Protocols.Thrift.Tests
{
    public class HBaseDataTests
    {
        [Fact]
        public void HBaseCellDataConvertsFromTCell()
        {
            TCell thriftCell = new TCell();
            thriftCell.Timestamp = 123;
            thriftCell.Value = Encoding.UTF8.GetBytes("cell");

            HBaseCellData cellData = new HBaseCellData(thriftCell);
            
            Assert.Equal(thriftCell.Timestamp, cellData.Timestamp);
            Assert.Equal(thriftCell.Value, cellData.Value);
        }

        [Fact]
        public void HBaseColumnFamilyDataConvertsFromColumnDescriptor()
        {
            byte[] columnName = Encoding.UTF8.GetBytes("cd");
            byte[] columnNameFromDb = Encoding.UTF8.GetBytes("cd:");

            ColumnDescriptor cd = new ColumnDescriptor
                                      {
                                          Name = columnNameFromDb,
                                          MaxVersions = 3,
                                          BlockCacheEnabled = true
                                      };
            HBaseColumnFamilyData cfData = new HBaseColumnFamilyData(cd);

            Assert.Equal(columnName, cfData.Name);
            Assert.Equal(cd.MaxVersions, cfData.MaxVersions);
            Assert.Equal(cd.BlockCacheEnabled, cfData.BlockCacheEnabled);
            Assert.Equal(cd, cfData.ColumnDescriptor);
        }

        [Fact]
        public void HBaseColumnFamilyDataConvertsFromHBaseColumnFamilyData()
        {
            byte[] columnNameInDb = Encoding.UTF8.GetBytes("cd:");

            ColumnDescriptor cd = new ColumnDescriptor
            {
                Name = columnNameInDb,
                MaxVersions = 3,
                BlockCacheEnabled = true
            };

            HBaseColumnFamilyData cf1 = new HBaseColumnFamilyData(cd);
            HBaseColumnFamilyData cf2 = new HBaseColumnFamilyData(cf1);

            Assert.Equal(cf1.Name, cf2.Name);
            Assert.Equal(cf1.MaxVersions, cf2.MaxVersions);
            Assert.Equal(cf1.BlockCacheEnabled, cf2.BlockCacheEnabled);
            
            Assert.Equal(cf1.ColumnDescriptor.Name, cf2.ColumnDescriptor.Name);
            Assert.Equal(cf1.ColumnDescriptor.MaxVersions, cf2.ColumnDescriptor.MaxVersions);
            Assert.Equal(cf1.ColumnDescriptor.BlockCacheEnabled, cf2.ColumnDescriptor.BlockCacheEnabled);
        }

        [Fact]
        public void HBaseRowDataConvertsFromTRowResult()
        {
            TCell cell = new TCell();
            cell.Timestamp = 123;
            cell.Value = Encoding.UTF8.GetBytes("cell");

            TRowResult rowResult = new TRowResult();
            rowResult.Row = Encoding.UTF8.GetBytes("r");
            rowResult.Columns = new Dictionary<byte[], TCell>();
            rowResult.Columns.Add(Encoding.UTF8.GetBytes("c"), cell);

            HBaseRowData rowData = new HBaseRowData(rowResult);
            Assert.Equal(rowResult.Row, rowData.Key);
            Assert.Equal(rowResult.Columns.Count, rowData.Columns.Count);
        }

        [Fact]
        public void HBaseRowDataConvertsFromListOfTRowResult()
        {
            byte[] column = Encoding.UTF8.GetBytes("c");
            byte[] row = Encoding.UTF8.GetBytes("r");

            TCell cell1 = new TCell();
            cell1.Timestamp = 123;
            cell1.Value = Encoding.UTF8.GetBytes("cell1");

            TCell cell2 = new TCell();
            cell2.Timestamp = 1234;
            cell2.Value = Encoding.UTF8.GetBytes("cell2");

            TRowResult rowResult1 = new TRowResult();
            rowResult1.Row = row;
            rowResult1.Columns = new Dictionary<byte[], TCell>();
            rowResult1.Columns.Add(column, cell1);

            TRowResult rowResult2 = new TRowResult();
            rowResult2.Row = row;
            rowResult2.Columns = new Dictionary<byte[], TCell>();
            rowResult2.Columns.Add(column, cell2);

            HBaseRowData rowData = new HBaseRowData(new List<TRowResult>() { rowResult1, rowResult2 });
            Assert.Equal(rowResult1.Row, rowData.Key);
            Assert.Equal(1, rowData.Columns.Count);
            Assert.Equal(2, rowData.Columns[column].Count);
        }
    }
}
