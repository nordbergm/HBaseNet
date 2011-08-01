using System.Collections.Generic;
using Thrift.HBase;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseCellData : IHBaseCellData
    {
        public HBaseCellData(TCell cell)
        {
            Values = new Dictionary<long, byte[]> { { cell.Timestamp, cell.Value } };
        }

        public IDictionary<long, byte[]> Values { get; private set; }
    }
}
