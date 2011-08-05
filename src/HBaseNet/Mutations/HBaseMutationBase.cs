
namespace HBaseNet.Mutations
{
    public abstract class HBaseMutationBase : IHBaseMutation
    {
        protected HBaseMutationBase(byte[] row, byte[] column)
        {
            Row = row;
            Column = column;
        }

        public byte[] Row { get; set; }
        public byte[] Column { get; set; }

        protected bool IsDelete { get; set; }
        protected byte[] Value { get; set; }

        #region Explict Implementation of Protocols.IHBaseMutation

        byte[] Protocols.IHBaseMutation.Column
        {
            get { return this.Column; }
        }

        byte[] Protocols.IHBaseMutation.Row
        {
            get { return this.Row; }
        }

        bool Protocols.IHBaseMutation.IsDelete
        {
            get { return this.IsDelete; }
        }

        byte[] Protocols.IHBaseMutation.Value
        {
            get { return this.Value; }
        }

        #endregion
    }
}
