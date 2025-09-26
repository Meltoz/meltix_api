using Application.Mappings;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Meltix.UnitTests
{
    public static class MapperFactory
    {
        public static IMapper Create()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<EntitiesToDtoProfile>();
                cfg.AddProfile<DtoToEntitiesProfile>();
            }, loggerFactory);

            return configuration.CreateMapper();
        }
    }
}
