using System.Collections.Generic;
using System.Linq;
using Thrift.HBase;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseRowData : IHBaseRowData
    {
        public HBaseRowData(TRowResult rowResult)
        {
            Key = rowResult.Row;
            Columns = new Dictionary<byte[], IList<IHBaseCellData>>();

            foreach (var column in rowResult.Columns)
            {
                Columns.Add(column.Key, new List<IHBaseCellData> { new HBaseCellData(column.Value) });   
            }
        }

        public HBaseRowData(IList<TRowResult> rowResult)
        {
            Key = rowResult[0].Row;
            Columns = new Dictionary<byte[], IList<IHBaseCellData>>();

            foreach (var column in rowResult.SelectMany(r => r.Columns))
            {
                if (Columns.ContainsKey(column.Key))
                {
                    Columns[column.Key].Add(new HBaseCellData(column.Value));
                }
                else
                {
                    Columns.Add(column.Key, new List<IHBaseCellData> { new HBaseCellData(column.Value)  });
                }
            }
        }

        public byte[] Key { get; private set; }
        public IDictionary<byte[], IList<IHBaseCellData>> Columns { get; private set; }
    }
}
