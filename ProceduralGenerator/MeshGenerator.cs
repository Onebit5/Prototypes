using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;

    public int textureWidth = 1024;
    public int textureHeight = 1024;

    public float noise01Scale = 2f;
    public float noise01Amp = 2f;

    public float noise02Scale = 4f;
    public float noise02Amp = 4f;

    public float noise03Scale = 6f;
    public float noise03Amp = 6f;

    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;

    public int seed;

    bool isGenerated = false;

    System.Random prng;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        minTerrainHeight = float.MaxValue;
        maxTerrainHeight = float.MinValue;

        seed = Random.Range(-100000, 100000);
        prng = new System.Random(seed);

    }

    private void Update()
    {
        if (!isGenerated)
        {
            CreateShape();
            UpdateMesh();
            isGenerated = true;
        }
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;

                float noise01 = (float)prng.NextDouble();
                float noise02 = (float)prng.NextDouble();
                float noise03 = (float)prng.NextDouble();

                y += Mathf.PerlinNoise(x * noise01Scale * noise01, z * noise01Scale * noise01) * noise01Amp;
                y += Mathf.PerlinNoise(x * noise02Scale * noise02, z * noise02Scale * noise02) * noise02Amp;
                y += Mathf.PerlinNoise(x * noise03Scale * noise03, z * noise03Scale * noise03) * noise03Amp;


                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y;

                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }
}
