using Rhino.Mocks;
using Xunit;

namespace HBaseNet.Tests
{
    public class HBaseEntityBaseTests
    {
        [Fact]
        public void LoadsOnLoad()
        {
            MockRepository mockRepository = new MockRepository();
            Mock mockedEntity = mockRepository.PartialMock<Mock>();

            using (mockRepository.Record())
            {
                Expect.Call(() => mockedEntity.LoadMocked("read")).Repeat.Once();
            }

            using (mockRepository.Playback())
            {
                mockedEntity.Load();
            }
        }

        public class Mock : HBaseEntityBase<string>
        {
            public virtual string ReadMocked()
            {
                return "read";
            }

            public virtual void LoadMocked(string s)
            {
            }

            #region Overrides of HBaseEntityBase<object>

            protected override string Read()
            {
                return this.ReadMocked();
            }

            protected override void Load(string data)
            {
                this.LoadMocked(data);
            }

            #endregion
        }
    }
}
