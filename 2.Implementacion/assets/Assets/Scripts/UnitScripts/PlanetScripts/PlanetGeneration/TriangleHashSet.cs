using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnitScripts.PlanetScripts.PlanetGeneration
{
    public class TriangleHashSet : HashSet<MeshTriangle>
    {
        public int IterationIndex = -1;

        public TriangleHashSet()
        {
        }

        public TriangleHashSet(TriangleHashSet source) : base(source)
        {
        }

        public BorderHashSet CreateBorderHashSet()
        {
            var borderSet = new BorderHashSet();
            foreach (var border in this.SelectMany(triangle =>
                         from neighbor in triangle.Neighbours
                         where !this.Contains(neighbor)
                         select new TriangleBorder(triangle, neighbor)))
            {
                borderSet.Add(border);
            }

            return borderSet;
        }

        public List<int> RemoveDuplicates()
        {
            var vertices = new List<int>();
            foreach (var vertexIndex in from triangle in this
                                        from vertexIndex in triangle.VertexIndices
                                        where !vertices.Contains(vertexIndex)
                                        select vertexIndex)
            {
                vertices.Add(vertexIndex);
            }

            return vertices;
        }

        public void ApplyColor(Color color)
        {
            foreach (var triangle in this)
            {
                triangle.Color = color;
            }
        }
    }
}