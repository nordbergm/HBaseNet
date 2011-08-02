using System;
using System.Collections.Generic;
using System.Linq;
using HBaseNet.Mutations;

namespace HBaseNet
{
    public class HBaseDatabase : IDisposable
    {
        public HBaseDatabase(IHBaseConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection", "A connection is required.");
            }

            if (!connection.IsOpen)
            {
                connection.Open();
            }

            Connection = connection;
        }

        internal IHBaseConnection Connection { get; set; }

        #region Table Operations

        public IList<HBaseTable> GetTables()
        {
            return Connection.GetTables().Select(t => new HBaseTable(t, this)).ToList();
        }

        public void CreateTable(HBaseTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            Connection.CreateTable(table);
        }

        public void DeleteTable(byte[] tableName)
        {
            Connection.DeleteTable(tableName);
        }

        public void EnableTable(byte[] tableName)
        {
            Connection.EnableTable(tableName);
        }

        public void DisableTable(byte[] tableName)
        {
            Connection.DisableTable(tableName);
        }

        public void MutateRows(byte[] tableName, IList<IHBaseMutation> mutations, long? timestamp = null)
        {
            Connection.MutateRows(tableName, mutations.Cast<Protocols.IHBaseMutation>().ToList(), timestamp);
        }

        #endregion


        public void Close()
        {
            Connection.Close();
        }

        public void Dispose()
        {
            this.Connection.Dispose();
        }
    }
}
