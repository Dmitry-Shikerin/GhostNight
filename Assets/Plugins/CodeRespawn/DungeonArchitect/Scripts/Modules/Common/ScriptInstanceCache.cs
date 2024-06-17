using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Utils
{
    public class ScriptInstanceCache<T> where T : ScriptableObject
    {
        private Dictionary<string, T> _scriptCache = new Dictionary<string, T>();

        public T GetScript(string typePath)
        {
            T script = null;
            if (_scriptCache.ContainsKey(typePath))
            {
                script = _scriptCache[typePath];
            }

            if (script == null)
            {
                var type = System.Type.GetType(typePath);
                if (type != null && type.IsSubclassOf(typeof(T)))
                {
                    var obj = ScriptableObject.CreateInstance(type); 
                    script = obj as T;
                }

                if (script != null)
                {
                    _scriptCache[typePath] = script;
                }
            }

            return script;
        }
        
        public void Release()
        {
            foreach (var entry in _scriptCache)
            {
                if (entry.Value != null)
                {
                    if (Application.isPlaying)
                    {
                        Object.Destroy(entry.Value);
                    }
                    else
                    {
                        Object.DestroyImmediate(entry.Value);
                    }
                }
            }
        }
    }
}