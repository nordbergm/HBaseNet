using System.Collections.Generic;

namespace HBaseNet
{
    public interface IHBaseCellData
    {
        IDictionary<long, string> Values { get; }
    }
}
