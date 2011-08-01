using System;

namespace HBaseNet
{
    public interface IHBaseCellVersion
    {
        DateTime Timestamp { get; }
        byte[] Value { get; }
    }
}
