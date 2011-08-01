using System;
using System.Collections.Generic;
using System.Linq;

namespace HBaseNet
{
    public class HBaseTable : IHBaseTable
    {
        public HBaseTable(byte[] name, HBaseDatabase database)
        {
            if (name == null || name.Length == 0)
            {
                throw new ArgumentNullException("name", "A table must have a name.");
            }

            Name = name;
            Database = database;
        }

        public HBaseDatabase Database { get; private set; }

        #region Implementation of IHBaseTable

        public byte[] Name { get; private set; }

        public HBaseRow GetRow(byte[] row, IList<byte[]> columns = null, long? timestamp = null)
        {
            return GetRows(new List<byte[]>() {row}, columns, timestamp).FirstOrDefault();
        }

        public IList<HBaseRow> GetRows(IList<byte[]> rows, IList<byte[]> columns = null, long? timestamp = null)
        {
            if (rows == null)
            {
                throw new ArgumentNullException("rows");
            }

            if (rows.Count == 0)
            {
                throw new ArgumentException("rows");
            }

            return Database.Connection.GetRows(rows, this.Name, columns, timestamp).Select(r => new HBaseRow(r)).ToList();
        }

        public IHBaseTableScan Scan()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
