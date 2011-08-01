using Thrift.HBase;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseColumnFamilyData : IHBaseColumnFamilyData
    {
        public HBaseColumnFamilyData(ColumnDescriptor columnDescriptor)
        {
            Name = columnDescriptor.Name;
            MaxVersions = columnDescriptor.MaxVersions;
            BlockCacheEnabled = columnDescriptor.BlockCacheEnabled;
        }

        public byte[] Name { get; private set; }
        public int MaxVersions { get; private set; }
        public bool BlockCacheEnabled { get; private set; }
    }
}
