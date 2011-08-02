namespace HBaseNet.Mutations
{
    public interface IHBaseMutation : Protocols.IHBaseMutation
    {
        new byte[] Row { get; set; }
        new byte[] Column { get; set; }
    }
}