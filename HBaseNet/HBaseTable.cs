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
            return GetRows(new List<byte[]> {row}, columns, timestamp).FirstOrDefault();
        }

        public IList<HBaseRow> GetRows(IList<byte[]> rows, IList<byte[]> columns = null, long? timestamp = null)
        {
            if (rows == null)
            {
                throw new ArgumentNullException("rows");
            }

            if (rows.Count == 0)
            {
                throw new ArgumentException("Number of rows must be greater than 0.", "rows");
            }

            return Database.Connection.GetRows(rows, this.Name, columns, timestamp)
                .Select(r => new HBaseRow(r))
                .ToList();
        }

        #endregion

        public IList<HBaseRow> Scan(byte[] startRow, IList<byte[]> columns, long? timestamp = null, int? numRows = null)
        {
            if (startRow == null || startRow.Length == 0)
            {
                throw new ArgumentNullException("startRow");
            }

            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }

            if (columns.Count == 0)
            {
                throw new ArgumentException("Number of columns must be greater than 0.", "columns");
            }

            return Database.Connection.Scan(this.Name, startRow, columns, timestamp, numRows)
                .Select(r => new HBaseRow(r))
                .ToList();
        }

        public IList<HBaseRow> ScanWithStop(byte[] startRow, byte[] stopRow, IList<byte[]> columns, long? timestamp = null, int? numRows = null)
        {
            if (startRow == null || startRow.Length == 0)
            {
                throw new ArgumentNullException("startRow");
            }

            if (stopRow == null || stopRow.Length == 0)
            {
                throw new ArgumentNullException("stopRow");
            }

            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }

            if (columns.Count == 0)
            {
                throw new ArgumentException("Number of columns must be greater than 0.", "columns");
            }

            return Database.Connection.ScanWithStop(this.Name, startRow, stopRow, columns, timestamp, numRows)
                .Select(r => new HBaseRow(r))
                .ToList();
        }

        public IList<HBaseRow> ScanWithPrefix(byte[] startRowPrefix, IList<byte[]> columns, int? numRows = null)
        {
            if (startRowPrefix == null || startRowPrefix.Length == 0)
            {
                throw new ArgumentNullException("startRowPrefix");
            }

            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }

            if (columns.Count == 0)
            {
                throw new ArgumentException("Number of columns must be greater than 0.", "columns");
            }

            return Database.Connection.ScanWithPrefix(this.Name, startRowPrefix, columns, numRows)
                .Select(r => new HBaseRow(r))
                .ToList();
        }
    }
}
