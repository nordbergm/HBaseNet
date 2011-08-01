using System;
using System.Collections.Generic;
using HBaseNet.Protocols;

namespace HBaseNet
{
    public class HBaseRow : HBaseEntityBase<IHBaseRowData>
    {
        public HBaseRow(byte[] key, HBaseTable table)
        {
            if (key == null || key.Length == 0)
            {
                throw new ArgumentNullException("key", "A row must have a key.");
            }

            if (table == null)
            {
                throw new ArgumentNullException("table", "A row must belong to a table.");
            }

            Key = key;
            Table = table;
        }

        public HBaseRow(IHBaseRowData rowData, HBaseTable table) : base(rowData)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            Table = table;
        }

        public byte[] Key { get; private set; }
        public IDictionary<byte[], HBaseColumn> Columns { get; private set; }
        public HBaseTable Table { get; private set; }

        #region Overrides of HBaseEntityBase<IHBaseRowData>

        protected override IHBaseRowData Read()
        {
            return this.Table.Database.Connection.GetRow(this.Table.Name, this.Key);
        }

        protected override void Load(IHBaseRowData data)
        {
            Key = data.Key;
            Columns = new Dictionary<byte[], HBaseColumn>(data.Columns.Count);

            foreach (var column in data.Columns)
            {
                Columns.Add(column.Key, new HBaseColumn(column.Key, this, column.Value));
            }
        }

        #endregion
    }
}
