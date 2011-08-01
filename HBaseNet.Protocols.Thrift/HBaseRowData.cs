using System.Collections.Generic;
using Thrift.HBase;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseRowData : IHBaseRowData
    {
        public HBaseRowData(TRowResult rowResult)
        {
            Key = rowResult.Row;
            Columns = new Dictionary<byte[], IHBaseCellData>();

            foreach (var column in rowResult.Columns)
            {
                if (Columns.ContainsKey(column.Key))
                {
                    Columns[column.Key].Values.Add(column.Value.Timestamp, column.Value.Value);
                }
                else
                {
                    Columns.Add(column.Key, new HBaseCellData(column.Value));   
                }
            }
        }

        public byte[] Key { get; private set; }
        public IDictionary<byte[], IHBaseCellData> Columns { get; private set; }
    }
}
