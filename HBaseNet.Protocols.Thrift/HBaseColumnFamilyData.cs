using Thrift.HBase;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseColumnFamilyData : IHBaseColumnFamilyData
    {
        public HBaseColumnFamilyData(ColumnDescriptor columnDescriptor)
        {
            ColumnDescriptor = columnDescriptor;
            Name = columnDescriptor.Name;
            MaxVersions = columnDescriptor.MaxVersions;
            BlockCacheEnabled = columnDescriptor.BlockCacheEnabled;
        }

        public HBaseColumnFamilyData(IHBaseColumnFamilyData other)
        {
            Name = other.Name;
            MaxVersions = other.MaxVersions;
            BlockCacheEnabled = other.BlockCacheEnabled;
            ColumnDescriptor = new ColumnDescriptor();
            {
                Name = this.Name;
                MaxVersions = this.MaxVersions;
                BlockCacheEnabled = this.BlockCacheEnabled;
            }
        }
        public ColumnDescriptor ColumnDescriptor { get; private set; }
        public byte[] Name { get; private set; }
        public int MaxVersions { get; private set; }
        public bool BlockCacheEnabled { get; private set; }
    }
}
