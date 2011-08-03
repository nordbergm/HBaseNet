
namespace HBaseNet.Mutations
{
    public class DeleteColumnValueMutation : HBaseMutationBase
    {
        public DeleteColumnValueMutation(byte[] row, byte[] column, byte[] value) : base(row, column)
        {
            this.Value = value;
            this.IsDelete = true;
        }
    }
}
