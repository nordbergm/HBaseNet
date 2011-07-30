using System;

namespace HBaseNet
{
    public class HBaseCellVersion : IHBaseCellVersion
    {
        public HBaseCellVersion(long timestamp, string value)
        {
            Timestamp = DateTime.FromBinary(timestamp);
            Value = value;
        }

        #region Implementation of IHBaseCellVersion

        public DateTime Timestamp { get; private set; }
        public string Value { get; private set; }

        #endregion
    }
}
