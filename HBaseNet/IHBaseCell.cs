using System.Collections.Generic;

namespace HBaseNet
{
    public interface IHBaseCell
    {
        string Value { get; }
        IList<IHBaseCellVersion> Versions { get; }
    }
}
