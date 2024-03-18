using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnitScripts.PlanetScripts.PlanetGeneration
{
    public class BorderHashSet : HashSet<TriangleBorder>
    {
        public void Separate(List<int> originalVertices, List<int> addedVertices)
        {
            foreach (var border in this)
            {
                for (var i = 0; i < 2; i++)
                {
                    border.InnerVertices[i] = addedVertices[originalVertices.IndexOf(border.OuterVertices[i])];
                }
            }
        }

        public List<int> RemoveDuplicates()
        {
            var vertices = new List<int>();
            foreach (var vertexIndex in this.SelectMany(border =>
                         border.OuterVertices.Where(vertexIndex => !vertices.Contains(vertexIndex))))
            {
                vertices.Add(vertexIndex);
            }

            return vertices;
        }

        public Dictionary<int, Vector3> GetInwardDirections(List<Vector3> vertexPositions)
        {
            var inwardDirections = new Dictionary<int, Vector3>();
            var numItems = new Dictionary<int, int>();

            foreach (var border in this)
            {
                var innerVertexPosition = vertexPositions[border.InwardDirectionVertex];
                var borderPosA = vertexPositions[border.InnerVertices[0]];
                var borderPosB = vertexPositions[border.InnerVertices[1]];
                var borderCenter = Vector3.Lerp(borderPosA, borderPosB, 0.5f);
                var innerVector = (innerVertexPosition - borderCenter).normalized;

                for (var i = 0; i < 2; i++)
                {
                    var borderVertex = border.InnerVertices[i];
                    if (!inwardDirections.TryAdd(borderVertex, innerVector))
                    {
                        inwardDirections[borderVertex] += innerVector;
                        numItems[borderVertex]++;
                    }
                    else
                    {
                        numItems.Add(borderVertex, 1);
                    }
                }
            }

            foreach (var (vertexIndex, contributionsToThisVertex) in numItems)
            {
                inwardDirections[vertexIndex] = (inwardDirections[vertexIndex] / contributionsToThisVertex).normalized;
            }

            return inwardDirections;
        }
    }
}