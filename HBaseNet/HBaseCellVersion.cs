using System;

namespace HBaseNet
{
    public class HBaseCellVersion : IHBaseCellVersion
    {
        public HBaseCellVersion(long timestamp, byte[] value)
        {
            Timestamp = DateTime.FromBinary(timestamp);
            Value = value;
        }

        #region Implementation of IHBaseCellVersion

        public DateTime Timestamp { get; private set; }
        public byte[] Value { get; private set; }

        #endregion
    }
}
