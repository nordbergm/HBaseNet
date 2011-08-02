using System;
using HBaseNet.Protocols;

namespace HBaseNet
{
    public class HBaseColumnFamily : HBaseEntityBase<IHBaseColumnFamilyData>, IHBaseColumnFamilyData
    {
        public HBaseColumnFamily(IHBaseColumnFamilyData cfd, HBaseTable table) : base(cfd)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table", "A column family must belong to a table.");
            }

            Table = table;
        }

        public HBaseColumnFamily(byte[] name, HBaseTable table)
        {
            if (name == null || name.Length == 0)
            {
                throw new ArgumentNullException("name", "A column family must have a name.");
            }

            if (table == null)
            {
                throw new ArgumentNullException("table", "A column family must belong to a table.");
            }

            Database = table.Database;
            Name = name;
            Table = table;
            MaxVersions = 3;
            BlockCacheEnabled = false;
        }

        public byte[] Name { get; private set; }
        public HBaseTable Table { get; private set; }
        public int MaxVersions { get; set; }
        public bool BlockCacheEnabled { get; set; }

        #region Overrides of HBaseEntityBase<IHBaseColumnFamilyData>

        protected override IHBaseColumnFamilyData Read()
        {
            var cf = Database.Connection.GetColumnFamilies(Table.Name);

            if (cf.ContainsKey(Name))
            {
                return cf[Name];
            }

            return null;
        }

        protected override void Load(IHBaseColumnFamilyData data)
        {
            Name = data.Name;
            MaxVersions = data.MaxVersions;
            BlockCacheEnabled = data.BlockCacheEnabled;
        }

        #endregion

        #region Explicit Implementation of IHBaseColumnFamilyData

        byte[] IHBaseColumnFamilyData.Name
        {
            get { return this.Name; }
        }

        int IHBaseColumnFamilyData.MaxVersions
        {
            get { return this.MaxVersions; }
        }

        bool IHBaseColumnFamilyData.BlockCacheEnabled
        {
            get { return this.BlockCacheEnabled; }
        }

        #endregion
    }
}
