using Thrift.HBase;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseCellData : IHBaseCellData
    {
        public HBaseCellData(TCell cell)
        {
            Timestamp = cell.Timestamp;
            Value = cell.Value;
        }

        public long Timestamp { get; private set; }
        public byte[] Value { get; private set; }
    }
}
