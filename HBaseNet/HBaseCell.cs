using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public string Value { get; private set; }
        public IList<IHBaseCellVersion> Versions { get; private set; }

        #endregion
    }
}
