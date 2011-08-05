using System.Text;
using HBaseNet.Mutations;
using Xunit;

namespace HBaseNet.Tests
{
    public class MutationTests
    {
        [Fact]
        public void DeleteColumnMutationConverts()
        {
            byte[] row = Encoding.UTF8.GetBytes("r");
            byte[] column = Encoding.UTF8.GetBytes("c");

            DeleteColumnMutation mutation = new DeleteColumnMutation(row, column);
            Protocols.IHBaseMutation converted = (Protocols.IHBaseMutation) mutation;

            Assert.Equal(row, converted.Row);
            Assert.Equal(column, converted.Column);
            Assert.Equal(true, converted.IsDelete);
            Assert.Null(converted.Value);
        }

        [Fact]
        public void UpdateColumnMutationConverts()
        {
            byte[] row = Encoding.UTF8.GetBytes("r");
            byte[] column = Encoding.UTF8.GetBytes("c");
            byte[] value = Encoding.UTF8.GetBytes("v");

            UpdateColumnMutation mutation = new UpdateColumnMutation(row, column, value);
            Protocols.IHBaseMutation converted = (Protocols.IHBaseMutation)mutation;

            Assert.Equal(row, converted.Row);
            Assert.Equal(column, converted.Column);
            Assert.Equal(false, converted.IsDelete);
            Assert.Equal(value, converted.Value);
        }
    }
}
