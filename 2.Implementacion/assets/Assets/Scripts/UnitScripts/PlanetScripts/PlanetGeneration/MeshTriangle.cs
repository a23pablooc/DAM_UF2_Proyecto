using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnitScripts.PlanetScripts.PlanetGeneration
{
    public class MeshTriangle
    {
        public readonly List<int> VertexIndices;
        public readonly List<Vector2> UVs;
        public readonly List<MeshTriangle> Neighbours;
        public Color Color;

        public MeshTriangle(int vertexIndexA, int vertexIndexB, int vertexIndexC)
        {
            VertexIndices = new List<int>() { vertexIndexA, vertexIndexB, vertexIndexC };
            UVs = new List<Vector2> { Vector2.zero, Vector2.zero, Vector2.zero };
            Neighbours = new List<MeshTriangle>();
        }

        public bool IsNeighbouring(MeshTriangle other)
        {
            var sharedVertices = VertexIndices.Count(index => other.VertexIndices.Contains(index));

            return sharedVertices > 1;
        }

        public void UpdateNeighbour(MeshTriangle initialNeighbour, MeshTriangle newNeighbour)
        {
            for (var i = 0; i < Neighbours.Count; i++)
            {
                if (initialNeighbour != Neighbours[i]) continue;
                Neighbours[i] = newNeighbour;
                return;
            }
        }
    }
}