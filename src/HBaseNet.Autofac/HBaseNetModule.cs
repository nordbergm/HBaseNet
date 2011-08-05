using Autofac;
using HBaseNet.Protocols.Thrift;

namespace HBaseNet.Autofac
{
    public class HBaseNetModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new HBaseThriftConnection("192.168.10.27", 9090)).As<IHBaseConnection>();
        }
    }
}
