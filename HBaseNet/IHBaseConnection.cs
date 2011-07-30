using System;
using System.Collections.Generic;

namespace HBaseNet
{
    public interface IHBaseConnection : IDisposable
    {
        void Open();
        bool IsOpen { get; }
        void Close();

        IList<string> GetTables();
    }
}
