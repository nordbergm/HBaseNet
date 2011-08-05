namespace HBaseNet.Mutations
{
    public class UpdateColumnMutation : HBaseMutationBase
    {
        public UpdateColumnMutation(byte[] row, byte[] column, byte[] value) : base(row, column)
        {
            this.Value = value;
        }
    }
}
