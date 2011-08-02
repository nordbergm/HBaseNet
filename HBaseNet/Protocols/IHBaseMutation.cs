namespace HBaseNet.Protocols
{
    public interface IHBaseMutation
    {
        byte[] Column { get; }
        byte[] Row { get; }
        bool IsDelete { get; }
        byte[] Value { get; }
    }
}
