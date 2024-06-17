//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DungeonArchitect.SxEngine
{
    public abstract class SxMaterial
    {
        protected Material unityMaterial;
        
        public virtual Material UnityMaterial { get => unityMaterial; }

        public float DepthBias = 0.0f;
        public int RenderQueue
        {
            get
            {
                if (unityMaterial != null)
                {
                    return unityMaterial.renderQueue;
                }
                return 0;
            }
        }

        public virtual SxMaterial Clone()
        {
            Material clonedMat = null;
            if (unityMaterial != null)
            {
                clonedMat = Object.Instantiate(unityMaterial);
            }

            return new SxUnityMaterial(clonedMat);
        }

        public void SetFloat(string name, float value)
        {
            unityMaterial.SetFloat(name, value);
        }

        public void SetInt(string name, int value)
        {
            unityMaterial.SetInt(name, value);
        }

        public void SetColor(string name, Color value)
        {
            unityMaterial.SetColor(name, value);
        }
    }

    public class SxUnityMaterial : SxMaterial
    {
        public SxUnityMaterial(Material material)
        {
            this.unityMaterial = material;
        }
    }
    
    public abstract class SxUnityResourceMaterial : SxMaterial
    {
        private string materialPath;
        private bool materialValid = false;

        public override Material UnityMaterial
        {
            get
            {
                if (materialValid && unityMaterial == null)
                {
                    LoadMaterial(); 
                }

                return unityMaterial;
            }
        }

        public SxUnityResourceMaterial(string resourceName)
        {
            materialPath = resourceName;
            LoadMaterial();
            materialValid = (unityMaterial != null);
        }

        void LoadMaterial()
        {
            unityMaterial = Resources.Load<Material>(materialPath);
        }
        
    }
    
    
    public abstract class SxUnityResourceCopyMaterial : SxMaterial {
        private string materialPath;
        private bool materialValid = false;
        
        public override Material UnityMaterial
        {
            get
            {
                if (materialValid && unityMaterial == null)
                {
                    LoadMaterial(); 
                }

                return unityMaterial;
            }
        }

        public SxUnityResourceCopyMaterial(string resourceName)
        {
            materialPath = resourceName;
            LoadMaterial();
            materialValid = (unityMaterial != null);
        }
        
        void LoadMaterial()
        {
            var template = Resources.Load<Material>(materialPath);
            if (template != null)
            {
                unityMaterial = new Material(template);
            }
        }
    }
    
    public class SxMaterialRegistry
    {
        private static Dictionary<System.Type, SxMaterial> cache = new Dictionary<Type, SxMaterial>();

        public static SxMaterial Get<T>() where T : SxMaterial, new()
        {
            if (cache.ContainsKey(typeof(T)))
            {
                return cache[typeof(T)];
            }

            var material = new T();
            cache.Add(typeof(T), material);
            return material;
        }
    }
    
    public class SxDefaultMaterial : SxMaterial
    {
        public SxDefaultMaterial()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            unityMaterial = new Material(shader);
            unityMaterial.hideFlags = HideFlags.HideAndDontSave;
            unityMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
            unityMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //unityMaterial.SetInt("_Cull", (int) UnityEngine.Rendering.CullMode.Off);
            //lineMaterial.SetInt("_ZWrite", 0);
        }
    }
}