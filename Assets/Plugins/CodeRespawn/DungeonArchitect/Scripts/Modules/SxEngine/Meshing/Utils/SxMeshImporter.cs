using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.SxEngine.Utils
{
    public class SxMeshImporter
    {
        public static void Import(Mesh unityMesh, SxMesh sxMesh)
        {
            if (unityMesh == null) return;
            
            sxMesh.ClearAllSections();
            for (int subMeshIdx = 0; subMeshIdx < unityMesh.subMeshCount; subMeshIdx++)
            {
                var subMesh = unityMesh.GetSubMesh(subMeshIdx);
                int drawMode = 0;
                if (subMesh.topology == MeshTopology.Quads)
                {
                    drawMode = GL.QUADS;
                }
                else if (subMesh.topology == MeshTopology.Triangles)
                {
                    drawMode = GL.TRIANGLES;
                }
                else
                {
                    continue;
                }

                var vertices = new List<SxMeshVertex>();
                var indices = unityMesh.GetIndices(subMeshIdx);
                foreach (var index in indices)
                {
                    var normal = index < unityMesh.normals.Length ? unityMesh.normals[index] : Vector3.zero;
                    normal = normal * 0.5f + new Vector3(0.5f, 0.5f, 0.5f);
                    var vertex = new SxMeshVertex();
                    vertex.Position = unityMesh.vertices[index];
                    vertex.Color = new Color(normal.x, normal.y, normal.z, 1); 
                    vertex.UV0 = index < unityMesh.uv.Length ? unityMesh.uv[index] : Vector2.zero; 
                    vertices.Add(vertex);
                }
                sxMesh.CreateSection(subMeshIdx, drawMode, vertices.ToArray());
            }
        }
    }
}