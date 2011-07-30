using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.HBase;
using Thrift.Protocol;
using Thrift.Transport;

namespace HBaseNet.Protocols.Thrift
{
    public class HBaseThriftConnection : IHBaseConnection
    {
        public HBaseThriftConnection(string host, int port)
        {
            Transport = new TBufferedTransport(new TSocket(host, port));
            Protocol = new TBinaryProtocol(Transport);
            Client = new Hbase.Client(Protocol);
            Encoding = Encoding.UTF8;
        }

        public Hbase.Client Client { get; private set; }
        public TTransport Transport { get; private set; }
        public TProtocol Protocol { get; private set; }
        public Encoding Encoding { get; private set; }

        public bool IsOpen
        {
            get { return Transport.IsOpen; }
        }

        public void Open()
        {
            Transport.Open();
        }

        public void Close()
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("The connection is already closed.");
            }

            Transport.Close();
        }

        public void Dispose()
        {
            if (IsOpen)
            {
                this.Close();
            }
        }

        public IList<string> GetTables()
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("Connection must be open to retrieve tables.");
            }

            return Client.getTableNames().Select(t => Encoding.GetString(t)).ToList();
        }
    }
}
