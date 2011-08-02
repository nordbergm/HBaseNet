namespace HBaseNet.Mutations
{
    public class DeleteColumnMutation : HBaseMutationBase
    {
        public DeleteColumnMutation(byte[] row, byte[] column) : base(row, column)
        {
            this.IsDelete = true;
        }
    }
}
