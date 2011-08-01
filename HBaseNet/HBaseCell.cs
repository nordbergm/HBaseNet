using System.Collections.Generic;
using System.Linq;
using HBaseNet.Protocols;

namespace HBaseNet
{
    public class HBaseCell : IHBaseCell
    {
        public HBaseCell(IHBaseCellData cellData)
        {
            Value = cellData.Values.Count > 0 ? cellData.Values[cellData.Values.Keys.Max()] : null;
            Versions = new List<IHBaseCellVersion>(cellData.Values.Count);

            foreach (var value in cellData.Values)
            {
                Versions.Add(new HBaseCellVersion(value.Key, value.Value));
            }
        }

        #region Implementation of IHBaseCell

        public byte[] Value { get; private set; }
        public IList<IHBaseCellVersion> Versions { get; private set; }

        #endregion
    }
}
