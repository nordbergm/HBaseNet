using System;
using System.Collections.Generic;
using System.Linq;
using HBaseNet.Protocols;

namespace HBaseNet
{
    public class HBaseColumn : HBaseEntityBase<IList<IHBaseCellData>>
    {
        public HBaseColumn(byte[] name, HBaseRow row, IList<IHBaseCellData> cellData) : base(cellData)
        {
            if (name == null || name.Length == 0)
            {
                throw new ArgumentNullException("name", "A column must have a name.");
            }

            if (row == null)
            {
                throw new ArgumentNullException("row", "A column must belong to a row.");
            }

            Database = row.Database;
            Name = name;
            Row = row;
        }

        public byte[] Value { get; private set; }
        public byte[] Name { get; private set; }
        public HBaseRow Row { get; set; }
        public IList<HBaseCell> Cells { get; private set; }

        #region Overrides of HBaseEntityBase<IList<IHBaseCellData>>

        protected override IList<IHBaseCellData> Read()
        {
            return Database.Connection.GetColumn(Row.Table.Name, Row.Key, Name);
        }

        protected override void Load(IList<IHBaseCellData> data)
        {
            this.Cells = data.Select(cd => new HBaseCell(cd, this)).ToList();

            if (Cells.Count > 0)
            {
                this.Value = Cells[0].Value;
            }
        }

        #endregion
    }
}
