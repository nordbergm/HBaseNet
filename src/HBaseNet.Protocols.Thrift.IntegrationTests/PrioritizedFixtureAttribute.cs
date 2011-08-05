using Xunit;

namespace HBaseNet.Protocols.Thrift.IntegrationTests
{
    public class PrioritizedFixtureAttribute : RunWithAttribute
    {
        public PrioritizedFixtureAttribute() : base(typeof(PrioritizedFixtureClassCommand)) { }
    }
}