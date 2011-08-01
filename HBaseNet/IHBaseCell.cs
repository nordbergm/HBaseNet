using System.Collections.Generic;

namespace HBaseNet
{
    public interface IHBaseCell
    {
        byte[] Value { get; }
        IList<IHBaseCellVersion> Versions { get; }
    }
}
