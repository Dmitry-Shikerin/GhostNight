using System.Collections.Generic;
using Sources.Frameworks.GameServices.Repositories.Domain.Interfaces;

namespace Sources.Frameworks.GameServices.Repositories.Services.Interfaces
{
    public interface IEntityRepository
    {
        IReadOnlyDictionary<string, IEntity> Entities { get; }
        
        void Add(IEntity entity);
        IEntity Get(string id);
        T Get<T>(string id) 
            where T : class, IEntity;
        void Release();
    }
}