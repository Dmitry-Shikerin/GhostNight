//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace DungeonArchitect.SxEngine
{
    public struct SxRenderContext
    {
        public Vector3 CameraPosition;
    }

    public class SxRenderCommand : IComparable<SxRenderCommand>
    {
        public Matrix4x4 AccumWorldTransform; 
        public SxMesh Mesh;
        public SxMaterial Material;
        private float distanceSqToCam = 0;

        public SxRenderCommand(Matrix4x4 accumWorldTransform, SxMesh mesh, SxMaterial material)
        {
            AccumWorldTransform = accumWorldTransform;
            Mesh = mesh;
            Material = material;
        }

        public int CompareTo(SxRenderCommand b)
        {
            var a = this;
            var queueA = a.Material != null ? a.Material.RenderQueue : 0;
            var queueB = b.Material != null ? b.Material.RenderQueue : 0;
            if (queueA == queueB)
            {
                var depthBiasA = a.Material != null ? a.Material.DepthBias : 0;
                var depthBiasB = b.Material != null ? b.Material.DepthBias : 0;
                var da = a.distanceSqToCam + depthBiasA;
                var db = b.distanceSqToCam + depthBiasB;
                if (da == db) return 0;
                return da > db ? -1 : 1;
            }
            else
            {
                return queueA < queueB ? -1 : 1;
            }
        }

        public void UpdateDistanceToCam(Vector3 camLocation)
        {
            distanceSqToCam = (Matrix.GetTranslationDivW(ref AccumWorldTransform) - camLocation).sqrMagnitude;
        }
    }

    public class SxRenderCommandList
    {   
        private List<SxRenderCommand> renderCommands = new List<SxRenderCommand>();
        public SxRenderCommand[] Commands
        {
            get => renderCommands.ToArray();
        }
        
        public void Add(SxRenderCommand command)
        {
            renderCommands.Add(command);
        }

        public void Sort(Vector3 camLocation)
        {
            UpdateDistanceFromCam(camLocation);
            renderCommands.Sort();
        }

        private void UpdateDistanceFromCam(Vector3 camLocation)
        {
            foreach (var command in renderCommands)
            {
                command.UpdateDistanceToCam(camLocation);
            }
        }
    }
    
    public class SxRenderer 
    {
        class ClearState
        {
            public bool ClearDepth = false;
            public bool ClearColor = false;
            public Color Color = Color.black;
        }
        
        public RenderTexture Texture { get; private set; }
        public SxCamera Camera { get; } = new SxCamera();

        public Matrix4x4 ViewMatrix => Camera.ViewMatrix;
        
        private ClearState clearState = new ClearState();

        public bool SortRenderCommands { get; set; } = true;

        public int BatchCount { get; private set; } = 0;
        
        public delegate void DrawDelegate(SxRenderContext context);

        public event DrawDelegate Draw;

        public void SetClearState(bool clearDepth, bool clearColor, Color color)
        {
            clearState.ClearDepth = clearDepth;
            clearState.ClearColor = clearColor;
            clearState.Color = color;
        }

        public SxRenderContext CreateRenderContext()
        {
            return new SxRenderContext
            {
                CameraPosition = Camera.Location
            };
        }
        
        public void Render(Vector2 size, SxWorld world)
        {
            AcquireTexture(size);
            
            var oldRTT = RenderTexture.active; 
            RenderTexture.active = Texture;
            
            GL.PushMatrix();
            GL.LoadProjectionMatrix(Camera.ProjectionMatrix);

            if (clearState.ClearColor || clearState.ClearDepth)
            {
                GL.Clear(clearState.ClearDepth, clearState.ClearColor, clearState.Color);
            }
            
            var context = CreateRenderContext();
            
            var renderCommandList = new SxRenderCommandList();
            world.Draw(context, renderCommandList);

            if (SortRenderCommands)
            {
                renderCommandList.Sort(context.CameraPosition);
            }
            
            Render(renderCommandList, Camera.ViewMatrix);
            
            if (Draw != null)
            {                
                Draw.Invoke(context);
            }
            
            GL.PopMatrix();
            
            RenderTexture.active = oldRTT;
        }
        
        public void Release()
        {
            ReleaseTexture();
        }
        
        private void AcquireTexture(Vector2 size)
        {
            var width = Mathf.RoundToInt(size.x);
            var height = Mathf.RoundToInt(size.y);
            if (Texture != null && (Texture.width != width || Texture.height != height))
            {
                ReleaseTexture();
            }
            
            if (Texture == null)
            {
                Texture = new RenderTexture(Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.y), 16, RenderTextureFormat.ARGB32);
                var textureCreated = Texture.Create();
                if (textureCreated)
                {
                    Camera.SetAspectRatio(Texture.width, Texture.height);
                }
                    
            }
        }

        private void ReleaseTexture()
        {
            Texture.Release();
            Object.DestroyImmediate(Texture);
            Texture = null;
        }
        
        public void Render(SxRenderCommandList renderCommandList, Matrix4x4 viewMatrix)
        {
            var renderQueueCommands = new Dictionary<int, List<SxRenderCommand>>();
            foreach (var command in renderCommandList.Commands)
            {
                if (command == null) continue;

                int renderQueue;
                if (command.Material != null && command.Material.UnityMaterial != null)
                {
                    renderQueue = command.Material.UnityMaterial.renderQueue;
                }
                else
                {
                    renderQueue = (int)RenderQueue.Geometry;
                }

                if (!renderQueueCommands.ContainsKey(renderQueue))
                {
                    renderQueueCommands.Add(renderQueue, new List<SxRenderCommand>());
                }
                renderQueueCommands[renderQueue].Add(command);
            }

            BatchCount = 0;
            var renderQueues = renderQueueCommands.Keys.ToList();
            renderQueues.Sort();
            foreach (var renderQueue in renderQueues)
            {
                var commands = renderQueueCommands[renderQueue];
                Material activeMaterial = null;
                int activePass = -1;
                foreach (var command in commands)
                {
                    var mat = command.Material.UnityMaterial;
                    if (mat == null) continue;
                    var passCount = mat.passCount;
                    for (int passIdx = 0; passIdx < passCount; passIdx++)
                    {
                        if (activeMaterial != mat || activePass != passIdx)
                        {
                            mat.SetPass(passIdx);
                            BatchCount++;
                            
                            activeMaterial = mat;
                            activePass = passIdx;
                        }
                        
                        GL.modelview = viewMatrix * command.AccumWorldTransform; 

                        foreach (var entry in command.Mesh.Sections)
                        {
                            var section = entry.Value;
                            GL.Begin(section.DrawMode);
                    
                            foreach (var vertex in section.Vertices)
                            {
                                GL.Color(vertex.Color);
                                GL.TexCoord(vertex.UV0);

                                var p = vertex.Position;
                                GL.Vertex3(p.x, p.y, p.z);
                            }
                    
                            GL.End();
                        }
                    }
                }
            }
        }
    }
}


