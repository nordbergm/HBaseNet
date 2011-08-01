using System.Collections.Generic;

namespace HBaseNet.Protocols
{
    public interface IHBaseCellData
    {
        IDictionary<long, byte[]> Values { get; }
    }
}
