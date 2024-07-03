using UnityEngine;

namespace Sources.Frameworks.GameServices.Prefabs.Interfaces
{
    public interface IPrefabLoader
    {
        T Load<T>(string path) where T : Object;
    }
}