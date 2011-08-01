using System;
using HBaseNet.Protocols;

namespace HBaseNet
{
    public class HBaseCell : HBaseEntityBase<IHBaseCellData>
    {
        public HBaseCell(IHBaseCellData cellData, HBaseColumn column) : base(cellData)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column", "A cell must belong to a column.");
            }

            Column = column;
        }

        public DateTime Timestamp { get; private set; }
        public byte[] Value { get; private set; }
        public HBaseColumn Column { get; set; }

        #region Overrides of HBaseEntityBase<IHBaseCellData>

        protected override IHBaseCellData Read()
        {
            throw new NotImplementedException();
        }

        protected override void Load(IHBaseCellData data)
        {
            Timestamp = DateTime.FromBinary(data.Timestamp);
            Value = data.Value;
        }

        #endregion
    }
}
