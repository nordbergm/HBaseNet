using Thrift.HBase;
using System;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseColumnFamilyData : IHBaseColumnFamilyData
    {
        public HBaseColumnFamilyData(ColumnDescriptor columnDescriptor)
        {
            // Remove ':' from name
            Name = new byte[columnDescriptor.Name.Length - 1];
            Array.Copy(columnDescriptor.Name, Name, Name.Length);

            ColumnDescriptor = columnDescriptor;
            MaxVersions = columnDescriptor.MaxVersions;
            BlockCacheEnabled = columnDescriptor.BlockCacheEnabled;
        }

        public HBaseColumnFamilyData(IHBaseColumnFamilyData other)
        {
            Name = other.Name;
            MaxVersions = other.MaxVersions;
            BlockCacheEnabled = other.BlockCacheEnabled;

            // Add ':' to name
            byte[] name = new byte[Name.Length + 1];
            Name.CopyTo(name, 0);
            name[name.Length - 1] = (byte) ':';

            ColumnDescriptor = new ColumnDescriptor
                                   {
                                       Name = name,
                                       MaxVersions = this.MaxVersions,
                                       BlockCacheEnabled = this.BlockCacheEnabled
                                   };
        }
        public ColumnDescriptor ColumnDescriptor { get; private set; }
        public byte[] Name { get; private set; }
        public int MaxVersions { get; private set; }
        public bool BlockCacheEnabled { get; private set; }
    }
}
