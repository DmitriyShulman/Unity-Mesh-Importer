using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MeshExtensions.Editor
{
    public class MeshImporter : AssetPostprocessor
    {
        #region Variables
        
        private UserData _userData;

        #endregion

        #region Unity Methods

        private void OnPostprocessModel(GameObject obj)
        {
            if (assetImporter.userData == string.Empty) return;
            _userData = JsonUtility.FromJson<UserData>(assetImporter.userData);
            if (_userData.meshFunctions == null || _userData.meshFunctions.Length == 0) return;
            
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
            List<MeshModifier> functions = _userData.meshFunctions.ToList();
            
            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (functions.FindIndex(f => f.id == meshFilter.sharedMesh.name) == -1) continue;
                
                List<MeshModifier> currentFunctions = functions.FindAll(f => f.id == meshFilter.sharedMesh.name);
                foreach (MeshModifier currentFunction in currentFunctions)
                {
                    switch (currentFunction.modifier)
                    {
                        case Modifier.Combine: Combine(meshFilter, currentFunction); break;
                        case Modifier.Manual: Manual(meshFilter, currentFunction); break;
                        case Modifier.Mesh: FromMesh(meshFilter, currentFunction); break;
                        case Modifier.Bounds: Bounds(meshFilter, currentFunction); break;
                    }
                }
            }
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        #endregion

        #region Private Methods

        private void Combine(MeshFilter m, MeshModifier f)
        {
            Mesh mesh = m.sharedMesh;

            List<Vector4> mainPoints = new List<Vector4>(), failPoints = new List<Vector4>();
            
            mesh.GetUVs((int)f.uVs[0], mainPoints);
            mesh.GetUVs((int)f.uVs[1], failPoints);

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                mainPoints[i] = new Vector4(mainPoints[i].x, mainPoints[i].y, failPoints[i].x, failPoints[i].y);
            }
            
            mesh.SetUVs((int)f.uVs[0], mainPoints);
            mesh.SetUVs((int)f.uVs[1], new Vector4[0]);
        }

        private void Manual(MeshFilter m, MeshModifier f)
        {
            Mesh mesh = m.sharedMesh;

            if (f.entities[0] == Entity.Position)
            {
                List<Vector3> points = new List<Vector3>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(f.points[0]);
                }
                mesh.SetVertices(points);
            }
            else if (f.entities[0] == Entity.Normal)
            {
                List<Vector3> points = new List<Vector3>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(f.points[0]);
                }
                mesh.SetNormals(points);
            }
            else if (f.entities[0] == Entity.Tangent)
            {
                List<Vector4> points = new List<Vector4>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(f.points[0]);
                }
                mesh.SetTangents(points);
            }
            else if (f.entities[0] == Entity.Color)
            {
                List<Color> colors = new List<Color>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    colors.Add(new Color(f.points[0].x, f.points[0].y, f.points[0].z, f.points[0].w));
                }
                mesh.SetColors(colors);
            }
            else if (f.entities[0] == Entity.UV)
            {
                List<Vector4> points = new List<Vector4>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(f.points[0]);
                }
                mesh.SetUVs((int) f.uVs[0], points);
            }
        }

        private void FromMesh(MeshFilter m, MeshModifier f)
        {
            Mesh mesh = m.sharedMesh;
            if (f.objects[0] == null) return;
            Mesh anotherMesh = (Mesh)f.objects[0];
            if (anotherMesh == null) return;
            
            List<Vector4> anotherPoints = new List<Vector4>();

            if (f.entities[1] == Entity.Position)
            {
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    anotherPoints.Add(anotherMesh.vertices[i]);
                }
            }
            else if (f.entities[1] == Entity.Normal)
            {
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    anotherPoints.Add(anotherMesh.normals[i]);
                }
            }
            else if (f.entities[1] == Entity.Tangent)
            {
                anotherMesh.GetTangents(anotherPoints);
            }
            else if (f.entities[1] == Entity.Color)
            {
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    anotherPoints.Add(new Vector4(
                        anotherMesh.colors[i].r,
                        anotherMesh.colors[i].g,
                        anotherMesh.colors[i].b,
                        anotherMesh.colors[i].a));
                }
            }
            else if (f.entities[1] == Entity.UV)
            {
                anotherMesh.GetUVs((int) f.uVs[1], anotherPoints);
            }
            
            if (anotherPoints.Count < mesh.vertexCount) return;

            if (f.entities[0] == Entity.Position)
            {
                List<Vector3> points = new List<Vector3>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(anotherPoints[i]);
                }
                mesh.SetVertices(points);
            }
            else if (f.entities[0] == Entity.Normal)
            {
                List<Vector3> points = new List<Vector3>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(anotherPoints[i]);
                }
                mesh.SetNormals(points);
            }
            else if (f.entities[0] == Entity.Tangent)
            {
                List<Vector4> points = new List<Vector4>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(anotherPoints[i]);
                }
                mesh.SetTangents(points);
            }
            else if (f.entities[0] == Entity.Color)
            {
                List<Color> colors = new List<Color>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    colors.Add(new Color(
                        anotherPoints[i].x, 
                        anotherPoints[i].y, 
                        anotherPoints[i].z, 
                        anotherPoints[i].w));
                }
                mesh.SetColors(colors);
            }
            else if (f.entities[0] == Entity.UV)
            {
                List<Vector4> points = new List<Vector4>();
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    points.Add(anotherPoints[i]);
                }
                mesh.SetUVs((int) f.uVs[0], points);
            }
        }

        private void Bounds(MeshFilter m, MeshModifier f)
        {
            Mesh mesh = m.sharedMesh;

            mesh.bounds = new Bounds(f.points[0], f.points[1]);
        }

        #endregion
    }
}