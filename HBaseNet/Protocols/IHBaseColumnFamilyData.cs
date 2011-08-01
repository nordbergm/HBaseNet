
namespace HBaseNet.Protocols
{
    public interface IHBaseColumnFamilyData
    {
        byte[] Name { get; }
        int MaxVersions { get; }
        bool BlockCacheEnabled { get; }
    }
}
