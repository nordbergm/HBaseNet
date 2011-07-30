using System;

namespace HBaseNet
{
    public class HBaseTable : IHBaseTable
    {
        public HBaseTable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "A table must have a name.");
            }

            Name = name;
        }

        #region Implementation of IHBaseTable

        public string Name { get; private set; }

        public HBaseRow GetRow(string row)
        {
            throw new NotImplementedException();
        }

        public IHBaseTableScan Scan()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
