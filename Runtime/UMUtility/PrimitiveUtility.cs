﻿using UM.Runtime.UMUtility.CollectionUtility.CustomCollections;
using UnityEngine;

namespace UM.Runtime.UMUtility
{
    public static class PrimitiveUtility
    {
        private static Dict<PrimitiveType, Mesh> primitiveMeshes = new Dict<PrimitiveType, Mesh>();

        public static GameObject CreatePrimitive(PrimitiveType type, bool withCollider)
        {
            if (withCollider) return GameObject.CreatePrimitive(type);

            GameObject gameObject = new GameObject(type.ToString());
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = PrimitiveUtility.GetPrimitiveMesh(type);
            gameObject.AddComponent<MeshRenderer>();

            return gameObject;
        }

        public static Mesh GetPrimitiveMesh(PrimitiveType type)
        {
            if (!primitiveMeshes.ContainsKey(type)) 
                CreatePrimitiveMesh(type);

            return primitiveMeshes[type];
        }

        private static Mesh CreatePrimitiveMesh(PrimitiveType type)
        {
            GameObject gameObject = GameObject.CreatePrimitive(type);
            Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
            GameObject.DestroyImmediate(gameObject);

            primitiveMeshes[type] = mesh;

            return mesh;
        }
    }
}