using System;

namespace HBaseNet
{
    public interface IHBaseCellVersion
    {
        DateTime Timestamp { get; }
        string Value { get; }
    }
}
