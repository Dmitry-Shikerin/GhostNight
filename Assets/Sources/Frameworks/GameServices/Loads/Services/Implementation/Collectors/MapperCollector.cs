using System;
using System.Collections.Generic;
using Sources.Frameworks.GameServices.Loads.Domain.Data;
using Sources.Frameworks.GameServices.Loads.Services.Interfaces.Collectors;
using Sources.Frameworks.GameServices.Repositories.Domain.Interfaces;

namespace Sources.Frameworks.GameServices.Loads.Services.Implementation.Collectors
{
    public class MapperCollector : IMapperCollector
    {
        private readonly Dictionary<Type, Func<IEntity, IDto>> _toDtoMappers;
        private readonly Dictionary<Type, Func<IDto, IEntity>> _toModelMappers;

        public MapperCollector()
        {
            _toDtoMappers = new Dictionary<Type, Func<IEntity, IDto>>();

            _toModelMappers = new Dictionary<Type, Func<IDto, IEntity>>();
            
        }

        public Func<IEntity, IDto> GetToDtoMapper(Type type)
        {
            if(_toDtoMappers.ContainsKey(type) == false)
                throw new NullReferenceException($"DtaModel Id {type} not registered in MapperCollector.");
            
            return _toDtoMappers[type];
        }

        public Func<IDto, IEntity> GetToModelMapper(Type type)
        {
            if(_toModelMappers.ContainsKey(type) == false)
                throw new NullReferenceException($"DtaModel Id {type} not registered in MapperCollector.");
            
            return _toModelMappers[type];
        }
    }
}