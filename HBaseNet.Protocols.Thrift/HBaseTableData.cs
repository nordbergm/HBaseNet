using System.Collections.Generic;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseTableData : IHBaseTableData
    {
        public HBaseTableData(byte[] name, bool isEnabled, IDictionary<byte[], IHBaseColumnFamilyData> columnFamilies)
        {
            Name = name;
            IsEnabled = isEnabled;
            ColumnFamilies = columnFamilies;
        }

        public byte[] Name { get; private set; }
        public bool IsEnabled { get; private set; }
        public IDictionary<byte[], IHBaseColumnFamilyData> ColumnFamilies { get; private set; }
    }
}
