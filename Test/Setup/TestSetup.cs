using AutoMapper;
using Moq;

namespace Test.Setup
{
    public class TestSetup
    {
        protected readonly IMapper _mapperMock;

        internal TestSetup()
        {
            var autoMapperProfile = new Application.AutoMapper.AutoMapper();
            var configuration = new MapperConfiguration(m => m.AddProfile(autoMapperProfile));

            _mapperMock = new Mapper(configuration);
        }

        protected static T Any<T>() => It.IsAny<T>();
        protected static Mock<T> CreateMock<T>() where T : class => new();
    }
}
