
namespace HBaseNet.Protocols
{
    public interface IHBaseCellData
    {
        long Timestamp { get; }
        byte[] Value { get; }
    }
}
