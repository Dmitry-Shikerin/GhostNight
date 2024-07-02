using System;
using Sources.Frameworks.GameServices.Loads.Domain.Data;
using Sources.Frameworks.GameServices.Repositories.Domain.Interfaces;

namespace Sources.Frameworks.GameServices.Loads.Services.Interfaces.Collectors
{
    public interface IMapperCollector
    {
        Func<IEntity, IDto> GetToDtoMapper(Type type);
        Func<IDto, IEntity> GetToModelMapper(Type type);
    }
}