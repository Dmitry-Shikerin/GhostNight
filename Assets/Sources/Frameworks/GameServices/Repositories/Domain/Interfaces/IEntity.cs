using System;

namespace Sources.Frameworks.GameServices.Repositories.Domain.Interfaces
{
    public interface IEntity
    {
        string Id { get; }
        Type Type { get; }
    }
}