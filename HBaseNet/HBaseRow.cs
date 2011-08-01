using System.Collections.Generic;
using HBaseNet.Protocols;

namespace HBaseNet
{
    public class HBaseRow
    {
        public HBaseRow(IHBaseRowData rowData)
        {
            Key = rowData.Key;
            Columns = new Dictionary<byte[], IHBaseCell>(rowData.Columns.Count);

            foreach (var column in rowData.Columns)
            {
                Columns.Add(column.Key, new HBaseCell(column.Value));
            }
        }

        public byte[] Key { get; private set; }
        public IDictionary<byte[], IHBaseCell> Columns { get; private set; }
    }
}
