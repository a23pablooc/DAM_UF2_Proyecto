using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnitScripts.PlanetScripts.PlanetGeneration
{
    public class PlanetGenerator : MonoBehaviour
    {
        private float _radius;
        private const int IcosphereSubdivisions = 2;
        private Dictionary<string, ColorSetting> _colors;
        private int _amountOfContinents;
        private float _continentsMinSize;
        private float _continentsMaxSize;
        private float _minLandExtrusionHeight;
        private float _maxLandExtrusionHeight;
        private float _amountOfMountains;
        private float _mountainBaseSize;
        private float _minMountainHeight;
        private float _maxMountainHeight;
        private float _minBumpFactor;
        private float _maxBumpFactor;

        private MeshFilter _meshFilter;
        private Mesh _planetMesh;
        private List<MeshTriangle> _meshTriangles = new();
        private List<Vector3> _vertices = new();
        private TriangleHashSet _oceans;
        private TriangleHashSet _continents;
        private TriangleHashSet _continentsSides;
        private TriangleHashSet _mountains;

        private void Awake()
        {
            StartGeneration();
            
            Destroy(this);
        }

        private void StartGeneration()
        {
            LoadSettings();
            CreatePlanetGameObject();

            GenerateIcosphere();
            CalculateNeighbors();

            AddContinents();
            AddOceans();
            AddMountains();
            GenerateMesh();
        }

        private void LoadSettings()
        {
            const float minRadius = 10f;
            const float maxRadius = 15f;

            var colorSettings = new List<Dictionary<string, ColorSetting>>
            {
                new()
                {
                    { ColorSetting.BaseColor, new ColorSetting(new Color32(28, 0, 171, 255)) },
                    { ColorSetting.LandColor, new ColorSetting(new Color32(9, 87, 15, 255)) },
                    { ColorSetting.HillColor, new ColorSetting(new Color32(111, 28, 14, 255)) },
                    { ColorSetting.MountainColor, new ColorSetting(new Color32(96, 85, 85, 255)) }
                },
                new()
                {
                    { ColorSetting.BaseColor, new ColorSetting(new Color32(13, 172, 186, 255)) },
                    { ColorSetting.LandColor, new ColorSetting(new Color32(221, 152, 250, 255)) },
                    { ColorSetting.HillColor, new ColorSetting(new Color32(176, 171, 32, 255)) },
                    { ColorSetting.MountainColor, new ColorSetting(new Color32(45, 224, 48, 255)) }
                },
                new()
                {
                    { ColorSetting.BaseColor, new ColorSetting(new Color32(227, 124, 45, 255)) },
                    { ColorSetting.LandColor, new ColorSetting(new Color32(227, 168, 41, 255)) },
                    { ColorSetting.HillColor, new ColorSetting(new Color32(227, 222, 93, 255)) },
                    { ColorSetting.MountainColor, new ColorSetting(new Color32(240, 235, 213, 255)) }
                }
            };
            const int minAmountOfContinents = 10;
            const int maxAmountOfContinents = 30;
            const float continentsMinSize = 0.1f;
            const float continentsMaxSize = 0.5f;
            const float minLandExtrusionHeight = 0.01f;
            const float maxLandExtrusionHeight = 0.05f;
            const int minAmountOfMountains = 5;
            const int maxAmountOfMountains = 15;
            const float mountainBaseSize = 0.4f;
            const float minMountainHeight = 0.01f;
            const float maxMountainHeight = 0.1f;
            const float minBumpFactor = 0.99f;
            const float maxBumpFactor = 1.05f;

            _radius = Random.Range(minRadius, maxRadius);
            _colors = colorSettings[Random.Range(0, colorSettings.Count)];
            _amountOfContinents =
                Random.Range(minAmountOfContinents, maxAmountOfContinents);
            _continentsMinSize = continentsMinSize;
            _continentsMaxSize = continentsMaxSize;
            _minLandExtrusionHeight = minLandExtrusionHeight;
            _maxLandExtrusionHeight = maxLandExtrusionHeight;
            _amountOfMountains = Random.Range(minAmountOfMountains, maxAmountOfMountains);
            _mountainBaseSize = mountainBaseSize;
            _minMountainHeight = minMountainHeight;
            _maxMountainHeight = maxMountainHeight;
            _minBumpFactor = minBumpFactor;
            _maxBumpFactor = maxBumpFactor;
        }

        private void CreatePlanetGameObject()
        {
            gameObject.transform.localScale = Vector3.one * _radius;

            _meshFilter = gameObject.GetComponent<MeshFilter>();
            _planetMesh = new Mesh();
        }

        private Color FindColor(string colorName)
        {
            return _colors.TryGetValue(colorName, out var color) ? color.Color : Color.magenta;
        }


        private void AddContinents()
        {
            _continents = new TriangleHashSet();

            for (var i = 0; i < _amountOfContinents; i++)
            {
                var continentSize = Random.Range(_continentsMinSize, _continentsMaxSize);
                var addedLandmass = GetTriangles(Random.onUnitSphere, continentSize, _meshTriangles);

                _continents.UnionWith(addedLandmass);
            }

            _continents.ApplyColor(FindColor(ColorSetting.LandColor));

            _continentsSides = Extrude(_continents, Random.Range(_minLandExtrusionHeight, _maxLandExtrusionHeight));
            _continentsSides.ApplyColor(FindColor(ColorSetting.HillColor));

            foreach (var triangle in _continents)
            {
                var currentVerts = new Vector3[3];
                for (var i = 0; i < triangle.VertexIndices.Count; i++)
                {
                    currentVerts[i] = _vertices[triangle.VertexIndices[i]];
                }

                AddBumpyness(currentVerts);
                for (var i = 0; i < triangle.VertexIndices.Count; i++)
                {
                    _vertices[triangle.VertexIndices[i]] = currentVerts[i];
                }
            }
        }

        private void AddOceans()
        {
            _oceans = new TriangleHashSet();

            foreach (var triangle in _meshTriangles.Where(triangle => !_continents.Contains(triangle)))
            {
                _oceans.Add(triangle);
            }

            var ocean = new TriangleHashSet(_oceans);
            ocean.ApplyColor(FindColor(ColorSetting.BaseColor));

            var shore = Extrude(ocean, -0.02f);
            shore.ApplyColor(FindColor(ColorSetting.BaseColor));

            shore = Inset(ocean, 0.02f);
            shore.ApplyColor(FindColor(ColorSetting.BaseColor));
        }

        private void AddMountains()
        {
            for (var i = 0; i < _amountOfMountains; i++)
            {
                _mountains = GetTriangles(Random.onUnitSphere, _mountainBaseSize, _continents);
                _mountains.ApplyColor(FindColor(ColorSetting.HillColor));
                _continents.UnionWith(_mountains);
                var sides = Extrude(_mountains, Random.Range(_minMountainHeight, _maxMountainHeight));
                sides.ApplyColor(FindColor(ColorSetting.HillColor));

                _mountains.ApplyColor(FindColor(ColorSetting.MountainColor));
                _mountains = GetTriangles(Random.onUnitSphere, _mountainBaseSize * -.33f, _continents);
                _continents.UnionWith(_mountains);
                Extrude(_mountains, Random.Range(_minMountainHeight, _maxMountainHeight));
                _mountains.ApplyColor(FindColor(ColorSetting.MountainColor));

                _mountains = GetTriangles(Random.onUnitSphere, _mountainBaseSize * -.66f, _continents);
                _continents.UnionWith(_mountains);
                Extrude(_mountains, Random.Range(_minMountainHeight, _maxMountainHeight));
                _mountains.ApplyColor(FindColor(ColorSetting.MountainColor));
            }
        }

        private void GenerateIcosphere()
        {
            var t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

            _vertices = new List<Vector3>
            {
                new Vector3(-1, t, 0).normalized,
                new Vector3(1, t, 0).normalized,
                new Vector3(-1, -t, 0).normalized,
                new Vector3(1, -t, 0).normalized,
                new Vector3(0, -1, t).normalized,
                new Vector3(0, 1, t).normalized,
                new Vector3(0, -1, -t).normalized,
                new Vector3(0, 1, -t).normalized,
                new Vector3(t, 0, -1).normalized,
                new Vector3(t, 0, 1).normalized,
                new Vector3(-t, 0, -1).normalized,
                new Vector3(-t, 0, 1).normalized
            };

            _meshTriangles = new List<MeshTriangle>
            {
                new(0, 11, 5),
                new(0, 5, 1),
                new(0, 1, 7),
                new(0, 7, 10),
                new(0, 10, 11),
                new(1, 5, 9),
                new(5, 11, 4),
                new(11, 10, 2),
                new(10, 7, 6),
                new(7, 1, 8),
                new(3, 9, 4),
                new(3, 4, 2),
                new(3, 2, 6),
                new(3, 6, 8),
                new(3, 8, 9),
                new(4, 9, 5),
                new(2, 4, 11),
                new(6, 2, 10),
                new(8, 6, 7),
                new(9, 8, 1)
            };

            Subdivide();
        }

        private void Subdivide()
        {
            var midPointCache = new Dictionary<int, int>();

            for (var i = 0; i < IcosphereSubdivisions; i++)
            {
                var newPolys = new List<MeshTriangle>();
                foreach (var poly in _meshTriangles)
                {
                    var a = poly.VertexIndices[0];
                    var b = poly.VertexIndices[1];
                    var c = poly.VertexIndices[2];

                    var ab = GetMidPointIndex(midPointCache, a, b);
                    var bc = GetMidPointIndex(midPointCache, b, c);
                    var ca = GetMidPointIndex(midPointCache, c, a);

                    newPolys.Add(new MeshTriangle(a, ab, ca));
                    newPolys.Add(new MeshTriangle(b, bc, ab));
                    newPolys.Add(new MeshTriangle(c, ca, bc));
                    newPolys.Add(new MeshTriangle(ab, bc, ca));
                }

                _meshTriangles = newPolys;
            }
        }

        private int GetMidPointIndex(IDictionary<int, int> cache, int indexA, int indexB)
        {
            var smallerIndex = Mathf.Min(indexA, indexB);
            var greaterIndex = Mathf.Max(indexA, indexB);
            var key = (smallerIndex << 16) + greaterIndex;

            // If a midpoint is already defined, just return it.

            if (cache.TryGetValue(key, out var ret))
                return ret;

            // If we're here, it's because a midpoint for these two
            // vertices hasn't been created yet. Let's do that now!

            var p1 = _vertices[indexA];
            var p2 = _vertices[indexB];
            var middle = Vector3.Lerp(p1, p2, 0.5f).normalized;

            ret = _vertices.Count;
            _vertices.Add(middle);

            // Add our new midpoint to the cache so we don't have
            // to do this again. =)

            cache.Add(key, ret);
            return ret;
        }

        private void CalculateNeighbors()
        {
            foreach (var poly in _meshTriangles)
            {
                foreach (var otherPoly in _meshTriangles.Where(otherPoly => poly != otherPoly)
                             .Where(otherPoly => poly.IsNeighbouring(otherPoly)))
                {
                    poly.Neighbours.Add(otherPoly);
                }
            }
        }

        private List<int> CloneVertices(List<int> oldVerts)
        {
            var newVerts = new List<int>();
            foreach (var clonedVert in oldVerts.Select(oldVert => _vertices[oldVert]))
            {
                newVerts.Add(_vertices.Count);
                _vertices.Add(clonedVert);
            }

            return newVerts;
        }

        private TriangleHashSet StitchPolys(TriangleHashSet polys, out BorderHashSet stitchedEdge)
        {
            var stichedPolys = new TriangleHashSet
            {
                IterationIndex = _vertices.Count
            };

            stitchedEdge = polys.CreateBorderHashSet();
            var originalVerts = stitchedEdge.RemoveDuplicates();
            var newVerts = CloneVertices(originalVerts);

            stitchedEdge.Separate(originalVerts, newVerts);

            foreach (var edge in stitchedEdge)
            {
                // Create new polys along the stitched edge. These
                // will connect the original poly to its former
                // neighbor.

                var stitchPoly1 = new MeshTriangle(edge.OuterVertices[0],
                    edge.OuterVertices[1],
                    edge.InnerVertices[0]);
                var stitchPoly2 = new MeshTriangle(edge.OuterVertices[1],
                    edge.InnerVertices[1],
                    edge.InnerVertices[0]);
                // Add the new stitched faces as neighbors to
                // the original Polys.
                edge.InnerTriangle.UpdateNeighbour(edge.OuterTriangle, stitchPoly2);
                edge.OuterTriangle.UpdateNeighbour(edge.InnerTriangle, stitchPoly1);

                _meshTriangles.Add(stitchPoly1);
                _meshTriangles.Add(stitchPoly2);

                stichedPolys.Add(stitchPoly1);
                stichedPolys.Add(stitchPoly2);
            }

            //Swap to the new vertices on the inner polys.
            foreach (var poly in polys)
            {
                for (var i = 0; i < 3; i++)
                {
                    var vertID = poly.VertexIndices[i];
                    if (!originalVerts.Contains(vertID))
                        continue;
                    var vertIndex = originalVerts.IndexOf(vertID);
                    poly.VertexIndices[i] = newVerts[vertIndex];
                }
            }

            return stichedPolys;
        }

        private TriangleHashSet Extrude(TriangleHashSet polys, float height)
        {
            var stitchedPolys = StitchPolys(polys, out _);
            var verts = polys.RemoveDuplicates();

            // Take each vertex in this list of polys, and push it
            // away from the center of the Planet by the height
            // parameter.

            foreach (var vert in verts)
            {
                var v = _vertices[vert];
                v = v.normalized * (v.magnitude + height);
                _vertices[vert] = v;
            }

            return stitchedPolys;
        }

        private TriangleHashSet Inset(TriangleHashSet polys, float insetDistance)
        {
            var stitchedPolys = StitchPolys(polys, out var stitchedEdge);

            var inwardDirections = stitchedEdge.GetInwardDirections(_vertices);

            // Push each vertex inwards, then correct
            // it's height so that it's as far from the center of
            // the planet as it was before.

            foreach (var (vertIndex, inwardDirection) in inwardDirections)
            {
                var vertex = _vertices[vertIndex];
                var originalHeight = vertex.magnitude;

                vertex += inwardDirection * insetDistance;
                vertex = vertex.normalized * originalHeight;
                _vertices[vertIndex] = vertex;
            }

            return stitchedPolys;
        }

        private TriangleHashSet GetTriangles(Vector3 center, float rad, IEnumerable<MeshTriangle> source)
        {
            var newSet = new TriangleHashSet();

            foreach (var p in source)
            {
                if (p.VertexIndices.Select(vertexIndex => Vector3.Distance(center, _vertices[vertexIndex]))
                    .Any(distanceToSphere => distanceToSphere <= rad))
                {
                    newSet.Add(p);
                }
            }

            return newSet;
        }

        private void GenerateMesh()
        {
            var vertexCount = _meshTriangles.Count * 3;

            var indices = new int[vertexCount];

            var vertices = new Vector3[vertexCount];
            var normals = new Vector3[vertexCount];
            var colors2 = new Color32[vertexCount];
            var uvs = new Vector2[vertexCount];

            for (var i = 0; i < _meshTriangles.Count; i++)
            {
                var poly = _meshTriangles[i];

                indices[i * 3 + 0] = i * 3 + 0;
                indices[i * 3 + 1] = i * 3 + 1;
                indices[i * 3 + 2] = i * 3 + 2;

                vertices[i * 3 + 0] = _vertices[poly.VertexIndices[0]];
                vertices[i * 3 + 1] = _vertices[poly.VertexIndices[1]];
                vertices[i * 3 + 2] = _vertices[poly.VertexIndices[2]];

                uvs[i * 3 + 0] = poly.UVs[0];
                uvs[i * 3 + 1] = poly.UVs[1];
                uvs[i * 3 + 2] = poly.UVs[2];

                colors2[i * 3 + 0] = poly.Color;
                colors2[i * 3 + 1] = poly.Color;
                colors2[i * 3 + 2] = poly.Color;

                var ab = _vertices[poly.VertexIndices[1]] - _vertices[poly.VertexIndices[0]];
                var ac = _vertices[poly.VertexIndices[2]] - _vertices[poly.VertexIndices[0]];

                var normal = Vector3.Cross(ab, ac).normalized;

                normals[i * 3 + 0] = normal;
                normals[i * 3 + 1] = normal;
                normals[i * 3 + 2] = normal;
            }

            _planetMesh.vertices = vertices;
            _planetMesh.normals = normals;
            _planetMesh.colors32 = colors2;
            _planetMesh.uv = uvs;

            _planetMesh.SetTriangles(indices, 0);

            _meshFilter.mesh = _planetMesh;
        }

        private void AddBumpyness(Vector3[] verts)
        {
            var dictionary = new Dictionary<Vector3, List<int>>();
            for (var x = 0; x < verts.Length; x++)
            {
                if (!dictionary.ContainsKey(verts[x]))
                {
                    dictionary.Add(verts[x], new List<int>());
                }

                dictionary[verts[x]].Add(x);
            }

            foreach (var (key, value) in dictionary)
            {
                var newPos = key * Random.Range(_minBumpFactor, _maxBumpFactor);
                foreach (var i in value)
                {
                    verts[i] = newPos;
                }
            }
        }
    }
}