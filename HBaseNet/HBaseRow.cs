using System.Collections.Generic;

namespace HBaseNet
{
    public class HBaseRow
    {
        public HBaseRow(IHBaseRowData rowData)
        {
            Key = rowData.Key;
            Columns = new Dictionary<string, IHBaseCell>(rowData.Columns.Count);

            foreach (var column in rowData.Columns)
            {
                Columns.Add(column.Key, new HBaseCell(column.Value));
            }
        }

        public string Key { get; private set; }
        public IDictionary<string, IHBaseCell> Columns { get; private set; }
    }
}
