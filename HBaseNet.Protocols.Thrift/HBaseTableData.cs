using System.Collections.Generic;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseTableData : IHBaseTableData
    {
        public HBaseTableData(byte[] name, IDictionary<byte[], IHBaseColumnFamilyData> columnFamilies)
        {
            Name = name;
            ColumnFamilies = columnFamilies;
        }

        public byte[] Name { get; private set; }
        public IDictionary<byte[], IHBaseColumnFamilyData> ColumnFamilies { get; private set; }
    }
}
